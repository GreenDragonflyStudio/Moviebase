using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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

        public async Task<string> GuessTitle(string filename)
        {
            await Task.Yield(); // TODO: is this the right usage of Task.Yield()?
            var matched = _imdbRegex.Match(filename);
            return matched.Success ? matched.Value : null;
        }
    }
}