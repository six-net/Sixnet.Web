using System;
using Microsoft.AspNetCore.Mvc;
using Sixnet.DependencyInjection;
using Sixnet.Serialization.Json;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Configure json behavior
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="configure">Configure json</param>
        /// <returns></returns>
        public static IServiceCollection ConfigureJson(this IServiceCollection services, Action<SixnetJsonSerializationOptions> configure = null)
        {
            services?.Configure<JsonOptions>(options =>
            {
                var jsonOptions = SixnetContainer.GetOptions<SixnetJsonSerializationOptions>() ?? new SixnetJsonSerializationOptions();
                if (configure != null)
                {
                    configure?.Invoke(jsonOptions);
                }
                jsonOptions?.MergeToJsonSerializerOptions(options.JsonSerializerOptions);
            });

            return services;
        }
    }
}
