using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace Moviebase.Core.App
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
        /// Associate folder content on target path to MediaFile table.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token to cancel the sync process.</param>
        /// <returns><see cref="Task"/> object of current process.</returns>
        /// <remarks>
        /// <para>
        /// This method will scan the target path with structured directory for subtitles and poster.
        /// Each movie file will be assigned with their subtitle and poster on MediaFile table only
        /// if the file rename pattern is using structured directory pattern.
        /// </para>
        /// <para>
        /// If you're using non structured directory for movies, you will get wrong subtitle and poster
        /// assignment because it's not implemented yet to distinguish subtitle and movie file.
        /// </para>
        /// </remarks>
        Task AssociateAsync(CancellationToken? cancellationToken = null);

        /// <summary>
        /// Organize files on database to designated folder.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token to cancel the scan process.</param>
        /// <returns><see cref="Task"/> object of current process.</returns>
        Task OrganizeFileAsync(CancellationToken? cancellationToken = null);
    }
}