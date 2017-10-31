using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.Form5NIOPump
{

    // ��� - 0004 - ����� 5-���
    public class Form5NIOPumpModule : TextRepPumpModuleBase
    {

        #region ����

        #region ��������������

        // ����������.��� 5 ��� (d_Marks_FNS5NIO)
        private IDbDataAdapter daMarks;
        private DataSet dsMarks;
        private IClassifier clsMarks;
        private Dictionary<string, int> cacheMarks = null;
        private int nullMarks;
        // ������.��� (d_Regions_FNS)
        private IDbDataAdapter daRegions;
        private DataSet dsRegions;
        private IClassifier clsRegions;
        private Dictionary<string, DataRow> cacheRegions = null;
        private Dictionary<string, string> cacheRegionsNames = null;
        private Dictionary<string, DataRow> cacheRegionsFirstRow = null;
        private int nullRegions;
        // �������.���� (d_Units_OKEI)
        private IDbDataAdapter daUnits;
        private DataSet dsUnits;
        private IClassifier clsUnits;
        private Dictionary<string, int> cacheUnits = null;
        private int nullUnits = -1;

        #endregion ��������������

        #region �����

        // ����.���_5 ���_������� (f_D_FNS5MNTotal)
        private IDbDataAdapter daFNS5NIOTotal;
        private DataSet dsFNS5NIOTotal;
        private IFactTable fctFNS5NIOTotal;
        // ����.���_5 ���_������ (f_D_FNS5MNRegions)
        private IDbDataAdapter daFNS5NIORegions;
        private DataSet dsFNS5NIORegions;
        private IFactTable fctFNS5NIORegions;

        #endregion �����

        private ReportType reportType;
        // ��������� ���������
        private int year = -1;
        private int month = -1;
        // �������� �����
        private decimal[] totalSums;
        // ����� ������ � ������ �������� (��� ������� �� �������)
        private int regionsRow;
        private decimal sumMultiplier = 1;

        private bool noSvodReports = false;
        private bool isTyvaRegion2008 = false;
        private bool isTyvaRegion2010 = false;

        #endregion ����

        #region ���������, ������������

        // ��� ������
        private enum ReportType
        {
            Svod,
            Str,
            Region
        }

        #endregion ���������, ������������

        #region ���������

        // ���������� ������� ��� ��������� � ����
        private const int constMaxQueryRecords = 10000;

        #endregion ���������

        #region �������������

        /// <summary>
        /// �����������
        /// </summary>
        public Form5NIOPumpModule()
            : base()
        {

        }

        #endregion �������������

        #region ������� ������

        #region ������ � ����� � �����

        // ��� ������� � ������������������� ������
        private void FillRegionsCache(string[] keyFields)
        {
            if (cacheRegions != null)
                cacheRegions.Clear();
            cacheRegions = new Dictionary<string, DataRow>(dsRegions.Tables[0].Rows.Count);
            int count = dsRegions.Tables[0].Rows.Count;
            for (int i = 0; i < count; i++)
            {
                DataRow row = dsRegions.Tables[0].Rows[i];
                if (row.RowState == DataRowState.Deleted)
                    continue;
                string key = GetComplexCacheKey(row, keyFields, "|").ToUpper();
                if (!cacheRegions.ContainsKey(key))
                    cacheRegions.Add(key, row);
            }
        }

        private void FillCaches()
        {
            FillRowsCache(ref cacheMarks, dsMarks.Tables[0], "CODE", "ID");
            FillRowsCache(ref cacheUnits, dsUnits.Tables[0], "NAME", "ID");
            FillRegionsCache(new string[] { "CODE", "NAME" });
        }

        /// <summary>
        /// ������ ������ �� ����
        /// </summary>
        protected override void QueryData()
        {
            InitDataSet(ref daUnits, ref dsUnits, clsUnits, true, string.Empty, string.Empty);
            InitClsDataSet(ref daRegions, ref dsRegions, clsRegions, false, string.Empty);
            InitClsDataSet(ref daMarks, ref dsMarks, clsMarks, false, string.Empty);
            InitFactDataSet(ref daFNS5NIORegions, ref dsFNS5NIORegions, fctFNS5NIORegions);
            InitFactDataSet(ref daFNS5NIOTotal, ref dsFNS5NIOTotal, fctFNS5NIOTotal);
            InitNullClsValues();
            FillCaches();
        }

        /// <summary>
        /// ������ ��������� � ����
        /// </summary>
        protected override void UpdateData()
        {
            UpdateDataSet(daRegions, dsRegions, clsRegions);
            UpdateDataSet(daMarks, dsMarks, clsMarks);
            UpdateDataSet(daFNS5NIORegions, dsFNS5NIORegions, fctFNS5NIORegions);
            UpdateDataSet(daFNS5NIOTotal, dsFNS5NIOTotal, fctFNS5NIOTotal);
        }

        /// <summary>
        /// �������������� ������ ��������������� "����������� ������"
        /// </summary>
        private void InitNullClsValues()
        {
            nullMarks = clsMarks.UpdateFixedRows(this.DB, this.SourceID);
            nullRegions = clsRegions.UpdateFixedRows(this.DB, this.SourceID);
        }

        /// <summary>
        /// ������������� �������� ��
        /// </summary>
        private const string D_REGIONS_FNS_GUID = "cf3202f9-e897-43ce-a158-5c617bedff55";
        private const string D_MARKS_FNS5NIO_GUID = "0cd8d09e-27a9-43e5-94ef-bb47ffe260a5";
        private const string D_UNITS_OKEI_GUID = "7ef0edfd-9461-4333-8420-ccb102051826";
        private const string F_F_FNS5NIO_REGIONS_GUID = "240c071f-4685-4c9d-8e41-ca9df965cd19";
        private const string F_F_FNS5NIO_TOTAL_GUID = "2c8a13bc-718a-4526-93ec-54168c7fa340";
        protected override void InitDBObjects()
        {
            clsUnits = this.Scheme.Classifiers[D_UNITS_OKEI_GUID];
            this.UsedClassifiers = new IClassifier[] {
                clsRegions = this.Scheme.Classifiers[D_REGIONS_FNS_GUID],
                clsMarks = this.Scheme.Classifiers[D_MARKS_FNS5NIO_GUID] };
            this.UsedFacts = new IFactTable[] {
                fctFNS5NIORegions = this.Scheme.FactTables[F_F_FNS5NIO_REGIONS_GUID],
                fctFNS5NIOTotal = this.Scheme.FactTables[F_F_FNS5NIO_TOTAL_GUID] };
        }

        /// <summary>
        /// ������� ���������� ����������� �������� ����
        /// </summary>
        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsRegions);
            ClearDataSet(ref dsMarks);
            ClearDataSet(ref dsUnits);
            ClearDataSet(ref dsFNS5NIORegions);
            ClearDataSet(ref dsFNS5NIOTotal);
        }

        #endregion ������ � ����� � �����

        #region ����� ������� �������

        // ��������� �������� �����
        private void SetNullTotalSum()
        {
            int sumsCount = totalSums.GetLength(0);
            for (int i = 0; i < sumsCount; i++)
            {
                totalSums[i] = 0;
            }
        }

        // �������� ����������� �����
        private void CheckTotalSum(decimal totalSum, decimal controlSum, string comment)
        {
            if (totalSum != controlSum)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, string.Format(
                    "����������� ����� {0:F} �� �������� � �������� {1:F} {2}",
                    controlSum, totalSum, comment));
            }
        }

        private decimal CleanFactValue(string factValue)
        {
            factValue = factValue.Trim().ToUpper().Trim('X').Trim('�').PadLeft(1, '0');
            return Convert.ToDecimal(factValue);
        }

        private void SetSumMultiplier(int marksCode)
        {
            if (this.DataSource.Year >= 2010)
            {
                if (marksCode == 1010)
                    sumMultiplier = 1;
                else
                    sumMultiplier = 1000;
            }
            else
            {
                sumMultiplier = 1000;
            }
        }

        private int GetReportDate()
        {
            // �������� �� ���������� ���������
            return this.DataSource.Year * 10000 + this.DataSource.Month * 100;
        }

        private void CheckMarks()
        {
            if ((reportType != ReportType.Svod) && (cacheMarks.Count == 0) && !noSvodReports)
                throw new Exception("�� �������� ������������� '����������.��� 5 ���' - ��������� ������� ������");
        }

        private void SetFlags()
        {
            isTyvaRegion2008 =
                (this.Region == RegionName.Tyva) && (reportType == ReportType.Region) && (this.DataSource.Year <= 2008);
            isTyvaRegion2010 =
                (this.Region == RegionName.Tyva) && (reportType == ReportType.Region) && (this.DataSource.Year >= 2010);
        }

        private void ProcessAllFiles(DirectoryInfo dir)
        {
            SetFlags();
            CheckMarks();
            if ((this.Region == RegionName.Kostroma) && (this.DataSource.Year < 2007))
                PumpReports(dir);
            ProcessFilesTemplate(dir, "*.xls", new ProcessFileDelegate(PumpXlsFile), false);
            ProcessFilesTemplate(dir, "*.rar", new ProcessFileDelegate(PumpRarFile), false);
        }

        private void PumpFiles(DirectoryInfo dir)
        {
            reportType = ReportType.Svod;
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStartFilePumping, "����� ������� ������ ������� �������.");
            ProcessAllFiles(dir.GetDirectories(constSvodDirName)[0]);
            reportType = ReportType.Region;
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStartFilePumping, "����� ������� ������ ������� � ������� �������.");
            ProcessAllFiles(dir.GetDirectories(constRegDirName)[0]);
            reportType = ReportType.Str;
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStartFilePumping, "����� ������� ������ ������� � ������� �����.");
            ProcessAllFiles(dir.GetDirectories(constStrDirName)[0]);
        }

        // ������������ ��������� ���������
        private const string constSvodDirName = "�������";
        private const string constStrDirName = "������";
        private const string constRegDirName = "������";
        private void CheckDirectories(DirectoryInfo dir)
        {
            noSvodReports =
                ((this.Region == RegionName.Orenburg) && (this.DataSource.Year == 2007)) ||
                ((this.Region == RegionName.Tyva) && ((this.DataSource.Year <= 2008) || (this.DataSource.Year >= 2010)));
            DirectoryInfo[] svod = dir.GetDirectories(constSvodDirName, SearchOption.TopDirectoryOnly);
            DirectoryInfo[] str = dir.GetDirectories(constStrDirName, SearchOption.TopDirectoryOnly);
            DirectoryInfo[] reg = dir.GetDirectories(constRegDirName, SearchOption.TopDirectoryOnly);
            // ������� "�������" ������ ��������������
            if (svod.GetLength(0) == 0)
            {
                dir.CreateSubdirectory(constSvodDirName);
                if (!noSvodReports)
                    throw new Exception(string.Format("����������� ������� \"{0}\"", constSvodDirName));
            }
            if (str.GetLength(0) == 0)
                dir.CreateSubdirectory(constStrDirName);
            if (reg.GetLength(0) == 0)
                dir.CreateSubdirectory(constRegDirName);
            // �������� ������ � ������ ��� ������ ������ �� ����� ���� ��������� ������������
            if ((str.GetLength(0) > 0 && str[0].GetFiles().GetLength(0) > 0) &&
                (reg.GetLength(0) > 0 && reg[0].GetFiles().GetLength(0) > 0))
                throw new Exception("�������� \"������\" � \"������\" ��� ������ ������ �� ����� ���� ��������� ������������");
        }

        #endregion ����� ������� �������

        #region ������ � Excel

        private void CheckXlsTotalSum(ExcelHelper excelDoc, int curRow)
        {
            int sumsCount = totalSums.GetLength(0);
            for (int i = 0; i < sumsCount; i++)
            {
                decimal controlSum = CleanFactValue(excelDoc.GetValue(curRow, i + 3));
                CheckTotalSum(totalSums[i], controlSum, string.Format("�� ������� {0}", i + 3));
            }
        }

        private int PumpRegion(string regionCode, string regionName)
        {
            // ���� � �������� ������������ ����������, � ���� ������,
            // �� � ������������ ���������� ����������� ��� � �������
            if (!cacheRegionsNames.ContainsKey(regionCode))
            {
                // ��������: ����������� �� ����� ������������, �� � ������ �����
                if (cacheRegionsNames.ContainsValue(regionName.ToUpper()))
                {
                    // ���� ��, �� ���������� �������� ������������ � ������ ���������� ������ � ����� �� �������������
                    if (cacheRegionsFirstRow.ContainsKey(regionName.ToUpper()))
                    {
                        DataRow firstRow = cacheRegionsFirstRow[regionName.ToUpper()];
                        firstRow["Name"] = string.Format("{0} ({1})", firstRow["Name"], firstRow["Code"]);
                        cacheRegionsFirstRow.Remove(regionName.ToUpper());
                    }
                    regionName = string.Format("{0} ({1})", regionName, regionCode);
                }
                cacheRegionsNames.Add(regionCode, regionName.ToUpper());
            }
            object[] mapping = new object[] { "NAME", regionName, "CODE", regionCode };
            string regionKey = string.Format("{0}|{1}", regionCode, regionName).ToUpper();
            DataRow regionRow = PumpCachedRow(cacheRegions, dsRegions.Tables[0], clsRegions, regionKey, mapping, false);
            // ���������� ������� � ����������� ��������������
            if (!cacheRegionsFirstRow.ContainsKey(regionName.ToUpper()))
                cacheRegionsFirstRow.Add(regionName.ToUpper(), regionRow);
            return Convert.ToInt32(regionRow["ID"]);
        }

        private void PumpFactRow(decimal valueReport, int refDate, int refMarks, int refRegions, int sumIndex)
        {
            if (valueReport == 0)
                return;

            totalSums[sumIndex] += valueReport;
            valueReport *= sumMultiplier;

            if (reportType == ReportType.Svod)
            {
                object[] mapping = new object[] { "ValueReport", valueReport, "RefYearDayUNV", refDate, "RefMarks", refMarks };
                PumpRow(dsFNS5NIOTotal.Tables[0], mapping);
            }
            else
            {
                object[] mapping = new object[] { "ValueReport", valueReport, "RefYearDayUNV", refDate, "RefMarks", refMarks, "RefRegions", refRegions };
                PumpRow(dsFNS5NIORegions.Tables[0], mapping);
                if (dsFNS5NIORegions.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
                {
                    UpdateData();
                    ClearDataSet(daFNS5NIORegions, ref dsFNS5NIORegions);
                }
            }
        }

        // ���������� ��������� ��� ������� ������������ � ���� ������� �� ������,
        // �������������� � ������� "��������� ����� ����������� ������������"
        Regex regExRegionsData = new Regex(@"[0-9]{11}", RegexOptions.IgnoreCase);
        private int PumpXlsRegions(ExcelHelper excelDoc, int curRow)
        {
            string regionName = string.Empty;
            string regionCode = string.Empty;
            if (this.Region == RegionName.Altay)
            {
                regionCode = CommonRoutines.TrimLetters(excelDoc.GetValue(curRow, 1).Trim());
                regionName = excelDoc.GetValue(curRow - 2, 1).Trim();
            }
            else if ((this.Region == RegionName.Samara) && (this.DataSource.Year >= 2010))
            {
                if (excelDoc.GetValue(curRow + 1, 1).Trim().ToUpper().Contains("���"))
                {
                    regionName = excelDoc.GetValue(curRow + 2, 1).Trim();
                    regionCode = CommonRoutines.TrimLetters(excelDoc.GetValue(curRow + 3, 1).Trim());
                }
                else if (excelDoc.GetValue(curRow + 2, 1).Trim().ToUpper().Contains("���"))
                {
                    regionName = excelDoc.GetValue(curRow + 1, 1).Trim();
                    regionCode = CommonRoutines.TrimLetters(excelDoc.GetValue(curRow + 3, 1).Trim());
                }
                else
                {
                    regionName = excelDoc.GetValue(curRow + 1, 1).Trim();
                    regionCode = CommonRoutines.TrimLetters(excelDoc.GetValue(curRow + 2, 1).Trim());
                }
            }
            else if ((this.Region == RegionName.Samara) && (this.DataSource.Year == 2007))
            {
                string cellValue = excelDoc.GetValue(curRow, 1).Trim();
                regionCode = regExRegionsData.Match(cellValue).Value;
                regionName = regExRegionsData.Split(cellValue)[1].Trim();
            }
            else if ((this.Region == RegionName.Orenburg) && (this.DataSource.Year >= 2008))
            {
                regionName = excelDoc.GetValue(curRow - 1, 3).Trim();
                regionCode = excelDoc.GetValue(curRow, 3).Trim();
            }
            else
            {
                regionName = excelDoc.GetValue(curRow + 1, 1).Trim();
                regionCode = CommonRoutines.TrimLetters(excelDoc.GetValue(curRow + 2, 1).Trim());
            }
            if (regionName == string.Empty)
                regionName = regionCode;
            return PumpRegion(regionCode, regionName);
        }

        private int PumpXlsRegionsTyva(ExcelHelper excelDoc)
        {
            int rowsCount = excelDoc.GetRowsCount();
            for (int curRow = 1; curRow <= rowsCount; curRow++)
            {
                string cellValue = excelDoc.GetValue(curRow, 2).Trim().ToUpper();
                if (cellValue == MARK_REGION)
                {
                    string regionCode = CommonRoutines.TrimLetters(excelDoc.GetValue(curRow, 4).Trim());
                    string regionName = excelDoc.GetValue(curRow, 5).Trim();
                    return PumpRegion(regionCode, regionName);
                }
            }
            return nullRegions;
        }

        private int PumpXlsMarks(ExcelHelper excelDoc, int curRow, string name)
        {
            int code = Convert.ToInt32(excelDoc.GetValue(curRow, 2).Trim());
            SetSumMultiplier(code);
            object[] mapping = new object[] { "NAME", name, "CODE", code };

            if ((reportType == ReportType.Svod) || noSvodReports)
                return PumpCachedRow(cacheMarks, dsMarks.Tables[0], clsMarks, mapping, code.ToString(), "ID");
            return FindCachedRow(cacheMarks, code.ToString(), nullMarks);
        }

        private void PumpXlsRow(ExcelHelper excelDoc, int curRow, int refDate, int refRegions, string marksName)
        {
            int refMarks = PumpXlsMarks(excelDoc, curRow, marksName);
            decimal valueReport = CleanFactValue(excelDoc.GetValue(curRow, 3).Trim());
            PumpFactRow(valueReport, refDate, refMarks, refRegions, 0);
        }

        #region ������ � ������� �����

        private int FindRegion(string regionCode, string regionName)
        {
            string regionKey = string.Format("{0}|{1}", regionCode, regionName).ToUpper();
            if (cacheRegions.ContainsKey(regionKey))
                return Convert.ToInt32(cacheRegions[regionKey]["ID"]);
            regionKey = string.Format("{0}|{1} ({0})", regionCode, regionName).ToUpper();
            if (cacheRegions.ContainsKey(regionKey))
                return Convert.ToInt32(cacheRegions[regionKey]["ID"]);
            return nullRegions;
        }

        private const string SUF = "[SUF]";
        private int PumpXlsRegionsStr(ExcelHelper excelDoc, int curRow)
        {
            for (int curCol = 3; ; curCol++)
            {
                string cellValue = excelDoc.GetValue(curRow, curCol).Trim();
                if (cellValue.Trim().ToUpper() == SUF)
                {
                    return (curCol - 3);
                }
                string regionCode = CommonRoutines.TrimLetters(cellValue);
                string regionName = excelDoc.GetValue(curRow - 1, curCol).Trim();
                PumpRegion(regionCode, regionName);
            }
        }

        private void PumpXlsRowStr(ExcelHelper excelDoc, int curRow, int refDate, string marksName)
        {
            int refMarks = PumpXlsMarks(excelDoc, curRow, marksName);
            for (int curCol = 3; ; curCol++)
            {
                string cellValue = excelDoc.GetValue(regionsRow, curCol).Trim();
                if (cellValue.Trim().ToUpper() == SUF)
                {
                    return;
                }
                string regionCode = CommonRoutines.TrimLetters(cellValue);
                string regionName = excelDoc.GetValue(regionsRow - 1, curCol).Trim();
                int refRegions = FindRegion(regionCode, regionName);

                decimal valueReport = CleanFactValue(excelDoc.GetValue(curRow, curCol));
                PumpFactRow(valueReport, refDate, refMarks, refRegions, curCol - 3);
            }
        }

        #endregion ������ � ������� �����

        // � ��������� ������� ������ ������ ����������� ���� ��������������� ������� - �� �� ������
        private const string AUX_TABLE_MARK_REGION = "��� �����";
        private bool IsAuxTable(string cellValue)
        {
            if (reportType == ReportType.Region)
                return (cellValue.ToUpper() == AUX_TABLE_MARK_REGION);
            return false;
        }

        private bool IsSectionStart(string cellValue)
        {
            return (cellValue.ToUpper() == "�");
        }

        private const string MARK_REGION = "������������� �����������";
        private const string TAX_DEP_ROW = "��������� �����";
        private bool IsRegionRow(string cellValue)
        {
            if ((reportType != ReportType.Region) || isTyvaRegion2008 || isTyvaRegion2010)
                return false;
            if ((this.Region == RegionName.Orenburg) && (this.DataSource.Year >= 2008))
                return (cellValue.ToUpper() == "�");
            if (((this.Region == RegionName.Samara) && (this.DataSource.Year == 2007)) || (this.Region == RegionName.Altay))
                return (cellValue.ToUpper().StartsWith(TAX_DEP_ROW));
            return (cellValue.ToUpper() == MARK_REGION);
        }

        public const string TOTAL_ROW = "����������� �����";
        private bool IsSectionEnd(string cellValue)
        {
            return cellValue.ToUpper().Contains(TOTAL_ROW);
        }

        // �������� ������ ������������ ����������,
        // �.�. ��� ����� ���������� �� ���������� �������
        private string GetXlsMarksName(ExcelHelper excelDoc, ref int curRow)
        {
            List<string> marksName = new List<string>();
            while (excelDoc.GetValue(curRow, 2).Trim() == string.Empty)
            {
                marksName.Add(excelDoc.GetValue(curRow, 1).Trim());
                curRow++;
            }
            marksName.Add(excelDoc.GetValue(curRow, 1).Trim());
            return string.Join(" ", marksName.ToArray());
        }

        private void PumpXlsSheetData(string fileName, ExcelHelper excelDoc, int refDate, int refRegions)
        {
            bool toPumpRow = false;
            string dataSourcePath = GetShortSourcePathBySourceID(this.SourceID);
            int rowsCount = excelDoc.GetRowsCount();
            for (int curRow = 1; curRow <= rowsCount; curRow++)
            {
                try
                {
                    SetProgress(rowsCount, curRow,
                        string.Format("��������� ����� {0}\\{1}...", dataSourcePath, fileName),
                        string.Format("������ {0} �� {1}", curRow, rowsCount));

                    string cellValue = excelDoc.GetValue(curRow, 1).Trim();
                    if (cellValue == string.Empty)
                        continue;

                    if (IsSectionEnd(cellValue))
                    {
                        CheckXlsTotalSum(excelDoc, curRow);
                        toPumpRow = false;
                    }

                    if (IsRegionRow(cellValue))
                    {
                        refRegions = PumpXlsRegions(excelDoc, curRow);
                        if ((this.Region != RegionName.Orenburg) || (this.DataSource.Year < 2008))
                        {
                            curRow += 2;
                            continue;
                        }
                    }

                    if (toPumpRow)
                    {
                        string markName = GetXlsMarksName(excelDoc, ref curRow);
                        if ((reportType == ReportType.Str) || isTyvaRegion2010)
                            PumpXlsRowStr(excelDoc, curRow, refDate, markName);
                        else
                            PumpXlsRow(excelDoc, curRow, refDate, refRegions, markName);
                    }

                    if (IsSectionStart(cellValue))
                    {
                        if (IsAuxTable(excelDoc.GetValue(curRow + 1, 1).Trim()))
                            continue;

                        int columnsCount = 1;
                        if ((reportType == ReportType.Str) || isTyvaRegion2010)
                        {
                            regionsRow = curRow;
                            columnsCount = PumpXlsRegionsStr(excelDoc, curRow);
                        }
                        totalSums = new decimal[columnsCount];
                        SetNullTotalSum();

                        toPumpRow = true;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format(
                        "��� ��������� ������ {0} ����� '{1}' �������� ������ ({2})",
                        curRow, excelDoc.GetWorksheetName(), ex.Message), ex);
                }
            }
        }

        private bool IsSkipWorksheet(string worksheetName)
        {
            if ((this.Region != RegionName.Omsk) || (reportType != ReportType.Svod) || (this.DataSource.Year >= 2010))
                return false;
            return (!worksheetName.Trim().ToUpper().Contains("����"));
        }

        private bool IsTitleSheet(string worksheetName)
        {
            worksheetName = worksheetName.ToUpper();
            return (isTyvaRegion2008 && worksheetName.StartsWith("���") && worksheetName.EndsWith("����"));
        }

        private void PumpXlsFile(FileInfo file)
        {
            WriteToTrace("�������� ���������: " + file.Name, TraceMessageKind.Information);
            ExcelHelper excelDoc = new ExcelHelper();
            try
            {
                excelDoc.AskToUpdateLinks = false;
                excelDoc.DisplayAlerts = false;
                excelDoc.OpenDocument(file.FullName);
                int refDate = GetReportDate();
                int refRegions = nullRegions;
                int wsCount = excelDoc.GetWorksheetsCount();
                for (int index = 1; index <= wsCount; index++)
                {
                    excelDoc.SetWorksheet(index);
                    if (IsSkipWorksheet(excelDoc.GetWorksheetName()))
                        continue;
                    else if (IsTitleSheet(excelDoc.GetWorksheetName()))
                        refRegions = PumpXlsRegionsTyva(excelDoc);
                    else
                        PumpXlsSheetData(file.Name, excelDoc, refDate, refRegions);
                }
            }
            finally
            {
                if (excelDoc != null)
                    excelDoc.CloseDocument();
            }
        }

        #endregion ������ � Excel

        #region ������ � Txt

        /// <summary>
        /// ���������� ������� ������
        /// </summary>
        private void PumpTotalReports()
        {
            int totalRecs;
            int rowsCount = 0;
            int skippedReports = 0;
            int skippedRows = 0;
            int processedReports = 0;
            int date = 0;
            string err = string.Empty;
            string processedFiles = "<��� ������>";

            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "����� ������� ������ ������� �������.");
            try
            {
                string dirName = GetShortSourcePathBySourceID(this.SourceID);
                totalRecs = GetTotalRecs();

                // ���������� ���������� ������
                // ������ ������� �������� - ���������, �� �� �����
                for (int i = 1; i < this.ResultDataSet.Tables.Count; i++)
                {
                    DataTable dt = this.ResultDataSet.Tables[i];
                    if (dt.Rows.Count == 0)
                        continue;
                    int fileIndex = Convert.ToInt32(dt.Rows[0][this.FileIndexFieldName]);
                    processedFiles = GetStringCellValue(this.ResultDataSet.Tables[0].Rows[fileIndex], "FILES", "<��� ������>");
                    // ���� �������
                    string str = this.FixedParameters[fileIndex]["ReportDate"].Value;
                    if (str != string.Empty)
                    {
                        date = CommonRoutines.DecrementDate(Convert.ToInt32(str));
                        if (date / 10000 != this.DataSource.Year || (date / 100) % 100 != this.DataSource.Month)
                        {
                            skippedReports++;
                            skippedRows += this.ResultDataSet.Tables[i].Rows.Count;
                            continue;
                        }
                    }
                    double totalSum = 0;
                    for (int j = 0; j < dt.Rows.Count; j++)
                    {
                        double sum = Convert.ToDouble(dt.Rows[j]["TOTAL"]);

                        // ���� �� ����������. �� ���������� ��� ����������� �����
                        if (Convert.ToString(dt.Rows[j]["MARKNAME"]).ToUpper() == "����������� �����")
                        {
                            // ���� �� �������� ����� �� ����� � ����������� �����, �������� ��������� � ��������
                            if (totalSum != sum)
                            {
                                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning,
                                    string.Format("����� �� ����� {0} �� �������� � ����������� ������ {1}.", totalSum, sum));
                            }
                            totalSum = 0;
                            continue;
                        }
                        else
                        {
                            totalSum += Convert.ToDouble(dt.Rows[j]["TOTAL"]);
                        }
                        if (sum == 0)
                            continue;
                        int id = PumpOriginalRow(dsMarks, clsMarks, new object[] {
                            "CODE", dt.Rows[j]["STRCODE"], "NAME", dt.Rows[j]["MARKNAME"] });

                        PumpRow(dsFNS5NIOTotal.Tables[0], new object[] { "VALUE", sum, "REFMARKS", id, "RefYearDayUNV", date });
                        rowsCount++;
                        this.SetProgress(totalRecs, rowsCount,
                            string.Format("�������� {0}. ��������� ������...", dirName),
                            string.Format("������ {0} �� {1}", rowsCount, totalRecs));
                    }
                    processedReports++;
                }

                WriteEventIntoDataPumpProtocol(
                    DataPumpEventKind.dpeSuccessfullFinishFilePump, string.Format(
                        "������� ������ ������� ������� ���������. ���������� �������: {0} ({1} �����), " +
                        "�� ��� ��������� ��-�� �������������� ���� ���������: {2} ������� ({3} �����).",
                        processedReports, rowsCount, skippedReports, skippedRows));
            }
            catch (Exception ex)
            {
                WriteEventIntoDataPumpProtocol(
                    DataPumpEventKind.dpeFinishFilePumpWithError, string.Format(
                        "������� ������ ������� ������� ��������� � ��������: {0}. \n" +
                        "�� ������ ������������� ������ ���������� ��������� ����������. " +
                        "���������� �������: {1} ({2} �����), " +
                        "�� ��� ��������� ��-�� �������������� ���� ���������: {3} ������� ({4} �����). " +
                        "������ �������� ��� ��������� ������ {5}.",
                        ex.Message, processedReports, rowsCount, skippedReports, skippedRows, processedFiles));
                throw;
            }
        }

        /// <summary>
        /// ���������� ������ �������
        /// </summary>
        private void PumpRegionsReports()
        {
            int totalRecs;
            int rowsCount = 0;
            int skippedReports = 0;
            int skippedRows = 0;
            int processedReports = 0;
            int date = 0;
            string err = string.Empty;
            string processedFiles = "<��� ������>";

            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "����� ������� ������ ������� �������.");
            try
            {
                string dirName = GetShortSourcePathBySourceID(this.SourceID);
                totalRecs = GetTotalRecs();
                // ���������� ���������� ������
                // ������ ������� �������� - ���������, �� �� �����
                for (int i = 1; i < this.ResultDataSet.Tables.Count; i++)
                {
                    DataTable dt = this.ResultDataSet.Tables[i];
                    if (dt.Rows.Count == 0)
                        continue;
                    int fileIndex = Convert.ToInt32(dt.Rows[0][this.FileIndexFieldName]);
                    processedFiles = GetStringCellValue(this.ResultDataSet.Tables[0].Rows[fileIndex], "FILES", "<��� ������>");
                    // ���� �������
                    string str = this.FixedParameters[fileIndex]["ReportDate"].Value;
                    if (str != string.Empty)
                    {
                        date = CommonRoutines.DecrementDate(Convert.ToInt32(str));

                        if (date / 10000 != this.DataSource.Year || (date / 100) % 100 != this.DataSource.Month)
                        {
                            skippedReports++;
                            skippedRows += this.ResultDataSet.Tables[i].Rows.Count;
                            continue;
                        }
                    }
                    double totalSum = 0;
                    for (int j = 0; j < dt.Rows.Count; j++)
                    {
                        if (Convert.ToString(dt.Rows[j]["MARKNAME"]).Replace("\\", string.Empty).Trim(' ', '-').ToUpper().Contains("����������� �����"))
                        {
                            break;
                        }
                        double sum = Convert.ToDouble(dt.Rows[j]["TOTAL"]);
                        // ���� �� ����������. �� ���������� ��� ����������� �����
                        if (Convert.ToString(dt.Rows[j]["REGIONNAME"]).ToUpper() == "�����:")
                        {
                            // ���� �� �������� ����� �� ������� � ����, �������� ��������� � ��������
                            if (totalSum != sum)
                            {
                                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning,
                                    string.Format("����� �� ������� {0} �� �������� � �������� ������ ������ {1}.", totalSum, sum));
                            }
                            totalSum = 0;
                            continue;
                        }
                        else
                        {
                            totalSum += sum;
                        }
                        if (sum == 0)
                            continue;
                        int regionID = PumpOriginalRow(dsRegions, clsRegions, new object[] {
                            "CODE", dt.Rows[j]["STRCODE"], "NAME", dt.Rows[j]["REGIONNAME"] });
                        int markID = FindRowID(dsMarks.Tables[0], new object[] { "CODE", dt.Rows[j]["MARKCODE"] }, nullMarks);
                        if (markID == nullMarks)
                        {
                            markID = PumpRow(dsMarks.Tables[0], clsMarks, new object[] {
                               "CODE", dt.Rows[j]["MARKCODE"], "NAME", dt.Rows[j]["MARKNAME"] });
                        }
                        PumpRow(dsFNS5NIORegions.Tables[0], new object[] {
                            "VALUE", sum, "REFMARKS", markID, "REFREGIONS", regionID, "RefYearDayUNV", date });
                        rowsCount++;
                        this.SetProgress(totalRecs, rowsCount,
                            string.Format("�������� {0}. ��������� ������...", dirName),
                            string.Format("������ {0} �� {1}", rowsCount, totalRecs));
                    }
                    processedReports++;
                }

                WriteEventIntoDataPumpProtocol(
                    DataPumpEventKind.dpeSuccessfullFinishFilePump, string.Format(
                        "������� ������ ������� ������� ���������. ���������� �������: {0} ({1} �����), " +
                        "�� ��� ��������� ��-�� �������������� ���� ���������: {2} ������� ({3} �����).",
                        processedReports, rowsCount, skippedReports, skippedRows));
            }
            catch (Exception ex)
            {
                WriteEventIntoDataPumpProtocol(
                    DataPumpEventKind.dpeFinishFilePumpWithError, string.Format(
                        "������� ������ ������� ������� ��������� � ��������: {0}. \n" +
                        "�� ������ ������������� ������ ���������� ��������� ����������. " +
                        "���������� �������: {1} ({2} �����), " +
                        "�� ��� ��������� ��-�� �������������� ���� ���������: {3} ������� ({4} �����). " +
                        "������ �������� ��� ��������� ������ {5}.",
                        ex.Message, processedReports, rowsCount, skippedReports, skippedRows, processedFiles));
                throw;
            }
        }

        /// <summary>
        /// ������� ������� �� TXT-������
        /// </summary>
        private void PumpReports(DirectoryInfo dir)
        {
            try
            {
                this.CallTXTSorcerer(xmlSettingsForm5NIO_Total, dir.FullName);
                if (GetTotalRecs() == 0)
                {
                    throw new Exception("������� ����� ���� ��� ����������� ������� ��������� �������.");
                }
                PumpTotalReports();
            }
            catch (FilesNotFoundException)
            {
                //if (GetFactRecordsAmount(this.DB, fctFNS1OBLTotal, this.SourceID, string.Empty) == 0)
                if (dsMarks.Tables[0].Rows.Count == 0)
                {
                    throw new Exception("������� ������ ������� ������� �� ����� ���� ��������� - " +
                        "��� ������� ������ ������� ������� ������ ���� ������� ������� ����");
                }
            }
            catch (Exception ex)
            {
                WriteEventIntoDataPumpProtocol(
                    DataPumpEventKind.dpeWarning, "������� ������ ������� ������� ��������� � ��������", ex);
                throw;
            }

            try
            {
                this.CallTXTSorcerer(xmlSettingsForm5NIO_Regions, dir.FullName);
                PumpRegionsReports();
            }
            catch (FilesNotFoundException)
            {

            }
            catch (Exception ex)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, "������� ������ ������� ������� ��������� � ��������", ex);
                throw;
            }
        }

        #endregion ������ � Txt

        #region ������ � Rar

        private void PumpRarFile(FileInfo file)
        {
            DirectoryInfo tempDir = CommonRoutines.ExtractArchiveFileToTempDir(
                file.FullName, FilesExtractingOption.SingleDirectory, ArchivatorName.Rar);
            try
            {
                ProcessAllFiles(tempDir);
            }
            finally
            {
                CommonRoutines.DeleteDirectory(tempDir);
            }
        }

        #endregion ������ � Rar

        #region ���������� ������ �������

        /// <summary>
        /// ���������� �����
        /// </summary>
        /// <param name="dir">������� � ������� ��� �������</param>
        protected override void ProcessFiles(DirectoryInfo dir)
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, "���� ����� ���������� ����������� ���������");
            cacheRegionsNames = new Dictionary<string, string>();
            cacheRegionsFirstRow = new Dictionary<string, DataRow>();
            try
            {
                CheckDirectories(dir);
                PumpFiles(dir);
                UpdateData();
            }
            finally
            {
                cacheRegionsFirstRow.Clear();
                cacheRegionsNames.Clear();
            }
        }

        /// <summary>
        /// ������� ������
        /// </summary>
        protected override void DirectPumpData()
        {
            PumpDataYMTemplate();
        }

        #endregion ���������� ������ �������

        #endregion ������� ������

        #region ��������� ������

        private const string ROUBLE_UNIT_NAME = "�����";
        private const string UNIT_UNIT_NAME = "�������";
        private string GetUnitName(int marksCode)
        {
            if (this.DataSource.Year >= 2010)
            {
                if (marksCode == 1010)
                    return UNIT_UNIT_NAME;
                return ROUBLE_UNIT_NAME;
            }
            return ROUBLE_UNIT_NAME;
        }

        private int GetRefUnits(int marksCode)
        {
            if (this.DataSource.Year >= 2007)
            {
                string unitName = GetUnitName(marksCode);
                return FindCachedRow(cacheUnits, unitName, nullUnits);
            }
            return -1;
        }

        protected void SetRefUnits()
        {
            foreach (DataRow row in dsMarks.Tables[0].Rows)
            {
                int refUnits = GetRefUnits(Convert.ToInt32(row["Code"]));
                row["RefUnits"] = refUnits;
            }
        }

        private void SetClsHierarchy()
        {
            string d_Marks_FNS4_HierarchyFileName = string.Empty;
            if (this.DataSource.Year >= 2010)
                d_Marks_FNS4_HierarchyFileName = const_d_Marks_FNS4_HierarchyFile2010;
            else
                d_Marks_FNS4_HierarchyFileName = const_d_Marks_FNS4_HierarchyFile2007;
            SetClsHierarchy(clsMarks, ref dsMarks, "CODE", d_Marks_FNS4_HierarchyFileName, ClsHierarchyMode.Special);
        }

        private void CorrectSumByHierarchy()
        {
            CommonSumCorrectionConfig commonCorrectionConfig = new CommonSumCorrectionConfig();
            commonCorrectionConfig.Sum1 = "Value";
            commonCorrectionConfig.Sum1Report = "ValueReport";
            GroupTable(fctFNS5NIOTotal, new string[] { "RefMarks", "RefYearDayUNV" }, commonCorrectionConfig);
            CorrectFactTableSums(fctFNS5NIOTotal, dsMarks.Tables[0], clsMarks, "RefMarks",
                commonCorrectionConfig, BlockProcessModifier.MRStandard, new string[] { "RefYearDayUNV" }, string.Empty, string.Empty, true);
            GroupTable(fctFNS5NIORegions, new string[] { "RefMarks", "RefYearDayUNV", "RefRegions" }, commonCorrectionConfig);
            CorrectFactTableSums(fctFNS5NIORegions, dsMarks.Tables[0], clsMarks, "RefMarks",
                commonCorrectionConfig, BlockProcessModifier.MRStandard, new string[] { "RefYearDayUNV" }, "RefRegions", string.Empty, true);
        }

        protected override void ProcessDataSource()
        {
            SetClsHierarchy();
            CorrectSumByHierarchy();
            SetRefUnits();
            UpdateData();
        }

        protected override void DirectProcessData()
        {
            year = -1;
            month = -1;
            GetPumpParams(ref year, ref month);
            ProcessDataSourcesTemplate(year, month, "����������� ������������� ���� ������ �� �������� ���������������");
        }

        #endregion ��������� ������

    }
}