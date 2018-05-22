using System;

namespace Moviebase.DAL.Entities
{
    public class Movie
    {
        public int MovieId { get; set; }

        public string Title { get; set; }
        public DateTime ReleaseDate { get; set; }
        public int[] GenreIds { get; set; }
        public string Synopsis { get; set; }

        public float Rating { get; set; }
        public bool Synced { get; set; }
        public string ImageLocation { get; set; }
    }
}
