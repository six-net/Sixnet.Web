using System;
using System.Collections.Generic;
using EZNEW.Application;

namespace EZNEW.Web.Security.Authorization
{
    /// <summary>
    /// Verify authorization option
    /// </summary>
    [Serializable]
    public class VerifyAuthorizationOption
    {
        /// <summary>
        /// Gets or sets the controller code
        /// </summary>
        public string ControllerCode
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the action code
        /// </summary>
        public string ActionCode
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the request method
        /// </summary>
        public string Method
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the application info
        /// </summary>
        public ApplicationInfo Application
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the claims
        /// </summary>
        public Dictionary<string, string> Claims
        {
            get; set;
        }
    }
}
