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
        IList<SynthModel> GetAllSynthModels();
        SynthModel GetSynthModelById(int id);
        SynthModel GetSynthModelByName(string name);
        IList<SynthModel> GetSynthModelsByManufacturerId(int id);
        void AddSynthModel(params SynthModel[] SynthModel);
        void UpdateSynthModel(params SynthModel[] SynthModel);
        void RemoveSynthModel(params SynthModel[] SynthModel);
    }
    
    public partial class BusinessLayer : IBusinessLayer
    {
        public IList<SynthModel> GetAllSynthModels()
        {
            return _SynthModelRepository.GetAll();
        }

        public SynthModel GetSynthModelById(int SynthModelId)
        {
            return _SynthModelRepository.GetSingle(d => d.Id.Equals(SynthModelId));
        }

        public SynthModel GetSynthModelByName(string name)
        {
            return _SynthModelRepository.GetSingle(d => d.Name.ToLower().Equals(name.ToLower()));
        }

        public IList<SynthModel> GetSynthModelsByManufacturerId(int id)
        {
            return _SynthModelRepository.GetList(d => d.ManufacturerId.Equals(id));
        }

        public void AddSynthModel(params SynthModel[] SynthModel)
        {
            _SynthModelRepository.Add(SynthModel);
        }

        public void UpdateSynthModel(params SynthModel[] SynthModel)
        {
            _SynthModelRepository.Update(SynthModel);
        }

        public void RemoveSynthModel(params SynthModel[] SynthModel)
        {
            _SynthModelRepository.Remove(SynthModel);
        }
    }
}
