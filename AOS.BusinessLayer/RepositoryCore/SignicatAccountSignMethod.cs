using AOS.DomainModel;
using System;
using System.Collections.Generic;

namespace AOS.BusinessLayer
{
    public partial interface IBusinessLayer
    {
        IList<SignicatAccountSignMethod> GetAllSignicatAccountSignMethods();
        SignicatAccountSignMethod GetSignicatAccountSignMethodById(int id);
        IList<SignicatAccountSignMethod> GetSignicatAccountSignMethodByAccountId(int? accountId);
        IList<SignicatAccountSignMethod> GetSignicatAccountSignMethodByMethodId(int methodId);
        void AddSignicatAccountSignMethod(params SignicatAccountSignMethod[] signicatAccountSignMethods);
        void UpdateSignicatAccountSignMethod(params SignicatAccountSignMethod[] signicatAccountSignMethods);
        void RemoveSignicatAccountSignMethod(params SignicatAccountSignMethod[] signicatAccountSignMethods);
    }

    public partial class BusinessLayer : IBusinessLayer
    {
        public IList<SignicatAccountSignMethod> GetAllSignicatAccountSignMethods()
        {
            return _SignicatAccountSignMethodRepository.GetAll();
        }

        public SignicatAccountSignMethod GetSignicatAccountSignMethodById(int id)
        {
            return _SignicatAccountSignMethodRepository.GetSingle(d => d.SignicatAccountSignMethodID.Equals(id));
        }

        public IList<SignicatAccountSignMethod> GetSignicatAccountSignMethodByAccountId(int? accountId)
        {
            return _SignicatAccountSignMethodRepository.GetList(d => d.AccountID.Equals(accountId), 
                d => d.Account, 
                d => d.SignicatSignMethod);
        }

        public IList<SignicatAccountSignMethod> GetSignicatAccountSignMethodByMethodId(int methodId)
        {
            return _SignicatAccountSignMethodRepository.GetList(d => d.SignicatSignMethodID.Equals(methodId));
        }

        public void AddSignicatAccountSignMethod(params SignicatAccountSignMethod[] signicatAccountSignMethods)
        {
            /* Validation and error handling omitted */

            // Set createdate and createuser
            foreach (var item in signicatAccountSignMethods)
            {
                item.CreateDate = DateTime.Now;
                item.CreateUser = _CurrentUser;
            }

            _SignicatAccountSignMethodRepository.Add(signicatAccountSignMethods);
        }

        public void UpdateSignicatAccountSignMethod(params SignicatAccountSignMethod[] signicatAccountSignMethods)
        {
            /* Validation and error handling omitted */
            foreach (var item in signicatAccountSignMethods)
            {
                item.UpdateDate = DateTime.Now;
                item.UpdateUser = _CurrentUser;
            }

            _SignicatAccountSignMethodRepository.Update(signicatAccountSignMethods);
        }

        public void RemoveSignicatAccountSignMethod(params SignicatAccountSignMethod[] signicatAccountSignMethods)
        {
            /* Validation and error handling omitted */
            _SignicatAccountSignMethodRepository.Remove(signicatAccountSignMethods);
        }
    }
}
