using AOS.DomainModel;
using AOS.GenericLibraries;
using AOS.WebAPIPlugins;
using System.Collections.Generic;
using System.Linq;

namespace AOS.BusinessLayer
{
    public enum AppLicenseReason
    {
        OKTrialVersion,
        OKFullVersion,
        WarnTrialVersionExpiring,
        WarnFullVersionExpiring,
        NoLicenseAccountHasNoTrialOrFullVersion,
        NoLicenseTrialVersionExpired,
        NoLicenseFullVersionExpired,
        NoLicenseInternalError
    }

    public enum UserLicenseReason
    {
        OKUserHaveSeat,
        OKSeatCreatedForUser,
        OKAppDoesNotHaveUserLicensing,
        NoLicenseSeatLimitExceeded,
        NoLicenseAutoAllocationDisabled,
        NoLicenseInternalError,
        NoLicenseSeeAppLicenseReason
    }

    public class LicenseStatusResponse
    {
        public bool IsOK { get; set; }
        public string ErrorMsg { get; set; }
        public bool HasLicense { get; set; }
        public string ResponseText { get; set; }
        public AppLicenseReason AppLicenseReason{ get; set; }
        public UserLicenseReason UserLicenseReason { get; set; }
        public Connection CurrentConnection { get; set; }
        public Account CurrentAccount { get; set; }
        public App CurrentApp { get; set; }
        public AccountApp CurrentAccountApp { get; set; }
        public SoEmployee CurrentPerson { get; set; }
        public List<ContactAssociate> ListOfSOAssociates { get; set; }
        public List<AccountAppAssociate> ListOfExistingUserLicenses { get; set; }

        public LicenseStatusResponse()
        {
            IsOK = true;
            ErrorMsg = string.Empty;
            HasLicense = false;
            ResponseText = string.Empty;
            AppLicenseReason = AppLicenseReason.NoLicenseInternalError;
            UserLicenseReason = UserLicenseReason.NoLicenseInternalError;
        }
    }

    public partial class BusinessLayer
    {
        public LicenseStatusResponse CheckLicense(string custId, int associateId, string appCode, bool checkUserLicensing, List<ContactAssociate> listOfAssociates)
        {
            var resp = new LicenseStatusResponse();

            // Get Account record by cust id (URL)
            var currAccount = GetAccountByURLThin(custId);
            if (currAccount == null)
            {
                resp.IsOK = false;
                resp.ErrorMsg = "Account record not found for CustId: " + custId;
                return resp;
            }

            // Account must have a connection
            if (currAccount.ConnectionID == null)
            {
                resp.IsOK = false;
                resp.ErrorMsg = "Account record with CustId: " + custId + " does not have a connection";
                return resp;
            }

            // Get connection
            var currConnection = GetConnectionById(currAccount.ConnectionID);
            if (currConnection == null)
            {
                resp.IsOK = false;
                resp.ErrorMsg = "Connection record not found for ConnectionId: " + currAccount.ConnectionID;
                return resp;
            }

            resp.CurrentConnection = currConnection;
            resp.CurrentAccount = currAccount;

            // Get App record
            var currApp = GetAppByCode(appCode);
            if (currApp == null)
            {
                resp.IsOK = false;
                resp.ErrorMsg = "App record not found for AppCode: " + appCode;
                return resp;
            }

            resp.CurrentApp = currApp;

            // Get AccountApp record
            var currAccountApp = GetAccountAppByAccountIdAndAppId(currAccount.AccountID, currApp.AppID);
            if (currAccountApp == null)
            {
                resp.AppLicenseReason = AppLicenseReason.NoLicenseAccountHasNoTrialOrFullVersion;
                resp.UserLicenseReason = UserLicenseReason.NoLicenseSeeAppLicenseReason;
                resp.ResponseText = "No app license. Reason: No trial or full version of the app has been purchased";
                return resp;
            }

            resp.CurrentAccountApp = currAccountApp;

            // Check Activated and installed flags
            if (!currAccountApp.Activated)
            {
                resp.IsOK = false;
                resp.ErrorMsg = "App with AppCode: " + appCode + " has been deactivated";
                return resp;
            }

            if (!currAccountApp.Installed)
            {
                resp.IsOK = false;
                resp.ErrorMsg = "App with AppCode: " + appCode + " is not installed";
                return resp;
            }

            // Check app license
            var appLicenseStatus = CheckAppLicense(currAccount.AccountID, currApp.AppID);
            if (appLicenseStatus.LicenseStatus == LicenseStatusType.Error)
            {
                resp.IsOK = false;
                resp.ErrorMsg = string.Format("No license info found for app with AppCode: {0}. Internal error message: {1}", appCode, appLicenseStatus.Message);
                return resp;
            }

            if (appLicenseStatus.LicenseStatus == LicenseStatusType.Expired)
            {
                resp.AppLicenseReason = (currAccountApp.IsTrial) ? AppLicenseReason.NoLicenseTrialVersionExpired : AppLicenseReason.NoLicenseFullVersionExpired;
                resp.UserLicenseReason = UserLicenseReason.NoLicenseSeeAppLicenseReason;
                resp.ResponseText = string.Format("No app license. Reason: License expired at {0}", currAccountApp.AppExpires);
                return resp;
            }

            // Check if we should check for userlicensing
            if (!checkUserLicensing)
            {
                resp.HasLicense = true;
                resp.AppLicenseReason = (currAccountApp.IsTrial) ? AppLicenseReason.OKTrialVersion : AppLicenseReason.OKFullVersion;
                resp.UserLicenseReason = UserLicenseReason.OKAppDoesNotHaveUserLicensing;
                return resp;

            }

            var hasUserLicensing = (currApp.UserPoolCount > 0);

            // If app doesn't have user licensing, he now has a valid license
            if (!hasUserLicensing)
            {
                resp.HasLicense = true;
                resp.AppLicenseReason = (currAccountApp.IsTrial) ? AppLicenseReason.WarnTrialVersionExpiring : AppLicenseReason.WarnFullVersionExpiring;
                resp.UserLicenseReason = UserLicenseReason.OKAppDoesNotHaveUserLicensing;
                resp.ResponseText = string.Format("App license OK. License expires at {0}", currAccountApp.AppExpires);
                return resp;
            }

            // App has user licensing. Get existing user licenses
            var currUserLicenses = GetAccountAppAssociatesByAccountIdAndAppId(currAccount.AccountID, currApp.AppID);
            resp.ListOfExistingUserLicenses = currUserLicenses.ToList();

            resp.ListOfSOAssociates = listOfAssociates;

            // Check if associate has a user license seat
            var currAccountAppAssociate = GetAccountAppAssociatesByAccountIdAppIdAndAssociateId(currAccount.AccountID, currApp.AppID, associateId);
            var associateIsInList = (currAccountAppAssociate == null) ? false : true;

            if (associateIsInList)
            {
                resp.HasLicense = true;
                resp.AppLicenseReason = (currAccountApp.IsTrial) ? AppLicenseReason.OKTrialVersion : AppLicenseReason.OKFullVersion;
                resp.UserLicenseReason = UserLicenseReason.OKUserHaveSeat;
                resp.ResponseText = string.Format("App license OK. License expires at {0}", currAccountApp.AppExpires);
                return resp;
            }

            // User does not have a user license seat. Check if auto allocation has been switched on
            if (!currAccountApp.AutoUserLicenseAllocation)
            {
                resp.AppLicenseReason = (currAccountApp.IsTrial) ? AppLicenseReason.OKTrialVersion : AppLicenseReason.OKFullVersion;
                resp.UserLicenseReason = UserLicenseReason.NoLicenseAutoAllocationDisabled;
                resp.ResponseText = "No user license. Reason: User does not have a user license seat, and auto seat allocation is switched off";
                return resp;
            }

            // Check if there are any available seats left
            var existingLicensesCount = GetAccountAppAssociatesByAccountIdAndAppId(currAccount.AccountID, currApp.AppID).Count;

            if ((existingLicensesCount + 1) > currApp.UserPoolCount)
            {
                resp.AppLicenseReason = (currAccountApp.IsTrial) ? AppLicenseReason.OKTrialVersion : AppLicenseReason.OKFullVersion;
                resp.UserLicenseReason = UserLicenseReason.NoLicenseSeatLimitExceeded;
                resp.ResponseText = "No user license. Reason: User license seat limit reached. Limit: " + currApp.UserPoolCount;
                return resp;
            }

            // Create associate among registered user licenses
            var newUserLicense = new AccountAppAssociate()
            {
                AccountID = currAccount.AccountID,
                AppID = currApp.AppID,
                AssociateID = associateId
            };

            AddAccountAppAssociate(newUserLicense);

            resp.HasLicense = true;
            resp.AppLicenseReason = (currAccountApp.IsTrial) ? AppLicenseReason.OKTrialVersion : AppLicenseReason.OKFullVersion;
            resp.UserLicenseReason = UserLicenseReason.OKSeatCreatedForUser;
            resp.ResponseText = "User license OK. Reason: User has been been given a seat among registered user licenses";

            return resp;
        }
    }
}
