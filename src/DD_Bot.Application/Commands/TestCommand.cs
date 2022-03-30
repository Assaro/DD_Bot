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

}
