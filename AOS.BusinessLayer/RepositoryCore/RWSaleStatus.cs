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
        IList<RWSaleStatus> GetAllRWSaleStatuses();
        RWSaleStatus GetRWSaleStatusById(int rwSaleStageId);
        void AddRWSaleStatus(params RWSaleStatus[] rwSaleStatuses);
        void UpdateRWSaleStatus(params RWSaleStatus[] rwSaleStatuses);
        void RemoveRWSaleStatus(params RWSaleStatus[] rwSaleStatuses);
    }   

    public partial class BusinessLayer : IBusinessLayer
    {
        public IList<RWSaleStatus> GetAllRWSaleStatuses()
        {
            return _RWSaleStatusRepository.GetAll();
        }

        public RWSaleStatus GetRWSaleStatusById(int rwSaleStageId)
        {
            return _RWSaleStatusRepository.GetSingle(d => d.RWSaleStageID.Equals(rwSaleStageId));
        }

        public void AddRWSaleStatus(params RWSaleStatus[] rwSaleStatuses)
        {
            /* Validation and error handling omitted */

            // Fill createdate and createuser fields
            foreach (var item in rwSaleStatuses)
            {
                item.CreateDate = DateTime.Now;
                item.CreateUser = _CurrentUser;
            }

            _RWSaleStatusRepository.Add(rwSaleStatuses);
        }

        public void UpdateRWSaleStatus(params RWSaleStatus[] rwSaleStatuses)
        {
            /* Validation and error handling omitted */

            // Fill updatedate and updateuser fields
            foreach (var item in rwSaleStatuses)
            {
                item.UpdateDate = DateTime.Now;
                item.UpdateUser = _CurrentUser;
            }

            _RWSaleStatusRepository.Update(rwSaleStatuses);
        }

        public void RemoveRWSaleStatus(params RWSaleStatus[] rwSaleStatuses)
        {
            /* Validation and error handling omitted */
            _RWSaleStatusRepository.Remove(rwSaleStatuses);
        }
    }
}
