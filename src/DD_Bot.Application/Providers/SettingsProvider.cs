using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DD_Bot.Domain;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace DD_Bot.Application.Providers
{
    public class SettingsProvider
    {
        public static void CreateBasicSettings(string settingsFile)
        {
            var settings = new Settings();
            File.WriteAllText(settingsFile, JsonConvert.SerializeObject(settings));
        }
    }
}
