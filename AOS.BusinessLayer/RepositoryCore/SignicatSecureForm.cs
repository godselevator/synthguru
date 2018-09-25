using AOS.DomainModel;
using System;
using System.Collections.Generic;

namespace AOS.BusinessLayer
{
    public partial interface IBusinessLayer
    {
        IList<SignicatSecureForm> GetAllSignicatSecureForms();
        SignicatSecureForm GetSignicatSecureFormById(int signicatSecureFormID);
        IList<SignicatSecureForm> GetSignicatSecureFormsByAccountId(int? accountId);
        void AddSignicatSecureForm(params SignicatSecureForm[] SignicatSecureForms);
        void UpdateSignicatSecureForm(params SignicatSecureForm[] SignicatSecureForms);
        void RemoveSignicatSecureForm(params SignicatSecureForm[] SignicatSecureForms);
    }

    public partial class BusinessLayer : IBusinessLayer
    {
        public IList<SignicatSecureForm> GetAllSignicatSecureForms()
        {
            return _SignicatSecureFormRepository.GetAll();
        }

        public SignicatSecureForm GetSignicatSecureFormById(int signicatSecureFormID)
        {
            return _SignicatSecureFormRepository.GetSingle(d => d.SignicatSecureFormID.Equals(signicatSecureFormID));
        }

        public IList<SignicatSecureForm> GetSignicatSecureFormsByAccountId(int? accountId)
        {
            return _SignicatSecureFormRepository.GetList(d => d.AccountID.Equals(accountId));
        }

        public void AddSignicatSecureForm(params SignicatSecureForm[] SignicatSecureForms)
        {
            /* Validation and error handling omitted */

            // Set createdate and createuser
            foreach (var item in SignicatSecureForms)
            {
                item.CreateDate = DateTime.Now;
                item.CreateUser = _CurrentUser;
            }

            _SignicatSecureFormRepository.Add(SignicatSecureForms);
        }

        public void UpdateSignicatSecureForm(params SignicatSecureForm[] SignicatSecureForms)
        {
            /* Validation and error handling omitted */
            foreach (var item in SignicatSecureForms)
            {
                item.UpdateDate = DateTime.Now;
                item.UpdateUser = _CurrentUser;
            }

            _SignicatSecureFormRepository.Update(SignicatSecureForms);
        }

        public void RemoveSignicatSecureForm(params SignicatSecureForm[] SignicatSecureForms)
        {
            /* Validation and error handling omitted */
            _SignicatSecureFormRepository.Remove(SignicatSecureForms);
        }
    }
}
