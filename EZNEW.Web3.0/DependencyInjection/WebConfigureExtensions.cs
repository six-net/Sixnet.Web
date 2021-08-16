using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EZNEW.Web;
using EZNEW.Serialization.Json;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class WebConfigureExtensions
    {
        /// <summary>
        /// Configure web options
        /// </summary>
        /// <param name="services">Services</param>
        /// <param name="configure">Web options configure action</param>
        public static IServiceCollection ConfigureWebBehavior(this IServiceCollection services, Action<WebOptions> configure = null)
        {
            WebOptions webOptions = new WebOptions();
            configure?.Invoke(webOptions);

            #region Json serialization

            services?.Configure<JsonOptions>(option =>
            {
                webOptions.JsonSerializationOptions.MergeToJsonSerializerOptions(option.JsonSerializerOptions);
            });

            #endregion

            return services;
        }
    }
}
