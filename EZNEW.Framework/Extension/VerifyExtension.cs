using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EZNEW.Framework.Extension
{
    /// <summary>
    /// verify extension
    /// </summary>
    public static class VerifyExtension
    {
        #region verify whether is integer

        /// <summary>
        /// verify whether is interger
        /// </summary>
        /// <param name="value">verify value</param>
        /// <returns>whether is interger</returns>
        public static bool IsInt(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }
            string intRex = "^-?[1-9]\\d*$";
            Regex intRegex = new Regex(intRex);
            return intRegex.IsMatch(value);
        }

        #endregion

        #region verify whether is positive integer

        /// <summary>
        /// verify whether is positive integer
        /// </summary>
        /// <param name="value">verify value</param>
        /// <returns>is positive integer</returns>
        public static bool IsPositiveInt(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }
            string intRex = "^\\d+$";
            Regex intRegex = new Regex(intRex);
            return intRegex.IsMatch(value);
        }

        #endregion

        #region verify whether is negative integer

        /// <summary>
        /// verify whether is negative integer
        /// </summary>
        /// <param name="value">verify value</param>
        /// <returns>is negative integer</returns>
        public static bool IsNegativeInt(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }
            string intRex = "^-[1-9]\\d*$";
            Regex intRegex = new Regex(intRex);
            return intRegex.IsMatch(value);
        }

        #endregion

        #region verify whether is number

        /// <summary>
        /// verify whether is number
        /// </summary>
        /// <param name="value">verify value</param>
        /// <returns>is number</returns>
        public static bool IsNumber(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }
            string intRex = "^([+-]?)\\d*\\.?\\d+$";
            Regex intRegex = new Regex(intRex);
            return intRegex.IsMatch(value);
        }

        #endregion

        #region verify whether is positive integer or 0

        /// <summary>
        /// verify whether is positive integer or 0
        /// </summary>
        /// <param name="value">verify</param>
        /// <returns>is positive integer or 0</returns>
        public static bool IsPositiveIntOrZero(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }
            string intRex = "^[1-9]\\d*|0$";
            Regex intRegex = new Regex(intRex);
            return intRegex.IsMatch(value);
        }

        #endregion

        #region verify whether is negative integer or 0

        /// <summary>
        /// verify whether is negative integer or 0
        /// </summary>
        /// <param name="value">verify value</param>
        /// <returns>is negative integer or 0</returns>
        public static bool IsNegativeIntOrZero(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }
            string intRex = "^-[1-9]\\d*|0$";
            Regex intRegex = new Regex(intRex);
            return intRegex.IsMatch(value);
        }

        #endregion

        #region verify whether is a number with fractional digits

        /// <summary>
        /// verify whether is a number with fractional digits
        /// </summary>
        /// <param name="value">verify value</param>
        /// <returns>is fraction</returns>
        public static bool IsFloat(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }
            string intRex = "^([+-]?)\\d*\\.\\d+$";
            Regex intRegex = new Regex(intRex);
            return intRegex.IsMatch(value);
        }

        #endregion

        #region verify whether is a positive fraction

        /// <summary>
        /// verify whether is a positive fraction
        /// </summary>
        /// <param name="value">verify value</param>
        /// <returns>is positive fraction</returns>
        public static bool IsPositiveFloat(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }
            string intRex = "^[1-9]\\d*.\\d*|0.\\d*[1-9]\\d*$";
            Regex intRegex = new Regex(intRex);
            return intRegex.IsMatch(value);
        }

        #endregion

        #region verify whether is a negative fraction

        /// <summary>
        /// verify whether is a negative fraction
        /// </summary>
        /// <param name="value">verify value</param>
        /// <returns>is negative fraction</returns>
        public static bool IsNegativeFloat(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }
            string intRex = "-([1-9]\\d*.\\d*|0.\\d*[1-9]\\d*)$";
            Regex intRegex = new Regex(intRex);
            return intRegex.IsMatch(value);
        }

        #endregion

        #region verify whether is a positive fraction or 0

        /// <summary>
        /// verify whether is a positive fraction or 0
        /// </summary>
        /// <param name="value">verify value</param>
        /// <returns>is positive fraction or zero</returns>
        public static bool IsPositiveFloatOrZero(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }
            string intRex = "^[1-9]\\d*.\\d*|0.\\d*[1-9]\\d*|0?.0+|0$";
            Regex intRegex = new Regex(intRex);
            return intRegex.IsMatch(value);
        }

        #endregion

        #region verify whether is a positive fraction or 0,allow null

        /// <summary>
        /// verify whether is a positive fraction or 0,allow null
        /// </summary>
        /// <param name="value">verify value</param>
        /// <returns>is null or positive fraction or 0</returns>
        public static bool IsPositiveFloatOrZeroOrNull(this double? value)
        {
            if (!value.HasValue)
            {
                return true;
            }
            return IsPositiveFloatOrZero(value.ToString());
        }

        #endregion

        #region verify whether is a negative fraction or 0,allow null

        /// <summary>
        /// verify whether is a negative fraction or 0,allow null
        /// </summary>
        /// <param name="value">verify value</param>
        /// <returns>is negative fraction or zero or null</returns>
        public static bool IsNegativeFloatOrZeroOrNull(this double? value)
        {
            if (!value.HasValue)
            {
                return false;
            }
            return IsNegativeIntOrZero(value.TooString());
        }

        #endregion

        #region verify whether is a negative fraction or zero

        /// <summary>
        /// verify whether is a negative fraction or zero
        /// </summary>
        /// <param name="value">verify</param>
        /// <returns>is negative fraction or zero</returns>
        public static bool IsNegativeFloatOrZero(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }
            string intRex = "^(-([1-9]\\d*.\\d*|0.\\d*[1-9]\\d*))|0?.0+|0$";
            Regex intRegex = new Regex(intRex);
            return intRegex.IsMatch(value);
        }

        #endregion

        #region verify whether is email address

        /// <summary>
        /// verify whether is email address
        /// </summary>
        /// <param name="value">verify value</param>
        /// <returns>is email address</returns>
        public static bool IsEmail(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }
            string intRex = "^\\w+((-\\w+)|(\\.\\w+))*\\@[A-Za-z0-9]+((\\.|-)[A-Za-z0-9]+)*\\.[A-Za-z0-9]+$";
            Regex intRegex = new Regex(intRex);
            return intRegex.IsMatch(value);
        }

        #endregion

        #region verify whether is email address or null

        /// <summary>
        /// verify whether is email address or null
        /// </summary>
        /// <param name="value">verify value</param>
        /// <returns>is email address or null</returns>
        public static bool IsEmailAllowNull(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return true;
            }
            return IsEmail(value);

        }

        #endregion

        #region verify whether is color

        /// <summary>
        ///  verify whether is color
        /// </summary>
        /// <param name="value">verify value</param>
        /// <returns>is color</returns>
        public static bool IsColor(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }
            string intRex = "^#[a-fA-F0-9]{6}$";
            Regex intRegex = new Regex(intRex);
            return intRegex.IsMatch(value);
        }

        #endregion

        #region verify whether is color or null

        /// <summary>
        /// verify whether is color or null
        /// </summary>
        /// <param name="value">verify value</param>
        /// <returns>is color or null</returns>
        public static bool IsColorAllowNull(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return true;
            }
            return IsColor(value);
        }

        #endregion

        #region verify whether is url

        /// <summary>
        /// verify whether is url
        /// </summary>
        /// <param name="value">verify value</param>
        /// <returns>is url</returns>
        public static bool IsUrl(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }
            string intRex = "^http[s]?:\\/\\/([\\w-]+\\.)+[\\w-]+([\\w-./?%&=]*)?$";
            Regex intRegex = new Regex(intRex);
            return intRegex.IsMatch(value);
        }

        #endregion

        #region verify whether is url,allow null

        /// <summary>
        /// verify whether is url,allow null
        /// </summary>
        /// <param name="value">verify value</param>
        /// <returns>is url or null</returns>
        public static bool IsUrlAllowNull(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return true;
            }
            return IsUrl(value);
        }

        #endregion

        #region verify whether has chinese char

        /// <summary>
        /// verify whether is has chinese char
        /// </summary>
        /// <param name="value">verify value</param>
        /// <returns>has chinese char</returns>
        public static bool IsContainChinese(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }
            string intRex = "^[\\u4E00-\\u9FA5\\uF900-\\uFA2D]+$";
            Regex intRegex = new Regex(intRex);
            return intRegex.IsMatch(value);
        }

        #endregion

        #region verify whether has chinese char,allow null

        /// <summary>
        /// verify whether has chinese char,allow null
        /// </summary>
        /// <param name="value">verify value</param>
        /// <returns>has chinese or null</returns>
        public static bool IsChineseAllowNull(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return true;
            }
            return IsContainChinese(value);
        }

        #endregion

        #region verify whether is postcode

        /// <summary>
        /// verify whether is postcode
        /// </summary>
        /// <param name="value">verify value</param>
        /// <returns>is post code</returns>
        public static bool IsZipCode(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }
            string intRex = "^[1-9][0-9]{5}$";
            Regex intRegex = new Regex(intRex);
            return intRegex.IsMatch(value);
        }

        #endregion

        #region verify whether is postcode,allow null

        /// <summary>
        /// verify whether is postcode,allow null
        /// </summary>
        /// <param name="value">verify value</param>
        /// <returns>is post code or null</returns>
        public static bool IsZipCodeAllowNull(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return true;
            }
            return IsZipCode(value);
        }

        #endregion

        #region verify whether is a mobile number

        /// <summary>
        /// verify whether is a mobile number
        /// </summary>
        /// <param name="value">verify value</param>
        /// <returns>is mobile number</returns>
        public static bool IsMobile(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }
            string intRex = "^13[0-9]{9}|15[012356789][0-9]{8}|18[0256789][0-9]{8}|147[0-9]{8}$";
            Regex intRegex = new Regex(intRex);
            return intRegex.IsMatch(value) && value.Length == 11;
        }

        #endregion

        #region verify whether is a mobile number or null

        /// <summary>
        /// verify whether is a mobile number or null
        /// </summary>
        /// <param name="value">verify value</param>
        /// <returns>is mobile or null</returns>
        public static bool IsMobileAllowNull(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return true;
            }
            return IsMobile(value);
        }

        #endregion

        #region verify whether is a ip address

        /// <summary>
        /// verify whether is a ip address
        /// </summary>
        /// <param name="value">verify value</param>
        /// <returns>is ip address</returns>
        public static bool IsIP4(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }
            string intRex = "^(25[0-5]|2[0-4]\\d|[0-1]\\d{2}|[1-9]?\\d)\\.(25[0-5]|2[0-4]\\d|[0-1]\\d{2}|[1-9]?\\d)\\.(25[0-5]|2[0-4]\\d|[0-1]\\d{2}|[1-9]?\\d)\\.(25[0-5]|2[0-4]\\d|[0-1]\\d{2}|[1-9]?\\d)$";
            Regex intRegex = new Regex(intRex);
            return intRegex.IsMatch(value);
        }

        #endregion

        #region verify whether is a ip address or null

        /// <summary>
        /// verify whether is a ip address or null
        /// </summary>
        /// <param name="value">verify null</param>
        /// <returns>is ip address or null</returns>
        public static bool IsIP4AllowNull(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return true;
            }
            return IsIP4(value);
        }

        #endregion

        #region verify whether is image

        /// <summary>
        /// verify whther is image
        /// </summary>
        /// <param name="value">verify value</param>
        /// <returns>is image</returns>
        public static bool IsPicture(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }
            string intRex = "(.*)\\.(jpg|bmp|gif|ico|pcx|jpeg|tif|png|raw|tga)$";
            Regex intRegex = new Regex(intRex);
            return intRegex.IsMatch(value);
        }

        #endregion

        #region verify whether is rar

        /// <summary>
        /// verify whether is compress file
        /// </summary>
        /// <param name="value">verify value</param>
        /// <returns>is compress file</returns>
        public static bool IsCompressFile(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }
            string intRex = "(.*)\\.(rar|zip|7zip|tgz)$";
            Regex intRegex = new Regex(intRex);
            return intRegex.IsMatch(value);
        }

        #endregion

        #region verify whether is date

        /// <summary>
        /// verify whether is date
        /// </summary>
        /// <param name="value">verify value</param>
        /// <returns>is date</returns>
        public static bool IsDate(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }
            string intRex = @"^\\d{4}(\\-|\\/|\.)\\d{1,2}\\1\\d{1,2}$";
            Regex intRegex = new Regex(intRex);
            return intRegex.IsMatch(value);
        }

        #endregion

        #region verify whether is date or null

        /// <summary>
        /// verify whether is date or null
        /// </summary>
        /// <param name="value">verify value</param>
        /// <returns>is date or null</returns>
        public static bool IsDateAllowNull(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return true;
            }
            return IsDate(value);
        }

        #endregion

        #region verify whether is datetime

        /// <summary>
        /// verify whether is datetime
        /// </summary>
        /// <param name="value">verify value</param>
        /// <returns>is datetime</returns>
        public static bool IsDateTime(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }
            string intRex = @"^(\d{1,4})(-|\/)(\d{1,2})\2(\d{1,2}) (\d{1,2}):(\d{1,2}):(\d{1,2})$";
            Regex intRegex = new Regex(intRex);
            return intRegex.IsMatch(value);
        }

        #endregion

        #region verify whether is datetime or null

        /// <summary>
        /// verify whether is datetime or null
        /// </summary>
        /// <param name="value">verify value</param>
        /// <returns>is datetime or null</returns>
        public static bool IsDateTimeAllowNull(this string value)
        {
            if (value.IsNullOrEmpty())
            {

                return true;
            }
            return IsDateTime(value);
        }

        #endregion

        #region verify whether is qq

        /// <summary>
        /// verify whether is qq
        /// </summary>
        /// <param name="value">verify value</param>
        /// <returns>is qq</returns>
        public static bool IsQQ(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }
            string intRex = "^[1-9]*[1-9][0-9]*$";
            Regex intRegex = new Regex(intRex);
            return intRegex.IsMatch(value);
        }

        #endregion

        #region verify whether is qq or null

        /// <summary>
        /// verify whether is qq or null
        /// </summary>
        /// <param name="value">verify value</param>
        /// <returns>is qq or null</returns>
        public static bool IsQQAllowNull(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return true;
            }
            return IsQQ(value);
        }

        #endregion

        #region verify whether is tel

        /// <summary>
        /// verify whether is tel
        /// </summary>
        /// <param name="value">verify value</param>
        /// <returns>is tel</returns>
        public static bool IsTel(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }
            string intRex = "^(([0\\+]\\d{2,3}-)?(0\\d{2,3})-)?(\\d{7,8})(-(\\d{3,}))?$";
            Regex intRegex = new Regex(intRex);
            return intRegex.IsMatch(value);
        }

        #endregion

        #region verify whether is tel or null

        /// <summary>
        /// verify whether is tel or null
        /// </summary>
        /// <param name="value">verify value</param>
        /// <returns>is tel or null</returns>
        public static bool IsTelAllowNull(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return true;
            }
            return IsTel(value);
        }

        #endregion

        #region verify whether is letter

        /// <summary>
        /// verify whether is letter
        /// </summary>
        /// <param name="value">verify value</param>
        /// <returns>is letter</returns>
        public static bool IsLetter(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }
            string intRex = "^[A-Za-z]+$";
            Regex intRegex = new Regex(intRex);
            return intRegex.IsMatch(value);
        }

        #endregion

        #region verify whether is upper letter

        /// <summary>
        /// verify whether is upper letter
        /// </summary>
        /// <param name="value">verify value</param>
        /// <returns>is upper letter</returns>
        public static bool IsUpperLetter(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }
            string intRex = "^[A-Z]+$";
            Regex intRegex = new Regex(intRex);
            return intRegex.IsMatch(value);
        }

        #endregion

        #region verify whether is lower letter

        /// <summary>
        /// verify whether is lower letter
        /// </summary>
        /// <param name="value">verify value</param>
        /// <returns>is lower letter</returns>
        public static bool IsLowerLetter(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }
            string intRex = "^[a-z]+$";
            Regex intRegex = new Regex(intRex);
            return intRegex.IsMatch(value);
        }

        #endregion

        #region verify whether is Id Card

        /// <summary>
        /// verify whether is Id Card
        /// </summary>
        /// <param name="value">verify value</param>
        /// <returns>Is Id Card</returns>
        public static bool IsCard(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }
            string intRex = "^[1-9]([0-9]{14}|[0-9]{17})$";
            Regex intRegex = new Regex(intRex);
            return intRegex.IsMatch(value);
        }

        #endregion

        #region verify whether is Id Card or null

        /// <summary>
        /// verify whether is Id Card or null
        /// </summary>
        /// <param name="value">verify value</param>
        /// <returns>is card or null</returns>
        public static bool IsCardAllowNull(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return true;
            }
            return IsCard(value);
        }

        #endregion
    }
}
