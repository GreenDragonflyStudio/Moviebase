using System.IO;
using System.Linq;
using System.Text;

namespace Moviebase.Core.Components
{
    /// <inheritdoc />
    public class PathTransformer : IPathTransformer
    {
        /// <summary>
        /// This is the default token, which is similar to <c>%collection%\%title%\(%year%) %title%</c>."
        /// </summary>
        public static readonly string DefaultTemplate = string.Format("{0}\\{1}\\({2}) {1}", PathToken.CollectionToken, PathToken.TitleToken, PathToken.YearToken);

        /// <inheritdoc />
        public string TargetPath { get; set; }

        /// <inheritdoc />
        public string TokenTemplate { get; set; }

        /// <inheritdoc />
        public bool SwapThe { get; set; }

        /// <summary>
        /// Check the specified template, returns <c>True</c> if the template is valid, otherwise <c>False</c>.
        /// </summary>
        /// <param name="value">Template to be validated</param>
        /// <returns>Returns <c>True</c> if the template is valid, otherwise <c>False</c>.</returns>
        public static bool IsTemplateValid(string value)
        {
            var tokens = new[] {PathToken.AllGenresToken, PathToken.CollectionToken, PathToken.EpisodeToken, PathToken.ScreenSizeToken, PathToken.GenreToken, PathToken.TitleToken, PathToken.YearToken };
            return !tokens.Aggregate(value, (current, token) => current.Replace(token, string.Empty)).Contains("%");
        }

        /// <inheritdoc />
        public string GetTokenizedFilePath(string filePath, PathToken token)
        {
            var sb = new StringBuilder(TokenTemplate);

            // replace with tokens
            sb.Replace(PathToken.TitleToken, token.Title);
            sb.Replace(PathToken.CollectionToken, token.Collection);
            sb.Replace(PathToken.EpisodeToken, token.Episode);
            sb.Replace(PathToken.ScreenSizeToken, token.ScreenSize);
            sb.Replace(PathToken.YearToken, token.Year);
            sb.Replace(PathToken.GenreToken, token.Genres.FirstOrDefault()?.Name ?? "");
            sb.Replace(PathToken.AllGenresToken, string.Join(",", token.Genres.Select(x => x.Name)));

            // swap the
            if (SwapThe && sb.ToString().StartsWith("The"))
            {
                sb.Remove(0, 3);
                sb.Append(", The");
            }

            // add extension
            sb.Append("." + Path.GetExtension(filePath));
            sb.Replace(@"\\", @"\");

            return Path.Combine(TargetPath, sb.ToString());
        }
    }
}