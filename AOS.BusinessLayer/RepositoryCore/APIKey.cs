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
        IList<APIKey> GetAllAPIKeys();
        APIKey GetAPIKeyByAppIdAndAccountId(int AppId, int AccountId);
        APIKey GetAPIKeyByAPIKey(string APIKey);
        void AddAPIKey(params APIKey[] APIKeys);
        void UpdateAPIKey(params APIKey[] APIKeys);
        void RemoveAPIKey(params APIKey[] APIKeys);
    }

    public partial class BusinessLayer : IBusinessLayer
    {
        public IList<APIKey> GetAllAPIKeys()
        {
            return _APIKeyRepository.GetAll();
        }

        public APIKey GetAPIKeyByAppIdAndAccountId(int AppId, int AccountId)
        {
            return _APIKeyRepository.GetSingle(d => d.AppId.Equals(AppId) && d.AccountId.Equals(AccountId));
        }

        public APIKey GetAPIKeyByAPIKey(string APIKey)
        {
            return _APIKeyRepository.GetSingle(d => d.APIKeyValue.Equals(APIKey));
        }

        public void AddAPIKey(params APIKey[] APIKeys)
        {
            /* Validation and error handling omitted */
            _APIKeyRepository.Add(APIKeys);
        }

        public void UpdateAPIKey(params APIKey[] APIKeys)
        {
            /* Validation and error handling omitted */
            _APIKeyRepository.Update(APIKeys);
        }

        public void RemoveAPIKey(params APIKey[] APIKeys)
        {
            /* Validation and error handling omitted */
            _APIKeyRepository.Remove(APIKeys);
        }
    }
}
