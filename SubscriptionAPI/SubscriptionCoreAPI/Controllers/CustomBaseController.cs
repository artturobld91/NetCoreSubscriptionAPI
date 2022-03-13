using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace SubscriptionCoreAPI.Controllers
{
    public class CustomBaseController: ControllerBase
    {
        protected string GetUserId()
        {
            var userClaim = HttpContext.User.Claims.Where(x => x.Type == "id").FirstOrDefault();
            var userId = userClaim.Value;
            return userId;
        }
    }
}
