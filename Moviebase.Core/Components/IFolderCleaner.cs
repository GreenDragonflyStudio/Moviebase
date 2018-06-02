namespace Moviebase.Core.Components
{
    /// <summary>
    /// Delete empty folders.
    /// </summary>
    public interface IFolderCleaner
    {
        /// <summary>
        /// Clean specified directory from empty folder(s).
        /// </summary>
        /// <param name="directory">Full path to directort being cleaned.</param>
        void Clean(string directory);
    }
}