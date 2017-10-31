using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

using Krista.FM.Common;
using Krista.FM.Server.Common;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.DataPumps.Form1NApp7DayPump
{
    // УФК_0011_1Н_7_Сводная ведомость поступлений (ежедневная) 
    public partial class Form1NApp7DayPumpModule : CorrectedPumpModuleBase
    {
        #region Поля

        #region классификаторы

        // КД.УФК (d.KD.UFK)
        private IDbDataAdapter daKD;
        private DataSet dsKD;
        private IClassifier clsKD;
        private Dictionary<string, int> kdCache = null;
        // ОКАТО.УФК (d.OKATO.UFK)
        private IDbDataAdapter daOKATO;
        private DataSet dsOKATO;
        private IClassifier clsOKATO;
        private Dictionary<string, int> okatoCache = null;
        // Районы.Служебный
        private IDbDataAdapter daRegionsForPump;
        private DataSet dsRegionsForPump;
        private IClassifier clsRegionsForPump;
        private int regionsForPumpSourceID;
        private Dictionary<string, int> regionsForPumpCache = null;
        // ОКАТО.Сопоставимый
        private IDbDataAdapter daOKATOBridge;
        private DataSet dsOKATOBridge;
        private IClassifier clsOKATOBridge;
        private Dictionary<string, string> okatoBridgeCache = null;

        #endregion классификаторы

        #region факты

        // Доходы.Сводная ведомость поступлений ежедневная (f.D.UFK111N)
        private IDbDataAdapter daUFK111N;
        private DataSet dsUFK111N;
        private IFactTable fctUFK111N;
        private string fctUFK111NFullDBName;

        #endregion факты

        private ExcelHelper excelHelper;
        private object excelObj = null;
        private int year = -1;
        private int month = -1;
        private List<int> deletedDate = new List<int>(200);
        private List<int> deletedMonths = new List<int>();
        private DataTable dtDates4Process = null;
        private string constr = string.Empty;
        private FileFormat fileFormat;
        private ReportType reportType;
        // уровень бюджета (для отчётов до 2008 года)
        private int budgetLevel = 0;

        #endregion Поля

        #region Константы

        // Ограничение для выборки данных фактов по указанному месяцу
        private const string constOracleSelectFactDataByMonthConstraint =
            "((floor(mod(RefYearDayUNV, 10000) / 100) = {0}) and (floor(RefYearDayUNV / 10000) = {1}))";

        // Ограничение для выборки данных фактов по указанному месяцу
        private const string constSQLServerSelectFactDataByMonthConstraint =
            "((floor((RefYearDayUNV % 10000) / 100) = {0}) and (floor(RefYearDayUNV / 10000) = {1}))";

        #endregion Константы

        #region Структуры, перечисления

        private enum FileFormat
        {
            FL04,
            FL05,
            SV,
            Unknown
        }

        private enum SectionType
        {
            // Бюджет субъекта
            SubjectBudget,
            // Бюджет района
            RegionBudget,
            // Строка Итого
            Total,
            Unknown
        }

        private enum ReportType
        {
            // краевой бюджет
            RegionalBudget, 
            // местный бюджет
            LocalBudget
        }

        #endregion Структуры, перечисления

        #region Закачка данных

        #region Работа с базой и кэшами

        protected override void QueryData()
        {
            InitClsDataSet(ref daKD, ref dsKD, clsKD);
            InitClsDataSet(ref daOKATO, ref dsOKATO, clsOKATO);
            InitFactDataSet(ref daUFK111N, ref dsUFK111N, fctUFK111N);
            FillCaches();
        }

        private void FillCaches()
        {
            FillRowsCache(ref kdCache, dsKD.Tables[0], "CODESTR");
            FillRowsCache(ref okatoCache, dsOKATO.Tables[0], new string[] { "Code", "Name" }, "|", "Id");
        }

        protected override void UpdateData()
        {
            UpdateDataSet(daKD, dsKD, clsKD);
            UpdateDataSet(daOKATO, dsOKATO, clsOKATO);
            UpdateDataSet(daUFK111N, dsUFK111N, fctUFK111N);
        }

        private const string D_KD_UFK_GUID = "b713e1df-5584-4e3d-a399-8828a2906971";
        private const string D_OKATO_UFK_GUID = "4ae52664-ca7c-4994-bc5e-ba982421540e";
        private const string F_D_UFK11_1N_GUID = "827e71f5-8e77-4844-8dbc-af16e0151642";
        protected override void InitDBObjects()
        {
            this.UsedClassifiers = new IClassifier[] {
                clsKD = this.Scheme.Classifiers[D_KD_UFK_GUID],
                clsOKATO = this.Scheme.Classifiers[D_OKATO_UFK_GUID] };
            this.UsedFacts = new IFactTable[] {
                fctUFK111N = this.Scheme.FactTables[F_D_UFK11_1N_GUID] };
            fctUFK111NFullDBName = fctUFK111N.FullDBName;
        }

        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsUFK111N);
            ClearDataSet(ref dsKD);
            ClearDataSet(ref dsOKATO);
            deletedDate.Clear();
            deletedMonths.Clear();
        }

        #endregion Работа с базой и кэшами

        #region Работа с Excel

        #region Особенности закачки отчётов до 2008 года

        private FileFormat GetFileFormat(FileInfo file)
        {
            string fileName = file.Name.ToUpper();
            if (fileName.StartsWith("FL04"))
                return FileFormat.FL04;
            else if (fileName.StartsWith("FL05"))
                return FileFormat.FL05;
            else if (fileName.StartsWith("SV"))
                return FileFormat.SV;
            return FileFormat.Unknown;
        }

        private void DeleteData2007(int refDate)
        {
            // Удаление данных за текущую дату
            if (!deletedDate.Contains(refDate))
            {
                DeleteData(string.Format("RefYearDayUNV = {0}", refDate), string.Format("Дата отчета: {0}.", refDate));
                deletedDate.Add(refDate);
            }
        }

        private SectionType GetSectionType(string cellValue)
        {
            if (string.IsNullOrEmpty(cellValue))
                return SectionType.Unknown;
            if (cellValue.ToUpper().StartsWith("ИТОГО"))
                return SectionType.Total;
            else if (cellValue.ToUpper()[1] == 'Б')
                return SectionType.RegionBudget;
            else if (string.Compare(cellValue, "Краевой бюджет", true) == 0)
                return SectionType.SubjectBudget;
            return SectionType.Unknown;
        }

        private void PumpXlsRow2007(int curRow, object sheet, int refDate)
        {
            string code = excelHelper.GetCell(sheet, curRow, 1).Value;
            // Уровень бюджета
            switch (GetSectionType(code))
            {
                // Если сумму закачиваем из блока по бюджету каждого района, то 14 – Конс.бюджет МО
                case SectionType.RegionBudget:
                    budgetLevel = 14;
                    return;
                // Если сумму закачиваем из блока по бюджету края, 03- Бюджет субъекта.
                case SectionType.SubjectBudget:
                    budgetLevel = 3;
                    return;
                case SectionType.Total:
                    return;
            }
            // ОКАТО.УФК
            code = code.TrimStart('0').PadRight(1, '0');
            string name = constDefaultClsName;
            string key = string.Format("{0}|{1}", code, name);
            int okatoID = PumpCachedRow(okatoCache, dsOKATO.Tables[0], clsOKATO, key, new object[] { "Code", code, "Name", name });
            // КД.УФК
            string kdCode = excelHelper.GetCell(sheet, curRow, 2).Value;
            int kdID = PumpCachedRow(kdCache, dsKD.Tables[0], clsKD, kdCode, "CODESTR", null);
            double fromBeginMonth = Convert.ToDouble(excelHelper.GetCell(sheet, curRow, 3).Value.PadLeft(1, '0'));
            double fromBeginYear = Convert.ToDouble(excelHelper.GetCell(sheet, curRow, 4).Value.PadLeft(1, '0'));
            PumpRow(dsUFK111N.Tables[0], new object[] { 
	            "FROMBEGINMONTH", fromBeginMonth, "FROMBEGINYEAR", fromBeginYear, 
	            "RefYearDayUNV", refDate, "REFKD", kdID, "REFOKATO", okatoID, "REFBDGTLEVELS", budgetLevel });
            if (dsUFK111N.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
            {
                UpdateData();
                ClearDataSet(daUFK111N, ref dsUFK111N);
            }
        }

        #endregion Особенности закачки отчётов до 2008 года

        #region Особенности закачки отчётов c 2008 года

        private void DeleteData2008(int refDate)
        {
            int year = refDate / 10000;
            int month = (refDate / 100) % 100;
            int minDate = (refDate / 100) * 100;
            int maxDate = (refDate / 100) * 100 + CommonRoutines.GetDaysInMonth(month, year);
            if (!deletedMonths.Contains(minDate))
            {
                DeleteData(string.Format("RefYearDayUNV >= {0} and RefYearDayUNV <= {1}", minDate, maxDate),
                    string.Format("Год: {0}, месяц {1}.", year, month));
                deletedMonths.Add(minDate);
            }
        }

        private int GetBudgetLevel2008()
        {
            if (reportType == ReportType.RegionalBudget)
                return 3;
            else
                return 14;
        }

        private int PumpKd(int curRow, object sheet)
        {
            string code = excelHelper.GetCell(sheet, curRow, 2).Value.Trim();
            string name = constDefaultClsName;
            return PumpCachedRow(kdCache, dsKD.Tables[0], clsKD, code, new object[] { "CodeStr", code, "Name", name });
        }

        private int PumpRegionalOkato(int curRow, object sheet)
        {
            string code = excelHelper.GetCell(sheet, curRow, 1).Value.Trim().TrimStart('0').PadRight(1, '0');
            string name = constDefaultClsName;
            string key = string.Format("{0}|{1}", code, name);
            return PumpCachedRow(okatoCache, dsOKATO.Tables[0], clsOKATO, key, new object[] { "Code", code, "Name", name });
        }

        private int PumpLocalOkato(int curRow, object sheet)
        {
            int code = 0;
            string name = excelHelper.GetCell(sheet, curRow, 1).Value.Trim();
            DataRow[] okatoRow = dsOKATO.Tables[0].Select(string.Format("Name = '{0}'", name));
            if (okatoRow.GetLength(0) == 0)
                return PumpRow(dsOKATO.Tables[0], clsOKATO, new object[] { "Code", code, "Name", name });
            else
                return Convert.ToInt32(okatoRow[0]["Id"]);
        }

        private int PumpOkato(int curRow, object sheet)
        {
            if (reportType == ReportType.RegionalBudget)
                return PumpRegionalOkato(curRow, sheet);
            else
                return PumpLocalOkato(curRow, sheet);
        }
       
        private const string TOTAL_VALUE = "ИТОГО";
        private bool IsTotalRow(string cellValue)
        {
            return (cellValue.StartsWith(TOTAL_VALUE));
        }

        private void PumpXlsRow2008(int curRow, object sheet, int refDate)
        {
            if (IsTotalRow(excelHelper.GetCell(sheet, curRow, 1).Value))
                return;

            int refKd = PumpKd(curRow, sheet);
            int refOkato = PumpOkato(curRow, sheet);
            decimal fromBeginMonth = Convert.ToDecimal(excelHelper.GetCell(sheet, curRow, 3).Value.Trim());
            decimal fromBeginYear = Convert.ToDecimal(excelHelper.GetCell(sheet, curRow, 4).Value.Trim());
            if ((fromBeginMonth == 0) && (fromBeginYear == 0))
                return;
            object[] mapping = new object[] { "RefBdgtLevels", GetBudgetLevel2008(), "RefYearDayUNV", refDate, 
                "RefKD", refKd, "RefOKATO", refOkato, "FromBeginMonth", fromBeginMonth, "FromBeginYear", fromBeginYear };
            PumpRow(dsUFK111N.Tables[0], mapping);
            if (dsUFK111N.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
            {
                UpdateData();
                ClearDataSet(daUFK111N, ref dsUFK111N);
            }
        }

        #endregion Особенности закачки отчётов c 2008 года

        // возвращает количетсво строк в выбранном Excel-листе отчёта
        private const string REGIONAL_REPORT_END = "НАЧАЛЬНИК";
        private const string LOCAL_REPORT_END = "РУКОВОДИТ";
        private const string LOCAL_REPORT_END_2 = "ИСПОЛНИТ";
        private const string TOTAL_ROW = "ИТОГО:";
        private int GetRowsCount(object sheet)
        {
            for (int curRow = GetFirstRow(); curRow <= 65536; curRow++)
            {
                string cellValue = excelHelper.GetCell(sheet, curRow, 1).Value.ToUpper();
                if (cellValue.StartsWith(REGIONAL_REPORT_END) || 
                    cellValue.StartsWith(LOCAL_REPORT_END) || 
                    cellValue.StartsWith(LOCAL_REPORT_END_2) || 
                    cellValue.StartsWith(TOTAL_ROW))
                        return curRow;
            }
            throw new Exception(string.Format("Не найдена нижняя граница страницы {0} отчета.",
                excelHelper.GetSheetName(sheet)));
        }

        private int GetFirstRow()
        {
            if (reportType == ReportType.LocalBudget)
                return 13;
            else
                return 14;
        }

        private bool CheckDate(int refDate)
        {
            if (!CheckDataSourceByDate(refDate, (this.DataSource.Year < 2008)))
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning,
                    string.Format("Дата {0} не соответствует параметрам источника, данные отчета закачаны не будут.", refDate));
                return false;
            }
            return true;
        }

        // дату берем из А8
        private int GetRegionalDateRef(object sheet)
        {
            string date = CommonRoutines.TrimLetters(excelHelper.GetCell(sheet, 8, 1).Value);
            return CommonRoutines.ShortDateToNewDate(date);
        }

        // дату берем из Е4
        private int GetLocalDateRef(object sheet)
        {
            string date = CommonRoutines.TrimLetters(excelHelper.GetCell(sheet, 4, 5).Value);
            int dateInt = CommonRoutines.ShortDateToNewDate(date);
            return CommonRoutines.DecrementDateWithLastDay(dateInt);
        }

        private int GetDateRef(object sheet)
        {
            if (this.DataSource.Year < 2008)
            {
                if (fileFormat == FileFormat.SV)
                    return CommonRoutines.ShortDateToNewDate(excelHelper.GetCell(sheet, 8, 1).Value);
                else
                    return CommonRoutines.ShortDateToNewDate(excelHelper.GetCell(sheet, 16, 3).Value);
            }
            else
            {
                if (reportType == ReportType.RegionalBudget)
                    return GetRegionalDateRef(sheet);
                else
                    return GetLocalDateRef(sheet);
            }
        }

        private void PumpXLSSheetData(string fileName, object sheet)
        {
            // Период.День
            int refDate = -1;
            // Если установлен параметр "Закачка заключительных оборотов", то закачиваемый файл должен закачаться 
            // со ссылкой на фиксированный Период.День по элементу "Заключительные обороты" предыдущего года.
            if (this.FinalOverturn)
                refDate = this.FinalOverturnDate;
            else
            {
                refDate = GetDateRef(sheet);
                if (!CheckDate(refDate))
                    return;
                if (this.DataSource.Year == 2008)
                    DeleteData2008(refDate);
                else
                    DeleteData2007(refDate);
            }

            string dataSourcePath = GetShortSourcePathBySourceID(this.SourceID);
            int rowsCount = GetRowsCount(sheet);
            for (int curRow = GetFirstRow(); curRow <= rowsCount; curRow++)
            {
                try
                {
                    SetProgress(rowsCount, curRow, string.Format("Обработка файла {0}\\{1}...", dataSourcePath, fileName),
                        string.Format("Строка {0} из {1}", curRow, rowsCount));

                    string cellValue = excelHelper.GetCell(sheet, curRow, 1).Value;
                    if (cellValue == string.Empty)
                        continue;

                    if (this.DataSource.Year < 2008)
                        PumpXlsRow2007(curRow, sheet, refDate);
                    else
                        PumpXlsRow2008(curRow, sheet, refDate);
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("При обработке строки {0} возникла ошибка ({1})", curRow, ex.Message), ex);
                }
            }
        }

        private void PumpXLSFile(FileInfo file)
        {
            if (this.DataSource.Year < 2008)
            {
                WriteToTrace("Определение типа файла", TraceMessageKind.Information);
                fileFormat = GetFileFormat(file);
            }
            WriteToTrace("Открытие документа: " + file.Name, TraceMessageKind.Information);
            object workbook = excelHelper.InitWorkBook(ref excelObj, file.FullName);
            try
            {
                int sheetCount = excelHelper.GetSheetCount(workbook);
                for (int i = 1; i <= sheetCount; i++)
                {
                    object sheet = excelHelper.GetSheet(workbook, i);
                    PumpXLSSheetData(file.Name, sheet);
                }
            }
            finally
            {
                excelHelper.CloseExcel(ref excelObj);
            }
        }

        #endregion Работа с Excel

        #region Перекрытые методы закачки

        private const string SEARCH_MASK = "sv*.xls";
        private void Process2007Files(DirectoryInfo dir)
        {
            ProcessFilesTemplate(dir, SEARCH_MASK, new ProcessFileDelegate(PumpXLSFile));
        }

        private const string REGIONAL_BUDGET_DIR_NAME = "КРАЕВОЙ";
        private const string LOCAL_BUDGET_DIR_NAME = "МЕСТНЫЕ";
        private void Process2008Files(DirectoryInfo dir)
        {
            reportType = ReportType.RegionalBudget;
            DirectoryInfo[] auxDir = dir.GetDirectories(REGIONAL_BUDGET_DIR_NAME);
            if (auxDir.GetLength(0) != 0)
                ProcessFilesTemplate(auxDir[0], "*.xls", new ProcessFileDelegate(PumpXLSFile), false);
            reportType = ReportType.LocalBudget;
            auxDir = dir.GetDirectories(LOCAL_BUDGET_DIR_NAME);
            if (auxDir.GetLength(0) != 0)
                ProcessFilesTemplate(auxDir[0], "*.xls", new ProcessFileDelegate(PumpXLSFile), false);
        }

        protected override void ProcessFiles(DirectoryInfo dir)
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStart, "Старт инициализации Excel.");
            excelHelper = new ExcelHelper();
            try
            {
                if (this.DataSource.Year < 2008)
                    Process2007Files(dir);
                else
                    Process2008Files(dir);
            }
            finally
            {
                if (excelHelper != null)
                    excelHelper.Close();
            }
        }

        protected override void DirectPumpData()
        {
            deletedDate.Clear();
            PumpDataYTemplate();
        }

        #endregion Перекрытые методы закачки

        #endregion Закачка данных

        #region Обработка данных

        #region функции обработки 

        protected int GetOkatoTerrType(int okatoID)
        {
            DataRow[] okatoRow = dsOKATO.Tables[0].Select(string.Format("Id = {0}", okatoID));
            string okatoCode = okatoRow[0]["Code"].ToString();
            string okatoName = okatoRow[0]["Name"].ToString();
            if (okatoCode != "0")
                okatoName = FindCachedRow(okatoBridgeCache, okatoCode, constDefaultClsName);
            string regKey = string.Format("{0}|{1}", okatoCode, okatoName);
            int terrType = 0;
            if (!regionsForPumpCache.ContainsKey(regKey))
            {
                PumpCachedRow(regionsForPumpCache, dsRegionsForPump.Tables[0], clsRegionsForPump, regKey,
                    new object[] { "SOURCEID", regionsForPumpSourceID, "OKATO", okatoCode, "NAME", okatoName, "REFTERRTYPE", 0 });
            }
            else
            {
                DataRow[] regForPumpRow = dsRegionsForPump.Tables[0].Select(
                    string.Format("Okato = {0} and Name = '{1}'", okatoCode, okatoName));
                terrType = Convert.ToInt32(regForPumpRow[0]["REFTERRTYPE"]);
            }
            okatoRow[0]["REFTERRTYPE"] = terrType;
            if (terrType == 0)
                WriteToBadOkatoCodesCache(nullTerrTypeOkatoCodesCache, string.Format("{0} ({1});", okatoCode, okatoName));
            return terrType;
        }

        // установка типа территории окато.уфк в соответствии с Районы.Служебный для закачки
        // также устанавливается поле факта - уровни бюджета
        private void SetTerrType()
        {
            for (int i = 0; i < dtDates4Process.Rows.Count; i++)
            {
                int currentDate = Convert.ToInt32(dtDates4Process.Rows[i][0]);
                // Запрашиваем данные для установки поля и устанавливаем его
                InitDataSet(ref daUFK111N, ref dsUFK111N, fctUFK111N, string.Format(
                    "RefYearDayUNV = {0} {1}", currentDate, constr));
                int count = dsUFK111N.Tables[0].Rows.Count;
                for (int j = 0; j < count; j++)
                {
                    DataRow row = dsUFK111N.Tables[0].Rows[j];
                    int okatoID = Convert.ToInt32(row["REFOKATO"]);
                    int terrType = GetOkatoTerrType(okatoID);
                    // В соответствии со значениями в полях «Тип территории» классификатора «ОКАТО.УФК» 
                    // корректируется поле «Уровни бюджета». При заполнении значение из поля Уровни бюджетов перезаписывается.
                    switch (terrType)
                    {
                        case 4:
                            row["REFBDGTLEVELS"] = 5;
                            break;
                        case 5:
                        case 6:
                            row["REFBDGTLEVELS"] = 6;
                            break;
                        case 7:
                            row["REFBDGTLEVELS"] = 15;
                            break;
                        default:
                            row["REFBDGTLEVELS"] = 14;
                            break;
                    }
                }
                UpdateProcessedData();
                ClearDataSet(ref dsUFK111N);
            }
        }

        #endregion функции обработки

        #region перекрытые методы обработки

        private const string D_REGIONS_FOR_PUMP_GUID = "e9d2898d-fc2d-4626-834a-ed1ac98a1673";
        private const string B_OKATO_BRIDGE_GUID = "ba98ebef-0b02-4548-9766-c1e8bc2e55e4";
        protected override void QueryDataForProcess()
        {
            clsRegionsForPump = this.Scheme.Classifiers[D_REGIONS_FOR_PUMP_GUID];
            regionsForPumpSourceID = AddDataSource("ФО", "0006", ParamKindTypes.Year, string.Empty,
                this.DataSource.Year, 0, string.Empty, 0, string.Empty).ID;
            InitDataSet(ref daRegionsForPump, ref dsRegionsForPump, clsRegionsForPump, false,
                string.Format("SOURCEID = {0}", regionsForPumpSourceID), string.Empty);
            FillRowsCache(ref regionsForPumpCache, dsRegionsForPump.Tables[0], new string[] { "OKATO", "Name" }, "|", "REFTERRTYPE");

            clsOKATOBridge = this.Scheme.Classifiers[B_OKATO_BRIDGE_GUID];
            InitDataSet(ref daOKATOBridge, ref dsOKATOBridge, clsOKATOBridge, true, "CODE > 0", string.Empty);
            FillRowsCache(ref okatoBridgeCache, dsOKATOBridge.Tables[0], "CODE", "NAME");

            QueryData();
        }

        protected override void UpdateProcessedData()
        {
            UpdateDataSet(daRegionsForPump, dsRegionsForPump, clsRegionsForPump);
            UpdateData();
        }

        protected override void AfterProcessDataAction()
        {
            WriteBadOkatoCodesCacheToBD();
            nullTerrTypeOkatoCodesCache.Clear();
        }

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

        protected override void ProcessDataSource()
        {
            if (this.StagesQueue[PumpProcessStates.PumpData].IsExecuted)
                constr = string.Format("and PUMPID = {0}", this.PumpID);
            // Если есть ограничение по месяцу - учитываем его
            if (month > 0)
                constr += "and " + string.Format(GetSelectFactDataByMonthConstraint(), month, this.DataSource.Year);
            string query = string.Format("select distinct RefYearDayUNV from {0} where (SOURCEID = {1} {2}) order by RefYearDayUNV asc",
                fctUFK111NFullDBName, this.SourceID, constr);
            // Запрашиваем список дат для обработки
            dtDates4Process = this.DB.ExecQuery(query, QueryResultTypes.DataTable) as DataTable;
            if (dtDates4Process == null || dtDates4Process.Rows.Count == 0)
                throw new Exception("Нет данных для обработки");
            SetTerrType();
        }

        protected override void DirectProcessData()
        {
            GetPumpParams(ref year, ref month);
            ProcessDataSourcesTemplate(year, month, "Установка типа территории ОКАТО и корректировка уровня бюджета факта.");
        }

        #endregion перекрытые методы обработки

       #endregion Обработка данных
    }
}