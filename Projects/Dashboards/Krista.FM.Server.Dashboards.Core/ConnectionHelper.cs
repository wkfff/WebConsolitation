using System;
using System.Configuration;
using System.Data;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using Krista.FM.Common;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.Dashboards.Core
{
	public class ConnectionHelper : Page
    {
        private const string CLIENT_SESSION_KEY_NAME = "ClientSession";
        private const string SERVER_KEY_NAME = "Server";
		public const string LOGICAL_CALL_CONTEXT_DATA_KEY_NAME = "LogicalCallContextData";
		public const string SCHEME_KEY_NAME = "Scheme";
                
        public static void Connect(AuthenticationType authType, string login, string password)
        {
            try
            {
                HttpSessionState session = HttpContext.Current.Session;
                session[SERVER_KEY_NAME] = null;
                session[SCHEME_KEY_NAME] = null;

                IServer server = session[SERVER_KEY_NAME] as IServer;
                if (server == null)
                {
                    server = (IServer)Activator.GetObject(typeof(IServer), String.Format(CustomReportConst.ServerURL, ConfigurationManager.AppSettings["SchemeServerName"]));
                    session[SERVER_KEY_NAME] = server;
                }
                                
                // Не будем пытаться достать запомненный.
                LogicalCallContextData.SetAuthorization(login);

                ClientSession clientSession = ClientSession.CreateSession(SessionClientType.WebService);
                // запоминаем ее в переменной сессии
                session[CLIENT_SESSION_KEY_NAME] = clientSession;

                if (authType != AuthenticationType.atWindows)
                {
                    password = PwdHelper.GetPasswordHash(password);
                }

                IScheme scheme;
                string errStr = string.Empty;
                if (!server.Connect(out scheme, authType, login, password, ref errStr))
                    throw new Exception(String.Format("Ошибка при подключении к схеме: {0}", errStr));

                DataTable users = scheme.UsersManager.GetUsers();
                DataRow[] rows = users.Select(String.Format("ID = {0}", scheme.UsersManager.GetCurrentUserID()));

                string username = scheme.UsersManager.GetCurrentUserName();
                string userSurname = string.Empty;
                string description = String.Empty;
                if (rows.Length > 0)
                {
                    userSurname = String.Format("{0} {1} {2}", rows[0]["LASTNAME"], rows[0]["FIRSTNAME"], rows[0]["PATRONYMIC"]);
                    if (rows[0]["DESCRIPTION"] != DBNull.Value)
                    {
                        description = rows[0]["DESCRIPTION"].ToString();
                    }
                }
                    
                userSurname = userSurname.Length > 2 ? userSurname : username;
                int currentUserId = scheme.UsersManager.GetCurrentUserID();
                DataTable groups = scheme.UsersManager.GetGroupsForUser(currentUserId);
                DataRow[] webAdminGroupRow = groups.Select("name = 'Web-администраторы' and IsMember = true");
                if (webAdminGroupRow.Length == 1)
                {
                    session["IsWebAdministrator"] = true;
                }
                DataRow[] oivAdminGroupRow = groups.Select("name = 'Администратор ОИВ' and IsMember = true");
                if (oivAdminGroupRow.Length == 1)
                {
                    session["IsOivAdministrator"] = true;
                }  
                session[SCHEME_KEY_NAME] = scheme;
                session[CustomReportConst.currentUserKeyName] = username;
                session[CustomReportConst.currentUserSurnameKeyName] = userSurname;
                session[CustomReportConst.currentUserDescriptionKeyName] = description;
                // запоминаем контекст вызова (для установки в последующих вызовах методов в сессии)
                session[LOGICAL_CALL_CONTEXT_DATA_KEY_NAME] = LogicalCallContextData.GetContext();
                // инициируем запуск процесса нотификации сервера о живости сессии
                clientSession.SessionManager = scheme.SessionManager;
                // устанавливаем сессии такой же таймаут как максимальная задрежка в отклике клиентской сессии
                session.Timeout = scheme.SessionManager.MaxClientResponseDelay.Minutes;

                CRHelper.SaveToUserLog(string.Format("Подключение: {0}, ID сессии: {1}",
                    scheme.UsersManager.GetCurrentUserName(), ClientAuthentication.SessionID));

                if (!scheme.UsersManager.CheckPermissionForSystemObject(
                        "WebReports", (int)WebReportsOperations.View, false))
                {
                    // тут надо перебросить на страничку аутентификации с сохранением сообщения.
                    HttpContext.Current.Session[CustomReportConst.strPermissionErrorMessage] = String.Format("У пользователя {0} нет прав на просмотр отчетов", scheme.UsersManager.GetCurrentUserName());
                    HttpContext.Current.Server.Transfer(CustomReportConst.autenticatePageUrl);
                }
            }
            finally
            {
                CallContext.SetData("Authorization", null);
            }
        }

        public static string CheckScheme()
        {
            LogicalCallContextData.SetAuthorization();
            IServer server = (IServer)Activator.GetObject(typeof(IServer), String.Format(CustomReportConst.ServerURL, ConfigurationManager.AppSettings["SchemeServerName"]));
            try
            {
                server.Activate();
            }
            catch (Exception e)
            {
                return e.Message;
            }
            string schemeName = String.Empty;
            foreach (string item in server.SchemeList)
            {
                schemeName = item;
            }
            return schemeName;
        }

        private static bool TryConnect(string login, string password, AuthenticationType authenticationType)
        {
            try
            {
                Connect(authenticationType, login, password);
                return true;
            }
            catch (Exception e)
            {
                CRHelper.SaveToUserLog(string.Format("Неудачная попытка подключения: {0}; {1}", login, e.Message));
                CRHelper.SaveToErrorLog(string.Format("Неудачная попытка подключения: {0}; {1};", login, CRHelper.GetExceptionInfo(e)));
                HttpContext.Current.Session[CustomReportConst.strAppErrorMessage] = e.Message;
                HttpContext.Current.Server.Transfer(CustomReportConst.userErrorPageUrl);
                return false;
            }
        }

        private static bool haveUrlLoginParam
        {
            get
            {
                return HttpContext.Current.Request.Params[CustomReportConst.loginParamName] != null &&
                       HttpContext.Current.Request.Params[CustomReportConst.passwordParamName] != null;
            }
        }

        /// <summary>
        /// Создает сессию.
        /// </summary>
        /// <returns>true если получилось, false в противном случае.</returns>
        public static bool CreateClientSession()
        {
            // если была доменная аутентификация
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                // создаем из домена.
                return TryConnect(HttpContext.Current.User.Identity.Name, string.Empty, AuthenticationType.atWindows);
            }
            // Если в урле есть параметры аутентификации
            if (haveUrlLoginParam)
            {
                // создаем по параметрам урла.
                return TryConnect(HttpContext.Current.Request.Params[CustomReportConst.loginParamName],
                        HttpContext.Current.Request.Params[CustomReportConst.passwordParamName],
                        AuthenticationType.adPwdSHA512);
            }

            // Попробуем достать из кукисов
            //            HttpCookie cookie = Request.Cookies["Login"];
            //            if (cookie != null)
            //            {
            //                string loginValue = cookie[CustomReportConst.loginParamName];
            //                string PasswordValue = cookie[CustomReportConst.passwordParamName];
            //                loginValue = CRHelper.ConvertEncoding(loginValue, Request.ContentEncoding, Encoding.Default);
            //                PasswordValue = CRHelper.ConvertEncoding(PasswordValue, Request.ContentEncoding, Encoding.Default);
            //                return TryConnect(loginValue, PasswordValue, AuthenticationType.adPwdSHA512);
            //            }

            // Попробуем создать гостевую сессию
            if (ConfigurationManager.AppSettings[CustomReportConst.GuestUserKeyName] != null)
            {
                string[] guestLogin =
                        ConfigurationManager.AppSettings[CustomReportConst.GuestUserKeyName].Split(';');
                if (guestLogin.Length == 2)
                {
                    return TryConnect(guestLogin[0], guestLogin[1], AuthenticationType.adPwdSHA512);
                }
            }
            // если никак не получилось
            return false;
        }
    }
}
