using System;
using System.Data.Entity;
using SQLite.CodeFirst;

namespace Moviebase.DAL
{
    public class MoviebaseContextInitializer : SqliteDropCreateDatabaseWhenModelChanges<MoviebaseContext>
    {
        public MoviebaseContextInitializer(DbModelBuilder modelBuilder) : base(modelBuilder)
        {
        }

        public MoviebaseContextInitializer(DbModelBuilder modelBuilder, Type historyEntityType) : base(modelBuilder, historyEntityType)
        {
        }

        protected override void Seed(MoviebaseContext context)
        {
            // add seed here

            base.Seed(context);
        }
    }
}
