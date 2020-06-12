using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Web.Mvc.View.Extension
{
    /// <summary>
    /// Authorize operation
    /// </summary>
    public class AuthorizeOperation
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
        } = "GET";

        public AuthorizeOperation(string controllerCode, string actionCode)
        {
            ControllerCode = controllerCode;
            ActionCode = actionCode;
        }
    }
}
