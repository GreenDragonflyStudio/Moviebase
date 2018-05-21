using System.Collections.Generic;
using System.Threading.Tasks;
using Moviebase.Services.Entities;
using Moviebase.Services.Entities.Internals;

namespace Moviebase.Services.Metadata
{
    public interface ITmdb
    {
        Task<List<string>> SearchMoviesAsync(string query, int year);

        Task<TmdbEntity> GetByImdbIdAsync(string imdbId);
        Task<TmdbEntity> GetByTmdbIdAsync(string tmdbId);
        
        string GetPosterUri(string path, PosterSize size);
    }
}
