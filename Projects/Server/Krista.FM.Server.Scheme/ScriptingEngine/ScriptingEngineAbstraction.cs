using System;
using System.Collections.Generic;

using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.Scheme.ScriptingEngine
{
    /// <summary>
    /// Определяет абстрактный интерфейс для генерации скриптов объектов базы данных
    /// </summary>
    internal abstract class ScriptingEngineAbstraction
    {
        protected ScriptingEngineImpl _impl;

        internal ScriptingEngineAbstraction(ScriptingEngineImpl impl)
        {
            if (impl == null)
                throw new ArgumentNullException("impl");
            _impl = impl;
        }

        internal abstract List<string> CreateScript(ICommonDBObject obj);

        internal abstract List<string> DropScript(ICommonDBObject obj);

        internal abstract List<string> CreateDependentScripts(IEntity entity, IDataAttribute withoutAttribute);

        /// <summary>
        /// Определяет существует ли объект в базе данных
        /// </summary>
        /// <param name="objectName">Имя объекта</param>
        /// <param name="objectType">Тип объекта</param>
        /// <returns>true - если объект существует</returns>
        internal bool ExistsObject(string objectName, ObjectTypes objectType)
        {
            return _impl.ExistsObject(objectName, objectType);
        }

        /// <summary>
        /// Активизация триггеров объекта базы данных.
        /// </summary>
        /// <param name="objectName">Имя объекта базы данных.</param>
        /// <returns>Скрипт активизирующий триггера.</returns>
        internal List<string> EnableAllTriggersForObject(string objectName)
        {
            return _impl.EnableAllTriggersScript(objectName);
        }

        /// <summary>
        /// Деактивизация триггеров объекта базы данных.
        /// </summary>
        /// <param name="objectName">Имя объекта базы данных.</param>
        /// <returns>Скрипт деактивизирующий триггера.</returns>
        internal List<string> DisableAllTriggers(string objectName)
        {
            return _impl.DisableAllTriggersScript(objectName);
        }

        /// <summary>
        /// Возвращает префиксный символ для имен параметров SQL-запроса.
        /// </summary>
        /// <returns>Префиксный символ.</returns>
        internal string ParameterPrefixChar()
        {
            return _impl.ParameterPrefixChar();
        }
    }

    [Flags]
    internal enum DMLEventTypes
    {
        Insert = 1,
        Update = 2,
        Delete = 4
    }

    internal enum TriggerFireTypes
    {
        Before,
        After,
        InsteadOf
    }

    /// <summary>
    /// Поведение ассоциации при удалении родительского ключа
    /// </summary>
    internal enum OnDeleteAction
    {
        /// <summary>
        /// Запрет удаления, если есть порожденные записи
        /// </summary>
        None,

        /// <summary>
        /// Удаление порожденных записей
        /// </summary>
        Cascade,

        /// <summary>
        /// Установка в Null ссылок
        /// </summary>
        SetNull
    }
}
