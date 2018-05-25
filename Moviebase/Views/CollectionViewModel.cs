using Moviebase.Helper;
using Moviebase.Core.Models;
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
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(action));
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