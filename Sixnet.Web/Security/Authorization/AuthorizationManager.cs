using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Sixnet.Algorithm.Selection;
using Sixnet.DependencyInjection;
using Sixnet.Net.Http;
using Sixnet.Serialization;

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
                if (!type.IsPublic || !controllerBaseType.IsAssignableFrom(type))
                {
                    continue;
                }
                var operationGroupAttr = type.GetCustomAttribute<AuthorizationGroupAttribute>(false) ?? new AuthorizationGroupAttribute()
                {
                    Name = type.Name,
                };
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
                var actions = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                foreach (var action in actions)
                {
                    if (!action.IsPublic)
                    {
                        continue;
                    }
                    var operationAttr = action.GetCustomAttribute<AuthorizationActionAttribute>(false) ?? new AuthorizationActionAttribute()
                    {
                        Name = action.Name,
                        Group = operationGroupAttr.Name,
                        Public = false
                    };
                    operationGroup.Actions.Add(new AuthorizationActionInfo()
                    {
                        Name = operationAttr.Name,
                        Action = action.Name,
                        Area = areName,
                        Controller = type.Name.LSplit("Controller")[0],
                        Public = operationAttr.Public
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

        public int GetHashCode([DisallowNull] Type obj)
        {
            return obj?.GetHashCode() ?? 0;
        }
    }
}
