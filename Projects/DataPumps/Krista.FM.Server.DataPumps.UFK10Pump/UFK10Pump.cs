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

    // ��� - 0010 - ������ �� �������� ������������������
    public class UFK10PumpModule : CorrectedPumpModuleBase
    {

        #region ����

        #region ��������������

        // ��.��� (d_KD_UFK)
        private IDbDataAdapter daKd;
        private DataSet dsKd;
        private IClassifier clsKd;
        private Dictionary<string, int> cacheKd = null;
        // �����������.���_����������� (d_Org_UFKPayers)
        private IDbDataAdapter daOrg;
        private DataSet dsOrg;
        private IClassifier clsOrg;
        private Dictionary<string, int> cacheOrg = null;
        // �����.��� (d_OKATO_UFK)
        private IDbDataAdapter daOkato;
        private DataSet dsOkato;
        private IClassifier clsOkato;
        private Dictionary<string, int> cacheOkato = null;
        // �����������.������������ (b_Organizations_Bridge)
        private IDbDataAdapter daOrgBridge;
        private DataSet dsOrgBridge;
        private IClassifier clsOrgBridge;
        private Dictionary<string, int> cacheOrgBridge = null;
        // ���.��������� ������ (d_Kind_TaxPayment)
        private IDbDataAdapter daTax;
        private DataSet dsTax;
        private IClassifier clsTax;
        private Dictionary<string, int> cacheTax = null;
        // ������.������������ ������������ ���� (d_Date_ConversionFK)
        private IDbDataAdapter daPeriod;
        private DataSet dsPeriod;
        private IClassifier clsPeriod;
        private Dictionary<int, int> cachePeriod = null;

        #endregion ��������������

        #region �����

        // ������.���_�����������������_��������� ��������� (f_D_UFK10Drafts)
        private IDbDataAdapter daDrafts;
        private DataSet dsDrafts;
        private IFactTable fctDrafts;
        // ������.���_�����������������_����������� � ������� (f_D_UFK10Taxpayers)
        private IDbDataAdapter daPayers;
        private DataSet dsPayers;
        private IFactTable fctPayers;
        // ������.���_�����������������_�������� � ������ (f_D_UFK10Repayment)
        private IDbDataAdapter daRepayment;
        private DataSet dsRepayment;
        private IFactTable fctRepayment;

        #endregion �����

        private int curRow;
        private Database dbfDatabase = null;
        protected DBDataAccess dbDataAccess = new DBDataAccess();
        private int clsSourceId;
        private List<string> deletedDateList = null;
        private List<int> deletedDateListCsv = null;
        // ������ ������������ ���, ��������������� �� SourceId
        // ������������ ��� �����������
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
        // ������ ������������� ��������������
        private int? bridgeClsSourceID = -1;
        private bool bridgePumpedFromDbf = false;

        #endregion ����

        #region ���������, ������������

        private enum ReportType
        {
            // ������ ������ ����� - � ������� "������.���_�����������������_�������� � ������"
            CC,
            // ��������� ��������� - � ������� "������.���_�����������������_��������� ���������"
            PP,
            // �������� - � ������� "������.���_�����������������_�������� � ������"
            VV,
            // ������ - � ������� "������.���_�����������������_�������� � ������"
            ZZ
        }

        #endregion ���������, ������������

        #region ��������

        private delegate void ProcessDbfDelegate(DataRow row, int fileDate);

        // ������� ��������� ������ Csv-������
        private delegate void PumpCsvDataRow(Dictionary<string, string> row, int refDate);

        #endregion ��������

        #region ������� ������

        #region ������ � ����� � ������

        protected override void QueryData()
        {
            bridgeClsSourceID = this.Scheme.DataVersionsManager.DataVersions.FindCurrentVersion(clsOrgBridge.ObjectKey);
            clsSourceId = AddDataSource("���", "0010", ParamKindTypes.Year,
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
                    "�� �������� ������������� '������.������������ ������������ ����', ���� �� ����� ��������������� �� ��� ��.");
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

            // ����� ������������ �� � ���.��������� ������ - �������� ���
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

        #endregion ������ � ����� � ������

        #region ������ � Dbf

        #region ����� ������

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
                case "��������������� ������������ �����":
                    return 7;
                case "�������":
                    return 14;
                case "��������� ������":
                    return 15;
                case "�������":
                case "������� ��":
                    return 3;
                case "������������� ������":
                    return 5;
                case "���������� ����":
                    return 8;
                case "�������� ���������":
                    return 17;
                case "��������������� ���� ���. �����.":
                    return 11;
                case "�����������":
                    return 1;
                case "���� ������ ������������ �����������":
                    return 10;
                case "���� ����������� �����������":
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

        #endregion ����� ������

        #region ������� ���������������

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

        #endregion ������� ���������������

        #region ������� ������

        #region ��������� ���������

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

        #endregion ��������� ���������

        #region �������� � ������

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
            ProcessDBF(GetRepayMapping(), file.Name, PumpRepayRow, "(LEV_NAME8 = '�������')");
        }

        #endregion �������� � ������

        #region ����������� � �������

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
                "(LEV_NAME8 <> '�������') and (LEV_NAME8 <> '')");
        }

        #endregion ����������� � �������

        #endregion ������� ������

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

        #endregion ������ � Dbf

        #region ������ � Cvs

        private long ValidateInt(string field, string value)
        {
            try
            {
                return Convert.ToInt64(value.Trim().PadLeft(1, '0'));
            }
            catch
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, string.Format(
                   "��� ��������� ������ {0} �������� ������: �������� � ���� '{1}' ({2}) ����� �������� ������. " +
                   "���� '{1}' ����� ��������� �������� �� ��������� (0).", curRow + 1, field, value));
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
                    "�������� � ���� '{0}' ({1}) ����� �������� ������", field, value));
            }
        }

        private string ValidateString(string field, string value, int maxLength)
        {
            if (value.Length > maxLength)
            {
                throw new InvalidDataException(string.Format(
                    "�������� � ���� '{0}' ({1}) ������� ������ (�����������: {2}, ������������: {3})",
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

        // ���������� ��������� ��� �������� ������� �� ����� � ������
        Regex regExNonDigit = new Regex(@"\D", RegexOptions.IgnoreCase);
        // �������� ������ � ������������� ��.���
        private int PumpCsvKd(string codeStr)
        {
            codeStr = ValidateString("��� ��", codeStr.Trim().PadLeft(1, '0'), 20);
            if (regExNonDigit.IsMatch(codeStr))
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, string.Format(
                   "��� ��������� ������ {0} �������� ������: �������� � ���� '��� ��' ({1}) ����� �������� ������. " +
                   "���� '��� ��' ����� ��������� �������� �� ��������� (0).", curRow + 1, codeStr));
                codeStr = "0";
            }
            object[] mapping = new object[] { "CodeStr", codeStr, "Name", constDefaultClsName, "SourceId", clsSourceId };
            return PumpCachedRow(cacheKd, dsKd.Tables[0], clsKd, mapping, codeStr, "ID");
        }

        // �������� ������ � ������������� ���.��������� ������
        private int PumpCsvTaxPayment(string codeStr)
        {
            codeStr = codeStr.Trim().TrimStart('0').PadLeft(1, '0').ToUpper();
            codeStr = ValidateString("��� �������", codeStr, 20);
            object[] mapping = new object[] { "CodeStr", codeStr, "SourceId", clsSourceId };
            return PumpCachedRow(cacheTax, dsTax.Tables[0], clsTax, mapping, codeStr, "ID");
        }

        // �������� ������ � ������������� �����������.���_�����������
        private int PumpCsvOrgUfk(string inn, string kpp, string okato, string name)
        {
            long innInt = ValidateInt("���", inn);
            long kppInt = ValidateInt("���", kpp);
            long okatoInt = ValidateInt("�����", okato);
            name = ValidateString("��� ����", name, 765);

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

        // �������� ������ � ������������� �����.���
        private int PumpCsvOkato(string okato)
        {
            long okatoInt = ValidateInt("�����", okato);
            object[] mapping = new object[] { "Code", okatoInt, "Account", DBNull.Value,
                "DutyAccount", DBNull.Value, "Name", constDefaultClsName, "SourceId", clsSourceId };
            return PumpCachedRow(cacheOkato, dsOkato.Tables[0], clsOkato, mapping, okatoInt.ToString(), "ID");
        }

        // �������� ������ � ������� ������ D.UFK10Drafts �� ������ "p_*.csv"
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
                "ForPeriod", ValidateFloat("����� �� ������", row["Summa"]),
                "DocDate", row["Date_PP"],
                "DocNumber",  ValidateString("����� ���������", row["N_PP"], 30),
                "PayPurpose", ValidateString("���������� �������", row["Nazn_Plat"], 255),
                "ChargeBasis", ValidateString("��������� �������", row["Osnov_Plat"], 20),
                "TaxPeriod", ValidateString("��������� ������", row["Tax_Period"], 20),
                "KBK", ValidateString("���", row["KBK"], 20),
                "OKATO", ValidateString("OKATO ��", row["OKATO_PP"], 20),
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

        // �������� ������ � ������� ������ D.UFK10Repayment �� ������ "v_*.csv"
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
                "ForPeriod", ValidateFloat("����� �� ������", row["Summa_Isp"]),
                "SummP", ValidateFloat("����� ���", row["Summa_PZV"]),
                "DocDate", row["Date"],
                "DocNumber", ValidateString("����� ���������", row["N"], 30),
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

        // �������� ������ � ������� ������ D.UFK10Repayment �� ������ "z_*.csv"
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
                "ForPeriod", ValidateFloat("����� �� ������", row["Summa_Isp"]),
                "SummP", ValidateFloat("����� ���", row["Summa_PDA"]),
                "DocDate", row["Date"],
                "DocNumber", ValidateString("����� ���������", row["N_RUD"], 30),
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

        // �������� ������ � ������� ������ D.UFK10Repayment �� ������ "c_*.csv"
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
                "ForPeriod", ValidateFloat("����� �� ������", row["Summa_PSCh"]),
                "SummP", ValidateFloat("����� ���", row["Summa_PZV"]),
                "DocDate", row["Date"],
                "DocNumber",  ValidateString("����� ���������", row["N"], 30),
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

        #region �������� �����

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

        #endregion �������� �����
        private const string CSV_REPORT_DELIMITER = "\",\"";
        private Dictionary<string, string> GetCsvDataRow(string reportRow, object[] mapping)
        {
            Dictionary<string, string> dataRow = new Dictionary<string, string>();

            string[] rowValues = reportRow.Split(new string[] { CSV_REPORT_DELIMITER }, StringSplitOptions.None);
            rowValues[0] = rowValues[0].TrimStart(new char[] { '"' });
            rowValues[rowValues.GetLength(0) - 1] = rowValues[rowValues.GetLength(0) - 1].TrimEnd(new char[] { '"' });

            if (rowValues.GetLength(0) < (mapping.GetLength(0) / 2))
                throw new InvalidDataException("������ ����� �������� ������");

            int columnsCount = mapping.GetLength(0);
            for (int i = 0; i < columnsCount; i += 2)
            {
                dataRow.Add(mapping[i].ToString(), rowValues[Convert.ToInt32(mapping[i + 1])]);
            }

            return dataRow;
        }

        // ������� ������ �� Csv-������
        private void PumpCsvReport(FileInfo file, PumpCsvDataRow pumpDataRow, object[] mapping, int refDate)
        {
            string[] report = CommonRoutines.GetTxtReportData(file, CommonRoutines.GetTxtWinCodePage());
            string dataSourcePath = GetShortSourcePathBySourceID(this.SourceID);
            int rowsCount = report.GetLength(0);
            for (curRow = 1; curRow < rowsCount; curRow++)
                try
                {
                    SetProgress(rowsCount, curRow,
                        string.Format("��������� ����� {0}\\{1}...", dataSourcePath, file.Name),
                        string.Format("������ {0} �� {1}", curRow, rowsCount));

                    if (report[curRow].Trim() == string.Empty)
                        continue;

                    Dictionary<string, string> dataRow = GetCsvDataRow(report[curRow], mapping);
                    pumpDataRow(dataRow, refDate);
                }
                catch (InvalidDataException ex)
                {
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, string.Format(
                        "��� ��������� ������ {0} ����� \"{1}\" �������� ������: {2}. ������ {0} ����� ���������.",
                        curRow + 1, file.Name, ex.Message));
                    continue;
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("��� ��������� ������ {0} �������� ������ ({1})",
                        curRow + 1, ex.Message), ex);
                }
        }

        // �������� ���� �� ��������� ��������� "���" � ����� ����� (x_MMDD.csv)
        private int GetCsvRefDate(string filename)
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning,
                "���� ����� ���������� �� ��������� ��������� \"���\" � �� ����� �����");
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
            WriteToTrace("�������� ���������: " + file.Name, TraceMessageKind.Information);
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

        #endregion ������ � Cvs

        #region ���������� ������ �������

        private void PumpDbfFiles(DirectoryInfo dir)
        {
            // ��� ��� ����� �������� - ���������� ������ (����� visual fox pro, �� ������� �� ����������)
            // ���������� ���������� �� � ������ dbase III ������ ����������� ������ ��������� ����������
            SetDBFEncoding("ANSI");
            // ����� �� �� ����� ��������� �������� �� �� ����� ��������������� � ���������� � ������� ���������
            // �������� ����� �� ��������� ����� � ������ ������
            DirectoryInfo tempDir = CopyFilesToTempDir(dir);
            ReconnectToDbfDataSource(ODBCDriverName.Microsoft_dBase_Driver, tempDir);
            try
            {
                // ��������� ���������
                if (dir.GetFiles("pp*.dbf", SearchOption.AllDirectories).GetLength(0) > 0)
                {
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "������� ��������� ���������");
                    reportType = ReportType.PP;
                    ProcessFilesTemplate(dir, "pp*.dbf", new ProcessFileDelegate(PumpDrafts), false);
                    UpdateData();
                }
                if ((dir.GetFiles("zz*.dbf", SearchOption.AllDirectories).GetLength(0) > 0) ||
                    (dir.GetFiles("vv*.dbf", SearchOption.AllDirectories).GetLength(0) > 0))
                {
                    // �������� � ������
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "������� ��������� � �������");
                    reportType = ReportType.VV;
                    ProcessFilesTemplate(dir, "vv*.dbf", new ProcessFileDelegate(PumpRepayments), false);
                    reportType = ReportType.ZZ;
                    ProcessFilesTemplate(dir, "zz*.dbf", new ProcessFileDelegate(PumpRepayments), false);
                    UpdateData();
                    // ����������� � �������
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "������� ����������� � �������");
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

            // ���� ���� ������������� ������ rar
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

        #endregion ���������� ������ �������

        #endregion ������� ������

        #region ��������� ������

        #region ���������� ������

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
                    string msg = string.Format("�� ������� �� ������ ����������� ��� �� = {0}, ��� = {1} ({2} �������).",
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

        #endregion ���������� ������

        #region ������������ ������ �� ���� ��� � ������� ������ "������.���_�����������������_��������� ���������"

        private bool ContainsSign(string payPurpose, string[] signs)
        {
            foreach (string sign in signs)
                if (payPurpose.ToUpper().Contains(sign.ToUpper()))
                    return true;
            return false;
        }

        private const string PAYMENTS_SIGN = "����;���.��.;�������;���-�� ��;��=;��-4;[_]40911;���-��;" + 
            "�� 4;��4;�� ���������;������� ��;��������;������� ��;�������;�� �������;������;���. ��.;" + 
            "�� -4;�� - 4;� ���.���;� ���. ���;�� ���.���;�� ���. ���;� 31;�.31;�. 31;�31;" + 
            "�.31;�. 31;�31;��������;�-�40911;��.=;����������;������� ���������;����.;���-;" + 
            "���=;�.187;� 187;�187;�� �/�;��. �/�;�� �\\�;��������� �/�;��������� �\\�;� ���������;" + 
            "����������;�������;��������� �������;��������;�����.��;�����. ��;����.��;����. ��;���;" + 
            "������� � ������;��������;�. 187;����;����;��.�/�;�-��;��� ��;����.;������������;������;" +
            "�,31;�,187;���� �/�;�����. �/�;������� �-��;������� ���;���-;� ��;���;� �/�;" + 
            "��������� ��;��������� ��;�����������;�� �����;� ������;� ����������;��������;��������� �/�;" + 
            "�-�;�/�;���;��. �\\�;��-��� �\\�;�������������;�,31;�. �/�;�����.�/�;���-��";
        private void SetPaymentSign(IFactTable fct)
        {
            string semantic = fct.FullCaption;
            WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeStart, 
                string.Format("����� ��������� ����� {0} (��������� ������ �� ������������� �������������).", semantic));
            int totalRecs = Convert.ToInt32(this.DB.ExecQuery(string.Format(
                "select count(id) from {0} where SOURCEID = {1}", fct.FullDBName, this.SourceID), QueryResultTypes.Scalar));
            if (totalRecs == 0)
            {
                WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeInformation,
                    string.Format("��� ������ �� {0}.", semantic));
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
                string.Format("���������� ��������� ����� {0} (��������� ������ �� ������������� �������������).", semantic));
        }

        #endregion ������������ ������ �� ���� ��� � ������� ������ "������.���_�����������������_��������� ���������"

        #region ����������� ��������� �������� �����������

        // �������� �������� ��� �����������: ������ ��� - ����
        // ��� - ���������� � ����
        // �� ��������� ������������ ������ �� �� ������, 
        // ���� ����� ����������� ������ � ��� = ����01��� - ������������ ����, ����50��� - ��� ����� ������������
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

        // ��� - �������� ����������� ���� �� � ����� ������ ������ �� ���
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

        // ��������� �������� �����������.��� (�������� �������� ������, � ������ �� ���������)
        private void SetOrgHierarchy()
        {
            WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeInformation,
                "������ ��������� �������� �������������� '�����������.��� �����������'");
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
                    // ������� ��� �� ������������ 
                    if (orgInn == 0)
                        continue;
                    DataRow[] innRows = cacheInnOrg[orgInn];
                    // ���� ������ �� ��� ������� �� ����� ������ ������ �� ������
                    if (innRows.GetLength(0) == 1)
                        continue;
                    DataRow parentInnRow = null;
                    // ���� ��� ����������� �������� - ������� ������������ ������
                    int parentId = -1;
                    if (IsHierarchied(innRows, ref parentId))
                        parentInnRow = cacheRowOrg[parentId];
                    DataRow orgRow = cacheRowOrg[cacheItem.Value];
                    // ��������� ������������ ������, ���� ����� �� �������
                    if (parentInnRow == null)
                    {
                        parentInnRow = dsOrg.Tables[0].NewRow();
                        parentId = GetGeneratorNextValue(clsOrg);
                        dsOrg.Tables[0].Rows.Add(parentInnRow);
                    }
                    CopyRowToRow(orgRow, parentInnRow);
                    parentInnRow["ID"] = parentId;
                    parentInnRow["ParentId"] = DBNull.Value;
                    // ��������� ��� ������ ������ ������������ ������
                    foreach (DataRow innRow in innRows)
                    {
                        int rowId = Convert.ToInt32(innRow["Id"]);
                        if (rowId != parentId)
                            hierDict.Add(rowId, parentId);
                    }
                }
                UpdateDataSet(daOrg, dsOrg, clsOrg);
                // ������������� ��������
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
                "���������� ��������� �������� �������������� '�����������.��� �����������'");
        }

        #endregion ����������� ��������� �������� �����������

        #region ������������ �������������� "�����������.������������"

        #region ����������

        // ���������� �������������� �� �������� "������������"
        // !!! �������� !!! 
        // ���� ������� �������� ��� ���� ���� ��� ����������, �� ������ �������� ��� � ������ NEEDED_FIELDS
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
                    string.Format("��������� ����� {0}...", fileName),
                    string.Format("������ {0} �� {1}", curRow, rowsCount));

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
                            "��� ��������� ������ {0} �������� ������: �������� ������ �������� � ���� {1} = \"{2}\". " +
                            "���� {1} ����� ��������� �������� �� ��������� ({3}).",
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

                    // ���� ������ � ����� ������������� ��� ����, �� ����������� � ������������ Id ��� ��������
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
        // ������� ����� ������������
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

                // ������ �������, ��� ������������ ������
                // �.�. ������ ������� �����, ������ ����� �� �������
                // dsOrgBridge.Tables[0].Clear();
                // GC.Collect();

                // ���������� �������������� "�����������.������������"
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

        // ������� ������������ �� dbf-����� (������ � ����� "__�����������")
        private const string PAYERS_PATH = "__�����������";
        private const string PAYERS_FILE = "Spisok1.dbf";
        private void PumpPayers()
        {
            bridgePumpedFromDbf = true;
            WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeStart,
                "����� ������������ �������������� \"�����������.������������\"");

            DirectoryInfo[] payers = this.RootDir.GetDirectories(PAYERS_PATH, SearchOption.TopDirectoryOnly);
            // ������� "�����������" ������ ��������������
            if (payers.GetLength(0) == 0)
            {
                WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeWarning, string.Format(
                    "����������� ������� \"{0}\". ������������� \"�����������.������������\" ����������� �� �����", PAYERS_PATH));
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
                        "����������� ���� \"{0}\". ������������� \"�����������.������������\" ����������� �� �����", PAYERS_FILE));
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
                "���������� ������������ �������������� \"�����������.������������\"");
        }

        #endregion ����������

        #region ����������� ������� �� �������������� �����������.���_�����������

        private void CopyFromPayers()
        {
            WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeStart,
                "����� ������������ �������������� \"�����������.������������\" �� \"�����������.���_�����������\"");

            InitDataSet(ref daOrgBridge, ref dsOrgBridge, clsOrgBridge, string.Format("SourceID = {0}", bridgeClsSourceID));
            FillRowsCache(ref cacheOrgBridge, dsOrgBridge.Tables[0], new string[] { "Code", "INN20", "OKATOCode" }, "|", "ID");

            try
            {
                int rowsCount = dsOrg.Tables[0].Rows.Count;
                for (int curRow = 0; curRow < rowsCount; curRow++)
                {
                    SetProgress(rowsCount, curRow,
                        "����������� ������� �� \"�����������.���_�����������\"...",
                        string.Format("������ {0} �� {1}", curRow, rowsCount));

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
                "���������� ������������ �������������� \"�����������.������������\" �� \"�����������.���_�����������\"");
        }

        #endregion ����������� ������� �� �������������� �����������.���_�����������

        #region ��������� ��������

        /*
        #region ������������� DataAdapter � ���������� DataSet

        // ������������� Insert-�������
        private void InitInsertCommand(string[] fields)
        {
            daOrgBridge.InsertCommand =
                this.DB.InitInsertCommand(this.DB.Transaction, clsOrgBridge.FullDBName, clsOrgBridge.Attributes);
            string queryInsert = string.Format("INSERT INTO {0} ({1}) VALUES (:{2})",
                clsOrgBridge.FullDBName, String.Join(", ", fields), String.Join(", :", fields));
            daOrgBridge.InsertCommand.CommandText = queryInsert;
        }

        // ������������� Update-�������
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

        // ������ ����� ����������� ��� ��������� ��������
        private string[] NEEDED_FIELDS = new string[] {
            "ID", "ParentID", "SourceID", "Code", "INN20", "OKATOCode", "Name", "MainOKVED" };

        // ����������� ������ ��������������.
        // ��� �������� ������ �������� ������ ������ ���� (������� � ������� NEEDED_FIELDS)
        private void QueryOrgBridgeData()
        {
            // ������������� ������ �� ���� Code (���), ����� � ���������� �� ���� ����� �������������
            // ������ � ��� = 0 ����������, �.�. ��� ��� �������� �� ���������������
            string querySelect = string.Format(
                "SELECT {0} FROM {1} WHERE code <> 0 ORDER BY code ASC",
                String.Join(", ", NEEDED_FIELDS), clsOrgBridge.FullDBName);
            InitLocalDataAdapter(this.DB, ref daOrgBridge, querySelect);

            // �������������� SQL-������� � ������������ ������� �����
            InitInsertCommand(NEEDED_FIELDS);
            InitUpdateCommand(NEEDED_FIELDS);

            // ��������� �������
            dsOrgBridge = new DataSet();
            daOrgBridge.Fill(dsOrgBridge);
        }

        #endregion ������������� DataAdapter � ���������� DataSet

        // ��������� ��������������� ���� ��� ��������� ��������
        // ���������� ������ ������������� �� ��� � ���� Id ������������ ������ ��� ������ ������
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
                    // �� ��������� � �������� ������������ ����� ������ ������ �� ������
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
                    // ���� ����� ��� = ����01���, �� ������������� Id � ����� ��� ������������
                    // � ���������� �����, �.�. ����� ����� ��� ������ � ��� = ����50���
                    if (kppPart.EndsWith("01"))
                        cacheParentId[inn] = id;
                    // ���� ����� ��� = ����50���, �� ������ � ����� ��� � ����� ������������
                    // ������ Id ������������ ������ �� ����
                    if (kppPart.EndsWith("50"))
                    {
                        cacheParentId[inn] = id;
                        toChangeId = false;
                    }
                }
            }
            return cacheParentId;
        }

        // ���� � ������ �� ��� ���� �� ���� ������ � ������������� ���������
        // ���� ����� ������ �������, �� ���������� Id � ��������, ����� -1
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
                "����� ��������� �������� �������������� \"�����������.������������\"");

            // ������ ������ ��������������
            QueryOrgBridgeData();

            // ��������� ����

            // ��� ������� ��������������: ���� - ID, �������� - ������
            Dictionary<int, DataRow> cacheOrgBridgeRow = new Dictionary<int, DataRow>();
            FillRowsCache(ref cacheOrgBridgeRow, dsOrgBridge.Tables[0], "Id");
            // ��� �������, ��������������� �� ���: ���� - ���, �������� - ������ �������
            Dictionary<long, DataRow[]> cacheGroupByInn = new Dictionary<long, DataRow[]>();
            // ��� id-������ ������������ �������: ���� - ��� ������, �������� - Id ������������ ������ � ������
            Dictionary<long, int> cacheParentId = GetCacheGroupByInn(ref cacheGroupByInn);
            // ��� ��������: ���� - Id ����������� ������, �������� - Id ������������ ������
            Dictionary<int, int> hierarchy = new Dictionary<int, int>();

            try
            {
                foreach (KeyValuePair<long, int> item in cacheParentId)
                {
                    DataRow[] innGroup = cacheGroupByInn[item.Key];
                    // ���� � ������ �� ����� ����� ������, �� �������� ������������� �� ����
                    if (innGroup.Length <= 1)
                        continue;

                    DataRow parentRow = null;
                    // �������� �������� Id ������������ ������ ��� ������,
                    // � ������ ���� �������� ����� ���� �����������
                    int parentId = GetParentRowId(innGroup);
                    // ���� ���� ����� ������, �� ���� � � �������� ������������
                    if (parentId != -1)
                    {
                        parentRow = cacheOrgBridgeRow[parentId];
                    }
                    // ���� �� ��� ����� ������, �� ������� �����
                    else
                    {

                        parentRow = dsOrgBridge.Tables[0].NewRow();
                        parentId = GetGeneratorNextValue(clsOrgBridge);
                        dsOrgBridge.Tables[0].Rows.Add(parentRow);
                    }

                    // ��������� ���� ������������ ������
                    DataRow futureParentRow = cacheOrgBridgeRow[item.Value];
                    CopyRowToRow(futureParentRow, parentRow);
                    parentRow["ID"] = parentId;
                    parentRow["ParentId"] = DBNull.Value;

                    // ��������� ��� ������ ������ ������������ ������
                    foreach (DataRow childRow in innGroup)
                    {
                        int childRowId = Convert.ToInt32(childRow["Id"]);
                        if (childRowId != parentId)
                            hierarchy.Add(childRowId, parentId);
                    }
                }
                // ��������� ����� ����������� ������������ ������
                // ������ ��� ��������� ������ �� ��� � ����������� �������
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
                "���������� ��������� �������� �������������� \"�����������.������������\"");
        }
        */

        #endregion ��������� ��������

        private void FillOrgBridge()
        {
            // ��������� ������������� �� Dbf-�����
            if (!bridgePumpedFromDbf)
                PumpPayers();
            // ��������� ������������� �� �����������.���_�����������
            CopyFromPayers();
            // ������������� ��������
            // ���� ��������������, ������ ������� ������������� �������
            // SetOrgBridgeHierarchy();
        }

        #endregion ������������ �������������� "�����������.������������"

        #region �����������

        private string GetDateConstraint(bool isDraft)
        {
            string dateConstraint = string.Empty;
            // ���� ������� �� ����, ����� ����������� �� ���������� ���������
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
                // ������� ����, �������� ���� �� ������ ���������� ���
                List<string> constraint = new List<string>();
                if (isDraft)
                {
                    // ��� ������� ������ "��������� ���������" �������� ������ ����
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
                    // ��� ������� ������ "�������� � ������" ��������� ��� � ��� ����������� (RefFXTypes),
                    // �.�. � ����� ������� �������� ������ 3-� �����: ��������, ������ � ������ ������ �����
                    // � ������ �� ��� �������� ��������
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
            this.SetProgress(-1, -1, "������ ������ ��� �����������...", string.Empty, true);
            WriteToTrace("������ ������ ��� �����������...", TraceMessageKind.Information);

            // ������������� ���������
            // ������� ��� ��������� ������ ��� ����������� (� ������������� �� � �����)
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

            WriteToTrace("������ ������ ��� ����������� �������.", TraceMessageKind.Information);
        }

        private void DisintRow(DataRow sourceRow, DataRow disintRow, string[] fieldsForDisint, bool isDrafts)
        {
            DataRow row = null;
            if (!regionsForPumpCache.ContainsKey(currentOkatoCode))
                return;
            DataRow regionRow = regionsForPumpCache[currentOkatoCode]; 
            int terrType = Convert.ToInt32(regionRow["REFTERRTYPE"]);
            // � ������������ ���� ���������� ������ �� ������
            if (terrType == 0)
                return;
            // ������������ ��� �������� �����������
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
                            // ��� ���� ���������� ��, ��, �������� ����� -  ����� �� ����� ������ ������� �����������
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

                    // ���� ���������� ����� ������������ �������, �� ���������� �� � ����
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
            // ������� �������
            int recCount = 0;
            DataRow disintRowEx = null;

            disintCount = 0;
            totalRecsForSourceID = 0;

            string constr = string.Format("{0}.SOURCEID = {1} and {0}.{2} = 0",
                fctSourceTable.FullDBName, this.SourceID, disintFlagFieldName);
            if (disintDateConstraint != string.Empty)
                constr += string.Format(" and {0}", disintDateConstraint);

            // ������, ���� �� ������ ��� ����������� � ������� ��
            // ����� �������
            int totalRecs = Convert.ToInt32(this.DB.ExecQuery(string.Format(
                "select count(id) from {0} where {1}", fctSourceTable.FullDBName, constr), QueryResultTypes.Scalar));
            if (totalRecs == 0)
            {
                WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeCriticalError, "��� ������ ��� �����������.");
                return;
            }

            WriteToTrace(string.Format("������� ��� �����������: {0}.", totalRecs), TraceMessageKind.Information);

            // ������ �������� ������� ��
            int firstID = Convert.ToInt32(this.DB.ExecQuery(string.Format(
                "select min(ID) from {0} where {1}", fctSourceTable.FullDBName, constr), QueryResultTypes.Scalar));
            // ������� ����������� �� ��� �������
            int lastID = firstID + constMaxQueryRecordsForDisint - 1;

            // ������� ������������ �������
            InitFactDataSet(ref daDisintegratedData, ref dsDisintegratedData, fctDisintegratedData);

            do
            {
                // ����������� ������� ��� ������� ������ ������
                string restrictID = string.Format(
                    "{0}.ID >= {1} and {0}.ID <= {2} and {3}", fctSourceTable.FullDBName, firstID, lastID, constr);
                firstID = lastID + 1;
                lastID += constMaxQueryRecordsForDisint;

                QuerySourceData(restrictID, refKDFieldName);

                if (dsSourceData.Tables[0].Rows.Count == 0)
                    continue;

                // ���������� ���������� ������
                for (int i = 0; i < dsSourceData.Tables[0].Rows.Count; i++)
                {
                    recCount++;

                    DataRow sourceRow = dsSourceData.Tables[0].Rows[i];

                    this.SetProgress(totalRecs, recCount,
                        string.Format("��������� ������ (ID ��������� {0})...", this.SourceID),
                        string.Format("������ {0} �� {1}", recCount, totalRecs));

                    // �� ������ ����� ����� �����������, �� ��� ����� ������; ��, ��� � �����. ��������� ���� - ������ ����� ����� ���� 
                    // � ����� ������ (��� ������ ���������)
                    currentOkatoCode = Convert.ToString(sourceRow["OrgOkato"]);
                    string kd = Convert.ToString(sourceRow["KdCode"]);
                    string date = Convert.ToString(sourceRow[refDateFieldName]);
                    int year = Convert.ToInt32(date.Substring(0, 4));

                    totalRecsForSourceID++;

                    // ���� ������� �����������, ��������������� ��������: ��, O���� ������ ����� ��� � ������ ������, ���,
                    // ���� ������ ���������, �� ����� ������ ��, ��� ���� "����, � �������� ��������� ���������" 
                    // ������ ��� ����� ����� ����.
                    DataRow disintRow = FindDisintRule(disintRulesCache, year, kd);

                    if (disintRow == null)
                    {
                        // �� ������� �� ������ ����������� - ����� � �������� ������������� ������
                        WriteRecInMessagesDS(date, year, this.SourceID, kd);
                        continue;
                    }
                    else
                    {
                        // ���� ���������
                        disintRowEx = GetDisintExRow(disintRow, Convert.ToInt32(date), currentOkatoCode.PadLeft(5, '0'));
                        // ���� ����� ���������, �� ��� � ����������
                        if (disintRowEx != null)
                            disintRow = disintRowEx;
                    }

                    // ���������� ������
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
            WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeInformation, "������ ����������� ����");

            // ��� ������ �����������: ���� - ���, �������� - ������ ������ (���� - ��� ��, �������� - ������ �������)
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

            // ��������� ���������
            DisintData(new string[] { "ForPeriod" }, true);

            // �������� � ������
            this.disintDateConstraint = GetDateConstraint(false);
            this.fctSourceTable = fctRepayment;
            PrepareDisintedData(disintAll, " and RefFXTypes <> 1");
            DisintData(new string[] { "ForPeriod" }, false);

            UpdateProcessedData();
            WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeInformation, "���������� ����������� ����");
        }

        #endregion �����������

        protected override void ProcessDataSource()
        {
            // ������������ ������ �� ���� ��� � ������� ������ "������.���_�����������������_��������� ���������"
            SetPaymentSign(fctDrafts);
            // �����������
            if (toDisintData)
            {
                DisintData();
            }
            // ��������� �������� �����������
            if (toProcessOrgCls)
            {
                SetOrgHierarchy();
                UpdateProcessedData();
            }
            // ������������ �������������� �����������.������������
            if (toFillOrgBridgeCls)
            {
                FillOrgBridge();
            }
        }

        private string GetProcessComment()
        {
            string message = string.Empty;
            if (toDisintData)
                message += "����������� ���� �� ���������� ���������� �������";
            if (toProcessOrgCls)
            {
                if (message != string.Empty)
                    message += "; ";
                message += "�������������� �������������� '�����������.��� �����������' ";
            }
            if (toFillOrgBridgeCls)
            {
                if (message != string.Empty)
                    message += "; ";
                message += "������������ �������������� '�����������.������������' ";
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

        #endregion ��������� ������

        #region �������������

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

        #endregion �������������

    }

}
