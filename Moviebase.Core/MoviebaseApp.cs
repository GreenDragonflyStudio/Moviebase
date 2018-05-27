﻿using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using Moviebase.Core.Components;
using Moviebase.Core.Utils;
using Moviebase.DAL;
using Moviebase.DAL.Entities;
using Moviebase.Services.Metadata;
using Ninject;
using TMDbLib.Client;
using TMDbLib.Objects.Find;
using TMDbLib.Objects.Search;

namespace Moviebase.Core
{
    public sealed class MoviebaseApp
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(MoviebaseApp));
        
        private readonly TMDbClient _apiClient;
        private readonly IFileScanner _fileScanner;
        private readonly IFileAnalyzer _analyzer;
        private readonly IFileOrganizer _fileOrganizer;

        public event EventHandler<MatchFoundEventArgs> MatchFound;
        public event ProgressChangedEventHandler ProgressChanged;

        public MoviebaseApp(IFileScanner fileScanner, IFileAnalyzer analyzer, IFileOrganizer organizer)
        {
            _fileScanner = fileScanner;
            _analyzer = analyzer;
            _fileOrganizer = organizer;

            _apiClient = new TMDbClient(GlobalSettings.Default.ApiKey)
            {
                DefaultLanguage = "id",
                DefaultCountry = "ID"
            };
        }
        
        public async Task<IEnumerable<Movie>> ScanAsync(string scanPath)
        {
            var files = _fileScanner.Scan(scanPath).ToList();
            var totalItems = files.Count;
            var identified = new List<Movie>();

            for (var index = 0; index < files.Count; index++)
            {
                var file = files[index];
                var item = await _analyzer.Analyze(file);

                if (!item.IsKnown)
                {
                    var res = await TryIdentify(item);
                    if (res == null) continue;

                    var args = new MatchFoundEventArgs(file, item.Title, res.Item2);
                    OnMatchFound(args);
                    if (args.Cancel) break;

                    identified.Add(res.Item1);
                }
                else
                {
                    using (var db = new LiteDatabase(GlobalSettings.Default.ConnectionString))
                    {
                        var movieCollection = db.GetCollection<Movie>();
                        var movie = movieCollection.FindOne(x => x.Id == item.MovieId);
                        identified.Add(movie);
                    }
                }

                OnProgressChanged(index, totalItems);
            }

            return identified;
        }

        private async Task<Tuple<Movie, float>> TryIdentify(AnalyzedFile item)
        {
            Log.DebugFormat("Querying remote: {0} ({1})", item.Title, item.Year);
            var matches = new List<TMDbLib.Objects.Movies.Movie>();
            List<SearchMovie> listContainer;

            if (!string.IsNullOrWhiteSpace(item.ImdbId))
            {
                // search by id
                var results = await _apiClient.FindAsync(FindExternalSource.Imdb, item.ImdbId);
                listContainer = results.MovieResults;
                Log.Debug($"Got {results.MovieResults.Count:N0} of {results.MovieResults.Count:N0} results");
            }
            else
            {
                // search by title
                var results = await _apiClient.SearchMovieAsync(item.Title);
                listContainer = results.Results;
                Log.Debug($"Got {results.Results.Count:N0} of {results.TotalResults:N0} results");
            }

            // fetch movie
            //foreach (var result in listContainer)
            //{
            //    Log.DebugFormat(" => {0}| {1} / {2} ({3})", result.Id, result.Title, result.OriginalTitle,
            //        result.ReleaseDate.GetValueOrDefault().Year);

            //    matches.Add(await _apiClient.GetMovieAsync(result.Id));
            //}
            
            //// sort matches
            //var resChart = matches
            //    .Select(x => new
            //    {
            //        Movie = EntityMapperHelpers.MapMovieToEntity(x),
            //        Match = GetMatch(x, item)
            //    })
            //    .OrderByDescending(z => z.Match).ToArray();

            // get most candidate
            var candidate = new
            {
                Movie = EntityMapperHelpers.MapMovieToEntity(await _apiClient.GetMovieAsync(listContainer.First().Id)),
                Match = 1
            };
            //if (candidate == null) return null;

            // find image URI
            _apiClient.GetConfig();
            candidate.Movie.ImageUri = _apiClient.GetImageUrl("w185", candidate.Movie.PosterPath).ToString();
            candidate.Movie.FilePath = item.FullPath;

            // save to db
            using (var db = new LiteDatabase(GlobalSettings.Default.ConnectionString))
            {
                var movieCollection = db.GetCollection<Movie>();
                movieCollection.Insert(candidate.Movie);
            }
            
            return new Tuple<Movie, float>(candidate.Movie, candidate.Match);
        }

        private void DoRename(string fullPath, Movie movie)
        {
            if (!GlobalSettings.Default.Reorganize) return;
            
            _fileOrganizer.Organize(fullPath, movie);
        }

        private float GetMatch(TMDbLib.Objects.Movies.Movie movie, AnalyzedFile item)
        {
            float res = 0;

            var dtt = DamerauLevenshtein.Calculate(item.Title.ToLower(), movie.Title.ToLower());
            var dto = DamerauLevenshtein.Calculate(item.Title.ToLower(), movie.OriginalTitle.ToLower());
            float score1 = Math.Max(0, 5 - Math.Min(dtt, dto));
            res += score1;
            
            //if (!string.IsNullOrEmpty(item.SubTitle))
            //{
            //    var dst = DamerauLevenshtein.Calculate(item.SubTitle.ToLower(), movie.Title.ToLower());
            //    var dso = DamerauLevenshtein.Calculate(item.SubTitle.ToLower(), movie.OriginalTitle.ToLower());
            //    float score2 = Math.Max(0, 3 - Math.Min(dst, dso));
            //    res += score2;
            //}

            if (LooksLike(item.Year, movie.ReleaseDate))
            {
                res += 3;
            }

            var duration = TimeSpan.FromMilliseconds(MediaHelper.GetMediaInfo(item.FullPath).General.Duration);
            if (duration > TimeSpan.Zero && movie.Runtime.HasValue)
            {
                var mtime = TimeSpan.FromMinutes(movie.Runtime.Value);
                var diff = Math.Abs(duration.Subtract(mtime).TotalMinutes);
                var score = (int)Math.Max(5 - diff, 0);
                {
                    res += score;
                }
            }
            else
            {
                res += 1;
            }

            return res / 16;
        }

        private bool LooksLike(int? itemYear, DateTime? matchReleaseDate)
        {
            if (!itemYear.HasValue || !matchReleaseDate.HasValue) return false;
            var yd = Math.Abs(itemYear.Value - matchReleaseDate.Value.Year);
            return yd <= 1;
        }

        private void OnMatchFound(MatchFoundEventArgs e)
        {
            MatchFound?.Invoke(this, e);
        }

        private void OnProgressChanged(int current, int total)
        {
            if (total <= 0) return;

            var p = 100 * current / total;
            var e = new ProgressChangedEventArgs(p, null);
            ProgressChanged?.Invoke(this, e);
        }
    }
}