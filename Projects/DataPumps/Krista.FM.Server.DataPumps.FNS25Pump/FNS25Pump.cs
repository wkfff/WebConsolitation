using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.FNS25Pump
{

    // ��� - 0025 - ����� 5-���
    public class FNS25PumpModule : CorrectedPumpModuleBase
    {

        #region ����

        #region ��������������

        // ����������.��� 5 ��� (d_Marks_FNS5DDK) 
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

        // ������.���_5 ���_������� (f_D_FNS5DDKTotal) 
        private IDbDataAdapter daIncomesTotal;
        private DataSet dsIncomesTotal;
        private IFactTable fctIncomesTotal;
        // ������.���_5 ���_������ (f_D_FNS5DDKRegions) 
        private IDbDataAdapter daIncomesRegion;
        private DataSet dsIncomesRegion;
        private IFactTable fctIncomesRegion;

        #endregion �����

        private ReportType reportType;
        private int sectionIndex = -1;
        // ��������� ���������
        private int year = -1;
        private int month = -1;
        // �������� �����
        private decimal[] totalSums = new decimal[10];
        private string regionCode;
        private string regionName;
        // ����������� �������� ����� � �����
        private decimal sumMultiplier = 1;

        private int[] startColumns;
        private int[] columnsCount;

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

        // ������������ ������ ��� �������
        private string[] sectionNames = new string[]
        {
           "������ I. �������� � �������������� � ��������� ������ � 2008 ���� ����������� ����� 3-���� �� ������� 2007 ���� (�� ������ �����������������)", 
           "������ II. ��������� ������� � ��������� ���� � ������� ��������� ������ ������ �� ����������� � ������� 2007 ���� (�� ������ �����������������)",
        };

        private List<string> marksSectionNamesList = new List<string>(2);
        // ������ �� �� ������� �������� � �������������� ����������.��� 5 ��
        private List<int> marksSectionRecordsIDList = new List<int>(2);

        #endregion ���������

        #region ������� ������

        #region ������ � ����� � ������

        // �������� �� �� ������� �������� �������������� ����������
        private void GetMarksParentId()
        {
            marksSectionNamesList.AddRange(sectionNames);
            marksSectionRecordsIDList.AddRange(new int[] { 0, 0 });
            // � ������� �������� ��� ����� ����
            // �������� �� �� ������������ ������� ��������, ���� ����� �� �������� ��� - ����������
            DataRow[] marksSectionRecords = dsMarks.Tables[0].Select("CODE = 0");
            foreach (DataRow marksSectionRecord in marksSectionRecords)
            {
                string sectionName = marksSectionRecord["NAME"].ToString().ToUpper();
                sectionIndex = GetSectionIndex(sectionName);
                if (sectionIndex == -1)
                    continue;
                marksSectionRecordsIDList[sectionIndex] = Convert.ToInt32(marksSectionRecord["ID"]);
            }
            for (sectionIndex = 0; sectionIndex <= 1; sectionIndex++)
            {
                if (marksSectionRecordsIDList[sectionIndex] > 0)
                    continue;
                marksSectionRecordsIDList[sectionIndex] = PumpRow(dsMarks.Tables[0], clsMarks,
                    new object[] { "CODE", 0, "NAME", marksSectionNamesList[sectionIndex] });
            }
        }

        protected override void QueryData()
        {
            InitDataSet(ref daUnits, ref dsUnits, clsUnits, true, string.Empty, string.Empty);
            InitClsDataSet(ref daMarks, ref dsMarks, clsMarks);
            nullMarks = clsMarks.UpdateFixedRows(this.DB, this.SourceID);
            InitClsDataSet(ref daRegions, ref dsRegions, clsRegions);
            nullRegions = clsRegions.UpdateFixedRows(this.DB, this.SourceID);
            InitFactDataSet(ref daIncomesTotal, ref dsIncomesTotal, fctIncomesTotal);
            InitFactDataSet(ref daIncomesRegion, ref dsIncomesRegion, fctIncomesRegion);
            FillCaches();
        }

        private void FillCaches()
        {
            FillRowsCache(ref cacheMarks, dsMarks.Tables[0], "CODE", "ID");
            FillRowsCache(ref cacheRegions, dsRegions.Tables[0], new string[] { "CODE", "NAME" }, "|");
            FillRowsCache(ref cacheUnits, dsUnits.Tables[0], "Name", "ID");
            GetMarksParentId();
        }

        protected override void UpdateData()
        {
            UpdateDataSet(daMarks, dsMarks, clsMarks);
            UpdateDataSet(daRegions, dsRegions, clsRegions);
            UpdateDataSet(daIncomesTotal, dsIncomesTotal, fctIncomesTotal);
            UpdateDataSet(daIncomesRegion, dsIncomesRegion, fctIncomesRegion);
        }

        private const string D_MARKS_FNS_5DDK_GUID = "6425bff8-8256-4770-a8af-da3f006bdec6";
        private const string D_REGIONS_FNS_GUID = "cf3202f9-e897-43ce-a158-5c617bedff55";
        private const string D_UNITS_OKEI_GUID = "7ef0edfd-9461-4333-8420-ccb102051826";
        private const string F_D_FNS_5DDK_TOTAL_GUID = "1247854a-45ac-4b10-a84c-67004c68230f";
        private const string F_D_FNS_5DDK_REGIONS_GUID = "c7074cef-8510-4e22-80bd-b187124515b9";
        protected override void InitDBObjects()
        {
            clsUnits = this.Scheme.Classifiers[D_UNITS_OKEI_GUID];
            this.UsedClassifiers = new IClassifier[] {
                clsMarks = this.Scheme.Classifiers[D_MARKS_FNS_5DDK_GUID],
                clsRegions = this.Scheme.Classifiers[D_REGIONS_FNS_GUID] };
            this.UsedFacts = new IFactTable[] {
                fctIncomesTotal = this.Scheme.FactTables[F_D_FNS_5DDK_TOTAL_GUID],
                fctIncomesRegion = this.Scheme.FactTables[F_D_FNS_5DDK_REGIONS_GUID] };
        }

        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsIncomesTotal);
            ClearDataSet(ref dsIncomesRegion);
            ClearDataSet(ref dsMarks);
            ClearDataSet(ref dsRegions);
            ClearDataSet(ref dsUnits);
            marksSectionRecordsIDList.Clear();
            marksSectionNamesList.Clear();
        }

        #endregion ������ � ����� � ������

        #region ����� ������� �������

        private int[] CONST_START_COLUMNS = new int[] { 4, 3 };
        private int[] CONST_COLUMNS_COUNT_2007 = new int[] { 6, 4 };
        private int[] CONST_COLUMNS_COUNT_2008 = new int[] { 6, 5 };
        private int[] CONST_COLUMNS_COUNT_REGIONS = new int[] { 5, 5 };
        private void InitAuxStructures()
        {
            startColumns = CONST_START_COLUMNS;
            if (((this.Region == RegionName.MoskvaObl) && (this.DataSource.Year != 2008)) || (this.Region == RegionName.Tyva))
                columnsCount = CONST_COLUMNS_COUNT_REGIONS;
            else if (this.DataSource.Year >= 2008)
                columnsCount = CONST_COLUMNS_COUNT_2008;
            else
                columnsCount = CONST_COLUMNS_COUNT_2007;
        }

        private int[] CONST_PERSONS_2007 = new int[] { 4, 5, 6, 7, 8, 9 };
        private int[] CONST_PERSONS_2008 = new int[] { 4, 5, 6, 7, 9, 8 };
        private int[] CONST_PERSONS_REGIONS = new int[] { 4, 5, 6, 7, 9 };
        private int GetPerson(int index)
        {
            if (sectionIndex == 0)
            {
                if (((this.Region == RegionName.MoskvaObl) && (this.DataSource.Year != 2008)) || (this.Region == RegionName.Tyva))
                    return CONST_PERSONS_REGIONS[index];
                else if (this.DataSource.Year >= 2008)
                    return CONST_PERSONS_2008[index];
                else
                    return CONST_PERSONS_2007[index];
            }
            return 3;
        }

        private int[] CONST_INCOMES = new int[] { 2, 3, 4, 5, 6 };
        private int GetIncomes(int index)
        {
            if (sectionIndex == 0)
                return 1;
            return CONST_INCOMES[index];
        }

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

        private void GetSumMultiplier(int marksCode)
        {
            switch (marksCode)
            {
                case 1020:
                case 1040:
                case 1060:
                case 1080:
                case 1100:
                case 1120:
                case 1140:
                case 1150:
                case 1160:
                case 1170:
                case 1180:
                case 1190:
                case 2020:
                case 2030:
                case 2040:
                case 2050:
                case 2060:
                case 2070:
                case 2080:
                case 2090:
                    sumMultiplier = 1000;
                    break;
                default:
                    sumMultiplier = 1;
                    break;
            }
        }

        private int GetSectionIndex(string cellValue)
        {
            if (cellValue.Contains("������ II"))
                return 1;
            else if (cellValue.Contains("������ I"))
                return 0;
            return -1;
        }

        private int GetReportDate()
        {
            // �������� �� ���������� ���������
            return this.DataSource.Year * 10000 + 1;
        }

        private void CheckMarks()
        {
            if ((reportType != ReportType.Svod) && (cacheMarks.Count == 0))
                throw new Exception("�� �������� ������������� '����������.��� 5 ���' - ��������� ������� ������");
        }

        private void ProcessAllFiles(DirectoryInfo dir)
        {
            CheckMarks();
            InitAuxStructures();
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
            // reportType = ReportType.Str;
            // WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStartFilePumping, "����� ������� ������ ������� � ������� �����.");
            // ProcessAllFiles(dir.GetDirectories(constStrDirName)[0]);
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
            // ������� "�������" ������ ��������������
            if (svod.GetLength(0) == 0)
            {
                dir.CreateSubdirectory(constSvodDirName);
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
            string comment = string.Empty;
            if (reportType != ReportType.Svod)
                comment = string.Format(" ��� ������� {0} '{1}'", regionCode, regionName);
            for (int index = 0; index < columnsCount[sectionIndex]; index++)
            {
                decimal controlSum = CleanFactValue(excelDoc.GetValue(curRow, index + startColumns[sectionIndex]));
                comment = string.Format("�� ������ {0} � ������� {1}{2}", index + startColumns[sectionIndex], sectionIndex, comment);
                CheckTotalSum(totalSums[index], controlSum, comment);
            }
        }

        private void PumpFactRow(decimal factValue, int refDate, int refMarks, int refRegions, int index)
        {
            if (factValue == 0)
                return;

            totalSums[index] += factValue;
            factValue *= sumMultiplier;

            int refPersons = GetPerson(index);
            int refIncomes = GetIncomes(index);

            if (reportType == ReportType.Svod)
            {
                object[] mapping = new object[] {
                    "Value", factValue, "RefYearDayUNV", refDate, "RefMarks", refMarks,
                    "RefTypes", refPersons, "RefTypesIncomes", refIncomes };
                PumpRow(dsIncomesTotal.Tables[0], mapping);
            }
            else
            {
                object[] mapping = new object[] {
                    "Value", factValue, "RefYearDayUNV", refDate, "RefMarks", refMarks,
                    "RefTypes", refPersons, "RefTypesIncomes", refIncomes, "RefRegions", refRegions };
                PumpRow(dsIncomesRegion.Tables[0], mapping);
                if (dsIncomesRegion.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
                {
                    UpdateData();
                    ClearDataSet(daIncomesRegion, ref dsIncomesRegion);
                }
            }
        }

        private int PumpRegion(string regionCode, string regionName)
        {
            if (regionName == string.Empty)
                regionName = regionCode;
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

        private int PumpXlsRegions(ExcelHelper excelDoc, int curRow)
        {
            regionName = excelDoc.GetValue(curRow + 1, 1).Trim();
            regionCode = CommonRoutines.TrimLetters(excelDoc.GetValue(curRow + 2, 1).Trim());
            return PumpRegion(regionCode, regionName);
        }

        private int PumpXlsMarks(ExcelHelper excelDoc, int curRow)
        {
            string name = excelDoc.GetValue(curRow, 1).Trim();
            string cellValue = excelDoc.GetValue(curRow, 2).Trim();
            if (cellValue == string.Empty)
                return -1;
            int code = Convert.ToInt32(cellValue);
            GetSumMultiplier(code);
            int parentId = marksSectionRecordsIDList[sectionIndex];

            object[] mapping = new object[] { "NAME", name, "CODE", code, "PARENTID", parentId };

            if (reportType == ReportType.Svod)
                return PumpCachedRow(cacheMarks, dsMarks.Tables[0], clsMarks, mapping, code.ToString(), "ID");
            return FindCachedRow(cacheMarks, code.ToString(), nullMarks);
        }

        private void PumpXlsRow(ExcelHelper excelDoc, int curRow, int refDate, int refRegions)
        {
            int refMarks = PumpXlsMarks(excelDoc, curRow);
            if (refMarks == -1)
                return;

            for (int index = 0; index < columnsCount[sectionIndex]; index++)
            {
                decimal factValue = CleanFactValue(excelDoc.GetValue(curRow, index + startColumns[sectionIndex]));
                PumpFactRow(factValue, refDate, refMarks, refRegions, index);
            }
        }

        private const string AUX_TABLE_MARK_REGION = "��� �����";
        private bool IsSectionStart(ExcelHelper excelDoc, int curRow)
        {
            string cellValue = excelDoc.GetValue(curRow, 1).Trim().ToUpper();
            if ((sectionIndex != -1) && (cellValue == "�"))
            {
                cellValue = excelDoc.GetValue(curRow + 1, 1).Trim().ToUpper();
                return (cellValue != AUX_TABLE_MARK_REGION);
            }
            return false;
        }

        private const string REGION_TITLE = "������������� �����������";
        private bool IsRegionRow(string cellValue)
        {
            if (reportType != ReportType.Region)
                return false;
            return (cellValue.ToUpper() == REGION_TITLE);
        }

        public const string TOTAL_ROW = "����������� �����";
        private bool IsSectionEnd(string cellValue)
        {
            return cellValue.ToUpper().Contains(TOTAL_ROW);
        }

        private void PumpXlsSheetData(string fileName, ExcelHelper excelDoc, int refDate)
        {
            int refRegions = nullRegions;
            sectionIndex = -1;
            bool toPumpRow = false;
            string dataSourcePath = GetShortSourcePathBySourceID(this.SourceID);
            int rowsCount = excelDoc.GetRowsCount();
            for (int curRow = 1; curRow <= rowsCount; curRow++)
                try
                {
                    SetProgress(rowsCount, curRow,
                        string.Format("��������� ����� {0}\\{1}...", dataSourcePath, fileName),
                        string.Format("������ {0} �� {1}", curRow, rowsCount));

                    string cellValue = excelDoc.GetValue(curRow, 1).Trim();
                    if (cellValue == string.Empty)
                        continue;

                    if (IsRegionRow(cellValue))
                        refRegions = PumpXlsRegions(excelDoc, curRow);

                    if (IsSectionEnd(cellValue))
                    {
                        CheckXlsTotalSum(excelDoc, curRow);
                        toPumpRow = false;
                        continue;
                    }

                    if (toPumpRow)
                    {
                        PumpXlsRow(excelDoc, curRow, refDate, refRegions);
                        continue;
                    }

                    if (IsSectionStart(excelDoc, curRow))
                    {
                        SetNullTotalSum();
                        toPumpRow = true;
                        continue;
                    }

                    if (cellValue.ToUpper().StartsWith("������"))
                        sectionIndex = GetSectionIndex(cellValue.ToUpper());
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format(
                        "��� ��������� ������ {0} ����� '{1}' �������� ������ ({2})",
                        curRow, excelDoc.GetWorksheetName(), ex.Message), ex);
                }
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
            PumpDataYTemplate();
        }

        #endregion ���������� ������ �������

        #endregion ������� ������

        #region ��������� ������

        private const string ROUBLE_UNIT_NAME = "�����";
        private const string UNIT_UNIT_NAME = "�������";
        private const string MAN_UNIT_NAME = "�������";
        private const string THING_UNIT_NAME = "�����";
        private string GetUnitName(int marksCode)
        {
            if (this.DataSource.Year >= 2007)
            {
                switch (marksCode)
                {
                    case 1010:
                        return MAN_UNIT_NAME;
                    case 2010:
                        return THING_UNIT_NAME;
                    case 1030:
                    case 1050:
                    case 1070:
                    case 1090:
                    case 1110:
                    case 1130:
                        return UNIT_UNIT_NAME;
                    case 1020:
                    case 1040:
                    case 1060:
                    case 1080:
                    case 1100:
                    case 1120:
                    case 1140:
                    case 1150:
                    case 1160:
                    case 1170:
                    case 1180:
                    case 1190:
                    case 2020:
                    case 2030:
                    case 2040:
                    case 2050:
                    case 2060:
                    case 2070:
                    case 2080:
                    case 2090:
                        return ROUBLE_UNIT_NAME;
                }
            }
            return string.Empty;
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

        protected override void ProcessDataSource()
        {
            SetRefUnits();
            UpdateData();
        }

        protected override void DirectProcessData()
        {
            year = -1;
            month = -1;
            GetPumpParams(ref year, ref month);
            ProcessDataSourcesTemplate(year, month, "����������� ����������� ������ � �������������� ������ �����������.��� 5 ��ʻ �� ������������� ��������.���Ȼ.");
        }

        #endregion ��������� ������

    }

}
