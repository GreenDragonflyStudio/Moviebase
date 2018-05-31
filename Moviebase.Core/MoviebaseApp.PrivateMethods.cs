using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Moviebase.DAL.Entities;
using Moviebase.Services.Metadata;
using TMDbLib.Objects.Find;
using TMDbLib.Objects.Search;

namespace Moviebase.Core
{
    public sealed partial class MoviebaseApp
    {
        // get a value how it's likely the candidate is the guessed movie
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

        // identify a file
        private async Task<Movie> IdentifyFile(AnalyzedFile item)
        {
            Log.DebugFormat("Querying remote: {0} ({1})", item.Title, item.Year);
            Movie movie = null;

            try
            {
                List<SearchMovie> listContainer;

                // remote find
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
                var candidateMovieRaw = listContainer.OrderByDescending(x => GetMatch(x, item)).FirstOrDefault();
                if (candidateMovieRaw == null) return null;

                movie = MapMovieToEntity(await _apiClient.GetMovieAsync(candidateMovieRaw.Id));
            }
            catch (Exception e)
            {
                Log.Error("Unable to process request", e);
            }

            // return fetched or fallback
            return movie ?? GetByFilename();
            Movie GetByFilename()
            {
                // the file is not identified by guessing algorithm
                Log.Debug("Unable to guess title, fallback to filename");
                return new Movie
                {
                    Title = Path.GetFileName(item.FullPath),
                };
            }
        }

        #region Event Invocator

        private void OnProgressChanged(int current, int total)
        {
            if (total <= 0) return;

            var p = 100 * current / total;
            var e = new ProgressChangedEventArgs(p, null);
            ProgressChanged?.Invoke(this, e);
        }

        #endregion
    }
}
