using System.IO;

namespace Moviebase.Core.Components.MediaAnalyzer
{
    public interface IFileAnalyzer
    {
        AnalyzedItem Analyze(FileInfo file);
    }
}