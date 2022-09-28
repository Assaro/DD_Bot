using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using DD_Bot.Application.Providers;
using DD_Bot.Application.Interfaces;
using DD_Bot.Application.Services;

#region CreateSettingsFiles
var settingsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "settings");
//var languageDirectory = Path.Combine(settingsDirectory, "languages");
var settingsFile = Path.Combine(settingsDirectory, "settings.json");
//var languageFile = Path.Combine(languageDirectory, "en.json");
if (!Directory.Exists(settingsDirectory))
{
    Directory.CreateDirectory(settingsDirectory);
}
if (!File.Exists(settingsFile))
{
    SettingsProvider.CreateBasicSettings(settingsFile);
}
/*
if (!Directory.Exists(languageDirectory))
{
    Directory.CreateDirectory(languageDirectory);
}
if (!File.Exists(languageFile))
{
    LanguageProvider.CreateStandardLanguage(languageFile);
}
*/

#endregion

#region ReadSettingsFromFile
var configuration = new ConfigurationBuilder()
    .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(),"settings"))
    .AddJsonFile("settings.json", false ,true)
    .Build();

//string languageJson = string.Format(configuration.Get<Settings>().LanguageSettings.Language + ".json");

/*
   var language = new ConfigurationBuilder()
    .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "settings/languages"))
    .AddJsonFile(languageJson, false, true)
    .Build();
*/

#endregion

var serviceProvider = new ServiceCollection()
    .AddScoped(_ => configuration)
    .AddSingleton<IDiscordService, DiscordService>()
    .AddSingleton<IDockerService, DockerService>()
    .BuildServiceProvider();


var dockerService = serviceProvider.GetRequiredService<IDockerService>() as DockerService;
if (dockerService == null) throw new ArgumentNullException(nameof(dockerService));
var discordBot = serviceProvider.GetRequiredService<IDiscordService>() as DiscordService;
if (discordBot == null) throw new ArgumentNullException(nameof(discordBot));
discordBot.Start();
dockerService.Start();