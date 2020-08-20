using Newtonsoft.Json;

namespace EZNEW.Web.Mvc.Display.Configuration
{
    /// <summary>
    /// Display info
    /// </summary>
    public class DisplayInfo
    {
        /// <summary>
        /// Gets or sets the display name
        /// </summary>
        [JsonProperty(PropertyName = "displayName")]
        public string DisplayName { get; set; }
    }
}
