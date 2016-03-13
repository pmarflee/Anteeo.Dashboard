using Anteeo.Dashboard.Web.Configuration;
using Anteeo.Dashboard.Web.Models;
using Anteeo.Dashboard.Web.Monitoring;
using Autofac;
using Autofac.Integration.SignalR;
using System.Reflection;

namespace Anteeo.Dashboard.Web
{
    public static class DependencyConfig
    {
        public static IContainer Initialise()
        {
            var builder = new ContainerBuilder();

            builder.RegisterInstance(ConfigurationSettings.MonitoringConfiguration).ExternallyOwned();

            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .AsClosedTypesOf(typeof(IMonitoringCommandHandler<>))
                .AsImplementedInterfaces()
                .SingleInstance();

            builder.RegisterType<MonitoringFactory>().AsImplementedInterfaces().SingleInstance();

            builder.RegisterType<TimerProvider>().AsImplementedInterfaces().SingleInstance();

            builder.RegisterType<MonitoringService>().AsImplementedInterfaces().SingleInstance();

            builder.RegisterHubs(Assembly.GetExecutingAssembly());

            return builder.Build();
        }
    }
}