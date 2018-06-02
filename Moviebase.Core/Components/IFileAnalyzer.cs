using System.Collections.Generic;
using System.Threading.Tasks;
using Moviebase.Core.Utils;

namespace Moviebase.Core.Components
{
    /// <summary>
    /// Provides file metadata analysis.
    /// </summary>
    public interface IFileAnalyzer
    {
        List<string> SubtitleExtensions { get; set; }

        List<string> PosterExtensions { get; set; }


        /// <summary>
        /// Analyze specified file for raw metadata.
        /// </summary>
        /// <param name="filePath">Full path to file.</param>
        /// <returns><see cref="AnalyzedFile"/> object which contains barely guessed movie metadata.</returns>
        Task<AnalyzedFile> Analyze(string filePath);
    }
}