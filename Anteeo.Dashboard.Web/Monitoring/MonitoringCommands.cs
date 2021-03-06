﻿using Anteeo.Dashboard.Web.Configuration;

namespace Anteeo.Dashboard.Web.Monitoring
{
    public abstract class MonitoringCommand
    {
        public MonitoringType Type { get; protected set; }
        public string Environment { get; protected set; }
        public string Group { get; protected set; }
        public string Source { get; protected set; }
        public string Name { get; protected set; }
        public string Connection { get; protected set; }
    }

    public class WebsiteMonitoringCommand : MonitoringCommand
    {
        public WebsiteMonitoringCommand(
            IMonitoringEnvironmentConfiguration environment,
            IMonitoringSourceConfiguration source)
        {
            Type = MonitoringType.Website;
            Environment = environment.Name;
            Group = source.Group;
            Source = source.Name;
            Name = source.Name;
            Connection = source.Url;
        }
    }

    public class DatabaseMonitoringCommand : MonitoringCommand
    {
        public DatabaseMonitoringCommand(
            IMonitoringEnvironmentConfiguration environment,
            IMonitoringSourceConfiguration source,
            IMonitoringDatabaseConfiguration database)
        {
            Type = MonitoringType.Database;
            Environment = environment.Name;
            Group = source.Group;
            Source = source.Name;
            Name = database.Name;
            Connection = database.ConnectionString;
        }
    }

    public class CPUUsageMonitoringCommand : MonitoringCommand
    {
        public CPUUsageMonitoringCommand(
            IMonitoringEnvironmentConfiguration environment,
            IMonitoringSourceConfiguration source)
        {
            Type = MonitoringType.Website;
            Environment = environment.Name;
            Group = source.Group;
            Source = source.Name;
            Name = source.Name;
            Connection = source.ApplicationPool;
        }
    }
}