
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
		public Dictionary<string, string> ConnectionHeaders;
		
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
					List<string> rawConnHeaders = new List<string>();
					string recvData;
					//Wait for HK
					waiting = true;
					while (!(recvData = InputStream.ReadLine()).Trim().Contains(CommonMessages.StartConnectionRequestIndication))
					{
					}
					SendLine(CommonMessages.BeginConnectionHelloBanner); //Send welcome banner
					//Get headers
					while ((recvData = InputStream.ReadLine()).Trim() != "")
					{
						rawConnHeaders.Add(recvData);
					}
					recvData = null;					
					ParseHeaders(rawConnHeaders.ToArray());
					if (ConnectionHeaders == null)
					{
						//Headers failed to parse
						throw new FormatException("Failed to parse headers because they were malformed.");
					}
					SendLine(CommonMessages.WelcomeMaster);
					//Interpreter mode
					while (true)
					{
						recvData = InputStream.ReadLine().Trim();
						ParseCommand(recvData);
					}
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
		
		public void SendLine(string line)
		{
			OutputStream.WriteLine(line);
			OutputStream.Flush();
		}
		
		public void ParseHeaders(string[] rawHeaders)
		{
			Dictionary<string, string> parsedHeaders = new Dictionary<string, string>();
			try
			{
				foreach (string rawHeaderLine in rawHeaders)
				{
					rawHeaderLine.Trim();
					var separator = rawHeaderLine.IndexOf(':');
	                if (separator == -1)
	                {
	                    throw new ArgumentOutOfRangeException("Invalid header line: " + rawHeaderLine);
	                }
	                var name = rawHeaderLine.Substring(0, separator).ToLowerInvariant();
	                var pos = separator + 1;
					var value = rawHeaderLine.Substring(pos, rawHeaderLine.Length - pos);
					Logger.WriteLine("Parsed header: {0}:{1}", name, value);
					parsedHeaders[name] = value;
				}
				ConnectionHeaders = parsedHeaders;
			}
			catch (KillConnectionException)
			{
				Logger.WriteLine("Connection closed by client.");
			}
			catch (IndexOutOfRangeException iox)
			{
				Logger.WriteLine("Malformed headers, parse error: {0}", iox);
			}
			catch (Exception ex)
			{
				Logger.WriteLine("Generic exception parsing headers: {0}", ex);
			}
		}
		private void ParseCommand(string command)
		{
			switch (command)
			{
				case "bye":
				case "exit":
				case "leave":
				case "kill":
					SendLine(CommonMessages.GracefulByeMessage); //Send graceful bye message
					throw new KillConnectionException("Client killed connection.");
			}
		}
	}
}
