using System.Collections.ObjectModel;
using System.Windows.Input;
using LiteDB;
using Moviebase.Core.App;
using Moviebase.DAL;
using Moviebase.DAL.Entities;
using Ookii.Dialogs.Wpf;

namespace Moviebase.ViewModels
{
    public class CollectionViewModel : BindableBase
    {
        private IMoviebaseApp _app;
        private Movie _selectedMovie;

        public ObservableCollection<Movie> Movies { get; } = new ObservableCollection<Movie>();
        public ICommand AddFolderCommand { get; set; }
        public ICommand AddFileCommand { get; set; }
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
            _app.ProgressChanged += _app_ProgressChanged;
            AddFolderCommand = new DelegateCommand(AddFolderCommandCallback);
            AddFileCommand = new DelegateCommand(AddFileCommandCallback);
            RemoveCommand = new DelegateCommand(RemoveCommandCallback);

            PopulateData();
        }

        private void _app_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == 100)
            {
                PopulateData();
            }
        }

        private void AddFolderCommandCallback()
        {
            var a = new VistaFolderBrowserDialog();
            a.Description = "Moviebase - Browse Observed Folder";
            a.ShowNewFolderButton = true;
            a.UseDescriptionForTitle = true;
            if (a.ShowDialog() == true)
            {
                // show dialog to browse single movie file
                _app.ScanDirectoryAsync(a.SelectedPath); // add a directory
            }
        }

        private void AddFileCommandCallback()
        {
            var dlg = new VistaOpenFileDialog
            {
                Title = "Select Movie to add into collection",
                CheckFileExists = true
            };
            if (dlg.ShowDialog() == true)
            {
                foreach (var i in dlg.FileNames)
                {
                    _app.ScanFileAsync(i); // add a single file
                }
            }
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
            Movies.Clear();
            using (var db = new LiteDatabase(GlobalSettings.Default.ConnectionString))
            {
                var col = db.GetCollection<Movie>();
                foreach (var movie in col.FindAll())
                {
                    Movies.Add(movie);
                }
            }
        }
    }
}