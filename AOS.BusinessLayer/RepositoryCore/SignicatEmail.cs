using AOS.DomainModel;
using System;
using System.Collections.Generic;

namespace AOS.BusinessLayer
{
    public partial interface IBusinessLayer
    {
        IList<SignicatEmail> GetAllSignicatEmails();
        SignicatEmail GetSignicatEmailById(int id);
        IList<SignicatEmail> GetSignicatEmailByAccountId(int? accountId);
        void AddSignicatEmail(params SignicatEmail[] signicatEmails);
        void UpdateSignicatEmail(params SignicatEmail[] signicatEmails);
        void RemoveSignicatEmail(params SignicatEmail[] signicatEmails);
    }

    public partial class BusinessLayer : IBusinessLayer
    {
        public IList<SignicatEmail> GetAllSignicatEmails()
        {
            return _SignicatEmailRepository.GetAll();
        }

        public SignicatEmail GetSignicatEmailById(int id)
        {
            return _SignicatEmailRepository.GetSingle(d => d.SignicatEmailID.Equals(id));
        }

        public IList<SignicatEmail> GetSignicatEmailByAccountId(int? accountId)
        {
            return _SignicatEmailRepository.GetList(d => d.AccountID.Equals(accountId));
        }

        public void AddSignicatEmail(params SignicatEmail[] signicatEmails)
        {
            /* Validation and error handling omitted */

            // Set createdate and createuser
            foreach (var item in signicatEmails)
            {
                item.CreateDate = DateTime.Now;
                item.CreateUser = _CurrentUser;
            }

            _SignicatEmailRepository.Add(signicatEmails);
        }

        public void UpdateSignicatEmail(params SignicatEmail[] signicatEmails)
        {
            /* Validation and error handling omitted */
            foreach (var item in signicatEmails)
            {
                item.UpdateDate = DateTime.Now;
                item.UpdateUser = _CurrentUser;
            }

            _SignicatEmailRepository.Update(signicatEmails);
        }

        public void RemoveSignicatEmail(params SignicatEmail[] signicatEmails)
        {
            /* Validation and error handling omitted */
            _SignicatEmailRepository.Remove(signicatEmails);
        }
    }
}
