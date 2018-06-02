using Moviebase.Helper;
using System.Collections.ObjectModel;
using Moviebase.DAL.Entities;

namespace Moviebase.ViewModels
{
    public class HomeViewModel : BindableBase
    {
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

        public int CollectionCount { get; set; }
        public int ObservedFolderCount { get; set; }
        public int UnsyncedCount { get; set; }
    }
}