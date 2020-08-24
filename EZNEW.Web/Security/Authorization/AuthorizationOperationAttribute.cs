using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Web.Security.Authorization
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AuthorizationOperationAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the operation name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the operation group name
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// Gets or sets whether allow access without authorized
        /// </summary>
        public bool Public { get; set; }
    }
}
