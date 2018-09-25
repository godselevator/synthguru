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
        IList<BisNodeInfo> GetAllBisNodeInfos();
        BisNodeInfo GetBisNodeInfoById(int BisNodeInfoId);
        IList<BisNodeInfo> GetBisNodesInfoByContactId(int contactId);
        IList<BisNodeInfo> GetBisNodesInfoByAccountId(int accountId);
        BisNodeInfo GetBisNodeByAccountIdAndContactId(int accountId, int contactId);
        void AddBisNodeInfo(params BisNodeInfo[] bisNodeInfos);
        void UpdateBisNodeInfo(params BisNodeInfo[] bisNodeInfos);
        void RemoveBisNodeInfo(params BisNodeInfo[] bisNodeInfos);
    }

    public partial class BusinessLayer : IBusinessLayer
    {
        public IList<BisNodeInfo> GetAllBisNodeInfos()
        {
            return _BisNodeInfoRepository.GetAll();
        }

        public BisNodeInfo GetBisNodeInfoById(int BisNodeInfoId)
        {
            return _BisNodeInfoRepository.GetSingle(d => d.BisNodeInfoID.Equals(BisNodeInfoId));
        }

        public IList<BisNodeInfo> GetBisNodesInfoByContactId(int contactId)
        {
            return _BisNodeInfoRepository.GetList(d => d.ContactID.Equals(contactId));
        }

        public IList<BisNodeInfo> GetBisNodesInfoByAccountId(int accountId)
        {
            return _BisNodeInfoRepository.GetList(d => d.AccountID.Equals(accountId));
        }

        public BisNodeInfo GetBisNodeByAccountIdAndContactId(int accountId, int contactId)
        {
            return _BisNodeInfoRepository.GetSingle(d => d.AccountID.Equals(accountId) && d.ContactID.Equals(contactId));
        }

        public void AddBisNodeInfo(params BisNodeInfo[] bisNodeInfos)
        {
            /* Validation and error handling omitted */

            _BisNodeInfoRepository.Add(bisNodeInfos);
        }

        public void UpdateBisNodeInfo(params BisNodeInfo[] bisNodeInfos)
        {
            /* Validation and error handling omitted */

            _BisNodeInfoRepository.Update(bisNodeInfos);
        }

        public void RemoveBisNodeInfo(params BisNodeInfo[] bisNodeInfos)
        {
            /* Validation and error handling omitted */
            _BisNodeInfoRepository.Remove(bisNodeInfos);
        }
    }
}
