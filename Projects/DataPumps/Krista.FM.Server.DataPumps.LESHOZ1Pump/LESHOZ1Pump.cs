using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.LESHOZ1Pump
{

    // ЛЕСХОЗ - 0001 - Сводная ведомость лесных пожаров
    public class LESHOZ1PumpModule : CorrectedPumpModuleBase
    {

        #region Коснтанты

        private const string LHZON_DOC_SEARCH_PATTERN = "*LHZon*.doc";
        private const string LHZON_RTF_SEARCH_PATTERN = "*LHZon*.rtf";

        #endregion Константы

        #region Поля

        #region Классификаторы

        // Лесхоз.Участковое лесничество (d_Forest_DtForestry)
        private IDbDataAdapter daDtForestry;
        private DataSet dsDtForestry;
        private IClassifier clsDtForestry;
        private Dictionary<string, int> cacheDtForestry = null;
        private int nullDtForestry = -1;

        #endregion Классификаторы

        #region Факты

        // Лесхоз.ЛЕСХОЗ_Сводная ведомость (f_Forest_Consolidat)
        private IDbDataAdapter daConsolidat;
        private DataSet dsConsolidat;
        private IFactTable fctConsolidat;

        #endregion Факты

        private bool toPumpDataForLastYears = false;

        private List<int> deletedDateList = null;
        private string forestryName;
        private int summaryYear;

        private bool hasPumpedFiles = false;

        #endregion Поля

        #region Закачка данных

        #region Работа с базой и кэшами

        protected override void QueryData()
        {
            InitDataSet(ref daDtForestry, ref dsDtForestry, clsDtForestry, string.Empty);
            InitFactDataSet(ref daConsolidat, ref dsConsolidat, fctConsolidat);
            FillCaches();
        }

        private void FillCaches()
        {
            FillRowsCache(ref cacheDtForestry, dsDtForestry.Tables[0], "Name", "Id");
        }

        protected override void UpdateData()
        {
            UpdateDataSet(daConsolidat, dsConsolidat, fctConsolidat);
        }

        private const string D_FOREST_DT_FORESTRY_GUID = "c608a32a-9fa6-45ed-b82f-e9d066df0ade";
        private const string F_FOREST_CONSOLIDAT_GUID = "5b8bf598-1ce0-43bb-8842-ffac249b6649";
        protected override void InitDBObjects()
        {
            clsDtForestry = this.Scheme.Classifiers[D_FOREST_DT_FORESTRY_GUID];
            this.UsedClassifiers = new IClassifier[] { };
            this.UsedFacts = new IFactTable[] {
                fctConsolidat = this.Scheme.FactTables[F_FOREST_CONSOLIDAT_GUID]
            };
        }

        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsDtForestry);
            ClearDataSet(ref dsConsolidat);
        }

        #endregion Работа с базой и кэшами

        #region Работа с Rtf

        private void MoveToArchive(DirectoryInfo dir)
        {
            if (Directory.GetFiles(dir.FullName, "*.*", SearchOption.AllDirectories).GetLength(0) <= 0)
                return;
            bool buffer = this.UseArchive;
            this.UseArchive = true;
            this.MoveFilesToArchive(dir);
            this.UseArchive = buffer;
        }

        private void RenameFile(string filename)
        {
            string newFilename = string.Format("{0}_{1:D4}_{2:D2}_{3:D2}.rtf",
                filename.Substring(0, filename.Length - 4),
                DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day);
            if (File.Exists(newFilename))
            {
                File.Delete(newFilename);
            }
            File.Move(filename, newFilename);
        }

        private decimal CleanFactValue(string value)
        {
            value = value.Trim().Replace("-", string.Empty).Replace(" ", string.Empty).Replace('.', ',');
            return Convert.ToDecimal(value.PadLeft(1, '0'));
        }

        private int GetRefDtForestry(string name)
        {
            if (cacheDtForestry.ContainsKey(name))
                return cacheDtForestry[name];
            return nullDtForestry;
        }

        private void PumpTerritoryRow(string[] dataRow, int refDate)
        {
            forestryName = dataRow[1].Trim();

            object[] mapping = new object[] {
                "RefUNV", refDate,
                "RefDtForestry", GetRefDtForestry(forestryName.Trim()),
                "yearNumber", CleanFactValue(dataRow[2]),
                "coveredForest", CleanFactValue(dataRow[3]),
                "coveredNonForest", CleanFactValue(dataRow[4]),
                "actsNumber", CleanFactValue(dataRow[5]),
                "actsForest", CleanFactValue(dataRow[6]),
                "localizedNumber", CleanFactValue(dataRow[7]),
                "localizedForest", CleanFactValue(dataRow[8]),
                "eliminatedNumber", CleanFactValue(dataRow[9]),
                "eliminatedForest", CleanFactValue(dataRow[10]),
                "workVS", CleanFactValue(dataRow[11]),
                "workPHS", CleanFactValue(dataRow[12]),
                "workAPS", CleanFactValue(dataRow[13]),
                "workB", CleanFactValue(dataRow[14]),
                "workT", CleanFactValue(dataRow[15]),
                "workA", CleanFactValue(dataRow[16]),
                "causeLZ", CleanFactValue(dataRow[17]),
                "causePal", CleanFactValue(dataRow[18]),
                "causeGr", CleanFactValue(dataRow[19]),
                "causeMn", CleanFactValue(dataRow[20]),
                "causePor", CleanFactValue(dataRow[21]),
                "causeEx", CleanFactValue(dataRow[22]),
                "causeMps", CleanFactValue(dataRow[23]),
                "causeNo", CleanFactValue(dataRow[24]),
                "causePL", CleanFactValue(dataRow[25]),
                "aviaNumber", CleanFactValue(dataRow[26]),
                "aviaForest", CleanFactValue(dataRow[27]),
                "groundNumber", CleanFactValue(dataRow[28]),
                "groundForest", CleanFactValue(dataRow[29]),
                "spaceNumber", CleanFactValue(dataRow[30]),
                "spaceForest", CleanFactValue(dataRow[31]),
            };

            PumpRow(dsConsolidat.Tables[0], mapping);
        }

        private void PumpSummaryRow(string[] dataRow, int refDate)
        {
            summaryYear = Convert.ToInt32(regExSummary.Match(dataRow[0]).Groups[1].Value);
            refDate = summaryYear * 10000 + refDate % 10000;

            object[] mapping = new object[] {
                "RefUNV", refDate,
                "RefDtForestry", nullDtForestry,
                "yearNumber", CleanFactValue(dataRow[1]),
                "coveredForest", CleanFactValue(dataRow[2]),
                "coveredNonForest", CleanFactValue(dataRow[3]),
            };

            PumpRow(dsConsolidat.Tables[0], mapping);
        }

        private int GetReportDate(string cellValue)
        {
            string date = CommonRoutines.TrimLetters(cellValue.Trim());
            int refDate = CommonRoutines.ShortDateToNewDate(date);
            if (!deletedDateList.Contains(refDate))
            {
                switch (this.ServerDBMSName)
                {
                    case DBMSName.Oracle:
                        DeleteData(
                            string.Format("mod(RefUNV, 10000) = {0}", refDate % 10000),
                            string.Format("Дата отчета: {0}.", refDate));
                        break;
                    case DBMSName.SQLServer:
                        DeleteData(
                            string.Format("(RefUNV % 10000) = {0}", refDate % 10000),
                            string.Format("Дата отчета: {0}.", refDate));
                        break;
                }
                
                deletedDateList.Add(refDate);
            }
            return refDate;
        }

        private bool IsTerritoryRow(string cellValue)
        {
            try
            {
                return (Convert.ToInt32(cellValue.Trim()) > 0);
            }
            catch
            {
                return false;
            }
        }

        // регулярное выражение для определения строк по итогам
        private Regex regExSummary = new Regex(@"В ([0-9]{4}) году на эту дату");
        private bool IsSummaryRow(string cellValue)
        {
            return (toPumpDataForLastYears && regExSummary.IsMatch(cellValue.Trim()));
        }

        private char[] ROWS_SPLITTER = new char[] { '\n' };
        private char[] CELLS_SPLITTER = new char[] { '\t' };
        private const string DATE_ROW_MARK = "ПО СОСТОЯНИЮ";
        private void PumpRtfFile(FileInfo file)
        {
            hasPumpedFiles = true;
            RtfHelper rtfDoc = new RtfHelper(file.FullName);
            string[] reportData = rtfDoc.GetPlainText().Split(ROWS_SPLITTER, StringSplitOptions.None);

            int refDate = -1;
            bool toPumpForestry = false;
            bool toPumpSummary = false;
            int rowsCount = reportData.GetLength(0);
            for (int curRow = 0; curRow < rowsCount; curRow++)
                try
                {
                    string[] dataRow = reportData[curRow].Split(CELLS_SPLITTER, StringSplitOptions.None);
                    if (dataRow[0].Trim().ToUpper().StartsWith(DATE_ROW_MARK))
                    {
                        refDate = GetReportDate(dataRow[0]);
                        continue;
                    }

                    if (IsTerritoryRow(dataRow[0]))
                    {
                        toPumpForestry = true;
                        PumpTerritoryRow(dataRow, refDate);
                        toPumpForestry = false;
                    }

                    if (IsSummaryRow(dataRow[0]))
                    {
                        toPumpSummary = true;
                        PumpSummaryRow(dataRow, refDate);
                        toPumpSummary = false;
                    }
                }
                #region catch
                catch (Exception ex)
                {
                    if (toPumpForestry)
                    {
                        string message = string.Format("Территориальный отдел '{0}' обработан с ошибкой.", forestryName);
                        WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeError, message);
                    }
                    else if (toPumpSummary)
                    {
                        string message = string.Format("Итоги за {0} год обработаны с ошибкой.", summaryYear);
                        WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeError, message);
                    }
                    else
                    {
                        throw new Exception(string.Format("При обработке строки {0} возникла ошибка ({1})", curRow, ex.Message), ex);
                    }
                }
                #endregion

            RenameFile(file.FullName);
        }

        #endregion Работа с Rtf

        #region Работа с Word

        private void PumpWordFile(FileInfo file)
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, string.Format(
                "Файл '{0}' будет сконвертирован в RTF-формат", file.Name));
            WordHelper wordDoc = new WordHelper();
            try
            {
                wordDoc.Visible = false;
                wordDoc.OpenDocument(file.FullName);
                string rtfFilename = file.FullName.Substring(0, file.FullName.Length - 4) + ".rtf";
                wordDoc.SaveAs(rtfFilename, WdSaveFormat.wdFormatRTF);
            }
            finally
            {
                if (wordDoc != null)
                    wordDoc.CloseDocument();
                if (File.Exists(file.FullName))
                    File.Delete(file.FullName);
            }
        }

        #endregion Работа с Word

        #region Перекрытые методы закачки

        // файлы с отчётами ежедневно присылают на почту
        // админы выкладывают их в папку "\\mailsrv\Fmdata\fire\", а в нашу папку с источниками перемещать не хотят
        // приходится сначала забирать файлы с сервера, а уже потом качать их
        private const string MAIL_SERVER_PATH = @"\\mailsrv\Fmdata\fire";
        private void GetFilesFromMailServer(string destDir)
        {
            // если в источнике уже есть файлы, то просто закачиваем их
            if ((Directory.GetFiles(destDir, "*.doc").GetLength(0) > 0) ||
                (Directory.GetFiles(destDir, "*.rtf").GetLength(0) > 0))
                return;
            string[] files = Directory.GetFiles(MAIL_SERVER_PATH, LHZON_DOC_SEARCH_PATTERN);
            files = (string[])CommonRoutines.ConcatArrays(files, Directory.GetFiles(MAIL_SERVER_PATH, LHZON_RTF_SEARCH_PATTERN));
            foreach (string sourceFile in files)
            {
                string destFile = sourceFile.Replace(MAIL_SERVER_PATH, destDir);
                File.Move(sourceFile, destFile);
            }
        }

        protected override void ProcessFiles(DirectoryInfo dir)
        {
            deletedDateList = new List<int>();
            try
            {
                GetFilesFromMailServer(dir.FullName);
                toPumpDataForLastYears = Convert.ToBoolean(
                    GetParamValueByName(this.PumpRegistryElement.ProgramConfig, "ucbPumpDataForLastYears", "False"));
                ProcessFilesTemplate(dir, LHZON_DOC_SEARCH_PATTERN, new ProcessFileDelegate(PumpWordFile), false);
                ProcessFilesTemplate(dir, LHZON_RTF_SEARCH_PATTERN, new ProcessFileDelegate(PumpRtfFile), false);
                MoveToArchive(dir);
            }
            finally
            {
                deletedDateList.Clear();
            }
        }

        protected override void DirectPumpData()
        {
            this.CheckSourceDirToEmpty = false;
            PumpDataYVTemplate();
        }

        #endregion Перекрытые методы закачки

        #endregion Закачка данных

        #region Расчет кубов

        protected override void DirectProcessCube()
        {
            if (hasPumpedFiles)
            {
                base.DirectProcessCube();
            }
        }

        #endregion Расчет кубов

    }

}
