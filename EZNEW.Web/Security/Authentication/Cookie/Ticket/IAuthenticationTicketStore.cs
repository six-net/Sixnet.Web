using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace EZNEW.Web.Security.Authentication.Cookie.Ticket
{
    /// <summary>
    /// Authentication ticket store
    /// </summary>
    public interface IAuthenticationTicketStore : ITicketStore
    {
        /// <summary>
        /// Verify ticket
        /// </summary>
        /// <param name="subject">Subject</param>
        /// <param name="sessionToken">Session token</param>
        /// <param name="renew">Whether renew ticket</param>
        /// <returns>Return whether verify successful</returns>
        Task<bool> VerifyTicketAsync(string subject, string sessionToken, bool renew = true);

        /// <summary>
        /// Get session token
        /// </summary>
        /// <param name="subject">Subject</param>
        /// <returns>Return session token</returns>
        Task<string> GetSessionTokenAsync(string subject);
    }
}
