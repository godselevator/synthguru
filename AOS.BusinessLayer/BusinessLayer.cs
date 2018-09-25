using AOS.DomainModel;
using System.Collections.Generic;
using AOS.DataAccessLayer;
using System;
using System.Text;
using System.Web;

namespace AOS.BusinessLayer
{
    public partial interface IBusinessLayer
    {
        SOCallReturn<string> SetSOProperties(string sysUser, string sysPassword, string endpointURL);
        AuthenticationStatus SOGetAuthenticationStatus();
        bool HasServiceLayer();
        string GetDBEnvironment();
        bool IsSystemUser(string userId);
        Account GetCurrentAccountForUser(string userId);
        AppLicenseStatus CheckAppLicense(int accountId, int appId);
        AppUserLicenseStatus CheckAppUserLicense(int accountId, int appId, int associateId);
    }

    public partial class BusinessLayer : IBusinessLayer
    {
        private string _CurrentUser;
        public string CurrentUser
        {
            get { return _CurrentUser; }
            set { _CurrentUser = value; }
        }

        private string _SOSysUser;
        public string SOSysUser
        {
            get { return _SOSysUser; }
            set { _SOSysUser = value; }
        }

        private string _SOSysPassword;
        public string SOSysPassword
        {
            get { return _SOSysPassword; }
            set { _SOSysPassword = value; }
        }

        private string _SOEndpointURL;
        public string SOEndpointURL
        {
            get { return _SOEndpointURL; }
            set { _SOEndpointURL = value; }
        }

        private IAOSServiceLayer _ServiceLayer;
        public IAOSServiceLayer ServiceLayer
        {
            get { return _ServiceLayer; }
            set { _ServiceLayer = value; }
        }

        private IAOSCredentials _SOCredentials;
        public IAOSCredentials SOCredentials
        {
            get { return _SOCredentials; }
            set { _SOCredentials = value; }
        }

        private readonly IAccountRepository _AccountRepository;
        private readonly IAccountAppRepository _AccountAppRepository;
        private readonly IAddressBookRepository _AddressBookRepository;
        private readonly ILogRepository _LogRepository;
        private readonly IAppRepository _AppRepository;
        private readonly IAppStateRepository _AppStateRepository;
        private readonly IRoleRepository _RoleRepository;
        private readonly IUserRepository _UserRepository;
        private readonly IBisNodeInfoRepository _BisNodeInfoRepository;
        private readonly IBisNodeManagerRepository _BisNodeManagerRepository;
        private readonly IConnectionRepository _ConnectionRepository;
        private readonly IConverterRepository _ConverterRepository;
        private readonly IElasticIORepository _ElasticIORepository;
        private readonly ILanguageRepository _LanguageRepository;
        private readonly ILogGroupRepository _LogGroupRepository;
        private readonly IMatchPersonRepository _MatchPersonRepository;
        private readonly IReferralRepository _ReferralRepository;
        private readonly IRelationWiseRepository _RelationWiseRepository;
        private readonly IRWAppointmentStatusRepository _RWAppointmentStatusRepository;
        private readonly IRWAppointmentTaskRepository _RWAppointmentTaskRepository;
        private readonly IRWAppointmentTriggerRepository _RWAppointmentTriggerRepository;
        private readonly IRWChosenVariableRepository _RWChosenVariableRepository;
        private readonly IRWSaleStatusRepository _RWSaleStatusRepository;
        private readonly IRWSaleTriggerRepository _RWSaleTriggerRepository;
        private readonly IRWSOVariableRepository _RWSOVariableRepository;
        private readonly IRWTriggerRepository _RWTriggerRepository;
        private readonly ISignicatRepository _SignicatRepository;
        private readonly ISignicatAccountSignLanguageRepository _SignicatAccountSignLanguageRepository;
        private readonly ISignicatAccountSignMethodRepository _SignicatAccountSignMethodRepository;
        private readonly ISignicatDocumentTemplateRepository _SignicatDocumentTemplateRepository;
        private readonly ISignicatEmailRepository _SignicatEmailRepository;
        private readonly ISignicatLanguageRepository _SignicatLanguageRepository;
        private readonly ISignicatLoggingRepository _SignicatLoggingRepository;
        private readonly ISignicatSignMethodRepository _SignicatSignMethodRepository;
        private readonly ISignicatStatusRepository _SignicatStatusRepository;
        private readonly IUserRoleRepository _UserRoleRepository;
        private readonly IUserRoleAccountRepository _UserRoleAccountRepository;
        private readonly IGlobalsRepository _GlobalsRepository;
        private readonly IMailRepository _MailRepository;
        private readonly INotificationRepository _NotificationRepository;
        private readonly ITextRepository _TextRepository;
        private readonly ICountryRepository _CountryRepository;
        private readonly ITextTypeRepository _TextTypeRepository;
        private readonly IAppIconsRepository _AppIconsRepository;
        private readonly IPDFManagerRepository _PDFManagerRepository;
        private readonly ISyncContactRepository _SyncContactRepository;
        private readonly ISyncPersonRepository _SyncPersonRepository;
        private readonly ISyncSaleRepository _SyncSaleRepository;
        private readonly IAppSystemUserTokenRepository _AppSystemUserTokenRepository;
        private readonly IAppPrivateKeyRepository _AppPrivateKeyRepository;
        private readonly IZapierRepository _ZapierRepository;
        private readonly IDashboardRepository _DashboardRepository;
        private readonly IAccountAppAssociateRepository _AccountAppAssociateRepository;
        private readonly ISessionRepository _SessionRepository;
        private readonly ISOIncludeRepository _SOIncludeRepository;
        private readonly IPBXDialRepository _PBXDialRepository;
        private readonly ISignicatSecureFormRepository _SignicatSecureFormRepository;
        private readonly IVisibleInRepository _VisibleInRepository;
        private readonly IMirroringLogTypeRepository _MirroringLogTypeRepository;
        private readonly IMirroringStatusRepository _MirroringStatusRepository;
        private readonly IMirroringLogRepository _MirroringLogRepository;
        private readonly IMirroringLogLevelRepository _MirroringLogLevelRepository;
        private readonly IAPIKeyRepository _APIKeyRepository;
        private readonly ISOCertificateRepository _SOCertificateRepository;
        private readonly IDataSyncRepository _DataSyncRepository;
        private readonly IInstallInfoRepository _InstallInfoRepository;
        private readonly IMachineNameRepository _MachineNameRepository;
        private readonly ISignicatEventStatusRepository _SignicatEventStatusRepository;
        private readonly IDocArcActivityRepository _DocArcActivityRepository;
        private readonly ISignicatEventLogRepository _SignicatEventLogRepository;
        private readonly ISignicatEventTypeRepository _SignicatEventTypeRepository;
        private readonly ISignicatEventOriginRepository _SignicatEventOriginRepository;
        private readonly IAppOnlineProvisioningRepository _AppOnlineProvisioningRepository;
        private readonly IDocArcTemplateRepository _DocArcTemplateRepository;
        private readonly IDocArcAssociationRepository _DocArcAssociationRepository;
        private readonly IDocArcAdministratorsRepository _DocArcAdministratorsRepository;
        private readonly ILOISLicenseCompanyRepository _LOISLicenseCompanyRepository;
        private readonly ILOISLicenseCompanyKeysRepository _LOISLicenseCompanyKeysRepository;

        // Constructor 1
        public BusinessLayer()
        {
            _AccountRepository = new AccountRepository();
            _AccountAppRepository = new AccountAppRepository();
            _AddressBookRepository = new AddressBookRepository();
            _AppRepository = new AppRepository();
            _AppStateRepository = new AppStateRepository();
            _RoleRepository = new RoleRepository();
            _UserRepository = new UserRepository();
            _BisNodeInfoRepository = new BisNodeInfoRepository();
            _BisNodeManagerRepository = new BisNodeManagerRepository();
            _ConnectionRepository = new ConnectionRepository();
            _ConverterRepository = new ConverterRepository();
            _ElasticIORepository = new ElasticIORepository();
            _LanguageRepository = new LanguageRepository();
            _MatchPersonRepository = new MatchPersonRepository();
            _ReferralRepository = new ReferralRepository();
            _RelationWiseRepository = new RelationWiseRepository();
            _RWAppointmentStatusRepository = new RWAppointmentStatusRepository();
            _RWAppointmentTaskRepository = new RWAppointmentTaskRepository();
            _RWAppointmentTriggerRepository = new RWAppointmentTriggerRepository();
            _RWChosenVariableRepository = new RWChosenVariableRepository();
            _RWSaleStatusRepository = new RWSaleStatusRepository();
            _RWSaleTriggerRepository = new RWSaleTriggerRepository();
            _RWSOVariableRepository = new RWSOVariableRepository();
            _RWTriggerRepository = new RWTriggerRepository();
            _SignicatRepository = new SignicatRepository();
            _SignicatAccountSignLanguageRepository = new SignicatAccountSignLanguageRepository();
            _SignicatAccountSignMethodRepository = new SignicatAccountSignMethodRepository();
            _SignicatDocumentTemplateRepository = new SignicatDocumentTemplateRepository();
            _SignicatEmailRepository = new SignicatEmailRepository();
            _SignicatLanguageRepository = new SignicatLanguageRepository();
            _SignicatLoggingRepository = new SignicatLoggingRepository();
            _SignicatSignMethodRepository = new SignicatSignMethodRepository();
            _SignicatStatusRepository = new SignicatStatusRepository();
            _UserRoleRepository = new UserRoleRepository();
            _UserRoleAccountRepository = new UserRoleAccountRepository();
            _GlobalsRepository = new GlobalsRepository();
            _MailRepository = new MailRepository();
            _NotificationRepository = new NotificationRepository();
            _TextRepository = new TextRepository();
            _CountryRepository = new CountryRepository();
            _LogRepository = new LogRepository();
            _LogGroupRepository = new LogGroupRepository();
            _TextTypeRepository = new TextTypeRepository();
            _AppIconsRepository = new AppIconsRepository();
            _PDFManagerRepository = new PDFManagerRepository();
            _SyncContactRepository = new SyncContactRepository();
            _SyncPersonRepository = new SyncPersonRepository();
            _SyncSaleRepository = new SyncSaleRepository();
            _AppSystemUserTokenRepository = new AppSystemUserTokenRepository();
            _AppPrivateKeyRepository = new AppPrivateKeyRepository();
            _ZapierRepository = new ZapierRepository();
            _DashboardRepository = new DashboardRepository();
            _AccountAppAssociateRepository = new AccountAppAssociateRepository();
            _SessionRepository = new SessionRepository();
            _SOIncludeRepository = new SOIncludeRepository();
            _PBXDialRepository = new PBXDialRepository();
            _SignicatSecureFormRepository = new SignicatSecureFormRepository();
            _VisibleInRepository = new VisibleInRepository();
            _MirroringLogTypeRepository = new MirroringLogTypeRepository();
            _MirroringStatusRepository = new MirroringStatusRepository();
            _MirroringLogRepository = new MirroringLogRepository();
            _MirroringLogLevelRepository = new MirroringLogLevelRepository();
            _APIKeyRepository = new APIKeyRepository();
            _SOCertificateRepository = new SOCertificateRepository();
            _DataSyncRepository = new DataSyncRepository();
            _InstallInfoRepository = new InstallInfoRepository();
            _MachineNameRepository = new MachineNameRepository();
            _SignicatEventStatusRepository = new SignicatEventStatusRepository();
            _DocArcActivityRepository = new DocArcActivityRepository();
            _SignicatEventLogRepository = new SignicatEventLogRepository();
            _SignicatEventTypeRepository = new SignicatEventTypeRepository();
            _SignicatEventOriginRepository = new SignicatEventOriginRepository();
            _AppOnlineProvisioningRepository = new AppOnlineProvisioningRepository();
            _DocArcTemplateRepository = new DocArcTemplateRepository();
            _DocArcAssociationRepository = new DocArcAssociationRepository();
            _DocArcAdministratorsRepository = new DocArcAdministratorsRepository();
            _LOISLicenseCompanyRepository = new LOISLicenseCompanyRepository();
            _LOISLicenseCompanyKeysRepository = new LOISLicenseCompanyKeysRepository();
        }

        // Constructor 2
        public BusinessLayer(string currentUser)
        {
            _CurrentUser = currentUser;
            _AccountRepository = new AccountRepository();
            _AccountAppRepository = new AccountAppRepository();
            _AddressBookRepository = new AddressBookRepository();
            _AppRepository = new AppRepository();
            _AppStateRepository = new AppStateRepository();
            _RoleRepository = new RoleRepository();
            _UserRepository = new UserRepository();
            _BisNodeInfoRepository = new BisNodeInfoRepository();
            _BisNodeManagerRepository = new BisNodeManagerRepository();
            _ConnectionRepository = new ConnectionRepository();
            _ConverterRepository = new ConverterRepository();
            _ElasticIORepository = new ElasticIORepository();
            _LanguageRepository = new LanguageRepository();
            _MatchPersonRepository = new MatchPersonRepository();
            _ReferralRepository = new ReferralRepository();
            _RelationWiseRepository = new RelationWiseRepository();
            _RWAppointmentStatusRepository = new RWAppointmentStatusRepository();
            _RWAppointmentTaskRepository = new RWAppointmentTaskRepository();
            _RWAppointmentTriggerRepository = new RWAppointmentTriggerRepository();
            _RWChosenVariableRepository = new RWChosenVariableRepository();
            _RWSaleStatusRepository = new RWSaleStatusRepository();
            _RWSaleTriggerRepository = new RWSaleTriggerRepository();
            _RWSOVariableRepository = new RWSOVariableRepository();
            _RWTriggerRepository = new RWTriggerRepository();
            _SignicatRepository = new SignicatRepository();
            _SignicatAccountSignLanguageRepository = new SignicatAccountSignLanguageRepository();
            _SignicatAccountSignMethodRepository = new SignicatAccountSignMethodRepository();
            _SignicatDocumentTemplateRepository = new SignicatDocumentTemplateRepository();
            _SignicatEmailRepository = new SignicatEmailRepository();
            _SignicatLanguageRepository = new SignicatLanguageRepository();
            _SignicatLoggingRepository = new SignicatLoggingRepository();
            _SignicatSignMethodRepository = new SignicatSignMethodRepository();
            _SignicatStatusRepository = new SignicatStatusRepository();
            _UserRoleRepository = new UserRoleRepository();
            _UserRoleAccountRepository = new UserRoleAccountRepository();
            _GlobalsRepository = new GlobalsRepository();
            _MailRepository = new MailRepository();
            _NotificationRepository = new NotificationRepository();
            _TextRepository = new TextRepository();
            _CountryRepository = new CountryRepository();
            _LogRepository = new LogRepository();
            _LogGroupRepository = new LogGroupRepository();
            _TextTypeRepository = new TextTypeRepository();
            _AppIconsRepository = new AppIconsRepository();
            _PDFManagerRepository = new PDFManagerRepository();
            _SyncContactRepository = new SyncContactRepository();
            _SyncPersonRepository = new SyncPersonRepository();
            _SyncSaleRepository = new SyncSaleRepository();
            _AppSystemUserTokenRepository = new AppSystemUserTokenRepository();
            _AppPrivateKeyRepository = new AppPrivateKeyRepository();
            _ZapierRepository = new ZapierRepository();
            _DashboardRepository = new DashboardRepository();
            _AccountAppAssociateRepository = new AccountAppAssociateRepository();
            _SessionRepository = new SessionRepository();
            _SOIncludeRepository = new SOIncludeRepository();
            _PBXDialRepository = new PBXDialRepository();
            _SignicatSecureFormRepository = new SignicatSecureFormRepository();
            _VisibleInRepository = new VisibleInRepository();
            _MirroringLogTypeRepository = new MirroringLogTypeRepository();
            _MirroringStatusRepository = new MirroringStatusRepository();
            _MirroringLogRepository = new MirroringLogRepository();
            _MirroringLogLevelRepository = new MirroringLogLevelRepository();
            _APIKeyRepository = new APIKeyRepository();
            _SOCertificateRepository = new SOCertificateRepository();
            _DataSyncRepository = new DataSyncRepository();
            _InstallInfoRepository = new InstallInfoRepository();
            _MachineNameRepository = new MachineNameRepository();
            _SignicatEventStatusRepository = new SignicatEventStatusRepository();
            _DocArcActivityRepository = new DocArcActivityRepository();
            _SignicatEventLogRepository = new SignicatEventLogRepository();
            _SignicatEventTypeRepository = new SignicatEventTypeRepository();
            _SignicatEventOriginRepository = new SignicatEventOriginRepository();
            _AppOnlineProvisioningRepository = new AppOnlineProvisioningRepository();
            _DocArcTemplateRepository = new DocArcTemplateRepository();
            _DocArcAssociationRepository = new DocArcAssociationRepository();
            _DocArcAdministratorsRepository = new DocArcAdministratorsRepository();
            _LOISLicenseCompanyRepository = new LOISLicenseCompanyRepository();
            _LOISLicenseCompanyKeysRepository = new LOISLicenseCompanyKeysRepository();
        }

        public SOCallReturn<string> SetSOProperties(string sysUser, string sysPassword, string endpointURL)
        {
            var returnVal = new SOCallReturn<string>();

            this.SOSysUser = sysUser;
            this.SOSysPassword = sysPassword;
            this.SOEndpointURL = endpointURL;

            // Attempt to find endpoint in database
            var currConnection = GetConnectionByURL(endpointURL);

            string version = "so75";

            //if (currConnection == null)
            //    version = ServiceLayerUtils.GetSOServiceVersion(endpointURL);
            //else
            //{
            //    if (currConnection.WSHttps)
            //        version = "so" + currConnection.WSVersion.ToString() + "ssl";
            //    else
            //        version = "so" + currConnection.WSVersion.ToString();
            //}

            this.ServiceLayer = AOSServiceLayerFactory.GetAOSServiceLayer(endpointURL, version);

            if (this.ServiceLayer == null)
            {
                returnVal.IsOK = false;
                returnVal.ErrorMsg = version;
                returnVal.ReturnValue = version;
                return returnVal;
            }

            // Connect to SO and get credentials
            this.SOCredentials = this.ServiceLayer.SOGetCredentials(sysUser, sysPassword);

            returnVal.ReturnValue = this.SOCredentials.ErrorMsg;
            returnVal.IsOK = this.SOCredentials.Success;

            return returnVal;
        }

        public AuthenticationStatus SOGetAuthenticationStatus()
        {
            if (this.SOCredentials == null)
                return AuthenticationStatus.NotAuthenticated;

            if (!this.SOCredentials.Success)
                return AuthenticationStatus.InvalidURL;

            if (this.SOCredentials.AuthenticationSucceeded)
                return AuthenticationStatus.Authenticated;
            else
                return AuthenticationStatus.NotAuthenticated;
        }

        public bool HasServiceLayer()
        {
            return this.ServiceLayer != null;
        }

        public string GetDBEnvironment()
        {
            var connString = DataAccessLayer.Utils.GetDBConnectionString();

            if (connString.Contains("adwizastagingsqlserver"))
                return "STAGE";
            else if (connString.Contains("adwizasqlserver"))
                return "PROD";
            else if (connString.Contains("192.168"))
                return "SOD";
            else
                return "UNKNOWN";
        }

        public AppLicenseStatus CheckAppLicense(int accountId, int appId)
        {
            var resp = new AppLicenseStatus();

            // Validate AccountApp entry
            var accountApp = GetAccountAppByAccountIdAndAppId(accountId, appId);
            if (accountApp == null)
            {
                resp.LicenseStatus = LicenseStatusType.Error;
                resp.Message = string.Format("ERROR: AccountApp entry with accountId:{0} and appId:{1} not found", accountId, appId);
                return resp;
            }

            var theAccountApp = accountApp;

            // Check if expire date is filled
            if (theAccountApp.AppExpires == null)
            {
                resp.LicenseStatus = LicenseStatusType.Error;
                resp.Message = string.Format("ERROR: No license info found (AppExpires column is NULL) for AccountApp entry with accountId:{0} and appId:{1}", accountId, appId);
                return resp;
            }

            // Check if app license has expired
            if (theAccountApp.AppExpires < DateTime.Now)
            {
                resp.LicenseStatus = LicenseStatusType.Expired;
                var dtExp = ((DateTime)theAccountApp.AppExpires).ToString("yyyy-MM-dd");
                resp.Message = string.Format("EXPIRED: The license expired at {0}. License invalid", dtExp);
                return resp;
            }

            // License has not expired. Check if it is expiring within the next 8 days
            var trialDaysLeft = Convert.ToInt32(Math.Round(((DateTime)theAccountApp.AppExpires - DateTime.Now).TotalDays, 0));
            if (trialDaysLeft < 8)
            {
                resp.LicenseStatus = LicenseStatusType.Expiring;
                var dtExp = ((DateTime)theAccountApp.AppExpires).ToString("yyyy-MM-dd");
                resp.Message = string.Format("WARNING: App expires in {0} day(s) on {1}. License OK", trialDaysLeft, dtExp);
                return resp;
            }

            resp.LicenseStatus = LicenseStatusType.OK;
            resp.Message = string.Empty;

            return resp;
        }

        public AppUserLicenseStatus CheckAppUserLicense(int accountId, int appId, int associateId)
        {
            var resp = new AppUserLicenseStatus();

            // Get current app
            var currApp = GetAppById(appId);
            if (currApp == null)
            {
                resp.LicenseStatus = UserLicenseStatusType.Error;
                resp.Message = string.Format("ERROR: App entry with AppId:{0} not found", appId);
                return resp;
            }

            // Check user pool count
            if (currApp.UserPoolCount == 0)
            {
                resp.LicenseStatus = UserLicenseStatusType.OK;
                resp.Message = string.Empty;
                return resp;
            }

            // Validate AccountApp entry
            var accountApp = GetAccountAppByAccountIdAndAppId(accountId, appId);
            if (accountApp == null)
            {
                resp.LicenseStatus = UserLicenseStatusType.Error;
                resp.Message = string.Format("ERROR: AccountApp entry with accountId:{0} and appId:{1} not found", accountId, appId);
                return resp;
            }

            // Check if associate can be found in AccountAppAssociate table
            var accountAppAssociate = GetAccountAppAssociate(accountId, appId, associateId);
            if (accountAppAssociate != null)
            {
                resp.LicenseStatus = UserLicenseStatusType.OK;
                resp.Message = string.Empty;
                return resp;
            }

            // Associate was not found. Check if AutoUserLicenseAllocation is turned on
            if (!accountApp.AutoUserLicenseAllocation)
            {
                resp.LicenseStatus = UserLicenseStatusType.NoUserLicense;
                resp.Message = "Associate was not found in User License table and AutoUserLicenseAllocation is switched off";
                return resp;
            }

            // Check available seats
            var availableSeats = GetAccountAppAssociatesByAccountIdAndAppId(accountId, appId);
            if (availableSeats.Count >= currApp.UserPoolCount)
            {
                resp.LicenseStatus = UserLicenseStatusType.UserPoolSizeLimitReached;
                resp.Message = "Associate was not found in User License table and maximum UserPoolCount has been reached";
                return resp;
            }

            var newAccountAppAssociate = new AccountAppAssociate()
            {
                AccountID = accountId,
                AppID = appId,
                AssociateID = associateId
            };

            AddAccountAppAssociate(newAccountAppAssociate);

            resp.LicenseStatus = UserLicenseStatusType.OK;
            resp.Message = string.Format("Associate Id was added to User License Pool. Seats left: {0}", currApp.UserPoolCount - (availableSeats.Count + 1));

            return resp;
        }

        public bool IsSystemUser(string userId)
        {
            var currentAccountId = GetCurrentAccountForUser(userId).AccountID;
            var currentURA = GetUserRoleAccountByUserIdAndAccountId(userId, currentAccountId);
            var currentRole = GetRoleById(currentURA.RoleID);

            return currentRole.Name.ToLower() == "system user";
        }

        public Account GetCurrentAccountForUser(string userId)
        {
            var userRoleAccounts = GetUserRoleAccountByUserId(userId);

            var currentAccountId = 0;
            var currentRoleId = string.Empty;

            // Add accounts to user accounts object
            foreach (var uac in userRoleAccounts)
            {
                if (uac.ActiveAccount == true)
                {
                    currentAccountId = uac.AccountID;
                    currentRoleId = uac.RoleID;
                }
            }

            if (currentAccountId != 0)
                return GetAccountById(currentAccountId);
            else
                return null;
        }

        public string GetXMLFileFromBlob(byte[] byteArr)
        {
            if (byteArr.Length == 0)
                return "Blob has zero length";

            if (byteArr == null)
                return "Blob is null";

            return Encoding.ASCII.GetString(byteArr);
        }

        public List<string> GetPrivateKeysAsStringForApp(int appId)
        {
            var resp = new List<string>();

            var currApp = GetAppById(appId);
            if (currApp == null)
            {
                resp.Add("App not found");
                return resp;
            }

            var listOfPrivateKeys = GetAppPrivateKeysByAppId(appId);

            foreach (var item in listOfPrivateKeys)
            {
                resp.Add(GetXMLFileFromBlob(item.PrivateKeyData));
            }

            return resp;
        }

        public string GetPrivateKeysAsStringByAppAndEnvironment(int appId, string environment)
        {
            var currApp = GetAppById(appId);
            if (currApp == null)
                return "App not found";

            switch (environment.ToLower())
            {
                case "sod":
                case "stage":
                case "prod":
                    break;
                default:
                    return ($"Invalid environment: {environment}. Use SOD, STAGE or PROD (nor case-sensitive)");
            }

            var privateKeyEntry = GetAppPrivateKeyByAppIdAndEnvironment(appId, environment);
            if (privateKeyEntry == null)
                return string.Format("Private key entry for app: {0} and environment: {1} does not exist", appId, environment);

            if (privateKeyEntry.PrivateKeyData == null)
                return string.Format("Private key data for app: {0} and environment: {1} not found", appId, environment);

            return GetXMLFileFromBlob(privateKeyEntry.PrivateKeyData);
        }

        public bool AppDebugModeEnabled(string appCode)
        {
            // Get App record
            var currApp = GetAppByCode(appCode);

            return (currApp == null) ? false : (currApp.DebugEnabled);
        }

        public void CreateCookie(CookieType cookieType, int accountId)
        {
            var cookieTokenPrefix = cookieType.ToString().ToLower() + "token";
            var cookieName = cookieType.ToString().ToLower();

            // Encrypt, Base64 encode and URL ENcode cookie value
            var cookieValue = Utils.ProtectServicePassword($"{cookieTokenPrefix}_{accountId}");
            var cookieValueBase64 = Convert.ToBase64String(Encoding.ASCII.GetBytes(cookieValue));
            var cookieValueBase64URLEncoded = HttpUtility.UrlEncode(cookieValueBase64);

            // Setup cookie if 
            HttpCookie cookie = new HttpCookie(cookieName);

            // Cookie must be readable for all subdomains under online.adwiza.com hence the "." wildcard
            cookie.Domain = ".online.adwiza.com";
            cookie.Value = cookieValueBase64URLEncoded;
            cookie.HttpOnly = true;
            cookie.Secure = true;
            cookie.Expires = DateTime.Now.AddYears(50); // For a cookie to effectively never expire

            // Add the cookie.
            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        private CookieStatusResponse ValidateCookie(CookieType cookieType, out HttpCookie cookie, out int accountId)
        {
            Logger.LogEvent("ValidateCookie", LogType.Info, $"cookieType: {cookieType}");

            var resp = new CookieStatusResponse();

            // Construct cookie name
            var cookieName = cookieType.ToString().ToLower();
            var cookieTokenPrefix = cookieName + "token";

            Logger.LogEvent("ValidateCookie", LogType.Info, $"cookieName: {cookieName}");
            Logger.LogEvent("ValidateCookie", LogType.Info, $"cookieTokenPrefix: {cookieName}");

            accountId = 0;

            // Get cookie
            cookie = HttpContext.Current.Request.Cookies[cookieName];

            if (cookie == null)
            {
                Logger.LogEvent("ValidateCookie", LogType.Info, $"Cookie not found. Return errormessage: Error (0)");
                resp.IsOK = false;
                resp.ErrorMessage = "Error (0)";
                return resp;
            }

            if (cookie.Value == null)
            {
                Logger.LogEvent("ValidateCookie", LogType.Info, $"Cookie value null. Return errormessage: Error (1)");
                resp.IsOK = false;
                resp.ErrorMessage = "Error (1)";
                return resp;
            }

            var unprotectedCookie = string.Empty;

            // URLDecode cookie value
            var cookieURLDecoded = HttpUtility.UrlDecode(cookie.Value);
            var cookieBase64Decoded = string.Empty;

            try
            {
                byte[] base64ByteArr = Convert.FromBase64String(cookieURLDecoded);
                cookieBase64Decoded = Encoding.ASCII.GetString(base64ByteArr);
                unprotectedCookie = Utils.UnprotectServicePassword(cookieBase64Decoded);
            }
            catch (Exception ex)
            {
                Logger.LogEvent("ValidateCookie", LogType.Info, $"Exception: {ex.Message}. Return errormessage: Error (2)");
                resp.IsOK = false;
                resp.ErrorMessage = "Error (2)";
                return resp;
            }

            // Validate cookie value
            var splitVal = unprotectedCookie.Split('_');

            Logger.LogEvent("ValidateCookie", LogType.Info, $"splitVal: {splitVal}");

            // splitVal[0] must contain the value "tokencheck"
            if (string.IsNullOrEmpty(splitVal[0]))
            {
                Logger.LogEvent("ValidateCookie", LogType.Info, $"Token-prefix is null or empty. Return errormessage: Error (3)");
                resp.IsOK = false;
                resp.ErrorMessage = "Error (3)";
                return resp;
            }

            if (splitVal[0] != cookieTokenPrefix)
            {
                Logger.LogEvent("ValidateCookie", LogType.Info, $"Token-prefix invalid: {splitVal[0]}. Return errormessage: Error (4)");
                resp.IsOK = false;
                resp.ErrorMessage = "Error (4)";
                return resp;
            }

            // Account ID must be in splitVal[1]
            if (string.IsNullOrEmpty(splitVal[1]))
            {
                Logger.LogEvent("ValidateCookie", LogType.Info, $"Encrypted account id is null or empty. Return errormessage: Error (5)");
                resp.IsOK = false;
                resp.ErrorMessage = "Error (5)";
                return resp;
            }

            // Ensure that account id match
            try
            {
                Logger.LogEvent("ValidateCookie", LogType.Info, "Convert account id to int and validate that it exists...");
                accountId = Convert.ToInt32(splitVal[1]);

                Logger.LogEvent("ValidateCookie", LogType.Info, $"Converted account id: {accountId}");

                // Ensure that account id exists
                var currAccount = GetAccountById(accountId);
                if (currAccount == null)
                {
                    Logger.LogEvent("ValidateCookie", LogType.Info, "Account id not found in AOS. Returning errormessage: Error (6)");
                    resp.IsOK = false;
                    resp.ErrorMessage = "Error (6)";
                    return resp;
                }
            }
            catch (Exception ex)
            {
                Logger.LogEvent("ValidateCookie", LogType.Info, $"Exception: {ex.Message}. Return errormessage: Error (7)");

                resp.IsOK = false;
                resp.ErrorMessage = "Error (7)";
                return resp;
            }

            Logger.LogEvent("ValidateCookie", LogType.Info, $"Validation of cookie type {cookieType} completed. No errors");

            return resp;
        }

        public CookieStatusResponse CookieCheck()
        {
            Logger.LogEvent("CookieCheck", LogType.Info, "Validate AOSCookie and IFrameCookie...");

            var resp = new CookieStatusResponse();

            HttpCookie aosCookie;
            HttpCookie iframeCookie;

            int aosAccountId = 0;
            int iFrameAccountId = 0;

            // Validate AOS cookie
            var validateAOSCookieResp = ValidateCookie(CookieType.AOSCookie, out aosCookie, out aosAccountId);
            if (!validateAOSCookieResp.IsOK)
            {
                Logger.LogEvent("CookieCheck", LogType.Info, $"Validation of AOSCookie failed. Return errormessage: Not authenticated (subcode: {validateAOSCookieResp.ErrorMessage})");
                resp.IsOK = false;
                resp.ErrorMessage = $"Not authenticated (subcode: {validateAOSCookieResp.ErrorMessage})";
                return resp;
            }

            // Validate IFrame cookie
            var validateIFrameCookieResp = ValidateCookie(CookieType.IFrameCookie, out iframeCookie, out iFrameAccountId);
            if (!validateIFrameCookieResp.IsOK)
            {
                Logger.LogEvent("CookieCheck", LogType.Info, $"Validation of IFrameCookie failed. Return errormessage: Not authenticated (subcode: {validateIFrameCookieResp.ErrorMessage})");
                resp.IsOK = false;
                resp.ErrorMessage = $"Not authenticated (subcode: {validateIFrameCookieResp.ErrorMessage})";
                return resp;
            }

            //// Compare account id's
            //if (aosAccountId != iFrameAccountId)
            //{
            //    Logger.LogEvent("CookieCheck", LogType.Info, $"aosAccountId ({aosAccountId}) and iFrameAccountId ({iFrameAccountId}) does not match. Return errormessage: Not authenticated (0)");
            //    resp.IsOK = false;
            //    resp.ErrorMessage = $"Not authenticated (0)";
            //    return resp;
            //}

            Logger.LogEvent("CookieCheck", LogType.Info, $"Cookie check completed. No errors");
            return resp;
        }

    }

    public enum AuthenticationStatus
    {
        InvalidURL,
        NotAuthenticated,
        Authenticated
    }

    public enum LicenseStatusType
    {
        OK,
        Expiring,
        Expired,
        Error
    }

    public enum UserLicenseStatusType
    {
        OK,
        NoUserLicense,
        UserPoolSizeLimitReached,
        Error
    }

    public class CookieStatusResponse
    {
        public bool IsOK { get; set; }
        public string ErrorMessage { get; set; }

        public CookieStatusResponse()
        {
            IsOK = true;
            ErrorMessage = string.Empty;
        }
    }

    public class AppLicenseStatus
    {
        public LicenseStatusType LicenseStatus { get; set; }
        public string Message { get; set; }
    }

    public class AppUserLicenseStatus
    {
        public UserLicenseStatusType LicenseStatus { get; set; }
        public string Message { get; set; }
    }

    public class ValidationResponse
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }

        public ValidationResponse()
        {
            IsValid = true;
            ErrorMessage = string.Empty;
        }
    }

    //public class SOAuthenticationResponse
    //{
    //    public bool IsOK { get; set; }
    //    public string ErrorMessage { get; set; }
    //    public SuperOffice82AuthenticationHttps AuthHttps { get; set; }
    //    public SuperOffice82AuthenticationHttp AuthHttp { get; set; }
    //    //public SuperOfficeWebServiceRepositoryOnline RepositorySOOnline { get; set; }
    //    //public SuperOfficeWebServiceRepositoryOnPremise RepositoryOnPremise { get; set; }
    //    public bool IsHttps { get; set; }
    //    public bool IsSOOnline { get; set; }

    //    public SOAuthenticationResponse()
    //    {
    //        IsOK = true;
    //        ErrorMessage = string.Empty;
    //    }
    //}

    public enum CookieType
    {
        AOSCookie,
        IFrameCookie
    }
}