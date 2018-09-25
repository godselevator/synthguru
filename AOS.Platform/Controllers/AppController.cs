using AOS.BusinessLayer;
using AOS.DomainModel;
using AOS.Platform.Common;
using AOS.Platform.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace AOS.Platform.Controllers
{
    public class AppController : Controller
    {
        public AppController()
        {
            this.ApplicationDbContext = new ApplicationDbContext();
            this.UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.ApplicationDbContext));
        }

        /// <summary>
        /// Application DB context
        /// </summary>
        protected ApplicationDbContext ApplicationDbContext { get; set; }

        /// <summary>
        /// User manager - attached to application DB context
        /// </summary>
        protected UserManager<ApplicationUser> UserManager { get; set; }

        [HttpGet]
        public ActionResult AppDetailsIndex(int id)
        {
            ViewBag.Message = "App details page";

            // Get current properties
            var currentUser = UserManager.FindById(User.Identity.GetUserId());

            // Get model
            var model = AppCommon.GetAppViewModelForApp(id, currentUser);

            // Depending on if we are logged in, choose the correct view
            if (currentUser == null)
            {
                /// *** EVENT: ANONYMOUSUSERCLICKEDAPP *** ///

                // Get visitors IP-address
                string IPAddress = PlatformCommon.GetIPAddress();

                System.Web.HttpContext context = System.Web.HttpContext.Current;
                string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

                if (!string.IsNullOrEmpty(ipAddress))
                {
                    string[] addresses = ipAddress.Split(',');
                    if (addresses.Length != 0)
                    {
                        IPAddress = addresses[0];
                    }
                }
                else
                    IPAddress = context.Request.ServerVariables["REMOTE_ADDR"];

                // Create notification info
                var notificationInfo = new NotificationInfo()
                {
                    UserId = null,
                    AccountId = 0,
                    AppId = id,
                    IPAddress = IPAddress
                };

                // Create mail info
                var mailInfoList = new List<MailInfo>();

                // Create log event and send mails if applicable
                AOSEventHandler.WriteToLog(EventCode.ANONYMOUSUSERCLICKEDAPP, notificationInfo, mailInfoList, SendMail.Default);

                return View("AppDetailsIndexNLI", model);
            }

            // If we are logged in, check user role 
            var currentAccount = PlatformCommon.GetCurrentAccountForUser(currentUser.Id);
            var currentRole = PlatformCommon.GetCurrentRoleForUser(currentUser.Id);

            model.IsNormalUser = (currentRole.Name.ToLower() == "user") ? true : false;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NotifyExistingUser(int AppId, bool OnPremise)
        {
            // Get current user
            var currentUser = UserManager.FindById(User.Identity.GetUserId());
            var currentAccount = AccountCommon.GetCurrentAccountForUser(currentUser.Id);

            var businessLayer = new BusinessLayer.BusinessLayer();

            var app = businessLayer.GetAppById(AppId);

            // Create notification info
            var notificationInfo = new NotificationInfo()
            {
                UserId = currentUser.Id,
                AccountId = currentAccount.AccountID,
                AppId = app.AppID,
                IPAddress = PlatformCommon.GetIPAddress()
            };

            // Create mail info
            var mailInfoList = new List<MailInfo>()
                {
                    new MailInfo
                    {
                        TargetEmail = currentUser.Email,
                        Parameters = new string[] { currentUser.FirstNameLastName, app.Name, app.Name, currentUser.FirstNameLastName, currentUser.Email }
                    }
                };

            if (OnPremise)
            {
                /// *** EVENT: NOTIFYUSERONPREMBETA *** ///

                // Create log event and send mails if applicable
                AOSEventHandler.WriteToLog(EventCode.NOTIFYUSERONPREMBETA, notificationInfo, mailInfoList, SendMail.Default);
            }
            else
            {
                /// *** EVENT: NOTIFYUSERONPREMBETA *** ///

                // Create log event and send mails if applicable
                AOSEventHandler.WriteToLog(EventCode.NOTIFYUSERSOONLINEBETA, notificationInfo, mailInfoList, SendMail.Default);
            }

            return RedirectToAction("AppDetailsIndex", "App", new { Id = AppId });
        }

        // GET
        [HttpGet]
        public ActionResult NotifyMeNLI(int Id, PurchaseVersion Version)
        {
            var model = AppCommon.GetNotifyUserViewModel(Id, Version);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendNotificationData(NotifyUserViewModel model)
        {
            // Create return Json object
            var resp = new AOSResponse();

            if (!ModelState.IsValid)
            {
                resp.IsOK = false;
                resp.ErrorMsg = "Validation failed. Please correct and submit form again";
                return Json(resp, "json", JsonRequestBehavior.AllowGet);
            }

            var businessLayer = new BusinessLayer.BusinessLayer();

            // Get visitors IP-address
            string IPAddress = PlatformCommon.GetIPAddress();

            System.Web.HttpContext context = System.Web.HttpContext.Current;
            string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipAddress))
            {
                string[] addresses = ipAddress.Split(',');
                if (addresses.Length != 0)
                {
                    IPAddress = addresses[0];
                }
            }
            else
                IPAddress = context.Request.ServerVariables["REMOTE_ADDR"];

            // Create notification info
            var notificationInfo = new NotificationInfo()
            {
                UserId = null,
                AccountId = 0,
                AppId = model.AppID,
                IPAddress = IPAddress
            };

            // Create mail info
            var mailInfoList = new List<MailInfo>()
                {
                    new MailInfo
                    {
                        TargetEmail = model.Email,
                        Parameters = new string[] { model.FirstNameLastName, model.AppName, model.AppName, model.FirstNameLastName, model.Email }
                    }
                };

            if (model.OnPremiseBetaVersion)
            {
                /// *** EVENT: NOTIFYUSERONPREMBETA *** ///

                // Create log event and send mails if applicable
                AOSEventHandler.WriteToLog(EventCode.NOTIFYUSERONPREMBETA, notificationInfo, mailInfoList, SendMail.Default);
            }
            else
            {
                /// *** EVENT: NOTIFYUSERONPREMBETA *** ///

                // Create log event and send mails if applicable
                AOSEventHandler.WriteToLog(EventCode.NOTIFYUSERSOONLINEBETA, notificationInfo, mailInfoList, SendMail.Default);
            }

            return Json(resp, "json", JsonRequestBehavior.AllowGet);
        }

        //[Authorize]
        [HttpGet]
        public ActionResult BuyApp(int id, PurchaseVersion OnPremisePurchaseVersion, PurchaseVersion SOOnlinePurchaseVersion)
        {
            // If user is not logged in return to preliminary page
            if (!User.Identity.IsAuthenticated)
            {
                /// *** EVENT: ANONYMOUSUSERBUYAPP *** ///

                // Get visitors IP-address
                string IPAddress = PlatformCommon.GetIPAddress();

                System.Web.HttpContext context = System.Web.HttpContext.Current;
                string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

                if (!string.IsNullOrEmpty(ipAddress))
                {
                    string[] addresses = ipAddress.Split(',');
                    if (addresses.Length != 0)
                    {
                        IPAddress = addresses[0];
                    }
                }
                else
                    IPAddress = context.Request.ServerVariables["REMOTE_ADDR"];

                // Create notification info
                var notificationInfo = new NotificationInfo()
                {
                    UserId = null,
                    AccountId = 0,
                    AppId = id,
                    IPAddress = IPAddress
                };

                // Create mail info
                var mailInfoList = new List<MailInfo>();

                // Create log event and send mails if applicable
                AOSEventHandler.WriteToLog(EventCode.ANONYMOUSUSERBUYAPP, notificationInfo, mailInfoList, SendMail.Default);

                // Get model
                var model = AppCommon.GetBuyTryAppViewModel(id, false);
                model.SOStartURL += "?IsTrialVersion=false";
                model.OnPremisePurchaseVersion = OnPremisePurchaseVersion;
                model.SOOnlinePurchaseVersion = SOOnlinePurchaseVersion;
                model.IsTrial = false;
                model.AppID = id;

                return View("BuyTryNotAuthenticated", model);
            }

            // Get current properties
            var currentUser = UserManager.FindById(User.Identity.GetUserId());
            var currentAccount = PlatformCommon.GetCurrentAccountForUser(currentUser.Id);
            var currentRole = PlatformCommon.GetCurrentRoleForUser(currentUser.Id);

            // If action has been called from browser without using the official GUI, check user role and redirect back if role = "User"
            if (currentRole.Name.ToLower() == "user")
                return RedirectToAction("UserHome", "User");

            // Get businesslayer
            IBusinessLayer businessLayer = new BusinessLayer.BusinessLayer(currentUser.Id);

            // Get app record
            var currentApp = businessLayer.GetAppById(id);

            // Check if account already has app. 
            var upgradeFromTrial = false;
            var purchaseText = "You are about to buy the " + currentApp.Name + " app...";
            var existing = businessLayer.GetAccountAppByAccountIdAndAppId(currentAccount.AccountID, id);
            if (existing != null)
            {
                // Check if we should change from trial to full version
                if (!existing.IsTrial)
                    return RedirectToAction("UserHome", "User");
                else
                {
                    purchaseText = "You are about to change your trial version of the " + currentApp.Name + " app to the full version...";
                    upgradeFromTrial = true;
                }
            }

            // Get model
            var viewModel = AppCommon.GetBuyTryAppViewModel(id, false);
            viewModel.UpgradeFromTrial = upgradeFromTrial;
            viewModel.PurchaseText = purchaseText;

            // Return view with model
            return View(viewModel);
        }

        //[Authorize]
        [HttpGet]
        public ActionResult TryApp(int id, PurchaseVersion OnPremisePurchaseVersion, PurchaseVersion SOOnlinePurchaseVersion)
        {
            // If user is not logged in return to preliminary page
            if (!User.Identity.IsAuthenticated)
            {
                /// *** EVENT: ANONYMOUSUSERTRYAPP *** ///

                // Get visitors IP-address
                string IPAddress = PlatformCommon.GetIPAddress();

                System.Web.HttpContext context = System.Web.HttpContext.Current;
                string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

                if (!string.IsNullOrEmpty(ipAddress))
                {
                    string[] addresses = ipAddress.Split(',');
                    if (addresses.Length != 0)
                    {
                        IPAddress = addresses[0];
                    }
                }
                else
                    IPAddress = context.Request.ServerVariables["REMOTE_ADDR"];

                // Create notification info
                var notificationInfo = new NotificationInfo()
                {
                    UserId = null,
                    AccountId = 0,
                    AppId = id,
                    IPAddress = IPAddress
                };

                // Create mail info
                var mailInfoList = new List<MailInfo>();

                // Create log event and send mails if applicable
                AOSEventHandler.WriteToLog(EventCode.ANONYMOUSUSERTRYAPP, notificationInfo, mailInfoList, SendMail.Default);

                // Get model
                var model = AppCommon.GetBuyTryAppViewModel(id, true);
                model.SOStartURL += "?IsTrialVersion=true";
                model.OnPremisePurchaseVersion = OnPremisePurchaseVersion;
                model.SOOnlinePurchaseVersion = SOOnlinePurchaseVersion;

                // Calculate trial dates
                model.TrialStart = DateTime.Now;
                model.TrialExpires = model.TrialStart.AddDays(model.TrialDays);
                model.IsTrial = true;
                model.AppID = id;

                return View("BuyTryNotAuthenticated", model);
            }

            // Get current properties
            var currentUser = UserManager.FindById(User.Identity.GetUserId());
            var currentAccount = PlatformCommon.GetCurrentAccountForUser(currentUser.Id);
            var currentRole = PlatformCommon.GetCurrentRoleForUser(currentUser.Id);

            // If action has been called from browser without using the official GUI, check user role and redirect back if role = "User"
            if (currentRole.Name.ToLower() == "user")
                return RedirectToAction("UserHome", "User");

            // Get businesslayer
            IBusinessLayer businessLayer = new BusinessLayer.BusinessLayer(currentUser.Id);

            // Check if account already has app
            var existing = businessLayer.GetAccountAppByAccountIdAndAppId(currentAccount.AccountID, id);
            if (existing != null)
                return RedirectToAction("UserHome", "User");

            // Get model
            var model2 = AppCommon.GetBuyTryAppViewModel(id, true);
            model2.SOStartURL += "?IsTrialVersion=true";
            model2.UpgradeFromTrial = false;

            // Calculate trial dates
            model2.TrialStart = DateTime.Now;
            model2.TrialExpires = model2.TrialStart.AddDays(model2.TrialDays);

            // Return view with model
            return View(model2);
        }

        [Authorize]
        public ActionResult ConfirmPurchase(int Id, bool isTrial, bool upgradeFromTrial)
        {
            // Get current properties
            var currentUser = UserManager.FindById(User.Identity.GetUserId());
            var currentAccount = PlatformCommon.GetCurrentAccountForUser(currentUser.Id);

            // Get businesslayer
            IBusinessLayer businessLayer = new BusinessLayer.BusinessLayer(currentUser.Id);

            // Get current app
            var app = businessLayer.GetAppById(Id);

            // Check if account already has app
            var existing = businessLayer.GetAccountAppByAccountIdAndAppId(currentAccount.AccountID, app.AppID);

            // Create notification info
            var notificationInfo = new NotificationInfo()
            {
                UserId = currentUser.Id,
                AccountId = currentAccount.AccountID,
                AppId = app.AppID,
                IPAddress = PlatformCommon.GetIPAddress()
            };

            if (existing == null)
            {
                // Create account/app association
                var accountApp = new AccountApp();
                accountApp.AccountID = currentAccount.AccountID;
                accountApp.AppID = app.AppID;
                accountApp.Activated = true;
                accountApp.Installed = true;
                accountApp.IsTrial = isTrial;
                accountApp.UserPoolCount = app.UserPoolCount;

                // Calculate trial period
                if (accountApp.IsTrial)
                {
                    accountApp.TrialStart = DateTime.Now;

                    // Check trialdays on app. If not found, default to 14.
                    if (app.TrialDays == null || app.TrialDays == 0)
                        app.TrialDays = 14;

                    accountApp.TrialExpire = DateTime.Now.AddDays((int)app.TrialDays);
                    accountApp.AppExpires = DateTime.Now.AddDays((int)app.TrialDays);

                    var callbackUrl = Url.Action("BuyApp", "App", new { id = app.AppID }, protocol: Request.Url.Scheme);

                    /// *** EVENT: TRYAPPCOMPLETED *** ///

                    // Create mail info
                    var mailInfoList = new List<MailInfo>()
                    {
                        new MailInfo
                        {
                            TargetEmail = currentUser.Email,
                            Parameters = new string[] {
                                currentUser.FirstNameLastName,
                                app.Name,
                                currentAccount.Name,
                                app.TrialDays.ToString(),
                                Convert.ToDateTime(accountApp.TrialExpire).ToString("dd-MM-yyyy") }
                        }
                        //new MailInfo
                        //{
                        //    TargetEmail = currentUser.Email,
                        //    Parameters = new string[] {
                        //        currentUser.FirstNameLastName,
                        //        app.Name,
                        //        currentAccount.Name,
                        //        app.TrialDays.ToString(),
                        //        Convert.ToDateTime(accountApp.TrialExpire).ToString("dd-MM-yyyy"),
                        //        callbackUrl  }
                        //}
                    };

                    // Create log event and send mails if applicable
                    AOSEventHandler.WriteToLog(EventCode.TRYAPPCOMPLETED, notificationInfo, mailInfoList, SendMail.Default);

                    // Create 2nd log event
                    var mailInfoList2 = new List<MailInfo>();
                    AOSEventHandler.WriteToLog(EventCode.APPTRIALPERIODSTARTED, notificationInfo, mailInfoList2, SendMail.Default);
                }
                else
                {
                    // Check trialdays on app. If not found, default to 14.
                    if (app.TrialDays == null || app.TrialDays == 0)
                        app.TrialDays = 14;

                    accountApp.AppExpires = DateTime.Now.AddDays((int)app.TrialDays);

                    /// *** EVENT: BUYAPPCOMPLETED *** ///

                    // Create mail info
                    var mailInfoList = new List<MailInfo>()
                    {
                        new MailInfo
                        {
                            TargetEmail = currentUser.Email,
                            Parameters = new string[] { currentUser.FirstNameLastName, app.Name, currentAccount.Name  }
                        }
                    };

                    // Create log event and send mails if applicable
                    AOSEventHandler.WriteToLog(EventCode.BUYAPPCOMPLETED, notificationInfo, mailInfoList, SendMail.Default);
                }

                businessLayer.AddAccountApp(accountApp);
            }
            else // Check if we should upgrade from trial version
            {
                if (upgradeFromTrial)
                {
                    existing.IsTrial = false;
                    existing.TrialStart = null;
                    existing.TrialExpire = null;
                    existing.AppExpires = DateTime.Now.AddDays(5);
                    existing.IsConverted = true;

                    businessLayer.UpdateAccountApp(existing);
                }

                /// *** EVENT: TRIALVERSIONCHANGEDTOFULLVERSION *** ///

                // Create mail info
                var mailInfoList = new List<MailInfo>();

                // Create log event and send mails if applicable
                AOSEventHandler.WriteToLog(EventCode.TRIALVERSIONCHANGEDTOFULLVERSION, notificationInfo, mailInfoList, SendMail.NoMails);
            }

            return RedirectToAction("UserHome", "User");
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UninstallApp(int id)
        {
            // Get current properties
            var currentUser = UserManager.FindById(User.Identity.GetUserId());
            var currentAccount = PlatformCommon.GetCurrentAccountForUser(currentUser.Id);

            // Get businesslayer
            IBusinessLayer businessLayer = new BusinessLayer.BusinessLayer(currentUser.Id);

            // Get current app
            var app = businessLayer.GetAppById(id);

            // Check if account already has app
            var existing = businessLayer.GetAccountAppByAccountIdAndAppId(currentAccount.AccountID, app.AppID);
            if (existing != null)
            {
                // Delete account/app association
                businessLayer.RemoveAccountApp(existing);

                // Delete AppSystemUserToken entries
                var currAppSystemUserToken = businessLayer.GetAppSystemUserTokenByAppIdAndConnectionIdSingle(app.AppID, (int)currentAccount.ConnectionID);
                if (currAppSystemUserToken != null)
                    businessLayer.RemoveAppSystemUserToken(currAppSystemUserToken);

                // Delete AccountAppAssociate entries
                var accountAppAssociateList = businessLayer.GetAccountAppAssociatesByAccountIdAndAppId(currentAccount.AccountID, app.AppID);

                foreach (var item in accountAppAssociateList)
                {
                    // Delete entry
                    businessLayer.RemoveAccountAppAssociate(item);
                }

            }

            /// *** EVENT: APPUNINSTALLED *** ///

            // Create notification info
            var notificationInfo = new NotificationInfo()
            {
                UserId = currentUser.Id,
                AccountId = currentAccount.AccountID,
                AppId = id,
                IPAddress = PlatformCommon.GetIPAddress()
            };

            // Create mail info
            var mailInfoList = new List<MailInfo>()
            {
                new MailInfo
                {
                    TargetEmail = currentUser.Email,
                    Parameters = new string[] { currentUser.FirstNameLastName, app.Name, currentAccount.Name }
                }
            };

            // Create log event and send mails if applicable
            AOSEventHandler.WriteToLog(EventCode.APPUNINSTALLED, notificationInfo, mailInfoList, SendMail.Default);

            return RedirectToAction("UserHome", "User");
        }

    }
}