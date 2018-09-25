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
        IList<MachineName> GetAllMachineNames();
        IList<MachineName> GetMachineNamesByAPIKeyId(int APIKeyId);
        MachineName GetMachineNameByApiKeyIdAndSID(int apiKeyId, string SID);
        MachineName GetMachineNameByID(int ID);
        void AddMachineName(params MachineName[] MachineNames);
        void UpdateMachineName(params MachineName[] MachineNames);
        void RemoveMachineName(params MachineName[] MachineNames);
    }

    public partial class BusinessLayer : IBusinessLayer
    {
        public IList<MachineName> GetAllMachineNames()
        {
            return _MachineNameRepository.GetAll();
        }

        public IList<MachineName> GetMachineNamesByAPIKeyId(int APIKeyId)
        {
            return _MachineNameRepository.GetList(d => d.APIKeyId.Equals(APIKeyId));
        }

        public MachineName GetMachineNameByApiKeyIdAndSID(int apiKeyId, string SID)
        {
            return _MachineNameRepository.GetSingle(d => d.SID.ToUpper().Equals(SID.ToUpper()) && d.APIKeyId.Equals(apiKeyId));
        }

        public MachineName GetMachineNameByID(int ID)
        {
            return _MachineNameRepository.GetSingle(d => d.MachineNameId.Equals(ID));
        }

        public void AddMachineName(params MachineName[] MachineNames)
        {
            /* Validation and error handling omitted */
            _MachineNameRepository.Add(MachineNames);
        }

        public void UpdateMachineName(params MachineName[] MachineNames)
        {
            /* Validation and error handling omitted */
            _MachineNameRepository.Update(MachineNames);
        }

        public void RemoveMachineName(params MachineName[] MachineNames)
        {
            /* Validation and error handling omitted */
            _MachineNameRepository.Remove(MachineNames);
        }
    }
}
