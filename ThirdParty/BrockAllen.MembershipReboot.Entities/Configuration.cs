namespace BrockAllen.MembershipReboot.Entities
{
    public class Configuration : System.Data.Entity.Migrations.DbMigrationsConfiguration<CustomDatabase>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(CustomDatabase context)
        {
        }
    }
}
