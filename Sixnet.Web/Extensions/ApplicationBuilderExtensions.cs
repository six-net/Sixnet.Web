using Sixnet.Web.Middleware;

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// Application builder extensions
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Adds the Microsoft.AspNetCore.Authentication.AuthenticationMiddleware to the specified Microsoft.AspNetCore.Builder.IApplicationBuilder, which enables tenant capabilities from claims.
        /// </summary>
        /// <param name="app">Application builder</param>
        /// <returns></returns>
        public static IApplicationBuilder UseSixnetSessionContext(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ClaimSessionMiddleware>();
        }
    }
}
