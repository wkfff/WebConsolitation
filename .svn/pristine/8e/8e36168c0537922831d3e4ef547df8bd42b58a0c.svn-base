using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;

using Krista.FM.Common;
using Krista.FM.Server.Common;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.ServerLibrary;
using Krista.FM.Common.Xml;

namespace Krista.FM.Server.DataPumps.FO53Pump
{
    public class FO53PumpModule : CorrectedPumpModuleBase
    {

        #region Поля

        #region Классификаторы

        // Организации.ФО_Мониторинг ФХД (d_Org_FOFHD)
        private IDbDataAdapter daOrg;
        private DataSet dsOrg;
        private IClassifier clsOrg;
        private Dictionary<string, int> cacheOrg = null;
        // ЛицСчета.ФО_Мониторинг ФХД (d_FacialAcc_FOFHD)
        private IDbDataAdapter daFacial;
        private DataSet dsFacial;
        private IClassifier clsFacial;
        private Dictionary<string, int> cacheFacial = null;
        // РасчСчета.ФО_Мониторинг ФХД (d_KVSR_KPI)
        private IDbDataAdapter daAccounts;
        private DataSet dsAccounts;
        private IClassifier clsAccounts;
        private Dictionary<string, int> cacheAccounts = null;
        // КОСГУ.ФО_Мониторинг ФХД (d_KOSGY_FOFHD)
        private IDbDataAdapter daKosgu;
        private DataSet dsKosgu;
        private IClassifier clsKosgu;
        private Dictionary<string, int> cacheKosgu = null;
        // КОСГУ.Эталонный (d_EKR_Etalon)
        private IDbDataAdapter daKosguEtalon;
        private DataSet dsKosguEtalon;
        private IClassifier clsKosguEtalon;
        private Dictionary<string, string> cacheKosguEtalon = null;

        #endregion Классификаторы

        #region Факты

        // Факт.ФО_Мониторинг ФХД_Движение средств (f_F_FOFHDMovFunds)
        private IDbDataAdapter daFOFHDMovFunds;
        private DataSet dsFOFHDMovFunds;
        private IFactTable fctFOFHDMovFunds;
        // Факт.ФО_Мониторинг ФХД_Отдельные показатели (f_F_FOFHDMarks)
        private IDbDataAdapter daFOFHDMarks;
        private DataSet dsFOFHDMarks;
        private IFactTable fctFOFHDMarks;
        // Факт.ФО_Мониторинг ФХД_Кредиторская задолженность (f_F_FOFHDPayable)
        private IDbDataAdapter daFOFHDPayable;
        private DataSet dsFOFHDPayable;
        private IFactTable fctFOFHDPayable;
        // Факт.ФО_Мониторинг ФХД_Дебиторская задолженность (f_F_FOFHDReceivable)
        private IDbDataAdapter daFOFHDReceivable;
        private DataSet dsFOFHDReceivable;
        private IFactTable fctFOFHDReceivable;
        // Факт.ФО_Мониторинг ФХД_Должности (f_F_FOFHDPost)
        private IDbDataAdapter daFOFHDPost;
        private DataSet dsFOFHDPost;
        private IFactTable fctFOFHDPost;
        // Факт.ФО_Мониторинг ФХД_Коммунальные услуги (f_F_FOFHDUtil)
        private IDbDataAdapter daFOFHDUtil;
        private DataSet dsFOFHDUtil;
        private IFactTable fctFOFHDUtil;
        // Факт.ФО_Мониторинг ФХД_Материальные затраты (f_F_FOFHDMater)
        private IDbDataAdapter daFOFHDMater;
        private DataSet dsFOFHDMater;
        private IFactTable fctFOFHDMater;
        // Факт.ФО_Мониторинг ФХД_Налоги (f_F_FOFHDTax)
        private IDbDataAdapter daFOFHDTax;
        private DataSet dsFOFHDTax;
        private IFactTable fctFOFHDTax;
        // Факт.ФО_Мониторинг ФХД_Стоимость ОС (f_F_FOFHDOS)
        private IDbDataAdapter daFOFHDOS;
        private DataSet dsFOFHDOS;
        private IFactTable fctFOFHDOS;
        // Факт.ФО_Мониторинг ФХД_Состояние имущества (f_F_FOFHDOSAge)
        private IDbDataAdapter daFOFHDOSAge;
        private DataSet dsFOFHDOSAge;
        private IFactTable fctFOFHDOSAge;
        // Факт.ФО_Мониторинг ФХД_Финансовый результат (f_F_FOFHDResult)
        private IDbDataAdapter daFOFHDResult;
        private DataSet dsFOFHDResult;
        private IFactTable fctFOFHDResult;

        #endregion Факты

        int curDate = 0;
        int refOrg = -1;
        // признак - нужно ли удалять данные факта
        bool deleteMark = true;

        #endregion Поля

        #region Закачка данных

        #region Работа с базой и кэшами

        private void FillCaches()
        {
            FillRowsCache(ref cacheOrg, dsOrg.Tables[0], new string[] { "Inn", "Name" }, "|", "Id");
            FillRowsCache(ref cacheFacial, dsFacial.Tables[0], "Code", "Id");
            FillRowsCache(ref cacheAccounts, dsAccounts.Tables[0], "CodeStr", "Id");
            FillRowsCache(ref cacheKosgu, dsKosgu.Tables[0], "Code", "Id");
            FillRowsCache(ref cacheKosguEtalon, dsKosguEtalon.Tables[0], "Code", "Name");
        }

        // инициализация эталонного классификатора
        // если за текущий месяц данных нет - берем предыдущий. за январь данные есть палюбасу
        private void InitStandartCls(ref IDbDataAdapter da, ref DataSet ds, IClassifier cls)
        {
            for (int curMonth = 12; curMonth >= 1; curMonth--)
            {
                string query = string.Format("select id from DataSources where DELETED = 0 and SUPPLIERCODE = 'ФО'" +
                                             " and DATACODE = 22 and year = {0} and month = {1}",
                    this.DataSource.Year, curMonth);
                DataTable sourceId = (DataTable)this.DB.ExecQuery(query, QueryResultTypes.DataTable, new IDbDataParameter[] { });
                if ((sourceId == null) || (sourceId.Rows.Count == 0))
                    continue;
                foreach (DataRow row in sourceId.Rows)
                {
                    string constr = string.Format("SOURCEID = {0}", row["Id"]);
                    InitDataSet(ref da, ref ds, cls, true, constr, string.Empty);
                    if (ds.Tables[0].Rows.Count > 3)
                        break;
                }
                if (ds.Tables[0].Rows.Count > 3)
                    break;
            }
            if (ds == null)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning,
                    string.Format("Не заполнен эталонный классификатор '{0}'", cls.FullCaption));
                InitDataSet(ref da, ref ds, cls, true, "1=0", string.Empty);
            }
            else
            {
                if (ds.Tables[0].Rows.Count <= 3)
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning,
                        string.Format("Не заполнен эталонный классификатор '{0}'", cls.FullCaption));
            }
        }

        protected override void QueryData()
        {
            InitClsDataSet(ref daOrg, ref dsOrg, clsOrg);
            InitClsDataSet(ref daFacial, ref dsFacial, clsFacial);
            InitClsDataSet(ref daAccounts, ref dsAccounts, clsAccounts);
            InitClsDataSet(ref daKosgu, ref dsKosgu, clsKosgu);

            InitStandartCls(ref daKosguEtalon, ref dsKosguEtalon, clsKosguEtalon);

            InitFactDataSet(ref daFOFHDMovFunds, ref dsFOFHDMovFunds, fctFOFHDMovFunds);
            InitFactDataSet(ref daFOFHDMarks, ref dsFOFHDMarks, fctFOFHDMarks);
            InitFactDataSet(ref daFOFHDPayable, ref dsFOFHDPayable, fctFOFHDPayable);
            InitFactDataSet(ref daFOFHDReceivable, ref dsFOFHDReceivable, fctFOFHDReceivable);
            InitFactDataSet(ref daFOFHDPost, ref dsFOFHDPost, fctFOFHDPost);
            InitFactDataSet(ref daFOFHDUtil, ref dsFOFHDUtil, fctFOFHDUtil);
            InitFactDataSet(ref daFOFHDMater, ref dsFOFHDMater, fctFOFHDMater);
            InitFactDataSet(ref daFOFHDTax, ref dsFOFHDTax, fctFOFHDTax);
            InitFactDataSet(ref daFOFHDOS, ref dsFOFHDOS, fctFOFHDOS);
            InitFactDataSet(ref daFOFHDOSAge, ref dsFOFHDOSAge, fctFOFHDOSAge);
            InitFactDataSet(ref daFOFHDResult, ref dsFOFHDResult, fctFOFHDResult);

            FillCaches();
        }

        protected override void UpdateData()
        {
            UpdateDataSet(daOrg, dsOrg, clsOrg);
            UpdateDataSet(daFacial, dsFacial, clsFacial);
            UpdateDataSet(daAccounts, dsAccounts, clsAccounts);
            UpdateDataSet(daKosgu, dsKosgu, clsKosgu);

            UpdateDataSet(daFOFHDMovFunds, dsFOFHDMovFunds, fctFOFHDMovFunds);
            UpdateDataSet(daFOFHDMarks, dsFOFHDMarks, fctFOFHDMarks);
            UpdateDataSet(daFOFHDPayable, dsFOFHDPayable, fctFOFHDPayable);
            UpdateDataSet(daFOFHDReceivable, dsFOFHDReceivable, fctFOFHDReceivable);
            UpdateDataSet(daFOFHDPost, dsFOFHDPost, fctFOFHDPost);
            UpdateDataSet(daFOFHDUtil, dsFOFHDUtil, fctFOFHDUtil);
            UpdateDataSet(daFOFHDMater, dsFOFHDMater, fctFOFHDMater);
            UpdateDataSet(daFOFHDTax, dsFOFHDTax, fctFOFHDTax);
            UpdateDataSet(daFOFHDOS, dsFOFHDOS, fctFOFHDOS);
            UpdateDataSet(daFOFHDOSAge, dsFOFHDOSAge, fctFOFHDOSAge);
            UpdateDataSet(daFOFHDResult, dsFOFHDResult, fctFOFHDResult);
        }

        private const string ORG_GUID = "81a8b52b-224c-4c27-ae0a-28812874d3c9";
        private const string FACIAL_GUID = "add57a5d-a8d0-43ec-b179-bc30e1956699";
        private const string ACCOUNTS_GUID = "63cbf4c9-b9bb-4523-82f6-8d7f3473a3c4";
        private const string KOSGU_GUID = "9a5c57dc-68b4-4b94-adbd-768638ceeaaf";
        private const string KOSGU_ETALON_GUID = "03c3a566-82fb-48e9-b3d9-3d71f42a947f";

        private const string FOFHDMovFunds_GUID = "83b82c67-0070-4cba-bc12-3a65e20894a8";
        private const string FOFHDMarks_GUID = "ce33df92-217f-441b-80ab-fbab6ff6e5a3";
        private const string FOFHDPayable_GUID = "f667fb2c-aeda-40bd-b5fb-dba98705f96f";
        private const string FOFHDReceivable_GUID = "1ce5ad12-1678-4e75-b787-e3a1d42afb57";
        private const string FOFHDPost_GUID = "dd1f3bc4-9c5d-4549-91a9-b48c81231011";
        private const string FOFHDUtil_GUID = "ac5ad6af-b732-4c3a-826a-ccabfe8e8648";
        private const string FOFHDMater_GUID = "6f6e9e05-74ae-40e2-8f69-3d539ecd5461";
        private const string FOFHDTax_GUID = "e4dd5fa3-1e1f-484a-8e88-c8351e1914f8";
        private const string FOFHDOS_GUID = "45820a3c-e26b-45da-b152-99dca33176c7";
        private const string FOFHDOSAge_GUID = "e8547841-39c5-4357-b40a-cf44e1b510ac";
        private const string FOFHDResult_GUID = "0e65b8ed-93d8-492d-b537-17ab49313234";
        protected override void InitDBObjects()
        {
            clsOrg = this.Scheme.Classifiers[ORG_GUID];
            clsFacial = this.Scheme.Classifiers[FACIAL_GUID];
            clsAccounts = this.Scheme.Classifiers[ACCOUNTS_GUID];
            clsKosgu = this.Scheme.Classifiers[KOSGU_GUID];
            this.UsedClassifiers = new IClassifier[] { clsOrg, clsFacial, clsAccounts, clsKosgu };

            clsKosguEtalon = this.Scheme.Classifiers[KOSGU_ETALON_GUID];

            fctFOFHDMovFunds = this.Scheme.FactTables[FOFHDMovFunds_GUID];
            fctFOFHDMarks = this.Scheme.FactTables[FOFHDMarks_GUID];
            fctFOFHDPayable = this.Scheme.FactTables[FOFHDPayable_GUID];
            fctFOFHDReceivable = this.Scheme.FactTables[FOFHDReceivable_GUID];
            fctFOFHDPost = this.Scheme.FactTables[FOFHDPost_GUID];
            fctFOFHDUtil = this.Scheme.FactTables[FOFHDUtil_GUID];
            fctFOFHDMater = this.Scheme.FactTables[FOFHDMater_GUID];
            fctFOFHDTax = this.Scheme.FactTables[FOFHDTax_GUID];
            fctFOFHDOS = this.Scheme.FactTables[FOFHDOS_GUID];
            fctFOFHDOSAge = this.Scheme.FactTables[FOFHDOSAge_GUID];
            fctFOFHDResult = this.Scheme.FactTables[FOFHDResult_GUID];

            this.UsedFacts = new IFactTable[] { fctFOFHDMovFunds, fctFOFHDMarks, fctFOFHDPayable, fctFOFHDReceivable, fctFOFHDPost, 
                fctFOFHDUtil, fctFOFHDMater, fctFOFHDTax, fctFOFHDOS, fctFOFHDOSAge, fctFOFHDResult };
        }

        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsFOFHDMovFunds);
            ClearDataSet(ref dsFOFHDMarks);
            ClearDataSet(ref dsFOFHDPayable);
            ClearDataSet(ref dsFOFHDReceivable);
            ClearDataSet(ref dsFOFHDPost);
            ClearDataSet(ref dsFOFHDUtil);
            ClearDataSet(ref dsFOFHDMater);
            ClearDataSet(ref dsFOFHDTax);
            ClearDataSet(ref dsFOFHDOS);
            ClearDataSet(ref dsFOFHDOSAge);
            ClearDataSet(ref dsFOFHDResult);

            ClearDataSet(ref dsOrg);
            ClearDataSet(ref dsFacial);
            ClearDataSet(ref dsAccounts);
            ClearDataSet(ref dsKosgu);
            ClearDataSet(ref dsKosguEtalon);
        }

        #endregion Работа с базой и кэшами

        #region работа с xml

        private void GetDate(XmlNode dateNode)
        {
            curDate = XmlHelper.GetIntAttrValue(dateNode, "date", 0);
//            if (!this.DeleteEarlierData)
 //               DirectDeleteFactData(this.UsedFacts, -1, this.SourceID, string.Format(" RefYearDayUNV = {0} ", curDate));
        }

        private void PumpOrg(XmlNode dataNode)
        {
            string inn = XmlHelper.GetStringAttrValue(dataNode, "INN", string.Empty);
            string name = XmlHelper.GetStringAttrValue(dataNode, "full_name", string.Empty);
            string okved = XmlHelper.GetStringAttrValue(dataNode, "okved", string.Empty);
            string okato = XmlHelper.GetStringAttrValue(dataNode, "okato", string.Empty);
            string grbs = XmlHelper.GetStringAttrValue(dataNode, "сode_GRBC", string.Empty);
            string grbsName = XmlHelper.GetStringAttrValue(dataNode, "full_name_GRBC", string.Empty);
            string refOrgType = XmlHelper.GetStringAttrValue(dataNode, "id_org_type", string.Empty);
            string key = string.Format("{0}|{1}", inn, name);
            refOrg = PumpCachedRow(cacheOrg, dsOrg.Tables[0], clsOrg, key, new object[] { "inn", inn, "name", name, 
                "okved", okved, "okato", okato, "GRBC", grbs, "GRBCName", grbsName, "refOrgType", refOrgType });
        }

        #region FOFHDMovFunds

        private void PumpFOFHDMovFundsSum(XmlNodeList dataNodes, string sumName, bool isFixKosgu)
        {
            foreach (XmlNode node in dataNodes)
            {
                if (deleteMark)
                {
                    DirectDeleteFactData(new IFactTable[] { fctFOFHDMovFunds }, -1, this.SourceID,
                        string.Format(" RefYearDayUNV = {0} and RefOrg = {1} ", curDate, refOrg));
                    deleteMark = false;
                }
                decimal sum = Convert.ToDecimal(XmlHelper.GetStringAttrValue(node, "summa", "0"));
                if (sum == 0)
                    continue;
                string facial = XmlHelper.GetStringAttrValue(node, "facial_account", "0");
                int refFacial = PumpCachedRow(cacheFacial, dsFacial.Tables[0], clsFacial, facial, 
                    new object[] { "Code", facial } );
                string account = XmlHelper.GetStringAttrValue(node, "calculation_account", "0");
                int refAccount = PumpCachedRow(cacheAccounts, dsAccounts.Tables[0], clsAccounts, account,
                    new object[] { "CodeStr", account });
                string kosgu = XmlHelper.GetStringAttrValue(node, "kosgy", "0");
                string kosguCls = XmlHelper.GetStringAttrValue(node, "kosgy", "0");
                if (isFixKosgu)
                    kosguCls = "0";
                else
                    kosgu = "0";
                string kosguName = constDefaultClsName;
                if (cacheKosguEtalon.ContainsKey(kosguCls))
                    kosguName = cacheKosguEtalon[kosguCls];
                int refKosgu = PumpCachedRow(cacheKosgu, dsKosgu.Tables[0], clsKosgu, kosguCls,
                    new object[] { "Code", kosguCls, "Name", kosguName });
                string refCashFlw = XmlHelper.GetStringAttrValue(node, "account", "0");
                string refTypePay = XmlHelper.GetStringAttrValue(node, "payment", "0");
                string refSource = XmlHelper.GetStringAttrValue(node, "vd", "0");

                object[] mapping = new object[] { sumName, sum, "RefOrg", refOrg, "RefFacial", refFacial, "RefAccount", refAccount, "RefKOSGU", kosgu, 
                    "RefCashFlw", refCashFlw, "RefTypePay", refTypePay, "RefSource", refSource, "RefdKOSGU", refKosgu, "RefYearDayUNV", curDate };
                PumpRow(dsFOFHDMovFunds.Tables[0], mapping);
            }
        }

        private void PumpFOFHDMovFunds(XmlNode dataNode)
        {
            deleteMark = true;
            PumpFOFHDMovFundsSum(dataNode.SelectNodes("мovement_funds/entrance_funds/entrance_fund"), "EntranceFunds", true);
            PumpFOFHDMovFundsSum(dataNode.SelectNodes("мovement_funds/сash_expense/сash"), "CashExpense", false);
            PumpFOFHDMovFundsSum(dataNode.SelectNodes("мovement_funds/remains_expense/remains"), "RemainsExpense", true);
            PumpFOFHDMovFundsSum(dataNode.SelectNodes("plan_activities_org/plan_org/plan[@type_plan=1]"), "PlanIncome", true);
            PumpFOFHDMovFundsSum(dataNode.SelectNodes("plan_activities_org/budget_org/budget[@type_plan=1]"), "PlanIncome", true);
            PumpFOFHDMovFundsSum(dataNode.SelectNodes("plan_activities_org/plan_org/plan[@type_plan=2]"), "PlanExpense", true);
            PumpFOFHDMovFundsSum(dataNode.SelectNodes("plan_activities_org/budget_org/budget[@type_plan=2]"), "PlanExpense", true);
            UpdateData();
        }

        #endregion FOFHDMovFunds

        #region FOFHDMarks

        private void PumpFOFHDMarksSum(XmlNodeList dataNodes, string sumName, string sumAttrName)
        {
            foreach (XmlNode node in dataNodes)
            {
                if (deleteMark)
                {
                    DirectDeleteFactData(new IFactTable[] { fctFOFHDMarks }, -1, this.SourceID,
                        string.Format(" RefYearDayUNV = {0} and RefOrg = {1} ", curDate, refOrg));
                    deleteMark = false;
                }
                decimal sum = Convert.ToDecimal(XmlHelper.GetStringAttrValue(node, sumAttrName, "0"));
                if (sum == 0)
                    continue;
                object[] mapping = new object[] { sumName, sum, "RefOrg", refOrg, "RefYearDayUNV", curDate };
                PumpRow(dsFOFHDMarks.Tables[0], mapping);
            }
        }

        private void PumpFOFHDMarks(XmlNode dataNode)
        {
            deleteMark = true;
            PumpFOFHDMarksSum(dataNode.SelectNodes("expense/average_payment"), "Wages", "summa");
            PumpFOFHDMarksSum(dataNode.SelectNodes("expense/average_payment"), "NumberWork", "number");
            PumpFOFHDMarksSum(dataNode.SelectNodes("financial_indicators/debts_delayed/debts"), "Balance", "balance");
            PumpFOFHDMarksSum(dataNode.SelectNodes("financial_indicators/debts_delayed/debts"), "DebtsDelayed", "summa");
            PumpFOFHDMarksSum(dataNode.SelectNodes("financial_indicators/debts_delayed/debts"), "SummaDelay", "summa_delay");
            PumpFOFHDMarksSum(dataNode.SelectNodes("financial_indicators/debts_delayed/debts"), "SummaDelayFounder", "summa_delay_founder");
            PumpFOFHDMarksSum(dataNode.SelectNodes("financial_indicators/debts_delayed/debts"), "DebtsPayment", "debts_payment");
            PumpFOFHDMarksSum(dataNode.SelectNodes("financial_indicators/debts_delayed/debts"), "DebtsTax", "debts_tax");
            PumpFOFHDMarksSum(dataNode.SelectNodes("financial_indicators/debts_delayed/debts"), "DebtsBalance", "debts_balance");
            UpdateData();
        }

        #endregion FOFHDMarks

        #region FOFHDPayable

        private void PumpFOFHDPayable(XmlNodeList dataNodes)
        {
            if (dataNodes.Count != 0)
                DirectDeleteFactData(new IFactTable[] { fctFOFHDPayable }, -1, this.SourceID,
                    string.Format(" RefYearDayUNV = {0} and RefOrg = {1} ", curDate, refOrg));
            
            foreach (XmlNode node in dataNodes)
            {
                decimal arrears = Convert.ToDecimal(XmlHelper.GetStringAttrValue(node, "summa", "0"));
                decimal arrearsPayment = Convert.ToDecimal(XmlHelper.GetStringAttrValue(node, "summa_payment", "0"));
                if ((arrears == 0) && (arrearsPayment == 0))
                    continue;
                string kosgu = XmlHelper.GetStringAttrValue(node, "kosgy", "0");
                string kosguName = constDefaultClsName;
                if (cacheKosguEtalon.ContainsKey(kosgu))
                    kosguName = cacheKosguEtalon[kosgu];
                int refKosgu = PumpCachedRow(cacheKosgu, dsKosgu.Tables[0], clsKosgu, kosgu,
                    new object[] { "Code", kosgu, "Name", kosguName });
                string refCashFlw = XmlHelper.GetStringAttrValue(node, "account", "0");
                string refTypePay = XmlHelper.GetStringAttrValue(node, "payment", "0");
                string refArrears = XmlHelper.GetStringAttrValue(node, "сode_payable", "0");
                if (refArrears == "1")
                    refArrears = "2";
                else if (refArrears == "2")
                    refArrears = "3";
                string refSource = XmlHelper.GetStringAttrValue(node, "vd", "0");

                object[] mapping = new object[] { "arrears", arrears, "RefOrg", refOrg, "arrearsPayment", arrearsPayment, "RefKOSGY", refKosgu, 
                    "refTypePay", refTypePay, "refArrears", refArrears, "RefSource", refSource, "RefYearDayUNV", curDate };
                PumpRow(dsFOFHDPayable.Tables[0], mapping);
            }
            UpdateData();
        }

        #endregion FOFHDPayable

        #region FOFHDReceivable

        private void PumpFOFHDReceivable(XmlNodeList dataNodes)
        {
            if (dataNodes.Count != 0)
                DirectDeleteFactData(new IFactTable[] { fctFOFHDReceivable }, -1, this.SourceID,
                    string.Format(" RefYearDayUNV = {0} and RefOrg = {1} ", curDate, refOrg));

            foreach (XmlNode node in dataNodes)
            {
                decimal sum = Convert.ToDecimal(XmlHelper.GetStringAttrValue(node, "summa", "0"));
                if (sum == 0)
                    continue;
                string refArrears = XmlHelper.GetStringAttrValue(node, "type_dz", "0");
                if (refArrears == "1")
                    refArrears = "4";
                else if (refArrears == "2")
                    refArrears = "5";
                string refSource = XmlHelper.GetStringAttrValue(node, "vd", "0");

                object[] mapping = new object[] { "Summa", sum, "RefOrg", refOrg, "RefArrears", refArrears, 
                    "RefSource", refSource, "RefYearDayUNV", curDate };
                PumpRow(dsFOFHDReceivable.Tables[0], mapping);
            }
            UpdateData();
        }

        #endregion FOFHDReceivable

        #region FOFHDPost

        private void PumpFOFHDPost(XmlNodeList dataNodes)
        {
            if (dataNodes.Count != 0)
                DirectDeleteFactData(new IFactTable[] { fctFOFHDPost }, -1, this.SourceID,
                    string.Format(" RefYearDayUNV = {0} and RefOrg = {1} ", curDate, refOrg));

            foreach (XmlNode node in dataNodes)
            {
                decimal summaCategory = Convert.ToDecimal(XmlHelper.GetStringAttrValue(node, "summa_category", "0"));
                decimal countCategory = Convert.ToDecimal(XmlHelper.GetStringAttrValue(node, "count", "0"));
                if ((summaCategory == 0) && (countCategory == 0))
                    continue;
                string refCategory = XmlHelper.GetStringAttrValue(node, "category", "0");

                object[] mapping = new object[] { "summaCategory", summaCategory, "countCategory", countCategory, 
                    "RefOrg", refOrg, "refCategory", refCategory, "RefYearDayUNV", curDate };
                PumpRow(dsFOFHDPost.Tables[0], mapping);
            }
            UpdateData();
        }

        #endregion FOFHDPost

        #region FOFHDUtil

        private void PumpFOFHDUtil(XmlNodeList dataNodes)
        {
            if (dataNodes.Count != 0)
                DirectDeleteFactData(new IFactTable[] { fctFOFHDUtil }, -1, this.SourceID,
                    string.Format(" RefYearDayUNV = {0} and RefOrg = {1} ", curDate, refOrg));

            foreach (XmlNode node in dataNodes)
            {
                decimal summa = Convert.ToDecimal(XmlHelper.GetStringAttrValue(node, "summa", "0"));
                decimal countService = Convert.ToDecimal(XmlHelper.GetStringAttrValue(node, "count", "0"));
                decimal price = Convert.ToDecimal(XmlHelper.GetStringAttrValue(node, "price", "0"));
                if ((summa == 0) && (countService == 0) && (price == 0))
                    continue;
                string refService = XmlHelper.GetStringAttrValue(node, "code_service", "0");
                string refSource = XmlHelper.GetStringAttrValue(node, "vd", "0");

                object[] mapping = new object[] { "summa", summa, "countService", countService, "price", price, 
                    "RefOrg", refOrg, "refService", refService, "refSource", refSource, "RefYearDayUNV", curDate };
                PumpRow(dsFOFHDUtil.Tables[0], mapping);
            }
            UpdateData();
        }

        #endregion FOFHDUtil

        #region FOFHDMater

        private void PumpFOFHDMater(XmlNodeList dataNodes)
        {
            if (dataNodes.Count != 0)
                DirectDeleteFactData(new IFactTable[] { fctFOFHDMater }, -1, this.SourceID,
                    string.Format(" RefYearDayUNV = {0} and RefOrg = {1} ", curDate, refOrg));

            foreach (XmlNode node in dataNodes)
            {
                decimal summa = Convert.ToDecimal(XmlHelper.GetStringAttrValue(node, "summa", "0"));
                if (summa == 0)
                    continue;
                string refExpens = XmlHelper.GetStringAttrValue(node, "type_expenses", "0");
                string refSource = XmlHelper.GetStringAttrValue(node, "vd", "0");

                object[] mapping = new object[] { "summa", summa, "RefOrg", refOrg, 
                    "refExpens", refExpens, "refSource", refSource, "RefYearDayUNV", curDate };
                PumpRow(dsFOFHDMater.Tables[0], mapping);
            }
            UpdateData();
        }

        #endregion FOFHDMater

        #region FOFHDTax

        private void PumpFOFHDTax(XmlNodeList dataNodes)
        {
            if (dataNodes.Count != 0)
                DirectDeleteFactData(new IFactTable[] { fctFOFHDTax }, -1, this.SourceID,
                    string.Format(" RefYearDayUNV = {0} and RefOrg = {1} ", curDate, refOrg));

            foreach (XmlNode node in dataNodes)
            {
                decimal summa = Convert.ToDecimal(XmlHelper.GetStringAttrValue(node, "summa", "0"));
                if (summa == 0)
                    continue;
                string refTypePay = XmlHelper.GetStringAttrValue(node, "payments", "0");
                string refSource = XmlHelper.GetStringAttrValue(node, "vd", "0");

                object[] mapping = new object[] { "summa", summa, "RefOrg", refOrg, 
                    "refTypePay", refTypePay, "refSource", refSource, "RefYearDayUNV", curDate };
                PumpRow(dsFOFHDTax.Tables[0], mapping);
            }
            UpdateData();
        }

        #endregion FOFHDTax

        #region FOFHDOS

        private void PumpFOFHDOS(XmlNodeList dataNodes)
        {
            if (dataNodes.Count != 0)
                DirectDeleteFactData(new IFactTable[] { fctFOFHDOS }, -1, this.SourceID,
                    string.Format(" RefYearDayUNV = {0} and RefOrg = {1} ", curDate, refOrg));

            foreach (XmlNode node in dataNodes)
            {
                decimal summaBS = Convert.ToDecimal(XmlHelper.GetStringAttrValue(node, "summa_bs", "0"));
                decimal summaOS = Convert.ToDecimal(XmlHelper.GetStringAttrValue(node, "summa_os", "0"));
                if ((summaBS == 0) && (summaOS == 0))
                    continue;
                string refGroupNF = XmlHelper.GetStringAttrValue(node, "group_NFA", "0");
                string refSource = XmlHelper.GetStringAttrValue(node, "vd", "0");

                object[] mapping = new object[] { "summaBS", summaBS, "summaOS", summaOS, 
                    "RefOrg", refOrg, "refGroupNF", refGroupNF, "refSource", refSource, "RefYearDayUNV", curDate };
                PumpRow(dsFOFHDOS.Tables[0], mapping);
            }
            UpdateData();
        }

        #endregion FOFHDOS

        #region FOFHDOSAge

        private void PumpFOFHDOSAgeSum(XmlNodeList dataNodes, string sumName, string sumAttrName)
        {
            foreach (XmlNode node in dataNodes)
            {
                if (deleteMark)
                {
                    DirectDeleteFactData(new IFactTable[] { fctFOFHDOSAge }, -1, this.SourceID,
                        string.Format(" RefYearDayUNV = {0} and RefOrg = {1} ", curDate, refOrg));
                    deleteMark = false;
                }
                decimal sum = Convert.ToDecimal(XmlHelper.GetStringAttrValue(node, sumAttrName, "0"));
                if (sum == 0)
                    continue;
                string refTypeFund = XmlHelper.GetStringAttrValue(node, "type_os", "0");
                string refSource = XmlHelper.GetStringAttrValue(node, "vd", "0");

                object[] mapping = new object[] { sumName, sum, "RefOrg", refOrg, 
                    "refTypeFund", refTypeFund, "refSource", refSource, "RefYearDayUNV", curDate };
                PumpRow(dsFOFHDOSAge.Tables[0], mapping);
            }
        }

        private void PumpFOFHDOSAge(XmlNode dataNode)
        {
            deleteMark = true;
            PumpFOFHDOSAgeSum(dataNode.SelectNodes("property_org/depreciation_OS/depreciation"), "deteriroration", "deterioration");
            PumpFOFHDOSAgeSum(dataNode.SelectNodes("property_org/average_age_OS/average_age"), "middleAge", "middle_age");
            UpdateData();
        }

        #endregion FOFHDOSAge

        #region FOFHDResult

        private void PumpFOFHDResult(XmlNodeList dataNodes)
        {
            if (dataNodes.Count != 0)
                DirectDeleteFactData(new IFactTable[] { fctFOFHDResult }, -1, this.SourceID,
                    string.Format(" RefYearDayUNV = {0} and RefOrg = {1} ", curDate, refOrg));

            foreach (XmlNode node in dataNodes)
            {
                decimal summa = Convert.ToDecimal(XmlHelper.GetStringAttrValue(node, "summa", "0"));
                if (summa == 0)
                    continue;
                string refSource = XmlHelper.GetStringAttrValue(node, "vd", "0");

                object[] mapping = new object[] { "summa", summa, "RefOrg", refOrg, 
                    "refSource", refSource, "RefYearDayUNV", curDate };
                PumpRow(dsFOFHDResult.Tables[0], mapping);
            }
            UpdateData();
        }

        #endregion FOFHDResult

        private void PumpXmlFile(FileInfo file)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(file.FullName);
            try
            {
                GetDate(doc.SelectSingleNode("/xml/caption"));
                PumpOrg(doc.SelectSingleNode("/xml/caption"));

                PumpFOFHDMovFunds(doc.SelectSingleNode("/xml"));
                PumpFOFHDMarks(doc.SelectSingleNode("/xml"));
                PumpFOFHDPayable(doc.SelectNodes("xml/financial_indicators/accounts_payable/account_payable"));
                PumpFOFHDReceivable(doc.SelectNodes("xml/financial_indicators/debt_receivable/debts"));
                PumpFOFHDPost(doc.SelectNodes("xml/expense/average_payment/average"));
                PumpFOFHDUtil(doc.SelectNodes("xml/expense/utilities/utilitie"));
                PumpFOFHDMater(doc.SelectNodes("xml/expense/expense_other/expense_others"));
                PumpFOFHDTax(doc.SelectNodes("xml/expense/tax/taxs"));
                PumpFOFHDOS(doc.SelectNodes("xml/property_org/os/os"));
                PumpFOFHDOSAge(doc.SelectSingleNode("/xml"));
                PumpFOFHDResult(doc.SelectNodes("xml/financial_result/financial"));

                UpdateData();
            }
            finally
            {
                XmlHelper.ClearDomDocument(ref doc);
            }
        }

        #endregion работа с xml

        #region Перекрытые методы закачки

        protected override void ProcessFiles(DirectoryInfo dir)
        {
            ProcessFilesTemplate(dir, "*.xml", new ProcessFileDelegate(PumpXmlFile), false);
        }

        protected override void DirectPumpData()
        {
            PumpDataYTemplate();
        }

        #endregion Перекрытые методы закачки

        #endregion Закачка данных

        #region Обработка данных

        private void GroupTables()
        {
            string constr = string.Format("SourceId = {0}", this.SourceID);
            if (curDate != 0)
                constr += string.Format(" and (RefYearDayUNV = {0})", curDate);
            GroupTable(fctFOFHDMovFunds, new string[] { "RefOrg", "RefFacial", "RefAccount", "RefKOSGU", 
                                                        "RefCashFlw", "RefTypePay", "RefSource", "RefdKOSGU", "RefYearDayUNV" },
                new string[] { "EntranceFunds", "CashExpense", "RemainsExpense", "PlanIncome", "PlanExpense" }, constr);
            GroupTable(fctFOFHDMarks, new string[] { "RefOrg", "RefYearDayUNV" },
                new string[] { "Wages", "NumberWork", "Balance", "DebtsDelayed", "SummaDelay", "SummaDelayFounder", 
                               "DebtsPayment", "DebtsTax", "DebtsBalance" }, constr);
            GroupTable(fctFOFHDPayable, new string[] { "RefOrg", "RefYearDayUNV", "RefKOSGY", "refTypePay", "refArrears", "RefSource" },
                new string[] { "arrears", "arrearsPayment" }, constr);
            GroupTable(fctFOFHDReceivable, new string[] { "RefOrg", "RefYearDayUNV", "refArrears", "RefSource" },
                new string[] { "Summa" }, constr);
            GroupTable(fctFOFHDPost, new string[] { "RefOrg", "RefYearDayUNV", "refCategory" },
                new string[] { "summaCategory", "countCategory" }, constr);
            GroupTable(fctFOFHDUtil, new string[] { "RefOrg", "RefYearDayUNV", "refService", "refSource" },
                new string[] { "summa", "countService", "price" }, constr);
            GroupTable(fctFOFHDMater, new string[] { "RefOrg", "RefYearDayUNV", "refExpens", "refSource" },
                new string[] { "summa" }, constr);
            GroupTable(fctFOFHDTax, new string[] { "RefOrg", "RefYearDayUNV", "refTypePay", "refSource" },
                new string[] { "summa" }, constr);
            GroupTable(fctFOFHDOS, new string[] { "RefOrg", "RefYearDayUNV", "refGroupNF", "refSource" },
                new string[] { "summaBS", "summaOS" }, constr);
            GroupTable(fctFOFHDOSAge, new string[] { "RefOrg", "RefYearDayUNV", "refTypeFund", "refSource" },
                new string[] { "deteriroration", "middleAge" }, constr);
            GroupTable(fctFOFHDResult, new string[] { "RefOrg", "RefYearDayUNV", "refSource" },
                new string[] { "summa" }, constr);
        }

        protected override void ProcessDataSource()
        {
            GroupTables();
            UpdateData();
        }

        protected override void DirectProcessData()
        {
            ProcessDataSourcesTemplate("Установка иерархии классификаторов и коррекция сумм в таблицах фактов");
        }

        #endregion Обработка данных


    }
}
