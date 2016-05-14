using System;
using HackerKonsoleServer.Common;

namespace HackerKonsole.ServerCore
{
	/// <summary>
	/// The class that controls the actual server instance
	/// </summary>
	public static class HackerKonsoleServerMaster
	{
		public static string Version = "0.0.2";
		public static void StartServer(string[] args)
		{
			var serverSettings = new ServerSettings()
			{
				BindAddress = args[0],
				Port = int.Parse(args[1]),
				EnableLogging = args[2]=="-enablelogging",
				WaitTimeout = int.Parse(args[3]),
			};
			var hackerKonsoleServer = new HackerKonsoleServer(serverSettings);
			Logger.EnableLogging = serverSettings.EnableLogging;
			hackerKonsoleServer.StartServer(); //Run the server
		}
	}
}
