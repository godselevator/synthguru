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
        IList<AspNetUsers> GetAllUsers();
        AspNetUsers GetUserById(string id);
        AspNetUsers GetUserByName(string name);
        AspNetUsers GetUserByEmail(string email);
    }

    public partial class BusinessLayer : IBusinessLayer
    {
        public IList<AspNetUsers> GetAllUsers()
        {
            return _UserRepository.GetAll();
        }

        public AspNetUsers GetUserById(string id)
        {
            return _UserRepository.GetSingle(
                d => d.Id.Equals(id));
        }

        public AspNetUsers GetUserByName(string name)
        {
            return _UserRepository.GetSingle(
                d => d.FirstNameLastName.ToLower().Equals(name.ToLower()));
        }

        public AspNetUsers GetUserByEmail(string email)
        {
            return _UserRepository.GetSingle(
                d => d.Email.ToLower().Equals(email.ToLower()));
        }

        public void RemoveUser(params AspNetUsers[] users)
        {
            /* Validation and error handling omitted */
            _UserRepository.Remove(users);
        }

    }
}
