using System.Threading.Tasks;

namespace Anteeo.Dashboard.Server.Monitoring
{
    public interface IMonitoringCommandHandler
    {
        Task<MonitoringResult> Handle(MonitoringCommand command);
    }

    public interface IMonitoringCommandHandler<TMonitoringCommand> : IMonitoringCommandHandler
        where TMonitoringCommand : MonitoringCommand
    {
    }
}
