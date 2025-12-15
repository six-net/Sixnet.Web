using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;

using Sixnet.App;
using Sixnet.DependencyInjection;
using Sixnet.Security.Authentication;
using Sixnet.Security.Authorization;
using Sixnet.Session;
using Sixnet.Web.Mvc.Controllers;

namespace Sixnet.Web.Security.Authorization
{
    /// <summary>
    /// Extend authorize filter
    /// </summary>
    public class SixnetAuthorizeFilter : AuthorizeFilter
    {
        static readonly SixnetAuthorizationOptions _defaultAuthorizationOptions = new();

        static readonly AuthorizationPolicy policy = new(new[] { new DenyAnonymousAuthorizationRequirement() }, Array.Empty<string>());

        public SixnetAuthorizeFilter() : base(policy) { }

        internal static bool AllowAnonymous(AuthorizationFilterContext context)
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
            var authOptions = SixnetContainer.GetOptions<SixnetAuthorizationOptions>() ?? _defaultAuthorizationOptions;

            // default authorize
            if (!authOptions.IgnoreDefaultAuthorize)
            {
                var originalResult = context.Result;
                await base.OnAuthorizationAsync(context).ConfigureAwait(false);
                if (context.Result != null && ((context.Result is ChallengeResult && !authOptions.IgnoreAuthentication) || context.Result is ForbidResult))
                {
                    return;
                }
                context.Result = originalResult;
            }

            // allow anonymous access
            if (AllowAnonymous(context))
            {
                return;
            }

            // is authenticated
            bool isAuthenticated = context.HttpContext.User?.Identity?.IsAuthenticated ?? false;
            if (!isAuthenticated && !authOptions.IgnoreAuthentication)
            {
                context.Result = new ChallengeResult();
                return;
            }

            // validate token
            var user = UserInfo.GetUserFromPrincipal(context.HttpContext.User);
            var authenOptions = SixnetContainer.GetOptions<SixnetAuthenticationOptions>();
            var tokenValidated = await SixnetAuthenticationManager.ValidateAuthenticationTokenAsync(setting =>
            {
                setting.AppTag = user.AppTag;
                setting.UserId = user.Id;
                setting.Token = user.Token;
                setting.Score = authenOptions?.Score ?? AuthenticationScore.Unlimited;
                setting.IgnoreServerValidation = authenOptions?.IgnoreServerValidation ?? false;
            }).ConfigureAwait(false);
            if (!tokenValidated)
            {
                context.Result = new ChallengeResult();
                return;
            }

            // ignore authorize
            if (IgnoreAuthorize(context))
            {
                return;
            }

            // admin
            var isAdmin = user?.IsAdmin ?? false;
            if (isAdmin && !authOptions.ValidationAdmin)
            {
                return;
            }

            // super action
            if (IsSuperAction(context) && !isAdmin)
            {
                context.Result = new ForbidResult();
                return;
            }

            // validate auth
            var authorizationResult = SixnetAuthorizationResult.SuccessResult();
            if (authOptions.AuthorizeAsync != null && context.ActionDescriptor is ControllerActionDescriptor actionDescriptor)
            {
                var authorizationContext = new SixnetAuthorizationContext()
                {
                    Application = SixnetApplication.Current,
                    Claims = context.HttpContext.User?.Claims?.ToDictionary(c => c.Type, c => c.Value) ?? new Dictionary<string, string>(0),
                    Operation = ControllerActionDescriptorHelper.GetControllerActionDescriptorFullName(actionDescriptor),
                    User = user
                };
                authorizationResult = await authOptions.AuthorizeAsync(authorizationContext).ConfigureAwait(false);
            }
            if (authorizationResult.AllowAccess)
            {
                return;
            }
            if (authorizationResult.RedirectType == AuthorizeRedirectType.Default)
            {
                switch (authorizationResult.Status)
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
                switch (authorizationResult.RedirectType)
                {
                    case AuthorizeRedirectType.RedirectToAction:
                        context.Result = new RedirectToActionResult(authorizationResult.Action, authorizationResult.Controller, authorizationResult.RouteValues);
                        break;
                    case AuthorizeRedirectType.RedirectToRoute:
                        context.Result = new RedirectToRouteResult(authorizationResult.RouteValues);
                        break;
                    case AuthorizeRedirectType.RedirectToUrl:
                        UrlHelper urlHelper = new UrlHelper(context);
                        if (urlHelper.IsLocalUrl(authorizationResult.Url))
                        {
                            context.Result = new LocalRedirectResult(authorizationResult.Url);
                        }
                        else
                        {
                            context.Result = new RedirectResult(authorizationResult.Url);
                        }
                        break;
                }
            }
        }
    }
}
