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
        IList<Language> GetAllLanguages();
        Language GetLanguageById(int languageId);
        Language GetLanguageByCountryCode(string countryCode);
        Language GetLanguageByLICD(int licd);
        void AddLanguage(params Language[] languages);
        void UpdateLanguage(params Language[] languages);
        void RemoveLanguage(params Language[] languages);
    }

    public partial class BusinessLayer : IBusinessLayer
    {
        public IList<Language> GetAllLanguages()
        {
            return _LanguageRepository.GetAll();
        }

        public Language GetLanguageById(int languageId)
        {
            return _LanguageRepository.GetSingle(d => d.LanguageID.Equals(languageId));
        }

        public Language GetLanguageByCountryCode(string countryCode)
        {
            return _LanguageRepository.GetSingle(d => d.CountryCode.ToLower().Equals(countryCode.ToLower()));
        }

        public Language GetLanguageByLICD(int licd)
        {
            return _LanguageRepository.GetSingle(d => d.LICDNumber.Equals(licd));
        }

        public void AddLanguage(params Language[] languages)
        {
            /* Validation and error handling omitted */

            // Fill createdate and createuser fields
            foreach (var item in languages)
            {
                item.CreateDate = DateTime.Now;
                item.CreateUser = _CurrentUser;
            }

            _LanguageRepository.Add(languages);
        }

        public void UpdateLanguage(params Language[] languages)
        {
            /* Validation and error handling omitted */

            // Fill updatedate and updateuser fields
            foreach (var item in languages)
            {
                item.UpdateDate = DateTime.Now;
                item.UpdateUser = _CurrentUser;
            }

            _LanguageRepository.Update(languages);
        }

        public void RemoveLanguage(params Language[] languages)
        {
            /* Validation and error handling omitted */
            _LanguageRepository.Remove(languages);
        }
    }
}
