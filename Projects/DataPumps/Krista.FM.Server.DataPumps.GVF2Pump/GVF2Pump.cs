using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using Krista.FM.Server.DataPumps;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.GVF2Pump
{
    // ГВФ - 0002 - Форма 9 Ф (СС)
    public class GVF2PumpModule : CorrectedPumpModuleBase
    {

        #region Поля

        #region Классификаторы

        // Показатели.ГВФ_9 Ф СС (d_Marks_GVF9FSS)
        private IDbDataAdapter daMarks;
        private DataSet dsMarks;
        private IClassifier clsMarks;
        private Dictionary<int, int> cacheMarks = null;
        // ЕдИзмер.ОКЕИ (d_Units_OKEI)
        private IDbDataAdapter daUnits;
        private DataSet dsUnits;
        private IClassifier clsUnits;
        private Dictionary<string, int> cacheUnits = null;
        private int nullUnits = -1;

        #endregion Классификаторы

        #region Факты

        // Фонды.ГВФ_9 Ф СС (f_Fund_GVF9FSS)
        private IDbDataAdapter daFundGVF9;
        private DataSet dsFundGVF9;
        private IFactTable fctFundGVF9;

        #endregion Факты

        // источник классификаторов (формируются на год)
        private int clsSourceId;
        // дата отчета (определяется параметрами источника)
        private int reportDate;
        // параметры обработки
        private int year;
        private int month;

        #endregion Поля

        #region Закачка данных

        #region Работа с базой и кэшами

        private void FillCaches()
        {
            FillRowsCache(ref cacheMarks, dsMarks.Tables[0], "Code");
            FillRowsCache(ref cacheUnits, dsUnits.Tables[0], "Name", "ID");
        }

        protected override void QueryData()
        {
            clsSourceId = AddDataSource("ГВФ", "2", ParamKindTypes.Year,
                string.Empty, this.DataSource.Year, 0, string.Empty, 0, string.Empty).ID;
            InitDataSet(ref daMarks, ref dsMarks, clsMarks, string.Format("SourceId = {0}", clsSourceId));
            InitDataSet(ref daUnits, ref dsUnits, clsUnits, true, string.Empty, string.Empty);
            InitFactDataSet(ref daFundGVF9, ref dsFundGVF9, fctFundGVF9);
            FillCaches();
        }

        #region GUIDs

        private const string D_MARKS_GVF9FPF_GUID = "bfa7cdec-f049-42a1-a523-ecdd027c41b6";
        private const string D_UNITS_OKEI_GUID = "7ef0edfd-9461-4333-8420-ccb102051826";
        private const string F_FUND_GVF9FPF_GUID = "a311e942-42a1-46b7-9faa-d8c0e845503e";

        #endregion GUIDs
        protected override void InitDBObjects()
        {
            clsMarks = this.Scheme.Classifiers[D_MARKS_GVF9FPF_GUID];
            clsUnits = this.Scheme.Classifiers[D_UNITS_OKEI_GUID];
            fctFundGVF9 = this.Scheme.FactTables[F_FUND_GVF9FPF_GUID];
            this.UsedClassifiers = new IClassifier[] { };
            this.UsedFacts = new IFactTable[] { fctFundGVF9 };
        }

        protected override void UpdateData()
        {
            UpdateDataSet(daMarks, dsMarks, clsMarks);
            UpdateDataSet(daFundGVF9, dsFundGVF9, fctFundGVF9);
        }

        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsFundGVF9);
            ClearDataSet(ref dsMarks);
            ClearDataSet(ref dsUnits);
        }

        #endregion Работа с базой и кэшами

        #region Общие методы

        // удаление цифр в конце строки
        private string RemoveLastDigits(string inputStr)
        {
            Regex regExLastDigits = new Regex(@"(.*?)(\d*)$", RegexOptions.Multiline | RegexOptions.Compiled);
            return regExLastDigits.Replace(inputStr, "$1");
        }

        private decimal CleanFactValue(string factValue)
        {
            factValue = CommonRoutines.TrimLetters(factValue.Trim()).PadLeft(1, '0');
            return Convert.ToDecimal(factValue);
        }

        private void SetReportDate()
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, "Дата будет определена параметрами источника");
            reportDate = this.DataSource.Year * 10000 + 9990 + this.DataSource.Quarter;
        }

        #endregion Общие методы

        #region Работа с Xls-файлами

        private int PumpXlsMarks(ExcelHelper excelDoc, int curRow, string marksName)
        {
            string codeStr = excelDoc.GetValue(curRow, 69).Trim().PadLeft(1, '0');
            int code = Convert.ToInt32(codeStr);
            return PumpCachedRow(cacheMarks, dsMarks.Tables[0], clsMarks, code, "Code", new object[] {
                "Code", code, "Name", marksName, "SourceID", clsSourceId });
        }

        private void PumpXlsRow(ExcelHelper excelDoc, int curRow, string marksName)
        {
            int refMarks = PumpXlsMarks(excelDoc, curRow, marksName);
            decimal value = CleanFactValue(excelDoc.GetValue(curRow, 75)) * 1000;
            object[] mapping = new object[] { "Value", value, "RefMarks", refMarks, "RefYearDayUNV", reportDate };

            PumpRow(dsFundGVF9.Tables[0], mapping);
            if (dsFundGVF9.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
            {
                UpdateData();
                ClearDataSet(daFundGVF9, ref dsFundGVF9);
            }
        }

        private string GetMarksName(ExcelHelper excelDoc, ref int curRow)
        {
            string marksName = RemoveLastDigits(excelDoc.GetValue(curRow, 1).Trim());
            while (excelDoc.GetValue(curRow + 1, 69).Trim() == string.Empty)
            {
                curRow++;
                string cellValue = excelDoc.GetValue(curRow, 1).Trim();
                if (cellValue == string.Empty)
                    break;
                marksName = RemoveLastDigits(string.Format("{0} {1}", marksName, cellValue));
            }
            return marksName;
        }

        private bool IsSkipRow(string cellValue)
        {
            return (cellValue == string.Empty);
        }

        private void PumpXlsSheetDate(ExcelHelper excelDoc, string fileName)
        {
            bool toPump = false;
            string dataSourcePath = GetShortSourcePathBySourceID(this.SourceID);
            int rowsCount = excelDoc.GetRowsCount();
            for (int curRow = 1; curRow <= rowsCount; curRow++)
                try
                {
                    SetProgress(rowsCount, curRow,
                        string.Format("Обработка файла {0}\\{1}...", dataSourcePath, fileName),
                        string.Format("Строка {0} из {1}", curRow, rowsCount));

                    string cellValue = excelDoc.GetValue(curRow, 1).Trim();
                    if (IsSkipRow(cellValue))
                        continue;

                    if (cellValue == "1")
                    {
                        toPump = true;
                        continue;
                    }

                    if (cellValue.ToUpper().StartsWith("СПРАВОЧНО"))
                        toPump = false;

                    if (toPump)
                    {
                        int newRow = curRow;
                        string marksName = GetMarksName(excelDoc, ref newRow);
                        PumpXlsRow(excelDoc, curRow, marksName);
                        curRow = newRow;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format(
                        "При обработке строки {0} листа '{1}' возникла ошибка ({2})",
                        curRow, excelDoc.GetWorksheetName(), ex.Message), ex);
                }
        }

        private bool IsSkipWorksheet(ExcelHelper excelDoc)
        {
            string worksheetName = excelDoc.GetWorksheetName().Trim().ToUpper();
            return ((worksheetName != "ЛИСТ2") && (worksheetName != "ЛИСТ3") && (worksheetName != "ЛИСТ4"));
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
                    excelDoc.SetWorksheet(index);
                    if (IsSkipWorksheet(excelDoc))
                        continue;
                    PumpXlsSheetDate(excelDoc, file.Name);
                }
            }
            finally
            {
                if (excelDoc != null)
                    excelDoc.CloseDocument();
            }
        }

        #endregion Работа с Xls-файлами

        #region Перектырые методы закачки

        protected override void ProcessFiles(DirectoryInfo dir)
        {
            SetReportDate();
            ProcessFilesTemplate(dir, "*.xls", new ProcessFileDelegate(PumpXlsFile), false);
            UpdateData();
        }

        protected override void DirectPumpData()
        {
            PumpDataYQTemplate();
        }

        #endregion Перекрытые методы закачки

        #endregion Закачка данных

        #region Обработка данных

        private const string ROUBLE_UNIT_NAME = "Рубль";
        private void SetRefUnits()
        {
            if (cacheUnits.Count <= 1)
            {
                WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeWarning, "Классификатор «ЕдИзмер.ОКЕИ» не заполнен.");
                return;
            }

            int refUnits = FindCachedRow(cacheUnits, ROUBLE_UNIT_NAME, nullUnits);
            foreach (DataRow row in dsMarks.Tables[0].Rows)
            {
                row["RefUnits"] = refUnits;
            }
        }

        protected override void ProcessDataSource()
        {
            SetRefUnits();
            UpdateData();
        }

        protected override void DirectProcessData()
        {
            year = -1;
            month = -1;
            GetPumpParams(ref year, ref month);
            ProcessDataSourcesTemplate(year, month, "Выполняется проставление ссылок на классификатор ЕдИзмер.ОКЕИ");
        }

        #endregion Обработка данных

    }
}
