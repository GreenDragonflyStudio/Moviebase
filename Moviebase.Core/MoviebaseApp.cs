using log4net;
using Moviebase.Core.Components.MediaAnalyzer;
using Moviebase.Core.Components.MediaOrganizer;
using Moviebase.Core.Components.MediaScanner;
using Moviebase.Core.Interfaces;
using Moviebase.Core.Models;
using Moviebase.Core.Utils.Algorithms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using TMDbLib.Client;
using Movie = TMDbLib.Objects.Movies.Movie;

namespace Moviebase.Core
{
    public sealed class MoviebaseApp
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(MoviebaseApp));

        private readonly IFileAnalyzer _analyzer;
        private readonly TMDbClient _apiClient;
        private readonly IMovieDb _db;
        private readonly IFileScanner _fileScanner;

        public MoviebaseApp(IConfigReader configReader, IFileScanner fileScanner, IFileAnalyzer analyzer, IMovieDb db)
        {
            _fileScanner = fileScanner;
            _analyzer = analyzer;
            _db = db;

            ApiSettings apiSettings = configReader.GetApiSettings();
            if (apiSettings == null)
            {
                throw new ArgumentNullException("configReader", "Argument not valid, Please provide ApiSettings");
            }
            _apiClient = new TMDbClient(apiSettings.ApiKey)
            {
                DefaultCountry = "ID",
                DefaultLanguage = "Id"
            };
        }

        public event EventHandler<MatchFoundEventArgs> MatchFound;

        public event ProgressChangedEventHandler ProgressChanged;

        public Task<int> ScanAsync(MoviebaseSettings settings)
        {
            var tf = new TaskFactory<int>();
            return tf.StartNew(() => Scan(settings));
        }

        public int Scan(MoviebaseSettings settings)
        {
            var count = 0;
            var index = 0;

            var files = _fileScanner.Scan().ToArray();
            var totalItems = files.Length;

            foreach (var file in files)
            {
                index++;
                OnProgressChanged(index, totalItems);

                var item = _analyzer.Analyze(file);
                if (!item.IsKnown)
                {
                    var res = TryIdentify(item, out float accuracy);
                    if (res != null)
                    {
                        var args = new MatchFoundEventArgs(item, res, accuracy);
                        OnMatchFound(args);

                        if (args.Cancel)
                        {
                            break;
                        }

                        if (args.IsMatch == true)
                        {
                            DoRename(args, settings);

                            UpdateItem(item, res);
                            count++;
                        }
                    }
                }
                else
                {
                    var movieId = _db.GetMovieIdFor(item);
                    var movie = _db.GetMovie(movieId.Value);
                    item.Title = movie.Title;
                    item.Year = movie.Year;

                    var res = TryIdentify(item, out float accuracy);
                    var args = new MatchFoundEventArgs(item, res, 1);
                    DoRename(args, settings);
                }
            }

            return count;
        }

        private static void DoRename(MatchFoundEventArgs args, MoviebaseSettings settings)
        {
            if (settings.Reorganize)
            {
                var sourcePath = args.LocalFile.Path;
                IFolderCleaner cleaner = settings.DeleteEmptyFolders ? new FolderCleaner() : null;
                var organizer = new FileOrganizer(settings.TargetPath, settings.RenameTemplate, cleaner);
                organizer.Organize(sourcePath, args.Movie);
            }
        }

        private Models.Movie TryIdentify(AnalyzedItem item, out float matchAccuracy)
        {
            Log.DebugFormat("Querying remote: {0} ({1})", item.Title, item.Year);
            IEnumerable<string> tokens = new[] { item.Title };
            var matches = new List<Movie>();
            foreach (var token in tokens)
            {
                var results = _apiClient.SearchMovieAsync(token).Result;

                Log.Debug($"Got {results.Results.Count:N0} of {results.TotalResults:N0} results");
                foreach (var result in results.Results)
                {
                    Log.DebugFormat(" => {0}| {1} / {2} ({3})", result.Id, result.Title, result.OriginalTitle,
                        result.ReleaseDate.GetValueOrDefault().Year);

                    var movieTask = _apiClient.GetMovieAsync(result.Id);
                    movieTask.Wait();
                    var movie = movieTask.Result;

                    matches.Add(movie);
                }
            }

            var resChart = matches
                .Select(x => new { Movie = MapDbItem(x), Match = GetMatch(x, item) })
                .OrderByDescending(z => z.Match).ToArray();

            var candidate = resChart.FirstOrDefault(x => x.Match >= 0.2f);
            if (candidate != null)
            {
                matchAccuracy = candidate.Match;

                _apiClient.GetConfig();
                candidate.Movie.ImageUri = _apiClient.GetImageUrl("w185", candidate.Movie.PosterPath).ToString();

                var movie = candidate.Movie;
                _db.Push(movie);

                _db.Push(item.Hash, movie.Id);

                return movie;
            }
            else
            {
                matchAccuracy = 0f;
            }

            return null;
        }

        private static float GetMatch(Movie movie, AnalyzedItem item)
        {
            float res = 0;

            var dtt = Distance.LevenshteinDistance(item.Title.ToLower(), movie.Title.ToLower());
            var dto = Distance.LevenshteinDistance(item.Title.ToLower(), movie.OriginalTitle.ToLower());
            float score1 = Math.Max(0, 5 - Math.Min(dtt, dto));
            res += score1;

            if (!string.IsNullOrEmpty(item.SubTitle))
            {
                var dst = Distance.LevenshteinDistance(item.SubTitle.ToLower(), movie.Title.ToLower());
                var dso = Distance.LevenshteinDistance(item.SubTitle.ToLower(), movie.OriginalTitle.ToLower());
                float score2 = Math.Max(0, 3 - Math.Min(dst, dso));
                res += score2;
            }

            if (LooksLike(item.Year, movie.ReleaseDate))
            {
                res += 3;
            }

            if (item.Duration > TimeSpan.Zero && movie.Runtime.HasValue)
            {
                var mtime = TimeSpan.FromMinutes(movie.Runtime.Value);
                var diff = Math.Abs(item.Duration.Subtract(mtime).TotalMinutes);
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

        private static Models.Movie MapDbItem(Movie match)
        {
            var res = new Models.Movie
            {
                Id = match.Id,
                Title = match.Title,
                OriginalTitle = match.OriginalTitle,
                ReleaseDate = match.ReleaseDate,
                Overview = match.Overview,
                Duration = TimeSpan.FromMinutes(match.Runtime.GetValueOrDefault()),
                Adult = match.Adult,
                Genres = match.Genres.Select(g => new MovieGenre { Id = g.Id, Name = g.Name }).ToArray(),
                ImdbId = match.ImdbId,
                OriginalLanguage = match.OriginalLanguage,
                Popularity = match.Popularity,
                PosterPath = match.PosterPath,
                VoteAverage = match.VoteAverage,
                VoteCount = match.VoteCount,
                Collection = match.BelongsToCollection?.Name.Replace(" - Collezione", string.Empty)
            };

            ////   match.Similar
            ////    match.Videos
            return res;
        }

        private static bool LooksLike(int? itemYear, DateTime? matchReleaseDate)
        {
            if (itemYear.HasValue && matchReleaseDate.HasValue)
            {
                var yd = Math.Abs(itemYear.Value - matchReleaseDate.Value.Year);
                return yd <= 1;
            }
            return false;
        }

        public void UpdateItem(AnalyzedItem item, Models.Movie res)
        {
            // Method intentionally left empty.
        }

        private void OnMatchFound(MatchFoundEventArgs e)
        {
            MatchFound?.Invoke(this, e);
        }

        private void OnProgressChanged(int current, int total)
        {
            if (total > 0)
            {
                var p = 100 * current / total;
                ProgressChangedEventArgs e = new ProgressChangedEventArgs(p, null);
                ProgressChanged?.Invoke(this, e);
            }
        }
    }
}