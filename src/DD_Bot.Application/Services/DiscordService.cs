using System;
using System.Threading;
using System.Threading.Tasks;
using DD_Bot.Application.Commands;
using DD_Bot.Application.Interfaces;
using DD_Bot.Domain;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DD_Bot.Application.Services
{
    public class DiscordService : IDiscordService
    {
        private IConfigurationRoot Configuration;
        private IServiceProvider ServiceProvider;
        private DiscordSocketClient DiscordClient;

        public DiscordService(IConfigurationRoot configuration, IServiceProvider serviceProvider)
        {
            Configuration = configuration;
            ServiceProvider = serviceProvider;
            DiscordClient = new DiscordSocketClient();
        }

        public DiscordSettings Setting => Configuration.Get<Settings>().DiscordSettings;

        public DockerService Docker => ServiceProvider.GetRequiredService<IDockerService>() as DockerService;

        public void Start()
        {
            DiscordClient.Log += DiscordClient_Log;
            DiscordClient.MessageReceived += DiscordClient_MessageReceived;
            DiscordClient.GuildAvailable += DiscordClient_GuildAvailable;
            DiscordClient.SlashCommandExecuted += DiscordClient_SlashCommandExecuted;
            DiscordClient.LoginAsync(Discord.TokenType.Bot, Setting.Token);
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

                case "docker":
                        DockerCommand.Execute(arg, Docker);
                    return Task.CompletedTask;

                case "list":
                    ListCommand.Execute(arg, Docker);
                    return Task.CompletedTask;
            }
            return Task.CompletedTask;
        }

        private async Task DiscordClient_GuildAvailable(SocketGuild arg)
        {
            await arg.CreateApplicationCommandAsync(TestCommand.Create());
            await arg.CreateApplicationCommandAsync(DockerCommand.Create());
            await arg.CreateApplicationCommandAsync(ListCommand.Create());
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
