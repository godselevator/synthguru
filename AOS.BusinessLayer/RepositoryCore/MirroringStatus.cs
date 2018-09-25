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
        IList<MirroringStatus> GetMirroringStatusByAppId(int appId);
        MirroringStatus GetMirroringStatus();
        void AddMirroringStatus(params MirroringStatus[] MirroringStatuss);
        void RemoveMirroringStatus(params MirroringStatus[] MirroringStatuss);
    }

    public partial class BusinessLayer : IBusinessLayer
    {
        public IList<MirroringStatus> GetMirroringStatusByAppId(int appId)
        {
            return _MirroringStatusRepository.GetList(d => d.LockedByAppID.Equals(appId));
        }

        public MirroringStatus GetMirroringStatus()
        {
            var maxTimeStamp = _MirroringStatusRepository.GetAll().Max(obj => obj.LockedDateTime);
            
            return _MirroringStatusRepository.GetSingle(d => d.LockedDateTime.Equals(maxTimeStamp));
        }

        public void AddMirroringStatus(params MirroringStatus[] MirroringStatuss)
        {
            _MirroringStatusRepository.Add(MirroringStatuss);
        }

        public void RemoveMirroringStatus(params MirroringStatus[] MirroringStatuss)
        {
            _MirroringStatusRepository.Remove(MirroringStatuss);
        }
    }
}
