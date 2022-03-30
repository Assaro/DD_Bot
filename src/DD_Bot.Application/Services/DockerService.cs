using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DD_Bot.Application.Interfaces;
using DD_Bot.Application.Providers;
using DD_Bot.Domain;
using Microsoft.Extensions.Configuration;
using Renci.SshNet;

namespace DD_Bot.Application.Services
{
    public class DockerService : IDockerService
    {
        private IConfigurationRoot Configuration;

        public DockerService(IConfigurationRoot configuration)
        {
            Configuration = configuration;
        }
        public SshSettings Setting => Configuration.Get<Settings>().SshSettings;

        private string updateCommand = "docker ps -a --format \"{{.Names}}\\t{{.State}}\"";
        public Dictionary<string, bool> DockerStatus { get; }

        public void DockerUpdate()
        {
            string result;
            using (var client = new SshClient(Setting.ServerIp, Setting.SshPort, Setting.SshUser, Setting.SshPassword))
            {
                client.Connect();
                result = client.RunCommand(updateCommand).Result;
                client.Disconnect();
            }
            #region DictionaryUpdate
            string[] lines = result.Split('\n');
            foreach (string line in lines)
            {
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
                if (!result.Contains(line.Key))
                {
                    DockerStatus.Remove(line.Key);
                }
            }
            #endregion
        }

        public void DockerCommand(string commandName, string dockerName)
        {


            using( var client = new SshClient(Setting.ServerIp, Setting.SshPort, Setting.SshUser, Setting.SshPassword))
            {
                client.Connect();
                client.RunCommand(string.Format(commandName + dockerName));
                client.Disconnect();
            }
        }

    }
}
