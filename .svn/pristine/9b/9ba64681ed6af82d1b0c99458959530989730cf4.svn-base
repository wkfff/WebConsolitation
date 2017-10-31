using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

using Krista.FM.Common;
using Krista.FM.Server.Common;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.ServerLibrary;
using System.Xml;
using Krista.FM.Common.Xml;

using Stimate;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Net;

namespace Krista.FM.Server.DataPumps.FO28Pump
{
    // ФО 28 - закачка ас "смета"
    public class FO28PumpModule : CorrectedPumpModuleBase
    {

        #region Поля

        #region Классификаторы

        // Районы.Планирование (d_Regions_Plan)
        private IDbDataAdapter daRegions;
        private DataSet dsRegions;
        private IClassifier clsRegions;
        private Dictionary<string, int> cacheRegions = null;
        // Организации.Планирование (d_Organizations_Plan)
        private IDbDataAdapter daOrg;
        private DataSet dsOrg;
        private IClassifier clsOrg;
        private Dictionary<string, int> cacheOrg = null;
        // ИФ.Виды ценных бумаг (d_S_Capital)
        private IDbDataAdapter daCapital;
        private DataSet dsCapital;
        private IClassifier clsCapital;
        private Dictionary<string, int> cacheCapital = null;
        // ИФ.Виды заимствований (d_S_KindBorrow)
        private IDbDataAdapter daKindBorrow;
        private DataSet dsKindBorrow;
        private IClassifier clsKindBorrow;
        private Dictionary<string, int> cacheKindBorrow = null;
        // ИФ.Виды заимствований (d_S_KindBorrow)
        private IDbDataAdapter daOkv;
        private DataSet dsOkv;
        private IClassifier clsOkv;
        private Dictionary<string, int> cacheOkv = null;
        // ИФ.Вид ссуды (d_S_Extensions)
        private IDbDataAdapter daExtensions;
        private DataSet dsExtensions;
        private IClassifier clsExtensions;
        private Dictionary<string, int> cacheExtensions = null;
        // ИФ.Курсы валют (d_S_ExchangeRate)
        private IDbDataAdapter daExchangeRate;
        private DataSet dsExchangeRate;
        private IClassifier clsExchangeRate;
        private Dictionary<string, int> cacheExchangeRate = null;

        #endregion Классификаторы

        #region Факты

        #region Кредиты полученные

        // ИФ.Кредиты полученные (f_S_Creditincome)
        private IDbDataAdapter daCreditIncome;
        private DataSet dsCreditIncome;
        private IFactTable fctCreditIncome;
        private Dictionary<int, DataRow> cacheCreditIncome = null;
        // ИФ.План привлечения (t_S_PlanAttractCI)
        private IDbDataAdapter daPlanAttractCI;
        private DataSet dsPlanAttractCI;
        private IEntity fctPlanAttractCI;
        // ИФ.План погашения основного долга (t_S_PlanDebtCI)
        private IDbDataAdapter daPlanDebtCI;
        private DataSet dsPlanDebtCI;
        private IEntity fctPlanDebtCI;
        // ИФ.План обслуживания долга (t_S_PlanServiceCI)
        private IDbDataAdapter daPlanServiceCI;
        private DataSet dsPlanServiceCI;
        private IEntity fctPlanServiceCI;
        // ИФ.Дополнительные расходы (t_S_CostAttractCI)
        private IDbDataAdapter daCostAttractCI;
        private DataSet dsCostAttractCI;
        private IEntity fctCostAttractCI;
        // ИФ.Факт привлечения (t_S_FactAttractCI)
        private IDbDataAdapter daFactAttractCI;
        private DataSet dsFactAttractCI;
        private IEntity fctFactAttractCI;
        // ИФ.Факт погашения основного долга (t_S_FactDebtCI)
        private IDbDataAdapter daFactDebtCI;
        private DataSet dsFactDebtCI;
        private IEntity fctFactDebtCI;
        // ИФ.Факт погашения процентов (t_S_FactPercentCI)
        private IDbDataAdapter daFactPercentCI;
        private DataSet dsFactPercentCI;
        private IEntity fctFactPercentCI;
        // ИФ.Факт погашения пени по основному долгу (t_S_FactPenaltyDebtCI)
        private IDbDataAdapter daFactPenaltyDebtCI;
        private DataSet dsFactPenaltyDebtCI;
        private IEntity fctFactPenaltyDebtCI;
        // ИФ.Факт погашения пени по процентам (t_S_FactPenaltyPercentCI)
        private IDbDataAdapter daFactPenaltyPercentCI;
        private DataSet dsFactPenaltyPercentCI;
        private IEntity fctFactPenaltyPercentCI;
        // ИФ.Обеспечение (t_S_CollateralCI)
        private IDbDataAdapter daCollateralCI;
        private DataSet dsCollateralCI;
        private IEntity fctCollateralCI;
        // ИФ.Курсовая разница (t_S_RateSwitchCI)
        private IDbDataAdapter daRateSwitchCI;
        private DataSet dsRateSwitchCI;
        private IEntity fctRateSwitchCI;
        // ИФ.Журнал процентов (t_S_JournalPercentCI)
        private IDbDataAdapter daJournalPercentCI;
        private DataSet dsJournalPercentCI;
        private IEntity fctJournalPercentCI;

        #endregion Кредиты полученные

        #region Гарантии предоставленные

        // ИФ.Гарантии предоставленные (f_S_Guarantissued)
        private IDbDataAdapter daGuarantIssued;
        private DataSet dsGuarantIssued;
        private IFactTable fctGuarantIssued;
        private Dictionary<int, DataRow> cacheGuarantIssued = null;
        // ИФ.Факт исполнения обязательств гарантом (t_S_FactAttractGrnt)
        private IDbDataAdapter daFactAttractGrnt;
        private DataSet dsFactAttractGrnt;
        private IEntity fctFactAttractGrnt;
        // ИФ.Факт погашения основного долга (t_S_FactDebtPrGrnt)
        private IDbDataAdapter daFactDebtPrGrnt;
        private DataSet dsFactDebtPrGrnt;
        private IEntity fctFactDebtPrGrnt;
        // ИФ.Факт погашения процентов (t_S_FactPercentPrGrnt)
        private IDbDataAdapter daFactPercentPrGrnt;
        private DataSet dsFactPercentPrGrnt;
        private IEntity fctFactPercentPrGrnt;
        // ИФ.Факт погашения пени по основному долгу (t_S_FactPenaltyDebtPrGrnt)
        private IDbDataAdapter daFactPenaltyDebtPrGrnt;
        private DataSet dsFactPenaltyDebtPrGrnt;
        private IEntity fctFactPenaltyDebtPrGrnt;
        // ИФ.Факт погашения пени по процентам (t_S_FactPenaltyPercentPrGrnt)
        private IDbDataAdapter daFactPenaltyPercentPrGrnt;
        private DataSet dsFactPenaltyPercentPrGrnt;
        private IEntity fctFactPenaltyPercentPrGrnt;
        // ИФ.Обеспечение (t_S_CollateralGrnt)
        private IDbDataAdapter daCollateralGrnt;
        private DataSet dsCollateralGrnt;
        private IEntity fctCollateralGrnt;
        // ИФ.Журнал процентов (t_S_JournalPercentGrnt)
        private IDbDataAdapter daJournalPercentGrnt;
        private DataSet dsJournalPercentGrnt;
        private IEntity fctJournalPercentGrnt;

        #endregion Гарантии предоставленные

        #region Кредиты предоставленные

        // ИФ.Кредиты предоставленные (f_S_Creditissued)
        private IDbDataAdapter daCreditIssued;
        private DataSet dsCreditIssued;
        private IFactTable fctCreditIssued;
        private Dictionary<int, DataRow> cacheCreditIssued = null;
        // ИФ.План погашения основного долга (t_S_PlanDebtCO)
        private IDbDataAdapter daPlanDebtCO;
        private DataSet dsPlanDebtCO;
        private IEntity fctPlanDebtCO;
        // ИФ.План обслуживания долга (t_S_PlanServiceCO)
        private IDbDataAdapter daPlanServiceCO;
        private DataSet dsPlanServiceCO;
        private IEntity fctPlanServiceCO;
        // ИФ.Факт предоставления (t_S_FactAttractCO)
        private IDbDataAdapter daFactAttractCO;
        private DataSet dsFactAttractCO;
        private IEntity fctFactAttractCO;
        // ИФ.Факт погашения основного долга (t_S_FactDebtCO)
        private IDbDataAdapter daFactDebtCO;
        private DataSet dsFactDebtCO;
        private IEntity fctFactDebtCO;
        // ИФ.Факт погашения процентов (t_S_FactPercentCO)
        private IDbDataAdapter daFactPercentCO;
        private DataSet dsFactPercentCO;
        private IEntity fctFactPercentCO;
        // ИФ.Факт погашения пени по основному долгу (t_S_FactPenaltyDebtCO)
        private IDbDataAdapter daFactPenaltyDebtCO;
        private DataSet dsFactPenaltyDebtCO;
        private IEntity fctFactPenaltyDebtCO;
        // ИФ.Факт погашения пени по процентам (t_S_FactPenaltyPercentCO)
        private IDbDataAdapter daFactPenaltyPercentCO;
        private DataSet dsFactPenaltyPercentCO;
        private IEntity fctFactPenaltyPercentCO;
        // ИФ.Журнал процентов (t_S_JournalPercentCO)
        private IDbDataAdapter daJournalPercentCO;
        private DataSet dsJournalPercentCO;
        private IEntity fctJournalPercentCO;

        #endregion Кредиты предоставленные

        #region ценные бумаги

        // ИФ.Ценные бумаги (f_S_Capital)
        private IDbDataAdapter daCapitalFct;
        private DataSet dsCapitalFct;
        private IFactTable fctCapitalFct;
        private Dictionary<int, DataRow> cacheCapitalFct = null;
        // ИФ.Итоги размещения (t_S_CPFactCapital)
        private IDbDataAdapter daCPFactCapital;
        private DataSet dsCPFactCapital;
        private IEntity fctCPFactCapital;
        // ИФ.План погашения номинальной стоимости (t_S_CPPlanDebt)
        private IDbDataAdapter daCPPlanDebt;
        private DataSet dsCPPlanDebt;
        private IEntity fctCPPlanDebt;
        // ИФ.Факт погашения номинальной стоимости (t_S_CPFactDebt)
        private IDbDataAdapter daCPFactDebt;
        private DataSet dsCPFactDebt;
        private IEntity fctCPFactDebt;
        // ИФ.Факт выплаты дохода (t_S_CPFactService)
        private IDbDataAdapter daCPFactService;
        private DataSet dsCPFactService;
        private IEntity fctCPFactService;
        // ИФ.Обеспечение (t_S_CPCollateral)
        private IDbDataAdapter daCPCollateral;
        private DataSet dsCPCollateral;
        private IEntity fctCPCollateral;
        // ИФ.Дополнительные расходы (t_S_CPFactCost)
        private IDbDataAdapter daCPFactCost;
        private DataSet dsCPFactCost;
        private IEntity fctCPFactCost;
        // ИФ.Журнал ставок процентов (t_S_CPJournalPercent)
        private IDbDataAdapter daCPJournalPercent;
        private DataSet dsCPJournalPercent;
        private IEntity fctCPJournalPercent;
        // ИФ.Курсовая разница (t_S_CPRateSwitch)
        private IDbDataAdapter daCPRateSwitch;
        private DataSet dsCPRateSwitch;
        private IEntity fctCPRateSwitch;
        // ИФ.План размещения (t_S_CPPlanCapital)
        private IDbDataAdapter daCPPlanCapital;
        private DataSet dsCPPlanCapital;
        private IEntity fctCPPlanCapital;

        #endregion ценные бумаги

        #endregion Факты

        private int clsSourceId = -1;
        // качаем на вариант 0 - действующие договора
        private int refVariant = 0;
        private string dogovorOkv = string.Empty;
        // айди родительской записи
        private int factId = -1;
        // имя поля - ссылки на мастер
        private string refMasterFieldName = string.Empty;
        // кол во записей в базе
        private int creditIncomeRowsCount = 0;
        private int creditIssuedRowsCount = 0;
        private int grntIssuedRowsCount = 0;
        private int capitalRowsCount = 0;
        private int curMasterRecordCount = 0;
        private bool nullNumPayOrder = false;
        // текущий блок
        private Block block;
        // дата договора
        private object startDate;

        private bool isUpdate = false;

        private bool toPumpCrInc = false;
        private bool toPumpCrIss = false;
        private bool toPumpGrntIss = false;
        private bool toPumpCapital = false;

        private StimateConnectMode stimateConnectMode;

        ContainerClass stimateClass = null;

        #endregion Поля

        #region константы

        private enum Block
        {
            bCreditIncomes,
            bCreditIssued,
            bGrntIssued,
            bCapital
        }

        public enum StimateConnectMode
        {
            useLocalServer= 0,
            useWebService = 1,
            useNothing  = 2
        }

        #endregion константы

        #region Закачка данных

        #region Работа с базой и кэшами

        protected override void QueryData()
        {
            clsSourceId = AddDataSource("ФО", "0029", ParamKindTypes.Year, string.Empty, this.DataSource.Year, 0, string.Empty, 0, string.Empty).ID;
            string constr = string.Format("SOURCEID = {0}", clsSourceId);
            InitDataSet(ref daRegions, ref dsRegions, clsRegions, false, constr, string.Empty);
            InitDataSet(ref daOrg, ref dsOrg, clsOrg, false, constr, string.Empty);
            InitDataSet(ref daCapital, ref dsCapital, clsCapital, false, string.Empty, string.Empty);
            InitDataSet(ref daKindBorrow, ref dsKindBorrow, clsKindBorrow, false, string.Empty, string.Empty);
            InitDataSet(ref daOkv, ref dsOkv, clsOkv, false, string.Empty, string.Empty);
            InitDataSet(ref daExtensions, ref dsExtensions, clsExtensions, false, string.Empty, string.Empty);
            InitDataSet(ref daExchangeRate, ref dsExchangeRate, clsExchangeRate, false, string.Empty, string.Empty);

            #region Кредиты полученные

            InitDataSet(ref daCreditIncome, ref dsCreditIncome, fctCreditIncome, false, string.Empty, string.Empty, false);
            creditIncomeRowsCount = dsCreditIncome.Tables[0].Rows.Count;

            InitDataSet(ref daPlanAttractCI, ref dsPlanAttractCI, fctPlanAttractCI, string.Empty);
            InitDataSet(ref daPlanDebtCI, ref dsPlanDebtCI, fctPlanDebtCI, string.Empty);
            InitDataSet(ref daPlanServiceCI, ref dsPlanServiceCI, fctPlanServiceCI, string.Empty);
            InitDataSet(ref daCostAttractCI, ref dsCostAttractCI, fctCostAttractCI, string.Empty);
            InitDataSet(ref daFactAttractCI, ref dsFactAttractCI, fctFactAttractCI, string.Empty);
            InitDataSet(ref daFactDebtCI, ref dsFactDebtCI, fctFactDebtCI, string.Empty);
            InitDataSet(ref daFactPercentCI, ref dsFactPercentCI, fctFactPercentCI, string.Empty);
            InitDataSet(ref daFactPenaltyDebtCI, ref dsFactPenaltyDebtCI, fctFactPenaltyDebtCI, string.Empty);
            InitDataSet(ref daFactPenaltyPercentCI, ref dsFactPenaltyPercentCI, fctFactPenaltyPercentCI, string.Empty);
            InitDataSet(ref daCollateralCI, ref dsCollateralCI, fctCollateralCI, string.Empty);
            InitDataSet(ref daRateSwitchCI, ref dsRateSwitchCI, fctRateSwitchCI, string.Empty);
            InitDataSet(ref daJournalPercentCI, ref dsJournalPercentCI, fctJournalPercentCI, string.Empty);

            #endregion Кредиты полученные

            #region Кредиты предоставленные

            InitDataSet(ref daCreditIssued, ref dsCreditIssued, fctCreditIssued, false, string.Empty, string.Empty, false);
            creditIssuedRowsCount = dsCreditIssued.Tables[0].Rows.Count;

            InitDataSet(ref daPlanDebtCO, ref dsPlanDebtCO, fctPlanDebtCO, string.Empty);
            InitDataSet(ref daPlanServiceCO, ref dsPlanServiceCO, fctPlanServiceCO, string.Empty);
            InitDataSet(ref daFactAttractCO, ref dsFactAttractCO, fctFactAttractCO, string.Empty);
            InitDataSet(ref daFactDebtCO, ref dsFactDebtCO, fctFactDebtCO, string.Empty);
            InitDataSet(ref daFactPercentCO, ref dsFactPercentCO, fctFactPercentCO, string.Empty);
            InitDataSet(ref daFactPenaltyDebtCO, ref dsFactPenaltyDebtCO, fctFactPenaltyDebtCO, string.Empty);
            InitDataSet(ref daFactPenaltyPercentCO, ref dsFactPenaltyPercentCO, fctFactPenaltyPercentCO, string.Empty);
            InitDataSet(ref daJournalPercentCO, ref dsJournalPercentCO, fctJournalPercentCO, string.Empty);

            #endregion Кредиты предоставленные

            #region Гарантии предоставленные

            InitDataSet(ref daGuarantIssued, ref dsGuarantIssued, fctGuarantIssued, false, string.Empty, string.Empty, false);
            grntIssuedRowsCount = dsGuarantIssued.Tables[0].Rows.Count;

            InitDataSet(ref daFactAttractGrnt, ref dsFactAttractGrnt, fctFactAttractGrnt, string.Empty);
            InitDataSet(ref daFactDebtPrGrnt, ref dsFactDebtPrGrnt, fctFactDebtPrGrnt, string.Empty);
            InitDataSet(ref daFactPercentPrGrnt, ref dsFactPercentPrGrnt, fctFactPercentPrGrnt, string.Empty);
            InitDataSet(ref daFactPenaltyDebtPrGrnt, ref dsFactPenaltyDebtPrGrnt, fctFactPenaltyDebtPrGrnt, string.Empty);
            InitDataSet(ref daFactPenaltyPercentPrGrnt, ref dsFactPenaltyPercentPrGrnt, fctFactPenaltyPercentPrGrnt, string.Empty);
            InitDataSet(ref daCollateralGrnt, ref dsCollateralGrnt, fctCollateralGrnt, string.Empty);
            InitDataSet(ref daJournalPercentGrnt, ref dsJournalPercentGrnt, fctJournalPercentGrnt, string.Empty);

            #endregion Гарантии предоставленные

            #region ценные бумаги

            InitDataSet(ref daCapitalFct, ref dsCapitalFct, fctCapitalFct, false, string.Empty, string.Empty, false);
            capitalRowsCount = dsCapitalFct.Tables[0].Rows.Count;

            InitDataSet(ref daCPFactCapital, ref dsCPFactCapital, fctCPFactCapital, string.Empty);
            InitDataSet(ref daCPPlanDebt, ref dsCPPlanDebt, fctCPPlanDebt, string.Empty);
            InitDataSet(ref daCPFactDebt, ref dsCPFactDebt, fctCPFactDebt, string.Empty);
            InitDataSet(ref daCPFactService, ref dsCPFactService, fctCPFactService, string.Empty);
            InitDataSet(ref daCPCollateral, ref dsCPCollateral, fctCPCollateral, string.Empty);
            InitDataSet(ref daCPFactCost, ref dsCPFactCost, fctCPFactCost, string.Empty);
            InitDataSet(ref daCPJournalPercent, ref dsCPJournalPercent, fctCPJournalPercent, string.Empty);
            InitDataSet(ref daCPRateSwitch, ref dsCPRateSwitch, fctCPRateSwitch, string.Empty);
            InitDataSet(ref daCPPlanCapital, ref dsCPPlanCapital, fctCPPlanCapital, string.Empty);

            #endregion ценные бумаги

            FillCaches();
        }

        private void FillCaches()
        {
            FillRowsCache(ref cacheRegions, dsRegions.Tables[0], "IDzap", "Id");
            FillRowsCache(ref cacheOrg, dsOrg.Tables[0], "IDzap", "Id");
            FillRowsCache(ref cacheCapital, dsCapital.Tables[0], "SourceKey", "Id");
            FillRowsCache(ref cacheKindBorrow, dsKindBorrow.Tables[0], "SourceKey", "Id");
            FillRowsCache(ref cacheOkv, dsOkv.Tables[0], "Code", "Id");
            FillRowsCache(ref cacheExtensions, dsExtensions.Tables[0], "Code", "Id");
            FillRowsCache(ref cacheExchangeRate, dsExchangeRate.Tables[0], "DateFixing", "Id");

            FillRowsCache(ref cacheCreditIncome, dsCreditIncome.Tables[0], "IDDoc");
            FillRowsCache(ref cacheCreditIssued, dsCreditIssued.Tables[0], "IDDoc");
            FillRowsCache(ref cacheGuarantIssued, dsGuarantIssued.Tables[0], "IDDoc");
            FillRowsCache(ref cacheCapitalFct, dsCapitalFct.Tables[0], "IDDoc");

        }

        protected override void UpdateData()
        {
            UpdateDataSet(daRegions, dsRegions, clsRegions);
            UpdateDataSet(daOrg, dsOrg, clsOrg);
            UpdateDataSet(daCapital, dsCapital, clsCapital);
            UpdateDataSet(daKindBorrow, dsKindBorrow, clsKindBorrow);
            UpdateDataSet(daExtensions, dsExtensions, clsExtensions);
            UpdateDataSet(daExchangeRate, dsExchangeRate, clsExchangeRate);

            #region Кредиты полученные

            UpdateDataSet(daCreditIncome, dsCreditIncome, fctCreditIncome);
            UpdateDataSet(daPlanAttractCI, dsPlanAttractCI, fctPlanAttractCI);
            UpdateDataSet(daPlanDebtCI, dsPlanDebtCI, fctPlanDebtCI);
            UpdateDataSet(daPlanServiceCI, dsPlanServiceCI, fctPlanServiceCI);
            UpdateDataSet(daCostAttractCI, dsCostAttractCI, fctCostAttractCI);
            UpdateDataSet(daFactAttractCI, dsFactAttractCI, fctFactAttractCI);
            UpdateDataSet(daFactDebtCI, dsFactDebtCI, fctFactDebtCI);
            UpdateDataSet(daFactPercentCI, dsFactPercentCI, fctFactPercentCI);
            UpdateDataSet(daFactPenaltyDebtCI, dsFactPenaltyDebtCI, fctFactPenaltyDebtCI);
            UpdateDataSet(daFactPenaltyPercentCI, dsFactPenaltyPercentCI, fctFactPenaltyPercentCI);
            UpdateDataSet(daCollateralCI, dsCollateralCI, fctCollateralCI);
            UpdateDataSet(daRateSwitchCI, dsRateSwitchCI, fctRateSwitchCI);
            UpdateDataSet(daJournalPercentCI, dsJournalPercentCI, fctJournalPercentCI);

            #endregion Кредиты полученные

            #region Кредиты предоставленные

            UpdateDataSet(daCreditIssued, dsCreditIssued, fctCreditIssued);
            UpdateDataSet(daPlanDebtCO, dsPlanDebtCO, fctPlanDebtCO);
            UpdateDataSet(daPlanServiceCO, dsPlanServiceCO, fctPlanServiceCO);
            UpdateDataSet(daFactAttractCO, dsFactAttractCO, fctFactAttractCO);
            UpdateDataSet(daFactDebtCO, dsFactDebtCO, fctFactDebtCO);
            UpdateDataSet(daFactPercentCO, dsFactPercentCO, fctFactPercentCO);
            UpdateDataSet(daFactPenaltyDebtCO, dsFactPenaltyDebtCO, fctFactPenaltyDebtCO);
            UpdateDataSet(daFactPenaltyPercentCO, dsFactPenaltyPercentCO, fctFactPenaltyPercentCO);
            UpdateDataSet(daJournalPercentCO, dsJournalPercentCO, fctJournalPercentCO);

            #endregion Кредиты предоставленные

            #region Гарантии предоставленные

            UpdateDataSet(daGuarantIssued, dsGuarantIssued, fctGuarantIssued);
            UpdateDataSet(daFactAttractGrnt, dsFactAttractGrnt, fctFactAttractGrnt);
            UpdateDataSet(daFactDebtPrGrnt, dsFactDebtPrGrnt, fctFactDebtPrGrnt);
            UpdateDataSet(daFactPercentPrGrnt, dsFactPercentPrGrnt, fctFactPercentPrGrnt);
            UpdateDataSet(daFactPenaltyDebtPrGrnt, dsFactPenaltyDebtPrGrnt, fctFactPenaltyDebtPrGrnt);
            UpdateDataSet(daFactPenaltyPercentPrGrnt, dsFactPenaltyPercentPrGrnt, fctFactPenaltyPercentPrGrnt);
            UpdateDataSet(daCollateralGrnt, dsCollateralGrnt, fctCollateralGrnt);
            UpdateDataSet(daJournalPercentGrnt, dsJournalPercentGrnt, fctJournalPercentGrnt);

            #endregion Гарантии предоставленные

            #region ценные бумаги

            UpdateDataSet(daCapitalFct, dsCapitalFct, fctCapitalFct);
            UpdateDataSet(daCPFactCapital, dsCPFactCapital, fctCPFactCapital);
            UpdateDataSet(daCPPlanDebt, dsCPPlanDebt, fctCPPlanDebt);
            UpdateDataSet(daCPFactDebt, dsCPFactDebt, fctCPFactDebt);
            UpdateDataSet(daCPFactService, dsCPFactService, fctCPFactService);
            UpdateDataSet(daCPCollateral, dsCPCollateral, fctCPCollateral);
            UpdateDataSet(daCPFactCost, dsCPFactCost, fctCPFactCost);
            UpdateDataSet(daCPJournalPercent, dsCPJournalPercent, fctCPJournalPercent);
            UpdateDataSet(daCPRateSwitch, dsCPRateSwitch, fctCPRateSwitch);
            UpdateDataSet(daCPPlanCapital, dsCPPlanCapital, fctCPPlanCapital);

            #endregion ценные бумаги

        }

        private void InitDetail(IFactTable fct, string guid, ref IEntity detailFct)
        {
            foreach (IEntityAssociation association in fct.Associated.Values)
            {
                if (association.AssociationClassType != AssociationClassTypes.MasterDetail)
                    continue;
                if (association.RoleData.ObjectKey == guid)
                {
                    detailFct = association.RoleData;
                    break;
                }
            }
        }

        private const string D_REGIONS_GUID = "1f34cc90-16fd-4fcf-b994-0c8a680d7e23";
        private const string D_ORG_GUID = "aeabb871-e583-439b-a329-b1f6ce78f212";
        private const string D_CAPITAL_GUID = "883bf07d-1460-4f1e-b92d-15e2c5b9b51f";
        private const string D_KIND_BORROW_GUID = "b18eda9d-cc97-4e68-8724-9a343cc2298c";
        private const string D_OKV_GUID = "72f22b5d-fea9-4c6b-8c8d-a6df7ddc4db0";
        private const string D_EXTENSIONS_GUID = "fb83c43e-4db7-48a7-933b-5454aca9e7d3";
        private const string D_EXCHANGE_RATE_GUID = "570d06d9-fe94-4091-96f4-2556e4150df4";

        #region Кредиты полученные

        private const string F_CREDIT_INCOME_GUID = "d3a9668b-0a65-4a6a-bca6-090768c822d0";
        private const string T_PLAN_ATTRACT_CI_GUID = "389e43c5-dd23-45d6-bc00-73f9ef0eca71";
        private const string T_PLAN_DEBT_CI_GUID = "fa3aea1c-e4eb-4fbf-9de4-934caa850749";
        private const string T_PLAN_SERVICE_CI_GUID = "b8f32e48-4c65-4e1a-9b16-70bf57648734";
        private const string T_COST_ATTRACT_CI_GUID = "3d9f981c-db4a-4cbf-8b62-bda21cb09014";
        private const string T_FACT_ATTRACT_CI_GUID = "f2bf5a02-6f76-4aa5-ae0f-0b4778072432";
        private const string T_FACT_DEBT_CI_GUID = "0e4d071b-8961-44a5-abb8-9facf5f7703c";
        private const string T_FACT_PERCENT_CI_GUID = "74f8b77c-84fa-4a49-96f2-8dd966128091";
        private const string T_FACT_PENALTY_DEBT_CI_GUID = "b0e4e16d-0b63-4c63-b19c-13b520fe795b";
        private const string T_FACT_PENALTY_PERCENT_CI_GUID = "a44e4af5-214e-44e3-b523-d30fcb09e500";
        private const string T_COLLATERAL_CI_GUID = "05597386-a326-45a5-9fab-6d3ed62e66a3";
        private const string T_RATE_SWITCH_CI_GUID = "5e7be224-5eb5-47bb-bcd0-3df5d0726439";
        private const string T_JOURNAL_PERCENT_CI_GUID = "28385425-8d75-4558-ab5a-fd4c7576694a";

        #endregion Кредиты полученные

        #region Кредиты предоставленные

        private const string F_CREDIT_ISSUED_GUID = "fb029d1d-e648-46b4-8a1f-bff21ea0fbf5";
        private const string T_PLAN_DEBT_CO_GUID = "8ab4b2f9-0744-4661-bacf-7d491cb2706e";
        private const string T_PLAN_SERVICE_CO_GUID = "ed7684bc-9390-4d54-8d73-bd35dd29d46d";
        private const string T_FACT_ATTRACT_CO_GUID = "cd88ce9b-d6d5-4051-8759-d12f36e5ca9d";
        private const string T_FACT_DEBT_CO_GUID = "725ab522-f0b8-428d-ac45-22c2118b0e8d";
        private const string T_FACT_PERCENT_CO_GUID = "c7cb6d4b-b1c5-4bc9-bab6-9a5e41bec95f";
        private const string T_FACT_PENALTY_DEBT_CO_GUID = "fe685500-9699-4d17-882d-544b050dd6e1";
        private const string T_FACT_PENALTY_PERCENT_CO_GUID = "3623c2b9-c1c8-4f77-879c-23b6e0978171";
        private const string T_JOURNAL_PERCENT_CO_GUID = "927b4cbe-70a2-4fd4-b6bd-00b3daa8e6bc";

        #endregion Кредиты предоставленные

        #region Гарантии предоставленные

        private const string F_GUARANT_ISSUED_GUID = "042556fd-89a9-4b44-bc3e-2e645560a6bf";
        private const string T_FACT_ATTRACT_GRNT_GUID = "1d326cf4-4d1b-4ea2-a61e-a8bcc8781ebb";
        private const string T_FACT_DEBT_PR_GRNT_GUID = "e27a1f65-e277-434f-a3e0-90f6e076ca33";
        private const string T_FACT_PERCENT_PR_GRNT_GUID = "09865793-fae0-4cf7-95b3-c0dcd5bb7340";
        private const string T_FACT_PENALTY_DEBT_PR_GRNT_GUID = "90ab578c-a87d-42b5-ba26-c739e7f1f5f7";
        private const string T_FACT_PENALTY_PERCENT_PR_GRNT_GUID = "8b05645b-38d7-460d-863e-4c4ba0c65712";
        private const string T_COLLATERAL_GRNT_GUID = "7446bef4-72e0-44c9-ae3d-dad51b969098";
        private const string T_JOURNAL_PERCENT_GRNT_GUID = "c1c09e58-c013-4005-9a12-aed8f2f466fd";

        #endregion Гарантии предоставленные

        #region ценные бумаги

        private const string F_CAPITAL_GUID = "799c95c4-1816-45dc-8faf-1326767c0a98";
        private const string T_CP_FACT_CAPITAL_GUID = "5fce32c3-2693-4799-973a-216e8e71001f";
        private const string T_CP_PLAN_DEBT_GUID = "b4576b34-a1b5-4368-b731-a386118a9672";
        private const string T_CP_FACT_DEBT_GUID = "e6daab7b-d5cd-431f-8d89-8c2a5f4d5026";
        private const string T_CP_FACT_SERVICE_GUID = "77274309-1858-4719-800b-a20b021b1170";
        private const string T_CP_COLLATERAL_GUID = "c01bed6b-58b7-4668-ace6-969af9668802";
        private const string T_CP_FACT_COST_GUID = "e63f05ed-4fe1-4bdd-a1ed-47b98010c900";
        private const string T_CP_JOURNAL_PERCENT_GUID = "fe8ead9d-9671-4a5b-8ad2-960b864972d7";
        private const string T_CP_RATE_SWITCH_GUID = "1715b2a7-a50a-441f-b3c1-c1a6eb11e442";
        private const string T_CP_PLAN_CAPITAL_GUID = "2ae8e70a-2065-4a8f-bd96-63c6a5f1a018";

        #endregion ценные бумаги

        protected override void InitDBObjects()
        {
            clsRegions = this.Scheme.Classifiers[D_REGIONS_GUID];
            clsOrg = this.Scheme.Classifiers[D_ORG_GUID];
            clsCapital = this.Scheme.Classifiers[D_CAPITAL_GUID];
            clsKindBorrow = this.Scheme.Classifiers[D_KIND_BORROW_GUID];
            clsOkv = this.Scheme.Classifiers[D_OKV_GUID];
            clsExtensions = this.Scheme.Classifiers[D_EXTENSIONS_GUID];
            clsExchangeRate = this.Scheme.Classifiers[D_EXCHANGE_RATE_GUID];
            this.UsedClassifiers = new IClassifier[] { };

            #region Кредиты полученные

            fctCreditIncome = this.Scheme.FactTables[F_CREDIT_INCOME_GUID];
            InitDetail(fctCreditIncome, T_PLAN_ATTRACT_CI_GUID, ref fctPlanAttractCI);
            InitDetail(fctCreditIncome, T_PLAN_DEBT_CI_GUID, ref fctPlanDebtCI);
            InitDetail(fctCreditIncome, T_PLAN_SERVICE_CI_GUID, ref fctPlanServiceCI);
            InitDetail(fctCreditIncome, T_COST_ATTRACT_CI_GUID, ref fctCostAttractCI);
            InitDetail(fctCreditIncome, T_FACT_ATTRACT_CI_GUID, ref fctFactAttractCI);
            InitDetail(fctCreditIncome, T_FACT_DEBT_CI_GUID, ref fctFactDebtCI);
            InitDetail(fctCreditIncome, T_FACT_PERCENT_CI_GUID, ref fctFactPercentCI);
            InitDetail(fctCreditIncome, T_FACT_PENALTY_DEBT_CI_GUID, ref fctFactPenaltyDebtCI);
            InitDetail(fctCreditIncome, T_FACT_PENALTY_PERCENT_CI_GUID, ref fctFactPenaltyPercentCI);
            InitDetail(fctCreditIncome, T_COLLATERAL_CI_GUID, ref fctCollateralCI);
            InitDetail(fctCreditIncome, T_RATE_SWITCH_CI_GUID, ref fctRateSwitchCI);
            InitDetail(fctCreditIncome, T_JOURNAL_PERCENT_CI_GUID, ref fctJournalPercentCI);

            #endregion Кредиты полученные

            #region Кредиты предоставленные

            fctCreditIssued = this.Scheme.FactTables[F_CREDIT_ISSUED_GUID];
            InitDetail(fctCreditIssued, T_PLAN_DEBT_CO_GUID, ref fctPlanDebtCO);
            InitDetail(fctCreditIssued, T_PLAN_SERVICE_CO_GUID, ref fctPlanServiceCO);
            InitDetail(fctCreditIssued, T_FACT_ATTRACT_CO_GUID, ref fctFactAttractCO);
            InitDetail(fctCreditIssued, T_FACT_DEBT_CO_GUID, ref fctFactDebtCO);
            InitDetail(fctCreditIssued, T_FACT_PERCENT_CO_GUID, ref fctFactPercentCO);
            InitDetail(fctCreditIssued, T_FACT_PENALTY_DEBT_CO_GUID, ref fctFactPenaltyDebtCO);
            InitDetail(fctCreditIssued, T_FACT_PENALTY_PERCENT_CO_GUID, ref fctFactPenaltyPercentCO);
            InitDetail(fctCreditIssued, T_JOURNAL_PERCENT_CO_GUID, ref fctJournalPercentCO);

           #endregion Кредиты предоставленные

            #region Гарантии предоставленные

            fctGuarantIssued = this.Scheme.FactTables[F_GUARANT_ISSUED_GUID];
            InitDetail(fctGuarantIssued, T_FACT_ATTRACT_GRNT_GUID, ref fctFactAttractGrnt);
            InitDetail(fctGuarantIssued, T_FACT_DEBT_PR_GRNT_GUID, ref fctFactDebtPrGrnt);
            InitDetail(fctGuarantIssued, T_FACT_PERCENT_PR_GRNT_GUID, ref fctFactPercentPrGrnt);
            InitDetail(fctGuarantIssued, T_FACT_PENALTY_DEBT_PR_GRNT_GUID, ref fctFactPenaltyDebtPrGrnt);
            InitDetail(fctGuarantIssued, T_FACT_PENALTY_PERCENT_PR_GRNT_GUID, ref fctFactPenaltyPercentPrGrnt);
            InitDetail(fctGuarantIssued, T_COLLATERAL_GRNT_GUID, ref fctCollateralGrnt);
            InitDetail(fctGuarantIssued, T_JOURNAL_PERCENT_GRNT_GUID, ref fctJournalPercentGrnt);

            #endregion Гарантии предоставленные

            #region ценные бумаги

            fctCapitalFct = this.Scheme.FactTables[F_CAPITAL_GUID];
            InitDetail(fctCapitalFct, T_CP_FACT_CAPITAL_GUID, ref fctCPFactCapital);
            InitDetail(fctCapitalFct, T_CP_PLAN_DEBT_GUID, ref fctCPPlanDebt);
            InitDetail(fctCapitalFct, T_CP_FACT_DEBT_GUID, ref fctCPFactDebt);
            InitDetail(fctCapitalFct, T_CP_FACT_SERVICE_GUID, ref fctCPFactService);
            InitDetail(fctCapitalFct, T_CP_COLLATERAL_GUID, ref fctCPCollateral);
            InitDetail(fctCapitalFct, T_CP_FACT_COST_GUID, ref fctCPFactCost);
            InitDetail(fctCapitalFct, T_CP_JOURNAL_PERCENT_GUID, ref fctCPJournalPercent);
            InitDetail(fctCapitalFct, T_CP_RATE_SWITCH_GUID, ref fctCPRateSwitch);
            InitDetail(fctCapitalFct, T_CP_PLAN_CAPITAL_GUID, ref fctCPPlanCapital);

            #endregion ценные бумаги

            this.UsedFacts = new IFactTable[] { fctCreditIncome, fctGuarantIssued, fctCreditIssued, fctCapitalFct };

        }

        protected override void PumpFinalizing()
        {
            #region Кредиты полученные

            ClearDataSet(ref dsPlanAttractCI);
            ClearDataSet(ref dsPlanDebtCI);
            ClearDataSet(ref dsPlanServiceCI);
            ClearDataSet(ref dsCostAttractCI);
            ClearDataSet(ref dsFactAttractCI);
            ClearDataSet(ref dsFactDebtCI);
            ClearDataSet(ref dsFactPercentCI);
            ClearDataSet(ref dsFactPenaltyDebtCI);
            ClearDataSet(ref dsFactPenaltyPercentCI);
            ClearDataSet(ref dsCollateralCI);
            ClearDataSet(ref dsRateSwitchCI);
            ClearDataSet(ref dsJournalPercentCI);

            #endregion Кредиты полученные

            #region Кредиты предоставленные

            ClearDataSet(ref dsPlanDebtCO);
            ClearDataSet(ref dsPlanServiceCO);
            ClearDataSet(ref dsFactAttractCO);
            ClearDataSet(ref dsFactDebtCO);
            ClearDataSet(ref dsFactPercentCO);
            ClearDataSet(ref dsFactPenaltyDebtCO);
            ClearDataSet(ref dsFactPenaltyPercentCO);
            ClearDataSet(ref dsJournalPercentCO);

            #endregion Кредиты предоставленные

            #region Гарантии предоставленные

            ClearDataSet(ref dsFactAttractGrnt);
            ClearDataSet(ref dsFactDebtPrGrnt);
            ClearDataSet(ref dsFactPercentPrGrnt);
            ClearDataSet(ref dsFactPenaltyDebtPrGrnt);
            ClearDataSet(ref dsFactPenaltyPercentPrGrnt);
            ClearDataSet(ref dsCollateralGrnt);
            ClearDataSet(ref dsJournalPercentGrnt);

            #endregion Гарантии предоставленные

            #region ценные бумаги

            ClearDataSet(ref dsCPFactCapital);
            ClearDataSet(ref dsCPPlanDebt);
            ClearDataSet(ref dsCPFactDebt);
            ClearDataSet(ref dsCPFactService);
            ClearDataSet(ref dsCPCollateral);
            ClearDataSet(ref dsCPFactCost);
            ClearDataSet(ref dsCPJournalPercent);
            ClearDataSet(ref dsCPRateSwitch);
            ClearDataSet(ref dsCPPlanCapital);

            #endregion ценные бумаги

            ClearDataSet(ref dsCreditIncome);
            ClearDataSet(ref dsGuarantIssued);
            ClearDataSet(ref dsCreditIssued);
            ClearDataSet(ref dsCapitalFct);

            ClearDataSet(ref dsRegions);
            ClearDataSet(ref dsOrg);
            ClearDataSet(ref dsCapital);
            ClearDataSet(ref dsKindBorrow);
            ClearDataSet(ref dsOkv);
            ClearDataSet(ref dsExtensions);
            ClearDataSet(ref dsExchangeRate);
        }

        #endregion Работа с базой и кэшами

        #region работа с xml

        #region общие методы

        private string GetConfigParamName(Block block)
        {
            switch (block)
            {
                case Block.bCreditIncomes:
                    return "ucbCrInc";
                case Block.bCreditIssued:
                    return "ucbCrIss";
                case Block.bGrntIssued:
                    return "ucbGrntIss";
                case Block.bCapital:
                    return "ucbCapital";
                default:
                    return string.Empty;
            }
        }

        private bool ToPumpBlock(Block block)
        {
            string configParamName = GetConfigParamName(block);
            return (Convert.ToBoolean(GetParamValueByName(this.PumpRegistryElement.ProgramConfig, configParamName, "False")));
        }

        private object[] GetNodeValues(XmlNode node, object[] mapping)
        {
            for (int i = 0; i <= mapping.GetLength(0) - 1; i += 2)
                try
                {
                    if (mapping[i].ToString() == "SourceId")
                        continue;
                    if (mapping[i + 1].ToString() == string.Empty)
                    {
                        mapping[i + 1] = DBNull.Value;
                        continue;
                    }

                    if (node.Attributes.GetNamedItem(mapping[i + 1].ToString()) == null)
                        mapping[i + 1] = string.Empty;
                    else
                        mapping[i + 1] = node.Attributes[mapping[i + 1].ToString()].Value;

                    string fieldName = mapping[i].ToString().ToUpper();
                 //   if (fieldName.Contains("SUM") || fieldName.Contains("COST") ||
                  //      fieldName.Contains("PERCENT") || fieldName.Contains("RATE") || fieldName.Contains("OKATO"))
                   //     mapping[i + 1] = mapping[i + 1].ToString().PadLeft(1, '0');
                    if (!fieldName.Contains("DATE") && !fieldName.Contains("NAME") &&
                        !fieldName.Contains("NOTE") && !fieldName.Contains("PURPOSE"))
                        mapping[i + 1] = mapping[i + 1].ToString().PadLeft(1, '0');
                    if ((fieldName == "NAME") && (mapping[i + 1].ToString().Trim('0') == string.Empty))
                        mapping[i + 1] = constDefaultClsName;

                }
                catch (Exception exp)
                {
                    throw new Exception(string.Format("При получении значения поля '{0}' произошла ошибка: {1}",
                        mapping[i + 1].ToString(), exp.Message));
                }
            return mapping;
        }

        private object[] GetChildNodeValues(XmlNode parentNode, string xPath, object[] mapping)
        {
            XmlNode childNode = parentNode;
            if (xPath != string.Empty)
                childNode = parentNode.SelectSingleNode(xPath);
            if (childNode == null)
            {
                for (int i = 0; i <= mapping.GetLength(0) - 1; i += 2)
                    mapping[i + 1] = string.Empty;
                return mapping;
            }
            else
                return GetNodeValues(childNode, mapping);
        }

        private bool CheckSums(ref object[] mapping)
        {
            bool zeroSums = true;
            bool sumPresence = false;
            for (int i = 0; i <= mapping.GetLength(0) - 1; i += 2)
            {
                string fieldName = mapping[i].ToString().ToUpper();
                if (!fieldName.Contains("SUM") && (fieldName != "COST") && 
                    !fieldName.Contains("PERCENT") && !fieldName.Contains("PENALTY"))
                    continue;
                if (fieldName.Contains("REF"))
                    continue;
                sumPresence = true;
                if (mapping[i + 1].ToString().Trim('0') != string.Empty)
                    zeroSums = false;
                mapping[i + 1] = mapping[i + 1].ToString().PadLeft(1, '0').Replace('.', ',');
            }
            if (!sumPresence)
                return true;
            return !zeroSums;
        }

        private void CheckDate(ref object[] mapping)
        {
            for (int i = 0; i <= mapping.GetLength(0) - 1; i += 2)
            {
                if (!mapping[i].ToString().ToUpper().Contains("DATE"))
                    continue;
                if (mapping[i + 1].ToString().Trim('0') == string.Empty)
                    mapping[i + 1] = DBNull.Value;
            }
        }

        private object[] GetFactMapping(XmlNode clsNode, object[] constMapping, object[] mapping)
        {
            object[] rowMapping = constMapping;
            for (int i = 0; i <= mapping.GetLength(0) - 1; i += 2)
            {
                object[] auxMapping = GetChildNodeValues(clsNode, mapping[i].ToString(), (object[])mapping[i + 1]);
                CheckDate(ref auxMapping);
                rowMapping = (object[])CommonRoutines.ConcatArrays(rowMapping, auxMapping);
            }
            if (!CheckSums(ref rowMapping))
                return null;
            return rowMapping;
        }

        private DataRow FindDetailRow(DataTable dt, object[] mapping)
        {
            // поиск по полям, содержащие в названии date или sum + берутся детали определенного мастера (factId)
            string filter = string.Format("({0} = '{1}')", refMasterFieldName, factId);
            for (int i = 0; i <= mapping.GetLength(0) - 1; i += 2)
            {
                string fieldName = mapping[i].ToString();
                if ((!fieldName.ToUpper().Contains("SUM")) && (!fieldName.ToUpper().Contains("DATE")))
                    continue;
                if (fieldName.ToUpper().Contains("REF"))
                    continue;
                // при сравнении сумм - ноль и нулл равны
                string sumExtraCondition = string.Empty;
                if (fieldName.ToUpper().Contains("SUM"))
                    if (Convert.ToDecimal(mapping[i + 1]) == 0)
                        sumExtraCondition = string.Format(" or {0} is null", fieldName);
                filter += string.Format(" and ({0} = '{1}' {2})", fieldName, mapping[i + 1], sumExtraCondition);
            }
            DataRow[] rows = dt.Select(filter);
            if (rows.GetLength(0) == 0)
                return null;
            else
                return rows[0];
        }

        private void UpdateRow(DataRow row, object[] mapping)
        {
            for (int i = 0; i <= mapping.GetLength(0) - 1; i += 2)
                row[mapping[i].ToString()] = mapping[i + 1];
        }

        private void PumpFactData(XmlNode parentNode, string detailXPath, object[] constMapping, object[] mapping, IEntity fct, DataTable dt)
        {
            //WriteToTrace(string.Format("начало закачки {0}", fct.FullCaption), TraceMessageKind.Information);
            XmlNodeList detailNodes = parentNode.SelectNodes(detailXPath);
            int number = 1;
            foreach (XmlNode detailNode in detailNodes)
            {
                object[] detailMapping = (object[])mapping.Clone();
                for (int i = 1; i <= mapping.GetLength(0) - 1; i += 2)
                    detailMapping[i] = ((object[])mapping[i]).Clone();

                object[] rowMapping = GetFactMapping(detailNode, constMapping, detailMapping);

                if (rowMapping == null)
                    continue;

                // для ИФ.Итоги размещения - ряд вычислений
                if (fct == fctCPFactCapital)
                {
                    if (Convert.ToDecimal(rowMapping[13]) != 0)
                        rowMapping[1] = Convert.ToDecimal(rowMapping[15]) / Convert.ToDecimal(rowMapping[13]);
                    if (Convert.ToDecimal(rowMapping[17]) != 0)
                        rowMapping[3] = Convert.ToDecimal(rowMapping[15]) / Convert.ToDecimal(rowMapping[17]);
                }
                if (fct == fctCPCollateral)
                {
                    rowMapping[1] = FindCachedRow(cacheOrg, rowMapping[1].ToString(), -1);
                }
                // для курсовой разницы заполняем поле "ExchangeRate"
                if (fct == fctRateSwitchCI)
                {
                    rowMapping[13] = string.Format("{0} - {1}", detailNode.Attributes["end_rate"].Value, rowMapping[13]);
                }
                if (fct == fctCPRateSwitch)
                {
                    rowMapping[11] = string.Format("{0} - {1}", detailNode.Attributes["end_rate"].Value, rowMapping[11]);
                }
                // для журналов процентов заполняем дату родительской, если не заполнена
                if ((fct == fctJournalPercentCI) || (fct == fctJournalPercentCO) || (fct == fctJournalPercentGrnt))
                {
                    if (rowMapping[5].ToString() == string.Empty)
                        rowMapping[5] = startDate;
                }
                // для ряда деталей - заполняем поле "карточка учетного долга"
                if ((fct == fctFactAttractCI) || (fct == fctFactDebtCI) || (fct == fctFactPercentCI) ||
                    (fct == fctFactPenaltyDebtCI) || (fct == fctFactPenaltyPercentCI) || (fct == fctFactAttractGrnt) ||
                    (fct == fctFactDebtPrGrnt) || (fct == fctFactPercentPrGrnt) || (fct == fctFactPenaltyDebtPrGrnt) ||
                    (fct == fctFactPenaltyPercentPrGrnt) || (fct == fctFactPenaltyDebtPrGrnt))
                {
                    if (detailNode.Attributes.GetNamedItem("type_doc") == null)
                        nullNumPayOrder = true;
                    else
                    {
                        string numPayOrder = string.Format("{0} {1} от {2}",
                            detailNode.Attributes["type_doc"].Value, detailNode.Attributes["num_doc"].Value,
                            detailNode.Attributes["date_doc"].Value);
                        rowMapping = (object[])CommonRoutines.ConcatArrays(rowMapping, new object[] { "NumPayOrder", numPayOrder });
                    }
                }

                // если таблица пустая - грузим все записи, если нет - 
                // отсекаем записи с годом, не совпадающем с годом источника
                if (curMasterRecordCount != 0) 
                {
                    string date = string.Empty;
                    for (int i = 0; i <= rowMapping.GetLength(0) - 1; i += 2)
                        if (rowMapping[i].ToString().ToUpper().Contains("DATE"))
                        {
                            date = rowMapping[i + 1].ToString().Trim();
                            break;
                        }
                    if (date != string.Empty)
                        if (date.Split('.')[2] != this.DataSource.Year.ToString())
                            continue;
                }

                for (int i = 0; i <= rowMapping.GetLength(0) - 1; i += 2)
                {
                    switch (rowMapping[i].ToString().ToUpper())
                    {
                        case "REFOKV":
                            rowMapping[i + 1] = FindCachedRow(cacheOkv, rowMapping[i + 1].ToString(), -1);
                            // если у детали не заполнен - берем у договора (родительской записи)
                            if (rowMapping[i + 1].ToString() == "-1")
                                rowMapping[i + 1] = FindCachedRow(cacheOkv, dogovorOkv, -1);
                            break;
                        case "PAYMENT":
                        case "NUMACTION":
                            // порядковый номер
                            rowMapping[i + 1] = number;
                            break;
                    }
                }

                DataRow detailRow = null;
                if (!this.DeleteEarlierData)
                    detailRow = FindDetailRow(dt, rowMapping);
                if (detailRow == null)
                {
                    // закачка
                    PumpRow(dt, fct, rowMapping);
                    number++;
                }
                else
                {
                    // обновление
                    UpdateRow(detailRow, rowMapping);
                }
            }
            //WriteToTrace(string.Format("завершение закачки {0}", fct.FullCaption), TraceMessageKind.Information);
        }

        private int GetRefPeriodDebt(string periodDebt)
        {
            if (periodDebt.Trim('0') == string.Empty)
                return -1;
            else if (periodDebt == "МЕСЯЦ")
                return 1;
            else if (periodDebt == "КВАРТАЛ")
                return 2;
            else if (periodDebt == "ПОЛГОДА")
                return 3;
            else if (periodDebt == "ГОД")
                return 4;
            else
                return 5;
        }

        private int GetFactMaxId(IEntity fct)
        {
            // переделать на это
            // Select IDENT_CURRENT('{0}')", schemeObject.FullDBName для получения id добавленной записи
            string query = string.Format("select max(id) from {0}", fct.FullDBName);
            return Convert.ToInt32(this.DB.ExecQuery(query, QueryResultTypes.Scalar, new IDbDataParameter[] { }));
        }

        #region группировка записей

        private void GroupDetailData(DataTable dt, string[] refsCls, string[] sumFields)
        {
            Dictionary<string, DataRow> factCache = new Dictionary<string, DataRow>();
            try
            {
                foreach (DataRow row in dt.Rows)
                {
                    string key = GetGroupCacheKey(row, refsCls);
                    if (!factCache.ContainsKey(key))
                    {
                        factCache.Add(key, row);
                        continue;
                    }
                    DataRow cacheRow = factCache[key];
                    foreach (string sumField in sumFields)
                        if (row[sumField] != DBNull.Value)
                            cacheRow[sumField] = Convert.ToDecimal(cacheRow[sumField].ToString().PadLeft(1, '0')) +
                                                 Convert.ToDecimal(row[sumField]);
                    row.Delete();
                }
            }
            finally
            {
                factCache.Clear();
            }
            UpdateData();
        }

        #endregion группировка записей

        #endregion общие методы

        #region классификаторы

        private void PumpCommonCls(XmlDocument doc, string xPath, IClassifier cls, DataTable dt,
            Dictionary<string, int> cache, object[] mapping)
        {
            XmlNode clsNode = doc.SelectSingleNode(xPath);
            if (clsNode == null)
                return;
            XmlNodeList clsNodes = clsNode.ChildNodes;
            foreach (XmlNode node in clsNodes)
            {
                object[] rowMapping = GetNodeValues(node, (object[])mapping.Clone());
                PumpCachedRow(cache, dt, cls, rowMapping[1].ToString(), rowMapping);
            }
        }

        private void PumpOrg(XmlDocument doc)
        {
            XmlNode clsNode = doc.SelectSingleNode("xml/classifires/organizations");
            if (clsNode == null)
                return;
            XmlNodeList clsNodes = clsNode.ChildNodes;
            foreach (XmlNode node in clsNodes)
            {
                object[] rowMapping = GetNodeValues(node,
                    new object[] { "IDzap", "id", "Code", "inn", "Name", "name", "SourceId", clsSourceId, "INN20", "kpp", 
                        "FullName", "full_name", "OKATO", "okato", "OGRN", "ogrn", "OKPO", "okpo", "Account", "account" });
                rowMapping = (object[])CommonRoutines.ConcatArrays(rowMapping,
                    GetChildNodeValues(node, "okved", new object[] { "OKVED", "code", "OKVEDName", "name" }));
                rowMapping = (object[])CommonRoutines.ConcatArrays(rowMapping,
                    GetChildNodeValues(node, "okopf", new object[] { "OKOPF", "code", "OKOPFName", "name" }));
                rowMapping = (object[])CommonRoutines.ConcatArrays(rowMapping,
                    GetChildNodeValues(node, "okfs", new object[] { "OKFS", "code", "OKFSName", "name" }));
                rowMapping = (object[])CommonRoutines.ConcatArrays(rowMapping,
                    GetChildNodeValues(node, "adress", new object[] { "Phone", "phone", "Email", "e-mail" }));
                PumpCachedRow(cacheOrg, dsOrg.Tables[0], clsOrg, rowMapping[1].ToString(), rowMapping);
            }
        }

        private void PumpExchangeRate(XmlDocument doc)
        {
            XmlNode clsNode = doc.SelectSingleNode("xml/classifires/currencies");
            if (clsNode == null)
                return;
            XmlNodeList clsNodes = clsNode.ChildNodes;
            foreach (XmlNode node in clsNodes)
            {
                string code = node.Attributes["code"].Value.Trim();
                if (code == string.Empty)
                    continue;
                string date = node.Attributes["date"].Value.Trim();
                if (date == string.Empty)
                    continue;
                if (date.Split('.')[2] != this.DataSource.Year.ToString())
                    continue;
                int refOkv = FindCachedRow(cacheOkv, code, -1);
                PumpCachedRow(cacheExchangeRate, dsExchangeRate.Tables[0], clsExchangeRate, Convert.ToDateTime(date).ToString(), 
                    new object[] { "DateFixing", date, "Nominal", 1, "ExchangeRate", node.Attributes["rate"].Value.Trim(), 
                        "IsPrognozExchRate", 0, "CurrencyCode", code, "RefOKV", refOkv });
            }
        }

        private void PumpCls(XmlDocument doc)
        {
            PumpCommonCls(doc, "xml/classifires/regions", clsRegions, dsRegions.Tables[0], cacheRegions,
                new object[] { "IDzap", "id", "Code", "code", "Name", "name", "SourceId", clsSourceId });
            PumpCommonCls(doc, "xml/classifires/typecapitals", clsCapital, dsCapital.Tables[0], cacheCapital,
                new object[] { "SourceKey", "id", "Code", "code", "Name", "name" });
            PumpCommonCls(doc, "xml/classifires/borrows", clsKindBorrow, dsKindBorrow.Tables[0], cacheKindBorrow,
                new object[] { "SourceKey", "id", "Code", "code", "Name", "name" });
            PumpCommonCls(doc, "xml/classifires/extensions", clsExtensions, dsExtensions.Tables[0], cacheExtensions,
                new object[] { "SourceKey", "id", "Code", "code", "Name", "name" });
            PumpOrg(doc);
            PumpExchangeRate(doc);
            UpdateData();
        }

        #endregion классификаторы

        #region факты

        #region Кредиты полученные

        private void PumpCreditIncome(XmlDocument doc)
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "начало закачки блока 'Кредиты полученные'");
            block = Block.bCreditIncomes;
            nullNumPayOrder = false;
            curMasterRecordCount = dsCreditIncome.Tables[0].Rows.Count;
            XmlNodeList clsNodes = doc.SelectNodes("xml/data/dogovor[@type_doc=\"0\"]");
            foreach (XmlNode clsNode in clsNodes)
            {
                string idBorrow = clsNode.Attributes["id_borrow"].Value.Trim().Trim('0');
                if (idBorrow == string.Empty)
                    continue;

                object[] auxMapping = null;

                auxMapping = GetChildNodeValues(clsNode, "capital", new object[] { "code", "code" });
                if (auxMapping[1].ToString().Trim().Trim('0') != string.Empty)
                    continue;

                object[] rowMapping = GetNodeValues(clsNode,
                    new object[] { "IDDoc", "id_doc", "Num", "number", "ContractDate", "date", "Purpose", "function", 
                        "EndDate", "date_end", "RenewalDate", "date_renewal", "IDGeneralDoc", "id_general_doc", 
                        "Note", "note", "ChargeFirstDay", "charge_first_day" });

                int idDoc = Convert.ToInt32(rowMapping[1]);

                try
                {

                    // ChargeFirstDay - boolean
                    rowMapping[17] = Convert.ToInt32(rowMapping[17]);

                    CheckDate(ref rowMapping);

                    startDate = rowMapping[5];

                    if (startDate == DBNull.Value)
                    {
                        string message = string.Format("У договора № {0} ({1}) не заполнено поле «Дата заключения», закачан не будет.",
                            rowMapping[3].ToString(), rowMapping[1].ToString());
                        WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, message);
                        continue;
                    }

                    auxMapping = GetNodeValues(clsNode, new object[] { "PeriodDebt", "period_debt", "PeriodRate", "period_charge_rate" });
                    int refPeriodDebt = GetRefPeriodDebt(auxMapping[1].ToString().ToUpper());
                    int refPeriodRate = GetRefPeriodDebt(auxMapping[3].ToString().ToUpper());

                    auxMapping = GetNodeValues(clsNode, new object[] { "RefSStatusPlan", "status_doс" });
                    int refStatusDoc = 0;
                    if (auxMapping[1].ToString().Trim('0') == "3")
                        refStatusDoc = 4;

                    // EndDate
                    if ((rowMapping[9] == DBNull.Value) && (refStatusDoc != 4))
                    {
                        string message = string.Format("У договора № {0} от {1}({2}) не заполнено поле «Дата погашения кредита».",
                            rowMapping[3].ToString(), rowMapping[5].ToString(), rowMapping[1].ToString());
                        WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, message);
                        auxMapping = GetNodeValues(clsNode, new object[] { "EndDate", "date_end_fact" });
                        CheckDate(ref auxMapping);
                        rowMapping[9] = auxMapping[1];
                    }

                    // RenewalDate
                    if (rowMapping[11] == DBNull.Value)
                    {
                        rowMapping[11] = rowMapping[9];
                    }

                    auxMapping = new object[] { "CurrencyBorrow", 0, "TypeDoc", -1, "RefSCreditPeriod", -1, 
                        "RefSExtension", -1, "RefSRepayDebt", -1, "RefSRepayPercent", 1, "RefSStatusPlan", refStatusDoc, 
                        "RefSTypeCredit", -1, "RefTypeContract", -1, "RefPeriodDebt", refPeriodDebt, "RefPeriodRate", refPeriodRate };
                    rowMapping = (object[])CommonRoutines.ConcatArrays(rowMapping, auxMapping);

                    auxMapping = GetChildNodeValues(clsNode, "sum",
                        new object[] { "Sum", "summa", "CurrencySum", "summa_val", "RefOKV", "okv" });
                    auxMapping[1] = auxMapping[1].ToString().Replace('.', ',').PadLeft(1, '0');
                    auxMapping[3] = auxMapping[3].ToString().Replace('.', ',').PadLeft(1, '0');
                    dogovorOkv = auxMapping[5].ToString();
                    auxMapping[5] = FindCachedRow(cacheOkv, dogovorOkv, -1);

                    if (refStatusDoc != 4)
                    {
                        if ((Convert.ToDecimal(auxMapping[1]) == 0) && (Convert.ToDecimal(auxMapping[3]) == 0))
                        {
                            string message = string.Format("У договора № {0} от {1}({2}) не заполнена Сумма по договору.",
                                rowMapping[3].ToString(), rowMapping[5].ToString(), rowMapping[1].ToString());
                            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, message);
                        }
                    }

                    rowMapping = (object[])CommonRoutines.ConcatArrays(rowMapping, auxMapping);

                    auxMapping = GetChildNodeValues(clsNode, "pog_plan/procent/doc",
                        new object[] { "CreditPercent", "ratio" });
                    if (auxMapping[1].ToString().Trim('0') == string.Empty)
                    {
                        auxMapping = GetChildNodeValues(clsNode, "journal_percent/record[@type_summa=\"02\"]",
                            new object[] { "CreditPercent", "percent" });
                        if (auxMapping[1].ToString().Trim('0') == string.Empty)
                            auxMapping = GetChildNodeValues(clsNode, "share_rate_cbr",
                                new object[] { "CreditPercent", "percent_rate" });
                    }
                    rowMapping = (object[])CommonRoutines.ConcatArrays(rowMapping, auxMapping);

                    auxMapping = GetChildNodeValues(clsNode, "privl_fact/doc",
                        new object[] { "StartDate", "date" });
                    CheckDate(ref auxMapping);

                    if (auxMapping[1] == DBNull.Value)
                        auxMapping[1] = startDate;

                    rowMapping = (object[])CommonRoutines.ConcatArrays(rowMapping, auxMapping);

                    auxMapping = GetChildNodeValues(clsNode, "share_rate_cbr",
                        new object[] { "PercentRate", "percent_rate", "PenaltyDebtRate", "penalty_debt_rate", "PenaltyPercentRate", "penalty_percent_rate" });
                    decimal percentRate = Convert.ToDecimal(auxMapping[1]);
                    if (percentRate > 1)
                        auxMapping[1] = percentRate / 100;
                    if (auxMapping[1].ToString() == "0")
                        auxMapping[1] = DBNull.Value;
                    if (auxMapping[3].ToString() == "0")
                        auxMapping[3] = DBNull.Value;
                    if (auxMapping[5].ToString() == "0")
                        auxMapping[5] = DBNull.Value;


                    rowMapping = (object[])CommonRoutines.ConcatArrays(rowMapping, auxMapping);

                    auxMapping = GetNodeValues(clsNode,
                        new object[] { "RefKindBorrow", "id_borrow", "RefOrganizations", "id_organization" });
                    auxMapping[1] = FindCachedRow(cacheKindBorrow, auxMapping[1].ToString(), -1);
                    auxMapping[3] = FindCachedRow(cacheOrg, auxMapping[3].ToString(), -1);
                    rowMapping = (object[])CommonRoutines.ConcatArrays(rowMapping, auxMapping);

                    if (clsNode.Attributes.GetNamedItem("reg_num_doc") != null)
                    {
                        auxMapping = GetNodeValues(clsNode, new object[] { "RegDate", "date", "RegNum", "reg_num_doc" });
                        if (auxMapping[3].ToString() == "0")
                            auxMapping[3] = string.Empty;
                        rowMapping = (object[])CommonRoutines.ConcatArrays(rowMapping, auxMapping);
                    }

                    isUpdate = false;
                    if (cacheCreditIncome.ContainsKey(idDoc))
                    {
                        // обновляем
                        DataRow creditRow = cacheCreditIncome[idDoc];
                        creditRow["RenewalDate"] = rowMapping[11];
                        creditRow["RefSStatusPlan"] = refStatusDoc;
                        creditRow["EndDate"] = rowMapping[9];
                        factId = Convert.ToInt32(creditRow["Id"]);
                        isUpdate = true;
                    }
                    else
                    {
                        // закачиваем
                        rowMapping = (object[])CommonRoutines.ConcatArrays(rowMapping, new object[] { "RefVariant", refVariant });
                        DataRow factRow = PumpRow(fctCreditIncome, dsCreditIncome.Tables[0], rowMapping, false);
                        UpdateDataSet(daCreditIncome, dsCreditIncome, fctCreditIncome);
                        factId = GetFactMaxId(fctCreditIncome);
                        factRow["Id"] = factId;
                        cacheCreditIncome.Add(idDoc, factRow);
                    }

                    refMasterFieldName = "RefCreditInc";

                    // ИФ.Факт привлечения
                    PumpFactData(clsNode, "privl_fact/doc[@type_summa=\"01\"]", new object[] { "RefKif", -1, "RefCreditInc", factId },
                        new object[] { string.Empty, new object[] { "FactDate", "date" }, 
                                   "sum", new object[] { "Sum", "summa", "CurrencySum", "summa_val", "ExchangeRate", "rate", "RefOKV", "okv" } },
                        fctFactAttractCI, dsFactAttractCI.Tables[0]);
                    // ИФ.Факт погашения основного долга
                    PumpFactData(clsNode, "pog_fact/doc[@type_summa=\"01\"]", new object[] { "RefKIF", -1, "RefCreditInc", factId, "Payment", 1 },
                        new object[] { string.Empty, new object[] { "FactDate", "date" }, 
                                   "sum", new object[] { "Sum", "summa", "CurrencySum", "summa_val", "ExchangeRate", "rate", "RefOKV", "okv" } },
                        fctFactDebtCI, dsFactDebtCI.Tables[0]);
                    // ИФ.Факт погашения процентов
                    PumpFactData(clsNode, "pog_fact/doc[@type_summa=\"02\"]", new object[] { "RefR", -1, "RefEKR", -1, "RefCreditInc", factId, "Payment", 1 },
                        new object[] { string.Empty, new object[] { "FactDate", "date" }, 
                                   "sum", new object[] { "Sum", "summa", "CurrencySum", "summa_val", "ExchangeRate", "rate", "RefOKV", "okv" } },
                        fctFactPercentCI, dsFactPercentCI.Tables[0]);
                    PumpFactData(clsNode, "privl_fact/doc[@type_summa=\"02\"]", new object[] { "RefR", -1, "RefEKR", -1, "RefCreditInc", factId, "Payment", 1 },
                        new object[] { string.Empty, new object[] { "FactDate", "date" }, 
                                   "sum", new object[] { "ChargeSum", "summa", "ExchangeRate", "rate", "RefOKV", "okv" } },
                        fctFactPercentCI, dsFactPercentCI.Tables[0]);
                    // ИФ.Факт погашения пени по основному долгу
                    PumpFactData(clsNode, "pog_fact/doc[@type_summa=\"03\"]",
                        new object[] { "RefR", -1, "RefEKR", -1, "RefCreditInc", factId, "Payment", 1 },
                        new object[] { string.Empty, new object[] { "FactDate", "date" }, 
                                   "sum", new object[] { "Sum", "summa", "CurrencySum", "summa_val", "ExchangeRate", "rate", "RefOKV", "okv" } },
                        fctFactPenaltyDebtCI, dsFactPenaltyDebtCI.Tables[0]);
                    // ИФ.Факт погашения пени по процентам
                    PumpFactData(clsNode, "pog_fact/doc[@type_summa=\"04\"]",
                        new object[] { "RefR", -1, "RefEKR", -1, "RefCreditInc", factId, "Payment", 1 },
                        new object[] { string.Empty, new object[] { "FactDate", "date" }, 
                                   "sum", new object[] { "Sum", "summa", "CurrencySum", "summa_val", "ExchangeRate", "rate", "RefOKV", "okv" } },
                        fctFactPenaltyPercentCI, dsFactPenaltyPercentCI.Tables[0]);
                    // ИФ.Курсовая разница
                    PumpFactData(clsNode, "difference_rate/doc", new object[] { "RefTypeSum", 1, "RefCreditInc", factId },
                        new object[] { string.Empty, new object[] { "DateCharge", "date", "StartDate", "start_date", 
                                   "EndDate", "end_date", "Sum", "summa", "ExchangeRate", "start_rate" } },
                        fctRateSwitchCI, dsRateSwitchCI.Tables[0]);

                    // ИФ.План привлечения
                    if (!isUpdate)
                        PumpFactData(clsNode, "privl_plan/doc", new object[] { "RefKif", -1, "RefCreditInc", factId },
                            new object[] { string.Empty, new object[] { "StartDate", "date_start", "EndDate", "date_end" },
                                   "sum", new object[] { "Sum", "summa", "CurrencySum", "summa_val" }, },
                            fctPlanAttractCI, dsPlanAttractCI.Tables[0]);
                    // ИФ.План погашения основного долга
                    if (!isUpdate)
                        PumpFactData(clsNode, "pog_plan/osn_dolg/doc", new object[] { "RefKif", -1, "RefCreditInc", factId, "Payment", 1 },
                            new object[] { string.Empty, new object[] { "StartDate", "date_start", "EndDate", "date_end" }, 
                                   "sum", new object[] { "Sum", "summa", "CurrencySum", "summa_val", "RefOKV", "okv"/*, "ExchangeRate", "rate" */} },
                            fctPlanDebtCI, dsPlanDebtCI.Tables[0]);
                    // ИФ.План обслуживания долга
                    if (!isUpdate)
                        PumpFactData(clsNode, "pog_plan/procent/doc", new object[] { "RefEKR", -1, "RefR", -1, "RefCreditInc", factId, "Payment", 1 },
                            new object[] { string.Empty, new object[] { "StartDate", "date_start", "EndDate", "date_end", 
                                                                    "CreditPercent", "ratio", "DayCount", "number_day"  },
                                       "sum", new object[] { "Sum", "summa", "CurrencySum", "summa_val", "ExchangeRate", "rate", "RefOKV", "okv" } },
                        fctPlanServiceCI, dsPlanServiceCI.Tables[0]);
                    // ИФ.Дополнительные расходы
                    PumpFactData(clsNode, "cost_attract/dос", new object[] { "RefEKR", -1, "RefR", -1, "RefCreditInc", factId },
                        new object[] { string.Empty, new object[] { "CostDate", "date", "Actions", "purpose", "Sum", "summa" } },
                        fctCostAttractCI, dsCostAttractCI.Tables[0]);
                    // ИФ.Обеспечение
                    PumpFactData(clsNode, "collaterals/collateral", new object[] { "RefSCollateral", 0, "RefOrg", -1, "RefCreditInc", factId },
                        new object[] { string.Empty, new object[] { "Name", "collateral_name", "Cost", "collateral_cost", 
                                   "Sum", "collateral_summa" } },
                        fctCollateralCI, dsCollateralCI.Tables[0]);
                    PumpFactData(clsNode, "collateral[string-length(@name)>0]", new object[] { "Sum", "1", "RefSCollateral", -1, "RefOrg", -1, "RefCreditInc", factId },
                        new object[] { string.Empty, new object[] { "Name", "name" } },
                        fctCollateralCI, dsCollateralCI.Tables[0]);
                    // ИФ.Журнал процентов
                    PumpFactData(clsNode, "journal_percent/record[@type_summa=\"02\"]",
                        new object[] { "RefCreditInc", factId },
                        new object[] { string.Empty, new object[] { "CreditPercent", "percent", "ChargeDate", "date" } },
                        fctJournalPercentCI, dsJournalPercentCI.Tables[0]);
                }
                catch (Exception exc)
                {
                    throw new Exception(string.Format("ошибка при обработке договора: {0}", idDoc), exc);
                }
            }

            foreach (DataRow row in dsCreditIncome.Tables[0].Rows)
            {
                if (Convert.ToInt32(row["RefPeriodDebt"]) == -1)
                    row["RefSRepayDebt"] = 0;
                else
                    row["RefSRepayDebt"] = 5;
            }

            UpdateData();

            GroupDetailData(dsFactPercentCI.Tables[0], new string[] { "RefCreditInc", "FactDate" }, new string[] { "Sum", "CurrencySum", "ChargeSum" });

            if (nullNumPayOrder)
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation,
                    "Из АС Смета не выгружены номера платежных поручений. " + 
                    "Необходимо обратиться к разработчику АС Смета за соответствующим обновлением");

            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation,
                string.Format("завершение закачки блока 'Кредиты полученные'. Добавлено записей: {0}.",
                dsCreditIncome.Tables[0].Rows.Count - creditIncomeRowsCount));
        }

        #endregion Кредиты полученные

        #region Кредиты предоставленные

        private void PumpCreditIssued(XmlDocument doc)
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "начало закачки блока 'Кредиты предоставленные'");
            block = Block.bCreditIssued;
            curMasterRecordCount = dsCreditIssued.Tables[0].Rows.Count;
            XmlNodeList clsNodes = doc.SelectNodes(
                "xml/data/dogovor[(@type_doc=\"1\") or (@account=\"2.07.01\") or (@account=\"2.07.02\") or starts-with(@account, '207.1')]");
            object[] auxMapping = null;
            foreach (XmlNode clsNode in clsNodes)
            {
                object[] rowMapping = GetNodeValues(clsNode,
                    new object[] { "IDDoc", "id_doc", "Num", "number", "DocDate", "date", "Purpose", "function", 
                        "EndDate", "date_end", "RenewalDate", "date_renewal", "IDGeneralDoc", "id_general_doc", 
                        "Note", "note", "ChargeFirstDay", "charge_first_day" });
                CheckDate(ref rowMapping);

                startDate = rowMapping[5];

                if (startDate == DBNull.Value)
                {
                    string message = string.Format("У договора № {0} ({1}) не заполнено поле «Дата заключения», закачан не будет.",
                        rowMapping[3].ToString(), rowMapping[1].ToString());
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, message);
                    continue;
                }

                // ChargeFirstDay - boolean
                rowMapping[17] = Convert.ToInt32(rowMapping[17]);

                int idDoc = Convert.ToInt32(rowMapping[1]);

                try
                {

                    // EndDate
                    if (rowMapping[9] == DBNull.Value)
                    {
                        auxMapping = GetNodeValues(clsNode, new object[] { "EndDate", "date_end_fact" });
                        CheckDate(ref auxMapping);
                        rowMapping[9] = auxMapping[1];
                    }

                    // RenewalDate
                    if (rowMapping[11] == DBNull.Value)
                    {
                        rowMapping[11] = rowMapping[9];
                    }

                    auxMapping = GetNodeValues(clsNode, new object[] { "Account", "account", "RefSStatusPlan", "status_doс" });
                    int refSTypeCredit = 3;
                    if (auxMapping[1].ToString().EndsWith("01"))
                        refSTypeCredit = 4;
                    int refStatusDoc = 0;
                    if (auxMapping[3].ToString().Trim('0') == "3")
                        refStatusDoc = 4;

                    auxMapping = GetNodeValues(clsNode, new object[] { "PeriodDebt", "period_debt", "PeriodRate", "period_charge_rate" });
                    int refPeriodDebt = GetRefPeriodDebt(auxMapping[1].ToString().ToUpper());
                    int refPeriodRate = GetRefPeriodDebt(auxMapping[3].ToString().ToUpper());

                    auxMapping = new object[] { "CurrencyBorrow", 0, "RefSCreditPeriod", -1, 
                        "RefSExtension", -1, "RefSRepayDebt", -1, "RefSRepayPercent", 2, "RefSStatusPlan", refStatusDoc, 
                        "RefSTypeCredit", refSTypeCredit, "RefTypeContract", -1, "RefPeriodDebt", refPeriodDebt, "RefPeriodRate", refPeriodRate };
                    rowMapping = (object[])CommonRoutines.ConcatArrays(rowMapping, auxMapping);

                    auxMapping = GetChildNodeValues(clsNode, "sum",
                        new object[] { "Sum", "summa", "RefOKV", "okv" });
                    auxMapping[1] = auxMapping[1].ToString().Replace('.', ',').PadLeft(1, '0');
                    dogovorOkv = auxMapping[3].ToString();
                    auxMapping[3] = FindCachedRow(cacheOkv, dogovorOkv, -1);

                    rowMapping = (object[])CommonRoutines.ConcatArrays(rowMapping, auxMapping);

                    auxMapping = GetChildNodeValues(clsNode, "pog_plan/procent/doc",
                        new object[] { "CreditPercent", "ratio" });
                    rowMapping = (object[])CommonRoutines.ConcatArrays(rowMapping, auxMapping);

                    auxMapping = GetChildNodeValues(clsNode, "privl_fact/doc",
                        new object[] { "StartDate", "date" });
                    if (auxMapping.GetLength(0) == 0)
                        auxMapping = new object[] { "StartDate", "0" };
                    if (auxMapping[1].ToString().TrimStart('0') == string.Empty)
                        auxMapping[1] = GetNodeValues(clsNode, new object[] { "startDate", "date_charge_rate" })[1];
                    if (auxMapping[1].ToString().TrimStart('0') == string.Empty)
                        auxMapping[1] = GetNodeValues(clsNode, new object[] { "startDate", "date" })[1];
                    CheckDate(ref auxMapping);

                    rowMapping = (object[])CommonRoutines.ConcatArrays(rowMapping, auxMapping);

                    auxMapping = GetChildNodeValues(clsNode, "share_rate_cbr",
                        new object[] { "PercentRate", "percent_rate", "PenaltyDebtRate", "penalty_debt_rate", "PenaltyPercentRate", "penalty_percent_rate" });
                    decimal percentRate = Convert.ToDecimal(auxMapping[1]);
                    if (percentRate > 1)
                        auxMapping[1] = percentRate / 100;
                    if (auxMapping[1].ToString() == "0")
                        auxMapping[1] = DBNull.Value;
                    if (auxMapping[3].ToString() == "0")
                        auxMapping[3] = DBNull.Value;
                    if (auxMapping[5].ToString() == "0")
                        auxMapping[5] = DBNull.Value;
                    rowMapping = (object[])CommonRoutines.ConcatArrays(rowMapping, auxMapping);

                    auxMapping = GetNodeValues(clsNode,
                        new object[] { "RefOrganizations", "id_organization", "RefExtensions", "id_extension" });
                    auxMapping[1] = FindCachedRow(cacheOrg, auxMapping[1].ToString(), -1);
                    auxMapping[3] = FindCachedRow(cacheExtensions, auxMapping[3].ToString(), -1);
                    rowMapping = (object[])CommonRoutines.ConcatArrays(rowMapping, auxMapping);

                    isUpdate = false;
                    if (cacheCreditIssued.ContainsKey(idDoc))
                    {
                        // обновляем
                        DataRow creditRow = cacheCreditIssued[idDoc];
                        creditRow["RenewalDate"] = rowMapping[11];
                        creditRow["RefSStatusPlan"] = refStatusDoc;
                        creditRow["EndDate"] = rowMapping[9];
                        factId = Convert.ToInt32(creditRow["Id"]);
                        isUpdate = true;
                    }
                    else
                    {
                        // закачиваем
                        rowMapping = (object[])CommonRoutines.ConcatArrays(rowMapping, new object[] { "RefVariant", refVariant });
                        DataRow factRow = PumpRow(fctCreditIssued, dsCreditIssued.Tables[0], rowMapping, false);
                        UpdateDataSet(daCreditIssued, dsCreditIssued, fctCreditIssued);
                        factId = GetFactMaxId(fctCreditIssued);
                        factRow["Id"] = factId;
                        cacheCreditIssued.Add(idDoc, factRow);
                    }

                    refMasterFieldName = "RefCreditInc";

                    // ИФ.Факт предоставления
                    PumpFactData(clsNode, "privl_fact/doc[@type_summa=\"01\"]", new object[] { "RefOKV", -1, "RefKif", -1, "RefCreditInc", factId },
                        new object[] { string.Empty, new object[] { "FactDate", "date" }, 
                                   "sum", new object[] { "Sum", "summa" } },
                        fctFactAttractCO, dsFactAttractCO.Tables[0]);
                    // ИФ.Факт погашения основного долга
                    PumpFactData(clsNode, "pog_fact/doc[@type_summa=\"01\"]",
                        new object[] { "RefOKV", -1, "RefKif", -1, "RefCreditInc", factId, "Payment", 1 },
                        new object[] { string.Empty, new object[] { "FactDate", "date" }, 
                                   "sum", new object[] { "Sum", "summa" } },
                        fctFactDebtCO, dsFactDebtCO.Tables[0]);
                    // ИФ.Факт погашения процентов
                    PumpFactData(clsNode, "pog_fact/doc[@type_summa=\"02\"]",
                        new object[] { "RefOKV", -1, "RefKD", -1, "RefCreditInc", factId, "Payment", 1 },
                        new object[] { string.Empty, new object[] { "FactDate", "date" }, 
                                   "sum", new object[] { "Sum", "summa" } },
                        fctFactPercentCO, dsFactPercentCO.Tables[0]);
                    // ИФ.Факт погашения пени по основному долгу 
                    PumpFactData(clsNode, "pog_fact/doc[@type_summa=\"03\"]",
                        new object[] { "RefOKV", -1, "RefKD", -1, "RefCreditInc", factId, "Payment", 1 },
                        new object[] { string.Empty, new object[] { "FactDate", "date" }, 
                                   "sum", new object[] { "Sum", "summa" } },
                        fctFactPenaltyDebtCO, dsFactPenaltyDebtCO.Tables[0]);
                    // ИФ.Факт погашения пени по процентам 
                    PumpFactData(clsNode, "pog_fact/doc[@type_summa=\"04\"]",
                        new object[] { "RefOKV", -1, "RefKD", -1, "RefCreditInc", factId, "Payment", 1 },
                        new object[] { string.Empty, new object[] { "FactDate", "date" }, 
                                   "sum", new object[] { "Sum", "summa" } },
                        fctFactPenaltyPercentCO, dsFactPenaltyPercentCO.Tables[0]);

                    // ИФ.План погашения основного долга
                    if (!isUpdate)
                        PumpFactData(clsNode, "pog_plan/osn_dolg/doc", new object[] { "RefOKV", -1, "RefKif", -1, "RefCreditInc", factId, "Payment", 1 },
                            new object[] { string.Empty, new object[] { "StartDate", "date_start", "EndDate", "date_end" }, 
                                       "sum", new object[] { "Sum", "summa" } },
                            fctPlanDebtCO, dsPlanDebtCO.Tables[0]);
                    // ИФ.План обслуживания долга
                    if (!isUpdate)
                        PumpFactData(clsNode, "pog_plan/procent/doc", new object[] { "RefOKV", -1, "RefKd", -1, "RefCreditInc", factId, "Payment", 1, "CreditPercent", 0 },
                            new object[] { string.Empty, new object[] { "StartDate", "date_start", "EndDate", "date_end" }, 
                                       "sum", new object[] { "Sum", "summa" } },
                            fctPlanServiceCO, dsPlanServiceCO.Tables[0]);
                    // ИФ.Журнал процентов 
                    PumpFactData(clsNode, "journal_percent/record[@type_summa=\"02\"]",
                        new object[] { "RefCreditInc", factId },
                        new object[] { string.Empty, new object[] { "CreditPercent", "percent", "ChargeDate", "date" }, },
                        fctJournalPercentCO, dsJournalPercentCO.Tables[0]);

                }
                catch (Exception exc)
                {
                    throw new Exception(string.Format("ошибка при обработке договора: {0}", idDoc), exc);
                }
            }

            foreach (DataRow row in dsCreditIssued.Tables[0].Rows)
            {
                if (Convert.ToInt32(row["RefPeriodDebt"]) == -1)
                    row["RefSRepayDebt"] = 0;
                else
                    row["RefSRepayDebt"] = 5;
            }

            UpdateData();

            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation,
                string.Format("завершение закачки блока 'Кредиты предоставленные'. Добавлено записей: {0}.",
                dsCreditIssued.Tables[0].Rows.Count - creditIssuedRowsCount));
        }

        #endregion Кредиты предоставленные

        #region Гарантии предоставленные

        private void SetExchangeRate(DataRow row)
        {
            // курс валюты
            decimal sum = Convert.ToDecimal(row["Sum"].ToString().PadLeft(1, '0'));
            decimal currencySum = Convert.ToDecimal(row["CurrencySum"].ToString().PadLeft(1, '0'));
            if (sum != 0)
                row["ExchangeRate"] = currencySum / sum;
        }

        private void PumpGuarantIssued(XmlDocument doc)
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "начало закачки блока 'Гарантии предоставленные'");
            block = Block.bGrntIssued;
            nullNumPayOrder = false;
            curMasterRecordCount = dsGuarantIssued.Tables[0].Rows.Count;
            XmlNodeList clsNodes = doc.SelectNodes("xml/data/dogovor[@type_doc=\"2\"]");
            foreach (XmlNode clsNode in clsNodes)
            {
                string account = clsNode.Attributes["account"].Value.Trim();
                if ((account == "2.07.01") || (account == "2.07.02"))
                    continue;

                if (account.StartsWith("207.1"))
                    continue;

                object[] auxMapping = null;

                object[] rowMapping = GetNodeValues(clsNode,
                    new object[] { "IDDoc", "id_doc", "Num", "number", "StartDate", "date", "Purpose", "function", 
                        "EndDate", "date_end", "RenewalDate", "date_renewal", "Regress", "regress", "Note", "note" });
                CheckDate(ref rowMapping);

                // Regress - boolean
                rowMapping[13] = Convert.ToInt32(rowMapping[13]);

                // EndDate
                if (rowMapping[9] == DBNull.Value)
                {
                    auxMapping = GetNodeValues(clsNode, new object[] { "EndDate", "date_end_fact" });
                    CheckDate(ref auxMapping);
                    rowMapping[9] = auxMapping[1];
                }

                // RenewalDate
                if (rowMapping[11] == DBNull.Value)
                {
                    rowMapping[11] = rowMapping[9];
                }

                startDate = rowMapping[5];

                int idDoc = Convert.ToInt32(rowMapping[1]);

                try
                {

                    auxMapping = GetNodeValues(clsNode, new object[] { "RefSStatusPlan", "status_doс" });
                    int refStatusDoc = 0;
                    if (auxMapping[1].ToString().Trim('0') == "3")
                        refStatusDoc = 4;

                    auxMapping = new object[] { "RefSStatusPlan", refStatusDoc, "RefTypeContract", -1 };
                    rowMapping = (object[])CommonRoutines.ConcatArrays(rowMapping, auxMapping);

                    auxMapping = GetChildNodeValues(clsNode, "sum",
                        new object[] { "Sum", "summa", "CurrencySum", "summa_val", "RefOKV", "okv" });
                    auxMapping[1] = auxMapping[1].ToString().Replace('.', ',').PadLeft(1, '0');
                    auxMapping[3] = auxMapping[3].ToString().Replace('.', ',').PadLeft(1, '0');
                    dogovorOkv = auxMapping[5].ToString();
                    auxMapping[5] = FindCachedRow(cacheOkv, dogovorOkv, -1);
                    rowMapping = (object[])CommonRoutines.ConcatArrays(rowMapping, auxMapping);

                    auxMapping = GetChildNodeValues(clsNode, "privl_fact/doc",
                        new object[] { "DateDoc", "date" });
                    CheckDate(ref auxMapping);
                    if (auxMapping[1] == DBNull.Value)
                    {
                        auxMapping = GetNodeValues(clsNode, new object[] { "DateDoc", "date" });
                        CheckDate(ref auxMapping);
                    }
                    rowMapping = (object[])CommonRoutines.ConcatArrays(rowMapping, auxMapping);

                    auxMapping = GetNodeValues(clsNode,
                        new object[] { "RefOrganizations", "id_principal", "RefOrganizationsPlan3", "id_organization" });
                    auxMapping[1] = FindCachedRow(cacheOrg, auxMapping[1].ToString(), -1);
                    auxMapping[3] = FindCachedRow(cacheOrg, auxMapping[3].ToString(), -1);
                    rowMapping = (object[])CommonRoutines.ConcatArrays(rowMapping, auxMapping);

                    if (clsNode.Attributes.GetNamedItem("reg_num_doc") != null)
                    {
                        auxMapping = GetNodeValues(clsNode, new object[] { "RegDate", "date", "RegNum", "reg_num_doc" });
                        rowMapping = (object[])CommonRoutines.ConcatArrays(rowMapping, auxMapping);
                    }

                    refMasterFieldName = "RefGrnt";
                    isUpdate = false;
                    if (cacheGuarantIssued.ContainsKey(idDoc))
                    {
                        // обновляем
                        DataRow grntRow = cacheGuarantIssued[idDoc];
                        grntRow["RenewalDate"] = rowMapping[11];
                        grntRow["RefSStatusPlan"] = refStatusDoc;
                        grntRow["EndDate"] = rowMapping[9];
                        factId = Convert.ToInt32(grntRow["Id"]);
                        isUpdate = true;
                    }
                    else
                    {
                        // закачиваем
                        rowMapping = (object[])CommonRoutines.ConcatArrays(rowMapping, new object[] { "RefVariant", refVariant });
                        DataRow factRow = PumpRow(fctGuarantIssued, dsGuarantIssued.Tables[0], rowMapping, false);
                        UpdateDataSet(daGuarantIssued, dsGuarantIssued, fctGuarantIssued);
                        factId = GetFactMaxId(fctGuarantIssued);
                        factRow["Id"] = factId;
                        cacheGuarantIssued.Add(idDoc, factRow);
                    }

                    // ИФ.Факт исполнения обязательств гарантом
                    PumpFactData(clsNode, "pog_fact/doc[@guarantor!=0]",
                        new object[] { "RefOKV", -1, "RefKif", -1, "RefGrnt", factId, "Payment", 1 },
                        new object[] { string.Empty, new object[] { "FactDate", "date", "RefTypSum", "type_summa" }, 
                                   "sum", new object[] { "Sum", "summa", "CurrencySum", "summa_val" } },
                        fctFactAttractGrnt, dsFactAttractGrnt.Tables[0]);
                    // ИФ.Факт погашения основного долга
                    PumpFactData(clsNode, "pog_fact/doc[@guarantor=0 and @type_summa=\"01\"]",
                        new object[] { "RefOKV", -1, "RefKif", -1, "RefGrnt", factId, "Payment", 1 },
                        new object[] { string.Empty, new object[] { "FactDate", "date" }, 
                                   "sum", new object[] { "Sum", "summa", "CurrencySum", "summa_val" } },
                        fctFactDebtPrGrnt, dsFactDebtPrGrnt.Tables[0]);
                    // ИФ.Факт погашения процентов
                    PumpFactData(clsNode, "pog_fact/doc[@guarantor=0 and @type_summa=\"02\"]",
                        new object[] { "RefOKV", -1, "RefKif", -1, "RefGrnt", factId, "Payment", 1 },
                        new object[] { string.Empty, new object[] { "FactDate", "date" }, 
                                   "sum", new object[] { "Sum", "summa", "CurrencySum", "summa_val" } },
                        fctFactPercentPrGrnt, dsFactPercentPrGrnt.Tables[0]);

                    // ИФ.Факт погашения пени по основному долгу
                    PumpFactData(clsNode, "pog_fact/doc[@guarantor=0 and @type_summa=\"03\"]",
                        new object[] { "RefOKV", -1, "RefGrnt", factId, "Payment", 1 },
                        new object[] { string.Empty, new object[] { "FactDate", "date" }, 
                                   "sum", new object[] { "Sum", "summa", "CurrencySum", "summa_val" } },
                        fctFactPenaltyDebtPrGrnt, dsFactPenaltyDebtPrGrnt.Tables[0]);

                    // ИФ.Обеспечение
                    PumpFactData(clsNode, "collaterals/collateral", new object[] { "RefS", 0, "RefGrnt", factId },
                        new object[] { string.Empty, new object[] { "Name", "collateral_name", "Cost", "collateral_cost", 
                                   "Sum", "collateral_summa" } },
                        fctCollateralGrnt, dsCollateralGrnt.Tables[0]);
                    PumpFactData(clsNode, "collateral[string-length(@name)>0]", new object[] { "Sum", "1", "RefS", -1, "RefGrnt", factId },
                        new object[] { string.Empty, new object[] { "Name", "name" } },
                        fctCollateralGrnt, dsCollateralGrnt.Tables[0]);

                    // ИФ.Журнал процентов
                    PumpFactData(clsNode, "journal_percent/record[@type_summa=\"02\"]",
                        new object[] { "RefGrnt", factId },
                        new object[] { string.Empty, new object[] { "CreditPercent", "percent", "ChargeDate", "date" } },
                        fctJournalPercentGrnt, dsJournalPercentGrnt.Tables[0]);
                }
                catch (Exception exc)
                {
                    throw new Exception(string.Format("ошибка при обработке договора: {0}", idDoc), exc);
                }
            }

            foreach (DataRow row in dsGuarantIssued.Tables[0].Rows)
            {
                // период гарантии
                if ((row["EndDate"] != DBNull.Value) && (row["StartDate"] != DBNull.Value))
                {
                    DateTime endDate = Convert.ToDateTime(row["EndDate"]);
                    DateTime startDate = Convert.ToDateTime(row["StartDate"]);
                    row["GuarantPeriod"] = (endDate - startDate).Days;
                }
                SetExchangeRate(row);
            }

            foreach (DataRow row in dsFactAttractGrnt.Tables[0].Rows)
                SetExchangeRate(row);
            foreach (DataRow row in dsFactDebtPrGrnt.Tables[0].Rows)
                SetExchangeRate(row);
            foreach (DataRow row in dsFactPercentPrGrnt.Tables[0].Rows)
                SetExchangeRate(row);
            foreach (DataRow row in dsFactPenaltyDebtPrGrnt.Tables[0].Rows)
                SetExchangeRate(row);

            UpdateData();

            if (nullNumPayOrder)
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation,
                    "Из АС Смета не выгружены номера платежных поручений. " +
                    "Необходимо обратиться к разработчику АС Смета за соответствующим обновлением");

            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation,
                string.Format("завершение закачки блока 'Гарантии предоставленные'. Добавлено записей: {0}.", 
                dsGuarantIssued.Tables[0].Rows.Count - grntIssuedRowsCount));
        }

        #endregion Гарантии предоставленные

        #region ценные бумаги

        private void PumpCapital(XmlDocument doc)
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "начало закачки блока 'Ценные бумаги'");
            block = Block.bCapital;
            curMasterRecordCount = dsCapitalFct.Tables[0].Rows.Count;

            Dictionary<string, string> cacheAuxOrg = null;
            FillRowsCache(ref cacheAuxOrg, dsOrg.Tables[0], "IDzap", "name");

            XmlNodeList clsNodes = doc.SelectNodes("xml/data/dogovor[@type_doc=\"0\"]");
            foreach (XmlNode clsNode in clsNodes)
            {
                string idBorrow = clsNode.Attributes["id_borrow"].Value.Trim().Trim('0');
                if (idBorrow == string.Empty)
                    continue;

                object[] auxMapping = null;
                auxMapping = GetChildNodeValues(clsNode, "capital", new object[] { "code", "code" });
                if (auxMapping[1].ToString().Trim().Trim('0') == string.Empty)
                    continue;

                object[] rowMapping = GetNodeValues(clsNode,
                    new object[] { "IDDoc", "id_doc", "StartDate", "date", "EndDate", "date_end", "Note", "note" });
                CheckDate(ref rowMapping);

                int idDoc = Convert.ToInt32(rowMapping[1]);

                try
                {
                    auxMapping = GetNodeValues(clsNode, new object[] { "Number", "number", "Date", "date" });
                    string number = auxMapping[1].ToString();
                    string date = auxMapping[3].ToString();

                    auxMapping = GetNodeValues(clsNode, new object[] { "status_doс", "status_doс" });
                    int refStatusDoc = 1;
                    if (auxMapping[1].ToString().Trim() != "1")
                        refStatusDoc = 3;
                    int refSFormCap = 1;
                    auxMapping = GetChildNodeValues(clsNode, "capital", new object[] { "form_issue", "form_issue" });
                    if (auxMapping[1].ToString() != "0")
                        refSFormCap = 0;

                    auxMapping = new object[] { "ExstraIssue", 0, "CurrencyBorrow", 0, "RefS", -1, "RefStatus", -1, "RefSFormCap", refSFormCap, 
                        "RefSOwnerCap", -1, "RefVariant", refVariant, "RefCapPer", -1, "RefStatusPlan", refStatusDoc, "PretermDischarge", 0 };
                    rowMapping = (object[])CommonRoutines.ConcatArrays(rowMapping, auxMapping);

                    auxMapping = GetChildNodeValues(clsNode, "capital",
                        new object[] { "DateDischarge", "date_discharge", "Trade", "trade", "Depository", "depository", "RefSCap", "type_capital", 
                        "OfficialNumber", "official_number", "CodeCapital", "code", "SeriesCapital", "series", "NumberCapital", 
                        "number", "Nominal", "par", "NominalCurrency", "par_currency", "RegEmissionDate", "date_reg_emission", 
                        "RegNumber", "reg_number", "Discount", "discount", "NameNPA", "base_issue", 
                        "Owner", "limit_owner", "DiscountCurrency", "discount_val", 
                        "RegEmissnNumber", "reg_number" });
                    auxMapping[3] = FindCachedRow(cacheAuxOrg, auxMapping[3].ToString(), "-1");
                    auxMapping[5] = FindCachedRow(cacheAuxOrg, auxMapping[5].ToString(), "-1");
                    auxMapping[7] = FindCachedRow(cacheCapital, auxMapping[7].ToString(), -1);
                    auxMapping[25] = Convert.ToDecimal(auxMapping[25].ToString().Replace('.', ','));
                    auxMapping[31] = Convert.ToDecimal(auxMapping[31].ToString().Replace('.', ','));
                    decimal nominal = Convert.ToDecimal(auxMapping[17].ToString().Replace('.', ','));
                    auxMapping[17] = nominal;
                    auxMapping[19] = Convert.ToDecimal(auxMapping[19].ToString().Replace('.', ','));
                    if (clsNode.Attributes.GetNamedItem("reg_num_doc") != null)
                        auxMapping[23] = clsNode.Attributes["reg_num_doc"].Value;

                    rowMapping = (object[])CommonRoutines.ConcatArrays(rowMapping, auxMapping);

                    auxMapping = new object[] { "RegDate", clsNode.Attributes["date"].Value, 
                                            "RefPeriodicity", clsNode.Attributes["period_discharge_rate"].Value};
                    string periodicity = auxMapping[3].ToString().Trim().ToUpper();
                    if (periodicity == "ПОЛГОДА")
                        auxMapping[3] = "1";
                    else if (periodicity == "ГОД")
                        auxMapping[3] = "3";
                    else if (periodicity == "КВАРТАЛ")
                        auxMapping[3] = "4";
                    else
                        auxMapping[3] = "-1";
                    rowMapping = (object[])CommonRoutines.ConcatArrays(rowMapping, auxMapping);

                    auxMapping = GetChildNodeValues(clsNode, "sum",
                        new object[] { "RefOKV", "okv", "Sum", "summa", "CurrencySum", "summa_val", "Count", string.Empty });
                    dogovorOkv = auxMapping[1].ToString();
                    int refOkv = FindCachedRow(cacheOkv, dogovorOkv, -1);
                    auxMapping[1] = refOkv;
                    if (refOkv == -1)
                        auxMapping[5] = DBNull.Value;
                    decimal sum = Convert.ToDecimal(auxMapping[3].ToString().Replace('.', ','));
                    if (nominal != 0)
                        auxMapping[7] = sum / nominal;
                    else
                        auxMapping[7] = 0;

                    rowMapping = (object[])CommonRoutines.ConcatArrays(rowMapping, auxMapping);
                    CheckSums(ref rowMapping);
                    CheckDate(ref rowMapping);

                    isUpdate = false;
                    if (cacheCapitalFct.ContainsKey(idDoc))
                    {
                        // обновляем
                        DataRow masterRow = cacheCapitalFct[idDoc];
                        factId = Convert.ToInt32(masterRow["Id"]);
                        masterRow["DateDischarge"] = rowMapping[29];
                        masterRow["RefStatusPlan"] = refStatusDoc;
                        isUpdate = true;
                    }
                    else
                    {
                        // закачиваем
                        DataRow factRow = PumpRow(fctCapitalFct, dsCapitalFct.Tables[0], rowMapping, false);
                        UpdateDataSet(daCapitalFct, dsCapitalFct, fctCapitalFct);
                        factId = GetFactMaxId(fctCapitalFct);
                        factRow["Id"] = factId;
                        cacheCapitalFct.Add(idDoc, factRow);
                    }
                    refMasterFieldName = "RefCap";

                    string currencySumAttrName = "summa_val";
                    if (refOkv == -1)
                        currencySumAttrName = string.Empty;

                    // ИФ.Итоги размещения
                    PumpFactData(clsNode, "privl_fact/doc[@type_summa=\"01\"]",
                        new object[] { "Price", DBNull.Value, "ExchangeRate", 1, "RefKif", -1, "RefCap", factId, "RefOKV", refOkv },
                        new object[] { string.Empty, new object[] { "DateDoc", "date", "Quantity", "count" }, 
                                   "sum", new object[] { "Sum", "summa", "CurrencySum", currencySumAttrName } },
                        fctCPFactCapital, dsCPFactCapital.Tables[0]);
                    // ИФ.Факт погашения номинальной стоимости
                    PumpFactData(clsNode, "pog_fact/doc[@type_summa=\"01\"]",
                        new object[] { "RefKif", -1, "RefCap", factId, "RefOkv", refOkv, "Payment", 1 },
                        new object[] { string.Empty, new object[] { "DateDischarge", "date", "Quantity", "count" },
                                   "sum", new object[] { "Sum", "summa", "CurrencySum", currencySumAttrName, "ExchangeRate", "rate" } },
                        fctCPFactDebt, dsCPFactDebt.Tables[0]);

                    // ИФ.Факт выплаты дохода
                    PumpFactData(clsNode, "pog_fact/doc[@type_summa=\"05\"]",
                        new object[] { "RefR", -1, "RefEKR", -1, "RefCap", factId, "RefOkv", refOkv, "Payment", 1, "RefTypeSum", 8, "StimateType", 5 },
                        new object[] { string.Empty, new object[] { "FactDate", "date" },
                                   "sum", new object[] { "Sum", "summa", "CurrencySum", currencySumAttrName } },
                        fctCPFactService, dsCPFactService.Tables[0]);
                    PumpFactData(clsNode, "pog_fact/doc[@type_summa=\"02\"]",
                        new object[] { "RefR", -1, "RefEKR", -1, "RefCap", factId, "RefOkv", refOkv, "Payment", 1, "RefTypeSum", 8, "StimateType", 2 },
                        new object[] { string.Empty, new object[] { "FactDate", "date" },
                                   "sum", new object[] { "Sum", "summa", "CurrencySum", currencySumAttrName } },
                        fctCPFactService, dsCPFactService.Tables[0]);
                    PumpFactData(clsNode, "pog_fact/doc[@type_summa=\"06\"]",
                        new object[] { "RefR", -1, "RefEKR", -1, "RefCap", factId, "RefOkv", refOkv, "Payment", 1, "RefTypeSum", 9, "StimateType", 6 },
                        new object[] { string.Empty, new object[] { "FactDate", "date" },
                                   "sum", new object[] { "Sum", "summa", "CurrencySum", currencySumAttrName } },
                        fctCPFactService, dsCPFactService.Tables[0]);

                    PumpFactData(clsNode, "privl_fact/doc[@type_summa=\"05\"]",
                        new object[] { "RefR", -1, "RefEKR", -1, "RefCap", factId, "RefOkv", refOkv, "Payment", 1, "RefTypeSum", 8, "StimateType", 5 },
                        new object[] { string.Empty, new object[] { "FactDate", "date" },
                                   "sum", new object[] { "ChargeSum", "summa" } },
                        fctCPFactService, dsCPFactService.Tables[0]);
                    PumpFactData(clsNode, "privl_fact/doc[@type_summa=\"02\"]",
                        new object[] { "RefR", -1, "RefEKR", -1, "RefCap", factId, "RefOkv", refOkv, "Payment", 1, "RefTypeSum", 8, "StimateType", 2 },
                        new object[] { string.Empty, new object[] { "FactDate", "date" },
                                   "sum", new object[] { "ChargeSum", "summa" } },
                        fctCPFactService, dsCPFactService.Tables[0]);
                    PumpFactData(clsNode, "privl_fact/doc[@type_summa=\"06\"]",
                        new object[] { "RefR", -1, "RefEKR", -1, "RefCap", factId, "RefOkv", refOkv, "Payment", 1, "RefTypeSum", 9, "StimateType", 6 },
                        new object[] { string.Empty, new object[] { "FactDate", "date" },
                                   "sum", new object[] { "ChargeSum", "summa" } },
                        fctCPFactService, dsCPFactService.Tables[0]);

                    // ИФ.Дополнительные расходы
                    PumpFactData(clsNode, "cost_attract/doc", new object[] { "RefR", -1, "RefEKR", -1, "RefCap", factId, "NumAction", 1 },
                        new object[] { string.Empty, new object[] { "CostDate", "date", "Action", "purpose", "sum", "summa" } },
                        fctCPFactCost, dsCPFactCost.Tables[0]);
                    // ИФ.Курсовая разница
                    PumpFactData(clsNode, "difference_rate/doc", new object[] { "RefCap", factId },
                        new object[] { string.Empty, new object[] { "DateCharge", "date", "StartDate", "start_date", 
                        "EndDate", "end_date", "Sum", "summa", "RefTypeSum", "type_summa", "ExchangeRate", "start_rate" } },
                        fctCPRateSwitch, dsCPRateSwitch.Tables[0]);

                    // ИФ.План погашения номинальной стоимости
                    if (!isUpdate)
                        PumpFactData(clsNode, "pog_plan/osn_dolg/doc", new object[] { "RefKif", -1, "RefCap", factId, "Payment", 1, "RefOKV", refOkv, 
                        "IsPrognozExchRate", false, "ExchangeRate", 0, "PercentNom", 0 },
                            new object[] { string.Empty, new object[] { "EndDate", "date_end" }, 
                                       "sum", new object[] { "Sum", "summa", "CurrencySum", currencySumAttrName } },
                            fctCPPlanDebt, dsCPPlanDebt.Tables[0]);
                    // ИФ.План размещения
                    if (!isUpdate)
                        PumpFactData(clsNode, "privl_plan/doc", new object[] { "RefKif", -1, "RefCap", factId, "RefOKV", refOkv, "Quantity", "0" },
                            new object[] { string.Empty, new object[] { "StartDate", "date_start" }, 
                                       "sum", new object[] { "Sum", "summa" } },
                            fctCPPlanCapital, dsCPPlanCapital.Tables[0]);
                    // ИФ.Обеспечение
                    PumpFactData(clsNode, "collaterals/collateral", new object[] { "RefCap", factId },
                        new object[] { string.Empty, new object[] { "RefOrg", "id_organization", "name", "collateral_name", "Cost", "collateral_cost", 
                        "Sum", "collateral_summa", "RefS", "collateral_type" } },
                        fctCPCollateral, dsCPCollateral.Tables[0]);
                    // ИФ.Журнал ставок процентов
                    PumpFactData(clsNode, "journal_percent/record[@type_summa=\"05\"]", new object[] { "RefCap", factId },
                        new object[] { string.Empty, new object[] { "ChargeDate", "date", "CreditPercent", "percent", "Coupon", "coupon" } },
                        fctCPJournalPercent, dsCPJournalPercent.Tables[0]);

                }
                catch (Exception exc)
                {
                    throw new Exception(string.Format("ошибка при обработке договора: {0}", idDoc), exc);
                }

            }
            UpdateData();

            GroupDetailData(dsCPFactService.Tables[0], new string[] { "RefCap", "FactDate", "StimateType" }, new string[] { "Sum", "CurrencySum", "ChargeSum" });

            cacheAuxOrg.Clear();
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation,
                string.Format("завершение закачки блока 'Ценные бумаги'. Добавлено записей: {0}.",
                dsCapitalFct.Tables[0].Rows.Count - capitalRowsCount));
        }

        #endregion ценные бумаги

        private void CheckDogovors(XmlDocument doc)
        {
            XmlNodeList clsNodes = doc.SelectNodes("xml/data/dogovor[@type_doc=\"-1\"]");
            foreach (XmlNode clsNode in clsNodes)
            {
                string message = string.Format("Договор № {0} от {1}({2}) не был загружен, так как для него не определен Тип договора. {3}", 
                    clsNode.Attributes["number"].Value, clsNode.Attributes["date"].Value, clsNode.Attributes["id_doc"].Value,
                    "Необходимо проставить Тип договора в АС «Смета», выгрузить заново в обменный формат и выполнить закачку данных АС «Смета» в систему планирования заново.");
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, message);
            }
        }

        private void PumpFactDoc(XmlDocument doc)
        {
            CheckDogovors(doc);
            if (toPumpCrInc)
                PumpCreditIncome(doc);
            if (toPumpCrIss)
                PumpCreditIssued(doc);
            if (toPumpGrntIss)
                PumpGuarantIssued(doc);
            if (toPumpCapital)
                PumpCapital(doc);
            UpdateData();
        }

        #endregion факты

        private void PumpXmlFile(FileInfo file)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(file.FullName);
            try
            {
                PumpCls(doc);
                PumpFactDoc(doc);
            }
            finally
            {
                XmlHelper.ClearDomDocument(ref doc);
            }
        }

        #endregion работа с xml

        #region Перекрытые методы закачки

        #region подключение к смете с помощью локального сервера

        private string[] GetUdlData(DirectoryInfo dir)
        {
            FileInfo[] fi = dir.GetFiles("*.udl");
            if (fi.GetLength(0) == 0)
                throw new Exception("в каталоге источника отсутствует UDl файл (параметры подключения к базе АС 'бюджет')");
            return CommonRoutines.GetTxtReportData(fi[0], CommonRoutines.GetTxtWinCodePage())[2].Split(';');
        }

        private void GetUdlParams(string[] udlData, ref string dbName, ref string login, ref string password)
        {
            foreach (string udlParam in udlData)
                if (udlParam.ToUpper().Contains("DATA SOURCE"))
                    dbName = udlParam.Split('=')[1].Trim();
                else if (udlParam.ToUpper().Contains("USER ID"))
                    login = udlParam.Split('=')[1].Trim();
                else if (udlParam.ToUpper().Contains("PASSWORD"))
                    password = udlParam.Split('=')[1].Trim();
        }

        private void SaveStimateData(IStream stimateStream, DirectoryInfo dir)
        {
            System.Runtime.InteropServices.ComTypes.STATSTG stat;
            stimateStream.Stat(out stat, 0);
            System.IntPtr intPtr11 = (IntPtr)0;
            int streamSize = (int)stat.cbSize;
            byte[] streamInfo = new byte[streamSize];
            stimateStream.Read(streamInfo, streamSize, intPtr11);

            string fileName = string.Format("{0}\\stimate_if_0_{1}.xml", dir.FullName, DateTime.Now.ToString("yyyyMMdd"));
            File.WriteAllBytes(fileName, streamInfo);
        }

        private void GetStimateDataLocalServer(DirectoryInfo dir)
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Использование подключения к АС 'Смета' с помощью локального сервера");
            stimateClass = new ContainerClass();
            try
            {
                string[] udlData = GetUdlData(dir);
                string dbName = string.Empty;
                string login = string.Empty;
                string password = string.Empty;
                GetUdlParams(udlData, ref dbName, ref login, ref password);
                if (stimateClass.OpenConfig(dbName, "automation", login, password))
                {
                    try
                    {
                        int progId = stimateClass.GetScriptHandle("Сервер.ФайлВыгрузкиВСПД", 2);
                        object param = string.Format("firstdate=01.01.{0}\r\nlastdate=31.12.{0}", this.DataSource.Year);
                        object result = stimateClass.RunABLScript(progId, ref param);
                        object stream = null;
                        stimateClass.GetSharedStream(out stream);
                        SaveStimateData((IStream)stream, dir);
                        WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Данные АС 'Смета' получены");
                    }
                    finally
                    {
                        stimateClass.CloseConfig();
                    }
                }
                else
                {
                    throw new Exception("Ошибка при подключении к АС 'Смета'");
                }
            }
            finally
            {
                Marshal.ReleaseComObject(stimateClass);
            }
        }

        #endregion подключение к смете с помощью локального сервера

        #region подключение к смете с помощью веб сервиса

        private void GetStimateDataWebService(DirectoryInfo dir)
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Использование подключения к АС 'Смета' с помощью веб сервиса");

            DirectoryInfo dirRegional = new DirectoryInfo(string.Format("{0}\\{1}", GetCurrentDir().FullName, "PumpRegionalParams"));
            FileInfo[] fi = dirRegional.GetFiles("FO28Pump.txt", SearchOption.TopDirectoryOnly);
            if (fi.GetLength(0) == 0)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, "Не найден файл с параметрами веб сервиса");
                return;
            }
            string fileText = File.ReadAllText(fi[0].FullName, Encoding.GetEncoding(1251)).Trim().Replace("\r", string.Empty).Replace("\n", string.Empty);
            string url = fileText.Trim().Replace("StimateWebServiceUrl=", string.Empty);
            string urlWithParams = string.Format("{0}&firstdate=01.01.{1}&lastdate=31.12.{1}", url, this.DataSource.Year);

            HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(urlWithParams);
            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            StreamReader sr = new StreamReader(httpWebResponse.GetResponseStream(), Encoding.GetEncoding(1251));
            string stimateData = sr.ReadToEnd();
            string fileName = string.Format("{0}\\stimate_if_0_{1}.xml", dir.FullName, DateTime.Now.ToString("yyyyMMdd"));
            File.WriteAllText(fileName, stimateData, Encoding.GetEncoding(1251));
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Данные АС 'Смета' получены");
        }

        #endregion подключение к смете с помощью веб сервиса

        private string GetFileNameWithMaxDate(DirectoryInfo dir)
        {
            string fileName = string.Empty;
            decimal maxfileDate = 0;
            FileInfo[] xmlFiles = dir.GetFiles("*.xml", SearchOption.AllDirectories);
            foreach (FileInfo xmlFile in xmlFiles)
            {
                decimal curFileDate = Convert.ToDecimal(Path.GetFileNameWithoutExtension(xmlFile.Name).Split('_')[3]);
                if (curFileDate > maxfileDate)
                {
                    maxfileDate = curFileDate;
                    fileName = xmlFile.Name;
                }
            }
            return fileName;
        }

        private void GetPumpMode()
        {
            stimateConnectMode = StimateConnectMode.useNothing;
            if (Convert.ToBoolean(GetParamValueByName(this.PumpRegistryElement.ProgramConfig, "ucbUseLocalServer", "false")))
                stimateConnectMode = StimateConnectMode.useLocalServer;
            else if (Convert.ToBoolean(GetParamValueByName(this.PumpRegistryElement.ProgramConfig, "ucbUseWeb", "false")))
                stimateConnectMode = StimateConnectMode.useWebService;
        }

        protected override void ProcessFiles(DirectoryInfo dir)
        {
            if (stimateConnectMode == StimateConnectMode.useWebService)
                GetStimateDataWebService(dir);

            if (stimateConnectMode == StimateConnectMode.useLocalServer)
                GetStimateDataLocalServer(dir);

            toPumpCrInc = ToPumpBlock(Block.bCreditIncomes);
            toPumpCrIss = ToPumpBlock(Block.bCreditIssued);
            toPumpGrntIss = ToPumpBlock(Block.bGrntIssued);
            toPumpCapital = ToPumpBlock(Block.bCapital);

            try
            {
                // закачиваем файл с макс датой (самый поздний)
                string fileNameWithMaxDate = GetFileNameWithMaxDate(dir);
                ProcessFilesTemplate(dir, fileNameWithMaxDate, new ProcessFileDelegate(PumpXmlFile), false);
                UpdateData();
            }
            finally
            {
                if (stimateConnectMode != StimateConnectMode.useNothing)
                {
                    FileInfo[] files = dir.GetFiles("*.xml", SearchOption.AllDirectories);
                    foreach (FileInfo file in files)
                        file.Delete();
                }
            }
        }

        protected override void DirectPumpData()
        {
            // для ставрополя удаляем данные фактов
            if (this.Region == RegionName.Stavropol)
                this.DeleteEarlierData = true;

            GetPumpMode();
            CheckSourceDirToEmpty = true;
            if (stimateConnectMode == StimateConnectMode.useWebService)
                //  в папке источника может ничего и не быть
                CheckSourceDirToEmpty = false;
            
            PumpDataYTemplate();
        }

        #endregion Перекрытые методы закачки

        #endregion Закачка данных

        #region обработка данных

        private void InitFactForProcess()
        {
            string constr = string.Format("SOURCEID = {0}", this.SourceID);
            InitDataSet(ref daCreditIncome, ref dsCreditIncome, fctCreditIncome, false, constr, string.Empty);
            InitDataSet(ref daCreditIssued, ref dsCreditIssued, fctCreditIssued, false, constr, string.Empty);
            InitDataSet(ref daGuarantIssued, ref dsGuarantIssued, fctGuarantIssued, false, constr, string.Empty);
            InitDataSet(ref daCapitalFct, ref dsCapitalFct, fctCapitalFct, false, constr, string.Empty);
        }

        protected override void QueryDataForProcess()
        {
            QueryData();
            InitFactForProcess();
        }

        // заполнение CurrencyBorrow
        private int GetCurrencyBorrow(DataRow row, bool useChargeFirstDay)
        {
            if ((row["EndDate"] == DBNull.Value) || (row["StartDate"] == DBNull.Value))
                return 0;
            DateTime startDate = Convert.ToDateTime(row["StartDate"]);
            DateTime endDate = Convert.ToDateTime(row["EndDate"]);
            int currencyBorrow = (endDate - startDate).Days;
            if (useChargeFirstDay)
            {
                bool chargeFirstDay = Convert.ToBoolean(row["ChargeFirstDay"]);
                if (chargeFirstDay)
                    currencyBorrow += 1;
            }
            return currencyBorrow;
        }

        private void CheckContractDate(DataTable dt, string dateField)
        {
            Dictionary<int, DataRow> cache = null;
            FillRowsCache(ref cache, dt, "IDDoc");
            try
            {
                foreach (DataRow row in dt.Rows)
                {
                    int generalDoc = Convert.ToInt32(row["IDGeneralDoc"]);
                    // проверяем дату заключения у генерального договора. если ген договор позже чем текущий - предупреждение
                    if (!cache.ContainsKey(generalDoc))
                        continue;
                    DataRow genRow = cache[generalDoc];
                    DateTime genDate = Convert.ToDateTime(genRow[dateField]);
                    DateTime curDate = Convert.ToDateTime(row[dateField]);
                    if (genDate > curDate)
                    {
                        string message = string.Format("У договора № {0} от {1} {2} № {3} от {4}. {5}",
                            row["Num"], curDate,
                            "в качестве генерального договора в АС Смета выбран более поздний договор",
                            genRow["Num"], genDate,
                            "Если это неверно, внесите изменения в договор в АС Смета, выгрузите обменный файл и закачайте в систему планирования заново.");
                        WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeWarning, message);
                    }
                }
            }
            finally
            {
                cache.Clear();
            }
        }

        private void SetGenInfo(DataTable dt)
        {
            Dictionary<int, DataRow> cache = null;
            FillRowsCache(ref cache, dt, "IDDoc");
            Dictionary<int, string> cacheOrg = null;
            FillRowsCache(ref cacheOrg, dsOrg.Tables[0], "Id", "Name");
            try
            {
                foreach (DataRow row in dt.Rows)
                {
                    int generalDoc = Convert.ToInt32(row["IDGeneralDoc"]);
                    int idDoc = Convert.ToInt32(row["IDDoc"]);
                    if (idDoc == generalDoc)
                        continue;
                    if (!cache.ContainsKey(generalDoc))
                        continue;
                    DataRow genRow = cache[generalDoc];
                    int refOrg = Convert.ToInt32(genRow["RefOrganizations"]);
                    string genInfo = string.Format("{0};{1};{2} - {3};{4}",
                        genRow["Num"], cacheOrg[refOrg], genRow["ContractDate"].ToString().Split(' ')[0], 
                        genRow["EndDate"].ToString().Split(' ')[0], genRow["Sum"]);
                    row["GenDocInfo"] = genInfo;
                }
            }
            finally
            {
                cache.Clear();
                cacheOrg.Clear();
            }
        }

        private void TransfertDebtBookData()
        {
            if (this.Region != RegionName.Stavropol)
                return;
            string errors = string.Empty;
            this.Scheme.FinSourcePlanningFace.TransfertDebtBookData(this.PumpID, ref errors);
            if (errors == string.Empty)
                WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeInformation,
                        "Перенос данных АС 'Смета' в АС 'Долговая книга' на текущий вариант из 'Вариант.Долговая книга' прошел успешно.");
            else
                WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeWarning,
                    string.Format("Перенос данных АС 'Смета' в АС 'Долговая книга' на текущий вариант из 'Вариант.Долговая книга' прошел с ошибками ({0}).", errors));
        }

        protected override void ProcessDataSource()
        {
            Dictionary<string, int> cacheBorrowsAux = null;
            FillRowsCache(ref cacheBorrowsAux, dsKindBorrow.Tables[0], "Id", "RefTypeCredit");

            Dictionary<string, int> cacheCIAux = null;
            FillRowsCache(ref cacheCIAux, dsCreditIncome.Tables[0], "IDDoc", "Id");

            try
            {
                bool isNullRefTypeCredit = false;

                foreach (DataRow row in dsCreditIncome.Tables[0].Rows)
                {
                    // заполнение ExchangeRate
                    decimal sum = Convert.ToDecimal(row["Sum"].ToString().PadLeft(1, '0'));
                    decimal currencySum = Convert.ToDecimal(row["CurrencySum"].ToString().PadLeft(1, '0'));
                    if (sum != 0)
                        row["ExchangeRate"] = currencySum / sum;

                    // заполнение RefSTypeCredit
                    int refBorrow = Convert.ToInt32(row["RefKindBorrow"]);
                    if (cacheBorrowsAux.ContainsKey(refBorrow.ToString()))
                        row["RefSTypeCredit"] = cacheBorrowsAux[refBorrow.ToString()];
                    if (Convert.ToInt32(row["RefSTypeCredit"]) == -1)
                        isNullRefTypeCredit = true;

                    // заполнение CurrencyBorrow
                    int currencyBorrow = GetCurrencyBorrow(row, true);
                    row["CurrencyBorrow"] = currencyBorrow;

                    // заполнение RefSCreditPeriod
                    int creditPeriod = currencyBorrow / 365;
                    int refSCreditPeriod = 1;
                    if (creditPeriod <= 1)
                        refSCreditPeriod = 1;
                    else if (creditPeriod <= 5)
                        refSCreditPeriod = 2;
                    else
                        refSCreditPeriod = 3;
                    row["RefSCreditPeriod"] = refSCreditPeriod;

                    // проставление ParentId
                    string generalDoc = row["IDGeneralDoc"].ToString();
                    if (generalDoc != string.Empty)
                    {
                        string idDoc = row["IDDoc"].ToString();
                        if (generalDoc != idDoc)
                            if (cacheCIAux.ContainsKey(generalDoc))
                                row["ParentId"] = cacheCIAux[generalDoc];
                    }

                }

                if (isNullRefTypeCredit)
                    WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeWarning,
                        "Проверьте проставлен ли тип кредита в справочнике «Виды заимствований», если не проставлен - проставьте и запустите этап обработки заново.");

                foreach (DataRow row in dsCreditIssued.Tables[0].Rows)
                {
                    // заполнение CurrencyBorrow
                    int currencyBorrow = GetCurrencyBorrow(row, true);
                    row["CurrencyBorrow"] = currencyBorrow;
                }

                foreach (DataRow row in dsCapitalFct.Tables[0].Rows)
                {
                    // заполнение CurrencyBorrow
                    int currencyBorrow = GetCurrencyBorrow(row, false);
                    row["CurrencyBorrow"] = currencyBorrow;
                    // заполнение RefCapPer
                    int creditPeriod = currencyBorrow / 365;
                    int refCapPer = 1;
                    if (creditPeriod <= 1)
                        refCapPer = 1;
                    else if (creditPeriod <= 5)
                        refCapPer = 2;
                    else
                        refCapPer = 3;
                    row["RefCapPer"] = refCapPer;
                }

                CheckContractDate(dsCreditIncome.Tables[0], "ContractDate");
                CheckContractDate(dsCreditIssued.Tables[0], "DocDate");

                // проставление информации по ген договору
                SetGenInfo(dsCreditIncome.Tables[0]);

                UpdateData();

                // проставление ссылок на классификацию
                int clsSourceId = AddDataSource("ФО", "0029", ParamKindTypes.Year, string.Empty, this.DataSource.Year + 1, 0, string.Empty, 0, string.Empty).ID;
                CommitTransaction();
                this.Scheme.FinSourcePlanningFace.SetAllReferences(clsSourceId);

                TransfertDebtBookData();
            }
            finally
            {
                cacheBorrowsAux.Clear();
                cacheCIAux.Clear();
            }
        }

        #endregion обработка данных

        #region расчет кубов

        protected override void DirectProcessCube()
        {
            cubesForProcess = new string[] { "712a6f76-72b8-4a16-8548-2ac70f8afa37", "ФО_ИФ_Кредиты полученные",    
                                             "fdb6623f-43d3-4d4a-8938-a9efa5f9c5ef", "ФО_ИФ_Кредиты предоставленные" };
            dimensionsForProcess = new string[] { "712a6f76-72b8-4a16-8548-2ac70f8afa37", "Договор__Кредиты полученные",    
                                             "fdb6623f-43d3-4d4a-8938-a9efa5f9c5ef", "Договор__Кредиты предоставленные" };
            base.DirectProcessCube();
        }

        #endregion расчет кубов

    }

}
