/*
 */

using HackerKonsole.ServerCore;

namespace HackerKonsole.ServerHosts.VisibleCLI
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            HackerKonsoleServerMaster.StartServer(args);
        }
    }
}