using System;
using System.Net.Sockets;
using HackerKonsole.ConnectionServices;
using HackerKonsole.Controller.Common;

namespace HackerKonsole.Controller.CLI
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("HackerKonsole CLI Controller");
            Console.WriteLine("(c) 2016, ExaPhaser Industries");
            ConnectionInfo connInfo = null;
            if (args.Length >= 2)
            {
                connInfo = new ConnectionInfo
                {
                    RemoteHost = args[0],
                    RemotePort = int.Parse(args[1])
                };
            }
            else
            {
                connInfo = new ConnectionInfo
                {
                    RemoteHost = ConsoleExtensions.ReadWrite("Remote Host: "),
                    RemotePort = int.Parse(ConsoleExtensions.ReadWrite("Remote Port: "))
                };
            }
            using (var tcpConnection = new TcpClient(connInfo.RemoteHost, connInfo.RemotePort))
            {
                var cryptConnection = new CryptTcpClient(tcpConnection);
                try
                {
                    Console.WriteLine("Attempting to establish connection...");
                    cryptConnection.ClientPerformKeyExchange();
                    Console.WriteLine("Connection successfully established!");

                    var connectedController = new ConnectedController(cryptConnection);
                    connectedController.InitializeSession();
                    connectedController.InteractiveNetShell();
                }
                catch (Exception ex)
                {
                    cryptConnection.Close();
                    Console.WriteLine("An error occurred with the connection: " + ex);
                }
            }
        }
    }
}