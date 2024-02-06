using System;
using Microsoft.Extensions.Options;
using Sixnet.DependencyInjection;

namespace Sixnet.Web.Security.Authentication.Session
{
    /// <summary>
    /// Session configuration
    /// </summary>
    public class SessionConfiguration
    {
        /// <summary>
        /// Gets or sets the expire times
        /// Default is 2h
        /// </summary>
        public TimeSpan Expires { get; set; } = TimeSpan.FromHours(2);

        /// <summary>
        /// Gets or sets session claim name
        /// </summary>
        public string SessionClaimName { get; set; } = "eznew_auth_session_name";

        /// <summary>
        /// Gets session configuration
        /// </summary>
        /// <returns>Return session configuration</returns>
        public static SessionConfiguration GetSessionConfiguration()
        {
            SessionConfiguration sessionConfiguration = null;
            if (ContainerManager.IsRegister<IOptions<SessionConfiguration>>())
            {
                sessionConfiguration = ContainerManager.Resolve<IOptions<SessionConfiguration>>()?.Value;
            }
            if (sessionConfiguration == null)
            {
                sessionConfiguration = new SessionConfiguration();
            }
            return sessionConfiguration;
        }
    }
}
