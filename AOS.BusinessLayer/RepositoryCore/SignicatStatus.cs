using AOS.DomainModel;
using System;
using System.Collections.Generic;

namespace AOS.BusinessLayer
{
    public partial interface IBusinessLayer
    {
        IList<SignicatStatus> GetAllSignicatStatuseses();
        SignicatStatus GetSignicatStatusesById(int id);
        void AddSignicatStatuses(params SignicatStatus[] SignicatStatuses);
        void UpdateSignicatStatuses(params SignicatStatus[] SignicatStatuses);
        void RemoveSignicatStatuses(params SignicatStatus[] SignicatStatuses);
    }

    public partial class BusinessLayer : IBusinessLayer
    {
        public IList<SignicatStatus> GetAllSignicatStatuseses()
        {
            return _SignicatStatusRepository.GetAll();
        }

        public SignicatStatus GetSignicatStatusesById(int id)
        {
            return _SignicatStatusRepository.GetSingle(d => d.SignicatStatusID.Equals(id));
        }

        public void AddSignicatStatuses(params SignicatStatus[] SignicatStatuses)
        {
            /* Validation and error handling omitted */

            // Set createdate and createuser
            foreach (var item in SignicatStatuses)
            {
                item.CreateDate = DateTime.Now;
                item.CreateUser = _CurrentUser;
            }

            _SignicatStatusRepository.Add(SignicatStatuses);
        }

        public void UpdateSignicatStatuses(params SignicatStatus[] SignicatStatuses)
        {
            /* Validation and error handling omitted */
            foreach (var item in SignicatStatuses)
            {
                item.UpdateDate = DateTime.Now;
                item.UpdateUser = _CurrentUser;
            }

            _SignicatStatusRepository.Update(SignicatStatuses);
        }

        public void RemoveSignicatStatuses(params SignicatStatus[] SignicatStatuses)
        {
            /* Validation and error handling omitted */
            _SignicatStatusRepository.Remove(SignicatStatuses);
        }
    }
}
