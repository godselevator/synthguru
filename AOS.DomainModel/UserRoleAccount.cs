//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AOS.DomainModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class UserRoleAccount
    {
        public string UserID { get; set; }
        public string RoleID { get; set; }
        public int AccountID { get; set; }
        public bool ActiveAccount { get; set; }
        public bool UserEnabled { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public string CreateUser { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public string UpdateUser { get; set; }
    
        public virtual Account Account { get; set; }
        public virtual AspNetRoles AspNetRoles { get; set; }
        public virtual AspNetUsers AspNetUsers { get; set; }
    }
}