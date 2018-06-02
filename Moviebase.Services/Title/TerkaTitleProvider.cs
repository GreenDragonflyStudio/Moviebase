using Moviebase.Services.Helpers;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Moviebase.Services.Entities;

namespace Moviebase.Services.Title
{
    public class TerkaTitleProvider : ITitleProvider
    {
        public void AddProvider(ITitleProvider provider)
        {
            throw new NotSupportedException("Could not add provider to this instance.");
        }

        public async Task<GuessTitle> GuessTitle(string filename)
        {
            return null; // TODO : Future Implementation
        }
    }
}