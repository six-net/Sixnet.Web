﻿using System;

namespace EZNEW.Web.Security.Authorization
{
    /// <summary>
    /// Defines authorize verify value
    /// </summary>
    [Serializable]
    public enum AuthorizationVerificationStatus
    {
        /// <summary>
        /// not log in
        /// </summary>
        Challenge = 110,
        /// <summary>
        /// not authorized
        /// </summary>
        Forbid = 120,
        /// <summary>
        /// successful
        /// </summary>
        Success = 130
    }
}