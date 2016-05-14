
using System;
using HackerKonsoleServer.Common;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace HackerKonsole.ServerCore
{
	/// <summary>
	/// A RatServer
	/// </summary>
	public abstract class RatServer
	{
		readonly ServerSettings _serverSettings;
		readonly string bindAddress;
		readonly int bindPort;
		readonly TcpListener _listenerSocket;
		bool _isActive;
		int _waitTimeout;
		
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
