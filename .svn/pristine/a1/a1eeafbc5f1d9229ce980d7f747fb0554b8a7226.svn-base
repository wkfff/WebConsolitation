using System;
using System.Collections.Generic;
using System.Data;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Server;
using Krista.FM.Client.Workplace.Gui;

using Krista.FM.Common;
using Krista.FM.ServerLibrary;
using Krista.FM.ServerLibrary.FinSourcePlanning;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Server.VolumeHoldings
{
    public class BorrowingVolumeServer
    {
        readonly IScheme scheme;
        readonly FinSourcesRererencesUtils finSourcesRererencesUtils;

        private IEntity budIncomesDataEntity;
        private IEntity budChargeDataEntity;
        
        public BorrowingVolumeServer(IScheme scheme)
        {
            this.scheme = scheme;
            finSourcesRererencesUtils = new FinSourcesRererencesUtils(scheme);
        }

        public DataRow[] VolumeHoldingResults(int sourceID, int borrowVariant, int incomesVariant, int outcomesVariant,
            decimal euroRate, decimal dollarRate, BorrowingVolumeBudgetType budgetDataType)
        {
            BudgetTransfert budgetTransfert = new BudgetTransfert(scheme);
            IClassifiersProtocol protocol = (IClassifiersProtocol)WorkplaceSingleton.Workplace.ActiveScheme.GetProtocol("Workplace.exe");
            budgetTransfert.TransfertData(incomesVariant, outcomesVariant, borrowVariant,
                sourceID, Utils.GetBudgetLevel(scheme), euroRate, dollarRate, BudgetTransfertOption.IfPart, protocol);

            DataRow[] resultRows = new DataRow[4];
            for (int i = 0; i <= 3; i++)
            {
                resultRows[i] = CalculateBorrowingVolume(sourceID, borrowVariant, incomesVariant, outcomesVariant, DateTime.Today.Year + i, budgetDataType);
            }
            return resultRows;
        }

        public DataRow CalculateBorrowingVolume(int sourceID, int borrowVariant, int incomesVariant, int outcomesVariant, int year, BorrowingVolumeBudgetType budgetDataType)
        {
            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                List<string> errors = new List<string>();
                IEntity volumeHoldingsEntity = scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.f_S_VolumeHoldings_Key);
                budIncomesDataEntity = scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.f_D_IncomesData);
                budChargeDataEntity = scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.f_R_BudgetData);
                int budgetLevel = GetRegionType();
                decimal income = GetIncomes(sourceID, incomesVariant, budgetLevel, year, budgetDataType, db);
                decimal subvention = GetIncomes(sourceID, incomesVariant, budgetLevel, year, "Codestr like '___20203%'", budgetDataType, db);
                decimal uncompensatReceipts = GetIncomes(sourceID, incomesVariant, budgetLevel, year, "Codestr like '___2%'", budgetDataType, db);
                decimal charge = GetOutcomes(outcomesVariant, sourceID, year, budgetLevel, budgetDataType, db);
                decimal deficit = income - charge;
                string constCode = GetConstCode("KIFStockSale", ref errors, 3, 6);
                decimal safetyStock = GetResultIFDirection(borrowVariant, sourceID, year, budgetLevel, constCode);
                constCode = GetConstCode("KIFSurplusBalances", ref errors, 3, 4);
                decimal remainsChange = 0;//GetResultIFDirection(borrowVariant, sourceID, year, budgetLevel, constCode);
                decimal issueCapital = GetConstsIfSum(borrowVariant, sourceID, year, budgetLevel, ref errors,
                    "KIFCapital", "KIFCapitalForgn");
                decimal dischargeCapital = GetConstsIfSum(borrowVariant, sourceID, year, budgetLevel, ref errors,
                    "KIFRetireCapital", "KIFRetireCapitalForgn");
                decimal capital = issueCapital - dischargeCapital;
                //Получение кредитов от кредитных организаций
                decimal receiptCredit = GetConstsIfSum(borrowVariant, sourceID, year, budgetLevel, ref errors,
                    "KIFCILendAgnc", "KIFCILendAgncForgn");
                decimal repayCredit = GetConstsIfSum(borrowVariant, sourceID, year, budgetLevel, ref errors,
                    "KIFCILendAgncRepay", "KIFCILendAgncForgnRepay");
                decimal credit = receiptCredit - repayCredit;
                decimal receiptBudgCredit = GetResultIF(borrowVariant, sourceID, year, budgetLevel, GetConstCode("KIFCIBudg", ref errors));
                decimal repayBudgCredit = GetResultIF(borrowVariant, sourceID, year, budgetLevel, GetConstCode("KIFCIBudgRepay", ref errors));
                decimal budgetCredit = receiptBudgCredit - repayBudgCredit;
                //Предоставление бюджетных кредитов и ссуд
                decimal creditExtensionBudget = GetConstsIfSum(borrowVariant, sourceID, year, budgetLevel, ref errors,
                    "KIFCOBudgExt", "KIFCOBudgExtMR", "KIFCOBudgExtPos");
                decimal creditExtensionPerson = GetResultIF(borrowVariant, sourceID, year, budgetLevel, GetConstCode("KIFCOBudgExtPerson", ref errors));
                //Возврат бюджетных кредитов и ссуд
                decimal creditReturnBudget = GetConstsIfSum(borrowVariant, sourceID, year, budgetLevel, ref errors,
                    "KIFCOBudgReturn", "KIFCOBudgReturnMR", "KIFCOBudgReturnPos");
                decimal creditReturnPerson = GetResultIF(borrowVariant, sourceID, year, budgetLevel, GetConstCode("KIFCOReturnPerson", ref errors));
                decimal nameGuarantee = GetResultIF(borrowVariant, sourceID, year, budgetLevel, GetConstCode("KIFGrnt", ref errors));
                decimal borrowing = creditReturnBudget + creditReturnPerson - creditExtensionPerson - creditExtensionBudget;
                decimal volumeHoldings =
                    -(income - charge + safetyStock + remainsChange + capital + credit + budgetCredit + borrowing - nameGuarantee);

                using (IDataUpdater du = volumeHoldingsEntity.GetDataUpdater())
                {
                    DataTable dt = new DataTable();
                    du.Fill(ref dt);
                    DataRow newRow = dt.NewRow();
                    newRow.BeginEdit();
                    newRow["SourceID"] = sourceID;
                    newRow["TaskID"] = -1;
                    newRow["ID"] = GetNewId(volumeHoldingsEntity);
                    newRow["RefYearDayUNV"] = string.Format("{0}0001", year);
                    newRow["RefBrwVariant"] = borrowVariant;
                    newRow["RefIncVariant"] = incomesVariant;
                    newRow["RefRVariant"] = outcomesVariant;
                    newRow["Income"] = income;
                    newRow["Subvention"] = subvention;
                    newRow["UncompensatReceipts"] = uncompensatReceipts;
                    newRow["Charge"] = charge;
                    newRow["Deficit"] = deficit;
                    newRow["SafetyStock"] = safetyStock;
                    newRow["RemainsChange"] = remainsChange;
                    newRow["Capital"] = capital;
                    newRow["IssueCapital"] = issueCapital;
                    newRow["DischargeCapital"] = dischargeCapital;
                    newRow["Credit"] = credit;
                    newRow["ReceiptCredit"] = receiptCredit;
                    newRow["RepayCredit"] = repayCredit;
                    newRow["BudgetCredit"] = budgetCredit;
                    newRow["ReceiptBudgCredit"] = receiptBudgCredit;
                    newRow["RepayBudgCredit"] = repayBudgCredit;
                    newRow["Borrowing"] = borrowing;
                    newRow["CreditExtensionBudget"] = creditExtensionBudget;
                    newRow["CreditExtensionPerson"] = creditExtensionPerson;
                    newRow["CreditReturnBudget"] = creditReturnBudget;
                    newRow["CreditReturnPerson"] = creditReturnPerson;
                    newRow["VolumeHoldings"] = volumeHoldings;
                    newRow["NameGuarantee"] = nameGuarantee;
                    newRow["IssueCapitalPlan"] = 0;
                    newRow["ReceiptCreditPlan"] = 0;
                    newRow["ReceiptBudgCreditPlan"] = 0;
                    newRow["NonBorrow"] = volumeHoldings;
                    newRow.EndEdit();
                    if (newRow.RowState == DataRowState.Detached)
                        dt.Rows.Add(newRow);

                    return newRow;
                }
            }
        }

        /// <summary>
        /// список уникальных кодов для набора констант (коды у разных констант могут быть одинаковыми)
        /// </summary>
        /// <param name="errors"></param>
        /// <param name="constNames"></param>
        /// <returns></returns>
        private List<string> GetConstUniqueCodes(ref List<string> errors, params string[] constNames)
        {
            List<string> constCodes = new List<string>();
            foreach (string constName in constNames)
            {
                string code = GetConstCode(constName, ref errors);
                if (!constCodes.Contains(code))
                    constCodes.Add(code);
            }
            return constCodes;
        }

        /// <summary>
        /// сумма значений для набора кодов из констант
        /// </summary>
        /// <returns></returns>
        private decimal GetConstsIfSum(int borrowVariant, int sourceId, int year, int budgetLevel, ref List<string> errors, params string[] constNames)
        {
            List<string> codes = GetConstUniqueCodes(ref errors, constNames);
            decimal sum = 0;
            foreach (string code in codes)
            {
                sum += GetResultIF(borrowVariant, sourceId, year, budgetLevel, code);
            }
            return sum;
        }

        private int GetRegionType()
        {
            int regionType = -1;
            if (scheme.GlobalConstsManager.Consts.ContainsKey("TerrPartType"))
                regionType = Utils.GetBudgetLevel(Convert.ToInt32(scheme.GlobalConstsManager.Consts["TerrPartType"].Value));
            return regionType;
        }

        private string GetConstCode(string constName, ref List<string> errors)
        {
            object[] constValues = finSourcesRererencesUtils.GetConstDataByName(constName);
            if (constValues == null)
            {
                errors.Add(string.Format("Константа с идентификатором '{0}' не найдена в справочнике констант для ИФ", constName));
                return string.Empty;
            }
            return finSourcesRererencesUtils.GetConstDataByName(constName)[0].ToString();
        }

        private string GetConstCode(string constName, ref List<string> errors, int codePartIndex, int codePartLength)
        {
            string constCode = GetConstCode(constName, ref errors);
            if (!string.IsNullOrEmpty(constCode))
                return constCode.Substring(codePartIndex, codePartLength);
            return null;
        }

        /// <summary>
        /// доходы с учетом кода доходов
        /// </summary>
        private Decimal GetIncomes(int sourceID, int variant, int budgetLevel, int year, string kdCodesFilter, BorrowingVolumeBudgetType budgetDataType, IDatabase db)
        {
            if (year == DateTime.Today.Year)
            {
                return GetBudgetIncomesData(year, kdCodesFilter, budgetDataType, db); 
            }

            IEntity entity = scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.f_D_FOPlanIncDivide);
            IEntity kdEntity = scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.d_KD_Plan_Key);
            decimal result = 0;

            string planKDQuery = string.Format("Select ID from {0} where ({1})", kdEntity.FullDBName, kdCodesFilter);

            string query = string.Format("Select Sum({0}) from {1} where SourceID = {2} and RefVariant = {3} and RefYearDayUNV like '{4}____' and RefKD IN ({5}) and RefBudLevel = {6}",
                year == DateTime.Today.Year ? "Estimate" : "Forecast",
                entity.FullDBName, sourceID, variant, year, planKDQuery, budgetLevel);

            object objResult = db.ExecQuery(query, QueryResultTypes.Scalar);
            if (!(objResult is DBNull))
                result = Convert.ToDecimal(objResult);
            return result;
        }

        /// <summary>
        /// доходы
        /// </summary>
        private Decimal GetIncomes(int sourceID, int variant, int budgetLevel, int year, BorrowingVolumeBudgetType budgetDataType, IDatabase db)
        {
            if (year == DateTime.Today.Year)
            {
                return GetBudgetIncomesData(year, string.Empty, budgetDataType, db);
            }
            IEntity entity = scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.f_D_FOPlanIncDivide);
            decimal result = 0;
                
            string query = string.Format("Select Sum({0}) from {1} where SourceID = {2} and RefVariant = {3} and RefBudLevel = {4} and RefYearDayUNV like '{5}____'",
                year == DateTime.Today.Year ? "Estimate" : "Forecast",
                entity.FullDBName, sourceID, variant, budgetLevel, year);

            object objResult = db.ExecQuery(query, QueryResultTypes.Scalar);
            if (!(objResult is DBNull))
                result = Convert.ToDecimal(objResult);
            return result;
        }

        /// <summary>
        /// расходы
        /// </summary>
        /// <param name="variant"></param>
        /// <param name="sourceID"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        private Decimal GetOutcomes(int variant, int sourceID, int year, int budgetLevel, BorrowingVolumeBudgetType budgetDataType, IDatabase db)
        {
            if (year == DateTime.Today.Year)
                return GetBudgetChargeData(year, budgetDataType, db);

            IEntity entity = scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.f_R_FO26R);
            decimal result = 0;

            string query = string.Format(
                    "select Sum(Summa) from {0} where SourceID = {1} and RefVariant = {2} and RefBdgtLvls = {3} and RefYearDayUNV like '{4}____'",
                    entity.FullDBName, sourceID, variant, budgetLevel, year);

            object objResult = db.ExecQuery(query, QueryResultTypes.Scalar);
            if (!(objResult is DBNull))
                result = Convert.ToDecimal(objResult);
            return result;
        }

        /// <summary>
        /// получение результата ИФ по разным кодам КИФ
        /// </summary>
        private Decimal GetResultIF(int variant, int sourceID, int year, int budgetLevel, string kifCode)
        {
            if (string.IsNullOrEmpty(kifCode))
                return 0;
            decimal result = 0;
            IEntity kifEntity = scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.d_KIF_Plan_Key);
            IEntity planIFEntity = scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.f_S_Plan_Key);
            using (IDatabase db = WorkplaceSingleton.Workplace.ActiveScheme.SchemeDWH.DB)
            {
                string kifSelect = string.Format("select id from {0} where CodeStr = '{1}' and SourceID = {2}",
                    kifEntity.FullDBName, kifCode, sourceID);
                object kif = db.ExecQuery(kifSelect, QueryResultTypes.Scalar);
                string query = string.Format("select Sum({0}) from {1} where SourceID = {2} and RefSVariant = {3} and RefBudgetLevels = {4} and RefYearDayUNV like '{5}____' and RefKIF in ({6})",
                    year == DateTime.Today.Year ? "COALESCE(Estimate, 0) + COALESCE(Fact, 0)" : "Forecast",
                    planIFEntity.FullDBName, sourceID, variant, budgetLevel, year, kifSelect);
                object objResult = db.ExecQuery(query, QueryResultTypes.Scalar);
                if (!(objResult is DBNull))
                    result = Convert.ToDecimal(objResult);
            }
            return result;
        }

        private decimal GetResultIFDirection(int variant, int sourceID, int year, int budgetLevel, string kifCode)
        {
            if (string.IsNullOrEmpty(kifCode))
                return 0;
            decimal result = 0;
            IEntity kifEntity = scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.d_KIF_Plan_Key);
            IEntity planIFEntity = scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.f_S_Plan_Key);
            using (IDatabase db = WorkplaceSingleton.Workplace.ActiveScheme.SchemeDWH.DB)
            {
                string query = string.Format(
                    @"select Sum(ifResult.{0}*kifPlan.RefKIF) from {1} ifResult, {2} kifPlan 
                    where kifPlan.ID in (select ID from {2} where CodeStr like '___{6}%') 
                    and ifResult.RefSVariant = {3} and ifResult.SourceID = {4} and ifResult.RefYearDayUNV like '{5}____' 
                    and ifResult.RefKIF = kifPlan.ID 
                    and ifResult.REFBUDGETLEVELS = {7}",
                    year == DateTime.Today.Year ? "Estimate" : "Forecast",                      
                    planIFEntity.FullDBName, kifEntity.FullDBName, variant, sourceID, year, kifCode, budgetLevel);

                object objResult = db.ExecQuery(query, QueryResultTypes.Scalar);
                if (!(objResult is DBNull))
                    result = Convert.ToDecimal(objResult);
            }
            return result;
        }

        private object GetNewId(IEntity entity)
        {
            if (string.Compare(scheme.SchemeDWH.FactoryName, "System.Data.SqlClient", true) == 0)
                return DBNull.Value;
            return entity.GetGeneratorNextValue;
        }


        private decimal GetBudgetChargeData(int year, BorrowingVolumeBudgetType budgetDataType, IDatabase db)
        {
            if (budIncomesDataEntity == null)
                return 0;

            string dateUNV = string.Format("{0}____", year);
            string query = string.Format("select Sum(Summe) from {0} where RefDateUNV like ? and RefYearDayUNV = ?",
                                         budChargeDataEntity.FullDBName);
            decimal result = 0;
            if (budgetDataType == BorrowingVolumeBudgetType.QuarterCashPlan)
            {
                for (int i = 1; i <= 4; i++)
                {
                    int refYearDayUNV = year*10000 + 9990 + i;
                    object queryResult = db.ExecQuery(query, QueryResultTypes.Scalar,
                                                      new DbParameterDescriptor("p0", dateUNV),
                                                      new DbParameterDescriptor("p1", refYearDayUNV));
                    if (queryResult != null && queryResult != DBNull.Value)
                        result += Convert.ToDecimal(queryResult);
                }
            }

            if (budgetDataType == BorrowingVolumeBudgetType.MonthCashPlan)
            {
                for (int i = 1; i <= 12; i++)
                {
                    int refYearDayUNV = year*10000 + i*100 + 00;
                    object queryResult = db.ExecQuery(query, QueryResultTypes.Scalar,
                                                      new DbParameterDescriptor("p0", dateUNV),
                                                      new DbParameterDescriptor("p1", refYearDayUNV));
                    if (queryResult != null && queryResult != DBNull.Value)
                        result += Convert.ToDecimal(queryResult);
                }
            }

            if (budgetDataType == BorrowingVolumeBudgetType.BudgetList)
            {
                int refYearDayUNV = year * 10000 + 1;
                object queryResult = db.ExecQuery(query, QueryResultTypes.Scalar,
                                                  new DbParameterDescriptor("p0", dateUNV),
                                                  new DbParameterDescriptor("p1", refYearDayUNV));
                if (queryResult != null && queryResult != DBNull.Value)
                    return Convert.ToDecimal(queryResult);
            }
            return result; ;
        }

        /// <summary>
        /// данные из AC Бюджет
        /// </summary>
        /// <param name="year"></param>
        /// <param name="kdCodesFilter"></param>
        /// <param name="isCashPlan"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        private decimal GetBudgetIncomesData(int year, string kdCodesFilter, BorrowingVolumeBudgetType budgetDataType, IDatabase db)
        {
            if (budIncomesDataEntity == null)
                return 0;

            string dateUNV = string.Format("{0}____", year);

            string query = string.Format("select Sum(Summa) from {0} where RefDateUNV like ? and RefYearDayUNV = ?", budIncomesDataEntity.FullDBName);
            if (!string.IsNullOrEmpty(kdCodesFilter))
            {
                query = query +
                        string.Format(" and RefKDASBudget in (select id from d_KD_ASBudget where {0})", kdCodesFilter);
            }
            decimal result = 0;
            if (budgetDataType == BorrowingVolumeBudgetType.BudgetList)
            {
                int refYearDayUNV = year * 10000 + 1;
                object queryResult = db.ExecQuery(query, QueryResultTypes.Scalar,
                                                  new DbParameterDescriptor("p0", dateUNV),
                                                  new DbParameterDescriptor("p1", refYearDayUNV));
                if (queryResult != null && queryResult != DBNull.Value)
                    return Convert.ToDecimal(queryResult);
            }
            if (budgetDataType == BorrowingVolumeBudgetType.MonthCashPlan)
            {
                for (int i = 1; i <= 12; i++)
                {
                    int refYearDayUNV = year * 10000 + i * 100 + 00;
                    object queryResult = db.ExecQuery(query, QueryResultTypes.Scalar,
                                                      new DbParameterDescriptor("p0", dateUNV),
                                                      new DbParameterDescriptor("p1", refYearDayUNV));
                    if (queryResult != null && queryResult != DBNull.Value)
                        result += Convert.ToDecimal(queryResult);
                }
            }
            if (budgetDataType == BorrowingVolumeBudgetType.QuarterCashPlan)
            {
                for (int i = 1; i <= 4; i++)
                {
                    int refYearDayUNV = year*10000 + 9990 + i;
                    object queryResult = db.ExecQuery(query, QueryResultTypes.Scalar,
                                 new DbParameterDescriptor("p0", dateUNV),
                                 new DbParameterDescriptor("p1", refYearDayUNV));
                    if (queryResult != null && queryResult != DBNull.Value)
                        result += Convert.ToDecimal(queryResult);
                }
            }

            return result;
        }
    }

    public class VolumeHoldingsReport
    {
        private readonly IScheme scheme;

        public VolumeHoldingsReport(IScheme scheme)
        {
            this.scheme = scheme;
        }

        /// <summary>
        /// получаем данные для отчета по одной записи
        /// </summary>
        public DataTable[] GetReportData(DataTable dtBorrowingData)
        {
            DataTable[] reportData = new DataTable[2];
            DataTable dtReportData = new DataTable();
            dtReportData.Columns.Add("Income", typeof(Decimal));
            dtReportData.Columns.Add("Subvention", typeof(Decimal));
            dtReportData.Columns.Add("UncompensatReceipts", typeof(Decimal));
            dtReportData.Columns.Add("Charge", typeof(Decimal));
            dtReportData.Columns.Add("Deficit", typeof(Decimal));
            dtReportData.Columns.Add("SafetyStock", typeof(Decimal));
            dtReportData.Columns.Add("RemainsChange", typeof(Decimal));
            dtReportData.Columns.Add("Capital", typeof(Decimal));
            dtReportData.Columns.Add("IssueCapital", typeof(Decimal));
            dtReportData.Columns.Add("DischargeCapital", typeof(Decimal));
            dtReportData.Columns.Add("Credit", typeof(Decimal));
            dtReportData.Columns.Add("ReceiptCredit", typeof(Decimal));
            dtReportData.Columns.Add("RepayCredit", typeof(Decimal));
            dtReportData.Columns.Add("BudgetCredit", typeof(Decimal));
            dtReportData.Columns.Add("ReceiptBudgCredit", typeof(Decimal));
            dtReportData.Columns.Add("RepayBudgCredit", typeof(Decimal));
            dtReportData.Columns.Add("Borrowing", typeof(Decimal));
            dtReportData.Columns.Add("CreditExtensionBudget", typeof(Decimal));
            dtReportData.Columns.Add("CreditExtensionPerson", typeof(Decimal));
            dtReportData.Columns.Add("CreditReturnBudget", typeof(Decimal));
            dtReportData.Columns.Add("CreditReturnPerson", typeof(Decimal));
            dtReportData.Columns.Add("NameGuarantee", typeof (Decimal));
            dtReportData.Columns.Add("VolumeHoldings", typeof(Decimal));
            dtReportData.Columns.Add("IssueCapitalPlan", typeof(Decimal));
            dtReportData.Columns.Add("ReceiptCreditPlan", typeof(Decimal));
            dtReportData.Columns.Add("ReceiptBudgCreditPlan", typeof(Decimal));
            dtReportData.Columns.Add("NonBorrow", typeof(Decimal));
            dtReportData.Columns.Add("RefYearDayUNV", typeof(Int32));

            string incomesVariantStr = string.Empty;
            string chargeVariantStr = string.Empty;
            string ifVariantStr = string.Empty;


            IEntity variantIncomesEntity = scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.variantIncomes_Key);
            IEntity variantChargeEntity = scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.variantCharge_Key);
            IEntity variantIFEntity = scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.d_Variant_Borrow_Key);

            DataView view = new DataView(dtBorrowingData);
            view.Sort = "RefYearDayUNV ASC";
            view.ToTable();

            DataRow newRow = null;
            foreach (DataRow row in view.ToTable().Rows)
            {
                newRow = dtReportData.NewRow();
                foreach (DataColumn clmn in dtReportData.Columns)
                {
                    if (string.Compare(clmn.ColumnName, "RefYearDayUNV", true) == 0)
                        newRow[clmn] = Convert.ToInt32(row[clmn.ColumnName].ToString().Substring(0, 4));
                    else
                        newRow[clmn] = row[clmn.ColumnName];
                }
                dtReportData.Rows.Add(newRow);

                using (IDataUpdater duVariant = variantIncomesEntity.GetDataUpdater(string.Format("ID = {0}", row["RefIncVariant"]), null))
                {
                    DataTable dtVariant = new DataTable();
                    duVariant.Fill(ref dtVariant);
                    incomesVariantStr = variantIncomesEntity.OlapName + string.Format(".{0}", dtVariant.Rows[0]["Name"]);
                }
                using (IDataUpdater duVariant = variantChargeEntity.GetDataUpdater(string.Format("ID = {0}", row["RefRVariant"]), null))
                {
                    DataTable dtVariant = new DataTable();
                    duVariant.Fill(ref dtVariant);
                    chargeVariantStr = variantChargeEntity.OlapName + string.Format(".{0}", dtVariant.Rows[0]["Name"]);
                }
                using (IDataUpdater duVariant = variantIFEntity.GetDataUpdater(string.Format("ID = {0}", row["RefBrwVariant"]), null))
                {
                    DataTable dtVariant = new DataTable();
                    duVariant.Fill(ref dtVariant);
                    ifVariantStr = variantIFEntity.OlapName + string.Format(".{0}", dtVariant.Rows[0]["Name"]);
                }
            }
            DataTable dtVariantsYear = new DataTable();
            dtVariantsYear.Columns.Add("Year");
            dtVariantsYear.Columns.Add("IncomesVariant");
            dtVariantsYear.Columns.Add("ChargeVariant");
            dtVariantsYear.Columns.Add("IFVariant");
            newRow = dtVariantsYear.NewRow();
            newRow[0] = view.ToTable().Rows[0]["RefYearDayUNV"].ToString().Substring(0, 4) + " - " + view.ToTable().Rows[3]["RefYearDayUNV"].ToString().Substring(0, 4);
            newRow[1] = incomesVariantStr;
            newRow[2] = chargeVariantStr;
            newRow[3] = ifVariantStr;
            dtVariantsYear.Rows.Add(newRow);

            reportData[0] = dtReportData;
            reportData[1] = dtVariantsYear;

            return reportData;
        }
    }
}
