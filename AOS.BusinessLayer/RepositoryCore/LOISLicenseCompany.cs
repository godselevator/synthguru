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
        IList<LOISLicenseCompany> GetAllLOISLicenseCompany();
        IList<LOISLicenseCompany> GetLOISLicenseCompanyByOwnerEmail(string OwnerEmail);
        IList<LOISLicenseCompany> GetLOISLicenseCompanyByCompanyName(string CompanyName);
        LOISLicenseCompany GetLOISLicenseCompanyByOwnerEmailAndCompanyName(string OwnerEmail, string CompanyName);
        LOISLicenseCompany GetLOISLicenseCompanyByCompanyLicenseKey(string CompanyLicenseKey);
        void AddLOISLicenseCompany(params LOISLicenseCompany[] LOISLicenseCompany);
        void UpdateLOISLicenseCompany(params LOISLicenseCompany[] LOISLicenseCompany);
        void RemoveLOISLicenseCompany(params LOISLicenseCompany[] LOISLicenseCompany);
    }

    public partial class BusinessLayer : IBusinessLayer
    {
        public IList<LOISLicenseCompany> GetAllLOISLicenseCompany()
        {
            return _LOISLicenseCompanyRepository.GetAll();
        }

        public IList<LOISLicenseCompany> GetLOISLicenseCompanyByOwnerEmail(string OwnerEmail)
        {
            return _LOISLicenseCompanyRepository.GetList(d => d.OwnerEmail.ToLower().Equals(OwnerEmail.ToLower()));
        }

        public IList<LOISLicenseCompany> GetLOISLicenseCompanyByCompanyName(string CompanyName)
        {
            return _LOISLicenseCompanyRepository.GetList(d => d.CompanyName.ToLower().Equals(CompanyName.ToLower()));
        }

        public LOISLicenseCompany GetLOISLicenseCompanyByOwnerEmailAndCompanyName(string OwnerEmail, string CompanyName)
        {
            return _LOISLicenseCompanyRepository.GetSingle(d => d.OwnerEmail.ToLower().Equals(OwnerEmail.ToLower()) && d.CompanyName.ToLower().Equals(CompanyName.ToLower()));
        }

        public LOISLicenseCompany GetLOISLicenseCompanyByCompanyLicenseKey(string CompanyLicenseKey)
        {
            return _LOISLicenseCompanyRepository.GetSingle(d => d.CompanyLicenseKey.Equals(CompanyLicenseKey));
        }

        public void AddLOISLicenseCompany(params LOISLicenseCompany[] LOISLicenseCompany)
        {
            /* Validation and error handling omitted */
            _LOISLicenseCompanyRepository.Add(LOISLicenseCompany);
        }

        public void UpdateLOISLicenseCompany(params LOISLicenseCompany[] LOISLicenseCompany)
        {
            /* Validation and error handling omitted */
            _LOISLicenseCompanyRepository.Update(LOISLicenseCompany);
        }

        public void RemoveLOISLicenseCompany(params LOISLicenseCompany[] LOISLicenseCompany)
        {
            /* Validation and error handling omitted */
            _LOISLicenseCompanyRepository.Remove(LOISLicenseCompany);
        }
    }
}
