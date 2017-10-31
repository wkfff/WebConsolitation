using System;
using System.Data;
using System.IO;
using System.Runtime.CompilerServices;

using Krista.FM.Common;
using Krista.FM.Server.Common;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.DataPumpManagement
{
	/// <summary>
	/// Элемент реестра закачек.
	/// </summary>
    public class PumpRegistryElement : DisposableObject, IPumpRegistryElement
    {
        #region Поля

        private int _ID;
		private string supplierCode;
		private string dataCode;
		private string programIdentifier;
		private string programConfig = string.Empty;
		private string description;
		private string name;
		private IScheme scheme;
        private string stagesParameters;
        private string schedule;
        private IPumpHistoryCollection pumpHistoryCollection;
        private StagesQueue stagesQueue;
        private IDataPumpModule dataPumpModule;
        private string pumpProgram = String.Empty;

        #endregion Поля


        #region Инициализация

        /// <summary>
		/// Конструктор объекта
		/// </summary>
        public PumpRegistryElement(IScheme scheme)
		{
            this.scheme = scheme;

			name = string.Empty;
			programConfig = string.Empty;
		}

        /// <summary>
        /// Деструктор
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (stagesQueue != null) stagesQueue.Dispose();
            }
            
            base.Dispose(disposing);
        }

        #endregion Инициализация


        #region Общие методы

        /// <summary>
        /// Выполняет действия по инициализации
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual void Initialize()
        {
            if (stagesQueue != null)
            {
                stagesQueue.Dispose();
            }

            stagesQueue = new StagesQueue(scheme, this);
        }

        /// <summary>
        /// Возвращает корневой каталог источника данных (local path).
        /// </summary>
        /// <returns>Объект каталога</returns>
        private DirectoryInfo GetDataSourceDir(string root)
        {
            string str = this.DataCode.PadLeft(4, '0');
            if (this.scheme.DataSourceManager.DataSuppliers.ContainsKey(this.SupplierCode))
            {
                if (this.scheme.DataSourceManager.DataSuppliers[this.SupplierCode].DataKinds.ContainsKey(str))
                {
                    str = string.Format("{0}_{1}", str, this.scheme.DataSourceManager.DataSuppliers[this.SupplierCode].DataKinds[str].Name);
                }
            }
            str = string.Format("{0}\\{1}\\{2}", root, this.SupplierCode, str);

            return new DirectoryInfo(str);
        }

        /// <summary>
        /// Возвращает корневой каталог источника данных (local path).
        /// </summary>
        /// <returns>Объект каталога</returns>
        private DirectoryInfo GetDataSourceRootDirLocal()
        {
            return GetDataSourceDir(this.scheme.DataSourceManager.BaseDirectory);
        }

        /// <summary>
        /// Возвращает корневой каталог источника данных (UNC path).
        /// </summary>
        /// <returns>Объект каталога</returns>
        private DirectoryInfo GetDataSourceRootDirUNC()
        {
            return GetDataSourceDir(this.scheme.DataSourceDirectory);
        }

        #endregion Общие методы


        #region Свойства класса

        /// <summary>
        /// Интерфейс закачки, соответствующей записи реестра
        /// </summary>
        public IDataPumpModule DataPumpModule
        {
            get
            {
                return dataPumpModule;
            }
            set
            {
                dataPumpModule = value;
            }
        }

        #endregion Свойства класса


        #region Реализация IPumpRegistryElement


        #region Реализация методов

        /// <summary>
		/// Сохраняет измененные свойства в базе данных
		/// </summary>
		[MethodImpl (MethodImplOptions.Synchronized)]
		public void Update()
		{
			// получаем объект подключения к базе данных
			IDatabase db = scheme.SchemeDWH.DB;
			try
			{
				db.ExecQuery(
					"UPDATE PUMPREGISTRY SET ProgramConfig = ? WHERE ID = ?",
					QueryResultTypes.NonQuery,
					db.CreateParameter("ProgramConfig", programConfig, DbType.AnsiString),
					db.CreateParameter("ID", _ID, DbType.Int64));
			}
			finally
			{
				db.Dispose();
			}
		}

		/// <summary>
		/// Возвращает значения всех измененных свойств в прежние значения
		/// </summary>
		[MethodImpl (MethodImplOptions.Synchronized)]
		public void Revert()
		{
			// получаем объект подключения к базе данных
			IDatabase db = scheme.SchemeDWH.DB;
			try
			{
				DataTable dt = (DataTable)db.ExecQuery(
					"select SupplierCode, DataCode, ProgramIdentifier, ProgramConfig, Comments, Name from PumpRegistry " +
					"where ID = ?",
					QueryResultTypes.DataTable,
					db.CreateParameter("ID", _ID));
				if (dt.Rows.Count > 0)
				{
					supplierCode = Convert.ToString(dt.Rows[0][0]);
					dataCode = Convert.ToString(dt.Rows[0][1]);
					ProgramIdentifier = Convert.ToString(dt.Rows[0][2]);
					programConfig = Convert.ToString(dt.Rows[0][3]);
					description = Convert.ToString(dt.Rows[0][4]);
					name = Convert.ToString(dt.Rows[0][5]);
				}
			}
			finally
			{
				db.Dispose();
			}
		}

		/// <summary>
		/// Запрашивает из базы историю закачки по этому элементу. Вызывать только после инициализации полей
		/// </summary>
		public void RequestPumpHistory()
		{
			this.pumpHistoryCollection = new PumpHistoryCollection(scheme, _ID);
		}
	
		#endregion Реализация методов


		#region Реализация свойств

		/// <summary>
		/// ID записи элемента реестра закачек в базе данных
		/// </summary>
		public int ID
		{
			get	
            { 
                return _ID; 
            }
			set 
            { 
                _ID = value; 
            }
		}

		/// <summary>
		/// Код поставщика. Текстовый идентификатор 8 символов
		/// </summary>
		public string SupplierCode
		{
			get 
            { 
                return supplierCode; 
            }
			set 
			{ 
				// Проверка на корректность устанавливаемого значения:
				// должно содержать русский строчный восьмибуквенный идентификатор
				//if (value.Length != 8) throw new Exception("Некорректный формат кода поставщика.");
				supplierCode = value.ToString(); 
			}
		}

		/// <summary>
		/// Порядковый номер поступающей информации, число 4 знака
		/// </summary>
		public string DataCode
		{
			get 
            { 
                return dataCode; 
            }
			set 
			{ 
				// Проверка на корректность устанавливаемого значения:
				// должно содержать четырехзначное число
				//if (value.Length != 4) throw new Exception("Некорректный формат номера поступающей информации.");
				dataCode = value; 
			}
		}

		/// <summary>
		/// Идентификатор программы закачки. Максимальная длина 38 символов.
		/// </summary>
		public string ProgramIdentifier
		{
			get 
            { 
                return programIdentifier; 
            }
			set 
			{
				// Проверка на корректность устанавливаемого значения
				//if (value.Length > 38) throw new Exception("Некорректный формат идентификатора программы закачки.");
				programIdentifier = value;
			}
		}

		/// <summary>
		/// Текстовое описание элемента реестра закачек. Максимальная длина 2048 символов.
		/// </summary>
		public string Description
		{
			get 
            { 
                return description; 
            }
			set 
			{
				description = value;
			}
		}

		/// <summary>
		/// Конфигурационные параметры для закачки (в формате XML)
		/// </summary>
		public string ProgramConfig
		{
			get 
            {
                IDatabase db = scheme.SchemeDWH.DB;
                try
                {
                    programConfig = Convert.ToString(db.ExecQuery(
                        "select PROGRAMCONFIG from PUMPREGISTRY where ID = ?",
                        QueryResultTypes.Scalar,
                        db.CreateParameter("ID", _ID, DbType.Int64)));

                    return programConfig;
                }
                catch
                {
                    throw;
                }
                finally
                {
                    db.Dispose();
                }
            }
			set 
			{
                // получаем объект подключения к базе данных
                IDatabase db = scheme.SchemeDWH.DB;
                try
                {
                    db.ExecQuery(
                        "update PUMPREGISTRY set PROGRAMCONFIG = ? where ID = ?",
                        QueryResultTypes.NonQuery,
                        db.CreateParameter("PROGRAMCONFIG", value, DbType.AnsiString),
                        db.CreateParameter("ID", _ID, DbType.Int64));
                    
                    programConfig = value;
                }
                finally
                {
                    db.Dispose();
                }
			}
		}

		/// <summary>
		/// Название операции закачки
		/// </summary>
		public string Name
		{
			get 
            { 
                return name; 
            }
			set 
			{
				name = value;
			}
		}

        /// <summary>
        /// Настройки этапов закачки
        /// </summary>
        public string StagesParameters
        {
            get
            {
                IDatabase db = scheme.SchemeDWH.DB;
                try
                {
                    stagesParameters = Convert.ToString(db.ExecQuery(
                        "select STAGESPARAMS from PUMPREGISTRY where ID = ?",
                        QueryResultTypes.Scalar,
                        db.CreateParameter("ID", _ID, DbType.Int64)));

                    return stagesParameters;
                }
                catch
                {
                    throw;
                }
                finally
                {
                    db.Dispose();
                }
            }
            set
            {
                // получаем объект подключения к базе данных
                IDatabase db = scheme.SchemeDWH.DB;
                try
                {
                    db.ExecQuery(
                        "update PUMPREGISTRY set STAGESPARAMS = ? where ID = ?",
                        QueryResultTypes.NonQuery,
                        db.CreateParameter("STAGESPARAMS", value, DbType.AnsiString),
                        db.CreateParameter("ID", _ID, DbType.Int64));

                    stagesParameters = value;
                }
                finally
                {
                    db.Dispose();
                }
            }
        }

        /// <summary>
        /// Расписание закачки
        /// </summary>
        public string Schedule
        {
            get
            {
                IDatabase db = scheme.SchemeDWH.DB;
                try
                {
                    schedule = Convert.ToString(db.ExecQuery(
                        "select SCHEDULE from PUMPREGISTRY where ID = ?",
                        QueryResultTypes.Scalar,
                        db.CreateParameter("ID", _ID, DbType.Int64)));

                    return schedule;
                }
                finally
                {
                    db.Dispose();
                }
            }

            set
            {
                // получаем объект подключения к базе данных
                IDatabase db = scheme.SchemeDWH.DB;
                try
                {
                    db.ExecQuery(
                        "update PUMPREGISTRY set SCHEDULE = ? where ID = ?",
                        QueryResultTypes.NonQuery,
                        db.CreateParameter("SCHEDULE", value, DbType.AnsiString),
                        db.CreateParameter("ID", _ID, DbType.Int64));

                    schedule = value;
                }
                finally
                {
                    db.Dispose();
                }
            }
        }

        public string PumpProgram
        {
            get { return pumpProgram; }
            set { pumpProgram = value; }
        }

		/// <summary>
		/// Выполненные операции закачки
		/// </summary>
		public IPumpHistoryCollection PumpHistoryCollection
		{
			get 
            { 
                return (IPumpHistoryCollection)pumpHistoryCollection; 
            }
			set
			{
				pumpHistoryCollection = (PumpHistoryCollection)value;
			}
		}

        /// <summary>
        /// Очередь этапов закачки
        /// </summary>
        public IStagesQueue StagesQueue
        {
            get
            {
                if (stagesQueue.StagesQueueElements.Count == 0)
                {
                    stagesQueue.LoadStagesQueue();
                }

                return stagesQueue as IStagesQueue;
            }
            set
            {
                stagesQueue = value as StagesQueue;
            }
        }

        /// <summary>
        /// История текущей закачки
        /// </summary>
        public DataTable PumpHistory
        {
            get
            {
                Database dbs = (Database)scheme.SchemeDWH.DB;

                try
                {
                    return dbs.ExecQuery(
                        "select * from PUMPHISTORY where REFPUMPREGISTRY = ?",
                        QueryResultTypes.DataTable,
                        dbs.CreateParameter("REFPUMPREGISTRY", this.ID)) as DataTable;
                }
                finally
                {
                    dbs.Close();
                }
            }
        }

        /// <summary>
        /// Все закачанные источники текущей закачки
        /// </summary>
        public DataTable DataSources
        {
            get
            {
                Database dbs = (Database)scheme.SchemeDWH.DB;

                try
                {
                    return dbs.ExecQuery(
                        "select * from DATASOURCES where UPPER(SUPPLIERCODE) = UPPER(?) and DATACODE = ?",
                        QueryResultTypes.DataTable,
                        dbs.CreateParameter("SUPPLIERCODE", this.SupplierCode),
                        dbs.CreateParameter("DATACODE", this.DataCode)) as DataTable;
                }
                finally
                {
                    dbs.Close();
                }
            }
        }

        /// <summary>
        /// Каталог, где находятся файлы для закачки данной программой
        /// </summary>
        public string DataSourcesUNCPath
        {
            get
            {
                string uncPath = GetDataSourceRootDirUNC().FullName;

                if (!Directory.Exists(uncPath))
                {
                    string str = this.DataSourcesLocalPath;
                }

                return uncPath;
            }
        }

        /// <summary>
        /// Каталог, где находятся файлы для закачки данной программой (local path)
        /// </summary>
        public string DataSourcesLocalPath
        {
            get
            {
                string localPath = GetDataSourceRootDirLocal().FullName;

                if (!Directory.Exists(localPath))
                {
                    Directory.CreateDirectory(localPath);
                }

                return localPath;
            }
        }

		#endregion Реализация свойств


        #endregion Реализация IPumpRegistryElement
	}
}