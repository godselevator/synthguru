using AOS.DomainModel;
using System;
using System.Collections.Generic;

namespace AOS.BusinessLayer
{
    public partial interface IBusinessLayer
    {
        IList<SignicatLanguage> GetAllSignicatLanguages();
        SignicatLanguage GetSignicatLanguageById(int id);
        void AddSignicatLanguage(params SignicatLanguage[] signicatLanguages);
        void UpdateSignicatLanguage(params SignicatLanguage[] signicatLanguages);
        void RemoveSignicatLanguage(params SignicatLanguage[] signicatLanguages);
    }

    public partial class BusinessLayer : IBusinessLayer
    {
        public IList<SignicatLanguage> GetAllSignicatLanguages()
        {
            return _SignicatLanguageRepository.GetAll();
        }

        public SignicatLanguage GetSignicatLanguageById(int id)
        {
            return _SignicatLanguageRepository.GetSingle(d => d.SignicatLanguageID.Equals(id));
        }

        public void AddSignicatLanguage(params SignicatLanguage[] signicatLanguages)
        {
            /* Validation and error handling omitted */

            // Set createdate and createuser
            foreach (var item in signicatLanguages)
            {
                item.CreateDate = DateTime.Now;
                item.CreateUser = _CurrentUser;
            }

            _SignicatLanguageRepository.Add(signicatLanguages);
        }

        public void UpdateSignicatLanguage(params SignicatLanguage[] signicatLanguages)
        {
            /* Validation and error handling omitted */
            foreach (var item in signicatLanguages)
            {
                item.UpdateDate = DateTime.Now;
                item.UpdateUser = _CurrentUser;
            }

            _SignicatLanguageRepository.Update(signicatLanguages);
        }

        public void RemoveSignicatLanguage(params SignicatLanguage[] signicatLanguages)
        {
            /* Validation and error handling omitted */
            _SignicatLanguageRepository.Remove(signicatLanguages);
        }
    }
}
