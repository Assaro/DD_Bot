namespace DD_Bot.Domain
{
    public class SshSettings 
    {
        public string SshUser { get; set; } = "root";
        public string SshPassword { get; set; } = "sshpassword";
        public int SshPort { get; set; } = 22;
        public string ServerIp { get; set; } = "192.168.1.1";
    }
}
