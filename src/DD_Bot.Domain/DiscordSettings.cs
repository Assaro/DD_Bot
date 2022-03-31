namespace DD_Bot.Domain
{
    public class DiscordSettings
    {
        public string Token { get; set; } = "<- Please Insert Token here! ->";
        public string AdminToken { get; set; } = null;
        public string[] AllowedContainers { get; set; } = {"Container1", "Container2,", "Add Names of Public Containers Here"};
        public string BotName { get; set; } = "DD_Bot";
    }
}
