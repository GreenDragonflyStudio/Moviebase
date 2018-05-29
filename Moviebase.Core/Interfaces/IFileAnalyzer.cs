using System.IO;

namespace Moviebase.Core.Components
{
    public interface IFileAnalyzer
    {
        AnalyzedItem Analyze(FileInfo file);
    }
}