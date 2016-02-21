using Anteeo.Dashboard.Server.Configuration;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Anteeo.Dashboard.Server.Monitoring
{
    public class MonitoringEngine
    {
        private readonly IHubContext _hubContext;
        private readonly IMonitoringConfiguration _configuration;
        private readonly IMonitoringCommandHandler<DatabaseMonitoringCommand> _databaseMonitoringCommandHandler;
        private readonly IMonitoringCommandHandler<WebsiteMonitoringCommand> _websiteMonitoringCommandHandler;

        public MonitoringEngine(
            IHubContext hubContext,
            IMonitoringConfiguration configuration, 
            IMonitoringCommandHandler<WebsiteMonitoringCommand> websiteMonitoringCommandHandler,
            IMonitoringCommandHandler<DatabaseMonitoringCommand> databaseMonitoringCommandHandler)
        {
            _hubContext = hubContext;
            _configuration = configuration;
            _websiteMonitoringCommandHandler = websiteMonitoringCommandHandler;
            _databaseMonitoringCommandHandler = databaseMonitoringCommandHandler;
        }

        public async Task OnMonitor()
        {
            while (true)
            {
                await Task.Delay(_configuration.PollInterval);

                var monitoringTasks = (from command in CreateCommands()
                                      let handler = (IMonitoringCommandHandler)GetHandlerForCommand((dynamic)command)
                                      select handler.Handle(command)).ToList();

                while (monitoringTasks.Count > 0)
                {
                    var firstFinishedTask = await Task.WhenAny(monitoringTasks);

                    monitoringTasks.Remove(firstFinishedTask);

                    var result = await firstFinishedTask;

                    _hubContext.Clients.All.broadcastMonitoringResult(result);
                }

                _hubContext.Clients.All.serverTime(DateTime.UtcNow.ToString());
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
    }
}