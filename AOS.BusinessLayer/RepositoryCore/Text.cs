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
        IList<Text> GetAllTexts();
        Text GetTextById(int Id);
        IList<Text> GetTextByTypeId(int Type);
        void AddText(params Text[] Text);
        void UpdateText(params Text[] Text);
        void RemoveText(params Text[] Text);
    }

    public partial class BusinessLayer : IBusinessLayer
    {
        public IList<Text> GetAllTexts()
        {
            return _TextRepository.GetAll();
        }

        public Text GetTextById(int Id)
        {
            return _TextRepository.GetSingle(d => d.TextID.Equals(Id));
        }

        public IList<Text> GetTextByTypeId(int TextTypeId)
        {
            return _TextRepository.GetList(d => d.TextTypeID.Equals(TextTypeId));
        }

        public void AddText(params Text[] Text)
        {
            /* Validation and error handling omitted */
            _TextRepository.Add(Text);
        }

        public void UpdateText(params Text[] Text)
        {
            /* Validation and error handling omitted */
            _TextRepository.Update(Text);
        }

        public void RemoveText(params Text[] Text)
        {
            /* Validation and error handling omitted */
            _TextRepository.Remove(Text);
        }
    }
}
