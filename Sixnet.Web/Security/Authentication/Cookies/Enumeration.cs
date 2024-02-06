using System;

namespace Sixnet.Web.Security.Authentication.Cookies
{
    /// <summary>
    /// Defines cookie storage model
    /// </summary>
    [Serializable]
    public enum CookieStorageModel
    {
        Default = 110,
        InMemory = 120,
        Distributed = 130
    }
}
