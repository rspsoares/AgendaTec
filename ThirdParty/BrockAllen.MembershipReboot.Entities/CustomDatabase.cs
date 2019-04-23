using System.Data.Entity;
using BrockAllen.MembershipReboot.Ef.Migrations;

namespace BrockAllen.MembershipReboot.Entities
{
    public class CustomDatabase : DbContext
    {
        static CustomDatabase()
        {
            Database.SetInitializer<CustomDatabase>(new System.Data.Entity.MigrateDatabaseToLatestVersion<CustomDatabase, Configuration>());
        }

        public CustomDatabase()
            : this("name=Subscriptions")
        {
            this.RegisterUserAccountChildTablesForDelete<CustomUserAccount>();
        }

        public CustomDatabase(string connectionName)
            : base(connectionName)
        {
            this.RegisterUserAccountChildTablesForDelete<CustomUserAccount>();
        }

        public DbSet<CustomUserAccount> UserAccountsTableWithSomeOtherName { get; set; }
        public DbSet<AuthenticationAudit> Audits { get; set; }
        public DbSet<PasswordHistory> PasswordHistory { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.ConfigureMembershipRebootUserAccounts<CustomUserAccount>();
        }
    }
}
