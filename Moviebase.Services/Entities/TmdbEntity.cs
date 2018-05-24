using System;
using System.Collections.Generic;

namespace Moviebase.Services.Entities
{
    public class TmdbEntity
    {
        public int TmdbId { get; set; }
        public string ImdbId { get; set; }

        public string Title { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Genre { get; set; }
        public string Plot { get; set; }

        public List<string> AlternativeNames { get; set; }
        public List<string> PosterPath { get; set; }
    }
}