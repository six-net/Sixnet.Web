using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Web.Mvc.View.Extension
{
    /// <summary>
    /// Authorize action options
    /// </summary>
    public class AuthorizationActionOptions
    {
        /// <summary>
        /// Gets or sets the area name
        /// </summary>
        public string Area { get; set; }

        /// <summary>
        /// Gets or sets the controller
        /// </summary>
        public string Controller { get; set; }

        /// <summary>
        /// Gets or sets the action
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// Gets or sets the request method
        /// </summary>
        public string Method { get; set; } = "GET";

        public AuthorizationActionOptions(string controller, string action, string area = "")
        {
            Controller = controller;
            Action = action;
            Area = area;
        }
    }
}
