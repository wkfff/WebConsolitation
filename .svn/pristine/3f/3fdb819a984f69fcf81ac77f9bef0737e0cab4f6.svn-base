using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.CapitalPlanningOperations
{
    internal class FinancialCalculation
    {
        internal decimal Calculate(FinancialCalculationParams calculationParams)
        {
            // в зависимости от вычисляемого параметра выбираем ту или иную формулу для расчета
            switch (calculationParams.CalculatedValue)
            {
                case CalculatedValue.YTM:
                    return CalculateYTM(calculationParams);
                case CalculatedValue.CurrPriceRur:
                    return Math.Round(CalculateCurrentRurCost(calculationParams), 4, MidpointRounding.AwayFromZero);
                case CalculatedValue.CurrPricePercent:
                    return CalculateCurrentCostPercent(calculationParams);
                case CalculatedValue.CouponR:
                    return CalculateCouponRate(calculationParams);
            }
            return 0;
        }

        private decimal CalculateCurrentRurCost(FinancialCalculationParams calculationParams)
        {
            decimal rurCost = 0;
            int t = 0;
            foreach (NominalCost nominalCost in calculationParams.PaymentsNominalCost)
            {
                t = nominalCost.DayCount;
                double x1 = Convert.ToDouble(1 + (calculationParams.YTM / 100));
                decimal dc = Decimal.Divide(t , calculationParams.Basis);
                double x2 = Convert.ToDouble(dc);
                double denominator = Math.Exp(x2 * Math.Log(x1));
                rurCost += nominalCost.NominalSum / Convert.ToDecimal(denominator);
            }
            t = 0;

            foreach (Coupon coupon in calculationParams.Coupons)
            {
                t += coupon.DayCount;
                double x1 = Convert.ToDouble(1 + (calculationParams.YTM / 100));
                decimal dc = Decimal.Divide(t, calculationParams.Basis);
                double x2 = Convert.ToDouble(dc);
                double denominator = Math.Exp(x2 * Math.Log(x1));
                decimal couponRate = calculationParams.CalculatedValue == CalculatedValue.CouponR
                                            ? calculationParams.CouponR
                                            : coupon.CouponRate;
                rurCost += (couponRate * coupon.Nominal * coupon.DayCount / calculationParams.Basis / 100) /
                    Convert.ToDecimal(denominator);
            }

            return rurCost;
        }

        private decimal CalculateYTM(FinancialCalculationParams calculationParams)
        {
            decimal interval = 100;
            decimal Y = 0;
            calculationParams.YTM = Y;
            decimal P = calculationParams.Nominal * calculationParams.CurrentPricePercent / 100;
            decimal pY = CalculateCurrentRurCost(calculationParams);
            decimal dif = P - pY;
            decimal sigma = new decimal(0.0001);
            while (Math.Abs(dif) > sigma)
            {
                if (dif > 0)
                {
                    interval = interval / 2; 
                    Y = Y - interval;
                }
                else
                {
                    Y = Y + interval;
                }
                calculationParams.YTM = Y;
                pY = CalculateCurrentRurCost(calculationParams);
                dif = P - pY;
            }
            return Math.Round(Y, 4, MidpointRounding.AwayFromZero);
        }

        private decimal CalculateCurrentCostPercent(FinancialCalculationParams calculationParams)
        {
            return calculationParams.CurrentPriceRur/calculationParams.Nominal;
        }

        private decimal CalculateCouponRate(FinancialCalculationParams calculationParams)
        {
            decimal interval = 100;
            decimal rate = 0;
            calculationParams.CouponR = rate;
            decimal P = calculationParams.Nominal * calculationParams.CurrentPricePercent / 100;
            decimal pRate = CalculateCurrentRurCost(calculationParams);
            decimal dif = P - pRate;
            decimal sigma = new decimal(0.01);
            while (Math.Abs(dif) > sigma)
            {
                if (dif < 0)
                {
                    interval = interval / 2;
                    rate = rate - interval;
                }
                else
                {
                    rate = rate + interval;
                }
                calculationParams.CouponR = rate;
                pRate = CalculateCurrentRurCost(calculationParams);
                dif = P - pRate;
            }
            return Math.Round(rate, 2, MidpointRounding.AwayFromZero);
        }
    }
}
