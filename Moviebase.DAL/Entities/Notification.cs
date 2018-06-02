namespace Moviebase.DAL.Entities
{
    public class Notification
    {
        public int NotificationId { get; set; }

        public int Priority { get; set; }
        public string Message { get; set; }
    }
}