using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Xml.Linq;

namespace Krista.FM.Update.Framework
{
    /// <summary>
    /// Состояние обновления
    /// </summary>
    [Serializable]
    public enum UpdateProcessState
    {
        /// <summary>
        /// Проверка на наличие обновлений НЕ произведена
        /// </summary>
        NotChecked,
        /// <summary>
        /// Проверка на наличие обновлений произведена
        /// </summary>
        Checked,
        /// <summary>
        /// Обновления подготовлены
        /// </summary>
        Prepared,
        /// <summary>
        /// Обновления успешно применены
        /// </summary>
        AppliedSuccessfully,
        /// <summary>
        /// Требуется откатить обновления
        /// </summary>
        RollbackRequired,
        /// <summary>
        /// Используется последняя версия
        /// </summary>
        LastVersion,
        /// <summary>
        /// Обновление с ошибками
        /// </summary>
        Error,
        /// <summary>
        /// Ожидание закрытия приложения
        /// </summary>
        WaitRestart,
        /// <summary>
        /// Обновление с предкпреждениями
        /// </summary>
        Warning
    }

    /// <summary>
    /// Клиентское приложение, использующее Framework
    /// </summary>
    public enum KristaApp
    {
        SchemeDesigner,
        Workplace,
        OlapAdmin,
        MDXExpert,
        OfficeAddIn,
        Updater
    }

    [ComVisible(false)]
    public interface IUpdateManager
    {
        /// <summary>
        /// Режим работы (серверный или клиентский)
        /// </summary>
        bool IsServerMode { get; }

        /// <summary>
        /// Обновляемое приложение
        /// </summary>
        KristaApp KristaApplication { get; set; }

        /// <summary>
        /// Список каналов обновления
        /// </summary>
        Dictionary<string, IUpdateFeed> Feeds { get; set; }

        /// <summary>
        /// Вызов процедуры обновления
        /// </summary>
        void StartUpdates();

        /// <summary>
        /// Вызов процедуры установки обновлений их клиентских приложений
        /// </summary>
        /// <returns></returns>
        bool ApplyUpdates();

        void Activate();

        /// <summary>
        /// Получает список установленных обновлений
        /// </summary>
        /// <returns></returns>
        IList<IUpdatePatch> GetPatchList();

        /// <summary>
        /// Получает спсок выкаченных обновлений (для службы автоматического обновления)
        /// </summary>
        /// <returns></returns>
        IList<IUpdatePatch> GetReceivedPatchList();

        /// <summary>
        /// Режим автоматического обновления
        /// </summary>
        bool AutoUpdateMode { get; set; }

        /// <summary>
        /// Добавляет клинт ав подписчики на события изменения состояния процесса обновления
        /// </summary>
        /// <param name="client"></param>
        void AttachClient(INotifierClient client);

        //void SetTasks(object task);
    }

    /// <summary>
    /// Обязательность патча
    /// </summary>
    public enum Use
    {
        /// <summary>
        /// Обязательный
        /// </summary>
        Required,
        /// <summary>
        /// Опциональный
        /// </summary>
        Optional,
        /// <summary>
        /// Запрещенный
        /// </summary>
        Prohibited
    }

    [ComVisible(false)]
    public interface IUpdatePatch
    {
        /// <summary>
        /// Имя патча
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// Описание
        /// </summary>
        string Description { get; set; }
        /// <summary>
        /// Детальное описание патча
        /// </summary>
        string DescriptionDetail { get; set; }
        /// <summary>
        /// Набор задач
        /// </summary>
        IList<IUpdateTask> Tasks { get; set; }
        /// <summary>
        /// Обязательность
        /// </summary>
        Use Use { get; set; }
        /// <summary>
        /// Каталог с обновлениями
        /// </summary>
        string BaseUrl { get; set; }
        /// <summary>
        /// Уникальный идентификатор патча
        /// </summary>
        string ObjectKey { get; set; }
        /// <summary>
        /// Условия выполнения патча
        /// </summary>
        IBooleanCondition UpdateConditions { get; set; }

        IUpdateFeed Feed { get; set; }

        XElement ToXml();
        ExecuteState Execute();
        void CheckForUpdates(IUpdateFeed updateFeed);
        bool Prepare(IUpdateSource source);
        bool RollbackAsync(Action<bool> callback);

        PrepareState IsPrepared { get; set; }
    }

    [ComVisible(false)]
    public interface IUpdateFeed
    {
        /// <summary>
        /// Уникальный идентификатор канала
        /// </summary>
        string ObjectKey { get; set; }
        /// <summary>
        /// Имя канала обновления
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// Каталог канала обновления
        /// </summary>
        string Url { get; set; }
        /// <summary>
        /// Все патчи в канале 
        /// </summary>
        IList<IUpdatePatch> Patches { get; set; }
        /// <summary>
        /// Патчи для установки
        /// </summary>
        IList<IUpdatePatch> UpdatesToApply { get; set; }

        bool IsBase { get; set; }

        void Save();
    }

    [ComVisible(false)]
    public interface IUpdateSource
    {
        /// <summary>
        /// Получить канал обновления
        /// </summary>
        string GetUpdatesFeed(string feedPath);
        /// <summary>
        /// Получить файл 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="basePath"></param>
        /// <param name="tempLocation"></param>
        /// <returns></returns>
        bool GetData(string filePath, string basePath, ref string tempLocation, bool needAddPostfix);
    }

    [ComVisible(false)]
    public interface IUpdateTask
    {
        IDictionary<string, string> Attributes { get; }
        string Description { get; set; }
        IBooleanCondition UpdateConditions { get; set; }

        bool Prepare(IUpdateSource source);
        ExecuteState Execute();
        bool Rollback();

        XElement ToXml();

        IUpdatePatch Owner { get; set; }

        /// <summary>
        /// Признак, по которому будем сортировать задачи при их выполнении
        /// </summary>
        int OrderByFactor { get; }
    }

    [ComVisible(false)]
    public interface IUpdateCondition
    {
        IDictionary<string, string> Attributes { get; }
        bool IsMet(IUpdateTask task);

        XElement ToXml();
    }

    [ComVisible(false)]
    public interface IBooleanCondition : IUpdateCondition
    {
        int ChildConditionsCount { get; }
        void AddCondition(IUpdateCondition cnd);
        void AddCondition(IUpdateCondition cnd, ConditionType type);
        IUpdateCondition Degrade();
    }

    public interface IConditionItem
    {
        IUpdateCondition _Condition { get; set; }
        ConditionType _ConditionType { get; set; }

    }

    [Flags]
    public enum ConditionType : byte
    {
        AND = 1,
        OR = 2,
        NOT = 4,
    }

   [ComVisible(false)]
    public interface IUpdateFeedReader
    {
        IList<IUpdatePatch> Read(string feed, IUpdateFeed updateFeed);
    }

   public enum ConnectionType
   {
       http,
       tcp,
       ftp
   }

    public enum PrepareState
    {
        PrepareSuccess,
        PrepareWithError,
        PrepareWithWarning
    }

    public enum ExecuteState
    {
        ExecuteSuccess,
        ExecuteWithError,
        ExecuteWithWarning
    }

    /// <summary>
    /// Интерфейс клиента
    /// </summary>
    public interface INotifierClient
    {
        /// <summary>
        /// Клиентское приложение получает новое состояние процесса обновления
        /// </summary>
        /// <param name="updateProcessState"></param>
        /// <returns></returns>
        object ReceiveNewState(UpdateProcessState updateProcessState);
    }
}
