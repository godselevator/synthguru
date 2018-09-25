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
        IList<Referral> GetAllReferrals();
        Referral GetReferralById(int referralId);
        IList<Referral> GetreferralsByAccount(int accountId);
        void AddReferral(params Referral[] referrals);
        void UpdateReferral(params Referral[] referrals);
        void RemoveReferral(params Referral[] referrals);
    }

    public partial class BusinessLayer : IBusinessLayer
    {
        public IList<Referral> GetAllReferrals()
        {
            return _ReferralRepository.GetAll();
        }

        public Referral GetReferralById(int referralId)
        {
            return _ReferralRepository.GetSingle(d => d.ReferralID.Equals(referralId));
        }

        public IList<Referral> GetreferralsByAccount(int accountId)
        {
            return _ReferralRepository.GetList(d => d.ReferralID.Equals(accountId));
        }

        public void AddReferral(params Referral[] referrals)
        {
            /* Validation and error handling omitted */

            // Fill createdate and createuser fields
            foreach (var item in referrals)
            {
                item.CreateDate = DateTime.Now;
                item.CreateUser = _CurrentUser;
            }

            _ReferralRepository.Add(referrals);
        }

        public void UpdateReferral(params Referral[] referrals)
        {
            /* Validation and error handling omitted */

            // Fill updatedate and updateuser fields
            foreach (var item in referrals)
            {
                item.UpdateDate = DateTime.Now;
                item.UpdateUser = _CurrentUser;
            }

            _ReferralRepository.Update(referrals);
        }

        public void RemoveReferral(params Referral[] referrals)
        {
            /* Validation and error handling omitted */
            _ReferralRepository.Remove(referrals);
        }
    }
}
