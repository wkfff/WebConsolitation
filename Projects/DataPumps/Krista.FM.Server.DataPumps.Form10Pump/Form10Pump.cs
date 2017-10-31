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


namespace Krista.FM.Server.DataPumps.Form10Pump
{
    /// <summary>
    /// ���_0005_����������� � ������_����������� �������
    /// </summary>
    public class Form10PumpModule : TextRepPumpModuleBase
    {
        #region ��������

        private delegate void PumpFactRowDelegate(DataRow sourceRow, int reportDate, int archiveDate);

        #endregion ��������


        #region ����

        // ��
        private IDbDataAdapter daKD;
        private DataSet dsKD;
        // OKATO
        private IDbDataAdapter daOKATO;
        private DataSet dsOKATO;
        // ���
        private IDbDataAdapter daKIF;
        private DataSet dsKIF;
        // �������������
        private IDbDataAdapter daKVSR;
        private DataSet dsKVSR;
        // �����������
        private IDbDataAdapter daOrganizations;
        private DataSet dsOrganizations;
        // ����� ��� �����������
        private IDbDataAdapter daUFK5IncomesDirty;
        private DataSet dsUFK5IncomesDirty;
        // ����� � ������������
        private IDbDataAdapter daUFK5Incomes;
        private DataSet dsUFK5Incomes;

        private IClassifier clsKD;
        private IClassifier clsOKATO;
        private IClassifier clsKIF;
        private IClassifier clsKVSR;
        private IClassifier clsOrganizations;

        private IFactTable fctUFK5IncomesDirty;
        private IFactTable fctUFK5Incomes;

        private int nullKD;
        private int nullKIF;

        // ��� ���������������
        private Dictionary<string, int> kdMapping = null; //new Dictionary<string, int>(1000);
        private Dictionary<string, int> kifMapping = null;//new Dictionary<string, int>(1000);
        private Dictionary<string, int> okatoMapping = null;//new Dictionary<string, int>(1000);
        private Dictionary<string, int> kvsrMapping = null;//new Dictionary<string, int>(1000);
        private Dictionary<string, DataRow> orgMapping = null;//new Dictionary<string, DataRow>(1000);

        private bool dataIsDeleted = false;
        private bool disintAll = false;

        #endregion ����


        #region ���������

        // ���������� ������� ��� ��������� � ����
        private const int constMaxQueryRecords = 10000;

        #endregion ���������


        #region �������������

        /// <summary>
        /// �����������
        /// </summary>
        public Form10PumpModule()
            : base()
        {

        }

        #endregion �������������


        #region ������� ������

        /// <summary>
        /// ���������� ����� ��������� �������
        /// </summary>
        /// <param name="xmlSettingsFile">���� � ����������� ��� ��������� ��������� �������</param>
        /// <param name="dir">��������� ������� � ������� ��� �������</param>
        /// <param name="archiveName">��� ������ � �������</param>
        /// <param name="formNo">����� ������</param>
        /// <param name="pumpFactRow">������� ������� ������ ���������� ������</param>
        private void PumpForm(string xmlSettingsFile, string dir, string archiveName, int formNo, PumpFactRowDelegate pumpFactRow)
        {
            int totalRecs;
            int rowsCount = 0;
            int skippedReports = 0;
            int skippedRows = 0;
            int processedReports = 0;
            int date = 0;
            string err = string.Empty;
            string processedFiles = "<��� ������>";

            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, string.Format("����� ������� ������ ��������� ������� ����� {0}.", formNo));

            try
            {
                try
                {
                    this.CallTXTSorcerer(xmlSettingsFile, dir);
                }
                catch (Exception ex)
                {
                    WriteEventIntoDataPumpProtocol(
                        DataPumpEventKind.dpeWarning, string.Format("������� ������ ��������� ������� ����� {0} ���������", formNo), ex);
                    return;
                }

                totalRecs = GetTotalRecs();

                int archiveDate = this.DataSource.Year * 10000 + Convert.ToInt32(archiveName.Substring(5, 2)) * 100 +
                    Convert.ToInt32(archiveName.Substring(3, 2));

                // ���������� ���������� ������
                // ������ ������� �������� - ���������, �� �� �����
                for (int i = 1; i < this.ResultDataSet.Tables.Count; i++)
                {
                    DataTable dt = this.ResultDataSet.Tables[i];
                    if (dt.Rows.Count == 0) continue;

                    int fileIndex = Convert.ToInt32(dt.Rows[0][this.FileIndexFieldName]);
                    processedFiles = GetStringCellValue(this.ResultDataSet.Tables[0].Rows[fileIndex], "FILES", "<��� ������>");

                    // ���� �������
                    string str = this.FixedParameters[fileIndex]["ReportDate"].Value;

                    if (str != string.Empty)
                    {
                        date = Convert.ToInt32(str);
                    }

                    if (date > 0 && date / 10000 != this.DataSource.Year)
                    {
                        skippedReports++;
                        skippedRows += this.ResultDataSet.Tables[i].Rows.Count;
                        continue;
                    }

                    if (processedFiles.ToUpper().StartsWith("KZ"))
                    {
                        date = Convert.ToInt32(string.Format(
                            "{0}{1:00}{2:00}",
                            this.DataSource.Year, CommonRoutines.Numeration36To10(processedFiles[4]),
                            CommonRoutines.Numeration36To10(processedFiles[5])));
                    }

                    // ������� ����� ���������� ������
                    if (!dataIsDeleted)
                    {
                        DeleteData(string.Format(
                            "(DATAKIND = 0 and RefYearDayUNV = {0}) or (DATAKIND = 1 and RefYearDayUNV = {1})",
                            date, archiveDate), string.Format("���� ������: {0}, ���� ������: {1}.", date, archiveDate));
                        dataIsDeleted = true;
                    }

                    for (int j = 0; j < dt.Rows.Count; j++)
                    {
                        pumpFactRow.Invoke(dt.Rows[j], date, archiveDate);

                        if (dsUFK5IncomesDirty.Tables[0].Rows.Count >= constMaxQueryRecords)
                        {
                            UpdateData();
                            ClearDataSet(daUFK5IncomesDirty, ref dsUFK5IncomesDirty);
                        }

                        rowsCount++;
                        SetProgress(totalRecs, rowsCount, string.Format(
                            "��������� ������ ����� {0} �� {1}...", processedFiles, archiveName),
                            string.Format("������ {0} �� {1}", rowsCount, totalRecs));
                    }

                    processedReports++;
                }

                if (skippedReports == this.ResultDataSet.Tables.Count - 1)
                {
                    throw new Exception("��-�� �������������� ��� �� ������� �� ���� �����");
                }

                WriteEventIntoDataPumpProtocol(
                    DataPumpEventKind.dpeInformation, string.Format(
                        "������� ������ ��������� ������� ����� {0} ���������. ���������� �������: {1} ({2} �����), " +
                        "�� ��� ��������� ��-�� �������������� ���� ���������: {3} ������� ({4} �����).",
                        formNo, processedReports, rowsCount, skippedReports, skippedRows));
            }
            catch (Exception ex)
            {
                WriteEventIntoDataPumpProtocol(
                    DataPumpEventKind.dpeError, string.Format(
                        "������� ������ ��������� ������� ����� {0} ��������� � ��������: {1}. \n" +
                        "�� ������ ������������� ������ ���������� ��������� ����������. " +
                        "���������� �������: {2} ({3} �����), " +
                        "�� ��� ��������� ��-�� �������������� ���� ���������: {4} ������� ({5} �����). " +
                        "������ �������� ��� ��������� ������ {6}.",
                        formNo, ex.Message, processedReports, rowsCount, skippedReports, skippedRows, processedFiles));
                throw;
            }
        }

        /// <summary>
        /// ��������� �������������� �� ������ ������� ��������� �����
        /// </summary>
        /// <param name="sourceRow">������ ������� ��������� �����</param>
        private object[] FormGeneralClsByRow(DataRow sourceRow, ref string kd)
        {
            object[] result = new object[10];

            result[0] = "REFOKATO";
            result[2] = "REFKD";
            result[4] = "REFKIF";
            result[6] = "REFKVSR";
            result[8] = "REFORG";

            // ����� ������� �� ����� 7
            string okato = Convert.ToString(sourceRow["OKATO"]).TrimStart('0').PadLeft(1, '0');
            result[1] = PumpCachedRow(okatoMapping, dsOKATO.Tables[0], clsOKATO, okato, "CODE", null);

            // ��� ������� �� ����� 8, ������ ���� ��������� ���� ���� ����� 0
            kd = Convert.ToString(sourceRow["KD"]);
            if (kd.Length > 4 && kd[3] == '0')
            {
                result[3] = nullKD;
                result[5] = PumpCachedRow(kifMapping, dsKIF.Tables[0], clsKIF,
                    kd, new object[] { "CODESTR", kd });
            }
            // �� ������� �� ����� 8, ������ ���� ��������� ������ ���� �� ����� 0
            else
            {
                result[3] = PumpCachedRow(kdMapping, dsKD.Tables[0], clsKD,
                    kd, new object[] { "CODESTR", kd });
                result[5] = nullKIF;
            }

            string kvsrINN = Convert.ToString(sourceRow["ARRADMINN"]);
            result[7] = PumpCachedRow(kvsrMapping, dsKVSR.Tables[0], clsKVSR,
                kvsrINN, new object[] { "CODESTR", kvsrINN, "KPP", sourceRow["ARRADMKPP"] });

            string orgINN = Convert.ToString(sourceRow["PAYERINN"]).TrimStart('0').PadLeft(1, '0');
            if (!sourceRow.Table.Columns.Contains("PAYERNAME"))
            {
                // ��� ������� ����� 5 ������������ �� �����������
                result[9] = PumpCachedRow(orgMapping, dsOrganizations.Tables[0], clsOrganizations, orgINN,
                    new object[] { "CODE", orgINN, "KPP", sourceRow["PAYERKPP"], "NAME", "����������� �����������" });
            }
            else
            {
                // ��� ������� ����� 10 ��������� ������������ ��������� �����������
                if (orgMapping.ContainsKey(orgINN))
                {
                    orgMapping[orgINN]["NAME"] = sourceRow["PAYERNAME"];
                    result[9] = orgMapping[orgINN]["ID"];
                }
                else
                {
                    result[9] = PumpCachedRow(orgMapping, dsOrganizations.Tables[0], clsOrganizations, orgINN,
                        new object[] { "CODE", orgINN, "KPP", sourceRow["PAYERKPP"], "NAME", sourceRow["PAYERNAME"] });
                }
            }

            return result;
        }

        /// <summary>
        /// ���������� ������ ������ ����� 5
        /// </summary>
        /// <param name="sourceRow">������ ������</param>
        /// <param name="reportDate">���� ������</param>
        /// <param name="archiveDate">���� ��������� ����� (�������)</param>
        private void PumpFactRow5(DataRow sourceRow, int reportDate, int archiveDate)
        {
            string kd = string.Empty;
            object[] generalClsRefs = FormGeneralClsByRow(sourceRow, ref kd);

            if (Convert.ToDouble(sourceRow["CREDIT"]) != 0)
            {
                PumpRow(dsUFK5IncomesDirty.Tables[0], 
                    (object[])CommonRoutines.ConcatArrays(generalClsRefs, new object[] {
                        "REFISDISINT", 0, "RefYearDayUNV", archiveDate, "CREDIT", sourceRow["CREDIT"],
                        "DEBIT", 0, "OUTBANK", 0, "DATAKIND", 1 }));
            }

            if (Convert.ToString(sourceRow["OKATO"]).Trim('0').PadLeft(1, '0') != "0" &&
                Convert.ToDouble(sourceRow["DEBIT"]) != 0)
            {
                PumpRow(dsUFK5IncomesDirty.Tables[0], 
                    (object[])CommonRoutines.ConcatArrays(generalClsRefs, new object[] {
                        "REFISDISINT", 0, "RefYearDayUNV", reportDate, "CREDIT", 0, 
                        "DEBIT", sourceRow["DEBIT"], "OUTBANK", 0, "DATAKIND", 0 }));
            }
        }

        /// <summary>
        /// ���������� ������ ������ ����� 10
        /// </summary>
        /// <param name="sourceRow">������ ������</param>
        /// <param name="reportDate">���� ������</param>
        /// <param name="archiveDate">���� ��������� ����� (�������)</param>
        private void PumpFactRow10(DataRow sourceRow, int reportDate, int archiveDate)
        {
            if (Convert.ToDouble(sourceRow["TOTAL"]) != 0)
            {
                string kd = string.Empty;
                object[] generalClsRefs = FormGeneralClsByRow(sourceRow, ref kd);

                DataRow disintRow = FindDisintRule(disintRulesCache, this.DataSource.Year, kd);
                if (GetDoubleCellValue(disintRow, "CONS_PERCENT", 0) == 0)
                {
                    return;
                }

                PumpRow(dsUFK5IncomesDirty.Tables[0], 
                    (object[])CommonRoutines.ConcatArrays(generalClsRefs, new object[] {
                    "REFISDISINT", 0, "RefYearDayUNV", archiveDate, "CREDIT", 0, "DEBIT", 0,
                    "OUTBANK", sourceRow["TOTAL"], "DATAKIND", 1 }));
            }
        }

        /// <summary>
        /// ���������� ��������� �����
        /// </summary>
        /// <param name="sourceDir">������� � ������� ��� �������</param>
        protected override void ProcessFiles(DirectoryInfo dir)
        {
            FileInfo[] files = dir.GetFiles("ok_*.arj", SearchOption.AllDirectories);
            if (files.GetLength(0) == 0)
            {
                throw new Exception("����� ��� ������� �� ����������.");
            }

            string tmpPath = string.Empty;

            for (int i = 0; i < files.GetLength(0); i++)
            {
                try
                {
                    WriteEventIntoDataPumpProtocol(
                        DataPumpEventKind.dpeStartFilePumping, string.Format("����� ��������� ����� {0}.", files[i].FullName));

                    tmpPath = CommonRoutines.ExtractArchiveFile(files[i].FullName, dir.FullName, ArchivatorName.Arj,
                        FilesExtractingOption.SeparateSubDirs);

                    dataIsDeleted = false;

                    PumpForm(xmlSettingsForm5, tmpPath, files[i].Name, 5, new PumpFactRowDelegate(PumpFactRow5));

                    PumpForm(xmlSettingsForm10, tmpPath, files[i].Name, 10, new PumpFactRowDelegate(PumpFactRow10));

                    WriteEventIntoDataPumpProtocol(
                        DataPumpEventKind.dpeSuccessfullFinishFilePump, string.Format("��������� ����� {0} ������� ���������.", files[i].FullName));
                }
                catch (ThreadAbortException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeFinishFilePumpWithError,
                        string.Format("��������� ����� {0} ��������� � ��������", files[i].FullName), ex);
                    throw;
                }
                finally
                {
                    CommonRoutines.DeleteExtractedDirectories(dir);
                }
            }
        }

        /// <summary>
        /// �������������� ������ ��������������� "����������� ������"
        /// </summary>
        private void InitNullClsValues()
        {
            nullKD = clsKD.UpdateFixedRows(this.DB, this.SourceID);
            nullKIF = clsKIF.UpdateFixedRows(this.DB, this.SourceID);
        }

        /// <summary>
        /// ��������� ��� ���������������
        /// </summary>
        private void FillCache()
        {
            FillRowsCache(ref kdMapping, dsKD.Tables[0], "CODESTR");
            FillRowsCache(ref kifMapping, dsKIF.Tables[0], "CODESTR");
            FillRowsCache(ref kvsrMapping, dsKVSR.Tables[0], "CODESTR");
            FillRowsCache(ref okatoMapping, dsOKATO.Tables[0], "CODE");
            FillRowsCache(ref orgMapping, dsOrganizations.Tables[0], new string[] { "CODE" });
        }

        /// <summary>
        /// ������ ������ �� ����
        /// </summary>
        protected override void QueryData()
        {
            InitClsDataSet(ref daKD, ref dsKD, clsKD, false, string.Empty);
            InitClsDataSet(ref daKIF, ref dsKIF, clsKIF, false, string.Empty);
            InitClsDataSet(ref daKVSR, ref dsKVSR, clsKVSR, false, string.Empty);
            InitClsDataSet(ref daOKATO, ref dsOKATO, clsOKATO, false, string.Empty);
            InitClsDataSet(ref daOrganizations, ref dsOrganizations, clsOrganizations, false, string.Empty);

            InitFactDataSet(ref daUFK5Incomes, ref dsUFK5Incomes, fctUFK5Incomes);
            InitFactDataSet(ref daUFK5IncomesDirty, ref dsUFK5IncomesDirty, fctUFK5IncomesDirty);

            InitNullClsValues();
            FillCache();
        }

        /// <summary>
        /// ������ ��������� � ����
        /// </summary>
        protected override void UpdateData()
        {
            UpdateDataSet(daKD, dsKD, clsKD);
            UpdateDataSet(daKIF, dsKIF, clsKIF);
            UpdateDataSet(daKVSR, dsKVSR, clsKVSR);
            UpdateDataSet(daOKATO, dsOKATO, clsOKATO);
            UpdateDataSet(daOrganizations, dsOrganizations, clsOrganizations);

            UpdateDataSet(daUFK5Incomes, dsUFK5Incomes, fctUFK5Incomes);
            UpdateDataSet(daUFK5IncomesDirty, dsUFK5IncomesDirty, fctUFK5IncomesDirty);
        }

        private const string D_KD_UFK_GUID = "b713e1df-5584-4e3d-a399-8828a2906971";
        private const string D_KIF_UFK_GUID = "73b83ed3-fa26-4d05-8e8e-30dbe226a801";
        private const string D_KVSR_UFK5_GUID = "3268c7b3-0c4f-4a99-acf1-598a474cc396";
        private const string D_OKATO_UFK_GUID = "4ae52664-ca7c-4994-bc5e-ba982421540e";
        private const string D_ORGANIZATIONS_UFK5_GUID = "c2adfc14-4829-4221-999e-b07c263c44d5";
        private const string F_D_UFK5_INCOMES_GUID = "dd513e2e-13d8-4422-9e8d-00bef27181b6";
        private const string F_D_UFK5_INCOMES_DIRTY_GUID = "429c0763-14b9-4b47-8752-9f5b4b9df232";
        protected override void InitDBObjects()
        {
            this.UsedClassifiers = new IClassifier[] {
                clsKD = this.Scheme.Classifiers[D_KD_UFK_GUID],
                clsKIF = this.Scheme.Classifiers[D_KIF_UFK_GUID],
                clsKVSR = this.Scheme.Classifiers[D_KVSR_UFK5_GUID],
                clsOKATO = this.Scheme.Classifiers[D_OKATO_UFK_GUID],
                clsOrganizations = this.Scheme.Classifiers[D_ORGANIZATIONS_UFK5_GUID] };

            this.UsedFacts = new IFactTable[] {
                fctUFK5Incomes = this.Scheme.FactTables[F_D_UFK5_INCOMES_GUID],
                fctUFK5IncomesDirty = this.Scheme.FactTables[F_D_UFK5_INCOMES_DIRTY_GUID] };
        }

        /// <summary>
        /// ������� ���������� ����������� �������� ����
        /// </summary>
        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsKD);
            ClearDataSet(ref dsKIF);
            ClearDataSet(ref dsKVSR);
            ClearDataSet(ref dsOKATO);
            ClearDataSet(ref dsOrganizations);
            ClearDataSet(ref dsUFK5Incomes);
            ClearDataSet(ref dsUFK5IncomesDirty);
        }

        /// <summary>
        /// ������� ������
        /// </summary>
        protected override void DirectPumpData()
        {
            FillDisintRulesCache();

            PumpDataYTemplate();
        }

        #endregion ������� ������


        #region ��������� ������

        /// <summary>
        /// ������� ������� ������ �� ����
        /// </summary>
        protected override void QueryDataForProcess()
        {

        }

        /// <summary>
        /// ������������� ������������ ������������ ���� ��� �������� ���������
        /// </summary>
        protected override void ProcessDataSource()
        {
            CheckDisintRulesCache();
            PrepareMessagesDS();
            PrepareBadOkatoCodesCache();
            PrepareRegionsForSumDisint();

            DisintegrateData(fctUFK5IncomesDirty, fctUFK5Incomes, clsKD, clsOKATO, new string[] { "DEBIT", "CREDIT", "OUTBANK" },
                "RefYearDayUNV", "REFKD", "REFOKATO", disintAll);
        }

        /// <summary>
        /// ������� ���������� ���������� ������ � ����
        /// </summary>
        protected override void UpdateProcessedData()
        {
            UpdateOkatoData();
            UpdateData();
        }

        /// <summary>
        /// ������� ���������� ����������� �������� �������
        /// </summary>
        protected override void ProcessFinalizing()
        {
            PumpFinalizing();
        }

        /// <summary>
        /// ��������, ����������� ����� ��������� ������
        /// </summary>
        protected override void AfterProcessDataAction()
        {
            UpdateMessagesDS();
            WriteBadOkatoCodesCacheToBD();
        }

        /// <summary>
        /// ���� ��������� ������
        /// </summary>
        protected override void DirectProcessData()
        {
            // ��������� ���� ������������ ������������ ����
            FillDisintRulesCache();

            int year = -1;
            int month = -1;

            if (!this.StagesQueue[PumpProcessStates.PumpData].IsExecuted)
            {
                GetDisintParams(ref year, ref month, ref disintAll);
            }

            ProcessDataSourcesTemplate(year, month, "����������� ���� �� ���������� ���������� �������");
        }

        #endregion ��������� ������
    }
}