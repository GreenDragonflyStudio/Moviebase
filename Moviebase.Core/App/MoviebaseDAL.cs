using System;
using System.Linq;
using LiteDB;
using Moviebase.Core.Utils;
using Moviebase.DAL;
using Moviebase.DAL.Entities;

// ReSharper disable once InconsistentNaming

namespace Moviebase.Core.App
{
    /// <inheritdoc />
    public class MoviebaseDAL : IMoviebaseDAL
    {
        /// <inheritdoc />
        public Movie MapMovieToEntity(TMDbLib.Objects.Movies.Movie match, string imageUri)
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
                Genres = match.Genres.Select(g => new Genre { Id = g.Id, Name = g.Name }).ToList(),

                ImageUri = imageUri,
                OriginalLanguage = match.OriginalLanguage,
                OriginalTitle = match.OriginalTitle,
                Popularity = match.Popularity,
                VoteAverage = match.VoteAverage,
                VoteCount = match.VoteCount,
            };
        }

        /// <inheritdoc />
        public Movie GetMovieByImdbId(string imdbId)
        {
            using (var db = new LiteDatabase(GlobalSettings.Default.ConnectionString))
            {
                var movieCollection = db.GetCollection<Movie>();
                return movieCollection.FindOne(x => x.ImdbId == imdbId);
            }
        }

        /// <inheritdoc />
        public void RecordScanFile(AnalyzedFile file, Movie movie)
        {
            using (var db = new LiteDatabase(GlobalSettings.Default.ConnectionString))
            {
                // update movie data
                var movieCollection = db.GetCollection<Movie>();
                movieCollection.Upsert(movie);

                // update data hash
                var hashCollection = db.GetCollection<MediaFileHash>();
                var hashEntity = hashCollection.FindOne(x => x.Hash == file.Hash) ?? new MediaFileHash();
                hashEntity.Hash = file.Hash;
                hashEntity.ImdbId = movie.ImdbId;
                hashEntity.Title = movie.Title;
                hashEntity.Year = movie.Year;

                hashCollection.Upsert(hashEntity);

                // update media file
                var mediaCollection = db.GetCollection<MediaFile>();
                var mediaEntity = mediaCollection.FindOne(x => x.Hash == file.Hash) ?? new MediaFile();
                mediaEntity.TmdbId = movie.TmdbId;
                mediaEntity.Hash = file.Hash;
                mediaEntity.FullPath = file.FullPath;
                mediaEntity.LastSync = DateTime.Now;

                mediaCollection.Upsert(mediaEntity);
            }
        }
    }
}
