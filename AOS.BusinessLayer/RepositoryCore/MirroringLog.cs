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
        IList<MirroringLog> GetAll();
        IList<MirroringLog> GetMirroringLogByApp(int appId);
        IList<MirroringLog> GetMirroringLogByLogType(int appId);
        void AddMirroringLog(params MirroringLog[] MirroringLogs);
        void RemoveMirroringLog(params MirroringLog[] MirroringLogs);
    }

    public partial class BusinessLayer : IBusinessLayer
    {
        public IList<MirroringLog> GetAll()
        {
            return _MirroringLogRepository.GetAll();
        }

        public IList<MirroringLog> GetMirroringLogByApp(int appId)
        {
            return _MirroringLogRepository.GetList(d => d.AppID.Equals(appId));
        }

        public IList<MirroringLog> GetMirroringLogByLogType(int mirroringLogTypeId)
        {
            return _MirroringLogRepository.GetList(d => d.MirroringLogType.Equals(mirroringLogTypeId));
        }

        public void AddMirroringLog(params MirroringLog[] MirroringLogs)
        {
            _MirroringLogRepository.Add(MirroringLogs);
        }

        public void RemoveMirroringLog(params MirroringLog[] MirroringLogs)
        {
            _MirroringLogRepository.Remove(MirroringLogs);
        }
    }
}
