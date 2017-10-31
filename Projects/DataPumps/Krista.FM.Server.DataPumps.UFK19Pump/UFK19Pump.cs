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

namespace Krista.FM.Server.DataPumps.UFK19Pump
{
    // уфк 19 - Сводная ведомость по кассовым выплатам из бюджета
    public partial class UFK19PumpModule : CorrectedPumpModuleBase
    {

        #region Поля

        #region Классификаторы

        // Местные бюджеты.УФК (d_LocBdgt_UFK)
        private IDbDataAdapter daBud;
        private DataSet dsBud;
        private IClassifier clsBud;
        private Dictionary<string, int> cacheBud = null;
        // Расходы.УФК (d_R_UFK)
        private IDbDataAdapter daOutcomes;
        private DataSet dsOutcomes;
        private IClassifier clsOutcomes;
        private Dictionary<string, int> cacheOutcomes = null;
        private Dictionary<string, DataRow> cacheOutcomesHier = null;
        private Dictionary<string, int> cacheOutcomesParents = null;
        private Dictionary<int, DataRow> cacheOutcomesRows = null;
        // СубЭКР.УФК (d_SubKESR_UFK)
        private IDbDataAdapter daSubEkr;
        private DataSet dsSubEkr;
        private IClassifier clsSubEkr;
        private Dictionary<string, int> cacheSubEkr = null;
        // Получатели.УФК (d_PBS_UFK)
        private IDbDataAdapter daPbs;
        private DataSet dsPbs;
        private IClassifier clsPbs;
        private Dictionary<string, int> cachePbs = null;
        private int nullPbs = -1;
        // Администратор.АС Бюджет
        private IDbDataAdapter daBudKVSR;
        private DataSet dsBudKVSR;
        private IClassifier clsBudKVSR;
        private Dictionary<string, string> cacheBudKVSR = null;
        // ФКР.АС Бюджет
        private IDbDataAdapter daBudFKR;
        private DataSet dsBudFKR;
        private IClassifier clsBudFKR;
        private Dictionary<string, string> cacheBudFKR = null;
        // КЦСР.АС Бюджет
        private IDbDataAdapter daBudKCSR;
        private DataSet dsBudKCSR;
        private IClassifier clsBudKCSR;
        private Dictionary<string, string> cacheBudKCSR = null;
        // КВР.АС Бюджет
        private IDbDataAdapter daBudKVR;
        private DataSet dsBudKVR;
        private IClassifier clsBudKVR;
        private Dictionary<string, string> cacheBudKVR = null;
        // Цели использования.МБТ (d_UsingGoal_MBT)
        private IDbDataAdapter daUsingGoal;
        private DataSet dsUsingGoal;
        private IClassifier clsUsingGoal;
        private Dictionary<int, int> cacheUsingGoal = null;
        private int nullUsingGoal = -1;

        // Районы.Служебный для закачки (d_Regions_ForPump)
        private IDbDataAdapter daRegionForPump;
        private DataSet dsRegionForPump;
        private IClassifier clsRegionForPump;
        private Dictionary<string, int> cacheRegionForPump = null;

        #endregion Классификаторы

        #region Факты

        // Расходы.УФК_Сводная ведомость кассовых выплат из бюджета (f_R_UFK19)
        private IDbDataAdapter daUFK19;
        private DataSet dsUFK19;
        private IFactTable fctUFK19;

        #endregion Факты

        private ExcelHelper excelHelper;
        private int rowsCount = 0;
        private object excelObj = null;
        private List<int> deletedDateList = null;
        private int refFx = 1;

        private List<int> clsSourceIds = new List<int>();
        private int clsSourceIdRegions = -1;

        private Dictionary<string, int> cacheKifBridge = null;

        #endregion Поля

        #region Закачка данных

        #region Работа с базой и кэшами

        private void FillCaches()
        {
            FillRowsCache(ref cacheBud, dsBud.Tables[0], "Name");
            FillRowsCache(ref cacheOutcomes, dsOutcomes.Tables[0], "Code");
            FillRowsCache(ref cacheSubEkr, dsSubEkr.Tables[0], "Code");
            FillRowsCache(ref cachePbs, dsPbs.Tables[0], "INN");
            FillRowsCache(ref cacheOutcomesParents, dsOutcomes.Tables[0], "ParentId");
            FillRowsCache(ref cacheOutcomesRows, dsOutcomes.Tables[0], "ID");
            FillRowsCache(ref cacheUsingGoal, dsUsingGoal.Tables[0], "GoalCode", "Id");
            FillRowsCache(ref cacheRegionForPump, dsRegionForPump.Tables[0], "Name", "Id");
            FillKifBridgeCache();
            FillCachesSources();
        }

        private void FillKifBridgeCache()
        {
            DataTable dt = (DataTable)this.DB.ExecQuery("select id, codeStr from b_KIF_Bridge", 
                QueryResultTypes.DataTable, new IDbDataParameter[] { });
            try
            {
                foreach (DataRow row in dt.Rows)
                    if (row["CodeStr"].ToString().Length >= 3)
                        row["CodeStr"] = row["CodeStr"].ToString().Remove(0, 3);
                FillRowsCache(ref cacheKifBridge, dt, "codeStr");
            }
            finally
            {
                dt.Clear();
            }
        }

        protected override void QueryData()
        {
            clsSourceIdRegions = AddDataSource("ФО", "0006", ParamKindTypes.Year, string.Empty, this.DataSource.Year, 0, string.Empty, 0, string.Empty).ID;
            clsSourceIds.Add(this.SourceID);

            InitClsDataSet(ref daBud, ref dsBud, clsBud, false, string.Empty);
            InitClsDataSet(ref daOutcomes, ref dsOutcomes, clsOutcomes, false, string.Empty);
            InitClsDataSet(ref daSubEkr, ref dsSubEkr, clsSubEkr, false, string.Empty);
            InitClsDataSet(ref daPbs, ref dsPbs, clsPbs, false, string.Empty);
            InitDataSet(ref daRegionForPump, ref dsRegionForPump, clsRegionForPump, string.Format("SOURCEID = {0}", clsSourceIdRegions));
            InitDataSet(ref daUsingGoal, ref dsUsingGoal, clsUsingGoal, string.Empty);
            InitFactDataSet(ref daUFK19, ref dsUFK19, fctUFK19);
            QueryDataSources();
            FillCaches();
        }

        protected override void UpdateData()
        {
            UpdateDataSet(daBud, dsBud, clsBud);
            UpdateDataSet(daRegionForPump, dsRegionForPump, clsRegionForPump);
            UpdateDataSet(daOutcomes, dsOutcomes, clsOutcomes);
            UpdateDataSet(daSubEkr, dsSubEkr, clsSubEkr);
            UpdateDataSet(daPbs, dsPbs, clsPbs);
            UpdateDataSet(daUsingGoal, dsUsingGoal, clsUsingGoal);
            UpdateDataSet(daUFK19, dsUFK19, fctUFK19);
            UpdateDataSources();
        }

        #region GUIDs

        private const string D_REGION_FOR_PUMP = "e9d2898d-fc2d-4626-834a-ed1ac98a1673";
        private const string D_BUD_UFK_GUID = "adbbb96f-f22e-4884-a576-9f477733d010";
        private const string D_OUTCOMES_UFK_GUID = "ba2b17a6-191f-477c-894d-2f879053a69e";
        private const string D_SUB_EKR_GUID = "7170b1a1-c2af-4dc2-90d5-f59024da0b55";
        private const string D_PBS_UFK_GUID = "36bda5d9-ac85-4850-b5e4-6f776932d665";
        private const string D_BUD_KVSR_GUID = "c5f2917f-109e-4c3a-bc0a-9736e28f532e";
        private const string D_BUD_FKR_GUID = "19999614-e1df-4205-a22d-7b098600a13c";
        private const string D_BUD_KCSR_GUID = "884a99a5-0623-4744-bd85-fe0d0fa7ff35";
        private const string D_BUD_KVR_GUID = "9371d823-773b-4afd-a278-24db14f9bf04";
        private const string D_USINGGOAL_GUID = "e911c101-598e-4bca-8c32-8a0e38cdd051";
        private const string F_R_UFK_19_GUID = "4f64828e-60dc-4eb5-b514-30d116f461fd";
        
        #endregion GUIDs
        protected override void InitDBObjects()
        {
            clsBudKVSR = this.Scheme.Classifiers[D_BUD_KVSR_GUID];
            clsRegionForPump = this.Scheme.Classifiers[D_REGION_FOR_PUMP];
            clsBudFKR = this.Scheme.Classifiers[D_BUD_FKR_GUID];
            clsBudKCSR = this.Scheme.Classifiers[D_BUD_KCSR_GUID];
            clsBudKVR = this.Scheme.Classifiers[D_BUD_KVR_GUID];
            clsUsingGoal = this.Scheme.Classifiers[D_USINGGOAL_GUID];
            this.UsedClassifiers = new IClassifier[] {
                clsBud = this.Scheme.Classifiers[D_BUD_UFK_GUID],
                clsOutcomes = this.Scheme.Classifiers[D_OUTCOMES_UFK_GUID],
                clsSubEkr = this.Scheme.Classifiers[D_SUB_EKR_GUID],
                clsPbs = this.Scheme.Classifiers[D_PBS_UFK_GUID] };
            this.HierarchyClassifiers = new IClassifier[] {
                clsBud = this.Scheme.Classifiers[D_BUD_UFK_GUID],
                clsSubEkr = this.Scheme.Classifiers[D_SUB_EKR_GUID],
                clsPbs = this.Scheme.Classifiers[D_PBS_UFK_GUID] };
            this.UsedFacts = new IFactTable[] {
                fctUFK19 = this.Scheme.FactTables[F_R_UFK_19_GUID] };
            InitDBObjectsSources();
        }

        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsUFK19);
            ClearDataSet(ref dsBud);
            ClearDataSet(ref dsRegionForPump);
            ClearDataSet(ref dsOutcomes);
            ClearDataSet(ref dsSubEkr);
            ClearDataSet(ref dsPbs);
            ClearDataSet(ref dsBudKVSR);
            ClearDataSet(ref dsBudFKR);
            ClearDataSet(ref dsBudKCSR);
            ClearDataSet(ref dsBudKVR);
            ClearDataSet(ref dsUsingGoal);
            PumpFinalizingSources();
        }

        #endregion Работа с базой и кэшами

        #region Общие методы

        #region Классификаторы

        private int PumpLocBudget(string name)
        {
            name = name.Trim();
            PumpCachedRow(cacheRegionForPump, dsRegionForPump.Tables[0], clsRegionForPump, name,
                new object[] { "Name", name, "OKATO", "0", "SourceId", clsSourceIdRegions });
            return PumpCachedRow(cacheBud, dsBud.Tables[0], clsBud, name,
                new object[] { "Name", name, "Okato", 0, "Account", "Неуказанный счет" });
        }

        private int PumpOutcomes(string codeStr, object codeType)
        {
            if (codeType.ToString().Trim() == string.Empty)
                codeType = DBNull.Value;
            codeStr = codeStr.Replace(".", string.Empty).PadRight(17, '0');
            long code = Convert.ToInt64(codeStr.Substring(0, 17).TrimStart('0').PadLeft(1, '0'));
            return PumpCachedRow(cacheOutcomes, dsOutcomes.Tables[0], clsOutcomes, code.ToString(),
                new object[] { "Code", code, "CodeType", codeType });
        }

        private int PumpSubEkr(string codeStr)
        {
            codeStr = codeStr.Replace(".", string.Empty).PadRight(17, '0');
            int code = Convert.ToInt32(codeStr.Substring(17).PadRight(7, '0'));
            return PumpCachedRow(cacheSubEkr, dsSubEkr.Tables[0], clsSubEkr, code.ToString(),
                new object[] { "Code", code });
        }

        private int PumpPbs(string inn, string kpp)
        {
            return PumpCachedRow(cachePbs, dsPbs.Tables[0], clsPbs, inn,
                new object[] { "INN", inn, "KPP", kpp });
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

        private void PumpUfk19FactRow(object[] mapping)
        {
            PumpRow(dsUFK19.Tables[0], mapping);
            if (dsUFK19.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
            {
                UpdateData();
                ClearDataSet(daUFK19, ref dsUFK19);
            }
        }

        #endregion Факты

        private int GetFactRecCountByDate(string constr)
        {
            string query = string.Format("select count (*) from {0} where {1}", fctUFK19.FullDBName, constr);
            return Convert.ToInt32(this.DB.ExecQuery(query, QueryResultTypes.Scalar, new IDbDataParameter[] { }));
        }

        private void DeleteDataByDate(int refDate)
        {
            if (deletedDateList.Contains(refDate))
                return;
            DeleteData(string.Format("RefYearDayUNV = {0}", refDate), string.Format("Дата отчета: {0}.", refDate));
            DeleteDataSources(refDate);
            deletedDateList.Add(refDate);
            // добавляем также дату за месяц, чтобы при одновременной закачке ежемесячных отчетов(новосиб)
            // не закачивать отчеты по тем месяцам, данные по которым уже закачаны при закачке ежедневных отчетов
            deletedDateList.Add(refDate / 100);
        }

        private decimal ConvertFactValue(string factValue)
        {
            return Convert.ToDecimal(CommonRoutines.TrimLetters(factValue.Trim().Replace('.', ',')).PadLeft(1, '0'));
        }

        #endregion Общие методы

        #region Работа с Excel

        // возвращает количетсво строк в выбранном Excel-листе отчёта
        private int GetRowsCount(object sheet)
        {
            int emptyStrCount = 0;
            int curRow = 1;
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

        private int GetRefDate(object sheet)
        {
            int refDate = CommonRoutines.ShortDateToNewDate(excelHelper.GetCell(sheet, 4, 8).Value);
            string constr = string.Format("(RefYearDayUNV = {0})", refDate / 100 * 100);
            if (GetFactRecCountByDate(constr) != 0)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning,
                    "В базе уже присутствуют ежемесячные данные за эту дату. Данные отчета закачаны не будут.");
                return -1;
            }
            DeleteDataByDate(refDate);
            return refDate;
        }

        private int GetFirstRow(object sheet)
        {
            for (int curRow = 1; curRow < rowsCount; curRow++)
                if (excelHelper.GetCell(sheet, curRow, 1).Value == "1")
                    return (curRow + 1);
            return 14;
        }

        private object[] GetXlsMapping()
        {
            if (this.Region == RegionName.Vologda)
                return new object[] { "BudName", 1, "Inn", 3, "Kpp", 4, "Code", 5, "Sum", 6 };
            return new object[] { "BudName", 1, "Code", 6, "Sum", 7 };
        }

        private bool CheckOutcomesCode(string sourceCode)
        {
            string code = sourceCode;
            if ((code.Length != 20) || (code.IndexOf('.') != -1))
                return true;
            code = code.Remove(0, 3);
            if (!cacheKifBridge.ContainsKey(code))
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning,
                    string.Format("Код '{0}' по признакам является ИФ, но отсутствует в классификаторе 'Киф.Сопоставимый'. " + 
                                  "Данные по нему будут закачаны.", sourceCode));
            return (!cacheKifBridge.ContainsKey(code));
        }

        private void PumpXlsRow(DataRow row, int refDate)
        {
            string code = row["Code"].ToString().Trim();
            // если код без точек и размер - 20 символов - проверяем - если есть в кифе сопоставимом - не закачиваем
            if (this.Region == RegionName.Novosibirsk)
                if (!CheckOutcomesCode(code))
                    return;

            int refOutcomes = PumpOutcomes(code, DBNull.Value);
            int refSubEkr = PumpSubEkr(code);
            int refBud = PumpLocBudget(row["BudName"].ToString());
            decimal sum = ConvertFactValue(row["Sum"].ToString());
            if (sum == 0)
                return;

            object[] mapping = new object[] {
                "RefYearDayUNV", refDate, "Amount", sum,  "RefLocBdgt", refBud,
                "RefR", refOutcomes, "RefSubKESR", refSubEkr, "RefFX", refFx, "RefMBT", nullUsingGoal };

            if (this.Region == RegionName.Vologda)
            {
                int refPbs = PumpPbs(row["Inn"].ToString(), row["Kpp"].ToString());
                mapping = (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "RefPBS", refPbs });
            }

            PumpUfk19FactRow(mapping);
        }

        private bool IsReportEnd(DataRow row)
        {
            if (this.Region == RegionName.Vologda && row[0].ToString().Trim().ToUpper() == "ИТОГО ПО КОДУ БК")
                return true;
            if (row[1].ToString().Trim().ToUpper().StartsWith(REPORT_END_MARK))
                return true;
            return false;
        }

        private const string REPORT_END_MARK = "ИТОГО";
        private void PumpXlsSheetData(string fileName, object sheet, int refDate)
        {
            int firstRow = GetFirstRow(sheet);
            object[] mapping = GetXlsMapping();
            string dataSourcePath = GetShortSourcePathBySourceID(this.SourceID);
            DataTable dt = excelHelper.GetSheetDataTable(sheet, firstRow, rowsCount - 1, mapping);
            int dataRowsCount = dt.Rows.Count;
            for (int curRow = 0; curRow < dataRowsCount; curRow++)
                try
                {
                    SetProgress(rowsCount, curRow,
                        string.Format("Обработка файла {0}\\{1}...", dataSourcePath, fileName),
                        string.Format("Строка {0} из {1}", curRow, dataRowsCount));

                    DataRow row = dt.Rows[curRow];
                    if (IsReportEnd(row))
                        break;

                    if (!row[0].ToString().Trim().ToUpper().StartsWith(REPORT_END_MARK))
                        PumpXlsRow(dt.Rows[curRow], refDate);
                }
                catch (Exception exp)
                {
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeError,
                        string.Format("Ошибка при обработке строки {0} отчета {1}: {2}", firstRow + curRow, fileName, exp.Message));
                    this.DataSourceProcessingResult = DataSourceProcessingResult.ProcessedWithErrors;
                }
        }

        private void PumpXlsFile(FileInfo file)
        {
            excelHelper = new ExcelHelper();
            object workbook = excelHelper.InitWorkBook(ref excelObj, file.FullName);
            try
            {
                int refDate = -1;
                int sheetCount = excelHelper.GetSheetCount(workbook);
                for (int curSheet = 1; curSheet <= sheetCount; curSheet++)
                {
                    object sheet = excelHelper.GetSheet(workbook, curSheet);
                    string sheetName = excelHelper.GetSheetName(sheet);
                    if (curSheet == 1)
                    {
                        refDate = GetRefDate(sheet);
                        if (refDate == -1)
                            break;
                    }
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation,
                        string.Format("Начало закачки листа '{0}'", sheetName));
                    rowsCount = GetRowsCount(sheet);
                    PumpXlsSheetData(file.Name, sheet, refDate);
                }
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

        private void PumpTxtRow(string[] rowValues, int refDate, string strLocalBudget)
        {
            if (strLocalBudget == string.Empty)
            {
                strLocalBudget = rowValues[1];
            }

            int refLocalBudget = PumpLocBudget(strLocalBudget);
           
            int refOutcomes = -1;
            if ((this.Region == RegionName.Samara) || (this.Region == RegionName.SamaraGO) || (this.Region == RegionName.Karelya) || (this.Region == RegionName.AltayKrai))
                refOutcomes = PumpOutcomes(rowValues[8], rowValues[7]);
            else
                refOutcomes = PumpOutcomes(rowValues[8], DBNull.Value);
            int refSubEkr = PumpSubEkr(rowValues[8]);
            int refUsingGoal = PumpUsingGoal(rowValues[9]);

            decimal amount = ConvertFactValue(rowValues[10]);
            decimal fromBeginYear = 0;

            object[] mapping = new object[] {
                "Amount", amount, "RefLocBdgt", refLocalBudget, "RefYearDayUNV", refDate,
                "RefR", refOutcomes, "RefSubKESR", refSubEkr, "RefPBS", nullPbs, "RefFX", refFx,
                "RefMBT", refUsingGoal };

            if ((this.Region != RegionName.Krasnodar) &&
                (this.Region != RegionName.Osetya) &&
                (this.Region != RegionName.Samara) &&
                (this.Region != RegionName.Karelya) &&
                (this.Region != RegionName.SamaraGO) &&
                (this.Region != RegionName.Saratov))
            {
                fromBeginYear = ConvertFactValue(rowValues[11]);
                mapping = (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "FromBeginYear", fromBeginYear });
            }

            if ((amount == 0) && (fromBeginYear == 0))
                return;

            PumpUfk19FactRow(mapping);
        }

        private int GetRefDateTxt(string[] rowValues)
        {
            int refDate = 0;
            if ((this.Region == RegionName.Krasnodar) || 
                (this.Region == RegionName.Osetya) ||
                (this.Region == RegionName.Samara) ||
                (this.Region == RegionName.Karelya) ||
                (this.Region == RegionName.SamaraGO) ||
                (this.Region == RegionName.Saratov) ||
                (this.Region == RegionName.AltayKrai))
            {
                refDate = CommonRoutines.ShortDateToNewDate(rowValues[8]);
            }
            else
            {
                refDate = Convert.ToInt32(string.Format("{0}{1}00", this.DataSource.Year, rowValues[10].PadLeft(2, '0')));
            }

            if ((this.Region == RegionName.SamaraGO) || (this.Region == RegionName.AltayKrai))
                refDate = CommonRoutines.DecrementDate(refDate);

            DeleteDataByDate(refDate);
            return refDate;
        }

        private char[] WIN_DELIMETER = new char[] { '|' };
        private void PumpTxtFile(FileInfo file)
        {
            int refDate = -1;
            string strLocalBudget = string.Empty;
            string[] reportData = CommonRoutines.GetTxtReportData(file, CommonRoutines.GetTxtWinCodePage());
            int rowsCount = reportData.GetLength(0);
            for (int curRow = 0; curRow < rowsCount; curRow++)
                try
                {
                    SetProgress(rowsCount, curRow,
                        string.Format("Обработка файла {0}...", file.FullName),
                        string.Format("Строка {0} из {1}", curRow + 1, rowsCount));

                    string[] rowValues = reportData[curRow].Split(WIN_DELIMETER, StringSplitOptions.None);
                    string value = rowValues[0].Replace("\n", string.Empty);

                    switch (value)
                    {
                        case "IV":
                            refDate = GetRefDateTxt(rowValues);
                            strLocalBudget = rowValues[11];
                            strLocalBudget = strLocalBudget.Trim();
                            if (refDate == -1)
                                return;
                            continue;
                        case "IVBK_E":
                        case "IVBK_M":
                            string mark = rowValues[7].Trim();
                            if ((mark == "31") || (mark == "32"))
                            {
                                if ((this.Region == RegionName.Novosibirsk) ||
                                    (this.Region == RegionName.Altay))
                                    continue;
                                if ((this.Region == RegionName.Samara) ||
                                    (this.Region == RegionName.SamaraGO)||
                                    (this.Region == RegionName.Karelya)||
                                    (this.Region == RegionName.AltayKrai))
                                {
                                    PumpTxtRowSource(rowValues, refDate, strLocalBudget);
                                    continue;
                                }
                            }
                            PumpTxtRow(rowValues, refDate, strLocalBudget);
                            continue;
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

        private void ProcessAuxDir(DirectoryInfo sourceDir, string auxDirName, string fileMask, ProcessFileDelegate pfd)
        {
            DirectoryInfo[] auxDir = sourceDir.GetDirectories(auxDirName, SearchOption.AllDirectories);
            if (auxDir.GetLength(0) == 0)
                return;
            ProcessFilesTemplate(auxDir[0], fileMask, pfd, false);
        }

        private const string EVERYDAY_DIR_NAME = "Ежедневные данные";
        private const string EVERYMONTH_DIR_NAME = "Ежемесячные данные";
        protected override void ProcessFiles(DirectoryInfo dir)
        {
            deletedDateList = new List<int>();
            try
            {
                // ежедневные
                refFx = 1;
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStart, "Старт закачки ежедневных отчетов.");
                ProcessFilesTemplate(dir, "*.XLS", new ProcessFileDelegate(PumpXlsFile), false);
                ProcessFilesTemplate(dir, "*.IV*", new ProcessFileDelegate(PumpTxtFile), false, SearchOption.TopDirectoryOnly);
                ProcessAuxDir(dir, EVERYDAY_DIR_NAME, "*.IV*", new ProcessFileDelegate(PumpTxtFile));
                UpdateData();
                // ежемесячные
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStart, "Старт закачки ежемесячных отчетов.");
                refFx = 2;
                ProcessAuxDir(dir, EVERYMONTH_DIR_NAME, "*.IV*", new ProcessFileDelegate(PumpTxtFile));
                UpdateData();
            }
            finally
            {
                deletedDateList.Clear();
            }
        }

        protected override void DirectPumpData()
        {
            PumpDataYTemplate();
        }

        protected override void DirectClsHierarchySetting()
        {
            base.DirectClsHierarchySetting();
            // киф.уфк
            foreach (int sourceId in ufk24SourceIds)
            {
                clsKif.DivideClassifierCode(sourceId);
            }
        }

        #endregion Перекрытые методы закачки

        #endregion Закачка данных

        #region Обработка данных

        protected void FillRepCode()
        {
            foreach (DataRow row in dsOutcomes.Tables[0].Rows)
            {
                string code = row["Code"].ToString().PadLeft(17, '0');
                row["RepCode"] = code.Substring(3);
                row["Code1"] = code.Substring(0, 3);
                row["Code2"] = code.Substring(3, 2);
                row["Code3"] = code.Substring(5, 2);
                row["Code4"] = code.Substring(7, 3);
                row["Code5"] = code.Substring(10, 2);
                row["Code6"] = code.Substring(12, 2);
                row["Code7"] = code.Substring(14, 3);
                row["FKR"] = string.Concat(row["Code2"].ToString().PadLeft(2, '0'), row["Code3"].ToString().PadLeft(2, '0'));
                row["KCSR"] = string.Concat(row["Code4"].ToString().PadLeft(3, '0'),
                    row["Code5"].ToString().PadLeft(2, '0'), row["Code6"].ToString().PadLeft(2, '0'));
            }
        }

        #region заполнение классификатора Расходы

        private int PumpOutcomesClsRow(string code, object codeType, int parentId, string name, int hierLevel)
        {
            if (codeType.ToString().Trim() == string.Empty)
                codeType = DBNull.Value;
            if (name.Length > 255)
                name = name.Substring(0, 255);
            object[] mapping = new object[] { "Code", code, "CodeType", codeType, "Name", name, "HierarchyLevel", hierLevel };
            string cacheKey = string.Format("{0}|", code);
            if (parentId != -1)
            {
                mapping = (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "ParentId", parentId });
                cacheKey += parentId.ToString();
            }
            return PumpCachedRow(cacheOutcomesHier, clsOutcomes, dsOutcomes.Tables[0], cacheKey, mapping, true);
        }

        private string GetClsNameByCode(Dictionary<string, string> cache, string code)
        {
            string name = constDefaultClsName;
            if (cache.ContainsKey(code))
                name = cache[code];
            if (name.Length > 255)
                name = name.Substring(0, 255);
            return name;
        }

        private const string NULL_FKR_CODE = "0000";
        private const string NULL_KCSR_CODE = "0000000";
        private const string NULL_KVR_CODE = "000";
        private int PumpOutcomesClsRows(string sourceCode, string codeType)
        {
            // 1 - XXX.0000.0000000.000
            string code = string.Format("{0}00000000000000", sourceCode.Substring(0, 3)).TrimStart('0').PadLeft(1, '0');
            string name = GetClsNameByCode(cacheBudKVSR, sourceCode.Substring(0, 3).TrimStart('0').PadLeft(1, '0'));
            int parentId = PumpOutcomesClsRow(code, codeType, -1, name, 1);
            // 2 - XXX.XX00.0000000.000
            code = string.Format("{0}000000000000", sourceCode.Substring(0, 5)).TrimStart('0').PadLeft(1, '0');
            name = GetClsNameByCode(cacheBudFKR, sourceCode.Substring(3, 2).PadRight(4, '0').TrimStart('0').PadLeft(1, '0'));
            parentId = PumpOutcomesClsRow(code, codeType, parentId, name, 2);
            // 3 - XXX.XXXX.000.00.00.000
            code = string.Format("{0}0000000000", sourceCode.Substring(0, 7)).TrimStart('0').PadLeft(1, '0');
            name = GetClsNameByCode(cacheBudFKR, sourceCode.Substring(3, 4).TrimStart('0').PadLeft(1, '0'));
            parentId = PumpOutcomesClsRow(code, codeType, parentId, name, 3);
            // 4 - XXX.XXXX.XXX.00.00.000
            code = string.Format("{0}0000000", sourceCode.Substring(0, 10)).TrimStart('0').PadLeft(1, '0');
            name = GetClsNameByCode(cacheBudKCSR, sourceCode.Substring(7, 3).PadRight(7, '0').TrimStart('0').PadLeft(1, '0'));
            parentId = PumpOutcomesClsRow(code, codeType, parentId, name, 4);
            // 5 - XXX.XXXX.XXX.XX.00.000
            code = string.Format("{0}00000", sourceCode.Substring(0, 12)).TrimStart('0').PadLeft(1, '0');
            name = GetClsNameByCode(cacheBudKCSR, sourceCode.Substring(7, 5).PadRight(7, '0').TrimStart('0').PadLeft(1, '0'));
            parentId = PumpOutcomesClsRow(code, codeType, parentId, name, 5);
            // 6 - XXX.XXXX.XXX.XX.XX.000
            code = string.Format("{0}000", sourceCode.Substring(0, 14)).TrimStart('0').PadLeft(1, '0');
            name = GetClsNameByCode(cacheBudKCSR, sourceCode.Substring(7, 7).TrimStart('0').PadLeft(1, '0'));
            return PumpOutcomesClsRow(code, codeType, parentId, name, 6);
        }

        private void FillOutcomesCls()
        {
            Dictionary<string, int> codes = new Dictionary<string,int>();
            // процедуру выполняем только для листовых элементов
            // запоминаем коды и айди записей, требующих выполнения процедуры
            foreach (DataRow row in dsOutcomes.Tables[0].Rows)
            {
                if (cacheOutcomesParents.ContainsKey(row["Id"].ToString()))
                    continue;
                string code = row["Code"].ToString().PadLeft(17, '0');
                if (code.TrimStart('0') == string.Empty)
                    continue;
                codes.Add(string.Format("{0}|{1}", code, row["CodeType"]), Convert.ToInt32(row["Id"]));
                // меняем пэрент ай ди листовой записи на фиктивный
                // нужно для исправлении ситуации, когда код записи на первом уровне будет совпадать с листовым
                // пэрент айди  у них тоже будет совпадать (нулл), и запись на первом уровне не создастся
                row["ParentId"] = -666;
                row["HierarchyLevel"] = 7;
            }
            FillRowsCache(ref cacheOutcomesHier, dsOutcomes.Tables[0], new string[] { "CODE", "ParentId" }, "|");
            // выполняем процедуру над данными записями
            // запоминаем айди записей, которым нужно проставить ParentId
            Dictionary<int, int> auxCache = new Dictionary<int, int>();
            foreach (KeyValuePair<string, int> pair in codes)
            {
                string[] code = pair.Key.Split(new char[] { '|' });
                int parentId = PumpOutcomesClsRows(code[0], code[1]);
                auxCache.Add(pair.Value, parentId);
                // возвращаем пэрент ай ди листового элемента на нулл
                cacheOutcomesRows[pair.Value]["ParentId"] = DBNull.Value;
            }
            // сохраняем добавленные записи
            UpdateData();
            // проставляем ParentId
            foreach (KeyValuePair<int, int> pair in auxCache)
            {
                DataRow row = cacheOutcomesRows[pair.Key];
                row["ParentId"] = pair.Value;
                string rowCode = row["Code"].ToString().PadLeft(17, '0');
                row["Name"] = GetClsNameByCode(cacheBudKVR, rowCode.Substring(14, 3).TrimStart('0').PadLeft(1, '0'));
            }
            UpdateData();
        }

        #endregion заполнение классификатора Расходы

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

        protected void FillBudgetLevel()
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
                foreach (DataRow row in dsBud.Tables[0].Rows)
                {
                    int refTerrType = FindCachedRow(cacheRegionsForPumpRefTerr, row["Name"].ToString(), -1);
                    row["RefBudgetLevels"] = GetRefBudgetLevels(refTerrType);
                }
                UpdateData();
            }
        }

        private void QueryProcessData()
        {
            // ищем первый попавшийся источник - фо 1 с годом равным году обрабатываемого источника
            string query = string.Format("select min(id) from DataSources where (year = {0}) and (DataCode = 1) and (SupplierCode = 'ФО')", this.DataSource.Year);
            int clsSourceId = 0; 
            object obj = this.DB.ExecQuery(query, QueryResultTypes.Scalar, new IDbDataParameter[] { });
            if (obj != DBNull.Value)
                clsSourceId = Convert.ToInt32(obj);
            InitDataSet(ref daBudKVSR, ref dsBudKVSR, clsBudKVSR, true, string.Format("SOURCEID = {0} and id > 0 and id < 1000000000", clsSourceId), string.Empty);
            InitDataSet(ref daBudFKR, ref dsBudFKR, clsBudFKR, true, string.Format("SOURCEID = {0} and id > 0 and id < 1000000000", clsSourceId), string.Empty);
            InitDataSet(ref daBudKCSR, ref dsBudKCSR, clsBudKCSR, true, string.Format("SOURCEID = {0} and id > 0 and id < 1000000000", clsSourceId), string.Empty);
            InitDataSet(ref daBudKVR, ref dsBudKVR, clsBudKVR, true, string.Format("SOURCEID = {0} and id > 0 and id < 1000000000", clsSourceId), string.Empty);
        }

        private void FillProcessCache()
        {
            FillRowsCache(ref cacheBudKVSR, dsBudKVSR.Tables[0], "Code", "Name");
            FillRowsCache(ref cacheBudFKR, dsBudFKR.Tables[0], "Code", "Name");
            FillRowsCache(ref cacheBudKCSR, dsBudKCSR.Tables[0], "Code", "Name");
            FillRowsCache(ref cacheBudKVR, dsBudKVR.Tables[0], "Code", "Name");
        }

        protected override void QueryDataForProcess()
        {
            QueryData();
            QueryProcessData();
            FillProcessCache();
        }

        protected override void ProcessDataSource()
        {
            FillBudgetLevel();
            FillBudgetLevelSources();
            FillOutcomesCls();
            FillRepCode();
            UpdateData();
        }

        protected override void DirectProcessData()
        {
            int year = -1;
            int month = -1;
            GetPumpParams(ref year, ref month);
            ProcessDataSourcesTemplate(year, month, "Выполняется установка иерархии и заполнение поля «Код_отчет» в классификаторе «Расходы.УФК»");
        }

        #endregion Обработка данных

    }
}
