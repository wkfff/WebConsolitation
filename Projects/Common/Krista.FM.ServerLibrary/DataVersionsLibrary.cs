using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

// Интерфейсы для управления версиями классификаторов

namespace Krista.FM.ServerLibrary
{
    /// <summary>
    /// Менеджер версий классификаторов
    /// </summary>
    public interface IDataVersionManager
    {
        /// <summary>
        /// Интерфейс для доступа к объектам схемы
        /// </summary>
        IScheme Scheme { get; }
        /// <summary>
        /// Коллекция версий классификатора
        /// </summary>
        IDataVersionsCollection DataVersions { get; }
        /// <summary>
        /// Заполнение таблицы версии по имеющимся данным
        /// </summary>
        void FillObjectVersionTable();
    }

    /// <summary>
    /// Коллекция версий классификаторов
    /// </summary>
    public interface IDataVersionsCollection : IEnumerable
    {
        /// <summary>
        /// Добавляет версию классификатора в базу данных
        /// </summary>
        /// <param name="value">DataVersion версия классификатора</param>
        /// <returns>ИД версии</returns>
        int Add(Object value);
        /// <summary>
        /// Проверяет на наличие версии в коллекции
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        bool Containts(Object value);
        /// <summary>
        /// Поиск версии классификатора
        /// </summary>
        /// <param name="value"></param>
        /// <returns>ИД версии, null - не нашли</returns>
        int? FindDataVersion(Object value);

        int? FindCurrentVersion(string objectKey);
        /// <summary>
        /// Создание новой версии
        /// </summary>
        /// <returns></returns>
        IDataVersion Create();
        /// <summary>
        /// Индексатор, возвращает версию классификатора по идентификатору объекта и по источнику
        /// </summary>
        /// <param name="objectKey"></param>
        /// <param name="sourceID"></param>
        /// <returns></returns>
        IDataVersion this[string objectKey, int sourceID] { get; }
        /// <summary>
        /// Дополнительная процедура при удалении версии структуры классификатора
        /// Для версий в поле представления указываем значение по умолчанию
        /// </summary>
        /// <param name="key"></param>
        void RemovePresentation(string key);
        /// <summary>
        /// Дополнительная процедура при удалении источника данных.
        /// Удаляем все версии, где используется этот источник
        /// </summary>
        /// <param name="SourceID"></param>
        void RemoveSource(int SourceID);
        /// <summary>
        /// Удаление всех версий для классификатора
        /// </summary>
        /// <param name="objectKey"></param>
        void RemoveObject(string objectKey);
        /// <summary>
        /// Обновление версии
        /// </summary>
        void UpdatePresentation(string presentationKey, string sourceID, string activeObjectKey, bool isCurrent);
    }

    /// <summary>
    /// Версия классификатора
    /// </summary>
    public interface IDataVersion
    {
        /// <summary>
        /// ID
        /// </summary>
        int ID { get; set; }
        /// <summary>
        /// Уникальный идентификатор объекта, для которого создана версия
        /// </summary>
        string ObjectKey { get; set; }
        /// <summary>
        /// ID версии структуры
        /// </summary>
        string PresentationKey { get; set;}
        /// <summary>
        /// ID источника данных. Для объекта по одному источнику создаем только одну версию
        /// </summary>
        int SourceID { get; set; }
        /// <summary>
        /// Имя версии классификатора
        /// </summary>
        string Name { get; set;}
        /// <summary>
        /// Признак текущей версии классификатора
        /// </summary>
        bool IsCurrent { get; set; }
    }
}
