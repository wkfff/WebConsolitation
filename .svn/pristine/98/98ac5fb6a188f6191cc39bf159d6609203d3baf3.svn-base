using System;
//using System.Runtime.Remoting.Lifetime;

using Krista.FM.ServerLibrary;
using Krista.FM.Common;

namespace Krista.FM.Client.Common
{
    /// <summary>
    /// Структура с набором методов для обеспечения взаимодействия с объектами третьего звена
    /// </summary>
    public struct ClientConnectionHelper
    {
        public static IServer ConnectToServer(string machineName, string port, string objectAddress, string userName)
        {
            if (userName == String.Empty)
                LogicalCallContextData.SetAuthorization();
            else
                LogicalCallContextData.SetAuthorization(userName);

            string protocol = System.Configuration.ConfigurationManager.AppSettings["ServerProtocol"];
            if (protocol == String.Empty)
                protocol = "tcp";

            string[] machineNameNPort = machineName.Split(':');
            if (machineNameNPort.GetLength(0) == 2)
            {
                machineName = machineNameNPort[0];
                port = machineNameNPort[1];
            }

            string url = String.Format("{0}://{1}:{2}/{3}", protocol, machineName, port, objectAddress);

            // Создаем прозрачный прокси объект для взаимодействия с сервером
            IServer proxy = (IServer)Activator.GetObject(typeof(IServer), url);

            try
            {
                // Делаем первое обращение к удаленному серверу для проверки соединения(доступности сервера)
                proxy.Activate();
            }
            catch
            {
                return null;
            }
            return proxy;
        }

        public static IServer ConnectToServer(string machineName, string port, string objectAddress)
        {
            return ConnectToServer(machineName, port, objectAddress, String.Empty);
        }

        public static IServer ConnectToServer(string machineName, string port)
        {
            return ConnectToServer(machineName, port, System.Configuration.ConfigurationManager.AppSettings["ServerAddress"]);
        }

        public static IServer ConnectToServer(string machineName)
        {
            return ConnectToServer(machineName, System.Configuration.ConfigurationManager.AppSettings["ServerPort"]);
        }
    }

}