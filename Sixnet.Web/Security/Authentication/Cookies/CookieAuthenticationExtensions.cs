using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;
using Sixnet.Web.Security.Authentication.Cookies;
using Sixnet.Web.Security.Authentication.Cookies.Ticket;
using Sixnet.DependencyInjection;
using Sixnet.Web.Utility;

namespace Microsoft.AspNetCore.Authentication
{
    public static class CookieAuthenticationExtensions
    {
        /// <summary>
        /// Add cookie authentication
        /// </summary>
        /// <param name="builder">Authentication builder</param>
        /// <param name="cookieOptionConfiguration">Cookie option configuration</param>
        public static void AddCookieAuthentication(this AuthenticationBuilder builder, Action<CustomCookieOptions> cookieOptionConfiguration)
        {
            var cookieOption = new CustomCookieOptions();
            cookieOptionConfiguration?.Invoke(cookieOption);
            var configureOptions = cookieOption?.CookieConfiguration;
            void customDefaultConfigure(CookieAuthenticationOptions options)
            {
                CookieAuthenticationEventHandler.ForceValidatePrincipal = cookieOption.ForceValidatePrincipal;
                CookieAuthenticationEventHandler.OnValidatePrincipalAsync = cookieOption.ValidatePrincipalAsync;
                options.EventsType = typeof(CookieAuthenticationEventHandler);
                options.Cookie.HttpOnly = true;
                var storageModel = cookieOption?.StorageModel ?? CookieStorageModel.Default;
                switch (storageModel)
                {
                    case CookieStorageModel.Default:
                    default:
                        options.SessionStore = null;
                        break;
                    case CookieStorageModel.Distributed:
                        options.SessionStore = SixnetContainer.GetService<ITicketDistributedStore>();
                        break;
                    case CookieStorageModel.InMemory:
                        options.SessionStore = new CookieMemoryCacheTicketStore();
                        break;
                }
                if (string.IsNullOrWhiteSpace(options.Cookie.Name))
                {
                    options.Cookie.Name = string.Format("{0}_{1}_{2}", HttpClientHelper.Host, HttpClientHelper.Port, "authenticationkey_~!@#$%^&*").MD5();
                }
            }
            if (configureOptions != null)
            {
                configureOptions += customDefaultConfigure;
            }
            else
            {
                configureOptions = customDefaultConfigure;
            }
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;

            }).AddCookie(configureOptions);
            builder.Services.AddSingleton<CookieAuthenticationEventHandler>();
        }

        /// <summary>
        /// Add cookie authentication
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="cookieOptionConfiguration">Cookie option configuration</param>
        public static void AddCookieAuthentication(this IServiceCollection services, Action<CustomCookieOptions> cookieOptionConfiguration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddCookieAuthentication(cookieOptionConfiguration);
        }
    }
}
