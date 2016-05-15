
using System;
using HackerKonsole.Controller.Common;

namespace HackerKonsole.Controller.CLI
{
	class Program
	{
		public static void Main(string[] args)
		{
			Console.WriteLine("HackerKonsole CLI Controller");
			Console.WriteLine("(c) 2016, ExaPhaser Industries");
			ConnectionInfo connInfo = null;
			if (args.Length >= 2)
			{
				connInfo = new ConnectionInfo()
				{
					RemoteHost = args[0],
					RemotePort = int.Parse(args[1]),
				};
			}
			else
			{
				connInfo = new ConnectionInfo()
				{
					RemoteHost = ConsoleExtensions.ReadWrite("Remote Host: "),
					RemotePort = int.Parse(ConsoleExtensions.ReadWrite("Remote Port: ")),
				};
			}
			Console.WriteLine("Attempting to establish connection...");
		}
	}
}