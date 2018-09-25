using AOS.BusinessLayer;
using AOS.DomainModel;
using AOS.Platform.Models;
using SuperOffice;
using SuperOffice.Configuration;
using SuperOffice.CRM.ArchiveLists;
using SuperOffice.CRM.Services;
using SuperOffice.Security.Principal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace AOS.Platform.Common
{
    public enum EndpointStatus
    {
        NotFound,
        SameAccount,
        Orphan,
        OtherAccount
    }

    public class AccountCommon
    {
        //public static SoSession session;

        public static EndpointStatus EndpointExists(ApplicationUser currentUser, string endpointURI)
        {
            var businessLayer = new BusinessLayer.BusinessLayer(currentUser.Id);
            var currentAccount = GetCurrentAccountForUser(currentUser.Id);

            // Get connection info
            var existConnection = businessLayer.GetConnectionByURL(endpointURI);

            // If connection does not exist, we are ok
            if (existConnection == null)
                return EndpointStatus.NotFound;

            // Get account by connection id
            var testAccount = businessLayer.GetAccountByConnectionId(existConnection.ConnectionID);

            // If no account is found the endpoint is orphan
            if (testAccount == null)
                return EndpointStatus.Orphan;

            // If account was found it must belong to the current account
            if (currentAccount.AccountID == testAccount.AccountID)
                return EndpointStatus.SameAccount;

            // The endpoint belongs to another account
            return EndpointStatus.OtherAccount;
        }

        public static CockpitViewModel GetCockpitViewModel(ApplicationUser currentUser, ApplicationUser currentOwner)
        {
            // TODO: Lav om til agents !!!!!!!!!!!!!
            var model = new CockpitViewModel();

            var businessLayer = new BusinessLayer.BusinessLayer(currentUser.Id);
            var currentAccount = GetCurrentAccountForUser(currentUser.Id);

            model.StatusAsInt = 0;

            model.CurrentUserId = currentUser.Id;
            model.CurrentAccountId = currentAccount.AccountID;
            model.CurrentAccountName = currentAccount.Name;
            model.CurrentAccountOwnerName = currentOwner.FirstNameLastName;
            model.CurrentAccountOwnerEmail = currentOwner.Email;

            var currentAccountId = 0;
            var currentRoleId = string.Empty;

            model.Accounts = new List<string>();
            model.AccountsList = new List<Account>();

            var userRoleAccounts = businessLayer.GetUserRoleAccountByUserId(currentUser.Id);

            // Add accounts to list
            foreach (var uac in userRoleAccounts)
            {
                var acc = businessLayer.GetAccountById(uac.AccountID);
                model.Accounts.Add(HttpUtility.HtmlEncode(acc.Name));
                model.AccountsList.Add(acc);

                if (uac.ActiveAccount)
                {
                    currentAccountId = uac.AccountID;
                    currentRoleId = uac.RoleID;
                }
            }

            foreach (var item in model.AccountsList)
            {
                item.Name = HttpUtility.HtmlDecode(item.Name);
            }

            model.AccountsList = model.AccountsList.OrderBy(a => a.Name).ToList();

            // Get current role name
            var currentRole = businessLayer.GetRoleById(currentRoleId);
            model.CurrentAccountRole = currentRole.Name;

            // Check if an endpoint has been defined
            if (currentAccount.ConnectionID == null)
            {
                model.CurrentConnectionStatus = "No endpoint defined";
                model.CurrentConnectionStatusIcon = "blink_me red fa fa-ban";

                return model;
            }

            // Get endpoint info
            var endpoint = businessLayer.GetConnectionById(currentAccount.ConnectionID);

            // Get endpoint polling info
            var endpointPollingInfo = businessLayer.GetAllGlobals();

            model.EndpointPollingEnabled = endpointPollingInfo[0].EndpointPollingEnabled;
            model.EndpointPollingInterval = endpointPollingInfo[0].EndpointPollingInterval;

            model.IsSOOnline = endpoint.IsSOOnline;

            // Get globals and check if we should test endpoint connection
            //var globals = businessLayer.GetGlobalsById(1);

            //if (!(bool)globals.TestEndpoint)
            //{
            //    model.CurrentConnectionStatus = "Up and running";
            //    model.CurrentConnectionStatusIcon = "green fa fa-thumbs-up";
            //    model.StatusAsInt = 1;

            //    return model;
            //}

            // External check of endpoint of SO Online
            if (model.IsSOOnline)
            {
                var resp = PlatformCommon.TestSOEndpoint(endpoint);
                if (resp.IsOK)
                {
                    model.CurrentConnectionStatus = "Up and running";
                    model.CurrentConnectionStatusIcon = "green fa fa-thumbs-up";
                    model.StatusAsInt = 1;
                }
                else
                {
                    model.CurrentConnectionStatus = resp.ErrorMsg;
                    model.CurrentConnectionStatusIcon = "blink_me red fa fa-exclamation-triangle";
                }

                return model;
            }

            // For On Premise endpoints we check endpoint connectivity
            try
            {
                var myRequest = (HttpWebRequest)WebRequest.Create(endpoint.URL + "SoPrincipal.svc");

                var response = (HttpWebResponse)myRequest.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    //  it's at least in some way responsive
                    //  but may be internally broken
                    //  as you could find out if you called one of the methods for real
                    model.CurrentConnectionStatus = "Up and running";
                    model.CurrentConnectionStatusIcon = "green fa fa-thumbs-up";
                    model.StatusAsInt = 1;
                }
                else
                {
                    //  well, at least it returned...
                    model.CurrentConnectionStatus = "Up and running";
                    model.CurrentConnectionStatusIcon = "green fa fa-thumbs-up";
                    model.StatusAsInt = 1;
                }
            }
            catch (Exception ex)
            {
                model.CurrentConnectionStatus = "Endpoint cannot be reached";
                model.CurrentConnectionStatusIcon = "blink_me red fa fa-exclamation-triangle";

                return model;
            }

            // On Premise connection
            try
            {
                var soUtils = new SOUtils(endpoint.SysUser, endpoint.SysPassword, endpoint.URL);

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
                        model.CurrentConnectionStatus = "Authentication failed";
                        model.CurrentConnectionStatusIcon = "blink_me red fa fa-exclamation-triangle";

                        return model;
                    }
                }

                model.CurrentConnectionStatus = "Up and running";
                model.CurrentConnectionStatusIcon = "green fa fa-thumbs-up";
                model.StatusAsInt = 1;

                // Check if we can read documents
                var docText = string.Empty;

                if (!CanReadDocuments(ref docText))
                {
                    model.CurrentConnectionStatus = "Up and running (documents not enabled)";
                    model.CurrentConnectionStatusIcon = "green fa fa-thumbs-up";
                    model.StatusAsInt = 1;
                }

                return model;
            }
            catch (Exception ex)
            {
                model.CurrentConnectionStatus = "Authentication failed";
                model.CurrentConnectionStatusIcon = "blink_me red fa fa-exclamation-triangle";

                return model;
            }

            //var soServiceLayer = businessLayer.SetSOProperties(endpoint.SysUser, endpoint.SysPassword, endpoint.URL);

            //if (soServiceLayer.ReturnValue == "unknown" || soServiceLayer.ReturnValue.ToLower().Contains("exception"))
            //{
            //    model.CurrentConnectionStatus = "No servicelayer (" + soServiceLayer.ReturnValue + ")";
            //    model.CurrentConnectionStatusIcon = "blink_me red fa fa-exclamation-triangle";

            //    return model;
            //}

            ////var endpointReturn = businessLayer.SOGetInfo(endpoint.URL, endpoint.SysUser, endpoint.SysPassword);

            //if (soServiceLayer.ReturnValue == "" || soServiceLayer.ReturnValue == "Ok")
            //{
            //    model.CurrentConnectionStatus = "Up and running";
            //    model.CurrentConnectionStatusIcon = "green fa fa-thumbs-up";
            //    model.StatusAsInt = 1;
            //}
            //else
            //{
            //    model.CurrentConnectionStatus = "No connection (" + soServiceLayer.ReturnValue + ")";
            //    model.CurrentConnectionStatusIcon = "blink_me red fa fa-exclamation-triangle";

            //    return model;
            //}

            // Check handshake data
            if (endpoint.PrefDescId == null)
            {
                model.CurrentConnectionStatus = "Endpoint handshake data key not present";
                model.CurrentConnectionStatusIcon = "blink_me red fa fa-exclamation-triangle";
                return model;
            }

            var prefExists = businessLayer.SOGetAccountIdInPreference(endpoint.PrefDescId);

            if (prefExists.ReturnValue == 0) // No value found
            {
                model.CurrentConnectionStatus = "Endpoint handshake data not present";
                model.CurrentConnectionStatusIcon = "blink_me red fa fa-exclamation-triangle";
            }

            if (prefExists.ReturnValue < 0) // Value found not numeric (invalid value)
            {
                model.CurrentConnectionStatus = "Endpoint handshake data invalid";
                model.CurrentConnectionStatusIcon = "blink_me red fa fa-exclamation-triangle";
            }

            if (prefExists.ReturnValue > 0) // Account Id found. Verify
            {
                if (currentAccount.AccountID != prefExists.ReturnValue)
                {
                    model.CurrentConnectionStatus = "Endpoint belongs to another account";
                    model.CurrentConnectionStatusIcon = "blink_me red fa fa-exclamation-triangle";
                }
            }

            return model;
        }

        public static bool CanReadDocuments(ref string ErrorText)
        {
            ErrorText = "Not Testet";

            try
            {
                using (DocumentAgent da = new DocumentAgent())
                {
                    var documentEntity = da.CreateDefaultDocumentEntity();
                    documentEntity.Name = "aostest.txt";

                    // Create byte array
                    var defaultByteArray = new byte[] { 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20 };

                    documentEntity = da.SaveDocumentEntity(documentEntity);

                    documentEntity = da.SetDocumentStream(documentEntity, defaultByteArray.ToStream(), true);

                    documentEntity = da.SaveDocumentEntity(documentEntity);

                    var returnInfo = da.DeletePhysicalDocument(documentEntity.DocumentId, new string[] { });

                    da.DeleteDocumentEntity(documentEntity.DocumentId);
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrorText = (ex.InnerException != null) ? ex.InnerException.Message : ex.Message;
                return false;
            }
        }

        public static AOSCheckConnection CheckConnection(Connection conn)
        {
            var resp = new AOSCheckConnection();

            var businessLayer = new BusinessLayer.BusinessLayer();

            try
            {
                if (conn == null)
                    return resp;

                resp.ConnectionId = conn.ConnectionID;
                resp.ConnectionURL = conn.URL;
                resp.NetServer = conn.NetServer;

                // Get account
                var account = businessLayer.GetAccountByConnectionId(conn.ConnectionID);
                if (account == null)
                {
                    resp.AccountId = 0;
                    resp.AccountName = "n/a";
                    resp.StatusText = "No account found. Connection is orphan!!";
                    resp.StatusIcon = "blink_me red fa fa-ban";
                    return resp;
                }

                resp.AccountId = account.AccountID;
                resp.AccountName = account.Name;
                resp.ConnectionType = (conn.IsSOOnline) ? "SO Online" : "On Premise";
                resp.StatusIcon = "green fa fa-thumbs-up";

                // Get connection status
                if (conn.IsSOOnline)
                {
                    var testResp = PlatformCommon.TestSOEndpoint(conn);
                    if (testResp.IsOK)
                        resp.StatusText = (testResp.ErrorMsg == "") ? "Up and running" : testResp.ErrorMsg;
                    else
                    {
                        resp.StatusText = testResp.ErrorMsg;
                        resp.StatusIcon = "blink_me red fa fa-ban";
                    }

                    return resp;
                }
                else // On Premise connection
                {
                    // Verify endpoint towards SO
                    if (conn.URL.ToLower().Contains("services75"))
                    {
                        var soServiceLayer = businessLayer.SetSOProperties(conn.SysUser, conn.SysPassword, conn.URL);

                        // Check if endpoint could reached at all
                        if (!soServiceLayer.IsOK)
                        {
                            resp.StatusText = soServiceLayer.ReturnValue;
                            resp.StatusIcon = "blink_me red fa fa-ban";
                            return resp;
                        }

                        // Check handshake data
                        if (conn.PrefDescId == 0)
                        {
                            resp.StatusText = "Endpoint handshake data key not present";
                            resp.StatusIcon = "blink_me red fa fa-ban";
                            return resp;
                        }

                        var prefExists = businessLayer.SOGetAccountIdInPreference(conn.PrefDescId);
                        if (prefExists.ReturnValue == 0) // No value found
                        {
                            resp.StatusText = "Endpoint handshake data not present";
                            resp.StatusIcon = "blink_me red fa fa-ban";
                            return resp;
                        }

                        if (prefExists.ReturnValue < 0) // Value found not numeric (invalid value)
                        {
                            resp.StatusText = "Endpoint handshake data invalid";
                            resp.StatusIcon = "blink_me red fa fa-ban";
                            return resp;
                        }

                        if (prefExists.ReturnValue > 0) // Account Id found. Verify
                        {
                            if (account.AccountID != prefExists.ReturnValue)
                            {
                                resp.StatusText = "Endpoint belongs to another account";
                                resp.StatusIcon = "blink_me red fa fa-ban";
                                return resp;
                            }
                        }

                        if (soServiceLayer.ReturnValue == "unknown" || soServiceLayer.ReturnValue.ToLower().Contains("exception"))
                        {
                            resp.StatusText = "No servicelayer (" + soServiceLayer.ReturnValue + ")";
                            resp.StatusIcon = "blink_me red fa fa-ban";
                            return resp;
                        }

                        if (soServiceLayer.ReturnValue == "" || soServiceLayer.ReturnValue == "Ok")
                        {
                            resp.StatusText = "Up and running";
                            return resp;
                        }

                        resp.StatusText = "No connection (" + soServiceLayer.ReturnValue + ")";
                        resp.StatusIcon = "blink_me red fa fa-ban";
                    }
                    else
                    {
                        // Attempt to login to SO
                        var soUtils = new SOUtils(conn.SysUser, conn.SysPassword, conn.URL);

                        // Check if we are logged in to SO
                        if (SoContext.IsAuthenticated)
                        {
                            var loggedInCustId = SoContext.CurrentPrincipal.DatabaseContextIdentifier.ToLower();

                            // If we are logging in with a nother context identifer, we must log out first
                            if (loggedInCustId != account.URL.ToLower())
                                SoContext.CloseCurrentSession();
                        }

                        // Log in if we do not have a session
                        if (!SoContext.IsAuthenticated)
                        {
                            var doLoginResp = soUtils.LogInToSO();
                            if (!doLoginResp.IsOK)
                            {
                                resp.StatusText = "No connection";
                                resp.StatusIcon = "blink_me red fa fa-ban";
                                return resp;
                            }
                        }

                        // Check handshake data
                        if (conn.PrefDescId == 0)
                        {
                            resp.StatusText = "Endpoint handshake data key not present";
                            resp.StatusIcon = "blink_me red fa fa-ban";
                            return resp;
                        }

                        var prefExists = soUtils.GetAccountIdInPreference(conn.PrefDescId);
                        if (prefExists == 0) // No value found
                        {
                            resp.StatusText = "Endpoint handshake data not present";
                            resp.StatusIcon = "blink_me red fa fa-ban";
                            return resp;
                        }

                        if (prefExists < 0) // Value found not numeric (invalid value)
                        {
                            resp.StatusText = "Endpoint handshake data invalid";
                            resp.StatusIcon = "blink_me red fa fa-ban";
                            return resp;
                        }

                        if (prefExists > 0) // Account Id found. Verify
                        {
                            if (account.AccountID != prefExists)
                            {
                                resp.StatusText = "Endpoint belongs to another account";
                                resp.StatusIcon = "blink_me red fa fa-ban";
                                return resp;
                            }
                        }

                        resp.StatusText = "Up and running";
                    }
                }

            }
            catch (Exception ex)
            {
            }

            return resp;
        }

        public static List<string> SearchSelections(ApplicationUser currentUser, string searchStr)
        {
            var returnVal = new List<string>();

            var businessLayer = new BusinessLayer.BusinessLayer(currentUser.Id);
            var currentAccount = GetCurrentAccountForUser(currentUser.Id);
            var endpoint = businessLayer.GetConnectionById(currentAccount.ConnectionID);

            if (endpoint.URL.ToLower().Contains("services75"))
            {
                businessLayer.SetSOProperties(endpoint.SysUser, endpoint.SysPassword, endpoint.URL);

                var foundSelections = businessLayer.SOFindSelections(searchStr).ReturnValue;

                foreach (var item in foundSelections)
                {
                    returnVal.Add(item.Name);
                }

            }
            else
            {
                // Attempt to login to SO
                var soUtils = new SOUtils(endpoint.SysUser, endpoint.SysPassword, endpoint.URL);

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
                        returnVal.Add($"Could not connect to SO: {doLoginResp.ErrorMsg}");
                        return returnVal;
                    }
                }

                var foundSelections = soUtils.FindSelections(searchStr);

                foreach (var item in foundSelections)
                {
                    returnVal.Add(item.Name);
                }

            }

            return returnVal;
        }

        public static List<string> GetAllAccounts(string searchStr)
        {
            var returnVal = new List<string>();

            var businessLayer = new BusinessLayer.BusinessLayer();
            var foundAccounts = businessLayer.GetAccountListByName(searchStr);
            var listOfAccounts = new List<string>();

            foreach (var account in foundAccounts)
            {
                returnVal.Add(account.Name);
            }

            return returnVal;
        }

        public static List<string> GetAllCountries(string searchStr)
        {
            var returnVal = new List<string>();

            var businessLayer = new BusinessLayer.BusinessLayer();
            var foundCountrys = businessLayer.GetCountryListByName(searchStr);
            var listOfCountrys = new List<string>();

            foreach (var country in foundCountrys)
            {
                returnVal.Add(country.Name);
            }

            return returnVal;
        }

        public static bool IsSystemUser(ApplicationUser currentUser)
        {
            var resp = false;

            if (currentUser == null)
                return resp;

            var businessLayer = new BusinessLayer.BusinessLayer(currentUser.Id);

            // Get all UACs for user
            var uacList = businessLayer.GetUserRoleAccountByUserId(currentUser.Id);
            var systemUserRole = businessLayer.GetRoleByName("system user");

            // Check for role = 'system user' and break if so
            foreach (var uac in uacList)
            {
                if (uac.RoleID == systemUserRole.Id)
                {
                    resp = true;
                    break;
                }
            }

            return resp;
        }

        public static PersonalSettingsViewModel GetPersonalSettingsViewModel(ApplicationUser currentUser)
        {
            var model = new PersonalSettingsViewModel();

            model.UserId = currentUser.Id;
            model.Email = currentUser.Email;
            model.FirstNameLastName = HttpUtility.HtmlDecode(currentUser.FirstNameLastName);
            model.PhoneNumber = currentUser.PhoneNumber;

            return model;
        }

        public static void AssignAccountUser(ApplicationUser currentUser, Account currentAccount, ApplicationUser accountUser, bool newUser)
        {
            var businessLayer = new BusinessLayer.BusinessLayer(currentUser.Id);

            var userRole = businessLayer.GetRoleByName("User");

            var uac = new UserRoleAccount()
            {
                RoleID = userRole.Id,
                UserID = accountUser.Id,
                AccountID = currentAccount.AccountID,
                ActiveAccount = newUser,
                UserEnabled = true
            };

            businessLayer.AddUserRoleAccount(uac);
        }

        public static string RemoveAccountUser(ApplicationUser currentUser, string userId, int accountId)
        {
            var returnVal = "Ok";

            var businessLayer = new BusinessLayer.BusinessLayer(currentUser.Id);
            var uac = businessLayer.GetUserRoleAccountByUserIdAndAccountId(userId, Convert.ToInt32(accountId));

            if (uac != null)
                businessLayer.RemoveUserRoleAccount(uac);

            return returnVal;

        }

        public static string UpdateUserRole(ApplicationUser currentUser, string roleCmd, string userId, string accountName)
        {
            var returnVal = "Ok";

            var businessLayer = new BusinessLayer.BusinessLayer(currentUser.Id);

            // Get account user
            var user = businessLayer.GetUserById(userId);

            // Define role name and get role
            var roleName = (roleCmd.ToLower() == "radiouser") ? "user" : (roleCmd.ToLower() == "radioadmin") ? "administrator" : "system user";
            var role = businessLayer.GetRoleByName(roleName);
            var sysUserRole = businessLayer.GetRoleByName("system user");

            // Get account
            var account = businessLayer.GetAccountByNameThin(accountName);

            // Get current uac for user on current account
            var uac = businessLayer.GetUserRoleAccountByUserIdAndAccountId(userId, account.AccountID);

            // Check if user is System user. If so, then get a list of all uac's
            var uacList = new List<UserRoleAccount>();

            if (uac.RoleID == sysUserRole.Id)
                uacList = businessLayer.GetUserRoleAccountByUserId(user.Id).ToList();

            var newUAC = new UserRoleAccount()
            {
                UserID = userId,
                RoleID = role.Id,
                AccountID = account.AccountID,
                ActiveAccount = (uacList.Count > 0) ? true : uac.ActiveAccount,
                UserEnabled = uac.UserEnabled
            };

            // Update account user role
            try
            {
                // Delete all uac's and aspnetuserrole entry if user is currently a system yser
                if (uacList.Count > 0)
                {
                    businessLayer.RemoveUserRoleAccount(uacList.ToArray());
                    var userRole = businessLayer.GetUserRoleByUserId(user.Id);
                    businessLayer.RemoveUserRole(userRole);
                    newUAC.ActiveAccount = true;
                }
                else
                    businessLayer.RemoveUserRoleAccount(uac);

                // Insert new UAC
                businessLayer.AddUserRoleAccount(newUAC);

                // Update all system user uac's and aspnetuserrole
                businessLayer.UpdateSystemUsers();
            }
            catch (Exception e)
            {
                returnVal = e.Message;
                return returnVal;
            }

            returnVal = string.Format("Role succesfully updated for user {0}", user.FirstNameLastName);

            return returnVal;
        }



        public static string UpdateAccountUser(ApplicationUser currentUser, string concatId, bool toBeAdmin)
        {
            var returnVal = "Ok";

            var businessLayer = new BusinessLayer.BusinessLayer(currentUser.Id);
            var splitStr = concatId.Split('#');

            var accountId = splitStr[0];
            var userId = splitStr[1];
            var adminRoleId = businessLayer.GetRoleByName("Administrator").Id;
            var userRoleId = businessLayer.GetRoleByName("User").Id;

            var user = businessLayer.GetUserById(userId);
            var uac = businessLayer.GetUserRoleAccountByUserIdAndAccountId(userId, Convert.ToInt32(accountId));

            var newUAC = new UserRoleAccount()
            {
                UserID = userId,
                RoleID = (toBeAdmin) ? adminRoleId : userRoleId,
                AccountID = Convert.ToInt32(accountId),
                ActiveAccount = uac.ActiveAccount,
                UserEnabled = uac.UserEnabled
            };

            // Update account user role
            try
            {
                // Delete old UAC
                businessLayer.RemoveUserRoleAccount(uac);

                // Insert new UAC
                businessLayer.AddUserRoleAccount(newUAC);
            }
            catch (Exception e)
            {
                returnVal = e.Message;
                return returnVal;
            }

            returnVal = string.Format("Role succesfully updated for user {0}", user.FirstNameLastName);

            return returnVal;
        }

        public static AccountUsersViewModel GetAccountUsersViewModel(ApplicationUser currentUser, ApplicationUser accountOwner)
        {
            var model = new AccountUsersViewModel();
            model.AccountUsers = new List<AccountUser>();

            var businessLayer = new BusinessLayer.BusinessLayer(currentUser.Id);

            // Get current account
            var currentAccount = GetCurrentAccountForUser(currentUser.Id);
            var currentOwnerId = currentAccount.Owner;

            model.AccountId = currentAccount.AccountID;

            // Get list of account users
            var uacList = businessLayer.GetUserRoleAccountByAccountId(currentAccount.AccountID);

            foreach (var uac in uacList)
            {
                var user = businessLayer.GetUserById(uac.UserID);
                var role = businessLayer.GetRoleById(uac.RoleID);

                // Do not include System users
                if (role.Name.ToLower() == "system user")
                    continue;

                // Do not include account owner
                //if (uac.UserID == accountOwner.Id)
                //    continue;

                if (role.Name.ToLower() != "system user")
                {
                    var accountUser = new AccountUser()
                    {
                        UserID = user.Id,
                        FirstNameLastName = user.FirstNameLastName,
                        Email = user.Email,
                        IsAdmin = (role.Name.ToLower() == "administrator"),
                        IsOwner = (user.Id == currentOwnerId) ? true : false
                    };

                    model.AccountUsers.Add(accountUser);
                }

                // Do not include current user


            }

            return model;
        }


        public static UserAdministratorViewModel GetUserAdministratorViewModel(ApplicationUser currentUser, Account currentAccount)
        {
            var model = new UserAdministratorViewModel();
            model.ListOfUsers = new List<UserAdminInfo>();
            model.ListOfRoles = new List<AspNetRoles>();

            var businessLayer = new BusinessLayer.BusinessLayer(currentUser.Id);

            var listOfUsers = new List<UserAdminInfo>();

            // Get user role account entries
            var uacs = businessLayer.GetUserRoleAccountByAccountId(currentAccount.AccountID);

            foreach (var item in uacs)
            {
                var userAdminInfo = new UserAdminInfo();

                // Get Account and User object
                var account = businessLayer.GetAccountById(item.AccountID);
                var user = businessLayer.GetUserById(item.UserID);

                userAdminInfo.UserId = user.Id;
                userAdminInfo.UserEnabled = user.Enabled; // User enabled
                userAdminInfo.UserLockedOut = (user.LockoutEnabled && user.LockoutEndDateUtc != null);
                userAdminInfo.UserLockedOutFailedCount = user.AccessFailedCount;

                userAdminInfo.AccountName = account.Name; // Account name
                userAdminInfo.UserName = user.UserName; // User name

                userAdminInfo.CurrentRole = businessLayer.GetRoleById(item.RoleID).Name; // Role
                userAdminInfo.CurrentRoleId = item.RoleID;

                model.ListOfUsers.Add(userAdminInfo);
            }

            model.ListOfRoles = businessLayer.GetAllRoles().ToList();

            return model;
        }

        public static AccountSettingsViewModel GetAccountSettingsViewModel(ApplicationUser currentUser)
        {
            var model = new AccountSettingsViewModel();
            var businessLayer = new BusinessLayer.BusinessLayer(currentUser.Id);

            var currentAccount = PlatformCommon.GetCurrentAccountForUser(currentUser.Id);

            model.Name = HttpUtility.HtmlDecode(currentAccount.Name);
            model.URL = currentAccount.URL;
            model.Address = currentAccount.Address;
            model.Address2 = currentAccount.Address2;
            model.Zip = currentAccount.Zip;
            model.City = currentAccount.City;
            model.Country = businessLayer.GetCountryById(currentAccount.CountryID).Name;
            model.Enabled = currentAccount.Enabled;
            model.Partner = currentAccount.IsPartner;
            model.Owner = currentAccount.Owner;

            // Attempt to get connection info
            if (currentAccount.ConnectionID != null)
            {
                var currConnection = businessLayer.GetConnectionById(currentAccount.ConnectionID);
                model.IsSOOnline = currConnection.IsSOOnline;
            }

            // Check if current user is system user
            var uar = businessLayer.GetUserRoleAccountByUserIdAndAccountId(currentUser.Id, currentAccount.AccountID);
            var currRole = businessLayer.GetRoleById(uar.RoleID);

            if (currRole.Name == "System user")
                model.IsSystemUser = true;
            else
                model.IsSystemUser = false;

            return model;
        }

        public static IFrameHostViewModel GetIFrameHostViewModel(ApplicationUser currentUser, int appId, bool SOOnlineEnabled)
        {
            var model = new IFrameHostViewModel();

            var currentAccount = PlatformCommon.GetCurrentAccountForUser(currentUser.Id);
            var appClass = AppCommon.GetAppClassInfo(currentAccount, currentUser, appId, SOOnlineEnabled);

            model.AppInfo = appClass;
            model.IsSystemUser = IsSystemUser(currentUser);

            return model;
        }

        public static EndpointViewModel GetEndpointViewModel(ApplicationUser currentUser)
        {
            var model = new EndpointViewModel();

            // Check if any endpoint has been defined
            var currAccount = PlatformCommon.GetCurrentAccountForUser(currentUser.Id);

            if (currAccount.ConnectionID != null)
            {
                var businessLayer = new BusinessLayer.BusinessLayer(currentUser.Id);

                var endpointInfo = businessLayer.GetConnectionById(currAccount.ConnectionID);

                model.ConnectionId = endpointInfo.ConnectionID;
                model.EndpointURI = (String.IsNullOrEmpty(endpointInfo.URL)) ? "-" : endpointInfo.URL;
                model.SysUser = endpointInfo.SysUser;
                model.SysPassword = endpointInfo.SysPassword;
                model.SOCompanyName = (String.IsNullOrEmpty(endpointInfo.CompanyName)) ? "-" : endpointInfo.CompanyName;
                model.SOVersion = endpointInfo.WSVersion.ToString();
                model.SOSerial = (String.IsNullOrEmpty(endpointInfo.CompanySerial)) ? "-" : endpointInfo.CompanySerial;
                model.SONetserverVersion = (String.IsNullOrEmpty(endpointInfo.NetServer)) ? "-" : endpointInfo.NetServer;
                model.PrefDescId = (endpointInfo.PrefDescId == null) ? 0 : endpointInfo.PrefDescId;
                model.IsSOOnline = endpointInfo.IsSOOnline;

                // Check if existing connection is SO Online connection. If so, then perform external connection test
                if (endpointInfo.IsSOOnline)
                {
                    model.HandshakeStatus = HandshakeStatus.Ok;

                    return model;
                }

                if (model.EndpointURI.ToLower().Contains("services75"))
                {
                    // A connection exists. Verify that handshake data is in order
                    businessLayer.SetSOProperties(model.SysUser, model.SysPassword, model.EndpointURI);

                    if (endpointInfo.PrefDescId == null)
                        model.HandshakeStatus = HandshakeStatus.NoKey;
                    else
                    {
                        var prefExists = businessLayer.SOGetAccountIdInPreference(endpointInfo.PrefDescId);

                        if (prefExists.ReturnValue < 0) // Value found not numeric (invalid value)
                            model.HandshakeStatus = HandshakeStatus.Invalid;
                        else if (prefExists.ReturnValue > 0) // Account Id found. Verify
                        {
                            if (currAccount.AccountID != prefExists.ReturnValue)
                                model.HandshakeStatus = HandshakeStatus.NoMatch;
                            else
                                model.HandshakeStatus = HandshakeStatus.Ok;

                        }
                        else
                            model.HandshakeStatus = HandshakeStatus.NotFound;
                    }
                }
                else
                {
                    // Attempt to login to SO
                    var soUtils = new SOUtils(model.SysUser, model.SysPassword, model.EndpointURI);

                    // Check if we are logged in to SO
                    if (SoContext.IsAuthenticated)
                    {
                        var loggedInCustId = SoContext.CurrentPrincipal.DatabaseContextIdentifier.ToLower();

                        // If we are logging in with a nother context identifer, we must log out first
                        if (loggedInCustId != currAccount.URL.ToLower())
                            SoContext.CloseCurrentSession();
                    }

                    // Log in if we do not have a session
                    if (!SoContext.IsAuthenticated)
                    {
                        var doLoginResp = soUtils.LogInToSO();
                        if (!doLoginResp.IsOK)
                        {
                            model.HandshakeStatus = HandshakeStatus.Invalid;

                            return model;
                        }
                    }

                    if (endpointInfo.PrefDescId == 0)
                        model.HandshakeStatus = HandshakeStatus.NoKey;
                    else
                    {
                        var prefExists = soUtils.GetAccountIdInPreference(endpointInfo.PrefDescId);

                        if (prefExists < 0) // Value found not numeric (invalid value)
                            model.HandshakeStatus = HandshakeStatus.Invalid;
                        else if (prefExists > 0) // Account Id found. Verify
                        {
                            if (currAccount.AccountID != prefExists)
                                model.HandshakeStatus = HandshakeStatus.NoMatch;
                            else
                                model.HandshakeStatus = HandshakeStatus.Ok;

                        }
                        else
                            model.HandshakeStatus = HandshakeStatus.NotFound;
                    }
                }
            }
            else
            {
                model.ConnectionId = 0;
                model.SOCompanyName = "-";
                model.SOVersion = "-";
                model.SOSerial = "-";
                model.SONetserverVersion = "-";
                model.HandshakeStatus = HandshakeStatus.NotFound;
            }

            return model;
        }

        public static AspNetRoles GetCurrentRoleForUser(string userId)
        {
            var currentAccountId = GetCurrentAccountForUser(userId).AccountID;
            var businessLayer = new BusinessLayer.BusinessLayer(userId);

            var currentURA = businessLayer.GetUserRoleAccountByUserIdAndAccountId(userId, currentAccountId);

            var currentRole = businessLayer.GetRoleById(currentURA.RoleID);

            return currentRole;
        }

        public static Account GetCurrentAccountForUser(string userId)
        {
            var businessLayer = new BusinessLayer.BusinessLayer(userId);
            var userRoleAccounts = businessLayer.GetUserRoleAccountByUserId(userId);

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
                return businessLayer.GetAccountById(currentAccountId);
            else
                return null;
        }


    }

    public class TypeAheadAccountList
    {
        public string AccountName { get; set; }
    }

    public class EndpointSOInfo
    {
        public string SOCompanyName { get; set; }
        public string SOVersion { get; set; }
        public string SOSerial { get; set; }
        public string SONetServerVersion { get; set; }
        public string SOHandshakeStatus { get; set; }

        public EndpointSOInfo()
        {
            SOCompanyName = string.Empty;
            SOVersion = string.Empty;
            SOSerial = string.Empty;
            SONetServerVersion = string.Empty;
            SOHandshakeStatus = string.Empty;
        }
    }
}