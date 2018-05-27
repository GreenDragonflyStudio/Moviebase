using System.IO;
using log4net;
using Moviebase.DAL;

namespace Moviebase.Core.Components
{
    public class FolderCleaner : IFolderCleaner
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(FolderCleaner));

        public void Clean(string path)
        {
            if (!GlobalSettings.Default.DeleteEmptyFolders) return;

            if (!Directory.Exists(path)) return;
            var deleted = 0;

            // find child
            foreach (var dir in Directory.GetDirectories(path, "*", SearchOption.AllDirectories))
            {
                Directory.Delete(dir);
                deleted++;
            }

            // delete parent
            Directory.Delete(path);
            Log.DebugFormat("Directory Cleaned: {0} ({1} parents deleted)", path, deleted);
        }
    }
}