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
        IList<RWAppointmentTask> GetAllRWAppointmentTasks();
        RWAppointmentTask GetRWAppointmentTaskById(int rwAppointmentTaskId);
        IList<RWAppointmentTask> GetRWAppointmentTaskByAccount(int accountId);
        void AddRWAppointmentTask(params RWAppointmentTask[] rwAppointmentTasks);
        void UpdateRWAppointmentTask(params RWAppointmentTask[] rwAppointmentTasks);
        void RemoveRWAppointmentTask(params RWAppointmentTask[] rwAppointmentTasks);
    }

    public partial class BusinessLayer : IBusinessLayer
    {
        public IList<RWAppointmentTask> GetAllRWAppointmentTasks()
        {
            return _RWAppointmentTaskRepository.GetAll();
        }

        public RWAppointmentTask GetRWAppointmentTaskById(int rwAppointmentTaskId)
        {
            return _RWAppointmentTaskRepository.GetSingle(d => d.RWAppointmentTaskID.Equals(rwAppointmentTaskId));
        }

        public IList<RWAppointmentTask> GetRWAppointmentTaskByAccount(int accountId)
        {
            return _RWAppointmentTaskRepository.GetList(d => d.AccountID.Equals(accountId));
        }

        public void AddRWAppointmentTask(params RWAppointmentTask[] rwAppointmentTasks)
        {
            /* Validation and error handling omitted */

            // Fill createdate and createuser fields
            foreach (var item in rwAppointmentTasks)
            {
                item.CreateDate = DateTime.Now;
                item.CreateUser = _CurrentUser;
            }

            _RWAppointmentTaskRepository.Add(rwAppointmentTasks);
        }

        public void UpdateRWAppointmentTask(params RWAppointmentTask[] rwAppointmentTasks)
        {
            /* Validation and error handling omitted */

            // Fill updatedate and updateuser fields
            foreach (var item in rwAppointmentTasks)
            {
                item.UpdateDate = DateTime.Now;
                item.UpdateUser = _CurrentUser;
            }

            _RWAppointmentTaskRepository.Update(rwAppointmentTasks);
        }

        public void RemoveRWAppointmentTask(params RWAppointmentTask[] rwAppointmentTasks)
        {
            /* Validation and error handling omitted */
            _RWAppointmentTaskRepository.Remove(rwAppointmentTasks);
        }
    }
}
