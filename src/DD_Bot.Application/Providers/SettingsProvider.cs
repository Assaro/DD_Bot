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

        public static IConfigurationRoot Configuration { get; private set; }
        public static void SetConfiguration(IConfigurationRoot configuration)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public static void CreateBasicSettings(string settingsFile)
        {
            var settings = new Settings();
            File.WriteAllText(settingsFile, JsonConvert.SerializeObject(settings));
        }

        public static Settings GetBotSettings()
        {
            return Configuration.Get<Settings>();
        }


    }
}
