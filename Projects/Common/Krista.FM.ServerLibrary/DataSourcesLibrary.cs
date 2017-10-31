using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
//
// Интерфейсы для управления источниками данных и реестром закaчек
//
namespace Krista.FM.ServerLibrary
{
	/// <summary>
	/// Источник данных
	/// </summary>
	public interface IDataSource
	{
		/// <summary>
		/// Удаляет все данные закаченные по этому источнику
		/// </summary>
		void DeleteData();

		/// <summary>
		/// ID источника данных
		/// </summary>
		int ID { get; }

		/// <summary>
		/// Код поставщика данных
		/// </summary>
		string SupplierCode { get; set; }

		/// <summary>
		/// Порядковый номер поступивщей информации
		/// </summary>
		string DataCode { get; set; }

		/// <summary>
		/// Наименование поступающей имформации
		/// </summary>
		string DataName { get; set; }

		/// <summary>
		/// Вид параметров источника данных
		/// </summary>
        ParamKindTypes ParametersType { get; set; }

		/// <summary>
		/// Вид параметров: наименование бюджета
		/// </summary>
		string BudgetName { get; set; }

        /// <summary>
        /// Вид параметров: Территория
        /// </summary>
        string Territory { get; set; }

		/// <summary>
		/// Вид параметров: год
		/// </summary>
		int Year { get; set; }

		/// <summary>
		/// Вид параметров: месяц
		/// </summary>
		int Month { get; set; }

		/// <summary>
		/// Вид параметров: вариант
		/// </summary>
		string Variant { get; set; }

		/// <summary>
		/// Вид параметров: квартал
		/// </summary>
		int Quarter { get; set; }

	    /// <summary>
	    /// Блокировка(закрытие) источника.
	    /// </summary>
	    void LockDataSource();

	    /// <summary>
	    /// Открытие источника.
	    /// </summary>
	    void UnlockDataSource();

        /// <summary>
        /// Удаляет данные по источнику и ставит источнику признак удаления.
        /// </summary>
        /// <param name="dependedObjects">Таблица зависимых объектов.</param>
	    /// <returns>Результаты удаления.</returns>
	    DataTable RemoveWithData(DataTable dependedObjects);

        /// <summary>
        /// ищет в коллекции источников источник с таким же набором параметров, как текущий
        /// </summary>
        /// <returns></returns>
	    int? FindInDatabase();

        /// <summary>
        /// Сохраняет новый источник в базе и в коллекции
        /// </summary>
        /// <returns></returns>
	    int Save();

        /// <summary>
        /// Переводит источник в состояние "Утвержден"
        /// </summary>
	    void ConfirmDataSource();

        /// <summary>
        /// Переводит источник в состояние "Не проверен"
        /// </summary>
	    void UnConfirmDataSource();
	}


	/// <summary>
	/// Коллекция источников данных
	/// </summary>
	public interface IDataSourceCollection : IEnumerable 
	{
        /// <summary>
        /// Ищет источник данных по его параметрам
        /// </summary>
        /// <param name="obj">Объект источника</param>
        /// <returns>ИД источника (null - не найден)</returns>
        int? FindDataSource(object obj);

        /// <summary>
        /// Определяет содержит ли коллекция указанный ключ
        /// </summary>
        /// <param name="key">ID источника данных</param>
        /// <returns>true если содержит указанный ключ; иначе false</returns>
        bool Contains(int key);

		/// <summary>
		/// Определяет, содержит ли коллекция указанный источник (среди всех источников)
		/// </summary>
		/// <param name="obj">Объект источника</param>
		/// <returns>true - содержит</returns>
		bool Contains(object obj);

        /// <summary>
        /// Возвращает коллекцию ключей (ID источников)
        /// </summary>
        ICollection Keys { get; }

        /// <summary>
        /// Добавляет источник в базу данных
        /// </summary>
        /// <param name="value">DataSource источник данных</param>
		/// <returns>ИД источника</returns>
        int Add(Object value);

        /// <summary>
        /// Добавляет источник в базу данных с привязкой к истории
        /// </summary>
        /// <param name="value">DataSource источник данных</param>
        /// <param name="phe"></param>
        /// <returns>ИД источника</returns>
        int Add(Object value, IPumpHistoryElement phe);

        /// <summary>
        /// Удаляет источник из базы
        /// </summary>
        /// <param name="index">ИД источника</param>
		/// <returns>Строка ошибки</returns>
		string RemoveAt(int index);

		/// <summary>
		/// Создает элемент коллекции
		/// </summary>
		/// <returns>Созданный элемент</returns>
		IDataSource CreateElement();

        /// <summary>
        /// Количество источников в базе данных
        /// </summary>
		int Count { get; }

		/// <summary>
		/// Индексатор возвращает источник данных с указанным ключом,
		/// если ключа нет, то возвращает null
		/// </summary>
		IDataSource this[int key] { get; }
	}

    /// <summary>
    /// Метод получения информации
    /// </summary>
    public enum TakeMethodTypes
    {
        /// <summary>
        /// ИМПОРТ
        /// </summary>
        [Description("ИМПОРТ")]
        Import,

        /// <summary>
        /// Ввод
        /// </summary>
        [Description("ВВОД")]
        Input,

        /// <summary>
        /// Сбор (НЕ ИСПОЛЬЗОВАТЬ)
        /// </summary>
        [Description("СБОР (НЕ ИСПОЛЬЗОВАТЬ)")]
        Receipt

    }

    /// <summary>
    /// Вид параметров источника
    /// </summary>
    public enum ParamKindTypes : int
    {
        [Description("Не делится")]
        NoDivide = -1,
        
        [Description("Бюджет")]
        Budget = 0,
        
        [Description("Год")]
        Year = 1,
        
        [Description("Год месяц")]
        YearMonth = 2,
        
        [Description("Год месяц вариант")]
        YearMonthVariant = 3,
        
        [Description("Год вариант")]
        YearVariant = 4,
        
        [Description("Год квартал")]
        YearQuarter = 5,
        
        [Description("Год территория")]
        YearTerritory = 6,
        
        [Description("Год квартал месяц")]
        YearQuarterMonth = 7,
        
        [Description("Без параметров")]
        WithoutParams = 8,

        [Description("Вариант")]
        Variant = 9,

        [Description("Год месяц территория")]
        YearMonthTerritory = 10,

        [Description("Год квартал территория")]
        YearQuarterTerritory = 11,

        [Description("Год вариант месяц территория")]
        YearVariantMonthTerritory = 12,
    }

    /// <summary>
    /// Поставщик данных
    /// </summary>
    public interface IDataSupplier : IServerSideObject, ICloneable, IDisposable
    {
        string Name { get; set;}
        string Description { get; set; }
        IDataKindCollection DataKinds { get; }
    }

    /// <summary>
    /// Коллекция поставщиков данных
    /// </summary>
    public interface IDataSupplierCollection : IDictionaryBase<string, IDataSupplier>
    {
        IDataSupplier New();
        void Add(IDataSupplier dataSupplier);

        void EndEdit();
        void CancelEdit();
    }

    /// <summary>
    /// Вид поступающей информации
    /// </summary>
    public interface IDataKind : IServerSideObject, ICloneable
    {
        /// <summary>
        /// Поставщик данных.
        /// </summary>
        IDataSupplier Supplier { get; }

        /// <summary>
        /// Код поступающей информации
        /// </summary>
        string Code { get; set;}

        /// <summary>
        /// Наименование поступающей информации
        /// </summary>
        string Name { get; set;}

        /// <summary>
        /// Описание поступающей информации
        /// </summary>
        string Description { get; set;}

        /// <summary>
        /// Вид параметров поступающей информации
        /// </summary>
        ParamKindTypes ParamKind { get; set;}

        /// <summary>
        /// Метод получения данных
        /// </summary>
        TakeMethodTypes TakeMethod { get; set;}
    }


    /// <summary>
    /// Коллекция видов поступающей информации
    /// </summary>
    public interface IDataKindCollection : IDictionaryBase<string, IDataKind>
    {
        IDataKind New();
        void Add(IDataKind dataKind);
    }

    /// <summary>
    /// Менеджер источников, реестра и программ закачек
    /// </summary>
    public interface IDataSourceManager : IDisposable
    {
        /// <summary>
        /// Путь к каталогу с исходными данными
        /// </summary>
        string BaseDirectory { get; }

        /// <summary>
        /// Путь к каталогу с исходными данными
        /// </summary>
        string ArchiveDirectory { get; }

        /// <summary>
        /// Источники данных
        /// </summary>
        IDataSourceCollection DataSources { get; }

        /// <summary>
        /// Интерфейс для доступа к объектам схемы
        /// </summary>
        IScheme Scheme { get; }

        /// <summary>
        /// Коллекция поставщиков данных
        /// </summary>
        IDataSupplierCollection DataSuppliers { get; }

        /// <summary>
        /// получение данных по источникам
        /// </summary>
        /// <returns></returns>
        DataTable GetDataSourcesInfo();

        /// <summary>
        /// Получение данных по источникам
        /// </summary>
        /// <param name="dataSourceKinds">Набор видов поступающей информации, 
        /// чтобы на клиенте можно было задать только параметр источника</param>
        /// <returns></returns>
        DataTable GetDataSourcesInfo(string dataSourceKinds);

        /// <summary>
        /// Определяет описание и параметры источника данных
        /// </summary>
        /// <param name="SourceID">ID источника данных</param>
        /// <returns>Описание и параметры источника данных</returns>
        string GetDataSourceName(int SourceID);

        /// <summary>
        /// Возвращает список источников по которым сформирован объект
        /// </summary>
        /// <param name="tableName">имя представления в базе данных. Должно получаться из свойства ICommonObject.FullDBName</param>
        /// <returns>Key - ID источника данных; Value - описание источника</returns>
        Dictionary<int, string> GetDataSourcesNames(string tableName);

		/// <summary>
		/// Визвращает IDataUpdater доступный только для чтения для получения всех источников данных
		/// </summary>
		IDataUpdater DataSourcesDataUpdater { get; }
    }
}