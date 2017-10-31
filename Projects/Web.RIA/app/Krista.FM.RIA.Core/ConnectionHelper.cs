using System;
using System.Configuration;
using System.Runtime.Remoting.Messaging;
using System.Web.SessionState;
using System.Web.UI;
using Krista.FM.Client.SMO;
using Krista.FM.Common;
using Krista.FM.ServerLibrary;
using Microsoft.Practices.Unity;

namespace Krista.FM.RIA.Core
{
    public class ConnectionHelper : Page
    {
        public const string SchemeKeyName = "Scheme";
        public const string LogicalCallContextDataKeyName = "LogicalCallContextData";

        private const string ClientSessionKeyName = "ClientSession";
        private const string Url = "tcp://{0}/FMServer/Server.rem";

        public static void Connect(AuthenticationType authType, string login, string password, HttpSessionState session)
        {
            try
            {
                session[SchemeKeyName] = null;

                IServer server = null;
                IScheme scheme = session[SchemeKeyName] as IScheme;
                if (scheme == null)
                {
                    server = (IServer)Activator.GetObject(typeof(IServer), String.Format(Url, ConfigurationManager.AppSettings["SchemeServerName"]));
                    server.Activate();
                }

                if (server != null)
                {
                    // Не будем пытаться достать запомненный.
                    LogicalCallContextData.SetAuthorization(login);

                    ClientSession clientSession = ClientSession.CreateSession(SessionClientType.RIA);
                    
                    // запоминаем ее в переменной сессии
                    session[ClientSessionKeyName] = clientSession;

                    if (authType != AuthenticationType.atWindows)
                    {
                        password = PwdHelper.GetPasswordHash(password);
                    }

                    string errStr = string.Empty;
                    if (!server.Connect(out scheme, authType, login, password, ref errStr))
                    {
                        throw new Exception(String.Format("Ошибка при подключении к схеме: {0}", errStr));
                    }

                    session[SchemeKeyName] = new SmoScheme(scheme.GetSMOObjectData());

                    // Для того, чтобы из задачи узнать тип аутентификации пользователя,
                    // сохраним его в контексте вызова
                    LogicalCallContextData lccd = LogicalCallContextData.GetContext();
                    lccd["AuthType"] = AuthenticationType.adPwdSHA512;

                    // запоминаем контекст вызова (для установки в последующих вызовах методов в сессии)
                    session[LogicalCallContextDataKeyName] = LogicalCallContextData.GetContext();
                    
                    // устанавливаем сессии такой же таймаут как максимальная задрежка в отклике клиентской сессии
                    session.Timeout = scheme.SessionManager.MaxClientResponseDelay.Minutes;

                    IUnityContainer container = Resolver.Get<IUnityContainer>();
                    container.RegisterInstance(session[SchemeKeyName], new ContainerControlledLifetimeManager());
                }
            }
            finally
            {
                CallContext.SetData("Authorization", null);
            }
        }

        public static void Disconnect(HttpSessionState session)
        {
            try
            {
                ClientSession clientSession = session[ClientSessionKeyName] as ClientSession;
                if (clientSession != null)
                {
                    clientSession.Close();
                }

                IScheme scheme = session[SchemeKeyName] as IScheme;
                if (scheme != null)
                {
                    try
                    {
                        LogicalCallContextData.SetContext((LogicalCallContextData)session[LogicalCallContextDataKeyName]);
                        scheme.Server.Disconnect();
                    }
                    catch (Exception e)
                    {
                        Core.Trace.TraceError("Ошибка вызова метода сервера Disconnect()", e);
                    }
                }
            }
            finally
            {
                CallContext.SetData("Authorization", null);

                session[SchemeKeyName] = null;
                session[ClientSessionKeyName] = null;
                session[LogicalCallContextDataKeyName] = null;
            }
        }
    }
}
