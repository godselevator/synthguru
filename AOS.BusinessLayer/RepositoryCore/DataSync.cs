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
        IList<DataSync> GetAllDataSyncs();
        DataSync GetDataSyncByAppId(int AppId);
        void AddDataSync(params DataSync[] DataSyncs);
        void UpdateDataSync(params DataSync[] DataSyncs);
        void RemoveDataSync(params DataSync[] DataSyncs);
    }

    public partial class BusinessLayer : IBusinessLayer
    {
        public IList<DataSync> GetAllDataSyncs()
        {
            return _DataSyncRepository.GetAll();
        }

        public DataSync GetDataSyncByAppId(int AppId)
        {
            return _DataSyncRepository.GetSingle(d => d.AppId.Equals(AppId));
        }

        public void AddDataSync(params DataSync[] DataSyncs)
        {
            /* Validation and error handling omitted */
            _DataSyncRepository.Add(DataSyncs);
        }

        public void UpdateDataSync(params DataSync[] DataSyncs)
        {
            /* Validation and error handling omitted */
            _DataSyncRepository.Update(DataSyncs);
        }

        public void RemoveDataSync(params DataSync[] DataSyncs)
        {
            /* Validation and error handling omitted */
            _DataSyncRepository.Remove(DataSyncs);
        }
    }
}
