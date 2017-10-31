using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Krista.FM.Server.DataPumps;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.MOFO27Pump
{

    // МОФО - 0027 - Показатели деятельности МУП
    public class MOFO27PumpModule : CorrectedPumpModuleBase
    {

        #region Поля

        #region Классификаторы

        // Организации.Показатели деятельности МУП (d_Org_IndexProfit)
        private IDbDataAdapter daOrg;
        private DataSet dsOrg;
        private IClassifier clsOrg;
        private Dictionary<string, int> cacheOrg = null;
        private int nullOrg = -1;
        // Виды деятельности.МОФО_Показатели деятельности МУП (fx_ActivityStatus_IndexProfit)
        private IDbDataAdapter daActivity;
        private DataSet dsActivity;
        private IClassifier clsActivity;
        private Dictionary<string, int> cachceActivity = null;
        private int nullActivity = 0;
        // Районы.Планирование (d_Regions_Plan)
        private IDbDataAdapter daRegions;
        private DataSet dsRegions;
        private IClassifier clsRegions;
        private Dictionary<string, int> cacheRegions = null;

        #endregion Классификаторы

        #region Факты

        // Факт.Показатели деятельности МУП (f_F_IndexProfit)
        private IDbDataAdapter daIndexProfit;
        private DataSet dsIndexProfit;
        private IFactTable fctIndexProfit;

        #endregion Факты

        private ReportType reportType;
        private int sourceID = -1;
        private List<int> deletedSourceIDList = null;
        private List<int> deletedRegionsList = null;
        private int[] refDates = new int[5];
        // true - будут качаться нули, в т.ч. и для неуказанных организаций
        // это нужно для отчетов по МО, от которых не поступили данные
        // используется при закачке ГО и МР, но не для поселений
        private bool fullPump = false;

        #endregion Поля

        #region Перечисления

        private enum ReportType
        {
            // данные по городским округам (файлы mupГГ_go_ННН.xls)
            IndexProfitGO,
            // данные по муниципальным районам (файлы mupГГ_mr_ННН.xls)
            IndexProfitMR
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
                DirectDeleteFactData(new IFactTable[] { fctIndexProfit }, -1, sourceID, string.Empty);
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
            FillRowsCache(ref cachceActivity, dsActivity.Tables[0], "Code");
            FillRegionsCache(dsRegions.Tables[0]);
        }

        protected override void QueryData()
        {
            SetNewSourceID();

            string constraint = string.Format("SourceID = {0}", sourceID);
            InitDataSet(ref daRegions, ref dsRegions, clsRegions, constraint);
            InitDataSet(ref daOrg, ref dsOrg, clsOrg, string.Empty);
            InitDataSet(ref daActivity, ref dsActivity, clsActivity, string.Empty);
            InitFactDataSet(ref daIndexProfit, ref dsIndexProfit, fctIndexProfit);

            FillCaches();
        }

        #region GUIDs

        private const string D_ORG_INDEX_PROFIT_GUID = "21905656-1f4f-4877-bb30-702965ec2ca6";
        private const string D_REGIONS_PLAN_GUID = "1f34cc90-16fd-4fcf-b994-0c8a680d7e23";
        private const string FX_ACTIVITY_STATUS_INDEX_PROFIT_GUID = "62a647bb-f9d7-4606-b5c0-c16efbe95f0b";
        private const string F_F_INDEX_PROFIT_GUID = "566f36f9-0c7e-4987-b5bc-eac5560cd707";

        #endregion GUIDs
        protected override void InitDBObjects()
        {
            clsOrg = this.Scheme.Classifiers[D_ORG_INDEX_PROFIT_GUID];
            clsActivity = this.Scheme.Classifiers[FX_ACTIVITY_STATUS_INDEX_PROFIT_GUID];
            clsRegions = this.Scheme.Classifiers[D_REGIONS_PLAN_GUID];
            fctIndexProfit = this.Scheme.FactTables[F_F_INDEX_PROFIT_GUID];

            this.UsedClassifiers = new IClassifier[] { };
            this.UsedFacts = new IFactTable[] { fctIndexProfit };
        }

        protected override void UpdateData()
        {
            UpdateDataSet(daOrg, dsOrg, clsOrg);
            UpdateDataSet(daIndexProfit, dsIndexProfit, fctIndexProfit);
        }

        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsIndexProfit);
            ClearDataSet(ref dsOrg);
            ClearDataSet(ref dsActivity);
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
                object value = mapping[i + 1];
                if ((value != null) && (value != DBNull.Value) && (Convert.ToDecimal(value) != 0))
                    return false;
            }
            return true;
        }

        private int GetRefActivity(string activityName)
        {
            string code = activityName.Split(' ')[0].Trim();
            return FindCachedRow(cachceActivity, code, nullActivity);
        }

        private int[] XLS_FACTS_COLUMNS = new int[] { 5, 6, 7, 8, 9 };
        private void PumpXlsFacts(ExcelHelper excelDoc, int firstRow, int refRegions, int refOrg)
        {
            int refAcitvity = GetRefActivity(excelDoc.GetValue(firstRow + 4, 2));
            decimal deduct = CleanFactValue(excelDoc.GetValue(firstRow + 5, 2));

            for (int i = 0; i < 5; i++)
            {
                int curColumn = XLS_FACTS_COLUMNS[i];

                object[] mapping = new object[] {
                    "SaleProfit", CleanFactValue(excelDoc.GetValue(firstRow + 8, curColumn)) * 1000,
                    "BaseProfit", CleanFactValue(excelDoc.GetValue(firstRow + 9, curColumn)) * 1000,
                    "AddProfit", CleanFactValue(excelDoc.GetValue(firstRow + 10, curColumn)) * 1000,
                    "TransferProfit", CleanFactValue(excelDoc.GetValue(firstRow + 11, curColumn)) * 1000,
                    "OldTransferProfit", CleanFactValue(excelDoc.GetValue(firstRow + 12, curColumn)) * 1000,
                    "TotalTransfer", CleanFactValue(excelDoc.GetValue(firstRow + 13, curColumn)) * 1000,
                    "DebtProfit", CleanFactValue(excelDoc.GetValue(firstRow + 14, curColumn)) * 1000,
                    "OldDebtProfit", CleanFactValue(excelDoc.GetValue(firstRow + 15, curColumn)) * 1000,
                    "TotalDebt", CleanFactValue(excelDoc.GetValue(firstRow + 16, curColumn)) * 1000
                };

                if (SumsIsNull(mapping))
                    continue;

                mapping = (object[])CommonRoutines.ConcatArrays(mapping, new object[] {
                    "Deduct", deduct, "RefYearDayUNV", refDates[i], "RefRegions", refRegions,
                    "RefOrg", refOrg, "RefActivityStatus", refAcitvity, "SourceID", sourceID });

                PumpRow(dsIndexProfit.Tables[0], mapping);
                if (dsIndexProfit.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
                {
                    UpdateData();
                    ClearDataSet(daIndexProfit, ref dsIndexProfit);
                }
            }
        }

        private int PumpXlsOrg(ExcelHelper excelDoc, int firstRow)
        {
            string innStr = excelDoc.GetValue(firstRow + 2, 2).Trim();
            string kppStr = excelDoc.GetValue(firstRow + 3, 2).Trim();
            if ((innStr == string.Empty) && (kppStr == string.Empty))
                return nullOrg;

            string name = excelDoc.GetValue(firstRow + 1, 2).Trim();
            if (name.Length > 255)
                name = name.Substring(0, 255);
            string note = excelDoc.GetValue(firstRow + 6, 2).Trim();
            if (note.Length > 255)
                note = note.Substring(0, 255);

            long inn = Convert.ToInt64(CommonRoutines.TrimLetters(innStr).PadLeft(1, '0'));
            long kpp = Convert.ToInt64(CommonRoutines.TrimLetters(kppStr).PadLeft(1, '0'));

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
                string cellValue = excelDoc.GetValue(firstRow, 1).Trim();
                if (cellValue == string.Empty)
                {
                    emptyStrCount++;
                    continue;
                }
                emptyStrCount = 0;

                if (cellValue.ToUpper() == "БЛОК ПОКАЗАТЕЛЕЙ ДЛЯ МУП")
                    return true;
            }
            return false;
        }

        private void PumpXlsSheet(ExcelHelper excelDoc, int refRegions)
        {
            int firstRow = 1;
            while (GetBlockFirstRow(excelDoc, ref firstRow))
                try
                {
                    int refOrg = PumpXlsOrg(excelDoc, firstRow);
                    PumpXlsFacts(excelDoc, firstRow, refRegions, refOrg);
                    firstRow++;
                }
                catch (Exception ex)
                {
                    throw new PumpDataFailedException(string.Format(
                        "При обработке листа '{0}' возникла ошибка ({1})",
                        excelDoc.GetWorksheetName(), ex.Message));
                }
        }

        private void SetReportType(string filename)
        {
            filename = filename.ToUpper();
            if (filename.Contains("GO"))
                reportType = ReportType.IndexProfitGO;
            if (filename.Contains("MR"))
                reportType = ReportType.IndexProfitMR;
        }

        private void DeleteEarlierDataByRegions(int refRegions)
        {
            if (!deletedRegionsList.Contains(refRegions) && !this.DeleteEarlierData)
            {
                string constr = string.Format("RefRegions = {0}", refRegions);
                DirectDeleteFactData(new IFactTable[] { fctIndexProfit }, -1, sourceID, constr);
                deletedRegionsList.Add(refRegions);
            }
        }

        private void FillRefDates(string filename)
        {
            // из названия файла mupГГ_go_ННН.xls или mupГГ_mr_ННН.xls берем год - ГГ
            int year = Convert.ToInt32(CommonRoutines.TrimLetters(filename.Split('_')[0])) + 2000;
            refDates[0] = year * 10000 + 9991;
            refDates[1] = year * 10000 + 9992;
            refDates[2] = year * 10000 + 9993;
            refDates[3] = year * 10000 + 9994;
            refDates[4] = year * 10000 + 1;
        }

        private int GetRefRegions(string value)
        {
            string codeLine = string.Empty;
            switch (reportType)
            {
                case ReportType.IndexProfitGO:
                    fullPump = true;
                    // из названия файла mupГГ_go_ННН.xls берем ННН
                    codeLine = CommonRoutines.TrimLetters(value.Split('_')[2].Trim());
                    break;
                case ReportType.IndexProfitMR:
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
                FillRefDates(file.Name);

                excelDoc.AskToUpdateLinks = false;
                excelDoc.DisplayAlerts = false;
                excelDoc.EnableEvents = false;
                excelDoc.OpenDocument(file.FullName);

                switch (reportType)
                {
                    case ReportType.IndexProfitGO:
                        excelDoc.SetWorksheet(1);
                        PumpXlsSheet(excelDoc, GetRefRegions(file.Name));
                        break;
                    case ReportType.IndexProfitMR:
                        int wsCount = excelDoc.GetWorksheetsCount();
                        for (int index = 1; index <= wsCount; index++)
                        {
                            excelDoc.SetWorksheet(index);
                            if (SkipSheet(excelDoc.GetWorksheetName()))
                                continue;
                            PumpXlsSheet(excelDoc, GetRefRegions(excelDoc.GetWorksheetName()));
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
                    clsOrg.FullDBName, fctIndexProfit.FullDBName);
                using (DataTable dt = (DataTable)this.DB.ExecQuery(query, QueryResultTypes.DataTable, new IDbDataParameter[] { }))
                {
                    foreach (DataRow row in dt.Rows)
                        deletedOrgs.Add(string.Format("{0} ({1})", row["INN"], row["Name"]));
                }

                if (deletedOrgs.Count == 0)
                    return;

                query = string.Format(
                    "delete from {0} where not ID in (select distinct RefOrg from {1}) and ID > 0",
                    clsOrg.FullDBName, fctIndexProfit.FullDBName);
                this.DB.ExecQuery(query, QueryResultTypes.NonQuery, new IDbDataParameter[] { });

                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, string.Format(
                    "Из классификатора '{0}' удалены организации, по которым нет данных в таблице фактов '{1}': {2}.",
                    clsOrg.FullCaption, fctIndexProfit.FullCaption, string.Join(", ", deletedOrgs.ToArray())));
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
