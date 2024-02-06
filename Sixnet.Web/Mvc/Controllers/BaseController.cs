using Sixnet.Serialization.Json;
using Sixnet.Session;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Sixnet.Web.Mvc.Controllers
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

        /// <summary>
        /// Gets the request user
        /// </summary>
        public new UserInfo User
        {
            get
            {
                var currentUser = SessionContext.Current?.User;
                currentUser ??= UserInfo.GetUserFromClaims(HttpContext.User.Claims);
                return currentUser;
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
            var serializerOptions = jsonSerializationOptions?.ConvertToJsonSerializerOptions();
            if (serializerOptions == null)
            {
                var jsonOptions = (HttpContext.RequestServices.GetService(typeof(IOptionsMonitor<JsonOptions>)) as IOptionsMonitor<JsonOptions>)?.CurrentValue;
                serializerOptions = jsonOptions?.JsonSerializerOptions;
            }
            return new JsonResult(data, serializerOptions);
        }

        #endregion
    }
}
