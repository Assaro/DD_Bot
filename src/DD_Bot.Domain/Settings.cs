namespace DD_Bot.Domain
{
    public class Settings
    {
        public LanguageSettings LanguageSettings { get; set; } = new LanguageSettings();
        public DiscordSettings DiscordSettings { get; set; } = new DiscordSettings();
        public SshSettings SshSettings { get; set; } = new SshSettings();
    }
}
