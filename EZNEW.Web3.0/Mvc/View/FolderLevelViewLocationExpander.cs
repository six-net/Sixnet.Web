using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;

namespace EZNEW.Web.Mvc
{
    public class FolderLevelViewLocationExpander : IViewLocationExpander
    {
        //1:controller name
        //0:view name
        readonly Dictionary<string, IEnumerable<string>> ViewLocationCollection = null;
        static readonly List<string> DefaultViewLocations = new List<string>()
        {
            "/Views/{1}/{0}.cshtml",
            "/Views/Shared/{0}.cshtml",
            "/Pages/Shared/{0}.cshtml"
        };

        /// <summary>
        /// Create view location expander
        /// </summary>
        /// <param name="assignableFromController">Assignable from controller type</param>
        public FolderLevelViewLocationExpander(Type assignableFromController)
        {
            if (assignableFromController == null)
            {
                throw new ArgumentNullException(nameof(assignableFromController));
            }
            var allControllers = assignableFromController.Assembly.GetTypes().Where(c => assignableFromController.IsAssignableFrom(c)).ToList();
            ViewLocationCollection = new Dictionary<string, IEnumerable<string>>(allControllers.Count);
            foreach (var controller in allControllers)
            {
                var areaAttribute = controller.GetCustomAttribute<AreaAttribute>();
                List<string> conttrollerViewLocations = new List<string>(7);
                if (areaAttribute != null)
                {
                    conttrollerViewLocations.Add("/Areas/{2}/Views/{1}/{0}.cshtml");
                    conttrollerViewLocations.Add("/Areas/{2}/Views/Shared/{0}.cshtml");
                }
                var controllerNamespace = controller.Namespace ?? string.Empty;
                var groupName = string.Empty;
                if (!controllerNamespace.EndsWith("Controllers"))
                {
                    groupName = string.Join('/', controllerNamespace.LSplit("Controllers")[^1].LSplit("."));
                }
                if (!string.IsNullOrWhiteSpace(groupName))
                {
                    if (areaAttribute != null)
                    {
                        conttrollerViewLocations.Add($"/Areas/{{2}}/Views/{groupName.Trim('.')}/{{1}}/{{0}}.cshtml");
                        conttrollerViewLocations.Add($"/Areas/{{2}}/Views/{groupName.Trim('.')}/{{0}}.cshtml");
                    }
                    conttrollerViewLocations.Add($"/Views/{groupName.Trim('.')}/{{1}}/{{0}}.cshtml");
                    conttrollerViewLocations.Add($"/Views/{groupName.Trim('.')}/{{0}}.cshtml");
                }
                if (!conttrollerViewLocations.IsNullOrEmpty())
                {
                    conttrollerViewLocations.AddRange(DefaultViewLocations);
                    ViewLocationCollection[controller.Name.LSplit("Controller")[0]] = conttrollerViewLocations;
                }
            }
        }

        /// <summary>
        /// Expand view locations
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="viewLocations">View locations</param>
        /// <returns>Return view locations</returns>
        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            var controllerName = context.ControllerName;
            if (ViewLocationCollection.ContainsKey(controllerName))
            {
                return ViewLocationCollection[controllerName];
            }
            return viewLocations;
        }

        /// <summary>
        /// Populate values
        /// </summary>
        /// <param name="context">context</param>
        public void PopulateValues(ViewLocationExpanderContext context)
        {
        }
    }
}
