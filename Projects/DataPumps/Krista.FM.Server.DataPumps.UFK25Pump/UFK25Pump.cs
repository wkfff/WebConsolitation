using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Xml;
using Krista.FM.Common.Xml;
using Krista.FM.Server.DataPumps;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.UFK25Pump
{

    // УФК - 0025 - Отчет по поступлениям и выбытиям
    public partial class UFK25PumpModule : CorrectedPumpModuleBase
    {

        #region Поля

        #region Классификаторы

        // КД.УФК (d_KD_UFK)
        private IDbDataAdapter daKd;
        private DataSet dsKd;
        private IClassifier clsKd;
        private Dictionary<string, int> cacheKd = null;
        // КИФ.УФК (d_KIF_UFK)
        private IDbDataAdapter daKif;
        private DataSet dsKif;
        private IClassifier clsKif;
        private Dictionary<string, int> cacheKif = null;
        // КОСГУ.УФК (d_EKR_UFK)
        private IDbDataAdapter daEkr;
        private DataSet dsEkr;
        private IClassifier clsEkr;
        private Dictionary<string, int> cacheEkr = null;
        // Расходы.УФК (d_R_UFK)
        private IDbDataAdapter daOutcomes;
        private DataSet dsOutcomes;
        private IClassifier clsOutcomes;
        private Dictionary<string, int> cacheOutcomes = null;

        #endregion Классификаторы

        #region Факты

        // Доходы.УФК_Отчет по поступл и выбыт_Доходы (f_D_ReportArrival)
        private IDbDataAdapter daReportArrival;
        private DataSet dsReportArrival;
        private IFactTable fctReportArrival;
        // Расходы.УФК_Отчет по поступл и выбыт_Расходы (f_R_ReportCharges)
        private IDbDataAdapter daReportCharges;
        private DataSet dsReportCharges;
        private IFactTable fctReportCharges;
        // ДефицитПрофицит.УФК_Отчет по поступл и выбыт_ДефицитПрофицит (f_DP_ReportDeficit)
        private IDbDataAdapter daReportDeficit;
        private DataSet dsReportDeficit;
        private IFactTable fctReportDeficit;
        // ИФ.УФК_Отчте по поступл и выбыт_ИстФин (f_S_ReportSource)
        private IDbDataAdapter daReportSource;
        private DataSet dsReportSource;
        private IFactTable fctReportSource;

        #endregion Факты

        private int sectionIndex = -1;
        private int nullRegions = -1;
        private XlsReportType xlsReportType = XlsReportType.Unknown;

        #endregion Поля

        #region Перечисления

        /// <summary>
        /// Тип Xls-отчета
        /// </summary>
        private enum XlsReportType
        {
            /// <summary>
            /// Xls-отчет с несколькими листами
            /// </summary>
            ManySheets,
            /// <summary>
            /// Xls-отчет с одним листом 1-го типа
            /// </summary>
            FirstOneSheet,
            /// <summary>
            /// Xls-отчет с одним листом 2-го типа
            /// </summary>
            SecondOneSheet,
            Unknown
        }

        #endregion Перечисления

        #region Закачка данных

        #region Работа с базой и кэшами

        private void FillCaches()
        {
            FillRowsCache(ref cacheKd, dsKd.Tables[0], "CodeStr");
            FillRowsCache(ref cacheKif, dsKif.Tables[0], "CodeStr");
            FillRowsCache(ref cacheEkr, dsEkr.Tables[0], "Code");
            FillRowsCache(ref cacheOutcomes, dsOutcomes.Tables[0], "Code");
        }

        protected override void QueryData()
        {
            InitClsDataSet(ref daKd, ref dsKd, clsKd);
            InitClsDataSet(ref daKif, ref dsKif, clsKif);
            InitClsDataSet(ref daEkr, ref dsEkr, clsEkr);
            InitClsDataSet(ref daOutcomes, ref dsOutcomes, clsOutcomes);
            InitFactDataSet(ref daReportArrival, ref dsReportArrival, fctReportArrival);
            InitFactDataSet(ref daReportCharges, ref dsReportCharges, fctReportCharges);
            InitFactDataSet(ref daReportDeficit, ref dsReportDeficit, fctReportDeficit);
            InitFactDataSet(ref daReportSource, ref dsReportSource, fctReportSource);
            FillCaches();
        }

        #region GUIDs

        private const string D_KD_UFK_GUID = "b713e1df-5584-4e3d-a399-8828a2906971";
        private const string D_KIF_UFK_GUID = "73b83ed3-fa26-4d05-8e8e-30dbe226a801";
        private const string D_EKR_UFK_GUID = "b234f8dc-d37d-4cc0-a32e-2e74b2bfb935";
        private const string D_R_UFK_GUID = "ba2b17a6-191f-477c-894d-2f879053a69e";
        private const string F_D_REPORT_ARRIVAL_GUID = "df44d882-89db-41d3-ba29-e928721d6871";
        private const string F_R_REPORT_CHARGES_GUID = "8e91b0d6-667a-4588-ae80-a83ce3c966db";
        private const string F_DP_REPORT_DEFICIT_GUID = "e7565a7a-f633-4f47-9e82-1d15b5184b14";
        private const string F_S_REPORT_SOURCE_GUID = "1eedcc93-dd48-4a38-ad19-b7481f0abc46";

        #endregion GUIDs
        protected override void InitDBObjects()
        {
            clsKd = this.Scheme.Classifiers[D_KD_UFK_GUID];
            clsKif = this.Scheme.Classifiers[D_KIF_UFK_GUID];
            clsEkr = this.Scheme.Classifiers[D_EKR_UFK_GUID];
            clsOutcomes = this.Scheme.Classifiers[D_R_UFK_GUID];

            fctReportArrival = this.Scheme.FactTables[F_D_REPORT_ARRIVAL_GUID];
            fctReportCharges = this.Scheme.FactTables[F_R_REPORT_CHARGES_GUID];
            fctReportDeficit = this.Scheme.FactTables[F_DP_REPORT_DEFICIT_GUID];
            fctReportSource = this.Scheme.FactTables[F_S_REPORT_SOURCE_GUID];

            this.UsedClassifiers = new IClassifier[] { clsKd, clsKif, clsEkr, clsOutcomes };
            this.UsedFacts = new IFactTable[] { fctReportArrival, fctReportCharges, fctReportDeficit, fctReportSource };
        }

        protected override void UpdateData()
        {
            UpdateDataSet(daKd, dsKd, clsKd);
            UpdateDataSet(daKif, dsKif, clsKif);
            UpdateDataSet(daEkr, dsEkr, clsEkr);
            UpdateDataSet(daOutcomes, dsOutcomes, clsOutcomes);
            UpdateDataSet(daReportArrival, dsReportArrival, fctReportArrival);
            UpdateDataSet(daReportCharges, dsReportCharges, fctReportCharges);
            UpdateDataSet(daReportDeficit, dsReportDeficit, fctReportDeficit);
            UpdateDataSet(daReportSource, dsReportSource, fctReportSource);
        }

        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsReportArrival);
            ClearDataSet(ref dsReportCharges);
            ClearDataSet(ref dsReportDeficit);
            ClearDataSet(ref dsReportSource);
            ClearDataSet(ref dsKd);
            ClearDataSet(ref dsKif);
            ClearDataSet(ref dsEkr);
            ClearDataSet(ref dsOutcomes);
        }

        #endregion Работа с базой и кэшами

        #region Общие методы

        #region Закачка классификаторов

        private string CleanCodeValue(string value)
        {
            return CommonRoutines.TrimLetters(value.Trim().Replace(" ", string.Empty)).Trim();
        }

        private int PumpKd(string code, string name)
        {
            code = CleanCodeValue(code);
            if (code == string.Empty)
                return -1;
            return PumpCachedRow(cacheKd, dsKd.Tables[0], clsKd, code,
                new object[] { "CodeStr", code, "Name", name.Trim() });
        }

        private int PumpOutcomes(string code, string name)
        {
            code = CleanCodeValue(code);
            if (code == string.Empty)
                return -1;
            code = code.Substring(0, 17);
            name = string.Format("{0} ({1})", name, code);
            return PumpCachedRow(cacheOutcomes, dsOutcomes.Tables[0], clsOutcomes, code,
                new object[] { "Code", code, "Name", name });
        }

        private int PumpEkr(string code)
        {
            code = CleanCodeValue(code);
            if (code == string.Empty)
                return -1;
            code = code.Substring(code.Length - 3);
            return PumpCachedRow(cacheEkr, dsEkr.Tables[0], clsEkr, code,
                new object[] { "Code", code });
        }

        private int PumpKif(string code)
        {
            code = CleanCodeValue(code);
            if (code == string.Empty)
                return -1;
            if (code.Length > 20)
                code = code.Substring(0, 20);
            return PumpCachedRow(cacheKif, dsKif.Tables[0], clsKif, code,
                new object[] { "CodeStr", code });
        }

        #endregion Закачка классификаторов

        #region Закачка фактов

        private double CleanFactValue(string value)
        {
            value = CleanCodeValue(value).Replace(".", ",").Trim();
            return Convert.ToDouble(value.PadLeft(1, '0'));
        }

        private void PumpReportArrivalFact(string value, int refDate, int refFx, int refKd, int refRegions, int refMarks)
        {
            double factValue = CleanFactValue(value);
            if (factValue == 0)
                return;

            object[] mapping = new object[] {
                "Value", factValue,
                "RefYearDayUNV", refDate,
                "RefFX", refFx,
                "RefKD", refKd,
                "RefFXMarks", refMarks,
                "RefRegions", refRegions
            };

            PumpRow(dsReportArrival.Tables[0], mapping);
            if (dsReportArrival.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
            {
                UpdateData();
                ClearDataSet(daReportArrival, ref dsReportArrival);
            }
        }

        private void PumpReportChargesFact(string value, int refDate, int refFx, int refOutcomes, int refEkr, int refRegions, int refMarks)
        {
            double factValue = CleanFactValue(value);
            if (factValue == 0)
                return;

            object[] mapping = new object[] {
                "Value", factValue,
                "RefYearDayUNV", refDate,
                "RefFX", refFx,
                "RefRUFK", refOutcomes,
                "RefEKRUFK", refEkr,
                "RefFXMarks", refMarks,
                "RefRegions", refRegions
            };

            PumpRow(dsReportCharges.Tables[0], mapping);
            if (dsReportCharges.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
            {
                UpdateData();
                ClearDataSet(daReportCharges, ref dsReportCharges);
            }
        }

        private void PumpReportDeficitFact(string value, int refDate, int refFx, int refRegions, int refMarks)
        {
            double factValue = CleanFactValue(value);
            if (factValue == 0)
                return;

            object[] mapping = new object[] {
                "Value", factValue,
                "RefYearDayUNV", refDate,
                "RefFX", refFx,
                "RefFXMarks", refMarks,
                "RefRegions", refRegions
            };

            PumpRow(dsReportDeficit.Tables[0], mapping);
            if (dsReportDeficit.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
            {
                UpdateData();
                ClearDataSet(daReportDeficit, ref dsReportDeficit);
            }
        }

        private void PumpReportSourceFact(string value, int refDate, int refFx, int refKif, int refRegions, int refMarks)
        {
            double factValue = CleanFactValue(value);
            if (factValue == 0)
                return;

            object[] mapping = new object[] {
                "Value", factValue,
                "RefYearDayUNV", refDate,
                "RefFX", refFx,
                "RefMarks", refMarks,
                "RefRegions", refRegions,
                "RefKIF", refKif
            };

            PumpRow(dsReportSource.Tables[0], mapping);
            if (dsReportSource.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
            {
                UpdateData();
                ClearDataSet(daReportSource, ref dsReportSource);
            }
        }

        #endregion Закачка фактов

        private int DecrementMonth(int refDate)
        {
            int year = refDate / 10000;
            int month = refDate % 10000 / 100;
            if (month == 1)
            {
                year -= 1;
                month = 12;
            }
            else
            {
                month -= 1;
            }
            return (year * 10000 + month * 100);
        }

        private int GetSectionIndex(string cellValue)
        {
            cellValue = cellValue.Trim().ToUpper();
            if (cellValue.Contains("ПОСТУПЛЕНИЯ") && cellValue.Contains("ВЫБЫТИЯ"))
                return 2;
            else if (cellValue.Contains("ПОСТУПЛЕНИЯ"))
                return 0;
            else if (cellValue.Contains("ВЫБЫТИЯ"))
                return 1;
            return -1;
        }

        private bool IsSectionStart(string cellValue)
        {
            return (cellValue.Trim().ToUpper() == "НАИМЕНОВАНИЕ ПОКАЗАТЕЛЯ");
        }

        private void PumpRarFile(FileInfo file)
        {
            DirectoryInfo tempDir = CommonRoutines.ExtractArchiveFileToTempDir(
                file.FullName, FilesExtractingOption.SingleDirectory, ArchivatorName.Rar);
            try
            {
                ProcessAllFiles(tempDir);
            }
            finally
            {
                CommonRoutines.DeleteDirectory(tempDir);
            }
        }

        private void ProcessAllFiles(DirectoryInfo dir)
        {
            ProcessFilesTemplate(dir, "*.rtf", new ProcessFileDelegate(PumpRtfFile), false, SearchOption.AllDirectories);
            ProcessFilesTemplate(dir, "*.xls", new ProcessFileDelegate(PumpXlsFile), false, SearchOption.AllDirectories);
            ProcessFilesTemplate(dir, "*.xml", new ProcessFileDelegate(PumpXmlFile), false, SearchOption.AllDirectories);
            ProcessFilesTemplate(dir, "*.rar", new ProcessFileDelegate(PumpRarFile), false, SearchOption.AllDirectories);
        }

        #endregion Общие методы

        #region Работа с Rtf

        #region Обычный формат

        private void PumpRtfRow(string[] reportRow, int refDate)
        {
            switch (sectionIndex)
            {
                case 0:
                    int refKd = PumpKd(reportRow[2], reportRow[0]);
                    if (refKd == -1)
                        return;
                    PumpReportArrivalFact(reportRow[3], refDate, 15, refKd, nullRegions, 1);
                    PumpReportArrivalFact(reportRow[4], refDate, 15, refKd, nullRegions, 2);
                    PumpReportArrivalFact(reportRow[5], refDate, 15, refKd, nullRegions, 3);
                    break;

                case 1:
                    if (reportRow[1].Trim() == "450")
                    {
                        PumpReportDeficitFact(reportRow[3], refDate, 15, nullRegions, 1);
                        PumpReportDeficitFact(reportRow[4], refDate, 15, nullRegions, 2);
                        PumpReportDeficitFact(reportRow[5], refDate, 15, nullRegions, 3);
                    }
                    else
                    {
                        int refOutcomes = PumpOutcomes(reportRow[2], reportRow[0]);
                        int refEkr = PumpEkr(reportRow[2]);
                        if ((refOutcomes == -1) || (refEkr == -1))
                            return;
                        PumpReportChargesFact(reportRow[3], refDate, 15, refOutcomes, refEkr, nullRegions, 1);
                        PumpReportChargesFact(reportRow[4], refDate, 15, refOutcomes, refEkr, nullRegions, 2);
                        PumpReportChargesFact(reportRow[5], refDate, 15, refOutcomes, refEkr, nullRegions, 3);
                    }
                    break;

                case 2:
                    int refKif = PumpKif(reportRow[2]);
                    if (refKif == -1)
                        return;
                    PumpReportSourceFact(reportRow[3], refDate, 15, refKif, nullRegions, 1);
                    PumpReportSourceFact(reportRow[4], refDate, 15, refKif, nullRegions, 2);
                    PumpReportSourceFact(reportRow[5], refDate, 15, refKif, nullRegions, 3);
                    break;
            }
        }

        private int GetRtfDate(string[] reportRow)
        {
            for (int i = 0; i < reportRow.Length; i++)
                if (reportRow[i].ToUpper().Contains("ДАТА"))
                    return DecrementMonth(CommonRoutines.ShortDateToNewDate(reportRow[i + 1]));
            return -1;
        }

        private char[] RTF_CELL_DELIMITER = new char[] { '\t' };
        private void PumpRtfDoc(string[] rtfDoc, string fileName)
        {
            bool toPump = false;
            sectionIndex = -1;
            int refDate = -1;
            int rowsCount = rtfDoc.GetLength(0);
            string dataSourcePath = GetShortSourcePathBySourceID(this.SourceID);
            for (int curRow = 0; curRow < rowsCount; curRow++)
                try
                {
                    SetProgress(rowsCount, curRow + 1,
                        string.Format("Обработка файла {0}\\{1}...", dataSourcePath, fileName),
                        string.Format("Строка {0} из {1}", curRow + 1, rowsCount));

                    if (rtfDoc[curRow].ToUpper().Contains("ДАТА"))
                    {
                        refDate = GetRtfDate(rtfDoc[curRow].Split(RTF_CELL_DELIMITER, StringSplitOptions.RemoveEmptyEntries));
                    }

                    string[] reportRow = rtfDoc[curRow].Split(RTF_CELL_DELIMITER);
                    if (reportRow[0].Trim() == string.Empty)
                        continue;

                    if (IsSectionStart(reportRow[0]))
                    {
                        sectionIndex = GetSectionIndex(rtfDoc[curRow - 1]);
                        if (sectionIndex > -1)
                            toPump = true;
                        curRow++;
                    }

                    if (toPump && (reportRow.Length >= 7))
                    {
                        PumpRtfRow(reportRow, refDate);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format(
                        "При обработке строки {0} файла '{1}' возникла ошибка ({2})",
                        curRow + 1, fileName, ex.Message), ex);
                }
        }

        #endregion Обычный формат

        #region Формат ФК

        private void PumpRtfRowFK(string[] reportRow, int refDate)
        {
            switch (reportRow[0].Trim().ToUpper())
            {
                case "RD1":
                    int refKd = PumpKd(reportRow[2], constDefaultClsName);
                    if (refKd == -1)
                        return;
                    PumpReportArrivalFact(reportRow[3], refDate, 15, refKd, nullRegions, 1);
                    PumpReportArrivalFact(reportRow[4], refDate, 15, refKd, nullRegions, 2);
                    PumpReportArrivalFact(reportRow[5], refDate, 15, refKd, nullRegions, 3);
                    break;
                case "RD2":
                    if (reportRow[1].Trim() == "450")
                    {
                        PumpReportDeficitFact(reportRow[3], refDate, 15, nullRegions, 1);
                        PumpReportDeficitFact(reportRow[4], refDate, 15, nullRegions, 2);
                        PumpReportDeficitFact(reportRow[5], refDate, 15, nullRegions, 3);
                    }
                    else
                    {
                        int refOutcomes = PumpOutcomes(reportRow[2], constDefaultClsName);
                        int refEkr = PumpEkr(reportRow[2]);
                        if ((refOutcomes == -1) || (refEkr == -1))
                            return;
                        PumpReportChargesFact(reportRow[3], refDate, 15, refOutcomes, refEkr, nullRegions, 1);
                        PumpReportChargesFact(reportRow[4], refDate, 15, refOutcomes, refEkr, nullRegions, 2);
                        PumpReportChargesFact(reportRow[5], refDate, 15, refOutcomes, refEkr, nullRegions, 3);
                    }
                    break;
                case "RD3":
                    int refKif = PumpKif(reportRow[2]);
                    if (refKif == -1)
                        return;
                    PumpReportSourceFact(reportRow[3], refDate, 15, refKif, nullRegions, 1);
                    PumpReportSourceFact(reportRow[4], refDate, 15, refKif, nullRegions, 2);
                    PumpReportSourceFact(reportRow[5], refDate, 15, refKif, nullRegions, 3);
                    break;
            }
        }

        private int GetRtfDateFK(string[] reportRow)
        {
            string date = reportRow[6].Trim();
            return DecrementMonth(CommonRoutines.ShortDateToNewDate(date));
        }

        // закачка Rtf-документа в формате ФК
        private char[] RTF_CELL_DELIMITER_FK = new char[] { '|' };
        private void PumpRtfDocFK(string[] rtfDoc, string fileName)
        {
            int refDate = -1;
            int rowsCount = rtfDoc.GetLength(0);
            string dataSourcePath = GetShortSourcePathBySourceID(this.SourceID);
            for (int curRow = 0; curRow < rowsCount; curRow++)
                try
                {
                    SetProgress(rowsCount, curRow + 1,
                        string.Format("Обработка файла {0}\\{1}...", dataSourcePath, fileName),
                        string.Format("Строка {0} из {1}", curRow + 1, rowsCount));

                    string[] reportRow = rtfDoc[curRow].Split(RTF_CELL_DELIMITER_FK);

                    if (reportRow[0].Trim().ToUpper() == "PW")
                    {
                        refDate = GetRtfDateFK(reportRow);
                        continue;
                    }

                    if (reportRow[0].Trim().ToUpper().StartsWith("RD"))
                    {
                        PumpRtfRowFK(reportRow, refDate);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format(
                        "При обработке строки {0} файла '{1}' возникла ошибка ({2})",
                        curRow + 1, fileName, ex.Message), ex);
                }
        }

        #endregion Формат ФК

        private void PumpRtfFile(FileInfo file)
        {
            WriteToTrace("Открытие документа: " + file.Name, TraceMessageKind.Information);
            RtfHelper rtfDoc = new RtfHelper(file.FullName);
            string[] report = rtfDoc.GetPlainText().Split(new char[] { '\n' });
            if (report[0].Trim().ToUpper().StartsWith("FK"))
                PumpRtfDocFK(report, file.Name);
            else
                PumpRtfDoc(report, file.Name);
        }

        #endregion Работа с Rtf

        #region Работа с Xls

        private void PumpXlsRow(Dictionary<string, string> dataRow, int refDate)
        {
            switch (sectionIndex)
            {
                case 0:
                    int refKd = PumpKd(dataRow["ClsCode"], dataRow["Name"]);
                    if (refKd == -1)
                        return;
                    PumpReportArrivalFact(dataRow["Sum1"], refDate, 15, refKd, nullRegions, 1);
                    PumpReportArrivalFact(dataRow["Sum2"], refDate, 15, refKd, nullRegions, 2);
                    PumpReportArrivalFact(dataRow["Sum3"], refDate, 15, refKd, nullRegions, 3);
                    break;
                case 1:
                    if (dataRow["StrCode"].Trim() == "450")
                    {
                        PumpReportDeficitFact(dataRow["Sum1"], refDate, 15, nullRegions, 1);
                        PumpReportDeficitFact(dataRow["Sum2"], refDate, 15, nullRegions, 2);
                        PumpReportDeficitFact(dataRow["Sum3"], refDate, 15, nullRegions, 3);
                    }
                    else
                    {
                        int refOutcomes = PumpOutcomes(dataRow["ClsCode"], dataRow["Name"]);
                        int refEkr = PumpEkr(dataRow["ClsCode"]);
                        if ((refOutcomes == -1) || (refEkr == -1))
                            return;
                        PumpReportChargesFact(dataRow["Sum1"], refDate, 15, refOutcomes, refEkr, nullRegions, 1);
                        PumpReportChargesFact(dataRow["Sum2"], refDate, 15, refOutcomes, refEkr, nullRegions, 2);
                        PumpReportChargesFact(dataRow["Sum3"], refDate, 15, refOutcomes, refEkr, nullRegions, 3);
                    }
                    break;
                case 2:
                    int refKif = PumpKif(dataRow["ClsCode"]);
                    if (refKif == -1)
                        return;
                    PumpReportSourceFact(dataRow["Sum1"], refDate, 15, refKif, nullRegions, 1);
                    PumpReportSourceFact(dataRow["Sum2"], refDate, 15, refKif, nullRegions, 2);
                    PumpReportSourceFact(dataRow["Sum3"], refDate, 15, refKif, nullRegions, 3);
                    break;
            }
        }

        private object[] XLS_MAPPING_MANY_SHEETS = new object[] {
            "Name", 1, "StrCode", 2, "ClsCode", 4, "Sum1", 5, "Sum2", 6, "Sum3", 7 };
        private object[] XLS_MAPPING_FIRST_ONE_SHEET = new object[] {
            "Name", 1, "StrCode", 2, "ClsCode", 3, "Sum1", 4, "Sum2", 5, "Sum3", 6 };
        private object[] XLS_MAPPING_SECOND_ONE_SHEET = new object[] {
            "Name", 1, "StrCode", 10, "ClsCode", 12, "Sum1", 18, "Sum2", 22, "Sum3", 26 };
        private object[] GetXlsMapping()
        {
            switch (xlsReportType)
            {
                case XlsReportType.ManySheets:
                    return XLS_MAPPING_MANY_SHEETS;
                case XlsReportType.FirstOneSheet:
                    return XLS_MAPPING_FIRST_ONE_SHEET;
                case XlsReportType.SecondOneSheet:
                    return XLS_MAPPING_SECOND_ONE_SHEET;
            }
            return null;
        }

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

        private string GetClsName(ExcelHelper excelDoc, ref int curRow)
        {
            List<string> clsName = new List<string>();
            for (; ; curRow++)
            {
                clsName.Add(excelDoc.GetValue(curRow, 1));
                if (excelDoc.GetBorderStyle(curRow, 1, ExcelBorderStyles.EdgeBottom) == ExcelLineStyles.Continuous)
                    break;
            }
            return string.Join(string.Empty, clsName.ToArray());
        }

        private void PumpXlsDoc(ExcelHelper excelDoc, string fileName, int refDate)
        {
            bool toPump = false;
            int rowsCount = excelDoc.GetRowsCount();
            object[] mapping = GetXlsMapping();
            string dataSourcePath = GetShortSourcePathBySourceID(this.SourceID);
            for (int curRow = 1; curRow <= rowsCount; curRow++)
                try
                {
                    SetProgress(rowsCount, curRow + 1,
                        string.Format("Обработка файла {0}\\{1}...", dataSourcePath, fileName),
                        string.Format("Строка {0} из {1}", curRow, rowsCount));

                    string cellValue = excelDoc.GetValue(curRow, 1).Trim();
                    if (cellValue == string.Empty)
                        continue;

                    if (cellValue.ToUpper().Contains("РУКОВОДИТЕЛЬ"))
                    {
                        toPump = false;
                        continue;
                    }

                    if (xlsReportType != XlsReportType.ManySheets)
                    {
                        int index = GetSectionIndex(cellValue);
                        if (index > -1)
                            sectionIndex = index;
                    }

                    if ((cellValue == "1") && (sectionIndex != -1))
                    {
                        toPump = true;
                        continue;
                    }

                    if (toPump)
                    {
                        Dictionary<string, string> dataRow = GetXlsDataRow(excelDoc, curRow, mapping);
                        if (xlsReportType == XlsReportType.SecondOneSheet)
                            dataRow["Name"] = GetClsName(excelDoc, ref curRow);
                        PumpXlsRow(dataRow, refDate);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format(
                        "При обработке строки {0} листа '{1}' файла '{2}' возникла ошибка ({3})",
                        curRow, excelDoc.GetWorksheetName(), fileName, ex.Message), ex);
                }
        }

        private XlsReportType GetXlsReportType(ExcelHelper excelDoc)
        {
            if (excelDoc.GetWorksheetsCount() == 3)
                return XlsReportType.ManySheets;
            if (excelDoc.GetValue("G2").Trim().ToUpper().Contains("КОДЫ"))
                return XlsReportType.FirstOneSheet;
            return XlsReportType.SecondOneSheet;
        }

        private int GetXlsDate(ExcelHelper excelDoc)
        {
            string cellValue = string.Empty;
            switch (xlsReportType)
            {
                case XlsReportType.ManySheets:
                    cellValue = excelDoc.GetValue("D4").Trim();
                    break;
                case XlsReportType.FirstOneSheet:
                    cellValue = excelDoc.GetValue("A3").Trim();
                    break;
                case XlsReportType.SecondOneSheet:
                    cellValue = excelDoc.GetValue("J5").Trim();
                    break;
            }
            cellValue = CommonRoutines.LongDateToNewDate(CommonRoutines.TrimLetters(cellValue));
            return DecrementMonth(Convert.ToInt32(cellValue));
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

                xlsReportType = GetXlsReportType(excelDoc);
                int refDate = GetXlsDate(excelDoc);
                int wsCount = excelDoc.GetWorksheetsCount();
                sectionIndex = -1;
                for (int index = 1; index <= wsCount; index++)
                {
                    excelDoc.SetWorksheet(index);
                    if (xlsReportType == XlsReportType.ManySheets)
                        sectionIndex = index - 1;
                    else if (index > 1)
                        break;
                    PumpXlsDoc(excelDoc, file.Name, refDate);
                }
            }
            finally
            {
                if (excelDoc != null)
                    excelDoc.CloseDocument();
            }
        }

        #endregion Работа с Xls

        #region Работа с Xml

        private void PumpXmlRow(XmlNode dataRow, string formCode, int refDate)
        {
            switch (formCode)
            {
                case "65101":
                    string codeKd = string.Concat(dataRow.ParentNode.Attributes["Адм"].Value, dataRow.Attributes["ВД"].Value);
                    int refKd = PumpKd(codeKd, constDefaultClsName);
                    if (refKd == -1)
                        return;
                    foreach (XmlNode row in dataRow.SelectNodes("Px"))
                    {
                        int num = Convert.ToInt32(row.Attributes["Num"].Value.Trim());
                        if ((num >= 1) && (num <= 3))
                            PumpReportArrivalFact(row.Attributes["Value"].Value, refDate, 15, refKd, nullRegions, num);
                    }
                    break;

                case "65103":
                    string codeKif = string.Concat(dataRow.ParentNode.Attributes["Адм"].Value, dataRow.Attributes["КИВФ"].Value);
                    int refKif = PumpKif(codeKif);
                    if (refKif == -1)
                        return;
                    foreach (XmlNode row in dataRow.SelectNodes("Px"))
                    {
                        int num = Convert.ToInt32(row.Attributes["Num"].Value.Trim());
                        if ((num >= 1) && (num <= 3))
                            PumpReportSourceFact(row.Attributes["Value"].Value, refDate, 15, refKif, nullRegions, num);
                    }
                    break;

                case "65112":
                    string codeOutcomes = string.Concat(
                        dataRow.ParentNode.Attributes["Адм"].Value,
                        dataRow.ParentNode.Attributes["РзПр"].Value,
                        dataRow.ParentNode.Attributes["ЦСР"].Value,
                        dataRow.ParentNode.Attributes["ВР"].Value);
                    int refOutcomes = PumpOutcomes(codeOutcomes, constDefaultClsName);
                    int refEkr = PumpEkr(dataRow.Attributes["ЭКР"].Value.Trim());
                    if ((refOutcomes == -1) || (refEkr == -1))
                        return;
                    foreach (XmlNode row in dataRow.SelectNodes("Px"))
                    {
                        int num = Convert.ToInt32(row.Attributes["Num"].Value.Trim());
                        if ((num >= 1) && (num <= 3))
                            PumpReportChargesFact(row.Attributes["Value"].Value, refDate, 15, refOutcomes, refEkr, nullRegions, num);
                    }
                    break;

                case "65122":
                    foreach (XmlNode row in dataRow.SelectNodes("Px"))
                    {
                        int num = Convert.ToInt32(row.Attributes["Num"].Value.Trim());
                        if ((num >= 1) && (num <= 3))
                            PumpReportDeficitFact(row.Attributes["Value"].Value, refDate, 15, nullRegions, num);
                    }
                    break;
            }
        }

        private void PumpXmlForms(XmlNodeList forms, int refDate)
        {
            foreach (XmlNode form in forms)
            {
                string formCode = form.Attributes["Code"].Value.Trim();
                XmlNodeList dataRows = form.SelectNodes("Document/Data");
                foreach (XmlNode dataRow in dataRows)
                {
                    PumpXmlRow(dataRow, formCode, refDate);
                }
            }
        }

        private int GetXmlDate(XmlNode node)
        {
            string date = node.Attributes["Date"].Value.Trim();
            return DecrementMonth(Convert.ToInt32(date.Replace("-", string.Empty)));
        }

        private void PumpXmlFile(FileInfo file)
        {
            WriteToTrace("Открытие документа: " + file.Name, TraceMessageKind.Information);
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(file.FullName);
            try
            {
                int refDate = GetXmlDate(xmlDoc.SelectSingleNode("RootXml/Report/Period"));
                PumpXmlForms(xmlDoc.SelectNodes("RootXml/Report/Period/Source/Form"), refDate);
            }
            finally
            {
                XmlHelper.ClearDomDocument(ref xmlDoc);
            }
        }

        #endregion Работа с Xml

        #region Перекрытые методы закачки

        protected override void ProcessFiles(DirectoryInfo dir)
        {
            ProcessAllFiles(dir);
            UpdateData();
        }

        protected override void DirectPumpData()
        {
            PumpDataYMTemplate();
        }

        #endregion Перекрытые методы закачки

        #endregion Закачка данных

    }

}
