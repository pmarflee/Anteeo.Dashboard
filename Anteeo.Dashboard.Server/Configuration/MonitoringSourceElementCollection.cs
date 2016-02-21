using System;
using System.Configuration;

namespace Anteeo.Dashboard.Server.Configuration
{
    [ConfigurationCollection(typeof(MonitoringSourceElement))]
    public class MonitoringSourceElementCollection : ConfigurationElementCollection
    {
        internal const string PropertyName = "source";

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
            return new MonitoringSourceElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((MonitoringSourceElement)(element)).Name;
        }

        public MonitoringSourceElement this[int idx]
        {
            get
            {
                return (MonitoringSourceElement)BaseGet(idx);
            }
        }
    }
}