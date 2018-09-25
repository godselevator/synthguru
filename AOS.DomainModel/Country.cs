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
    
    public partial class Country
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Country()
        {
            this.Account = new HashSet<Account>();
        }
    
        public int CountryID { get; set; }
        public string Name { get; set; }
        public Nullable<short> Rank { get; set; }
        public string Tooltip { get; set; }
        public Nullable<short> Deleted { get; set; }
        public string EnglishName { get; set; }
        public string ZipPrefix { get; set; }
        public Nullable<int> FlagresID { get; set; }
        public int AddressLayout { get; set; }
        public int AddressLayoutDomestic { get; set; }
        public int AddressLayoutForeign { get; set; }
        public Nullable<int> CurrencyID { get; set; }
        public string OrgNrText { get; set; }
        public string DomainName { get; set; }
        public System.DateTime Registered { get; set; }
        public int Registered_associate_id { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public int updateDateAssociateId { get; set; }
        public short UpdateDateCount { get; set; }
        public Nullable<int> DefaultLCID { get; set; }
        public string DialInPrefix { get; set; }
        public string DialOutPrefix { get; set; }
        public string InterAreaPrefix { get; set; }
        public string CustomPhoneDesc { get; set; }
        public Nullable<short> PhonePartOfNA { get; set; }
        public Nullable<short> IsBuiltIn { get; set; }
        public Nullable<int> ISONumber { get; set; }
        public string Abbrev3 { get; set; }
        public string Abbrev2 { get; set; }
        public Nullable<int> TZLocationId { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Account> Account { get; set; }
    }
}