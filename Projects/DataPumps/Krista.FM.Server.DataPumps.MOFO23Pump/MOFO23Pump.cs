using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Krista.FM.Server.DataPumps;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.MOFO23Pump
{

    // МОФО - 0023 - Поступления по арендной плате
    public class MOFO23PumpModule : CorrectedPumpModuleBase
    {

        #region Поля

        #region Классификаторы

        // Показатели.Поступления по арендной плате (d_Marks_Receipt)
        private IDbDataAdapter daMarks;
        private DataSet dsMarks;
        private IClassifier clsMarks;
        private Dictionary<int, int> cacheMarks;
        // Районы.Планирование (d_Regions_Plan)
        private IDbDataAdapter daRegions;
        private DataSet dsRegions;
        private IClassifier clsRegions;
        private Dictionary<string, int> cacheRegions = null;
        private Dictionary<string, int> cacheRegionsTerrType = null;
        private int nullRegions = -1;
        private int nullTerrType = 0;

        #endregion Классификаторы

        #region Факты

        // Факт.Поступления по арендной плате (f_F_Rental)
        private IDbDataAdapter daRental;
        private DataSet dsRental;
        private IFactTable fctRental;

        #endregion Факты

        private int refDate;
        private ReportType reportType;
        private int sourceID;
        private List<int> deletedSourceIDList = null;
        private List<string> deletedDatesAndRegionsList = null;
        // соответствие: строка отчета => код показателя
        private Dictionary<int, int> marksCodes = null;
        private List<string> absentMarksCodes = null;

        #endregion Поля

        #region Перечисления

        private enum ReportType
        {
            // данные по городским округам (файлы ar_contr_go_ГГMMННН.xls)
            ArendaGO,
            // данные по муниципальным районам (файлы ar_contr_mr_ГГMMННН.xls)
            ArendaMR
        }

        #endregion Перечисления

        #region Закачка данных

        #region Работа с базой и кэшем

        private void SetNewSourceID()
        {
            sourceID = this.AddDataSource("ФО", "0029", ParamKindTypes.Year, string.Empty,
                this.DataSource.Year, 0, string.Empty, 0, string.Empty).ID;

            if (deletedSourceIDList == null)
                deletedSourceIDList = new List<int>();

            if (!deletedSourceIDList.Contains(sourceID) && this.DeleteEarlierData)
            {
                DirectDeleteFactData(new IFactTable[] { fctRental }, -1, sourceID, string.Empty);
                deletedSourceIDList.Add(sourceID);
            }
        }

        private void FillRegionsCache(DataTable dt)
        {
            cacheRegions = new Dictionary<string, int>();
            cacheRegionsTerrType = new Dictionary<string, int>();
            foreach (DataRow row in dt.Rows)
            {
                string codeLine = Convert.ToString(row["CodeLine"]);
                // если поле CodeLine заполнено, то это МР или ГО, его просто добавляем в кэш
                if (codeLine == string.Empty)
                {
                    // если нет, то это поселение. в этом случае порядковый номер CodeLine будет составной:
                    // в начале идут первые 1 или 2 цифры из поля Code, дополненные до 3-х знаков нулями;
                    // в конце - последние 2 цифры из поля Code
                    string code = Convert.ToString(row["Code"]);
                    if (code.Length < 5)
                        continue;
                    codeLine = code.Substring(0, code.Length - 4).PadLeft(3, '0') + code.Substring(code.Length - 2);
                }
                if (!cacheRegions.ContainsKey(codeLine))
                {
                    cacheRegions.Add(codeLine, Convert.ToInt32(row["ID"]));
                    cacheRegionsTerrType.Add(codeLine, Convert.ToInt32(row["RefTerr"]));
                }
            }
        }

        private void FillCaches()
        {
            FillRowsCache(ref cacheMarks, dsMarks.Tables[0], "Codeind");
            FillRegionsCache(dsRegions.Tables[0]);
        }

        protected override void QueryData()
        {
            SetNewSourceID();

            InitDataSet(ref daRegions, ref dsRegions, clsRegions, string.Format("SourceID = {0}", sourceID));
            nullRegions = clsRegions.UpdateFixedRows(this.DB, sourceID);
            InitDataSet(ref daMarks, ref dsMarks, clsMarks, string.Empty);
            InitFactDataSet(ref daRental, ref dsRental, fctRental);

            FillCaches();
        }

        #region GUIDs

        private const string D_MARKS_RECEIPT_GUID = "31707260-f304-4f7f-9d20-45466cd3c329";
        private const string D_REGIONS_ANALYSIS_GUID = "1f34cc90-16fd-4fcf-b994-0c8a680d7e23";
        private const string F_F_RENTAL_GUID = "4cc2886b-1498-4295-9d71-8aa8ac265959";

        #endregion GUIDs
        protected override void InitDBObjects()
        {
            clsMarks = this.Scheme.Classifiers[D_MARKS_RECEIPT_GUID];
            clsRegions = this.Scheme.Classifiers[D_REGIONS_ANALYSIS_GUID];
            fctRental = this.Scheme.FactTables[F_F_RENTAL_GUID];

            this.UsedClassifiers = new IClassifier[] { };
            this.UsedFacts = new IFactTable[] { fctRental };
        }

        protected override void UpdateData()
        {
            UpdateDataSet(daRental, dsRental, fctRental);
        }

        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsRental);
            ClearDataSet(ref dsMarks);
            ClearDataSet(ref dsRegions);
        }

        #endregion Работа с базой и кэшем

        #region Работа с Xls

        private decimal CleanFactValue(string value)
        {
            decimal factValue = 0;
            Decimal.TryParse(CommonRoutines.TrimLetters(value).Replace('.', ','), out factValue);
            return factValue;
        }

        private void PumpFactRow(string value, int refDate, int refRegions, int refBudgetLevel, int refMarks)
        {
            decimal factValue = CleanFactValue(value);
            if (factValue == 0)
                return;

            object[] mapping = new object[] {
                "Fact", factValue * 1000, // данные в тыс.руб. переводим в рубли
                "RefYearDayUNV", refDate,
                "RefRegions", refRegions,
                "RefBudLevel", refBudgetLevel,
                "RefReceipt", refMarks,
                "SourceID", sourceID
            };

            PumpRow(dsRental.Tables[0], mapping);
            if (dsRental.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
            {
                UpdateData();
                ClearDataSet(daRegions, ref dsRental);
            }
        }

        private int GetRefMarks(int curRow)
        {
            if (!marksCodes.ContainsKey(curRow))
                return -1;

            int code = marksCodes[curRow];
            if (!cacheMarks.ContainsKey(code))
            {
                if (!absentMarksCodes.Contains(code.ToString()))
                    absentMarksCodes.Add(code.ToString());
                return -1;
            }

            return cacheMarks[code];
        }

        private int GetRefBudgetLevel(int curColumn, int refTerrType)
        {
            switch (reportType)
            {
                case ReportType.ArendaGO:
                    if (curColumn == 9)
                        return 3;
                    return 15;
                case ReportType.ArendaMR:
                    if (curColumn == 9)
                        return 5;
                    switch (refTerrType)
                    {
                        case 5:
                            return 16;
                        case 6:
                            return 17;
                    }
                    break;
            }
            return 0;
        }

        private void GetReportBorders(ref int firstRow, ref int lastRow)
        {
            switch (reportType)
            {
                case ReportType.ArendaGO:
                    firstRow = 16;
                    lastRow = 30;
                    break;
                case ReportType.ArendaMR:
                    firstRow = 16;
                    lastRow = 35;
                    break;
            }
        }

        private void PumpXlsSheet(ExcelHelper excelDoc, int refDate, int refRegions, int refTerrType)
        {
            int firstRow = 0;
            int lastRow = 0;
            GetReportBorders(ref firstRow, ref lastRow);
            for (int curRow = firstRow; curRow <= lastRow; curRow++)
            {
                int refMarks = GetRefMarks(curRow);
                if (refMarks == -1)
                    continue;

                PumpFactRow(excelDoc.GetValue(curRow, 9), refDate, refRegions, GetRefBudgetLevel(9, refTerrType), refMarks);
                PumpFactRow(excelDoc.GetValue(curRow, 10), refDate, refRegions, GetRefBudgetLevel(10, refTerrType), refMarks);
            }
        }

        private void SetReportType(string filename)
        {
            filename = filename.ToUpper();
            if (filename.Contains("GO"))
                reportType = ReportType.ArendaGO;
            if (filename.Contains("MR"))
                reportType = ReportType.ArendaMR;
        }

        private void DeleteEarlierDataByDateAndRegions(int refDate, int refRegions)
        {
            if (deletedDatesAndRegionsList == null)
                deletedDatesAndRegionsList = new List<string>();

            string key = string.Format("{0}|{1}", refDate, refRegions);
            if (!deletedDatesAndRegionsList.Contains(key) && !this.DeleteEarlierData)
            {
                string constaint = string.Format("RefRegions = {0} and RefYearDayUNV = {1}", refRegions, refDate);
                DirectDeleteFactData(new IFactTable[] { fctRental }, -1, sourceID, constaint);
                deletedDatesAndRegionsList.Add(key);
            }
        }

        private void GetRefRegions(string value, ref int refRegions, ref int refTerrType)
        {
            string codeLine = string.Empty;
            switch (reportType)
            {
                case ReportType.ArendaGO:
                    // из названия файла ar_contr_go_ГГMMННН.xls берем ННН
                    codeLine = CommonRoutines.TrimLetters(value.Split('_')[3].Trim());
                    codeLine = codeLine.Substring(4);
                    break;
                case ReportType.ArendaMR:
                    // из названия листа z_РРРНН берем РРР - для МР, РРРНН - для поселений
                    codeLine = CommonRoutines.TrimLetters(value);
                    if (Convert.ToInt32(codeLine.Substring(codeLine.Length - 2)) == 0)
                        codeLine = codeLine.Substring(0, 3);
                    break;
            }

            if (!cacheRegions.ContainsKey(codeLine))
            {
                throw new PumpDataFailedException(string.Format(
                    "Не найдено муниципальное образование с кодом {0} в справочнике «Районы.Планирование».", codeLine));
            }

            refRegions = cacheRegions[codeLine];
            refTerrType = cacheRegionsTerrType[codeLine];

            DeleteEarlierDataByDateAndRegions(refDate, refRegions);
        }

        private bool SkipSheet(string sheetname)
        {
            return !sheetname.Trim().ToUpper().StartsWith("Z_");
        }

        private void SetMarksCodes()
        {
            switch (reportType)
            {
                case ReportType.ArendaGO:
                    marksCodes = new Dictionary<int, int>() { { 16, 1 }, { 18, 2 }, { 20, 3 },
                        { 21, 7 }, { 23, 8 }, { 25, 9 }, { 26, 16 }, { 28, 17 }, { 30, 18 } };
                    break;
                case ReportType.ArendaMR:
                    marksCodes = new Dictionary<int, int>() { { 16, 4 }, { 18, 5 }, { 20, 6 },
                        { 21, 10 }, { 23, 11 }, { 25, 12 }, { 26, 13 }, { 28, 14 }, { 30, 15 },
                        { 31, 16 }, { 33, 17 }, { 35, 18 } };
                    break;
            }
        }

        private int GetRefDate()
        {
            // дату берем по параметрам источника
            return this.DataSource.Year * 10000 + this.DataSource.Month * 100;
        }

        private void PumpXlsFile(FileInfo file)
        {
            WriteToTrace("Открытие документа: " + file.Name, TraceMessageKind.Information);
            ExcelHelper excelDoc = new ExcelHelper();
            try
            {
                SetReportType(file.Name);
                SetMarksCodes();

                excelDoc.AskToUpdateLinks = false;
                excelDoc.DisplayAlerts = false;
                excelDoc.EnableEvents = false;
                excelDoc.OpenDocument(file.FullName);

                int refRegions = nullRegions;
                int refTerrType = nullTerrType;
                switch (reportType)
                {
                    case ReportType.ArendaGO:
                        excelDoc.SetWorksheet(1);
                        GetRefRegions(file.Name, ref refRegions, ref refTerrType);
                        PumpXlsSheet(excelDoc, refDate, refRegions, refTerrType);
                        break;
                    case ReportType.ArendaMR:
                        int wsCount = excelDoc.GetWorksheetsCount();
                        for (int index = 1; index <= wsCount; index++)
                        {
                            excelDoc.SetWorksheet(index);
                            if (SkipSheet(excelDoc.GetWorksheetName()))
                                continue;
                            GetRefRegions(excelDoc.GetWorksheetName(), ref refRegions, ref refTerrType);
                            PumpXlsSheet(excelDoc, refDate, refRegions, refTerrType);
                        }
                        break;
                }
            }
            catch (PumpDataFailedException ex)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeFinishFilePumpWithError, string.Format(
                    "Ошибка при обработке файла '{0}': {1} Файл будет пропущен.", file.Name, ex.Message));
            }
            finally
            {
                if (excelDoc != null)
                    excelDoc.CloseDocument();
            }
        }

        #endregion Работа с Xls

        #region Перекрытые методы

        private void ShowAbsentMarksCodes()
        {
            if (absentMarksCodes.Count == 0)
                return;
            absentMarksCodes.Sort();
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, string.Format(
                "В классификаторе '{0}' отсутствуют записи с кодами: {1}.",
                clsMarks.FullCaption, string.Join("', '", absentMarksCodes.ToArray())));
        }

        protected override void ProcessFiles(DirectoryInfo dir)
        {
            absentMarksCodes = new List<string>();
            try
            {
                refDate = GetRefDate();
                ProcessFilesTemplate(dir, "*.xls", new ProcessFileDelegate(PumpXlsFile), false);
                ShowAbsentMarksCodes();
            }
            finally
            {
                absentMarksCodes.Clear();
            }
        }

        protected override void DirectPumpData()
        {
            PumpDataYMTemplate();
        }

        #endregion Перекрытые методы

        #endregion Закачка данных

    }

}
