using System;

namespace HackerKonsoleServer.Common
{
    /// <summary>
    ///     A logger
    /// </summary>
    public static class Logger
    {
        public static bool EnableLogging;

        public static void WriteLine(string format, params object[] args)
        {
            if (EnableLogging)
                Console.WriteLine(format, args);
        }
    }
}