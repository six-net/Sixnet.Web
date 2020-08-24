using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Web.Security.Authorization
{
    /// <summary>
    /// Authorization operation info
    /// </summary>
    public class AuthorizationOperationInfo
    {
        /// <summary>
        /// Gets or sets the operation name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the controller code
        /// </summary>
        public string ControllerCode { get; set; }

        /// <summary>
        /// Gets or sets the action code
        /// </summary>
        public string ActionCode { get; set; }

        /// <summary>
        /// Gets or sets whether allow access without authorized
        /// </summary>
        public bool Public { get; set; }
    }
}
