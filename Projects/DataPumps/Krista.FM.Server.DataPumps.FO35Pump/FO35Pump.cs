using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Xml;
using Krista.FM.Common.Xml;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.ServerLibrary;

using WorkPlace;
using VariablesTools;
using System.Runtime.InteropServices;

namespace Krista.FM.Server.DataPumps.FO35Pump
{

    // ФО - 0035 - Исполнение кассового плана
    public partial class FO35PumpModule : CorrectedPumpModuleBase
    {

        #region Поля

        #region Классификаторы

        // Администратор.ИспКасПлан (d_KVSR_ExctCachPl)
        private IDbDataAdapter daKvsr;
        private DataSet dsKvsr;
        private IClassifier clsKvsr;
        private Dictionary<string, int> cacheKvsr = null;
        private int sourceIdKvsr = -1;
        // Районы.ФО_ИспКасПлан (d_Regions_ExctCachPl)
        private IDbDataAdapter daRegion;
        private DataSet dsRegion;
        private IClassifier clsRegion;
        private Dictionary<string, int> cacheRegion = null;
        private int sourceIdRegion = -1;
        // КД.ФО_ИспКасПлан (d_KD_ExctCachPl)
        private IDbDataAdapter daKd;
        private DataSet dsKd;
        private IClassifier clsKd;
        private Dictionary<string, int> cacheKd = null;
        private int sourceIdKd = -1;
        // Тип средств.ФО_ИспКасПлан (d_MeansType_ExctCachPl)
        private IDbDataAdapter daMeansType;
        private DataSet dsMeansType;
        private IClassifier clsMeansType;
        private Dictionary<string, int> cacheMeansType = null;
        private int sourceIdMeansType = -1;
        // Расходы.ФО_ИспКасПлан (d_R_ExctCachPl)
        private IDbDataAdapter daOutcomesCls;
        private DataSet dsOutcomesCls;
        private IClassifier clsOutcomesCls;
        private Dictionary<string, int> cacheOutcomesCls = null;
        private int sourceIdOutcomesCls = -1;
        // КОСГУ.ФО_ИспКасПлан (d_EKR_ExctCachPl)
        private IDbDataAdapter daEkr;
        private DataSet dsEkr;
        private IClassifier clsEkr;
        private Dictionary<string, int> cacheEkr = null;
        private int sourceIdEkr = -1;
        // СубЭКР.ФО_ИспКасПлан (d_SubKESR_ExctCachPl)
        private IDbDataAdapter daSubEkr;
        private DataSet dsSubEkr;
        private IClassifier clsSubEkr;
        private Dictionary<string, int> cacheSubEkr = null;
        private int sourceIdSubEkr = -1;

        #endregion Классификаторы

        #region Факты

        // Факт.ФО_Исполнение кассового плана (f_F_ExctCachPl)
        private IDbDataAdapter daFactFO35;
        private DataSet dsFactFO35;
        private IFactTable fctFactFO35;
        // Факт.ФО_Исполнение кассового плана_Расходы (f_F_ExpExctCachPl)
        private IDbDataAdapter daFactFO35Outcomes;
        private DataSet dsFactFO35Outcomes;
        private IFactTable fctFactFO35Outcomes;
        // Доходы.ФО_Исполнение кассового плана (f_D_ExctCachPl)
        private IDbDataAdapter daIncomes;
        private DataSet dsIncomes;
        private IFactTable fctIncomes;
        // Расходы.ФО_Исполнение кассового плана (f_R_ExctCachPl)
        private IDbDataAdapter daOutcomes;
        private DataSet dsOutcomes;
        private IFactTable fctOutcomes;

        // самара
        // Факт.ФО_Кассовый план исполнения бюджета_Расходы (f_F_CachPlBudR)
        private IDbDataAdapter daCachPlBudR;
        private DataSet dsCachPlBudR;
        private IFactTable fctCachPlBudR;
        // Факт.ФО_Кассовый план исполнения бюджета (f_F_CachPlBud)
        private IDbDataAdapter daCachPlBud;
        private DataSet dsCachPlBud;
        private IFactTable fctCachPlBud;

        #endregion Факты

        private decimal sumMultiplier = 1;
        private List<int> deletedDateList = null;
        private string kvsrCode = string.Empty;
        private Dictionary<string, int> kvsrCodesIndices = new Dictionary<string, int>();
        private ReportType reportType;
        private int kvsrCodeLine = 0;

        WorkplaceAutoObjectClass budWP = null;
        private const string BUD_SCRIPT_OMSK = "Обмен данными\\ПостроениеОтчета.abl";
        private const string BUD_SCRIPT_UNV = "Обмен данными\\УниверсальноеПостроениеОтчета.abl";
        private int curDate = 0;
        private int startDate = 0;
        private int startMonthDate = 0;
        private string dateMonth = string.Empty;
        private string dateYear = string.Empty;
        private string curDateMonth = string.Empty;

        #endregion Поля

        #region Структуры, перечисления

        // Тип отчета
        private enum ReportType
        {
            // Исполнение по доходам
            Incomes,
            // Исполнение по расходам
            Outcomes,
            // Исполнение по источникам
            Sources,
            // Остатки бюджетных средств
            CalcBalanc,
            // Остатки средств бюджета ГРБС
            BalancKVSR,
            // Поступления и расходования МБТ
            Transfer,
            // Капитальное строительство
            Capital,
            // Дорожная инфраструктура
            Traffic,
            // другие отчёты
            Other
        }

        #endregion Структуры, перечисления

        #region Закачка данных

        #region Работа с базой и кэшами

        protected override void QueryData()
        {
            if (this.Region == RegionName.YNAO)
            {
                QueryDataYanao();
                return;
            }

            if (this.Region == RegionName.Vologda)
            {
                QueryDataVologda();
                return;
            }

            if (this.Region == RegionName.Omsk)
            {
                QueryDataOmsk();
                return;
            }

            if (this.Region == RegionName.Novosibirsk)
            {
                QueryDataNovosib();
                return;
            }

            if (this.Region == RegionName.HMAO)
            {
                QueryDataHmao();
                return;
            }

            if (this.Region == RegionName.Samara)
            {
                InitClsDataSet(ref daKvsr, ref dsKvsr, clsKvsr);
                InitFactDataSet(ref daCachPlBudR, ref dsCachPlBudR, fctCachPlBudR);
                InitFactDataSet(ref daCachPlBud, ref dsCachPlBud, fctCachPlBud);
            }
            else
            {
                InitClsDataSet(ref daKvsr, ref dsKvsr, clsKvsr);
                sourceIdRegion = AddDataSource("ФО", "0035", ParamKindTypes.Year, string.Empty, this.DataSource.Year, 0, string.Empty, 0, string.Empty).ID;
                string constr = string.Format("SOURCEID = {0}", sourceIdRegion);
                InitDataSet(ref daRegion, ref dsRegion, clsRegion, false, constr, string.Empty);
                InitClsDataSet(ref daKd, ref dsKd, clsKd);
                InitClsDataSet(ref daMeansType, ref dsMeansType, clsMeansType);
                InitClsDataSet(ref daOutcomesCls, ref dsOutcomesCls, clsOutcomesCls);
                InitClsDataSet(ref daEkr, ref dsEkr, clsEkr);
                InitClsDataSet(ref daSubEkr, ref dsSubEkr, clsSubEkr);
                InitFactDataSet(ref daFactFO35, ref dsFactFO35, fctFactFO35);
                InitFactDataSet(ref daFactFO35Outcomes, ref dsFactFO35Outcomes, fctFactFO35Outcomes);
                InitFactDataSet(ref daIncomes, ref dsIncomes, fctIncomes);
                InitFactDataSet(ref daOutcomes, ref dsOutcomes, fctOutcomes);
            }

            FillCaches();
        }

        private void FillCaches()
        {
            if ((this.Region == RegionName.Chechnya) || (this.Region == RegionName.Naur))
                FillRowsCache(ref cacheKvsr, dsKvsr.Tables[0], "Code", "Id");
            else
                FillRowsCache(ref cacheKvsr, dsKvsr.Tables[0], new string[] { "Code", "Name" }, "|", "Id");

            if (this.Region == RegionName.Samara)
                return;

            FillRowsCache(ref cacheRegion, dsRegion.Tables[0], "OKATO", "Id");
            FillRowsCache(ref cacheKd, dsKd.Tables[0], "CodeStr", "Id");
            FillRowsCache(ref cacheMeansType, dsMeansType.Tables[0], "Code", "Id");
            FillRowsCache(ref cacheOutcomesCls, dsOutcomesCls.Tables[0], "Code", "Id");
            FillRowsCache(ref cacheEkr, dsEkr.Tables[0], "Code", "Id");
            FillRowsCache(ref cacheSubEkr, dsSubEkr.Tables[0], "Code", "Id");
        }

        protected override void UpdateData()
        {
            if (this.Region == RegionName.YNAO)
            {
                UpdateDataYanao();
                return;
            }

            if (this.Region == RegionName.Vologda)
            {
                UpdateDataVologda();
                return;
            }

            if (this.Region == RegionName.Omsk)
            {
                UpdateDataOmsk();
                return;
            }

            if (this.Region == RegionName.Novosibirsk)
            {
                UpdateDataNovosib();
                return;
            }

            if (this.Region == RegionName.HMAO)
            {
                UpdateDataHmao();
                return;
            }

            if (this.Region == RegionName.Samara)
            {
                UpdateDataSet(daKvsr, dsKvsr, clsKvsr);
                UpdateDataSet(daCachPlBudR, dsCachPlBudR, fctCachPlBudR);
                UpdateDataSet(daCachPlBud, dsCachPlBud, fctCachPlBud);
            }
            else
            {
                UpdateDataSet(daKvsr, dsKvsr, clsKvsr);
                UpdateDataSet(daRegion, dsRegion, clsRegion);
                UpdateDataSet(daKd, dsKd, clsKd);
                UpdateDataSet(daMeansType, dsMeansType, clsMeansType);
                UpdateDataSet(daOutcomesCls, dsOutcomesCls, clsOutcomesCls);
                UpdateDataSet(daEkr, dsEkr, clsEkr);
                UpdateDataSet(daSubEkr, dsSubEkr, clsSubEkr);
                UpdateDataSet(daFactFO35, dsFactFO35, fctFactFO35);
                UpdateDataSet(daFactFO35Outcomes, dsFactFO35Outcomes, fctFactFO35Outcomes);
                UpdateDataSet(daIncomes, dsIncomes, fctIncomes);
                UpdateDataSet(daOutcomes, dsOutcomes, fctOutcomes);
            }
        }

        private const string D_KVSR_GUID = "6317eb20-0b1a-4851-ba01-eff869635ad3";
        private const string D_REGION_GUID = "8c7a8db6-4ee4-4968-9071-9d1fee8fc185";
        private const string D_KD_GUID = "492510fc-a4a3-47f5-ba33-66aed51c3134";
        private const string D_MEANS_TYPE_GUID = "d6a8a447-5460-4735-910b-70ab45462909";
        private const string D_OUTCOMES_CLS_GUID = "e95f83ae-5082-48e2-a7b0-1725f34b299b";
        private const string D_EKR_GUID = "d00cc7ac-da58-424a-b8a0-d027b56270dc";
        private const string D_SUB_EKR_GUID = "369316e9-325c-448f-a008-e6b8a8c79dfd";

        private const string F_FO35_GUID = "6acf6529-b09d-42bf-86ca-6cdf9fce816a";
        private const string F_FO35_OUTCOMES_GUID = "d96e6326-962b-43ce-bed4-749174c36ace";
        private const string F_INCOMES_GUID = "e3dacff6-3547-4dea-b838-aac7415fa538";
        private const string F_OUTCOMES_GUID = "3a43d437-0e70-4566-9046-5f55525e8876";

        private const string F_CACH_PL_BUD_R_GUID = "b50d7187-a808-4e56-ac47-80a2a9ff55b3";
        private const string F_CACH_PL_BUD_GUID = "41916b06-310e-43d2-95cc-1c02827fc9a2";
        protected override void InitDBObjects()
        {
            if (this.Region == RegionName.YNAO)
            {
                InitDBObjectsYanao();
                return;
            }

            if (this.Region == RegionName.Vologda)
            {
                InitDBObjectsVologda();
                return;
            }

            if (this.Region == RegionName.Omsk)
            {
                InitDBObjectsOmsk();
                return;
            }

            if (this.Region == RegionName.Novosibirsk)
            {
                InitDBObjectsNovosib();
                return;
            }

            if (this.Region == RegionName.HMAO)
            {
                InitDBObjectsHmao();
                return;
            }

            if (this.Region == RegionName.Samara)
            {
                this.UsedFacts = new IFactTable[] { };
                this.UsedClassifiers = new IClassifier[] { };
                clsKvsr = this.Scheme.Classifiers[D_KVSR_GUID];
                fctCachPlBudR = this.Scheme.FactTables[F_CACH_PL_BUD_R_GUID];
                fctCachPlBud = this.Scheme.FactTables[F_CACH_PL_BUD_GUID];
            }
            else
            {
                clsKvsr = this.Scheme.Classifiers[D_KVSR_GUID];
                clsRegion = this.Scheme.Classifiers[D_REGION_GUID];
                clsKd = this.Scheme.Classifiers[D_KD_GUID];
                clsMeansType = this.Scheme.Classifiers[D_MEANS_TYPE_GUID];
                clsOutcomesCls = this.Scheme.Classifiers[D_OUTCOMES_CLS_GUID];
                clsEkr = this.Scheme.Classifiers[D_EKR_GUID];
                clsSubEkr = this.Scheme.Classifiers[D_SUB_EKR_GUID];
                if ((this.Region == RegionName.Chechnya) || (this.Region == RegionName.Naur))
                    this.UsedClassifiers = new IClassifier[] { };
                else
                    this.UsedClassifiers = new IClassifier[] { clsKvsr };
                this.UsedFacts = new IFactTable[] {
                    fctFactFO35 = this.Scheme.FactTables[F_FO35_GUID],
                    fctFactFO35Outcomes = this.Scheme.FactTables[F_FO35_OUTCOMES_GUID],
                    fctIncomes = this.Scheme.FactTables[F_INCOMES_GUID],
                    fctOutcomes = this.Scheme.FactTables[F_OUTCOMES_GUID] };
            }
        }

        protected override void PumpFinalizing()
        {
            if (this.Region == RegionName.YNAO)
            {
                PumpFinalizingYanao();
                return;
            }

            if (this.Region == RegionName.Vologda)
            {
                PumpFinalizingVologda();
                return;
            }

            if (this.Region == RegionName.Omsk)
            {
                PumpFinalizingOmsk();
                return;
            }

            if (this.Region == RegionName.Novosibirsk)
            {
                PumpFinalizingNovosib();
                return;
            }

            if (this.Region == RegionName.HMAO)
            {
                PumpFinalizingHmao();
                return;
            }

            if (this.Region == RegionName.Samara)
            {
                ClearDataSet(ref dsCachPlBudR);
                ClearDataSet(ref dsCachPlBud);
                ClearDataSet(ref dsKvsr);
            }
            else
            {
                ClearDataSet(ref dsFactFO35);
                ClearDataSet(ref dsFactFO35Outcomes);
                ClearDataSet(ref dsIncomes);
                ClearDataSet(ref dsOutcomes);

                ClearDataSet(ref dsKvsr);

                ClearDataSet(ref dsRegion);
                ClearDataSet(ref dsKd);
                ClearDataSet(ref dsMeansType);
                ClearDataSet(ref dsOutcomesCls);
                ClearDataSet(ref dsEkr);
                ClearDataSet(ref dsSubEkr);
            }
        }

        #endregion Работа с базой и кэшами

        #region Работа с Xls

        #region Общие методы

        private decimal CleanFactValue(string factValue)
        {
            factValue = CommonRoutines.TrimLetters(factValue.Trim()).Trim();
            return Convert.ToDecimal(factValue.PadLeft(1, '0'));
        }

        private int PumpXlsKd(string code, string name)
        {
            if (code == string.Empty)
                return -1;
            return PumpCachedRow(cacheKd, dsKd.Tables[0], clsKd, code,
                new object[] { "CodeStr", code, "Name", name });
        }

        private int PumpXlsOutcomes(string code, string name)
        {
            if (code == string.Empty)
                return -1;
            code = code.Substring(3, 14);
            return PumpCachedRow(cacheOutcomesCls, dsOutcomesCls.Tables[0], clsOutcomesCls, code,
                new object[] { "Code", code, "Name", name });
        }

        private int PumpXlsEkr(string code, string name)
        {
            if (code == string.Empty)
                return -1;
            code = code.Substring(17);
            return PumpCachedRow(cacheEkr, dsEkr.Tables[0], clsEkr, code,
                new object[] { "Code", code, "Name", name });
        }

        private int PumpXlsRegion(string codeStr, string name)
        {
            int code = 0;
            Int32.TryParse(codeStr, out code);
            return PumpCachedRow(cacheRegion, dsRegion.Tables[0], clsRegion, code.ToString(),
                new object[] { "Code", code, "Name", name.Trim(), "OKATO", 0 });
        }

        private int PumpXlsTransfert(string codeStr, string targetStr, string name)
        {
            int code = Convert.ToInt32(codeStr.Replace(".", string.Empty));
            object target = DBNull.Value;
            if (targetStr != string.Empty)
                target = targetStr;
            return PumpCachedRow(cacheTransfert, dsTransfert.Tables[0], clsTransfert, code.ToString(),
                new object[] { "Code", code, "Name", name, "Target", target });
        }

        private int PumpXlsFactExct(string codeStr, string name)
        {
            string direction = string.Empty;
            switch (reportType)
            {
                case ReportType.Capital:
                    direction = "Капитальное строительство";
                    break;
                case ReportType.Traffic:
                    direction = "Дорожная инфраструктура";
                    break;
            }

            int code = Convert.ToInt32(codeStr.Replace(".", string.Empty));
            return PumpCachedRow(cacheFactExct, dsFactExct.Tables[0], clsFactExct, code.ToString(),
                new object[] { "Code", code, "Name", name, "Direction", direction });
        }

        #endregion Общие методы

        #region факт фо35

        private Dictionary<string, string> GetXlsDataRow(ExcelHelper excelDoc, int curRow, object[] mapping)
        {
            Dictionary<string, string> dataRow = new Dictionary<string, string>();
            int count = mapping.GetLength(0);
            for (int i = 0; i < count; i += 2)
            {
                dataRow.Add(mapping[i].ToString(), excelDoc.GetValue(curRow, Convert.ToInt32(mapping[i + 1])));
            }
            return dataRow;
        }

        #region Константы

        private object[] XLS_MAPPING_FO35 = new object[] { "Plan", 45, "Fact", 46 };
        private object[] XLS_MAPPING_FO35_2012 = new object[] { "Plan", 40, "Fact", 41 };
        private object[] XLS_MAPPING_FO35_KURSK = new object[] { "Plan", 46, "Fact", 47 };
        private object[] GetXlsMapping()
        {
            if (this.Region == RegionName.Kursk)
                return XLS_MAPPING_FO35_KURSK;
            if (this.DataSource.Year >= 2012)
                return XLS_MAPPING_FO35_2012;
            return XLS_MAPPING_FO35;
        }

        private int[] REF_MARKS_FO35 = new int[] { 1, 2, 3, 5, 6, 7, 8, 10, 11, 12, 13, 14, 15, 16 };
        private int[] REF_MARKS_FO35_KURSK = new int[] { 1, 2, 3, 5, 6, 7, 8, 9, 10, 11, 12, 13, 15, 16 };
        private int[] GetRefMarksMapping()
        {
            if (this.Region == RegionName.Kursk)
                return REF_MARKS_FO35_KURSK;
            return REF_MARKS_FO35;
        }

        private int[] FIRST_ROWS = new int[] { 14, 18, 68 };
        private int[] FIRST_ROWS_2010 = new int[] { 14, 18, 64 };
        private int[] FIRST_ROWS_2011 = new int[] { 14, 18, 62 };
        private int[] FIRST_ROWS_2012 = new int[] { 10, 14, 54 };
        private int[] FIRST_ROWS_KURSK = new int[] { 20, 24, 72, 74, 77, 84, 82, 89 };
        private int[] GetFirstRowsMapping()
        {
            if (this.Region == RegionName.Kursk)
                return FIRST_ROWS_KURSK;

            if (this.DataSource.Year >= 2012)
                return FIRST_ROWS_2012;
            else if (this.DataSource.Year >= 2011)
                return FIRST_ROWS_2011;
            else if (this.DataSource.Year >= 2010)
                return FIRST_ROWS_2010;
            return FIRST_ROWS;
        }

        private int[] LAST_ROWS = new int[] { 16, 21, 74 };
        private int[] LAST_ROWS_2010 = new int[] { 16, 21, 70 };
        private int[] LAST_ROWS_2011 = new int[] { 16, 21, 68 };
        private int[] LAST_ROWS_2012 = new int[] { 12, 17, 60};
        private int[] LAST_ROWS_KURSK = new int[] { 22, 28, 72, 74, 77, 88, 82, 89 };
        private int[] GetLastRowsMapping()
        {
            if (this.Region == RegionName.Kursk)
                return LAST_ROWS_KURSK;

            if (this.DataSource.Year >= 2012)
                return LAST_ROWS_2012;
            else if (this.DataSource.Year >= 2011)
                return LAST_ROWS_2011;
            else if (this.DataSource.Year >= 2010)
                return LAST_ROWS_2010;
            return LAST_ROWS;
        }

        private ArrayList SUM_ROWS_KURSK = new ArrayList(new int[] { 24, 85, 86, 87 });

        #endregion Константы

        private void PumpFO35Fact(ExcelHelper excelDoc, int refDate)
        {
            object[] xlsMapping = GetXlsMapping();
            int[] refMarksFO35 = GetRefMarksMapping();
            int[] firstRows = GetFirstRowsMapping();
            int[] lastRows = GetLastRowsMapping();

            int refMarksIndex = -1;
            decimal sumFacts = 0;
            decimal sumPlans = 0;
            for (int i = 0; i <= firstRows.GetLength(0) - 1; i++)
            {
                for (int curRow = firstRows[i]; curRow <= lastRows[i]; curRow++)
                {
                    Dictionary<string, string> dataRow = GetXlsDataRow(excelDoc, curRow, xlsMapping);

                    decimal fact = CleanFactValue(dataRow["Fact"]) * sumMultiplier;
                    decimal plan = CleanFactValue(dataRow["Plan"]) * sumMultiplier;

                    sumFacts += fact;
                    sumPlans += plan;

                    if (this.Region == RegionName.Kursk && (SUM_ROWS_KURSK.Contains(curRow)))
                        continue;

                    refMarksIndex++;
                    object[] mapping = null;
                    if (this.Region == RegionName.Kursk)
                    {
                        mapping = new object[] {
                            "FactRep", sumFacts, "PlanRep", sumPlans, "RefYearDayUNV", refDate,
                            "RefMarks", refMarksFO35[refMarksIndex] };
                    }
                    else
                    {
                        mapping = new object[] {
                            "Fact", sumFacts, "Plane", sumPlans, "RefYearDayUNV", refDate,
                            "RefMarks", refMarksFO35[refMarksIndex] };
                    }
                    PumpRow(dsFactFO35.Tables[0], mapping);
                    if (dsFactFO35.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
                    {
                        UpdateData();
                        ClearDataSet(daFactFO35, ref dsFactFO35);
                    }
                    sumFacts = 0;
                    sumPlans = 0;
                }
            }
        }

        #endregion факт фо35

        #region факт фо35_расходы

        private const string LAST_ROW_MARK = "ВСЕГО";
        private int GetLastRow(ExcelHelper excelDoc, int firstRow)
        {
            if (this.Region == RegionName.Yaroslavl)
            {
                if (this.DataSource.Year >= 2012)
                    return 51;
                else if (this.DataSource.Year >= 2011)
                    return 59;
                else if (this.DataSource.Year >= 2010)
                    return 61;
                else
                    return 65;
            }
            else
            {
                for (int i = firstRow + 1; ; i++)
                {
                    string value = excelDoc.GetValue(i, 6).Trim().ToUpper();
                    if (value.EndsWith(LAST_ROW_MARK))
                        return i - 1;
                }
            }
        }

        private const string FIRST_ROW_START = "РАСХОДЫ";
        private const string FIRST_ROW_END = "ВСЕГО";
        private int GetFirstRow(ExcelHelper excelDoc)
        {
            if (this.Region == RegionName.Yaroslavl)
            {
                if (this.DataSource.Year >= 2012)
                    return 17;
                if (this.DataSource.Year >= 2011)
                    return 21;
                return 23;
            }
            else
            {
                for (int i = 0; ; i++)
                {
                    string value = excelDoc.GetValue(i, 6).Trim().ToUpper();
                    if (value.StartsWith(FIRST_ROW_START) && value.EndsWith(FIRST_ROW_END))
                        return i;
                }
            }
        }

        private int GetKvsrCodeIndex(string code)
        {
            if (!kvsrCodesIndices.ContainsKey(code))
                kvsrCodesIndices.Add(code, 1);
            else
                kvsrCodesIndices[code] += 1;
            return kvsrCodesIndices[code];
        }

        private int PumpKvsr(Dictionary<string, string> dataRow)
        {
            string code;
            if (this.DataSource.Year>=2012)
                code = CommonRoutines.RemoveLetters(dataRow["Code"].Trim().Substring(0, dataRow["Code"].Trim().IndexOf(' '))).TrimStart('0').PadLeft(1, '0');
            else code = dataRow["Code"].Trim().TrimStart('0').PadLeft(1, '0');
            // если код не заполнен, то это код предыдущей строки
            if ((code == "0") && (kvsrCode != "-1"))
                code = kvsrCode;
            else
                kvsrCode = code;

            string codeIndex = string.Empty;
            if (this.Region == RegionName.Kursk)
            {
                codeIndex = code;
                code = string.Format("8{0:00}", Convert.ToInt32(code));
            }
            else
            {
                // получаем код строки (код + индекс кода (встречается в первый раз - 1, второй 2..))
                codeIndex = string.Format("{0}{1}", code, GetKvsrCodeIndex(code));
            }
            // "резервный фонд" - исключение, код у него должен быть 0
            if (codeIndex == "9202")
                code = "0";

            string name;
            if (this.DataSource.Year>=2012)
                name = CommonRoutines.RemoveNumbers(dataRow["Name"].Trim()).Replace(".", "").Trim();
            else name = dataRow["Name"].Trim();
            string key = string.Format("{0}|{1}", code, name);
            return PumpCachedRow(cacheKvsr, dsKvsr.Tables[0], clsKvsr, key,
                new object[] { "Code", code, "Name", name, "CodeLine", codeIndex });
        }

        private bool IsSkipRow(int curRow)
        {
            if (this.Region == RegionName.Yaroslavl)
                if (this.DataSource.Year >= 2012)
                    return (curRow == 18);
                else if (this.DataSource.Year >= 2011)
                    return (curRow == 22);
            return false;
        }

        private object[] XLS_MAPPING_FO35_OUTCOMES = new object[] {
            "Name", 6, "Code", 34, "Plan1", 35, "Fact1", 36, "Plan2", 37, "Fact2", 38, "Plan3", 39,
            "Fact3", 40, "Plan4", 41, "Fact4", 42, "Plan5", 43, "Fact5", 44, "Plan6", 45, "Fact6", 46 };
        private object[] XLS_MAPPING_FO35_OUTCOMES_2012 = new object[] {
            "Name", 1, "Code", 1, "Plan1", 28, "Fact1", 29, "Plan2", 30, "Fact2", 31, "Plan3", 32,
            "Fact3", 33, "Plan4", 34, "Fact4", 35, "Plan5", 38, "Fact5", 39, "Plan6", 40, "Fact6", 41, "Plan7", 36, "Fact7", 37 };
        private object[] XLS_MAPPING_FO35_OUTCOMES_KURSK = new object[] {
            "Name", 6, "Code", 2, "Plan1", 36, "Fact1", 37, "Plan2", 38, "Fact2", 39, "Plan3", 40,
            "Fact3", 41, "Plan4", 42, "Fact4", 43, "Plan5", 44, "Fact5", 45, "Plan6", 46, "Fact6", 47 };

        private object[] GetXlsMappingOutcomes()
        {
            if (this.Region == RegionName.Kursk)
                return XLS_MAPPING_FO35_OUTCOMES_KURSK;
            if (this.DataSource.Year >= 2012)
                return XLS_MAPPING_FO35_OUTCOMES_2012;
            return XLS_MAPPING_FO35_OUTCOMES;
        }

        private int[] REF_MARKS_FO35_OUTCOMES = new int[] { 1, 2, 3, 4, 5, 9 };
        private int[] REF_MARKS_FO35_OUTCOMES_2012 = new int[] { 1, 2, 3, 4, 5, 9, 13};

        private int[] GetRefMarksMappingOutcomes()
        {
            if (this.DataSource.Year >= 2012)
                return REF_MARKS_FO35_OUTCOMES_2012;
            return REF_MARKS_FO35_OUTCOMES;
        }

        private void PumpFO35FactOutcomes(ExcelHelper excelDoc, int refDate)
        {
            object[] xlsMapping = GetXlsMappingOutcomes();
            int firstRow = GetFirstRow(excelDoc);
            int lastRow = GetLastRow(excelDoc, firstRow);
            int[] refMarksFO35Outcomes = GetRefMarksMappingOutcomes();
            for (int curRow = firstRow; curRow <= lastRow; curRow++)
            {
                if (IsSkipRow(curRow))
                    continue;

                Dictionary<string, string> dataRow = GetXlsDataRow(excelDoc, curRow, xlsMapping);
                int refKvsr = PumpKvsr(dataRow);
                for (int i = 1; i <= (xlsMapping.Length-4)/4; i++)
                {
                    decimal fact = CleanFactValue(dataRow["Fact" + i.ToString()]) * sumMultiplier;
                    decimal plan = CleanFactValue(dataRow["Plan" + i.ToString()]) * sumMultiplier;
                    if ((fact == 0) && (plan == 0))
                        continue;
                    object[] mapping = null;
                    if (this.Region == RegionName.Kursk)
                    {
                        mapping = new object[] {
                            "FactRep", fact, "PlanRep", plan, "RefKVSR", refKvsr,
                            "RefYearDay", refDate, "RefRCachPl", refMarksFO35Outcomes[i - 1] };
                    }
                    else
                    {
                        mapping = new object[] {
                            "Fact", fact, "Plane", plan, "RefKVSR", refKvsr,
                            "RefYearDay", refDate, "RefRCachPl", refMarksFO35Outcomes[i - 1] };
                    }
                    PumpRow(dsFactFO35Outcomes.Tables[0], mapping);
                    if (dsFactFO35Outcomes.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
                    {
                        UpdateData();
                        ClearDataSet(daFactFO35Outcomes, ref dsFactFO35Outcomes);
                    }
                }
            }
        }

        #endregion факт фо35_расходы

        #region факт фо35_остатки

        /*
        private void PumpFO35CalcBalanc(ExcelHelper excelDoc, int refDate)
        {
            for (int curRow = 13; curRow < 20; curRow++)
            {
                int refMarks = curRow - 13;
                decimal balance = CleanFactValue(excelDoc.GetValue(curRow, 12));
                decimal income = CleanFactValue(excelDoc.GetValue(curRow, 13));
                decimal fbReturn = CleanFactValue(excelDoc.GetValue(curRow, 15));

                object[] mapping = new object[] { "Balance", balance, "income", income, "FBReturn", fbReturn,
                    "RefMarks", refMarks, "RefYearDayUNV", refDate };

                // поля UnReturn, ReturnOld, ReturnCurr необязательные, если там 0, то в базу их не сохраняем
                decimal value = CleanFactValue(excelDoc.GetValue(curRow, 14));
                if (value != 0)
                    mapping = (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "UnReturn", value });
                value = CleanFactValue(excelDoc.GetValue(curRow, 16));
                if (value != 0)
                    mapping = (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "ReturnOld", value });
                value = CleanFactValue(excelDoc.GetValue(curRow, 17));
                if (value != 0)
                    mapping = (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "ReturnCurr", value });

                PumpRow(dsCalcBalanc.Tables[0], mapping);
                if (dsCalcBalanc.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
                {
                    UpdateData();
                    ClearDataSet(daCalcBalanc, ref dsCalcBalanc);
                }
            }
        }
        */

        #endregion факт фо35_остатки

        #region факт фо35_остатки грбс

        private int PumpMarksFOTarget(ExcelHelper excelDoc, int curRow)
        {
            string name = excelDoc.GetValue(curRow, 3).Trim();
            string code = CommonRoutines.TrimLetters(excelDoc.GetValue(curRow, 5).Trim());
            int marksCode = Convert.ToInt32(code.PadLeft(1, '0'));
            string key = string.Format("{0}|{1}", marksCode, name);
            return PumpCachedRow(cacheMarksFOTarget, dsMarksFOTarget.Tables[0], clsMarksFOTarget, key,
                new object[] { "Code", marksCode, "Name", name });
        }

        private void PumpFO35BalancKVSR(ExcelHelper excelDoc, int refDate)
        {
            int refKvsr = -1;
            int rowsCount = excelDoc.GetRowsCount();
            for (int curRow = 5; curRow <= rowsCount; curRow++)
            {
                string cellValue = excelDoc.GetValue(curRow, 3).Trim();
                if (excelDoc.IsRowHidden(curRow) || (cellValue == string.Empty))
                    continue;

                if (excelDoc.GetValue(curRow, 7).Trim() == string.Empty)
                {
                    refKvsr = PumpCachedRow(cacheKvsrName, dsKvsr.Tables[0], clsKvsr, cellValue,
                        new object[] { "Code", 0, "Name", cellValue });
                    continue;
                }

                if (excelDoc.GetValue(curRow, 5).Trim() == string.Empty)
                    continue;

                int refMarks = PumpMarksFOTarget(excelDoc, curRow);
                decimal factFb = CleanFactValue(excelDoc.GetValue(curRow, 7));
                decimal factKvsr = CleanFactValue(excelDoc.GetValue(curRow, 8));
                decimal balance = CleanFactValue(excelDoc.GetValue(curRow, 9));

                object[] mapping = new object[] {
                    "FactFB", factFb, "FactKVSR", factKvsr, "Balance", balance,
                    "RefMarks", refMarks, "RefKVSR", refKvsr, "RefYearDayUNV", refDate };

                PumpRow(dsBalancKVSR.Tables[0], mapping);
                if (dsBalancKVSR.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
                {
                    UpdateData();
                    ClearDataSet(daBalancKVSR, ref dsBalancKVSR);
                }
            }
        }

        #endregion факт фо35_остатки грбс

        #region факт фо35_поступление и расходование МБТ

        private void PumpFactTransfer(decimal proceeds, decimal outgoing, decimal balance, object[] mapping)
        {
            mapping = (object[])CommonRoutines.ConcatArrays(mapping, new object[] {
                "Proceeds", proceeds, "Outgoing", outgoing, "Balance", balance });
            PumpRow(dsFOTransfer.Tables[0], mapping);
            if (dsFOTransfer.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
            {
                UpdateData();
                ClearDataSet(daFOTransfer, ref dsFOTransfer);
            }
        }

        private int PumpKvsrForTransfer(ExcelHelper excelDoc, int curRow)
        {
            string transferCode = excelDoc.GetValue(curRow, 6).Trim();
            // если в базе бюджета нет соответствия "КЦР" -> "КВСР", то ссылку на КВСР ставим -1
            if (!cacheKvsrFromBudget.ContainsKey(transferCode))
                return -1;

            // ключ квср = Код|Наименование
            string kvsrKey = cacheKvsrFromBudget[transferCode];
            // если в нашей базе уже есть квср с таким кодом и наименованием, то просто ставим на него ссылку
            if (cacheKvsr.ContainsKey(kvsrKey))
                return cacheKvsr[kvsrKey];

            // иначе добавляем в нашу базу запись квср и ставим на нее ссылку
            string[] kvsrData = kvsrKey.Split(new char[] { '|' });
            return PumpCachedRow(cacheKvsr, dsKvsr.Tables[0], clsKvsr, kvsrKey, new object[] {
                "Code", kvsrData[0], "Name", kvsrData[1], "CodeLine", kvsrData[0] });
        }

        private void PumpTranferRow(ExcelHelper excelDoc, int curRow, int refDate, int refRegion)
        {
            int refTransfert = -1;
            int refKvsr = -1;
            if (excelDoc.GetValue(curRow, 6).Trim() != string.Empty)
            {
                refTransfert = PumpXlsTransfert(
                    excelDoc.GetValue(curRow, 6).Trim(),
                    excelDoc.GetValue(curRow, 7).Trim(),
                    excelDoc.GetValue(curRow, 8).Trim());
                refKvsr = PumpKvsrForTransfer(excelDoc, curRow);
            }
            if (refTransfert == -1)
                return;

            int refKD = PumpXlsKd(excelDoc.GetValue(curRow, 9).Trim(), constDefaultClsName);
            int refOutcomes = PumpXlsOutcomes(excelDoc.GetValue(curRow, 11).Trim(), constDefaultClsName);
            int refEkr = PumpXlsEkr(excelDoc.GetValue(curRow, 11).Trim(), constDefaultClsName);

            object[] mapping = new object[] {
                "RefYearDayUNV", refDate, "RefTransf", refTransfert, "RefRegion", refRegion,
                "RefKD", refKD, "RefKVSR", refKvsr, "RefR", refOutcomes, "RefEKR", refEkr };

            decimal proceeds = CleanFactValue(excelDoc.GetValue(curRow, 10));
            decimal outgoing = CleanFactValue(excelDoc.GetValue(curRow, 12));
            decimal balance = proceeds - outgoing;
            // decimal balance = CleanFactValue(excelDoc.GetValue(curRow, 13));
            PumpFactTransfer(proceeds, outgoing, balance, mapping);
        }

        private bool IsSkipRow(ExcelHelper excelDoc, int curRow, bool toPump)
        {
            string cellValue = excelDoc.GetValue(curRow, 2).Trim().ToUpper();
            if (cellValue.Contains("ИТОГО"))
                return true;
            cellValue = excelDoc.GetValue(curRow, 6).Trim().ToUpper();
            if (cellValue.Contains("ИТОГО"))
                return true;
            return (!toPump && (cellValue == string.Empty));
        }

        private void PumpFO35Transfer(ExcelHelper excelDoc, string fileName, int refDate)
        {
            string regionCode = CommonRoutines.TrimLetters(fileName.Split('_')[2]);
            int refRegion = PumpXlsRegion(regionCode, excelDoc.GetValue("F1"));


            bool toPump = false;
            int rowsCount = excelDoc.GetRowsCount();
            for (int curRow = 1; curRow <= rowsCount; curRow++)
            {
                string cellValue = excelDoc.GetValue(curRow, 6).Trim();
                if (cellValue.ToUpper().Contains("ВСЕГО"))
                {
                    toPump = false;
                    continue;
                }

                if (IsSkipRow(excelDoc, curRow, toPump))
                    continue;

                if (toPump)
                    PumpTranferRow(excelDoc, curRow, refDate, refRegion);

                if (cellValue.ToUpper() == "КОД ЦЕЛЕВЫХ СРЕДСТВ")
                    toPump = true;
            }
        }

        #endregion факт фо35_поступление и расходование МБТ

        #region факт фо35_объекты капитального строительства

        private void PumpFactCapital(string factField, string factValue, int refDate, int refRegion, int refTransfert, int refFactExct, int refBdgt)
        {
            decimal fact = CleanFactValue(factValue);
            if (fact == 0)
                return;

            object[] mapping = new object[] {
                factField, Convert.ToDecimal(factValue),
                "RefFact", refFactExct,
                "RefRegion", refRegion,
                "RefYearDayUNV", refDate,
                "RefTransf", refTransfert,
                "RefBdgtLvls", refBdgt
            };

            PumpRow(dsFOFinObj.Tables[0], mapping);
            if (dsFOFinObj.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
            {
                UpdateData();
                ClearDataSet(daFOFinObj, ref dsFOFinObj);
            }
        }

        private void PumpCapitalRow(ExcelHelper excelDoc, int curRow, int refDate, int refRegion)
        {
            int refTransfert = PumpXlsTransfert(
                excelDoc.GetValue(curRow, 4).Trim(),
                excelDoc.GetValue(curRow, 6).Trim(),
                excelDoc.GetValue(curRow, 5).Trim());

            int refFactExct = PumpXlsFactExct(
                 excelDoc.GetValue(curRow, 2).Trim(),
                 excelDoc.GetValue(curRow, 3).Trim());

            PumpFactCapital("BdgtCurrY", excelDoc.GetValue(curRow, 7), refDate, refRegion, refTransfert, refFactExct, 3);
            PumpFactCapital("Outgoing", excelDoc.GetValue(curRow, 8), refDate, refRegion, refTransfert, refFactExct, 3);
            PumpFactCapital("BdgtCurrY", excelDoc.GetValue(curRow, 9), refDate, refRegion, refTransfert, refFactExct, 18);
            PumpFactCapital("Outgoing", excelDoc.GetValue(curRow, 10), refDate, refRegion, refTransfert, refFactExct, 18);
            PumpFactCapital("BdgtCurrY", excelDoc.GetValue(curRow, 11), refDate, refRegion, refTransfert, refFactExct, 9);
            PumpFactCapital("Outgoing", excelDoc.GetValue(curRow, 12), refDate, refRegion, refTransfert, refFactExct, 9);
            PumpFactCapital("BdgtFirstY", excelDoc.GetValue(curRow, 13), refDate, refRegion, refTransfert, refFactExct, 3);
            PumpFactCapital("BdgtFirstY", excelDoc.GetValue(curRow, 14), refDate, refRegion, refTransfert, refFactExct, 18);
            PumpFactCapital("BdgtFirstY", excelDoc.GetValue(curRow, 15), refDate, refRegion, refTransfert, refFactExct, 9);
            PumpFactCapital("BdgtSecY", excelDoc.GetValue(curRow, 16), refDate, refRegion, refTransfert, refFactExct, 3);
            PumpFactCapital("BdgtFirstY", excelDoc.GetValue(curRow, 17), refDate, refRegion, refTransfert, refFactExct, 18);
            PumpFactCapital("BdgtFirstY", excelDoc.GetValue(curRow, 18), refDate, refRegion, refTransfert, refFactExct, 9);
        }

        private void PumpFO35Capital(ExcelHelper excelDoc, string filename, int refDate)
        {
            string regionCode = CommonRoutines.TrimLetters(filename.Split('_')[2]);
            int refRegion = PumpXlsRegion(regionCode, excelDoc.GetValue("A1"));

            bool toPump = false;
            int rowsCount = excelDoc.GetRowsCount();
            for (int curRow = 1; curRow <= rowsCount; curRow++)
                try
                {
                    string cellValue = excelDoc.GetValue(curRow, 2).Trim();
                    if ((cellValue == string.Empty) || cellValue.ToUpper().StartsWith("ИТОГО"))
                        continue;

                    if (toPump)
                        PumpCapitalRow(excelDoc, curRow, refDate, refRegion);

                    if (cellValue == "1")
                        toPump = true;
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format(
                        "При обработке строки {0} листа '{1}' возникла ошибка ({2})",
                        curRow, excelDoc.GetWorksheetName(), ex.Message), ex);
                }
        }

        #endregion факт фо35_объекты капитального строительства

        #region Факты для Карелии и Пензы

        private void PumpFact(decimal fact, decimal plan, int refMark, int refDate)
        {
            object[] mapping = new object[] {
                "FactRep", fact, "PlanRep", plan, "RefYearDayUNV", refDate, "RefMarks", refMark };
            PumpRow(dsFactFO35.Tables[0], mapping);
            if (dsFactFO35.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
            {
                UpdateData();
                ClearDataSet(daFactFO35, ref dsFactFO35);
            }
        }

        // 0 - начальная строка,       2 - столбец с фактом
        // 1 - столбец с кодом дохода, 3 - столбец с планом
        private int[] INCOMES_INDICES_KARELIA = new int[] { 6, 5, 7, 6 };
        private int[] INCOMES_INDICES_PENZA = new int[] { 16, 4, 12, 11 };
        private void PumpFO35Incomes(ExcelHelper excelDoc, int refDate)
        {
            int[] indices;
            if (this.Region == RegionName.Karelya)
                indices = INCOMES_INDICES_KARELIA;
            else
                indices = INCOMES_INDICES_PENZA;

            decimal fact = CleanFactValue(excelDoc.GetValue(indices[0], indices[2]));
            decimal plan = CleanFactValue(excelDoc.GetValue(indices[0], indices[3]));
            PumpFact(fact, plan, 3, refDate);

            int index = 0;
            decimal[] facts = new decimal[2];
            decimal[] plans = new decimal[2];
            for (int curRow = (indices[0] + 1); ; curRow++)
            {
                string cellValue = excelDoc.GetValue(curRow, indices[1]).Trim();
                if (cellValue == string.Empty)
                    break;

                if (cellValue.StartsWith("1"))
                    index = 0;
                else if (cellValue.StartsWith("2"))
                    index = 1;
                else
                    continue;

                facts[index] += CleanFactValue(excelDoc.GetValue(curRow, indices[2]));
                plans[index] += CleanFactValue(excelDoc.GetValue(curRow, indices[3]));
            }
            PumpFact(facts[0], plans[0], 5, refDate);
            PumpFact(facts[1], plans[1], 6, refDate);
        }

        private int GetSourceIndex(string value)
        {
            if (this.Region == RegionName.Karelya)
                value = value.Substring(value.Length - 3);
            if (value == "710")
                return 0;
            if (value == "810")
                return 1;
            if (value == "540")
                return 2;
            if (value == "640")
                return 3;
            if (value == "530")
                return 4;
            if (value == "630")
                return 5;
            return -1;
        }

        // 0 - начальная строка,          2 - столбец с фактом
        // 1 - столбец с кодом источника, 3 - столбец с планом
        private int[] SOURCES_INDICES_KARELIA = new int[] { 6, 6, 8, 7 };
        private int[] SOURCES_INDICES_PENZA_2009 = new int[] { 16, 7, 22, 21 };
        private int[] SOURCES_INDICES_PENZA_2010 = new int[] { 9, 7, 10, 9 };
        private void PumpFO35Sources(ExcelHelper excelDoc, int refDate)
        {
            int[] indices;
            if (this.Region == RegionName.Karelya)
                indices = SOURCES_INDICES_KARELIA;
            else if (this.DataSource.Year >= 2010)
                indices = SOURCES_INDICES_PENZA_2010;
            else
                indices = SOURCES_INDICES_PENZA_2009;

            decimal fact = CleanFactValue(excelDoc.GetValue(indices[0], indices[2]));
            decimal plan = CleanFactValue(excelDoc.GetValue(indices[0], indices[3]));
            PumpFact(fact, plan, 9, refDate);

            decimal[] plans = new decimal[6];
            decimal[] facts = new decimal[6];
            for (int curRow = (indices[0] + 1); ; curRow++)
            {
                string cellValue = excelDoc.GetValue(curRow, indices[1]).Trim();
                if (cellValue == string.Empty)
                    break;

                int index = GetSourceIndex(cellValue);
                if (index < 0)
                    continue;

                facts[index] += CleanFactValue(excelDoc.GetValue(curRow, indices[2]));
                plans[index] += CleanFactValue(excelDoc.GetValue(curRow, indices[3]));
            }

            for (int i = 0; i < 6; i++)
                if ((facts[i] != 0) || (plans[i] != 0))
                    PumpFact(facts[i], plans[i], i + 10, refDate);
        }

        private int GetRefRCach(string code)
        {
            if (code.EndsWith("211000") || code.EndsWith("211"))
                return 1;
            if (code.EndsWith("213000") || code.EndsWith("213"))
                return 2;
            if (code.EndsWith("223000") || code.EndsWith("223"))
                return 3;
            if (code.EndsWith("251000") || code.EndsWith("251"))
                return 4;
            if (code.EndsWith("290000") || code.EndsWith("290"))
                return 5;
            if ((this.Region == RegionName.Karelya && code == "000") ||
                (this.Region == RegionName.Penza && code.Length == 3))
                return 9;
            return -1;
        }

        private int PumpKvsrExt(ExcelHelper excelDoc, int row, int column)
        {
            kvsrCodeLine++;
            string code = excelDoc.GetValue(row, column).Trim().TrimStart('0').PadLeft(1, '0');
            string name;
            if (this.Region == RegionName.Karelya)
                name = "Неуказанное наименование";
            else
                name = excelDoc.GetValue(row, 3).Trim();
            string key = string.Format("{0}|{1}", code, name);
            return PumpCachedRow(cacheKvsr, dsKvsr.Tables[0], clsKvsr, key,
                new object[] { "Code", code, "Name", name, "CodeLine", kvsrCodeLine });
        }

        // 0 - начальная строка,          3 - столбец с кодом классификатора
        // 1 - начальная строка расходов, 4 - столбец с фактом
        // 2 - столбец с кодом расхода,   5 - столбец с планом
        private int[] OUTCOMES_INDICES_KARELIA = new int[] { 6, 6, 9, 5, 11, 10 };
        private int[] OUTCOMES_INDICES_PENZA = new int[] { 14, 15, 2, 2, 6, 5 };
        private void PumpFO35Outcomes(ExcelHelper excelDoc, int refDate)
        {
            int[] indices;
            if (this.Region == RegionName.Karelya)
                indices = OUTCOMES_INDICES_KARELIA;
            else
                indices = OUTCOMES_INDICES_PENZA;

            decimal fact = CleanFactValue(excelDoc.GetValue(indices[0], indices[4]));
            decimal plan = CleanFactValue(excelDoc.GetValue(indices[0], indices[5])); ;
            PumpFact(fact, plan, 8, refDate);

            int refKvsr = -1;
            kvsrCodeLine = 0;
            for (int curRow = indices[1]; ; curRow++)
            {
                string cellValue = excelDoc.GetValue(curRow, indices[2]).Trim();
                if (cellValue == string.Empty)
                    break;

                int refRCach = GetRefRCach(cellValue);
                if (refRCach < 0)
                    continue;

                if ((this.Region == RegionName.Karelya && Convert.ToInt32(cellValue) != 0) ||
                    (this.Region == RegionName.Penza && cellValue.Length == 3))
                    refKvsr = PumpKvsrExt(excelDoc, curRow, indices[3]);

                fact = CleanFactValue(excelDoc.GetValue(curRow, indices[4]));
                plan = CleanFactValue(excelDoc.GetValue(curRow, indices[5]));
                if (fact == 0 && plan == 0)
                    continue;

                object[] mapping = new object[] { "FactRep", fact, "PlanRep", plan,
                    "RefKVSR", refKvsr, "RefYearDay", refDate, "RefRCachPl", refRCach };
                PumpRow(dsFactFO35Outcomes.Tables[0], mapping);
                if (dsFactFO35Outcomes.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
                {
                    UpdateData();
                    ClearDataSet(daFactFO35Outcomes, ref dsFactFO35Outcomes);
                }
            }
        }

        #endregion Факты для Карелии и Пензы

        private void DeleteEarlierDataByDate(int refDate)
        {
            if (!deletedDateList.Contains(refDate))
            {
                if (reportType == ReportType.BalancKVSR)
                {
                    string constr = string.Format("RefYearDayUNV = {0}", refDate);
                    DirectDeleteFactData(new IFactTable[] { fctBalancKVSR }, -1, this.SourceID, constr);
                }
                else if (reportType == ReportType.Transfer)
                {
                    string constr = string.Format("RefYearDayUNV = {0}", refDate);
                    DirectDeleteFactData(new IFactTable[] { fctFOTransfer }, -1, this.SourceID, constr);
                }
                else if ((reportType == ReportType.Capital) || (reportType == ReportType.Traffic))
                {
                    string constr = string.Format("RefYearDayUNV = {0}", refDate);
                    DirectDeleteFactData(new IFactTable[] { fctFOFinObj }, -1, this.SourceID, constr);
                }
                else
                {
                    string constr = string.Format("RefYearDay = {0}", refDate);
                    DirectDeleteFactData(new IFactTable[] { fctFactFO35Outcomes }, -1, this.SourceID, constr);
                    constr = string.Format("RefYearDayUNV = {0}", refDate);
                    DirectDeleteFactData(new IFactTable[] { fctFactFO35 }, -1, this.SourceID, constr);
                }
                deletedDateList.Add(refDate);
            }
        }

        private int GetRefDate(ExcelHelper excelDoc, string fileName)
        {
            string value = string.Empty;
            switch (this.Region)
            {
                case RegionName.Karelya:
                    value = fileName.Substring(fileName.Length - 12, 8);
                    break;
                case RegionName.Kursk:
                    value = excelDoc.GetValue("A7").Trim();
                    value = value.Substring(value.Length - 10);
                    sumMultiplier = 1;
                    break;
                case RegionName.Novosibirsk:
                    if (reportType == ReportType.Transfer)
                        value = CommonRoutines.TrimLetters(excelDoc.GetValue("F3").Trim());
                    else
                        value = CommonRoutines.TrimLetters(excelDoc.GetValue("I2").Trim());
                    break;
                case RegionName.Penza:
                    if ((reportType == ReportType.Sources) && (this.DataSource.Year >= 2010))
                        value = excelDoc.GetValue("A2").Trim();
                    else if (reportType == ReportType.Outcomes)
                        value = excelDoc.GetValue("A8").Trim();
                    else
                        value = excelDoc.GetValue("A6").Trim();
                    value = value.Substring(value.Length - 10).Replace(" ", string.Empty);
                    break;
                default:
                    if (this.DataSource.Year<2012)
                        value = excelDoc.GetValue("AL9").Trim();
                    else value = excelDoc.GetValue("AG5").Trim();
                    sumMultiplier = 1000;
                    break;
            }

            int refDate = -1;
            // из имени файла "Капитальное строительство_ГГГГММДД_ИИИИИ.xls" или
            // "Дорожная инфраструктура_ГГГГММДД_ИИИИИ.xls"
            if ((reportType == ReportType.Capital) || (reportType == ReportType.Traffic))
                refDate = Convert.ToInt32(fileName.Split('_')[1].Trim());
            else
                refDate = CommonRoutines.ShortDateToNewDate(CommonRoutines.TrimLetters(value));
            if (refDate == -1)
                throw new Exception("Не удалось определить дату отчёта. Вероятно, файл не соответствует формату.");

            DeleteEarlierDataByDate(refDate);

            return refDate;
        }

        private ReportType GetReportType(string value)
        {
            value = value.Trim().ToUpper();
            switch (this.Region)
            {
                case RegionName.Karelya:
                case RegionName.Penza:
                    if (value.Contains("ДОХОД"))
                        return ReportType.Incomes;
                    else if (value.Contains("РАСХОД"))
                        return ReportType.Outcomes;
                    return ReportType.Sources;
                case RegionName.Novosibirsk:
                    if (value.StartsWith("МБТ"))
                        return ReportType.Transfer;
                    if (value.StartsWith("КАПИТАЛЬНОЕ"))
                        return ReportType.Capital;
                    if (value.StartsWith("ДОРОЖНАЯ"))
                        return ReportType.Traffic;
                    return ReportType.BalancKVSR;
                default:
                    return ReportType.Other;
            }
        }

        private void PumpXlsSheet(ExcelHelper excelDoc, string fileName)
        {
            string worksheetName = excelDoc.GetWorksheetName();
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation,
                string.Format("Начало закачки листа '{0}'", worksheetName));

            if (this.Region == RegionName.Karelya)
                reportType = GetReportType(worksheetName);
            else
                reportType = GetReportType(fileName);

            int refDate = GetRefDate(excelDoc, fileName);
            switch (reportType)
            {
                case ReportType.Other:
                    PumpFO35Fact(excelDoc, refDate);
                    PumpFO35FactOutcomes(excelDoc, refDate);
                    break;
                case ReportType.Incomes:
                    PumpFO35Incomes(excelDoc, refDate);
                    break;
                case ReportType.Outcomes:
                    PumpFO35Outcomes(excelDoc, refDate);
                    break;
                case ReportType.Sources:
                    PumpFO35Sources(excelDoc, refDate);
                    break;
                    /*
                case ReportType.CalcBalanc:
                    PumpFO35CalcBalanc(excelDoc, refDate);
                    break;
                    */
                case ReportType.BalancKVSR:
                    PumpFO35BalancKVSR(excelDoc, refDate);
                    break;
                case ReportType.Transfer:
                    PumpFO35Transfer(excelDoc, fileName, refDate);
                    break;
                case ReportType.Capital:
                case ReportType.Traffic:
                    PumpFO35Capital(excelDoc, fileName, refDate);
                    break;
            }

            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation,
                string.Format("Завершение закачки листа '{0}'", worksheetName));
        }

        private void PumpXlsFile(FileInfo file)
        {
            WriteToTrace("Открытие документа: " + file.Name, TraceMessageKind.Information);
            ExcelHelper excelDoc = new ExcelHelper();
            try
            {
                excelDoc.AskToUpdateLinks = false;
                excelDoc.DisplayAlerts = false;
                excelDoc.EnableEvents = false;
                excelDoc.OpenDocument(file.FullName);
                int wsCount = excelDoc.GetWorksheetsCount();
                for (int index = 1; index <= wsCount; index++)
                {
                    if (this.Region != RegionName.Novosibirsk)
                    {
                        kvsrCodesIndices.Clear();
                        kvsrCode = "-1";
                    }
                    excelDoc.SetWorksheet(index);
                    PumpXlsSheet(excelDoc, file.Name);
                    UpdateData();
                }
            }
            finally
            {
                if (excelDoc != null)
                    excelDoc.CloseDocument();
            }
        }

        #endregion Работа с Xls

        #region закачка бюджета поселений

        #region работа с txt (классификаторы)

        private void PumpTxtCls(DirectoryInfo dir, Dictionary<string, int> cache, DataTable dt, IClassifier cls,
            string fileSearchPattern, string rowMark, object[] mapping)
        {
            FileInfo[] files = dir.GetFiles(fileSearchPattern);
            if (files.GetLength(0) == 0)
                return;
            foreach (FileInfo file in files)
            {
                string[] reportData = CommonRoutines.GetTxtReportData(file, CommonRoutines.GetTxtWinCodePage());
                foreach (string row in reportData)
                {
                    string[] values = row.Split('|');
                    if (!values[0].ToUpper().Contains(rowMark))
                        continue;
                    if (cls == clsKd)
                    {
                        if (values[4].ToUpper() != "20")
                            continue;
                    }
                    if (cls == clsOutcomesCls)
                    {
                        if (values[4].ToUpper() != "10")
                            continue;
                    }
                    object[] rowMapping = (object[])mapping.Clone();
                    for (int i = 0; i <= rowMapping.GetLength(0) - 1; i += 2)
                    {
                        string value = values[Convert.ToInt32(rowMapping[i + 1])].TrimStart('0').Trim();
                        if (rowMapping[i].ToString().ToUpper() == "NAME")
                            if (value == string.Empty)
                                value = constDefaultClsName;
                        rowMapping[i + 1] = value;
                    }
                    if (cls == clsOutcomesCls)
                        rowMapping[1] = rowMapping[1].ToString().Substring(3, 14).TrimStart('0').PadLeft(1, '0');
                    PumpCachedRow(cache, dt, cls, rowMapping[1].ToString(), rowMapping);
                }
            }
        }

        // инициализация эталонного классификатора
        // если за текущий месяц данных нет - берем предыдущий. за январь данные есть палюбасу
        private int InitTxtCls(ref IDbDataAdapter da, ref DataSet ds, IClassifier cls)
        {
            object sourceId = this.SourceID;
            for (int curMonth = this.DataSource.Month; curMonth >= 1; curMonth--)
            {
                string query = string.Format("select id from DataSources where Deleted = 0 and SUPPLIERCODE = 'ФО' and DATACODE = 35 and year = {0} and month = {1}",
                    this.DataSource.Year, curMonth);
                sourceId = this.DB.ExecQuery(query, QueryResultTypes.Scalar, new IDbDataParameter[] { });
                if ((sourceId == null) || (sourceId == DBNull.Value))
                    continue;
                string constr = string.Format("SOURCEID = {0}", sourceId);
                InitDataSet(ref da, ref ds, cls, false, constr, string.Empty);
                if (ds.Tables[0].Rows.Count > 0)
                    break;
            }
            if (Convert.ToInt32(sourceId) == 0)
                sourceId = this.SourceID;
            return Convert.ToInt32(sourceId);
        }

        private void PumpTxtClss(DirectoryInfo dir)
        {
            PumpTxtCls(dir, cacheKd, dsKd.Tables[0], clsKd, "*.DK*", "DKST", new object[] { "CodeStr", 5, "Name", 6});
            PumpTxtCls(dir, cacheOutcomesCls, dsOutcomesCls.Tables[0], clsOutcomesCls,
                "*.DK*", "DKST", new object[] { "Code", 5, "Name", 6 });
            PumpTxtCls(dir, cacheKvsr, dsKvsr.Tables[0], clsKvsr, "*.FD*", "FDST", new object[] { "Code", 4, "Name", 5 });
            PumpTxtCls(dir, cacheEkr, dsEkr.Tables[0], clsEkr, "*.FG*", "FGST", new object[] { "Code", 4, "Name", 5 });
            PumpTxtCls(dir, cacheMeansType, dsMeansType.Tables[0], clsMeansType, "*.FZ*", "FZST", new object[] { "Code", 4, "Name", 5 });
            PumpTxtCls(dir, cacheSubEkr, dsSubEkr.Tables[0], clsSubEkr, "*.FY*", "FYST", new object[] { "Code", 4, "Name", 5 });

            sourceIdKd = this.SourceID;
            sourceIdOutcomesCls = this.SourceID;
            sourceIdKvsr = this.SourceID;
            sourceIdEkr = this.SourceID;
            sourceIdMeansType = this.SourceID;
            sourceIdSubEkr = this.SourceID;

            if (cacheKd.Count == 0)
                sourceIdKd = InitTxtCls(ref daKd, ref dsKd, clsKd);
            if (cacheOutcomesCls.Count == 0)
                sourceIdOutcomesCls = InitTxtCls(ref daOutcomesCls, ref dsOutcomesCls, clsOutcomesCls);
            if (cacheKvsr.Count == 0)
                sourceIdKvsr = InitTxtCls(ref daKvsr, ref dsKvsr, clsKvsr);
            if (cacheEkr.Count == 0)
                sourceIdEkr = InitTxtCls(ref daEkr, ref dsEkr, clsEkr);
            if (cacheMeansType.Count == 0)
                sourceIdMeansType = InitTxtCls(ref daMeansType, ref dsMeansType, clsMeansType);
            if (cacheSubEkr.Count == 0)
                sourceIdSubEkr = InitTxtCls(ref daSubEkr, ref dsSubEkr, clsSubEkr);

            FillCaches();
        }

        #endregion работа с txt (классификаторы)

        #region работа с xml (факты)

        private decimal GetSumValue(XmlNode clsNode, string sumName)
        {
            return Convert.ToDecimal(clsNode.Attributes[sumName].Value.Trim().Replace('.', ','));
        }

        private void PumpXmlIncomes(XmlDocument doc, int budLevel, int refRegion, int refDate)
        {
            XmlNodeList clsNodes = doc.SelectNodes("xml/fact_data/incomes/income_row");
            foreach (XmlNode node in clsNodes)
            {
                string kdCode = node.Attributes["kd_code"].Value;
                int refKd = PumpCachedRow(cacheKd, dsKd.Tables[0], clsKd, kdCode,
                    new object[] { "CodeStr", kdCode, "Name", constDefaultClsName, "SourceId", sourceIdKd });
                string meansTypeCode = node.Attributes["means_type"].Value;
                int refMeansType = PumpCachedRow(cacheMeansType, dsMeansType.Tables[0], clsMeansType,
                    meansTypeCode, new object[] { "Code", meansTypeCode, "Name", constDefaultClsName, "SourceId", sourceIdMeansType });

                object[] mapping = new object[] { "RefBdgtLev", budLevel, "RefRegions", refRegion, "RefYearDayUNV", refDate,
                    "RefKD", refKd, "RefMeansType", refMeansType, "Fact", GetSumValue(node, "fact"),
                    "Quart1", GetSumValue(node, "quart1"), "Quart2", GetSumValue(node, "quart2"),
                    "Quart3", GetSumValue(node, "quart3"), "Quart4", GetSumValue(node, "quart4") };
                PumpRow(dsIncomes.Tables[0], mapping);
                if (dsIncomes.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
                {
                    UpdateData();
                    ClearDataSet(daIncomes, ref dsIncomes);
                }
            }
        }

        private void PumpXmlOutcomes(XmlDocument doc, int budLevel, int refRegion, int refDate)
        {
            XmlNodeList clsNodes = doc.SelectNodes("xml/fact_data/outcomes/outcome_row");
            foreach (XmlNode node in clsNodes)
            {
                string meansTypeCode = node.Attributes["means_type"].Value;
                int refMeansType = PumpCachedRow(cacheMeansType, dsMeansType.Tables[0], clsMeansType,
                    meansTypeCode, new object[] { "Code", meansTypeCode, "Name", constDefaultClsName, "SourceId", sourceIdMeansType });
                string kvsrCode = node.Attributes["kvsr_code"].Value;
                int refKvsr = PumpCachedRow(cacheKvsr, dsKvsr.Tables[0], clsKvsr, kvsrCode,
                    new object[] { "Code", kvsrCode, "Name", constDefaultClsName, "SourceId", sourceIdKvsr });
                string ekrCode = node.Attributes["ekr_code"].Value;
                int refEkr = PumpCachedRow(cacheEkr, dsEkr.Tables[0], clsEkr, ekrCode,
                    new object[] { "Code", ekrCode, "Name", constDefaultClsName, "SourceId", sourceIdEkr });
                string subEkrCode = node.Attributes["subekr_code"].Value;
                int refSubEkr = PumpCachedRow(cacheSubEkr, dsSubEkr.Tables[0], clsSubEkr, subEkrCode,
                    new object[] { "Code", subEkrCode, "Name", constDefaultClsName, "SourceId", sourceIdSubEkr });

                string fkrCode = node.Attributes["fkr_code"].Value;
                string kcsrCode = node.Attributes["kcsr_code"].Value;
                string kvrCode = node.Attributes["kvr_code"].Value;
                string outcomesClsCode = string.Format("{0}{1}{2}", fkrCode, kcsrCode.PadLeft(7, '0'), kvrCode.PadLeft(3, '0'));
                int refOutcomesCls = PumpCachedRow(cacheOutcomesCls, dsOutcomesCls.Tables[0], clsOutcomesCls, outcomesClsCode,
                    new object[] { "Code", outcomesClsCode, "Name", constDefaultClsName, "SourceId", sourceIdOutcomesCls });

                object[] mapping = new object[] { "RefBdgtLev", budLevel, "RefRegions", refRegion, "RefYearDayUNV", refDate,
                    "RefMeansType", refMeansType, "RefKVSR", refKvsr, "RefEKR", refEkr, "RefR", refOutcomesCls, "RefSubEKR", refSubEkr,
                    "Fact", GetSumValue(node, "fact"), "POF", GetSumValue(node, "pof"),
                    "Quart1", GetSumValue(node, "quart1"), "Quart2", GetSumValue(node, "quart2"),
                    "Quart3", GetSumValue(node, "quart3"), "Quart4", GetSumValue(node, "quart4") };
                PumpRow(dsOutcomes.Tables[0], mapping);
                if (dsOutcomes.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
                {
                    UpdateData();
                    ClearDataSet(daOutcomes, ref dsOutcomes);
                }
            }
        }

        private int PumpXmlRegion(string code, string name)
        {
            return PumpCachedRow(cacheRegion, dsRegion.Tables[0], clsRegion, code,
                new object[] { "Okato", code, "Name", name, "SourceId", sourceIdRegion });
        }

        private int GetBudgetLevel(int nodeValue)
        {
            switch (nodeValue)
            {
                case 1:
                    return 3;
                case 2:
                    return 5;
                case 3:
                    return 4;
                case 4:
                    return 6;
                default:
                    return 3;
            }
        }

        private void PumpXmlFile(FileInfo file)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(file.FullName);
            try
            {
                XmlNode titleNode = doc.SelectSingleNode("xml/title");
                int budLevel = GetBudgetLevel(Convert.ToInt32(titleNode.Attributes["budget_level"].Value));
                int refRegion = PumpXmlRegion(titleNode.Attributes["okato"].Value, titleNode.Attributes["okato_name"].Value);
                int refDate = Convert.ToInt32(titleNode.Attributes["date"].Value);
                PumpXmlIncomes(doc, budLevel, refRegion, refDate);
                PumpXmlOutcomes(doc, budLevel, refRegion, refDate);

                clsOutcomesCls.DivideAndFormHierarchy(sourceIdOutcomesCls, this.PumpID, ref dsOutcomesCls);
                clsKd.DivideAndFormHierarchy(sourceIdKd, this.PumpID, ref dsKd);

            }
            finally
            {
                XmlHelper.ClearDomDocument(ref doc);
            }
        }

        #endregion работа с xml (факты)

        #endregion закачка бюджета поселений

        #region Перекрытые методы закачки

        protected override void ProcessFiles(DirectoryInfo dir)
        {
            deletedDateList = new List<int>();

            if (this.Region == RegionName.YNAO)
            {
                PumpYanaoData();
                return;
            }

            if (this.Region == RegionName.Vologda)
            {
                PumpVologdaData(dir);
                return;
            }

            if (this.Region == RegionName.Samara)
            {
                PumpSamaraData(dir);
                return;
            }

            if (this.Region == RegionName.Omsk)
            {
                PumpOmskData(dir);
                return;
            }

            if (this.Region == RegionName.HMAO)
            {
                PumpHmaoData(dir);
                return;
            }

            if (this.Region == RegionName.Novosibirsk)
            {
                PumpKvsrFromBudget(dir);
                PumpNovosibData(dir);
            }
            else
            {
                PumpTxtClss(dir);
                UpdateData();
                ProcessFilesTemplate(dir, "*.xml", new ProcessFileDelegate(PumpXmlFile), false);
                UpdateData();
            }
            ProcessFilesTemplate(dir, "*.xls*", new ProcessFileDelegate(PumpXlsFile), false);
            UpdateData();
        }

        protected override void DirectPumpData()
        {
            if (this.Region == RegionName.YNAO)
                CheckSourceDirToEmpty = false;

            if ((this.Region == RegionName.Chechnya) || (this.Region == RegionName.Samara) ||
                (this.Region == RegionName.Naur))
                PumpDataYMTemplate();
            else
                PumpDataYTemplate();
        }

        #endregion Перекрытые методы закачки

        #endregion Закачка данных

        #region Обработка данных

        private decimal SolveSum(Dictionary<int, string> cache, int[] ids)
        {
            decimal sum = 0;
            for (int i = 0; i < ids.Length; i++)
            {
                int id = ids[i];
                if (cache.ContainsKey(id))
                    sum += Convert.ToDecimal(cache[id].PadLeft(1, '0'));
            }
            return sum;
        }

        private void FillFacts(int refDate)
        {
            IDbDataAdapter da = null;
            DataSet ds = null;
            InitDataSet(ref da, ref ds, fctFactFO35, false,
                string.Format("SOURCEID = {0} and REFYEARDAYUNV = {1}", this.SourceID, refDate), string.Empty);

            if (ds.Tables[0].Rows.Count == 0)
                return;

            Dictionary<int, string> cachePlan = null;
            FillRowsCache(ref cachePlan, ds.Tables[0], "RefMarks", "PlanRep");
            Dictionary<int, string> cacheFact = null;
            FillRowsCache(ref cacheFact, ds.Tables[0], "RefMarks", "FactRep");

            decimal factFirst = SolveSum(cacheFact, new int[] { 3, 10, 13, 15 });
            decimal planFirst = SolveSum(cachePlan, new int[] { 3, 10, 13, 15 });
            PumpFact(factFirst, planFirst, 2, refDate);

            decimal factSecond = SolveSum(cacheFact, new int[] { 8, 11, 12, 14 });
            decimal planSecond = SolveSum(cachePlan, new int[] { 8, 11, 12, 14 });
            PumpFact(factSecond, planSecond, 7, refDate);

            PumpFact(factFirst - factSecond, planFirst - planSecond, 17, refDate);
        }

        private void AppendFacts()
        {
            foreach (int refDate in deletedDateList)
            {
                FillFacts(refDate);
            }
        }

        protected override void ProcessDataSource()
        {
            if (this.Region == RegionName.Penza || this.Region == RegionName.Karelya)
            {
                AppendFacts();
                UpdateData();
            }
            if (this.Region == RegionName.Samara)
                ProcessSamaraData();

            if (this.Region == RegionName.YNAO)
                ProcessYanaoData();

            if (this.Region == RegionName.Omsk)
                ProcessOmskData();

            if (this.Region == RegionName.Novosibirsk)
                ProcessNovosibData();
        }

        protected override void DirectProcessData()
        {
            if (this.Region != RegionName.Penza && this.Region != RegionName.Karelya && this.Region != RegionName.YNAO &&
                this.Region != RegionName.Samara && this.Region != RegionName.Omsk && this.Region != RegionName.Novosibirsk)
                return;
            ProcessDataSourcesTemplate("Добавление записей в таблицу фактов");
        }

        #endregion Обработка данных

    }
}
