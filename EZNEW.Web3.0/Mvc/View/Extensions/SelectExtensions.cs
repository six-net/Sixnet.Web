using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using EZNEW.Web.Mvc;
using EZNEW.Expressions;

namespace Microsoft.AspNetCore.Mvc.Rendering
{
    /// <summary>
    /// Select extensions
    /// </summary>
    public static class SelectExtensions
    {
        #region Enum to select

        /// <summary>
        /// Enum to select
        /// </summary>
        /// <param name="htmlHelper">Html helper</param>
        /// <param name="name">Name</param>
        /// <param name="enumType">Enum type</param>
        /// <param name="htmlAttributes">Html attributes</param>
        /// <param name="firstOptionValue">First option value</param>
        /// <param name="firstOptionText">First option text</param>
        /// <param name="selectedValue">Already selected value</param>
        /// <returns>Return html content</returns>
        public static IHtmlContent EnumToSelect(this IHtmlHelper htmlHelper, string name, Enum enumType, object htmlAttributes = null, string firstOptionValue = null, string firstOptionText = null, string selectedValue = null)
        {
            List<SelectListItem> listItems = new List<SelectListItem>();
            if (firstOptionValue != null && firstOptionText != null)
            {
                listItems.Add(new SelectListItem()
                {
                    Value = firstOptionValue,
                    Text = firstOptionText,
                    Selected = selectedValue == null ? true : selectedValue == firstOptionValue
                });
            }
            listItems.AddRange(EnumHelper.GetSelectList(enumType.GetType(), selectedValue == null ? null : new List<string>() { selectedValue }));
            var selectBuilder = new HtmlContentBuilder();
            TagBuilder selectTag = new TagBuilder("select");
            selectTag.MergeAttribute("name", name);
            selectTag.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
            foreach (var item in listItems)
            {
                TagBuilder optionTag = new TagBuilder("option");
                optionTag.MergeAttribute("value", item.Value);
                if (item.Selected)
                {
                    optionTag.MergeAttribute("selected", "selected");
                }
                if (item.Disabled)
                {
                    optionTag.MergeAttribute("disabled", "");
                }
                optionTag.InnerHtml.SetHtmlContent(item.Text);
                selectTag.InnerHtml.AppendHtml(optionTag);
            }
            selectBuilder.AppendHtml(selectTag);
            return selectBuilder;
        }

        /// <summary>
        /// Enum to select
        /// </summary>
        /// <typeparam name="TModel">Data type</typeparam>
        /// <typeparam name="TProperty">Data property</typeparam>
        /// <param name="htmlHelper">Html helper</param>
        /// <param name="property">Property</param>
        /// <param name="enumType">Enum type</param>
        /// <param name="htmlAttributes">Html attributes</param>
        /// <param name="firstOptionValue">First option value</param>
        /// <param name="firstOptionText">First option text</param>
        /// <param name="selectedValue">Selected value</param>
        /// <returns>Return html content</returns>
        public static IHtmlContent EnumToSelect<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> property, Enum enumType, object htmlAttributes = null, string firstOptionValue = null, string firstOptionText = null, string selectedValue = null)
        {
            return EnumToSelect(htmlHelper, ExpressionHelper.GetExpressionText(property), enumType, htmlAttributes, firstOptionValue, firstOptionText, selectedValue);
        }

        /// <summary>
        /// Enum to multiple select
        /// </summary>
        /// <param name="htmlHelper">Html helper</param>
        /// <param name="name">Name</param>
        /// <param name="enumType">Enum type</param>
        /// <param name="htmlAttributes">Html attributes</param>
        /// <param name="firstOptionValue">First option value</param>
        /// <param name="firstOptionText">First option text</param>
        /// <param name="selectedValues">Selected values</param>
        /// <returns>Return html content</returns>
        public static IHtmlContent EnumToMultipleSelect(this IHtmlHelper htmlHelper, string name, Enum enumType, object htmlAttributes = null, string firstOptionValue = null, string firstOptionText = null, params string[] selectedValues)
        {
            List<SelectListItem> listItems = new List<SelectListItem>();
            if (firstOptionValue != null && firstOptionText != null)
            {
                listItems.Add(new SelectListItem()
                {
                    Value = firstOptionValue,
                    Text = firstOptionText
                });
            }
            listItems.AddRange(EnumHelper.GetSelectList(enumType.GetType(), selectedValues.ToList()));
            return htmlHelper.ListBox(name, listItems, htmlAttributes);
        }

        /// <summary>
        /// Enum to multiple select
        /// </summary>
        /// <param name="htmlHelper">Html helper</param>
        /// <param name="property">Property</param>
        /// <param name="enumType">Enum type</param>
        /// <param name="htmlAttributes">Html attributes</param>
        /// <param name="firstOptionValue">First option value</param>
        /// <param name="firstOptionText">First option text</param>
        /// <param name="selectedValues">Selected values</param>
        /// <returns>Return html content</returns>
        public static IHtmlContent EnumToMultipleSelect<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> property, Enum enumType, object htmlAttributes = null, string firstOptionValue = null, string firstOptionText = null, params string[] selectedValues)
        {
            return EnumToMultipleSelect(htmlHelper, ExpressionHelper.GetExpressionText(property), enumType, htmlAttributes, firstOptionValue, firstOptionText, selectedValues);
        }

        #endregion

        #region Datatable to select

        /// <summary>
        /// DataTable to select
        /// </summary>
        /// <param name="htmlHelper">Html helper</param>
        /// <param name="name">Name</param>
        /// <param name="dataTable">Datatable</param>
        /// <param name="optionValueFieldName">Value field name</param>
        /// <param name="optionTextFieldName">Text field name</param>
        /// <param name="htmlAttributes">Html attributes</param>
        /// <param name="firstOptionValue">First option value</param>
        /// <param name="firstOptionText">First option text</param>
        /// <param name="selectedValue">Selected value</param>
        /// <returns>Return html content</returns>
        public static IHtmlContent DataTableToSelect(this IHtmlHelper htmlHelper, string name, DataTable dataTable, string optionValueFieldName, string optionTextFieldName, object htmlAttributes = null, string firstOptionValue = null, string firstOptionText = null, string selectedValue = null)
        {
            List<SelectListItem> listItems = new List<SelectListItem>();
            if (firstOptionValue != null && firstOptionText != null)
            {
                listItems.Add(new SelectListItem()
                {
                    Value = firstOptionValue,
                    Text = firstOptionText,
                    Selected = firstOptionValue == selectedValue
                });
            }
            if (dataTable != null)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    string rowValue = row[optionValueFieldName].ToString();
                    listItems.Add(new SelectListItem()
                    {
                        Value = rowValue,
                        Text = row[optionTextFieldName].ToString(),
                        Selected = rowValue == selectedValue
                    });
                }
            }
            return htmlHelper.DropDownList(name, listItems, null, htmlAttributes);
        }

        /// <summary>
        /// DataTable to select
        /// </summary>
        /// <param name="htmlHelper">Html helper</param>
        /// <param name="name">Name</param>
        /// <param name="dataTable">Datatable</param>
        /// <param name="optionValueFieldName">Value field name</param>
        /// <param name="optionTextFieldName">Text field name</param>
        /// <param name="htmlAttributes">Html attributes</param>
        /// <param name="firstOptionValue">First option value</param>
        /// <param name="firstOptionText">First option text</param>
        /// <param name="selectedValue">Selected value</param>
        /// <returns>Return html content</returns>
        public static IHtmlContent DataTableToSelect<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> name, DataTable dataTable, string optionValueFieldName, string optionTextFieldName, object htmlAttributes = null, string firstOptionValue = null, string firstOptionText = null, string selectedValue = null)
        {
            return DataTableToSelect(htmlHelper, ExpressionHelper.GetExpressionText(name), dataTable, optionValueFieldName, optionTextFieldName, htmlAttributes, firstOptionValue, firstOptionText, selectedValue);
        }

        /// <summary>
        /// DataTable to select
        /// </summary>
        /// <param name="htmlHelper">Html helper</param>
        /// <param name="name">Name</param>
        /// <param name="dataTable">Datatable</param>
        /// <param name="optionValueFieldName">Value field name</param>
        /// <param name="optionTextFieldName">Text field name</param>
        /// <param name="htmlAttributes">Html attributes</param>
        /// <param name="firstOptionValue">First option value</param>
        /// <param name="firstOptionText">First option text</param>
        /// <param name="selectedValues">Selected values</param>
        /// <returns>Return html content</returns>
        public static IHtmlContent DataTableToMultipleSelect(this IHtmlHelper htmlHelper, string name, DataTable dataTable, string optionValueFieldName, string optionTextFieldName, object htmlAttributes = null, string firstOptionValue = null, string firstOptionText = null, params string[] selectedValues)
        {
            bool hasSelected = selectedValues != null && selectedValues.Length > 0;
            List<SelectListItem> listItems = new List<SelectListItem>();
            if (firstOptionValue != null && firstOptionText != null)
            {
                listItems.Add(new SelectListItem()
                {
                    Value = firstOptionValue,
                    Text = firstOptionText,
                    Selected = hasSelected ? selectedValues.Contains(firstOptionValue) : hasSelected
                });
            }
            if (dataTable != null)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    string rowValue = row[optionValueFieldName].ToString();
                    listItems.Add(new SelectListItem()
                    {
                        Value = rowValue,
                        Text = row[optionTextFieldName].ToString(),
                        Selected = hasSelected ? selectedValues.Contains(rowValue) : hasSelected
                    });
                }
            }
            return htmlHelper.ListBox(name, listItems, htmlAttributes);
        }

        /// <summary>
        /// DataTable to select
        /// </summary>
        /// <param name="htmlHelper">Html helper</param>
        /// <param name="name">Name</param>
        /// <param name="dataTable">Datatable</param>
        /// <param name="optionValueFieldName">Value field name</param>
        /// <param name="optionTextFieldName">Text field name</param>
        /// <param name="htmlAttributes">Html attributes</param>
        /// <param name="firstOptionValue">First option value</param>
        /// <param name="firstOptionText">First option text</param>
        /// <param name="selectedValues">Selected values</param>
        /// <returns>Return html content</returns>
        public static IHtmlContent DataTableToMultipleSelect<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> name, DataTable dataTable, string optionValueFieldName, string optionTextFieldName, object htmlAttributes = null, string firstOptionValue = null, string firstOptionText = null, params string[] selectedValues)
        {
            return DataTableToMultipleSelect(htmlHelper, ExpressionHelper.GetExpressionText(name), dataTable, optionValueFieldName, optionTextFieldName, htmlAttributes, firstOptionValue, firstOptionText, selectedValues);
        }

        #endregion

        #region Datatable to tree select

        /// <summary>
        /// DataTable to tree select
        /// </summary>
        /// <param name="htmlHelper">Html helper</param>
        /// <param name="name">Name</param>
        /// <param name="dataTable">Data table</param>
        /// <param name="valueField">Value field</param>
        /// <param name="textField">Text field</param>
        /// <param name="parentField">Parent field</param>
        /// <param name="topValue">Top value</param>
        /// <param name="htmlAttributes">Html attributes</param>
        /// <param name="firstOptionValue">First option value</param>
        /// <param name="firstOptionText">First option text</param>
        /// <param name="selectedValue">Selected value</param>
        /// <returns>Return html content</returns>
        public static IHtmlContent DataTableToTreeSelect(this IHtmlHelper htmlHelper, string name, DataTable dataTable, string valueField, string textField, string parentField, string topValue, object htmlAttributes = null, string firstOptionValue = null, string firstOptionText = null, string selectedValue = null)
        {
            List<SelectListItem> itemList = new List<SelectListItem>(dataTable.Rows.Count + 1);
            if (firstOptionValue != null && firstOptionText != null)
            {
                itemList.Add(new SelectListItem()
                {
                    Text = string.Format("├{0}", firstOptionText),
                    Value = firstOptionValue,
                    Selected = firstOptionValue == selectedValue
                });
            }
            if (dataTable != null)
            {
                DataRow[] topDatas = dataTable.Select(parentField + "='" + topValue + "'");
                foreach (var dataRow in topDatas)
                {
                    string value = dataRow[valueField].ToString();
                    string text = string.Format("├{0}", dataRow[textField].ToString());
                    itemList.Add(GetNewItem(htmlHelper, text, value, value == selectedValue, 1));
                    CreateChildOption(htmlHelper, dataTable, valueField, textField, parentField, value, new List<string>() { selectedValue }, 1, itemList);
                }
            }
            return htmlHelper.DropDownList(name, itemList, null, htmlAttributes);
        }

        /// <summary>
        /// DataTable to tree select
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <typeparam name="TProperty">property</typeparam>
        /// <param name="htmlHelper">Html helper</param>
        /// <param name="name">Name</param>
        /// <param name="dataTable">Data table</param>
        /// <param name="valueField">Value field</param>
        /// <param name="textField">Text field</param>
        /// <param name="parentField">Parent field</param>
        /// <param name="topValue">Top value</param>
        /// <param name="htmlAttributes">Html attributes</param>
        /// <param name="firstOptionValue">First option value</param>
        /// <param name="firstOptionText">First option text</param>
        /// <param name="selectedValue">Selected value</param>
        /// <returns>Return html content</returns>
        public static IHtmlContent DataTableToTreeSelect<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> name, DataTable dataTable, string valueField, string textField, string parentField, string topValue, object htmlAttributes = null, string firstOptionValue = null, string firstOptionText = null, string selectedValue = null)
        {
            return DataTableToTreeSelect(htmlHelper, ExpressionHelper.GetExpressionText(name), dataTable, valueField, textField, parentField, topValue, htmlAttributes, firstOptionValue, firstOptionText, selectedValue);
        }

        /// <summary>
        /// DataTable to multiple tree select
        /// </summary>
        /// <param name="htmlHelper">Html helper</param>
        /// <param name="name">Name</param>
        /// <param name="dataTable">Data table</param>
        /// <param name="valueField">Value field</param>
        /// <param name="textField">Text field</param>
        /// <param name="parentField">Parent field</param>
        /// <param name="topValue">Top value</param>
        /// <param name="htmlAttributes">Html attributes</param>
        /// <param name="firstOptionValue">First option value</param>
        /// <param name="firstOptionText">First option text</param>
        /// <param name="selectedValues">Selected values</param>
        /// <returns>Return html content</returns>
        public static IHtmlContent DataTableToMultipleTreeSelect(this IHtmlHelper htmlHelper, string name, DataTable dataTable, string valueField, string textField, string parentField, string topValue, object htmlAttributes = null, string firstOptionValue = null, string firstOptionText = null, params string[] selectedValues)
        {
            if (dataTable == null)
            {
                return HtmlString.Empty;
            }

            List<SelectListItem> itemList = new List<SelectListItem>(dataTable.Rows.Count + 1);
            List<string> selectedValueList = selectedValues == null ? new List<string>(0) : selectedValues.ToList();
            if (firstOptionValue != null && firstOptionText != null)
            {
                itemList.Add(new SelectListItem()
                {
                    Text = string.Format("├{0}", firstOptionText),
                    Value = firstOptionValue,
                    Selected = selectedValueList.Contains(firstOptionValue)
                });
            }
            if (dataTable != null)
            {
                DataRow[] topDatas = dataTable.Select(parentField + "='" + topValue + "'");
                foreach (var dataRow in topDatas)
                {
                    string value = dataRow[valueField].ToString();
                    string text = string.Format("├{0}", dataRow[textField].ToString());
                    itemList.Add(GetNewItem(htmlHelper, text, value, selectedValueList.Contains(value), 1));
                    CreateChildOption(htmlHelper, dataTable, valueField, textField, parentField, value, selectedValueList, 1, itemList);
                }
            }
            return htmlHelper.ListBox(name, itemList, htmlAttributes);
        }

        /// <summary>
        /// Create child option
        /// </summary>
        /// <param name="htmlHelper">Html helper</param>
        /// <param name="dataTable">Data table</param>
        /// <param name="valueField">Value field</param>
        /// <param name="textField">Text field</param>
        /// <param name="parentField">Parent field</param>
        /// <param name="parentValue">Parent value</param>
        /// <param name="selectedValues">Selected values</param>
        /// <param name="parentLevel">Parent level</param>
        /// <param name="allListItems">all list items</param>
        static void CreateChildOption(IHtmlHelper htmlHelper, DataTable dataTable, string valueField, string textField, string parentField, string parentValue, List<string> selectedValues, int parentLevel, List<SelectListItem> allListItems)
        {
            DataRow[] childRows = dataTable.Select(string.Format("{0}='{1}'", parentField, parentValue));
            if (childRows == null || childRows.Length <= 0)
            {
                return;
            }
            var nowLevel = parentLevel + 1;
            foreach (DataRow dataRow in childRows)
            {
                string value = dataRow[valueField].ToString();
                string text = string.Format("{0}∟{1}", new string('　', parentLevel), dataRow[textField].ToString());
                allListItems.Add(GetNewItem(htmlHelper, text, value, selectedValues.Contains(value), nowLevel));
                CreateChildOption(htmlHelper, dataTable, valueField, textField, parentField, value, selectedValues, nowLevel, allListItems);
            }
        }

        /// <summary>
        /// Get new item
        /// </summary>
        /// <param name="htmlHelper">Html helper</param>
        /// <param name="text">Text</param>
        /// <param name="value">Value</param>
        /// <param name="selected">Selected</param>
        /// <param name="level">level</param>
        /// <returns>Return select list item</returns>
        static SelectListItem GetNewItem(IHtmlHelper htmlHelper, string text, string value, bool selected, int level)
        {
            return new SelectListItem()
            {
                Text = text,
                Value = value,
                Selected = selected
            };
        }

        #endregion

        #region Model to select

        /// <summary>
        /// Model to select
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <typeparam name="TValueProperty">Value property</typeparam>
        /// <typeparam name="TTextProperty">Text property</typeparam>
        /// <param name="htmlHelper">Html helper</param>
        /// <param name="name">Name</param>
        /// <param name="modelList">Model list</param>
        /// <param name="valueProperty">Value property</param>
        /// <param name="textProperty">Text property</param>
        /// <param name="htmlAttributes">Html attributes</param>
        /// <param name="firstOptionValue">First option value</param>
        /// <param name="firstOptionText">First option text</param>
        /// <param name="selectedValue">Selected value</param>
        /// <returns>Return html content</returns>
        public static IHtmlContent ModelListToSelect<TModel, TValueProperty, TTextProperty>(this IHtmlHelper htmlHelper, string name, IEnumerable<TModel> modelList, Expression<Func<TModel, TValueProperty>> valueProperty, Expression<Func<TModel, TTextProperty>> textProperty, object htmlAttributes, string firstOptionValue = null, string firstOptionText = null, string selectedValue = null)
        {
            List<SelectListItem> listItems = new List<SelectListItem>();
            if (firstOptionValue != null && firstOptionText != null)
            {
                listItems.Add(new SelectListItem()
                {
                    Value = firstOptionValue,
                    Text = firstOptionText,
                    Selected = firstOptionValue == selectedValue
                });
            }
            if (modelList != null)
            {
                var modelType = typeof(TModel);
                string valuePropertyName = ExpressionHelper.GetExpressionText(valueProperty);
                string textPropertyName = ExpressionHelper.GetExpressionText(textProperty);
                PropertyInfo valuePropertyInfo = modelType.GetProperty(valuePropertyName);
                PropertyInfo textPropertyInfo = modelType.GetProperty(textPropertyName);
                foreach (var model in modelList)
                {
                    string value = valuePropertyInfo.GetValue(model).ToString();
                    string text = textPropertyInfo.GetValue(model).ToString();
                    listItems.Add(new SelectListItem()
                    {
                        Value = value,
                        Text = text,
                        Selected = value == selectedValue
                    });
                }
            }
            return htmlHelper.DropDownList(name, listItems, null, htmlAttributes);
        }

        /// <summary>
        /// Model to select
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <typeparam name="TNameModel">Name model type</typeparam>
        /// <typeparam name="TNameProperty">Name property</typeparam>
        /// <typeparam name="TValueProperty">Value property</typeparam>
        /// <typeparam name="TTextProperty">Text property</typeparam>
        /// <param name="htmlHelper">Html helper</param>
        /// <param name="name">Name</param>
        /// <param name="modelList">Model list</param>
        /// <param name="valueProperty">Value property</param>
        /// <param name="textProperty">Text property</param>
        /// <param name="htmlAttributes">Html attributes</param>
        /// <param name="firstOptionValue">First option value</param>
        /// <param name="firstOptionText">First option text</param>
        /// <param name="selectedValue">Selected value</param>
        /// <returns>Return html content</returns>
        public static IHtmlContent ModelListToSelect<TModel, TNameModel, TNameProperty, TValueProperty, TTextProperty>(this IHtmlHelper<TNameModel> htmlHelper, Expression<Func<TNameModel, TNameProperty>> name, IEnumerable<TModel> modelList, Expression<Func<TModel, TValueProperty>> valueProperty, Expression<Func<TModel, TTextProperty>> textProperty, object htmlAttributes, string firstOptionValue = null, string firstOptionText = null, string selectedValue = null)
        {
            return ModelListToSelect(htmlHelper, ExpressionHelper.GetExpressionText(name), modelList, valueProperty, textProperty, htmlAttributes, firstOptionValue, firstOptionText, selectedValue);
        }

        /// <summary>
        /// Model to multiple select
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <typeparam name="TValueProperty">Value property</typeparam>
        /// <typeparam name="TTextProperty">Text property</typeparam>
        /// <param name="htmlHelper">Html helper</param>
        /// <param name="name">Name</param>
        /// <param name="modelList">Model list</param>
        /// <param name="valueProperty">Value property</param>
        /// <param name="textProperty">Text property</param>
        /// <param name="htmlAttributes">Html attributes</param>
        /// <param name="firstOptionValue">First option value</param>
        /// <param name="firstOptionText">First option text</param>
        /// <param name="selectedValues">Selected values</param>
        /// <returns>Return html content</returns>
        public static IHtmlContent ModelListToMultipleSelect<TModel, TValueProperty, TTextProperty>(this IHtmlHelper htmlHelper, string name, IEnumerable<TModel> modelList, Expression<Func<TModel, TValueProperty>> valueProperty, Expression<Func<TModel, TTextProperty>> textProperty, object htmlAttributes, string firstOptionValue = null, string firstOptionText = null, params string[] selectedValues)
        {
            List<SelectListItem> listItems = new List<SelectListItem>();
            List<string> selectedValueList = selectedValues == null ? new List<string>(0) : selectedValues.ToList();
            if (firstOptionValue != null && firstOptionText != null)
            {
                listItems.Add(new SelectListItem()
                {
                    Value = firstOptionValue,
                    Text = firstOptionText,
                    Selected = selectedValueList.Contains(firstOptionValue)
                });
            }
            if (modelList != null)
            {
                var modelType = typeof(TModel);
                string valuePropertyName = ExpressionHelper.GetExpressionText(valueProperty);
                string textPropertyName = ExpressionHelper.GetExpressionText(textProperty);
                PropertyInfo valuePropertyInfo = modelType.GetProperty(valuePropertyName);
                PropertyInfo textPropertyInfo = modelType.GetProperty(textPropertyName);
                foreach (var model in modelList)
                {
                    string value = valuePropertyInfo.GetValue(model).ToString();
                    string text = textPropertyInfo.GetValue(model).ToString();
                    listItems.Add(new SelectListItem()
                    {
                        Value = value,
                        Text = text,
                        Selected = selectedValueList.Contains(value)
                    });
                }
            }
            return htmlHelper.ListBox(name, listItems, htmlAttributes);
        }

        /// <summary>
        /// Model to multiple select
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <typeparam name="TNameModel">name model type</typeparam>
        /// <typeparam name="TNameProperty">name property</typeparam>
        /// <typeparam name="TValueProperty">Value property</typeparam>
        /// <typeparam name="TTextProperty">Text property</typeparam>
        /// <param name="htmlHelper">Html helper</param>
        /// <param name="name">Name</param>
        /// <param name="modelList">Model list</param>
        /// <param name="valueProperty">Value property</param>
        /// <param name="textProperty">Text property</param>
        /// <param name="htmlAttributes">Html attributes</param>
        /// <param name="firstOptionValue">First option value</param>
        /// <param name="firstOptionText">First option text</param>
        /// <param name="selectedValues">Selected values</param>
        /// <returns>Return html content</returns>
        public static IHtmlContent ModelListToMultipleSelect<TModel, TNameModel, TNameProperty, TValueProperty, TTextProperty>(this IHtmlHelper<TNameModel> htmlHelper, Expression<Func<TNameModel, TNameProperty>> name, IEnumerable<TModel> modelList, Expression<Func<TModel, TValueProperty>> valueProperty, Expression<Func<TModel, TTextProperty>> textProperty, object htmlAttributes, string firstOptionValue = null, string firstOptionText = null, params string[] selectedValues)
        {
            return ModelListToMultipleSelect(htmlHelper, ExpressionHelper.GetExpressionText(name), modelList, valueProperty, textProperty, htmlAttributes, firstOptionValue, firstOptionText, selectedValues);
        }

        #endregion

        #region Model to treeselect

        /// <summary>
        /// Model list to tree select
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <typeparam name="TValueProperty">Value property</typeparam>
        /// <typeparam name="TTextProperty">Text property</typeparam>
        /// <typeparam name="TParentProperty">Parent property</typeparam>
        /// <param name="htmlHelper">Html helper</param>
        /// <param name="name">Name</param>
        /// <param name="modelList">Model list</param>
        /// <param name="valueProperty">Value property</param>
        /// <param name="textProperty">Text property</param>
        /// <param name="parentProperty">Parent property</param>
        /// <param name="topValue">Top value</param>
        /// <param name="htmlAttributes">Html attributes</param>
        /// <param name="firstOptionValue">First option value</param>
        /// <param name="firstOptionText">First option text</param>
        /// <param name="selectedValue">Selected value</param>
        /// <returns>Return html content</returns>
        public static IHtmlContent ModelListToTreeSelect<TModel, TValueProperty, TTextProperty, TParentProperty>(this IHtmlHelper htmlHelper, string name, IEnumerable<TModel> modelList, Expression<Func<TModel, TValueProperty>> valueProperty, Expression<Func<TModel, TTextProperty>> textProperty, Expression<Func<TModel, TParentProperty>> parentProperty, string topValue, object htmlAttributes = null, string firstOptionValue = null, string firstOptionText = null, string selectedValue = null)
        {
            List<SelectListItem> itemList = new List<SelectListItem>();
            if (firstOptionValue != null && firstOptionText != null)
            {
                itemList.Add(new SelectListItem()
                {
                    Text = string.Format("├{0}", firstOptionText),
                    Value = firstOptionValue,
                    Selected = firstOptionValue == selectedValue
                });
            }
            var modelType = typeof(TModel);
            var valuePropertyInfo = modelType.GetProperty(ExpressionHelper.GetExpressionText(valueProperty));
            var textPropertyInfo = modelType.GetProperty(ExpressionHelper.GetExpressionText(textProperty));
            var parentPropertyInfo = modelType.GetProperty(ExpressionHelper.GetExpressionText(parentProperty));
            if (modelList != null)
            {
                IEnumerable<TModel> topModelList = modelList.Where(c => parentPropertyInfo.GetValue(c).ToString() == topValue);
                foreach (var model in topModelList)
                {
                    string value = valuePropertyInfo.GetValue(model).ToString();
                    string text = string.Format("├{0}", textPropertyInfo.GetValue(model).ToString());
                    itemList.Add(GetNewItem(htmlHelper, text, value, value == selectedValue, 1));
                    CreateChildOption(htmlHelper, modelList, valuePropertyInfo, textPropertyInfo, parentPropertyInfo, value, new List<string>() { selectedValue }, 1, itemList);
                }
            }
            return htmlHelper.DropDownList(name, itemList, null, htmlAttributes);
        }

        /// <summary>
        /// Model list to multiple tree select
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <typeparam name="TValueProperty">Value property</typeparam>
        /// <typeparam name="TTextProperty">Text property</typeparam>
        /// <typeparam name="TParentProperty">Parent property</typeparam>
        /// <param name="htmlHelper">Html helper</param>
        /// <param name="name">Name</param>
        /// <param name="modelList">Model list</param>
        /// <param name="valueProperty">Value property</param>
        /// <param name="textProperty">Text property</param>
        /// <param name="parentProperty">Parent property</param>
        /// <param name="topValue">Top value</param>
        /// <param name="htmlAttributes">Html attributes</param>
        /// <param name="firstOptionValue">First option value</param>
        /// <param name="firstOptionText">First option text</param>
        /// <param name="selectedValues">Selected values</param>
        /// <returns>Return html content</returns>
        public static IHtmlContent ModelListToMultipleTreeSelect<TModel, TValueProperty, TTextProperty, TParentProperty>(this IHtmlHelper htmlHelper, string name, IEnumerable<TModel> modelList, Expression<Func<TModel, TValueProperty>> valueProperty, Expression<Func<TModel, TTextProperty>> textProperty, Expression<Func<TModel, TParentProperty>> parentProperty, string topValue, object htmlAttributes = null, string firstOptionValue = null, string firstOptionText = null, params string[] selectedValues)
        {
            List<SelectListItem> itemList = new List<SelectListItem>();
            List<string> selectedValueList = selectedValues == null ? new List<string>(0) : selectedValues.ToList();
            if (firstOptionValue != null && firstOptionText != null)
            {
                itemList.Add(new SelectListItem()
                {
                    Text = string.Format("├{0}", firstOptionText),
                    Value = firstOptionValue,
                    Selected = selectedValueList.Contains(firstOptionValue)
                });
            }
            var modelType = typeof(TModel);
            var valuePropertyInfo = modelType.GetProperty(ExpressionHelper.GetExpressionText(valueProperty));
            var textPropertyInfo = modelType.GetProperty(ExpressionHelper.GetExpressionText(textProperty));
            var parentPropertyInfo = modelType.GetProperty(ExpressionHelper.GetExpressionText(parentProperty));
            if (modelList != null)
            {
                IEnumerable<TModel> topModelList = modelList.Where(c => parentPropertyInfo.GetValue(c).ToString() == topValue);
                foreach (var model in topModelList)
                {
                    string value = valuePropertyInfo.GetValue(model).ToString();
                    string text = string.Format("├{0}", textPropertyInfo.GetValue(model).ToString());
                    itemList.Add(GetNewItem(htmlHelper, text, value, selectedValueList.Contains(value), 1));
                    CreateChildOption(htmlHelper, modelList, valuePropertyInfo, textPropertyInfo, parentPropertyInfo, value, selectedValueList, 1, itemList);
                }
            }
            return htmlHelper.ListBox(name, itemList, htmlAttributes);
        }

        /// <summary>
        /// Create child option
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <param name="htmlHelper">Html helper</param>
        /// <param name="modelList">Model list</param>
        /// <param name="valueProperty">Value property</param>
        /// <param name="textProperty">Text property</param>
        /// <param name="parentProperty">Parent property</param>
        /// <param name="parentValue">Parent value</param>
        /// <param name="selectedValues">Selected values</param>
        /// <param name="parentLevel">Parent level</param>
        /// <param name="allListItems">all list items</param>
        static void CreateChildOption<TModel>(IHtmlHelper htmlHelper, IEnumerable<TModel> modelList, PropertyInfo valueProperty, PropertyInfo textProperty, PropertyInfo parentProperty, string parentValue, List<string> selectedValues, int parentLevel, List<SelectListItem> allListItems)
        {
            IEnumerable<TModel> childModelList = modelList.Where(c => parentProperty.GetValue(c).ToString() == parentValue);
            var nowLevel = parentLevel + 1;
            foreach (var childModel in childModelList)
            {
                string value = valueProperty.GetValue(childModel).ToString();
                string text = string.Format("{0}∟{1}", new string('　', parentLevel), textProperty.GetValue(childModel).ToString());
                allListItems.Add(GetNewItem(htmlHelper, text, value, selectedValues.Contains(value), nowLevel));
                CreateChildOption(htmlHelper, modelList, valueProperty, textProperty, parentProperty, value, selectedValues, nowLevel, allListItems);
            }
        }

        #endregion
    }
}
