using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.MFRF5Pump
{

    // МФРМ 5 - Сумма долга по ценным бумагам
    public class MFRF5PumpModule : CorrectedPumpModuleBase
    {

        #region Константы

        private const int TOTAL_ROWS = 1;
        private const string SUBJECTS = "СУБЪЕКТЫ";
        private const string TOTAL = "ИТОГО";
        private const string TOTALSUM = "ВСЕГО";

        #endregion  
      
        #region Поля

        

        #region Классификаторы

        // Территории.МФРФ (d.Territory.MFRF)
        private IDbDataAdapter daTerritory;
        private DataSet dsTerritory;
        private IClassifier clsTerritory;
        private Dictionary<string, DataRow> cacheTerritory = null;

        private IDbDataAdapter daIssueCapital;
        private DataSet dsIssueCapital;
        private IClassifier clsIssueCapital;
        private Dictionary<string, DataRow> cacheIssueCapital = null;

        #endregion Классификаторы

        #region Факты

        // МФРФ.Сумма долга по ценным бумагам (f.MFRF.DebtCapital)
        private IDbDataAdapter daDebtCapital;
        private DataSet dsDebtCapital;
        private IFactTable fctDebtCapital;
        private Dictionary<string, DataRow> cacheDebitCapital;

        #endregion Факты

        private List<int> deletedDateList = null;

        private int curTerritoryCod = 0;

        #endregion Поля

        #region Закачка данных

        #region Работа с базой и кэшами

        protected override void QueryData()
        {
            InitDataSet(ref daTerritory, ref dsTerritory, clsTerritory,
                        String.Format("SOURCEID = {0} AND ROWTYPE = 0",this.SourceID));
            InitDataSet(ref daIssueCapital, ref dsIssueCapital, clsIssueCapital, false, string.Empty, string.Empty);
            InitFactDataSet(ref daDebtCapital, ref dsDebtCapital, fctDebtCapital);
            FillCaches();
        }

        private void SetMaxTerritoryCode()
        {
            //найдем последний использованный код для территорий.
            if (cacheTerritory.Count > 0)
                curTerritoryCod = cacheTerritory.Max(x => Convert.ToInt32(x.Value["Cod"]));
            if (curTerritoryCod < 0)
                curTerritoryCod = 0;
        }

        private void FillCaches()
        {
            FillRowsCache(ref cacheTerritory, dsTerritory.Tables[0], new string[] { "Name" });
            FillRowsCache(ref cacheIssueCapital, dsIssueCapital.Tables[0], new string[] {"RegNumber", "RegEmissnNumber"}, "|");
            FillRowsCache(ref cacheDebitCapital, dsDebtCapital.Tables[0], new string[] { "RefIssueCapital" });

            SetMaxTerritoryCode();
        }

        protected override void UpdateData()
        {
            UpdateDataSet(daTerritory, dsTerritory, clsTerritory);
            UpdateDataSet(daIssueCapital, dsIssueCapital, clsIssueCapital);
            UpdateDataSet(daDebtCapital, dsDebtCapital, fctDebtCapital);
        }

        private const string F_MFRF_DEBITCAPITAL_GUID = "9f250fc3-b355-4e76-b21c-93a3bdffd198";
        private const string D_TERRITORY_MFRF_GUID = "2c6b9217-60ca-4fac-8b87-4e0ff3f9bba3";
        private const string D_MFRF_ISSUECAPITAL_GUID = "00e814fd-2cc8-4e97-b5b4-c15eb14790b6";

        protected override void InitDBObjects()
        {
            clsIssueCapital = this.Scheme.Classifiers[D_MFRF_ISSUECAPITAL_GUID];
            clsTerritory = this.Scheme.Classifiers[D_TERRITORY_MFRF_GUID];
            this.UsedClassifiers = new IClassifier[] { };
            this.UsedFacts = new IFactTable[] { 
                fctDebtCapital = this.Scheme.FactTables[F_MFRF_DEBITCAPITAL_GUID], 
                };
        }

        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsTerritory);
            ClearDataSet(ref dsIssueCapital);
            ClearDataSet(ref dsDebtCapital);
        }

        #endregion Работа с базой и кэшами

        #region Перекрытые методы закачки

        private int PumpTerritory(string name, int cod)
        {
            return PumpCachedRow(cacheTerritory, dsTerritory.Tables[0], clsTerritory,
                name, new object[] { "Name", name, "Cod", cod, "Code", 0, "OKATO", 0 });
        }

        private int PumpIssueCapital(string regNumber, string regEmissnNumber, string regEmissionDate, string nameNPA, string dateDischarge)
        {
            //проверим уникальность
            string numEmiss = string.Format("{0}|{1}", regNumber, regEmissnNumber);

            DataRow row;

            if (!cacheIssueCapital.ContainsKey(numEmiss))
            {
                //поле ОКТМО(Code), OKATO не откуда брать.  поэтому заполняем 0
                object[] mapping = new object[] { "RegNumber", regNumber, "RegEmissnNumber", regEmissnNumber, "RegEmissionDate", Convert.ToDateTime(regEmissionDate),
                                                    "NameNPA", nameNPA};
                PumpCachedRow(cacheIssueCapital, dsIssueCapital.Tables[0], clsIssueCapital, numEmiss, mapping);
            }
            row = cacheIssueCapital[numEmiss];
            row["DateDischarge"] = Convert.ToDateTime(dateDischarge);
            return Convert.ToInt32(row["ID"]);
        }

        private void PumpDebitCapital(decimal sum, int refTerritory, int refBudLevel, int refIssueCapital, int refYearDayUNV)
        {
            object[] mapping = new object[] { "Sum", sum, "RefYearDayUNV", refYearDayUNV, 
                "RefTerritory", refTerritory, "RefBudLevels", refBudLevel, "RefIssueCapital", refIssueCapital };
            PumpRow(dsDebtCapital.Tables[0], mapping);
            if (dsDebtCapital.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
            {
                UpdateData();
                ClearDataSet(daDebtCapital, ref dsDebtCapital);
            }
        }

        private void DeleteEarlierDataByDate(int refDate)
        {
            if (!deletedDateList.Contains(refDate))
            {
                DeleteData(string.Format("RefYearDayUNV = {0}", refDate), string.Format("Дата отчета: {0}.", refDate));
                deletedDateList.Add(refDate);
            }
        }

        private int DecrementDate(int date)
        {
            int year = date / 10000;
            int month = (date / 100) % 100;
            int day = date % 100;
            month--;
            if (month == 0)
            {
                year--;
                month = 12;
            }
            return year * 10000 + month * 100;
        }

        /// <summary>
        /// считывает последовательность ячеек в столбце до границы
        /// </summary>
        /// <returns></returns>
        private string ReadArrangedCell(ExcelHelper excelDoc, int atRow, int col)
        {
            string result = excelDoc.GetValue(atRow, col).Trim();
            while (excelDoc.GetBorderStyle(atRow, col, ExcelBorderStyles.EdgeBottom) != ExcelLineStyles.Continuous)
            {
                atRow++;
                if (excelDoc.GetBorderStyle(atRow, col, ExcelBorderStyles.EdgeRight) == ExcelLineStyles.Continuous)
                    result += " " + excelDoc.GetValue(atRow, col).Trim();
            }
            return result.Trim();
        }

        private int GetRefDate(ExcelHelper excelDoc, string fileName)
        {
            int countRows = excelDoc.GetRowsCount();
            string strDate = String.Empty;

            //выделим дату
            for (int curRow = 1; curRow<countRows; curRow++)
            {
                strDate = excelDoc.GetValue(curRow, 1).Trim().ToUpper();

                //пропускаем строки вида 08.04.2011 12:26

                if (strDate.Contains(":"))
                    continue;
                strDate = CommonRoutines.LongDateToNewDate(CommonRoutines.TrimLetters(strDate));

                if (strDate != String.Empty)
                    break;
            }

            if (strDate == String.Empty)
            {
                string strErr = string.Format("Не удалось определить дату в файле {0}", fileName);
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeError, strErr);
                throw new Exception(strErr);
            }

            int refYearDayUNV = DecrementDate(Convert.ToInt32(strDate));
            //удалим ранее закаченные данные с такой датой
            DeleteEarlierDataByDate(refYearDayUNV);
            return refYearDayUNV;
        }

        private Decimal GetCost(ExcelHelper excelDoc, int curRow)
        {
            string cost = ReadArrangedCell(excelDoc, curRow, 7);

            //возможно в ячейке оказалась дата/время при экспорте из pdf. убираем.
            if (cost.Contains(':'))
            {
                int indStart = cost.IndexOf(':');
                int indEnd = indStart;
                while ((indStart >= 0) && ((cost[indStart] == ' ') || (cost[indEnd] == ':') || ((cost[indStart] >= '0') && (cost[indStart] <= '9'))))
                    indStart--;
                indStart++;

                while ((indEnd < cost.Length) && ((cost[indEnd] == ':') || ((cost[indEnd] >= '0') && (cost[indEnd] <= '9'))))
                    indEnd++;

                cost = cost.Remove(indStart, indEnd - indStart);
            }

            //после запятой два знака. остальное убираем
            int indSem = cost.IndexOf(',');
            if ((indSem > -1) && (cost.Length > (indSem + 3)))
            {
                cost = cost.Remove(indSem + 3);
            }
            return Convert.ToDecimal(cost.Trim());
        }

        private void PumpXlsSheet(ExcelHelper excelDoc, string fileName)
        {
            string regEmissionDate = "";
            string nameNPA = "";
            string dateDischarge = "";
            string region = "";
            string regNumber = "";
            string regEmissnNumber = "";

            int curRow = 1;
            int refTerritory = -1;
            int refIssueCapital = -1;

            int budgetLevel = 3;

            int refYearDayUNV = GetRefDate(excelDoc, fileName);

            int countRows = excelDoc.GetRowsCount();

            //пропустим шапку таблицы
            while (!excelDoc.GetValue(curRow++, 1).Trim().ToUpper().Contains(SUBJECTS)) ;

            bool startNewBlock = true;

            for (; ; curRow++)
                try
                {
                    SetProgress(countRows, curRow,
                        string.Format("Обработка файла {0}...", fileName),
                        string.Format("Строка {0} из {1}", curRow , countRows));

                    string str = excelDoc.GetValue(curRow, 1).Trim();

                    if (excelDoc.GetBorderStyle(curRow, 1, ExcelBorderStyles.EdgeRight) != ExcelLineStyles.Continuous)
                        continue;

                    if (str.ToUpper() == TOTAL)
                    {
                        startNewBlock = true;
                        continue;
                    }

                    if (str.ToUpper() == TOTALSUM)
                        break;

                    bool fEmptyLine = true;
                    for (int j = 2; (j < 8) && (fEmptyLine);j++)
                    {
                        fEmptyLine = ReadArrangedCell(excelDoc, curRow, j) == "";
                    }

                    if (fEmptyLine)
                        continue;

                    str = ReadArrangedCell(excelDoc, curRow, 1);
                    if (startNewBlock && (str != ""))
                    {
                        region = str;

                        if (region.ToUpper().StartsWith("Г."))
                        {
                            if ((region.ToUpper().Contains("МОСКВА")) || (region.ToUpper().Contains("ПЕТЕРБУРГ")))
                                budgetLevel = 3;
                            else budgetLevel = 15;
                        }
                        else budgetLevel = 3;

                        if (!cacheTerritory.ContainsKey(region))
                        {
                            refTerritory = PumpTerritory(region, ++curTerritoryCod);
                        }
                        else refTerritory = Convert.ToInt32(cacheTerritory[region]["ID"]);
                        startNewBlock = false;
                    }

                    regNumber = ReadArrangedCell(excelDoc, curRow, 2);

                    regEmissnNumber = ReadArrangedCell(excelDoc, curRow, 3);

                    string numEmiss = string.Format("{0}|{1}", regNumber, regEmissnNumber);

                    if (!cacheIssueCapital.ContainsKey(numEmiss))
                    {
                        regEmissionDate = ReadArrangedCell(excelDoc, curRow, 4);
                        nameNPA = ReadArrangedCell(excelDoc, curRow, 5);
                    }
                    dateDischarge = ReadArrangedCell(excelDoc, curRow, 6);

                    refIssueCapital = PumpIssueCapital(regNumber, regEmissnNumber, regEmissionDate, nameNPA, dateDischarge);

                    Decimal cost = GetCost(excelDoc, curRow);
                    PumpDebitCapital(cost * 1000, refTerritory, budgetLevel, refIssueCapital, refYearDayUNV);

                    //пропускаем все ячейки. которые прочитали
                    while (excelDoc.GetBorderStyle(curRow, 2, ExcelBorderStyles.EdgeBottom) != ExcelLineStyles.Continuous)
                    {
                        curRow++;
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
            WriteToTrace("Открытие документа: " + file.Name, TraceMessageKind.Information);
            ExcelHelper excelDoc = new ExcelHelper();
            try
            {
                excelDoc.AskToUpdateLinks = false;
                excelDoc.DisplayAlerts = false;
                excelDoc.EnableEvents = false;
                excelDoc.OpenDocument(file.FullName);
                excelDoc.SetWorksheet(1);
                PumpXlsSheet(excelDoc, file.Name);
            }
            finally
            {
                if (excelDoc != null)
                    excelDoc.CloseDocument();
            }

        }

        protected override void ProcessFiles(DirectoryInfo dir)
        {
            deletedDateList = new List<int>();
            try
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStart, "Старт инициализации Excel.");
                ProcessFilesTemplate(dir, "*.xls", new ProcessFileDelegate(PumpXLSFile), false);
                ProcessFilesTemplate(dir, "*.xml", new ProcessFileDelegate(PumpXLSFile), false);
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

        #endregion Перекрытые методы закачки

        #endregion Закачка данных

    }
}
