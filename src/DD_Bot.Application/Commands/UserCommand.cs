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
    public class UserCommand
    {
        private DiscordSocketClient _discord;

        public UserCommand(DiscordSocketClient discord)
        {
            _discord = discord;
        }

        #region CreateCommand

        public static ApplicationCommandProperties Create()
        {
            var builder = new SlashCommandBuilder()
            {
                Name = "user",
                Description = "Grant or Revoke User Permissions"
            };

            builder.AddOption(
                "user",
                ApplicationCommandOptionType.User,
                "choose a user",
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
                var user = arg.Data.Options.FirstOrDefault(option => option.Name == "user")?.Value as SocketGuildUser;
                var permission = arg.Data.Options.FirstOrDefault(option => option.Name == "permission")?.Value as string;
                var container = arg.Data.Options.FirstOrDefault(option => option.Name == "container")?.Value as string;
                
                switch (choice)
                {
                    case "grant":
                        switch (permission)
                        {
                            case "start":
                                if (!settings.DiscordSettings.UserStartPermissions.ContainsKey(user.Id))
                                {
                                    settings.DiscordSettings.UserStartPermissions.Add(user.Id, new List<string>());
                                }
                                if (settings.DiscordSettings.UserStartPermissions[user.Id].Contains(container))
                                {
                                    await arg.ModifyOriginalResponseAsync(
                                        edit => edit.Content =
                                            user.Username + 
                                            " already has permission to start " + 
                                            container);
                                }
                                else
                                {
                                    settings.DiscordSettings.UserStartPermissions[user.Id].Add(container);
                                    await arg.ModifyOriginalResponseAsync(
                                        edit => edit.Content =
                                            "Granted "+ 
                                            user.Username + 
                                            " permission to start " + 
                                            container);
                                }
                                break;
                            case "stop":
                                if (!settings.DiscordSettings.UserStopPermissions.ContainsKey(user.Id))
                                {
                                    settings.DiscordSettings.UserStopPermissions.Add(user.Id, new List<string>());
                                }
                                if (settings.DiscordSettings.UserStopPermissions[user.Id].Contains(container))
                                {
                                    await arg.ModifyOriginalResponseAsync(
                                        edit => edit.Content =
                                            user.Username + " already has permission to stop " + container);
                                }
                                else
                                {
                                    settings.DiscordSettings.UserStopPermissions[user.Id].Add(container);
                                    await arg.ModifyOriginalResponseAsync(
                                        edit => edit.Content ="Granted "+ 
                                                              user.Username + " permission to stop " + container);
                                }
                                break;
                        }
                        break;
                    case "revoke":
                        switch (permission)
                        {
                            case "start":
                                if (!settings.DiscordSettings.UserStartPermissions.ContainsKey(user.Id))
                                {
                                    settings.DiscordSettings.UserStartPermissions.Add(user.Id, new List<string>());
                                }
                                if (settings.DiscordSettings.UserStartPermissions[user.Id].Contains(container))
                                {
                                    settings.DiscordSettings.UserStartPermissions[user.Id].Remove(container);
                                    await arg.ModifyOriginalResponseAsync(
                                        edit => edit.Content ="Revoked "+ 
                                                              user.Username + "'s permission to start " + container);
                                }
                                else
                                {
                                    await arg.ModifyOriginalResponseAsync(
                                        edit => edit.Content =
                                                              user.Username + "has no permission to start " + container);
                                }
                                break;
                            case "stop":
                                if (!settings.DiscordSettings.UserStopPermissions.ContainsKey(user.Id))
                                {
                                    settings.DiscordSettings.UserStopPermissions.Add(user.Id, new List<string>());
                                }
                                if (settings.DiscordSettings.UserStopPermissions[user.Id].Contains(container))
                                {
                                    settings.DiscordSettings.UserStopPermissions[user.Id].Remove(container);
                                    await arg.ModifyOriginalResponseAsync(
                                        edit => edit.Content ="Revoked "+ 
                                                              user.Username + "'s permission to stop " + container);
                                }
                                else
                                {
                                    await arg.ModifyOriginalResponseAsync(
                                        edit => edit.Content =
                                            user.Username + "has no permission to stop " + container);
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