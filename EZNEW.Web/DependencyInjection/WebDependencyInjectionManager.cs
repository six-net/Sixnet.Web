using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using EZNEW.DependencyInjection;
using EZNEW.Web.Mvc;

namespace EZNEW.Web.DependencyInjection
{
    public class WebDependencyInjectionManager
    {
        /// <summary>
        /// Configure default web service
        /// </summary>
        public static void ConfigureDefaultWebService()
        {
            Configure();
        }

        /// <summary>
        /// Configure service
        /// </summary>
        static void Configure()
        {
        }
    }
}
