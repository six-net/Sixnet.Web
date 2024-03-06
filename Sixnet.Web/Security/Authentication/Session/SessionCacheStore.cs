using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sixnet.Cache;
using Sixnet.Cache.Keys.Parameters;
using Sixnet.Cache.String.Parameters;
using Sixnet.Serialization;

namespace Sixnet.Web.Security.Authentication.Session
{
    /// <summary>
    /// Session cache store
    /// </summary>
    public static class SessionCacheStore
    {
        /// <summary>
        /// 获取Cache Object Name
        /// </summary>
        public const string CacheObjectName = "SIXNET_SESSION_CACHE_STORE";

        #region Store Session

        /// <summary>
        /// Store Session
        /// </summary>
        /// <param name="sessionObject">session</param>
        /// <returns></returns>
        public static async Task StoreSessionAsync(AuthSession sessionObject)
        {
            if (sessionObject == null)
            {
                throw new ArgumentNullException(nameof(sessionObject));
            }
            string subjectId = sessionObject.GetSubjectId();
            if (string.IsNullOrWhiteSpace(subjectId))
            {
                throw new Exception("authentication subject is null or empty");
            }
            string sessionId = sessionObject.SessionId;
            if (string.IsNullOrWhiteSpace(sessionId))
            {
                throw new Exception("session key is null or empty");
            }
            var sessionConfig = SessionConfiguration.GetSessionConfiguration();
            var nowDate = DateTimeOffset.Now;
            var expiresDate = nowDate.Add(sessionConfig.Expires);
            sessionObject.Expires = expiresDate;
            var expiresSeconds = Convert.ToInt64((expiresDate - nowDate).TotalSeconds);
            var expiration = new CacheExpiration()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(expiresSeconds),
                SlidingExpiration = true
            };
            await SixnetCacher.String.SetAsync(new StringSetParameter()
            {
                CacheObject = GetCacheObject(),
                Items = new List<CacheEntry>()
                {
                    new CacheEntry()
                    {
                        Key=sessionId,
                        Value=subjectId,
                        When=CacheSetWhen.Always,
                        Expiration=expiration
                    },
                    new CacheEntry()
                    {
                        Key=subjectId,
                        Value=SixnetJsonSerializer.Serialize(sessionObject),
                        Expiration=expiration,
                        When=CacheSetWhen.Always
                    }
                }
            }).ConfigureAwait(false);
        }

        #endregion

        #region Delete Session

        /// <summary>
        /// Delete Session
        /// </summary>
        /// <param name="sessionKey">Session key</param>
        /// <returns></returns>
        public static async Task DeleteSessionAsync(string sessionKey)
        {
            if (string.IsNullOrWhiteSpace(sessionKey))
            {
                await Task.CompletedTask.ConfigureAwait(false);
            }
            string subject = await SixnetCacher.String.GetAsync(sessionKey, GetCacheObject()).ConfigureAwait(false);
            if (string.IsNullOrWhiteSpace(subject))
            {
                return;
            }
            await SixnetCacher.Keys.DeleteAsync(new DeleteParameter()
            {
                CacheObject = GetCacheObject(),
                Keys = new List<CacheKey>()
                {
                    ConstantCacheKey.Create(sessionKey),
                    ConstantCacheKey.Create(subject)
                }
            }).ConfigureAwait(false);
        }

        #endregion

        #region Get session

        /// <summary>
        /// Get session
        /// </summary>
        /// <param name="sessionId">Session key</param>
        /// <returns></returns>
        public static async Task<AuthSession> GetSessionAsync(string sessionId)
        {
            if (string.IsNullOrWhiteSpace(sessionId))
            {
                return null;
            }
            string subject = await SixnetCacher.String.GetAsync(sessionId, GetCacheObject()).ConfigureAwait(false);
            var session = await GetSessionBySubjectAsync(subject).ConfigureAwait(false);
            if (!(session?.AllowUse(sessionId: sessionId) ?? false))
            {
                await SixnetCacher.Keys.DeleteAsync(new DeleteParameter()
                {
                    CacheObject = GetCacheObject(),
                    Keys = new List<CacheKey>()
                    {
                        ConstantCacheKey.Create(sessionId)
                    }
                }).ConfigureAwait(false);
                return null;
            }
            return session;
        }

        /// <summary>
        /// Get session by subject
        /// </summary>
        /// <param name="subject">Subject</param>
        /// <returns></returns>
        public static async Task<AuthSession> GetSessionBySubjectAsync(string subject)
        {
            if (subject == null)
            {
                return null;
            }
            string sessionValue = await SixnetCacher.String.GetAsync(subject, GetCacheObject()).ConfigureAwait(false);
            var session = SixnetJsonSerializer.Deserialize<AuthSession>(sessionValue);
            if (!(session?.AllowUse() ?? false))
            {
                session = null;
            }
            return session;
        }

        #endregion

        #region Verify session

        /// <summary>
        /// Verify session
        /// </summary>
        /// <param name="sessionToken">session key</param>
        /// <param name="refresh">refresh session</param>
        /// <returns></returns>
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
        /// <returns></returns>
        public static async Task<bool> VerifySessionAsync(Dictionary<string, string> claims, bool refresh = true)
        {
            if (claims == null || claims.Count <= 0)
            {
                return false;
            }
            var subject = AuthSession.GetSubject(claims);
            var sessionToken = AuthSession.GetSessionToken(claims);
            return await VerifySessionAsync(subject, sessionToken, refresh).ConfigureAwait(false);
        }

        #endregion

        #region Get cache object

        /// <summary>
        /// Get cache object
        /// </summary>
        /// <returns></returns>
        static CacheObject GetCacheObject()
        {
            return new CacheObject()
            {
                ObjectName = CacheObjectName
            };
        }

        #endregion
    }
}
