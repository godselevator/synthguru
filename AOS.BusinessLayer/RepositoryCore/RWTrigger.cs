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
        IList<RWTrigger> GetAllRWTriggers();
        RWTrigger GetRWTriggerById(int RWTriggerId);
        IList<RWTrigger> GetRWTriggerByAccount(int accountId);
        void AddRWTrigger(params RWTrigger[] rwTriggers);
        void UpdateRWTrigger(params RWTrigger[] rwTriggers);
        void RemoveRWTrigger(params RWTrigger[] rwTriggers);
    }

    public partial class BusinessLayer : IBusinessLayer
    {
        public IList<RWTrigger> GetAllRWTriggers()
        {
            return _RWTriggerRepository.GetAll();
        }

        public RWTrigger GetRWTriggerById(int RWTriggerId)
        {
            return _RWTriggerRepository.GetSingle(d => d.RWTriggerID.Equals(RWTriggerId));
        }

        public IList<RWTrigger> GetRWTriggerByAccount(int accountId)
        {
            return _RWTriggerRepository.GetList(d => d.AccountID.Equals(accountId));
        }

        public void AddRWTrigger(params RWTrigger[] rwTriggers)
        {
            /* Validation and error handling omitted */

            // Fill createdate and createuser fields
            foreach (var item in rwTriggers)
            {
                item.CreateDate = DateTime.Now;
                item.CreateUser = _CurrentUser;
            }

            _RWTriggerRepository.Add(rwTriggers);
        }

        public void UpdateRWTrigger(params RWTrigger[] rwTriggers)
        {
            /* Validation and error handling omitted */

            // Fill updatedate and updateuser fields
            foreach (var item in rwTriggers)
            {
                item.UpdateDate = DateTime.Now;
                item.UpdateUser = _CurrentUser;
            }

            _RWTriggerRepository.Update(rwTriggers);
        }

        public void RemoveRWTrigger(params RWTrigger[] rwTriggers)
        {
            /* Validation and error handling omitted */
            _RWTriggerRepository.Remove(rwTriggers);
        }
    }
}
