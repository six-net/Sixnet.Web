using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using EZNEW.DataValidation;

namespace Microsoft.AspNetCore.Mvc.Rendering
{
    public static class ValidationExtensions
    {
        /// <summary>
        /// Gets or sets the tip ico class
        /// </summary>
        public static string TipIco { get; set; } = "tip";

        /// <summary>
        ///  Create default validation message
        /// </summary>
        /// <typeparam name="TModel">model type</typeparam>
        /// <typeparam name="TProperty">Property</typeparam>
        /// <param name="htmlHelper">Html helper</param>
        /// <param name="property">Property</param>
        /// <param name="validationMessage">Validation message</param>
        /// <param name="htmlAttributes">Html attributes</param>
        /// <returns>Return html content</returns>
        public static IHtmlContent DefaultValidationMessageFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> property, string validationMessage, object htmlAttributes)
        {
            if (string.IsNullOrWhiteSpace(validationMessage))
            {
                validationMessage = ValidationManager.GetValidationTipMessage<TModel, TProperty>(property);
            }
            IDictionary<string, object> attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            string attrName = "class";
            if (attributes != null && string.IsNullOrWhiteSpace(validationMessage) && attributes.ContainsKey(attrName))
            {
                var attrVal = attributes[attrName];
                if (attrVal != null)
                {
                    attributes[attrName] = attrVal.ToString().Replace(TipIco, "");
                }
            }
            return htmlHelper.ValidationMessageFor(property, validationMessage, attributes);
        }
    }
}
