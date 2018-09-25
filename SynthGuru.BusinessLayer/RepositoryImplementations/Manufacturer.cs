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
        IList<Manufacturer> GetAllManufacturers();
        Manufacturer GetManufacturerById(int id);
        Manufacturer GetManufacturerByName(string name);
        void AddManufacturer(params Manufacturer[] manufacturers);
        void UpdateManufacturer(params Manufacturer[] manufacturers);
        void RemoveManufacturer(params Manufacturer[] manufacturers);
    }
    
    public partial class BusinessLayer : IBusinessLayer
    {
        public IList<Manufacturer> GetAllManufacturers()
        {
            return _ManufacturerRepository.GetAll();
        }

        public Manufacturer GetManufacturerById(int ManufacturerId)
        {
            return _ManufacturerRepository.GetSingle(d => d.Id.Equals(ManufacturerId));
        }

        public Manufacturer GetManufacturerByName(string name)
        {
            return _ManufacturerRepository.GetSingle(d => d.Name.ToLower().Equals(name.ToLower()));
        }

        public void AddManufacturer(params Manufacturer[] Manufacturers)
        {
            _ManufacturerRepository.Add(Manufacturers);
        }

        public void UpdateManufacturer(params Manufacturer[] Manufacturers)
        {
            _ManufacturerRepository.Update(Manufacturers);
        }

        public void RemoveManufacturer(params Manufacturer[] Manufacturers)
        {
            _ManufacturerRepository.Remove(Manufacturers);
        }
    }
}
