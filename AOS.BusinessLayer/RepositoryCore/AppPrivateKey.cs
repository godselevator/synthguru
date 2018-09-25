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
        AppPrivateKey GetAppPrivateKeyById(int AppPrivateKeyId);
        IList<AppPrivateKey> GetAppPrivateKeysByAppId(int AppId);
        AppPrivateKey GetAppPrivateKeyByAppIdAndEnvironment(int AppId, string Environment);
        void AddAppPrivateKey(params AppPrivateKey[] AppPrivateKey);
        void UpdateAppPrivateKey(params AppPrivateKey[] AppPrivateKey);
        void RemoveAppPrivateKey(params AppPrivateKey[] AppPrivateKey);
    }

    public partial class BusinessLayer : IBusinessLayer
    {
        public AppPrivateKey GetAppPrivateKeyById(int AppPrivateKeyId)
        {
            return _AppPrivateKeyRepository.GetSingle(d => d.AppPrivateKeyID.Equals(AppPrivateKeyId));
        }

        public IList<AppPrivateKey> GetAppPrivateKeysByAppId(int AppId)
        {
            return _AppPrivateKeyRepository.GetList(d => d.AppID.Equals(AppId));
        }

        public AppPrivateKey GetAppPrivateKeyByAppIdAndEnvironment(int AppId, string Environment)
        {
            return _AppPrivateKeyRepository.GetSingle(d => d.AppID.Equals(AppId) && d.Environment.ToLower().Equals(Environment.ToLower()));
        }

        public void AddAppPrivateKey(params AppPrivateKey[] AppPrivateKey)
        {
            _AppPrivateKeyRepository.Add(AppPrivateKey);
        }

        public void UpdateAppPrivateKey(params AppPrivateKey[] AppPrivateKey)
        {
            _AppPrivateKeyRepository.Update(AppPrivateKey);
        }

        public void RemoveAppPrivateKey(params AppPrivateKey[] AppPrivateKey)
        {
            /* Validation and error handling omitted */
            _AppPrivateKeyRepository.Remove(AppPrivateKey);
        }
    }
}
