
using System;
using System.Collections.Generic;

namespace HackerKonsoleServer.Common
{
	public class ServerSettings
	{
		public string BindAddress;
		public int Port;
		public int WaitTimeout = 10000;
		public bool EnableLogging = false;
	}
}