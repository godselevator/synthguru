﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AOS.DataAccessLayer
{
    using SynthGuru.DomainModel;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class SynthGuruEntities : DbContext
    {
        public SynthGuruEntities()
            : base("name=SynthGuruEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Manufacturer> Manufacturer { get; set; }
        public virtual DbSet<SynthesisType> SynthesisType { get; set; }
        public virtual DbSet<SynthModel> SynthModel { get; set; }
    }
}