using System;
using System.Collections.Generic;
using EZNEW.Selection;

namespace EZNEW.Web.Security.Authorization
{
    /// <summary>
    /// Authorization configuration
    /// </summary>
    [Serializable]
    public class AuthorizationConfiguration
    {
        /// <summary>
        /// Gets or sets the Servers
        /// </summary>
        public List<string> Servers { get; set; }

        /// <summary>
        /// Gets or sets the server select pattern
        /// </summary>
        public SelectionMatchPattern ServerSelectPattern { get; set; } = SelectionMatchPattern.EquiprobableRandom;

        /// <summary>
        /// Gets or sets whether enable remote authorization verify
        /// </summary>
        public bool RemoteVerify { get; set; }
    }
}
