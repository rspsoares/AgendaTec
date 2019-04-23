using BrockAllen.MembershipReboot.Ef;
using System;
using System.Linq;

namespace BrockAllen.MembershipReboot.Entities
{
    public class CustomRepository : DbContextUserAccountRepository<CustomDatabase, CustomUserAccount>, IUserAccountRepository<CustomUserAccount>
    {
        // you can do either style ctor (or none) -- depends how much control 
        // you want over instantiating the CustomDatabase instance
        public CustomRepository()
            : base(new CustomDatabase())
        {
            this.isContextOwner = true;
        }
        public CustomRepository(string name)
            : base(new CustomDatabase(name))
        {
            this.isContextOwner = true;
        }
        public CustomRepository(CustomDatabase db)
            : base(db)
        {
        }

        protected override IQueryable<CustomUserAccount> DefaultQueryFilter(IQueryable<CustomUserAccount> query, string filter)
        {
            if (query == null) throw new ArgumentNullException("query");
            if (filter == null) throw new ArgumentNullException("filter");

            return
                from a in query
                from c in a.ClaimCollection
                where c.Value.Contains(filter)
                select a;
        }
    }
}
