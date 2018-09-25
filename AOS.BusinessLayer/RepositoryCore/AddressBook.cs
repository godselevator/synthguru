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
        IList<Addressbook> GetAllAddressbooks();
        Addressbook GetAddressbookById(int addressBookId);
        IList<Addressbook> GetAddressbooksByAccount(int accountId);
        void AddAddressbook(params Addressbook[] addressBooks);
        void UpdateAddressbook(params Addressbook[] addressBooks);
        void RemoveAddressbook(params Addressbook[] addressBooks);
    }

    public partial class BusinessLayer : IBusinessLayer
    {
        public IList<Addressbook> GetAllAddressbooks()
        {
            return _AddressBookRepository.GetAll();
        }

        public Addressbook GetAddressbookById(int addressBookId)
        {
            return _AddressBookRepository.GetSingle(d => d.AddressbookID.Equals(addressBookId));
        }

        public IList<Addressbook> GetAddressbooksByAccount(int accountId)
        {
            return _AddressBookRepository.GetList(d => d.AccountID.Equals(accountId));
        }

        public void AddAddressbook(params Addressbook[] addressBooks)
        {
            /* Validation and error handling omitted */

            // Set createdate and createuser
            foreach (var item in addressBooks)
            {
                item.CreateDate = DateTime.Now;
                item.CreateUser = _CurrentUser;
            }

            _AddressBookRepository.Add(addressBooks);
        }

        public void UpdateAddressbook(params Addressbook[] addressBooks)
        {
            /* Validation and error handling omitted */

            // Set updatedate and updateuser
            foreach (var item in addressBooks)
            {
                item.UpdateDate = DateTime.Now;
                item.UpdateUser = _CurrentUser;
            }

            _AddressBookRepository.Update(addressBooks);
        }

        public void RemoveAddressbook(params Addressbook[] addressBooks)
        {
            /* Validation and error handling omitted */
            _AddressBookRepository.Remove(addressBooks);
        }
    }
}
