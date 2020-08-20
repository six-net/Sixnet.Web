using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Web.Security.Authorization
{
    /// <summary>
    /// Authorization operation group info
    /// </summary>
    public class AuthorizationOperationGroupInfo
    {
        /// <summary>
        /// Gets or sets the group name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the child groups
        /// </summary>
        public List<AuthorizationOperationGroupInfo> ChildGroups { get; set; }

        /// <summary>
        /// Gets or sets the operations
        /// </summary>
        public List<AuthorizationOperationInfo> Operations { get; set; }
    }
}
