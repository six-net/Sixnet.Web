using System;
using System.Collections.Generic;
using Microsoft.Extensions.Primitives;
using System.Linq;

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
        public static Dictionary<string, StringValues> GetAllParameters(this HttpRequest request)
        {
            var parameters = new Dictionary<string, string>();
            IEnumerable<KeyValuePair<string, StringValues>> collection = string.Equals(request.Method, HttpMethods.Post, StringComparison.OrdinalIgnoreCase)
                ? request.Form
                : request.Query;
            return collection?.ToDictionary(item => item.Key, item => item.Value) ?? new Dictionary<string, StringValues>(0);
        }

        #endregion

        #region Get all sorted parameters 

        /// <summary>
        /// Get all parameters with sorted
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Return all parameters</returns>
        public static SortedDictionary<string, StringValues> GetAllSortedParameters(HttpRequest request)
        {
            return new SortedDictionary<string, StringValues>(request.GetAllParameters());
        }

        #endregion
    }
}
