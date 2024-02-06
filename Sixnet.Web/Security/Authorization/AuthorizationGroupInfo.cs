using System;
using System.Collections.Generic;
using System.Text;

namespace Sixnet.Web.Security.Authorization
{
    /// <summary>
    /// Authorization group info
    /// </summary>
    public class AuthorizationGroupInfo
    {
        /// <summary>
        /// Gets or sets the group name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the child groups
        /// </summary>
        public List<AuthorizationGroupInfo> ChildGroups { get; set; }

        /// <summary>
        /// Gets or sets the operations
        /// </summary>
        public List<AuthorizationActionInfo> Actions { get; set; }
    }
}
