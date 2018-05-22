using MahApps.Metro.IconPacks;
using Moviebase.Helper;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Moviebase.DAL.Entities;

namespace Moviebase.ViewModels
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
        private ObservableCollection<Notification> AppNotification = new ObservableCollection<Notification>();
        private static readonly ObservableCollection<Movie> UnsyncedMovies = new ObservableCollection<Movie>();
        private readonly ObservableCollection<Movie> Folders = new ObservableCollection<Movie>();

        public string NotificationCount
        {
            get => (NotificationProvider.Count() == 0 ? "" : NotificationProvider.Count().ToString());
        }

        public ObservableCollection<Notification> NotificationCollection
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

        public ObservableCollection<Movie> UnsyncedCollection
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