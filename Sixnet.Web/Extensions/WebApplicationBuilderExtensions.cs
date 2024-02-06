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
using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

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
            var webOptions = builder.Configuration.GetSection(nameof(SixnetWebOptions))?.Get<SixnetWebOptions>() ?? new SixnetWebOptions();
            configure?.Invoke(webOptions);

            // Web options
            builder.Services.AddSingleton(webOptions);

            // Configure host builder
            ConfigureHostBuilder(builder.Host, webOptions);

            var app = builder.Build();

            // Configure app
            ConfigureApplicationBuilder(app, builder.Environment, webOptions);

            app.Run();
        }

        /// <summary>
        /// Add sixnet web
        /// </summary>
        /// <param name="builder">Host builder</param>
        /// <param name="configure">Configure</param>
        /// <returns></returns>
        public static IHostBuilder AddSixnetWeb(this IHostBuilder builder, Action<SixnetHostOptions> configure = null)
        {
            var hostOptions = new SixnetHostOptions();
            configure?.Invoke(hostOptions);

            ConfigureHostBuilder(builder, hostOptions);

            return builder;
        }

        /// <summary>
        ///  Configure host builder
        /// </summary>
        /// <param name="builder">Host builder</param>
        /// <param name="hostOptions">Host options</param>
        static void ConfigureHostBuilder(IHostBuilder builder, SixnetHostOptions hostOptions)
        {
            builder.UseServiceProviderFactory(new SixnetServiceProviderFactory((services) =>
            {
                var configuration = ContainerManager.Resolve<IConfiguration>();

                #region Http context

                services.AddHttpContextAccessor();

                #endregion

                #region Mvc

                services.AddControllersWithViews(options =>
                {
                    options.InputFormatters.Insert(0, new TextPlainInputFormatter());
                    options.ModelValidatorProviders.Add(new SixnetDataAnnotationsModelValidatorProvider());
                    if (!hostOptions.DisableAuthorization)
                    {
                        options.Filters.Add<ExtendAuthorizeFilter>();
                    }
                    if (!hostOptions.DisableGlobalRoutePrefix)
                    {
                        options.UseGlobalRoutePrefix(new RouteAttribute(hostOptions.DisableApiVersioning ? hostOptions.ApiRoutePrefix : hostOptions.ApiRoutePrefix + "/v{version:apiVersion}"));
                    }
                    if (!hostOptions.DisableExceptionFilter)
                    {
                        options.Filters.Add<SixnetExceptionFilter>();
                    }
                    hostOptions.ConfigureMvc?.Invoke(options);
                });

                #endregion

                #region Jwt

                var jwtConfig = configuration?.GetSection(nameof(JwtConfiguration)).Get<JwtConfiguration>();
                if (!hostOptions.DisableJwtAuthentication)
                {
                    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                        .AddJwtBearer(jwtOptions =>
                        {
                            var tokenValidationParameters = new TokenValidationParameters()
                            {
                                NameClaimType = JwtClaimTypes.Name,
                                RoleClaimType = JwtClaimTypes.Role
                            };
                            if (jwtConfig != null)
                            {
                                tokenValidationParameters.ValidIssuer = jwtConfig.ValidIssuer;
                                tokenValidationParameters.ValidAudience = jwtConfig.ValidAudience;
                                if (!string.IsNullOrWhiteSpace(jwtConfig.IssuerSigningKey))
                                {
                                    tokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.IssuerSigningKey));
                                }
                                if (jwtConfig.ClockSkewSeconds > 0)
                                {
                                    tokenValidationParameters.ClockSkew = TimeSpan.FromSeconds(jwtConfig.ClockSkewSeconds);
                                }
                            }
                            jwtOptions.TokenValidationParameters = tokenValidationParameters;
                        });
                }

                #endregion

                #region Authorization

                if (!hostOptions.DisableAuthorization)
                {
                    services.AddAuthorization();
                }

                #endregion

                #region Json

                services.ConfigureJson(hostOptions.ConfigureJson);

                #endregion

                #region Routing

                services.AddRouting(options => options.LowercaseUrls = true);

                #endregion

                #region Versioning

                if (!hostOptions.DisableApiVersioning)
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
                    });
                }

                #endregion

                #region Swagger

                if (!hostOptions.DisableSwagger)
                {
                    if (hostOptions.DisableApiVersioning)
                    {
                        var docGroupName = Assembly.GetEntryAssembly().GetName().Name;
                        services.AddOpenApiDocument(config =>
                        {
                            ConfigSwaggerDoc(config, null);
                            hostOptions.ConfigureSwagger?.Invoke(null, config);
                        });
                    }
                    else
                    {
                        var serviceProvider = services.BuildServiceProvider();
                        var apiDocProvider = serviceProvider.GetService<IApiVersionDescriptionProvider>();
                        foreach (var description in apiDocProvider.ApiVersionDescriptions)
                        {
                            services.AddOpenApiDocument(config =>
                            {
                                ConfigSwaggerDoc(config, description);
                                hostOptions.ConfigureSwagger?.Invoke(description, config);
                            });
                        }
                    }
                }

                #endregion

                #region Cors

                if (!hostOptions.DisableDefaultCors)
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

                hostOptions.ConfigureServices?.Invoke(services);
            }, hostOptions.ConfigureApplication));
        }

        /// <summary>
        /// Configure application builder
        /// </summary>
        /// <param name="app">Application builder</param>
        /// <param name="env">Host environment</param>
        /// <param name="webOptions">Web options</param>
        static void ConfigureApplicationBuilder(IApplicationBuilder app, IWebHostEnvironment env, SixnetWebOptions webOptions)
        {
            if (!webOptions.NotConfigureDefaultMiddlewares)
            {
                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }
                else
                {
                    app.UseExceptionHandler("/Error");
                    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                    app.UseHsts();
                }
                if (webOptions.UseLocalization)
                {
                    app.UseRequestLocalization(new RequestLocalizationOptions
                    {
                        DefaultRequestCulture = new RequestCulture("zh-Hans"),
                        SupportedCultures = CultureInfo.GetCultures(CultureTypes.AllCultures),
                        SupportedUICultures = CultureInfo.GetCultures(CultureTypes.AllCultures),
                    });
                }
                if (!webOptions.DisableHttpsRedirection)
                {
                    app.UseHttpsRedirection();
                }
                if (!webOptions.DisableDefaultCors)
                {
                    app.UseCors();
                }
                app.UseStaticFiles();
                app.UseRouting();
                if (!webOptions.DisableJwtAuthentication)
                {
                    app.UseAuthentication();
                }
                app.UseSessionContext();
                if (!webOptions.DisableSwagger)
                {
                    app.UseOpenApi();
                    app.UseSwaggerUi3();
                }
                if (!webOptions.DisableAuthorization)
                {
                    app.UseAuthorization();
                }
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllerRoute(
                        name: "default",
                        pattern: "{controller=Home}/{action=Index}/{id?}");
                });
            }
        }

        /// <summary>
        /// Config swagger doc
        /// </summary>
        /// <param name="config"></param>
        /// <param name="apiVersionDescription"></param>
        static void ConfigSwaggerDoc(AspNetCoreOpenApiDocumentGeneratorSettings config, ApiVersionDescription apiVersionDescription)
        {
            var title = ApplicationManager.Current.Title;
            var version = ApplicationManager.Current.Version;
            var docName = ApplicationManager.Current.Title;

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
                        if (returnType != null && !typeof(IResult).IsAssignableFrom(returnType))
                        {
                            apiResponse.Type = typeof(Result<>).MakeGenericType(returnType);
                        }
                    }
                }
                return true;
            });
        }
    }
}
