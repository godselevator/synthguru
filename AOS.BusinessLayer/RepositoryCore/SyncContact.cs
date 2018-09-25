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
        IList<SyncContact> GetAllSyncContacts();
        SyncContact GetSyncContactById(int Id);
        SyncContact GetSyncContactByContactId(int ContactId);
        SyncContact GetSyncContactByName(string Name);
        void AddSyncContact(params SyncContact[] SyncContact);
        void UpdateSyncContact(params SyncContact[] SyncContact);
        void RemoveSyncContact(params SyncContact[] SyncContact);
    }

    public partial class BusinessLayer : IBusinessLayer
    {
        public IList<SyncContact> GetAllSyncContacts()
        {
            return _SyncContactRepository.GetAll();
        }

        public SyncContact GetSyncContactById(int Id)
        {
            return _SyncContactRepository.GetSingle(d => d.SyncContactID.Equals(Id));
        }

        public SyncContact GetSyncContactByContactId(int ContactId)
        {
            return _SyncContactRepository.GetSingle(d => d.ContactId.Equals(ContactId));
        }

        public SyncContact GetSyncContactByName(string Name)
        {
            return _SyncContactRepository.GetSingle(d => d.Name.ToLower().Equals(Name.ToLower()));
        }

        public void AddSyncContact(params SyncContact[] SyncContact)
        {
            /* Validation and error handling omitted */
            _SyncContactRepository.Add(SyncContact);
        }

        public void UpdateSyncContact(params SyncContact[] SyncContact)
        {
            /* Validation and error handling omitted */
            _SyncContactRepository.Update(SyncContact);
        }

        public void RemoveSyncContact(params SyncContact[] SyncContact)
        {
            /* Validation and error handling omitted */
            _SyncContactRepository.Remove(SyncContact);
        }
    }
}
