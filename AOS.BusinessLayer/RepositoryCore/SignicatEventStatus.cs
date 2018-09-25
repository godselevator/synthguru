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
        IList<SignicatEventStatus> GetAllSignicatEventStatuses();
        IList<SignicatEventStatus> GetSignicatEventStatusByAccountId(int accountId);
        IList<SignicatEventStatus> GetSignicatEventStatusByRequestId(string requestId);
        IList<SignicatEventStatus> GetSignicatEventStatusByRequestIdAndTaskId(string requestId, string taskId);
        void AddSignicatEventStatus(params SignicatEventStatus[] SignicatEventStatuss);
        void UpdateSignicatEventStatus(params SignicatEventStatus[] SignicatEventStatuss);
        void RemoveSignicatEventStatus(params SignicatEventStatus[] SignicatEventStatuss);
    }

    public partial class BusinessLayer : IBusinessLayer
    {
        public IList<SignicatEventStatus> GetAllSignicatEventStatuses()
        {
            return _SignicatEventStatusRepository.GetAll();
        }

        public IList<SignicatEventStatus> GetSignicatEventStatusByAccountId(int accountId)
        {
            return _SignicatEventStatusRepository.GetList(d => d.AccountId.Equals(accountId));
        }

        public IList<SignicatEventStatus> GetSignicatEventStatusByRequestId(string requestId)
        {
            return _SignicatEventStatusRepository.GetList(d => d.RequestId.Equals(requestId));
        }

        public IList<SignicatEventStatus> GetSignicatEventStatusByRequestIdAndTaskId(string requestId, string taskId)
        {
            return _SignicatEventStatusRepository.GetList(d => d.RequestId.Equals(requestId) && d.TaskId.Equals(taskId));
        }

        public void AddSignicatEventStatus(params SignicatEventStatus[] SignicatEventStatuss)
        {
            /* Validation and error handling omitted */
            _SignicatEventStatusRepository.Add(SignicatEventStatuss);
        }

        public void UpdateSignicatEventStatus(params SignicatEventStatus[] SignicatEventStatuss)
        {
            /* Validation and error handling omitted */
            _SignicatEventStatusRepository.Update(SignicatEventStatuss);
        }

        public void RemoveSignicatEventStatus(params SignicatEventStatus[] SignicatEventStatuss)
        {
            /* Validation and error handling omitted */
            _SignicatEventStatusRepository.Remove(SignicatEventStatuss);
        }
    }
}
