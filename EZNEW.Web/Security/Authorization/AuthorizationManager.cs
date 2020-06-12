using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using EZNEW.DependencyInjection;
using EZNEW.Serialize;
using EZNEW.Selection;
using EZNEW.Http;

namespace EZNEW.Web.Security.Authorization
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
        static readonly DataSelectionProvider<string> DataSelectionProvider = null;

        static AuthorizationManager()
        {
            var authorizationConfiguration = ContainerManager.Resolve<IOptions<AuthorizationConfiguration>>()?.Value ?? new AuthorizationConfiguration();
            AuthorizationConfiguration = authorizationConfiguration;
            if ((!authorizationConfiguration?.Servers.IsNullOrEmpty()) ?? false)
            {
                DataSelectionProvider = new DataSelectionProvider<string>(authorizationConfiguration.Servers);
            }
        }

        /// <summary>
        /// Gets or sets verify authorization operation
        /// </summary>
        static Func<VerifyAuthorizationOption, VerifyAuthorizationResult> AuthorizationVerifyProxy;

        /// <summary>
        /// Configure authorization verify operation
        /// </summary>
        /// <param name="authorizationVerifyOperation">Authorization verify operation</param>
        public static void ConfigureAuthorizationVerify(Func<VerifyAuthorizationOption, VerifyAuthorizationResult> authorizationVerifyOperation)
        {
            AuthorizationVerifyProxy = authorizationVerifyOperation;
        }

        /// <summary>
        /// Verify authorization
        /// </summary>
        /// <param name="authorizationOption">Authorization option</param>
        /// <returns>Return verify authorization reuslt</returns>
        public static async Task<VerifyAuthorizationResult> VerifyAuthorizeAsync(VerifyAuthorizationOption authorizationOption)
        {
            if (authorizationOption == null)
            {
                return VerifyAuthorizationResult.ForbidResult();
            }
            if (!AuthorizationConfiguration.RemoteVerify)
            {
                if (AuthorizationVerifyProxy == null)
                {
                    throw new ArgumentNullException(nameof(AuthorizationVerifyProxy));
                }
                return AuthorizationVerifyProxy(authorizationOption) ?? VerifyAuthorizationResult.ForbidResult();
            }
            string server = SelectRemoteVerifyServer();
            if (string.IsNullOrWhiteSpace(server))
            {
                throw new ArgumentNullException(nameof(AuthorizationConfiguration.Servers));
            }
            var result = await HttpHelper.PostJsonAsync(server, authorizationOption).ConfigureAwait(false);
            var stringValue = await result.Content.ReadAsStringAsync().ConfigureAwait(false);
            VerifyAuthorizationResult verifyResult = JsonSerializeHelper.JsonToObject<VerifyAuthorizationResult>(stringValue);
            return verifyResult ?? VerifyAuthorizationResult.ForbidResult();
        }

        /// <summary>
        /// Verify authorize
        /// </summary>
        /// <param name="authorizationOption">Authorization option</param>
        /// <returns></returns>
        public static VerifyAuthorizationResult VerifyAuthorize(VerifyAuthorizationOption authorizationOption)
        {
            return VerifyAuthorizeAsync(authorizationOption).Result;
        }

        /// <summary>
        /// Select a remote verify server
        /// </summary>
        /// <returns>Return remote server address</returns>
        static string SelectRemoteVerifyServer()
        {
            if (DataSelectionProvider == null || AuthorizationConfiguration == null)
            {
                return string.Empty;
            }
            return DataSelectionProvider.Get(AuthorizationConfiguration.ServerSelectMode);
        }
    }
}
