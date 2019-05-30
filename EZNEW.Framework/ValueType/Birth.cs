using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Framework.ValueType
{
    /// <summary>
    /// Birth Date Type
    /// </summary>
    [Serializable]
    public struct Birth
    {
        #region fields

        private int _age;//age
        private Constellation _constellation;//constellation
        private static readonly Dictionary<Constellation, Tuple<DateTime, DateTime>> constellationDic = new Dictionary<Constellation, Tuple<DateTime, DateTime>>()
        {
            { Constellation.Aquarius,new Tuple<DateTime, DateTime>(new DateTime(2000,1,20),new DateTime(2000,2,18))},
            { Constellation.Pisces,new Tuple<DateTime, DateTime>(new DateTime(2000,2,19),new DateTime(2000,3,20))},
            { Constellation.Aries,new Tuple<DateTime, DateTime>(new DateTime(2000,3,21),new DateTime(2000,4,19))},
            { Constellation.Taurus,new Tuple<DateTime, DateTime>(new DateTime(2000,4,20),new DateTime(2000,5,20))},
            { Constellation.Gemini,new Tuple<DateTime, DateTime>(new DateTime(2000,5,21),new DateTime(2000,6,21))},
            { Constellation.Cacer,new Tuple<DateTime, DateTime>(new DateTime(2000,6,22),new DateTime(2000,7,22))},
            { Constellation.Leo,new Tuple<DateTime, DateTime>(new DateTime(2000,7,23),new DateTime(2000,8,22))},
            { Constellation.Virgo,new Tuple<DateTime, DateTime>(new DateTime(2000,8,23),new DateTime(2000,9,22))},
            { Constellation.Libra,new Tuple<DateTime, DateTime>(new DateTime(2000,9,23),new DateTime(2000,10,23))},
            { Constellation.Scorpio,new Tuple<DateTime, DateTime>(new DateTime(2000,10,24),new DateTime(2000,11,22))},
            { Constellation.Sagittarius,new Tuple<DateTime, DateTime>(new DateTime(2000,11,23),new DateTime(2000,12,21))},
            { Constellation.Capricom,new Tuple<DateTime, DateTime>(new DateTime(2000,12,22),new DateTime(2000,1,19))},
        };

        #endregion

        #region constructor

        /// <summary>
        /// instance a Birth object
        /// </summary>
        /// <param name="birthDate">birth datetime</param>
        public Birth(DateTime birthDate)
        {
            BirthDate = birthDate;
            _constellation = GetConstellation(birthDate);
            _age = GetAge(birthDate);
        }

        /// <summary>
        /// get age
        /// </summary>
        public int Age
        {
            get
            {
                return GetAge(BirthDate);
            }
        }

        /// <summary>
        /// get birth datetime
        /// </summary>
        public DateTime BirthDate { get; }

        /// <summary>
        /// get constellation
        /// </summary>
        public Constellation Constellation
        {
            get
            {
                return GetConstellation(BirthDate);
            }
        }

        #endregion

        #region static methods

        /// <summary>
        /// get constellation
        /// </summary>
        /// <param name="dateTime">datetime</param>
        /// <returns>Constellation</returns>
        public static Constellation GetConstellation(DateTime dateTime)
        {
            int month = dateTime.Month;
            int day = dateTime.Day;
            var constell = Constellation.Gemini;
            var constellDate = new DateTime(2000, month, day);
            var constellItem = constellationDic.FirstOrDefault(c => c.Value.Item1 <= constellDate && c.Value.Item2 >= constellDate);
            constell = constellItem.Key;
            return constell;
        }

        /// <summary>
        /// get age
        /// </summary>
        /// <param name="dateTime">birth date</param>
        /// <returns>age</returns>
        public static int GetAge(DateTime dateTime)
        {
            var nowDate = DateTime.Now.Date;
            var birthDate = dateTime.Date;
            if (nowDate < birthDate.AddYears(1))
            {
                return 0;
            }
            return (nowDate - birthDate).Days / 365;
        }

        #endregion
    }

    /// <summary>
    /// Constellation
    /// </summary>
    [Serializable]
    public enum Constellation
    {
        Aquarius = 120218,
        Pisces = 219320,
        Aries = 321419,
        Taurus = 420520,
        Gemini = 521621,
        Cacer = 622722,
        Leo = 723822,
        Virgo = 823922,
        Libra = 9231023,
        Scorpio = 10241122,
        Sagittarius = 11231221,
        Capricom = 12220119
    }
}
