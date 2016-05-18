﻿using System.Net.Sockets;

namespace HackerKonsole.Tools.InternetRouting.Core
{
    /// <summary>
    /// A class to route a TCP connection over the internet.
    /// </summary>
    public class InternetRoutingProxy
    {
        #region Private Fields

        private TcpClient _baseSocket;

        #endregion Private Fields

        #region Public Constructors

        public InternetRoutingProxy(string bindHostname, int port)
        {
            _baseSocket = new TcpClient(bindHostname, port);
        }

        #endregion Public Constructors

        #region Public Methods

        public void StartProxy()
        {
        }

        #endregion Public Methods
    }
}