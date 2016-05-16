using HackerKonsoleServer.Common;

namespace HackerKonsole.ServerCore
{
    /// <summary>
    ///     The generic HackerKonsole Server core
    /// </summary>
    public class HackerKonsoleServer : RatServer
    {
        public HackerKonsoleServer(ServerSettings serverSettings) : base(serverSettings)
        {
        }

        public override void StartServer()
        {
            base.StartServer();
            Logger.WriteLine("Started HackerKonsole Server");
        }
    }
}