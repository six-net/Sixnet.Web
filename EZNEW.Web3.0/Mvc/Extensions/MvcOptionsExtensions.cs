using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EZNEW.Web.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Microsoft.AspNetCore.Mvc
{
    public static class MvcOptionsExtensions
    {
        /// <summary>
        /// Use global route prefix
        /// </summary>
        /// <param name="options">Mvc options</param>
        /// <param name="routeAttribute">Route attribute</param>
        /// <param name="onlyApi">Whether only api controller</param>
        public static void UseGlobalRoutePrefix(this MvcOptions options, IRouteTemplateProvider routeAttribute, bool onlyApi = true)
        {
            // Add our custom OuteConvention to implement the IApplication Model Convention
            options.Conventions.Insert(0, new GlobalRouteConvention(routeAttribute, onlyApi));
        }
    }
}
