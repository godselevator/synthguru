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
        IList<TextType> GetAllTextTypes();
        TextType GetTextTypeById(int TextTypeId);
        TextType GetTextTypeByName(string Name);
        void AddTextType(params TextType[] TextTypes);
        void UpdateTextType(params TextType[] TextTypes);
        void RemoveTextType(params TextType[] TextTypes);
    }

    public partial class BusinessLayer : IBusinessLayer
    {
        public IList<TextType> GetAllTextTypes()
        {
            return _TextTypeRepository.GetAll();
        }

        public TextType GetTextTypeById(int TextTypeId)
        {
            return _TextTypeRepository.GetSingle(d => d.TextTypeID.Equals(TextTypeId));
        }

        public TextType GetTextTypeByName(string Name)
        {
            return _TextTypeRepository.GetSingle(d => d.Name.ToLower().Equals(Name.ToLower()));
        }

        public void AddTextType(params TextType[] TextTypes)
        {
            /* Validation and error handling omitted */
            _TextTypeRepository.Add(TextTypes);
        }

        public void UpdateTextType(params TextType[] TextTypes)
        {
            /* Validation and error handling omitted */
            _TextTypeRepository.Update(TextTypes);
        }

        public void RemoveTextType(params TextType[] TextTypes)
        {
            /* Validation and error handling omitted */
            _TextTypeRepository.Remove(TextTypes);
        }
    }
}
