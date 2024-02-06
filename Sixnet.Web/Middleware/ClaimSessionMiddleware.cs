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
                var isolationDataId = claims?.FirstOrDefault(c => c.Type == SessionContext.IsolationIdKey)?.Value;
                var isolationDataCode = claims?.FirstOrDefault(c => c.Type == SessionContext.IsolationCodeKey)?.Value;
                var isolationDataName = claims?.FirstOrDefault(c => c.Type == SessionContext.IsolationNameKey)?.Value;
                using (var session = SessionContext.Create(session =>
                {
                    session.IsolationData = new IsolationData()
                    {
                        Id = isolationDataId,
                        Code = isolationDataCode,
                        Name = isolationDataName
                    };
                    session.User = UserInfo.GetUserFromClaims(claims);
                }))
                {
                    await _next(context);
                }
            }
        }
    }
}
