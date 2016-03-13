using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Anteeo.Dashboard.Web.Configuration
{
    public class MonitoringSourceElement : ConfigurationElement, IMonitoringSourceConfiguration
    {
        [ConfigurationProperty("name", DefaultValue = "", IsKey = true, IsRequired = true)]
        public string Name
        {
            get { return (string)base["name"]; }
            set { base["name"] = value; }
        }

        [ConfigurationProperty("group", DefaultValue = "", IsKey = false, IsRequired = true)]
        public string Group
        {
            get { return (string)base["group"]; }
            set { base["group"] = value; }
        }

        [ConfigurationProperty("url", DefaultValue = "", IsKey = false, IsRequired = true)]
        public string Url
        {
            get { return (string)base["url"]; }
            set { base["url"] = value; }
        }

        [ConfigurationProperty("applicationPool", DefaultValue = "", IsKey = false, IsRequired = false)]
        public string ApplicationPool
        {
            get { return (string)base["applicationPool"]; }
            set { base["applicationPool"] = value; }
        }

        [ConfigurationProperty("monitorCPU", DefaultValue = false, IsKey = false, IsRequired = false)]
        public bool MonitorCPU
        {
            get { return (bool)base["monitorCPU"]; }
            set { base["monitorCPU"] = value; }
        }

        [ConfigurationProperty("databases", IsKey = false, IsRequired = false)]
        public MonitoringDatabaseElementCollection Databases
        {
            get { return (MonitoringDatabaseElementCollection)base["databases"]; }
            set { base["databases"] = value; }
        }

        ICollection<IMonitoringDatabaseConfiguration> IMonitoringSourceConfiguration.Databases
        {
            get
            {
                return Databases.Cast<IMonitoringDatabaseConfiguration>().ToList();
            }
        }
    }
}