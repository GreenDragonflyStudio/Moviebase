using System.Collections.Generic;

namespace Moviebase.Core.Components
{
    /// <summary>
    /// Provides file system search support.
    /// </summary>
    public interface IFileScanner
    {
        /// <summary>
        /// Search for movie files in specified <paramref name="path"/>.
        /// </summary>
        /// <param name="path">Full path to directory to be scanned.</param>
        /// <returns><see cref="IEnumerable{T}"/> of file paths.</returns>
        IEnumerable<string> ScanMovie(string path);

        /// <summary>
        /// Search for subtitle files in specified <paramref name="path"/>.
        /// </summary>
        /// <param name="path">Full path to directory to be scanned.</param>
        /// <returns><see cref="IEnumerable{T}"/> of file paths.</returns>
        IEnumerable<string> ScanSubtitle(string path);

        /// <summary>
        /// Search for poster images files in specified <paramref name="path"/>.
        /// </summary>
        /// <param name="path">Full path to directory to be scanned.</param>
        /// <returns><see cref="IEnumerable{T}"/> of file paths.</returns>
        IEnumerable<string> ScanPoster(string path);
    }
}