using Anteeo.Dashboard.Web.Configuration;
using Anteeo.Dashboard.Web.Models;
using Anteeo.Dashboard.Web.Monitoring;
using SimpleInjector;
using SimpleInjector.Integration.Web;
using System.Reflection;

namespace Anteeo.Dashboard.Web
{
    public static class DependencyConfig
    {
        public static Container Initialise()
        {
            var container = new Container();

            container.Options.DefaultScopedLifestyle = new WebRequestLifestyle();

            container.RegisterSingleton(ConfigurationSettings.MonitoringConfiguration);

            container.Register(typeof(IMonitoringCommandHandler<>), new[] { Assembly.GetExecutingAssembly() }, Lifestyle.Singleton);

            container.RegisterSingleton<IMonitoringFactory, MonitoringFactory>();

            container.RegisterSingleton<ITimerProvider, TimerProvider>();

            container.RegisterSingleton<IMonitoringService, MonitoringService>();

            container.Verify();

            return container;
        }
    }
}