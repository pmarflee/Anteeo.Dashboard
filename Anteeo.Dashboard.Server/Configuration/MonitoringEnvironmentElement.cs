using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Anteeo.Dashboard.Server.Configuration
{
    public class MonitoringEnvironmentElement : ConfigurationElement, IMonitoringEnvironmentConfiguration
    {
        [ConfigurationProperty("name", DefaultValue = "", IsKey = true, IsRequired = true)]
        public string Name
        {
            get { return (string)base["name"]; }
            set { base["name"] = value; }
        }

        [ConfigurationProperty("sources")]
        public MonitoringSourceElementCollection Sources
        {
            get { return (MonitoringSourceElementCollection)base["sources"]; }
            set { base["sources"] = value; }
        }

        ICollection<IMonitoringSourceConfiguration> IMonitoringEnvironmentConfiguration.Sources
        {
            get
            {
                return Sources.Cast<IMonitoringSourceConfiguration>().ToList();
            }
        }
    }
}