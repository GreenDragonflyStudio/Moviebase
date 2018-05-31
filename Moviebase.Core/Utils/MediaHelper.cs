using MediaInfoDotNet;

namespace Moviebase.Core.Utils
{
    /// <summary>
    /// Provides a proxy for MediaInfo library.
    /// </summary>
    public static class MediaHelper
    {
        /// <summary>
        /// Get information about specified media file.
        /// </summary>
        /// <param name="fullPath">Full path to file.</param>
        /// <returns><see cref="MediaFile"/> object containing media information.</returns>
        public static MediaFile GetMediaInfo(string fullPath)
        {
            return new MediaFile(fullPath);
        }
    }
}
