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
    
    public partial class SignicatLanguage
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SignicatLanguage()
        {
            this.SignicatAccountSignLanguage = new HashSet<SignicatAccountSignLanguage>();
        }
    
        public int SignicatLanguageID { get; set; }
        public string LanguageText { get; set; }
        public string LanguageValue { get; set; }
        public bool Enabled { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public string CreateUser { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public string UpdateUser { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SignicatAccountSignLanguage> SignicatAccountSignLanguage { get; set; }
    }
}