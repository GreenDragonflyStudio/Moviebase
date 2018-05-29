using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace Moviebase.Core
{
    /// <summary>
    /// Top level implementation of Moviebase functionality.
    /// </summary>
    public interface IMoviebaseApp
    {
        /// <summary>
        /// Progress changed event.
        /// </summary>
        event ProgressChangedEventHandler ProgressChanged;

        /// <summary>
        /// Scans directory for movie files.
        /// </summary>
        /// <param name="scanPath">Full path to directory being scanned.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the scan process.</param>
        /// <returns><see cref="Task"/> object of current process.</returns>
        Task ScanDirectoryAsync(string scanPath, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Scans file and record to database.
        /// </summary>
        /// <param name="filePath">Full path to movie file.</param>
        /// <returns><see cref="Task"/> object of current process.</returns>
        Task ScanFileAsync(string filePath);

        /// <summary>
        /// Organize files on database to designated folder.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token to cancel the scan process.</param>
        /// <returns><see cref="Task"/> object of current process.</returns>
        Task OrganizeFileAsync(CancellationToken? cancellationToken = null);
    }
}