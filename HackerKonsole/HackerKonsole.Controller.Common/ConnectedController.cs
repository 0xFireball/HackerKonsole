using System;
using System.IO;
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
                string command = ConsoleExtensions.ReadWrite(NetShellPrompt);
                SendCommandCli(command);
            }
        }

        public void ReceiveDataWithCallback(Action<string> callback)
        {
            StayConnected = true;
            while (StayConnected)
            {
                callback(ReceiveStuff());
            }
        }

        public void SendCommandCli(string command)
        {
            switch (command)
            {
                case "pullfile":
                    Console.WriteLine("You're pulling a file. Follow the wizard:");
                    string remoteFilePath = ConsoleExtensions.ReadWriteLine("Path on remote machine: ");
                    string localFilePath = ConsoleExtensions.ReadWriteLine("Path to save file to: ");
                    _encryptedConnection.WriteLineCrypto(command + " " + remoteFilePath); //send a request to pull the remote file
                    byte[] fileHunk = _encryptedConnection.ReadLineCrypto().GetBytes(); //Get a big chunk file
                    File.WriteAllBytes(localFilePath, fileHunk);
                    break;

                case "pushfile":
                    Console.WriteLine("You're pushing a file. Follow the wizard:");
                    string localPushFileLocation = ConsoleExtensions.ReadWriteLine("Path on local machine: ");
                    string remoteFileSavePath = ConsoleExtensions.ReadWriteLine("Path on remote machine to save file to: ");
                    _encryptedConnection.WriteLineCrypto(command + " " + remoteFileSavePath); //send a request to push the file to the remote
                    _encryptedConnection.WriteLineCrypto(File.ReadAllBytes(localPushFileLocation).GetString());
                    _encryptedConnection.Flush();
                    break;

                case "exit":
                    _encryptedConnection.WriteLineCrypto(command);
                    _encryptedConnection.Close();
                    break;

                default:
                    SendGenericCommand(command);
                    break;
            }
        }

        public void SendGenericCommand(string genericCommand)
        {
            _encryptedConnection.WriteLineCrypto(genericCommand);
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
                Console.WriteLine(ReceiveStuff());
                if (typedSomething)
                {
                    Console.Write(NetShellPrompt);
                }
            }
        }

        private string ReceiveStuff()
        {
            return _encryptedConnection.ReadLineCrypto();
        }

        #endregion Private Methods
    }
}