using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Sixnet.Model;

namespace Sixnet.Web.Mvc.Filters
{
    public class SixnetActionFilter : IResultFilter
    {
        public void OnResultExecuted(ResultExecutedContext context)
        {
        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
            // Unify result
            var webOptions = SixnetWeb.Options;
            if (webOptions.UnifyActionResult)
            {
                if (context.Result is ObjectResult objectResult && (objectResult.StatusCode == null
                    || objectResult.StatusCode == StatusCodes.Status200OK))
                {
                    var resultValue = objectResult.Value;
                    if (resultValue is not ISixnetResult)
                    {
                        context.Result = new JsonResult(SixnetResult.SuccessResult(data: resultValue));
                    }
                }
                else if (context.Result is EmptyResult)
                {
                    context.Result = new JsonResult(SixnetResult.SuccessResult());
                }
            }
        }
    }
}
