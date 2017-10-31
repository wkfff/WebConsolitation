using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

using Krista.FM.Server.Common;
using Krista.FM.Server.DataPumps;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.DataPumps.TaxesRegulationDataPump
{
    /// <summary>
    /// ���_0009_���������� ����������� ������� ����� �������� �������� �������������� ����
    /// </summary>
    public class TaxesRegulationDataPumpModule : CorrectedPumpModuleBase
    {
        #region ����

        // �����.��� (d.OKATO.UFK)
        private IDbDataAdapter daOkatoUfk;
        private DataSet dsOkatoUfk;
        // ��.��� (d.KD.UFK)
        private IDbDataAdapter daKdUfk;
        private DataSet dsKdUfk;

        // ������.���_������_���������� ����������� (f.D.IncomesRC)
        private IDbDataAdapter daIncomesRC;
        private DataSet dsIncomesRC;

        private IClassifier clsOkatoUfk;
        private IClassifier clsKdUfk;

        private IFactTable fctIncomesRC;

        // ���� ���������������
        private Dictionary<string, int> kdCache = null;
        private Dictionary<string, int> okatoCache = null;

        private DBDataAccess dbDataAccess = new DBDataAccess();
        private Database dbfDB = null;
        private int totalFiles = 0;
        private int filesCount = 0;
        private int month = -1;

        private List<string> deletedData = new List<string>(200);

        #endregion ����


        #region ���������

        /// <summary>
        /// ���������� �������, � ������� ����� ��������
        /// </summary>
        private const int constMaxQueryRecords = 10000;

        /// <summary>
        /// ����������� ��� ������� �������� ������ �� �������������� ��������
        /// </summary>
        private const string constOracleDisableDeletionFinalOverturnConstraint =
            "(mod(REFFKDAYUNV, 10000) <> 1232 and REFFKDAYUNV = {0} and GHOST = '{1}')";

        /// <summary>
        /// ����������� ��� ������� �������� ������ �� �������������� ��������
        /// </summary>
        private const string constSQLServerDisableDeletionFinalOverturnConstraint =
            "((REFFKDAYUNV % 10000) <> 1232 and REFFKDAYUNV = {0} and GHOST = '{1}')";

        /// <summary>
        /// ����������� ��� ������� ������ ������ �� ���������� ������
        /// </summary>
        private const string constOracleSelectFactDataByMonthConstraint =
            "(floor(mod(REFFKDAYUNV, 10000) / 100) = {0})";

        /// <summary>
        /// ����������� ��� ������� ������ ������ �� ���������� ������
        /// </summary>
        private const string constSQLServerSelectFactDataByMonthConstraint =
            "(floor((REFFKDAYUNV % 10000) / 100) = {0})";

        #endregion ���������


        #region �������������

        /// <summary>
        /// ������������ ��������
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (dbDataAccess != null) dbDataAccess.Dispose();
            }

            base.Dispose(disposing);
        }

        #endregion �������������


        #region ������� ������

        /// <summary>
        /// ���������� ����������� �� ������� �������� ���� ����
        /// </summary>
        /// <returns>�����������</returns>
        private string GetDisableDeletionFinalOverturnConstraint()
        {
            switch (this.ServerDBMSName)
            {
                case DBMSName.SQLServer:
                    return constSQLServerDisableDeletionFinalOverturnConstraint;

                default:
                    return constOracleDisableDeletionFinalOverturnConstraint;
            }
        }

        /// <summary>
        /// ���������� ���� ������� ���
        /// </summary>
        /// <param name="fileInfo">����</param>
        private void PumpDBFFile(FileInfo fileInfo)
        {
            int budgetLevel = 0;
            string ghost = string.Empty;

            // ���� ������� ��� ������� DFB � ������� ������� 01 (����������� ������)
            if (fileInfo.Name.ToUpper().StartsWith("DFB"))
            {
                budgetLevel = 1;
                ghost = "DFB";
            }
            // ���� ������� ��� ������� DKB � ������� ������� 03 (������ ��������)
            else if (fileInfo.Name.ToUpper().StartsWith("DKB"))
            {
                budgetLevel = 3;
                ghost = "DKB";
            }
            // ���� ������� ��� ������� DMB � ������� ������� 14 (����.������ ��)
            else if (fileInfo.Name.ToUpper().StartsWith("DMB"))
            {
                budgetLevel = 14;
                ghost = "DMB";
            }
            else
            {
                return;
            }

            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStartFilePumping,
                string.Format("����� ��������� ����� {0}.", fileInfo.FullName));

            IDbDataAdapter da = null;
            DataSet ds = null;
            InitDataSet(dbfDB, ref da, ref ds, string.Format("select * from {0}", fileInfo.Name));
            DataTable dt = ds.Tables[0];

            bool pumpRSMB = this.DataSource.Year >= 2006;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow row = dt.Rows[i];

                try
                {
                    int date = Convert.ToInt32(CommonRoutines.GlobalShortDateToNewDate(
                        Convert.ToString(row["D_OPER"])));

                    // ���� ���������� �������� "������� �������������� ��������", �� ������������ ���� ������ ���������� 
                    // �� ������� �� ������������� ������.���� �� �������� "�������������� �������" ����������� ����.
                    if (this.FinalOverturn)
                    {
                        date = this.FinalOverturnDate;
                    }

                    CheckDataSourceByDate(date, true);

                    string key = GetComplexCacheKey(new object[] { date, ghost });
                    if (!deletedData.Contains(key))
                    {
                        // ������� ���������� ����� ������ �� ���� ����
                        DeleteData(string.Format(GetDisableDeletionFinalOverturnConstraint(), date, ghost), 
                            string.Format("���� ������: {0}.", date));

                        deletedData.Add(key);
                    }

                    int okatoID;
                    if (pumpRSMB)
                    {
                        okatoID = PumpCachedRow(okatoCache, dsOkatoUfk.Tables[0], clsOkatoUfk,
                            GetComplexCacheKey(row, new string[] { "OKATO", "RS_MB" }),
                            new object[] { "CODE", row["OKATO"], "ACCOUNT", row["RS_MB"] });
                    }
                    else
                    {
                        okatoID = PumpCachedRow(okatoCache, dsOkatoUfk.Tables[0], clsOkatoUfk,
                            row["OKATO"], "CODE", null);
                    }

                    int kdID = PumpCachedRow(kdCache, dsKdUfk.Tables[0], clsKdUfk, row["C_PRIV"], "CODESTR", null);

                    PumpRow(dsIncomesRC.Tables[0], new object[] { "FORPERIOD", row["S_PAY"], "REFFKDAYUNV", date,
                        "RefFODayUNV", date, "REFKD", kdID, "REFOKATO", okatoID, "REFBUDGETLEVELS", budgetLevel,
                        "GHOST", ghost });

                    if (dsIncomesRC.Tables[0].Rows.Count > constMaxQueryRecords)
                    {
                        UpdateData();
                        ClearDataSet(daIncomesRC, ref dsIncomesRC);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("��� ��������� ������ {0} �������� ������ {1}", i, ex.Message), ex);
                }
            }

            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeSuccessfullFinishFilePump, string.Format(
                "��������� ����� {0} ���������. ���������� {1} �����.",
                fileInfo.FullName, dt.Rows.Count));
        }

        /// <summary>
        /// ���������� ������ ������ ������ �������
        /// </summary>
        /// <param name="filesList">������ ������</param>
        protected override void ProcessFiles(DirectoryInfo dir)
		{
            if (this.FinalOverturn)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, string.Format(
                    "������� �������������� �������� �� {0}.", 
                    CommonRoutines.NewDateToLongDate(this.FinalOverturnDate.ToString())));
            }

            FileInfo[] files = dir.GetFiles("*.DBF", SearchOption.AllDirectories);
            for (int i = 0; i < files.GetLength(0); i++)
            {
                // ������������ � ���������
                dbDataAccess.ConnectToDataSource(ref dbfDB, files[i].DirectoryName, ODBCDriverName.Microsoft_dBase_Driver);                

                filesCount++;
                SetProgress(totalFiles, filesCount,
                    string.Format("��������� ����� {0}...", files[i].FullName),
                    string.Format("���� {0} �� {1}", filesCount, totalFiles), true);

                try
                {
                    PumpDBFFile(files[i]);
                }
                catch (Exception ex)
                {
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeError, 
                        "��������� ����� ��������� � ��������", ex);
                    continue;
                }
            }
		}

		/// <summary>
		/// ������ ������ �� ����
		/// </summary>
        protected override void QueryData()
		{
            InitClsDataSet(ref daKdUfk, ref dsKdUfk, clsKdUfk);
            InitClsDataSet(ref daOkatoUfk, ref dsOkatoUfk, clsOkatoUfk);

            InitFactDataSet(ref daIncomesRC, ref dsIncomesRC, fctIncomesRC);

            FillRowsCache(ref kdCache, dsKdUfk.Tables[0], "CODESTR");

            if (this.DataSource.Year >= 2006)
            {
                FillRowsCache(ref okatoCache, dsOkatoUfk.Tables[0], new string[] { "CODE", "ACCOUNT" }, "ID");
            }
            else
            {
                FillRowsCache(ref okatoCache, dsOkatoUfk.Tables[0], "CODE");
            }
		}

		/// <summary>
		/// ������ ��������� � ����
		/// </summary>
        protected override void UpdateData()
		{
            UpdateDataSet(daKdUfk, dsKdUfk, clsKdUfk);
            UpdateDataSet(daOkatoUfk, dsOkatoUfk, clsOkatoUfk);

            UpdateDataSet(daIncomesRC, dsIncomesRC, fctIncomesRC);
		}

        private const string D_KD_UFK_GUID = "b713e1df-5584-4e3d-a399-8828a2906971";
        private const string D_OKATO_UFK_GUID = "4ae52664-ca7c-4994-bc5e-ba982421540e";
        private const string F_D_INCOMES_RC_GUID = "4285d4d8-2c6f-4cdc-952f-ea8d4d38bb98";
        protected override void InitDBObjects()
        {
            this.UsedClassifiers = new IClassifier[] { 
                clsKdUfk = this.Scheme.Classifiers[D_KD_UFK_GUID],
                clsOkatoUfk = this.Scheme.Classifiers[D_OKATO_UFK_GUID] };

            this.UsedFacts = new IFactTable[] { 
                fctIncomesRC = this.Scheme.FactTables[F_D_INCOMES_RC_GUID] };
        }

        /// <summary>
        /// ������� ���������� ����������� �������� ����
        /// </summary>
        protected override void PumpFinalizing()
        {
            if (dbfDB != null) dbfDB.Close();

            ClearDataSet(ref dsIncomesRC);
            ClearDataSet(ref dsKdUfk);
            ClearDataSet(ref dsOkatoUfk);

            deletedData.Clear();
        }

        /// <summary>
        /// ������� ������
        /// </summary>
        protected override void DirectPumpData()
        {
            totalFiles = this.RootDir.GetFiles("*.DBF", SearchOption.AllDirectories).GetLength(0);
            filesCount = 0;

            deletedData.Clear();

            PumpDataYTemplate();
        }

		#endregion ������� ������


        #region ��������� ������

        /// <summary>
        /// ���������� ����������� �� ������� �������� ���� ����
        /// </summary>
        /// <returns>�����������</returns>
        private string GetSelectFactDataByMonthConstraint()
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
        /// ������� ������� ������ �� ����
        /// </summary>
        protected override void QueryDataForProcess()
        {
            PrepareRegionsForSumDisint();
            PrepareOkatoForSumDisint(clsOkatoUfk);
        }

        /// <summary>
        /// ������������� ������������ ������������ ���� ��� �������� ���������
        /// </summary>
        protected override void ProcessDataSource()
        {
            if (month > 0)
            {
                SetOperationDaysForFact(fctIncomesRC, "GHOST", "REFFKDAYUNV", "REFFODAYUNV",
                    string.Format(GetSelectFactDataByMonthConstraint(), month));
            }
            else
            {
                SetOperationDaysForFact(fctIncomesRC, "GHOST", "REFFKDAYUNV", "REFFODAYUNV", string.Empty);
            }
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

        }

        /// <summary>
        /// ��������, ����������� ����� ��������� ������
        /// </summary>
        protected override void AfterProcessDataAction()
        {
            WriteBadOkatoCodesCacheToBD();
            WriteNullAccountsCacheToBD();
        }

        /// <summary>
        /// ���� ��������� ������
        /// </summary>
        protected override void DirectProcessData()
        {
            // ��������� ���� ������������ ������������ ����
            FillOperationDaysCorrCache();

            int year = -1;
            GetPumpParams(ref year, ref month);

            ProcessDataSourcesTemplate(year, month, "��������� ������������ ������������ ����");
        }

        #endregion ��������� ������
    }
}
