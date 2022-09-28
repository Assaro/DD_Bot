using Discord;
using Discord.WebSocket;

namespace DD_Bot.Application.Commands
{
    public class TestCommand
    {

        private DiscordSocketClient _discord;

        public TestCommand(DiscordSocketClient discord)
        {
            _discord = discord;
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
