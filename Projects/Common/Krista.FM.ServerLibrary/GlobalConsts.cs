using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.InteropServices;

namespace Krista.FM.ServerLibrary
{
    [ComVisible(true)]
    public enum GlobalConstCategories : int
    {
        Administration = 0,     // администрирование
        BudgetPlanning = 1,     // планирование бюджета
        RegionalSettings = 2    // региональные настройки
    }

    [ComVisible(true)]
    public enum GlobalConstsTypes : int
    {
        Configuration = 0,  // конфигурционные
        General = 1,        // обычные
        Custom = 2          // пользовательские
    }

    [ComVisible(true)]
    public interface IGlobalConst: IDisposable
    {
        int ID { get;}
        string Name { get; set;}
        string Caption { get; set;}
        string Description { get; set;}
        object Value { get; set;}
        DataAttributeTypes ConstValueType { get; set;}
        GlobalConstCategories ConstCactegory { get; set;}
        GlobalConstsTypes ConstType { get; set;}

    }

    [ComVisible(true)]
    public interface IGlobalConstsCollection : IDictionary<string, IGlobalConst>
    {
        /// <summary>
        /// получение данных по констатам для интерфейса пользователя
        /// </summary>
        /// <param name="constType"></param>
        /// <returns></returns>
        [ComVisible(false)]
        DataTable GetData(GlobalConstsTypes constType);

        /// <summary>
        /// получение данных по констатам для интерфейса пользователя
        /// </summary>
        /// <param name="constType"></param>
        /// <param name="data"></param>
        [ComVisible(false)]
        void GetData(GlobalConstsTypes constType, ref DataTable data);

        /// <summary>
        /// сохранение данных для интерфейса пользователя
        /// </summary>
        /// <param name="changes"></param>
        [ComVisible(false)]
        void ApplyChanges(ref DataTable changes);

        /// <summary>
        /// возвращает константу по имени
        /// </summary>
        /// <param name="constName"></param>
        /// <returns></returns>
        IGlobalConst ConstByName(string constName);

        /// <summary>
        /// добавляет константу в коллекцию
        /// </summary>
        /// <param name="constName"></param>
        IGlobalConst AddNew(string constName);

        /// <summary>
        /// добавление константы в коллекцию 
        /// используется для добавления записей через интерфейс пользователя
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="constName"></param>
        /// <returns></returns>
        [ComVisible(false)]
        IGlobalConst AddNew(int ID, string constName);

        /// <summary>
        /// получение нового значения генератора для ID
        /// </summary>
        /// <returns></returns>
        [ComVisible(false)]
        int GetGeneratorValue();

        /// <summary>
        /// сохранение данных
        /// </summary>
        void ApplyChanges();

        /// <summary>
        /// проверка, присутствует ли константа с таким русским наименованием (заголовком)
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        bool ContainCaption(string caption, ref int id);

        /// <summary>
        /// отмена всех внесенных изменений в коллекцию
        /// </summary>
        void CancelChanges();

    }

    [ComVisible(true)]
    public interface IGlobalConstsManager
    {
        IGlobalConstsCollection Consts { get; }
    }
}
