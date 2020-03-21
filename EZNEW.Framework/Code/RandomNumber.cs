using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Framework.Code
{

    /// <summary>
    /// Generate Random Number
    /// </summary>
    public static class RandomNumber
    {
        private static Random random;

        #region static constructor

        /// <summary>
        /// static constructor
        /// </summary>
        static RandomNumber()
        {
            random = new Random();
        }

        #endregion

        #region functions

        /// <summary>
        /// generate a random number short or equal to the maxLength
        /// maxLength must be between 1 to 10
        /// number length must be equal to maxLength when constraintMaxLength is true
        /// </summary>
        /// <param name="maxLength">number max length(must be between 1 to 10)</param>
        /// <param name="constraintMaxLength">constraint number length equal maxLength</param>
        /// <returns>random number</returns>
        public static int GetRandomNumberByLength(int maxLength, bool constraintMaxLength = false)
        {
            if (maxLength <= 0)
            {
                return 0;
            }
            int maxValue = maxLength >= 10 ? int.MaxValue : GetMaxNumber(maxLength);
            int minValue = constraintMaxLength ? GetMinNumber(maxLength) : 0;
            return random.Next(minValue, maxValue);
        }

        /// <summary>
        /// get the max number by numberLength
        /// numberLength must be between 1 and 9
        /// </summary>
        /// <param name="numberLength">number length(must be between 1 and 9)</param>
        /// <returns>max number</returns>
        public static int GetMaxNumber(int numberLength)
        {
            if (numberLength < 1 || numberLength > 9)
            {
                throw new Exception("numberLength must be between 1 to 9");
            }
            string maxValueString = new string('9', numberLength);
            return Convert.ToInt32(maxValueString);
        }

        /// <summary>
        /// get the min number by numberLength
        /// numberLength must be between 1 to 10
        /// </summary>
        /// <param name="numberLength">number length(must be between 1 to 10)</param>
        /// <returns>min number</returns>
        public static int GetMinNumber(int numberLength)
        {
            if (numberLength < 1 || numberLength > 10)
            {
                throw new Exception("numberLength must be between 1 to 10");
            }
            string minValueString = new string('0', numberLength - 1);
            return Convert.ToInt32("1" + minValueString);

        }

        /// <summary>
        /// get a random number
        /// </summary>
        /// <param name="max">max value</param>
        /// <param name="min">min value</param>
        /// <returns></returns>
        public static int GetRandomNumber(int max, int min = 0)
        {
            return random.Next(min, max + 1);
        }

        #endregion
    }
}
