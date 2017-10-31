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

namespace Krista.FM.Server.DataPumps.FO24Pump
{
    // ФО_0024_ДОЛГОВАЯ КНИГА
    public class FO24PumpModule : CorrectedPumpModuleBase
    {

        #region Поля

        #region Классификаторы

        // ГосВнутрДолг.Договора (d.GID.Treaties)
        private IDbDataAdapter daTreaties;
        private DataSet dsTreaties;
        private IClassifier clsTreaties;
        private Dictionary<string, int> cacheTreaties = null;
        // Вариант.ФО_Долговые обязательства (d.Variant.FODebenture)
        private IDbDataAdapter daVariant;
        private DataSet dsVariant;
        private IClassifier clsVariant;
        private Dictionary<string, int> cacheVariant = null;
        // КИФ.Планирование (d.KIF.Plan)
        private IDbDataAdapter daKIF;
        private DataSet dsKIF;
        private IClassifier clsKIF;
        private Dictionary<string, int> cacheKIF = null;

        #endregion Классификаторы

        #region Факты

        // ГосВнутрДолг.ФО_Долговые обязательства (f.GID.FODebenture)
        private IDbDataAdapter daFactFO24;
        private DataSet dsFactFO24;
        private IFactTable fctFactFO24;

        #endregion Факты

        private ExcelHelper excelHelper = null;
        private object excelObj = null;
        private List<string> deletedVariantList = null;
        private int multiplier = 1;

        #endregion Поля

        #region Закачка данных

        #region Работа с базой и кэшами

        protected override void QueryData()
        {
            InitClsDataSet(ref daTreaties, ref dsTreaties, clsTreaties, false, string.Empty);
            InitDataSet(ref daVariant, ref dsVariant, clsVariant, string.Empty);
            InitClsDataSet(ref daKIF, ref dsKIF, clsKIF, false, string.Empty);
            InitFactDataSet(ref daFactFO24, ref dsFactFO24, fctFactFO24);
            FillCaches();
        }

        private void FillCaches()
        {
            FillRowsCache(ref cacheTreaties, dsTreaties.Tables[0], "CodeDoc");
            FillRowsCache(ref cacheVariant, dsVariant.Tables[0], "Name");
            FillRowsCache(ref cacheKIF, dsKIF.Tables[0], new string[] { "Name", "ParentId" }, "|", "Id");
        }

        protected override void UpdateData()
        {
            UpdateDataSet(daTreaties, dsTreaties, clsTreaties);
            UpdateDataSet(daVariant, dsVariant, clsVariant);
            UpdateDataSet(daKIF, dsKIF, clsKIF);
            UpdateDataSet(daFactFO24, dsFactFO24, fctFactFO24);
        }

        private const string D_GID_TREATIES_GUID = "37d5dbcb-7589-4db7-bc85-01bbe0f7b629";
        private const string D_VARIANT_FO_DEBENTURE_GUID = "90c3eceb-902e-468b-8104-aeffa5f7df73";
        private const string D_KIF_PLAN_GUID = "a531f087-b785-4ab6-8934-6f7f29ea4660";
        private const string F_GID_FO_DEBENTURE_GUID = "eeaa02b6-6ea1-412d-8d20-7bb11c6d0487";
        protected override void InitDBObjects()
        {
            this.UsedClassifiers = new IClassifier[] {
                clsTreaties = this.Scheme.Classifiers[D_GID_TREATIES_GUID],
                clsVariant = this.Scheme.Classifiers[D_VARIANT_FO_DEBENTURE_GUID],
                clsKIF = this.Scheme.Classifiers[D_KIF_PLAN_GUID] };
            this.UsedFacts = new IFactTable[] { fctFactFO24 = this.Scheme.FactTables[F_GID_FO_DEBENTURE_GUID] };
        }

        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsFactFO24);
            ClearDataSet(ref dsTreaties);
            ClearDataSet(ref dsVariant);
            ClearDataSet(ref dsKIF);
        }

        #endregion Работа с базой и кэшами

        #region Работа с экселем

        private string NULL_COMMENT = "Значение не указано";
        private int GetRefVariant(string variantName)
        {
            if (!deletedVariantList.Contains(variantName.ToUpper()))
            {
                if (cacheVariant.ContainsKey(variantName))
                {
                    DeleteData(string.Format("RefVariantFODeb = '{0}'", cacheVariant[variantName]), string.Format("Вариант отчета: {0}.", variantName));
                    deletedVariantList.Add(variantName.ToUpper());
                }
            }
            return PumpCachedRow(cacheVariant, dsVariant.Tables[0], clsVariant, variantName,
                new object[] { "Name", variantName, "VariantComment", NULL_COMMENT, "VariantCompleted", 0 });
        }

        private const string THOUSAND_MARK = "ТЫС";
        private const string MILLION_MARK = "МЛН";
        private bool CheckCurrency(ref string currency, string cellValue)
        {
            if (cellValue != string.Empty)
                currency = cellValue;
            if (currency.ToUpper().Contains(THOUSAND_MARK))
                multiplier = 1000;
            else if (currency.ToUpper().Contains(MILLION_MARK))
                multiplier = 1000000;
            return currency.Contains("RUR");
        }

        private int GetRefTreaty(string treatyCode)
        {
            return PumpCachedRow(cacheTreaties, dsTreaties.Tables[0], clsTreaties, treatyCode, new object[] { "CodeDoc", treatyCode });
        }

        private int GetKifParentID(string kifName)
        {
            foreach (KeyValuePair<string, int> item in cacheKIF)
                if (item.Key.ToString().Split('|')[0] == kifName)
                    return item.Value;
            return 0;
        }

        private int GetRefKIF(string kifName, int parentID, string kifParentName)
        {
            if (kifParentName != string.Empty)
                kifName = string.Format("{0} {1}", kifParentName, kifName);
            object[] mapping = new object[] { "CodeStr", 0, "Name", kifName };
            if (parentID > 0)
                mapping = (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "ParentID", parentID });
            else
            {
                parentID = GetKifParentID(kifName);
                if (parentID > 0)
                    return parentID;
            }
            string kifKey = string.Format("{0}|{1}", kifName, parentID);
            return PumpCachedRow(cacheKIF, dsKIF.Tables[0], clsKIF, kifKey, mapping);
        }

        private const string REPORT_END_MARK = "ИТОГО";
        private bool IsReportEnd(string value)
        {
            return value.ToUpper().Trim().StartsWith(REPORT_END_MARK);
        }

        private const string TOTAL_ROW = "ИТОГО";
        private bool IsTotalRow(string value)
        {
            return value.ToUpper().StartsWith(TOTAL_ROW);
        }

        private int GetRefYear(string cellValue)
        {
            return Convert.ToInt32(cellValue.Split('-')[0].Trim()) * 10000;
        }

        private const int START_COLUMN = 5;
        private const int YEAR_ROW = 5;
        private void PumpXlsRow(object sheet, int row, object[] clsMapping, int kifParentID, string kifParentName)
        {
            string cellValue = excelHelper.GetCell(sheet, row, 2).Value;
            if (IsTotalRow(cellValue))
                return;
            int refKIF = GetRefKIF(cellValue, kifParentID, kifParentName);
            clsMapping = (object[])CommonRoutines.ConcatArrays(clsMapping, new object[] { "RefKIFPlan", refKIF });
            int column = START_COLUMN;
            for (; column <= 14; column++)
            {
                int refYear = GetRefYear(excelHelper.GetCell(sheet, YEAR_ROW, column).Value);
                string sum = excelHelper.GetCell(sheet, row, column).Value.Trim();
                if ((sum == string.Empty) || (sum == "-") || (sum == "0"))
                    continue;
                double d = Convert.ToDouble(sum) * multiplier;
                object[] mapping = (object[])CommonRoutines.ConcatArrays(clsMapping,
                    new object[] { "RefYearDayUNV", refYear, "Liability", d });
                PumpRow(dsFactFO24.Tables[0], mapping);
            }
            if (dsFactFO24.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
            {
                UpdateData();
                ClearDataSet(daFactFO24, ref dsFactFO24);
            }
        }

        private const int START_ROW = 6;
        private void PumpXlsSheet(object sheet, string fileName)
        {
            string currency = string.Empty;
            int refTreaty = -1;
            int kifParentID = -1;
            string kifParentName = string.Empty;
            int refVariant = GetRefVariant(excelHelper.GetCell(sheet, 2, 1).Value.Trim());
            int curRow = START_ROW;
            for (; ; curRow++)
                try
                {
                    string cellValue = excelHelper.GetCell(sheet, curRow, 1).Value.Trim();
                    if (IsReportEnd(cellValue))
                        return;
                    if (cellValue != string.Empty)
                    {
                        refTreaty = GetRefTreaty(cellValue);
                        kifParentName = cellValue.Substring(11);
                        kifParentID = GetRefKIF(kifParentName, 0, string.Empty);
                    }
                    if (!CheckCurrency(ref currency, excelHelper.GetCell(sheet, curRow, 3).Value.Trim()))
                        continue;
                    object[] clsMapping = new object[] { "RefVariantFODeb", refVariant, "RefTreaties", refTreaty };
                    PumpXlsRow(sheet, curRow, clsMapping, kifParentID, kifParentName);
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("При обработке строки {0} отчета {1} возникла ошибка ({2})",
                        curRow, fileName, ex.Message), ex);
                }
        }

        private void PumpXLSFile(FileInfo file)
        {
            object workbook = excelHelper.InitWorkBook(ref excelObj, file.FullName);
            try
            {
                object sheet = excelHelper.GetSheet(workbook, 1);
                PumpXlsSheet(sheet, file.FullName);
            }
            finally
            {
                excelHelper.CloseExcel(ref excelObj);
            }
        }

        #endregion Работа с экселем

        #region Перекрытые методы закачки

        protected override void ProcessFiles(DirectoryInfo dir)
        {
            deletedVariantList = new List<string>();
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
                deletedVariantList.Clear();
            }
        }

        protected override void DirectPumpData()
        {
            // иерархия устанавливается в закачке
            toSetHierarchy = false;
            PumpDataYTemplate();
        }

        #endregion Перекрытые методы закачки

        #endregion Закачка данных

    }
}
