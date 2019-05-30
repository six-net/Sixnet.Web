using EZNEW.Framework.ValueType;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Framework.Extension
{
    /// <summary>
    /// collection extension methods
    /// </summary>
    public static class IEnumerableExtension
    {
        #region determines whether the collection is null or empty

        ///// <summary>
        ///// determines whether the collection is null or empty
        ///// </summary>
        ///// <param name="objCollection">collection</param>
        ///// <returns>determined result</returns>
        //public static bool IsNullOrEmpty(this IEnumerable<object> objCollection)
        //{
        //    return objCollection == null || !objCollection.Any();
        //}

        ///// <summary>
        ///// determines whether the collection is null or empty
        ///// </summary>
        ///// <param name="objCollection">collection</param>
        ///// <returns>determined result</returns>
        //public static bool IsNullOrEmpty(this IEnumerable<string> objCollection)
        //{
        //    return objCollection == null || !objCollection.Any();
        //}

        ///// <summary>
        ///// determines whether the collection is null or empty
        ///// </summary>
        ///// <param name="objCollection">collection</param>
        ///// <returns>determined result</returns>
        //public static bool IsNullOrEmpty(this IEnumerable<Guid> objCollection)
        //{
        //    return objCollection == null || !objCollection.Any();
        //}

        ///// <summary>
        ///// determines whether the collection is null or empty
        ///// </summary>
        ///// <param name="objCollection">collection</param>
        ///// <returns>determined result</returns>
        //public static bool IsNullOrEmpty(this IEnumerable<int> objCollection)
        //{
        //    return objCollection == null || !objCollection.Any();
        //}

        ///// <summary>
        ///// determines whether the collection is null or empty
        ///// </summary>
        ///// <param name="objCollection">collection</param>
        ///// <returns>determined result</returns>
        //public static bool IsNullOrEmpty(this IEnumerable<long> objCollection)
        //{
        //    return objCollection == null || !objCollection.Any();
        //}

        /// <summary>
        /// determines whether the collection is null or empty
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="datas">datas</param>
        /// <returns></returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> datas)
        {
            return datas == null || !datas.Any();
        }

        #endregion

        #region determines whether the array is null or empty

        /// <summary>
        /// determines whether the array is null or empty
        /// </summary>
        /// <param name="objArray">array</param>
        /// <returns>determined result</returns>
        public static bool IsNullOrEmpty(this int[] objArray)
        {
            return objArray == null || objArray.Length < 1;
        }

        /// <summary>
        /// determines whether the array is null or empty
        /// </summary>
        /// <param name="objArray">array</param>
        /// <returns>determined result</returns>
        public static bool IsNullOrEmpty(this Guid[] objArray)
        {
            return objArray == null || objArray.Length < 1;
        }

        #endregion

        #region dictionary extension methods

        #region Dynamic

        /// <summary>
        /// set value to the dictionary,update current value if the key already exists or add if not
        /// </summary>
        /// <param name="dic">dictionary</param>
        /// <param name="name">key name</param>
        /// <param name="value">value</param>
        public static void SetValue(this IDictionary<dynamic, dynamic> dic, dynamic name, dynamic value)
        {
            if (dic == null)
            {
                return;
            }
            if (dic.ContainsKey(name))
            {
                dic[name] = value;
            }
            else
            {
                dic.Add(name, value);
            }
        }

        /// <summary>
        /// get value from the dictionary,return default value if the key doesn't exists
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="dic">dictionary</param>
        /// <param name="name">key name</param>
        /// <returns></returns>
        public static T GetValue<T>(this IDictionary<dynamic, dynamic> dic, dynamic name)
        {
            dynamic value = default(T);
            if (dic != null && dic.ContainsKey(name))
            {
                value = dic[name];
            }
            var type = typeof(T);
            var typeCode = Type.GetTypeCode(type);
            if (typeCode == TypeCode.String && value == default(T))
            {
                value = string.Empty;
            }
            return value;
        }

        #endregion

        #region Dynamic

        /// <summary>
        /// set value to the dictionary,update current value if the key already exists or add if not
        /// </summary>
        /// <param name="dic">dictionary</param>
        /// <param name="name">key name</param>
        /// <param name="value">value</param>
        public static void SetValue(this IDictionary<string, dynamic> dic, string name, dynamic value)
        {
            if (dic == null)
            {
                return;
            }
            if (dic.ContainsKey(name))
            {
                dic[name] = value;
            }
            else
            {
                dic.Add(name, value);
            }
        }

        /// <summary>
        /// get value from the dictionary,return default value if the key doesn't exists
        /// </summary>
        /// <param name="dic">dictionary</param>
        /// <param name="name">key name</param>
        /// <param name="value">value</param>
        public static void SetValue<T>(this IDictionary<string, dynamic> dic, string name, T value)
        {
            if (dic == null)
            {
                return;
            }
            dynamic realValue = value;
            var type = typeof(T);
            var typeCode = Type.GetTypeCode(type);
            if (typeCode == TypeCode.String && realValue == default(T))
            {
                realValue = string.Empty;
            }
            if (dic.ContainsKey(name))
            {
                dic[name] = realValue;
            }
            else
            {
                dic.Add(name, realValue);
            }
        }

        /// <summary>
        /// get value from the dictionary,return default value if the key doesn't exists
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="dic">dictionary</param>
        /// <param name="name">key name</param>
        /// <returns></returns>
        public static T GetValue<T>(this IDictionary<string, dynamic> dic, string name)
        {
            dynamic value = default(T);
            if (dic != null && dic.ContainsKey(name))
            {
                value = dic[name];
            }
            var type = typeof(T);
            var typeCode = Type.GetTypeCode(type);
            if (typeCode == TypeCode.String && value == default(T))
            {
                value = string.Empty;
            }
            if (!(value is T))
            {
                return DataConverter.ConvertToSimpleType<T>(value);
            }
            return value;
        }

        #endregion

        #endregion
    }
}
