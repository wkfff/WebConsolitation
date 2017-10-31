using System;
using System.Collections.Generic;
using System.Data;

using Krista.FM.Common;

namespace Krista.FM.Server.Common
{
    /// <summary>
    /// Статический класс предоставляющий методы для доступа к информации в контексте пользователя
    /// </summary>
    public static class SessionContext
    {
        /// <summary>
        /// Системный контекст сервера.
        /// </summary>
        private static LogicalCallContextData systemContext;

        /// <summary>
        /// Логин пользователя
        /// </summary>
        public static string UserName
        {
            get
            {
                LogicalCallContextData lccd = LogicalCallContextData.GetContext();
                if (lccd == null)
                    return "Контекст не создан";

				return Session.Principal.Identity.Name;
            }
        }

        public static bool IsDeveloper
        {
            get
            {
                LogicalCallContextData lccd = LogicalCallContextData.GetContext();
                if (lccd == null)
                    throw new Exception("Контекст вызова не установлен.");
                else
                    return Convert.ToBoolean(lccd["IsDeveloper"]);
            }
        }

        /// <summary>
        /// Проверка блокировки сессии
        /// </summary>
		private static void CheckSessionState(Session session)
        {
            if (session.IsBlocked)
                throw new Exception("Сессия заблокирована.");
        }

        /// <summary>
        /// Возвращает текущую сессию
        /// </summary>
        public static Session Session
        {
            [System.Diagnostics.DebuggerStepThrough]
            get
            {
				Session session = (Session)SessionManager.Instance.Sessions[SessionId];
				CheckSessionState(session);
            	return session;
            }
        }

        /// <summary>
        /// Возвращает ID текущей сессии
        /// </summary>
        public static string SessionId
        {
            [System.Diagnostics.DebuggerStepThrough]
            get
            {
            	string sessionId = LogicalCallContextData.GetContext()["SessionID"] as String;
				if (String.IsNullOrEmpty(sessionId))
					throw new ServerException("В контексте вызова не установлен ID сессии.");
				return sessionId;
            }
        }

        /// <summary>
        /// Регистрация объекта с списке объектов занятых текущей сессией
        /// </summary>
        /// <param name="obj">Регистрируемый объект</param>
        public static void RegisterObject(DisposableObject obj)
        {
            if (LogicalCallContextData.GetContext() == null)
                return;

            CheckSessionState(Session);
            try
            {
                Session.Register(obj);
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
            }
        }

        /// <summary>
        /// Удаление объекта из списка объектов занятых текущей сессией
        /// </summary>
        /// <param name="obj">Освобождаемый объект</param>
        public static void UnregisterObject(DisposableObject obj)
        {
            if (LogicalCallContextData.GetContext() == null)
                return;

            try
            {
                Session.Unregister(obj);
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
            }
        }

        /// <summary>
        /// Сохраняет текстовое описание в списке выполненых действий сессии
        /// </summary>
        /// <param name="actionText">Текстовое описание выполненого действия</param>
        public static void PostAction(string actionText)
        {
            return;
            /*
            if (LogicalCallContextData.GetContext() == null)
                return;

            CheckSessionState();
            try
            {
                ((ISession)LogicalCallContextData.GetContext()["Session"]).PostAction(actionText);
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
            }
            */
        }

        /// <summary>
        /// Возвращает подключение к базе данных из пула подключений текущей сессии
        /// </summary>
        public static IDbConnection Connection
        {
            get { return Session.ConnectionPool.GetConnection(); }
        }

        /// <summary>
        /// Устанавливает системный контекст для текущей схемы с которой работает пользователь
        /// </summary>
        public static void SetSystemContext()
        {
            LogicalCallContextData.SetContext(systemContext);
        }

        /// <summary>
        /// Устанавливает системный контекст для текущей схемы с которой работает пользователь
        /// </summary>
        public static void SetSystemContext(LogicalCallContextData logicalCallContextData)
        {
            if (systemContext == null)
                systemContext = logicalCallContextData;
            else
                throw new ServerException("Системный контекст схемы уже установлен.");
        }

        public static List<IDisposable> SessionResources
        {
            get { return Session.Resources; }
        }
    }
}
