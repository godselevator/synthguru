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
        IList<DocArcActivity> GetAllDocArcActivities();
        IList<DocArcActivity> GetDocArcActivitiesByAccountId(int accountId);
        DocArcActivity GetDocArcActivityById(int id);
        void AddDocArcActivity(params DocArcActivity[] DocArcActivitys);
        void UpdateDocArcActivity(params DocArcActivity[] DocArcActivitys);
        void RemoveDocArcActivity(params DocArcActivity[] DocArcActivitys);
    }

    public partial class BusinessLayer : IBusinessLayer
    {
        public IList<DocArcActivity> GetAllDocArcActivities()
        {
            return _DocArcActivityRepository.GetAll();
        }

        public IList<DocArcActivity> GetDocArcActivitiesByAccountId(int accountId)
        {
            return _DocArcActivityRepository.GetList(d => d.AccountId.Equals(accountId));
        }

        public DocArcActivity GetDocArcActivityById(int id)
        {
            return _DocArcActivityRepository.GetSingle(d => d.ItemId.Equals(id));
        }

        public void AddDocArcActivity(params DocArcActivity[] DocArcActivitys)
        {
            /* Validation and error handling omitted */
            _DocArcActivityRepository.Add(DocArcActivitys);
        }

        public void UpdateDocArcActivity(params DocArcActivity[] DocArcActivitys)
        {
            /* Validation and error handling omitted */
            _DocArcActivityRepository.Update(DocArcActivitys);
        }

        public void RemoveDocArcActivity(params DocArcActivity[] DocArcActivitys)
        {
            /* Validation and error handling omitted */
            _DocArcActivityRepository.Remove(DocArcActivitys);
        }
    }
}
