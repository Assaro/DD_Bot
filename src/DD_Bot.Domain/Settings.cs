namespace DD_Bot.Domain
{
    public class Settings
    {
        public LanguageSettings LanguageSettings { get; set; } = new LanguageSettings();
        public DiscordSettings DiscordSettings { get; set; } = new DiscordSettings();
        public DockerSettings SshSettings { get; set; } = new DockerSettings();
    }
}
