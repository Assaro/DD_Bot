using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DD_Bot.Application.Commands;
using DD_Bot.Application.Interfaces;
using DD_Bot.Application.Providers;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

namespace DD_Bot.Application.Services
{
    public class DiscordService : IDiscordService
    {
        private IConfigurationRoot Configuration;
        private DiscordSocketClient DiscordClient;

        public DiscordService(IConfigurationRoot configuration)
        {
            Configuration = configuration;
            DiscordClient = new DiscordSocketClient();
        }

        public void Start()
        {
            DiscordClient.Log += DiscordClient_Log;
            DiscordClient.MessageReceived += DiscordClient_MessageReceived;
            DiscordClient.GuildAvailable += DiscordClient_GuildAvailable;
            DiscordClient.SlashCommandExecuted += DiscordClient_SlashCommandExecuted;
            DiscordClient.LoginAsync(Discord.TokenType.Bot, SettingsProvider.GetBotSettings().Token);
            DiscordClient.StartAsync();
            while (true)
      
                Thread.Sleep(1000);
            
        }

        private Task DiscordClient_SlashCommandExecuted(SocketSlashCommand arg)
        {
            switch (arg.CommandName)
            {
                case "ping":
                    TestCommand.Execute(arg);
                    return Task.CompletedTask;

                case "StartServer":
                        TestCommand.Execute(arg);
                    return Task.CompletedTask;
            }
            return Task.CompletedTask;
        }

        private async Task DiscordClient_GuildAvailable(SocketGuild arg)
        {
            await arg.CreateApplicationCommandAsync(TestCommand.Create());
        }

        private Task DiscordClient_MessageReceived(SocketMessage arg)
        {
            Console.WriteLine($"{arg.Author.Username}: {arg.Content}");
            return Task.CompletedTask;
        }

        private Task DiscordClient_Log(Discord.LogMessage arg)
        {
            Console.WriteLine($"{arg.Severity}:{arg.Message}");
            return Task.CompletedTask;
        }

    }
}
