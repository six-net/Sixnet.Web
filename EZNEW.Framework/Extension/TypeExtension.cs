using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Framework.Extension
{
    /// <summary>
    /// type extension methods
    /// </summary>
    public static class TypeExtension
    {
        static readonly Type AllowNullType = typeof(System.Nullable<>);

        #region generate dictionary by enum

        /// <summary>
        /// generate dictionary by enum
        /// </summary>
        /// <param name="enumType">enum type</param>
        /// <returns>dictionary</returns>
        public static Dictionary<int, string> GetEnumValueAndNames(this Type enumType)
        {
            if (enumType == null)
            {
                return new Dictionary<int, string>(0);
            }
            var values = Enum.GetValues(enumType);
            Dictionary<int, string> enumValues = new Dictionary<int, string>();
            foreach (int val in values)
            {
                enumValues.Add(val, Enum.GetName(enumType, val));
            }
            return enumValues;
        }

        #endregion

        #region allow set null value

        /// <summary>
        /// allow set null value
        /// </summary>
        /// <param name="type">type</param>
        /// <returns></returns>
        public static bool AllowNull(this Type type)
        {
            return !type.IsValueType || (type.IsGenericType && AllowNullType.Equals(type.GetGenericTypeDefinition()));
        }

        #endregion
    }
}
