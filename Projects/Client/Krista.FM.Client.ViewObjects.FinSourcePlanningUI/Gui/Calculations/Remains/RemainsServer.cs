using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Server;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Calculations.Remains
{
    internal class RemainsServer : CalculationServerBase
    {
        internal RemainsServer(IScheme scheme)
            : base(scheme)
        {
            entity = scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.f_S_RemainsDesign);
        }

        internal DataRow[] CalculateRemains(int sourceID, int borrowVariant, int incomesVariant, int outcomesVariant)
        {
            // перед расчетами переносим данные в IF
            BudgetTransfert budgetTransfert = new BudgetTransfert(scheme);
            IClassifiersProtocol protocol = (IClassifiersProtocol)scheme.GetProtocol("Workplace.exe");
            budgetTransfert.TransfertData(incomesVariant, outcomesVariant, borrowVariant, sourceID, Utils.GetBudgetLevel(scheme), ServerLibrary.FinSourcePlanning.BudgetTransfertOption.IfPart, protocol);
            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                DataRow[] resultRows = new DataRow[3];
                for (int i = 1; i <= 3; i++)
                {
                    resultRows[i-1] = CalculateRemains4Year(sourceID, borrowVariant, incomesVariant, outcomesVariant,
                                                          DateTime.Today.Year + i, db);
                }
                return resultRows;
            }
        }

        private DataRow CalculateRemains4Year(int sourceId, int finSourceVariant, int incomesVariant, int chargeVariant, int year, IDatabase db)
        {
            List<string> errors = new List<string>();
            int budgetLevel = GetRegionType();
            decimal income = GetIncomes(sourceId, incomesVariant, budgetLevel, year, db);

            decimal charge = GetOutcomes(chargeVariant, sourceId, year, budgetLevel, db);

            string constCode = GetConstCode("KIFStockSale", ref errors, 3, 6);
            decimal safetyStock = GetResultIFDirection(finSourceVariant, sourceId, year, budgetLevel, constCode, db);
            constCode = GetConstCode("KIFSurplusBalances", ref errors, 3, 4);

            decimal issueCapital = GetConstsIfSum(finSourceVariant, sourceId, year, budgetLevel, db, ref errors,
                                                  "KIFCapital", "KIFCapitalForgn");
            decimal dischargeCapital = GetConstsIfSum(finSourceVariant, sourceId, year, budgetLevel, db, ref errors,
                                                      "KIFRetireCapital", "KIFRetireCapitalForgn");
            //Получение кредитов от кредитных организаций
            decimal receiptCredit = GetConstsIfSum(finSourceVariant, sourceId, year, budgetLevel, db, ref errors,
                                                   "KIFCILendAgnc", "KIFCILendAgncForgn");
            decimal repayCredit = GetConstsIfSum(finSourceVariant, sourceId, year, budgetLevel, db, ref errors,
                                                 "KIFCILendAgncRepay", "KIFCILendAgncForgnRepay");
            decimal credit = receiptCredit - repayCredit;
            decimal receiptBudgCredit = GetResultIF(finSourceVariant, sourceId, year, budgetLevel,
                                                    GetConstCode("KIFCIBudg", ref errors), db);
            decimal repayBudgCredit = GetResultIF(finSourceVariant, sourceId, year, budgetLevel,
                                                  GetConstCode("KIFCIBudgRepay", ref errors), db);
            //Предоставление бюджетных кредитов и ссуд
            decimal creditExtensionBudget = GetConstsIfSum(finSourceVariant, sourceId, year, budgetLevel, db, ref errors,
                                                           "KIFCOBudgExt", "KIFCOBudgExtMR", "KIFCOBudgExtPos");
            decimal creditExtensionPerson = GetResultIF(finSourceVariant, sourceId, year, budgetLevel,
                                                        GetConstCode("KIFCOBudgExtPerson", ref errors), db);
            //Возврат бюджетных кредитов и ссуд
            decimal creditReturnBudget = GetConstsIfSum(finSourceVariant, sourceId, year, budgetLevel, db, ref errors,
                                                        "KIFCOBudgReturn", "KIFCOBudgReturnMR", "KIFCOBudgReturnPos");
            decimal creditReturnPerson = GetResultIF(finSourceVariant, sourceId, year, budgetLevel,
                                                     GetConstCode("KIFCOReturnPerson", ref errors), db);
            decimal nameGuarantee = GetResultIF(finSourceVariant, sourceId, year, budgetLevel,
                                                GetConstCode("KIFGrnt", ref errors), db);

            decimal remainsAccretion = income + receiptCredit + receiptBudgCredit + issueCapital + creditReturnBudget +
                                       creditReturnPerson + safetyStock;
            decimal remainsRecession = charge + repayCredit + repayBudgCredit + dischargeCapital + creditExtensionBudget +
                                       creditExtensionPerson + nameGuarantee;
            decimal remainsChange = remainsAccretion - remainsRecession;

            using (IDataUpdater du = entity.GetDataUpdater())
            {
                DataTable dt = new DataTable();
                du.Fill(ref dt);
                DataRow newRow = dt.NewRow();
                newRow.BeginEdit();
                newRow["SourceID"] = sourceId;
                newRow["TaskID"] = -1;
                newRow["ID"] = GetNewId(entity);
                newRow["RefYearDayUNV"] = string.Format("{0}0001", year);
                newRow["RefBrwVariant"] = finSourceVariant;
                newRow["RefIncVariant"] = incomesVariant;
                newRow["RefRVariant"] = chargeVariant;

                newRow["CreditReturnPerson"] = creditReturnPerson;
                newRow["SafetyStock"] = safetyStock;
                newRow["CreditExtensionPerson"] = creditExtensionPerson;
                newRow["NameGuarantee"] = nameGuarantee;
                newRow["Income"] = income;
                newRow["ReceiptCredit"] = receiptCredit;
                newRow["ReceiptBudgCredit"] = receiptBudgCredit;
                newRow["IssueCapital"] = issueCapital;
                newRow["CreditReturnBudget"] = creditReturnBudget;
                newRow["DischargeCapital"] = dischargeCapital;
                newRow["Charge"] = charge;
                newRow["RepayCredit"] = repayCredit;
                newRow["RepayBudgCredit"] = repayBudgCredit;
                newRow["CreditExtensionBudget"] = creditExtensionBudget;
                newRow["RemainsAccretion"] = remainsAccretion;
                newRow["RemainsRecession"] = remainsRecession;
                newRow["RemainsChange"] = remainsChange;

                newRow.EndEdit();
                if (newRow.RowState == DataRowState.Detached)
                    dt.Rows.Add(newRow);

                return newRow;
            }
        }

        public static void AddNewIfRow(IScheme scheme, int refKif, decimal value, int variant, int period, int budLevel, int sourceId)
        {
            IEntity entity = scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.f_S_Plan_Key);
            using (IDataUpdater du = entity.GetDataUpdater("1 = 2", null))
            {
                DataTable dataTable = new DataTable();
                du.Fill(ref dataTable);
                DataRow newRow = dataTable.NewRow();
                newRow["RefSVariant"] = variant;
                newRow["RefBudgetLevels"] = period;
                newRow["RefRegions"] = -1;
                newRow["Forecast"] = value;
                newRow["RefKIF"] = refKif;
                newRow["InterfaceSign"] = 3;
                newRow["FromSF"] = 1;
                newRow["RefBudgetLevels"] = budLevel;
                newRow["RefYearDayUNV"] = period;
                newRow["RefKVSR"] = -1;
                newRow["TaskID"] = -1;
                newRow["SourceID"] = sourceId;
                dataTable.Rows.Add(newRow);
                du.Update(ref dataTable);
            }
        }
    }

    internal class RemainsReport
    {
        private readonly IScheme scheme;

        public RemainsReport(IScheme scheme)
        {
            this.scheme = scheme;
        }

        /// <summary>
        /// получаем данные для отчета по одной записи
        /// </summary>
        internal DataTable[] GetReportData(DataTable dtRemainsData)
        {
            DataTable[] reportData = new DataTable[2];
            DataTable dtReportData = new DataTable();
            dtReportData.Columns.Add("Income", typeof(Decimal));
            dtReportData.Columns.Add("ReceiptCredit", typeof(Decimal));
            dtReportData.Columns.Add("ReceiptBudgCredit", typeof(Decimal));
            dtReportData.Columns.Add("IssueCapital", typeof(Decimal));
            dtReportData.Columns.Add("CreditReturnBudget", typeof(Decimal));
            dtReportData.Columns.Add("CreditReturnPerson", typeof(Decimal));
            dtReportData.Columns.Add("SafetyStock", typeof(Decimal));
            dtReportData.Columns.Add("RemainsAccretion", typeof(Decimal));
            dtReportData.Columns.Add("Charge", typeof(Decimal));
            dtReportData.Columns.Add("RepayCredit", typeof(Decimal));
            dtReportData.Columns.Add("RepayBudgCredit", typeof(Decimal));
            dtReportData.Columns.Add("DischargeCapital", typeof(Decimal));
            dtReportData.Columns.Add("CreditExtensionBudget", typeof(Decimal));
            dtReportData.Columns.Add("CreditExtensionPerson", typeof(Decimal));
            dtReportData.Columns.Add("NameGuarantee", typeof(Decimal));
            dtReportData.Columns.Add("RemainsRecession", typeof(Decimal));
            dtReportData.Columns.Add("RemainsChange", typeof(Decimal));
            dtReportData.Columns.Add("RefYearDayUNV", typeof(Int32));

            string incomesVariantStr = string.Empty;
            string chargeVariantStr = string.Empty;
            string ifVariantStr = string.Empty;


            IEntity variantIncomesEntity = scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.variantIncomes_Key);
            IEntity variantChargeEntity = scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.variantCharge_Key);
            IEntity variantIFEntity = scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.d_Variant_Borrow_Key);

            DataView view = new DataView(dtRemainsData);
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
            //newRow[0] = view.ToTable().Rows[0]["RefYearDayUNV"].ToString().Substring(0, 4) + " - " + view.ToTable().Rows[3]["RefYearDayUNV"].ToString().Substring(0, 4);
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
