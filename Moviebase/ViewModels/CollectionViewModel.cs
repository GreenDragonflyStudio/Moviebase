using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Threading;
using LiteDB;
using Moviebase.Core;
using Moviebase.Core.App;
using Moviebase.DAL;
using Moviebase.DAL.Entities;
using Ninject;

namespace Moviebase.ViewModels
{
    public class CollectionViewModel : BindableBase
    {
        private IMoviebaseApp _app;

        public ObservableCollection<Movie> Movies { get; } = new ObservableCollection<Movie>();
        public virtual Movie SelectedMovie { get; set; }
        public ICommand AddCommand { get; set; }
        public ICommand RemoveCommand { get; set; }

        public CollectionViewModel(IMoviebaseApp app)
        {
            AddCommand = new DelegateCommand(AddCommand_Handler);

            // load data
            using (var db = new LiteDatabase(GlobalSettings.Default.ConnectionString))
            {
                var col = db.GetCollection<Movie>();
                foreach (var movie in col.FindAll())
                {
                    Movies.Add(movie);
                }
            }
        }

        private void AddCommand_Handler()
        {
            // show dialog to browse single movie file
            _app.ScanFileAsync("");
        }
    }
}