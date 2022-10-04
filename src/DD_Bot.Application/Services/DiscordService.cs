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
        private readonly IConfigurationRoot _configuration;
        private readonly IServiceProvider _serviceProvider;
        private readonly DiscordSocketClient _discordClient;

        public DiscordService(IConfigurationRoot configuration, IServiceProvider serviceProvider)//Discord Initialising
        {
            _configuration = configuration;
            _serviceProvider = serviceProvider;
            _discordClient = new DiscordSocketClient();
        }

        private DiscordSettings Setting => _configuration.Get<Settings>().DiscordSettings;

        private DockerService Docker => _serviceProvider.GetRequiredService<IDockerService>() as DockerService;

        public void Start() //Discord Start
        {
            _discordClient.Log += DiscordClient_Log;
            _discordClient.MessageReceived += DiscordClient_MessageReceived;
            _discordClient.GuildAvailable += DiscordClient_GuildAvailable;
            _discordClient.SlashCommandExecuted += DiscordClient_SlashCommandExecuted;
            _discordClient.LoginAsync(Discord.TokenType.Bot, Setting.Token);
            _discordClient.StartAsync();

            while (true)
                Thread.Sleep(1000);
            // ReSharper disable once FunctionNeverReturns
        }

        private Task DiscordClient_SlashCommandExecuted(SocketSlashCommand arg)
        {
            switch (arg.CommandName)
            {
                case "ping":
                    TestCommand.Execute(arg);
                    return Task.CompletedTask;

                case "docker":
                        DockerCommand.Execute(arg, Docker, Setting);
                    return Task.CompletedTask;

                case "list":
                    ListCommand.Execute(arg, Docker, Setting);
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
