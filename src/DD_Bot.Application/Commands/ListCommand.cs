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

            if (!settings.AdminIDs.Contains(arg.User.Id))
            {
                if (settings.UserWhitelist && !settings.UserIDs.Contains(arg.User.Id))
                {
                    await arg.ModifyOriginalResponseAsync(edit =>
                        edit.Content = "You are not allowed to use this command");
                    return;
                }
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
                    outputList = String.Empty;
                    
                    foreach (var item in partitionedContainerList[i])
                    {
                        if (settings.AllowedContainers.Contains(item.Names[0]) || settings.AdminIDs.Contains(arg.User.Id))
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
                string outputList = String.Empty;
                foreach (var item in dockerService.DockerStatus)
                {
                    if (settings.AllowedContainers.Contains(item.Names[0]) || settings.AdminIDs.Contains(arg.User.Id))
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
                string output = "**List of all known Containers**\n```\n" + outputHeader + outputList + outputFooter;
                await arg.ModifyOriginalResponseAsync(edit => edit.Content = output);
            }
        }

        #endregion

    }
}
