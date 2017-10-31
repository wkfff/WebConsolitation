using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;

using Krista.FM.Providers.DataAccess;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.FKMonthRepPump
{
    /// <summary>
    /// ��_0001_����������� �����
    /// </summary>
    public partial class FKMonthRepPumpModule : CorrectedPumpModuleBase
    {
        #region ����

        #region ��������������

        // ����������.�� (d_Territory_FK)
        private IDbDataAdapter daTerritoryFK;
        private DataSet dsTerritoryFK;
        private IClassifier clsTerritoryFK;
        private int nullTerritoryFK;
        private Dictionary<string, int> terrCache = null;
        // ��.��_������ (d_KD_FKMR)
        private IDbDataAdapter daKD;
        private DataSet dsKD;
        private IClassifier clsKD;
        private int nullKD;
        private Dictionary<string, int> kdCache = null;
        // ���.��_������ (d_KIF_FKMR)
        private IDbDataAdapter daKIF;
        private DataSet dsKIF;
        private IClassifier clsKIF;
        private int nullKIF;
        private Dictionary<string, int> kifCache = null;
        // ���.��_������ (d_FKR_FKMR)
        private IDbDataAdapter daFKR;
        private DataSet dsFKR;
        private IClassifier clsFKR;
        private int nullFKR;
        private Dictionary<string, int> fkrCache = null;
        // ���.��_������_2005 (d_EKR_FKMR2005)
        private IDbDataAdapter daEKR2005;
        private DataSet dsEKR2005;
        private IClassifier clsEKR2005;
        private int nullEKR2005;
        private Dictionary<string, int> ekrCache = null;
        // ����������.��_������_������� (d_Marks_FKMROutcomes)
        private IDbDataAdapter daMarksOutcomes;
        private DataSet dsMarksOutcomes;
        private IClassifier clsMarksOutcomes;
        private int nullMarksOutcomes;
        private Dictionary<string, int> marksOutcomesCache = null;
        // ����������.��_������ (d_Marks_FKMR)
        private IDbDataAdapter daMarks;
        private DataSet dsMarks;
        private IClassifier clsMarks;
        private int nullMarks;
        private Dictionary<string, int> marksCache = null;
        // �������.��_������ (d_R_FKMR)
        private IDbDataAdapter daClsOutcomes;
        private DataSet dsClsOutcomes;
        private IClassifier clsOutcomes;
        private int nullClsOutcomes;
        private Dictionary<string, int> clsOutcomesCache = null;

        // ����������.��_������_������������ (d_Marks_FKMRInDebt)
        private IDbDataAdapter daIFSRefsCls;
        private DataSet dsIFSRefsCls;
        private IClassifier clsIFSRefsCls;
        private int nullIFSRefsCls;
        private Dictionary<string, int> iFSRefsClsCache = null;
        // ����������.��_������_����������� (d_Marks_FKMROutDebt)
        private IDbDataAdapter daOFSRefsCls;
        private DataSet dsOFSRefsCls;
        private IClassifier clsOFSRefsCls;
        private int nullOFSRefsCls;
        private Dictionary<string, int> oFSRefsClsCache = null;
        // ����������.��_������_���������������� (d_Marks_FKMRArrears)
        private IDbDataAdapter daArrearsRefsCls;
        private DataSet dsArrearsRefsCls;
        private IClassifier clsArrearsRefsCls;
        private int nullArrearsRefsCls;
        private Dictionary<string, int> arrearsRefsClsCache = null;
        // ����������.��_������_���������� (d_Marks_FKMROutlay)
        private IDbDataAdapter daOutcomesRefsCls;
        private DataSet dsOutcomesRefsCls;
        private IClassifier clsOutcomesRefsCls;
        private int nullOutcomesRefsCls;
        private Dictionary<string, int> outcomesRefsClsCache = null;
        // ����������.��_������_���������� (d_Marks_FKMRExcess)
        private IDbDataAdapter daExcessRefsCls;
        private DataSet dsExcessRefsCls;
        private IClassifier clsExcessRefsCls;
        private int nullExcessRefsCls;
        private Dictionary<string, int> excessRefsClsCache = null;

        #endregion ��������������

        #region �����

        // ���������������.��_������_������� �������� (f_DP_FKMRDefProf)
        private IDbDataAdapter daFKMRDefProf;
        private DataSet dsFKMRDefProf;
        private IFactTable fctFKMRDefProf;
        // ������.��_������_������ (f_D_FKMRIncomes)
        private IDbDataAdapter daFKMRIncomes;
        private DataSet dsFKMRIncomes;
        private IFactTable fctFKMRIncomes;
        // �����������������������.��_������_��������� �������������� (f_SrcFin_FKMRSrcFin)
        private IDbDataAdapter daFKMRSrcFin;
        private DataSet dsFKMRSrcFin;
        private IFactTable fctFKMRSrcFin;
        // �������.��_������_������� (f_R_FKMROutcomes)
        private IDbDataAdapter daFKMROutcomes;
        private DataSet dsFKMROutcomes;
        private IFactTable fctFKMROutcomes;
        // �������.��_������_��������� (f_R_FKMRAdd)
        private IDbDataAdapter daFKMRAdd;
        private DataSet dsFKMRAdd;
        private IFactTable fctFKMRAdd;

        // ����.��_������_������������ (f_F_FKMRInDebtBooks)
        private IDbDataAdapter daIFSRefsFact;
        private DataSet dsIFSRefsFact;
        private IFactTable fctIFSRefsFact;
        // ����.��_������_����������� (f_F_FKMROutDebtBooks)
        private IDbDataAdapter daOFSRefsFact;
        private DataSet dsOFSRefsFact;
        private IFactTable fctOFSRefsFact;
        // ����.��_������_���������������� (f_F_FKMRArrearsBooks)
        private IDbDataAdapter daArrearsRefsFact;
        private DataSet dsArrearsRefsFact;
        private IFactTable fctArrearsRefsFact;
        // ����.��_������_������������� (f_F_FKMROutcomesBooksAdd)
        private IDbDataAdapter daOutcomesRefsFact;
        private DataSet dsOutcomesRefsFact;
        private IFactTable fctOutcomesRefsFact;
        // ����.��_������_���������� (f_F_FKMRExcessBooks)
        private IDbDataAdapter daExcessRefsFact;
        private DataSet dsExcessRefsFact;
        private IFactTable fctExcessRefsFact;

        #endregion �����

        private int sourceDate;
        private int totalFiles = 0;
        private int filesCount = 0;
        private ReportFormat reportFormat;

        bool toPumpIncomes = false;
        bool toPumpOutcomes = false;
        bool toPumpDefProf = false;
        bool toPumpFinSources = false;
        bool toPumpReference = false;

        bool toPumpIFSRefs = false;
        bool toPumpOFSRefs = false;
        bool toPumpArrearsRefs = false;
        bool toPumpOutcomesRefs = false;
        bool toPumpExcessRefs = false;

        // �������� ���������������
        // ���� - ����������� ��� (-1 - ����� ���), �������� - ������������ ���
        private Dictionary<int, int> kdHierarchy2001 = new Dictionary<int, int>();
        private Dictionary<int, int> kifHierarchy2001 = new Dictionary<int, int>();
        private Dictionary<int, int> marksHierarchy = new Dictionary<int, int>();

        #endregion ����

        #region ���������, ������������

        private enum ReportFormat
        {
            February2002,
            Format2005,
            October2005,
            Format2006,
            November2006,
            March2007,
            Format2011
        }

        private enum FKMonthRepBlock
        {
            Add,
            DefProf,
            Incomes,
            Outcomes,
            SrcFin,
            SrcOutFin,
            Unknown
        }

        #endregion ���������, ������������

        #region ������� ������

        #region ������ � ����� � ������

        private void InitNullClsValues()
        {
            nullFKR = clsFKR.UpdateFixedRows(this.DB, this.SourceID);
            nullMarksOutcomes = clsMarksOutcomes.UpdateFixedRows(this.DB, this.SourceID);
            nullEKR2005 = clsEKR2005.UpdateFixedRows(this.DB, this.SourceID);
            nullKD = clsKD.UpdateFixedRows(this.DB, this.SourceID);
            nullKIF = clsKIF.UpdateFixedRows(this.DB, this.SourceID);
            nullMarks = clsMarks.UpdateFixedRows(this.DB, this.SourceID);
            nullTerritoryFK = clsTerritoryFK.UpdateFixedRows(this.DB, this.SourceID);
            nullClsOutcomes = clsOutcomes.UpdateFixedRows(this.DB, this.SourceID);

            nullIFSRefsCls = clsIFSRefsCls.UpdateFixedRows(this.DB, this.SourceID);
            nullOFSRefsCls = clsOFSRefsCls.UpdateFixedRows(this.DB, this.SourceID);
            nullArrearsRefsCls = clsArrearsRefsCls.UpdateFixedRows(this.DB, this.SourceID);
            nullOutcomesRefsCls = clsOutcomesRefsCls.UpdateFixedRows(this.DB, this.SourceID);
            nullExcessRefsCls = clsExcessRefsCls.UpdateFixedRows(this.DB, this.SourceID);
        }

        private void FillCache()
        {
            if (this.DataSource.Year >= 2008)
                FillRowsCache(ref ekrCache, dsEKR2005.Tables[0], "CODE");
            else if (this.DataSource.Year >= 2005)
                FillRowsCache(ref ekrCache, dsEKR2005.Tables[0], new string[] { "CODE", "StringCode" }, "|", "ID");
            else
                FillRowsCache(ref marksOutcomesCache, dsMarksOutcomes.Tables[0], "CODESTR");
            FillRowsCache(ref kdCache, dsKD.Tables[0], "CODESTR");
            FillRowsCache(ref kifCache, dsKIF.Tables[0], "CODESTR");
            FillRowsCache(ref fkrCache, dsFKR.Tables[0], "CODE");
            FillRowsCache(ref marksCache, dsMarks.Tables[0], "CODE");
            FillRowsCache(ref clsOutcomesCache, dsClsOutcomes.Tables[0], "CodeStr");
            FillRowsCache(ref terrCache, dsTerritoryFK.Tables[0], new string[] { "CODE", "Name" }, "|", "ID");

            FillRowsCache(ref iFSRefsClsCache, dsIFSRefsCls.Tables[0], "LongCode");
            FillRowsCache(ref oFSRefsClsCache, dsOFSRefsCls.Tables[0], "LongCode");
            FillRowsCache(ref arrearsRefsClsCache, dsArrearsRefsCls.Tables[0], "LongCode");
            FillRowsCache(ref outcomesRefsClsCache, dsOutcomesRefsCls.Tables[0], "LongCode");
            FillRowsCache(ref excessRefsClsCache, dsExcessRefsCls.Tables[0], "LongCode");
        }

        protected override void QueryData()
        {
            InitFactDataSet(ref daFKMRAdd, ref dsFKMRAdd, fctFKMRAdd);
            InitFactDataSet(ref daFKMRDefProf, ref dsFKMRDefProf, fctFKMRDefProf);
            InitFactDataSet(ref daFKMRIncomes, ref dsFKMRIncomes, fctFKMRIncomes);
            InitFactDataSet(ref daFKMROutcomes, ref dsFKMROutcomes, fctFKMROutcomes);
            InitFactDataSet(ref daFKMRSrcFin, ref dsFKMRSrcFin, fctFKMRSrcFin);

            InitFactDataSet(ref daIFSRefsFact, ref dsIFSRefsFact, fctIFSRefsFact);
            InitFactDataSet(ref daOFSRefsFact, ref dsOFSRefsFact, fctOFSRefsFact);
            InitFactDataSet(ref daArrearsRefsFact, ref dsArrearsRefsFact, fctArrearsRefsFact);
            InitFactDataSet(ref daOutcomesRefsFact, ref dsOutcomesRefsFact, fctOutcomesRefsFact);
            InitFactDataSet(ref daExcessRefsFact, ref dsExcessRefsFact, fctExcessRefsFact);

            InitClsDataSet(ref daFKR, ref dsFKR, clsFKR);
            InitClsDataSet(ref daEKR2005, ref dsEKR2005, clsEKR2005);
            InitClsDataSet(ref daKD, ref dsKD, clsKD);
            InitClsDataSet(ref daKIF, ref dsKIF, clsKIF);
            InitClsDataSet(ref daMarks, ref dsMarks, clsMarks);
            InitClsDataSet(ref daMarksOutcomes, ref dsMarksOutcomes, clsMarksOutcomes);
            InitClsDataSet(ref daTerritoryFK, ref dsTerritoryFK, clsTerritoryFK);
            InitClsDataSet(ref daClsOutcomes, ref dsClsOutcomes, clsOutcomes);

            InitClsDataSet(ref daIFSRefsCls, ref dsIFSRefsCls, clsIFSRefsCls);
            InitClsDataSet(ref daOFSRefsCls, ref dsOFSRefsCls, clsOFSRefsCls);
            InitClsDataSet(ref daArrearsRefsCls, ref dsArrearsRefsCls, clsArrearsRefsCls);
            InitClsDataSet(ref daOutcomesRefsCls, ref dsOutcomesRefsCls, clsOutcomesRefsCls);
            InitClsDataSet(ref daExcessRefsCls, ref dsExcessRefsCls, clsExcessRefsCls);

            InitNullClsValues();
            FillCache();
        }

        protected override void UpdateData()
        {
            UpdateDataSet(daFKR, dsFKR, clsFKR);
            UpdateDataSet(daEKR2005, dsEKR2005, clsEKR2005);
            UpdateDataSet(daKD, dsKD, clsKD);
            UpdateDataSet(daKIF, dsKIF, clsKIF);
            UpdateDataSet(daMarks, dsMarks, clsMarks);
            UpdateDataSet(daMarksOutcomes, dsMarksOutcomes, clsMarksOutcomes);
            UpdateDataSet(daTerritoryFK, dsTerritoryFK, clsTerritoryFK);
            UpdateDataSet(daClsOutcomes, dsClsOutcomes, clsOutcomes);

            UpdateDataSet(daIFSRefsCls, dsIFSRefsCls, clsIFSRefsCls);
            UpdateDataSet(daOFSRefsCls, dsOFSRefsCls, clsOFSRefsCls);
            UpdateDataSet(daArrearsRefsCls, dsArrearsRefsCls, clsArrearsRefsCls);
            UpdateDataSet(daOutcomesRefsCls, dsOutcomesRefsCls, clsOutcomesRefsCls);
            UpdateDataSet(daExcessRefsCls, dsExcessRefsCls, clsExcessRefsCls);

            UpdateDataSet(daFKMRAdd, dsFKMRAdd, fctFKMRAdd);
            UpdateDataSet(daFKMRDefProf, dsFKMRDefProf, fctFKMRDefProf);
            UpdateDataSet(daFKMRIncomes, dsFKMRIncomes, fctFKMRIncomes);
            UpdateDataSet(daFKMROutcomes, dsFKMROutcomes, fctFKMROutcomes);
            UpdateDataSet(daFKMRSrcFin, dsFKMRSrcFin, fctFKMRSrcFin);

            UpdateDataSet(daIFSRefsFact, dsIFSRefsFact, fctIFSRefsFact);
            UpdateDataSet(daOFSRefsFact, dsOFSRefsFact, fctOFSRefsFact);
            UpdateDataSet(daArrearsRefsFact, dsArrearsRefsFact, fctArrearsRefsFact);
            UpdateDataSet(daOutcomesRefsFact, dsOutcomesRefsFact, fctOutcomesRefsFact);
            UpdateDataSet(daExcessRefsFact, dsExcessRefsFact, fctExcessRefsFact);
        }

        #region GUID

        private const string D_KD_FKMR_GUID = "66450b83-d2fa-465c-a35d-009704607c7b";
        private const string D_KIF_FKMR_GUID = "356b81bc-d988-43b5-a4f7-2f44890634a9";
        private const string D_FKR_FKMR_GUID = "f8f710aa-bcd7-4179-8acd-327545961a9b";
        private const string D_EKR_FKMR_2005_GUID = "5b070556-75ea-4610-936e-67afec85ec56";
        private const string D_MARKS_FKMR_OUTCOMES_GUID = "657126bd-06ee-47b5-a3ad-27221f803182";
        private const string D_MARKS_FKMR_GUID = "8e4c9151-7894-446f-bb77-7c1c8fa1dfc7";
        private const string D_TERRITORY_FK_GUID = "ea935338-c034-418b-984f-d22547a86077";
        private const string D_CLS_OUTCOMES_GUID = "a1ed2fea-066c-4574-988c-77bb3c7ce53c";

        private const string D_CLS_IFS_REFS_GUID = "f78a0175-401c-4935-acb9-aa847a48f246";
        private const string D_CLS_OFS_REFS_GUID = "78a92ed2-7d65-4ab6-b474-e70bb9f69933";
        private const string D_CLS_ARREARS_REFS_GUID = "db09e3c9-6db1-4a7d-b91b-98554686d501";
        private const string D_CLS_OUTCOMES_REFS_GUID = "336a7437-9a61-42d7-9c87-38de499aa842";
        private const string D_CLS_EXCESS_REFS_GUID = "4f76dff0-368a-4259-89ea-3a8662bcbb90";

        private const string F_R_FKMR_ADD_GUID = "ae449653-db39-4934-b98e-f3fa9410e1d4";
        private const string F_DP_FKMR_DEF_PROF_GUID = "ef451704-6a0a-4101-b5bb-9fa67c68afe0";
        private const string F_D_FKMR_INCOMES_GUID = "c889ea71-3df3-4cd7-9a2d-b08525e9d804";
        private const string F_R_FKMR_OUTCOMES_GUID = "004f4640-d92d-4998-85ab-55d4ad472b95";
        private const string F_SRC_FIN_FKMR_SRC_FIN_GUID = "e22fec47-5d14-4ded-8ec7-c3b450bc11ba";

        private const string F_IFS_REFS_GUID = "aa555f2c-2dda-4f22-8657-747a7c56024e";
        private const string F_OFS_REFS_GUID = "89006e0b-a3f9-426a-aa34-43f4f828be7f";
        private const string F_ARREARS_REFS_GUID = "eb81f997-9776-4d2e-a57e-ffa2e66d64dc";
        private const string F_OUTCOMES_REFS_GUID = "7c546c7d-609b-494d-be71-f1ee8279d023";
        private const string F_EXCESS_REFS_GUID = "fb0bb6c9-9282-40e4-a861-4c1eddccf483";

        #endregion GUID
        protected override void InitDBObjects()
        {
            this.UsedClassifiers = new IClassifier[] {
                clsKD = this.Scheme.Classifiers[D_KD_FKMR_GUID],
                clsKIF = this.Scheme.Classifiers[D_KIF_FKMR_GUID],
                clsFKR = this.Scheme.Classifiers[D_FKR_FKMR_GUID],
                clsEKR2005 = this.Scheme.Classifiers[D_EKR_FKMR_2005_GUID],
                clsMarksOutcomes = this.Scheme.Classifiers[D_MARKS_FKMR_OUTCOMES_GUID],
                clsMarks = this.Scheme.Classifiers[D_MARKS_FKMR_GUID],
                clsTerritoryFK = this.Scheme.Classifiers[D_TERRITORY_FK_GUID],
                clsOutcomes = this.Scheme.Classifiers[D_CLS_OUTCOMES_GUID], 
            
                clsIFSRefsCls = this.Scheme.Classifiers[D_CLS_IFS_REFS_GUID], 
                clsOFSRefsCls = this.Scheme.Classifiers[D_CLS_OFS_REFS_GUID], 
                clsArrearsRefsCls = this.Scheme.Classifiers[D_CLS_ARREARS_REFS_GUID], 
                clsOutcomesRefsCls = this.Scheme.Classifiers[D_CLS_OUTCOMES_REFS_GUID], 
                clsExcessRefsCls = this.Scheme.Classifiers[D_CLS_EXCESS_REFS_GUID] };

            this.VersionClassifiers = new IClassifier[] { clsKD, clsKIF };

            this.UsedFacts = new IFactTable[] {
                fctFKMRAdd = this.Scheme.FactTables[F_R_FKMR_ADD_GUID],
                fctFKMRDefProf = this.Scheme.FactTables[F_DP_FKMR_DEF_PROF_GUID],
                fctFKMRIncomes = this.Scheme.FactTables[F_D_FKMR_INCOMES_GUID],
                fctFKMROutcomes = this.Scheme.FactTables[F_R_FKMR_OUTCOMES_GUID],
                fctFKMRSrcFin = this.Scheme.FactTables[F_SRC_FIN_FKMR_SRC_FIN_GUID], 

                fctIFSRefsFact = this.Scheme.FactTables[F_IFS_REFS_GUID], 
                fctOFSRefsFact = this.Scheme.FactTables[F_OFS_REFS_GUID], 
                fctArrearsRefsFact = this.Scheme.FactTables[F_ARREARS_REFS_GUID], 
                fctOutcomesRefsFact = this.Scheme.FactTables[F_OUTCOMES_REFS_GUID], 
                fctExcessRefsFact = this.Scheme.FactTables[F_EXCESS_REFS_GUID] };
        }

        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsFKMRAdd);
            ClearDataSet(ref dsFKMRDefProf);
            ClearDataSet(ref dsFKMRIncomes);
            ClearDataSet(ref dsFKMROutcomes);
            ClearDataSet(ref dsFKMRSrcFin);

            ClearDataSet(ref dsIFSRefsFact);
            ClearDataSet(ref dsOFSRefsFact);
            ClearDataSet(ref dsArrearsRefsFact);
            ClearDataSet(ref dsOutcomesRefsFact);
            ClearDataSet(ref dsExcessRefsFact);

            ClearDataSet(ref dsFKR);
            ClearDataSet(ref dsEKR2005);
            ClearDataSet(ref dsKD);
            ClearDataSet(ref dsKIF);
            ClearDataSet(ref dsMarks);
            ClearDataSet(ref dsMarksOutcomes);
            ClearDataSet(ref dsTerritoryFK);
            ClearDataSet(ref dsClsOutcomes);

            ClearDataSet(ref dsIFSRefsCls);
            ClearDataSet(ref dsOFSRefsCls);
            ClearDataSet(ref dsArrearsRefsCls);
            ClearDataSet(ref dsOutcomesRefsCls);
            ClearDataSet(ref dsExcessRefsCls);
        }

        #endregion ������ � ����� � ������

        #region ������ � html

        #region ��������� ������� � ������� �� ������ 2006 ����

        /// <summary>
        /// ���������� �������� ����� ��������������� �� � ���
        /// </summary>
        /// <param name="kdCodeField">��� ��</param>
        /// <param name="kifCodeField">��� ���</param>
        private void GetClsCodeFields(ref string kdCodeField, ref string kifCodeField)
        {
            kdCodeField = GetClsCodeField(clsKD);
            kifCodeField = GetClsCodeField(clsKIF);
        }

        private void SetReportFormat()
        {
            if (this.DataSource.Year >= 2011)
                reportFormat = ReportFormat.Format2011;
            else if (this.DataSource.Year * 100 + this.DataSource.Month >= 200703)
                reportFormat = ReportFormat.March2007;
            else if (this.DataSource.Year * 100 + this.DataSource.Month >= 200611)
                reportFormat = ReportFormat.November2006;
            else if (this.DataSource.Year >= 2006)
                reportFormat = ReportFormat.Format2006;
            else if (this.DataSource.Year * 100 + this.DataSource.Month >= 200510)
                reportFormat = ReportFormat.October2005;
            else if (this.DataSource.Year * 100 + this.DataSource.Month == 200202)
                reportFormat = ReportFormat.February2002;
            else
                reportFormat = ReportFormat.Format2005;
        }

        /// <summary>
        /// ���������� ������ ������
        /// </summary>
        /// <param name="factTable">������� ������</param>
        /// <param name="htmlRow">�������� ������</param>
        /// <param name="fct">������ ������� ������</param>
        /// <param name="columnsMapping">������ ��� ���� ���_�������_������-�����_���_�������</param>
        /// <param name="clsRefsMapping">������ �������� ������ �� ��������������</param>
        /// <param name="budgetLevel">������� �������</param>
        private void PumpFactRow(DataTable factTable, DataRow htmlRow, IFactTable fct, object[] columnsMapping,
            object[] clsRefsMapping, int budgetLevel)
        {
            object[] sumValues = new object[columnsMapping.GetLength(0)];

            bool zeroSum = true;
            int count = columnsMapping.GetLength(0);
            for (int i = 0; i < count; i += 2)
            {
                sumValues[i] = columnsMapping[i];
                int colIndex = Convert.ToInt32(columnsMapping[i + 1]);
                string value = CommonRoutines.TrimLetters(htmlRow[colIndex].ToString().Trim());
                decimal sum = Convert.ToDecimal(value.PadLeft(1, '0'));
                if (sum != 0)
                    zeroSum = false;
                sumValues[i + 1] = sum;
            }

            if (!zeroSum)
            {
                PumpRow(factTable, (object[])CommonRoutines.ConcatArrays(sumValues, new object[] { 
                    "RefYearDayUNV", sourceDate, "REFBUDGETLEVELS", budgetLevel }, clsRefsMapping));
            }
        }

        /// <summary>
        /// ���������� ������ ������� ������
        /// </summary>
        /// <param name="htmlRow">������ �������� � ������� ����� ����</param>
        /// <param name="factTable">������� ������</param>
        /// <param name="clsRefsMapping">������ ��� ����_������_��_�������������-��������_������</param>
        private void ProcessFactRow(DataRow htmlRow, DataTable factTable, IDbDataAdapter da, IFactTable fct,
            object[] clsRefsMapping, FKMonthRepBlock repBlock)
        {
            switch (reportFormat)
            {
                case ReportFormat.February2002:
                    PumpFactRow(factTable, htmlRow, fct, new object[] { "FACTREPORT", 8 }, clsRefsMapping, 2);
                    PumpFactRow(factTable, htmlRow, fct, new object[] { "FACTREPORT", 9 }, clsRefsMapping, 3);
                    PumpFactRow(factTable, htmlRow, fct, new object[] { "FACTREPORT", 10 }, clsRefsMapping, 7);
                    break;

                case ReportFormat.Format2005:
                    PumpFactRow(factTable, htmlRow, fct, new object[] { "FACTREPORT", 2 }, clsRefsMapping, 2);
                    PumpFactRow(factTable, htmlRow, fct, new object[] { "FACTREPORT", 3 }, clsRefsMapping, 3);
                    PumpFactRow(factTable, htmlRow, fct, new object[] { "FACTREPORT", 4 }, clsRefsMapping, 7);
                    break;

                case ReportFormat.October2005:
                    PumpFactRow(factTable, htmlRow, fct, new object[] { "FACTREPORT", 7, "ASSIGNEDREPORT", 3 }, clsRefsMapping, 2);
                    PumpFactRow(factTable, htmlRow, fct, new object[] { "FACTREPORT", 8, "ASSIGNEDREPORT", 4 }, clsRefsMapping, 3);
                    PumpFactRow(factTable, htmlRow, fct, new object[] { "FACTREPORT", 10, "ASSIGNEDREPORT", 6 }, clsRefsMapping, 7);
                    PumpFactRow(factTable, htmlRow, fct, new object[] { "FACTREPORT", 9, "ASSIGNEDREPORT", 5 }, clsRefsMapping, 8);
                    break;

                case ReportFormat.Format2006:
                    PumpFactRow(factTable, htmlRow, fct, new object[] { "FACTREPORT", 11, "ASSIGNEDREPORT", 3 }, clsRefsMapping, 1);
                    PumpFactRow(factTable, htmlRow, fct, new object[] { "FACTREPORT", 12, "ASSIGNEDREPORT", 4 }, clsRefsMapping, 2);
                    PumpFactRow(factTable, htmlRow, fct, new object[] { "FACTREPORT", 13, "ASSIGNEDREPORT", 5 }, clsRefsMapping, 3);
                    PumpFactRow(factTable, htmlRow, fct, new object[] { "FACTREPORT", 14, "ASSIGNEDREPORT", 6 }, clsRefsMapping, 4);
                    PumpFactRow(factTable, htmlRow, fct, new object[] { "FACTREPORT", 15, "ASSIGNEDREPORT", 7 }, clsRefsMapping, 5);
                    PumpFactRow(factTable, htmlRow, fct, new object[] { "FACTREPORT", 16, "ASSIGNEDREPORT", 8 }, clsRefsMapping, 6);
                    PumpFactRow(factTable, htmlRow, fct, new object[] { "FACTREPORT", 17, "ASSIGNEDREPORT", 9 }, clsRefsMapping, 7);
                    PumpFactRow(factTable, htmlRow, fct, new object[] { "FACTREPORT", 18, "ASSIGNEDREPORT", 10 }, clsRefsMapping, 8);
                    break;
                case ReportFormat.November2006:
                    if (repBlock != FKMonthRepBlock.Outcomes)
                    {
                        // ������, ��������� ��������������, ���������������
                        PumpFactRow(factTable, htmlRow, fct, new object[] { "FACTREPORT", 16, "ASSIGNEDREPORT", 8 }, clsRefsMapping, 1);
                        PumpFactRow(factTable, htmlRow, fct, new object[] { "FACTREPORT", 17, "ASSIGNEDREPORT", 9 }, clsRefsMapping, 2);
                        PumpFactRow(factTable, htmlRow, fct, new object[] { "FACTREPORT", 18, "ASSIGNEDREPORT", 10 }, clsRefsMapping, 3);
                        PumpFactRow(factTable, htmlRow, fct, new object[] { "FACTREPORT", 19, "ASSIGNEDREPORT", 11 }, clsRefsMapping, 4);
                        PumpFactRow(factTable, htmlRow, fct, new object[] { "FACTREPORT", 20, "ASSIGNEDREPORT", 12 }, clsRefsMapping, 5);
                        PumpFactRow(factTable, htmlRow, fct, new object[] { "FACTREPORT", 21, "ASSIGNEDREPORT", 13 }, clsRefsMapping, 6);
                        PumpFactRow(factTable, htmlRow, fct, new object[] { "FACTREPORT", 22, "ASSIGNEDREPORT", 14 }, clsRefsMapping, 7);
                        PumpFactRow(factTable, htmlRow, fct, new object[] { "FACTREPORT", 23, "ASSIGNEDREPORT", 15 }, clsRefsMapping, 8);
                    }
                    else
                    {
                        // �������
                        PumpFactRow(factTable, htmlRow, fct, new object[] { "FACTREPORT", 15, "ASSIGNEDREPORT", 7 }, clsRefsMapping, 1);
                        PumpFactRow(factTable, htmlRow, fct, new object[] { "FACTREPORT", 16, "ASSIGNEDREPORT", 8 }, clsRefsMapping, 2);
                        PumpFactRow(factTable, htmlRow, fct, new object[] { "FACTREPORT", 17, "ASSIGNEDREPORT", 9 }, clsRefsMapping, 3);
                        PumpFactRow(factTable, htmlRow, fct, new object[] { "FACTREPORT", 18, "ASSIGNEDREPORT", 10 }, clsRefsMapping, 4);
                        PumpFactRow(factTable, htmlRow, fct, new object[] { "FACTREPORT", 19, "ASSIGNEDREPORT", 11 }, clsRefsMapping, 5);
                        PumpFactRow(factTable, htmlRow, fct, new object[] { "FACTREPORT", 20, "ASSIGNEDREPORT", 12 }, clsRefsMapping, 6);
                        PumpFactRow(factTable, htmlRow, fct, new object[] { "FACTREPORT", 21, "ASSIGNEDREPORT", 13 }, clsRefsMapping, 7);
                        PumpFactRow(factTable, htmlRow, fct, new object[] { "FACTREPORT", 22, "ASSIGNEDREPORT", 14 }, clsRefsMapping, 8);
                    }
                    break;
                case ReportFormat.March2007:
                    if (repBlock != FKMonthRepBlock.Outcomes)
                    {
                        // ������, ��������� ��������������, ���������������
                        PumpFactRow(factTable, htmlRow, fct, new object[] { "FACTREPORT", 16, "ASSIGNEDREPORT", 8 }, clsRefsMapping, 1);
                        PumpFactRow(factTable, htmlRow, fct, new object[] { "FACTREPORT", 17, "ASSIGNEDREPORT", 9 }, clsRefsMapping, 2);
                        PumpFactRow(factTable, htmlRow, fct, new object[] { "FACTREPORT", 18, "ASSIGNEDREPORT", 10 }, clsRefsMapping, 3);
                        PumpFactRow(factTable, htmlRow, fct, new object[] { "FACTREPORT", 19, "ASSIGNEDREPORT", 11 }, clsRefsMapping, 11);
                        PumpFactRow(factTable, htmlRow, fct, new object[] { "FACTREPORT", 20, "ASSIGNEDREPORT", 12 }, clsRefsMapping, 4);
                        PumpFactRow(factTable, htmlRow, fct, new object[] { "FACTREPORT", 21, "ASSIGNEDREPORT", 13 }, clsRefsMapping, 5);
                        PumpFactRow(factTable, htmlRow, fct, new object[] { "FACTREPORT", 22, "ASSIGNEDREPORT", 14 }, clsRefsMapping, 6);
                        PumpFactRow(factTable, htmlRow, fct, new object[] { "FACTREPORT", 23, "ASSIGNEDREPORT", 15 }, clsRefsMapping, 8);
                    }
                    else
                    {
                        // �������
                        PumpFactRow(factTable, htmlRow, fct, new object[] { "FACTREPORT", 15, "ASSIGNEDREPORT", 7 }, clsRefsMapping, 1);
                        PumpFactRow(factTable, htmlRow, fct, new object[] { "FACTREPORT", 16, "ASSIGNEDREPORT", 8 }, clsRefsMapping, 2);
                        PumpFactRow(factTable, htmlRow, fct, new object[] { "FACTREPORT", 17, "ASSIGNEDREPORT", 9 }, clsRefsMapping, 3);
                        PumpFactRow(factTable, htmlRow, fct, new object[] { "FACTREPORT", 18, "ASSIGNEDREPORT", 10 }, clsRefsMapping, 11);
                        PumpFactRow(factTable, htmlRow, fct, new object[] { "FACTREPORT", 19, "ASSIGNEDREPORT", 11 }, clsRefsMapping, 4);
                        PumpFactRow(factTable, htmlRow, fct, new object[] { "FACTREPORT", 20, "ASSIGNEDREPORT", 12 }, clsRefsMapping, 5);
                        PumpFactRow(factTable, htmlRow, fct, new object[] { "FACTREPORT", 21, "ASSIGNEDREPORT", 13 }, clsRefsMapping, 6);
                        PumpFactRow(factTable, htmlRow, fct, new object[] { "FACTREPORT", 22, "ASSIGNEDREPORT", 14 }, clsRefsMapping, 8);
                    }
                    break;
                case ReportFormat.Format2011:
                    switch (repBlock)
                    {
                        case FKMonthRepBlock.Incomes:
                        case FKMonthRepBlock.SrcFin:
                            PumpFactRow(factTable, htmlRow, fct,
                                new object[] { "FACTREPORT", 18, "ASSIGNEDREPORT", 8, "ExcSumPRep", 9, "ExcSumFRep", 19 }, clsRefsMapping, 1);
                            PumpFactRow(factTable, htmlRow, fct,
                                new object[] { "FACTREPORT", 20, "ASSIGNEDREPORT", 10, "ExcSumPRep", 11, "ExcSumFRep", 21 }, clsRefsMapping, 2);
                            PumpFactRow(factTable, htmlRow, fct, new object[] { "FACTREPORT", 22, "ASSIGNEDREPORT", 12 }, clsRefsMapping, 3);
                            PumpFactRow(factTable, htmlRow, fct, new object[] { "FACTREPORT", 23, "ASSIGNEDREPORT", 13 }, clsRefsMapping, 11);
                            PumpFactRow(factTable, htmlRow, fct, new object[] { "FACTREPORT", 24, "ASSIGNEDREPORT", 14 }, clsRefsMapping, 4);
                            PumpFactRow(factTable, htmlRow, fct, new object[] { "FACTREPORT", 25, "ASSIGNEDREPORT", 15 }, clsRefsMapping, 5);
                            PumpFactRow(factTable, htmlRow, fct, new object[] { "FACTREPORT", 26, "ASSIGNEDREPORT", 16 }, clsRefsMapping, 6);
                            PumpFactRow(factTable, htmlRow, fct, new object[] { "FACTREPORT", 27, "ASSIGNEDREPORT", 17 }, clsRefsMapping, 8);
                            break;
                        case FKMonthRepBlock.DefProf:
                            PumpFactRow(factTable, htmlRow, fct, new object[] { "FACTREPORT", 17, "ASSIGNEDREPORT", 7 }, clsRefsMapping, 1);
                            PumpFactRow(factTable, htmlRow, fct, new object[] { "FACTREPORT", 19, "ASSIGNEDREPORT", 9 }, clsRefsMapping, 2);
                            PumpFactRow(factTable, htmlRow, fct, new object[] { "FACTREPORT", 21, "ASSIGNEDREPORT", 11 }, clsRefsMapping, 3);
                            PumpFactRow(factTable, htmlRow, fct, new object[] { "FACTREPORT", 22, "ASSIGNEDREPORT", 12 }, clsRefsMapping, 11);
                            PumpFactRow(factTable, htmlRow, fct, new object[] { "FACTREPORT", 23, "ASSIGNEDREPORT", 13 }, clsRefsMapping, 4);
                            PumpFactRow(factTable, htmlRow, fct, new object[] { "FACTREPORT", 24, "ASSIGNEDREPORT", 14 }, clsRefsMapping, 5);
                            PumpFactRow(factTable, htmlRow, fct, new object[] { "FACTREPORT", 25, "ASSIGNEDREPORT", 15 }, clsRefsMapping, 6);
                            PumpFactRow(factTable, htmlRow, fct, new object[] { "FACTREPORT", 26, "ASSIGNEDREPORT", 16 }, clsRefsMapping, 8);
                            break;
                        case FKMonthRepBlock.Outcomes:
                            PumpFactRow(factTable, htmlRow, fct,
                                new object[] { "FACTREPORT", 17, "ASSIGNEDREPORT", 7, "ExcSumPRep", 8, "ExcSumFRep", 18 }, clsRefsMapping, 1);
                            PumpFactRow(factTable, htmlRow, fct,
                                new object[] { "FACTREPORT", 19, "ASSIGNEDREPORT", 9, "ExcSumPRep", 10, "ExcSumFRep", 20 }, clsRefsMapping, 2);
                            PumpFactRow(factTable, htmlRow, fct, new object[] { "FACTREPORT", 21, "ASSIGNEDREPORT", 11 }, clsRefsMapping, 3);
                            PumpFactRow(factTable, htmlRow, fct, new object[] { "FACTREPORT", 22, "ASSIGNEDREPORT", 12 }, clsRefsMapping, 11);
                            PumpFactRow(factTable, htmlRow, fct, new object[] { "FACTREPORT", 23, "ASSIGNEDREPORT", 13 }, clsRefsMapping, 4);
                            PumpFactRow(factTable, htmlRow, fct, new object[] { "FACTREPORT", 24, "ASSIGNEDREPORT", 14 }, clsRefsMapping, 5);
                            PumpFactRow(factTable, htmlRow, fct, new object[] { "FACTREPORT", 25, "ASSIGNEDREPORT", 15 }, clsRefsMapping, 6);
                            PumpFactRow(factTable, htmlRow, fct, new object[] { "FACTREPORT", 26, "ASSIGNEDREPORT", 16 }, clsRefsMapping, 8);
                            break;
                    }
                    break;
            }

            if (factTable.Rows.Count >= MAX_DS_RECORDS_AMOUNT)
            {
                UpdateData();
                DataSet ds = factTable.DataSet;
                ClearDataSet(da, ref ds);
            }
        }

        /// <summary>
        /// ��������� � ���������� ������ ���������� �� ������ �����
        /// </summary>
        /// <param name="fileName">������������ �����</param>
        /// <param name="subjName">������������ ��������</param>
        private int PumpTerritoryRow(string fileName, string subjName)
        {
            // �������� ���
            //string terrCode = CommonRoutines.TrimLetters(fileName.Split('.')[0]);
            // .. �������� ����� ��� ����������
            string terrCode = fileName.Split('.')[0];
            // .. ��� ��������� ����� �����
            terrCode = CommonRoutines.TrimLetters(terrCode.Substring(terrCode.Length - 2, 2)).PadLeft(2, '0');

            // ���������� ������
            return PumpOriginalRow(dsTerritoryFK, clsTerritoryFK,
                new object[] { "CODE", terrCode, "NAME", subjName });
        }

        #region ������� ������
        /// <summary>
        /// ���������� ������� ���� ������
        /// </summary>
        /// <param name="markName">�������� ������ ������, �� �������� ������������ ����</param>
        /// <returns>����</returns>
        private FKMonthRepBlock GetReportBlock(string markName, int kst, FKMonthRepBlock defaultBlock, int rowIndex,
            Dictionary<int, FKMonthRepBlock> blockMapping)
        {
            switch (reportFormat)
            {
                case ReportFormat.October2005:
                case ReportFormat.Format2006:
                    if (kst <= 99)
                    {
                        return FKMonthRepBlock.Incomes;
                    }
                    else if (kst >= 200 && kst <= 299)
                    {
                        return FKMonthRepBlock.Outcomes;
                    }
                    else if (kst == 450)
                    {
                        return FKMonthRepBlock.DefProf;
                    }
                    else if (kst >= 500 && kst <= 799)
                    {
                        return FKMonthRepBlock.SrcFin;
                    }
                    break;

                default:
                    if (blockMapping != null && blockMapping.ContainsKey(rowIndex))
                    {
                        return blockMapping[rowIndex];
                    }
                    break;
            }

            return defaultBlock;
        }

        /// <summary>
        /// ���������� ������� ���� ������
        /// </summary>
        /// <param name="markName">�������� ������ ������, �� �������� ������������ ����</param>
        /// <returns>����</returns>
        private FKMonthRepBlock GetOldReportCurrentBlock(string markName)
        {
            switch (reportFormat)
            {
                case ReportFormat.October2005:
                case ReportFormat.Format2006:
                    break;

                default:
                    if (markName.ToUpper() == "����� �������")
                    {
                        return FKMonthRepBlock.Incomes;
                    }
                    else if (markName.ToUpper() == "����� ��������")
                    {
                        return FKMonthRepBlock.Outcomes;
                    }
                    else if (markName.ToUpper().Contains("�������"))
                    {
                        return FKMonthRepBlock.DefProf;
                    }
                    else
                    {
                        if ((this.DataSource.Year > 2002 ||
                            (this.DataSource.Year == 2002 && this.DataSource.Month > 1)) && this.DataSource.Year <= 2004)
                        {
                            if (markName.ToUpper() == "����� ���������� ����������� ��������������" ||
                                markName.ToUpper() == "����� ���������� ����������� ��������������" ||
                                markName.ToUpper() == "����� ���������� ��������������" ||
                                markName.ToUpper() == "����� ���������� ��������������")
                            {
                                return FKMonthRepBlock.SrcFin;
                            }
                            else if (markName.ToUpper() == "����� ���������� �������� ��������������" ||
                                markName.ToUpper() == "����� ���������� �������� ��������������")
                            {
                                return FKMonthRepBlock.SrcOutFin;
                            }
                        }
                        else
                        {
                            if (markName.ToUpper() == "����� ���������� ��������������" ||
                                markName.ToUpper() == "����� ���������� ��������������")
                            {
                                return FKMonthRepBlock.SrcFin;
                            }
                        }
                    }
                    break;
            }

            return FKMonthRepBlock.Unknown;
        }

        /// <summary>
        /// ���������� ������� ������
        /// </summary>
        /// <param name="dt">������� � ������� ������</param>
        /// <returns>������� ������. ���� - ����� ������ ������ �����, �������� - �������� �����.</returns>
        private Dictionary<int, FKMonthRepBlock> GetBlockMargins(DataTable dt)
        {
            if ((reportFormat != ReportFormat.Format2005) && (reportFormat != ReportFormat.February2002))
                return null;

            Dictionary<int, FKMonthRepBlock> result = new Dictionary<int, FKMonthRepBlock>(20);

            // ����� ������ ������ �����
            int blockStartRow = 0;
            FKMonthRepBlock repBlock = FKMonthRepBlock.Unknown;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string markName = Convert.ToString(dt.Rows[i][1]);
                repBlock = GetOldReportCurrentBlock(markName);

                // ���� ����� ������ ������� ������-�� �����, �� ���������� � ������ ����� ��� ������ ������
                if (repBlock != FKMonthRepBlock.Unknown)
                {
                    result.Add(blockStartRow, repBlock);
                    blockStartRow = i + 1;
                }
            }

            if (repBlock == FKMonthRepBlock.Unknown)
            {
                result.Add(blockStartRow, FKMonthRepBlock.Add);
            }

            return result;
        }
        #endregion

        /// <summary>
        /// ���������� ������ ������ ����� ������
        /// </summary>
        /// <param name="ds">������� � ������� �����</param>
        /// <param name="fileName">������������ �����</param>
        /// <param name="subjName">������������ ��������</param>
        private void PumpHtmlFileData(DataSet ds, string fileName, string subjName)
        {
            DataTable dt = ds.Tables[1];

            // ����������
            int terrID = PumpTerritoryRow(fileName, subjName);
            FKMonthRepBlock repBlock = FKMonthRepBlock.Incomes;

            string kdCodeField = string.Empty;
            string kifCodeField = string.Empty;

            GetClsCodeFields(ref kdCodeField, ref kifCodeField);

            // FMQ00005147 ������� : � ������ 2006 ���� ������ ������ ������������ ������� ���������
            // � ��� ������ ��������� �������������� ������� �� ������.
            // ������� ����� ������� �������� � ��������� ���������
            if ((reportFormat == ReportFormat.November2006) || (reportFormat == ReportFormat.March2007) ||
                (reportFormat == ReportFormat.Format2011))
            {
                PumpHtmlFileDataForNovember2006(ds, fileName, subjName, terrID, kdCodeField, kifCodeField);
                return;
            }

            Dictionary<int, FKMonthRepBlock> blockMapping = GetBlockMargins(dt);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string kbk = string.Empty;
                string markName = string.Empty;
                int kst = -1;

                switch (reportFormat)
                {
                    case ReportFormat.October2005:
                    case ReportFormat.Format2006:
                        kbk = Convert.ToString(dt.Rows[i][2]);
                        markName = Convert.ToString(dt.Rows[i][0]).Trim();
                        kst = Convert.ToInt32(dt.Rows[i][1]);
                        break;

                    default:
                        kbk = Convert.ToString(dt.Rows[i][0]);
                        markName = Convert.ToString(dt.Rows[i][1]).Trim();
                        break;
                }

                repBlock = GetReportBlock(markName, kst, repBlock, i, blockMapping);

                // ���������� �� ������������� �����
                switch (repBlock)
                {
                    #region FKMonthRepBlock.Add
                    case FKMonthRepBlock.Add:
                        if (!toPumpReference)
                            break;

                        int markID = PumpCachedRow(marksCache, dsMarks.Tables[0], clsMarks, kbk, "CODE",
                            new object[] { "NAME", markName });

                        ProcessFactRow(dt.Rows[i], dsFKMRAdd.Tables[0], daFKMRAdd, fctFKMRAdd,
                            new object[] { "REFTERRITORY", terrID, "REFMARKS", markID }, repBlock);

                        break;
                    #endregion

                    #region FKMonthRepBlock.DefProf
                    case FKMonthRepBlock.DefProf:

                        if (!toPumpDefProf)
                            break;

                        ProcessFactRow(dt.Rows[i], dsFKMRDefProf.Tables[0], daFKMRDefProf, fctFKMRDefProf,
                            new object[] { "REFTERRITORY", terrID }, repBlock);

                        break;
                    #endregion

                    #region FKMonthRepBlock.Incomes
                    case FKMonthRepBlock.Incomes:

                        if (!toPumpIncomes)
                            break;
                        int kdID = -1;
                        if (this.DataSource.Year >= 2005)
                            kdID = PumpCachedRow(kdCache, dsKD.Tables[0], clsKD, kbk.PadRight(3, '0'), kdCodeField,
                                new object[] { "NAME", markName, "STRINGCODE", kst });
                        else if (this.DataSource.Year > 2002 || (this.DataSource.Year == 2002 && this.DataSource.Month > 1))
                            kdID = PumpCachedRow(kdCache, dsKD.Tables[0], clsKD, kbk.PadRight(3, '0'), kdCodeField,
                                new object[] { "NAME", markName });
                        else
                            kdID = PumpCachedRow(kdCache, dsKD.Tables[0], clsKD, kbk.PadRight(3, '0'), kdCodeField,
                                new object[] { "NAME", markName });
                        ProcessFactRow(dt.Rows[i], dsFKMRIncomes.Tables[0], daFKMRIncomes, fctFKMRIncomes, 
                            new object[] { "REFTERRITORY", terrID, "REFKD", kdID }, repBlock);

                        break;
                    #endregion

                    #region FKMonthRepBlock.Outcomes
                    case FKMonthRepBlock.Outcomes:

                        if (!toPumpOutcomes)
                            break;

                        if (this.DataSource.Year > 2005 ||
                            (this.DataSource.Year == 2005 && this.DataSource.Month >= 10))
                        {
                            if (markName == string.Empty)
                                markName = kbk.Substring(17, 3);
                            string code = kbk.Substring(17, 3);

                            string fkr = kbk.Substring(3, 4);
                            // ���������� ������� �� ��� (��� = 0)
                            if (code.TrimStart('0') != string.Empty)
                                PumpOutcomesCls(constDefaultClsName, string.Empty, fkr, string.Empty, string.Empty, string.Empty);

                            int refOutcomes = PumpOutcomesCls(markName, string.Empty, fkr,
                                string.Empty, string.Empty, code);
                            ProcessFactRow(dt.Rows[i], dsFKMROutcomes.Tables[0], daFKMROutcomes, fctFKMROutcomes,
                                new object[] { "REFTERRITORY", terrID, "REFFKR", nullFKR, "RefR", refOutcomes,
                                    "REFMARKS", nullMarksOutcomes, "REFEKR2005", nullEKR2005 }, repBlock);
                        }
                        else if (this.DataSource.Year * 100 + this.DataSource.Month >= 200202)
                        {
                            int refOutcomes = PumpOutcomesCls(markName, string.Empty, kbk, 
                                string.Empty, string.Empty, string.Empty);
                            ProcessFactRow(dt.Rows[i], dsFKMROutcomes.Tables[0], daFKMROutcomes, fctFKMROutcomes,
                                new object[] { "REFTERRITORY", terrID, "REFFKR", nullFKR, "REFMARKS", 
                                    nullMarksOutcomes, "REFEKR2005", nullEKR2005, "RefR", refOutcomes }, repBlock);
                        }
                        else
                        {
                            int marksID = PumpCachedRow(marksOutcomesCache, dsMarksOutcomes.Tables[0], clsMarksOutcomes,
                                kbk, "CODESTR", new object[] { "NAME", markName });

                            ProcessFactRow(dt.Rows[i], dsFKMROutcomes.Tables[0], daFKMROutcomes, fctFKMROutcomes,
                                new object[] { "REFTERRITORY", terrID, "REFMARKS", marksID, "REFFKR", nullFKR, "REFEKR2005", nullEKR2005 }, repBlock);
                        }

                        break;
                    #endregion

                    #region FKMonthRepBlock.SrcFin, FKMonthRepBlock.SrcOutFin
                    case FKMonthRepBlock.SrcFin:
                    case FKMonthRepBlock.SrcOutFin:

                        if (!toPumpFinSources)
                            break;

                        int kifID = -1;
                        if (this.DataSource.Year >= 2005)
                            kifID = PumpCachedRow(kifCache, dsKIF.Tables[0], clsKIF, kbk, kifCodeField,
                                new object[] { "NAME", markName, "STRINGCODE", kst });
                        else if (this.DataSource.Year > 2002 || (this.DataSource.Year == 2002 && this.DataSource.Month > 1))
                            switch (repBlock)
                            {
                                case FKMonthRepBlock.SrcFin:
                                    // ��� ������� �� 2002 ��� ��������� �� ����� ���������� �� 10.
                                    if (this.DataSource.Year == 2002 && this.DataSource.Month > 1) 
                                        kbk += "0";
                                    kifID = PumpCachedRow(kifCache, dsKIF.Tables[0], clsKIF, kbk, kifCodeField,
                                        new object[] { "NAME", markName });
                                    break;
                                case FKMonthRepBlock.SrcOutFin:
                                    kifID = PumpCachedRow(kifCache, dsKIF.Tables[0], clsKIF, kbk, "CODESTR",
                                        new object[] { "NAME", markName });
                                    break;
                            }
                        else
                            kifID = PumpCachedRow(kifCache, dsKIF.Tables[0], clsKIF, kbk, kifCodeField,
                                new object[] { "NAME", markName });

                        ProcessFactRow(dt.Rows[i], dsFKMRSrcFin.Tables[0], daFKMRSrcFin, fctFKMRSrcFin, 
                            new object[] { "REFTERRITORY", terrID, "REFKIF", kifID }, repBlock);
                        break;

                    #endregion
                }
            }
        }

        #endregion ��������� ������� � ������� �� ������ 2006 ����

        #region ��������� ������� � ������� � ������ 2006 ����

        private string GetKbkForNovember2006(DataRow htmlRow)
        {
            return String.Concat(
                htmlRow[2].ToString(),
                htmlRow[3].ToString(),
                htmlRow[4].ToString(),
                htmlRow[5].ToString(),
                htmlRow[6].ToString(),
                htmlRow[7].ToString()
            );
        }

        private int PumpOutcomesCls(string name, string kvsr, string fkr, string kcsr, string kvr, string ekr)
        {
            // ��� �������� = ���+���+���+���+��� (KVSR+FKR+KCSR+KVR+EKR)
            kvsr = kvsr.Trim().Replace("*", string.Empty).PadLeft(3, '0');
            fkr = fkr.Trim().PadLeft(4, '0');
            kcsr = kcsr.Trim().PadLeft(7, '0');
            kvr = kvr.Trim().PadLeft(3, '0');
            ekr = ekr.Trim().PadLeft(3, '0');
            string codeStr = string.Concat(kvsr, fkr, kcsr, kvr, ekr);
            return PumpCachedRow(clsOutcomesCache, dsClsOutcomes.Tables[0], clsOutcomes, codeStr,
                new object[] { "CodeStr", codeStr, "Name", name, "KVSR", kvsr, "FKR", fkr, "KCSR", kcsr, "KVR", kvr, "EKR", ekr });
        }

        private void PumpHtmlFileDataForNovember2006(DataSet ds, string fileName, string subjName,
            int terrID, string kdCodeField, string kifCodeField)
        {
            for (int curTableIndex = 1; curTableIndex < ds.Tables.Count; curTableIndex++)
            {
                DataTable dt = ds.Tables[curTableIndex];
                DataColumn codeStrColumn = dt.Columns[1];
                FKMonthRepBlock mainRepBlock = FKMonthRepBlock.Unknown;
                switch (curTableIndex)
                {
                    case 1:
                        mainRepBlock = FKMonthRepBlock.Incomes;
                        break;
                    case 2:
                        mainRepBlock = FKMonthRepBlock.Outcomes;
                        break;
                    case 3:
                        mainRepBlock = FKMonthRepBlock.SrcFin;
                        break;
                    case 5:
                        // � 20080300 ��������� ������� ������: 4 "������� (�����������)" - ���������� �� �����
                        continue;
                }

                foreach (DataRow htmlRow in dt.Rows)
                {
                    #region ���������� ����� ���� ����������
                    // ���������� ����� ���� ����������
                    FKMonthRepBlock repBlock = mainRepBlock;
                    // .. ������� �������� ��� ������ - �� ��������������� �� ��� ?
                    int codeStr = GetIntCellValue(htmlRow, codeStrColumn, 0);
                    if (codeStr == 450)
                    {
                        repBlock = FKMonthRepBlock.DefProf;
                    }
                    #endregion

                    #region ������� ������ �����
                    string markName = htmlRow[0].ToString();
                    string kbk;
                    switch (repBlock)
                    {
                        #region ������
                        case FKMonthRepBlock.Incomes:

                            if (!toPumpIncomes)
                                break;

                            kbk = GetKbkForNovember2006(htmlRow);
                            // �������� ��������� �� ����
                            kbk = kbk.Replace("*", "0");
                            int kdID = PumpCachedRow(kdCache, dsKD.Tables[0], clsKD, kbk.PadRight(3, '0'), kdCodeField,
                                new object[] { "NAME", markName, "STRINGCODE", codeStr });

                            ProcessFactRow(htmlRow, dsFKMRIncomes.Tables[0], daFKMRIncomes, fctFKMRIncomes, new object[] { 
                                "REFTERRITORY", terrID, "REFKD", kdID }, mainRepBlock);
                            break;
                        #endregion
                        #region �������
                        case FKMonthRepBlock.Outcomes:
                            if (!toPumpOutcomes)
                                break;
                            string ekr = htmlRow[6].ToString().Trim().TrimStart('0');
                            // ���������� ������� �� ��� (��� = 0)
                            if (ekr != string.Empty)
                                PumpOutcomesCls(constDefaultClsName, string.Empty,
                                    htmlRow[3].ToString(), string.Empty, string.Empty, string.Empty);

                            int refOutcomes = PumpOutcomesCls(htmlRow[0].ToString().Trim(), htmlRow[2].ToString(),
                                htmlRow[3].ToString(), htmlRow[4].ToString(), htmlRow[5].ToString(), ekr);
                            ProcessFactRow(htmlRow, dsFKMROutcomes.Tables[0], daFKMROutcomes, fctFKMROutcomes,
                                new object[] { "REFTERRITORY", terrID, "REFFKR", nullFKR, "REFMARKS", nullMarksOutcomes, 
                                    "REFEKR2005", nullEKR2005, "RefR", refOutcomes }, mainRepBlock);
                            break;
                        #endregion
                        #region ��������� ��������������
                        case FKMonthRepBlock.SrcFin:

                            if (!toPumpFinSources)
                                break;

                            kbk = GetKbkForNovember2006(htmlRow);
                            // �������� ��������� �� ����
                            kbk = kbk.Replace("*", "0");
                            int kifID = PumpCachedRow(kifCache, dsKIF.Tables[0], clsKIF, kbk, kifCodeField,
                                new object[] { "NAME", markName, "STRINGCODE", codeStr });

                            ProcessFactRow(htmlRow, dsFKMRSrcFin.Tables[0], daFKMRSrcFin, fctFKMRSrcFin, 
                                new object[] { "REFTERRITORY", terrID, "REFKIF", kifID }, mainRepBlock);
                            break;
                        #endregion
                        #region ������� ��������
                        case FKMonthRepBlock.DefProf:

                            if (!toPumpDefProf)
                                break;

                            ProcessFactRow(htmlRow, dsFKMRDefProf.Tables[0], daFKMRDefProf, fctFKMRDefProf,
                                new object[] { "REFTERRITORY", terrID }, mainRepBlock);
                            break;
                        #endregion
                    }
                    #endregion

                }

            }
        }
        #endregion

        #endregion ������ � html

        #region ��������� ��������

        private void FillKDHierarchy()
        {
            kdHierarchy2001.Clear();
            kdHierarchy2001.Add(30, 10);
            kdHierarchy2001.Add(31, 30);
            kdHierarchy2001.Add(32, 30);
            kdHierarchy2001.Add(33, 30);
            kdHierarchy2001.Add(50, 10);
            kdHierarchy2001.Add(51, 50);
            kdHierarchy2001.Add(52, 50);
            kdHierarchy2001.Add(53, 50);
            kdHierarchy2001.Add(54, 50);
            kdHierarchy2001.Add(55, 50);
            kdHierarchy2001.Add(56, 50);
            kdHierarchy2001.Add(70, 10);
            kdHierarchy2001.Add(71, 70);
            kdHierarchy2001.Add(72, 70);
            kdHierarchy2001.Add(90, 10);
            kdHierarchy2001.Add(110, 10);
            kdHierarchy2001.Add(111, 110);
            kdHierarchy2001.Add(112, 110);
            kdHierarchy2001.Add(113, 110);
            kdHierarchy2001.Add(114, 110);
            kdHierarchy2001.Add(115, 110);
            kdHierarchy2001.Add(116, 110);
            kdHierarchy2001.Add(117, 110);
            kdHierarchy2001.Add(130, 10);
            kdHierarchy2001.Add(150, 10);
            kdHierarchy2001.Add(151, 150);
            kdHierarchy2001.Add(152, 150);
            kdHierarchy2001.Add(153, 150);
            kdHierarchy2001.Add(154, 150);
            kdHierarchy2001.Add(190, 170);
            kdHierarchy2001.Add(210, 170);
            kdHierarchy2001.Add(230, 170);
            kdHierarchy2001.Add(250, 170);
            kdHierarchy2001.Add(270, 170);
            kdHierarchy2001.Add(290, 170);
            kdHierarchy2001.Add(350, 330);
            kdHierarchy2001.Add(351, 350);
            kdHierarchy2001.Add(352, 350);
            kdHierarchy2001.Add(353, 350);
            kdHierarchy2001.Add(354, 350);
            kdHierarchy2001.Add(355, 350);
            kdHierarchy2001.Add(370, 330);
            kdHierarchy2001.Add(390, 330);
            kdHierarchy2001.Add(411, 410);
            kdHierarchy2001.Add(412, 410);
            kdHierarchy2001.Add(413, 410);
            kdHierarchy2001.Add(414, 410);
        }

        private void FillKIFHierarchy()
        {
            kifHierarchy2001.Clear();
            kifHierarchy2001.Add(510, 51);
            kifHierarchy2001.Add(511, 51);
            kifHierarchy2001.Add(531, 53);
            kifHierarchy2001.Add(532, 53);
            kifHierarchy2001.Add(551, 55);
            kifHierarchy2001.Add(552, 55);
            kifHierarchy2001.Add(571, 57);
            kifHierarchy2001.Add(572, 57);
            kifHierarchy2001.Add(591, 59);
            kifHierarchy2001.Add(592, 59);
            kifHierarchy2001.Add(611, 61);
            kifHierarchy2001.Add(612, 61);
            kifHierarchy2001.Add(631, 63);
            kifHierarchy2001.Add(632, 63);
            kifHierarchy2001.Add(651, 65);
            kifHierarchy2001.Add(652, 65);
            kifHierarchy2001.Add(710, 71);
            kifHierarchy2001.Add(711, 71);
            kifHierarchy2001.Add(731, 73);
            kifHierarchy2001.Add(732, 73);
            kifHierarchy2001.Add(751, 75);
            kifHierarchy2001.Add(752, 75);
            kifHierarchy2001.Add(771, 77);
            kifHierarchy2001.Add(772, 77);
            kifHierarchy2001.Add(791, 79);
            kifHierarchy2001.Add(792, 79);
            kifHierarchy2001.Add(811, 81);
            kifHierarchy2001.Add(812, 81);
            kifHierarchy2001.Add(831, 83);
            kifHierarchy2001.Add(832, 83);
        }

        private void FillMarksHierarchy()
        {
            marksHierarchy.Clear();
            marksHierarchy.Add(1020, 102);
            marksHierarchy.Add(1021, 102);
            marksHierarchy.Add(1040, 104);
            marksHierarchy.Add(1041, 104);
            marksHierarchy.Add(1050, 105);
            marksHierarchy.Add(1051, 105);
            marksHierarchy.Add(1091, 109);
        }

        private void FillClsHierarchy()
        {
            FillKDHierarchy();
            FillKIFHierarchy();
            FillMarksHierarchy();
        }

        private void SetClsHierarchy()
        {
            // ������
            int sourceDate = this.DataSource.Year * 100 + this.DataSource.Month;
            ClearHierarchy(dsKD.Tables[0]);
            SetPresentationContext(clsKD);
            if (this.DataSource.Year >= 2011)
            {
                SetClsHierarchy(ref dsKD, clsKD, null, string.Empty, ClsHierarchyMode.Standard);
                FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00010102000010000110'",
                    "CodeStr = '00010102011010000110'");
                FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00010202000000000000'",
                    "CodeStr = '00010202041060000160'");
                FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00011105000000000120'",
                    "CodeStr = '00011105026000000120'");
                FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00011110000010000120'",
                    "CodeStr = '00011111000010000120'");
                FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00011301400010000000'",
                    "CodeStr = '00011301410010000130' or CodeStr = '00011301420010000180' or " +
                    "CodeStr = '00011301430010000410' or CodeStr = '00011301440010000420' or " +
                    "CodeStr = '00011301450010000440'");
                FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00011706000000000180'",
                    "CodeStr = '00011706011060000180'");
                FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00020205100060000151'",
                    "CodeStr = '00020205110060000151' or CodeStr = '00020205111060000151' or " +
                    "CodeStr = '00020205112060000151' or CodeStr = '00020205113060000151' or " +
                    "CodeStr = '00020205114060000151' or CodeStr = '00020205115060000151' or " +
                    "CodeStr = '00020205116060000151' or CodeStr = '00020205117060000151' or " +
                    "CodeStr = '00020205119060000151'");
                FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00020205100060000151'",
                    "CodeStr = '00020205120060000151' or CodeStr = '00020205122060000151' or " +
                    "CodeStr = '00020205123060000151'");
                FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00020205300070000151'",
                    "CodeStr = '00020205310070000151' or CodeStr = '00020205311070000151' or " +
                    "CodeStr = '00020205312070000151' or CodeStr = '00020205313070000151' or " +
                    "CodeStr = '00020205314070000151' or CodeStr = '00020205315070000151' or " +
                    "CodeStr = '00020205316070000151'");
                FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00020205800090000151'",
                    "CodeStr = '00020205810090000151'");
                FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00020701010010000180'",
                    "CodeStr = '00020701011010000180' or CodeStr = '00020701012010000180'");
                FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00021906010000000151'",
                    "CodeStr = '00021906011060000151' or CodeStr = '00021906012070000151' or " +
                    "CodeStr = '00021906013080000151' or CodeStr = '00021906014090000151'");
                FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00021906020000000151'",
                    "CodeStr = '00021906021060000151' or CodeStr = '00021906022070000151' or " +
                    "CodeStr = '00021906023080000151' or CodeStr = '00021906024090000151'");
                FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00021906030000000151'",
                    "CodeStr = '00021906031060000151' or CodeStr = '00021906032070000151' or " +
                    "CodeStr = '00021906033080000151' or CodeStr = '00021906034090000151'");
                FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00087000000000000000'",
                    "CodeStr = '00087000000000001151' or CodeStr = '00087000000000002151'");
            }
            else if (this.DataSource.Year >= 2010)
            {
                SetClsHierarchy(ref dsKD, clsKD, null, string.Empty, ClsHierarchyMode.Standard);
                FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00010102000010000110'",
                    "CodeStr = '00010102011010000110'");
                FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00010202000000000000'",
                    "CodeStr = '00010202041060000160'");
                FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00011105000000000120'",
                    "CodeStr = '00011105026000000120'");
                FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00011500000000000140'",
                    "CodeStr = '00011502011060000140'");
                FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00011706000000000180'",
                    "CodeStr = '00011706011060000180'");
                FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00020205300070000151'",
                    "CodeStr = '00020205310070000151' or CodeStr = '00020205311070000151' or " +
                    "CodeStr = '00020205312070000151' or CodeStr = '00020205313070000151' or " +
                    "CodeStr = '00020205314070000151' or CodeStr = '00020205315070000151' or " +
                    "CodeStr = '00020205316070000151'");
                FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00020205100060000151'",
                    "CodeStr = '00020205110060000151' or CodeStr = '00020205111060000151' or " +
                    "CodeStr = '00020205112060000151' or CodeStr = '00020205113060000151' or " +
                    "CodeStr = '00020205114060000151' or CodeStr = '00020205115060000151' or " +
                    "CodeStr = '00020205116060000151' or CodeStr = '00020205117060000151' or " +
                    "CodeStr = '00020205119060000151'");
                FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00020205100060000151'",
                    "CodeStr = '00020205120060000151' or CodeStr = '00020205122060000151' or " +
                    "CodeStr = '00020205123060000151'");
                FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00087000000000000000'",
                    "CodeStr = '00087000000000001151' or CodeStr = '00087000000000002151'");
            }
            else if (this.DataSource.Year >= 2008)
            {
                SetClsHierarchy(ref dsKD, clsKD, null, string.Empty, ClsHierarchyMode.Standard);
                // ��� ��������� ��� 2008
                FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00020205100060000151'",
                    "CodeStr = '00020205110060000151' or CodeStr = '00020205111060000151' or " +
                    "CodeStr = '00020205112060000151' or CodeStr = '00020205113060000151' or " +
                    "CodeStr = '00020205114060000151' or CodeStr = '00020205115060000151'");
                FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00087000000000000000'",
                    "CodeStr = '00087100000000000000' or CodeStr = '00087200000000000000'");
                FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00010102000010000110'",
                    "CodeStr = '00010102011010000110'");
                FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00020205800090000151'",
                    "CodeStr = '00020205810090000151'");
                FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00010202000000000000'",
                    "CodeStr = '00010202041060000160'");
                FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00011706000000000180'",
                    "CodeStr = '00011706011060000180'");
                FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00011105000000000120'",
                    "CodeStr = '00011105026000000120'");
                if ((sourceDate >= 200808) && (sourceDate <= 200812))
                {
                    FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00011402000000000000'",
                        "CodeStr = '00011402022020000410' or CodeStr = '00011402023020000410' or " +
                        "CodeStr = '00011402022020000440' or CodeStr = '00011402023020000440' or " +
                        "CodeStr = '00011402032030000410' or CodeStr = '00011402033030000410' or " +
                        "CodeStr = '00011402032030000440' or CodeStr = '00011402033030000440' or " +
                        "CodeStr = '00011402032040000410' or CodeStr = '00011402033040000410' or " +
                        "CodeStr = '00011402032040000440' or CodeStr = '00011402033040000440' or " +
                        "CodeStr = '00011402032050000410' or CodeStr = '00011402033050000410' or " +
                        "CodeStr = '00011402032050000440' or CodeStr = '00011402033050000440' or " +
                        "CodeStr = '00011402032100000410' or CodeStr = '00011402032100000410' or " +
                        "CodeStr = '00011402032100000440' or CodeStr = '00011402033100000440' or " +
                        "CodeStr = '00011402033100000410' or CodeStr = '00011402011010000410' or " +
                        "CodeStr = '00011402013010000410' or CodeStr = '00011402014010000410' or " +
                        "CodeStr = '00011402015010000410' or CodeStr = '00011402016010000410' or " +
                        "CodeStr = '00011402017010000410' or CodeStr = '00011402019010000410' or " +
                        "CodeStr = '00011402011010000440' or CodeStr = '00011402013010000440' or " +
                        "CodeStr = '00011402014010000440' or CodeStr = '00011402015010000440' or " +
                        "CodeStr = '00011402016010000440' or CodeStr = '00011402017010000440' or " +
                        "CodeStr = '00011402018010000440' or CodeStr = '00011402019010000440'");
                }
            }
            else if (this.DataSource.Year >= 2007)
            {
                SetClsHierarchy(ref dsKD, clsKD, null, string.Empty, ClsHierarchyMode.Standard);
                // ��� ��������� ��� 2007
                FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00020203900090000151'",
                    "CodeStr = '00020203901090000151' or CodeStr = '00020203902090000151' or " +
                    "CodeStr = '00020203903090000151' or CodeStr = '00020203999000000151'");
                FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00020200000000000000'",
                    "CodeStr = '00020203999000000151'");
                FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00020204010000000151'",
                    "CodeStr = '00020204011020000151' or CodeStr = '00020204012020000151' or " +
                    "CodeStr = '00020204013020000151' or CodeStr = '00020204014020000151' or " +
                    "CodeStr = '00020204015020000151' or CodeStr = '00020204016020000151' or " +
                    "CodeStr = '00020204017020000151' or CodeStr = '00020204018020000151'");
                FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00020204900090000151'",
                    "CodeStr = '00020204901090000151' or CodeStr = '00020204903090000151' or " +
                    "CodeStr = '00020204904090000151' or CodeStr = '00020204905090000151' or " +
                    "CodeStr = '00020204906090000151' or CodeStr = '00020204908090000151' or " +
                    "CodeStr = '00020204999000000151'");
                FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00020204000000000151'",
                    "CodeStr = '00020204999000000151'");
                FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00020205100060000151'",
                    "CodeStr = '00020205110060000151' or CodeStr = '00020205111060000151' or " +
                    "CodeStr = '00020205112060000151'");
            }
            else if (this.DataSource.Year >= 2005)
                SetClsHierarchy(ref dsKD, clsKD, null, string.Empty, ClsHierarchyMode.Standard);
            else if (this.DataSource.Year > 2002 || (this.DataSource.Year == 2002 && this.DataSource.Month > 1))
                SetClsHierarchy(ref dsKD, clsKD, null, string.Empty, ClsHierarchyMode.KD2004);
            else
                SetClsHierarchy(ref dsKD, clsKD, kdHierarchy2001, "CODE", ClsHierarchyMode.KD2004);

            // ��������� ��������������
            SetPresentationContext(clsKIF);
            if (this.DataSource.Year > 2002 || (this.DataSource.Year == 2002 && this.DataSource.Month > 1))
                SetClsHierarchy(ref dsKIF, clsKIF, null, string.Empty, ClsHierarchyMode.Standard);
            else
                SetClsHierarchy(ref dsKIF, clsKIF, kifHierarchy2001, "CodeStr", ClsHierarchyMode.Special);
            FormClsGroupHierarchy(dsKIF.Tables[0], string.Empty, "CodeStr = '00001050000000000000'");

            // �������
            SetClsHierarchy(ref dsClsOutcomes, clsOutcomes, null, string.Empty, ClsHierarchyMode.Standard);

            if (this.DataSource.Year <= 2001 || (this.DataSource.Year == 2002 && this.DataSource.Month <= 1))
                SetClsHierarchy(ref dsMarksOutcomes, clsMarksOutcomes, null, "CODESTR", ClsHierarchyMode.StartCodeHierarchy);

            // ���������
            SetClsHierarchy(ref dsMarks, clsMarks, marksHierarchy, "CODE", ClsHierarchyMode.Special);

            // ����������.��_������_������������ 
            SetClsHierarchy(ref dsIFSRefsCls, clsIFSRefsCls, null, "Kst", ClsHierarchyMode.Standard);
            if (this.DataSource.Year >= 2011)
            {
                FormClsGroupHierarchy(dsIFSRefsCls.Tables[0], "Kst = '10562'", "Kst = '10563'");
                FormClsGroupHierarchy(dsIFSRefsCls.Tables[0], "Kst = '10564'", "Kst = '10565'");
                FormClsGroupHierarchy(dsIFSRefsCls.Tables[0], "Kst = '10762'", "Kst = '10763'");
                FormClsGroupHierarchy(dsIFSRefsCls.Tables[0], "Kst = '10764'", "Kst = '10765'");
                FormClsGroupHierarchy(dsIFSRefsCls.Tables[0], string.Empty,
                    "Kst = '10562' or Kst = '10564' or Kst = '10570' or Kst = '10762' or Kst = '10764' or Kst = '10770'");
            }
            // ����������.��_������_����������� 
            SetClsHierarchy(ref dsOFSRefsCls, clsOFSRefsCls, null, "Kst", ClsHierarchyMode.Standard);
            if (this.DataSource.Year >= 2011)
            {
                FormClsGroupHierarchy(dsOFSRefsCls.Tables[0], "Kst = '10632'", "Kst = '10633'");
                FormClsGroupHierarchy(dsOFSRefsCls.Tables[0], "Kst = '10634'", "Kst = '10635'");
                FormClsGroupHierarchy(dsOFSRefsCls.Tables[0], string.Empty,
                    "Kst = '10632' or Kst = '10634' or Kst = '10640'");
            }
            // ����������.��_������_����������������  
            SetClsHierarchy(ref dsArrearsRefsCls, clsArrearsRefsCls, null, "Kst", ClsHierarchyMode.Standard);
            if (this.DataSource.Year >= 2011)
            {
                FormClsGroupHierarchy(dsArrearsRefsCls.Tables[0], "Kst = '10900'",
                    "Kst = '10911' or Kst = '10912' or Kst = '10913'");
                FormClsGroupHierarchy(dsArrearsRefsCls.Tables[0], "Kst = '11100'",
                    "Kst = '11200' or Kst = '11300' or Kst = '11400' or Kst = '11500'");
                FormClsGroupHierarchy(dsArrearsRefsCls.Tables[0], string.Empty, "Kst = '11710'");
            }
            // ����������.��_������_���������� 
            SetClsHierarchy(ref dsOutcomesRefsCls, clsOutcomesRefsCls, null, "Kst", ClsHierarchyMode.Standard);
            if (this.DataSource.Year >= 2011)
            {
                FormClsGroupHierarchy(dsOutcomesRefsCls.Tables[0], "Kst = '1802'",
                    "Kst = '1803' or Kst = '1804' or Kst = '1805' or Kst = '1806' or Kst = '1807' or Kst = '1808' or Kst = '1809' or " +
                    "Kst = '1810' or Kst = '1811' or Kst = '1812' or Kst = '1813' or Kst = '1814' or Kst = '1815'");
                FormClsGroupHierarchy(dsOutcomesRefsCls.Tables[0], "Kst = '7000'",
                    "Kst = '7100' or Kst = '7200' or Kst = '7300' or Kst = '7400'");
                FormClsGroupHierarchy(dsOutcomesRefsCls.Tables[0], string.Empty, "Kst = '750' or Kst = '10350'");
            }
        }

        #endregion ��������� ��������

        #region ���������� ������ �������

        protected override void DeleteEarlierPumpedData()
        {
            if (!this.DeleteEarlierData)
                return;

            toPumpIncomes = Convert.ToBoolean(GetParamValueByName(this.PumpRegistryElement.ProgramConfig, "ucbIncomes", "False"));
            toPumpOutcomes = Convert.ToBoolean(GetParamValueByName(this.PumpRegistryElement.ProgramConfig, "ucbOutcomes", "False"));
            toPumpDefProf = Convert.ToBoolean(GetParamValueByName(this.PumpRegistryElement.ProgramConfig, "ucbDefProf", "False"));
            toPumpFinSources = Convert.ToBoolean(GetParamValueByName(this.PumpRegistryElement.ProgramConfig, "ucbFinSources", "False"));
            toPumpReference = Convert.ToBoolean(GetParamValueByName(this.PumpRegistryElement.ProgramConfig, "ucbReference", "False"));

            toPumpIFSRefs = Convert.ToBoolean(GetParamValueByName(this.PumpRegistryElement.ProgramConfig, "ucbInnerFinSourcesRefs", "False")); ;
            toPumpOFSRefs = Convert.ToBoolean(GetParamValueByName(this.PumpRegistryElement.ProgramConfig, "ucbOuterFinSourcesRefs", "False")); ;
            toPumpArrearsRefs = Convert.ToBoolean(GetParamValueByName(this.PumpRegistryElement.ProgramConfig, "ucbArrearsRefs", "False")); ;
            toPumpOutcomesRefs = Convert.ToBoolean(GetParamValueByName(this.PumpRegistryElement.ProgramConfig, "ucbOutcomesRefsAdd", "False")); ;
            toPumpExcessRefs = Convert.ToBoolean(GetParamValueByName(this.PumpRegistryElement.ProgramConfig, "ucbExcessRefs", "False")); ;

            if (toPumpIncomes)
            {
                DirectDeleteFactData(new IFactTable[] { this.Scheme.FactTables[F_D_FKMR_INCOMES_GUID] }, -1, this.SourceID, string.Empty);
                DirectDeleteClsData(new IClassifier[] { clsKD }, -1, this.SourceID, string.Empty);
            }
            if (toPumpOutcomes)
            {
                DirectDeleteFactData(new IFactTable[] { this.Scheme.FactTables[F_R_FKMR_OUTCOMES_GUID] }, -1, this.SourceID, string.Empty);
                DirectDeleteClsData(new IClassifier[] { this.Scheme.Classifiers[D_FKR_FKMR_GUID], this.Scheme.Classifiers[D_EKR_FKMR_2005_GUID],
                    this.Scheme.Classifiers[D_MARKS_FKMR_OUTCOMES_GUID], this.Scheme.Classifiers[D_CLS_OUTCOMES_GUID]}, -1, this.SourceID, string.Empty);
            }
            if (toPumpDefProf)
            {
                DirectDeleteFactData(new IFactTable[] { this.Scheme.FactTables[F_DP_FKMR_DEF_PROF_GUID] }, -1, this.SourceID, string.Empty);
            }
            if (toPumpFinSources)
            {
                DirectDeleteFactData(new IFactTable[] { this.Scheme.FactTables[F_SRC_FIN_FKMR_SRC_FIN_GUID] }, -1, this.SourceID, string.Empty);
                DirectDeleteClsData(new IClassifier[] { clsKIF }, -1, this.SourceID, string.Empty);
            }
            if (toPumpReference)
            {
                DirectDeleteFactData(new IFactTable[] { this.Scheme.FactTables[F_R_FKMR_ADD_GUID] }, -1, this.SourceID, string.Empty);
                DirectDeleteClsData(new IClassifier[] { this.Scheme.Classifiers[D_MARKS_FKMR_GUID] }, -1, this.SourceID, string.Empty);
            }

            if (toPumpIFSRefs)
            {
                DirectDeleteFactData(new IFactTable[] { fctIFSRefsFact }, -1, this.SourceID, string.Empty);
                DirectDeleteClsData(new IClassifier[] { clsIFSRefsCls }, -1, this.SourceID, string.Empty);
            }
            if (toPumpOFSRefs)
            {
                DirectDeleteFactData(new IFactTable[] { fctOFSRefsFact }, -1, this.SourceID, string.Empty);
                DirectDeleteClsData(new IClassifier[] { clsOFSRefsCls }, -1, this.SourceID, string.Empty);
            }
            if (toPumpArrearsRefs)
            {
                DirectDeleteFactData(new IFactTable[] { fctArrearsRefsFact }, -1, this.SourceID, string.Empty);
                DirectDeleteClsData(new IClassifier[] { clsArrearsRefsCls }, -1, this.SourceID, string.Empty);
            }
            if (toPumpOutcomesRefs)
            {
                DirectDeleteFactData(new IFactTable[] { fctOutcomesRefsFact }, -1, this.SourceID, string.Empty);
                DirectDeleteClsData(new IClassifier[] { clsOutcomesRefsCls }, -1, this.SourceID, string.Empty);
            }
            if (toPumpExcessRefs)
            {
                DirectDeleteFactData(new IFactTable[] { fctExcessRefsFact }, -1, this.SourceID, string.Empty);
                DirectDeleteClsData(new IClassifier[] { clsExcessRefsCls }, -1, this.SourceID, string.Empty);
            }

            if ((toPumpIncomes) && (toPumpOutcomes) && (toPumpDefProf) && (toPumpFinSources) && (toPumpReference))
                DirectDeleteClsData(new IClassifier[] { this.Scheme.Classifiers[D_TERRITORY_FK_GUID] }, -1, this.SourceID, string.Empty);
        }

        protected override void ProcessFiles(DirectoryInfo dir)
        {
            PumpFK2TxtReports(dir);

            SetReportFormat();

            FileInfo[] files = dir.GetFiles("*.html", SearchOption.AllDirectories);
            sourceDate = this.DataSource.Year * 10000 + this.DataSource.Month * 100;
            string sourcePath = GetShortSourcePathBySourceID(this.SourceID);
            filesCount = 0;

            HtmlHelper hh = new HtmlHelper();

            try
            {
                for (int i = 0; i < files.GetLength(0); i++)
                {
                    filesCount++;

                    try
                    {
                        SetProgress(totalFiles, filesCount,
                            string.Format("��������� ����� {0}\\{1}...", sourcePath, files[i].Name),
                            string.Format("���� {0} �� {1}", filesCount, totalFiles), true);

                        if (!files[i].Exists) continue;

                        WriteEventIntoDataPumpProtocol(
                            DataPumpEventKind.dpeStartFilePumping,
                            string.Format("����� ������� ����� {0}.", files[i].FullName));

                        Stopwatch sw = new Stopwatch();
                        sw.Start();
                        DataSet ds = hh.GetTablesFromHtml(files[i], true);
                        sw.Stop();
                        WriteToTrace(String.Format("������� �����: {0} ��", sw.ElapsedMilliseconds), TraceMessageKind.Information);
                        if (sourceDate < 20080300)
                            if (!((ds.Tables.Count == 2) || (ds.Tables.Count == 4)))
                            {
                                WriteEventIntoDataPumpProtocol(
                                     DataPumpEventKind.dpeWarning, string.Format(
                                         "������������ ���������� ������ � ������ {0}: {1}. ����� ��������.",
                                         files[i].Name, ds.Tables.Count));
                                continue;
                            }

                        // ��������� ���� ������ � ����� ���������
                        string[] reportHeader =
                            Convert.ToString(ds.Tables[0].Rows[0][0]).Split(new string[] { "\n" }, StringSplitOptions.None);
                        if (reportHeader.GetLength(0) > 1)
                        {
                            int reportDate = CommonRoutines.DecrementDate(CommonRoutines.ShortDateToNewDate(reportHeader[2].Trim()));
                            if (reportDate != sourceDate)
                            {
                                WriteEventIntoDataPumpProtocol(
                                    DataPumpEventKind.dpeWarning, string.Format(
                                        "���� {0} ������ {1} �� ������������� ���������� ���������. ����� ��������.",
                                        reportDate, files[i].Name));
                                continue;
                            }
                        }

                        sw.Reset();
                        sw.Start();
                        PumpHtmlFileData(ds, files[i].Name, reportHeader[0].Trim());
                        sw.Stop();
                        WriteToTrace(String.Format("������� �����: {0} ��", sw.ElapsedMilliseconds), TraceMessageKind.Information);

                        WriteEventIntoDataPumpProtocol(
                            DataPumpEventKind.dpeSuccessfullFinishFilePump,
                            string.Format("������� ����� {0} ������� ���������.", files[i].FullName));
                    }
                    catch (ThreadAbortException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        // FMQ00003791 �� ������� ��������������� ��� ������ � ����� ����� ��� ��������� �� ����������
                        this.DataSourceProcessingResult = DataSourceProcessingResult.ProcessedWithErrors;
                        WriteEventIntoDataPumpProtocol(
                            DataPumpEventKind.dpeFinishFilePumpWithError,
                            string.Format("������� ����� {0} ��������� � ��������.", files[i].FullName), ex);
                        continue;
                    }
                }
                UpdateData();

                SetClsHierarchy();
                UpdateData();
            }
            finally
            {
                hh.Dispose();
            }
        }

        protected override void DirectPumpData()
        {
            totalFiles = this.RootDir.GetFiles("*.html", SearchOption.AllDirectories).GetLength(0);
            FillClsHierarchy();
            PumpDataYMTemplate();
        }

        #endregion ���������� ������ �������

        #endregion ������� ������

        #region ��������� ������

        private void DoFkrRenaming(IClassifier cls, DataSet ds, IDbDataAdapter da, string codeFieldName)
        {
            // ���� �������������� �� �����-�� �������� ���� - �������
            if ((ds == null) || (ds.Tables.Count == 0) || (ds.Tables[0].Rows.Count == 0))
                return;

            // �������� �������
            Stopwatch sw = new Stopwatch();
            sw.Start();
            string clsSemantic = cls.FullCaption;
            string msgStr = String.Format("����������� �������������� {0} �� D_FKR_Analysis..", clsSemantic);
            WriteToTrace(msgStr, TraceMessageKind.Information);
            // ���� �� � ���������� ������ ����������?
            string query = String.Format(
                "Select ID from DataSources where (SupplierCode = '��') and (DataCode = 6) and (Year = {0})", 
                this.DataSource.Year);
            DataTable dt = (DataTable)this.DB.ExecQuery(query, QueryResultTypes.DataTable);
            // ���� ��� ��������� - ������ ��������������, ����������� �� ����������
            if ((dt == null) || (dt.Rows.Count == 0))
            {
                msgStr = String.Format(
                    "��� ��������� ������ ��� ������� (SupplierCode = '��', DataCode = 6, Year = {0}). " +
                    "����������� �������������� '{1}' ����������� �� �����.", this.DataSource.Year, clsSemantic);
                WriteToTrace(msgStr, TraceMessageKind.Warning);
                WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeWarning, msgStr);
                return;
            }
            // ���� ���������� ��������� - ������ �������������� � ����� ������
            if (dt.Rows.Count > 1)
            {
                msgStr = String.Format(
                    "������� {0} ���������� ������ ��� ������� (SupplierCode = '��', DataCode = 6, Year = {1}). " +
                    "��� ����������� �������������� '{2}' ����� ����������� ������.",
                    dt.Rows.Count, this.DataSource.Year, clsSemantic);
                WriteToTrace(msgStr, TraceMessageKind.Warning);
                WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeWarning, msgStr);
            }
            // ���������� ID ���������
            int renamingDataSourceId = Convert.ToInt32(dt.Rows[0][0]);
            // �������� �� D_FKR_Analysis ����������� �� ����� ���������
            query = String.Format("Select Code, Name from D_FKR_ANALYSIS where SourceId = {0}", renamingDataSourceId);
            dt = (DataTable)this.DB.ExecQuery(query, QueryResultTypes.DataTable);
            // ���� ����������� �� ����� ��������� �� ������� - ������ ��������������
            if ((dt == null) || (dt.Rows.Count <= 1))
            {
                msgStr = String.Format(
                    "������������� D_FKR_ANALYSIS �� �������� ������ �� ��������� (ID={0}). " +
                    "����������� �������������� '{1}' ����������� �� �����.",
                    renamingDataSourceId, clsSemantic);
                WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeWarning, msgStr);
                return;
            }
            // �������� �����������
            dt.PrimaryKey = new DataColumn[] { dt.Columns["Code"] };
            DataColumn codeColumn = ds.Tables[0].Columns[codeFieldName];
            DataColumn fkrNameColumn = ds.Tables[0].Columns["NAME"];
            DataColumn renamingNameColumn = dt.Columns["NAME"];

            foreach (DataRow fkrRow in ds.Tables[0].Rows)
            {
                // ��� ������ �������������� ����������� ������ ������ ��� ���=0
                if (fkrRow["Ekr"].ToString().TrimStart('0') != string.Empty)
                    continue;
                int code = Convert.ToInt32(fkrRow[codeColumn].ToString().PadLeft(1, '0'));
                if (code == 0)
                    continue;
                DataRow renamingRow = dt.Rows.Find(code);
                if (renamingRow != null)
                    fkrRow[fkrNameColumn] = renamingRow[renamingNameColumn];
            }
            UpdateDataSet(da, ds, cls);
            sw.Stop();
            msgStr = String.Format("����������� �������������� {0} �� D_FKR_Analysis ��������� ({1} ��)", 
                clsSemantic, sw.ElapsedMilliseconds);
            WriteToTrace(msgStr, TraceMessageKind.Information);
        }

        private void SetEKRHierarchy()
        {
            DataRow[] rows = dsEKR2005.Tables[0].Select("StringCode = 210 and code = 0");
            if (rows.GetLength(0) == 0)
                return;
            string parentID = rows[0]["ID"].ToString();
            rows = dsEKR2005.Tables[0].Select("code = 0 and StringCode >= 211 and StringCode <= 219");
            foreach (DataRow row in rows)
                row["PARENTID"] = parentID;
            UpdateDataSet(daEKR2005, dsEKR2005, clsEKR2005);
        }

        /// <summary>
        /// ������� ��������� ���� ������ �� ������ ���������
        /// </summary>
        /// <param name="sourceID">�� ���������</param>
        protected override void ProcessDataSource()
        {
            // �������� ��������� � ���� ������
            toSetHierarchy = false;
            SetClsHierarchy();
            UpdateData();
            // �������������� ��������� ��������
            SetEKRHierarchy();
            CommonSumCorrectionConfig fkmrSumCorrectionConfig = new CommonSumCorrectionConfig();
            fkmrSumCorrectionConfig.Sum1 = "ASSIGNED";
            fkmrSumCorrectionConfig.Sum1Report = "ASSIGNEDREPORT";
            fkmrSumCorrectionConfig.Sum2 = "FACT";
            fkmrSumCorrectionConfig.Sum2Report = "FACTREPORT";

            // ������������� ��� ����� ������������� �������������� �� 
            // ���������������� �������������� D_FKR_Analysis, �.�. � �������� ������ ������������ �����������
            int period = this.DataSource.Year * 100 + this.DataSource.Month;
            if ((period >= 200510) && (period < 200900))
                DoFkrRenaming(clsOutcomes, dsClsOutcomes, daClsOutcomes, "FKR");

            // ���������������
            TransferSourceSums(fctFKMRDefProf, fkmrSumCorrectionConfig);

            fkmrSumCorrectionConfig.Sum3 = "ExcSumP";
            fkmrSumCorrectionConfig.Sum3Report = "ExcSumPRep";
            fkmrSumCorrectionConfig.Sum4 = "ExcSumF";
            fkmrSumCorrectionConfig.Sum4Report = "ExcSumFRep";

            // ������
            AddParentRecords(fctFKMRIncomes, dsKD.Tables[0], "CodeStr", "RefKD",
                new string[] { "RefYearDayUNV", "RefBudgetLevels", "RefTerritory" }, fkmrSumCorrectionConfig);
            CorrectFactTableSums(fctFKMRIncomes, dsKD.Tables[0], clsKD, "REFKD", 
                fkmrSumCorrectionConfig, BlockProcessModifier.MRStandard, null, "REFTERRITORY", "REFBUDGETLEVELS");

            // ��������� ��������������
            CorrectFactTableSums(fctFKMRSrcFin, dsKIF.Tables[0], clsKIF, "REFKIF", 
                fkmrSumCorrectionConfig, BlockProcessModifier.MRStandard, null, "REFTERRITORY", "REFBUDGETLEVELS", true);

            // �������
            if (this.DataSource.Year * 100 + this.DataSource.Month >= 200202)
            {
                CorrectFactTableSums(fctFKMROutcomes, dsClsOutcomes.Tables[0], clsOutcomes, "RefR",
                    fkmrSumCorrectionConfig, BlockProcessModifier.MRStandard, new string[] { },
                    "REFTERRITORY", "REFBUDGETLEVELS", true);
            }
            else
            {
                CorrectFactTableSums(fctFKMROutcomes, dsMarksOutcomes.Tables[0],
                    clsMarksOutcomes, "REFMARKS", fkmrSumCorrectionConfig, BlockProcessModifier.MRStandard,
                    new string[] { "REFEKR2005" }, "REFTERRITORY", "REFBUDGETLEVELS", true);
            }

          //  fkmrSumCorrectionConfig.AssignedField = string.Empty;
          //  fkmrSumCorrectionConfig.AssignedReportField = string.Empty;
            // ���������
           // CorrectFactTableSums(fctFKMRAdd, dsMarks.Tables[0], clsMarks, "REFMARKS",
            //    fkmrSumCorrectionConfig, BlockProcessModifier.MRStandard, null, "REFTERRITORY", "REFBUDGETLEVELS", true);

            CommonLiteSumCorrectionConfig scc = new CommonLiteSumCorrectionConfig();
            scc.sumFieldForCorrect = new string[] { "AssignedReport", "FactReport" };
            scc.fields4CorrectedSums = new string[] { "Assigned", "Fact" };
            CorrectFactTableSums(fctIFSRefsFact, dsIFSRefsCls.Tables[0], clsIFSRefsCls, "RefMarks",
                scc, BlockProcessModifier.MRStandard, new string[] { "RefYearDayUNV", "RefTerritory", "RefBdgtLevels" }, string.Empty, string.Empty, true);
            CorrectFactTableSums(fctOFSRefsFact, dsOFSRefsCls.Tables[0], clsOFSRefsCls, "RefMarks",
                scc, BlockProcessModifier.MRStandard, new string[] { "RefYearDayUNV", "RefTerritory", "RefBdgtLevels" }, string.Empty, string.Empty, true);
            CorrectFactTableSums(fctArrearsRefsFact, dsArrearsRefsCls.Tables[0], clsArrearsRefsCls, "RefMarks",
                scc, BlockProcessModifier.MRStandard, new string[] { "RefYearDayUNV", "RefTerritory", "RefBdgtLevels" }, string.Empty, string.Empty, true);
            CorrectFactTableSums(fctOutcomesRefsFact, dsOutcomesRefsCls.Tables[0], clsOutcomesRefsCls, "RefMarks",
                scc, BlockProcessModifier.MRStandard, new string[] { "RefYearDayUNV", "RefTerritory", "RefBdgtLevels" }, string.Empty, string.Empty, true);

            scc.sumFieldForCorrect = new string[] { "FactReport" };
            scc.fields4CorrectedSums = new string[] { "Fact" };
            CorrectFactTableSums(fctExcessRefsFact, dsExcessRefsCls.Tables[0], clsExcessRefsCls, "RefMarks",
                scc, BlockProcessModifier.MRStandard, new string[] { "RefYearDayUNV", "RefTerritory", "RefBdgtLevels" }, string.Empty, string.Empty, true);
        }

        protected override void DirectProcessData()
        {
            int year = -1;
            int month = -1;
            GetPumpParams(ref year, ref month);
            ProcessDataSourcesTemplate(year, month, "��������� ���� ������ �� ������ ���������");
        }

        #endregion ��������� ������

        #region �������� ����������������� ����

        protected override void QueryDataForCheck()
        {
            QueryData();
            string constraint = string.Format("SourceID = {0}", this.SourceID);
            InitDataSet(ref daFKMRIncomes, ref dsFKMRIncomes, fctFKMRIncomes, false, constraint, string.Empty);
        }

        private List<int> GetClsParents(DataTable clsTable)
        {
            List<int> list = new List<int>();
            foreach (DataRow row in clsTable.Rows)
            {
                if (row.IsNull("ParentId"))
                    continue;
                int parentId = Convert.ToInt32(row["ParentId"]);
                if (!list.Contains(parentId))
                    list.Add(parentId);
            }
            return list;
        }

        private string GetFieldCaption(string fieldName)
        {
            switch (fieldName.ToUpper())
            {
                case "FACT":
                    return "����";
                case "ASSIGNED":
                    return "���������";
                default:
                    return string.Empty;
            }
        }

        private void CheckParentSums(DataRow factRow, string[] sumFieldNames, DataRow clsRow)
        {
            foreach (string sumFieldName in sumFieldNames)
            {
                decimal sum = 0;
                if (factRow[sumFieldName] != DBNull.Value)
                    sum = Convert.ToDecimal(factRow[sumFieldName]);
                if (sum != 0)
                {
                    string message = string.Format("����������� ��������������� ����� ����� (Id:{0}; ����: {1}; �����: {2})." +
                        "��������, � ����������� ���� {3} (Id: {4}) ������� �������������� ������ '��.������.2005' ������� ����������� ��������.",
                        factRow["Id"], GetFieldCaption(sumFieldName), sum, clsRow["CodeStr"], clsRow["Id"]);
                    CheckDataProtocol.WriteEventIntoReviseDataProtocol(ReviseDataEventKind.pdeWarning, message, this.PumpID, this.SourceID);
                }
            }
        }

        private void CheckCorrectedSums(DataTable factTable, string refClsFieldName, DataTable clsTable, string[] sumFieldNames)
        {
            Dictionary<int, DataRow> clsCache = null;
            FillRowsCache(ref clsCache, clsTable, "Id");
            List<int> clsParentsList = GetClsParents(clsTable);
            List<int> incorrectClsList = new List<int>();
            try
            {
                foreach (DataRow row in factTable.Rows)
                {
                    int clsId = Convert.ToInt32(row[refClsFieldName]);
                    if (!clsParentsList.Contains(clsId))
                        continue;
                    CheckParentSums(row, sumFieldNames, clsCache[clsId]);
                }
            }
            finally
            {
                clsParentsList.Clear();
                incorrectClsList.Clear();
            }
        }

        protected override void CheckDataSource()
        {
            CheckCorrectedSums(dsFKMRIncomes.Tables[0], "REFKD", dsKD.Tables[0], new string[] { "Fact", "Assigned" });
        }

        protected override void DirectCheckData()
        {
            int year = -1;
            int month = -1;
            GetPumpParams(ref year, ref month);
            CheckDataSourcesTemplate(year, month, "�������� ����������������� ����.");
        }

        #endregion �������� ����������������� ����

    }
}