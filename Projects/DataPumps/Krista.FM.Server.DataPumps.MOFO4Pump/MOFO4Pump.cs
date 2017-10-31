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

namespace Krista.FM.Server.DataPumps.MOFO4Pump
{
    // мофо 4 - бюджетообразующие налогоплательщики
    public class MOFO4PumpModule : CorrectedPumpModuleBase
    {

        #region Поля

        #region Классификаторы

        // Районы.Планирование (d.Regions.Plan)
        private IDbDataAdapter daRegions;
        private DataSet dsRegions;
        private IClassifier clsRegions;
        private Dictionary<string, int> regionCache = null;
        private int nullRegions;
        // КД.Планирование (d.KD.PlanIncomes)
        private IDbDataAdapter daKD;
        private DataSet dsKD;
        private IClassifier clsKD;
        private Dictionary<string, int> kdCache = null;
        private int nullKD;
        // Организации.Планирование (d.Organizations.Plan)
        private IDbDataAdapter daOrg;
        private DataSet dsOrg;
        private IClassifier clsOrg;
        private Dictionary<string, int> orgCache = null;
        private int nullOrg;

        #endregion Классификаторы

        #region Факты

        // Доходы.МОФО Бюджетообразующие плательщики (f.D.MOFO4IncPayer)
        private IDbDataAdapter daMOFOIncPayer;
        private DataSet dsMOFOIncPayer;
        private IFactTable fctMOFOIncPayer;

        #endregion Факты

        private ReportType reportType;
        private ExcelHelper excelHelper;
        private int rowsCount = 0;
        private object excelObj = null;
        private int planSourceId;

        #endregion Поля

        #region Структуры, перечисления

        private enum ReportType
        {
            Region,
            Settlement
        }

        #endregion Структуры, перечисления

        #region Константы

        private const string REG_DIR_NAME = "По районам";
        private const string SETTLEMENT_DIR_NAME = "По поселениям";

        #endregion Константы

        #region Закачка данных

        #region Работа с базой и кэшами

        private void GetPlanSourceId()
        {
            planSourceId = AddDataSource("ФО", "0003", ParamKindTypes.Year, string.Empty, this.DataSource.Year, 0, string.Empty, 0, string.Empty).ID;
        }

        protected override void QueryData()
        {
            GetPlanSourceId();
            InitDataSet(ref daRegions, ref dsRegions, clsRegions, true, string.Format("SOURCEID = {0}", planSourceId), string.Empty);
            nullRegions = clsRegions.UpdateFixedRows(this.DB, this.SourceID);
            InitDataSet(ref daKD, ref dsKD, clsKD, true, string.Format("SOURCEID = {0}", planSourceId), string.Empty);
            nullKD = clsKD.UpdateFixedRows(this.DB, this.SourceID);
            InitDataSet(ref daOrg, ref dsOrg, clsOrg, false, string.Format("SOURCEID = {0}", planSourceId), string.Empty);
            nullOrg = clsRegions.UpdateFixedRows(this.DB, this.SourceID);
            InitFactDataSet(ref daMOFOIncPayer, ref dsMOFOIncPayer, fctMOFOIncPayer);
            FillCaches();
        }

        private void FillCaches()
        {
            FillRowsCache(ref regionCache, dsRegions.Tables[0], "Name", "ID");
            FillRowsCache(ref kdCache, dsKD.Tables[0], "CodeStr", "ID");
            FillRowsCache(ref orgCache, dsOrg.Tables[0], new string[] { "Code", "INN20", "Name", "RefRegionsPlan" }, "|", "ID");
        }

        protected override void UpdateData()
        {
            UpdateDataSet(daOrg, dsOrg, clsOrg);
            UpdateDataSet(daMOFOIncPayer, dsMOFOIncPayer, fctMOFOIncPayer);
        }

        private const string D_REGIONS_PLAN_GUID = "1f34cc90-16fd-4fcf-b994-0c8a680d7e23";
        private const string D_KD_PLAN_INCOMES_GUID = "a6e33772-325a-4932-a0aa-7ce82f0b3921";
        private const string D_ORGANIZATIONS_PLAN_GUID = "aeabb871-e583-439b-a329-b1f6ce78f212";
        private const string F_D_MOFO4_INC_PAYER_GUID = "1849d0bb-bcd9-4ea3-8388-f974b98da1e4";
        protected override void InitDBObjects()
        {
            this.UsedClassifiers = new IClassifier[] { };

            this.AssociateClassifiersEx = new IClassifier[] {
                clsRegions = this.Scheme.Classifiers[D_REGIONS_PLAN_GUID],
                clsKD = this.Scheme.Classifiers[D_KD_PLAN_INCOMES_GUID],
                clsOrg = this.Scheme.Classifiers[D_ORGANIZATIONS_PLAN_GUID] };

            this.UsedFacts = new IFactTable[] { fctMOFOIncPayer = this.Scheme.FactTables[F_D_MOFO4_INC_PAYER_GUID] };
        }

        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsMOFOIncPayer);
            ClearDataSet(ref dsRegions);
            ClearDataSet(ref dsKD);
            ClearDataSet(ref dsOrg);
        }

        #endregion Работа с базой и кэшами

        #region Общие функции

        private void CheckDirectories(DirectoryInfo dir)
        {
            DirectoryInfo[] region = dir.GetDirectories(REG_DIR_NAME, SearchOption.TopDirectoryOnly);
            DirectoryInfo[] settlement = dir.GetDirectories(SETTLEMENT_DIR_NAME, SearchOption.TopDirectoryOnly);
            if (region.GetLength(0) == 0)
            {
                dir.CreateSubdirectory(REG_DIR_NAME);
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, string.Format("Отсутствует каталог \"{0}\"", REG_DIR_NAME));
            }
            if (settlement.GetLength(0) == 0)
            {
                dir.CreateSubdirectory(SETTLEMENT_DIR_NAME);
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, string.Format("Отсутствует каталог \"{0}\"", SETTLEMENT_DIR_NAME));
            }
        }

        #endregion Общие функции

        #region работа с Excel

        // возвращает количетсво строк в выбранном Excel-листе отчёта
        private int GetRowsCount(object sheet)
        {
            int emptyStrCount = 0;
            int curRow = 14;
            while (emptyStrCount < 10)
            {
                if (excelHelper.GetCell(sheet, curRow, 1).Value.Trim() == string.Empty)
                    emptyStrCount++;
                else
                    emptyStrCount = 0;
                curRow++;
            }
            return (curRow - 10);
        }

        private void PumpXLSRow(object sheet, int row, object[] mapping, string incomeSource, string[] years)
        {
            double fact;
            double estimate;
            double forecast;
            switch (incomeSource)
            {
                case "Налог на имущество организаций":
                    fact = Convert.ToDouble(excelHelper.GetCell(sheet, row, 4).Value.PadLeft(1, '0')) * 1000;
                    PumpRow(dsMOFOIncPayer.Tables[0], (object[])CommonRoutines.ConcatArrays(mapping,
                        new object[] { "RefYearDayUNV", years[0], "RefMarks", 8, "Fact", fact, "Estimate", 0, "Forecast", 0 }));

                    fact = Convert.ToDouble(excelHelper.GetCell(sheet, row, 5).Value.PadLeft(1, '0')) * 1000;
                    PumpRow(dsMOFOIncPayer.Tables[0], (object[])CommonRoutines.ConcatArrays(mapping,
                        new object[] { "RefYearDayUNV", years[0], "RefMarks", 9, "Fact", fact, "Estimate", 0, "Forecast", 0 }));

                    fact = Convert.ToDouble(excelHelper.GetCell(sheet, row, 6).Value.PadLeft(1, '0')) * 1000;
                    PumpRow(dsMOFOIncPayer.Tables[0], (object[])CommonRoutines.ConcatArrays(mapping,
                        new object[] { "RefYearDayUNV", years[0], "RefMarks", 10, "Fact", fact, "Estimate", 0, "Forecast", 0 }));

                    fact = Convert.ToDouble(excelHelper.GetCell(sheet, row, 7).Value.PadLeft(1, '0')) * 1000;
                    estimate = Convert.ToDouble(excelHelper.GetCell(sheet, row, 10).Value.PadLeft(1, '0')) * 1000;
                    PumpRow(dsMOFOIncPayer.Tables[0], (object[])CommonRoutines.ConcatArrays(mapping,
                        new object[] { "RefYearDayUNV", years[1], "RefMarks", 8, "Fact", fact, "Estimate", estimate, "Forecast", 0 }));

                    fact = Convert.ToDouble(excelHelper.GetCell(sheet, row, 8).Value.PadLeft(1, '0')) * 1000;
                    estimate = Convert.ToDouble(excelHelper.GetCell(sheet, row, 11).Value.PadLeft(1, '0')) * 1000;
                    PumpRow(dsMOFOIncPayer.Tables[0], (object[])CommonRoutines.ConcatArrays(mapping,
                        new object[] { "RefYearDayUNV", years[1], "RefMarks", 9, "Fact", fact, "Estimate", estimate, "Forecast", 0 }));

                    fact = Convert.ToDouble(excelHelper.GetCell(sheet, row, 9).Value.PadLeft(1, '0')) * 1000;
                    estimate = Convert.ToDouble(excelHelper.GetCell(sheet, row, 12).Value.PadLeft(1, '0')) * 1000;
                    PumpRow(dsMOFOIncPayer.Tables[0], (object[])CommonRoutines.ConcatArrays(mapping,
                        new object[] { "RefYearDayUNV", years[1], "RefMarks", 10, "Fact", fact, "Estimate", estimate, "Forecast", 0 }));

                    forecast = Convert.ToDouble(excelHelper.GetCell(sheet, row, 13).Value.PadLeft(1, '0')) * 1000;
                    PumpRow(dsMOFOIncPayer.Tables[0], (object[])CommonRoutines.ConcatArrays(mapping,
                        new object[] { "RefYearDayUNV", years[2], "RefMarks", 8, "Fact", 0, "Estimate", 0, "Forecast", forecast }));
                    break;
                case "Налог на прибыль организаций":
                    fact = Convert.ToDouble(excelHelper.GetCell(sheet, row, 4).Value.PadLeft(1, '0')) * 1000;
                    PumpRow(dsMOFOIncPayer.Tables[0], (object[])CommonRoutines.ConcatArrays(mapping,
                        new object[] { "RefYearDayUNV", years[0], "RefMarks", 1, "Fact", fact, "Estimate", 0, "Forecast", 0 }));

                    fact = Convert.ToDouble(excelHelper.GetCell(sheet, row, 5).Value.PadLeft(1, '0')) * 1000;
                    PumpRow(dsMOFOIncPayer.Tables[0], (object[])CommonRoutines.ConcatArrays(mapping,
                        new object[] { "RefYearDayUNV", years[0], "RefMarks", 3, "Fact", fact, "Estimate", 0, "Forecast", 0 }));

                    fact = Convert.ToDouble(excelHelper.GetCell(sheet, row, 6).Value.PadLeft(1, '0')) * 1000;
                    PumpRow(dsMOFOIncPayer.Tables[0], (object[])CommonRoutines.ConcatArrays(mapping,
                        new object[] { "RefYearDayUNV", years[0], "RefMarks", 4, "Fact", fact, "Estimate", 0, "Forecast", 0 }));

                    fact = Convert.ToDouble(excelHelper.GetCell(sheet, row, 7).Value.PadLeft(1, '0')) * 1000;
                    PumpRow(dsMOFOIncPayer.Tables[0], (object[])CommonRoutines.ConcatArrays(mapping,
                        new object[] { "RefYearDayUNV", years[0], "RefMarks", 2, "Fact", fact, "Estimate", 0, "Forecast", 0 }));

                    fact = Convert.ToDouble(excelHelper.GetCell(sheet, row, 8).Value.PadLeft(1, '0')) * 1000;
                    estimate = Convert.ToDouble(excelHelper.GetCell(sheet, row, 11).Value.PadLeft(1, '0')) * 1000;
                    PumpRow(dsMOFOIncPayer.Tables[0], (object[])CommonRoutines.ConcatArrays(mapping,
                        new object[] { "RefYearDayUNV", years[1], "RefMarks", 1, "Fact", fact, "Estimate", estimate, "Forecast", 0 }));

                    fact = Convert.ToDouble(excelHelper.GetCell(sheet, row, 9).Value.PadLeft(1, '0')) * 1000;
                    estimate = Convert.ToDouble(excelHelper.GetCell(sheet, row, 12).Value.PadLeft(1, '0')) * 1000;
                    PumpRow(dsMOFOIncPayer.Tables[0], (object[])CommonRoutines.ConcatArrays(mapping,
                        new object[] { "RefYearDayUNV", years[1], "RefMarks", 3, "Fact", fact, "Estimate", estimate, "Forecast", 0 }));

                    fact = Convert.ToDouble(excelHelper.GetCell(sheet, row, 10).Value.PadLeft(1, '0')) * 1000;
                    estimate = Convert.ToDouble(excelHelper.GetCell(sheet, row, 13).Value.PadLeft(1, '0')) * 1000;
                    PumpRow(dsMOFOIncPayer.Tables[0], (object[])CommonRoutines.ConcatArrays(mapping,
                        new object[] { "RefYearDayUNV", years[1], "RefMarks", 4, "Fact", fact, "Estimate", estimate, "Forecast", 0 }));

                    estimate = Convert.ToDouble(excelHelper.GetCell(sheet, row, 14).Value.PadLeft(1, '0')) * 1000;
                    PumpRow(dsMOFOIncPayer.Tables[0], (object[])CommonRoutines.ConcatArrays(mapping,
                        new object[] { "RefYearDayUNV", years[1], "RefMarks", 2, "Fact", 0, "Estimate", estimate, "Forecast", 0 }));

                    forecast = Convert.ToDouble(excelHelper.GetCell(sheet, row, 15).Value.PadLeft(1, '0')) * 1000;
                    PumpRow(dsMOFOIncPayer.Tables[0], (object[])CommonRoutines.ConcatArrays(mapping,
                        new object[] { "RefYearDayUNV", years[2], "RefMarks", 1, "Fact", 0, "Estimate", 0, "Forecast", forecast }));
                    break;
                case "НДФЛ":
                    fact = Convert.ToDouble(excelHelper.GetCell(sheet, row, 5).Value.PadLeft(1, '0')) * 1000;
                    PumpRow(dsMOFOIncPayer.Tables[0], (object[])CommonRoutines.ConcatArrays(mapping,
                        new object[] { "RefYearDayUNV", years[0], "RefMarks", 5, "Fact", fact, "Estimate", 0, "Forecast", 0 }));

                    fact = Convert.ToDouble(excelHelper.GetCell(sheet, row, 6).Value.PadLeft(1, '0')) * 1000;
                    PumpRow(dsMOFOIncPayer.Tables[0], (object[])CommonRoutines.ConcatArrays(mapping,
                        new object[] { "RefYearDayUNV", years[0], "RefMarks", 6, "Fact", fact, "Estimate", 0, "Forecast", 0 }));

                    fact = Convert.ToDouble(excelHelper.GetCell(sheet, row, 7).Value.PadLeft(1, '0')) * 1000;
                    PumpRow(dsMOFOIncPayer.Tables[0], (object[])CommonRoutines.ConcatArrays(mapping,
                        new object[] { "RefYearDayUNV", years[0], "RefMarks", 7, "Fact", fact, "Estimate", 0, "Forecast", 0 }));

                    fact = Convert.ToDouble(excelHelper.GetCell(sheet, row, 8).Value.PadLeft(1, '0')) * 1000;
                    estimate = Convert.ToDouble(excelHelper.GetCell(sheet, row, 9).Value.PadLeft(1, '0')) * 1000;
                    PumpRow(dsMOFOIncPayer.Tables[0], (object[])CommonRoutines.ConcatArrays(mapping,
                        new object[] { "RefYearDayUNV", years[1], "RefMarks", 5, "Fact", fact, "Estimate", estimate, "Forecast", 0 }));

                    fact = Convert.ToDouble(excelHelper.GetCell(sheet, row, 9).Value.PadLeft(1, '0')) * 1000;
                    estimate = Convert.ToDouble(excelHelper.GetCell(sheet, row, 12).Value.PadLeft(1, '0')) * 1000;
                    PumpRow(dsMOFOIncPayer.Tables[0], (object[])CommonRoutines.ConcatArrays(mapping,
                        new object[] { "RefYearDayUNV", years[1], "RefMarks", 6, "Fact", fact, "Estimate", estimate, "Forecast", 0 }));

                    fact = Convert.ToDouble(excelHelper.GetCell(sheet, row, 10).Value.PadLeft(1, '0')) * 1000;
                    estimate = Convert.ToDouble(excelHelper.GetCell(sheet, row, 11).Value.PadLeft(1, '0')) * 1000;
                    PumpRow(dsMOFOIncPayer.Tables[0], (object[])CommonRoutines.ConcatArrays(mapping,
                        new object[] { "RefYearDayUNV", years[1], "RefMarks", 7, "Fact", fact, "Estimate", estimate, "Forecast", 0 }));

                    forecast = Convert.ToDouble(excelHelper.GetCell(sheet, row, 14).Value.PadLeft(1, '0')) * 1000;
                    PumpRow(dsMOFOIncPayer.Tables[0], (object[])CommonRoutines.ConcatArrays(mapping,
                        new object[] { "RefYearDayUNV", years[2], "RefMarks", 5, "Fact", 0, "Estimate", 0, "Forecast", forecast }));

                    forecast = Convert.ToDouble(excelHelper.GetCell(sheet, row, 15).Value.PadLeft(1, '0')) * 1000;
                    PumpRow(dsMOFOIncPayer.Tables[0], (object[])CommonRoutines.ConcatArrays(mapping,
                        new object[] { "RefYearDayUNV", years[2], "RefMarks", 6, "Fact", 0, "Estimate", 0, "Forecast", forecast }));

                    forecast = Convert.ToDouble(excelHelper.GetCell(sheet, row, 16).Value.PadLeft(1, '0')) * 1000;
                    PumpRow(dsMOFOIncPayer.Tables[0], (object[])CommonRoutines.ConcatArrays(mapping,
                        new object[] { "RefYearDayUNV", years[2], "RefMarks", 7, "Fact", 0, "Estimate", 0, "Forecast", forecast }));
                    break;
            }
            if (dsMOFOIncPayer.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
            {
                UpdateData();
                ClearDataSet(daMOFOIncPayer, ref dsMOFOIncPayer);
            }
        }

        private string GetRegionName(string fileName)
        {
            return fileName.Split('_')[2];
        }

        private bool GetRefRegion(string fileName, string regionName, ref int refRegion, bool toShowWarning)
        {
            refRegion = FindCachedRow(regionCache, regionName, nullRegions);
            if (refRegion == nullRegions)
            {
                if (toShowWarning)
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning,
                        string.Format("Район '{0}' отсутствует в классификаторе 'Районы.Планирование'. Данные по этому району из файла '{1}' закачаны не будут.",
                        regionName, fileName));
                return false;
            }
            return true;
        }

        private bool GetRefDate(object sheet, ref int refDate, string fileName)
        {
            refDate = -1;
            // дата в ячейке B4
            string cellText = excelHelper.GetCell(sheet, 4, 2).Value;
            refDate = CommonRoutines.ShortDateToNewDate(CommonRoutines.TrimLetters(cellText));
            if (refDate == -1)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning,
                    String.Format("Не удалось получить дату отчета из ячейки B4. Данные файла '{0}' закачаны не будут.", fileName));
                return false;
            }
            CheckDataSourceByDate(refDate, true);
            return true;
        }

        private int GetRefKD(object sheet)
        {
            // код кд - B7
            string kdCode = excelHelper.GetCell(sheet, 7, 2).Value;
            return FindCachedRow(kdCache, kdCode, nullKD);
        }

        private const string OTHER_REGION_NAME = "ПРОЧИЕ";
        private const string ALL_ORG_NAME = "ВСЕГО, В ТОМ ЧИСЛЕ";
        private bool GetRefOrg(object sheet, int row, string regionName, int refRegion, ref int refOrg)
        {
            int offset = 0;
            if (reportType == ReportType.Settlement)
                offset = 1;
            string inn = excelHelper.GetCell(sheet, row, 2 + offset).Value;
            if (inn.Trim() == string.Empty)
                inn = "0";
            string kpp = excelHelper.GetCell(sheet, row, 3 + offset).Value;
            string name = excelHelper.GetCell(sheet, row, 1 + offset).Value;
            if (name.ToUpper().StartsWith(ALL_ORG_NAME))
                return false;
            if (name.ToUpper() == OTHER_REGION_NAME)
                name += string.Format(" {0}", regionName);
            // ключ - инн|кпп|имя|район
            string cacheKey = string.Format("{0}|{1}|{2}|{3}", inn, kpp, name, refRegion.ToString());
            refOrg = PumpCachedRow(orgCache, dsOrg.Tables[0], clsOrg, cacheKey,
                new object[] { "Code", inn, "INN20", kpp, "Name", name, "RefRegionsPlan", refRegion, "SourceId", planSourceId });
            return true;
        }

        private string GetIncomeSource(string fileName)
        {
            return fileName.Split('_')[3].Split('-')[0].Trim();
        }

        private string[] GetYears(object sheet)
        {
            // года - прошедший B1, текущий B2, очередной B3 
            return new string[] { excelHelper.GetCell(sheet, 1, 2).Value,
                excelHelper.GetCell(sheet, 2, 2).Value, excelHelper.GetCell(sheet, 3, 2).Value };
        }

        private string GetSettlementName(object sheet, int row, ref bool isEmptySettlementName)
        {
            string settlementName = excelHelper.GetCell(sheet, row, 1).Value;
            isEmptySettlementName = (settlementName == string.Empty);
            while (settlementName == string.Empty)
            {
                settlementName = excelHelper.GetCell(sheet, row - 1, 1).Value;
            }
            return settlementName;
        }

        private string GetOrgRegionName(string settlementName, string regionName)
        {
            if ((settlementName == string.Empty) || (settlementName == regionName))
                return regionName;
            else
                return string.Format("{0} {1}", settlementName, regionName);
        }

        private void PumpXLSSheetData(string fileName, object sheet)
        {
            int refRegion = nullRegions;
            string regionName = GetRegionName(fileName);
            if (!GetRefRegion(fileName, regionName, ref refRegion, true))
                return;
            int refDate = -1;
            if (!GetRefDate(sheet, ref refDate, fileName))
                return;
            int refKD = GetRefKD(sheet);
            string incomeSource = GetIncomeSource(fileName);
            string[] years = GetYears(sheet); 
            for (int row = 14; row <= rowsCount; row++)
                try
                {
                    SetProgress(rowsCount, row, string.Format("Обработка файла {0}...", fileName),
                        string.Format("Строка {0} из {1}", row, rowsCount));
                    string settlementName = string.Empty;
                    if (reportType == ReportType.Settlement)
                    {
                        bool isEmptySettlementName = false;
                        settlementName = GetSettlementName(sheet, row, ref isEmptySettlementName);
                        if (!GetRefRegion(fileName, settlementName, ref refRegion, !isEmptySettlementName))
                            continue;
                    }
                    int refOrg = nullOrg;
                    string orgRegionName = GetOrgRegionName(settlementName, regionName);
                    if (!GetRefOrg(sheet, row, orgRegionName, refRegion, ref refOrg))
                        continue;
                    object[] mapping = new object[] { "RefDayUNVReport", refDate, "RefKD", refKD, "RefOrg", refOrg };
                    PumpXLSRow(sheet, row, mapping, incomeSource, years);
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("При обработке строки {0} возникла ошибка ({1})", row, ex.Message), ex);
                }
        }

        private void PumpXLSFile(FileInfo file)
        {
            object workbook = excelHelper.InitWorkBook(ref excelObj, file.FullName);
            try
            {
                object sheet = excelHelper.GetSheet(workbook, 1);
                rowsCount = GetRowsCount(sheet);
                PumpXLSSheetData(file.Name, sheet);
            }
            finally
            {
                excelHelper.CloseExcel(ref excelObj);
            }
        }

        private void PumpXLSFiles(DirectoryInfo dir)
        {
            reportType = ReportType.Region;
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStartFilePumping, "Старт закачки данных отчетов в разрезе районов.");
            ProcessFilesTemplate(dir.GetDirectories(REG_DIR_NAME)[0], "*.xls", new ProcessFileDelegate(PumpXLSFile), false);
            reportType = ReportType.Settlement;
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStartFilePumping, "Старт закачки данных отчетов в разрезе поселений.");
            ProcessFilesTemplate(dir.GetDirectories(SETTLEMENT_DIR_NAME)[0], "*.xls", new ProcessFileDelegate(PumpXLSFile), false);
        }

        #endregion работа с Excel

        #region Перекрытые методы закачки

        protected override void ProcessFiles(DirectoryInfo dir)
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStart, "Старт инициализации Excel.");
            excelHelper = new ExcelHelper();
            try
            {
                CheckDirectories(dir);
                if (dir.GetFiles("*.xls", SearchOption.AllDirectories).GetLength(0) == 0)
                    return;
                PumpXLSFiles(dir);
                UpdateData();
            }
            finally
            {
                if (excelHelper != null)
                    excelHelper.Close();
            }
        }

        protected override void DirectPumpData()
        {
            PumpDataYTemplate();
        }

        #endregion Перекрытые методы закачки

        #endregion Закачка данных

        #region Сопоставление

        protected override int GetClsSourceID(int sourceID)
        {
            if (sourceID <= 0)
                return -1;
            IDataSource ds = this.Scheme.DataSourceManager.DataSources[sourceID];
            return FindDataSource(ParamKindTypes.Year, "ФО", "0003", string.Empty, ds.Year, 0, string.Empty, 0, string.Empty).ID;
        }

        #endregion Сопоставление

    }
}
