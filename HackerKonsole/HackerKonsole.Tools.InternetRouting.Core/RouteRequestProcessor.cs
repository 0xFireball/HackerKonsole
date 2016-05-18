using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace HackerKonsole.Tools.InternetRouting.Core
{
    public class RouteRequestProcessor
    {
        #region Private Fields

        private TcpClient _baseSocket;
        private ProxyRoutingConnectorService _proxyRoutingConnectorService;
        private bool _stayConnected;

        #endregion Private Fields

        #region Public Constructors

        public RouteRequestProcessor(TcpClient baseSocket, ProxyRoutingConnectorService proxyRoutingConnectorService)
        {
            this._baseSocket = baseSocket;
            this._proxyRoutingConnectorService = proxyRoutingConnectorService;
        }

        #endregion Public Constructors

        #region Public Methods

        public void KillConnection()
        {
            _baseSocket.Close();
            _stayConnected = false;
        }

        public void ProcessConnection()
        {
            try
            {
                var inputStream = new StreamReader(_baseSocket.GetStream());
                var outputStream = new StreamWriter(new BufferedStream(_baseSocket.GetStream()));
                _proxyRoutingConnectorService.ConnectedClients.Add(HostClient);
                //Impatiently wait for data
                _baseSocket.ReceiveTimeout = 10000;
                bool getCmds = true;
                while (getCmds)
                {
                    string command = inputStream.ReadLine();
                    switch (command)
                    {
                        case "setid":
                            string id = inputStream.ReadLine();
                            HostClient.MemberId = id;
                            break;

                        case "getroute":
                            string targetRouteId = inputStream.ReadLine();
                            var targetCandidates = _proxyRoutingConnectorService.ConnectedClients.Where(cc => cc.MemberId == targetRouteId).ToList();
                            if (targetCandidates.Count > 0)
                            {
                                outputStream.WriteLine("SUCCESS");
                                var routingController = new InternetRoutingProxy();
                                var routeTargetClient = targetCandidates[0];
                                var remoteClientEndpoint = (IPEndPoint)routeTargetClient.RequestProcessor._baseSocket.Client.RemoteEndPoint;
                                routingController.StartProxy(new IPEndPoint(IPAddress.Parse("0.0.0.0"), 21337), remoteClientEndpoint);
                                outputStream.Flush();
                            }
                            else
                            {
                                outputStream.WriteLine("FAIL");
                                outputStream.Flush();
                            }
                            break;

                        case "keepalive":
                            _stayConnected = true;
                            break;

                        case "endcmds":
                            getCmds = false;
                            break;
                    }
                }
                //Got command, now keep alive
                _baseSocket.ReceiveTimeout = -1;
                while (_stayConnected)
                {
                    //Keep connection alive
                }
            }
            catch (Exception)
            {
                if (_baseSocket.Connected)
                    _baseSocket.Close();
            }
            _proxyRoutingConnectorService.ConnectedClients.Remove(HostClient);
        }

        #endregion Public Methods

        #region Public Properties

        public RoutingConnectorClient HostClient { get; set; }

        #endregion Public Properties
    }
}