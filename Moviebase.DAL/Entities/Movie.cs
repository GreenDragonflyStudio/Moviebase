using System;
using System.Collections.Generic;

namespace Moviebase.DAL.Entities
{
    public class Movie
    {
        public int MovieId { get; set; }

        public string Title { get; set; }
        public DateTime ReleaseDate { get; set; }
        public ICollection<Genre> Genres { get; set; }
        public string Synopsis { get; set; }
        public bool IsSerial { get; set; }

        public string ImagePath { get; set; }
        public string SubtitlePath { get; set; }
        public float Rating { get; set; }
        public bool Synced { get; set; }
    }
}
