using System.Reflection;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using AgendaTec.Business.Contracts;
using AgendaTec.Business.Bindings;

namespace AgendaTec.Portal.App_Start
{
    public class AutofacConfig
    {
        public static void ConfigureContainer()
        {            
            var builder = new ContainerBuilder();
            builder.RegisterControllers(Assembly.GetAssembly(typeof(AutofacConfig)));

            builder.RegisterType<CustomerFacade>().As<ICustomerFacade>().InstancePerLifetimeScope();
            builder.RegisterType<ServiceFacade>().As<IServiceFacade>().InstancePerLifetimeScope();
            builder.RegisterType<ProfessionalServiceFacade>().As<IProfessionalServiceFacade>().InstancePerLifetimeScope();
            builder.RegisterType<ProfessionalFacade>().As<IProfessionalFacade>().InstancePerLifetimeScope();
            builder.RegisterType<UserFacade>().As<IUserFacade>().InstancePerLifetimeScope();
            builder.RegisterType<ScheduleFacade>().As<IScheduleFacade>().InstancePerLifetimeScope();
            builder.RegisterType<DirectMailFacade>().As<IDirectMailFacade>().InstancePerLifetimeScope();

            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}