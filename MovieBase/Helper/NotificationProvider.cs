using MovieBase.Models;
using System.Collections.ObjectModel;

namespace MovieBase.Helper
{
    public static class NotificationProvider
    {
        public static ObservableCollection<NotificationItem> Notifications = new ObservableCollection<NotificationItem>();

        public static ObservableCollection<MovieItem> UnsyncedData = new ObservableCollection<MovieItem>();

        internal static void Notify(string msg, int prior = 1)
        {
            Notifications.Add(new NotificationItem() { Message = msg, Priority = prior });
        }

        internal static int Count()
        {
            return Notifications.Count;
        }

        internal static void Clear(int v)
        {
            var a = new NotificationItem[Notifications.Count];
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