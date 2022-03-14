namespace SubscriptionCoreAPI.Entidades
{
    public class IPRestriction
    {
        public int Id { get; set; }
        public int KeyId { get; set; }
        public string IP { get; set; }
        public APIKey Key { get; set; }
    }
}
