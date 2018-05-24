using System;
using System.Collections.Generic;

namespace Moviebase.Core.Models
{
    public class Movie
    {
        #region Properties

        public bool Adult { get; set; }
        public string Collection { get; set; }
        public TimeSpan Duration { get; set; }
        public IEnumerable<MovieGenre> Genres { get; set; }
        public int Id { get; set; }

        public string ImageUri { get; set; }
        public string ImdbId { get; set; }
        public string OriginalLanguage { get; set; }
        public string OriginalTitle { get; set; }
        public string Overview { get; set; }
        public double Popularity { get; set; }
        public string PosterPath { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public string Title { get; set; }
        public double VoteAverage { get; set; }
        public int VoteCount { get; set; }
        public int? Year => ReleaseDate?.Year;

        #endregion Properties
    }
}