using System.Collections.Generic;

namespace Moviebase.Models
{
    public  class MovieItem
    {
        public string Title { get; set; }
        public string Year { get; set; }
        public int ID { get; set; }
        public float Rating { get; set; }
        public bool Synced { get; set; }
        public int[] GenreID { get; set; }
        public string ImageLocation { get; set; }
        public string Synopsis { get; set; }
    }
    public class Genre
    {
        
        public List<MovieItem> Albums { get; set; }
    }
     
     

    public static class SampleData
    {
        public static List<Genre> Genres { get; set; } 
        public static List<MovieItem> Movies { get; set; }

        static SampleData()
        {
            Seed();
        }

        public static void Seed()
        {
            if (Genres != null)
                return;

            Genres = new List<Genre>
            {
                new Genre { Name = "Horror" }, 
                new Genre { Name = "Romance" },
                new Genre { Name = "Comedy" },
                new Genre { Name = "School" },
                new Genre { Name = "Action" },
                new Genre { Name = "Thiller" },
                new Genre { Name = "Sci-Fi" }
            };
            var i = 0;
            Movies = new List<MovieItem>
            {
                new MovieItem{ ID = i++, Title = "AAAAA", Year="2018", Rating = 9.0f, Synced = false },
                new MovieItem{ ID = i++, Title = "VVVVV", Year="2018", Rating =  7.8F, Synced = true },
                new MovieItem{ ID = i++, Title = "CCCCCC", Year="2016", Rating =  8.0f, Synced = false },
                new MovieItem{ ID = i++, Title = "DDDDD", Year="2015", Rating =  8.0f, Synced = true },
                new MovieItem{ ID = i++, Title = "EEEEEE", Year="2017", Rating = 6.0f, Synced = true },
                new MovieItem{ ID = i++, Title = "WWWWWW", Year="2014", Rating =  9.0f, Synced = false },
                new MovieItem{ ID = i++, Title = "QQQQQQ", Year="2017", Rating =  7.0f, Synced =  true },
                new MovieItem{ ID = i++, Title = "TTTTTTT", Year="2019", Rating =  9.0f, Synced = false }
            };
        }
    }
}
