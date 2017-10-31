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

namespace Krista.FM.Server.DataPumps.MFRF3Pump
{

    // МФРМ 3 - долговые обязательства
    public class MFRF3PumpModule : CorrectedPumpModuleBase
    {

        #region Поля

        #region Классификаторы

        // МФ РФ.Долговые обязательства (d_MFRF_Charge)
        private IDbDataAdapter daCharge;
        private DataSet dsCharge;
        private IClassifier clsCharge;
        private Dictionary<string, int> cacheCharge = null;
        // Территории.МФРФ (d_Territory_MFRF)
        private IDbDataAdapter daTerr;
        private DataSet dsTerr;
        private IClassifier clsTerr;
        private Dictionary<string, int> cacheTerr = null;

        #endregion Классификаторы

        #region Факты

        // МФ РФ.Долговые обязательства (f_MFRF_Charge)
        private IDbDataAdapter daMFRF3;
        private DataSet dsMFRF3;
        private IFactTable fctMFRF3;

        #endregion Факты

        private ExcelHelper excelHelper = null;
        private object excelObj = null;
        private decimal sumMultiplier = 1;
        private int sourceDate = 1;

        #endregion Поля

        #region Закачка данных

        #region Работа с базой и кэшами

        protected override void QueryData()
        {
            InitClsDataSet(ref daCharge, ref dsCharge, clsCharge);
            InitClsDataSet(ref daTerr, ref dsTerr, clsTerr);
            InitFactDataSet(ref daMFRF3, ref dsMFRF3, fctMFRF3);
            FillCaches();
        }

        private void FillCaches()
        {
            FillRowsCache(ref cacheTerr, dsTerr.Tables[0], new string[] { "Code", "Name" }, "|", "Id");
            FillRowsCache(ref cacheCharge, dsCharge.Tables[0], "Name");
        }

        protected override void UpdateData()
        {
            UpdateDataSet(daCharge, dsCharge, clsCharge);
            UpdateDataSet(daTerr, dsTerr, clsTerr);
            UpdateDataSet(daMFRF3, dsMFRF3, fctMFRF3);
        }

        private const string D_CHARGE_GUID = "bf9640dc-c226-45ae-9e95-3d2f75f44db3";
        private const string D_TERR_GUID = "2c6b9217-60ca-4fac-8b87-4e0ff3f9bba3";
        private const string F_MFRF3_GUID = "e78d3a59-a632-45a7-8f8d-82f5213fc326";
        protected override void InitDBObjects()
        {
            this.UsedClassifiers = new IClassifier[] { 
                clsCharge = this.Scheme.Classifiers[D_CHARGE_GUID], 
                clsTerr = this.Scheme.Classifiers[D_TERR_GUID] };
            this.UsedFacts = new IFactTable[] { fctMFRF3 = this.Scheme.FactTables[F_MFRF3_GUID] };
        }

        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsMFRF3);
            ClearDataSet(ref dsCharge);
            ClearDataSet(ref dsTerr);
        }

        #endregion Работа с базой и кэшами

        #region Работа с экселем

        private void PumpXlsRow(decimal sum, int refTerr, int refCharge)
        {
            object[] mapping = new object[] { "SizeCharge", sum, "RefYearDayUNV", sourceDate, 
                "RefCharge", refCharge, "RefTerritory", refTerr };
            PumpRow(dsMFRF3.Tables[0], mapping);
            if (dsMFRF3.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
            {
                UpdateData();
                ClearDataSet(daMFRF3, ref dsMFRF3);
            }
        }

        private int PumpTerr(string code, string name)
        {
            code = code.TrimStart('0').PadLeft(1, '0');
            string key = string.Format("{0}|{1}", code, name);
            return PumpCachedRow(cacheTerr, dsTerr.Tables[0], clsTerr,
                key, new object[] { "Code", code, "Name", name, "Okato", code });
        }

        private int PumpCharge(string name)
        {
            return PumpCachedRow(cacheCharge, dsCharge.Tables[0], clsCharge,
                name, new object[] { "Name", name });
        }

        private int GetFirstRow(object sheet)
        {
            if (sourceDate >= 20090700)
                return 10;
            for (int curRow = 1; ; curRow++)
                if (excelHelper.GetCell(sheet, curRow, 1).Value.Trim().ToUpper() == "КОД")
                    return (curRow + 1);
        }

        private bool IsLastRow(object sheet, int curRow)
        {
            if (sourceDate >= 20090700)
                return excelHelper.GetCell(sheet, curRow, 2).Value.Trim().ToUpper().StartsWith("ИТОГО");
            return excelHelper.GetCell(sheet, curRow, 1).Value.Trim().ToUpper().StartsWith("ВСЕГО");
        }

        private bool IsNeedlessRow(object sheet, int curRow)
        {
            if (sourceDate >= 20090700)
                return excelHelper.GetCell(sheet, curRow, 2).Value.Trim().ToUpper().Contains("ФЕДЕРАЛЬНЫЙ ОКРУГ");
            return false;
        }

        private int GetLastCoumn()
        {
            if (sourceDate >= 20090700)
                return 4;
            if (sourceDate >= 20080600)
                return 7;
            return 8;
        }

        private void PumpXlsSheet(object sheet, string fileName)
        {
            int firstRow = GetFirstRow(sheet);
            int lastColumn = GetLastCoumn();
            for (int curRow = firstRow; ; curRow++)
                try
                {
                    if (IsLastRow(sheet, curRow))
                        return;

                    string cellValue = excelHelper.GetCell(sheet, curRow, 1).Value.Trim();
                    if ((cellValue == string.Empty) || IsNeedlessRow(sheet, curRow))
                        continue;

                    int refTerr = PumpTerr(cellValue, excelHelper.GetCell(sheet, curRow, 2).Value.Trim());
                    for (int curColumn = 3; curColumn <= lastColumn; curColumn++)
                    {
                        string columnName = excelHelper.GetCell(sheet, firstRow - 1, curColumn).Value.Trim();
                        int refCharge = PumpCharge(columnName);
                        string sumStr = CommonRoutines.TrimLetters(excelHelper.GetCell(sheet, curRow, curColumn).Value.Trim()).
                            TrimStart('0').PadLeft(1, '0');
                        decimal sum = Convert.ToDecimal(sumStr.Replace('.', ',')) * sumMultiplier;
                        if (sum == 0)
                            continue;
                        PumpXlsRow(sum, refTerr, refCharge);
                    }
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
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStart, "Старт инициализации Excel.");
            excelHelper = new ExcelHelper();
            sourceDate = this.DataSource.Year * 10000 + this.DataSource.Month * 100;
            sumMultiplier = 1;
            if (sourceDate >= 20090400)
                sumMultiplier = 1000;
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
