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
        IList<LogGroup> GetAllLogGroups();
        LogGroup GetLogGroupById(int LogGroupId);
        void AddLogGroup(params LogGroup[] LogGroups);
        void UpdateLogGroup(params LogGroup[] LogGroups);
        void RemoveLogGroup(params LogGroup[] LogGroups);
    }

    public partial class BusinessLayer : IBusinessLayer
    {
        public IList<LogGroup> GetAllLogGroups()
        {
            return _LogGroupRepository.GetAll();
        }

        public LogGroup GetLogGroupById(int LogGroupId)
        {
            return _LogGroupRepository.GetSingle(d => d.LogGroupID.Equals(LogGroupId));
        }

        public void AddLogGroup(params LogGroup[] LogGroups)
        {
            /* Validation and error handling omitted */
            _LogGroupRepository.Add(LogGroups);
        }

        public void UpdateLogGroup(params LogGroup[] LogGroups)
        {
            /* Validation and error handling omitted */
            _LogGroupRepository.Update(LogGroups);
        }

        public void RemoveLogGroup(params LogGroup[] LogGroups)
        {
            /* Validation and error handling omitted */
            _LogGroupRepository.Remove(LogGroups);
        }
    }
}
