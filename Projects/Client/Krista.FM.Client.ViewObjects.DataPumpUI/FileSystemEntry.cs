namespace Krista.FM.Client.ViewObjects.DataPumpUI
{
	/// <summary>
	/// Класс предоставляющий информацию о каталоге или файле
	/// </summary>
	public class FileSystemEntry
	{
		#region Поля

		private FileSystemEntryType					fileSystemEntryType = FileSystemEntryType.File;
		private string								fullyQualifiedName = "";

        #endregion Поля


        #region Инициализация

        /// <summary>
		/// Конструктор
		/// </summary>
		/// <param name="fileSystemEntryType">Файл или каталог</param>
		/// <param name="fullyQualifiedName">Полный путь</param>
		public FileSystemEntry(FileSystemEntryType fileSystemEntryType, string fullyQualifiedName)
		{
			this.fileSystemEntryType	= fileSystemEntryType;
			this.fullyQualifiedName		= fullyQualifiedName;
        }

        #endregion Инициализация


        #region Методы класса

        /// <summary>
		/// Имя каталога или файла
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return Name;
        }

        #endregion Методы класса


        #region Свойства класса

        /// <summary>
		/// Тип сущности: файл или каталог
		/// </summary>
		public FileSystemEntryType EntryType
		{
			get	{ return fileSystemEntryType; }
		}

		/// <summary>
		/// Полное имя
		/// </summary>
		public string FullName
		{
			get	{ return fullyQualifiedName; }
		}

		/// <summary>
		/// Имя
		/// </summary>
		public string Name
		{
			get	{ return FileManager.RemovePath(fullyQualifiedName); }
        }

        #endregion Свойства класса
    }


	/// <summary>
	/// Тип сущности файловой системы
	/// </summary>
	public enum FileSystemEntryType
	{
        /// <summary>
        /// Файл
        /// </summary>
		File,

        /// <summary>
        /// Каталог
        /// </summary>
		Directory
	}
}
