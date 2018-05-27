using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Moviebase.Services.Entities;

namespace Moviebase.Services.Title
{
    public class ImdbTitleProvider : ITitleProvider
    {
        private readonly Regex _imdbRegex;

        public ImdbTitleProvider()
        {
            _imdbRegex = new Regex("[0-9]{7}", RegexOptions.Compiled);
        }

        public void AddProvider(ITitleProvider provider)
        {
            throw new NotSupportedException("Could not add provider to this instance.");
        }

        public async Task<GuessTitle> GuessTitle(string filename)
        {
            await Task.Yield();

            var matched = _imdbRegex.Match(filename);
            return matched.Success ? new GuessTitle {ImdbId = matched.Value} : null;
        }
    }
}