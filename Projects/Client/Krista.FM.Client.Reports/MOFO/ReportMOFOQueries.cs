using System;
using Krista.FM.Client.Reports.Database.ClsData;
using Krista.FM.Client.Reports.Database.ClsData.MOFO;
using Krista.FM.Client.Reports.Database.FactTables;
using Krista.FM.Client.Reports.Database.FactTables.MOFO;
using Krista.FM.Client.Reports.UFNS.ReportQueries;

namespace Krista.FM.Client.Reports.MOFO.Queries
{
    public class QRent : QFactTable
    {
        public enum Keys { Period, Day, Okato, Mark, Lvl, Org };

        public QRent(): base(f_F_Rent.TableKey)
        {
            Result = String.Format("Sum({0}.{1}) as Sum, Sum({0}.{2}) as Sum1, Sum({0}.{3}) as {3}",
                                    InfoFact.Prefix,
                                    f_F_Rent.TotalDeb,
                                    f_F_Rent.BeznDeb,
                                    f_F_Rent.Quantity);

            AddSource(Keys.Okato, d_Regions_Plan.internalKey);
            
            AddParam(Keys.Period, null, null);
            AddParam(Keys.Day, f_F_Rent.RefYearDayUNV, null);
            AddParam(Keys.Okato, f_F_Rent.RefRegions, d_Regions_Plan.RefBridge);
            AddParam(Keys.Mark, f_F_Rent.RefMarks, null);
            AddParam(Keys.Lvl, f_F_Rent.RefBudgetLevels, null);
            AddParam(Keys.Org, f_F_Rent.RefOrganizations, null);
        }
    }

    public class QMarksTaxBenPay : QFactTable
    {
        public enum Keys { Period, Day, Okato, Mark, Lvl, Org };

        public QMarksTaxBenPay()
            : base(f_Marks_TaxBenPay.TableKey)
        {
            Result = String.Format("Sum({0}.{1}) as {1}, Sum({0}.{2}) as {2}",
                                    InfoFact.Prefix,
                                    f_Marks_TaxBenPay.SumPayment,
                                    f_Marks_TaxBenPay.SumReduction);

            AddSource(Keys.Okato, d_Regions_Plan.internalKey);

            AddParam(Keys.Period, null, null);
            AddParam(Keys.Day, f_Marks_TaxBenPay.RefYearDayUNV, null);
            AddParam(Keys.Okato, f_Marks_TaxBenPay.RefRegions, d_Regions_Plan.RefBridge);
            AddParam(Keys.Mark, f_Marks_TaxBenPay.RefMarks, null);
            AddParam(Keys.Lvl, f_Marks_TaxBenPay.RefBdgLevels, null);
            AddParam(Keys.Org, f_Marks_TaxBenPay.RefOrg, null);
        }
    }

    public class QFOPlanIncDivide : QFactTable
    {
        public enum Keys { Period, Day, Lvl, KD, KVSR, Okato, Variant };

        public QFOPlanIncDivide()
            : base(f_D_FOPlanIncDivide.InternalKey)
        {
            Result = String.Format("Sum({0}.{1}) as {1}, Sum({0}.{2}) as {2}, Sum({0}.{3}) as {3}",
                                    InfoFact.Prefix,
                                    f_D_FOPlanIncDivide.YearPlan,
                                    f_D_FOPlanIncDivide.TaxResource,
                                    f_D_FOPlanIncDivide.Estimate);

            AddSource(Keys.KD, d_KD_PlanIncomes.InternalKey);
            AddSource(Keys.KVSR, d_KVSR_Plan.InternalKey);
            AddSource(Keys.Okato, d_Regions_Plan.internalKey);

            AddParam(Keys.Period, null, null);
            AddParam(Keys.Day, f_D_FOPlanIncDivide.RefYearDayUNV, null);
            AddParam(Keys.Okato, f_D_FOPlanIncDivide.RefRegions, d_Regions_Plan.RefBridge);
            AddParam(Keys.Lvl, f_D_FOPlanIncDivide.RefBudLevel, null);
            AddParam(Keys.Variant, f_D_FOPlanIncDivide.RefVariant, null);
            AddParam(Keys.KD, f_D_FOPlanIncDivide.RefKD, d_KD_PlanIncomes.RefBridge);
            AddParam(Keys.KVSR, f_D_FOPlanIncDivide.RefKVSR, d_KVSR_Plan.RefBridge);
        }
    }

    public class QFMarksForecast : QFactTable
    {
        public enum Keys { Period, Day, Okato, Lvl, Mark, Variant };

        public QFMarksForecast()
            : base(f_Marks_Forecast.TableKey)
        {
            Result = String.Format("Sum({0}.{1}) as {1}, Sum({0}.{2}) as {2}, Sum({0}.{3}) as {3}, Sum({0}.{4}) as {4}",
                                    InfoFact.Prefix,
                                    f_Marks_Forecast.Fact,
                                    f_Marks_Forecast.Estimate,
                                    f_Marks_Forecast.Forecast,
                                    f_Marks_Forecast.PlanMO);

            AddSource(Keys.Okato, d_Regions_Plan.internalKey);
            AddSource(Keys.Mark, d_Marks_Forecast.InternalKey);
            
            AddParam(Keys.Period, null, null);
            AddParam(Keys.Day, f_Marks_Forecast.RefYearDayUNV, null);
            AddParam(Keys.Lvl, f_Marks_Forecast.RefBudgetLevels, null);
            AddParam(Keys.Variant, f_Marks_Forecast.RefVariant, null);
            AddParam(Keys.Mark, f_Marks_Forecast.RefMarks, d_Marks_Forecast.RefMarksBridge);
            AddParam(Keys.Okato, f_Marks_Forecast.RefRegions, d_Regions_Plan.RefBridge);
        }
    }

    public class QFRental : QFactTable
    {
        public enum Keys { Period, Day, Okato, Lvl, Mark };

        public QFRental()
            : base(f_F_Rental.TableKey)
        {
            Result = String.Format("Sum({0}.{1}) as Sum", InfoFact.Prefix, f_F_Rental.Fact);

            AddSource(Keys.Okato, d_Regions_Plan.internalKey);

            AddParam(Keys.Period, null, null);
            AddParam(Keys.Day, f_F_Rental.RefYearDayUNV, null);
            AddParam(Keys.Lvl, f_F_Rental.RefBudLevel, null);
            AddParam(Keys.Mark, f_F_Rental.RefReceipt, null);
            AddParam(Keys.Okato, f_F_Rental.RefRegions, d_Regions_Plan.RefBridge);
        }
    }

    public class QFIndexCapital : QFactTable
    {
        public enum Keys { Period, Day, Okato, Org };

        public QFIndexCapital()
            : base(f_F_IndexCapital.TableKey)
        {
            Result = String.Format("Sum({0}.{1}) as {1}, Sum({0}.{2}) as {2}, Sum({0}.{3}) as {3}, Sum({0}.{4}) as {4}, Sum({0}.{5}) as {5}, Sum({0}.{6}) as {6}, Sum({0}.{7}) as {7}, Sum({0}.{8}) as {8}",
                                    InfoFact.Prefix,
                                    f_F_IndexCapital.Capital,
                                    f_F_IndexCapital.ClearAsset,
                                    f_F_IndexCapital.ClearProfit,
                                    f_F_IndexCapital.DeductResFund,
                                    f_F_IndexCapital.DeductSpecFund,
                                    f_F_IndexCapital.AddDividend,
                                    f_F_IndexCapital.TransferDividend,
                                    f_F_IndexCapital.DebtDividend
                                  );

            AddSource(Keys.Okato, d_Regions_Plan.internalKey);

            AddParam(Keys.Period, null, null);
            AddParam(Keys.Day, f_F_IndexCapital.RefYearDayUNV, null);
            AddParam(Keys.Okato, f_F_IndexCapital.RefRegions, d_Regions_Plan.RefBridge);
            AddParam(Keys.Org, f_F_IndexCapital.RefOrg, null);
        }
    }

    public class QFMarksChargeRent : QFactTable
    {
        public enum Keys { Period, Day, Okato, Mark };

        public QFMarksChargeRent()
            : base(f_Marks_ChargeRent.TableKey)
        {
            Result = String.Format("Sum({0}.{1}) as {1}, Sum({0}.{2}) as {2}, Sum({0}.{3}) as {3}, Sum({0}.{4}) as {4}, Sum({0}.{5}) as {5}, Sum({0}.{6}) as {6}, Sum({0}.{7}) as {7}",
                                    InfoFact.Prefix,
                                    f_Marks_ChargeRent.Amount,
                                    f_Marks_ChargeRent.RentArea,
                                    f_Marks_ChargeRent.ChargeAnnual,
                                    f_Marks_ChargeRent.Facility,
                                    f_Marks_ChargeRent.FactArrivalAnnual,
                                    f_Marks_ChargeRent.BorrowArea,
                                    f_Marks_ChargeRent.FactArrivalOMSY
                                  );

            AddSource(Keys.Okato, d_Regions_Plan.internalKey);

            AddParam(Keys.Period, null, null);
            AddParam(Keys.Day, f_Marks_ChargeRent.RefYearDayUNV, null);
            AddParam(Keys.Okato, f_Marks_ChargeRent.RefRegions, d_Regions_Plan.RefBridge);
            AddParam(Keys.Mark, f_Marks_ChargeRent.RefMarks, null);
        }
    }

    public class QFIndexProfit : QFactTable
    {
        public enum Keys { Period, Day, Okato, Mark, Org };

        public QFIndexProfit()
            : base(f_F_IndexProfit.TableKey)
        {
            Result = String.Format("Sum({0}.{1}) as {1}, Sum({0}.{2}) as {2}, Sum({0}.{3}) as {3}, Sum({0}.{4}) as {4}, Sum({0}.{5}) as {5}, Sum({0}.{6}) as {6}, Sum({0}.{7}) as {7}, Sum({0}.{8}) as {8}, Sum({0}.{9}) as {9}",
                                    InfoFact.Prefix,
                                    f_F_IndexProfit.SaleProfit,
                                    f_F_IndexProfit.BaseProfit,
                                    f_F_IndexProfit.AddProfit,
                                    f_F_IndexProfit.TransferProfit,
                                    f_F_IndexProfit.OldTransferProfit,
                                    f_F_IndexProfit.TotalTransfer,
                                    f_F_IndexProfit.DebtProfit,
                                    f_F_IndexProfit.OldDebtProfit,
                                    f_F_IndexProfit.TotalDebt
                                  );

            AddSource(Keys.Okato, d_Regions_Plan.internalKey);

            AddParam(Keys.Period, null, null);
            AddParam(Keys.Day, f_F_IndexProfit.RefYearDayUNV, null);
            AddParam(Keys.Okato, f_F_IndexProfit.RefRegions, d_Regions_Plan.RefBridge);
            AddParam(Keys.Mark, f_F_IndexProfit.RefActivityStatus, null);
            AddParam(Keys.Org, f_F_IndexProfit.RefOrg, null);
        }
    }

    public class QFPropertyTax : QFactTable
    {
        public enum Keys { Period, Day, Okato, Mark, Variant };

        public QFPropertyTax()
            : base(f_F_PropertyTax.TableKey)
        {
            Result = String.Format("Max({0}.{1}) as {1}, Min({0}.{2}) as {2}, Sum({0}.{3}) as {3}, Sum({0}.{4}) as {4}",
                                    InfoFact.Prefix,
                                    f_F_PropertyTax.MaxRateTax,
                                    f_F_PropertyTax.MinRateTax,
                                    f_F_PropertyTax.RateTax,
                                    f_F_PropertyTax.AddTax
                                  );

            AddSource(Keys.Okato, d_Regions_Plan.internalKey);

            AddParam(Keys.Period, null, null);
            AddParam(Keys.Day, f_F_PropertyTax.RefYearDayUNV, null);
            AddParam(Keys.Okato, f_F_PropertyTax.RefRegions, d_Regions_Plan.RefBridge);
            AddParam(Keys.Variant, f_F_PropertyTax.RefVariant, null);
            AddParam(Keys.Mark, f_F_PropertyTax.RefMarks, null);
        }
    }

    public class QFMarksLandTax : QFactTable
    {
        public enum Keys { Period, Day, Okato, Mark };

        public QFMarksLandTax()
            : base(f_Marks_LandTax.TableKey)
        {
            Result = String.Format("Max({0}.{1}) as {1}", InfoFact.Prefix, f_Marks_LandTax.Value);

            AddSource(Keys.Okato, d_Regions_Plan.internalKey);

            AddParam(Keys.Period, null, null);
            AddParam(Keys.Day, f_Marks_LandTax.RefYearDayUNV, null);
            AddParam(Keys.Okato, f_Marks_LandTax.RefRegions, d_Regions_Plan.RefBridge);
            AddParam(Keys.Mark, f_Marks_LandTax.RefMarks, null);
        }
    }
}
