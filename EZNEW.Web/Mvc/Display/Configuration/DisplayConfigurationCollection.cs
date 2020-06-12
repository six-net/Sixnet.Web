using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Web.Mvc.Display.Configuration
{
    /// <summary>
    /// Display configuration collection
    /// </summary>
    public class DisplayConfigurationCollection
    {
        /// <summary>
        /// Type display configuration
        /// </summary>
        [JsonProperty(PropertyName = "types")]
        public List<TypeDisplayConfiguration> Types { get; set; }
    }
}
