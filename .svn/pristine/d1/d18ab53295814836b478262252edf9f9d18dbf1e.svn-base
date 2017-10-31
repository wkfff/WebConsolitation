using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Krista.FM.Client.ViewObjects.ConsBudgetForecastUI.Common;
using Krista.FM.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.ConsBudgetForecastUI.Server
{
    enum FactTableKind
    {
        Incomes = 0,
        Charge = 1,
        IF = 2
    }

    public enum FixedRowIndex
    {
        /// <summary>
        /// ИТОГО ДОХОДОВ
        /// </summary>
        Incomes = 0,
        /// <summary>
        /// Дотации бюджетам субъектов РФ и муниципальных образований
        /// </summary>
        BudgetDotation = 1,
        /// <summary>
        /// Областной фонд финансовой поддержки  поселений
        /// </summary>
        SettlementsRegionalFund = 2,
        /// <summary>
        /// Областной фонд финансовой поддержки муниципальных районов (городских округов)» 
        /// </summary>
        RegionRegionalFund = 3,
        /// <summary>
        /// Субсидии  бюджетам субъектов РФ и муниципальных образований
        /// </summary>
        BudgetSubsidy = 4,
        /// <summary>
        /// Субвенции бюджетам субъектов РФ и муниципальных образований
        /// </summary>
        BudgetSubvention = 5,
        /// <summary>
        /// Иные межбюджетные трансферты
        /// </summary>
        OtherBudgetTransferts = 6,
        /// <summary>
        /// ВСЕГО ДОХОДОВ
        /// </summary>
        TotalIncomes = 7,
        /// <summary>
        /// ВСЕГО РАСХОДОВ
        /// </summary>
        TotalCharge = 8,
        /// <summary>
        /// Превышение доходов над расходами (дефицит)
        /// </summary>
        Deficit = 9,
        /// <summary>
        /// Разрыв
        /// </summary>
        Fault = 10,
        /// <summary>
        /// Источники внутреннего финансирования дефицита бюджета
        /// </summary>
        FinSources = 11,
        /// <summary>
        /// Межбюджетные трансферты
        /// </summary>
        IntergovernmentalTransfers = 12,
        /// <summary>
        /// ИСТОЧНИКИ ВНУТРЕННЕГО ФИНАНСИРОВАНИЯ ДЕФИЦИТОВ  БЮДЖЕТОВ
        /// </summary>
        Sources = 13,
        /// <summary>
        /// налоговые и неналоговые
        /// </summary>
        TaxNoTax = 14,
        /// <summary>
        /// безвозмездные поступления (в части фед.ср.)
        /// </summary>
        FreeSupply = 15,
        /// <summary>
        /// приносящая доход деятельность
        /// </summary>
        IncomeWork = 16,
        /// <summary>
        /// данные по отдельному коду дохода
        /// </summary>
        IncomeCode = 17
    }

    public struct BalanceParams
    {
        public int Year
        {
            get;
            set;
        }

        public object IncomesVariant
        {
            get;
            set;
        }

        public object ChargeVariant
        {
            get;
            set;
        }

        public object IFVariant
        {
            get;
            set;
        }

        public int VariantYear
        {
            get;
            set;
        }
    }

    public class BalanceService
    {
        private IScheme Scheme
        {
            get;
            set;
        }

        private object SourceId
        {
            get;
            set;
        }

        public BalanceService(IScheme scheme)
        {
            Scheme = scheme;
            DataColumns = new List<string>();
        }

        private List<string> DataColumns
        {
            get;
            set;
        }

        public Dictionary<string, string> Regions
        {
            get;
            set;
        }

        private DataTable RegionsData
        {
            get;
            set;
        }

        public DataTable GetData(BalanceParams balanceParams, BalanceIncomeParams incomeParams)
        {
            using (IDatabase db = Scheme.SchemeDWH.DB)
            {
                DataColumns.Clear();
                GetSourceId(db, balanceParams.VariantYear);
                Regions = GetRegionNames(db);
                RegionsData = GetRegions(db);
                DataTable dtBalance = GetEmptyTable();

                GetIncomeRow(dtBalance, string.Empty, "ИТОГО ДОХОДОВ", FixedRowIndex.Incomes, FactTableKind.Incomes);
                var dtTaxNoTax = GetIncomesData(db, balanceParams, "1");
                FillIncomesData(dtTaxNoTax, string.Empty, "Налоговые и неналоговые",FixedRowIndex.TaxNoTax, ref dtBalance);
                if (incomeParams.TaxNotaxCodes.Count > 0)
                {
                    // добавляем строки по выбранным кодам по налоговым и неналоговым
                    foreach (var codeStr in incomeParams.TaxNotaxCodes)
                    {
                        dtTaxNoTax = GetIncomesData4Code(db, balanceParams, codeStr);
                        FillIncomesData(dtTaxNoTax, codeStr, incomeParams.TaxNotaxNames[codeStr], FixedRowIndex.IncomeCode, ref dtBalance);
                    }
                }
                var freeSupply = GetIncomesData(db, balanceParams, "2");
                FillIncomesData(freeSupply, string.Empty, "Безвозмездные поступления (в части фед.ср.)", FixedRowIndex.FreeSupply, ref dtBalance);
                if (incomeParams.FreeSupplyCodes.Count > 0)
                {
                    // добавляем строки по выбранным кодам по безвозмездным поступлениям
                    foreach (var codeStr in incomeParams.FreeSupplyCodes)
                    {
                        freeSupply = GetIncomesData4Code(db, balanceParams, codeStr);
                        FillIncomesData(freeSupply, codeStr, incomeParams.FreeSupplyNames[codeStr], FixedRowIndex.IncomeCode, ref dtBalance);
                    }
                }
                var incomeWork = GetIncomesData(db, balanceParams, "3");
                FillIncomesData(incomeWork, string.Empty, "Приносящая доход деятельность", FixedRowIndex.IncomeWork, ref dtBalance);
                if (incomeParams.IncomeWorkCodes.Count > 0)
                {
                    // добавляем строки по выбранным кодам по приносящей доход деятельности
                    foreach (var codeStr in incomeParams.IncomeWorkCodes)
                    {
                        incomeWork = GetIncomesData4Code(db, balanceParams, codeStr);
                        FillIncomesData(incomeWork, codeStr, incomeParams.IncomeWorkNames[codeStr], FixedRowIndex.IncomeCode, ref dtBalance);
                    }
                }
                // Областной фонд финансовой поддержки  поселений
                DataTable dtCharge = GetKCSRLayer(db, balanceParams, 5160110, 8);
                FillChargeData(dtCharge, ref dtBalance);
                // Областной фонд финансовой поддержки муниципальных районов (городских округов)
                DataTable dKCSRLayer = GetKCSRLayer(db, balanceParams, 5160120, 8);
                FillKCSRLayer(dKCSRLayer, ref dtBalance, FixedRowIndex.RegionRegionalFund);
                dKCSRLayer = GetKCSRLayer(db, balanceParams, -1, 7);
                FillKCSRLayer(dKCSRLayer, ref dtBalance, FixedRowIndex.RegionRegionalFund);
                // Субсидии  бюджетам субъектов РФ и муниципальных образований
                dKCSRLayer = GetKCSRLayer(db, balanceParams, -1, 10);
                FillKCSRLayer(dKCSRLayer, ref dtBalance, FixedRowIndex.BudgetSubsidy);
                dKCSRLayer = GetKCSRLayer(db, balanceParams, 4400202, 17);
                FillKCSRLayer(dKCSRLayer, ref dtBalance, FixedRowIndex.BudgetSubsidy);
                // Субвенции бюджетам субъектов РФ и муниципальных образований
                dKCSRLayer = GetKCSRLayer(db, balanceParams, -1, 9);
                FillKCSRLayer(dKCSRLayer, ref dtBalance, FixedRowIndex.BudgetSubvention);
                // Иные межбюджетные трансферты
                dKCSRLayer = GetKCSRLayer(db, balanceParams, -1, 17);
                FillKCSRLayer(dKCSRLayer, ref dtBalance, FixedRowIndex.OtherBudgetTransferts);
                // всего расходов
                DataTable dtTotalCharges = GetChargeResult(db, balanceParams);
                FillTotalChargeData(dtTotalCharges, ref dtBalance);

                DataTable dtIF = GetIfData(db, balanceParams);
                DataTable dtKif = GetKifData(db);
                FillIFData(dtIF, dtKif, ref dtBalance);

                SetResults(ref dtBalance);
                SetRowResults(ref dtBalance);

                return dtBalance;
            }
        }

        private DataTable GetIncomesData(IDatabase db, BalanceParams balanceParams, string codeFilter)
        {
            var queryParams = new List<DbParameterDescriptor>();
            string query =
                    @"select Forecast, RefRegions, RefBudLevel from f_D_FOPlanIncDivide where RefVariant = ? and RefYearDayUNV like ? and 
                    RefKD in (select id from d_KD_PlanIncomes where SourceId = ? and CodeStr like ?)";
            queryParams.Add(new DbParameterDescriptor("p0", balanceParams.IncomesVariant));
            queryParams.Add(new DbParameterDescriptor("p1", string.Format("{0}____", balanceParams.Year)));
            queryParams.Add(new DbParameterDescriptor("p2", SourceId));
            queryParams.Add(new DbParameterDescriptor("p3", string.Format("___{0}%", codeFilter)));
            switch (codeFilter)
            {
                case "2":
                    query += "and RefBudLevel = 3";
                    break;
            }
            var dt = (DataTable)db.ExecQuery(query, QueryResultTypes.DataTable, queryParams.ToArray());
            return dt;
        }

        /// <summary>
        /// данные по доходам в разрезе одного кода
        /// </summary>
        /// <param name="db"></param>
        /// <param name="balanceParams"></param>
        /// <param name="codeStr"></param>
        /// <returns></returns>
        private DataTable GetIncomesData4Code(IDatabase db, BalanceParams balanceParams, string codeStr)
        {
            var queryParams = new List<DbParameterDescriptor>();
            string query =
                    @"select Forecast, RefRegions, RefBudLevel from f_D_FOPlanIncDivide where RefVariant = ? and RefYearDayUNV like ? and 
                    RefKD in (select id from d_KD_PlanIncomes where SourceId = ? and CodeStr = ?)";
            queryParams.Add(new DbParameterDescriptor("p0", balanceParams.IncomesVariant));
            queryParams.Add(new DbParameterDescriptor("p1", string.Format("{0}____", balanceParams.Year)));
            queryParams.Add(new DbParameterDescriptor("p2", SourceId));
            queryParams.Add(new DbParameterDescriptor("p3", codeStr));
            string codePart = codeStr.Substring(3, 1);
            switch (codePart)
            {
                case "2":
                    query += "and RefBudLevel = 3";
                    break;
            }
            var dt = (DataTable)db.ExecQuery(query, QueryResultTypes.DataTable, queryParams.ToArray());
            return dt;
        }

        #region данные из таблицы расходов

        private DataTable GetChargeResult(IDatabase db, BalanceParams balanceParams)
        {
            string query =
                @"select Summa, RefRegions, RefBdgtLvls from f_R_FO26R where RefVariant = ? and RefYearDayUNV like ?";
            IDbDataParameter[] queryParams = new IDbDataParameter[2];
            queryParams[0] = new DbParameterDescriptor("p0", balanceParams.ChargeVariant);
            queryParams[1] = new DbParameterDescriptor("p1", string.Format("{0}____", balanceParams.Year));
            DataTable dt = (DataTable)db.ExecQuery(query, QueryResultTypes.DataTable, queryParams);
            return dt;
        }

        private DataTable GetKCSRLayer(IDatabase db, BalanceParams balanceParams, int kcsrCode, int kvrCode)
        {
            if (kcsrCode >= 0 && kvrCode >= 0)
            {
                string query =
                    @"select charge.Summa, charge.RefRegions, charge.RefBdgtLvls, kcsr.Code as KCSR, kcsr.Name, kvr.Code as kvr from f_R_FO26R charge, d_KCSR_PlanOutcomes kcsr, d_KVR_PlanOutcomes kvr where 
                charge.RefVariant = ? and charge.RefYearDayUNV like ? and
                charge.KCSR in (select id from d_KCSR_PlanOutcomes where Code = ?) and 
                charge.KVR in (select id from d_KVR_PlanOutcomes where Code = ?) and charge.RefBdgtLvls = 3 and
                charge.KCSR = kcsr.id and charge.KVR = kvr.id";
                IDbDataParameter[] queryParams = new IDbDataParameter[4];
                queryParams[0] = new DbParameterDescriptor("p0", balanceParams.ChargeVariant);
                queryParams[1] = new DbParameterDescriptor("p1", string.Format("{0}____", balanceParams.Year));
                queryParams[2] = new DbParameterDescriptor("p2", kcsrCode);
                queryParams[3] = new DbParameterDescriptor("p3", kvrCode);
                return (DataTable)db.ExecQuery(query, QueryResultTypes.DataTable, queryParams);
            }
            if (kcsrCode >= 0)
            {
                string query =
                @"select charge.Summa, charge.RefRegions, charge.RefBdgtLvls, kcsr.Code as KCSR, kcsr.Name, kvr.Code as kvr from f_R_FO26R charge, d_KCSR_PlanOutcomes kcsr, d_KVR_PlanOutcomes kvr where 
                charge.RefVariant = ? and charge.RefYearDayUNV like ? and
                charge.KCSR in (select id from d_KCSR_PlanOutcomes where Code = ?)
                and charge.RefBdgtLvls = 3 and charge.KCSR = kcsr.id and charge.KVR = kvr.id";
                IDbDataParameter[] queryParams = new IDbDataParameter[3];
                queryParams[0] = new DbParameterDescriptor("p0", balanceParams.ChargeVariant);
                queryParams[1] = new DbParameterDescriptor("p1", string.Format("{0}____", balanceParams.Year));
                queryParams[2] = new DbParameterDescriptor("p2", kcsrCode);
                return (DataTable)db.ExecQuery(query, QueryResultTypes.DataTable, queryParams);
            }
            if (kvrCode >= 0)
            {
                string query =
                @"select charge.Summa, charge.RefRegions, charge.RefBdgtLvls, kcsr.Code as KCSR, kcsr.Name, kvr.Code as kvr from f_R_FO26R charge, d_KCSR_PlanOutcomes kcsr, d_KVR_PlanOutcomes kvr where 
                charge.RefVariant = ? and charge.RefYearDayUNV like ? and
                charge.KVR in (select id from d_KVR_PlanOutcomes where Code = ?)
                and charge.RefBdgtLvls = 3 and charge.KCSR = kcsr.id and charge.KVR = kvr.id";
                IDbDataParameter[] queryParams = new IDbDataParameter[3];
                queryParams[0] = new DbParameterDescriptor("p0", balanceParams.ChargeVariant);
                queryParams[1] = new DbParameterDescriptor("p1", string.Format("{0}____", balanceParams.Year));
                queryParams[2] = new DbParameterDescriptor("p2", kvrCode);
                return (DataTable)db.ExecQuery(query, QueryResultTypes.DataTable, queryParams);
            }
            return null;
        }

        #endregion

        #region данные по источникам финансирования

        private DataTable GetIfData(IDatabase db, BalanceParams balanceParams)
        {
            string query =
                @"select ifplan.Forecast, ifplan.RefBudgetLevels, ifplan.RefRegions, kif.ID, kif.ParentID, kif.CodeStr as Code, kif.Name, kif.RefKif 
                from f_S_Plan ifplan, d_KIF_Plan kif where ifplan.RefSVariant = ? and ifplan.RefYearDayUNV like ?
                and ifplan.RefKIF = kif.ID order by kif.CodeStr";
            IDbDataParameter[] queryParams = new IDbDataParameter[2];
            queryParams[0] = new DbParameterDescriptor("p0", balanceParams.IFVariant);
            queryParams[1] = new DbParameterDescriptor("p1", string.Format("{0}____", balanceParams.Year));

            DataTable dt = (DataTable)db.ExecQuery(query, QueryResultTypes.DataTable, queryParams);
            return dt;
        }

        private DataTable GetKifData(IDatabase db)
        {
            string query = "select ID, ParentID, CodeStr, Name, RefKif from d_KIF_Plan where SourceID = ? order by CodeStr";
            return (DataTable)db.ExecQuery(query, QueryResultTypes.DataTable, new DbParameterDescriptor("p0", SourceId));
        }

        #endregion

        private void GetSourceId(IDatabase db, int year)
        {
            object sourceID = db.ExecQuery(
                "select ID from DataSources where SupplierCode = ? and DataCode = ? and Year = ? and deleted = 0",
                QueryResultTypes.Scalar,
                 new DbParameterDescriptor("SupplierCode", "ФО"),
                 new DbParameterDescriptor("DataCode", "0029"),
                 new DbParameterDescriptor("Year", year));

            if (sourceID == null || sourceID == DBNull.Value)
            {
                IDataSource ds = Scheme.DataSourceManager.DataSources.CreateElement();
                ds.SupplierCode = "ФО";
                ds.DataCode = "0029";
                ds.DataName = "Проект бюджета";
                ds.Year = year;
                ds.ParametersType = ParamKindTypes.Year;
                sourceID = ds.Save();
            }
            SourceId = sourceID;
        }

        private Dictionary<string, string> GetRegionNames(IDatabase db)
        {
            DataTable dtRegions = GetRegions(db);
            Dictionary<string, string> result = new Dictionary<string, string>();
            foreach (DataRow row in dtRegions.Select("RefTerr = 7 or RefTerr = 4"))
            {
                result.Add(row[0].ToString(), row[1].ToString());
            }

            result.Add("ResultMR", "Итого по МР");
            result.Add("ResultGO", "Итого по ГО");

            DataRow[] region = dtRegions.Select("RefTerr = 3");
            if (region.Length > 0)
            {
                result.Add(region[0][0].ToString(), region[0][1].ToString());
            }
            result.Add("Result", "ВСЕГО");
            return result;
        }

        private DataTable GetRegions(IDatabase db)
        {
            IEntity d_Regions_Plan = Scheme.RootPackage.FindEntityByName(ObjectKeys.d_Regions_Plan);
            DataTable dtRegions = (DataTable)db.ExecQuery(
            string.Format(
                "select reg.id, reg.name, reg.RefTerr from {0} reg where reg.SourceID = ? and (reg.RefTerr = 3 or reg.RefTerr = 4 or reg.RefTerr = 7) order by reg.Code",
                d_Regions_Plan.FullDBName), QueryResultTypes.DataTable,
            new DbParameterDescriptor("p0", SourceId));
            return dtRegions;
        }

        private DataTable GetEmptyTable()
        {
            DataTable dtEmpty = new DataTable();
            DataColumn column = dtEmpty.Columns.Add("Id", typeof(decimal));
            column.Caption = string.Empty;
            column = dtEmpty.Columns.Add("ParentId", typeof(decimal));
            column.Caption = string.Empty;
            column = dtEmpty.Columns.Add("SortColumn", typeof(string));
            column.Caption = string.Empty;
            column = dtEmpty.Columns.Add("Code", typeof(string));
            column.Caption = "Код";
            column = dtEmpty.Columns.Add("Name", typeof(string));
            column.Caption = "Наименование";
            column = dtEmpty.Columns.Add("KCSR", typeof(int));
            column.Caption = string.Empty;
            column = dtEmpty.Columns.Add("KVR", typeof(int));
            column.Caption = string.Empty;
            column = dtEmpty.Columns.Add("Index", typeof(decimal));
            column.Caption = string.Empty;
            column = dtEmpty.Columns.Add("FactTableIndex", typeof(string));
            column.Caption = string.Empty;
            foreach (DataRow regionRow in RegionsData.Rows)
            {
                int terrType;
                if (!regionRow.IsNull("RefTerr") && int.TryParse(regionRow["RefTerr"].ToString(), out terrType))
                {
                    switch (terrType)
                    {
                        case 4:
                            column = dtEmpty.Columns.Add(regionRow["id"] + "_4", typeof(decimal));
                            column.Caption = "КБМР";
                            DataColumns.Add(column.ColumnName);
                            column = dtEmpty.Columns.Add(regionRow["id"] + "_5", typeof(decimal));
                            column.Caption = "МР";
                            DataColumns.Add(column.ColumnName);
                            column = dtEmpty.Columns.Add(regionRow["id"] + "_6", typeof(decimal));
                            column.Caption = "Всего по поселениям";
                            DataColumns.Add(column.ColumnName);
                            break;
                        case 7:
                            column = dtEmpty.Columns.Add(regionRow["id"] + "_15", typeof(decimal));
                            column.Caption = "Бюджет ГО";
                            DataColumns.Add(column.ColumnName);
                            break;
                    }
                }
            }

            column = dtEmpty.Columns.Add("ResultMR_Cons", typeof(decimal));
            column.Caption = "Консолидированный бюджет муниципального района";
            DataColumns.Add(column.ColumnName);
            column = dtEmpty.Columns.Add("ResultMR_Reg", typeof(decimal));
            column.Caption = "Муниципальный район";
            DataColumns.Add(column.ColumnName);
            column = dtEmpty.Columns.Add("ResultMR_Settl", typeof(decimal));
            column.Caption = "Всего по поселениям";
            DataColumns.Add(column.ColumnName);

            column = dtEmpty.Columns.Add("ResultGO_GO", typeof(decimal));
            column.Caption = "Бюджет ГО";
            DataColumns.Add(column.ColumnName);

            DataRow[] region = RegionsData.Select("RefTerr = 3");
            if (region.Length > 0)
            {
                column = dtEmpty.Columns.Add(string.Format("{0}_{1}", region[0]["ID"], 3), typeof(decimal));
                column.Caption = "Бюджет субъекта";
                DataColumns.Add(column.ColumnName);
            }

            column = dtEmpty.Columns.Add("Result_Result", typeof(decimal));
            column.Caption = "Итого";
            DataColumns.Add(column.ColumnName);

            return dtEmpty;
        }

        private void FillIncomesData(DataTable dtIncomes, string codeStr, string rowName,FixedRowIndex rowIndex, ref DataTable dtData)
        {
            // ИТОГО ДОХОДОВ
            DataRow newRow = GetIncomeRow(dtData, codeStr, rowName, rowIndex, FactTableKind.Incomes);
            foreach (string columnName in DataColumns)
            {
                newRow[columnName] = 0;
                string[] columnParams = columnName.Split('_');
                long regionId;
                if (long.TryParse(columnParams[0], out regionId))
                {
                    int budLevel = Convert.ToInt32(columnParams[1]);
                    if (budLevel == 6)
                    {
                        foreach (DataRow incomesRow in dtIncomes.Rows.Cast<DataRow>().Where(w => Convert.ToInt64(w["RefRegions"]) == regionId &&
                        (Convert.ToInt32(w["RefBudLevel"]) == budLevel || Convert.ToInt32(w["RefBudLevel"]) == 16 || Convert.ToInt32(w["RefBudLevel"]) == 17)))
                        {
                            decimal value = newRow.IsNull(columnName) ? 0 : Convert.ToDecimal(newRow[columnName]);
                            value += incomesRow.IsNull("Forecast") ? 0 : Convert.ToDecimal(incomesRow["Forecast"]);
                            newRow[columnName] = value;
                        }
                    }
                    else if (budLevel == 3)
                    {
                        foreach (DataRow incomesRow in dtIncomes.Rows.Cast<DataRow>().Where(w => Convert.ToInt32(w["RefBudLevel"]) == budLevel))
                        {
                            decimal value = newRow.IsNull(columnName) ? 0 : Convert.ToDecimal(newRow[columnName]);
                            value += incomesRow.IsNull("Forecast") ? 0 : Convert.ToDecimal(incomesRow["Forecast"]);
                            newRow[columnName] = value;
                        }
                    }
                    else
                    {
                        foreach (DataRow incomesRow in dtIncomes.Rows.Cast<DataRow>().Where(w => Convert.ToInt64(w["RefRegions"]) == regionId &&
                        Convert.ToInt32(w["RefBudLevel"]) == budLevel))
                        {
                            decimal value = newRow.IsNull(columnName) ? 0 : Convert.ToDecimal(newRow[columnName]);
                            value += incomesRow.IsNull("Forecast") ? 0 : Convert.ToDecimal(incomesRow["Forecast"]);
                            newRow[columnName] = value;
                        }
                    }
                }
            }
        }

        private void FillChargeData(DataTable dtCharge, ref DataTable dtData)
        {
            // Межбюджетные трансферты
            DataRow intergovernmentalRow = dtData.NewRow();
            intergovernmentalRow["Name"] = "Межбюджетные трансферты";
            intergovernmentalRow["Index"] = FixedRowIndex.IntergovernmentalTransfers;
            SetZeroData(intergovernmentalRow);
            dtData.Rows.Add(intergovernmentalRow);

            DataRow budgetDotationRow = dtData.NewRow();
            // строка Дотации бюджетам субъектов РФ и муниципальных образований
            budgetDotationRow["Name"] = "Дотации бюджетам субъектов РФ и муниципальных образований";
            budgetDotationRow["Index"] = FixedRowIndex.BudgetDotation;
            SetZeroData(budgetDotationRow);
            dtData.Rows.Add(budgetDotationRow);
            DataRow settlementsRegionalFundRow = dtData.NewRow();
            // Областной фонд финансовой поддержки  поселений
            settlementsRegionalFundRow["Name"] = "Областной фонд финансовой поддержки  поселений";
            settlementsRegionalFundRow["Index"] = FixedRowIndex.SettlementsRegionalFund;
            SetZeroData(settlementsRegionalFundRow);
            dtData.Rows.Add(settlementsRegionalFundRow);

            foreach (string columnName in DataColumns)
            {
                //newRow[columnName] = 0;
                string[] columnParams = columnName.Split('_');
                long regionId;
                if (long.TryParse(columnParams[0], out regionId))
                {
                    int budLevel = Convert.ToInt32(columnParams[1]);
                    if (budLevel == 5 || budLevel == 15)
                        budLevel = 3;
                    foreach (DataRow incomesRow in dtCharge.Rows.Cast<DataRow>().Where(w => Convert.ToInt64(w["RefRegions"]) == regionId &&
                        Convert.ToInt32(w["RefBdgtLvls"]) == budLevel))
                    {
                        int kcsr = Convert.ToInt32(incomesRow["KCSR"]);
                        int kvr = Convert.ToInt32(incomesRow["KVR"]);
                        DataRow kcsrRow = GetKCSRRow(dtData, kcsr, kvr, incomesRow["Name"].ToString());

                        decimal value = kcsrRow.IsNull(columnName) ? 0 : Convert.ToDecimal(kcsrRow[columnName]);
                        value += incomesRow.IsNull("Summa") ? 0 : Convert.ToDecimal(incomesRow["Summa"]);
                        kcsrRow[columnName] = value;
                        settlementsRegionalFundRow[columnName] = value;
                    }
                }
            }
        }

        private void FillKCSRLayer(DataTable dtKCSR, ref DataTable dtData, FixedRowIndex fixedRowIndex)
        {
            // добавляем результирующие строки
            DataRow resultRow = GetResultRow(ref dtData, fixedRowIndex);

            foreach (string columnName in DataColumns)
            {
                string[] columnParams = columnName.Split('_');
                long regionId;
                if (long.TryParse(columnParams[0], out regionId))
                {
                    int budLevel = Convert.ToInt32(columnParams[1]);
                    if (budLevel == 5 || budLevel == 15)
                        budLevel = 3;
                    foreach (DataRow incomesRow in dtKCSR.Rows.Cast<DataRow>().Where(w => Convert.ToInt64(w["RefRegions"]) == regionId &&
                        Convert.ToInt32(w["RefBdgtLvls"]) == budLevel))
                    {
                        int kcsr = Convert.ToInt32(incomesRow["KCSR"]);
                        int kvr = Convert.ToInt32(incomesRow["KVR"]);
                        DataRow kcsrRow = GetKCSRRow(dtData, kcsr, kvr, incomesRow["Name"].ToString());
                        decimal value = Convert.ToDecimal(incomesRow["Summa"]);
                        kcsrRow[columnName] = kcsrRow.IsNull(columnName) ? value : Convert.ToDecimal(kcsrRow[columnName]) + value;
                        resultRow[columnName] = resultRow.IsNull(columnName)
                                                    ? value
                                                    : Convert.ToDecimal(resultRow[columnName]) + value;
                    }
                }
            }
        }

        private void FillTotalChargeData(DataTable dtTotalCharge, ref DataTable dtData)
        {
            DataRow newRow = dtData.NewRow();
            newRow["Name"] = "ВСЕГО ДОХОДОВ";
            newRow["Index"] = (int)FixedRowIndex.TotalIncomes;
            SetZeroData(newRow);
            dtData.Rows.Add(newRow);

            newRow = dtData.NewRow();
            newRow["Name"] = "ВСЕГО РАСХОДОВ";
            newRow["Index"] = (int)FixedRowIndex.TotalCharge;
            foreach (string columnName in DataColumns)
            {
                newRow[columnName] = 0;
                string[] columnParams = columnName.Split('_');
                long regionId;
                if (long.TryParse(columnParams[0], out regionId))
                {
                    int budLevel = Convert.ToInt32(columnParams[1]);
                    if (budLevel == 3)
                    {
                        foreach (DataRow incomesRow in dtTotalCharge.Rows.Cast<DataRow>().Where(w => Convert.ToInt32(w["RefBdgtLvls"]) == budLevel))
                        {
                            decimal value = newRow.IsNull(columnName) ? 0 : Convert.ToDecimal(newRow[columnName]);
                            value += incomesRow.IsNull("Summa") ? 0 : Convert.ToDecimal(incomesRow["Summa"]);
                            newRow[columnName] = value;
                        }
                    }
                    else
                    {
                        foreach (DataRow incomesRow in dtTotalCharge.Rows.Cast<DataRow>().Where(
                                    w => Convert.ToInt64(w["RefRegions"]) == regionId && Convert.ToInt32(w["RefBdgtLvls"]) == budLevel))
                        {
                            decimal value = newRow.IsNull(columnName) ? 0 : Convert.ToDecimal(newRow[columnName]);
                            value += incomesRow.IsNull("Summa") ? 0 : Convert.ToDecimal(incomesRow["Summa"]);
                            newRow[columnName] = value;
                        }
                    }
                }
            }
            dtData.Rows.Add(newRow);
            // строка дефицит
            newRow = dtData.NewRow();
            newRow["Name"] = "Дефицит";
            newRow["Index"] = (int)FixedRowIndex.Deficit;
            SetZeroData(newRow);
            dtData.Rows.Add(newRow);
            // строка Разрыв
            newRow = dtData.NewRow();
            newRow["Name"] = "Разрыв";
            newRow["Index"] = (int)FixedRowIndex.Fault;
            SetZeroData(newRow);
            dtData.Rows.Add(newRow);
        }

        private void SetZeroData(DataRow row)
        {
            foreach (string columnName in DataColumns)
            {
                row[columnName] = 0;
            }
        }

        private void FillIFData(DataTable dtIF, DataTable dtKif, ref DataTable dtData)
        {
            foreach (string columnName in DataColumns)
            {
                string[] columnParams = columnName.Split('_');
                long regionId;
                if (long.TryParse(columnParams[0], out regionId))
                {
                    int budLevel = Convert.ToInt32(columnParams[1]);
                    if (budLevel == 3)
                    {
                        foreach (DataRow ifRow in dtIF.Rows.Cast<DataRow>().Where(w => Convert.ToInt32(w["RefBudgetLevels"]) == budLevel))
                        {
                            int direction = ifRow.IsNull("RefKif") ? 1 : Convert.ToInt32(ifRow["RefKif"]);
                            if (direction == 0) direction = 1;
                            string code = ifRow["Code"].ToString();
                            string name = ifRow["Name"].ToString();
                            decimal value = (ifRow.IsNull("Forecast") ? 0 : Convert.ToDecimal(ifRow["Forecast"]));
                            if (!ifRow.IsNull("ParentID"))
                            {
                                long parentID = Convert.ToInt64(ifRow["ParentID"]);
                                FillParentIFData(dtKif, parentID, value * direction, columnName, ref dtData);
                            }
                            DataRow newRow = GetIFRow(dtData, code, name, FactTableKind.IF);
                            newRow[columnName] = newRow.IsNull(columnName) ? value : Convert.ToDecimal(newRow[columnName]) + value;
                        }
                    }
                    else
                    {
                        foreach (DataRow ifRow in dtIF.Rows.Cast<DataRow>().Where(w => Convert.ToInt64(w["RefRegions"]) == regionId &&
                        Convert.ToInt32(w["RefBudgetLevels"]) == budLevel))
                        {
                            int direction = ifRow.IsNull("RefKif") ? 1 : Convert.ToInt32(ifRow["RefKif"]);
                            if (direction == 0) direction = 1;
                            string code = ifRow["Code"].ToString();
                            string name = ifRow["Name"].ToString();
                            decimal value = (ifRow.IsNull("Forecast") ? 0 : Convert.ToDecimal(ifRow["Forecast"]));
                            if (!ifRow.IsNull("ParentID"))
                            {
                                long parentID = Convert.ToInt64(ifRow["ParentID"]);
                                FillParentIFData(dtKif, parentID, value * direction, columnName, ref dtData);
                            }
                            DataRow newRow = GetIFRow(dtData, code, name, FactTableKind.IF);
                            newRow[columnName] = newRow.IsNull(columnName) ? value : Convert.ToDecimal(newRow[columnName]) + value;
                        }
                    }
                }
            }
        }

        private void FillParentIFData(DataTable dtKif, long parentID, decimal childValue, string columnName, ref DataTable dtData)
        {
            DataRow[] parentRows = dtKif.Select(string.Format("ID = {0}", parentID));
            DataRow parentRow = parentRows[0];
            string code = parentRow["CodeStr"].ToString();
            string name = parentRow["Name"].ToString();
            int direction = parentRow.IsNull("RefKif") ? 0 : Convert.ToInt32(parentRow["RefKif"]);
            if (!parentRow.IsNull("ParentID"))
            {
                long ifParentId = Convert.ToInt64(parentRow["ParentID"]);
                FillParentIFData(dtKif, ifParentId, childValue, columnName, ref dtData);
            }
            DataRow newRow = GetIFRow(dtData, code, name, FactTableKind.IF);
            decimal childDirectionValue = direction == 0 ? childValue : Math.Abs(childValue);
            decimal parentValue = newRow.IsNull(columnName) ? 0 : Convert.ToDecimal(newRow[columnName]) + childDirectionValue;
            newRow[columnName] = parentValue;
            if (parentRow.IsNull("ParentID"))
                newRow["Index"] = (int)FixedRowIndex.FinSources;
        }

        private DataRow GetKCSRRow(DataTable dataTable, int kcsr, int kvr, string name)
        {
            DataRow[] rows = dataTable.Select(string.Format("KCSR = {0} and KVR = {1}", kcsr, kvr));
            if (rows.Length != 0)
                return rows[0];
            DataRow newRow = dataTable.NewRow();
            newRow["KCSR"] = kcsr;
            newRow["KVR"] = kvr;
            newRow["Name"] = name;
            newRow["FactTableIndex"] = FactTableKind.Charge;
            SetZeroData(newRow);
            dataTable.Rows.Add(newRow);
            return newRow;
        }

        private DataRow GetIncomeRow(DataTable dataTable,string codeStr, string name, FixedRowIndex rowIndex, FactTableKind factTableIndex)
        {
            DataRow[] rows = dataTable.Select(string.Format("Name = '{0}' and FactTableIndex = '{1}'", name, factTableIndex));
            if (rows.Length != 0)
                return rows[0];
            DataRow newRow = dataTable.NewRow();
            if (!string.IsNullOrEmpty(codeStr))
                newRow["Code"] = codeStr;
            newRow["Name"] = name;
            newRow["Name"] = name;
            newRow["FactTableIndex"] = factTableIndex;
            newRow["Index"] = (int)rowIndex;
            SetZeroData(newRow);
            dataTable.Rows.Add(newRow);
            return newRow;
        }

        private DataRow GetIFRow(DataTable dataTable, string code, string name, FactTableKind factTableIndex)
        {
            DataRow[] rows = dataTable.Select(string.Format("Code = {0} and FactTableIndex = '{1}'", code, factTableIndex));
            if (rows.Length != 0)
                return rows[0];
            DataRow newRow = dataTable.NewRow();
            newRow["Code"] = code;
            newRow["Name"] = name;
            newRow["FactTableIndex"] = factTableIndex;
            SetZeroData(newRow);
            dataTable.Rows.Add(newRow);
            return newRow;
        }

        private DataRow GetResultRow(ref DataTable dtData, FixedRowIndex fixedRowIndex)
        {
            DataRow[] results = dtData.Select(string.Format("Index = {0}", (int)fixedRowIndex));
            if (results.Length != 0)
                return results[0];

            DataRow resultRow = null;
            switch (fixedRowIndex)
            {
                case FixedRowIndex.RegionRegionalFund:
                    resultRow = dtData.NewRow();
                    resultRow["Name"] = "Областной фонд финансовой поддержки муниципальных районов (городских округов)";
                    resultRow["Index"] = (int)fixedRowIndex;
                    dtData.Rows.Add(resultRow);
                    break;
                case FixedRowIndex.BudgetSubsidy:
                    resultRow = dtData.NewRow();
                    resultRow["Name"] = "Субсидии  бюджетам субъектов РФ и муниципальных образований";
                    resultRow["Index"] = (int)fixedRowIndex;
                    dtData.Rows.Add(resultRow);
                    break;
                case FixedRowIndex.BudgetSubvention:
                    resultRow = dtData.NewRow();
                    resultRow["Name"] = "Субвенции бюджетам субъектов РФ и муниципальных образований";
                    resultRow["Index"] = (int)fixedRowIndex;
                    dtData.Rows.Add(resultRow);
                    break;
                case FixedRowIndex.OtherBudgetTransferts:
                    resultRow = dtData.NewRow();
                    resultRow["Name"] = "Иные межбюджетные трансферты";
                    resultRow["Index"] = (int)fixedRowIndex;
                    dtData.Rows.Add(resultRow);
                    break;
            }
            SetZeroData(resultRow);
            return resultRow;
        }

        private void SetResults(ref DataTable dtData)
        {
            // итого доходов
            DataRow incomes = dtData.Select(string.Format("index = {0}", (int)FixedRowIndex.Incomes))[0];
            DataRow taxNoTax = dtData.Select(string.Format("index = {0}", (int)FixedRowIndex.TaxNoTax))[0];
            DataRow freeSupply = dtData.Select(string.Format("index = {0}", (int)FixedRowIndex.FreeSupply))[0];
            DataRow incomeWork = dtData.Select(string.Format("index = {0}", (int)FixedRowIndex.IncomeWork))[0];
            GetRowSum(ref incomes, new DataRow[] {taxNoTax, freeSupply, incomeWork});
            // межбюджетные трансферты
            DataRow intergovernmentalTransfers = dtData.Select(string.Format("index = {0}", (int)FixedRowIndex.IntergovernmentalTransfers))[0];
            // Дотации бюджетам субъектов РФ и муниципальных образований
            DataRow budgetDotation = dtData.Select(string.Format("index = {0}", (int)FixedRowIndex.BudgetDotation))[0];
            // Областной фонд финансовой поддержки  поселений
            DataRow settlementsRegionalFund = dtData.Select(string.Format("index = {0}", (int)FixedRowIndex.SettlementsRegionalFund))[0];
            // Областной фонд финансовой поддержки муниципальных районов (городских округов)
            DataRow regionRegionalFund = dtData.Select(string.Format("index = {0}", (int)FixedRowIndex.RegionRegionalFund))[0];
            // Субсидии бюджетам субъектов РФ и муниципальных образований
            DataRow budgetSubsidy = dtData.Select(string.Format("index = {0}", (int)FixedRowIndex.BudgetSubsidy))[0];
            // Субвенции бюджетам субъектов РФ и муниципальных образований
            DataRow budgetSubvention = dtData.Select(string.Format("index = {0}", (int)FixedRowIndex.BudgetSubvention))[0];
            // Иные межбюджетные трансферты
            DataRow otherBudgetTransferts = dtData.Select(string.Format("index = {0}", (int)FixedRowIndex.OtherBudgetTransferts))[0];
            // ВСЕГО ДОХОДОВ
            DataRow totalIncomes = dtData.Select(string.Format("index = {0}", (int)FixedRowIndex.TotalIncomes))[0];
            // ВСЕГО РАСХОДОВ
            DataRow totalCharge = dtData.Select(string.Format("index = {0}", (int)FixedRowIndex.TotalCharge))[0];
            // дефицит
            DataRow deficit = dtData.Select(string.Format("index = {0}", (int)FixedRowIndex.Deficit))[0];
            // Разрыв
            DataRow fault = dtData.Select(string.Format("index = {0}", (int)FixedRowIndex.Fault))[0];
            // ИСТОЧНИКИ ВНУТРЕННЕГО ФИНАНСИРОВАНИЯ ДЕФИЦИТОВ  БЮДЖЕТОВ

            GetRowSum(ref budgetDotation, new DataRow[]
                          {
                              settlementsRegionalFund, regionRegionalFund
                          });

            GetRowSum(ref intergovernmentalTransfers, new DataRow[]
                          {
                              budgetDotation, budgetSubsidy, budgetSubvention,
                              otherBudgetTransferts
                          });

            GetRowSum(ref totalIncomes, new DataRow[] { incomes, intergovernmentalTransfers });

            GetRowDif(ref deficit, totalIncomes, totalCharge);

            DataRow[] finSources = dtData.Select(string.Format("index = {0}", (int)FixedRowIndex.FinSources));
            if (finSources.Length != 0)
            {
                GetRowDif(ref fault, deficit, finSources[0]);
            }
        }

        private void GetRowSum(ref DataRow destRow, params DataRow[] sourceRows)
        {
            foreach (string columnName in DataColumns)
            {
                decimal sourceValue = 0;
                foreach (DataRow sourceRow in sourceRows)
                {
                    sourceValue += sourceRow.IsNull(columnName) ? 0 : Convert.ToDecimal(sourceRow[columnName]);
                }
                destRow[columnName] = sourceValue;
            }
        }

        private void GetRowDif(ref DataRow destRow, DataRow row1, DataRow row2)
        {
            foreach (string columnName in DataColumns)
            {
                decimal sourceValue = row1.IsNull(columnName) ? 0 : Convert.ToDecimal(row1[columnName]);
                sourceValue -= row2.IsNull(columnName) ? 0 : Convert.ToDecimal(row2[columnName]);

                destRow[columnName] = sourceValue;
            }
        }

        private void SetRowResults(ref DataTable dtData)
        {
            foreach (DataRow row in dtData.Rows)
            {
                foreach (string columnName in DataColumns)
                {
                    long refRegion;
                    decimal value = 0;
                    if (long.TryParse(columnName.Split('_')[0], out refRegion))
                    {
                        value = Convert.ToDecimal(row[columnName]);
                        int budLevel = Convert.ToInt32(columnName.Split('_')[1]);
                        if (budLevel == 4)
                            continue;
                        if (budLevel == 5 || budLevel == 6)
                        {
                            string consName = refRegion + "_" + 4;
                            row[consName] = Convert.ToDecimal(row[consName]) + value;
                            row["ResultMR_Cons"] = Convert.ToDecimal(row["ResultMR_Cons"]) + value;
                        }
                        if (columnName != "Result_Result")
                            row["Result_Result"] = Convert.ToDecimal(row["Result_Result"]) + value;
                        switch (budLevel)
                        {
                            case 5:
                                if (columnName != "ResultMR_Reg")
                                    row["ResultMR_Reg"] = Convert.ToDecimal(row["ResultMR_Reg"]) + value;
                                break;
                            case 6:
                                if (columnName != "ResultMR_Settl")
                                    row["ResultMR_Settl"] = Convert.ToDecimal(row["ResultMR_Settl"]) + value;
                                break;
                            case 15:
                                if (columnName != "ResultGO_GO")
                                    row["ResultGO_GO"] = Convert.ToDecimal(row["ResultGO_GO"]) + value;
                                break;
                        }
                    }
                }
            }
        }
    }
}
