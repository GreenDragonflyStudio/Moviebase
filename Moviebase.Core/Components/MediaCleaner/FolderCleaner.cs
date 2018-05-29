using log4net;
using Moviebase.Core.Utils;
using System.IO;

namespace Moviebase.Core.Components
{
    public class FolderCleaner : IFolderCleaner
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(FolderCleaner));

        public void Clean(DirectoryInfo directory)
        {
            var deletes = CleanDirectory(directory);
            Log.DebugFormat("Directory Cleaned: {0} ({1} parents deleted)", directory, deletes - 1);
        }

        public static int CleanDirectory(DirectoryInfo directory)
        {
            var deleted = 0;
            if (directory.Exists)
            {
                var toDelete = directory;
                var parent = directory.Parent;
                while (toDelete.IsEmpty())
                {
                    toDelete.Delete();
                    deleted++;
                    toDelete = parent;
                    if (parent == null) break;
                    parent = toDelete.Parent;
                }
            }

            return deleted;
        }

        public static int CleanDirectory(string directory)
        {
            return CleanDirectory(new DirectoryInfo(directory));
        }
    }
}