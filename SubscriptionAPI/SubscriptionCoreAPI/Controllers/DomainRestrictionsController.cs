using Microsoft.AspNetCore.Mvc;

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
        public async Task<ActionResult> Post()
        { 
        
        }

    }
}
