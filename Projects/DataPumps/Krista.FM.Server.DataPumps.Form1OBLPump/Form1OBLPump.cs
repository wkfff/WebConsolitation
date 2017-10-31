using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

using Krista.FM.Server.DataPumps.Common;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.DataPumps.Form1OBLPump
{
    /// <summary>
    /// ��� 0002_����� 1���
    /// </summary>
    public class Form1OBLPumpModule : TextRepPumpModuleBase
    {
        #region ����

        // ������
        private IDbDataAdapter daRegions;
        private DataSet dsRegions;
        // ����������
        private IDbDataAdapter daMarks;
        private DataSet dsMarks;
        // �����
        private IDbDataAdapter daFNS1OBLRegions;
        private DataSet dsFNS1OBLRegions;
        private IDbDataAdapter daFNS1OBLTotal;
        private DataSet dsFNS1OBLTotal;

        private IClassifier clsRegions;
        private IClassifier clsMarks;
        private IFactTable fctFNS1OBLRegions;
        private IFactTable fctFNS1OBLTotal;

        private int nullMarks;

        #endregion ����


        #region ���������

        // ���������� ������� ��� ��������� � ����
        private const int constMaxQueryRecords = 10000;

        #endregion ���������


        #region �������������

        /// <summary>
        /// �����������
        /// </summary>
        public Form1OBLPumpModule()
            : base()
        {

        }

        #endregion �������������


        #region ������� ������

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
                    if (dt.Rows.Count == 0) continue;

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
                                    string.Format("����� �� ����� {0} �� �������� � ����������� ������ {1}.", totalRecs, sum));
                            }
                            totalSum = 0;

                            continue;
                        }
                        else
                        {
                            totalSum += Convert.ToDouble(dt.Rows[j]["TOTAL"]);
                        }

                        if (sum == 0) continue;

                        int id = PumpOriginalRow(dsMarks, clsMarks, new object[] {
                            "CODE", dt.Rows[j]["STRCODE"], "NAME", dt.Rows[j]["MARKNAME"] });

                        PumpRow(dsFNS1OBLTotal.Tables[0], new object[] { "VALUE", sum, "REFMARKS", id, "RefYearDayUNV", date });

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
                    if (dt.Rows.Count == 0) continue;

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
                        if (Convert.ToString(dt.Rows[j]["MARKNAME"]).Replace(
                            "\\", string.Empty).Trim(' ', '-').ToUpper().Contains("����������� �����"))
                        {
                            break;
                        }

                        double sum = Convert.ToDouble(dt.Rows[j]["TOTAL"]);

                        // ���� �� ����������. �� ���������� ��� ����������� �����
                        if (Convert.ToString(dt.Rows[j]["REGIONNAME"]).ToUpper() == "����� :")
                        {
                            // ���� �� �������� ����� �� ������� � ����, �������� ��������� � ��������
                            if (totalSum != sum)
                            {
                                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, string.Format(
                                    "����� �� ������� {0} �� �������� � �������� ������ {1} ��� ������� {2} ������ {3}.",
                                    totalSum, sum, Convert.ToInt32(dt.Rows[j][this.TableIndexFieldName]) + 1, processedFiles));
                            }
                            totalSum = 0;

                            continue;
                        }
                        else
                        {
                            totalSum += sum;
                        }

                        if (sum == 0) continue;

                        int regionID = PumpOriginalRow(dsRegions, clsRegions, new object[] {
                            "CODE", dt.Rows[j]["STRCODE"], "NAME", dt.Rows[j]["REGIONNAME"] });
                        
                        int markID = FindRowID(dsMarks.Tables[0], new object[] { "CODE", dt.Rows[j]["MARKCODE"] }, nullMarks);
                        
                        if (markID == nullMarks)
                        {
                            markID = PumpRow(dsMarks.Tables[0], clsMarks, new object[] { 
                               "CODE", dt.Rows[j]["MARKCODE"], "NAME", dt.Rows[j]["MARKNAME"] });
                        }

                        PumpRow(dsFNS1OBLRegions.Tables[0], new object[] { 
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
        /// ���������� �����
        /// </summary>
        /// <param name="dir">������� � ������� ��� �������</param>
        protected override void ProcessFiles(DirectoryInfo dir)
        {
            try
            {
                this.CallTXTSorcerer(xmlSettingsForm1OBL_Total, dir.FullName);
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
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, "������� ������ ������� ������� ��������� � ��������", ex);
                throw;
            }

            try
            {
                this.CallTXTSorcerer(xmlSettingsForm1OBL_Regions, dir.FullName);
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

        /// <summary>
        /// ������ ������ �� ����
        /// </summary>
        protected override void QueryData()
        {
            InitClsDataSet(ref daRegions, ref dsRegions, clsRegions, false, string.Empty);
            InitClsDataSet(ref daMarks, ref dsMarks, clsMarks, false, string.Empty);

            InitFactDataSet(ref daFNS1OBLRegions, ref dsFNS1OBLRegions, fctFNS1OBLRegions);
            InitFactDataSet(ref daFNS1OBLTotal, ref dsFNS1OBLTotal, fctFNS1OBLTotal);

            InitNullClsValues();
        }

        /// <summary>
        /// ������ ��������� � ����
        /// </summary>
        protected override void UpdateData()
        {
            UpdateDataSet(daRegions, dsRegions, clsRegions);
            UpdateDataSet(daMarks, dsMarks, clsMarks);
            UpdateDataSet(daFNS1OBLRegions, dsFNS1OBLRegions, fctFNS1OBLRegions);
            UpdateDataSet(daFNS1OBLTotal, dsFNS1OBLTotal, fctFNS1OBLTotal);
        }

        /// <summary>
        /// �������������� ������ ��������������� "����������� ������"
        /// </summary>
        private void InitNullClsValues()
        {
            nullMarks = clsMarks.UpdateFixedRows(this.DB, this.SourceID);
        }

        private const string D_REGIONS_FNS_GUID = "cf3202f9-e897-43ce-a158-5c617bedff55";
        private const string D_MARKS_FNS1_OBL_GUID = "434627b0-2b68-4e49-8797-9a1151f76493";
        private const string F_F_FNS1_OBL_REGIONS_GUID = "3ce3df25-6941-4f82-a6d3-692476bf160a";
        private const string F_F_FNS1_OBL_TOTAL_GUID = "043b6668-9a43-4cc7-b3e5-57c9d38e727a";
        protected override void InitDBObjects()
        {
            this.UsedClassifiers = new IClassifier[] {
                clsRegions = this.Scheme.Classifiers[D_REGIONS_FNS_GUID],
                clsMarks = this.Scheme.Classifiers[D_MARKS_FNS1_OBL_GUID] };

            this.UsedFacts = new IFactTable[] {
                fctFNS1OBLRegions = this.Scheme.FactTables[F_F_FNS1_OBL_REGIONS_GUID],
                fctFNS1OBLTotal = this.Scheme.FactTables[F_F_FNS1_OBL_TOTAL_GUID] };
        }

        /// <summary>
        /// ������� ���������� ����������� �������� ����
        /// </summary>
        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsRegions);
            ClearDataSet(ref dsMarks);
            ClearDataSet(ref dsFNS1OBLRegions);
            ClearDataSet(ref dsFNS1OBLTotal);
        }

        /// <summary>
        /// ������� ������
        /// </summary>
        protected override void DirectPumpData()
        {
            PumpDataYMTemplate();
        }

        #endregion ������� ������
    }
}