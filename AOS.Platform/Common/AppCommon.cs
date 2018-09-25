using AOS.BusinessLayer;
using AOS.DomainModel;
using AOS.Platform.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace AOS.Platform.Common
{
    public class AppCommon
    {
        public static NotifyUserViewModel GetNotifyUserViewModel(int appId, PurchaseVersion version)
        {
            var model = new NotifyUserViewModel();

            var businessLayer = new BusinessLayer.BusinessLayer();

            var app = businessLayer.GetAppById(appId);

            model.AppID = appId;
            model.AppName = app.Name;
            model.FirstNameLastName = "";
            model.Email = "";
            model.OnPremiseBetaVersion = (version == PurchaseVersion.OnPremiseTryBetaVersion) ? true : false;

            return model;
        }


        public static AppViewModel GetAppViewModelForApp(int appId, ApplicationUser currentUser)
        {
            var model = new AppViewModel();
            Account currentAccount = null;
            AccountApp accountApp = null;
            Connection currConnection = null;

            var businessLayer = new BusinessLayer.BusinessLayer();

            // If logged in, check to see if app already has been bought or is on trial
            if (currentUser != null)
            {
                currentAccount = PlatformCommon.GetCurrentAccountForUser(currentUser.Id);
                accountApp = businessLayer.GetAccountAppByAccountIdAndAppId(currentAccount.AccountID, appId);

                if (currentAccount.ConnectionID.HasValue)
                    currConnection = businessLayer.GetConnectionById(currentAccount.ConnectionID);

                if (currentAccount != null && accountApp != null)
                    model.IsBoughtOrOnTrial = true;
                else
                    model.IsBoughtOrOnTrial = false;
            }
            else
                model.IsBoughtOrOnTrial = false;

            var app = businessLayer.GetAppById(appId);

            // Get OnPremise and SOOnline state and texts
            var onPremState = businessLayer.GetAppStateById(app.OnPremiseState);
            var soOnlineState = businessLayer.GetAppStateById(app.SOOnlineState);

            model.OnPremiseState = app.OnPremiseState;
            model.SOOnlineState = app.SOOnlineState;

            // Check account type and don't show buy options for Online version if account is OnPremise and vice-versa
            if (currConnection != null)
            {
                if (currConnection.IsSOOnline)
                {
                    model.OnPremiseState = 0;
                }
                else
                {
                    model.SOOnlineState = 0;
                }
            }

            model.OnPremiseText = businessLayer.GetTextById((int)onPremState.OnPremiseTextID).Text1;
            model.SOOnlineText = businessLayer.GetTextById((int)soOnlineState.SOOnlineTextID).Text1;
            //model.UsesConnectApp = app.SOStartURL.ToLower().Contains("stageconnect");

            // Get mirroring disclaimer text
            var mirroringDisclaimerEntry = businessLayer.GetTextById(49);
            if (mirroringDisclaimerEntry == null)
            {
                var devMirroringDisclaimerEntry = businessLayer.GetTextById(1047);
                model.MirroringDisclaimer = devMirroringDisclaimerEntry.Text1;
            }
            else
                model.MirroringDisclaimer = mirroringDisclaimerEntry.Text1;

            List<App> listOfApps = new List<App>();

            listOfApps.Add(app);

            model.Apps = listOfApps.ToArray();

            foreach (var app2 in model.Apps)
            {
                app2.AppAboutTextShort = GetAppLogo(app2.AppID, false);
            }

            return model;
        }

        public static BuyTryAppViewModel GetBuyTryAppViewModel(int appId, bool isTrial)
        {
            var model = new BuyTryAppViewModel();

            var businessLayer = new BusinessLayer.BusinessLayer();

            var app = businessLayer.GetAppById(appId);

            if (app == null)
                return null;

            model.AppID = app.AppID;
            model.AppName = app.Name;
            model.IsTrial = isTrial;
            model.TrialDays = (int)app.TrialDays;
            model.SOStartURL = app.SOStartURL;
            model.HasMirroring = app.HasMirroring;

            // Get mirroring disclaimer text
            var mirroringDisclaimerEntry = businessLayer.GetTextById(49);
            if (mirroringDisclaimerEntry == null)
            {
                var devMirroringDisclaimerEntry = businessLayer.GetTextById(1047);
                model.MirroringDisclaimer = devMirroringDisclaimerEntry.Text1;
            }
            else
                model.MirroringDisclaimer = mirroringDisclaimerEntry.Text1;

            return model;
        }

        public static AppIconInfo GetAppIconInfo(int appId)
        {
            var resp = new AppIconInfo();

            var businessLayer = new BusinessLayer.BusinessLayer();

            // Check if app has any icons saved
            var appIcons = businessLayer.GetAppIconsByAppId(appId);
            var appLogoPresent = false;
            var trialLogoPresent = false;

            if (appIcons != null)
            {
                appLogoPresent = appIcons.ImageDataAppLogo != null;
                trialLogoPresent = appIcons.ImageDataTrialLogo != null;
            }

            // Get placeholder image in case we need it (if no icons)
            byte[] placeHolderImage = PlatformCommon.GetPlaceholderImage();
            var placeHolderName = "placehold.jpg";
            int placeholderSize = placeHolderImage.Length;

            resp.AppLogoName = appLogoPresent ? appIcons.ImageNameAppLogo : placeHolderName;
            resp.AppLogoSize = appLogoPresent ? (int)appIcons.ImageSizeAppLogo : placeholderSize;
            resp.AppLogoIcon = appLogoPresent ? appIcons.ImageDataAppLogo : placeHolderImage;
            resp.TrialLogoName = trialLogoPresent ? appIcons.ImageNameTrialLogo : placeHolderName;
            resp.TrialLogoSize = trialLogoPresent ? (int)appIcons.ImageSizeTrialLogo : placeholderSize;
            resp.TrialLogoIcon = trialLogoPresent ? appIcons.ImageDataTrialLogo : placeHolderImage;

            return resp;
        }

        public static AppAdminGlobalViewModel GetSingleApp(int appId)
        {
            // Global model
            var model = new AppAdminGlobalViewModel();

            // Sub-models
            var modelGeneralInfo = new AppAdminGeneralInfoViewModel();
            var modelMaintenance = new AppAdminMaintenanceViewModel();
            var modelIcons = new AppAdminIconsViewModel();
            var modelAvailability = new AppAdminAvailabilityViewModel();
            var modelURLInfo = new AppAdminURLInfoViewModel();
            var modelDescrText = new AppAdminDescrTextViewModel();
            var modelAdvanced = new AppAdminAdvancedViewModel();

            var businessLayer = new BusinessLayer.BusinessLayer();

            // If appId is 0, then get first app in database
            var theApp = (appId == 0) ? businessLayer.GetAllApps().First() : businessLayer.GetAppById(appId);

            // Main panel properties
            model.AppId = theApp.AppID;
            model.ListOfApps = (from obj in businessLayer.GetAllApps() // For the dropdown
                                select new AppAlt
                                {
                                    AppID = obj.AppID,
                                    Name = obj.Name
                                }).ToArray();

            // General info properties
            modelGeneralInfo.AppId = theApp.AppID;
            modelGeneralInfo.AppName = theApp.Name;
            modelGeneralInfo.AppEnabled = theApp.Enabled;
            modelGeneralInfo.TrialDays = theApp.TrialDays;
            modelGeneralInfo.UserPoolCount = theApp.UserPoolCount;
            modelGeneralInfo.SendExpireMails = theApp.SendExpireMails;
            model.GeneralInfo = modelGeneralInfo;

            // Maintenance TODO: Replace hardcoded values with real values
            modelMaintenance.AppId = theApp.AppID;
            modelMaintenance.MaintenanceEnabled = theApp.Maintenance;
            modelMaintenance.MaintenanceText = theApp.MaintenanceText;
            modelMaintenance.MaintenancePlannedEnabled = theApp.MaintenancePlanned;
            modelMaintenance.MaintenancePlannedText = theApp.MaintenancePlannedText;
            model.Maintenance = modelMaintenance;

            // Icons properties
            var appIconInfo = GetAppIconInfo(theApp.AppID);

            modelIcons.AppId = theApp.AppID;
            modelIcons.AppLogoIcon = Convert.ToBase64String(appIconInfo.AppLogoIcon, 0, appIconInfo.AppLogoIcon.Length);
            modelIcons.AppLogoName = appIconInfo.AppLogoName;
            modelIcons.AppLogoSize = appIconInfo.AppLogoSize.ToString();
            modelIcons.TrialLogoIcon = Convert.ToBase64String(appIconInfo.TrialLogoIcon, 0, appIconInfo.TrialLogoIcon.Length);
            modelIcons.TrialLogoName = appIconInfo.TrialLogoName;
            modelIcons.TrialLogoSize = appIconInfo.TrialLogoSize.ToString();
            model.Icons = modelIcons;

            // Availability
            modelAvailability.AppId = theApp.AppID;

            modelAvailability.ListOfAppStates = (from obj in businessLayer.GetAllAppStates()
                                                 select new AppStateAlt
                                                 {
                                                     AppStateID = obj.AppStateID,
                                                     State = obj.State
                                                 }).ToArray();

            modelAvailability.AppStateOnPremise = theApp.OnPremiseState;
            modelAvailability.AppStateSOOnline = theApp.SOOnlineState;
            model.Availability = modelAvailability;

            // URL info
            modelURLInfo.AppId = theApp.AppID;
            modelURLInfo.AdminURL = theApp.SOAdminURL;
            modelURLInfo.StartURL = theApp.SOStartURL;
            modelURLInfo.AlternateRedirectURL = theApp.AlternateRedirectURL;
            model.URLInfo = modelURLInfo;

            // Description text
            modelDescrText.AppId = theApp.AppID;
            modelDescrText.AppAboutText = theApp.AppAboutText;
            model.DescrText = modelDescrText;

            // Advanced
            modelAdvanced.AppId = theApp.AppID;
            modelAdvanced.SOAppId = theApp.SOAppID;
            modelAdvanced.ApplicationToken = theApp.ApplicationToken;
            modelAdvanced.SelectedEnvironment = theApp.GlobalSettingsId;
            modelAdvanced.ListOfGlobals = (from obj in businessLayer.GetAllGlobals() // For the dropdown
                                           select new GlobalsAlt
                                           {
                                               GlobalsID = obj.Id,
                                               Name = obj.Environment
                                           }).ToArray();

            // Get Private keys for SOD, STAGE and PROD for the app
            var privateKeySOD = businessLayer.GetAppPrivateKeyByAppIdAndEnvironment(theApp.AppID, "sod");
            var privateKeySTAGE = businessLayer.GetAppPrivateKeyByAppIdAndEnvironment(theApp.AppID, "stage");
            var privateKeyPROD = businessLayer.GetAppPrivateKeyByAppIdAndEnvironment(theApp.AppID, "prod");

            modelAdvanced.PrivateKeyFileNameSOD = (privateKeySOD == null) ? "" : privateKeySOD.PrivateKeyName;
            modelAdvanced.PrivateKeyFileSizeSOD = (privateKeySOD == null) ? 0 : privateKeySOD.PrivateKeyFileSize;
            modelAdvanced.SODXML = (privateKeySOD == null) ? "" : businessLayer.GetXMLFileFromBlob(privateKeySOD.PrivateKeyData);

            modelAdvanced.PrivateKeyFileNameSTAGE = (privateKeySTAGE == null) ? "" : privateKeySTAGE.PrivateKeyName;
            modelAdvanced.PrivateKeyFileSizeSTAGE = (privateKeySTAGE == null) ? 0 : privateKeySTAGE.PrivateKeyFileSize;
            modelAdvanced.STAGEXML = (privateKeySTAGE == null) ? "" : businessLayer.GetXMLFileFromBlob(privateKeySTAGE.PrivateKeyData);

            modelAdvanced.PrivateKeyFileNamePROD = (privateKeyPROD == null) ? "" : privateKeyPROD.PrivateKeyName;
            modelAdvanced.PrivateKeyFileSizePROD = (privateKeyPROD == null) ? 0 : privateKeyPROD.PrivateKeyFileSize;
            modelAdvanced.PRODXML = (privateKeyPROD == null) ? "" : businessLayer.GetXMLFileFromBlob(privateKeyPROD.PrivateKeyData);

            model.Advanced = modelAdvanced;

            return model;
        }

        public static AppAdminGeneralInfoViewModel UpdateGeneralInfo(string userId, AppAdminGeneralInfoViewModel theApp)
        {
            var businessLayer = new BusinessLayer.BusinessLayer(userId);

            try
            {
                // Get app
                var app = businessLayer.GetAppById(theApp.AppId);
                app.Name = theApp.AppName;
                app.Enabled = theApp.AppEnabled;
                app.TrialDays = theApp.TrialDays;
                app.UserPoolCount = theApp.UserPoolCount;
                app.SendExpireMails = theApp.SendExpireMails;

                businessLayer.UpdateApp(app);
            }
            catch (Exception ex)
            {
                return null;
            }

            return theApp;
        }

        public static AppAdminMaintenanceViewModel UpdateMaintenance(string userId, AppAdminMaintenanceViewModel theApp)
        {
            var businessLayer = new BusinessLayer.BusinessLayer(userId);

            try
            {
                // Get app
                var app = businessLayer.GetAppById(theApp.AppId);
                app.Maintenance = theApp.MaintenanceEnabled;
                app.MaintenanceText = theApp.MaintenanceText;
                app.MaintenancePlanned = theApp.MaintenancePlannedEnabled;
                app.MaintenancePlannedText = theApp.MaintenancePlannedText;

                businessLayer.UpdateApp(app);
            }
            catch (Exception ex)
            {
                return null;
            }

            return theApp;
        }

        public static AppAdminAvailabilityViewModel UpdateAvailability(string userId, AppAdminAvailabilityViewModel theApp)
        {
            var businessLayer = new BusinessLayer.BusinessLayer(userId);

            try
            {
                // Get app
                var app = businessLayer.GetAppById(theApp.AppId);
                app.OnPremiseState = theApp.AppStateOnPremise;
                app.SOOnlineState = theApp.AppStateSOOnline;

                businessLayer.UpdateApp(app);
            }
            catch (Exception ex)
            {
                return null;
            }

            return theApp;
        }

        public static AppAdminURLInfoViewModel UpdateURLInfo(string userId, AppAdminURLInfoViewModel theApp)
        {
            var businessLayer = new BusinessLayer.BusinessLayer(userId);

            try
            {
                // Get app
                var app = businessLayer.GetAppById(theApp.AppId);
                app.SOAdminURL = theApp.AdminURL;
                app.SOStartURL = theApp.StartURL;
                app.AlternateRedirectURL = theApp.AlternateRedirectURL;

                businessLayer.UpdateApp(app);
            }
            catch (Exception ex)
            {
                return null;
            }

            return theApp;
        }

        public static AppAdminDescrTextViewModel UpdateAppDescrText(string userId, AppAdminDescrTextViewModel theApp)
        {
            var businessLayer = new BusinessLayer.BusinessLayer(userId);

            try
            {
                // Get app
                var app = businessLayer.GetAppById(theApp.AppId);
                app.AppAboutText = theApp.AppAboutText;

                businessLayer.UpdateApp(app);
            }
            catch (Exception ex)
            {
                return null;
            }

            return theApp;
        }

        public static AppAdminAdvancedViewModel UpdateAdvanced(string userId, AppAdminAdvancedViewModel theApp)
        {
            var businessLayer = new BusinessLayer.BusinessLayer(userId);

            try
            {
                // Get app
                var app = businessLayer.GetAppById(theApp.AppId);
                app.SOAppID = theApp.SOAppId;
                app.ApplicationToken = theApp.ApplicationToken;
                app.GlobalSettingsId = theApp.SelectedEnvironment;

                businessLayer.UpdateApp(app);
            }
            catch (Exception ex)
            {
                return null;
            }

            return theApp;
        }

        public static AppClass GetAppClassInfo(Account currentAccount, ApplicationUser currentUser, int appId, bool SOOnlineEnabled)
        {
            var resp = new AppClass();
            resp.IsOK = true;
            resp.ErrorMsg = string.Empty;

            try
            {
                // If we haven't logged in then just return the model
                if (currentUser == null)
                {
                    resp.IsOK = false;
                    resp.ErrorMsg = "GetAppClassInfo => Current user is null. Is user logged in ?";
                    return resp;
                }

                // If we haven't logged in then just return the model
                if (currentAccount == null)
                {
                    resp.IsOK = false;
                    resp.ErrorMsg = "GetAppClassInfo => Current account is null";
                    return resp;
                }

                // Create businesslayer object
                var businessLayer = new BusinessLayer.BusinessLayer(currentUser.Id);

                // If we haven't logged in then just return the model
                if (appId == 0)
                {
                    resp.IsOK = false;
                    resp.ErrorMsg = "GetAppClassInfo => App id is 0";
                    return resp;
                }

                // Get app information
                var appInfo = businessLayer.GetAppById(appId);

                if (!currentAccount.ConnectionID.HasValue)
                {
                    resp.IsOK = false;
                    resp.ErrorMsg = "Account has no connection";
                    return resp;
                }

                var currConnection = businessLayer.GetConnectionById(currentAccount.ConnectionID);
                if (currConnection == null)
                {
                    resp.IsOK = false;
                    resp.ErrorMsg = $"Account has orphan connection. Connection id: {currentAccount.ConnectionID}";
                    return resp;
                }

                var isSO82 = currConnection.WSVersion >= 82;
                // Create cookie
                businessLayer.CreateCookie(CookieType.IFrameCookie, currentAccount.AccountID);

                if (SOOnlineEnabled)
                    resp.AppAdminURL = HttpUtility.UrlDecode(string.Format("{0}?userId={1}&accountId={2}&soonlineactivate=1", appInfo.SOAdminURL, currentUser.Id, currentAccount.AccountID));
                else
                    resp.AppAdminURL = HttpUtility.UrlDecode(string.Format("{0}?userId={1}&accountId={2}&soonlineactivate=0", appInfo.SOAdminURL, currentUser.Id, currentAccount.AccountID));

                if (appInfo.Code.ToUpper() == "SIGN" & isSO82)
                    resp.AppAdminURL = resp.AppAdminURL.Replace("signature.online", "signature2.online");

                resp.AppIFrameHeader = appInfo.Name + " Administration";

                return resp;
            }
            catch (Exception e)
            {
                resp.IsOK = false;
                resp.ErrorMsg = "GetAppClassInfo => Exception: " + e.Message;
                return resp;
            }

        }

        private static string GetAppLogo(int appId, bool isTrial)
        {
            var businessLayer = new BusinessLayer.BusinessLayer();

            // Get app logo
            var appLogo = businessLayer.GetAppIconsByAppId(appId);
            if (appLogo != null)
            {
                if (isTrial)
                    return (appLogo.ImageDataTrialLogo != null) ? Convert.ToBase64String(appLogo.ImageDataTrialLogo, 0, (int)appLogo.ImageSizeTrialLogo) : string.Empty;
                else
                    return (appLogo.ImageDataAppLogo != null) ? Convert.ToBase64String(appLogo.ImageDataAppLogo, 0, (int)appLogo.ImageSizeAppLogo) : string.Empty;
            }
            else
                return string.Empty;
        }

        private static string GetJSONFromXML(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            return JsonConvert.SerializeXmlNode(doc);
        }

    }


    public enum PurchaseVersion
    {
        OnPremiseTryBetaVersion,
        OnPremiseBuyTry,
        OnPremiseNone,
        SOOnlineTryBetaVersion,
        SOOnlineBuyTry,
        SOOnlineNone
    }
}