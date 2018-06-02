using System.Collections.Generic;
using System.IO;
using Moviebase.DAL;

namespace Moviebase.Core.Components
{
    /// <inheritdoc />
    public class DirectoryScanner : IDirectoryScanner
    {
        /// <inheritdoc />
        public IEnumerable<string> EnumerateDirectories()
        {
            return Directory.EnumerateDirectories(GlobalSettings.Default.TargetPath);
        }
    }
}
