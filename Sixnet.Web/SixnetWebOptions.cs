using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace Sixnet.Web
{
    /// <summary>
    /// Sixnet web options
    /// </summary>
    public class SixnetWebOptions : SixnetHostOptions
    {
        /// <summary>
        /// Whether not configure default middlewares
        /// </summary>
        public bool NotConfigureDefaultMiddlewares { get; set; }

        /// <summary>
        /// Whether disable https redirection
        /// </summary>
        public bool DisableHttpsRedirection { get; set; }

        /// <summary>
        /// Gets or sets configure application builder
        /// </summary>
        public Action<IApplicationBuilder, IWebHostEnvironment> ConfigureApplicationBuilder { get; set; }
    }
}
