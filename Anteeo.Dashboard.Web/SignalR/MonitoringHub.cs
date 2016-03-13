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
        }

        public Models.Monitoring GetDashboard()
        {
            return _monitoringService.CurrentStatus;
        }
    }
}