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
        IList<AspNetUserRoles> GetAllUserRoles();
        AspNetUserRoles GetUserRoleByUserId(string userId);
        void AddUserRole(params AspNetUserRoles[] userRoles);
        void UpdateUserRole(params AspNetUserRoles[] userRoles);
        void RemoveUserRole(params AspNetUserRoles[] userRoles);
    }

    public partial class BusinessLayer : IBusinessLayer
    {
        public IList<AspNetUserRoles> GetAllUserRoles()
        {
            return _UserRoleRepository.GetAll();
        }

        public AspNetUserRoles GetUserRoleByUserId(string userId)
        {
            return _UserRoleRepository.GetSingle(
                d => d.UserId.Equals(userId));
        }
        public void AddUserRole(params AspNetUserRoles[] UserRoles)
        {
            _UserRoleRepository.Add(UserRoles);
        }

        public void UpdateUserRole(params AspNetUserRoles[] UserRoles)
        {

            _UserRoleRepository.Update(UserRoles);
        }

        public void RemoveUserRole(params AspNetUserRoles[] UserRoles)
        {
            _UserRoleRepository.Remove(UserRoles);
        }
    }
}
