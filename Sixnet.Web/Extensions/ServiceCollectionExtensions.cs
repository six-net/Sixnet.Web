using System;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NSwag;
using NSwag.Generation.AspNetCore;
using Sixnet.DependencyInjection;
using Sixnet.Model;
using Sixnet.Serialization.Json;
using Sixnet.Token.Jwt;
using Sixnet.Web;
using Sixnet.Web.Mvc.Filters;
using Sixnet.Web.Mvc.Formatters;
using Sixnet.Web.Mvc.ModelBinding.Validation;
using Sixnet.Web.Security.Authorization;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Configure json behavior
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="configureJson">Configure json</param>
        /// <returns></returns>
        public static IServiceCollection ConfigureJson(this IServiceCollection services, Action<JsonSerializationOptions> configureJson = null)
        {
            services?.Configure<JsonOptions>(options =>
            {
                var jsonSerializationOptions = new JsonSerializationOptions();
                jsonSerializationOptions?.MergeFromJsonSerializerOptions(options.JsonSerializerOptions);
                configureJson?.Invoke(jsonSerializationOptions);
                jsonSerializationOptions?.ApplyToJsonSerializerOptions(options.JsonSerializerOptions);
            });

            return services;
        }
    }
}
