using System;
using System.Data;
using System.Text;
using Krista.FM.Common;
using Microsoft.Practices.ServiceLocation;

using Krista.FM.ServerLibrary;
using System.Collections.Generic;

namespace Krista.FM.Domain.Services.FinSourceDebtorBook
{
    public class TransfertDataService
    {
        private readonly IScheme scheme;

        public TransfertDataService()
            : this(ServiceLocator.Current.GetInstance<IScheme>())
        {
        }

        public TransfertDataService(IScheme scheme)
        {
            this.scheme = scheme;
        }

        public void TransfertFinSourceData(DateTime calculateDate, int transfertYear, int currentVariantId, int currentSourceId, int currentRegion)
        {
            DeletePrevData();
            GuaranteeTransfert(calculateDate, transfertYear, currentVariantId, currentSourceId, currentRegion);
            CreditPlaningDataTransfert(calculateDate, transfertYear, currentVariantId, currentSourceId, currentRegion);
        }

        #region удаление данных, которые были перенесены в долговую книгу ранее

        private void DeletePrevData()
        {
            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                IEntity creditEntity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.f_S_SchBCreditincome);
                IEntity guaranteeEntity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.f_S_SchBGuarantissued);
                db.ExecQuery(string.Format("delete from {0} where FromFinSource = 1", creditEntity.FullDBName), QueryResultTypes.NonQuery);
                db.ExecQuery(string.Format("delete from {0} where FromFinSource = 1", guaranteeEntity.FullDBName), QueryResultTypes.NonQuery);
            }
        }

        #endregion

        #region перенос данных по гарантиям

        #region валютные гарантии

        const string upDebtCurrency = "select Sum(exch.ExchangeRate*factAttract.CurrencySum) from t_S_FactAttractPrGrnt factAttract, d_S_ExchangeRate exch " +
            "where factAttract.RefGrnt = {0} and factAttract.FactDate BETWEEN ? and ? and exch.ID = " +
            "(select ID from d_S_ExchangeRate where RefOKV = {1} and DateFixing = " +
            "(select Max(DateFixing) from d_S_ExchangeRate where DateFixing <= factAttract.FactDate and RefOKV = {1}))";

        const string upServiceCurrency = "select Sum(exch.ExchangeRate*(COALESCE(planService.CurrencySum, 0) + COALESCE(planService.CurrencyMargin, 0) + COALESCE(planService.CurrencyCommission, 0))) from t_S_PlanServicePrGrnt planService, d_S_ExchangeRate exch " +
            "where planService.RefGrnt = {0} and planService.EndDate BETWEEN ? and ? and exch.ID = " +
            "(select ID from d_S_ExchangeRate where RefOKV = {1} and DateFixing = " +
            "(select Max(DateFixing) from d_S_ExchangeRate where DateFixing <= planService.EndDate and RefOKV = {1}))";

        const string downDebtCurrency1 = "select Sum(exch.ExchangeRate*factDebt.CurrencySum) from t_S_FactDebtPrGrnt factDebt, d_S_ExchangeRate exch " +
            "where factDebt.RefGrnt = {0} and factDebt.FactDate BETWEEN ? and ? and exch.ID = " +
            "(select ID from d_S_ExchangeRate where RefOKV = {1} and DateFixing = " +
            "(select Max(DateFixing) from d_S_ExchangeRate where DateFixing <= factDebt.FactDate and RefOKV = {1}))";

        const string downDebtCurrency2 = "select Sum(exch.ExchangeRate*factGrnt.CurrencySum) from t_S_FactAttractGrnt factGrnt, d_S_ExchangeRate exch " +
            "where factGrnt.RefGrnt = {0} and factGrnt.RefTypSum = 1 and factGrnt.FactDate BETWEEN ? and ? and exch.ID = " +
            "(select ID from d_S_ExchangeRate where RefOKV = {1} and DateFixing = " +
            "(select Max(DateFixing) from d_S_ExchangeRate where DateFixing <= factGrnt.FactDate and RefOKV = {1}))";

        const string downServiceCurrency1 = "select Sum(exch.ExchangeRate*(COALESCE(factService.CurrencySum, 0) + COALESCE(factService.CurrencyMargin, 0) + COALESCE(factService.CurrencyCommission, 0))) from t_S_FactPercentPrGrnt factService, d_S_ExchangeRate exch " +
            "where factService.RefGrnt = {0} and factService.FactDate BETWEEN ? and ? and exch.ID = " +
            "(select ID from d_S_ExchangeRate where RefOKV = {1} and DateFixing = " +
            "(select Max(DateFixing) from d_S_ExchangeRate where DateFixing <= factService.FactDate and RefOKV = {1}))";

        const string downServiceCurrency2 = "select Sum(exch.ExchangeRate*factGrnt.CurrencySum) from t_S_FactAttractGrnt factGrnt, d_S_ExchangeRate exch " +
            "where factGrnt.RefGrnt = {0} and factGrnt.RefTypSum BETWEEN 2 and 7 and factGrnt.FactDate BETWEEN ? and ? and exch.ID = " +
            "(select ID from d_S_ExchangeRate where RefOKV = {1} and DateFixing = " +
            "(select Max(DateFixing) from d_S_ExchangeRate where DateFixing <= factGrnt.FactDate  and RefOKV = {1}))";

        const string downGuarantCurency = "select Sum(exch.ExchangeRate*factGrnt.CurrencySum) from t_S_FactAttractGrnt factGrnt, d_S_ExchangeRate exch " +
            "where factGrnt.RefGrnt = {0} and factGrnt.FactDate BETWEEN ? and ? and exch.ID = " +
            "(select ID from d_S_ExchangeRate where RefOKV = {1} and DateFixing = " +
            "(select Max(DateFixing) from d_S_ExchangeRate where DateFixing <= factGrnt.FactDate and RefOKV = {1}))";

        const string capitalDebtCurrency1 = "select Sum(exch.ExchangeRate*factPrinc.CurrencySum) from t_S_FactAttractPrGrnt factPrinc, d_S_ExchangeRate exch " +
            "where factPrinc.RefGrnt = {0} and factPrinc.FactDate <= ? and exch.ID = " +
            "(select ID from d_S_ExchangeRate where RefOKV = {1} and DateFixing = " +
            "(select Max(DateFixing) from d_S_ExchangeRate where DateFixing <= ? and RefOKV = {1}))";

        const string capitalDebtCurrency2 = "select Sum(exch.ExchangeRate*factDebt.CurrencySum) from t_S_FactDebtPrGrnt factDebt, d_S_ExchangeRate exch " +
            "where factDebt.RefGrnt = {0} and factDebt.FactDate <= ? and exch.ID = " +
            "(select ID from d_S_ExchangeRate where RefOKV = {1} and DateFixing = " +
            "(select Max(DateFixing) from d_S_ExchangeRate where DateFixing <= ? and RefOKV = {1}))";

        const string capitalDebtCurrency3 = "select Sum(exch.ExchangeRate*factGrnt.CurrencySum) from t_S_FactAttractGrnt factGrnt, d_S_ExchangeRate exch " +
            "where factGrnt.RefGrnt = {0} and factGrnt.RefTypSum = 1 and factGrnt.FactDate <= ? and exch.ID = " +
            "(select ID from d_S_ExchangeRate where RefOKV = {1} and DateFixing = " +
            "(select Max(DateFixing) from d_S_ExchangeRate where DateFixing <= ? and RefOKV = {1}))";

        const string serviceDebtCurrency1 = "select Sum(exch.ExchangeRate*(COALESCE(planService.CurrencySum, 0) + COALESCE(planService.CurrencyMargin, 0) + COALESCE(planService.CurrencyCommission, 0))) from t_S_PlanServicePrGrnt planService, d_S_ExchangeRate exch " +
            "where planService.RefGrnt = {0} and planService.EndDate <= ? and exch.ID = " +
            "(select ID from d_S_ExchangeRate where RefOKV = {1} and DateFixing = " +
            "(select Max(DateFixing) from d_S_ExchangeRate where DateFixing <= ? and RefOKV = {1}))";

        const string serviceDebtCurrency2 = "select Sum(exch.ExchangeRate*(COALESCE(factService.CurrencySum, 0) + COALESCE(factService.CurrencyMargin, 0) + COALESCE(factService.CurrencyCommission, 0))) from t_S_FactPercentPrGrnt factService, d_S_ExchangeRate exch " +
            "where factService.RefGrnt = {0} and factService.FactDate <= ? and exch.ID = " +
            "(select ID from d_S_ExchangeRate where RefOKV = {1} and DateFixing = " +
            "(select Max(DateFixing) from d_S_ExchangeRate where DateFixing <= ? and RefOKV = {1}))";

        const string serviceDebtCurrency3 = "select Sum(exch.ExchangeRate*factGrnt.CurrencySum) from t_S_FactAttractGrnt factGrnt, d_S_ExchangeRate exch " +
            "where factGrnt.RefGrnt = {0} and factGrnt.RefTypSum BETWEEN 2 and 7 and factGrnt.FactDate <= ? and exch.ID = " +
            "(select ID from d_S_ExchangeRate where RefOKV = {1} and DateFixing = " +
            "(select Max(DateFixing) from d_S_ExchangeRate where DateFixing <= ?  and RefOKV = {1}))";

        const string stalePrincipalDebtCurrency1 = "select Sum(exch.ExchangeRate*planDebt.CurrencySum) from t_S_PlanDebtPrGrnt planDebt, d_S_ExchangeRate exch " +
            "where planDebt.RefGrnt = {0} and planDebt.EndDate <= ? and exch.ID = " +
            "(select ID from d_S_ExchangeRate where RefOKV = {1} and DateFixing = " +
            "(select Max(DateFixing) from d_S_ExchangeRate where DateFixing <= ? and RefOKV = {1}))";

        const string stalePrincipalDebtCurrency2 = "select Sum(exch.ExchangeRate*(COALESCE(planService.CurrencySum, 0) + COALESCE(planService.CurrencyMargin, 0) + COALESCE(planService.CurrencyCommission, 0))) from t_S_PlanServicePrGrnt planService, d_S_ExchangeRate exch " +
            "where planService.RefGrnt = {0} and planService.EndDate <= ? and exch.ID = " +
            "(select ID from d_S_ExchangeRate where RefOKV = {1} and DateFixing = " +
            "(select Max(DateFixing) from d_S_ExchangeRate where DateFixing <= ? and RefOKV = {1}))";

        const string stalePrincipalDebtCurrency3 = "select Sum(exch.ExchangeRate*factDebt.CurrencySum) from t_S_FactDebtPrGrnt factDebt, d_S_ExchangeRate exch " +
            "where factDebt.RefGrnt = {0} and factDebt.FactDate <= ? and exch.ID = " +
            "(select ID from d_S_ExchangeRate where RefOKV = {1} and DateFixing = " +
            "(select Max(DateFixing) from d_S_ExchangeRate where DateFixing <= ? and RefOKV = {1}))";

        const string stalePrincipalDebtCurrency4 = "select Sum(exch.ExchangeRate*(COALESCE(factService.CurrencySum, 0) + COALESCE(factService.CurrencyMargin, 0) + COALESCE(factService.CurrencyCommission, 0))) from t_S_FactPercentPrGrnt factService, d_S_ExchangeRate exch " +
            "where factService.RefGrnt = {0} and factService.FactDate <= ? and exch.ID = " +
            "(select ID from d_S_ExchangeRate where RefOKV = {1} and DateFixing = " +
            "(select Max(DateFixing) from d_S_ExchangeRate where DateFixing <= ? and RefOKV = {1}))";

        const string staleGarantDebtCurrency1 = "select Sum(exch.ExchangeRate*planGrnt.CurrencySum) from t_S_PlanAttractGrnt planGrnt, d_S_ExchangeRate exch " +
            "where planGrnt.RefGrnt = {0} and planGrnt.RefTypSum BETWEEN 2 and 7 and planGrnt.EndDate <= ? and exch.ID = " +
            "(select ID from d_S_ExchangeRate where RefOKV = {1} and DateFixing = " +
            "(select Max(DateFixing) from d_S_ExchangeRate where DateFixing <= ?  and RefOKV = {1}))";

        const string staleGarantDebtCurrency2 = "select Sum(exch.ExchangeRate*factGrnt.CurrencySum) from t_S_FactAttractGrnt factGrnt, d_S_ExchangeRate exch " +
            "where factGrnt.RefGrnt = {0} and factGrnt.RefTypSum BETWEEN 2 and 7 and factGrnt.FactDate <= ? and exch.ID = " +
            "(select ID from d_S_ExchangeRate where RefOKV = {1} and DateFixing = " +
            "(select Max(DateFixing) from d_S_ExchangeRate where DateFixing <= ? and RefOKV = {1}))";

        #endregion

        #region рублевые гарантии

        const string downDebt1 = "select Sum(factDebt.Sum) from t_S_FactDebtPrGrnt factDebt " +
            "where factDebt.RefGrnt = {0} and factDebt.FactDate BETWEEN ? and ?";

        const string downDebt2 = "select Sum(factGrnt.Sum) from t_S_FactAttractGrnt factGrnt " +
            "where factGrnt.RefGrnt = {0} and factGrnt.RefTypSum = 1 and factGrnt.FactDate BETWEEN ? and ?";

        const string downService1 = "select Sum(COALESCE(factService.Sum, 0) + COALESCE(factService.Margin, 0) + COALESCE(factService.Commission, 0)) from t_S_FactPercentPrGrnt factService " +
            "where factService.RefGrnt = {0} and factService.FactDate BETWEEN ? and ?";

        const string downService2 =
            "select Sum(factGrnt.Sum) from t_S_FactAttractGrnt factGrnt " +
            "where factGrnt.RefGrnt = {0} and factGrnt.RefTypSum BETWEEN 2 and 7 and factGrnt.FactDate BETWEEN ? and ?";

        const string downGuarantQuery = "select Sum(factGrnt.Sum) from t_S_FactAttractGrnt factGrnt " +
            "where factGrnt.RefGrnt = {0} and factGrnt.FactDate BETWEEN ? and ?";

        const string capitalDebtQuery1 = "select Sum(principalContr.Sum) from t_S_PrincipalContrGrnt principalContr " +
            "where principalContr.RefGrnt = {0}";

        const string capitalDebtQuery2 = "select Sum(factDebt.Sum) from t_S_FactDebtPrGrnt factDebt " +
            "where factDebt.RefGrnt = {0} and factDebt.FactDate <= ?";

        const string serviceDebt1 = "select Sum(planService.Sum) from t_S_PlanServicePrGrnt planService " +
            "where planService.RefGrnt = {0}";

        const string serviceDebt2 = "select Sum(COALESCE(factService.Sum, 0) + COALESCE(factService.Margin, 0) + COALESCE(factService.Commission, 0)) from t_S_FactPercentPrGrnt factService " +
            "where factService.RefGrnt = {0} and factService.FactDate <= ?";

        const string stalePrincipalDebt1 = "select Sum(COALESCE(planService.Sum, 0) + COALESCE(planService.Margin, 0) + COALESCE(planService.Commission, 0)) from t_S_PlanServicePrGrnt planService " +
            "where planService.RefGrnt = {0} and planService.EndDate <= ?";

        const string stalePrincipalDebt2 = "select Sum(COALESCE(factService.Sum, 0) + COALESCE(factService.Margin, 0) + COALESCE(factService.Commission, 0)) from t_S_FactPercentPrGrnt factService " +
            "where factService.RefGrnt = {0} and factService.FactDate <= ?";

        const string staleGarantDebt1 = "select Sum(planGrnt.Sum) from t_S_PlanAttractGrnt planGrnt " +
            "where planGrnt.RefGrnt = {0} and planGrnt.RefTypSum BETWEEN 2 and 7 and planGrnt.EndDate <= ?";

        const string staleGarantDebt2 = "select Sum(factGrnt.Sum) from t_S_FactAttractGrnt factGrnt " +
            "where factGrnt.RefGrnt = {0} and factGrnt.RefTypSum BETWEEN 2 and 7 and factGrnt.FactDate <= ?";

        #endregion

        public void GuaranteeTransfert(DateTime calculateDate, int currentYear, int currentVariantId, int currentSourceId, int currentRegion)
        {
            // запрос по кредитам
            string guaranteeQuery =
                "select guarantee.ID, guarantee.RegNum, creditType.Name, guarantee.Num, guarantee.StartDate, guarantee.EndDate, guarantee.RenewalDate, guarantee.RegDate, " +
                "guarantee.Occasion, guarantee.RefOrganizations, guarantee.RefOrganizationsPlan3, guarantee.Sum, guarantee.CurrencySum, guarantee.DebtSum, guarantee.RefOKV, guarantee.Regress, guarantee.Recourse, " +
                "guarantee.DateDoc, guarantee.GuarantPeriod, guarantee.Antecedent, guarantee.Antecedent, guarantee.RefTypeContract " +
                "from f_S_Guarantissued guarantee, d_S_TypeContract creditType " +
                "where guarantee.RefTypeContract = creditType.ID " +
                "and (guarantee.RefVariant = 0 or guarantee.RefVariant = -2) ";

            // по каждому кредиту получаем информацию о деталях, обрабатываем все и добавляем запись в долговую книгу
            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                IEntity entity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.f_S_SchBGuarantissued);
                using (IDataUpdater du = entity.GetDataUpdater("1 = 2", null, null))
                {
                    DataTable dtDebtorGuarantee = new DataTable();
                    du.Fill(ref dtDebtorGuarantee);

                    DataTable dtGuarantee = (DataTable)db.ExecQuery(guaranteeQuery, QueryResultTypes.DataTable);

                    const string collateralQuery = "select Name from t_S_CollateralGrnt where RefGrnt = {0}";

                    string furtherConventionQuery =
                        "select NumberContract, DataContract from t_S_ListContractGrnt where RefGrnt = {0} and DataContract <= ? and BaseDoc = 0 order by DataContract ASC";

                    foreach (DataRow guaranteeRow in dtGuarantee.Rows)
                    {
                        DateTime startDate = Convert.ToDateTime(guaranteeRow["StartDate"]);
                        DateTime endDate = DateTime.MinValue;
                        if (!DateTime.TryParse(guaranteeRow["RenewalDate"].ToString(), out endDate))
                            DateTime.TryParse(guaranteeRow["EndDate"].ToString(), out endDate);

                        if (startDate > calculateDate || endDate.Year < calculateDate.Year)
                            continue;

                        #region получение данных по деталям
                        DataTable dtCollateral = (DataTable)db.ExecQuery(string.Format(collateralQuery, guaranteeRow["ID"]), QueryResultTypes.DataTable);
                        string[] collateralArray = new string[dtCollateral.Rows.Count];
                        for (int i = 0; i <= dtCollateral.Rows.Count - 1; i++)
                        {
                            collateralArray[i] = dtCollateral.Rows[i][0].ToString();
                        }
                        string collateralName = string.Join("; ", collateralArray);
                        int refOKV = Convert.ToInt32(guaranteeRow["RefOKV"]);

                        List<string> furtherConvention = new List<string>();
                        DataTable dtFurtherConvention = (DataTable)db.ExecQuery(string.Format(furtherConventionQuery, guaranteeRow["ID"]),
                            QueryResultTypes.DataTable, new DbParameterDescriptor("p0", calculateDate));
                        foreach (DataRow row in dtFurtherConvention.Rows)
                        {
                            furtherConvention.Add(string.Format("№ {0} от {1}", row[0], Convert.ToDateTime(row[1]).ToShortDateString()));
                        }
                        #endregion

                        DataRow newRow = dtDebtorGuarantee.NewRow();
                        newRow.BeginEdit();
                        //newRow["ID"] = entity.GetGeneratorNextValue;
                        newRow["RefVariant"] = currentVariantId;
                        newRow["SourceID"] = currentSourceId;
                        newRow["TaskID"] = -1;
                        newRow["PumpID"] = -1;
                        newRow["RefRegion"] = currentRegion;
                        newRow["RegNum"] = guaranteeRow["RegNum"];
                        newRow["Num"] = guaranteeRow["Num"];
                        newRow["Occasion"] = guaranteeRow["Occasion"];
                        newRow["DateDoc"] = guaranteeRow.IsNull("RegDate") ? (object)DBNull.Value :
                            Convert.ToDateTime(guaranteeRow["RegDate"]).ToShortDateString();
                        newRow["Sum"] = refOKV == -1 ? guaranteeRow["Sum"] : guaranteeRow["CurrencySum"];
                        newRow["Regress"] = guaranteeRow["Recourse"];//Convert.ToBoolean(guaranteeRow["Regress"]) ? "Да" : "Нет";
                        newRow["Collateral"] = collateralName;
                        newRow["FurtherConvention"] = String.Join("; ", furtherConvention.ToArray());
                        newRow["StartDate"] = Convert.ToDateTime(guaranteeRow["StartDate"]).ToShortDateString();
                        newRow["EndDate"] = Convert.ToDateTime(guaranteeRow["EndDate"]).ToShortDateString();
                        newRow["RefOrganizations"] = guaranteeRow["RefOrganizations"];
                        newRow["RefOrganizationsPlan3"] = guaranteeRow["RefOrganizationsPlan3"];
                        newRow["refOKV"] = refOKV;
                        newRow["RefTypeContract"] = guaranteeRow["RefTypeContract"];
                        newRow["FromFinSource"] = 1;
                        if (refOKV == -1)
                        {
                            GetGuaranteeRow(guaranteeRow["ID"], Convert.ToDecimal(guaranteeRow["DebtSum"]), newRow, calculateDate, currentYear, db);
                        }
                        else
                        {
                            GetCurrencyGuaranteeRow(guaranteeRow["ID"], newRow, refOKV, calculateDate, currentYear, db);
                        }
                        newRow["StalePrincipalDebt"] = 0;
                        newRow.EndEdit();
                        dtDebtorGuarantee.Rows.Add(newRow);
                    }
                    du.Update(ref dtDebtorGuarantee);
                }
            }
        }

        private void GetCurrencyGuaranteeRow(object guaranteeID, DataRow guaranteeRow, int refOKV, DateTime reportDate, int currentYear, IDatabase db)
        {
            DateTime currentYearDate = new DateTime(currentYear, 1, 1);
            decimal upDebtSum = 0;
            object queryResult = db.ExecQuery(string.Format(upDebtCurrency, guaranteeID, refOKV), QueryResultTypes.Scalar,
                new System.Data.OleDb.OleDbParameter("p0", currentYearDate),
                new System.Data.OleDb.OleDbParameter("p1", reportDate));
            if (!(queryResult is DBNull))
                upDebtSum += Convert.ToDecimal(queryResult);

            decimal upService = 0;
            queryResult = db.ExecQuery(string.Format(upServiceCurrency, guaranteeID, refOKV), QueryResultTypes.Scalar,
                new System.Data.OleDb.OleDbParameter("p0", currentYearDate),
                new System.Data.OleDb.OleDbParameter("p1", reportDate));
            if (!(queryResult is DBNull))
                upService += Convert.ToDecimal(queryResult);

            decimal downDebt = 0;
            queryResult = db.ExecQuery(string.Format(downDebtCurrency1, guaranteeID, refOKV), QueryResultTypes.Scalar,
                new System.Data.OleDb.OleDbParameter("p0", currentYearDate),
                new System.Data.OleDb.OleDbParameter("p1", reportDate));
            if (!(queryResult is DBNull))
                downDebt += Convert.ToDecimal(queryResult);
            queryResult = db.ExecQuery(string.Format(downDebtCurrency2, guaranteeID, refOKV), QueryResultTypes.Scalar,
                new System.Data.OleDb.OleDbParameter("p0", currentYearDate),
                new System.Data.OleDb.OleDbParameter("p1", reportDate));
            if (!(queryResult is DBNull))
                downDebt += Convert.ToDecimal(queryResult);

            decimal downService = 0;
            queryResult = db.ExecQuery(string.Format(downServiceCurrency1, guaranteeID, refOKV), QueryResultTypes.Scalar,
                new System.Data.OleDb.OleDbParameter("p0", currentYearDate),
                new System.Data.OleDb.OleDbParameter("p1", reportDate));
            if (!(queryResult is DBNull))
                downService += Convert.ToDecimal(queryResult);
            queryResult = db.ExecQuery(string.Format(downServiceCurrency2, guaranteeID, refOKV), QueryResultTypes.Scalar,
                new System.Data.OleDb.OleDbParameter("p0", currentYearDate),
                new System.Data.OleDb.OleDbParameter("p1", reportDate));
            if (!(queryResult is DBNull))
                downService += Convert.ToDecimal(queryResult);

            decimal downGarant = 0;
            queryResult = db.ExecQuery(string.Format(downGuarantCurency, guaranteeID, refOKV), QueryResultTypes.Scalar,
                new System.Data.OleDb.OleDbParameter("p0", currentYearDate),
                new System.Data.OleDb.OleDbParameter("p1", reportDate));
            if (!(queryResult is DBNull))
                downGarant += Convert.ToDecimal(queryResult);

            decimal capitalDebt = 0;
            queryResult = db.ExecQuery(string.Format(capitalDebtCurrency1, guaranteeID, refOKV), QueryResultTypes.Scalar,
                new System.Data.OleDb.OleDbParameter("p0", reportDate),
                new System.Data.OleDb.OleDbParameter("p1", reportDate));
            if (!(queryResult is DBNull))
                capitalDebt += Convert.ToDecimal(queryResult);
            queryResult = db.ExecQuery(string.Format(capitalDebtCurrency2, guaranteeID, refOKV), QueryResultTypes.Scalar,
                new System.Data.OleDb.OleDbParameter("p0", reportDate),
                new System.Data.OleDb.OleDbParameter("p1", reportDate));
            if (!(queryResult is DBNull))
                capitalDebt -= Convert.ToDecimal(queryResult);
            queryResult = db.ExecQuery(string.Format(capitalDebtCurrency3, guaranteeID, refOKV), QueryResultTypes.Scalar,
                new System.Data.OleDb.OleDbParameter("p0", reportDate),
                new System.Data.OleDb.OleDbParameter("p1", reportDate));
            if (!(queryResult is DBNull))
                capitalDebt -= Convert.ToDecimal(queryResult);

            decimal serviceDebt = 0;
            queryResult = db.ExecQuery(string.Format(serviceDebtCurrency1, guaranteeID, refOKV), QueryResultTypes.Scalar,
                new System.Data.OleDb.OleDbParameter("p0", reportDate),
                new System.Data.OleDb.OleDbParameter("p1", reportDate));
            if (!(queryResult is DBNull))
                serviceDebt += Convert.ToDecimal(queryResult);
            queryResult = db.ExecQuery(string.Format(serviceDebtCurrency2, guaranteeID, refOKV), QueryResultTypes.Scalar,
                new System.Data.OleDb.OleDbParameter("p0", reportDate),
                new System.Data.OleDb.OleDbParameter("p1", reportDate));
            if (!(queryResult is DBNull))
                serviceDebt -= Convert.ToDecimal(queryResult);
            queryResult = db.ExecQuery(string.Format(serviceDebtCurrency3, guaranteeID, refOKV), QueryResultTypes.Scalar,
                new System.Data.OleDb.OleDbParameter("p0", reportDate),
                new System.Data.OleDb.OleDbParameter("p1", reportDate));
            if (!(queryResult is DBNull))
                serviceDebt -= Convert.ToDecimal(queryResult);

            decimal stalePrincipalDebt = 0;
            queryResult = db.ExecQuery(string.Format(stalePrincipalDebtCurrency2, guaranteeID, refOKV), QueryResultTypes.Scalar,
                new System.Data.OleDb.OleDbParameter("p0", reportDate),
                new System.Data.OleDb.OleDbParameter("p1", reportDate));
            if (!(queryResult is DBNull))
                stalePrincipalDebt += Convert.ToDecimal(queryResult);
            queryResult = db.ExecQuery(string.Format(stalePrincipalDebtCurrency4, guaranteeID, refOKV), QueryResultTypes.Scalar,
                new System.Data.OleDb.OleDbParameter("p0", reportDate),
                new System.Data.OleDb.OleDbParameter("p1", reportDate));
            if (!(queryResult is DBNull))
                stalePrincipalDebt -= Convert.ToDecimal(queryResult);

            decimal staleGarantDebt = 0;
            queryResult = db.ExecQuery(string.Format(staleGarantDebtCurrency1, guaranteeID, refOKV), QueryResultTypes.Scalar,
                new System.Data.OleDb.OleDbParameter("p0", reportDate),
                new System.Data.OleDb.OleDbParameter("p1", reportDate));
            if (!(queryResult is DBNull))
                staleGarantDebt += Convert.ToDecimal(queryResult);
            queryResult = db.ExecQuery(string.Format(staleGarantDebtCurrency2, guaranteeID, refOKV), QueryResultTypes.Scalar,
                new System.Data.OleDb.OleDbParameter("p0", reportDate),
                new System.Data.OleDb.OleDbParameter("p1", reportDate));
            if (!(queryResult is DBNull))
                staleGarantDebt -= Convert.ToDecimal(queryResult);

            guaranteeRow["UpDebt"] = upDebtSum;
            guaranteeRow["UpService"] = upService;
            guaranteeRow["DownDebt"] = downDebt;
            guaranteeRow["DownService"] = downService;
            guaranteeRow["DownGarant"] = downGarant;
            guaranteeRow["CapitalDebt"] = capitalDebt;
            guaranteeRow["StalePrincipalDebt"] = stalePrincipalDebt;
            guaranteeRow["StaleGarantDebt"] = staleGarantDebt;
            guaranteeRow["ServiceDebt"] = serviceDebt;
            guaranteeRow["TotalDebt"] = capitalDebt + serviceDebt;
        }

        private void GetGuaranteeRow(object guaranteeID, decimal guaranteeDebtSum, DataRow guaranteeRow, DateTime reportDate, int currentYear, IDatabase db)
        {
            DateTime currentYearDate = new DateTime(currentYear, 1, 1);

            decimal downDebt = 0;
            object queryResult = db.ExecQuery(string.Format(downDebt1, guaranteeID), QueryResultTypes.Scalar,
                new System.Data.OleDb.OleDbParameter("p0", currentYearDate),
                new System.Data.OleDb.OleDbParameter("p1", reportDate));
            if (!(queryResult is DBNull))
                downDebt += Convert.ToDecimal(queryResult);
            queryResult = db.ExecQuery(string.Format(downDebt2, guaranteeID), QueryResultTypes.Scalar,
                new System.Data.OleDb.OleDbParameter("p0", currentYearDate),
                new System.Data.OleDb.OleDbParameter("p1", reportDate));
            if (!(queryResult is DBNull))
                downDebt += Convert.ToDecimal(queryResult);

            decimal downService = 0;
            queryResult = db.ExecQuery(string.Format(downService1, guaranteeID), QueryResultTypes.Scalar,
                new System.Data.OleDb.OleDbParameter("p0", currentYearDate),
                new System.Data.OleDb.OleDbParameter("p1", reportDate));
            if (!(queryResult is DBNull))
                downService += Convert.ToDecimal(queryResult);
            queryResult = db.ExecQuery(string.Format(downService2, guaranteeID), QueryResultTypes.Scalar,
                new System.Data.OleDb.OleDbParameter("p0", currentYearDate),
                new System.Data.OleDb.OleDbParameter("p1", reportDate));
            if (!(queryResult is DBNull))
                downService += Convert.ToDecimal(queryResult);

            decimal downGarant = 0;
            queryResult = db.ExecQuery(string.Format(downGuarantQuery, guaranteeID), QueryResultTypes.Scalar,
                new System.Data.OleDb.OleDbParameter("p0", currentYearDate),
                new System.Data.OleDb.OleDbParameter("p1", reportDate));
            if (!(queryResult is DBNull))
                downGarant += Convert.ToDecimal(queryResult);

            decimal sum = Convert.ToDecimal(guaranteeRow["Sum"]);
            decimal debtSum = guaranteeDebtSum;
            decimal capitalDebt = 0;

            queryResult = db.ExecQuery(string.Format(capitalDebtQuery1, guaranteeID), QueryResultTypes.Scalar);
            if (!(queryResult is DBNull))
                capitalDebt = Convert.ToDecimal(queryResult);
            queryResult = db.ExecQuery(string.Format(capitalDebtQuery2, guaranteeID), QueryResultTypes.Scalar,
                new System.Data.OleDb.OleDbParameter("p0", reportDate));
            if (!(queryResult is DBNull))
                capitalDebt -= Convert.ToDecimal(queryResult);
            if (capitalDebt > debtSum)
                capitalDebt = debtSum;

            decimal serviceDebt = 0;
            queryResult = db.ExecQuery(string.Format(serviceDebt1, guaranteeID), QueryResultTypes.Scalar);
            if (!(queryResult is DBNull))
            {
                serviceDebt += Convert.ToDecimal(queryResult);
                queryResult = db.ExecQuery(string.Format(serviceDebt2, guaranteeID), QueryResultTypes.Scalar,
                                           new System.Data.OleDb.OleDbParameter("p0", reportDate));
                if (!(queryResult is DBNull))
                    serviceDebt -= Convert.ToDecimal(queryResult);
                if (serviceDebt > (sum - debtSum))
                    serviceDebt = sum - debtSum;
            }
            else
            {
                serviceDebt = sum - debtSum;
            }

            decimal stalePrincipalDebt = 0;
            queryResult = db.ExecQuery(string.Format(stalePrincipalDebt1, guaranteeID), QueryResultTypes.Scalar,
                new System.Data.OleDb.OleDbParameter("p0", reportDate));
            if (!(queryResult is DBNull))
                stalePrincipalDebt += Convert.ToDecimal(queryResult);
            queryResult = db.ExecQuery(string.Format(stalePrincipalDebt2, guaranteeID), QueryResultTypes.Scalar,
                new System.Data.OleDb.OleDbParameter("p0", reportDate));
            if (!(queryResult is DBNull))
                stalePrincipalDebt -= Convert.ToDecimal(queryResult);

            decimal staleGarantDebt = 0;
            queryResult = db.ExecQuery(string.Format(staleGarantDebt1, guaranteeID), QueryResultTypes.Scalar,
                new System.Data.OleDb.OleDbParameter("p0", reportDate));
            if (!(queryResult is DBNull))
                staleGarantDebt += Convert.ToDecimal(queryResult);
            queryResult = db.ExecQuery(string.Format(staleGarantDebt2, guaranteeID), QueryResultTypes.Scalar,
                new System.Data.OleDb.OleDbParameter("p0", reportDate));
            if (!(queryResult is DBNull))
                staleGarantDebt -= Convert.ToDecimal(queryResult);

            guaranteeRow["UpDebt"] = debtSum;
            guaranteeRow["UpService"] = sum - debtSum;
            guaranteeRow["DownDebt"] = downDebt;
            guaranteeRow["DownService"] = downService;
            guaranteeRow["DownGarant"] = downGarant;
            guaranteeRow["CapitalDebt"] = capitalDebt;
            guaranteeRow["StalePrincipalDebt"] = stalePrincipalDebt;
            guaranteeRow["StaleGarantDebt"] = staleGarantDebt;
            guaranteeRow["ServiceDebt"] = serviceDebt;
            guaranteeRow["TotalDebt"] = capitalDebt + serviceDebt;
        }

        #endregion

        public void CreditPlaningDataTransfert(DateTime periodTime, int currentYear, int currentVariantId, int currentSourceId, int currentRegion)
        {
            TransfertOrgCredits(periodTime, currentYear,
                "(credit.RefSTypeCredit = 0)", currentVariantId, currentSourceId, currentRegion);
            TransfertBudCredits(periodTime, currentYear,
                "(credit.RefSTypeCredit = 1)", currentVariantId, currentSourceId, currentRegion);
        }

        /// <summary>
        /// перенос кредитов от кредитных организаций
        /// </summary>
        /// <param name="periodTime"></param>
        /// <param name="currentYear"></param>
        /// <param name="CreditsFilter"></param>
        /// <param name="currentVariantId"></param>
        /// <param name="currentSourceId"></param>
        /// <param name="currentRegion"></param>
        private void TransfertOrgCredits(DateTime periodTime, int currentYear, string CreditsFilter, int currentVariantId, int currentSourceId, int currentRegion)
        {
            IEntity creditEntity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.f_S_Creditincome);
            IEntity creditTypeEntity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.d_S_TypeContract);

            // запрос по кредитам
            string creditQuery = "select credit.ID, credit.Num, credit.RegNum, credit.StartDate, credit.EndDate, credit.RenewalDate, credit.ContractDate, credit.PretermDischarge, " +
                "credit.Sum, credit.Occasion, credit.CreditPercent, credit.RefSTypeCredit, credit.RefTypeContract, credit.RefOrganizations, creditType.Name from {0} credit, {1} creditType where " +
                "credit.RefTypeContract = creditType.ID and (RefVariant = 0 or RefVariant = -2) and " + CreditsFilter;

            // по каждому кредиту получаем информацию о деталях, обрабатываем все и добавляем запись в долговую книгу
            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                DateTime currentYearDate = new DateTime(currentYear, 1, 1);
                IEntity entity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.f_S_SchBCreditincome);
                using (IDataUpdater du = entity.GetDataUpdater("1 = 2", null, null))
                {
                    DataTable dtDebtorCredits = new DataTable();
                    du.Fill(ref dtDebtorCredits);
                    DataTable dtCredits = (DataTable)db.ExecQuery(string.Format(creditQuery, creditEntity.FullDBName,
                        creditTypeEntity.FullDBName), QueryResultTypes.DataTable);

                    const string futherConventionQuery =
                        "select NumberContract, DataContract from t_S_ListContractCl where RefCreditInc = {0} and DataContract <= ? and BaseDoc = 0 order by DataContract ASC";

                    const string serviceDebtQuery1 = "select Sum(sum) from t_S_PlanServiceCI where RefCreditInc = {0} and EndDate <= ?";
                    const string serviceDebtQuery2 = "select Sum(sum) from t_S_FactPercentCI where RefCreditInc = {0} and FactDate <= ?";
                    const string serviceDebtQuery3 = "select Sum(sum) from t_S_ChargePenaltyDebtCI where RefCreditInc = {0} and StartDate <= ?";
                    const string serviceDebtQuery4 = "select Sum(sum) from t_S_FactPenaltyDebtCI where RefCreditInc = {0} and FactDate <= ?";
                    const string serviceDebtQuery5 = "select Sum(sum) from t_S_CIChargePenaltyPercent where RefCreditInc = {0} and StartDate <= ?";
                    const string serviceDebtQuery6 = "select Sum(sum) from t_S_FactPenaltyPercentCI where RefCreditInc = {0} and FactDate <= ?";

                    const string capitalDebtQuery1 = "select Sum(sum) from t_S_FactAttractCI where RefCreditInc = {0} and FactDate <= ?";
                    const string capitalDebtQuery2 = "select Sum(sum) from t_S_FactDebtCI where RefCreditInc = {0} and FactDate <= ?";

                    const string collateralQuery = "select Name from t_S_CollateralCI where RefCreditInc = {0}";
                    // данные из журнала ставок процентов
                    const string percentsQuery = "select CreditPercent, ChargeDate from t_S_JournalPercentCI where RefCreditInc = {0}";

                    const string factDateQuery = "select FactDate from t_S_FactDebtCI where RefCreditInc = {0} order by FactDate";

                    const string dischargeQuery =
                        "select Sum(sum) from t_S_FactDebtCI where RefCreditInc = {0} and FactDate BETWEEN ? and ?";

                    const string planServiceQuery1 =
                        "select Sum(sum) from t_S_PlanServiceCI where RefCreditInc = {0} and EndDate BETWEEN ? and ?";
                    const string planServiceQuery2 =
                        "select Sum(sum) from t_S_ChargePenaltyDebtCI where RefCreditInc = {0} and StartDate BETWEEN ? and ?";
                    const string planServiceQuery3 =
                        "select Sum(sum) from t_S_CIChargePenaltyPercent where RefCreditInc = {0} and StartDate BETWEEN ? and ?";

                    const string factServiceQuery1 =
                        "select Sum(sum) from t_S_FactPercentCI where RefCreditInc = {0} and FactDate BETWEEN ? and ?";
                    const string factServiceQuery2 =
                        "select Sum(sum) from t_S_FactPenaltyDebtCI where RefCreditInc = {0} and FactDate BETWEEN ? and ?";
                    const string factServiceQuery3 =
                        "select Sum(sum) from t_S_FactPenaltyPercentCI where RefCreditInc = {0} and FactDate BETWEEN ? and ?";

                    const string factAttractSumQuery1 =
                        "select Sum(sum) from t_S_FactAttractCI where RefCreditInc = {0} and FactDate BETWEEN ? and ?";
                    const string factAttractSumQuery2 =
                        "select Sum(sum) from t_S_FactAttractCI where RefCreditInc = {0}";

                    foreach (DataRow creditRow in dtCredits.Rows)
                    {
                        DateTime startDate = Convert.ToDateTime(creditRow["StartDate"]);
                        DateTime endDate = DateTime.MinValue;
                        if (!DateTime.TryParse(creditRow["RenewalDate"].ToString(), out endDate))
                            DateTime.TryParse(creditRow["EndDate"].ToString(), out endDate);

                        if (startDate > periodTime || endDate.Year < periodTime.Year)
                            continue;

                        decimal capitalDebt = 0;
                        object queryResult = db.ExecQuery(string.Format(capitalDebtQuery1, creditRow["ID"]), QueryResultTypes.Scalar,
                            new System.Data.OleDb.OleDbParameter("p0", periodTime));
                        if (!(queryResult is DBNull))
                            capitalDebt += Convert.ToDecimal(queryResult);
                        queryResult = db.ExecQuery(string.Format(capitalDebtQuery2, creditRow["ID"]), QueryResultTypes.Scalar,
                            new System.Data.OleDb.OleDbParameter("p0", periodTime));
                        if (!(queryResult is DBNull))
                            capitalDebt -= Convert.ToDecimal(queryResult);

                        decimal serviceDebt = 0;
                        queryResult = db.ExecQuery(string.Format(serviceDebtQuery1, creditRow["ID"]), QueryResultTypes.Scalar,
                            new System.Data.OleDb.OleDbParameter("p0", periodTime));
                        if (!(queryResult is DBNull))
                            serviceDebt = Convert.ToDecimal(queryResult);
                        queryResult = db.ExecQuery(string.Format(serviceDebtQuery2, creditRow["ID"]), QueryResultTypes.Scalar,
                            new System.Data.OleDb.OleDbParameter("p0", periodTime));
                        if (!(queryResult is DBNull))
                            serviceDebt -= Convert.ToDecimal(queryResult);
                        queryResult = db.ExecQuery(string.Format(serviceDebtQuery3, creditRow["ID"]), QueryResultTypes.Scalar,
                            new System.Data.OleDb.OleDbParameter("p0", periodTime));
                        if (!(queryResult is DBNull))
                            serviceDebt += Convert.ToDecimal(queryResult);
                        queryResult = db.ExecQuery(string.Format(serviceDebtQuery4, creditRow["ID"]), QueryResultTypes.Scalar,
                            new System.Data.OleDb.OleDbParameter("p0", periodTime));
                        if (!(queryResult is DBNull))
                            serviceDebt -= Convert.ToDecimal(queryResult);
                        queryResult = db.ExecQuery(string.Format(serviceDebtQuery5, creditRow["ID"]), QueryResultTypes.Scalar,
                            new System.Data.OleDb.OleDbParameter("p0", periodTime));
                        if (!(queryResult is DBNull))
                            serviceDebt += Convert.ToDecimal(queryResult);
                        queryResult = db.ExecQuery(string.Format(serviceDebtQuery6, creditRow["ID"]), QueryResultTypes.Scalar,
                            new System.Data.OleDb.OleDbParameter("p0", periodTime));
                        if (!(queryResult is DBNull))
                            serviceDebt -= Convert.ToDecimal(queryResult);
                        decimal staleDebt = serviceDebt;

                        decimal discharge = 0;
                        queryResult = db.ExecQuery(string.Format(dischargeQuery, creditRow["ID"]), QueryResultTypes.Scalar,
                            new System.Data.OleDb.OleDbParameter("p0", currentYearDate),
                            new System.Data.OleDb.OleDbParameter("p1", periodTime));
                        if (!(queryResult is DBNull))
                            discharge = Convert.ToDecimal(queryResult);

                        decimal planService = 0;
                        queryResult = db.ExecQuery(string.Format(planServiceQuery1, creditRow["ID"]), QueryResultTypes.Scalar,
                            new System.Data.OleDb.OleDbParameter("p0", currentYearDate),
                            new System.Data.OleDb.OleDbParameter("p1", periodTime));
                        if (!(queryResult is DBNull))
                            planService = Convert.ToDecimal(queryResult);
                        queryResult = db.ExecQuery(string.Format(planServiceQuery2, creditRow["ID"]), QueryResultTypes.Scalar,
                            new System.Data.OleDb.OleDbParameter("p0", currentYearDate),
                            new System.Data.OleDb.OleDbParameter("p1", periodTime));
                        if (!(queryResult is DBNull))
                            planService += Convert.ToDecimal(queryResult);
                        queryResult = db.ExecQuery(string.Format(planServiceQuery3, creditRow["ID"]), QueryResultTypes.Scalar,
                            new System.Data.OleDb.OleDbParameter("p0", currentYearDate),
                            new System.Data.OleDb.OleDbParameter("p1", periodTime));
                        if (!(queryResult is DBNull))
                            planService += Convert.ToDecimal(queryResult);

                        decimal factService = 0;
                        queryResult = db.ExecQuery(string.Format(factServiceQuery1, creditRow["ID"]), QueryResultTypes.Scalar,
                            new System.Data.OleDb.OleDbParameter("p0", currentYearDate),
                            new System.Data.OleDb.OleDbParameter("p1", periodTime));
                        if (!(queryResult is DBNull))
                            factService = Convert.ToDecimal(queryResult);
                        queryResult = db.ExecQuery(string.Format(factServiceQuery2, creditRow["ID"]), QueryResultTypes.Scalar,
                            new System.Data.OleDb.OleDbParameter("p0", currentYearDate),
                            new System.Data.OleDb.OleDbParameter("p1", periodTime));
                        if (!(queryResult is DBNull))
                            factService += Convert.ToDecimal(queryResult);
                        queryResult = db.ExecQuery(string.Format(factServiceQuery3, creditRow["ID"]), QueryResultTypes.Scalar,
                            new System.Data.OleDb.OleDbParameter("p0", currentYearDate),
                            new System.Data.OleDb.OleDbParameter("p1", periodTime));
                        if (!(queryResult is DBNull))
                            factService += Convert.ToDecimal(queryResult);

                        decimal attractSum = Convert.ToDecimal(creditRow["Sum"]);
                        queryResult = db.ExecQuery(string.Format(factAttractSumQuery2, creditRow["ID"]), QueryResultTypes.Scalar);
                        if (!(queryResult is DBNull))
                            attractSum = 0;
                        queryResult = db.ExecQuery(string.Format(factAttractSumQuery1, creditRow["ID"]), QueryResultTypes.Scalar,
                            new System.Data.OleDb.OleDbParameter("p0", currentYearDate),
                            new System.Data.OleDb.OleDbParameter("p1", periodTime));
                        if (!(queryResult is DBNull))
                            attractSum = Convert.ToDecimal(queryResult);

                        DataTable dtFactDate = (DataTable)db.ExecQuery(string.Format(factDateQuery, creditRow["ID"]), QueryResultTypes.DataTable);
                        string[] factDates = new string[dtFactDate.Rows.Count];
                        for (int i = 0; i <= dtFactDate.Rows.Count - 1; i++)
                        {
                            factDates[i] = Convert.ToDateTime(dtFactDate.Rows[i][0]).ToShortDateString();
                        }

                        DataTable dtPercents = (DataTable)db.ExecQuery(string.Format(percentsQuery, creditRow["ID"]), QueryResultTypes.DataTable);
                        DataRow[] rows = dtPercents.Select(string.Format("ChargeDate < '{0}'", periodTime), "ChargeDate ASC");
                        string[] percents = new string[rows.Length];
                        for (int i = 0; i <= rows.Length - 1; i++)
                        {
                            percents[i] = string.Format("с {0} - {1}",
                                Convert.ToDateTime(rows[i]["ChargeDate"]).ToShortDateString(), rows[i]["CreditPercent"]);
                        }
                        if (percents.Length == 0)
                        {
                            percents = new string[1];
                            percents[0] = string.Format("c {0} - {1}", Convert.ToDateTime(creditRow["StartDate"]).ToShortDateString(), creditRow["CreditPercent"]);
                        }

                        List<string> furtherConvention = new List<string>();
                        DataTable dtFurtherConvention = (DataTable)db.ExecQuery(string.Format(futherConventionQuery, creditRow["ID"]),
                            QueryResultTypes.DataTable, new DbParameterDescriptor("p0", periodTime));
                        foreach (DataRow row in dtFurtherConvention.Rows)
                        {
                            furtherConvention.Add(string.Format("№ {0} от {1}", row[0], Convert.ToDateTime(row[1]).ToShortDateString()));
                        }

                        object collateralName = db.ExecQuery(string.Format(collateralQuery, creditRow["ID"]), QueryResultTypes.Scalar);

                        DataRow newRow = dtDebtorCredits.NewRow();
                        newRow.BeginEdit();
                        newRow["ID"] = entity.GetGeneratorNextValue;
                        newRow["RefVariant"] = currentVariantId;
                        newRow["SourceID"] = currentSourceId;
                        newRow["RefRegion"] = currentRegion;
                        newRow["RegNum"] = creditRow["RegNum"];
                        newRow["Num"] = creditRow["Num"];
                        newRow["ContractDate"] = creditRow["ContractDate"];
                        newRow["Occasion"] = creditRow["Occasion"];
                        newRow["Sum"] = creditRow["Sum"];
                        newRow["CreditPercent"] = string.Join("; ", percents);
                        if (collateralName != null && collateralName != DBNull.Value)
                            newRow["Collateral"] = collateralName;
                        newRow["FurtherConvention"] = String.Join("; ", furtherConvention.ToArray());
                        newRow["StartDate"] = creditRow.IsNull("StartDate") ? DBNull.Value.ToString() : Convert.ToDateTime(creditRow["StartDate"]).ToShortDateString();
                        newRow["EndDate"] = creditRow.IsNull("EndDate") ? DBNull.Value.ToString() : Convert.ToDateTime(creditRow["EndDate"]).ToShortDateString();
                        newRow["FactDate"] = string.Join("; ", factDates);
                        newRow["Attract"] = attractSum;
                        newRow["Discharge"] = discharge;// сумма по погашению кредита
                        newRow["PlanService"] = planService;// сумма процентов начисленных до определенной даты
                        newRow["FactService"] = factService;// сумма процентов, уплаченных до определенной даты
                        newRow["CapitalDebt"] = capitalDebt;// вычислимое поле
                        newRow["ServiceDebt"] = serviceDebt;// вычислимое поле
                        newRow["StaleDebt"] = staleDebt;// вычислимое поле
                        newRow["RefTypeCredit"] = creditRow["RefSTypeCredit"];
                        newRow["RefTypeContract"] = creditRow["RefTypeContract"];
                        newRow["RefOrganizations"] = creditRow["RefOrganizations"];
                        newRow["FromFinSource"] = 1;
                        newRow.EndEdit();
                        dtDebtorCredits.Rows.Add(newRow);
                    }
                    du.Update(ref dtDebtorCredits);
                }
            }
        }

        /// <summary>
        /// перенос кредитов из других бюджетов
        /// </summary>
        /// <param name="periodTime"></param>
        /// <param name="currentYear"></param>
        /// <param name="CreditsFilter"></param>
        /// <param name="currentVariantId"></param>
        /// <param name="currentSourceId"></param>
        /// <param name="currentRegion"></param>
        private void TransfertBudCredits(DateTime periodTime, int currentYear, string CreditsFilter, int currentVariantId, int currentSourceId, int currentRegion)
        {
            IEntity creditEntity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.f_S_Creditincome);
            IEntity creditTypeEntity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.d_S_TypeContract);

            // запрос по кредитам
            string creditQuery = "select credit.ID, credit.Num, credit.RegNum, credit.StartDate, credit.EndDate, credit.RenewalDate, credit.ContractDate, " +
                "credit.Sum, credit.Occasion, credit.CreditPercent, credit.PercentRate, credit.RefSTypeCredit, credit.RefTypeContract, credit.RefOrganizations, creditType.Name from {0} credit, {1} creditType where " +
                "credit.RefTypeContract = creditType.ID and (RefVariant = 0 or RefVariant = -2) and " + CreditsFilter;

            // по каждому кредиту получаем информацию о деталях, обрабатываем все и добавляем запись в долговую книгу
            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                DateTime currentYearDate = new DateTime(currentYear, 1, 1);
                IEntity entity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.f_S_SchBCreditincome);
                using (IDataUpdater du = entity.GetDataUpdater("1 = 2", null, null))
                {
                    DataTable dtDebtorCredits = new DataTable();
                    du.Fill(ref dtDebtorCredits);
                    DataTable dtCredits = (DataTable)db.ExecQuery(string.Format(creditQuery, creditEntity.FullDBName,
                        creditTypeEntity.FullDBName), QueryResultTypes.DataTable);

                    const string serviceDebtQuery1 = "select Sum(sum) from t_S_PlanServiceCI where RefCreditInc = {0} and EndDate <= ?";
                    const string serviceDebtQuery2 = "select Sum(sum) from t_S_FactPercentCI where RefCreditInc = {0} and FactDate <= ?";
                    const string serviceDebtQuery3 = "select Sum(sum) from t_S_ChargePenaltyDebtCI where RefCreditInc = {0} and StartDate <= ?";
                    const string serviceDebtQuery4 = "select Sum(sum) from t_S_FactPenaltyDebtCI where RefCreditInc = {0} and FactDate <= ?";
                    const string serviceDebtQuery5 = "select Sum(sum) from t_S_CIChargePenaltyPercent where RefCreditInc = {0} and StartDate <= ?";
                    const string serviceDebtQuery6 = "select Sum(sum) from t_S_FactPenaltyPercentCI where RefCreditInc = {0} and FactDate <= ?";

                    const string capitalDebtQuery1 = "select Sum(sum) from t_S_FactAttractCI where RefCreditInc = {0} and FactDate <= ?";
                    const string capitalDebtQuery2 = "select Sum(sum) from t_S_FactDebtCI where RefCreditInc = {0} and FactDate <= ?";

                    const string collateralQuery = "select Name from t_S_CollateralCI where RefCreditInc = {0}";
                    // данные из журнала ставок процентов
                    const string percentsQuery = "select CreditPercent, ChargeDate from t_S_JournalPercentCI where RefCreditInc = {0}";

                    const string factDateQuery = "select FactDate from t_S_FactDebtCI where RefCreditInc = {0} and FactDate Between ? and ? order by FactDate";

                    const string dischargeQuery =
                        "select Sum(sum) from t_S_FactDebtCI where RefCreditInc = {0} and FactDate BETWEEN ? and ?";

                    const string planServiceQuery1 =
                        "select Sum(sum) from t_S_PlanServiceCI where RefCreditInc = {0} and EndDate between ? and ?";
                    const string planServiceQuery2 =
                        "select Sum(sum) from t_S_ChargePenaltyDebtCI where RefCreditInc = {0} and StartDate BETWEEN ? and ?";
                    const string planServiceQuery3 =
                        "select Sum(sum) from t_S_CIChargePenaltyPercent where RefCreditInc = {0} and StartDate BETWEEN ? and ?";

                    const string factServiceQuery1 =
                        "select Sum(sum) from t_S_FactPercentCI where RefCreditInc = {0} and FactDate BETWEEN ? and ?";
                    const string factServiceQuery2 =
                        "select Sum(sum) from t_S_FactPenaltyDebtCI where RefCreditInc = {0} and FactDate BETWEEN ? and ?";
                    const string factServiceQuery3 =
                        "select Sum(sum) from t_S_FactPenaltyPercentCI where RefCreditInc = {0} and FactDate BETWEEN ? and ?";

                    const string attractQuery = "select Sum from t_S_FactAttractCI where RefCreditInc = {0} and FactDate BETWEEN ? and ?";

                    foreach (DataRow creditRow in dtCredits.Rows)
                    {
                        DateTime startDate = Convert.ToDateTime(creditRow["StartDate"]);
                        DateTime endDate = DateTime.MinValue;
                        if (!DateTime.TryParse(creditRow["RenewalDate"].ToString(), out endDate))
                            DateTime.TryParse(creditRow["EndDate"].ToString(), out endDate);

                        if (startDate > periodTime || endDate.Year < periodTime.Year)
                            continue;

                        decimal capitalDebt = 0;
                        object queryResult = db.ExecQuery(string.Format(capitalDebtQuery1, creditRow["ID"]),
                            QueryResultTypes.Scalar, new DbParameterDescriptor("p0", periodTime, DbType.DateTime));
                        if (!(queryResult is DBNull))
                            capitalDebt += Convert.ToDecimal(queryResult);
                        queryResult = db.ExecQuery(string.Format(capitalDebtQuery2, creditRow["ID"]), QueryResultTypes.Scalar,
                            new DbParameterDescriptor("p0", periodTime));
                        if (!(queryResult is DBNull))
                            capitalDebt -= Convert.ToDecimal(queryResult);

                        decimal serviceDebt = 0;
                        queryResult = db.ExecQuery(string.Format(serviceDebtQuery1, creditRow["ID"]), QueryResultTypes.Scalar,
                            new DbParameterDescriptor("p0", periodTime));
                        if (!(queryResult is DBNull))
                            serviceDebt = Convert.ToDecimal(queryResult);
                        queryResult = db.ExecQuery(string.Format(serviceDebtQuery2, creditRow["ID"]), QueryResultTypes.Scalar,
                            new DbParameterDescriptor("p0", periodTime));
                        if (!(queryResult is DBNull))
                            serviceDebt -= Convert.ToDecimal(queryResult);
                        queryResult = db.ExecQuery(string.Format(serviceDebtQuery3, creditRow["ID"]), QueryResultTypes.Scalar,
                            new DbParameterDescriptor("p0", periodTime));
                        if (!(queryResult is DBNull))
                            serviceDebt += Convert.ToDecimal(queryResult);
                        queryResult = db.ExecQuery(string.Format(serviceDebtQuery4, creditRow["ID"]), QueryResultTypes.Scalar,
                            new DbParameterDescriptor("p0", periodTime));
                        if (!(queryResult is DBNull))
                            serviceDebt -= Convert.ToDecimal(queryResult);
                        queryResult = db.ExecQuery(string.Format(serviceDebtQuery5, creditRow["ID"]), QueryResultTypes.Scalar,
                            new DbParameterDescriptor("p0", periodTime));
                        if (!(queryResult is DBNull))
                            serviceDebt += Convert.ToDecimal(queryResult);
                        queryResult = db.ExecQuery(string.Format(serviceDebtQuery6, creditRow["ID"]), QueryResultTypes.Scalar,
                            new DbParameterDescriptor("p0", periodTime));
                        if (!(queryResult is DBNull))
                            serviceDebt -= Convert.ToDecimal(queryResult);

                        decimal discharge = 0;
                        queryResult = db.ExecQuery(string.Format(dischargeQuery, creditRow["ID"]), QueryResultTypes.Scalar,
                            new DbParameterDescriptor("p0", currentYearDate),
                            new DbParameterDescriptor("p1", periodTime));
                        if (!(queryResult is DBNull))
                            discharge = Convert.ToDecimal(queryResult);

                        decimal planService = 0;
                        queryResult = db.ExecQuery(string.Format(planServiceQuery1, creditRow["ID"]), QueryResultTypes.Scalar,
                            new DbParameterDescriptor("p0", currentYearDate),
                            new DbParameterDescriptor("p1", periodTime));
                        if (!(queryResult is DBNull))
                            planService = Convert.ToDecimal(queryResult);
                        queryResult = db.ExecQuery(string.Format(planServiceQuery2, creditRow["ID"]), QueryResultTypes.Scalar,
                            new DbParameterDescriptor("p0", currentYearDate),
                            new DbParameterDescriptor("p1", periodTime));
                        if (!(queryResult is DBNull))
                            planService += Convert.ToDecimal(queryResult);
                        queryResult = db.ExecQuery(string.Format(planServiceQuery3, creditRow["ID"]), QueryResultTypes.Scalar,
                            new DbParameterDescriptor("p0", currentYearDate),
                            new DbParameterDescriptor("p1", periodTime));
                        if (!(queryResult is DBNull))
                            planService += Convert.ToDecimal(queryResult);

                        decimal factService = 0;
                        queryResult = db.ExecQuery(string.Format(factServiceQuery1, creditRow["ID"]), QueryResultTypes.Scalar,
                            new DbParameterDescriptor("p0", currentYearDate),
                            new System.Data.OleDb.OleDbParameter("p1", periodTime));
                        if (!(queryResult is DBNull))
                            factService = Convert.ToDecimal(queryResult);
                        queryResult = db.ExecQuery(string.Format(factServiceQuery2, creditRow["ID"]), QueryResultTypes.Scalar,
                            new DbParameterDescriptor("p0", currentYearDate),
                            new DbParameterDescriptor("p1", periodTime));
                        if (!(queryResult is DBNull))
                            factService += Convert.ToDecimal(queryResult);
                        queryResult = db.ExecQuery(string.Format(factServiceQuery3, creditRow["ID"]), QueryResultTypes.Scalar,
                            new DbParameterDescriptor("p0", currentYearDate),
                            new DbParameterDescriptor("p1", periodTime));
                        if (!(queryResult is DBNull))
                            factService += Convert.ToDecimal(queryResult);

                        decimal attractSum = 0;
                        queryResult = db.ExecQuery(string.Format(attractQuery, creditRow["ID"]), QueryResultTypes.Scalar,
                            new DbParameterDescriptor("p0", currentYearDate),
                            new DbParameterDescriptor("p1", periodTime));
                        if (!(queryResult is DBNull))
                            attractSum = Convert.ToDecimal(queryResult);

                        DataTable dtFactDate = (DataTable)db.ExecQuery(string.Format(factDateQuery, creditRow["ID"]), QueryResultTypes.DataTable,
                            new DbParameterDescriptor("p0", new DateTime(currentYear, 01, 01)),
                            new DbParameterDescriptor("p1", new DateTime(currentYear, 12, 31)));
                        string[] factDates = new string[dtFactDate.Rows.Count];
                        for (int i = 0; i <= dtFactDate.Rows.Count - 1; i++)
                        {
                            factDates[i] = Convert.ToDateTime(dtFactDate.Rows[i][0]).ToShortDateString();
                        }

                        DataTable dtPercents = (DataTable)db.ExecQuery(string.Format(percentsQuery, creditRow["ID"]), QueryResultTypes.DataTable);
                        DataRow[] rows = dtPercents.Select(string.Empty, "ChargeDate ASC");
                        string[] percents = new string[rows.Length];
                        for (int i = 0; i <= rows.Length - 1; i++)
                        {
                            percents[i] = string.Format("с {0} - {1}",
                                Convert.ToDateTime(rows[i]["ChargeDate"]).ToShortDateString(), rows[i]["CreditPercent"]);
                        }
                        if (percents.Length == 0)
                        {
                            percents = new string[1];
                            percents[0] = string.Format("c {0} - {1}", Convert.ToDateTime(creditRow["StartDate"]).ToShortDateString(), creditRow["CreditPercent"]);
                        }

                        object collateral = db.ExecQuery(string.Format(collateralQuery, creditRow["ID"]), QueryResultTypes.Scalar);

                        DataRow newRow = dtDebtorCredits.NewRow();
                        newRow.BeginEdit();
                        newRow["ID"] = entity.GetGeneratorNextValue;
                        newRow["RefVariant"] = currentVariantId;
                        newRow["SourceID"] = currentSourceId;
                        newRow["RefRegion"] = currentRegion;
                        newRow["RegNum"] = creditRow["RegNum"];
                        newRow["Num"] = creditRow["Num"];
                        newRow["ContractDate"] = creditRow["ContractDate"];
                        newRow["Occasion"] = creditRow["Occasion"];
                        newRow["Sum"] = creditRow["Sum"];
                        newRow["CreditPercent"] = GetBudPercent(creditRow);//string.Join("; ", percents);
                        if (collateral != null && collateral != DBNull.Value)
                            newRow["Collateral"] = collateral;
                        newRow["StartDate"] = creditRow.IsNull("StartDate") ? DBNull.Value.ToString() : Convert.ToDateTime(creditRow["StartDate"]).ToShortDateString();
                        newRow["EndDate"] = creditRow.IsNull("EndDate") ? DBNull.Value.ToString() : Convert.ToDateTime(creditRow["EndDate"]).ToShortDateString();
                        newRow["FactDate"] = factDates.Length == 0 ? DBNull.Value.ToString() : string.Join("; ", factDates);
                        newRow["Attract"] = attractSum;
                        newRow["Discharge"] = discharge;// сумма по погашению кредита
                        newRow["PlanService"] = planService;// сумма процентов начисленных до определенной даты
                        newRow["FactService"] = factService;// сумма процентов, уплаченных до определенной даты
                        newRow["CapitalDebt"] = capitalDebt;// вычислимое поле
                        newRow["ServiceDebt"] = serviceDebt;// вычислимое поле
                        newRow["StaleDebt"] = 0;//staleDebt;
                        newRow["RefTypeCredit"] = creditRow["RefSTypeCredit"];
                        newRow["RefTypeContract"] = creditRow["RefTypeContract"];
                        newRow["RefOrganizations"] = creditRow["RefOrganizations"];
                        newRow["FromFinSource"] = 1;
                        newRow.EndEdit();
                        dtDebtorCredits.Rows.Add(newRow);
                    }
                    du.Update(ref dtDebtorCredits);
                }
            }
        }

        private string GetBudPercent(DataRow creditRow)
        {
            if (creditRow.IsNull("PercentRate"))
            {
                return String.Format("{0} %", creditRow["CreditPercent"]);
            }
            return String.Format("{0} ({1} %)", GetFraction(Convert.ToDecimal(creditRow["PercentRate"])),
                                 creditRow["PercentRate"]);
        }

        /// <summary>
        /// перевод деятичной дроби в обыкновенную
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string GetFraction(decimal value)
        {
            decimal aliquot = Math.Truncate(value);
            decimal fractional = value - aliquot;
            if (fractional == 0)
                return String.Format("{0}/{1}", aliquot, 1);

            fractional = Convert.ToDecimal(fractional.ToString().Remove(0, 2).Trim('0'));
            int m = Convert.ToInt32(aliquot*(fractional.ToString().Length)*10 + fractional);
            int n = Convert.ToInt32(Math.Exp(10*Math.Log(fractional.ToString().Length - 1)) * 10); 
            int k = n > m ? m : n;
            int nod = 0;
            for (int i = 1; i <= k ; i++) 
                if ((n % i == 0) && (m % i == 0))
                    nod = i;
            m = m / nod;
            n = n / nod;
            return String.Format("{0}/{1}", m, n);

            //StrToInt(FloatToStr(i*length(FloatToStr(f))*10+f))
        }
    }
}
