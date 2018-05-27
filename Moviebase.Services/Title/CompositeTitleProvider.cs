using System.Collections.Generic;
using System.Threading.Tasks;
using Moviebase.Services.Entities;

namespace Moviebase.Services.Title
{
    public class CompositeTitleProvider : ITitleProvider
    {
        private readonly List<ITitleProvider> _providers = new List<ITitleProvider>();

        public CompositeTitleProvider()
        {
            AddProvider(new ImdbTitleProvider());
            AddProvider(new GuessitTitleProvider());
            AddProvider(new TitleCleanerProvider());
        }

        public void AddProvider(ITitleProvider provider)
        {
            _providers.Add(provider);
        }

        public async Task<GuessTitle> GuessTitle(string filename)
        {
            GuessTitle result = null;
            foreach (var titleProvider in _providers)
            {
                try
                {
                    result = await titleProvider.GuessTitle(filename);
                }
                catch
                {
                    result = null;
                }

                if (!string.IsNullOrWhiteSpace(result?.ImdbId) || !string.IsNullOrWhiteSpace(result?.Title)) break;
            }
            return result;
        }
    }
}