using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Web.Security.Authorization
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class AuthorizationGroupAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the operation group name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the parent group name
        /// </summary>
        public string Parent { get; set; }
    }
}
