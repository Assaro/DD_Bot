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


using System.Collections.Generic;
using Discord;
using Discord.WebSocket;
using DD_Bot.Application.Services;
using System.Linq;
using DD_Bot.Domain;

namespace DD_Bot.Application.Commands
{
    public class RoleCommand
    {
        private DiscordSocketClient _discord;

        public RoleCommand(DiscordSocketClient discord)
        {
            _discord = discord;
        }

        #region CreateCommand

        public static ApplicationCommandProperties Create()
        {
            var builder = new SlashCommandBuilder()
            {
                Name = "role",
                Description = "Grant or Revoke Role Permissions"
            };

            builder.AddOption(
                "role",
                ApplicationCommandOptionType.Role,
                "choose a role",
                isRequired: true
            );

            builder.AddOption("choice",
                ApplicationCommandOptionType.String,
                "choose grant or revoke",
                true,
                choices: new[]
                {
                    new ApplicationCommandOptionChoiceProperties()
                    {
                        Name = "grant",
                        Value = "grant",
                    },
                    new ApplicationCommandOptionChoiceProperties()
                    {
                        Name = "revoke",
                        Value = "revoke",
                    }
                });
            
            builder.AddOption(
                "container",
                ApplicationCommandOptionType.String,
                "state a docker container",
                true
            );
            builder.AddOption(
                "permission",
                ApplicationCommandOptionType.String,
                "choose the permissions to grant/revoke",
                true,
                choices:new[]
                {
                    new ApplicationCommandOptionChoiceProperties()
                    {
                        Name = "start",
                        Value = "start",
                    },
                    new ApplicationCommandOptionChoiceProperties()
                    {
                        Name = "stop",
                        Value = "revoke",
                    }
                });

            return builder.Build();
        }

        #endregion

        #region ExecuteCommand

        public static async void Execute(SocketSlashCommand arg, Settings settings, SettingsService settingsService)
        {
            await arg.RespondAsync("Contacting Settings Service");
            DiscordSettings discordSettings = settings.DiscordSettings;
            if (!discordSettings.AdminIDs.Contains(arg.User.Id))
            {
                await arg.ModifyOriginalResponseAsync(edit => edit.Content = "You are not an Admin!");
            }
            else
            {
                var choice = arg.Data.Options.FirstOrDefault(option => option.Name == "choice")?.Value as string;
                var role = arg.Data.Options.FirstOrDefault(option => option.Name == "role")?.Value as SocketRole;
                var permission = arg.Data.Options.FirstOrDefault(option => option.Name == "permission")?.Value as string;
                var container = arg.Data.Options.FirstOrDefault(option => option.Name == "container")?.Value as string;

                switch (permission)
                {
                    case "start":
                        if (!settings.DiscordSettings.RoleStartPermissions.ContainsKey(role.Id))
                        {
                            settings.DiscordSettings.RoleStartPermissions.Add(role.Id, new List<string>());
                        }
                        switch (choice)
                        {
                            case "grant":
                                if (settings.DiscordSettings.RoleStartPermissions[role.Id].Contains(container))
                                {
                                    await arg.ModifyOriginalResponseAsync(
                                        edit => edit.Content =
                                            role.Name + 
                                            " already has permission to start " + 
                                            container);
                                }
                                else
                                {
                                    settings.DiscordSettings.RoleStartPermissions[role.Id].Add(container);
                                    await arg.ModifyOriginalResponseAsync(
                                        edit => edit.Content =
                                            "Granted "+ 
                                            role.Name + 
                                            " permission to start " + 
                                            container);
                                }
                                break;
                            case "revoke":
                                if (settings.DiscordSettings.RoleStartPermissions[role.Id].Contains(container))
                                {
                                    settings.DiscordSettings.RoleStartPermissions[role.Id].Remove(container);
                                    await arg.ModifyOriginalResponseAsync(
                                        edit => edit.Content =
                                            "Revoked "+ 
                                            role.Name + 
                                            "'s permission to start " + 
                                            container);
                                }
                                else
                                {
                                    await arg.ModifyOriginalResponseAsync(
                                        edit => edit.Content =
                                            role.Name + 
                                            "has no permission to start " + 
                                            container);
                                }
                                break;
                        }
                        break;
                    case "stop":
                        if (!settings.DiscordSettings.RoleStopPermissions.ContainsKey(role.Id))
                        {
                            settings.DiscordSettings.RoleStopPermissions.Add(role.Id, new List<string>());
                        }

                        switch (permission)
                        {
                            case "grant":
                                if (settings.DiscordSettings.RoleStopPermissions[role.Id].Contains(container))
                                {
                                    await arg.ModifyOriginalResponseAsync(
                                        edit => edit.Content =
                                            role.Name + " already has permission to stop " + container);
                                }
                                else
                                {
                                    settings.DiscordSettings.RoleStopPermissions[role.Id].Add(container);
                                    await arg.ModifyOriginalResponseAsync(
                                        edit => edit.Content ="Granted "+ 
                                                              role.Name + " permission to stop " + container);
                                }
                                break;
                            case "revoke":
                                if (settings.DiscordSettings.RoleStopPermissions[role.Id].Contains(container))
                                {
                                    settings.DiscordSettings.RoleStopPermissions[role.Id].Remove(container);
                                    await arg.ModifyOriginalResponseAsync(
                                        edit => edit.Content ="Revoked "+ 
                                                              role.Name + "'s permission to stop " + container);
                                }
                                else
                                {
                                    await arg.ModifyOriginalResponseAsync(
                                        edit => edit.Content =
                                            role.Name + "has no permission to stop " + container);
                                }
                                break;
                        }
                        break;
                }
                settingsService.WriteSettings(settings);
            }
        }

        #endregion
        
    }
}