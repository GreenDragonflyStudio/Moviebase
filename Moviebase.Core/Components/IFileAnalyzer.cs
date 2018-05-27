using System.Threading.Tasks;

namespace Moviebase.Core.Components
{
    public interface IFileAnalyzer
    {
        Task<AnalyzedFile> Analyze(string file);
    }
}