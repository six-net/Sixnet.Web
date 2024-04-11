using System.Linq;
using System.Threading.Tasks;
using Sixnet.Session;
using Microsoft.AspNetCore.Http;

namespace Sixnet.Web.Middleware
{
    /// <summary>
    /// Session middleware
    /// </summary>
    public class ClaimSessionMiddleware
    {
        private readonly RequestDelegate _next;

        public ClaimSessionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.User == null)
            {
                await _next(context);
            }
            else
            {
                var claims = context.User.Claims;
                using (var session = SessionContext.Create(session =>
                {
                    session.Isolation = SixnetWeb.Options?.GetIsolationInfo?.Invoke(context);
                    session.User = UserInfo.GetUserFromClaims(claims);
                }))
                {
                    await _next(context);
                }
            }
        }
    }
}
