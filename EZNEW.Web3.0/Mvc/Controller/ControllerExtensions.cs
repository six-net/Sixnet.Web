using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Mvc
{
    public static class ControllerExtensions
    {
        #region RenderViewContent

        /// <summary>
        /// Render view and Return The HTML Content
        /// </summary>
        /// <param name="controller">Controller</param>
        /// <param name="viewName">View name</param>
        /// <param name="model">Model</param>
        /// <param name="masterName">Master name</param>
        /// <param name="partialView">Whether is partial view</param>
        /// <returns>Return view string</returns>
        public static async Task<string> RenderViewContentAsync(this Controller controller, string viewName, object model, string masterName, bool partialView = false)
        {
            controller.ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var actionContext = new ActionContext(controller.HttpContext, controller.RouteData, new ActionDescriptor());
                if (partialView)
                {
                    PartialViewResult partialViewResult = new PartialViewResult()
                    {
                        ViewData = controller.ViewData,
                        TempData = controller.TempData,
                        ViewName = viewName
                    };
                    var partialViewExecutor = controller.HttpContext.RequestServices.GetRequiredService<IActionResultExecutor<PartialViewResult>>() as PartialViewResultExecutor;
                    var pView = partialViewExecutor.FindView(actionContext, partialViewResult).View;
                    var pviewContext = new ViewContext(actionContext, pView, partialViewResult.ViewData, partialViewResult.TempData, sw, new HtmlHelperOptions());
                    await pView.RenderAsync(pviewContext).ConfigureAwait(false);
                    return sw.GetStringBuilder().ToString();
                }

                ViewResult viewResult = new ViewResult()
                {
                    ViewData = controller.ViewData,
                    TempData = controller.TempData,
                    ViewName = viewName
                };
                var viewExecutor = controller.HttpContext.RequestServices.GetRequiredService<ViewResultExecutor>();
                var view = viewExecutor.FindView(actionContext, viewResult).View;
                var viewContext = new ViewContext(actionContext, view, viewResult.ViewData, viewResult.TempData, sw, new HtmlHelperOptions());
                await view.RenderAsync(viewContext).ConfigureAwait(false);
                return sw.GetStringBuilder().ToString();
            }
        }

        #endregion
    }
}
