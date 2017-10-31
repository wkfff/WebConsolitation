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

namespace Krista.FM.Server.DataPumps.MOFO16Pump
{
    // МОФО_0016_ОПЕРАТИВНЫЙ АНАЛИЗ ИСПОЛНЕНИЯ ДОХОДОВ
    public class MOFO16PumpModule : CorrectedPumpModuleBase
    {

        #region Поля

        #region Классификаторы

        // КД.Анализ (d_KD_Analysis)
        private IDbDataAdapter daKd;
        private DataSet dsKd;
        private IClassifier clsKd;
        private Dictionary<string, int> kdCache = null;
        private int nullKd;
        // Районы.Анализ (d_Regions_Analysis)
        private IDbDataAdapter daRegions;
        private DataSet dsRegions;
        private IClassifier clsRegions;
        private Dictionary<string, int> regionCache = null;
        private Dictionary<int, int> regionCacheRefTerr = null;

        #endregion Классификаторы

        #region Факты

        // Доходы.МОФО_Оперативный анализ исполнения доходов (f_D_MOFO16Inc)
        private IDbDataAdapter daMOFO16;
        private DataSet dsMOFO16;
        private IFactTable fctMOFO16;

        #endregion Факты

        private ExcelHelper excelHelper;
        private object excelObj = null;
        int analSourceId = -1;
        private List<int> deletedDateList = null;
        private int month = -1;
        private int year = -1;
        private string fileName;

        #endregion Поля

        #region Закачка данных

        #region Работа с базой и кэшами

        private void GetAnalSourceId()
        {
            analSourceId = AddDataSource("ФО", "0006", ParamKindTypes.Year, string.Empty, this.DataSource.Year, 0, string.Empty, 0, string.Empty).ID;
        }

        protected override void QueryData()
        {
            GetAnalSourceId();
            InitDataSet(ref daRegions, ref dsRegions, clsRegions, false, string.Format("SOURCEID = {0}", analSourceId), string.Empty);
            InitDataSet(ref daKd, ref dsKd, clsKd, false, string.Format("SOURCEID = {0}", analSourceId), string.Empty);
            nullKd = clsKd.UpdateFixedRows(this.DB, analSourceId);
            InitFactDataSet(ref daMOFO16, ref dsMOFO16, fctMOFO16);
            FillCaches();
        }

        private void FillKdCutCache()
        {
            Dictionary<string, int> cache = new Dictionary<string,int>();
            foreach (KeyValuePair<string, int> kd in kdCache)
            {
                string kdKey = (kd.Key.Length > 3) ? kd.Key.Substring(3) : kd.Key;
                if (!cache.ContainsKey(kdKey))
                    cache.Add(kdKey, kd.Value);
            }
            kdCache = cache;
        }

        private void FillCaches()
        {
            if (this.Region == RegionName.Vologda)
            {
                FillRowsCache(ref regionCache, dsRegions.Tables[0], "Name", "Id");
                FillRowsCache(ref regionCacheRefTerr, dsRegions.Tables[0], "Id", "RefTerr");
                FillRowsCache(ref kdCache, dsKd.Tables[0], "CodeStr", "Id");
                FillKdCutCache();
            }
            else
            {
                FillRowsCache(ref regionCache, dsRegions.Tables[0], "Code", "Id");
                FillRowsCache(ref kdCache, dsKd.Tables[0], "CodeStr", "Id");
            }
        }

        protected override void UpdateData()
        {
            UpdateDataSet(daRegions, dsRegions, clsRegions);
            UpdateDataSet(daKd, dsKd, clsKd);
            UpdateDataSet(daMOFO16, dsMOFO16, fctMOFO16);
        }

        private const string D_REGIONS_GUID = "383f887a-3ebb-4dba-8abb-560b5777436f";
        private const string D_KD_GUID = "2553274b-4cee-4d20-a9a6-eef173465d8b";
        private const string F_D_MOFO16_GUID = "29a00d3b-2d26-47c4-a7e8-edc40565ad40";
        protected override void InitDBObjects()
        {
            this.UsedClassifiers = new IClassifier[] { };
            this.AssociateClassifiersEx = new IClassifier[] {
                clsRegions = this.Scheme.Classifiers[D_REGIONS_GUID],
                clsKd = this.Scheme.Classifiers[D_KD_GUID] };
            this.UsedFacts = new IFactTable[] { fctMOFO16 = this.Scheme.FactTables[F_D_MOFO16_GUID] };
        }

        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsMOFO16);
            ClearDataSet(ref dsRegions);
            ClearDataSet(ref dsKd);
        }

        #endregion Работа с базой и кэшами

        #region Работа с Excel
        
        #region Вологда

        private void PumpFact(int refDate, int refKd, int refRegion, decimal value, string factField)
        {
            if (value == 0)
                return;

            object[] mapping = new object[] { "RefYearDayUNV", refDate, "RefKD", refKd,
                "RefRegions", refRegion, factField, value, "SourceId", analSourceId };

            PumpRow(dsMOFO16.Tables[0], mapping);
            if (dsMOFO16.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
            {
                UpdateData();
                ClearDataSet(daMOFO16, ref dsMOFO16);
            }
        }

        private int GetRefKd(object sheet, int curRow)
        {
            string code = excelHelper.GetCell(sheet, curRow, 1).Value.ToString().Trim();
            if (!IsNumber(code))
                return -1;

            code = code.Replace(" ", string.Empty);
            if (code.Length > 20)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeError,
                    string.Format("Длина кода \"КД.Анализ\" превышает допустимое значение. Данные по этому коду закачаны не будут. " +
                        "(Файл: {0}, Лист: {1}, Ячейка: A{2})", fileName, excelHelper.GetSheetName(sheet), curRow));
                return -1;
            }

            int refKd = FindCachedRow(kdCache, code.Substring(3), nullKd);
            if (refKd == nullKd)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning,
                    string.Format("В файле \"{0}\", лист \"{1}\", ячейка \"A{2}\" найден код, который не найден в классификаторе КД.Анализ. " + 
                        "Данные по нему закачаны не будут.", fileName, excelHelper.GetSheetName(sheet), curRow));
                return -1;
            }
            return refKd;
        }

        private int[] FACT_COLS = new int[] { 3, 4, 5, 7, 8, 9, 12, 13, 14, 17, 18, 19 };
        private void PumpXLSRowVologda(object sheet, int curRow, int refDate, int refRegion)
        {
            int refKd = GetRefKd(sheet, curRow);
            if (refKd == -1)
                return;

            string value = excelHelper.GetCell(sheet, curRow, 2).Value.ToString().Trim();
            decimal yearPlan = Convert.ToDecimal(CommonRoutines.TrimLetters(value).PadLeft(1, '0'));
            PumpFact(refDate + 1200, refKd, refRegion, yearPlan, "YearPlan");

            for (int i = 0; i < FACT_COLS.Length; i++)
            {
                value = excelHelper.GetCell(sheet, curRow, FACT_COLS[i]).Value.ToString().Trim();
                decimal fact = Convert.ToDecimal(CommonRoutines.TrimLetters(value).PadLeft(1, '0'));
                PumpFact(refDate + (i + 1) * 100, refKd, refRegion, fact, "Fact");
            }
        }

        private bool IsNumber(string value)
        {
            if (value != string.Empty)
                return char.IsDigit(value[0]);
            return false;
        }

        private const string TOTAL_ROW = "ИТОГО";
        private void GetFirstLastRows(object sheet, ref int firstRow, ref int lastRow)
        {
            for (firstRow = 1; ; firstRow++)
                if (IsNumber(excelHelper.GetCell(sheet, firstRow, 2).Value.ToString().Trim()))
                    break;
            for (lastRow = firstRow + 1; ; lastRow++)
                if (excelHelper.GetCell(sheet, lastRow, 1).Value.ToString().Trim().ToUpper().Contains(TOTAL_ROW))
                    break;
        }

        private int GetRegion(string name)
        {
            return FindCachedRow(regionCache, name.ToString(), -1);
        }

        private void PumpXLSSheetDataVologda(FileInfo file, object sheet, int refDate)
        {
            fileName = file.Name;
            int refRegion = GetRegion(excelHelper.GetCell(sheet, 1, 1).Value.ToString());
            if (refRegion == -1)
                return;

            int firstRow = 1, lastRow = 1;
            GetFirstLastRows(sheet, ref firstRow, ref lastRow);
            for (int curRow = firstRow; curRow < lastRow; curRow++)
                try
                {
                    SetProgress(lastRow, curRow, string.Format("Обработка файла {0}...", file.FullName),
                        string.Format("Строка {0} из {1}", curRow, lastRow));
                    PumpXLSRowVologda(sheet, curRow, refDate, refRegion);
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("При обработке строки {0} возникла ошибка ({1})", curRow, ex.Message), ex);
                }
        }

        #endregion Вологда

        #region Общая закачка

        private int GetBudgetLevel(int terrType)
        {
            switch (terrType)
            {
                case 4:
                    return 5;
                case 5:
                    return 16;
                case 6:
                    return 17;
                case 7:
                    return 15;
                default:
                    return 0;
            }
        }

        private int PumpKd(string code)
        {
            return PumpCachedRow(kdCache, dsKd.Tables[0], clsKd, code,
                new object[] { "CodeStr", code, "Name", constDefaultClsName, "SourceId", analSourceId });
        }

        private int PumpRegion(string code)
        {
            return PumpCachedRow(regionCache, dsRegions.Tables[0], clsRegions, code,
                new object[] { "Code", code, "Name", constDefaultClsName, "SourceId", analSourceId });
        }

        private void PumpXlsRow(DataRow row, int refDate)
        {
            decimal yearPlan = Convert.ToDecimal(row["YearPlan"].ToString().PadLeft(1, '0'));
            decimal periodPlan = Convert.ToDecimal(row["PeriodPlan"].ToString().PadLeft(1, '0'));
            decimal fact = Convert.ToDecimal(row["Fact"].ToString().PadLeft(1, '0'));
            if ((yearPlan == 0) && (periodPlan == 0) && (fact == 0))
                return;
            int budgetLevel = GetBudgetLevel(Convert.ToInt32(row["TerrType"]));
            int refKd = PumpKd(row["Kd"].ToString().Trim().PadLeft(1, '0'));
            int refRegion = PumpRegion(row["Region"].ToString().Trim().PadLeft(1, '0'));
            object[] mapping = new object[] { "RefYearDayUNV", refDate, "RefKD", refKd, "RefRegions", refRegion, 
                "RefBdgtLvls", budgetLevel, "YearPlan", yearPlan, "PlanForPeriod", periodPlan, "Fact", fact };
            PumpRow(dsMOFO16.Tables[0], mapping);
            if (dsMOFO16.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
            {
                UpdateData();
                ClearDataSet(daMOFO16, ref dsMOFO16);
            }
        }

        private int GetLastRow(object sheet)
        {
            for (int i = 1; ; i++)
                if (excelHelper.GetCell(sheet, i, 1).Value.Trim() == string.Empty)
                    return i - 1;
        }

        private int GetDateRef(string dirName)
        {
            if (dirName.Length == 4)
                dirName = string.Format("0000{0}", dirName);
            int refDate = CommonRoutines.ShortDateToNewDate(dirName);

            if (this.Region == RegionName.Vologda)
            {
                // удаляем данные за каждый месяц текущего года
                for (int i = 1; i <= 12; i++)
                {
                    int date = refDate + i * 100;
                    if (!deletedDateList.Contains(date))
                    {
                        WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Проверка наличия в базе данных фактов.");
                        string whereStr = string.Format("SOURCEID = {0} and REFYEARDAYUNV = {1}", analSourceId, date);
                        int recCount = Convert.ToInt32(this.DB.ExecQuery(string.Format("select count(id) from {0} where {1}",
                            fctMOFO16.FullDBName, whereStr), QueryResultTypes.Scalar));
                        if (recCount > 0)
                        {
                            string query = string.Format("delete from {0} where {1}", fctMOFO16.FullDBName, whereStr);
                            this.DB.ExecQuery(query, QueryResultTypes.NonQuery);
                            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, string.Format("Данные фактов удалены ({0} строк).", recCount));
                        }
                        else
                        {
                            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Данные фактов отсутствуют.");
                        }
                        deletedDateList.Add(date);
                    }
                }
            }
            else
            {
                if (!deletedDateList.Contains(refDate))
                {
                    DeleteData(string.Format("RefYearDayUNV = {0}", refDate), string.Format("Дата отчета: {0}.", refDate));
                    deletedDateList.Add(refDate);
                }
            }

            return refDate;
        }

        private object[] XLS_MAPPING = new object[] { "Region", 1, "TerrType", 2, "Kd", 3, "YearPlan", 4, "PeriodPlan", 5, "Fact", 6 };
        private void PumpXLSSheetData(FileInfo file, object sheet)
        {
            int refDate = GetDateRef(file.Directory.Name);
            int lastRow = GetLastRow(sheet);
            DataTable dt = excelHelper.GetSheetDataTable(sheet, 2, lastRow, XLS_MAPPING);
            for (int i = 0; i < dt.Rows.Count; i++)
                try
                {
                    SetProgress(lastRow, i, string.Format("Обработка файла {0}...", file.FullName),
                        string.Format("Строка {0} из {1}", i, lastRow));
                    PumpXlsRow(dt.Rows[i], refDate);
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("При обработке строки {0} возникла ошибка ({1})", i, ex.Message), ex);
                }
        }

        #endregion Общая закачка

        private void PumpXLSFile(FileInfo file)
        {
            object workbook = excelHelper.InitWorkBook(ref excelObj, file.FullName);
            try
            {
                if (this.Region == RegionName.Vologda)
                {
                    int refDate = GetDateRef(file.Directory.Name);
                    int sheetCount = excelHelper.GetSheetCount(workbook);
                    for (int i = 1; i <= sheetCount; i++)
                    {
                        object sheet = excelHelper.GetSheet(workbook, i);
                        PumpXLSSheetDataVologda(file, sheet, refDate);
                    }
                }
                else
                {
                    object sheet = excelHelper.GetSheet(workbook, 1);
                    PumpXLSSheetData(file, sheet);
                }
            }
            finally
            {
                excelHelper.CloseExcel(ref excelObj);
            }
        }

        #endregion Работа с Excel

        #region Перекрытые методы закачки

        protected override void ProcessFiles(DirectoryInfo dir)
        {
            deletedDateList = new List<int>();
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStart, "Старт инициализации Excel.");
            excelHelper = new ExcelHelper();
            try
            {
                ProcessFilesTemplate(dir, "*.xls", new ProcessFileDelegate(PumpXLSFile), false);
                UpdateData();
            }
            finally
            {
                if (excelHelper != null)
                    excelHelper.Close();
                deletedDateList.Clear();
            }
        }

        protected override void DirectPumpData()
        {
            PumpDataYTemplate();
        }

        #endregion Перекрытые методы закачки

        #endregion Закачка данных

        #region Обработка данных

        private void FillBudgetLevel()
        {
            InitDataSet(ref daMOFO16, ref dsMOFO16, fctMOFO16, string.Format("SOURCEID = {0}", analSourceId));

            foreach (DataRow row in dsMOFO16.Tables[0].Rows)
            {
                int budgetLevel = GetBudgetLevel(FindCachedRow(regionCacheRefTerr, Convert.ToInt32(row["RefRegions"].ToString()), -1));
                row["RefBdgtLvls"] = budgetLevel;
            }
        }

        protected override void ProcessDataSource()
        {
            FillBudgetLevel();
            UpdateDataSet(daMOFO16, dsMOFO16, fctMOFO16);
        }

        protected override void DirectProcessData()
        {
            year = -1;
            month = -1;
            GetPumpParams(ref year, ref month);
            ProcessDataSourcesTemplate(year, month,
                "Выполняется проставление ссылок с таблицы фактов «Доходы.МОФО_Оперативный анализ исполнения доходов» на классификатор данных «Фиксированный уровни бюджета»");
        }

        #endregion Обработка данных

    }
}
