namespace DD_Bot.Domain
{
    public class DockerSettings 
    {
        public string BotName { get; set; } = "DD_Bot";
        public int Retries { get; set; } = 6;
        public int TimeBeforeRetry { get; set; } = 5;
    }
}
