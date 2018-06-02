namespace Moviebase.Core.Components
{
    /// <summary>
    /// Path transformation using tokens.
    /// </summary>
    public interface IPathTransformer
    {
        /// <summary>
        /// Gets or sets resulting target path.
        /// </summary>
        string TargetPath { get; set; }

        /// <summary>
        /// Token template used to transform path.
        /// </summary>
        string TokenTemplate { get; set; }
        
        /// <summary>
        /// Gets or sets if the file name starts with "The" file name it will be swapped to last.
        /// </summary>
        bool SwapThe { get; set; }

        /// <summary>
        /// Transform specified file path using specified <see cref="TokenTemplate"/>.
        /// </summary>
        /// <param name="filePath">Full path to file.</param>
        /// <param name="token"><see cref="PathToken"/> object containing token informations.</param>
        /// <returns>Full path to new file path after transformations.</returns>
        /// <remarks>
        /// <para>
        /// This method transforms the specified file path using specified <see cref="TokenTemplate"/> and <see cref="TargetPath"/>.
        /// It's working behavior is straightforward. This method will create new filename using the template and information from 
        /// <paramref name="token"/> parameter. The only reason you have to supply fullpath to file to <paramref name="filePath"/> 
        /// is to get the original file extension.
        /// </para>
        /// </remarks>
        string GetTokenizedFilePath(string filePath, PathToken token);
    }
}