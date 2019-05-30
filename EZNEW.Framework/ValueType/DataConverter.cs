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
        public static T ConvertToSimpleType<T>(object value)
        {
            if (value == null)
            {
                return default(T);
            }
            var type = typeof(T);
            var typeCode = Type.GetTypeCode(type);
            var stringValue = value.ToString();
            dynamic data = default(T);
            switch (typeCode)
            {
                case TypeCode.Boolean:
                    bool boolVal = false;
                    if (bool.TryParse(stringValue, out boolVal))
                    {
                        data = boolVal;
                    }
                    break;
                case TypeCode.Byte:
                    byte byteVal = 0;
                    if (byte.TryParse(stringValue, out byteVal))
                    {
                        data = byteVal;
                    }
                    break;
                case TypeCode.Char:
                    char charVal;
                    if (Char.TryParse(stringValue, out charVal))
                    {
                        data = charVal;
                    }
                    break;
                case TypeCode.DateTime:
                    DateTime datetimeVal;
                    if (DateTime.TryParse(stringValue, out datetimeVal))
                    {
                        data = datetimeVal;
                    }
                    break;
                case TypeCode.Decimal:
                    Decimal decimalVal = 0.00M;
                    if (Decimal.TryParse(stringValue, out decimalVal))
                    {
                        data = decimalVal;
                    }
                    break;
                case TypeCode.Double:
                    Double doubleVal = 0.00;
                    if (Double.TryParse(stringValue, out doubleVal))
                    {
                        data = doubleVal;
                    }
                    break;
                case TypeCode.Int16:
                    Int16 int16Val = 0;
                    if (Int16.TryParse(stringValue, out int16Val))
                    {
                        data = int16Val;
                    }
                    break;
                case TypeCode.Int32:
                    Int32 int32Val = 0;
                    if (Int32.TryParse(stringValue, out int32Val))
                    {
                        data = int32Val;
                    }
                    break;
                case TypeCode.Int64:
                    Int64 int64Val = 0;
                    if (Int64.TryParse(stringValue, out int64Val))
                    {
                        data = int64Val;
                    }
                    break;
                case TypeCode.SByte:
                    SByte sbyteVal = 0;
                    if (SByte.TryParse(stringValue, out sbyteVal))
                    {
                        data = sbyteVal;
                    }
                    break;
                case TypeCode.Single:
                    Single singleVal = 0;
                    if (Single.TryParse(stringValue, out singleVal))
                    {
                        data = singleVal;
                    }
                    break;
                case TypeCode.String:
                    data = stringValue;
                    break;
                case TypeCode.UInt16:
                    UInt16 uint16Val = 0;
                    if (UInt16.TryParse(stringValue, out uint16Val))
                    {
                        data = uint16Val;
                    }
                    break;
                case TypeCode.UInt32:
                    UInt32 uint32Val = 0;
                    if (UInt32.TryParse(stringValue, out uint32Val))
                    {
                        data = uint32Val;
                    }
                    break;
                case TypeCode.UInt64:
                    UInt64 uint64Val = 0;
                    if (UInt64.TryParse(stringValue, out uint64Val))
                    {
                        data = uint64Val;
                    }
                    break;
            }
            return data;
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
