using AOS.DomainModel;
using System;
using System.Collections.Generic;

namespace AOS.BusinessLayer
{
    public partial interface IBusinessLayer
    {
        AccountAppAssociate GetAccountAppAssociate(int accountId, int appId, int associateId);
        IList<AccountAppAssociate> GetAccountAppAssociatesByAccountId(int accountId);
        IList<AccountAppAssociate> GetAccountAppAssociatesByAccountIdAndAppId(int accountId, int appId);
        AccountAppAssociate GetAccountAppAssociatesByAccountIdAppIdAndAssociateId(int accountId, int appId, int associateId);
        void AddAccountAppAssociate(params AccountAppAssociate[] AccountAppAssociates);
        void UpdateAccountAppAssociate(params AccountAppAssociate[] AccountAppAssociates);
        void RemoveAccountAppAssociate(params AccountAppAssociate[] AccountAppAssociates);
    }

    public partial class BusinessLayer : IBusinessLayer
    {
        public AccountAppAssociate GetAccountAppAssociate(int accountId, int appId, int associateId)
        {
            return _AccountAppAssociateRepository.GetSingle(e => e.AccountID.Equals(accountId) && e.AppID.Equals(appId) && e.AssociateID.Equals(associateId));
        }

        public IList<AccountAppAssociate> GetAccountAppAssociatesByAccountId(int accountId)
        {
            return _AccountAppAssociateRepository.GetList(e => e.AccountID.Equals(accountId));
        }

        public IList<AccountAppAssociate> GetAccountAppAssociatesByAccountIdAndAppId(int accountId, int appId)
        {
            return _AccountAppAssociateRepository.GetList(e => e.AccountID.Equals(accountId) && e.AppID.Equals(appId));
        }

        public AccountAppAssociate GetAccountAppAssociatesByAccountIdAppIdAndAssociateId(int accountId, int appId, int associateId)
        {
            return _AccountAppAssociateRepository.GetSingle(e => e.AccountID.Equals(accountId) && e.AppID.Equals(appId) && e.AssociateID.Equals(associateId));
        }

        public void AddAccountAppAssociate(params AccountAppAssociate[] AccountAppAssociates)
        {
            /* Validation and error handling omitted */

            // Fill createdate and createuser fields
            foreach (var item in AccountAppAssociates)
            {
                item.CreateDate = DateTime.Now;
                item.CreateUser = _CurrentUser;
            }

            _AccountAppAssociateRepository.Add(AccountAppAssociates);
        }

        public void UpdateAccountAppAssociate(params AccountAppAssociate[] AccountAppAssociates)
        {
            /* Validation and error handling omitted */

            // Fill createdate and createuser fields
            foreach (var item in AccountAppAssociates)
            {
                item.UpdateDate = DateTime.Now;
                item.UpdateUser = _CurrentUser;
            }

            _AccountAppAssociateRepository.Update(AccountAppAssociates);
        }

        public void RemoveAccountAppAssociate(params AccountAppAssociate[] AccountAppAssociates)
        {
            /* Validation and error handling omitted */
            _AccountAppAssociateRepository.Remove(AccountAppAssociates);
        }
    }
}
