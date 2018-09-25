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
        IList<Zapier> GetAllZapiers();
        Zapier GetZapierById(int ZapierId);
        Zapier GetZapierByAccountId(int accountId);
        Zapier GetZapierByAPIKey(string APIKey);
        void AddZapier(params Zapier[] Zapiers);
        void UpdateZapier(params Zapier[] Zapiers);
        void RemoveZapier(params Zapier[] Zapiers);
    }

    public partial class BusinessLayer : IBusinessLayer
    {
        public IList<Zapier> GetAllZapiers()
        {
            return _ZapierRepository.GetAll();
        }

        public Zapier GetZapierById(int ZapierId)
        {
            return _ZapierRepository.GetSingle(d => d.ZapierID.Equals(ZapierId));
        }

        public Zapier GetZapierByAccountId(int accountId)
        {
            return _ZapierRepository.GetSingle(d => d.AccountID.Equals(accountId));
        }

        public Zapier GetZapierByAPIKey(string APIKey)
        {
            return _ZapierRepository.GetSingle(d => d.APIKey.Equals(APIKey));
        }

        public void AddZapier(params Zapier[] Zapiers)
        {
            /* Validation and error handling omitted */
            _ZapierRepository.Add(Zapiers);
        }

        public void UpdateZapier(params Zapier[] Zapiers)
        {
            /* Validation and error handling omitted */
            _ZapierRepository.Update(Zapiers);
        }

        public void RemoveZapier(params Zapier[] Zapiers)
        {
            /* Validation and error handling omitted */
            _ZapierRepository.Remove(Zapiers);
        }
    }
}
