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
        IList<MatchPerson> GetAllMatchPersons();
        IList<MatchPerson> GetMatchPersonListByAccountId(int accountId);
        MatchPerson GetMatchPersonById(int matchPersonId);
        void AddMatchPerson(params MatchPerson[] matchPersons);
        void UpdateMatchPerson(params MatchPerson[] matchPersons);
        void RemoveMatchPerson(params MatchPerson[] matchPersons);
    }

    public partial class BusinessLayer : IBusinessLayer
    {
        public IList<MatchPerson> GetAllMatchPersons()
        {
            return _MatchPersonRepository.GetAll();
        }

        public IList<MatchPerson> GetMatchPersonListByAccountId(int accountId)
        {
            return _MatchPersonRepository.GetList(d => d.AccountID.Equals(accountId));
        }

        public MatchPerson GetMatchPersonById(int matchPersonId)
        {
            return _MatchPersonRepository.GetSingle(d => d.MatchPersonId.Equals(matchPersonId));
        }

        public void AddMatchPerson(params MatchPerson[] matchPersons)
        {
            /* Validation and error handling omitted */

            // Fill createdate and createuser fields
            foreach (var item in matchPersons)
            {
                item.CreateDate = DateTime.Now;
                item.CreateUser = _CurrentUser;
            }

            _MatchPersonRepository.Add(matchPersons);
        }

        public void UpdateMatchPerson(params MatchPerson[] matchPersons)
        {
            /* Validation and error handling omitted */

            // Fill updatedate and updateuser fields
            foreach (var item in matchPersons)
            {
                item.UpdateDate = DateTime.Now;
                item.UpdateUser = _CurrentUser;
            }

            _MatchPersonRepository.Update(matchPersons);
        }

        public void RemoveMatchPerson(params MatchPerson[] matchPersons)
        {
            /* Validation and error handling omitted */
            _MatchPersonRepository.Remove(matchPersons);
        }
    }
}
