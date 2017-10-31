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

namespace Krista.FM.Server.DataPumps.MOFO15Pump
{
    // МОФО_0015_ПЛАН ДОХОДОВ_РОСПИСЬ
    public class MOFO15PumpModule : CorrectedPumpModuleBase
    {

        #region Поля

        #region Классификаторы

        // КД.Анализ (d_KD_Analysis)
        private IDbDataAdapter daKd;
        private DataSet dsKd;
        private IClassifier clsKd;
        private Dictionary<string, int> kdCache = null;
        // Районы.Анализ (d_Regions_Analysis)
        private IDbDataAdapter daRegions;
        private DataSet dsRegions;
        private IClassifier clsRegions;
        private Dictionary<string, int> regionCache = null;
        // Вариант.МОФО_План (d.Variant.MOFOPlan)
        private IDbDataAdapter daVariant;
        private DataSet dsVariant;
        private IClassifier clsVariant;
        private Dictionary<string, int> variantCache = null;

        #endregion Классификаторы

        #region Факты

        // Доходы.МОФО_План доходов_Роспись (f_D_MOFOPlan)
        private IDbDataAdapter daMOFO15;
        private DataSet dsMOFO15;
        private IFactTable fctMOFO15;

        #endregion Факты

        private ExcelHelper excelHelper;
        private object excelObj = null;
        int analSourceId = -1;
        private List<string> deletedDateList = null;

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
            InitDataSet(ref daVariant, ref dsVariant, clsVariant, false, string.Empty, string.Empty);
            InitFactDataSet(ref daMOFO15, ref dsMOFO15, fctMOFO15);
            FillCaches();
        }

        private void FillCaches()
        {
            FillRowsCache(ref regionCache, dsRegions.Tables[0], "Code", "Id");
            FillRowsCache(ref kdCache, dsKd.Tables[0], "CodeStr", "Id");
            FillRowsCache(ref variantCache, dsVariant.Tables[0], "Code", "Id");
        }

        protected override void UpdateData()
        {
            UpdateDataSet(daRegions, dsRegions, clsRegions);
            UpdateDataSet(daVariant, dsVariant, clsVariant);
            UpdateDataSet(daKd, dsKd, clsKd);
            UpdateDataSet(daMOFO15, dsMOFO15, fctMOFO15);
        }

        private const string D_REGIONS_GUID = "383f887a-3ebb-4dba-8abb-560b5777436f";
        private const string D_KD_GUID = "2553274b-4cee-4d20-a9a6-eef173465d8b";
        private const string D_VARIANT_GUID = "298b134e-9d3a-463a-ad56-7248c76f47af";
        private const string F_D_MOFO15_GUID = "391a01eb-630d-4094-a67d-30c867a14bf5";
        protected override void InitDBObjects()
        {
            this.UsedClassifiers = new IClassifier[] { };
            clsVariant = this.Scheme.Classifiers[D_VARIANT_GUID];
            clsRegions = this.Scheme.Classifiers[D_REGIONS_GUID];
            clsKd = this.Scheme.Classifiers[D_KD_GUID];
            this.UsedFacts = new IFactTable[] { fctMOFO15 = this.Scheme.FactTables[F_D_MOFO15_GUID] };
        }

        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsMOFO15);
            ClearDataSet(ref dsRegions);
            ClearDataSet(ref dsVariant);
            ClearDataSet(ref dsKd);
        }

        #endregion Работа с базой и кэшами

        #region работа с Excel

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

        private void PumpXlsRow(DataRow row, int refVariant)
        {
            int budgetLevel = GetBudgetLevel(Convert.ToInt32(row["TerrType"]));
            int refKd = PumpKd(row["Kd"].ToString().Trim().PadLeft(1, '0'));
            int refRegion = PumpRegion(row["Region"].ToString().Trim().PadLeft(1, '0'));
            for (int i = 1; i <= 4; i++)
            {
                string sumFieldName = string.Format("Q{0}", i);
                decimal sum = Convert.ToDecimal(row[sumFieldName].ToString().PadLeft(1, '0'));
                if (sum == 0)
                    continue;
                string refDate = string.Format("{0}999{1}", this.DataSource.Year, i);
                object[] mapping = new object[] { "RefKD", refKd, "RefRegions", refRegion, "RefVariant", refVariant, 
                    "RefBdgtLvls", budgetLevel, "RefYearDayUNV", refDate, "ForPeriod", sum };
                PumpRow(dsMOFO15.Tables[0], mapping);
                if (dsMOFO15.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
                {
                    UpdateData();
                    ClearDataSet(daMOFO15, ref dsMOFO15);
                }
            }
        }

        private int GetLastRow(object sheet)
        {
            for (int i = 1; ; i++)
                if (excelHelper.GetCell(sheet, i, 1).Value.Trim() == string.Empty)
                    return i - 1;
        }

        private int PumpVariant(int code)
        {
            int refVariant = PumpCachedRow(variantCache, dsVariant.Tables[0], clsVariant, code.ToString(),
                new object[] { "Code", code, "Name", CommonRoutines.MonthByNumber[code - 1],
                "VariantCompleted", 0, "VariantComment", " " });
            string key = string.Format("{0}|{1}", this.DataSource.Year, code);
            if (!deletedDateList.Contains(key))
            {
                DeleteData(string.Format("RefVariant = {0}", refVariant), string.Format("Вариант: {0}", code));
                deletedDateList.Add(key);
            }
            return refVariant;
        }

        private object[] XLS_MAPPING = new object[] { "Region", 1, "TerrType", 2, "Kd", 3, "Q1", 4, "Q2", 5, "Q3", 6, "Q4", 7 };
        private void PumpXLSSheetData(FileInfo file, object sheet)
        {
            int refVariant = PumpVariant(Convert.ToInt32(file.Directory.Name));
            int lastRow = GetLastRow(sheet);
            DataTable dt = excelHelper.GetSheetDataTable(sheet, 2, lastRow, XLS_MAPPING);
            for (int i = 0; i < dt.Rows.Count; i++)
                try
                {
                    SetProgress(lastRow, i, string.Format("Обработка файла {0}...", file.FullName),
                        string.Format("Строка {0} из {1}", i, lastRow));
                    PumpXlsRow(dt.Rows[i], refVariant);
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("При обработке строки {0} возникла ошибка ({1})", i, ex.Message), ex);
                }
        }

        private void PumpXLSFile(FileInfo file)
        {
            object workbook = excelHelper.InitWorkBook(ref excelObj, file.FullName);
            try
            {
                object sheet = excelHelper.GetSheet(workbook, 1);
                PumpXLSSheetData(file, sheet);
            }
            finally
            {
                excelHelper.CloseExcel(ref excelObj);
            }
        }

        #endregion работа с Excel

        #region Перекрытые методы закачки

        protected override void ProcessFiles(DirectoryInfo dir)
        {
            deletedDateList = new List<string>();
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

    }
}
