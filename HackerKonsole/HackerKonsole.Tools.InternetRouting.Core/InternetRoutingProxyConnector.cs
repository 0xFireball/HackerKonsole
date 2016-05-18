using System.Net.Sockets;

namespace HackerKonsole.Tools.InternetRouting.Core
{
    public class InternetRoutingProxyConnector
    {
        #region Private Fields

        private TcpClient _baseSocket;
        private string _host;
        private int _port;

        #endregion Private Fields

        #region Public Constructors

        public InternetRoutingProxyConnector(string hostname, int port)
        {
            _host = hostname;
            _port = port;
        }

        #endregion Public Constructors

        #region Public Methods

        public void ConnectToProxy()
        {
        }

        #endregion Public Methods
    }
}