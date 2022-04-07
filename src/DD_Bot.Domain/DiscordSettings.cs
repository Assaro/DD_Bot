namespace DD_Bot.Domain
{
    public class DiscordSettings
    {
        public string Token { get; set; } = "<- Please Insert Token here! ->";
        public ulong AdminID { get; set; } = 00000000000000000;
        public string[] AllowedContainers { get; set; } = {"Container1", "Container2,", "Add Names of Public Containers Here"};
    }
}
