using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using Krista.FM.Client.Reports.Common;
using Krista.FM.Client.Reports.Database.ClsData;
using Krista.FM.Client.Reports.Database.FactTables.Planning.Capital;
using Krista.FM.Client.Reports.Database.FactTables.Planning.CreditIncome;
using Krista.FM.Client.Reports.Database.FactTables.Planning.Garant;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.Reports
{
    public enum ReportDocumentType
    {
        CreditOrg,
        CreditBud,
        Capital,
        Garant
    }

    public static class ReportConsts
    {
        // внутренние ключи параметров
        public const string ParamStartDate = "IFStartDate";
        public const string ParamEndDate = "IFEndDate";
        public const string ParamVariantType = "IFVariantType";
        public const string ParamPeriodType = "IFPeriodType";
        public const string ParamVariantID = "IFVariantID";
        public const string ParamVariantRID = "IFRVariantID";
        public const string ParamVariantDID = "IFDVariantID";
        public const string ParamYear = "IFYear";
        public const string ParamMonth = "IFMonth";
        public const string ParamYearStart = "IFYearStart";
        public const string ParamYearEnd = "IFYearEnd";
        public const string ParamOrgID = "OrgID";
        public const string ParamOrgName = "OrgName";
        public const string ParamExchangeRate = "IFPlanExchange";
        public const string ParamStartExchangeRate = "IFStartExchange";
        public const string ParamPhone = "Phone";
        public const string ParamExecutor = "Executor";
        public const string ParamExecutor1 = "Executor1";
        public const string ParamExecutor2 = "Executor2";
        public const string ParamExecutor3 = "Executor3";
        public const string ParamRegionAnalisID = "RegionAnalisysID";
        public const string ParamReportList = "ReportList";
        public const string ParamTestType = "ParamTestType";
        public const string ParamKDComparable = "ParamKDComparable";
        public const string ParamKIFComparable = "ParamKIFComparable";
        public const string ParamKDUFK = "ParamKDUFK";
        public const string ParamKVSRComparable = "ParamKVSRComparable";
        public const string ParamBdgtLevels = "ParamBdgtLevels";
        public const string ParamBdgtLevelsSKIF = "ParamBdgtLevelsSKIF";
        public const string ParamRegionLevels = "ParamRegionLevels";
        public const string ParamRegionComparable = "ParamRegionComparable";
        public const string ParamPrecision = "ParamPrecision";
        public const string ParamSumModifier = "ParamSumModifier";
        public const string ParamRegNum = "ParamRegNum";
        public const string ParamMasterFilter = "ParamMasterFilter";
        public const string ParamCreditPlanFilter1 = "CreditPlanFilter1";
        public const string ParamCreditPlanFilter2 = "CreditPlanFilter2";
        public const string ParamExchangeDate = "ExchangeDate";
        public const string ParamDigitNumber = "ParamDigitNumber";
        public const string ParamRegionListType = "RegionListType";
        public const string ParamKDUfkGroupType = "KDUfkGroupType";
        public const string ParamOutputMode = "ParamOutputMode";
        public const string ParamKladrInfo = "ParamKladrInfo";
        public const string ParamOKOPF = "ParamOKOPF";
        public const string ParamKOPF = "ParamKOPF";
        public const string ParamMark = "ParamMark";
        public const string ParamSum = "ParamSum";
        public const string ParamOKATO = "ParamOKATO";
        public const string ParamINN = "ParamINN";
        public const string ParamOKVED = "ParamOKVED";
        public const string ParamHideEmptyStr = "ParamHideEmptyStr";
        public const string ParamGroupKD = "ParamGroupKD";
        public const string ParamArrearsFNS = "ParamArrearsFNS";
        public const string ParamReportDate = "ParamReportDate";
        public const string ParamQuarter = "ParamQuarter";
        public const string ParamHalfYear = "ParamHalfYear";
        public const string ParamStructure = "ParamStructure";
        public const string ParamMeans = "ParamMeans";
        public const string ParamDocType = "ParamDocType";
        public const string ParamIncomes = "ParamIncomes";
        public const string ParamPersons = "ParamPersons";
        // стартовое значение выбора из справочника
        public const string DefBookID = "0";
        // фиксированные коды справочника вариантов
        public const string ActiveVariantID = "0";
        public const string ArchivVariantID = "-2";
        public const string FixedVariantsID = "0,-2";
        // названи€ полей сум
        public const string SumField = "Sum";
        public const string CurrencySumField = "CurrencySum";
        public const string Commission = "Commission";
        public const string CurrencyCommission = "CurrencyCommission";
        public const string Margin = "Margin";
        public const string CurrencyMargin = "CurrencyMargin";
        // фиксированные коды типов документов
        public const string OrgCreditCode = "0";
        public const string BudCreditCode = "1";
        public const string AllCreditsCode = "0,1";
        public const string CreditIssuedOrgCode = "4";
        public const string CreditIssuedBudCode = "3";
        // типы сумм дл€ факта привлечений гарантий
        public const string GrntTypeSumDbt = "-1,1";
        public const string GrntTypeSumMainDbt = "1";
        public const string GrntTypeSumUndefined = "-1";
        public const string GrntTypeSumPct = "2";
        public const string GrntTypeSumPeniDbt = "3";
        public const string GrntTypeSumPeniPct = "4";
        public const string GrntTypeSumFine = "6";
        // типы сумм дл€ факта обслуживани€ цб
        public const string CapTypeSumDiscount = "9";
        public const string CapTypeSumCoupon = "8";
        public const string CapTypeSumCommission = "5";
        public const string CapRowPrimary = "0";
        public const string CapRowSecondary = "1";
        // строковые валюты
        public const string RUB = "RUB";
        public const string USD = "USD";
        public const string EUR = "EUR";
        public const string codeRUBStr = "-1";
        public const string codeUSDStr = "131";
        public const string codeEURStr = "163";
        public const int codeRUB = -1;
        public const int codeUSD = 131;
        public const int codeEUR = 163;
        // фиксированные статусы документов
        public const string StatusAccepted = "0";
        // фиксированные коды типов документов ƒ 
        public const string DKOrgCreditCode = "0";
        public const string DKBudCreditCode = "1";
        // строковые значени€ булевских
        public const string strTrue = "true";
        public const string strFalse = "false";
        // названи€ параметров объекта построител€
        public const string ParamLoDate = "StartDate";
        public const string ParamHiDate = "EndDate";
        // типы полей
        public const string ftDateTime = "DateTime";
        public const string ftInt32 = "Int32";
        public const string ftDecimal = "Decimal";
        public const string ftString = "String";
        // фиксированные коды районов
        public const string UndefinedRegion = "-1";
        // фиксированные коды кредитов предоставленных
        public const string OrgCreditIssuedCode = "4";
        public const string BudCreditIssuedCode = "3";
        // таблица источников
        public const string HUB_DATASOURCES = "HUB_DATASOURCES";
        // unused id
        public const string UndefinedKey = "-666";
        public const string OKTMO = "OKTMO";
        public const string KBK = "KBK";
        public const string RowType = "0";

        public const string ActiveCapStatus = "1";
        public const string ActiveCrdStatus = "0";
    }

    public class DataTableUtils
    {
        public static DataTable SortDataSet(DataTable dt, string orderStr)
        {
            var dtTemp = dt.Clone();
            var rows = dt.Select(String.Empty, orderStr);

            foreach (var t in rows)
            {
                dtTemp.ImportRow(t);
            }

            dtTemp.AcceptChanges();
            return dtTemp;
        }

        public static DataTable FilterDataSet(DataTable dt, string filterStr)
        {
            var dtTemp = dt.Clone();
            var rows = dt.Select(filterStr);
            
            foreach (var t in rows)
            {
                dtTemp.ImportRow(t);
            }

            dtTemp.AcceptChanges();
            return dtTemp;
        }

        public static DataTable MergeDataSet(DataTable dt1, DataTable dt2)
        {
            var dtTemp = dt1.Clone();

            for (var i = 0; i < dt1.Rows.Count; i++)
            {
                dtTemp.ImportRow(dt1.Rows[i]);
            }
            
            for (var i = 0; i < dt2.Rows.Count; i++)
            {
                dtTemp.ImportRow(dt2.Rows[i]);
            }

            return dtTemp;
        }
    }

    public partial class ReportDataServer
    {
        public IScheme scheme;

        //  ласс - набор параметров дл€ вологодских группировок кредитов по территории
        public class TerritoryColumnParam
        {
            public DataTable dtContracts, dtResult;
            public int columnKind, colIndex1, colIndex2, monthIndex;
            public decimal[] sumArray;
            public bool writeSummary, copyEachMonthRecords;
        }

        //  ласс - набор параметров дл€ типичной выборки по договорам
        public class CommonQueryParam
        {
            public string sourceID;
            public string variantID;
            public string yearStart;
            public string yearEnd;
        }

        // “ип территории
        public enum ReportTerritoryType
        {
            ttRegion,
            ttSubject,
            ttSettlement
        }

        // ћен€ убивает этот код, но функции не нашел котора€ склон€ет мес€ца
        public static string GetMonthText1(int monthNum)
        {
            if (monthNum == 1) return "€нваре";
            if (monthNum == 2) return "феврале";
            if (monthNum == 3) return "марте";
            if (monthNum == 4) return "апреле";
            if (monthNum == 5) return "мае";
            if (monthNum == 6) return "июне";
            if (monthNum == 7) return "июле";
            if (monthNum == 8) return "августе";
            if (monthNum == 9) return "сент€бре";
            if (monthNum == 10) return "окт€бре";
            if (monthNum == 11) return "но€бре";
            return "декабре";
        }

        public static string GetMonthText2(int monthNum)
        {
            if (monthNum == 1) return "€нвар€";
            if (monthNum == 2) return "феврал€";
            if (monthNum == 3) return "марта";
            if (monthNum == 4) return "апрел€";
            if (monthNum == 5) return "ма€";
            if (monthNum == 6) return "июн€";
            if (monthNum == 7) return "июл€";
            if (monthNum == 8) return "августа";
            if (monthNum == 9) return "сент€бр€";
            if (monthNum == 10) return "окт€бр€";
            if (monthNum == 11) return "но€бр€";
            return "декабр€";
        }

        private static string GetEndQuarter(string calcDate)
        {
            calcDate = Convert.ToDateTime(calcDate).ToShortDateString();
            var monthNum = Convert.ToDateTime(calcDate).Month;
            var year = Convert.ToDateTime(calcDate).Year;
            while (monthNum % 3 != 0) monthNum++;
            calcDate = String.Format("{0}.{1}.{2}", DateTime.DaysInMonth(year, monthNum), monthNum, year);
            return Convert.ToDateTime(calcDate).ToShortDateString();
        }

        private static string GetStartQuarter(int year, int num)
        {
            var date = new DateTime(year, num * 3, 1).AddMonths(1);
            return date.ToShortDateString();
        }

        private static string GetDKVariantByDate(IScheme scheme, string calcDate, ref string variantDate)
        {
            var result = String.Empty;
            variantDate = String.Empty;
            var dbHelper = new ReportDBHelper(scheme);
            var dtData = dbHelper.GetEntityData(d_Variant_Schuldbuch.internalKey);
            dtData = DataTableUtils.FilterDataSet(dtData, String.Format("{0} <= '{1}'", d_Variant_Schuldbuch.ReportDate, calcDate));
            dtData = DataTableUtils.SortDataSet(dtData, StrSortDown(d_Variant_Schuldbuch.ReportDate));
            
            if (dtData.Rows.Count > 0)
            {
                result = dtData.Rows[0][d_Variant_Schuldbuch.id].ToString();
                variantDate = Convert.ToDateTime(dtData.Rows[0][d_Variant_Schuldbuch.ReportDate]).ToShortDateString();
            }
            
            return result;
        }

        private static string GetSourceID(IScheme scheme, string calcDate)
        {
            var year = Convert.ToDateTime(calcDate).Year;
            var sourceQuery = String.Format("select k.{1} from {0} k where k.{2} = '‘ќ' and k.{3} = 6 and k.{4} = {5}", 
                ReportConsts.HUB_DATASOURCES, 
                HUB_Datasources.id, 
                HUB_Datasources.SUPPLIERCODE, 
                HUB_Datasources.DATACODE, 
                HUB_Datasources.YEAR, 
                year);
            var dbHelper = new ReportDBHelper(scheme);
            var dtSource = dbHelper.GetTableData(sourceQuery);
            
            if (dtSource.Rows.Count > 0)
            {
                return GetLastRow(dtSource)[0].ToString();
            }

            return ReportConsts.UndefinedKey;
        }

        private static string GetSubjectID(IScheme scheme, string calcDate)
        {
            if (calcDate.Length > 0)
            {
                string selectRegion = String.Format("{0} = 3 and {1} = {3} and {2} <> -1",
                        d_Regions_Analysis.RefTerr,
                        d_Regions_Analysis.SourceID,
                        d_Regions_Analysis.RefBridgeRegions,
                        GetSourceID(scheme, calcDate));
                var dbHelper = new ReportDBHelper(scheme);
                var dtData = dbHelper.GetEntityData(d_Regions_Analysis.internalKey, selectRegion);
                var dr = GetLastRow(dtData);
                
                if (dr != null)
                {
                    return GetLastRow(dtData)[d_Regions_Analysis.id].ToString();
                }
            }
            return ReportConsts.UndefinedKey;
        }

        private static string GetTerritoryID(IScheme scheme, ReportTerritoryType terrType)
        {
            var terrytoryCodes = "5,6,11";

            if (terrType == ReportTerritoryType.ttRegion)
            {
                terrytoryCodes = "4,7";
            }
            
            if (terrType == ReportTerritoryType.ttSubject)
            {
                terrytoryCodes = "3";
            }

            var result = String.Empty;
            var selectTerr = String.Format("{0} in ({1})", d_Regions_Plan.RefTerr, terrytoryCodes);
            var dbHelper = new ReportDBHelper(scheme);
            var dtData = dbHelper.GetEntityData(d_Regions_Plan.internalKey, selectTerr);

            result = dtData.Rows.Cast<DataRow>().Aggregate(result, (current, drSelect) =>
                String.Format("{0},{1}", current, drSelect[d_Regions_Plan.id]));

            return result.TrimStart(',');
        }

        private static string GetParentTerritoryData(IScheme scheme, int refRegion)
        {
            var result = "-1==";
            var moCodes = new Collection<int> { 4, 7 };
            var selectRegion = String.Format("{0} = {1}", d_Regions_Plan.id, refRegion);
            var dbHelper = new ReportDBHelper(scheme);
            var dtData = dbHelper.GetEntityData(d_Regions_Plan.internalKey, selectRegion);

            if (dtData.Rows.Count > 0)
            {
                var rowData = dtData.Rows[0];

                if (rowData[d_Regions_Plan.parentId] != DBNull.Value)
                {
                    var parentId = Convert.ToInt32(rowData[d_Regions_Plan.parentId]);
                    var territoryKind = Convert.ToInt32(rowData[d_Regions_Plan.RefTerr]);

                    while (parentId != -1 && !moCodes.Contains(territoryKind))
                    {
                        var selectParent = String.Format("{0} = {1}", d_Regions_Plan.id, parentId);
                        dtData = dbHelper.GetEntityData(d_Regions_Plan.internalKey, selectParent);

                        if (dtData.Rows.Count > 0 && dtData.Rows[0][d_Regions_Plan.parentId] != DBNull.Value)
                        {
                            var rowParent = dtData.Rows[0];
                            parentId = Convert.ToInt32(rowParent[d_Regions_Plan.parentId]);
                            territoryKind = Convert.ToInt32(rowParent[d_Regions_Plan.RefTerr]);
                        }
                        else
                        {
                            parentId = -1;
                        }
                    }
                    if (dtData.Rows.Count > 0)
                    {
                        var dr = dtData.Rows[0];
                        result = String.Format("{0}={1}={2}", dr[d_Regions_Plan.id], dr[d_Regions_Plan.Name], dr[d_Regions_Plan.Code]);
                    }
                }
            }

            return result;
        }

        public static string GetOKTMOCode(IScheme scheme)
        {
            var regionCode = String.Empty;

            if (scheme.GlobalConstsManager.Consts.ContainsKey(ReportConsts.OKTMO))
            {
                regionCode = Convert.ToString(scheme.GlobalConstsManager.Consts[ReportConsts.OKTMO].Value);
            }

            return regionCode;
        }

        private static void SetGarantFilter(Dictionary<string, string> mainFilter, string calcDate, string yearStart)
        {
            mainFilter.Add(f_S_Guarantissued.DateDoc, String.Format("<='{0}'", calcDate));
            mainFilter.Add(f_S_Guarantissued.EndDate, String.Format(">='{0}' or c.{1}>='{0}'",
                yearStart, f_S_Guarantissued.RenewalDate));
            mainFilter.Add(f_S_Guarantissued.RefVariant, ReportConsts.FixedVariantsID);
        }

        private static void SetGarantFilterDateDoc(Dictionary<string, string> mainFilter, string startDate, string endDate)
        {
            mainFilter.Add(f_S_Guarantissued.DateDoc, String.Format("<='{0}' and c.{2} >= '{1}'",
                endDate, startDate, f_S_Guarantissued.DateDoc));
            mainFilter.Add(f_S_Guarantissued.RefVariant, ReportConsts.FixedVariantsID);
        }

        private static void SetCreditFilter(Dictionary<string, string> mainFilter, string calcDate, string yearStart)
        {
            mainFilter.Add(f_S_Creditincome.StartDate, String.Format("<='{0}'", calcDate));
            mainFilter.Add(f_S_Creditincome.EndDate, String.Format(">='{0}' or c.{1}>='{0}'",
                yearStart, f_S_Creditincome.RenewalDate));
            mainFilter.Add(f_S_Creditincome.RefVariant, ReportConsts.FixedVariantsID);
        }

        private static void SetCapitalFilter(Dictionary<string, string> mainFilter, string calcDate, string yearStart)
        {
            mainFilter.Add(f_S_Capital.StartDate, String.Format("<='{0}'", calcDate));
            mainFilter.Add(f_S_Capital.DateDischarge, String.Format(">='{0}'", yearStart));
            mainFilter.Add(f_S_Capital.RefVariant, ReportConsts.FixedVariantsID);
        }

        private static string GetYearStart(string calcDate)
        {
            int year = Convert.ToDateTime(calcDate).Year;
            return GetYearStart(year);
        }

        public static string GetYearStart(int year)
        {
            return String.Format("01.01.{0}", year);
        }

        public static string GetYearEnd(string calcDate)
        {
            return GetYearEnd(Convert.ToDateTime(calcDate).Year);
        }

        public static string GetYearEnd(int year)
        {
            return String.Format("31.12.{0}", year);
        }

        private static string GetParamDate(Dictionary<string, string> reportParams, string paramName)
        {
            return Convert.ToDateTime(reportParams[paramName]).ToShortDateString();
        }

        private static bool GetBoolValue(object boolValue)
        {
            if (boolValue != DBNull.Value)
            {
                var valueStr = boolValue.ToString();
                if (valueStr == "1") valueStr = "True";
                if (valueStr == "0") valueStr = "False";
                return Convert.ToBoolean(valueStr);
            }

            return false;
        }

        private static decimal GetLastRowValue(DataTable dt, int columnIndex)
        {
            return GetNumber(dt.Rows[dt.Rows.Count - 1][columnIndex]);
        }

        public static DataRow GetLastRow(DataTable dt)
        {
            return dt.Rows.Count > 0 ? dt.Rows[dt.Rows.Count - 1] : null;
        }

        private static DataRow GetFirstRow(DataTable dt)
        {
            return dt.Rows.Count > 0 ? dt.Rows[0] : null;
        }

        private static int GetLastRowIndex(DataTable dt)
        {
            return dt.Rows.Count - 1;
        }

        /// <summary>
        /// —уммы из рублей в тыс€чи переводит
        /// </summary>
        private static void CorrectThousandSumValue(DataTable dt, int index)
        {
            foreach (DataRow row in dt.Rows)
            {
                if (dt.Columns.Count > index)
                {
                    if (row[index] != DBNull.Value) row[index] = Convert.ToDecimal(row[index]) / 1000;
                }
            }
        }

        /// <summary>
        /// —уммы из рублей в миллионы переводит
        /// </summary>
        private static void CorrectBillionSumValue(DataTable dt, int index)
        {
            foreach (DataRow row in dt.Rows)
            {
                if (row[index] != DBNull.Value) row[index] = Convert.ToDecimal(row[index]) / 1000000;
            }
        }

        /// <summary>
        /// —уммы из рублей в миллионы переводит
        /// </summary>
        private static decimal ConvertToBillion(decimal sum)
        {
            return sum / 1000000;
        }

        /// <summary>
        /// —уммы из рублей в тыс€чи переводит
        /// </summary>
        private static decimal ConvertTo1000(decimal sum)
        {
            return sum / 1000;
        }

        public static DataRow FindDataRow(DataTable dt, string pattern, string fieldName)
        {
            for (var i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i][fieldName].ToString() == pattern)
                    return dt.Rows[i];
            }

            return null;
        }

        /// <summary>
        /// код региона
        /// </summary>
        public int GetUserRegionCode(IScheme scheme)
        {
            var refRegion = GetUserRegion(scheme);
            
            if (refRegion != -1)
            {
                var regionQuery = String.Format("select {0} from d_Regions_Analysis where {1} = {2}",
                    d_Regions_Analysis.Code, d_Regions_Analysis.id, refRegion);
                var dbHelper = new ReportDBHelper(scheme);
                var codeValue = dbHelper.GetScalarData(regionQuery);
                
                if (codeValue != DBNull.Value)
                {
                    return Convert.ToInt32(codeValue);
                }
            }
            return -1;
        }

        /// <summary>
        /// регион текущего пользовател€
        /// </summary>
        public int GetUserRegion(IScheme scheme)
        {
            var userRegionQuery = String.Format("select refregion from users where id = {0}",
                scheme.UsersManager.GetCurrentUserID());
            var dbHelper = new ReportDBHelper(scheme);
            var queryResult = dbHelper.GetScalarData(userRegionQuery);

            if (queryResult != DBNull.Value)
            {
                return Convert.ToInt32(queryResult);
            }

            return -1;
        }

        /// <summary>
        /// получение данных из таблицы с константами
        /// </summary>
        public object GetConstDataByName(IScheme scheme, string constName)
        {
            if (String.IsNullOrEmpty(constName))
            {
                return null;
            }

            var constFilter = String.Format("IDConst = '{0}'", constName);
            var dbHelper = new ReportDBHelper(scheme);
            var constTable = dbHelper.GetEntityData(d_S_Constant.InternalKey, constFilter);

            return constTable.Rows.Count > 0 ? constTable.Rows[0][ReportConsts.KBK] : null;
        }

        private string GetConstCode(IScheme scheme, string constName)
        {
            var constValue = GetConstDataByName(scheme, constName);
            return constValue != null ? constValue.ToString() : String.Empty;
        }

        /// <summary>
        /// создание структуры дл€ таблицы-заголовка
        /// </summary>
        public DataTable CreateReportCaptionTable(int columnCount)
        {
            var dt = new DataTable("Report");

            for (var i = 0; i < columnCount; i++)
            {
                dt.Columns.Add(String.Format("CreditCaptionField{0}", i), typeof(String));
            }

            return dt;
        }

        /// <summary>
        /// создание структуры дл€ таблицы-заголовка
        /// </summary>
        private DataTable CreateReportCaptionTable(int columnCount, int rowCount)
        {
            var dt = CreateReportCaptionTable(columnCount);
            
            for (var i = 0; i < rowCount; i++)
            {
                dt.Rows.Add();
            }

            return dt;
        }

        private string GetBookValue(IScheme scheme, string book, string key, string fieldName)
        {
            var result = String.Empty;
            var dbHelper = new ReportDBHelper(scheme);
            var dtBook = dbHelper.GetEntityData(book, String.Format("ID = {0}", key));
            
            if (dtBook.Rows.Count > 0)
            {
                result = dtBook.Rows[0][fieldName].ToString();
            }

            return result;
        }

        private string GetBookValue(IScheme scheme, string book, string key)
        {
            return GetBookValue(scheme, book, key, "Name");
        }

        private static decimal FindExchangeRate(CommonDataObject cdo, int moneyCode)
        {
            return cdo.okvValues.ContainsKey(moneyCode) ? cdo.okvValues[moneyCode] : 0;
        }

        private static decimal GetUSDValue(CommonDataObject cdo)
        {
            return FindExchangeRate(cdo, ReportConsts.codeUSD);
        }

        private DataRow CreateReportParamsRow(DataTable[] dtTables)
        {
            dtTables[dtTables.Length - 1] = CreateReportCaptionTable(250);
            return dtTables[dtTables.Length - 1].Rows.Add();
        }

        // √руппировка по колонке с номером groupColumnIndex
        private static DataTable CommonGroupDataSet(DataTable dt, int groupColumnIndex, bool useSummaryRow)
        {
            var dtResult = dt.Clone();
            var summary = new decimal[dt.Columns.Count];
            var foundKeys = new Collection<string>();

            foreach (DataRow dr in dt.Rows)
            {
                var key = dr[groupColumnIndex].ToString();
                
                if (key.Length == 0 || foundKeys.Contains(key))
                {
                    continue;
                }

                var drsSelect = dt.Select(String.Format("{0} = '{1}'",
                                                              dt.Columns[groupColumnIndex].ColumnName, dr[groupColumnIndex]));
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    summary[j] = 0;
                }

                for (var i = 0; i < drsSelect.Length; i++)
                {
                    for (var j = 0; j < dt.Columns.Count; j++)
                    {
                        if (j != groupColumnIndex)
                            summary[j] += Convert.ToDecimal(drsSelect[i][j]);
                    }
                }

                var drData = dtResult.Rows.Add();
                
                for (var j = 0; j < dt.Columns.Count; j++)
                {
                    drData[j] = summary[j];
                }
                
                drData[groupColumnIndex] = key;
                foundKeys.Add(key);
            }

            if (useSummaryRow)
            {
                dtResult.ImportRow(GetLastRow(dt));
            }

            return dtResult;
        }

        // —ли€ние по колонке с номером groupColumnIndex
        private DataTable CommonMergeDataSet(DataTable dt1, DataTable dt2, int mergeColumnIndex,
            bool useSummaryRow)
        {
            var totalColumnCount = dt1.Columns.Count + dt2.Columns.Count - 1;
            var dtResult = CreateReportCaptionTable(totalColumnCount);
            var keys1 = new Collection<string>();
            var keys2 = new Collection<string>();

            // —ливаем первый со вторым
            foreach (DataRow dr in dt1.Rows)
            {
                var key = dr[mergeColumnIndex].ToString();
                if (key.Length > 0)
                {
                    var drData = dtResult.Rows.Add();

                    for (var j = 0; j < dt1.Columns.Count; j++)
                    {
                        drData[j] = dr[j];
                    }

                    var drsSelect = dt2.Select(String.Format("{0} = '{1}'",
                        dt2.Columns[mergeColumnIndex].ColumnName, key));
                    
                    if (drsSelect.Length > 0)
                    {
                        var realColumnIndex = dt1.Columns.Count;
                        
                        for (var j = 0; j < dt2.Columns.Count; j++)
                        {
                            if (j != mergeColumnIndex)
                            {
                                drData[realColumnIndex++] = drsSelect[0][j];
                            }
                        }
                        
                        keys2.Add(key);
                    }
                    
                    keys1.Add(key);
                }
            }

            // ƒобавл€ем записи из второго, которых нет в первом
            foreach (DataRow dr in dt2.Rows)
            {
                var key = dr[mergeColumnIndex].ToString();

                if (key.Length > 0 && !keys1.Contains(key))
                {
                    var drsSelect = dtResult.Select(String.Format("{0} = '{1}'",
                        dtResult.Columns[mergeColumnIndex].ColumnName, key));

                    DataRow drInsert;

                    if (drsSelect.Length > 0)
                    {
                        drInsert = drsSelect[0];
                    }
                    else
                    {
                        drInsert = dtResult.Rows.Add();
                        for (var kk = 0; kk < totalColumnCount; kk++)
                        {
                            drInsert[kk] = 0;
                        }
                        drInsert[mergeColumnIndex] = key;
                    }

                    var realColumnIndex = dt1.Columns.Count;

                    for (var j = 0; j < dt2.Columns.Count; j++)
                    {
                        if (j != mergeColumnIndex)
                        {
                            drInsert[realColumnIndex++] = dr[j];
                        }
                    }
                }
            }

            if (useSummaryRow)
            {
                var drResult = dtResult.Rows.Add();
                var drLast1 = GetLastRow(dt1);
                var drLast2 = GetLastRow(dt2);

                for (var j = 0; j < dt1.Columns.Count; j++)
                {
                    if (j != mergeColumnIndex)
                    {
                        drResult[j] = drLast1[j];
                    }
                }

                var realColumnIndex = dt1.Columns.Count;

                for (var j = 0; j < dt2.Columns.Count; j++)
                {
                    if (j != mergeColumnIndex) drResult[realColumnIndex++] = drLast2[j];
                }
            }

            return dtResult;
        }

        private static decimal GetNumber(object obj)
        {
            decimal result = 0;

            if (obj != null)
            {
                if (!decimal.TryParse(obj.ToString(), out result))
                {
                    result = 0;
                }
            }
            return result;
        }

        public static decimal GetDecimal(object obj)
        {
            decimal result = 0;
            
            if (obj != null)
            {
                if (!Decimal.TryParse(obj.ToString(), out result))
                {
                    result = 0;
                }
            }

            return result;
        }

        private static string GetMonthStart(int year, int month)
        {
            var monthStart = String.Format("01.{0}.{1}", month, year);
            return Convert.ToDateTime(monthStart).ToShortDateString();
        }

        private static string GetMonthStart(DateTime calcDate)
        {
            return GetMonthStart(calcDate.Year, calcDate.Month);
        }

        private static string GetMonthStart(string calcDate)
        {
            return GetMonthStart(Convert.ToDateTime(calcDate));
        }

        private static string GetMonthEnd(int year, int month)
        {
            var monthEnd = String.Format("{0}.{1}.{2}",
                DateTime.DaysInMonth(year, month), month, year);
            return Convert.ToDateTime(monthEnd).ToShortDateString();
        }

        private static string GetMonthEnd(DateTime calcDate)
        {
            return GetMonthEnd(calcDate.Year, calcDate.Month);
        }

        public static string[] GetMonthRusNames()
        {
            var ci = new CultureInfo("RU-ru");
            return ci.DateTimeFormat.MonthNames;
        }

        public static string Combine(object part1, object part2, string splitter)
        {
            return String.Format("{0}{1}{2}", part1, splitter, part2);
        }

        private static string Combine(object part1, object part2)
        {
            return Combine(part1, part2, ",");
        }

        public static string CombineAnd(params object[] parts)
        {
            var elements = parts.Select(Convert.ToString).Where(e => e != String.Empty);
            return String.Join(" and ", elements.ToArray());
        }

        public static string CombineOr(params object[] parts)
        {
            return String.Join(" or ", parts.Select(Convert.ToString).ToArray());
        }

        public static string Trim(object value, string splitter)
        {
            var strValue = Convert.ToString(value);
            var sptLen = splitter.Length;

            while (strValue.StartsWith(splitter))
            {
                strValue = strValue.Remove(0, sptLen);
            }

            while (strValue.EndsWith(splitter))
            {
                strValue = strValue.Remove(strValue.Length - sptLen, sptLen);
            }

            return strValue;
        }

        private static string CombineList(params string[] parts)
        {
            var result = parts.Aggregate(String.Empty, (current, t) => Combine(current, t, ";"));
            return result.Trim(';');
        }

        private static string FormFilterValue(string fieldName, string fieldValue)
        {
            return String.Format("{0}={1}", fieldName, fieldValue);
        }

        private static string FormNegativeFilterValue(string fieldName, string fieldValue)
        {
            return String.Format("{0}!{1}", fieldName, fieldValue);
        }

        private static string FormNegFilterValue(string fieldValue)
        {
            return String.Format("<>{0}", fieldValue);
        }

        public static string StrSortUp(string fieldName)
        {
            return String.Format("{0} asc", fieldName);
        }

        public static string StrSortDown(string fieldName)
        {
            return String.Format("{0} desc", fieldName);
        }

        private static string FormSortString(params object[] sortFields)
        {
            var result = sortFields.Aggregate(String.Empty, (current, t) => Combine(current, t.ToString()));
            return result.Trim(',');
        }

        private static string CreateValuePair(string fieldName)
        {
            return String.Format("{0};{0}", fieldName);
        }

        private static string SafeSubstrReplace(object value, string oldstr, string newstr)
        {
            return value != null ? value.ToString().Replace(oldstr, newstr) : String.Empty;
        }

        private static string GetDateText(string dateValue)
        {
            var reportDate = Convert.ToDateTime(dateValue);
            return String.Format("{0} {1} {2}", reportDate.Day, GetMonthText2(reportDate.Month), reportDate.Year);
        }

        private static IEnumerable<string> GetColumnValues(IEnumerable<DataRow> drs, string columnName)
        {
            var result = new Collection<string>();

            foreach (var drData in drs)
            {
                var itemValue = drData[columnName].ToString();
                
                if (itemValue.Length > 0 && !result.Contains(itemValue))
                {
                    result.Add(itemValue);
                }
            }
            return result;
        }

        private static IEnumerable<string> GetColumnValues(DataTable dt, string columnName)
        {
            return GetColumnValues(dt.Select(), columnName);
        }

        private static void AppendRows(DataTable dtSource, DataTable dtResult, int columnCount)
        {
            for (var j = 0; j < dtSource.Rows.Count; j++)
            {
                var rowInsert = dtResult.Rows.Add();

                for (var i = 0; i < columnCount; i++)
                {
                    rowInsert[i] = dtSource.Rows[j][i];
                }
            }
        }

        public static string GetUNVDate(DateTime date)
        {
            return String.Format("{0}{1}{2}",
                                 date.Year,
                                 date.Month.ToString().PadLeft(2, '0'),
                                 date.Day.ToString().PadLeft(2, '0'));
        }

        public static string GetUNVDate(string calcDate)
        {
            return GetUNVDate(Convert.ToDateTime(calcDate));
        }

        public static string GetUNVDate(int year, int month, int day)
        {
            return String.Format("{0}{1}{2}", year, month.ToString().PadLeft(2, '0'), day.ToString().PadLeft(2, '0'));
        }

        public static DateTime GetNormalDate(string unvDate)
        {
            return new DateTime(
                Convert.ToInt32(unvDate.Substring(0, 4)),
                Math.Max(1, Convert.ToInt32(unvDate.Substring(4, 2))),
                Math.Max(1, Convert.ToInt32(unvDate.Substring(6, 2))));
        }

        public static DateTime GetNormalDate(int unvDate)
        {
            return GetNormalDate(Convert.ToString(unvDate));
        }

        public static string GetUNVMonthStart(int year, int month)
        {
            return String.Format("{0}{1}00", year, month.ToString().PadLeft(2, '0'));
        }

        public static string GetUNVMonthEnd(int year, int month)
        {
            return String.Format("{0}{1}31", year, month.ToString().PadLeft(2, '0'));
        }

        public static string GetUNVAbsMonthEnd(int year, int month)
        {
            return String.Format("{0}{1}99", year, month.ToString().PadLeft(2, '0'));
        }

        public static string GetUNVYearStart(int year)
        {
            return String.Format("{0}0100", year);
        }

        public static string GetUNVYearLoBound(int year)
        {
            return String.Format("{0}0000", year);
        }

        public static string GetUNVYearPlanLoBound(object year)
        {
            return String.Format("{0}0001", year);
        }

        public static string GetUNVYearEnd(int year)
        {
            return String.Format("{0}1231", year);
        }

        public static string GetUNVAbsYearEnd(int year)
        {
            return String.Format("{0}1299", year);
        }

        public static string GetUNVYearQuarter(int year, int quarter)
        {
            return String.Format("{0}999{1}", year, quarter);
        }

        public static string GetDateDayMonth(object cellValue)
        {
            if (cellValue != DBNull.Value && cellValue != null)
            {
                var dateValue = Convert.ToDateTime(cellValue);
                var dayNum = Convert.ToString(dateValue.Day).PadLeft(2, '0');
                var monNum = Convert.ToString(dateValue.Month).PadLeft(2, '0');
                return String.Format("{0}.{1}", dayNum, monNum);
            }

            return String.Empty;
        }

        private DataRow GetVariantInfo(object idVariant)
        {
            var selectVariant = String.Format("{0} = {1}", d_Variant_Borrow.ID, idVariant);
            var dbHelper = new ReportDBHelper(scheme);
            var tblVariantData = dbHelper.GetEntityData(d_Variant_Borrow.internalKey, selectVariant);
            return tblVariantData.Rows.Count > 0 ? GetFirstRow(tblVariantData) : null;
        }

        private DataRow GetJobInfo(object idJob)
        {
            var selectTitle = String.Format("{0} = {1}", d_Readings_JobTitleSignature.id, idJob);
            var dbHelper = new ReportDBHelper(scheme);
            var tblJobData = dbHelper.GetEntityData(d_Readings_JobTitleSignature.internalKey, selectTitle);
            return tblJobData.Rows.Count > 0 ? tblJobData.Rows[0] : null;
        }

        private void FillSignatureData(DataRow rowCaption, int startIndex, Dictionary<string, string> paramList, params string[] signList)
        {
            const int fieldCount = 2;

            for (var i = 0; i < signList.Length; i++)
            {
                var jobKey = ReportConsts.UndefinedKey;

                if (paramList.ContainsKey(signList[i]))
                {
                    jobKey = paramList[signList[i]];
                }

                var rowJob = GetJobInfo(jobKey);
                var cellIndex = startIndex + i * fieldCount;
                rowCaption[cellIndex + 0] = String.Empty;
                rowCaption[cellIndex + 1] = String.Empty;
                
                if (rowJob != null)
                {
                    rowCaption[cellIndex + 0] = rowJob[d_Readings_JobTitleSignature.Name];
                    rowCaption[cellIndex + 1] = rowJob[d_Readings_JobTitleSignature.JobTitle];
                }
            }
        }

        private static DataTable ClearEmptyRows(DataTable tblData, string keyField, IEnumerable<int> columnsList)
        {
            var deleteKeys = new Collection<string>();

            foreach (DataRow rowData in tblData.Rows)
            {
                var hasData = columnsList.Aggregate(false, (current, columnIndex) => 
                    current || Math.Abs(GetNumber(rowData[columnIndex])) > 0);

                if (!hasData && rowData[keyField] != DBNull.Value)
                {
                    deleteKeys.Add(Convert.ToString(rowData[keyField]));
                }
            }

            for (var index = 0; index < deleteKeys.Count; index++)
            {
                var rowKey = deleteKeys[index];
                var deletedRow = FindDataRow(tblData, rowKey, keyField);

                if (deletedRow != null)
                {
                    tblData.Rows.Remove(deletedRow);
                }
            }

            return tblData;
        }

        private static DataTable ClearNegativeCells(DataTable tblData, IEnumerable<int> columnsList)
        {
            foreach (DataRow rowData in tblData.Rows)
            {
                foreach (var columnIndex in columnsList)
                {
                    if (GetNumber(rowData[columnIndex]) < 0)
                    {
                        rowData[columnIndex] = 0;
                    }
                }
            }

            return tblData;
        }

        private static DataTable CorrectDetailPeriodText(DataTable tblData, int sourceIndex, int destIndex)
        {
            foreach (DataRow rowCap in tblData.Rows)
            {
                if (rowCap[sourceIndex] == DBNull.Value)
                {
                    continue;
                }

                var planPayParts = Convert.ToString(rowCap[sourceIndex]).Split(',');

                if (planPayParts.Length > 1)
                {
                    rowCap[destIndex] = String.Format("{0}-{1}", planPayParts[0], planPayParts[planPayParts.Length - 1]);
                }
            }

            return tblData;
        }

        private int GetQuarterNum(string calcDate)
        {
            return ((Convert.ToDateTime(calcDate).Month - 1) / 3) + 1;
        }

        private int GetCurrencyCode(string code)
        {
            var dbHelper = new ReportDBHelper(ConvertorSchemeLink.scheme);
            var tblOkvBook = dbHelper.GetEntityData(d_OKV_Currency.internalKey);
            var rowsEuro = tblOkvBook.Select(String.Format("{0} = '{1}'", d_OKV_Currency.CodeLetter, code));

            if (rowsEuro.Length > 0)
            {
                return Convert.ToInt32(rowsEuro[0][d_OKV_Currency.ID]);
            }

            return -1;
        }

        private int GetEuroCode()
        {
            return GetCurrencyCode("EUR");
        }

        private int GetUsdCode()
        {
            return GetCurrencyCode("USD");
        }

        private decimal RoundValue(object sumValue, string precision)
        {
            var rndType = (PrecisionNumberEnum)Enum.Parse(typeof(PrecisionNumberEnum), precision);
            // делитель сумм
            var rndValue = 2;

            if (rndType == PrecisionNumberEnum.ctN0)
            {
                rndValue = 0;
            }

            if (rndType == PrecisionNumberEnum.ctN1)
            {
                rndValue = 1;
            }

            return Math.Round(GetDecimal(sumValue), rndValue);
        }

        public static decimal GetDividerValue(string divider)
        {
            // делитель сумм
            var divType = (SumDividerEnum)Enum.Parse(typeof(SumDividerEnum), divider);

            switch (divType)
            {
                case SumDividerEnum.i2:
                    return 1000;
                case SumDividerEnum.i3:
                    return 1000000;
                case SumDividerEnum.i4:
                    return 1000000000;
            }

            return 1;
        }

        private decimal DivideSumValue(object sumValue, string divider)
        {
            return GetDecimal(sumValue) / GetDividerValue(divider);
        }

        private void DivideColumn(DataTable tbl, int columnIndex, string divider)
        {
            foreach (DataRow row in tbl.Rows)
            {
                row[columnIndex] = DivideSumValue(row[columnIndex], divider);
            }
        }

        private void RoundColumn(DataTable tbl, int columnIndex, string precision)
        {
            foreach (DataRow row in tbl.Rows)
            {
                row[columnIndex] = RoundValue(row[columnIndex], precision);
            }
        }

        public static int GetEnumItemIndex(Enum enumObj, string enumValue)
        {
            var enumType = enumObj.GetType();
            var values = Enum.GetValues(enumType);
            int index = 0, resultIndex = 0;

            foreach (var value in values)
            {
                if (value.ToString() == enumValue)
                {
                    resultIndex = index;
                }

                index++;
            }

            return resultIndex;
        }

        public static string GetEnumItemDescription(Enum enumObj, string enumValue)
        {
            var enumType = enumObj.GetType();
            var resultValue = GetEnumItemValue(enumObj, enumValue);
            var enumField = enumType.GetField(Enum.GetName(enumType, resultValue));
            var dna = (DescriptionAttribute)Attribute.GetCustomAttribute(enumField, typeof(DescriptionAttribute));
            return dna != null ? dna.Description : String.Empty;
        }

        private string GetPrevDay(object dateValue)
        {
            DateTime date;
            return DateTime.TryParse(Convert.ToString(dateValue), out date)
                       ? date.AddDays(-1).ToShortDateString()
                       : String.Empty;
        }

        public int GetColumnIndex(DataTable tbl, string columnName)
        {
            return tbl.Columns.Contains(columnName) ? tbl.Columns[columnName].Ordinal : -1;
        }

        public string GetColumnName(DataTable tbl, int columnIndex)
        {
            return tbl.Columns[columnIndex].ColumnName;
        }

        public string CombineFieldText(DataRow row, params string[] fields)
        {
            var strBuilder = new StringBuilder();

            foreach (var field in fields)
            {
                strBuilder.Append(row[field]);
                strBuilder.Append(" ");
            }

            return strBuilder.ToString().Trim();
        }

        public static object GetEnumItemValue(Enum enumObj, string enumValue)
        {
            var enumType = enumObj.GetType();
            var values = Enum.GetValues(enumType);
            return values.Cast<object>().FirstOrDefault(value => value.ToString() == enumValue);
        }

        public void SumColumns(DataTable tblData, int columnIndex1, int columnIndex2, int resultIndex)
        {
            if (tblData == null)
            {
                return;
            }

            foreach (DataRow row in tblData.Rows)
            {
                row[resultIndex] = GetDecimal(row[columnIndex1]) + GetDecimal(row[columnIndex2]);
            }
        }

        public static void AddValueToCell(DataTable tbl, int rowIndex, int colIndex, object value)
        {
            tbl.Rows[rowIndex][colIndex] = GetDecimal(tbl.Rows[rowIndex][colIndex]) + GetDecimal(value);
        }
    }
}