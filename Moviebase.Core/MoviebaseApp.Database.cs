using System;
using System.Linq;
using LiteDB;
using Moviebase.DAL;
using Moviebase.DAL.Entities;

namespace Moviebase.Core
{
    public sealed partial class MoviebaseApp
    {
        // map specified TMDB movie object to Moviebase's DAL Movie object
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
                Genres = match.Genres.Select(g => new Genre { Id = g.Id, Name = g.Name }).ToList(),

                ImageUri = _apiClient.GetImageUrl("w185", match.PosterPath).ToString(),
                OriginalLanguage = match.OriginalLanguage,
                OriginalTitle = match.OriginalTitle,
                Popularity = match.Popularity,
                VoteAverage = match.VoteAverage,
                VoteCount = match.VoteCount,
            };
        }

        // lookup Movie info from IMDB ID
        private Movie GetMovieByImdbId(string imdbId)
        {
            using (var db = new LiteDatabase(GlobalSettings.Default.ConnectionString))
            {
                var movieCollection = db.GetCollection<Movie>();
                return movieCollection.FindOne(x => x.ImdbId == imdbId);
            }
        }

        // record specified folder to folder table
        private void RecordScanFolder(string path)
        {
            using (var db = new LiteDatabase(GlobalSettings.Default.ConnectionString))
            {
                var collection = db.GetCollection<Folder>();
                var entity = collection.FindOne(x => x.Path == path) ?? new Folder();
                entity.Path = path;
                entity.LastSync = DateTime.Now;
                entity.Synced = true;

                collection.Upsert(entity);
            }
        }

        // record specified file to media file table and hash table
        private void RecordScanFile(Movie movie, AnalyzedFile file)
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
