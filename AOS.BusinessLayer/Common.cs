using AOS.DomainModel;
using AOS.SAMLTokenHandler;
using AOS.WebAPIPlugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOS.BusinessLayer
{
    public partial interface IBusinessLayer
    {
        Account GetActiveAccountForUserId(string userId);
        ValidationResponse ValidateAdminPage(int accountId, string userId, string appCode);
    }

    public partial class BusinessLayer : IBusinessLayer
    {
        public string GetAccountRoleForUser(string userId, int accountId)
        {
            var resp = string.Empty;

            // Get UserRoleAccount record
            var currURA = GetUserRoleAccountByUserIdAndAccountId(userId, accountId);
            if (currURA == null)
                return "No URA record found";

            // Get Role
            var currRole = GetRoleById(currURA.RoleID);
            if (currRole == null)
                return "No Role found";

            return currRole.Name;
        }

        public GenericResponse<AccountInfoThin> GetAccountInfoByAPIKey(string APIKey)
        {
            var resp = new GenericResponse<AccountInfoThin>();
            resp.ReturnObj = new AccountInfoThin();

            try
            {
                // Validate APIKey
                if (string.IsNullOrEmpty(APIKey))
                {
                    resp.IsOK = false;
                    resp.ErrorMsg = "APIKey is not filled";
                    return resp;
                }

                // Get APIKey record
                var currAPiKey = GetAPIKeyByAPIKey(APIKey);
                if (currAPiKey == null)
                {
                    resp.IsOK = false;
                    resp.ErrorMsg = $"APIKey: {APIKey} not found";
                    return resp;
                }

                // Get app record
                var currApp = GetAppById(currAPiKey.AppId);
                if (currApp == null)
                {
                    resp.IsOK = false;
                    resp.ErrorMsg = $"APIKey references an AppId: {currAPiKey.AppId} that does not exist";
                    return resp;
                }

                // Get account record
                var currAccount = GetAccountById(currAPiKey.AccountId);
                if (currAccount == null)
                {
                    resp.IsOK = false;
                    resp.ErrorMsg = $"APIKey references an AccountId: {currAPiKey.AccountId} that does not exist";
                    return resp;
                }

                resp.ReturnObj.AccountId = currAccount.AccountID;
                resp.ReturnObj.CustId = currAccount.URL;
                resp.ReturnObj.MirrorDBName = "MirrorDatabase_" + currAccount.URL;

                return resp;
            }
            catch (Exception ex)
            {
                var errMsg = (ex.InnerException != null) ? ex.InnerException.Message : ex.Message;
                resp.IsOK = false;
                resp.ErrorMsg = "Exception occurred: " + errMsg;

                return resp;
            }
        }

        public ValidationResponse ValidateAdminPage(int accountId, string userId, string appCode)
        {
            var resp = new ValidationResponse();

            // Validate account
            var currAccount = GetAccountById(accountId);
            if (currAccount == null)
            {
                resp.IsValid = false;
                resp.ErrorMessage = string.Format("Account id {0} could not be found", accountId);
                return resp;
            }

            // Validate user
            var currUser = GetUserById(userId);
            if (currUser == null)
            {
                resp.IsValid = false;
                resp.ErrorMessage = string.Format("User id {0} could not be found", userId);
                return resp;
            }

            // Get app by code (not case sensitive)
            var currApp = GetAppByCode(appCode);
            if (currApp == null)
            {
                resp.IsValid = false;
                resp.ErrorMessage = "App entry was not found for app code " + appCode;
                return resp;
            }

            return resp;
        }

        public Account GetActiveAccountForUserId(string userId)
        {
            // Get all UAC's for user
            var uacList = GetUserRoleAccountByUserId(userId);

            var activeUAC = uacList.Where(e => e.ActiveAccount).FirstOrDefault();

            if (activeUAC == null)
                return null;

            // Get account
            return GetAccountById(activeUAC.AccountID);
        }

        public BLResp IsOwnerOfAccount(string userId, int accountId)
        {
            var resp = new BLResp();

            // Validate userid
            if (GetUserById(userId) == null)
            {
                resp.ErrorMsg = "UserId not found";
                resp.ReturnResponse = ReturnResponse.Error;
                return resp;
            }

            // Validate accountid
            var account = GetAccountById(accountId);
            if (account == null)
            {
                resp.ErrorMsg = "AccountId not found";
                resp.ReturnResponse = ReturnResponse.Error;
                return resp;
            }

            // Userid and owner must match
            if (userId != account.Owner)
            {
                resp.ErrorMsg = "User is now account owner";
                resp.ReturnResponse = ReturnResponse.Error;
                return resp;
            }

            return resp;
        }

        public BLResp CanByAppsForAccount(string userId, int accountId)
        {
            var resp = new BLResp();

            // Validate userid
            if (GetUserById(userId) == null)
            {
                resp.ErrorMsg = "UserId not found";
                resp.ReturnResponse = ReturnResponse.Error;
                return resp;
            }

            // Validate accountid
            if (GetAccountById(accountId) == null)
            {
                resp.ErrorMsg = "AccountId not found";
                resp.ReturnResponse = ReturnResponse.Error;
                return resp;
            }

            // Get UAR entry for user/account
            var UAR = GetUserRoleAccountByUserIdAndAccountId(userId, accountId);
            if (UAR == null)
            {
                resp.ErrorMsg = "Could not find association between userId and accountId";
                resp.ReturnResponse = ReturnResponse.Error;
                return resp;
            }

            // Get role entry
            var currRole = GetRoleById(UAR.RoleID);
            if (currRole == null)
            {
                resp.ErrorMsg = "Invalid role. Contact system user";
                resp.ReturnResponse = ReturnResponse.Error;
                return resp;
            }

            var roleName = (currRole.Name.ToLower() == "user") ? IsInRole.User : (currRole.Name.ToLower() == "administrator") ? IsInRole.Administrator : IsInRole.SystemUser;

            if (roleName == IsInRole.User)
            {
                resp.ErrorMsg = string.Format("User '{0}' is not allowed to urchase apps for account '{1}'", userId, accountId);
                resp.ReturnResponse = ReturnResponse.Error;
                return resp;
            }

            return resp;
        }

        public BLResp IsInRoleForAccount(string userId, int accountId, IsInRole isInRole)
        {
            var resp = new BLResp();

            // Validate userid
            if (GetUserById(userId) == null)
            {
                resp.ErrorMsg = "UserId not found";
                resp.ReturnResponse = ReturnResponse.Error;
                return resp;
            }

            // Validate accountid
            if (GetAccountById(accountId) == null)
            {
                resp.ErrorMsg = "AccountId not found";
                resp.ReturnResponse = ReturnResponse.Error;
                return resp;
            }

            // Get UAR entry for user/account
            var UAR = GetUserRoleAccountByUserIdAndAccountId(userId, accountId);
            if (UAR == null)
            {
                resp.ErrorMsg = "Could not find association between userId and accountId";
                resp.ReturnResponse = ReturnResponse.Error;
                return resp;
            }

            // Get role entry
            var currRole = GetRoleById(UAR.RoleID);
            if (currRole == null)
            {
                resp.ErrorMsg = "Invalid role. Contact system user";
                resp.ReturnResponse = ReturnResponse.Error;
                return resp;
            }

            var roleName = (currRole.Name.ToLower() == "user") ? "User" : (currRole.Name.ToLower() == "administrator") ? "Administrator" : "System user";

            if (isInRole == IsInRole.User && roleName == "User")
                return resp;
            else if (isInRole == IsInRole.Administrator && roleName == "Administrator")
                return resp;
            else if (isInRole == IsInRole.SystemUser && roleName == "System user")
                return resp;
            else
            {
                resp.ErrorMsg = string.Format("User '{0}' does not have role '{1}' for account '{2}'", userId, roleName, accountId);
                resp.ReturnResponse = ReturnResponse.Error;
                return resp;
            }
        }
    }

    public class AOSSOOnlineSAMLResponse
    {
        public bool IsOK { get; set; }
        public string ReturnMsg { get; set; }
        public SOOnlineCreateUser CreateUser { get; set; }

        public AOSSOOnlineSAMLResponse()
        {
            IsOK = true;
            ReturnMsg = string.Empty;
        }

    }

    public class AOSResponse
    {
        public bool IsOK { get; set; }
        public string ErrorMsg { get; set; }
        public bool IsDirty { get; set; }
        public bool Redirect { get; set; }
        public string RedirectAction { get; set; }
        public string RedirectController { get; set; }
        public string Parameter { get; set; }
        public string ReturnedJsonData { get; set; }

        public AOSResponse()
        {
            IsOK = true;
            Redirect = false;
            IsDirty = false;
        }

    }

    public class AOSGenericResponse<T>
    {
        public bool IsOK { get; set; }
        public string ErrorMsg { get; set; }
        public T ReturnObj { get; set; }

        public AOSGenericResponse()
        {
            IsOK = true;
            ErrorMsg = string.Empty;
        }

    }

    public class BLResp
    {
        public string ErrorMsg { get; set; }
        public ReturnResponse ReturnResponse { get; set; }

        public BLResp()
        {
            ErrorMsg = string.Empty;
            ReturnResponse = AOS.BusinessLayer.ReturnResponse.Success;
        }
    }

    public enum ReturnResponse
    {
        Success,
        Error
    }

    public enum IsInRole
    {
        User,
        Administrator,
        SystemUser
    }
}
