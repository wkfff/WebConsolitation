using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.BudgetDataPump
{
    // ������� ��������������� �� ������

    public partial class BudgetDataPumpModule : BudgetPumpModuleBase
    {

        #region ����� ������

        /// <summary>
        /// ���������� ������������� (���������) � ������ ������ ������� ����� �������
        /// </summary>
        /// <param name="tableName">�������� ������� � ���� �������</param>
        /// <param name="destTable">������� ��� ������� ������</param>
        /// <param name="cls">������ ��������������</param>
        /// <param name="fields">����, ���������� �� ����</param>
        /// <param name="joinScript">����� ������� � join ������</param>
        /// <param name="constr">����������� �������</param>
        /// <param name="processClsRowDelegate">������� ������� ��������� ������ ��������������</param>
        /// <param name="fieldsMapping">������ ��� ����� (���� �� - ���� �������)
        /// ���� ������� ����� ��������� ������: 
        /// "somestring" - �������� ���� � �������;
        /// "��������� � ������� �������" - �����-�� ���������, �������������� ��� �������� ���� �������������� ������;
        /// "somestring+somestring2" - ���� somestring + ���� somestring2;
        /// "somestring;somestring2" - ���� �������� ���� somestring = null, �� ��������� somestring2</param>
        /// <param name="codesMapping">��� ��������������</param>
        private void PumpBudgetCls(string tableName, DataTable destTable, IClassifier cls, string fields, 
            string joinScript, string constr, ProcessClsRowDelegate processClsRowDelegate, string[] fieldsMapping, 
            ref Dictionary<int, DataRow> codesMapping)
        {
            int firstRecCount = 0;
            int removedRecsCount = 0;
            int addedRecsCount = 0;
            int updatedRecsCount = 0;
            DataRow row;
            bool isAdded;

            try
            {
                string progressMsg = string.Format("��������� ������ {0}...", cls.FullCaption);
                WriteToTrace(progressMsg, TraceMessageKind.Information);

                SetProgress(0, 0, progressMsg, string.Empty);

                string whereStr = string.Empty;
                if (constr != string.Empty)
                {
                    whereStr = "where " + constr; 
                }

                ClearDataSet(ref dsBudgetFacts);
                string query = string.Format("select {0} from {1} {2} {3}", fields, tableName, joinScript, whereStr);
                InitLocalDataAdapter(this.BudgetDB, ref daBudgetCls, query);
                daBudgetCls.Fill(dsBudgetFacts);

                DataTable sourceTable = dsBudgetFacts.Tables[0];

                firstRecCount = destTable.Rows.Count;

                // ������� �� �������������� ������ ��� ������, ������� ������ ��� � �������� ������� �� ������
                RemoveAbsentRecsFromCls(tableName, sourceTable, destTable, out removedRecsCount);

                // ������������ ����������� ��� �������� � ����������� �� ������ �������
                string dateConstr = string.Empty;
                if (sourceTable.Columns.Contains("CREATEDATE") && sourceTable.Columns.Contains("UPDATEDATE"))
                {
                    dateConstr = GetDateConstrForDataSet();
                }
                DataRow[] rows = sourceTable.Select(dateConstr);

                // ��������� ���
               // if (this.PumpMode == BudgetDataPumpMode.FullFact || this.PumpMode == BudgetDataPumpMode.Update)
                    FillRowsCache(ref clsCache, destTable, new string[] { "SOURCEKEY" });

                // ��������� � ������������� ������ ��� ������, ������� �� ���� � �������������� ������, 
                // �� ������� ���� � �� ������
                ClassTypes objClassType = cls.ClassType;
                string generatorName = cls.GeneratorName;

                int count = rows.GetLength(0);
                string s = string.Empty;
                if (fieldsMapping != null)
                    s = fieldsMapping[0];
                for (int i = 0; i < count; i++)
                {
                    row = GetRowForUpdate(Convert.ToInt32(rows[i]["ID"]), destTable, clsCache, objClassType, generatorName, out isAdded);
                    row.BeginEdit();
                    if (isAdded)
                    {
                        addedRecsCount++;
                    }
                    else
                    {
                        updatedRecsCount++;
                    }

                    row["SOURCEKEY"] = rows[i]["ID"];
                    processClsRowDelegate(rows[i], row, fieldsMapping);
                    if ((fieldsMapping != null) && (fieldsMapping[0] == "fuck888"))
                    {
                        if (isAdded)
                            row.Delete();
                        else
                            row.EndEdit();
                        if (fieldsMapping != null)
                            fieldsMapping[0] = s; 
                        continue;
                    }

                    row.EndEdit();

                    SetProgress(rows.GetLength(0), i + 1, progressMsg,
                        string.Format("������ {0} �� {1}", i + 1, rows.GetLength(0)));
                }

                //string ss = CommonRoutines.UpdateDataSetByInsert(this.DB, dsKD2005.Tables[0], clsKD2005);

                FillRowsCache(ref codesMapping, destTable, "SOURCEKEY");

                // ���������� � ��������, ����� ������������� ������ �������, ������� ������� � �������������� ������ 
                // ����, ������� �����, ������� �������, ��������� � ���������
                WriteEventIntoDataPumpProtocol(
                    DataPumpEventKind.dpeInformation, string.Format(
                        "��������� ������������� {0}. �������� ���������� ������� ��������������: {1}, " +
                        "�������: {2}, ���������: {3}, ���������: {4}.",
                        cls.FullCaption, firstRecCount, removedRecsCount, addedRecsCount, updatedRecsCount));
            }
            catch (ThreadAbortException)
            {
                // ���������� � ��������, ����� ������������� ������ �������, ������� ������� � �������������� ������ 
                // ����, ������� �����, ������� �������, ��������� � ���������
                WriteEventIntoDataPumpProtocol(
                    DataPumpEventKind.dpeCriticalError, string.Format(
                        "������������� {0} ��������� � �������� : �������� �������� �������������. " +
                        "�� ������ ������������� ������ ���������� ��������� ����������. " +
                        "�������� ���������� ������� ��������������: {1}, " +
                        "�������: {2}, ���������: {3}, ���������: {4}. ������ �� ���������.",
                        cls.FullCaption, firstRecCount, removedRecsCount, addedRecsCount, updatedRecsCount));

                throw;
            }
            catch (Exception ex)
            {
                // ���������� � ��������, ����� ������������� ������ �������, ������� ������� � �������������� ������ 
                // ����, ������� �����, ������� �������, ��������� � ���������
                string msg = string.Format(
                        "������������� {0} ��������� � �������� : {1}. �� ������ ������������� ������ ���������� " +
                        "��������� ����������. �������� ���������� ������� ��������������: {2}, " +
                        "�������: {3}, ���������: {4}, ���������: {5}. ������ �� ���������.",
                        cls.FullCaption, ex, firstRecCount, removedRecsCount, 
                        addedRecsCount, updatedRecsCount);

                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeError, msg);
                throw;
            }
        }

        /// <summary>
        /// ���������� ������������� (���������) � ������ ������ ������� ����� �������
        /// </summary>
        /// <param name="tableName">�������� ������� � ���� �������</param>
        /// <param name="destTable">������� ��� ������� ������</param>
        /// <param name="cls">������ ��������������</param>
        /// <param name="joinScript">����� ������� � join ������</param>
        /// <param name="constr">����������� �������</param>
        /// <param name="processClsRowDelegate">������� ������� ��������� ������ ��������������</param>
        /// <param name="fieldsMapping">������ ��� ����� (���� �� - ���� �������)
        /// ���� ������� ����� ��������� ������: 
        /// "somestring" - �������� ���� � �������;
        /// "��������� � ������� �������" - �����-�� ���������, �������������� ��� �������� ���� �������������� ������;
        /// "somestring+somestring2" - ���� somestring + ���� somestring2;
        /// "somestring;somestring2" - ���� �������� ���� somestring = null, �� ��������� somestring2</param>
        /// <param name="codesMapping">��� ��������������</param>
        private void PumpBudgetCls(string tableName, DataTable destTable, IClassifier cls, 
            string joinScript, string constr, ProcessClsRowDelegate processClsRowDelegate, string[] fieldsMapping,
            ref Dictionary<int, DataRow> codesMapping)
        {
            PumpBudgetCls(tableName, destTable, cls, "*", joinScript, constr, processClsRowDelegate, fieldsMapping, ref codesMapping);
        }

        /// <summary>
        /// ���������� ������������� (���������) � ������ ������ ������� ����� �������
        /// </summary>
        /// <param name="tableName">�������� ������� � ���� �������</param>
        /// <param name="destTable">������� ��� ������� ������</param>
        /// <param name="cls">������ ��������������</param>
        /// <param name="joinScript">����� ������� � join ������</param>
        /// <param name="constr">����������� �������</param>
        /// <param name="processClsRowDelegate">������� ������� ��������� ������ ��������������</param>
        /// <param name="fieldsMapping">������ ��� ����� (���� �� - ���� �������)
        /// ���� ������� ����� ��������� ������: 
        /// "somestring" - �������� ���� � �������;
        /// "��������� � ������� �������" - �����-�� ���������, �������������� ��� �������� ���� �������������� ������;
        /// "somestring+somestring2" - ���� somestring + ���� somestring2;
        /// "somestring;somestring2" - ���� �������� ���� somestring = null, �� ��������� somestring2</param>
        private void PumpBudgetCls(string tableName, DataTable destTable, IClassifier cls,
            string joinScript, string constr, ProcessClsRowDelegate processClsRowDelegate, string[] fieldsMapping)
        {
            Dictionary<int, DataRow> codesMapping = null;
            PumpBudgetCls(tableName, destTable, cls, "*", joinScript, constr, processClsRowDelegate, fieldsMapping, ref codesMapping);
            codesMapping.Clear();
        }

        /// <summary>
        /// ���� ������ ��, �� ������� ������ �� ������� KESR � ��������� ISEKD = 1
        /// </summary>
        private void CheckBadKD()
        {
            switch (this.CurrentDBVersion)
            {
                case "27.02":
                case "28.00":
                case "29.01":
                case "29.02":
                case "30.00":
                case "30.01":
                    break;

                default:
                    ClearDataSet(ref dsBudgetFacts);

                    InitLocalDataAdapter(this.BudgetDB, ref daBudgetCls, string.Format(
                        "select Cast(id as varchar(30)) id from KD where KESR not in (select id from KESR where ISEKD = 1)"));
                    daBudgetCls.Fill(dsBudgetFacts);
                    dsBudgetFacts.Tables["Table"].TableName = "badkd";

                    string err = string.Empty;
                    for (int i = 0; i < dsBudgetFacts.Tables["badkd"].Rows.Count; i++)
                    {
                        err += ", " + Convert.ToString(dsBudgetFacts.Tables["badkd"].Rows[i]["ID"]);
                    }
                    if (err != string.Empty)
                    {
                        err = err.Remove(0, 2);
                        WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, string.Format(
                            "������� ������ ��, �� ������� ������ �� ������� KESR � ��������� ISEKD = 1: {0}.", err));
                    }
                    dsBudgetFacts.Tables.Remove("badkd");

                    break;
            }
        }

        /// <summary>
        /// ������������� �������� ����� ��������������� ��������� ������
        /// </summary>
        /// <param name="row">������</param>
        /// <param name="clsRow">������ ��������������</param>
        /// <param name="idField">������������ ���� �� ��������������</param>
        /// <param name="codeField">������������ ���� ���� ��������������</param>
        /// <param name="nameField">������������ ���� ����� ��������������</param>
        /// <param name="defaultValue">�������� �� �������������� ����������� ������</param>
        private void SetClsFieldsByRow(DataRow row, DataRow clsRow, string idField,
            string codeField, string nameField, int defaultValue)
        {
            if (clsRow != null)
            {
                row[idField] = clsRow["ID"];
                row[codeField] = clsRow["CODE"];
                row[nameField] = clsRow["NAME"];
            }
            else
            {
                row[idField] = defaultValue;
                row[codeField] = 0;
                row[nameField] = constDefaultClsName;
            }
        }

        private static DataRow FindRowByPrimarySourceKey(DataTable dt, object sourceKeyValue)
        {
            try
            {
                return dt.Rows.Find(sourceKeyValue);
            }
            catch
            {
                return null;
            }
        }

        private string GetBudgetBudgetString(int refBudget)
        {
            DataRow row = FindCachedRow(budgetBudgetCache, refBudget);
            if (row == null)
                return string.Empty;
            string budgetBudgetString = string.Format("{0}, {1}, {2}", row["Name"], row["currentbudget"], row["OKATO"]);
            string year = row["AYear"].ToString();
            if (year != string.Empty)
                budgetBudgetString += string.Format(", {0}", year);
            return budgetBudgetString;
        }

        private bool IsBudgetPresence(DataRow budgetRow, DataRow clsRow)
        {
            string name = budgetRow["Name"].ToString();
            object year = budgetRow["AYear"];
            if (year.ToString() == string.Empty)
                year = DBNull.Value;
            foreach (DataRow row in clsRow.Table.Rows)
                if (year == DBNull.Value)
                {
                    if ((row["Name"].ToString() == name) && (row["AYear"] == year))
                        return true;
                }
                else
                {
                    if (row["AYear"] != DBNull.Value)
                        if ((row["Name"].ToString() == name) && (Convert.ToInt32(row["AYear"]) == Convert.ToInt32(year)))
                            return true;
                }
            return false;
        }

        /// <summary>
        /// ������������ ������ �������������� �������
        /// </summary>
        /// <param name="budgetRow">������ �������������� �������</param>
        /// <param name="clsRow">������ ������ ��������������</param>
        /// <param name="fieldsMapping">������ ��� ����� (���� �� - ���� �������)
        /// ���� ������� ����� ��������� ������: 
        /// "somestring" - �������� ���� � �������;
        /// "��������� � ������� �������" - �����-�� ���������, �������������� ��� �������� ���� �������������� ������;
        /// "somestring+somestring2" - ���� somestring + ���� somestring2;
        /// "somestring;somestring2" - ���� �������� ���� somestring = null, �� ��������� somestring2</param>
        private void PumpSimpleClsRow(DataRow budgetRow, DataRow clsRow, string[] fieldsMapping)
        {
            string[] fields;

            for (int j = 0; j < fieldsMapping.GetLength(0) - 1; j += 2)
            {
                switch (ParseFieldsMapping(fieldsMapping[j + 1], out fields))
                {
                    case MappedFieldKind.Constant:
                        clsRow[fieldsMapping[j]] = fields[0];
                        break;

                    case MappedFieldKind.FieldName:
                        clsRow[fieldsMapping[j]] = budgetRow[fields[0]];
                        break;

                    case MappedFieldKind.FieldsSum:
                        string value = string.Empty;
                        int count = fields.GetLength(0);
                        for (int k = 0; k < count; k++)
                        {
                            value += budgetRow[fields[k]] + " ";
                        }
                        clsRow[fieldsMapping[j]] = value.Trim();
                        break;

                    case MappedFieldKind.ConstForNull:
                        if (!budgetRow.IsNull(fields[0]) && 
                            GetStringCellValue(budgetRow, fields[0], string.Empty) != string.Empty)
                        {
                            clsRow[fieldsMapping[j]] = budgetRow[fields[0]];
                        }
                        else
                        {
                            clsRow[fieldsMapping[j]] = fields[1];
                        }
                        break;
                }
                if ((fieldsMapping[j].ToUpper() == "NAME") && (clsRow[fieldsMapping[j]].ToString().Trim() == string.Empty))
                    clsRow[fieldsMapping[j]] = constDefaultClsName;
            }
        }

        private bool recordPresence(DataRow budgetRow, DataRow clsRow, string keyFieldName)
        {
            string budValue = budgetRow[keyFieldName].ToString();
            string query = string.Format("{0} = '{1}'", keyFieldName, budValue);
            DataRow[] rows = clsRow.Table.Select(query);
            return (rows.GetLength(0) != 0);
        }

        // � ��������� �� ��� ��������� ������
        private void PumpSimpleClsRowEx(DataRow budgetRow, DataRow clsRow, string[] fieldsMapping)
        {
            string keyFieldName = "Name";
            if (recordPresence(budgetRow, clsRow, keyFieldName))
            {
                fieldsMapping[0] = "fuck888";
                return;
            }
            string[] fields;
            for (int j = 0; j < fieldsMapping.GetLength(0) - 1; j += 2)
            {
                switch (ParseFieldsMapping(fieldsMapping[j + 1], out fields))
                {
                    case MappedFieldKind.Constant:
                        clsRow[fieldsMapping[j]] = fields[0];
                        break;

                    case MappedFieldKind.FieldName:
                        clsRow[fieldsMapping[j]] = budgetRow[fields[0]];
                        break;

                    case MappedFieldKind.FieldsSum:
                        string value = string.Empty;
                        int count = fields.GetLength(0);
                        for (int k = 0; k < count; k++)
                        {
                            value += budgetRow[fields[k]] + " ";
                        }
                        clsRow[fieldsMapping[j]] = value.Trim();
                        break;

                    case MappedFieldKind.ConstForNull:
                        if (!budgetRow.IsNull(fields[0]) &&
                            GetStringCellValue(budgetRow, fields[0], string.Empty) != string.Empty)
                        {
                            clsRow[fieldsMapping[j]] = budgetRow[fields[0]];
                        }
                        else
                        {
                            clsRow[fieldsMapping[j]] = fields[1];
                        }
                        break;
                }
            }
        }

        private void DeleteNullSourceKey(DataTable dt)
        {
            foreach (DataRow row in dt.Rows)
                if (row["SourceKey"].ToString() == string.Empty)
                    row.Delete();
        }

        #endregion ����� ������

        #region ������� ������� ���������������

        private const string BUDGET_CLS_DEFAULT_NAME = "NAME;" + constDefaultClsName;

        private void PumpKvsr()
        {
            if (!toPumpIncomes && !toPumpIncomesPlan && !toPumpOutcomesPlan && !toPumpTreasury &&
                !toPumpFinancing)
                return;
            if (this.MajorDBVersion >= 38)
            {
                PumpBudgetCls("KVSR", dsKVSR.Tables[0], clsKVSR,
                    "Cast(ID as Varchar(10)) ID, Cast(Code as Varchar(10)) Code, Name, Cast(BudgetRef as Varchar(10)) BudgetRef, CREATEDATE, UPDATEDATE",
                    string.Empty, string.Empty, new ProcessClsRowDelegate(PumpSimpleClsRow),
                    new string[] { "CODE", "Code", "NAME", BUDGET_CLS_DEFAULT_NAME, "Budget", "BudgetRef" }, ref kvsrCache);
                foreach (DataRow row in dsKVSR.Tables[0].Rows)
                    row["Budget"] = GetBudgetBudgetString(GetIntCellValue(row, "Budget", 0));
            }
            else
            {
                PumpBudgetCls("KVSR", dsKVSR.Tables[0], clsKVSR, "Cast(ID as Varchar(10)) ID, Name, CREATEDATE, UPDATEDATE",
                    string.Empty, string.Empty, new ProcessClsRowDelegate(PumpSimpleClsRow),
                    new string[] { "CODE", "ID", "NAME", BUDGET_CLS_DEFAULT_NAME }, ref kvsrCache);
            }
            dsKVSR.Tables[0].PrimaryKey = new DataColumn[] { dsKVSR.Tables[0].Columns["SourceKey"] };
        }

        private void PumpRegion()
        {
            if (!toPumpIncomes && !toPumpIncomesPlan && !toPumpOutcomesPlan && !toPumpTreasury &&
                !toPumpFinancing && !toPumpAccountOperations && !toPumpIFPlan && !toPumpIFFact)
                return;
            PumpBudgetCls("REGIONCLS", dsRegions.Tables[0], clsRegions,
                "Cast(ID as Varchar(10)) ID, Name, Cast(BUDGETTYPE as Varchar(100)) BUDGETTYPE, " +
                "Cast(KVB_EXPORT as Varchar(100)) KVB_EXPORT, Cast(ORG_EXPORT as Varchar(100)) ORG_EXPORT, CREATEDATE, UPDATEDATE",
                string.Empty, string.Empty, new ProcessClsRowDelegate(PumpSimpleClsRow), new string[] { 
                    "CODE", "ID", "NAME", BUDGET_CLS_DEFAULT_NAME, "BUDGETTYPE", "BUDGETTYPE", 
                    "KVB_EXPORT", "KVB_EXPORT;0", "ORG_EXPORT", "ORG_EXPORT;0"}, ref regionsCache);
            DeleteNullSourceKey(dsRegions.Tables[0]);
            dsRegions.Tables[0].PrimaryKey = new DataColumn[] { dsRegions.Tables[0].Columns["SourceKey"] };
        }

        private void PumpBudgetAccounts()
        {
            if (!toPumpIncomes)
                return;
            Dictionary<int, DataRow> dict = null;
            PumpBudgetCls("BUDGETACCOUNTS", dsBudgetAccounts.Tables[0], clsBudgetAccounts,
                "Cast(ID as Varchar(10)) ID, Name, CREATEDATE, UPDATEDATE",       
                string.Empty, string.Empty, new ProcessClsRowDelegate(PumpSimpleClsRow),
                new string[] { "CODE", "ID", "NAME", BUDGET_CLS_DEFAULT_NAME }, ref dict);
        }

        private void PumpNotifyTypes()
        {
            if (!toPumpOutcomesPlan && !toPumpTreasury && !toPumpFinancing && !toPumpIFPlan)
                return;
            Dictionary<int, DataRow> dict = null;
            PumpBudgetCls("NOTIFYTYPES", dsNotifyTypes.Tables[0], clsNotifyTypes,
                "Cast(ID as Varchar(10)) ID, Name, CREATEDATE, UPDATEDATE",
                string.Empty, string.Empty, new ProcessClsRowDelegate(PumpSimpleClsRow),
                new string[] { "CODE", "ID", "NAME", BUDGET_CLS_DEFAULT_NAME }, ref dict);
        }

        private void PumpSubKesr()
        {
            if (!toPumpOutcomesPlan && !toPumpTreasury && !toPumpFinancing && !toPumpIFPlan)
                return;
            PumpBudgetCls("KESRSUBCODE", dsSubKESR.Tables[0], clsSubKESR,
                "Cast(ID as Varchar(10)) ID, Name, CREATEDATE, UPDATEDATE",
                string.Empty, string.Empty, new ProcessClsRowDelegate(PumpSimpleClsRow),
                new string[] { "CODE", "ID", "NAME", BUDGET_CLS_DEFAULT_NAME }, ref subEkrCache);
        }

        private void PumpEkr()
        {
            if (!toPumpIncomes && !toPumpOutcomesPlan && !toPumpTreasury && !toPumpFinancing)
                return;
            if (this.BudgetYear < 2005)
            {
                PumpBudgetCls("KESR", dsEKR.Tables[0], clsEKR,
                    "Cast(ID as Varchar(10)) ID, Name, CREATEDATE, UPDATEDATE",
                    string.Empty, string.Empty, new ProcessClsRowDelegate(PumpSimpleClsRow),
                    new string[] { "CODE", "ID", "NAME", BUDGET_CLS_DEFAULT_NAME }, ref ekrCache);
                DeleteNullSourceKey(dsEKR.Tables[0]);
              //  dsEKR.Tables[0].PrimaryKey = new DataColumn[] { dsEKR.Tables[0].Columns["SourceKey"] };
            }
            else
            {
                PumpBudgetCls("KESR", dsEKR.Tables[0], clsEKR,
                    "Cast(ID as Varchar(10)) ID, Name, CREATEDATE, UPDATEDATE",
                    string.Empty, "ISEKR = 1", new ProcessClsRowDelegate(PumpSimpleClsRow),
                    new string[] { "CODE", "ID", "NAME", BUDGET_CLS_DEFAULT_NAME }, ref ekrCache);
                DeleteNullSourceKey(dsEKR.Tables[0]);
             //   dsEKR.Tables[0].PrimaryKey = new DataColumn[] { dsEKR.Tables[0].Columns["SourceKey"] };
            }
        }

        private void PumpFkr()
        {
            if (!toPumpIncomes && !toPumpOutcomesPlan && !toPumpTreasury && !toPumpFinancing)
                return;
            PumpBudgetCls("KFSR", dsFKR.Tables[0], clsFKR,
                "Cast(ID as Varchar(10)) ID, Name, CREATEDATE, UPDATEDATE",
                string.Empty, string.Empty, new ProcessClsRowDelegate(PumpSimpleClsRow),
                new string[] { "CODE", "ID", "NAME", BUDGET_CLS_DEFAULT_NAME }, ref fkrCache);
            DeleteNullSourceKey(dsFKR.Tables[0]);
            dsFKR.Tables[0].PrimaryKey = new DataColumn[] { dsFKR.Tables[0].Columns["SourceKey"] };
        }

        private void PumpKcsr()
        {
            if (!toPumpIncomes && !toPumpOutcomesPlan && !toPumpTreasury && !toPumpFinancing)
                return;
            if (this.MajorDBVersion >= 38)
            {
                PumpBudgetCls("KCSR", dsKCSR.Tables[0], clsKCSR,
                    "Cast(ID as Varchar(10)) ID, Name, Cast(BudgetRef as Varchar(10)) BudgetRef, Cast(Code as Varchar(10)) Code, CREATEDATE, UPDATEDATE",
                    string.Empty, string.Empty, new ProcessClsRowDelegate(PumpSimpleClsRow),
                    new string[] { "CODE", "Code", "NAME", BUDGET_CLS_DEFAULT_NAME, "Budget", "BudgetRef" }, ref kcsrCache);
                foreach (DataRow row in dsKCSR.Tables[0].Rows)
                    row["Budget"] = GetBudgetBudgetString(GetIntCellValue(row, "Budget", 0));
            }
            else
            {
                PumpBudgetCls("KCSR", dsKCSR.Tables[0], clsKCSR,
                    "Cast(ID as Varchar(10)) ID, Name, CREATEDATE, UPDATEDATE",
                    string.Empty, string.Empty, new ProcessClsRowDelegate(PumpSimpleClsRow),
                    new string[] { "CODE", "ID", "NAME", BUDGET_CLS_DEFAULT_NAME }, ref kcsrCache);
            }
        }

        private void PumpEkd()
        {
            if (!toPumpIncomes && !toPumpIncomesPlan)
                return;
            if (this.BudgetYear < 2005)
                return;
            Dictionary<int, DataRow> dict = null;
            PumpBudgetCls("KESR", dsEKD.Tables[0], clsEKD,
                "Cast(ID as Varchar(10)) ID, Name, CREATEDATE, UPDATEDATE",
                string.Empty, "ISEKD = 1", new ProcessClsRowDelegate(PumpSimpleClsRow),
                new string[] { "CODE", "ID", "NAME", BUDGET_CLS_DEFAULT_NAME }, ref dict);
        }

        private void PumpFact()
        {
            if (!toPumpOutcomesPlan && !toPumpTreasury && !toPumpFinancing && !toPumpIFPlan)
                return;
            PumpBudgetCls("FACT", dsFact.Tables[0], clsFact,
                "Cast(ID as Varchar(10)) ID, Name, CREATEDATE, UPDATEDATE",
                string.Empty, string.Empty, new ProcessClsRowDelegate(PumpSimpleClsRow),
                new string[] { "CODE", "ID", "NAME", BUDGET_CLS_DEFAULT_NAME }, ref factClsCache);
        }

        private void PumpMeansType()
        {
            if (!toPumpIncomes && !toPumpIncomesPlan && !toPumpOutcomesPlan && !toPumpTreasury &&
                !toPumpFinancing && !toPumpAccountOperations && !toPumpIFPlan && !toPumpIFFact)
                return;
            if (this.MajorDBVersion <= 31)
            {
                PumpBudgetCls("MEANSTYPE", dsMeansType.Tables[0], clsMeansType,
                    "Cast(ID as Varchar(10)) ID, Name, Cast(ACTIVITYKIND as Varchar(10)) ACTIVITYKIND, CREATEDATE, UPDATEDATE",
                    string.Empty, string.Empty, new ProcessClsRowDelegate(PumpSimpleClsRow),
                    new string[] { "CODE", "ID", "NAME", BUDGET_CLS_DEFAULT_NAME, "ACTIVITYKIND", "<0>" }, ref meansTypeCache);
                DeleteNullSourceKey(dsMeansType.Tables[0]);
                dsMeansType.Tables[0].PrimaryKey = new DataColumn[] { dsMeansType.Tables[0].Columns["SourceKey"] };
            }
            else
            {
                PumpBudgetCls("MEANSTYPE", dsMeansType.Tables[0], clsMeansType,
                    "Cast(ID as Varchar(10)) ID, Name, Cast(ACTIVITYKIND as Varchar(10)) ACTIVITYKIND, CREATEDATE, UPDATEDATE",
                    string.Empty, string.Empty, new ProcessClsRowDelegate(PumpSimpleClsRow),
                    new string[] { "CODE", "ID", "NAME", BUDGET_CLS_DEFAULT_NAME, "ACTIVITYKIND", "ACTIVITYKIND" }, ref meansTypeCache);
                DeleteNullSourceKey(dsMeansType.Tables[0]);
                dsMeansType.Tables[0].PrimaryKey = new DataColumn[] { dsMeansType.Tables[0].Columns["SourceKey"] };
            }
        }

        private void CheckKvrTable(string codeFieldName)
        {
            string query = string.Format("select count(*) from KVR where {0} >= 1000", codeFieldName);
            int count = Convert.ToInt32(this.BudgetDB.ExecQuery(query, QueryResultTypes.Scalar, new IDbDataParameter[] { }));
            if (count != 0)
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning,
                    "� �������������� ��� ���������� ������, � ������� ����� ���� ������ 3 ��������. ������ ������ �������� �� �����.");
        }

        private void PumpKvr()
        {
            if (!toPumpIncomes && !toPumpOutcomesPlan && !toPumpTreasury && !toPumpFinancing)
                return;
            if (this.MajorDBVersion <= 30)
            {
                PumpBudgetCls("KVR", dsKVR.Tables[0], clsKVR, "Cast(ID as Varchar(10)) ID, Name, CREATEDATE, UPDATEDATE",
                    string.Empty, string.Empty, new ProcessClsRowDelegate(PumpSimpleClsRow),
                    new string[] { "CODE", "Id", "NAME", BUDGET_CLS_DEFAULT_NAME }, ref kvrCache);
                DeleteNullSourceKey(dsKVR.Tables[0]);
                dsKVR.Tables[0].PrimaryKey = new DataColumn[] { dsKVR.Tables[0].Columns["SourceKey"] };
            }
            else
            {
                if (this.MajorDBVersion >= 38)
                {
                    PumpBudgetCls("KVR", dsKVR.Tables[0], clsKVR,
                        "Cast(ID as Varchar(10)) ID, Cast(Code as Varchar(10)) Code, Name, Cast(BudgetRef as Varchar(10)) BudgetRef, CREATEDATE, UPDATEDATE",
                        string.Empty, " code < 1000 ", new ProcessClsRowDelegate(PumpSimpleClsRow),
                        new string[] { "CODE", "Code", "NAME", "NAME", "Budget", "BudgetRef" }, ref kvrCache);
                    CheckKvrTable("Code");
                    DeleteNullSourceKey(dsKVR.Tables[0]);
                    foreach (DataRow row in dsKVR.Tables[0].Rows)
                        row["Budget"] = GetBudgetBudgetString(GetIntCellValue(row, "Budget", 0));
                }
                else
                {
                    PumpBudgetCls("KVR", dsKVR.Tables[0], clsKVR, "Cast(ID as Varchar(10)) ID, Name, CREATEDATE, UPDATEDATE",
                        string.Empty, " id < 1000 ", new ProcessClsRowDelegate(PumpSimpleClsRow),
                        new string[] { "CODE", "ID", "NAME", "NAME" }, ref kvrCache);
                    CheckKvrTable("Id");
                    DeleteNullSourceKey(dsKVR.Tables[0]);
                }
                dsKVR.Tables[0].PrimaryKey = new DataColumn[] { dsKVR.Tables[0].Columns["SourceKey"] };
            }
        }

        private void PumpProgram()
        {
            if (!toPumpIncomes && !toPumpIncomesPlan)
                return;
            Dictionary<int, DataRow> dict = null;
            PumpBudgetCls("PROGRAMCODE", dsPrograms.Tables[0], clsPrograms,
                "Cast(ID as Varchar(10)) ID, Name, CREATEDATE, UPDATEDATE",
                string.Empty, string.Empty, new ProcessClsRowDelegate(PumpSimpleClsRow),
                new string[] { "CODE", "ID", "NAME", BUDGET_CLS_DEFAULT_NAME }, ref dict);
        }

        private void PumpDirection()
        {
            if (!toPumpIncomes && !toPumpTreasury && !toPumpFinancing && !toPumpOutcomesPlan)
                return;
            PumpBudgetCls("DIRECTIONCLS", dsDirection.Tables[0], clsDirection,
                "Cast(ID as Varchar(10)) ID, Name, CREATEDATE, UPDATEDATE",
                string.Empty, " id < 1000 ", new ProcessClsRowDelegate(PumpSimpleClsRow),
                new string[] { "CODE", "ID", "NAME", BUDGET_CLS_DEFAULT_NAME }, ref directionCache);
        }

        #region budget

        private void PumpBudgets(DataRow budgetRow, DataRow clsRow, string[] fieldsMapping)
        {
            if (IsBudgetPresence(budgetRow, clsRow))
            {
                fieldsMapping[0] = "fuck888";
                return;
            }
            clsRow["Name"] = budgetRow["Name"];
            clsRow["CurrentBudget"] = budgetRow["CurrentBudget"];
            clsRow["AddOKATO"] = budgetRow["AddOKATO"];
            clsRow["Note"] = budgetRow["Note"];
            clsRow["AYear"] = budgetRow["AYear"];
            string dateStr = budgetRow["LastModifdate_b"].ToString().Split('.')[0];
            if (dateStr != string.Empty)
            {
                int year = Convert.ToInt32(dateStr.Substring(0, 4));
                int month = Convert.ToInt32(dateStr.Substring(4, 2));
                int day = Convert.ToInt32(dateStr.Substring(6, 2));
                DateTime date = new DateTime(year, month, day);
                clsRow["LastModifdate"] = date;
            }
            clsRow["OKATO"] = "0";
        }

        private void PumpBudgetBudget()
        {
            if (!toPumpIncomes && !toPumpOutcomesPlan && !toPumpTreasury && !toPumpIFPlan && !toPumpIFFact)
                return;
            PumpBudgetCls("Budgets_s", dsBudgetBudget.Tables[0], clsBudgetBudget,
                "Cast(ID as Varchar(10)) ID, Name, Cast(CurrentBudget as Varchar(10)) CurrentBudget, Cast(AddOKATO as Varchar(10)) AddOKATO, " +
                "Cast(AYear as Varchar(10)) AYear, Note, LastModifdate_b,  Cast(OKATO as Varchar(10)) OKATO",
                string.Empty, string.Empty, new ProcessClsRowDelegate(PumpBudgets), 
                new string[] { "Name", "Name" }, ref budgetBudgetCache);
        }

        #endregion budget

        private void PumpFundsSource()
        {
            if (!toPumpIncomes && !toPumpIncomesPlan && !toPumpOutcomesPlan &&
                !toPumpTreasury && !toPumpAccountOperations && !toPumpIFPlan && !toPumpIFFact)
                return;
            PumpBudgetCls("FundsSource", dsFundsSource.Tables[0], clsFundsSource, string.Empty, string.Empty,
                new ProcessClsRowDelegate(PumpSimpleClsRowEx), new string[] { "Name", "Name" }, ref fundsSourceCache);
        }

        private void PumpAsgmtKind()
        {
            if (!toPumpIncomesPlan && !toPumpOutcomesPlan && !toPumpIFPlan)
                return;
            PumpBudgetCls("AssignmentKind", dsAsgmtKind.Tables[0], clsAsgmtKind, string.Empty, string.Empty,
                new ProcessClsRowDelegate(PumpSimpleClsRowEx), new string[] { "Name", "Name" }, ref asgmtKindCache);
        }

        private void PumpAsgmtSrce()
        {
            if (!toPumpIncomesPlan && !toPumpOutcomesPlan && !toPumpIFPlan)
                return;
            PumpBudgetCls("AssignmentSource", dsAsgmtSrce.Tables[0], clsAsgmtSrce, string.Empty, string.Empty,
                new ProcessClsRowDelegate(PumpSimpleClsRowEx), new string[] { "Name", "Name" }, ref asgmtSrceCache);
        }

        private void PumpBuhOperations()
        {
            if (!toPumpIncomes)
                return;
            PumpBudgetCls("BuhOperationCLS", dsBuhOperations.Tables[0], clsBuhOperations,
                "Cast(ID as Varchar(10)) ID, Name, CREATEDATE, UPDATEDATE",
                string.Empty, string.Empty, new ProcessClsRowDelegate(PumpSimpleClsRowEx), 
                new string[] { "Code", "Id", "Name", "Name" }, ref buhOperationsCache);
        }

        private void PumpKD()
        {
            if (!toPumpIncomes && !toPumpIncomesPlan)
                return;

            int kdFirstRecCount = 0;
            int kdRemovedRecsCount = 0;
            int kdAddedRecsCount = 0;
            int kdUpdatedRecsCount = 0;
            int kdSkippedRecsCount = 0;
            int kindsFirstRecCount = 0;
            int kindsRemovedRecsCount = 0;
            int kindsAddedRecsCount = 0;
            int kindsUpdatedRecsCount = 0;
            bool isAdded;
            DataRow row;
            DataRow[] rows;
            string kdName;
            DataRow kdRow;

            try
            {
                SetProgress(-1, -1, "��������� ������ ��...", string.Empty);
                WriteToTrace("��������� ������ ��...", TraceMessageKind.Information);

                CheckBadKD();

                ClearDataSet(ref dsBudgetFacts);

                string kdNames = string.Empty;
                if (this.MajorDBVersion < 38)
                    kdNames = " Name2, Name3, ";
                string adQuery = string.Format("select Cast(ID as Varchar(10)) ID, Name, {0} CREATEDATE, UPDATEDATE, Cast(UNIONFLAG as Varchar(10)) UNIONFLAG, " +
                    "  KDVALUE, Cast(ITEMCODE as Varchar(30)) ITEMCODE, Cast(PROGRAMCODE as Varchar(30)) PROGRAMCODE, " +
                    "  Cast(DESCRIPTIONCODE as Varchar(30)) DESCRIPTIONCODE, Cast(KESR as Varchar(30)) KESR, " +
                    "  Cast(KVSR as Varchar(30)) KVSR, Cast(MAINDESCRIPTIONCODE as Varchar(30)) MAINDESCRIPTIONCODE, " +
                    "  Cast(BudgetRef as Varchar(30)) BudgetRef from KD", kdNames);
                InitLocalDataAdapter(this.BudgetDB, ref daBudgetCls, adQuery);
                daBudgetCls.Fill(dsBudgetFacts);
                DataTable sourceTable = dsBudgetFacts.Tables[0];

                kdFirstRecCount = dsKD.Tables[0].Rows.Count;
                RemoveAbsentRecsFromCls("KD", sourceTable, dsKD.Tables[0], out kdRemovedRecsCount);
                FillRowsCache(ref clsCache, dsKD.Tables[0], new string[] { "SOURCEKEY" });

                kindsFirstRecCount = dsIncomesKinds.Tables[0].Rows.Count;

                // ������� �� �������������� ������ ��� ������, ������� ������ ��� � �������� ������� �� ������
                switch (this.CurrentDBVersion)
                {
                    case "27.02":
                    case "28.00":
                    case "29.01":
                    case "29.02":
                    case "30.00":
                    case "30.01":
                    case "31.00":
                        break;

                    default:
                        RemoveAbsentRecsFromCls("KD", sourceTable, dsIncomesKinds.Tables[0], out kindsRemovedRecsCount);
                        break;
                }

                // ������������ ����������� ��� �������� � ����������� �� ������ �������
                string dateConstr = string.Empty;
                if (sourceTable.Columns.Contains("CREATEDATE") && sourceTable.Columns.Contains("UPDATEDATE"))
                {
                    dateConstr = GetDateConstrForDataSet();
                }
                rows = sourceTable.Select(dateConstr);

                int count = rows.GetLength(0);

                ClassTypes clsKDClassType = clsKD.ClassType;
                string clsKDGeneratorName = clsKD.GeneratorName;

                string kdItemCodeList = string.Empty;
                for (int i = 0; i < count; i++)
                {
                    if (this.BudgetYear < 2005)
                    {
                        kdRow = GetRowForUpdate(Convert.ToInt32(rows[i]["ID"]), dsKD.Tables[0], clsCache,
                            clsKDClassType, clsKDGeneratorName, out isAdded);
                        if (isAdded)
                        {
                            kdAddedRecsCount++;
                        }
                        else
                        {
                            kdUpdatedRecsCount++;
                        }

                        kdName = GetStringCellValue(rows[i], "NAME", "����������� ��� ������");

                        kdRow["SOURCEKEY"] = rows[i]["ID"];
                        kdRow["CODE"] = rows[i]["ID"];
                        kdRow["NAME"] = kdName;
                    }
                    else
                    {
                        // ��� ������� ������ ����������� �� ������ KD, � ������� UnionFlag = 1
                        if (GetIntCellValue(rows[i], "UNIONFLAG", -1) == 1)
                        {
                            kdSkippedRecsCount++;
                            continue;
                        }

                        if (this.MajorDBVersion >= 38)
                            kdName = rows[i]["NAME"].ToString().Trim();
                        else
                            kdName = string.Format("{0} {1} {2}", rows[i]["NAME"], rows[i]["NAME2"], rows[i]["NAME3"]).Trim();
                        if (kdName == string.Empty)
                        {
                            kdName = "����������� ��� ������";
                        }

                        // ��������� ��
                        kdRow = GetRowForUpdate(Convert.ToInt32(rows[i]["ID"]), dsKD.Tables[0],
                            clsCache, clsKDClassType, clsKDGeneratorName, out isAdded);
                        if (isAdded)
                        {
                            kdAddedRecsCount++;
                        }
                        else
                        {
                            kdUpdatedRecsCount++;
                        }

                        kdRow["SOURCEKEY"] = rows[i]["ID"];
                        kdRow["CODESTR"] = rows[i]["KDVALUE"];
                        kdRow["NAME"] = kdName;

                        int itemCode = Convert.ToInt32(rows[i]["ITEMCODE"]);
                        if ((itemCode < 1) || (itemCode > 10))
                        {
                            itemCode = -1;
                            kdItemCodeList += string.Format("{0}; ", rows[i]["ID"].ToString());
                        }
                        kdRow["ITEMCODE"] = itemCode;

                        kdRow["PROGRAMCODE"] = FindRowID(dsPrograms.Tables[0],
                            new object[] { "SOURCEKEY", rows[i]["PROGRAMCODE"] }, nullPrograms);
                        kdRow["DESCRIPTIONCODE"] = Convert.ToDecimal(rows[i]["DESCRIPTIONCODE"].ToString().Replace('.', ','));
                        kdRow["KESR"] = rows[i]["KESR"];
                        kdRow["KVSR"] = FindRowID(dsKVSR.Tables[0],
                            new object[] { "SOURCEKEY", rows[i]["KVSR"] }, nullKVSR);
                    }

                    switch (this.CurrentDBVersion)
                    {
                        case "27.02":
                        case "28.00":
                        case "29.01":
                        case "29.02":
                        case "30.00":
                        case "30.01":
                        case "31.00":
                            break;

                        default:
                            // ��������� ���� �������
                            decimal kindsValue = Convert.ToDecimal(GetStringCellValue(rows[i], "DESCRIPTIONCODE", string.Empty).Replace('.', ','));

                            // ���� ������ �� ���������� valuesMapping
                            row = FindRow(dsIncomesKinds.Tables[0], new object[] { "CODE", kindsValue, "NAME", kdName });
                            if (row == null)
                            {
                                PumpRow(clsIncomesKinds, dsIncomesKinds.Tables[0], new object[] { 
                                    "SOURCEKEY", rows[i]["ID"], "CODE", kindsValue, "NAME", kdName, 
                                    "MAINDESCRIPTIONCODE", rows[i]["MAINDESCRIPTIONCODE"], 
                                    "ITEMCODE", rows[i]["ITEMCODE"] }, true);
                                kindsAddedRecsCount++;
                            }
                            else if (this.PumpMode == BudgetDataPumpMode.FullFact ||
                                this.PumpMode == BudgetDataPumpMode.Update)
                            {
                                row["SOURCEKEY"] = rows[i]["ID"];
                                row["CODE"] = kindsValue;
                                row["NAME"] = kdName;
                                row["MAINDESCRIPTIONCODE"] = rows[i]["MAINDESCRIPTIONCODE"];
                                row["ITEMCODE"] = rows[i]["ITEMCODE"];
                                kindsUpdatedRecsCount++;
                            }

                            break;
                    }

                    if (MajorDBVersion >= 35)
                        kdRow["Budget"] = GetBudgetBudgetString(GetIntCellValue(rows[i], "BudgetRef", 0));

                    SetProgress(rows.GetLength(0), i + 1, "��������� ������ ��...",
                        string.Format("������ {0} �� {1}", i + 1, rows.GetLength(0)));
                }

                if (kdItemCodeList != string.Empty)
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning,
                        string.Format("� ���� �� '������' � �������������� '��� ������' (KD) ���� ������ �� ��������� ���� '�������' (ItemCode) �� ������ 01-10. " + 
                                      "��� ����, ����������� �� ��� ������, �� ����� ���������� �������� '������� �������'. " + 
                                      "������ ID ������������ ������� �������������� '��� ������' � ���� �� '������': {0}", kdItemCodeList));

                FillRowsCache(ref kdCache, dsKD.Tables[0], "SOURCEKEY");

                FillRowsCache(ref incomesKindsCache, dsIncomesKinds.Tables[0], new string[] { "CODE", "NAME" });

                WriteToTrace("������ �� ����������.", TraceMessageKind.Information);

                // ���������� � ��������, ����� ������������� ������ �������, ������� ������� � �������������� ������ 
                // ����, ������� �����, ������� �������, ��������� � ���������
                WriteEventIntoDataPumpProtocol(
                    DataPumpEventKind.dpeInformation, string.Format(
                        "��������� ������������� ��. �������� ���������� ������� ��������������: {0}, " +
                        "�������: {1}, ���������: {2}, ���������: {3}, ��������� ������������ �������: {4}.",
                        kdFirstRecCount, kdRemovedRecsCount, kdAddedRecsCount, kdUpdatedRecsCount,
                        kdSkippedRecsCount));
                WriteEventIntoDataPumpProtocol(
                    DataPumpEventKind.dpeInformation, string.Format(
                        "��������� ������������� ���� �������. �������� ���������� ������� ��������������: {0}, " +
                        "�������: {1}, ���������: {2}, ���������: {3}, ��������� ������������ �������: {4}.",
                        kindsFirstRecCount, kindsRemovedRecsCount, kindsAddedRecsCount, kindsUpdatedRecsCount,
                        kdSkippedRecsCount));
            }
            catch (Exception ex)
            {
                // ���������� � ��������, ����� ������������� ������ �������, ������� ������� � �������������� ������ 
                // ����, ������� �����, ������� �������, ��������� � ���������
                WriteEventIntoDataPumpProtocol(
                    DataPumpEventKind.dpeError, string.Format(
                        "������������� �� ��������� � ��������. �� ������ ������������� ������ ���������� " +
                        "��������� ����������. �������� ���������� ������� ��������������: {0}, " +
                        "�������: {1}, ���������: {2}, ���������: {3}, ��������� ������������ �������: {4}. ������ �� ���������.",
                        kdFirstRecCount, kdRemovedRecsCount, kdAddedRecsCount, kdUpdatedRecsCount,
                        kdSkippedRecsCount), ex);
                WriteEventIntoDataPumpProtocol(
                    DataPumpEventKind.dpeError, string.Format(
                        "������������� ���� ������� ��������� � ��������. �� ������ ������������� ������ ���������� " +
                        "��������� ����������. �������� ���������� ������� ��������������: {0}, " +
                        "�������: {1}, ���������: {2}, ���������: {3}, ��������� ������������ �������: {4}. ������ �� ���������.",
                        kindsFirstRecCount, kindsRemovedRecsCount, kindsAddedRecsCount,
                        kindsUpdatedRecsCount, kdSkippedRecsCount), ex);
                throw;
            }
        }

        private void PumpKif()
        {
            if (!toPumpFinancing && !toPumpIFPlan && !toPumpIFFact)
                return;

            switch (this.CurrentDBVersion)
            {
                case "27.02":
                case "28.00":
                case "29.01":
                case "29.02":
                case "30.00":
                case "30.01":
                    PumpBudgetCls("INNERFINSOURCE", dsKIF2004.Tables[0], clsKIF2004, string.Empty, string.Empty,
                        new ProcessClsRowDelegate(PumpSimpleClsRow),
                        new string[] { "CODE", "ID", "NAME", BUDGET_CLS_DEFAULT_NAME }, ref ifsCache);
                    break;
                case "31.00":
                case "31.01":
                    PumpBudgetCls("INNERFINSOURCE", dsKIF2005.Tables[0], clsKIF2005, string.Empty, string.Empty,
                        new ProcessClsRowDelegate(PumpSimpleClsRow), new string[] { 
                            "CODESTR", "FINSOURCEVALUE", "NAME", BUDGET_CLS_DEFAULT_NAME, "REFDIRECTION", "DIRECTION",
                            "KVSR", "KVSR", "DESCRIPTIONCODE", "DESCRIPTIONCODE", 
                            "MAINDESCRIPTIONCODE", "MAINDESCRIPTIONCODE", "ITEMCODE", "ITEMCODE",
                            "PROGRAMCODE", "PROGRAMCODE", "KESR", "KESR", "REFCLSASPECT", "CLSASPECT" },
                        ref ifsCache);
                    break;
                default:
                    if (MajorDBVersion >= 35)
                    {
                        string fields = "Cast(ID as Varchar(10)) ID, Name, FINSOURCEVALUE, Cast(UNIONFLAG as Varchar(10)) UNIONFLAG, " +
                            "Cast(DIRECTION as Varchar(30)) DIRECTION, Cast(KVSR as Varchar(30)) KVSR, " +
                            "DESCRIPTIONCODE, Cast(ITEMCODE as Varchar(30)) ITEMCODE, " +
                            "Cast(KESR as Varchar(30)) KESR, Cast(MAINDESCRIPTIONCODE as Varchar(30)) MAINDESCRIPTIONCODE, " +
                            "Cast(BudgetRef as Varchar(30)) BudgetRef, Cast(PROGRAMCODE as Varchar(30)) PROGRAMCODE, " +
                            "Cast(CLSASPECT as Varchar(30)) CLSASPECT";

                        PumpBudgetCls("INNERFINSOURCE", dsKIF2005.Tables[0], clsKIF2005, fields, string.Empty, "UNIONFLAG = 0",
                            new ProcessClsRowDelegate(PumpSimpleClsRow), new string[] { 
                            "CODESTR", "FINSOURCEVALUE", "NAME", "NAME", "REFDIRECTION", "DIRECTION",
                            "KVSR", "KVSR", "DESCRIPTIONCODE", "DESCRIPTIONCODE", 
                            "MAINDESCRIPTIONCODE", "MAINDESCRIPTIONCODE", "ITEMCODE", "ITEMCODE",
                            "PROGRAMCODE", "PROGRAMCODE", "KESR", "KESR", "REFCLSASPECT", "CLSASPECT", "Budget", "BudgetRef" },
                            ref ifsCache);
                        foreach (DataRow kifRow in dsKIF2005.Tables[0].Rows)
                            kifRow["Budget"] = GetBudgetBudgetString(GetIntCellValue(kifRow, "Budget", 0));
                    }
                    else
                    {
                        string fields = "Cast(ID as Varchar(10)) ID, Name, FINSOURCEVALUE, Cast(UNIONFLAG as Varchar(10)) UNIONFLAG, " +
                            "Cast(DIRECTION as Varchar(30)) DIRECTION, Cast(KVSR as Varchar(30)) KVSR, " +
                            "Cast(DESCRIPTIONCODE as Varchar(30)) DESCRIPTIONCODE, Cast(ITEMCODE as Varchar(30)) ITEMCODE, " +
                            "Cast(KESR as Varchar(30)) KESR, Cast(MAINDESCRIPTIONCODE as Varchar(30)) MAINDESCRIPTIONCODE, " +
                            "Cast(PROGRAMCODE as Varchar(30)) PROGRAMCODE, " +
                            "Cast(CLSASPECT as Varchar(30)) CLSASPECT";

                        PumpBudgetCls("INNERFINSOURCE", dsKIF2005.Tables[0], clsKIF2005, fields, string.Empty, "UNIONFLAG = 0",
                            new ProcessClsRowDelegate(PumpSimpleClsRow), new string[] { 
                            "CODESTR", "FINSOURCEVALUE", "NAME", BUDGET_CLS_DEFAULT_NAME, "REFDIRECTION", "DIRECTION",
                            "KVSR", "KVSR", "DESCRIPTIONCODE", "DESCRIPTIONCODE", 
                            "MAINDESCRIPTIONCODE", "MAINDESCRIPTIONCODE", "ITEMCODE", "ITEMCODE",
                            "PROGRAMCODE", "PROGRAMCODE", "KESR", "KESR", "REFCLSASPECT", "CLSASPECT"},
                            ref ifsCache);
                    }
                    break;
            }
        }

        #region facialAcc

        /// <summary>
        /// ������������ ������ �������������� �������
        /// </summary>
        /// <param name="budgetRow">������ �������������� �������</param>
        /// <param name="clsRow">������ ������ ��������������</param>
        /// <param name="fieldsMapping">������ ��� ����� (���� �� - ���� �������)
        /// ���� ������� ����� ��������� ������: 
        /// "somestring" - �������� ���� � �������;
        /// "��������� � ������� �������" - �����-�� ���������, �������������� ��� �������� ���� �������������� ������;
        /// "somestring+somestring2" - ���� somestring + ���� somestring2;
        /// "somestring;somestring2" - ���� �������� ���� somestring = null, �� ��������� somestring2</param>
        private void PumpFacialAcc(DataRow budgetRow, DataRow clsRow, string[] fieldsMapping)
        {
            clsRow["CODE"] = budgetRow["ID"];
            clsRow["NAME"] = budgetRow["NAME"];
            if (clsRow["NAME"].ToString().Trim() == string.Empty)
                clsRow["NAME"] = constDefaultClsName;
            //DataRow findedRow = FindRow(dsKVSR.Tables[0], new object[] { "SOURCEKEY", budgetRow["KVSR"] });
            DataRow findedRow = FindRowByPrimarySourceKey(dsKVSR.Tables[0], budgetRow["KVSR"]);
            SetClsFieldsByRow(clsRow, findedRow,
                "KVSRID", "KVSRCODE", "KVSRNAME", nullKVSR);

            findedRow = FindRowByPrimarySourceKey(dsFKR.Tables[0], budgetRow["KFSR"]);
            SetClsFieldsByRow(clsRow, findedRow, "KFSRID", "KFSRCODE", "KFSRNAME", nullFKR);

            findedRow = FindRowByPrimarySourceKey(dsEKR.Tables[0], budgetRow["KESR"]);
            SetClsFieldsByRow(clsRow, findedRow, "KESRID", "KESRCODE", "KESRNAME", nullEKR);

            SetClsFieldsByRow(clsRow, FindCachedRow(kcsrCache, GetIntCellValue(budgetRow, "KCSR", 0)),
                "KCSRID", "KCSRCODE", "KCSRNAME", nullKCSR);

            //findedRow = FindRow(dsKVR.Tables[0], new object[] { "SOURCEKEY", budgetRow["KVR"] });
            findedRow = FindRowByPrimarySourceKey(dsKVR.Tables[0], budgetRow["KVR"]);
            SetClsFieldsByRow(clsRow, findedRow,
                "KVRID", "KVRCODE", "KVRNAME", nullKVR);

            //findedRow = FindRow(dsRegions.Tables[0], new object[] { "SOURCEKEY", budgetRow["REGIONCLS"] });
            findedRow = FindRowByPrimarySourceKey(dsRegions.Tables[0], budgetRow["REGIONCLS"]);
            SetClsFieldsByRow(clsRow, findedRow,
                "REGIONCLSID", "REGIONCLSCODE", "REGIONCLSNAME", nullRegions);

            //findedRow = FindRow(dsMeansType.Tables[0], new object[] { "SOURCEKEY", budgetRow["MEANSTYPE"] });
            findedRow = FindRowByPrimarySourceKey(dsMeansType.Tables[0], budgetRow["MEANSTYPE"]);
            SetClsFieldsByRow(clsRow, findedRow,
                "MEANSTYPEID", "MEANSTYPECODE", "MEANSTYPENAME", nullMeansType);

            clsRow["ServiceAccount"] = budgetRow["ServiceAccount"];
        }

        private void PumpFacialAcc()
        {
            if (!toPumpOutcomesPlan && !toPumpTreasury && !toPumpFinancing && !toPumpIncomes && !toPumpIncomesPlan && !toPumpIFPlan && !toPumpIFFact)
                return;
            string fields = "cast(f.id as varChar(10)) Id, f.Name, cast(f.KVSR as varChar(10)) KVSR, cast(f.KFSR as varChar(10)) KFSR, " +
                "cast(f.KESR as varChar(10)) KESR, cast(f.KCSR as varChar(10)) KCSR, cast(f.KVR as varChar(10)) KVR, " +
                "cast(f.FINTYPE as varChar(10)) FINTYPE, cast(f.REGIONCLS as varChar(10)) REGIONCLS, cast(f.MEANSTYPE as varChar(10)) MEANSTYPE, " +
                "cast(f.ORG_REF as varChar(10)) ORG_REF, cast(f.GENERALORG_REF as varChar(10)) GENERALORG_REF, " +
                "cast(f.HIGHERORG_REF as varChar(10)) HIGHERORG_REF, cast(f.ServiceAccount as varChar(10)) ServiceAccount ";
            PumpBudgetCls("FACIALACC_CLS f", dsFacialAcc.Tables[0], clsFacialAcc, fields, string.Empty, string.Empty,
                new ProcessClsRowDelegate(PumpFacialAcc), null, ref facialAccCache);
        }

        #endregion facialAcc

        private void PumpPlanKind()
        {
            if (!toPumpOutcomesPlan && !toPumpIFPlan)
                return;
            PumpBudgetCls("PlanKind", dsPlanKind.Tables[0], clsPlanKind, string.Empty, string.Empty,
                new ProcessClsRowDelegate(PumpSimpleClsRowEx), new string[] { "Name", "Name" }, ref planKindCache);
        }

        private void PumpFinType()
        {
            if (!toPumpOutcomesPlan && !toPumpTreasury && !toPumpFinancing && !toPumpIFFact)
                return;
            PumpBudgetCls("FINTYPE", dsFinType.Tables[0], clsFinType,
                "Cast(ID as Varchar(10)) ID, Name, CREATEDATE, UPDATEDATE",
                string.Empty, string.Empty, new ProcessClsRowDelegate(PumpSimpleClsRow),
                new string[] { "CODE", "ID", "NAME", BUDGET_CLS_DEFAULT_NAME }, ref finTypeCache);
            DeleteNullSourceKey(dsFinType.Tables[0]);
            dsFinType.Tables[0].PrimaryKey = new DataColumn[] { dsFinType.Tables[0].Columns["SourceKey"] };
        }

        private void PumpPlanDocType()
        {
            if (!toPumpOutcomesPlan && !toPumpIncomesPlan && !toPumpIFPlan)
                return;
            PumpBudgetCls("PlanDocType", dsPlanDocType.Tables[0], clsPlanDocType,
                "Cast(ID as Varchar(10)) ID, Name, CREATEDATE, UPDATEDATE",
                string.Empty, string.Empty, new ProcessClsRowDelegate(PumpSimpleClsRow),
                new string[] { "CODE", "ID", "NAME", BUDGET_CLS_DEFAULT_NAME }, ref planDocTypeCache);
            DeleteNullSourceKey(dsPlanDocType.Tables[0]);
        }

        #region transfert

        private void PumpTransfert(DataRow budgetRow, DataRow clsRow, string[] fieldsMapping)
        {
            clsRow["CODE"] = budgetRow["Code"];
            clsRow["NAME"] = budgetRow["NAME"];
            if (clsRow["NAME"].ToString().Trim() == string.Empty)
                clsRow["NAME"] = constDefaultClsName;
            if (MajorDBVersion >= 38)
                clsRow["TARGETCLS"] = budgetRow["TARGETCLS"];

            if (Convert.ToInt32(budgetRow["TARGETMEANSSIGN"]) == 0)
            {
                clsRow["RECEIVEKVSR"] = budgetRow["RECEIVEKVSR"];
                clsRow["RECEIVEKFSR"] = budgetRow["RECEIVEKFSR"];
                clsRow["RECEIVEKCSR"] = budgetRow["RECEIVEKCSR"];
                clsRow["RECEIVEKVR"] = budgetRow["RECEIVEKVR"];
                clsRow["RECEIVEKESR"] = budgetRow["RECEIVEKESR"];
                clsRow["RECEIVEKD"] = budgetRow["RECEIVEKD"];
            }
            else
            {
                clsRow["TRANSFERKVSR"] = budgetRow["TRANSFERKVSR"];
                clsRow["TRANSFERKFSR"] = budgetRow["TRANSFERKFSR"];
                clsRow["TRANSFERKCSR"] = budgetRow["TRANSFERKCSR"];
                clsRow["TRANSFERKVR"] = budgetRow["TRANSFERKVR"];
                clsRow["TRANSFERKESR"] = budgetRow["TRANSFERKESR"];
                clsRow["TRANSFERKDVALUE"] = budgetRow["TRANSFERKDVALUE"];
            }
        }

        private void PumpTransfert()
        {
            if (!toPumpOutcomesPlan && !toPumpIncomesPlan && !toPumpTreasury && !toPumpIncomes)
                return;
            if (MajorDBVersion < 38)
                return;

            string fields = "Cast(t.ID as Varchar(10)) ID, Cast(t.Code as Varchar(10)) Code, t.Name, t.CREATEDATE, t.UPDATEDATE, " + 
                            "Cast(t.RECEIVEKVSR as Varchar(10)) RECEIVEKVSR, Cast(kvsrT.Code as Varchar(10)) TRANSFERKVSR, " +
                            "Cast(kfsrR.Id as Varchar(10)) RECEIVEKFSR, Cast(kfsrT.Id as Varchar(10)) TRANSFERKFSR, " +
                            "Cast(t.RECEIVEKCSR as Varchar(10)) RECEIVEKCSR, Cast(kcsrT.Code as Varchar(10)) TRANSFERKCSR, " +
                            "Cast(t.RECEIVEKVR as Varchar(10)) RECEIVEKVR, Cast(kvrT.Code as Varchar(10)) TRANSFERKVR, " +
                            "Cast(kesrR.Id as Varchar(10)) RECEIVEKESR, Cast(kesrT.Id as Varchar(10)) TRANSFERKESR, " +
                            "Cast(kdR.KdValue as Varchar(30)) RECEIVEKD, Cast(t.TRANSFERKDVALUE as Varchar(30)) TRANSFERKDVALUE, " +
                            "Cast(tCls.StrCode as Varchar(10)) TARGETCLS, Cast(t.TARGETMEANSSIGN as Varchar(10)) TARGETMEANSSIGN ";

            string joinStr = " left join kvsr kvsrT on (t.TRANSFERKVSR = kvsrT.ID) " +
                             " left join kfsr kfsrR on (t.RECEIVEKFSR = kfsrR.ID) " +
                             " left join kfsr kfsrT on (t.TRANSFERKFSR = kfsrT.ID) " +
                             " left join kcsr kcsrT on (t.TRANSFERkcsr = kcsrT.ID) " +
                             " left join kvr kvrT on (t.TRANSFERkvr = kvrT.ID) " +
                             " left join kesr kesrR on (t.RECEIVEkesr = kesrR.ID) " +
                             " left join kesr kesrT on (t.TRANSFERkesr = kesrT.ID) " +
                             " left join kd kdR on (t.RECEIVEkd = kdR.ID) " +
                             " left join targetcls tCls on (t.TargetCls = tCls.Id) ";

            PumpBudgetCls("TransfertCls t", dsTransfert.Tables[0], clsTransfert, fields,
                joinStr, string.Empty, new ProcessClsRowDelegate(PumpTransfert), null, ref transfertCache);
            DeleteNullSourceKey(dsTransfert.Tables[0]);
            UpdateDataSet(daTransfert, dsTransfert, clsTransfert);

            // ���������� ������ �� ����
            Dictionary<string, DataRow> clsCache = new Dictionary<string, DataRow>();
            try
            {
                foreach (DataRow row in dsTransfert.Tables[0].Rows)
                {
                    string key = row["Code"].ToString();
                    if (!clsCache.ContainsKey(key))
                    {
                        clsCache.Add(key, row);
                        continue;
                    }
                    DataRow cacheRow = clsCache[key];
                    string[] clsFields = new string[] { "RECEIVEKVSR", "RECEIVEKFSR", "RECEIVEKCSR", "RECEIVEKVR", "RECEIVEKESR", "RECEIVEKD", 
                    "TRANSFERKVSR", "TRANSFERKFSR", "TRANSFERKCSR", "TRANSFERKVR", "TRANSFERKESR", "TRANSFERKDVALUE" };
                    foreach (string clsField in clsFields)
                        if (row[clsField] != DBNull.Value)
                            cacheRow[clsField] = row[clsField];
                    row.Delete();
                }
            }
            finally
            {
                clsCache.Clear();
            }
            UpdateDataSet(daTransfert, dsTransfert, clsTransfert);
            transfertCache.Clear();
            FillRowsCache(ref transfertCache, dsTransfert.Tables[0], "Code");
        }

        #endregion transfert

        private void PumpBudgetClsData()
        {
            // � ������ 6.09 �������� ��� ����� ���������������
            if (this.MajorDBVersion >= 35)
            {
                PumpBudgetBudget();
                PumpFundsSource();
                PumpAsgmtKind();
                PumpAsgmtSrce();
                PumpBuhOperations();
                PumpPlanKind();
            }
            PumpKvsr();
            PumpRegion();
            PumpBudgetAccounts();
            PumpNotifyTypes();
            PumpSubKesr();
            PumpEkr();
            PumpFkr();
            PumpKcsr();
            PumpEkd();
            PumpFinType(); 
            PumpFact();
            PumpMeansType();
            PumpKvr();
            if (this.MajorDBVersion >= 31)
            {
                PumpProgram();
                PumpDirection();
            }
            PumpKD();
            PumpKif();
            PumpFacialAcc();
            PumpPlanDocType();
            PumpTransfert();
            UpdateData();
        }

        #endregion ������� ������� ���������������

    }
}