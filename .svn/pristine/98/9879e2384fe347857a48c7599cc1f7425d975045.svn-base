using System;
using Krista.FM.Common;
using Krista.FM.Extensions;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.Consolidation.Tests.Helpers
{
    public class ServerSchemeConnectionHelper
    {
        public static IScheme Connect(string serverAddress, string user)
        {
            var server = (IServer)Activator.GetObject(typeof(IServer), "tcp://{0}/FMServer/Server.rem".FormatWith(serverAddress));
            server.Activate();

            LogicalCallContextData.SetAuthorization(user);
            ClientSession.CreateSession(SessionClientType.WebService);
            string errStr = string.Empty;
            IScheme serverScheme;
            if (!server.Connect(out serverScheme, AuthenticationType.atWindows, user, String.Empty, ref errStr))
            {
                throw new Exception(String.Format("Ошибка при подключении к схеме: {0}", errStr));
            }

            return serverScheme;
        }
    }
}
