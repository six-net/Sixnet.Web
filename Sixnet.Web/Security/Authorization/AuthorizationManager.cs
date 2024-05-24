using Microsoft.Extensions.Options;
using Sixnet.Algorithm.Selection;
using Sixnet.DependencyInjection;
using Sixnet.Net.Http;
using Sixnet.Serialization.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Sixnet.Web.Security.Authorization
{
    /// <summary>
    /// Authorize manager
    /// </summary>
    public static class AuthorizationManager
    {
        /// <summary>
        /// Authorization configuration
        /// </summary>
        static readonly AuthorizationConfiguration AuthorizationConfiguration;

        /// <summary>
        /// Data selection provider
        /// </summary> 
        static readonly SixnetDataSelecter<string> DataSelectionProvider = null;

        static AuthorizationManager()
        {
            var authorizationConfiguration = SixnetContainer.GetService<IOptions<AuthorizationConfiguration>>()?.Value ?? new AuthorizationConfiguration();
            AuthorizationConfiguration = authorizationConfiguration;
            if ((!authorizationConfiguration?.Servers.IsNullOrEmpty()) ?? false)
            {
                DataSelectionProvider = new SixnetDataSelecter<string>(authorizationConfiguration.Servers);
            }
        }

        /// <summary>
        /// Authorize proxy
        /// </summary>
        static Func<AuthorizeOptions, AuthorizeResult> AuthorizeProxy;

        /// <summary>
        /// Whether ingore authentication
        /// </summary>
        public static bool IngoreAuthentication = false;

        /// <summary>
        /// Whether ingore default authorize
        /// </summary>
        public static bool IngoreDefaultAuthorize = false;

        /// <summary>
        /// Default public permission code
        /// </summary>
        public static string DefaultPublicPermissionCode = "-20220515";

        /// <summary>
        /// Configure the authorization
        /// </summary>
        /// <param name="authorizeAcion">Authorize action</param>
        public static void ConfigureAuthorization(Func<AuthorizeOptions, AuthorizeResult> authorizeAcion)
        {
            AuthorizeProxy = authorizeAcion;
        }

        /// <summary>
        /// Authorize
        /// </summary>
        /// <param name="authorizeOptions">Authorize options</param>
        /// <returns>Return the authorize reuslt</returns>
        public static async Task<AuthorizeResult> AuthorizeAsync(AuthorizeOptions authorizeOptions)
        {
            if (authorizeOptions == null)
            {
                return AuthorizeResult.ForbidResult();
            }
            if (!AuthorizationConfiguration.RemoteVerify)
            {
                return AuthorizeProxy?.Invoke(authorizeOptions) ?? AuthorizeResult.SuccessResult();
            }
            string server = SelectRemoteServer();
            if (string.IsNullOrWhiteSpace(server))
            {
                throw new ArgumentNullException(nameof(AuthorizationConfiguration.Servers));
            }
            var result = await SixnetHttp.PostJsonAsync(server, authorizeOptions).ConfigureAwait(false);
            var stringValue = await result.Content.ReadAsStringAsync().ConfigureAwait(false);
            AuthorizeResult verifyResult = SixnetJsonSerializer.Deserialize<AuthorizeResult>(stringValue);
            return verifyResult ?? AuthorizeResult.ForbidResult();
        }

        /// <summary>
        /// Authorize
        /// </summary>
        /// <param name="authorizeOptions">Authorize options</param>
        /// <returns>Return the authorize reuslt</returns>
        public static AuthorizeResult Authorize(AuthorizeOptions authorizeOptions)
        {
            return AuthorizeAsync(authorizeOptions).Result;
        }

        /// <summary>
        /// Select a remote server
        /// </summary>
        /// <returns>Return remote server address</returns>
        static string SelectRemoteServer()
        {
            if (DataSelectionProvider == null || AuthorizationConfiguration == null)
            {
                return string.Empty;
            }
            return DataSelectionProvider.Get(AuthorizationConfiguration.ServerSelectMode);
        }
    }

    class TypeNameEqualityComparer : IEqualityComparer<Type>
    {
        public bool Equals(Type x, Type y)
        {
            return x?.FullName == y?.FullName;
        }

        public int GetHashCode([DisallowNull] Type obj)
        {
            return obj?.GetHashCode() ?? 0;
        }
    }
}
