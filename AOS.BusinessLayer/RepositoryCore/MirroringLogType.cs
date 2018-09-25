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
        MirroringLogType GetMirroringLogTypeById(int mirroringLogTypeId);
        MirroringLogType GetMirroringLogTypeByType(string logType);
        void AddMirroringLogType(params MirroringLogType[] MirroringLogTypes);
        void RemoveMirroringLogType(params MirroringLogType[] MirroringLogTypes);
    }

    public partial class BusinessLayer : IBusinessLayer
    {
        public MirroringLogType GetMirroringLogTypeById(int mirroringLogTypeId)
        {
            return _MirroringLogTypeRepository.GetSingle(d => d.MirroringLogTypeId.Equals(mirroringLogTypeId));
        }

        public MirroringLogType GetMirroringLogTypeByType(string logType)
        {
            return _MirroringLogTypeRepository.GetSingle(d => d.Type.ToLower().Equals(logType.ToLower()));
        }

        public void AddMirroringLogType(params MirroringLogType[] MirroringLogTypes)
        {
            _MirroringLogTypeRepository.Add(MirroringLogTypes);
        }

        public void RemoveMirroringLogType(params MirroringLogType[] MirroringLogTypes)
        {
            _MirroringLogTypeRepository.Remove(MirroringLogTypes);
        }
    }
}
