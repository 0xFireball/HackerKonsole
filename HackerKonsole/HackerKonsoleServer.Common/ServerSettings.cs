namespace HackerKonsoleServer.Common
{
    public class ServerSettings
    {
        public string BindAddress;
        public bool EnableLogging = false;
        public int Port;
        public int WaitTimeout = 10000;
    }
}