using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.UFK20Pump
{

    // УФК - 0020 - Сводная ведомость по кассовым поступлениям в бюджеты (ежедневная)
    public partial class UFK20PumpModule : CorrectedPumpModuleBase
    {

        #region Поля

        #region Классификаторы

        // КД.УФК (d_KD_UFK)
        private IDbDataAdapter daKd;
        private DataSet dsKd;
        private IClassifier clsKd;
        private Dictionary<string, DataRow> cacheKd = null;
        // Местные бюджеты.УФК (d_LocBdgt_UFK)
        private IDbDataAdapter daBudget;
        private DataSet dsBudget;
        private IClassifier clsBudget;
        private Dictionary<string, int> cacheBudget = null;
        private Dictionary<int, int> cacheBudgetUfk20Check = null;
        private Dictionary<int, int> cacheBudgetUfk6Check = null;
        private int nullBudget;
        // период.соответствие операционных дней (d_Date_ConversionFK)
        private IDbDataAdapter daPeriod;
        private DataSet dsPeriod;
        private IClassifier clsPeriod;
        private Dictionary<int, int> cachePeriod = null;
        // Районы.Сопоставимый (b_Regions_Bridge)
        private IDbDataAdapter daRegion;
        private DataSet dsRegion;
        private IClassifier clsRegion;
        private Dictionary<int, string> cacheRegion = null;
        // Районы.Служебный для закачки (d_Regions_ForPump)
        private IDbDataAdapter daRegionForPump;
        private DataSet dsRegionForPump;
        private IClassifier clsRegionForPump;
        private Dictionary<string, int> cacheRegionForPump = null;
        // Цели использования.МБТ (d_UsingGoal_MBT)
        private IDbDataAdapter daUsingGoal;
        private DataSet dsUsingGoal;
        private IClassifier clsUsingGoal;
        private Dictionary<int, int> cacheUsingGoal = null;
        private int nullUsingGoal = -1;

        #endregion Классификаторы

        #region Факты

        // Доходы.УФК_Сводная ведомость по кассовым поступл ежеднев (f_D_UFK20)
        private IDbDataAdapter daUFK20;
        private DataSet dsUFK20;
        private IFactTable fctUFK20;
        // Доходы.УФК_Справочно (f_D_Information)
        private IFactTable fctUFK6;

        #endregion Факты

        private ExcelHelper excelHelper;
        private object excelObj = null;
        private int clsSourceId = -1;
        private List<int> clsSourceIds = new List<int>();
        private int clsSourceIdRegions = -1;
        private List<int> deletedDateList = null;
        private int year = -1;
        private int month = -1;
        private bool disintAll = false;
        // список дат загуженных текстовых файлов
        private List<int> pumpedDateList = null;

        #endregion Поля

        #region Закачка данных

        #region Работа с базой и кэшами

        private void GetClsSourceId()
        {
            clsSourceId = AddDataSource("УФК", "0020", ParamKindTypes.Year, string.Empty, this.DataSource.Year, 0, string.Empty, 0, string.Empty).ID;
            clsSourceIdRegions = AddDataSource("ФО", "0006", ParamKindTypes.Year, string.Empty, this.DataSource.Year, 0, string.Empty, 0, string.Empty).ID;
            clsSourceIds.Add(clsSourceId);
        }

        protected override void QueryData()
        {
            GetClsSourceId();
            InitClsDataSet(ref daBudget, ref dsBudget, clsBudget);
            nullBudget = clsBudget.UpdateFixedRows(this.DB, this.SourceID);
            InitDataSet(ref daPeriod, ref dsPeriod, clsPeriod, string.Empty);
            InitDataSet(ref daKd, ref dsKd, clsKd, false, string.Format("SOURCEID = {0}", clsSourceId), string.Empty);
            InitDataSet(ref daRegionForPump, ref dsRegionForPump, clsRegionForPump, string.Format("SOURCEID = {0}", clsSourceIdRegions));
            InitDataSet(ref daUsingGoal, ref dsUsingGoal, clsUsingGoal, string.Empty);
            InitFactDataSet(ref daUFK20, ref dsUFK20, fctUFK20);
            QueryDataSources();
            FillCaches();
        }

        private void FillCaches()
        {
            FillRowsCache(ref cacheBudget, dsBudget.Tables[0], "Name", "Id");
            FillRowsCache(ref cacheKd, dsKd.Tables[0], new string[] { "CodeStr" });
            if ((this.Region == RegionName.Samara) || (this.Region == RegionName.Karelya))
                FillRowsCache(ref cachePeriod, dsPeriod.Tables[0], "RefFKDate", "RefFODate");
            else
                FillRowsCache(ref cachePeriod, dsPeriod.Tables[0], "RefFODate", "RefFKDate");
            FillRowsCache(ref cacheRegionForPump, dsRegionForPump.Tables[0], "Name", "Id");
            FillRowsCache(ref cacheUsingGoal, dsUsingGoal.Tables[0], "GoalCode", "Id");
            FillCachesSources();
        }

        protected override void UpdateData()
        {
            UpdateDataSet(daBudget, dsBudget, clsBudget);
            UpdateDataSet(daKd, dsKd, clsKd);
            UpdateDataSet(daRegionForPump, dsRegionForPump, clsRegionForPump);
            UpdateDataSet(daUsingGoal, dsUsingGoal, clsUsingGoal);
            UpdateDataSet(daUFK20, dsUFK20, fctUFK20);
            UpdateDataSources();
        }

        #region GUIDs

        private const string D_DATE_CONVERSION_FK_GUID = "414c27e7-393c-4516-8b47-cf6df384569d";
        private const string D_BUDGET_GUID = "adbbb96f-f22e-4884-a576-9f477733d010";
        private const string D_KD_GUID = "b713e1df-5584-4e3d-a399-8828a2906971";
        private const string D_REGION_FOR_PUMP = "e9d2898d-fc2d-4626-834a-ed1ac98a1673";
        private const string D_USINGGOAL_GUID = "e911c101-598e-4bca-8c32-8a0e38cdd051";
        private const string B_REGION_BRIDGE_GUID = "0906ba3d-3d9a-4c6f-b3a1-f45dbe84a04a";
        private const string F_D_UFK20_GUID = "31b98204-4199-4821-a4a0-b7a85dd88840";
        private const string F_D_UFK6_GUID = "c69847ad-0ac3-4fe8-ba46-ff9b4c43ff36";

        #endregion GUIDs
        protected override void InitDBObjects()
        {
            this.UsedClassifiers = new IClassifier[] { clsBudget = this.Scheme.Classifiers[D_BUDGET_GUID] };
            this.AssociateClassifiersEx = new IClassifier[] { };
            clsPeriod = this.Scheme.Classifiers[D_DATE_CONVERSION_FK_GUID];
            clsKd = this.Scheme.Classifiers[D_KD_GUID];
            clsRegion = this.Scheme.Classifiers[B_REGION_BRIDGE_GUID];
            clsRegionForPump = this.Scheme.Classifiers[D_REGION_FOR_PUMP];
            clsUsingGoal = this.Scheme.Classifiers[D_USINGGOAL_GUID];
            this.UsedFacts = new IFactTable[] { fctUFK20 = this.Scheme.FactTables[F_D_UFK20_GUID] };
            if (this.Region == RegionName.Vologda)
                fctUFK6 = this.Scheme.FactTables[F_D_UFK6_GUID];
            InitDBObjectsSources();
        }

        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsUFK20);
            ClearDataSet(ref dsBudget);
            ClearDataSet(ref dsKd);
            ClearDataSet(ref dsPeriod);
            ClearDataSet(ref dsRegionForPump);
            ClearDataSet(ref dsUsingGoal);
            PumpFinalizingSources();
        }

        #endregion Работа с базой и кэшами

        #region Общие методы

        #region Классификаторы

        private int PumpKd(string code)
        {
            code = code.Trim().PadLeft(1, '0');
            object[] mapping = new object[] { "SourceId", clsSourceId, "CodeStr", code,
                "Name", constDefaultClsName, "CodeType", DBNull.Value };
            return PumpCachedRow(cacheKd, dsKd.Tables[0], clsKd, code, mapping);
        }

        private int PumpKd(string code, string codeType)
        {
            code = code.Trim().PadLeft(1, '0');
            if (cacheKd.ContainsKey(code))
                cacheKd[code]["CodeType"] = codeType.Trim();
            object[] mapping = new object[] { "SourceId", clsSourceId, "CodeStr", code,
                "Name", constDefaultClsName, "CodeType", codeType.Trim() };
            return PumpCachedRow(cacheKd, dsKd.Tables[0], clsKd, code, mapping);
        }

        private int PumpBudget(string name)
        {
            name = name.Trim();
            PumpCachedRow(cacheRegionForPump, dsRegionForPump.Tables[0], clsRegionForPump, name,
                new object[] { "Name", name, "OKATO", "0", "SourceId", clsSourceIdRegions });
            return PumpCachedRow(cacheBudget, dsBudget.Tables[0], clsBudget, name,
                new object[] { "Account", "Неуказанный счет", "Name", name, "OKATO", "0" });
        }

        private int PumpUsingGoal(string codeStr)
        {
            if (codeStr.Trim() == string.Empty)
                return nullUsingGoal;
            int code = Convert.ToInt32(CommonRoutines.TrimLetters(codeStr.Trim()));
            return PumpCachedRow(cacheUsingGoal, dsUsingGoal.Tables[0], clsUsingGoal,
                code, "Id", new object[] { "GoalCode", code });
        }

        #endregion Классификаторы

        #region Факты

        private void PumpFactRow(object[] mapping)
        {
            PumpRow(dsUFK20.Tables[0], mapping);
            if (dsUFK20.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
            {
                UpdateData();
                ClearDataSet(daUFK20, ref dsUFK20);
            }
        }

        #endregion Факты

        private decimal ConvertFactValue(string factValue)
        {
            return Convert.ToDecimal(factValue.Trim().Replace('.', ',').PadLeft(1, '0'));
        }

        private int GetDateRef(string date)
        {
            int refDate = CommonRoutines.ShortDateToNewDate(date);
            if (!CheckDataSourceByDate(refDate, false))
                return -1;
            if (!deletedDateList.Contains(refDate))
            {
                if ((this.Region == RegionName.Samara) || (this.Region == RegionName.Karelya))
                    DeleteData(string.Format("RefFKDayUNV = {0}", refDate), string.Format("Дата отчета: {0}.", refDate));
                else
                    DeleteData(string.Format("RefYearDayUNV = {0}", refDate), string.Format("Дата отчета: {0}.", refDate));
                DeleteDataSources(refDate);
                deletedDateList.Add(refDate);
            }
            return refDate;
        }

        #endregion Общие методы

        #region Работа с Excel

        private void PumpXlsRow(DataRow row, int refDate)
        {
            int refKd = PumpKd(row["Kd"].ToString());
            int refBudget = PumpBudget(row["Budget"].ToString());
            decimal sum = ConvertFactValue(row["Sum"].ToString());
            if (sum == 0)
                return;

            object[] mapping = new object[] {
                "ForPeriod", sum, "RefYearDayUNV", refDate, "RefFKDayUNV", cachePeriod[refDate],
                "RefKD", refKd, "RefLocBdgt", refBudget, "RefMBT", nullUsingGoal };
            PumpFactRow(mapping);
        }

        private int GetFirstRow(object sheet, string titleMark)
        {
            for (int i = 1; ; i++)
                if (excelHelper.GetCell(sheet, i, 1).Value.Trim().ToUpper() == titleMark)
                    return i + 1;
        }

        private int GetLastRow(object sheet, string reportEndMark, int firstRow)
        {
            for (int i = firstRow; ; i++)
                if (excelHelper.GetCell(sheet, i, 1).Value.Trim().ToUpper() == reportEndMark)
                    return i - 1;
        }

        private object[] XLS_MAPPING = new object[] { "Budget", 1, "Kd", 3, "Sum", 4 };
        private const string TABLE_TITLE_MARK = "1";
        private const string REPORT_END_MARK = "ИТОГО ПО КОДУ БК";
        private void PumpXlsSheetData(FileInfo file, object sheet)
        {
            int refDate = GetDateRef(excelHelper.GetCell(sheet, 4, 6).Value);
            if (refDate == -1)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, string.Format(
                    "Дата отчета не совпадает с параметрами источника, данные закачаны не будут (отчет: {0})", file.FullName));
                return;
            }
            if (pumpedDateList.Contains(refDate))
            {
                return;
            }
            int firstRow = GetFirstRow(sheet, TABLE_TITLE_MARK);
            int lastRow = GetLastRow(sheet, REPORT_END_MARK, 1);
            DataTable dt = excelHelper.GetSheetDataTable(sheet, firstRow, lastRow, XLS_MAPPING);
            for (int i = 0; i < dt.Rows.Count; i++)
                try
                {
                    SetProgress(lastRow, i,
                        string.Format("Обработка файла {0}...", file.FullName),
                        string.Format("Строка {0} из {1}", i, lastRow));
                    if (dt.Rows[i]["Budget"].ToString().ToUpper().Contains("ИТОГО"))
                        continue;
                    PumpXlsRow(dt.Rows[i], refDate);
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("При обработке строки {0} возникла ошибка ({1})", i, ex.Message), ex);
                }
        }

        private void PumpXlsFile(FileInfo file)
        {
            excelHelper = new ExcelHelper();
            object workbook = excelHelper.InitWorkBook(ref excelObj, file.FullName);
            try
            {
                object sheet = excelHelper.GetSheet(workbook, 1);
                PumpXlsSheetData(file, sheet);
            }
            finally
            {
                excelHelper.CloseExcel(ref excelObj);
                if (excelHelper != null)
                    excelHelper.Close();
            }
        }

        #endregion Работа с Excel

        #region Работа с Txt

        private int GetBudget(string name)
        {
            if ((this.Region != RegionName.Samara) && (this.Region != RegionName.Karelya) || (name == string.Empty))
                return -1;
            return PumpBudget(name);
        }

        private void PumpTxtRow(string[] rowValues, int refDate)
        {
            int refBudget = PumpBudget(rowValues[1]);
            int refKd = PumpKd(rowValues[3]);
            decimal sum = ConvertFactValue(rowValues[5]);
            if (sum == 0)
                return;

            object[] mapping = new object[] {
                "ForPeriod", sum, "RefYearDayUNV", refDate, "RefFKDayUNV", cachePeriod[refDate],
                "RefKD", refKd, "RefLocBdgt", refBudget, "RefMBT", nullUsingGoal };
            PumpFactRow(mapping);
        }

        private void PumpTxtRowSamara(string[] rowValues, int refDate, string strLocalBudget)
        {
            if (IsSourceRow(rowValues[2]))
                return;

            if (strLocalBudget == string.Empty)
                strLocalBudget = rowValues[1];

            int refBudget = PumpBudget(strLocalBudget);

            int refKd = PumpKd(rowValues[3], rowValues[2]);
            int refMbt = PumpUsingGoal(rowValues[4]);
            decimal sum = ConvertFactValue(rowValues[5]);
            if (sum == 0)
                return;

            object[] mapping = new object[] {
                "ForPeriod", sum, "RefYearDayUNV", cachePeriod[refDate], "RefFKDayUNV", refDate,
                "RefKD", refKd, "RefLocBdgt", refBudget, "RefMBT", refMbt };
            PumpFactRow(mapping);
        }

        private const string IPSTBK_E_MARK = "IPSTBK_E";
        private bool IsPumpedRow(string[] rowValues)
        {
            if (rowValues.GetLength(0) < 7)
                return false;
            return (rowValues[0].Trim() == IPSTBK_E_MARK);
        }

        private bool IsSourceRow(string mark)
        {
            if ((this.Region != RegionName.Samara) && (this.Region != RegionName.Karelya))
                return false;
            return ((mark.Trim() == "31") || (mark.Trim() == "32"));
        }

        private const string IP_MARK = "IP";
        private void PumpTxtFile(FileInfo file)
        {
            string[] reportData = CommonRoutines.GetTxtReportData(file, CommonRoutines.GetTxtWinCodePage());
            int refDate = -1;
            int globalBudgetId = -1;
            string globalBudget = string.Empty;
            int rowsCount = reportData.GetLength(0);
            for (int curRow = 0; curRow < rowsCount; curRow++)
                try
                {
                    SetProgress(rowsCount, curRow,
                        string.Format("Обработка файла {0}...", file.FullName),
                        string.Format("Строка {0} из {1}", curRow + 1, rowsCount));

                    string[] rowValues = reportData[curRow].Split(new char[] { '|' }, StringSplitOptions.None);
                    if (rowValues[0].Trim() == IP_MARK)
                    {
                        refDate = GetDateRef(rowValues[4].Trim());
                        if (refDate == -1)
                        {
                            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, string.Format(
                                "Дата отчета не совпадает с параметрами источника, данные закачаны не будут (отчет: {0})", file.FullName));
                            return;
                        }
                        pumpedDateList.Add(refDate);
                        globalBudget = rowValues[9].Trim(); //GetBudget(rowValues[9].Trim());
                    }

                    if (IsPumpedRow(rowValues))
                    {
                        if (IsSourceRow(rowValues[2]))
                            PumpTxtRowSource(rowValues, refDate, globalBudget);
                        else if ((this.Region == RegionName.Samara) || (this.Region == RegionName.Karelya))
                            PumpTxtRowSamara(rowValues, refDate, globalBudget);
                        else
                            PumpTxtRow(rowValues, refDate);
                    }
                }
                catch (Exception exp)
                {
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeError,
                        string.Format("Ошибка при обработке строки {0} отчета {1}: {2}", curRow + 1, file.Name, exp.Message));
                    this.DataSourceProcessingResult = DataSourceProcessingResult.ProcessedWithErrors;
                }
        }

        #endregion Работа с Txt

        #region Перекрытые методы закачки

        protected override void DirectClsHierarchySetting()
        {
            base.DirectClsHierarchySetting();
            // кд.уфк
            foreach (int sourceId in clsSourceIds)
            {
                DataSet ds = null;
                clsKd.DivideAndFormHierarchy(sourceId, this.PumpID, ref ds);
            }
            // киф.уфк
            foreach (int sourceId in ufk24SourceIds)
            {
                clsKif.DivideClassifierCode(sourceId);
            }
        }

        protected override void ProcessFiles(DirectoryInfo dir)
        {
            pumpedDateList = new List<int>();
            deletedDateList = new List<int>();
            try
            {
                ProcessFilesTemplate(dir, "*.ip*", new ProcessFileDelegate(PumpTxtFile), false);
                ProcessFilesTemplate(dir, "*.xls", new ProcessFileDelegate(PumpXlsFile), false);
                UpdateData();
            }
            finally
            {
                pumpedDateList.Clear();
                deletedDateList.Clear();
            }
        }

        protected override void DirectPumpData()
        {
            deletedSourcesList = new List<int>();
            PumpDataYMTemplate();
        }

        #endregion Перекрытые методы закачки

        #endregion Закачка данных

        #region Обработка данных

        private int GetRefBudgetLevels(int refTerrType)
        {
            if (refTerrType == 3)
                return 3;
            else if (refTerrType == 4)
                return 5;
            else if (refTerrType == 5)
                return 16;
            else if (refTerrType == 6)
                return 17;
            else if (refTerrType == 7)
                return 15;
            else if (refTerrType == 11)
                return 6;
            return 0;
        }

        protected override void ProcessDataSource()
        {
            Dictionary<string, int> cacheRegionsForPumpRefTerr = null;
            FillRowsCache(ref cacheRegionsForPumpRefTerr, dsRegionForPump.Tables[0], "Name", "RefTerrType");
            if (cacheRegionsForPumpRefTerr.Count <= 1)
            {
                WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeWarning,
                    "Классификатор «Районы.Служебный для закачки» не заполнен.");
            }
            else
            {
                foreach (DataRow row in dsBudget.Tables[0].Rows)
                {
                    int refTerrType = FindCachedRow(cacheRegionsForPumpRefTerr, row["Name"].ToString(), -1);
                    row["RefBudgetLevels"] = GetRefBudgetLevels(refTerrType);
                }

                ProcessDataSourcesUFK24();

                UpdateData();
            }
        }

        protected override void DirectProcessData()
        {
            int year = -1;
            int month = -1;
            GetPumpParams(ref year, ref month);
            ProcessDataSourcesTemplate(year, month, "Установка ссылок на уровни бюджета в классификаторе «Местные бюджеты.УФК»");
        }

        #endregion Обработка данных

        #region сопоставление
        #warning obsolete - после перехода на версию переделать на массивы вспомогательных классификаторов (алсо иерархия для кд)

        private int GetClsSource(int sourceID)
        {
            if (sourceID <= 0)
                return -1;
            IDataSource ds = this.Scheme.DataSourceManager.DataSources[sourceID];
            IDataSource clsDs = FindDataSource(ParamKindTypes.Year, ds.SupplierCode, ds.DataCode, string.Empty, ds.Year, 0, string.Empty, 0, string.Empty);
            if (clsDs == null)
                return -1;
            return clsDs.ID;
        }

        private List<int> GetClsDataSourcesList(Dictionary<int, string> dataSources)
        {
            List<int> clsDataSourcesList = new List<int>();
            foreach (int dataSourceId in dataSources.Keys)
            {
                int clsSourceID = GetClsSource(dataSourceId);
                if (clsSourceID > 0 && !clsDataSourcesList.Contains(clsSourceID))
                    clsDataSourcesList.Add(clsSourceID);
            }
            return clsDataSourcesList;
        }

        protected override void DirectAssociateData()
        {
            base.DirectAssociateData();
            // сопоставляем КД.УФК
            Dictionary<int, string> dataSources = null;
            if (this.StagesQueue[PumpProcessStates.PumpData].IsExecuted ||
                this.StagesQueue[PumpProcessStates.ProcessData].IsExecuted)
                dataSources = this.PumpedSources;
            else
                dataSources = GetAllPumpedDataSources();
            List<int> clsDataSources = GetClsDataSourcesList(dataSources);
            try
            {
                for (int i = 0; i < clsDataSources.Count; i++)
                {
                    SetDataSource(clsDataSources[i]);
                    DoBridgeCls(clsDataSources[i], string.Format("источник {0} из {1}", i + 1, clsDataSources.Count),
                        new IClassifier[] { this.Scheme.Classifiers[D_KD_GUID] });
                }
            }
            finally
            {
                clsDataSources.Clear();
            }
        }

        #endregion сопоставление

        #region проверка данных

        #region собственно проверка

        // возвращает кэш данных факта формата:
        // ключ - связка районы.сопоставимый.окато (выходим через местные бюджеты) + датаФк
        // значение - сумма
        private Dictionary<string, decimal> GetDbCheckData(IFactTable fct, string constraint,
            Dictionary<int, int> cacheBudgetCheck, string dateFieldName, string sumFieldName)
        {
            Dictionary<string, decimal> cache = new Dictionary<string, decimal>();
            string semantic = fct.FullCaption;
            int totalRecs = Convert.ToInt32(this.DB.ExecQuery(string.Format(
                "select count(id) from {0} where {1}",
                fct.FullDBName, constraint), QueryResultTypes.Scalar));
            if (totalRecs == 0)
                return cache;
            int firstID = Convert.ToInt32(this.DB.ExecQuery(string.Format(
                "select min(id) from {0} where {1}",
                fct.FullDBName, constraint), QueryResultTypes.Scalar));
            int lastID = firstID + MAX_DS_RECORDS_AMOUNT * 2 - 1;
            int processedRecCount = 0;
            IDbDataAdapter da = null;
            DataSet ds = null;
            do
            {
                string idConstr = string.Format("ID >= {0} and ID <= {1} and {2}", firstID, lastID, constraint);
                firstID = lastID + 1;
                lastID += MAX_DS_RECORDS_AMOUNT * 2;
                InitDataSet(ref da, ref ds, fct, idConstr);
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    processedRecCount++;
                    int refRegionBridge = cacheBudgetCheck[Convert.ToInt32(row["RefLocBdgt"])];
                    if (refRegionBridge < 0)
                        continue;
                    string cacheKey = string.Format("{0}|{1}", cacheRegion[refRegionBridge], Convert.ToInt32(row[dateFieldName]));
                    decimal sum = Convert.ToDecimal(row[sumFieldName].ToString().PadLeft(1, '0'));
                    if (cache.ContainsKey(cacheKey))
                        cache[cacheKey] += sum;
                    else
                        cache.Add(cacheKey, sum);
                }
                ClearDataSet(ref ds);
            }
            while (processedRecCount < totalRecs);
            return cache;
        }

        private void CheckSums(decimal ufk6Sum, decimal ufk20Sum, string cacheKey)
        {
            if (ufk6Sum == ufk20Sum)
                return;
            string okato = cacheKey.Split('|')[0];
            string date = cacheKey.Split('|')[1];
            string message = string.Concat(string.Format("Данные по OKATO '{0}' ", okato),
                                           string.Format("за день ФК '{0}' не сходятся ", date),
                                           string.Format("(Сумма по уфк20: {0}; Сумма по уфк6: {1}).", ufk20Sum, ufk6Sum));
            CheckDataProtocol.WriteEventIntoReviseDataProtocol(ReviseDataEventKind.pdeWarning, message, this.PumpID, this.SourceID);
        }

        private void CheckData()
        {
            int ufk6SourceId = AddDataSource("УФК", "0006", ParamKindTypes.Year, string.Empty, this.DataSource.Year, 0, string.Empty, 0, string.Empty).ID;
            string monthConstraint = string.Format(GetMonthFactConstraint(), this.DataSource.Month, this.DataSource.Year);
            string constraint = string.Format("SourceId = {0} and {1} ", ufk6SourceId, monthConstraint);
            Dictionary<string, decimal> dbUfk6Cache = GetDbCheckData(fctUFK6, constraint, cacheBudgetUfk6Check, "RefYearDayUNV", "Summa");

            constraint = string.Format("SourceId = {0}", this.SourceID);
            Dictionary<string, decimal> dbUfk20Cache = GetDbCheckData(fctUFK20, constraint, cacheBudgetUfk20Check, "RefFKDayUNV", "ForPeriod");
            try
            {
                foreach (KeyValuePair<string, decimal> dbUfk6CacheItem in dbUfk6Cache)
                {
                    if (!dbUfk20Cache.ContainsKey(dbUfk6CacheItem.Key))
                    {
                        string okato = dbUfk6CacheItem.Key.Split('|')[0];
                        string date = dbUfk6CacheItem.Key.Split('|')[1];
                        string message = string.Concat(string.Format("В закачанных данных (уфк20) отсутствуют данные по OKATO '{0}' ", okato),
                                                       string.Format("за день ФК '{0}'.", date));
                        CheckDataProtocol.WriteEventIntoReviseDataProtocol(ReviseDataEventKind.pdeWarning, message, this.PumpID, this.SourceID);
                        continue;
                    }
                    CheckSums(dbUfk6CacheItem.Value, dbUfk20Cache[dbUfk6CacheItem.Key], dbUfk6CacheItem.Key);
                }
            }
            finally
            {
                dbUfk6Cache.Clear();
                dbUfk20Cache.Clear();
            }
        }

        #endregion собственно проверка

        #region перекрытые методы проверки

        private string GetMonthFactConstraint()
        {
            switch (this.ServerDBMSName)
            {
                case DBMSName.SQLServer:
                    return MONTH_CONSTRAINT_SQL_SERVER;
                default:
                    return MONTH_CONSTRAINT_ORACLE;
            }
        }

        private const string MONTH_CONSTRAINT_ORACLE =
            "((floor(mod(RefYearDayUNV, 10000) / 100) = {0}) and (floor(RefYearDayUNV / 10000) = {1}))";
        private const string MONTH_CONSTRAINT_SQL_SERVER =
            "((floor((RefYearDayUNV % 10000) / 100) = {0}) and (floor(RefYearDayUNV / 10000) = {1}))";
        private void QueryCheckData()
        {
            QueryData();
            InitDataSet(ref daRegion, ref dsRegion, clsRegion, true, string.Empty, string.Empty);
            FillRowsCache(ref cacheRegion, dsRegion.Tables[0], "Id", "OKATO");
            FillRowsCache(ref cacheBudgetUfk20Check, dsBudget.Tables[0], "Id", "RefRegionsBridge");

            int ufk6SourceId = AddDataSource("УФК", "0006", ParamKindTypes.Year, string.Empty, this.DataSource.Year, 0, string.Empty, 0, string.Empty).ID;
            string constraint = string.Format("SourceID = {0}", ufk6SourceId);
            InitDataSet(ref daBudget, ref dsBudget, clsBudget, true, constraint, string.Empty);
            FillRowsCache(ref cacheBudgetUfk6Check, dsBudget.Tables[0], "Id", "RefRegionsBridge");
        }

        private void CheckFinal()
        {
            PumpFinalizing();
            ClearDataSet(ref dsRegion);
        }

        private void GetCheckParams()
        {
            year = -1;
            month = -1;
            if (!this.StagesQueue[PumpProcessStates.PumpData].IsExecuted)
                GetDisintParams(ref year, ref month, ref disintAll);
        }

        private Dictionary<int, string> GetDataSources()
        {
            Dictionary<int, string> dataSources = null;
            if (!this.StagesQueue[PumpProcessStates.PumpData].IsExecuted)
                dataSources = GetAllPumpedDataSources();
            else
                dataSources = this.PumpedSources;
            SortDataSources(ref dataSources);
            return dataSources;
        }

        protected override void DirectCheckData()
        {
            if (this.Region != RegionName.Vologda)
                return;
            GetCheckParams();
            Dictionary<int, string> dataSources = GetDataSources();
            foreach (KeyValuePair<int, string> dataSource in dataSources)
            {
                IDataSource source = GetDataSourceBySourceID(dataSource.Key);
                if ((year > 0) && (source.Year != year))
                    continue;
                if ((month > 0) && (source.Month != month))
                    continue;
                SetDataSource(dataSource.Key);
                QueryCheckData();
                CheckData();
                CheckFinal();
            }
        }

        #endregion перекрытые методы проверки

        #endregion проверка данных

    }
}
