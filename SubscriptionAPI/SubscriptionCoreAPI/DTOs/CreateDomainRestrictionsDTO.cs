using System.ComponentModel.DataAnnotations;

namespace SubscriptionCoreAPI.DTOs
{
    public class CreateDomainRestrictionsDTO
    {
        public int KeyId { get; set; }
        [Required]
        public string Domain { get; set; }
    }
}
