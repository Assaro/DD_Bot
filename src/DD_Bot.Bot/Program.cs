/* DD_Bot - A Discord Bot to control Docker containers*/

/*  Copyright (C) 2022 Maxim Kovac

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.

*/

using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using DD_Bot.Application.Providers;
using DD_Bot.Application.Interfaces;
using DD_Bot.Application.Services;
using DD_Bot.Domain;
using Newtonsoft.Json;

string version = "0.1.0";

Console.WriteLine("DD_Bot, Version "+ version);

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

File.WriteAllText(settingsFile, JsonConvert.SerializeObject(configuration.Get<Settings>(), Formatting.Indented));

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
    .AddScoped(_=> settingsFile)
    .AddSingleton<IDiscordService, DiscordService>()
    .AddSingleton<IDockerService, DockerService>()
    .AddSingleton<ISettingsService, SettingsService>()
    .BuildServiceProvider();


var dockerService = serviceProvider.GetRequiredService<IDockerService>() as DockerService;
if (dockerService == null) throw new ArgumentNullException(nameof(dockerService));
var settingsService = serviceProvider.GetRequiredService<ISettingsService>() as SettingsService;
if (settingsService == null) throw new ArgumentNullException(nameof(settingsService));
var discordBot = serviceProvider.GetRequiredService<IDiscordService>() as DiscordService;
if (discordBot == null) throw new ArgumentNullException(nameof(discordBot));
discordBot.Start();
dockerService.Start();
settingsService.Start();