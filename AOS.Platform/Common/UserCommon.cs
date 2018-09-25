using AOS.BusinessLayer;
using AOS.DomainModel;
using AOS.Platform.Models;
using SuperOffice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace AOS.Platform.Common
{
    public class UserCommon
    {
        public static UserHomeViewModel GetUserHomeViewModel(ApplicationUser currentUser)
        {
            var model = new UserHomeViewModel();

            // Get businesslayer
            var businessLayer = new BusinessLayer.BusinessLayer(currentUser.Id);

            // Get current account
            var currentAccount = PlatformCommon.GetCurrentAccountForUser(currentUser.Id);

            // If user doesn't have any accounts we cannot continue. We will redirect him to the error page
            if (currentAccount == null)
                return null;

            // Get current connection info
            var currentConnection = PlatformCommon.GetCurrentConnection(currentUser.Id, currentAccount);

            if (currentConnection == null)
                model.EndpointAlive = false;
            else
            {
                // Check if endpoint can be reached

                // Check if endpoint is SO Online. Then perform external check
                if (currentConnection.IsSOOnline)
                {
                    var resp = PlatformCommon.TestSOEndpoint(currentConnection);

                    model.IsSOOnlineAccount = true;
                    model.EndpointAlive = resp.IsOK;
                    model.EndpointStatusText = resp.ErrorMsg;
                }
                else
                {
                    var soUtils = new SOUtils(currentConnection.SysUser, currentConnection.SysPassword, currentConnection.URL);

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
                            model.EndpointAlive = false;
                        }
                        else
                        {
                            model.EndpointAlive = true;
                        }
                    }
                    model.IsSOOnlineAccount = false;
                }
            }

            // Get accountapps for current account
            var accountApps = businessLayer.GetAccountAppsByAccountId(currentAccount.AccountID);

            model.MyAppsExtended = new List<AppExtended>();
            model.AvailableApps = new List<App>();

            // Get app entities from accountapp and get the apps for the account
            foreach (var item in accountApps)
            {
                var app = businessLayer.GetAppById(item.AppID);

                // ConnectApp is skipped
                if (app.Code.ToUpper() == "CONNECTAPP")
                    continue;

                var appExtended = new AppExtended();
                appExtended.MyApp = app;

                // Check if app is Online Signature and account is SO 8.2 OnPrem. If so modify admin url to point to signature2 web app (NuGet packages / agent version)
                if (currentConnection != null)
                {
                    if (currentConnection.WSVersion >= 82 && app.Code.ToUpper() == "SIGN")
                    {
                        appExtended.MyApp.SOAdminURL = appExtended.MyApp.SOAdminURL.Replace("signature.online", "signature2.online");
                    }
                }

                // Check if app is under maintenance
                appExtended.AppUnderMaintenance = app.Maintenance;
                appExtended.AppUnderMaintenanceText = app.MaintenanceText;

                // Get app logo
                appExtended.AppLogoIcon = GetAppLogo(app.AppID, item.IsTrial);

                // Check App Admin page
                appExtended.HasAdminPage = true;
                appExtended.Trial = item.IsTrial;
                appExtended.AppExpires = item.AppExpires;

                // If app has an expiration date, then check if it has expired
                if (appExtended.AppExpires != null)
                {
                    var licenseInfo = businessLayer.CheckAppLicense(item.AccountID, item.AppID);
                    var dateTimeNow = DateTime.Now;
                    var dateAppExpires = (DateTime)appExtended.AppExpires;

                    var appDaysLeft = Convert.ToInt32(Math.Round(((dateAppExpires - dateTimeNow).TotalDays), 0));

                    appExtended.ShowExpirationInfo = true;

                    if (appDaysLeft < 0)
                    {
                        appExtended.AppRemaining = "App has expired";
                        appExtended.AppExpired = true;
                    }
                    else if (appDaysLeft > 1 && appDaysLeft < 15)
                        appExtended.AppRemaining = string.Format("App will expire in {0} days", appDaysLeft);
                    else if (appDaysLeft == 1)
                        appExtended.AppRemaining = "App will expire tomorrow";
                    else
                        appExtended.ShowExpirationInfo = false;
                }

                // If trial, calculate remaining days
                if (item.IsTrial)
                {
                    //var trialDaysTotal = ((DateTime)item.TrialExpire - (DateTime)item.TrialStart).TotalDays;
                    var trialDaysLeft = Convert.ToInt32(Math.Round(((DateTime)item.TrialExpire - DateTime.Now).TotalDays, 0));
                    //var trialDayNow = ((DateTime)item.TrialExpire - DateTime.Now).TotalDays;

                    appExtended.TrialExpired = false;

                    // Check if trial period has expired
                    if (trialDaysLeft < 0)
                    {
                        appExtended.TrialRemaining = "Trial expired";
                        appExtended.TrialExpired = true;
                    }
                    else if (trialDaysLeft > 1)
                        appExtended.TrialRemaining = string.Format("{0} trial days left", trialDaysLeft);
                    else
                        appExtended.TrialRemaining = string.Format("{0} trial day left", trialDaysLeft);

                }

                // Image handling (trial/no trial)
                if (!appExtended.MyApp.AppAboutTextShort.ToLower().Contains("placehold"))
                {
                    if (appExtended.Trial)
                        appExtended.MyApp.AppAboutTextShort = "~/Content/Images/" + appExtended.MyApp.AppAboutTextShort + "_trial.jpg";
                    else
                        appExtended.MyApp.AppAboutTextShort = "~/Content/Images/" + appExtended.MyApp.AppAboutTextShort + ".jpg";
                }
                else
                {
                    if (appExtended.Trial)
                        appExtended.MyApp.AppAboutTextShort = "~/Content/Images/placehold_trial.jpg";
                    else
                        appExtended.MyApp.AppAboutTextShort = "~/Content/Images/placehold.jpg";
                }

                appExtended.SOOnlineState = app.SOOnlineState;
                appExtended.OnPremiseState = app.OnPremiseState;
                appExtended.IsConverted = item.IsConverted;

                model.MyAppsExtended.Add(appExtended);
            }

            // Get a list of all apps and remove those already assigned to the current account
            var allApps = businessLayer.GetAllNormalApps();

            foreach (var item in allApps)
            {
                bool found = false;

                foreach (var item2 in model.MyAppsExtended)
                {
                    // Also remove the Partner App, which is used only internally by developers
                    if (item.AppID == item2.MyApp.AppID)
                    {
                        found = true;
                        break;
                    }
                }

                // Get app logo
                item.AppAboutTextShort = GetAppLogo(item.AppID, false);

                //if (!item.AppAboutTextShort.ToLower().Contains("placehold"))
                //    item.AppAboutTextShort = "~/Content/Images/" + item.AppAboutTextShort + ".jpg";

                if (!found && item.Enabled)
                    model.AvailableApps.Add(item);
            }

            // User role
            var currentUAC = businessLayer.GetUserRoleAccountByUserIdAndAccountId(currentUser.Id, currentAccount.AccountID);
            var currentRole = businessLayer.GetRoleById(currentUAC.RoleID);

            model.IsNormalUser = (currentRole.Name.ToLower() == "user") ? true : false;

            return model;
        }

        public static string GetAppLogo(int appId, bool isTrial)
        {
            var businessLayer = new BusinessLayer.BusinessLayer();

            // Get app logo
            var appLogo = businessLayer.GetAppIconsByAppId(appId);
            if (appLogo != null)
            {
                if (isTrial)
                    return (appLogo.ImageDataTrialLogo != null) ? Convert.ToBase64String(appLogo.ImageDataTrialLogo, 0, (int)appLogo.ImageSizeTrialLogo) : "https://placehold.it/147x200";
                else
                    return (appLogo.ImageDataAppLogo != null) ? Convert.ToBase64String(appLogo.ImageDataAppLogo, 0, (int)appLogo.ImageSizeAppLogo) : "https://placehold.it/147x200";
            }
            else
                return string.Empty;
        }

        public static TermsOfServiceViewModel GetTermsOfServiceViewModel()
        {
            var businessLayer = new BusinessLayer.BusinessLayer();
            var textType = businessLayer.GetTextTypeByName("terms of service");
            var text = businessLayer.GetTextByTypeId(textType.TextTypeID);

            var termsOfServiceText = "<strong>Terms of Service text not found</strong>";

            if (text.Count > 0)
                termsOfServiceText = text[0].Text1;

            var model = new TermsOfServiceViewModel() { TermsOfServiceText = termsOfServiceText };

            return model;
        }
    }
}