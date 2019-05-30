using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Framework.Language
{
    /// <summary>
    /// Chinese Process
    /// </summary>
    public interface IChineseLanguage
    {
        #region get the spelling from chinese string

        /// <summary>
        /// get the spelling from chinese string
        /// </summary>
        /// <param name="chineseValue">chinese value</param>
        /// <param name="split">split string,default is a space</param>
        /// <param name="toLowerCase">to lower,default is upper</param>
        /// <returns>spelling</returns>
        string GetSpellingBySimpleChinese(string chineseValue, string split = " ", bool toLowerCase = false);

        #endregion

        #region get the spelling first char from chinese string

        /// <summary>
        /// get the spelling first char from chinese string
        /// </summary>
        /// <param name="chineseValue">chinese value</param>
        /// <param name="split">split string</param>
        /// <param name="toLowerCase">convert to lower,default is upper</param>
        /// <returns>spelling first char</returns>
        string GetSpellingShortSimpleChinese(string chineseValue, string split = "", bool toLowerCase = false);

        #endregion

        #region get the spelling from chinese char

        /// <summary>
        /// get the spelling from chinese char
        /// </summary>
        /// <param name="charVal">chinese char</param>
        /// <returns>spelling</returns>
        List<string> GetChineseCharSpellingList(char charVal);

        #endregion

        #region chinese simple to traditional

        /// <summary>
        /// chinese simple to traditional
        /// </summary>
        /// <param name="chineseValue">chinese simple value</param>
        /// <returns>traditional value</returns>
        string SimpleToTraditional(string chineseValue);

        #endregion

        #region traditional to chinese simple

        /// <summary>
        /// traditional to chinese simple
        /// </summary>
        /// <param name="chineseValue">chinese traditional value</param>
        /// <returns>chinese simple value</returns>
        string TraditionalToSimple(string chineseValue);

        #endregion
    }
}
