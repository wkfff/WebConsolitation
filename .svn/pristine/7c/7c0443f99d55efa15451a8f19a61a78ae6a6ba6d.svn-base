using System;
using Krista.FM.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Tests.Common
{
    public class ServerSchemeConnectionHelper
    {
        public static IScheme Connect(
                                       string serverAddress, 
                                       int serverPort, 
                                       string user, 
                                       string password, 
                                       AuthenticationType authenticationType)
        {
            var server = (IServer)Activator.GetObject(typeof(IServer), String.Format("tcp://{0}:{1}/FMServer/Server.rem", serverAddress, serverPort));
            server.Activate();

            LogicalCallContextData.SetAuthorization(user);
            ClientSession.CreateSession(SessionClientType.WebService);
            string errStr = string.Empty;
            IScheme serverScheme;
            if (!server.Connect(out serverScheme, authenticationType, user, PwdHelper.GetPasswordHash(password), ref errStr))
            {
                throw new Exception(String.Format("Ошибка при подключении к схеме: {0}", errStr));
            }

            return serverScheme;
        }
    }
}
