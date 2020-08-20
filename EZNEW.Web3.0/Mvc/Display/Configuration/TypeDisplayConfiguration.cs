using System.Collections.Generic;
using Newtonsoft.Json;

namespace EZNEW.Web.Mvc.Display.Configuration
{
    /// <summary>
    /// Type display configuration
    /// </summary>
    public class TypeDisplayConfiguration
    {
        /// <summary>
        /// Gets or sets the model type full name
        /// </summary>
        [JsonProperty(PropertyName = "typeName")]
        public string ModelTypeFullName { get; set; }

        /// <summary>
        /// Gets or sets the property display configs
        /// </summary>
        [JsonProperty(PropertyName = "displays")]
        public List<PropertyDisplay> Propertys { get; set; }
    }
}
