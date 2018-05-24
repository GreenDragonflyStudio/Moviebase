using Moviebase.Services.Entities;
using Moviebase.Services.Entities.Internals;
using Moviebase.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Moviebase.Services.Metadata
{
    public class Tmdb : ITmdb
    {
        private const string ApiEndpoint = "api.themoviedb.org";
        private const string PosterEndpoint = "image.tmdb.org";

        private static readonly HttpClient HttpClientInstance = new HttpClient();
        private readonly string _apiKey;

        public Tmdb(string apiKey)
        {
            _apiKey = apiKey;
        }

        public string GetPosterUri(string path, PosterSize size)
        {
            const string posterPath = "t/p";
            return HttpClientHelpers.BuildFullUri(PosterEndpoint, posterPath + size + path, null);
        }

        public async Task<List<string>> SearchMoviesAsync(string query, int year)
        {
            const string searchMoviePath = "3/search/movie";
            var param = new NameValueCollection
            {
                {"api_key", _apiKey},
                {"query", Uri.EscapeDataString(query)},
                {"include_adult", "0"}
            };
            if (year > 0) param.Add("year", year.ToString());

            // get
            var uri = HttpClientHelpers.BuildFullUri(ApiEndpoint, searchMoviePath, param);
            var response = await HttpClientInstance.GetRequestBody<MovieSearchRoot>(uri);
            return response.total_results <= 0 ? null : response.results.Select(x => x.id.ToString()).ToList();
        }

        public async Task<TmdbEntity> GetByImdbIdAsync(string imdbId)
        {
            if (imdbId == "-1") return null;

            const string findPath = "3/find/{0}";
            var col = new NameValueCollection
            {
                {"api_key", _apiKey},
                {"external_source", "imdb_id"}
            };

            // get
            var uri = HttpClientHelpers.BuildFullUri(ApiEndpoint, string.Format(findPath, imdbId), col);
            var response = await HttpClientInstance.GetRequestBody<FindRoot>(uri);
            if (response.movie_results != null && response.movie_results.Count > 0)
            {
                return await GetByTmdbIdAsync(response.movie_results.First().id.ToString());
            }
            return null;
        }

        public async Task<TmdbEntity> GetByTmdbIdAsync(string tmdbId)
        {
            if (tmdbId == "-1") return null;

            const string moviePath = "3/movie/{0}";
            var col = new NameValueCollection
            {
                {"api_key", _apiKey},
                {"append_to_response", "alternative_titles"}
            };

            // get
            var uri = HttpClientHelpers.BuildFullUri(ApiEndpoint, string.Format(moviePath, tmdbId), col);
            var response = await HttpClientInstance.GetRequestBody<MovieDetailsRoot>(uri);
            var data = new TmdbEntity
            {
                TmdbId = response.id,
                ImdbId = response.imdb_id,

                Genre = string.Join(", ", response.genres.Select(x => x.name)),
                Plot = response.overview,
                Title = response.title,
                ReleaseDate = DateTime.Parse(response.release_date),

                AlternativeNames = ParseAlternatives(response),
                PosterPath = await GetPosterUris(response.id.ToString())
            };
            return data;
        }

        private async Task<List<string>> GetPosterUris(string tmdbId)
        {
            const string postersPath = "3/movie/{0}/images";
            var col = new NameValueCollection
            {
                {"api_key", _apiKey},
                {"include_image_language", "en,null"}
            };

            // get
            var uri = HttpClientHelpers.BuildFullUri(ApiEndpoint, string.Format(postersPath, tmdbId), col);
            var response = await HttpClientInstance.GetRequestBody<PosterFindRoot>(uri);
            return response.posters.Select(x => x.file_path).ToList();
        }

        private List<string> ParseAlternatives(MovieDetailsRoot root)
        {
            var combined = new List<string> { root.title };
            combined.AddRange(root.alternative_titles.titles.Select(x => x.title));
            return combined;
        }
    }
}