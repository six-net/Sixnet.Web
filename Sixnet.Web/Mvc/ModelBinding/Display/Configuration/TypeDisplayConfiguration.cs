using System.Collections.Generic;

namespace Sixnet.Web.Mvc.ModelBinding.Display.Configuration
{
    /// <summary>
    /// Type display configuration
    /// </summary>
    public class TypeDisplayConfiguration
    {
        /// <summary>
        /// Gets or sets the model type full name
        /// </summary>
        public string TypeAssemblyQualifiedName { get; set; }

        /// <summary>
        /// Gets or sets the property display configs
        /// </summary>
        public List<PropertyDisplay> Properties { get; set; }
    }
}
