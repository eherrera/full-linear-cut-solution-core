﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LinealCutOptimizer.Core
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class EFLinearCutSolutionEntities : DbContext
    {
        public EFLinearCutSolutionEntities()
            : base("name=EFLinearCutSolutionEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<MeasurementUnit> MeasurementUnit { get; set; }
        public virtual DbSet<BarPattern> BarPatterns { get; set; }
        public virtual DbSet<Params> Params { get; set; }
    }
}