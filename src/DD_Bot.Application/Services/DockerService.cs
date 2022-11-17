/* DD_Bot - A Discord Bot to control Docker containers*/

/*  Copyright (C) 2022 Maxim Kovac

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.

*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DD_Bot.Application.Interfaces;
using DD_Bot.Domain;
using Microsoft.Extensions.Configuration;
using Docker.DotNet;
using Docker.DotNet.Models;
using Timer = System.Timers.Timer;

namespace DD_Bot.Application.Services
{
    public class DockerService : IDockerService
    {
        private readonly IConfigurationRoot _configuration;
        private IList<ContainerListResponse> _dockerResponse;
        public List<ContainerListResponse> DockerStatus { get; private set; }
        private readonly DockerClient _client = new DockerClientConfiguration(
                new Uri("unix:///var/run/docker.sock"))
            .CreateClient();

        public DockerSettings Settings => _configuration.Get<Settings>().DockerSettings;
        public DockerService(IConfigurationRoot configuration) // Initialising
        {
            _configuration = configuration;
            DockerUpdate();
            var updateTimer = new Timer();
            updateTimer.Interval = TimeSpan.FromMinutes(5).TotalMilliseconds;
            updateTimer.Elapsed += (s, e) => DockerUpdate();
            updateTimer.AutoReset = true;
            updateTimer.Start();
        }

     public string[] RunningDockers => DockerStatus.Where(docker => docker.Status.Contains("Up")).Select(pairs => pairs.Names[0]).ToArray();
     public string[] StoppedDockers => DockerStatus.Where(docker => !docker.Status.Contains("Up")).Select(pairs => pairs.Names[0]).ToArray();
     
        public async Task DockerUpdate() //Update
        {
            _dockerResponse = await _client.Containers.ListContainersAsync(new ContainersListParameters(){All = true});
            DockerStatus = new List<ContainerListResponse>();
            foreach (var variable in _dockerResponse)
            {
                DockerStatus.Add(variable);
            }

            if (DockerStatus == null) return;
            {
                foreach (var variable in DockerStatus)
                {
                    variable.Names[0] = variable.Names[0].Substring(1);
                }
            }
        }

        public void DockerContainerSort()
        {
            DockerStatus.Sort((x,y)=>String.Compare(x.Names[0], y.Names[0], StringComparison.Ordinal));
        }

        public int DockerStatusLongestName()
        {
            int counter = 0;
            foreach (var item in DockerStatus)
            {
                if (item.Names[0].Length> counter)
                {
                    counter = item.Names[0].Length;
                }
            }
            return counter;
        }

        public async void DockerCommandStart(string id)
        {
            await _client.Containers.StartContainerAsync(id, new ContainerStartParameters());
        }

        public async void DockerCommandStop(string id)
        {
            await _client.Containers.StopContainerAsync(id, new ContainerStopParameters());
        }

        public async void DockerCommandRestart(string id)
        {
            await _client.Containers.RestartContainerAsync(id, new ContainerRestartParameters());
        }
        
        public void Start()
        {
            Console.WriteLine("DockerService started");
        }
    }
}
