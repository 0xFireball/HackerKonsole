/*
 */

using System;
using System.Diagnostics;
using System.IO;
using HackerKonsole.ConnectionServices;

namespace HackerKonsole.ServerCore
{
    /// <summary>
    ///     HK Tools
    /// </summary>
    public static class HKServices
    {
        public static void RemoteShell(CryptTcpClient encryptedConnection, Action<string> sendLine)
        {
            sendLine("=======SHELL=======");
            sendLine("[you are now in shell mode, type `exit` to exit shell.]");
            var stayInShell = true;
            var shellPath = @"C:\Windows\System32\CMD.exe";
            var shellProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    FileName = shellPath
                }
            };


            shellProcess.OutputDataReceived += (sender, e) => sendLine("[shell] " + e.Data);
            shellProcess.ErrorDataReceived += (sender, e) => sendLine("[shell] " + e.Data);
            shellProcess.Start();
            var procStdIn = shellProcess.StandardInput;

            shellProcess.BeginOutputReadLine();
            shellProcess.BeginErrorReadLine();

            while (stayInShell)
            {
                var command = encryptedConnection.ReadLineCrypto();
                switch (command)
                {
                    case "exit":
                        sendLine("You have exited shell.");
                        stayInShell = false;
                        shellProcess.Close();
                        break;
                    default:
                        //Execute shell command
                        procStdIn.WriteLine(command);
                        procStdIn.Flush();
                        break;
                }
            }
        }
    }
}