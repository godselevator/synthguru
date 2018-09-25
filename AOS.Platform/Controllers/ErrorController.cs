using AOS.BusinessLayer;
using AOS.Platform.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace AOS.Platform.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult AccessDenied()
        {
            return View();
        }

        public ActionResult NotFound(string url)
        {
            var originalUri = url ?? Request.QueryString["aspxerrorpath"] ?? Request.Url.OriginalString;

            var controllerName = (string)RouteData.Values["controller"];
            var actionName = (string)RouteData.Values["action"];

            var model = new NotFoundModel(new HttpException(404, "Failed to find page"), controllerName, actionName)
            {
                RequestedUrl = originalUri,
                ReferrerUrl = Request.UrlReferrer == null ? "" : Request.UrlReferrer.OriginalString
            };

            Response.StatusCode = 404;
            return View("NotFound", model);
        }

        protected override void HandleUnknownAction(string actionName)
        {
            // HACK! If actionName = Index, then change it to NotFound
            if (actionName == "Index")
                actionName = "Error";

            var name = GetViewName(ControllerContext, string.Format("~/Views/Shared/{0}.cshtml", actionName),
                                                        "~/Views/Error/Error.cshtml",
                                                        "~/Views/Error/General.cshtml",
                                                        "~/Views/Shared/Error.cshtml");

            var controllerName = (string)RouteData.Values["controller"];

            //var exception = Server.GetLastError();
            //if (exception == null)
            //    exception = new Exception("Unknown page");

            //var model = new HandleErrorInfo(exception, controllerName, actionName);
            var model = new AOSResponse()
            {
                IsOK = false,
                ErrorMsg = "Unknown action"
            };

            var result = new ViewResult
            {
                ViewName = name,
                ViewData = new ViewDataDictionary<AOSResponse>(model),
            };

            Response.StatusCode = 501;

            result.ExecuteResult(ControllerContext);
        }

        protected string GetViewName(ControllerContext context, params string[] names)
        {
            foreach (var name in names)
            {
                var result = ViewEngines.Engines.FindView(ControllerContext, name, null);
                if (result.View != null)
                    return name;
            }
            return null;
        }
    }
}