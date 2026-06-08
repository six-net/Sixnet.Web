using System.Linq;
using System.Threading.Tasks;
using Sixnet.Session;
using Microsoft.AspNetCore.Http;

namespace Sixnet.Web.Middleware
{
    /// <summary>
    /// Session middleware
    /// </summary>
    public class SixnetClaimSessionMiddleware
    {
        private readonly RequestDelegate _next;

        public SixnetClaimSessionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var claims = context.User?.Claims;
            using (var session = SixnetSessionContext.Create(session =>
            {
                session.Isolation = SixnetWeb.Options?.GetIsolationInfo(context);
                session.User = SixnetUserInfo.GetUserFromClaims(claims);
            }))
            {
                await _next(context).ConfigureAwait(false);
            }
        }
    }
}
