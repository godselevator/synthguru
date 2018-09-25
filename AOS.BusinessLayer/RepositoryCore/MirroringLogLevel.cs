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
        MirroringLogLevel GetMirroringLogLevelById(int mirroringLogLevelId);
        MirroringLogLevel GetMirroringLogLevelByType(string LogLevel);
        void AddMirroringLogLevel(params MirroringLogLevel[] MirroringLogLevels);
        void RemoveMirroringLogLevel(params MirroringLogLevel[] MirroringLogLevels);
    }

    public partial class BusinessLayer : IBusinessLayer
    {
        public MirroringLogLevel GetMirroringLogLevelById(int mirroringLogLevelId)
        {
            return _MirroringLogLevelRepository.GetSingle(d => d.MirroringLogLevelId.Equals(mirroringLogLevelId));
        }

        public MirroringLogLevel GetMirroringLogLevelByType(string LogLevel)
        {
            return _MirroringLogLevelRepository.GetSingle(d => d.LogLevel.ToLower().Equals(LogLevel.ToLower()));
        }

        public void AddMirroringLogLevel(params MirroringLogLevel[] MirroringLogLevels)
        {
            _MirroringLogLevelRepository.Add(MirroringLogLevels);
        }

        public void RemoveMirroringLogLevel(params MirroringLogLevel[] MirroringLogLevels)
        {
            _MirroringLogLevelRepository.Remove(MirroringLogLevels);
        }
    }
}
