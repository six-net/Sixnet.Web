using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Framework.Extension
{
    /// <summary>
    /// number extension methods
    /// </summary>
    public static class NumberExtension
    {
        /// <summary>
        /// convert object to int32
        /// </summary>
        /// <param name="value">object value</param>
        /// <returns>int32 value</returns>
        public static int ObjToInt32(this object value)
        {
            if (value == null)
            {
                return 0;
            }
            int newIntValue = 0;
            int.TryParse(value.ToString(), out newIntValue);
            return newIntValue;
        }

        #region convert object to int64

        /// <summary>
        /// convert object to int64
        /// </summary>
        /// <param name="value">object value</param>
        /// <returns>int64 value</returns>
        public static long ObjToInt64(this object value)
        {
            if (value == null)
            {
                return 0;
            }
            long newIntValue = 0;
            long.TryParse(value.ToString(), out newIntValue);
            return newIntValue;
        }

        #endregion

        #region return an least integer which is great than or equal the double value 

        /// <summary>
        /// returns the smallest integral value that is greater than or equal to the specified double-precision number
        /// </summary>
        /// <param name="value">double-precision number</param>
        /// <returns>integral value</returns>
        public static long DoubleCeiling(this double value)
        {
            return Math.Ceiling(value).ObjToInt64();
        }

        #endregion

        #region returns the largest integer that is less than or equal to the specified double-precision number

        /// <summary>
        /// returns the largest integer that is less than or equal to the specified double-precision number
        /// </summary>
        /// <param name="value">double-precision number</param>
        /// <returns>integer</returns>
        public static long DoubleFloor(this double value)
        {
            return Math.Floor(value).ObjToInt64();
        }

        #endregion

        #region rounds a double-precision value to a specified number of 2 fractional digits

        /// <summary>
        /// rounds a double-precision value to a specified number of fractional digits
        /// </summary>
        /// <param name="value">double-precision nmumber</param>
        /// <returns>the number nearest to value that contains a number of fractional digits equal to digits</returns>
        public static double ToDouble(this object value)
        {
            double newDoubleValue = 0.00;
            if (double.TryParse(value.ToString(), out newDoubleValue))
            {
                return Math.Round(newDoubleValue, 2);
            }
            return 0.00;
        }

        #endregion

        #region returns the largest integral value that is greater than or equal to the calculate result by the specified double-precision number divide 1024

        /// <summary>
        /// returns the largest integral value that is greater than or equal to the calculate result by the specified double-precision number divide 1024
        /// </summary>
        /// <param name="value">double-precision number</param>
        /// <returns>integral value</returns>
        public static double Divide1024(this double value)
        {
            double result = value / 1024;
            return Math.Ceiling(result);
        }

        #endregion

        #region rounds the calculate result by two double-precision numbers divide to a specified number of 2 fractional digits

        /// <summary>
        /// rounds the calculate result by two double-precision numbers divide to a specified number of 2 fractional digits
        /// </summary>
        /// <param name="value">double-precision number</param>
        /// <param name="divideValue">other double-precision number</param>
        /// <returns>calculate result</returns>
        public static double Divide(this double value, double divideValue)
        {
            if (divideValue != 0)
            {
                return Math.Round(value / divideValue, 2);
            }
            return 0;
        }

        #endregion

        #region calculate the height by the now-width and now-height scale value

        /// <summary>
        /// calculate the height by the nowWidth and nowHeight scale value
        /// </summary>
        /// <param name="width">width</param>
        /// <param name="nowWidth">nowWidth</param>
        /// <param name="nowHeight">nowheight</param>
        /// <returns>height</returns>
        public static double ComputeHeight(this double width, double nowWidth, double nowHeight)
        {
            #region verify args

            if (width <= 0 || nowWidth <= 0 || nowHeight <= 0)
            {
                return 0.0;
            }

            #endregion

            double rate = Math.Round(nowWidth / nowHeight, 2);
            double newHeight = Math.Ceiling(width / rate);
            return newHeight;
        }

        #endregion

        #region calculate the width by the now-width and now-height scale value

        /// <summary>
        /// calculate the width by the now-width and now-height scale value
        /// </summary>
        /// <param name="height">height</param>
        /// <param name="nowWidth">now width</param>
        /// <param name="nowHeight">now height`</param>
        /// <returns>height value</returns>
        public static double ComputeWdith(this double height, double nowWidth, double nowHeight)
        {
            #region verify args

            if (height <= 0 || nowWidth <= 0 || nowHeight <= 0)
            {
                return 0.0;
            }

            #endregion

            double rate = Math.Round(nowWidth / nowHeight, 2);
            double newWidth = Math.Ceiling(height * rate);
            return newWidth;
        }

        #endregion

        #region rounds a decimal value to a specified value of 2 fractional digits

        /// <summary>
        /// rounds a decimal value to a specified value of 2 fractional digits
        /// </summary>
        /// <param name="value">decimal value</param>
        /// <returns>round value</returns>
        public static decimal MathRound(this decimal value)
        {
            return Math.Round(value, 2);
        }

        /// <summary>
        /// rounds a double-precision value to a specified value of 2 fractional digits
        /// </summary>
        /// <param name="value">double-precision value</param>
        /// <returns></returns>
        public static double MathRound(this double value)
        {
            return Math.Round(value, 2);
        }

        #endregion
    }
}
