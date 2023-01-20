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
using System.Diagnostics;
using Discord;
using Discord.WebSocket;
using DD_Bot.Application.Services;
using System.Linq;
using DD_Bot.Domain;

namespace DD_Bot.Application.Commands
{
    public class AdminCommand
    {
        private DiscordSocketClient _discord;

        public AdminCommand(DiscordSocketClient discord)
        {
            _discord = discord;
        }
        
        #region CreateCommand

        public static ApplicationCommandProperties Create()
        {
            var builder = new SlashCommandBuilder()
            {
                Name = "admin",
                Description = "Grant or Revoke Admin Privileges"
            };

            builder.AddOption("user",
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
            
            return builder.Build();
        }

        #endregion

        #region ExecuteCommand

        public static async void Execute(SocketSlashCommand arg, Settings settings, SettingsService settingsService)
        {
            await arg.RespondAsync("Contacting Docker Service...");
            DiscordSettings discordSettings = settings.DiscordSettings;
            if (!discordSettings.AdminIDs.Contains(arg.User.Id))
            {
                await arg.ModifyOriginalResponseAsync(edit => edit.Content = "You are not an Admin!");
            }else
            {
                var choice = arg.Data.Options.FirstOrDefault(option => option.Name == "choice")?.Value as string;
                var user = arg.Data.Options.FirstOrDefault(option => option.Name == "user")?.Value as SocketGuildUser;
                switch (choice)
                {
                    case "grant":
                        Console.WriteLine("grant");
                        if (discordSettings.AdminIDs.Contains(user.Id))
                        {
                            await arg.ModifyOriginalResponseAsync(edit => edit.Content = user.Username + " is already an admin!");
                        }
                        else
                        {
                            settings.DiscordSettings.AdminIDs.Add(user.Id);
                            settingsService.WriteSettings(settings);
                            await arg.ModifyOriginalResponseAsync(edit => edit.Content = "Made " + user.Username + " an admin!");
                        }
                        break;
                    case "revoke":
                        if (user.Id == arg.User.Id)
                        {
                            await arg.ModifyOriginalResponseAsync(edit => edit.Content = "You are not allowed to revoke your own admin privileges!");
                        }else if (!discordSettings.AdminIDs.Contains(user.Id))
                        {
                            await arg.ModifyOriginalResponseAsync(edit => edit.Content = "User is not an admin!");
                        }
                        else
                        {
                            settings.DiscordSettings.AdminIDs.Remove(user.Id);
                            settingsService.WriteSettings(settings);
                            await arg.ModifyOriginalResponseAsync(edit => edit.Content = user.Username + "'s admin privileges have been removed!");
                        }
                        break;
                }
            }
        }

        #endregion
    }
}