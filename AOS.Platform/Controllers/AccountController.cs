using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using AOS.Platform.Models;
using System.Collections.Generic;
using AOS.BusinessLayer;
using AOS.Platform.Common;
using AOS.DomainModel;
using System.Web.Script.Serialization;
using AOS.SAMLTokenHandler;
using System.Web.Routing;
using System.Net;
using System.IO;
using System.Text;
using System.Data.Entity.Validation;
using AOS.WebAPIPlugins;
using SuperOffice;

namespace AOS.Platform.Controllers
{
    [HandleError()]
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationUserManager _userManager;
        private BusinessLayer.BusinessLayer AOSBLL;

        // Constructor 1
        public AccountController()
        {
            AOSBLL = new BusinessLayer.BusinessLayer();
        }

        // Constructor 2
        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;

            AOSBLL = new BusinessLayer.BusinessLayer();
        }

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

        // GET: TestPage
        public ActionResult TestPage()
        {
            return View();
        }

        public ActionResult NewTestEndpoint(int accountId)
        {
            // Create return Json object
            var resp = new AOSResponse();



            //if (!result.Succeeded)
            //{
            //    resp.IsOK = false;
            //    resp.ErrorMsg = "An error occurred while attempting to change role: " + result.Errors.ToString();
            //    return Json(resp, "json", JsonRequestBehavior.AllowGet);
            //}

            resp.ErrorMsg = "Up and running...";
            return Json(resp, "json", JsonRequestBehavior.AllowGet);
        }

        public ActionResult AccountSettings()
        {
            var currentUser = UserManager.FindById(User.Identity.GetUserId());

            // Administrator or system user role required to execute this page
            if (!PlatformCommon.HasRole(currentUser, UserRole.Administrator))
                return RedirectToAction("AccessDenied", "Error");

            var model = AccountCommon.GetAccountSettingsViewModel(currentUser);

            return View(model);
        }

        // POST: Update User info
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateAccountSettings(AccountSettingsViewModel model)
        {
            // Create return Json object
            var resp = new AOSResponse();

            // Get current user
            var currentUser = UserManager.FindById(User.Identity.GetUserId());
            var currentAccount = AccountCommon.GetCurrentAccountForUser(currentUser.Id);

            // Administrator or system user role required to execute this page
            if (!PlatformCommon.HasRole(currentUser, UserRole.Administrator))
                return RedirectToAction("AccessDenied", "Error");

            var businessLayer = new BusinessLayer.BusinessLayer(currentUser.Id);

            // Check if user has changed account name - it must be unique
            bool accountNameChanged = currentAccount.Name != model.Name;

            // Ensure updated email does not exist already in database
            if (accountNameChanged)
            {
                var existAccount = businessLayer.GetAccountByName(model.Name);

                if (existAccount != null)
                {
                    resp.IsOK = false;
                    resp.ErrorMsg = "Account does already exist";
                    return Json(resp, "json");
                }
            }

            // Validate country
            var country = businessLayer.GetCountryByName(model.Country);

            if (country == null)
            {
                resp.IsOK = false;
                resp.ErrorMsg = "Invalid country";
                return Json(resp, "json");
            }

            currentAccount.Name = model.Name;

            if (businessLayer.IsSystemUser(currentUser.Id))
                currentAccount.URL = model.URL;

            currentAccount.Address = model.Address;
            currentAccount.Address2 = model.Address2;
            currentAccount.Zip = model.Zip;
            currentAccount.City = model.City;
            currentAccount.CountryID = country.CountryID;
            //currentAccount.Enabled = model.Enabled;
            //currentAccount.IsPartner = model.Partner;
            //currentAccount.Owner = model.Owner;

            // Update account
            businessLayer.UpdateAccount(currentAccount);

            return Json(resp, "json");
        }

        // Only used på testpage
        public ActionResult FindSelections(string search)
        {
            // Get current properties
            var currentUser = UserManager.FindById(User.Identity.GetUserId());

            // Administrator or system user role required to execute this page
            if (!PlatformCommon.HasRole(currentUser, UserRole.Administrator))
                return RedirectToAction("AccessDenied", "Error");

            var typeAheadSelectionList = AccountCommon.SearchSelections(currentUser, search);

            return Json(typeAheadSelectionList, "json", JsonRequestBehavior.AllowGet);
        }

        // GET: IFrameHost
        public ActionResult IFrameHost(string AppId, bool SOOnlineEnabled)
        {
            var resp = new AOSResponse();

            // Get current user
            var currentUser = UserManager.FindById(User.Identity.GetUserId());
            if (currentUser == null)
            {
                resp.IsOK = false;
                resp.ErrorMsg = "IFrameHost ==> User could not be identified";
                return View("Error", resp);
            }

            if (string.IsNullOrEmpty(AppId))
            {
                resp.IsOK = false;
                resp.ErrorMsg = "IFrameHost ==> AppId is null or empty";
                return View("Error", resp);
            }

            // Get UserProperties object
            var model = AccountCommon.GetIFrameHostViewModel(currentUser, Convert.ToInt32(AppId), SOOnlineEnabled);

            if (!model.AppInfo.IsOK)
            {
                resp.IsOK = false;
                resp.ErrorMsg = model.AppInfo.ErrorMsg;
                return View("Error", resp);
            }

            // Return view with model
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TestEndpoint(EndpointViewModel model)
        {
            // Get current properties
            var currentUser = UserManager.FindById(User.Identity.GetUserId());
            var currentAccount = AccountCommon.GetCurrentAccountForUser(currentUser.Id);

            // Administrator or system user role required to execute this page
            if (!PlatformCommon.HasRole(currentUser, UserRole.Administrator))
                return RedirectToAction("AccessDenied", "Error");

            // Get businesslayer
            IBusinessLayer businessLayer = new BusinessLayer.BusinessLayer(currentUser.Id);

            // Create return Json object
            var resp = new AOSResponse();
            resp.ErrorMsg = "Endpoint URL and credentials are valid. Click 'Save connection' to save settings";

            // If endpoint is SO Online connection, then call external test connection routine
            var conn = businessLayer.GetConnectionById(model.ConnectionId);

            if (conn != null)
            {
                if (conn.IsSOOnline)
                {
                    resp = PlatformCommon.TestSOEndpoint(conn);

                    var rVal = new EndpointSOInfo();
                    rVal.SOCompanyName = conn.CompanyName;
                    rVal.SOVersion = conn.WSVersion.ToString();
                    rVal.SONetServerVersion = conn.NetServer;
                    rVal.SOSerial = conn.CompanySerial;
                    rVal.SOHandshakeStatus = "Ok";

                    var rValJson = new JavaScriptSerializer().Serialize(rVal);
                    resp.ReturnedJsonData = rValJson;

                    return Json(resp, "json", JsonRequestBehavior.AllowGet);
                }
            }

            // Append slash '/' to endpoint url if missing
            if (!model.EndpointURI.EndsWith("/"))
                model.EndpointURI += "/";

            // Validate URI
            Uri uriResult = null;

            bool validUrl = Uri.TryCreate(model.EndpointURI, UriKind.Absolute, out uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            bool hasLocalHost = model.EndpointURI.ToLower().Contains("localhost");

            if (!validUrl)
            {
                resp.IsOK = false;
                resp.IsDirty = false;
                resp.ErrorMsg = "Endpoint Url format is invalid";
                return Json(resp, "json");
            }

            // No allowance for localhost
            if (hasLocalHost)
            {
                resp.IsOK = false;
                resp.IsDirty = false;
                resp.ErrorMsg = "Endpoint Url format is invalid. Replace localhost by IP-address";
                return Json(resp, "json");
            }

            // Check if endpoint url already exists
            var existConnection = businessLayer.GetConnectionByURL(model.EndpointURI);

            if (existConnection != null)
            {
                // Ensure that existing endpoint does not belong to another account
                var checkEndpointStatus = AccountCommon.EndpointExists(currentUser, model.EndpointURI);

                if (checkEndpointStatus == EndpointStatus.OtherAccount)
                {
                    resp.IsOK = false;
                    resp.IsDirty = false;
                    resp.ErrorMsg = "Endpoint Url is allready in use on another account in AOS";
                    return Json(resp, "json");
                }

                if (checkEndpointStatus == EndpointStatus.Orphan)
                {
                    resp.IsOK = false;
                    resp.IsDirty = false;
                    resp.ErrorMsg = "Endpoint Url exists but is orphan. Contact Adwiza Support";
                    return Json(resp, "json");
                }
            }

            // Check if http/https version already exists
            if (model.EndpointURI.ToLower().Contains("https"))
            {
                var testEndpoint = model.EndpointURI.Replace("https", "http");
                var altConnection = businessLayer.GetConnectionByURL(testEndpoint);
                if (altConnection != null)
                {
                    resp.IsOK = false;
                    resp.IsDirty = false;
                    resp.ErrorMsg = "Endpoint Url already exists with header http";
                    return Json(resp, "json");
                }
            }
            else
            {
                var testEndpoint = model.EndpointURI.Replace("http", "https");
                var altConnection = businessLayer.GetConnectionByURL(testEndpoint);
                if (altConnection != null)
                {
                    resp.IsOK = false;
                    resp.IsDirty = false;
                    resp.ErrorMsg = "Endpoint Url already exists with header https";
                    return Json(resp, "json");
                }
            }

            // Endpoint appears to be valid. Now verify endpoint towards SO
            var retVal = new EndpointSOInfo();

            // Check if we should authenticate with SO75
            if (model.EndpointURI.ToLower().Contains("services75"))
            {
                businessLayer.SetSOProperties(model.SysUser, model.SysPassword, model.EndpointURI);

                var authStatus = businessLayer.SOGetAuthenticationStatus();

                if (authStatus == AuthenticationStatus.InvalidURL)
                {
                    resp.IsOK = false;
                    resp.IsDirty = false;
                    resp.ErrorMsg = "Endpoint URL is invalid. No connection could be made";
                    return Json(resp, "json");
                }
                else if (authStatus == AuthenticationStatus.NotAuthenticated)
                {
                    resp.IsOK = false;
                    resp.IsDirty = false;
                    resp.ErrorMsg = "Endpoint URL seems ok, but credentials could not be authenticated";
                    return Json(resp, "json");
                }

                var endpointReturn = businessLayer.SOGetInfo(model.EndpointURI, model.SysUser, model.SysPassword);
                if (endpointReturn.ErrorMsg != string.Empty) // Something went wrong during SO call
                {
                    resp.IsOK = false;
                    resp.IsDirty = false;
                    resp.ErrorMsg = endpointReturn.ErrorMsg;
                    return Json(resp, "json");
                }

                retVal.SOCompanyName = endpointReturn.ReturnValue.Connection.CompanyName;
                retVal.SOVersion = endpointReturn.ReturnValue.Connection.WSVersion.ToString();
                retVal.SONetServerVersion = endpointReturn.ReturnValue.Connection.NetServer;
                retVal.SOSerial = endpointReturn.ReturnValue.Connection.CompanySerial;
                retVal.SOHandshakeStatus = "Ok";

                // Endpoint exists. Now check if any fields were changed and validate handshake info
                if (existConnection != null)
                {
                    // Endpoint can be reached. Now check Preferences and verify Account Id
                    if (existConnection.PrefDescId == 0)
                    {
                        resp.IsOK = false;
                        resp.IsDirty = true;
                        resp.ErrorMsg = "Handshake error: No handshake key present. Click 'Save connection'";
                        return Json(resp, "json");
                    }

                    var prefExists = businessLayer.SOGetAccountIdInPreference(existConnection.PrefDescId);

                    if (prefExists.ReturnValue < 0) // Value found not numeric (invalid value)
                    {
                        resp.IsOK = false;
                        resp.IsDirty = false;
                        resp.ErrorMsg = "Handshake error: Invalid value. Value is not numeric";
                        return Json(resp, "json");
                    }

                    if (prefExists.ReturnValue > 0) // Account Id found. Verify
                    {
                        if (currentAccount.AccountID != prefExists.ReturnValue)
                        {
                            resp.IsOK = false;
                            resp.IsDirty = false;
                            resp.ErrorMsg = "Handshake error: Endpoint belongs to another account";
                            return Json(resp, "json");
                        }
                    }

                    if (prefExists.ReturnValue == 0) // Value not found
                    {
                        resp.IsOK = false;
                        resp.IsDirty = true;
                        resp.ErrorMsg = "Handshake error: Endpoint handshake data not present. Click 'Save connection'";
                        return Json(resp, "json");
                    }

                    var changed = false;

                    if (model.SysUser.ToLower() != existConnection.SysUser.ToLower())
                        changed = true;

                    if (model.SysPassword != existConnection.SysPassword)
                        changed = true;

                    if (model.EndpointURI.ToLower() != existConnection.URL.ToLower())
                        changed = true;

                    if (!changed)
                        resp.ErrorMsg = "Endpoint still valid, but nothing was changed";
                    else
                        resp.IsDirty = true;
                }
                else
                {
                    // Endpoint is valid, but we need to check if it belongs to another account
                    var existPref = businessLayer.SOCheckPreference();

                    if (existPref.ReturnValue < 0) // Value found not numeric (invalid value)
                    {
                        resp.IsOK = false;
                        resp.IsDirty = false;
                        resp.ErrorMsg = "Handshake error: Invalid value. Value is not numeric";
                        return Json(resp, "json");
                    }

                    if (existPref.ReturnValue > 0) // Account Id found. Verify
                    {
                        if (currentAccount.AccountID != existPref.ReturnValue)
                        {
                            resp.IsOK = false;
                            resp.IsDirty = false;
                            resp.ErrorMsg = "Handshake error: Endpoint belongs to another account";
                            return Json(resp, "json");
                        }
                    }

                    resp.ErrorMsg = "Endpoint valid and can be saved";
                    resp.IsDirty = true;
                }
            }
            else // Not SO75 (use agent-based authentication)
            {
                // Attempt to login to SO
                var soUtils = new SOUtils(model.SysUser, model.SysPassword, model.EndpointURI);

                // Check if we are logged in to SO
                if (SoContext.IsAuthenticated)
                {
                    var loggedInCustId = SoContext.CurrentPrincipal.DatabaseContextIdentifier.ToLower();

                    // If we are logging in with a nother context identifer, we must log out first
                    if (loggedInCustId != currentAccount.URL.ToLower())
                        SoContext.CloseCurrentSession();
                }

                // Log in if we do not have a session
                if (!SoContext.IsAuthenticated)
                {
                    var doLoginResp = soUtils.LogInToSO();
                    if (!doLoginResp.IsOK)
                    {
                        resp.IsOK = false;
                        resp.IsDirty = false;
                        resp.ErrorMsg = $"Could not login to SuperOffice - check your endpoint: {doLoginResp.ErrorMsg}";

                        return Json(resp, "json");
                    }
                }

                // Get SO info
                var soInfo = soUtils.GetSOInfo();

                retVal.SOCompanyName = soInfo.CompanyName;
                retVal.SOVersion = soInfo.AssemblyVersion.Replace(".", "").Substring(0, 2);
                retVal.SONetServerVersion = soInfo.Description;
                retVal.SOSerial = soInfo.License.SerialNr;
                retVal.SOHandshakeStatus = "Ok";

                // Endpoint exists. Now check if any fields were changed and validate handshake info
                if (existConnection != null)
                {
                    // Endpoint can be reached. Now check Preferences and verify Account Id
                    if (existConnection.PrefDescId == 0)
                    {
                        resp.IsOK = false;
                        resp.IsDirty = true;
                        resp.ErrorMsg = "Handshake error: No handshake key present. Click 'Save connection'";
                        return Json(resp, "json");
                    }

                    var prefExists = soUtils.GetPrefDescId(existConnection.PrefDescId);

                    if (prefExists < 0) // Value found not numeric (invalid value)
                    {
                        resp.IsOK = false;
                        resp.IsDirty = false;
                        resp.ErrorMsg = "Handshake error: Invalid value. Value is not numeric";
                        return Json(resp, "json");
                    }

                    if (prefExists > 0) // Account Id found. Verify
                    {
                        if (currentAccount.AccountID != prefExists)
                        {
                            resp.IsOK = false;
                            resp.IsDirty = false;
                            resp.ErrorMsg = "Handshake error: Endpoint belongs to another account";
                            return Json(resp, "json");
                        }
                    }

                    if (prefExists == 0) // Value not found
                    {
                        resp.IsOK = false;
                        resp.IsDirty = true;
                        resp.ErrorMsg = "Handshake error: Endpoint handshake data not present. Click 'Save connection'";
                        return Json(resp, "json");
                    }

                    var changed = false;

                    if (model.SysUser.ToLower() != existConnection.SysUser.ToLower())
                        changed = true;

                    if (model.SysPassword != existConnection.SysPassword)
                        changed = true;

                    if (model.EndpointURI.ToLower() != existConnection.URL.ToLower())
                        changed = true;

                    if (model.SOCompanyName != existConnection.CompanyName)
                        changed = true;

                    if (model.SOVersion != existConnection.WSVersion.ToString())
                        changed = true;

                    if (model.SOSerial != existConnection.CompanySerial)
                        changed = true;

                    if (!changed)
                        resp.ErrorMsg = "Endpoint still valid, but nothing was changed";
                    else
                        resp.IsDirty = true;
                }
                else
                {
                    // Endpoint is valid, but we need to check if it belongs to another account
                    var existPref = soUtils.GetPreferenceBySectionAndKey("Adwiza.OnlineServices", "AccountId");

                    if (existPref < 0) // Value found not numeric (invalid value)
                    {
                        resp.IsOK = false;
                        resp.IsDirty = false;
                        resp.ErrorMsg = "Handshake error: Invalid value. Value is not numeric";
                        return Json(resp, "json");
                    }

                    if (existPref > 0) // Account Id found. Verify
                    {
                        if (currentAccount.AccountID != existPref)
                        {
                            resp.IsOK = false;
                            resp.IsDirty = false;
                            resp.ErrorMsg = "Handshake error: Endpoint belongs to another account";
                            return Json(resp, "json");
                        }
                    }

                    resp.ErrorMsg = "Endpoint valid and can be saved";
                    resp.IsDirty = true;
                }
            }

            var json = new JavaScriptSerializer().Serialize(retVal);
            resp.ReturnedJsonData = json;

            return Json(resp, "json");
        }

        // POST: Update endpoint
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateEndpoint(EndpointViewModel model)
        {
            // Create return Json object
            var resp = new AOSResponse();
            resp.ErrorMsg = "Connection info succesfully saved";

            // Get current properties
            var currentUser = UserManager.FindById(User.Identity.GetUserId());
            var currentAccount = PlatformCommon.GetCurrentAccountForUser(currentUser.Id);
            var currentOwner = UserManager.FindById(currentAccount.Owner);

            // Get businesslayer
            IBusinessLayer businessLayer = new BusinessLayer.BusinessLayer(currentUser.Id);

            // Administrator or system user role required to execute this page
            if (!PlatformCommon.HasRole(currentUser, UserRole.Administrator))
                return RedirectToAction("AccessDenied", "Error");

            // Append slash '/' to endpoint url if missing
            if (!model.EndpointURI.EndsWith("/"))
                model.EndpointURI += "/";

            // Validate URI
            Uri uriResult = null;

            bool validUrl = Uri.TryCreate(model.EndpointURI, UriKind.Absolute, out uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            bool hasLocalHost = model.EndpointURI.ToLower().Contains("localhost");

            if (!validUrl)
            {
                resp.IsOK = false;
                resp.ErrorMsg = "Endpoint Url format is invalid";
                return Json(resp, "json");
            }

            // No allowance for localhost
            if (hasLocalHost)
            {
                resp.IsOK = false;
                resp.ErrorMsg = "Endpoint Url format is invalid. Replace localhost by IP-address";
                return Json(resp, "json");
            }

            // Check if endpoint url already exists
            var existConnection = businessLayer.GetConnectionByURL(model.EndpointURI);

            if (existConnection != null)
            {
                // Ensure that existing endpoint does not belong to another account
                var checkEndpointStatus = AccountCommon.EndpointExists(currentUser, model.EndpointURI);

                if (checkEndpointStatus == EndpointStatus.OtherAccount)
                {
                    resp.IsOK = false;
                    resp.IsDirty = false;
                    resp.ErrorMsg = "Endpoint Url is allready in use on another account in AOS";
                    return Json(resp, "json");
                }

                if (checkEndpointStatus == EndpointStatus.Orphan)
                {
                    resp.IsOK = false;
                    resp.IsDirty = false;
                    resp.ErrorMsg = "Endpoint Url exists but is not assigned to any account. Contact support@adwiza.com";
                    return Json(resp, "json");
                }
            }

            bool connectionModified = false;

            // Check if anything was modied
            if (existConnection != null)
            {
                // Check if endpint URI was modified
                if (model.EndpointURI.ToLower() != existConnection.URL.ToLower())
                    connectionModified = true;

                // Check if sysuser was modified
                if (model.SysUser.ToLower() != existConnection.SysUser.ToLower())
                    connectionModified = true;

                // Check if syspassword was modified
                if (model.SysPassword.ToLower() != existConnection.SysPassword.ToLower())
                    connectionModified = true;

                // Check if Account id has been saved to Preferences
                if (existConnection != null && existConnection.PrefDescId == 0)
                    connectionModified = true;
            }

            // Check if we should create or modify connection
            if (existConnection == null) // New connection
            {
                var retVal = new EndpointSOInfo();

                // Check if endpoint is SO75
                if (model.EndpointURI.ToLower().Contains("services75"))
                {
                    // Endpoint appears to be valid. Now verify endpoint towards SO
                    businessLayer.SetSOProperties(model.SysUser, model.SysPassword, model.EndpointURI);

                    var authStatus = businessLayer.SOGetAuthenticationStatus();

                    if (authStatus == AuthenticationStatus.InvalidURL)
                    {
                        resp.IsOK = false;
                        resp.IsDirty = false;
                        resp.ErrorMsg = "Endpoint URL is invalid. No connection could be made";
                        return Json(resp, "json");
                    }
                    else if (authStatus == AuthenticationStatus.NotAuthenticated)
                    {
                        resp.IsOK = false;
                        resp.IsDirty = false;
                        resp.ErrorMsg = "Endpoint URL seems ok, but credentials could not be authenticated";
                        return Json(resp, "json");
                    }

                    var endpointInfo = businessLayer.SOGetInfo(model.EndpointURI, model.SysUser, model.SysPassword);

                    // Check Preferences and insert Account Id for handshaking later
                    var prefExistsSO75 = businessLayer.SOGetAccountIdInPreference(model.PrefDescId);

                    if (prefExistsSO75.ReturnValue < 0) // Value found not numeric (invalid value)
                    {
                        resp.IsOK = false;
                        resp.ErrorMsg = "Invalid value found";
                        return Json(resp, "json");
                    }

                    if (prefExistsSO75.ReturnValue > 0) // Account Id found. Verify
                    {
                        if (currentAccount.AccountID != prefExistsSO75.ReturnValue)
                        {
                            resp.IsOK = false;
                            resp.ErrorMsg = "Endpoint belongs to another account";
                            return Json(resp, "json");
                        }
                    }

                    // Create Preference entries
                    var aaa75 = businessLayer.SOSavePrefDesc("Adwiza.OnlineServices", ".", "Adwiza Online Services");
                    var bbb75 = businessLayer.SOSavePrefDesc("Adwiza.OnlineServices", "AccountId", "Account Id");
                    var prefDescId75 = bbb75.ReturnValue;

                    var ddd75 = businessLayer.SOSaveAccountIdInPreference(prefDescId75, currentAccount.AccountID);

                    // Create new Connection object and insert it into database
                    var newConnection75 = new Connection
                    {
                        URL = model.EndpointURI,
                        SysUser = model.SysUser,
                        SysPassword = model.SysPassword,
                        WSVersion = endpointInfo.ReturnValue.Connection.WSVersion,
                        WSBuild = endpointInfo.ReturnValue.Connection.WSBuild,
                        WSHttps = (model.EndpointURI.ToLower().Contains("https")) ? true : false,
                        NetServer = endpointInfo.ReturnValue.Connection.NetServer,
                        CompanyID = endpointInfo.ReturnValue.Connection.CompanyID,
                        CompanyName = endpointInfo.ReturnValue.Connection.CompanyName,
                        CompanySerial = endpointInfo.ReturnValue.Connection.CompanySerial,
                        PrefDescId = ddd75.ReturnValue
                    };

                    businessLayer.AddConnection(newConnection75);

                    // Get new connection id
                    var newConnectionId75 = businessLayer.GetConnectionByURL(model.EndpointURI).ConnectionID;

                    // Update connection id on current account
                    var currAccount75 = PlatformCommon.GetCurrentAccountForUser(currentUser.Id);

                    currAccount75.ConnectionID = newConnectionId75;

                    businessLayer.UpdateAccount(currAccount75);

                    // Return SO info
                    retVal.SOCompanyName = endpointInfo.ReturnValue.Connection.CompanyName;
                    retVal.SOVersion = endpointInfo.ReturnValue.Connection.WSVersion.ToString();
                    retVal.SONetServerVersion = endpointInfo.ReturnValue.Connection.NetServer;
                    retVal.SOSerial = endpointInfo.ReturnValue.Connection.CompanySerial;
                    retVal.SOHandshakeStatus = "Ok";
                }
                else // Not SO75 - use agents
                {
                    // Attempt to login to SO
                    var soUtils = new SOUtils(model.SysUser, model.SysPassword, model.EndpointURI);

                    // Check if we are logged in to SO
                    if (SoContext.IsAuthenticated)
                    {
                        var loggedInCustId = SoContext.CurrentPrincipal.DatabaseContextIdentifier.ToLower();

                        // If we are logging in with a nother context identifer, we must log out first
                        if (loggedInCustId != currentAccount.URL.ToLower())
                            SoContext.CloseCurrentSession();
                    }

                    // Log in if we do not have a session
                    if (!SoContext.IsAuthenticated)
                    {
                        var doLoginResp = soUtils.LogInToSO();
                        if (!doLoginResp.IsOK)
                        {
                            resp.IsOK = false;
                            resp.IsDirty = false;
                            resp.ErrorMsg = $"Could not login to SuperOffice - check your endpoint: {doLoginResp.ErrorMsg}";

                            return Json(resp, "json");
                        }
                    }

                    // Get SO info
                    var soInfo = soUtils.GetSOInfo();

                    // Check Preferences and insert Account Id for handshaking later
                    var prefExists = soUtils.GetPrefDescId(model.PrefDescId);

                    if (prefExists < 0) // Value found not numeric (invalid value)
                    {
                        resp.IsOK = false;
                        resp.ErrorMsg = "Invalid value found";
                        return Json(resp, "json");
                    }

                    if (prefExists > 0) // Account Id found. Verify
                    {
                        if (currentAccount.AccountID != prefExists)
                        {
                            resp.IsOK = false;
                            resp.ErrorMsg = "Endpoint belongs to another account";
                            return Json(resp, "json");
                        }
                    }

                    // Create Preference entries
                    var aaa = soUtils.SavePreference("Adwiza.OnlineServices", ".", "Adwiza Online Services");
                    var bbb = soUtils.SavePreference("Adwiza.OnlineServices", "AccountId", "Account Id");
                    var prefDescId = bbb;

                    var ddd = soUtils.SavePreferenceEntity(prefDescId, currentAccount.AccountID);

                    // Create new Connection object and insert it into database
                    var newConnection = new Connection
                    {
                        URL = model.EndpointURI,
                        SysUser = model.SysUser,
                        SysPassword = model.SysPassword,
                        WSVersion = Convert.ToInt32(soInfo.FileVersion.Substring(0, 3).Replace(".", "")),
                        WSBuild = 0,
                        WSHttps = (model.EndpointURI.ToLower().Contains("https")) ? true : false,
                        NetServer = soInfo.Description,
                        CompanyID = soInfo.CompanyId,
                        CompanyName = soInfo.CompanyName,
                        CompanySerial = soInfo.License.SerialNr,
                        PrefDescId = ddd
                    };

                    businessLayer.AddConnection(newConnection);

                    // Get new connection id
                    var newConnectionId = businessLayer.GetConnectionByURL(model.EndpointURI).ConnectionID;

                    // Update connection id on current account
                    var currAccount = PlatformCommon.GetCurrentAccountForUser(currentUser.Id);

                    currAccount.ConnectionID = newConnectionId;

                    businessLayer.UpdateAccount(currAccount);

                    // Return SO info
                    retVal.SOCompanyName = soInfo.CompanyName;
                    retVal.SOVersion = soInfo.FileVersion.Substring(0, 3).Replace(".", "");
                    retVal.SONetServerVersion = soInfo.Description;
                    retVal.SOSerial = soInfo.License.SerialNr;
                    retVal.SOHandshakeStatus = "Ok";
                }

                var cockpitModel = AccountCommon.GetCockpitViewModel(currentUser, currentOwner);

                // Update cache
                if (model != null)
                    System.Web.HttpContext.Current.Session["CachedCockpitViewModel"] = model;

                var json = new JavaScriptSerializer().Serialize(retVal);

                resp.ReturnedJsonData = json;

                return Json(resp, "json");
            }
            else // Existing connection
            {
                // Check if endpoint is SO75. If so, we must communicate with it the old-fashioned way using proxy-classes
                // Attempt to login to SO
                var soUtils = new SOUtils(model.SysUser, model.SysPassword, model.EndpointURI);

                // Check if we are logged in to SO
                if (SoContext.IsAuthenticated)
                {
                    var loggedInCustId = SoContext.CurrentPrincipal.DatabaseContextIdentifier.ToLower();

                    // If we are logging in with a nother context identifer, we must log out first
                    if (loggedInCustId != currentAccount.URL.ToLower())
                        SoContext.CloseCurrentSession();
                }

                // Log in if we do not have a session
                if (!SoContext.IsAuthenticated)
                {
                    var doLoginResp = soUtils.LogInToSO();
                    if (!doLoginResp.IsOK)
                    {
                        resp.IsOK = false;
                        resp.IsDirty = false;
                        resp.ErrorMsg = $"Could not login to SuperOffice - check your endpoint: {doLoginResp.ErrorMsg}";

                        return Json(resp, "json");
                    }
                }

                // Get SO info
                var soInfo = soUtils.GetSOInfo();

                if (model.PrefDescId != 0)
                {
                    // Check Preferences and insert Account Id for handshaking later
                    var prefExists = soUtils.GetPrefDescId(existConnection.PrefDescId);

                    if (prefExists < 0) // Value found not numeric (invalid value)
                    {
                        resp.IsOK = false;
                        resp.ErrorMsg = "Invalid value found";
                        return Json(resp, "json");
                    }

                    if (prefExists > 0) // Account Id found. Verify
                    {
                        if (currentAccount.AccountID != prefExists)
                        {
                            resp.IsOK = false;
                            resp.ErrorMsg = "Endpoint belongs to another account";
                            return Json(resp, "json");
                        }
                    }

                    if (prefExists == 0) // Value found not numeric (invalid value)
                        connectionModified = true; // Set this to true so that we can create the handshake data
                }
                else
                    connectionModified = true; // No key. Create it

                // Check if anything was modified
                if (!connectionModified) // Nothing was changed
                {
                    var retVal = new EndpointSOInfo();
                    retVal.SOCompanyName = soInfo.CompanyName;
                    retVal.SOVersion = soInfo.DatabaseName.Replace("SO", "");
                    retVal.SONetServerVersion = soInfo.Description;
                    retVal.SOSerial = soInfo.License.SerialNr;
                    retVal.SOHandshakeStatus = "Ok";

                    var json = new JavaScriptSerializer().Serialize(retVal);

                    resp.IsDirty = false;
                    resp.ReturnedJsonData = json;
                    resp.ErrorMsg = "Endpoint still valid, but nothing was changed";

                    return Json(resp, "json");
                }

                // Create Preference entries
                var aaa = soUtils.SavePreference("Adwiza.OnlineServices", ".", "Adwiza Online Services");
                var bbb = soUtils.SavePreference("Adwiza.OnlineServices", "AccountId", "Account Id");
                var prefDescId = bbb;

                var ddd = soUtils.SavePreferenceEntity(prefDescId, currentAccount.AccountID);

                // Update properties on existing endpoint
                existConnection.URL = model.EndpointURI;
                existConnection.SysUser = model.SysUser;
                existConnection.SysPassword = model.SysPassword;
                existConnection.WSVersion = Convert.ToInt32(soInfo.AssemblyVersion.Replace(".","").Substring(0,2));
                existConnection.WSBuild = 0;
                existConnection.WSHttps = (model.EndpointURI.ToLower().Contains("https")) ? true : false;
                existConnection.NetServer = soInfo.Description;
                existConnection.CompanyID = soInfo.CompanyId;
                existConnection.CompanyName = soInfo.CompanyName;
                existConnection.CompanySerial = soInfo.License.SerialNr;
                existConnection.PrefDescId = ddd;

                // Update existing endpoint
                businessLayer.UpdateConnection(existConnection);

                // Return SO info
                var retVal2 = new EndpointSOInfo()
                {
                    SOCompanyName = soInfo.CompanyName,
                    SOVersion = soInfo.AssemblyVersion.Replace(".", "").Substring(0, 2),
                    SONetServerVersion = soInfo.Description,
                    SOSerial = soInfo.License.SerialNr,
                    SOHandshakeStatus = "Ok"
                };

                var json2 = new JavaScriptSerializer().Serialize(retVal2);

                resp.ReturnedJsonData = json2;
                resp.IsDirty = false;

                return Json(resp, "json");
            }

            //return Json(resp, "json");
            //return View(model);
        }

        // GET: Endpoint settings
        public ActionResult Endpoint()
        {
            // Get current properties
            var currentUser = UserManager.FindById(User.Identity.GetUserId());

            // Administrator or system user role required to execute this page
            if (!PlatformCommon.HasRole(currentUser, UserRole.Administrator))
                return RedirectToAction("AccessDenied", "Error");

            // Get model
            var model = AccountCommon.GetEndpointViewModel(currentUser);

            // Return view with model
            return View(model);
        }

        // POST: Assign account user
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssignAccountUser(AccountUsersViewModel model)
        {
            // Create return Json object
            var resp = new AOSResponse();

            if (!ModelState.IsValid)
            {
                resp.IsOK = false;
                resp.ErrorMsg = "There are validation errors";
                return Json(resp, "json", JsonRequestBehavior.AllowGet);
            }

            // Get current properties
            var currentUser = UserManager.FindById(User.Identity.GetUserId());
            var currentAccount = PlatformCommon.GetCurrentAccountForUser(currentUser.Id);
            var currentOwner = UserManager.FindById(currentAccount.Owner);

            // Administrator or system user role required to execute this page
            if (PlatformCommon.HasRole(currentUser, UserRole.User))
                return RedirectToAction("AccessDenied", "Error");

            // Server side validation
            if (model.AssignUserEmail == null)
            {
                resp.IsOK = false;
                resp.ErrorMsg = "Email must be filled";

                return Json(resp, "json", JsonRequestBehavior.AllowGet);
            }

            var businessLayer = new BusinessLayer.BusinessLayer(currentUser.Id);

            // Check if the user already exists
            var existUser = UserManager.FindByEmail(model.AssignUserEmail);

            if (existUser != null)
            {
                // If the user exists, ensure that he is not already assigned to the account
                var testUAC = businessLayer.GetUserRoleAccountByUserIdAndAccountId(existUser.Id, currentAccount.AccountID);

                if (testUAC != null)
                {
                    resp.IsOK = false;
                    resp.ErrorMsg = "The user is already attached to this account";

                    return Json(resp, "json", JsonRequestBehavior.AllowGet);
                }

                // Server validation to ensure that user is not a system user
                if (AccountCommon.IsSystemUser(existUser))
                {
                    resp.IsOK = false;
                    resp.ErrorMsg = "The user you try to assign is a system user and system users already have access to all accounts in AOS";

                    return Json(resp, "json", JsonRequestBehavior.AllowGet);
                }

                var callBackUrl = Url.Action("Login", "Account", new { returnUrl = Url.Action("UserHome", "User") }, protocol: Request.Url.Scheme);

                // Assign user to current account with role 
                AccountCommon.AssignAccountUser(currentUser, currentAccount, existUser, true);

                /// *** EVENT: USERASSIGNEDBYOWNER *** ///

                // Create notification info
                var notificationInfo = new NotificationInfo()
                {
                    UserId = existUser.Id,
                    AccountId = currentAccount.AccountID,
                    AppId = 0,
                    IPAddress = PlatformCommon.GetIPAddress()
                };

                // Create mail info
                var mailInfoList = new List<MailInfo>()
                {
                    new MailInfo
                    {
                        TargetEmail = existUser.Email,
                        Parameters = new string[] { existUser.FirstNameLastName, currentOwner.FirstNameLastName, currentAccount.Name, callBackUrl }
                    }
                };

                // Create log event and send mails if applicable
                AOSEventHandler.WriteToLog(EventCode.USERASSIGNEDBYOWNER, notificationInfo, mailInfoList, SendMail.Default);
            }
            else // User does not exist - create user
            {
                // Server side validation
                if (model.AssignUserFirstName == null)
                {
                    resp.IsOK = false;
                    resp.ErrorMsg = "Firstname must be filled";

                    return Json(resp, "json", JsonRequestBehavior.AllowGet);
                }

                if (model.AssignUserLastName == null)
                {
                    resp.IsOK = false;
                    resp.ErrorMsg = "Lastname must be filled";

                    return Json(resp, "json", JsonRequestBehavior.AllowGet);
                }

                if (model.AssignUserEmail == null)
                {
                    resp.IsOK = false;
                    resp.ErrorMsg = "Email must be filled";

                    return Json(resp, "json", JsonRequestBehavior.AllowGet);
                }

                var user = new ApplicationUser { UserName = model.AssignUserEmail, Email = model.AssignUserEmail, FirstNameLastName = model.AssignUserFirstName + " " + model.AssignUserLastName };
                var result = UserManager.Create(user);

                if (result.Succeeded)
                {
                    // Get newly created user
                    var newUser = UserManager.FindByEmail(model.AssignUserEmail);

                    // Create random password
                    string code = PlatformCommon.CreatePassword(8);

                    // Update user with random password
                    var result2 = UserManager.AddPassword(newUser.Id, code);

                    // Confirm email right away
                    newUser.EmailConfirmed = true;

                    // Set enabled to true
                    newUser.Enabled = true;

                    UserManager.Update(newUser);

                    // Assign user to current account with role 
                    AccountCommon.AssignAccountUser(currentUser, currentAccount, newUser, true);

                    // Prepare mails
                    var callbackUrl = Url.Action("Login", "Account", new { returnUrl = Url.Action("UserHome", "User") }, protocol: Request.Url.Scheme);

                    /// *** EVENT: USERCREATEDBYOWNER *** ///

                    // Create notification info
                    var notificationInfo = new NotificationInfo()
                    {
                        UserId = newUser.Id,
                        AccountId = currentAccount.AccountID,
                        AppId = 0,
                        IPAddress = PlatformCommon.GetIPAddress()
                    };

                    // Create mail info
                    var mailInfoList = new List<MailInfo>()
                    {
                        new MailInfo
                        {
                            TargetEmail = newUser.Email,
                            Parameters = new string[] { newUser.FirstNameLastName, currentOwner.FirstNameLastName, currentAccount.Name, newUser.Email, code, callbackUrl  }
                        }
                    };

                    // Create log event and send mails if applicable
                    AOSEventHandler.WriteToLog(EventCode.USERCREATEDBYOWNER, notificationInfo, mailInfoList, SendMail.Default);
                }
                else
                {
                    resp.IsOK = false;
                    resp.ErrorMsg = "An error occurred while creating user: " + result.Errors.ToString();

                    return Json(resp, "json", JsonRequestBehavior.AllowGet);
                }
            }

            resp.ErrorMsg = "User succesfully assigned. User will be notified by email";

            return Json(resp, "json", JsonRequestBehavior.AllowGet);
        }

        // GET: Account users
        public ActionResult AccountUsers()
        {
            // Get current properties
            var currentUser = UserManager.FindById(User.Identity.GetUserId());
            var currentAccount = PlatformCommon.GetCurrentAccountForUser(currentUser.Id);
            var currentOwner = UserManager.FindById(currentAccount.Owner);

            // Administrator or system user role required to execute this page
            if (!PlatformCommon.HasRole(currentUser, UserRole.Administrator))
                return RedirectToAction("AccessDenied", "Error");

            // Get model
            var model = AccountCommon.GetAccountUsersViewModel(currentUser, currentOwner);

            // Return view with model
            return View(model);
        }

        [HttpPost]
        public ActionResult RemoveAccountUser(string UserId, int AccountId)
        {
            // Get current properties
            var currentUser = UserManager.FindById(User.Identity.GetUserId());
            var currentAccount = PlatformCommon.GetCurrentAccountForUser(currentUser.Id);
            var currentOwner = UserManager.FindById(currentAccount.Owner);

            // Administrator or system user role required to execute this page
            if (!PlatformCommon.HasRole(currentUser, UserRole.Administrator))
                return RedirectToAction("AccessDenied", "Error");

            // Remove account user
            var result = AccountCommon.RemoveAccountUser(currentUser, UserId, AccountId);

            // Get model
            var model = AccountCommon.GetAccountUsersViewModel(currentUser, currentOwner);

            // Return view with model
            return View("AccountUsers", model);
        }

        public ActionResult UpdateAccountUser(string concatId, bool toBeAdmin)
        {
            // Create return Json object
            var resp = new AOSResponse();

            // Get current user
            var currentUser = UserManager.FindById(User.Identity.GetUserId());

            // Administrator or system user role required to execute this page
            if (!PlatformCommon.HasRole(currentUser, UserRole.Administrator))
                return RedirectToAction("AccessDenied", "Error");

            // Update account user role
            var result = AccountCommon.UpdateAccountUser(currentUser, concatId, toBeAdmin);

            if (!result.ToLower().Contains("role succesfully updated"))
            {
                resp.IsOK = false;
                resp.ErrorMsg = "An error occurred while attempting to change role: " + result;
                return Json(resp, "json", JsonRequestBehavior.AllowGet);
            }

            resp.ErrorMsg = result;
            return Json(resp, "json", JsonRequestBehavior.AllowGet);
        }

        // GET: Personal settings
        public ActionResult PersonalSettings()
        {
            // Get current properties
            var currentUser = UserManager.FindById(User.Identity.GetUserId());

            // Get model
            var model = AccountCommon.GetPersonalSettingsViewModel(currentUser);

            // Return view with model
            return View(model);
        }

        // POST: Update Personal Settings
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdatePersonalSettings(PersonalSettingsViewModel model)
        {
            // Create return Json object
            var resp = new AOSResponse();

            // Get current user
            var currentUser = UserManager.FindById(User.Identity.GetUserId());

            // Check if user has changed his username, which is the email
            bool emailChanged = currentUser.Email != model.Email;

            // Ensure updated email does not exist already in database
            if (emailChanged)
            {
                var existUser = UserManager.FindByEmail(model.Email);

                if (existUser != null)
                {
                    resp.IsOK = false;
                    resp.ErrorMsg = "Email does already exist for another user";
                    return Json(resp, "json");
                }
            }

            currentUser.Email = model.Email;
            currentUser.UserName = model.Email;
            currentUser.FirstNameLastName = model.FirstNameLastName;
            currentUser.PhoneNumber = model.PhoneNumber;

            // Update the user
            var result = UserManager.Update(currentUser);

            // If user has changed his email, we will have to login again
            if (emailChanged)
            {
                // Sign out user
                AuthenticationManager.SignOut();

                SignInManager.SignIn(currentUser, true, false);

                resp.RedirectAction = currentUser.UserName; // Hack. This field is just a datacarrier for the javascript
            }

            return Json(resp, "json");
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssignExistingAccount(AccountModel model)
        {
            // Create return Json object
            var resp = new AOSResponse();

            // Create businesslayer object
            IBusinessLayer businessLayer = new BusinessLayer.BusinessLayer(model.UserId);

            // Account must exist.
            var existingAccount = businessLayer.GetAccountByName(model.ExistingAccount.ExistingAccountName);
            if (existingAccount == null)
            {
                resp.IsOK = false;
                resp.ErrorMsg = "Account does not exist. Please choose an existing account";
                return Json(resp, "json");
            }

            // Get account owner
            var accountOwner = UserManager.FindById(existingAccount.Owner);
            if (accountOwner == null)
            {
                resp.IsOK = false;
                resp.ErrorMsg = "Account owner cannot be found in database";
                return Json(resp, "json");
            }

            // Get current user - TEST: Hardcoded userid if model.UserId is null
            if (model.UserId == null)
                model.UserId = "2448ca62-c0df-4ec0-b566-cadf2ab3e6be";

            var currentUser = UserManager.FindById(model.UserId);
            if (currentUser == null)
            {
                resp.IsOK = false;
                resp.ErrorMsg = "User cannot be found in database";
                return Json(resp, "json");
            }

            // Get the role "User"
            var role = businessLayer.GetRoleByName("User");
            if (role == null)
            {
                resp.IsOK = false;
                resp.ErrorMsg = "Role 'User' could not be found in database";
                return Json(resp, "json");
            }

            // Hash accountid
            var hashedAccountId = BusinessLayer.Utils.ProtectServicePassword(existingAccount.AccountID.ToString());

            var callbackUrl = Url.Action("GrantAccess", "Account", new { userId = currentUser.Id, accountId = hashedAccountId }, protocol: Request.Url.Scheme);

            /// *** EVENT: USERAPPLIEDFORMEMBERSHIP *** ///

            // Create notification info
            var notificationInfo = new NotificationInfo()
            {
                UserId = currentUser.Id,
                AccountId = existingAccount.AccountID,
                AppId = 0,
                IPAddress = PlatformCommon.GetIPAddress()
            };

            // Create mail info
            var mailInfoList = new List<MailInfo>()
            {
                new MailInfo {
                    TargetEmail = currentUser.Email,
                    Parameters = new string[] { currentUser.FirstNameLastName, existingAccount.Name, accountOwner.FirstNameLastName }
                },
                new MailInfo {
                    TargetEmail = accountOwner.Email,
                    Parameters = new string[] { accountOwner.FirstNameLastName, currentUser.FirstNameLastName, currentUser.Email, existingAccount.Name, callbackUrl }
                }
            };

            // Create log event and send mails if applicable
            AOSEventHandler.WriteToLog(EventCode.USERAPPLIEDFORMEMBERSHIP, notificationInfo, mailInfoList, SendMail.Default);

            // Create confirm message for user
            resp.ErrorMsg = string.Format("The account owner <strong>{0}</strong> has now been contacted. You will be notified by email when you have been granted access", accountOwner.FirstNameLastName);

            return Json(resp, "json");
        }

        [AllowAnonymous]
        public ActionResult GrantAccess(string userId, string accountId)
        {
            // Create businesslayer object
            IBusinessLayer businessLayer = new BusinessLayer.BusinessLayer(userId);

            // Create model
            var model = new GrantAccessViewModel();

            // Unhash parameters
            var unhashedAccountId = BusinessLayer.Utils.UnprotectServicePassword(accountId);

            // Get user to grant access to
            var grantUser = UserManager.FindById(userId);

            if (grantUser == null)
            {
                model.ErrorMessage = "User to grant access to could not be found";
                return View(model);
            }

            // Get account to grant access for
            int numAccountId;
            var convertSucceeded = int.TryParse(unhashedAccountId, out numAccountId);

            if (!convertSucceeded)
            {
                model.ErrorMessage = "Account id could not be parsed";
                return View(model);
            }

            var grantAccount = businessLayer.GetAccountById(numAccountId);

            if (grantAccount == null)
            {
                model.ErrorMessage = "Account to grant access for could not be found";
                return View(model);
            }

            model.GrantedAccount = grantAccount.Name;
            model.GrantedUser = (grantUser.FirstNameLastName == null) ? grantUser.UserName : grantUser.FirstNameLastName;

            // Grant access to account
            var userRole = businessLayer.GetRoleByName("User");

            // Check if user already is a member of the account, but with a different role
            var uacList = businessLayer.GetUserRoleAccountByUserId(grantUser.Id);

            var alreadyMember = false;
            foreach (var item in uacList)
            {
                if (item.AccountID == grantAccount.AccountID)
                {
                    if (item.RoleID == userRole.Id)
                    {
                        alreadyMember = true;
                        model.ErrorMessage = "User is already a member of the account with the role as a normal user";
                        break;
                    }
                    else
                    {
                        alreadyMember = true;
                        model.ErrorMessage = "User is already a member of the account, but with a different role";
                        break;
                    }
                }
            }

            if (alreadyMember)
                return View(model);

            var newUAC = new UserRoleAccount()
            {
                AccountID = grantAccount.AccountID,
                UserID = grantUser.Id,
                RoleID = userRole.Id,
                ActiveAccount = (uacList.Count > 0) ? false : true // Only set to active account if user doesn't have any other accounts
            };

            businessLayer.AddUserRoleAccount(newUAC);

            var owner = UserManager.FindById(grantAccount.Owner);
            var callbackUrl = Url.Action("Login", "Account", new { returnUrl = Url.Action("UserHome", "User") }, protocol: Request.Url.Scheme);

            /// *** EVENT: OWNERGRANTEDACCESS *** ///

            // Create notification info
            var notificationInfo = new NotificationInfo()
            {
                UserId = grantUser.Id,
                AccountId = grantAccount.AccountID,
                AppId = 0,
                IPAddress = PlatformCommon.GetIPAddress()
            };

            // Create mail info
            var mailInfoList = new List<MailInfo>()
            {
                new MailInfo {
                    TargetEmail = grantUser.Email,
                    Parameters = new string[] { grantUser.FirstNameLastName, owner.FirstNameLastName, grantAccount.Name, callbackUrl }
                }
            };

            // Create log event and send mails if applicable
            AOSEventHandler.WriteToLog(EventCode.OWNERGRANTEDACCESS, notificationInfo, mailInfoList, SendMail.Default);

            return View(model);
        }

        public ActionResult ChangeToOwnerAccount()
        {
            var resp = new AOSResponse();

            // Get current user
            var currentUser = UserManager.FindById(User.Identity.GetUserId());
            if (currentUser == null)
            {
                resp.IsOK = false;
                resp.ErrorMsg = "User could not be identified";
                return View("Error", resp);
            }

            var businessLayer = new BusinessLayer.BusinessLayer(currentUser.Id);

            // Get users owner account
            var ownerAccounts = businessLayer.GetAccountsByOwner(currentUser.Id);
            if (ownerAccounts.Count == 0)
            {
                resp.IsOK = false;
                resp.ErrorMsg = "User has no owner accounts";
                return View("Error", resp);
            }

            if (ownerAccounts.Count > 1)
            {
                resp.IsOK = false;
                resp.ErrorMsg = "User has more than one owner account";
                return View("Error", resp);
            }

            // Get list of user's accounts
            var uacList = businessLayer.GetUserRoleAccountByUserId(currentUser.Id);

            var deactivateAccount = new UserRoleAccount();
            var activateAccount = new UserRoleAccount();

            // Locate account to be deactivated and activated
            foreach (var uac in uacList)
            {
                if (uac.ActiveAccount)
                    deactivateAccount = uac;

                if (uac.AccountID == ownerAccounts[0].AccountID)
                    activateAccount = uac;
            }

            deactivateAccount.ActiveAccount = false;
            activateAccount.ActiveAccount = true;

            businessLayer.UpdateUserRoleAccount(deactivateAccount, activateAccount);

            // Return to User Homepage
            return RedirectToAction("UserHome", "User");
        }

        public ActionResult ChangeAccount(int accountId)
        {
            var resp = new AOSResponse();

            // Get current properties
            var currentUser = UserManager.FindById(User.Identity.GetUserId());

            if (currentUser == null)
            {
                resp.IsOK = false;
                resp.ErrorMsg = "User could not be identified";
                return View("Error", resp);
            }

            // Change default account
            var businessLayer = new BusinessLayer.BusinessLayer(currentUser.Id);

            // Get list of user's accounts
            var uacList = businessLayer.GetUserRoleAccountByUserId(currentUser.Id);

            var deactivateAccount = new UserRoleAccount();
            var activateAccount = new UserRoleAccount();

            // Locate account to be deactivated
            foreach (var uac in uacList)
            {
                if (uac.ActiveAccount)
                    deactivateAccount = uac;

                if (uac.AccountID == accountId)
                    activateAccount = uac;
            }

            deactivateAccount.ActiveAccount = false;
            activateAccount.ActiveAccount = true;

            businessLayer.UpdateUserRoleAccount(deactivateAccount, activateAccount);

            // Return to User Homepage
            return RedirectToAction("UserHome", "User");
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateNewAccount(AccountModel model)
        {
            // Create return Json object
            var resp = new AOSResponse();
            resp.IsOK = true;

            // Create businesslayer object
            IBusinessLayer businessLayer = new BusinessLayer.BusinessLayer(model.UserId);

            // Check if account already exists
            if (businessLayer.GetAccountByName(model.NewAccount.NewAccountName) != null)
            {
                resp.IsOK = false;
                resp.ErrorMsg = "Account already exists. Please choose another name";
                return Json(resp, "json");
            }

            // Get current user
            var currentUser = UserManager.FindById(model.UserId);
            if (currentUser == null)
            {
                resp.IsOK = false;
                resp.ErrorMsg = "User cannot be found in database";
                return Json(resp, "json");
            }

            // Validate if user already has created an account
            var accountsForUser = businessLayer.GetAccountsByOwner(currentUser.Id);
            if (accountsForUser.Count > 0)
            {
                resp.IsOK = false;
                resp.ErrorMsg = "User already owns an account";
                return Json(resp, "json");
            }

            // Validate country
            var country = businessLayer.GetCountryByName(model.NewAccount.NewAccountCountry);
            if (country == null)
            {
                resp.IsOK = false;
                resp.ErrorMsg = "Invalid country";
                return Json(resp, "json");
            }

            // Create new account
            try
            {
                Account newAccount = new Account()
                {
                    Name = model.NewAccount.NewAccountName,
                    URL = model.NewAccount.NewAccountPublishURL,
                    Address = model.NewAccount.NewAccountAddress,
                    Address2 = model.NewAccount.NewAccountAddress2,
                    Zip = (model.NewAccount.NewAccountZip.Length > 10) ? model.NewAccount.NewAccountZip.Substring(0, 10) : model.NewAccount.NewAccountZip,
                    City = model.NewAccount.NewAccountCity,
                    CountryID = country.CountryID,
                    Enabled = true,
                    Owner = currentUser.Id,
                    IsPartner = true
                };

                businessLayer.AddAccount(newAccount);

            }
            catch (Exception ex)
            {
            }

            // Assign user to account as role "Administrator"
            var role = businessLayer.GetRoleByName("Administrator");

            if (role == null)
            {
                resp.IsOK = false;
                resp.ErrorMsg = "Role 'Administrator' could not be found in database";
                return Json(resp, "json");
            }

            var account = businessLayer.GetAccountByNameThin(model.NewAccount.NewAccountName);
            if (account == null)
            {
                resp.IsOK = false;
                resp.ErrorMsg = "Newly inserted account " + model.NewAccount.NewAccountName + ", cannot be found in database";
                return Json(resp, "json");
            };

            UserRoleAccount userRoleAccount = new UserRoleAccount()
            {
                UserID = currentUser.Id,
                RoleID = role.Id,
                AccountID = account.AccountID,
                ActiveAccount = true
            };

            businessLayer.AddUserRoleAccount(userRoleAccount);

            /// *** EVENT: ACCOUNTCREATEDBYUSER *** ///

            // Create notification info
            var notificationInfo = new NotificationInfo()
            {
                UserId = currentUser.Id,
                AccountId = account.AccountID,
                AppId = 0,
                IPAddress = PlatformCommon.GetIPAddress()
            };

            // Create mail info
            var mailInfoList = new List<MailInfo>(); // No emails

            // Create log event and send mails if applicable
            AOSEventHandler.WriteToLog(EventCode.ACCOUNTCREATEDBYUSER, notificationInfo, mailInfoList, SendMail.Default);

            // Log in user and take him to User Home
            SignInManager.SignIn(currentUser, true, false);

            // Check if user is in the process of buying an app
            if (model.AppID > 0)
            {
                // Get app
                var app = businessLayer.GetAppById(model.AppID);

                // Create notification info
                var notificationInfo2 = new NotificationInfo()
                {
                    UserId = currentUser.Id,
                    AccountId = account.AccountID,
                    AppId = app.AppID,
                    IPAddress = PlatformCommon.GetIPAddress()
                };

                // Create account/app association
                var accountApp = new AccountApp();
                accountApp.AccountID = account.AccountID;
                accountApp.AppID = app.AppID;
                accountApp.Activated = true;
                accountApp.Installed = true;
                accountApp.IsTrial = model.IsTrial;

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
                    var mailInfoList2 = new List<MailInfo>()
                    {
                        new MailInfo
                        {
                            TargetEmail = currentUser.Email,
                            Parameters = new string[] {
                                currentUser.FirstNameLastName,
                                app.Name,
                                account.Name,
                                app.TrialDays.ToString(),
                                Convert.ToDateTime(accountApp.TrialExpire).ToString("dd-MM-yyyy") }
                        }
                        //new MailInfo
                        //{
                        //    TargetEmail = currentUser.Email,
                        //    Parameters = new string[] {
                        //        currentUser.FirstNameLastName,
                        //        app.Name,
                        //        account.Name,
                        //        app.TrialDays.ToString(),
                        //        Convert.ToDateTime(accountApp.TrialExpire).ToString("dd-MM-yyyy"),
                        //        callbackUrl  }
                        //}
                    };

                    // Create log event and send mails if applicable
                    AOSEventHandler.WriteToLog(EventCode.TRYAPPCOMPLETED, notificationInfo, mailInfoList2, SendMail.Default);

                    // Create 2nd log event
                    var mailInfoList3 = new List<MailInfo>();
                    AOSEventHandler.WriteToLog(EventCode.APPTRIALPERIODSTARTED, notificationInfo, mailInfoList3, SendMail.Default);
                }
                else
                {
                    /// *** EVENT: BUYAPPCOMPLETED *** ///

                    // Create mail info
                    var mailInfoList3 = new List<MailInfo>()
                    {
                        new MailInfo
                        {
                            TargetEmail = currentUser.Email,
                            Parameters = new string[] { currentUser.FirstNameLastName, app.Name, account.Name  }
                        }
                    };

                    // Create log event and send mails if applicable
                    AOSEventHandler.WriteToLog(EventCode.BUYAPPCOMPLETED, notificationInfo, mailInfoList3, SendMail.Default);
                }

                businessLayer.AddAccountApp(accountApp);
            }

            resp.RedirectAction = "UserHome";
            resp.RedirectController = "User";
            resp.Redirect = true;

            return Json(resp, "json");
        }

        [AllowAnonymous]
        public ActionResult GetAllAccounts(string search)
        {
            var typeAheadAccountList = AccountCommon.GetAllAccounts(search);

            return Json(typeAheadAccountList, "json", JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult GetAllCountries(string search)
        {
            var typeAheadCountryList = AccountCommon.GetAllCountries(search);

            return Json(typeAheadCountryList, "json", JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
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

        public ActionResult CheckEndpoint(string accountName)
        {
            // Create response object
            var resp = new AOSResponse();

            // Get current properties
            var currentUser = UserManager.FindById(User.Identity.GetUserId());

            if (currentUser == null)
            {
                resp.ErrorMsg = "User could not be identified";
                resp.IsOK = false;
                return Json(resp, "json", JsonRequestBehavior.AllowGet);
            }

            var currentAccount = PlatformCommon.GetCurrentAccountForUser(currentUser.Id);

            if (currentAccount == null)
            {
                resp.ErrorMsg = "User has no active accounts";
                resp.IsOK = false;
                return Json(resp, "json", JsonRequestBehavior.AllowGet);
            }

            var currentOwner = UserManager.FindById(currentAccount.Owner);

            // Get model
            var model = AccountCommon.GetCockpitViewModel(currentUser, currentOwner);

            var smallModel = new CockpitSmallModel()
            {
                CurrentConnectionStatusIcon = model.CurrentConnectionStatusIcon,
                CurrentConnectionStatus = model.CurrentConnectionStatus,
                StatusAsInt = model.StatusAsInt
            };

            var json = new JavaScriptSerializer().Serialize(smallModel);

            resp.ReturnedJsonData = json;

            return Json(resp, "json", JsonRequestBehavior.AllowGet);
        }

        //[ChildActionOnly]
        public ActionResult ShowCockpit()
        {
            var resp = new AOSResponse();

            // Get current properties
            var currentUser = UserManager.FindById(User.Identity.GetUserId());
            if (currentUser == null)
            {
                resp.IsOK = false;
                resp.ErrorMsg = "User could not be identified";
                return View("Error", resp);
            }

            var currentAccount = PlatformCommon.GetCurrentAccountForUser(currentUser.Id);
            if (currentAccount == null)
            {
                resp.IsOK = false;
                resp.ErrorMsg = "User could not be identified";
                return View("Error", resp);
            }

            var currentOwner = UserManager.FindById(currentAccount.Owner);

            // Get model either from cache or new model
            var cachedCockpitViewModel = System.Web.HttpContext.Current.Session["CachedCockpitViewModel"] as CockpitViewModel;
            if (cachedCockpitViewModel != null && currentAccount.AccountID == cachedCockpitViewModel.CurrentAccountId && cachedCockpitViewModel.CurrentUserId == currentUser.Id)
                return PartialView("_CockpitPartial", cachedCockpitViewModel);
            else
            {
                var model = AccountCommon.GetCockpitViewModel(currentUser, currentOwner);

                // Update cache
                if (model != null)
                    System.Web.HttpContext.Current.Session["CachedCockpitViewModel"] = model;

                // Return view with model
                return PartialView("_CockpitPartial", model);
            }

        }

        public ActionResult ShowCockpitAlternative()
        {
            var resp = new AOSResponse();

            // Get current properties
            var currentUser = UserManager.FindById(User.Identity.GetUserId());

            if (currentUser == null)
            {
                resp.IsOK = false;
                resp.ErrorMsg = "User could not be identified";
                return View("Error", resp);
            }

            var currentAccount = PlatformCommon.GetCurrentAccountForUser(currentUser.Id);

            if (currentAccount == null)
            {
                resp.IsOK = false;
                resp.ErrorMsg = "User has no active accounts";
                return View("Error", resp);
            }

            var currentOwner = UserManager.FindById(currentAccount.Owner);

            // Get model
            var model = AccountCommon.GetCockpitViewModel(currentUser, currentOwner);

            // Return view with model
            return PartialView("_CockpitPartialAlternative", model);
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Require the user to have a confirmed email before they can log on.
            // var user = await UserManager.FindByNameAsync(model.Email);
            var existUser = UserManager.FindByEmail(model.Email);

            if (existUser == null)
            {
                ModelState.AddModelError("", "Invalid login attempt. User not found");
                return View(model);
            }

            var user = UserManager.Find(model.Email, model.Password);

            if (user != null)
            {
                if (!await UserManager.IsEmailConfirmedAsync(user.Id))
                {
                    string code = UserManager.GenerateEmailConfirmationToken(user.Id);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    var termsOfServiceUrl = Url.Action("TermsOfService", "User");

                    /// *** EVENT: RESENDEMAILCONFIRMATION *** ///

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
                        new MailInfo {
                            TargetEmail = user.Email,
                            Parameters = new string[] { user.FirstNameLastName, termsOfServiceUrl, callbackUrl }
                        }
                    };

                    // Create log event and send mails if applicable
                    AOSEventHandler.WriteToLog(EventCode.RESENDEMAILCONFIRMATION, notificationInfo, mailInfoList, SendMail.Default);

                    // Uncomment to debug locally  
                    ViewBag.Link = callbackUrl;
                    ViewBag.errorMessage = "You must have a confirmed email to log on. "
                                         + "The confirmation token has been resent to your email account.";

                    return View("Error");
                }

                // Check is user is enabled/disabled. This can be set by system user
                if (!user.Enabled)
                {
                    ModelState.AddModelError("", "Invalid login attempt. Your user profile has been disabled. If you think this is incorrect, please contact your system administrator");
                    return View(model);
                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid login attempt. Wrong password");
                return View(model);
            }

            // Check if user has any accounts assigned
            var currentAccount = AccountCommon.GetCurrentAccountForUser(user.Id);

            if (currentAccount == null)
            {
                ModelState.AddModelError("", "You have no accounts assigned to your user profile");
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl, model.Email);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Login2(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { ok = true, msg = "Invalid model", newurl = Url.Action("Index", "Home") }, JsonRequestBehavior.AllowGet);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);

            switch (result)
            {
                case SignInStatus.Success:
                    return Json(new { ok = true, msg = "", newurl = Url.Action("Index", "Home") }, JsonRequestBehavior.AllowGet);
                case SignInStatus.LockedOut:
                    return Json(new { ok = false, msg = "Locked out", newurl = Url.Action("Index", "Home") }, JsonRequestBehavior.AllowGet);
                case SignInStatus.RequiresVerification:
                    return Json(new { ok = false, msg = "Send code", newurl = Url.Action("Index", "Home") }, JsonRequestBehavior.AllowGet);
                case SignInStatus.Failure:
                default:
                    return Json(new { ok = false, msg = "Error!", newurl = Url.Action("Index", "Home") }, JsonRequestBehavior.AllowGet);
            }

            //switch (result)
            //{
            //    case SignInStatus.Success:
            //        return RedirectToLocal(returnUrl);
            //    case SignInStatus.LockedOut:
            //        return View("Lockout");
            //    case SignInStatus.RequiresVerification:
            //        return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
            //    case SignInStatus.Failure:
            //    default:
            //        ModelState.AddModelError("", "Invalid login attempt.");
            //        return View(model);
            //}
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            var user = await UserManager.FindByIdAsync(await SignInManager.GetVerifiedUserIdAsync());
            if (user != null)
            {
                var code = await UserManager.GenerateTwoFactorTokenAsync(user.Id, provider);
                // Remove for Debug
                ViewBag.Code = code;
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult RegisterAndBuy(int AppID, bool IsTrial)
        {
            var resp = new AOSResponse();
            var businessLayer = new BusinessLayer.BusinessLayer();

            var app = businessLayer.GetAppById(AppID);

            if (app == null)
            {
                resp.IsOK = false;
                resp.ErrorMsg = string.Format("AppId: {0} not found", AppID);
                return View("Error", resp);
            }

            var model = new RegisterViewModel();
            model.AppID = AppID;
            model.AppName = app.Name;
            model.IsTrial = IsTrial;

            return View(model);
        }

        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterAndBuy(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = UserManager.Create(user, model.Password);

                if (result.Succeeded)
                {
                    // Update the user with firstname and lastname
                    var currentUser = UserManager.FindByEmail(model.Email);

                    currentUser.FirstNameLastName = model.FirstName + " " + model.LastName;

                    if (model.Phone != null)
                        currentUser.PhoneNumber = model.Phone;

                    // Set enabled to true
                    currentUser.Enabled = true;

                    UserManager.Update(currentUser);

                    string code = UserManager.GenerateEmailConfirmationToken(user.Id);
                    var callbackUrl = Url.Action("ConfirmEmailAndBuy", "Account", new { userId = user.Id, code = code, appID = model.AppID, isTrial = model.IsTrial }, protocol: Request.Url.Scheme);
                    var termsOfServiceUrl = Url.Action("TermsOfService", "User", null, protocol: Request.Url.Scheme);

                    /// *** EVENT: USERCREATED *** ///

                    // Create notification info
                    var notificationInfo = new NotificationInfo()
                    {
                        UserId = currentUser.Id,
                        AccountId = 0,
                        AppId = 0,
                        IPAddress = PlatformCommon.GetIPAddress()
                    };

                    // Create mail info
                    var mailInfoList = new List<MailInfo>()
                    {
                        new MailInfo {
                            TargetEmail = user.Email,
                            Parameters = new string[] { currentUser.FirstNameLastName, termsOfServiceUrl, callbackUrl }
                        }
                    };

                    // Create log event and send mails if applicable
                    AOSEventHandler.WriteToLog(EventCode.USERCREATED, notificationInfo, mailInfoList, SendMail.Default);

                    ViewBag.Message = "Check your email and confirm your account, you must be confirmed "
                                    + "before you can log in.";

                    // For local debug only
                    ViewBag.Link = callbackUrl;

                    return View("Info");
                    //return RedirectToAction("Index", "Home");
                }

                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult AutoRegister(string token, int? testAppId)
        {
            var resp = new AOSResponse();

            var currentURL = Request.Url;
            var isDevelopment = currentURL.ToString().Contains("192.168");

            ApplicationUser theUser = null;
            Account theAccount = null;
            UserRoleAccount theURA = null;
            string code = string.Empty;

            var newUserCreated = false;
            var newAccountCreated = false;
            var newAccountAppCreated = false;
            var newUARCreated = false;

            var log = new List<string>();

            var businessLayer = new BusinessLayer.BusinessLayer();

            try
            {
                // Unpack SAML token and get CreateUser object
                log.Add("Unpacking SAML token...");

                var samlTokenUnpackResponse = PlatformCommon.UnpackSAMLToken(token, isDevelopment);
                if (!samlTokenUnpackResponse.IsOK)
                {
                    resp.IsOK = false;
                    resp.ErrorMsg = samlTokenUnpackResponse.ReturnMsg;
                    log.Add("SAML token could not be unpacked");
                    resp.ReturnedJsonData = string.Join("|", log.ToArray());
                    return View("AutoRegisterError", resp);
                }

                log.Add("SAML token succesfully unpacked");

                // Get the CreateUser object from the unpack token response
                SOOnlineCreateUser createUser = samlTokenUnpackResponse.CreateUser;

                // Validate CreateUser object
                log.Add("Validating SAML taken...");

                var samlTokenValidateResponse = PlatformCommon.ValidateSAMLToken(createUser);
                if (!samlTokenValidateResponse.IsOK)
                {
                    resp.IsOK = false;
                    resp.ErrorMsg = samlTokenValidateResponse.ErrorMsg;
                    log.Add("SAML token did not validate");
                    resp.ReturnedJsonData = string.Join("|", log.ToArray());
                    return View("AutoRegisterError", resp);
                }

                // HTML encode token data to prevent XSS attacks
                createUser.CustAddress = HttpUtility.HtmlEncode(createUser.CustAddress);
                createUser.CustAddress2 = HttpUtility.HtmlEncode(createUser.CustAddress2);
                createUser.CustZip = HttpUtility.HtmlEncode(createUser.CustZip);
                createUser.CustCity = HttpUtility.HtmlEncode(createUser.CustCity);
                createUser.EMail = HttpUtility.HtmlEncode(createUser.EMail);
                createUser.FirstName = HttpUtility.HtmlEncode(createUser.FirstName);
                createUser.LastName = HttpUtility.HtmlEncode(createUser.LastName);
                createUser.OwnerCompanyName = HttpUtility.HtmlEncode(createUser.OwnerCompanyName);
                createUser.CustId = HttpUtility.HtmlEncode(createUser.CustId);

                log.Add("SAML token succesfully validated");
                log.Add("Extracted SAML token data:");
                log.Add("==> App id: <strong>" + createUser.AOSAppId.ToString() + "</strong>");
                log.Add("==> Connection id: <strong>" + createUser.ConnectionId.ToString() + "</strong>");
                log.Add("==> Address: <strong>" + createUser.CustAddress + "</strong>");
                log.Add("==> Address 2: <strong>" + createUser.CustAddress2 + "</strong>");
                log.Add("==> City: <strong>" + createUser.CustCity + "</strong>");
                log.Add("==> Country id: <strong>" + createUser.CustCountryId.ToString() + "</strong>");
                log.Add("==> Cust id: <strong>" + createUser.CustId + "</strong>");
                log.Add("==> Zip: <strong>" + createUser.CustZip + "</strong>");
                log.Add("==> Email: <strong>" + createUser.EMail + "</strong>");
                log.Add("==> First name: <strong>" + createUser.FirstName + "</strong>");
                log.Add("==> Last name: <strong>" + createUser.LastName + "</strong>");
                log.Add("==> Company name: <strong>" + createUser.OwnerCompanyName + "</strong>");

                // ****************************************
                // *** Token fields validation complete ***
                // ****************************************

                // Check if testAppId is filled
                if (testAppId != null)
                {
                    log.Add("==> Test app id is filled: " + testAppId);
                    log.Add("==> Validate test app id..." + testAppId);

                    // Validate app id
                    var testApp = businessLayer.GetAppById((int)testAppId);
                    if (testApp == null)
                    {
                        log.Add("==> Test app id was not found");
                        resp.IsOK = false;
                        resp.ErrorMsg = "Test App record could not be found (TestAppId: " + testAppId + ")";
                        resp.ReturnedJsonData = string.Join("|", log.ToArray());
                        return View("AutoRegisterError", resp);
                    }
                }

                log.Add("Check if App record exists...");

                var currApp = businessLayer.GetAppById(createUser.AOSAppId);
                if (currApp == null)
                {
                    resp.IsOK = false;
                    resp.ErrorMsg = "App record could not be found (AppId: " + createUser.AOSAppId.ToString() + ")";
                    resp.ReturnedJsonData = string.Join("|", log.ToArray());
                    return View("AutoRegisterError", resp);
                }

                log.Add("App record found"); ;

                log.Add("Check if account exists...");

                // Check if the account already exists
                var existAccount = businessLayer.GetAccountByURLThin(createUser.CustId);
                if (existAccount == null)
                {
                    log.Add("Account was not found (CustId: " + createUser.CustId + "). Account will be created");

                    // Check if user already exists
                    log.Add("Check if user exists...");

                    var existUser = UserManager.FindByEmail(createUser.EMail);
                    if (existUser == null)
                    {
                        log.Add("User was not found (Email: " + createUser.EMail + "). User will be created");
                        var user = new ApplicationUser
                        {
                            UserName = createUser.EMail,
                            Email = createUser.EMail,
                            FirstNameLastName = createUser.FirstName + " " + createUser.LastName
                        };

                        log.Add("Creating user...");

                        var result = UserManager.Create(user);

                        // Check if user was created succesfully
                        if (!result.Succeeded)
                        {
                            resp.IsOK = false;
                            resp.ErrorMsg = "Error creating user (" + result.Errors.ToString() + ")";
                            resp.ReturnedJsonData = string.Join("|", log.ToArray());
                            return View("AutoRegisterError", resp);
                        }

                        log.Add("User successfully created");

                        newUserCreated = true;

                        // Get newly created user
                        var newUser = UserManager.FindByEmail(createUser.EMail);

                        // Create random password
                        log.Add("Create user password, enable user and confirm email address...");

                        code = RandomPassword.GenerateAdwizaPassword();

                        // Update user with random password
                        var result2 = UserManager.AddPassword(newUser.Id, code);

                        // Confirm email right away
                        newUser.EmailConfirmed = true;

                        // Set enabled to true
                        newUser.Enabled = true;

                        // Update user
                        UserManager.Update(newUser);

                        log.Add("User profile successfully updated");

                        // Set common user object
                        theUser = newUser;
                    }
                    else // User exists. He must not be owner of other accounts
                    {
                        log.Add("User already exists. Continuing...");

                        var ownedAccounts = businessLayer.GetOwnedAccountsForUser(existUser.Id);

                        if (ownedAccounts.Count > 0)
                        {
                            var accList = string.Empty;

                            foreach (var acc in ownedAccounts)
                            {
                                accList += "'" + acc.Name + "', ";
                            }

                            accList = accList.Substring(0, accList.Length - 2);

                            if (ownedAccounts.Count > 1)
                                resp.ErrorMsg = string.Format("User {0} is already an account owner for accounts {1}. Purchase not allowed", existUser.Email, accList);
                            else
                                resp.ErrorMsg = string.Format("User {0} is already an account owner of account {1}. Purchase not allowed", existUser.Email, accList);

                            resp.IsOK = false;
                            resp.ReturnedJsonData = string.Join("|", log.ToArray());

                            return View("AutoRegisterError", resp);
                        }

                        // Set common user object
                        theUser = existUser;
                    }

                    log.Add("Creating new account...");

                    // User check complete. Create the new account
                    try
                    {
                        var endZip = "";

                        // Check Zip code
                        var currZipCode = (string.IsNullOrEmpty(createUser.CustZip)) ? "" : createUser.CustZip;

                        // Truncate Zip to maximum 10 chars
                        if (currZipCode.Length > 10)
                            endZip = currZipCode.Substring(0, 10);
                        else
                            endZip = currZipCode;

                        var newAccount = new Account()
                        {
                            ConnectionID = createUser.ConnectionId,
                            Owner = theUser.Id, // User is the new owner of the account
                            URL = createUser.CustId.ToLower(),
                            Name = createUser.OwnerCompanyName,
                            Address = createUser.CustAddress,
                            Address2 = createUser.CustAddress2,
                            Zip = endZip,
                            City = createUser.CustCity,
                            CountryID = createUser.CustCountryId,
                            Enabled = true,
                            IsPartner = true
                        };

                        businessLayer.AddAccount(newAccount);
                        log.Add("New account successfully created");

                        newAccountCreated = true;

                        // Set common account object
                        theAccount = newAccount;
                    }
                    catch (DbEntityValidationException dbEx)
                    {
                        foreach (var validationErrors in dbEx.EntityValidationErrors)
                        {
                            foreach (var validationError in validationErrors.ValidationErrors)
                            {
                                log.Add(string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage));
                            }
                        }
                    }

                    log.Add("Assigning user to account as role Administrator...");

                    // *** Assign user to account with role "Administrator" *** //
                    var userRole = businessLayer.GetRoleByName("Administrator");
                    var newURA = new UserRoleAccount()
                    {
                        RoleID = userRole.Id,
                        UserID = theUser.Id,
                        AccountID = theAccount.AccountID,
                        ActiveAccount = true,
                        UserEnabled = false
                    };

                    businessLayer.AddUserRoleAccount(newURA);

                    log.Add("User successfully assigned Administrator role for account");

                    newUARCreated = true;

                    // Set common UserRoleAccount object
                    theURA = newURA;
                }
                else // Account already exists
                {
                    log.Add("Account already exists. Continuing...");
                    log.Add("Check if user exists...");
                    // Check if user already exists
                    var existUser = UserManager.FindByEmail(createUser.EMail);
                    if (existUser == null)
                    {
                        resp.IsOK = false;
                        var owner = businessLayer.GetUserById(existAccount.Owner);
                        resp.ErrorMsg = string.Format("You are not allowed to purchase apps. Please contact the account owner (Name: {0}, Email: {1})", owner.FirstNameLastName, owner.Email);
                        resp.ReturnedJsonData = string.Join("|", log.ToArray());
                        return View("AutoRegisterError", resp);
                    }

                    log.Add("User exists. Check if he is account owner...");

                    theUser = existUser;

                    // User must be account owner to buy apps
                    if (businessLayer.IsOwnerOfAccount(theUser.Id, existAccount.AccountID).ReturnResponse == ReturnResponse.Error)
                    {
                        resp.IsOK = false;
                        var owner = businessLayer.GetUserById(existAccount.Owner);
                        resp.ErrorMsg = string.Format("You are not allowed to purchase apps. Please contact the account owner (Name: {0}, Email: {1})", owner.FirstNameLastName, owner.Email);
                        resp.ReturnedJsonData = string.Join("|", log.ToArray());
                        return View("AutoRegisterError", resp);
                    }

                    log.Add("User is account owner. Continuing...");

                    theAccount = existAccount;
                }

                // *** Connect app to account (buy app/try app) *** //
                log.Add("Connect app to account...");

                var existAccountApp = businessLayer.GetAccountAppByAccountIdAndAppId(theAccount.AccountID, currApp.AppID);
                var accountAppExists = (existAccountApp == null) ? false : true;
                AccountApp theAccountApp = null;

                if (!accountAppExists)
                {
                    var newAccountApp = new AccountApp()
                    {
                        AccountID = theAccount.AccountID,
                        AppID = createUser.AOSAppId,
                        Activated = true,
                        Installed = true,
                        IsTrial = createUser.IsTrial,
                        TrialStart = (createUser.IsTrial) ? DateTime.Now : (DateTime?)null,
                        TrialExpire = (createUser.IsTrial) ? DateTime.Now.AddDays((int)businessLayer.GetAppById(createUser.AOSAppId).TrialDays) : (DateTime?)null,
                        AppExpires = (createUser.IsTrial) ? DateTime.Now.AddDays((int)businessLayer.GetAppById(createUser.AOSAppId).TrialDays) : (DateTime?)null,
                        AutoUserLicenseAllocation = true,
                        UserPoolCount = currApp.UserPoolCount
                    };

                    businessLayer.AddAccountApp(newAccountApp);

                    newAccountAppCreated = true;

                    theAccountApp = businessLayer.GetAccountAppByAccountIdAndAppId(theAccount.AccountID, createUser.AOSAppId);

                    log.Add("App was successfully assigned to account");
                }
                else // App is already purchased. Just login user and redirect to App Admin page
                {
                    log.Add("App is already purchased. Signing in user and redirecting to App Admin page...");

                    // Sign in user
                    SignInManager.SignIn(theUser, true, false);

                    log.Add("User successfully logged in");

                    return RedirectToAction("IFrameHost", new { AppId = createUser.AOSAppId, SOOnlineEnabled = "true" });
                }

                // *********************************
                // *** Purchase process complete ***
                // *********************************

                log.Add("Purchase process complete");
                log.Add("Creating notification events...");

                // *** Check if we should send email to the user about user and account creation, or just a mail confirming his purchase *** //
                if (!newUserCreated && !newAccountCreated) // This is the account owner purchasing a new app for his account
                {
                    /// *** EVENT: TRYAPPCOMPLETED *** ///

                    // Create notification info
                    var notificationInfo1 = new NotificationInfo()
                    {
                        UserId = theUser.Id,
                        AccountId = theAccount.AccountID,
                        AppId = createUser.AOSAppId,
                        IPAddress = PlatformCommon.GetIPAddress()
                    };

                    // Create mail info
                    var mailInfoList6 = new List<MailInfo>()
                    {
                        new MailInfo
                        {
                            TargetEmail = theUser.Email,
                            Parameters = new string[]
                            {
                                theUser.FirstNameLastName,
                                currApp.Name,
                                theAccount.Name,
                                currApp.TrialDays.ToString(),
                                Convert.ToDateTime(theAccountApp.TrialExpire).ToString("dd-MM-yyyy"),
                                Url.Action("BuyApp", "App", new { id = currApp.AppID }, protocol: Request.Url.Scheme)  }
                        }
                    };


                    // Create log event and send mails if applicable
                    AOSEventHandler.WriteToLog(EventCode.TRYAPPCOMPLETED, notificationInfo1, mailInfoList6, SendMail.NoMails);
                    log.Add("TRYAPPCOMPLETED notification event created");

                    // Create 2nd log event
                    var mailInfoList7 = new List<MailInfo>();
                    AOSEventHandler.WriteToLog(EventCode.APPTRIALPERIODSTARTED, notificationInfo1, mailInfoList7, SendMail.NoMails);
                    log.Add("APPTRIALPERIODSTARTED notification event created");

                    log.Add("Signing in user...");

                    // Sign in user
                    SignInManager.SignIn(theUser, true, false);

                    log.Add("User successfully signed in. Redirecting to App Admin page...");

                    return RedirectToAction("IFrameHost", new { AppId = currApp.AppID, SOOnlineEnabled = "true" });
                }

                // *** Send email confirmation email (create USERANDACCOUNTCREATED event) *** //

                /// *** EVENT: USERANDACCOUNTAUTOCREATEDBYAOS *** ///

                // Create notification info
                var notificationInfo = new NotificationInfo()
                {
                    UserId = theUser.Id,
                    AccountId = theAccount.AccountID,
                    AppId = createUser.AOSAppId,
                    IPAddress = PlatformCommon.GetIPAddress()
                };

                // Create mail info
                var mailInfoList = new List<MailInfo>();

                // Create log event and send mails if applicable
                AOSEventHandler.WriteToLog(EventCode.USERANDACCOUNTAUTOCREATEDBYAOS, notificationInfo, mailInfoList, SendMail.Default);
                log.Add("USERANDACCOUNTAUTOCREATEDBYAOS notification event created");

                var usertoken = BusinessLayer.Utils.ProtectServicePassword(theUser.Id) + "#" + token;
                var routeValues = new RouteValueDictionary();
                routeValues.Add("Token", usertoken);

                // Check if user purchased the app from the SO App Store
                if (createUser.FromSOOnlineAppStore)
                {
                    // Create mail info
                    var mailInfoList2 = new List<MailInfo>()
                    {
                        new MailInfo {
                            TargetEmail = theUser.Email,
                            Parameters = new string[]
                            {
                                theUser.FirstNameLastName, // 1
                                currApp.Name, // 2 
                                theAccount.Name, // 3 
                                currApp.Name, // 4
                                "Trial version", // 5
                                ((DateTime)theAccountApp.TrialStart).ToString("dd-MM-yyyy"), // 6 TODO: Include culture and shortdate format
                                ((DateTime)theAccountApp.TrialExpire).ToString("dd-MM-yyyy"), // 7 TODO: Include culture and shortdate format
                                currApp.TrialDays.ToString(), // 8
                                theUser.Email, // 9
                                code, // 10
                                theAccount.Name, // 11
                                theAccount.URL, // 12
                                theAccount.Address, // 13
                                theAccount.Address2, // 14
                                theAccount.Zip, // 15
                                theAccount.City, // 16
                                businessLayer.GetCountryById(theAccount.CountryID).Name, // 17
                                Url.Action("GoToApp", "Account", routeValues, protocol: Request.Url.Scheme) // 18
                            }
                        }
                    };

                    // Send mail
                    var mail = businessLayer.GetMailById(14);
                    AOSEventHandler.SendEmail(mail, theUser.Email, mailInfoList2[0].Parameters);
                    log.Add("Mail 14 sent");

                }
                else // Purchased from AOS 
                {
                    if (createUser.IsTrial)
                    {
                        // Create mail info
                        var mailInfoList3 = new List<MailInfo>()
                        {
                            new MailInfo {
                                TargetEmail = theUser.Email,
                                Parameters = new string[]
                                {
                                    theUser.FirstNameLastName, // 1
                                    currApp.Name, // 2 
                                    theAccount.Name, // 3 
                                    currApp.Name, // 4
                                    "Trial version", // 5
                                    ((DateTime)theAccountApp.TrialStart).ToString("dd-MM-yyyy"), // 6 TODO: Include culture and shortdate format
                                    ((DateTime)theAccountApp.TrialExpire).ToString("dd-MM-yyyy"), // 7 TODO: Include culture and shortdate format
                                    currApp.TrialDays.ToString(), // 8
                                    theUser.Email, // 9
                                    code, // 10
                                    theAccount.Name, // 11
                                    theAccount.URL, // 12
                                    theAccount.Address, // 13
                                    theAccount.Address2, // 14
                                    theAccount.Zip, // 15
                                    theAccount.City, // 16
                                    businessLayer.GetCountryById(theAccount.CountryID).Name, // 17
                                    Url.Action("GoToApp", "Account", routeValues, protocol: Request.Url.Scheme) // 18
                                }
                            }
                        };

                        // Send mail
                        var mail3 = businessLayer.GetMailById(15);
                        AOSEventHandler.SendEmail(mail3, theUser.Email, mailInfoList3[0].Parameters);
                        log.Add("Mail 15 sent");
                    }
                    else
                    {
                        // Create mail info
                        var mailInfoList4 = new List<MailInfo>()
                        {
                            new MailInfo {
                                TargetEmail = theUser.Email,
                                Parameters = new string[]
                                {
                                    theUser.FirstNameLastName, // 1
                                    currApp.Name, // 2 
                                    theAccount.Name, // 3 
                                    currApp.Name, // 4
                                    "Full version", // 5
                                    theUser.Email, // 6
                                    code, // 7
                                    theAccount.Name, // 8
                                    theAccount.URL, // 9
                                    theAccount.Address, // 10
                                    theAccount.Address2, // 11
                                    theAccount.Zip, // 12
                                    theAccount.City, // 13
                                    businessLayer.GetCountryById(theAccount.CountryID).Name, // 14
                                    Url.Action("GoToApp", "Account", routeValues, protocol: Request.Url.Scheme) // 15
                                }
                            }
                        };

                        // Send mail
                        var mail4 = businessLayer.GetMailById(13);
                        AOSEventHandler.SendEmail(mail4, theUser.Email, mailInfoList4[0].Parameters);
                        log.Add("Mail 13 sent");

                        /// *** EVENT: BUYAPPCOMPLETED *** ///

                        // Create notification info
                        var notificationInfo5 = new NotificationInfo()
                        {
                            UserId = theUser.Id,
                            AccountId = theAccount.AccountID,
                            AppId = currApp.AppID,
                            IPAddress = PlatformCommon.GetIPAddress()
                        };

                        // Create mail info
                        var mailInfoList5 = new List<MailInfo>()
                        {
                            new MailInfo
                            {
                                TargetEmail = theUser.Email,
                                Parameters = new string[] { theUser.FirstNameLastName, currApp.Name, theAccount.Name  }
                            }
                        };

                        // Create log event and send mails if applicable
                        AOSEventHandler.WriteToLog(EventCode.BUYAPPCOMPLETED, notificationInfo, mailInfoList, SendMail.NoMails);
                        log.Add("BUYAPPCOMPLETED notification event created");
                    }
                }

                // Create trial log events if trial version
                if (createUser.IsTrial)
                {
                    /// *** EVENT: TRYAPPCOMPLETED *** ///

                    // Create mail info
                    var mailInfoList6 = new List<MailInfo>()
                    {
                        new MailInfo
                        {
                            TargetEmail = theUser.Email,
                            Parameters = new string[]
                            {
                                theUser.FirstNameLastName,
                                currApp.Name,
                                theAccount.Name,
                                currApp.TrialDays.ToString(),
                                Convert.ToDateTime(theAccountApp.TrialExpire).ToString("dd-MM-yyyy"),
                                Url.Action("TryApp", "App", new { id = currApp.AppID }, protocol: Request.Url.Scheme)  }
                        }
                    };

                    // Create log event and send mails if applicable
                    AOSEventHandler.WriteToLog(EventCode.TRYAPPCOMPLETED, notificationInfo, mailInfoList, SendMail.NoMails);
                    log.Add("TRYAPPCOMPLETED notification event created");

                    // Create 2nd log event
                    var mailInfoList7 = new List<MailInfo>();
                    AOSEventHandler.WriteToLog(EventCode.APPTRIALPERIODSTARTED, notificationInfo, mailInfoList7, SendMail.NoMails);
                    log.Add("APPTRIALPERIODSTARTED notification event created");
                }

                /// Sign in user ///

                // Check if new user was created or we should sign in an existing user
                if (newUserCreated) // Sign in user with new generated password (code)
                {
                    log.Add("Signing in new user...");

                    var res = SignInManager.PasswordSignIn(theUser.Email, code, false, shouldLockout: false);

                    switch (res)
                    {
                        case SignInStatus.Success:
                            log.Add("User successfully signed in. Redirecting to App Admin page...");
                            return RedirectToAction("IFrameHost", new { AppId = currApp.AppID, SOOnlineEnabled = "true" });
                        case SignInStatus.LockedOut:
                            log.Add("User could not be signed in. Reason: User is locked out");
                            return View("Lockout");
                        case SignInStatus.RequiresVerification:
                            log.Add("User not signed in. Reason: Code sent to user");
                            resp.ReturnedJsonData = string.Join("|", log.ToArray());
                            return RedirectToAction("SendCode", new { ReturnUrl = Url.Action("AccountSettings", "User"), RememberMe = false });
                        case SignInStatus.Failure:
                            resp.ErrorMsg = "User could not be signed in. Reason: Failure";
                            resp.IsOK = false;
                            ViewBag.ErrMsg = "Auto-login failed";
                            resp.ReturnedJsonData = string.Join("|", log.ToArray());
                            return View("AutoRegisterError", resp);
                        default:
                            resp.ErrorMsg = "Auto-login failed";
                            resp.IsOK = false;
                            ViewBag.ErrMsg = "Auto-login failed";
                            return View("AutoRegisterError", resp);
                    }
                }
                else // Sign in existing user
                {
                    log.Add("Signing in existing user...");

                    // Sign in user
                    SignInManager.SignIn(theUser, true, false);

                    log.Add("User successfully signed in. Redirecting to App Admin page...");
                }

                // Redirecting
                return RedirectToAction("IFrameHost", new { AppId = currApp.AppID, SOOnlineEnabled = "true" });

            }
            catch (Exception ex)
            {
                var innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                resp.IsOK = false;
                resp.ErrorMsg = "Exception occurred. Message: " + ex.Message + "<br />Inner exception message: " + innerMessage;
                resp.ReturnedJsonData = string.Join("|", log.ToArray());
                return View("AutoRegisterError", resp);
            }
        }

        [AllowAnonymous]
        public ActionResult AutoRegister2(string token, bool useInternalEncryptionRoutine)
        {
            var resp = new AOSResponse();

            var currentURL = Request.Url;
            var isDevelopment = currentURL.ToString().Contains("localhost");

            AutoRegisterPayload payload = new AutoRegisterPayload();
            ApplicationUser theUser = null;
            Account theAccount = null;
            UserRoleAccount theURA = null;
            string code = string.Empty;

            var isFromAppStore = true; // AutoRegister is ONLY invoked in purchase from AppStore situations
            var isTrial = true; // AutoRegister is ONLY invoked in purchase from AppStore situations

            var newUserCreated = false;
            var newAccountCreated = false;
            var newAccountAppCreated = false;
            var newUARCreated = false;

            var log = new List<string>();

            var businessLayer = new BusinessLayer.BusinessLayer();

            try
            {
                // Unpack SAML token and get CreateUser object
                log.Add("Unpacking payload...");

                // Get AppOnlineProvisioning entry
                var currAppOnlineProvisioning = AOSBLL.GetAppOnlineProvisioningByProvisioningKey(token);

                // Decode Base64 string to object
                var decodedObj = WebAPIPlugins.Utils.StringToObject(currAppOnlineProvisioning.ProvisioningValue);

                // Set the payload object
                payload = (AutoRegisterPayload)decodedObj;

                log.Add("Payload unpacked (1)");

                // Validate CreateUser object
                log.Add("Validating payload...");

                var validatePayloadResp = PlatformCommon.ValidatePayload(payload);
                if (!validatePayloadResp.IsOK)
                {
                    resp.IsOK = false;
                    resp.ErrorMsg = validatePayloadResp.ErrorMsg;
                    log.Add("Payload did not validate");
                    resp.ReturnedJsonData = string.Join("|", log.ToArray());
                    return View("AutoRegisterError", resp);
                }

                // HTML encode payload data to prevent XSS attacks
                payload.VisitAddress = HttpUtility.HtmlEncode(payload.VisitAddress);
                payload.PostalAddress = HttpUtility.HtmlEncode(payload.PostalAddress);
                payload.PostalZipCode = HttpUtility.HtmlEncode(payload.PostalZipCode);
                payload.PostalCity = HttpUtility.HtmlEncode(payload.PostalCity);
                payload.PersonEmail = HttpUtility.HtmlEncode(payload.PersonEmail);
                payload.PersonFirstName = HttpUtility.HtmlEncode(payload.PersonFirstName);
                payload.PersonLastName = HttpUtility.HtmlEncode(payload.PersonLastName);
                payload.CompanyName = HttpUtility.HtmlEncode(payload.CompanyName);
                payload.CustId = HttpUtility.HtmlEncode(payload.CustId);

                log.Add("Payload succesfully validated");
                log.Add("Extracted payload data:");
                log.Add("==> App id: <strong>" + payload.AOSAppId.ToString() + "</strong>");
                log.Add("==> Connection id: <strong>" + payload.AOSConnectionId.ToString() + "</strong>");
                log.Add("==> Address: <strong>" + payload.VisitAddress + "</strong>");
                log.Add("==> Address 2: <strong>" + payload.PostalAddress + "</strong>");
                log.Add("==> City: <strong>" + payload.PostalCity + "</strong>");
                log.Add("==> Country id: <strong>" + payload.CountryId.ToString() + "</strong>");
                log.Add("==> Cust id: <strong>" + payload.CustId + "</strong>");
                log.Add("==> Zip: <strong>" + payload.PostalZipCode + "</strong>");
                log.Add("==> Email: <strong>" + payload.PersonEmail + "</strong>");
                log.Add("==> First name: <strong>" + payload.PersonFirstName + "</strong>");
                log.Add("==> Last name: <strong>" + payload.PersonLastName + "</strong>");
                log.Add("==> Company name: <strong>" + payload.CompanyName + "</strong>");

                // ****************************************
                // *** Token fields validation complete ***
                // ****************************************

                log.Add("Check if App record exists...");

                var currApp = businessLayer.GetAppById(payload.AOSAppId);
                if (currApp == null)
                {
                    resp.IsOK = false;
                    resp.ErrorMsg = "App record could not be found (AppId: " + payload.AOSAppId.ToString() + ")";
                    resp.ReturnedJsonData = string.Join("|", log.ToArray());
                    return View("AutoRegisterError", resp);
                }

                log.Add("App record found");

                log.Add("Check if account exists...");

                // Check if the account already exists
                var existAccount = businessLayer.GetAccountByURLThin(payload.CustId);
                if (existAccount == null)
                {
                    log.Add("Account was not found (CustId: " + payload.CustId + "). Account will be created");

                    // Check if user already exists
                    log.Add("Check if user exists...");

                    var existUser = UserManager.FindByEmail(payload.PersonEmail);
                    if (existUser == null)
                    {
                        log.Add("User was not found (Email: " + payload.PersonEmail + "). User will be created");
                        var user = new ApplicationUser
                        {
                            UserName = payload.PersonEmail,
                            Email = payload.PersonEmail,
                            FirstNameLastName = payload.PersonFirstName + " " + payload.PersonLastName
                        };

                        log.Add("Creating user...");

                        var result = UserManager.Create(user);

                        // Check if user was created succesfully
                        if (!result.Succeeded)
                        {
                            resp.IsOK = false;
                            resp.ErrorMsg = "Error creating user (" + result.Errors.ToString() + ")";
                            resp.ReturnedJsonData = string.Join("|", log.ToArray());
                            return View("AutoRegisterError", resp);
                        }

                        log.Add("User successfully created");

                        newUserCreated = true;

                        // Get newly created user
                        var newUser = UserManager.FindByEmail(payload.PersonEmail);

                        // Create random password
                        log.Add("Create user password, enable user and confirm email address...");

                        code = RandomPassword.GenerateAdwizaPassword();

                        // Update user with random password
                        var result2 = UserManager.AddPassword(newUser.Id, code);

                        // Confirm email right away
                        newUser.EmailConfirmed = true;

                        // Set enabled to true
                        newUser.Enabled = true;

                        // Update user
                        UserManager.Update(newUser);

                        log.Add("User profile successfully updated");

                        // Set common user object
                        theUser = newUser;
                    }
                    else // User exists. He must not be owner of other accounts
                    {
                        log.Add("User already exists. Continuing...");

                        var ownedAccounts = businessLayer.GetOwnedAccountsForUser(existUser.Id);

                        if (ownedAccounts.Count > 0)
                        {
                            var accList = string.Empty;

                            foreach (var acc in ownedAccounts)
                            {
                                accList += "'" + acc.Name + "', ";
                            }

                            accList = accList.Substring(0, accList.Length - 2);

                            if (ownedAccounts.Count > 1)
                                resp.ErrorMsg = string.Format("User {0} is already an account owner for accounts {1}. Purchase not allowed", existUser.Email, accList);
                            else
                                resp.ErrorMsg = string.Format("User {0} is already an account owner of account {1}. Purchase not allowed", existUser.Email, accList);

                            resp.IsOK = false;
                            resp.ReturnedJsonData = string.Join("|", log.ToArray());

                            return View("AutoRegisterError", resp);
                        }

                        // Set common user object
                        theUser = existUser;
                    }

                    log.Add("Creating new account...");

                    // User check complete. Create the new account
                    try
                    {
                        var endZip = "";

                        // Check Zip code
                        var currZipCode = (string.IsNullOrEmpty(payload.PostalZipCode)) ? "" : payload.PostalZipCode;

                        // Truncate Zip to maximum 10 chars
                        if (currZipCode.Length > 10)
                            endZip = currZipCode.Substring(0, 10);
                        else
                            endZip = currZipCode;

                        var newAccount = new Account()
                        {
                            ConnectionID = payload.AOSConnectionId,
                            Owner = theUser.Id, // User is the new owner of the account
                            URL = payload.CustId.ToLower(),
                            Name = payload.OwnerCompanyName,
                            Address = payload.VisitAddress,
                            Address2 = payload.PostalAddress,
                            Zip = endZip,
                            City = payload.PostalCity,
                            CountryID = payload.CountryId,
                            Enabled = true,
                            IsPartner = true
                        };

                        businessLayer.AddAccount(newAccount);
                        log.Add("New account successfully created");

                        newAccountCreated = true;

                        // Set common account object
                        theAccount = newAccount;
                    }
                    catch (DbEntityValidationException dbEx)
                    {
                        foreach (var validationErrors in dbEx.EntityValidationErrors)
                        {
                            foreach (var validationError in validationErrors.ValidationErrors)
                            {
                                log.Add(string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage));
                            }
                        }
                    }

                    log.Add("Assigning user to account as role Administrator...");

                    // *** Assign user to account with role "Administrator" *** //
                    var userRole = businessLayer.GetRoleByName("Administrator");
                    var newURA = new UserRoleAccount()
                    {
                        RoleID = userRole.Id,
                        UserID = theUser.Id,
                        AccountID = theAccount.AccountID,
                        ActiveAccount = true,
                        UserEnabled = false
                    };

                    businessLayer.AddUserRoleAccount(newURA);

                    log.Add("User successfully assigned Administrator role for account");

                    newUARCreated = true;

                    // Set common UserRoleAccount object
                    theURA = newURA;
                }
                else // Account already exists
                {
                    log.Add("Account already exists. Continuing...");
                    log.Add("Check if user exists...");
                    // Check if user already exists
                    var existUser = UserManager.FindByEmail(payload.PersonEmail);
                    if (existUser == null)
                    {
                        resp.IsOK = false;
                        var owner = businessLayer.GetUserById(existAccount.Owner);
                        resp.ErrorMsg = string.Format("You are not allowed to purchase apps. Please contact the account owner (Name: {0}, Email: {1})", owner.FirstNameLastName, owner.Email);
                        resp.ReturnedJsonData = string.Join("|", log.ToArray());
                        return View("AutoRegisterError", resp);
                    }

                    log.Add("User exists. Check if he is account owner...");

                    theUser = existUser;

                    // User must be account owner to buy apps
                    if (businessLayer.IsOwnerOfAccount(theUser.Id, existAccount.AccountID).ReturnResponse == ReturnResponse.Error)
                    {
                        resp.IsOK = false;
                        var owner = businessLayer.GetUserById(existAccount.Owner);
                        resp.ErrorMsg = string.Format("You are not allowed to purchase apps. Please contact the account owner (Name: {0}, Email: {1})", owner.FirstNameLastName, owner.Email);
                        resp.ReturnedJsonData = string.Join("|", log.ToArray());
                        return View("AutoRegisterError", resp);
                    }

                    log.Add("User is account owner. Continuing...");

                    theAccount = existAccount;
                }

                // *** Connect app to account (buy app/try app) *** //
                log.Add("Connect app to account...");

                var existAccountApp = businessLayer.GetAccountAppByAccountIdAndAppId(theAccount.AccountID, currApp.AppID);
                var accountAppExists = (existAccountApp == null) ? false : true;
                AccountApp theAccountApp = null;

                if (!accountAppExists)
                {
                    var newAccountApp = new AccountApp()
                    {
                        AccountID = theAccount.AccountID,
                        AppID = payload.AOSAppId,
                        Activated = true,
                        Installed = true,
                        IsTrial = true,
                        TrialStart = DateTime.Now,
                        TrialExpire = DateTime.Now.AddDays((int)businessLayer.GetAppById(payload.AOSAppId).TrialDays),
                        AppExpires = DateTime.Now.AddDays((int)businessLayer.GetAppById(payload.AOSAppId).TrialDays),
                        AutoUserLicenseAllocation = true,
                        UserPoolCount = currApp.UserPoolCount
                    };

                    businessLayer.AddAccountApp(newAccountApp);

                    newAccountAppCreated = true;

                    theAccountApp = businessLayer.GetAccountAppByAccountIdAndAppId(theAccount.AccountID, payload.AOSAppId);

                    log.Add("App was successfully assigned to account");
                }
                else // App is already purchased. Just login user and redirect to App Admin page
                {
                    log.Add("App is already purchased. Signing in user and redirecting to App Admin page...");

                    // Sign in user
                    SignInManager.SignIn(theUser, true, false);

                    log.Add("User successfully logged in");

                    // Write cookies containg current account id
                    AOSBLL.CreateCookie(CookieType.IFrameCookie, theAccount.AccountID);
                    AOSBLL.CreateCookie(CookieType.AOSCookie, theAccount.AccountID);

                    return RedirectToAction("IFrameHost", new { AppId = payload.AOSAppId, SOOnlineEnabled = "true" });
                }

                // *********************************
                // *** Purchase process complete ***
                // *********************************

                log.Add("Purchase process complete");
                log.Add("Creating notification events...");

                // *** Check if we should send email to the user about user and account creation, or just a mail confirming his purchase *** //
                if (!newUserCreated && !newAccountCreated) // This is the account owner purchasing a new app for his account
                {
                    /// *** EVENT: TRYAPPCOMPLETED *** ///

                    // Create notification info
                    var notificationInfo1 = new NotificationInfo()
                    {
                        UserId = theUser.Id,
                        AccountId = theAccount.AccountID,
                        AppId = payload.AOSAppId,
                        IPAddress = PlatformCommon.GetIPAddress()
                    };

                    // Create mail info
                    var mailInfoList6 = new List<MailInfo>()
                    {
                        new MailInfo
                        {
                            TargetEmail = theUser.Email,
                            Parameters = new string[]
                            {
                                theUser.FirstNameLastName,
                                currApp.Name,
                                theAccount.Name,
                                currApp.TrialDays.ToString(),
                                Convert.ToDateTime(theAccountApp.TrialExpire).ToString("dd-MM-yyyy") }
                        }
                        //new MailInfo
                        //{
                        //    TargetEmail = theUser.Email,
                        //    Parameters = new string[]
                        //    {
                        //        theUser.FirstNameLastName,
                        //        currApp.Name,
                        //        theAccount.Name,
                        //        currApp.TrialDays.ToString(),
                        //        Convert.ToDateTime(theAccountApp.TrialExpire).ToString("dd-MM-yyyy"),
                        //        Url.Action("BuyApp", "App", new { id = currApp.AppID }, protocol: Request.Url.Scheme)  }
                        //}
                    };


                    // Create log event and send mails if applicable
                    AOSEventHandler.WriteToLog(EventCode.TRYAPPCOMPLETED, notificationInfo1, mailInfoList6, SendMail.NoMails);
                    log.Add("TRYAPPCOMPLETED notification event created");

                    // Create 2nd log event
                    var mailInfoList7 = new List<MailInfo>();
                    AOSEventHandler.WriteToLog(EventCode.APPTRIALPERIODSTARTED, notificationInfo1, mailInfoList7, SendMail.NoMails);
                    log.Add("APPTRIALPERIODSTARTED notification event created");

                    log.Add("Signing in user...");

                    // Sign in user
                    SignInManager.SignIn(theUser, true, false);

                    log.Add("User successfully signed in. Redirecting to App Admin page...");

                    // Write cookies containg current account id
                    AOSBLL.CreateCookie(CookieType.IFrameCookie, theAccount.AccountID);
                    AOSBLL.CreateCookie(CookieType.AOSCookie, theAccount.AccountID);

                    return RedirectToAction("IFrameHost", new { AppId = currApp.AppID, SOOnlineEnabled = "true" });
                }

                // *** Send email confirmation email (create USERANDACCOUNTCREATED event) *** //

                /// *** EVENT: USERANDACCOUNTAUTOCREATEDBYAOS *** ///

                // Create notification info
                var notificationInfo = new NotificationInfo()
                {
                    UserId = theUser.Id,
                    AccountId = theAccount.AccountID,
                    AppId = payload.AOSAppId,
                    IPAddress = PlatformCommon.GetIPAddress()
                };

                // Create mail info
                var mailInfoList = new List<MailInfo>();

                // Create log event and send mails if applicable
                AOSEventHandler.WriteToLog(EventCode.USERANDACCOUNTAUTOCREATEDBYAOS, notificationInfo, mailInfoList, SendMail.Default);
                log.Add("USERANDACCOUNTAUTOCREATEDBYAOS notification event created");

                var usertoken = BusinessLayer.Utils.ProtectServicePassword(theUser.Id) + "#" + token;
                var routeValues = new RouteValueDictionary();
                routeValues.Add("Token", usertoken);

                if (isFromAppStore)
                {
                    // Create mail info
                    var mailInfoList2 = new List<MailInfo>()
                    {
                        new MailInfo {
                            TargetEmail = theUser.Email,
                            Parameters = new string[]
                            {
                                theUser.FirstNameLastName, // 1
                                currApp.Name, // 2 
                                theAccount.Name, // 3 
                                currApp.Name, // 4
                                "Trial version", // 5
                                ((DateTime)theAccountApp.TrialStart).ToString("dd-MM-yyyy"), // 6 TODO: Include culture and shortdate format
                                ((DateTime)theAccountApp.TrialExpire).ToString("dd-MM-yyyy"), // 7 TODO: Include culture and shortdate format
                                currApp.TrialDays.ToString(), // 8
                                theUser.Email, // 9
                                code, // 10
                                theAccount.Name, // 11
                                theAccount.URL, // 12
                                theAccount.Address, // 13
                                theAccount.Address2, // 14
                                theAccount.Zip, // 15
                                theAccount.City, // 16
                                businessLayer.GetCountryById(theAccount.CountryID).Name, // 17
                                Url.Action("GoToApp", "Account", routeValues, protocol: Request.Url.Scheme) // 18
                            }
                        }
                    };

                    // Send mail
                    var mail = businessLayer.GetMailById(14);
                    AOSEventHandler.SendEmail(mail, theUser.Email, mailInfoList2[0].Parameters);
                    log.Add("Mail 14 sent");

                }
                else // Purchased from AOS 
                {
                    if (isTrial)
                    {
                        // Create mail info
                        var mailInfoList3 = new List<MailInfo>()
                        {
                            new MailInfo {
                                TargetEmail = theUser.Email,
                                Parameters = new string[]
                                {
                                    theUser.FirstNameLastName, // 1
                                    currApp.Name, // 2 
                                    theAccount.Name, // 3 
                                    currApp.Name, // 4
                                    "Trial version", // 5
                                    ((DateTime)theAccountApp.TrialStart).ToString("dd-MM-yyyy"), // 6 TODO: Include culture and shortdate format
                                    ((DateTime)theAccountApp.TrialExpire).ToString("dd-MM-yyyy"), // 7 TODO: Include culture and shortdate format
                                    currApp.TrialDays.ToString(), // 8
                                    theUser.Email, // 9
                                    code, // 10
                                    theAccount.Name, // 11
                                    theAccount.URL, // 12
                                    theAccount.Address, // 13
                                    theAccount.Address2, // 14
                                    theAccount.Zip, // 15
                                    theAccount.City, // 16
                                    businessLayer.GetCountryById(theAccount.CountryID).Name, // 17
                                    Url.Action("GoToApp", "Account", routeValues, protocol: Request.Url.Scheme) // 18
                                }
                            }
                        };

                        // Send mail
                        var mail3 = businessLayer.GetMailById(15);
                        AOSEventHandler.SendEmail(mail3, theUser.Email, mailInfoList3[0].Parameters);
                        log.Add("Mail 15 sent");
                    }
                    else
                    {
                        // Create mail info
                        var mailInfoList4 = new List<MailInfo>()
                        {
                            new MailInfo {
                                TargetEmail = theUser.Email,
                                Parameters = new string[]
                                {
                                    theUser.FirstNameLastName, // 1
                                    currApp.Name, // 2 
                                    theAccount.Name, // 3 
                                    currApp.Name, // 4
                                    "Full version", // 5
                                    theUser.Email, // 6
                                    code, // 7
                                    theAccount.Name, // 8
                                    theAccount.URL, // 9
                                    theAccount.Address, // 10
                                    theAccount.Address2, // 11
                                    theAccount.Zip, // 12
                                    theAccount.City, // 13
                                    businessLayer.GetCountryById(theAccount.CountryID).Name, // 14
                                    Url.Action("GoToApp", "Account", routeValues, protocol: Request.Url.Scheme) // 15
                                }
                            }
                        };

                        // Send mail
                        var mail4 = businessLayer.GetMailById(13);
                        AOSEventHandler.SendEmail(mail4, theUser.Email, mailInfoList4[0].Parameters);
                        log.Add("Mail 13 sent");

                        /// *** EVENT: BUYAPPCOMPLETED *** ///

                        // Create notification info
                        var notificationInfo5 = new NotificationInfo()
                        {
                            UserId = theUser.Id,
                            AccountId = theAccount.AccountID,
                            AppId = currApp.AppID,
                            IPAddress = PlatformCommon.GetIPAddress()
                        };

                        // Create mail info
                        var mailInfoList5 = new List<MailInfo>()
                        {
                            new MailInfo
                            {
                                TargetEmail = theUser.Email,
                                Parameters = new string[] { theUser.FirstNameLastName, currApp.Name, theAccount.Name  }
                            }
                        };

                        // Create log event and send mails if applicable
                        AOSEventHandler.WriteToLog(EventCode.BUYAPPCOMPLETED, notificationInfo, mailInfoList, SendMail.NoMails);
                        log.Add("BUYAPPCOMPLETED notification event created");
                    }
                }

                // Create trial log events if trial version
                if (isTrial)
                {
                    /// *** EVENT: TRYAPPCOMPLETED *** ///

                    // Create mail info
                    var mailInfoList6 = new List<MailInfo>()
                    {
                        new MailInfo
                        {
                            TargetEmail = theUser.Email,
                            Parameters = new string[]
                            {
                                theUser.FirstNameLastName,
                                currApp.Name,
                                theAccount.Name,
                                currApp.TrialDays.ToString(),
                                Convert.ToDateTime(theAccountApp.TrialExpire).ToString("dd-MM-yyyy"),
                                Url.Action("TryApp", "App", new { id = currApp.AppID }, protocol: Request.Url.Scheme)  }
                        }
                    };

                    // Create log event and send mails if applicable
                    AOSEventHandler.WriteToLog(EventCode.TRYAPPCOMPLETED, notificationInfo, mailInfoList, SendMail.NoMails);
                    log.Add("TRYAPPCOMPLETED notification event created");

                    // Create 2nd log event
                    var mailInfoList7 = new List<MailInfo>();
                    AOSEventHandler.WriteToLog(EventCode.APPTRIALPERIODSTARTED, notificationInfo, mailInfoList7, SendMail.NoMails);
                    log.Add("APPTRIALPERIODSTARTED notification event created");
                }

                /// Sign in user ///

                // Check if new user was created or we should sign in an existing user
                if (newUserCreated) // Sign in user with new generated password (code)
                {
                    log.Add("Signing in new user...");

                    var res = SignInManager.PasswordSignIn(theUser.Email, code, false, shouldLockout: false);

                    switch (res)
                    {
                        case SignInStatus.Success:
                            log.Add("User successfully signed in. Redirecting to App Admin page...");

                            // Write cookies containg current account id
                            AOSBLL.CreateCookie(CookieType.AOSCookie, theAccount.AccountID);
                            AOSBLL.CreateCookie(CookieType.IFrameCookie, theAccount.AccountID);

                            return RedirectToAction("IFrameHost", new { AppId = currApp.AppID, SOOnlineEnabled = "true" });
                        case SignInStatus.LockedOut:
                            log.Add("User could not be signed in. Reason: User is locked out");
                            return View("Lockout");
                        case SignInStatus.RequiresVerification:
                            log.Add("User not signed in. Reason: Code sent to user");
                            resp.ReturnedJsonData = string.Join("|", log.ToArray());
                            return RedirectToAction("SendCode", new { ReturnUrl = Url.Action("AccountSettings", "User"), RememberMe = false });
                        case SignInStatus.Failure:
                            resp.ErrorMsg = "User could not be signed in. Reason: Failure";
                            resp.IsOK = false;
                            ViewBag.ErrMsg = "Auto-login failed";
                            resp.ReturnedJsonData = string.Join("|", log.ToArray());
                            return View("AutoRegisterError", resp);
                        default:
                            resp.ErrorMsg = "Auto-login failed";
                            resp.IsOK = false;
                            ViewBag.ErrMsg = "Auto-login failed";
                            return View("AutoRegisterError", resp);
                    }
                }
                else // Sign in existing user
                {
                    log.Add("Signing in existing user...");

                    // Sign in user
                    SignInManager.SignIn(theUser, true, false);

                    log.Add("User successfully signed in. Redirecting to App Admin page...");
                }

                // Write IFrame cookie containg current account id
                AOSBLL.CreateCookie(CookieType.AOSCookie, theAccount.AccountID);
                AOSBLL.CreateCookie(CookieType.IFrameCookie, theAccount.AccountID);

                // Redirecting
                return RedirectToAction("IFrameHost", new { AppId = currApp.AppID, SOOnlineEnabled = "true" });

            }
            catch (Exception ex)
            {
                var innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                resp.IsOK = false;
                resp.ErrorMsg = "Exception occurred. Message: " + ex.Message + "<br />Inner exception message: " + innerMessage;
                resp.ReturnedJsonData = string.Join("|", log.ToArray());
                return View("AutoRegisterError", resp);
            }
        }

        [AllowAnonymous]
        public ActionResult GoToApp(string Token)
        {
            var resp = new AOSResponse();

            // Unpack token
            var unpackedToken = BusinessLayer.Utils.UnprotectServicePassword(Token);

            if (!unpackedToken.Contains("#"))
            {
                resp.IsOK = false;
                resp.ErrorMsg = "Invalid token - error 1";
                return View("Error", resp);
            }

            var arr = unpackedToken.Split('#');

            if (arr.Length != 2)
            {
                resp.IsOK = false;
                resp.ErrorMsg = "Invalid token - error 2";
                return View("Error", resp);
            }

            var userId = arr[0];
            var pwd = arr[1];

            // Get user
            var currUser = UserManager.FindById(userId);

            if (currUser == null)
            {
                resp.IsOK = false;
                resp.ErrorMsg = "User could not be identified";
                return View("Error", resp);
            }

            // Sign in user
            var returnUrl = Url.Action("UserHome", "Home");

            var result = SignInManager.PasswordSignIn(currUser.Email, pwd, false, shouldLockout: false);

            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    resp.IsOK = false;
                    resp.ErrorMsg = "Auto-login failed";
                    return View("Error", resp);
            }

        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    //  Comment the following line to prevent log in until the user is confirmed.
                    //  await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);

                    // Update the user with firstname and lastname
                    var currentUser = UserManager.FindByEmail(model.Email);

                    currentUser.FirstNameLastName = model.FirstName + " " + model.LastName;

                    if (model.Phone != null)
                        currentUser.PhoneNumber = model.Phone;

                    // Set enabled to true
                    currentUser.Enabled = true;

                    UserManager.Update(currentUser);

                    string code = UserManager.GenerateEmailConfirmationToken(user.Id);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    var termsOfServiceUrl = Url.Action("TermsOfService", "User", null, protocol: Request.Url.Scheme);

                    /// *** EVENT: USERCREATED *** ///

                    // Create notification info
                    var notificationInfo = new NotificationInfo()
                    {
                        UserId = currentUser.Id,
                        AccountId = 0,
                        AppId = 0,
                        IPAddress = PlatformCommon.GetIPAddress()
                    };

                    // Create mail info
                    var mailInfoList = new List<MailInfo>()
                    {
                        new MailInfo {
                            TargetEmail = user.Email,
                            Parameters = new string[] { currentUser.FirstNameLastName, termsOfServiceUrl, callbackUrl }
                        }
                    };

                    // Create log event and send mails if applicable
                    AOSEventHandler.WriteToLog(EventCode.USERCREATED, notificationInfo, mailInfoList, SendMail.Default);

                    ViewBag.Message = "Check your email and confirm your account, you must be confirmed "
                                    + "before you can log in.";

                    // For local debug only
                    ViewBag.Link = callbackUrl;

                    return View("Info");
                    //return RedirectToAction("Index", "Home");
                }

                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }

            var result = await UserManager.ConfirmEmailAsync(userId, code);

            if (result.Succeeded)
            {
                var model = new AccountModel();
                model.UserId = userId;

                /// *** EVENT: USERCONFIRMEDEMAIL *** ///

                // Create notification info
                var notificationInfo = new NotificationInfo()
                {
                    UserId = userId,
                    AccountId = 0,
                    AppId = 0,
                    IPAddress = PlatformCommon.GetIPAddress()
                };

                // Create mail info
                var mailInfoList = new List<MailInfo>(); // No emails

                // Create log event and send mails if applicable
                AOSEventHandler.WriteToLog(EventCode.USERCONFIRMEDEMAIL, notificationInfo, mailInfoList, SendMail.Default);

                return View("SignupWizard", model);
            }
            else
                return View("Error", new AOSResponse() { ErrorMsg = $"Error confirming email: ", IsOK = false });
        }

        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmailAndBuy(string userId, string code, int appID, bool isTrial)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);

            if (result.Succeeded)
            {
                var model = new AccountModel();
                model.UserId = userId;
                model.AppID = appID;
                model.IsTrial = isTrial;

                /// *** EVENT: USERCONFIRMEDEMAIL *** ///

                // Create notification info
                var notificationInfo = new NotificationInfo()
                {
                    UserId = userId,
                    AccountId = 0,
                    AppId = 0,
                    IPAddress = PlatformCommon.GetIPAddress()
                };

                // Create mail info
                var mailInfoList = new List<MailInfo>(); // No emails

                // Create log event and send mails if applicable
                AOSEventHandler.WriteToLog(EventCode.USERCONFIRMEDEMAIL, notificationInfo, mailInfoList, SendMail.Default);

                return View("SignupWizard", model);
            }
            else
                return View("Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);

                var protectedEmail = BusinessLayer.Utils.ProtectServicePassword(model.Email);

                var callbackUrl = Url.Action("ResetPassword", "Account", new { email = HttpUtility.UrlEncode(protectedEmail), userId = user.Id, code = HttpUtility.UrlEncode(code) }, protocol: Request.Url.Scheme);

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

                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            ViewBag.Link = TempData["ViewBagLink"];
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string email, string userid, string code)
        {
            var resp = new AOSResponse();

            // Validate email
            if (email == null)
            {
                resp.IsOK = false;
                resp.ErrorMsg = "Email was null";
                return View("Error", resp);
            }

            // Validate email
            if (code == null)
            {
                resp.IsOK = false;
                resp.ErrorMsg = "Code token was null";
                return View("Error", resp);
            }

            var unprotectedEmail = BusinessLayer.Utils.UnprotectServicePassword(HttpUtility.UrlDecode(email));

            code = HttpUtility.UrlDecode(code);

            //code = code.Replace(" ", "+");

            // Create model
            var model = new ResetPasswordViewModel()
            {
                Email = unprotectedEmail,
                Code = code
            };

            return View(model);
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            model.Email = HttpUtility.UrlDecode(model.Email);
            var unprotectedEmail = BusinessLayer.Utils.UnprotectServicePassword(model.Email);

            var user = await UserManager.FindByNameAsync(unprotectedEmail);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }

            model.Code = HttpUtility.UrlDecode(model.Code);
            //model.Code = model.Code.Replace(" ", "+");

            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }

            AddErrors(result);

            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new
            {
                Provider = model.SelectedProvider,
                ReturnUrl = model.ReturnUrl,
                RememberMe = model.RememberMe
            });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // GET: /Account/LogOff
        [HttpGet]
        public ActionResult LogOff2(string dummy)
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult SignupWizard(AccountModel model)
        {
            // Get existing accounts and update model
            var businessLayer = new BusinessLayer.BusinessLayer();

            model.ExistingAccounts = businessLayer.GetAllAccounts();

            return View(model);
        }

        public ActionResult Error()
        {
            return View();
        }

        //[AllowAnonymous]
        //public ActionResult Test()
        //{
        //    var businessLayer = new BusinessLayer.BusinessLayer();
        //    var conn = businessLayer.GetConnectionById(1020);

        //    //var testConnection = new TestConnection(conn);

        //    //ConnectionStatusObj connStatus = testConnection.GetConnectionStatus();
        //    ConnectionStatusObj connStatus = testConnection.GetConnectionStatus();

        //    return Json(connStatus, "json", JsonRequestBehavior.AllowGet);
        //}

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("UserHome", "User");
        }

        private ActionResult RedirectToLocal(string returnUrl, string email)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            TempData["UserEmail"] = email;

            return RedirectToAction("UserHome", "User");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}