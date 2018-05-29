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
        //private Movie _SelectedMovie;

        //public Movie SelectedMovie
        //{
        //    get => _SelectedMovie;
        //    set => SetProperty(ref _SelectedMovie, value);
        //}

        public virtual Movie SelectedMovie { get; set; }

        public ObservableCollection<Movie> Movies { get; } = new ObservableCollection<Movie>();
        private DispatcherTimer timer;

        public ICommand AddCommand { get; set; }

        public CollectionViewModel()
        {
            using (var db = new LiteDatabase(GlobalSettings.Default.ConnectionString))
            {
                var col = db.GetCollection<Movie>();
                foreach (var movie in col.FindAll())
                {
                    Movies.Add(movie);
                }
            }

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