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
        IList<AccountApp> GetAccountAppsByAppId(int appId);
        IList<AccountApp> GetAccountAppsByAccountId(int accountId);
        AccountApp GetAccountAppByAccountIdAndAppId(int accountId, int appId);
        void AddAccountApp(params AccountApp[] accountApps);
        void UpdateAccountApp(params AccountApp[] accountApps);
        void RemoveAccountApp(params AccountApp[] accountApps);
    }

    public partial class BusinessLayer : IBusinessLayer
    {
        public IList<AccountApp> GetAccountAppsByAppId(int appId)
        {
            return _AccountAppRepository.GetList(e => e.AppID.Equals(appId));
        }

        public IList<AccountApp> GetAccountAppsByAccountId(int accountId)
        {
            return _AccountAppRepository.GetList(e => e.AccountID.Equals(accountId));
        }

        public AccountApp GetAccountAppByAccountIdAndAppId(int accountId, int appId)
        {
            return _AccountAppRepository.GetSingle(e => e.AccountID.Equals(accountId) && e.AppID.Equals(appId));
        }

        public void AddAccountApp(params AccountApp[] accountApps)
        {
            /* Validation and error handling omitted */

            // Fill createdate and createuser fields
            foreach (var item in accountApps)
            {
                item.CreateDate = DateTime.Now;
                item.CreateUser = _CurrentUser;
            }

            _AccountAppRepository.Add(accountApps);
        }

        public void UpdateAccountApp(params AccountApp[] accountApps)
        {
            /* Validation and error handling omitted */

            // Fill createdate and createuser fields
            foreach (var item in accountApps)
            {
                item.UpdateDate = DateTime.Now;
                item.UpdateUser = _CurrentUser;
            }

            _AccountAppRepository.Update(accountApps);
        }

        public void RemoveAccountApp(params AccountApp[] accountApps)
        {
            /* Validation and error handling omitted */
            _AccountAppRepository.Remove(accountApps);
        }
    }
}
