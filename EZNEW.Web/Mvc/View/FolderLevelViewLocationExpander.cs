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
        Dictionary<string, IEnumerable<string>> ViewLocationDict = null;
        static List<string> DefaultViewLocations = new List<string>()
        {
            "/Views/{1}/{0}.cshtml",
            "/Views/Shared/{0}.cshtml",
            "/Pages/Shared/{0}.cshtml"
        };

        /// <summary>
        /// create view location expander
        /// </summary>
        /// <param name="assignableFromController">assignable from controller type</param>
        public FolderLevelViewLocationExpander(Type assignableFromController)
        {
            if (assignableFromController == null)
            {
                throw new ArgumentNullException(nameof(assignableFromController));
            }
            var allControllerList = assignableFromController.Assembly.GetTypes().Where(c => assignableFromController.IsAssignableFrom(c)).ToList();
            ViewLocationDict = new Dictionary<string, IEnumerable<string>>(allControllerList.Count);
            foreach (var c in allControllerList)
            {
                var controllerNamespace = c.Namespace ?? string.Empty;
                if (controllerNamespace.EndsWith("Controllers"))
                {
                    continue;
                }
                var namespaceArray = controllerNamespace.LSplit("Controllers");
                var groupName = string.Join("/", namespaceArray[namespaceArray.Length - 1].LSplit("."));
                ViewLocationDict[c.Name.LSplit("Controller")[0]] = DefaultViewLocations.Union(new List<string>(2)
                {
                    $"/Views/{groupName.Trim('.')}/{{1}}/{{0}}.cshtml",
                    $"/Views/{groupName.Trim('.')}/{{0}}.cshtml"
                });
            }
        }

        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            var controllerName = context.ControllerName;
            if (ViewLocationDict.ContainsKey(controllerName))
            {
                return ViewLocationDict[controllerName];
            }
            return viewLocations;
        }

        public void PopulateValues(ViewLocationExpanderContext context)
        {
        }
    }
}
