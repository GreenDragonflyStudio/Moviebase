using Moviebase.Core.Components;
using Movie = Moviebase.Core.Models.Movie;

namespace Moviebase.Core.Interfaces
{
    public interface IMovieDb
    {
        #region Methods

        string GetHashFor(FileRef item);

        Movie GetMovie(int movieId);

        int? GetMovieIdFor(AnalyzedItem item);

        bool HasHash(FileRef item);

        bool HasMatch(AnalyzedItem item);

        void Push(FileRef fref, string hash);

        void Push(Movie movieTaskResult);

        void Push(string hash, int movieId);

        #endregion Methods
    }
}