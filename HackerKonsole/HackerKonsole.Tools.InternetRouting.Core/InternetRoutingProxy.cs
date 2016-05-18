using System;
using System.Net;
using System.Net.Sockets;

namespace HackerKonsole.Tools.InternetRouting.Core
{
    /// <summary>
    /// A class to route a TCP connection over the internet.
    /// </summary>
    public class InternetRoutingProxy
    {
        private readonly Socket _mainSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        public void StartProxy(IPEndPoint local, IPEndPoint remote)
        {
            _mainSocket.Bind(local);
            _mainSocket.Listen(10);

            while (true)
            {
                var source = _mainSocket.Accept();
                var destination = new InternetRoutingProxy();
                var forwardingInfo = new ForwardingInfo(source, destination._mainSocket);
                destination.Connect(remote, source);
                source.BeginReceive(forwardingInfo.Buffer, 0, forwardingInfo.Buffer.Length, 0, OnDataReceive, forwardingInfo);
            }
        }

        private void Connect(EndPoint remoteEndpoint, Socket destination)
        {
            var forwardingInfo = new ForwardingInfo(_mainSocket, destination);
            _mainSocket.Connect(remoteEndpoint);
            _mainSocket.BeginReceive(forwardingInfo.Buffer, 0, forwardingInfo.Buffer.Length, SocketFlags.None, OnDataReceive, forwardingInfo);
        }

        private static void OnDataReceive(IAsyncResult result)
        {
            var forwardingInfo = (ForwardingInfo)result.AsyncState;
            try
            {
                var bytesRead = forwardingInfo.SourceSocket.EndReceive(result);
                if (bytesRead > 0)
                {
                    forwardingInfo.DestinationSocket.Send(forwardingInfo.Buffer, bytesRead, SocketFlags.None);
                    forwardingInfo.SourceSocket.BeginReceive(forwardingInfo.Buffer, 0, forwardingInfo.Buffer.Length, 0, OnDataReceive, forwardingInfo);
                }
            }
            catch
            {
                forwardingInfo.DestinationSocket.Close();
                forwardingInfo.SourceSocket.Close();
            }
        }
    }
}