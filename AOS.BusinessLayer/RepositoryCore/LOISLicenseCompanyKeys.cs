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
        IList<LOISLicenseCompanyKeys> GetAllLOISLicenseCompanyKeys();
        IList<LOISLicenseCompanyKeys> GetLOISLicenseCompanyKeysByCompanyId(int CompanyId);
        IList<LOISLicenseCompanyKeys> GetLOISLicenseCompanyKeysBySystemId(string SystemId);
        LOISLicenseCompanyKeys GetLOISLicenseCompanyKeysByCompanyIdAndSystemId(int CompanyId, string SystemId);
        LOISLicenseCompanyKeys GetLOISLicenseCompanyKeysByLOISSystemLicenseKey(string SystemLicenseKey);
        void AddLOISLicenseCompanyKeys(params LOISLicenseCompanyKeys[] LOISLicenseCompanyKeys);
        void UpdateLOISLicenseCompanyKeys(params LOISLicenseCompanyKeys[] LOISLicenseCompanyKeys);
        void RemoveLOISLicenseCompanyKeys(params LOISLicenseCompanyKeys[] LOISLicenseCompanyKeys);
    }

    public partial class BusinessLayer : IBusinessLayer
    {
        public IList<LOISLicenseCompanyKeys> GetAllLOISLicenseCompanyKeys()
        {
            return _LOISLicenseCompanyKeysRepository.GetAll();
        }

        public IList<LOISLicenseCompanyKeys> GetLOISLicenseCompanyKeysByCompanyId(int CompanyId)
        {
            return _LOISLicenseCompanyKeysRepository.GetList(d => d.LOISCompanyId.Equals(CompanyId));
        }

        public IList<LOISLicenseCompanyKeys> GetLOISLicenseCompanyKeysBySystemId(string SystemId)
        {
            return _LOISLicenseCompanyKeysRepository.GetList(d => d.LOISSystemId.Equals(SystemId));
        }

        public LOISLicenseCompanyKeys GetLOISLicenseCompanyKeysByCompanyIdAndSystemId(int CompanyId, string SystemId)
        {
            return _LOISLicenseCompanyKeysRepository.GetSingle(d => d.LOISCompanyId.Equals(CompanyId) && d.LOISSystemId.Equals(SystemId));
        }
        public LOISLicenseCompanyKeys GetLOISLicenseCompanyKeysByLOISSystemLicenseKey(string SystemLicenseKey)
        {
            return _LOISLicenseCompanyKeysRepository.GetSingle(d => d.LOISSystemLicenseKey.Equals(SystemLicenseKey));
        }

        public void AddLOISLicenseCompanyKeys(params LOISLicenseCompanyKeys[] LOISLicenseCompanyKeys)
        {
            /* Validation and error handling omitted */
            _LOISLicenseCompanyKeysRepository.Add(LOISLicenseCompanyKeys);
        }

        public void UpdateLOISLicenseCompanyKeys(params LOISLicenseCompanyKeys[] LOISLicenseCompanyKeys)
        {
            /* Validation and error handling omitted */
            _LOISLicenseCompanyKeysRepository.Update(LOISLicenseCompanyKeys);
        }

        public void RemoveLOISLicenseCompanyKeys(params LOISLicenseCompanyKeys[] LOISLicenseCompanyKeys)
        {
            /* Validation and error handling omitted */
            _LOISLicenseCompanyKeysRepository.Remove(LOISLicenseCompanyKeys);
        }
    }
}
