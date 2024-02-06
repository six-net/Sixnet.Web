using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Sixnet.Web.Security.Authentication.Cookies
{
    /// <summary>
    /// Cookie authentication event handler
    /// </summary>
    public class CookieAuthenticationEventHandler : CookieAuthenticationEvents
    {
        /// <summary>
        /// Gets or sets whether force validate principal
        /// </summary>
        internal static bool ForceValidatePrincipal { get; set; } = false;

        /// <summary>
        /// Gets or sets the validate principal operation
        /// </summary>
        internal static Func<CookieValidatePrincipalContext, Task<bool>> OnValidatePrincipalAsync { get; set; }

        /// <summary>
        /// Validate principal
        /// </summary>
        /// <param name="context">Principal context</param>
        /// <returns></returns>
        public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
        {
            if (OnValidatePrincipalAsync == null)
            {
                if (ForceValidatePrincipal)
                {
                    context.RejectPrincipal();
                }
                context.ShouldRenew = true;
            }
            else
            {
                var result = await OnValidatePrincipalAsync(context).ConfigureAwait(false);
                if (!result)
                {
                    context.RejectPrincipal();
                }
                else
                {
                    context.ShouldRenew = true;
                }
            }
        }
    }
}
