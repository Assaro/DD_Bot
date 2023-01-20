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
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading;
using DD_Bot.Application.Interfaces;
using DD_Bot.Domain;
using Newtonsoft.Json;

namespace DD_Bot.Application.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly IConfigurationRoot _configuration;
        private readonly string _settingsPath;

        public SettingsService(IConfigurationRoot configuration,string settingsFile) //Discord Initialising
        {
            _settingsPath = settingsFile;
            _configuration = configuration;
        }

        private Settings Setting => _configuration.Get<Settings>();
        
        public void WriteSettings(Settings settings)
        {
            File.WriteAllText(_settingsPath, JsonConvert.SerializeObject(settings, Formatting.Indented));
        }
        
        public void Start()
        {
            Console.WriteLine("SettingsService started");
        }
    }
}