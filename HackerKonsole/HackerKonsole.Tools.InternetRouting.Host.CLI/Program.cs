using HackerKonsole.Tools.InternetRouting.Core;

namespace HackerKonsole.Tools.InternetRouting.Host.CLI
{
    internal class Program
    {
        #region Private Methods

        private static void Main(string[] args)
        {
            ProxyRoutingConnectorService routingConnectorService = new ProxyRoutingConnectorService("0.0.0.0", 25555);
            routingConnectorService.StartRouter();
        }

        #endregion Private Methods
    }
}