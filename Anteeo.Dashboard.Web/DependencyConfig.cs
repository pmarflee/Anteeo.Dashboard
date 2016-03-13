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

            builder.RegisterType<WebsiteMonitoringCommandHandler>()
                .SingleInstance()
                .Keyed<IMonitoringCommandHandler>(MonitoringType.Website);
            builder.RegisterType<DatabaseMonitoringCommandHandler>()
                .SingleInstance()
                .Keyed<IMonitoringCommandHandler>(MonitoringType.Database);
            builder.RegisterType<CPUUsageMonitoringCommandHandler>()
                .SingleInstance()
                .Keyed<IMonitoringCommandHandler>(MonitoringType.CPUUsage);

            builder.RegisterType<MonitoringFactory>().AsImplementedInterfaces().SingleInstance();

            builder.RegisterType<MonitoringService>().AsImplementedInterfaces().SingleInstance();

            builder.RegisterHubs(Assembly.GetExecutingAssembly());

            return builder.Build();
        }
    }
}