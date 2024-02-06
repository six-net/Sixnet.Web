using Sixnet.Web.Mvc;

namespace Microsoft.AspNetCore.Routing
{
    public static class RouteExtensions
    {
        /// <summary>
        /// Get the area name
        /// </summary>
        /// <param name="values">Route values</param>
        /// <returns>Return the area name</returns>
        public static string GetAreaName(this RouteValueDictionary values)
        {
            object area = null;
            values?.TryGetValue(MvcConstants.Route.Area, out area);
            return area?.ToString() ?? string.Empty;
        }

        /// <summary>
        /// Get controller name
        /// </summary>
        /// <param name="values">Route values</param>
        /// <returns>Return the controller name</returns>
        public static string GetController(this RouteValueDictionary values)
        {
            object controller = null;
            values?.TryGetValue(MvcConstants.Route.Controller, out controller);
            return controller?.ToString() ?? string.Empty;
        }

        /// <summary>
        /// Get action name
        /// </summary>
        /// <param name="values">Route values</param>
        /// <returns>Return the action name</returns>
        public static string GetAction(this RouteValueDictionary values)
        {
            object action = null;
            values?.TryGetValue(MvcConstants.Route.Action, out action);
            return action?.ToString() ?? string.Empty;
        }
    }
}
