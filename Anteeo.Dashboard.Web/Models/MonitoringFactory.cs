using Anteeo.Dashboard.Web.Configuration;
using Anteeo.Dashboard.Web.Monitoring;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Anteeo.Dashboard.Web.Models
{
    public interface IMonitoringFactory
    {
        Monitoring Create(IEnumerable<MonitoringResult> results);
    }

    public class MonitoringFactory : IMonitoringFactory
    {
        private readonly IMonitoringConfiguration _configuration;

        private static readonly StringComparison _comparisonType = StringComparison.InvariantCultureIgnoreCase;

        public MonitoringFactory(IMonitoringConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Monitoring Create(IEnumerable<MonitoringResult> results)
        {
            var monitoringModel = new Monitoring();

            foreach (var environment in _configuration.Environments)
            {
                monitoringModel.Environments.Add(
                    CreateEnvironment(environment, 
                    results.Where(result => result.Environment.Equals(environment.Name, _comparisonType)).ToList()));
            }

            return monitoringModel;
        }

        private Environment CreateEnvironment(
            IMonitoringEnvironmentConfiguration environment, IEnumerable<MonitoringResult> results)
        {
            var environmentModel = new Environment();
            environmentModel.Name = environment.Name;

            foreach (var sourceGroup in environment.Sources.ToLookup(s => s.Group))
            {
                var group = CreateGroup(sourceGroup, 
                    results.Where(result => result.Group.Equals(sourceGroup.Key, _comparisonType)).ToList());

                environmentModel.Groups.Add(group);
            }

            return environmentModel;
        }

        private Group CreateGroup(
            IGrouping<string, IMonitoringSourceConfiguration> sourceGroup,
            IEnumerable<MonitoringResult> results)
        {
            var groupModel = new Group { Name = sourceGroup.Key };

            foreach (var source in sourceGroup)
            {
                groupModel.Sources.Add(CreateSource(source, 
                    results.Where(result => result.Source.Equals(source.Name, _comparisonType)).ToList()));
            }

            return groupModel;
        }

        private Source CreateSource(
            IMonitoringSourceConfiguration source,
            IEnumerable<MonitoringResult> results)
        {
            var sourceResult = results.FirstOrDefault(result => result.Type == MonitoringResultType.Website);
            var sourceModel = new Source
            {
                Name = source.Name,
                Url = source.Url,
                Status = sourceResult?.Status,
                Message = sourceResult?.Message
            };

            var pairs = from database in source.Databases
                        join result in results.Where(result => result.Type == MonitoringResultType.Database)
                        on database.Name equals result.Name into matched
                        from match in matched.DefaultIfEmpty()
                        select new { database, result = match };

            foreach (var pair in pairs)
            {
                sourceModel.Databases.Add(new Database
                {
                    Name = pair.database.Name,
                    Status = pair.result?.Status,
                    Message = pair.result?.Message
                });
            }

            return sourceModel;
        }
    }
}