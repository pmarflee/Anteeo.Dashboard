using System;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Anteeo.Dashboard.Web.Monitoring
{
    public class CPUUsageMonitoringCommandHandler : IMonitoringCommandHandler<CPUUsageMonitoringCommand>
    {
        private static readonly Regex _regex = new Regex(@"-ap\s""(\w+)""", RegexOptions.Compiled);

        public async Task<MonitoringResult> Handle(MonitoringCommand command)
        {
            var result = new PerformanceMonitoringResult
            {
                Environment = command.Environment,
                Group = command.Group,
                Source = command.Source,
                Name = command.Name,
                Type = MonitoringType.CPUUsage,
            };

            try
            {
                var scope = new ManagementScope($@"\\{command.Environment}\root\cimv2");
                var query = new SelectQuery("SELECT * FROM Win32_Process where Name = 'w3wp.exe'");
                int? processId;

                using (var searcher = new ManagementObjectSearcher(scope, query))
                {
                    processId = (from p in searcher.Get().Cast<ManagementObject>()
                                 let commandLine = p["CommandLine"]
                                 where commandLine != null && _regex.IsMatch(commandLine.ToString())
                                 select int.Parse(p["ProcessId"].ToString())).FirstOrDefault();
                }

                if (processId.HasValue)
                {
                    var processInstanceName = GetProcessInstanceName(processId.Value);
                    var cpuCounter = new PerformanceCounter("Process", "% Processor Time", processInstanceName, true);
                    cpuCounter.NextValue();
                    await Task.Delay(1000);
                    result.Value = cpuCounter.NextValue();
                    result.Message = $"% Processor Time: {result.Value:0.00}";
                    if (result.Value < 80)
                    {
                        result.Status = Models.Status.Success;
                    }
                    else if (result.Value < 90)
                    {
                        result.Status = Models.Status.Warning;
                    }
                    else
                    {
                        result.Status = Models.Status.Danger;
                    }
                }
            }
            catch (Exception ex)
            {
                result.Message = $"Unable to monitor CPU usage for {result.Environment}\\{result.Source}. Error is '{ex.Message}'";
                result.Status = Models.Status.Danger;
            }

            return result;
        }

        private static string GetProcessInstanceName(int processId)
        {
            PerformanceCounterCategory cat = new PerformanceCounterCategory("Process");

            string[] instances = cat.GetInstanceNames();
            foreach (string instance in instances)
            {
                using (PerformanceCounter cnt = new PerformanceCounter("Process",
                     "ID Process", instance, true))
                {
                    int val = (int)cnt.RawValue;
                    if (val == processId)
                    {
                        return instance;
                    }
                }
            }

            throw new Exception("Could not find performance counter " +
                "instance name for current process. This is truly strange ...");
        }
    }
}