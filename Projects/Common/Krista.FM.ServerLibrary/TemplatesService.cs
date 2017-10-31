using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.ComponentModel;

namespace Krista.FM.ServerLibrary.TemplatesService
{
	/// <summary>
	/// Виды шаблонов.
	/// </summary>
	public enum TemplateTypes
	{
		/// <summary>
		/// Шаблоны MDX-Эксперта и прочие шаблоны.
		/// </summary>
		General = 1,

		/// <summary>
		/// Отчеты системы (иточники финансирования и пр.)
		/// </summary>
		System = 2,
		
		/// <summary>
		/// Веб-отчеты (aspx).
		/// </summary>
		Web = 3,
		
		/// <summary>
		/// Веб-отчеты для Мобильных устройств (aspx).
		/// </summary>
		IPhone = 4
	}

	/// <summary>
	/// Типы документов.
	/// </summary>
    public enum TemplateDocumentTypes
    { 
		/// <summary>
		/// Группа отчетов.
		/// </summary>
        Group = 0,
		
		/// <summary>
		/// Отчет ворд.
		/// </summary>
		MSWord = 1,

		/// <summary>
		/// Отчет ексель.
		/// </summary>
		MSExcel = 2,
        
		/// <summary>
		/// Отчет мдх эксперт.
		/// </summary>
		MDXExpert = 3,
        
		/// <summary>
		/// Новый отчет эксперта.
		/// </summary>
		MDXExpert3 = 4,
        
		/// <summary>
		/// Лист планирования ворд.
		/// </summary>
		MSWordPlaning = 5,
        
		/// <summary>
		/// Лист планирования ексель.
		/// </summary>
		MSExcelPlaning = 6,
        
		/// <summary>
		/// Произвольный документ.
		/// </summary>
		Arbitrary = 7,
        
		/// <summary>
		/// Шаблон отчета ворд.
		/// </summary>
		MSWordTemplate = 8,
        
		/// <summary>
		/// Шаблон отчета ексель.
		/// </summary>
		MSExcelTemplate = 9,

		/// <summary>
		/// Шаблон отчета веб-сайта.
		/// </summary>
		WebReport = 10
    }

    /// <summary>
    /// Виды шаблонов для мобильных устройств
    /// </summary>
    public enum MobileTemplateTypes
    {
        /// <summary>
        /// Не определен (по умолчанию возмется у родительского элемента)
        /// </summary>
        None,

        /// <summary>
        /// Для iPad
        /// </summary>
        IPad,

        /// <summary>
        /// Для iPhone
        /// </summary>
        IPhone,

        /// <summary>
        /// Для Windows Mobile с разрешением 240х320
        /// </summary>
        WM240x320,

        /// <summary>
        /// Для Windows Mobile с разрешением 480х640 и 480х800
        /// </summary>
        WM480x640,

        /// <summary>
        /// Для Андройд
        /// </summary>
        Android,

        /// <summary>
        /// Для Windows Phone
        /// </summary>
        WindowsPhone
    }

    /// <summary>
    /// Интерфейс управления репозиторием шаблонов.
    /// </summary>
    public interface ITemplatesService : IDisposable
    {
        /// <summary>
        /// репозиторий эксперта
        /// </summary>
        ITemplatesRepository Repository { get; }
    }

    /// <summary>
    /// Интерфейс коллекции шаблонов
    /// </summary>
    public interface ITemplatesRepository : IDisposable
    {
		/// <summary>
		/// Получить доступные пользователю шаблоны заданного вида.
		/// </summary>
		DataTable GetTemplatesInfo(TemplateTypes templateType);

        /// <summary>
        /// Получает шаблоны заданного вида с возможностью динамической загрузки
        /// </summary>
        /// <param name="templateType">Вид шаблона</param>
        /// <param name="parentId">ID родительской записи</param>
        /// <returns></returns>
        IList<TemplateDTO> GetTemplatesInfo(TemplateTypes templateType, int? parentId);
		
		/// <summary>
		/// Возвращает IDataUpdater для применения изменений.
		/// </summary>
		IDataUpdater GetDataUpdater();

		/// <summary>
		/// Блокировка шаблона текущим пользователем.
		/// </summary>
		/// <param name="templateID">ID шаблона.</param>
		void LockTemplate(int templateID);

		/// <summary>
		/// Снятие блокировки шаблона текущим пользователем.
		/// </summary>
		/// <param name="templateID">ID шаблона.</param>
		void UnlockTemplate(int templateID);

		/// <summary>
        /// ID нового элемента репозитория.
        /// </summary>
        /// <returns></returns>
        int NewTemplateID();

    	/// <summary>
    	/// Проверяет прикреплен ли документ к шаблону.
    	/// </summary>
    	/// <param name="templateID">ID шаблона.</param>
    	bool ExistDocument(int templateID);

		/// <summary>
		/// Возвращает документ из базы данных.
		/// </summary>
		/// <param name="templateID">ID шаблона.</param>
		/// <returns></returns>
		byte[] GetDocument(int templateID);

		/// <summary>
		/// Сохраняет документ в базу данных.
		/// </summary>
		/// <param name="documentData">Данные документа.</param>
		/// <param name="templateID">ID шаблона.</param>
		void SetDocument(byte[] documentData, int templateID);

        /// <summary>
        /// Экспорт данных репозитория в поток
        /// </summary>
		void RepositoryExport(Stream exportStream, TemplateTypes templateType);

        /// <summary>
        /// Экспорт данных репозитория по отдельным записям
        /// </summary>
		void RepositoryExport(Stream exportStream, List<int> exportRowsIds, TemplateTypes templateType);

        /// <summary>
        /// Импорт данных из потока
        /// </summary>
		void RepositoryImport(Stream stream, TemplateTypes templateType);

        /// <summary>
        /// Импорт данных с установкой родительской записи для записей верхнего уровня
        /// </summary>
		void RepositoryImport(Stream stream, int parentId, TemplateTypes templateType);

        /// <summary>
        /// Получает имя территории по коду
        /// </summary>
        string GetTerritoryName(int code);

        /// <summary>
        /// Получает имя тематического раздела по коду
        /// </summary>
        string GetThemeSectionName(int code);
    }

	public sealed class TemplatesConstants
	{
		public const string ReportTemplates = "Шаблоны отчетов";
		public const string SystemTemplatesFinSources = "FinSources";
		public const string SystemTemplatesWebReports = "Веб-отчеты";
		public const string SystemTemplatesIPhone = "Отчеты для iProne";
	}

	/// <summary>
	/// Наименования полей в базе данных таблицы Templates.
	/// </summary>
	public static class TemplateFields
	{
		public const string ID = "ID";
		public const string Code = "Code";
		public const string Name = "Name";
		public const string Description = "Description";
		public const string Type = "Type";
		public const string Editor = "Editor";
		public const string Document = "Document";
		public const string DocumentFileName = "DocumentFileName";
		public const string RefTemplatesTypes = "RefTemplatesTypes";
		public const string ParentID = "ParentID";
		public const string IsVisible = "IsVisible";
		public const string SortIndex = "SortIndex";
		public const string Flags = "Flags";
		public const string LastEditData = "LastEditData";
	}

	/// <summary>
	/// Флажки шаблона.
	/// </summary>
	[Flags]
	public enum TemplateFlags
	{
		/// <summary>
		/// Журнал "Бюджет".
		/// </summary>
		[Description("Журнал \"Бюджет\"")]
		Favorite = 1,

		/// <summary>
		/// Важный.
		/// </summary>
		[Description("Важный")]
		Important = 2,

		/// <summary>
		/// Новый. Важный
		/// </summary>
		[Description("Новый")]
		New = 4,

		/// <summary>
		/// Все флажки.
		/// </summary>
		All = Favorite | Important | New
	}

	/// <summary>
	/// Структура описывает расширенные свойства шаблона для мобильных устройств
	/// </summary>
	[Serializable]
	public struct IPhoteTemplateDescriptor
	{
		/// <summary>
		/// Субъекто зависымый отчет.
		/// </summary>
		public bool SubjectDepended;

        /// <summary>
        /// Не скроллируемый отчет
        /// </summary>
        public bool IsNotScrollable;
		
		/// <summary>
		/// Дата последнего развертывания отчета на сервере.
		/// </summary>
		public DateTime LastDeployDate;

        /// <summary>
        /// Тип шаблона
        /// </summary>
        public MobileTemplateTypes TemplateType;

        /// <summary>
        /// Массив байт с изображением
        /// </summary>
        public byte[] IconByte;
        
        /// <summary>
        /// ID дискуссии в форуме, соответвующий данному разделу
        /// </summary>
        public int ForumDiscussionID;

        /// <summary>
        /// Тип территории
        /// </summary>
	    public string TerritoryRF;

        /// <summary>
        /// Тематический раздел
        /// </summary>
	    public string ThemeSection;
	}

    /// <summary>
    /// Представление для работы на клиенте
    /// </summary>
    [Serializable]
    public class TemplateDTO
    {
        public int? ID { get; set; }
        public int? ParentID { get; set; }
        public int Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string DocumentFileName { get; set; }
        public int? Editor { get; set; }
        public string Code { get; set; }
        public int? SortIndex { get; set; }
        public int? Flags { get; set; }
        public int RefTemplatesTypes { get; set; }
        public string TerritoryRF { get; set; }
        public string ThemeSection { get; set; }
        /// <summary>
        /// Субъекто зависымый отчет.
        /// </summary>
        public bool SubjectDepended;
        /// <summary>
        /// Не скроллируемый отчет
        /// </summary>
        public bool IsNotScrollable;
        /// <summary>
        /// Дата последнего развертывания отчета на сервере.
        /// </summary>
        public DateTime LastDeployDate;
        /// <summary>
        /// Тип шаблона
        /// </summary>
        public MobileTemplateTypes TemplateType;
        /// <summary>
        /// Массив байт с изображением
        /// </summary>
        public byte[] IconByte;
        /// <summary>
        /// ID дискуссии в форуме, соответвующий данному разделу
        /// </summary>
        public int ForumDiscussionID;
    }

    [Serializable]
    public class TemplatesTypeDTO
    {
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
    }
}
