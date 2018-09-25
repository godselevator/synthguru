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
        IList<RWAppointmentStatus> GetAllRWAppointmentStatuses();
        RWAppointmentStatus GetRWAppointmentStatusById(int rwAppointmentStatusId);
        void AddRWAppointmentStatus(params RWAppointmentStatus[] rwAppointmentStatuses);
        void UpdateRWAppointmentStatus(params RWAppointmentStatus[] rwAppointmentStatuses);
        void RemoveRWAppointmentStatus(params RWAppointmentStatus[] rwAppointmentStatuses);
    }

    public partial class BusinessLayer : IBusinessLayer
    {
        public IList<RWAppointmentStatus> GetAllRWAppointmentStatuses()
        {
            return _RWAppointmentStatusRepository.GetAll();
        }

        public RWAppointmentStatus GetRWAppointmentStatusById(int rwAppointmentStatusId)
        {
            return _RWAppointmentStatusRepository.GetSingle(d => d.RWAppointmentStatusID.Equals(rwAppointmentStatusId));
        }

        public void AddRWAppointmentStatus(params RWAppointmentStatus[] rwAppointmentStatuses)
        {
            /* Validation and error handling omitted */

            // Fill createdate and createuser fields
            foreach (var item in rwAppointmentStatuses)
            {
                item.CreateDate = DateTime.Now;
                item.CreateUser = _CurrentUser;
            }

            _RWAppointmentStatusRepository.Add(rwAppointmentStatuses);
        }

        public void UpdateRWAppointmentStatus(params RWAppointmentStatus[] rwAppointmentStatuses)
        {
            /* Validation and error handling omitted */

            // Fill updatedate and updateuser fields
            foreach (var item in rwAppointmentStatuses)
            {
                item.UpdateDate = DateTime.Now;
                item.UpdateUser = _CurrentUser;
            }

            _RWAppointmentStatusRepository.Update(rwAppointmentStatuses);
        }

        public void RemoveRWAppointmentStatus(params RWAppointmentStatus[] rwAppointmentStatuses)
        {
            /* Validation and error handling omitted */
            _RWAppointmentStatusRepository.Remove(rwAppointmentStatuses);
        }
    }
}
