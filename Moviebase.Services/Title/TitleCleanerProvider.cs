using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Moviebase.Services.Entities;
using Moviebase.Services.Helpers;

namespace Moviebase.Services.Title
{
    public class TitleCleanerProvider : ITitleProvider
    {
        private const string SpecialMarker = "§=§";
        private static readonly string[] QualityWords = new[] { "320p", "480p", "720p", "1080p" };
        private static readonly string[] Languages;
        private static readonly List<string> ReservedWords;
        private static readonly string[] SpaceChars = new[] { ".", "_", " " };

        static TitleCleanerProvider()
        {
            ReservedWords = new List<string>(new[]
            {
                SpecialMarker, "hevc", "bdrip", "dvdrip", "Bluray", "x264", "h264", "AC3", "DTS", "hd","cam"
            });
            ReservedWords.AddRange(QualityWords);
            var cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
            var l = cultures.Select(x => x.EnglishName).ToList();
            l.AddRange(cultures.Select(x => x.ThreeLetterISOLanguageName));
            Languages = l.Distinct().ToArray();
        }
        
        public void AddProvider(ITitleProvider provider)
        {
            throw new NotImplementedException();
        }

        public async Task<GuessTitle> GuessTitle(string filename)
        {
            await Task.Yield();
            var temp = Path.GetFileNameWithoutExtension(filename);
            int? maybeYear = null;

            // Remove what's inside brackets trying to keep year info.
            temp = RemoveBrackets(temp, '{', '}', ref maybeYear);
            temp = RemoveBrackets(temp, '[', ']', ref maybeYear);
            temp = RemoveBrackets(temp, '(', ')', ref maybeYear);

            // Removes special markers (codec, formats, ecc...)
            var tokens = temp.Split(SpaceChars, StringSplitOptions.RemoveEmptyEntries);
            var title = string.Empty;
            foreach (var tok in tokens)
            {
                if (ReservedWords.Any(x => tok.StartsWith(x, StringComparison.OrdinalIgnoreCase)))
                {
                    if (title.Length > 0) break;
                }
                else
                {
                    title = string.Join(" ", title, tok).Trim();
                }
            }

            temp = title;
            tokens = temp.Split(SpaceChars, StringSplitOptions.RemoveEmptyEntries);
            for (var i = tokens.Length - 1; i >= 0; i--)
            {
                var tok = tokens[i];
                if (Languages.Any(x => string.Equals(x, tok, StringComparison.OrdinalIgnoreCase)))
                    tokens[i] = string.Empty;
                else
                    break;
            }
            title = string.Join(" ", tokens).Trim();

            // If year is not found inside parenthesis try to catch at the end, just after the title
            if (!maybeYear.HasValue)
            {
                var resplit = title.Split(SpaceChars, StringSplitOptions.RemoveEmptyEntries);
                var last = resplit.Last();
                if (LooksLikeYear(last))
                {
                    maybeYear = int.Parse(last);
                    title = title.Replace(last, string.Empty).Trim();
                }
            }
            var res = new GuessTitle
            {
                Year = maybeYear ?? 0
            };

            var rx = new Regex(@"([^\s]+)\s*-\s*([^\n]+)", RegexOptions.Compiled & RegexOptions.IgnoreCase);
            var a = rx.Match(title + "\n");
            if (a.Success)
            {
                res.Title = a.Groups[0].Value;
                res.SubTitle = a.Groups[1].Value;
            }
            else
            {
                res.Title = title;
            }

            return res;
        }

        private static bool LooksLikeYear(string dataRound)
        {
            return Regex.IsMatch(dataRound, " ^ (19|20)[0-9][0-9]");
        }

        private static string RemoveBrackets(string inputString, char openChar, char closeChar, ref int? maybeYear)
        {
            var str = inputString;
            while (str.IndexOf(openChar) >= 1 && str.IndexOf(closeChar) >= 1)
            {
                var dataGraph = str.GetBetween(openChar.ToString(), closeChar.ToString());
                if (LooksLikeYear(dataGraph))
                {
                    maybeYear = int.Parse(dataGraph.Substring(0, 4));
                }
                else
                {
                    var parts = dataGraph.Split(SpaceChars, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var part in parts)
                        if (LooksLikeYear(part))
                        {
                            maybeYear = int.Parse(part);
                            break;
                        }
                }
                str = str.ReplaceBetween(openChar, closeChar, $" {SpecialMarker} ");
            }
            return str;
        }

    }
}