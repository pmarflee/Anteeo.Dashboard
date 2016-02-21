using Microsoft.Owin;
using Owin;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;
using Anteeo.Dashboard.Server.Monitoring;
using Anteeo.Dashboard.Server.Configuration;
using System.Threading.Tasks;
using Anteeo.Dashboard.Server.Hubs;

[assembly: OwinStartup(typeof(Anteeo.Dashboard.Server.SignalRStartup))]

namespace Anteeo.Dashboard.Server
{
    public class SignalRStartup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll);
            var hubConfiguration = new HubConfiguration();
            hubConfiguration.EnableDetailedErrors = true;
            app.MapSignalR(hubConfiguration);

            MonitoringEngine monitoringEngine = new MonitoringEngine(
                GlobalHost.ConnectionManager.GetHubContext<MonitoringHub>(),
                ConfigurationSettings.MonitoringConfiguration,
                new WebsiteMonitoringCommandHandler(),
                new DatabaseMonitoringCommandHandler());

            Task.Factory.StartNew(async () => await monitoringEngine.OnMonitor());
        }
    }
}
