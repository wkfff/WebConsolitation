using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.FNS10Pump
{
    // ��� - 0010 - ����� 5-����
    public class FNS10PumpModule : CorrectedPumpModuleBase
    {

        #region ����

        #region ��������������

        // ����������.��� 5 ���� (d_Marks_FNS5NDFL)
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

        #endregion ��������������

        #region �����

        // ������.���_5 ����_������� (f_D_FNS5NDFLTotal)
        private IDbDataAdapter daIncomesTotal;
        private DataSet dsIncomesTotal;
        private IFactTable fctIncomesTotal;
        // ������.���_5 ����_������ (f_D_FNS5NDFLRegions)
        private IDbDataAdapter daIncomesRegion;
        private DataSet dsIncomesRegion;
        private IFactTable fctIncomesRegion;

        #endregion �����

        private ReportType reportType;
        private int sectionIndex = -1;
        private int sectionsCount;
        private int formatYear;
        private bool noSvodReports = false;
        // ��������� ���������
        private int year = -1;
        private int month = -1;
        private int marksParentCode = -1;
        private int marksParentID = -1;

        #endregion ����

        #region ���������, ������������

        private enum ReportType
        {
            Svod,
            Str,
            Region
        }

        #endregion ���������, ������������

        #region ���������

        // ������ ������ ������ � �����
        private const string DATE_CELL_TEXT = "�� ��������� ��";
        // ������������ ������
        private string[] sectionNames2005 = new string[] {
            "������ I. ��������� ����, ���������� ��������������� �� ������ 13%, ����� ����� ������������, ����������� � �������������� ������",
            "������ II. ��������� ����, ���������� ��������������� �� ������ 30%, ����� ����� ������������, ����������� � �������������� ������",
            "������ III. ��������� ����, ���������� ��������������� �� ������ 9%, ����� ����� ������������ ����������� � �������������� ������",
            "������ IV. ��������� ����, ���������� ��������������� �� ������ 35%, ����� ����� ������������, ����������� � �������������� ������",
            "������ V. ��������� ����, ���������� ��������������� �� ��������� �������, ������������� � ����������� �� ��������� �������� ���������������, ����� ����� ������������, ����������� � �������������� ������ (5%, 10%, 15%)",
            "������ VI. ��������� ������" };
        private string[] sectionNames2007 = new string[] {
            "������ I. ��������� ����, ���������� ��������������� �� ������ 13%, � ����� ������",
            "������ II. ��������� ����, ���������� ��������������� �� ������ 30%, � ����� ������",
            "������ III. ��������� ����, ���������� ��������������� �� ������ 9%, � ����� ������",
            "������ IV. ��������� ����, ���������� ��������������� �� ������ 35%, � ����� ������",
            "������ V. ��������� ����, ���������� ��������������� �� ��������� �������, ������������� � ����������� �� ��������� �������� ���������������, � ����� ������",
            "������ VI. ��������� ������" };
        private string[] sectionNames2008 = new string[] {
            "������ I. ��������� ����, ���������� ��������������� �� ������ 13%, � ����� ������",
            "������ II. ��������� ����, ���������� ��������������� �� ������ 30%, � ����� ������",
            "������ III. ��������� ����, ���������� ��������������� �� ������ 9%, � ����� ������",
            "������ IV. ��������� ����, ���������� ��������������� �� ������ 35%, � ����� ������",
            "������ V. ��������� ����, ���������� ��������������� �� ���� ��������� ������� � ����� ������",
            "������ VI. �������� � ������������� � ����������� ��������� �������",
            "������ VII. �������� � ��������� �������, ��������������� �� ��������� ����� �������" };
        private string[] sectionNames2009 = new string[] {
            "������ I. ��������� ����, ���������� ��������������� �� ������ 13%, � ����� ������",
            "������ II. ��������� ����, ���������� ��������������� �� ������ 30%, � ����� ������",
            "������ III. ��������� ����, ���������� ��������������� �� ������ 9%, � ����� ������",
            "������ IV. ��������� ����, ���������� ��������������� �� ������ 35%, � ����� ������",
            "������ V. ��������� ����, ���������� ��������������� �� ������ 15%, � ����� ������",
            "������ VI. ��������� ����, ���������� ��������������� �� ���� ��������� ������� � ����� ������",
            "������ VII. �������� � ������������� � ����������� ��������� �������",
            "������ VIII. �������� � ��������� �������, ��������������� �� ��������� ����� �������" };
        private List<string> marksSectionNamesList = new List<string>(8);
        // ������ �� �� ������� �������� � �������������� ����������.��� 5 ����
        private List<int> marksSectionRecordsIDList = new List<int>(8);

        #endregion ���������

        #region ������� ������

        #region ������ � ����� � ������

        // �������� �� �� ������� �������� �������������� ���������� ����
        private void GetMarksParentId()
        {
            if (formatYear >= 2009)
            {
                sectionsCount = 8;
                marksSectionNamesList.AddRange(sectionNames2009);
                marksSectionRecordsIDList.AddRange(new int[] { 0, 0, 0, 0, 0, 0, 0, 0 });
            }
            else if (formatYear >= 2008)
            {
                sectionsCount = 7;
                marksSectionNamesList.AddRange(sectionNames2008);
                marksSectionRecordsIDList.AddRange(new int[] { 0, 0, 0, 0, 0, 0, 0 });
            }
            else if (formatYear >= 2007)
            {
                sectionsCount = 6;
                marksSectionNamesList.AddRange(sectionNames2007);
                marksSectionRecordsIDList.AddRange(new int[] { 0, 0, 0, 0, 0, 0 });
            }
            else
            {
                sectionsCount = 6;
                marksSectionNamesList.AddRange(sectionNames2005);
                marksSectionRecordsIDList.AddRange(new int[] { 0, 0, 0, 0, 0, 0 });
            }
            
            // � ������� �������� ��� ����� ����
            // �������� �� �� ������������ ������� ��������, ���� �����-�� �������� ��� - ����������
            DataRow[] marksSectionRecords = dsMarks.Tables[0].Select("CODE = 0");
            foreach (DataRow marksSectionRecord in marksSectionRecords)
            {
                string sectionName = marksSectionRecord["NAME"].ToString().ToUpper();
                sectionIndex = GetSectionIndex(sectionName);
                if (sectionIndex == -1)
                    continue;
                marksSectionRecordsIDList[sectionIndex] = Convert.ToInt32(marksSectionRecord["ID"]);
            }

            for (sectionIndex = 0; sectionIndex < sectionsCount; sectionIndex++)
            {
                if (marksSectionRecordsIDList[sectionIndex] > 0)
                    continue;
                marksSectionRecordsIDList[sectionIndex] = PumpRow(dsMarks.Tables[0], clsMarks,
                    new object[] { "CODE", 0, "NAME", marksSectionNamesList[sectionIndex] });
            }
        }

        // �������� �� �� ������� �������� �������������� �������������
        // ��������� ������ ��������� ����� ������
        private void InitAuxStructures()
        {
            GetMarksParentId();
        }

        protected override void QueryData()
        {
            SetFormatYear();
            InitClsDataSet(ref daMarks, ref dsMarks, clsMarks);
            nullMarks = clsMarks.UpdateFixedRows(this.DB, this.SourceID);
            InitClsDataSet(ref daRegions, ref dsRegions, clsRegions);
            nullRegions = clsRegions.UpdateFixedRows(this.DB, this.SourceID);
            InitFactDataSet(ref daIncomesTotal, ref dsIncomesTotal, fctIncomesTotal);
            InitFactDataSet(ref daIncomesRegion, ref dsIncomesRegion, fctIncomesRegion);
            FillCaches();
            InitAuxStructures();
        }

        private void FillCaches()
        {
            FillRowsCache(ref cacheMarks, dsMarks.Tables[0], "CODE", "ID");
            FillRowsCache(ref cacheRegions, dsRegions.Tables[0], new string[] { "CODE", "NAME" }, "|");
        }

        protected override void UpdateData()
        {
            UpdateDataSet(daMarks, dsMarks, clsMarks);
            UpdateDataSet(daRegions, dsRegions, clsRegions);
            UpdateDataSet(daIncomesTotal, dsIncomesTotal, fctIncomesTotal);
            UpdateDataSet(daIncomesRegion, dsIncomesRegion, fctIncomesRegion);
        }

        private const string D_MARKS_FNS_5NDFL_GUID = "b1b4fb58-7b8c-44d5-8da5-92edb40185aa";
        private const string D_REGIONS_FNS_GUID = "cf3202f9-e897-43ce-a158-5c617bedff55";
        private const string F_D_FNS_5NDFL_TOTAL_GUID = "99f5b0bc-b4ec-4b76-a424-fb9509c94a6d";
        private const string F_D_FNS_5NDFL_REGIONS_GUID = "6405ebb0-4bb4-44ca-a300-34a5f620cc57";
        protected override void InitDBObjects()
        {
            this.UsedClassifiers = new IClassifier[] {
                clsMarks = this.Scheme.Classifiers[D_MARKS_FNS_5NDFL_GUID],
                clsRegions = this.Scheme.Classifiers[D_REGIONS_FNS_GUID] };
            this.UsedFacts = new IFactTable[] {
                fctIncomesTotal = this.Scheme.FactTables[F_D_FNS_5NDFL_TOTAL_GUID],
                fctIncomesRegion = this.Scheme.FactTables[F_D_FNS_5NDFL_REGIONS_GUID] };
        }

        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsIncomesTotal);
            ClearDataSet(ref dsIncomesRegion);
            ClearDataSet(ref dsMarks);
            ClearDataSet(ref dsRegions);
            marksSectionRecordsIDList.Clear();
            marksSectionNamesList.Clear();
        }

        #endregion ������ � ����� � ������

        #region ����� ������� �������

        private int GetReportDate()
        {
            return this.DataSource.Year * 10000 + this.DataSource.Month * 100;
        }

        private DataTable GetFactTable()
        {
            if (reportType == ReportType.Svod)
                return dsIncomesTotal.Tables[0];
            return dsIncomesRegion.Tables[0];
        }

        private void SetFormatYear()
        {
            switch (this.Region)
            {
                case RegionName.Tula:
                case RegionName.Kostroma:
                case RegionName.Stavropol:
                case RegionName.Vologda:
                case RegionName.Samara:
                    if (this.DataSource.Year >= 2007 && this.DataSource.Year <= 2008)
                        formatYear = 2008;
                    else
                        formatYear = this.DataSource.Year;
                    break;
                default:
                    formatYear = this.DataSource.Year;
                    break;
            }
        }

        private int GetSectionIndex(string cellValue)
        {
            cellValue = cellValue.ToUpper();
            if (cellValue.Contains("������ III"))
                return 2;
            else if (cellValue.Contains("������ II"))
                return 1;
            else if (cellValue.Contains("������ IV"))
                return 3;
            else if (cellValue.Contains("������ I"))
                return 0;
            else if (cellValue.Contains("������ VIII"))
                return 7;
            else if (cellValue.Contains("������ VII"))
                return 6;
            else if (cellValue.Contains("������ VI"))
                return 5;
            else if (cellValue.Contains("������ V"))
                return 4;
            return -1;
        }

        private void CheckMarks()
        {
            if (!noSvodReports && (cacheMarks.Count == 0))
                throw new Exception("�� �������� ������������� '����������.��� 5 ����' - ��������� ������� ������");
        }

        // ������������ ��������� ���������
        private const string constSvodDirName = "�������";
        private const string constStrDirName = "������";
        private const string constRegDirName = "������";
        private void CheckDirectories(DirectoryInfo dir)
        {
            DirectoryInfo[] svod = dir.GetDirectories(constSvodDirName, SearchOption.TopDirectoryOnly);
            DirectoryInfo[] str = dir.GetDirectories(constStrDirName, SearchOption.TopDirectoryOnly);
            DirectoryInfo[] reg = dir.GetDirectories(constRegDirName, SearchOption.TopDirectoryOnly);
            noSvodReports =
                ((this.Region == RegionName.Tyva) && (this.DataSource.Year <= 2008)) ||
                (this.Region == RegionName.EAO) || (this.Region == RegionName.Omsk) ||
                (this.Region == RegionName.OmskMO) || (this.Region == RegionName.Orenburg);
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

        private void ProcessAllFiles(DirectoryInfo dir)
        {
            ProcessFilesTemplate(dir, "*.xls", new ProcessFileDelegate(PumpXlsFile), false);
            ProcessFilesTemplate(dir, "*.rar", new ProcessFileDelegate(PumpRarFile), false);
        }

        private void PumpFiles(DirectoryInfo dir)
        {
            reportType = ReportType.Svod;
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStartFilePumping, "����� ������� ������ ������� �������.");
            ProcessAllFiles(dir.GetDirectories(constSvodDirName)[0]);
            reportType = ReportType.Region;
            CheckMarks();
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStartFilePumping, "����� ������� ������ ������� � ������� �������.");
            ProcessAllFiles(dir.GetDirectories(constRegDirName)[0]);
            // reportType = ReportType.Str;
            // WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStartFilePumping, "����� ������� ������ ������� � ������� �����.");
            // ProcessAllFiles(dir.GetDirectories(constStrDirName)[0]);
        }

        #endregion ����� ������� �������

        #region ������ � Excel

        private int PumpRegion(string regionCode, string regionName)
        {
            // ���� � �������� ������������ ����������, � ���� ������,
            // �� � ������������ ���������� ����������� ��� � �������
            if (!cacheRegionsNames.ContainsKey(regionCode))
            {
                // ��������: ����������� �� ����� ������������, �� � ������ �����
                if (cacheRegionsNames.ContainsValue(regionName))
                {
                    // ���� ��, �� ���������� �������� ������������ � ������ ���������� ������ � ����� �� �������������
                    if (cacheRegionsFirstRow.ContainsKey(regionName))
                    {
                        DataRow firstRow = cacheRegionsFirstRow[regionName];
                        firstRow["Name"] = string.Format("{0} ({1})", firstRow["Name"], firstRow["Code"]);
                        cacheRegionsFirstRow.Remove(regionName);
                    }
                    regionName = string.Format("{0} ({1})", regionName, regionCode);
                }
                cacheRegionsNames.Add(regionCode, regionName);
            }
            object[] mapping = new object[] { "NAME", regionName, "CODE", regionCode };
            string regionKey = string.Format("{0}|{1}", regionCode, regionName);
            DataRow regionRow = PumpCachedRow(cacheRegions, dsRegions.Tables[0], clsRegions, regionKey, mapping, false);
            // ���������� ������� � ����������� ��������������
            if (!cacheRegionsFirstRow.ContainsKey(regionName))
                cacheRegionsFirstRow.Add(regionName, regionRow);
            return Convert.ToInt32(regionRow["ID"]);
        }

        // ���������� ��������� ��� ������ ���� ������� �� ������ ���� "������: 52223000000, ���� ������������: 03.07.2009"
        Regex regExRegionCode = new Regex(@"(?<=������: )[0-9]*(?=\,)", RegexOptions.IgnoreCase);
        private string GetRegionCode(string cellValue)
        {
            return regExRegionCode.Match(cellValue).Value.Trim();
        }

        private int PumpXlsRegions(ExcelHelper excelDoc)
        {
            string regionName = excelDoc.GetValue(4, 1).Trim();
            string regionCode = GetRegionCode(excelDoc.GetValue(5, 1).Trim());
            return PumpRegion(regionCode, regionName);
        }

        private int PumpXlsMarks(ExcelHelper excelDoc, int curRow)
        {
            int code = -1;
            string name = excelDoc.GetValue(curRow, 2).Trim();
            string value = excelDoc.GetValue(curRow, 3).Trim();
            if (value != string.Empty)
            {
                marksParentCode = Convert.ToInt32(value);
                code = marksParentCode * 10000;
                if (sectionIndex >= sectionsCount)
                    return nullMarks;
                marksParentID = marksSectionRecordsIDList[sectionIndex];
            }
            else
            {
                code = marksParentCode * 10000 + Convert.ToInt32(name);
            }
            object[] mapping = new object[] { "NAME", name, "CODE", code, "PARENTID", marksParentID };
            int refMarks;
            if ((reportType == ReportType.Svod) || noSvodReports)
                refMarks = PumpCachedRow(cacheMarks, dsMarks.Tables[0], clsMarks, mapping, code.ToString(), "ID");
            else
                refMarks = FindCachedRow(cacheMarks, code.ToString(), nullMarks);
            if (value != string.Empty)
                marksParentID = refMarks;
            return refMarks;
        }

        private bool IsBigSection()
        {
            if (this.DataSource.Year >= 2009)
                return sectionIndex >= 6;
            return sectionIndex >= 5;
        }

        private const string AUX_ROW = "� ��� ����� �� ����� ������:";
        private void PumpXlsRow(ExcelHelper excelDoc, int curRow, int refDate, int refRegions)
        {
            string value = excelDoc.GetValue(curRow, 2).Trim();
            if ((value.ToUpper() == AUX_ROW) || (value == "1"))
                return;
            value = excelDoc.GetValue(curRow, 1).Trim().ToUpper();
            if ((sectionIndex < 6) && (value == "1."))
                return;

            int refMarks = PumpXlsMarks(excelDoc, curRow);

            if (IsBigSection())
                value = excelDoc.GetValue(curRow, 5).Trim();
            else
                value = excelDoc.GetValue(curRow, 4).Trim();
            decimal factValue = Convert.ToDecimal(value.PadLeft(1, '0'));
            if (factValue == 0)
                return;

            object[] mapping = new object[] { "ValueReport", factValue, "Value", 0, "RefYearDayUNV", refDate, "RefMarks", refMarks };
            if (IsBigSection())
            {
                string taxPayers = excelDoc.GetValue(curRow, 4).Trim();
                if (taxPayers != string.Empty)
                    mapping = (object[])CommonRoutines.ConcatArrays(mapping,
                        new object[] { "TaxpayersNumberReport", Convert.ToDecimal(taxPayers), "TaxpayersNumber", 0 });
            }
            if (reportType == ReportType.Region)
                mapping = (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "RefRegions", refRegions });
            PumpRow(GetFactTable(), mapping);
            if (dsIncomesRegion.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
            {
                UpdateData();
                ClearDataSet(daIncomesRegion, ref dsIncomesRegion);
            }
        }

        private bool IsSectionEndMark(string cellValue)
        {
            return (cellValue.Trim().ToUpper().StartsWith("*"));
        }

        private const string TABLE_TITLE_MARK = "� �/�";
        private const string TABLE_TITLE_MARK_2008 = "������������ �����������";
        private bool IsTableTitle(ExcelHelper excelDoc, int curRow)
        {
            if (formatYear < 2008)
                return (string.Compare(excelDoc.GetValue(curRow, 1).Trim().ToUpper(), TABLE_TITLE_MARK, true) == 0);
            return (string.Compare(excelDoc.GetValue(curRow, 2).Trim().ToUpper(), TABLE_TITLE_MARK_2008, true) == 0);
        }

        private void PumpXlsSheetData(string fileName, ExcelHelper excelDoc, int refDate)
        {
            int refRegions = nullRegions;
            if (reportType == ReportType.Region)
                refRegions = PumpXlsRegions(excelDoc);
            sectionIndex = -1;
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
                    {
                        cellValue = excelDoc.GetValue(curRow, 2).Trim();
                        if (cellValue == string.Empty)
                            continue;
                    }

                    if (cellValue.ToUpper().StartsWith("������"))
                    {
                        sectionIndex = GetSectionIndex(cellValue);
                        toPumpRow = false;
                    }

                    if (IsSectionEndMark(cellValue))
                        toPumpRow = false;

                    if (toPumpRow)
                        PumpXlsRow(excelDoc, curRow, refDate, refRegions);

                    if (IsTableTitle(excelDoc, curRow))
                        toPumpRow = true;
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("��� ��������� ������ {0} �������� ������ ({1})", curRow, ex.Message), ex);
                }
            }
        }

        private bool IsSkipFile(FileInfo file)
        {
            if (file.Name.Contains("OPR_"))
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation,
                    string.Format("����� {0} ������� �� �����.", file.FullName));
                return true;
            }
            return false;
        }

        private void PumpXlsFile(FileInfo file)
        {
            if (IsSkipFile(file))
                return;
            WriteToTrace("�������� ���������: " + file.Name, TraceMessageKind.Information);
            ExcelHelper excelDoc = new ExcelHelper();
            try
            {
                excelDoc.AskToUpdateLinks = false;
                excelDoc.DisplayAlerts = false;
                excelDoc.OpenDocument(file.FullName);
                int refDate = GetReportDate();
                int wsCount = excelDoc.GetWorksheetsCount();
                for (int index = 1; index <= wsCount; index++)
                {
                    excelDoc.SetWorksheet(index);
                    PumpXlsSheetData(file.Name, excelDoc, refDate);
                }
            }
            finally
            {
                if (excelDoc != null)
                    excelDoc.CloseDocument();
            }
        }

        #endregion ������ � Excel

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

        protected override void ProcessFiles(DirectoryInfo dir)
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, "���� ����� ���������� ����������� ���������");
            cacheRegionsNames = new Dictionary<string, string>();
            cacheRegionsFirstRow = new Dictionary<string, DataRow>();
            try
            {
                CheckDirectories(dir);
                PumpFiles(dir);
                // �������� ������������� �������
                toSetHierarchy = false;
                SetClsHierarchy();
                UpdateData();
            }
            finally
            {
                cacheRegionsFirstRow.Clear();
                cacheRegionsNames.Clear();
            }
        }

        protected override void DirectPumpData()
        {
            PumpDataYMTemplate();
        }

        #endregion ���������� ������ �������

        #endregion ������� ������

        #region ��������� ������

        private void SetClsHierarchy()
        {
            // �������� ������������� �������
            toSetHierarchy = false;
            string d_Marks_FNS10_HierarchyFileName = string.Empty;
            if (formatYear >= 2010)
                d_Marks_FNS10_HierarchyFileName = const_d_MARKS_FNS10_HierarchyFile2010;
            else if (formatYear >= 2009)
                d_Marks_FNS10_HierarchyFileName = const_d_MARKS_FNS10_HierarchyFile2009;
            else if (formatYear >= 2008)
                d_Marks_FNS10_HierarchyFileName = const_d_MARKS_FNS10_HierarchyFile2008;
            else
                d_Marks_FNS10_HierarchyFileName = const_d_MARKS_FNS10_HierarchyFile2006;
            SetClsHierarchy(clsMarks, ref dsMarks, "CODE", d_Marks_FNS10_HierarchyFileName, ClsHierarchyMode.Special);
        }

        private void CorrectSumByHierarchy()
        {
            F1NMSumCorrectionConfig f1nmSumCorrectionConfig = new F1NMSumCorrectionConfig();
            f1nmSumCorrectionConfig.EarnedField = "Value";
            f1nmSumCorrectionConfig.EarnedReportField = "ValueReport";
            f1nmSumCorrectionConfig.InpaymentsField = "TaxpayersNumber";
            f1nmSumCorrectionConfig.InpaymentsReportField = "TaxpayersNumberReport";
            CorrectFactTableSums(fctIncomesTotal, dsMarks.Tables[0], clsMarks, "RefMarks",
                f1nmSumCorrectionConfig, BlockProcessModifier.MRStandard, new string[] { "RefYearDayUNV" }, string.Empty, string.Empty, true);
            CorrectFactTableSums(fctIncomesRegion, dsMarks.Tables[0], clsMarks, "RefMarks",
                f1nmSumCorrectionConfig, BlockProcessModifier.MRStandard, new string[] { "RefYearDayUNV" }, "RefRegions", string.Empty, true);
        }

        protected override void ProcessDataSource()
        {
            SetClsHierarchy();
            CorrectSumByHierarchy();
            UpdateData();
        }

        protected override void DirectProcessData()
        {
            year = -1;
            month = -1;
            GetPumpParams(ref year, ref month);
            ProcessDataSourcesTemplate(year, month, "��������� ���� ������ �� ������ ���������");
        }

        #endregion ��������� ������

    }
}
