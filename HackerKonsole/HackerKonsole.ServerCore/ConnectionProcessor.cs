
using System;
using System.Collections.Generic;
using System.IO;
using HackerKonsoleServer.Common;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;


namespace HackerKonsole.ServerCore
{
	/// <summary>
	/// A class to process connections
	/// </summary>
	public class ConnectionProcessor
	{
		public RatServer BaseServer;
		public TcpClient BaseSocket;
		public StreamReader InputStream;
		public StreamWriter OutputStream;
		public int WaitTimeout;
		
		public ConnectionProcessor(TcpClient s, RatServer srv, int waitTimeout)
		{
			BaseSocket = s;
			BaseServer = srv;
			WaitTimeout = waitTimeout;
		}
		
		public void ProcessConnection()
		{
			InputStream = new StreamReader(BaseSocket.GetStream());
			BaseSocket.ReceiveTimeout = WaitTimeout;
			bool waiting = false;
			// StreamWriter - easy processing for output
			using (OutputStream = new StreamWriter(new BufferedStream(BaseSocket.GetStream()))) 
			{
				try 
				{
					List<string> connHeaders = new List<string>();
					string recvData;
					//Wait for HK
					waiting = true;
					while (!(recvData = InputStream.ReadLine()).Trim().Contains("HK>>>"))
					{
					}
					//Get headers
					while ((recvData = InputStream.ReadLine()).Trim() != "")
					{
						connHeaders.Add(recvData);
					}
					recvData = null;
					
					
				}
				catch (IOException)
				{
					if (waiting)
					{
						Console.WriteLine("Connection timeout waiting for client.");
					}
				}
				catch (Exception ex) {
					//The ultimate catch block to prevent crashes due to bad input
					Logger.WriteLine("Exception: {0}", ex);
				} 
				finally {
					InputStream = null;
					OutputStream = null;
				}
			}
			BaseSocket.Close();
		}
	}
}
