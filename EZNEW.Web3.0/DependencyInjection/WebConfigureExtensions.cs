using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EZNEW.Serialization.Json;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class WebConfigureExtensions
    {
        /// <summary>
        /// Configure json behavior
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="jsonSerializationOptions">Json serialization options</param>
        /// <returns></returns>
        public static IServiceCollection ConfigureJson(this IServiceCollection services, Action<JsonSerializationOptions> configureJsonSerializationOptions = null)
        {
            services?.Configure<JsonOptions>(options =>
            {
                JsonSerializationOptions jsonSerializationOptions = new JsonSerializationOptions();
                jsonSerializationOptions?.MergeFromJsonSerializerOptions(options.JsonSerializerOptions);
                configureJsonSerializationOptions?.Invoke(jsonSerializationOptions);
                jsonSerializationOptions?.ApplyToJsonSerializerOptions(options.JsonSerializerOptions);
            });

            return services;
        }
    }
}
