using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Framework.Serialize
{
    /// <summary>
    /// binary serialization
    /// </summary>
    public static class BinarySerialize
    {
        #region serialization an object to a string

        /// <summary>
        /// serialization an object to a string
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="obj">object</param>
        /// <returns>string value</returns>
        public static string SerializeToString<T>(T obj)
        {
            byte[] buffer = SerializeToByte<T>(obj);
            if (buffer == null)
            {
                return string.Empty;
            }
            return Convert.ToBase64String(buffer);
        }

        #endregion

        #region serialization an object to a bytes array

        /// <summary>
        /// serialization an object to a bytes array
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="obj">data object</param>
        /// <returns>array value</returns>
        public static byte[] SerializeToByte<T>(T obj)
        {
            if (obj == null)
            {
                return null;
            }
            IFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, obj);
                stream.Position = 0;
                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                stream.Flush();
                stream.Close();
                return buffer;
            }
        }

        #endregion

        #region deserialization a string to an object

        /// <summary>
        /// deserialization a string to an object
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="value">string value</param>
        /// <returns>data object</returns>
        public static T DesrializeByString<T>(string value)
        {
            T obj = default(T);
            if (string.IsNullOrWhiteSpace(value))
            {
                return default(T);
            }
            IFormatter formatter = new BinaryFormatter();
            byte[] buffer = Convert.FromBase64String(value);
            using (MemoryStream stream = new MemoryStream(buffer))
            {
                obj = (T)formatter.Deserialize(stream);
                stream.Flush();
                stream.Close();
            }
            return obj;
        }

        #endregion

        #region deserialization a bytes array to an object

        /// <summary>
        /// deserialization a bytes array to an object
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="values">bytes array</param>
        /// <returns>data object</returns>
        public static T DesrializeByByte<T>(byte[] values)
        {
            T obj = default(T);
            if (values == null || values.Length < 1)
            {
                return default(T);
            }
            IFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream(values))
            {
                obj = (T)formatter.Deserialize(stream);
                stream.Flush();
                stream.Close();
            }
            return obj;
        }

        #endregion
    }
}
