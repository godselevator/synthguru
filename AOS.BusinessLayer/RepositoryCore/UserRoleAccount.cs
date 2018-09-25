using AOS.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOS.BusinessLayer
{
    public partial interface IBusinessLayer
    {
        IList<UserRoleAccount> GetAllUserRoleAccounts();
        IList<UserRoleAccount> GetUserRoleAccountByUserId(string userId);
        IList<UserRoleAccount> GetUserRoleAccountByAccountId(int accountId);
        IList<UserRoleAccount> GetUserRoleAccountByRoleId(string roleId);
        UserRoleAccount GetUserRoleAccountByUserIdAndAccountId(string userId, int accountId);
        UserRoleAccount GetActiveAccountId();

        void AddUserRoleAccount(params UserRoleAccount[] userRoleAccounts);
        void UpdateUserRoleAccount(params UserRoleAccount[] userRoleAccounts);
        void RemoveUserRoleAccount(params UserRoleAccount[] userRoleAccounts);
        void UpdateSystemUsers();
    }

    public partial class BusinessLayer : IBusinessLayer
    {
        public IList<UserRoleAccount> GetAllUserRoleAccounts()
        {
            return _UserRoleAccountRepository.GetAll();
        }

        public IList<UserRoleAccount> GetUserRoleAccountByUserId(string userId)
        {
            return _UserRoleAccountRepository.GetList(e => e.UserID.Equals(userId));
        }

        public IList<UserRoleAccount> GetUserRoleAccountByAccountId(int accountId)
        {
            return _UserRoleAccountRepository.GetList(e => e.AccountID.Equals(accountId));
        }

        public IList<UserRoleAccount> GetUserRoleAccountByRoleId(string roleId)
        {
            return _UserRoleAccountRepository.GetList(e => e.RoleID.Equals(roleId));
        }

        public UserRoleAccount GetUserRoleAccountByUserIdAndAccountId(string userId, int accountId)
        {
            return _UserRoleAccountRepository.GetSingle(e => e.UserID.Equals(userId) && e.AccountID.Equals(accountId));
        }

        public UserRoleAccount GetActiveAccountId()
        {
            return _UserRoleAccountRepository.GetList(e => e.ActiveAccount == true).FirstOrDefault();
        }

        public void AddUserRoleAccount(params UserRoleAccount[] userRoleAccounts)
        {
            /* Validation and error handling omitted */

            // Fill createdate and createuser fields
            foreach (var item in userRoleAccounts)
            {
                item.CreateDate = DateTime.Now;
                item.CreateUser = _CurrentUser;
            }

            _UserRoleAccountRepository.Add(userRoleAccounts);
        }

        public void UpdateUserRoleAccount(params UserRoleAccount[] userRoleAccounts)
        {
            /* Validation and error handling omitted */

            // Fill updatedate and updateuser fields
            foreach (var item in userRoleAccounts)
            {
                item.UpdateDate = DateTime.Now;
                item.UpdateUser = _CurrentUser;
            }

            _UserRoleAccountRepository.Update(userRoleAccounts);
        }

        public void RemoveUserRoleAccount(params UserRoleAccount[] UserRoleAccount)
        {
            /* Validation and error handling omitted */
            _UserRoleAccountRepository.Remove(UserRoleAccount);
        }

        public void UpdateSystemUsers()
        {
            // Get role id of System user
            var systemUserRoleId = GetRoleByName("System user").Id;

            // Get a list of all accounts
            var accountsList = GetAllAccounts();

            // Get a list of System users
            var systemUserUACList = GetUserRoleAccountByRoleId(systemUserRoleId);

            // Get distinct list of user id's from System users list
            var distinctUserIdList = (
                from p in systemUserUACList
                select p.UserID)
                .Distinct();

            // Get users active account
            var systemUsersList = new Dictionary<string, bool>();

            foreach (var item in distinctUserIdList)
            {
                var uraList = GetUserRoleAccountByUserId(item);

                var actFound = false;

                foreach (var item2 in uraList)
                {

                    if (item2.ActiveAccount)
                    {
                        actFound = true;
                        break;
                    }
                }

                systemUsersList.Add(item, actFound);
            }

            foreach (KeyValuePair<string, bool> item in systemUsersList)
            {
                // Get list of UAC for this System user
                var uacList = GetUserRoleAccountByUserId(item.Key);

                foreach (var acc in accountsList)
                {
                    bool accFound = false;

                    var role = string.Empty;

                    foreach (var uac in uacList)
                    {
                        if (acc.AccountID == uac.AccountID)
                        {
                            role = GetRoleById(uac.RoleID).Name;
                            accFound = true;
                        }
                    }

                    if (!accFound)
                    {
                        // Create UAC entry
                        var newUAC = new UserRoleAccount()
                        {
                            UserID = item.Key,
                            RoleID = systemUserRoleId,
                            AccountID = acc.AccountID,
                            ActiveAccount = (item.Value) ? false : true,
                            UserEnabled = false // Not used
                        };

                        // Create the missing account
                        AddUserRoleAccount(newUAC);
                    }

                }
            }

            // Update userrole table
            foreach (var userId in distinctUserIdList)
            {
                // Attempt to get userrole entry
                var ur = GetUserRoleByUserId(userId);

                // If user does not have an entry in AspNetUserRole table, then create one
                if (ur == null)
                {
                    var newUr = new AspNetUserRoles()
                    {
                        RoleId = systemUserRoleId,
                        UserId = userId
                    };

                    AddUserRole(newUr);
                }
            }

            // Get a list of userrole entries
            var userRoleList = GetAllUserRoles();

            // Delete users that are not system users anymore
            foreach (var existItem in userRoleList)
            {
                var found = false;

                foreach (var uac in distinctUserIdList)
                {
                    if (uac == existItem.UserId)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                    RemoveUserRole(existItem);
            }
        }

    }
}
