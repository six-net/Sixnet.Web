using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sixnet.App;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;
using Sixnet.Web.Mvc;

namespace Sixnet.Web.Security.Authorization
{
    /// <summary>
    /// Customized authorize filter
    /// </summary>
    public class CustomizedAuthorizeFilter : IAsyncAuthorizationFilter
    {
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (ExtendAuthorizeFilter.HasAllowAnonymous(context))//allow anonymous access
            {
                return;
            }
            bool isAuthenticated = context.HttpContext.User?.Identity?.IsAuthenticated ?? false;
            if (!isAuthenticated && !AuthorizationManager.IngoreAuthentication)
            {
                context.Result = new ChallengeResult();
                return;
            }
            var verifyResult = await AuthorizationManager.AuthorizeAsync(new AuthorizeOptions()
            {
                Controller = context.RouteData.Values[MvcConstants.Route.Controller]?.ToString() ?? string.Empty,
                Action = context.RouteData.Values[MvcConstants.Route.Action]?.ToString() ?? string.Empty,
                Area = context.RouteData.Values[MvcConstants.Route.Area]?.ToString() ?? string.Empty,
                Application = ApplicationManager.Current,
                Method = context?.HttpContext?.Request?.Method,
                Claims = context.HttpContext.User?.Claims?.ToDictionary(c => c.Type, c => c.Value) ?? new Dictionary<string, string>(0),
                ActionContext = context
            }).ConfigureAwait(false);
            if (verifyResult.AllowAccess)
            {
                return;
            }
            if (verifyResult.RedirectType == AuthorizeRedirectType.Default)
            {
                switch (verifyResult.Status)
                {
                    case AuthorizationStatus.Success:
                        break;
                    case AuthorizationStatus.Challenge:
                        context.Result = new ChallengeResult();
                        break;
                    case AuthorizationStatus.Forbid:
                    default:
                        context.Result = new ForbidResult();
                        break;
                }
            }
            else
            {
                switch (verifyResult.RedirectType)
                {
                    case AuthorizeRedirectType.RedirectToAction:
                        context.Result = new RedirectToActionResult(verifyResult.Action, verifyResult.Controller, verifyResult.RouteValues);
                        break;
                    case AuthorizeRedirectType.RedirectToRoute:
                        context.Result = new RedirectToRouteResult(verifyResult.RouteValues);
                        break;
                    case AuthorizeRedirectType.RedirectToUrl:
                        UrlHelper urlHelper = new UrlHelper(context);
                        if (urlHelper.IsLocalUrl(verifyResult.Url))
                        {
                            context.Result = new LocalRedirectResult(verifyResult.Url);
                        }
                        else
                        {
                            context.Result = new RedirectResult(verifyResult.Url);
                        }
                        break;
                }
            }
        }
    }
}
