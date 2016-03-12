using System;
using System.Configuration;

namespace Anteeo.Dashboard.Web.Configuration
{
    [ConfigurationCollection(typeof(MonitoringEnvironmentElement))]
    public class MonitoringEnvironmentElementCollection : ConfigurationElementCollection
    {
        internal const string PropertyName = "environment";

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
            return new MonitoringEnvironmentElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((MonitoringEnvironmentElement)(element)).Name;
        }

        public MonitoringEnvironmentElement this[int idx]
        {
            get
            {
                return (MonitoringEnvironmentElement)BaseGet(idx);
            }
        }
    }
}