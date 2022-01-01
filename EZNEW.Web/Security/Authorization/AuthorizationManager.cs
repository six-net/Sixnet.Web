using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;
using EZNEW.Configuration;
using EZNEW.DependencyInjection;
using EZNEW.Selection;
using EZNEW.Http;
using EZNEW.Application;
using EZNEW.Logging;
using System.Reflection;
using EZNEW.Serialization;

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

        /// <summary>
        /// Whether ingore authentication
        /// </summary>
        public static bool IngoreAuthentication = false;

        /// <summary>
        /// Whether ingore default authorize
        /// </summary>
        public static bool IngoreDefaultAuthorize = false;

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
        /// Authorize proxy
        /// </summary>
        static Func<AuthorizeOptions, AuthorizeResult> AuthorizeProxy;

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
                if (AuthorizeProxy == null)
                {
                    throw new ArgumentNullException(nameof(AuthorizeProxy));
                }
                return AuthorizeProxy(authorizeOptions) ?? AuthorizeResult.ForbidResult();
            }
            string server = SelectRemoteServer();
            if (string.IsNullOrWhiteSpace(server))
            {
                throw new ArgumentNullException(nameof(AuthorizationConfiguration.Servers));
            }
            var result = await HttpHelper.PostJsonAsync(server, authorizeOptions).ConfigureAwait(false);
            var stringValue = await result.Content.ReadAsStringAsync().ConfigureAwait(false);
            AuthorizeResult verifyResult = JsonSerializer.Deserialize<AuthorizeResult>(stringValue);
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
            return DataSelectionProvider.Get(AuthorizationConfiguration.ServerSelectPattern);
        }

        /// <summary>
        /// Resolve default authorizations
        /// </summary>
        /// <returns>Return the default authorizations</returns>
        public static List<AuthorizationGroupInfo> ResolveDefaultAuthorizations(params string[] files)
        {
            List<AuthorizationGroupInfo> operationGroups = new List<AuthorizationGroupInfo>();
            var controllerBaseType = typeof(ControllerBase);
            IEnumerable<Type> types = Assembly.GetEntryAssembly().GetTypes();
            var comparer = new TypeNameEqualityComparer();
            foreach (var file in files)
            {
                types = types.Union(Assembly.LoadFrom(file).GetTypes(), comparer);
            }
            foreach (var type in types)
            {
                if (!controllerBaseType.IsAssignableFrom(type))
                {
                    continue;
                }
                var operationGroupAttr = type.GetCustomAttribute<AuthorizationGroupAttribute>(false);
                if (string.IsNullOrWhiteSpace(operationGroupAttr?.Name))
                {
                    continue;
                }
                var areaAttr = type.GetCustomAttribute<AreaAttribute>(true);
                string areName = areaAttr?.RouteKey ?? string.Empty;
                AuthorizationGroupInfo operationGroup = operationGroups.FirstOrDefault(c => c.Name == operationGroupAttr.Name) ?? new AuthorizationGroupInfo()
                {
                    Name = operationGroupAttr.Name,
                };
                operationGroup.Actions ??= new List<AuthorizationActionInfo>();
                var actions = type.GetMethods();
                foreach (var action in actions)
                {
                    var operationAttrs = action.GetCustomAttributes<AuthorizationActionAttribute>(false);
                    if (operationAttrs.IsNullOrEmpty())
                    {
                        continue;
                    }
                    var firstOperationAttr = operationAttrs.First();
                    operationGroup.Actions.Add(new AuthorizationActionInfo()
                    {
                        Name = firstOperationAttr.Name,
                        Action = action.Name,
                        Area = areName,
                        Controller = type.Name.LSplit("Controller")[0],
                        Public = firstOperationAttr.Public
                    });
                }
                AuthorizationGroupInfo parentGroup = null;
                if (!string.IsNullOrWhiteSpace(operationGroupAttr.Parent))
                {
                    parentGroup = operationGroups.FirstOrDefault(c => c.Name == operationGroupAttr.Parent);
                    if (parentGroup == null)
                    {
                        parentGroup = new AuthorizationGroupInfo()
                        {
                            Name = operationGroupAttr.Parent,
                            ChildGroups = new List<AuthorizationGroupInfo>()
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
            return operationGroups;
        }
    }

    class TypeNameEqualityComparer : IEqualityComparer<Type>
    {
        public bool Equals(Type x, Type y)
        {
            return x?.FullName == y?.FullName;
        }

        public int GetHashCode(Type obj)
        {
            return obj?.GetHashCode() ?? 0;
        }
    }
}
