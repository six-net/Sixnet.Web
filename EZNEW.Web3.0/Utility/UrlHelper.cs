using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Http;

namespace EZNEW.Web.Utility
{
    /// <summary>
    /// Url helper
    /// </summary>
    public static class UrlHelper
    {
        #region Encode/Decode

        /// <summary>
        /// Url encode
        /// </summary>
        /// <param name="url">Url</param>
        /// <returns>Return the encoded url</returns>
        public static string UrlEncode(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return string.Empty;
            }
            return HttpUtility.UrlEncode(url);
        }

        /// <summary>
        /// Url decode
        /// </summary>
        /// <param name="url">Url</param>
        /// <returns>Return the decoded url</returns>
        public static string UrlDecode(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return string.Empty;
            }
            return HttpUtility.UrlDecode(url);
        }

        #endregion

        #region Parameter

        /// <summary>
        /// Get url without parameter
        /// </summary>
        /// <param name="url">Url</param>
        /// <returns>Return the new url</returns>
        public static string GetUrlWithOutParameter(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return url;
            }
            string[] urlArray = url.LSplit("?");
            if (urlArray.Length < 1)
            {
                return string.Empty;
            }
            return urlArray[0];
        }

        /// <summary>
        /// Remove parameters
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="removeParameterNames">Parameter names</param>
        /// <returns>Return the new url</returns>
        public static string RemoveUrlParameters(string url, IDictionary<string, string> parameters, IEnumerable<string> removeParameterNames)
        {
            if (removeParameterNames.IsNullOrEmpty() || string.IsNullOrWhiteSpace(url) || parameters.IsNullOrEmpty())
            {
                return url;
            }
            List<string> removeNames = removeParameterNames.Select(c => c.ToLower()).ToList();
            url = GetUrlWithOutParameter(url).Trim('/', '?', '&');
            List<string> parameterValues = new List<string>(parameters.Count);
            foreach (var parameterItem in parameters)
            {
                string parameterName = parameterItem.Key.ToLower();
                if (removeNames.Contains(parameterName))
                {
                    continue;
                }
                parameterValues.Add(string.Format("{0}={1}", parameterName, UrlEncode(parameterItem.Value)));
            }
            if (parameterValues.IsNullOrEmpty())
            {
                return url;
            }
            return string.Format("{0}?{1}", url, string.Join("&", parameterValues));
        }

        /// <summary>
        /// Remove url parameters
        /// </summary>
        /// <param name="request">Request</param>
        /// <param name="removeParameterNames">Remove parameter names</param>
        /// <returns>Return the new url</returns>
        public static string RemoveUrlParameters(HttpRequest request, IEnumerable<string> removeParameterNames)
        {
            if (request == null)
            {
                return string.Empty;
            }
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            foreach (string parameterKey in removeParameterNames)
            {
                if (string.IsNullOrWhiteSpace(parameterKey))
                {
                    continue;
                }
                string parameterValue = request.Query[parameterKey];
                if (string.IsNullOrWhiteSpace(parameterValue))
                {
                    continue;
                }
                parameters.Add(parameterKey, parameterValue);
            }
            return RemoveUrlParameters(request.Path, parameters, removeParameterNames);
        }

        /// <summary>
        /// Append parameters
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>Return new url</returns>
        public static string AppendParameters(string url, IDictionary<string, string> parameters)
        {
            if (string.IsNullOrWhiteSpace(url) || parameters.IsNullOrEmpty())
            {
                return string.Empty;
            }
            Dictionary<string, string> nowParameters = new Dictionary<string, string>();
            string[] urlArray = url.LSplit("?");
            if (urlArray.Length > 1)
            {
                string urlParameterString = urlArray[1];
                var urlParameterValues = HttpUtility.ParseQueryString(urlParameterString);
                string[] parameterKeys = urlParameterValues.AllKeys;
                foreach (string key in parameterKeys)
                {
                    nowParameters.Add(key.ToLower(), urlParameterValues[key]);
                }
            }
            foreach (var newParameter in parameters)
            {
                string keyName = newParameter.Key.ToLower();
                if (nowParameters.ContainsKey(keyName))
                {
                    nowParameters[keyName] = newParameter.Value;
                }
                else
                {
                    nowParameters.Add(keyName, newParameter.Value);
                }
            }
            url = GetUrlWithOutParameter(url);
            if (nowParameters == null || nowParameters.Count <= 0)
            {
                return url;
            }
            List<string> parameterValueString = new List<string>(nowParameters.Count);
            foreach (var parameter in nowParameters)
            {
                parameterValueString.Add(string.Format("{0}={1}", parameter.Key, UrlEncode(parameter.Value)));
            }
            return string.Format("{0}?{1}", url, string.Join("&", parameterValueString));
        }

        #endregion
    }
}
