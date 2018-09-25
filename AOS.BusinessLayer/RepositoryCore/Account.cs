using AOS.DataAccessLayer;
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
        IList<Account> GetAllAccounts();
        Account GetAccountById(int accountId);
        IList<Account> GetAccountsByOwner(string ownerId);
        Account GetAccountByName(string accountName);
        Account GetAccountByNameThin(string accountName);
        Account GetAccountByURL(string url);
        Account GetAccountByURLThin(string url);
        Account GetAccountByConnectionId(int connectionId);
        void AddAccount(params Account[] accounts);
        void UpdateAccount(params Account[] accounts);
        void RemoveAccount(params Account[] accounts);
        bool IsAccountOwner(string userId);
        bool IsAccountOwnerForAccount(string userId, Account account);
        IList<Account> GetOwnedAccountsForUser(string userId);
    }
    
    public partial class BusinessLayer : IBusinessLayer
    {
        public IList<Account> GetAllAccounts()
        {
            return _AccountRepository.GetAll();
        }

        public Account GetAccountById(int accountId)
        {
            // Return only account. Skip related tables
            return _AccountRepository.GetSingle(
                d => d.AccountID.Equals(accountId));
        }

        public IList<Account> GetAccountsByOwner(string ownerId)
        {
            // Return only account. Skip related tables
            return _AccountRepository.GetList(
                d => d.Owner.ToLower().Equals(ownerId.ToLower()));
        }

        public IList<Account> GetAccountListByName(string searchStr)
        {
            // Return only account. Skip related tables
            return _AccountRepository.GetList(
                d => d.Name.ToLower().Contains(searchStr.ToLower()));
        }

        public IList<Account> GetAccountListByURL(string searchStr)
        {
            // Return only account. Skip related tables
            return _AccountRepository.GetList(
                d => d.URL.ToLower().Contains(searchStr.ToLower()));
        }

        public Account GetAccountByName(string accountName)
        {
            // Return account and all related tables
            return _AccountRepository.GetSingle(
                d => d.Name.ToLower().Equals(accountName.ToLower()),
                d => d.AccountApp,
                d => d.Addressbook,
                d => d.BisnodeManager,
                d => d.Connection,
                d => d.Converter,
                d => d.ElasticIO,
                d => d.Referral,
                d => d.RelationWise,
                d => d.RWAppointmentTask,
                d => d.RWAppointmentTrigger,
                d => d.RWChosenVariable,
                d => d.RWSaleTrigger,
                d => d.RWTrigger,
                d => d.SignicatDocumentTemplate,
                d => d.Signicat);
        }

        public Account GetAccountByNameThin(string accountName)
        {
            // Return only account. Skip related tables
            return _AccountRepository.GetSingle(
                d => d.Name.ToLower().Equals(accountName.ToLower()));
        }

        public Account GetAccountByURL(string url)
        {
            //// Return only account. Skip related tables
            //return _AccountRepository.GetSingle(
            //    d => d.URL.ToLower().Equals(url.ToLower()));
            // Return account and all related tables
            return _AccountRepository.GetSingle(
                d => d.URL.ToLower().Equals(url.ToLower()),
                d => d.AccountApp,
                d => d.Addressbook,
                d => d.BisnodeManager,
                d => d.Connection,
                d => d.Converter,
                d => d.ElasticIO,
                d => d.Referral,
                d => d.RelationWise,
                d => d.RWAppointmentTask,
                d => d.RWAppointmentTrigger,
                d => d.RWChosenVariable,
                d => d.RWSaleTrigger,
                d => d.RWTrigger,
                d => d.SignicatDocumentTemplate,
                d => d.Signicat);
        }

        public Account GetAccountByURLThin(string url)
        {
            //// Return only account. Skip related tables
            //return _AccountRepository.GetSingle(
            //    d => d.URL.ToLower().Equals(url.ToLower()));
            // Return account and all related tables
            return _AccountRepository.GetSingle(
                d => d.URL.ToLower().Equals(url.ToLower()));
        }

        public Account GetAccountByConnectionId(int connectionId)
        {
            // Return only account. Skip related tables
            return _AccountRepository.GetSingle(
                d => d.ConnectionID.Equals(connectionId));
        }

        public void AddAccount(params Account[] accounts)
        {
            /* Validation and error handling omitted */

            // Fill createdate and createuser fields
            foreach (var item in accounts)
            {
                item.CreateDate = DateTime.Now;
                item.CreateUser = _CurrentUser;
            }

            int numberOfRowsAdded = _AccountRepository.Add(accounts);

            // Post-processing. When new accounts are added, all System users must be updated in the UserAccountRole table
            if (numberOfRowsAdded > 0)
                UpdateSystemUsers();
        }

        public void UpdateAccount(params Account[] accounts)
        {
            /* Validation and error handling omitted */

            // Fill createdate and createuser fields
            foreach (var item in accounts)
            {
                item.UpdateDate = DateTime.Now;
                item.UpdateUser = _CurrentUser;
            }

            _AccountRepository.Update(accounts);
        }

        public void RemoveAccount(params Account[] accounts)
        {
            /* Validation and error handling omitted */

            int numberOfRowsRemoved = _AccountRepository.Remove(accounts);

            // Post-processing. When new accounts are added, all System users must be updated in the UserAccountRole table
            if (numberOfRowsRemoved > 0)
                UpdateSystemUsers();
        }

        public bool IsAccountOwner(string userId)
        {
            var accounts = GetAllAccounts();

            return accounts.Any(acc => acc.Owner == userId);
        }

        public bool IsAccountOwnerForAccount(string userId, Account account)
        {
            return (account.Owner == userId);
        }

        public IList<Account> GetOwnedAccountsForUser(string userId)
        {
            var accounts = GetAllAccounts();

            return accounts.Where(acc => acc.Owner == userId).ToList();
        }
    }
}
