namespace Moviebase.Core.Components
{
    /// <summary>
    /// Path transformation using tokens.
    /// </summary>
    public interface IPathTokenizer
    {
        /// <summary>
        /// Token template used to transform path.
        /// </summary>
        string TokenTemplate { get; set; }

        /// <summary>
        /// Specifies target path to be used as root path.
        /// </summary>
        string TargetPath { get; set; }


        /// <summary>
        /// Transform specified file path using specified <see cref="TokenTemplate"/> and <see cref="TargetPath"/>.
        /// </summary>
        /// <param name="filePath">Full path to file.</param>
        /// <param name="token"><see cref="PathToken"/> object containing token informations.</param>
        /// <returns>Full path to new file path. File directory is changed with the <see cref="TargetPath"/> and its file name with tokeninzed file name.</returns>
        /// <remarks>
        /// <para>
        /// This method transforms the specified file path using specified <see cref="TokenTemplate"/> and <see cref="TargetPath"/>.
        /// It's working behavior is straightforward. This method will create new filename using the template and information from 
        /// <paramref name="token"/> parameter. Then the new filename will be appended with <see cref="TargetPath"/>. The only
        /// reason you have to supply fullpath to file to <paramref name="filePath"/> is to get the original file extension.
        /// </para>
        /// </remarks>
        string GetTokenizedFilePath(string filePath, PathToken token);
    }
}