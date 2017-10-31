using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;

using Krista.FM.Common;
using Krista.FM.Server.Common;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.DataPumps
{
    // ����� ��� ����������� ���� ������ �� ���������� ���������� �������

    /// <summary>
    /// ������� ����� ��� ���� �������.
    /// </summary>
    public abstract partial class CorrectedPumpModuleBase : DataPumpModuleBase
    {
        #region ����

        protected IDbDataAdapter daDisintegratedRulesKD;
        protected IDbDataAdapter daDisintegratedRulesEX;
        protected IDbDataAdapter daDisintegratedRulesALTKD;
        protected DataSet dsDisintRules;
        protected IDbDataAdapter daSourceData;
        protected DataSet dsSourceData;
        protected IDbDataAdapter daDisintegratedData;
        protected DataSet dsDisintegratedData;
        // �����
        protected IDbDataAdapter daOKATO;
        protected DataSet dsOKATO;
        // �����.������������
        protected IDbDataAdapter daOKATOBridge;
        protected DataSet dsOKATOBridge;

        protected IFactTable fctSourceTable;
        protected IFactTable fctDisintegratedData;
        protected IClassifier clsKD;
        protected IClassifier clsOKATO;

        protected IClassifier brdOKATO;

        protected string refDateFieldName;
        protected string refKDFieldName;
        protected string refOkatoFieldName;

        // ������� ������������ �������
        protected int disintCount = 0;
        protected int totalRecsForSourceID = 0;
        protected int currentOkatoID;
        protected string currentOkatoCode;

        // ��� ������ �����������: ���� - ���, �������� - ������ ������ (���� - ��� ��, �������� - ������ �������)
        protected Dictionary<int, Dictionary<string, DataRow>> disintRulesCache =
            new Dictionary<int, Dictionary<string, DataRow>>(5);
        protected bool disintRulesIsEmpty = false;

        protected DataSet dsMessages;
        protected Dictionary<string, DataRow> messagesCache = new Dictionary<string, DataRow>(1000);

        // ��� ���� - ������� ��� �������
        protected string disintFlagFieldName = "REFISDISINT";
        protected string refBudgetLevelFieldName = "REFBUDGETLEVELS";
        protected string disintDateConstraint = string.Empty;
        #endregion ����


        #region ���������

        /// <summary>
        /// ���������� �������, � ������� ����� �������� ��� �����������
        /// </summary>
        protected const int constMaxQueryRecordsForDisint = 50000;

        #endregion ���������


        #region ������� ��� ������ � ����� ������ �����������

        /// <summary>
        /// ��������� ������ � ��� ������ �����������
        /// </summary>
        /// <param name="kd">��</param>
        /// <param name="year">���</param>
        /// <param name="row">������ ������ �����������</param>
        private void AddRuleToCache(Dictionary<int, Dictionary<string, DataRow>> cache, string kd, int year, 
            DataRow row)
        {
            if (!cache[year].ContainsKey(kd))
            {
                cache[year].Add(kd, row);
            }
        }

        /// <summary>
        /// ��������� ��� ������ �����������
        /// </summary>
        protected void FillDisintRulesCache()
        {
            QueryDisintRules();

            disintRulesCache.Clear();

            DataTable dt = dsDisintRules.Tables["disintrules_kd"];
            DataRow[] rows = dt.Select(string.Empty, "YEAR ASC");
            int year = -1;

            int count = rows.GetLength(0);
            for (int i = 0; i < count; i++)
            {
                if (year != Convert.ToInt32(rows[i]["YEAR"]))
                {
                    year = Convert.ToInt32(rows[i]["YEAR"]);
                    disintRulesCache.Add(year, new Dictionary<string, DataRow>(1000));
                }

                AddRuleToCache(disintRulesCache, Convert.ToString(rows[i]["KD"]), year, rows[i]);

                // � ���� �� ��� ����� � �������������� ����
                DataRow[] altKdRows = rows[i].GetChildRows("KD_ALTKD");
                int kdRowsCount = altKdRows.GetLength(0);
                for (int j = 0; j < kdRowsCount; j++)
                {
                    AddRuleToCache(disintRulesCache, Convert.ToString(altKdRows[j]["KD"]), year, rows[i]);
                }
            }
        }

        /// <summary>
        /// ���� ������� �����������
        /// </summary>
        /// <param name="year">���</param>
        /// <param name="kd">��� ��</param>
        /// <returns>������ ������� �����������</returns>
        protected DataRow FindDisintRule(Dictionary<int, Dictionary<string, DataRow>> cache, int year, string kd)
        {
            if (cache == null || !cache.ContainsKey(year)) return null;

            // ���� �� ������� (=�����), �� ���������� ������ ��� ����� - �������������� ��������� �������
            // ��� ��������� �� �����������, ������ ����������� ���
            string restrictedKD = kd;
            if (kd.Length == 20)
            {
                restrictedKD = kd.Remove(0, 3);
            }

            if (cache.ContainsKey(year))
            {
                // ������� ���� � ������ ���� ��������
                if (cache[year].ContainsKey(restrictedKD))
                {
                    return cache[year][restrictedKD];
                }
                // ���� �� �����, �� ���� � ������� ����� ��������
                else
                {
                    if (kd.Length == 20)
                    {
                        restrictedKD = restrictedKD.Remove(10, 4).Insert(10, "0000");
                        if (cache[year].ContainsKey(restrictedKD))
                        {
                            return cache[year][restrictedKD];
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// ������������� �������� ������ �����������
        /// </summary>
        private void QueryDisintRules()
        {
            DataRelation rel;

            this.ClearDataSet(ref dsDisintRules);

            // ������������� ���������
            // �������� ��
            InitLocalDataAdapter(this.DB, ref daDisintegratedRulesKD,
                "select k.ID, k.KD, k.YEAR, k.BYBUDGET, k.FED_PERCENT, k.CONS_PERCENT, k.SUBJ_PERCENT, " +
                "       k.CONSMR_PERCENT, k.MR_PERCENT, k.STAD_PERCENT, k.OUTOFBUDGETFOND_PERCENT, " +
                "       k.SMOLENSKACCOUNT_PERCENT, k.TUMENACCOUNT_PERCENT, k.CONSMO_PERCENT, k.GO_PERCENT " +
                "from DISINTRULES_KD k " +
                "order by k.KD asc");
            daDisintegratedRulesKD.Fill(dsDisintRules);
            dsDisintRules.Tables["Table"].TableName = "disintrules_kd";

            if (dsDisintRules.Tables["disintrules_kd"].Rows.Count == 0)
            {
                //throw new Exception("����������� ������� �����������");
            }

            // ���������
            InitLocalDataAdapter(this.DB, ref daDisintegratedRulesEX,
                "select e.INIT_DATE, e.REGION, e.FED_PERCENT, e.CONS_PERCENT, e.SUBJ_PERCENT, e.CONSMR_PERCENT, " +
                "       e.MR_PERCENT, e.STAD_PERCENT, e.OUTOFBUDGETFOND_PERCENT, e.SMOLENSKACCOUNT_PERCENT, " +
                "       e.TUMENACCOUNT_PERCENT, e.CONSMO_PERCENT, e.GO_PERCENT, e.REFDISINTRULES_KD " +
                "from DISINTRULES_EX e " +
                "order by e.INIT_DATE asc");
            daDisintegratedRulesEX.Fill(dsDisintRules);
            dsDisintRules.Tables["Table"].TableName = "disintrules_ex";

            // �������������� ��
            InitLocalDataAdapter(this.DB, ref daDisintegratedRulesALTKD,
                "select a.KD, a.REFDISINTRULES_KD from DISINTRULES_ALTKD a");
            daDisintegratedRulesALTKD.Fill(dsDisintRules);
            dsDisintRules.Tables["Table"].TableName = "disintrules_altkd";

            // ������� ��������� ����� ��������� ��������
            rel = new DataRelation(
                "KD_EX",
                dsDisintRules.Tables["disintrules_kd"].Columns["ID"],
                dsDisintRules.Tables["disintrules_ex"].Columns["refdisintrules_kd"]);
            dsDisintRules.Relations.Add(rel);

            rel = new DataRelation(
                "KD_ALTKD",
                dsDisintRules.Tables["disintrules_kd"].Columns["ID"],
                dsDisintRules.Tables["disintrules_altkd"].Columns["refdisintrules_kd"]);
            dsDisintRules.Relations.Add(rel);
        }

        /// <summary>
        /// ������������� �������� ��� �������� ������� ���� (����� �� ������ � ���� ������������� ������)
        /// </summary>
        protected void PrepareMessagesDS()
        {
            this.ClearDataSet(ref dsMessages);
            if (messagesCache != null)
                messagesCache.Clear();

            dsMessages.Tables.Add("Messages");
            dsMessages.Tables[0].Columns.Add("PumpID", typeof(string));
            dsMessages.Tables[0].Columns.Add("SOURCEID", typeof(string));
            dsMessages.Tables[0].Columns.Add("KD", typeof(string));
            dsMessages.Tables[0].Columns.Add("YEAR", typeof(int));
            dsMessages.Tables[0].Columns.Add("DATE", typeof(int));
            dsMessages.Tables[0].Columns.Add("COUNTER", typeof(int));
        }

        /// <summary>
        /// ������ ������ � ������� � ������� � ����������� �������
        /// </summary>
        /// <param name="date">����</param>
        /// <param name="year">���</param>
        /// <param name="sourceID">�� ���������</param>
        /// <param name="fullKD">��� ��</param>
        protected void WriteRecInMessagesDS(string date, int year, int sourceID, string fullKD)
        {
            if (disintRulesIsEmpty) return;

            string key = GetComplexCacheKey(new object[] { this.PumpID, sourceID, fullKD, year, date });

            if (!messagesCache.ContainsKey(key))
            {
                DataRow row = dsMessages.Tables[0].NewRow();
                row["PUMPID"] = this.PumpID;
                row["SOURCEID"] = sourceID;
                row["KD"] = fullKD;
                row["YEAR"] = year;
                row["DATE"] = date;
                row["COUNTER"] = 1;
                dsMessages.Tables[0].Rows.Add(row);

                messagesCache.Add(key, row);
            }
            else
            {
                messagesCache[key]["COUNTER"] = Convert.ToInt32(messagesCache[key]["COUNTER"]) + 1;
            }
        }

        /// <summary>
        /// ���������� ��������� � ����������� ������������ � ���
        /// </summary>
        protected void UpdateMessagesDS()
        {
            if (disintRulesIsEmpty) return;

            if (dsMessages != null)
            {
                for (int i = 0; i < dsMessages.Tables[0].Rows.Count; i++)
                {
                    string msg = string.Format(
                        "�� ������� �� ������ ����������� ��� �� = {0}, ��� = {1}, ���� = {2} ({3} �������).",
                        dsMessages.Tables[0].Rows[i]["KD"],
                        dsMessages.Tables[0].Rows[i]["YEAR"],
                        dsMessages.Tables[0].Rows[i]["DATE"],
                        dsMessages.Tables[0].Rows[i]["COUNTER"]);

                    WriteEventIntoProcessDataProtocol(
                        ProcessDataEventKind.pdeWarning, 
                        Convert.ToInt32(dsMessages.Tables[0].Rows[i]["PUMPID"]),
                        Convert.ToInt32(dsMessages.Tables[0].Rows[i]["SOURCEID"]), 
                        msg, null);
                }
            }
        }

        #endregion ������� ��� ������ � ����� ������ �����������


        #region ������� ����������� ������

        /// <summary>
        /// ���������� �������� ���������� ������ ��������� ��� ������� ������
        /// </summary>
        /// <param name="row">������� ������</param>
        /// <param name="date">����</param>
        /// <param name="okato">��� �����</param>
        /// <returns>������ ���������</returns>
        protected DataRow GetDisintExRow(DataRow row, int date, string okato)
        {
            DataRow realOkato = null;
            DataRow zeroOkato = null;

            DataRow[] rows = row.GetChildRows("KD_EX");

            // ����� ������ - ���� ��� ��������� �� ����� ���������, �� ��������� �������� �� ����� ������ 
            // (������ 5 ���� ����� ���������, ��������� ����)
            string okatoRegion = string.Concat(okato.Substring(0, 5), "000000");

            for (int i = rows.GetLength(0) - 1; i >= 0; i--)
            {
                // ����� ������ ��, ��� ���� �����, � �������� ��������� ��������� ������ ��� ����� ����� ����
                if ((Convert.ToInt32(rows[i]["INIT_DATE"]) / 100) > (date / 100))
                    continue;

                string region = Convert.ToString(rows[i]["REGION"]);
                if (region == okato)
                {
                    // ����� �������� �� ��������� ����� - �������
                    realOkato = rows[i];
                    break;
                }
                if (region == okatoRegion)
                    // ����� �������� �� ����� ������ - ���� �� ���������, ��� ��� �������� ���� �������� �� ��������� �����
                    realOkato = rows[i];
                // ���������� ������� ����� �� ������, ���� �� ������ ��������� ����� - ����� ������ �������
                else if (region.Trim('0') == string.Empty)
                    zeroOkato = rows[i];
            }
            if (realOkato != null)
                return realOkato;
            else
                return zeroOkato;
        }

        /// <summary>
        /// ����������, �������� �� ��������� ������� � ������������ ����������
        /// </summary>
        /// <param name="percentIndex">���������� ����� �������� �����������</param>
        /// <param name="terrType">��� ����������</param>
        /// <returns>��������� ��� ���</returns>
        protected bool CheckPercentByTerrType(int percentIndex, int terrType)
        {
            // ��� �����, � ������� ��� ������ = ��������� ����� ������ ����������� ��������� �� ���� 
            // "% ������ ��" � �� ������ ����������� ����������� "% ����.������ ��", "% ������ ������", 
            // "% ������ ���������"
            if (terrType == 7 && (percentIndex >= 4 && percentIndex <= 6))
            {
                return false;
            }

            // ��� �����, � ������� ��� ������ = ������������� ����� ������ ����������� ��������� �� ���� 
            // "% ����.������ ��" � �� ������ ����������� ����������� "% ������ ��"
            if (terrType == 4 && percentIndex == 15)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// ���������� ������ � ��������� �� � ������� ������������
        /// </summary>
        /// <param name="sourceRow">������ ��� �����������</param>
        /// <param name="disintRow">������� �����������</param>
        /// <param name="fieldsForDisint">������ ����� � ������� ��� �����������</param>
        private void DisintegrateRow(DataRow sourceRow, DataRow disintRow, string[] fieldsForDisint)
        {
            DataRow row = null;
            DataRow okatoRow = GetOkatoRow(currentOkatoID);
            if (okatoRow == null)
                return;
            int terrType = Convert.ToInt32(okatoRow["REFTERRTYPE"]);
            // � ������������ ���� ���������� ������ �� ������
            if (terrType == 0)
                return;

            bool isUFK14Pump = (this.PumpProgramID == PumpProgramID.UFK14Pump);
            // ������� ��� - 14, � ����� ���������� 1, 2, 8 (����.���� ����������) - ������ �� ������
            if (isUFK14Pump)
                if ((terrType == 1) || (terrType == 2) || (terrType == 8))
                    return;

            // ������������ ��� �������� �����������
            for (int j = 1; j <= 15; j++)
            {
                if (j == 8)
                {
                    j = 12;
                }

                bool zeroSums = true;
                bool skipRow = false;

                int count = fieldsForDisint.GetLength(0);
                for (int i = 0; i < count; i++)
                {
                    if (!CheckPercentByTerrType(j, terrType))
                    {
                        skipRow = true;
                        break;
                    }

                    if (row == null)
                    {
                        row = dsDisintegratedData.Tables[0].NewRow();
                        CopyRowToRow(sourceRow, row);
                    }

                    double d = Convert.ToDouble(sourceRow[fieldsForDisint[i]]);

                    switch (j)
                    {
                        case 1:
                            // ��� 14 - �� ������� � ���� ���� - 9
                            if (isUFK14Pump)
                                if (terrType == 9)
                                {
                                    d = 0;
                                    break;
                                }
                            d *= Convert.ToDouble(disintRow["FED_PERCENT"]) / 100;
                            break;

                        case 2:
                            if (isUFK14Pump)
                                if (this.Region == RegionName.MoskvaObl)
                                {
                                    d = 0;
                                    break;
                                }
                            d *= Convert.ToDouble(disintRow["CONS_PERCENT"]) / 100;
                            break;

                        case 3:
                            // ��� 14 - �� ������� � ���� ���� - 9
                            if (isUFK14Pump)
                                if (terrType == 9)
                                {
                                    d = 0;
                                    break;
                                }
                            d *= Convert.ToDouble(disintRow["SUBJ_PERCENT"]) / 100;
                            break;

                        case 4:
                            // ��� 14 - �� ������� � ���� ���� - 3, 7
                            if (isUFK14Pump)
                                if ((terrType == 3) || (terrType == 7) || (this.Region == RegionName.MoskvaObl))
                                {
                                    d = 0;
                                    break;
                                }
                            d *= Convert.ToDouble(disintRow["CONSMR_PERCENT"]) / 100;
                            break;

                        case 5:
                            // ��� 14 - �� ������� � ���� ���� - 3, 7
                            if (isUFK14Pump)
                                if ((terrType == 3) || (terrType == 7))
                                {
                                    d = 0;
                                    break;
                                }
                            d *= Convert.ToDouble(disintRow["MR_PERCENT"]) / 100;
                            break;

                        case 6:
                            // ��� 14 - �� ������� � ���� ���� - 3, 7, 9
                            if (isUFK14Pump)
                                if ((terrType == 3) || (terrType == 7) || (terrType == 9))
                                {
                                    d = 0;
                                    break;
                                }
                            d *= Convert.ToDouble(disintRow["STAD_PERCENT"]) / 100;
                            break;

                        case 7:
                            d *= Convert.ToDouble(disintRow["OUTOFBUDGETFOND_PERCENT"]) / 100;
                            break;

                        case 12:
                            // ��� 14 - �� ������� ������ � ���� ���� - 9
                            if (isUFK14Pump)
                                if (terrType == 9)
                                {
                                    d = 0;
                                    break;
                                }
                            d *= Convert.ToDouble(disintRow["SMOLENSKACCOUNT_PERCENT"]) / 100;
                            break;

                        case 13:
                            // ��� 14 - �� ������� ������ � ���� ���� - 9
                            if (isUFK14Pump)
                                if (terrType == 9)
                                {
                                    d = 0;
                                    break;
                                }
                            d *= Convert.ToDouble(disintRow["TUMENACCOUNT_PERCENT"]) / 100;
                            break;

                        case 14:
                            if (isUFK14Pump)
                                 if (this.Region == RegionName.MoskvaObl)
                                 {
                                     d = 0;
                                     break;
                                 }
                            d *= Convert.ToDouble(disintRow["CONSMO_PERCENT"]) / 100;
                            break;

                        case 15: 
                            // ��� 14 - ������� ������ � ���� ���� - 7
                            if (isUFK14Pump)
                            {
                                if (terrType != 7)
                                    d = 0;
                                else
                                    d *= Convert.ToDouble(disintRow["GO_PERCENT"]) / 100;
                            }
                            else
                            {
                                // ��� ���� ���������� ��, ��, �������� ����� -  ����� �� ����� ������ ������� �����������
                                if ((terrType == 5) || (terrType == 6) || (terrType == 10))
                                    d = 0;
                                else
                                    d *= Convert.ToDouble(disintRow["GO_PERCENT"]) / 100;
                            }
                            break;
                    }

                    if (d != 0)
                        zeroSums = false;

                    row[fieldsForDisint[i]] = d;
                }

                if (!zeroSums && !skipRow)
                {
                    row["SOURCEKEY"] = sourceRow["ID"];
                    row[refBudgetLevelFieldName] = j;

                    // ��������� ������� - ���� ����������� �� �������� �� ����� ��������
                    if (this.PumpProgramID == PumpProgramID.FNS28nDataPump)
                        row["RefNormDeduct"] = 6;

                    dsDisintegratedData.Tables[0].Rows.Add(row);

                    // ���� ���������� ����� ������������ �������, �� ���������� �� � ����
                    if (dsDisintegratedData.Tables[0].Rows.Count >= constMaxQueryRecordsForDisint)
                    {
                        UpdateOkatoData();
                        UpdateDataSet(daDisintegratedData, dsDisintegratedData, fctDisintegratedData);
                        ClearDataSet(daDisintegratedData, ref dsDisintegratedData);
                    }
                }

                row = null;
            }

            this.DB.ExecQuery(
                string.Format("update {0} set {1} = 1 where ID = ?", fctSourceTable.FullDBName, disintFlagFieldName),
                QueryResultTypes.NonQuery,
                this.DB.CreateParameter("ID", sourceRow["ID"], DbType.Int64));
        }

        /// </summary>
        /// ������ ������ ������ �� ���� ��� �����������
        /// </summary>
        /// <param name="constr">����������� �������</param>
        /// <param name="refKDFieldName">��� ���� - ������ �� ��</param>
        /// <param name="refOkatoFieldName">��� ���� - ������ �� OKATO</param>
        private void QuerySourceData(string constr, string refKDFieldName, string refOkatoFieldName)
        {
            this.SetProgress(-1, -1, "������ ������ ��� �����������...", string.Empty, true);
            WriteToTrace("������ ������ ��� �����������...", TraceMessageKind.Information);

            // ������������� ���������
            // ������� ��� ��������� ������ ��� ����������� (� ������������� ��)
            string str = string.Format(
                "select {0}.*, k.CODESTR KDCODE " +
                "from {0} left join {1} k on (k.id = {0}.{2})",
                fctSourceTable.FullDBName, clsKD.FullDBName, refKDFieldName);

            if (constr != string.Empty)
            {
                str += string.Format(" where {0}", constr);
            }
            str += string.Format(
                " order by {0}.{1} asc, {0}.sourceid asc", fctSourceTable.FullDBName, refKDFieldName);

            InitLocalDataAdapter(this.DB, ref daSourceData, str);
            ClearDataSet(ref dsSourceData);
            daSourceData.Fill(dsSourceData);

            WriteToTrace("������ ������ ��� ����������� �������.", TraceMessageKind.Information);
        }

        /// <summary>
        /// ������� ����������� ������
        /// </summary>
        /// <param name="fieldsForDisint">������ ����� � ������� ��� �����������</param>
        private void DisintegrateData(string[] fieldsForDisint)
        {
            // ������� �������
            int recCount = 0;
            DataRow disintRowEx = null;

            disintCount = 0;
            totalRecsForSourceID = 0;

            // ����������� ������ ����� �� �������� ��������� � ��������� ��� �����
            PrepareOkatoForSumDisint(clsOKATO);
            // ��������� ������ ����� � ������.��������� ��� �������, � ����� �� ���������� �����-�� ��� ���������� ��� �����
            AddOkatoToRegionsForPump();

            string constr = string.Format("{0}.SOURCEID = {1} and {0}.{2} = 0",
                fctSourceTable.FullDBName, this.SourceID, disintFlagFieldName);
            if (disintDateConstraint != string.Empty)
                constr += string.Format(" and {0}", disintDateConstraint);

            // ������, ���� �� ������ ��� ����������� � ������� ��
            // ����� �������
            int totalRecs = Convert.ToInt32(this.DB.ExecQuery(string.Format(
                "select count(id) from {0} where {1}", fctSourceTable.FullDBName, constr), QueryResultTypes.Scalar));
            if (totalRecs == 0)
            {
                WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeCriticalError, "��� ������ ��� �����������.");
                return;
            }

            WriteToTrace(string.Format("������� ��� �����������: {0}.", totalRecs), TraceMessageKind.Information);

            // ������ �������� ������� ��
            int firstID = Convert.ToInt32(this.DB.ExecQuery(string.Format(
                "select min(ID) from {0} where {1}", fctSourceTable.FullDBName, constr), QueryResultTypes.Scalar));
            // ������� ����������� �� ��� �������
            int lastID = firstID + constMaxQueryRecordsForDisint - 1;

            // ������� ������������ �������
            InitFactDataSet(ref daDisintegratedData, ref dsDisintegratedData, fctDisintegratedData);

            do
            {
                // ����������� ������� ��� ������� ������ ������
                string restrictID = string.Format(
                    "{0}.ID >= {1} and {0}.ID <= {2} and {3}", fctSourceTable.FullDBName, firstID, lastID, constr);
                firstID = lastID + 1;
                lastID += constMaxQueryRecordsForDisint;

                QuerySourceData(restrictID, refKDFieldName, refOkatoFieldName);

                if (dsSourceData.Tables[0].Rows.Count == 0)
                    continue;

                // ���������� ���������� ������
                for (int i = 0; i < dsSourceData.Tables[0].Rows.Count; i++)
                {
                    recCount++;

                    DataRow sourceRow = dsSourceData.Tables[0].Rows[i];

                    this.SetProgress(totalRecs, recCount, 
                        string.Format("��������� ������ (ID ��������� {0})...", this.SourceID),
                        string.Format("������ {0} �� {1}", recCount, totalRecs));

                    // �� ������ 28� ����� ����� ������, ��, ��� � �����. ��������� ���� - ������ ����� ����� ���� 
                    // � ����� ������ (��� ������ ���������)
                    currentOkatoID = Convert.ToInt32(sourceRow[refOkatoFieldName]);
                    currentOkatoCode = FindCachedRow(okatoCodesCache, currentOkatoID, string.Empty);
                    if (currentOkatoCode == string.Empty)
                    {
                        throw new Exception(string.Format("�� ������� ������ ����� �� ������ {0}", currentOkatoID));
                    }
                    
                    string kd = Convert.ToString(sourceRow["KDCODE"]);
                    string date = Convert.ToString(sourceRow[refDateFieldName]);
                    int year = Convert.ToInt32(date.Substring(0, 4));

                    totalRecsForSourceID++;

                    // ���� ������� �����������, ��������������� ��������: ��, O���� ������ ����� ��� � ������ ������, ���,
                    // ���� ������ ���������, �� ����� ������ ��, ��� ���� "����, � �������� ��������� ���������" 
                    // ������ ��� ����� ����� ����.
                    DataRow disintRow = FindDisintRule(disintRulesCache, year, kd);

                    if (disintRow == null)
                    {
                        // �� ������� �� ������ ����������� - ����� � �������� ������������� ������
                        WriteRecInMessagesDS(date, year, this.SourceID, kd);

                        GetOkatoRow(currentOkatoID);

                        continue;
                    }
                    else
                    {
                        // ���� ���������
                        disintRowEx = GetDisintExRow(disintRow, Convert.ToInt32(date), currentOkatoCode);
                        // ���� ����� ���������, �� ��� � ����������
                        if (disintRowEx != null)
                            disintRow = disintRowEx;
                    }

                    // ���������� ������
                    DisintegrateRow(sourceRow, disintRow, fieldsForDisint);
                    disintCount++;
                }
            }
            while (recCount < totalRecs);

            UpdateOkatoData();
            UpdateDataSet(daDisintegratedData, dsDisintegratedData, fctDisintegratedData);
        }

        /// <summary>
        /// ���������� ����� ������ �����������: 0 - ������ ��������������, 1 - ���
        /// </summary>
        private int GetDisintMode()
        {
            string str = GetParamValueByName(
                this.PumpRegistryElement.ProgramConfig, "rbtnDisintegratedOnly", "false");
            if (Convert.ToBoolean(str))
            {
                return 0;
            }

            return 1;
        }

        /// <summary>
        /// ���������� ��������� ����� ����������� ������
        /// </summary>
        /// <param name="year">���</param>
        /// <param name="month">�����</param>
        /// <param name="disintAll">���������� ��� ��� ���</param>
        protected void GetDisintParams(ref int year, ref int month, ref bool disintAll)
        {
            if (GetDisintMode() == 0)
            {
                disintAll = false;
            }
            else
            {
                disintAll = true;
            }

            string str = GetParamValueByName(
                this.PumpRegistryElement.ProgramConfig, "umeYears", string.Empty);
            if (str == string.Empty)
            {
                year = -1;
            }
            else
            {
                year = Convert.ToInt32(str);
            }

            str = GetParamValueByName(
                this.PumpRegistryElement.ProgramConfig, "ucMonths", string.Empty);
            if (str == string.Empty)
            {
                month = -1;
            }
            else
            {
                month = Convert.ToInt32(str);
            }
        }

        /// <summary>
        /// ������� ������������ ������
        /// </summary>
        /// <returns>������</returns>
        protected void PrepareDisintData(bool disintAll)
        {
            string query = string.Format("update {0} set {1} = 0 where SOURCEID = {2}",
                fctSourceTable.FullDBName, disintFlagFieldName, this.SourceID);
            if (disintDateConstraint != string.Empty)
                query += " and " + disintDateConstraint;
            if (this.StagesQueue[PumpProcessStates.PumpData].IsExecuted || disintAll)
            {
                DeleteTableData(fctDisintegratedData, -1, this.SourceID, disintDateConstraint);
                this.DB.ExecQuery(query, QueryResultTypes.NonQuery);
            }
            if (!this.StagesQueue[PumpProcessStates.PumpData].IsExecuted && disintAll)
            {

                this.DB.ExecQuery(query, QueryResultTypes.NonQuery);
            }
        }

        /// <summary>
        /// ���������� ������ �� ���������� ���������� �������
        /// </summary>
        /// <param name="dpm">������ �������, ���������� ������ �����</param>
        /// <param name="fctSourceTable">������� �������� ������</param>
        /// <param name="fctDisintegratedData">������� ��� ������������ ������</param>
        /// <param name="clsKD">������ �������������� ��</param>
        /// <param name="clsOKATO">������ �������������� �����</param>
        /// <param name="fieldsForDisint">������ ����� � ������� ��� �����������</param>
        /// <param name="refDateFieldName">��� ���� - ������ �� ����</param>
        /// <param name="refKDFieldName">��� ���� - ������ �� ��</param>
        /// <param name="refOkatoFieldName">��� ���� - ������ �� OKATO</param>
        protected void DisintegrateData(IFactTable fctSourceTable, IFactTable fctDisintegratedData, IClassifier clsKD,
            IClassifier clsOKATO, string[] fieldsForDisint, string refDateFieldName, string refKDFieldName,
            string refOkatoFieldName, bool disintAll)
        {
            this.fctSourceTable = fctSourceTable;
            this.fctDisintegratedData = fctDisintegratedData;
            this.clsKD = clsKD;
            this.clsOKATO = clsOKATO;
            this.refDateFieldName = refDateFieldName;
            this.refKDFieldName = refKDFieldName;
            this.refOkatoFieldName = refOkatoFieldName;

            PrepareDisintData(disintAll);

            DisintegrateData(fieldsForDisint);
        }

        /// <summary>
        /// ��������� ������� ������ ����������� ��� �������� ����
        /// </summary>
        protected void CheckDisintRulesCache()
        {
            // ��������� ������� ������ ����������� ��� �������� ����
            if (!disintRulesCache.ContainsKey(this.DataSource.Year))
            {
                WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeInformation,
                    string.Format("����������� ������� ����������� ��� {0} ���� - ������ �� ����� ����������.", this.DataSource.Year));
                disintRulesIsEmpty = true;
            }
            else
            {
                disintRulesIsEmpty = false;
            }
        }

        #endregion ������� ����������� ������
    }
}