using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Krista.FM.ServerLibrary
{
    #region Сообщения

    public interface IMessageManager
    {
        #region Методы создания нового сообщения

        SendMessageStatus SendMessage(MessageWrapper messageWrapper);

        #endregion

        /// <summary>
        /// Получает все сообщения адрессованные конкретному пользователю
        /// </summary>
        /// <param name="userId"> Пользователь</param>
        /// <returns> Список сообщений</returns>
        IList<MessageDTO> ReceiveMessages(int userId);

        /// <summary>
        /// Получает все сообщения адрессованные конкретному пользователю c пейджингом
        /// </summary>
        /// <param name="userId"> Пользователь</param>
        /// <param name="page"> № страницы</param>
        /// <param name="itemPerRage"> Количество сообщений на странице</param>
        /// <returns> Сообщения</returns>
        IList<MessageDTO> ReceiveMessages(int userId, int page, int itemPerRage);

        /// <summary>
        /// Обновляет статус сообщения
        /// </summary>
        /// <param name="messageId"> Идентификатор сообщения</param>
        /// <param name="status"> Новый статус</param>
        void UpdateMessage(int messageId, MessageStatus status);

        /// <summary>
        /// Удаление сообщения (не приводит к полному удалению
        /// сообщения из БД, а только помечает его как "удаленное".
        /// Полное удаление осуществляется сборщиком неактуальных сообщений по расписанию.
        /// </summary>
        /// <param name="messageId"></param>
        void DeleteMessage(int messageId);

        /// <summary>
        /// Получает вложение для сообщения
        /// </summary>
        MessageAttachmentDTO GetMessageAttachment(int messageId);

        /// <summary>
        /// Возвращает количество непрочитанных сообщений
        /// </summary>
        /// <param name="userId"> Пользователь</param>
        /// <returns></returns>
        int GetNewMessageCount(int userId);

        /// <summary>
        /// Возвращает количество всех сообщений
        /// </summary>
        /// <param name="userId"> Пользователь</param>
        /// <returns></returns>
        int GetMessageCount(int userId);
    }

    public enum MessageStatus
    {
        [Description("Новое сообщение")]
        New = 1,
        [Description("Сообщение прочитано")]
        Read = 2,
        [Description("Сообщение удалено")]
        Deleted = 3
    }

    public enum MessageImportance
    {
        [Description("Высокая важность")]
        HighImportance = 1,
        [Description("Важное")]
        Importance = 2,
        [Description("Обычное")]
        Regular = 3,
        [Description("Неважное")]
        Unimportant = 4
    }

    #region Типы сообщений

    public enum MessageType
    {
        [MessageTypeObjectKey("3133843A-10EE-424F-A4F1-80F403384CC6")]
        [Description("Сообщения от администратора системы")]
        [LiveTime(14)]
        AdministratorMessage = 1,

        [MessageTypeObjectKey("8521ECFC-CC8D-4E6E-9F1A-6A72C2747DBF")]
        [Description("Сообщения от интерфейса расчета кубов")]
        [LiveTime(14)]
        CubesManagerMessage = 2,

        [MessageTypeObjectKey("8334D1B9-06E4-4E08-B567-51F4DE8419FD")]
        [Description("Сообщения от интерфейса сопоставления классификаторов")]
        [LiveTime(14)]
        BridgeClassifiersMessage = 3,

        [MessageTypeObjectKey("516967AD-E99C-4086-95FA-35E1845F8836")]
        [Description("Сообщения от подсистемы задач")]
        [LiveTime(14)]
        TaskMessage = 4,

        #region сообщения от подсистемы закачек

        [MessageTypeObjectKey("B8C9D687-0C48-47D7-B81A-E5B60E195A95")]
        [Description("Сообщения от подсистемы закачек - этап закачки")]
        [LiveTime(14)]
        PumpMessage = 5, 

        [MessageTypeObjectKey("E864329A-7301-432D-92F3-9DF9BF592847")]
        [Description("Сообщения от подсистемы закачек - этап обработки")]
        [LiveTime(14)]
        PumpProcessMessage = 6,

        [MessageTypeObjectKey("67279B98-3C98-4523-B149-9B76310A5C96")]
        [Description("Сообщения от подсистемы закачек - этап сопоставления")]
        [LiveTime(14)]
        PumpAssociateMessage = 7,

        [MessageTypeObjectKey("8785A233-59B1-4FC7-B87C-51DFE77BFF1E")]
        [Description("Сообщения от подсистемы закачек - этап расчета кубов")]
        [LiveTime(14)]
        PumpProcessCubesMessage = 8,

        [MessageTypeObjectKey("3555C8BB-0736-4B1F-8E4D-DEA108235CD6")]
        [Description("Сообщения от подсистемы закачек - этап проверки данных")]
        [LiveTime(14)]
        PumpCheckDataMessage = 9,

        #endregion сообщения от подсистемы закачек

        [MessageTypeObjectKey("ED4B141A-6C70-4989-9ED2-BC494B467E1D")]
        [Description("Сообщения от блока прогнозирование")]
        [LiveTime(14)]
        ForecastMessage = 10,

        [Description("Сообщение от форм сбора")]
        [LiveTime(14)]
        ConsolidationMessage = 11,

        /// <summary>
        /// Сообщение целевой группе, не требует подписки на рассылку
        /// </summary>
        [LiveTime(14)]
        [Description("Сообщение от RIA-интерфейса")]
        TargetRiaMessage = 12
    }

    public class MessageTypeObjectKey : Attribute
    {
        public string ObjectKey { get; set; }

        public MessageTypeObjectKey(string objectKey)
        {
            ObjectKey = objectKey;
        }
    }

    /// <summary>
    /// Актуальность сообщения в днях. Для каждого типа сообщения можно указать свое время жизни
    /// </summary>
    public class LiveTimeAttribute : Attribute
    {
        private readonly int days;

        public LiveTimeAttribute(int days)
        {
            this.days = days;
        }

        public int Days
        {
            get { return days; }
        }
    }

    #endregion

    /// <summary>
    /// Состаяния отправки сообщения. Значения подобраны так, чтобы
    /// 1: S & F = F
    /// 2: S & W = W
    /// 3: F & W = F
    /// 4: S & S = S
    /// 5: W & W = W
    /// 6: F & F = F
    /// </summary>
    [Serializable]
    [Flags]
    public enum SendMessageStatus
    {
        /// <summary>
        /// Успешно
        /// </summary>
        Success = 187,     // 10111011

        /// <summary>
        /// С ошибками
        /// </summary>
        Failure = 32,      // 00100000     

        /// <summary>
        /// Успешно с предупреждениями
        /// </summary>
        Warning = 171      // 10101011
    }

    /// <summary>
    /// Объект для переноса информации о сообщение на клиента
    /// </summary>
    [Serializable]
    public class MessageDTO
    {
        public int ID { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public MessageStatus MessageStatus { get; set; }
        public MessageImportance MessageImportance { get; set; }
        public MessageType MessageType { get; set; }
        public string RefUserSender { get; set; }
        public int? RefMessageAttachment { get; set; }
        public DateTime ReceivedDate { get; set; }
        public string TransferLink { get; set; }
    }

    /// <summary>
    /// Объект для переноса информации о вложении к сообщению на клиента
    /// </summary>
    [Serializable]
    public class MessageAttachmentDTO
    {
        public byte[] Document { get; set; }
        public string DocumentName { get; set; }
        public string DocumentFileName { get; set; }
    }

    /// <summary>
    /// Объект для переноса информации о пользователе на клиента
    /// </summary>
    public class UsersDTO
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }

    #endregion
    
    [Serializable]
    public class MessageWrapper
    {
        public MessageWrapper()
        {
            // Default value
            DateTimeOfCreation = DateTime.Now;
            DateTimeOfActual = DateTime.MinValue;
            MessageStatus = MessageStatus.New;
            MessageImportance = MessageImportance.Regular;
            MessageType = MessageType.AdministratorMessage;
        }

        public string Subject { get; set; }
        public string Body { get; set; }
        public DateTime DateTimeOfCreation { get; set; }
        public DateTime DateTimeOfActual { get; set; }
        public MessageType MessageType { get; set; }
        public MessageStatus MessageStatus { get; set; }
        public MessageImportance MessageImportance { get; set; }
        public int? RefUserSender { get; set; }
        public int? RefUserRecipient { get; set; }
        public int? RefGroupRecipient { get; set; }
        public bool SendAll { get; set; }
        public MessageAttachmentWrapper RefMessageAttachment { get; set; }
        public string TransferLink { get; set; }
    }

    [Serializable]
    public class MessageAttachmentWrapper
    {
        // Default value
        public MessageAttachmentWrapper()
        {
            DocumentName = "DocumentName";
        }

        public string DocumentName { get; set; }
        public string DocumentFileName { get; set; }
        public byte[] Document { get; set; }
    }

    /// <summary>
    /// Интерфейс менеджера очистки неактуальных сообщений
    /// </summary>
    public interface IMessageCleanerManager
    {
        void Start();
        void Stop();
    }
}
