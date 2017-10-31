using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using Krista.FM.Server.Common;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.UFK10Pump
{

    // УФК - 0010 - Доходы по расчетам налогоплательщиков
    public class UFK10PumpModule : CorrectedPumpModuleBase
    {

        #region Поля

        #region Классификаторы

        // КД.УФК (d_KD_UFK)
        private IDbDataAdapter daKd;
        private DataSet dsKd;
        private IClassifier clsKd;
        private Dictionary<string, int> cacheKd = null;
        // Организации.УФК_Плательщики (d_Org_UFKPayers)
        private IDbDataAdapter daOrg;
        private DataSet dsOrg;
        private IClassifier clsOrg;
        private Dictionary<string, int> cacheOrg = null;
        // ОКАТО.УФК (d_OKATO_UFK)
        private IDbDataAdapter daOkato;
        private DataSet dsOkato;
        private IClassifier clsOkato;
        private Dictionary<string, int> cacheOkato = null;
        // Организации.Сопоставимый (b_Organizations_Bridge)
        private IDbDataAdapter daOrgBridge;
        private DataSet dsOrgBridge;
        private IClassifier clsOrgBridge;
        private Dictionary<string, int> cacheOrgBridge = null;
        // Тип.Налоговый платеж (d_Kind_TaxPayment)
        private IDbDataAdapter daTax;
        private DataSet dsTax;
        private IClassifier clsTax;
        private Dictionary<string, int> cacheTax = null;
        // Период.Соответствие операционных дней (d_Date_ConversionFK)
        private IDbDataAdapter daPeriod;
        private DataSet dsPeriod;
        private IClassifier clsPeriod;
        private Dictionary<int, int> cachePeriod = null;

        #endregion Классификаторы

        #region Факты

        // Доходы.УФК_Налогоплательщики_Платежные поручения (f_D_UFK10Drafts)
        private IDbDataAdapter daDrafts;
        private DataSet dsDrafts;
        private IFactTable fctDrafts;
        // Доходы.УФК_Налогоплательщики_Поступления в бюджеты (f_D_UFK10Taxpayers)
        private IDbDataAdapter daPayers;
        private DataSet dsPayers;
        private IFactTable fctPayers;
        // Доходы.УФК_Налогоплательщики_Возвраты и зачеты (f_D_UFK10Repayment)
        private IDbDataAdapter daRepayment;
        private DataSet dsRepayment;
        private IFactTable fctRepayment;

        #endregion Факты

        private int curRow;
        private Database dbfDatabase = null;
        protected DBDataAccess dbDataAccess = new DBDataAccess();
        private int clsSourceId;
        private List<string> deletedDateList = null;
        private List<int> deletedDateListCsv = null;
        // список закачиваемых дат, сгруппированных по SourceId
        // используется при расщеплении
        private Dictionary<int, List<string>> pumpedDateList = null;
        private int year = -1;
        private int month = -1;
        private bool disintAll = false;
        private bool toProcessOrgCls = false;
        private bool toDisintData = false;
        private bool toFillOrgBridgeCls = false;
        private ReportType reportType;
        bool isFinalOverturn = false;
        int finalOverturnDate = 0;
        // версия сопоставимого классификатора
        private int? bridgeClsSourceID = -1;
        private bool bridgePumpedFromDbf = false;

        #endregion Поля

        #region Структуры, перечисления

        private enum ReportType
        {
            // зачеты внутри счета - в таблицу "Доходы.УФК_Налогоплательщики_Возвраты и зачеты"
            CC,
            // платежные поручения - в таблицу "Доходы.УФК_Налогоплательщики_Платежные поручения"
            PP,
            // возвраты - в таблицу "Доходы.УФК_Налогоплательщики_Возвраты и зачеты"
            VV,
            // зачеты - в таблицу "Доходы.УФК_Налогоплательщики_Возвраты и зачеты"
            ZZ
        }

        #endregion Структуры, перечисления

        #region Делегаты

        private delegate void ProcessDbfDelegate(DataRow row, int fileDate);

        // Функция обработки строки Csv-отчёта
        private delegate void PumpCsvDataRow(Dictionary<string, string> row, int refDate);

        #endregion Делегаты

        #region Закачка данных

        #region Работа с базой и кэшами

        protected override void QueryData()
        {
            bridgeClsSourceID = this.Scheme.DataVersionsManager.DataVersions.FindCurrentVersion(clsOrgBridge.ObjectKey);
            clsSourceId = AddDataSource("УФК", "0010", ParamKindTypes.Year,
                string.Empty, this.DataSource.Year, 0, string.Empty, 0, string.Empty).ID;
            string constr = string.Format("SOURCEID = {0}", clsSourceId);
            InitClsDataSet(ref daOrg, ref dsOrg, clsOrg);
            InitDataSet(ref daKd, ref dsKd, clsKd, false, constr, string.Empty);
            InitDataSet(ref daTax, ref dsTax, clsTax, false, constr, string.Empty);
            InitDataSet(ref daOkato, ref dsOkato, clsOkato, false, constr, string.Empty);
            InitDataSet(ref daPeriod, ref dsPeriod, clsPeriod, string.Empty);
            InitFactDataSet(ref daDrafts, ref dsDrafts, fctDrafts);
            InitFactDataSet(ref daPayers, ref dsPayers, fctPayers);
            InitFactDataSet(ref daRepayment, ref dsRepayment, fctRepayment);
            FillCaches();
        }

        private void FillCaches()
        {
            FillRowsCache(ref cacheKd, dsKd.Tables[0], "CodeStr", "ID");
            FillRowsCache(ref cacheTax, dsTax.Tables[0], "CodeStr", "ID");
            FillRowsCache(ref cacheOrg, dsOrg.Tables[0], new string[] { "INN", "KPP", "Okato", "Name" }, "|", "Id");
            FillRowsCache(ref cacheOkato, dsOkato.Tables[0], "Code", "ID");
            FillRowsCache(ref cachePeriod, dsPeriod.Tables[0], "RefFKDate", "RefFODate");
            if (cachePeriod.Count < 5)
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning,
                    "Не заполнен классификатор 'период.соответствие операционных дней', дата фо будет устанавливаться по дню фк.");
        }

        protected override void UpdateData()
        {
            UpdateDataSet(daKd, dsKd, clsKd);
            UpdateDataSet(daTax, dsTax, clsTax);
            UpdateDataSet(daOrg, dsOrg, clsOrg);
            UpdateDataSet(daOkato, dsOkato, clsOkato);
            UpdateDataSet(daDrafts, dsDrafts, fctDrafts);
            UpdateDataSet(daPayers, dsPayers, fctPayers);
            UpdateDataSet(daRepayment, dsRepayment, fctRepayment);
        }

        #region GUIDs

        private const string D_KD_GUID = "b713e1df-5584-4e3d-a399-8828a2906971";
        private const string D_TAX_GUID = "6f4532c5-16e8-402e-ba74-9bc7040a4be6";
        private const string D_ORG_GUID = "5d7f6e1d-c202-49b3-b6ad-d584616aded0";
        private const string D_OKATO_GUID = "4ae52664-ca7c-4994-bc5e-ba982421540e";
        private const string D_DATE_CONVERSION_FK_GUID = "414c27e7-393c-4516-8b47-cf6df384569d";
        private const string B_ORG_BRIDGE_GUID = "d952b688-d298-430c-89b2-17ef1e831e4f";
        private const string F_DRAFTS_GUID = "b4105bc0-49cf-4f25-bcf9-1ae785d4a36d";
        private const string F_PAYERS_GUID = "cf998c61-ca13-4080-b5dd-969a2fd76258";
        private const string F_REPAYMENT_GUID = "ab80f97f-4a1c-4113-8b1c-bd41b3dede65";

        #endregion GUIDs
        protected override void InitDBObjects()
        {
            clsPeriod = this.Scheme.Classifiers[D_DATE_CONVERSION_FK_GUID];
            clsOrgBridge = this.Scheme.Classifiers[B_ORG_BRIDGE_GUID];

            this.UsedClassifiers = new IClassifier[] {
                clsKd = this.Scheme.Classifiers[D_KD_GUID],
                clsTax = this.Scheme.Classifiers[D_TAX_GUID],
                clsOrg = this.Scheme.Classifiers[D_ORG_GUID],
                clsOkato = this.Scheme.Classifiers[D_OKATO_GUID] };

            this.HierarchyClassifiers = new IClassifier[] { clsKd, clsTax, clsOrg };

            this.UsedFacts = new IFactTable[] { 
                fctDrafts = this.Scheme.FactTables[F_DRAFTS_GUID], 
                fctPayers = this.Scheme.FactTables[F_PAYERS_GUID], 
                fctRepayment = this.Scheme.FactTables[F_REPAYMENT_GUID] };

            // нужно сопоставлять кд и Тип.Налоговый платеж - источник год
            this.AssociateClassifiersEx = new IClassifier[] { clsKd, clsTax };
        }

        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsDrafts);
            ClearDataSet(ref dsPayers);
            ClearDataSet(ref dsRepayment);
            ClearDataSet(ref dsKd);
            ClearDataSet(ref dsTax);
            ClearDataSet(ref dsOrg);
            ClearDataSet(ref dsOkato);
            ClearDataSet(ref dsPeriod);
        }

        #endregion Работа с базой и кэшами

        #region Работа с Dbf

        #region Общие методы

        private string GetPulseDBFQuery(object[] mapping, string patternName, string constraint)
        {
            string dbfQuery = "select ";
            int length = mapping.GetLength(0);
            string comma = ",";
            for (int i = 0; i <= length - 1; i++)
            {
                if (i == length - 1)
                    comma = string.Empty;
                string[] field = mapping[i].ToString().Split('=');
                if (field.GetLength(0) == 2)
                {
                    dbfQuery += string.Format("{0} as {1}{2} ", field[0], field[1], comma);
                }
                else
                {
                    dbfQuery += string.Format("{0}{1} ", field[0], comma);
                }
            }
            dbfQuery += string.Format("from {0} ", patternName);
            if (constraint != string.Empty)
                dbfQuery += string.Format("where {0} ", constraint);
            return dbfQuery;
        }

        private int FormatDate(string date)
        {
            return CommonRoutines.ShortDateToNewDate(date.Split(' ')[0]);
        }

        private int GetFileNameDate(string fileName)
        {
            return CommonRoutines.ShortDateToNewDate(fileName.Split('.')[0].Substring(2));
        }

        private void DeleteDataByDate(IFactTable[] fct, int dateRef, int factMark, int reportType, string constraint)
        {
            string key = string.Format("{0}|{1}|{2}", dateRef, factMark, reportType);
            if (!deletedDateList.Contains(key))
            {
                string constr = string.Format(" RefFileDay = {0} ", dateRef);
                if (constraint != string.Empty)
                    constr += string.Format(" and {0}", constraint);
                if (!this.DeleteEarlierData)
                    DirectDeleteFactData(fct, -1, this.SourceID, constr);
                deletedDateList.Add(key);
            }
        }

        private void ProcessDBF(object[] mapping, string fileName, ProcessDbfDelegate pDbfD, string constr)
        {
            int fileNameDate = GetFileNameDate(fileName);
            IDbDataAdapter da = null;
            DataSet ds = new DataSet();
            string query = GetPulseDBFQuery(mapping, fileName, constr);
            InitDataSet(dbfDatabase, ref da, ref ds, query);
            try
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                    pDbfD(row, fileNameDate);
            }
            finally
            {
                ClearDataSet(ref ds);
            }
        }

        private int GetBudLevel(string levelName)
        {
            switch (levelName.Trim().ToUpper())
            {
                case "ГОСУДАРСТВЕННЫЕ ВНЕБЮДЖЕТНЫЕ ФОНДЫ":
                    return 7;
                case "МЕСТНЫЕ":
                    return 14;
                case "ГОРОДСКИЕ ОКРУГА":
                    return 15;
                case "КРАЕВОЙ":
                case "СУБЪЕКТ РФ":
                    return 3;
                case "МУНИЦИПАЛЬНЫЕ РАЙОНЫ":
                    return 5;
                case "ПЕНСИОННЫЙ ФОНД":
                    return 8;
                case "СЕЛЬСКИЕ ПОСЕЛЕНИЯ":
                    return 17;
                case "ТЕРРИТОРИАЛЬНЫЙ ФОНД МЕД. СТРАХ.":
                    return 11;
                case "ФЕДЕРАЛЬНЫЙ":
                    return 1;
                case "ФОНД ОБЩЕГО МЕДИЦИНСКОГО СТРАХОВАНИЯ":
                    return 10;
                case "ФОНД СОЦИАЛЬНОГО СТРАХОВАНИЯ":
                    return 9;
                default:
                    return 0;
            }
        }

        private int GetFxType()
        {
            if (reportType == ReportType.VV)
                return 2;
            return 3;
        }

        #endregion Общие методы

        #region Закачка классификаторов

        private int PumpTax(string codeStr)
        {
            codeStr = codeStr.TrimStart('0').PadLeft(1, '0').ToUpper();
            return PumpCachedRow(cacheTax, dsTax.Tables[0], clsTax, codeStr, 
                new object[] { "CodeStr", codeStr, "SourceId", clsSourceId });
        }

        private int PumpOrg(string inn, string kpp, string okato, string name)
        {
            inn = CommonRoutines.TrimLetters(inn).TrimStart('0').PadLeft(1, '0');
            kpp = CommonRoutines.TrimLetters(kpp).TrimStart('0').PadLeft(1, '0');
            okato = okato.TrimStart('0').PadLeft(1, '0');
            name = string.Format("{0} ({1}, {2}, {3})", name.Trim(), inn, kpp, okato);
            string key = string.Format("{0}|{1}|{2}|{3}", inn, kpp, okato, name);
            return PumpCachedRow(cacheOrg, dsOrg.Tables[0], clsOrg, key,
                new object[] { "INN", inn, "KPP", kpp, "Okato", okato, "Name", name });
        }

        private int PumpKd(string codeStr)
        {
            codeStr = codeStr.TrimStart('0').PadLeft(1, '0');
            return PumpCachedRow(cacheKd, dsKd.Tables[0], clsKd, codeStr,
                new object[] { "CodeStr", codeStr, "Name", constDefaultClsName, "SourceId", clsSourceId });
        }

        #endregion Закачка классификаторов

        #region Закачка фактов

        #region Платежные поручения

        private void PumpDraftRow(DataRow row, int fileDate)
        {
            int refTax = PumpTax(row["TAX"].ToString());
            int refKd = PumpKd(row["KD"].ToString());
            int refOrg = PumpOrg(row["ORG_INN"].ToString(), row["ORG_KPP"].ToString(),
                row["ORG_OKATO"].ToString(), row["ORG_NAME"].ToString());
            decimal forPeriod = Convert.ToDecimal(row["ForPeriod"].ToString().PadLeft(1, '0'));
            if (forPeriod == 0)
                return;
            int refFKDay = FormatDate(row["RefFKDay"].ToString());
            if (isFinalOverturn)
                refFKDay = finalOverturnDate;
            string key = string.Format("{0}|{1}|{2}", refFKDay, 0, 1);
            if (!pumpedDateList[this.SourceID].Contains(key))
                pumpedDateList[this.SourceID].Add(key);

            int refFoDay = refFKDay;
            if (cachePeriod.ContainsKey(refFKDay))
                refFoDay = cachePeriod[refFKDay];
            object[] mapping = new object[] { "ForPeriod", forPeriod, "PayPurpose", row["PayPurpose"].ToString(), "DocDate", row["DocDate"].ToString(), 
                "DocNumber", row["DocNumber"].ToString(), "RefFKDay", refFKDay, "RefFODay", refFoDay, "RefFileDay", fileDate, 
                "PaymentSign", 0, "RefKD", refKd, "RefOrgUFK", refOrg, "RefTaxPayment", refTax };
            PumpRow(dsDrafts.Tables[0], mapping);
            if (dsDrafts.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
            {
                UpdateData();
                ClearDataSet(daDrafts, ref dsDrafts);
            }
        }

        private object[] DRAFTS_MAPPING = new object[] { "S_CRED=ForPeriod", "NAZN=PayPurpose", "DATE=DocDate", "NUMBER=DocNumber", 
            "DATE_EXEC=RefFKDay", "CINC=KD", "PAYINCTYPE=TAX", "PCORR_INN=ORG_INN", "PCORR_CPP=ORG_KPP", "PCORR_N=ORG_NAME", "CATE=ORG_OKATO"};
        private void PumpDrafts(FileInfo file)
        {
            int fileNameDate = GetFileNameDate(file.Name);
            DeleteDataByDate(new IFactTable[] { fctDrafts }, fileNameDate, 0, 1, string.Empty);
            ProcessDBF(DRAFTS_MAPPING, file.Name, PumpDraftRow, string.Empty);
        }

        #endregion Платежные поручения

        #region Возвраты и зачеты

        private void PumpRepayRow(DataRow row, int fileDate)
        {
            int refKd = PumpKd(row["KD"].ToString());
            int refOrg = PumpOrg(row["ORG_INN"].ToString(), row["ORG_KPP"].ToString(),
                row["ORG_OKATO"].ToString(), row["ORG_NAME"].ToString());
            decimal forPeriod = Convert.ToDecimal(row["ForPeriod"].ToString().PadLeft(1, '0'));
            if (forPeriod == 0)
                return;
            int refFKDay = fileDate;
            if (this.DataSource.Year < 2009)
                refFKDay = FormatDate(row["RefFKDay"].ToString());
            if (isFinalOverturn)
                refFKDay = finalOverturnDate;
            string key = string.Format("{0}|{1}|{2}", refFKDay, 1, GetFxType());
            if (!pumpedDateList[this.SourceID].Contains(key))
                pumpedDateList[this.SourceID].Add(key);

            int refFoDay = refFKDay;
            if (cachePeriod.ContainsKey(refFKDay))
                refFoDay = cachePeriod[refFKDay];
            int refFxTypes = GetFxType();
            int budLevel = GetBudLevel(row["BudLevel"].ToString());
            object[] mapping = new object[] { "ForPeriod", forPeriod, "DocDate", row["DocDate"].ToString(), 
                "DocNumber", row["DocNumber"].ToString(), "RefFKDay", refFKDay, "RefFODay", refFoDay, 
                "RefFXTypes", refFxTypes, "RefFileDay", fileDate, "RefKD", refKd, "RefOrgUFK", refOrg, "RefBdgtLevels", budLevel };
            PumpRow(dsRepayment.Tables[0], mapping);
            if (dsRepayment.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
            {
                UpdateData();
                ClearDataSet(daRepayment, ref dsRepayment);
            }
        }

        private object[] REPAY_VV_MAPPING = new object[] { "S_DEB_B=ForPeriod", "DATE0=DocDate", "NUMBER=DocNumber", 
            "DATE=RefFKDay", "CINC=KD", "LEV_NAME8=BudLevel", "PCORR_INN=ORG_INN", "PCORR_CPP=ORG_KPP", "PCORR_N=ORG_NAME", "CATE=ORG_OKATO"};
        private object[] REPAY_ZZ_MAPPING = new object[] { "S_Z_B=ForPeriod", "DATE0=DocDate", "NUMBER=DocNumber", 
            "DATE=RefFKDay", "CINC=KD", "LEV_NAME8=BudLevel", "PCORR_INN=ORG_INN", "PCORR_CPP=ORG_KPP", "PCORR_N=ORG_NAME", "CATE=ORG_OKATO"};
        private object[] REPAY_ZZ_MAPPING_2009 = new object[] { "S_Z_B=ForPeriod", "DATE0=DocDate", "NUMBER0=DocNumber", 
            "CINC=KD", "LEV_NAME8=BudLevel", "PCORR_INN=ORG_INN", "PCORR_CPP=ORG_KPP", "PCORR_N=ORG_NAME", "CATE=ORG_OKATO"};
        private object[] GetRepayMapping()
        {
            if (reportType == ReportType.VV)
                return REPAY_VV_MAPPING;
            else
            {
                if (this.DataSource.Year >= 2009)
                    return REPAY_ZZ_MAPPING_2009;
                else
                    return REPAY_ZZ_MAPPING;
            }
        }

        private void PumpRepayments(FileInfo file)
        {
            int fileNameDate = GetFileNameDate(file.Name);
            DeleteDataByDate(new IFactTable[] { fctRepayment }, fileNameDate, 1, GetFxType(), string.Empty);
            ProcessDBF(GetRepayMapping(), file.Name, PumpRepayRow, "(LEV_NAME8 = 'Местные')");
        }

        #endregion Возвраты и зачеты

        #region Поступления в бюджеты

        private void PumpPayersRow(DataRow row, int fileDate)
        {
            int refKd = PumpKd(row["KD"].ToString());
            int refOrg = PumpOrg(row["ORG_INN"].ToString(), row["ORG_KPP"].ToString(),
                row["ORG_OKATO"].ToString(), row["ORG_NAME"].ToString());
            decimal forPeriod = Convert.ToDecimal(row["ForPeriod"].ToString().PadLeft(1, '0'));
            if (forPeriod == 0)
                return;
            int refFKDay = fileDate;
            if (this.DataSource.Year < 2009)
                refFKDay = FormatDate(row["RefFKDay"].ToString());
            if (isFinalOverturn)
                refFKDay = finalOverturnDate;
            string key = string.Format("{0}|{1}|{2}", refFKDay, 2, GetFxType());
            if (!pumpedDateList[this.SourceID].Contains(key))
                pumpedDateList[this.SourceID].Add(key);

            int refFoDay = refFKDay;
            if (cachePeriod.ContainsKey(refFKDay))
                refFoDay = cachePeriod[refFKDay];
            int refFxTypes = GetFxType();
            int budLevel = GetBudLevel(row["BudLevel"].ToString());
            object[] mapping = new object[] { "ForPeriod", forPeriod, "DocDate", row["DocDate"].ToString(), 
                "DocNumber", row["DocNumber"].ToString(), "RefFKDay", refFKDay, "RefFODay", refFoDay, 
                "RefFXTypes", refFxTypes, "RefFileDay", fileDate, "RefKD", refKd, "RefOrgUFK", refOrg, "RefBdgtLevels", budLevel };
            PumpRow(dsPayers.Tables[0], mapping);
            if (dsPayers.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
            {
                UpdateData();
                ClearDataSet(daPayers, ref dsPayers);
            }
        }

        private object[] PAYERS_VV_MAPPING = new object[] { "S_DEB_B=ForPeriod", "DATE0=DocDate", "NUMBER=DocNumber", 
            "DATE=RefFKDay", "CINC=KD", "LEV_NAME8=BudLevel", "PCORR_INN=ORG_INN", "PCORR_CPP=ORG_KPP", "PCORR_N=ORG_NAME", "CATE=ORG_OKATO"};
        private object[] PAYERS_ZZ_MAPPING = new object[] { "S_Z_B=ForPeriod", "DATE0=DocDate", "NUMBER=DocNumber", 
            "DATE=RefFKDay", "CINC=KD", "LEV_NAME8=BudLevel", "PCORR_INN=ORG_INN", "PCORR_CPP=ORG_KPP", "PCORR_N=ORG_NAME", "CATE=ORG_OKATO"};
        private object[] PAYERS_ZZ_MAPPING_2009 = new object[] { "S_Z_B=ForPeriod", "DATE0=DocDate", "NUMBER0=DocNumber", 
            "CINC=KD", "LEV_NAME8=BudLevel", "PCORR_INN=ORG_INN", "PCORR_CPP=ORG_KPP", "PCORR_N=ORG_NAME", "CATE=ORG_OKATO"};
        private object[] GetPayersMapping()
        {
            if (reportType == ReportType.VV)
                return PAYERS_VV_MAPPING;
            else
            {
                if (this.DataSource.Year >= 2009)
                    return PAYERS_ZZ_MAPPING_2009;
                else
                    return PAYERS_ZZ_MAPPING;
            }
        }

        private void PumpTaxPayers(FileInfo file)
        {
            int fileNameDate = GetFileNameDate(file.Name);
            string constr = string.Format("RefFXTypes = {0}", GetFxType());
            DeleteDataByDate(new IFactTable[] { fctPayers }, fileNameDate, 2, GetFxType(), constr);
            ProcessDBF(GetPayersMapping(), file.Name, PumpPayersRow,
                "(LEV_NAME8 <> 'Местные') and (LEV_NAME8 <> '')");
        }

        #endregion Поступления в бюджеты

        #endregion Закачка фактов

        private void ReconnectToDbfDataSource(ODBCDriverName driver, DirectoryInfo dir)
        {
            dbDataAccess.ConnectToDataSource(ref dbfDatabase, dir.FullName, driver);
        }

        private DirectoryInfo CopyFilesToTempDir(DirectoryInfo sourceDir)
        {
            DirectoryInfo tempDir = CommonRoutines.GetTempDir();
            foreach (FileInfo file in sourceDir.GetFiles("*.dbf"))
                file.CopyTo(string.Format("{0}\\{1}", tempDir.FullName, file.Name), true);
            return tempDir;
        }

        private void SetDBFEncoding(string encodingName)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software\\microsoft\\jet\\4.0\\Engines\\Xbase", true);
            key.DeleteValue("DataCodePage", false);
            key.SetValue("DataCodePage", encodingName);
        }

        #endregion Работа с Dbf

        #region Работа с Cvs

        private long ValidateInt(string field, string value)
        {
            try
            {
                return Convert.ToInt64(value.Trim().PadLeft(1, '0'));
            }
            catch
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, string.Format(
                   "При обработке строки {0} возникла ошибка: Значение в поле '{1}' ({2}) имеет неверный формат. " +
                   "Полю '{1}' будет присвоено значение по умолчанию (0).", curRow + 1, field, value));
                return 0;
            }
        }

        private decimal ValidateFloat(string field, string value)
        {
            try
            {
                return Convert.ToDecimal(value.Trim().PadLeft(1, '0'));
            }
            catch
            {
                throw new InvalidDataException(string.Format(
                    "Значение в поле '{0}' ({1}) имеет неверный формат", field, value));
            }
        }

        private string ValidateString(string field, string value, int maxLength)
        {
            if (value.Length > maxLength)
            {
                throw new InvalidDataException(string.Format(
                    "Значение в поле '{0}' ({1}) слишком велико (фактическое: {2}, максимальное: {3})",
                    field, value, value.Length, maxLength));
            }
            return value;
        }

        private int GetRefFxTypes()
        {
            if (reportType == ReportType.ZZ)
                return 3;
            else if (reportType == ReportType.CC)
                return 0;
            return 2;
        }

        private int GetFkDay(string date)
        {
            if (isFinalOverturn)
                return finalOverturnDate;
            return CommonRoutines.ShortDateToNewDate(date);
        }

        private int GetFoDay(int refFkDay)
        {
            if (cachePeriod.ContainsKey(refFkDay))
                return cachePeriod[refFkDay];
            return refFkDay;
        }

        // Регулярное выражение для проверки наличия не чисел в строке
        Regex regExNonDigit = new Regex(@"\D", RegexOptions.IgnoreCase);
        // Закачать запись в классификатор КД.УФК
        private int PumpCsvKd(string codeStr)
        {
            codeStr = ValidateString("КБК ЭД", codeStr.Trim().PadLeft(1, '0'), 20);
            if (regExNonDigit.IsMatch(codeStr))
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, string.Format(
                   "При обработке строки {0} возникла ошибка: Значение в поле 'КБК ЭД' ({1}) имеет неверный формат. " +
                   "Полю 'КБК ЭД' будет присвоено значение по умолчанию (0).", curRow + 1, codeStr));
                codeStr = "0";
            }
            object[] mapping = new object[] { "CodeStr", codeStr, "Name", constDefaultClsName, "SourceId", clsSourceId };
            return PumpCachedRow(cacheKd, dsKd.Tables[0], clsKd, mapping, codeStr, "ID");
        }

        // Закачать запись в классификатор Тип.Налоговый платеж
        private int PumpCsvTaxPayment(string codeStr)
        {
            codeStr = codeStr.Trim().TrimStart('0').PadLeft(1, '0').ToUpper();
            codeStr = ValidateString("Тип платежа", codeStr, 20);
            object[] mapping = new object[] { "CodeStr", codeStr, "SourceId", clsSourceId };
            return PumpCachedRow(cacheTax, dsTax.Tables[0], clsTax, mapping, codeStr, "ID");
        }

        // Закачать запись в классификатор Организации.УФК_Плательщики
        private int PumpCsvOrgUfk(string inn, string kpp, string okato, string name)
        {
            long innInt = ValidateInt("ИНН", inn);
            long kppInt = ValidateInt("КПП", kpp);
            long okatoInt = ValidateInt("ОКАТО", okato);
            name = ValidateString("Имя Плат", name, 765);

            object[] mapping = new object[] { "INN", innInt, "KPP", kppInt, "OKATO", okatoInt, "Name", name };
            string cacheKey = string.Format("{0}|{1}|{2}|{3}", innInt, kppInt, okatoInt, name);
            int refOrg = PumpCachedRow(cacheOrg, dsOrg.Tables[0], clsOrg, mapping, cacheKey, "ID");
            if (dsOrg.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
            {
                UpdateDataSet(daOrg, dsOrg, clsOrg);
                ClearDataSet(daOrg, dsOrg.Tables[0]);
            }
            return refOrg;
        }

        // Закачать запись в классификатор ОКАТО.УФК
        private int PumpCsvOkato(string okato)
        {
            long okatoInt = ValidateInt("ОКАТО", okato);
            object[] mapping = new object[] { "Code", okatoInt, "Account", DBNull.Value,
                "DutyAccount", DBNull.Value, "Name", constDefaultClsName, "SourceId", clsSourceId };
            return PumpCachedRow(cacheOkato, dsOkato.Tables[0], clsOkato, mapping, okatoInt.ToString(), "ID");
        }

        // Закачать запись в таблицу фактов D.UFK10Drafts из файлов "p_*.csv"
        private void PumpCsvDrafts(Dictionary<string, string> row, int refDate)
        {
            int refKd = PumpCsvKd(row["KBK_ED"]);
            int refTax = PumpCsvTaxPayment(row["Type_Plat"]);
            int refOrgUfk = PumpCsvOrgUfk(row["INN_Plat"], row["KPP_Plat"], row["OKATO_ED"], row["Name_Plat"]);
            int refOkato = PumpCsvOkato(row["OKATO_ED"]);
            int refFkDay = GetFkDay(row["Date_OD"]);

            string key = string.Format("{0}|{1}|{2}", refFkDay, 0, 1);
            if (!pumpedDateList[this.SourceID].Contains(key))
                pumpedDateList[this.SourceID].Add(key);

            object[] mapping = new object[] {
                "ForPeriod", ValidateFloat("Сумма за период", row["Summa"]),
                "DocDate", row["Date_PP"],
                "DocNumber",  ValidateString("Номер документа", row["N_PP"], 30),
                "PayPurpose", ValidateString("Назначение платежа", row["Nazn_Plat"], 255),
                "ChargeBasis", ValidateString("Основание платежа", row["Osnov_Plat"], 20),
                "TaxPeriod", ValidateString("Налоговый период", row["Tax_Period"], 20),
                "KBK", ValidateString("КБК", row["KBK"], 20),
                "OKATO", ValidateString("OKATO ПП", row["OKATO_PP"], 20),
                "RefFKDay", refFkDay,
                "RefFODay", GetFoDay(refFkDay),
                "RefFileDay", refDate,
                "PaymentSign", 0,
                "RefKD", refKd,
                "RefOrgUFK", refOrgUfk,
                "RefTaxPayment", refTax,
                "RefIsDisint", 0,
                "RefOKATOUFK", refOkato
            };

            PumpRow(dsDrafts.Tables[0], mapping);
            if (dsDrafts.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
            {
                UpdateData();
                ClearDataSet(daDrafts, ref dsDrafts);
            }
        }

        // Закачать запись в таблицу фактов D.UFK10Repayment из файлов "v_*.csv"
        private void PumpCsvRepaymentVV(Dictionary<string, string> row, int refDate)
        {
            int refKd = PumpCsvKd(row["KBK"]);
            int refOrgUfk = PumpCsvOrgUfk(row["INN_Pol"], row["KPP_Pol"], row["OKATO"], row["Name_Pol"]);
            int refOkato = PumpCsvOkato(row["OKATO"]);
            int refFkDay = GetFkDay(row["Doc_Date"]);
            int refFxTypes = GetRefFxTypes();

            string key = string.Format("{0}|{1}|{2}", refFkDay, 1, refFxTypes);
            if (!pumpedDateList[this.SourceID].Contains(key))
                pumpedDateList[this.SourceID].Add(key);

            object[] mapping = new object[] {
                "ForPeriod", ValidateFloat("Сумма за период", row["Summa_Isp"]),
                "SummP", ValidateFloat("Сумма ПДА", row["Summa_PZV"]),
                "DocDate", row["Date"],
                "DocNumber", ValidateString("Номер документа", row["N"], 30),
                "RefFKDay", refFkDay,
                "RefFODay", GetFoDay(refFkDay),
                "RefFileDay", refDate,
                "RefKD", refKd,
                "RefBdgtLevels", 0,
                "RefOrgUFK", refOrgUfk,
                "RefFXTypes", refFxTypes,
                "RefIsDisint", 0,
                "RefOKATOUFK", refOkato
            };

            PumpRow(dsRepayment.Tables[0], mapping);
            if (dsRepayment.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
            {
                UpdateData();
                ClearDataSet(daRepayment, ref dsRepayment);
            }
        }

        // Закачать запись в таблицу фактов D.UFK10Repayment из файлов "z_*.csv"
        private void PumpCsvRepaymentZZ(Dictionary<string, string> row, int refDate)
        {
            int refKd = PumpCsvKd(row["KBK"]);
            int refOrgUfk = PumpCsvOrgUfk(row["INN_Plat"], row["KPP_Plat"], row["OKATO"], row["Name_Plat"]);
            int refOkato = PumpCsvOkato(row["OKATO"]);
            int refFkDay = GetFkDay(row["Doc_Date"]);
            int refFxTypes = GetRefFxTypes();

            string key = string.Format("{0}|{1}|{2}", refFkDay, 1, refFxTypes);
            if (!pumpedDateList[this.SourceID].Contains(key))
                pumpedDateList[this.SourceID].Add(key);

            object[] mapping = new object[] {
                "ForPeriod", ValidateFloat("Сумма за период", row["Summa_Isp"]),
                "SummP", ValidateFloat("Сумма ПДА", row["Summa_PDA"]),
                "DocDate", row["Date"],
                "DocNumber", ValidateString("Номер документа", row["N_RUD"], 30),
                "RefFKDay", refFkDay,
                "RefFODay", GetFoDay(refFkDay),
                "RefFileDay", refDate,
                "RefKD", refKd,
                "RefBdgtLevels", 0,
                "RefOrgUFK", refOrgUfk,
                "RefFXTypes", refFxTypes,
                "RefIsDisint", 0,
                "RefOKATOUFK", refOkato
            };

            PumpRow(dsRepayment.Tables[0], mapping);
            if (dsRepayment.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
            {
                UpdateData();
                ClearDataSet(daRepayment, ref dsRepayment);
            }
        }

        // Закачать запись в таблицу фактов D.UFK10Repayment из файлов "c_*.csv"
        private void PumpCsvRepaymentCC(Dictionary<string, string> row, int refDate)
        {
            int refKd = PumpCsvKd(row["KBK_PSCh"]);
            int refOrgUfk = PumpCsvOrgUfk(row["INN_Pol"], row["KPP_Pol"], row["OKATO_PSCh"], row["Name_Pol"]);
            int refOkato = PumpCsvOkato(row["OKATO_PSCh"]);
            int refFkDay = GetFkDay(row["Doc_Date"]);
            int refFxTypes = GetRefFxTypes();

            string key = string.Format("{0}|{1}|{2}", refFkDay, 1, refFxTypes);
            if (!pumpedDateList[this.SourceID].Contains(key))
                pumpedDateList[this.SourceID].Add(key);

            object[] mapping = new object[] {
                "ForPeriod", ValidateFloat("Сумма за период", row["Summa_PSCh"]),
                "SummP", ValidateFloat("Сумма ПДА", row["Summa_PZV"]),
                "DocDate", row["Date"],
                "DocNumber",  ValidateString("Номер документа", row["N"], 30),
                "RefFKDay", refFkDay,
                "RefFODay", GetFoDay(refFkDay),
                "RefFileDay", refDate,
                "RefKD", refKd,
                "RefBdgtLevels", 0,
                "RefOrgUFK", refOrgUfk,
                "RefFXTypes", refFxTypes,
                "RefIsDisint", 0,
                "RefOKATOUFK", refOkato
            };

            PumpRow(dsRepayment.Tables[0], mapping);
            if (dsRepayment.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
            {
                UpdateData();
                ClearDataSet(daRepayment, ref dsRepayment);
            }
        }

        #region маппинги полей

        private object[] CSV_P_MAPPING_2010 = new object[] {
            "Date_OD", 0, "N_PP", 1, "Date_PP", 2, "INN_Plat", 3, "KPP_Plat", 4, "Name_Plat", 5,
            "Summa", 6, "KBK", 7, "OKATO_PP", 8, "KBK_ED", 9, "OKATO_ED", 10, "INN_AP", 11,
            "KPP_AP", 12, "Nazn_Plat", 13, "Osnov_Plat", 14, "Tax_Period", 15, "Tax_Number", 16,
            "Tax_Date", 17, "Type_Plat", 18 };
        private object[] CSV_P_MAPPING_2011_01 = new object[] {
            "Date_OD", 0, "N_PP", 1, "Date_PP", 2, "INN_Plat", 3, "KPP_Plat", 4, "Name_Plat", 5,
            "RS_Plat", 6, "Summa", 7, "KBK", 8, "OKATO_PP", 9, "INN_Pol", 10, "KPP_Pol", 11,
            "Name_Pol", 12, "LS", 13, "KBK_ED", 14, "OKATO_ED", 15, "INN_AP", 16, "KPP_AP", 17,
            "Nazn_Plat", 18, "Osnov_Plat", 19, "Tax_Period", 20, "Tax_Number", 21, "Tax_Date", 22,
            "Type_Plat", 23 };
        private object[] CSV_V_MAPPING_2010 = new object[] {
            "Doc_Date", 0, "N", 1, "Date", 2, "INN_Pol", 3, "KPP_Pol", 4, "Name_Pol", 5,
            "OKATO", 6, "KBK", 7, "INN_AP", 8, "KPP_AP", 9, "Name_AP", 10, "Summa_PZV", 11,
            "Summa_Isp", 12 };
        private object[] CSV_Z_MAPPING_2010 = new object[] {
            "Doc_Date", 0, "Type_PDA", 1, "N", 2, "Date", 3, "N_RUD", 4, "Date_RUD", 5,
            "Kod_UBP", 6, "Name_UBP", 7, "INN_Plat", 8, "KPP_Plat", 9, "Name_Plat", 10,
            "Dt_Kt", 11, "N_PD", 12, "Date_PD", 13, "Name_PD", 14, "KBK", 15, "OKATO", 16,
            "Summa_PDA", 17, "Summa_Isp", 18 };
        private object[] CSV_C_MAPPING_2010 = new object[] {
            "Doc_Date", 0, "N", 1, "Date", 2, "INN_Pol", 3, "KPP_Pol", 4, "Name_Pol", 5,
            "OKATO", 6, "KBK", 7, "INN_AP", 8, "KPP_AP", 9, "Name_AP", 10, "Summa_PZV", 11,
            "KBK_PSCh", 12, "OKATO_PSCh", 13, "Summa_PSCh", 14 };

        #endregion маппинги полей
        private const string CSV_REPORT_DELIMITER = "\",\"";
        private Dictionary<string, string> GetCsvDataRow(string reportRow, object[] mapping)
        {
            Dictionary<string, string> dataRow = new Dictionary<string, string>();

            string[] rowValues = reportRow.Split(new string[] { CSV_REPORT_DELIMITER }, StringSplitOptions.None);
            rowValues[0] = rowValues[0].TrimStart(new char[] { '"' });
            rowValues[rowValues.GetLength(0) - 1] = rowValues[rowValues.GetLength(0) - 1].TrimEnd(new char[] { '"' });

            if (rowValues.GetLength(0) < (mapping.GetLength(0) / 2))
                throw new InvalidDataException("Строка имеет неверный формат");

            int columnsCount = mapping.GetLength(0);
            for (int i = 0; i < columnsCount; i += 2)
            {
                dataRow.Add(mapping[i].ToString(), rowValues[Convert.ToInt32(mapping[i + 1])]);
            }

            return dataRow;
        }

        // Закачка данных из Csv-файлов
        private void PumpCsvReport(FileInfo file, PumpCsvDataRow pumpDataRow, object[] mapping, int refDate)
        {
            string[] report = CommonRoutines.GetTxtReportData(file, CommonRoutines.GetTxtWinCodePage());
            string dataSourcePath = GetShortSourcePathBySourceID(this.SourceID);
            int rowsCount = report.GetLength(0);
            for (curRow = 1; curRow < rowsCount; curRow++)
                try
                {
                    SetProgress(rowsCount, curRow,
                        string.Format("Обработка файла {0}\\{1}...", dataSourcePath, file.Name),
                        string.Format("Строка {0} из {1}", curRow, rowsCount));

                    if (report[curRow].Trim() == string.Empty)
                        continue;

                    Dictionary<string, string> dataRow = GetCsvDataRow(report[curRow], mapping);
                    pumpDataRow(dataRow, refDate);
                }
                catch (InvalidDataException ex)
                {
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, string.Format(
                        "При обработке строки {0} файла \"{1}\" возникла ошибка: {2}. Строка {0} будет пропущена.",
                        curRow + 1, file.Name, ex.Message));
                    continue;
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("При обработке строки {0} возникла ошибка ({1})",
                        curRow + 1, ex.Message), ex);
                }
        }

        // Получить дату из параметра источника "Год" и имени файла (x_MMDD.csv)
        private int GetCsvRefDate(string filename)
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning,
                "Дата будет определена по параметру источника \"Год\" и по имени файла");
            return (this.DataSource.Year * 10000 + Convert.ToInt32(filename.Substring(2, 4)));
        }

        private void DeleteCsvData(IFactTable fct, int refDate, int type)
        {
            if (!deletedDateListCsv.Contains(refDate))
            {
                string constr = string.Format(" (RefFileDay = {0}) ", refDate);
                if (type != -1)
                    constr = string.Format(" {0} AND (RefFXTypes = {1}) ", constr, type);
                if (!this.DeleteEarlierData)
                    DirectDeleteFactData(new IFactTable[] { fct }, -1, this.SourceID, constr);
                deletedDateListCsv.Add(refDate);
            }
        }

        private void PumpCsvFile(FileInfo file)
        {
            WriteToTrace("Открытие документа: " + file.Name, TraceMessageKind.Information);
            try
            {
                int refDate = GetCsvRefDate(file.Name);
                switch (reportType)
                {
                    case ReportType.PP:
                        DeleteCsvData(fctDrafts, refDate, -1);
                        if ((this.DataSource.Year == 2011) && (this.DataSource.Month == 1))
                            PumpCsvReport(file, PumpCsvDrafts, CSV_P_MAPPING_2011_01, refDate);
                        else
                            PumpCsvReport(file, PumpCsvDrafts, CSV_P_MAPPING_2010, refDate);
                        UpdateData();
                        break;
                    case ReportType.VV:
                        DeleteCsvData(fctRepayment, refDate, 2);
                        PumpCsvReport(file, PumpCsvRepaymentVV, CSV_V_MAPPING_2010, refDate);
                        UpdateData();
                        break;
                    case ReportType.ZZ:
                        DeleteCsvData(fctRepayment, refDate, 3);
                        PumpCsvReport(file, PumpCsvRepaymentZZ, CSV_Z_MAPPING_2010, refDate);
                        UpdateData();
                        break;
                    case ReportType.CC:
                        DeleteCsvData(fctRepayment, refDate, 0);
                        PumpCsvReport(file, PumpCsvRepaymentCC, CSV_C_MAPPING_2010, refDate);
                        UpdateData();
                        break;
                }
            }
            finally
            {
                GC.GetTotalMemory(true);
            }
        }

        #endregion Работа с Cvs

        #region Перекрытые методы закачки

        private void PumpDbfFiles(DirectoryInfo dir)
        {
            // так как дбфки ебанутые - непонятный формат (вроде visual fox pro, но открыть не получилось)
            // приходится конвертить их в формат dbase III причем обязательно менять кодировку провайдера
            SetDBFEncoding("ANSI");
            // блять из за смены кодировки драйвера он не может приконнектиться к директории с русским названием
            // копируем файлы во временную папку и качаем оттуда
            DirectoryInfo tempDir = CopyFilesToTempDir(dir);
            ReconnectToDbfDataSource(ODBCDriverName.Microsoft_dBase_Driver, tempDir);
            try
            {
                // платежные поручения
                if (dir.GetFiles("pp*.dbf", SearchOption.AllDirectories).GetLength(0) > 0)
                {
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Закачка платежных поручений");
                    reportType = ReportType.PP;
                    ProcessFilesTemplate(dir, "pp*.dbf", new ProcessFileDelegate(PumpDrafts), false);
                    UpdateData();
                }
                if ((dir.GetFiles("zz*.dbf", SearchOption.AllDirectories).GetLength(0) > 0) ||
                    (dir.GetFiles("vv*.dbf", SearchOption.AllDirectories).GetLength(0) > 0))
                {
                    // возвраты и зачеты
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Закачка возвратов и зачетов");
                    reportType = ReportType.VV;
                    ProcessFilesTemplate(dir, "vv*.dbf", new ProcessFileDelegate(PumpRepayments), false);
                    reportType = ReportType.ZZ;
                    ProcessFilesTemplate(dir, "zz*.dbf", new ProcessFileDelegate(PumpRepayments), false);
                    UpdateData();
                    // поступления в бюджеты
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Закачка поступлений в бюджеты");
                    reportType = ReportType.VV;
                    ProcessFilesTemplate(dir, "vv*.dbf", new ProcessFileDelegate(PumpTaxPayers), false);
                    reportType = ReportType.ZZ;
                    ProcessFilesTemplate(dir, "zz*.dbf", new ProcessFileDelegate(PumpTaxPayers), false);
                    UpdateData();
                }
            }
            finally
            {
                if (dbDataAccess != null)
                    dbDataAccess.Dispose();
                SetDBFEncoding("OEM");
                CommonRoutines.DeleteDirectory(tempDir);
            }
        }

        private void PumpCvsFiles(DirectoryInfo dir)
        {
            reportType = ReportType.PP;
            ProcessFilesTemplate(dir, "p*.csv", new ProcessFileDelegate(PumpCsvFile), false);
            reportType = ReportType.VV;
            ProcessFilesTemplate(dir, "v*.csv", new ProcessFileDelegate(PumpCsvFile), false);
            reportType = ReportType.ZZ;
            ProcessFilesTemplate(dir, "z*.csv", new ProcessFileDelegate(PumpCsvFile), false);
            reportType = ReportType.CC;
            ProcessFilesTemplate(dir, "c*.csv", new ProcessFileDelegate(PumpCsvFile), false);
        }

        private void ProcessAllFiles(DirectoryInfo dir)
        {
            FileInfo[] files = dir.GetFiles("*.dbf", SearchOption.AllDirectories);
            if (files.GetLength(0) > 0)
                PumpDbfFiles(dir);
            files = dir.GetFiles("*.csv", SearchOption.AllDirectories);
            if (files.GetLength(0) > 0)
                PumpCvsFiles(dir);

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

        protected override void ProcessFiles(DirectoryInfo dir)
        {
            isFinalOverturn = Convert.ToBoolean(GetParamValueByName(this.ProgramConfig, "cbFinalOverturn", "false"));
            finalOverturnDate = this.DataSource.Year * 10000 + 1232;
            deletedDateList = new List<string>();
            deletedDateListCsv = new List<int>();
            try
            {
                pumpedDateList.Add(this.SourceID, new List<string>());
                ProcessAllFiles(dir);
            }
            finally
            {
                deletedDateList.Clear();
                deletedDateListCsv.Clear();
            }
        }

        protected override void DirectPumpData()
        {
            pumpedDateList = new Dictionary<int, List<string>>();
            PumpDataYMTemplate();
        }

        #endregion Перекрытые методы закачки

        #endregion Закачка данных

        #region Обработка данных

        #region Перекрытые методы

        new protected void UpdateMessagesDS()
        {
            if (disintRulesIsEmpty) return;

            if (dsMessages != null)
            {
                Dictionary<string, int> cache = new Dictionary<string, int>();
                int rowsCount = dsMessages.Tables[0].Rows.Count;
                for (int i = 0; i < rowsCount; i++)
                {
                    string key = string.Format("{0}|{1}",
                        dsMessages.Tables[0].Rows[i]["KD"],
                        dsMessages.Tables[0].Rows[i]["YEAR"]);

                    if (cache.ContainsKey(key))
                        cache[key] += Convert.ToInt32(dsMessages.Tables[0].Rows[i]["COUNTER"]);
                    else
                        cache.Add(key, Convert.ToInt32(dsMessages.Tables[0].Rows[i]["COUNTER"]));
                }
                foreach (KeyValuePair<string, int> pair in cache)
                {
                    string[] key = pair.Key.Split(new char[] { '|' });
                    string msg = string.Format("Не найдено ни одного расщепления для КД = {0}, Год = {1} ({2} записей).",
                        key[0], key[1], pair.Value);
                    WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeWarning, this.PumpID, this.SourceID, msg, null);
                }
            }
        }

        protected override void UpdateProcessedData()
        {
            UpdateOkatoData();
            UpdateData();
        }

        protected override void AfterProcessDataAction()
        {
            UpdateMessagesDS();
            WriteBadOkatoCodesCacheToBD();
        }

        #endregion Перекрытые методы

        #region проставление ссылки на фикс клс у таблицы фактов "Доходы.УФК_Налогоплательщики_Платежные поручения"

        private bool ContainsSign(string payPurpose, string[] signs)
        {
            foreach (string sign in signs)
                if (payPurpose.ToUpper().Contains(sign.ToUpper()))
                    return true;
            return false;
        }

        private const string PAYMENTS_SIGN = "квит;кол.кв.;принято;кол-во кв;кв=;пд-4;[_]40911;кол-во;" + 
            "пд 4;пд4;от населения;перевод от;комиссия;платежи от;возврат;по заявлен;реестр;кол. кв.;" + 
            "пд -4;пд - 4;у физ.лиц;у физ. лиц;от физ.лиц;от физ. лиц;Ф 31;ф.31;ф. 31;ф31;" + 
            "Ф.31;Ф. 31;Ф31;китанции;д-т40911;кв.=;количество;платежи населения;возв.;кол-;" + 
            "кол=;Ф.187;Ф 187;Ф187;кв б/н;кв. б/н;кв б\\н;заявления б/н;заявления б\\н;у населения;" + 
            "документов;перевод;заявления клиента;принятая;заявл.от;заявл. от;заяв.от;заяв. от;ГСК;" + 
            "платежи в бюджет;откредит;Ф. 187;ПГСК;ГСТК;кв.б/н;к-ве;кол кв;возн.;комиссионное;итанци;" +
            "Ф,31;Ф,187;заяв б/н;заявл. б/н;списано с-но;списано сог;ГСК-;Ф ПД;кол;Ф б/н;" + 
            "ЗАЯВЛЕНИЯ ОТ;ЗАЯВЛЕНИЮ ОТ;гражданства;из пенси;с пенсии;с пенсионера;наличным;заявлений б/н;" + 
            "ч-з;ч/з;КОЛ;кв. б\\н;кв-ции б\\н;загранпаспорт;Ф,31;Ф. б/н;извещ.б/н;КОЛ-ВЕ";
        private void SetPaymentSign(IFactTable fct)
        {
            string semantic = fct.FullCaption;
            WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeStart, 
                string.Format("Старт обработки факта {0} (установка ссылки на фиксированный классификатор).", semantic));
            int totalRecs = Convert.ToInt32(this.DB.ExecQuery(string.Format(
                "select count(id) from {0} where SOURCEID = {1}", fct.FullDBName, this.SourceID), QueryResultTypes.Scalar));
            if (totalRecs == 0)
            {
                WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeInformation,
                    string.Format("Нет данных по {0}.", semantic));
                return;
            }
            string[] paymentsSign = PAYMENTS_SIGN.Split(';');
            int firstID = Convert.ToInt32(this.DB.ExecQuery(string.Format(
                "select min(id) from {0} where SOURCEID = {1}", fct.FullDBName, this.SourceID), QueryResultTypes.Scalar));
            int lastID = firstID + MAX_DS_RECORDS_AMOUNT * 2 - 1;
            int processedRecCount = 0;
            IDbDataAdapter da = null;
            DataSet ds = null;
            do
            {
                string idConstr = string.Format("ID >= {0} and ID <= {1} and SOURCEID = {2}", firstID, lastID, this.SourceID);
                firstID = lastID + 1;
                lastID += MAX_DS_RECORDS_AMOUNT * 2; 
                InitDataSet(ref da, ref ds, fct, idConstr);
                try
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        processedRecCount++;
                        if (ContainsSign(row["PayPurpose"].ToString(), paymentsSign))
                            row["PaymentSign"] = 1;
                        else
                            row["PaymentSign"] = 0;
                    }
                    UpdateDataSet(da, ds, fct);
                }
                finally
                {
                    ClearDataSet(ref ds);
                }
            }
            while (processedRecCount < totalRecs);
            WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeSuccefullFinished,
                string.Format("Завершение обработки факта {0} (установка ссылки на фиксированный классификатор).", semantic));
        }

        #endregion проставление ссылки на фикс клс у таблицы фактов "Доходы.УФК_Налогоплательщики_Платежные поручения"

        #region ниибическая установка иерархии организаций

        // получить ахуенный кэш организаций: связка инн - айди
        // инн - уникальный в кэше
        // по умолчанию записывается первый ай ди группы, 
        // если потом встречается запись с КПП = ХХХХ01ХХХ - переписываем айди, ХХХХ50ХХХ - еще более приоритетная
        private Dictionary<decimal, int> GetAuxOrgCache(ref Dictionary<decimal, DataRow[]> cacheInnOrg)
        {
            Dictionary<decimal, int> cacheAuxOrg = new Dictionary<decimal, int>();
            DataRow[] orgRows = dsOrg.Tables[0].Select(string.Empty, "Inn Asc");
            bool toChangeId = true;
            foreach (DataRow orgRow in orgRows)
            {
                decimal orgInn = Convert.ToDecimal(orgRow["Inn"]);
                string orgKpp = orgRow["Kpp"].ToString();
                string orgKppPart = string.Empty;
                if (orgKpp.Length > 3)
                    orgKppPart = orgKpp.Remove(orgKpp.Length - 3);
                int orgId = Convert.ToInt32(orgRow["Id"]);
                if (!cacheAuxOrg.ContainsKey(orgInn))
                {
                    cacheAuxOrg.Add(orgInn, orgId);
                    cacheInnOrg.Add(orgInn, new DataRow[] { orgRow });
                    toChangeId = true;
                }
                else
                {
                    cacheInnOrg[orgInn] = (DataRow[])CommonRoutines.ConcatArrays(cacheInnOrg[orgInn], new DataRow[] { orgRow });
                    if (!toChangeId)
                        continue;
                    if (orgKpp.Length <= 3)
                        continue;
                    if (orgKppPart.EndsWith("01"))
                        cacheAuxOrg[orgInn] = orgId;
                    if (orgKppPart.EndsWith("50"))
                    {
                        cacheAuxOrg[orgInn] = orgId;
                        toChangeId = false;
                    }
                }
            }
            return cacheAuxOrg;
        }

        // тру - иерархия установлена хотя бы у одной записи группы по инн
        private bool IsHierarchied(DataRow[] innRows, ref int parentId)
        {
            foreach (DataRow row in innRows)
                if (row["ParentId"] != DBNull.Value)
                {
                    parentId = Convert.ToInt32(row["ParentId"]);
                    return true;
                }
            return false;
        }

        // установка иерархии организации.уфк (алгоритм ниибаццо сложен, и логике не поддается)
        private void SetOrgHierarchy()
        {
            WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeInformation,
                "начало установки иерархии классификатора 'Организации.УФК плательщики'");
            Dictionary<decimal, DataRow[]> cacheInnOrg = new Dictionary<decimal, DataRow[]>();
            Dictionary<decimal, int> cacheAuxOrg = GetAuxOrgCache(ref cacheInnOrg);
            Dictionary<int, DataRow> cacheRowOrg = null;
            FillRowsCache(ref cacheRowOrg, dsOrg.Tables[0], "ID");
            Dictionary<int, int> hierDict = new Dictionary<int, int>();
            try
            {
                foreach (KeyValuePair<decimal, int> cacheItem in cacheAuxOrg)
                {
                    decimal orgInn = cacheItem.Key;
                    // нулевый инн не группируются 
                    if (orgInn == 0)
                        continue;
                    DataRow[] innRows = cacheInnOrg[orgInn];
                    // если группа по инн состоит из одной записи ничего не делаем
                    if (innRows.GetLength(0) == 1)
                        continue;
                    DataRow parentInnRow = null;
                    // если уже установлена иерархия - находим родительскую запись
                    int parentId = -1;
                    if (IsHierarchied(innRows, ref parentId))
                        parentInnRow = cacheRowOrg[parentId];
                    DataRow orgRow = cacheRowOrg[cacheItem.Value];
                    // добавляем родительскую запись, если ранее не найдена
                    if (parentInnRow == null)
                    {
                        parentInnRow = dsOrg.Tables[0].NewRow();
                        parentId = GetGeneratorNextValue(clsOrg);
                        dsOrg.Tables[0].Rows.Add(parentInnRow);
                    }
                    CopyRowToRow(orgRow, parentInnRow);
                    parentInnRow["ID"] = parentId;
                    parentInnRow["ParentId"] = DBNull.Value;
                    // подчиняем все записи группы родительской записи
                    foreach (DataRow innRow in innRows)
                    {
                        int rowId = Convert.ToInt32(innRow["Id"]);
                        if (rowId != parentId)
                            hierDict.Add(rowId, parentId);
                    }
                }
                UpdateDataSet(daOrg, dsOrg, clsOrg);
                // устанавливаем иерархию
                foreach (KeyValuePair<int, int> cacheItem in hierDict)
                    cacheRowOrg[cacheItem.Key]["ParentId"] = cacheItem.Value;
            }
            finally
            {
                cacheAuxOrg.Clear();
                cacheRowOrg.Clear();
                cacheInnOrg.Clear();
                hierDict.Clear();
            }
            WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeInformation,
                "завершение установки иерархии классификатора 'Организации.УФК плательщики'");
        }

        #endregion ниибическая установка иерархии организаций

        #region формирование классификатора "Организации.Сопоставимый"

        #region Заполнение

        // Заполнение классификатора из датасета "Плательщиков"
        // !!! ВНИМАНИЕ !!! 
        // Если придётся добавить ещё одно поле для заполнения, не забыть добавить его в массив NEEDED_FIELDS
        private void FillOrgBridgeFromPayers(DataSet dsPayers, string fileName, Dictionary<string, DataRow> cacheName)
        {
            Dictionary<string, object> defaultValues = new Dictionary<string, object>();
            defaultValues.Add("Code", 0);
            defaultValues.Add("OKATOCode", 0);
            defaultValues.Add("INN20", "0");
            defaultValues.Add("Name", string.Empty);
            defaultValues.Add("MainOKVED", "0");

            int curRow = 0;
            int rowsCount = dsPayers.Tables[0].Rows.Count;
            foreach (DataRow row in dsPayers.Tables[0].Rows)
            {
                SetProgress(rowsCount, curRow,
                    string.Format("Обработка файла {0}...", fileName),
                    string.Format("Строка {0} из {1}", curRow, rowsCount));

                Dictionary<string, object> fields = new Dictionary<string, object>();
                foreach (KeyValuePair<string, object> field in defaultValues)
                    try
                    {
                        string fieldName = field.Key;
                        if (row[fieldName].ToString().Trim() == string.Empty)
                        {
                            fields.Add(fieldName, defaultValues[fieldName]);
                            continue;
                        }

                        object value = null;
                        if ((fieldName == "Code") || (fieldName == "OKATOCode") || (fieldName == "INN20"))
                            value = Convert.ToInt64(row[fieldName].ToString().Trim());
                        else
                            value = row[fieldName].ToString();
                        fields.Add(fieldName, value);
                    }
                    catch
                    {
                        WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeWarning, string.Format(
                            "При обработке строки {0} возникла ошибка: неверный формат значения в поле {1} = \"{2}\". " +
                            "Полю {1} будет присвоено значение по умолчанию ({3}).",
                            curRow, field.Key, row[field.Key].ToString(), field.Value.ToString()));
                        fields.Add(field.Key, defaultValues[field.Key]);
                    }

                string key = string.Format("{0}|{1}|{2}", fields["Code"], fields["OKATOCode"], fields["INN20"]);
                if (!cacheOrgBridge.ContainsKey(key))
                {
                    if (fields["MainOKVED"].ToString() == "0")
                        fields["MainOKVED"] = DBNull.Value;
                    else
                        fields["MainOKVED"] = fields["MainOKVED"].ToString().PadRight(6, '0');

                    object[] mapping = new object[] {
                        "Code", fields["Code"],
                        "OKATOCode", fields["OKATOCode"],
                        "INN20", fields["INN20"],
                        "Name", fields["Name"],
                        "MainOKVED", fields["MainOKVED"],
                        "SourceId", bridgeClsSourceID };

                    DataRow pumpedRow = PumpRow(clsOrgBridge, dsOrgBridge.Tables[0], mapping, true);

                    // если запись с таким наименованием уже есть, то приписываем к наименованию Id без пробелов
                    string name = Convert.ToString(fields["Name"]);
                    if (cacheName.ContainsKey(name))
                    {
                        DataRow cachedRow = cacheName[name];
                        if (!cachedRow["Name"].ToString().StartsWith(cachedRow["ID"].ToString()))
                            cachedRow["Name"] = string.Concat(cachedRow["ID"], cachedRow["Name"]);
                        pumpedRow["Name"] = string.Concat(pumpedRow["ID"], name);
                    }
                    else
                    {
                        cacheName.Add(name, pumpedRow);
                    }

                    cacheOrgBridge.Add(key, -1);
                }
                curRow++;
            }
        }

        private object[] PAYERS_MAPPING =
            new object[] { "INN=Code", "CPP=INN20", "CATE=OKATOCode", "NAME", "OKVED=MainOKVED" };
        // Закачка файла Плательщиков
        private void PumpPayersFile(string fileName)
        {
            IDbDataAdapter da = null;
            DataSet ds = new DataSet();
            string query = GetPulseDBFQuery(PAYERS_MAPPING, fileName, string.Empty);
            InitDataSet(dbfDatabase, ref da, ref ds, query);
            Dictionary<string, DataRow> cacheName = new Dictionary<string, DataRow>();
            try
            {
                InitDataSet(ref daOrgBridge, ref dsOrgBridge, clsOrgBridge, string.Format("SourceID = {0}", bridgeClsSourceID));
                FillRowsCache(ref cacheOrgBridge, dsOrgBridge.Tables[0],
                    new string[] { "Code", "INN20", "OKATOCode" }, "|", "Id");
                FillRowsCache(ref cacheName, dsOrgBridge.Tables[0], new string[] { "Name" });

                // чистим датасет, для освобождения памяти
                // т.к. качать придётся много, памяти может не хватить
                // dsOrgBridge.Tables[0].Clear();
                // GC.Collect();

                // Заполнение классификатора "Организации.Сопоставимый"
                FillOrgBridgeFromPayers(ds, fileName, cacheName);

                UpdateDataSet(daOrgBridge, dsOrgBridge, clsOrgBridge);
                ClearDataSet(ref dsOrgBridge);

                cacheOrgBridge.Clear();
                cacheName.Clear();
            }
            finally
            {
                ClearDataSet(ref ds);
                if (dbfDatabase != null)
                {
                    dbfDatabase.Close();
                    dbfDatabase = null;
                }
                GC.Collect();
            }
        }

        // Закачка плательщиков из dbf-файла (ищется в папке "__ПЛАТЕЛЬЩИКИ")
        private const string PAYERS_PATH = "__ПЛАТЕЛЬЩИКИ";
        private const string PAYERS_FILE = "Spisok1.dbf";
        private void PumpPayers()
        {
            bridgePumpedFromDbf = true;
            WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeStart,
                "Старт формирования классификатора \"Организации.Сопоставимый\"");

            DirectoryInfo[] payers = this.RootDir.GetDirectories(PAYERS_PATH, SearchOption.TopDirectoryOnly);
            // каталог "Плательщики" должен присутствовать
            if (payers.GetLength(0) == 0)
            {
                WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeWarning, string.Format(
                    "Отсутствует каталог \"{0}\". Классификатор \"Организации.Сопоставимый\" сформирован не будет", PAYERS_PATH));
                return;
            }

            DirectoryInfo tempDir = CopyFilesToTempDir(payers[0]);
            ReconnectToDbfDataSource(ODBCDriverName.Microsoft_dBase_Driver, tempDir);
            try
            {
                FileInfo[] files = payers[0].GetFiles(PAYERS_FILE, SearchOption.AllDirectories);
                if (files.GetLength(0) <= 0)
                {
                    WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeWarning, string.Format(
                        "Отсутствует файл \"{0}\". Классификатор \"Организации.Сопоставимый\" сформирован не будет", PAYERS_FILE));
                    return;
                }
                PumpPayersFile(files[0].Name);
            }
            finally
            {
                if (dbDataAccess != null)
                    dbDataAccess.Dispose();
                CommonRoutines.DeleteDirectory(tempDir);
            }

            WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeSuccefullFinished,
                "Завершение формирования классификатора \"Организации.Сопоставимый\"");
        }

        #endregion Заполнение

        #region Копирование записей из классификатора Организации.УФК_Плательщики

        private void CopyFromPayers()
        {
            WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeStart,
                "Старт формирования классификатора \"Организации.Сопоставимый\" из \"Организации.УФК_Плательщики\"");

            InitDataSet(ref daOrgBridge, ref dsOrgBridge, clsOrgBridge, string.Format("SourceID = {0}", bridgeClsSourceID));
            FillRowsCache(ref cacheOrgBridge, dsOrgBridge.Tables[0], new string[] { "Code", "INN20", "OKATOCode" }, "|", "ID");

            try
            {
                int rowsCount = dsOrg.Tables[0].Rows.Count;
                for (int curRow = 0; curRow < rowsCount; curRow++)
                {
                    SetProgress(rowsCount, curRow,
                        "Копирование записей из \"Организации.УФК_Плательщики\"...",
                        string.Format("Строка {0} из {1}", curRow, rowsCount));

                    DataRow orgRow = dsOrg.Tables[0].Rows[curRow];
                    string orgKey = GetComplexCacheKey(orgRow, new string[] { "INN", "KPP", "OKATO" }, "|");
                    if (cacheOrgBridge.ContainsKey(orgKey))
                        continue;

                    DataRow orgBridgeRow = dsOrgBridge.Tables[0].NewRow();

                    int bridgeId = GetGeneratorNextValue(clsOrgBridge);
                    orgBridgeRow["Id"] = bridgeId;
                    orgBridgeRow["SourceId"] = bridgeClsSourceID;
                    orgBridgeRow["ParentId"] = DBNull.Value;
                    orgBridgeRow["Code"] = orgRow["INN"];
                    orgBridgeRow["INN20"] = orgRow["KPP"];
                    orgBridgeRow["Name"] = orgRow["Name"];
                    orgBridgeRow["Code"] = orgRow["INN"];
                    orgBridgeRow["OKATOCode"] = orgRow["OKATO"];
                    orgBridgeRow["MainOKVED"] = orgRow["OKVED"];

                    dsOrgBridge.Tables[0].Rows.Add(orgBridgeRow);
                    cacheOrgBridge.Add(orgKey, bridgeId);
                }

                UpdateDataSet(daOrgBridge, dsOrgBridge, clsOrgBridge);
                ClearDataSet(ref dsOrgBridge);
            }
            finally
            {
                cacheOrgBridge.Clear();
            }

            WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeSuccefullFinished,
                "Завершение формирования классификатора \"Организации.Сопоставимый\" из \"Организации.УФК_Плательщики\"");
        }

        #endregion Копирование записей из классификатора Организации.УФК_Плательщики

        #region Установка иерархии

        /*
        #region Инициализация DataAdapter и заполнение DataSet

        // инициализация Insert-запроса
        private void InitInsertCommand(string[] fields)
        {
            daOrgBridge.InsertCommand =
                this.DB.InitInsertCommand(this.DB.Transaction, clsOrgBridge.FullDBName, clsOrgBridge.Attributes);
            string queryInsert = string.Format("INSERT INTO {0} ({1}) VALUES (:{2})",
                clsOrgBridge.FullDBName, String.Join(", ", fields), String.Join(", :", fields));
            daOrgBridge.InsertCommand.CommandText = queryInsert;
        }

        // инициализация Update-запроса
        private void InitUpdateCommand(string[] fields)
        {
            daOrgBridge.UpdateCommand =
                this.DB.InitUpdateCommand(this.DB.Transaction, clsOrgBridge.FullDBName, clsOrgBridge.Attributes);
            string[] updateFields = new string[fields.GetLength(0)];
            for (int i = 0; i < fields.GetLength(0); i++)
                updateFields[i] = string.Concat(fields[i], "=:", fields[i]);
            string queryUpdate = string.Format("UPDATE {0} SET {1} WHERE ID = :RefId",
                clsOrgBridge.FullDBName, String.Join(", ", updateFields));
            daOrgBridge.UpdateCommand.CommandText = queryUpdate;
        }

        // Массив полей необходимых для установки иерархии
        private string[] NEEDED_FIELDS = new string[] {
            "ID", "ParentID", "SourceID", "Code", "INN20", "OKATOCode", "Name", "MainOKVED" };

        // Запрашиваем данные классификатора.
        // Для экономии памяти выбираем только нужные поля (указаны в массиве NEEDED_FIELDS)
        private void QueryOrgBridgeData()
        {
            // Упорядочиваем записи по полю Code (Инн), чтобы в дальнейшем их было проще сгруппировать
            // записи с Инн = 0 игнорируем, т.к. для них иерархия не устанавливается
            string querySelect = string.Format(
                "SELECT {0} FROM {1} WHERE code <> 0 ORDER BY code ASC",
                String.Join(", ", NEEDED_FIELDS), clsOrgBridge.FullDBName);
            InitLocalDataAdapter(this.DB, ref daOrgBridge, querySelect);

            // Инициализируем SQL-запросы с ограниченным набором полей
            InitInsertCommand(NEEDED_FIELDS);
            InitUpdateCommand(NEEDED_FIELDS);

            // Заполняем датасет
            dsOrgBridge = new DataSet();
            daOrgBridge.Fill(dsOrgBridge);
        }

        #endregion Инициализация DataAdapter и заполнение DataSet

        // Формируем вспомогательные кэши для установки иерархии
        // Группируем записи классификатор по ИНН и ищем Id родительской записи для каждой группы
        private Dictionary<long, int> GetCacheGroupByInn(ref Dictionary<long, DataRow[]> cacheGroupByInn)
        {
            bool toChangeId = true;
            Dictionary<long, int> cacheParentId = new Dictionary<long, int>();
            foreach (DataRow row in dsOrgBridge.Tables[0].Rows)
            {
                int id = Convert.ToInt32(row["id"]);
                long inn = Convert.ToInt64(row["code"]);
                string kpp = row["inn20"].ToString();
                string kppPart = string.Empty;
                if (kpp.Length > 3)
                    kppPart = kpp.Remove(kpp.Length - 3);
                if (!cacheGroupByInn.ContainsKey(inn))
                {
                    // по умолчанию в качестве родительской берем первую запись из группы
                    cacheParentId.Add(inn, id);
                    cacheGroupByInn.Add(inn, new DataRow[] { row });
                    toChangeId = true;
                }
                else
                {
                    cacheGroupByInn[inn] = (DataRow[])CommonRoutines.ConcatArrays(
                        cacheGroupByInn[inn], new DataRow[] { row });
                    if (!toChangeId)
                        continue;
                    if (kppPart == string.Empty)
                        continue;
                    // если нашли КПП = ХХХХ01ХХХ, то устанавливаем Id с таким КПП родительским
                    // и продолжаем поиск, т.к. можем найти ещё запись с КПП = ХХХХ50ХХХ
                    if (kppPart.EndsWith("01"))
                        cacheParentId[inn] = id;
                    // если нашли КПП = ХХХХ50ХХХ, то запись с таким КПП и будет родительской
                    // больше Id родительской записи не ищем
                    if (kppPart.EndsWith("50"))
                    {
                        cacheParentId[inn] = id;
                        toChangeId = false;
                    }
                }
            }
            return cacheParentId;
        }

        // Ищем в группе по ИНН хотя бы одну запись с установленной иерархией
        // Если такая запись найдена, то возвращаем Id её родителя, иначи -1
        private int GetParentRowId(DataRow[] innGroup)
        {
            foreach (DataRow row in innGroup)
                if (row["ParentId"] != DBNull.Value)
                    return Convert.ToInt32(row["ParentId"]); ;
            return -1;
        }

        private void SetOrgBridgeHierarchy()
        {
            WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeInformation,
                "Старт установки иерархии классификатора \"Организации.Сопоставимый\"");

            // Запрос данных классификатора
            QueryOrgBridgeData();

            // Заполняем кэши

            // кэш записей классификатора: ключ - ID, значение - запись
            Dictionary<int, DataRow> cacheOrgBridgeRow = new Dictionary<int, DataRow>();
            FillRowsCache(ref cacheOrgBridgeRow, dsOrgBridge.Tables[0], "Id");
            // кэш записей, сгруппированных по ИНН: ключ - ИНН, значение - массив записей
            Dictionary<long, DataRow[]> cacheGroupByInn = new Dictionary<long, DataRow[]>();
            // кэш id-шников родительских записей: ключ - ИНН группы, значение - Id родительской записи в группе
            Dictionary<long, int> cacheParentId = GetCacheGroupByInn(ref cacheGroupByInn);
            // кэш иерархии: ключ - Id подчиненной записи, значение - Id родительской записи
            Dictionary<int, int> hierarchy = new Dictionary<int, int>();

            try
            {
                foreach (KeyValuePair<long, int> item in cacheParentId)
                {
                    DataRow[] innGroup = cacheGroupByInn[item.Key];
                    // если в группе не более одной записи, то иерархию устанавливать не надо
                    if (innGroup.Length <= 1)
                        continue;

                    DataRow parentRow = null;
                    // пытаемся получить Id родительской записи для группы,
                    // в случае если иерархия ранее была установлена
                    int parentId = GetParentRowId(innGroup);
                    // если есть такая запись, то берём её в качестве родительской
                    if (parentId != -1)
                    {
                        parentRow = cacheOrgBridgeRow[parentId];
                    }
                    // если же нет такой записи, то создаем новую
                    else
                    {

                        parentRow = dsOrgBridge.Tables[0].NewRow();
                        parentId = GetGeneratorNextValue(clsOrgBridge);
                        dsOrgBridge.Tables[0].Rows.Add(parentRow);
                    }

                    // Заполняем поля родительской записи
                    DataRow futureParentRow = cacheOrgBridgeRow[item.Value];
                    CopyRowToRow(futureParentRow, parentRow);
                    parentRow["ID"] = parentId;
                    parentRow["ParentId"] = DBNull.Value;

                    // Подчиняем все записи группы родительской записи
                    foreach (DataRow childRow in innGroup)
                    {
                        int childRowId = Convert.ToInt32(childRow["Id"]);
                        if (childRowId != parentId)
                            hierarchy.Add(childRowId, parentId);
                    }
                }
                // сохраняем новые добавленные родительские записи
                // прежде чем проставим ссылки на них в подчиненных записях
                UpdateDataSet(daOrgBridge, dsOrgBridge, clsOrgBridge);
                foreach (KeyValuePair<int, int> hier in hierarchy)
                    cacheOrgBridgeRow[hier.Key]["ParentId"] = hier.Value;
                UpdateDataSet(daOrgBridge, dsOrgBridge, clsOrgBridge);
                ClearDataSet(ref dsOrgBridge);
            }
            finally
            {
                hierarchy.Clear();
                cacheGroupByInn.Clear();
                cacheParentId.Clear();
                cacheOrgBridgeRow.Clear();
                GC.Collect();
            }

            UpdateOkatoData();

            WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeInformation,
                "Завершение установки иерархии классификатора \"Организации.Сопоставимый\"");
        }
        */

        #endregion Установка иерархии

        private void FillOrgBridge()
        {
            // Заполняем классификатор из Dbf-файла
            if (!bridgePumpedFromDbf)
                PumpPayers();
            // Заполняем классификатор из Организации.УФК_Плательщики
            CopyFromPayers();
            // Устанавливаем иерархию
            // пока закомментируем, решили сделать классификатор плоским
            // SetOrgBridgeHierarchy();
        }

        #endregion формирование классификатора "Организации.Сопоставимый"

        #region расщепление

        private string GetDateConstraint(bool isDraft)
        {
            string dateConstraint = string.Empty;
            // если закачки не было, берем ограничение из параметров обработки
            if (!this.StagesQueue[PumpProcessStates.PumpData].IsExecuted)
            {
                int dateRefMin = -1;
                int dateRefMax = -1;
                if (year > 0)
                {
                    dateRefMin = year * 10000 + (month) * 100;
                    if (month > 0)
                        dateRefMax = year * 10000 + (month) * 100 + 33;
                    else
                        dateRefMax = (year + 1) * 10000;
                }
                if (dateRefMin != -1)
                    dateConstraint = string.Format("RefFKDay >= {0} and RefFKDay < {1}", dateRefMin, dateRefMax);
            }
            else if (pumpedDateList.ContainsKey(this.SourceID))
            {
                // закачка была, получаем даты из списка закачанных дат
                List<string> constraint = new List<string>();
                if (isDraft)
                {
                    // для таблицы фактов "Платежные поручения" выбираем только даты
                    foreach (string date in pumpedDateList[this.SourceID])
                    {
                        string[] pumpedDate = date.Split('|');
                        if (pumpedDate[1] != "0")
                            continue;
                        constraint.Add(string.Format(" (RefFKDay = {0}) ", pumpedDate[0]));
                    }
                }
                else
                {
                    // для таблицы фактов "Возвраты и зачеты" учитываем еще и вид поступлений (RefFXTypes),
                    // т.к. в одной таблице хранятся данные 3-х видов: возвраты, зачёты и зачёты внутри счёта
                    // с каждым из них работает отдельно
                    foreach (string date in pumpedDateList[this.SourceID])
                    {
                        string[] pumpedDate = date.Split('|');
                        if (pumpedDate[1] == "0")
                            continue;
                        constraint.Add(string.Format(" ((RefFKDay = {0}) and (RefFXTypes = {1})) ", pumpedDate[0], pumpedDate[2]));
                    }
                }
                dateConstraint = string.Join(" or ", constraint.ToArray());
            }
            if (dateConstraint != string.Empty)
                dateConstraint = string.Format("({0})", dateConstraint);
            return dateConstraint;
        }

        private int SetRegTerrType(string okato)
        {
            DataRow regionsForPumpRow = null;
            if (regionsForPumpCache.ContainsKey(okato))
            {
                regionsForPumpRow = regionsForPumpCache[okato];
            }
            else
            {
                regionsForPumpRow = PumpCachedRow(regionsForPumpCache, dsRegionsForPump.Tables[0], clsRegionsForPump,
                    okato, new object[] { "SOURCEID", regionsForPumpSourceID, "OKATO", okato, 
                    "NAME", constDefaultClsName, "REFTERRTYPE", 0 }, false);
                WriteToBadOkatoCodesCache(badOkatoCodesCache, okato);
            }
            int terrType = Convert.ToInt32(regionsForPumpRow["REFTERRTYPE"]);
            if (terrType == 0)
                WriteToBadOkatoCodesCache(nullTerrTypeOkatoCodesCache, okato);
            return terrType;
        }

        private void SetRegionsTerrType()
        {
            Dictionary<string, int> orgOkatoCache = null;
            FillRowsCache(ref orgOkatoCache, dsOrg.Tables[0], "Okato");
            foreach (KeyValuePair<string, int> item in orgOkatoCache)
            {
                string okato = item.Key;
                int terrType = SetRegTerrType(okato);
            }
        }

        private void QuerySourceData(string constr, string refKDFieldName)
        {
            this.SetProgress(-1, -1, "Запрос данных для расщепления...", string.Empty, true);
            WriteToTrace("Запрос данных для расщепления...", TraceMessageKind.Information);

            // Инициализация адаптеров
            // Адаптер для просмотра данных для расщепления (с подтягиванием КД и окато)
            string fctTableName = fctSourceTable.FullDBName;
            string kdTableMame = clsKD.FullDBName;
            string orgTableName = clsOrg.FullDBName;
            string str = string.Format("select {0}.*, k.CODESTR KdCode, o.okato OrgOkato " +
                "from {0} left join {1} k on (k.id = {0}.{2}) " +
                "         left join {3} o on (o.id = {0}.{4}) ",
                fctTableName, kdTableMame, refKDFieldName, orgTableName, "RefOrgUFK");


            if (constr != string.Empty)
            {
                str += string.Format(" where {0}", constr);
            }
            str += string.Format(
                " order by {0}.{1} asc, {0}.sourceid asc", fctSourceTable.FullDBName, refKDFieldName);

            InitLocalDataAdapter(this.DB, ref daSourceData, str);
            ClearDataSet(ref dsSourceData);
            daSourceData.Fill(dsSourceData);

            WriteToTrace("Запрос данных для расщепления окончен.", TraceMessageKind.Information);
        }

        private void DisintRow(DataRow sourceRow, DataRow disintRow, string[] fieldsForDisint, bool isDrafts)
        {
            DataRow row = null;
            if (!regionsForPumpCache.ContainsKey(currentOkatoCode))
                return;
            DataRow regionRow = regionsForPumpCache[currentOkatoCode]; 
            int terrType = Convert.ToInt32(regionRow["REFTERRTYPE"]);
            // у неуказанного типа территории ничего не делаем
            if (terrType == 0)
                return;
            // Обрабатываем все проценты расщепления
            for (int j = 1; j <= 15; j++)
            {
                if (j == 8)
                    j = 12;
                bool zeroSums = true;
                bool skipRow = false;
                int count = fieldsForDisint.GetLength(0);
                for (int i = 0; i < count; i++)
                {
                    if (!CheckPercentByTerrType(j, terrType))
                    {
                        skipRow = true;
                        break;
                    }
                    if (row == null)
                    {
                        row = dsDisintegratedData.Tables[0].NewRow();
                        CopyRowToRow(sourceRow, row);
                    }
                    double d = Convert.ToDouble(sourceRow[fieldsForDisint[i]]);

                    bool exclusiveBudget = false;
                    if (!isDrafts)
                    {
                        int budLevel = Convert.ToInt32(sourceRow["RefBdgtLevels"]);
                        if (budLevel == 0)
                            exclusiveBudget = false;
                        if (budLevel == 14)
                            exclusiveBudget = true;
                    }

                    switch (j)
                    {
                        case 1:
                            if (exclusiveBudget)
                                continue;
                            d *= Convert.ToDouble(disintRow["FED_PERCENT"]) / 100;
                            break;
                        case 2:
                            if (exclusiveBudget)
                                continue;
                            d *= Convert.ToDouble(disintRow["CONS_PERCENT"]) / 100;
                            break;
                        case 3:
                            if (exclusiveBudget)
                                continue;
                            d *= Convert.ToDouble(disintRow["SUBJ_PERCENT"]) / 100;
                            break;
                        case 4:
                            d *= Convert.ToDouble(disintRow["CONSMR_PERCENT"]) / 100;
                            break;
                        case 5:
                            d *= Convert.ToDouble(disintRow["MR_PERCENT"]) / 100;
                            break;
                        case 6:
                            d *= Convert.ToDouble(disintRow["STAD_PERCENT"]) / 100;
                            break;
                        case 7:
                            if (exclusiveBudget)
                                continue;
                            d *= Convert.ToDouble(disintRow["OUTOFBUDGETFOND_PERCENT"]) / 100;
                            break;
                        case 12:
                            if (exclusiveBudget)
                                continue;
                            d *= Convert.ToDouble(disintRow["SMOLENSKACCOUNT_PERCENT"]) / 100;
                            break;
                        case 13:
                            if (exclusiveBudget)
                                continue;
                            d *= Convert.ToDouble(disintRow["TUMENACCOUNT_PERCENT"]) / 100;
                            break;
                        case 14:
                            d *= Convert.ToDouble(disintRow["CONSMO_PERCENT"]) / 100;
                            break;
                        case 15:
                            // для типа территорий ГП, СП, Районный центр -  сумма по этому уровню бюджета отсутствует
                            if ((terrType == 5) || (terrType == 6) || (terrType == 10))
                                d = 0;
                            else
                                d *= Convert.ToDouble(disintRow["GO_PERCENT"]) / 100;
                            break;
                    }
                    if (d != 0)
                        zeroSums = false;
                    row[fieldsForDisint[i]] = d;
                }
                if (!zeroSums && !skipRow)
                {
                    row["SOURCEKEY"] = sourceRow["ID"];
                    row[refBudgetLevelFieldName] = j;
                    if (isDrafts)
                    {
                        row["RefFXTypes"] = 1;
                        row["RefPaymentSign"] = sourceRow["PaymentSign"];
                    }
                    row["RefIsDisint"] = 1;

                    dsDisintegratedData.Tables[0].Rows.Add(row);

                    // Если накопилось много расщепленных записей, то сбрасываем их в базу
                    if (dsDisintegratedData.Tables[0].Rows.Count >= constMaxQueryRecordsForDisint)
                    {
                        UpdateOkatoData();
                        UpdateDataSet(daDisintegratedData, dsDisintegratedData, fctDisintegratedData);
                        ClearDataSet(daDisintegratedData, ref dsDisintegratedData);
                    }
                }
                row = null;
            }
            this.DB.ExecQuery(
                string.Format("update {0} set {1} = 1 where ID = ?", fctSourceTable.FullDBName, disintFlagFieldName),
                QueryResultTypes.NonQuery,
                this.DB.CreateParameter("ID", sourceRow["ID"], DbType.Int64));
        }

        private void DisintData(string[] fieldsForDisint, bool isDrafts)
        {
            // Счетчик записей
            int recCount = 0;
            DataRow disintRowEx = null;

            disintCount = 0;
            totalRecsForSourceID = 0;

            string constr = string.Format("{0}.SOURCEID = {1} and {0}.{2} = 0",
                fctSourceTable.FullDBName, this.SourceID, disintFlagFieldName);
            if (disintDateConstraint != string.Empty)
                constr += string.Format(" and {0}", disintDateConstraint);

            // Узнаем, есть ли данные для расщепления и сколько их
            // Всего записей
            int totalRecs = Convert.ToInt32(this.DB.ExecQuery(string.Format(
                "select count(id) from {0} where {1}", fctSourceTable.FullDBName, constr), QueryResultTypes.Scalar));
            if (totalRecs == 0)
            {
                WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeCriticalError, "Нет данных для расщепления.");
                return;
            }

            WriteToTrace(string.Format("Записей для расщепления: {0}.", totalRecs), TraceMessageKind.Information);

            // Узнаем значение первого ИД
            int firstID = Convert.ToInt32(this.DB.ExecQuery(string.Format(
                "select min(ID) from {0} where {1}", fctSourceTable.FullDBName, constr), QueryResultTypes.Scalar));
            // Верхнее ограничение ИД для выборки
            int lastID = firstID + constMaxQueryRecordsForDisint - 1;

            // Таблица расщепленных записей
            InitFactDataSet(ref daDisintegratedData, ref dsDisintegratedData, fctDisintegratedData);

            do
            {
                // Ограничение запроса для выборки порции данных
                string restrictID = string.Format(
                    "{0}.ID >= {1} and {0}.ID <= {2} and {3}", fctSourceTable.FullDBName, firstID, lastID, constr);
                firstID = lastID + 1;
                lastID += constMaxQueryRecordsForDisint;

                QuerySourceData(restrictID, refKDFieldName);

                if (dsSourceData.Tables[0].Rows.Count == 0)
                    continue;

                // Расщепляем полученные данные
                for (int i = 0; i < dsSourceData.Tables[0].Rows.Count; i++)
                {
                    recCount++;

                    DataRow sourceRow = dsSourceData.Tables[0].Rows[i];

                    this.SetProgress(totalRecs, recCount,
                        string.Format("Обработка данных (ID источника {0})...", this.SourceID),
                        string.Format("Запись {0} из {1}", recCount, totalRecs));

                    // По записи факта берем организацию, из нее окато района; КД, год и месяц. Формируем дату - первое число этого года 
                    // и этого месяца (для поиска уточнений)
                    currentOkatoCode = Convert.ToString(sourceRow["OrgOkato"]);
                    string kd = Convert.ToString(sourceRow["KdCode"]);
                    string date = Convert.ToString(sourceRow[refDateFieldName]);
                    int year = Convert.ToInt32(date.Substring(0, 4));

                    totalRecsForSourceID++;

                    // Ищем правила расщепления, удовлетворяющие условиям: КД, OКАТО района такое как у нашего района, Год,
                    // Если запись уточнения, то берем только те, где поле "дата, с которого действует уточнение" 
                    // меньше или равна нашей дате.
                    DataRow disintRow = FindDisintRule(disintRulesCache, year, kd);

                    if (disintRow == null)
                    {
                        // Не найдено не одного расщепления - пишем в протокол некритическую ошибку
                        WriteRecInMessagesDS(date, year, this.SourceID, kd);
                        continue;
                    }
                    else
                    {
                        // Ищем уточнение
                        disintRowEx = GetDisintExRow(disintRow, Convert.ToInt32(date), currentOkatoCode.PadLeft(5, '0'));
                        // Если нашли уточнение, то его и используем
                        if (disintRowEx != null)
                            disintRow = disintRowEx;
                    }

                    // Расщепляем строку
                    DisintRow(sourceRow, disintRow, fieldsForDisint, isDrafts);
                    disintCount++;
                }
            }
            while (recCount < totalRecs);

            UpdateDataSet(daRegionsForPump, dsRegionsForPump, clsRegionsForPump);
            UpdateDataSet(daDisintegratedData, dsDisintegratedData, fctDisintegratedData);
        }

        private void PrepareDisintedData(bool disintAll, string constr)
        {
            string query = string.Format("update {0} set {1} = 0 where SOURCEID = {2}",
                fctSourceTable.FullDBName, disintFlagFieldName, this.SourceID);
            if (disintDateConstraint != string.Empty)
                query += " and " + disintDateConstraint;
            if (this.StagesQueue[PumpProcessStates.PumpData].IsExecuted || disintAll)
            {
                string q = "(RefIsDisint = 1) ";
                if (disintDateConstraint != string.Empty)
                    q += " and " + disintDateConstraint;
                DeleteTableData(fctDisintegratedData, -1, this.SourceID, q + constr);
                this.DB.ExecQuery(query, QueryResultTypes.NonQuery);
            }
            if (!this.StagesQueue[PumpProcessStates.PumpData].IsExecuted && disintAll)
            {
                this.DB.ExecQuery(query, QueryResultTypes.NonQuery);
            }
        }

        private void DisintData()
        {
            WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeInformation, "начало расщепления сумм");

            // Кэш правил расщепления: ключ - год, значение - список правил (ключ - код КД, значение - строка правила)
            FillDisintRulesCache();

            CheckDisintRulesCache();
            PrepareMessagesDS();
            PrepareBadOkatoCodesCache();
            PrepareRegionsForSumDisint();
            disintFlagFieldName = "RefIsDisint";
            refBudgetLevelFieldName = "RefBdgtLevels";

            this.disintDateConstraint = GetDateConstraint(true);
            this.fctSourceTable = fctDrafts;
            this.fctDisintegratedData = fctPayers;
            this.clsKD = clsKd;
            this.refDateFieldName = "RefFKDay";
            this.refKDFieldName = "RefKD";

            PrepareDisintedData(disintAll, string.Empty);
            SetRegionsTerrType();

            // Платежные поручения
            DisintData(new string[] { "ForPeriod" }, true);

            // Возвраты и зачеты
            this.disintDateConstraint = GetDateConstraint(false);
            this.fctSourceTable = fctRepayment;
            PrepareDisintedData(disintAll, " and RefFXTypes <> 1");
            DisintData(new string[] { "ForPeriod" }, false);

            UpdateProcessedData();
            WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeInformation, "завершение расщепления сумм");
        }

        #endregion расщепление

        protected override void ProcessDataSource()
        {
            // проставление ссылки на фикс клс у таблицы фактов "Доходы.УФК_Налогоплательщики_Платежные поручения"
            SetPaymentSign(fctDrafts);
            // расщепление
            if (toDisintData)
            {
                DisintData();
            }
            // установка иерархии организаций
            if (toProcessOrgCls)
            {
                SetOrgHierarchy();
                UpdateProcessedData();
            }
            // формирование классификатора Организации.Сопоставимый
            if (toFillOrgBridgeCls)
            {
                FillOrgBridge();
            }
        }

        private string GetProcessComment()
        {
            string message = string.Empty;
            if (toDisintData)
                message += "Расщепление сумм по нормативам отчисления доходов";
            if (toProcessOrgCls)
            {
                if (message != string.Empty)
                    message += "; ";
                message += "Преобразование классификатора 'Организации.УФК плательщики' ";
            }
            if (toFillOrgBridgeCls)
            {
                if (message != string.Empty)
                    message += "; ";
                message += "Формирование классификатора 'Организации.Сопоставимый' ";
            }
            return message;
        }

        protected override void DirectProcessData()
        {
            year = -1;
            month = -1;
            GetPumpParams(ref year, ref month);
            if (!this.StagesQueue[PumpProcessStates.PumpData].IsExecuted)
                GetDisintParams(ref year, ref month, ref disintAll);
            toProcessOrgCls = Convert.ToBoolean(GetParamValueByName(this.PumpRegistryElement.ProgramConfig, "cbProcessOrgCls", "False"));
            toDisintData = Convert.ToBoolean(GetParamValueByName(this.PumpRegistryElement.ProgramConfig, "cbDisintData", "False"));
            toFillOrgBridgeCls = Convert.ToBoolean(GetParamValueByName(this.PumpRegistryElement.ProgramConfig, "cbFillOrgBridgeCls", "False"));
            string comment = GetProcessComment();
            ProcessDataSourcesTemplate(year, month, comment);
        }

        #endregion Обработка данных

        #region сопоставление

        protected override int GetClsSourceID(int sourceID)
        {
            if (sourceID <= 0)
                return -1;
            IDataSource ds = this.Scheme.DataSourceManager.DataSources[sourceID];
            IDataSource clsDs = FindDataSource(ParamKindTypes.Year, ds.SupplierCode, ds.DataCode, string.Empty, ds.Year, 0, string.Empty, 0, string.Empty);
            if (clsDs == null)
                return -1;
            return clsDs.ID;
        }

        #endregion сопоставление

    }

}
