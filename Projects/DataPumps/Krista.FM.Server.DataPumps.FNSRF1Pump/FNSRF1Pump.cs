using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.FNSRF1Pump
{

    // ��� �� - 0001 - ����� 4-��
    public partial class FNSRF1PumpModule : CorrectedPumpModuleBase
    {

        #region ����

        #region ��������������

        // �������������.��� (d_Arrears_FNS)
        private IDbDataAdapter daArrears;
        private DataSet dsArrears;
        private IClassifier clsArrears;
        private Dictionary<string, int> arrearsCache = null;
        private int nullArrears;
        // ������.������ ��� (d_D_GroupFNS)
        private IDbDataAdapter daIncomes;
        private DataSet dsIncomes;
        private IClassifier clsIncomes;
        private Dictionary<string, int> incomesCache = null;
        private int nullIncomes;
        // ����������.��� �� (d_Territory_FNSRF)
        private IDbDataAdapter daTerritory;
        private DataSet dsTerritory;
        private IClassifier clsTerritory;
        private Dictionary<string, int> territoryCache = null;
        private Dictionary<string, int> territoryCacheByName = null;
        private int nullTerritory;

        #endregion ��������������

        #region �����

        // ������.��� ��_4�� (f_D_FNSRF4NM)
        private IDbDataAdapter daIncomesFact;
        private DataSet dsIncomesFact;
        private IFactTable fctIncomesFact;
        private List<DataRow> rowsIncomesFact = null;

        #endregion �����

        // ����� ��������������� �������
        private int sectionIndex = -1;
        // ��� �������� ����������� ����
        private decimal[] totalSums = new decimal[26];
        // ��������� ���������
        private int year;
        private int month;
        // id ��������� ��� �������������� "�������������"
        private int arrearsSourceId;

        // ������������ ��������
        private string[] sectionNames;
        // ���� �������������� �� ��������
        private Dictionary<int, List<int>> arrearsCodes;
        // Id ������������ ������� �������������
        private int[] arrearsParentIds;
        // �������� ����� � ������ �������
        private Dictionary<int, int[]> incomesMappings;
        // ����������� ����� "�������������" � ������������ "�������"
        private Dictionary<int, int> incomesByArrears = new Dictionary<int, int>();

        // �������������� �������� �������: ����������� ������������� "�������������.���"
        private bool toPumpArrears = false;

        //��� ��������
        private bool isSvod = false;
        private int svodPart = -1; // ������ 3 ������� �� ���������� ������
        private int territoryIndex = 0; //� ������� ����� ����������� �� �������

        //� ������� ������ 4.1 �� ������ ������� �� ��������������
        private int[] arrearsCodeSprav = new int[4];
        private int[] arrearsNameSprav = new int[4];

        #endregion ����

        #region ������� ������

        #region ������ � ����� � ������

        private void QueryArrearsData()
        {
            if (this.DataSource.Month == 1)
            {
                toPumpArrears = true;
                // � ������ ���� (����� = 01) ��� ����� �������� ��������� ucbPumpArrearsFNS
                // ������������� ��������������.��ѻ ����������� �� ������� ����� ����
                arrearsSourceId = AddDataSource("�����", "0001", ParamKindTypes.YearMonth,
                    string.Empty, this.DataSource.Year, 0, string.Empty, 0, string.Empty).ID;
                InitClsDataSet(ref daArrears, ref dsArrears, clsArrears, false, string.Empty, arrearsSourceId);
            }
            else
            {
                toPumpArrears = Convert.ToBoolean(GetParamValueByName(this.PumpRegistryElement.ProgramConfig,
                    "ucbPumpArrearsFNS", "False"));
                // ���� ����� �� 01, �� ��� ���������� ��������� ucbPumpArrearsFNS
                // ������������� ��������������.��ѻ ������ ������������� �� �������� ��������� �������
                if (toPumpArrears)
                {
                    arrearsSourceId = AddDataSource("�����", "0001", ParamKindTypes.YearMonth,
                        string.Empty, this.DataSource.Year, this.DataSource.Month, string.Empty, 0, string.Empty).ID;
                    InitClsDataSet(ref daArrears, ref dsArrears, clsArrears, false, string.Empty, arrearsSourceId);
                }
                // ����� ������������� ��������������.��ѻ �� �����������
                // ������ �� ������� ������ �������� �� ��������� ��������� �������� ����� ����
                else
                {
                    for (int curMonth = this.DataSource.Month; curMonth >= 0; curMonth--)
                    {
                        string query = string.Format(
                            "select id from DataSources where DELETED = 0 and SUPPLIERCODE = '�����' and DATACODE = 1 and year = {0} and month = {1}",
                            this.DataSource.Year, curMonth);
                        object arrSourceId = this.DB.ExecQuery(query, QueryResultTypes.Scalar, new IDbDataParameter[] { });
                        if ((arrSourceId == null) || (arrSourceId == DBNull.Value))
                            continue;
                        arrearsSourceId = Convert.ToInt32(arrSourceId);
                        InitClsDataSet(ref daArrears, ref dsArrears, clsArrears, false, string.Empty, arrearsSourceId);
                        if (dsArrears.Tables[0].Rows.Count > 3)
                            break;
                    }
                }
            }
            nullArrears = clsArrears.UpdateFixedRows(this.DB, arrearsSourceId);
        }

        protected override void QueryData()
        {
            QueryArrearsData();
            int incomesSourceId = AddDataSource("�����", "0001", ParamKindTypes.YearMonth, string.Empty, this.DataSource.Year, 0, string.Empty, 0, string.Empty).ID;
            InitDataSet(ref daIncomes, ref dsIncomes, clsIncomes, false, string.Format("SOURCEID = {0} AND ID > 0", incomesSourceId), string.Empty);
            nullIncomes = clsIncomes.UpdateFixedRows(this.DB, incomesSourceId);
            InitClsDataSet(ref daTerritory, ref dsTerritory, clsTerritory);
            nullTerritory = clsTerritory.UpdateFixedRows(this.DB, this.SourceID);

            //���������� ����� ����������
            Object res = this.DB.ExecQuery("select max(Code) from d_Territory_FNSRF", null, QueryResultTypes.Scalar);
            if (res == DBNull.Value)
                territoryIndex = 1;
            else territoryIndex = Convert.ToInt32(res) + 1;

            InitFactDataSet(ref daIncomesFact, ref dsIncomesFact, fctIncomesFact);
            FillCaches();
        }

        private void FillCaches()
        {
            FillRowsCache(ref arrearsCache, dsArrears.Tables[0], "CODE", "ID");
            FillRowsCache(ref incomesCache, dsIncomes.Tables[0], "CODE", "ID");
            FillRowsCache(ref territoryCache, dsTerritory.Tables[0], "CODE", "ID");
            FillRowsCache(ref territoryCacheByName, dsTerritory.Tables[0], "NAME", "ID");
        }

        protected override void UpdateData()
        {
            if (toPumpArrears)
                UpdateDataSet(daArrears, dsArrears, clsArrears);
            UpdateDataSet(daIncomes, dsIncomes, clsIncomes);
            UpdateDataSet(daTerritory, dsTerritory, clsTerritory);
            UpdateDataSet(daIncomesFact, dsIncomesFact, fctIncomesFact);
        }

        private const string D_ARREARS_FNS_GUID = "516ec293-bf4c-4ff8-a2c5-bc04acb70a81";
        private const string D_D_GROUP_FNS_GUID = "b9169eb6-de81-420b-8a2b-05ffa2fd35c1";
        private const string D_TERRITORY_FNSRF_GUID = "965c78a9-b857-44ce-9e26-4a14ba740be6";
        private const string F_D_FNS4RFNM_FACT_GUID = "4a6d0737-6916-4a44-8909-ee0d5cea667f";
        protected override void InitDBObjects()
        {
            this.AssociateClassifiersEx = new IClassifier[] {
                clsArrears = this.Scheme.Classifiers[D_ARREARS_FNS_GUID] };
            this.UsedClassifiers = new IClassifier[] {
                clsTerritory = this.Scheme.Classifiers[D_TERRITORY_FNSRF_GUID],
                clsIncomes = this.Scheme.Classifiers[D_D_GROUP_FNS_GUID] };
            this.UsedFacts = new IFactTable[] {
                fctIncomesFact = this.Scheme.FactTables[F_D_FNS4RFNM_FACT_GUID] };
        }

        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsIncomesFact);
            ClearDataSet(ref dsArrears);
            ClearDataSet(ref dsIncomes);
            ClearDataSet(ref dsTerritory);
        }

        #endregion ������ � ����� � ������

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
            factValue = CommonRoutines.TrimLetters(factValue.Trim()).PadLeft(1, '0');
            return Convert.ToDecimal(factValue);
        }

        private void CheckIncomes()
        {
            // ���� �� �������� ������������� ������.������ ��� - ��������������
            if (incomesCache.Count <= 1)
                throw new Exception("�� �������� ������������� '������.������ ���'. ������ �� ����� ��������� �������� �� �����.");
        }

        private int GetReportDate()
        {
            return this.DataSource.Year * 10000 + this.DataSource.Month * 100;
        }

        private void PumpDataRow(DataTable dt, ref List<DataRow> dataRows, object[] mapping)
        {
            if (mapping == null)
                return;

            DataRow row = dt.NewRow();
            if (dt.Columns.Contains("SOURCEID"))
                row["SOURCEID"] = this.SourceID;
            if (dt.Columns.Contains("PUMPID"))
                row["PUMPID"] = this.PumpID;
            if (dt.Columns.Contains("TASKID"))
                row["TASKID"] = -1;
            CopyValuesToRow(row, mapping);

            dataRows.Add(row);
        }

        private void SaveDataRowsToTables(DataTable dt, ref List<DataRow> dataRows)
        {
            foreach (DataRow row in dataRows)
            {
                dt.Rows.Add(row);
            }
            dataRows.Clear();
        }

        #endregion ����� ������� �������

        #region ������ � Excel

        private void CheckXlsTotalSums(ExcelHelper excelDoc, int curRow)
        {
            if (CommonRoutines.CheckValueEntry(sectionIndex, new int[] { 4, 6, 8 }))
                return;
            int count = incomesMappings[sectionIndex].GetLength(0);
            for (int i = 0; i < count; i += 2)
            {
                int curColumn = incomesMappings[sectionIndex][i];
                string comment = string.Format("� ������� '{0}', ������� '{1}'", curColumn, sectionNames[sectionIndex]);
                decimal controlSum = CleanFactValue(excelDoc.GetValue(curRow, curColumn));
                CheckTotalSum(totalSums[i / 2], controlSum, comment);
            }
        }

        private const string TERRITORY_CODE = "��������� �����";
        private int GetXlsTerritoryCodeRow(ExcelHelper excelDoc)
        {
            for (int curRow = 5; curRow < 25; curRow++)
                if (excelDoc.GetValue(curRow, 1).Trim().ToUpper().Contains(TERRITORY_CODE))
                    return curRow;
            return 1;
        }

        private const string TERRITORY_NAME = "�����";
        private int GetXlsTerritoryNameRow(ExcelHelper excelDoc)
        {
            for (int curRow = 5; curRow < 25; curRow++)
                if (excelDoc.GetValue(curRow, 1).Trim().ToUpper().Contains(TERRITORY_NAME))
                    return curRow;
            return 1;
        }

        private int PumpXlsTerritory(ExcelHelper excelDoc, int curRow)
        {
            string territoryName = string.Empty;
            string territoryCode = "0";

            if (isSvod)
            {
                territoryName = excelDoc.GetValue(curRow, 1).Trim();
                if (territoryName == string.Empty)
                    territoryName = constDefaultClsName;
                
                if (territoryCacheByName.ContainsKey(territoryName))
                    return territoryCacheByName[territoryName];
                
                territoryCode = Convert.ToString(territoryIndex++);
            }
            else
            {
                int territoryCodeRow = GetXlsTerritoryCodeRow(excelDoc);
                if (territoryCodeRow != 1)
                    territoryCode = CommonRoutines.TrimLetters(excelDoc.GetValue(territoryCodeRow, 1).Trim());

                int territoryNameRow = GetXlsTerritoryNameRow(excelDoc);
                if (territoryNameRow != 1)
                    territoryName = excelDoc.GetValue(territoryNameRow + 1, 1).Trim();
                if (territoryName == string.Empty)
                    territoryName = constDefaultClsName;

            }

            object[] mapping = new object[] { "NAME", territoryName, "CODE", territoryCode };
            int resultID = PumpCachedRow(territoryCache, dsTerritory.Tables[0], clsTerritory, mapping, territoryCode, "ID");
            territoryCacheByName.Add(territoryName, resultID);
            return resultID;
        }

        // ��������, �������� �� ��� ������������� ������������ ���
        // �������������� ����� �������� ������ (� ������� II.I)
        private bool IsParentForAppendArrearsCodes(int arrearsCode)
        {
            switch (arrearsCode)
            {
                case 2400:
                case 2410:
                case 2420:
                case 2430:
                case 2440:
                case 2450:
                case 2460:
                case 2470:
                case 2900:
                case 2910:
                case 2920:
                case 2925:
                case 2930:
                case 2940:
                case 2950:
                case 2960:
                    return true;
            }
            return false;
        }

        // ��� ���� ������ ����� �������� ������ �� �������� ������ �������� ��������������
        private int GetChildArrearsId(int column, int arrearsCode)
        {
            if (sectionIndex == 1)
            {
                if ((column == 2) || ( isSvod && (column == 0)))
                    arrearsCode = arrearsCode * 10 + 1;
                else if ((column == 4) || (isSvod && (column == 2)))
                    arrearsCode = arrearsCode * 10 + 2;
            }
            else if ((sectionIndex == 2) && IsParentForAppendArrearsCodes(arrearsCode))
            {
                if (((column == 2) && (!isSvod)) || (isSvod && (column == 0)))
                    arrearsCode = arrearsCode * 10 + 1;
                else if (((column == 4) && (!isSvod)) || ((isSvod && (column == 2))))
                    arrearsCode = arrearsCode * 10 + 2;
                else if (((column == 6) && (!isSvod)) || (isSvod && (column == 4)))
                    arrearsCode = arrearsCode * 10 + 3;
            }
            return FindCachedRow(arrearsCache, arrearsCode.ToString(), nullArrears);
        }

        private int PumpArrears(object[] mapping, string arrearsKey)
        {
            if (toPumpArrears)
                return PumpCachedRow(arrearsCache, dsArrears.Tables[0], clsArrears, arrearsKey, mapping);
            return FindCachedRow(arrearsCache, arrearsKey, nullArrears);
        }

        private object[] GetXlsArrearsMapping(ExcelHelper excelDoc, int curRow)
        {
            string arrearsName;
            string arrearsCode;

            if (isSvod)
            {
                //��� ����� ���� '��� - ��������'
                arrearsCode = excelDoc.GetWorksheetName().Trim();
                if (sectionIndex == 4)
                {
                    arrearsName = excelDoc.GetValue(5, 2); // ���������� B5
                    arrearsName = arrearsName.Substring(0, arrearsName.ToUpper().IndexOf("������")).Trim();// �� ����� "������"
                }
                else
                {
                    arrearsName = excelDoc.GetValue(4, 1);
                    arrearsName = arrearsName.Substring(arrearsName.IndexOf('-') + 1).Trim();
                }
            }
            else
            {
                arrearsName = excelDoc.GetValue(curRow, 1).Trim();
                arrearsCode = excelDoc.GetValue(curRow, 2).Trim();
            }
            object[] mapping = new object[] { "SOURCEID", arrearsSourceId, "NAME", arrearsName, "CODE", arrearsCode, "PARENTID", GetArrearsParentId(sectionIndex) };
            return mapping;
        }

        private int PumpXlsArrears(ExcelHelper excelDoc, int curRow, int curCol)
        {
            string arrearsName = GetXlsArrearsName(excelDoc, curRow, curCol);
            string arrearsCode = GetXlsArrearsCode(excelDoc, curRow, curCol);
            //� ������� ������� 3� ������ ������� �� 3 �����. ��� ����� ������������ ��� ������ 9,10. � arrearsparent ����� ������� ���. ������� �������� �������� ��� � ������ 3
            object[] mapping = new object[] { "SOURCEID", arrearsSourceId, "NAME", arrearsName, "CODE", arrearsCode, "PARENTID", GetArrearsParentId((sectionIndex>8) ? 3 : sectionIndex) };
            int arrearsId = PumpArrears(mapping, arrearsCode);


            switch (sectionIndex)
            {
                case 1:
                    // � ������ II ��������� 2 ������ �� ������� ������ �������� - ���� � ������� 
                    mapping = new object[] { "SOURCEID", arrearsSourceId, "NAME", "������������� �� ������ �����", "CODE", arrearsCode + "1", "PARENTID", arrearsId };
                    PumpArrears(mapping, arrearsCode + "1");
                    mapping = new object[] { "SOURCEID", arrearsSourceId, "NAME", "������������� �� ������ ��������� �������", "CODE", arrearsCode + "2", "PARENTID", arrearsId };
                    PumpArrears(mapping, arrearsCode + "2");
                    break;
                case 2:
                    // � ������ II.I ��������� 3 ������ �� ������� ������ �������� - ��������, ���� � ������
                    if (!IsParentForAppendArrearsCodes(Convert.ToInt32(arrearsCode)))
                        break;
                    mapping = new object[] { "SOURCEID", arrearsSourceId, "NAME", "������������� �� ��������", "CODE", arrearsCode + "1", "PARENTID", arrearsId };
                    PumpArrears(mapping, arrearsCode + "1");
                    mapping = new object[] { "SOURCEID", arrearsSourceId, "NAME", "������������� �� ����", "CODE", arrearsCode + "2", "PARENTID", arrearsId };
                    PumpArrears(mapping, arrearsCode + "2");
                    mapping = new object[] { "SOURCEID", arrearsSourceId, "NAME", "������������� �� ������� (�� 01.01.1999)", "CODE", arrearsCode + "3", "PARENTID", arrearsId };
                    PumpArrears(mapping, arrearsCode + "3");
                    break;
            }
            return arrearsId;
        }

        private int GetColumnsCount(ExcelHelper excelDoc, int curRow)
        {
            int columnsCount = 0;
            for (int curColumn = (isSvod)?2:3; ; curColumn++)
            {
                string cellValue = excelDoc.GetValue(curRow, curColumn).Trim();
                string cellValueNext = excelDoc.GetValue(curRow, curColumn + 1).Trim();
                if ((cellValue == string.Empty) && (cellValueNext == string.Empty))
                    break;
                cellValueNext = excelDoc.GetValue(curRow + 1, curColumn).Trim();
                if ((cellValue != string.Empty) || (cellValueNext != string.Empty))
                    columnsCount++;
            }
            return columnsCount;
        }

        private void CheckColumnsCount(ExcelHelper excelDoc, int curRow)
        {
            int columnsCount = incomesMappings[sectionIndex].GetLength(0) / 2;
            int reportColumnsCount = GetColumnsCount(excelDoc, curRow);
            if (columnsCount != reportColumnsCount)
            {
                throw new Exception(string.Format(
                    "���������� �������� � ������� \"{0}\" �� ������������� ������� ������ (������ ���� {1}, � ������ {2})",
                    sectionNames[sectionIndex], columnsCount, reportColumnsCount));
            }
        }

        private string GetXlsArrearsName(ExcelHelper excelDoc, int curRow, int curCol)
        {
            string arrearsName;

            if (isSvod)
            {
                //��� ����� ���� '��� - ��������'
                if (sectionIndex == 4)
                {
                    arrearsName = excelDoc.GetValue(5, 2); // ���������� B5
                    arrearsName = arrearsName.Substring(0, arrearsName.ToUpper().IndexOf("�����")).Trim();// �� ����� "������"
                }
                else if ((sectionIndex == 6) || sectionIndex == 8)
                {
                    if (sectionIndex == 6)
                        curRow = 6;
                    else curRow = 5;
                    if (curCol >3)
                        curRow++;
                    arrearsName = excelDoc.GetValue(curRow, curCol);
                    arrearsName = arrearsName.Substring(0, arrearsName.ToUpper().IndexOf("�����")).Trim();// �� ����� "������"
                }
                else
                {
                    arrearsName = excelDoc.GetValue(4, 1);
                    arrearsName = arrearsName.Substring(arrearsName.IndexOf('-') + 1).Trim();
                }
            }
            else arrearsName = excelDoc.GetValue(curRow, 1).Trim();
            return arrearsName;
        }

        private string GetXlsArrearsCode(ExcelHelper excelDoc, int curRow, int curCol)
        {
            string arrearsCode;

            if (isSvod)
            {
                //��� ����� ���� '��� - ��������'
                if ((sectionIndex == 6) || sectionIndex == 8)
                {
                    if (sectionIndex == 6)
                        curRow = 6;
                    else curRow = 5;
                    if (curCol >3)
                        curRow++;
                    arrearsCode = excelDoc.GetValue(curRow, curCol);
                    arrearsCode = CommonRoutines.TrimLetters(arrearsCode.Substring(arrearsCode.ToUpper().IndexOf("�����"))).Trim();// ����� ����� "������"
                }
                else arrearsCode = excelDoc.GetWorksheetName().Trim();
            }
            else
            {
                arrearsCode = excelDoc.GetValue(curRow, 2).Trim();
            }
            return arrearsCode;
        }

        private void PumpXlsRow(ExcelHelper excelDoc, int curRow, int refDate, int refTerritory)
        {
            string arrearsCodeStr = GetXlsArrearsCode(excelDoc, curRow, 2);
            if (arrearsCodeStr == string.Empty)
                return;
            int arrearsCode = Convert.ToInt32(arrearsCodeStr);
            sectionIndex = GetSectionIndexByArrearsCode(arrearsCode);
            CheckColumnsCount(excelDoc, curRow);
            int refArrears = PumpXlsArrears(excelDoc, curRow, 0);

            int count = incomesMappings[sectionIndex].GetLength(0);
            for (int i = 0; i < count; i += 2)
            {
                int curCol = incomesMappings[sectionIndex][i];
                decimal factValue = CleanFactValue(excelDoc.GetValue(curRow, curCol));
                if (factValue == 0)
                    continue;

                totalSums[i / 2] += factValue;
                factValue *= 1000;

                refArrears = GetChildArrearsId(i, arrearsCode);
                int refIncomes = GetRefIncomes(i, arrearsCode);
                object[] mapping = new object[] {
                    "ValueReport", factValue, "Value", 0, "RefYearDayUNV", refDate, 
                    "RefD", refIncomes, "RefArrears", refArrears, "RefTerritory", refTerritory };
                PumpDataRow(dsIncomesFact.Tables[0], ref rowsIncomesFact, mapping);
                if (rowsIncomesFact.Count >= MAX_DS_RECORDS_AMOUNT)
                {
                    SaveDataRowsToTables(dsIncomesFact.Tables[0], ref rowsIncomesFact);
                    UpdateData();
                    ClearDataSet(daIncomesFact, ref dsIncomesFact);
                }
            }
        }

        private void PumpXlsRowSvod(ExcelHelper excelDoc, int curRow, int refDate)
        {
            //���� ����� �����, ���� ����� ����� ��������. ������ ������ ��� ����������� �������� ����� �� ����� �������.
            //����� ������������� �� ������� ����.
            int arrearsCode;
            string arrearsCodeStr;
            int refTerritory = PumpXlsTerritory(excelDoc, curRow);

            int count = incomesMappings[sectionIndex].GetLength(0);
            for (int i = 0; i < count; i += 2)
            {
                int curCol = incomesMappings[sectionIndex][i];
                decimal factValue = CleanFactValue(excelDoc.GetValue(curRow, curCol));
                if (factValue == 0)
                    continue;

                arrearsCodeStr = GetXlsArrearsCode(excelDoc, curRow, (i / 2) + 2);
                if (arrearsCodeStr == string.Empty)
                    return;
                arrearsCode = Convert.ToInt32(arrearsCodeStr);

                int refArrears = PumpXlsArrears(excelDoc, curRow, (i / 2) + 2);

                totalSums[i / 2] += factValue;
                factValue *= 1000;

                refArrears = GetChildArrearsId(i, arrearsCode);
                int refIncomes = GetRefIncomes(i, arrearsCode);
                object[] mapping = new object[] {
                    "ValueReport", factValue, "Value", 0, "RefYearDayUNV", refDate, 
                    "RefD", refIncomes, "RefArrears", refArrears, "RefTerritory", refTerritory };
                PumpDataRow(dsIncomesFact.Tables[0], ref rowsIncomesFact, mapping);
                if (rowsIncomesFact.Count >= MAX_DS_RECORDS_AMOUNT)
                {
                    SaveDataRowsToTables(dsIncomesFact.Tables[0], ref rowsIncomesFact);
                    UpdateData();
                    ClearDataSet(daIncomesFact, ref dsIncomesFact);
                }
            }
        }
        

        private bool IsTableTitle(string cellValue)
        {
            return (string.Compare(cellValue, "�", true) == 0);
        }

        private bool IsSectionEndMark(string cellValue)
        {
            return (cellValue.StartsWith("����������� �����"));
        }

        private bool IsReportEndMark(string cellValue)
        {
            return (cellValue.StartsWith("�����������"));
        }

        private void PumpXlsSheetData(string fileName, ExcelHelper excelDoc, int refDate)
        {
            int rowsCount = excelDoc.GetRowsCount();
            if (rowsCount < 10)
                return;

            int refTerritory = (isSvod)? 0 : PumpXlsTerritory(excelDoc,0);
            if (!isSvod)
                sectionIndex = -1;
            bool toPumpRow = false;
            string dataSourcePath = GetShortSourcePathBySourceID(this.SourceID);
            for (int curRow = 1; curRow <= rowsCount; curRow++)
            {
                try
                {
                    SetProgress(rowsCount, curRow,
                        string.Format("��������� ����� {0}\\{1}...", dataSourcePath, fileName),
                        string.Format("������ {0} �� {1}", curRow, rowsCount));

                    string cellValue = excelDoc.GetValue(curRow, 1).Trim().ToUpper();
                    if (cellValue == string.Empty)
                        toPumpRow = false;
                    if (cellValue.Contains("���") && cellValue.Contains("�����") && isSvod)
                        continue;

                    if (IsReportEndMark(cellValue))
                        return;

                    if (cellValue.StartsWith("������") && (!isSvod))
                    {
                        toPumpRow = false;
                        sectionIndex = GetSectionIndex(cellValue);
                    }
                    if (IsSectionEndMark(cellValue))
                    {
                        toPumpRow = false;
                        // ��������� ����������� �����
                        if (sectionIndex != -1)
                            CheckXlsTotalSums(excelDoc, curRow);
                        sectionIndex = -1;
                    }
                    if (toPumpRow)
                        if (isSvod)
                            PumpXlsRowSvod(excelDoc, curRow, refDate);
                        else PumpXlsRow(excelDoc, curRow, refDate, refTerritory);
                    if (IsTableTitle(cellValue))
                    {
                        // ����� ������ - ������� ����������� �����
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

        private bool CheckOnSvod(string fileName)
        {
            bool isSvod = false;
            svodPart = -1;
            string fname = fileName.ToUpper();
            if (fname.Contains("������"))
            {
                string str = CommonRoutines.RemoveLetters(fileName).Trim();
                sectionIndex = Convert.ToInt32(str.Substring(1,1)); //��������� ������ 1� ����� ����� �������

                switch (sectionIndex)
                {
                    case 1: sectionIndex = 0; break;
                    case 2: sectionIndex = 1; break;
                    case 3: sectionIndex = 3; break;
                    case 4: sectionIndex = 5; break;
                    case 5: sectionIndex = 7; break;
                    default: sectionIndex = -1; break;
                }

                if (fname.Contains("����")) //4-�� ������ 3 ����� 11.xls
                {
                    svodPart = Convert.ToInt32(str.Substring(2, 1));
                    if (svodPart == 2)
                        sectionIndex = 9;
                    else if (svodPart == 3)
                        sectionIndex = 10;
                }
                else if (fname.Contains("�����")) //4-�� ������ ����� 1-2-31.xls
                {
                    switch (sectionIndex)
                    {
                        case 0: sectionIndex = 2; break;
                        case 3: sectionIndex = 4; break;
                        case 5: sectionIndex = 6; break;
                        case 7: sectionIndex = 8; break;
                    }
                }
                isSvod = true;
            }
            return isSvod;
        }

        private void PumpXlsFile(FileInfo file)
        {
            WriteToTrace("�������� ���������: " + file.Name, TraceMessageKind.Information);
            ExcelHelper excelDoc = new ExcelHelper();
            rowsIncomesFact = new List<DataRow>();
            try
            {
                excelDoc.AskToUpdateLinks = false;
                excelDoc.DisplayAlerts = false;
                excelDoc.OpenDocument(file.FullName);
                isSvod = CheckOnSvod(file.Name);
                int refDate = GetReportDate();
                int wsCount = excelDoc.GetWorksheetsCount();
                //���������������� ��������� ����������� ����� �������� �� ������� ����.
                InitAuxStructures();

                for (int index = 1; index <= wsCount; index++)
                {
                    excelDoc.SetWorksheet(index);
                    PumpXlsSheetData(file.Name, excelDoc, refDate);
                }
                WriteToTrace("�� ����� �" + file.Name + "� �������� " + wsCount.ToString() + " ������", TraceMessageKind.Information);
                SaveDataRowsToTables(dsIncomesFact.Tables[0], ref rowsIncomesFact);
                UpdateData();
            }
            catch (Exception ex)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeError, ex.Message);
                this.DataSourceProcessingResult = DataSourceProcessingResult.ProcessedWithErrors;
            }
            finally
            {
                rowsIncomesFact.Clear();
                if (excelDoc != null)
                    excelDoc.CloseDocument();
            }
        }

        private void PumpXlsFiles(DirectoryInfo dir)
        {
            CheckIncomes();
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStartFilePumping, "����� ������� ������ �� �������.");
            ProcessFilesTemplate(dir, "*.xls", new ProcessFileDelegate(PumpXlsFile), false);
        }

        #endregion ������ � Excel

        #region ���������� ������ �������

        protected override void ProcessFiles(DirectoryInfo dir)
        {
            try
            {
                if (dir.GetFiles("*.xls", SearchOption.AllDirectories).GetLength(0) == 0)
                    return;
                PumpXlsFiles(dir);
                SetClsHierarchy();
                UpdateData();
            }
            finally
            {
                arrearsCodes.Clear();
                incomesMappings.Clear();
                incomesByArrears.Clear();
            }
        }

        protected override void DirectPumpData()
        {
            PumpDataYMTemplate();
        }

        #endregion ���������� ������ �������

        #endregion ������� ������

        #region ��������� ������

        protected override void ProcessFinalizing()
        {
            PumpFinalizing();
        }

        private void SetClsHierarchy()
        {
            string d_Arrears_FNSRF1_HierarchyFileName = string.Empty;

            if (this.DataSource.Year >= 2011)
                d_Arrears_FNSRF1_HierarchyFileName = const_d_Arrears_FNSRF1_HierarchyFile2011;
            else if (this.DataSource.Year >= 2010)
                d_Arrears_FNSRF1_HierarchyFileName = const_d_Arrears_FNSRF1_HierarchyFile2010;
            else if (this.DataSource.Year >= 2009)
                d_Arrears_FNSRF1_HierarchyFileName = const_d_Arrears_FNSRF1_HierarchyFile2009;
            else if (this.DataSource.Year >= 2006)
                d_Arrears_FNSRF1_HierarchyFileName = const_d_Arrears_FNSRF1_HierarchyFile2006;
            else
                d_Arrears_FNSRF1_HierarchyFileName = const_d_Arrears_FNSRF1_HierarchyFile2005;
            SetClsHierarchy(clsArrears, ref dsArrears, "CODE", d_Arrears_FNSRF1_HierarchyFileName, ClsHierarchyMode.Special);
        }

        private void CorrectSumByDataSource()
        {
            F4NMSumCorrectionConfig f4nmSumCorrectionConfig = new F4NMSumCorrectionConfig();
            f4nmSumCorrectionConfig.ValueField = "Value";
            f4nmSumCorrectionConfig.ValueReportField = "ValueReport";
            GroupTable(fctIncomesFact, new string[] { "RefArrears", "RefD", "RefYearDayUNV", "RefTerritory" }, f4nmSumCorrectionConfig);
            CorrectFactTableSums(fctIncomesFact, dsArrears.Tables[0], clsArrears, "RefArrears",
                f4nmSumCorrectionConfig, BlockProcessModifier.MRStandard,
                new string[] { "RefD", "RefYearDayUNV", "RefTerritory" }, string.Empty, string.Empty, true);
            CorrectFactTableSums(fctIncomesFact, dsIncomes.Tables[0], clsIncomes, "RefD",
                f4nmSumCorrectionConfig, BlockProcessModifier.MRStandard,
                new string[] { "RefArrears", "RefYearDayUNV", "RefTerritory" }, string.Empty, string.Empty, false);
        }

        protected override void ProcessDataSource()
        {
            SetClsHierarchy();
            CorrectSumByDataSource();
            UpdateData();
        }

        protected override void DirectProcessData()
        {
            year = -1;
            month = -1;
            GetPumpParams(ref year, ref month);
            ProcessDataSourcesTemplate(year, month, "����������� ��������� �������� � �������������� ��������������.��ѻ � ��������� ���� ������ �� ������ ���������");
        }

        #endregion ��������� ������

    }

}