using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sixnet.Cache;
using Sixnet.Cache.Keys.Options;
using Sixnet.Cache.String;
using Sixnet.Serialization;
using Sixnet.Web.Security.Authentication.Session;

namespace Sixnet.Web.SessionCacheStore
{
    /// <summary>
    /// Session存储配置
    /// </summary>
    public static class SessionCacheStore
    {
        /// <summary>
        /// 获取Cache Object Name
        /// </summary>
        public const string CacheObjectName = "session_cache_store";

        #region 存储Session

        /// <summary>
        /// 存储Session对象
        /// </summary>
        /// <param name="sessionObject">session对象</param>
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
            await CacheManager.String.SetAsync(new StringSetOptions()
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
                        Value=JsonSerializer.Serialize(sessionObject),
                        Expiration=expiration,
                        When=CacheSetWhen.Always
                    }
                }
            }).ConfigureAwait(false);
        }

        #endregion

        #region 删除Session

        /// <summary>
        /// 删除Session
        /// </summary>
        /// <param name="sessionKey">session键值</param>
        /// <returns></returns>
        public static async Task DeleteSessionAsync(string sessionKey)
        {
            if (string.IsNullOrWhiteSpace(sessionKey))
            {
                await Task.CompletedTask.ConfigureAwait(false);
            }
            string subject = await CacheManager.String.GetAsync(sessionKey, GetCacheObject()).ConfigureAwait(false);
            if (string.IsNullOrWhiteSpace(subject))
            {
                return;
            }
            await CacheManager.Keys.DeleteAsync(new DeleteOptions()
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

        #region 获取Session

        /// <summary>
        /// 获取Session
        /// </summary>
        /// <param name="sessionId">session key</param>
        /// <returns></returns>
        public static async Task<AuthSession> GetSessionAsync(string sessionId)
        {
            if (string.IsNullOrWhiteSpace(sessionId))
            {
                return null;
            }
            string subject = await CacheManager.String.GetAsync(sessionId, GetCacheObject()).ConfigureAwait(false);
            var session = await GetSessionBySubjectAsync(subject).ConfigureAwait(false);
            if (!(session?.AllowUse(sessionId: sessionId) ?? false))
            {
                await CacheManager.Keys.DeleteAsync(new DeleteOptions()
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
        /// 根据登陆账号身份编号获取session
        /// </summary>
        /// <param name="subject">身份编号</param>
        /// <returns></returns>
        public static async Task<AuthSession> GetSessionBySubjectAsync(string subject)
        {
            if (subject == null)
            {
                return null;
            }
            string sessionValue = await CacheManager.String.GetAsync(subject, GetCacheObject()).ConfigureAwait(false);
            var session = JsonSerializer.Deserialize<AuthSession>(sessionValue);
            if (!(session?.AllowUse() ?? false))
            {
                session = null;
            }
            return session;
        }

        #endregion

        #region 验证Session

        /// <summary>
        /// 验证Session是否有效
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
        /// 验证Session凭据是否有效
        /// </summary>
        /// <param name="claims">凭据</param>
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

        #region 获取CacheObject

        /// <summary>
        /// 获取CacheObject
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
