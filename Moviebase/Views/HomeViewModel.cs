using Moviebase.DAL.Entities;
using Moviebase.Helper;
using System.Collections.ObjectModel;

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
    }
}