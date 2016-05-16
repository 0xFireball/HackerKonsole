using System;
using System.IO;
using System.Threading.Tasks;
using HackerKonsole.ConnectionServices;

namespace HackerKonsole.Controller.Common
{
    public class ConnectedController
    {
        private const string NetShellPrompt = "HK $>";
        private readonly CryptTcpClient _encryptedConnection;
        public bool StayConnected { get; private set; }

        public ConnectedController(CryptTcpClient connectedCryptTcpClient)
        {
            _encryptedConnection = connectedCryptTcpClient;
        }

        public void InteractiveNetShell()
        {
            StayConnected = true;
            Task.Factory.StartNew(ReceiveData);
            while (StayConnected)
            {
                _encryptedConnection.WriteLineCrypto(ConsoleExtensions.ReadWrite(NetShellPrompt));
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
                Console.WriteLine(_encryptedConnection.ReadLineCrypto());
                if (typedSomething)
                {
                    Console.Write(NetShellPrompt);
                }
            }
        }
    }
}