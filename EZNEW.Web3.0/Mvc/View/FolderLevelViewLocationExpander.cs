using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Razor;

namespace EZNEW.Web.Mvc
{
    public class FolderLevelViewLocationExpander : IViewLocationExpander
    {
        //1:controller name
        //0:view name
        Dictionary<string, IEnumerable<string>> ViewLocations = null;
        static List<string> DefaultViewLocations = new List<string>()
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
            var allControllerList = assignableFromController.Assembly.GetTypes().Where(c => assignableFromController.IsAssignableFrom(c)).ToList();
            ViewLocations = new Dictionary<string, IEnumerable<string>>(allControllerList.Count);
            foreach (var c in allControllerList)
            {
                var controllerNamespace = c.Namespace ?? string.Empty;
                if (controllerNamespace.EndsWith("Controllers"))
                {
                    continue;
                }
                var groupName = string.Join('/', controllerNamespace.LSplit("Controllers")[^1].LSplit("."));
                ViewLocations[c.Name.LSplit("Controller")[0]] = DefaultViewLocations.Union(new List<string>(2)
                {
                    $"/Views/{groupName.Trim('.')}/{{1}}/{{0}}.cshtml",
                    $"/Views/{groupName.Trim('.')}/{{0}}.cshtml"
                });
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
            if (ViewLocations.ContainsKey(controllerName))
            {
                return ViewLocations[controllerName];
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
