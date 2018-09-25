using AOS.DomainModel;
using System;
using System.Collections.Generic;

namespace AOS.BusinessLayer
{
    public partial interface IBusinessLayer
    {
        IList<SignicatSignMethod> GetAllSignicatSignMethods();
        SignicatSignMethod GetSignicatSignMethodById(int id);
        void AddSignicatSignMethod(params SignicatSignMethod[] signicatSignMethods);
        void UpdateSignicatSignMethod(params SignicatSignMethod[] signicatSignMethods);
        void RemoveSignicatSignMethod(params SignicatSignMethod[] signicatSignMethods);
    }

    public partial class BusinessLayer : IBusinessLayer
    {
        public IList<SignicatSignMethod> GetAllSignicatSignMethods()
        {
            return _SignicatSignMethodRepository.GetAll();
        }

        public SignicatSignMethod GetSignicatSignMethodById(int id)
        {
            return _SignicatSignMethodRepository.GetSingle(d => d.SignicatSignMethodID.Equals(id));
        }

        public void AddSignicatSignMethod(params SignicatSignMethod[] signicatSignMethods)
        {
            /* Validation and error handling omitted */

            // Set createdate and createuser
            foreach (var item in signicatSignMethods)
            {
                item.CreateDate = DateTime.Now;
                item.CreateUser = _CurrentUser;
            }

            _SignicatSignMethodRepository.Add(signicatSignMethods);
        }

        public void UpdateSignicatSignMethod(params SignicatSignMethod[] signicatSignMethods)
        {
            /* Validation and error handling omitted */
            foreach (var item in signicatSignMethods)
            {
                item.UpdateDate = DateTime.Now;
                item.UpdateUser = _CurrentUser;
            }

            _SignicatSignMethodRepository.Update(signicatSignMethods);
        }

        public void RemoveSignicatSignMethod(params SignicatSignMethod[] signicatSignMethods)
        {
            /* Validation and error handling omitted */
            _SignicatSignMethodRepository.Remove(signicatSignMethods);
        }
    }
}
