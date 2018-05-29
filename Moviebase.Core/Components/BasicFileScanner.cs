using System.Collections.Generic;
using System.IO;
using System.Linq;
using log4net;

namespace Moviebase.Core.Components
{
    /// <inheritdoc />
    public class BasicFileScanner : IFileScanner
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(FileAnalyzer));

        /// <inheritdoc />
        public IEnumerable<string> Scan(string path)
        {
            Log.DebugFormat("Scanning: {0}", path);

            var extensions = new List<string> { "mkv", "mp4", "avi" };
            return Directory.Exists(path) 
                ? Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories).Where(x => extensions.Any(x.EndsWith)) 
                : new[] {""};
        }
    }
}