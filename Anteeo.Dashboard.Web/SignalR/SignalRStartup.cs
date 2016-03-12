using Microsoft.Owin;
using Owin;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[assembly: OwinStartup(typeof(Anteeo.Dashboard.Web.SignalR.SignalRStartup))]

namespace Anteeo.Dashboard.Web.SignalR
{
    public class SignalRStartup
    {
        public void Configuration(IAppBuilder app)
        {
            var container = DependencyConfig.Initialise();
            var activator = new SimpleInjectorHubActivator(container);

            GlobalHost.DependencyResolver.Register(typeof(IHubActivator), () => activator);

            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new StringEnumConverter { CamelCaseText = true });
            settings.ContractResolver = new SignalRContractResolver();
            var serializer = JsonSerializer.Create(settings);
            GlobalHost.DependencyResolver.Register(typeof(JsonSerializer), () => serializer);

            app.MapSignalR();
        }
    }
}
