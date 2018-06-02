using System.Collections.Generic;
using Moviebase.DAL.Entities;

namespace Moviebase.Core.Components
{
    /// <summary>
    /// Defines token informations.
    /// </summary>
    public class PathToken
    {
        /// <summary>
        /// Title token pattern.
        /// </summary>
        public const string TitleToken = "%title%";
        /// <summary>
        /// Collection token pattern.
        /// </summary>
        public const string CollectionToken = "%collection%";
        /// <summary>
        /// Episode token pattern.
        /// </summary>
        public const string EpisodeToken = "%episode%";
        /// <summary>
        /// Screen size token pattern.
        /// </summary>
        public const string ScreenSizeToken = "%screen_size%";
        /// <summary>
        /// Year token pattern.
        /// </summary>
        public const string YearToken = "%year%";
        /// <summary>
        /// Genre token pattern.
        /// </summary>
        public const string GenreToken = "%first_genre%";
        /// <summary>
        /// All genre token pattern.
        /// </summary>
        public const string AllGenresToken = "%allgenres%";

        /// <summary>
        /// Initialize new instance of <see cref="PathToken"/>.
        /// </summary>
        /// <param name="media"><see cref="MediaFile"/> object which contains information about certain file.</param>
        /// <param name="movie"><see cref="Movie"/> object which contains <paramref name="media"/> metadata.</param>
        public PathToken(MediaFile media, Movie movie)
        {
            Title = movie.Title;
            Genres = movie.Genres;
            Collection = movie.Collection;
            Episode = media.Episode.ToString();
            ScreenSize = media.ScreenSize;
            Year = movie.Year.ToString();
        }

        /// <summary>
        /// Movie title.
        /// </summary>
        public string Title { get; }
        /// <summary>
        /// Movie genre(s).
        /// </summary>
        public IEnumerable<Genre> Genres { get; }
        /// <summary>
        /// Movie collection.
        /// </summary>
        public string Collection { get; }
        /// <summary>
        /// Episode number.
        /// </summary>
        public string Episode { get; }
        /// <summary>
        /// Screen size.
        /// </summary>
        public string ScreenSize { get; }
        /// <summary>
        /// Movie release year.
        /// </summary>
        public string Year { get; }
    }
}
