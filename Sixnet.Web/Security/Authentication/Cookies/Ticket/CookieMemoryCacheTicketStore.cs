using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Sixnet.Web.Security.Authentication.Session;

namespace Sixnet.Web.Security.Authentication.Cookies.Ticket
{
    /// <summary>
    /// Cookie memory cache ticket store
    /// </summary>
    public class CookieMemoryCacheTicketStore : IAuthenticationTicketStore
    {
        public CookieMemoryCacheTicketStore()
        {
        }

        /// <summary>
        /// Get session token
        /// </summary>
        /// <param name="subject">Subject</param>
        /// <returns>Return session token</returns>
        public async Task<string> GetSessionTokenAsync(string subject)
        {
            var session = await CookieMemoryCacheSessionStore.GetSessionBySubjectAsync(subject).ConfigureAwait(false);
            return session?.SessionToken ?? string.Empty;
        }

        /// <summary>
        /// Remove ticket
        /// </summary>
        /// <param name="key">Ticket key</param>
        /// <returns></returns>
        public async Task RemoveAsync(string key)
        {
            await CookieMemoryCacheSessionStore.DeleteSessionAsync(key).ConfigureAwait(false);
        }

        /// <summary>
        /// Renew ticket
        /// </summary>
        /// <param name="key">Session key</param>
        /// <param name="ticket">Ticket</param>
        /// <returns></returns>
        public async Task RenewAsync(string key, AuthenticationTicket ticket)
        {
            var session = AuthSession.FromAuthenticationTicket(ticket);
            if (session == null)
            {
                await Task.CompletedTask.ConfigureAwait(false);
            }
            session.SessionId = key;
            await CookieMemoryCacheSessionStore.StoreSessionAsync(session).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieve ticket
        /// </summary>
        /// <param name="key">Session key</param>
        /// <returns>Return authentication ticket</returns>
        public async Task<AuthenticationTicket> RetrieveAsync(string key)
        {
            var session = await CookieMemoryCacheSessionStore.GetSessionAsync(key).ConfigureAwait(false);
            if (session == null)
            {
                return null;
            }
            return session.ConvertToTicket();
        }

        /// <summary>
        /// Store ticket
        /// </summary>
        /// <param name="ticket">Ticket</param>
        /// <returns>Return ticket key</returns>
        public async Task<string> StoreAsync(AuthenticationTicket ticket)
        {
            var key = Guid.NewGuid().ToString("N");
            await RenewAsync(key, ticket).ConfigureAwait(false);
            return key;
        }

        /// <summary>
        /// Verify ticket
        /// </summary>
        /// <param name="subject">Subject</param>
        /// <param name="sessionToken">Session token</param>
        /// <param name="renew">Whether renew ticket</param>
        /// <returns>Return whether verify successful</returns>
        public async Task<bool> VerifyTicketAsync(string subject, string sessionToken, bool renew = true)
        {
            return await CookieMemoryCacheSessionStore.VerifySessionAsync(subject, sessionToken, renew).ConfigureAwait(false);
        }
    }
}
