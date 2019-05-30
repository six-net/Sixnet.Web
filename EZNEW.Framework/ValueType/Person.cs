using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Framework.ValueType
{
    /// <summary>
    /// Person data type
    /// </summary>
    [Serializable]
    public class Person
    {
        #region Propertys

        /// <summary>
        /// get or set name
        /// </summary>
        public ChineseText Name { get; set; }

        /// <summary>
        /// get or set birth
        /// </summary>
        public Birth Birth { get; set; }

        /// <summary>
        /// get or set contact
        /// </summary>
        public Contact Contact { get; set; }

        /// <summary>
        /// get or set sex
        /// </summary>
        public Sex Sex { get; set; }

        /// <summary>
        /// get or set idcard
        /// </summary>
        public string IdCard { get; set; }

        #endregion
    }

    /// <summary>
    /// sex
    /// </summary>
    [Serializable]
    public enum Sex
    {
        Man = 2,
        Woman = 4,
        Secrecy = 8
    }
}
