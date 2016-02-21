using System.Collections.Generic;

namespace Anteeo.Dashboard.Server.Models
{
    public enum Status
    {
        GREEN = 1,
        AMBER = 2,
        RED = 3
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

        public IList<Database> Databases { get; private set; }
    }

    public class Database
    {
        public string Name { get; set; }
    }
}