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




        }

    }
}
