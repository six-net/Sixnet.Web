using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace EZNEW.Web.Security.Authentication.Cookie
{
    /// <summary>
    /// Custom cookie option
    /// </summary>
    public class CustomCookieOption
    {
        /// <summary>
        /// Gets or sets the cookie configuration
        /// </summary>
        public Action<CookieAuthenticationOptions> CookieConfiguration { get; set; }

        /// <summary>
        /// Gets or sets the cookie storage model
        /// </summary>
        public CookieStorageModel StorageModel { get; set; } = CookieStorageModel.Default;

        /// <summary>
        /// Gets or sets the Validate principal operation
        /// </summary>
        public Func<CookieValidatePrincipalContext, Task<bool>> ValidatePrincipalAsync { get; set; }

        /// <summary>
        /// Gets or sets whether force validate principal
        /// Must be set ValidatePrincipalAsync if value is true
        /// </summary>
        public bool ForceValidatePrincipal { get; set; } = false;
    }
}
