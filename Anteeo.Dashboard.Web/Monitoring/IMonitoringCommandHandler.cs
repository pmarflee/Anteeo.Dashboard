using System.Threading.Tasks;

namespace Anteeo.Dashboard.Web.Monitoring
{
    public interface IMonitoringCommandHandler
    {
        Task<MonitoringResult> Handle(MonitoringCommand command);
    }

    public interface IMonitoringCommandHandler<out TMonitoringCommand> : IMonitoringCommandHandler
        where TMonitoringCommand : MonitoringCommand
    {
    }
}
