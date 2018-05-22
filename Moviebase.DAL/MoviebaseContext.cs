using System.Data.Entity;
using Moviebase.DAL.Entities;

namespace Moviebase.DAL
{
    public class MoviebaseContext : DbContext
    {
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Folder> Folders { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
    }
}
