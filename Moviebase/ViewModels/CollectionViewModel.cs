using System.Collections.ObjectModel;
using System.Windows.Input;
using LiteDB;
using Moviebase.Core.App;
using Moviebase.DAL;
using Moviebase.DAL.Entities;

namespace Moviebase.ViewModels
{
    public class CollectionViewModel : BindableBase
    {
        private IMoviebaseApp _app;
        private Movie _selectedMovie;

        public ObservableCollection<Movie> Movies { get; } = new ObservableCollection<Movie>();
        public ICommand AddCommand { get; set; }
        public ICommand RemoveCommand { get; set; }

        public Movie SelectedMovie
        {
            get => _selectedMovie;
            set
            {
                _selectedMovie = value;
                OnPropertyChanged();
            }
        }

        public CollectionViewModel()
        {
            _app = MoviebaseApp.Instance;   

            AddCommand = new DelegateCommand(AddCommandCallback);
            RemoveCommand = new DelegateCommand(RemoveCommandCallback);

            PopulateData();
        }

        private void AddCommandCallback()
        {
            // show dialog to browse single movie file
            _app.ScanFileAsync(""); // add a single file
            _app.ScanDirectoryAsync(""); // add a directory
        }

        private void RemoveCommandCallback()
        {
            using (var db = new LiteDatabase(GlobalSettings.Default.ConnectionString))
            {
                var movies = db.GetCollection<Movie>();
                movies.Delete(x => x.Id == SelectedMovie.Id);

                var mediaFiles = db.GetCollection<MediaFile>();
                mediaFiles.Delete(x => x.TmdbId == SelectedMovie.TmdbId);
            }

            PopulateData();
        }

        private void PopulateData()
        {
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