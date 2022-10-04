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
