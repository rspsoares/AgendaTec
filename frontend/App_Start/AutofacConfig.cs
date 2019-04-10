using System.Reflection;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using BrockAllen.MembershipReboot;
using BrockAllen.MembershipReboot.Ef;
using BrockAllen.MembershipReboot.WebHost;
using AgendaTech.View.Models;
using AgendaTech.Business.Contracts;
using AgendaTech.Business.Bindings;

namespace AgendaTech.View
{
    public class AutofacConfig
    {
        public static void ConfigureContainer()
        {
            var config = App_Start.MembershipRebootConfig.Create();
            var builder = new ContainerBuilder();
            builder.RegisterControllers(Assembly.GetAssembly(typeof(AutofacConfig)));

            #region MembershipReboot
            builder.RegisterInstance(config).As<MembershipRebootConfiguration<CustomUserAccount>>();            
            builder.RegisterType<SamAuthenticationService<CustomUserAccount>>().As<AuthenticationService<CustomUserAccount>>().InstancePerLifetimeScope();
            builder.RegisterType<UserAccountService<CustomUserAccount>>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<CustomRepository>()
                .As<IUserAccountRepository<CustomUserAccount>>()
                .As<IUserAccountQuery>()
                .InstancePerLifetimeScope();

            builder.RegisterType<DefaultGroupRepository>()
                .As<IGroupRepository>()
                .As<IGroupQuery>()
                .InstancePerLifetimeScope();
            builder.RegisterType<GroupService>().AsSelf().InstancePerLifetimeScope();
            #endregion
            
            builder.RegisterType<CustomerFacade>().As<ICustomerFacade>().InstancePerLifetimeScope();
            builder.RegisterType<ServiceFacade>().As<IServiceFacade>().InstancePerLifetimeScope();
            builder.RegisterType<ProfessionalFacade>().As<IProfessionalFacade>().InstancePerLifetimeScope();
            builder.RegisterType<UserFacade>().As<IUserFacade>().InstancePerLifetimeScope();

            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}