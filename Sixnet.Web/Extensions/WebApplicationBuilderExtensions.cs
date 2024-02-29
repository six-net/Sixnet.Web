using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using NSwag;
using NSwag.Generation.AspNetCore;
using Sixnet.App;
using Sixnet.DependencyInjection;
using Sixnet.Model;
using Sixnet.Token.Jwt;
using Sixnet.Web.Mvc.Filters;
using Sixnet.Web.Mvc.Formatters;
using Sixnet.Web.Mvc.ModelBinding.Validation;
using Sixnet.Web.Security.Authorization;

namespace Sixnet.Web.Extensions
{
    /// <summary>
    /// Web application builder extensions
    /// </summary>
    public static class WebApplicationBuilderExtensions
    {
        /// <summary>
        /// Add sixnet web
        /// </summary>
        /// <param name="builder">Web application builder</param>
        /// <param name="configure">Configure</param>
        /// <returns></returns>
        public static void RunWeb(this WebApplicationBuilder builder, Action<SixnetWebOptions> configure = null)
        {
            GetWebApp(builder, configure).Run();
        }

        /// <summary>
        /// Add sixnet web
        /// </summary>
        /// <param name="builder">Web application builder</param>
        /// <param name="configure">Configure</param>
        /// <returns></returns>
        public static Task RunWebAsync(this WebApplicationBuilder builder, Action<SixnetWebOptions> configure = null)
        {
            return GetWebApp(builder, configure).RunAsync();
        }

        /// <summary>
        /// Configure web core
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configure"></param>
        static WebApplication GetWebApp(WebApplicationBuilder builder, Action<SixnetWebOptions> configure = null)
        {
            var webOptions = GetDefaultWebOptions();
            configure?.Invoke(webOptions);

            // Configure host builder
            ConfigureHostBuilder(builder.Host, webOptions);

            var app = builder.Build();

            // Configure app
            ConfigureApplicationBuilder(app, builder.Environment, webOptions);

            return app;
        }

        /// <summary>
        /// Add sixnet web
        /// </summary>
        /// <param name="builder">Host builder</param>
        /// <param name="configure">Configure</param>
        /// <returns></returns>
        public static IHostBuilder AddSixnetWeb(this IHostBuilder builder, Action<SixnetWebOptions> configure = null)
        {
            var webOptions = GetDefaultWebOptions();
            configure?.Invoke(webOptions);

            ConfigureHostBuilder(builder, webOptions);

            return builder;
        }

        /// <summary>
        ///  Configure host builder
        /// </summary>
        /// <param name="builder">Host builder</param>
        /// <param name="webOptions">Host options</param>
        static void ConfigureHostBuilder(IHostBuilder builder, SixnetWebOptions webOptions)
        {
            void configureHostServices(IServiceCollection services)
            {
                #region Http context

                services.AddHttpContextAccessor();

                #endregion

                #region Mvc

                services.AddControllersWithViews(options =>
                {
                    options.InputFormatters.Insert(0, new TextPlainInputFormatter());
                    options.ModelValidatorProviders.Add(new SixnetDataAnnotationsModelValidatorProvider());
                    if (!webOptions.UseAuthorization)
                    {
                        options.Filters.Add<ExtendAuthorizeFilter>();
                    }
                    if (!webOptions.UseGlobalRoutePrefix)
                    {
                        options.UseGlobalRoutePrefix(new RouteAttribute(webOptions.UseApiVersioning ? webOptions.ApiRoutePrefix : webOptions.ApiRoutePrefix + "/v{version:apiVersion}"));
                    }
                    if (!webOptions.UseExceptionFilter)
                    {
                        options.Filters.Add<SixnetExceptionFilter>();
                    }
                    webOptions.ConfigureMvc?.Invoke(options);
                });

                #endregion

                #region Jwt

                var jwtOptions = SixnetContainer.GetOptions<JwtOptions>();
                if (webOptions.UseJwtAuthentication)
                {
                    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                        .AddJwtBearer(jwtBearOptions =>
                        {
                            var tokenValidationParameters = new TokenValidationParameters()
                            {
                                NameClaimType = JwtClaimTypes.Name,
                                RoleClaimType = JwtClaimTypes.Role
                            };
                            if (jwtOptions != null)
                            {
                                tokenValidationParameters.ValidIssuer = jwtOptions.ValidIssuer;
                                tokenValidationParameters.ValidAudience = jwtOptions.ValidAudience;
                                if (!string.IsNullOrWhiteSpace(jwtOptions.IssuerSigningKey))
                                {
                                    tokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.IssuerSigningKey));
                                }
                                if (jwtOptions.ClockSkewSeconds > 0)
                                {
                                    tokenValidationParameters.ClockSkew = TimeSpan.FromSeconds(jwtOptions.ClockSkewSeconds);
                                }
                            }
                            jwtBearOptions.TokenValidationParameters = tokenValidationParameters;
                        });
                }

                #endregion

                #region Authorization

                if (webOptions.UseAuthorization)
                {
                    services.AddAuthorization();
                }

                #endregion

                #region Json

                services.ConfigureJson(webOptions.ConfigureJson);

                #endregion

                #region Routing

                if (webOptions.LowercaseUrls)
                {
                    services.AddRouting(options => options.LowercaseUrls = true);
                }

                #endregion

                #region Versioning

                if (webOptions.UseApiVersioning)
                {
                    services.AddApiVersioning(options =>
                    {
                        options.ReportApiVersions = true;
                        options.AssumeDefaultVersionWhenUnspecified = true;
                        options.DefaultApiVersion = new ApiVersion(1, 0);
                    })
                    .AddApiExplorer(option =>
                    {
                        option.GroupNameFormat = "'v'V";
                        option.AssumeDefaultVersionWhenUnspecified = true;
                        option.SubstituteApiVersionInUrl = true;
                    })
                    .AddMvc();
                }

                #endregion

                #region Swagger

                if (webOptions.UseSwagger)
                {
                    if (webOptions.UseApiVersioning)
                    {
                        var serviceProvider = services.BuildServiceProvider();
                        var apiDocProvider = serviceProvider.GetService<IApiVersionDescriptionProvider>();
                        foreach (var description in apiDocProvider.ApiVersionDescriptions)
                        {
                            services.AddOpenApiDocument(config =>
                            {
                                ConfigSwaggerDoc(config, description);
                                webOptions.ConfigureSwagger?.Invoke(description, config);
                            });
                        }
                    }
                    else
                    {
                        services.AddOpenApiDocument(config =>
                        {
                            ConfigSwaggerDoc(config, null);
                            webOptions.ConfigureSwagger?.Invoke(null, config);
                        });
                    }
                }

                #endregion

                #region Cors

                if (webOptions.UseDefaultCors)
                {
                    services.AddCors(options =>
                    {
                        options.AddDefaultPolicy(builder =>
                        {
                            builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                        });
                    });
                }

                #endregion

                #region Spa

                if (webOptions.UseSpa)
                {
                    services.AddSpaStaticFiles(spa =>
                    {
                        spa.RootPath = webOptions.SpaRootPath;
                    });
                }

                #endregion
            }
            var optionsConfigureServices = webOptions.ConfigureService;
            webOptions.ConfigureService = (services) =>
            {
                configureHostServices(services);
                optionsConfigureServices?.Invoke(services);
            };
            builder.UseServiceProviderFactory(new SixnetServiceProviderFactory(webOptions));
            webOptions?.ConfigureHostBuilder?.Invoke(builder);
            SixnetWeb.Options = webOptions;
        }

        /// <summary>
        /// Configure application builder
        /// </summary>
        /// <param name="app">Application builder</param>
        /// <param name="env">Host environment</param>
        /// <param name="webOptions">Web options</param>
        static void ConfigureApplicationBuilder(IApplicationBuilder app, IWebHostEnvironment env, SixnetWebOptions webOptions)
        {
            if (webOptions.ConfigureApplicationBuilder != null)
            {
                webOptions.ConfigureApplicationBuilder(app, env);
            }
            else
            {
                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(webOptions.ExceptionPath))
                    {
                        app.UseExceptionHandler(webOptions.ExceptionPath);
                    }
                    if (webOptions.UseHsts)
                    {
                        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                        app.UseHsts();
                    }
                }
                if (webOptions.ConfigureRequestLocalization != null)
                {
                    app.UseRequestLocalization(webOptions.ConfigureRequestLocalization);
                }
                if (webOptions.UseHttpsRedirection)
                {
                    app.UseHttpsRedirection();
                }
                if (webOptions.UseDefaultCors)
                {
                    app.UseCors();
                }
                else if (webOptions.ConfigureCors != null)
                {
                    app.UseCors(webOptions.ConfigureCors);
                }
                if (webOptions.UseStaticFile)
                {
                    if (webOptions.StaticFileOptions == null)
                    {
                        app.UseStaticFiles();
                    }
                    else
                    {
                        app.UseStaticFiles(webOptions.StaticFileOptions);
                    }
                }
                if(webOptions.UseSpa)
                {
                    app.UseSpaStaticFiles();
                }
                app.UseRouting();
                if (webOptions.UseJwtAuthentication)
                {
                    app.UseAuthentication();
                }
                if (webOptions.UseSixnetSessionContext)
                {
                    app.UseSessionContext();
                }
                if (webOptions.UseSwagger)
                {
                    app.UseOpenApi();
                    app.UseSwaggerUi3();
                }
                if (webOptions.UseAuthorization)
                {
                    app.UseAuthorization();
                }
                if (webOptions.ConfigureEndpoints != null)
                {
                    app.UseEndpoints(webOptions.ConfigureEndpoints);
                }
                if (webOptions.UseSpa)
                {
                    app.UseSpa(webOptions.ConfigureSpaBuilder);
                }
            }
        }

        /// <summary>
        /// Config swagger doc
        /// </summary>
        /// <param name="config"></param>
        /// <param name="apiVersionDescription"></param>
        static void ConfigSwaggerDoc(AspNetCoreOpenApiDocumentGeneratorSettings config, ApiVersionDescription apiVersionDescription)
        {
            var title = SixnetApplication.Current.Title;
            var version = SixnetApplication.Current.Version;
            var docName = SixnetApplication.Current.Title;

            if (apiVersionDescription != null)
            {
                title = $"{title}_{apiVersionDescription.GroupName}";
                docName = title;
                version = apiVersionDescription.GroupName;
            }

            config.PostProcess = doc =>
            {
                doc.Info.Title = title;
                doc.Info.Version = version;
            };
            config.UseControllerSummaryAsTagDescription = true;
            config.AddSecurity("JwtBearer", Enumerable.Empty<string>(), new OpenApiSecurityScheme
            {
                Description = "JWT Authentication, please input {token}",
                Name = "Authorization",
                In = OpenApiSecurityApiKeyLocation.Header,
                Type = OpenApiSecuritySchemeType.Http,
                Scheme = "bearer"
            });
            config.GenerateEnumMappingDescription = true;
            config.AllowReferencesWithProperties = true;
            config.DocumentName = docName;
            config.ApiGroupNames = apiVersionDescription == null ? null : new string[] { version };
            config.AddOperationFilter(context =>
            {
                context.OperationDescription.Operation.Parameters.Add(new OpenApiParameter()
                {
                    Name = "Accept-Language",
                    Kind = OpenApiParameterKind.Header,
                    Type = NJsonSchema.JsonObjectType.String,
                    IsRequired = true,
                    Default = "zh-Hans"
                });
                if (context is AspNetCoreOperationProcessorContext aspnetContext)
                {
                    foreach (var apiResponse in aspnetContext.ApiDescription.SupportedResponseTypes)
                    {
                        var returnType = apiResponse.Type;
                        if (returnType != null && !typeof(ISixnetResult).IsAssignableFrom(returnType))
                        {
                            apiResponse.Type = typeof(SixnetResult<>).MakeGenericType(returnType);
                        }
                    }
                }
                return true;
            });
        }

        /// <summary>
        /// Get default web options
        /// </summary>
        /// <returns></returns>
        static SixnetWebOptions GetDefaultWebOptions()
        {
            return new SixnetWebOptions();
        }
    }
}
