using System;

namespace Moviebase.DAL.Entities
{
    public class Subscription
    {
        public int SubscriptionId { get; set; }

        public Movie Movie { get; set; }
        public DateTime LastUpdate { get; set; }
        public DateTime LastChecked { get; set; }
    }
}
