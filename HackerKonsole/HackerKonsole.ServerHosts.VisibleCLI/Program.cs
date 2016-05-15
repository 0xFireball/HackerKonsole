/*
 */
using System;
using HackerKonsole.ServerCore;

namespace HackerKonsole.ServerHosts.VisibleCLI
{
	class Program
	{
		public static void Main(string[] args)
		{
			HackerKonsoleServerMaster.StartServer(args);
		}
	}
}