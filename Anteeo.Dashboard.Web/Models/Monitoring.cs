using System.Collections.Generic;

namespace Anteeo.Dashboard.Web.Models
{
    public enum Status
    {
        Success = 1,
        Warning = 2,
        Danger = 3
    }

    public class Monitoring
    {
        public Monitoring()
        {
            Environments = new List<Environment>();
        }

        public IList<Environment> Environments { get; private set; }
    }

    public class Environment
    {
        public Environment()
        {
            Groups = new List<Group>();
        }

        public string Name { get; set; }

        public IList<Group> Groups { get; private set; }
    }

    public class Group
    {
        public Group()
        {
            Sources = new List<Source>();
        }

        public string Name { get; set; }

        public IList<Source> Sources { get; private set; }
    }

    public class Source
    {
        public Source()
        {
            Databases = new List<Database>();
        }

        public string Name { get; set; }

        public string Url { get; set; }

        public Status? Status { get; set; }

        public string Message { get; set; }

        public bool MonitorCPU { get; set; }

        public float PercentProcessorTime { get; set; }

        public IList<Database> Databases { get; private set; }
    }

    public class Database
    {
        public string Name { get; set; }

        public Status? Status { get; set; }

        public string Message { get; set; }
    }
}