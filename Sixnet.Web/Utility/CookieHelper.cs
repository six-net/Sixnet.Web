using System;
using Microsoft.AspNetCore.Http;
using Sixnet.Web.Security.Authentication.Cookies;

namespace Sixnet.Web.Utility
{
    /// <summary>
    /// Cookie Helper
    /// </summary>
    public static class CookieHelper
    {
        #region Save Cookie

        /// <summary>
        /// Save Cookie
        /// </summary>
        /// <param name="cookie">Cookie object</param>
        public static void SaveCookie(CookieEntry cookie)
        {
            if (cookie == null)
            {
                return;
            }
            cookie.Options = cookie.Options ?? new CookieOptions();
            cookie.Options.HttpOnly = true;

            HttpContextHelper.Current.Response.Cookies.Append(cookie.Key, cookie.Value, cookie.Options);
        }

        #endregion

        #region Get Cookie By Name

        /// <summary>
        /// Get cookie by name
        /// </summary>
        /// <param name="cookieName">Name</param>
        /// <returns>Cookie object</returns>
        public static CookieEntry GetCookie(string cookieName)
        {
            if (string.IsNullOrWhiteSpace(cookieName))
            {
                return null;
            }
            var cookieCollection = HttpContextHelper.Current.Request.Cookies;
            if (cookieCollection.Keys.Contains(cookieName))
            {
                return new CookieEntry()
                {
                    Key = cookieName,
                    Value = cookieCollection[cookieName]
                };
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region Get Value By Name

        /// <summary>
        /// Get value by name
        /// </summary>
        /// <param name="cookieName">Cookie name</param>
        /// <returns>Return cookie value</returns>
        public static string GetCookieValue(string cookieName)
        {
            if (string.IsNullOrWhiteSpace(cookieName))
            {
                return string.Empty;
            }
            var nowCookie = GetCookie(cookieName);
            if (nowCookie == null)
            {
                return string.Empty;
            }
            return nowCookie.Value;
        }

        #endregion

        #region Set Cookie Value

        /// <summary>
        /// Set cookie value
        /// </summary>
        /// <param name="cookieName">Cookie name</param>
        /// <param name="value">Value</param>
        /// <returns>Return whether set successful</returns>
        public static bool SetCookieValue(string cookieName, string value, DateTimeOffset? expiresTime = null)
        {
            if (string.IsNullOrWhiteSpace(cookieName))
            {
                return false;
            }
            var nowCookie = GetCookie(cookieName);
            if (nowCookie == null)
            {
                nowCookie = new CookieEntry()
                {
                    Key = cookieName
                };
            }
            if (!expiresTime.HasValue)
            {
                expiresTime = DateTimeOffset.Now.AddHours(2);
            }
            var options = nowCookie.Options ?? new CookieOptions();
            nowCookie.Value = value;
            options.Expires = expiresTime.Value;
            nowCookie.Options = options;
            SaveCookie(nowCookie);
            return true;
        }

        #endregion

        #region Remove Cookie By Name

        /// <summary>
        /// Remove cookie by name
        /// </summary>
        /// <param name="cookieName">Cookie name</param>
        /// <returns>Return whether remove successful</returns>
        public static bool RemoveCookie(string cookieName)
        {
            if (string.IsNullOrWhiteSpace(cookieName))
            {
                return false;
            }
            HttpContextHelper.Current.Response.Cookies.Delete(cookieName);
            return true;
        }

        #endregion
    }
}
