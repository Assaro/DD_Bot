namespace DD_Bot.Domain
{
    public class DiscordSettings
    {
        public string Token { get; set; } = "<- Please Insert Token here! ->";
        public ulong[] AdminIDs { get; set; } = { 1111111111111111111, 222222222222222222 };
        public bool UserWhitelist { get; set; } = true;
        public ulong[] UserIDs { get; set; } = { 11111111111111111111, 222222222222222222 };
        public bool UsersCanStopContainers { get; set; } = false;
        public string[] AllowedContainers { get; set; } = {"Container1", "Container2", "Add Names of Public Containers Here"};
    }
}
