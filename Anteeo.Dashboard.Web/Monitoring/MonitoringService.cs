using Anteeo.Dashboard.Web.Configuration;
using Anteeo.Dashboard.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Anteeo.Dashboard.Web.Monitoring
{
    public interface IMonitoringService
    {
        Models.Monitoring GetCurrentStatus();
        event EventHandler<MonitoringResult> MonitoringResultPublished;
    }

    public class MonitoringService : IMonitoringService
    {
        private readonly ITimerProvider _timerProvider;
        private readonly IMonitoringConfiguration _configuration;
        private readonly IMonitoringCommandHandler<DatabaseMonitoringCommand> _databaseMonitoringCommandHandler;
        private readonly IMonitoringCommandHandler<WebsiteMonitoringCommand> _websiteMonitoringCommandHandler;
        private readonly IMonitoringFactory _monitoringFactory;

        private Models.Monitoring _currentStatus;

        private volatile bool _updatingCurrentStatus;

        public MonitoringService(
            ITimerProvider timerProvider,
            IMonitoringConfiguration configuration,
            IMonitoringCommandHandler<WebsiteMonitoringCommand> websiteMonitoringCommandHandler,
            IMonitoringCommandHandler<DatabaseMonitoringCommand> databaseMonitoringCommandHandler,
            IMonitoringFactory monitoringFactory)
        {
            _timerProvider = timerProvider;
            _configuration = configuration;
            _websiteMonitoringCommandHandler = websiteMonitoringCommandHandler;
            _databaseMonitoringCommandHandler = databaseMonitoringCommandHandler;
            _monitoringFactory = monitoringFactory;

            SetCurrentStatus(Enumerable.Empty<MonitoringResult>());

            _timerProvider.Register(
                async state => await OnMonitor(state),
                TimeSpan.FromMilliseconds(_configuration.PollInterval));
        }

        public Models.Monitoring GetCurrentStatus()
        {
            return _currentStatus;
        }

        private async Task OnMonitor(object state)
        {
            if (_updatingCurrentStatus) return;

            try
            {

                _updatingCurrentStatus = true;

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
            }
            finally
            {
                _updatingCurrentStatus = false;
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
            _currentStatus = _monitoringFactory.Create(results);
        }

        public event EventHandler<MonitoringResult> MonitoringResultPublished;
        private void BroadcastMonitoringResult(MonitoringResult result)
        {
            if (MonitoringResultPublished != null)
                MonitoringResultPublished(this, result);
        }
    }
}