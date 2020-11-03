using System;

namespace EZNEW.Web.Security.Authorization
{
    /// <summary>
    /// Authorize result
    /// </summary>
    [Serializable]
    public class AuthorizeResult
    {
        /// <summary>
        /// Gets or sets the authorization status
        /// </summary>
        public AuthorizationStatus Status { get; set; }

        /// <summary>
        /// Gets or sets whether allow to access
        /// </summary>
        public bool AllowAccess => Status == AuthorizationStatus.Success;

        /// <summary>
        /// Gets verify authorization result with challenge status
        /// </summary>
        /// <returns></returns>
        public static AuthorizeResult ChallengeResult()
        {
            return GetAuthorizeResult(AuthorizationStatus.Challenge);
        }

        /// <summary>
        /// Gets verify authorization result with forbid status
        /// </summary>
        /// <returns></returns>
        public static AuthorizeResult ForbidResult()
        {
            return GetAuthorizeResult(AuthorizationStatus.Forbid);
        }

        /// <summary>
        /// Gets verify authorization result with success status
        /// </summary>
        /// <returns></returns>
        public static AuthorizeResult SuccessResult()
        {
            return GetAuthorizeResult(AuthorizationStatus.Success);
        }

        /// <summary>
        /// Get autthorize result
        /// </summary>
        /// <param name="status">Verification status</param>
        /// <returns></returns>
        public static AuthorizeResult GetAuthorizeResult(AuthorizationStatus status = AuthorizationStatus.Forbid)
        {
            return new AuthorizeResult()
            {
                Status = status
            };
        }
    }
}
