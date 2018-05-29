using Moviebase.DAL.Entities;

namespace Moviebase.Core.Components
{
    /// <summary>
    /// Organize <see cref="MediaFile"/> into designated directory.
    /// </summary>
    public interface IFileOrganizer
    {
        /// <summary>
        /// Organize specified <paramref name="media"/>.
        /// </summary>
        /// <param name="media"><see cref="MediaFile"/> object about the file.</param>
        void Organize(MediaFile media);
    }
}