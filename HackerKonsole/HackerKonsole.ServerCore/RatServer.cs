using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using HackerKonsole.Tools.InternetRouting.Core;
using HackerKonsoleServer.Common;

namespace HackerKonsole.ServerCore
{
    /// <summary>
    ///     A RatServer
    /// </summary>
    public abstract class RatServer
    {
        #region Protected Fields

        protected InternetRoutingProxyConnector RoutingProxyConnector;

        #endregion Protected Fields

        #region Private Fields

        private readonly string _bindAddress;
        private readonly int _bindPort;
        private readonly TcpListener _listenerSocket;
        private readonly ServerSettings _serverSettings;
        private readonly int _waitTimeout;
        private bool _isActive;

        #endregion Private Fields

        #region Protected Constructors

        protected RatServer(ServerSettings serverSettings)
        {
            _serverSettings = serverSettings;
            _bindAddress = _serverSettings.BindAddress;
            _bindPort = _serverSettings.Port;
            _waitTimeout = _serverSettings.WaitTimeout;
            _listenerSocket = new TcpListener(new IPEndPoint(IPAddress.Parse(_bindAddress), _bindPort));
        }

        #endregion Protected Constructors

        #region Public Properties

        public bool RoutingProxyAvailable { get; set; }

        #endregion Public Properties

        #region Public Methods

        public virtual void StartServer()
        {
            _listenerSocket.Start();
            StartRoutingProxy();
            _isActive = true;
            while (_isActive)
            {
                var s = _listenerSocket.AcceptTcpClient();
                var processor = new ConnectionProcessor(s, this, _waitTimeout);
                Task.Factory.StartNew(processor.ProcessConnection);
            }
        }

        #endregion Public Methods

        #region Protected Methods

        protected void StartRoutingProxy()
        {
            try
            {
                RoutingProxyConnector = new InternetRoutingProxyConnector(_serverSettings.RoutingProxyAddress, _serverSettings.RoutingProxyPort);

                RoutingProxyAvailable = true;
            }
            catch (Exception)
            {
                RoutingProxyConnector = null;
                RoutingProxyAvailable = false;
            }
        }

        #endregion Protected Methods
    }
}