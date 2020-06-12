using System.Text;
using System.Threading.Tasks;
using EZNEW.Serialize;

namespace Microsoft.AspNetCore.Http
{
    /// <summary>
    /// Http response extensions
    /// </summary>
    public static class HttpResponseExtensions
    {
        #region Write json

        /// <summary>
        /// Write json
        /// </summary>
        /// <param name="response">Response</param>
        /// <param name="data">Data</param>
        /// <returns></returns>
        public static async Task WriteJsonAsync(this HttpResponse response, object data)
        {
            var json = JsonSerializeHelper.ObjectToJson(data);
            await response.WriteJsonAsync(json).ConfigureAwait(false);
        }

        /// <summary>
        /// Write json
        /// </summary>
        /// <param name="response">Response</param>
        /// <param name="json">Json data</param>
        /// <returns></returns>
        public static async Task WriteJsonAsync(this HttpResponse response, string json)
        {
            response.ContentType = "application/json; charset=UTF-8";
            await response.WriteAsync(json).ConfigureAwait(false);
        }

        #endregion

        #region Set cache

        /// <summary>
        /// Set cache
        /// </summary>
        /// <param name="response">Response</param>
        /// <param name="maxAge">Cache max age</param>
        public static void SetCache(this HttpResponse response, int maxAge)
        {
            if (maxAge == 0)
            {
                response.SetNoCache();
            }
            else if (maxAge > 0)
            {
                if (!response.Headers.ContainsKey("Cache-Control"))
                {
                    response.Headers.Add("Cache-Control", $"max-age={maxAge}");
                }
            }
        }

        #endregion

        #region Set no cache

        /// <summary>
        /// Set no cache
        /// </summary>
        /// <param name="response">Http response</param>
        private static void SetNoCache(this HttpResponse response)
        {
            if (!response.Headers.ContainsKey("Cache-Control"))
            {
                response.Headers.Add("Cache-Control", "no-store, no-cache, max-age=0");
            }
            if (!response.Headers.ContainsKey("Pragma"))
            {
                response.Headers.Add("Pragma", "no-cache");
            }
        }

        #endregion

        #region Write html

        /// <summary>
        /// Write html
        /// </summary>
        /// <param name="response">Response</param>
        /// <param name="html">Html</param>
        /// <returns></returns>
        public static async Task WriteHtmlAsync(this HttpResponse response, string html)
        {
            response.ContentType = "text/html; charset=UTF-8";
            await response.WriteAsync(html, Encoding.UTF8).ConfigureAwait(false);
        }

        #endregion
    }
}
