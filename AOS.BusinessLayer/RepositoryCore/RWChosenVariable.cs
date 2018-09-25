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
        IList<RWChosenVariable> GetAllRWChosenVariables();
        RWChosenVariable GetRWChosenVariableById(int RWChosenVariableId);
        IList<RWChosenVariable> GetRWChosenVariableByAccount(int accountId);
        void AddRWChosenVariable(params RWChosenVariable[] rwChosenVariables);
        void UpdateRWChosenVariable(params RWChosenVariable[] rwChosenVariables);
        void RemoveRWChosenVariable(params RWChosenVariable[] rwChosenVariables);
    }

    public partial class BusinessLayer : IBusinessLayer
    {
        public IList<RWChosenVariable> GetAllRWChosenVariables()
        {
            return _RWChosenVariableRepository.GetAll();
        }

        public RWChosenVariable GetRWChosenVariableById(int RWChosenVariableId)
        {
            return _RWChosenVariableRepository.GetSingle(d => d.RWChosenVariableID.Equals(RWChosenVariableId));
        }

        public IList<RWChosenVariable> GetRWChosenVariableByAccount(int accountId)
        {
            return _RWChosenVariableRepository.GetList(d => d.AccountID.Equals(accountId));
        }

        public void AddRWChosenVariable(params RWChosenVariable[] rwChosenVariables)
        {
            /* Validation and error handling omitted */

            // Fill createdate and createuser fields
            foreach (var item in rwChosenVariables)
            {
                item.CreateDate = DateTime.Now;
                item.CreateUser = _CurrentUser;
            }

            _RWChosenVariableRepository.Add(rwChosenVariables);
        }

        public void UpdateRWChosenVariable(params RWChosenVariable[] rwChosenVariables)
        {
            /* Validation and error handling omitted */

            // Fill updatedate and updateuser fields
            foreach (var item in rwChosenVariables)
            {
                item.UpdateDate = DateTime.Now;
                item.UpdateUser = _CurrentUser;
            }

            _RWChosenVariableRepository.Update(rwChosenVariables);
        }

        public void RemoveRWChosenVariable(params RWChosenVariable[] rwChosenVariables)
        {
            /* Validation and error handling omitted */
            _RWChosenVariableRepository.Remove(rwChosenVariables);
        }
    }
}
