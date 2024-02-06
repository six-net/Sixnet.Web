using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;

namespace Microsoft.AspNetCore.Mvc.Rendering
{
    public static class FormExtensions
    {
        public static MvcForm AjaxBeginForm(this IHtmlHelper htmlHelper, AjaxFormOptions ajaxOptions)
        {
            string formAction = htmlHelper.ViewContext.HttpContext.Request.GetEncodedUrl();
            return FormHelper(htmlHelper, formAction,"", ajaxOptions, new RouteValueDictionary());
        }

        public static MvcForm AjaxBeginForm(this IHtmlHelper htmlHelper, string actionName, AjaxFormOptions ajaxOptions)
        {
            return AjaxBeginForm(htmlHelper, actionName, (string)null /* controllerName */, ajaxOptions);
        }

        public static MvcForm AjaxBeginForm(this IHtmlHelper htmlHelper, string actionName, object routeValues, AjaxFormOptions ajaxOptions)
        {
            return AjaxBeginForm(htmlHelper, actionName, (string)null /* controllerName */, routeValues, ajaxOptions);
        }

        public static MvcForm AjaxBeginForm(this IHtmlHelper htmlHelper, string actionName, object routeValues, AjaxFormOptions ajaxOptions, object htmlAttributes)
        {
            return AjaxBeginForm(htmlHelper, actionName, (string)null /* controllerName */, routeValues, ajaxOptions, htmlAttributes);
        }

        public static MvcForm AjaxBeginForm(this IHtmlHelper htmlHelper, string actionName, RouteValueDictionary routeValues, AjaxFormOptions ajaxOptions)
        {
            return AjaxBeginForm(htmlHelper, actionName, (string)null /* controllerName */, routeValues, ajaxOptions);
        }

        public static MvcForm AjaxBeginForm(this IHtmlHelper htmlHelper, string actionName, RouteValueDictionary routeValues, AjaxFormOptions ajaxOptions, IDictionary<string, object> htmlAttributes)
        {
            return AjaxBeginForm(htmlHelper, actionName, (string)null /* controllerName */, routeValues, ajaxOptions, htmlAttributes);
        }

        public static MvcForm AjaxBeginForm(this IHtmlHelper htmlHelper, string actionName, string controllerName, AjaxFormOptions ajaxOptions)
        {
            return AjaxBeginForm(htmlHelper, actionName, controllerName, null /* values */, ajaxOptions, null /* htmlAttributes */);
        }

        public static MvcForm AjaxBeginForm(this IHtmlHelper htmlHelper, string actionName, string controllerName, object routeValues, AjaxFormOptions ajaxOptions)
        {
            return AjaxBeginForm(htmlHelper, actionName, controllerName, routeValues, ajaxOptions, null /* htmlAttributes */);
        }

        public static MvcForm AjaxBeginForm(this IHtmlHelper htmlHelper, string actionName, string controllerName, object routeValues, AjaxFormOptions ajaxOptions, object htmlAttributes)
        {
            RouteValueDictionary newValues = new RouteValueDictionary(routeValues);
            var newAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            return AjaxBeginForm(htmlHelper, actionName, controllerName, newValues, ajaxOptions, newAttributes);
        }

        public static MvcForm AjaxBeginForm(this IHtmlHelper htmlHelper, string actionName, string controllerName, RouteValueDictionary routeValues, AjaxFormOptions ajaxOptions)
        {
            return AjaxBeginForm(htmlHelper, actionName, controllerName, routeValues, ajaxOptions, null /* htmlAttributes */);
        }

        public static MvcForm AjaxBeginForm(this IHtmlHelper htmlHelper, string actionName, string controllerName, RouteValueDictionary routeValues, AjaxFormOptions ajaxOptions, IDictionary<string, object> htmlAttributes)
        {
            // get target URL
            return FormHelper(htmlHelper,actionName,controllerName, ajaxOptions, htmlAttributes);
        }

        private static MvcForm FormHelper(this IHtmlHelper htmlHelper,string actionName,string controllerName, AjaxFormOptions ajaxOptions, IDictionary<string, object> htmlAttributes)
        {
            htmlAttributes = htmlAttributes ?? new Dictionary<string, object>();
            if (ajaxOptions != null)
            {
                htmlAttributes = htmlAttributes.Union(ajaxOptions.ToUnobtrusiveHtmlAttributes()).ToDictionary(c=>c.Key,c=>c.Value);
            }
            return htmlHelper.BeginForm(actionName, controllerName,FormMethod.Post, htmlAttributes);
        }
    }
}
