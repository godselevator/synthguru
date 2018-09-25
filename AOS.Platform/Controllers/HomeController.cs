using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using AOS.Platform.Models;
using System.Collections.Generic;
using AOS.BusinessLayer;
using AOS.Platform.Common;
using AOS.DomainModel;

namespace AOS.Platform.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationSignInManager _signInManager;

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set { _signInManager = value; }
        }

        private ApplicationUserManager _userManager;

        public HomeController()
        {
        }

        public HomeController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ActionResult NotFound()
        {
            return View();
        }

        public ActionResult Error(AOSResponse model)
        {
            return View(model);
        }

        public ActionResult Index(string email, string accountId, string usec)
        {
            // Create return Json object
            var resp = new AOSResponse();

            ViewBag.Link = TempData["ViewBagLink"];

            // Create businesslayer object
            IBusinessLayer businessLayer = new BusinessLayer.BusinessLayer();

            // Check if usec ticket is filled. If so, we have been called from AOS panel and must login the user directly if the email is found
            if (!string.IsNullOrEmpty(usec))
            {
                // If email is not filled redirect to AOS welcome page
                if (string.IsNullOrEmpty(email))
                    return RedirectToAction("Index", "Home");

                // If email is filled, but cannot be found in AOS, redirect to AOS welcome page
                var currUser = UserManager.FindByEmail(email);
                if (currUser == null)
                    return RedirectToAction("Index", "Home");

                // Account id must be filled so we can get the account
                if (string.IsNullOrEmpty(accountId))
                {
                    resp.IsOK = false;
                    resp.ErrorMsg = "USEC ticket was filled, but account id was null. Cannot continue";
                    //return Json(resp, "json", JsonRequestBehavior.AllowGet);
                    return View("Error", resp);
                }

                // Attempt to convert account id to numeric value
                int accountIdNum;
                bool result = Int32.TryParse(accountId, out accountIdNum);
                if (!result)
                {
                    resp.IsOK = false;
                    resp.ErrorMsg = string.Format("Attempted conversion of account id failed. Value:{0}. Cannot continue", accountId == null ? "<null>" : accountId);
                    //return Json(resp, "json", JsonRequestBehavior.AllowGet);
                    return View("Error", resp);
                }

                // Get the account
                var currAccount = businessLayer.GetAccountById(accountIdNum);
                if (currUser == null)
                {
                    resp.IsOK = false;
                    resp.ErrorMsg = string.Format("No account found with account id:{0}. Cannot continue", accountIdNum);
                    //return Json(resp, "json", JsonRequestBehavior.AllowGet);
                    return View("Error", resp);
                }

                // User must be member of account
                var currURA = businessLayer.GetUserRoleAccountByUserIdAndAccountId(currUser.Id, accountIdNum);
                if (currUser == null)
                {
                    resp.IsOK = false;
                    resp.ErrorMsg = string.Format("User with email {0} is not a member of account {1}. Cannot continue", email, accountIdNum);
                    //return Json(resp, "json", JsonRequestBehavior.AllowGet);
                    return View("Error", resp);
                }

                // Account must have a connection
                if (currAccount.ConnectionID == null)
                {
                    resp.IsOK = false;
                    resp.ErrorMsg = string.Format("Account {0} has no connection info. Cannot continue", accountIdNum);
                    //return Json(resp, "json", JsonRequestBehavior.AllowGet);
                    return View("Error", resp);
                }

                // Get connection entry
                var currConnection = businessLayer.GetConnectionById(currAccount.ConnectionID);
                if (currConnection == null)
                {
                    resp.IsOK = false;
                    resp.ErrorMsg = string.Format("Connection id:{0} not found. Cannot continue", currAccount.ConnectionID, accountIdNum);
                    //return Json(resp, "json", JsonRequestBehavior.AllowGet);
                    return View("Error", resp);
                }

                // Parse the USEC and compare email adresses
                //SoEmployee person;
                //SoWebService soWS = new SoWebService();

                //person = soWS.GetPersonFromConnectionAndUsec(usec, currConnection);
                //if (!person.GetSuccess)
                //{
                //    resp.IsOK = false;
                //    resp.ErrorMsg = string.Format("USEC parsing error. Error message: {0}. Cannot continue", person.ErrorMessage);
                //    //return Json(resp, "json", JsonRequestBehavior.AllowGet);
                //    return View("Error", resp);
                //}

                //// Check emails
                //if (person.Emails.Count == 0 || person.Emails == null)
                //{
                //    resp.IsOK = false;
                //    resp.ErrorMsg = "No emails were found in person object. Cannot continue";
                //    //return Json(resp, "json", JsonRequestBehavior.AllowGet);
                //    return View("Error", resp);
                //}

                //var emailFound = false;

                //foreach (var emailAddr in person.Emails)
                //{
                //    if (emailAddr.Email.ToLower() == email)
                //    {
                //        emailFound = true;
                //        break;
                //    }
                //}

                //if (!emailFound)
                //{
                //    resp.IsOK = false;
                //    resp.ErrorMsg = string.Format("Email {0} was not found searching through email list in USEC. Cannot continue", email);
                //    return Json(resp, "json", JsonRequestBehavior.AllowGet);
                //}

                // We have a match. Now we can sign in the user
                SignInManager.SignIn(currUser, true, false);

                return RedirectToAction("UserHome", "User");
            }

            // Apps collection
            var apps = businessLayer.GetAllNormalApps();

            var enabledApps = new List<App>();

            foreach (var app in apps)
            {
                if (app.Enabled)
                    enabledApps.Add(app);
            }

            // Create ViewModel
            var model = new AppViewModel();

            model.Apps = enabledApps.ToArray();

            foreach (var app in model.Apps)
            {
                app.AppAboutTextShort = UserCommon.GetAppLogo(app.AppID, false);
            }

            return View(model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [Authorize]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [Authorize]
        public ActionResult NotImplemented()
        {
            return View();
        }
    }
}