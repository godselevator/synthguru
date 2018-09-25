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
        IList<ElasticIO> GetAllElasticIOs();
        ElasticIO GetElasticIOById(int ElasticIOId);
        ElasticIO GetElasticIOByAccountId(int accountId);
        ElasticIO GetElasticIOByElasticGUID(string elasticGUID);
        void AddElasticIO(params ElasticIO[] elasticIOs);
        void UpdateElasticIO(params ElasticIO[] elasticIOs);
        void RemoveElasticIO(params ElasticIO[] elasticIOs);
    }

    public partial class BusinessLayer : IBusinessLayer
    {
        public IList<ElasticIO> GetAllElasticIOs()
        {
            return _ElasticIORepository.GetAll();
        }

        public ElasticIO GetElasticIOById(int ElasticIOId)
        {
            return _ElasticIORepository.GetSingle(d => d.ElasticIOID.Equals(ElasticIOId));
        }

        public ElasticIO GetElasticIOByAccountId(int accountId)
        {
            return _ElasticIORepository.GetSingle(d => d.AccountID.Equals(accountId));
        }

        public ElasticIO GetElasticIOByElasticGUID(string elasticGUID)
        {
            return _ElasticIORepository.GetSingle(d => d.ElasticGUID.Equals(elasticGUID));
        }

        public void AddElasticIO(params ElasticIO[] elasticIOs)
        {
            /* Validation and error handling omitted */

            // Fill createdate and createuser fields
            foreach (var item in elasticIOs)
            {
                item.CreateDate = DateTime.Now;
                item.CreateUser = _CurrentUser;
            }

            _ElasticIORepository.Add(elasticIOs);
        }

        public void UpdateElasticIO(params ElasticIO[] elasticIOs)
        {
            /* Validation and error handling omitted */

            // Fill updatedate and updateuser fields
            foreach (var item in elasticIOs)
            {
                item.UpdateDate = DateTime.Now;
                item.UpdateUser = _CurrentUser;
            }

            _ElasticIORepository.Update(elasticIOs);
        }

        public void RemoveElasticIO(params ElasticIO[] elasticIOs)
        {
            /* Validation and error handling omitted */
            _ElasticIORepository.Remove(elasticIOs);
        }
    }
}
