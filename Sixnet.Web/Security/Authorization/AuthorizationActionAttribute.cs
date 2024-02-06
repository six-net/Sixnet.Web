using System;
using System.Collections.Generic;
using System.Text;

namespace Sixnet.Web.Security.Authorization
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class AuthorizationActionAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the action name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the group name
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// Gets or sets whether allow access without authorized
        /// </summary>
        public bool Public { get; set; }
    }
}
