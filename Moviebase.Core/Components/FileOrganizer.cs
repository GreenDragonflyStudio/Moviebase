using System;
using System.IO;
using System.Linq;
using log4net;
using Moviebase.Core.Utils;
using Moviebase.DAL;
using Moviebase.DAL.Entities;

namespace Moviebase.Core.Components
{
    public class FileOrganizer : IFileOrganizer
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(FileOrganizer));
        private readonly IFolderCleaner _cleaner;
        private string _renameTemplate;

        public static readonly string DefaultTemplate = string.Format("{0}\\{1}\\({2}) {1}.{3}",
            FileNameTokens.Collection, FileNameTokens.Title, FileNameTokens.Year, FileNameTokens.Extension);

        public string DestinationFolder { get; }
        public string RenameTemplate
        {
            get => _renameTemplate;
            private set
            {
                if (!FileNameTokens.IsTemplateValid(value)) throw new ArgumentException("Rename Template is invalid");
                _renameTemplate = value;
            }
        }

        public FileOrganizer(IFolderCleaner cleaner)
        {
            DestinationFolder = GlobalSettings.Default.TargetPath;
            _renameTemplate = GlobalSettings.Default.RenameTemplate;
            _cleaner = cleaner;
        }
        
        #region Methods

        public string Organize(string filePath, Movie movie)
        {
            var fname = GetRenamedPath(filePath, movie);
            var target = Path.Combine(DestinationFolder, fname);
            var targetPath = SafeAddSuffix(target);

            IOExtension.SafeCreateDirectory(Path.GetDirectoryName(targetPath));
            IOExtension.Rename(filePath, targetPath);

            Log.InfoFormat("Match Saved: {0} ==> {1}", filePath, targetPath);
            _cleaner?.Clean(Path.GetDirectoryName(filePath));

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
                var tempFileName = $"{fileNameOnly}({count++})";
                newFullPath = Path.Combine(path, tempFileName + extension);
            }
            if (dupes)
            {
                Log.WarnFormat("Seems path {0} contains some duplicated movies", path);
            }
            return newFullPath;
        }

        private string GetRenamedPath(string filePath, Movie movie)
        {
            var fren = RenameTemplate;
            fren = fren.Replace(FileNameTokens.Title, movie.Title);
            fren = fren.Replace(FileNameTokens.Year, movie.Year?.ToString() ?? string.Empty);
            fren = fren.Replace(FileNameTokens.Collection, movie.Collection);
            fren = fren.Replace(FileNameTokens.Extension, Path.GetExtension(filePath));
            fren = fren.Replace(FileNameTokens.Genre, movie.Genres.FirstOrDefault()?.Name);
            fren = fren.Replace(FileNameTokens.AllGenres, string.Join(",", movie.Genres.Select(x => x.Name)));

            while (fren.Contains(@"\\"))
            {
                fren = fren.Replace(@"\\", @"\");
            }

            var frenamed = CleanFileName(fren);
            return frenamed;
        }

        #endregion Methods
    }
}