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
        IList<AppSystemUserToken> GetAllAppSystemUserTokens();
        IList<AppSystemUserToken> GetAppSystemUserTokensByAppId(int appId);
        IList<AppSystemUserToken> GetAppSystemUserTokensByConnectionId(int connectionId);
        IList<AppSystemUserToken> GetAppSystemUserTokenByAppIdAndConnectionId(int appId, int connectionId);
        AppSystemUserToken GetAppSystemUserTokenByAppIdAndConnectionIdSingle(int appId, int connectionId);
        void AddAppSystemUserToken(params AppSystemUserToken[] AppSystemUserTokens);
        void UpdateAppSystemUserToken(params AppSystemUserToken[] AppSystemUserTokens);
        void RemoveAppSystemUserToken(params AppSystemUserToken[] AppSystemUserTokens);
    }

    public partial class BusinessLayer : IBusinessLayer
    {
        public IList<AppSystemUserToken> GetAllAppSystemUserTokens()
        {
            return _AppSystemUserTokenRepository.GetAll();
        }

        public IList<AppSystemUserToken> GetAppSystemUserTokensByAppId(int appId)
        {
            return _AppSystemUserTokenRepository.GetList(e => e.AppID.Equals(appId));
        }

        public IList<AppSystemUserToken> GetAppSystemUserTokensByConnectionId(int connectionId)
        {
            return _AppSystemUserTokenRepository.GetList(e => e.ConnectionID.Equals(connectionId));
        }

        public IList<AppSystemUserToken> GetAppSystemUserTokenByAppIdAndConnectionId(int appId, int connectionId)
        {
            return _AppSystemUserTokenRepository.GetList(e => e.AppID.Equals(appId) && e.ConnectionID.Equals(connectionId));
        }

        public AppSystemUserToken GetAppSystemUserTokenByAppIdAndConnectionIdSingle(int appId, int connectionId)
        {
            return _AppSystemUserTokenRepository.GetSingle(e => e.AppID.Equals(appId) && e.ConnectionID.Equals(connectionId));
        }

        public void AddAppSystemUserToken(params AppSystemUserToken[] AppSystemUserTokens)
        {
            /* Validation and error handling omitted */

            _AppSystemUserTokenRepository.Add(AppSystemUserTokens);
        }

        public void UpdateAppSystemUserToken(params AppSystemUserToken[] AppSystemUserTokens)
        {
            /* Validation and error handling omitted */

            _AppSystemUserTokenRepository.Update(AppSystemUserTokens);
        }

        public void RemoveAppSystemUserToken(params AppSystemUserToken[] AppSystemUserTokens)
        {
            /* Validation and error handling omitted */
            _AppSystemUserTokenRepository.Remove(AppSystemUserTokens);
        }
    }
}
