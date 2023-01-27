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
using System.Linq;
using DD_Bot.Domain;

namespace DD_Bot.Application.Commands
{
    public class PermissionCommand
    {
        private DiscordSocketClient _discord;

        public PermissionCommand(DiscordSocketClient discord)
        {
            _discord = discord;
        }

        #region CreateCommand

        public static ApplicationCommandProperties Create()
        {
            var builder = new SlashCommandBuilder()
            {
                Name = "permission",
                Description = "See a the permissions of a user/role"
            };

            builder.AddOption(
                "user",
                ApplicationCommandOptionType.User,
                "choose a user",
                isRequired: false
            );

            builder.AddOption(
                "role",
                ApplicationCommandOptionType.Role,
                "choose a role",
                isRequired: false
            );

            return builder.Build();
        }
        #endregion

        #region ExecuteCommand

        public static async void Execute(SocketSlashCommand arg, Settings settings)
        {
            await arg.RespondAsync("Contacting Settings Service");
            var user = arg.Data.Options.FirstOrDefault(option => option.Name == "user")?.Value as SocketGuildUser;
            var role = arg.Data.Options.FirstOrDefault(option => option.Name == "role")?.Value as SocketRole;
            
            if (user != null && role !=null)
            {
                await arg.ModifyOriginalResponseAsync(edit =>
                    edit.Content = "Please only specify a user or a role, not both");
                return;
            }

            List<string> startPermissions = new List<string>();
            List<string> stopPermissions = new List<string>();

            string listOwner;
            
            if (user != null)
            {
                if (!UserIsAdmin(arg, settings))
                {
                    await arg.ModifyOriginalResponseAsync(edit =>
                        edit.Content = "Only Admins are allowed to check other users permissions!");
                    return;
                }

                listOwner = user.Username;
                if (settings.DiscordSettings.UserStartPermissions.ContainsKey(user.Id))
                {
                    startPermissions.AddRange(settings.DiscordSettings.UserStartPermissions[user.Id]);
                }

                if (settings.DiscordSettings.UserStopPermissions.ContainsKey(user.Id))
                {
                    stopPermissions.AddRange(settings.DiscordSettings.UserStopPermissions[user.Id]);
                }
            }
            else if (role != null)
            {
                if (!UserIsAdmin(arg, settings))
                {
                    await arg.ModifyOriginalResponseAsync(edit =>
                        edit.Content = "Only Admins are allowed to check role permissions!");
                    return;
                }
                
                listOwner = role.Name;
                if (settings.DiscordSettings.RoleStartPermissions.ContainsKey(role.Id))
                {
                    startPermissions.AddRange(settings.DiscordSettings.RoleStartPermissions[role.Id]);
                }
                if (settings.DiscordSettings.RoleStopPermissions.ContainsKey(role.Id))
                {
                    stopPermissions.AddRange(settings.DiscordSettings.RoleStopPermissions[role.Id]);
                }
            }
            else
            {
                listOwner = arg.User.Username;
                var socketUser = arg.User as SocketGuildUser;
                if (socketUser == null)
                {
                    await arg.ModifyOriginalResponseAsync(edit => edit.Content = "Failed to retrieve UserData");
                    return;
                }
                var guild = socketUser.Guild;
                var socketGuildUser = guild.GetUser(socketUser.Id);
                var userRoles = socketGuildUser.Roles;
                if (settings.DiscordSettings.UserStartPermissions.ContainsKey(arg.User.Id))
                {
                    startPermissions.AddRange(settings.DiscordSettings.UserStartPermissions[arg.User.Id]);
                }
                if (settings.DiscordSettings.UserStopPermissions.ContainsKey(arg.User.Id))
                {
                    stopPermissions.AddRange(settings.DiscordSettings.UserStopPermissions[arg.User.Id]);
                }

                foreach (var roleToTest in userRoles)
                {
                    if (settings.DiscordSettings.RoleStartPermissions.ContainsKey(roleToTest.Id))
                    {
                        startPermissions.AddRange(settings.DiscordSettings.RoleStartPermissions[roleToTest.Id]);
                    }

                    if (settings.DiscordSettings.RoleStopPermissions.ContainsKey(roleToTest.Id))
                    {
                        stopPermissions.AddRange(settings.DiscordSettings.RoleStopPermissions[roleToTest.Id]);
                    }
                }
            }

            if (startPermissions.Count == 0 && stopPermissions.Count == 0)
            {
                await arg.ModifyOriginalResponseAsync(edit => edit.Content = "No Permissions have been found");
                return;
            }
            
            startPermissions = startPermissions.Distinct().ToList();
            startPermissions.Sort();
            stopPermissions = stopPermissions.Distinct().ToList();
            stopPermissions.Sort();
            
            List<ContainerPermission> permissions = new List<ContainerPermission>();

            foreach (var startPermission in startPermissions)
            {
                permissions.Add(new ContainerPermission(startPermission));
                permissions.Find(x => x.ContainerName == startPermission).StartPermission = true;
            }

            foreach (var stopPermission in stopPermissions)
            {
                if ((from x in permissions select x.ContainerName).Contains(stopPermission))
                {

                    permissions.Find(x => x.ContainerName == stopPermission).StopPermission = true;
                }
                else
                {
                    permissions.Add(new ContainerPermission(stopPermission));
                    permissions.Find(x => x.ContainerName == stopPermission).StopPermission = true;
                }
            }

            if (permissions.Count == 0)
            {
                await arg.ModifyOriginalResponseAsync(edit =>
                    edit.Content = "No Permissions have been found for " + listOwner);
                return;
            }
            
            int maxLength = 0;

            foreach (var permission in permissions)
            {
                if (maxLength < permission.ContainerName.Length)
                {
                    maxLength = permission.ContainerName.Length;
                }
            }

            if (maxLength < 14)
            {
                maxLength = 14;
            }

            string outputHeader = "**List of Permissions for " + listOwner + "**\n```\n";
            string outputTableHeader = new string('-', maxLength + 19) 
                                       + '\n' 
                                       + "| ContainerName"
                                       + new string(' ', maxLength - 13)
                                       + "| Start | Stop  |\n"
                                       + new string('-', maxLength + 19) 
                                       + '\n';
            string outputTableBody = FormatListObjects(permissions, maxLength);
            string outputTableFooter = new string('-', maxLength + 19)+
                                       "```";

            string output = outputHeader + outputTableHeader + outputTableBody + outputTableFooter;

            await arg.ModifyOriginalResponseAsync(edit => edit.Content = output);
        }

        private static bool UserIsAdmin(SocketSlashCommand arg, Settings settings)
        {
            if (settings.DiscordSettings.AdminIDs.Contains(arg.User.Id))
            {
                return true;
            }
            return false;
        }
        
        #endregion

        private static string FormatListObjects(List<ContainerPermission> list, int maxLength)
        {
            string outputList= String.Empty;
            string line = String.Empty;
            foreach (var permission in list)
            {
                line = "| " + permission.ContainerName;
                line += new string(' ', maxLength-permission.ContainerName.Length);
                line += "|";
                if (permission.StartPermission)
                {
                    line += "   x   ";
                }
                else
                {
                    line += "       ";
                }
                line += '|';
                if (permission.StopPermission)
                {
                    line += "   x   ";
                }
                else
                {
                    line += "       ";
                }

                line += "|\n";
                outputList += line;
            }

            return outputList;
        }
        
    }

    class ContainerPermission
    {
        public string ContainerName { get; }
        public bool StartPermission { get; set; }
        public bool StopPermission { get; set; }

        public ContainerPermission(string containerName)
        {
            ContainerName = containerName;
            StartPermission = false;
            StopPermission = false;
        }
    }
}