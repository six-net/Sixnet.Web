using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace EZNEW.Web.Mvc
{
    /// <summary>
    /// Url Extensions
    /// </summary>
    public static class UrlExtensions
    {
        #region modify url parameters

        /// <summary>
        /// copy route value
        /// </summary>
        /// <param name="routeValueDictionary">route values</param>
        /// <returns>new route values</returns>
        public static RouteValueDictionary CopyRouteValueDictionary(RouteValueDictionary routeValueDictionary)
        {
            if (routeValueDictionary == null)
            {
                return new RouteValueDictionary();
            }
            RouteValueDictionary newValue = new RouteValueDictionary();
            foreach (KeyValuePair<string, object> rvdItem in routeValueDictionary)
            {
                newValue.Add(rvdItem.Key.ToNullableString(), rvdItem.Value);
            }
            return newValue;
        }

        /// <summary>
        /// update or add parameters
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="parameterName">parameter name</param>
        /// <param name="newValue">value</param>
        /// <returns>get new url</returns>
        public static string AlterParameterValue(this IUrlHelper url, string parameterName, string newValue)
        {
            string newUrl = string.Empty;
            //get route values
            RouteValueDictionary routeValueDictionary = url.ActionContext.RouteData.Values;
            routeValueDictionary = CopyRouteValueDictionary(routeValueDictionary);

            //get query string
            var collection = url.ActionContext.HttpContext.Request.Query;
            string[] allQueryKeys = collection.Keys?.ToArray();
            if (routeValueDictionary.Keys.Contains(parameterName))
            {
                routeValueDictionary[parameterName] = newValue;
            }

            foreach (string qk in allQueryKeys)
            {
                if (qk == parameterName)
                {
                    routeValueDictionary.Add(qk, newValue);
                }
                else
                {
                    routeValueDictionary.Add(qk, collection[qk].ToNullableString());
                }
            }

            if (!routeValueDictionary.ContainsKey(parameterName) && !string.IsNullOrEmpty(parameterName))
            {
                routeValueDictionary.Add(parameterName, newValue);
            }
            newUrl = url.Action(routeValueDictionary["action"].ToNullableString(), routeValueDictionary["controller"].ToNullableString(), routeValueDictionary);
            return newUrl;
        }

        /// <summary>
        /// update or add parameters,then redirect to other action
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="action">action</param>
        /// <param name="controller">controller</param>
        /// <param name="parameterName">parameter name</param>
        /// <param name="newValue">value</param>
        /// <returns>new url</returns>
        public static string AlterParameterValue(this IUrlHelper url, string action, string controller, string parameterName, string newValue)
        {
            string newUrl = string.Empty;
            RouteValueDictionary routeValueDictionary = url.ActionContext.RouteData.Values;
            routeValueDictionary = CopyRouteValueDictionary(routeValueDictionary);
            var collection = url.ActionContext.HttpContext.Request.Query;
            string[] allQueryKeys = collection.Keys.ToArray();
            if (routeValueDictionary.Keys.Contains(parameterName))
            {
                routeValueDictionary[parameterName] = newValue;
            }
            if (!string.IsNullOrEmpty(action))
            {
                routeValueDictionary["action"] = action;
            }
            if (!string.IsNullOrEmpty(controller))
            {
                routeValueDictionary["controller"] = controller;
            }
            foreach (string qk in allQueryKeys)
            {
                if (qk == parameterName)
                {
                    routeValueDictionary.Add(qk, parameterName);
                }
                else
                {
                    routeValueDictionary.Add(qk, collection[qk].ToNullableString());
                }
            }
            if (!routeValueDictionary.ContainsKey(parameterName) && !string.IsNullOrEmpty(parameterName))
            {
                routeValueDictionary.Add(parameterName, newValue);
            }
            newUrl = url.Action(routeValueDictionary["action"].ToNullableString(), routeValueDictionary["controller"].ToNullableString(), routeValueDictionary);
            return newUrl;
        }

        /// <summary>
        /// redirect to other action or controller
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="action">action</param>
        /// <param name="controller">controller</param>
        /// <returns>new url</returns>
        public static string SaveParameterAction(this IUrlHelper url, string action, string controller)
        {
            string newUrl = string.Empty;
            RouteValueDictionary routeValueDictionary = url.ActionContext.RouteData.Values;
            routeValueDictionary = CopyRouteValueDictionary(routeValueDictionary);
            var collection = url.ActionContext.HttpContext.Request.Query;
            string[] allQueryKeys = collection.Keys.ToArray();
            if (!string.IsNullOrEmpty(action))
            {
                routeValueDictionary["action"] = action;
            }
            if (!string.IsNullOrEmpty(controller))
            {
                routeValueDictionary["controller"] = action;
            }
            foreach (string qk in allQueryKeys)
            {
                routeValueDictionary.Add(qk, collection[qk].ToNullableString());
            }
            newUrl = url.Action(routeValueDictionary["action"].ToNullableString(), routeValueDictionary["controller"].ToNullableString(), routeValueDictionary);
            return newUrl;
        }

        /// <summary>
        /// update and delete parameter
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="alterParameterName">alter parameter name</param>
        /// <param name="newValue">value</param>
        /// <param name="deleteParameterNames">delete parameter name</param>
        /// <returns>new url</returns>
        public static string AlterAndDeleteParameter(this IUrlHelper url, string alterParameterName, string newValue, params string[] deleteParameterNames)
        {
            string newUrl = string.Empty;
            RouteValueDictionary routeValueDictionary = url.ActionContext.RouteData.Values;
            routeValueDictionary = CopyRouteValueDictionary(routeValueDictionary);
            var collection = url.ActionContext.HttpContext.Request.Query;
            string[] allQueryKeys = collection.Keys.ToArray();
            if (routeValueDictionary.Keys.Contains(alterParameterName))
            {
                routeValueDictionary[alterParameterName] = newValue;
            }
            foreach (string qk in allQueryKeys)
            {
                if (qk == alterParameterName)
                {
                    routeValueDictionary.Add(qk, newValue);
                }
                else
                {
                    routeValueDictionary.Add(qk, collection[qk].ToNullableString());
                }
            }
            if (deleteParameterNames != null && deleteParameterNames.Length > 0)
            {
                foreach (string deleteKey in deleteParameterNames)
                {
                    if (routeValueDictionary.Keys.Contains(deleteKey))
                    {
                        routeValueDictionary.Remove(deleteKey);
                    }
                }
            }
            newUrl = url.Action(routeValueDictionary["action"].ToNullableString(), routeValueDictionary["controller"].ToNullableString(), routeValueDictionary);
            return newUrl;
        }

        /// <summary>
        /// update and delete parameter,then redirect
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="action">action</param>
        /// <param name="controller">controller</param>
        /// <param name="alterParameterName">alter parameter name</param>
        /// <param name="newValue">value</param>
        /// <param name="deleteParameterNames">delete parameter names</param>
        /// <returns>new url</returns>
        public static string AlterAndDeleteParameter(this IUrlHelper url, string action, string controller, string alterParameterName, string newValue, params string[] deleteParameterNames)
        {
            string newUrl = string.Empty;
            RouteValueDictionary routeValueDictionary = url.ActionContext.RouteData.Values;
            routeValueDictionary = CopyRouteValueDictionary(routeValueDictionary);
            var collection = url.ActionContext.HttpContext.Request.Query;
            string[] allQueryKeys = collection.Keys.ToArray();
            if (routeValueDictionary.Keys.Contains(alterParameterName))
            {
                routeValueDictionary[alterParameterName] = newValue;
            }
            if (!string.IsNullOrEmpty(action))
            {
                routeValueDictionary["action"] = action;
            }
            if (!string.IsNullOrEmpty(controller))
            {
                routeValueDictionary["controller"] = controller;
            }
            foreach (string qk in allQueryKeys)
            {
                if (qk == alterParameterName)
                {
                    routeValueDictionary.Add(qk, newValue);
                }
                else
                {
                    routeValueDictionary.Add(qk, collection[qk].ToNullableString());
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
                    if (routeValueDictionary.Keys.Contains(deleteKey))
                    {
                        routeValueDictionary.Remove(deleteKey);
                    }
                }
            }
            newUrl = url.Action(routeValueDictionary["action"].ToNullableString(), routeValueDictionary["controller"].ToNullableString(), routeValueDictionary);
            return newUrl;
        }

        /// <summary>
        /// add and delete parameter value
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="parameterName">parameter name</param>
        /// <param name="addValue">add value</param>
        /// <param name="deleteValue">delete value</param>
        /// <returns>new url</returns>
        public static string AddAndDeleteParameterValue(this IUrlHelper url, string parameterName, string addValue, string deleteValue)
        {
            string newUrl = string.Empty;
            RouteValueDictionary routeValueDictionary = url.ActionContext.RouteData.Values;
            routeValueDictionary = CopyRouteValueDictionary(routeValueDictionary);
            var collection = url.ActionContext.HttpContext.Request.Query;
            string[] allQueryKeys = collection.Keys.ToArray();
            foreach (string qk in allQueryKeys)
            {
                routeValueDictionary.Add(qk, collection[qk].ToNullableString());
            }
            string nowValue = string.Empty;
            if (routeValueDictionary.ContainsKey(parameterName))
            {
                nowValue = routeValueDictionary[parameterName].ToNullableString();
                if (!deleteValue.IsNullOrEmpty())
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
            newUrl = url.Action(routeValueDictionary["action"].ToNullableString(), routeValueDictionary["controller"].ToNullableString(), routeValueDictionary);
            return newUrl;
        }

        /// <summary>
        /// add and delete parameter value
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="parameterName">parameter name</param>
        /// <param name="addValue">add value</param>
        /// <param name="deleteValue">delete value</param>
        /// <returns>new url</returns>
        public static string AddAndDeleteParameterValue(this IUrlHelper url, string action, string controller, string parameterName, string addValue, string deleteValue)
        {
            string newUrl = string.Empty;
            RouteValueDictionary routeValueDictionary = url.ActionContext.RouteData.Values;
            routeValueDictionary = CopyRouteValueDictionary(routeValueDictionary);
            var collection = url.ActionContext.HttpContext.Request.Query;
            string[] allQueryKeys = collection.Keys.ToArray();
            foreach (string qk in allQueryKeys)
            {
                routeValueDictionary.Add(qk, collection[qk].ToNullableString());
            }
            if (!string.IsNullOrEmpty(action))
            {
                routeValueDictionary["action"] = action;
            }
            if (!string.IsNullOrEmpty(controller))
            {
                routeValueDictionary["controller"] = controller;
            }
            string nowValue = string.Empty;
            if (routeValueDictionary.ContainsKey(parameterName))
            {
                nowValue = routeValueDictionary[parameterName].ToNullableString();
            }
            if (!string.IsNullOrEmpty(nowValue))
            {
                nowValue.Replace(deleteValue, string.Empty);
                if (nowValue.IndexOf(addValue) < 0)
                {
                    nowValue += addValue;
                }
            }
            newUrl = url.Action(routeValueDictionary["action"].ToNullableString(), routeValueDictionary["controller"].ToNullableString(), routeValueDictionary);
            newUrl = newUrl.Trim(new char[] { '&', '?' });
            return newUrl;
        }

        /// <summary>
        /// get parameter value
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="parameterName">parameter name</param>
        /// <returns>parameter value</returns>
        public static string ParameterValue(this IUrlHelper url, string parameterName)
        {
            RouteValueDictionary routeValueDictionary = url.ActionContext.RouteData.Values;
            var collection = url.ActionContext.HttpContext.Request.Query;
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
        /// delete parameter
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="parameterName">parameter name</param>
        /// <returns>new url</returns>
        public static string DeleteParameter(this IUrlHelper url, string parameterName)
        {
            string newUrl = string.Empty;
            RouteValueDictionary routeValueDictionary = url.ActionContext.RouteData.Values;
            routeValueDictionary = CopyRouteValueDictionary(routeValueDictionary);
            var collection = url.ActionContext.HttpContext.Request.Query;
            string[] allQueryKeys = collection.Keys.ToArray();
            foreach (string qk in allQueryKeys)
            {
                routeValueDictionary.Add(qk, collection[qk].ToNullableString());
            }
            if (routeValueDictionary.Keys.Contains(parameterName))
            {
                routeValueDictionary.Remove(parameterName);
            }
            newUrl = url.Action(routeValueDictionary["action"].ToNullableString(), routeValueDictionary["controller"].ToNullableString(), routeValueDictionary);
            return newUrl;
        }

        #endregion
    }
}
