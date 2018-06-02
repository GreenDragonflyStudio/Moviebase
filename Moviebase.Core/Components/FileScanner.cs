using System.Collections.Generic;
using System.IO;
using System.Linq;
using log4net;

namespace Moviebase.Core.Components
{
    /// <inheritdoc />
    public class FileScanner : IFileScanner
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(FileAnalyzer));

        /// <inheritdoc />
        public List<string> MovieExtensions { get; set; }

        /// <inheritdoc />
        public IEnumerable<string> ScanMovie(string path)
        {
            Log.DebugFormat("Scanning: {0}", path);

            return Directory.Exists(path)
                ? Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories)
                    .Where(x => MovieExtensions.Any(x.EndsWith))
                : new[] {""};
        }
    }
}