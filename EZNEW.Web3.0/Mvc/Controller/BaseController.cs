using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using EZNEW.Web.Security.Authentication;

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
        /// <param name="jsonSerializerOptions">Json serializer options</param>
        /// <returns>Return json result</returns>
        public virtual JsonResult Json(object data, JsonSerializerOptions jsonSerializerOptions)
        {
            if (jsonSerializerOptions == null)
            {
                var jsonOptions = (HttpContext.RequestServices.GetService(typeof(IOptions<JsonOptions>)) as IOptions<JsonOptions>)?.Value;
                jsonSerializerOptions = jsonOptions?.JsonSerializerOptions;
            }
            return new CustomJsonResult(data, jsonSerializerOptions);
        }

        /// <summary>
        /// Gets json result
        /// </summary>
        /// <param name="data">Data</param>
        /// <returns>Return json result</returns>
        public override JsonResult Json(object data)
        {
            JsonSerializerOptions jsonSerializerOptions = null;
            return Json(data, jsonSerializerOptions);
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
