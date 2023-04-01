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
using Discord;
using Discord.WebSocket;
using DD_Bot.Application.Services;
using System.Linq;
using System.Threading;
using DD_Bot.Domain;

namespace DD_Bot.Application.Commands
{
    public class DockerCommand
    {
        private DiscordSocketClient _discord;

        public DockerCommand(DiscordSocketClient discord)
        {
            _discord=discord;
        }

        #region CreateCommand

        public static ApplicationCommandProperties Create() //Create-Methode mit 3 Auswahlmöglichkeiten für den Reiter Command
        {
            var builder = new SlashCommandBuilder()
            {
                Name = "docker",
                Description = "Issue a command to Docker"
            };

            builder.AddOption("dockername",
                ApplicationCommandOptionType.String,
                    "choose a container",
                    true);

            builder.AddOption("command",
                ApplicationCommandOptionType.String, 
                "choose a command", 
                true, 
                choices: new[]
                {
                    new ApplicationCommandOptionChoiceProperties()
                    {
                        Name = "start",
                        Value ="start",
                    },
                    new ApplicationCommandOptionChoiceProperties()
                    {
                        Name = "stop",
                        Value ="stop",
                    },
                    new ApplicationCommandOptionChoiceProperties()
                    {
                        Name = "restart",
                        Value ="restart",
                    },
                });

            return builder.Build();
        }

        #endregion

        #region ExecuteCommand

        public static async void Execute(SocketSlashCommand arg, DockerService dockerService, DiscordSettings settings)
        {
            await arg.RespondAsync("Contacting Docker Service...");
            await dockerService.DockerUpdate();
            
            var command = arg.Data.Options.FirstOrDefault(option => option.Name == "command")?.Value as string;
            var dockerName = arg.Data.Options.FirstOrDefault(option => option.Name == "dockername")?.Value as string;

            #region authCheck

            bool authorized = true;
            
            if (!settings.AdminIDs.Contains(arg.User.Id)) //Auth Checks
            {
                authorized = false;
                var socketUser = arg.User as SocketGuildUser;
                var guild = socketUser.Guild;
                var socketGuildUser = guild.GetUser(socketUser.Id);
                var userRoles = socketGuildUser.Roles;

                switch (command)
                {
                    case "start":
                        if (settings.UserStartPermissions.ContainsKey(arg.User.Id))
                        {
                            if (settings.UserStartPermissions[arg.User.Id].Contains(dockerName))
                            {
                                authorized = true;
                            }
                        }
                        foreach (var role in userRoles)
                        {
                            if (settings.RoleStartPermissions.ContainsKey(role.Id))
                            {
                                if (settings.RoleStartPermissions[role.Id].Contains(dockerName))
                                {
                                    authorized = true;
                                }
                            }
                        }
                        break;
                    case "stop":
                    case "restart":
                        if (settings.UserStopPermissions.ContainsKey(arg.User.Id))
                        {
                            if (settings.UserStopPermissions[arg.User.Id].Contains(dockerName))
                            {
                                authorized = true;
                            }
                        }
                        foreach (var role in userRoles)
                        {
                            if (settings.RoleStopPermissions.ContainsKey(role.Id))
                            {
                                if (settings.RoleStopPermissions[role.Id].Contains(dockerName))
                                {
                                    authorized = true;
                                }
                            }
                        }
                        break;
                }

                if (!authorized)
                {
                    await arg.ModifyOriginalResponseAsync(edit =>
                        edit.Content = "You are not allowed to use this command");
                    return;
                }
            }

            #endregion

            if (string.IsNullOrEmpty(dockerName)) //Schaut ob ein Name für den Docker eingegeben wurde
            {
                await arg.ModifyOriginalResponseAsync(edit => edit.Content = "No name has been specified");
                return;
            }


            var docker = dockerService.DockerStatus.FirstOrDefault(docker => docker.Names[0] == dockerName);

            if (docker == null) //Schaut ob gesuchter Docker Existiert
            {
                await arg.ModifyOriginalResponseAsync(edit => edit.Content = "Docker doesnt exist");
                return;
            }

            var dockerId = docker.ID;

            switch (command)
            {
                case "start":
                    if (dockerService.RunningDockers.Contains(dockerName))
                    {
                        await arg.ModifyOriginalResponseAsync(edit => edit.Content = string.Format(dockerName + " is already running"));
                        return;
                    }
                    break;
                case "stop":
                case "restart":
                    if (dockerService.StoppedDockers.Contains(dockerName))
                    {
                        await arg.ModifyOriginalResponseAsync(edit => edit.Content = string.Format(dockerName + " is already stopped"));
                        return;
                    }
                    break;
            }

            switch (command)
            {
               case "start":
                   dockerService.DockerCommandStart(dockerId);
                    break;
               case "stop":
                   dockerService.DockerCommandStop(dockerId);
                    break;
               case "restart":
                   dockerService.DockerCommandRestart(dockerId);
                    break;
            }

            await arg.ModifyOriginalResponseAsync(edit =>
                edit.Content = "Command has been sent. Awaiting response. This will take up to " + dockerService.Settings.Retries * dockerService.Settings.TimeBeforeRetry + " Seconds.");

            for (int i = 0; i < dockerService.Settings.Retries; i++)
            {
                Thread.Sleep(TimeSpan.FromSeconds(dockerService.Settings.TimeBeforeRetry));
                await dockerService.DockerUpdate();
                
                switch (command)
                {
                    case "start":
                        if (dockerService.RunningDockers.Contains(dockerName))
                        {
                            await arg.ModifyOriginalResponseAsync(edit => edit.Content = arg.User.Mention+ " " + dockerName + " has been started");
                            return;
                        }
                        else
                        {
                            break;
                        }
                    case "stop":
                        if (dockerService.StoppedDockers.Contains(dockerName))
                        {
                            await arg.ModifyOriginalResponseAsync(edit => edit.Content = arg.User.Mention + " " + dockerName + " has been stopped");
                            return;
                        }
                        else
                        {
                            break;
                        }
                    case "restart":
                        if (dockerService.RunningDockers.Contains(dockerName))
                        {
                            await arg.ModifyOriginalResponseAsync(edit => edit.Content = arg.User.Mention + " " + dockerName +  " has been restarted");
                            return;
                        }
                        else
                        {
                            break;
                        }
                }
            }
            
            
            await dockerService.DockerUpdate();

            switch (command)
            {
                case "start":
                    if (dockerService.RunningDockers.Contains(dockerName))
                    {
                        await arg.ModifyOriginalResponseAsync(edit => edit.Content = arg.User.Mention+ " " + dockerName + " has been started");
                        return;
                    }
                    else
                    {
                        await arg.ModifyOriginalResponseAsync(edit => edit.Content = arg.User.Mention + " " + dockerName + " could not be started");
                        return;
                    }
                case "stop":
                    if (dockerService.StoppedDockers.Contains(dockerName))
                    {
                        await arg.ModifyOriginalResponseAsync(edit => edit.Content = arg.User.Mention + " " + dockerName + " has been stopped");
                        return;
                    }
                    else
                    {

                        await arg.ModifyOriginalResponseAsync(edit => edit.Content = arg.User.Mention + " " + dockerName +  " could not be stopped");

                        return;
                    }
                case "restart":
                    if (dockerService.RunningDockers.Contains(dockerName))
                    {
                        await arg.ModifyOriginalResponseAsync(edit => edit.Content = arg.User.Mention + " " + dockerName +  " has been restarted");
                        return;
                    }
                    else
                    {
                        await arg.ModifyOriginalResponseAsync(edit => edit.Content = arg.User.Mention + " " + dockerName +  " could not be restarted");
                        return;
                    }
            }
        }

        #endregion
    }
}