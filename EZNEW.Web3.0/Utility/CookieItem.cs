using Microsoft.AspNetCore.Http;

namespace EZNEW.Web.Utility
{
    /// <summary>
    /// Cookie item
    /// </summary>
    public class CookieItem
    {
        /// <summary>
        /// Gets or sets the cookie key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the cookie value
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets cookie options
        /// </summary>
        public CookieOptions Option { get; set; }
    }
}
