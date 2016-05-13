
using System;
using HackerKonsoleServer.Common;
using HackerKonsole.ServerCore;

namespace HackerKonsole.Server.Stealth
{
	class Program
	{
		public static void Main(string[] args)
		{
			var serverSettings = new ServerSettings()
			{
				BindAddress = args[0],
				Port = int.Parse(args[1]),
				EnableLogging = args[2]=="-enablelogging",
			};
			HackerKonsoleServerMaster.StartServer(serverSettings);
		}
	}
}