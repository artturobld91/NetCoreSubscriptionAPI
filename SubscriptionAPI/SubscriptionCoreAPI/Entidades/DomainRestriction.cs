namespace SubscriptionCoreAPI.Entidades
{
    public class DomainRestriction
    {
        public int Id { get; set; }
        public int KeyId { get; set; }
        public string Domain { get; set; }
        public APIKey key { get; set; }

    }
}
