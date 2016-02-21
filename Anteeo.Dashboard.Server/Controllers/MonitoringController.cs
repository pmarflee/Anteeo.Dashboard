using Anteeo.Dashboard.Server.Configuration;
using System.Web.Http;
using Anteeo.Dashboard.Server.Models;
using System.Linq;

namespace Anteeo.Dashboard.Server.Controllers
{
    public class MonitoringController : ApiController
    {
        public Models.Monitoring GetAllMonitoring()
        {
            var configuration = ConfigurationSettings.MonitoringConfiguration;

            var monitoringModel = new Models.Monitoring();

            foreach (var environment in configuration.Environments)
            {
                monitoringModel.Environments.Add(CreateEnvironment(environment));
            }

            return monitoringModel;
        }

        private Environment CreateEnvironment(IMonitoringEnvironmentConfiguration environment)
        {
            var environmentModel = new Environment();
            environmentModel.Name = environment.Name;

            foreach (var sourceGroup in environment.Sources.GroupBy(s => s.Group))
            {
                var group = CreateGroup(sourceGroup);

                environmentModel.Groups.Add(group);
            }

            return environmentModel;
        }

        private Group CreateGroup(IGrouping<string, IMonitoringSourceConfiguration> sourceGroup)
        {
            var groupModel = new Group { Name = sourceGroup.Key };
            
            foreach (var source in sourceGroup)
            {
                groupModel.Sources.Add(CreateSource(source));
            }

            return groupModel;
        }

        private Source CreateSource(IMonitoringSourceConfiguration source)
        {
            var sourceModel = new Source { Name = source.Name, Url = source.Url };

            foreach (var database in source.Databases)
            {
                sourceModel.Databases.Add(new Database { Name = database.Name });
            }

            return sourceModel;
        }
    }
}
