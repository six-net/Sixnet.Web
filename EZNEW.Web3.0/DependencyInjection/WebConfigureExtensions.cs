using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EZNEW.Web;
using EZNEW.Web.Mvc;

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
                var defaultJsonSerializerOptions = option.JsonSerializerOptions;
                var jsonSerializationOptions = webOptions.JsonSerializationOptions ?? new JsonSerializationOptions();
                if (jsonSerializationOptions.UseCustomConverter)
                {
                    defaultJsonSerializerOptions.Converters.Add(new LongJsonConverter());
                    defaultJsonSerializerOptions.Converters.Add(new LongAllowNullJsonConverter());
                    defaultJsonSerializerOptions.Converters.Add(new ULongJsonConverter());
                    defaultJsonSerializerOptions.Converters.Add(new ULongAllowNullJsonConverter());
                    defaultJsonSerializerOptions.Converters.Add(new DecimalJsonConverter());
                    defaultJsonSerializerOptions.Converters.Add(new DecimalAllowNullJsonConverter());
                }
                if (jsonSerializationOptions.UseCustomPropertyNamingPolicy)
                {
                    defaultJsonSerializerOptions.PropertyNamingPolicy = new DefaultJsonNamingPolicy();
                }
            });

            #endregion

            return services;
        }
    }
}
