using System;
using System.Collections.Generic;
using Sixnet.Algorithm.Selection;

namespace Sixnet.Web.Security.Authorization
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
        /// Gets or sets the server select mode
        /// </summary>
        public SelectionMatchPattern ServerSelectMode { get; set; } = SelectionMatchPattern.EquiprobableRandom;

        /// <summary>
        /// Gets or sets whether enable remote authorization verify
        /// </summary>
        public bool RemoteVerify { get; set; }
    }
}
