using AOS.BusinessLayer;
using AOS.Platform.Common;
using AOS.Platform.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using System.Web;
using System.Web.Mvc;

namespace AOS.Platform.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        BusinessLayer.BusinessLayer AOSBLL;

        public UserController()
        {
            ApplicationDbContext = new ApplicationDbContext();
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.ApplicationDbContext));

            AOSBLL = new BusinessLayer.BusinessLayer();
        }

        /// <summary>
        /// Application DB context
        /// </summary>
        protected ApplicationDbContext ApplicationDbContext { get; set; }

        /// <summary>
        /// User manager - attached to application DB context
        /// </summary>
        protected UserManager<ApplicationUser> UserManager { get; set; }

        private bool IsAjax(ExceptionContext filterContext)
        {
            return filterContext.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.ExceptionHandled || !filterContext.HttpContext.IsCustomErrorEnabled)
            {
                return;
            }

            // if the request is AJAX return JSON else view.
            if (IsAjax(filterContext))
            {
                //Because its a exception raised after ajax invocation
                //Lets return Json
                filterContext.Result = new JsonResult()
                {
                    Data = filterContext.Exception.Message,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };

                filterContext.ExceptionHandled = true;
                filterContext.HttpContext.Response.Clear();
            }
            else
            {
                //Normal Exception
                //So let it handle by its default ways.
                base.OnException(filterContext);

            }

            // Write error logging code here if you wish.

            //if want to get different of the request
            //var currentController = (string)filterContext.RouteData.Values["controller"];
            //var currentActionName = (string)filterContext.RouteData.Values["action"];
        }

        // GET: Preparations
        [AllowAnonymous]
        public ActionResult Preparations()
        {
            return View();
        }

        // GET: UserHome
        public ActionResult UserHome()
        {
            var resp = new AOSResponse();

            // Get current user
            var currentUser = UserManager.FindById(User.Identity.GetUserId());
            var currentAccount = PlatformCommon.GetCurrentAccountForUser(currentUser.Id);
            var currentOwner = UserManager.FindById(currentAccount.Owner);

            // Get model
            var model = UserCommon.GetUserHomeViewModel(currentUser);

            if (model == null)
            {
                resp.IsOK = false;
                resp.ErrorMsg = "You have no accounts assigned to your user profile";
                return View("Error", resp);
            }

            // Write AOS cookie containg current accound id
            AOSBLL.CreateCookie(CookieType.AOSCookie, currentAccount.AccountID);

            return View(model);
        }

        [AllowAnonymous]
        public ActionResult TermsOfService()
        {
            var model = UserCommon.GetTermsOfServiceViewModel();

            return View(model);
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }


    }

}