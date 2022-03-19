using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Routing;

namespace EZNEW.Web.Mvc.ApplicationModels
{
    public class GlobalRouteConvention : IApplicationModelConvention
    {
        /// <summary>
        /// Define a routing prefix variable
        /// </summary>
        private readonly AttributeRouteModel _globalPrefix;

        /// <summary>
        /// Input the specified routing prefix when invoked
        /// </summary>
        /// <param name="routeTemplateProvider"></param>
        public GlobalRouteConvention(IRouteTemplateProvider routeTemplateProvider)
        {
            _globalPrefix = new AttributeRouteModel(routeTemplateProvider);
        }

        // Apply Method of Interface
        public void Apply(ApplicationModel application)
        {
            // Traversing through all Controllers
            foreach (var controller in application.Controllers)
            {
                // 1. Controller that has marked RouteAttribute
                // It is important to note that if a routing has been marked in the controller, the specified routing content will be added before the routing.

                var matchedSelectors = controller.Selectors.Where(x => x.AttributeRouteModel != null).ToList();
                if (matchedSelectors.Any())
                {
                    foreach (var selectorModel in matchedSelectors)
                    {
                        // Add another routing prefix to the current routing
                        selectorModel.AttributeRouteModel = AttributeRouteModel.CombineAttributeRouteModel(_globalPrefix,
                          selectorModel.AttributeRouteModel);
                    }
                }

                // 2. Controller without RouteAttribute tag
                var unmatchedSelectors = controller.Selectors.Where(x => x.AttributeRouteModel == null).ToList();
                if (unmatchedSelectors.Any())
                {
                    foreach (var selectorModel in unmatchedSelectors)
                    {
                        // Add a routing prefix
                        selectorModel.AttributeRouteModel = _globalPrefix;
                    }
                }
            }
        }
    }
}
