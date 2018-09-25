using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using AOS.Platform.Models;
using System.Collections.Generic;
using AOS.Platform.Common;
using AOS.DomainModel;
using System.Web.Script.Serialization;
using System.Web.Security.AntiXss;
using System.Drawing;
using System.Drawing.Imaging;
using System.Xml.Linq;
using System.Net;
using AOS.BusinessLayer;
using X.PagedList;

namespace AOS.Platform.Controllers
{
    [CustomAuthorize(Roles = "System user")]
    public class SysAdminController : Controller
    {
        BusinessLayer.BusinessLayer AOSBLL;

        // Constructor 1
        public SysAdminController()
        {
            AOSBLL = new BusinessLayer.BusinessLayer();
        }

        // Constructor 2
        public SysAdminController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        private ApplicationUserManager _userManager;
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

        private ApplicationSignInManager _signInManager;
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }

            private set { _signInManager = value; }
        }

        BusinessLayer.BusinessLayer BLL = new BusinessLayer.BusinessLayer();

        [HttpGet]
        public ActionResult Test(int? page)
        {
            IEnumerable<int> listOfAccountIds = null;

            // Get accounts from Session (cached list) or from database
            if (Session["ListOfAccountIds"] != null)
                listOfAccountIds = Session["ListOfAccountIds"] as IEnumerable<int>;
            else
            {
                // Get accountslist from database
                listOfAccountIds = AOSBLL.GetAllAccounts().Select(x => x.AccountID);

                // Cache list in Session
                Session["ListOfAccountIds"] = listOfAccountIds;
            }

            var pageNumber = page ?? 1; // if no page was specified in the querystring, default to the first page (1)

            var onePageOfAccountIds = listOfAccountIds.ToPagedList(pageNumber, 10); // will only contain 10 items max because of the pageSize

            var listOfAccounts = new List<Account>();

            foreach (var item in onePageOfAccountIds)
            {
                var currAccount = AOSBLL.GetAccountById(item);

                listOfAccounts.Add(currAccount);
            }

            ViewBag.OnePageOfAccounts = listOfAccounts;
            ViewBag.OnePageOfAccountIds = onePageOfAccountIds;

            return View();

        }

        // GET: Administrator home
        public ActionResult AdministratorHome()
        {
            // Get current properties
            var currentUser = UserManager.FindById(User.Identity.GetUserId());

            // Get UserProperties object
            var currentRole = PlatformCommon.GetCurrentRoleForUser(currentUser.Id);

            if (currentRole.Name.ToLower() == "system user")
            {
                // Create AOS APIuser if he doesn't exist
                ApplicationUser theUser = null;

                var existUser = UserManager.FindByEmail("aoswebapiuser@adwiza.com");
                if (existUser == null)
                {
                    var apiUser = "aoswebapiuser@adwiza.com";
                    var user = new ApplicationUser
                    {
                        UserName = apiUser,
                        Email = apiUser,
                        FirstNameLastName = "AOS APIUser"
                    };

                    var result = UserManager.Create(user);

                    // Check if user was created succesfully
                    if (!result.Succeeded)
                    {
                        // Go to error page
                        var resp2 = new AOSResponse();
                        resp2.IsOK = false;
                        resp2.ErrorMsg = "AOS APIUser could not be created";

                        TempData["AOSModel"] = resp2;

                        return RedirectToAction("Error", "Home", resp2);
                    }

                    // Get newly created user
                    var newUser = UserManager.FindByEmail(apiUser);

                    var code = "Adwiza4711!";

                    // Update user with random password
                    var result2 = UserManager.AddPassword(newUser.Id, code);

                    // Confirm email right away
                    newUser.EmailConfirmed = true;

                    // Set enabled to true
                    newUser.Enabled = true;

                    // Update user
                    UserManager.Update(newUser);

                    theUser = newUser;
                }
                else
                    theUser = existUser;

                // Get model
                var model = SysAdminCommon.GetAdministratorHomeViewModel(0, UserManager);

                // Return view with model
                return View(model);
            }

            // Go to error page
            var resp = new AOSResponse();
            resp.IsOK = false;
            resp.ErrorMsg = "System users only";

            TempData["AOSModel"] = resp;

            return RedirectToAction("Error", "Home", resp);
        }

        public ActionResult GetGlobalsInfo(int globalsId)
        {
            // Get model
            var model = SysAdminCommon.GetAdministratorHomeViewModel(globalsId, UserManager);

            return Json(model, "json", JsonRequestBehavior.AllowGet);
        }

        // POST: Admin home - Update endpoint settings
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateEndpointSettings(AdminHomeEndpointSettingsViewModel model)
        {
            // Create return Json object
            var resp = new AOSResponse();

            // Get current properties
            var currentUser = UserManager.FindById(User.Identity.GetUserId());

            // Update globals using model
            model = SysAdminCommon.UpdateEndpointSettings(currentUser.Id, model);

            if (model == null)
            {
                resp.IsOK = false;
                resp.ErrorMsg = "Error saving globals";
            }

            resp.ErrorMsg = "Settings succesfully saved";
            return Json(resp, "json");

        }

        // POST: Admin home - Update endpoint settings
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateNotificationSettings(AdminHomeNotificationSettingsViewModel model)
        {
            // Create return Json object
            var resp = new AOSResponse();

            // Get current properties
            var currentUser = UserManager.FindById(User.Identity.GetUserId());

            // Update globals using model
            model = SysAdminCommon.UpdateNotificationSettings(currentUser.Id, model);

            if (model == null)
            {
                resp.IsOK = false;
                resp.ErrorMsg = "Error saving globals";
            }

            resp.ErrorMsg = "Settings succesfully saved";
            return Json(resp, "json");

        }

        // POST: Admin home - Update SO Online settings
        [HttpPost]
        [ValidateInput(false)]
        //[ValidateAntiForgeryToken]
        public ActionResult UpdateSOOnlineSettings(AdminHomeSOOnlineSettingsViewModel model)
        {
            // Create return Json object
            var resp = new AOSResponse();

            // Get current properties
            var currentUser = UserManager.FindById(User.Identity.GetUserId());

            // Update globals using model
            model = SysAdminCommon.UpdateSOOnlineSettings(currentUser.Id, model);

            if (model == null)
            {
                resp.IsOK = false;
                resp.ErrorMsg = "Error saving globals";
            }

            resp.Parameter = model.Environment.ToString();
            resp.ErrorMsg = "Settings succesfully saved";

            return Json(resp, "json");
        }

        // POST: Admin home - Update AOS WebAPI settings
        [HttpPost]
        [ValidateInput(false)]
        //[ValidateAntiForgeryToken]
        public ActionResult UpdateAOSWebAPISettings(AdminHomeAOSWebAPISettingsViewModel model)
        {
            // Create return Json object
            var resp = new AOSResponse();

            // Get current properties
            var currentUser = UserManager.FindById(User.Identity.GetUserId());

            // Update globals using model
            model = SysAdminCommon.UpdateAOSWebAPISettings(model);

            if (model == null)
            {
                resp.IsOK = false;
                resp.ErrorMsg = "Error saving globals";
            }

            resp.ErrorMsg = "Settings succesfully saved";

            return Json(resp, "json");
        }

        public ActionResult ChangeNotification(int notificationId, bool sendEmailStatus)
        {
            // Create return Json object
            var resp = new AOSResponse();

            // Get current properties
            var currentUser = UserManager.FindById(User.Identity.GetUserId());

            // Update globals using model
            var result = SysAdminCommon.SaveNotification(currentUser.Id, notificationId, sendEmailStatus);

            if (!result)
            {
                resp.IsOK = false;
                resp.ErrorMsg = "Error saving notification settings";
                return Json(resp, "json");
            }

            resp.ErrorMsg = "Succesfully saved";
            return Json(resp, "json", JsonRequestBehavior.AllowGet);
        }

        // POST: SaveNotification
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveNotificationEmailAddress(AdministratorHomeViewModel model)
        {
            // Create return Json object
            var resp = new AOSResponse();

            if (!PlatformCommon.IsValidEmail(model.NotificationEmail))
            {
                resp.IsOK = false;
                resp.ErrorMsg = "Not a valid email address";
                return Json(resp, "json");
            }

            model.NotificationEmail = AntiXssEncoder.HtmlEncode(model.NotificationEmail, false);

            // Get current properties
            var currentUser = UserManager.FindById(User.Identity.GetUserId());

            // Update globals using model
            model = SysAdminCommon.SaveNotificationEmailAddress(currentUser.Id, model);

            if (model == null)
            {
                resp.IsOK = false;
                resp.ErrorMsg = "Error saving notification settings";
                return Json(resp, "json");
            }

            resp.ErrorMsg = "Notification settings succesfully saved";
            return Json(resp, "json", JsonRequestBehavior.AllowGet);

        }

        // POST: Create new app
        [HttpPost]
        public ActionResult AddNewApp(AppAdminNewApp model)
        {
            // Get current properties
            var currentUser = UserManager.FindById(User.Identity.GetUserId());

            // Create return Json object
            var resp = new AOSResponse();

            if (string.IsNullOrEmpty(model.Code))
            {
                resp.IsOK = false;
                resp.ErrorMsg = "Code must be filled";
                return Json(resp, "json");
            }

            if (string.IsNullOrEmpty(model.Name))
            {
                resp.IsOK = false;
                resp.ErrorMsg = "Name must be filled";
                return Json(resp, "json");
            }

            if (model.Code.Length > 10)
            {
                resp.IsOK = false;
                resp.ErrorMsg = "Code too long. Max. 10 chars";
                return Json(resp, "json");
            }

            if (model.Version.Length > 20)
            {
                resp.IsOK = false;
                resp.ErrorMsg = "Version too long. Max. 20 chars";
                return Json(resp, "json");
            }

            var businessLayer = new BusinessLayer.BusinessLayer(currentUser.Id);

            // Check if app name is already taken
            var existName = businessLayer.GetAppByName(model.Name);
            if (existName != null)
                return null;

            // Check if app name is already taken
            var existCode = businessLayer.GetAppByCode(model.Code);
            if (existCode != null)
                return null;

            // Add the new app
            var newApp = new App()
            {
                Code = model.Code.ToUpper(),
                Name = model.Name,
                Controller = string.Empty,
                Enabled = false,
                AppVersion = model.Version,
                AppAboutText = string.Format("Complete about text for {0} app", model.Name),
                AppAboutTextShort = "https://placehold.it/147x200",
                TrialDays = 14,
                SendExpireMails = false,
                SOStartURL = null,
                SOAdminURL = null,
                OnPremiseState = 2,
                SOOnlineState = 2,
                SOAppID = null,
                ApplicationToken = null,
                GlobalSettingsId = 2
            };

            businessLayer.AddApp(newApp);

            var allApps = businessLayer.GetAllApps();

            var newestAppId = allApps.OrderByDescending(x => x.AppID).First().AppID;

            // Create JSON serialized object to return from Ajax call
            var appAlt = new AppAlt()
            {
                AppID = newestAppId,
                Name = model.Name
            };

            // Serialize model as JSON and save it in the response
            var rValJson = new JavaScriptSerializer().Serialize(appAlt);
            resp.ReturnedJsonData = rValJson;

            resp.ErrorMsg = "New app successfully inserted. Now continue to fill in additional app information";
            return Json(resp, "json");
        }

        // GET: App Administration
        public ActionResult AppAdministrator()
        {
            // Get model
            var model = AppCommon.GetSingleApp(0); // 0 = get first app in database

            // Return view with model
            return View(model);
        }

        public ActionResult GetAppInfo(int appId)
        {
            // Get model
            var model = AppCommon.GetSingleApp(appId);
            return Json(model, "json", JsonRequestBehavior.AllowGet);
        }

        // POST: App Administrator - update General info tab
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateGeneralInfo(AppAdminGeneralInfoViewModel model)
        {
            // Create return Json object
            var resp = new AOSResponse();

            // Get current properties
            var currentUser = UserManager.FindById(User.Identity.GetUserId());

            // Update app using model
            model = AppCommon.UpdateGeneralInfo(currentUser.Id, model);

            if (model == null)
            {
                resp.IsOK = false;
                resp.ErrorMsg = "Error updating app properties";
            }

            return Json(resp, "json");
        }

        // POST: App Administrator - update Maintenance tab
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateMaintenance(AppAdminMaintenanceViewModel model)
        {
            // Create return Json object
            var resp = new AOSResponse();

            // Get current properties
            var currentUser = UserManager.FindById(User.Identity.GetUserId());

            // Update app using model
            model = AppCommon.UpdateMaintenance(currentUser.Id, model);

            if (model == null)
            {
                resp.IsOK = false;
                resp.ErrorMsg = "Error updating app properties";
            }

            return Json(resp, "json");
        }

        // POST: App Administrator - update Logo images
        [HttpPost]
        public ActionResult UploadLogo(int AppId, string LogoKind, int FileSize, string FileName, HttpPostedFileBase LogoFile)
        {
            // Create return Json object
            var resp = new AOSResponse();

            // Check if a file was selected
            if (LogoFile == null)
            {
                resp.IsOK = false;
                resp.ErrorMsg = "No file selected";
                return Json(resp, "json", JsonRequestBehavior.AllowGet);
            }

            // Check filesize
            if (FileSize == 0)
            {
                resp.IsOK = false;
                resp.ErrorMsg = "Filelength is 0";
                return Json(resp, "json", JsonRequestBehavior.AllowGet);
            }

            // Validate width and height
            var formatOK = false;
            var mimeType = "image/unknown";

            using (Image image = Image.FromStream(LogoFile.InputStream, true, true))
            {
                var imgguid = image.RawFormat.Guid;

                foreach (ImageCodecInfo codec in ImageCodecInfo.GetImageDecoders())
                {
                    if (codec.FormatID == imgguid)
                    {
                        mimeType = codec.MimeType;
                        break;
                    }
                }

                if (ImageFormat.Jpeg.Equals(image.RawFormat))
                    formatOK = true;

                if (ImageFormat.Png.Equals(image.RawFormat))
                    formatOK = true;

                if (!formatOK)
                {
                    resp.IsOK = false;
                    resp.ErrorMsg = string.Format("Image was not in the correct format {0}. Must be JPEG or PNG", mimeType);
                    return Json(resp, "json", JsonRequestBehavior.AllowGet);
                }

                if (image.Width != 147)
                {
                    resp.IsOK = false;
                    resp.ErrorMsg = "Image width must be 147 or it will displayed incorrectly";
                    return Json(resp, "json", JsonRequestBehavior.AllowGet);
                }

                if (image.Height != 200)
                {
                    resp.IsOK = false;
                    resp.ErrorMsg = "Image height must be 200 or it will displayed incorrectly";
                    return Json(resp, "json", JsonRequestBehavior.AllowGet);
                }
            }

            // Save logo image
            var appIconInfo = SysAdminCommon.SaveLogo(AppId, LogoKind, LogoFile);

            var rValJson = new JavaScriptSerializer().Serialize(appIconInfo);
            resp.ReturnedJsonData = rValJson;

            return Json(resp, "json", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UploadPrivateKey(int appId, string environment)
        {
            // Create return Json object
            var resp = new AOSResponse();

            try
            {
                if (Request.Files.Count == 0)
                {
                    resp.IsOK = false;
                    resp.ErrorMsg = "No file selected";
                    return Json(resp, "json", JsonRequestBehavior.AllowGet);
                }

                if (Request.Files.Count > 1)
                {
                    resp.IsOK = false;
                    resp.ErrorMsg = "Upload of multiple files not allowed";
                    return Json(resp, "json", JsonRequestBehavior.AllowGet);
                }

                var fileContent = Request.Files[0];
                var fileSize = fileContent.ContentLength;
                var fileName = fileContent.FileName;
                var xmlFile = fileContent.InputStream;

                // Check filesize
                if (fileSize == 0)
                {
                    resp.IsOK = false;
                    resp.ErrorMsg = "Filelength is 0";
                    return Json(resp, "json", JsonRequestBehavior.AllowGet);
                }

                // Check that file is XML file
                if (fileContent.ContentType != "text/xml")
                {
                    resp.IsOK = false;
                    resp.ErrorMsg = "File must be an XML file";
                    return Json(resp, "json", JsonRequestBehavior.AllowGet);
                }

                // Validate XML file against XSD schema to ensure that the private key is valid
                var xDoc = XDocument.Load(xmlFile);

                var validXMLFile = SysAdminCommon.IsValidXml(xDoc, Server.MapPath("~/XSD/PrivateKeySchema.xsd"));
                if (validXMLFile != string.Empty)
                {
                    resp.IsOK = false;
                    resp.ErrorMsg = "File is not a valid PrivateKey file. Errors: " + validXMLFile;
                    return Json(resp, "json", JsonRequestBehavior.AllowGet);
                }

                // Save private key
                var appPKeyInfo = SysAdminCommon.SavePrivateKey(appId, environment, fileContent);

                var rValJson = new JavaScriptSerializer().Serialize(appPKeyInfo);
                resp.ReturnedJsonData = rValJson;
                resp.ErrorMsg = string.Format("Private key for {0} successfully updated", environment.ToUpper());
                return Json(resp, "json", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Upload failed");
            }
        }

        [HttpPost]
        public ActionResult RemovePrivateKey(int appId, string environment)
        {
            // Create return Json object
            var resp = new AOSResponse();

            // Remove private key
            SysAdminCommon.RemovePrivateKey(appId, environment);

            resp.ErrorMsg = string.Format("Private key for {0} successfully updated", environment.ToUpper());
            return Json(resp, "json", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UploadSOCertificate()
        {
            // Create return Json object
            var resp = new AOSResponse();

            try
            {
                if (Request.Files.Count == 0)
                {
                    resp.IsOK = false;
                    resp.ErrorMsg = "No file selected";
                    return Json(resp, "json", JsonRequestBehavior.AllowGet);
                }

                if (Request.Files.Count > 1)
                {
                    resp.IsOK = false;
                    resp.ErrorMsg = "Upload of multiple files not allowed";
                    return Json(resp, "json", JsonRequestBehavior.AllowGet);
                }

                var fileContent = Request.Files[0];
                var fileSize = fileContent.ContentLength;
                var fileName = fileContent.FileName;
                var xmlFile = fileContent.InputStream;

                // Check filesize
                if (fileSize == 0)
                {
                    resp.IsOK = false;
                    resp.ErrorMsg = "Filelength is 0";
                    return Json(resp, "json", JsonRequestBehavior.AllowGet);
                }

                // Check that file is a CRT file
                if (fileContent.ContentType != "application/x-x509-ca-cert")
                {
                    resp.IsOK = false;
                    resp.ErrorMsg = "File must be a CRT (x-x509-ca-cert) file";
                    return Json(resp, "json", JsonRequestBehavior.AllowGet);
                }

                // Save So Certificate
                var certificateInfo = SysAdminCommon.SaveCertificate(fileContent);

                var rValJson = new JavaScriptSerializer().Serialize(certificateInfo);
                resp.ReturnedJsonData = rValJson;
                resp.ErrorMsg = "SO Certificate successfully updated";
                return Json(resp, "json", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Upload failed");
            }
        }

        [HttpPost]
        public ActionResult RemoveSOCertificate()
        {
            // Create return Json object
            var resp = new AOSResponse();

            // Remove private key
            SysAdminCommon.RemoveCertificate();

            resp.ErrorMsg = "SO Certificate successfully removed";
            return Json(resp, "json", JsonRequestBehavior.AllowGet);
        }

        // POST: App Administrator - update Availability
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateAvailability(AppAdminAvailabilityViewModel model)
        {
            // Create return Json object
            var resp = new AOSResponse();

            // Get current properties
            var currentUser = UserManager.FindById(User.Identity.GetUserId());

            //model.AppAboutText = HttpUtility.HtmlEncode(model.AppAboutText);

            // Update app using model
            model = AppCommon.UpdateAvailability(currentUser.Id, model);

            if (model == null)
            {
                resp.IsOK = false;
                resp.ErrorMsg = "Error updating app properties";
            }

            return Json(resp, "json");
        }

        // POST: App Administrator - update URL Info
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateURLInfo(AppAdminURLInfoViewModel model)
        {
            // Create return Json object
            var resp = new AOSResponse();

            // Get current properties
            var currentUser = UserManager.FindById(User.Identity.GetUserId());

            //model.AppAboutText = HttpUtility.HtmlEncode(model.AppAboutText);

            // Update app using model
            model = AppCommon.UpdateURLInfo(currentUser.Id, model);

            if (model == null)
            {
                resp.IsOK = false;
                resp.ErrorMsg = "Error updating app properties";
            }

            return Json(resp, "json");
        }

        // POST: App Administrator - update Description Text
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateDescrText(AppAdminDescrTextViewModel model)
        {
            // Create return Json object
            var resp = new AOSResponse();

            // Get current properties
            var currentUser = UserManager.FindById(User.Identity.GetUserId());

            //model.AppAboutText = HttpUtility.HtmlEncode(model.AppAboutText);

            // Update app using model
            model = AppCommon.UpdateAppDescrText(currentUser.Id, model);

            if (model == null)
            {
                resp.IsOK = false;
                resp.ErrorMsg = "Error updating app properties";
            }

            return Json(resp, "json");
        }

        // POST: App Administrator - update Availability
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateAdvanced(AppAdminAdvancedViewModel model)
        {
            // Create return Json object
            var resp = new AOSResponse();

            // Get current properties
            var currentUser = UserManager.FindById(User.Identity.GetUserId());

            //model.AppAboutText = HttpUtility.HtmlEncode(model.AppAboutText);

            // Update app using model
            model = AppCommon.UpdateAdvanced(currentUser.Id, model);

            if (model == null)
            {
                resp.IsOK = false;
                resp.ErrorMsg = "Error updating app properties";
            }

            return Json(resp, "json");
        }

        public ActionResult ChangeUserStatus(string userId, bool userStatus)
        {
            // Create return Json object
            var resp = new AOSResponse();

            // Get current user
            var currentUser = UserManager.FindById(User.Identity.GetUserId());

            // Get account user
            var accountUser = UserManager.FindById(userId);

            // Change user status (enabled/disabled)
            accountUser.Enabled = userStatus;

            // Update the user
            var result = UserManager.Update(accountUser);

            if (!result.Succeeded)
            {
                resp.IsOK = false;
                resp.ErrorMsg = "An error occurred while attempting to change role: " + result.Errors.ToString();
                return Json(resp, "json", JsonRequestBehavior.AllowGet);
            }

            resp.ErrorMsg = "User successfully updated";
            return Json(resp, "json", JsonRequestBehavior.AllowGet);
        }

        // GET: User Administration
        public ActionResult UserAdministrator()
        {
            // Get current user
            var currentUser = UserManager.FindById(User.Identity.GetUserId());
            var currentAccount = PlatformCommon.GetCurrentAccountForUser(currentUser.Id);

            // Get model
            var model = AccountCommon.GetUserAdministratorViewModel(currentUser, currentAccount);

            // Return view with model
            return View(model);
        }

        public ActionResult UpdateUserRole(string userKey)
        {
            // Create return Json object
            var resp = new AOSResponse();

            // Get current user
            var currentUser = UserManager.FindById(User.Identity.GetUserId());

            // Split userkey
            var splitStr = userKey.Split('_');

            var roleCmd = splitStr[0];
            var roleUserId = splitStr[1];
            var roleAccountName = splitStr[2];

            // Update account user role
            var result = AccountCommon.UpdateUserRole(currentUser, roleCmd, roleUserId, roleAccountName);

            if (!result.ToLower().Contains("role succesfully updated"))
            {
                resp.IsOK = false;
                resp.ErrorMsg = "An error occurred while attempting to change role: " + result;
                return Json(resp, "json", JsonRequestBehavior.AllowGet);
            }

            resp.ErrorMsg = result;
            return Json(resp, "json", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResendResetPasswordToken(string userId)
        {
            // Get current properties
            var currentUser = UserManager.FindById(User.Identity.GetUserId());
            var currentAccount = PlatformCommon.GetCurrentAccountForUser(currentUser.Id);
            var currentOwner = UserManager.FindById(currentAccount.Owner);
            var user = UserManager.FindById(userId);

            string code = UserManager.GeneratePasswordResetToken(user.Id);
            var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);

            /// *** EVENT: USERFORGOTPASSWORD *** ///

            // Create notification info
            var notificationInfo = new NotificationInfo()
            {
                UserId = user.Id,
                AccountId = 0,
                AppId = 0,
                IPAddress = PlatformCommon.GetIPAddress()
            };

            // Create mail info
            var mailInfoList = new List<MailInfo>()
                {
                    new MailInfo
                    {
                        TargetEmail = user.Email,
                        Parameters = new string[] { user.FirstNameLastName, callbackUrl }
                    }
                };

            // Create log event and send mails if applicable
            AOSEventHandler.WriteToLog(EventCode.USERFORGOTPASSWORD, notificationInfo, mailInfoList, SendMail.Default);

            TempData["ViewBagLink"] = callbackUrl;

            // Get model
            var model = AccountCommon.GetUserAdministratorViewModel(currentUser, currentAccount);

            return View("UserAdministrator", model);
        }

        // GET: ConnectionStatus
        public ActionResult HealthCheckConnectionStatus(string connectionType, int connectionId = -1)
        {
            try
            {
                // Check if we are searching for a specific connection
                if (connectionId > -1)
                {
                    // Get connection
                    var conn = BLL.GetConnectionById(connectionId);
                    if (conn == null)
                        return Json("Connection id: " + connectionId + " could not be found", "json", JsonRequestBehavior.AllowGet);
                    else
                    {
                        var resp = AccountCommon.CheckConnection(conn);
                        return Json(resp, "json", JsonRequestBehavior.AllowGet);
                    }
                }

                var model = SysAdminCommon.GetConnectionStatusViewModel(connectionType);

                return Json(model, "json", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var errorMsg = (ex.InnerException == null) ? ex.Message : ex.InnerException.Message;
                return Json(errorMsg, "json", JsonRequestBehavior.AllowGet);
            }
        }

        // GET: HealthCheck
        public ActionResult HealthCheck()
        {
            try
            {
                var model = SysAdminCommon.GetIntegrityCheckViewModel();

                return Json(model, "json", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var errorMsg = (ex.InnerException == null) ? ex.Message : ex.InnerException.Message;
                return Json(errorMsg, "json", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Diagnostics()
        {
            // Get current properties
            var currentUser = UserManager.FindById(User.Identity.GetUserId());

            // Get UserProperties object
            var currentRole = PlatformCommon.GetCurrentRoleForUser(currentUser.Id);

            if (currentRole.Name.ToLower() == "system user")
            {
                // Get model
                var model = SysAdminCommon.GetDiagnosticsViewModel();

                // Return view with model
                return View(model);
            }

            // Go to error page
            var resp = new AOSResponse();
            resp.IsOK = false;
            resp.ErrorMsg = "System users only";

            return RedirectToAction("Error", "Home", resp);
        }

        public ActionResult AOSAPITester()
        {
            // Get current properties
            var currentUser = UserManager.FindById(User.Identity.GetUserId());

            // Get UserProperties object
            var currentRole = PlatformCommon.GetCurrentRoleForUser(currentUser.Id);

            if (currentRole.Name.ToLower() == "system user")
            {
                // Get model
                var model = SysAdminCommon.GetDiagnosticsViewModel();

                // Return view with model
                return View(model);
            }

            // Go to error page
            var resp = new AOSResponse();
            resp.IsOK = false;
            resp.ErrorMsg = "System users only";

            return RedirectToAction("Error", "Home", resp);
        }

        // GET: RemoveAccount
        public ActionResult RemoveAccount(int accountId, bool preview)
        {
            var resp = new RemoveAccountReturnResponse();

            // Counters
            int AccountFoundCount = 0;
            int ConnectionFoundCount = 0;
            int UserFoundCount = 0;
            int AddressBookFoundCount = 0;
            int BisNodeInfoFoundCount = 0;
            int BisNodeManagerFoundCount = 0;
            int ConverterFoundCount = 0;
            int ElasticIOFoundCount = 0;
            int MatchPersonFoundCount = 0;
            int ReferralFoundCount = 0;
            int RelationWiseFoundCount = 0;
            int RWAppointmentTaskFoundCount = 0;
            int RWAppointmentTriggerFoundCount = 0;
            int RWChosenVariableFoundCount = 0;
            int RWSaleTriggerFoundCount = 0;
            int RWTriggerFoundCount = 0;
            int SignicatFoundCount = 0;
            int SignicatAccountSignLanguageFoundCount = 0;
            int SignicatAccountSignMethodFoundCount = 0;
            int SignicatDocumentTemplateFoundCount = 0;
            int SignicatEmailFoundCount = 0;
            int SignicatLoggingFoundCount = 0;
            int SignicatSecureFormFoundCount = 0;
            int PDFManagerFoundCount = 0;
            int ZapierFoundCount = 0;
            int DashboardFoundCount = 0;
            int AppSystemUserTokenFoundCount = 0;
            int AccountAppAssociatesFoundCount = 0;
            int AccountAppFoundCount = 0;
            int UserRoleFoundCount = 0;
            int UserRoleAccountFoundCount = 0;

            int AccountDeletedCount = 0;
            int ConnectionDeletedCount = 0;
            int UserDeletedCount = 0;
            int AddressBookDeletedCount = 0;
            int BisNodeInfoDeletedCount = 0;
            int BisNodeManagerDeletedCount = 0;
            int ConverterDeletedCount = 0;
            int ElasticIODeletedCount = 0;
            int MatchPersonDeletedCount = 0;
            int ReferralDeletedCount = 0;
            int RelationWiseDeletedCount = 0;
            int RWAppointmentTaskDeletedCount = 0;
            int RWAppointmentTriggerDeletedCount = 0;
            int RWChosenVariableDeletedCount = 0;
            int RWSaleTriggerDeletedCount = 0;
            int RWTriggerDeletedCount = 0;
            int SignicatDeletedCount = 0;
            int SignicatAccountSignLanguageDeletedCount = 0;
            int SignicatAccountSignMethodDeletedCount = 0;
            int SignicatDocumentTemplateDeletedCount = 0;
            int SignicatEmailDeletedCount = 0;
            int SignicatLoggingDeletedCount = 0;
            int SignicatSecureFormDeletedCount = 0;
            int PDFManagerDeletedCount = 0;
            int ZapierDeletedCount = 0;
            int DashboardDeletedCount = 0;
            int AppSystemUserTokenDeletedCount = 0;
            int AccountAppAssociatesDeletedCount = 0;
            int AccountAppDeletedCount = 0;
            int UserRoleDeletedCount = 0;
            int UserRoleAccountDeletedCount = 0;

            // Get businesslayer
            var businessLayer = new BusinessLayer.BusinessLayer();

            if (accountId == 0)
            {
                resp.IsOK = false;
                resp.ReturnMsg = "Account id must be filled";
                return Json(resp, "json", JsonRequestBehavior.AllowGet);
            }

            // Get account
            var currAccount = businessLayer.GetAccountById(accountId);
            if (currAccount == null)
            {
                resp.IsOK = false;
                resp.ReturnMsg = "Account with id " + accountId + " could not be found";
                return Json(resp, "json", JsonRequestBehavior.AllowGet);
            }

            AccountFoundCount++;

            if (String.IsNullOrEmpty(currAccount.Owner))
            {
                resp.IsOK = false;
                resp.ReturnMsg = "Account with name " + accountId + " was found, but owner info is not present";
                return Json(resp, "json", JsonRequestBehavior.AllowGet);
            }

            // Get owner
            var currOwner = businessLayer.GetUserById(currAccount.Owner);
            //if (currOwner == null)
            //{
            //    resp.IsOK = false;
            //    resp.ReturnMsg = "Account with name " + accountId + " was found, but owner with userid " + currAccount.Owner + " was not found";
            //    return Json(resp, "json", JsonRequestBehavior.AllowGet);
            //}

            if (currOwner != null)
                UserFoundCount++;

            // *** Delete all foreign key table entries *** //

            try
            {
                // Delete AddressBook entries
                var addressBookList = businessLayer.GetAddressbooksByAccount(currAccount.AccountID);
                AddressBookFoundCount = addressBookList.Count;
                if (addressBookList.Count > 0)
                {
                    foreach (var item in addressBookList)
                    {
                        if (!preview)
                        {
                            businessLayer.RemoveAddressbook(item);
                            AddressBookDeletedCount++;
                        }
                    }
                }

                // Delete BisNodeInfo entries
                var bisNodeList = businessLayer.GetBisNodesInfoByAccountId(currAccount.AccountID);
                BisNodeInfoFoundCount = bisNodeList.Count;
                if (bisNodeList.Count > 0)
                {
                    foreach (var item in bisNodeList)
                    {
                        if (!preview)
                        {
                            businessLayer.RemoveBisNodeInfo(item);
                            BisNodeInfoDeletedCount++;
                        }
                    }
                }

                // Delete BisNodeManager entry
                var bisNodeManagerList = businessLayer.GetBisnodeManagerByAccountId(currAccount.AccountID);
                BisNodeManagerFoundCount = bisNodeManagerList.Count;
                if (bisNodeManagerList.Count > 0)
                {
                    foreach (var item in bisNodeManagerList)
                    {
                        if (!preview)
                        {
                            businessLayer.RemoveBisnodeManager(item);
                            BisNodeManagerDeletedCount++;
                        }
                    }
                }

                // Delete Converter entries
                var converterList = businessLayer.GetConverterByAccountId(currAccount.AccountID);
                ConverterFoundCount = converterList.Count;
                if (converterList.Count > 0)
                {
                    foreach (var item in converterList)
                    {
                        if (!preview)
                        {
                            businessLayer.RemoveConverter(item);
                            ConverterDeletedCount++;
                        }
                        businessLayer.RemoveConverter(item);
                        ConverterDeletedCount++;
                    }
                }

                // Delete ElasticIO entry
                var elasticIO = businessLayer.GetElasticIOByAccountId(currAccount.AccountID);
                if (elasticIO != null)
                {
                    ElasticIOFoundCount++;
                    if (!preview)
                    {
                        businessLayer.RemoveElasticIO(elasticIO);
                        ElasticIODeletedCount++;
                    }
                }

                // Delete MatchPerson entries
                var matchPersonList = businessLayer.GetMatchPersonListByAccountId(currAccount.AccountID);
                MatchPersonFoundCount = matchPersonList.Count;
                if (matchPersonList.Count > 0)
                {
                    foreach (var item in matchPersonList)
                    {
                        if (!preview)
                        {
                            businessLayer.RemoveMatchPerson(item);
                            MatchPersonDeletedCount++;
                        }
                    }
                }

                // Delete Referral entries
                var referralList = businessLayer.GetreferralsByAccount(currAccount.AccountID);
                ReferralFoundCount = referralList.Count;
                if (referralList.Count > 0)
                {
                    foreach (var item in referralList)
                    {
                        if (!preview)
                        {
                            businessLayer.RemoveReferral(item);
                            ReferralDeletedCount++;
                        }
                    }
                }

                // Delete RelationWise entries
                var relationWiseList = businessLayer.GetrelationWisesByAccount(currAccount.AccountID);
                RelationWiseFoundCount = relationWiseList.Count;
                if (relationWiseList.Count > 0)
                {
                    foreach (var item in relationWiseList)
                    {
                        if (!preview)
                        {
                            businessLayer.RemoveRelationWise(item);
                            RelationWiseDeletedCount++;
                        }
                    }
                }

                // Delete RWAppointmentTask entries
                var rwAppointmentTaskList = businessLayer.GetRWAppointmentTaskByAccount(currAccount.AccountID);
                RWAppointmentTaskFoundCount = rwAppointmentTaskList.Count;
                if (rwAppointmentTaskList.Count > 0)
                {
                    foreach (var item in rwAppointmentTaskList)
                    {
                        if (!preview)
                        {
                            businessLayer.RemoveRWAppointmentTask(item);
                            RWAppointmentTaskDeletedCount++;
                        }
                    }
                }

                // Delete RWAppointmentTrigger entries
                var rwAppointmentTriggerList = businessLayer.GetRWAppointmentTriggerByAccount(currAccount.AccountID);
                RWAppointmentTriggerFoundCount = rwAppointmentTriggerList.Count;
                if (rwAppointmentTriggerList.Count > 0)
                {
                    foreach (var item in rwAppointmentTriggerList)
                    {
                        if (!preview)
                        {
                            businessLayer.RemoveRWAppointmentTrigger(item);
                            RWAppointmentTriggerDeletedCount++;
                        }
                    }
                }

                // Delete RWChosenVariable entries
                var rwVariablesList = businessLayer.GetRWChosenVariableByAccount(currAccount.AccountID);
                RWChosenVariableFoundCount = rwVariablesList.Count;
                if (rwVariablesList.Count > 0)
                {
                    foreach (var item in rwVariablesList)
                    {
                        if (!preview)
                        {
                            businessLayer.RemoveRWChosenVariable(item);
                            RWChosenVariableDeletedCount++;
                        }
                    }
                }

                // Delete RWSaleTrigger entries
                var rwSaleTriggerList = businessLayer.GetRWSaleTriggerByAccount(currAccount.AccountID);
                RWSaleTriggerFoundCount = rwSaleTriggerList.Count;
                if (rwSaleTriggerList.Count > 0)
                {
                    foreach (var item in rwSaleTriggerList)
                    {
                        if (!preview)
                        {
                            businessLayer.RemoveRWSaleTrigger(item);
                            RWSaleTriggerDeletedCount++;
                        }
                    }
                }

                // Delete RWTrigger entries
                var rwTriggerList = businessLayer.GetRWTriggerByAccount(currAccount.AccountID);
                RWTriggerFoundCount = rwTriggerList.Count;
                if (rwTriggerList.Count > 0)
                {
                    foreach (var item in rwTriggerList)
                    {
                        if (!preview)
                        {
                            businessLayer.RemoveRWTrigger(item);
                            RWTriggerDeletedCount++;
                        }
                    }
                }

                // Delete Signicat entries
                var signicat = businessLayer.GetSignicatByAccountId(currAccount.AccountID);
                if (signicat != null)
                {
                    SignicatFoundCount++;

                    if (!preview)
                    {
                        businessLayer.RemoveSignicat(signicat);
                        SignicatDeletedCount++;
                    }
                }

                // Delete SignicatAccountSignLanguage entries
                var signicatAccountSignLanguageList = businessLayer.GetSignicatAccountSignLanguageByAccountId(currAccount.AccountID);
                SignicatAccountSignLanguageFoundCount = signicatAccountSignLanguageList.Count;
                if (signicatAccountSignLanguageList.Count > 0)
                {
                    foreach (var item in signicatAccountSignLanguageList)
                    {
                        if (!preview)
                        {
                            businessLayer.RemoveSignicatAccountSignLanguage(item);
                            SignicatAccountSignLanguageDeletedCount++;
                        }
                    }
                }

                // Delete SignicatAccountSignMethod entries
                var signicatAccountSignMethodList = businessLayer.GetSignicatAccountSignMethodByAccountId(currAccount.AccountID);
                SignicatAccountSignMethodFoundCount = signicatAccountSignMethodList.Count;
                if (signicatAccountSignMethodList.Count > 0)
                {
                    foreach (var item in signicatAccountSignMethodList)
                    {
                        if (!preview)
                        {
                            businessLayer.RemoveSignicatAccountSignMethod(item);
                            SignicatAccountSignMethodDeletedCount++;
                        }
                    }
                }

                // Delete SignicatDocumentTemplate entries
                var signicatDocumentTemplateList = businessLayer.GetSignicatDocumentTemplateByAccountId(currAccount.AccountID);
                SignicatDocumentTemplateFoundCount = signicatDocumentTemplateList.Count;
                if (signicatDocumentTemplateList.Count > 0)
                {
                    foreach (var item in signicatDocumentTemplateList)
                    {
                        if (!preview)
                        {
                            businessLayer.RemoveSignicatDocumentTemplate(item);
                            SignicatDocumentTemplateDeletedCount++;
                        }
                    }
                }

                // Delete SignicatEmail entries
                var signicatEmailList = businessLayer.GetSignicatEmailByAccountId(currAccount.AccountID);
                SignicatEmailFoundCount = signicatEmailList.Count;
                if (signicatEmailList.Count > 0)
                {
                    foreach (var item in signicatEmailList)
                    {
                        if (!preview)
                        {
                            businessLayer.RemoveSignicatEmail(item);
                            SignicatEmailDeletedCount++;
                        }
                    }
                }

                // Delete SignicatLogging entries
                var signicatLoggingList = businessLayer.GetSignicatLoggingByAccountId(currAccount.AccountID);
                SignicatLoggingFoundCount = signicatLoggingList.Count;
                if (signicatLoggingList.Count > 0)
                {
                    foreach (var item in signicatLoggingList)
                    {
                        if (!preview)
                        {
                            businessLayer.RemoveSignicatLogging(item);
                            SignicatLoggingDeletedCount++;
                        }
                    }
                }

                // Delete SignicatSecureForm entries
                var signicatSecureFormList = businessLayer.GetSignicatSecureFormsByAccountId(currAccount.AccountID);
                SignicatSecureFormFoundCount = signicatSecureFormList.Count;
                if (signicatSecureFormList.Count > 0)
                {
                    foreach (var item in signicatSecureFormList)
                    {
                        if (!preview)
                        {
                            businessLayer.RemoveSignicatSecureForm(item);
                            SignicatSecureFormDeletedCount++;
                        }
                    }
                }

                // Delete PDF Manager entry
                var pdfManager = businessLayer.GetPDFManagerByAccountId(currAccount.AccountID);
                if (pdfManager != null)
                {
                    PDFManagerFoundCount++;
                    if (!preview)
                    {
                        businessLayer.RemovePDFManager(pdfManager);
                        PDFManagerDeletedCount++;
                    }
                }

                // Delete Zapier entry
                var zapier = businessLayer.GetZapierByAccountId(currAccount.AccountID);
                if (zapier != null)
                {
                    ZapierFoundCount++;
                    if (!preview)
                    {
                        businessLayer.RemoveZapier(zapier);
                        ZapierDeletedCount++;
                    }
                }

                // Delete Dashboard entry
                var dashboard = businessLayer.GetDashboardByAccountId(currAccount.AccountID);
                if (dashboard != null)
                {
                    DashboardFoundCount++;
                    if (!preview)
                    {
                        businessLayer.RemoveDashboard(dashboard);
                        DashboardDeletedCount++;
                    }
                }

                // Delete AccountAppAssociate entries
                var accountAppAssociateList = businessLayer.GetAccountAppAssociatesByAccountId(currAccount.AccountID);
                AccountAppAssociatesFoundCount = accountAppAssociateList.Count;
                if (accountAppAssociateList.Count > 0)
                {
                    foreach (var item in accountAppAssociateList)
                    {
                        if (!preview)
                        {
                            businessLayer.RemoveAccountAppAssociate(item);
                            AccountAppAssociatesDeletedCount++;
                        }
                    }
                }

                // Delete AccountApp entries
                var accountAppList = businessLayer.GetAccountAppsByAccountId(currAccount.AccountID);
                AccountAppFoundCount = accountAppList.Count;
                if (accountAppList.Count > 0)
                {
                    foreach (var item in accountAppList)
                    {
                        if (!preview)
                        {
                            businessLayer.RemoveAccountApp(item);
                            AccountAppDeletedCount++;
                        }
                    }
                }

                // Delete AspNetUserRoles entry
                if (currOwner != null)
                {
                    var UserRoleEntry = businessLayer.GetUserRoleByUserId(currOwner.Id);
                    if (UserRoleEntry != null)
                    {
                        UserRoleFoundCount++;

                        if (!preview)
                        {
                            businessLayer.RemoveUserRole(UserRoleEntry);
                            UserRoleDeletedCount++;
                        }
                    }
                }

                // Delete UAR entries
                var uarList = businessLayer.GetUserRoleAccountByAccountId(currAccount.AccountID);
                UserRoleAccountFoundCount = uarList.Count;
                if (uarList.Count > 0)
                {
                    foreach (var item in uarList)
                    {
                        if (!preview)
                        {
                            businessLayer.RemoveUserRoleAccount(item);
                            UserRoleAccountDeletedCount++;
                        }
                    }
                }

                // Check if account has any connection entry and delete it if so
                if (currAccount.ConnectionID != null)
                {
                    var connId = currAccount.ConnectionID;
                    ConnectionFoundCount++;

                    var tokenList = businessLayer.GetAppSystemUserTokensByConnectionId((int)connId);

                    AppSystemUserTokenFoundCount = tokenList.Count;

                    if (!preview)
                    {
                        // Remove AppSystemUserToken entries first
                        foreach (var token in tokenList)
                        {
                            businessLayer.RemoveAppSystemUserToken(token);
                            AppSystemUserTokenDeletedCount++;
                        }

                        // Remove connection reference from account before we delete the connection itself
                        currAccount.ConnectionID = null;
                        businessLayer.UpdateAccount(currAccount);
                        var currConnection = businessLayer.GetConnectionById(connId);
                        businessLayer.RemoveConnection(currConnection);
                        ConnectionDeletedCount++;
                    }
                }

                // *** Done deleting any foreign key table entries *** //

                // Delete account
                if (!preview)
                {
                    businessLayer.RemoveAccount(currAccount);
                    AccountDeletedCount++;
                }

                // Delete user
                if (!preview)
                {
                    if (currOwner != null)
                    {
                        businessLayer.RemoveUser(currOwner);
                        UserDeletedCount++;
                    }
                }
            }
            catch (Exception e)
            {
                var exceptionMessage = (e.InnerException != null) ? e.InnerException.InnerException.Message : e.Message;

                resp.IsOK = false;
                resp.ReturnMsg = "An exception occurred: " + exceptionMessage;
                return Json(resp, "json", JsonRequestBehavior.AllowGet);
            }

            resp.IsOK = true;

            if (!preview)
            {
                if (currOwner != null)
                    resp.ReturnMsg = string.Format("Account with account id: {0} and name: {1}, its owner: {2} and all underlying tables were succesfully removed", currAccount.AccountID, currAccount.Name, currOwner.Email);
                else
                    resp.ReturnMsg = string.Format("Account with account id: {0} and name: {1} and all underlying tables were succesfully removed", currAccount.AccountID, currAccount.Name, currOwner.Email);
            }
            else
                resp.ReturnMsg = "Preview complete";

            // Construct stats strings
            resp.AccountId = currAccount.AccountID;
            resp.AccountName = currAccount.Name;
            resp.UserId = (currOwner == null) ? "Owner not found" : currOwner.Id;
            resp.UserName = (currOwner == null) ? "Owner not found" : currOwner.Email;
            resp.AccountStats = string.Format("Found: {0}, Deleted: {1}", AccountFoundCount, AccountDeletedCount);
            resp.ConnectionStats = string.Format("Found: {0}, Deleted: {1}", ConnectionFoundCount, ConnectionDeletedCount);
            resp.UserStats = string.Format("Found: {0}, Deleted: {1}", UserFoundCount, UserDeletedCount);
            resp.AddressBookStats = string.Format("Found: {0}, Deleted: {1}", AddressBookFoundCount, AddressBookDeletedCount);
            resp.BisNodeInfoStats = string.Format("Found: {0}, Deleted: {1}", BisNodeInfoFoundCount, BisNodeInfoDeletedCount);
            resp.BisNodeManagerStats = string.Format("Found: {0}, Deleted: {1}", BisNodeManagerFoundCount, BisNodeManagerDeletedCount);
            resp.ConverterStats = string.Format("Found: {0}, Deleted: {1}", ConverterFoundCount, ConverterDeletedCount);
            resp.ElasticIOStats = string.Format("Found: {0}, Deleted: {1}", ElasticIOFoundCount, ElasticIODeletedCount);
            resp.MatchPersonStats = string.Format("Found: {0}, Deleted: {1}", MatchPersonFoundCount, MatchPersonDeletedCount);
            resp.ReferralStats = string.Format("Found: {0}, Deleted: {1}", ReferralFoundCount, ReferralDeletedCount);
            resp.RelationWiseStats = string.Format("Found: {0}, Deleted: {1}", RelationWiseFoundCount, RelationWiseDeletedCount);
            resp.RWAppointmentTaskStats = string.Format("Found: {0}, Deleted: {1}", RWAppointmentTaskFoundCount, RWAppointmentTaskDeletedCount);
            resp.RWAppointmentTriggerStats = string.Format("Found: {0}, Deleted: {1}", RWAppointmentTriggerFoundCount, RWAppointmentTriggerDeletedCount);
            resp.RWChosenVariableStats = string.Format("Found: {0}, Deleted: {1}", RWChosenVariableFoundCount, RWChosenVariableDeletedCount);
            resp.RWSaleTriggerStats = string.Format("Found: {0}, Deleted: {1}", RWSaleTriggerFoundCount, RWSaleTriggerDeletedCount);
            resp.RWTriggerStats = string.Format("Found: {0}, Deleted: {1}", RWTriggerFoundCount, RWTriggerDeletedCount);
            resp.SignicatStats = string.Format("Found: {0}, Deleted: {1}", SignicatFoundCount, SignicatDeletedCount);
            resp.SignicatAccountSignLanguageStats = string.Format("Found: {0}, Deleted: {1}", SignicatAccountSignLanguageFoundCount, SignicatAccountSignLanguageDeletedCount);
            resp.SignicatAccountSignMethodStats = string.Format("Found: {0}, Deleted: {1}", SignicatAccountSignMethodFoundCount, SignicatAccountSignMethodDeletedCount);
            resp.SignicatDocumentTemplateStats = string.Format("Found: {0}, Deleted: {1}", SignicatDocumentTemplateFoundCount, SignicatDocumentTemplateDeletedCount);
            resp.SignicatEmailStats = string.Format("Found: {0}, Deleted: {1}", SignicatEmailFoundCount, SignicatEmailDeletedCount);
            resp.SignicatLoggingStats = string.Format("Found: {0}, Deleted: {1}", SignicatLoggingFoundCount, SignicatLoggingDeletedCount);
            resp.SignicatSecureFormStats = string.Format("Found: {0}, Deleted: {1}", SignicatSecureFormFoundCount, SignicatSecureFormDeletedCount);
            resp.PDFManagerStats = string.Format("Found: {0}, Deleted: {1}", PDFManagerFoundCount, PDFManagerDeletedCount);
            resp.ZapierStats = string.Format("Found: {0}, Deleted: {1}", ZapierFoundCount, ZapierDeletedCount);
            resp.DashboardStats = string.Format("Found: {0}, Deleted: {1}", DashboardFoundCount, DashboardDeletedCount);
            resp.AppSystemUserTokenStats = string.Format("Found: {0}, Deleted: {1}", AppSystemUserTokenFoundCount, AppSystemUserTokenDeletedCount);
            resp.AccountAppAssociatesStats = string.Format("Found: {0}, Deleted: {1}", AccountAppAssociatesFoundCount, AccountAppAssociatesDeletedCount);
            resp.AccountAppStats = string.Format("Found: {0}, Deleted: {1}", AccountAppFoundCount, AccountAppDeletedCount);
            resp.UserRoleStats = string.Format("Found: {0}, Deleted: {1}", UserRoleFoundCount, UserRoleDeletedCount);
            resp.UserRoleAccountStats = string.Format("Found: {0}, Deleted: {1}", UserRoleAccountFoundCount, UserRoleAccountDeletedCount);

            return Json(resp, "json", JsonRequestBehavior.AllowGet);
        }
    }


}