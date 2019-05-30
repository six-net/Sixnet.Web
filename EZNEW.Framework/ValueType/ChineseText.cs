using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EZNEW.Framework.Extension;
using EZNEW.Framework.Language;

namespace EZNEW.Framework.ValueType
{
    /// <summary>
    /// Chinese Text
    /// </summary>
    [Serializable]
    public struct ChineseText
    {

        #region fields

        /// <summary>
        /// chinese spelling
        /// </summary>
        string spelling;

        /// <summary>
        /// spelling short form
        /// </summary>
        string spellingShort;

        /// <summary>
        /// spelling is inited
        /// </summary>
        bool spellingInit;

        #endregion

        #region constructor

        /// <summary>
        /// instance a ChineseText object
        /// </summary>
        /// <param name="text">full chinese text</param>
        public ChineseText(string text)
        {
            Text = text.Trim();
            spelling = "";
            spellingShort = "";
            spellingInit = false;
        }

        #endregion

        #region Propertys

        /// <summary>
        /// spelling
        /// </summary>
        public string Spelling
        {
            get
            {
                return GetSpelling();
            }
        }

        /// <summary>
        /// spelling short
        /// </summary>
        public string SpellingShort
        {
            get
            {
                return GetSpellingShort();
            }
        }

        /// <summary>
        /// chinese text
        /// </summary>
        public string Text { get; }

        #endregion

        #region methods

        /// <summary>
        /// get spelling
        /// </summary>
        /// <returns></returns>
        string GetSpelling()
        {
            if (spellingInit)
            {
                InitSpelling();
            }
            return spelling;
        }

        /// <summary>
        /// get spelling short
        /// </summary>
        /// <returns></returns>
        string GetSpellingShort()
        {
            if (spellingInit)
            {
                InitSpelling();
            }
            return spellingShort;
        }

        /// <summary>
        /// init spelling
        /// </summary>
        void InitSpelling()
        {
            if (!Text.IsNullOrEmpty())
            {
                var chineseUtil = this.Instance<IChineseLanguage>();
                if (chineseUtil == null)
                {
                    return;
                }
                spelling = chineseUtil.GetSpellingBySimpleChinese(Text);
                spellingShort = chineseUtil.GetSpellingShortSimpleChinese(Text);
            }
            spellingInit = true;
        }

        /// <summary>
        /// override ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Text;
        }

        /// <summary>
        /// implicit convert to string
        /// </summary>
        /// <param name="text"></param>
        public static implicit operator string(ChineseText text)
        {
            return text.Text;
        }

        /// <summary>
        /// implicit convert to chineseText
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator ChineseText(string value)
        {
            return new ChineseText(value);
        }

        #endregion
    }
}
