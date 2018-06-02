using System;
using System.Diagnostics;
using System.IO;
using log4net;
using LiteDB;
using Moviebase.Core.Utils;
using Moviebase.DAL;
using Moviebase.DAL.Entities;

namespace Moviebase.Core.Components
{
    /// <inheritdoc />
    public class FileOrganizer : IFileOrganizer
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(FileOrganizer));
        private readonly IFolderCleaner _cleaner;
        private readonly IPathTransformer _fileNameTransformer;

        /// <inheritdoc />
        public bool DeleteEmptyDirectories { get; set; }

        /// <summary>
        /// Initialize new instance of <see cref="FileOrganizer"/>.
        /// </summary>
        /// <param name="cleaner">Folder cleaner instance.</param>
        /// <param name="fileNameTransformer">Path tokenizer instance.</param>
        public FileOrganizer(IFolderCleaner cleaner, IPathTransformer fileNameTransformer)
        {
            _cleaner = cleaner;
            _fileNameTransformer = fileNameTransformer;
        }

        /// <inheritdoc />
        public void Organize(MediaFile media)
        {
            // trasnform moviePath
            var originalPath = media.FullPath;
            var moviePath = TransformPath(media);
            
            // target
            var targetDir = Path.GetDirectoryName(moviePath);
            Trace.Assert(targetDir != null);
            Directory.CreateDirectory(targetDir);

            // apply moviePath transformation
            var subtitlePath = Path.Combine(targetDir, Path.GetFileName(media.SubtitlePath));
            var posterPath = Path.Combine(targetDir, Path.GetFileName(media.PosterPath));
            
            // move movie, sub, poster
            File.Move(media.FullPath, moviePath);
            File.Move(media.SubtitlePath, subtitlePath);
            File.Move(media.PosterPath, posterPath);

            // update db
            UpdateMediaFile(moviePath, subtitlePath, posterPath, media);

            // clean empty dir
            if (DeleteEmptyDirectories) _cleaner.Clean(Path.GetDirectoryName(originalPath));
            Log.InfoFormat("Media organized: {0} ==> {1}", originalPath, moviePath);
        }

        private string TransformPath(MediaFile media)
        {
            string moviePath;
            moviePath = _fileNameTransformer.GetTokenizedFilePath(media.FullPath,
                new PathToken(media, LookupMovie(media)));
            moviePath = IOExtension.CleanFilePath(moviePath);
            moviePath = IOExtension.EnsureNonDuplicateName(moviePath, out int duplicateCount);
            Log.InfoFormat("Target moviePath sanitized. Possible {0} duplicates.", duplicateCount);

            return moviePath;
        }

        // lookup associated movie from media in database
        private Movie LookupMovie(MediaFile media)
        {
            using (var db = new LiteDatabase(GlobalSettings.Default.ConnectionString))
            {
                var movieCollection = db.GetCollection<Movie>();
                return movieCollection.FindOne(x => x.TmdbId == media.TmdbId);
            }
        }

        // update media with new moviePath
        private void UpdateMediaFile(string moviePath, string subPath, string posterPath, MediaFile media)
        {
            using (var db = new LiteDatabase(GlobalSettings.Default.ConnectionString))
            {
                var mediaCollection = db.GetCollection<MediaFile>();
                media.LastSync = DateTime.Now;
                media.FullPath = moviePath;
                media.SubtitlePath = subPath;
                media.PosterPath = posterPath;

                mediaCollection.Update(media);
                mediaCollection.EnsureIndex(x => x.Id);
            }
        }
    }
}