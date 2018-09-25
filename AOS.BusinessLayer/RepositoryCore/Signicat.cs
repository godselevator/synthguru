using AOS.DomainModel;
using System;
using System.Collections.Generic;

namespace AOS.BusinessLayer
{
    public partial interface IBusinessLayer
    {
        IList<Signicat> GetAllSignicats();
        Signicat GetSignicatById(int id);
        Signicat GetSignicatByAccountId(int? accountId);
        void AddSignicat(params Signicat[] Signicats);
        void UpdateSignicat(params Signicat[] Signicats);
        void RemoveSignicat(params Signicat[] Signicats);
    }

    public partial class BusinessLayer : IBusinessLayer
    {
        public IList<Signicat> GetAllSignicats()
        {
            return _SignicatRepository.GetAll();
        }

        public Signicat GetSignicatById(int id)
        {
            return _SignicatRepository.GetSingle(d => d.SignicatID.Equals(id));
        }

        public Signicat GetSignicatByAccountId(int? accountId)
        {
            return _SignicatRepository.GetSingle(d => d.AccountID.Equals(accountId));
        }

        public void AddSignicat(params Signicat[] Signicats)
        {
            /* Validation and error handling omitted */

            // Set createdate and createuser
            foreach (var item in Signicats)
            {
                item.CreateDate = DateTime.Now;
                item.CreateUser = _CurrentUser;
            }

            _SignicatRepository.Add(Signicats);
        }

        public void UpdateSignicat(params Signicat[] Signicats)
        {
            /* Validation and error handling omitted */
            foreach (var item in Signicats)
            {
                item.UpdateDate = DateTime.Now;
                item.UpdateUser = _CurrentUser;
            }

            _SignicatRepository.Update(Signicats);
        }

        public void RemoveSignicat(params Signicat[] Signicats)
        {
            /* Validation and error handling omitted */
            _SignicatRepository.Remove(Signicats);
        }
    }
}
