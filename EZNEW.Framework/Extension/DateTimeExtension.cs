using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Framework.Extension
{
    /// <summary>
    /// DateTime Extension Methods
    /// </summary>
    public static class DateTimeExtension
    {
        #region formatting DateTime to like (yyyy-MM-dd)

        /// <summary>
        /// formatting DateTime to like (yyyy-MM-dd)
        /// </summary>
        /// <param name="dateTime">datetime value</param>
        /// <returns>formatted value</returns>
        public static string ToDate(this DateTime? dateTime)
        {
            if (!dateTime.HasValue)
            {
                return string.Empty;
            }
            return dateTime.Value.ToString("yyyy-MM-dd");
        }

        #endregion

        #region formatting DateTime to chinese format like（yyyy年MM月dd日）

        /// <summary>
        /// formatting DateTime to chinese format like（yyyy年MM月dd日）
        /// </summary>
        /// <param name="dateTime">datetime value</param>
        /// <returns>formatted value</returns>
        public static string ToChineseDate(this DateTime? dateTime)
        {
            if (!dateTime.HasValue)
            {
                return string.Empty;
            }
            return dateTime.Value.ToString("yyyy年MM月dd日");
        }

        #endregion

        #region formatting DateTime to chinese format like（yyyy年MM月dd日）

        /// <summary>
        /// formatting DateTime to chinese format like（yyyy年MM月dd日）
        /// </summary>
        /// <param name="dateTime">datetime value</param>
        /// <returns>formatted value</returns>
        public static string ToChineseDate(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy年MM月dd日");
        }

        #endregion

        #region formatting DateTime to chinese format like（yyyy年MM月dd日 HH:mm:ss）

        /// <summary>
        /// formatting DateTime to chinese format like（yyyy年MM月dd日 HH:mm:ss）
        /// </summary>
        /// <param name="dateTime">datetime value</param>
        /// <returns>formatted value</returns>
        public static string ToChineseDateTime(this DateTime? dateTime)
        {
            if (!dateTime.HasValue)
            {
                return string.Empty;
            }
            return dateTime.Value.ToString("yyyy年MM月dd日 HH:mm:ss");
        }

        #endregion

        #region formatting DateTime to chinese format like（yyyy年MM月dd日 HH:mm:ss）

        /// <summary>
        /// formatting DateTime to chinese format like（yyyy年MM月dd日 HH:mm:ss）
        /// </summary>
        /// <param name="dateTime">datetime value</param>
        /// <returns>formatted value</returns>
        public static string ToChineseDateTime(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy年MM月dd日 HH:mm:ss");
        }

        #endregion

        #region formatting DateTime to chinese format with week like（yyyy年MM月dd日 星期一）

        /// <summary>
        /// formatting DateTime to chinese format with week like（yyyy年MM月dd日 星期一）
        /// </summary>
        /// <param name="dateTime">datetime value</param>
        /// <returns>formatted value</returns>
        public static string ToChineseDateWeek(this DateTime dateTime)
        {
            return dateTime.ToChineseDate() + " 星期" + EnglishToChineseWeek(dateTime.DayOfWeek.ToString());
        }

        #endregion

        #region formatting DateTime to like(yyyy-MM-dd HH:mm:ss)

        /// <summary>
        /// formatting DateTime to like(yyyy-MM-dd HH:mm:ss)
        /// </summary>
        /// <param name="dateTime">datetime value</param>
        /// <returns>formatted value</returns>
        public static string FormatToSecond(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
        }

        #endregion

        #region formatting DateTime to like(yyyy-MM-dd HH:mm)

        /// <summary>
        /// formatting DateTime to like(yyyy-MM-dd HH:mm)
        /// </summary>
        /// <param name="dateTime">datetime value</param>
        /// <returns>formatted value</returns>
        public static string FormatToMinute(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd HH:mm");
        }

        #endregion

        #region formatting DateTime to like (yyyy-MM-dd HH)

        /// <summary>
        /// formatting DateTime to like (yyyy-MM-dd HH)
        /// </summary>
        /// <param name="dateTime">datetime value</param>
        /// <returns>formatted value</returns>
        public static string FormatToHour(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd HH");
        }

        #endregion

        #region convert english weekday to chinese

        /// <summary>
        /// convert english weekday to chinese
        /// </summary>
        /// <param name="value">english value</param>
        /// <returns>chinese value</returns>
        private static string EnglishToChineseWeek(this string value)
        {
            string weekValue = string.Empty;
            if (value.IsNullOrEmpty()) return weekValue;
            value = value.ToLower();
            switch (value)
            {
                case "sunday":
                    weekValue = "日";
                    break;
                case "monday":
                    weekValue = "一";
                    break;
                case "tuesday":
                    weekValue = "二";
                    break;
                case "wednesday":
                    weekValue = "三";
                    break;
                case "thursday":
                    weekValue = "四";
                    break;
                case "friday":
                    weekValue = "五";
                    break;
                case "saturday":
                    weekValue = "六";
                    break;

            }
            return weekValue;
        }

        #endregion

        #region calculate time difference

        /// <summary>
        /// calculate time difference
        /// </summary>
        /// <param name="nowDate">now date</param>
        /// <param name="endTime">end date</param>
        /// <param name="hours">hours</param>
        /// <param name="minutes">minutes</param>
        /// <param name="seconds">seconds</param>
        public static void TDOA(this DateTime nowDate, DateTime endTime, out long hours, out long minutes, out long seconds)
        {
            hours = minutes = seconds = 0;
            DateTime maxDate = endTime;
            DateTime minDate = nowDate;
            if (nowDate > endTime)
            {
                minDate = endTime;
                maxDate = nowDate;
            }
            TimeSpan timeSpan = maxDate - minDate;
            long totalSeconds = Math.Floor(timeSpan.TotalSeconds).ObjToInt64();
            seconds = totalSeconds % 60;
            long totalMinutes = totalSeconds / 60;
            minutes = totalMinutes % 60;
            hours = totalMinutes / 60;
        }

        #endregion

        #region wheather is null or too small

        /// <summary>
        /// value is most small
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static bool IsMostSmall(this DateTime dateTime)
        {
            SqlDateTime minValue = SqlDateTime.MinValue;
            return dateTime < minValue.Value;
        }

        #endregion
    }
}
