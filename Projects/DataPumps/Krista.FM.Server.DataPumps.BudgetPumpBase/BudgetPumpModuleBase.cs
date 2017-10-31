using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Diagnostics;
using System.IO;
using System.Text;

using Krista.FM.Common;
using Krista.FM.Server.Common;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.DataPumps
{
	/// <summary>
	/// Главный класс закачки данных бюджета. Управляет закачками из отдельных блоков.
	/// </summary>
    public partial class BudgetPumpModuleBase : DataPumpModuleBase
    {
        #region Поля
        
        private Database budgetDB = null;
        private List<string> supportedVersions = new List<string>();
		private BudgetDataPumpMode pumpMode;
        public string dateMin;
        public string dateMax;
        public bool useDateConstraint;
        private string databasePath = string.Empty;
		private SortedList budgetTablesID;
		private string currentDBVersion = string.Empty;
        private int majorDBVersion = -1;
        private int minorDBVersion = -1;
        private List<string> budgetRefs = new List<string>();
        private int budgetRef = -1;
		private int budgetYear = -1;
		private string innFO = string.Empty;
        private FileInfo udlFile;
        private bool isOleDbConnection = true;

        #endregion Поля


        #region Структуры, перечисления

        /// <summary>
        /// Режим закачки
        /// </summary>
        public enum BudgetDataPumpMode
        {
            /// <summary>
            /// Полная закачка
            /// </summary>
            Full = 0,

            /// <summary>
            /// Обновление
            /// </summary>
            Update = 1,

            /// <summary>
            /// Полная закачка фактов, обновление классификаторов
            /// </summary>
            FullFact = 2
        }

        /// <summary>
        /// Название СУБД, к которой подключены
        /// </summary>
        protected enum BudgetDBMSName
        {
            /// <summary>
            /// Interbase
            /// </summary>
            Interbase,

            /// <summary>
            /// Oracle
            /// </summary>
            Oracle
        }

        /// <summary>
        /// Константы АС Бюджет
        /// </summary>
        public enum BudgetConst
        {
            /// <summary>
            /// Текущий бюджет
            /// </summary>
            CurrentBudget,

            /// <summary>
            /// Код ошибочных доходов
            /// </summary>
            ErroneousIncomesCode,

            /// <summary>
            /// ИНН организации-финоргана
            /// </summary>
            FOOrgINN,

            /// <summary>
            /// Уведомления по варианту росписи
            /// </summary>
            IncomesVariantNotify,

            /// <summary>
            /// Суммы в тысячах или рублях (для плана доходов)
            /// </summary>
            IncomesSumFactor,

            /// <summary>
            /// Вариант росписи
            /// </summary>
            IncomesVariant,

            /// <summary>
            /// Суммы в тысячах или рублях (для плана расходов)
            /// </summary>
            OutcomesSumFactor,

            /// <summary>
            /// Ввод бюджета по месяцам (доходы)
            /// </summary>
            IncomesByMonths,

            /// <summary>
            /// Ввод бюджета по месяцам (расходы)
            /// </summary>
            OutcomesByMonths
        }

        #endregion Структуры, перечисления


        #region Инициализация

        /// <summary>
        /// Конструктор
        /// </summary>
        public BudgetPumpModuleBase() 
            : base()
		{
            budgetTablesID = new SortedList(50);
            budgetTablesID.Add("DocTypes", 1);
            budgetTablesID.Add("KFSR", 2);
            budgetTablesID.Add("KVSR", 3);
            budgetTablesID.Add("KCSR", 4);
            budgetTablesID.Add("KVR", 5);
            budgetTablesID.Add("KESR", 6);
            budgetTablesID.Add("KESRSUBCODE", 7);
            budgetTablesID.Add("FinType", 8);
            budgetTablesID.Add("Fact", 9);
            budgetTablesID.Add("NotifyTypes", 10);
            budgetTablesID.Add("PayTypeCls", 12);
            budgetTablesID.Add("MeansType", 13);
            budgetTablesID.Add("PaySymbolCls", 14);
            budgetTablesID.Add("TYPEOPERATIONCLS", 15);
            budgetTablesID.Add("BuhPaymentCls", 16);
            budgetTablesID.Add("RejectCls", 17);
            budgetTablesID.Add("RegionCLS", 18);
            budgetTablesID.Add("FACIALACC_CLS", 19);
            budgetTablesID.Add("Banks", 20);
            budgetTablesID.Add("ORG_ACCOUNTS", 21);
            budgetTablesID.Add("FacialFinDetail", 22);
            budgetTablesID.Add("FacialFinCaption", 23);
            budgetTablesID.Add("PayDoc", 24);
            budgetTablesID.Add("FinDocCaption", 24);
            budgetTablesID.Add("FinDocDetail", 24);
            budgetTablesID.Add("LimitFinDetail", 25);
            budgetTablesID.Add("BudgetData", 26);
            budgetTablesID.Add("BudNotify", 26);
            budgetTablesID.Add("Organizations", 27);
            budgetTablesID.Add("KD", 80);
            budgetTablesID.Add("MeasurementCls", 81);
            budgetTablesID.Add("OKDP", 82);
            budgetTablesID.Add("State_Types", 87);
            budgetTablesID.Add("DocumentTypes", 88);
            budgetTablesID.Add("State_DocStates", 89);
            budgetTablesID.Add("OKATO", 116);
            budgetTablesID.Add("InnerFinSource", 145);
            budgetTablesID.Add("Incomes32", 161);
        }

        #endregion Инициализация


        #region Свойства класса

        /// <summary>
        /// Объект БД АС Бюджет
        /// </summary>
        public Database BudgetDB
        {
            get { return budgetDB; }
            set { budgetDB = value; }
        }

        /// <summary>
        /// Режим закачки
        /// </summary>
        public BudgetDataPumpMode PumpMode
        {
            get { return pumpMode; }
            set { pumpMode = value; }
        }

        /// <summary>
        /// Путь до базы, из которой качаем
        /// </summary>
        protected string DatabasePath
        {
            get { return databasePath; }
            set { databasePath = value; }
        }

        protected string CurrentDBVersion
        {
            get { return currentDBVersion; }
            set { currentDBVersion = value; }
        }

        protected int MajorDBVersion
        {
            get { return majorDBVersion; }
            set { majorDBVersion = value; }
        }

        protected int MinorDBVersion
        {
            get { return minorDBVersion; }
            set { minorDBVersion = value; }
        }

        protected int BudgetRef
        {
            get { return budgetRef; }
            set { budgetRef = value; }
        }

        protected List<string> BudgetRefs
        {
            get { return budgetRefs; }
            set { budgetRefs = value; }
        }

        /// <summary>
        /// Год бюджета
        /// </summary>
        protected int BudgetYear
        {
            get { return budgetYear; }
            set { budgetYear = value; }
        }

        /// <summary>
        /// ИНН ФО
        /// </summary>
        protected string InnFO
        {
            get { return innFO; }
            set { innFO = value; }
        }

        /// <summary>
        /// Файл UDL, через который подключаемся к базе источника
        /// </summary>
        protected FileInfo UdlFile
        {
            get { return udlFile; }
            set { udlFile = value; }
        }

        /// <summary>
        /// Подключение к базе через ODBC или OLE DB
        /// </summary>
        protected bool IsOleDbConnection
        {
            get { return isOleDbConnection; }
            set { isOleDbConnection = value; }
        }

        /// <summary>
        /// Поддерживаемые версии АС Бюджет
        /// </summary>
        public List<string> SupportedVersions
        {
            get
            {
                if (supportedVersions == null)
                {
                    supportedVersions = new List<string>();
                }

                if (supportedVersions.Count == 0)
                {
                    // Поддерживаемые версии АС Бюджет
                    supportedVersions.Add("27.02");
                    supportedVersions.Add("28.00");
                    supportedVersions.Add("29.01");
                    supportedVersions.Add("29.02");
                    supportedVersions.Add("30.00");
                    supportedVersions.Add("30.01");
                    supportedVersions.Add("31.00");
                    supportedVersions.Add("31.01");
                    supportedVersions.Add("32.02");
                    supportedVersions.Add("32.04");
                    supportedVersions.Add("32.05");
                    supportedVersions.Add("32.07");
                    supportedVersions.Add("32.08");
                    supportedVersions.Add("32.09");
                    supportedVersions.Add("32.10");
                    supportedVersions.Add("33.00");
                    supportedVersions.Add("33.01");
                    supportedVersions.Add("33.02");
                    supportedVersions.Add("33.03");
                    supportedVersions.Add("34.00");
                    supportedVersions.Add("34.01");
                    supportedVersions.Add("34.02");
                    supportedVersions.Add("35.00");
                    supportedVersions.Add("35.01");
                    supportedVersions.Add("36.00");
                    supportedVersions.Add("36.01");
                    supportedVersions.Add("37.00");
                    supportedVersions.Add("37.01");
                    supportedVersions.Add("37.02");
                    supportedVersions.Add("37.03");
                }

                return supportedVersions;
            }
        }

        #endregion Свойства класса


		#region Общие функции

        /// <summary>
        /// Освобождает ресурсы, занятые свойствами класса
        /// </summary>
        protected override void DisposeProperties()
        {
            if (budgetDB != null)
            {
                budgetDB.Dispose();
                budgetDB = null;
            }
            if (supportedVersions != null)
                supportedVersions.Clear();

            base.DisposeProperties();
        }

		/// <summary>
		/// Функция подключения к базе
		/// </summary>
        /// <param name="udlFile">Файл удл со строкой подключения к базе</param>
		/// <returns>Объект БД</returns>
		protected Database ConnectToDatabase(FileInfo udlFile)
		{
			IDbConnection connection = null;

            this.UdlFile = udlFile;
            string errString = UDLFileDataAccess.GetConnectionFromUdl(this.UdlFile.FullName, ref connection, ref this.isOleDbConnection);

            if (connection == null)
            {
                throw new Exception("Невозможно установить соединение с источником данных. " +
                    "Проверьте настройку подключения к базе АС Бюджет. (" + errString +")");
            }

            if (!this.isOleDbConnection)
            {
                return new Database(connection, 
                    DbProviderFactories.GetFactory("System.Data.Odbc"), false, constCommandTimeout);
            }
            else
            {
                return new Database(connection,
                    DbProviderFactories.GetFactory("System.Data.OleDb"), false, constCommandTimeout);
            }
		}

		/// <summary>
		/// Инициализация объекта базы бюджета
		/// </summary>
        /// <param name="udlFile">Файл удл со строкой подключения к базе</param>
        protected bool InitBudgetDB(FileInfo udlFile)
		{
            if (!udlFile.Exists) return false;

            budgetDB = ConnectToDatabase(udlFile);
			if (budgetDB == null) return false;

			databasePath = UDLFileDataAccess.GetConnectionParam(budgetDB.Connection, "DATABASE");
            if (databasePath == string.Empty)
            {
                databasePath = UDLFileDataAccess.GetConnectionParam(budgetDB.Connection, "DBQ");
            }
            if (databasePath == string.Empty)
            {
                databasePath = UDLFileDataAccess.GetConnectionParam(budgetDB.Connection, "Data Source");
            }

			// Выбираем последнюю запись из таблицы версий бюджета и проверяем, есть ли эта версия 
			// в списке поддерживаемых
			currentDBVersion = Convert.ToString(budgetDB.ExecQuery(
				"select version from databaseversion order by major desc, minor desc",
				QueryResultTypes.Scalar)).Replace(',', '.').Trim();
            majorDBVersion = Convert.ToInt32(budgetDB.ExecQuery(
                "select Cast(major as Varchar(10)) major2 from databaseversion order by major desc, minor desc", QueryResultTypes.Scalar));
            minorDBVersion = Convert.ToInt32(budgetDB.ExecQuery(
                "select Cast(minor as Varchar(10)) minor2 from databaseversion order by major desc, minor desc", QueryResultTypes.Scalar));
			// Текущий бюджет
            string query = string.Empty;
            budgetRefs.Clear();
            if (majorDBVersion >= 35)
            {
                budgetYear = this.DataSource.Year;
                query = string.Format("select cast(id as varchar(10)) id from BUDGETS_S where aYear = {0}", budgetYear);
                DataTable dt = (DataTable)this.BudgetDB.ExecQuery(query, QueryResultTypes.DataTable);
                foreach (DataRow row in dt.Rows)
                    budgetRefs.Add(row["Id"].ToString());
            }
            else
            {
                budgetRefs.Add(GetBudgetConst(BudgetConst.CurrentBudget, "0"));
                query = string.Format("select cast(year as varchar(10)) year from BUDGETS_S where ID = {0}", budgetRefs[0]);
                budgetYear = Convert.ToInt32(this.BudgetDB.ExecQuery(query, QueryResultTypes.Scalar));
            }
			// Получаем ИНН организации-финоргана.
            innFO = GetBudgetConst(BudgetConst.FOOrgINN, string.Empty);
			return true;
		}

		/// <summary>
		/// Получает параметры закачки
		/// </summary>
		/// <returns>Признак ошибки</returns>
		protected bool GetPumpParams()
		{
			try
			{
                if (Convert.ToBoolean(GetParamValueByName(
                    this.PumpRegistryElement.ProgramConfig, "rbtnFull", "false")))
                {
                    pumpMode = BudgetDataPumpMode.Full;
                }
                else if (Convert.ToBoolean(GetParamValueByName(
                    this.PumpRegistryElement.ProgramConfig, "rbtnUpdate", "false")))
                {
                    pumpMode = BudgetDataPumpMode.Update;
                }
                else if (Convert.ToBoolean(GetParamValueByName(
                    this.PumpRegistryElement.ProgramConfig, "rbtnFullFact", "false")))
                {
                    pumpMode = BudgetDataPumpMode.FullFact;
                }
                else
                {
                    pumpMode = BudgetDataPumpMode.Full; 
                }
                dateMin = GetParamValueByName(this.PumpRegistryElement.ProgramConfig, "mePeriodMin", string.Empty).Replace(".", "");
                dateMax = GetParamValueByName(this.PumpRegistryElement.ProgramConfig, "mePeriodMax", string.Empty).Replace(".", "");
                useDateConstraint = Convert.ToBoolean(GetParamValueByName(this.PumpRegistryElement.ProgramConfig, "cbUseDateConstraint", string.Empty));
				return true;
			}
			catch
			{
				return false;
			}
		}

		/// <summary>
		/// Удаляет из классификатора данных все записи, которых теперь нет в исходной таблице АС Бюджет
		/// </summary>
		/// <param name="tableName">Наименование таблицы бюджета</param>
		/// <param name="sourceTable">Таблица классификатора бюджета</param>
		/// <param name="destTable">Таблица классификатора данных</param>
		/// <param name="removedRecsCount">Количество удаленных записей</param>
        protected void RemoveAbsentRecsFromCls(string tableName, DataTable sourceTable, DataTable destTable, 
			out int removedRecsCount)
		{
			removedRecsCount = 0;

            // Удаление записей временно убрано, т.к. возникают проблемы с иерархией классификаторов.
            return;

            #region UNUSED
            /*
            // Удалям из классификатора данных все записи, которых теперь нет в исходной таблице АС Бюджет
            if (budgetTablesID.ContainsKey(tableName))
			{
				DataTable dt = (DataTable)this.BudgetDB.ExecQuery(
					string.Format("select RECORDID from DELRECINDEXES where TABLEID = {0}",
					budgetTablesID[tableName]), QueryResultTypes.DataTable);

				for (int i = 0; i < dt.Rows.Count; i++)
				{
					DataRow[] rows = destTable.Select(string.Format("SOURCEKEY = {0}", dt.Rows[i]["RECORDID"]));
					int count = rows.GetLength(0);
                    for (int j = 0; j < count; j++)
					{
						rows[j].Delete();
						removedRecsCount++;
					}
				}
			}
			else
			{
				// Проверяем все записи
				for (int i = destTable.Rows.Count - 1; i >= 0; i--)
				{
                    if (destTable.Rows[i].IsNull("SOURCEKEY"))
                    {
                        continue;
                    }

					DataRow[] rows = sourceTable.Select(string.Format("ID = {0}", destTable.Rows[i]["SOURCEKEY"]));
					if (rows.GetLength(0) == 0)
					{
						destTable.Rows[i].Delete();
						removedRecsCount++;
					}
				}
            }
            */
            #endregion
        }

		/// <summary>
		/// Возвращает строку для закачки. Сначала строка ищется среди закачанных, если ее там нет, то создается.
		/// </summary>
		/// <param name="sourceRowID">ИД исходной строки</param>
        /// <param name="destTable">Таблица фактов</param>
        /// <param name="cache">Кэш с закачанными данными (ключ - SOURCEKEY)</param>
        /// <param name="obj">Объект классификатора</param>
		/// <param name="isAdded">true - строка добавлена, иначе найдена уже закачанная</param>
		/// <returns>Собственно строка</returns>
		protected DataRow GetRowForUpdate(int sourceRowID, DataTable destTable, Dictionary<string, DataRow> cache,
            ClassTypes classType, string generatorName, out bool isAdded)
		{
			DataRow row = null;
			isAdded = false;

            bool tmpBool = false;
            //ClassTypes classType = obj.ClassType;

            if (classType == ClassTypes.clsFactData)
            {
                tmpBool = this.PumpMode == BudgetDataPumpMode.Update;
            }
            else
            {
                if (classType == ClassTypes.clsDataClassifier || classType == ClassTypes.clsFixedClassifier)
                {
                    // непонятно, почему бы всегда не искать запись
                    tmpBool = true;
                    //tmpBool = this.PumpMode == BudgetDataPumpMode.Update ||
                      //  this.PumpMode == BudgetDataPumpMode.FullFact;
                }
                else
                {
                    throw new Exception("Неизвестный тип классификатора.");
                }
            }

			// Если работаем в режиме обновления, то сначала поищем такую запись
			if (tmpBool)
			{
                if (cache != null)
                {
                    row = FindCachedRow(cache, new string[] { sourceRowID.ToString() });
                }
                else
                {
                    row = FindRow(destTable, new object[] { "SOURCEKEY", sourceRowID });
                }

				// Что нашли, то и возвращаем
				if (row != null)
				{
					isAdded = false;
					return row;
				}
			}

			// Добавляем строку, если она не найдена
			if (row == null)
			{
				row = destTable.NewRow();
                if (classType == ClassTypes.clsDataClassifier || classType == ClassTypes.clsFixedClassifier)
                {
                    row["ID"] = GetGeneratorNextValue(generatorName);
                }
                row["PUMPID"] = this.PumpID;
                if (row.Table.Columns.Contains("SOURCEID"))
                    row["SOURCEID"] = this.SourceID;
				destTable.Rows.Add(row);
				isAdded = true;
			}

			return row;
		}

        /// <summary>
        /// Формирование ограничения для запросов в зависимости от режима закачки
        /// </summary>
        /// <returns>Ограничение</returns>
        protected string GetDateConstr(string fieldPrefix, string fieldName)
        {
            string dateConstr = string.Empty;

            if (this.PumpMode == BudgetDataPumpMode.Update)
            {
                string lastTime = GetLastPumpDateBySourceID(this.SourceID);
                if (lastTime != string.Empty)
                {
                    switch (GetDbmsName())
                    {
                        case BudgetDBMSName.Interbase:
                            if (fieldName == string.Empty)
                            {
                                dateConstr = string.Format(
                                    " and ({0}CREATEDATE >= '{1}' or {0}UPDATEDATE >= '{1}')",
                                    fieldPrefix, CommonRoutines.GlobalDateToLongDate(lastTime));
                            }
                            else
                            {
                                dateConstr = string.Format(
                                    " and ({0}{1} >= '{2}')",
                                    fieldPrefix, fieldName, CommonRoutines.GlobalDateToLongDate(lastTime));
                            }
                            break;

                        case BudgetDBMSName.Oracle:
                            if (fieldName == string.Empty)
                            {
                                dateConstr = string.Format(
                                    " and (({0}CREATEDATE >= TO_DATE('{1}', 'DD.MM.YYYY HH24:MI:SS') or " +
                                    "{0}UPDATEDATE >= TO_DATE('{1}', 'DD.MM.YYYY HH24:MI:SS')))", 
                                    fieldPrefix, lastTime);
                            }
                            else
                            {
                                dateConstr = string.Format(
                                    " and ({0}(1) >= TO_DATE('{2}', 'DD.MM.YYYY HH24:MI:SS')",
                                    fieldPrefix, fieldName, lastTime);
                            }
                            break;
                    }
                }
            }

            return dateConstr;
        }

        /// <summary>
        /// Формирование ограничения для запросов в зависимости от режима закачки
        /// </summary>
        /// <returns>Ограничение</returns>
        protected string GetDateConstr(string fieldPrefix)
        {
            return GetDateConstr(fieldPrefix, string.Empty);
        }

        /// <summary>
        /// Формирование ограничения для запросов в зависимости от режима закачки для выборки из ДатаСета
        /// </summary>
        /// <returns>Ограничение</returns>
        protected string GetDateConstrForDataSet()
        {
            string dateConstr = string.Empty;

            if (this.PumpMode == BudgetDataPumpMode.Update)
            {
                string lastTime = GetLastPumpDateBySourceID(this.SourceID);
                if (lastTime != string.Empty)
                {
                    dateConstr = string.Format(
                        " (CREATEDATE >= '{0}' or UPDATEDATE >= '{0}')", 
                        CommonRoutines.GlobalDateToLongDate(lastTime));
                }
            }

            return dateConstr;
        }

        /// <summary>
        /// Возвращает нименование СУБД, к которой подключены
        /// </summary>
        /// <returns>Название СУБД</returns>
        protected BudgetDBMSName GetDbmsName()
        {
            if (!this.IsOleDbConnection)
            {
                OdbcConnection odbcCon = (OdbcConnection)this.BudgetDB.Connection;
                if (odbcCon.Driver.ToUpper() == "SQORA32.DLL") return BudgetDBMSName.Oracle;
            }
            else
            {
                OleDbConnection oledbCon = (OleDbConnection)this.BudgetDB.Connection;
                if (oledbCon.Provider.ToUpper() == "ORAOLEDB.ORACLE.1") return BudgetDBMSName.Oracle;
            }

            return BudgetDBMSName.Interbase;
        }

        /// <summary>
        /// Возвращает значение указанной константы бюджета
        /// </summary>
        /// <param name="budgetConst">Константа</param>
        /// <param name="defaultValue">Значение, возвращаемое в случае если константа отсутствует</param>
        /// <returns>Значение константы</returns>
        public string GetBudgetConst(BudgetConst budgetConst, string defaultValue)
        {
            string constValue = string.Empty;

            switch (budgetConst)
            {
                case BudgetConst.CurrentBudget:
                    constValue = Convert.ToString(budgetDB.ExecQuery(
                        "select cast(CONSTVALUE as varchar(10)) CONSTVALUE from INTCONST where PROGINDEX = -1 and CONSTINDEX = 3",
                        QueryResultTypes.Scalar));
                    break;

                case BudgetConst.ErroneousIncomesCode:
                    // константа - обсолетие, не юзается
                    constValue = string.Empty;
           //         constValue = Convert.ToString(this.BudgetDB.ExecQuery(
            //            "select cast(CONSTVALUE as varchar(30)) CONSTVALUE from VARCHARCONST where PROGINDEX = 3 and CONSTINDEX = 1",
             //           QueryResultTypes.Scalar)).Replace("_", string.Empty);
                    break;

                case BudgetConst.FOOrgINN:
                    constValue = Convert.ToString(budgetDB.ExecQuery(
                        "select cast(CONSTVALUE as varchar(10)) CONSTVALUE from REALCONST where PROGINDEX = -1 and CONSTINDEX = 0",
                        QueryResultTypes.Scalar));
                    break;

                case BudgetConst.IncomesSumFactor:
                    constValue = Convert.ToString(this.BudgetDB.ExecQuery(
                        "select cast(CONSTVALUE as varchar(10)) CONSTVALUE from INTCONST where PROGINDEX = 32 and CONSTINDEX = 11",
                        QueryResultTypes.Scalar));
                    break;

                case BudgetConst.IncomesVariant:
                    constValue = Convert.ToString(this.BudgetDB.ExecQuery(
                        "select cast(CONSTVALUE as varchar(10)) CONSTVALUE from INTCONST where PROGINDEX = 18 and CONSTINDEX = 1",
                        QueryResultTypes.Scalar));
                    break;

                case BudgetConst.IncomesVariantNotify:
                    constValue = Convert.ToString(this.BudgetDB.ExecQuery(
                        "select cast(CONSTVALUE as varchar(10)) CONSTVALUE from INTCONST where PROGINDEX = 32 and CONSTINDEX = 1",
                        QueryResultTypes.Scalar));
                    break;

                case BudgetConst.OutcomesSumFactor:
                    constValue = Convert.ToString(this.BudgetDB.ExecQuery(
                        "select cast(CONSTVALUE as varchar(10)) CONSTVALUE from INTCONST where PROGINDEX = 32 and CONSTINDEX = 10",
                        QueryResultTypes.Scalar));
                    break;

                case BudgetConst.IncomesByMonths:
                    constValue = Convert.ToString(this.BudgetDB.ExecQuery(
                        "select cast(CONSTVALUE as varchar(10)) CONSTVALUE from INTCONST where PROGINDEX = 32 and CONSTINDEX = 13",
                        QueryResultTypes.Scalar));
                    break;

                case BudgetConst.OutcomesByMonths:
                    constValue = Convert.ToString(this.BudgetDB.ExecQuery(
                        "select cast(CONSTVALUE as varchar(10)) CONSTVALUE from INTCONST where PROGINDEX = 32 and CONSTINDEX = 3",
                        QueryResultTypes.Scalar));
                    break;
            }

            if (constValue == string.Empty)
            {
                return defaultValue;
            }

            return constValue;
        }

		#endregion Общие функции
	}
}