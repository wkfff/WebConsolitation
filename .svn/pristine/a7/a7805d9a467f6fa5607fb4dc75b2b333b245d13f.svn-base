using System;
using System.Collections.Generic;

namespace Krista.FM.ServerLibrary
{
    /// <summary>
    /// Типы модификаций.
    /// </summary>
    public enum ModificationTypes : int
    {
        /// <summary>
        /// Создание нового объекта.
        /// </summary>
        Create = 0,

        /// <summary>
        /// Изменение объекта.
        /// </summary>
        Modify = 1,

        /// <summary>
        /// Удаление существующего объекта.
        /// </summary>
        Remove = 2,

        /// <summary>
        /// Неприменимая операция модификации.
        /// </summary>
        Inapplicable = 3
    }

    /// <summary>
    /// Состояние операции модификации.
    /// </summary>
    public enum ModificationStates
    {
        NotApplied,
        Applied,
        AppliedWithErrors,
        AppliedPartially
    }

    public delegate void ModificationMessageEventHandler(object sender, ModificationMessageEventArgs e);

    /// <summary>
    /// Тип события модификации.
    /// </summary>
    public enum ModificationEventTypes
    {
        StartOperation,
        EndOperation
    }

    /// <summary>
    /// Данные события модификации.
    /// </summary>
    [Serializable]
    public class ModificationMessageEventArgs : EventArgs
    {
        private string message;
        private IModificationItem item;
        private ModificationEventTypes type;
        private int indentLevel;

        /// <summary>
        /// Инициализация экземпляра класса.
        /// </summary>
        /// <param name="message">Текст сообщения.</param>
        public ModificationMessageEventArgs(string message)
        {
            this.message = message;
        }

        /// <summary>
        /// Инициализация экземпляра класса.
        /// </summary>
        /// <param name="message">Текст сообщения.</param>
        /// <param name="item">Операция модификации структуры.</param>
        /// <param name="type">Тип операции модификации.</param>
        /// <param name="indentLevel">Уровень вложенности операции.</param>
        public ModificationMessageEventArgs(string message, IModificationItem item, ModificationEventTypes type, int indentLevel)
            : this(message)
        {
            this.item = item;
            this.type = type;
            this.indentLevel = indentLevel;
        }

        /// <summary>
        /// Информационное сообщение.
        /// </summary>
        public string Message
        {
            get { return message; }
        }

        /// <summary>
        /// Операция модификации структуры.
        /// </summary>
        public IModificationItem Item
        {
            get { return item; }
        }

        /// <summary>
        /// Тип события модификации.
        /// </summary>
        public ModificationEventTypes Type
        {
            get { return type; }
        }

        /// <summary>
        /// Уровень вложенности операции.
        /// </summary>
        public int IndentLevel
        {
            get { return indentLevel; }
        }
    }

    /// <summary>
    /// Контекст в котором выполняются изменения структуры схемы.
    /// </summary>
    public interface IModificationContext : IDisposable
    {
        /// <summary>
        /// Сигнализирует о начале процесса обновления.
        /// </summary>
        /// <returns>ID записи протокола "Операции пользователя".</returns>
        int BeginUpdate();
        
        /// <summary>
        /// Сигнализирует о начале процесса обновления.
        /// </summary>
        void EndUpdate();

        event ModificationMessageEventHandler OnModificationMessage;
    }

    /// <summary>
    /// Операция модификации структуры.
    /// </summary>
    public interface IModificationItem : IDisposable
    {
        /// <summary>
        /// Дочерние операции модификации.
        /// </summary>
        Dictionary<string, IModificationItem> Items { get; }

        /// <summary>
        /// Уникальный ключ в дереве операций.
        /// </summary>
        string Key { get; }

        /// <summary>
        /// Имя операции модификации отобрадаемое в интерфейсе.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Тип операции модификации.
        /// </summary>
        ModificationTypes Type { get; }

        /// <summary>
        /// Состояние операции модификации.
        /// </summary>
        ModificationStates State { get; }

        /// <summary>
        /// Исключение возникшее при выполнении операции.
        /// </summary>
        Exception Exception { get; }

        /// <summary>
        /// ImageIndex
        /// </summary>
        int ImageIndex { get; }

        /// <summary>
        /// Применение изменения.
        /// </summary>
        /// <param name="context">Контекст в котором выполняются изменения структуры схемы.</param>
        /// <param name="isAppliedPartially">Признак частично-успешного применения изменений</param>
        void Applay(IModificationContext context, out bool isAppliedPartially);
    }

    /// <summary>
    /// Определяет методы для получения и применения изменений
    /// </summary>
    public interface IModifiable
    {
        /// <summary>
        /// Формирует список отличий (операций изменения) текущего объекта от toObject
        /// </summary>
        /// <param name="toObject">Объект с которым будет производиться сравнение</param>
        /// <returns>список отличий (операций изменения)</returns>
        IModificationItem GetChanges(IModifiable toObject);
    }

    /// <summary>
    /// Определяет методы для получения и применения изменений
    /// </summary>
    public interface IMinorModifiable : IModifiable
    {
        /// <summary>
        /// Применение изменений. Приводит текущий объект к виду объекта toObject
        /// </summary>
        /// <param name="toObject">Объект к виду которого будет приведен текущий объект</param>
        //void Update(IModifiable toObject);
    }

    /// <summary>
    /// Определяет методы для получения и применения изменений
    /// </summary>
    public interface IMajorModifiable : IMinorModifiable
    {
        IModificationItem GetChanges();
    }

    /// <summary>
    /// Изменяемая коллекция объектов.
    /// </summary>
    /// <typeparam name="TKey">Уникалькый ключ.</typeparam>
    /// <typeparam name="TValue">Значение.</typeparam>
    public interface IModifiableCollection<TKey, TValue> : IDictionaryBase<TKey, TValue>, IDisposable
    {
        /// <summary>
        /// Создает новый элемент не добавляя в коллекцию.
        /// </summary>
        /// <returns>Созданный объект.</returns>
        TValue CreateItem();
    }
}
