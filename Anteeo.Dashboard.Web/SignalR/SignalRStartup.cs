using Microsoft.Owin;
using Owin;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Autofac.Integration.SignalR;
using Microsoft.AspNet.SignalR.Infrastructure;
using Autofac;
using Anteeo.Dashboard.Web.Monitoring;

[assembly: OwinStartup(typeof(Anteeo.Dashboard.Web.SignalR.SignalRStartup))]

namespace Anteeo.Dashboard.Web.SignalR
{
    public class SignalRStartup
    {
        public void Configuration(IAppBuilder app)
        {
            var container = DependencyConfig.Initialise();

            var hubConfig = new HubConfiguration();
            var dependencyResolver = new AutofacDependencyResolver(container);
            hubConfig.Resolver = dependencyResolver;

            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new StringEnumConverter { CamelCaseText = true });
            settings.ContractResolver = new SignalRContractResolver();
            var serializer = JsonSerializer.Create(settings);
            dependencyResolver.Register(typeof(JsonSerializer), () => serializer);

            app.UseAutofacMiddleware(container);
            app.MapSignalR(hubConfig);

            var builder = new ContainerBuilder();
            var connManager = hubConfig.Resolver.Resolve<IConnectionManager>();
            builder.RegisterInstance(connManager)
                .As<IConnectionManager>()
                .SingleInstance();
            builder.Update(container);

            var monitoringService = container.Resolve<IMonitoringService>();
            monitoringService.Start();
        }
    }
}
