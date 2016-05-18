namespace HackerKonsoleServer.Common
{
    public class ServerSettings
    {
        public string BindAddress;
        public bool EnableLogging = false;
        public int Port;
        public int WaitTimeout = 60000; //1 minute
        public string RoutingProxyAddress;
        public int RoutingProxyPort;
    }
}