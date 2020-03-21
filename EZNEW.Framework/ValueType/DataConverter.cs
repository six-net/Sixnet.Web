using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace EZNEW.Framework.ValueType
{
    /// <summary>
    /// Data Converter
    /// </summary>
    public static class DataConverter
    {
        /// <summary>
        /// convert collections to xml
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="itemName">root element name</param>
        /// <param name="modelList">data list</param>
        /// <returns>xml object</returns>
        public static object ConvertIEnumerableToXml<T>(string itemName, IEnumerable<T> modelList)
        {
            if (modelList == null)
            {
                return DBNull.Value;
            }
            using (var sw = new StringWriter())
            {
                var writer = new XmlTextWriter(sw);
                writer.WriteStartElement(itemName + "s");
                foreach (T model in modelList)
                {
                    writer.WriteStartElement(itemName);
                    var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    foreach (PropertyInfo p in properties)
                    {
                        try
                        {
                            writer.WriteAttributeString(p.Name, p.GetValue(model, null) == null ? "" : p.GetValue(model, null).ToString());
                        }
                        catch
                        {
                            writer.WriteAttributeString(p.Name, DBNull.Value.ToString(CultureInfo.InvariantCulture));
                        }
                    }
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                writer.Close();
                return sw.ToString();
            }
        }

        /// <summary>
        /// convert collections to datatable
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="modelList">data list</param>
        /// <returns>datatable</returns>
        public static DataTable ConvertIEnumerableToTable<T>(IEnumerable<T> modelList)
        {
            if (modelList == null || !modelList.Any())
            {
                return null;
            }
            DataTable table = new DataTable();
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var pro in properties)
            {
                table.Columns.Add(pro.Name);
            }
            foreach (var model in modelList)
            {
                DataRow dr = table.NewRow();
                foreach (var pro in properties)
                {
                    dr[pro.Name] = pro.GetValue(model);
                }
                table.Rows.Add(dr);
            }
            return table;
        }

        /// <summary>
        /// convert data type
        /// </summary>
        /// <typeparam name="T">target data type</typeparam>
        /// <param name="value">object</param>
        /// <returns>target data object</returns>
        public static T Convert<T>(object value)
        {
            if (value == null)
            {
                return default(T);
            }
            return Convert(value, typeof(T));
        }

        /// <summary>
        /// convert data type
        /// </summary>
        /// <param name="targetType">data type</param>
        /// <param name="value">value</param>
        /// <returns></returns>
        public static dynamic Convert(object value, Type targetType)
        {
            if (value == null)
            {
                return null;
            }
            return System.Convert.ChangeType(value, targetType);
        }

        /// <summary>
        /// determine whether is a simple data type
        /// </summary>
        /// <param name="type">data type</param>
        /// <returns>whether is simple data type</returns>
        public static bool IsSimpleType(Type type)
        {
            if (type == null)
            {
                return false;
            }
            bool simpleType = false;
            var typeCode = Type.GetTypeCode(type);
            switch (typeCode)
            {
                case TypeCode.Boolean:
                case TypeCode.Byte:
                case TypeCode.Char:
                case TypeCode.DateTime:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.String:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    simpleType = true;
                    break;
                default:
                    simpleType = false;
                    break;
            }
            return simpleType;
        }
    }
}
