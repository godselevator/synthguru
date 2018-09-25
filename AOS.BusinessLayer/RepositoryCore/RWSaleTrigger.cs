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
        IList<RWSaleTrigger> GetAllRWSaleTriggers();
        RWSaleTrigger GetRWSaleTriggerById(int RWSaleTriggerId);
        IList<RWSaleTrigger> GetRWSaleTriggerByAccount(int accountId);
        void AddRWSaleTrigger(params RWSaleTrigger[] rwSaleTriggers);
        void UpdateRWSaleTrigger(params RWSaleTrigger[] rwSaleTriggers);
        void RemoveRWSaleTrigger(params RWSaleTrigger[] rwSaleTriggers);
    }

    public partial class BusinessLayer : IBusinessLayer
    {
        public IList<RWSaleTrigger> GetAllRWSaleTriggers()
        {
            return _RWSaleTriggerRepository.GetAll();
        }

        public RWSaleTrigger GetRWSaleTriggerById(int RWSaleTriggerId)
        {
            return _RWSaleTriggerRepository.GetSingle(d => d.RWSaleTriggerID.Equals(RWSaleTriggerId));
        }

        public IList<RWSaleTrigger> GetRWSaleTriggerByAccount(int accountId)
        {
            return _RWSaleTriggerRepository.GetList(d => d.AccountID.Equals(accountId));
        }

        public void AddRWSaleTrigger(params RWSaleTrigger[] rwSaleTriggers)
        {
            /* Validation and error handling omitted */

            // Fill createdate and createuser fields
            foreach (var item in rwSaleTriggers)
            {
                item.CreateDate = DateTime.Now;
                item.CreateUser = _CurrentUser;
            }

            _RWSaleTriggerRepository.Add(rwSaleTriggers);
        }

        public void UpdateRWSaleTrigger(params RWSaleTrigger[] rwSaleTriggers)
        {
            /* Validation and error handling omitted */

            // Fill updatedate and updateuser fields
            foreach (var item in rwSaleTriggers)
            {
                item.UpdateDate = DateTime.Now;
                item.UpdateUser = _CurrentUser;
            }

            _RWSaleTriggerRepository.Update(rwSaleTriggers);
        }

        public void RemoveRWSaleTrigger(params RWSaleTrigger[] rwSaleTriggers)
        {
            /* Validation and error handling omitted */
            _RWSaleTriggerRepository.Remove(rwSaleTriggers);
        }
    }
}
