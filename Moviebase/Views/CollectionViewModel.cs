using Moviebase.DAL.Entities;
using Moviebase.Helper;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;

namespace Moviebase.ViewModels
{
    public class CollectionViewModel : BindableBase
    {
        private Movie _SelectedMovie;

        public Movie SelectedMovie { get { return _SelectedMovie; } set { _SelectedMovie = value; OnPropertyChanged("SelectedMovie"); } }
        public ObservableCollection<Movie> Movies { get; } = new ObservableCollection<Movie>();
        private DispatcherTimer timer;
        private Random random;

        public CollectionViewModel()
        {
            random = new Random(Int32.MaxValue);
            timer = new DispatcherTimer(DispatcherPriority.Background, Application.Current.Dispatcher);
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (Movies.Count >= 5)
            {
                Stop();
                NotificationProvider.Notify("Load Complete");
            }

            //Action action = () => { foreach (Movie i in Models.SampleData.Movies) Movies.Add(i); };
            //Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(action));
        }

        public void Start()
        {
            timer.Start();
        }

        public void Stop()
        {
            timer.Stop();
        }
    }
}