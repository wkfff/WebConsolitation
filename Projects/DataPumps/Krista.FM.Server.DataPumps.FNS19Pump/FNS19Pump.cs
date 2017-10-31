using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Text.RegularExpressions;
using Krista.FM.Common;
using Krista.FM.Server.Common;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.FNS19Pump
{
    public class FNS19PumpModule : CorrectedPumpModuleBase
    {

        #region Поля

        #region Классификаторы

        // Показатели.ФНС 5 ВН (d.Marks.FNS5VN)
        private IDbDataAdapter daMarks;
        private DataSet dsMarks;
        private IClassifier clsMarks;
        private Dictionary<string, int> cacheMarks = null;
        private int nullMarks;
        // ЕдИзмер.ОКЕИ (d.Units.OKEI)
        private IDbDataAdapter daUnits;
        private DataSet dsUnits;
        private IClassifier clsUnits;
        private Dictionary<string, int> cacheUnits = null;
        private int nullUnits = -1;

        #endregion Классификаторы

        #region Факты

        // Доходы.ФНС 5 ВН Сводный (f.D.FNS5VNTotal)
        private IDbDataAdapter daIncomesTotal;
        private DataSet dsIncomesTotal;
        private IFactTable fctIncomesTotal;
        // Доходы.ФНС 5 ВН Районы (f.D.FNS5VNRegions)
        private IDbDataAdapter daIncomesRegion;
        private DataSet dsIncomesRegion;
        private IFactTable fctIncomesRegion;

        #endregion Факты

        private ReportType reportType;
        // итоговая сумма
        private decimal[] totalSums = new decimal[1];
        private int tableNumber;
        private int year;
        private int month;

        // коэффициент перевода тысяч в рубли
        private decimal sumMultiplier = 1;

        #endregion Поля

        #region Структуры, перечисления

        // Тип отчета
        private enum ReportType
        {
            Svod,
            Str,
            Region
        }

        #endregion Структуры, перечисления

        #region Закачка данных

        #region Работа с базой и кэшами

        protected override void QueryData()
        {
            InitDataSet(ref daUnits, ref dsUnits, clsUnits, true, string.Empty, string.Empty);
            InitClsDataSet(ref daMarks, ref dsMarks, clsMarks);
            nullMarks = clsMarks.UpdateFixedRows(this.DB, this.SourceID);
            InitFactDataSet(ref daIncomesTotal, ref dsIncomesTotal, fctIncomesTotal);
            InitFactDataSet(ref daIncomesRegion, ref dsIncomesRegion, fctIncomesRegion);
            FillCaches();
        }

        private void FillCaches()
        {
            FillRowsCache(ref cacheMarks, dsMarks.Tables[0], "CODE", "ID");
            FillRowsCache(ref cacheUnits, dsUnits.Tables[0], "Name", "ID");
        }

        protected override void UpdateData()
        {
            UpdateDataSet(daMarks, dsMarks, clsMarks);
            UpdateDataSet(daIncomesTotal, dsIncomesTotal, fctIncomesTotal);
            UpdateDataSet(daIncomesRegion, dsIncomesRegion, fctIncomesRegion);
        }

        private const string D_MARKS_FNS_5VN_GUID = "006d4a83-3492-4486-ab45-a57c88eddc59";
        private const string D_UNITS_OKEI_GUID = "7ef0edfd-9461-4333-8420-ccb102051826";
        private const string F_D_FNS_5VN_TOTAL_GUID = "3677c88d-4ba6-4cde-b70f-ef9a621392ee";
        private const string F_D_FNS_5VN_REGIONS_GUID = "a0d4767f-b36d-45e0-b2f1-691e2fa61bde";
        protected override void InitDBObjects()
        {
            clsUnits = this.Scheme.Classifiers[D_UNITS_OKEI_GUID];
            this.UsedClassifiers = new IClassifier[] {
                clsMarks = this.Scheme.Classifiers[D_MARKS_FNS_5VN_GUID] };
            this.UsedFacts = new IFactTable[] { 
                fctIncomesTotal = this.Scheme.FactTables[F_D_FNS_5VN_TOTAL_GUID], 
                fctIncomesRegion = this.Scheme.FactTables[F_D_FNS_5VN_REGIONS_GUID] };
        }

        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsIncomesTotal);
            ClearDataSet(ref dsIncomesRegion);
            ClearDataSet(ref dsMarks);
            ClearDataSet(ref dsUnits);
        }

        #endregion Работа с базой и кэшами

        #region Общие функции закачки

        // обнуление итоговой суммы
        private void SetNullTotalSum()
        {
            for (int i = 0; i < totalSums.Length; i++)
                totalSums[i] = 0.0M;
        }

        private void CheckTotalSum(decimal controlSum, decimal totalSum, string column)
        {
            if (totalSum != controlSum)
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, string.Format(
                    "Контрольная сумма {0:F} не сходится с итоговой {1:F} по столбцу {2}", controlSum, totalSum, column));
        }

        private DataTable GetFactTable()
        {
            if (reportType == ReportType.Svod)
                return dsIncomesTotal.Tables[0];
            else
                return dsIncomesRegion.Tables[0];
        }

        private int GetReportDate()
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, "Дата будет определена параметрами источника");
            return (this.DataSource.Year * 10000 + 1);
        }

        // наименования служебных каталогов
        private const string constSvodDirName = "Сводный";
        private const string constStrDirName = "Строки";
        private const string constRegDirName = "Районы";
        private void CheckDirectories(DirectoryInfo dir)
        {
            DirectoryInfo[] svod = dir.GetDirectories(constSvodDirName, SearchOption.TopDirectoryOnly);
            DirectoryInfo[] str = dir.GetDirectories(constStrDirName, SearchOption.TopDirectoryOnly);
            DirectoryInfo[] reg = dir.GetDirectories(constRegDirName, SearchOption.TopDirectoryOnly);
            // Каталог "Сводный" должен присутствовать
            if (svod.GetLength(0) == 0)
            {
                dir.CreateSubdirectory(constSvodDirName);
                if (this.DataSource.Year == 2007)
                    throw new Exception(string.Format("Отсутствует каталог \"{0}\"", constSvodDirName));
            }
            if (str.GetLength(0) == 0)
                dir.CreateSubdirectory(constStrDirName);
            if (reg.GetLength(0) == 0)
                dir.CreateSubdirectory(constRegDirName);
            // Каталоги Строки и Районы для одного месяца не могут быть заполнены одновременно
            if ((str.GetLength(0) > 0 && str[0].GetFiles().GetLength(0) > 0) &&
                (reg.GetLength(0) > 0 && reg[0].GetFiles().GetLength(0) > 0))
                throw new Exception("Каталоги \"Строки\" и \"Районы\" для одного месяца не могут быть заполнены одновременно");
        }

        private void ProcessAllFiles(DirectoryInfo dir)
        {
            ProcessFilesTemplate(dir, "*.xls", new ProcessFileDelegate(PumpXLSFile), false);

            // если есть распаковываем архивы rar
            FileInfo[] rarFiles = dir.GetFiles("*.rar", SearchOption.AllDirectories);
            foreach (FileInfo rarFile in rarFiles)
            {
                DirectoryInfo tempDir = CommonRoutines.ExtractArchiveFileToTempDir(rarFile.FullName,
                    FilesExtractingOption.SingleDirectory, ArchivatorName.Rar);
                try
                {
                    ProcessAllFiles(tempDir);
                }
                finally
                {
                    CommonRoutines.DeleteDirectory(tempDir);
                }
            }
        }

        private void PumpFiles(DirectoryInfo dir)
        {
            reportType = ReportType.Svod;
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStartFilePumping, "Старт закачки данных сводных отчетов.");
            ProcessAllFiles(dir.GetDirectories(constSvodDirName)[0]);
/*            reportType = ReportType.Region;
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStartFilePumping, "Старт закачки данных отчетов в разрезе районов.");
            ProcessAllFiles(dir.GetDirectories(constRegDirName)[0]);
            reportType = ReportType.Str;
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStartFilePumping, "Старт закачки данных отчетов в разрезе строк.");
            ProcessAllFiles(dir.GetDirectories(constStrDirName)[0]);*/
        }

        #endregion Общие функции закачки

        #region Работа с Excel

        private const string LITERAL_X = "XХ";
        private string CleanValue(string value)
        {
            return value.ToUpper().Trim(LITERAL_X.ToCharArray());
        }

        // Регулярное выражение для поиска сочетания тыс. руб. или тыс.руб.
        Regex regExThousandRobule = new Regex(@"тыс\.(.*)руб(лей)?\.?", RegexOptions.IgnoreCase);
        private string DeleteThousandRouble(string value)
        {
            return regExThousandRobule.Replace(value, String.Empty).Trim();
        }

        private void CheckXLSTotalSum(ExcelHelper excelDoc, int curRow)
        {
            string value = CleanValue(excelDoc.GetValue(curRow, 3).Trim());
            if (value != string.Empty)
                CheckTotalSum(Convert.ToDecimal(value), totalSums[0], "Значение показателей");
        }

        private int PumpMarks(ExcelHelper excelDoc, int curRow, string marksName)
        {
            string name = DeleteThousandRouble(marksName).Trim();
            string value = excelDoc.GetValue(curRow, 2).Trim();
            if (value == string.Empty)
                return -1;

            int code = Convert.ToInt32(value);
            object[] mapping = new object[] { "NAME", name, "CODE", code };

            return PumpCachedRow(cacheMarks, dsMarks.Tables[0], clsMarks, mapping, code.ToString(), "ID");
        }

        private void PumpFactRow(object[] mapping)
        {
            PumpRow(GetFactTable(), mapping);
            if ((dsIncomesRegion.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT) || (dsIncomesTotal.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT))
            {
                UpdateData();
                if (this.reportType != ReportType.Svod)
                    ClearDataSet(daIncomesRegion, ref dsIncomesRegion);
                else ClearDataSet(daIncomesTotal, ref dsIncomesTotal);
            }
        }

        // установить коэффициент sumMultiplier
        private void SetSumMultiplier(int marksCode)
        {
            if ((marksCode >= 200) && (marksCode <= 300))
                sumMultiplier = 1000;
            else
                sumMultiplier = 1;
        }

        private void PumpXLSRow(ExcelHelper excelDoc, int curRow, int refDate, string markName)
        {
            int refMarks = PumpMarks(excelDoc, curRow, markName);
            if (refMarks == -1)
                return;

            string v = excelDoc.GetValue(curRow, 2).Trim();
            int code = Convert.ToInt32(v);
            SetSumMultiplier(code);

            string valueStr = CleanValue(excelDoc.GetValue(curRow, 3).Trim()).PadLeft(1, '0');
            decimal value = Convert.ToDecimal(valueStr);
            if (value == 0)
                return;
            totalSums[0] += value;
            object[] mapping = new object[] { "RefMarks", refMarks, "RefYearDayUNV", refDate,
                    "Value", value*sumMultiplier};
            PumpFactRow(mapping);
        }

        private bool IsSectionStart(string cellValue)
        {
            return (cellValue.ToUpper() == "А");
        }

        public const string TOTAL_ROW = "КОНТРОЛЬНАЯ СУММА";
        private bool IsSectionEnd(string cellValue)
        {
            return cellValue.ToUpper().Contains(TOTAL_ROW);
        }

        // получаем полное наименование показателя,
        // т.к. оно может находиться на нескольких строках
        private string GetXlsMarksName(ExcelHelper excelDoc, ref int curRow)
        {
            List<string> marksName = new List<string>();
            while (excelDoc.GetValue(curRow, 2).Trim() == string.Empty)
            {
                marksName.Add(excelDoc.GetValue(curRow, 1).Trim());
                curRow++;
            }
            marksName.Add(excelDoc.GetValue(curRow, 1).Trim());
            return string.Join(" ", marksName.ToArray());
        }

        private void PumpXlsSheet(ExcelHelper excelDoc, string fileName, int refDate)
        {
            int rowsCount = excelDoc.GetRowsCount();
            string dataSourcePath = GetShortSourcePathBySourceID(this.SourceID);
            string workSheetname = excelDoc.GetWorksheetName().Trim();

            tableNumber = 0;
            bool toPumpRow = false;
            for (int curRow = 1; curRow <= rowsCount; curRow++)
            {
                try
                {
                    SetProgress(rowsCount, curRow, string.Format("Обработка файла {0}\\{1}...", dataSourcePath, fileName),
                        string.Format("Строка {0} из {1} листа {2}", curRow, rowsCount, workSheetname));

                    string cellValue = excelDoc.GetValue(curRow, 1).Trim();
                    if (cellValue == string.Empty)
                        continue;

                    if (IsSectionEnd(cellValue))
                    {
                        CheckXLSTotalSum(excelDoc, curRow);
                        toPumpRow = false;
                        continue;
                    }

                    if (toPumpRow)
                    {
                        string markName = GetXlsMarksName(excelDoc, ref curRow);
                        PumpXLSRow(excelDoc, curRow, refDate, markName);
                        continue;
                    }

                    if (IsSectionStart(cellValue))
                    {
                        SetNullTotalSum();
                        toPumpRow = true;
                        continue;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("При обработке строки {0} возникла ошибка ({1})", curRow, ex.Message), ex);
                }
            }
        }

        private void PumpXLSFile(FileInfo file)
        {
            WriteToTrace("Открытие документа: " + file.Name, TraceMessageKind.Information);

            ExcelHelper excelDoc = new ExcelHelper();

            try {
                excelDoc.AskToUpdateLinks = false;
                excelDoc.DisplayAlerts = false;
                excelDoc.EnableEvents = false;
                excelDoc.OpenDocument(file.FullName);
                int wsCount = excelDoc.GetWorksheetsCount();

                int refDate = GetReportDate();

                for (int index = 1; index <= wsCount; index++)
                {
                    excelDoc.SetWorksheet(index);
                    PumpXlsSheet(excelDoc, file.Name, refDate);
                }
            }
            finally
            {
                excelDoc.CloseDocument();
            }
        }

        #endregion Работа с Excel

        #region Перекрытые методы закачки

        protected override void ProcessFiles(DirectoryInfo dir)
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStart, "Старт инициализации Excel.");
            try
            {
                CheckDirectories(dir);
                PumpFiles(dir);
                UpdateData();
            }
            finally
            {
            }
        }

        protected override void DirectPumpData()
        {
            PumpDataYTemplate();
        }

        #endregion Перекрытые методы закачки

        #endregion Закачка данных

        #region Обработка данных

        private const string ROUBLE_UNIT_NAME = "Рубль";
        private const string DEFAULT_UNIT_NAME = "Единица";
        private const string METRE_UNIT_NAME = "Миллион кубических метров";
        private const string KILOMETRE_UNIT_NAME = "Квадратный километр";
        private const string BOT_UNIT_NAME = "Миллион кВт.ч";

        private int GetRefUnits(int marksCode)
        {
            if ((marksCode >= 100) && (marksCode <= 125))
                return FindCachedRow(cacheUnits, METRE_UNIT_NAME, nullUnits);

            if (marksCode == 140)
                return FindCachedRow(cacheUnits, KILOMETRE_UNIT_NAME, nullUnits);

            if (marksCode == 150)
                return FindCachedRow(cacheUnits, BOT_UNIT_NAME, nullUnits);

            if ((marksCode >= 200) && (marksCode <= 300))
                return FindCachedRow(cacheUnits, ROUBLE_UNIT_NAME, nullUnits);
            return FindCachedRow(cacheUnits, DEFAULT_UNIT_NAME, nullUnits);
        }

        protected void SetRefUnits()
        {
            if (cacheUnits.Count <= 1)
            {
                WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeWarning, "Классификатор «ЕдИзмер.ОКЕИ» не заполнен.");
                return;
            }
            foreach (DataRow row in dsMarks.Tables[0].Rows)
            {
                int refUnits = GetRefUnits(Convert.ToInt32(row["Code"]));
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
            ProcessDataSourcesTemplate(year, month, "Выполняется расстановка ссылок на классификатор ЕдИзмер.ОКЕИ");
        }

        #endregion Обработка данных

    }
}