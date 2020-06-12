using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Formatters.Json.Internal;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EZNEW.Web.Mvc
{
    /// <summary>
    /// Custom Json Result
    /// </summary>
    public class CustomJsonResult : JsonResult
    {
        public CustomJsonResult(object value, JsonSerializerSettings serializerSettings) : base(value, serializerSettings)
        {
        }

        public override Task ExecuteResultAsync(ActionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var services = context.HttpContext.RequestServices;
            var executor = services.GetRequiredService<JsonResultExecutor>();
            return executor.ExecuteAsync(context, this);
        }
    }
}
