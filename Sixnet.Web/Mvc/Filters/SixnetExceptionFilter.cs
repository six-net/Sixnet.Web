﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Sixnet.Logging;
using Sixnet.Model;

namespace Sixnet.Web.Mvc.Filters
{
    /// <summary>
    /// Sixnet exception filter
    /// </summary>
    public class SixnetExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            context.Result = new JsonResult(Result.FailedResult(context.Exception));
            LogManager.LogError(context.Exception, context.Exception?.Message);
        }
    }
}
