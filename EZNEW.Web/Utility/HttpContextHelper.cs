using Microsoft.AspNetCore.Http;
using EZNEW.DependencyInjection;

namespace EZNEW.Web.Utility
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
                object factory = ContainerManager.Resolve<IHttpContextAccessor>();
                HttpContext context = ((HttpContextAccessor)factory).HttpContext;
                return context;
            }
        }
    }
}
