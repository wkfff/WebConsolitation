using System;
using System.Collections.Generic;
using System.Text;

namespace Krista.FM.Server.Dashboards.Core
{
    public static class CubeInfoHelper
    {
        private static CubeInfo monthReportIncomesInfo;
        private static CubeInfo monthReportOutcomesInfo;
        private static CubeInfo budgetIncomesFactInfo;
        private static CubeInfo budgetOutocmesFactInfo;
        private static CubeInfo foDebtKzDz;
        private static CubeInfo fkMonthReportIncomesInfo;
        private static CubeInfo fkMonthReportOutcomesInfo;
        private static CubeInfo fns28nSplitInfo;
        private static CubeInfo fns28nNonSplitInfo;
        private static CubeInfo moFinPassportInfo;
        private static CubeInfo foBudgetOutcomesInfo;
        private static CubeInfo foYearReportStatesInfo;
        private static CubeInfo foYearReportBalansInfo;
        private static CubeInfo foMonthReportDebtInfo;
        private static CubeInfo fstTariffsAndRegulationsInfo;

        public static CubeInfo MonthReportIncomesInfo
        {
            get
            {
                if (monthReportIncomesInfo == null)
                {
                    FillMonthReportIncomesInfo();
                }
                return monthReportIncomesInfo;
            }
        }

        public static CubeInfo MonthReportOutcomesInfo
        {
            get
            {
                if (monthReportOutcomesInfo == null)
                {
                    FillMonthReportOutcomesInfo();
                }
                return monthReportOutcomesInfo;
            }
        }

        public static CubeInfo BudgetIncomesFactInfo
        {
            get
            {
                if (budgetIncomesFactInfo == null)
                {
                    FillBudgetIncomesFactInfo();
                }
                return budgetIncomesFactInfo;
            }
        }

        public static CubeInfo BudgetOutocmesFactInfo
        {
            get
            {
                if (budgetOutocmesFactInfo == null)
                {
                    FillBudgetOutocmesFactInfo();
                }
                return budgetOutocmesFactInfo;
            }
        }

        public static CubeInfo FoDebtKzDz
        {
            get
            {
                if (foDebtKzDz == null)
                {
                    FillFoDebtKzDz();
                }
                return foDebtKzDz;
            }
        }

        public static CubeInfo FkMonthReportIncomesInfo
        {
            get
            {
                if (fkMonthReportIncomesInfo == null)
                {
                    FillFkMonthReportIncomesInfo();
                }
                return fkMonthReportIncomesInfo;
            }
        }

        public static CubeInfo FkMonthReportOutcomesInfo
        {
            get
            {
                if (fkMonthReportOutcomesInfo == null)
                {
                    FillFkMonthReportOutcomesInfo();
                }
                return fkMonthReportOutcomesInfo;
            }
        }

        public static CubeInfo Fns28nSplitInfo
        {
            get
            {
                if (fns28nSplitInfo == null)
                {
                    FillFns28nSplitInfo();
                }
                return fns28nSplitInfo;
            }
        }

        public static CubeInfo Fns28nNonSplitInfo
        {
            get
            {
                if (fns28nNonSplitInfo == null)
                {
                    FillFns28nNonSplitInfo();
                }
                return fns28nNonSplitInfo;
            }
        }

        public static CubeInfo MoFinPassportInfo
        {
            get
            {
                if (moFinPassportInfo == null)
                {
                    FillMoFinPassportInfo();
                }
                return moFinPassportInfo;
            }
        }

        public static CubeInfo FoBudgetOutcomesInfo
        {
            get
            {
                if (foBudgetOutcomesInfo == null)
                {
                    FillFoBudgetOutcomesInfo();
                }
                return foBudgetOutcomesInfo;
            }
        }

        public static CubeInfo FoYearReportStatesInfo
        {
            get
            {
                if (foYearReportStatesInfo == null)
                {
                    FillFoYearReportStatesInfo();
                }
                return foYearReportStatesInfo;
            }
        }

        public static CubeInfo FoYearReportBalansInfo
        {
            get
            {
                if (foYearReportBalansInfo == null)
                {
                    FillFoYearReportBalansInfo();
                }
                return foYearReportBalansInfo;
            }
        }

        public static CubeInfo FoMonthReportDebtInfo
        {
            get
            {
                if (foMonthReportDebtInfo == null)
                {
                    FillFoMonthReportDebtInfo();
                }
                return foMonthReportDebtInfo;
            }
        }

        public static CubeInfo FstTariffsAndRegulationsInfo
        {
            get
            {
                if (fstTariffsAndRegulationsInfo == null)
                {
                    FillFstTariffsAndRegulationsInfo();
                }
                return fstTariffsAndRegulationsInfo;
            }
        }

        private static void FillMonthReportIncomesInfo()
        {
            monthReportIncomesInfo = new CubeInfo(DataProviders.DataProvidersFactory.PrimaryMASDataProvider, "МесОтч_Доходы", "MonthReportIncomesLastDate");
            monthReportIncomesInfo.AddDimensionInfo("КД.Сопоставимый", "IncomesKDElementList");
        }

        private static void FillMonthReportOutcomesInfo()
        {
            monthReportOutcomesInfo = new CubeInfo(DataProviders.DataProvidersFactory.PrimaryMASDataProvider, "МесОтч_Расходы", "MonthReportOutcomesLastDate");
            monthReportOutcomesInfo.AddDimensionInfo("РзПр.Сопоставимый", "OutcomesRzPrElementList");
            monthReportOutcomesInfo.AddDimensionInfo("КОСГУ.Сопоставимый", "OutcomesKOSGUElementList");
        }

        private static void FillBudgetIncomesFactInfo()
        {
            budgetIncomesFactInfo = new CubeInfo(DataProviders.DataProvidersFactory.PrimaryMASDataProvider, "ФО_АС Бюджет_Факт доходов", "BudgetIncomesFactLastDate");
        }

        private static void FillBudgetOutocmesFactInfo()
        {
            budgetOutocmesFactInfo = new CubeInfo(DataProviders.DataProvidersFactory.PrimaryMASDataProvider, "ФО_АС Бюджет_КазнИсп_Факт расхода", "BudgetOutcomesFactLastDate");
            budgetOutocmesFactInfo.AddDimensionInfo("Расходы.Базовый", "BaseOutcomesElementList");
            budgetOutocmesFactInfo.AddDimensionInfo("Администратор.Сопоставим", "AdminElementList");
            budgetOutocmesFactInfo.AddDimensionInfo("КОСГУ.Сопоставимый", "KOSGUElementList");
        }

        private static void FillFoDebtKzDz()
        {
            foDebtKzDz = new CubeInfo(DataProviders.DataProvidersFactory.PrimaryMASDataProvider, "ФО_Задолженность_КзДз", "FoDebtKzDzLastDate");
        }

        private static void FillFkMonthReportIncomesInfo()
        {
            fkMonthReportIncomesInfo = new CubeInfo(DataProviders.DataProvidersFactory.SecondaryMASDataProvider, "ФК_МесОтч_Доходы", "FkMonthReportIncomesLastDate");
        }

        private static void FillFkMonthReportOutcomesInfo()
        {
            fkMonthReportOutcomesInfo = new CubeInfo(DataProviders.DataProvidersFactory.SecondaryMASDataProvider, "ФК_МесОтч_Расходы", "FkMonthReportOutcomesLastDate");
        }

        private static void FillFns28nSplitInfo()
        {
            fns28nSplitInfo = new CubeInfo(DataProviders.DataProvidersFactory.PrimaryMASDataProvider, "ФНС_28н_с расщеплением", "fns28nSplitLastDate");
            fns28nSplitInfo.AddDimensionInfo("ОКВЭД.Сопоставимый", "FnsOkvedElementList");
        }

        private static void FillFns28nNonSplitInfo()
        {
            fns28nNonSplitInfo = new CubeInfo(DataProviders.DataProvidersFactory.PrimaryMASDataProvider, "ФНС_28н_без расщепления", "fns28nNonSplitLastDate");
        }

        private static void FillMoFinPassportInfo()
        {
            moFinPassportInfo = new CubeInfo(DataProviders.DataProvidersFactory.PrimaryMASDataProvider, "ФО_Финпаспорт МО", "moFinPassportLastDate");
        }

        private static void FillFoBudgetOutcomesInfo()
        {
            foBudgetOutcomesInfo = new CubeInfo(DataProviders.DataProvidersFactory.PrimaryMASDataProvider, "ФО_Расходы бюджета", "foBudgetOutcomesLastDate");
        }

        private static void FillFoYearReportStatesInfo()
        {
            foYearReportStatesInfo = new CubeInfo(DataProviders.DataProvidersFactory.PrimaryMASDataProvider, "ФО_ГодОтч_Сеть Штаты Контингент", "foYearReportStatesLastDate");
        }

        private static void FillFoYearReportBalansInfo()
        {
            foYearReportBalansInfo = new CubeInfo(DataProviders.DataProvidersFactory.PrimaryMASDataProvider, "ФО_ГодОтч_Баланс", "foYearReportBalansLastDate");
        }

        private static void FillFoMonthReportDebtInfo()
        {
            foMonthReportDebtInfo = new CubeInfo(DataProviders.DataProvidersFactory.PrimaryMASDataProvider, "ФО_МесОтч_Задолженность", "foMonthReportDebtLastDate");
        }

        private static void FillFstTariffsAndRegulationsInfo()
        {
            fstTariffsAndRegulationsInfo = new CubeInfo(DataProviders.DataProvidersFactory.SpareMASDataProvider, "ФСТ_ЖКХ по МО_Тарифы и нормативы", "fstTariffsAndRegulationsLastDate");
        }
    }

    public enum CubeDimensions
    {
        IncomesKD,
        OutcomesRzPr,
        OutcomesKOSGU
    }
}


