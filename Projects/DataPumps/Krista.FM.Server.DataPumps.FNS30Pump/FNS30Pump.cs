using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.FNS30Pump
{
    // фнс 30 - дополнительная информация от фнс
    public class FNS30PumpModule : CorrectedPumpModuleBase
    {

        #region Поля

        #region Классификаторы

        // Организации.ФНС_Должники (d_Org_FNSDebtor)
        private IDbDataAdapter daDebtor;
        private DataSet dsDebtor;
        private IClassifier clsDebtor;
        private Dictionary<string, int> cacheDebtor = null;

        #endregion Классификаторы

        #region Факты

        // Факт.ФНС_Задолженность (f_F_FNSArrears)
        private IDbDataAdapter daArrears;
        private DataSet dsArrears;
        private IFactTable fctArrears;

        #endregion Факты

        private ReportType reportType;
        private bool isDeletedFNSData = false;
        private bool isDeletedFondsData = false;

        #endregion Поля

        #region Константы

        private const string DATA_KIND_FNS = "ФНС";
        private const string DATA_KIND_FONDS = "ПФ и ФОМС";

        #endregion Константы

        #region Перечисления

        /// <summary>
        /// Тип отчета
        /// </summary>
        private enum ReportType
        {
            /// <summary>
            /// Файлы вида ФНС_ГГГГММДД.xls
            /// </summary>
            FNSFacts,
            /// <summary>
            /// Файлы вида Фонды_ГГГГММДД.xls
            /// </summary>
            FondsCls,
            /// <summary>
            /// Неизвестный формат
            /// </summary>
            Unknown
        }

        #endregion Перечисления

        #region Закачка данных

        #region Работа с базой и кэшами

        protected override void QueryData()
        {
            InitDataSet(ref daDebtor, ref dsDebtor, clsDebtor, false, string.Empty, string.Empty);
            InitFactDataSet(ref daArrears, ref dsArrears, fctArrears);
        }

        protected override void UpdateData()
        {
            UpdateDataSet(daDebtor, dsDebtor, clsDebtor);
            UpdateDataSet(daArrears, dsArrears, fctArrears);
        }

        #region GUIDs

        private const string D_ORG_FNSDEBTOR_GUID = "1157397c-a2d2-4190-9e22-f013bfe6012d";
        private const string F_F_FNS_ARREARS_GUID = "5802d147-590b-4a77-acf2-669affbd2620";

        #endregion GUIDs
        protected override void InitDBObjects()
        {
            clsDebtor = this.Scheme.Classifiers[D_ORG_FNSDEBTOR_GUID];
            fctArrears = this.Scheme.FactTables[F_F_FNS_ARREARS_GUID];
            this.CubeClassifiers = new IClassifier[] { clsDebtor };
            this.UsedClassifiers = new IClassifier[] { };
            this.UsedFacts = new IFactTable[] { };
        }

        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsArrears);
            ClearDataSet(ref dsDebtor);
        }

        #endregion Работа с базой и кэшами

        #region Работа с Excel

        private double CleanFactValue(string value)
        {
            return Convert.ToDouble(value.PadLeft(1, '0'));
        }

        private void PumpArrears(string value, int refDate, int refDebtor)
        {
            double factValue = CleanFactValue(value);
            if (factValue == 0)
                return;

            if (reportType == ReportType.FondsCls)
                factValue *= 1000;
            PumpRow(dsArrears.Tables[0], new object[] { "Value", factValue, "RefOrg", refDebtor, "RefYearDayUNV", refDate });
            if (dsArrears.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
            {
                UpdateData();
                ClearDataSet(daArrears, ref dsArrears);
            }
        }

        private int PumpDebtor(string innStr, string kppStr, string name, string dataKind)
        {
            long inn = Convert.ToInt64(innStr.Trim().PadLeft(1, '0'));
            long kpp = Convert.ToInt64(kppStr.Trim().PadLeft(1, '0'));
            name = name.Trim();
            int ifns = 0;
            if (kppStr.Trim().Length > 4)
                ifns = Convert.ToInt32(kppStr.Trim().Substring(0, 4));

            string key = string.Format("{0}|{1}|{2}|{3}", inn, kpp, name, dataKind);
            return PumpCachedRow(cacheDebtor, dsDebtor.Tables[0], clsDebtor, key, new object[] {
                "Inn", inn, "Kpp", kpp, "Name", name, "DataKind", dataKind, "IFNS", ifns });
        }

        private void PumpXlsRow(ExcelHelper excelDoc, int curRow, int refDate)
        {
            switch (reportType)
            {
                case ReportType.FNSFacts:
                    int refDebtor = PumpDebtor(excelDoc.GetValue(curRow, 2), excelDoc.GetValue(curRow, 3),
                        excelDoc.GetValue(curRow, 4), DATA_KIND_FNS);
                    PumpArrears(excelDoc.GetValue(curRow, 5), refDate, refDebtor);
                    break;
                case ReportType.FondsCls:
                    PumpDebtor(excelDoc.GetValue(curRow, 3), excelDoc.GetValue(curRow, 4),
                        excelDoc.GetValue(curRow, 2), DATA_KIND_FONDS);
                    break;
            }
        }

        private int GetXlsDate(string filename)
        {
            if (reportType == ReportType.FNSFacts)
            {
                string date = CommonRoutines.TrimLetters(filename);
                return Convert.ToInt32(date);
            }
            return -1;
        }

        private void PumpXlsSheet(ExcelHelper excelDoc, string filename)
        {
            int refDate = GetXlsDate(filename);
            int rowsCount = excelDoc.GetRowsCount();
            int startRow = reportType == ReportType.FNSFacts ? 3 : 4;
            for (int curRow = startRow; curRow <= rowsCount; curRow++)
                try
                {
                    SetProgress(rowsCount, curRow,
                        string.Format("Обработка файла {0}...", filename),
                        string.Format("Строка {0} из {1}", curRow, rowsCount));

                    string cellValue = excelDoc.GetValue(curRow, 1).Trim();
                    if (cellValue == string.Empty)
                        continue;

                    PumpXlsRow(excelDoc, curRow, refDate);
                }
                catch (Exception ex)
                {
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeError,
                        string.Format("При обработке строки {0} листа '{1}' отчета '{2}' возникла ошибка ({3})",
                        curRow, excelDoc.GetWorksheetName(), filename, ex.Message));
                    this.DataSourceProcessingResult = DataSourceProcessingResult.ProcessedWithErrors;
                }
        }

        private ReportType GetReportType(string filename)
        {
            filename = filename.Trim().ToUpper();
            if (filename.StartsWith("ФНС"))
                return ReportType.FNSFacts;
            if (filename.StartsWith("ФОНДЫ"))
                return ReportType.FondsCls;
            return ReportType.Unknown;
        }

        private void DeletePumpedData()
        {
            switch (reportType)
            {
                case ReportType.FNSFacts:
                    if (!isDeletedFNSData)
                    {
                        DirectDeleteFactData(new IFactTable[] { fctArrears }, -1, -1, string.Empty);
                        DirectDeleteClsData(new IClassifier[] { clsDebtor }, -1, -1, string.Format(
                            "upper(DataKind) <> upper('{0}') or DataKind is null", DATA_KIND_FONDS));
                        isDeletedFNSData = true;
                    }
                    break;
                case ReportType.FondsCls:
                    if (!isDeletedFondsData)
                    {
                        DirectDeleteClsData(new IClassifier[] { clsDebtor }, -1, -1, string.Format(
                            "upper(DataKind) <> upper('{0}') or DataKind is null", DATA_KIND_FNS));
                        isDeletedFondsData = true;
                    }
                    break;
            }
        }

        private void PumpXlsFile(FileInfo file)
        {
            WriteToTrace("Открытие документа: " + file.Name, TraceMessageKind.Information);
            ExcelHelper excelDoc = new ExcelHelper();
            try
            {
                reportType = GetReportType(file.Name);
                if (reportType == ReportType.Unknown)
                {
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, string.Format(
                        "Не удалось определить формат данных. Файл '{0}' будет пропущен.", file.Name));
                    return;
                }
                DeletePumpedData();

                excelDoc.AskToUpdateLinks = false;
                excelDoc.DisplayAlerts = false;
                excelDoc.EnableEvents = false;
                excelDoc.OpenDocument(file.FullName);

                int wsCount = excelDoc.GetWorksheetsCount();
                for (int index = 1; index <= wsCount; index++)
                {
                    excelDoc.SetWorksheet(index);
                    PumpXlsSheet(excelDoc, file.Name);
                }
            }
            finally
            {
                if (excelDoc != null)
                    excelDoc.CloseDocument();
            }
        }

        #endregion Работа с Excel

        #region Перекрытые методы закачки

        protected override void ProcessFiles(DirectoryInfo dir)
        {
            ProcessFilesTemplate(dir, "*.xls", new ProcessFileDelegate(PumpXlsFile), false);
            UpdateData();
        }

        protected override void DirectPumpData()
        {
            cacheDebtor = new Dictionary<string, int>();
            try
            {
                PumpDataYTemplate();
            }
            finally
            {
                cacheDebtor.Clear();
            }
        }

        #endregion Перекрытые методы закачки

        #endregion Закачка данных

    }
}
