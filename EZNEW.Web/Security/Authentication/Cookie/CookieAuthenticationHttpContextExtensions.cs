using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using EZNEW.Web.Security.Authentication;

namespace Microsoft.AspNetCore.Http
{
    public static class CookieAuthenticationHttpContextExtensions
    {
        /// <summary>
        /// Sign in
        /// </summary>
        /// <typeparam name="TIdentityKey">Identity key</typeparam>
        /// <param name="context">Http context</param>
        /// <param name="user">User</param>
        /// <param name="properties">Properties</param>
        /// <returns></returns>
        public static async Task SignInAsync<TIdentityKey>(this HttpContext context, AuthenticationUser<TIdentityKey> user, AuthenticationProperties properties = null)
        {
            var claimIdentity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            claimIdentity.AddClaims(user.GetClaims());
            var claimsPrincipal = new ClaimsPrincipal(claimIdentity);
            await context.SignInAsync(claimsPrincipal, properties).ConfigureAwait(false);
        }
    }
}
