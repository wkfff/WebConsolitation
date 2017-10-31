using System;
using Krista.FM.Server.Dashboards.Core;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public class CashPlanHelper
    {

        public static void InitRegionSettings(CustomParams UserParams)
        {
            CustomParam KIFBudgetCredits = UserParams.CustomParam("KIF_budget_credits");
            CustomParam KIFRestsChanges = UserParams.CustomParam("KIF_rests_changes");
            CustomParam KIFOtherFinSources = UserParams.CustomParam("KIF_other_fin_sources");

            KIFBudgetCredits.Value = RegionSettingsHelper.Instance.GetPropertyValue("KIFBudgetCredits");
            KIFRestsChanges.Value = RegionSettingsHelper.Instance.GetPropertyValue("KIFRestsChanges");
            KIFOtherFinSources.Value = RegionSettingsHelper.Instance.GetPropertyValue("KIFOtherFinSources");

            bool fns28nSplitting = Convert.ToBoolean(RegionSettingsHelper.Instance.GetPropertyValue("FNS28nSplitting"));
            CustomParam cubeName = UserParams.CustomParam("cube_name");
            cubeName.Value = fns28nSplitting ? "ФНС_28н_с расщеплением" : "ФНС_28н_без расщепления";

            CustomParam fnsBudgetLevelFilter = UserParams.CustomParam("fns_budget_level_filter");
            fnsBudgetLevelFilter.Value = fns28nSplitting ? ",[Уровни бюджетов].[Все].[Все уровни].[Конс.бюджет субъекта].[Конс.бюджет МО]" : String.Empty;

            CustomParam settlementLevel = UserParams.CustomParam("settlement_level");
            settlementLevel.Value = RegionSettingsHelper.Instance.GetPropertyValue("SettlementLevel");

            CustomParam regionsConsolidateLevel = UserParams.CustomParam("regions_consolidate_level");
            regionsConsolidateLevel.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionsConsolidateLevel");

            CustomParam incomeTotal = UserParams.CustomParam("income_total");
            incomeTotal.Value = RegionSettingsHelper.Instance.GetPropertyValue("IncomeTotal");

            CustomParam outcomeFKRTotal = UserParams.CustomParam("outcome_FKR_total");
            outcomeFKRTotal.Value = RegionSettingsHelper.Instance.GetPropertyValue("OutcomeFKRTotal");

            CustomParam regionsLevel = UserParams.CustomParam("regions_level");
            regionsLevel.Value = RegionSettingsHelper.Instance.RegionsLevel;

            UserParams.IncomesKDAllLevel.Value = RegionSettingsHelper.Instance.IncomesKDAllLevel;

            CustomParam populationDimension = UserParams.CustomParam("population_measure");
            populationDimension.Value = RegionSettingsHelper.Instance.PopulationMeasure;
        }

    }
}
