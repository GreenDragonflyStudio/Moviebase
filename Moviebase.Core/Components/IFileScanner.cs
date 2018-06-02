using System.Collections.Generic;

namespace Moviebase.Core.Components
{
    /// <summary>
    /// Provides file system search support.
    /// </summary>
    public interface IFileScanner
    {
        /// <summary>
        /// Gets or sets movie file detection based on file extension.
        /// </summary>
        List<string> MovieExtensions { get; set; }

        /// <summary>
        /// Search for movie files in specified <paramref name="path"/>.
        /// </summary>
        /// <param name="path">Full path to directory to be scanned.</param>
        /// <returns><see cref="IEnumerable{T}"/> of file paths.</returns>
        IEnumerable<string> ScanMovie(string path);
    }
}