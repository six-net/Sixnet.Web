using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;
using Sixnet.App;
using Sixnet.Session;
using Sixnet.Web.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sixnet.Web.Security.Authorization
{
    /// <summary>
    /// Extend authorize filter
    /// </summary>
    public class SixnetAuthorizeFilter : AuthorizeFilter
    {
        private static readonly AuthorizationPolicy policy = new AuthorizationPolicy(new[] { new DenyAnonymousAuthorizationRequirement() }, new string[] { });

        public SixnetAuthorizeFilter() : base(policy) { }

        internal static bool HasAllowAnonymous(AuthorizationFilterContext context)
        {
            var filters = context.Filters;
            var anonymousFilter = filters?.Any(f => f is IAllowAnonymousFilter) ?? false;
            if (anonymousFilter)
            {
                return true;
            }
            var endpoint = context.HttpContext.GetEndpoint();
            return endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null;
        }

        internal static bool IgnoreAuthorize(AuthorizationFilterContext context)
        {
            return context?.Filters?.Any(f => f is IgnoreAuthorizeAttribute) ?? false;
        }

        internal static bool IsSuperAction(AuthorizationFilterContext context)
        {
            return context?.Filters?.Any(f => f is SuperActionAttribute) ?? false;
        }

        public override async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (!AuthorizationManager.IngoreDefaultAuthorize)
            {
                var originalResult = context.Result;
                await base.OnAuthorizationAsync(context).ConfigureAwait(false);
                if (context.Result != null && ((context.Result is ChallengeResult && !AuthorizationManager.IngoreAuthentication) || context.Result is ForbidResult))
                {
                    return;
                }
                context.Result = originalResult;
            }
            if (HasAllowAnonymous(context))//allow anonymous access
            {
                return;
            }
            bool isAuthenticated = context.HttpContext.User?.Identity?.IsAuthenticated ?? false;
            if (!isAuthenticated && !AuthorizationManager.IngoreAuthentication)
            {
                context.Result = new ChallengeResult();
                return;
            }
            if (IgnoreAuthorize(context))
            {
                return;
            }
            var user = UserInfo.GetUserFromPrincipal(context.HttpContext.User);
            var isAdmin = user?.IsAdmin ?? false;
            if (IsSuperAction(context) && !isAdmin)
            {
                context.Result = new ForbidResult();
                return;
            }
            var verifyResult = await AuthorizationManager.AuthorizeAsync(new AuthorizeOptions()
            {
                Controller = context.RouteData.Values[MvcConstants.Route.Controller]?.ToString() ?? string.Empty,
                Action = context.RouteData.Values[MvcConstants.Route.Action]?.ToString() ?? string.Empty,
                Area = context.RouteData.Values[MvcConstants.Route.Area]?.ToString() ?? string.Empty,
                Application = SixnetApplication.Current,
                Method = context?.HttpContext?.Request?.Method,
                Claims = context.HttpContext.User?.Claims?.ToDictionary(c => c.Type, c => c.Value) ?? new Dictionary<string, string>(0),
                ActionContext = context,
                ActionDescriptor = context.ActionDescriptor
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
