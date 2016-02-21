using Microsoft.AspNet.SignalR;

namespace Anteeo.Dashboard.Server.Hubs
{
    public class MonitoringHub : Hub
    {
        public void SendMonitoring(Models.Monitoring monitoringModel)
        {
            Clients.All.broadcastMonitoring(monitoringModel);
        }
    }
}