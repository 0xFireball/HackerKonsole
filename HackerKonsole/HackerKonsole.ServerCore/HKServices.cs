/*
 */
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace HackerKonsole.ServerCore
{
	/// <summary>
	/// HK Tools
	/// </summary>
	public static class HKServices
	{
		public static void RemoteShell(StreamWriter outputStream, StreamReader inputStream, Action<string> sendLine)
		{
			sendLine("=======SHELL=======");
			sendLine("[you are now in shell mode, type `exit` to exit shell.]");
			bool stayInShell = true;
			while (stayInShell)
			{
				string command = inputStream.ReadLine();
				switch (command)
				{
					case "exit":
						sendLine("You have exited shell.");
						stayInShell = false;
						break;
					default:
						//Execute shell command
						sendLine(command);
						break;
				}
			}
		}
	}
}
