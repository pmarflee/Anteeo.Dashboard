using System.Net.Http;
using System.Threading.Tasks;

namespace Anteeo.Dashboard.Web.Monitoring
{
    public class WebsiteMonitoringCommandHandler : IMonitoringCommandHandler<WebsiteMonitoringCommand>
    {
        public async Task<MonitoringResult> Handle(MonitoringCommand command)
        {
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, command.Connection)))
                {
                    return new MonitoringResult
                    {
                        Environment = command.Environment,
                        Group = command.Group,
                        Source = command.Source,
                        Type = MonitoringResultType.Website,
                        Name = command.Name,
                        Status = response.IsSuccessStatusCode ? Models.Status.Success : Models.Status.Danger,
                        Message = response.ReasonPhrase
                    };
                }
            }
        }
    }
}