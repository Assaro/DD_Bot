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
using Discord;
using Discord.WebSocket;
using DD_Bot.Application.Services;
using DD_Bot.Domain;
using System.Linq;
using Docker.DotNet.Models;

namespace DD_Bot.Application.Commands
{
    internal class ListCommand
    {
        private DiscordSocketClient _discord;
        public ListCommand(DiscordSocketClient discord)
        {
            _discord = discord;
        }

        #region CreateCommand
        public static ApplicationCommandProperties Create()
        {
            var builder = new SlashCommandBuilder()
            {
                Name = "list",
                Description = " List all Docker containers"
            };
            return builder.Build();
        }

        #endregion

        #region ExecuteCommand

        public static async void Execute(SocketSlashCommand arg, DockerService dockerService, DiscordSettings settings)
        {
            await arg.RespondAsync("Contacting Docker Service...");
            await dockerService.DockerUpdate();
            List<string> allowedContainers = new List<string>();
            
            if (!settings.AdminIDs.Contains(arg.User.Id))
            {
                var socketUser = arg.User as SocketGuildUser;
                var guild = socketUser.Guild;
                var socketGuildUser = guild.GetUser(socketUser.Id);
                var userRoles = socketGuildUser.Roles;
                
                if (settings.UserStartPermissions.ContainsKey(arg.User.Id))
                {
                    allowedContainers.AddRange(settings.UserStartPermissions[arg.User.Id]);   
                }
                if (settings.UserStopPermissions.ContainsKey(arg.User.Id))
                {
                    allowedContainers.AddRange(settings.UserStopPermissions[arg.User.Id]);   
                }

                foreach (var role in userRoles)
                {
                    if (settings.RoleStartPermissions.ContainsKey(role.Id))
                    {
                        allowedContainers.AddRange(settings.RoleStartPermissions[role.Id]);
                    }
                    if (settings.RoleStopPermissions.ContainsKey(role.Id))
                    {
                        allowedContainers.AddRange(settings.RoleStopPermissions[role.Id]);
                    }
                }
                allowedContainers.Distinct();
            }
            
            int maxLength = dockerService.DockerStatusLongestName();
            maxLength++;
            if (maxLength< 14)
            {
                maxLength = 14;
            }
            string outputHeader = new string('¯', 12 + maxLength)
                            + "\n|Containername"
                            + new string(' ', maxLength - 13)
                            + "| Status  |\n"
                            + new string('¯', 12 + maxLength)
                            + "\n";
            
            string outputFooter = new string('¯', 12 + maxLength) + "\n" + "```";
            
            if (dockerService.DockerStatus.Count > dockerService.Settings.ContainersPerMessage)
            {
                string output;
                string outputList;
                List < List < ContainerListResponse >> partitionedContainerList =
                    dockerService.DockerStatus.Partition(dockerService.Settings.ContainersPerMessage);
                for (int i = 0; i < partitionedContainerList.Count; i++)
                {
                    output = String.Empty;
                    outputList = FormatListObjects(partitionedContainerList[i], settings, maxLength, arg, allowedContainers);

                    int n = i + 1;
                    output = $"**List of all known Containers ({n}/{partitionedContainerList.Count})**\n```\n" +  outputHeader + outputList + outputFooter;
                    
                    if (i == 0)
                    {
                        await arg.ModifyOriginalResponseAsync(edit => edit.Content =output);
                    }
                    else
                    {
                        await arg.Channel.SendMessageAsync(output);
                    }
                }



                //arg.Channel.SendMessageAsync("");
            }
            else
            {
                string outputList = FormatListObjects(dockerService.DockerStatus, settings, maxLength, arg, allowedContainers);
                string output = "**List of all known Containers**\n```\n" + outputHeader + outputList + outputFooter;
                await arg.ModifyOriginalResponseAsync(edit => edit.Content = output);
            }
        }

        private static string FormatListObjects(List<ContainerListResponse> list, DiscordSettings settings, int maxLength, SocketSlashCommand arg, List<string> allowedContainers)
        {
            string outputList = String.Empty;
            foreach (var item in list)
            {
                if (allowedContainers.Contains(item.Names[0]) || settings.AdminIDs.Contains(arg.User.Id))
                {
                    outputList = outputList + "|" + item.Names[0] + new string(' ', maxLength - item.Names[0].Length);
                    if (item.Status.Contains("Up"))
                    {
                        outputList = outputList + "| Running |\n";
                    }
                    else
                    {
                        outputList = outputList + "| Stopped |\n";
                    }
                }
            }

            return outputList;
        }

        #endregion

    }
}
