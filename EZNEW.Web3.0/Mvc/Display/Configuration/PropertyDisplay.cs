using Newtonsoft.Json;

namespace EZNEW.Web.Mvc.Display.Configuration
{
    /// <summary>
    /// Property display info
    /// </summary>
    public class PropertyDisplay
    {
        /// <summary>
        /// Gets or sets the property name
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the display info
        /// </summary>
        [JsonProperty(PropertyName = "display")]
        public DisplayInfo Display { get; set; }
    }
}
