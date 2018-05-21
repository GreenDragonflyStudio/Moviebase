using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Moviebase.Services.Helpers;
using Newtonsoft.Json;

namespace Moviebase.Services.Title
{
    public class GuessitTitleProvider : ITitleProvider
    {
        private const string AppName = "guessit";
        private const string Arguments = "-j \"{0}\"";

        public void AddProvider(ITitleProvider provider)
        {
            throw new NotSupportedException("Could not add provider to this instance.");
        }

        public async Task<string> GuessTitle(string filename)
        {
            try
            {
                var output = await ProcessHelper.StartWithOutput(AppName, string.Format(Arguments, filename), RedirectStream.StandardOutput);
                return JsonConvert.DeserializeAnonymousType(output, new { title = "" }).title;
            }
            catch (Exception e)
            {
                Debug.Print("GuessIt error: " + e);
                return null;
            }
        }
    }
}
