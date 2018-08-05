using CommonCode.BusinessLayer;
using CommonCode.Web.Models;
using System.IO;
using System.Web.Mvc;

namespace CommonCode.Web.Controllers
{
    public class BaseController : Controller
    {
        public static MessageViewModel Message { get; set; }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            if (Message != null)
            {
                if (Message.TimesSeen > 1)
                {
                    Message = null;
                }
                else
                {
                    Message.TimesSeen++;
                }
            }

            ViewBag.Message = Message;
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            base.OnException(filterContext);
        }

        protected ActionResult HandleSaveResult(DataResult result, int? id = null)
        {
            var success = result.Type == DataResultType.Success;

            Message = new MessageViewModel
            {
                Class = success ? "alert alert-success" : "alert alert-danger",
                Message = success ? "Record updated successfully." : "Operation could not be completed."
            };

            var controllerName = this.ControllerContext.RouteData.Values["Controller"].ToString();

            var url = id > 0
                ? Url.Action("Details", controllerName, new { id })
                : Url.Action("Index", controllerName);

            if (Request.IsAjaxRequest())
            {
                return Json(new { success, url });
            }

            return Redirect(url);
        }

        protected string RenderPartialViewToString(string viewName, object model)
        {
            ViewData.Model = model;

            using (var writer = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, writer);
                viewResult.View.Render(viewContext, writer);

                return writer.GetStringBuilder().ToString();
            }
        }
    }
}