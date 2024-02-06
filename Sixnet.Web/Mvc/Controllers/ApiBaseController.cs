using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Sixnet.Model;
using IResult = Sixnet.Model.IResult;

namespace Sixnet.Web.Mvc.Controllers
{
    /// <summary>
    /// Defines base controller for api
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class ApiBaseController : BaseController
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Result is ObjectResult objectResult && (objectResult.StatusCode == null || objectResult.StatusCode == StatusCodes.Status200OK))
            {
                var resultValue = objectResult.Value;
                if (resultValue is not IResult)
                {
                    context.Result = new JsonResult(Result.SuccessResult(data: resultValue));
                }
            }
        }
    }
}
