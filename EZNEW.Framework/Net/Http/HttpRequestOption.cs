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
        /// 是否启用Cookie
        /// </summary>
        public bool UseCookie
        {
            get;set;
        }

        /// <summary>
        /// 请求消息
        /// </summary>
        public HttpRequestMessage RequestMessage
        {
            get;set;
        }

        /// <summary>
        /// 请求参数
        /// </summary>
        public Dictionary<string, string> RequestParameters
        {
            get;set;
        }

        /// <summary>
        /// 请求文件
        /// </summary>
        public Dictionary<string, byte[]> RequestFiles
        {
            get;set;
        }

        /// <summary>
        /// 超时时间(以秒为单位，默认300秒)
        /// </summary>
        public int TimeOutSeconds
        {
            get; set;
        } = 300;
    }
}
