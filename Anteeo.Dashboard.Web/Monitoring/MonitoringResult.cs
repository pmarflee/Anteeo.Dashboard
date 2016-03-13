namespace Anteeo.Dashboard.Web.Monitoring
{
    public class MonitoringResult
    {
        public string Environment { get; set; }
        public string Group { get; set; }
        public string Source { get; set; }
        public MonitoringType Type { get; set; }
        public string Name { get; set; }
        public Models.Status Status { get; set; }
        public string Message { get; set; }
    }
}