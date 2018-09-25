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
        IList<Converter> GetAllConverters();
        Converter GetConverterById(int ConverterId);
        IList<Converter> GetConverterByAccountId(int accountId);
        void AddConverter(params Converter[] converters);
        void UpdateConverter(params Converter[] converters);
        void RemoveConverter(params Converter[] converters);
    }

    public partial class BusinessLayer : IBusinessLayer
    {
        public IList<Converter> GetAllConverters()
        {
            return _ConverterRepository.GetAll();
        }

        public Converter GetConverterById(int converterId)
        {
            return _ConverterRepository.GetSingle(d => d.ConverterID.Equals(converterId));
        }

        public IList<Converter> GetConverterByAccountId(int accountId)
        {
            return _ConverterRepository.GetList(d => d.AccountID.Equals(accountId));
        }

        public void AddConverter(params Converter[] converters)
        {
            /* Validation and error handling omitted */

            // Fill createdate and createuser fields
            foreach (var item in converters)
            {
                item.CreateDate = DateTime.Now;
                item.CreateUser = _CurrentUser;
            }

            _ConverterRepository.Add(converters);
        }

        public void UpdateConverter(params Converter[] converters)
        {
            /* Validation and error handling omitted */

            // Fill updatedate and updateuser fields
            foreach (var item in converters)
            {
                item.UpdateDate = DateTime.Now;
                item.UpdateUser = _CurrentUser;
            }

            _ConverterRepository.Update(converters);
        }

        public void RemoveConverter(params Converter[] converters)
        {
            /* Validation and error handling omitted */
            _ConverterRepository.Remove(converters);
        }
    }
}
