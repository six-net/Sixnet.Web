using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace EZNEW.Framework.Net.Http
{
    /// <summary>
    /// http request message option
    /// </summary>
    public class HttpRequestOption
    {
        /// <summary>
        /// whether cookies are enabled
        /// </summary>
        public bool UseCookie
        {
            get;set;
        }

        /// <summary>
        /// http request message
        /// </summary>
        public HttpRequestMessage RequestMessage
        {
            get;set;
        }

        /// <summary>
        /// http request parameters
        /// </summary>
        public Dictionary<string, string> RequestParameters
        {
            get;set;
        }

        /// <summary>
        /// http request files
        /// </summary>
        public Dictionary<string, byte[]> RequestFiles
        {
            get;set;
        }

        /// <summary>
        /// timeout (in seconds, default 300 seconds)
        /// </summary>
        public int TimeOutSeconds
        {
            get; set;
        } = 300;
    }
}
