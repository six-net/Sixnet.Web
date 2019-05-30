using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Framework.Security
{
    /// <summary>
    /// MD5 encrypt
    /// </summary>
    public static class MD5
    {
        #region encrypt string by md5

        /// <summary>
        /// encrypt string by md5
        /// </summary>
        /// <param name="value">string value</param>
        /// <returns>encrypt value</returns>
        public static string Encrypt(string value)
        {
            #region varyfy args

            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            #endregion

            byte[] valueBytes = Encoding.UTF8.GetBytes(value);
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] md5Bytes = md5.ComputeHash(valueBytes);
            string encryptString = BitConverter.ToString(md5Bytes);
            return encryptString.Replace("-", string.Empty).ToLower();
        }

        #endregion

        #region encrypt string by md5 with specified times

        /// <summary>
        /// encrypt string by md5 with specified times
        /// </summary>
        /// <param name="value">string value</param>
        /// <param name="times">times</param>
        /// <returns>encrypted value</returns>
        public static string Encrypt(string value, int times)
        {
            #region verify args

            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            #endregion

            string encryptValue = string.Empty;
            do
            {
                encryptValue = Encrypt(value);
                times--;
            } while (times > 0);
            return encryptValue;
        }

        #endregion
    }
}
