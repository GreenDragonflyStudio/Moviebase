using System;
using System.Diagnostics;
using System.IO;
using log4net;
using LiteDB;
using Moviebase.Core.Utils;
using Moviebase.DAL;
using Moviebase.DAL.Entities;
using Ninject.Planning.Targets;

namespace Moviebase.Core.Components
{
    /// <inheritdoc />
    public class FileOrganizer : IFileOrganizer
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(FileOrganizer));
        private readonly IFolderCleaner _cleaner;
        private readonly IPathTokenizer _fileNameTokenizer;

        /// <summary>
        /// Initialize new instance of <see cref="FileOrganizer"/>.
        /// </summary>
        /// <param name="cleaner">Folder cleaner instance.</param>
        /// <param name="fileNameTokenizer">Path tokenizer instance.</param>
        public FileOrganizer(IFolderCleaner cleaner, IPathTokenizer fileNameTokenizer)
        {
            _cleaner = cleaner;
            _fileNameTokenizer = fileNameTokenizer;

            _fileNameTokenizer.TokenTemplate = GlobalSettings.Default.RenameTemplate;
            _fileNameTokenizer.TargetPath = GlobalSettings.Default.TargetPath;
        }

        /// <inheritdoc />
        public void Organize(MediaFile media)
        {
            // trasnform path
            var originalPath = media.FullPath;
            var targetPath = _fileNameTokenizer.GetTokenizedFilePath(originalPath, new PathToken(media, LookupMovie(media)));
            var targetDir = Path.GetDirectoryName(targetPath);
            Trace.Assert(targetDir != null);

            // move
            targetPath = IOExtension.EnsureNonDuplicateName(targetPath, out int duplicateCount);
            Log.InfoFormat("Target path sanitized. Possible {0} duplicates.", duplicateCount);

            Directory.CreateDirectory(targetDir);
            File.Move(originalPath, targetPath);

            // clean empty dir
            UpdateMediaFile(targetPath, media);
            _cleaner.Clean(Path.GetDirectoryName(originalPath));
            Log.InfoFormat("Media organized: {0} ==> {1}", originalPath, targetPath);
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

        // update media with new path
        private void UpdateMediaFile(string path, MediaFile media)
        {
            using (var db = new LiteDatabase(GlobalSettings.Default.ConnectionString))
            {
                var mediaCollection = db.GetCollection<MediaFile>();
                media.FullPath = path;
                media.LastSync = DateTime.Now;

                mediaCollection.Update(media);
                mediaCollection.EnsureIndex(x => x.Id);
            }
        }
    }
}