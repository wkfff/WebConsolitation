using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Krista.FM.Server.DataPumps;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.MOFO20Pump
{

    // МОФО - 0020 - Трёхстороннее соглашение о МРОТ
    public class MOFO20PumpModule : CorrectedPumpModuleBase
    {

        #region Поля

        #region Классификаторы

        // Районы.Анализ (d_Regions_Analysis)
        private IDbDataAdapter daRegions;
        private DataSet dsRegions;
        private IClassifier clsRegions;
        private Dictionary<string, DataRow> cacheRegions = null;
        private int nullRegions = -1;
        // Сбор.Задача формирования отчета (d_CD_Task)
        private IDbDataAdapter daTask;
        private DataSet dsTask;
        private IClassifier clsTask;
        private Dictionary<string, int> cacheTask = null;
        // Отчет.МОФО_Трехстороннее соглашение о МРОТ (d_Report_TrihedrAgr)
        private IDbDataAdapter daReport;
        private DataSet dsReport;
        private IClassifier clsReport;
        private Dictionary<int, int> cacheReport = null;
        // Отчет.МОФО_Трехстороннее соглашение о МРОТ Должности (d_Report_TAJobTitle)
        private IDbDataAdapter daJobTitle;
        private DataSet dsJobTitle;
        private IClassifier clsJobTitle;
        private Dictionary<int, DataRow> cacheJobTitle = null;
        // Сбор.Субъекты сбора (d_CD_Subjects)
        private IClassifier clsSubjects;
        // Сбор.Шаблоны форм сбора (d_CD_Templates)
        private IClassifier clsTemplates;

        #endregion Классификаторы

        #region Факты

        // Показатели.МОФО_Трехстороннее соглашение о МРОТ (f_Marks_MOFOTrihedralAgr)
        private IDbDataAdapter daMarksFact;
        private DataSet dsMarksFact;
        private IFactTable fctMarksFact;

        #endregion Факты

        private int foSourceID = -1;
        private List<string> deletedDatesAndRegionsList = null;
        private Dictionary<string, List<string>> pumpedDateList = null;

        #endregion Поля

        #region Закачка данных

        #region Работа с базой и кэшами

        private int GetQuarterByMonth(int month)
        {
            if ((month > 1) && (month <= 4))
                return 1;
            if ((month > 4) && (month <= 7))
                return 2;
            if ((month > 7) && (month <= 10))
                return 3;
            return 4;
        }

        // формируем блядовитый кэш для клс "Сбор.Задача формирования отчета"
        // ключ составной:
        //    1) год - берем из поля "Год" (RefYear)
        //    2) квартал - берем из поля "Наименование" (Name), наименование содержит «за X квартал», где Х и есть квартал
        //    3) ссылка на клс "Районы.Сопоставимый" - берем по полю RefSubject субъект сбора,
        //       у которого берем по полю RefRegion запись из клс "Районы.Анализ", у которой в свою очередь берем ссылку на сопоставимый RefBridgeRegions
        // значение кэша:
        //    ID записи "Сбор.Задача формирования отчета"
        private void FillTaskCache()
        {
            string query = string.Format(
                " select task.*, regions.RefBridgeRegions " +
                " from {0} task inner join {1} subj on (task.RefSubject = subj.ID), {2} regions " +
                " where subj.RefRegion = regions.ID ",
                clsTask.FullDBName, clsSubjects.FullDBName, clsRegions.FullDBName);
            using (DataTable dt = (DataTable)this.DB.ExecQuery(query, QueryResultTypes.DataTable, new IDbDataParameter[] { }))
            {
                cacheTask = new Dictionary<string, int>();
                foreach (DataRow row in dt.Rows)
                {
                    int quarter = GetQuarterByMonth(Convert.ToDateTime(row["EndDate"]).Month);
                    string key = string.Format("{0}|{1}|{2}", row["RefYear"], quarter, row["RefBridgeRegions"]);
                    if (cacheTask.ContainsKey(key))
                        continue;

                    cacheTask.Add(key, Convert.ToInt32(row["ID"]));
                }
            }
        }

        private void FillCaches()
        {
            FillRowsCache(ref cacheRegions, dsRegions.Tables[0], new string[] { "CodeLine" });
            FillRowsCache(ref cacheReport, dsReport.Tables[0], "RefTask", "ID");
            FillRowsCache(ref cacheJobTitle, dsJobTitle.Tables[0], "RefReport");
            FillTaskCache();
        }

        protected override void QueryData()
        {
            InitDataSet(ref daRegions, ref dsRegions, clsRegions, string.Format("SourceID = {0}", foSourceID));
            nullRegions = clsRegions.UpdateFixedRows(this.DB, foSourceID);
            InitDataSet(ref daTask, ref dsTask, clsTask, string.Empty);
            InitDataSet(ref daReport, ref dsReport, clsReport, string.Empty);
            InitDataSet(ref daJobTitle, ref dsJobTitle, clsJobTitle, string.Empty);
            InitFactDataSet(ref daMarksFact, ref dsMarksFact, fctMarksFact);
            FillCaches();
        }

        #region GUIDs

        private const string D_REGIONS_ANALYSIS_GUID = "383f887a-3ebb-4dba-8abb-560b5777436f";
        private const string D_CD_TASK_GUID = "e0375948-f46d-46ed-b65f-4d85e1b75340";
        private const string D_CD_SUBJECTS_GUID = "c2d46b94-cf54-4392-b226-f4a377dd9d78";
        private const string D_CD_TEMPLATES_GUID = "f27b352b-d3e5-441c-bc69-1fbce8c52e6f";
        private const string D_REPORT_TA_JOB_TITLE_GUID = "0fcb7d5f-6c1b-4531-ac5a-17e0b3d2db54";
        private const string D_REPORT_TRIHEDR_AGR_GUID = "c4bfced8-47a8-4f21-9a09-03c55b71bd35";
        private const string F_MARKS_MOFO_TRIHEDRAL_AGR_GUID = "c3a74cf5-782f-4845-ac35-3a45950cb74e";

        #endregion GUIDs
        protected override void InitDBObjects()
        {
            clsRegions = this.Scheme.Classifiers[D_REGIONS_ANALYSIS_GUID];
            clsTask = this.Scheme.Classifiers[D_CD_TASK_GUID];
            clsSubjects = this.Scheme.Classifiers[D_CD_SUBJECTS_GUID];
            clsReport = this.Scheme.Classifiers[D_REPORT_TRIHEDR_AGR_GUID];
            clsJobTitle = this.Scheme.Classifiers[D_REPORT_TA_JOB_TITLE_GUID];
            clsTemplates = this.Scheme.Classifiers[D_CD_TEMPLATES_GUID];

            fctMarksFact = this.Scheme.FactTables[F_MARKS_MOFO_TRIHEDRAL_AGR_GUID];

            this.UsedClassifiers = new IClassifier[] { };
            this.UsedFacts = new IFactTable[] { fctMarksFact };
        }

        protected override void UpdateData()
        {
            UpdateDataSet(daReport, dsReport, clsReport);
            UpdateDataSet(daJobTitle, dsJobTitle, clsJobTitle);
            UpdateDataSet(daTask, dsTask, clsTask);
            UpdateDataSet(daMarksFact, dsMarksFact, fctMarksFact);
        }

        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsMarksFact);
            ClearDataSet(ref dsRegions);
            ClearDataSet(ref dsTask);
            ClearDataSet(ref dsReport);
            ClearDataSet(ref dsJobTitle);
        }

        #endregion Работа с базой и кэшами

        #region Работа с Xls

        private decimal CleanFactValue(string value)
        {
            // если в файле встречается хоть одна некорректная сумма, пропускаем весь файл
            try
            {
                // бывают такие ебанутые суммы типа "42  111", при конвертации они считаются корректными
                // в результате получается число 42111
                // чтобы число считалось некорректным, заменяем все пробелы каким-нибудь левым символом, например 'x',
                return Convert.ToDecimal(value.Replace('.', ',').Trim().Replace(' ', 'x').PadLeft(1, '0'));
            }
            catch
            {
                throw new PumpDataFailedException(string.Format("Некорректное значение данных '{0}'.", value));
            }
        }

        private void PumpFactRow(string value, int refDate, int refRegions, int refOrgType, int refReport, int refMarks)
        {
            decimal factValue = CleanFactValue(value);
            if (factValue == 0)
                return;

            object[] mapping = new object[] {
                "Value", factValue,
                "RefYearDayUNV", refDate,
                "RefRegions", refRegions,
                "RefMarks", refMarks,
                "RefOrgType", refOrgType,
                "RefReport", refReport,
                "SourceID", foSourceID
            };

            PumpRow(dsMarksFact.Tables[0], mapping);
        }

        // охуенный алгоритм получения ссылки на клс "Отчет.МОФО_Трехстороннее соглашение о МРОТ":
        // 1) ищем в клс "Сбор.Задача формирования отчета" записи, у которых
        //    a) поле "Год" = году из refDate
        //    b) в поле "Наименование" содержится квартал = кварталу из refDate
        // 2) ищем среди них запись, у которой ссылка Субъект сбора (RefSubject) проставлена на запись
        //    в клс "Сбор.Субъекты сбора", у которой ссылка Районы.Анализ (RefRegion) проставлена на запись
        //    в клс "Районы.Анализ", которая ссылается на ту же запись клс "Районы.Сопоставимый", что и refRegions
        // 3) если нашли запись X, которая соответствует пунктам 1 и 2, то ищем запись Y в клс "Отчет.МОФО_Трехстороннее соглашение о МРОТ",
        //    которая ссылается на найденную нами запись X. Запись Y и будет искомый нами результат
        private int GetRefReport(DataRow regions, int refReport)
        {
            int quarter = refReport % 10;
            // чтобы успростить себе жизнь, сформирован специальный кэш cacheTask (см. метод FillTaskCache)
            // чтобы использовать этот кэш, необходимо знать текущие год, квартал и ссылку на клс "Районы.Сопоставимый"
            string keyTask = string.Format("{0}|{1}|{2}", this.DataSource.Year, quarter, regions["RefBridgeRegions"]);
            if (!cacheTask.ContainsKey(keyTask))
            {
                throw new PumpDataFailedException(string.Format(
                    "Не удалось определить ссылку на классификатор «Отчет.МОФО_Трехстороннее соглашение о МРОТ» (год: {0}, квартал: {1}, код района: {2}).",
                    this.DataSource.Year, quarter, regions["CodeLine"]));
            }
            int refTask = cacheTask[keyTask];
            if (cacheReport.ContainsKey(refTask))
                return cacheReport[refTask];
            // если по задаче не удалось определить отчет, то закачиваем в клс "Отчет.МОФО_Трехстороннее соглашение о МРОТ"
            // запись со ссылкой на задачу
            return PumpCachedRow(cacheReport, dsReport.Tables[0], clsReport, refTask, "ID", new object[] { "RefTask", refTask });
        }

        private void PumpXlsJobTitle(ExcelHelper excelDoc, int curColumn, int refReport)
        {
            int curRow = 1;
            for (; ; curRow++)
            {
                string cellValue =excelDoc.GetValue(curRow, curColumn).Trim();
                if (cellValue.ToUpper().StartsWith("ФИО ИСПОЛНИТЕЛЯ"))
                    break;
            }

            string name = excelDoc.GetValue(curRow, curColumn + 1).Trim();
            string phone = excelDoc.GetValue(curRow + 1, curColumn + 1).Trim();

            // если в клс есть такая запись, то просто обновляем поля записи
            if (cacheJobTitle.ContainsKey(refReport))
            {
                cacheJobTitle[refReport]["Name"] = name;
                cacheJobTitle[refReport]["Phone"] = phone;
                return;
            }

            PumpCachedRow(cacheJobTitle, dsJobTitle.Tables[0], clsJobTitle, refReport,
                new object[] { "Name", name, "Phone", phone, "RefReport", refReport });
        }

        #region Закачка файлов первого типа

        private void PumpXlsRowFirst(ExcelHelper excelDoc, int curRow, int refDate, int refRegions, int refOrgType, int refReport)
        {
            PumpFactRow(excelDoc.GetValue(curRow, 3), refDate, refRegions, refOrgType, refReport, 2);
            PumpFactRow(excelDoc.GetValue(curRow, 4), refDate, refRegions, refOrgType, refReport, 3);
            PumpFactRow(excelDoc.GetValue(curRow, 5), refDate, refRegions, refOrgType, refReport, 5);
            PumpFactRow(excelDoc.GetValue(curRow, 6), refDate, refRegions, refOrgType, refReport, 6);
            PumpFactRow(excelDoc.GetValue(curRow, 7), refDate, refRegions, refOrgType, refReport, 8);
            PumpFactRow(excelDoc.GetValue(curRow, 8), refDate, refRegions, refOrgType, refReport, 9);
            PumpFactRow(excelDoc.GetValue(curRow, 9), refDate, refRegions, refOrgType, refReport, 11);
            PumpFactRow(excelDoc.GetValue(curRow, 10), refDate, refRegions, refOrgType, refReport, 12);
            PumpFactRow(excelDoc.GetValue(curRow, 11), refDate, refRegions, refOrgType, refReport, 14);
            PumpFactRow(excelDoc.GetValue(curRow, 12), refDate, refRegions, refOrgType, refReport, 15);
        }

        private void PumpXlsSheetFirst(ExcelHelper excelDoc, int refDate, DataRow regions)
        {
            int refReport = GetRefReport(regions, refDate);
            PumpXlsJobTitle(excelDoc, 2, refReport);
            int refRegions = Convert.ToInt32(regions["ID"]);
            PumpXlsRowFirst(excelDoc, 20, refDate, refRegions, 1, refReport);
            PumpXlsRowFirst(excelDoc, 22, refDate, refRegions, 2, refReport);
            PumpXlsRowFirst(excelDoc, 23, refDate, refRegions, 3, refReport);
        }

        #endregion

        #region Закачка файлов второго типа

        private void PumpXlsRowSecond(ExcelHelper excelDoc, int curRow, int refDate, int refRegions, int refMarks, int refReport)
        {
            PumpFactRow(excelDoc.GetValue(curRow, 4), refDate, refRegions, 1, refReport, refMarks);
            PumpFactRow(excelDoc.GetValue(curRow, 5), refDate, refRegions, 2, refReport, refMarks);
            PumpFactRow(excelDoc.GetValue(curRow, 6), refDate, refRegions, 3, refReport, refMarks);
        }

        private void PumpXlsSheetSecond(ExcelHelper excelDoc, int refDate, DataRow regions)
        {
            int refReport = GetRefReport(regions, refDate);
            PumpXlsJobTitle(excelDoc, 3, refReport);
            int refRegions = Convert.ToInt32(regions["ID"]);
            PumpXlsRowSecond(excelDoc, 14, refDate, refRegions, 2, refReport);
            PumpXlsRowSecond(excelDoc, 15, refDate, refRegions, 3, refReport);
            PumpXlsRowSecond(excelDoc, 17, refDate, refRegions, 5, refReport);
            PumpXlsRowSecond(excelDoc, 18, refDate, refRegions, 6, refReport);
            PumpXlsRowSecond(excelDoc, 21, refDate, refRegions, 8, refReport);
            PumpXlsRowSecond(excelDoc, 22, refDate, refRegions, 9, refReport);
            PumpXlsRowSecond(excelDoc, 24, refDate, refRegions, 11, refReport);
            PumpXlsRowSecond(excelDoc, 25, refDate, refRegions, 12, refReport);
            PumpXlsRowSecond(excelDoc, 27, refDate, refRegions, 14, refReport);
            PumpXlsRowSecond(excelDoc, 28, refDate, refRegions, 15, refReport);
        }

        #endregion

        private bool CheckReportType(ExcelHelper excelDoc)
        {
            return (excelDoc.GetValue("C12").Trim() == string.Empty);
        }

        private DataRow GetRegions(string filename)
        {
            // код региона берем из названия файла sogl_ГГКННН_XX.xls, где ННН - код региона
            string code = CommonRoutines.TrimLetters(filename.Split('_')[1]).Substring(3);
            DataRow region = FindCachedRow(cacheRegions, new string[] { code });
            if (region == null)
            {
                throw new PumpDataFailedException(string.Format(
                    "Не найден район с кодом {0} в справочнике «Районы.Анализ».", code));
            }
            return region;
        }

        private int GetRefDate(string filename)
        {
            // дату формируем из названия файла sogl_ГГКННН_XX.xls,
            // где ГГ - год, К - квартал
            string strDate = filename.Split('_')[1].Trim().Substring(0, 3);
            int year = Convert.ToInt32(strDate.Substring(0, 2)) + 2000;
            int quarter = Convert.ToInt32(strDate.Substring(2, 1));
            return year * 10000 + 9990 + quarter;
        }

        private void DeleteEarlierDataByDateAndRegions(int refDate, int refRegions)
        {
            if (!this.DeleteEarlierData)
            {
                string key = string.Format("{0}|{1}", refDate, refRegions);
                if (!deletedDatesAndRegionsList.Contains(key))
                {
                    string constr = string.Format("RefRegions = {0} and RefYearDayUNV = {1}", refRegions, refDate);
                    DirectDeleteFactData(new IFactTable[] { fctMarksFact }, -1, foSourceID, constr);
                    deletedDatesAndRegionsList.Add(key);
                }
            }
        }

        private bool IsPumpFile(string filename)
        {
            string key = CommonRoutines.TrimLetters(filename.Split('_')[1]);
            if (!pumpedDateList.ContainsKey(key))
            {
                pumpedDateList.Add(key, new List<string>());
                pumpedDateList[key].Add(filename);
                return true;
            }
            pumpedDateList[key].Add(filename);
            return false;
        }

        private void CheckCorrectFile(ExcelHelper excelDoc)
        {
            // в правильном файле в столбце А ничего не должно быть
            for (int curRow = 1; curRow <= 100; curRow++)
            {
                if (excelDoc.GetValue(curRow, 1).Trim() != string.Empty)
                    throw new PumpDataFailedException("Данные представлены в неверном формате.");
            }
        }

        private void PumpXlsFile(FileInfo file)
        {
            if (!IsPumpFile(file.Name))
                return;
            WriteToTrace("Открытие документа: " + file.Name, TraceMessageKind.Information);

            ExcelHelper excelDoc = new ExcelHelper();
            try
            {
                excelDoc.AskToUpdateLinks = false;
                excelDoc.DisplayAlerts = false;
                excelDoc.EnableEvents = false;
                excelDoc.OpenDocument(file.FullName);

                int refDate = GetRefDate(file.Name);
                DataRow regions = GetRegions(file.Name);
                DeleteEarlierDataByDateAndRegions(refDate, Convert.ToInt32(regions["ID"]));

                int wsCount = excelDoc.GetWorksheetsCount();
                for (int index = 1; index <= wsCount; index++)
                {
                    CheckCorrectFile(excelDoc);
                    excelDoc.SetWorksheet(index);
                    if (CheckReportType(excelDoc))
                        PumpXlsSheetFirst(excelDoc, refDate, regions);
                    else
                        PumpXlsSheetSecond(excelDoc, refDate, regions);
                }
                UpdateData();
            }
            catch (PumpDataFailedException ex)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeFinishFilePumpWithError, string.Format(
                    "Ошибка при обработке файла '{0}': {1} Файл будет пропущен.", file.Name, ex.Message));
            }
            finally
            {
                ClearDataSet(daMarksFact, ref dsMarksFact);
                if (excelDoc != null)
                    excelDoc.CloseDocument();
            }
        }

        #endregion Работа с Xls

        #region Перекрытые методы

        // обновляем ссылку "Состояние отчета" (RefStatus) в клс "Сбор.Задача формирования отчета"
        // если в таблице фактов "Показатели.МОФО_Трехстороннее соглашение о МРОТ" данные по отчету есть,
        // то состояние задачи отчета ID = 3 (Утвержден), если нет - то ID = 1 (Редактируется)
        private void ChangeReportState()
        {
            string query = string.Format(
                " update {0} set REFSTATUS = 3 where ID in ( " +
                "   select distinct REFTASK from {1} where ID in ( " +
                "     select distinct REFREPORT from {2} )) " +
                "   and " +
                "   REFTEMPLATE in ( select ID from {3} where CLASS = 'ConsFormMrot' ) ",
                clsTask.FullDBName, clsReport.FullDBName, fctMarksFact.FullDBName, clsTemplates.FullDBName);
            this.DB.ExecQuery(query, QueryResultTypes.NonQuery, new IDbDataParameter[] { });

            query = string.Format(
                " update {0} set REFSTATUS = 1 where not ID in ( " +
                "   select distinct REFTASK from {1} where ID in ( " +
                "     select distinct REFREPORT from {2} )) " +
                "   and " +
                "   REFTEMPLATE in ( select ID from {3} where CLASS = 'ConsFormMrot' ) ",
                clsTask.FullDBName, clsReport.FullDBName, fctMarksFact.FullDBName, clsTemplates.FullDBName);
            this.DB.ExecQuery(query, QueryResultTypes.NonQuery, new IDbDataParameter[] { });
        }

        private void CheckPumpedDate()
        {
            foreach (KeyValuePair<string, List<string>> pumpedDate in pumpedDateList)
            {
                if (pumpedDate.Value.Count > 1)
                {
                    string firstFile = pumpedDate.Value[0];
                    int refDate = GetRefDate(firstFile);
                    string regionCode = pumpedDate.Key.Substring(3);
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, string.Format(
                        "В каталоге источника обнаружены файлы с одинаковой датой ({0}) и районом ({1}): '{2}'. " +
                        "Будет закачан только один из них ('{3}').",
                        refDate, regionCode, string.Join("', '", pumpedDate.Value.ToArray()), firstFile));
                }
            }
        }

        protected override void DeleteEarlierPumpedData()
        {
            if (this.DeleteEarlierData)
            {
                DirectDeleteFactData(new IFactTable[] { fctMarksFact }, -1, foSourceID, string.Empty);
            }
        }

        protected override void ProcessFiles(DirectoryInfo dir)
        {
            deletedDatesAndRegionsList = new List<string>();
            pumpedDateList = new Dictionary<string, List<string>>();
            try
            {
                ProcessFilesTemplate(dir, "*.xls", new ProcessFileDelegate(PumpXlsFile), false);
                UpdateData();
                CheckPumpedDate();
                ChangeReportState();
            }
            finally
            {
                deletedDatesAndRegionsList.Clear();
                pumpedDateList.Clear();
            }
        }

        protected override void PumpDataSource(DirectoryInfo dir)
        {
            foSourceID = AddDataSource("ФО", "0006", ParamKindTypes.Year, string.Empty,
                this.DataSource.Year, 0, string.Empty, 0, string.Empty).ID;
            base.PumpDataSource(dir);
        }

        protected override void DirectPumpData()
        {
            PumpDataYTemplate();
        }

        #endregion Перекрытые методы

        #endregion Закачка данных

    }

}
