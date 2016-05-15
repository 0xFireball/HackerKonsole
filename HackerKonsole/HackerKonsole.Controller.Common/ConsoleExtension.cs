/*
 */
using System;

namespace HackerKonsole.Controller.Common
{
	public static class ConsoleExtensions
	{
		public static string ReadWriteLine(string format, params object[] args)
		{
			Console.WriteLine(format, args);
			return Console.ReadLine();
		}
		
		public static string ReadWrite(string format, params object[] args)
		{
			Console.Write(format, args);
			return Console.ReadLine();
		}
	}
}
