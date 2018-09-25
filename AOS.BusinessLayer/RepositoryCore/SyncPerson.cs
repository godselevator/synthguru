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
        IList<SyncPerson> GetAllSyncPersons();
        SyncPerson GetSyncPersonById(int Id);
        SyncPerson GetSyncPersonByPersonId(int PersonId);
        void AddSyncPerson(params SyncPerson[] SyncPerson);
        void UpdateSyncPerson(params SyncPerson[] SyncPerson);
        void RemoveSyncPerson(params SyncPerson[] SyncPerson);
    }

    public partial class BusinessLayer : IBusinessLayer
    {
        public IList<SyncPerson> GetAllSyncPersons()
        {
            return _SyncPersonRepository.GetAll();
        }

        public SyncPerson GetSyncPersonById(int Id)
        {
            return _SyncPersonRepository.GetSingle(d => d.SyncPersonId.Equals(Id));
        }

        public SyncPerson GetSyncPersonByPersonId(int PersonId)
        {
            return _SyncPersonRepository.GetSingle(d => d.PersonId.Equals(PersonId));
        }

        public void AddSyncPerson(params SyncPerson[] SyncPerson)
        {
            /* Validation and error handling omitted */
            _SyncPersonRepository.Add(SyncPerson);
        }

        public void UpdateSyncPerson(params SyncPerson[] SyncPerson)
        {
            /* Validation and error handling omitted */
            _SyncPersonRepository.Update(SyncPerson);
        }

        public void RemoveSyncPerson(params SyncPerson[] SyncPerson)
        {
            /* Validation and error handling omitted */
            _SyncPersonRepository.Remove(SyncPerson);
        }
    }
}
