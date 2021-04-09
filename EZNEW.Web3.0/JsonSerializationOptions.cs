using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Web
{
    public class JsonSerializationOptions
    {
        /// <summary>
        /// Indecates whether use custom json converter
        /// Default value is true
        /// </summary>
        public bool UseCustomConverter { get; set; } = true;

        /// <summary>
        /// Indecates whether use custom naming policy
        /// Default value is true
        /// </summary>
        public bool UseCustomPropertyNamingPolicy { get; set; } = true;
    }
}
