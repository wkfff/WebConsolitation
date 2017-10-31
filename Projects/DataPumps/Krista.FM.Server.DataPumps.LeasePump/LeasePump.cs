using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

using Krista.FM.Server.DataPumps.Common;
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.DataPumps.LeasePump
{
    /// <summary>
    /// ���_0004_������
    /// ����� � ����������� ������� � ��������������� ������ �� ������� � ������������� ���������, 
    /// ������������ � ��������������� �������������, ��� �� ������������ ��������������� �����������, 
    /// �������������� ������������� �� (.xls)
    /// </summary>
    public class LeasePumpModule : DataPumpModuleBase
    {
        #region ����

        // ��
        private IDbDataAdapter daKD;
        private DataSet dsKD;
        // �����������.����������
        private IDbDataAdapter daOrgLessees;
        private DataSet dsOrgLessees;
        // �����������.������������
        private IDbDataAdapter daOrgLessors;
        private DataSet dsOrgLessors;
        // �����
        private IDbDataAdapter daIncomesLease;
        private DataSet dsIncomesLease;

        private IClassifier clsKD;
        private IClassifier clsOrgLessees;
        private IClassifier clsOrgLessors;
        private IFactTable fctIncomesLease;

        private int nullKD;
        private int nullOrgLesses;
        private int nullOrgLessors;

        private ExcelHelper excelHelper;
        private int totalFiles;
        private int filesCount;

        private Dictionary<string, int> kdList = null;//new Dictionary<string, int>(1000);

        #endregion ����


        #region ���������

        // ���������� ������� ��� ��������� � ����
        private const int constMaxQueryRecords = 10000;
        // �������� �������� ������, ������ ����� ������
        private const string constExcelSheetName = "����.����.";

        #endregion ���������


        #region �������������

        /// <summary>
        /// �����������
        /// </summary>
        public LeasePumpModule()
            : base()
        {

        }

        /// <summary>
        /// ������������ ��������
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (excelHelper != null) excelHelper.Dispose();
            }

            base.Dispose(disposing);
        }

        #endregion �������������


        #region ���������, ������������

        /// <summary>
        /// ��� ������ ������
        /// </summary>
        private enum ReportRowKind
        {
            /// <summary>
            /// ������ ����������� - ������������
            /// </summary>
            Lessor,

            /// <summary>
            /// ������ ����������� - ������������, ���������� �����
            /// </summary>
            LessorWithSums,

            /// <summary>
            /// ������ �����������-����������
            /// </summary>
            Lessee,

            /// <summary>
            /// ������ "�����"
            /// </summary>
            Total,

            /// <summary>
            /// ����������� �����������
            /// </summary>
            Unknown
        }

        #endregion ���������, ������������


        #region ������� ������

        /// <summary>
        /// ���������� ����������������� ���� ������
        /// </summary>
        /// <param name="sheet">�������� � ������� ������</param>
        /// <returns>����</returns>
        private int GetDate(object sheet)
        {
            int result = Convert.ToInt32(
                CommonRoutines.LongDateToNewDate(excelHelper.GetCell(sheet, "A6").Value));
            return (result / 100) * 100;
        }

        /// <summary>
        /// ���������� ��� ������ ������
        /// </summary>
        /// <param name="sheet">�������� � �������</param>
        /// <param name="rowIndex">����� ������</param>
        /// <param name="rightMargin">������ ������� �������</param>
        /// <returns>��� ������</returns>
        private ReportRowKind GetReportRowKind(object sheet, int rowIndex, int rightMargin)
        {
            ExcelCell cell = excelHelper.GetCell(sheet, rowIndex, 2);

            // ���� ��� ������ ���� ������, �� ��� ������ ������������, ����� - ������ ����������
            for (int i = 3; i < rightMargin; i++)
            {
                if (excelHelper.GetCell(sheet, rowIndex, i).Value != string.Empty)
                {
                    if (cell.Value.ToUpper() == "�����")
                    {
                        return ReportRowKind.Total;
                    }
                    else if (cell.Font.Bold)
                    {
                        return ReportRowKind.LessorWithSums;
                    }
                    else
                    {
                        return ReportRowKind.Lessee;
                    }
                }
            }

            if (!cell.Font.Bold)
            {
                return ReportRowKind.Lessee;
            }

            return ReportRowKind.Lessor;
        }

        /// <summary>
        /// ���������� ������ ������� ������� ������
        /// </summary>
        /// <param name="sheet">�������� � �������</param>
        /// <returns>����� ������ ������ �������</returns>
        private int GetReportBottomMargin(object sheet)
        {
            int i = 13;
            for (; excelHelper.GetCell(sheet, i, 2).Value.ToUpper() != "�����"; i++) { }
            return i;
        }

        /// <summary>
        /// ���������� �����
        /// </summary>
        /// <param name="dir">������� � �������</param>
        protected override void ProcessFiles(DirectoryInfo dir)
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStart, "����� ������������� Excel.");
            excelHelper = new ExcelHelper();
            excelHelper.InitCellFont = true;
            object excelObj = excelHelper.OpenExcel(false);

            try
            {
                FileInfo[] files = dir.GetFiles("*.xls", SearchOption.AllDirectories);
                if (files.GetLength(0) == 0)
                {
                    throw new Exception("����������� ������ ��� �������.");
                }

                for (int i = 0; i < files.GetLength(0); i++)
                {
                    filesCount++;
                    SetProgress(totalFiles, filesCount,
                        string.Format("��������� ����� {0}...", files[i].FullName),
                        string.Format("���� {0} �� {1}", filesCount, totalFiles));

                    WriteEventIntoDataPumpProtocol(
                        DataPumpEventKind.dpeStartFilePumping, string.Format("����� ������� ����� {0}.", files[i].FullName));

                    // ������� ����� � ������� ������
                    int rowsCount = 0;
                    // ������� ���������� �����
                    int pumpedRowsCount = 0;

                    try
                    {
                        // �������� ������ �������� ������
                        object sheet = excelHelper.GetSheet(
                            excelHelper.GetWorkbook(excelObj, files[i].FullName, true), constExcelSheetName);

                        int date = GetDate(sheet);

                        // ����� �� ������ ���������, � ������� ��������� ���� ������ ���������� ��.
                        // ������ ������� � ������ ������� ������� ������
                        int rightMargin = PumpKD(sheet);

                        int parentOrgCode = -1;
                        int childOrgCode = 0;
                        int lessorOrgID = nullOrgLessors;
                        int lesseeOrgID = nullOrgLesses;
                        ReportRowKind rowKind = ReportRowKind.Unknown;
                        int bottomMargin = GetReportBottomMargin(sheet);

                        // ������ ������, ������� � 13 ������
                        for (int j = 13; j < bottomMargin; j++)
                        {
                            if (rowKind == ReportRowKind.LessorWithSums &&
                                excelHelper.GetCell(sheet, j, 2).Value.ToUpper() == "�����")
                            {
                                continue;
                            }

                            ExcelCell orgCode = excelHelper.GetCell(sheet, j, 1);
                            ExcelCell orgName = excelHelper.GetCell(sheet, j, 2);
                            rowKind = GetReportRowKind(sheet, j, rightMargin);

                            switch (rowKind)
                            {
                                // ���������� �����������-������������
                                case ReportRowKind.Lessor:
                                case ReportRowKind.LessorWithSums:
                                    // ��� ���.�� ����������� �� ����� �. ��� �������, � ������� � ����� � ��� ����, 
                                    // ��� ����������� ��������� �������: ������� ��� ������ ����� ����� ����������� �� 
                                    // ���� ���������� ������, ����� - �� �������, � ������� �����������
                                    if (orgCode.Value != string.Empty)
                                    {
                                        parentOrgCode = Convert.ToInt32(orgCode.Value.PadLeft(3, '0').PadRight(5, '0'));
                                        childOrgCode = 0;
                                        lessorOrgID = PumpOriginalRow(dsOrgLessors, clsOrgLessors, 
                                            new object[] { "CODE", parentOrgCode, "NAME", orgName.Value });
                                    }
                                    else
                                    {
                                        childOrgCode++;
                                        lessorOrgID = PumpOriginalRow(dsOrgLessors, clsOrgLessors, 
                                            new object[] { "CODE", parentOrgCode + childOrgCode, "NAME", orgName.Value });
                                    }

                                    // � ������ � ������������� ������������ ���� ���, ��� ��� ��������� � ���������
                                    if (rowKind == ReportRowKind.Lessor)
                                    {
                                        continue;
                                    }

                                    break;

                                // ���������� �����������-����������
                                case ReportRowKind.Lessee:
                                    lesseeOrgID = PumpOriginalRow(dsOrgLessees, clsOrgLessees, 
                                        new object[] { /*"CODE", orgCode.Value,*/ "NAME", orgName.Value });

                                    break;
                            }

                            // ���������� �����
                            for (int k = 5; k < rightMargin; k += 2)
                            {
                                string forMonth = excelHelper.GetCell(sheet, j, k).Value;
                                string fromBeginYear = excelHelper.GetCell(sheet, j, k + 1).Value;

                                if (forMonth.Trim('0', ',') == string.Empty && fromBeginYear.Trim('0', ',') == string.Empty) continue;

                                switch (rowKind)
                                {
                                    case ReportRowKind.Lessee:
                                        PumpRow(dsIncomesLease.Tables[0], new object[] {
                                            "SOURCEKEY", j,
                                            "FORMONTH", forMonth.PadLeft(1, '0'),
                                            "FROMBEGINYEAR", fromBeginYear.PadLeft(1, '0'),
                                            "REFKD", FindRowID(dsKD.Tables[0], 
                                                new object[] { "CODESTR", excelHelper.GetCell(sheet, 9, k).Value }, nullKD ),
                                            "REFORGLESSEES", lesseeOrgID,
                                            "REFORGLESSORS", nullOrgLessors,
                                            "RefYearDayUNV", date });
                                        break;

                                    case ReportRowKind.Total:
                                    case ReportRowKind.LessorWithSums:
                                        PumpRow(dsIncomesLease.Tables[0], new object[] {
                                            "SOURCEKEY", j,
                                            "FORMONTH", forMonth.PadLeft(1, '0'),
                                            "FROMBEGINYEAR", fromBeginYear.PadLeft(1, '0'),
                                            "REFKD", FindRowID(dsKD.Tables[0], 
                                                new object[] { "CODESTR", excelHelper.GetCell(sheet, 9, k).Value }, nullKD ),
                                            "REFORGLESSEES", nullOrgLesses,
                                            "REFORGLESSORS", lessorOrgID,
                                            "RefYearDayUNV", date });
                                        break;
                                }

                                pumpedRowsCount++;
                            }

                            rowsCount++;
                        }

                        WriteEventIntoDataPumpProtocol(
                            DataPumpEventKind.dpeSuccessfullFinishFilePump, string.Format(
                                "������� ����� {0} ���������. ���������� �����: {1}, �������� �����: {2}.",
                                files[i].FullName, rowsCount, pumpedRowsCount));
                    }
                    catch (ThreadAbortException)
                    {
                        WriteEventIntoDataPumpProtocol(
                            DataPumpEventKind.dpeCriticalError, string.Format(
                                "������� ����� {0} �������� �������������. �� ������ ���������� ���������� ��������� ����������. " +
                                "���������� �����: {1}, �������� �����: {2}. ������ �� ���������.",
                                files[i].FullName, rowsCount, pumpedRowsCount));
                        throw;
                    }
                    catch (Exception ex)
                    {
                        WriteEventIntoDataPumpProtocol(
                            DataPumpEventKind.dpeFinishFilePumpWithError, string.Format(
                                "������� ����� {0} ��������� � ��������. " +
                                "�� ������ ������������� ������ ���������� ��������� ����������. " +
                                "���������� �����: {1}, �������� �����: {2}. ������ �� ���������.",
                                files[i].FullName, rowsCount, pumpedRowsCount), ex);
                        throw;
                    }
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                if (excelHelper != null)
                {
                    excelHelper.CloseExcel(ref excelObj);
                    excelHelper.Dispose();
                }
            }
        }

        /// <summary>
        /// ���������� �� �� ��������� ������� ������
        /// </summary>
        /// <param name="sheet">�������� � �������</param>
        /// <returns>������� ������ ������� �������</returns>
        private int PumpKD(object sheet)
        {
            // ��������� ������ ����� ������������ �� �� ��������� "�����"
            string mainName = excelHelper.GetCell(sheet, "E8").Value;

            // ������ ��������� �� ���������� ������� ������ ���� �� � ������� ������������
            // �������� � 5-�� �������
            int i = 5;
            for (; excelHelper.GetCell(sheet, 9, i).Value != string.Empty; i += 2)
            {
                PumpOriginalRow(dsKD, clsKD, new object[] { 
                    "CODESTR", excelHelper.GetCell(sheet, 9, i).Value,
                    "NAME", mainName + " " + excelHelper.GetCell(sheet, 10, i).Value });
            }

            return i;
        }

        /// <summary>
        /// ������ ������ �� ����
        /// </summary>
        protected override void QueryData()
        {
            InitClsDataSet(ref daKD, ref dsKD, clsKD, false, string.Empty);
            InitClsDataSet(ref daOrgLessees, ref dsOrgLessees, clsOrgLessees, false, string.Empty);
            InitClsDataSet(ref daOrgLessors, ref dsOrgLessors, clsOrgLessors, false, string.Empty);

            InitFactDataSet(ref daIncomesLease, ref dsIncomesLease, fctIncomesLease);

            FillRowsCache(ref kdList, dsKD.Tables[0], "CODESTR");
            InitNullClsRows();
        }

        /// <summary>
        /// ������ ��������� � ����
        /// </summary>
        protected override void UpdateData()
        {
            UpdateDataSet(daKD, dsKD, clsKD);
            UpdateDataSet(daOrgLessees, dsOrgLessees, clsOrgLessees);
            UpdateDataSet(daOrgLessors, dsOrgLessors, clsOrgLessors);
            UpdateDataSet(daIncomesLease, dsIncomesLease, fctIncomesLease);
        }

        /// <summary>
        /// �������������� ������ ��������������� "����������� ������"
        /// </summary>
        private void InitNullClsRows()
        {
            nullKD = clsKD.UpdateFixedRows(this.DB, this.SourceID);
            nullOrgLesses = clsOrgLessees.UpdateFixedRows(this.DB, this.SourceID);
            nullOrgLessors = clsOrgLessors.UpdateFixedRows(this.DB, this.SourceID);
        }

        /// <summary>
        /// ������������� �������� ��
        /// </summary>
        private const string D_KD_LEASE_GUID = "5c01b8dd-3086-4568-b925-bfc4e08387f5";
        private const string D_ORGANIZATIONS_LESSEES_GUID = "f8ea4ba0-55ea-4ce0-8dbb-472b1171cb4a";
        private const string D_ORGANIZATIONS_LESSORS_GUID = "035abb61-601d-4a09-a8b6-6e6d3a6099d8";
        private const string F_F_INCOMES_LEASE_GUID = "e4c60fc4-1c11-47f3-a104-e89c015f3be5";
        protected override void InitDBObjects()
        {
            this.UsedClassifiers = new IClassifier[] {
                clsKD = this.Scheme.Classifiers[D_KD_LEASE_GUID],
                clsOrgLessees = this.Scheme.Classifiers[D_ORGANIZATIONS_LESSEES_GUID],
                clsOrgLessors = this.Scheme.Classifiers[D_ORGANIZATIONS_LESSORS_GUID] };

            this.UsedFacts = new IFactTable[] {
                fctIncomesLease = this.Scheme.FactTables[F_F_INCOMES_LEASE_GUID] };
        }

        /// <summary>
        /// ������� ���������� ����������� �������� ����
        /// </summary>
        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsKD);
            ClearDataSet(ref dsOrgLessees);
            ClearDataSet(ref dsOrgLessors);
            ClearDataSet(ref dsIncomesLease);
        }

        /// <summary>
        /// ������� ������
        /// </summary>
        protected override void DirectPumpData()
        {
            totalFiles = this.RootDir.GetFiles("*.xls", SearchOption.AllDirectories).GetLength(0);
            filesCount = 0;

            PumpDataYTemplate();
        }

        #endregion ������� ������
    }
}
