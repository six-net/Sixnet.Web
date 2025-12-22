using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Asp.Versioning;
using Asp.Versioning.ApiExplorer;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

using NSwag;
using NSwag.Generation.AspNetCore;

using Sixnet.App;
using Sixnet.DependencyInjection;
using Sixnet.Model;
using Sixnet.Security.Authentication;
using Sixnet.Token.Jwt;
using Sixnet.Web.Middleware;
using Sixnet.Web.Mvc.Filters;
using Sixnet.Web.Mvc.Formatters;
using Sixnet.Web.Mvc.ModelBinding.Validation;
using Sixnet.Web.Mvc.Routing;
using Sixnet.Web.Security.Authorization;
using Sixnet.Web.Swagger.Processors;

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

            // Configure app builder
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
                    if (webOptions.RemoveDefaultModelValidator)
                    {
                        options.ModelValidatorProviders.Clear();
                    }
                    if (webOptions.UseCustomModelValidator)
                    {
                        options.ModelValidatorProviders.Add(new SixnetDataAnnotationsModelValidatorProvider());
                    }
                    if (webOptions.UseAuthorization)
                    {
                        options.Filters.Add<SixnetAuthorizeFilter>();
                    }
                    if (webOptions.KebabCaseUrls)
                    {
                        options.Conventions.Add(new RouteTokenTransformerConvention(new KebabCaseParameterTransformer()));
                    }
                    if (webOptions.UseGlobalRoutePrefix)
                    {
                        options.UseGlobalRoutePrefix(new RouteAttribute(webOptions.UseApiVersioning ? webOptions.ApiRoutePrefix + "/v{version:apiVersion}" : webOptions.ApiRoutePrefix));
                    }
                    options.Filters.Add<SixnetActionFilter>();
                    webOptions.ConfigureMvc(options);
                });

                #endregion

                #region Jwt

                var authenOptions = SixnetContainer.GetOptions<SixnetAuthenticationOptions>();
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
                            if (authenOptions != null)
                            {
                                tokenValidationParameters.ValidIssuer = authenOptions.JwtValidIssuer;
                                tokenValidationParameters.ValidAudience = authenOptions.JwtValidAudience;
                                if (!string.IsNullOrWhiteSpace(authenOptions.JwtIssuerSigningKey))
                                {
                                    tokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenOptions.JwtIssuerSigningKey));
                                }
                                if (authenOptions.JwtClockSkewSeconds > 0)
                                {
                                    tokenValidationParameters.ClockSkew = TimeSpan.FromSeconds(authenOptions.JwtClockSkewSeconds);
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

                services.ConfigureJson();

                #endregion

                #region Routing

                services.AddRouting(options =>
                {
                    options.LowercaseUrls = webOptions.LowercaseUrls;
                });

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
                    .AddApiExplorer(options =>
                    {
                        options.GroupNameFormat = "'v'V";
                        options.AssumeDefaultVersionWhenUnspecified = true;
                        options.SubstituteApiVersionInUrl = true;
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
                                ConfigSwaggerDoc(webOptions, config, description);
                                webOptions.ConfigureSwagger(description, config);
                            });
                        }
                    }
                    else
                    {
                        services.AddOpenApiDocument(config =>
                        {
                            ConfigSwaggerDoc(webOptions, config, null);
                            webOptions.ConfigureSwagger(null, config);
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
            webOptions.ConfigureService(configureHostServices, true);
            webOptions.SetHostBuilder(builder);
            builder.UseServiceProviderFactory(new SixnetServiceProviderFactory(webOptions));
            webOptions.SetHostBuilder(builder);
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
            if (webOptions.ConfigureApplicationBuilderAction != null)
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
                if (webOptions.WrapExceptionResult)
                {
                    app.UseMiddleware<SixnetWrapExceptionMiddleware>();
                }
                if (webOptions.UseHttpLogging)
                {
                    app.UseMiddleware<SixnetHttpLoggingMiddleware>();
                }
                app.UseRequestLocalization(rdlOptions =>
                {
                    rdlOptions.DefaultRequestCulture = new RequestCulture("zh");
                    rdlOptions.SupportedCultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
                    rdlOptions.SupportedUICultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
                    webOptions.ConfigureRequestLocalization(rdlOptions);
                });
                if (webOptions.UseHttpsRedirection)
                {
                    app.UseHttpsRedirection();
                }
                if (webOptions.UseDefaultCors)
                {
                    app.UseCors();
                }
                else
                {
                    app.UseCors(corsBuilder =>
                    {
                        webOptions.ConfigureCors(corsBuilder);
                    });
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
                if (webOptions.UseSpa)
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
                    app.UseSixnetSessionContext();
                }
                if (webOptions.UseSwagger)
                {
                    app.UseOpenApi();
                    app.UseSwaggerUi();
                }
                if (webOptions.UseAuthorization)
                {
                    app.UseAuthorization();
                }
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapDefaultControllerRoute();
                });
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
        static void ConfigSwaggerDoc(SixnetWebOptions options, AspNetCoreOpenApiDocumentGeneratorSettings config, ApiVersionDescription apiVersionDescription)
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
            config.SchemaSettings.GenerateEnumMappingDescription = true;
            config.SchemaSettings.AllowReferencesWithProperties = true;
            config.DocumentName = docName;
            config.ApiGroupNames = apiVersionDescription == null ? null : new string[] { version };
            config.AddOperationFilter(context =>
            {
                context.OperationDescription.Operation.Parameters.Add(new OpenApiParameter()
                {
                    Name = "Accept-Language",
                    Kind = OpenApiParameterKind.Header,
                    Type = NJsonSchema.JsonObjectType.String,
                    IsRequired = false,
                    Default = "zh-Hans"
                });
                if (context is AspNetCoreOperationProcessorContext aspnetContext)
                {
                    foreach (var apiResponse in aspnetContext.ApiDescription.SupportedResponseTypes)
                    {
                        var returnType = apiResponse.Type;
                        if (!typeof(ISixnetResult).IsAssignableFrom(returnType))
                        {
                            if (returnType != null && returnType != typeof(void))
                            {
                                apiResponse.Type = typeof(SixnetResult<>).MakeGenericType(returnType);
                            }
                            else
                            {
                                apiResponse.Type = typeof(SixnetResult);
                            }
                        }
                    }
                }
                return true;
            });

            if (options.RemoveTagFromSwaggerOperationId)
            {
                config.OperationProcessors.Add(new RemoveTagFromOperationIdProcessor());
            }
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
