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
        IList<RelationWise> GetAllRelationWises();
        RelationWise GetRelationWiseById(int relationWiseId);
        IList<RelationWise> GetrelationWisesByAccount(int accountId);
        void AddRelationWise(params RelationWise[] relationWises);
        void UpdateRelationWise(params RelationWise[] relationWises);
        void RemoveRelationWise(params RelationWise[] relationWises);
    }

    public partial class BusinessLayer : IBusinessLayer
    {
        public IList<RelationWise> GetAllRelationWises()
        {
            return _RelationWiseRepository.GetAll();
        }

        public RelationWise GetRelationWiseById(int relationWiseId)
        {
            return _RelationWiseRepository.GetSingle(d => d.RelationWiseID.Equals(relationWiseId));
        }

        public IList<RelationWise> GetrelationWisesByAccount(int accountId)
        {
            return _RelationWiseRepository.GetList(d => d.RelationWiseID.Equals(accountId));
        }

        public void AddRelationWise(params RelationWise[] relationWises)
        {
            /* Validation and error handling omitted */

            // Fill createdate and createuser fields
            foreach (var item in relationWises)
            {
                item.CreateDate = DateTime.Now;
                item.CreateUser = _CurrentUser;
            }

            _RelationWiseRepository.Add(relationWises);
        }

        public void UpdateRelationWise(params RelationWise[] relationWises)
        {
            /* Validation and error handling omitted */

            // Fill updatedate and updateuser fields
            foreach (var item in relationWises)
            {
                item.UpdateDate = DateTime.Now;
                item.UpdateUser = _CurrentUser;
            }

            _RelationWiseRepository.Update(relationWises);
        }

        public void RemoveRelationWise(params RelationWise[] relationWises)
        {
            /* Validation and error handling omitted */
            _RelationWiseRepository.Remove(relationWises);
        }
    }
}
