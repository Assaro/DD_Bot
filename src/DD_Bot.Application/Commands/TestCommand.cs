using Discord;
using Discord.WebSocket;

namespace DD_Bot.Application.Commands
{
    public class TestCommand
    {

        private DiscordSocketClient Discord;

        public TestCommand(DiscordSocketClient discord)//Verbindung zur DiscordAPI
        {
            Discord = discord;
        }

        public static async void Execute(SocketSlashCommand arg) //Logik hinter dem Befehl
        {
            //arg.Channel.SendMessageAsync("Pong!");
            //Do Stuff
            await arg.RespondAsync($"Pong! " + arg.User.Mention);
        }

        public static ApplicationCommandProperties Create() //Werte zum Erstellen des Commands für Discord
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
