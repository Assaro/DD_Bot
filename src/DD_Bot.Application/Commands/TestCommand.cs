using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DD_Bot.Application.Commands
{
    public class TestCommand
    {

        private DiscordSocketClient Discord;

        public TestCommand(DiscordSocketClient discord)
        {
            Discord = discord;
        }

        public static async void Execute(SocketSlashCommand arg)
        {
            //arg.Channel.SendMessageAsync("Pong!");
            //Do Stuff
            await arg.RespondAsync($"Pong! " + arg.User.Mention);
        }

        public static ApplicationCommandProperties Create()
        {
            var builder = new SlashCommandBuilder()
            {
                Name = "ping",
                Description = "Ping"
            };
            return builder.Build();
        }

    }

    public class StartServer
    {
        private DiscordSocketClient Discord;

        public StartServer(DiscordSocketClient discord)
        {
            Discord=discord;
        }

        public static async void Execute(SocketSlashCommand arg)
        {
            await arg.RespondAsync($"" + arg.User.Mention );
        }
        public static ApplicationCommandProperties Create()
        {
            var builder = new SlashCommandBuilder()
            {
                Name = "StartServer",
                Description = "Start a Server with a name",
                Options = new List<SlashCommandOptionBuilder>()
            };
            return builder.Build();
        }


    }

}
