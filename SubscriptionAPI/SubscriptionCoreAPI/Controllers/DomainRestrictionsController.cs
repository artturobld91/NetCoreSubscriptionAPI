using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Linq;
using SubscriptionCoreAPI.DTOs;

namespace SubscriptionCoreAPI.Controllers
{
    [ApiController]
    [Route("api/domainrestrictions")]
    public class DomainRestrictionsController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        public DomainRestrictionsController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpPost]
        public async Task<ActionResult> Post(CreateDomainRestrictionsDTO createDomainRestrictionDTO)
        {
            var keyDB = await context.APIKeys.FirstOrDefault(x => x.Id == createDomainRestrictionDTO.KeyId);

            if (keyDB == null)
                return NotFound();

            
        }

    }
}
