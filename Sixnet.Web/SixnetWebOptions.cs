using System;
using Asp.Versioning.ApiExplorer;
using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using NSwag.Generation.AspNetCore;
using Sixnet.DependencyInjection;
using Microsoft.AspNetCore.SpaServices;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Routing;

namespace Sixnet.Web
{
    /// <summary>
    /// Sixnet web options
    /// </summary>
    public class SixnetWebOptions : SixnetOptions
    {
        /// <summary>
        /// Whether use jwt authentication.
        /// Default is true.
        /// </summary>
        public bool UseJwtAuthentication { get; set; } = true;

        /// <summary>
        /// Whether use authorization.
        /// Default is true.
        /// </summary>
        public bool UseAuthorization { get; set; } = true;

        /// <summary>
        /// Whether use api versioning.
        /// Default is true.
        /// </summary>
        public bool UseApiVersioning { get; set; } = true;

        /// <summary>
        /// Whether use global route prefix.
        /// Default is true.
        /// </summary>
        public bool UseGlobalRoutePrefix { get; set; } = true;

        /// <summary>
        /// Whether use swagger.
        /// Default is true.
        /// </summary>
        public bool UseSwagger { get; set; } = true;

        /// <summary>
        /// Gets or sets the api route prefix
        /// Default is "api"
        /// </summary>
        public string ApiRoutePrefix { get; set; } = "api";

        /// <summary>
        /// Whether use exception filter
        /// Default is true.
        /// </summary>
        public bool UseExceptionFilter { get; set; } = true;

        /// <summary>
        /// Whether use https redirection
        /// Default is true.
        /// </summary>
        public bool UseHttpsRedirection { get; set; } = true;

        /// <summary>
        /// Whether use default cors
        /// Default is true.
        /// </summary>
        public bool UseDefaultCors { get; set; } = true;

        /// <summary>
        /// Whether lowercase urls
        /// Default is true.
        /// </summary>
        public bool LowercaseUrls { get; set; } = true;

        /// <summary>
        /// Whether use hsts
        /// Default is true.
        /// </summary>
        public bool UseHsts { get; set; } = true;

        /// <summary>
        /// Whether use statice files
        /// Default is true
        /// </summary>
        public bool UseStaticFile { get; set; } = true;

        /// <summary>
        /// Gets or sets the static file options
        /// </summary>
        public StaticFileOptions StaticFileOptions { get; set; }

        /// <summary>
        /// Whether use sixnet session context
        /// Default is true.
        /// </summary>
        public bool UseSixnetSessionContext { get; set; } = true;

        /// <summary>
        /// Exception path
        /// </summary>
        public string ExceptionPath { get; set; }

        /// <summary>
        /// Whether use spa 
        /// </summary>
        public bool UseSpa { get; set; }

        /// <summary>
        /// Gets or sets the spa app root path
        /// </summary>
        public string SpaRootPath { get; set; }

        /// <summary>
        /// Configure host builder
        /// </summary>
        public Action<IHostBuilder> ConfigureHostBuilder { get; set; }

        /// <summary>
        /// Gets or sets the configure json
        /// </summary>
        public Action<MvcOptions> ConfigureMvc { get; set; }

        /// <summary>
        /// Gets or sets configure swagger
        /// </summary>
        public Action<ApiVersionDescription, AspNetCoreOpenApiDocumentGeneratorSettings> ConfigureSwagger { get; set; }

        /// <summary>
        /// Gets or sets configure application builder
        /// </summary>
        public Action<IApplicationBuilder, IWebHostEnvironment> ConfigureApplicationBuilder { get; set; }

        /// <summary>
        /// Configure request localization
        /// </summary>
        public Action<RequestLocalizationOptions> ConfigureRequestLocalization { get; set; }

        /// <summary>
        /// Configure cors
        /// </summary>
        public Action<CorsPolicyBuilder> ConfigureCors { get; set; }

        /// <summary>
        /// Configure endpoints
        /// </summary>
        public Action<IEndpointRouteBuilder> ConfigureEndpoints { get; set; } = endpoints =>
        {
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
        };

        /// <summary>
        /// Configure spa builder
        /// </summary>
        public Action<ISpaBuilder> ConfigureSpaBuilder { get; set; } = spa => 
        {
            spa.Options.DefaultPageStaticFileOptions = new StaticFileOptions
            {
                OnPrepareResponse = fileCtx =>
                {
                    fileCtx.Context.Response.Headers.Remove("cache-control");
                    fileCtx.Context.Response.Headers.Add(
                        "cache-control", "no-store, no-cache, must-revalidate");
                }
            };
        };
    }
}
