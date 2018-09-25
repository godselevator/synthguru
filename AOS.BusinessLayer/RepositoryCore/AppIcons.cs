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
        AppIcons GetAppIconsById(int AppIconsId);
        AppIcons GetAppIconsByAppId(int AppIconsId);
        void AddAppIcons(params AppIcons[] AppIcons);
        void UpdateAppIcons(params AppIcons[] AppIcons);
        void RemoveAppIcons(params AppIcons[] AppIcons);
    }

    public partial class BusinessLayer : IBusinessLayer
    {
        public AppIcons GetAppIconsById(int AppIconsId)
        {
            return _AppIconsRepository.GetSingle(d => d.AppIconID.Equals(AppIconsId));
        }

        public AppIcons GetAppIconsByAppId(int AppId)
        {
            return _AppIconsRepository.GetSingle(d => d.AppID.Equals(AppId));
        }

        public void AddAppIcons(params AppIcons[] AppIcons)
        {
            _AppIconsRepository.Add(AppIcons);
        }

        public void UpdateAppIcons(params AppIcons[] AppIcons)
        {
            _AppIconsRepository.Update(AppIcons);
        }

        public void RemoveAppIcons(params AppIcons[] AppIcons)
        {
            /* Validation and error handling omitted */
            _AppIconsRepository.Remove(AppIcons);
        }
    }
}
