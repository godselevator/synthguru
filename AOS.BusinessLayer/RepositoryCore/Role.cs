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
        IList<AspNetRoles> GetAllRoles();
        AspNetRoles GetRoleById(string id);
        AspNetRoles GetRoleByName(string name);
    }

    public partial class BusinessLayer : IBusinessLayer
    {
        public IList<AspNetRoles> GetAllRoles()
        {
            return _RoleRepository.GetAll();
        }

        public AspNetRoles GetRoleById(string id)
        {
            return _RoleRepository.GetSingle(
                d => d.Id.Equals(id));
        }

        public AspNetRoles GetRoleByName(string name)
        {
            return _RoleRepository.GetSingle(
                d => d.Name.ToLower().Equals(name.ToLower()));
        }
    }
}
