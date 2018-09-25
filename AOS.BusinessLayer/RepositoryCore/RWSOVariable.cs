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
        IList<RWSOVariable> GetAllRWSOVariables();
        RWSOVariable GetRWSOVariableById(int RWSOVariableId);
        IList<RWSOVariable> GetRWSOVariableByCode(string code);
        void AddRWSOVariable(params RWSOVariable[] rwSOVariables);
        void UpdateRWSOVariable(params RWSOVariable[] rwSOVariables);
        void RemoveRWSOVariable(params RWSOVariable[] rwSOVariables);
    }

    public partial class BusinessLayer : IBusinessLayer
    {
        public IList<RWSOVariable> GetAllRWSOVariables()
        {
            return _RWSOVariableRepository.GetAll();
        }

        public RWSOVariable GetRWSOVariableById(int RWSOVariableId)
        {
            return _RWSOVariableRepository.GetSingle(d => d.RWVariableID.Equals(RWSOVariableId));
        }

        public IList<RWSOVariable> GetRWSOVariableByCode(string code)
        {
            return _RWSOVariableRepository.GetList(d => d.Code.ToLower().Equals(code.ToLower()));
        }

        public void AddRWSOVariable(params RWSOVariable[] rwSOVariables)
        {
            /* Validation and error handling omitted */

            // Fill createdate and createuser fields
            foreach (var item in rwSOVariables)
            {
                item.CreateDate = DateTime.Now;
                item.CreateUser = _CurrentUser;
            }

            _RWSOVariableRepository.Add(rwSOVariables);
        }

        public void UpdateRWSOVariable(params RWSOVariable[] rwSOVariables)
        {
            /* Validation and error handling omitted */

            // Fill updatedate and updateuser fields
            foreach (var item in rwSOVariables)
            {
                item.UpdateDate = DateTime.Now;
                item.UpdateUser = _CurrentUser;
            }

            _RWSOVariableRepository.Update(rwSOVariables);
        }

        public void RemoveRWSOVariable(params RWSOVariable[] rwSOVariables)
        {
            /* Validation and error handling omitted */
            _RWSOVariableRepository.Remove(rwSOVariables);
        }
    }
}
