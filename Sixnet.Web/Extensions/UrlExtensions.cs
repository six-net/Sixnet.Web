using System;
using System.Linq;
using Microsoft.AspNetCore.Routing;
using Sixnet.Web.Mvc;

namespace Microsoft.AspNetCore.Mvc
{
    /// <summary>
    /// Url Extensions
    /// </summary>
    public static class UrlExtensions
    {
        /// <summary>
        /// Copy route value
        /// </summary>
        /// <param name="routeValueDictionary">Route values</param>
        /// <returns>Return new route values</returns>
        public static RouteValueDictionary CopyRouteValueDictionary(RouteValueDictionary routeValueDictionary)
        {
            if (routeValueDictionary == null)
            {
                return new RouteValueDictionary();
            }
            var newValue = new RouteValueDictionary();
            foreach (var routeValueItem in routeValueDictionary)
            {
                newValue.Add(routeValueItem.Key.ToNullableString(), routeValueItem.Value);
            }
            return newValue;
        }

        /// <summary>
        /// Update or add parameters
        /// </summary>
        /// <param name="urlHelper">Url helper</param>
        /// <param name="parameterName">Parameter name</param>
        /// <param name="newValue">New value</param>
        /// <returns>Return new url</returns>
        public static string AlterParameterValue(this IUrlHelper urlHelper, string parameterName, string newValue)
        {
            //get route values
            var routeValueDictionary = urlHelper.ActionContext.RouteData.Values;
            routeValueDictionary = CopyRouteValueDictionary(routeValueDictionary);

            //get query string
            var collection = urlHelper.ActionContext.HttpContext.Request.Query;
            var allQueryKeys = collection.Keys?.ToArray();
            if (routeValueDictionary.ContainsKey(parameterName))
            {
                routeValueDictionary[parameterName] = newValue;
            }

            foreach (string queryParameter in allQueryKeys)
            {
                if (queryParameter == parameterName)
                {
                    routeValueDictionary.Add(queryParameter, newValue);
                }
                else
                {
                    routeValueDictionary.Add(queryParameter, collection[queryParameter].ToNullableString());
                }
            }

            if (!routeValueDictionary.ContainsKey(parameterName) && !string.IsNullOrEmpty(parameterName))
            {
                routeValueDictionary.Add(parameterName, newValue);
            }
            return urlHelper.Action(routeValueDictionary[MvcConstants.Route.Action].ToNullableString(), routeValueDictionary[MvcConstants.Route.Controller].ToNullableString(), routeValueDictionary);
        }

        /// <summary>
        /// Update or add parameters,then redirect to other action
        /// </summary>
        /// <param name="urlHelper">Url helper</param>
        /// <param name="action">Action</param>
        /// <param name="controller">Controller</param>
        /// <param name="parameterName">Parameter name</param>
        /// <param name="newValue">New value</param>
        /// <returns>Return new url</returns>
        public static string AlterParameterValue(this IUrlHelper urlHelper, string action, string controller, string parameterName, string newValue)
        {
            var routeValueDictionary = urlHelper.ActionContext.RouteData.Values;
            routeValueDictionary = CopyRouteValueDictionary(routeValueDictionary);
            var collection = urlHelper.ActionContext.HttpContext.Request.Query;
            var allQueryKeys = collection.Keys.ToArray();
            if (routeValueDictionary.ContainsKey(parameterName))
            {
                routeValueDictionary[parameterName] = newValue;
            }
            if (!string.IsNullOrEmpty(action))
            {
                routeValueDictionary[MvcConstants.Route.Action] = action;
            }
            if (!string.IsNullOrEmpty(controller))
            {
                routeValueDictionary[MvcConstants.Route.Controller] = controller;
            }
            foreach (string queryKey in allQueryKeys)
            {
                if (queryKey == parameterName)
                {
                    routeValueDictionary.Add(queryKey, parameterName);
                }
                else
                {
                    routeValueDictionary.Add(queryKey, collection[queryKey].ToNullableString());
                }
            }
            if (!routeValueDictionary.ContainsKey(parameterName) && !string.IsNullOrEmpty(parameterName))
            {
                routeValueDictionary.Add(parameterName, newValue);
            }
            return urlHelper.Action(routeValueDictionary[MvcConstants.Route.Action].ToNullableString(), routeValueDictionary[MvcConstants.Route.Controller].ToNullableString(), routeValueDictionary);
        }

        /// <summary>
        /// Redirect to other action or controller
        /// </summary>
        /// <param name="urlHelper">Url helper</param>
        /// <param name="action">Action</param>
        /// <param name="controller">Controller</param>
        /// <returns>Return new url</returns>
        public static string RedirectToAction(this IUrlHelper urlHelper, string action, string controller)
        {
            var routeValueDictionary = urlHelper.ActionContext.RouteData.Values;
            routeValueDictionary = CopyRouteValueDictionary(routeValueDictionary);
            var collection = urlHelper.ActionContext.HttpContext.Request.Query;
            var allQueryKeys = collection.Keys.ToArray();
            if (!string.IsNullOrEmpty(action))
            {
                routeValueDictionary[MvcConstants.Route.Action] = action;
            }
            if (!string.IsNullOrEmpty(controller))
            {
                routeValueDictionary[MvcConstants.Route.Controller] = action;
            }
            foreach (string queryKey in allQueryKeys)
            {
                routeValueDictionary.Add(queryKey, collection[queryKey].ToNullableString());
            }
            return urlHelper.Action(routeValueDictionary[MvcConstants.Route.Action].ToNullableString(), routeValueDictionary[MvcConstants.Route.Controller].ToNullableString(), routeValueDictionary);
        }

        /// <summary>
        /// Update and delete parameter
        /// </summary>
        /// <param name="urlHelper">Url helper</param>
        /// <param name="alterParameterName">Alter parameter name</param>
        /// <param name="newValue">New value</param>
        /// <param name="deleteParameterNames">Delete parameter name</param>
        /// <returns>Return new url</returns>
        public static string AlterAndDeleteParameter(this IUrlHelper urlHelper, string alterParameterName, string newValue, params string[] deleteParameterNames)
        {
            var routeValueDictionary = urlHelper.ActionContext.RouteData.Values;
            routeValueDictionary = CopyRouteValueDictionary(routeValueDictionary);
            var collection = urlHelper.ActionContext.HttpContext.Request.Query;
            var allQueryKeys = collection.Keys.ToArray();
            if (routeValueDictionary.ContainsKey(alterParameterName))
            {
                routeValueDictionary[alterParameterName] = newValue;
            }
            foreach (string queryKey in allQueryKeys)
            {
                if (queryKey == alterParameterName)
                {
                    routeValueDictionary.Add(queryKey, newValue);
                }
                else
                {
                    routeValueDictionary.Add(queryKey, collection[queryKey].ToNullableString());
                }
            }
            if (deleteParameterNames != null && deleteParameterNames.Length > 0)
            {
                foreach (string deleteKey in deleteParameterNames)
                {
                    if (routeValueDictionary.ContainsKey(deleteKey))
                    {
                        routeValueDictionary.Remove(deleteKey);
                    }
                }
            }
            return urlHelper.Action(routeValueDictionary[MvcConstants.Route.Action].ToNullableString(), routeValueDictionary[MvcConstants.Route.Controller].ToNullableString(), routeValueDictionary);
        }

        /// <summary>
        /// Update and delete parameter,then redirect
        /// </summary>
        /// <param name="urlHelper">Url helper</param>
        /// <param name="action">Action</param>
        /// <param name="controller">Controller</param>
        /// <param name="alterParameterName">Alter parameter name</param>
        /// <param name="newValue">New value</param>
        /// <param name="deleteParameterNames">Delete parameter names</param>
        /// <returns>Return new url</returns>
        public static string AlterAndDeleteParameter(this IUrlHelper urlHelper, string action, string controller, string alterParameterName, string newValue, params string[] deleteParameterNames)
        {
            var routeValueDictionary = urlHelper.ActionContext.RouteData.Values;
            routeValueDictionary = CopyRouteValueDictionary(routeValueDictionary);
            var collection = urlHelper.ActionContext.HttpContext.Request.Query;
            var allQueryKeys = collection.Keys.ToArray();
            if (routeValueDictionary.ContainsKey(alterParameterName))
            {
                routeValueDictionary[alterParameterName] = newValue;
            }
            if (!string.IsNullOrEmpty(action))
            {
                routeValueDictionary[MvcConstants.Route.Action] = action;
            }
            if (!string.IsNullOrEmpty(controller))
            {
                routeValueDictionary[MvcConstants.Route.Controller] = controller;
            }
            foreach (string queryKey in allQueryKeys)
            {
                if (queryKey == alterParameterName)
                {
                    routeValueDictionary.Add(queryKey, newValue);
                }
                else
                {
                    routeValueDictionary.Add(queryKey, collection[queryKey].ToNullableString());
                }
            }
            if (!routeValueDictionary.ContainsKey(alterParameterName) && !string.IsNullOrEmpty(alterParameterName))
            {
                routeValueDictionary.Add(alterParameterName, newValue);
            }

            if (deleteParameterNames != null && deleteParameterNames.Length > 0)
            {
                foreach (string deleteKey in deleteParameterNames)
                {
                    if (routeValueDictionary.ContainsKey(deleteKey))
                    {
                        routeValueDictionary.Remove(deleteKey);
                    }
                }
            }
            return urlHelper.Action(routeValueDictionary[MvcConstants.Route.Action].ToNullableString(), routeValueDictionary[MvcConstants.Route.Controller].ToNullableString(), routeValueDictionary);
        }

        /// <summary>
        /// Add and delete parameter value
        /// </summary>
        /// <param name="urlHelper">Url helper</param>
        /// <param name="parameterName">Parameter name</param>
        /// <param name="addValue">Add value</param>
        /// <param name="deleteValue">Delete value</param>
        /// <returns>Return new url</returns>
        public static string AddAndDeleteParameterValue(this IUrlHelper urlHelper, string parameterName, string addValue, string deleteValue)
        {
            var routeValueDictionary = urlHelper.ActionContext.RouteData.Values;
            routeValueDictionary = CopyRouteValueDictionary(routeValueDictionary);
            var collection = urlHelper.ActionContext.HttpContext.Request.Query;
            var allQueryKeys = collection.Keys.ToArray();
            foreach (string qk in allQueryKeys)
            {
                routeValueDictionary.Add(qk, collection[qk].ToNullableString());
            }
            if (routeValueDictionary.ContainsKey(parameterName))
            {
                string nowValue = routeValueDictionary[parameterName].ToNullableString();
                if (!string.IsNullOrWhiteSpace(deleteValue))
                {
                    nowValue = nowValue.Replace(deleteValue, string.Empty);
                }
                if (nowValue.IndexOf(addValue) < 0)
                {
                    nowValue += addValue;
                }
                routeValueDictionary[parameterName] = nowValue;
            }
            else
            {
                routeValueDictionary.Add(parameterName, addValue);
            }
            return urlHelper.Action(routeValueDictionary[MvcConstants.Route.Action].ToNullableString(), routeValueDictionary[MvcConstants.Route.Controller].ToNullableString(), routeValueDictionary);
        }

        /// <summary>
        /// Add and delete parameter value
        /// </summary>
        /// <param name="urlHelper">Url helper</param>
        /// <param name="parameterName">Parameter name</param>
        /// <param name="addValue">Add value</param>
        /// <param name="deleteValue">Delete value</param>
        /// <returns>Return new url</returns>
        public static string AddAndDeleteParameterValue(this IUrlHelper urlHelper, string action, string controller, string parameterName, string addValue, string deleteValue)
        {
            var routeValueDictionary = urlHelper.ActionContext.RouteData.Values;
            routeValueDictionary = CopyRouteValueDictionary(routeValueDictionary);
            var collection = urlHelper.ActionContext.HttpContext.Request.Query;
            var allQueryKeys = collection.Keys.ToArray();
            foreach (string queryKey in allQueryKeys)
            {
                routeValueDictionary.Add(queryKey, collection[queryKey].ToNullableString());
            }
            if (!string.IsNullOrEmpty(action))
            {
                routeValueDictionary[MvcConstants.Route.Action] = action;
            }
            if (!string.IsNullOrEmpty(controller))
            {
                routeValueDictionary[MvcConstants.Route.Controller] = controller;
            }
            string nowValue = string.Empty;
            if (routeValueDictionary.ContainsKey(parameterName))
            {
                nowValue = routeValueDictionary[parameterName].ToNullableString();
            }
            if (!string.IsNullOrEmpty(nowValue))
            {
                nowValue = nowValue.Replace(deleteValue, string.Empty);
                if (nowValue.IndexOf(addValue) < 0)
                {
                    nowValue += addValue;
                }
            }
            routeValueDictionary[parameterName] = nowValue;
            var newUrl = urlHelper.Action(routeValueDictionary[MvcConstants.Route.Action].ToNullableString(), routeValueDictionary[MvcConstants.Route.Controller].ToNullableString(), routeValueDictionary);
            newUrl = newUrl.Trim(new char[] { '&', '?' });
            return newUrl;
        }

        /// <summary>
        /// Get parameter value
        /// </summary>
        /// <param name="urlHelper">Url helper</param>
        /// <param name="parameterName">Parameter name</param>
        /// <returns>Return parameter value</returns>
        public static string ParameterValue(this IUrlHelper urlHelper, string parameterName)
        {
            var routeValueDictionary = urlHelper.ActionContext.RouteData.Values;
            var collection = urlHelper.ActionContext.HttpContext.Request.Query;
            if (routeValueDictionary.ContainsKey(parameterName))
            {
                return routeValueDictionary[parameterName].ToNullableString();
            }
            else if (collection.Keys.Contains(parameterName))
            {
                return collection[parameterName].ToNullableString();
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Delete parameter
        /// </summary>
        /// <param name="urlHelper">Url helper</param>
        /// <param name="parameterName">Parameter name</param>
        /// <returns>Return new url</returns>
        public static string DeleteParameter(this IUrlHelper urlHelper, string parameterName)
        {
            var routeValueDictionary = urlHelper.ActionContext.RouteData.Values;
            routeValueDictionary = CopyRouteValueDictionary(routeValueDictionary);
            var collection = urlHelper.ActionContext.HttpContext.Request.Query;
            var allQueryKeys = collection.Keys.ToArray();
            foreach (string queryKey in allQueryKeys)
            {
                routeValueDictionary.Add(queryKey, collection[queryKey].ToNullableString());
            }
            if (routeValueDictionary.Keys.Contains(parameterName))
            {
                routeValueDictionary.Remove(parameterName);
            }
            return urlHelper.Action(routeValueDictionary[MvcConstants.Route.Action].ToNullableString(), routeValueDictionary[MvcConstants.Route.Controller].ToNullableString(), routeValueDictionary);
        }
    }
}
