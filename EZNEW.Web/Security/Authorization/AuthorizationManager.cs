using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Reflection;
using System.Linq;
using System.IO;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;
using EZNEW.DependencyInjection;
using EZNEW.Serialize;
using EZNEW.Selection;
using EZNEW.Http;
using EZNEW.Logging;
using EZNEW.Application;
using EZNEW.Configuration;

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

        /// <summary>
        /// Resolve default operation
        /// </summary>
        /// <returns>Return default operations</returns>
        public static List<AuthorizationOperationGroupInfo> ResolveDefaultOperation()
        {
            List<AuthorizationOperationGroupInfo> operationGroups = new List<AuthorizationOperationGroupInfo>();
            try
            {
                var files = new DirectoryInfo(ApplicationManager.ApplicationExecutableDirectory).GetFiles("*.dll", SearchOption.AllDirectories)
    .Where(c => !ConfigurationOptions.ConfigurationExcludeFileRegex.IsMatch(c.FullName));
                var controllerBaseType = typeof(ControllerBase);
                foreach (var file in files)
                {
                    foreach (var type in Assembly.LoadFrom(file.FullName).GetTypes())
                    {
                        if (!controllerBaseType.IsAssignableFrom(type))
                        {
                            continue;
                        }
                        var operationGroupAttrs = type.GetCustomAttributes<AuthorizationOperationGroupAttribute>(false);
                        if (operationGroupAttrs.IsNullOrEmpty())
                        {
                            continue;
                        }
                        var firstGroupAttr = operationGroupAttrs.First();
                        if (string.IsNullOrWhiteSpace(firstGroupAttr.Name))
                        {
                            continue;
                        }
                        AuthorizationOperationGroupInfo operationGroup = operationGroups.FirstOrDefault(c => c.Name == firstGroupAttr.Name) ?? new AuthorizationOperationGroupInfo()
                        {
                            Name = firstGroupAttr.Name,
                        };
                        operationGroup.Operations ??= new List<AuthorizationOperationInfo>();
                        var actions = type.GetMethods();
                        foreach (var action in actions)
                        {
                            var operationAttrs = action.GetCustomAttributes<AuthorizationOperationAttribute>(false);
                            if (operationAttrs.IsNullOrEmpty())
                            {
                                continue;
                            }
                            var firstOperationAttr = operationAttrs.First();
                            operationGroup.Operations.Add(new AuthorizationOperationInfo()
                            {
                                Name = firstOperationAttr.Name,
                                ActionCode = action.Name,
                                ControllerCode = type.Name.LSplit("Controller")[0],
                                Public = firstOperationAttr.Public
                            });
                        }
                        AuthorizationOperationGroupInfo parentGroup = null;
                        if (!string.IsNullOrWhiteSpace(firstGroupAttr.Parent))
                        {
                            parentGroup = operationGroups.FirstOrDefault(c => c.Name == firstGroupAttr.Parent);
                            if (parentGroup == null)
                            {
                                parentGroup = new AuthorizationOperationGroupInfo()
                                {
                                    Name = firstGroupAttr.Parent,
                                    ChildGroups = new List<AuthorizationOperationGroupInfo>()
                                };
                                operationGroups.Add(parentGroup);
                            }
                        }
                        if (parentGroup != null)
                        {
                            parentGroup.ChildGroups.Add(operationGroup);
                        }
                        else
                        {
                            operationGroups.Add(operationGroup);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.LogError(nameof(AuthorizationManager), ex, ex.Message);
            }
            return operationGroups;
        }
    }
}
