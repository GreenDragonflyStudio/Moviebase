using System.Threading.Tasks;

namespace Moviebase.Services.Title
{
    public interface ITitleProvider
    {
        void AddProvider(ITitleProvider provider);
        Task<string> GuessTitle(string filename);
    }
}
