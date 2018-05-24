using System.Collections.Generic;
using System.Threading.Tasks;

namespace Moviebase.Services.Title
{
    public class CompositeTitleProvider : ITitleProvider
    {
        private readonly List<ITitleProvider> _providers = new List<ITitleProvider>();

        public void AddProvider(ITitleProvider provider)
        {
            _providers.Add(provider);
        }

        public async Task<string> GuessTitle(string filename)
        {
            string result = null;
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

                if (!string.IsNullOrWhiteSpace(result)) break;
            }
            return result;
        }
    }
}