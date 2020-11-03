using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Html;
using EZNEW.Web.Security.Authorization;
using EZNEW.Web.Utility;
using EZNEW.Application;
using EZNEW.Web.Mvc.View.Extension;

namespace Microsoft.AspNetCore.Mvc.Rendering
{
    public static class ButtonExtensions
    {
        /// <summary>
        /// Create auth button
        /// </summary>
        /// <param name="htmlHelper">Html helper</param>
        /// <param name="options">Button options</param>
        /// <returns>Return button html content</returns>
        public static IHtmlContent AuthButton(this IHtmlHelper htmlHelper, AuthButtonOptions options)
        {
            if (options == null || (options.UseNowVerifyResult && !options.AllowAccess && options.ForbidStyle != ForbidStyle.Disable))
            {
                return HtmlString.Empty;
            }
            bool allowAccess = true;
            if (!options.UseNowVerifyResult)
            {
                allowAccess = AuthorizationManager.Authorize(new AuthorizeOptions()
                {
                    Action = options.ActionOptions?.Action,
                    Controller = options.ActionOptions?.Controller,
                    Area = options.ActionOptions?.Area,
                    Application = ApplicationManager.Current,
                    Claims = HttpContextHelper.Current.User.Claims.ToDictionary(c => c.Type, c => c.Value),
                    Method = options.ActionOptions?.Method,
                    ActionContext = htmlHelper?.ViewContext
                })?.AllowAccess ?? false;
            }
            if (!allowAccess && options.ForbidStyle != ForbidStyle.Disable)
            {
                return HtmlString.Empty;
            }
            var btnTagBuilder = new TagBuilder("button");
            var btnHtmlAttributes = options.HtmlAttributes ?? new Dictionary<string, object>();
            if (!btnHtmlAttributes.ContainsKey("type"))
            {
                btnHtmlAttributes.Add("type", "button");
            }
            if (!allowAccess && !btnHtmlAttributes.ContainsKey("disabled"))
            {
                btnHtmlAttributes.Add("disabled", "disabled");
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

        #region Default Auth Button

        /// <summary>
        /// Create auth button
        /// </summary>
        /// <param name="htmlHelper">Html helper</param>
        /// <param name="text">Text</param>
        /// <param name="authorizationActionOptions">Authorization action options</param>
        /// <param name="htmlAttributes">Html attributes</param>
        /// <returns>Return button html content</returns>
        public static IHtmlContent AuthButton(this IHtmlHelper htmlHelper, string text, AuthorizationActionOptions authorizationActionOptions, object htmlAttributes = null)
        {
            return AuthButton(htmlHelper, new AuthButtonOptions()
            {
                Text = text,
                ActionOptions = authorizationActionOptions,
                HtmlAttributes = htmlAttributes?.ObjectToDcitionary()
            });
        }

        /// <summary>
        /// Create auth button
        /// </summary>
        /// <param name="htmlHelper">Html helper</param>
        /// <param name="text">Text</param>
        /// <param name="controllerCode">Controller code</param>
        /// <param name="actionCode">Action code</param>
        /// <param name="htmlAttributes">Html attributes</param>
        /// <returns>Return button html content</returns>
        public static IHtmlContent AuthButton(this IHtmlHelper htmlHelper, string text, string controllerCode, string actionCode, object htmlAttributes = null)
        {
            return AuthButton(htmlHelper, text, new AuthorizationActionOptions(controllerCode, actionCode), htmlAttributes);
        }

        /// <summary>
        /// Create auth button
        /// </summary>
        /// <param name="htmlHelper">Html helper</param>
        /// <param name="text">Text</param>
        /// <param name="allowAccess">Allow access</param>
        /// <param name="htmlAttributes">Html attributes</param>
        /// <returns>Return button html content</returns>
        public static IHtmlContent AuthButton(this IHtmlHelper htmlHelper, string text, bool allowAccess, object htmlAttributes = null)
        {
            return AuthButton(htmlHelper, new AuthButtonOptions()
            {
                Text = text,
                AllowAccess = allowAccess,
                UseNowVerifyResult = true,
                HtmlAttributes = htmlAttributes?.ObjectToDcitionary()
            });
        }

        #endregion

        #region Ico Auth Button

        /// <summary>
        /// Create ico auth button
        /// </summary>
        /// <param name="htmlHelper">Html helper</param>
        /// <param name="text">Text</param>
        /// <param name="authorizationActionOptions">Authorization action options</param>
        /// <param name="htmlAttributes">Html attributes</param>
        /// <returns>Return button html content</returns>
        public static IHtmlContent AuthIcoButton(this IHtmlHelper htmlHelper, string text, AuthorizationActionOptions authorizationActionOptions, object htmlAttributes = null, object icoHtmlAttributes = null)
        {
            return AuthButton(htmlHelper, new AuthButtonOptions()
            {
                Text = text,
                ActionOptions = authorizationActionOptions,
                HtmlAttributes = htmlAttributes?.ObjectToDcitionary(),
                IcoHtmlAttributes = icoHtmlAttributes?.ObjectToDcitionary(),
                UseIco = true
            });
        }

        /// <summary>
        /// Create ico auth button
        /// </summary>
        /// <param name="htmlHelper">Html helper</param>
        /// <param name="text">Text</param>
        /// <param name="controllerCode">Controller code</param>
        /// <param name="actionCode">Action code</param>
        /// <param name="htmlAttributes">Html attributes</param>
        /// <returns>Return button html content</returns>
        public static IHtmlContent AuthIcoButton(this IHtmlHelper htmlHelper, string text, string controllerCode, string actionCode, object htmlAttributes = null, object icoHtmlAttributes = null)
        {
            return AuthIcoButton(htmlHelper, text, new AuthorizationActionOptions(controllerCode, actionCode), htmlAttributes, icoHtmlAttributes);
        }

        /// <summary>
        /// Create ico auth button
        /// </summary>
        /// <param name="htmlHelper">Html helper</param>
        /// <param name="text">Text</param>
        /// <param name="allowAccess">Allow access</param>
        /// <param name="htmlAttributes">Html attributes</param>
        /// <returns>Return button html content</returns>
        public static IHtmlContent AuthIcoButton(this IHtmlHelper htmlHelper, string text, bool allowAccess, object htmlAttributes = null, object icoHtmlAttributes = null)
        {
            return AuthButton(htmlHelper, new AuthButtonOptions()
            {
                Text = text,
                AllowAccess = allowAccess,
                UseNowVerifyResult = true,
                HtmlAttributes = htmlAttributes?.ObjectToDcitionary(),
                IcoHtmlAttributes = icoHtmlAttributes?.ObjectToDcitionary(),
                UseIco = true
            });
        }

        #endregion

        #region Pre Attribute Auth Button

        /// <summary>
        /// Create auth button use pre attribute
        /// </summary>
        /// <param name="htmlHelper">Html helper</param>
        /// <param name="text">Text</param>
        /// <param name="authorizationActionOptions">Authorization action options</param>
        /// <param name="attrName">Attr name</param>
        /// <param name="attrValues">Attr values</param>
        /// <param name="htmlAttributes">Html attributes</param>
        /// <param name="icoHtmlAttributes">Ico html attributes</param>
        /// <returns>Return button html content</returns>
        public static IHtmlContent PreAttributeAuthButton(this IHtmlHelper htmlHelper, string text, AuthorizationActionOptions authorizationActionOptions, string attrName, List<string> attrValues = null, object htmlAttributes = null, object icoHtmlAttributes = null)
        {
            var attributesDict = htmlAttributes?.ObjectToDcitionary() ?? new Dictionary<string, object>();
            if (!attrValues.IsNullOrEmpty())
            {
                if (attributesDict.ContainsKey(attrName))
                {
                    attributesDict[attrName] += string.Join(" ", attrValues.ToArray());
                }
                else
                {
                    attributesDict.Add(attrName, string.Join(" ", attrValues.ToArray()));
                }
            }
            return AuthButton(htmlHelper, new AuthButtonOptions()
            {
                Text = text,
                ActionOptions = authorizationActionOptions,
                HtmlAttributes = attributesDict,
                UseIco = icoHtmlAttributes != null,
                IcoHtmlAttributes = icoHtmlAttributes?.ObjectToDcitionary()
            });
        }

        /// <summary>
        /// Create auth button use pre attribute
        /// </summary>
        /// <param name="htmlHelper">Html helper</param>
        /// <param name="text">Text</param>
        /// <param name="allowAccess">Allow access</param>
        /// <param name="attrName">Attr name</param>
        /// <param name="attrValues">Attr values</param>
        /// <param name="htmlAttributes">Html attributes</param>
        /// <param name="icoHtmlAttributes">Ico html attributes</param>
        /// <returns>Return button html content</returns>
        public static IHtmlContent PreAttributeAuthButton(this IHtmlHelper htmlHelper, string text, bool allowAccess, string attrName, List<string> attrValues = null, object htmlAttributes = null, object icoHtmlAttributes = null)
        {
            var attributesDict = htmlAttributes?.ObjectToDcitionary() ?? new Dictionary<string, object>();
            if (!attrValues.IsNullOrEmpty())
            {
                if (attributesDict.ContainsKey(attrName))
                {
                    attributesDict[attrName] += string.Join(" ", attrValues.ToArray());
                }
                else
                {
                    attributesDict.Add(attrName, string.Join(" ", attrValues.ToArray()));
                }
            }
            return AuthButton(htmlHelper, new AuthButtonOptions()
            {
                Text = text,
                UseNowVerifyResult = true,
                AllowAccess = allowAccess,
                HtmlAttributes = attributesDict,
                UseIco = icoHtmlAttributes != null,
                IcoHtmlAttributes = icoHtmlAttributes?.ObjectToDcitionary()
            });
        }

        /// <summary>
        /// Create auth button use pre attribute
        /// </summary>
        /// <param name="htmlHelper">Html helper</param>
        /// <param name="text">Text</param>
        /// <param name="authorizationActionOptions">Authorization action options</param>
        /// <param name="classValues">Class values</param>
        /// <param name="htmlAttributes">Html attributes</param>
        /// <param name="icoHtmlAttributes">Ico html attributes</param>
        /// <returns>Return button html content</returns>
        public static IHtmlContent PreClassAuthButton(this IHtmlHelper htmlHelper, string text, AuthorizationActionOptions authorizationActionOptions, List<string> classValues = null, object htmlAttributes = null, object icoHtmlAttributes = null)
        {
            return PreAttributeAuthButton(htmlHelper, text, authorizationActionOptions, "class", classValues, htmlAttributes, icoHtmlAttributes);
        }

        /// <summary>
        /// Create auth button use pre attribute
        /// </summary>
        /// <param name="htmlHelper">Html helper</param>
        /// <param name="text">Text</param>
        /// <param name="allowAccess">Allow access</param>
        /// <param name="classValues">Class values</param>
        /// <param name="htmlAttributes">Html attributes</param>
        /// <param name="icoHtmlAttributes">Ico html attributes</param>
        /// <returns>Return button html content</returns>
        public static IHtmlContent PreClassAuthButton(this IHtmlHelper htmlHelper, string text, bool allowAccess, List<string> classValues = null, object htmlAttributes = null, object icoHtmlAttributes = null)
        {
            return PreAttributeAuthButton(htmlHelper, text, allowAccess, "class", classValues, htmlAttributes, icoHtmlAttributes);
        }

        #endregion
    }
}
