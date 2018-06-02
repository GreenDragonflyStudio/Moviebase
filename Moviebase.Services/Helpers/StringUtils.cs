using System;
using System.Text.RegularExpressions;

namespace Moviebase.Services.Helpers
{
    public static class StringUtils
    {
        public static string GetBetween(this string src, string a, string b,
            StringComparison comparison = StringComparison.Ordinal)
        {
            var idxStr = src.IndexOf(a, comparison);
            var idxEnd = src.IndexOf(b, comparison);
            if (idxStr >= 0 && idxEnd > 0)
            {
                if (idxStr > idxEnd)
                    Swap(ref idxStr, ref idxEnd);
                return src.Substring(idxStr + a.Length, idxEnd - idxStr - a.Length);
            }
            return src;
        }

        private static void Swap<T>(ref T idxStr, ref T idxEnd)
        {
            var temp = idxEnd;
            idxEnd = idxStr;
            idxStr = temp;
        }

        public static string ReplaceBetween(this string s, char begin, char end, string replacement = null)
        {
            var regex = new Regex($"\\{begin}.*?\\{end}");
            return regex.Replace(s, replacement ?? string.Empty);
        }
    }
}