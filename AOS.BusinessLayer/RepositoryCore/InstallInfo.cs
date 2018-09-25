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
        IList<InstallInfo> GetAllInstallInfos();
        InstallInfo GetInstallInfoByAppId(int AppId);
        void AddInstallInfo(params InstallInfo[] InstallInfos);
        void UpdateInstallInfo(params InstallInfo[] InstallInfos);
        void RemoveInstallInfo(params InstallInfo[] InstallInfos);
    }

    public partial class BusinessLayer : IBusinessLayer
    {
        public IList<InstallInfo> GetAllInstallInfos()
        {
            return _InstallInfoRepository.GetAll();
        }

        public InstallInfo GetInstallInfoByAppId(int AppId)
        {
            return _InstallInfoRepository.GetSingle(d => d.AppId.Equals(AppId));
        }

        public void AddInstallInfo(params InstallInfo[] InstallInfos)
        {
            /* Validation and error handling omitted */
            _InstallInfoRepository.Add(InstallInfos);
        }

        public void UpdateInstallInfo(params InstallInfo[] InstallInfos)
        {
            /* Validation and error handling omitted */
            _InstallInfoRepository.Update(InstallInfos);
        }

        public void RemoveInstallInfo(params InstallInfo[] InstallInfos)
        {
            /* Validation and error handling omitted */
            _InstallInfoRepository.Remove(InstallInfos);
        }
    }
}
