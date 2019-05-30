using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Framework.ValueType
{
    /// <summary>
    /// Address
    /// </summary>
    [Serializable]
    public class Address
    {
        #region constructor

        /// <summary>
        /// instance a address
        /// </summary>
        /// <param name="streetAddress">street address</param>
        /// <param name="regionList">region list</param>
        /// <param name="zipCode">zip code</param>
        public Address(string streetAddress, List<Region> regionList = null, string zipCode = "")
        {
            StreetAddress = streetAddress;
            Regions = regionList;
            ZipCode = zipCode;
        }

        #endregion

        #region Propertys

        /// <summary>
        /// get regions
        /// </summary>
        public List<Region> Regions { get; }

        /// <summary>
        /// get street address
        /// </summary>
        public string StreetAddress { get; }

        /// <summary>
        /// get zip code
        /// </summary>
        public string ZipCode { get; }

        #endregion
    }

    /// <summary>
    /// region
    /// </summary>
    [Serializable]
    public class Region
    {
        #region constructor

        /// <summary>
        /// instance a Region
        /// </summary>
        /// <param name="name">region name</param>
        /// <param name="code">region code</param>
        public Region(string name, string code = "",int level=0)
        {
            Name = name;
            Code = code;
            Level = level;
        }

        #endregion

        #region Propertys

        /// <summary>
        /// get region name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// get region code
        /// </summary>
        public string Code { get; }

        /// <summary>
        /// get region level
        /// </summary>
        public int Level { get; }

        #endregion
    }
}
