using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using HackerKonsoleServer.Common;

namespace HackerKonsole.ServerCore
{
    /// <summary>
    ///     A RatServer
    /// </summary>
    public abstract class RatServer
    {
        private readonly TcpListener _listenerSocket;
        private readonly ServerSettings _serverSettings;
        private readonly string bindAddress;
        private readonly int bindPort;
        private bool _isActive;
        private readonly int _waitTimeout;

        public RatServer(ServerSettings serverSettings)
        {
            _serverSettings = serverSettings;
            bindAddress = _serverSettings.BindAddress;
            bindPort = _serverSettings.Port;
            _waitTimeout = _serverSettings.WaitTimeout;
            _listenerSocket = new TcpListener(new IPEndPoint(IPAddress.Parse(bindAddress), bindPort));
        }

        public virtual void StartServer()
        {
            _listenerSocket.Start();
            _isActive = true;
            while (_isActive)
            {
                var s = _listenerSocket.AcceptTcpClient();
                var processor = new ConnectionProcessor(s, this, _waitTimeout);
                Task.Factory.StartNew(processor.ProcessConnection);
            }
        }
    }
}