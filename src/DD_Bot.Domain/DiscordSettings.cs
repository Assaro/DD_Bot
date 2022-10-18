namespace DD_Bot.Domain
{
    public class DiscordSettings
    {
        public string Token { get; set; } = "<- Please Insert Token here! ->";
        public ulong[] AdminIDs { get; set; } = System.Array.Empty<ulong>();
        public bool UserWhitelist { get; set; } = true;
        public ulong[] UserIDs { get; set; } = System.Array.Empty<ulong>();
        public bool UsersCanStopContainers { get; set; } = false;
        public string[] AllowedContainers { get; set; } = System.Array.Empty<string>();
    }
}
