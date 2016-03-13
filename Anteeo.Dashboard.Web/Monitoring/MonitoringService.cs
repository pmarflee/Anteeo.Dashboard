using Anteeo.Dashboard.Web.Configuration;
using Anteeo.Dashboard.Web.Models;
using Anteeo.Dashboard.Web.SignalR;
using Autofac.Features.Indexed;
using Microsoft.AspNet.SignalR.Infrastructure;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Anteeo.Dashboard.Web.Monitoring
{
    public interface IMonitoringService
    {
        Models.Monitoring CurrentStatus { get; }
        void Start();
        void Stop();
    }

    public class MonitoringService : IMonitoringService
    {
        private readonly IMonitoringConfiguration _configuration;
        private readonly IIndex<MonitoringType, IMonitoringCommandHandler> _commandHandlers;
        private readonly IMonitoringFactory _monitoringFactory;
        private readonly IConnectionManager _connectionManager;

        private bool _polling;

        public MonitoringService(
            IMonitoringConfiguration configuration,
            IIndex<MonitoringType, IMonitoringCommandHandler> commandHandlers,
            IMonitoringFactory monitoringFactory,
            IConnectionManager connectionManager)
        {
            _configuration = configuration;
            _commandHandlers = commandHandlers;
            _monitoringFactory = monitoringFactory;
            _connectionManager = connectionManager;
        }

        public Models.Monitoring CurrentStatus { get; private set; }

        public void Start()
        {
            SetCurrentStatus(Enumerable.Empty<MonitoringResult>());
            _polling = true;

            Task.Run(async () => await OnMonitor());
            Task.Run(async () => await OnMonitorPerformance());
        }

        public void Stop()
        {
            _polling = false;
        }

        private async Task OnMonitor()
        {
           while (_polling)
           {
                var monitoringTasks = CreateMonitoringCommands()
                    .Select(command => _commandHandlers[command.Type].Handle(command))
                    .ToList();

                var results = new List<MonitoringResult>(monitoringTasks.Count);

                while (monitoringTasks.Count > 0)
                {
                    var firstFinishedTask = await Task.WhenAny(monitoringTasks);

                    monitoringTasks.Remove(firstFinishedTask);

                    var result = await firstFinishedTask;

                    results.Add(result);

                    BroadcastMonitoringResult(result);
                }

                SetCurrentStatus(results);

                await Task.Delay(_configuration.PollInterval);
            }            
        }

        private async Task OnMonitorPerformance()
        {
            while (_polling)
            {
                var monitoringTasks = CreatePerformanceMonitoringCommands()
                    .Select(command => _commandHandlers[MonitoringType.CPUUsage].Handle(command))
                    .ToList();

                var results = new List<MonitoringResult>(monitoringTasks.Count);

                while (monitoringTasks.Count > 0)
                {
                    var firstFinishedTask = await Task.WhenAny(monitoringTasks);

                    monitoringTasks.Remove(firstFinishedTask);

                    var result = await firstFinishedTask;

                    results.Add(result);

                    BroadcastPerformanceMonitoringResult(result);
                }

                await Task.Delay(_configuration.PerformancePollInterval);
            }
        }

        private IEnumerable<MonitoringCommand> CreateMonitoringCommands()
        {
            foreach (var environment in _configuration.Environments)
            {
                foreach (var source in environment.Sources)
                {
                    yield return new WebsiteMonitoringCommand(environment, source);

                    foreach (var database in source.Databases)
                    {
                        yield return new DatabaseMonitoringCommand(environment, source, database);
                    }
                }
            }
        }

        private IEnumerable<MonitoringCommand> CreatePerformanceMonitoringCommands()
        {
            return from environment in _configuration.Environments
                   from source in environment.Sources
                   where source.MonitorCPU && !string.IsNullOrEmpty(source.ApplicationPool)
                   select new CPUUsageMonitoringCommand(environment, source);
        }

        private void SetCurrentStatus(IEnumerable<MonitoringResult> results)
        {
            CurrentStatus = _monitoringFactory.Create(results);
        }

        private void BroadcastMonitoringResult(MonitoringResult result)
        {
            _connectionManager.GetHubContext<MonitoringHub>().Clients.All.broadcastMonitoring(result);
        }

        private void BroadcastPerformanceMonitoringResult(MonitoringResult result)
        {
            _connectionManager.GetHubContext<MonitoringHub>().Clients.All.broadcastPerformanceMonitoring(result);
        }
    }
}