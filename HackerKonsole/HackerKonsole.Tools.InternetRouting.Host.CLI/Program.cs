using HackerKonsole.Tools.InternetRouting.Core;

namespace HackerKonsole.Tools.InternetRouting.Host.CLI
{
    internal class Program
    {
        #region Private Methods

        private static void Main(string[] args)
        {
            InternetRoutingProxy routingProxy = new InternetRoutingProxy("0.0.0.0", 25555);
            routingProxy.StartProxy();
        }

        #endregion Private Methods
    }
}