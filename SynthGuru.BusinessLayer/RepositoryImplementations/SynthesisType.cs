using SynthGuru.DataAccessLayer;
using SynthGuru.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthGuru.BusinessLayer
{
    public partial interface IBusinessLayer
    {
        IList<SynthesisType> GetAllSynthesisTypes();
        SynthesisType GetSynthesisTypeById(int id);
        SynthesisType GetSynthesisTypeByName(string name);
        void AddSynthesisType(params SynthesisType[] synthesisTypes);
        void UpdateSynthesisType(params SynthesisType[] synthesisTypes);
        void RemoveSynthesisType(params SynthesisType[] synthesisTypes);
    }
    
    public partial class BusinessLayer : IBusinessLayer
    {
        public IList<SynthesisType> GetAllSynthesisTypes()
        {
            return _SynthesisTypeRepository.GetAll();
        }

        public SynthesisType GetSynthesisTypeById(int SynthesisTypeId)
        {
            return _SynthesisTypeRepository.GetSingle(d => d.Id.Equals(SynthesisTypeId));
        }

        public SynthesisType GetSynthesisTypeByName(string name)
        {
            return _SynthesisTypeRepository.GetSingle(d => d.Name.ToLower().Equals(name.ToLower()));
        }

        public void AddSynthesisType(params SynthesisType[] SynthesisType)
        {
            _SynthesisTypeRepository.Add(SynthesisType);
        }

        public void UpdateSynthesisType(params SynthesisType[] SynthesisType)
        {
            _SynthesisTypeRepository.Update(SynthesisType);
        }

        public void RemoveSynthesisType(params SynthesisType[] SynthesisType)
        {
            _SynthesisTypeRepository.Remove(SynthesisType);
        }
    }
}
