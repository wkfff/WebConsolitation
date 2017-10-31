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
using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.DataPumps.Form14Pump
{
    /// <summary>
    /// УФК_0003_ФОРМА 14
    /// Приложение 14 к инструкции о порядке ведения бухгалтерского учета доходов федерального бюджета и 
    /// распределения в порядке регулирования доходов между бюджетами разных уровней бюджетной системы РФ (.TXT)
    /// </summary>
    public class Form14PumpModule : TextRepPumpModuleBase
    {
        #region Поля

        // КД
        private IDbDataAdapter daKD;
        private DataSet dsKD;
        // Показатели формы 14
        private IDbDataAdapter daDataMarksF14;
        private DataSet dsDataMarksF14;
        // Факты
        private IDbDataAdapter daIncomesForm14;
        private DataSet dsIncomesForm14;

        private IClassifier clsKD;
        private IClassifier fxcDataMarksF14;
        private IFactTable fctIncomesForm14;

        // Счетчик закачанных записей
        private int recCount = 0;

        private Dictionary<string, int> kdList = null;//new Dictionary<string, int>(1000);

        #endregion Поля


        #region Константы

        // Количество записей для занесения в базу
        private const int constMaxQueryRecords = 10000;

        #endregion Константы


        #region Инициализация

        /// <summary>
        /// Конструктор
        /// </summary>
        public Form14PumpModule()
            : base()
        {

        }

        #endregion Инициализация


        #region Закачка данных

        /// <summary>
        /// Закачивает строку таблицы отчета
        /// </summary>
        /// <param name="row">Строка отчета</param>
        /// <param name="date">Дата</param>
        private void PumpFactRow(DataRow row, int date)
        {
            string kd = Convert.ToString(row["KD"]);

            AddFactRow(
                GetDoubleCellValue(row, "ForDay_Debit", 0),
                GetDoubleCellValue(row, "ForCurrMonth_Debit", 0),
                GetDoubleCellValue(row, "FromBegYear_Debit", 0),
                date, kd, 1);

            AddFactRow(
                GetDoubleCellValue(row, "ForDay_Credit", 0),
                GetDoubleCellValue(row, "ForCurrMonth_Credit", 0),
                GetDoubleCellValue(row, "FromBegYear_Credit", 0),
                date, kd, 2);

            AddFactRow(
                GetDoubleCellValue(row, "ForDay_Balance", 0),
                GetDoubleCellValue(row, "ForCurrMonth_Balance", 0),
                GetDoubleCellValue(row, "FromBegYear_Balance", 0),
                date, kd, 3);
        }

        /// <summary>
        /// Возвращает дату отчета из имени файла
        /// </summary>
        /// <param name="file">Файл отчета</param>
        /// <returns>Дата</returns>
        private int GetReportDate(FileInfo file)
        {
            if (file.Name.ToUpper().StartsWith("R"))
            {
                return Convert.ToInt32(string.Format(
                    "{0}{1}{2}",
                    this.DataSource.Year,
                    file.Name.Substring(4, 2).PadLeft(2, '0'),
                    file.Name.Substring(6, 2).PadLeft(2, '0')));
            }
            return -1;
        }

        /// <summary>
        /// Закачивает строку из датасета текстовых отчетов
        /// </summary>
        /// <param name="fromBeginYear">Сумма с начала года</param>
        /// <param name="forCurrMonth">Сумма за текущий месяц</param>
        /// <param name="date">Дата справки</param>
        /// <param name="kd">Код КД</param>
        /// <param name="refDataMarks">ИД показателя формы 14</param>
        private void AddFactRow(double forDay, double forCurrMonth, double fromBeginYear, int date, string kd, int refDataMarks)
        {
            if (forDay == 0 && forCurrMonth == 0 && fromBeginYear == 0) return;

            int kdID = PumpCachedRow(kdList, dsKD.Tables[0], clsKD, kd, new object[] { "CODESTR", kd });

            DataRow row = dsIncomesForm14.Tables[0].NewRow();
            //row["ID"] = fctIncomesForm14.GetGeneratorNextValue;
            row["PUMPID"] = this.PumpID;
            row["SOURCEID"] = this.SourceID;
            row["FORDAY"] = forDay;
            row["FORCURRMONTH"] = forCurrMonth;
            row["FROMBEGINYEAR"] = fromBeginYear;
            row["REFKD"] = kdID;
            row["RefYearDayUNV"] = date;
            row["REFDATAMARKSFORM14"] = refDataMarks;
            dsIncomesForm14.Tables[0].Rows.Add(row);

            recCount++;
            if (recCount >= constMaxQueryRecords)
            {
                UpdateData();
                ClearDataSet(daIncomesForm14, ref dsIncomesForm14);
                recCount = 0;
            }
        }
        
        /// <summary>
        /// Закачивает текстовые файлы
        /// </summary>
        /// <param name="sourceDir">Каталог с файлами для закачки</param>
        protected override void ProcessFiles(DirectoryInfo dir)
        {
            int totalRecs;
            int rowsCount = 0;
            int skippedReports = 0;
            int skippedRows = 0;
            int processedReports = 0;
            int date = 0;
            string err = string.Empty;
            string processedFiles = "<Нет данных>";

            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStartFilePumping, "Старт закачки данных текстовых отчетов.");

            try
            {
                CommonRoutines.ExtractArchiveFiles(dir.FullName, dir.FullName, ArchivatorName.Arj,
                    FilesExtractingOption.SeparateSubDirs);

                try
                {
                    this.CallTXTSorcerer(xmlSettingsForm14, dir.FullName);
                }
                catch (Exception ex)
                {
                    WriteEventIntoDataPumpProtocol(
                        DataPumpEventKind.dpeWarning, "Закачка данных текстовых отчетов закончена: " + ex.Message);
                    return;
                }

                totalRecs = GetTotalRecs();

                // Закачиваем полученные данные
                // Первая таблица датасета - служебная, ее не берем
                for (int i = 1; i < this.ResultDataSet.Tables.Count; i++)
                {
                    DataTable dt = this.ResultDataSet.Tables[i];
                    if (dt.Rows.Count == 0) continue;

                    int fileIndex = Convert.ToInt32(dt.Rows[0][this.FileIndexFieldName]);
                    processedFiles = GetStringCellValue(this.ResultDataSet.Tables[0].Rows[fileIndex], "FILES", "<Нет данных>");

                    // Дата справки
                    string str = this.FixedParameters[fileIndex]["ReportDateEx"].Value;

                    if (str != string.Empty)
                    {
                        date = Convert.ToInt32(str);
                    }
                    else
                    {
                        date = GetReportDate(this.RepFilesLists[0][fileIndex]);
                    }

                    if (date > 0 && date / 10000 != this.DataSource.Year)
                    {
                        skippedReports++;
                        skippedRows += this.ResultDataSet.Tables[i].Rows.Count;
                        continue;
                    }

                    for (int j = 0; j < dt.Rows.Count; j++)
                    {
                        PumpFactRow(dt.Rows[j], date);

                        rowsCount++;
                        this.SetProgress(totalRecs, rowsCount,
                            string.Format("Источник {0}. Обработка данных...", dir.FullName),
                            string.Format("Строка {0} из {1}", rowsCount, totalRecs));
                    }

                    processedReports++;
                }

                WriteEventIntoDataPumpProtocol(
                    DataPumpEventKind.dpeSuccessfullFinishFilePump, string.Format(
                        "Закачка данных текстовых отчетов закончена. Обработано отчетов: {0} ({1} строк), " +
                        "из них пропущено из-за несоответствия даты источнику: {2} отчетов ({3} строк).",
                        processedReports, rowsCount, skippedReports, skippedRows));
            }
            catch (Exception ex)
            {
                WriteEventIntoDataPumpProtocol(
                    DataPumpEventKind.dpeFinishFilePumpWithError, string.Format(
                        "Закачка данных текстовых отчетов закончена с ошибками. \n" +
                        "На момент возникновения ошибки достигнуты следующие результаты. " +
                        "Обработано отчетов: {0} ({1} строк), " +
                        "из них пропущено из-за несоответствия даты источнику: {2} отчетов ({3} строк). " +
                        "Ошибка возникла при обрабатке файлов {4}.",
                        processedReports, rowsCount, skippedReports, skippedRows, processedFiles), ex);
                throw;
            }
            finally
            {
                CommonRoutines.DeleteExtractedDirectories(dir);
            }
        }

        /// <summary>
        /// Запрос данных из базы
        /// </summary>
        protected override void QueryData()
        {
            InitClsDataSet(ref daKD, ref dsKD, clsKD, false, string.Empty);
            InitDataSet(ref daDataMarksF14, ref dsDataMarksF14, fxcDataMarksF14, false, string.Empty, string.Empty);

            InitFactDataSet(ref daIncomesForm14, ref dsIncomesForm14, fctIncomesForm14);

            FillRowsCache(ref kdList, dsKD.Tables[0], "CODESTR");
        }

        /// <summary>
        /// Внести изменения в базу
        /// </summary>
        protected override void UpdateData()
        {
            UpdateDataSet(daKD, dsKD, clsKD);
            UpdateDataSet(daIncomesForm14, dsIncomesForm14, fctIncomesForm14);
        }

        private const string FX_FX_DATA_MARKS_FORM14_GUID = "64c5e040-36c1-47f2-8cf4-063ae156583a";
        private const string D_KD_FORM14_GUID = "f9c80382-0efb-41d8-a9db-12462f5e58d3";
        private const string F_F_INCOMES_FORM14_GUID = "e3bea522-1552-458d-9275-0262b0636e99";
        protected override void InitDBObjects()
        {
            // Названия таблиц схемы
            fxcDataMarksF14 = this.Scheme.Classifiers[FX_FX_DATA_MARKS_FORM14_GUID];

            this.UsedClassifiers = new IClassifier[] {
                clsKD = this.Scheme.Classifiers[D_KD_FORM14_GUID] };

            this.UsedFacts = new IFactTable[] {
                fctIncomesForm14 = this.Scheme.FactTables[F_F_INCOMES_FORM14_GUID] };
        }

        /// <summary>
        /// Функция выполнения завершающих действий этап
        /// </summary>
        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsKD);
            ClearDataSet(ref dsDataMarksF14);
            ClearDataSet(ref dsIncomesForm14);
        }

        /// <summary>
        /// Закачка данных
        /// </summary>
        protected override void DirectPumpData()
        {
            PumpDataYTemplate();
        }

        #endregion Закачка данных
    }
}

