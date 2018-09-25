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
        IList<BisnodeManager> GetAllBisnodeManagers();
        BisnodeManager GetBisnodeManagerById(int bisnodeManagerId);
        IList<BisnodeManager> GetBisnodeManagerByAccountId(int contactId);
        void AddBisnodeManager(params BisnodeManager[] bisnodeManagers);
        void UpdateBisnodeManager(params BisnodeManager[] bisnodeManagers);
        void RemoveBisnodeManager(params BisnodeManager[] bisnodeManagers);
    }

    public partial class BusinessLayer : IBusinessLayer
    {
        public IList<BisnodeManager> GetAllBisnodeManagers()
        {
            return _BisNodeManagerRepository.GetAll();
        }

        public BisnodeManager GetBisnodeManagerById(int bisnodeManagerId)
        {
            return _BisNodeManagerRepository.GetSingle(d => d.BisnodeID.Equals(bisnodeManagerId));
        }

        public IList<BisnodeManager> GetBisnodeManagerByAccountId(int accountId)
        {
            return _BisNodeManagerRepository.GetList(d => d.AccountID.Equals(accountId));
        }

        public void AddBisnodeManager(params BisnodeManager[] bisnodeManagers)
        {
            /* Validation and error handling omitted */

            // Fill createdate and createuser fields
            foreach (var item in bisnodeManagers)
            {
                item.CreateDate = DateTime.Now;
                item.CreateUser = _CurrentUser;
            }

            _BisNodeManagerRepository.Add(bisnodeManagers);
        }

        public void UpdateBisnodeManager(params BisnodeManager[] bisnodeManagers)
        {
            /* Validation and error handling omitted */

            // Fill createdate and createuser fields
            foreach (var item in bisnodeManagers)
            {
                item.UpdateDate = DateTime.Now;
                item.UpdateUser = _CurrentUser;
            }

            _BisNodeManagerRepository.Update(bisnodeManagers);
        }

        public void RemoveBisnodeManager(params BisnodeManager[] bisnodeManagers)
        {
            /* Validation and error handling omitted */
            _BisNodeManagerRepository.Remove(bisnodeManagers);
        }
    }
}
