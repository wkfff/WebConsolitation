using System;
using System.Collections.Generic;
using System.Text;
using Krista.FM.ServerLibrary;
using System.Data;
using Krista.FM.Common;
using Krista.FM.ServerLibrary.FinSourcePlanning;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Server
{
    public class BudgetTransfert
    {
        private readonly IScheme scheme;

        private Dictionary<int, decimal> currensyRateList;
        private Dictionary<int, int> currensyCodes;

        private int ifVariant;

        public BudgetTransfert(IScheme scheme)
        {
            this.scheme = scheme;
        }

        public void TransfertData(int incomesVariant, int chargeVariant, int ifVariant,
            int sourceID, int budgetLevel, decimal euroRate, decimal dollarRate, 
            BudgetTransfertOption transfertOption, IClassifiersProtocol protocol)
        {
            currensyRateList = new Dictionary<int, decimal>();
            currensyRateList.Add(840, dollarRate);
            currensyRateList.Add(978, euroRate);
            TransfertData(incomesVariant, chargeVariant, ifVariant, sourceID, budgetLevel, transfertOption, protocol);
        }

        public void TransfertData(int incomesVariant, int chargeVariant, int ifVariant,
            int sourceID, int budgetLevel, BudgetTransfertOption transfertOption, IClassifiersProtocol protocol)
        {
            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                if (currensyRateList == null)
                    currensyRateList = new Dictionary<int, decimal>();
                currensyCodes = new Dictionary<int, int>();
                this.ifVariant = ifVariant;

                IEntity creditIncomeEntity = scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.f_S_Сreditincome_Key);
                IEntity creditOutcomeEntity = scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.f_S_Creditissued_Key);
                IEntity guarantieEntity = scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.f_S_Guarantissued_Key);
                IEntity capitalEntity = scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.f_S_Capital_Key);

                // запросы на получение данных из мастер-таблиц
                string query = "select id, refokv, RefSStatusPlan from {0} where (RefVariant = 0 or (RefVariant = {1} and RefSStatusPlan = 0)) and (RefSTypeCredit = 0 or RefSTypeCredit = 1)";
                DataTable dtCreditsIncome = (DataTable)db.ExecQuery(string.Format(query, creditIncomeEntity.FullDBName, ifVariant), QueryResultTypes.DataTable);
                query = "select id, RefRegions, RefSStatusPlan from {0} where (RefVariant = 0 or (RefVariant = {1} and RefSStatusPlan = 0)) and (RefSTypeCredit = 3 or RefSTypeCredit = 4)";
                DataTable dtCreditsOutcome = (DataTable)db.ExecQuery(string.Format(query, creditOutcomeEntity.FullDBName, ifVariant), QueryResultTypes.DataTable);
                query = "select id, regress, refokv, RefSStatusPlan from {0} where RefVariant = 0 or (RefVariant = {1} and RefSStatusPlan = 0)";
                DataTable dtGuarantees = (DataTable)db.ExecQuery(string.Format(query, guarantieEntity.FullDBName, ifVariant), QueryResultTypes.DataTable);
                query = "select id, RefReg, refokv, RefStatusPlan, RefVariant from {0} where RefVariant = 0 or (RefVariant = {1} and RefStatusPlan = 1)";
                DataTable dtCapitals = (DataTable)db.ExecQuery(string.Format(query, capitalEntity.FullDBName, ifVariant), QueryResultTypes.DataTable);

                if ((transfertOption & BudgetTransfertOption.IncomesPart) != 0)
                    TransfertIncomesData(incomesVariant, db, sourceID, budgetLevel, dtCreditsOutcome, protocol);
                if ((transfertOption & BudgetTransfertOption.ChargesPart) != 0)
                    TransfertOutcomesData(chargeVariant, db, sourceID, budgetLevel, dtCreditsIncome, dtGuarantees, dtCapitals, protocol);
                if ((transfertOption & BudgetTransfertOption.IfPart) != 0)
                    TransfertIfData(ifVariant, db, sourceID, budgetLevel, dtCreditsIncome, dtCreditsOutcome, dtGuarantees, dtCapitals, protocol);
            }
        }

        public void TransfertData(int incomesVariant, int chargeVariant, int ifVariant, int sourceID,
            int budgetLevel, IClassifiersProtocol protocol)
        {
            TransfertData(incomesVariant, chargeVariant, ifVariant, sourceID,
                budgetLevel, BudgetTransfertOption.All, protocol);
        }

        /// <summary>
        /// перенос данных из источников в доходы
        /// </summary>
        /// <param name="incomesVariant"></param>
        /// <param name="db"></param>
        /// <param name="sourceID"></param>
        /// <param name="budgetLevel"></param>
        /// <param name="dtCreditsOutcome"></param>
        /// <param name="protocol"></param>
        private void TransfertIncomesData(int incomesVariant, IDatabase db, int sourceID,
            int budgetLevel, DataTable dtCreditsOutcome, IClassifiersProtocol protocol)
        {
            // периоды, за которые переносим данные
            DateTime startPeriod = new DateTime(DateTime.Today.Year, 01, 01);
            DateTime endPeriod = new DateTime(DateTime.Today.Year + 3, 12, 31);
            IEntity f_D_FOPlanIncDivideEntity = scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.f_D_FOPlanIncDivide);
            IDataUpdater duPlanIncDivideEntity = f_D_FOPlanIncDivideEntity.GetDataUpdater(string.Format("SourceID = {0} and RefVariant = {1} and FromSF = 1", sourceID, incomesVariant), null);
            // удаляем данные, которые перенесли ранее на указанный вариант
            f_D_FOPlanIncDivideEntity.DeleteData(string.Format("where SourceID = {0} and RefVariant = {1} and FromSF = 1", sourceID, incomesVariant));
            DataTable dtPlanIncDivide = new DataTable();
            duPlanIncDivideEntity.Fill(ref dtPlanIncDivide);
            // обрабатываем данные и добавляем записи в таблицу
            GetIncomesData(f_D_FOPlanIncDivideEntity, db, dtPlanIncDivide, dtCreditsOutcome,
                    budgetLevel, incomesVariant, sourceID, startPeriod, endPeriod);
            protocol.WriteEventIntoClassifierProtocol(ClassifiersEventKind.ceInformation,
            f_D_FOPlanIncDivideEntity.FullCaption, -1, sourceID, (int)ClassTypes.clsFactData,
                string.Format("Перенос данных в проект бюджета из источников финансирования. Добавлено {0} записей", dtPlanIncDivide.Rows.Count));
            // сохраняем данные
            duPlanIncDivideEntity.Update(ref dtPlanIncDivide);
        }

        /// <summary>
        /// перенос данных из источников в расходы
        /// </summary>
        /// <param name="chargeVariant"></param>
        /// <param name="db"></param>
        /// <param name="sourceID"></param>
        /// <param name="budgetLevel"></param>
        /// <param name="dtCreditsIncome"></param>
        /// <param name="dtGuarantees"></param>
        /// <param name="dtCapitals"></param>
        /// <param name="protocol"></param>
        private void TransfertOutcomesData(int chargeVariant, IDatabase db, int sourceID,
            int budgetLevel, DataTable dtCreditsIncome, DataTable dtGuarantees, DataTable dtCapitals, IClassifiersProtocol protocol)
        {
            // периоды, за которые переносим данные
            DateTime startPeriod = new DateTime(DateTime.Today.Year, 01, 01);
            DateTime endPeriod = new DateTime(DateTime.Today.Year + 3, 12, 31);
            IEntity f_R_FO26REntity = scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.f_R_FO26R);
            // удаляем данные, которые перенесли ранее на указанный вариант
            f_R_FO26REntity.DeleteData(string.Format("where SourceID = {0} and RefVariant = {1} and FromSF = 1", sourceID, chargeVariant));
            IDataUpdater duFO26RPlan = f_R_FO26REntity.GetDataUpdater(string.Format("SourceID = {0} and RefVariant = {1} and FromSF = 1", sourceID, chargeVariant), null);
            DataTable dtFO26RPlan = new DataTable();
            duFO26RPlan.Fill(ref dtFO26RPlan);
            // обрабатываем данные и добавляем записи в таблицу
            GetChargesData(f_R_FO26REntity, db, dtFO26RPlan, dtCreditsIncome, dtGuarantees, dtCapitals,
                    budgetLevel, chargeVariant, sourceID, startPeriod, endPeriod);
            protocol.WriteEventIntoClassifierProtocol(ClassifiersEventKind.ceInformation,
                    f_R_FO26REntity.FullCaption, -1, sourceID, (int)ClassTypes.clsFactData, string.Format("Перенос данных в проект бюджета из источников финансирования. Добавлено {0} записей", dtFO26RPlan.Rows.Count));
            // сохраняем данные
            duFO26RPlan.Update(ref dtFO26RPlan);
        }

        /// <summary>
        /// перенос данных в проект бюджета ИФ
        /// </summary>
        /// <param name="ifVariant"></param>
        /// <param name="db"></param>
        /// <param name="sourceID"></param>
        /// <param name="budgetLevel"></param>
        /// <param name="dtCreditsIncome"></param>
        /// <param name="dtCreditsOutcome"></param>
        /// <param name="dtGuarantees"></param>
        /// <param name="dtCapitals"></param>
        /// <param name="protocol"></param>
        private void TransfertIfData(int ifVariant, IDatabase db, int sourceID, int budgetLevel,
            DataTable dtCreditsIncome, DataTable dtCreditsOutcome, DataTable dtGuarantees, DataTable dtCapitals,
            IClassifiersProtocol protocol)
        {
            // периоды, за которые переносим данные
            DateTime startPeriod = new DateTime(DateTime.Today.Year, 01, 01);
            DateTime endPeriod = new DateTime(DateTime.Today.Year + 3, 12, 31);
            IEntity f_S_PlanEntity = scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.f_S_Plan_Key);
            // удаляем данные, которые перенесли ранее на указанный вариант
            f_S_PlanEntity.DeleteData(string.Format("where SourceID = {0} and RefSVariant = {1} and FromSF = 1", sourceID, ifVariant));
            IDataUpdater duPlan = f_S_PlanEntity.GetDataUpdater(string.Format("SourceID = {0} and RefSVariant = {1} and FromSF = 1", sourceID, ifVariant), null);
            DataTable dtPlan = new DataTable();
            duPlan.Fill(ref dtPlan);
            // обрабатываем данные и добавляем записи в таблицу
            GetPlanData(f_S_PlanEntity, db, dtPlan, dtCreditsIncome, dtCreditsOutcome, dtGuarantees, dtCapitals,
                    budgetLevel, ifVariant, sourceID, startPeriod, endPeriod);
            protocol.WriteEventIntoClassifierProtocol(ClassifiersEventKind.ceInformation,
                f_S_PlanEntity.FullCaption, -1, sourceID, (int)ClassTypes.clsFactData, string.Format("Перенос данных в проект бюджета из источников финансирования. Добавлено {0} записей", dtPlan.Rows.Count));
            // сохраняем данные
            duPlan.Update(ref dtPlan);
        }

        /// <summary>
        /// проверка на корректность данных относительно текущего и планового годов
        /// </summary>
        private static bool IsCorrectData(DateTime checkedDate, int currentYear, int nextYear)
        {
            return !(checkedDate.Year < currentYear || checkedDate.Year > nextYear);
        }

        private delegate bool CheckDetailDataDelegate(int contractPlaningType, DateTime detailDate);

        /// <summary>
        /// статус при планировании, для которого данные всегда попадают в проект бюджета
        /// </summary>
        private int chekedPlaningStatus = -2;

        /// <summary>
        /// данные для ФО.Результат ИФ
        /// </summary>
        private void GetPlanData(IEntity planEntity, IDatabase db, DataTable dtPlan,
            DataTable dtCreditsIncome, DataTable dtCreditsOutcome, DataTable dtGuarantees, DataTable dtCapitals,
            int budgetLevel, int variant, int sourceID, DateTime startPeriod, DateTime endPeriod)
        {
            const string creditQuery = "select sum, {0}, RefKIF from {1} where RefCreditInc = {2} and ({0} Between ? and ?)";
            const string creditCurrencyQuery = "select Sum, CurrencySum, {0}, RefKIF from {1} where RefCreditInc = {2} and ({0} Between ? and ?)";
            const string guaranteeQuery = "select sum, {0}, RefKIF from {1} where RefGrnt = {2} and ({0} Between ? and ?)";
            const string guaranteeCurrencyQuery = "select Sum, CurrencySum, {0}, RefKIF from {1} where RefGrnt = {2} and ({0} Between ? and ?)";
            const string capitalQuery = "select sum, {0}, RefKIF from {1} where RefCap = {2} and ({0} Between ? and ?)";
            const string capitalCurrencyQuery = "select Sum, CurrencySum, {0} as EndDate, RefKIF from {1} where RefCap = {2} and ({0} Between ? and ?)";

            CheckDetailDataDelegate checkFact = new CheckDetailDataDelegate(
                delegate(int contractPlaningType, DateTime detailDate)
                {
                    if ((contractPlaningType == 2 || contractPlaningType == 3 || contractPlaningType == 4) && detailDate.Year == DateTime.Today.Year)
                        return true;
                    if ((contractPlaningType == 0 || contractPlaningType == 5) && (detailDate >= new DateTime(DateTime.Today.Year, 1, 1) && detailDate <= DateTime.Today))
                        return true;
                    return false;
                });

            #region данные из кредитов полученных
            foreach (DataRow row in dtCreditsIncome.Rows)
            {
                object id = row[0];
                int refOkv = -1;
                // для валют получим последний актуальный курс
                decimal currencyRate = 1;
                int planingStatus = Convert.ToInt32(row["RefSStatusPlan"]);
                // план привлечения
                DataRow[] detailRows = null;
                // анонимный методя для проверки 

                // если у нас договор принят или на рефинансировании считаем план
                detailRows = GetDetailData(creditQuery, creditCurrencyQuery, "StartDate", "t_S_PlanAttractCI",
                    startPeriod, endPeriod, id, refOkv, currencyRate, db);
                foreach (DataRow detailRow in detailRows)
                {
                    AddEstimateForecastToPlan(planEntity, dtPlan, detailRow, budgetLevel, variant, sourceID,
                        startPeriod.Year, endPeriod.Year, -1, planingStatus);
                }

                //план погашения основного долга
                detailRows = GetDetailData(creditQuery, creditCurrencyQuery, "EndDate", "t_S_PlanDebtCI", startPeriod, endPeriod, id, refOkv, currencyRate, db);
                foreach (DataRow detailRow in detailRows)
                {
                    AddEstimateForecastToPlan(planEntity, dtPlan, detailRow, budgetLevel, variant, sourceID,
                        startPeriod.Year, endPeriod.Year, -1, planingStatus);
                }
                // курсовая разница
                detailRows = GetDetailData(db,
                   string.Format("select sum, EndDate, RefKIF from t_S_RateSwitchCI where RefCreditInc = {0} and (EndDate >= ? and EndDate <= ?)", id),
                   new System.Data.OleDb.OleDbParameter("p0", startPeriod), new System.Data.OleDb.OleDbParameter("p1", endPeriod));
                foreach (DataRow detailRow in detailRows)
                {
                    AddRateSwitchToPlan(planEntity, dtPlan, detailRow, budgetLevel, variant, sourceID,
                        startPeriod.Year, endPeriod.Year, -1, planingStatus);
                }
                // факт считаем если у нас договор Закрыт, Рефинансирован, Досрочно погашен 
                detailRows = GetDetailData(creditQuery, creditCurrencyQuery, "FactDate", "t_S_FactAttractCI",
                                            startPeriod, endPeriod, id, refOkv, currencyRate, db);
                foreach (DataRow detailRow in detailRows)
                {
                    AddFactToPlan(planEntity, dtPlan, detailRow, budgetLevel, variant, sourceID,
                        startPeriod.Year, -1, planingStatus);
                }
                // факт погашения основного долга
                detailRows = GetDetailData(creditQuery, creditCurrencyQuery, "FactDate", "t_S_FactDebtCI",
                                            startPeriod, endPeriod, id, refOkv, currencyRate, db);
                foreach (DataRow detailRow in detailRows)
                {
                    AddFactToPlan(planEntity, dtPlan, detailRow, budgetLevel, variant, sourceID,
                        startPeriod.Year, -1, planingStatus);
                }
            }

            #endregion

            #region данные из кредитов предоставленных
            foreach (DataRow row in dtCreditsOutcome.Rows)
            {
                object id = row[0];
                int refOkv = -1;
                decimal currencyRate = 1;
                int planingStatus = Convert.ToInt32(row["RefSStatusPlan"]);
                // план привлечения 
                DataRow[] detailRows = null;

                detailRows = GetDetailData(creditQuery, creditCurrencyQuery, "StartDate", "t_S_PlanAttractCO",
                                            startPeriod, endPeriod, id, refOkv, currencyRate, db);
                foreach (DataRow detailRow in detailRows)
                {
                    AddEstimateForecastToPlan(planEntity, dtPlan, detailRow, budgetLevel, variant, sourceID,
                        startPeriod.Year, endPeriod.Year, row["RefRegions"], planingStatus);
                }
                // план погашения
                detailRows = GetDetailData(creditQuery, creditCurrencyQuery, "EndDate", "t_S_PlanDebtCO",
                    startPeriod, endPeriod, id, refOkv, currencyRate, db);
                foreach (DataRow detailRow in detailRows)
                {
                    AddEstimateForecastToPlan(planEntity, dtPlan, detailRow, budgetLevel, variant, sourceID,
                        startPeriod.Year, endPeriod.Year, row["RefRegions"], planingStatus);
                }

                // факт привлечения
                detailRows = GetDetailData(creditQuery, creditCurrencyQuery, "FactDate", "t_S_FactAttractCO",
                    startPeriod, endPeriod, id, refOkv, currencyRate, db);
                foreach (DataRow detailRow in detailRows)
                {
                    AddFactToPlan(planEntity, dtPlan, detailRow, budgetLevel, variant, sourceID,
                        startPeriod.Year, row["RefRegions"], planingStatus);
                }
                // факт погашения основного долга
                detailRows = GetDetailData(creditQuery, creditCurrencyQuery, "FactDate", "t_S_FactDebtCO",
                    startPeriod, endPeriod, id, refOkv, currencyRate, db);
                foreach (DataRow detailRow in detailRows)
                {
                    AddFactToPlan(planEntity, dtPlan, detailRow, budgetLevel, variant, sourceID,
                        startPeriod.Year, row["RefRegions"], planingStatus);
                }

            }
            #endregion

            #region данные по гарантиям
            foreach (DataRow row in dtGuarantees.Rows)
            {
                object id = row[0];
                int refOkv = -1;
                // для валют получим последний актуальный курс
                decimal currencyRate = GetCurrencyExchRate(refOkv, startPeriod, db);
                if (Convert.ToBoolean(row["Regress"]))
                {
                    int planingStatus = Convert.ToInt32(row["RefSStatusPlan"]);
                    DataRow[] detailRows = null;
                    detailRows = GetDetailData(guaranteeQuery, guaranteeCurrencyQuery, "EndDate",
                        "t_S_PlanAttractGrnt", startPeriod, endPeriod, id, refOkv, currencyRate, db);
                    foreach (DataRow detailRow in detailRows)
                    {
                        AddEstimateForecastToPlan(planEntity, dtPlan, detailRow, budgetLevel, variant, sourceID,
                            startPeriod.Year, endPeriod.Year, -1, planingStatus);
                    }

                    detailRows = GetDetailData(guaranteeQuery, guaranteeCurrencyQuery, "EndDate",
                        "t_S_PlanDebtPrGrnt", startPeriod, endPeriod, id, refOkv, currencyRate, db);
                    foreach (DataRow detailRow in detailRows)
                    {
                        AddEstimateForecastToPlan(planEntity, dtPlan, detailRow, budgetLevel, variant, sourceID,
                            startPeriod.Year, endPeriod.Year, -1, planingStatus);
                    }

                    detailRows = GetDetailData(guaranteeQuery, guaranteeCurrencyQuery, "EndDate",
                        "t_S_PlanServicePrGrnt", startPeriod, endPeriod, id, refOkv, currencyRate, db);
                    foreach (DataRow detailRow in detailRows)
                    {
                        AddEstimateForecastToPlan(planEntity, dtPlan, detailRow, budgetLevel, variant, sourceID,
                            startPeriod.Year, endPeriod.Year, -1, planingStatus);
                    }

                    detailRows = GetDetailData(guaranteeQuery, guaranteeCurrencyQuery, "FactDate",
                        "t_S_FactAttractGrnt", startPeriod, endPeriod, id, refOkv, currencyRate, db);
                    foreach (DataRow detailRow in detailRows)
                    {
                        AddFactToPlan(planEntity, dtPlan, detailRow, budgetLevel, variant, sourceID,
                            startPeriod.Year, -1, planingStatus);
                    }

                    detailRows = GetDetailData(guaranteeQuery, guaranteeCurrencyQuery, "FactDate",
                        "t_S_FactDebtPrGrnt", startPeriod, endPeriod, id, refOkv, currencyRate, db);
                    foreach (DataRow detailRow in detailRows)
                    {
                        AddFactToPlan(planEntity, dtPlan, detailRow, budgetLevel, variant, sourceID,
                            startPeriod.Year, -1, planingStatus);
                    }

                    detailRows = GetDetailData(guaranteeQuery, guaranteeCurrencyQuery, "FactDate",
                        "t_S_FactPercentPrGrnt", startPeriod, endPeriod, id, refOkv, currencyRate, db);
                    foreach (DataRow detailRow in detailRows)
                    {
                        AddFactToPlan(planEntity, dtPlan, detailRow, budgetLevel, variant, sourceID,
                            startPeriod.Year, -1, planingStatus);
                    }
                }
            }
            #endregion

            #region данные по ценным бумагам
            foreach (DataRow row in dtCapitals.Rows)
            {
                object id = row[0];
                int refOkv = -1;
                int planingStatus = Convert.ToInt32(row["RefStatusPlan"]);
                int rowVariant = Convert.ToInt32(row["RefVariant"]);
                if (planingStatus == 1 && rowVariant == 0)
                    planingStatus = 0;
                else
                    planingStatus = -2;
                // для валют получим последний актуальный курс
                decimal currencyRate = GetCurrencyExchRate(refOkv, startPeriod, db);
                DataRow[] detailRows = null;
                detailRows = GetDetailData(capitalQuery, capitalCurrencyQuery, "StartDate", "t_S_CPPlanCapital", startPeriod, endPeriod, id, refOkv, currencyRate, db);
                foreach (DataRow detailRow in detailRows)
                {
                    AddEstimateForecastToPlan(planEntity, dtPlan, detailRow, budgetLevel, variant, sourceID,
                        startPeriod.Year, endPeriod.Year, -1, planingStatus);
                }

                detailRows = GetDetailData(capitalQuery, capitalCurrencyQuery, "EndDate", "t_S_CPPlanDebt", startPeriod, endPeriod, id, refOkv, currencyRate, db);
                foreach (DataRow detailRow in detailRows)
                {
                    AddEstimateForecastToPlan(planEntity, dtPlan, detailRow, budgetLevel, variant, sourceID,
                        startPeriod.Year, endPeriod.Year, -1, planingStatus);
                }

                detailRows = GetDetailData(capitalQuery, capitalCurrencyQuery, "DateDoc", "t_S_CPFactCapital", startPeriod, endPeriod, id, refOkv, currencyRate, db);
                foreach (DataRow detailRow in detailRows)
                {
                    AddRateSwitchToPlan(planEntity, dtPlan, detailRow, budgetLevel, variant, sourceID,
                        startPeriod.Year, endPeriod.Year, -1, planingStatus);
                }

                detailRows = GetDetailData(db,
                   string.Format("select sum, EndDate, RefKIF from t_S_CPRateSwitch where RefCap = {0}", id));
                foreach (DataRow detailRow in detailRows)
                {
                    AddRateSwitchToPlan(planEntity, dtPlan, detailRow, budgetLevel, variant, sourceID,
                        startPeriod.Year, endPeriod.Year, -1, planingStatus);
                }

                detailRows = GetDetailData(capitalQuery, capitalCurrencyQuery, "DateDischarge", "t_S_CPFactDebt", startPeriod, endPeriod, id, refOkv, currencyRate, db);
                foreach (DataRow detailRow in detailRows)
                {
                    AddFactToPlan(planEntity, dtPlan, detailRow, budgetLevel, variant, sourceID,
                        startPeriod.Year, -1, planingStatus);
                }
            }
            #endregion
        }

        /// <summary>
        /// Данные для расходов
        /// </summary>
        private void GetChargesData(IEntity chargesEntity, IDatabase db, DataTable dtCharges,
            DataTable dtCreditsIncome, DataTable dtGuarantees, DataTable dtCapitals,
            int budgetLevel, int variant, int sourceID, DateTime startPeriod, DateTime endPeriod)
        {
            const string creditQuery = "select sum, {0}, RefR, RefEKR from {1} where RefCreditInc = {2} and ({0} Between ? and ?)";
            const string creditCurrencyQuery = "select Sum, CurrencySum, {0}, RefR, RefEKR from {1} where RefCreditInc = {2} and ({0} Between ? and ?)";
            const string guaranteeQuery = "select sum, {0}, RefR, RefEKR from {1} where RefGrnt = {2} and ({0} Between ? and ?)";
            const string guaranteeCurrencyQuery = "select Sum, CurrencySum, {0}, RefR, RefEKR from {1} where RefGrnt = {2} and ({0} Between ? and ?)";
            const string capitalQuery = "select sum, {0}, RefR, RefEKR from {1} where RefCap = {2} and ({0} Between ? and ?)";
            const string capitalCurrencyQuery = "select Sum, CurrencySum, {0} as EndDate, RefR, RefEKR from {1} where RefCap = {2} and ({0} Between ? and ?)";

            // данные из кредитов полученных
            foreach (DataRow row in dtCreditsIncome.Rows)
            {
                object id = row[0];
                int refOkv = -1;//Convert.ToInt32(row["RefOKV"]);
                int planingStatus = Convert.ToInt32(row["RefSStatusPlan"]);
                // для валют получим последний актуальный курс
                decimal currencyRate = GetCurrencyExchRate(refOkv, startPeriod, db);

                DataRow[] detailRows = GetDetailData(creditQuery, creditCurrencyQuery, "EndDate", "t_S_PlanServiceCI", startPeriod, endPeriod, id, refOkv, currencyRate, db);
                foreach (DataRow detailRow in detailRows)
                {
                    AddToChargeData(chargesEntity, dtCharges, detailRow, budgetLevel, variant, sourceID,
                        startPeriod.Year, endPeriod.Year, planingStatus);
                }

                detailRows = GetDetailData(creditQuery, creditCurrencyQuery, "StartDate", "t_S_ChargePenaltyDebtCI", startPeriod, endPeriod, id, refOkv, currencyRate, db);
                foreach (DataRow detailRow in detailRows)
                {
                    AddToChargeData(chargesEntity, dtCharges, detailRow, budgetLevel, variant, sourceID,
                        startPeriod.Year, endPeriod.Year, planingStatus);
                }

                detailRows = GetDetailData(creditQuery, creditCurrencyQuery, "StartDate", "t_S_CIChargePenaltyPercent", startPeriod, endPeriod, id, refOkv, currencyRate, db);
                foreach (DataRow detailRow in detailRows)
                {
                    AddToChargeData(chargesEntity, dtCharges, detailRow, budgetLevel, variant, sourceID,
                        startPeriod.Year, endPeriod.Year, planingStatus);
                }
            }

            // данные по гарантиям
            foreach (DataRow row in dtGuarantees.Rows)
            {
                if (!Convert.ToBoolean(row["Regress"]))
                {
                    object id = row[0];
                    int refOkv = Convert.ToInt32(row["RefOKV"]);
                    // для валют получим последний актуальный курс
                    decimal currencyRate = GetCurrencyExchRate(refOkv, startPeriod, db);
                    int planingStatus = Convert.ToInt32(row["RefSStatusPlan"]);
                    DataRow[] detailRows = GetDetailData(guaranteeQuery, guaranteeCurrencyQuery, "EndDate", "t_S_PlanAttractGrnt", startPeriod, endPeriod, id, refOkv, currencyRate, db);
                    foreach (DataRow detailRow in detailRows)
                    {
                        AddToChargeData(chargesEntity, dtCharges, detailRow, budgetLevel, variant, sourceID,
                            startPeriod.Year, endPeriod.Year, planingStatus);
                    }
                }
            }
            // данные по ценным бумагам
            foreach (DataRow row in dtCapitals.Rows)
            {
                object id = row[0];
                int refOkv = -1;//Convert.ToInt32(row["RefOKV"]);
                // для валют получим последний актуальный курс
                decimal currencyRate = GetCurrencyExchRate(refOkv, startPeriod, db);
                DataRow[] detailRows = new DataRow[0];

                detailRows = GetDetailData(capitalQuery, capitalCurrencyQuery, "CostDate", "t_S_CPPlanCost", startPeriod, endPeriod, id, -1, currencyRate, db);
                foreach (DataRow detailRow in detailRows)
                {
                    AddToChargeData(chargesEntity, dtCharges, detailRow, budgetLevel, variant, sourceID,
                        startPeriod.Year, endPeriod.Year, 0);
                }

                detailRows = GetDetailData(capitalQuery, capitalCurrencyQuery, "EndDate", "t_S_CPPlanService", startPeriod, endPeriod, id, refOkv, currencyRate, db);
                foreach (DataRow detailRow in detailRows)
                {
                    AddToChargeData(chargesEntity, dtCharges, detailRow, budgetLevel, variant, sourceID,
                        startPeriod.Year, endPeriod.Year, 0);
                }

                detailRows = GetDetailData(capitalQuery, capitalCurrencyQuery, "StartDate", "t_S_CPChargePenaltyDeb", startPeriod, endPeriod, id, refOkv, currencyRate, db);
                foreach (DataRow detailRow in detailRows)
                {
                    AddToChargeData(chargesEntity, dtCharges, detailRow, budgetLevel, variant, sourceID,
                        startPeriod.Year, endPeriod.Year, 0);
                }

                detailRows = GetDetailData(capitalQuery, capitalCurrencyQuery, "StartDate", "t_S_CPChargePenaltyPer", startPeriod, endPeriod, id, refOkv, currencyRate, db);
                foreach (DataRow detailRow in detailRows)
                {
                    AddToChargeData(chargesEntity, dtCharges, detailRow, budgetLevel, variant, sourceID,
                        startPeriod.Year, endPeriod.Year, 0);
                }
            }
        }

        /// <summary>
        /// Данные для доходов
        /// </summary>
        private void GetIncomesData(IEntity incomesEntity, IDatabase db, DataTable dtPlan,
            DataTable dtCreditsOutcome, int budgetLevel, int variant, int sourceID, DateTime startPeriod, DateTime endPeriod)
        {
            const string creditQuery = "select sum, {0}, RefKD from {1} where RefCreditInc = {2} and ({0} Between ? and ?)";
            const string creditCurrencyQuery = "select Sum, CurrencySum, {0}, RefKD from {1} where RefCreditInc = {2} and ({0} Between ? and ?)";
            // данные из кредитов предоставленных
            foreach (DataRow row in dtCreditsOutcome.Rows)
            {
                object id = row[0];
                int refOkv = -1;
                // для валют получим последний актуальный курс
                decimal currencyRate = 1;
                int planingStatus = Convert.ToInt32(row["RefSStatusPlan"]);
                if (planingStatus == 0 || planingStatus == 5)
                {
                    DataRow[] detailRows = GetDetailData(creditQuery, creditCurrencyQuery, "EndDate",
                                                         "t_S_PlanServiceCO", startPeriod, endPeriod, id, refOkv,
                                                         currencyRate, db);
                    foreach (DataRow detailRow in detailRows)
                    {
                        AddToIncomesData(incomesEntity, dtPlan, detailRow, budgetLevel, variant, sourceID,
                                         startPeriod.Year, endPeriod.Year, row["RefRegions"]);
                    }

                    detailRows = GetDetailData(creditQuery, creditCurrencyQuery, "StartDate", "t_S_ChargePenaltyDebtCO",
                                               startPeriod, endPeriod, id, refOkv, currencyRate, db);
                    foreach (DataRow detailRow in detailRows)
                    {
                        AddToIncomesData(incomesEntity, dtPlan, detailRow, budgetLevel, variant, sourceID,
                                         startPeriod.Year, endPeriod.Year, row["RefRegions"]);
                    }

                    detailRows = GetDetailData(creditQuery, creditCurrencyQuery, "StartDate",
                                               "t_S_COChargePenaltyPercent", startPeriod, endPeriod, id, refOkv,
                                               currencyRate, db);
                    foreach (DataRow detailRow in detailRows)
                    {
                        AddToIncomesData(incomesEntity, dtPlan, detailRow, budgetLevel, variant, sourceID,
                                         startPeriod.Year, endPeriod.Year, row["RefRegions"]);
                    }
                }
            }
        }

        private decimal GetCurrencyExchRate(int refOkv, DateTime date, IDatabase db)
        {
            decimal currencyRate = 1;
            if (refOkv != -1)
            {
                int currencyCode = 0;
                if (!currensyCodes.TryGetValue(refOkv, out currencyCode))
                {
                    currencyCode = Convert.ToInt32(db.ExecQuery("select Code from d_OKV_Currency where ID = ?",
                                                    QueryResultTypes.Scalar, new DbParameterDescriptor("p0", refOkv)));
                    currensyCodes.Add(refOkv, currencyCode);
                }
                else
                {
                    currencyCode = currensyCodes[refOkv];
                }

                if (!currensyRateList.TryGetValue(currencyCode, out currencyRate))
                {
                    currencyRate = Utils.GetLastCurrencyExchange(date, refOkv);
                    currensyRateList.Add(currencyCode, currencyRate);
                }
            }
            return currencyRate;
        }

        /// <summary>
        /// Получение данных из детали с учетом валюты и курса на нее
        /// </summary>
        /// <returns></returns>
        private DataRow[] GetDetailData(string queryTemplate, string currencyQueryTemplate, string dateColumn, string tableName,
            DateTime startPeriod, DateTime endPeriod, object masterID, int refOkv, decimal currencyRate, IDatabase db)
        {
            DataRow[] detailRows = null;
            if (refOkv == -1)
            {
                detailRows = GetDetailData(db, string.Format(queryTemplate, dateColumn, tableName, masterID),
                    new System.Data.OleDb.OleDbParameter("p0", startPeriod), new System.Data.OleDb.OleDbParameter("p1", endPeriod));
            }
            else
            {
                detailRows = GetDetailData(db, string.Format(currencyQueryTemplate, dateColumn, tableName, masterID),
                    new System.Data.OleDb.OleDbParameter("p0", startPeriod), new System.Data.OleDb.OleDbParameter("p1", endPeriod));
                foreach (DataRow detailRow in detailRows)
                {
                    if (!detailRow.IsNull("CurrencySum"))
                        detailRow["Sum"] = currencyRate * Convert.ToDecimal(detailRow["CurrencySum"]);
                }
            }
            return detailRows;
        }

        /// <summary>
        /// Получаем данные детали по запросу
        /// </summary>
        private static DataRow[] GetDetailData(IDatabase db, string detailQuery, params IDbDataParameter[] queryParams)
        {
            DataTable dtDetailData = (DataTable)db.ExecQuery(detailQuery, QueryResultTypes.DataTable, queryParams);
            return dtDetailData.Select();
        }

        #region запись данных в план ИФ

        private void AddEstimateToPlan(IEntity planEntity, DataTable dtPlan, DataRow detailRow,
            int budgetLevel, int variant, int sourceID, object region,
            int planingStatus, DateTime rowDate)
        {
            // проверяем даты на возможность переноса
            if (!((rowDate > DateTime.Today && rowDate <= new DateTime(DateTime.Today.Year, 12, 31)) && (planingStatus == 0 || planingStatus == 5 || planingStatus == -2)))
                return;

            string period = rowDate.Year + rowDate.Month.ToString().PadLeft(2, '0') + rowDate.Day.ToString().PadLeft(2, '0');

            DataRow[] rows =
                dtPlan.Select(
                    string.Format("FromSF = 1 and RefKif = {0} and RefYearDayUNV = {1} and RefRegions = {2} and Forecast is null and Fact is null",
                    detailRow["RefKif"], period, region));

            if (rows.Length == 0)
            {
                DataRow newRow = dtPlan.NewRow();
                newRow.BeginEdit();
                newRow["TaskID"] = -1;
                newRow["ID"] = GetNewId(planEntity);
                newRow["RefKif"] = detailRow["RefKif"];
                newRow["RefRegions"] = region;
                newRow["RefBudgetLevels"] = budgetLevel;
                newRow["RefSVariant"] = variant;
                if (newRow.Table.Columns.Contains("RefVariant"))
                    newRow["RefVariant"] = -1;
                newRow["SourceID"] = sourceID;
                newRow["RefKVSR"] = -1;
                newRow["RefYearDayUNV"] = period;
                newRow["FromSF"] = 1;
                newRow["Estimate"] = detailRow["Sum"];
                newRow["InterfaceSign"] = ifVariant > 0 ? 1 : 0;
                newRow.EndEdit();
                dtPlan.Rows.Add(newRow);
            }
            else
            {
                decimal sum = Convert.ToDecimal(detailRow["Sum"]);
                sum += rows[0].IsNull("Estimate") ? 0 : Convert.ToDecimal(rows[0]["Estimate"]);
                rows[0]["Estimate"] = sum;
            }
        }

        private void AddForecastToPlan(IEntity planEntity, DataTable dtPlan, DataRow detailRow,
            int budgetLevel, int variant, int sourceID, int currentYear, int lastYear, object region,
            int planingStatus, DateTime rowDate)
        {
            // проверяем даты на возможность переноса
            if (!((rowDate.Year > currentYear && rowDate.Year <= lastYear) && (planingStatus == 0 || planingStatus == 5 || planingStatus == -2)))
                return;

            string period = rowDate.Year + rowDate.Month.ToString().PadLeft(2, '0') + rowDate.Day.ToString().PadLeft(2, '0');

            DataRow[] rows =
                dtPlan.Select(
                    string.Format("FromSF = 1 and RefKif = {0} and RefYearDayUNV = {1} and RefRegions = {2} and Estimate is null and Fact is null",
                    detailRow["RefKif"], period, region));

            if (rows.Length == 0)
            {
                DataRow newRow = dtPlan.NewRow();
                newRow.BeginEdit();
                newRow["TaskID"] = -1;
                newRow["ID"] = GetNewId(planEntity);
                newRow["RefKif"] = detailRow["RefKif"];
                newRow["RefRegions"] = region;
                newRow["RefBudgetLevels"] = budgetLevel;
                newRow["RefSVariant"] = variant;
                if (newRow.Table.Columns.Contains("RefVariant"))
                    newRow["RefVariant"] = -1;
                newRow["SourceID"] = sourceID;
                newRow["RefKVSR"] = -1;
                newRow["RefYearDayUNV"] = period;
                newRow["FromSF"] = 1;
                newRow["Forecast"] = detailRow["Sum"];
                newRow["InterfaceSign"] = ifVariant > 0 ? 1 : 0;
                newRow.EndEdit();
                dtPlan.Rows.Add(newRow);
            }
            else
            {
                decimal sum = Convert.ToDecimal(detailRow["Sum"]);
                sum += rows[0].IsNull("Forecast") ? 0 : Convert.ToDecimal(rows[0]["Forecast"]);
                rows[0]["Forecast"] = sum;
            }
        }

        private void AddEstimateForecastToPlan(IEntity planEntity, DataTable dtPlan, DataRow detailRow,
            int budgetLevel, int variant, int sourceID, int currentYear, int lastYear, object region,
            int planingStatus)
        {
            DateTime date = DateTime.Today;
            if (detailRow.Table.Columns.Contains("DateDoc"))
                date = Convert.ToDateTime(detailRow["DateDoc"]);
            if (detailRow.Table.Columns.Contains("EndDate"))
                date = Convert.ToDateTime(detailRow["EndDate"]);
            if (detailRow.Table.Columns.Contains("StartDate"))
                date = Convert.ToDateTime(detailRow["StartDate"]);
            if (date.Year == currentYear)
                AddEstimateToPlan(planEntity, dtPlan, detailRow, budgetLevel, variant, sourceID, region, planingStatus, date);
            else if (date.Year > currentYear && date.Year <= lastYear)
                AddForecastToPlan(planEntity, dtPlan, detailRow, budgetLevel, variant, sourceID, currentYear, lastYear, region, planingStatus, date);
        }

        private void AddRateSwitchToPlan(IEntity planEntity, DataTable dtPlan, DataRow detailRow,
            int budgetLevel, int variant, int sourceID, int currentYear, int nextYear, object region, int planingStatus)
        {
            DateTime date = DateTime.Today;
            if (detailRow.Table.Columns.Contains("DateDoc"))
                date = Convert.ToDateTime(detailRow["DateDoc"]);
            if (detailRow.Table.Columns.Contains("EndDate"))
                date = Convert.ToDateTime(detailRow["EndDate"]);
            if (detailRow.Table.Columns.Contains("StartDate"))
                date = Convert.ToDateTime(detailRow["StartDate"]);

            if (!IsCorrectData(date, currentYear, nextYear))
                return;
            // для статуса планирования с -1
            if (date.Year == currentYear && date < DateTime.Today)
                AddFactToPlan(planEntity, dtPlan, detailRow, budgetLevel, variant, sourceID, currentYear, region, planingStatus);
            else
                AddEstimateForecastToPlan(planEntity, dtPlan, detailRow,
                    budgetLevel, variant, sourceID, currentYear, nextYear, region, planingStatus);
        }

        private void AddFactToPlan(IEntity planEntity, DataTable dtPlan, DataRow detailRow,
            int budgetLevel, int variant, int sourceID, int currentYear, object region, int planingStatus)
        {
            DateTime date = DateTime.Today;
            if (detailRow.Table.Columns.Contains("DateDoc"))
                date = Convert.ToDateTime(detailRow["DateDoc"]);
            if (detailRow.Table.Columns.Contains("EndDate"))
                date = Convert.ToDateTime(detailRow["EndDate"]);
            if (detailRow.Table.Columns.Contains("StartDate"))
                date = Convert.ToDateTime(detailRow["StartDate"]);
            if (detailRow.Table.Columns.Contains("FactDate"))
                date = Convert.ToDateTime(detailRow["FactDate"]);

            // проверяем можно соответствует ли статус договора и дата записи детали статусу переноса в проект бюджета
            if (!((planingStatus == 2 || planingStatus == 3 || planingStatus == 4) && date.Year == DateTime.Today.Year) &&
                !((planingStatus == 0 || planingStatus == 5) && (date >= new DateTime(DateTime.Today.Year, 1, 1) && date <= DateTime.Today)))
                return;

            string period = date.Year + date.Month.ToString().PadLeft(2, '0') + date.Day.ToString().PadLeft(2, '0');
            DataRow[] rows = dtPlan.Select(
                string.Format("FromSF = 1 and RefKif = {0} and RefYearDayUNV = {1} and RefRegions = {2} and Forecast is null and Estimate is null",
                detailRow["RefKif"], period, region));
            if (rows.Length == 0)
            {
                DataRow newRow = dtPlan.NewRow();
                newRow.BeginEdit();
                newRow["TaskID"] = -1;
                newRow["ID"] = GetNewId(planEntity);
                newRow["RefKif"] = detailRow["RefKif"];
                newRow["RefRegions"] = region;
                newRow["RefBudgetLevels"] = budgetLevel;
                newRow["RefSVariant"] = variant;
                if (newRow.Table.Columns.Contains("RefVariant"))
                    newRow["RefVariant"] = -1;
                newRow["SourceID"] = sourceID;
                newRow["RefKVSR"] = -1;
                newRow["RefYearDayUNV"] = period;
                newRow["Fact"] = detailRow["Sum"];
                newRow["FromSF"] = 1;
                newRow["InterfaceSign"] = ifVariant > 0 ? 1 : 0;
                newRow.EndEdit();
                dtPlan.Rows.Add(newRow);
            }
            else
            {
                decimal sum = Convert.ToDecimal(detailRow["Sum"]);
                if (date.Year == currentYear)
                {
                    sum += rows[0].IsNull("Fact") ? 0 : Convert.ToDecimal(rows[0]["Fact"]);
                    rows[0]["Fact"] = sum;
                }
            }
        }

        #endregion

        #region данные по расходам

        private void AddToChargeData(IEntity chargeEntity, DataTable dtCharge, DataRow detailRow,
            int budgetLevel, int variant, int sourceID, int currentYear, int nextYear, int planingStatus)
        {
            DateTime date = DateTime.Today;
            if (detailRow.Table.Columns.Contains("FactDate"))
                date = Convert.ToDateTime(detailRow["FactDate"]);
            if (detailRow.Table.Columns.Contains("EndDate"))
                date = Convert.ToDateTime(detailRow["EndDate"]);
            if (detailRow.Table.Columns.Contains("StartDate"))
                date = Convert.ToDateTime(detailRow["StartDate"]);

            if (!IsCorrectData(date, currentYear, nextYear))
                return;

            if (!((planingStatus == 0 || planingStatus == 5) && (date.Year >= currentYear && date.Year <= nextYear)))
                return;

            string period = date.Year + date.Month.ToString().PadLeft(2, '0') + date.Day.ToString().PadLeft(2, '0');

            DataRow[] rows = dtCharge.Select(string.Format("FromSF = 1 and RefR = {0} and RefEKR = {1} and RefYearDayUNV = {2} and Summa is null",
                detailRow["RefR"], detailRow["RefEKR"], period));

            if (rows.Length == 0)
            {
                DataRow newRow = dtCharge.NewRow();
                newRow.BeginEdit();
                newRow["TaskID"] = -1;
                newRow["ID"] = GetNewId(chargeEntity);
                newRow["RefR"] = detailRow["RefR"];
                newRow["RefEKR"] = detailRow["RefEKR"];
                newRow["RefRegions"] = -1;
                newRow["RefBdgtLvls"] = budgetLevel;
                newRow["RefKindDebt"] = -1;
                newRow["RefFODepartments"] = -1;
                newRow["KCSR"] = -1;
                newRow["FKR"] = -1;
                newRow["KVR"] = -1;
                newRow["KVSR"] = -1;
                newRow["RefVariant"] = variant;
                newRow["SourceID"] = sourceID;
                newRow["RefYearDayUNV"] = period;
                newRow["FromSF"] = 1;
                if (date.Year >= currentYear && date.Year <= nextYear)
                    newRow["Summa"] = detailRow["Sum"];
                newRow["InterfaceSign"] = ifVariant > 0 ? 1 : 0;
                newRow.EndEdit();
                dtCharge.Rows.Add(newRow);
            }
            else
            {
                // суммируем с существующими данными
                decimal sum = Convert.ToDecimal(detailRow["Sum"]);
                if (date.Year >= currentYear && date.Year <= nextYear)
                {
                    sum += rows[0].IsNull("Summa") ? 0 : Convert.ToDecimal(rows[0]["Summa"]);
                    rows[0]["Summa"] = sum;
                }
            }
        }

        #endregion

        #region данные по доходам

        private void AddToIncomesData(IEntity incomesEntity, DataTable dtIncomes, DataRow detailRow,
            int budgetLevel, int variant, int sourceID, int currentYear, int nextYear, object region)
        {
            DateTime date = DateTime.Today;
            if (detailRow.Table.Columns.Contains("FactDate"))
                date = Convert.ToDateTime(detailRow["FactDate"]);
            if (detailRow.Table.Columns.Contains("EndDate"))
                date = Convert.ToDateTime(detailRow["EndDate"]);
            if (detailRow.Table.Columns.Contains("StartDate"))
                date = Convert.ToDateTime(detailRow["StartDate"]);
            // если год в дате детали больше  или меньше, чем текущий год, то такие данные в таблицу не попадают
            if (!IsCorrectData(date, currentYear, nextYear))
                return;

            string period = date.Year + date.Month.ToString().PadLeft(2, '0') + date.Day.ToString().PadLeft(2, '0');

            DataRow[] rows = date.Year == currentYear
                ?
                dtIncomes.Select(string.Format("FromSF = 1 and RefKD = {0} and RefRegions = {1} and RefYearDayUNV = {2} and Forecast is null",
                    detailRow["RefKD"], region, period))
                :
                dtIncomes.Select(string.Format("FromSF = 1 and RefKD = {0} and RefRegions = {1} and RefYearDayUNV = {2} and Estimate is null",
                    detailRow["RefKD"], region, period));

            if (rows.Length == 0)
            {
                DataRow newRow = dtIncomes.NewRow();
                newRow.BeginEdit();
                newRow["TaskID"] = -1;
                newRow["ID"] = GetNewId(incomesEntity);
                newRow["RefKD"] = detailRow["RefKD"];
                newRow["RefRegions"] = region;
                newRow["RefBudLevel"] = budgetLevel;
                newRow["RefFODepartments"] = -1;
                newRow["RefVariant"] = variant;
                newRow["SourceID"] = sourceID;
                newRow["RefKVSR"] = -1;
                newRow["RefOrganizations"] = -1;
                newRow["RefTaxObjects"] = -1;
                newRow["RefYearDayUNV"] = period;
                newRow["FromSF"] = 1;
                if (date.Year == currentYear)
                    newRow["Estimate"] = detailRow["Sum"];
                else if (date.Year <= nextYear)
                    newRow["Forecast"] = detailRow["Sum"];
                newRow["InterfaceSign"] = ifVariant > 0 ? 1 : 0;
                newRow.EndEdit();
                dtIncomes.Rows.Add(newRow);
            }
            else
            {
                // суммируем с существующими данными
                decimal sum = Convert.ToDecimal(detailRow["Sum"]);
                if (date.Year == currentYear)
                {
                    sum += rows[0].IsNull("Estimate") ? 0 : Convert.ToDecimal(rows[0]["Estimate"]);
                    rows[0]["Estimate"] = sum;
                }
                else if (date.Year == nextYear)
                {
                    sum += rows[0].IsNull("Forecast") ? 0 : Convert.ToDecimal(rows[0]["Forecast"]);
                    rows[0]["Forecast"] = sum;
                }
            }
        }

        #endregion

        private object GetNewId(IEntity entity)
        {
            if (string.Compare(scheme.SchemeDWH.FactoryName, "System.Data.SqlClient", true) == 0)
                return DBNull.Value;
            return entity.GetGeneratorNextValue;
        }
    }
}