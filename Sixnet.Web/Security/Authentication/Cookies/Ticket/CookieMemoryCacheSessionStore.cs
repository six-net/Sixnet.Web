using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Sixnet.Web.Security.Authentication.Session;

namespace Sixnet.Web.Security.Authentication.Cookies.Ticket
{
    /// <summary>
    /// Cookie memory cache session store
    /// </summary>
    public static class CookieMemoryCacheSessionStore
    {
        private static readonly IMemoryCache _cache;

        static CookieMemoryCacheSessionStore()
        {
            _cache = new MemoryCache(new MemoryCacheOptions());
        }

        #region Store session

        /// <summary>
        /// Store session
        /// </summary>
        /// <param name="authSession">Auth session</param>
        /// <returns></returns>
        public static async Task StoreSessionAsync(AuthSession authSession)
        {
            if (authSession == null)
            {
                throw new ArgumentNullException(nameof(authSession));
            }
            string subjectId = authSession.GetSubjectId();
            if (string.IsNullOrWhiteSpace(subjectId))
            {
                throw new Exception("authentication subject is null or empty");
            }
            string sessionId = authSession.SessionId;
            if (string.IsNullOrWhiteSpace(sessionId))
            {
                throw new Exception("session id is null or empty");
            }
            var expiresSeconds = (authSession.Expires - DateTimeOffset.Now).TotalSeconds;
            var options = new MemoryCacheEntryOptions();
            options.SetSlidingExpiration(TimeSpan.FromSeconds(expiresSeconds));
            _cache.Set(sessionId, subjectId, options);
            _cache.Set(subjectId, authSession, options);
            await Task.CompletedTask.ConfigureAwait(false);
        }

        #endregion

        #region Delete session

        /// <summary>
        /// Delete session
        /// </summary>
        /// <param name="sessionId">Session id</param>
        /// <returns></returns>
        public static async Task DeleteSessionAsync(string sessionId)
        {
            if (string.IsNullOrWhiteSpace(sessionId))
            {
                await Task.CompletedTask.ConfigureAwait(false);
            }
            var session = await GetSessionAsync(sessionId).ConfigureAwait(false);
            _cache.Remove(sessionId);
            if (session != null)
            {
                _cache.Remove(session.GetSubjectId());
            }
            await Task.CompletedTask.ConfigureAwait(false);
        }

        #endregion

        #region Get session

        /// <summary>
        /// Get session
        /// </summary>
        /// <param name="sessionId">Session id</param>
        /// <returns>Return auth session object</returns>
        public static async Task<AuthSession> GetSessionAsync(string sessionId)
        {
            if (string.IsNullOrWhiteSpace(sessionId))
            {
                return null;
            }
            var subject = _cache.Get<string>(sessionId);
            if (string.IsNullOrWhiteSpace(subject))
            {
                return null;
            }
            var session = await GetSessionBySubjectAsync(subject).ConfigureAwait(false);
            if (!(session?.AllowUse(sessionId) ?? false))
            {
                _cache.Remove(sessionId);
                session = null;
            }
            return session;
        }

        /// <summary>
        /// Get session by login subject
        /// </summary>
        /// <param name="subject">Login subject</param>
        /// <returns>Return auth session object</returns>
        public static async Task<AuthSession> GetSessionBySubjectAsync(string subject)
        {
            if (subject == null)
            {
                return null;
            }
            _cache.TryGetValue(subject, out AuthSession session);
            if (!(session?.AllowUse() ?? false))
            {
                session = null;
            }
            return await Task.FromResult(session).ConfigureAwait(false);
        }

        #endregion

        #region Verify Session

        /// <summary>
        /// Verify session
        /// </summary>
        /// <param name="subject">Subject</param>
        /// <param name="sessionToken">Session token</param>
        /// <param name="refresh">Whether refresh session</param>
        /// <returns>Return whether verify is pass</returns>
        public static async Task<bool> VerifySessionAsync(string subject, string sessionToken, bool refresh = true)
        {
            if (string.IsNullOrWhiteSpace(sessionToken) || string.IsNullOrWhiteSpace(subject))
            {
                return false;
            }
            var session = await GetSessionBySubjectAsync(subject).ConfigureAwait(false);
            var verifySuccess = session?.AllowUse(sessionToken: sessionToken) ?? false;
            if (verifySuccess && refresh)
            {
                await StoreSessionAsync(session).ConfigureAwait(false);
            }
            return verifySuccess;
        }

        /// <summary>
        /// Verify session
        /// </summary>
        /// <param name="claims">Claims</param>
        /// <param name="refresh">Whether refresh session</param>
        /// <returns>Return whether verify is pass</returns>
        public static async Task<bool> VerifySessionAsync(Dictionary<string, string> claims, bool refresh = true)
        {
            if (claims == null || claims.Count <= 0)
            {
                return false;
            }
            var sessionToken = AuthSession.GetSessionToken(claims);
            var subject = AuthSession.GetSubject(claims);
            return await VerifySessionAsync(subject, sessionToken, refresh).ConfigureAwait(false);
        }

        #endregion
    }
}
