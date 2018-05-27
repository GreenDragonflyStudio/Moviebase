using System;
using System.Linq;
using Moviebase.DAL.Entities;

namespace Moviebase.Core.Utils
{
    public static class EntityMapperHelpers
    {
        public static Movie MapMovieToEntity(TMDbLib.Objects.Movies.Movie match)
        {
            var res = new Movie
            {
                TmdbId = match.Id,
                ImdbId = match.ImdbId,

                Title = match.Title,
                ReleaseDate = match.ReleaseDate,
                Overview = match.Overview,

                Adult = match.Adult,
                Collection = match.BelongsToCollection?.Name.Replace(" - Collezione", string.Empty),
                Duration = TimeSpan.FromMinutes(match.Runtime.GetValueOrDefault()),
                Genres = match.Genres.Select(g => new Genre { Id = g.Id, Name = g.Name }).ToList(),

                OriginalLanguage = match.OriginalLanguage,
                OriginalTitle = match.OriginalTitle,
                Popularity = match.Popularity,
                VoteAverage = match.VoteAverage,
                VoteCount = match.VoteCount,
            };

            ////   match.Similar
            ////    match.Videos
            return res;
        }


    }
}
