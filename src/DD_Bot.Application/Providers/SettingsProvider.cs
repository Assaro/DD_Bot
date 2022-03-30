using System.IO;
using DD_Bot.Domain;
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
