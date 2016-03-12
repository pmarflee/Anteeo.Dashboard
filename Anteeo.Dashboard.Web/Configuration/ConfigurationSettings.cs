using System.Configuration;

namespace Anteeo.Dashboard.Web.Configuration
{
    public static class ConfigurationSettings
    {
        public static IMonitoringConfiguration MonitoringConfiguration
        {
            get
            {
                return (MonitoringSection)ConfigurationManager.GetSection("monitoring");
            }
        }
    }
}