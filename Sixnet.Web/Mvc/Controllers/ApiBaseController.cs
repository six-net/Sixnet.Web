using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Sixnet.Model;

namespace Sixnet.Web.Mvc.Controllers
{
    /// <summary>
    /// Defines base controller for api
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class ApiBaseController : BaseController
    {
    }
}
