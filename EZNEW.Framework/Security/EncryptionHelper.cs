using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Framework.Security
{
    /// <summary>
    /// encrypy/decrypt
    /// </summary>
    public static class EncryptionHelper
    {
        static EncryptionHelper()
        {
            rm = new RijndaelManaged()
            {
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7
            };
            encoding = Encoding.UTF8;
            string securityKey = "~!wb#c*&^%$.com]";
            rm.Key = encoding.GetBytes(securityKey);
            rm.BlockSize = 128;
            rm.IV = rm.Key;
        }

        #region Propertys

        static RijndaelManaged rm = null;
        static Encoding encoding = null;

        /// <summary>
        /// get or set SecretKey
        /// </summary>
        public static string SecretKey
        {
            get
            {
                return encoding.GetString(rm.Key);
            }
            set
            {
                rm.Key = encoding.GetBytes(value);
                rm.BlockSize = 128;
                rm.IV = rm.Key;
            }
        }

        #endregion

        #region encrypt string

        /// <summary>
        /// encrypt string
        /// </summary>
        /// <param name="value">string value</param>
        /// <returns>encrypt value</returns>
        public static string Encrypt(this string value)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    return string.Empty;
                }
                if (rm.Key == null)
                {
                    return value;
                }
                string encryptValue = string.Empty;
                using (ICryptoTransform ict = rm.CreateEncryptor())
                {
                    byte[] valueArray = encoding.GetBytes(value);
                    byte[] resultArray = ict.TransformFinalBlock(valueArray, 0, valueArray.Length);
                    encryptValue = Convert.ToBase64String(resultArray);
                    return encryptValue;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region decrypy string

        /// <summary>
        /// decrypy a string
        /// </summary>
        /// <param name="value">encrypied string value</param>
        /// <returns>decrypied string value</returns>
        public static string Decrypt(this string value)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    return string.Empty;
                }
                if (rm.Key == null)
                {
                    return value;
                }
                string decryptValue = string.Empty;
                byte[] decryptArray = Convert.FromBase64String(value);
                using (ICryptoTransform ict = rm.CreateDecryptor())
                {
                    byte[] resultArray = ict.TransformFinalBlock(decryptArray, 0, decryptArray.Length);
                    decryptValue = encoding.GetString(resultArray);
                    return decryptValue;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}
