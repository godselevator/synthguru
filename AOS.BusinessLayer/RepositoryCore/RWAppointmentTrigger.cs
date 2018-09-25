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
        IList<RWAppointmentTrigger> GetAllRWAppointmentTriggers();
        RWAppointmentTrigger GetRWAppointmentTriggerById(int rwAppointmentTriggerId);
        IList<RWAppointmentTrigger> GetRWAppointmentTriggerByAccount(int accountId);
        void AddRWAppointmentTrigger(params RWAppointmentTrigger[] rwAppointmentTriggers);
        void UpdateRWAppointmentTrigger(params RWAppointmentTrigger[] rwAppointmentTriggers);
        void RemoveRWAppointmentTrigger(params RWAppointmentTrigger[] rwAppointmentTriggers);
    }

    public partial class BusinessLayer : IBusinessLayer
    {
        public IList<RWAppointmentTrigger> GetAllRWAppointmentTriggers()
        {
            return _RWAppointmentTriggerRepository.GetAll();
        }

        public RWAppointmentTrigger GetRWAppointmentTriggerById(int rwAppointmentTriggerId)
        {
            return _RWAppointmentTriggerRepository.GetSingle(d => d.RWAppointmentTriggerID.Equals(rwAppointmentTriggerId));
        }

        public IList<RWAppointmentTrigger> GetRWAppointmentTriggerByAccount(int accountId)
        {
            return _RWAppointmentTriggerRepository.GetList(d => d.AccountID.Equals(accountId));
        }

        public void AddRWAppointmentTrigger(params RWAppointmentTrigger[] rwAppointmentTriggers)
        {
            /* Validation and error handling omitted */

            // Fill createdate and createuser fields
            foreach (var item in rwAppointmentTriggers)
            {
                item.CreateDate = DateTime.Now;
                item.CreateUser = _CurrentUser;
            }

            _RWAppointmentTriggerRepository.Add(rwAppointmentTriggers);
        }

        public void UpdateRWAppointmentTrigger(params RWAppointmentTrigger[] rwAppointmentTriggers)
        {
            /* Validation and error handling omitted */

            // Fill updatedate and updateuser fields
            foreach (var item in rwAppointmentTriggers)
            {
                item.UpdateDate = DateTime.Now;
                item.UpdateUser = _CurrentUser;
            }

            _RWAppointmentTriggerRepository.Update(rwAppointmentTriggers);
        }

        public void RemoveRWAppointmentTrigger(params RWAppointmentTrigger[] rwAppointmentTriggers)
        {
            /* Validation and error handling omitted */
            _RWAppointmentTriggerRepository.Remove(rwAppointmentTriggers);
        }
    }
}
