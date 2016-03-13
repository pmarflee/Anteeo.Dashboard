using Anteeo.Dashboard.Web.Configuration;
using Anteeo.Dashboard.Web.Models;
using Anteeo.Dashboard.Web.SignalR;
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
        private readonly IMonitoringCommandHandler<DatabaseMonitoringCommand> _databaseMonitoringCommandHandler;
        private readonly IMonitoringCommandHandler<WebsiteMonitoringCommand> _websiteMonitoringCommandHandler;
        private readonly IMonitoringFactory _monitoringFactory;
        private readonly IConnectionManager _connectionManager;

        private bool _polling;

        public MonitoringService(
            IMonitoringConfiguration configuration,
            IMonitoringCommandHandler<WebsiteMonitoringCommand> websiteMonitoringCommandHandler,
            IMonitoringCommandHandler<DatabaseMonitoringCommand> databaseMonitoringCommandHandler,
            IMonitoringFactory monitoringFactory,
            IConnectionManager connectionManager)
        {
            _configuration = configuration;
            _websiteMonitoringCommandHandler = websiteMonitoringCommandHandler;
            _databaseMonitoringCommandHandler = databaseMonitoringCommandHandler;
            _monitoringFactory = monitoringFactory;
            _connectionManager = connectionManager;
        }

        public Models.Monitoring CurrentStatus { get; private set; }

        public void Start()
        {
            SetCurrentStatus(Enumerable.Empty<MonitoringResult>());
            _polling = true;

            Task.Run(async () => await OnMonitor());
        }

        public void Stop()
        {
            _polling = false;
        }

        private async Task OnMonitor()
        {
           while (_polling)
           {
                var monitoringTasks = (from command in CreateCommands()
                                       let handler = (IMonitoringCommandHandler)GetHandlerForCommand((dynamic)command)
                                       select handler.Handle(command)).ToList();

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

        private IEnumerable<MonitoringCommand> CreateCommands()
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

        private IMonitoringCommandHandler GetHandlerForCommand(WebsiteMonitoringCommand command)
        {
            return _websiteMonitoringCommandHandler;
        }

        private IMonitoringCommandHandler GetHandlerForCommand(DatabaseMonitoringCommand command)
        {
            return _databaseMonitoringCommandHandler;
        }

        private void SetCurrentStatus(IEnumerable<MonitoringResult> results)
        {
            CurrentStatus = _monitoringFactory.Create(results);
        }

        private void BroadcastMonitoringResult(MonitoringResult result)
        {
            _connectionManager.GetHubContext<MonitoringHub>().Clients.All.broadcastMonitoring(result);
        }
    }
}