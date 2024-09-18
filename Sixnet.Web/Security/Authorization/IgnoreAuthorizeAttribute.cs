using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sixnet.Web.Security.Authorization
{
    /// <summary>
    /// Defines ignore authorize attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class IgnoreAuthorizeAttribute : Attribute, IFilterMetadata
    {
    }
}
