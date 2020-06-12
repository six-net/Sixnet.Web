using System;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace EZNEW.Web.Mvc
{
    /// <summary>
    /// Custom json result
    /// </summary>
    public class CustomJsonResult : JsonResult
    {
        public CustomJsonResult(object value, JsonSerializerOptions jsonSerializerOptions) : base(value, jsonSerializerOptions)
        {
        }

        public override Task ExecuteResultAsync(ActionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            var services = context.HttpContext.RequestServices;
            var executor = services.GetRequiredService<IActionResultExecutor<JsonResult>>();
            return executor.ExecuteAsync(context, this);
        }
    }
}
