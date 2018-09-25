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
        IList<SignicatEventLog> GetAllSignicatEventLogs();
        IList<SignicatEventLog> GetSignicatEventLogsByAccountId(int accountId);
        IList<SignicatEventLog> GetSignicatEventLogsByRequestId(string requestId);
        IList<SignicatEventLog> GetSignicatEventLogsByRequestIdAndTaskId(string requestId, string taskId);
        void AddSignicatEventLog(params SignicatEventLog[] SignicatEventLogs);
        void UpdateSignicatEventLog(params SignicatEventLog[] SignicatEventLogs);
        void RemoveSignicatEventLog(params SignicatEventLog[] SignicatEventLogs);
    }

    public partial class BusinessLayer : IBusinessLayer
    {
        public IList<SignicatEventLog> GetAllSignicatEventLogs()
        {
            return _SignicatEventLogRepository.GetAll();
        }

        public IList<SignicatEventLog> GetSignicatEventLogsByAccountId(int accountId)
        {
            return _SignicatEventLogRepository.GetList(d => d.AccountId.Equals(accountId));
        }

        public IList<SignicatEventLog> GetSignicatEventLogsByRequestId(string requestId)
        {
            return _SignicatEventLogRepository.GetList(d => d.RequestId.Equals(requestId));
        }

        public IList<SignicatEventLog> GetSignicatEventLogsByRequestIdAndTaskId(string requestId, string taskId)
        {
            return _SignicatEventLogRepository.GetList(d => d.RequestId.Equals(requestId) && d.TaskId.Equals(taskId));
        }

        public void AddSignicatEventLog(params SignicatEventLog[] SignicatEventLogs)
        {
            /* Validation and error handling omitted */
            _SignicatEventLogRepository.Add(SignicatEventLogs);
        }

        public void UpdateSignicatEventLog(params SignicatEventLog[] SignicatEventLogs)
        {
            /* Validation and error handling omitted */
            _SignicatEventLogRepository.Update(SignicatEventLogs);
        }

        public void RemoveSignicatEventLog(params SignicatEventLog[] SignicatEventLogs)
        {
            /* Validation and error handling omitted */
            _SignicatEventLogRepository.Remove(SignicatEventLogs);
        }
    }
}
