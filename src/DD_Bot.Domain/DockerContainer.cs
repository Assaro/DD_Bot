using System;

namespace DD_Bot.Domain
{
    public class DockerContainer
    {
        public string Name { get; set; }
        public bool Running { get; set; }
        public TimeSpan uptime { get; set; }

        public DockerContainer(string name, bool running)
        {
            Name = name;
            Running = running;
        }

        public DockerContainer(string name, bool running, TimeSpan uptime) : this(name, running)
        {
            this.uptime = uptime;
        }
    }
}
