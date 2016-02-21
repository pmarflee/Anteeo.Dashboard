using System;
using System.Configuration;

namespace Anteeo.Dashboard.Server.Configuration
{
    [ConfigurationCollection(typeof(MonitoringDatabaseElement))]
    public class MonitoringDatabaseElementCollection : ConfigurationElementCollection
    {
        internal const string PropertyName = "database";

        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.BasicMapAlternate;
            }
        }

        protected override string ElementName
        {
            get
            {
                return PropertyName;
            }
        }

        protected override bool IsElementName(string elementName)
        {
            return elementName.Equals(PropertyName, StringComparison.InvariantCultureIgnoreCase);
        }


        public override bool IsReadOnly()
        {
            return false;
        }


        protected override ConfigurationElement CreateNewElement()
        {
            return new MonitoringDatabaseElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((MonitoringDatabaseElement)(element)).Name;
        }

        public MonitoringDatabaseElement this[int idx]
        {
            get
            {
                return (MonitoringDatabaseElement)BaseGet(idx);
            }
        }
    }
}