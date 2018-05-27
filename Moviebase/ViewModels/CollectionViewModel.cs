using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Threading;
using LiteDB;
using Moviebase.Core;
using Moviebase.DAL;
using Moviebase.DAL.Entities;
using Ninject;

namespace Moviebase.ViewModels
{
    public class CollectionViewModel : BindableBase
    {
        private Movie _SelectedMovie;

        public Movie SelectedMovie
        {
            get
            {
                return _SelectedMovie;
            }
            set
            {
                _SelectedMovie = value;
                OnPropertyChanged("SelectedMovie");
            }
        }

        public ObservableCollection<Movie> Movies { get; } = new ObservableCollection<Movie>();
        private DispatcherTimer timer;

        public ICommand AddCommand { get; set; }

        public CollectionViewModel()
        {
            //using (var db = new LiteDatabase(GlobalSettings.Default.ConnectionString))
            //{
            //    var movies = db.GetCollection<Movie>();
            //    foreach (var movie in movies.FindAll())
            //    {
            //        Movies.Add(movie);
            //    }
            //}

            AddCommand = new DelegateCommand(async () =>
            {
                var app = App.Kernel.Get<MoviebaseApp>();
                var result = await app.ScanAsync("E:\\Programming\\Sandbox\\Moviebase");
                foreach (var movie in result)
                {
                    Movies.Add(movie);
                }
            });
        }



        private void Timer_Tick(object sender, EventArgs e)
        {
            // Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(action));
        }

        public void Start()
        {
            //timer.Start();
        }

        public void Stop()
        {
            //timer.Stop();
        }
    }
}