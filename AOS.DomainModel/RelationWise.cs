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
    
    public partial class RelationWise
    {
        public int RelationWiseID { get; set; }
        public int AccountID { get; set; }
        public int AppID { get; set; }
        public int SOCompanyID { get; set; }
        public int SOAssociateID { get; set; }
        public decimal SOPersonID { get; set; }
        public string RelationWiseKey { get; set; }
        public string Email { get; set; }
        public bool EnabledSale { get; set; }
        public bool EnabledAppointment { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public string CreateUser { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public string UpdateUser { get; set; }
    
        public virtual Account Account { get; set; }
        public virtual App App { get; set; }
    }
}
