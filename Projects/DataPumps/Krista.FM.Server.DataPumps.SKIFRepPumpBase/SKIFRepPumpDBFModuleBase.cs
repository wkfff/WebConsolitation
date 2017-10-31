using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;

using Krista.FM.Common;
using Krista.FM.Server.Common;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.DataPumps
{
    /// <summary>
    /// ������� ����� ��� ������� ������� ����
    /// </summary>
    public abstract partial class SKIFRepPumpModuleBase : CorrectedPumpModuleBase
    {
        #region ���������, ������������

        /// <summary>
        /// ��� ����� ������ ���
        /// </summary>
        protected enum DBFReportKind
        {
            /// <summary>
            /// ������
            /// </summary>
            Pattern,

            /// <summary>
            /// ����� ������
            /// </summary>
            Region,

            /// <summary>
            /// ����������������� �����
            /// </summary>
            Consolidated,

            /// <summary>
            /// ����������������� ����� ��� �������� � ������
            /// </summary>
            ConsolidatedMF
        }

        #endregion ���������, ������������


        #region ����� ������� DBF

        /// <summary>
        /// ���������� ����� ����� ����� ���
        /// </summary>
        /// <param name="fileName">������������ �����</param>
        /// <returns>����� �����</returns>
        protected int GetFileFormNo(FileInfo file)
        {
            return Convert.ToInt32(file.Name.Substring(file.Name.Length - file.Extension.Length - 2, 2));
        }

        /// <summary>
        /// ��������� ������ �� ������� ����� � ����� ����
        /// </summary>
        /// <param name="row">������</param>
        /// <returns>���� ��� ����� = 0, �� true</returns>
        private bool CheckZeroSums(DataRow row)
        {
            for (int i = 1; i <= 9; i++)
            {
                if (GetDoubleCellValue(row, string.Format("P{0}", i), 0) != 0) return false;
            }
            return true;
        }

        /// <summary>
        /// ��������� ������ ����������� �� ������ ����� �����
        /// </summary>
        /// <param name="kl">������ ����� �����.
        /// ������ ��������� �������: "code" - ���������� ����, ������ ����������;
        /// "code1;code2" - ���������� ��������� ����;
        /// "code1..code2" - ���������� ���� �� ���������� ���������</param>
        /// <param name="fieldName">����, �� �������� �������� �����������</param>
        /// <returns>������ �����������</returns>
        private string GetConstrByKlList(string kl, string fieldName)
        {
            if (kl == string.Empty) return string.Empty;

            string result = string.Empty;
            string[] values = kl.Split(';');

            int count = values.GetLength(0);
            for (int i = 0; i < count; i++)
            {
                string[] intervals = values[i].Split(new string[] { ".." }, StringSplitOptions.None);
                if (intervals.GetLength(0) == 1)
                {
                    result += string.Format(" or (KL = {0})", intervals[0]);
                }
                else
                {
                    result += string.Format(" or (KL >= {0} and KL <= {1})", intervals[0], intervals[1]);
                }
            }

            if (result != string.Empty) result = result.Remove(0, 3).Trim();

            return result;
        }

        /// <summary>
        /// ���������� ��� ������ ���
        /// </summary>
        /// <param name="fileName">������������ �����</param>
        /// <returns>��� ������</returns>
        protected DBFReportKind GetDBFReportKind(string fileName)
        {
            if (fileName.ToUpper().StartsWith("SS")) return DBFReportKind.Pattern;
            else if (fileName.ToUpper().StartsWith("SV")) return DBFReportKind.Consolidated;
            else if (fileName.StartsWith("650") || fileName.StartsWith("651")) return DBFReportKind.ConsolidatedMF;
            else return DBFReportKind.Region;
        }

        /// <summary>
        /// ��������� �������� xml-����� �� ������������ ��������� � �.�.
        /// </summary>
        /// <param name="fileName">�������� �����</param>
        private bool CheckDBFFileName(string fileName)
        {
            string str = fileName.ToUpper();

            if (str.StartsWith("MBUD") ||
                str.StartsWith("MDK") ||
                str.StartsWith("MES") ||
                str.StartsWith("MFORM") ||
                str.StartsWith("MOKRUG") ||
                str.StartsWith("VK")) return false;

            try
            {
                switch (GetDBFReportKind(fileName))
                {
                    case DBFReportKind.Consolidated:
                        switch (this.SkifReportFormat)
                        {
                            case SKIFFormat.MonthReports:
                                // ��� ����������������� ������� ��������� ��� � �����
                                if (Convert.ToInt32(fileName.Substring(2, fileName.Length - 10)) != this.DataSource.Month)
                                {
                                    throw new Exception("����� � �������� ����� �� ������������� ���������.");
                                }
                                if (Convert.ToInt32(fileName.Substring(fileName.Length - 8, 2)) != this.DataSource.Year % 100)
                                {
                                    throw new Exception("��� � �������� ����� �� ������������� ���������.");
                                }
                                break;

                            case SKIFFormat.YearReports:
                                // ��� ����������������� ������� ��������� ��� � �����
                                if (Convert.ToInt32(fileName.Substring(2, fileName.Length - 10)) != this.DataSource.Month)
                                {
                                    throw new Exception("��� � �������� ����� �� ������������� ���������.");
                                }
                                break;
                        }
                        break;

                    case DBFReportKind.ConsolidatedMF:
                        break;

                    case DBFReportKind.Pattern:
                        // ��� ������� ��������� ������ ���
                        if (Convert.ToInt32(fileName.Substring(2, 2)) != this.DataSource.Year % 100)
                        {
                            throw new Exception("��� � �������� ����� �� ������������� ���������.");
                        }
                        break;

                    case DBFReportKind.Region:
                        switch (this.SkifReportFormat)
                        {
                            case SKIFFormat.MonthReports:
                                // ��� ������� ������� ��������� ��� � �����
                                if (Convert.ToInt32(fileName.Substring(0, fileName.Length - 8)) != this.DataSource.Month)
                                {
                                    throw new Exception("����� � �������� ����� �� ������������� ���������.");
                                }
                                if (Convert.ToInt32(fileName.Substring(fileName.Length - 8, 2)) != this.DataSource.Year % 100)
                                {
                                    throw new Exception("��� � �������� ����� �� ������������� ���������.");
                                }
                                break;

                            case SKIFFormat.YearReports:
                                // ��� ������� ������� ��������� ���
                                if (Convert.ToInt32(fileName.Substring(0, fileName.Length - 8)) != this.DataSource.Month)
                                {
                                    throw new Exception("��� � �������� ����� �� ������������� ���������.");
                                }
                                break;
                        }
                        break;
                }

                return true;
            }
            catch (Exception ex)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeError,
                    string.Format("������ ��� �������� �������� ����� {0}", fileName), ex);
                return false;
            }
        }

        /// <summary>
        /// ��������� dbf-�����
        /// </summary>
        /// <param name="filesList">������ ������</param>
        /// <param name="filesRepList">������ ������ �������</param>
        /// <param name="filesPtrnList">������ ��������</param>
        private void LoadDBFFiles(FileInfo[] filesList, out FileInfo[] filesRepList, out FileInfo[] filesPtrnList)
        {
            filesRepList = new FileInfo[0];
            filesPtrnList = new FileInfo[0];

            for (int i = 0; i < filesList.GetLength(0); i++)
            {
                if (!File.Exists(filesList[i].FullName)) continue;
                if (!CheckDBFFileName(filesList[i].Name))
                {
                    dbfFilesCount--;
                    continue;
                }

                switch (GetDBFReportKind(filesList[i].Name))
                {
                    case DBFReportKind.Pattern:
                        filesPtrnList =
                            (FileInfo[])CommonRoutines.RedimArray(filesPtrnList, filesPtrnList.GetLength(0) + 1);
                        filesPtrnList[filesPtrnList.GetLength(0) - 1] = filesList[i];
                        dbfFilesCount--;
                        break;

                    case DBFReportKind.Consolidated:
                    case DBFReportKind.Region:
                        // ��������� ������ ������ �������
                        filesRepList =
                            (FileInfo[])CommonRoutines.RedimArray(filesRepList, filesRepList.GetLength(0) + 1);
                        filesRepList[filesRepList.GetLength(0) - 1] = filesList[i];
                        break;
                }
            }
        }

        #endregion ����� ������� DBF


        #region ������� ������� �������

        /// <summary>
        /// ���������� ��� ��� ��� ������ �� ���� ���������������
        /// </summary>
        /// <param name="code">�������� ���</param>
        /// <param name="kl">��� �����</param>
        /// <param name="kst">��� ������</param>
        /// <returns>��� ��� ������</returns>
        protected string GetCacheCode(string code, int kl, int kst)
        {
            return code;// + Convert.ToString(kl).PadLeft(10, '0');// + Convert.ToString(kst).PadLeft(10, '0');
        }

        /// <summary>
        /// ���������� ������������� �� �������
        /// </summary>
        /// <param name="fileFormNo">����� ����� �� �������� �����</param>
        /// <param name="kl">���� KL</param>
        /// <param name="kst">���� KST</param>
        /// <param name="kbk">���� KBK</param>
        /// <param name="n2">���� N2</param>
        /// <returns>�� ��������������</returns>
        protected virtual int PumpClsFromPattern(int fileFormNo, int kl, int kst, string kbk, string n2)
        {
            return -1;
        }

        /// <summary>
        /// ���������� ������� ���
        /// </summary>
        /// <param name="filesPtrnList">������ ������ ��������</param>
        private void PumpDBFPattern(FileInfo[] filesPtrnList)
        {
            if (filesPtrnList.GetLength(0) == 0)
            {
                throw new PumpDataFailedException("����������� ������.");
            }

            for (int i = 0; i < filesPtrnList.GetLength(0); i++)
            {
                if (!CommonRoutines.CheckValueEntry(GetFileFormNo(filesPtrnList[i]), GetAllFormNo())) continue;

                WriteToTrace(string.Format("����� ��������� ������� {0}.", filesPtrnList[i].Name), TraceMessageKind.Information);

                IDbDataAdapter da = null;
                DataSet ds = new DataSet();
                InitDataSet(this.DbfDB, ref da, ref ds, string.Format("SELECT * FROM {0}", filesPtrnList[i].Name));

                // ���������� ����� �������
                for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
                {
                    int kl = GetIntCellValue(ds.Tables[0].Rows[j], "KL", 0);
                    int kst = GetIntCellValue(ds.Tables[0].Rows[j], "KST", 0);
                    string kbk = GetStringCellValue(ds.Tables[0].Rows[j], "KBK", "0");
                    string n2 = GetStringCellValue(ds.Tables[0].Rows[j], "N2", constDefaultClsName).Trim();

                    PumpClsFromPattern(GetFileFormNo(filesPtrnList[i]), kl, kst, kbk, n2);

                    SetProgress(ds.Tables[0].Rows.Count, j + 1,
                        string.Format("��������� ������� {0}...", filesPtrnList[i].Name),
                        string.Format("������ {0} �� {1}", j + 1, ds.Tables[0].Rows.Count));
                }

                WriteToTrace(string.Format("������ {0} ���������.", filesPtrnList[i].Name), TraceMessageKind.Information);
            }

            UpdateData();
        }

        #endregion ������� ������� �������


        #region ������� ������� ������ �����

        /// <summary>
        /// ���� ��� ��������������. ���� �� ������ - ���������� � name = ???
        /// </summary>
        /// <param name="codesMapping">������ ��� ���_�_��� - ��_������_�_�������� ��� ������� ��������������</param>
        /// <param name="clsTable">������� ��������������</param>
        /// <param name="cls">IClassifier</param>
        /// <param name="fileFormNo">����� ����� �����</param>
        /// <param name="kl">���� KL</param>
        /// <param name="kst">���� KST</param>
        /// <param name="kbk">���� KBK</param>
        /// <param name="nullRefToCls">������ �� ������� ������ �� �������������� "����������� ������"</param>
        /// <param name="clsValuesMapping">������ �������� ������ �� ��������������</param>
        /// <param name="isMark">true - ������������ ������������� ����������. ��� ��� ������ ���� ��������������
        /// ����� ������������� ������������� KL � KBK</param>
        /// <returns>�� ��������������</returns>
        private void FindClsID(Dictionary<string, int>[] codesMapping, DataTable[] clsTable, IClassifier[] cls,
            int fileFormNo, int kl, int kst, string kbk, int[] nullRefToCls, object[] clsValuesMapping, bool isMark)
        {
            string code = kbk;
            if (isMark) code = kbk + kst.ToString();

            // ���� ���
            if (codesMapping != null)
            {
                int count = clsValuesMapping.GetLength(0);
                for (int i = 0; i < count; i += 2)
                {
                    int id = FindCachedRow(codesMapping[i / 2], code, nullRefToCls[i / 2]);
                    // ���� ��� �� ������, ��������� �������������
                    int patternClsID = -1;
                    if (id == nullRefToCls[i / 2])
                    {
                        patternClsID = PumpClsFromPattern(fileFormNo, kl, kst, kbk, constDefaultClsName);
                        if (patternClsID == -1)
                            patternClsID = nullRefToCls[i / 2];
                        clsValuesMapping[i + 1] = patternClsID;
                    }
                    else
                    {
                        clsValuesMapping[i + 1] = id;
                    }
                }
            }
            else
            {
                int count = clsValuesMapping.GetLength(0);
                for (int i = 0; i < count; i += 2)
                {
                    clsValuesMapping[i + 1] = FindRowID(clsTable[i / 2],
                        new object[] { GetClsCodeField(cls[i / 2]), code }, nullRefToCls[i / 2]);
                }
            }
        }

        /// <summary>
        /// ���� ��� ��������������. ���� �� ������ - ���������� � name = ???
        /// </summary>
        /// <param name="codesMapping">������ ��� ���_�_��� - ��_������_�_�������� ��� ������� ��������������</param>
        /// <param name="clsTable">������� ��������������</param>
        /// <param name="cls">IClassifier</param>
        /// <param name="fileFormNo">����� ����� �����</param>
        /// <param name="kl">���� KL</param>
        /// <param name="kst">���� KST</param>
        /// <param name="kbk">���� KBK</param>
        /// <param name="clsCode">��� ��������������. ���� ��������� ������������ ��� ���������� ������.</param>
        /// <param name="nullRefToCls">������ �� ������� ������ �� �������������� "����������� ������"</param>
        /// <param name="clsValuesMapping">������ �������� ������ �� ��������������</param>
        /// <param name="isMark">true - ������������ ������������� ����������. ��� ��� ������ ���� ��������������
        /// ����� ������������� ������������� KL � KBK</param>
        /// <returns>�� ��������������</returns>
        private int FindClsID(Dictionary<string, int> codesMapping, DataTable clsTable, IClassifier cls,
            int fileFormNo, int kl, int kst, string kbk, string clsCode, int nullRefToCls, object[] clsValuesMapping, 
            bool isMark)
        {
            int clsID = nullRefToCls;
            string code = kbk;
            string kbkEx = GetCacheCode(kbk, kl, kst);

            if (isMark)
            {
                code = kbk + kst.ToString();
            }

            // ���� ���
            if (codesMapping != null)
            {
                clsID = FindCachedRow(codesMapping, kbkEx, nullRefToCls);
            }
            else
            {
                clsID = FindRowID(clsTable, new object[] { GetClsCodeField(cls), code }, nullRefToCls);
            }

            // ���� ��� �� ������, ��������� �������������
            if (clsID == nullRefToCls)
            {
                int patternClsID = -1;
                if (clsCode == string.Empty)
                {
                    patternClsID = PumpClsFromPattern(fileFormNo, kl, kst, kbk, constDefaultClsName);
                    if (patternClsID != -1)
                        clsID = patternClsID;
                }
                else
                {
                    clsID = PumpCachedRow(codesMapping, clsTable, cls, kbkEx,
                        new object[] { GetClsCodeField(cls), clsCode, "NAME", constDefaultClsName, "KL", kl, "KST", kst });
                }
            }

            return clsID;
        }

        /// <summary>
        /// ��������� ������ ������ �� ��������������
        /// </summary>
        /// <param name="fileFormNo">����� ����� �����</param>
        /// <param name="clsTable">������� ���������������</param>
        /// <param name="cls">������� ���������������, ��������������� clsTable</param>
        /// <param name="nullRefsToCls">������ �� ������� ������ �� �������������� "����������� ������"</param>
        /// <param name="codesMapping">������ ��� ���_�_��� - ��_������_�_�������� ��� ������� ��������������</param>
        /// <param name="isMark">true - ������������ ������������� ����������. ��� ��� ������ ���� ��������������
        /// ����� ������������� ������������� KL � KBK</param>
        /// <param name="clsValuesMapping">������ �������� ���������������</param>
        /// <param name="row">������ ���-�����</param>
        /// <param name="blockProcessModifier">����������� ��������� �����. ����� ��� ���������� �������������� 
        /// �������� ��� ������-���� �� ������.</param>
        /// <param name="kbk2ClsMapping"> ������ ����������_�������� ������������� ���� ��������������, ������ ����� ������� 
        /// �������� ��� ��������� ���� ���������������.
        /// ������ ����������_��������:
        /// "num" - ����� ������� (-1 - ��� �������);
        /// "num:mask" - mask ���������� ���������� ��������, �� �������� ����� ��������� ������ ������ ���������� ��������;
        /// "num1;num2;..." - ������� num1, num2...;
        /// "num1..num2" - �������� num1..num2 (�������� ������������� � �.2; 0..num - ������ num ��������,
        /// -1..num - ��������� num ��������)</param>
        private bool GetClsValuesMapping(int fileFormNo, DataTable[] clsTable, IClassifier[] cls, int[] nullRefsToCls, 
            Dictionary<string, int>[] codesMapping, bool isMark, int yearIndex, object[] clsValuesMapping, 
            DataRow row, BlockProcessModifier blockProcessModifier, string[] kbk2ClsMapping)
        {
            string kbk = Convert.ToString(row["KBK"]).Trim();
            if (kbk.StartsWith("LIST")) return false;
            
            int kl = Convert.ToInt32(row["KL"]);
            int kst = Convert.ToInt32(row["KST"]);
            string kbkEx = GetCacheCode(kbk, kl, kst);

            try
            {
                // �������� �� ��������������
                switch (blockProcessModifier)
                {
                    case BlockProcessModifier.MROutcomesBooksEx:
                        // ��� �������������� ������ �������������� �� ���� ��������
                        return CheckClsIDByCode(clsValuesMapping, codesMapping, kbkEx);

                    case BlockProcessModifier.YROutcomes:
                    case BlockProcessModifier.MROutcomesBooks:
                        // ��������� ������ �� �������������� �� ����
                        string[] codeValues = GetCodeValuesAsSubstring(kbk2ClsMapping, kbk, "0");

                        int count = codeValues.GetLength(0);
                        for (int i = 0; i < count; i++)
                        {
                            if (clsTable[i] == null) continue;

                            if (blockProcessModifier == BlockProcessModifier.MROutcomesBooks)
                            {
                                clsValuesMapping[i * 2 + 1] = PumpCachedRow(codesMapping[i], clsTable[i], cls[i],
                                    codeValues[i], "CODE", new string[] { "NAME", constDefaultClsName, "KL", kl.ToString(), "KST", kst.ToString() });
                            }
                            else
                            {
                                clsValuesMapping[i * 2 + 1] = FindClsID(
                                    codesMapping[i], clsTable[i], cls[i], fileFormNo, kl, kst, codeValues[i],
                                    codeValues[i], nullRefsToCls[i], clsValuesMapping, false);
                            }
                        }

                        if (blockProcessModifier == BlockProcessModifier.MROutcomesBooks)
                        {
                            return CheckClsIDByCode(codesMapping, codeValues);
                        }

                        break;

                    case BlockProcessModifier.YREmbezzles:
                        break;

                    default:
                        if (yearIndex >= 0)
                        {
                            if (codesMapping == null)
                            {
                                if (clsValuesMapping.GetLength(0) != 0)
                                    clsValuesMapping[yearIndex * 2 + 1] = FindClsID(null, clsTable[yearIndex], cls[yearIndex], fileFormNo,
                                        kl, kst, kbk, string.Empty, nullRefsToCls[yearIndex], clsValuesMapping, isMark);
                            }
                            else
                            {
                                clsValuesMapping[yearIndex * 2 + 1] =
                                    FindClsID(codesMapping[0], clsTable[yearIndex], cls[yearIndex], fileFormNo,
                                        kl, kst, kbk, string.Empty, nullRefsToCls[yearIndex], clsValuesMapping, isMark);
                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("������ ��� ������������ ����� ������������� �� ��� {0}", kbk), ex);
            }

            return true;
        }

        /// <summary>
        /// ������� ������ �������� ������ �� �������������� � ����������� �� �����
        /// </summary>
        /// <param name="formNo">����� �����</param>
        /// <param name="blockProcessModifier">����������� ��������� �����. ����� ��� ���������� �������������� 
        /// �������� ��� ������-���� �� ������.</param>
        /// <returns>������ ��������</returns>
        private object[] PrepareIndividualCodesMappingDBF(int formNo, BlockProcessModifier blockProcessModifier)
        {
            object[] result = new object[0];

            switch (blockProcessModifier)
            {
                case BlockProcessModifier.MRStandard:
                case BlockProcessModifier.MRDefProf:
                case BlockProcessModifier.MRIncomes:
                case BlockProcessModifier.MROutcomes:
                case BlockProcessModifier.MRSrcInFin:
                case BlockProcessModifier.MRSrcOutFin:
                case BlockProcessModifier.YRDefProf:
                case BlockProcessModifier.YROutcomes:
                case BlockProcessModifier.YRIncomes:
                case BlockProcessModifier.YRSrcFin:
                    Array.Resize(ref result, 2);
                    result[0] = "REFMEANSTYPE";
                    result[1] = 1;
                    break;
            }

            return result;
        }

        /// <summary>
        /// ����������� �������� ������ ������
        /// </summary>
        /// <param name="row">������</param>
        /// <param name="field">�������� ����. ����� ��������� �������� ����� ; - ����� ����� ������� �������� ���� ����, ��� ���� ��������;
        /// ������ ������ ������������ �������� �������� ����.</param>
        /// <param name="defaultValue">�������� �� ���������</param>
        /// <returns>�������� ����</returns>
        private double GetDBFRowCellValue(DataRow row, string field, double defaultValue)
        {
            if (field == string.Empty) return defaultValue;

            string[] fieldArray = field.Split(';');
            int count = fieldArray.GetLength(0);
            for (int i = 0; i < count; i++)
            {
                double sum = GetDoubleCellValue(row, fieldArray[i], 0);
                if (sum != 0)
                {
                    return sum;
                }
            }

            return defaultValue;
        }

        /// <summary>
        /// ���������� ������ ������ �������
        /// </summary>
        /// <param name="factTable">������� ������</param>
        /// <param name="fct">IFactTable</param>
        /// <param name="date">���� ���������</param>
        /// <param name="row">������ ���</param>
        /// <param name="fieldValuesMapping">������ ��� ���� ���_����_�����-���_����_���.
        /// ���_����_��� ����� ��������� �������� ����� ; - ����� ����� ������� �������� ���� ����, ��� ���� ��������;
        /// ������ ������ ������������ �������� �������� ����.</param>
        /// <param name="clsValuesMapping">������ �������� ���������������</param>
        /// <param name="regionID">�� ������</param>
        /// <param name="budgetLevel">������� �������</param>
        private void PumpDBFRow(DataTable factTable, IFactTable fct, string date, DataRow row,
            string[] fieldValuesMapping, object[] clsValuesMapping, int regionID, int budgetLevel)
        {
            object[] fieldsMapping = new object[fieldValuesMapping.GetLength(0)];

            bool zeroSums = true;
            int count = fieldValuesMapping.GetLength(0);
            for (int i = 0; i < count; i += 2)
            {
                fieldsMapping[i] = fieldValuesMapping[i];
                double sum = GetDBFRowCellValue(row, fieldValuesMapping[i + 1], 0) * this.SumFactor;
                fieldsMapping[i + 1] = sum;

                if (sum != 0)
                {
                    zeroSums = false;
                }
            }

            if (!zeroSums)
            {
                switch (this.SkifReportFormat)
                {
                    case SKIFFormat.MonthReports:
                        PumpRow(factTable,
                            (object[])CommonRoutines.ConcatArrays(fieldsMapping, clsValuesMapping, new object[] { 
                                "RefYearDayUNV", date, "REFBDGTLEVELS", budgetLevel, "REFREGIONS", regionID }));
                        break;

                    case SKIFFormat.YearReports:
                        PumpRow(factTable,
                            (object[])CommonRoutines.ConcatArrays(fieldsMapping, clsValuesMapping, new object[] { 
                                "REFYEAR", date, "REFBDGTLEVELS", budgetLevel, "REFREGIONS", regionID }));
                        break;
                }
            }
        }

        /// <summary>
        /// ���������� ������ ������ �������
        /// </summary>
        /// <param name="fileFormNo">����� ����� �����</param>
        /// <param name="factTable">������� ������</param>
        /// <param name="fct">IFactTable</param>
        /// <param name="date">���� ���������</param>
        /// <param name="row">������ ���</param>
        /// <param name="clsValuesMapping">������ �������� ���������������</param>
        private void ProcessDBFRow(int fileFormNo, DBFReportKind repKind, DataTable factTable, IFactTable fct, 
            string date, DataRow row, object[] clsValuesMapping, 
            Dictionary<string, int> regionCache, int nullRegions, Dictionary<string, int> regions4PumpSkifCache)
        {
            int kvb = GetIntCellValue(row, "KVB", 0);
            int org = GetIntCellValue(row, "ORG", 0);
            int kst = GetIntCellValue(row, "KST", 0);
            int budgetLevel = 3;
            int regionID = -1;

            switch (repKind)
            {
                case DBFReportKind.Region:
                    string code = GetDBFRegionCode(kvb.ToString(), org.ToString());
                    string name = string.Empty;
                    foreach (KeyValuePair<string, int> item in regionCache)
                        if (item.Key.ToString().Split('|')[0] == code)
                        {
                            name = item.Key.ToString().Split('|')[1];
                            break;
                        }
                    string regKey = code + "|" + name;
                    if (regions4PumpSkifCache != null)
                    {
                        if (!regions4PumpSkifCache.ContainsKey(regKey))
                            return;
                        switch (regions4PumpSkifCache[regKey])
                        {
                            case 2:
                            case 4:
                            case 5:
                            case 6:
                            case 7:
                            case 8:
                                regionID = FindCachedRow(regionCache, regKey, nullRegions);
                                if ((kvb == 1 || kvb == 2) && org == 900)
                                    budgetLevel = 3;
                                else
                                    budgetLevel = 7;
                                break;
                            case 3:
                                regionID = FindCachedRow(regionCache, regKey, nullRegions);
                                break;
                            default:
                                return;
                        }
                    }
                    break;
                default:
                    regionID = FindCachedRow(regionCache, "0000000001|����������������� ����� ��������", nullRegions);
                    budgetLevel = 2;
                    break;
            }

            switch (this.SkifReportFormat)
            {
                case SKIFFormat.MonthReports:
                    switch (fileFormNo)
                    {
                        case 50:
                            PumpDBFRow(factTable, fct, date, row, new string[] {
                                "YEARPLANREPORT", "P1", "QUARTERPLANREPORT", "P4",
                                "MONTHPLANREPORT", string.Empty, "FACTREPORT", "P7" },
                                clsValuesMapping, regionID, 2);

                            PumpDBFRow(factTable, fct, date, row, new string[] {
                                "YEARPLANREPORT", "P2", "QUARTERPLANREPORT", "P5",
                                "MONTHPLANREPORT", string.Empty, "FACTREPORT", "P8" },
                                clsValuesMapping, regionID, 3);

                            PumpDBFRow(factTable, fct, date, row, new string[] {
                                "YEARPLANREPORT", "P3", "QUARTERPLANREPORT", "P6",
                                "MONTHPLANREPORT", string.Empty, "FACTREPORT", "P9" },
                                clsValuesMapping, regionID, 7);
                            break;

                        case 51:
                            PumpDBFRow(factTable, fct, date, row, new string[] {
                                "YEARPLANREPORT", string.Empty, "QUARTERPLANREPORT", string.Empty,
                                "MONTHPLANREPORT", string.Empty, "FACTREPORT", "P1" },
                                clsValuesMapping, regionID, 2);

                            PumpDBFRow(factTable, fct, date, row, new string[] {
                                "YEARPLANREPORT", string.Empty, "QUARTERPLANREPORT", string.Empty,
                                "MONTHPLANREPORT", string.Empty, "FACTREPORT", "P2" },
                                clsValuesMapping, regionID, 3);

                            PumpDBFRow(factTable, fct, date, row, new string[] {
                                "YEARPLANREPORT", string.Empty, "QUARTERPLANREPORT", string.Empty,
                                "MONTHPLANREPORT", string.Empty, "FACTREPORT", "P3" },
                                clsValuesMapping, regionID, 7);
                            break;
                    }
                    break;

                case SKIFFormat.YearReports:
                    switch (fileFormNo)
                    {
                        case 2:
                            PumpDBFRow(factTable, fct, date, row, new string[] {
                                "ASSIGNEDREPORT", string.Empty, "PERFORMEDREPORT", "P2" }, clsValuesMapping, regionID, 2);

                            PumpDBFRow(factTable, fct, date, row, new string[] {
                                "ASSIGNEDREPORT", string.Empty, "PERFORMEDREPORT", "P3" }, clsValuesMapping, regionID, 3);

                            PumpDBFRow(factTable, fct, date, row, new string[] {
                                "ASSIGNEDREPORT", string.Empty, "PERFORMEDREPORT", "P4" }, clsValuesMapping, regionID, 7);

                            PumpDBFRow(factTable, fct, date, row, new string[] {
                                "ASSIGNEDREPORT", "P1", "PERFORMEDREPORT", string.Empty }, clsValuesMapping, regionID, budgetLevel);
                            break;

                        case 3:
                            PumpDBFRow(factTable, fct, date, row, new string[] {
                                "FACTBEGINYEAR", "P1", "FACTENDYEAR", "P2", "BUDGETMIDYEAR", "P4", "FACTMIDYEAR", "P3" },
                                clsValuesMapping, regionID, budgetLevel);
                            break;

                        case 8:
                            for (int i = 1; i <= 9; i++)
                            {
                                PumpDBFRow(factTable, fct, date, row, new string[] {
                                    "FACT", string.Format("P{0}", i) }, (object[])CommonRoutines.ConcatArrays(
                                        clsValuesMapping, new object[] { "REFMARKS", kst, "REFMEANSTYPE", i }), 
                                    regionID, budgetLevel);
                            }
                            break;
                    }
                    break;
            }
        }

        /// <summary>
        /// ��������� ��� ������ ������� ��� �� KVB � ORG
        /// </summary>
        /// <param name="kvb">KVB</param>
        /// <param name="org">ORG</param>
        /// <returns>��� ������</returns>
        private string GetDBFRegionCode(string kvb, string org)
        {
            return kvb.PadLeft(5, '0') + org.PadLeft(5, '0');
        }

        /// <summary>
        /// ��������� ��� ������ ������� ��� �� KVB � ORG
        /// </summary>
        /// <param name="kvb">KVB</param>
        /// <param name="org">ORG</param>
        /// <returns>��� ������</returns>
        private string GetDBFRegionCode(int kvb, int org)
        {
            return GetDBFRegionCode(kvb.ToString(), org.ToString());
        }

        private bool PumpRegionForPump(string code, string key, string name, DataTable regions4PumpTable, 
            IClassifier regions4PumpCls, Dictionary<string, int> regions4PumpCache)
        {
            if (regions4PumpCache == null)
                return true;
            if (!regions4PumpCache.ContainsKey(key))
            {
                PumpCachedRow(regions4PumpCache, regions4PumpTable, regions4PumpCls, code, key, "CODESTR", "REFDOCTYPE",
                    new object[] { "NAME", name, "REFDOCTYPE", 1, "SOURCEID", GetRegions4PumpSourceID() });
                return false;
            }
            return true;
        }

        /// <summary>
        /// ���������� ����� �� MOKRUG.DBF
        /// </summary>
        protected void PumpRegionsDBF(DataTable dt, IClassifier cls, Dictionary<string, int> regionCache, int nullRegions,
            DataTable regions4PumpTable, IClassifier regions4PumpCls, Dictionary<string, int> regions4PumpCache)
        {
            ReconnectToDbfDataSource(ODBCDriverName.Microsoft_dBase_Driver);

            IDbDataAdapter da = null;
            DataSet ds = new DataSet();
            InitDataSet(this.DbfDB, ref da, ref ds,
                "select mo.KVB, mo.ORG, mo.N2 as MO_N2, mb.N2 as MB_N2 " +
                "from MOKRUG mo left join MBUD mb on (mo.KVB = mb.KVB)");
            bool noRegForPump = false;
            string code; string name; string regKey;
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                // ��������� �������
                string kvb = Convert.ToString(ds.Tables[0].Rows[i]["KVB"]);
                string org = Convert.ToString(ds.Tables[0].Rows[i]["ORG"]);
                code = GetDBFRegionCode(kvb, org);
                name = GetStringCellValue(ds.Tables[0].Rows[i], "MO_N2", "����������� �����");
                regKey = code + "|" + name;
                if ((regions4PumpCache != null) && (regions4PumpCache.ContainsKey(regKey)))
                {
                    switch (regions4PumpCache[regKey])
                    {
                        case 2:
                        case 4:
                        case 5:
                        case 6:
                        case 7:
                        case 8:
                        case 3:
                            PumpCachedRow(regionCache, dt, cls, regKey, new object[] { 
                                "CODESTR", code, "NAME", name, "BUDGETKIND", kvb, "BUDGETNAME", ds.Tables[0].Rows[i]["MB_N2"] });
                            break;
                    }
                }
                if (!PumpRegionForPump(code, regKey, name, regions4PumpTable, regions4PumpCls, regions4PumpCache))
                    noRegForPump = true;
            }
            // ����������������� 
            code = "0000000001";
            name = "����������������� ����� ��������";
            regKey = code + "|" + name;
            PumpCachedRow(regionCache, dt, cls, regKey, 
                new object[] { "CODESTR", code, "NAME", name, "BUDGETKIND", "�", "BUDGETNAME", constDefaultClsName });
            if (!PumpRegionForPump(code, regKey, name, regions4PumpTable, regions4PumpCls, regions4PumpCache))
                noRegForPump = true;
            if (noRegForPump)
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, string.Format(
                    "������������� ������.��������� (SOURCEID {0}) ����� ������ � ����������� ����� ������. " +
                    "���������� ���������� �������� ���� \"������������.����\" � ��������� ���� ���������.", GetRegions4PumpSourceID()));
            ReconnectToDbfDataSource(ODBCDriverName.Microsoft_dBase_Driver);
        }

        /// <summary>
        /// ���������� ������ ������
        /// </summary>
        /// <param name="blockName">�������� ����� (��� ���������)</param>
        /// <param name="dbfTable">������� ���</param>
        /// <param name="file">���� ������</param>
        /// <param name="fileFormNo">����� ����� �����</param>
        /// <param name="kl">������ ����� �����.
        /// ������ ��������� �������: "code" - ���������� ����, ������ ����������;
        /// "code1;code2" - ���������� ��������� ����;
        /// "code1..code2" - ���������� ���� �� ���������� ���������</param>
        /// <param name="da">����������� ������� ������</param>
        /// <param name="factTable">������� ������</param>
        /// <param name="fct">IFactTable</param>
        /// <param name="clsTable">������� ���������������</param>
        /// <param name="cls">������� ���������������, ��������������� clsTable</param>
        /// <param name="clsYears">����, ������������ � ������ ������ ���� � ����� ������������� �� clsTable ������</param>
        /// <param name="factRefsToCls">������ �� ������� ������ �� �������������� clsTable</param>
        /// <param name="nullRefsToCls">������ �� ������� ������ �� �������������� "����������� ������"</param>
        /// <param name="codesMapping">������ ��� ���_�_��� - ��_������_�_�������� ��� ������� ��������������</param>
        /// <param name="codeExclusions">���������� ���� �������������� (��������� ���� �� ������������).
        /// ������ ��� Rule - CodePart, ��� CodePart - ����� �������� ����, � �������� ����� ��������� ������� 
        /// ���������� Rule. ���� ������������ ������ Rule, �.�. ������ �������� 1 �������, �� ��� ������� �����
        /// ��������� �� ����� ����. ���� ��� ������������� ���� �� ������ �� ������, �� �� ����� ��������.
        /// ������ ��������� �������: 
        /// "rule1;rule2" - ��������� ������, ����������� ����������, ����� ���� ����������� ����� ����� � �������;
        /// ������ ����� ��. � PumpComplexClsFromInternalPatternXML.AttrValuesMapping.
        /// ������ ������:
        /// "code" - ����������� ����, ������ ����������;
        /// "code*" - ����������� ����, ������������ � ����������;
        /// "*code" - ����������� ����, ��������������� �� ���������;
        /// "code1..code2" - ����������� ����, �������� � �������� code1..code2;
        /// ">=code" - ����������� ���� >= code;
        /// "code" - ����������� ����, ������� ��� ������ code (����� code ���� ������-����� ����� �������, �
        /// ����� ����������� �� ��������� :).
        /// �������� ������:
        /// "!" - ���������; "#" - ������������� ������� (���� ��� ��� �� �������������, �� �� �� ����� �������� 
        /// ��� ����������� �� ������ ������; ��� ������� ������ ���� ������)</param>
        /// <param name="isMark">true - ������������ ������������� ����������. ��� ��� ������ ���� ��������������
        /// ����� ������������� ������������� KL � KBK</param>
        /// <param name="multipleCls">true - ������� ������ ����� ��������� ������ �� ��������������, 
        /// �������������� �� ������� �� ����.</param>
        /// <param name="sumFieldForCorrect">������ ����� ���� ��� ��������� � ������� ������</param>
        /// <param name="blockProcessModifier">����������� ��������� �����. ����� ��� ���������� �������������� 
        /// �������� ��� ������-���� �� ������.</param>
        /// <param name="hierarchyMapping">��������� ������������ �� ����������� � ������������ ������� 
        /// ��� ������� ��������������</param>
        /// <param name="kbk2ClsMapping"> ������ ����������_�������� ������������� ���� ��������������, ������ ����� ������� 
        /// �������� ��� ��������� ���� ���������������.
        /// ������ ����������_��������:
        /// "num" - ����� ������� (-1 - ��� �������);
        /// "num:mask" - mask ���������� ���������� ��������, �� �������� ����� ��������� ������ ������ ���������� ��������;
        /// "num1;num2;..." - ������� num1, num2...;
        /// "num1..num2" - �������� num1..num2 (�������� ������������� � �.2; 0..num - ������ num ��������,
        /// -1..num - ��������� num ��������)</param>
        protected void PumpDBFReportData(string blockName, DataTable dbfTable, FileInfo file, int fileFormNo,
            string kl, IDbDataAdapter da, DataTable factTable, IFactTable fct, 
            DataTable[] clsTable, IClassifier[] cls, int[] clsYears, string[] factRefsToCls, int[] nullRefsToCls, string progressMsg,
            Dictionary<string, int>[] codesMapping, string[] codeExclusions, bool isMark, BlockProcessModifier blockProcessModifier,
            Dictionary<string, int> regionCache, int nullRegions, string[] kbk2ClsMapping, Dictionary<string, int> regions4PumpSkifCache)
        {
            if (fileFormNo != GetFileFormNo(file) || clsTable.GetLength(0) != cls.GetLength(0)) return;

            string date = string.Empty;
            switch (this.SkifReportFormat)
            {
                case SKIFFormat.MonthReports:
                    date = string.Format("{0}{1:00}00", this.DataSource.Year, this.DataSource.Month);
                    break;

                case SKIFFormat.YearReports:
                    date = this.DataSource.Year.ToString();
                    break;
            }

            // ���������� ������ ���� �������� ��������� � ������� ���� ���.
            // ����� ��� ����, ����� ����������, ��� ����� �� ������ ��������.
            int yearIndex = GetYearIndexByYear(clsYears);

            // ��������� ������ �������� ������ �� ��������������
            object[] clsValuesMapping = new object[factRefsToCls.GetLength(0) * 2];
            int count = factRefsToCls.GetLength(0);
            for (int i = 0; i < count; i++)
            {
                clsValuesMapping[i * 2] = factRefsToCls[i];
                clsValuesMapping[i * 2 + 1] = nullRefsToCls[i];
            }

            DataRow[] rows = dbfTable.Select(GetConstrByKlList(kl, "KL"));
            int rowsCount = rows.GetLength(0);

            DBFReportKind repKind = GetDBFReportKind(file.Name);

            object[] individualCodesMapping =
                PrepareIndividualCodesMappingDBF(fileFormNo, blockProcessModifier);

            // ���������� ������ � ����������� �� ����� ������
            for (int i = 0; i < rowsCount; i++)
            {
                SetProgress(rowsCount, i + 1, progressMsg,
                    string.Format("{0}. ������ {1} �� {2}", blockName, i + 1, rowsCount));

                if (CheckCodeExclusion(rows[i]["KBK"], codeExclusions) ||
                    !GetClsValuesMapping(fileFormNo, clsTable, cls, nullRefsToCls, codesMapping,
                        isMark, yearIndex, clsValuesMapping, rows[i], blockProcessModifier, kbk2ClsMapping))
                {
                    continue;
                }

                ProcessDBFRow(fileFormNo, repKind, factTable, fct, date, rows[i],
                    (object[])CommonRoutines.ConcatArrays(clsValuesMapping, individualCodesMapping),
                    regionCache, nullRegions, regions4PumpSkifCache);

                if (factTable.Rows.Count >= constMaxQueryRecords)
                {
                    UpdateData();
                    ClearDataSet(da, factTable);
                }
            }
        }

        #endregion ������� ������� ������ �����


        #region ����� ����������� ������� ������� DBF

        /// <summary>
        /// ��������� ��� ����� � �������� �� ������������ ���������, ������� �������� � �.�.
        /// </summary>
        /// <param name="dir">�������</param>
        private void CheckDBFFilesInDir(DirectoryInfo dir)
        {
            if (dir.GetFiles("MBUD.DBF", SearchOption.AllDirectories).GetLength(0) == 0)
                throw new PumpDataFailedException("����������� ���������� ����� ��������.");

            if (dir.GetFiles("MOKRUG.DBF", SearchOption.AllDirectories).GetLength(0) == 0)
                throw new PumpDataFailedException("����������� ���������� ��������������� �����������.");
        }

        /// <summary>
        /// ���������� ����� ������� DBF
        /// </summary>
        /// <param name="file">���� ������</param>
        protected virtual void PumpDBFReport(FileInfo file, string progressMsg)
        {

        }

        /// <summary>
        /// ���������� ��� ����� ������, ����������� � �������
        /// </summary>
        /// <returns>������ ������� ����</returns>
        protected virtual int[] GetAllFormNo()
        {
            return null;
        }

        /// <summary>
        /// ������������� �������� ��������� ��� ���� � ����������� �� ����
        /// </summary>
        private void ConfigureDbfParams()
        {
            sumFactor = 1;

            if (this.DataSource.Year < 2005)
            {
                // ��� ������� ������� �� 2005 ���������� � �����.
                sumFactor = 1000;
            }
        }

        /// <summary>
        /// ����������������� ���������� � ����������, ��������� ��������� �������
        /// </summary>
        /// <param name="driver">�������</param>
        private void ReconnectToDbfDataSource(ODBCDriverName driver)
        {
            // ����������� � ���������
            dbDataAccess.ConnectToDataSource(ref this.dbfDB, this.currentDir.FullName, driver);
        }

        /// <summary>
        /// ���������� ������ ������� DBF
        /// </summary>
        /// <param name="dir">������� � ��������</param>
        private void PumpDBFReports(DirectoryInfo dir)
        {
            this.currentDir = dir;

            switch (this.SkifReportFormat)
            {
                case SKIFFormat.MonthReports: if (this.DataSource.Year < 2002 || this.DataSource.Year > 2004) return;
                    break;

                case SKIFFormat.YearReports: if (this.DataSource.Year < 2000 || this.DataSource.Year > 2004) return;
                    break;

                default: return;
            }

            FileInfo[] filesList = dir.GetFiles("*.DBF", SearchOption.AllDirectories);
            if (filesList.GetLength(0) == 0) return;

            ConfigureDbfParams();

            // ��������� ����� � ��������
            CheckDBFFilesInDir(dir);

            try
            {
                FileInfo[] filesRepList;
                FileInfo[] filesPtrnList;
                LoadDBFFiles(filesList, out filesRepList, out filesPtrnList);

                ReconnectToDbfDataSource(ODBCDriverName.Microsoft_dBase_Driver);

                // ���������� ������
                PumpDBFPattern(filesPtrnList);

                ReconnectToDbfDataSource(ODBCDriverName.Microsoft_dBase_Driver);

                // ������������ �����
                for (int i = 0; i < filesRepList.GetLength(0); i++)
                {
                    filesCount++;
                    string progressMsg = string.Format("��������� ����� {0} ({1} �� {2})...",
                        filesRepList[i].Name, filesCount, dbfFilesCount);

                    if (!filesRepList[i].Exists) continue;

                    if (!CommonRoutines.CheckValueEntry(GetFileFormNo(filesRepList[i]), GetAllFormNo())) continue;

                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStartFilePumping, 
                        string.Format("����� ������� ����� {0}.", filesRepList[i].FullName));

                    try
                    {
                        PumpDBFReport(filesRepList[i], progressMsg);

                        WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeSuccessfullFinishFilePump,
                            string.Format("���� {0} ������� �������.", filesRepList[i].FullName));
                    }
                    catch (ThreadAbortException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeFinishFilePumpWithError, 
                            string.Format("������� �� ����� {0} ��������� � ��������", filesRepList[i].FullName), ex);
                        throw;
                    }
                    finally
                    {
                        CollectGarbage();
                    }

                    // ���������� ������
                    UpdateData();
                }
            }
            finally
            {
                if (this.DbfDB != null)
                {
                    this.DbfDB.Close();
                    this.DbfDB = null;
                }
            }
        }

        #endregion ����� ����������� ������� ������� DBF
    }
}