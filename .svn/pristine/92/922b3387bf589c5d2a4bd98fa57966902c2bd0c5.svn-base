using System;
using System.Collections.Generic;
using Krista.FM.Client.Reports.Database.ClsData;
using Krista.FM.Client.Reports.Database.FactTables;
using Krista.FM.Client.Reports.UFK;

namespace Krista.FM.Client.Reports.Month.Queries
{
    class QPlanIncomeParams
    {
        public string Period { get; set; }
        public string KD { get; set; }
        public string Lvl { get; set; }
        public string Variant { get; set; }
        public string KVSR { get; set; }

        public QPlanIncomeParams()
        {
            Period = String.Empty;
            KD = String.Empty;
            Lvl = String.Empty;
            Variant = "-2";
            KVSR = String.Empty;
        }
    }

    public enum QPlanIncomeGroup
    {
        Region,
        Kd,
        Period,
        Lvl
    }

    class QPlanIncomeDivide
    {
        public static string GroupBy(QPlanIncomeParams qParams, List<QPlanIncomeGroup> groupList)
        {
            const string templateFilter = " and {2}.{0} in ({1})";
            const string sptGroups = ", ";
            var fltKVSR = String.Empty;
            var fltKBK = String.Empty;

            var infoFact = new GroupHelper()
            {
                Prefix = QFilterHelper.fltPrefixName,
                EntityKey = f_D_FOPlanIncDivide.InternalKey
            };

            var infoKD = new GroupHelper
            {
                Prefix = "kd",
                EntityKey = d_KD_PlanIncomes.InternalKey
            };

            var infoRgn = new GroupHelper
            {
                Prefix = "rgn",
                EntityKey = d_Regions_Plan.internalKey
            };

            var infoKVSR = new GroupHelper
            {
                Prefix = "kvsr",
                EntityKey = d_KVSR_Plan.InternalKey
            };

            if (qParams.KVSR.Length != 0)
            {
                fltKVSR = String.Format(templateFilter, d_KVSR_Plan.RefBridge, qParams.KVSR, infoKVSR.Prefix);
            }

            if (qParams.KD.Length != 0)
            {
                fltKBK = String.Format(templateFilter, d_KD_PlanIncomes.RefBridge, qParams.KD, infoKD.Prefix);
            }

            var strGroup = String.Empty;

            foreach (var group in groupList)
            {
                var tblPrefix = QFilterHelper.fltPrefix;
                var tblFieldName = String.Empty;

                switch (group)
                {
                    case QPlanIncomeGroup.Kd:
                        tblPrefix = infoKD.FullPrefix;
                        tblFieldName = d_KD_PlanIncomes.RefBridge;
                        break;
                    case QPlanIncomeGroup.Period:
                        tblFieldName = f_D_FOPlanIncDivide.RefYearDayUNV;
                        break;
                    case QPlanIncomeGroup.Lvl:
                        tblFieldName = f_D_FOPlanIncDivide.RefBudLevel;
                        break;
                    case QPlanIncomeGroup.Region:
                        tblPrefix = infoRgn.FullPrefix;
                        tblFieldName = d_Regions_Plan.RefBridge;
                        break;
                }

                strGroup = String.Join(sptGroups, new[] { strGroup, String.Format("{0}{1}", tblPrefix, tblFieldName) });
            }

            strGroup = ReportDataServer.Trim(strGroup, sptGroups);

            var queryText =
                String.Format(
                    @"
                    select 
                        Sum({31}.{1}) as {1}, 
                        {0}, 
                        Sum({31}.{10}) as {10}, 
                        Sum({31}.{11}) as {11},
                        Sum({31}.{12}) as {12}
                    from 
                        {16} {31}, {17} {32}, {18} {33}, {19} {34}
                    where 
                        {31}.{5} = {24} and
                        {33}.{8} in (3, 4, 5, 6, 7) and
                        {31}.{2} in ({23}) and 
                        {21} and 
                        {31}.{4} = {33}.id and 
                        {31}.{6} = {32}.id and
                        {31}.{13} = {34}.id 
                        {22}
                        {25}
                    group by 
                        {0}",
                    strGroup, // 0
                    f_D_FOPlanIncDivide.YearPlan, // 1
                    f_D_FOPlanIncDivide.RefBudLevel, // 2
                    f_D_FOPlanIncDivide.RefYearDayUNV, // 3
                    f_D_FOPlanIncDivide.RefRegions, // 4
                    f_D_FOPlanIncDivide.RefVariant, // 5
                    f_D_FOPlanIncDivide.RefKD, // 6
                    d_KD_PlanIncomes.RefBridge, // 7
                    d_Regions_Plan.RefTerr, // 8
                    d_Regions_Plan.RefBridge, // 9
                    f_D_FOPlanIncDivide.Estimate, // 10
                    f_D_FOPlanIncDivide.Forecast, // 11
                    f_D_FOPlanIncDivide.TaxResource, // 12
                    f_D_FOPlanIncDivide.RefKVSR, // 13
                    String.Empty, // 14
                    String.Empty, // 15
                    infoFact.Entity.FullDBName, // 16
                    infoKD.Entity.FullDBName, // 17
                    infoRgn.Entity.FullDBName, // 18
                    infoKVSR.Entity.FullDBName, // 19
                    String.Empty, // 20
                    qParams.Period, // 21
                    fltKBK, // 22
                    qParams.Lvl, // 23
                    qParams.Variant, // 24
                    fltKVSR, // 25
                    String.Empty, // 26
                    String.Empty, // 27
                    String.Empty, // 28
                    String.Empty, // 29
                    String.Empty, // 30
                    infoFact.Prefix, // 31
                    infoKD.Prefix, // 32
                    infoRgn.Prefix, // 33
                    infoKVSR.Prefix // 34
                    );

            return queryText;
        }
    }
}
