using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Krista.FM.Server.DataPumps;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.MOFO24Pump
{

    // МОФО - 0024 - Суммы НП к доплате (уменьшению)
    public class MOFO24PumpModule : CorrectedPumpModuleBase
    {

        #region Поля

        #region Классификаторы

        // Организации.МОФО_НП к доплате_уменьшению (d_Org_TaxBenPay)
        private IDbDataAdapter daOrg;
        private DataSet dsOrg;
        private IClassifier clsOrg;
        private Dictionary<string, int> cacheOrg = null;
        private Dictionary<string, int> cacheOrgInnName = null;
        private int nullOrg = -1;
        // Районы.Планирование (d_Regions_Plan)
        private IDbDataAdapter daRegions;
        private DataSet dsRegions;
        private IClassifier clsRegions;
        private Dictionary<string, int> cacheRegions = null;

        #endregion Классификаторы

        #region Факты

        // Показатели.МОФО_НП к доплате_уменьшению (f_Marks_TaxBenPay)
        private IDbDataAdapter daTaxBenPay;
        private DataSet dsTaxBenPay;
        private IFactTable fctTaxBenPay;

        #endregion Факты

        private ReportType reportType;
        private int prevRefOrg = -1;
        private int sourceID = -1;
        private List<int> deletedSourceIDList = null;
        private List<string> deletedRegionsList = null;

        #endregion Поля

        #region Перечисления

        private enum ReportType
        {
            // данные по городским округам (файлы np_go_ГГКNNN.xls)
            TaxBenPayGO,
            // данные по муниципальным районам (файлы np_mr_ГГКNNN.xls)
            TaxBenPayMR
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
                DirectDeleteFactData(new IFactTable[] { fctTaxBenPay }, -1, sourceID, string.Empty);
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
            FillRowsCache(ref cacheOrg, dsOrg.Tables[0], "INN");
            FillRowsCache(ref cacheOrgInnName, dsOrg.Tables[0], new string[] { "INN", "Name" }, "|", "ID");
            FillRegionsCache(dsRegions.Tables[0]);
        }

        protected override void QueryData()
        {
            SetNewSourceID();

            InitDataSet(ref daOrg, ref dsOrg, clsOrg, string.Empty);
            InitDataSet(ref daRegions, ref dsRegions, clsRegions, string.Format("SourceID = {0}", sourceID));
            InitFactDataSet(ref daTaxBenPay, ref dsTaxBenPay, fctTaxBenPay);

            FillCaches();
        }

        #region GUIDs

        private const string D_ORG_TAXBENPAY_GUID = "3cb29958-a461-4c4b-b8dd-3f6ff0f67982";
        private const string D_REGIONS_PLAN_GUID = "1f34cc90-16fd-4fcf-b994-0c8a680d7e23";
        private const string F_MARKS_TAXBENPAY_GUID = "d48a95e7-e323-4e22-87be-e0599e62e47f";

        #endregion GUIDs
        protected override void InitDBObjects()
        {
            clsOrg = this.Scheme.Classifiers[D_ORG_TAXBENPAY_GUID];
            clsRegions = this.Scheme.Classifiers[D_REGIONS_PLAN_GUID];
            fctTaxBenPay = this.Scheme.FactTables[F_MARKS_TAXBENPAY_GUID];

            this.UsedClassifiers = new IClassifier[] { };
            this.UsedFacts = new IFactTable[] { fctTaxBenPay };
        }

        protected override void UpdateData()
        {
            UpdateDataSet(daOrg, dsOrg, clsOrg);
            UpdateDataSet(daTaxBenPay, dsTaxBenPay, fctTaxBenPay);
        }

        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsTaxBenPay);
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

        private void PumpFactRow(string valuePayment, string valueReduction, int refRegions,
            int refDate, int refOrg, int refMarks, int refBudgetLevel)
        {
            decimal sumPayment = CleanFactValue(valuePayment);
            decimal sumReduction = CleanFactValue(valueReduction);
            if ((sumPayment == 0) && (sumReduction == 0))
                return;

            object[] mapping = new object[] {
                "SumPayment", sumPayment * 1000,
                "SumReduction", sumReduction * 1000,
                "RefRegions", refRegions,
                "RefYearDayUNV", refDate,
                "RefOrg", refOrg,
                "RefMarks", refMarks,
                "RefBdgLevels", refBudgetLevel,
                "SourceID", sourceID
            };

            PumpRow(dsTaxBenPay.Tables[0], mapping);
            if (dsTaxBenPay.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
            {
                UpdateData();
                ClearDataSet(daTaxBenPay, ref dsTaxBenPay);
            }
        }

        private int PumpXlsOrgNullInn(string name)
        {
            if (name == string.Empty)
                return nullOrg;

            string key = string.Format("0|{0}", name);
            if (cacheOrgInnName.ContainsKey(key))
                return cacheOrgInnName[key];

            // необходимо также проверить случай, когда к наименованиею приписан ИНН
            if (cacheOrgInnName.ContainsKey(string.Format("{0} (0)", key)))
                return cacheOrgInnName[string.Format("{0} (0)", key)];

            return PumpCachedRow(cacheOrgInnName, dsOrg.Tables[0], clsOrg, key, new object[] { "INN", 0, "Name", name });
        }

        private int PumpXlsOrg(ExcelHelper excelDoc, int curRow)
        {
            string cellValue = excelDoc.GetValue(curRow, 2).Trim().ToUpper();
            if (!cellValue.Contains("В ТОМ ЧИСЛЕ"))
                return prevRefOrg;

            string innStr = excelDoc.GetValue(curRow, 4).Trim();
            string name = excelDoc.GetValue(curRow, 5).Trim();
            if (innStr == string.Empty)
            {
                prevRefOrg = nullOrg;
                return nullOrg;
            }

            int refOrg = nullOrg;
            long inn = Convert.ToInt64(CommonRoutines.TrimLetters(innStr).PadLeft(1, '0'));
            if (inn == 0)
            {
                // качаем, когда ИНН = 0 (в этом случае ключом кэша будет ИНН+Наименование)
                refOrg = PumpXlsOrgNullInn(name);
            }
            else
            {
                refOrg = PumpCachedRow(cacheOrg, dsOrg.Tables[0], clsOrg, inn.ToString(),
                    new object[] { "INN", inn, "Name", name });
            }
            prevRefOrg = refOrg;

            return refOrg;
        }

        private int GetRefMarks(string cellValue)
        {
            return Convert.ToInt32(cellValue.Split('.')[0].Trim());
        }

        private int GetRefBudgetLevel(string cellValue)
        {
            cellValue = cellValue.Trim().ToUpper();
            if (cellValue.Contains("ФЕДЕРАЛЬНЫЙ БЮДЖЕТ"))
                return 1;
            if (cellValue.Contains("ОБЛАСТНОЙ БЮДЖЕТ"))
                return 2;
            return 0;
        }

        private void PumpXlsRow(ExcelHelper excelDoc, int curRow, int refDate, int refRegions)
        {
            int refOrg = PumpXlsOrg(excelDoc, curRow);
            int refMarks = GetRefMarks(excelDoc.GetValue(curRow, 1));
            int refBudgetLevel = GetRefBudgetLevel(excelDoc.GetValue(curRow, 2));

            string valuePayment = excelDoc.GetValue(curRow, 6);
            string valueReduction = excelDoc.GetValue(curRow, 7);
            PumpFactRow(valuePayment, valueReduction, refRegions, refDate, refOrg, refMarks, refBudgetLevel);
        }

        private void DeleteEarlierDataByDateAndRegions(int refDate, int refRegions)
        {
            string key = string.Format("{0}|{1}", refDate, refRegions);
            if (!deletedRegionsList.Contains(key) && !this.DeleteEarlierData)
            {
                string constr = string.Format("RefYearDayUNV = {0} and RefRegions = {1}", refDate, refRegions);
                DirectDeleteFactData(new IFactTable[] { fctTaxBenPay }, -1, sourceID, constr);
                deletedRegionsList.Add(key);
            }
        }

        private void PumpXlsSheet(ExcelHelper excelDoc, int refDate, int refRegions)
        {
            DeleteEarlierDataByDateAndRegions(refDate, refRegions);

            prevRefOrg = nullOrg;
            bool toPump = false;
            // т.к. ячейки листа заблокированы, то функция, возвращая кол-во строк, не работает
            // поэтому качаем до тех пор, пока не будет 10 пустых строк подряд
            int emptyStrCount = 0;
            for (int curRow = 1; emptyStrCount <= 10; curRow++)
                try
                {
                    string cellValue = excelDoc.GetValue(curRow, 1).Trim();
                    if (cellValue == string.Empty)
                    {
                        emptyStrCount++;
                        toPump = false;
                        continue;
                    }
                    emptyStrCount = 0;

                    if ((cellValue == "1") && !toPump)
                    {
                        toPump = true;
                        continue;
                    }

                    if (toPump)
                    {
                        PumpXlsRow(excelDoc, curRow, refDate, refRegions);
                    }
                }
                catch (Exception ex)
                {
                    throw new PumpDataFailedException(string.Format(
                        "При обработке строки {0} листа '{1}' возникла ошибка ({2})",
                        curRow, excelDoc.GetWorksheetName(), ex.Message));
                }
        }

        private void SetReportType(string filename)
        {
            filename = filename.ToUpper();
            if (filename.Contains("GO"))
                reportType = ReportType.TaxBenPayGO;
            if (filename.Contains("MR"))
                reportType = ReportType.TaxBenPayMR;
        }

        private int GetRefRegions(string value)
        {
            string codeLine = string.Empty;
            switch (reportType)
            {
                case ReportType.TaxBenPayGO:
                    // из названия файла np_go_ГГКNNN.xls берем NNN
                    codeLine = CommonRoutines.TrimLetters(value.Split('_')[2].Trim());
                    codeLine = codeLine.Substring(3);
                    break;
                case ReportType.TaxBenPayMR:
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

            return cacheRegions[codeLine];
        }

        private int GetRefDate(string filename)
        {
            // из названия файла np_go_ГГКNNN.xls или np_mr_ГГКNNN.xls берем ГГ - год, К - квартал
            string strDate = filename.Split('_')[2].Trim().Substring(0, 3);
            int year = Convert.ToInt32(strDate.Substring(0, 2)) + 2000;
            int quarter = Convert.ToInt32(strDate.Substring(2, 1));
            return year * 10000 + 9990 + quarter;
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
                    case ReportType.TaxBenPayGO:
                        excelDoc.SetWorksheet(1);
                        PumpXlsSheet(excelDoc, refDate, GetRefRegions(file.Name));
                        break;
                    case ReportType.TaxBenPayMR:
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

        protected override void DeleteEarlierPumpedData()
        {
            // заглушка
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

        protected override void ProcessFiles(DirectoryInfo dir)
        {
            deletedRegionsList = new List<string>();
            try
            {
                ProcessFilesTemplate(dir, "*.xls", new ProcessFileDelegate(PumpXlsFile), false);
                UpdateData();
                CorrectOrgsNames();
            }
            finally
            {
                deletedRegionsList.Clear();
            }
        }

        protected override void DirectPumpData()
        {
            PumpDataYQTemplate();
        }

        #endregion Перекрытые методы

        #endregion Закачка данных

    }

}
