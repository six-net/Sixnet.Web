using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Html;
using EZNEW.Application;
using EZNEW.Web.Security.Authorization;
using EZNEW.Web.Utility;
using EZNEW.Web.Mvc.View.Extension;

namespace Microsoft.AspNetCore.Mvc.Rendering
{
    public static class LinkExtensions
    {
        /// <summary>
        /// Create auth link
        /// </summary>
        /// <param name="htmlHelper">Html helper</param>
        /// <param name="options">Options</param>
        /// <returns>Return html content</returns>
        public static IHtmlContent AuthLink(this IHtmlHelper htmlHelper, AuthButtonOptions options)
        {
            if (options == null || (options.UseNowVerifyResult && !options.AllowAccess))
            {
                return HtmlString.Empty;
            }
            if (!options.UseNowVerifyResult)
            {
                var allowAccess = AuthorizationManager.Authorize(new AuthorizeOptions()
                {
                    Action = options.ActionOptions?.Action,
                    Controller = options.ActionOptions?.Controller,
                    Area=options.ActionOptions?.Area,
                    Application = ApplicationManager.Current,
                    Claims = HttpContextHelper.Current.User.Claims.ToDictionary(c => c.Type, c => c.Value),
                    ActionContext = htmlHelper?.ViewContext,
                    Method = options.ActionOptions?.Method
                })?.AllowAccess ?? false;
                if (!allowAccess)
                {
                    return HtmlString.Empty;
                }
            }
            var btnTagBuilder = new TagBuilder("a");
            var btnHtmlAttributes = options.HtmlAttributes ?? new Dictionary<string, object>();
            if (!btnHtmlAttributes.ContainsKey("href"))
            {
                btnHtmlAttributes.Add("href", "javascript:void(0)");
            }
            btnTagBuilder.MergeAttributes(btnHtmlAttributes);
            if (options.UseIco)
            {
                var icoTagBuilder = new TagBuilder("i");
                icoTagBuilder.MergeAttributes(options.IcoHtmlAttributes);
                btnTagBuilder.InnerHtml.AppendHtml(icoTagBuilder);
                btnTagBuilder.InnerHtml.Append(" ");
            }
            btnTagBuilder.InnerHtml.Append(options.Text);
            return btnTagBuilder;
        }

        /// <summary>
        /// Create auth link
        /// </summary>
        /// <param name="htmlHelper">Html helper</param>
        /// <param name="text">Text</param>
        /// <param name="controllerCode">Controller code</param>
        /// <param name="actionCode">Action code</param>
        /// <param name="htmlAttributes">Html attributes</param>
        /// <returns>Return </returns>
        public static IHtmlContent AuthLink(this IHtmlHelper htmlHelper, string text, string controllerCode, string actionCode, object htmlAttributes = null)
        {
            return AuthLink(htmlHelper, new AuthButtonOptions()
            {
                Text = text,
                ActionOptions = new AuthorizationActionOptions(controllerCode, actionCode),
                HtmlAttributes = htmlAttributes?.ObjectToDcitionary()
            });
        }

        /// <summary>
        /// Dropdown auth link
        /// </summary>
        /// <param name="htmlHelper">Html helper</param>
        /// <param name="text">Text</param>
        /// <param name="authorizationActionOptions">Authorization action options</param>
        /// <param name="htmlAttributes">Html attributes</param>
        /// <returns>Return html content</returns>
        public static IHtmlContent DropdownAuthLink(this IHtmlHelper htmlHelper, string text, AuthorizationActionOptions authorizationActionOptions, object htmlAttributes = null, object icoHtmlAttributes = null)
        {
            return AuthLink(htmlHelper, new AuthButtonOptions()
            {
                Text = text,
                ActionOptions = authorizationActionOptions,
                HtmlAttributes = htmlAttributes?.ObjectToDcitionary(),
                UseIco = icoHtmlAttributes != null,
                IcoHtmlAttributes = icoHtmlAttributes?.ObjectToDcitionary()
            });
        }

        /// <summary>
        /// Dropdown auth link
        /// </summary>
        /// <param name="htmlHelper">Html helper</param>
        /// <param name="text">Text</param>
        /// <param name="controllerCode">Controller code</param>
        /// <param name="actionCode">Action code</param>
        /// <param name="htmlAttributes">Html attributes</param>
        /// <returns>Return html content</returns>
        public static IHtmlContent DropdownAuthLink(this IHtmlHelper htmlHelper, string text, string controllerCode, string actionCode, object htmlAttributes = null, object icoHtmlAttributes = null)
        {
            return DropdownAuthLink(htmlHelper, text, new AuthorizationActionOptions(controllerCode, actionCode), htmlAttributes, icoHtmlAttributes);
        }
    }
}
