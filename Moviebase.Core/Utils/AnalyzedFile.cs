namespace Moviebase.Core.Utils
{
    /// <summary>
    /// Represent an analyzed file.
    /// </summary>
    /// <remarks>This class contains least metadata about a file which is being identified.</remarks>
    public class AnalyzedFile
    {
        /// <summary>
        /// Initialize new instance of <see cref="AnalyzedFile"/>.
        /// </summary>
        /// <param name="filePath">Full path to file.</param>
        public AnalyzedFile(string filePath)
        {
            FullPath = filePath;
        }

        /// <summary>
        /// Gets a value representing the current file is known and stored in database.
        /// </summary>
        /// <remarks><see cref="IsKnown"/> property only set to true if the file is exist in the hash table and media file table.</remarks>
        public bool IsKnown { get; set; }
        /// <summary>
        /// Full path to file.
        /// </summary>
        public string FullPath { get; }
        /// <summary>
        /// File hash using MD5.
        /// </summary>
        public string Hash { get; set; }

        /// <summary>
        /// Gets the gussed IMDB ID.
        /// </summary>
        public string ImdbId { get; set; }
        /// <summary>
        /// Gets the guessed movie title.
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Gets the guessed movie release year.
        /// </summary>
        public int Year { get; set; }
    }
}