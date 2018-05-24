using Moviebase.DAL.Entities;
using System.Data.Entity;

namespace Moviebase.DAL
{
    public class MoviebaseContext : DbContext
    {
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Folder> Folders { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var sqliteConnectionInitializer = new MoviebaseContextInitializer(modelBuilder);
            Database.SetInitializer(sqliteConnectionInitializer);
        }
    }
}