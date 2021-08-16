using System.Collections.Generic;

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
        public string ModelTypeFullName { get; set; }

        /// <summary>
        /// Gets or sets the property display configs
        /// </summary>
        public List<PropertyDisplay> Propertys { get; set; }
    }
}
