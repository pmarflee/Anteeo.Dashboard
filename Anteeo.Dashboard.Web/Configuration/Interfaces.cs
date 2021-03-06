﻿using System.Collections.Generic;

namespace Anteeo.Dashboard.Web.Configuration
{
    public interface IMonitoringConfiguration
    {
        int PollInterval { get; }
        int PerformancePollInterval { get; }
        ICollection<IMonitoringEnvironmentConfiguration> Environments { get; }
    }

    public interface IMonitoringEnvironmentConfiguration
    {
        string Name { get; }
        ICollection<IMonitoringSourceConfiguration> Sources { get; }
    }

    public interface IMonitoringSourceConfiguration
    {
        string Name { get; }
        string Group { get; }
        string Url { get; }
        string ApplicationPool { get; }
        bool MonitorCPU { get; }
        ICollection<IMonitoringDatabaseConfiguration> Databases { get; }
    }

    public interface IMonitoringDatabaseConfiguration
    {
        string Name { get; }
        string ConnectionString { get; }
    }
}
