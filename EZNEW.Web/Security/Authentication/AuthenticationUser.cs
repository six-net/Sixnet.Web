using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using IdentityModel;
using EZNEW.ValueType;

namespace EZNEW.Web.Security.Authentication
{
    /// <summary>
    /// Authentication user 
    /// </summary>
    public class AuthenticationUser<TIdentityKey> : IIdentity
    {
        /// <summary>
        /// Admin tag key
        /// </summary>
        const string ADMIN_TAG_KEY = "EZNEW_SUPERADMIN";

        #region Properties

        /// <summary>
        /// Gets or sets the user id
        /// </summary>
        public TIdentityKey Id
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the authentication type
        /// </summary>
        public string AuthenticationType
        {
            get
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets or sets whether is authenticated
        /// </summary>
        public bool IsAuthenticated
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the real name
        /// </summary>
        public string RealName
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets whether is admin
        /// </summary>
        public bool IsAdmin
        {
            get; set;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get authentication user from principal
        /// </summary>
        /// <param name="principal">Principal</param>
        /// <returns>Return authentication user</returns>
        public static AuthenticationUser<TIdentityKey> GetUserFromPrincipal(ClaimsPrincipal principal)
        {
            if (principal == null)
            {
                return null;
            }
            return GetUserFromClaims(principal.Claims);
        }

        /// <summary>
        /// Get authentication user from claims
        /// </summary>
        /// <param name="claims">Claims</param>
        /// <returns>Return authentication user</returns>
        public static AuthenticationUser<TIdentityKey> GetUserFromClaims(IEnumerable<Claim> claims)
        {
            if (claims.IsNullOrEmpty())
            {
                return null;
            }
            var idClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (idClaim == null)
            {
                idClaim = claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Subject);
            }
            var nameClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
            if (nameClaim == null)
            {
                nameClaim = claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Name);
            }
            var realNameClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName);
            if (realNameClaim == null)
            {
                realNameClaim = realNameClaim = claims.FirstOrDefault(c => c.Type == JwtClaimTypes.NickName);
            }
            var adminClaim = claims.FirstOrDefault(c => c.Type == ADMIN_TAG_KEY);

            if (idClaim == null)
            {
                return null;
            }
            return new AuthenticationUser<TIdentityKey>()
            {
                Id = DataConverter.Convert<TIdentityKey>(idClaim.Value),
                Name = nameClaim?.Value,
                RealName = realNameClaim?.Value,
                IsAdmin = adminClaim?.Value == ADMIN_TAG_KEY
            };
        }

        /// <summary>
        /// Get claims
        /// </summary>
        /// <returns>Return claims</returns>
        public virtual List<Claim> GetClaims()
        {
            return new List<Claim>()
            {
                new Claim(JwtClaimTypes.Subject,Id.ToString()),
                new Claim(JwtClaimTypes.Name,Name??string.Empty),
                new Claim(JwtClaimTypes.NickName,RealName??string.Empty),
                new Claim(ADMIN_TAG_KEY,IsAdmin?ADMIN_TAG_KEY:"")
            };
        }

        #endregion
    }
}
