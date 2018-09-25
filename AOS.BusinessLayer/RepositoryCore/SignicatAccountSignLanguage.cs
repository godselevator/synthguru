using AOS.DomainModel;
using System;
using System.Collections.Generic;

namespace AOS.BusinessLayer
{
    public partial interface IBusinessLayer
    {
        IList<SignicatAccountSignLanguage> GetAllSignicatAccountSignLanguages();
        SignicatAccountSignLanguage GetSignicatAccountSignLanguageById(int id);
        IList<SignicatAccountSignLanguage> GetSignicatAccountSignLanguageByAccountId(int? accountId);
        IList<SignicatAccountSignLanguage> GetSignicatAccountSignLanguageByLanguageId(int languageId);
        void AddSignicatAccountSignLanguage(params SignicatAccountSignLanguage[] signicatAccountSignLanguages);
        void UpdateSignicatAccountSignLanguage(params SignicatAccountSignLanguage[] signicatAccountSignLanguages);
        void RemoveSignicatAccountSignLanguage(params SignicatAccountSignLanguage[] signicatAccountSignLanguages);
    }

    public partial class BusinessLayer : IBusinessLayer
    {
        public IList<SignicatAccountSignLanguage> GetAllSignicatAccountSignLanguages()
        {
            return _SignicatAccountSignLanguageRepository.GetAll();
        }

        public SignicatAccountSignLanguage GetSignicatAccountSignLanguageById(int id)
        {
            return _SignicatAccountSignLanguageRepository.GetSingle(d => d.SignicatAccountSignLanguageID.Equals(id));
        }

        public IList<SignicatAccountSignLanguage> GetSignicatAccountSignLanguageByAccountId(int? accountId)
        {
            return _SignicatAccountSignLanguageRepository.GetList(d => d.AccountID.Equals(accountId),
                d => d.Account,
                d => d.SignicatLanguage);
        }

        public IList<SignicatAccountSignLanguage> GetSignicatAccountSignLanguageByLanguageId(int languageId)
        {
            return _SignicatAccountSignLanguageRepository.GetList(d => d.LanguageID.Equals(languageId));
        }

        public void AddSignicatAccountSignLanguage(params SignicatAccountSignLanguage[] signicatAccountSignLanguages)
        {
            /* Validation and error handling omitted */

            // Set createdate and createuser
            foreach (var item in signicatAccountSignLanguages)
            {
                item.CreateDate = DateTime.Now;
                item.CreateUser = _CurrentUser;
            }

            _SignicatAccountSignLanguageRepository.Add(signicatAccountSignLanguages);
        }

        public void UpdateSignicatAccountSignLanguage(params SignicatAccountSignLanguage[] signicatAccountSignLanguages)
        {
            /* Validation and error handling omitted */
            foreach (var item in signicatAccountSignLanguages)
            {
                item.UpdateDate = DateTime.Now;
                item.UpdateUser = _CurrentUser;
            }

            _SignicatAccountSignLanguageRepository.Update(signicatAccountSignLanguages);
        }

        public void RemoveSignicatAccountSignLanguage(params SignicatAccountSignLanguage[] signicatAccountSignLanguages)
        {
            /* Validation and error handling omitted */
            _SignicatAccountSignLanguageRepository.Remove(signicatAccountSignLanguages);
        }
    }
}
