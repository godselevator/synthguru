using AOS.DomainModel;
using AOS.Platform.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace AOS.Platform.Common
{
    public class SysAdminCommon
    {
        static BusinessLayer.BusinessLayer BLL = new BusinessLayer.BusinessLayer();

        public static byte[] FileToByteArr(HttpPostedFileBase file)
        {
            byte[] data = new byte[file.ContentLength];
            file.InputStream.Position = 0;
            var bytesRead = file.InputStream.Read(data, 0, data.Length);

            return data;
        }

        public static string IsValidXml(XDocument xdoc, string xsdFilePath)
        {
            bool errors = false;
            var valErrors = string.Empty;

            string xsdMarkup =
                @"<xs:schema attributeFormDefault='unqualified' elementFormDefault='qualified' xmlns:xs='http://www.w3.org/2001/XMLSchema'>
                  <xs:element name='RSAKeyValue'>
                    <xs:complexType>
                      <xs:sequence>
                        <xs:element type='xs:string' name='Modulus'/>
                        <xs:element type='xs:string' name='Exponent'/>
                        <xs:element type='xs:string' name='P'/>
                        <xs:element type='xs:string' name='Q'/>
                        <xs:element type='xs:string' name='DP'/>
                        <xs:element type='xs:string' name='DQ'/>
                        <xs:element type='xs:string' name='InverseQ'/>
                        <xs:element type='xs:string' name='D'/>
                      </xs:sequence>
                    </xs:complexType>
                  </xs:element>
                </xs:schema>";

            XmlSchemaSet schemas = new XmlSchemaSet();
            schemas.Add("", XmlReader.Create(new StringReader(xsdMarkup)));

            xdoc.Validate(schemas, (o, e) =>
            {
                valErrors = e.Message;
                errors = true;
            });

            return errors ? valErrors : string.Empty;
        }

        public static DiagnosticsViewModel GetDiagnosticsViewModel()
        {
            // Check cache
            DiagnosticsViewModel diagnosticsCache = HttpContext.Current.Session["diagnosticsCache"] as DiagnosticsViewModel;

            if (diagnosticsCache != null)
                return diagnosticsCache;

            var model = new DiagnosticsViewModel();
            model.IntegrityCheck = GetIntegrityCheckViewModel();
            model.ConnectionStatus = GetConnectionStatusViewModel(null);

            HttpContext.Current.Session["diagnosticsCache"] = model;

            return model;
        }

        public static IntegrityCheckViewModel GetIntegrityCheckViewModel()
        {
            var model = new IntegrityCheckViewModel();

            // Create filter
            var filterList = new List<DiagnosticFilter>();
            filterList.Add(new DiagnosticFilter() { FilterId = 1, FilterName = "Orphan account owners" });
            filterList.Add(new DiagnosticFilter() { FilterId = 2, FilterName = "Users with no account" });
            filterList.Add(new DiagnosticFilter() { FilterId = 3, FilterName = "Orphan URA users" });
            filterList.Add(new DiagnosticFilter() { FilterId = 4, FilterName = "Accounts with no connection" });
            filterList.Add(new DiagnosticFilter() { FilterId = 5, FilterName = "Accounts with no apps" });
            filterList.Add(new DiagnosticFilter() { FilterId = 6, FilterName = "Orphan connections" });
            model.ListOfFilters = filterList;

            var listOfOrphanAccountOwners = new List<OrphanAccountOwner>();
            var listOfAccountlessUsers = new List<AccountlessUser>();
            var listOfAccountsWithNoConnection = new List<AccountWithNoConnection>();
            var listOfAccountsWithNoApps = new List<AccountWithNoApps>();
            var listOfOrphanConnections = new List<OrphanConnection>();
            var listOfOrphanUsers = new List<OrphanURAUser>();

            // Check orphan account owners
            var accountList = BLL.GetAllAccounts();
            foreach (var account in accountList)
            {
                // Get AspNetUser entry for account owner
                var currOwner = BLL.GetUserById(account.Owner);
                if (currOwner == null)
                    listOfOrphanAccountOwners.Add(new OrphanAccountOwner() { AccountId = account.AccountID, AccountName = account.Name, OwnerId = account.Owner });

                // Check if account has a connection defined
                if (account.ConnectionID == null)
                    listOfAccountsWithNoConnection.Add(new AccountWithNoConnection() { AccountId = account.AccountID, AccountName = account.Name });

                // Check if account has any apps assigned
                var accountApps = BLL.GetAccountAppsByAccountId(account.AccountID);
                if (accountApps.Count == 0)
                    listOfAccountsWithNoApps.Add(new AccountWithNoApps() { AccountId = account.AccountID, AccountName = account.Name });
            }

            var connectionList = BLL.GetAllConnections();
            foreach (var conn in connectionList)
            {
                var account = BLL.GetAccountByConnectionId(conn.ConnectionID);
                if (account == null)
                    listOfOrphanConnections.Add(new OrphanConnection() { ConnectionId = conn.ConnectionID, ConnectionURL = conn.URL });
            }

            // Check users with no account
            var allUsers = BLL.GetAllUsers();
            foreach (var user in allUsers)
            {
                // Skip AOS APIUser
                if (user.Email.ToLower() != "aoswebapiuser@adwiza.com")
                {
                    // Attempt to get entry in URA table
                    var ura = BLL.GetUserRoleAccountByUserId(user.Id);
                    if (ura.Count == 0 || ura == null)
                        listOfAccountlessUsers.Add(new AccountlessUser() { UserId = user.Id, FirstNameLastName = user.FirstNameLastName, Email = user.Email });
                }
            }

            // Check orphan UAR users
            var uraList = BLL.GetAllUserRoleAccounts();
            // Get distinct list of user id's

            var distinctUserIdList = (
                from p in uraList
                select p.UserID)
                .Distinct();

            foreach (var userId in distinctUserIdList)
            {
                // Get AspNetUser entry for user id
                var currUser = BLL.GetUserById(userId);
                if (currUser == null)
                {
                    // Get all URA entries for user
                    var uras = BLL.GetUserRoleAccountByUserId(userId);

                    foreach (var ura in uras)
                    {
                        // Get role 
                        var role = BLL.GetRoleById(ura.RoleID);
                        var account = BLL.GetAccountById(ura.AccountID);

                        listOfOrphanUsers.Add(new OrphanURAUser() { AccountId = account.AccountID, AccountName = account.Name, Role = role.Name, UserId = userId });
                    }
                }
            }

            // Construct stats strings
            model.OrphanAccountOwners = listOfOrphanAccountOwners;
            model.AccountlessUsers = listOfAccountlessUsers;
            model.AccountWithNoConnections = listOfAccountsWithNoConnection;
            model.AccountWithNoApps = listOfAccountsWithNoApps;
            model.OrphanConnections = listOfOrphanConnections;
            model.OrphanURAUsers = listOfOrphanUsers;

            return model;
        }

        public static CheckConnectionViewModel GetConnectionStatusViewModel(string connectionType)
        {
            var model = new CheckConnectionViewModel();
            model.Connections = new List<AOSCheckConnection>();

            var listOfConnectionStatus = new List<string>();

            var connList = BLL.GetAllConnections();
            foreach (var conn in connList)
            {
                // Check if connection type is specified
                if (connectionType == null)
                    model.Connections.Add(AccountCommon.CheckConnection(conn));
                else if (connectionType.ToLower() == "soonline" && conn.IsSOOnline)
                    model.Connections.Add(AccountCommon.CheckConnection(conn));
                else if (connectionType.ToLower() == "onpremise" && !conn.IsSOOnline)
                    model.Connections.Add(AccountCommon.CheckConnection(conn));
                else if (connectionType == null)
                    model.Connections.Add(AccountCommon.CheckConnection(conn));
            }

            return model;
        }

        public static CertificateFileInfo SaveCertificate(HttpPostedFileBase file)
        {
            var resp = new CertificateFileInfo();

            // Check if SO Certificate record exists
            var existSOCertificate = BLL.GetSOCertificate();
            if (existSOCertificate == null)
            {
                var newSOCertificate = new SOCertificate()
                {
                    CertificateName = file.FileName,
                    CertificateData = FileToByteArr(file),
                    CertificateFileSize = file.ContentLength
                };

                BLL.AddSOCertificate(newSOCertificate);
            }
            else // Update
            {
                existSOCertificate.CertificateFileSize = file.ContentLength;
                existSOCertificate.CertificateName = file.FileName;
                existSOCertificate.CertificateData = FileToByteArr(file);

                BLL.UpdateSOCertificate(existSOCertificate);
            }

            resp.Certificate = FileToByteArr(file);
            resp.CertificateName = file.FileName;
            resp.CertificateSize = file.ContentLength;

            return resp;
        }

        public static AppPKeyInfo SavePrivateKey(int appId, string environment, HttpPostedFileBase file)
        {
            var resp = new AppPKeyInfo();

            // Check if app already has a private key entry
            var existAppPrivateKey = BLL.GetAppPrivateKeyByAppIdAndEnvironment(appId, environment);
            if (existAppPrivateKey == null) // Insert
            {
                var newAppPrivateKey = new AppPrivateKey()
                {
                    AppID = appId,
                    Environment = environment,
                    PrivateKeyFileSize = file.ContentLength,
                    PrivateKeyName = file.FileName,
                    PrivateKeyData = FileToByteArr(file)
                };

                BLL.AddAppPrivateKey(newAppPrivateKey);
            }
            else // Update
            {
                existAppPrivateKey.PrivateKeyFileSize = file.ContentLength;
                existAppPrivateKey.PrivateKeyName = file.FileName;
                existAppPrivateKey.PrivateKeyData = FileToByteArr(file);

                BLL.UpdateAppPrivateKey(existAppPrivateKey);
            }

            resp.PKey = FileToByteArr(file);
            resp.PKeyName = file.FileName;
            resp.PKeySize = file.ContentLength;

            return resp;
        }

        public static void RemovePrivateKey(int appId, string environment)
        {
            // Check if app already has a private key entry
            var existAppPrivateKey = BLL.GetAppPrivateKeyByAppIdAndEnvironment(appId, environment);
            if (existAppPrivateKey == null)
                return;

            BLL.RemoveAppPrivateKey(existAppPrivateKey);
        }

        public static void RemoveCertificate()
        {
            // Check if app already has a SO Certificate entry
            var existSOCertificate = BLL.GetSOCertificate();
            if (existSOCertificate == null)
                return;

            BLL.RemoveSOCertificate(existSOCertificate);
        }

        public static AppIconInfoStr SaveLogo(int appId, string logoType, HttpPostedFileBase file)
        {
            var resp = new AppIconInfo();

            // Check if app already has an appicon entry
            var existAppIcons = BLL.GetAppIconsByAppId(appId);

            if (existAppIcons == null) // Insert
            {
                var newAppIcons = new AppIcons()
                {
                    AppID = appId,
                    ImageSizeAppLogo = (logoType.ToLower() == "applogo") ? (int?)file.ContentLength : null,
                    ImageNameAppLogo = (logoType.ToLower() == "applogo") ? file.FileName : null,
                    ImageDataAppLogo = (logoType.ToLower() == "applogo") ? FileToByteArr(file) : null,
                    ImageSizeTrialLogo = (logoType.ToLower() == "triallogo") ? (int?)file.ContentLength : null,
                    ImageNameTrialLogo = (logoType.ToLower() == "triallogo") ? file.FileName : null,
                    ImageDataTrialLogo = (logoType.ToLower() == "triallogo") ? FileToByteArr(file) : null,
                };

                BLL.AddAppIcons(newAppIcons);
            }
            else // Update
            {
                if (logoType.ToLower() == "applogo")
                {
                    existAppIcons.ImageSizeAppLogo = file.ContentLength;
                    existAppIcons.ImageNameAppLogo = file.FileName;
                    existAppIcons.ImageDataAppLogo = FileToByteArr(file);
                }
                else
                {
                    existAppIcons.ImageSizeTrialLogo = file.ContentLength;
                    existAppIcons.ImageNameTrialLogo = file.FileName;
                    existAppIcons.ImageDataTrialLogo = FileToByteArr(file);
                }

                BLL.UpdateAppIcons(existAppIcons);
            }

            // Get new app icon info
            var appIconInfo = AppCommon.GetAppIconInfo(appId);


            var appIconInfoStr = new AppIconInfoStr()
            {
                AppLogoName = appIconInfo.AppLogoName,
                AppLogoSize = appIconInfo.AppLogoSize.ToString(),
                AppLogoIcon = Convert.ToBase64String(appIconInfo.AppLogoIcon, 0, appIconInfo.AppLogoIcon.Length),
                TrialLogoName = appIconInfo.TrialLogoName,
                TrialLogoSize = appIconInfo.TrialLogoSize.ToString(),
                TrialLogoIcon = Convert.ToBase64String(appIconInfo.TrialLogoIcon, 0, appIconInfo.TrialLogoIcon.Length),
            };

            // Return appicon info
            return appIconInfoStr;
        }

        public static AdminHomeEndpointSettingsViewModel UpdateEndpointSettings(string userId, AdminHomeEndpointSettingsViewModel model)
        {
            try
            {
                // Get globals
                var globals = BLL.GetAllGlobals()[0];

                globals.TestEndpoint = model.UpdateEndpoint;
                globals.EndpointPollingEnabled = model.PollingEnabled;
                globals.EndpointPollingInterval = model.PollingInterval;

                //Update globals
                BLL.UpdateGlobals(globals);
            }
            catch (Exception ex)
            {
                return null;
            }

            return model;
        }

        public static AdminHomeNotificationSettingsViewModel UpdateNotificationSettings(string userId, AdminHomeNotificationSettingsViewModel model)
        {
            try
            {
                // Get globals
                var globals = BLL.GetAllGlobals()[0];

                globals.NotificationEmailAddress = model.NotificationEmail;

                // Update globals
                BLL.UpdateGlobals(globals);

                // Update notification settings
                foreach (var item in model.NotificationList)
                {
                    // Get notification
                    var notification = BLL.GetNotificationById(item.NotificationId);

                    // Change sendemail status
                    notification.SendEmail = item.NotificationSendEmail;

                    // Update notification
                    BLL.UpdateNotification(notification);
                }
            }
            catch (Exception ex)
            {
                return null;
            }

            return model;
        }

        public static AdminHomeSOOnlineSettingsViewModel UpdateSOOnlineSettings(string userId, AdminHomeSOOnlineSettingsViewModel model)
        {
            try
            {
                // Get globals depending on environment
                var globals = BLL.GetGlobalsById(model.GlobalsId);

                // Update values
                globals.AdwizaCertificate = model.AdwizaCertificate;
                globals.FederationGateway = model.FederationGateway;
                //globals.SystemTokenCertificatePath = model.SystemTokenCertificatePath;
                globals.SuperIdCertificate = model.SuperIdCertificate;
                globals.CreateUserRedirectURL = model.CreateUserRedirectURL;
                globals.AOSUrl = model.AOSURL;
                globals.MirroringEnabled = model.MirroringStatus;

                // Update globals
                BLL.UpdateGlobals(globals);
            }
            catch (Exception ex)
            {
                return null;
            }

            return model;
        }

        public static AdminHomeAOSWebAPISettingsViewModel UpdateAOSWebAPISettings(AdminHomeAOSWebAPISettingsViewModel model)
        {
            try
            {
                // *** Get globals for SOD **
                var globalsSOD = BLL.GetGlobalsByEnvironmentType("SOD");

                // Update value
                globalsSOD.AOSWebAPIEnabled = model.AOSWebAPIEnabled;

                // Update globals
                BLL.UpdateGlobals(globalsSOD);

                // *** Get globals for STAGE ***
                var globalsSTAGE = BLL.GetGlobalsByEnvironmentType("STAGE");

                // Update value
                globalsSTAGE.AOSWebAPIEnabled = model.AOSWebAPIEnabled;

                // Update globals
                BLL.UpdateGlobals(globalsSTAGE);

                // *** Get globals for PROD ***
                var globalsPROD = BLL.GetGlobalsByEnvironmentType("PROD");

                // Update value
                globalsPROD.AOSWebAPIEnabled = model.AOSWebAPIEnabled;

                // Update globals
                BLL.UpdateGlobals(globalsPROD);
            }
            catch (Exception ex)
            {
                return null;
            }

            return model;
        }

        public static bool SaveNotification(string userId, int notificationId, bool sendEmailStatus)
        {
            try
            {
                // Get notification
                var notification = BLL.GetNotificationById(notificationId);

                // Change sendemail status
                notification.SendEmail = sendEmailStatus;

                // Update notification
                BLL.UpdateNotification(notification);
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        public static AdministratorHomeViewModel SaveNotificationEmailAddress(string userId, AdministratorHomeViewModel model)
        {
            try
            {
                // Save globals
                var globals = BLL.GetAllGlobals()[0];

                globals.NotificationEmailAddress = model.NotificationEmail;

                BLL.UpdateGlobals(globals);
            }
            catch (Exception ex)
            {
                return null;
            }

            return model;
        }

        public static AdminHomeGlobalViewModel GetAdministratorHomeViewModel(int globalsId, ApplicationUserManager userManager)
        {
            var model = new AdminHomeGlobalViewModel();
            var endpointSettingsViewModel = new AdminHomeEndpointSettingsViewModel();
            var notificationSettingsViewModel = new AdminHomeNotificationSettingsViewModel();
            var soOnlineSettingsViewModel = new AdminHomeSOOnlineSettingsViewModel();
            var aosWebAPISettingsViewModel = new AdminHomeAOSWebAPISettingsViewModel();

            // Get Globals for SOD as default
            var DBEnv = BLL.GetDBEnvironment();
            var globals = (globalsId == 0) ? BLL.GetGlobalsByEnvironmentType(DBEnv) : BLL.GetGlobalsById(globalsId);
            model.DBEnvironment = globals.Environment;

            // Extract Endpoint Settings
            endpointSettingsViewModel.UpdateEndpoint = (bool)globals.TestEndpoint;
            endpointSettingsViewModel.PollingEnabled = globals.EndpointPollingEnabled;
            endpointSettingsViewModel.PollingInterval = globals.EndpointPollingInterval;

            // Get Notification Settings
            var allNotifications = BLL.GetAllNotifications();

            var notificationAndTextList = new List<NotificationAndText>();
            foreach (var item in allNotifications)
            {
                var text = BLL.GetTextById(item.TextID);
                var xxx = new NotificationAndText()
                {
                    NotificationId = item.NotificationID,
                    NotificationSendEmail = item.SendEmail,
                    NotificationText = text.Text1
                };

                notificationAndTextList.Add(xxx);
            }

            notificationSettingsViewModel.NotificationList = notificationAndTextList.ToArray();
            notificationSettingsViewModel.NotificationEmail = globals.NotificationEmailAddress;

            // Extract SO Online Settings
            soOnlineSettingsViewModel.ListOfGlobals = (from obj in BLL.GetAllGlobals() // For the dropdown
                                                       select new GlobalsAlt
                                                       {
                                                           GlobalsID = obj.Id,
                                                           Name = obj.Environment
                                                       }).ToArray();

            soOnlineSettingsViewModel.GlobalsId = globals.Id;
            soOnlineSettingsViewModel.Environment = globals.Environment;
            soOnlineSettingsViewModel.AdwizaCertificate = globals.AdwizaCertificate;
            soOnlineSettingsViewModel.FederationGateway = globals.FederationGateway;
            //soOnlineSettingsViewModel.SystemTokenCertificatePath = globals.SystemTokenCertificatePath;
            soOnlineSettingsViewModel.SuperIdCertificate = globals.SuperIdCertificate;
            soOnlineSettingsViewModel.CreateUserRedirectURL = globals.CreateUserRedirectURL;
            soOnlineSettingsViewModel.AOSURL = globals.AOSUrl;
            soOnlineSettingsViewModel.MirroringStatus = globals.MirroringEnabled;

            var soCertificate = BLL.GetSOCertificateByEnvironment(globals.Environment);
            if (soCertificate != null)
            {
                soOnlineSettingsViewModel.AdwizaCertificate = (soCertificate != null) ? BLL.GetXMLFileFromBlob(soCertificate.CertificateData) : string.Empty;
                soOnlineSettingsViewModel.SOCertificateFileName = soCertificate.CertificateName;
                soOnlineSettingsViewModel.SOCertificateFileSize = (int)soCertificate.CertificateFileSize;
            }

            // Extract AOS WebAPI settings
            var aosWebAPIUser = BLL.GetUserByEmail("aoswebapiuser@adwiza.com");
            if (aosWebAPIUser == null)
            {
                aosWebAPISettingsViewModel.AOSWebAPIUser = "Not found";
                aosWebAPISettingsViewModel.Password = "Not found";
                aosWebAPISettingsViewModel.AOSWebAPIEnabled = false;
            }
            else
            {
                aosWebAPISettingsViewModel.AOSWebAPIUser = aosWebAPIUser.UserName;
                aosWebAPISettingsViewModel.Password = "Adwiza4711!";
                aosWebAPISettingsViewModel.AOSWebAPIEnabled = globals.AOSWebAPIEnabled;
            }

            // Add all sub-objects to main View Model object
            model.EndpointSettings = endpointSettingsViewModel;
            model.NotificationSettings = notificationSettingsViewModel;
            model.SOOnlineSettings = soOnlineSettingsViewModel;
            model.AOSWebAPISettings = aosWebAPISettingsViewModel;

            return model;
        }

    }
}