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
    
    public partial class MachineName
    {
        public int MachineNameId { get; set; }
        public int APIKeyId { get; set; }
        public string MachineName1 { get; set; }
        public string Comments { get; set; }
        public bool Enabled { get; set; }
        public bool Approved { get; set; }
        public string SID { get; set; }
    
        public virtual APIKey APIKey { get; set; }
    }
}
