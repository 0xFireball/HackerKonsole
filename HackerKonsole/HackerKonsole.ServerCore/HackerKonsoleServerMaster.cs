using System;
using HackerKonsoleServer.Common;

namespace HackerKonsole.ServerCore
{
	/// <summary>
	/// The class that controls the actual server instance
	/// </summary>
	public static class HackerKonsoleServerMaster
	{
		public static void StartServer(ServerSettings serverSettings)
		{
			var hackerKonsoleServer = new HackerKonsoleServer(serverSettings);
			Logger.EnableLogging = serverSettings.EnableLogging;
			hackerKonsoleServer.StartServer(); //Run the server
		}
	}
}
