using System.Collections.Generic;

namespace Anteeo.Dashboard.Server.Configuration
{
    public interface IMonitoringConfiguration
    {
        int PollInterval { get; }
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
        ICollection<IMonitoringDatabaseConfiguration> Databases { get; }
    }

    public interface IMonitoringDatabaseConfiguration
    {
        string Name { get; }
        string ConnectionString { get; }
    }
}
