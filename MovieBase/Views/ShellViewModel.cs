using MahApps.Metro.IconPacks;
using MovieBase.Helper;
using MovieBase.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace MovieBase.ViewModels
{
    public class ShellViewModel : BindableBase
    {
        public DelegateCommand ClearNotificationCommand { get; } = new DelegateCommand(DoClearNotification);

        private static void DoClearNotification()
        {
            NotificationProvider.Clear(1);
        }

        public ShellViewModel()
        {
            // Build the menus
            this.Menu.Add(new MenuItem() { Icon = new PackIconMaterial() { Kind = PackIconMaterialKind.Home }, Text = "Home", NavigationDestination = new Uri("Views/HomeView.xaml", UriKind.RelativeOrAbsolute) });
            this.Menu.Add(new MenuItem() { Icon = new PackIconMaterial() { Kind = PackIconMaterialKind.Collage }, Text = "Collection", NavigationDestination = new Uri("Views/CollectionView.xaml", UriKind.RelativeOrAbsolute) });
            this.Menu.Add(new MenuItem() { Icon = new PackIconMaterial() { Kind = PackIconMaterialKind.Folder }, Text = "Folders", NavigationDestination = new Uri("Views/FoldersView.xaml", UriKind.RelativeOrAbsolute) });
            this.Menu.Add(new MenuItem() { Icon = new PackIconMaterial() { Kind = PackIconMaterialKind.Sync }, Text = "Synchronization", NavigationDestination = new Uri("Views/SynchronizeView.xaml", UriKind.RelativeOrAbsolute) });

            this.OptionsMenu.Add(new MenuItem() { Icon = new PackIconMaterial() { Kind = PackIconMaterialKind.Settings }, Text = "Settings", NavigationDestination = new Uri("Views/SettingsView.xaml", UriKind.RelativeOrAbsolute) });
            this.OptionsMenu.Add(new MenuItem() { Icon = new PackIconMaterial() { Kind = PackIconMaterialKind.Help }, Text = "About", NavigationDestination = new Uri("Views/AboutView.xaml", UriKind.RelativeOrAbsolute) });

            NotificationProvider.Notify("Hello World");
            NotificationProvider.Notify("[Ongoing] Antman & the Wasp", 2);
        }

        public object GetItem(object uri)
        {
            return null == uri ? null : this.Menu.FirstOrDefault(m => m.NavigationDestination.Equals(uri));
        }

        public object GetOptionsItem(object uri)
        {
            return null == uri ? null : this.OptionsMenu.FirstOrDefault(m => m.NavigationDestination.Equals(uri));
        }

        private static readonly ObservableCollection<MenuItem> AppMenu = new ObservableCollection<MenuItem>();
        private static readonly ObservableCollection<MenuItem> AppOptionsMenu = new ObservableCollection<MenuItem>();
        private ObservableCollection<NotificationItem> AppNotification = new ObservableCollection<NotificationItem>();
        private static readonly ObservableCollection<Models.MovieItem> UnsyncedMovies = new ObservableCollection<Models.MovieItem>();
        private readonly ObservableCollection<Models.MovieFolder> Folders = new ObservableCollection<Models.MovieFolder>();

        public string NotificationCount
        {
            get => (NotificationProvider.Count() == 0 ? "" : NotificationProvider.Count().ToString());
        }

        public ObservableCollection<NotificationItem> NotificationCollection
        {
            get
            {
                return NotificationProvider.Notifications;
            }
            set
            {
                NotificationProvider.Notifications = value;
                OnPropertyChanged("NotificationCollection");
            }
        }

        public ObservableCollection<MovieItem> UnsyncedCollection
        {
            get
            {
                return NotificationProvider.UnsyncedData;
            }
            set
            {
                NotificationProvider.UnsyncedData = value;
                OnPropertyChanged("NotificationCollection");
            }
        }

        public ObservableCollection<MenuItem> Menu => AppMenu;
        public ObservableCollection<MenuItem> OptionsMenu => AppOptionsMenu;
    }
}