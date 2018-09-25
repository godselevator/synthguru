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
        IList<SyncSale> GetAllSyncSales();
        SyncSale GetSyncSaleById(int Id);
        SyncSale GetSyncSaleBySaleId(int SaleId);
        void AddSyncSale(params SyncSale[] SyncSale);
        void UpdateSyncSale(params SyncSale[] SyncSale);
        void RemoveSyncSale(params SyncSale[] SyncSale);
    }

    public partial class BusinessLayer : IBusinessLayer
    {
        public IList<SyncSale> GetAllSyncSales()
        {
            return _SyncSaleRepository.GetAll();
        }

        public SyncSale GetSyncSaleById(int Id)
        {
            return _SyncSaleRepository.GetSingle(d => d.SyncSaleID.Equals(Id));
        }

        public SyncSale GetSyncSaleBySaleId(int SaleId)
        {
            return _SyncSaleRepository.GetSingle(d => d.SaleId.Equals(SaleId));
        }

        public void AddSyncSale(params SyncSale[] SyncSale)
        {
            /* Validation and error handling omitted */
            _SyncSaleRepository.Add(SyncSale);
        }

        public void UpdateSyncSale(params SyncSale[] SyncSale)
        {
            /* Validation and error handling omitted */
            _SyncSaleRepository.Update(SyncSale);
        }

        public void RemoveSyncSale(params SyncSale[] SyncSale)
        {
            /* Validation and error handling omitted */
            _SyncSaleRepository.Remove(SyncSale);
        }
    }
}
