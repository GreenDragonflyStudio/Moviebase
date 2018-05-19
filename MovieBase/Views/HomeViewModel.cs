using Moviebase.Helper;
using Moviebase.Models;
using System.Collections.ObjectModel;

namespace Moviebase.ViewModels
{
    public class HomeViewModel : BindableBase
    {
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
    }
}