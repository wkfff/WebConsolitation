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

namespace Krista.FM.Server.DataPumps.MOFO18Pump
{
    // МОФО_0018_зеленая форма
    public class MOFO18PumpModule : CorrectedPumpModuleBase
    {

        #region Поля

        #region Классификаторы

        // Показатели.МОФО_Зеленая форма (d_Marks_GreenForm)
        private IDbDataAdapter daMarks;
        private DataSet dsMarks;
        private IClassifier clsMarks;
        private Dictionary<string, int> marksCache = null;
        // Районы.МОФО (d_Regions_MOFO)
        private IDbDataAdapter daRegions;
        private DataSet dsRegions;
        private IClassifier clsRegions;
        private Dictionary<string, int> regionCache = null;

        #endregion Классификаторы

        #region Факты

        // Показатели.МОФО_Зеленая форма (f_Marks_GreenForm)
        private IDbDataAdapter daMOFO18;
        private DataSet dsMOFO18;
        private IFactTable fctMOFO18;

        #endregion Факты

        private ExcelHelper excelHelper;
        private object excelObj = null;
        private int refDate = -1;
        private bool isOutcomes = false;

        #endregion Поля

        #region Закачка данных

        #region Работа с базой и кэшами

        protected override void QueryData()
        {
            InitClsDataSet(ref daRegions, ref dsRegions, clsRegions);
            InitClsDataSet(ref daMarks, ref dsMarks, clsMarks);
            InitFactDataSet(ref daMOFO18, ref dsMOFO18, fctMOFO18);
            FillCaches();
        }

        private void FillCaches()
        {
            FillRowsCache(ref regionCache, dsRegions.Tables[0], "Code", "Id");
            FillRowsCache(ref marksCache, dsMarks.Tables[0], new string[] { "CodeStr", "Name" }, "|", "Id");
        }

        protected override void UpdateData()
        {
            UpdateDataSet(daRegions, dsRegions, clsRegions);
            UpdateDataSet(daMarks, dsMarks, clsMarks);
            UpdateDataSet(daMOFO18, dsMOFO18, fctMOFO18);
        }

        private const string D_REGIONS_GUID = "c57bc57c-45fd-4b4f-8a6a-7b6c9c660f2e";
        private const string D_MARKS_GUID = "cab9c7b3-2fda-4beb-bc92-683d050fe5dd";
        private const string F_D_MOFO18_GUID = "77e8d172-18a4-404a-976a-08914ed1f109";
        protected override void InitDBObjects()
        {
            this.UsedClassifiers = new IClassifier[] { 
                clsRegions = this.Scheme.Classifiers[D_REGIONS_GUID],
                clsMarks = this.Scheme.Classifiers[D_MARKS_GUID] };
            this.UsedFacts = new IFactTable[] { fctMOFO18 = this.Scheme.FactTables[F_D_MOFO18_GUID] };
        }

        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsMOFO18);
            ClearDataSet(ref dsRegions);
            ClearDataSet(ref dsMarks);
        }

        #endregion Работа с базой и кэшами

        #region работа с Excel

        private int PumpMarks(string code, string name)
        {
            if ((name == string.Empty) && (code == string.Empty))
                return -1;
            if (name == string.Empty)
                name = constDefaultClsName;
            string key = string.Format("{0}|{1}", code, name);
            object fkr = DBNull.Value;
            if (isOutcomes)
                if (code.Length >= 7)
                    fkr = code.Substring(3, 4);
            return PumpCachedRow(marksCache, dsMarks.Tables[0], clsMarks, key,
                new object[] { "CodeStr", code, "Name", name, "FKR", fkr });
        }

        private void PumpXlsRow(DataRow row, int refRegion)
        {
            int refMarks = PumpMarks(row["Code"].ToString().Trim(), row["Name"].ToString().Trim());
            if (refMarks == -1)
                return;
            // сохраняем для порядка записей.. (аццкая хуита)
            UpdateDataSet(daMarks, dsMarks, clsMarks);
            decimal plan = Convert.ToDecimal(row["Plan"].ToString().PadLeft(1, '0')) * 1000;
            decimal fact = Convert.ToDecimal(row["Fact"].ToString().PadLeft(1, '0')) * 1000;
            if ((plan == 0) && (fact == 0))
                return;
            object[] mapping = new object[] { "RefYearDayUNV", refDate, "RefMarks", refMarks, 
                "RefRegions", refRegion, "YearPlan", plan, "Fact", fact };
            PumpRow(dsMOFO18.Tables[0], mapping);
            if (dsMOFO18.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
            {
                UpdateData();
                ClearDataSet(daMOFO18, ref dsMOFO18);
            }
        }

        private int GetLastRow(object sheet)
        {
            for (int i = 10; ; i++)
                if ((excelHelper.GetCell(sheet, i, 1).Value.Trim() == string.Empty) &&
                    (excelHelper.GetCell(sheet, i, 2).Value.Trim() == string.Empty) && 
                    (excelHelper.GetCell(sheet, i, 3).Value.Trim() == string.Empty))
                    return i - 1;
        }

        private int PumpRegion(string code)
        {
            return PumpCachedRow(regionCache, dsRegions.Tables[0], clsRegions, code,
                new object[] { "Code", code });
        }

        private object[] XLS_MAPPING = new object[] { "Index", 1, "Code", 2, "Name", 3, "Plan", 4, "Fact", 5 };
        private void PumpXLSSheetData(FileInfo file, object sheet)
        {
            isOutcomes = false;
            int refRegion = PumpRegion(file.Name.Split('.')[0]);
            int lastRow = GetLastRow(sheet);
            DataTable dt = excelHelper.GetSheetDataTable(sheet, 10, lastRow, XLS_MAPPING);
            for (int i = 0; i < dt.Rows.Count; i++)
                try
                {
                    SetProgress(lastRow, i, string.Format("Обработка файла {0}...", file.FullName),
                        string.Format("Строка {0} из {1}", i, lastRow));
                    string index = dt.Rows[i]["Index"].ToString();
                    if (index.Contains("II."))
                        isOutcomes = true;
                    if ((index == string.Empty) || (index.Contains("I.")))
                        continue;
                    PumpXlsRow(dt.Rows[i], refRegion);
                }
                catch (Exception ex)
                {
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeError,
                        string.Format("Ошибка при обработке строки {0} отчета {1}: {2}", i + 10, file.Name, ex.Message));
                    this.DataSourceProcessingResult = DataSourceProcessingResult.ProcessedWithErrors;
                    return;
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
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStart, "Старт инициализации Excel.");
            excelHelper = new ExcelHelper();
            // дату берем из параметров источника
            refDate = this.DataSource.Year * 10000 + this.DataSource.Month * 100;
            try
            {
                ProcessFilesTemplate(dir, "*.xls", new ProcessFileDelegate(PumpXLSFile), false);
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
            PumpDataYMTemplate();
        }

        #endregion Перекрытые методы закачки

        #endregion Закачка данных

    }
}
