using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Framework.Serialize
{
    /// <summary>
    /// json serialize setting
    /// </summary>
    public class JsonSerializeSetting
    {
        /// <summary>
        /// get or set 
        /// </summary>
        public bool ResolveNonPublic
        {
            get;set;
        }

        /// <summary>
        /// deserialize type
        /// </summary>
        public Type DeserializeType
        {
            get;set;
        }
    }
}
