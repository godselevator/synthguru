using AOS.DomainModel;
using System;
using System.Collections.Generic;

namespace AOS.BusinessLayer
{
    public partial interface IBusinessLayer
    {
        IList<SignicatLogging> GetAllSignicatLoggings();
        SignicatLogging GetSignicatLoggingById(int id);
        IList<SignicatLogging> GetSignicatLoggingByAccountId(int? accountId);
        void AddSignicatLogging(params SignicatLogging[] signicatLoggings);
        void UpdateSignicatLogging(params SignicatLogging[] signicatLoggings);
        void RemoveSignicatLogging(params SignicatLogging[] signicatLoggings);
    }

    public partial class BusinessLayer : IBusinessLayer
    {
        public IList<SignicatLogging> GetAllSignicatLoggings()
        {
            return _SignicatLoggingRepository.GetAll();
        }

        public SignicatLogging GetSignicatLoggingById(int id)
        {
            return _SignicatLoggingRepository.GetSingle(d => d.SignicatLoggingID.Equals(id));
        }

        public IList<SignicatLogging> GetSignicatLoggingByAccountId(int? accountId)
        {
            return _SignicatLoggingRepository.GetList(d => d.AccountID.Equals(accountId));
        }

        public void AddSignicatLogging(params SignicatLogging[] signicatLoggings)
        {
            /* Validation and error handling omitted */

            // Set createdate and createuser
            foreach (var item in signicatLoggings)
            {
                item.CreateDate = DateTime.Now;
                item.CreateUser = _CurrentUser;
            }

            _SignicatLoggingRepository.Add(signicatLoggings);
        }

        public void UpdateSignicatLogging(params SignicatLogging[] signicatLoggings)
        {
            /* Validation and error handling omitted */
            foreach (var item in signicatLoggings)
            {
                item.UpdateDate = DateTime.Now;
                item.UpdateUser = _CurrentUser;
            }

            _SignicatLoggingRepository.Update(signicatLoggings);
        }

        public void RemoveSignicatLogging(params SignicatLogging[] signicatLoggings)
        {
            /* Validation and error handling omitted */
            _SignicatLoggingRepository.Remove(signicatLoggings);
        }
    }
}
