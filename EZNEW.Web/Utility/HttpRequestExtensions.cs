using System;
using System.Collections.Generic;
using Microsoft.Extensions.Primitives;

namespace Microsoft.AspNetCore.Http
{
    /// <summary>
    /// Http request extensions
    /// </summary>
    public static class HttpRequestExtensions
    {
        #region Determine wheather http request sent by ajax

        /// <summary>
        /// Determine wheather http request sent by ajax
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Return wheather http request sent by ajax</returns>
        public static bool IsAjaxRequest(this HttpRequest request)
        {
            return string.Equals(request.Query["X-Requested-With"], "XMLHttpRequest", StringComparison.Ordinal) ||
                string.Equals(request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.Ordinal);
        }

        #endregion

        #region Get parameter value

        /// <summary>
        /// Get parameter value
        /// </summary>
        /// <param name="request">Request</param>
        /// <param name="key">Parameter key</param>
        /// <returns>Return the parameter value</returns>
        public static string GetValue(this HttpRequest request, string key)
        {
            if (string.IsNullOrWhiteSpace(key) || request == null)
            {
                return string.Empty;
            }
            if (request.Query.ContainsKey(key))
            {
                return request.Query[key];
            }
            if (request.Form.ContainsKey(key))
            {
                return request.Form[key];
            }
            return string.Empty;
        }

        #endregion

        #region Get all parameters

        /// <summary>
        /// Get all request parameters
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Return all parameters</returns>
        public static Dictionary<string, string> GetAllParameters(this HttpRequest request)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            IEnumerable<KeyValuePair<string, StringValues>> collection;
            if (string.Equals(request.Method, HttpMethods.Post, StringComparison.OrdinalIgnoreCase))
            {
                collection = request.Form;
            }
            else
            {
                collection = request.Query;
            }
            foreach (var item in collection)
            {
                if (parameters.ContainsKey(item.Key))
                {
                    parameters[item.Key] = item.Value;
                }
                else
                {
                    parameters.Add(item.Key, item.Value);
                }
            }
            return parameters;
        }

        #endregion

        #region Get all parameters with sorted

        /// <summary>
        /// Get all parameters with sorted
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Return all parameters</returns>
        public static SortedDictionary<string, string> GetAllSortedParameters(HttpRequest request)
        {
            return new SortedDictionary<string, string>(GetAllParameters(request));
        }

        #endregion
    }
}
