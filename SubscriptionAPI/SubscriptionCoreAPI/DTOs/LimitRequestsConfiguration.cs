namespace SubscriptionCoreAPI.DTOs
{
    public class LimitRequestsConfiguration
    {
        public int RequestsPerDayFree { get; set; }
        public string[] WhiteListRoutes { get; set; }
    }
}
