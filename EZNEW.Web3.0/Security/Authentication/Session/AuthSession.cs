using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Newtonsoft.Json;
using IdentityModel;

namespace EZNEW.Web.Security.Authentication.Session
{
    /// <summary>
    /// Auth session
    /// </summary>
    public class AuthSession
    {
        public AuthSession()
        {
            SessionToken = Guid.NewGuid().ToString("N");
        }

        /// <summary>
        /// Gets or sets the authentication scheme
        /// </summary>
        [JsonProperty(PropertyName = "authentication_scheme")]
        public string AuthenticationScheme
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the claims
        /// </summary>
        [JsonProperty(PropertyName = "claims")]
        public Dictionary<string, string> Claims { get; set; }

        /// <summary>
        /// Gets or sets the properties items
        /// </summary>
        [JsonProperty(PropertyName = "properties_items")]
        public Dictionary<string, string> PropertiesItems { get; set; }

        /// <summary>
        /// Gets or sets the expires
        /// </summary>
        [JsonProperty(PropertyName = "expires")]
        public DateTimeOffset Expires { get; set; }

        /// <summary>
        /// Gets or sets the session token
        /// </summary>
        [JsonProperty(PropertyName = "session_token")]
        public string SessionToken { get; set; }

        /// <summary>
        /// Gets or sets the session id
        /// </summary>
        [JsonProperty(PropertyName = "session_id")]
        public string SessionId { get; set; }

        /// <summary>
        /// Gets or sets the subject id
        /// </summary>
        /// <returns></returns>
        public string GetSubjectId()
        {
            if (Claims == null || Claims.Count <= 0)
            {
                return string.Empty;
            }
            if (Claims.ContainsKey(ClaimTypes.NameIdentifier))
            {
                return Claims[ClaimTypes.NameIdentifier];
            }
            if (Claims.ContainsKey(JwtClaimTypes.Subject))
            {
                return Claims[JwtClaimTypes.Subject];
            }
            return string.Empty;
        }

        /// <summary>
        /// Get auth session by authentication ticket
        /// </summary>
        /// <param name="ticket">Ticket</param>
        /// <returns>Return auth session</returns>
        public static AuthSession FromAuthenticationTicket(AuthenticationTicket ticket)
        {
            if (ticket == null)
            {
                return null;
            }
            var session = new AuthSession()
            {
                AuthenticationScheme = ticket.AuthenticationScheme,
                PropertiesItems = ticket.Properties.Items.ToDictionary(c => c.Key, c => c.Value),
                Claims = ticket.Principal.Claims?.ToDictionary(c => c.Type, c => c.Value),
            };
            var sessionConfig = SessionConfiguration.GetSessionConfiguration();
            if (session.Claims?.ContainsKey(sessionConfig.SessionClaimName) ?? false)
            {
                session.SessionToken = session.Claims[sessionConfig.SessionClaimName];
            }
            session.Expires = DateTimeOffset.Now.Add(sessionConfig.Expires);
            return session;
        }

        /// <summary>
        /// Convert auth session to ticket
        /// </summary>
        /// <returns></returns>
        public AuthenticationTicket ConvertToTicket()
        {
            var claimIdentity = new ClaimsIdentity(AuthenticationScheme);
            claimIdentity.AddClaims(Claims.Select(c => new Claim(c.Key, c.Value)));
            var sessionConfig = SessionConfiguration.GetSessionConfiguration();
            var nowSessionClaim = claimIdentity.Claims.FirstOrDefault(c => c.Type == sessionConfig.SessionClaimName);
            if (nowSessionClaim == null)
            {
                claimIdentity.AddClaim(new Claim(sessionConfig.SessionClaimName, SessionToken));
            }
            AuthenticationProperties props = new AuthenticationProperties();
            if (PropertiesItems != null)
            {
                foreach (var pitem in PropertiesItems)
                {
                    props.Items.Add(pitem.Key, pitem.Value);
                }
            }
            props.IsPersistent = true;
            props.AllowRefresh = true;
            return new AuthenticationTicket(new ClaimsPrincipal(claimIdentity), props, AuthenticationScheme);
        }

        /// <summary>
        /// Get session token
        /// </summary>
        /// <param name="values">Values</param>
        /// <returns>Return session token</returns>
        public static string GetSessionToken(Dictionary<string, string> values)
        {
            if (values == null || values.Count <= 0)
            {
                return string.Empty;
            }
            var sessionConfig = SessionConfiguration.GetSessionConfiguration();
            values.TryGetValue(sessionConfig.SessionClaimName, out string sessionToken);
            return sessionToken;
        }

        /// <summary>
        /// Gets session token
        /// </summary>
        /// <param name="claims">Claims</param>
        /// <returns>Return session token</returns>
        public static string GetSessionToken(IEnumerable<Claim> claims)
        {
            if (claims != null && claims.Any())
            {
                var sessionConfig = SessionConfiguration.GetSessionConfiguration();
                var sessionClaim = claims.FirstOrDefault(c => c.Type == sessionConfig.SessionClaimName);
                return sessionClaim?.Value ?? string.Empty;
            }
            return string.Empty;
        }

        /// <summary>
        /// Get subject
        /// </summary>
        /// <param name="values">Values</param>
        /// <returns>Return subject</returns>
        public static string GetSubject(Dictionary<string, string> values)
        {
            if (values == null || values.Count <= 0)
            {
                return string.Empty;
            }
            if (values.ContainsKey(ClaimTypes.NameIdentifier))
            {
                return values[ClaimTypes.NameIdentifier];
            }
            if (values.ContainsKey(JwtClaimTypes.Subject))
            {
                return values[JwtClaimTypes.Subject];
            }
            return string.Empty;
        }

        /// <summary>
        /// Get subject
        /// </summary>
        /// <param name="claims">Claims</param>
        /// <returns>Return subject</returns>
        public static string GetSubject(IEnumerable<Claim> claims)
        {
            if (claims != null && claims.Any())
            {
                var sessionConfig = SessionConfiguration.GetSessionConfiguration();
                return GetSubject(claims.ToDictionary(c => c.Type, c => c.Value));
            }
            return string.Empty;
        }

        /// <summary>
        /// Verify auth session allow to use
        /// </summary>
        /// <param name="sessionId">Session id</param>
        /// <returns>Return whether session allow to use</returns>
        public bool AllowUse(string sessionId = "", string sessionToken = "")
        {
            var success = DateTimeOffset.Now <= Expires;
            if (success && !string.IsNullOrWhiteSpace(sessionId))
            {
                success = SessionId == sessionId;
            }
            if (success && !string.IsNullOrWhiteSpace(sessionToken))
            {
                success = SessionToken == sessionToken;
            }
            return success;
        }
    }
}
