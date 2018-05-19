using MovieBase.Helper;
using MovieBase.Models;
using System.Collections.ObjectModel;

namespace MovieBase.ViewModels
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