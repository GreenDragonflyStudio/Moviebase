using Moviebase.Core.Utils;
using Moviebase.DAL.Entities;

// ReSharper disable once InconsistentNaming

namespace Moviebase.Core.App
{
    /// <summary>
    /// Provides simplified database access to Moviebase Database.
    /// </summary>
    public interface IMoviebaseDAL
    {
        /// <summary>
        /// Maps TMDBLib <see cref="TMDbLib.Objects.Movies.Movie"/> object to Moviebase's <see cref="Movie"/> object.
        /// </summary>
        /// <param name="match"><see cref="TMDbLib.Objects.Movies.Movie"/> object to map.</param>
        /// <param name="imageUri">Extra URI to <paramref name="match"/> associated poster.</param>
        /// <returns><see cref="Movie"/> object containing movie information.</returns>
        Movie MapMovieToEntity(TMDbLib.Objects.Movies.Movie match, string imageUri);

        /// <summary>
        /// Get a <see cref="Movie"/> object by specified <paramref name="imdbId"/> value.
        /// </summary>
        /// <param name="imdbId">IMDB ID to lookup.</param>
        /// <returns><see cref="Movie"/> object containing information from specified <paramref name="imdbId"/>.</returns>
        Movie GetMovieByImdbId(string imdbId);

        /// <summary>
        /// Record specified scanned file to <c>MediaFileHash</c> and <c>MediaFile</c> table respectively.
        /// </summary>
        /// <param name="file"><see cref="AnalyzedFile"/> object representing current file.</param>
        /// <param name="movie"><see cref="Movie"/> object associated with specified <see cref="AnalyzedFile"/> object.</param>
        void RecordScanFile(AnalyzedFile file, Movie movie);

    }
}