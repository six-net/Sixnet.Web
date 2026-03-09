using System;

using Asp.Versioning.ApiExplorer;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices;
using Microsoft.Extensions.Hosting;

using NSwag.Generation.AspNetCore;

using Sixnet.Session;

namespace Sixnet.Web
{
    /// <summary>
    /// Sixnet web options
    /// </summary>
    public class SixnetWebOptions : SixnetOptions
    {
        #region Properties

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
        /// Whether remove tag from operation id.
        /// Default is true
        /// </summary>
        public bool RemoveTagFromSwaggerOperationId { get; set; } = false;

        /// <summary>
        /// Gets or sets the api route prefix
        /// Default is "api"
        /// </summary>
        public string ApiRoutePrefix { get; set; } = "api";

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
        /// Whether kebab-case urls
        /// Default is true
        /// </summary>
        public bool KebabCaseUrls { get; set; } = true;

        /// <summary>
        /// Whether use hsts
        /// Default is true.
        /// </summary>
        public bool UseHsts { get; set; } = true;

        /// <summary>
        /// Whether use static files
        /// Default is true
        /// </summary>
        public bool UseStaticFile { get; set; } = true;

        /// <summary>
        /// Whether use http logging
        /// Default is true
        /// </summary>
        public bool UseHttpLogging { get; set; } = true;

        /// <summary>
        /// Whether intercept all exception then response SixnetResult
        /// Default is true
        /// </summary>
        public bool WrapExceptionResult { get; set; } = true;

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
        /// Remove default model validator
        /// </summary>
        public bool RemoveDefaultModelValidator { get; set; } = true;

        /// <summary>
        /// Use custom cmodel validator
        /// </summary>
        public bool UseCustomModelValidator { get; set; } = true;

        /// <summary>
        /// Unify action result.
        /// Default is true
        /// </summary>
        public bool UnifyActionResult { get; set; } = true;

        /// <summary>
        /// Gets or sets the spa app root path
        /// </summary>
        public string SpaRootPath { get; set; }

        #endregion

        #region Method

        #region Configure mvc

        internal Action<MvcOptions> ConfigureMvcAction;

        /// <summary>
        /// Configure mvc
        /// </summary>
        public SixnetWebOptions ConfigureMvc(Action<MvcOptions> configure, bool toFirst = false)
        {
            ConfigureMvcAction = toFirst
                ? configure + ConfigureMvcAction
                : ConfigureMvcAction + configure;
            return this;
        }

        /// <summary>
        /// Configure mvc
        /// </summary>
        /// <param name="options"></param>
        internal void ConfigureMvc(MvcOptions options)
        {
            ConfigureMvcAction?.Invoke(options);
        }

        #endregion

        #region Configure swagger

        internal Action<ApiVersionDescription, AspNetCoreOpenApiDocumentGeneratorSettings> ConfigureSwaggerAction;

        /// <summary>
        /// Register configure swagger action
        /// </summary>
        public SixnetWebOptions ConfigureSwagger(Action<ApiVersionDescription, AspNetCoreOpenApiDocumentGeneratorSettings> configure, bool toFirst = false)
        {
            ConfigureSwaggerAction = toFirst
                ? configure + ConfigureSwaggerAction
                : ConfigureSwaggerAction + configure;
            return this;
        }

        /// <summary>
        /// Invoke the registered configure swagger action
        /// </summary>
        internal void ConfigureSwagger(
            ApiVersionDescription apiVersion,
            AspNetCoreOpenApiDocumentGeneratorSettings settings)
        {
            ConfigureSwaggerAction?.Invoke(apiVersion, settings);
        }

        #endregion

        #region Configure application builder

        internal Action<IApplicationBuilder, IWebHostEnvironment> ConfigureApplicationBuilderAction;

        /// <summary>
        /// Register configure application builder action
        /// </summary>
        public SixnetWebOptions ConfigureApplicationBuilder(
            Action<IApplicationBuilder, IWebHostEnvironment> configure, bool toFirst = false)
        {
            ConfigureApplicationBuilderAction = toFirst
                ? configure + ConfigureApplicationBuilderAction
                : ConfigureApplicationBuilderAction + configure;
            return this;
        }

        /// <summary>
        /// Invoke the registered configure application builder action
        /// </summary>
        internal void ConfigureApplicationBuilder(
            IApplicationBuilder app,
            IWebHostEnvironment environment)
        {
            ConfigureApplicationBuilderAction?.Invoke(app, environment);
        }

        #endregion

        #region Configure request localization

        internal Action<RequestLocalizationOptions> ConfigureRequestLocalizationAction;

        /// <summary>
        /// Register configure request localization action
        /// </summary>
        public SixnetWebOptions ConfigureRequestLocalization(Action<RequestLocalizationOptions> configure, bool toFirst = false)
        {
            ConfigureRequestLocalizationAction = toFirst
                ? configure + ConfigureRequestLocalizationAction
                : ConfigureRequestLocalizationAction + configure;
            return this;
        }

        /// <summary>
        /// Invoke the registered configure request localization action
        /// </summary>
        internal void ConfigureRequestLocalization(RequestLocalizationOptions options)
        {
            ConfigureRequestLocalizationAction?.Invoke(options);
        }

        #endregion

        #region Configure cors

        internal Action<CorsPolicyBuilder> ConfigureCorsAction;

        /// <summary>
        /// Register configure cors action
        /// </summary>
        public SixnetWebOptions ConfigureCors(Action<CorsPolicyBuilder> configure, bool toFirst = false)
        {
            ConfigureCorsAction = toFirst
                ? configure + ConfigureCorsAction
                : ConfigureCorsAction + configure;
            return this;
        }

        /// <summary>
        /// Invoke the registered configure cors action
        /// </summary>
        internal void ConfigureCors(CorsPolicyBuilder builder)
        {
            ConfigureCorsAction?.Invoke(builder);
        }

        #endregion

        #region Configure spa builder

        internal Action<ISpaBuilder> ConfigureSpaBuilderAction
            = spa =>
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

        /// <summary>
        /// Register configure spa builder action
        /// </summary>
        public SixnetWebOptions ConfigureSpaBuilder(Action<ISpaBuilder> configure, bool toFirst = false)
        {
            ConfigureSpaBuilderAction = toFirst
                ? configure + ConfigureSpaBuilderAction
                : ConfigureSpaBuilderAction + configure;
            return this;
        }

        /// <summary>
        /// Invoke the registered configure spa builder action
        /// </summary>
        internal void ConfigureSpaBuilder(ISpaBuilder spaBuilder)
        {
            ConfigureSpaBuilderAction?.Invoke(spaBuilder);
        }

        #endregion

        #region Configure isolation

        internal Func<HttpContext, IsolationInfo> GetIsolationInfoAction;

        /// <summary>
        /// Configure isolation info
        /// </summary>
        public SixnetWebOptions ConfigureIsolationInfo(Func<HttpContext, IsolationInfo> getIsolationInfo)
        {
            GetIsolationInfoAction = getIsolationInfo;
            return this;
        }

        /// <summary>
        /// Get isolation info
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public IsolationInfo GetIsolationInfo(HttpContext context)
        {
            return GetIsolationInfoAction?.Invoke(context);
        }

        #endregion

        #endregion
    }
}
