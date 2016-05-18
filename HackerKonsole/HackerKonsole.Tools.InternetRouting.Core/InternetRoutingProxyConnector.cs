using System.Net.Sockets;

namespace HackerKonsole.Tools.InternetRouting.Core
{
    public class InternetRoutingProxyConnector
    {
        #region Private Fields

        private TcpClient _baseSocket;

        #endregion Private Fields

        #region Public Constructors

        public InternetRoutingProxyConnector(string hostname, int port)
        {
            _baseSocket = new TcpClient(hostname, port);
        }

        #endregion Public Constructors

        #region Public Methods

        public void ConnectToProxy()
        {
        }

        #endregion Public Methods
    }
}