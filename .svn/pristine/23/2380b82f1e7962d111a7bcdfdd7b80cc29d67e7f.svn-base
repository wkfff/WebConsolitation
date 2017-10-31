using System;

namespace Krista.FM.ServerLibrary
{
    /// <summary>
    /// Тип сереализуемого объекта
    /// </summary>
    public enum ReturnType
    {
        /// <summary>
        /// Указываем в случае, когда свойство возращает значение
        /// </summary>
        Value,
        /// <summary>
        /// Указываем в случае, когда свойство возращает ссылку на объект
        /// </summary>
        Object,
        /// <summary>
        /// Используется для сериализации коллекции
        /// </summary>
        Dictionary
    }

    /// <summary>
    /// Определяет глубину сериализации объекта
    /// </summary>
    public enum LevelSerialization
    {
        /// <summary>
        /// Не сериализовать объект, передавать ссылку на объект сервера
        /// </summary>
        None,
        /// <summary>
        /// Сериализовать только сам объект
        /// </summary>
        Serialize,
        /// <summary>
        /// Применяется для коллекций, позволяя сериализовать также все ee объекты
        /// </summary>
        DeepSerialize
    }

    /// <summary>
    /// Атрибут для интерфейсов. Позволяет получить базовый интерфейс для объектов сервера, 
    /// который содержит атрибыты для сериализации свойств
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public class SMOInterfaceAttribute : Attribute
    {
    }

    /// <summary>
    /// Атрибут для свойств объекта. Отмеченные свойства десериализуются на клиенте 
    /// и обращение к ним уже идет ни к как к proxy 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class SMOSerializableAttribute : Attribute
    {
        /// <summary>
        /// Тип возвращаемого значения свойства
        /// </summary>
        protected ReturnType returnType;

        /// <summary>
        /// Глубина сериализации значения свойства
        /// </summary>
        protected LevelSerialization levelSerialization;

        public SMOSerializableAttribute(ReturnType returnType, LevelSerialization levelSerialization)
        {
            this.returnType = returnType;
            this.levelSerialization = levelSerialization;
        }

        public SMOSerializableAttribute(ReturnType returnType)
            : this(returnType, LevelSerialization.None)
        {
            this.returnType = returnType;
        }

        /// <summary>
        /// Тип возвращаемого значения свойства
        /// </summary>
        public ReturnType ReturnType
        {
            get { return returnType; }
            set { returnType = value; }
        }

        /// <summary>
        /// Глубина сериализации значения свойства
        /// </summary>
        public LevelSerialization LevelSerialization
        {
            get { return levelSerialization; }
            set { levelSerialization = value; }
        }
    }
}
