using Discord;
using Discord.WebSocket;
using System.Linq;
using DD_Bot.Application.Services;

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

        public static async void Execute(SocketSlashCommand arg, DockerService dockerService)
        {
            await arg.RespondAsync("Contacting Docker Service...");
            await dockerService.DockerUpdate();
            int maxlength = dockerService.DockerStatusLongestName();
            maxlength++;
            if (maxlength< 14)
            {
                maxlength = 14;
            }
            string output = "**Liste aller Docker-Container**\n```\n"
                + new string('¯', 12 + maxlength)
                + "\n|Containername"
                + new string(' ', maxlength - 13)
                + "| Status  |\n"
                + new string('¯', 12 + maxlength)
                + "\n";
            foreach (var item in dockerService.DockerStatus)
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
            output = output + new string('¯', 12 + maxlength) + "\n" + "```";
            await arg.ModifyOriginalResponseAsync(edit => edit.Content = output);
            return;
        }

    }
}
