using log4net;
using Moviebase.Core.Models;
using Moviebase.Core.Utils;
using System;
using System.IO;
using System.Linq;

namespace Moviebase.Core.Components.MediaOrganizer
{
    public class FileOrganizer : IFileOrganizer
    {
        #region Fields

        private static readonly ILog Log = LogManager.GetLogger(typeof(FileOrganizer));
        private readonly IFolderCleaner _cleaner;
        private string _renameTemplate;

        #endregion Fields

        #region Constructors

        internal FileOrganizer(string destinationFolder, string renameTemplate, IFolderCleaner cleaner)
        {
            DestinationFolder = destinationFolder;
            _renameTemplate = renameTemplate;
            _cleaner = cleaner;
        }

        #endregion Constructors

        #region Properties

        public static string DefaultTemplate => string.Format("{0}\\{1}\\({2}) {1}.{3}",
            Tokens.Collection, Tokens.Title, Tokens.Year, Tokens.Extension);

        public string DestinationFolder { get; }

        public string RenameTemplate
        {
            get
            {
                return _renameTemplate;
            }
            private set
            {
                if (!IsTemplateValid(value))
                    throw new ArgumentException("Rename Template is invalid");
                _renameTemplate = value;
            }
        }

        #endregion Properties

        #region Methods

        public string Organize(FileInfo itemPath, Movie movie)
        {
            var fname = GetRenamedPath(itemPath, movie);
            var target = Path.Combine(DestinationFolder, fname);
            var targetPath = SafeAddSuffix(target);

            FilesystemTools.SafeCreateDirectory(Path.GetDirectoryName(targetPath));

            File.Move(itemPath.FullName, targetPath);
            Log.InfoFormat("Match Saved: {0} ==> {1}", itemPath.Name, targetPath);

            if (_cleaner != null)
            {
                _cleaner.Clean(itemPath.Directory);
            }

            return targetPath;
        }

        private static string CleanFileName(string fileName)
        {
            var pathTokens = fileName.Split(new[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < pathTokens.Length; i++)
            {
                var token = pathTokens[i];
                pathTokens[i] = Path.GetInvalidFileNameChars()
                .Aggregate(token, (current, c) => current.Replace(c.ToString(), string.Empty));
            }
            return string.Join("\\", pathTokens);
        }

        private static bool IsTemplateValid(string value)
        {
            var tokens = new[] { Tokens.Title, Tokens.Extension, Tokens.Year, Tokens.Collection, Tokens.Genre };
            var val = value;

            foreach (var token in tokens)
                val = val.Replace(token, string.Empty);

            return !val.Contains("%");
        }

        private static string SafeAddSuffix(string fullPath)
        {
            var count = 1;

            var fileNameOnly = Path.GetFileNameWithoutExtension(fullPath);
            var extension = Path.GetExtension(fullPath);
            var path = Path.GetDirectoryName(fullPath) ?? string.Empty;
            var newFullPath = fullPath;

            var dupes = false;
            while (File.Exists(newFullPath))
            {
                dupes = true;
                var tempFileName = string.Format("{0}({1})", fileNameOnly, count++);
                newFullPath = Path.Combine(path, tempFileName + extension);
            }
            if (dupes)
            {
                Log.WarnFormat("Seems path {0} contains some duplicated movies", path);
            }
            return newFullPath;
        }

        private string GetRenamedPath(FileInfo item, Movie movie)
        {
            var fren = RenameTemplate;
            fren = fren.Replace(Tokens.Title, movie.Title);
            fren = fren.Replace(Tokens.Year, movie.Year?.ToString() ?? string.Empty);
            fren = fren.Replace(Tokens.Collection, movie.Collection);
            fren = fren.Replace(Tokens.Extension, item.Extension);
            fren = fren.Replace(Tokens.Genre, movie.Genres.FirstOrDefault()?.Name);
            fren = fren.Replace(Tokens.AllGenres, string.Join(",", movie.Genres.Select(x => x.Name)));

            while (fren.Contains(@"\\"))
            {
                fren = fren.Replace(@"\\", @"\");
            }

            var frenamed = CleanFileName(fren);

            return frenamed;
        }

        #endregion Methods

        #region Classes

        public static class Tokens
        {
            #region Fields

            public const string AllGenres = "%allgenres%";
            public const string Collection = "%collection%";
            public const string Extension = "%ext%";
            public const string Genre = "%firstgenre%";
            public const string Title = "%title%";
            public const string Year = "%year%";

            #endregion Fields
        }

        #endregion Classes
    }
}