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
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.IncomesDistributionPump
{
    /// <summary>
    /// ���_0012 ������������� ������� (����������) 
    /// ���������� �� ������������� � ������� �����, ���� ������, ����, ���������� � ������������.
    /// </summary>
    public class IncomesDistributionPumpModule : CorrectedPumpModuleBase
    {

        #region ����

        #region ��������������

        // �����������.��� (d.Org.UFKPayers)
        private IDbDataAdapter daPayers;
        private DataSet dsPayers;
        private IClassifier clsPayers;
        private Dictionary<string, int> payersCache = null;
        // ��.��� (d.KD.UFK)
        private IDbDataAdapter daKD;
        private DataSet dsKD;
        private IClassifier clsKD;
        private Dictionary<string, int> kdCache = null;
        // �����.��� (d.OKATO.UFK)
        private IDbDataAdapter daOKATO;
        private DataSet dsOKATO;
        private IClassifier clsOKATO;
        private Dictionary<string, int> okatoCache = null;

        #endregion ��������������

        #region �����

        // ������.���_������������� ������� ���������� (f.D.UFKAssgnInc)
        private IDbDataAdapter daUFKAssgnInc;
        private DataSet dsUFKAssgnInc;
        private IFactTable fctUFKAssgnInc;
        private string fctUFKAssgnIncFullDBName;

        #endregion �����

        // ��������� ���������
        private int year = -1;
        private int month = -1;
        private DataTable dtDatesForProcess = null;
        private string queryParams = string.Empty;
        // ������ ������� � ������
        private ExcelHelper excelHelper;
        private int rowsCount = 0;
        private object excelObj = null;

        #endregion ����


        #region ���������

        /// <summary>
        /// ���������� ������� ��� ��������� � ����
        /// </summary>
        private const int constMaxQueryRecords = 10000;

        /// <summary>
        /// ����������� ��� ������� ������ ������ �� ���������� ������
        /// </summary>
        private const string constOracleSelectFactDataByMonthConstraint =
            "((floor(mod(RefYearDayUNV, 10000) / 100) = {0}) and (floor(RefYearDayUNV / 10000) = {1}))";

        /// <summary>
        /// ����������� ��� ������� ������ ������ �� ���������� ������
        /// </summary>
        private const string constSQLServerSelectFactDataByMonthConstraint =
            "((floor((RefYearDayUNV % 10000) / 100) = {0}) and (floor(RefYearDayUNV / 10000) = {1}))";

        #endregion ���������


        #region ������� ������

        #region ������ � ����� � ������

        /// <summary>
        /// ������ ������ �� ����
        /// </summary>
        protected override void QueryData()
        {
            InitClsDataSet(ref daPayers, ref dsPayers, clsPayers);
            InitClsDataSet(ref daKD, ref dsKD, clsKD);
            InitClsDataSet(ref daOKATO, ref dsOKATO, clsOKATO);
            InitFactDataSet(ref daUFKAssgnInc, ref dsUFKAssgnInc, fctUFKAssgnInc);
            FillCaches();
        }

        /// <summary>
        /// ���������� �����
        /// </summary>
        private void FillCaches()
        {
            FillRowsCache(ref payersCache, dsPayers.Tables[0], new string[] { "INN", "NAME", "KPP" }, "|", "ID");
            FillRowsCache(ref kdCache, dsKD.Tables[0], "CODESTR", "ID");
            FillRowsCache(ref okatoCache, dsOKATO.Tables[0], "CODE", "ID");
        }

        /// <summary>
        /// ������ ��������� � ����
        /// </summary>
        protected override void UpdateData()
        {
            UpdateDataSet(daPayers, dsPayers, clsPayers);
            UpdateDataSet(daKD, dsKD, clsKD);
            UpdateDataSet(daOKATO, dsOKATO, clsOKATO);
            UpdateDataSet(daUFKAssgnInc, dsUFKAssgnInc, fctUFKAssgnInc);
        }

        /// <summary>
        /// ������������� �������� ��
        /// </summary>
        private const string D_ORG_UFK_PAYERS_GUID = "5d7f6e1d-c202-49b3-b6ad-d584616aded0";
        private const string D_KD_UFK_GUID = "b713e1df-5584-4e3d-a399-8828a2906971";
        private const string D_OKATO_UFK_GUID = "4ae52664-ca7c-4994-bc5e-ba982421540e";
        private const string F_D_UFK_ASSGN_INC_GUID = "6b90305f-6ee9-49ca-946f-1bf0815affba";
        protected override void InitDBObjects()
        {
            this.UsedClassifiers = new IClassifier[] {
                clsPayers = this.Scheme.Classifiers[D_ORG_UFK_PAYERS_GUID],
                clsKD = this.Scheme.Classifiers[D_KD_UFK_GUID],
                clsOKATO = this.Scheme.Classifiers[D_OKATO_UFK_GUID] };
            this.UsedFacts = new IFactTable[] { fctUFKAssgnInc = this.Scheme.FactTables[F_D_UFK_ASSGN_INC_GUID] };
            fctUFKAssgnIncFullDBName = fctUFKAssgnInc.FullDBName;
        }

        /// <summary>
        /// ������� ���������� ����������� �������� ����
        /// </summary>
        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsUFKAssgnInc);
            ClearDataSet(ref dsPayers);
            ClearDataSet(ref dsKD);
            ClearDataSet(ref dsOKATO);
        }

        #endregion ������ � ����� � ������

        #region ������ � �������

        // ���������� ���������� ����� � ��������� Excel-����� ������
        private int GetRowsCount(object sheet)
        {
            int emptyStrCount = 0;
            int curRow = 1;
            while (emptyStrCount < 10)
            {
                if (excelHelper.GetCell(sheet, curRow, 1).Value.Trim() == string.Empty)
                    emptyStrCount++;
                else
                    emptyStrCount = 0;
                curRow++;
            }
            return (curRow - 10);
        }

        /// <summary>
        /// ���������� ������������ ������� ������
        /// </summary>
        /// <param name="sheet"> ���� ������ </param>
        /// <param name="top"> �������� ������� ������ </param>
        /// <param name="bottom"> ������ ������� ������ </param>
        /// <returns> ������� �� �������� ������� </returns>
        private bool GetSheetMargins(object sheet, ref int top, ref int bottom)
        {
            for (int i = 1; i <= rowsCount; i++)
            {
                string value = excelHelper.GetCell(sheet, i, 1).Value.Trim();
                if (value.StartsWith("� �/�"))
                    top = i + 1;
                else if (value.ToUpper().StartsWith("�����"))
                {
                    bottom = i - 1;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// ���������� ���� ������
        /// </summary>
        /// <param name="sheet">�������� ������</param>
        /// <returns>����</returns>
        private int GetReportDate(object sheet)
        {
            string date = excelHelper.GetCell(sheet, 2, 1).Value;
            date = date.Remove(0, date.Length - 11);
            return CommonRoutines.ShortDateToNewDate(date);
        }

        /// <summary>
        /// ���������� ������ ������ � �������
        /// </summary>
        /// <param name="row">������ ������</param>
        /// <param name="date">���� ������</param>
        private void PumpXLSRow(object sheet, int curRow, int refDate)
        {
            try
            {
                // �����������.���: ��� + ��� + ������������
                string payersINN = excelHelper.GetCell(sheet, curRow, 2).Value.TrimStart('0');
                // ������ ��� ���������� 
                if (payersINN == string.Empty)
                    return;
                // KPP - �������������� ����
                string payersKPP = excelHelper.GetCell(sheet, curRow, 12).Value.TrimStart('0');
                if (payersKPP == string.Empty)
                    payersKPP = "0";
                // ������������ �����������
                string payersName = excelHelper.GetCell(sheet, curRow, 4).Value;
                if (payersName == string.Empty)
                    payersName = "����������� �����������";
                object[] mapping = new object[] { "INN", payersINN, "KPP", payersKPP, "NAME", payersName };
                // �������� ID �����������
                string cacheKey = payersINN + "|" + payersName + "|" + payersKPP ;
                int payersID = PumpCachedRow(payersCache, dsPayers.Tables[0], clsPayers, mapping, cacheKey, "ID");
                // �����.���
                cacheKey = excelHelper.GetCell(sheet, curRow, 3).Value;
                mapping = new object[] { "CODE", cacheKey };
                int okatoID = PumpCachedRow(okatoCache, dsOKATO.Tables[0], clsOKATO, mapping, cacheKey, "ID");
                // ��.���
                cacheKey = excelHelper.GetCell(sheet, curRow, 8).Value;
                mapping = new object[] { "CODESTR", cacheKey };
                int kdID = PumpCachedRow(kdCache, dsKD.Tables[0], clsKD, mapping, cacheKey, "ID");
                // ������.���_������������� ������� ����������
                // ��� ������� ������� � ����� ������� ��������� ����� "�� ������ �����", 14 - I, 0 - J
                double forPeriodReport = Convert.ToDouble(excelHelper.GetCell(sheet, curRow, 10).Value.Replace(" ", "").PadLeft(1, '0'));
                if (forPeriodReport != 0) 
                    PumpRow(dsUFKAssgnInc.Tables[0], new object[] { 
                            "FORPERIODREPORT", forPeriodReport, "FORPERIOD", 0, "RefYearDayUNV", refDate, 
                            "REFORGUFKPAYERS", payersID, "REFKD", kdID, "REFOKATO", okatoID, "REFBDGTLEVELS", 0 });
                forPeriodReport = Convert.ToDouble(excelHelper.GetCell(sheet, curRow, 9).Value.Replace(" ", "").PadLeft(1, '0'));
                if (forPeriodReport != 0)
                    PumpRow(dsUFKAssgnInc.Tables[0], new object[] { 
                            "FORPERIODREPORT", forPeriodReport, "FORPERIOD", 0, "RefYearDayUNV", refDate, 
                            "REFORGUFKPAYERS", payersID, "REFKD", kdID, "REFOKATO", okatoID, "REFBDGTLEVELS", 14 });
                if (dsUFKAssgnInc.Tables[0].Rows.Count >= constMaxQueryRecords)
                {
                    UpdateData();
                    ClearDataSet(daUFKAssgnInc, ref dsUFKAssgnInc);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("��� ��������� ������ {0} �������� ������ ({1})", curRow, ex.Message), ex);
            }
        }

        /// <summary>
        /// ���������� �������� ������
        /// </summary>
        /// <param name="sheet"> ���� ������ </param>
        private void PumpXLSSheet(object sheet, string fileName)
        {
            string sheetName = excelHelper.GetSheetName(sheet);
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, string.Format("��������� �������� {0} ������.", sheetName));
            // �������� ������������ ������� ������
            int top = 0;
            int bottom = 0;
            if (!GetSheetMargins(sheet, ref top, ref bottom))
                throw new Exception(string.Format("�� ������� ������ ������� �������� {0} ������.", sheetName));
            // ������.����
            int date = GetReportDate(sheet);
            // ��������� ���� �� ������������ ���������
            CheckDataSourceByDate(date, true);
            // �������� ������ �� ������� ����
            DeleteData(string.Format("RefYearDayUNV = {0}", date), string.Format("���� ������: {0}.", date));
            string dataSourcePath = GetShortSourcePathBySourceID(this.SourceID);
            // ��������� ��������� ������ ������
            for (int i = top; i <= bottom; i++)
            {
                SetProgress(bottom, i, string.Format("��������� ����� {0}\\{1}...", dataSourcePath, fileName),
                            string.Format("������ {0} �� {1}", i, bottom));
                PumpXLSRow(sheet, i, date);
            }
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, string.Format("�������� {0} ������ ������� ����������.", sheetName));
        }

        /// <summary>
        /// ���������� ���� ������
        /// </summary>
        /// <param name="file">����</param>
        private void PumpXLSFile(FileInfo file)
        {
            WriteToTrace("�������� ���������", TraceMessageKind.Information);
            object workbook = excelHelper.InitWorkBook(ref excelObj, file.FullName);
            try
            {
                int sheetCount = excelHelper.GetSheetCount(workbook);
                for (int i = 1; i <= sheetCount; i++)
                {
                    object sheet = excelHelper.GetSheet(workbook, i);
                    rowsCount = GetRowsCount(sheet);
                    // ��������� ����� ����������
                    if (rowsCount < 0)
                        continue;
                    PumpXLSSheet(sheet, file.Name);
                }

            }
            finally
            {
                excelHelper.CloseExcel(ref excelObj);
            }
        }

        #endregion ������ � �������

        #region ���������� ������ �������

        private void ProcessArchFiles(DirectoryInfo dir, string archMask, ArchivatorName archName)
        {
            FileInfo[] archFiles = dir.GetFiles(archMask, SearchOption.AllDirectories);
            foreach (FileInfo archFile in archFiles)
            {
                DirectoryInfo tempDir = CommonRoutines.ExtractArchiveFileToTempDir(archFile.FullName,
                    FilesExtractingOption.SingleDirectory, archName);
                try
                {
                    ProcessFilesTemplate(tempDir, "*.xls", new ProcessFileDelegate(PumpXLSFile), false);
                }
                finally
                {
                    CommonRoutines.DeleteDirectory(tempDir);
                }
            }
        }

        protected override void ProcessFiles(DirectoryInfo dir)
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStart, "����� ������������� Excel.");
            excelHelper = new ExcelHelper();
            try
            {
                ProcessFilesTemplate(dir, "*.xls", new ProcessFileDelegate(PumpXLSFile), false);
                // ���� ���� ������, ������������� � ������ ����� XLS
                ProcessArchFiles(dir, "*.arj", ArchivatorName.Arj);
                ProcessArchFiles(dir, "*.rar", ArchivatorName.Rar);
            }
            finally
            {
                if (excelHelper != null)
                    excelHelper.Close();
            }
        }

        /// <summary>
        /// ������� ������
        /// </summary>
        protected override void DirectPumpData()
        {
            PumpDataYTemplate();
        }

        #endregion ���������� ������ �������

        #endregion ������� ������


        #region ��������� ������

        #region ���������� ���� ���������

        /// <summary>
        /// ���������� ����������� �� ������� �� ������ �������� ���� ����
        /// </summary>
        /// <returns>�����������</returns>
        private string GetMonthConstraint()
        {
            switch (this.ServerDBMSName)
            {
                case DBMSName.SQLServer:
                    return constSQLServerSelectFactDataByMonthConstraint;
                default:
                    return constOracleSelectFactDataByMonthConstraint;
            }
        }

        /// <summary>
        /// �������� ����������� �������
        /// </summary>
        private void GetQueryParams()
        {
            // ����������� �� �� �� �������
            if (this.StagesQueue[PumpProcessStates.PumpData].IsExecuted)
                queryParams = string.Format("and PUMPID = {0}", this.PumpID);
            // ����������� �� ������
            if (month > 0)
                queryParams += "and " + string.Format(GetMonthConstraint(), month, this.DataSource.Year);
        }

        /// <summary>
        /// �������� ������ �� �������
        /// </summary>
        /// <returns>����� �������</returns>
        private string GetQuery()
        {
            GetQueryParams();
            return string.Format("select distinct RefYearDayUNV from {0} where (SOURCEID = {1} {2}) order by RefYearDayUNV asc",
                    fctUFKAssgnIncFullDBName, this.SourceID, queryParams);
        }

        /// <summary>
        /// � ����������� �� ���� ���������� �������� ������� �������
        /// </summary>
        /// <param name="terrType">��� ����������</param>
        /// <returns>������� �������</returns>
        private int GetBudgetLevel(int terrType)
        {
            switch (terrType)
            {
                case 4:
                    return 5;
                case 5:
                case 6:
                    return 6;
                case 7:
                    return 15;
                default:
                    return 14;
            }
        }

        /// <summary>
        /// �������� ����� � ������.��������� ��� �������
        /// </summary>
        private void PumpOKATO()
        {
            foreach (KeyValuePair<string, int> item in okatoCache)
                GetOkatoRow(Convert.ToInt32(item.Value));
        }

        /// <summary>
        /// ������������� ������: ������������� �������� ���� "�� ������" � �������������� ��� �������
        /// </summary>
        private void CorrectData()
        {
            // ������� ���������� ���� � ������������� � �� ������ �������� ���� "�� ������"
            for (int i = 0; i < dtDatesForProcess.Rows.Count; i++)
            {
                int currentDate = Convert.ToInt32(dtDatesForProcess.Rows[i][0]);
                // ����������� ������ ��� ��������� ���� � ������������� ���
                InitDataSet(ref daUFKAssgnInc, ref dsUFKAssgnInc, fctUFKAssgnInc, string.Format("RefYearDayUNV = {0} {1}", currentDate, queryParams));
                // ����� ���� ��������� ������� � ����� �����������, �������� ����� ���� ������ � ����� �� ���
                List<string> alreadyCorrectedList = new List<string>();
                try
                {
                    string msg = string.Format("��������� ���� \"�� ������\" ��� ���� {0} (�������� {1})...", currentDate, this.SourceID);
                    WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeInformation, msg);
                    int count = dsUFKAssgnInc.Tables[0].Rows.Count;
                    for (int j = 0; j < count; j++)
                    {
                        SetProgress(count, j + 1, msg, string.Format("������ {0} �� {1}", j + 1, count));
                        DataRow row = dsUFKAssgnInc.Tables[0].Rows[j];
                        int currentOkatoID = Convert.ToInt32(row["REFOKATO"]);
                        DataRow okatoRow = GetOkatoRow(currentOkatoID);
                        int terrType = GetIntCellValue(okatoRow, "REFTERRTYPE", -1);
                        // � ������������ �� ���������� � ����� ���� ���������� �������������� ������.��ʻ 
                        // �������������� ���� ������� �������. ������������� �������������� ������ ��� �����, � �������� ������� 14-����.������ ��
                        if (Convert.ToInt32(row["REFBDGTLEVELS"]) != 0)
                            row["REFBDGTLEVELS"] = GetBudgetLevel(terrType);
                        // ������������� ����� "�� ������", ��� �����: 
                        // ��� ������� 0 - �������� "������ �����" � ���� "������ �����" ����������� ����� �������
                        // ��������� ���� ������� - "������ �����"
                        double sum = GetDoubleCellValue(row, "FORPERIODREPORT", 0);
                        if (Convert.ToInt32(row["REFBDGTLEVELS"]) != 0)
                            row["FORPERIOD"] = sum;
                        else
                        {
                            string refORGUFKPAYERS; string refKD; string refOKATO;
                            refORGUFKPAYERS = row["REFORGUFKPAYERS"].ToString();
                            refKD = row["REFKD"].ToString();
                            refOKATO = row["REFOKATO"].ToString();
                            string sumParams = string.Format("{0}|{1}|{2}", refORGUFKPAYERS, refKD, refOKATO);
                            if (!alreadyCorrectedList.Contains(sumParams))
                            {
                                DataRow[] subBudgetLevels = dsUFKAssgnInc.Tables[0].Select(
                                    string.Format("REFORGUFKPAYERS = {0} and REFKD = {1} and REFOKATO = {2} and REFBDGTLEVELS <> 0",
                                    refORGUFKPAYERS, refKD, refOKATO));
                                foreach (DataRow subBudgetLevel in subBudgetLevels)
                                    sum -= Convert.ToDouble(subBudgetLevel["FORPERIODREPORT"]);
                                alreadyCorrectedList.Add(sumParams);
                            }
                            row["FORPERIOD"] = sum;
                        }
                    }
                    UpdateProcessedData();
                }
                finally
                {
                    ClearDataSet(ref dsUFKAssgnInc);
                    alreadyCorrectedList.Clear();
                }
            }
        }

        #endregion

        #region ���������� ������ ��������� (� ������� ������)

        /// <summary>
        /// ���� ��������� ������
        /// </summary>
        protected override void DirectProcessData()
        {
            GetPumpParams(ref year, ref month);
            ProcessDataSourcesTemplate(year, month, "���������� ���� \"�� ������\"");
        }

        /// <summary>
        /// ������� ������� ������ �� ����
        /// </summary>
        protected override void QueryDataForProcess()
        {
            InitClsDataSet(ref daOKATO, ref dsOKATO, clsOKATO);
            FillRowsCache(ref okatoCache, dsOKATO.Tables[0], "CODE", "ID");
            PrepareBadOkatoCodesCache();
            PrepareRegionsForSumDisint();
            PrepareOkatoForSumDisint(clsOKATO);
        }

        /// <summary>
        /// ������������� ������������ ������������ ���� ��� �������� ���������
        /// </summary>
        protected override void ProcessDataSource()
        {
            // ����������� ������ ��� ��� ���������
            dtDatesForProcess = this.DB.ExecQuery(GetQuery(), QueryResultTypes.DataTable) as DataTable;
            if (dtDatesForProcess == null || dtDatesForProcess.Rows.Count == 0)
                throw new Exception("��� ������ ��� ���������");
            // ��������� ������.��������� ��� ������� (������� ����� ������������, � ����� �� ��� ��� ����� ���� ����������)
            PumpOKATO();
            // ������������ ������
            CorrectData();
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
            ClearDataSet(ref dsOKATO);
        }

        /// <summary>
        /// ��������, ����������� ����� ��������� ������
        /// </summary>
        protected override void AfterProcessDataAction()
        {
            WriteBadOkatoCodesCacheToBD();
        }

        #endregion

        #endregion ��������� ������

    }
}
