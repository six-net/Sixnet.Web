using Microsoft.AspNetCore.Http;

namespace Sixnet.Web.Security.Authentication.Cookies
{
    /// <summary>
    /// Cookie item
    /// </summary>
    public class CookieEntry
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
        public CookieOptions Options { get; set; }
    }
}
