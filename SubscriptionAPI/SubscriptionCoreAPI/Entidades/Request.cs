using System;

namespace SubscriptionCoreAPI.Entidades
{
    public class Request
    {
        public int Id { get; set; }
        public int KeyId { get; set; }
        public DateTime RequestDate { get; set; }
        public APIKey Key { get; set; }
    }
}
