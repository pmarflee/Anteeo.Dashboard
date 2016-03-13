using System.Linq;
using System.Collections.Generic;
using System.Configuration;

namespace Anteeo.Dashboard.Web.Configuration
{
    public class MonitoringSection : ConfigurationSection, IMonitoringConfiguration
    {
        [ConfigurationProperty("pollInterval", IsKey = false, IsRequired = true)]
        public int PollInterval
        {
            get { return (int)base["pollInterval"]; }
            set { base["pollInterval"] = value; }
        }

        [ConfigurationProperty("performancePollInterval", IsKey = false, IsRequired = true)]
        public int PerformancePollInterval
        {
            get { return (int)base["performancePollInterval"]; }
            set { base["performancePollInterval"] = value; }
        }

        [ConfigurationProperty("environments")]
        public MonitoringEnvironmentElementCollection Environments
        {
            get { return (MonitoringEnvironmentElementCollection)base["environments"]; }
            set { base["environments"] = value; }
        }

        ICollection<IMonitoringEnvironmentConfiguration> IMonitoringConfiguration.Environments
        {
            get
            {
                return Environments.Cast<IMonitoringEnvironmentConfiguration>().ToList();
            }
        }
    }
}