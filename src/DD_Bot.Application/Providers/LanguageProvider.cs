using System.IO;
using DD_Bot.Domain;
using Newtonsoft.Json;

namespace DD_Bot.Application.Providers
{
    public class LanguageProvider
    {
        public static void CreateStandardLanguage(string languageFile)
        {
            var language = new Language();
            File.WriteAllText(languageFile, JsonConvert.SerializeObject(language));
        }
    }
}
