/*
 */
using System;
using System.Net;
using System.Net.Sockets;

namespace HackerKonsole.ConnectionServices
{
	/// <summary>
	/// A virtualization layer over TcpClient that provides end-to-end encryption.
	/// </summary>
	public class CryptTcpClient
	{
		TcpClient _tcpClient;
		
		public CryptTcpClient(TcpClient existingTcpClient)
		{
			_tcpClient = existingTcpClient;
		}
	}
}
