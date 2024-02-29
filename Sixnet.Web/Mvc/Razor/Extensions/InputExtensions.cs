using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Sixnet.Expressions.Linq;
using Sixnet.Web.Mvc.Razor.Extensions;

namespace Microsoft.AspNetCore.Mvc.Rendering
{
    public static class InputExtensions
    {
        #region Checkbox

        /// <summary>
        /// Create checkbox
        /// </summary>
        /// <param name="htmlHelper">Html helper</param>
        /// <param name="name">Name</param>
        /// <param name="items">Items</param>
        /// <param name="cbxHtmlAttributes">Checkbox html attributes</param>
        /// <param name="lableHtmlAttributes">Lable html attributes</param>
        /// <param name="itemGroupAttributes">Item group attributes</param>
        /// <returns>Return html content</returns>
        static IHtmlContent CheckBoxInternal(IHtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> items, IDictionary<string, object> cbxHtmlAttributes, IDictionary<string, object> lableHtmlAttributes, IDictionary<string, object> itemGroupAttributes)
        {
            if (items.IsNullOrEmpty())
            {
                return HtmlString.Empty;
            }
            string fullName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
            if (string.IsNullOrWhiteSpace(fullName))
            {
                throw new ArgumentException("CheckBox Control Name Is Null Or Empty");
            }
            var checkBoxBuilder = new HtmlContentBuilder();
            int i = 0;
            foreach (SelectListItem item in items)
            {
                string cbxId = string.Format("cbx-{0}-{1}", fullName, i.ToString());
                //group
                TagBuilder groupTag = new TagBuilder("span");
                groupTag.MergeAttribute("cbx-group-id", cbxId);
                groupTag.MergeAttributes(itemGroupAttributes);
                //cbx
                TagBuilder cbxTag = new TagBuilder("input");
                cbxTag.MergeAttribute("cbx-id", cbxId);
                cbxTag.MergeAttribute("id", cbxId);
                cbxTag.MergeAttributes(cbxHtmlAttributes);
                cbxTag.MergeAttribute("type", GetInputTypeName(InputType.CheckBox));
                cbxTag.MergeAttribute("value", item.Value);
                cbxTag.MergeAttribute("name", fullName);
                if (item.Selected)
                {
                    cbxTag.MergeAttribute("checked", "checked");
                }
                //lable
                TagBuilder labTag = new TagBuilder("label");
                labTag.MergeAttributes(lableHtmlAttributes);
                labTag.MergeAttribute("cbx-lable-id", cbxId);
                labTag.MergeAttribute("for", cbxId);
                labTag.InnerHtml.SetContent(item.Text);
                var groupInnerTag = new HtmlContentBuilder(2);
                groupInnerTag.AppendHtml(cbxTag);
                groupInnerTag.AppendHtml(labTag);
                groupTag.InnerHtml.SetHtmlContent(groupInnerTag);
                checkBoxBuilder.AppendHtml(groupTag);
                i++;
            }
            return checkBoxBuilder;
        }

        /// <summary>
        /// Create checkbox
        /// </summary>
        /// <param name="htmlHelper">Html helper</param>
        /// <param name="name">Name</param>
        /// <param name="items">Items</param>
        /// <param name="htmlAttributes">Html attributes</param>
        /// <param name="textAttributes">Text attributes</param>
        /// <param name="valueAttributes">Value attributes</param>
        /// <returns>Return html content</returns>
        public static IHtmlContent CheckBoxInternal(this IHtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> items, IDictionary<string, object> htmlAttributes = null, IEnumerable<string> textAttributes = null, IEnumerable<string> valueAttributes = null)
        {
            if (items.IsNullOrEmpty())
            {
                return HtmlString.Empty;
            }
            string fullName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
            if (string.IsNullOrWhiteSpace(fullName))
            {
                throw new ArgumentException("checkbox name is null or empty");
            }
            var checkboxBuilder = new HtmlContentBuilder();
            foreach (SelectListItem item in items)
            {
                TagBuilder cbxTag = new TagBuilder("input");
                if (!htmlAttributes.IsNullOrEmpty())
                {
                    cbxTag.MergeAttributes(htmlAttributes);
                }
                cbxTag.MergeAttribute("type", GetInputTypeName(InputType.CheckBox));
                cbxTag.MergeAttribute("value", item.Value);
                cbxTag.MergeAttribute("name", fullName);
                if (item.Selected)
                {
                    cbxTag.MergeAttribute("checked", "checked");
                }
                if (!textAttributes.IsNullOrEmpty())
                {
                    foreach (var textAttr in textAttributes)
                    {
                        cbxTag.MergeAttribute(textAttr, item.Text);
                    }
                }
                if (!valueAttributes.IsNullOrEmpty())
                {
                    foreach (var valAttr in valueAttributes)
                    {
                        cbxTag.MergeAttribute(valAttr, item.Value);
                    }
                }
                checkboxBuilder.AppendHtml(cbxTag);
            }
            return checkboxBuilder;
        }

        #region Enum to checkbox

        /// <summary>
        /// Enum to checkbox
        /// </summary>
        /// <param name="htmlHelper">Html helper</param>
        /// <param name="name">Name</param>
        /// <param name="enumType">Enum type</param>
        /// <param name="cbxHtmlAttributes">Checkbox html attributes</param>
        /// <param name="lableHtmlAttributes">Lable html attributes</param>
        /// <param name="groupHtmlAttributes">Group html attributes</param>
        /// <param name="checkedValues">Checked values</param>
        /// <returns>Return html content</returns>
        public static IHtmlContent EnumToCheckBox(this IHtmlHelper htmlHelper, string name, Enum enumType, object cbxHtmlAttributes = null, object lableHtmlAttributes = null, object groupHtmlAttributes = null, params string[] checkedValues)
        {
            IList<SelectListItem> items = EnumHelper.GetSelectList(enumType.GetType(), checkedValues == null ? null : checkedValues.ToList());
            return CheckBoxInternal(htmlHelper, name, items, HtmlHelper.AnonymousObjectToHtmlAttributes(cbxHtmlAttributes), HtmlHelper.AnonymousObjectToHtmlAttributes(lableHtmlAttributes), HtmlHelper.AnonymousObjectToHtmlAttributes(groupHtmlAttributes));
        }

        /// <summary>
        /// Enum to checkbox
        /// </summary>
        /// <param name="htmlHelper">Html helper</param>
        /// <param name="name">Name</param>
        /// <param name="enumType">Enum type</param>
        /// <param name="cbxHtmlAttributes">Checkbox html attributes</param>
        /// <param name="lableHtmlAttributes">Lable html attributes</param>
        /// <param name="groupHtmlAttributes">Group html attributes</param>
        /// <param name="checkedValue">Checked value</param>
        /// <returns>Return html content</returns>
        public static IHtmlContent EnumToCheckBox(this IHtmlHelper htmlHelper, string name, Enum enumType, object cbxHtmlAttributes = null, object lableHtmlAttributes = null, object groupHtmlAttributes = null, Enum checkedValue = null)
        {
            IList<SelectListItem> items = EnumHelper.GetSelectList(enumType.GetType(), checkedValue);
            return CheckBoxInternal(htmlHelper, name, items, HtmlHelper.AnonymousObjectToHtmlAttributes(cbxHtmlAttributes), HtmlHelper.AnonymousObjectToHtmlAttributes(lableHtmlAttributes), HtmlHelper.AnonymousObjectToHtmlAttributes(groupHtmlAttributes));
        }

        /// <summary>
        /// Enum to checkbox
        /// </summary>
        /// <param name="htmlHelper">Html helper</param>
        /// <param name="expression">Name Expression</param>
        /// <param name="enumType">Enum type</param>
        /// <param name="cbxHtmlAttributes">Checkbox html attributes</param>
        /// <param name="lableHtmlAttributes">Lable html attributes</param>
        /// <param name="groupHtmlAttributes">Group html attributes</param>
        /// <param name="checkedValues">Checked values</param>
        /// <returns>Return html content</returns>
        public static IHtmlContent EnumToCheckBox<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, Enum enumType, object cbxHtmlAttributes = null, object lableHtmlAttributes = null, object groupHtmlAttributes = null, params string[] checkedValues)
        {
            return EnumToCheckBox(htmlHelper, SixnetExpressionHelper.GetExpressionText(expression), enumType, cbxHtmlAttributes, lableHtmlAttributes, groupHtmlAttributes, checkedValues);
        }

        /// <summary>
        /// Enum to checkbox
        /// </summary>
        /// <param name="htmlHelper">Html helper</param>
        /// <param name="expression">Name Expression</param>
        /// <param name="enumType">Enum type</param>
        /// <param name="cbxHtmlAttributes">Checkbox html attributes</param>
        /// <param name="lableHtmlAttributes">Lable html attributes</param>
        /// <param name="groupHtmlAttributes">Group html attributes</param>
        /// <param name="checkedValue">Checked value</param>
        /// <returns>Return html content</returns>
        public static IHtmlContent EnumToCheckBox<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, Enum enumType, object cbxHtmlAttributes = null, object lableHtmlAttributes = null, object groupHtmlAttributes = null, Enum checkedValue = null)
        {
            return EnumToCheckBox(htmlHelper, SixnetExpressionHelper.GetExpressionText(expression), enumType, cbxHtmlAttributes, lableHtmlAttributes, groupHtmlAttributes, checkedValue);
        }

        /// <summary>
        /// Enum to checkbox
        /// </summary>
        /// <param name="htmlHelper">Html helper</param>
        /// <param name="name">Name</param>
        /// <param name="enumType">Enum type</param>
        /// <param name="htmlAttributes">Html attributes</param>
        /// <param name="textAttributes">Text attributes</param>
        /// <param name="valueAttributes">Value attributes</param>
        /// <returns>Return html content</returns>
        public static IHtmlContent EnumToCheckBox(this IHtmlHelper htmlHelper, string name, Enum enumType, object htmlAttributes = null, Enum checkedValue = null, IEnumerable<string> textAttributes = null, IEnumerable<string> valueAttributes = null)
        {
            IList<SelectListItem> items = EnumHelper.GetSelectList(enumType.GetType(), checkedValue);
            return CheckBoxInternal(htmlHelper, name, items, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes), textAttributes, valueAttributes);
        }

        /// <summary>
        /// Enum to checkbox
        /// </summary>
        /// <param name="htmlHelper">Html helper</param>
        /// <param name="name">Name</param>
        /// <param name="enumType">Enum type</param>
        /// <param name="htmlAttributes">Html attributes</param>
        /// <param name="textAttributes">Text attributes</param>
        /// <param name="valueAttributes">Value attributes</param>
        /// <returns>Return html content</returns>
        public static IHtmlContent EnumToCheckBox<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> name, Enum enumType, object htmlAttributes = null, Enum checkedValue = null, IEnumerable<string> textAttributes = null, IEnumerable<string> valueAttributes = null)
        {
            var nameValue = SixnetExpressionHelper.GetExpressionText(name);
            return EnumToCheckBox(htmlHelper, nameValue, enumType, htmlAttributes, checkedValue, textAttributes, valueAttributes);
        }

        #endregion

        #region DataTable to checkbox

        /// <summary>
        /// DataTable to checkbox
        /// </summary>
        /// <param name="htmlHelper">Html helper</param>
        /// <param name="name">Name</param>
        /// <param name="dataTable">DataTable</param>
        /// <param name="optionValueFieldName">Value Field</param>
        /// <param name="optionTextFieldName">Text Field</param>
        /// <param name="cbxHtmlAttributes">Checkbox html attributes</param>
        /// <param name="lableHtmlAttributes">Lable html attributes</param>
        /// <param name="groupHtmlAttributes">Group html attributes</param>
        /// <param name="checkedValues">Checked values</param>
        /// <returns>Return html content</returns>
        public static IHtmlContent DataTableToCheckBox(this IHtmlHelper htmlHelper, string name, DataTable dataTable, string optionValueFieldName, string optionTextFieldName, object cbxHtmlAttributes = null, object lableHtmlAttributes = null, object groupHtmlAttributes = null, params string[] checkedValues)
        {
            List<SelectListItem> listItems = new List<SelectListItem>();
            List<string> checkValueList = checkedValues == null ? new List<string>(0) : checkedValues.ToList();
            if (dataTable != null)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    string rowValue = row[optionValueFieldName].ToString();
                    listItems.Add(new SelectListItem()
                    {
                        Value = rowValue,
                        Text = row[optionTextFieldName].ToString(),
                        Selected = checkValueList.Contains(rowValue)
                    });
                }
            }
            return CheckBoxInternal(htmlHelper, name, listItems, HtmlHelper.AnonymousObjectToHtmlAttributes(cbxHtmlAttributes), HtmlHelper.AnonymousObjectToHtmlAttributes(lableHtmlAttributes), HtmlHelper.AnonymousObjectToHtmlAttributes(groupHtmlAttributes));
        }

        /// <summary>
        /// DataTable to checkbox
        /// </summary>
        /// <param name="htmlHelper">Html helper</param>
        /// <param name="expression">Name Expression</param>
        /// <param name="dataTable">DataTable</param>
        /// <param name="optionValueFieldName">Value Field</param>
        /// <param name="optionTextFieldName">Text Field</param>
        /// <param name="cbxHtmlAttributes">Checkbox html attributes</param>
        /// <param name="lableHtmlAttributes">Lable html attributes</param>
        /// <param name="groupHtmlAttributes">Group html attributes</param>
        /// <param name="checkedValues">Checked values</param>
        /// <returns>Return html content</returns>
        public static IHtmlContent DataTableToCheckBox<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, DataTable dataTable, string optionValueFieldName, string optionTextFieldName, object cbxHtmlAttributes = null, object lableHtmlAttributes = null, object groupHtmlAttributes = null, params string[] checkedValues)
        {
            return DataTableToCheckBox(htmlHelper, SixnetExpressionHelper.GetExpressionText(expression), dataTable, optionValueFieldName, optionTextFieldName, cbxHtmlAttributes, lableHtmlAttributes, groupHtmlAttributes, checkedValues);
        }

        #endregion

        #endregion

        #region Radio

        /// <summary>
        /// Create radio
        /// </summary>
        /// <param name="htmlHelper">Html helper</param>
        /// <param name="name">Name</param>
        /// <param name="items">Items</param>
        /// <param name="radioHtmlAttributes">Radio html attributes</param>
        /// <param name="lableHtmlAttributes">Lable html attributes</param>
        /// <param name="itemGroupAttributes">Item group attributes</param>
        /// <returns>Return html content</returns>
        static IHtmlContent RadioInternal(IHtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> items, IDictionary<string, object> radioHtmlAttributes, IDictionary<string, object> lableHtmlAttributes, IDictionary<string, object> itemGroupAttributes)
        {
            if (items.IsNullOrEmpty())
            {
                return HtmlString.Empty;
            }
            string fullName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
            if (string.IsNullOrWhiteSpace(fullName))
            {
                throw new ArgumentException("radio name is null Or empty");
            }
            var radioBuilder = new HtmlContentBuilder();
            int i = 0;
            foreach (SelectListItem item in items)
            {
                string cbxId = string.Format("rdo-{0}-{1}", fullName, i.ToString());
                //group
                TagBuilder groupTag = new TagBuilder("span");
                groupTag.MergeAttribute("rdo-group-id", cbxId);
                groupTag.MergeAttributes(itemGroupAttributes);
                //cbx
                TagBuilder cbxTag = new TagBuilder("input");
                cbxTag.MergeAttribute("rdo-id", cbxId);
                cbxTag.MergeAttribute("id", cbxId);
                cbxTag.MergeAttributes(radioHtmlAttributes);
                cbxTag.MergeAttribute("type", GetInputTypeName(InputType.Radio));
                cbxTag.MergeAttribute("value", item.Value);
                cbxTag.MergeAttribute("name", fullName);
                if (item.Selected)
                {
                    cbxTag.MergeAttribute("checked", "checked");
                }
                //lable
                TagBuilder labTag = new TagBuilder("label");
                labTag.MergeAttributes(lableHtmlAttributes);
                labTag.MergeAttribute("rdo-lable-id", cbxId);
                labTag.MergeAttribute("for", cbxId);
                labTag.InnerHtml.SetContent(item.Text);
                var groupInnerBuilder = new HtmlContentBuilder(2);
                groupInnerBuilder.AppendHtml(cbxTag);
                groupInnerBuilder.AppendHtml(labTag);
                groupTag.InnerHtml.SetHtmlContent(groupInnerBuilder);
                radioBuilder.AppendHtml(groupTag);
                i++;
            }
            return radioBuilder;
        }

        /// <summary>
        /// Create radio
        /// </summary>
        /// <param name="htmlHelper">Html helper</param>
        /// <param name="name">Name</param>
        /// <param name="items">Items</param>
        /// <param name="htmlAttributes">Html attributes</param>
        /// <param name="textAttributes">Text attributes</param>
        /// <param name="valueAttributes">Value attributes</param>
        /// <returns>Return html content</returns>
        public static IHtmlContent RadioInternal(this IHtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> items, IDictionary<string, object> htmlAttributes = null, IEnumerable<string> textAttributes = null, IEnumerable<string> valueAttributes = null)
        {
            if (items.IsNullOrEmpty())
            {
                return HtmlString.Empty;
            }
            string fullName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
            if (string.IsNullOrWhiteSpace(fullName))
            {
                throw new ArgumentException("radio name is null or empty");
            }
            var radioBuilder = new HtmlContentBuilder();
            foreach (SelectListItem item in items)
            {
                TagBuilder cbxTag = new TagBuilder("input");
                if (!htmlAttributes.IsNullOrEmpty())
                {
                    cbxTag.MergeAttributes(htmlAttributes);
                }
                cbxTag.MergeAttribute("type", GetInputTypeName(InputType.Radio));
                cbxTag.MergeAttribute("value", item.Value);
                cbxTag.MergeAttribute("name", fullName);
                if (item.Selected)
                {
                    cbxTag.MergeAttribute("checked", "checked");
                }
                if (!textAttributes.IsNullOrEmpty())
                {
                    foreach (var textAttr in textAttributes)
                    {
                        cbxTag.MergeAttribute(textAttr, item.Text);
                    }
                }
                if (!valueAttributes.IsNullOrEmpty())
                {
                    foreach (var valAttr in valueAttributes)
                    {
                        cbxTag.MergeAttribute(valAttr, item.Value);
                    }
                }
                radioBuilder.AppendHtml(cbxTag);
            }
            return radioBuilder;
        }

        #region Enum to radio

        /// <summary>
        /// Enum to radio
        /// </summary>
        /// <param name="htmlHelper">Html helper</param>
        /// <param name="name">Name</param>
        /// <param name="enumType">Enum type</param>
        /// <param name="rdoHtmlAttributes">Radio html attributes</param>
        /// <param name="lableHtmlAttributes">Lable html attributes</param>
        /// <param name="groupHtmlAttributes">Group html attributes</param>
        /// <param name="checkedValues">Checked values</param>
        /// <returns>Return html content</returns>
        public static IHtmlContent EnumToRadio(this IHtmlHelper htmlHelper, string name, Enum enumType, object rdoHtmlAttributes = null, object lableHtmlAttributes = null, object groupHtmlAttributes = null, string checkedValue = null)
        {
            IList<SelectListItem> items = EnumHelper.GetSelectList(enumType.GetType(), checkedValue == null ? null : new List<string>() { checkedValue });
            return RadioInternal(htmlHelper, name, items, HtmlHelper.AnonymousObjectToHtmlAttributes(rdoHtmlAttributes), HtmlHelper.AnonymousObjectToHtmlAttributes(lableHtmlAttributes), HtmlHelper.AnonymousObjectToHtmlAttributes(groupHtmlAttributes));
        }

        /// <summary>
        /// Enum to radio
        /// </summary>
        /// <param name="htmlHelper">Html helper</param>
        /// <param name="name">Name</param>
        /// <param name="enumType">Enum type</param>
        /// <param name="rdoHtmlAttributes">Radio html attributes</param>
        /// <param name="lableHtmlAttributes">Lable html attributes</param>
        /// <param name="groupHtmlAttributes">Group html attributes</param>
        /// <param name="checkedValues">Checked values</param>
        /// <returns>Return html content</returns>
        public static IHtmlContent EnumToRadio(this IHtmlHelper htmlHelper, string name, Enum enumType, object rdoHtmlAttributes = null, object lableHtmlAttributes = null, object groupHtmlAttributes = null, Enum checkedValue = null)
        {
            IList<SelectListItem> items = EnumHelper.GetSelectList(enumType.GetType(), checkedValue);
            return RadioInternal(htmlHelper, name, items, HtmlHelper.AnonymousObjectToHtmlAttributes(rdoHtmlAttributes), HtmlHelper.AnonymousObjectToHtmlAttributes(lableHtmlAttributes), HtmlHelper.AnonymousObjectToHtmlAttributes(groupHtmlAttributes));
        }

        /// <summary>
        /// Enum to radio
        /// </summary>
        /// <param name="htmlHelper">Html helper</param>
        /// <param name="expression">Name Expression</param>
        /// <param name="enumType">Enum type</param>
        /// <param name="rdoHtmlAttributes">Radio html attributes</param>
        /// <param name="lableHtmlAttributes">Lable html attributes</param>
        /// <param name="groupHtmlAttributes">Group html attributes</param>
        /// <param name="checkedValues">Checked values</param>
        /// <returns>Return html content</returns>
        public static IHtmlContent EnumToRadio<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, Enum enumType, object rdoHtmlAttributes = null, object lableHtmlAttributes = null, object groupHtmlAttributes = null, string checkedValue = null)
        {
            return EnumToRadio(htmlHelper, SixnetExpressionHelper.GetExpressionText(expression), enumType, rdoHtmlAttributes, lableHtmlAttributes, groupHtmlAttributes, checkedValue);
        }

        /// <summary>
        /// Enum to radio
        /// </summary>
        /// <param name="htmlHelper">Html helper</param>
        /// <param name="expression">Name Expression</param>
        /// <param name="enumType">Enum type</param>
        /// <param name="rdoHtmlAttributes">Radio html attributes</param>
        /// <param name="lableHtmlAttributes">Lable html attributes</param>
        /// <param name="groupHtmlAttributes">Group html attributes</param>
        /// <param name="checkedValues">Checked values</param>
        /// <returns>Return html content</returns>
        public static IHtmlContent EnumToRadio<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, Enum enumType, object rdoHtmlAttributes = null, object lableHtmlAttributes = null, object groupHtmlAttributes = null, Enum checkedValue = null)
        {
            return EnumToRadio(htmlHelper, SixnetExpressionHelper.GetExpressionText(expression), enumType, rdoHtmlAttributes, lableHtmlAttributes, groupHtmlAttributes, checkedValue);
        }

        /// <summary>
        /// Enum to radio
        /// </summary>
        /// <param name="htmlHelper">Html helper</param>
        /// <param name="name">Name</param>
        /// <param name="enumType">Enum type</param>
        /// <param name="htmlAttributes">Html attributes</param>
        /// <param name="textAttributes">Text attributes</param>
        /// <param name="valueAttributes">Value attributes</param>
        /// <returns>Return html content</returns>
        public static IHtmlContent EnumToRadio(this IHtmlHelper htmlHelper, string name, Enum enumType, object htmlAttributes = null, Enum checkedValue = null, IEnumerable<string> textAttributes = null, IEnumerable<string> valueAttributes = null)
        {
            IList<SelectListItem> items = EnumHelper.GetSelectList(enumType.GetType(), checkedValue);
            return RadioInternal(htmlHelper, name, items, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes), textAttributes, valueAttributes);
        }

        /// <summary>
        /// Enum to radio
        /// </summary>
        /// <param name="htmlHelper">Html helper</param>
        /// <param name="name">Name</param>
        /// <param name="enumType">Enum type</param>
        /// <param name="htmlAttributes">Html attributes</param>
        /// <param name="textAttributes">Text attributes</param>
        /// <param name="valueAttributes">Value attributes</param>
        /// <returns>Return html content</returns>
        public static IHtmlContent EnumToRadio<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> name, Enum enumType, object htmlAttributes = null, Enum checkedValue = null, IEnumerable<string> textAttributes = null, IEnumerable<string> valueAttributes = null)
        {
            var nameValue = SixnetExpressionHelper.GetExpressionText(name);
            return EnumToRadio(htmlHelper, nameValue, enumType, htmlAttributes, checkedValue, textAttributes, valueAttributes);
        }

        #endregion

        #region DataTable to radio

        /// <summary>
        /// DataTable to radio
        /// </summary>
        /// <param name="htmlHelper">Html helper</param>
        /// <param name="name">Name</param>
        /// <param name="dataTable">DataTable</param>
        /// <param name="optionValueFieldName">Value Field</param>
        /// <param name="optionTextFieldName">Text Field</param>
        /// <param name="cbxHtmlAttributes">Checkbox html attributes</param>
        /// <param name="lableHtmlAttributes">Lable html attributes</param>
        /// <param name="groupHtmlAttributes">Group html attributes</param>
        /// <param name="checkedValues">Checked values</param>
        /// <returns>Return html content</returns>
        public static IHtmlContent DataTableToRadio(this IHtmlHelper htmlHelper, string name, DataTable dataTable, string optionValueFieldName, string optionTextFieldName, object cbxHtmlAttributes = null, object lableHtmlAttributes = null, object groupHtmlAttributes = null, string checkedValue = null)
        {
            List<SelectListItem> listItems = new List<SelectListItem>();
            if (dataTable != null)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    string rowValue = row[optionValueFieldName].ToString();
                    listItems.Add(new SelectListItem()
                    {
                        Value = rowValue,
                        Text = row[optionTextFieldName].ToString(),
                        Selected = rowValue == checkedValue
                    });
                }
            }
            return RadioInternal(htmlHelper, name, listItems, HtmlHelper.AnonymousObjectToHtmlAttributes(cbxHtmlAttributes), HtmlHelper.AnonymousObjectToHtmlAttributes(lableHtmlAttributes), HtmlHelper.AnonymousObjectToHtmlAttributes(groupHtmlAttributes));
        }

        /// <summary>
        /// DataTable to radio
        /// </summary>
        /// <param name="htmlHelper">Html helper</param>
        /// <param name="expression">Name Expression</param>
        /// <param name="dataTable">DataTable</param>
        /// <param name="optionValueFieldName">Value Field</param>
        /// <param name="optionTextFieldName">Text Field</param>
        /// <param name="cbxHtmlAttributes">Checkbox html attributes</param>
        /// <param name="lableHtmlAttributes">Lable html attributes</param>
        /// <param name="groupHtmlAttributes">Group html attributes</param>
        /// <param name="checkedValues">Checked values</param>
        /// <returns>Return html content</returns>
        public static IHtmlContent DataTableToRadio<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, DataTable dataTable, string optionValueFieldName, string optionTextFieldName, object cbxHtmlAttributes = null, object lableHtmlAttributes = null, object groupHtmlAttributes = null, string checkedValue = null)
        {
            return DataTableToRadio(htmlHelper, SixnetExpressionHelper.GetExpressionText(expression), dataTable, optionValueFieldName, optionTextFieldName, cbxHtmlAttributes, lableHtmlAttributes, groupHtmlAttributes, checkedValue);
        }

        #endregion

        #endregion

        #region Get input type name

        /// <summary>
        /// Get input type name
        /// </summary>
        /// <param name="inputType">Input type</param>
        /// <returns>Return input type name</returns>
        private static string GetInputTypeName(InputType inputType)
        {
            switch (inputType)
            {
                case InputType.CheckBox:
                    return "checkbox";
                case InputType.Hidden:
                    return "hidden";
                case InputType.Password:
                    return "password";
                case InputType.Radio:
                    return "radio";
                case InputType.Text:
                    return "text";
                default:
                    return "text";
            }
        }

        #endregion
    }
}
