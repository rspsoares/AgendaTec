﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AgendaTech.Infrastructure.DatabaseModel
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class AgendaTechEntities : DbContext
    {
        public AgendaTechEntities()
            : base("name=AgendaTechEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<AspNetUserClaims> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogins> AspNetUserLogins { get; set; }
        public virtual DbSet<TCGServices> TCGServices { get; set; }
        public virtual DbSet<TSchedules> TSchedules { get; set; }
        public virtual DbSet<AspNetRoles> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUsers> AspNetUsers { get; set; }
        public virtual DbSet<TCGProfessionals> TCGProfessionals { get; set; }
        public virtual DbSet<C__MigrationHistory> C__MigrationHistory { get; set; }
        public virtual DbSet<AuthenticationAudits> AuthenticationAudits { get; set; }
        public virtual DbSet<LinkedAccountClaims> LinkedAccountClaims { get; set; }
        public virtual DbSet<LinkedAccounts> LinkedAccounts { get; set; }
        public virtual DbSet<PasswordHistories> PasswordHistories { get; set; }
        public virtual DbSet<PasswordResetSecrets> PasswordResetSecrets { get; set; }
        public virtual DbSet<TCGCustomers> TCGCustomers { get; set; }
        public virtual DbSet<TwoFactorAuthTokens> TwoFactorAuthTokens { get; set; }
        public virtual DbSet<UserAccounts> UserAccounts { get; set; }
        public virtual DbSet<UserCertificates> UserCertificates { get; set; }
        public virtual DbSet<UserClaims> UserClaims { get; set; }
    }
}
