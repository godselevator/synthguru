using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AOS.DomainModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using AOS.Platform.Common;
using System.Xml.Serialization;

namespace AOS.Platform.Models
{
    public class Pager
    {
        public int TotalItems { get; private set; }
        public int CurrentPage { get; private set; }
        public int PageSize { get; private set; }
        public int TotalPages { get; private set; }
        public int StartPage { get; private set; }
        public int EndPage { get; private set; }

        public Pager(int totalItems, int? page, int pageSize = 10)
        {
            // calculate total, start and end pages
            var totalPages = (int)Math.Ceiling((decimal)totalItems / (decimal)pageSize);
            var currentPage = page != null ? (int)page : 1;
            var startPage = currentPage - 5;
            var endPage = currentPage + 4;

            if (startPage <= 0)
            {
                endPage -= (startPage - 1);
                startPage = 1;
            }

            if (endPage > totalPages)
            {
                endPage = totalPages;

                if (endPage > 10)
                    startPage = endPage - 9;
            }

            TotalItems = totalItems;
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalPages = totalPages;
            StartPage = startPage;
            EndPage = endPage;
        }
    }

    public class PagerModel
    {
        public IEnumerable<Account> Accounts { get; set; }
        public Pager Pager { get; set; }
    }

    public class CertificateFileInfo
    {
        public byte[] Certificate { get; set; }
        public string CertificateName { get; set; }
        public int CertificateSize { get; set; }
    }

    public class AppPKeyInfo
    {
        public byte[] PKey { get; set; }
        public string PKeyName { get; set; }
        public int PKeySize { get; set; }
    }

    public class AppIconInfo
    {
        public byte[] AppLogoIcon { get; set; }
        public string AppLogoName { get; set; }
        public int AppLogoSize { get; set; }
        public byte[] TrialLogoIcon { get; set; }
        public string TrialLogoName { get; set; }
        public int TrialLogoSize { get; set; }
    }

    public class AppIconInfoStr
    {
        public string AppLogoIcon { get; set; }
        public string AppLogoName { get; set; }
        public string AppLogoSize { get; set; }
        public string TrialLogoIcon { get; set; }
        public string TrialLogoName { get; set; }
        public string TrialLogoSize { get; set; }
    }

    public class AppAdminGlobalViewModel
    {
        public int AppId { get; set; }
        public AppAlt[] ListOfApps { get; set; }
        public AppAdminGeneralInfoViewModel GeneralInfo { get; set; }
        public AppAdminMaintenanceViewModel Maintenance { get; set; }
        public AppAdminIconsViewModel Icons { get; set; }
        public AppAdminAvailabilityViewModel Availability { get; set; }
        public AppAdminURLInfoViewModel URLInfo { get; set; }
        public AppAdminDescrTextViewModel DescrText { get; set; }
        public AppAdminAdvancedViewModel Advanced { get; set; }
    }

    public class AppAdminNewApp
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Version { get; set; }
    }

    public class AppAdminGeneralInfoViewModel
    {
        public int AppId { get; set; }
        public string AppName { get; set; }
        public bool AppEnabled { get; set; }
        [Range(0, 100, ErrorMessage = "Value must be between 0 and 100")]
        public int? TrialDays { get; set; }
        public int UserPoolCount { get; set; }
        public bool SendExpireMails { get; set; }
    }

    public class AppAdminMaintenanceViewModel
    {
        public int AppId { get; set; }
        public bool MaintenanceEnabled { get; set; }
        public string MaintenanceText { get; set; }
        public bool MaintenancePlannedEnabled { get; set; }
        public string MaintenancePlannedText { get; set; }
    }

    public class AppAdminIconsViewModel
    {
        public int AppId { get; set; }
        public string AppLogoIcon { get; set; }
        public string AppLogoName { get; set; }
        public string AppLogoSize { get; set; }
        public string TrialLogoIcon { get; set; }
        public string TrialLogoName { get; set; }
        public string TrialLogoSize { get; set; }
    }

    public class AppAdminAvailabilityViewModel
    {
        public int AppId { get; set; }
        public AppStateAlt[] ListOfAppStates { get; set; }
        public int AppStateOnPremise { get; set; }
        public int AppStateSOOnline { get; set; }
    }

    public class AppAdminURLInfoViewModel
    {
        public int AppId { get; set; }
        public string AdminURL { get; set; }
        public string StartURL { get; set; }
        public string AlternateRedirectURL { get; set; }
    }

    public class AppAdminDescrTextViewModel
    {
        public int AppId { get; set; }
        [AllowHtml]
        public string AppAboutText { get; set; }
    }

    public class AppAdminAdvancedViewModel
    {
        public int AppId { get; set; }
        public string SOAppId { get; set; }
        public string ApplicationToken { get; set; }
        public int SelectedEnvironment { get; set; }
        public GlobalsAlt[] ListOfGlobals { get; set; }

        public string PrivateKeyFileNameSOD { get; set; }
        public int PrivateKeyFileSizeSOD { get; set; }
        [Required, FileExtensions(Extensions = "xml", ErrorMessage = "Specify an XML file containing the private key")]
        public HttpPostedFileBase PrivateKeyFileDataSOD { get; set; }
        [AllowHtml]
        public string SODXML { get; set; }

        public string PrivateKeyFileNameSTAGE { get; set; }
        public int PrivateKeyFileSizeSTAGE { get; set; }
        [Required, FileExtensions(Extensions = "xml", ErrorMessage = "Specify an XML file containing the private key")]
        public HttpPostedFileBase PrivateKeyFileDataSTAGE { get; set; }
        [AllowHtml]
        public string STAGEXML { get; set; }

        public string PrivateKeyFileNamePROD { get; set; }
        public int PrivateKeyFileSizePROD { get; set; }
        [Required, FileExtensions(Extensions = "xml", ErrorMessage = "Specify an XML file containing the private key")]
        public HttpPostedFileBase PrivateKeyFileDataPROD { get; set; }
        [AllowHtml]
        public string PRODXML { get; set; }
    }

    // Diagnostics
    public class DiagnosticFilter
    {
        public int FilterId { get; set; }
        public string FilterName { get; set; }
    }

    public class DiagnosticsViewModel
    {
        public IntegrityCheckViewModel IntegrityCheck { get; set; }
        public CheckConnectionViewModel ConnectionStatus { get; set; }
    }

    public class IntegrityCheckViewModel
    {
        public int FilterId { get; set; }
        public List<DiagnosticFilter> ListOfFilters { get; set; }
        public List<OrphanAccountOwner> OrphanAccountOwners { get; set; }
        public List<AccountlessUser> AccountlessUsers { get; set; }
        public List<OrphanURAUser> OrphanURAUsers { get; set; }
        public List<AccountWithNoConnection> AccountWithNoConnections { get; set; }
        public List<AccountWithNoApps> AccountWithNoApps { get; set; }
        public List<OrphanConnection> OrphanConnections { get; set; }
    }

    public class OrphanAccountOwner
    {
        public string OwnerId { get; set; }
        public int AccountId { get; set; }
        public string AccountName { get; set; }
    }

    public class OrphanURAUser
    {
        public string UserId { get; set; }
        public int AccountId { get; set; }
        public string AccountName { get; set; }
        public string Role { get; set; }
    }

    public class AccountlessUser
    {
        public string UserId { get; set; }
        public string FirstNameLastName { get; set; }
        public string Email { get; set; }
    }

    public class AccountWithNoConnection
    {
        public int AccountId { get; set; }
        public string AccountName { get; set; }
    }

    public class AccountWithNoApps
    {
        public int AccountId { get; set; }
        public string AccountName { get; set; }
    }

    public class OrphanConnection
    {
        public int ConnectionId { get; set; }
        public string ConnectionURL { get; set; }
    }

    public class CheckConnectionViewModel
    {
        public int FilterId { get; set; }
        public List<string> ListOfFilters { get; set; }
        public List<AOSCheckConnection> Connections { get; set; }
    }

    public class AOSCheckConnection
    {
        public string StatusIcon { get; set; }
        public string StatusText { get; set; }
        public string ConnectionType { get; set; }
        public int AccountId { get; set; }
        public string AccountName { get; set; }
        public int ConnectionId { get; set; }
        public string ConnectionURL { get; set; }
        public string NetServer { get; set; }
    }

    // Licenses
    public class LicensesViewModel
    {
        public int SelectedAccountId { get; set; }
        public List<LicenseAccount> AccountList { get; set; }
        public int SelectedAppId { get; set; }
        public List<LicenseApp> AppList { get; set; }
        public int SelectedAssociateId { get; set; }
        public List<LicenseAssociate> AssociateList { get; set; }
        public bool AutoAllocation { get; set; }
        public int MaxSeats { get; set; }
        public List<LicenseAssociate> ExistingAssociateList { get; set; }
    }

    public class LicenseAccount
    {
        public int AccountId { get; set; }
        public string AccountName { get; set; }
    }

    public class LicenseApp
    {
        public int AppId { get; set; }
        public string AppName { get; set; }
    }

    public class LicenseAssociate
    {
        public int AssociateId { get; set; }
        public string AssociateName { get; set; }
    }

    public class AccountInfo
    {
        public List<LicenseAssociate> SOAssociateList { get; set; }
        public List<LicenseAssociate> UserLicenseList { get; set; }
        public bool AutoAllocation { get; set; }
        public int MaxSeats { get; set; }
    }

    // Admin Home
    public class AdminHomeGlobalViewModel
    {
        public string DBEnvironment { get; set; }
        public AdminHomeEndpointSettingsViewModel EndpointSettings { get; set; }
        public AdminHomeNotificationSettingsViewModel NotificationSettings { get; set; }
        public AdminHomeSOOnlineSettingsViewModel SOOnlineSettings { get; set; }
        public AdminHomeAOSWebAPISettingsViewModel AOSWebAPISettings { get; set; }
    }

    public class AdminHomeEndpointSettingsViewModel
    {
        [Display(Name = "Update endpoint in cockpit")]
        public bool UpdateEndpoint { get; set; }

        [Display(Name = "Polling enabled")]
        public bool PollingEnabled { get; set; }

        [Range(0, 30000)]
        [Display(Name = "Polling interval (ms)")]
        public int PollingInterval { get; set; }
    }

    public class AdminHomeNotificationSettingsViewModel
    {
        public NotificationAndText[] NotificationList { get; set; }

        [Required]
        [Display(Name = "Notification email")]
        [EmailAddress]
        public string NotificationEmail { get; set; }
    }

    public class AdminHomeSOOnlineSettingsViewModel
    {
        public GlobalsAlt[] ListOfGlobals { get; set; }
        public int GlobalsId { get; set; }
        public string AdwizaCertificate { get; set; }
        public string FederationGateway { get; set; }
        public string SystemTokenCertificatePath { get; set; }
        public string SuperIdCertificate { get; set; }

        public string SOCertificateFileName { get; set; }
        public int SOCertificateFileSize { get; set; }
        [Required, FileExtensions(Extensions = "crt", ErrorMessage = "Specify a SuperOfficeFederatedLogin.crt file")]
        public HttpPostedFileBase SOCertificateFileData { get; set; }
        [AllowHtml]
        public string SOCertificateContents { get; set; }

        public string CreateUserRedirectURL { get; set; }
        [AllowHtml]
        public string AOSURL { get; set; }
        public string Environment { get; set; }
        public bool MirroringStatus { get; set; }
    }

    public class AdminHomeAOSWebAPISettingsViewModel
    {
        public string AOSWebAPIUser { get; set; }
        public string Password { get; set; }
        public bool AOSWebAPIEnabled { get; set; }
    }

    public class NotificationAndText
    {
        public int NotificationId { get; set; }
        public bool NotificationSendEmail { get; set; }
        public string NotificationText { get; set; }
    }

    public class AppStateAlt
    {
        public int AppStateID { get; set; }
        public string State { get; set; }
    }

    public class AppAlt
    {
        public int AppID { get; set; }
        public string Name { get; set; }
    }

    public class GlobalsAlt
    {
        public int GlobalsID { get; set; }
        public string Name { get; set; }
    }

    public enum SOOnlineEnvironment
    {
        SOD,
        STAGE,
        PROD
    }
}