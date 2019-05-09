using AgendaTec.Business.Profiles;
using AutoMapper;

namespace AgendaTec.Business.Helpers
{
    public static class ProfilesHelper
    {
        public static void Initialize()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile<CustomerProfile>();



            });
        }

        public static void Reset()
        {
            Mapper.Reset();
        }
    }
}
