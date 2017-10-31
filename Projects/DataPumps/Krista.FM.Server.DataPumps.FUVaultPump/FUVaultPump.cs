using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

using Krista.FM.Server.Common;
using Krista.FM.Server.DataPumps;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.DataPumps.FUVaultPump
{
	/// <summary>
	/// УФК_0002_СводФУ
	/// </summary>
	public class FUVaultPumpModule : CorrectedPumpModuleBase
    {
        #region Поля

        #region классификаторы

        // КД.УФК_Свод ФУ (d_KD_UFK2)
        private IDbDataAdapter daKdUfk2;
        private DataSet dsKdUfk2;
        private IClassifier clsKdUfk2;
        private Dictionary<string, int> kdCache = null;
        // ОКАТО.УФК_Свод ФУ (d_OKATO_UFK2)
        private IDbDataAdapter daOkatoUfk2;
        private DataSet dsOkatoUfk2;
        private IClassifier clsOkatoUfk2;
        private Dictionary<string, int> okatoUfk2Cache = null;
        // Администратор.УФК_Свод ФУ (d_KVSR_UFK2)
        private IDbDataAdapter daKvsrUfk2;
        private DataSet dsKvsrUfk2;
        private IClassifier clsKvsrUfk2;
        private Dictionary<string, int> kvsrCache = null;
        // Районы.УФК_Свод ФУ (d_Regions_UFK2)
        private IDbDataAdapter daRegionsUfk2;
        private DataSet dsRegionsUfk2;
        private IClassifier clsRegionsUfk2;
        private Dictionary<string, int> regionsUfk2Cache = null;
        // Документы.Ведомость Операция
        private IDbDataAdapter daDocList;
        private DataSet dsDocList;
        private IClassifier clsDocList;
        private Dictionary<string, int> docCache = null;
        // Местные бюджеты.УФК
        private IDbDataAdapter daLogBdgt;
        private DataSet dsLocBdgt;
        private IClassifier clsLocBdgt;
        private Dictionary<string, int> locBdgtCache = null;
        // Районы.Служебный для закачки (d_Regions_ForPump)
        private IDbDataAdapter daRegions;
        private DataSet dsRegions;
        private IClassifier clsRegions;
        private Dictionary<string, int> regionsCache = null;
        // Фиксированный.УФК_СводФУ_Районы (fx_FX_RegionsUFK2)
        private IDbDataAdapter daRegionsUfk2Fx;
        private DataSet dsRegionsUfk2Fx;
        private IClassifier fxcRegionsUFK2;
        private Dictionary<int, DataRow> fxcRegionsCache = null;

        #endregion классификаторы

        #region факты

        // Факт.УФК_Свод ФУ (f_F_BudgetIncomingsTula)
        private IDbDataAdapter daBudgetIncomings;
        private DataSet dsBudgetIncomings;
        private IFactTable fctBudgetIncomings;
        private Dictionary<string, DataRow> budgetCache;

        #endregion факты

		private IDbDataAdapter daSvodFU;
		private DataSet dsSvodFU;
		private IDbDataAdapter daRayon;
		private DataSet dsRayon;
		private IDbDataAdapter daKod;
		private DataSet dsKod;

        private DBDataAccess dbDataAccess = new DBDataAccess();

        private Database dbfDB = null;
        private ArrayList arTempDirectories = new ArrayList();
        private int docID = -1;
        private int ghost = -1;
        private int refLocBdgt = -1;
        private int totalFiles = 0;
        private int filesCount = 0;
        // переменные для восстановления каталогов "Справка" и "Ведомость" после их переноса в архив
        private DirectoryInfo sourceDir = null;
        private bool wasInfoSubdir = false;
        private bool wasSummarySubdir = false;
        
        private int regForPumpSourceID = -1;

        private int addedRowsCount = 0;
        private int zeroSumRowsCount = 0;
        private int badDateRowsCount = 0;
        private int skippedRowsCount = 0;

        private List<string> enabledKodDoh;

        private int year = -1;
        private int month = -1;

        private ReportType reportType;
        private Formats1nParser formats1nParser = new Formats1nParser();
        private ExcelHelper excelHelper = null;
        private object excelObj = null;

        private List<int> deletedDate = new List<int>();

        #endregion Поля

        #region Константы

        /// <summary>
        /// Ограничение для запрета удаления данных по заключительным оборотам
        /// </summary>
        private const string constOracleDisableDeletionFinalOverturnConstraint =
            "((mod(RefFKDay, 10000) <> 1232 and RefFKDay <> {0} and GHOST = 1) or " +
            "(mod(RefYearDayUNV, 10000) <> 1232 and RefYearDayUNV <> {0} and GHOST >= 2))";

        /// <summary>
        /// Ограничение для запрета удаления данных по заключительным оборотам
        /// </summary>
        private const string constSQLServerDisableDeletionFinalOverturnConstraint =
            "(((RefFKDay % 10000) <> 1232 and RefFKDay <> {0} and GHOST = 1) or " +
            "((RefYearDayUNV % 10000) <> 1232 and RefYearDayUNV <> {0} and GHOST >= 2))";

        /// <summary>
        /// Ограничение для выборки данных фактов по указанному месяцу
        /// </summary>
        private const string constOracleSelectFactDataByMonthConstraint =
            "((floor(mod(RefFKDay, 10000)) / 100 = {0} and GHOST = 1) or " +
            "(floor(mod(RefYearDayUNV, 10000) / 100) = {0} and GHOST >= 2))";

        /// <summary>
        /// Ограничение для выборки данных фактов по указанному месяцу
        /// </summary>
        private const string constSQLServerSelectFactDataByMonthConstraint =
            "((floor((RefFKDay % 10000)) / 100 = {0} and GHOST = 1) or " +
            "(floor((RefYearDayUNV % 10000) / 100) = {0} and GHOST >= 2))";

        private const string DOC_NAME_REFUND = "Возвращено платежей";
        private const string DOC_NAME_TRANSFER = "Перечислено в бюджет поступлений";
        private const string DOC_NAME_CASH = "Кассовые поступления";
        
        #endregion Константы

        #region Структуры и перечисления

        /// <summary>
        /// Формат файла отчета
        /// </summary>
        private enum FileFormat
        {
            /// <summary>
            /// Новый формат дбф
            /// </summary>
            NewDbf,
            /// <summary>
            /// Старый формат дбф
            /// </summary>
            OldDbf,
            /// <summary>
            /// Формат 1н
            /// </summary>
            Format1n,
            /// <summary>
            /// Формат 1н c 2009
            /// </summary>
            Format1nNew,
            /// <summary>
            /// Формат Excel
            /// </summary>
            Excel,
            /// <summary>
            /// Текстовый формат
            /// </summary>
            Txt,
            /// <summary>
            /// Текстовый формат с расширением .IPN
            /// </summary>
            TxtIpn,
            /// <summary>
            /// Архив ARJ
            /// </summary>
            Arj,
            /// <summary>
            /// Архив RAR
            /// </summary>
            Rar,
            /// <summary>
            /// ХЗ какой формат
            /// </summary>
            Unknown
        }

        /// <summary>
        /// Тип отчёта
        /// </summary>
        private enum ReportType
        {
            /// <summary>
            /// Справка
            /// </summary>
            Info,
            /// <summary>
            /// Ведомость
            /// </summary>
            Summary,
            /// <summary>
            /// Другие отчёты
            /// </summary>
            Other
        }

        #endregion Структуры и перечисления

        #region Делегаты

        // Функция закачки строки данных в таблицу фактов из файлов формата Excel
        private delegate void PumpXlsDataRow(double sum, int budgetLevel, int date, int regionId,
            int kdId, int okatoId, int kvsrId, FileFormat fileFormat);

        #endregion Делегаты

        #region Инициализация

        /// <summary>
        /// Конструктор
        /// </summary>
		public FUVaultPumpModule() 
            : base()
		{

		}

        /// <summary>
        /// Освобождение ресурсов
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (dbDataAccess != null) dbDataAccess.Dispose();
                if (formats1nParser != null) formats1nParser.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion Инициализация

        #region Закачка данных

        #region работа с базой и кэшами

        private void FillCaches()
        {
            FillRowsCache(ref kdCache, dsKdUfk2.Tables[0], "CodeStr");
            FillRowsCache(ref okatoUfk2Cache, dsOkatoUfk2.Tables[0], "Code");
            FillRowsCache(ref kvsrCache, dsKvsrUfk2.Tables[0], "CodeStr");
            FillRowsCache(ref regionsUfk2Cache, dsRegionsUfk2.Tables[0], new string[] { "Code", "Name" }, "|", "Id");
            FillRowsCache(ref fxcRegionsCache, dsRegionsUfk2Fx.Tables[0], "Code");
            FillRowsCache(ref docCache, dsDocList.Tables[0], "Name");
            FillRowsCache(ref locBdgtCache, dsLocBdgt.Tables[0], "Name");
            FillRowsCache(ref regionsCache, dsRegions.Tables[0], "Name");
        }

        protected override void QueryData()
        {
            InitClsDataSet(ref daKdUfk2, ref dsKdUfk2, clsKdUfk2, false, string.Empty);
            InitClsDataSet(ref daOkatoUfk2, ref dsOkatoUfk2, clsOkatoUfk2, false, string.Empty);
            InitClsDataSet(ref daKvsrUfk2, ref dsKvsrUfk2, clsKvsrUfk2, false, string.Empty);
            InitClsDataSet(ref daLogBdgt, ref dsLocBdgt, clsLocBdgt, false, string.Empty);
            InitClsDataSet(ref daRegionsUfk2, ref dsRegionsUfk2, clsRegionsUfk2, false, string.Empty);
            InitDataSet(ref daDocList, ref dsDocList, clsDocList, false, string.Empty, string.Empty);
            InitDataSet(ref daRegionsUfk2Fx, ref dsRegionsUfk2Fx, fxcRegionsUFK2, false, string.Empty, string.Empty);
            regForPumpSourceID = AddDataSource("ФО", "0006", ParamKindTypes.Year,
                string.Empty, this.DataSource.Year, 0, string.Empty, 0, string.Empty).ID;
            InitDataSet(ref daRegions, ref dsRegions, clsRegions, false,
                string.Format("SOURCEID = {0}", regForPumpSourceID), string.Empty);
            InitFactDataSet(ref daBudgetIncomings, ref dsBudgetIncomings, fctBudgetIncomings);
            FillCaches();
        }

        protected override void UpdateData()
        {
            UpdateDataSet(daKdUfk2, dsKdUfk2, clsKdUfk2);
            UpdateDataSet(daOkatoUfk2, dsOkatoUfk2, clsOkatoUfk2);
            UpdateDataSet(daRegionsUfk2, dsRegionsUfk2, clsRegionsUfk2);
            UpdateDataSet(daKvsrUfk2, dsKvsrUfk2, clsKvsrUfk2);
            UpdateDataSet(daDocList, dsDocList, clsDocList);
            UpdateDataSet(daLogBdgt, dsLocBdgt, clsLocBdgt);
            UpdateDataSet(daRegions, dsRegions, clsRegions);
            UpdateDataSet(daBudgetIncomings, dsBudgetIncomings, fctBudgetIncomings);
        }

        private const string FX_FX_REGIONS_UFK2_GUID = "24342296-3958-4f35-8e7a-4cd81866bd35";
        private const string D_KD_UFK2_GUID = "e171bc36-6247-4dd4-bbcf-cb76c35a7676";
        private const string D_REGIONS_UFK2_GUID = "827d8aae-df08-435f-bda6-9b1ddab087fe";
        private const string D_OKATO_UFK2_GUID = "588c73da-ee89-453a-8b61-0ae128f77cbb";
        private const string D_KVSR_UFK2_GUID = "3a6fe855-bee6-408c-867c-c57746fc200c";
        private const string D_DOC_LIST_GUID = "f31e70b3-3ce2-49bc-809c-96002469a216";
        private const string D_LOCBDGT_UFK_GUID = "adbbb96f-f22e-4884-a576-9f477733d010";
        private const string D_REGIONS_FOR_PUMP_GUID = "e9d2898d-fc2d-4626-834a-ed1ac98a1673";
        private const string F_F_BUDGET_INCOMINGS_TULA_GUID = "826d5da9-c9cc-4f8d-9432-d7357fda4677";
        protected override void InitDBObjects()
        {
            // Названия таблиц схемы
            fxcRegionsUFK2 = this.Scheme.Classifiers[FX_FX_REGIONS_UFK2_GUID];
            clsDocList = this.Scheme.Classifiers[D_DOC_LIST_GUID];

            this.UsedClassifiers = new IClassifier[] {
                clsKdUfk2 = this.Scheme.Classifiers[D_KD_UFK2_GUID],
                clsRegionsUfk2 = this.Scheme.Classifiers[D_REGIONS_UFK2_GUID],
                clsOkatoUfk2 = this.Scheme.Classifiers[D_OKATO_UFK2_GUID],
                clsKvsrUfk2 = this.Scheme.Classifiers[D_KVSR_UFK2_GUID],
                clsLocBdgt = this.Scheme.Classifiers[D_LOCBDGT_UFK_GUID] };

            this.AssociateClassifiersEx = new IClassifier[] {
                clsRegions = this.Scheme.Classifiers[D_REGIONS_FOR_PUMP_GUID] };

            this.UsedFacts = new IFactTable[] {
                fctBudgetIncomings = this.Scheme.FactTables[F_F_BUDGET_INCOMINGS_TULA_GUID] };
        }

        private void RecoverSubdirs()
        {
            DirectoryInfo[] info = sourceDir.GetDirectories(INFO_DIR_NAME, SearchOption.TopDirectoryOnly);
            if (wasInfoSubdir && (info.GetLength(0) == 0))
                sourceDir.CreateSubdirectory(INFO_DIR_NAME);
            DirectoryInfo[] summary = sourceDir.GetDirectories(SUMMARY_DIR_NAME, SearchOption.TopDirectoryOnly);
            if (wasSummarySubdir && (summary.GetLength(0) == 0))
                sourceDir.CreateSubdirectory(SUMMARY_DIR_NAME);
        }

        protected override void PumpFinalizing()
        {
            if (dbfDB != null)
                dbfDB.Close();

            ClearDataSet(ref dsBudgetIncomings);
            ClearDataSet(ref dsKdUfk2);
            ClearDataSet(ref dsRegionsUfk2);
            ClearDataSet(ref dsRegionsUfk2Fx);
            ClearDataSet(ref dsOkatoUfk2);
            ClearDataSet(ref dsSvodFU);
            ClearDataSet(ref dsRayon);
            ClearDataSet(ref dsKvsrUfk2);
            ClearDataSet(ref dsKod);
            ClearDataSet(ref dsDocList);
            ClearDataSet(ref dsLocBdgt);
            ClearDataSet(ref dsRegions);

            RecoverSubdirs();
        }

        #endregion работа с базой и кэшами

        #region Общие функции

        /// <summary>
        /// Добавляет строку в таблицу фактов
        /// </summary>
        /// <param name="sum">Сумма</param>
        /// <param name="budgetLevel">Уровень бюджета</param>
        /// <param name="date">Дата</param>
        /// <param name="regionID">ИД района</param>
        /// <param name="kdID">ИД КД</param>
        /// <param name="okatoID">ИД ОКАТО</param>
        /// <param name="kvsrID">ИД КВСР</param>
        /// <param name="fileFormat">Формат файла</param>
        private DataRow AddRow(double sum, int budgetLevel, int date, int regionID, int kdID, int okatoID, int kvsrID,
            FileFormat fileFormat)
        {
            if (ghost != 3)
            {
                if (sum == 0)
                {
                    zeroSumRowsCount++;
                    return null;
                }
            }

            DataRow row = dsBudgetIncomings.Tables[0].NewRow();

            row["PUMPID"] = this.PumpID;
            row["SOURCEID"] = this.SourceID;
            row["SOURCEKEY"] = DBNull.Value;
            row["REFKD"] = kdID;
            row["REFOKATO"] = okatoID;
            row["REFREGIONS"] = regionID;
            row["REFBUDGETLEVELS"] = budgetLevel;
            row["REFKVSR"] = kvsrID;
            if (reportType == ReportType.Info)
                row["FROMBEGINYEAR"] = sum;
            else
                row["SUMME"] = sum;
            row["DOCCOUNT"] = 1;
            row["REFYEARDAYUNV"] = date;
            row["REFFKDAY"] = date;
            row["GHOST"] = ghost;
            if (date >= 20090101)
                row["REFDOC"] = docID;
            if (ghost == 4)
                row["REFLOCBDGTUFK"] = refLocBdgt;

            dsBudgetIncomings.Tables[0].Rows.Add(row);
            addedRowsCount++;
            return row;
        }

        /// <summary>
        /// Возвращает ограничение на выборку согласно типу базы
        /// </summary>
        /// <returns>Ограничение</returns>
        private string GetDisableDeletionFinalOverturnConstraint()
        {
            switch (this.ServerDBMSName)
            {
                case DBMSName.SQLServer:
                    return constSQLServerDisableDeletionFinalOverturnConstraint;

                default:
                    return constOracleDisableDeletionFinalOverturnConstraint;
            }
        }

        /// <summary>
        /// Удаление данных за текущую дату
        /// </summary>
        /// <param name="date">Дата</param>
        private void DeleteDataByReportDate(int date)
        {
            if (!deletedDate.Contains(date))
            {
                int overturnDate = date;
                if (!this.FinalOverturn)
                {
                    overturnDate = 0;
                }

                if (ghost >= 2)
                    DeleteData(string.Format("RefYearDayUNV = {0} and {1} and GHOST = {2}",
                        date, string.Format(GetDisableDeletionFinalOverturnConstraint(), overturnDate), ghost),
                        string.Format("Дата отчета: {0}.", date));
                else
                    DeleteData(string.Format("RefFKDay = {0} and {1} and GHOST = {2}",
                        date, string.Format(GetDisableDeletionFinalOverturnConstraint(), overturnDate), ghost),
                        string.Format("Дата отчета: {0}.", date));

                deletedDate.Add(date);
            }
        }

        /// <summary>
        /// Определяет формат файла отчета
        /// </summary>
        /// <param name="file">Файл</param>
        /// <returns>Формат файла</returns>
        private FileFormat GetFileFormat(FileInfo file)
        {
            string extension = file.Extension.ToUpper();
            string filename = file.Name.ToUpper();
            if (extension == ".FFF" || filename == "SVODFU.DBF")
                return FileFormat.NewDbf;
            else if (filename.StartsWith(string.Format("MNI{0}", this.DataSource.Year)) ||
                     filename.StartsWith(string.Format("UFK{0}", this.DataSource.Year)))
                return FileFormat.OldDbf;
            else if (extension.StartsWith(".CE"))
                return FileFormat.Format1n;
            else if (extension.StartsWith(".BD"))
                return FileFormat.Format1nNew;
            else if (extension == ".XLS")
                return FileFormat.Excel;
            else if (extension == ".SDI")
                return FileFormat.Txt;
            else if (extension.StartsWith(".IPN"))
                return FileFormat.TxtIpn;
            else if (extension == ".ARJ")
                return FileFormat.Arj;
            else if (extension == ".RAR")
                return FileFormat.Rar;
            return FileFormat.Unknown;
        }

        private void SetGhostValue(FileFormat fileFormat)
        {
            if (fileFormat == FileFormat.Format1n)
                ghost = 2;
            else if (reportType == ReportType.Info)
                ghost = 3;
            else if ((reportType == ReportType.Summary) || (fileFormat == FileFormat.TxtIpn))
                ghost = 4;
            else if (this.DataSource.Year >= 2010 && this.Region == RegionName.Tula)
                ghost = 2;
            else
                ghost = 1;
        }

        #endregion Общие функции

        #region закачка dbf

        /// <summary>
        /// Проверяет правильность названия файла
        /// </summary>
        /// <param name="fileName">Название файла</param>
        private string CheckNewFileName(FileInfo fileName)
        {
            // TMДNNOOO.ППП - формат названия файла.
            // T – признак типа посылки. Имеет значение «B», что означает посылка с платежными документами, своды по области
            if (!fileName.Name.StartsWith("B"))
                return string.Format("Ошибка при проверке имени файла {0}: неверный признак типа посылки.", fileName.FullName);

            // M – месяц. Имеет значение 1-9, А,В,С (в 36-ричной системе)
            if (CommonRoutines.Numeration36To10(fileName.Name[1]) > 12)
                return string.Format("Ошибка при проверке имени файла {0}: неверный номер месяца.", fileName.FullName);

            // Д – день в месяце. Имеет значение 1-9, А-V (36-ричной системе)
            if (CommonRoutines.Numeration36To10(fileName.Name[2]) > 31)
                return string.Format("Ошибка при проверке имени файла {0}: неверный номер дня месяца.", fileName.FullName);

            // NN – номер посылки за день в 36-ричной системе

            // ООО – код отправителя. Имеет значение 983
            if (fileName.Name.Substring(5, 3) != "983")
                return string.Format("Ошибка при проверке имени файла {0}: неверный код отправителя.", fileName.FullName);

            // ППП – код получателя. Имеет значение FFF.
            if (fileName.Extension != ".FFF")
                return string.Format("Ошибка при проверке имени файла {0}: неверный код получателя.", fileName.FullName);

            // Шаблон имени посылки обеспечивает уникальность имени посылки в течение года.
            return string.Empty;
        }
        
        /// <summary>
        /// Закачивает КД
        /// </summary>
        /// <param name="fileFormat">Формат файла</param>
        /// <param name="kd">Код КД</param>
        /// <returns>ИД закачанной записи</returns>
        private int PumpKD(FileFormat fileFormat, string kd)
        {
            string kdName = string.Empty;

            switch (fileFormat)
            {
                case FileFormat.NewDbf:
                    kdName = kd;
                    break;

                case FileFormat.OldDbf:
                    if (dsKod.Tables.Count > 0)
                    {
                        DataRow row = FindRow(dsKod.Tables[0], new object[] { "XXXX", kd });
                        if (row != null)
                        {
                            kdName = GetStringCellValue(row, "XXXXXXXXXX", string.Empty);
                        }
                    }
                    break;
            }

            if (kdName == string.Empty)
            {
                kdName = kd;
            }

            return PumpCachedRow(kdCache, dsKdUfk2.Tables[0], clsKdUfk2, kd,
                new object[] { "CODESTR", kd, "NAME", kdName });
        }

        /// <summary>
        /// Закачивает район
        /// </summary>
        /// <param name="fileFormat">Формат файла</param>
        /// <param name="region">Код района</param>
        /// <returns>ИД закачанной записи</returns>
        private int PumpRegion(FileFormat fileFormat, int region)
        {
            string regionName = string.Empty;

            switch (fileFormat)
            {
                case FileFormat.NewDbf:
                    DataRow row = FindCachedRow(fxcRegionsCache, region);
                    if (row != null)
                    {
                        regionName = GetStringCellValue(row, "NAME", string.Empty);
                    }
                    break;

                case FileFormat.OldDbf:
                    row = FindRow(dsRayon.Tables[0], new object[] { "XXXXX_0", region });
                    if (row != null)
                    {
                        regionName = GetStringCellValue(row, "XXXXX_1", string.Empty);
                    }
                    break;
            }

            if (regionName == string.Empty)
            {
                regionName = region.ToString();
            }

            return PumpOriginalRow(dsRegionsUfk2, clsRegionsUfk2,
                new object[] { "CODE", region, "NAME", regionName });
        }

        /// <summary>
        /// Добавляет строку в таблицу фактов
        /// </summary>
        /// <param name="dbfRow">Исходная строка</param>
        /// <param name="sum">Сумма</param>
        /// <param name="budgetLevel">Уровень бюджета</param>
        /// <param name="kdField">Наименования поля КД</param>
        /// <param name="okatoField">Наименования поля ОКАТО (пустое - значит 0)</param>
        /// <param name="regionField">Наименования поля Район</param>
        /// <param name="dateField">Наименования поля даты</param>
        /// <param name="kvsrField">Наименования поля КВСР</param>
        /// <param name="fileFormat">Формат файла</param>
        private void ProcessDbfRow(DataRow dbfRow, double sum, int budgetLevel, string kdField, string okatoField,
            string regionField, string dateField, string kvsrField, FileFormat fileFormat)
        {
            if (sum == 0)
            {
                zeroSumRowsCount++;
                return;
            }

            // Проверяем дату
            int date = Convert.ToInt32(CommonRoutines.GlobalShortDateToNewDate(Convert.ToString(dbfRow[dateField])));

            // Удаление данных за текущую дату
            DeleteDataByReportDate(date);

            switch (fileFormat)
            {
                case FileFormat.NewDbf:
                    if (date / 10000 != this.DataSource.Year)
                    {
                        badDateRowsCount++;
                        return;
                    }
                    // Если установлен параметр "Закачка заключительных оборотов", то закачиваемый файл должен закачаться 
                    // со ссылкой на фиксированный Период.День по элементу "Заключительные обороты" предыдущего года.
                    if (this.FinalOverturn)
                    {
                        date = this.FinalOverturnDate;
                    }
                    break;

                case FileFormat.OldDbf:
                    if (this.FinalOverturn || date / 10000 != this.DataSource.Year)
                    {
                        date = this.FinalOverturnDate;
                    }
                    break;
            }

            // Обработка района
            int region = GetIntCellValue(dbfRow, regionField, 0);
            if (region == 28)
            {
                skippedRowsCount++;
                return;
            }
            int regionID = PumpRegion(fileFormat, region);

            // Добавляем КД, если такого еще нет
            string kd = Convert.ToString(dbfRow[kdField]);
            if (kd == string.Empty || kd.Replace("0", string.Empty).Length == 0)
            {
                return;
            }
            int kdID = PumpKD(fileFormat, kd);

            // Добавляем OKATO, если такого еще нет
            string okato = string.Empty;
            if (okatoField == string.Empty)
            {
                okato = "0";
            }
            else
            {
                okato = GetStringCellValue(dbfRow, okatoField, "0");
            }
            int okatoID = PumpCachedRow(okatoUfk2Cache, dsOkatoUfk2.Tables[0], clsOkatoUfk2, okato,
                new object[] { "CODE", okato });

            // Добавляем КВСР, если такого еще нет
            string kvsr = string.Empty;
            if (kvsrField == string.Empty)
            {
                kvsr = "0";
            }
            else
            {
                kvsr = GetStringCellValue(dbfRow, kvsrField, "0");
            }
            int kvsrID = PumpOriginalRow(dsKvsrUfk2, clsKvsrUfk2, new object[] { "CODESTR", kvsr });

            AddRow(sum, budgetLevel, date, regionID, kdID, okatoID, kvsrID, fileFormat);
        }
        
        /// <summary>
        /// Закачивает файл старого формата
        /// </summary>
        /// <param name="file">Файл</param>
        /// <param name="this.DataSource.Year">Год (из имени каталога)</param>
        private void PumpOldFormatFile(FileInfo file)
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStartFilePumping,
                string.Format("Старт закачки файла {0}.", file.FullName));

            string progressMessage = string.Format(
                "Обработка файла {0} ({1} из {2})", file.Name, filesCount, totalFiles);

            // Проверяем, есть ли в каталоге файл с районами
            if (file.Directory.GetFiles("rayon.dbf", SearchOption.TopDirectoryOnly).GetLength(0) == 0)
            {
                throw new PumpDataFailedException("RAYON.DBF не найден.");
            }

            // Проверяем, есть ли в каталоге файл с наименованиями КД
            if (file.Directory.GetFiles(string.Format("kod{0}.dbf", this.DataSource.Year),
                SearchOption.TopDirectoryOnly).GetLength(0) == 0)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, string.Format("KOD{0}.DBF не найден.", this.DataSource.Year));
            }

            // Выбираем данные
            SetProgress(0, 0, "Запрос данных...", string.Empty);
            InitDataSet(dbfDB, ref daSvodFU, ref dsSvodFU, "SELECT * FROM " + file.Name);

            // Есть ли данные в этом файле
            if (dsSvodFU.Tables[0].Rows.Count == 0)
            {
                throw new PumpDataFailedException(string.Format("Файл {0} пуст.", file.FullName));
            }

            // Закачиваем
            for (int i = 0; i < dsSvodFU.Tables[0].Rows.Count; i++)
            {
                try
                {
                    double sum = 0;
                    if (dsSvodFU.Tables[0].Columns.Contains("X_4"))
                    {
                        sum = GetDoubleCellValue(dsSvodFU.Tables[0].Rows[i], "X_4", 0);
                    }
                    else
                    {
                        sum = GetDoubleCellValue(dsSvodFU.Tables[0].Rows[i], "X_0", 0) +
                            GetDoubleCellValue(dsSvodFU.Tables[0].Rows[i], "X_1", 0) +
                            GetDoubleCellValue(dsSvodFU.Tables[0].Rows[i], "X_2", 0) +
                            GetDoubleCellValue(dsSvodFU.Tables[0].Rows[i], "X_3", 0);
                    }

                    switch (file.Name.Substring(0, 3).ToUpper())
                    {
                        case "UFK":
                            ProcessDbfRow(dsSvodFU.Tables[0].Rows[i],
                                GetDoubleCellValue(dsSvodFU.Tables[0].Rows[i], "X_0", 0), 1,
                                "XXXX_1", string.Empty, "XXXXX", "XXXX_0", string.Empty, FileFormat.OldDbf);

                            ProcessDbfRow(dsSvodFU.Tables[0].Rows[i],
                                GetDoubleCellValue(dsSvodFU.Tables[0].Rows[i], "X_1", 0), 3,
                                "XXXX_1", string.Empty, "XXXXX", "XXXX_0", string.Empty, FileFormat.OldDbf);

                            ProcessDbfRow(dsSvodFU.Tables[0].Rows[i],
                                GetDoubleCellValue(dsSvodFU.Tables[0].Rows[i], "X_2", 0), 5,
                                "XXXX_1", string.Empty, "XXXXX", "XXXX_0", string.Empty, FileFormat.OldDbf);

                            ProcessDbfRow(dsSvodFU.Tables[0].Rows[i],
                                GetDoubleCellValue(dsSvodFU.Tables[0].Rows[i], "X_3", 0), 7,
                                "XXXX_1", string.Empty, "XXXXX", "XXXX_0", string.Empty, FileFormat.OldDbf);

                            ProcessDbfRow(dsSvodFU.Tables[0].Rows[i], sum, 0, "XXXX_1", string.Empty,
                                "XXXXX", "XXXX_0", string.Empty, FileFormat.OldDbf);

                            break;

                        case "MNI":
                            ProcessDbfRow(dsSvodFU.Tables[0].Rows[i],
                                GetDoubleCellValue(dsSvodFU.Tables[0].Rows[i], "X_0", 0), 1,
                                "XXXX_1", string.Empty, "XXXXX", "XXXX_0", string.Empty, FileFormat.OldDbf);

                            ProcessDbfRow(dsSvodFU.Tables[0].Rows[i],
                                GetDoubleCellValue(dsSvodFU.Tables[0].Rows[i], "X_1", 0), 3,
                                "XXXX_1", string.Empty, "XXXXX", "XXXX_0", string.Empty, FileFormat.OldDbf);

                            ProcessDbfRow(dsSvodFU.Tables[0].Rows[i],
                                GetDoubleCellValue(dsSvodFU.Tables[0].Rows[i], "X_2", 0), 5,
                                "XXXX_1", string.Empty, "XXXXX", "XXXX_0", string.Empty, FileFormat.OldDbf);

                            ProcessDbfRow(dsSvodFU.Tables[0].Rows[i],
                                GetDoubleCellValue(dsSvodFU.Tables[0].Rows[i], "X_3", 0), 7,
                                "XXXX_1", string.Empty, "XXXXX", "XXXX_0", string.Empty, FileFormat.OldDbf);

                            ProcessDbfRow(dsSvodFU.Tables[0].Rows[i], sum, 0, "XXXX_1", string.Empty,
                                "XXXXX", "XXXX_0", string.Empty, FileFormat.OldDbf);

                            break;
                    }
                }
                catch (ThreadAbortException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new PumpDataFailedException(string.Format("{0}, строка {1}", ex.Message, i));
                }

                SetProgress(dsSvodFU.Tables[0].Rows.Count, i + 1, progressMessage,
                    string.Format("Запись {0} из {1}", i + 1, dsSvodFU.Tables[0].Rows.Count));
            }
        }

        /// <summary>
        /// Закачивает файл нового формата
        /// </summary>
        /// <param name="file">Файл</param>
        /// <returns>Строка ошибки</returns>
        private void PumpNewFormatFile(FileInfo file)
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStartFilePumping,
                string.Format("Старт закачки файла {0}.", file.FullName));

            Database db = null;
            string svodfuDir = string.Empty;
            string progressMessage = string.Format(
                "Обработка файла {0} ({1} из {2})", file.Name, filesCount, totalFiles);

            try
            {
                // Если это заархивированный файл, то распаковываем
                if (file.Extension.ToUpper() == ".FFF")
                {
                    // Проверим корректность названия файла
                    string err = CheckNewFileName(file);
                    if (err != string.Empty)
                    {
                        throw new Exception(err);
                    }

                    // Разорхивируем файл
                    svodfuDir = CommonRoutines.ExtractArchiveFile(file.FullName, file.Directory.FullName, ArchivatorName.Arj,
                        FilesExtractingOption.SeparateSubDirs);

                    // Проверим каталог с разорхивированными данными
                    if (!Directory.Exists(svodfuDir))
                    {
                        throw new PumpDataFailedException(
                            string.Format("Каталог с разорхивированными данными не найден ({0}).", svodfuDir));
                    }

                    // Проверяем, есть ли в этом архиве че-нить
                    if (Directory.GetFiles(svodfuDir, "svodfu.dbf", SearchOption.TopDirectoryOnly).GetLength(0) == 0)
                    {
                        throw new PumpDataFailedException(
                            string.Format("Каталог {0} не содержит данных для закачки.", svodfuDir));
                    }

                    // Подключаемся к источнику
                    dbDataAccess.ConnectToDataSource(ref db, svodfuDir, ODBCDriverName.Microsoft_dBase_Driver);
                }
                else
                {
                    db = dbfDB;
                }

                if (db == null)
                {
                    throw new PumpDataFailedException("Ошибка при создании соединения с источником.");
                }

                // Выбираем данные
                InitDataSet(db, ref daSvodFU, ref dsSvodFU, "SELECT * FROM SVODFU.DBF");

                // Есть ли данные в этом файле
                if (dsSvodFU.Tables[0].Rows.Count == 0)
                {
                    throw new PumpDataFailedException(string.Format("Файл {0} пуст.", file.FullName));
                }

                // Закачиваем
                for (int i = 0; i < dsSvodFU.Tables[0].Rows.Count; i++)
                {
                    try
                    {
                        ProcessDbfRow(dsSvodFU.Tables[0].Rows[i],
                            GetDoubleCellValue(dsSvodFU.Tables[0].Rows[i], "QQ1", 0), 1,
                            "NUM", "OKATO", "NOM", "DATA", "INN", FileFormat.NewDbf);

                        ProcessDbfRow(dsSvodFU.Tables[0].Rows[i],
                            GetDoubleCellValue(dsSvodFU.Tables[0].Rows[i], "QQ2", 0), 3,
                            "NUM", "OKATO", "NOM", "DATA", "INN", FileFormat.NewDbf);

                        ProcessDbfRow(dsSvodFU.Tables[0].Rows[i],
                            GetDoubleCellValue(dsSvodFU.Tables[0].Rows[i], "QQ3", 0), 5,
                            "NUM", "OKATO", "NOM", "DATA", "INN", FileFormat.NewDbf);

                        ProcessDbfRow(dsSvodFU.Tables[0].Rows[i],
                            GetDoubleCellValue(dsSvodFU.Tables[0].Rows[i], "QQ4", 0), 7,
                            "NUM", "OKATO", "NOM", "DATA", "INN", FileFormat.NewDbf);

                        ProcessDbfRow(dsSvodFU.Tables[0].Rows[i],
                            GetDoubleCellValue(dsSvodFU.Tables[0].Rows[i], "QQS", 0), 0,
                            "NUM", "OKATO", "NOM", "DATA", "INN", FileFormat.NewDbf);
                    }
                    catch (ThreadAbortException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        throw new PumpDataFailedException(string.Format("{0}, строка {1}", ex.Message, i));
                    }

                    SetProgress(dsSvodFU.Tables[0].Rows.Count, i + 1, progressMessage,
                        string.Format("Запись {0} из {1}", i + 1, dsSvodFU.Tables[0].Rows.Count));
                }

                UpdateData();
                ClearDataSet(daBudgetIncomings, ref dsBudgetIncomings);
            }
            finally
            {
                if (db != null)
                {
                    db.Close();
                    db = null;
                }

                CommonRoutines.DeleteExtractedDirectories(file.Directory);
            }
        }

        /// <summary>
        /// Закачивает в датасеты дбф с данными по районам и наименованиям КД
        /// </summary>
        /// <param name="dir">Каталог с файлами ДБФ</param>
        private void PumpServiceTables(DirectoryInfo dir)
        {
            ClearDataSet(ref dsKod);
            ClearDataSet(ref dsRayon);

            // Проверяем, есть ли в каталоге файл с районами
            dsRayon.Tables.Clear();
            if (dir.GetFiles("rayon.dbf", SearchOption.TopDirectoryOnly).GetLength(0) > 0)
            {
                // Выбираем данные по районам
                InitLocalDataAdapter(dbfDB, ref daRayon, "SELECT * FROM RAYON.DBF");
                dsRayon.Tables.Clear();
                daRayon.Fill(dsRayon);
            }

            // Проверяем, есть ли в каталоге файл с наименованиями КД
            dsKod.Tables.Clear();
            if (dir.GetFiles(string.Format("kod{0}.dbf", dir.Name),
                SearchOption.TopDirectoryOnly).GetLength(0) > 0)
            {
                // Выбираем данные по районам
                InitLocalDataAdapter(
                    dbfDB, ref daKod, string.Format("SELECT * FROM KOD{0}.DBF", dir.Name));
                dsKod.Tables.Clear();
                daKod.Fill(dsKod);
            }
        }

        #endregion закачка dbf

        #region закачка архивов (rar и arj)

        /// <summary>
        /// Закачка файла формата екселя из архива ARJ
        /// </summary>
        /// <param name="file">Файл</param>
        private void PumpArjFile(FileInfo archFile)
        {
            DirectoryInfo tempDir = CommonRoutines.GetTempDir(archFile.Directory);
            try
            {
                string output = string.Empty;
                CommonRoutines.ExtractARJ(archFile.FullName, tempDir.FullName, out output);
                PumpFiles(tempDir, SearchOption.AllDirectories);
            }
            finally
            {
                CommonRoutines.DeleteDirectory(tempDir);
            }
        }

        /// <summary>
        /// Закачка файла формата екселя из архива RAR
        /// </summary>
        /// <param name="file">Файл</param>
        private void PumpRarFile(FileInfo archFile)
        {
            DirectoryInfo tempDir = CommonRoutines.GetTempDir(archFile.Directory);
            try
            {
                string output = string.Empty;
                CommonRoutines.ExtractRar(archFile.FullName, tempDir.FullName, out output);
                PumpFiles(tempDir, SearchOption.AllDirectories);
            }
            finally
            {
                CommonRoutines.DeleteDirectory(tempDir);
            }
        }

        #endregion закачка архивов (rar и arj)

        #region закачка excel

        /// <summary>
        /// Получить значение ОКАТО
        /// </summary>
        /// <param name="sheet"></param>
        /// <returns></returns>
        private string GetOkato(object sheet)
        {
            // с 24.10.2007 - окато в ячейке J6
            string okato = excelHelper.GetCell(sheet, "L6").Value;
            if (okato == string.Empty)
                okato = excelHelper.GetCell(sheet, "J6").Value;
            if (okato == string.Empty)
                okato = "0";
            return okato;
        }

        /// <summary>
        /// Получить уровень бюджета по номеру столбца
        /// </summary>
        /// <param name="curColumn">Номер столбца</param>
        /// <param name="columnsChange">Признак изменённого порядка столбцов</param>
        /// <returns>Уровень бюджета</returns>
        private int GetBudgetLevel(int curColumn, bool columnsChange)
        {
            if (curColumn > 10)
                return 3;
            else if (curColumn == 3)
                return 0;
            else if (curColumn == 4)
                return 1;
            else if (curColumn == 5)
                return 3;
            else if (curColumn == 6)
            {
                if (columnsChange)
                    return 5;
                return 6;
            }
            else if (curColumn == 7)
            {
                if (columnsChange)
                    return 6;
                return 5;
            }
            else if (curColumn == 8)
                return 15;
            else if (curColumn == 9)
                return 7;
            else if (curColumn == 10)
                return 12;
            return 0;
        }

        /// <summary>
        /// Возвращает дату отчета
        /// </summary>
        /// <param name="sheet">Страница отчета</param>
        /// <returns>Дата</returns>
        private int GetReportDate(object sheet)
        {
            string date = string.Empty;
            if (reportType == ReportType.Summary)
            {
                date = excelHelper.GetCell(sheet, "G4").Value;
            }
            else if (this.DataSource.Year >= 2010)
            {
                date = excelHelper.GetCell(sheet, "AN7").Value;
            }
            else if (this.DataSource.Year >= 2009)
            {
                date = excelHelper.GetCell(sheet, "K4").Value;
                // с 23.06.2009 - дата в ячейке J4
                if (date == string.Empty)
                    date = excelHelper.GetCell(sheet, "J4").Value;
            }
            else
            {
                date = excelHelper.GetCell(sheet, "L4").Value;
                // с 24.10.2007 - дата в ячейке J4
                if (date == string.Empty)
                    date = excelHelper.GetCell(sheet, "J4").Value;
            }
            if (date.Length > 10)
                date = date.Substring(0, 10);
            return CommonRoutines.ShortDateToNewDate(date);
        }

        /// <summary>
        /// Возвращает нижнюю границу отчета на странице
        /// </summary>
        /// <param name="sheet">Страница</param>
        private bool GetSheetMargins(object sheet, ref int top, ref int bottom)
        {
            top = -1;
            int tmp = -1;
            for (int i = 1; i <= 50000; i++)
            {
                string value = excelHelper.GetCell(sheet, i, 1).Value.ToUpper();
                if (string.Compare(value, "1") == 0)
                {
                    do
                    {
                        i++;
                    }
                    while (string.IsNullOrEmpty(excelHelper.GetCell(sheet, i, 1).Value) && i <= 50000);

                    top = i;
                }
                else if ((this.DataSource.Year >= 2010) && (this.Region == RegionName.Tula))
                {
                    if ((top != -1) && (value == string.Empty))
                    {
                        bottom = i;
                        return true;
                    }
                }
                else if (value.StartsWith("ИТОГО ПО КОДУ"))
                {
                    tmp = i;
                }
                else if ((string.Compare(value, "ИТОГО") == 0) || (string.Compare(value, "ВСЕГО") == 0))
                {
                    bottom = tmp;
                    return true;
                }
            }

            return false;
        }

        private bool IsIgnoreRow(string cellValue)
        {
            return (cellValue == string.Empty ||
                cellValue.ToUpper().StartsWith("ИТОГО") ||
                cellValue.ToUpper() == "БЮДЖЕТ ТУЛЬСКОЙ ОБЛАСТИ");
        }

        private void PumpXlsRow(double sum, int budgetLevel, int date, int regionId, int kdId, int okatoId,
            int kvsrId, FileFormat fileFormat)
        {
            AddRow(sum, budgetLevel, date, regionId, kdId, okatoId, kvsrId, fileFormat);
        }

        private void PumpXlsRowTula(double sum, int budgetLevel, int date, int regionId, int kdId, int okatoId,
            int kvsrId, FileFormat fileFormat)
        {
            string key = string.Format("{0}|{1}|{2}", kdId, okatoId, date);
            if (budgetCache.ContainsKey(key))
            {
                budgetCache[key]["FROMBEGINYEAR"] = sum + Convert.ToDouble(budgetCache[key]["FROMBEGINYEAR"]);
            }
            else
            {
                DataRow row = AddRow(sum, budgetLevel, date, regionId, kdId, okatoId, kvsrId, fileFormat);
                budgetCache.Add(key, row);
            }
        }

        /// <summary>
        /// Закачка файла формата екселя
        /// </summary>
        /// <param name="file">Файл</param>
        private int[] COLUMNS_NUM_DEFAULT = new int[] { 3, 4, 5, 6, 7, 8, 9, 10 };
        private int[] COLUMNS_NUM_TULA_2010 = new int[] { 21 };
        private void PumpXlsSheet(object sheet, int sheetIndex, string sheetName, PumpXlsDataRow pumpDataRow)
        {
            int top = 0;
            int bottom = 0;
            refLocBdgt = -1;
            if (!GetSheetMargins(sheet, ref top, ref bottom))
            {
                throw new Exception(string.Format("Не найдена нижняя граница страницы {0} отчета.", sheetIndex));
            }
            else
            {
                // Период.День
                int date = GetReportDate(sheet);

                // Если установлен параметр "Закачка заключительных оборотов", то закачиваемый файл должен закачаться 
                // со ссылкой на фиксированный Период.День по элементу "Заключительные обороты" предыдущего года.
                if (this.FinalOverturn)
                {
                    date = this.FinalOverturnDate;
                }

                CheckDataSourceByDate(date, true);

                // Удаление данных за текущую дату
                DeleteDataByReportDate(date);

                int okatoID = -1;
                int regionID = -1;
                int columnKd = -1;
                int[] columnsNum;
                if ((this.DataSource.Year >= 2010) && (this.Region == RegionName.Tula))
                {
                    docID = PumpCachedRow(docCache, dsDocList.Tables[0], clsDocList,
                        DOC_NAME_TRANSFER, new object[] { "NAME", DOC_NAME_TRANSFER });
                    columnsNum = COLUMNS_NUM_TULA_2010;
                    columnKd = 1;
                }
                else
                {
                    regionID = PumpOriginalRow(dsRegionsUfk2, clsRegionsUfk2, new object[] {
                        "CODE", CommonRoutines.TrimLetters(sheetName),
                        "NAME", excelHelper.GetCell(sheet, "B5").Value,
                        "OKATO", GetOkato(sheet) });
                    okatoID = PumpCachedRow(okatoUfk2Cache, dsOkatoUfk2.Tables[0], clsOkatoUfk2, "0",
                        new object[] { "CODE", 0 });
                    columnsNum = COLUMNS_NUM_DEFAULT;
                    columnKd = 2;
                }

                int kvsrID = PumpOriginalRow(dsKvsrUfk2, clsKvsrUfk2, new object[] { "CODESTR", 0 });

                // с 01.09.2007, колонки F и G "меняются" уровнями бюджета, f - на 5, g - на 6
                bool columnsChange = (this.DataSource.Year >= 2008) ||
                    ((this.DataSource.Year >= 2007) && (date % 10000 / 100 >= 9));

                for (int i = top; i <= bottom; i++)
                {
                    try
                    {
                        string name = excelHelper.GetCell(sheet, i, 1).Value;
                        if (string.IsNullOrEmpty(name) || name.ToUpper().StartsWith("ИТОГО"))
                            continue;

                        // Cумма по строке 'Ведомость учета возвратов (возмещений)' сумма должна 
                        // браться с минусом
                        bool negativeSum = string.Compare(name, "Ведомость учета возвратов (возмещений)", true) == 0;

                        string kd = CommonRoutines.TrimLetters(excelHelper.GetCell(sheet, i, columnKd).Value);
                        int kdID = PumpCachedRow(kdCache, dsKdUfk2.Tables[0], clsKdUfk2, kd,
                            new object[] { "CODESTR", kd, "NAME", constDefaultClsName });

                        if ((this.DataSource.Year >= 2010) && (this.Region == RegionName.Tula))
                        {
                            string okato = CommonRoutines.TrimLetters(excelHelper.GetCell(sheet, i, 9).Value.Trim());
                            if (okato == string.Empty)
                                okato = "0";
                            regionID = PumpOriginalRow(dsRegionsUfk2, clsRegionsUfk2, new object[] {
                                "CODE", 0, "NAME", constDefaultClsName, "OKATO", okato });
                            okatoID = PumpCachedRow(okatoUfk2Cache, dsOkatoUfk2.Tables[0], clsOkatoUfk2, okato,
                                new object[] { "CODE", okato });
                        }
                        else if (date >= 20090101)
                        {
                            docID = PumpCachedRow(docCache, dsDocList.Tables[0], clsDocList, name, new object[] { "NAME", name });
                        }

                        // Качаем суммы
                        for (int j = 0; j < columnsNum.GetLength(0); j++)
                        {
                            double sum;
                            if (this.DataSource.Year == 2009)
                                sum = Convert.ToDouble(excelHelper.GetCell(sheet, i, columnsNum[j] + 1).Value.PadLeft(1, '0'));
                            else
                                sum = Convert.ToDouble(excelHelper.GetCell(sheet, i, columnsNum[j]).Value.PadLeft(1, '0'));

                            if (negativeSum)
                                sum = -sum;

                            if ((columnsNum[j] == 10) && (date >= 20090623))
                                continue;
                            int budgetLevel = GetBudgetLevel(columnsNum[j], columnsChange);

                            pumpDataRow(sum, budgetLevel, date, regionID, kdID, okatoID, kvsrID, FileFormat.Excel);
                        }

                        /*if (dsBudgetIncomings.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
                        {
                            UpdateData();
                            ClearDataSet(daBudgetIncomings, ref dsBudgetIncomings);
                        }*/
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(string.Format(
                            "При обработке строки {0} возникла ошибка ({1})", i, ex.Message), ex);
                    }
                }
            }
        }

        private void PumpXlsSheetSummary(object sheet)
        {
            int date = GetReportDate(sheet);
            if (this.FinalOverturn)
                date = this.FinalOverturnDate;
            CheckDataSourceByDate(date, true);
            // Удаление данных за текущую дату
            DeleteDataByReportDate(date);

            int okatoID = PumpOriginalRow(dsOkatoUfk2, clsOkatoUfk2, new object[] { "CODE", 0 });
            docID = PumpOriginalRow(dsDocList, clsDocList, new object[] { "NAME", DOC_NAME_CASH });
            int kvsrID = PumpOriginalRow(dsKvsrUfk2, clsKvsrUfk2, new object[] { "CODESTR", 0 });

            bool toPumpRow = false;
            for (int curRow = 1; ; curRow++)
            {
                try
                {
                    string cellValue = excelHelper.GetCell(sheet, curRow, 1).Value.Trim();
                    if (IsIgnoreRow(cellValue))
                        continue;

                    if (cellValue.ToUpper().StartsWith("ВСЕГО"))
                        return;

                    if (toPumpRow)
                    {
                        string kd = excelHelper.GetCell(sheet, curRow, 2).Value.Trim();
                        int kdID = PumpCachedRow(kdCache, dsKdUfk2.Tables[0], clsKdUfk2, kd,
                            new object[] { "CODESTR", kd, "NAME", constDefaultClsName });

                        refLocBdgt = PumpCachedRow(locBdgtCache, dsLocBdgt.Tables[0], clsLocBdgt, cellValue,
                            new object[] { "ACCOUNT", 0, "NAME", cellValue, "OKATO", 0 });

                        int regionID = PumpOriginalRow(dsRegionsUfk2, clsRegionsUfk2, new object[] {
                            "CODE", 0, "NAME", cellValue, "OKATO", 0 });

                        PumpCachedRow(regionsCache, dsRegions.Tables[0], clsRegions, cellValue,
                            new object[] { "ACCOUNT", 0, "NAME", cellValue, "OKATO", 0, "SOURCEID", regForPumpSourceID });

                        cellValue = excelHelper.GetCell(sheet, curRow, 3).Value.Trim().Replace('.', ',');
                        double sum = Convert.ToDouble(cellValue);
                        AddRow(sum, 0, date, regionID, kdID, okatoID, kvsrID, FileFormat.Excel);
                        continue;
                    }

                    if (cellValue.ToUpper() == "1")
                    {
                        toPumpRow = true;
                        continue;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format(
                        "При обработке строки {0} возникла ошибка ({1})", curRow, ex.Message), ex);
                }
            }
        }

        /// <summary>
        /// Закачка отчётов в формате XLS
        /// </summary>
        /// <param name="file"></param>
        private void PumpXlsFormat(FileInfo file)
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStart, "Старт инициализации Excel.");
            excelHelper = new ExcelHelper();
            excelObj = excelHelper.OpenExcel(false);
            object workbook = excelHelper.GetWorkbook(excelObj, file.FullName, false);
            try
            {
                string dataSourcePath = GetShortSourcePathBySourceID(this.SourceID);
                int count = excelHelper.GetSheetCount(workbook);
                for (int sheetIndex = 1; sheetIndex <= count; sheetIndex++)
                {
                    object sheet = excelHelper.GetSheet(workbook, sheetIndex);
                    string sheetName = excelHelper.GetSheetName(sheet);
                    SetProgress(count, sheetIndex,
                        string.Format("Обработка файла {0}\\{1}...", dataSourcePath, file.Name),
                        string.Format("Станица {0} из {1}", sheetIndex, count));
                    WriteToTrace(string.Format(
                        "Обработка страницы {0} ({1}) отчета.", sheetIndex, sheetName), TraceMessageKind.Information);

                    if (reportType == ReportType.Summary)
                        PumpXlsSheetSummary(sheet);
                    else if ((this.Region == RegionName.Tula) && (this.DataSource.Year >= 2010))
                        PumpXlsSheet(sheet, sheetIndex, sheetName, PumpXlsRowTula);
                    else
                        PumpXlsSheet(sheet, sheetIndex, sheetName, PumpXlsRow);

                    WriteToTrace("Страница отчета успешно обработана.", TraceMessageKind.Information);
                }
            }
            finally
            {
                excelHelper.CloseWorkBooks(excelObj);
            }
        }

        #endregion закачка excel

        #region закачка txt (sdi и ipn)

        private const char DELIMETER = '|';

        /// <summary>
        /// Закачка текстового файла
        /// </summary>
        /// <param name="file">Файл</param>
        private void PumpTxtFile(FileInfo file)
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStartFilePumping,
                string.Format("Старт закачки файла {0}.", file.FullName));
            string dataSourcePath = GetShortSourcePathBySourceID(this.SourceID);
            // загружаем файл
            string[] txtReport = CommonRoutines.GetTxtReportData(file, CommonRoutines.GetTxtWinCodePage());
            int rowsCount = txtReport.Length;

            int[] docIds = new int[2];
            docIds[0] = PumpCachedRow(docCache, dsDocList.Tables[0], clsDocList,
                DOC_NAME_REFUND, new object[] { "NAME", DOC_NAME_REFUND });
            docIds[1] = PumpCachedRow(docCache, dsDocList.Tables[0], clsDocList,
                DOC_NAME_TRANSFER, new object[] { "NAME", DOC_NAME_TRANSFER });
            int kvsrID = PumpOriginalRow(dsKvsrUfk2, clsKvsrUfk2, new object[] { "CODESTR", 0 });
            refLocBdgt = -1;
            int date = -1;
            for (int curRow = 0; curRow < (rowsCount - 1); curRow++)
            {
                try
                {
                    SetProgress(rowsCount, curRow,
                        string.Format("Обработка файла {0}\\{1}...", dataSourcePath, file.Name),
                        string.Format("Строка {0} из {1}", curRow, rowsCount));

                    string strValue = txtReport[curRow].Trim();
                    if (strValue == string.Empty)
                        continue;

                    string[] rowsReport = strValue.Split(DELIMETER);

                    if (rowsReport[0].ToUpper() == "SD")
                    {
                        date = CommonRoutines.ShortDateToNewDate(rowsReport[3].Trim());
                        if (this.FinalOverturn)
                            date = this.FinalOverturnDate;
                        CheckDataSourceByDate(date, true);
                        // Удаление данных за текущую дату
                        DeleteDataByReportDate(date);
                        // местные бюджеты
                        string locBdgt = rowsReport[7].Trim();
                        refLocBdgt = PumpCachedRow(locBdgtCache, dsLocBdgt.Tables[0], clsLocBdgt, locBdgt,
                            new object[] { "ACCOUNT", 0, "NAME", locBdgt, "OKATO", 0 });
                        PumpCachedRow(regionsCache, dsRegions.Tables[0], clsRegions, locBdgt,
                            new object[] { "ACCOUNT", 0, "NAME", locBdgt, "OKATO", 0, "SOURCEID", regForPumpSourceID });
                        continue;
                    }

                    if (rowsReport[0].ToUpper() == "SDST")
                    {
                        string kd = rowsReport[2].Trim();
                        int kdID = PumpCachedRow(kdCache, dsKdUfk2.Tables[0], clsKdUfk2, kd,
                            new object[] { "CODESTR", kd, "NAME", constDefaultClsName });

                        string okato = CommonRoutines.TrimLetters(rowsReport[4].Trim());
                        if (okato == string.Empty)
                            okato = "0";

                        string regionKey = string.Format("{0}|{1}", okato, constDefaultClsName);
                        int regionID = PumpCachedRow(regionsUfk2Cache, dsRegionsUfk2.Tables[0], clsRegionsUfk2, regionKey,
                            new object[] { "CODE", 0, "NAME", constDefaultClsName, "OKATO", okato });

                        int okatoID = PumpCachedRow(okatoUfk2Cache, dsOkatoUfk2.Tables[0], clsOkatoUfk2,
                            okato, new object[] { "CODE", okato });

                        docID = docIds[0];
                        double sum = Convert.ToDouble(rowsReport[6].Trim().Replace('.', ','));
                        AddRow(sum, 0, date, regionID, kdID, okatoID, kvsrID, FileFormat.Excel);
                        docID = docIds[1];
                        sum = Convert.ToDouble(rowsReport[7].Trim().Replace('.', ','));
                        AddRow(sum, 0, date, regionID, kdID, okatoID, kvsrID, FileFormat.Excel);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("При обработке строки {0} возникла ошибка ({1})", curRow, ex.Message), ex);
                }
            }
        }

        /// <summary>
        /// Закачка текстового файла с расширением .IPN
        /// </summary>
        /// <param name="file">Файл</param>
        private void PumpTxtIpnFile(FileInfo file)
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStartFilePumping,
                string.Format("Старт закачки файла {0}.", file.FullName));

            int refOkato = PumpCachedRow(okatoUfk2Cache, dsOkatoUfk2.Tables[0], clsOkatoUfk2, "0",
                new object[] { "CODE", "0" });
            docID = PumpCachedRow(docCache, dsDocList.Tables[0], clsDocList, DOC_NAME_CASH,
                new object[] { "NAME", DOC_NAME_CASH });
            int refKvsr = PumpOriginalRow(dsKvsrUfk2, clsKvsrUfk2, new object[] { "CODESTR", 0 });
            int refDate = -1;

            // загружаем файл
            string[] txtReport = CommonRoutines.GetTxtReportData(file, CommonRoutines.GetTxtWinCodePage());
            int rowsCount = txtReport.Length;
            string dataSourcePath = GetShortSourcePathBySourceID(this.SourceID);
            for (int curRow = 0; curRow < (rowsCount - 1); curRow++)
            {
                try
                {
                    SetProgress(rowsCount, curRow,
                        string.Format("Обработка файла {0}\\{1}...", dataSourcePath, file.Name),
                        string.Format("Строка {0} из {1}", curRow, rowsCount));

                    string strValue = txtReport[curRow].Trim();
                    if (strValue == string.Empty)
                        continue;

                    string[] reportRow = strValue.Split(DELIMETER);

                    if (reportRow[0].ToUpper() == "IPSTBK_E")
                    {
                        string kdCode = reportRow[3].Trim();
                        if (kdCode == string.Empty)
                            continue;
                        int refKd = PumpCachedRow(kdCache, dsKdUfk2.Tables[0], clsKdUfk2, kdCode,
                            new object[] { "CODESTR", kdCode, "NAME", constDefaultClsName });

                        string locBdgt = reportRow[1].Trim();
                        refLocBdgt = PumpCachedRow(locBdgtCache, dsLocBdgt.Tables[0], clsLocBdgt, locBdgt,
                            new object[] { "ACCOUNT", 0, "NAME", locBdgt, "OKATO", 0 });

                        string regionKey = string.Format("{0}|{1}", 0, locBdgt);
                        int refRegions = PumpCachedRow(regionsUfk2Cache, dsRegionsUfk2.Tables[0], clsRegionsUfk2, regionKey,
                            new object[] { "CODE", 0, "NAME", locBdgt, "OKATO", 0 });

                        double sum = Convert.ToDouble(reportRow[5].Trim().Replace('.', ','));
                        AddRow(sum, 0, refDate, refRegions, refKd, refOkato, refKvsr, FileFormat.TxtIpn);
                        continue;
                    }

                    if (reportRow[0].ToUpper() == "IP")
                    {
                        refDate = CommonRoutines.ShortDateToNewDate(reportRow[5].Trim());
                        if (this.FinalOverturn)
                            refDate = this.FinalOverturnDate;
                        CheckDataSourceByDate(refDate, true);
                        // Удаление данных за текущую дату
                        DeleteDataByReportDate(refDate);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("При обработке строки {0} возникла ошибка ({1})", curRow, ex.Message), ex);
                }
            }
        }

        #endregion закачка txt (sdi и ipn)

        #region закачка 1н

        /// <summary>
        /// Закачивает файл формата 1н
        /// </summary>
        /// <param name="file">Файл</param>
        private void Pump1nFormatFile(FileInfo file)
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStartFilePumping, string.Format(
                "Старт закачки файла ежедневной ведомости кассовых поступлений (1Н) - данные по доходам от уплаты акцизов {0}.", file.FullName));

            Format1nHierarchicalData format1nData = formats1nParser.ParseHierarchicalFile(file, Format1n.VKP);

            if (format1nData.ContainsKey("VKPSTBK"))
            {
                List<Dictionary<string, string>> vkpstbk = format1nData["VKPSTBK"];

                int date;
                if (format1nData.ContainsKey("VKP") && format1nData["VKP"].Count > 0 &&
                    format1nData["VKP"][0]["DATE_VED"] != string.Empty)
                {
                    date = CommonRoutines.ShortDateToNewDate(format1nData["VKP"][0]["DATE_VED"]);

                    // Если установлен параметр "Закачка заключительных оборотов", то закачиваемый файл должен закачаться 
                    // со ссылкой на фиксированный Период.День по элементу "Заключительные обороты" предыдущего года.
                    if (this.FinalOverturn)
                    {
                        date = this.FinalOverturnDate;
                    }

                    if (!CheckDataSourceByDate(date, false))
                    {
                        badDateRowsCount++;
                        return;
                    }
                }
                else
                {
                    throw new Exception("не найдена дата День ФО");
                }

                // Удаляем закачанные ранее данные по этой дате
                if (this.FinalOverturn)
                {
                    DeleteData(string.Format("RefYearDayUNV = {0} and {1} and GHOST = 2",
                        date, string.Format(GetDisableDeletionFinalOverturnConstraint(), date)),
                        string.Format("Дата отчета: {0}.", date));
                }
                else
                {
                    DeleteData(string.Format("RefYearDayUNV = {0} and {1} and GHOST = 2",
                        date, string.Format(GetDisableDeletionFinalOverturnConstraint(), 0)),
                        string.Format("Дата отчета: {0}.", date));
                }

                int regionID = PumpOriginalRow(dsRegionsUfk2, clsRegionsUfk2,
                    new object[] { "CODE", 26, "NAME", fxcRegionsCache[26]["NAME"] });
                int okatoID = PumpCachedRow(okatoUfk2Cache, dsOkatoUfk2.Tables[0], clsOkatoUfk2, "0",
                    new object[] { "CODE", 0 });
                int kvsrID = PumpOriginalRow(dsKvsrUfk2, clsKvsrUfk2, new object[] { "CODESTR", 0 });

                for (int i = 0; i < vkpstbk.Count; i++)
                {
                    if (!enabledKodDoh.Contains(vkpstbk[i]["KOD_DOH"])) continue;

                    string kd = vkpstbk[i]["KOD_DOH"];
                    int kdID = PumpCachedRow(kdCache, dsKdUfk2.Tables[0], clsKdUfk2, kd,
                        new object[] { "CODESTR", kd, "NAME", constDefaultClsName });

                    AddRow(CommonRoutines.ReduceDouble(vkpstbk[i]["DEB_SUM"]), 3, date,
                        regionID, kdID, okatoID, kvsrID, FileFormat.Format1n);

                    AddRow(CommonRoutines.ReduceDouble(vkpstbk[i]["CRED_SUM"]), 3, date,
                        regionID, kdID, okatoID, kvsrID, FileFormat.Format1n);
                }
            }
        }

        #endregion закачка 1н

        #region закачка 1н - с 2009 года

        private int GetRefDate(string date)
        {
            if (this.FinalOverturn)
                return this.FinalOverturnDate;
            else
            {
                int refDate = CommonRoutines.ShortDateToNewDate(date);
                if (!deletedDate.Contains(refDate))
                {
                    DeleteData(string.Format("RefYearDayUNV = {0} and ghost = 2", refDate),
                        string.Format("Дата отчета: {0}.", refDate));
                    deletedDate.Add(refDate);
                }
                return refDate;
            }
        }

        private const char WIN_DELIMETER = '|';
        private const string BD_MARK = "BD";
        private const string VB_MARK = "VB";
        private const string IP_MARK = "IP";
        private const string BDPDST_MARK = "BDPDST";
        private void Pump1nNewFormatFile(FileInfo file)
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStartFilePumping, string.Format(
                "Старт закачки файла 1Н - {0}.", file.FullName));
            int rowIndex = 0;
            int refDate = -1;
            int refKd = -1;
            bool toPumpRow = false;
            int refRegion = PumpCachedRow(regionsUfk2Cache, dsRegionsUfk2.Tables[0], clsRegionsUfk2, "26|Финуправление",
                new object[] { "Code", "26", "Name", "Финуправление", "Okato", "0" });
            int refKvsr = PumpCachedRow(kvsrCache, dsKvsrUfk2.Tables[0], clsKvsrUfk2, "0",
                new object[] { "CodeStr", "0", "Name", constDefaultClsName });
            int refOkato = PumpCachedRow(okatoUfk2Cache, dsOkatoUfk2.Tables[0], clsOkatoUfk2, "0",
                new object[] { "Code", "0", "Name", constDefaultClsName });
            string[] reportData = CommonRoutines.GetTxtReportData(file, CommonRoutines.GetTxtWinCodePage());
            foreach (string row in reportData)
                try
                {
                    rowIndex++;
                    string[] data = row.Split(WIN_DELIMETER);
                    string mark = data[0].Trim().ToUpper();
                    if (mark == BD_MARK)
                    {
                        toPumpRow = ((data[4].Trim().ToUpper() == VB_MARK) || (data[4].Trim().ToUpper() == IP_MARK));
                        // если данные по этой дате уже закачаны, пропускаем отчет
                        if (!this.FinalOverturn && (deletedDate.Contains(CommonRoutines.ShortDateToNewDate(data[2].Trim())))) 
                            toPumpRow = false;
                        if (toPumpRow)
                            refDate = GetRefDate(data[2].Trim());
                    }
                    if (!toPumpRow)
                        continue;
                    if (mark == BDPDST_MARK)
                    {
                        string kdCode = data[1].Trim().ToUpper();
                        if (!enabledKodDoh.Contains(kdCode))
                            continue;
                        refKd = PumpCachedRow(kdCache, dsKdUfk2.Tables[0], clsKdUfk2, kdCode,
                            new object[] { "CodeStr", kdCode, "Name", constDefaultClsName });
                        decimal sum = Convert.ToDecimal(data[6].Trim().PadLeft(1, '0').Replace('.', ','));
                        if (sum == 0)
                            continue;
                        object[] mapping = new object[] { "Summe", sum, "DocCount", 1, "Ghost", 2, "RefBudgetLevels", 3, 
                            "RefFKDay", refDate, "RefYearDayUNV", refDate, "RefKVSR", refKvsr, "RefKD", refKd, 
                            "RefOKATO", refOkato, "RefRegions", refRegion };
                        PumpRow(dsBudgetIncomings.Tables[0], mapping);
                        if (dsBudgetIncomings.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
                        {
                            UpdateData();
                            ClearDataSet(daBudgetIncomings, ref dsBudgetIncomings);
                        }
                    }
                }
                catch (Exception exp)
                {
                    throw new Exception(string.Format("Ошибка при обработке строки {0} отчета {1}: {2}", rowIndex, file.Name, exp.Message), exp);
                }
        }

        #endregion закачка 1н - с 2009 года

        #region перекрытые методы закачки

        /// <summary>
        /// Закачивается список файлов
        /// </summary>
        /// <param name="dir">Каталог с файлами</param>
        /// <param name="searchOption">Параметр поиска: искать файлы только в корневом каталоге или в подкаталогах тоже</param>
        private void PumpFiles(DirectoryInfo dir, SearchOption searchOption)
        {
            try
            {
                if (dir.GetFiles(string.Format("*.dbf", this.DataSource.Year)).GetLength(0) > 0)
                {
                    // Подключаемся к источнику
                    dbDataAccess.ConnectToDataSource(ref dbfDB, dir.FullName, ODBCDriverName.Microsoft_dBase_Driver);
                    // Закачиваем в датасеты дбф с данными по районам и наименованиям КД
                    PumpServiceTables(dir);
                }

                FileInfo[] filesList = dir.GetFiles("*.*", searchOption);

                // Перебираем файлы в каталоге
                for (int i = 0; i < filesList.GetLength(0); i++)
                {
                    if (!File.Exists(filesList[i].FullName))
                        continue;

                    addedRowsCount = 0;
                    zeroSumRowsCount = 0;
                    badDateRowsCount = 0;
                    skippedRowsCount = 0;

                    try
                    {
                        filesCount++;

                        FileFormat fileFormat = GetFileFormat(filesList[i]);

                        // Если название файла корректно, то закачиваем, иначе пропускаем
                        if (fileFormat == FileFormat.Unknown)
                        {
                            if (filesList[i].Name.ToUpper() != "RAYON.DBF" &&
                                filesList[i].Name.ToUpper() != string.Format("KOD{0}.DBF", this.DataSource.Year))
                            {
                                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning,
                                    string.Format("Пропущен файл {0}.", filesList[i].FullName));
                            }

                            continue;
                        }

                        SetGhostValue(fileFormat);

                        if (fileFormat == FileFormat.Arj)
                            PumpArjFile(filesList[i]);
                        else if (fileFormat == FileFormat.Excel)
                            PumpXlsFormat(filesList[i]);
                        else if (fileFormat == FileFormat.TxtIpn)
                            PumpTxtIpnFile(filesList[i]);
                        else if (fileFormat == FileFormat.Txt)
                            PumpTxtFile(filesList[i]);
                        else if (fileFormat == FileFormat.Format1n)
                            Pump1nFormatFile(filesList[i]);
                        else if (fileFormat == FileFormat.Format1nNew)
                            Pump1nNewFormatFile(filesList[i]);
                        else if (fileFormat == FileFormat.NewDbf)
                            PumpNewFormatFile(filesList[i]);
                        else if (fileFormat == FileFormat.OldDbf)
                            PumpOldFormatFile(filesList[i]);
                        else if (fileFormat == FileFormat.Rar)
                            PumpRarFile(filesList[i]);

                        WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeSuccessfullFinishFilePump, string.Format(
                            "Файл {0} успешно закачан. Добавлено записей в таблицу: {1}, " +
                            "пропущено с нулевыми суммами: {2}, пропущено записей с Район = 28: {3}, " +
                            "пропущено записей с датами, не соответствующими источнику: {4}.",
                            filesList[i].FullName, addedRowsCount, zeroSumRowsCount, skippedRowsCount, badDateRowsCount));
                    }
                    catch (ThreadAbortException)
                    {
                        WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeFinishFilePumpWithError, string.Format(
                            "Ошибка при закачке файла {0}: операция прервана пользователем. \n" +
                            "На момент возникновения ошибки достигнуты следующие результаты. " +
                            "Добавлено записей в таблицу: {1}, пропущено с нулевыми суммами: {2}, " +
                            "пропущено записей с Район = 28: {3}, пропущено записей с датами, не соответствующими источнику: {4}. " +
                            "Данные не сохранены.",
                            filesList[i].FullName, addedRowsCount, zeroSumRowsCount, skippedRowsCount, badDateRowsCount));
                        throw;
                    }
                    catch (Exception ex)
                    {
                        this.DataSourceProcessingResult = DataSourceProcessingResult.ProcessedWithErrors;
                        WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeFinishFilePumpWithError, string.Format(
                            "Ошибка при закачке файла {0}: {1}. \nНа момент возникновения ошибки достигнуты следующие результаты. " +
                            "Добавлено записей в таблицу: {2}, пропущено с нулевыми суммами: {3}, " +
                            "пропущено записей с Район = 28: {4}, пропущено записей с датами, не соответствующими источнику: {5}.",
                            filesList[i].FullName, ex.Message, addedRowsCount, zeroSumRowsCount, skippedRowsCount,
                            badDateRowsCount));
                        continue;
                    }
                }
            }
            finally
            {
                if (excelHelper != null)
                {
                    excelHelper.CloseExcel(ref excelObj);
                    excelHelper.Dispose();
                }
            }
        }

        /// <summary>
        /// Закачка каталогов с отчётами
        /// </summary>
        /// <param name="dir">Каталог источника</param>
        private const string INFO_DIR_NAME = "СПРАВКА";
        private const string SUMMARY_DIR_NAME = "ВЕДОМОСТЬ";
        protected override void ProcessFiles(DirectoryInfo dir)
        {
            sourceDir = dir;
            if (this.FinalOverturn)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning,
                    string.Format("Закачка заключительных оборотов на {0}.",
                        CommonRoutines.NewDateToLongDate(this.FinalOverturnDate.ToString())));
            }

            // для Тулы с 2010 года в каталог источника добавляются ещё каталоги "Справка" и "Ведомость"
            if ((this.Region == RegionName.Tula) && (this.DataSource.Year >= 2010))
            {
                budgetCache = new Dictionary<string, DataRow>();
                // качаем данные из корня источника
                reportType = ReportType.Other;
                PumpFiles(dir, SearchOption.TopDirectoryOnly);

                DirectoryInfo[] info = dir.GetDirectories(INFO_DIR_NAME, SearchOption.TopDirectoryOnly);
                DirectoryInfo[] summary = dir.GetDirectories(SUMMARY_DIR_NAME, SearchOption.TopDirectoryOnly);
                wasInfoSubdir = info.GetLength(0) != 0;
                wasSummarySubdir = summary.GetLength(0) != 0;
                if (!wasInfoSubdir && !wasSummarySubdir)
                {
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning,
                        string.Format("В источнике {0} отсутствуют каталоги \"Справка\" и \"Ведомость\".", dir.FullName));
                }

                // качаем данные из вложенных каталогов
                DirectoryInfo[] subdirs = dir.GetDirectories("*", SearchOption.TopDirectoryOnly);
                for (int i = 0; i < subdirs.GetLength(0); i++)
                {
                    if (subdirs[i].Name.ToUpper() == INFO_DIR_NAME)
                        reportType = ReportType.Info;
                    else if (subdirs[i].Name.ToUpper() == SUMMARY_DIR_NAME)
                        reportType = ReportType.Summary;
                    else
                        reportType = ReportType.Other;
                    PumpFiles(subdirs[i], SearchOption.AllDirectories);
                }
                budgetCache.Clear();
            }
            else
            {
                reportType = ReportType.Other;
                PumpFiles(dir, SearchOption.AllDirectories);
            }
        }

        /// <summary>
        /// Заполняет список кодов дохода, разрешенных к закачке из файлов 1н
        /// </summary>
        private void FillEnabledKodDoh()
        {
            enabledKodDoh = new List<string>();
            enabledKodDoh.Add("10010302150010000110");
            enabledKodDoh.Add("10010302160010000110");
            enabledKodDoh.Add("10010302170010000110");
            enabledKodDoh.Add("10010302180010000110");
            enabledKodDoh.Add("10010302190010000110");
            enabledKodDoh.Add("10010302200010000110");
        }

        protected override void DirectPumpData()
        {
            totalFiles = this.RootDir.GetFiles("*.FFF", SearchOption.AllDirectories).GetLength(0) +
                this.RootDir.GetFiles("*.DBF", SearchOption.AllDirectories).GetLength(0) +
                this.RootDir.GetFiles("*.BD*", SearchOption.AllDirectories).GetLength(0) +
                this.RootDir.GetFiles("*.CE*", SearchOption.AllDirectories).GetLength(0);
            filesCount = 0;
            deletedDate.Clear();
            FillEnabledKodDoh();
            PumpDataYTemplate();
        }

        #endregion перекрытые методы закачки

        #endregion Закачка данных

        #region Обработка данных

        /// <summary>
        /// Получить уровень бюджета по наименованию района
        /// </summary>
        /// <param name="regionName">Наименование района</param>
        /// <returns></returns>
        private int GetBudgetLevelByName(string regionName)
        {
            if (!regionsCache.ContainsKey(regionName))
                return 0;
            int regionId = regionsCache[regionName];
            DataRow[] regions = dsRegions.Tables[0].Select(string.Format("ID = {0}", regionId));
            int terrType = Convert.ToInt32(regions[0]["REFTERRTYPE"]);
            if (terrType == 3)
                return 3;
            else if (terrType == 4)
                return 5;
            else if (terrType == 5)
                return 16;
            else if (terrType == 6)
                return 17;
            else if (terrType == 7)
                return 15;
            else if (terrType == 11)
                return 6;
            return 0;
        }

        /// <summary>
        /// Проставление ссылок на "Фиксированный.Уровни бюджетов"
        /// </summary>
        private void SetBudgetLevels()
        {
            // проставление ссылок в классификаторе "Местные бюджеты.УФК"
            if (dsLocBdgt.Tables.Count == 0)
            {
                WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeInformation,
                    "Классификатор «Местные бюджеты.УФК» не заполнен.");
                return;
            }
            DataTable dt = dsLocBdgt.Tables[0];
            int count = dt.Rows.Count;
            for (int i = 0; i < count; i++)
            {
                DataRow row = dt.Rows[i];
                row["REFBUDGETLEVELS"] = GetBudgetLevelByName(row["NAME"].ToString());
            }
            // проставление ссылок в таблице фактов "Доходы.УФК_СводФУ" 
            InitDataSet(ref daBudgetIncomings, ref dsBudgetIncomings, fctBudgetIncomings, false,
                string.Format("SOURCEID = {0} AND GHOST = 4", this.SourceID), string.Empty);
            if (dsBudgetIncomings.Tables.Count == 0)
                return;
            dt = dsBudgetIncomings.Tables[0];
            count = dt.Rows.Count;
            for (int i = 0; i < count; i++)
            {
                DataRow row = dt.Rows[i];
                int refLocBdgt = Convert.ToInt32(row["REFLOCBDGTUFK"]);
                DataRow[] locBdgt = dsLocBdgt.Tables[0].Select(string.Format("ID = {0}", refLocBdgt));
                if (locBdgt.Length > 0)
                    row["REFBUDGETLEVELS"] = Convert.ToInt32(locBdgt[0]["REFBUDGETLEVELS"]);
            }
        }

        /// <summary>
        /// Возвращает ограничение на выборку согласно типу базы.
        /// Проставление ссылок на "Фиксированный.Уровни бюджетов"
        /// </summary>
        /// <returns>Ограничение</returns>
        private string GetSelectFactDataByMonthConstraint()
        {
            switch (this.ServerDBMSName)
            {
                case DBMSName.SQLServer:
                    return constSQLServerSelectFactDataByMonthConstraint;

                default:
                    return constOracleSelectFactDataByMonthConstraint;
            }
        }

        #region заполнение поля "сумма"

        private string GetDateConstraint()
        {
            string dateConstraint = string.Empty;
            int dateRefMin = -1;
            int dateRefMax = -1;
            string prevDayConstr = string.Empty;
            if (year > 0)
            {
                dateRefMin = year * 10000 + (month) * 100;
                if (month > 1)
                {
                    string query = string.Format("select max(refYearDayUnv) from {0} where refyeardayunv < {1} and refyeardayunv >= {2} and ghost = 3",
                        fctBudgetIncomings.FullDBName, dateRefMin, year * 10000);
                    object prevDay = this.DB.ExecQuery(query, QueryResultTypes.Scalar, new IDbDataParameter[] { });
                    if (prevDay != DBNull.Value)
                        prevDayConstr = string.Format(" or (RefYearDayUNV = {0}) ", prevDay);
                }

                if (month > 0)
                    dateRefMax = year * 10000 + (month) * 100 + CommonRoutines.GetDaysInMonth(month, year);
                else
                    dateRefMax = (year + 1) * 10000;
            }
            if (dateRefMin != -1)
                dateConstraint = string.Format("((RefYearDayUNV >= {0} and RefYearDayUNV <= {1}) {2})", dateRefMin, dateRefMax, prevDayConstr);
            return dateConstraint;
        }

        // составляем структуру <дата, <синтетический ключ из клс, dataRow>>
        private Dictionary<int, Dictionary<string, DataRow>> GetFactAuxCache(DataRow[] rows)
        {
            Dictionary<int, Dictionary<string, DataRow>> auxCache = new Dictionary<int, Dictionary<string, DataRow>>();
            Dictionary<string, DataRow> factByDateCache = null;
            foreach (DataRow row in rows)
            {
                int date = Convert.ToInt32(row["RefYearDayUNV"]);
                string key = GetGroupCacheKey(row, new string[] { "RefBudgetLevels", "RefKVSR", "RefKD", "RefOKATO", 
                    "RefRegions", "RefDoc", "RefLocBdgtUFK" });
                if (!auxCache.ContainsKey(date))
                {
                    factByDateCache = new Dictionary<string, DataRow>();
                    auxCache.Add(date, factByDateCache);
                }
                else
                {
                    factByDateCache = auxCache[date];
                }
                if (!factByDateCache.ContainsKey(key))
                    factByDateCache.Add(key, row);
            }
            return auxCache;
        }

        private List<int> GetFactDateList(Dictionary<int, Dictionary<string, DataRow>> auxCache)
        {
            List<int> list = new List<int>();
            foreach (KeyValuePair<int, Dictionary<string, DataRow>> item in auxCache)
                list.Add(item.Key);
            return list;
        }

        private void FillFactSumConstr(string dateConstr)
        {
            InitDataSet(ref daBudgetIncomings, ref dsBudgetIncomings, fctBudgetIncomings, false,
                string.Format("(GHOST = 3) and ({0})", dateConstr), string.Empty);
            DataRow[] rows = dsBudgetIncomings.Tables[0].Select(string.Empty, string.Format("{0} ASC", "RefYearDayUNV"));
            Dictionary<int, Dictionary<string, DataRow>> auxCache = GetFactAuxCache(rows);
            List<int> dateList = GetFactDateList(auxCache);
            try
            {
                // идем по всем записям, ищем запись в разрезности, совпадающей с текущей (но по предыдущей дате), и вычисляем поле "сумма" на основе двух записей
                for (int i = 0; i <= dateList.Count - 1; i++)
                {
                    if (dateList[i] / 100 % 100 != month)
                        continue;
                    Dictionary<string, DataRow> factByDateCache = auxCache[dateList[i]];
                    foreach (KeyValuePair<string, DataRow> item in factByDateCache)
                    {
                        string key = item.Key;
                        decimal curYearSum = Convert.ToDecimal(item.Value["FromBeginYear"]);
                        item.Value["Summe"] = curYearSum;
                        if (i == 0)
                            continue;
                        for (int k = i - 1; k >= 0; k--)
                        {
                            if (!auxCache[dateList[k]].ContainsKey(key))
                                continue;
                            decimal prevYearSum = Convert.ToDecimal(auxCache[dateList[k]][key]["FromBeginYear"]);
                            item.Value["Summe"] = curYearSum - prevYearSum;
                            break;
                        }
                    }
                }
            }
            finally
            {
                auxCache.Clear();
                dateList.Clear();
            }
            UpdateData();
        }

        private void FillFactSum()
        {
            List<int> dateList = new List<int>();
            try
            {
                if (!this.StagesQueue[PumpProcessStates.PumpData].IsExecuted)
                {
                    dateList.Add(year * 100 + month);
                }
                else
                {
                    foreach (int date in deletedDate)
                    {
                        if (!dateList.Contains(date / 100))
                            dateList.Add(date / 100);
                    }
                }

                foreach (int date in dateList)
                {
                    year = date / 100;
                    if (year != this.DataSource.Year)
                        continue;
                    month = date % 100;
                    string dateConstr = GetDateConstraint();
                    FillFactSumConstr(dateConstr);
                }
            }
            finally
            {
                dateList.Clear();
            }
        }

        #endregion заполнение поля "сумма"

        /// <summary>
        /// Устанавливает соответствие операционных дней для текущего источника
        /// </summary>
        protected override void ProcessDataSource()
        {
            SetBudgetLevels();
            UpdateData();
            if (month > 0)
            {
                SetOperationDaysForFact(fctBudgetIncomings, "GHOST", "RefFKDay", "RefYearDayUNV",
                    string.Format(GetSelectFactDataByMonthConstraint(), month));
            }
            else
            {
                SetOperationDaysForFact(fctBudgetIncomings, "GHOST", "RefFKDay", "RefYearDayUNV", string.Empty);
            }
            UpdateData();
            // заполнение поля "сумма" факта 
            FillFactSum();
        }

        /// <summary>
        /// Функция сохранения закачанных данных в базу
        /// </summary>
        protected override void UpdateProcessedData()
        {
            
        }

        /// <summary>
        /// Функция выполнения завершающих действий закачки
        /// </summary>
        protected override void ProcessFinalizing()
        {

        }

        /// <summary>
        /// Этап обработки данных
        /// </summary>
        protected override void DirectProcessData()
        {
            // Заполняем кэши соответствия операционных дней
            FillOperationDaysCorrCache();

            GetPumpParams(ref year, ref month);

            ProcessDataSourcesTemplate(year, month, "Установка соответствия операционных дней. Проставление ссылок на «Фиксированный.Уровни бюджетов».");

            deletedDate.Clear();

        }

        #endregion Обработка данных
    }
}
