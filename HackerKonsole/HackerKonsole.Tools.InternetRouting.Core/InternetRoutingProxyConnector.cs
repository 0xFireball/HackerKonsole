﻿using System;
using System.IO;
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
            _baseSocket = new TcpClient(_host, _port);
            var outputStream = new StreamWriter(new BufferedStream(_baseSocket.GetStream()));
            outputStream.WriteLine("keepalive");
            outputStream.Flush();
            outputStream.WriteLine("setid");
            outputStream.Flush();
            outputStream.WriteLine(Guid.NewGuid().ToString("N"));
            outputStream.Flush();
            outputStream.WriteLine("endcmds");
            outputStream.Flush();
        }

        #endregion Public Methods
    }
}