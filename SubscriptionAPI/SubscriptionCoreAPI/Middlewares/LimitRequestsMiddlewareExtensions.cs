using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SubscriptionCoreAPI.DTOs;
using SubscriptionCoreAPI.Entidades;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SubscriptionCoreAPI.Middlewares
{
    public static class LimitRequestsMiddlewareExtensions
    {
        public static IApplicationBuilder UseLimitRequests(this IApplicationBuilder app)
        {
            return app.UseMiddleware<LimitRequestsMiddleware>();
        }
    }

    public class LimitRequestsMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IConfiguration configuration;

        public LimitRequestsMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            this.next = next;
            this.configuration = configuration;
        }
        public async Task InvokeAsync(HttpContext httpContext, ApplicationDbContext context)
        {
            var limitRequestsConfiguration = new LimitRequestsConfiguration();
            configuration.GetRequiredSection("RequestsLimit").Bind(limitRequestsConfiguration);

            var route = httpContext.Request.Path.ToString();
            var isRouteInWhiteList = limitRequestsConfiguration.WhiteListRoutes.Any(x => route.Contains(x));

            if (isRouteInWhiteList)
            { 
                await next(httpContext);
                return;
            }

            var keyStringValues = httpContext.Request.Headers["X-Api-Key"];

            if (keyStringValues.Count == 0)
            {
                httpContext.Response.StatusCode = 400;
                await httpContext.Response.WriteAsync("You must provide the key in the header X-Api-Key.");
                return;
            }

            if (keyStringValues.Count > 1)
            {
                httpContext.Response.StatusCode = 400;
                await httpContext.Response.WriteAsync("Just a single key must be present.");
                return;
            }

            var key = keyStringValues[0];

            var keyDB = await context.APIKeys.FirstOrDefaultAsync(x => x.Key == key);

            if (keyDB == null)
            {
                httpContext.Response.StatusCode = 400;
                await httpContext.Response.WriteAsync("Key does not exists.");
                return;
            }

            if (!keyDB.Active)
            {
                httpContext.Response.StatusCode = 400;
                await httpContext.Response.WriteAsync("Key is inactive.");
                return;
            }

            if (keyDB.KeyType == Entidades.KeyType.Free)
            {
                var today = DateTime.Today;
                var tomorrow = today.AddDays(1);
                var requestsPerformedTodayQuantity = await context.Requests.CountAsync(x => x.KeyId == keyDB.Id && x.RequestDate >= today && x.RequestDate < tomorrow);
                if (requestsPerformedTodayQuantity > limitRequestsConfiguration.RequestsPerDayFree)
                {
                    httpContext.Response.StatusCode = 429; //Too many requests
                    await httpContext.Response.WriteAsync("You have reached the maximun requests per day.");
                    return;
                }
            }

            var restrictionsExceeded = RequestExccedsAnyRestriction(keyDB, httpContext);

            if (!restrictionsExceeded)
            {
                httpContext.Response.StatusCode = 403;
                return;
            }

            var request = new Request() { KeyId = keyDB.Id, RequestDate = DateTime.UtcNow };
            context.Add(request);
            await context.SaveChangesAsync();

            await next(httpContext);

        }

        private bool RequestExccedsAnyRestriction(APIKey apikey, HttpContext httpContext)   
        {
            var restrictionsExists = apikey.DomainRestrictions.Any(); || apikey.IPRestrictions.Any();

            if (!restrictionsExists)
            {
                return true;
            }

            var requestExceedsDomainRestrictions = RequestExccedsDomainRestrictions(apikey.DomainRestrictions, httpContext);

            var requestExceedsIPRestrictions = RequestExccedsIPRestrictions(apikey.IPRestrictions, httpContext);

            return requestExceedsDomainRestrictions || requestExceedsIPRestrictions;
        }

        private bool RequestExccedsIPRestrictions(List<IPRestriction> restrinctions, HttpContext httpContext)
        {
            if (restrinctions == null || restrinctions.Count == 0)
                return false;

            var ip = httpContext.Connection.RemoteIpAddress.ToString();

            if (ip == String.Empty)
            {
                return false;
            }

            var exceedsRestriction = restrinctions.Any(x => x.IP == ip);
            return exceedsRestriction;
        }

        private bool RequestExccedsDomainRestrictions(List<DomainRestriction> restrinctions, HttpContext httpContext)
        {
            if (restrinctions == null || restrinctions.Count == 0)
                return false;

            var referer = httpContext.Request.Headers["Referer"].ToString();

            if (referer == String.Empty)
            {
                return false;
            }

            Uri myUri = new Uri(referer);
            string host = myUri.Host;

            var restrictionExceeded = restrinctions.Any(x => x.Domain == host);
            return restrictionExceeded;
        }
    }
}
