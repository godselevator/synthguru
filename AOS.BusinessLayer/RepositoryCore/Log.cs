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
        IList<Log> GetAllLogs();
        Log GetLogById(int LogId);
        IList<Log> GetLogByLogGroupId(int logTypeId);
        IList<Log> GetLogByAccountId(int accountId);
        IList<Log> GetLogByUserId(string userId);
        IList<Log> GetLogByAppId(int appId);
        IList<Log> GetLogByDateInterval(DateTime startDate, DateTime endDate);
        void AddLog(params Log[] Logs);
        void UpdateLog(params Log[] Logs);
        void RemoveLog(params Log[] Logs);
    }

    public partial class BusinessLayer : IBusinessLayer
    {
        public IList<Log> GetAllLogs()
        {
            return _LogRepository.GetAll();
        }

        public Log GetLogById(int LogId)
        {
            return _LogRepository.GetSingle(d => d.LogID.Equals(LogId));
        }

        public IList<Log> GetLogByLogGroupId(int logGroupId)
        {
            return _LogRepository.GetList(d => d.LogGroupID.Equals(logGroupId));
        }

        public IList<Log> GetLogByAccountId(int accountId)
        {
            return _LogRepository.GetList(d => d.AccountID.Equals(accountId));
        }

        public IList<Log> GetLogByUserId(string userId)
        {
            return _LogRepository.GetList(d => d.UserID.Equals(userId));
        }

        public IList<Log> GetLogByAppId(int appId)
        {
            return _LogRepository.GetList(d => d.AppID.Equals(appId));
        }

        public IList<Log> GetLogByDateInterval(DateTime startDate, DateTime endDate)
        {
            return _LogRepository.GetList(d => d.LogDate >= startDate.Date && d.LogDate <= endDate);
        }

        public void AddLog(params Log[] Logs)
        {
            /* Validation and error handling omitted */
            _LogRepository.Add(Logs);
        }

        public void UpdateLog(params Log[] Logs)
        {
            /* Validation and error handling omitted */
            _LogRepository.Update(Logs);
        }

        public void RemoveLog(params Log[] Logs)
        {
            /* Validation and error handling omitted */
            _LogRepository.Remove(Logs);
        }
    }
}
