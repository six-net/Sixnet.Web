using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace EZNEW.Framework.Drawing
{
    /// <summary>
    /// verification code base
    /// </summary>
    public abstract class VerificationCodeBase
    {
        #region fields

        protected string code = string.Empty;

        protected int length = 5;//code length

        protected int minLength = 1;//min length

        protected int maxLenght = 50;//max length

        protected VerificationCodeType codeType = VerificationCodeType.NumberAndLetter;//type

        protected int interfereNum = 3; //interfere num

        protected static readonly char[] charArray = { '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'm', 'n', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };

        protected int fontSize = 30;

        protected int spaceBetween = 1;

        protected Color backgroundColor = Color.White;

        protected Color? fontColor = null;//use random color if null

        protected Color? interfereColor = null;//use random color if null

        protected string frontFamilyName = "";//front family name

        protected static readonly Random random = new Random();

        #endregion

        #region propertys

        /// <summary>
        /// get or set code length
        /// </summary>
        public int Length
        {
            get
            {
                return length;
            }
            set
            {
                SetCodeLength(value);
            }
        }

        /// <summary>
        /// get or set verification code type
        /// </summary>
        public VerificationCodeType CodeType
        {
            get
            {
                return codeType;
            }
            set
            {
                codeType = value;
            }
        }

        /// <summary>
        /// get code value
        /// </summary>
        public string Code
        {
            get
            {
                return code;
            }
        }

        /// <summary>
        /// get or se font size
        /// </summary>
        public int FontSize
        {
            get
            {
                return fontSize;
            }
            set
            {
                fontSize = value;
            }
        }

        /// <summary>
        /// get or set char space
        /// </summary>
        public int SpaceBetween
        {
            get
            {
                return spaceBetween;
            }
            set
            {
                spaceBetween = value;
            }
        }

        /// <summary>
        /// get or set background
        /// use White by default
        /// </summary>
        public Color BackgroundColor
        {
            get
            {
                return backgroundColor;
            }
            set
            {
                backgroundColor = value;
            }
        }

        /// <summary>
        /// get or set font color
        /// use random colors if set to null value
        /// </summary>
        public Color? FontColor
        {
            get
            {
                return fontColor;
            }
            set
            {
                fontColor = value;
            }

        }

        /// <summary>
        /// interfere num
        /// </summary>
        public int InterfereNum
        {
            get
            {
                return interfereNum;
            }
            set
            {
                interfereNum = value;
            }
        }

        /// <summary>
        /// get or set interfere line color
        /// use random colors if set to null value
        /// </summary>
        public Color? InterfereColor
        {
            get
            {
                return interfereColor;
            }
            set
            {
                interfereColor = value;
            }
        }

        /// <summary>
        /// front family name
        /// </summary>
        public string FrontFamilyName
        {
            get
            {
                return frontFamilyName;
            }
            set
            {
                frontFamilyName = value;
            }
        }

        #endregion

        #region methods

        #region set code length

        /// <summary>
        /// set code lengt
        /// </summary>
        /// <param name="length">code length</param>
        private void SetCodeLength(int length)
        {
            if (length <= 0)
            {
                return;
            }
            length = length < minLength ? minLength : length;
            length = length > maxLenght ? maxLenght : length;
            this.length = length;
        }

        #endregion

        #region Generate Code

        /// <summary>
        /// Generate Code
        /// </summary>
        /// <returns>code image bytearray</returns>
        public abstract byte[] CreateCode();

        #endregion

        #endregion
    }
}
