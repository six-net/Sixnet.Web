using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Sixnet.Web.Mvc.ApplicationModels
{
    public class GlobalRouteConvention : IApplicationModelConvention
    {
        /// <summary>
        /// Define a routing prefix variable
        /// </summary>
        private readonly AttributeRouteModel _globalPrefix;

        private readonly bool onlyApi = true;

        /// <summary>
        /// Input the specified routing prefix when invoked
        /// </summary>
        /// <param name="routeTemplateProvider"></param>
        public GlobalRouteConvention(IRouteTemplateProvider routeTemplateProvider, bool onlyApi = true)
        {
            _globalPrefix = new AttributeRouteModel(routeTemplateProvider);
            this.onlyApi = onlyApi;
        }

        // Apply Method of Interface
        public void Apply(ApplicationModel application)
        {
            // Traversing through all Controllers
            foreach (var controller in application.Controllers)
            {
                if (onlyApi && !IsApiController(controller.ControllerType))
                {
                    continue;
                }
                var controllerRouteModel = _globalPrefix;
                // 1. Area
                var areaAttribute = controller.Attributes?.FirstOrDefault(c => c.GetType() == typeof(AreaAttribute)) as AreaAttribute;
                if(areaAttribute!=null)
                {
                    controllerRouteModel = AttributeRouteModel.CombineAttributeRouteModel(new AttributeRouteModel(new RouteAttribute(areaAttribute.RouteValue)), controllerRouteModel);
                }

                // 2. Controller that has marked RouteAttribute
                // It is important to note that if a routing has been marked in the controller, the specified routing content will be added before the routing.

                var matchedSelectors = controller.Selectors.Where(x => x.AttributeRouteModel != null).ToList();
                if (matchedSelectors.Any())
                {
                    foreach (var selectorModel in matchedSelectors)
                    {
                        // Add another routing prefix to the current routing
                        selectorModel.AttributeRouteModel = AttributeRouteModel.CombineAttributeRouteModel(controllerRouteModel,
                          selectorModel.AttributeRouteModel);
                    }
                }

                // 3. Controller without RouteAttribute tag
                var unmatchedSelectors = controller.Selectors.Where(x => x.AttributeRouteModel == null).ToList();
                if (unmatchedSelectors.Any())
                {
                    foreach (var selectorModel in unmatchedSelectors)
                    {
                        // Add a routing prefix
                        selectorModel.AttributeRouteModel = controllerRouteModel;
                    }
                }
            }
        }

        static bool IsApiController(Type controllerType)
        {
            var apiAttributes = controllerType.GetCustomAttributes(true);
            return apiAttributes?.Any(a => a is ApiControllerAttribute) ?? false;
        }
    }
}
