using System;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using NSwag.Generation.AspNetCore;
using Sixnet.App;
using Sixnet.Serialization.Json;

namespace Sixnet.Web
{
    /// <summary>
    /// Sixnet web options
    /// </summary>
    public class SixnetHostOptions
    {
        /// <summary>
        /// Whether disable jwt authentication
        /// </summary>
        public bool DisableJwtAuthentication { get; set; }

        /// <summary>
        /// Whether disable authorization
        /// </summary>
        public bool DisableAuthorization { get; set; }

        /// <summary>
        /// Gets or sets configure json
        /// </summary>
        public Action<JsonSerializationOptions> ConfigureJson { get; set; }

        /// <summary>
        /// Whether disable api versioning
        /// </summary>
        public bool DisableApiVersioning { get; set; }

        /// <summary>
        /// Whether disable global route prefix
        /// </summary>
        public bool DisableGlobalRoutePrefix { get; set; }

        /// <summary>
        /// Whether disable swagger
        /// </summary>
        public bool DisableSwagger { get; set; }

        /// <summary>
        /// Gets or sets the api route prefix
        /// </summary>
        public string ApiRoutePrefix { get; set; } = "api";

        /// <summary>
        /// Gets or sets the configure json
        /// </summary>
        public Action<MvcOptions> ConfigureMvc { get; set; }

        /// <summary>
        /// Gets or sets configure swagger
        /// </summary>
        public Action<ApiVersionDescription, AspNetCoreOpenApiDocumentGeneratorSettings> ConfigureSwagger { get; set; }

        /// <summary>
        /// Whether disable exception filter
        /// </summary>
        public bool DisableExceptionFilter { get; set; }

        /// <summary>
        /// Gets or sets configure services
        /// </summary>
        public Action<IServiceCollection> ConfigureServices { get; set; }

        /// <summary>
        /// Gets or sets configure application
        /// </summary>
        public Action<ApplicationOptions> ConfigureApplication { get; set; }

        /// <summary>
        /// Whether use localization
        /// </summary>
        public bool UseLocalization { get; set; }

        /// <summary>
        /// Whether disable default cors
        /// </summary>
        public bool DisableDefaultCors { get; set; }
    }
}
