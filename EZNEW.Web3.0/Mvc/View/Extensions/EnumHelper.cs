using System;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EZNEW.Web.Mvc
{
    public static class EnumHelper
    {
        #region GetSelectList

        /// <summary>
        /// Gets a list of <see cref="SelectListItem"/> objects corresponding to enum constants defined in the given
        /// <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to evaluate.</param>
        /// <returns> An <see cref="List{SelectListItem}"/> for the given <paramref name="type"/>.</returns>
        /// <remarks>
        /// Throws if <see cref="IsValidForEnumHelper(Type)"/> returns <see langref="false"/> for the given
        /// <paramref name="type"/>.
        /// </remarks>
        public static List<SelectListItem> GetSelectList(Type type, params Enum[] currentValues)
        {
            List<string> currentValueStringList = null;
            if (currentValues != null && currentValues.Length > 0)
            {
                currentValueStringList = new List<string>(currentValues.Length);
                foreach (var currentValue in currentValues)
                {
                    if (currentValue == null)
                    {
                        continue;
                    }
                    var currentValueArray = currentValue.ToString().LSplit(",");
                    foreach (var val in currentValueArray)
                    {
                        var enumVal = Enum.Parse(type, val);
                        currentValueStringList.Add(((int)enumVal).ToString());
                    }
                }
            }
            return GetSelectList(type, currentValueStringList);
        }

        public static List<SelectListItem> GetSelectList(Type type, List<string> selectedValues)
        {
            if (type == null)
            {
                return new List<SelectListItem>(0);
            }

            if (!IsValidForEnumHelper(type))
            {
                throw new ArgumentException("type is not a enum");
            }
            bool hasSelectedValue = selectedValues != null && selectedValues.Count > 0;
            List<SelectListItem> selectList = new List<SelectListItem>();
            Type checkedType = Nullable.GetUnderlyingType(type) ?? type;
            const BindingFlags BindingFlags = BindingFlags.DeclaredOnly | BindingFlags.GetField | BindingFlags.Public | BindingFlags.Static;
            foreach (FieldInfo field in checkedType.GetFields(BindingFlags))
            {
                string fieldValue = field.GetRawConstantValue().ToString();
                selectList.Add(new SelectListItem
                {
                    Text = GetDisplayName(field),
                    Value = fieldValue,
                    Selected = hasSelectedValue ? selectedValues.Contains(fieldValue) : hasSelectedValue
                });
            }
            return selectList;
        }

        #endregion

        #region IsValidForEnumHelper

        public static bool IsValidForEnumHelper(Type type)
        {
            bool isValid = false;
            if (type != null)
            {
                // Type.IsEnum is false for Nullable<T> even if T is an enum.  Check underlying type (if any).
                // Do not support Enum type itself -- IsEnum property is false for that class.
                Type checkedType = Nullable.GetUnderlyingType(type) ?? type;
                if (checkedType.IsEnum)
                {
                    isValid = true; //!HasFlagsInternal(checkedType);
                }
            }

            return isValid;
        }

        #endregion

        #region HasFlagsInternal

        public static bool HasFlagsInternal(Type type)
        {
            FlagsAttribute attribute = type.GetCustomAttribute<FlagsAttribute>(inherit: false);
            return attribute != null;
        }

        #endregion

        #region GetDisplayName

        // Return non-empty name specified in a [Display] attribute for the given field, if any; field's name otherwise
        private static string GetDisplayName(FieldInfo field)
        {
            DisplayAttribute display = field.GetCustomAttribute<DisplayAttribute>(inherit: false);
            if (display != null)
            {
                string name = display.GetName();
                if (!string.IsNullOrEmpty(name))
                {
                    return name;
                }
            }

            return field.Name;
        }

        #endregion
    }
}
