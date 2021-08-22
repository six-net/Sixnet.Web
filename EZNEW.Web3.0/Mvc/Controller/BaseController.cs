using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using EZNEW.Web.Security.Authentication;
using EZNEW.Serialization.Json;

namespace EZNEW.Web.Mvc
{
    /// <summary>
    /// Base controller
    /// </summary>
    public class BaseController : Controller
    {
        #region Properties

        /// <summary>
        /// Gets whether is post request
        /// </summary>
        public bool IsPost
        {
            get
            {
                return HttpMethods.IsPost(HttpContext.Request.Method);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets json result
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="jsonSerializationOptions">Json serialization options</param>
        /// <returns>Return json result</returns>
        [NonAction]
        public virtual JsonResult JsonResult(object data, JsonSerializationOptions jsonSerializationOptions = null)
        {
            JsonSerializerOptions serializerOptions = jsonSerializationOptions?.ConvertToJsonSerializerOptions();
            if (serializerOptions == null)
            {
                var jsonOptions = (HttpContext.RequestServices.GetService(typeof(IOptions<JsonOptions>)) as IOptions<JsonOptions>)?.Value;
                serializerOptions = jsonOptions?.JsonSerializerOptions;
            }
            return new JsonResult(data, serializerOptions);
        }

        #endregion
    }

    /// <summary>
    /// Base controller
    /// </summary>
    public class BaseController<TIdentityKey> : BaseController
    {
        /// <summary>
        /// Gets the request user
        /// </summary>
        public new AuthenticationUser<TIdentityKey> User
        {
            get
            {
                return AuthenticationUser<TIdentityKey>.GetUserFromClaims(HttpContext.User.Claims);
            }
        }
    }
}
