using System;

namespace EZNEW.Web.Security.Authorization
{
    /// <summary>
    /// Verify authorization result
    /// </summary>
    [Serializable]
    public class VerifyAuthorizationResult
    {
        /// <summary>
        /// Gets or sets the authorization verification status
        /// </summary>
        public AuthorizationVerificationStatus Status
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets whether allow to access
        /// </summary>
        public bool AllowAccess
        {
            get
            {
                return Status == AuthorizationVerificationStatus.Success;
            }
        }

        /// <summary>
        /// Gets verify authorization result with challenge status
        /// </summary>
        /// <returns></returns>
        public static VerifyAuthorizationResult ChallengeResult()
        {
            return GetVerifyAuthorizationResult(AuthorizationVerificationStatus.Challenge);
        }

        /// <summary>
        /// Gets verify authorization result with forbid status
        /// </summary>
        /// <returns></returns>
        public static VerifyAuthorizationResult ForbidResult()
        {
            return GetVerifyAuthorizationResult(AuthorizationVerificationStatus.Forbid);
        }

        /// <summary>
        /// Gets verify authorization result with success status
        /// </summary>
        /// <returns></returns>
        public static VerifyAuthorizationResult SuccessResult()
        {
            return GetVerifyAuthorizationResult(AuthorizationVerificationStatus.Success);
        }

        /// <summary>
        /// Get verify authorization result
        /// </summary>
        /// <param name="status">Verification status</param>
        /// <returns></returns>
        public static VerifyAuthorizationResult GetVerifyAuthorizationResult(AuthorizationVerificationStatus status = AuthorizationVerificationStatus.Forbid)
        {
            return new VerifyAuthorizationResult()
            {
                Status = status
            };
        }
    }
}
