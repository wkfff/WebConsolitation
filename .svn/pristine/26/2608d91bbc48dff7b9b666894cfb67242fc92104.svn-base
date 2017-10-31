using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Lifetime;
using System.Threading;
using System.Xml;

using Krista.FM.Common;
using Krista.FM.Common.Handling;
using Krista.FM.Server.Common;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.ServerLibrary;
using System.Data.Common;


namespace Krista.FM.Server.DataPumps
{
	/// <summary>
	/// Базовый класс для всех закачек
	/// </summary>
    public abstract partial class DataPumpModuleBase : DisposableObject, IDataPumpModule
    {
        #region Структуры и перечисления

        /// <summary>
        /// Название базы ФМ
        /// </summary>
        public enum DBMSName
        {
            /// <summary>
            /// Оракл
            /// </summary>
            Oracle,

            /// <summary>
            /// СкульСервер
            /// </summary>
            SQLServer,

            /// <summary>
            /// хз что за база
            /// </summary>
            Unknown
        }

        /// <summary>
        /// Наименование текущего региона
        /// </summary>
        public enum RegionName
        {
            RF = 0,
            
            Altay = 84000000,
            AltayKrai = 1000000,
            Arkhangelsk = 11000000,
            Chechnya = 96000000,
            EAO = 99000000,
            HMAO = 71800000,
            Kalmykya = 85000000,
            Karelya = 86000000,
            Kostroma = 34000000,
            Krasnodar = 3000000,
            Kursk = 38000000,
            Moskva = 45000000,
            MoskvaObl = 46000000,
            Nadym = 71916000,
            Naur = 96200000,
            Noginsk = 46639000,
            Novosibirsk = 50000000,
            Omsk = 52000000,
            OmskMO = 52800000,
            OmskCity = 52701000,
            Orenburg = 53000000,
            Orsk = 53723000,
            Osetya = 90000000,
            Penza = 56000000,
            Samara = 36000000,
            SamaraGO = 36700000,
            Saratov = 63000000,
            Stavropol = 7000000,
            Tambov = 68000000,
            Tula = 70000000,
            Tyumen = 71000000,
            Tyva = 93000000,
            Vologda = 19000000,
            YNAO = 71900000,
            Yaroslavl = 78000000,
            Urengoy =  71956000,

            Unknown = -1
        }

        #endregion Структуры и перечисления


        #region Поля

        private PumpProcessStates state = PumpProcessStates.Prepared;
        private string programIdentifier = string.Empty;
        private PumpProgramID pumpProgramID;
        private string systemVersion = string.Empty;
        private string programVersion = string.Empty;
		private IScheme scheme = null;
		private bool exitFlag = true;
		private Database db;
		private ISystemEventsProtocol systemProtocol = null;
        private IPreviewDataProtocol previewDataProtocol = null;
		private IDataPumpProtocol dataPumpProtocol = null;
		private IProcessDataProtocol processDataProtocol = null;
        private IBridgeOperationsProtocol associateDataProtocol = null;
        private IMDProcessingProtocol processCubeProtocol = null;
        private IReviseDataProtocol checkDataProtocol = null;
		private IDeleteDataProtocol deleteDataProtocol = null;
        private IUsersOperationProtocol usersOperationProtocol = null;
		private int pumpID = -1;
		private DirectoryInfo rootDir = null;
		private bool useArchive = false;
		private bool deleteEarlierData = true;
        private bool checkSourceDirToEmpty = true;
        private IClassifier[] usedClassifiers;
        private IClassifier[] hierarchyClassifiers;
        private IClassifier[] associateClassifiers;
        private IClassifier[] associateClassifiersEx;
        private IClassifier[] cubeClassifiers;
        private IClassifier[] versionClassifiers;
        private IFactTable[] usedFacts;
        private IFactTable[] cubeFacts;
        private LogicalCallContextData context = null;
        private IDataSource dataSource = null;
        private IBaseProtocol baseProtocol;
        private bool locked = false;
        private DataSourcesProcessingResult dataSourcesProcessingResult = null;
        private DataSourceProcessingResult dataSourceProcessingResult = DataSourceProcessingResult.SuccessfulProcessed;
        private string sessionID;
        private IDataPumpProgress dataPumpProgress;
        private IServerSideDataPumpProgress serverSideDataPumpProgress;
        private ServerSideDataPumpProgressHandling serverSideDataPumpProgressHandling;
        private IPumpRegistryElement pumpRegistryElement = null;
        private bool finalOverturn;
        private int sourceID = -1;
        private DBMSName serverDBMSName = DBMSName.Unknown;
        private RegionName region = RegionName.Unknown;
        public string userName = string.Empty;
        public string userHost = string.Empty;
        public string userSessionID = string.Empty;
        // пользовательский параметр для использования в теле закачки
        // используется в случае, когда закачка создается не в воркплэйсе (например веб интерфейс)
        public string userCustomParam = string.Empty;

        #endregion Поля


        #region Константы

        /// <summary>
        /// Константа наименования неизвестного классификатора
        /// </summary>
        public const string constDefaultClsName = "Неуказанное наименование";

        /// <summary>
        /// Название каталога с файлами ошибок закачки
        /// </summary>
        public const string constDataPumpErrorsDir = "DataPumpErrors";

        /// <summary>
        /// Переменная окружения, используемая для получения пути к временному каталогу
        /// </summary>
        public const string constTempDirEnvironmentVariable = "TEMP";

        /// <summary>
        /// Месяц и день даты заключительных оборотов
        /// </summary>
        public const int constFinalOverturnMonthDay = 1232;

        /// <summary>
        /// Название глобальной контанты ОКТМО текущего региона
        /// </summary>
        private const string constRegionGlobalConst = "OKTMO";

        #endregion Константы


        #region Инициализация

        /// <summary>
		/// Конструктор
		/// </summary>
		/// <param name="scheme">Ссылка на интерфейс объекта схемы</param>
		public DataPumpModuleBase()
		{

		}

		/// <summary>
		/// Деструктор
		/// </summary>
        protected override void Dispose(bool disposing)
		{
            if (disposing)
            {
                try
                {
                    ClearServerSideDataPumpProgressHandling();

                    if (baseProtocol != null)
                    {
                        baseProtocol.Dispose();
                        baseProtocol = null;
                    }

                    if (dataSourcesProcessingResult != null)
                        dataSourcesProcessingResult.Dispose();

                    if (db != null)
                    {
                        db.Close();
                        db = null;
                    }
                }
                finally
                {
                    try
                    {
                        string sessionID = Convert.ToString(this.Context["SessionID"]);
                        this.scheme.SessionManager.Sessions[sessionID].Dispose();
                    }
                    catch (Exception ex)
                    {
                        WriteToTrace("Ошибка при уничтожении сессии: " + ex.ToString(), TraceMessageKind.Error);
                    }

                    CollectGarbage();
                }
            }
        }

        #endregion Инициализация


        #region Свойства класса

        /// <summary>
		/// Класс доступа к данным
		/// </summary>
        public Database DB
		{
			get 
            {
                try
                {
                    if (db == null || db.Disposed)
                    {
                        if (db != null && db.Disposed)
                        {
                            WriteToTrace("DataPumpModuleBase.DB is disposed.", TraceMessageKind.Error);
                        }

                        WriteToTrace("Создание DataPumpModuleBase.DB", TraceMessageKind.Information);

                        DbProviderFactory factory = new DbProviderFactoryWrapper(
                            DbProviderFactories.GetFactory(scheme.SchemeDWH.FactoryName));
                        IDbConnection conn = factory.CreateConnection();
                        conn.ConnectionString = scheme.SchemeDWH.ConnectionString;

                        db = new Database(conn, factory, false, constCommandTimeout);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Ошибка при создании DataPumpModuleBase.DB", ex);
                }

                return db; 
            }
        }

        #region Протоколы
        /// <summary>
        /// Базовый класс протокола
        /// </summary>
        public IBaseProtocol BaseProtocol
        {
            get
            {
                if (baseProtocol == null)
                {
                    baseProtocol = this.scheme.GetProtocol(Assembly.GetExecutingAssembly().ManifestModule.Name);
                }
                return baseProtocol;
            }
            set
            {
                baseProtocol = value;
            }
        }

		/// <summary>
		/// Интерфейс системного протокола
		/// </summary>
        public ISystemEventsProtocol SystemProtocol
		{
			get 
            {
                if (systemProtocol == null)
                {
                    systemProtocol = (ISystemEventsProtocol)this.BaseProtocol;
                }

                return systemProtocol; 
            }
            private set 
            { 
                systemProtocol = value; 
            }
		}

        /// <summary>
        /// Интерфейс протокола предпросмотра
        /// </summary>
        public IPreviewDataProtocol PreviewDataProtocol
        {
            get
            {
                if (previewDataProtocol == null)
                {
                    previewDataProtocol = (IPreviewDataProtocol)this.BaseProtocol;
                }

                return previewDataProtocol;
            }
            private set
            {
                previewDataProtocol = value;
            }
        }

		/// <summary>
		/// Интерфейс протокола закачки
		/// </summary>
        public IDataPumpProtocol DataPumpProtocol
		{
			get 
            {
                if (dataPumpProtocol == null)
                {
                    dataPumpProtocol = (IDataPumpProtocol)this.BaseProtocol;
                }

                return dataPumpProtocol; 
            }
            private set 
            { 
                dataPumpProtocol = value; 
            }
		}

		/// <summary>
		/// Интерфейс протокола обработки данных
		/// </summary>
        public IProcessDataProtocol ProcessDataProtocol
		{
			get 
            {
                if (processDataProtocol == null)
                {
                    processDataProtocol = (IProcessDataProtocol)this.BaseProtocol;
                }

                return processDataProtocol; 
            }
            private set 
            { 
                processDataProtocol = value; 
            }
		}

        /// <summary>
        /// Интерфейс протокола сопоставления данных
        /// </summary>
        public IBridgeOperationsProtocol AssociateDataProtocol
        {
            get 
            {
                if (associateDataProtocol == null)
                {
                    associateDataProtocol = (IBridgeOperationsProtocol)this.BaseProtocol;
                }

                return associateDataProtocol; 
            }
            private set 
            { 
                associateDataProtocol = value; 
            }
        }

		/// <summary>
		/// Интерфейс протокола обработки данных
		/// </summary>
        public IMDProcessingProtocol ProcessCubeProtocol
		{
            get 
            {
                if (processCubeProtocol == null)
                {
                    processCubeProtocol = (IMDProcessingProtocol)this.BaseProtocol;
                }

                return processCubeProtocol; 
            }
            private set 
            { 
                processCubeProtocol = value; 
            }
		}

        /// <summary>
        /// Интерфейс протокола проверки данных
        /// </summary>
        public IReviseDataProtocol CheckDataProtocol
        {
            get 
            {
                if (checkDataProtocol == null)
                {
                    checkDataProtocol = (IReviseDataProtocol)this.BaseProtocol;
                }

                return checkDataProtocol; 
            }
            private set 
            { 
                checkDataProtocol = value; 
            }
        }

		/// <summary>
		/// Интерфейс протокола удалении данных
		/// </summary>
        public IDeleteDataProtocol DeleteDataProtocol
		{
			get 
            {
                if (deleteDataProtocol == null)
                {
                    deleteDataProtocol = (IDeleteDataProtocol)this.BaseProtocol;
                }

                return deleteDataProtocol; 
            }
			set 
            { 
                deleteDataProtocol = value; 
            }
		}

		/// <summary>
		/// Интерфейс протокола действий пользователей
		/// </summary>
        public IUsersOperationProtocol UsersOperationProtocol
		{
			get 
            {
                if (usersOperationProtocol == null)
                {
                    usersOperationProtocol = (IUsersOperationProtocol)this.BaseProtocol;
                }

                return usersOperationProtocol; 
            }
            private set 
            {
                usersOperationProtocol = value; 
            }
        }
        #endregion

        /// <summary>
		/// ИД текущей закачки
		/// </summary>
        public int PumpID
		{
			get 
            { 
                return pumpID; 
            }
            private set 
            { 
                pumpID = value; 
            }
		}

        /// <summary>
        /// ИД текущего источника
        /// </summary>
        public int SourceID
        {
            get 
            {
                return this.sourceID; 
            }
        }

		/// <summary>
		/// Каталог с данными для закачки
		/// </summary>
        public DirectoryInfo RootDir
		{
			get 
            { 
                return rootDir; 
            }
            private set 
            { 
                rootDir = value; 
            }
		}

		/// <summary>
		/// Перемещать файлы в архив
		/// </summary>
        public bool UseArchive
		{
			get 
            { 
                return useArchive; 
            }
            protected set 
            { 
                useArchive = value; 
            }
		}

		/// <summary>
		/// Удалять закачанные ранее данные из того же источника
		/// </summary>
        public bool DeleteEarlierData
		{
			get 
            { 
                return deleteEarlierData; 
            }
            set 
            { 
                deleteEarlierData = value; 
            }
		}

        /// <summary>
        /// Нужна проверка, что каталог источника пуст? (по умолчанию нужна)
        /// </summary>
        public bool CheckSourceDirToEmpty
        {
            get
            {
                return checkSourceDirToEmpty;
            }
            set
            {
                checkSourceDirToEmpty = value;
            }
        }

        /// <summary>
        /// Массив используемых закачкой классификаторов. 
        /// Нужен для единообразного выполнения установки иерархии, удаления, сопоставления, расчета кубов и т.п.
        /// </summary>
        public IClassifier[] UsedClassifiers
        {
            get 
            { 
                return usedClassifiers; 
            }
            set 
            { 
                usedClassifiers = value; 
            }
        }

        /// <summary>
        /// Массив классификаторов для установки иерархии
        /// </summary>
        public IClassifier[] HierarchyClassifiers
        {
            get
            {
                return hierarchyClassifiers;
            }
            set
            {
                hierarchyClassifiers = value;
            }
        }

        /// <summary>
        /// Массив классификаторов для сопоставления
        /// </summary>
        public IClassifier[] AssociateClassifiers
        {
            get
            {
                return associateClassifiers;
            }
            set
            {
                associateClassifiers = value;
            }
        }

        /// <summary>
        /// Массив классификаторов для сопоставления, которые формируются по источникам, отличающимся от источников закачки
        /// </summary>
        public IClassifier[] AssociateClassifiersEx
        {
            get
            {
                return associateClassifiersEx;
            }
            set
            {
                associateClassifiersEx = value;
            }
        }

        /// <summary>
        /// Массив классификаторов для расчета кубов
        /// </summary>
        public IClassifier[] CubeClassifiers
        {
            get
            {
                return cubeClassifiers;
            }
            set
            {
                cubeClassifiers = value;
            }
        }

        /// <summary>
        /// Массив классификаторов, для которых нужно устанавливать версию
        /// </summary>
        public IClassifier[] VersionClassifiers
        {
            get
            {
                return versionClassifiers;
            }
            set
            {
                versionClassifiers = value;
            }
        }

        /// <summary>
        /// Массив используемых закачкой таблиц фактов. 
        /// Нужен для единообразного выполнения удаления и т.п.
        /// </summary>
        public IFactTable[] UsedFacts
        {
            get 
            { 
                return usedFacts; 
            }
            set 
            { 
                usedFacts = value; 
            }
        }

        /// <summary>
        /// Массив фактов для расчета кубов
        /// </summary>
        public IFactTable[] CubeFacts
        {
            get
            {
                return cubeFacts;
            }
            set
            {
                cubeFacts = value;
            }
        }

        /// <summary>
        /// Элемент реестра закачек текущей закачки
        /// </summary>
        public IPumpRegistryElement PumpRegistryElement
        {
            get
            {
                return pumpRegistryElement;
            }
        }

        /// <summary>
        /// Контекст закачки
        /// </summary>
        public LogicalCallContextData Context
        {
            get
            {
                return context;
            }
            set
            {
                if (context != null)
                {
                    throw new ArgumentException("Контекст закачки уже инициализирован.");
                }

                context = value;
            }
        }

        // Создаем враппер для датасоурса чтобы обращение к свойствам не шло через Remoting
        internal class DataSourceWrapper : IDataSource
        {
            private IDataSource dataSourceProxy = null;

            public DataSourceWrapper(IDataSource dataSourceProxy)
            {
                if (dataSourceProxy == null)
                    throw new Exception("Не задан dataSourceProxy");

                this.dataSourceProxy = dataSourceProxy;
                this.id = dataSourceProxy.ID;
                this.supplierCode = dataSourceProxy.SupplierCode;
                this.dataCode = dataSourceProxy.DataCode;
                this.dataName = dataSourceProxy.DataName;
                this.parametersType = dataSourceProxy.ParametersType;
                this.budgetName = dataSourceProxy.BudgetName;
                this.territory = dataSourceProxy.Territory;
                this.year = dataSourceProxy.Year;
                this.month = dataSourceProxy.Month;
                this.quarter = dataSourceProxy.Quarter;
                this.variant = dataSourceProxy.Variant;
            }

            #region IDataSource
            public void DeleteData()
            {
                dataSourceProxy.DeleteData();
            }

            public void LockDataSource()
            {
                dataSourceProxy.LockDataSource();
            }

            public void UnlockDataSource()
            {
                dataSourceProxy.UnlockDataSource();
            }

            public DataTable RemoveWithData(DataTable dependedObjects)
            {
                return dataSourceProxy.RemoveWithData(dependedObjects);
            }

            private int id;
            public int ID
            {
                get { return id; }
            }

            private string supplierCode;
            public string SupplierCode
            {
                get { return supplierCode; }
                set
                {
                    supplierCode = value;
                    dataSourceProxy.SupplierCode = supplierCode;
                }
            }

            private string dataCode;
            public string DataCode
            {
                get { return dataCode; }
                set
                {
                    dataCode = value;
                    dataSourceProxy.DataCode = dataCode;
                }
            }

            private string dataName;
            public string DataName
            {
                get { return dataName; }
                set
                {
                    dataName = value;
                    dataSourceProxy.DataName = dataName;
                }
            }

            private ParamKindTypes parametersType;
            public ParamKindTypes ParametersType
            {
                get { return parametersType; }
                set
                {
                    parametersType = value;
                    dataSourceProxy.ParametersType = parametersType;
                }
            }

            private string budgetName;
            public string BudgetName
            {
                get { return budgetName; }
                set
                {
                    budgetName = value;
                    dataSourceProxy.BudgetName = budgetName;
                }
            }

            private string territory;
            public string Territory
            {
                get { return territory; }
                set
                {
                    territory = value;
                    dataSourceProxy.Territory = territory;
                }
            }

            private int year;
            public int Year
            {
                get { return year; }
                set
                {
                    year = value;
                    dataSourceProxy.Year = year;
                }
            }

            private int month;
            public int Month
            {
                get { return month; }
                set
                {
                    month = value;
                    dataSourceProxy.Month = month;
                }
            }

            private string variant;
            public string Variant
            {
                get { return variant; }
                set
                {
                    variant = value;
                    dataSourceProxy.Variant = variant;
                }
            }

            private int quarter;
            public int Quarter
            {
                get { return quarter; }
                set
                {
                    quarter = value;
                    dataSourceProxy.Quarter = quarter;
                }
            }

            public int? FindInDatabase()
            {
                return dataSourceProxy.FindInDatabase();
            }

            public int Save()
            {
                return dataSourceProxy.Save();
            }

            /// <summary>
            /// Переводит источник в состояние "Утвержден".
            /// </summary>
            public void ConfirmDataSource()
            {
                this.dataSourceProxy.ConfirmDataSource();
            }

            /// <summary>
            /// Переводит источник в состояние "Не проверен".
            /// </summary>
            public void UnConfirmDataSource()
            {
                this.dataSourceProxy.UnConfirmDataSource();
            }

            #endregion
        }

        /// <summary>
        /// Текущий источник
        /// </summary>
        public IDataSource DataSource
        {
            get
            {
                return dataSource;
            }
            set
            {
                if (value != null)
                {
                    this.dataSource = new DataSourceWrapper(value);
                    this.sourceID = this.dataSource.ID;
                    this.DataSourceProcessingResult = DataSourceProcessingResult.SuccessfulProcessed;
                }
                else
                {
                    this.dataSource = null;
                    this.sourceID = -1;
                }
            }
        }

        /// <summary>
        /// Информация о результатах обработки источников данных закачки
        /// </summary>
        public DataSourcesProcessingResult DataSourcesProcessingResult
        {
            get
            {
                return dataSourcesProcessingResult;
            }
        }

        /// <summary>
        /// Список датасетов с обработанными данными источников для предпросмотра и последующей закачки
        /// </summary>
        public Dictionary<int, DataSet> PreviewDataSources
        {
            get
            {
                return dataSourcesProcessingResult.PreviewDataSources;
            }
        }

        /// <summary>
        /// Список закачанных источников.
        /// Ключ - ИД источника, значение - сообщение о результатах
        /// </summary>
        public Dictionary<int, string> PumpedSources
        {
            get
            {
                return dataSourcesProcessingResult.PumpedSources;
            }
        }

        /// <summary>
        /// Список обработанных источников.
        /// Ключ - ИД источника, значение - сообщение о результатах закачки источника
        /// </summary>
        public SortedList<int, string> ProcessedSources
        {
            get
            {
                return dataSourcesProcessingResult.ProcessedSources;
            }
        }

        /// <summary>
        /// Настройки обработки источников данных
        /// Ключ - ИД источника, значение - Настройки обработки источника данных
        /// </summary>
        public Dictionary<int, DataSourceProcessingSettings> DataSourcesProcessingSettings
        {
            get
            {
                return dataSourcesProcessingResult.DataSourcesProcessingSettings;
            }
        }

        /// <summary>
        /// Результат обработки данных текущего источника.
        /// Влияет на запись в логе об успешном или нет окончании обработки источника.
        /// </summary>
        public DataSourceProcessingResult DataSourceProcessingResult
        {
            get
            {
                return dataSourceProcessingResult;
            }
            set
            {
                dataSourceProcessingResult = value;
            }
        }

        /// <summary>
        /// Интерфейс информационной части закачки
        /// </summary>
        public IDataPumpInfo DataPumpInfo
        {
            get
            {
                return this.Scheme.DataPumpManager.DataPumpInfo;
            }
        }

        /// <summary>
        /// Информация о ходе закачки для клиента
        /// </summary>
        public IDataPumpProgress DataPumpProgress
        {
            get
            {
                return dataPumpProgress;
            }
        }

        /// <summary>
        /// Информация о ходе закачки для сервера
        /// </summary>
        public IServerSideDataPumpProgress ServerSideDataPumpProgress
        {
            get
            {
                return serverSideDataPumpProgress;
            }
        }

        /// <summary>
        /// Очередь этапов закачки
        /// </summary>
        public IStagesQueue StagesQueue
        {
            get
            {
                if (this.PumpRegistryElement == null) return null;

                return this.PumpRegistryElement.StagesQueue;
            }
        }

        /// <summary>
        /// Запись истории текущей закачки
        /// </summary>
        public IPumpHistoryElement PumpHistoryElement
        {
            get
            {
                if (this.PumpID > 0)
                {
                    return this.PumpRegistryElement.PumpHistoryCollection[this.PumpID];
                }

                return null;
            }
        }

        /// <summary>
        /// Признак закачки заключительных оборотов
        /// </summary>
        protected bool FinalOverturn
        {
            get
            {
                return finalOverturn;
            }
            private set
            {
                finalOverturn = value;
            }
        }

        /// <summary>
        /// Признак закачки заключительных оборотов
        /// </summary>
        protected int FinalOverturnDate
        {
            get
            {
                return this.DataSource.Year * 10000 + constFinalOverturnMonthDay;
            }
        }

        /// <summary>
        /// Название СУБД базы ФМ
        /// </summary>
        public DBMSName ServerDBMSName
        {
            get
            {
                return serverDBMSName;
            }
        }

        /// <summary>
        /// Текущий район
        /// </summary>
        public RegionName Region
        {
            get
            {
                return region;
            }
        }

		#endregion Свойства класса


		#region Общие функции

		/// <summary>
		/// Добавляет запись истории закачки
		/// </summary>
		/// <param name="programIdentifier">Идентификатор программы закачки</param>
		/// <param name="comment">Комментарий</param>
		/// <returns>ИД записи</returns>
        private int AddPumpHistoryElement(string programIdentifier, string comment)
		{
			try
			{
                IPumpHistoryElement his = this.PumpRegistryElement.PumpHistoryCollection.CreateElement(this.ProgramIdentifier);
                his.ProgramConfig = this.PumpRegistryElement.ProgramConfig;
				his.SystemVersion = this.SystemVersion;
				his.ProgramVersion = this.ProgramVersion;
				his.PumpDate = Convert.ToDateTime(DateTime.Now);
				his.StartedBy = 1;
				his.Description = comment;
                his.UserName = this.userName;
                his.UserHost = this.userHost;
                his.SessionID = this.userSessionID;
                return this.PumpRegistryElement.PumpHistoryCollection.Add(his);
			}
			catch (Exception ex)
			{
                throw new Exception("Ошибка при добавлении записи истории закачки", ex);
			}
		}

        /// <summary>
		/// Добавляет запись истории закачки
		/// </summary>
		/// <param name="this.ProgramIdentifier">Идентификатор программы закачки</param>
		/// <param name="comment">Комментарий</param>
		/// <returns>ИД записи</returns>
        private int AddPumpHistoryElement(PumpProgramID programIdentifier, string comment)
        {
            return AddPumpHistoryElement(Convert.ToString(programIdentifier), comment);
        }

		/// <summary>
		/// Устанавливает состояние прогресса
		/// </summary>
		/// <param name="maxPos">Макс. значение (-1 - не менять)</param>
		/// <param name="currentPos">Текущее значение (-1 - не менять)</param>
		/// <param name="message">Сообщение над прогрессом (пустая строка - не менять)</param>
		/// <param name="text">Текст полосы прогресса (пустая строка - не менять)</param>
        /// <param name="constantRefresh">Обновлять прогресс на каждом шаге</param>
        public void SetProgress(int maxPos, int currentPos, string message, string text, bool constantRefresh)
		{
            if (!constantRefresh && currentPos % 100 != 0) return;

            if (maxPos >= 0) this.DataPumpProgress.ProgressMaxPos = maxPos;

            if (currentPos >= 0) this.DataPumpProgress.ProgressCurrentPos = currentPos;

            if (message != string.Empty) this.DataPumpProgress.ProgressMessage = message;

            if (maxPos == 0 && currentPos == 0)
            {
                this.DataPumpProgress.ProgressText = string.Empty;
            }
            else if (text != string.Empty)
            {
                this.DataPumpProgress.ProgressText = text;
            }
		}

        /// <summary>
		/// Устанавливает состояние прогресса
		/// </summary>
		/// <param name="maxPos">Макс. значение (-1 - не менять)</param>
		/// <param name="currentPos">Текущее значение (-1 - не менять)</param>
		/// <param name="message">Сообщение над прогрессом (пустая строка - не менять)</param>
		/// <param name="text">Текст полосы прогресса (пустая строка - не менять)</param>
        public void SetProgress(int maxPos, int currentPos, string message, string text)
        {
            SetProgress(maxPos, currentPos, message, text, false);
        }

		/// <summary>
		/// Перемещает список файлов в каталог архива
		/// </summary>
        /// <param name="dir">Каталог с файлами</param>
        protected string MoveFilesToArchive(DirectoryInfo dir, string supplierCode, string dataCode, int pumpID, 
            int sourceID, string searchPattern)
		{
			try
			{
				// Получаем путь к архивному каталогу
				string archive = string.Format("{0}\\{1}{2}{3} {4} {5} {6} {7} {8}\\",
					this.Scheme.DataSourceManager.ArchiveDirectory,
					DateTime.Now.Year, DateTime.Now.Month.ToString().PadLeft(2, '0'),
					DateTime.Now.Day.ToString().PadLeft(2, '0'),
					DateTime.Now.ToLongTimeString().ToString().Replace(':', '-'), 
					supplierCode, dataCode.PadLeft(4, '0'), pumpID, sourceID);

				// Если такого каталога нет - создаем
				if (!Directory.Exists(archive)) Directory.CreateDirectory(archive);
				if (!Directory.Exists(archive)) return "Невозможно создать каталог архива.";

                // Перемещаем подкаталоги
                DirectoryInfo[] subDirs = dir.GetDirectories("*", SearchOption.TopDirectoryOnly);
                for (int i = 0; i < subDirs.GetLength(0); i++)
                {
                    Directory.CreateDirectory(archive + subDirs[i].Name);

                    FileInfo[] subdirFiles = subDirs[i].GetFiles();
                    for (int j = 0; j < subdirFiles.GetLength(0); j++)
                        subdirFiles[j].MoveTo(string.Format("{0}{1}\\{2}", archive, subDirs[i].Name, subdirFiles[j].Name));

                    Directory.Delete(subDirs[i].FullName);
                }

				// Перемещаем файлы
                FileInfo[] files = dir.GetFiles(searchPattern, SearchOption.AllDirectories);
				for (int i = 0; i < files.GetLength(0); i++)
				{
					File.Move(files[i].FullName, archive + files[i].Name);
				}

				return string.Empty;
			}
			catch (Exception ex)
			{
				return ex.Message;
			}
		}

        protected string MoveFilesToArchive(DirectoryInfo dir, string supplierCode, string dataCode, int pumpID,
            int sourceID)
        {
            return MoveFilesToArchive(dir, supplierCode, dataCode, pumpID, sourceID, "*.*");
        }

        /// <summary>
        /// Перемещает файлы в арихив. Только для этапа закачки.
        /// </summary>
        /// <param name="dir">Каталог с файлами</param>
        protected void MoveFilesToArchive(DirectoryInfo dir)
        {
            if (this.UseArchive)
            {
                string str = this.MoveFilesToArchive(
                    dir,
                    this.PumpRegistryElement.SupplierCode,
                    this.PumpRegistryElement.DataCode.PadLeft(4, '0'),
                    this.PumpID, this.SourceID);

                if (str != string.Empty)
                {
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeError,
                        string.Format("Ошибка при перемещении файлов в каталог архива: {0}.", str));
                }
                else
                {
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Файлы источника перемещены в каталог архива.");
                }
            }
        }

        /// <summary>
        /// Функция сбора мусора
        /// </summary>
        protected void CollectGarbage()
        {
            WriteToTrace("Старт сбора мусора.", TraceMessageKind.Information);

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            WriteToTrace("Сбор мусора завершен.", TraceMessageKind.Information);
        }

        /// <summary>
        /// Освобождает ресурсы, занятые свойствами класса
        /// </summary>
        protected virtual void DisposeProperties()
        {
            try
            {
                /*if (db != null)
                {
                    db.Close();
                    db = null;
                }

                if (baseProtocol != null) baseProtocol.Dispose();
                baseProtocol = null;*/
            }
            catch (Exception ex)
            {
                WriteToTrace("Ошибка при освобождении ресурсов: " + ex.ToString(), TraceMessageKind.Error);
            }
        }

        /// <summary>
        /// Тип сообщения, записываемого в лог
        /// </summary>
        public enum TraceMessageKind
        {
            /// <summary>
            /// Критическая ошибка
            /// </summary>
            CriticalError,

            /// <summary>
            /// Ошибка
            /// </summary>
            Error,

            /// <summary>
            /// Информационное сообщение
            /// </summary>
            Information,

            /// <summary>
            /// Некритическая ошибка
            /// </summary>
            Warning
        }

        /// <summary>
        /// Пишет сообщение в лог сервера
        /// </summary>
        /// <param name="message">Сообщение</param>
        /// <param name="traceMessageKind">Тип сообщения, записываемого в лог</param>
        protected void WriteToTrace(string message, TraceMessageKind traceMessageKind, string category)
        {
            string prefix = string.Empty;

            switch (traceMessageKind)
            {
                case TraceMessageKind.CriticalError:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    prefix = "CRITICAL ERROR: ";
                    break;

                case TraceMessageKind.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    prefix = "ERROR: ";
                    break;

                case TraceMessageKind.Information:
                    break;

                case TraceMessageKind.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    prefix = "WARNING: ";
                    break;
            }
            string traceMsg = string.Format("{0} {1}{2}", DateTime.Now, prefix, message);
            System.Diagnostics.Trace.WriteLine(traceMsg, category);
            Console.ResetColor();
        }

        /// <summary>
        /// Пишет сообщение в лог сервера
        /// </summary>
        /// <param name="message">Сообщение</param>
        /// <param name="traceMessageKind">Тип сообщения, записываемого в лог</param>
        public void WriteToTrace(string message, TraceMessageKind traceMessageKind)
        {
            WriteToTrace(message, traceMessageKind, this.ProgramIdentifier);
        }

        /// <summary>
        /// Возвращает текущий рабочий каталог (в котором располагается закачка)
        /// </summary>
        /// <returns>Каталог</returns>
        public DirectoryInfo GetCurrentDir()
        {
            FileInfo file = new FileInfo(Assembly.GetAssembly(this.GetType()).ManifestModule.FullyQualifiedName);
            return file.Directory;
        }

        /// <summary>
        /// Записывает строку ошибки в файл. 
        /// Формат файла: Дата Время Наименование_модуля_закачки.
        /// Файл располагается в подкаталоге DataPumpErrors
        /// </summary>
        /// <param name="error"></param>
        private void WriteStringToErrorFile(string error)
        {
            DirectoryInfo currDir = GetCurrentDir();
            string dirName = currDir.FullName + "\\" + constDataPumpErrorsDir;
            string fileName = string.Format("{0}\\{1}{2}{3} {4} {5}.txt",
                dirName,
                DateTime.Now.Year, DateTime.Now.Month.ToString().PadLeft(2, '0'),
                DateTime.Now.Day.ToString().PadLeft(2, '0'),
                DateTime.Now.ToLongTimeString().ToString().Replace(':', '-'),
                this.ProgramIdentifier);

            // Создаем каталог с файлами ошибок, если он еще не создан
            if (!Directory.Exists(dirName))
            {
                Directory.CreateDirectory(dirName);
            }

            // Создаем файл ошибки и записываем в него строку
            File.WriteAllText(fileName, error);
        }

        /// <summary>
        /// Разыменовка семантической принадлежности объекта
        /// </summary>
        /// <param name="cmnObj">объект</param>
        /// <returns>русское название семантической принадлежности</returns>
		[Obsolete("Для получения полного русского имени таблицы использовать свойство FullCaption интерфейса IEntity.")]
		protected string GetDataObjSemanticRus(IEntity cmnObj)
        {
        	return cmnObj.FullCaption;
        }

        /// <summary>
        /// Устанавливает свойство наименование региона
        /// </summary>
        private void SetRegionName()
        {
            if (this.Scheme.GlobalConstsManager.Consts.ContainsKey(constRegionGlobalConst))
            {
                object obj = this.Scheme.GlobalConstsManager.Consts[constRegionGlobalConst].Value;
                try
                {
                    this.region = (RegionName)System.Convert.ToInt32(System.Convert.ToString(obj).Replace(" ", string.Empty));
                }
                catch
                {
                    this.region = RegionName.Unknown;
                }

                WriteToTrace(string.Format("Текущий регион: {0}, OKTMO {1}", this.Region, (int)this.Region), TraceMessageKind.Warning);
            }
        }

        /// <summary>
        /// Выполняет действия по инициализации программы закачки
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual void Initialize(IScheme scheme, string programIdentifier, string userParams)
        {
            WriteToTrace("Инициализация программы закачки...", TraceMessageKind.Information, "DataPumpModuleBase");

            this.scheme = scheme;
            this.programIdentifier = programIdentifier;
            this.pumpProgramID = StringToPumpProgramID(programIdentifier);
            if (userParams != string.Empty)
            {
                this.userName = userParams.Split(' ')[0];
                this.userHost = userParams.Split(' ')[1];
                this.userSessionID = userParams.Split(' ')[2];
                if (userParams.Split(' ').GetLength(0) >= 4)
                    this.userCustomParam = userParams.Split(' ')[3];
            }

            // Создаем основной поток в котором будет выполняться закачка
            mainThread = new Thread(new ThreadStart(this.MainThreadFunction));

            LogicalCallContextData.SetAuthorization("Krista.FM.Server.DataPumps." + this.ProgramIdentifier);
            Krista.FM.Common.ClientSession.CreateSession(SessionClientType.DataPump);
            SessionManager sm = this.scheme.SessionManager as SessionManager;
            this.sessionID = sm.Create(LogicalCallContextData.GetContext());
            this.context = LogicalCallContextData.GetContext();

            Authentication.UserID = (int?)FixedUsers.FixedUsersIds.DataPump;

            this.dataPumpProgress = this.DataPumpInfo[programIdentifier] as IDataPumpProgress;
            this.pumpRegistryElement = this.Scheme.DataPumpManager.DataPumpInfo.PumpRegistry[programIdentifier];
            this.pumpRegistryElement.StagesQueue.ClearExecutingInformation();

            this.serverSideDataPumpProgress = this.DataPumpInfo[programIdentifier] as IServerSideDataPumpProgress;
            InitServerSideDataPumpProgressHandling();

            this.dataSourcesProcessingResult = new DataSourcesProcessingResult(this.Scheme);

            this.serverDBMSName = GetServerDBMSName();

            // Получаем текущий район
            SetRegionName();

            WriteToTrace(string.Format("Программа закачки проинициализирована. Создана сессия закачки {0} {1}", 
                this.ProgramIdentifier, sessionID), TraceMessageKind.Information, "DataPumpModuleBase");
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual void Initialize(IScheme scheme, string programIdentifier)
        {
            Initialize(scheme, programIdentifier, string.Empty);
        }

        /// <summary>
        /// Запуск потока закачки
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void StartThread()
        {
            mainThread.Start();
        }

        /// <summary>
        /// Создает имя для мьютекса синхронизации текущей закачки
        /// </summary>
        /// <returns>Имя мьютекса</returns>
        public string GetCurrentPumpMutexName()
        {
            return string.Format(
                "Krista.FM.Server.DataPumps.{0}.{0}Module.{1}",
                this.ProgramIdentifier, this.sessionID);
        }

        /// <summary>
        /// Возвращает мьютекс текущей закачки
        /// </summary>
        private Mutex GetCurrentPumpMutex()
        {
            return Mutex.OpenExisting(GetCurrentPumpMutexName());
        }

        /// <summary>
        /// Преобразует строку в элемент перечисления ИД закачек
        /// </summary>
        /// <param name="str">Строка</param>
        /// <returns>элемент перечисления ИД закачек</returns>
        private PumpProgramID StringToPumpProgramID(string str)
        {
            switch (str.ToUpper())
            {
                case "FKMONTHREPPUMP":
                    return PumpProgramID.FKMonthRepPump;
                case "SKIFMONTHREPPUMP":
                    return PumpProgramID.SKIFMonthRepPump;
                case "SKIFYEARREPPUMP":
                    return PumpProgramID.SKIFYearRepPump;
                case "BUDGETDATAPUMP":
                    return PumpProgramID.BudgetDataPump;
                case "FORM13PUMP":
                    return PumpProgramID.Form13Pump;
                case "FORM1NAPP7DAYPUMP":
                    return PumpProgramID.Form1NApp7DayPump;
                case "FORM1NAPP7MONTHPUMP":
                    return PumpProgramID.Form1NApp7MonthPump;
                case "FORM1NDPPUMP":
                    return PumpProgramID.Form1NDPPump;
                case "FORM1OBLPUMP":
                    return PumpProgramID.Form1OBLPump;
                case "LEASEPUMP":
                    return PumpProgramID.LeasePump;
                case "FUVAULTPUMP":
                    return PumpProgramID.FUVaultPump;
                case "FORM5NIOPUMP":
                    return PumpProgramID.Form5NIOPump;
                case "FORM16PUMP":
                    return PumpProgramID.Form16Pump;
                case "FNS28NDATAPUMP":
                    return PumpProgramID.FNS28nDataPump;
                case "TAXESREGULATIONDATAPUMP":
                    return PumpProgramID.TaxesRegulationDataPump;
                case "FORM10PUMP":
                    return PumpProgramID.Form10Pump;
                case "FORM1NMPUMP":
                    return PumpProgramID.Form1NMPump;
                case "FORM14PUMP":
                    return PumpProgramID.Form14Pump;
                case "GRBSOUTCOMESPROJECTPUMP":
                    return PumpProgramID.GRBSOutcomesProjectPump;
                case "INCOMESDISTRIBUTIONPUMP":
                    return PumpProgramID.IncomesDistributionPump;
                case "BUDGETLAYERSDATAPUMP":
                    return PumpProgramID.BudgetLayersDataPump;
                case "BUDGETVAULTPUMP":
                    return PumpProgramID.BudgetVaultPump;
                case "FORM4NMPUMP":
                    return PumpProgramID.Form4NMPump;
                case "BUDGETCASHRECEIPTSPUMP":
                    return PumpProgramID.BudgetCashReceiptsPump;
                case "FORM1NOMPUMP":
                    return PumpProgramID.Form1NOMPump;
                case "MOFO4PUMP":
                    return PumpProgramID.MOFO4Pump;
                case "UFK14PUMP":
                    return PumpProgramID.UFK14Pump;
                case "FNS23PUMP":
                    return PumpProgramID.FNS23Pump;
                case "UFK15PUMP":
                    return PumpProgramID.UFK15Pump;
                case "UFK16PUMP":
                    return PumpProgramID.UFK16Pump;
                case "UFK17PUMP":
                    return PumpProgramID.UFK17Pump;
                case "UFK18PUMP":
                    return PumpProgramID.UFK18Pump;
                case "UFK19PUMP":
                    return PumpProgramID.UFK19Pump;
                case "FO24PUMP":
                    return PumpProgramID.FO24Pump;
                case "FNS24PUMP":
                    return PumpProgramID.FNS24Pump;
                case "EO7PUMP":
                    return PumpProgramID.EO7Pump;
                case "FNS10PUMP":
                    return PumpProgramID.FNS10Pump;
                case "RNDV1PUMP":
                    return PumpProgramID.RNDV1Pump;
                case "FO18PUMP":
                    return PumpProgramID.FO18Pump;
                case "FNS11PUMP":
                    return PumpProgramID.FNS11Pump;
                case "FNS22PUMP":
                    return PumpProgramID.FNS22Pump;
                case "FNS14PUMP":
                    return PumpProgramID.FNS14Pump;
                case "FNS9PUMP":
                    return PumpProgramID.FNS9Pump;
                case "FNS8PUMP":
                    return PumpProgramID.FNS8Pump;
                case "FO25PUMP":
                    return PumpProgramID.FO25Pump;
                case "FNS7PUMP":
                    return PumpProgramID.FNS7Pump;
                case "FNS13PUMP":
                    return PumpProgramID.FNS13Pump;
                case "FNS25PUMP":
                    return PumpProgramID.FNS25Pump;
                case "FNSRF1PUMP":
                    return PumpProgramID.FNSRF1Pump;
                case "ORG5PUMP":
                    return PumpProgramID.ORG5Pump;
                case "MOFO16PUMP":
                    return PumpProgramID.MOFO16Pump;
                case "MOFO15PUMP":
                    return PumpProgramID.MOFO15Pump;
                case "UFK20PUMP":
                    return PumpProgramID.UFK20Pump;
                case "FO35PUMP":
                    return PumpProgramID.FO35Pump;
                case "UFK21PUMP":
                    return PumpProgramID.UFK21Pump;
                case "UFK22PUMP":
                    return PumpProgramID.UFK22Pump;
                case "EO8PUMP":
                    return PumpProgramID.EO8Pump;
                case "UFK10PUMP":
                    return PumpProgramID.UFK10Pump;
                case "MFRF4PUMP":
                    return PumpProgramID.MFRF4Pump;
                case "MFRF3PUMP":
                    return PumpProgramID.MFRF3Pump;
                case "EO9PUMP":
                    return PumpProgramID.EO9Pump;
                case "MOFO18PUMP":
                    return PumpProgramID.MOFO18Pump;
                case "FO28PUMP":
                    return PumpProgramID.FO28Pump;
                case "FO36PUMP":
                    return PumpProgramID.FO36Pump;
                case "FO37PUMP":
                    return PumpProgramID.FO37Pump;
                case "EO5PUMP":
                    return PumpProgramID.EO5Pump;
                case "FNS12PUMP":
                    return PumpProgramID.FNS12Pump;
                case "FNS17PUMP":
                    return PumpProgramID.FNS17Pump;
                case "FNS18PUMP":
                    return PumpProgramID.FNS18Pump;
                case "FNS27PUMP":
                    return PumpProgramID.FNS27Pump;
                case "STAT3PUMP":
                    return PumpProgramID.STAT3Pump;
                case "FNSRF3PUMP":
                    return PumpProgramID.FNSRF3Pump;
                case "FNS26PUMP":
                    return PumpProgramID.FNS26Pump;
                case "FST1PUMP":
                    return PumpProgramID.FST1Pump;
                case "FNS30PUMP":
                    return PumpProgramID.FNS30Pump;
                case "MINZDRAV1PUMP":
                    return PumpProgramID.MINZDRAV1Pump;
                case "LESHOZ1PUMP":
                    return PumpProgramID.LESHOZ1Pump;
                case "FNS28PUMP":
                    return PumpProgramID.FNS28Pump;
                case "FNS29PUMP":
                    return PumpProgramID.FNS29Pump;
                case "FO99PUMP":
                    return PumpProgramID.FO99Pump;
                case "FST2PUMP":
                    return PumpProgramID.FST2Pump;
                case "FO42PUMP":
                    return PumpProgramID.FO42Pump;
                case "FK4PUMP":
                    return PumpProgramID.FK4Pump;
                case "MFRF5PUMP":
                    return PumpProgramID.MFRF5Pump;
                case "GVF1PUMP":
                    return PumpProgramID.GVF1Pump;
                case "GVF2PUMP":
                    return PumpProgramID.GVF2Pump;
                case "GVF3PUMP":
                    return PumpProgramID.GVF3Pump;
                case "FK5PUMP":
                    return PumpProgramID.FK5Pump;
                case "FK6PUMP":
                    return PumpProgramID.FK6Pump;
                case "FK7PUMP":
                    return PumpProgramID.FK7Pump;
                case "EO19PUMP":
                    return PumpProgramID.EO19Pump;
                case "EO20PUMP":
                    return PumpProgramID.EO20Pump;
                case "UFK25PUMP":
                    return PumpProgramID.UFK25Pump;
                case "MOFO22PUMP":
                    return PumpProgramID.MOFO22Pump;
                case "MOFO20PUMP":
                    return PumpProgramID.MOFO20Pump;
                case "OOS1PUMP":
                    return PumpProgramID.OOS1Pump;
                case "ORG3PUMP":
                    return PumpProgramID.ORG3Pump;
                case "MOFO21PUMP":
                    return PumpProgramID.MOFO21Pump;
                case "MOFO23PUMP":
                    return PumpProgramID.MOFO23Pump;
                case "MOFO24PUMP":
                    return PumpProgramID.MOFO24Pump;
                case "MOFO26PUMP":
                    return PumpProgramID.MOFO26Pump;
                case "FO30PUMP":
                    return PumpProgramID.FO30Pump;
                case "MOFO27PUMP":
                    return PumpProgramID.MOFO27Pump;
                case "FK8PUMP":
                    return PumpProgramID.FK8Pump;
                case "FK9PUMP":
                    return PumpProgramID.FK9Pump;
                case "FK10PUMP":
                    return PumpProgramID.FK10Pump;
                case "MOFO28PUMP":
                    return PumpProgramID.MOFO28Pump;
                case "RKC1PUMP":
                    return PumpProgramID.RKC1Pump;
                case "MOFO29PUMP":
                    return PumpProgramID.MOFO29Pump;
                case "MOFO31PUMP":
                    return PumpProgramID.MOFO31Pump;
                case "MOFO33PUMP":
                    return PumpProgramID.MOFO33Pump;
                case "MOFO25PUMP":
                    return PumpProgramID.MOFO25Pump;
                case "FO53PUMP":
                    return PumpProgramID.FO53Pump;
                case "FNS19PUMP":
                    return PumpProgramID.FNS19Pump;
                case "LESHOZ2PUMP":
                    return PumpProgramID.LESHOZ2Pump;
                case "ADMIN6PUMP":
                    return PumpProgramID.ADMIN6Pump;
                case "STAT31PUMP":
                    return PumpProgramID.STAT31Pump;
                case "ADMIN7PUMP":
                    return PumpProgramID.ADMIN7Pump;
                case "ADMIN9PUMP":
                    return PumpProgramID.ADMIN9Pump;
                case "ADMIN8PUMP":
                    return PumpProgramID.ADMIN8Pump;
                case "FNSRF4PUMP":
                    return PumpProgramID.FNSRF4Pump;
                case "FK2PUMP":
                    return PumpProgramID.FK2Pump;
                case "FO47PUMP":
                    return PumpProgramID.FO47Pump;
                default:
                    return PumpProgramID.Unknown;
            }
        }

        /// <summary>
        /// Возвращает значение параметра по его имени
        /// </summary>
        /// <param name="paramsXML">хмл с параметрами</param>
        /// <param name="paramName">Название параметра</param>
        /// <param name="defaultValue">Значение по умолчанию</param>
        /// <returns>Значение параметра</returns>
        public string GetParamValueByName(string paramsXML, string paramName, string defaultValue)
        {
            try
            {
                XmlDocument xml_doc = new XmlDocument();
                xml_doc.LoadXml(paramsXML);

                XmlNode node = xml_doc.SelectSingleNode(string.Format("//*[@Name = '{0}']", paramName));
                if (node == null)
                {
                    return defaultValue;
                }
                if (node.Attributes["Value"] == null)
                {
                    return defaultValue;
                }
                else
                {
                    return node.Attributes["Value"].Value;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(string.Format(
                    "Ошибка при получении значения параметра закачки {0}: {1}", paramName, ex), "PumpParamsHelper");
                return defaultValue;
            }
        }

        /// <summary>
        /// Преобразует состтояние закачки в строку на русском языке
        /// </summary>
        /// <param name="state">Состояние закачки</param>
        /// <returns>Строка</returns>
        private string PumpProcessStatesToRusString(PumpProcessStates state)
        {
            switch (state)
            {
                case PumpProcessStates.PreviewData:
                    return "Предварительный просмотр";

                case PumpProcessStates.PumpData:
                    return "Закачка данных";

                case PumpProcessStates.ProcessData:
                    return "Обработка данных";

                case PumpProcessStates.AssociateData:
                    return "Сопоставление классификаторов";

                case PumpProcessStates.ProcessCube:
                    return "Расчет кубов";

                case PumpProcessStates.CheckData:
                    return "Проверка данных";

                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Определяет, это закачка заключительных оборотов или нет
        /// </summary>
        /// <returns>это закачка заключительных оборотов или нет</returns>
        private bool IsFinalOverturn()
        {
            return Convert.ToBoolean(
                GetParamValueByName(this.ProgramConfig, "ucbFinalOverturn", "false"));
        }

        /// <summary>
        /// Возвращает дату заключительных оборотов по указанной дате
        /// </summary>
        /// <param name="date">Дата</param>
        /// <returns>Дата заключительных оборотов</returns>
        protected int GetFinalOverturn(int date)
        {
            int year = date / 10000;
            int month = (date / 100) % 100;
            int day = date % 100;

            // Если это уже заключительный оборот, то его и возвращаем
            if (month == 12 && day == 32)
                return date;

            return (year - 1) * 10000 + constFinalOverturnMonthDay;
        }

        /// <summary>
        /// Возвращает название СУБД базы ФМ
        /// </summary>
        /// <returns></returns>
        private DBMSName GetServerDBMSName()
        {
			switch (this.Scheme.SchemeDWH.FactoryName)
			{
				case ProviderFactoryConstants.OracleClient:
				case ProviderFactoryConstants.OracleDataAccess:
				case ProviderFactoryConstants.MSOracleDataAccess:
					return DBMSName.Oracle;

				case ProviderFactoryConstants.SqlClient:
					return DBMSName.SQLServer;
			}
			return DBMSName.Unknown;
        }

        /// <summary>
        /// Старт транзакции
        /// </summary>
        protected void BeginTransaction()
        {
            if (this.DB.Transaction == null)
                this.DB.BeginTransaction();
        }

        /// <summary>
        /// Завершение текущей транзакции
        /// </summary>
        protected void CommitTransaction()
        {
            if (this.DB.Transaction != null)
                this.DB.Commit();
        }

        /// <summary>
        /// Откат текущей транзакции
        /// </summary>
        protected void RollbackTransaction()
        {
            if (this.DB.Transaction != null)
                this.DB.Rollback();
        }

        #endregion Общие функции

        
        #region Обработка событий сервера

        /// <summary>
        /// Событие остановки этапа
        /// </summary>
        private void serverSideDataPumpProgressHandling_SetState(PumpProcessStates state)
        {
            this.State = state;
        }

        /// <summary>
        /// Событие получение признака того, что закачка до сих пор работает, а не отвалилась нафиг
        /// </summary>
        private bool serverSideDataPumpProgressHandling_GetPumpLiveStatus()
        {
            return true;
        }

        /// <summary>
        /// Очистка обработчиков событий 
        /// </summary>
        private void ClearServerSideDataPumpProgressHandling()
        {
            if (serverSideDataPumpProgressHandling != null)
            {
                serverSideDataPumpProgressHandling.SetState -= new GetPumpStateDelegate(serverSideDataPumpProgressHandling_SetState);
                serverSideDataPumpProgressHandling.GetPumpLiveStatus -= new GetBoolDelegate(serverSideDataPumpProgressHandling_GetPumpLiveStatus);

                serverSideDataPumpProgressHandling.Dispose();
            }
        }

        /// <summary>
        /// Назначение обработчиков событий закачки
        /// </summary>
        private void InitServerSideDataPumpProgressHandling()
        {
            ClearServerSideDataPumpProgressHandling();

            serverSideDataPumpProgressHandling = new ServerSideDataPumpProgressHandling();
            serverSideDataPumpProgressHandling.ServerSideDataPumpProgress = serverSideDataPumpProgress;

            serverSideDataPumpProgressHandling.SetState += new GetPumpStateDelegate(serverSideDataPumpProgressHandling_SetState);
            serverSideDataPumpProgressHandling.GetPumpLiveStatus += new GetBoolDelegate(serverSideDataPumpProgressHandling_GetPumpLiveStatus);
        }

        #endregion Обработка событий сервера


        #region Функции для работы с логами

        #region Логи предпросмотра

        /// <summary>
        /// Записать событие в прокол предпросмотра данных
        /// </summary>
        /// <param name="eventKind">Тип события</param>
        /// <param name="pumpID">ИД закачки</param>
        /// <param name="sourceID">ИД источника</param>
        /// <param name="infoMsg">Информационное сообщение</param>
        /// <param name="ex">Исключение. Если не null, то в лог дописывается сообщение исключения, а в трейс - стек</param>
        protected void WriteEventIntoPreviewDataProtocol(PreviewDataEventKind eventKind, int pumpID, int sourceID,
            string infoMsg, Exception ex)
        {
            try
            {
                TraceMessageKind traceMessageKind = TraceMessageKind.Information;
                switch (eventKind)
                {
                    case PreviewDataEventKind.dpeCriticalError:
                    case PreviewDataEventKind.dpeError:
                    case PreviewDataEventKind.dpeFinishedWithErrors:
                    case PreviewDataEventKind.dpeFinishFilePumpWithError:
                        traceMessageKind = TraceMessageKind.Error;
                        break;

                    case PreviewDataEventKind.dpeWarning:
                        traceMessageKind = TraceMessageKind.Warning;
                        break;
                }

                if (ex == null)
                {
                    WriteToTrace(infoMsg, traceMessageKind);
                    this.PreviewDataProtocol.WriteEventIntoPreviewDataProtocol(eventKind, pumpID, sourceID, infoMsg);
                }
                else
                {
                    WriteToTrace(infoMsg + "\nТекст ошибки:\n" + ex.ToString(), traceMessageKind);
                    this.PreviewDataProtocol.WriteEventIntoPreviewDataProtocol(eventKind, pumpID, sourceID, infoMsg + "\nТекст ошибки:\n" + ex.Message);
                }
            }
            catch (Exception exp)
            {
                WriteToTrace("Ошибка при записи в лог: " + exp.ToString(), TraceMessageKind.Error);
            }
        }

        /// <summary>
        /// Записать событие в прокол предпросмотра данных
        /// </summary>
        /// <param name="eventKind">Тип события</param>
        /// <param name="infoMsg">Информационное сообщение</param>
        /// <param name="ex">Исключение. Если не null, то в лог дописывается сообщение исключения, а в трейс - стек</param>
        protected void WriteEventIntoPreviewDataProtocol(PreviewDataEventKind eventKind, string infoMsg, Exception ex)
        {
            WriteEventIntoPreviewDataProtocol(eventKind, this.PumpID, this.SourceID, infoMsg, ex);
        }

        /// <summary>
        /// Записать событие в прокол предпросмотра данных
        /// </summary>
        /// <param name="eventKind">Тип события</param>
        /// <param name="infoMsg">Информационное сообщение</param>
        protected void WriteEventIntoPreviewDataProtocol(PreviewDataEventKind eventKind, string infoMsg)
        {
            WriteEventIntoPreviewDataProtocol(eventKind, infoMsg, null);
        }

        #endregion Логи предпросмотра


        #region Логи закачки

        /// <summary>
        /// Записать событие в прокол закачки данных
        /// </summary>
        /// <param name="eventKind">Тип события</param>
        /// <param name="pumpID">ИД закачки</param>
        /// <param name="sourceID">ИД источника</param>
        /// <param name="infoMsg">Информационное сообщение</param>
        /// <param name="ex">Исключение. Если не null, то в лог дописывается сообщение исключения, а в трейс - стек</param>
        protected void WriteEventIntoDataPumpProtocol(DataPumpEventKind eventKind, int pumpID, int sourceID, 
            string infoMsg, Exception ex)
        {
            try
            {
                TraceMessageKind traceMessageKind = TraceMessageKind.Information;
                string prefix = "\n";

                switch (eventKind)
                {
                    case DataPumpEventKind.dpeCriticalError:
                    case DataPumpEventKind.dpeError:
                    case DataPumpEventKind.dpeFinishedWithErrors:
                    case DataPumpEventKind.dpeFinishFilePumpWithError: 
                        traceMessageKind = TraceMessageKind.Error;
                        prefix = "\nТекст ошибки:\n";
                        break;

                    case DataPumpEventKind.dpeWarning: 
                        traceMessageKind = TraceMessageKind.Warning;
                        break;
                }

                if (ex == null)
                {
                    WriteToTrace(infoMsg, traceMessageKind);
                    this.DataPumpProtocol.WriteEventIntoDataPumpProtocol(eventKind, pumpID, sourceID, infoMsg);
                }
                else
                {
                    WriteToTrace(infoMsg + prefix + ex.ToString(), traceMessageKind);
                    this.DataPumpProtocol.WriteEventIntoDataPumpProtocol(eventKind, pumpID, sourceID,
                        infoMsg + prefix + ex.Message);
                }
            }
            catch (Exception exp)
            {
                WriteToTrace("Ошибка при записи в лог: " + exp.ToString(), TraceMessageKind.Error);
            }
        }

        /// <summary>
        /// Записать событие в прокол закачки данных
        /// </summary>
        /// <param name="eventKind">Тип события</param>
        /// <param name="infoMsg">Информационное сообщение</param>
        /// <param name="ex">Исключение. Если не null, то в лог дописывается сообщение исключения, а в трейс - стек</param>
        protected void WriteEventIntoDataPumpProtocol(DataPumpEventKind eventKind, string infoMsg, Exception ex)
        {
            WriteEventIntoDataPumpProtocol(eventKind, this.PumpID, this.SourceID, infoMsg, ex);
        }

        /// <summary>
        /// Записать событие в прокол закачки данных
        /// </summary>
        /// <param name="eventKind">Тип события</param>
        /// <param name="infoMsg">Информационное сообщение</param>
        protected void WriteEventIntoDataPumpProtocol(DataPumpEventKind eventKind, string infoMsg)
        {
            WriteEventIntoDataPumpProtocol(eventKind, infoMsg, null);
        }

        /// <summary>
        /// Записать событие в прокол закачки данных
        /// </summary>
        /// <param name="eventKind">Тип события</param>
        /// <param name="infoMsg">Информационное сообщение</param>
        /// <param name="showProgress">Вывести сообщение на прогресс</param>
        protected void WriteEventIntoDataPumpProtocol(DataPumpEventKind eventKind, string infoMsg,
            bool showProgress)
        {
            WriteEventIntoDataPumpProtocol(eventKind, infoMsg, null);
            SetProgress(-1, -1, infoMsg, string.Empty, true);
        }

        #endregion Логи закачки


        #region Логи обработки данных

        /// <summary>
        /// Записать событие в протокол обработки данных
        /// </summary>
        /// <param name="eventKind">Вид событий</param>
        /// <param name="pumpID">ИД закачки</param>
        /// <param name="sourceID">ИД источника</param>
        /// <param name="infoMsg">Информационное сообщение</param>
        /// <param name="ex">Исключение. Если не null, то в лог дописывается сообщение исключения, а в трейс - стек</param>
        protected void WriteEventIntoProcessDataProtocol(ProcessDataEventKind eventKind, int pumpID, int sourceID, 
            string infoMsg, Exception ex)
        {
            try
            {
                TraceMessageKind traceMessageKind = TraceMessageKind.Information;
                string prefix = "\n";

                switch (eventKind)
                {
                    case ProcessDataEventKind.pdeCriticalError:
                    case ProcessDataEventKind.pdeError:
                    case ProcessDataEventKind.pdeFinishedWithErrors: 
                        traceMessageKind = TraceMessageKind.Error;
                        prefix = "\nТекст ошибки:\n";
                        break;

                    case ProcessDataEventKind.pdeWarning: 
                        traceMessageKind = TraceMessageKind.Warning;
                        break;
                }

                if (ex == null)
                {
                    WriteToTrace(infoMsg, traceMessageKind);
                    this.ProcessDataProtocol.WriteEventIntoProcessDataProtocol(eventKind, infoMsg, pumpID, sourceID);
                }
                else
                {
                    WriteToTrace(infoMsg + prefix + ex.ToString(), traceMessageKind);
                    this.ProcessDataProtocol.WriteEventIntoProcessDataProtocol(eventKind,
                        infoMsg + prefix + ex.Message, pumpID, sourceID);
                }
            }
            catch (Exception exp)
            {
                WriteToTrace("Ошибка при записи в лог: " + exp.ToString(), TraceMessageKind.Error);
            }
        }

        /// <summary>
        /// Записать событие в протокол обработки данных
        /// </summary>
        /// <param name="eventKind">Вид событий</param>
        /// <param name="infoMsg">Информационное сообщение</param>
        /// <param name="ex">Исключение. Если не null, то в лог дописывается сообщение исключения, а в трейс - стек</param>
        protected void WriteEventIntoProcessDataProtocol(ProcessDataEventKind eventKind, string infoMsg, Exception ex)
        {
            WriteEventIntoProcessDataProtocol(eventKind, this.PumpID, this.SourceID, infoMsg, ex);
        }

        /// <summary>
        /// Записать событие в протокол обработки данных
        /// </summary>
        /// <param name="eventKind">Вид событий</param>
        /// <param name="infoMsg">Информационное сообщение</param>
        protected void WriteEventIntoProcessDataProtocol(ProcessDataEventKind eventKind, string infoMsg)
        {
            WriteEventIntoProcessDataProtocol(eventKind, infoMsg, null);
        }

        /// <summary>
        /// Записать событие в протокол обработки данных
        /// </summary>
        /// <param name="eventKind">Вид событий</param>
        /// <param name="infoMsg">Информационное сообщение</param>
        /// <param name="showProgress">Вывести сообщение на прогресс</param>
        protected void WriteEventIntoProcessDataProtocol(ProcessDataEventKind eventKind, string infoMsg,
            bool showProgress)
        {
            WriteEventIntoProcessDataProtocol(eventKind, infoMsg, null);
            SetProgress(-1, -1, infoMsg, string.Empty, true);
        }

        #endregion Логи обработки данных


        #region Логи удаления данных

        /// <summary>
        /// Записать событие в протокол удаления данных
        /// </summary>
        /// <param name="eventKind">Тип события</param>
        /// <param name="pumpID">ИД закачки</param>
        /// <param name="sourceID">ИД источника</param>
        /// <param name="infoMsg">Информационное сообщение</param>
        /// <param name="ex">Исключение. Если не null, то в лог дописывается сообщение исключения, а в трейс - стек</param>
        protected void WriteEventIntoDeleteDataProtocol(DeleteDataEventKind eventKind, int pumpID, int sourceID, 
            string infoMsg, Exception ex)
        {
            try
            {
                TraceMessageKind traceMessageKind = TraceMessageKind.Information;
                string prefix = "\n";

                switch (eventKind)
                {
                    case DeleteDataEventKind.ddeCriticalError:
                    case DeleteDataEventKind.ddeError:
                    case DeleteDataEventKind.ddeFinishedWithErrors: 
                        traceMessageKind = TraceMessageKind.Error;
                        prefix = "\nТекст ошибки:\n";
                        break;

                    case DeleteDataEventKind.ddeWarning: 
                        traceMessageKind = TraceMessageKind.Warning;
                        break;
                }

                if (ex == null)
                {
                    WriteToTrace(infoMsg, traceMessageKind);
                    this.DeleteDataProtocol.WriteEventIntoDeleteDataProtocol(eventKind, infoMsg, pumpID, sourceID);
                }
                else
                {
                    WriteToTrace(infoMsg + prefix + ex.ToString(), traceMessageKind);
                    this.DeleteDataProtocol.WriteEventIntoDeleteDataProtocol(eventKind,
                        infoMsg + prefix + ex.Message, pumpID, sourceID);
                }
            }
            catch (Exception exp)
            {
                WriteToTrace("Ошибка при записи в лог: " + exp.ToString(), TraceMessageKind.Error);
            }
        }

        /// <summary>
        /// Записать событие в протокол удаления данных
        /// </summary>
        /// <param name="eventKind">Тип события</param>
        /// <param name="infoMsg">Информационное сообщение</param>
        /// <param name="ex">Исключение. Если не null, то в лог дописывается сообщение исключения, а в трейс - стек</param>
        protected void WriteEventIntoDeleteDataProtocol(DeleteDataEventKind eventKind, string infoMsg, Exception ex)
        {
            WriteEventIntoDeleteDataProtocol(eventKind, this.PumpID, this.SourceID, infoMsg, ex);
        }

        /// <summary>
        /// Записать событие в протокол удаления данных
        /// </summary>
        /// <param name="eventKind">Тип события</param>
        /// <param name="infoMsg">Информационное сообщение</param>
        protected void WriteEventIntoDeleteDataProtocol(DeleteDataEventKind eventKind, string infoMsg)
        {
            WriteEventIntoDeleteDataProtocol(eventKind, infoMsg, null);
        }

        /// <summary>
        /// Записать событие в протокол удаления данных
        /// </summary>
        /// <param name="eventKind">Тип события</param>
        /// <param name="infoMsg">Информационное сообщение</param>
        /// <param name="showProgress">Вывести сообщение на прогресс</param>
        protected void WriteEventIntoDeleteDataProtocol(DeleteDataEventKind eventKind, string infoMsg,
            bool showProgress)
        {
            WriteEventIntoDeleteDataProtocol(eventKind, infoMsg, null);
            SetProgress(-1, -1, infoMsg, string.Empty, true);
        }

        #endregion Логи удаления данных

        #endregion Функции для работы с логами


        #region Реализация IDataPumpModule

        #region Реализация методов

        /// <summary>
        /// Удалить данные закачки по ИД закачки и/или источника (-1 - игнорировать)
        /// </summary>
        /// <param name="pumpID">ИД закачки</param>
        /// <param name="sourceID">ИД источника</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public string DeleteData(int pumpID, int sourceID)
		{
            stateChangeProcessingEvent.Reset();

            Mutex mutex = null;

            LogicalCallContextData userContext = LogicalCallContextData.GetContext();

            try
            {
                LogicalCallContextData.SetContext(this.Context);

                if (Thread.CurrentThread.Name == null)
                {
                    Thread.CurrentThread.Name = this.ProgramIdentifier + " DeleteThread";
                }

                WriteToTrace(string.Format("Запрос на владение мьютексом {0}.", GetCurrentPumpMutexName()),
                    TraceMessageKind.Information);

                // Делаем текущий поток владельцем мьютекса
                mutex = GetCurrentPumpMutex();
                mutex.WaitOne();

                WriteToTrace("Поток удаления данных закачки владеет мьютексом.", TraceMessageKind.Information);

                this.state = PumpProcessStates.DeleteData;
                this.ServerSideDataPumpProgress.OnPumpProcessStateChanged(PumpProcessStates.Prepared, this.state);

                WriteToTrace("Инициализация и подготовка данных для удаления...", TraceMessageKind.Information);

                WriteEventIntoDeleteDataProtocol(DeleteDataEventKind.ddeStart, pumpID, sourceID, 
                    "Старт удаления данных.", null);

                this.Scheme.UsersManager.CheckPermissionForSystemObject(
                    this.ProgramIdentifier, (int)DataPumpOperations.DeletePump, true);

                Dictionary<int, string> dataSources = null;

                if (sourceID > 0)
                {
                    dataSources = new Dictionary<int, string>(1);
                    dataSources.Add(sourceID, string.Empty);
                }
                else
                {
                    dataSources = GetAllPumpedDataSources(pumpID);
                }

                InitDBObjects();
                this.PumpID = pumpID;

                BeginTransaction();

                int i = 1;
                foreach (KeyValuePair<int, string> dataSource in dataSources)
                {
                    WriteToTrace(string.Format("Удаление данных источника ID {0}...", dataSource.Key), 
                        TraceMessageKind.Information);
                    
                    SetProgress(dataSources.Count, i,
                        string.Format("Удаление данных источника ID {0}...", dataSource.Key),
                        string.Format("Источник {0} из {1}", i, dataSources.Count), true);

                    this.DataSource = GetDataSourceBySourceID(dataSource.Key);

                    DirectDeleteData(this.PumpID, dataSource.Key, string.Empty);

                    // Удаляем данные логов
                    DirectDeleteProtocolData(this.PumpID, dataSource.Key);

                    WriteToTrace(string.Format(
                        "Удаление данных источника ID {0} завершено.", dataSource.Key), TraceMessageKind.Information);

                    i++;
                }

                // Чистим историю
                if (GetAllPumpedDataSources(pumpID).Count == 0)
                    ClearPumpHistory(pumpID);

                WriteEventIntoDeleteDataProtocol(DeleteDataEventKind.ddeSuccefullFinished, pumpID, sourceID, 
                    "Удаление данных успешно завершено.", null);

                CommitTransaction();

                return string.Empty;
            }
            catch (ThreadAbortException)
            {
                WriteEventIntoDeleteDataProtocol(DeleteDataEventKind.ddeFinishedWithErrors, pumpID, sourceID, 
                    "Операция прервана пользователем.", null);

                RollbackTransaction();

                return "Операция прервана пользователем.";
            }
            catch (Exception ex)
            {
                WriteEventIntoDeleteDataProtocol(DeleteDataEventKind.ddeFinishedWithErrors, pumpID, sourceID,
                    "Удаление данных завершено с ошибками", ex);

                RollbackTransaction();

                return ex.ToString();
            }
            finally
            {
                this.state = PumpProcessStates.Finished;
                this.ServerSideDataPumpProgress.OnPumpProcessStateChanged(PumpProcessStates.DeleteData, this.state);

                try
                {
                    if (this.StagesQueue.GetInProgressQueueElement() == null &&
                        this.StagesQueue.GetLastExecutedQueueElement() == null)
                    {
                        WriteToTrace("DataPumpModuleBase.DeleteData: освобождение ресурсов.", 
                            TraceMessageKind.Information);
                        DisposeProperties();
                    }
                }
                finally
                {
                    LogicalCallContextData.SetContext(userContext);

                    if (mutex != null)
                    {
                        mutex.ReleaseMutex();
                    }

                    stateChangeProcessingEvent.Set();
                }
            }
		}

		#endregion Реализация методов


		#region Реализация свойств

		/// <summary>
		/// Состояние процесса закачка 
		/// </summary>
		public PumpProcessStates State
		{
			get 
            { 
                return state; 
            }
			set 
			{
                stateChangeProcessingEvent.WaitOne();

                WriteToTrace("Переключение контекста и этапа закачки.", TraceMessageKind.Information);
                LogicalCallContextData userContext = LogicalCallContextData.GetContext();

                try
                {
                    LogicalCallContextData.SetContext(this.Context);

                    //OnPauseTimerElapsed(null);

                    if (this.State == PumpProcessStates.Prepared)
                    {
                        SetStagesStateBeforeStage(value, StageState.OutOfQueue);
                    }

                    if (value == PumpProcessStates.Aborted &&
                        !this.Scheme.UsersManager.CheckPermissionForSystemObject(
                            this.ProgramIdentifier, (int)DataPumpOperations.StopPump, false))
                    {
                        WriteToTrace("Попытка остановить закачку отклонена за отсутствием прав.", 
                            TraceMessageKind.Warning, "DataPumpModuleBase");
                        return;
                    }

                    //WriteToTrace("State.Set to " + this.DataPumpInfo.PumpProcessStatesToString(value), TraceMessageKind.Information);

                    state = value;
                    stateChangedEvent.Set();
                }
                finally
                {
                    LogicalCallContextData.SetContext(userContext);
                }
			}
		}

		/// <summary>
		/// Уникальный идентификатор модуля закачки
		/// </summary>
        public string ProgramIdentifier
		{
			get 
            { 
                return programIdentifier; 
            }
			set 
            { 
                programIdentifier = value; 
            }
		}

        /// <summary>
        /// Уникальный идентификатор модуля закачки
        /// </summary>
        public PumpProgramID PumpProgramID
        {
            get
            {
                return pumpProgramID;
            }
            set
            {
                pumpProgramID = value;
            }
        }

        /// <summary>
        /// Версия системы
        /// </summary>
        public string SystemVersion
        {
            get
            {
                return systemVersion;
                //return AppVersionControl.GetAssemblyBaseVersion(AppVersionControl.GetServerLibraryVersion());
            }
            set
            {
                systemVersion = value;
            }
        }

		/// <summary>
		/// Версия программы закачки
		/// </summary>
		public string ProgramVersion
		{
			get 
            {
                return programVersion;
                //if (this.ProgramIdentifier == string.Empty) return this.SystemVersion;

                //return AppVersionControl.GetAssemblyVersion(Assembly.GetAssembly(
                //    Type.GetType(string.Format("Krista.FM.Server.DataPumps.{0}.{0}Module", this.ProgramIdentifier)))); 
            }
			set 
            { 
                programVersion = value; 
            }
		}

		/// <summary>
		/// XML настройки программы закачки
		/// </summary>
        public string ProgramConfig
		{
			get 
            {
                return this.PumpRegistryElement.ProgramConfig;
            }
		}

		/// <summary>
		/// Интерфейс для доступа к объектам схемы
		/// </summary>
		public IScheme Scheme
		{
			get 
            { 
                return scheme; 
            }
			set 
			{ 
				if (value == null) throw new ArgumentNullException("Scheme is null");
				scheme = value;
			}
		}

		/// <summary>
		/// true - выгрузка модуля после выполнения одного этапа, иначе - после всех
		/// </summary>
		public bool AutoSuicide
		{
			get 
            { 
                return autoSuicide; 
            }
			set 
            { 
                autoSuicide = value; 
            }
		}

        /// <summary>
        /// Если установлено, то закачка ждет завершения текущего этапа и ждет явного указания (через State)
        /// следующего
        /// </summary>
        public bool Locked
        {
            get
            {
                return locked;
            }

            set
            {
                locked = value;
            }
        }

        /// <summary>
        /// ИД сессии закачки
        /// </summary>
        public string SessionID
        {
            get
            {
                return sessionID;
            }
        }

		#endregion Реализация свойств

		#endregion Реализация IDataPumpModule
    }
}
