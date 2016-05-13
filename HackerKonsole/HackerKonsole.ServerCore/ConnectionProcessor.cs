
using System;
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
		public Stream InputStream;
		public StreamWriter OutputStream;
		
		public ConnectionProcessor(TcpClient s, RatServer srv)
		{
			BaseSocket = s;
			BaseServer = srv;
		}
		
		public void ProcessConnection()
		{
			// we can't use a StreamReader for input, because it buffers up extra data on us inside its
            // "processed" view of the world, and we want the data raw after the headers.
			InputStream = new BufferedStream(BaseSocket.GetStream());
			// StreamWriter - easy processing for output
			using (OutputStream = new StreamWriter(new BufferedStream(BaseSocket.GetStream()))) 
			{
				try 
				{
					//TODO: Do something
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
