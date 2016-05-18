using HackerKonsoleServer.Common;

namespace HackerKonsole.ServerCore
{
    /// <summary>
    ///     The class that controls the actual server instance
    /// </summary>
    public static class HackerKonsoleServerMaster
    {
        public static string Version = "0.0.2";

        public static void StartServer(string[] args)
        {
            ServerSettings serverSettings;
            if (args.Length >= 4)
            {
                serverSettings = new ServerSettings
                {
                    BindAddress = args[0],
                    Port = int.Parse(args[1]),
                    WaitTimeout = int.Parse(args[2]),
                    EnableLogging = args[3] == "-enablelogging"
                };
            }
            else
            {
                serverSettings = new ServerSettings
                {
                    BindAddress = "0.0.0.0",
                    Port = 25000,
                    WaitTimeout = 600000, //10 minutes
                    EnableLogging = true,
                    //Public Routing Proxy:
                    RoutingProxyAddress = "127.0.0.1",
                    RoutingProxyPort = 25555,
                };
            }
            var hackerKonsoleServer = new HackerKonsoleServer(serverSettings);
            Logger.EnableLogging = serverSettings.EnableLogging;
            Logger.WriteLine("HackerKonsole Server Started.");
            hackerKonsoleServer.StartServer(); //Run the server
        }
    }
}