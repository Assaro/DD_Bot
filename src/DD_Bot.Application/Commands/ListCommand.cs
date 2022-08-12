using Discord;
using Discord.WebSocket;
using DD_Bot.Application.Services;
using DD_Bot.Domain;
using System.Linq;

namespace DD_Bot.Application.Commands
{
    internal class ListCommand
    {
        private DiscordSocketClient Discord;
        public ListCommand(DiscordSocketClient discord)
        {
            Discord = discord;
        }

        public static ApplicationCommandProperties Create()
        {
            var builder = new SlashCommandBuilder()
            {
                Name = "list",
                Description = " List all Docker containers"
            };
            return builder.Build();
        }

        public static async void Execute(SocketSlashCommand arg, DockerService dockerService, DiscordSettings settings)
        {
            await arg.RespondAsync("Contacting Docker Service...");
            await dockerService.DockerUpdate();

            if (settings.UserWhitelist && settings.UserIDs.Contains(arg.User.Id))
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
                if (settings.AllowedContainers.Contains(item.Name) || settings.AdminIDs.Contains(arg.User.Id))
                {
                    output = output + "|" + item.Name + new string(' ', maxlength - item.Name.Length);
                    if (item.Running)
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
            return;
        }

    }
}
