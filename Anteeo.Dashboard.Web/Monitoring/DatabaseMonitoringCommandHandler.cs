using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Anteeo.Dashboard.Web.Monitoring
{
    public class DatabaseMonitoringCommandHandler : IMonitoringCommandHandler<DatabaseMonitoringCommand>
    {
        public async Task<MonitoringResult> Handle(MonitoringCommand command)
        {
            Models.Status status;
            string message;

            using (var conn = new SqlConnection(command.Connection))
            {
                try
                {
                    await conn.OpenAsync();

                    status = Models.Status.Success;
                    message = "OK";
                }
                catch (SqlException ex)
                {
                    status = Models.Status.Danger;
                    message = ex.GetBaseException().Message;
                }

                return new MonitoringResult
                {
                    Environment = command.Environment,
                    Group = command.Group,
                    Source = command.Source,
                    Type = MonitoringType.Database,
                    Name = command.Name,
                    Status = status,
                    Message = message
                };
            }
        }
    }
}