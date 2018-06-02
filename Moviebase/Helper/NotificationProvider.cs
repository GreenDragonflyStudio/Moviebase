using Moviebase.DAL.Entities;
using System.Collections.ObjectModel;

namespace Moviebase.Helper
{
    public static class NotificationProvider
    {
        public static ObservableCollection<Notification> Notifications = new ObservableCollection<Notification>();

        //public static ObservableCollection<MovieItem> UnsyncedData = new ObservableCollection<MovieItem>();

        internal static void Notify(string msg, int prior = 1)
        {
            Notifications.Add(new Notification() { Message = msg, Priority = prior });
        }

        internal static int Count()
        {
            return Notifications.Count;
        }

        internal static void Clear(int v)
        {
            var a = new Notification[Notifications.Count];
            Notifications.CopyTo(a, 0);
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i].Priority <= v)
                {
                    Notifications.Remove(a[i]);
                }
            }
        }
    }
}