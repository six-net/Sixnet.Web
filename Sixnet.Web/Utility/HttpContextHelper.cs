using Microsoft.AspNetCore.Http;
using Sixnet.DependencyInjection;

namespace Sixnet.Web.Utility
{
    /// <summary>
    /// Http context helper
    /// </summary>
    public static class HttpContextHelper
    {
        /// <summary>
        /// Gets the current http context
        /// </summary>
        public static HttpContext Current
        {
            get
            {
                return SixnetContainer.GetService<IHttpContextAccessor>()?.HttpContext;
            }
        }
    }
}
