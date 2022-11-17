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

using Discord;
using Discord.WebSocket;
using DD_Bot.Application.Services;
using DD_Bot.Domain;
using System.Linq;

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

            if (settings.UserWhitelist && !settings.UserIDs.Contains(arg.User.Id) && !settings.AdminIDs.Contains(arg.User.Id))
            {
                await arg.ModifyOriginalResponseAsync(edit => edit.Content = "You are not allowed to use this command");
                return;
            }

            int maxlength = dockerService.DockerStatusLongestName();
            maxlength++;
            if (maxlength< 14)
            {
                maxlength = 14;
            }
            string output = "**List of all known Containers**\n```\n"
                + new string('¯', 12 + maxlength)
                + "\n|Containername"
                + new string(' ', maxlength - 13)
                + "| Status  |\n"
                + new string('¯', 12 + maxlength)
                + "\n";
            foreach (var item in dockerService.DockerStatus)
            {
                if (settings.AllowedContainers.Contains(item.Names[0]) || settings.AdminIDs.Contains(arg.User.Id))
                {
                    output = output + "|" + item.Names[0] + new string(' ', maxlength - item.Names[0].Length);
                    if (item.Status.Contains("Up"))
                    {
                        output = output + "| Running |\n";
                    }
                    else
                    {
                        output = output + "| Stopped |\n";
                    }
                }
            }
            output = output + new string('¯', 12 + maxlength) + "\n" + "```";
            await arg.ModifyOriginalResponseAsync(edit => edit.Content = output);
        }

        #endregion

    }
}
