using Anteeo.Dashboard.Web.Monitoring;
using Microsoft.AspNet.SignalR;

namespace Anteeo.Dashboard.Web.SignalR
{
    public class MonitoringHub : Hub
    {
        private readonly IMonitoringService _monitoringService;

        public MonitoringHub(IMonitoringService monitoringService)
        {
            _monitoringService = monitoringService;

            _monitoringService.MonitoringResultPublished += MonitoringResultPublished;
        }

        public Models.Monitoring GetDashboard()
        {
            return _monitoringService.GetCurrentStatus();
        }

        public void MonitoringResultPublished(object service, MonitoringResult result)
        {
            Clients.All.broadcastMonitoring(result);
        }
    }
}