using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;
using EZNEW.Web.Security.Authentication.Cookie;
using EZNEW.Web.Security.Authentication.Cookie.Ticket;
using EZNEW.DependencyInjection;
using EZNEW.Web.Utility;

namespace Microsoft.AspNetCore.Authentication
{
    public static class CookieAuthenticationExtensions
    {
        /// <summary>
        /// Add cookie authentication
        /// </summary>
        /// <param name="builder">Authentication builder</param>
        /// <param name="cookieOptionConfiguration">Cookie option configuration</param>
        public static void AddCookieAuthentication(this AuthenticationBuilder builder, Action<CustomCookieOption> cookieOptionConfiguration)
        {
            var cookieOption = new CustomCookieOption();
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
                        options.SessionStore = ContainerManager.Resolve<ITicketDistributedStore>();
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
        public static void AddCookieAuthentication(this IServiceCollection services, Action<CustomCookieOption> cookieOptionConfiguration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddCookieAuthentication(cookieOptionConfiguration);
        }
    }
}
