using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using EZNEW.Web.Security.Authentication.Session;
using EZNEW.Web.Security.Authentication.Cookie.Ticket;

namespace EZNEW.Web.SessionCacheStore
{
    public class SessionCacheTicketStore : ITicketDistributedStore
    {
        public async Task<string> GetSessionTokenAsync(string subject)
        {
            var session = await SessionCacheStore.GetSessionBySubjectAsync(subject).ConfigureAwait(false);
            return session?.SessionToken ?? string.Empty;
        }

        public async Task RemoveAsync(string key)
        {
            await SessionCacheStore.DeleteSessionAsync(key).ConfigureAwait(false);
        }

        public async Task RenewAsync(string key, AuthenticationTicket ticket)
        {
            var session = AuthSession.FromAuthenticationTicket(ticket);
            if (session == null)
            {
                await Task.CompletedTask.ConfigureAwait(false);
            }
            session.SessionId = key;
            await SessionCacheStore.StoreSessionAsync(session).ConfigureAwait(false);
        }

        public async Task<AuthenticationTicket> RetrieveAsync(string key)
        {
            var session = await SessionCacheStore.GetSessionAsync(key).ConfigureAwait(false);
            if (session == null)
            {
                return null;
            }
            return session.ConvertToTicket();
        }

        public async Task<string> StoreAsync(AuthenticationTicket ticket)
        {
            var key = Guid.NewGuid().ToString("N");
            await RenewAsync(key, ticket).ConfigureAwait(false);
            return key;
        }

        public async Task<bool> VerifyTicketAsync(string subject, string sessionToken, bool renew = true)
        {
            return await SessionCacheStore.VerifySessionAsync(subject, sessionToken, renew).ConfigureAwait(false);
        }
    }
}
