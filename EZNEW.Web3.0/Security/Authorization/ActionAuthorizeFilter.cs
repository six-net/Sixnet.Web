using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using EZNEW.Application;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace EZNEW.Web.Security.Authorization
{
    /// <summary>
    /// Action authorize filter
    /// </summary>
    public class ActionAuthorizeFilter : AuthorizeFilter
    {
        private static readonly AuthorizationPolicy policy = new AuthorizationPolicy(new[] { new DenyAnonymousAuthorizationRequirement() }, new string[] { });

        public ActionAuthorizeFilter() : base(policy)
        { }

        private static bool HasAllowAnonymous(AuthorizationFilterContext context)
        {
            var filters = context.Filters;
            for (var i = 0; i < filters.Count; i++)
            {
                if (filters[i] is IAllowAnonymousFilter)
                {
                    return true;
                }
            }
            var endpoint = context.HttpContext.GetEndpoint();
            if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
            {
                return true;
            }
            return false;
        }

        public override async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            await base.OnAuthorizationAsync(context).ConfigureAwait(false);
            if (context.Result != null && (context.Result is ChallengeResult || context.Result is ForbidResult))
            {
                return;
            }
            if (HasAllowAnonymous(context))//allow anonymous access
            {
                return;
            }
            bool isAuthenticated = context.HttpContext.User?.Identity?.IsAuthenticated ?? false;
            if (!isAuthenticated)
            {
                context.Result = new ChallengeResult();
                return;
            }
            var verifyResult = await AuthorizationManager.AuthorizeAsync(new AuthorizeOptions()
            {
                Controller = context.RouteData.Values["controller"]?.ToString() ?? string.Empty,
                Action = context.RouteData.Values["action"]?.ToString() ?? string.Empty,
                Area = context.RouteData.Values["area"]?.ToString() ?? string.Empty,
                Application = ApplicationManager.Current,
                Method = context?.HttpContext?.Request?.Method,
                Claims = context.HttpContext.User?.Claims?.ToDictionary(c => c.Type, c => c.Value) ?? new Dictionary<string, string>(0),
                ActionContext = context
            }).ConfigureAwait(false);
            switch (verifyResult.Status)
            {
                case AuthorizationStatus.Challenge:
                    context.Result = new ChallengeResult();
                    break;
                case AuthorizationStatus.Forbid:
                default:
                    context.Result = new ForbidResult();
                    break;
                case AuthorizationStatus.Success:
                    break;
            }
        }
    }
}
