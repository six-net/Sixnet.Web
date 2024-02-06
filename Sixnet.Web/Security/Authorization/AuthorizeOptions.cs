using System;
using System.Collections.Generic;
using Sixnet.App;
using Microsoft.AspNetCore.Mvc;

namespace Sixnet.Web.Security.Authorization
{
    /// <summary>
    /// Authorize options
    /// </summary>
    [Serializable]
    public class AuthorizeOptions
    {
        /// <summary>
        /// Gets or sets the area name
        /// </summary>
        public string Area { get; set; }

        /// <summary>
        /// Gets or sets the controller code
        /// </summary>
        public string Controller { get; set; }

        /// <summary>
        /// Gets or sets the action code
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// Gets or sets the request method
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// Gets or sets the application info
        /// </summary>
        public ApplicationInfo Application { get; set; }

        /// <summary>
        /// Gets or sets the claims
        /// </summary>
        public Dictionary<string, string> Claims { get; set; }

        /// <summary>
        /// Gets or sets the action context
        /// </summary>
        public ActionContext ActionContext { get; set; }
    }
}
