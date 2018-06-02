using Moviebase.Services.Helpers;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Moviebase.Services.Entities;

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

        public async Task<GuessTitle> GuessTitle(string filename)
        {
            try
            {
                var output = await ProcessHelper.StartWithOutput(AppName, string.Format(Arguments, filename), RedirectStream.StandardOutput);
                var type = new {title = "", year = 0, screen_size=""};
                var obj = JsonConvert.DeserializeAnonymousType(output, type);
                return new GuessTitle
                {
                    Title = obj.title,
                    Year = obj.year,
                    ScreenSize = obj.screen_size
                };
            }
            catch (Exception e)
            {
                Debug.Print("GuessIt error: " + e);
                return null;
            }
        }
    }
}