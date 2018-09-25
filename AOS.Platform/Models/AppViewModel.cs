using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AOS.DomainModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using AOS.Platform.Common;

namespace AOS.Platform.Models
{
    public class AppViewModel
    {
        public App[] Apps { get; set; }
        public bool IsNormalUser { get; set; }
        public bool IsBoughtOrOnTrial { get; set; }
        public int OnPremiseState { get; set; }
        public string OnPremiseText { get; set; }
        public int SOOnlineState { get; set; }
        public string SOOnlineText { get; set; }
        public bool UsesConnectApp { get; set; }
        [DataType(DataType.MultilineText)]
        public string MirroringDisclaimer { get; set; }
    }

    public class AppExtended
    {
        public App MyApp { get; set; }
        public bool Trial { get; set; }
        public string TrialRemaining { get; set; }
        public bool TrialExpired { get; set; }
        public DateTime ExpirationDate { get; set; }
        public bool HasAdminPage { get; set; }
        public DateTime? AppExpires { get; set; }
        public string AppRemaining { get; set; }
        public bool AppExpired { get; set; }
        public bool ShowExpirationInfo { get; set; }
        public string AppLogoIcon { get; set; }
        public bool AppUnderMaintenance { get; set; }
        public string AppUnderMaintenanceText { get; set; }
        public int SOOnlineState { get; set; }
        public int OnPremiseState { get; set; }
        public bool IsConverted { get; set; }
    }

    public class SingleApp
    {
        public int AppID { get; set; }
        public string AppName { get; set; }
        public bool AppEnabled { get; set; }
        [Range(0, 100, ErrorMessage="Value must be between 0 and 100")]
        public int? TrialDays { get; set; }
        public bool SendExpireMails { get; set; }
        public App[] ListOfApps { get; set; }
        public AppIconInfo AppIconInfo { get; set; }
        public AppState[] ListOfAppStates { get; set; }
        public int AppStateOnPremise { get; set; }
        public int AppStateSOOnline { get; set; }
        public string AppAdminURL { get; set; }
        public string AppStartURL { get; set; }
        [AllowHtml]
        public string AppAboutText { get; set; }
        public string SOAppId { get; set; }
        public string ApplicationToken { get; set; }
    }

    public class BuyTryAppViewModel
    {
        public int AppID { get; set; }
        public PurchaseVersion OnPremisePurchaseVersion { get; set; }
        public PurchaseVersion SOOnlinePurchaseVersion { get; set; }
        public string AppName { get; set; }
        public bool IsTrial { get; set; }
        public DateTime TrialStart { get; set; }
        public DateTime TrialExpires { get; set; }
        public int TrialDays { get; set; }
        public string SOStartURL { get; set; }
        public bool UpgradeFromTrial { get; set; }
        public string PurchaseText { get; set; }
        public bool HasMirroring { get; set; }
        [DataType(DataType.MultilineText)]
        public string MirroringDisclaimer { get; set; }
    }

    public class NotifyUserViewModel
    {
        [Required]
        public string FirstNameLastName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public int AppID { get; set; }
        public string AppName { get; set; }
        public bool OnPremiseBetaVersion { get; set; }
    }
}