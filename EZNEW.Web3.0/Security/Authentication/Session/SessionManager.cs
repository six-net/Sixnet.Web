using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.Cookies;
using EZNEW.DependencyInjection;
using EZNEW.Web.Security.Authentication.Cookie.Ticket;

namespace EZNEW.Web.Security.Authentication.Session
{

    /// <summary>
    /// Session manager
    /// </summary>
    public static class SessionManager
    {
        /// <summary>
        /// Verify session
        /// </summary>
        /// <param name="subject">Subject</param>
        /// <param name="sessionToken">Session token</param>
        /// <returns>Return whether session verify successful</returns>
        public static async Task<bool> VerifySessionAsync(string subject, string sessionToken)
        {
            if (string.IsNullOrWhiteSpace(sessionToken))
            {
                return false;
            }
            CookieAuthenticationOptions cookieOptions = ContainerManager.Resolve<IOptionsMonitor<CookieAuthenticationOptions>>().Get(CookieAuthenticationDefaults.AuthenticationScheme);
            if (cookieOptions?.SessionStore != null)
            {
                var sessionStore = cookieOptions.SessionStore as IAuthenticationTicketStore;
                if (sessionStore != null)
                {
                    return await sessionStore.VerifyTicketAsync(subject, sessionToken).ConfigureAwait(false);
                }
            }
            return true;
        }

        /// <summary>
        /// Verify session
        /// </summary>
        /// <param name="claims">Clims</param>
        /// <returns>Return whether session verify successful</returns>
        public static async Task<bool> VerifySessionAsync(IEnumerable<Claim> claims)
        {
            if (claims == null || !claims.Any())
            {
                return false;
            }
            var sessionToken = AuthSession.GetSessionToken(claims);
            var subject = AuthSession.GetSubject(claims);
            return await VerifySessionAsync(subject, sessionToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Get session token
        /// </summary>
        /// <param name="subject">Subject</param>
        /// <returns>Return session token</returns>
        public static async Task<string> GetSessionToken(string subject)
        {
            if (string.IsNullOrWhiteSpace(subject))
            {
                return string.Empty;
            }
            CookieAuthenticationOptions cookieOptions = ContainerManager.Resolve<IOptionsMonitor<CookieAuthenticationOptions>>().Get(CookieAuthenticationDefaults.AuthenticationScheme);
            if (cookieOptions?.SessionStore != null)
            {
                var sessionStore = cookieOptions.SessionStore as IAuthenticationTicketStore;
                if (sessionStore != null)
                {
                    return await sessionStore.GetSessionTokenAsync(subject).ConfigureAwait(false);
                }
            }
            return string.Empty;
        }
    }
}
