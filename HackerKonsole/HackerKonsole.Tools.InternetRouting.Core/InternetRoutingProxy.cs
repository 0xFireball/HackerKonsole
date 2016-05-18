using System;
using System.Net;
using System.Net.Sockets;

namespace HackerKonsole.Tools.InternetRouting.Core
{
    /// <summary>
    /// A class to proxy a Socket connection
    /// </summary>
    public class InternetRoutingProxy
    {
        private readonly Socket _baseSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); //Proxy through a TCP socket

        /// <summary>
        /// Starts the proxy between localEndPoint and remoteEndPoint
        /// </summary>
        /// <param name="localEndPoint">The local endpoint of the tunnel</param>
        /// <param name="remoteEndPoint">The remote endpoint of the tunnel</param>
        public void StartProxy(IPEndPoint localEndPoint, IPEndPoint remoteEndPoint)
        {
            _baseSocket.Bind(localEndPoint);
            _baseSocket.Listen(10);

            while (true)
            {
                var source = _baseSocket.Accept();
                var destination = new InternetRoutingProxy();
                var forwardingInformation = new ForwardingInfo(source, destination._baseSocket);
                destination.Connect(remoteEndPoint, source);
                source.BeginReceive(forwardingInformation.Buffer, 0, forwardingInformation.Buffer.Length, 0, OnDataReceive, forwardingInformation);
            }
        }

        private void Connect(EndPoint remoteEndpoint, Socket destination)
        {
            var forwardingInformation = new ForwardingInfo(_baseSocket, destination);
            _baseSocket.Connect(remoteEndpoint);
            _baseSocket.BeginReceive(forwardingInformation.Buffer, 0, forwardingInformation.Buffer.Length, SocketFlags.None, OnDataReceive, forwardingInformation);
        }

        private static void OnDataReceive(IAsyncResult result)
        {
            var forwardingInformation = (ForwardingInfo)result.AsyncState;
            try
            {
                var bytesRead = forwardingInformation.SourceSocket.EndReceive(result);
                if (bytesRead > 0)
                {
                    forwardingInformation.DestinationSocket.Send(forwardingInformation.Buffer, bytesRead, SocketFlags.None);
                    forwardingInformation.SourceSocket.BeginReceive(forwardingInformation.Buffer, 0, forwardingInformation.Buffer.Length, 0, OnDataReceive, forwardingInformation);
                }
            }
            catch
            {
                forwardingInformation.DestinationSocket.Close();
                forwardingInformation.SourceSocket.Close();
            }
        }
    }
}