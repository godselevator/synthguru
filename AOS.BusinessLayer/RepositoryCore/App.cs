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
        IList<App> GetAllApps();
        List<App> GetAllNormalApps();
        IList<App> GetAllEnabledApps();
        App GetAppById(int appId);
        App GetAppByCode(string code);
        App GetAppByName(string name);
        App GetAppByApplicationToken(string applicationToken);
        void AddApp(params App[] apps);
        void UpdateApp(params App[] apps);
        void RemoveApp(params App[] apps);
    }

    public partial class BusinessLayer : IBusinessLayer
    {
        public IList<App> GetAllApps()
        {
            return _AppRepository.GetAll();
        }

        public List<App> GetAllNormalApps()
        {
            var resp = new List<App>();

            var allApps = _AppRepository.GetAll().ToList();

            foreach (var item in allApps)
            {
                if (item.Code.ToUpper() != "PARTNERAPP" && item.Code.ToUpper() != "CONNECTAPP")
                    resp.Add(item);
            }

            return resp;
        }

        public IList<App> GetAllEnabledApps()
        {
            return _AppRepository.GetAll(d => d.Enabled.Equals(true));
        }

        public App GetAppById(int appId)
        {
            return _AppRepository.GetSingle(d => d.AppID.Equals(appId));
        }

        public App GetAppByCode(string code)
        {
            return _AppRepository.GetSingle(d => d.Code.ToLower().Equals(code.ToLower()));
        }

        public App GetAppByName(string name)
        {
            return _AppRepository.GetSingle(d => d.Name.ToLower().Equals(name.ToLower()));
        }

        public App GetAppByApplicationToken(string applicationToken)
        {
            return _AppRepository.GetSingle(d => d.ApplicationToken.ToLower().Equals(applicationToken.ToLower()));
        }

        public void AddApp(params App[] apps)
        {
            /* Validation and error handling omitted */

            // Fill createdate and createuser fields
            foreach (var item in apps)
            {
                item.CreateDate = DateTime.Now;
                item.CreateUser = _CurrentUser;
            }

            _AppRepository.Add(apps);
        }

        public void UpdateApp(params App[] apps)
        {
            /* Validation and error handling omitted */

            // Fill updatedate and updateuser fields
            foreach (var item in apps)
            {
                item.UpdateDate = DateTime.Now;
                item.UpdateUser = _CurrentUser;
            }

            _AppRepository.Update(apps);
        }

        public void RemoveApp(params App[] apps)
        {
            /* Validation and error handling omitted */
            _AppRepository.Remove(apps);
        }
    }
}
