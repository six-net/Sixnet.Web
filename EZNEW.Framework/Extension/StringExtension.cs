using EZNEW.Framework.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EZNEW.Framework.Extension
{
    /// <summary>
    /// string extension methods
    /// </summary>
    public static class StringExtension
    {

        #region encrypy string by MD5

        /// <summary>
        /// encrypy a string by MD5
        /// </summary>
        /// <param name="value">string value</param>
        /// <returns>encrypy value</returns>
        public static string MD5(this string value)
        {
            return Security.MD5.Encrypt(value);
        }

        #endregion

        #region split a string to a string array

        /// <summary>
        /// split a string to a string array
        /// </summary>
        /// <param name="stringValue">string value</param>
        /// <param name="splitString">split string</param>
        /// <returns>string array</returns>
        public static string[] LSplit(this string stringValue, string splitString)
        {
            if (string.IsNullOrWhiteSpace(stringValue))
            {
                return new string[0];
            }
            if (string.IsNullOrWhiteSpace(splitString))
            {
                return new string[] { stringValue };
            }
            return stringValue.Split(new string[] { splitString }, StringSplitOptions.RemoveEmptyEntries);
        }

        #endregion

        #region determine a string whether is null or empty

        /// <summary>
        /// determine a string whether is null or empty
        /// </summary>
        /// <param name="stringValue">string value</param>
        /// <returns>determined value</returns>
        public static bool IsNullOrEmpty(this string stringValue)
        {
            return string.IsNullOrWhiteSpace(stringValue);
        }

        #endregion

        #region substring

        /// <summary>
        /// cut out a string value with the specified length then append the specified chars at the end
        /// </summary>
        /// <param name="value">string value</param>
        /// <param name="length">substring length</param>
        /// <param name="sig">end string</param>
        /// <returns>substring</returns>
        private static string LSubString(this string value, int length, string sig = "")
        {
            if (value.IsNullOrEmpty())
            {
                return string.Empty;
            }
            int stringLength = value.Length;
            if (stringLength <= length)
            {
                return value;
            }
            Regex reg = new Regex(@"[^\x00-\xff]");
            int subStringLength = 0;
            int singleCharLength = 0;
            int doubleCharLength = 0;
            for (int i = 0; i < stringLength; i++)
            {
                char nowChar = value[i];
                if (reg.IsMatch(nowChar.ToString()))
                {
                    subStringLength++;
                    doubleCharLength++;
                }
                else
                {
                    subStringLength += 1;
                    singleCharLength++;
                }
                if (((singleCharLength / 2) + doubleCharLength) == length)
                {
                    break;
                }
            }
            subStringLength = subStringLength > stringLength ? stringLength : subStringLength;
            return value.Substring(0, subStringLength) + sig;
        }

        #endregion

        #region return the string value first char

        /// <summary>
        /// return the string value first char
        /// </summary>
        /// <param name="value">string value</param>
        /// <returns>first char</returns>
        public static string FirstChar(this string value)
        {
            #region verify args

            if (value.IsNullOrEmpty())
            {
                return string.Empty;
            }

            #endregion

            char firstCode = value[0];
            if (firstCode.IsChineseLetter())
            {
                return firstCode.ChineseFirtChar();
            }
            return firstCode.ToString();
        }

        #endregion

        #region return the chinese char first letter

        /// <summary>
        /// return the chinese char first letter
        /// </summary>
        /// <param name="value">chinese char</param>
        /// <returns>first letter</returns>
        public static string ChineseFirtChar(this char value)
        {
            string firstCode = string.Empty;
            byte[] valueBytes = Encoding.Default.GetBytes(value.ToString());
            if (valueBytes != null && valueBytes.Length == 2)
            {
                int byteOneValue = (int)(valueBytes[0]);
                int byteTwoValue = (int)(valueBytes[1]);
                int chineseValue = byteOneValue * 256 + byteTwoValue;
                // 'A'; 45217..45252  'B'; 45253..45760 'C'; 45761..46317  'D'; 46318..46825 'E'; 46826..47009 'F'; 47010..47296  'G'; 47297..47613
                // 'H'; 47614..48118 'J'; 48119..49061 'K'; 49062..49323 'L'; 49324..49895 'M'; 49896..50370 'N'; 50371..50613  
                // 'O'; 50614..50621 'P'; 50622..50905 'Q'; 50906..51386 'R'; 51387..51445 'S'; 51446..52217  'T'; 52218..52697  
                // not have U,V
                // 'W'; 52698..52979  'X'; 52980..53640  'Y'; 53689..54480 'Z'; 54481..55289
                if (chineseValue < 45217 || chineseValue > 55289)
                {
                    return string.Empty;
                }
                if (chineseValue <= 45252)
                {
                    firstCode = "A";
                }
                else if (chineseValue <= 45760)
                {
                    firstCode = "B";
                }
                else if (chineseValue <= 46317)
                {
                    firstCode = "C";
                }
                else if (chineseValue <= 46825)
                {
                    firstCode = "D";
                }
                else if (chineseValue <= 47009)
                {
                    firstCode = "E";
                }
                else if (chineseValue <= 47296)
                {
                    firstCode = "F";
                }
                else if (chineseValue <= 47613)
                {
                    firstCode = "G";
                }
                else if (chineseValue <= 48118)
                {
                    firstCode = "H";
                }
                else if (chineseValue <= 49061)
                {
                    firstCode = "J";
                }
                else if (chineseValue <= 49323)
                {
                    firstCode = "K";
                }
                else if (chineseValue <= 49895)
                {
                    firstCode = "L";
                }
                else if (chineseValue <= 50370)
                {
                    firstCode = "M";
                }
                else if (chineseValue <= 50613)
                {
                    firstCode = "N";
                }
                else if (chineseValue <= 50621)
                {
                    firstCode = "O";
                }
                else if (chineseValue <= 50905)
                {
                    firstCode = "P";
                }
                else if (chineseValue <= 51386)
                {
                    firstCode = "Q";
                }
                else if (chineseValue <= 51445)
                {
                    firstCode = "R";
                }
                else if (chineseValue <= 52217)
                {
                    firstCode = "S";
                }
                else if (chineseValue <= 52697)
                {
                    firstCode = "T";
                }
                else if (chineseValue <= 52979)
                {
                    firstCode = "W";
                }
                else if (chineseValue <= 53640)
                {
                    firstCode = "X";
                }
                else if (chineseValue <= 54480)
                {
                    firstCode = "Y";
                }
                else if (chineseValue <= 55289)
                {
                    firstCode = "Z";
                }
            }

            return firstCode;
        }

        #endregion

        #region determine whether is a chinese char

        /// <summary>
        /// determine whether is a chinese char
        /// </summary>
        /// <param name="value">char value</param>
        /// <returns>is chinese char or not</returns>
        public static bool IsChineseLetter(this char value)
        {
            Regex chineseRex = new Regex("^[\u4e00-\u9fa5]$");
            return chineseRex.IsMatch(value.ToString());
        }

        #endregion

        #region determine whether all of char value in the string are chinese

        /// <summary>
        /// etermine whether all of char value in the string are chinese
        /// </summary>
        /// <param name="value">string value</param>
        /// <returns>all of them is chinese or not</returns>
        public static bool IsAllChineseLetter(this string value)
        {
            #region verify args

            if (value.IsNullOrEmpty())
            {
                return false;
            }

            #endregion

            return value.All(c => c.IsChineseLetter());
        }

        #endregion

        #region remove html tags

        /// <summary>
        /// remove all of the html tags in the string value
        /// </summary>
        /// <param name="value">string value</param>
        /// <returns>removed string</returns>
        public static string RemoveHtml(this string value)
        {
            //删除脚本
            value = Regex.Replace(value, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
            //删除HTML
            value = Regex.Replace(value, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
            value = Regex.Replace(value, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
            value = Regex.Replace(value, @"-->", "", RegexOptions.IgnoreCase);
            value = Regex.Replace(value, @"<!--.*", "", RegexOptions.IgnoreCase);
            value = Regex.Replace(value, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
            value = Regex.Replace(value, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
            value = Regex.Replace(value, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
            value = Regex.Replace(value, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
            value = Regex.Replace(value, @"&(nbsp|#160);", " ", RegexOptions.IgnoreCase);
            value = Regex.Replace(value, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
            value = Regex.Replace(value, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
            value = Regex.Replace(value, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
            value = Regex.Replace(value, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
            value = Regex.Replace(value, @"&#(\d+);", "", RegexOptions.IgnoreCase);
            value.Replace("<", "");
            value.Replace(">", "");
            value.Replace("\r\n", "");
            //value = HttpContext.Current.Server.HtmlEncode(value).Trim();
            return value;
        }

        #endregion

        #region encrypt string

        /// <summary>
        /// encrypt string
        /// </summary>
        /// <param name="value">string value</param>
        /// <returns>encrypted value</returns>
        public static string Encrypt(this string value)
        {
            return EncryptionHelper.Encrypt(value);
        }

        #endregion

        #region decrypt string

        /// <summary>
        /// decrypt string
        /// </summary>
        /// <param name="value">encrypted value</param>
        /// <returns>string value</returns>
        public static string Decrypt(this string value)
        {
            return EncryptionHelper.Decrypt(value);
        }

        #endregion

        #region replace by regex

        /// <summary>
        /// replace string value by regex
        /// </summary>
        /// <param name="value"></param>
        /// <param name="pattern"></param>
        /// <param name="replaceValue"></param>
        /// <returns></returns>
        public static string ReplaceByRegex(this string value, string pattern, string replaceValue)
        {
            if (value.IsNullOrEmpty() || pattern.IsNullOrEmpty())
            {
                return value;
            }
            Regex reg = new Regex(pattern);
            return reg.Replace(value, replaceValue);
        }

        #endregion

        #region string to binary

        /// <summary>
        /// convert string value to binary string
        /// </summary>
        /// <param name="value">original value</param>
        /// <returns></returns>
        public static string ToBinary(this string value)
        {
            if (value == null)
            {
                return null;
            }
            byte[] data = Encoding.Unicode.GetBytes(value);
            StringBuilder result = new StringBuilder(data.Length * 8);
            foreach (byte b in data)
            {
                result.Append(Convert.ToString(b, 2).PadLeft(8, '0'));
            }
            return result.ToString();
        }

        #endregion

        #region binary to string

        /// <summary>
        /// binary to string
        /// </summary>
        /// <param name="binaryString">binary string</param>
        /// <returns></returns>
        public static string ToOriginalString(this string binaryString)
        {
            if (binaryString.IsNullOrEmpty())
            {
                return string.Empty;
            }
            CaptureCollection captures = Regex.Match(binaryString, @"([01]{8})+").Groups[1].Captures;
            byte[] data = new byte[captures.Count];
            for (int i = 0; i < captures.Count; i++)
            {
                data[i] = Convert.ToByte(captures[i].Value, 2);
            }
            return Encoding.Unicode.GetString(data, 0, data.Length);
        }

        #endregion
    }
}
