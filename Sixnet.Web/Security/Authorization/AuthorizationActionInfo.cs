using System;
using System.Collections.Generic;
using System.Text;

namespace Sixnet.Web.Security.Authorization
{
    /// <summary>
    /// Authorization action info
    /// </summary>
    public class AuthorizationActionInfo
    {
        /// <summary>
        /// Gets or sets the action name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the controller code
        /// </summary>
        public string Controller { get; set; }

        /// <summary>
        /// Gets or sets the action code
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// Gets or sets the area name
        /// </summary>
        public string Area { get; set; }

        /// <summary>
        /// Gets or sets whether allow access without authorized
        /// </summary>
        public bool Public { get; set; }
    }
}
