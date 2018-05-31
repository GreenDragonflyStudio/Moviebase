using System.Collections.Generic;

namespace Moviebase.Core.Components
{
    /// <summary>
    /// Provides file system search support for directories in target path.
    /// </summary>
    public interface IDirectoryScanner
    {
        /// <summary>
        /// Enumerate directories on target path.
        /// </summary>
        /// <returns><see cref="IEnumerable{T}"/> object to enumerate target path directories.</returns>
        IEnumerable<string> EnumerateDirectories();
    }
}
