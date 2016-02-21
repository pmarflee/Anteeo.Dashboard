namespace Anteeo.Dashboard.Server.Monitoring
{
    public enum MonitoringResultType
    {
        Website = 1,
        Database = 2
    }

    public class MonitoringResult
    {
        public string Environment { get; set; }
        public string Group { get; set; }
        public string Source { get; set; }
        public MonitoringResultType Type { get; set; }
        public string Name { get; set; }
        public Models.Status Status { get; set; }
        public string Message { get; set; }
    }
}