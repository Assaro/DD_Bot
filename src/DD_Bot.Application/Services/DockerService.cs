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
        public Dictionary<string, bool> DockerStatus { get; }
        private readonly string updateCommand = "docker ps -a --format \"{{.Names}}\\t{{.State}}\"";
        public SshSettings Setting => Configuration.Get<Settings>().SshSettings;


        public DockerService(IConfigurationRoot configuration)
        {
            Configuration = configuration;
            DockerStatus = new Dictionary<string, bool>();
#pragma warning disable CS4014 // Da auf diesen Aufruf nicht gewartet wird, wird die Ausführung der aktuellen Methode vor Abschluss des Aufrufs fortgesetzt.
            UpdateTimer.Elapsed += (s, e) => DockerUpdate();
            DockerUpdate();
            UpdateTimer = new Timer();
#pragma warning restore CS4014 // Da auf diesen Aufruf nicht gewartet wird, wird die Ausführung der aktuellen Methode vor Abschluss des Aufrufs fortgesetzt.
            UpdateTimer.Interval = TimeSpan.FromMinutes(5).TotalMilliseconds;
            UpdateTimer.AutoReset = true;
            UpdateTimer.Start();
        }

        public string[] RunningDockers => DockerStatus.Where(docker => docker.Value).Select(pairs=>pairs.Key).ToArray();
        public string[] StoppedDockers => DockerStatus.Where(docker => !docker.Value).Select(pairs => pairs.Key).ToArray();

        public async Task DockerUpdate()
        {
            string result;
            using (var client = new SshClient(Setting.ServerIp, Setting.SshPort, Setting.SshUser, Setting.SshPassword))
            {
                client.Connect();
                result = client.RunCommand(updateCommand).Result;
                client.Disconnect();
            }

            DictionaryUpdate(result);
            Console.WriteLine("Updated Status");
            return;
        }

        public void DictionaryUpdate(string sshData) 
        {
            string[] lines = sshData.Split('\n');
            foreach (string line in lines)
            {
                if (string.IsNullOrEmpty(line)) continue;

                string[] parts = line.Split('\t');

                bool status = false;
                if (parts[1] == "running")

                { status = true; }
                try
                {
                    DockerStatus.Add(parts[0], status);
                }
                catch (ArgumentException)
                {
                    DockerStatus[parts[0]] = status;
                }
            }

            foreach (var line in DockerStatus)
            {
                if (!sshData.Contains(line.Key))
                {
                    DockerStatus.Remove(line.Key);
                }
            }
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
