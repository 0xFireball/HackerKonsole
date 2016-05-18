using System.Net.Sockets;

namespace HackerKonsole.Tools.InternetRouting.Core
{
    internal class ForwardingInfo
    {
        #region Public Properties

        public byte[] Buffer { get; private set; }
        public Socket DestinationSocket { get; private set; }
        public Socket SourceSocket { get; private set; }

        #endregion Public Properties

        #region Public Methods

        public ForwardingInfo(Socket source, Socket destination)
        {
            SourceSocket = source;
            DestinationSocket = destination;
            Buffer = new byte[8192];
        }

        #endregion Public Methods
    }
}