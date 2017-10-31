using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Principal;

namespace Krista.FM.ServerLibrary
{
    /// <summary>
    /// Типы клиентских сессий
    /// </summary>
    public enum SessionClientType
    {
        /// <summary>
        /// Неопределенное значение
        /// </summary>
        [Description("Неопределенное значение")]
        Undefined,
        /// <summary>
        /// Серверная сессия
        /// </summary>
        [Description("Сервер")]
        Server,

        /// <summary>
        /// Сессия закачки данных
        /// </summary>
        [Description("Закачка данных")]
        DataPump,

        /// <summary>
        /// Сессия Веб-сервиса
        /// </summary>
        [Description("Веб-сервис")]
        WebService,

        /// <summary>
        /// Windows Net managed клиенты (Workplace и пр.)
        /// </summary>
        [Description("Клиент .Net")]
        WindowsNetClient,

        /// <summary>
        /// Сессия RIA-интерфейса
        /// </summary>
        [Description("RIA-интерфейс")]
        RIA,

        /// <summary>
        /// Сессия сайта АЦР
        /// </summary>
        [Description("Сайт АЦР")]
        Dashboadrds
    }

    #region Server.Common

    /// <summary>
    /// Сессия
    /// </summary>
    public interface ISession : IDisposable
    {
        /// <summary>
        /// ID сессии
        /// </summary>
        string SessionId { get; }

        /// <summary>
        /// IPrincipal пользователя
        /// </summary>
        IPrincipal Principal { get; }

        /// <summary>
        /// Время подключения
        /// </summary>
        DateTime LogonTime { get; }

        /// <summary>
        /// Тип клиентской сессий
        /// </summary>
        SessionClientType ClientType { get; }

        /// <summary>
        /// Прилоложение из которого подключились
        /// </summary>
        string Application { get; }

        /// <summary>
        /// Имя машины с которой подключилось клиентское приложение
        /// </summary>
        string Host { get; }

        /// <summary>
        /// Признак заблокированности сессии
        /// </summary>
        bool IsBlocked { get; set; }

        /// <summary>
        /// Количество выделенных ресурсов
        /// </summary>
        int ResourcesCount { get; }

        /// <summary>
        /// Регистрация ресурса в контексте сессии
        /// </summary>
        /// <param name="obj">Регистрируемый объект</param>
        void Register(IDisposable obj);

        /// <summary>
        /// Удаление ресурса из контекста сессии
        /// </summary>
        /// <param name="obj">Удаляемый объект</param>
        void Unregister(IDisposable obj);

        /// <summary>
        /// Запись в историю выполненых действий
        /// </summary>
        /// <param name="actionText">Описание выполненого действия</param>
        void PostAction(string actionText);

        string ExecutedActions { get; }
    }

    /// <summary>
    /// Менеджер сессий
    /// </summary>
    public interface ISessionManager
    {
        /// <summary>
        /// Индексатор
        /// </summary>
        ISession this[string sessionId] { get; }

        /// <summary>
        /// Коллекция сессий
        /// </summary>
        IDictionary<string, ISession> Sessions { get; }

        void ClientSessionIsAlive(string sessionID);

        TimeSpan MaxClientResponseDelay { get; }
    }

    #endregion Server.Common

}