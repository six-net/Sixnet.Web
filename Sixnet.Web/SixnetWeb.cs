using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Sixnet.Web.Extensions;

namespace Sixnet.Web
{
    /// <summary>
    /// Sixnet web
    /// </summary>
    public static class SixnetWeb
    {
        /// <summary>
        /// Run a web service
        /// </summary>
        /// <param name="args">args</param>
        /// <param name="configure">configure</param>
        public static void Run(string[] args, Action<SixnetWebOptions> configure = null)
        {
            WebApplication.CreateBuilder(args).RunWeb(configure);
        }
    }
}
