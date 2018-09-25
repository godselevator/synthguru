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
        AppOnlineProvisioning GetAppOnlineProvisioningByProvisioningKey(string key);
        void AddAppOnlineProvisioning(params AppOnlineProvisioning[] AppOnlineProvisioning);
        void UpdateAppOnlineProvisioning(params AppOnlineProvisioning[] AppOnlineProvisioning);
        void RemoveAppOnlineProvisioning(params AppOnlineProvisioning[] AppOnlineProvisioning);
    }

    public partial class BusinessLayer : IBusinessLayer
    {
        public AppOnlineProvisioning GetAppOnlineProvisioningByProvisioningKey(string key)
        {
            return _AppOnlineProvisioningRepository.GetSingle(d => d.ProvisioningKey.ToLower().Equals(key.ToLower()));
        }

        public void AddAppOnlineProvisioning(params AppOnlineProvisioning[] AppOnlineProvisioning)
        {
            /* Validation and error handling omitted */
            _AppOnlineProvisioningRepository.Add(AppOnlineProvisioning);
        }

        public void UpdateAppOnlineProvisioning(params AppOnlineProvisioning[] AppOnlineProvisioning)
        {
            /* Validation and error handling omitted */
            _AppOnlineProvisioningRepository.Update(AppOnlineProvisioning);
        }

        public void RemoveAppOnlineProvisioning(params AppOnlineProvisioning[] AppOnlineProvisioning)
        {
            /* Validation and error handling omitted */
            _AppOnlineProvisioningRepository.Remove(AppOnlineProvisioning);
        }
    }
}
