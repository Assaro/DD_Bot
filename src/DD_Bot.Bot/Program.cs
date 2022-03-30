using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using DD_Bot.Application.Providers;
using DD_Bot.Application.Interfaces;
using DD_Bot.Application.Services;

var settingsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Settings");
var settingsFile = Path.Combine(settingsDirectory, "settings.json");
if (!Directory.Exists(settingsDirectory))
{
    Directory.CreateDirectory(settingsDirectory);
}
if (!File.Exists(settingsFile))
{
    SettingsProvider.CreateBasicSettings(settingsFile);
}



var configuration = new ConfigurationBuilder()
    .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(),"Settings"))
    .AddJsonFile("Settings.json", false ,true)
    .Build();

var serviceProvider = new ServiceCollection()
    .AddScoped(_ => configuration)
    .AddSingleton<IDiscordService, DiscordService>()
    .AddSingleton<IDockerService, DockerService>()
    .BuildServiceProvider();

var _dockerService = serviceProvider.GetRequiredService<IDockerService>() as DockerService;
var _discordBot = serviceProvider.GetRequiredService<IDiscordService>() as DiscordService;
_discordBot.Start();
_dockerService.Start();