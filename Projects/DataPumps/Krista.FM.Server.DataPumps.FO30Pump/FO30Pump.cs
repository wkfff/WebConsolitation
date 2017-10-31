using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.FO30Pump
{

    // ФО 0030 - Информация из муниципальных долговых книг
    public class FO30PumpModule : CorrectedPumpModuleBase
    {
        #region Константы
        #endregion

        #region Структуры, перечисления
        #endregion Структуры, перечисления

        #region Поля

        #region Классификаторы

        //Районы.Анализ
        private IDbDataAdapter daRegions;
        private DataSet dsRegions;
        private IClassifier clsRegions;
        private Dictionary<string, int> cacheRegions = null;

        //Вариант.Долговая книга (d.Variant.Schuldbuch)
        private IDbDataAdapter daVariant;
        private DataSet dsVariant;
        private IClassifier clsVariant;
        private Dictionary<string, int> cacheVariant = null;

        //Виды ценных бумаг (d.S.Capital)
        private IDbDataAdapter daCapital;
        private DataSet dsCapital;
        private IClassifier clsCapital;
        private Dictionary<int, int> cacheCapital = null;

        #endregion Классификаторы

        #region Факты
        //Ценные бумаги (f.S.SchBCapital)
        private IDbDataAdapter daSchBCapital;
        private DataSet dsSchBCapital;
        private IFactTable fctSchBCapital;

        //Кредиты полученные (f.S.SchBCreditincome)
        private IDbDataAdapter daSchBCreditIncome;
        private DataSet dsSchBCreditIncome;
        private IFactTable fctSchBCreditIncome;

        //Гарантии предоставленные (f.S.SchBGuarantissued)
        private IDbDataAdapter daSchBGuarantIssued;
        private DataSet dsSchBGuarantIssued;
        private IFactTable fctSchBGuarantIssued;

        #endregion Факты

        private int sectionIndex = -1;
        int variantCode = 0;
        int sourceID;


        private List<string> deletedDatesAndRegionsList = null;

        #endregion Поля

        #region Закачка данных

        #region Работа с базой и кэшами

        protected override void QueryData()
        {

            sourceID = this.AddDataSource("ФО", "0006", ParamKindTypes.Year, string.Empty,
                this.DataSource.Year, 0, string.Empty, 0, string.Empty).ID;
            InitDataSet(ref daRegions, ref dsRegions, clsRegions, String.Format("SourceID = {0}", sourceID));

            InitDataSet(ref daVariant, ref dsVariant, clsVariant, string.Empty);
            InitDataSet(ref daCapital, ref dsCapital, clsCapital, string.Empty);
            InitFactDataSet(ref daSchBCapital, ref dsSchBCapital, fctSchBCapital);
            InitFactDataSet(ref daSchBCreditIncome, ref dsSchBCreditIncome, fctSchBCreditIncome);
            InitFactDataSet(ref daSchBGuarantIssued, ref dsSchBGuarantIssued, fctSchBGuarantIssued);
            FillCaches();
            
            //порядковый код
            Object res = this.DB.ExecQuery("select max(Code) from d_Variant_Schuldbuch", null, QueryResultTypes.Scalar);
            if (res == DBNull.Value)
                variantCode = 1;
            else variantCode = Convert.ToInt32(res);
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
            FillRegionsCache(dsRegions.Tables[0]);
            FillRowsCache(ref cacheVariant, dsVariant.Tables[0], "ReportDate", "ID");
            FillRowsCache(ref cacheCapital, dsCapital.Tables[0], "Code", "ID");
        }

        protected override void UpdateData()
        {
            UpdateDataSet(daRegions, dsRegions, clsRegions);
            UpdateDataSet(daVariant, dsVariant, clsVariant);
            UpdateDataSet(daCapital, dsCapital, clsCapital);
            UpdateDataSet(daSchBCapital, dsSchBCapital, fctSchBCapital);
            UpdateDataSet(daSchBCreditIncome, dsSchBCreditIncome, fctSchBCreditIncome);
            UpdateDataSet(daSchBGuarantIssued, dsSchBGuarantIssued, fctSchBGuarantIssued);
        }

        private const string D_REGIONS_ANALYSIS_GUID = "383f887a-3ebb-4dba-8abb-560b5777436f";
        private const string D_VARIANT_SCHULDBUCH_GUID = "f37827df-c22a-4569-9512-c0c48791d46c";
        private const string D_S_CAPITAL_GUID = "883bf07d-1460-4f1e-b92d-15e2c5b9b51f";

        private const string F_SCHBCAPITAL_GUID = "328a93cf-9769-4980-97e3-32570636b125";
        private const string F_SCHBCREDITINCOME_GUID = "43c55c92-c819-4e0b-95a1-3b941bc2789f";
        private const string F_SCHBGUARANTISSUED_GUID = "6930d45e-89a3-4f28-b1c4-b28502593750";

        protected override void InitDBObjects()
        {
            clsRegions = this.Scheme.Classifiers[D_REGIONS_ANALYSIS_GUID];
            clsVariant = this.Scheme.Classifiers[D_VARIANT_SCHULDBUCH_GUID];
            clsCapital= this.Scheme.Classifiers[D_S_CAPITAL_GUID];
            fctSchBCapital = this.Scheme.FactTables[F_SCHBCAPITAL_GUID];
            fctSchBCreditIncome = this.Scheme.FactTables[F_SCHBCREDITINCOME_GUID];
            fctSchBGuarantIssued = this.Scheme.FactTables[F_SCHBGUARANTISSUED_GUID];

            this.UsedClassifiers = new IClassifier[] {clsVariant};
            this.UsedFacts = new IFactTable[] { fctSchBCapital, fctSchBCreditIncome, fctSchBGuarantIssued };
        }

        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsRegions);
            ClearDataSet(ref dsVariant);
            ClearDataSet(ref dsCapital);
            ClearDataSet(ref dsSchBCapital);
            ClearDataSet(ref dsSchBCreditIncome);
            ClearDataSet(ref dsSchBGuarantIssued);
        }

        #endregion Работа с базой и кэшами

        #region Перекрытые методы закачки

        /// <summary>
        /// удаляет данные по дате и коду организации
        /// </summary>
        /// <param name="code"></param>
        /// 
        private void DeleteEarlierDataByDateAndRegions(int refVariant, int refRegions)
        {
            string key = string.Format("{0}|{1}", refVariant, refRegions);
            if (!deletedDatesAndRegionsList.Contains(key))
            {
                DirectDeleteFactData(this.UsedFacts, -1, this.SourceID, string.Format("RefRegion = {0} and  RefVariant = {1}", refRegions, refVariant));
                deletedDatesAndRegionsList.Add(key);
            }
        }

        private int GetRefRegions(string value)
        {
            string codeLine = string.Empty;
            codeLine = CommonRoutines.TrimLetters(value);
            if (Convert.ToInt32(codeLine.Substring(codeLine.Length - 2)) == 0)
                codeLine = codeLine.Substring(0, 3);

            if (!cacheRegions.ContainsKey(codeLine))
            {
                throw new PumpDataFailedException(string.Format(
                    "Не найдено муниципальное образование с кодом {0} в справочнике «Районы.Анализ».", codeLine));
            }

            return cacheRegions[codeLine];
        }

        private int GetRefCapital()
        {
            if (!cacheCapital.ContainsKey(300))
            {
                throw new PumpDataFailedException(string.Format(
                    "Не найдено ценной бумаги с кодом {0} в справочнике «ИФ.Виды ценных бумаг».", 300));
            }
            return cacheCapital[300];
        }

        private object[] XLS_MAPPING_SECTION1 = new object[] {
            "OfficialNumber", 3, "FormCap", 5, "RegNumber", 6, "RegEmissionDate", 7, "NameNPA", 8,
            "NameOrg", 9, "DateNPA", 10, "NumberNPA", 11, "Sum", 13, "StartDate", 14, "Owner", 15,
            "Nominal", 16, "DateDischarge", 17, "DatePartDischarge", 18, "IssueSum", 19, "Discharge", 20,
            "DueDate", 21, "Coupon", 22, "Income", 23, "FactServiceSum", 24, "Discount", 25,
            "FactDiscountSum", 26, "TotalSum", 27, "GenAgent", 28, "Depository", 29, "Trade", 30,
            "StalePenlt", 31, "StaleDebt", 32, "StaleInterest", 33, "Attract", 34};

        private object[] XLS_MAPPING_SECTION2 = new object[] {
            "Occasion", 3, "ContractDate", 4, "Num", 5, "OldAgreementDate", 6, "OldAgrNum", 7, "OffsetDate", 8,
            "RegNum", 9, "RenewalDate", 10, "RenwlNum", 11, "DebtStartDate", 13, "FurtherConvention", 14, "ComprAgreeDate", 15,
            "ComprAgrNum", 16, "Creditor", 17, "ChargeDate", 18, "Sum", 19, "CreditPercent", 20,
            "PaymentDate", 21, "StaleInterest", 22, "RemnsEndMnthDbt", 23, "StaleDebt", 24, "Attract", 25};

        private object[] XLS_MAPPING_SECTION4 = new object[] {
            "Occasion", 3, "ContractDate", 4, "Num", 5, "OldAgreementDate", 6, "OldAgrNum", 7, "OffsetDate", 8,
            "RegNum", 9, "RenewalDate", 10, "RenwlNum", 11, "DebtStartDate", 13, "FurtherConvention", 14, "ComprAgreeDate", 15,
            "ComprAgrNum", 16, "Creditor", 17, "Sum", 18, "ChargeDate", 19, "PaymentDate", 20, "StaleDebt", 21, "Attract", 22};

        private object[] XLS_MAPPING_SECTION5 = new object[] {
            "Occasion", 3, "Purpose", 4, "ContractDate", 5, "Num", 6, "OldAgreementDate", 8, "OldAgrNum", 9,
            "RenewalDate", 10, "RenwlNum", 11, "ComprAgreeDate", 12, "FurtherConvention", 13, "Collateral", 14,
            "Creditor", 15, "Sum", 16, "StartDate", 17, "EndDate", 18, "StaleDebt", 19, "CapitalDebt", 20};

        private object[] XLS_MAPPING_SECTION3 = new object[] {
            "Num", 4, "StartDate", 3, "OldAgreementDate", 5, "OldAgrNum", 6, "RenewalDate", 7, "RenewalNum", 8,
            "PrincipalStartDate", 9, "PrincipalNum", 10, "Garant", 12, "Principal", 13, "Creditor", 14, "StartCreditDate", 15,
            "ChargeDate", 16, "EndCreditDate", 17, "PrincipalEndDate", 18, "TotalDebt", 19, "CapitalDebt", 20,
            "DownDebt", 21, "DownGarant", 22, "StalePrincipalDebt", 23, "RemnsEndMnthDbt", 24, "BgnYearDebt", 25};

        object[] GetXlsMapping()
        {
            if (sectionIndex == 1)
                return XLS_MAPPING_SECTION1;
            else if (sectionIndex == 2)
                return XLS_MAPPING_SECTION2;
            else if (sectionIndex == 3)
                return XLS_MAPPING_SECTION3;
            else if (sectionIndex == 4)
                return XLS_MAPPING_SECTION4;
            else return XLS_MAPPING_SECTION5;
        }

        DataSet GetDataSetBySection()
        {
            if (sectionIndex == 1)
                return dsSchBCapital;
            if (sectionIndex == 3)
                return dsSchBGuarantIssued;
            return dsSchBCreditIncome;
        }
        
        IDbDataAdapter GetDataAdapterBySection()
        {
            if (sectionIndex == 0)
                return daSchBCapital;
            if (sectionIndex == 3)
                return daSchBGuarantIssued;
            return daSchBCreditIncome;
        }

        private void PumpXLSRow(ExcelHelper excelDoc, int curRow, int refRegions, int refVariant)
        {
            if (sectionIndex == 0)
                return;

            object[] xlsMapping = GetXlsMapping();

            object[] mapping = new object[]{};

            bool emptyRow = true;

            for (int i = 0; i < xlsMapping.Length; i += 2)
            {
                string value = excelDoc.GetValue(curRow, (int)xlsMapping[i + 1]).Trim();
                if (value != string.Empty)
                {
                    emptyRow = false;
                    mapping = (object[])CommonRoutines.ConcatArrays(mapping, new object[] { xlsMapping[i], value });
                }
            }

            if (emptyRow)
                return;

            mapping = (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "RefRegion", refRegions, "RefOKV", -1, "RefVariant", refVariant });
            
            if (sectionIndex == 1)
                mapping = (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "RefSCap", GetRefCapital() });
            else if ((sectionIndex == 2) || (sectionIndex == 4) || (sectionIndex == 5))
            {
                int refTypeCredit = 0;
                if (sectionIndex == 4)
                    refTypeCredit = 1;
                else if (sectionIndex == 5)
                    refTypeCredit = 5;

                mapping = (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "RefTypeCredit ", refTypeCredit });
            }

            PumpRow(GetDataSetBySection().Tables[0], mapping);
            if (GetDataSetBySection().Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
            {
                UpdateData();
                DataSet ds = GetDataSetBySection();
                ClearDataSet(GetDataAdapterBySection(), ref ds);
            }
        }

        private int PumpVariant()
        {
            DateTime date;
            if ((this.DataSource.Month>0) && (this.DataSource.Month < 12))
                date = new DateTime(this.DataSource.Year,this.DataSource.Month+1, 1);
            else date = new DateTime(this.DataSource.Year+1, 1, 1);
            object[] mapping = new object[] { "ReportDate", date, "name", "Долговая книга за " + CommonRoutines.MonthByNumber[this.DataSource.Month], "Code", ++variantCode, "CurrentVariant", 0, "ActualYear", this.DataSource.Year, "VariantComment", "Долговая книга за " + CommonRoutines.MonthByNumber[this.DataSource.Month] };
            return PumpCachedRow(cacheVariant, dsVariant.Tables[0], clsVariant, mapping, date.ToString(), "ID");
        }

        private void PumpXlsSheet(ExcelHelper excelDoc, string fileName, int startRow)
        {
            int curRow = startRow;
            int countRows = excelDoc.GetRowsCount();
            string sheetName = excelDoc.GetWorksheetName();
            int refRegions = 0;
            string strValue;
            int refVariant = PumpVariant();

            curRow = curRow - 1;

            while (true)
            {
                try
                {
                    curRow++;
                    SetProgress(countRows, curRow,
                        string.Format("Обработка файла {0}...", fileName),
                        string.Format("Строка {0} из {1} листа '{2}'", curRow, countRows, sheetName));
                    strValue = excelDoc.GetValue(curRow, 2).Trim();
                    if (strValue.ToUpper().Contains("ИТОГО"))
                        continue;
                    if (strValue.ToUpper().Contains("ВНЕШНИЙ") && strValue.ToUpper().Contains("ДОЛГ"))
                        continue;
                    if (strValue.ToUpper().Contains("ВСЕГО"))
                        return;

                    if (strValue != string.Empty)
                    {
                        refRegions = GetRefRegions(strValue);
                        DeleteEarlierDataByDateAndRegions(refRegions, refVariant);
                        continue;
                    }

                    PumpXLSRow(excelDoc, curRow, refRegions, refVariant);
                }

                catch (Exception ex)
                {
                    throw new Exception(string.Format("При обработке строки {0} листа '{3}' отчета {1} возникла ошибка ({2})",
                        curRow, fileName, ex.Message, sheetName), ex);
                }
            }
        }

        private int GetStartRow(int section)
        {
            if (section <= 0)
                return -1;
            return 16;
        }

        private int GetSectionIndex(string sectionName)
        {
            if (sectionName == "TITUL")
                return 0;
            else return Convert.ToInt32(CommonRoutines.RemoveLetters(sectionName.Trim()));
        }

        private void PumpXLSFile(FileInfo file)
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
                    string wsName = excelDoc.GetWorksheetName().Trim().ToUpper();
                    sectionIndex = GetSectionIndex(wsName);
                    if (sectionIndex == 0 )
                        continue;
                    PumpXlsSheet(excelDoc, file.Name, GetStartRow(sectionIndex));
                }
            }
            finally
            {
                if (excelDoc != null)
                    excelDoc.CloseDocument();
            }

        }

        protected override void ProcessFiles(DirectoryInfo dir)
        {
            deletedDatesAndRegionsList = new List<string>();
            try
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStart, "Старт инициализации Excel.");
                ProcessFilesTemplate(dir, "*.xls", new ProcessFileDelegate(PumpXLSFile), false);
                UpdateData();
            }
            finally
            {
                deletedDatesAndRegionsList.Clear();
            }
        }

        protected override void DirectPumpData()
        {
            PumpDataYMTemplate();
        }

        #endregion Перекрытые методы закачки

        #endregion Закачка данных
    }
}