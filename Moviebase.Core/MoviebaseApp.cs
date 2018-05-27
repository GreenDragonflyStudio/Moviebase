using log4net;
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
                        var movie = movieCollection.FindOne(x => x.ImdbId == item.ImdbId);
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

            // get most candidate
            var candidateMovieRaw = listContainer.Select(x => new {Raw = x, ApproxMatch = GetMatch(x, item)})
                .OrderByDescending(x => x.ApproxMatch).FirstOrDefault();
            if (candidateMovieRaw == null) return null;
            var candidate = new
            {
                Movie = MapMovieToEntity(await _apiClient.GetMovieAsync(candidateMovieRaw.Raw.Id)),
                ApproximateMatch = candidateMovieRaw.ApproxMatch
            };
            
            // save to db
            using (var db = new LiteDatabase(GlobalSettings.Default.ConnectionString))
            {
                // update movie data
                var movieCollection = db.GetCollection<Movie>();
                movieCollection.Upsert(candidate.Movie);

                // update data hash
                var hashCollection = db.GetCollection<MediaFileHash>();
                var hashEntity = hashCollection.FindOne(x => x.Hash == item.Hash) ?? new MediaFileHash();
                hashEntity.Hash = item.Hash;
                hashEntity.ImdbId = candidate.Movie.ImdbId;
                hashEntity.Title = candidate.Movie.Title;
                hashEntity.Year = candidate.Movie.Year;

                hashCollection.Upsert(hashEntity);

                // update media file
                var mediaCollection = db.GetCollection<MediaFile>();
                var mediaEntity = mediaCollection.FindOne(x => x.Hash == item.Hash) ?? new MediaFile();
                mediaEntity.Hash = item.Hash;
                mediaEntity.FullPath = item.FullPath;
                mediaEntity.Synced = true;
                mediaEntity.LastSync = DateTime.Now;

                mediaCollection.Upsert(mediaEntity);
            }
            
            return new Tuple<Movie, float>(candidate.Movie, candidate.ApproximateMatch);
        }

        private void DoRename(string fullPath, Movie movie)
        {
            if (!GlobalSettings.Default.Reorganize) return;
            
            _fileOrganizer.Organize(fullPath, movie);
        }

        private float GetMatch(SearchMovie movie, AnalyzedFile candidate)
        {
            float res = 0;

            var dtt = DamerauLevenshtein.Calculate(candidate.Title.ToUpper(), movie.Title.ToUpper());
            var dto = DamerauLevenshtein.Calculate(candidate.Title.ToUpper(), movie.OriginalTitle.ToUpper());
            res += Math.Max(0, 5 - Math.Min(dtt, dto));

            if (Math.Abs(candidate.Year - (movie.ReleaseDate?.Year ?? 0)) <= 1)
            {
                res += 3;
            }
            return res; // TODO: return value unchecked for 0<x<1 interval
        }

        private Movie MapMovieToEntity(TMDbLib.Objects.Movies.Movie match)
        {
            return new Movie
            {
                TmdbId = match.Id,
                ImdbId = match.ImdbId,

                Title = match.Title,
                ReleaseDate = match.ReleaseDate,
                Overview = match.Overview,

                Adult = match.Adult,
                Collection = match.BelongsToCollection?.Name.Replace(" - Collezione", string.Empty),
                Duration = TimeSpan.FromMinutes(match.Runtime.GetValueOrDefault()),
                Genres = match.Genres.Select(g => new Genre {Id = g.Id, Name = g.Name}).ToList(),

                ImageUri = _apiClient.GetImageUrl("w185", match.PosterPath).ToString(),
                PosterPath = match.PosterPath,
                OriginalLanguage = match.OriginalLanguage,
                OriginalTitle = match.OriginalTitle,
                Popularity = match.Popularity,
                VoteAverage = match.VoteAverage,
                VoteCount = match.VoteCount,
            };
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