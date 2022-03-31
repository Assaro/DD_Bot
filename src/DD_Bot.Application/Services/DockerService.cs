using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DD_Bot.Application.Interfaces;
using DD_Bot.Domain;
using Microsoft.Extensions.Configuration;
using Renci.SshNet;
using Timer = System.Timers.Timer;

namespace DD_Bot.Application.Services
{
    public class DockerService : IDockerService
    {
        private IConfigurationRoot Configuration;
        public Timer UpdateTimer;
        public List<DockerContainer> DockerStatus { get; }
        private readonly string updateCommand = "docker ps -a --format \"{{.Names}}\\t{{.State}}\"";
        public SshSettings Setting => Configuration.Get<Settings>().SshSettings;


        public DockerService(IConfigurationRoot configuration)
        {
            Configuration = configuration;
            DockerStatus = new List<DockerContainer>();
            DockerUpdate();
            UpdateTimer = new Timer();
            UpdateTimer.Interval = TimeSpan.FromMinutes(5).TotalMilliseconds;
            UpdateTimer.Elapsed += (s, e) => DockerUpdate();
            UpdateTimer.AutoReset = true;
            UpdateTimer.Start();
        }

        public string[] RunningDockers => DockerStatus.Where(docker => docker.Running).Select(pairs => pairs.Name).ToArray();
        public string[] StoppedDockers => DockerStatus.Where(docker => !docker.Running).Select(pairs => pairs.Name).ToArray();

        public async Task DockerUpdate()
        {
            string result;
            using (var client = new SshClient(Setting.ServerIp, Setting.SshPort, Setting.SshUser, Setting.SshPassword))
            {
                client.Connect();
                result = client.RunCommand(updateCommand).Result;
                client.Disconnect();
            }

            DockerContainerUpdate(result);
            DockerContainerSort();
            Console.WriteLine("Updated Status");
            return;
        }

        public void DockerContainerUpdate(string sshData) 
        {
            string[] lines = sshData.Split('\n');
            foreach (string line in lines)
            {
                if (string.IsNullOrEmpty(line)) continue;

                string[] parts = line.Split('\t');

                bool status = false;
                if (parts[1] == "running")
                { 
                    status = true;
                }
                var docker = DockerStatus.FirstOrDefault(docker => docker.Name == parts[0]);
                if (docker == null)
                {
                    DockerStatus.Add(new DockerContainer(parts[0], status));
                }
                else
                {
                    docker.Running = status;
                }
                
            }

            foreach (var item in DockerStatus)
            {
                if (!sshData.Contains(item.Name))
                {
                    DockerStatus.Remove(item);
                }
            }
        }

        public void DockerContainerSort()
        {
            DockerStatus.Sort((x,y)=>x.Name.CompareTo(y.Name));
        }

        public int DockerStatusLongestName()
        {
            int counter = 0;
            foreach (var item in DockerStatus)
            {
                if (item.Name.Length> counter)
                {
                    counter = item.Name.Length;
                }
            }
            return counter;
        }

        public void Start()
        {
            Console.WriteLine("DockerService startet");
        }

        public async Task DockerCommand(string commandName, string dockerName)
        {
            using( var client = new SshClient(Setting.ServerIp, Setting.SshPort, Setting.SshUser, Setting.SshPassword))
            {
                client.Connect();
                client.RunCommand(string.Format("docker " +commandName + dockerName));
                client.Disconnect();
            }
            return;
        }
    }
}
