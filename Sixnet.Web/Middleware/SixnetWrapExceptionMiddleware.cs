using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Sixnet.Model;
using Sixnet.Session;

namespace Sixnet.Web.Middleware
{
    public class SixnetWrapExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public SixnetWrapExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                var result = SixnetResult.FailedResult(ex.Message);
                await context.Response.WriteAsJsonAsync(result).ConfigureAwait(false);
            }
        }
    }
}
