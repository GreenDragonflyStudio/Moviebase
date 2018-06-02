using System.Threading.Tasks;
using Moviebase.Services.Entities;

namespace Moviebase.Services.Title
{
    public interface ITitleProvider
    {
        void AddProvider(ITitleProvider provider);

        Task<GuessTitle> GuessTitle(string filename);
    }
}