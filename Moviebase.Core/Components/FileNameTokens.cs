using System.Linq;

namespace Moviebase.Core.Components
{
    public static class FileNameTokens
    {
        public const string AllGenres = "%allgenres%";
        public const string Collection = "%collection%";
        public const string Extension = "%ext%";
        public const string Genre = "%firstgenre%";
        public const string Title = "%title%";
        public const string Year = "%year%";

        public static bool IsTemplateValid(string value)
        {
            var tokens = new[] { Title, Extension, Year, Collection, Genre };
            return !tokens.Aggregate(value, (current, token) => current.Replace(token, string.Empty)).Contains("%");
        }
    }
}