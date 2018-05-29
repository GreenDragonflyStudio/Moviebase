using System;
using System.Collections.Generic;

namespace Moviebase.DAL.Entities
{
    public class Movie
    {
        public int Id { get; set; }

        public int TmdbId { get; set; }
        public string ImdbId { get; set; }

        public string Title { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public int Year => ReleaseDate?.Year ?? 0;
        public string Overview { get; set; }

        public bool Adult { get; set; }
        public string Collection { get; set; }
        public TimeSpan Duration { get; set; }
        public IEnumerable<Genre> Genres { get; set; }

        public string PosterPath { get; set; }
        public string ImageUri { get; set; }
        public string OriginalLanguage { get; set; }
        public string OriginalTitle { get; set; }
        public double Popularity { get; set; }
        public double VoteAverage { get; set; }
        public int VoteCount { get; set; }
    }
}