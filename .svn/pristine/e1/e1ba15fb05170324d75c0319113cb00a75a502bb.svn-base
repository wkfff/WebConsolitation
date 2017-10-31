using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Krista.FM.Server.DataPumps;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.MOFO26Pump
{

    // МОФО - 0026 - Показатели деятельности АО
    public class MOFO26PumpModule : CorrectedPumpModuleBase
    {

        #region Поля

        #region Классификаторы

        // Организации.Показатели деятельности АО (d_Org_IndexCapital)
        private IDbDataAdapter daOrg;
        private DataSet dsOrg;
        private IClassifier clsOrg;
        private Dictionary<string, int> cacheOrg = null;
        private int nullOrg = -1;
        // Районы.Планирование (d_Regions_Plan)
        private IDbDataAdapter daRegions;
        private DataSet dsRegions;
        private IClassifier clsRegions;
        private Dictionary<string, int> cacheRegions = null;

        #endregion Классификаторы

        #region Факты

        // Факт.Показатели деятельности АО (f_F_IndexCapital)
        private IDbDataAdapter daIndexCapital;
        private DataSet dsIndexCapital;
        private IFactTable fctIndexCapital;

        #endregion Факты

        private ReportType reportType;
        private int sourceID = -1;
        private List<int> deletedSourceIDList = null;
        private List<int> deletedRegionsList = null;
        // true - будут качаться нули, в т.ч. и для неуказанных организаций
        // это нужно для отчетов по МО, от которых не поступили данные
        // используется при закачке ГО и МР, но не для поселений
        private bool fullPump = false;

        #endregion Поля

        #region Перечисления

        private enum ReportType
        {
            // данные по городским округам (файлы aoГГ_go_ННН.xls)
            IndexCapitalGO,
            // данные по муниципальным районам (файлы aoГГ_mr_ННН.xls)
            IndexCapitalMR
        }

        #endregion Перечисления

        #region Закачка данных

        #region Работа с базой и кэшами

        private void SetNewSourceID()
        {
            sourceID = this.AddDataSource("ФО", "0029", ParamKindTypes.Year, string.Empty,
                this.DataSource.Year, 0, string.Empty, 0, string.Empty).ID;

            if (deletedSourceIDList == null)
                deletedSourceIDList = new List<int>();

            if (!deletedSourceIDList.Contains(sourceID) && this.DeleteEarlierData)
            {
                DirectDeleteFactData(new IFactTable[] { fctIndexCapital }, -1, sourceID, string.Empty);
                deletedSourceIDList.Add(sourceID);
            }
        }

        private void FillRegionsCache(DataTable dt)
        {
            cacheRegions = new Dictionary<string, int>();
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
                    cacheRegions.Add(codeLine, Convert.ToInt32(row["ID"]));
            }
        }

        private void FillCaches()
        {
            FillRowsCache(ref cacheOrg, dsOrg.Tables[0], new string[] { "INN", "KPP" }, "|", "ID");
            FillRegionsCache(dsRegions.Tables[0]);
        }

        protected override void QueryData()
        {
            SetNewSourceID();

            string constraint = string.Format("SourceID = {0}", sourceID);
            InitDataSet(ref daRegions, ref dsRegions, clsRegions, constraint);
            InitDataSet(ref daOrg, ref dsOrg, clsOrg, string.Empty);
            InitFactDataSet(ref daIndexCapital, ref dsIndexCapital, fctIndexCapital);

            FillCaches();
        }

        #region GUIDs

        private const string D_ORG_INDEXCAPITAL_GUID = "c24ea915-76e3-4310-b195-12dd61631a69";
        private const string D_REGIONS_PLAN_GUID = "1f34cc90-16fd-4fcf-b994-0c8a680d7e23";
        private const string F_F_INDEXCAPITAL_GUID = "8a5c5ccb-768d-47b2-80b2-b1b1dce83f9e";

        #endregion GUIDs
        protected override void InitDBObjects()
        {
            clsOrg = this.Scheme.Classifiers[D_ORG_INDEXCAPITAL_GUID];
            clsRegions = this.Scheme.Classifiers[D_REGIONS_PLAN_GUID];
            fctIndexCapital = this.Scheme.FactTables[F_F_INDEXCAPITAL_GUID];

            this.UsedClassifiers = new IClassifier[] { };
            this.UsedFacts = new IFactTable[] { fctIndexCapital };
        }

        protected override void UpdateData()
        {
            UpdateDataSet(daOrg, dsOrg, clsOrg);
            UpdateDataSet(daIndexCapital, dsIndexCapital, fctIndexCapital);
        }

        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsIndexCapital);
            ClearDataSet(ref dsOrg);
            ClearDataSet(ref dsRegions);
        }

        #endregion Работа с базой и кэшами

        #region Работа с Xls

        private decimal CleanFactValue(string value)
        {
            decimal factValue = 0;
            Decimal.TryParse(CommonRoutines.TrimLetters(value).Replace('.', ','), out factValue);
            return factValue;
        }

        private bool SumsIsNull(object[] mapping)
        {
            if (fullPump)
                return false;
            for (int i = 0; i < mapping.GetLength(0); i += 2)
            {
                if (Convert.ToDecimal(mapping[i + 1]) != 0)
                    return false;
            }
            return true;
        }

        private object GetDevidendDate(string dateStr)
        {
            DateTime date = new DateTime();
            if (DateTime.TryParse(CommonRoutines.TrimLetters(dateStr), out date))
                return date;
            return DBNull.Value;
        }

        private void PumpXlsFact(ExcelHelper excelDoc, int firstRow, int refDate, int refRegions, int refOrg)
        {
            object[] mapping = new object[] {
                "Capital", CleanFactValue(excelDoc.GetValue(firstRow + 3, 9)) * 1000,
                "ClearAsset", CleanFactValue(excelDoc.GetValue(firstRow + 4, 9)) * 1000,
                "ReserveFund", CleanFactValue(excelDoc.GetValue(firstRow + 5, 9)),
                "PartStock", CleanFactValue(excelDoc.GetValue(firstRow + 6, 9)),
                "ClearProfit", CleanFactValue(excelDoc.GetValue(firstRow + 7, 9)) * 1000,
                "DeductResFund", CleanFactValue(excelDoc.GetValue(firstRow + 8, 9)) * 1000,
                "DeductSpecFund", CleanFactValue(excelDoc.GetValue(firstRow + 9, 9)) * 1000,
                "AddDividend", CleanFactValue(excelDoc.GetValue(firstRow + 10, 9)) * 1000,
                "TransferDividend", CleanFactValue(excelDoc.GetValue(firstRow + 12, 9)) * 1000,
                "DebtDividend", CleanFactValue(excelDoc.GetValue(firstRow + 13, 9)) * 1000,
            };

            if (SumsIsNull(mapping))
                return;

            object dateDividend = GetDevidendDate(excelDoc.GetValue(firstRow + 11, 9));

            mapping = (object[])CommonRoutines.ConcatArrays(mapping, new object[] {
                "DateDividend", dateDividend, "RefYearDayUNV", refDate, "RefRegions", refRegions,
                "RefOrg", refOrg, "SourceID", sourceID });
            PumpRow(dsIndexCapital.Tables[0], mapping);
            if (dsIndexCapital.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
            {
                UpdateData();
                ClearDataSet(daIndexCapital, ref dsIndexCapital);
            }
        }

        private int PumpXlsOrg(ExcelHelper excelDoc, int firstRow)
        {
            string name = excelDoc.GetValue(firstRow, 9).Trim();
            if (name.Length > 255)
                name = name.Substring(0, 255);
            string innStr = excelDoc.GetValue(firstRow + 1, 9).Trim();
            string kppStr = excelDoc.GetValue(firstRow + 2, 9).Trim();
            if ((innStr == string.Empty) && (kppStr == string.Empty))
                return nullOrg;

            for (; ; firstRow++)
                if (excelDoc.GetValue(firstRow, 2).Trim().ToUpper().StartsWith("ПРИМЕЧАНИЕ"))
                    break;
            string note = excelDoc.GetValue(firstRow, 9).Trim();
            if (note.Length > 255)
                note = note.Substring(0, 255);

            long inn = Convert.ToInt64(innStr.PadLeft(1, '0'));
            long kpp = Convert.ToInt64(kppStr.PadLeft(1, '0'));

            string key = string.Format("{0}|{1}", inn, kpp);
            return PumpCachedRow(cacheOrg, dsOrg.Tables[0], clsOrg, key, new object[] {
                "INN", inn, "KPP", kpp, "Name", name, "Note", note });
        }

        private bool GetBlockFirstRow(ExcelHelper excelDoc, ref int firstRow)
        {
            // т.к. ячейки листа заблокированы, то функция, возвращая кол-во строк, не работает
            // поэтому качаем до тех пор, пока не будет 10 пустых строк подряд
            int emptyStrCount = 0;
            for (; emptyStrCount <= 10; firstRow++)
            {
                string cellValue = excelDoc.GetValue(firstRow, 2).Trim();
                if (cellValue == string.Empty)
                {
                    emptyStrCount++;
                    continue;
                }
                emptyStrCount = 0;

                if (cellValue.ToUpper().StartsWith("НАИМЕНОВАНИЕ АО"))
                    return true;
            }
            return false;
        }

        private void PumpXlsSheet(ExcelHelper excelDoc, int refDate, int refRegions)
        {
            int firstRow = 1;
            while (GetBlockFirstRow(excelDoc, ref firstRow))
                try
                {
                    int refOrg = PumpXlsOrg(excelDoc, firstRow);
                    PumpXlsFact(excelDoc, firstRow, refDate, refRegions, refOrg);
                    firstRow++;
                }
                catch (Exception ex)
                {
                    throw new PumpDataFailedException(string.Format(
                        "При обработке листа '{0}' возникла ошибка ({1}).",
                        excelDoc.GetWorksheetName(), ex.Message));
                }
        }

        private void SetReportType(string filename)
        {
            filename = filename.ToUpper();
            if (filename.Contains("GO"))
                reportType = ReportType.IndexCapitalGO;
            if (filename.Contains("MR"))
                reportType = ReportType.IndexCapitalMR;
        }

        private int GetRefDate(string filename)
        {
            // из названия файла aoГГ_go_ННН.xls или aoГГ_mr_ННН.xls берем ГГ - год
            string strDate = CommonRoutines.TrimLetters(filename.Split('_')[0]);
            int year = Convert.ToInt32(strDate) + 2000;
            return year * 10000 + 1;
        }

        private void DeleteEarlierDataByRegions(int refRegions)
        {
            if (!deletedRegionsList.Contains(refRegions) && !this.DeleteEarlierData)
            {
                string constr = string.Format("RefRegions = {0}", refRegions);
                DirectDeleteFactData(new IFactTable[] { fctIndexCapital }, -1, sourceID, constr);
                deletedRegionsList.Add(refRegions);
            }
        }

        private int GetRefRegions(string value)
        {
            string codeLine = string.Empty;
            switch (reportType)
            {
                case ReportType.IndexCapitalGO:
                    fullPump = true;
                    // из названия файла aoГГ_go_ННН.xls берем ННН
                    codeLine = CommonRoutines.TrimLetters(value.Split('_')[2].Trim());
                    break;
                case ReportType.IndexCapitalMR:
                    fullPump = false;
                    // из названия листа z_РРРНН берем РРР - для МР, РРРНН - для поселений
                    codeLine = CommonRoutines.TrimLetters(value);
                    if (Convert.ToInt32(codeLine.Substring(codeLine.Length - 2)) == 0)
                    {
                        fullPump = true;
                        codeLine = codeLine.Substring(0, 3);
                    }
                    break;
            }

            if (!cacheRegions.ContainsKey(codeLine))
            {
                throw new PumpDataFailedException(string.Format(
                    "Не найдено муниципальное образование с кодом {0} в справочнике «Районы.Планирование».", codeLine));
            }

            DeleteEarlierDataByRegions(cacheRegions[codeLine]);

            return cacheRegions[codeLine];
        }

        private bool SkipSheet(string sheetname)
        {
            return !sheetname.Trim().ToUpper().StartsWith("Z_");
        }

        private void PumpXlsFile(FileInfo file)
        {
            WriteToTrace("Открытие документа: " + file.Name, TraceMessageKind.Information);
            ExcelHelper excelDoc = new ExcelHelper();
            try
            {
                SetReportType(file.Name);

                excelDoc.AskToUpdateLinks = false;
                excelDoc.DisplayAlerts = false;
                excelDoc.EnableEvents = false;
                excelDoc.OpenDocument(file.FullName);

                int refDate = GetRefDate(file.Name);
                switch (reportType)
                {
                    case ReportType.IndexCapitalGO:
                        excelDoc.SetWorksheet(1);
                        PumpXlsSheet(excelDoc, refDate, GetRefRegions(file.Name));
                        break;
                    case ReportType.IndexCapitalMR:
                        int wsCount = excelDoc.GetWorksheetsCount();
                        for (int index = 1; index <= wsCount; index++)
                        {
                            excelDoc.SetWorksheet(index);
                            if (SkipSheet(excelDoc.GetWorksheetName()))
                                continue;
                            PumpXlsSheet(excelDoc, refDate, GetRefRegions(excelDoc.GetWorksheetName()));
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

        // удаление организаций, по которым нет данных в таблице фактов
        private void DeleteUnusedOrgs()
        {
            List<string> deletedOrgs = new List<string>();
            try
            {
                string query = string.Format(
                    "select * from {0} where not ID in (select distinct RefOrg from {1}) and ID > 0",
                    clsOrg.FullDBName, fctIndexCapital.FullDBName);
                using (DataTable dt = (DataTable)this.DB.ExecQuery(query, QueryResultTypes.DataTable, new IDbDataParameter[] { }))
                {
                    foreach (DataRow row in dt.Rows)
                        deletedOrgs.Add(string.Format("{0} ({1})", row["INN"], row["Name"]));
                }

                if (deletedOrgs.Count == 0)
                    return;

                query = string.Format(
                    "delete from {0} where not ID in (select distinct RefOrg from {1}) and ID > 0",
                    clsOrg.FullDBName, fctIndexCapital.FullDBName);
                this.DB.ExecQuery(query, QueryResultTypes.NonQuery, new IDbDataParameter[] { });

                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, string.Format(
                    "Из классификатора '{0}' удалены организации, по которым нет данных в таблице фактов '{1}': {2}.",
                    clsOrg.FullCaption, fctIndexCapital.FullCaption, string.Join(", ", deletedOrgs.ToArray())));
            }
            finally
            {
                deletedOrgs.Clear();
            }
        }

        // корректировка дублирующихся наименований организаций путем приписывания инн
        private void CorrectOrgsNames()
        {
            Dictionary<string, long> auxCache = new Dictionary<string, long>();
            try
            {
                InitDataSet(ref daOrg, ref dsOrg, clsOrg, "ID > 0");
                foreach (DataRow org in dsOrg.Tables[0].Rows)
                {
                    long inn = Convert.ToInt64(org["Inn"]);

                    string name = Convert.ToString(org["Name"]);
                    // если наименование встречается впервые, то просто пишем его в кэш
                    if (!auxCache.ContainsKey(name))
                    {
                        auxCache.Add(name, inn);
                        continue;
                    }

                    if (auxCache[name] == inn)
                        continue;

                    // вот имя повторилось, а инн другой. дописываем его к наименованию
                    org["Name"] = string.Format("{0} ({1})", name, inn);
                }
            }
            finally
            {
                auxCache.Clear();
            }
        }

        protected override void DeleteEarlierPumpedData()
        {
            // заглушка
        }

        protected override void ProcessFiles(DirectoryInfo dir)
        {
            deletedRegionsList = new List<int>();
            try
            {
                ProcessFilesTemplate(dir, "*.xls", new ProcessFileDelegate(PumpXlsFile), false);
                UpdateData();
                DeleteUnusedOrgs();
                CorrectOrgsNames();
            }
            finally
            {
                deletedRegionsList.Clear();
            }
        }

        protected override void DirectPumpData()
        {
            PumpDataYTemplate();
        }

        #endregion Перекрытые методы

        #endregion Закачка данных

    }

}
