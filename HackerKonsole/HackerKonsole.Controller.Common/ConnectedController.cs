using System;
using System.IO;
using System.Threading.Tasks;
using HackerKonsole.ConnectionServices;

namespace HackerKonsole.Controller.Common
{
    public class ConnectedController
    {
        private const string NetShellPrompt = "HK $>";
        private CryptTcpClient _baseSocket;
        public StreamReader InputStream { get; }
        public StreamWriter OutputStream { get; }
        public bool StayConnected { get; private set; }

        public ConnectedController(CryptTcpClient connectedCryptTcpClient)
        {
            _baseSocket = connectedCryptTcpClient;
            OutputStream = new StreamWriter(new BufferedStream(_baseSocket.GetStream()));
            InputStream = new StreamReader(_baseSocket.GetStream());
        }

        public void InteractiveNetShell()
        {
            StayConnected = true;
            Task.Factory.StartNew(ReceiveData);
            while (StayConnected)
            {
                OutputStream.WriteLine(ConsoleExtensions.ReadWrite(NetShellPrompt));
                OutputStream.Flush();
            }
        }

        private void ReceiveData()
        {
            while (StayConnected)
            {
                bool typedSomething = Console.CursorLeft > NetShellPrompt.Length;
                if (typedSomething)
                {
                    Console.WriteLine();                    
                }
                Console.WriteLine(InputStream.ReadLine());
                if (typedSomething)
                {
                    Console.Write(NetShellPrompt);
                }
            }
        }
    }
}