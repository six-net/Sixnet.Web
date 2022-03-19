using System.IO;
using Microsoft.AspNetCore.Http.Extensions;

namespace EZNEW.Web.Utility
{
    /// <summary>
    /// Http client helper
    /// </summary>
    public static class HttpClientHelper
    {
        /// <summary>
        /// Https protocol name
        /// </summary>
        private const string HttpsProtocolName = "https";

        /// <summary>
        /// Http protocol name
        /// </summary>
        private const string HttpProtocolName = "http";

        /// <summary>
        /// Gets the host
        /// </summary>
        public static string Host
        {
            get
            {
                return HttpContextHelper.Current.Request.Host.Host;
            }
        }

        /// <summary>
        /// Gets the host and port
        /// </summary>
        public static string FullHost
        {
            get
            {
                var host = Host;
                var port = Port;
                if (port != 80)
                {
                    host = string.Format("{0}:{1}", host, port);
                }
                return host;
            }
        }

        /// <summary>
        /// Gets the port
        /// </summary>
        public static int Port
        {
            get
            {
                return HttpContextHelper.Current.Request.Host.Port ?? 80;
            }
        }

        /// <summary>
        /// Gets the remote ip address
        /// </summary>
        public static string RemoteIp
        {
            get
            {
                return HttpContextHelper.Current.Connection.RemoteIpAddress.ToString();
            }
        }

        /// <summary>
        /// Gets the original url
        /// </summary>
        public static string RawUrl
        {
            get
            {
                return HttpContextHelper.Current.Request.GetEncodedUrl();
            }
        }

        /// <summary>
        /// Gets the request url
        /// </summary>
        public static string Url
        {
            get
            {
                return HttpContextHelper.Current.Request.GetDisplayUrl();
            }
        }

        /// <summary>
        /// Gets the protocol
        /// </summary>
        public static string Protocol
        {
            get
            {
                var https = HttpContextHelper.Current.Request.IsHttps;
                return https ? HttpsProtocolName : HttpProtocolName;
            }
        }

        /// <summary>
        /// Gets the full path
        /// </summary>
        /// <param name="relativePath">Relative path</param>
        /// <returns>Return the full path</returns>
        public static string GetFullPath(string relativePath)
        {
            var path = string.Format("{0}://{1}", Protocol, FullHost);
            if (string.IsNullOrWhiteSpace(relativePath))
            {
                return path;
            }
            return Path.Combine(path.Trim('/', '\\'), relativePath.Trim('\\', '/'));
        }
    }
}
