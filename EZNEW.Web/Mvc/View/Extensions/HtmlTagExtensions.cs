using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using EZNEW.Application;
using EZNEW.Web.Mvc.View.Extension;
using EZNEW.Web.Security.Authorization;
using EZNEW.Web.Utility;
using Microsoft.AspNetCore.Mvc;

namespace Microsoft.AspNetCore.Mvc.Rendering
{
    public static class HtmlTagExtensions
    {
        /// <summary>
        /// Create auth html element
        /// </summary>
        /// <param name="htmlHelper">Html helper</param>
        /// <param name="tagName">Tag name</param>
        /// <param name="options">Options</param>
        /// <returns>Return html content</returns>
        public static IHtmlContent AuthHtmlElement(this IHtmlHelper htmlHelper, string tagName, AuthButtonOptions options)
        {
            if (options == null || (options.UseNowVerifyResult && !options.AllowAccess))
            {
                return HtmlString.Empty;
            }
            if (!options.UseNowVerifyResult)
            {
                var allowAccess = AuthorizationManager.VerifyAuthorize(new VerifyAuthorizationOption()
                {
                    ActionCode = options.AuthorizeOperation?.ActionCode,
                    ControllerCode = options.AuthorizeOperation?.ControllerCode,
                    Application = ApplicationManager.Current,
                    Claims = HttpContextHelper.Current.User.Claims.ToDictionary(c => c.Type, c => c.Value)
                })?.AllowAccess ?? false;
                if (!allowAccess)
                {
                    return HtmlString.Empty;
                }
            }
            var htmlTagBuilder = new TagBuilder(tagName);
            var htmlAttributes = options.HtmlAttributes ?? new Dictionary<string, object>();
            htmlTagBuilder.MergeAttributes(htmlAttributes);
            if (options.UseIco)
            {
                var icoTagBuilder = new TagBuilder("i");
                icoTagBuilder.MergeAttributes(options.IcoHtmlAttributes);
                htmlTagBuilder.InnerHtml.AppendHtml(icoTagBuilder);
                htmlTagBuilder.InnerHtml.Append(" ");
            }
            htmlTagBuilder.InnerHtml.Append(options.Text);
            return htmlTagBuilder;
        }

        /// <summary>
        /// Create auth menu element
        /// </summary>
        /// <param name="htmlHelper">Html helper</param>
        /// <param name="urlHelper">Url helper</param>
        /// <param name="text">Test</param>
        /// <param name="url">Url</param>
        /// <param name="options">Options</param>
        /// <returns>Return menu element</returns>
        public static IHtmlContent AuthMenuItem(this IHtmlHelper htmlHelper, IUrlHelper urlHelper, string text, string controlerCode, string actionCode, AuthButtonOptions options = null)
        {
            options ??= new AuthButtonOptions();
            options.HtmlAttributes ??= new Dictionary<string, object>();
            options.HtmlAttributes["action"] = urlHelper.Action(actionCode, controlerCode);
            options.Text = text;
            options.AuthorizeOperation = new AuthorizeOperation(controlerCode, actionCode);
            return htmlHelper.AuthHtmlElement("li", options);
        }
    }
}
