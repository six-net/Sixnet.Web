using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Linq;

using Asp.Versioning;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;

using Sixnet.DependencyInjection;
using Sixnet.Web.Mvc.Routing;
using Sixnet.Web.Security.Authorization;

namespace Sixnet.Web.Mvc.Controllers
{
    /// <summary>
    /// Controller action descriptor
    /// </summary>
    public static class ControllerActionDescriptorHelper
    {
        public static List<ControllerActionInfo> GetControllerAuthActionInfos(Action<ControllerActionDescriptorOptions> configure = null)
        {
            var options = new ControllerActionDescriptorOptions();
            configure?.Invoke(options);
            return GetControllerAuthActionInfosCore(options, null);
        }

        public static List<ControllerActionInfo> GetControllerAuthActionInfos<TController>(Action<ControllerActionDescriptorOptions> configure = null)
        {
            var options = new ControllerActionDescriptorOptions();
            configure?.Invoke(options);
            return GetControllerAuthActionInfosCore(options, new List<Type>() { typeof(TController) });
        }

        static List<ControllerActionInfo> GetControllerAuthActionInfosCore(ControllerActionDescriptorOptions options, IEnumerable<Type> controllerTypes)
        {
            var actionDescriptors = SixnetContainer.GetService<IActionDescriptorCollectionProvider>().ActionDescriptors.Items;
            var controllerActionDescriptors = actionDescriptors.OfType<ControllerActionDescriptor>().ToList();
            var xmlComments = LoadXmlComments(options);

            var result = new List<ControllerActionInfo>();
            foreach (var descriptor in controllerActionDescriptors)
            {
                var controllerType = descriptor.ControllerTypeInfo;
                var actionMethod = descriptor.MethodInfo;

                if (!controllerTypes.IsNullOrEmpty() && !controllerTypes.Contains(controllerType))
                {
                    continue;
                }

                // super admin
                var superAttr = controllerType.GetCustomAttribute<SuperActionAttribute>()
                                ?? actionMethod.GetCustomAttribute<SuperActionAttribute>();
                if (superAttr != null)
                {
                    continue;
                }

                // anonymous
                var anonymousAttr = controllerType.GetCustomAttribute<AllowAnonymousAttribute>()
                                    ?? actionMethod.GetCustomAttribute<AllowAnonymousAttribute>();
                if (anonymousAttr != null && options.IgnoreAnonymous)
                {
                    continue;
                }

                // ignore authorize
                var ignoreAuth = controllerType.GetCustomAttribute<IgnoreAuthorizeAttribute>()
                                ?? actionMethod.GetCustomAttribute<IgnoreAuthorizeAttribute>();
                if (ignoreAuth != null && options.IgnoreNotAuth)
                {
                    continue;
                }

                var apiVersions = GetApiVersions(controllerType, actionMethod);
                var actionSummary = GetActionComment(xmlComments, actionMethod);
                var controllerSummary = GetControllerComment(xmlComments, descriptor.ControllerTypeInfo);

                result.Add(new ControllerActionInfo
                {
                    Namespace = descriptor.ControllerTypeInfo.Namespace,
                    ControllerName = descriptor.ControllerTypeInfo.Name,
                    ControllerSummary = controllerSummary,
                    ActionName = descriptor.MethodInfo.Name,
                    ActionSummary = actionSummary,
                    ApiVersions = apiVersions,
                    Route = GetRoute(descriptor, options),
                    FullName = GetControllerActionDescriptorFullName(descriptor)
                });
            }

            return result;
        }

        static List<string> GetApiVersions(TypeInfo controllerType, MethodInfo actionMethod)
        {
            var versions = new List<string>();
            var controllerVersions = controllerType.GetCustomAttributes<ApiVersionAttribute>()
                                                    .SelectMany(attr => attr.Versions)
                                                    .Select(v => v.ToString())
                                                    .ToList();

            var actionVersions = actionMethod.GetCustomAttributes<MapToApiVersionAttribute>()
                                             .SelectMany(attr => attr.Versions)
                                             .Select(v => v.ToString())
                                             .ToList();

            versions.AddRange(controllerVersions);
            versions.AddRange(actionVersions);
            return versions.Distinct().ToList();
        }

        static string GetActionComment(Dictionary<string, string> xmlComments, MethodInfo method)
        {
            if (xmlComments.IsNullOrEmpty() || method == null)
            {
                return string.Empty;
            }
            var memberKey = $"M:{method.DeclaringType.FullName}.{method.Name}";
            KeyValuePair<string, string>? commentItem = xmlComments.FirstOrDefault(c => c.Key.StartsWith(memberKey));
            return commentItem?.Value ?? "";
        }

        static string GetControllerComment(Dictionary<string, string> xmlComments, TypeInfo controller)
        {
            if (xmlComments.IsNullOrEmpty() || controller == null)
            {
                return string.Empty;
            }
            var memberKey = $"T:{controller.FullName}";
            return xmlComments.TryGetValue(memberKey, out var summary) ? summary : "";
        }

        static string GetRoute(ControllerActionDescriptor descriptor, ControllerActionDescriptorOptions options)
        {
            var controllerRouteTemplate = descriptor.ControllerTypeInfo
                .GetCustomAttributes()
                .OfType<RouteAttribute>()
                .Select(attr => attr.Template)?.FirstOrDefault() ?? string.Empty;

            var actionRouteTemplate = descriptor.MethodInfo
                        .GetCustomAttributes()
                        .OfType<RouteAttribute>()
                        .Select(attr => attr.Template)?.FirstOrDefault() ?? string.Empty;

            var controllerName = descriptor.ControllerName;
            var actionName = descriptor.ActionName;
            if (options.KebabCase)
            {
                var transformer = new KebabCaseParameterTransformer();
                controllerName = transformer.TransformOutbound(controllerName);
                actionName = transformer.TransformOutbound(actionName);
            }

            var route = $"{controllerRouteTemplate}/{actionRouteTemplate}".Trim('/');
            var controllerPattern = @"\[controller\]";
            route = Regex.Replace(route, controllerPattern, controllerName.ToLower());
            var actionPattern = @"\[action\]";
            route = Regex.Replace(route, actionPattern, actionName.ToLower());
            return route;
        }

        static Dictionary<string, string> LoadXmlComments(ControllerActionDescriptorOptions options)
        {
            var filePath = options.XmlCommentsFilePath;
            if (string.IsNullOrWhiteSpace(filePath))
            {
                filePath = Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetEntryAssembly().GetName().Name}.xml");
            }
            var xmlComments = new Dictionary<string, string>();
            if (!File.Exists(filePath))
            {
                return xmlComments;
            }
            var xdoc = XDocument.Load(filePath);
            var members = xdoc.Descendants("member");
            foreach (var member in members)
            {
                var name = member.Attribute("name")?.Value;
                var summary = member.Descendants("summary").FirstOrDefault()?.Value.Trim();
                if (name != null && summary != null)
                {
                    xmlComments[name] = summary;
                }
            }
            return xmlComments;
        }

        internal static string GetControllerActionDescriptorFullName(ControllerActionDescriptor descriptor)
        {
            return $"{descriptor.ControllerTypeInfo.FullName}.{descriptor.MethodInfo.Name}";
        }
    }

    public class ControllerActionDescriptorOptions
    {
        /// <summary>
        /// Comments file path
        /// </summary>
        public string XmlCommentsFilePath { get; set; }

        /// <summary>
        /// Kebab case
        /// </summary>
        public bool KebabCase { get; set; } = true;

        /// <summary>
        /// Ignore anonymous
        /// </summary>
        public bool IgnoreAnonymous { get; set; } = true;

        /// <summary>
        /// Ignore super action
        /// </summary>
        public bool IgnoreSuper { get; set; } = true;

        /// <summary>
        /// Ignore not auth
        /// </summary>
        public bool IgnoreNotAuth { get; set; } = true;
    }

    public class ControllerActionInfo
    {
        /// <summary>
        /// Namespace
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        /// Controller name
        /// </summary>
        public string ControllerName { get; set; }

        /// <summary>
        /// Action name
        /// </summary>
        public string ActionName { get; set; }

        /// <summary>
        /// Version
        /// </summary>
        public List<string> ApiVersions { get; set; }

        /// <summary>
        /// Summary
        /// </summary>
        public string ActionSummary { get; set; }

        /// <summary>
        /// Controller summary
        /// </summary>
        public string ControllerSummary { get; set; }

        /// <summary>
        /// Route
        /// </summary>
        public string Route { get; set; }

        /// <summary>
        /// Full name
        /// </summary>
        public string FullName { get; set; }
    }
}
