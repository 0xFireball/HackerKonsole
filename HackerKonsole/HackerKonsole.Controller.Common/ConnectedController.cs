using System;
using System.Threading.Tasks;
using HackerKonsole.ConnectionServices;

namespace HackerKonsole.Controller.Common
{
    public class ConnectedController
    {
        #region Private Fields

        private const string NetShellPrompt = "HK $>";
        private readonly CryptTcpClient _encryptedConnection;

        #endregion Private Fields

        #region Public Constructors

        public ConnectedController(CryptTcpClient connectedCryptTcpClient)
        {
            _encryptedConnection = connectedCryptTcpClient;
        }

        #endregion Public Constructors

        #region Public Properties

        public bool SessionIsInitialized { get; set; }
        public bool StayConnected { get; private set; }
        #endregion Public Properties

        #region Public Methods

        public void InitializeSession()
        {
            _encryptedConnection.WriteLineCrypto("HK>>>");
            _encryptedConnection.WriteLineCrypto("\n");
            _encryptedConnection.WriteLineCrypto("\n");
            SessionIsInitialized = true;
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

        #endregion Public Methods

        #region Private Methods

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

        #endregion Private Methods
    }
}