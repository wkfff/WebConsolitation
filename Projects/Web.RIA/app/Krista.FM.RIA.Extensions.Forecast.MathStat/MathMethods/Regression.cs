using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Krista.FM.RIA.Extensions.Forecast.MathStat
{
    public class Regression
    {
        private readonly MathGroups LoadedMathGroups;

        private int numYear;
        
        private StepModel optimalSM = null;
        private double[] optimalH;

        private int[] bin;

        private double[] x;

        private int order;
        
        public Regression(double[] x, MathGroups mathGroups)
        {
            this.X = x;
            numYear = x.Length;
            
            bin = new int[numYear];

            for (int i = 0; i < bin.Length; i++)
            {
                bin[i] = 1;
            }

            LoadedMathGroups = mathGroups;
        }

        public Regression(double[] x, bool[] weigth, MathGroups mathGroups)
        {
            this.X = x;
            numYear = x.Length;

            bin = new int[numYear];

            for (int i = 0; i < bin.Length; i++)
            {
                bin[i] = weigth[i] ? 1 : 0;
            }

            LoadedMathGroups = mathGroups;
        }

        public delegate double[] StepModel(double[] h, int nY, int nPY);

        public double[] X
        {
            get { return x; }
            set { x = value; }
        }

        public double[] OptimalH
        {
            get { return optimalH; }
        }

        public static double Determine(double[,] a)
        {
            int order = a.GetLength(0);

            double[,] aa = new double[order, order];

            for (int i = 0; i < order; i++)
            {
                for (int j = 0; j < order; j++)
                {
                    aa[i, j] = a[i, j];
                }
            }

            for (int i = 0; i < order; i++)
            {
                for (int j = i + 1; j < order; j++)
                {
                    double mult = aa[j, i] / aa[i, i];
                    for (int k = i; k < order; k++)
                    {
                        aa[j, k] -= aa[i, k] * mult;
                    }
                }
            }

            double deter = 1;
            for (int i = 0; i < order; i++)
            {
                deter *= aa[i, i];
            }

            return deter;
        }

        public Method GetOptimal(int order)
        {
            double[] hexp, hpow, hln, hlogist, hpoly4, hpolyExp4, hpolyPow4, hpolyExp5, hpolyPow5, hpolyLn5, hpolyLogist5;

            hexp = new double[4];
            hpow = new double[4];
            hln = new double[4];
            hlogist = new double[4];
            hpoly4 = new double[4];
            hpolyExp4 = new double[4];
            hpolyPow4 = new double[4];
            hpolyExp5 = new double[5];
            hpolyPow5 = new double[5];
            hpolyLn5 = new double[5];
            hpolyLogist5 = new double[5];

            this.order = order;
            double ax, bx;
            double[,] a = new double[2, 2] { { 0, 0 }, { 0, 0 } };
            double[] b = new double[2] { 0, 0 };
            for (int i = 0; i < numYear; i++)
            {
                a[0, 0] += i * i * bin[i];
                a[0, 1] += i * bin[i];
                a[1, 0] += i * bin[i];
                a[1, 1] += 1 * bin[i];
                b[0] += X[i] * i * bin[i];
                b[1] += X[i] * bin[i];
            }

            ax = ((a[1, 1] * b[0]) - (a[1, 0] * b[1])) / ((a[0, 0] * a[1, 1]) - (a[0, 1] * a[1, 0]));
            bx = ((-a[0, 1] * b[0]) + (a[0, 0] * b[1])) / ((a[0, 0] * a[1, 1]) - (a[0, 1] * a[1, 0]));

            double[] val;
            int minValIdx = 0;

            if (this.order == FixedMathGroups.FirstOrderRegression)
            {
                val = new double[FirstOrderRegression.MaxCode + 1];

                hexp[0] = 100 * ax; 
                hexp[1] = bx - (100 * ax);
                hexp[2] = 0; 
                hexp[3] = 0.01;
                val[FirstOrderRegression.ExpReg] = OptimLm(X, hexp, SmExp, bin);

                hpow[0] = ax;
                hpow[1] = bx;
                hpow[2] = 0;
                hpow[3] = 1;
                val[FirstOrderRegression.PowReg] = OptimLm(X, hpow, SmPow, bin);

                hln[0] = 0;
                hln[1] = bx;
                hln[2] = -0.5;
                hln[3] = ax;
                val[FirstOrderRegression.LogReg] = OptimLm(X, hln, SmLn, bin);

                hlogist[0] = 10000 * ax;
                hlogist[1] = bx - (10100 * ax);
                hlogist[2] = 0.01;
                hlogist[3] = 0.01;
                val[FirstOrderRegression.LogistReg] = OptimLm(X, hlogist, SmLogist, bin);

                hpoly4[0] = 0;
                hpoly4[1] = bx;
                hpoly4[2] = 0;
                hpoly4[3] = ax;
                val[FirstOrderRegression.PolyReg] = OptimLm(X, hpoly4, SmPoly4, bin);

                hpolyExp4[0] = 0;
                hpolyExp4[1] = bx;
                hpolyExp4[2] = 0;
                hpolyExp4[3] = ax / bx;
                val[FirstOrderRegression.PolyExpReg] = OptimLm(X, hpolyExp4, SmPolyExp4, bin);

                hpolyPow4[0] = 0;
                hpolyPow4[1] = bx;
                hpolyPow4[2] = ax;
                hpolyPow4[3] = 1;
                val[FirstOrderRegression.PolyPowReg] = OptimLm(X, hpolyPow4, SmPolyPow4, bin);

                double minVal = val[1];
                minValIdx = 1;

                for (int i = 1; i <= FirstOrderRegression.MaxCode; i++)
                {
                    if (minVal > val[i])
                    {
                        minValIdx = i;
                        minVal = val[i];
                    }
                }
            }
            else
            {
                val = new double[SecondOrderRegression.MaxCode + 1];

                hpolyExp5[0] = 0; 
                hpolyExp5[1] = ax / 0.01; 
                hpolyExp5[2] = 0;
                hpolyExp5[3] = 0.01;
                hpolyExp5[4] = bx - (ax / 0.01);
                val[SecondOrderRegression.PolyExpReg] = OptimLm(X, hpolyExp5, SmPolyExp5, bin);

                hpolyPow5[0] = 0;
                hpolyPow5[1] = bx;
                hpolyPow5[2] = 0;
                hpolyPow5[3] = ax;
                hpolyPow5[4] = 1;
                val[SecondOrderRegression.PolyPowReg] = OptimLm(X, hpolyPow5, SmPolyPow5, bin);

                hpolyLn5[0] = 0;
                hpolyLn5[1] = 1 + (bx / 100);
                hpolyLn5[2] = 0;
                hpolyLn5[3] = ax / 100;
                hpolyLn5[4] = 100;
                val[SecondOrderRegression.PolyLogReg] = OptimLm(X, hpolyLn5, SmPolyLn5, bin);

                hpolyLogist5[0] = 10000 * ax;
                hpolyLogist5[1] = bx - (10100 * ax);
                hpolyLogist5[2] = 0.01;
                hpolyLogist5[3] = 0;
                hpolyLogist5[4] = 0.01;
                val[SecondOrderRegression.PolyLogistReg] = OptimLm(X, hpolyLogist5, SmPolyLogist5, bin);

                double minVal = val[1];
                minValIdx = 1;

                for (int i = 1; i <= SecondOrderRegression.MaxCode; i++)
                {
                    if (minVal > val[i])
                    {
                        minValIdx = i;
                        minVal = val[i];
                    }
                }
            }
            
            if (this.order == FixedMathGroups.FirstOrderRegression)
            {
                var group = LoadedMathGroups.GetGroupByCode(FixedMathGroups.FirstOrderRegression);
                if (group.HasValue)
                {
                    var method = group.Value.Methods.GetMethodByCode(minValIdx);

                    switch (minValIdx)
                    {
                        case FirstOrderRegression.ExpReg:
                            optimalSM = SmExp;
                            optimalH = hexp;
                            break;

                        case FirstOrderRegression.PowReg:
                            optimalSM = SmPow;
                            optimalH = hpow;
                            break;

                        case FirstOrderRegression.LogReg:
                            optimalSM = SmLn;
                            optimalH = hln;
                            break;

                        case FirstOrderRegression.LogistReg:
                            optimalSM = SmLogist;
                            optimalH = hlogist;
                            break;

                        case FirstOrderRegression.PolyReg:
                            optimalSM = SmPoly4;
                            optimalH = hpoly4;
                            break;
                
                        case FirstOrderRegression.PolyExpReg:
                            optimalSM = SmPolyExp4;
                            optimalH = hpolyExp4;
                            break;
                
                        case FirstOrderRegression.PolyPowReg:
                            optimalSM = SmPolyPow4;
                            optimalH = hpolyPow4;
                            break;
                    }

                    if (method.HasValue)
                    {
                        return method.Value;
                    }
                }

                throw new Exception("method not found");
            }
            else
            {
                var group = LoadedMathGroups.GetGroupByCode(FixedMathGroups.SecondOrderRegression);
                if (group.HasValue)
                {
                    var method = group.Value.Methods.GetMethodByCode(minValIdx);

                    switch (minValIdx)
                    {
                        case SecondOrderRegression.PolyExpReg:
                            optimalSM = SmPolyExp5;
                            optimalH = hpolyExp5;
                            break;

                        case SecondOrderRegression.PolyPowReg:
                            optimalSM = SmPolyPow5;
                            optimalH = hpolyPow5;
                            break;

                        case SecondOrderRegression.PolyLogReg:
                            optimalSM = SmPolyLn5;
                            optimalH = hpolyLn5;
                            break;

                        case SecondOrderRegression.PolyLogistReg:
                            optimalSM = SmPolyLogist5;
                            optimalH = hpolyLogist5;
                            break;
                    }

                    if (method.HasValue)
                    {
                        return method.Value;
                    }
                }

                throw new Exception("method not found");
            }
        }

        public Method? GetConcrete(int order, int met)
        {
            this.order = order;

            double ax, bx;
            double[,] a = new double[2, 2] { { 0, 0 }, { 0, 0 } };
            double[] b = new double[2] { 0, 0 };
            for (int i = 0; i < numYear; i++)
            {
                a[0, 0] += i * i * bin[i];
                a[0, 1] += i * bin[i];
                a[1, 0] += i * bin[i];
                a[1, 1] += 1 * bin[i];
                b[0] += X[i] * i * bin[i];
                b[1] += X[i] * bin[i];
            }

            ax = ((a[1, 1] * b[0]) - (a[1, 0] * b[1])) / ((a[0, 0] * a[1, 1]) - (a[0, 1] * a[1, 0]));
            bx = ((-a[0, 1] * b[0]) + (a[0, 0] * b[1])) / ((a[0, 0] * a[1, 1]) - (a[0, 1] * a[1, 0]));

            if (this.order == FixedMathGroups.FirstOrderRegression)
            {
                optimalH = new double[4];

                var group = LoadedMathGroups.GetGroupByCode(FixedMathGroups.FirstOrderRegression);
                if (group.HasValue)
                {
                    var method = group.Value.Methods.GetMethodByCode(met);

                    switch (met)
                    {
                        case FirstOrderRegression.ExpReg:
                            optimalH[0] = 100 * ax;
                            optimalH[1] = bx - (100 * ax);
                            optimalH[2] = 0;
                            optimalH[3] = 0.01;
                            OptimLm(X, optimalH, SmExp, bin);
                            optimalSM = SmExp;
                            break;

                        case FirstOrderRegression.PowReg:
                            optimalH[0] = ax;
                            optimalH[1] = bx;
                            optimalH[2] = 0;
                            optimalH[3] = 1;
                            OptimLm(X, optimalH, SmPow, bin);
                            optimalSM = SmPow;
                            break;

                        case FirstOrderRegression.LogReg:
                            optimalH[0] = 0;
                            optimalH[1] = bx;
                            optimalH[2] = -0.5;
                            optimalH[3] = ax;
                            OptimLm(X, optimalH, SmLn, bin);
                            optimalSM = SmLn;
                            break;

                        case FirstOrderRegression.LogistReg:
                            optimalH[0] = 10000 * ax;
                            optimalH[1] = bx - (10100 * ax);
                            optimalH[2] = 0.01;
                            optimalH[3] = 0.01;
                            OptimLm(X, optimalH, SmLogist, bin);
                            optimalSM = SmLogist;
                            break;

                        case FirstOrderRegression.PolyReg:
                            optimalH[0] = 0;
                            optimalH[1] = bx;
                            optimalH[2] = 0;
                            optimalH[3] = ax;
                            OptimLm(X, optimalH, SmPoly4, bin);
                            optimalSM = SmPoly4;
                            break;

                        case FirstOrderRegression.PolyExpReg:
                            optimalH[0] = 0;
                            optimalH[1] = bx;
                            optimalH[2] = 0;
                            optimalH[3] = ax / bx;
                            OptimLm(X, optimalH, SmPolyExp4, bin);
                            optimalSM = SmPolyExp4;
                            break;

                        case FirstOrderRegression.PolyPowReg:
                            optimalH[0] = 0;
                            optimalH[1] = bx;
                            optimalH[2] = ax;
                            optimalH[3] = 1;
                            OptimLm(X, optimalH, SmPolyPow4, bin);
                            optimalSM = SmPolyPow4;
                            break;
                    }

                    if (method.HasValue)
                    {
                        return method.Value;
                    }

                    throw new Exception("method not found");
                }
            }
            else
            {
                    optimalH = new double[5];

                    var group = LoadedMathGroups.GetGroupByCode(FixedMathGroups.SecondOrderRegression);
                    if (group.HasValue)
                    {
                        var method = group.Value.Methods.GetMethodByCode(met);

                        switch (met)
                        {
                            case SecondOrderRegression.PolyExpReg:
                                optimalH[0] = 0;
                                optimalH[1] = ax / 0.01;
                                optimalH[2] = 0;
                                optimalH[3] = 0.01;
                                optimalH[4] = bx - (ax / 0.01);
                                OptimLm(X, optimalH, SmPolyExp5, bin);
                                optimalSM = SmPolyExp5;
                                break;

                            case SecondOrderRegression.PolyPowReg:
                                optimalH[0] = 0;
                                optimalH[1] = bx;
                                optimalH[2] = 0;
                                optimalH[3] = ax;
                                optimalH[4] = 1;
                                OptimLm(X, optimalH, SmPolyPow5, bin);
                                optimalSM = SmPolyPow5;
                                break;

                            case SecondOrderRegression.PolyLogReg:
                                optimalH[0] = 0;
                                optimalH[1] = 1 + (bx / 100);
                                optimalH[2] = 0;
                                optimalH[3] = ax / 100;
                                optimalH[4] = 100;
                                OptimLm(X, optimalH, SmPolyLn5, bin);
                                optimalSM = SmPolyLn5;
                                break;

                            case SecondOrderRegression.PolyLogistReg:
                                optimalH[0] = 10000 * ax;
                                optimalH[1] = bx - (10100 * ax);
                                optimalH[2] = 0.01;
                                optimalH[3] = 0;
                                optimalH[4] = 0.01;
                                OptimLm(X, optimalH, SmPolyLogist5, bin);
                                optimalSM = SmPolyLogist5;
                                break;
                        }

                        if (method.HasValue)
                        {
                            return method.Value;
                        }
                    }

                    throw new Exception("method not found");
                }

            return null;
        }

        public double[] Predict(int numPredYears, out double[] predValues)
        {
            double[] xPred = new double[numYear + numPredYears];
            double[] remainders = new double[numYear];

            xPred = optimalSM(OptimalH, numYear, numPredYears);
            predValues = xPred;

            for (int i = 0; i < numYear; i++)
            {
                remainders[i] = x[i] - xPred[i];
            }

            return remainders;
        }

        private static double[] SmExp(double[] h, int nY, int nPY)
        {
            double[] y = new double[nY + nPY];
            for (int i = 0; i < nY + nPY; i++)
            {
                y[i] = (h[0] * Math.Exp(i * h[3])) + (h[2] * i) + h[1];
            }

            return y;
        }

        private static double[] SmPow(double[] h, int nY, int nPY)
        {
            double[] y = new double[nY + nPY];
            for (int i = 0; i < nY + nPY; i++)
            {
                y[i] = (h[0] * Math.Pow(Math.Abs(i - h[2]), h[3])) + h[1];
            }

            return y;
        }

        private static double[] SmLn(double[] h, int nY, int nPY)
        {
            int xl = h.Length;
            double[] y = new double[nY + nPY];
            for (int i = 0; i < nY + nPY; i++)
            {
                y[i] = (h[0] * Math.Log(Math.Abs(i - h[2]))) + (h[3] * i) + h[1];
            }

            return y;
        }

        private static double[] SmLogist(double[] h, int nY, int nPY)
        {
            double[] y = new double[nY + nPY];
            for (int i = 0; i < nY + nPY; i++)
            {
                y[i] = (h[0] / (1 - (h[2] * Math.Exp(i * h[3])))) + h[1];
            }

            return y;
        }

        private static double[] SmPoly4(double[] h, int nY, int nPY)
        {
            double[] y = new double[nY + nPY];
            for (int i = 0; i < nY + nPY; i++)
            {
                y[i] = (h[0] * i * i * i) + (h[2] * i * i) + (h[3] * i) + h[1];
            }

            return y;
        }

        private static double[] SmPolyExp4(double[] h, int nY, int nPY)
        {
            double[] y = new double[nY + nPY];
            for (int i = 0; i < nY + nPY; i++)
            {
                y[i] = h[1] * Math.Exp((h[0] * i * i * i) + (h[2] * i * i) + (h[3] * i));
            }

            return y;
        }

        private static double[] SmPolyPow4(double[] h, int nY, int nPY)
        {
            double[] y = new double[nY + nPY];
            for (int i = 0; i < nY + nPY; i++)
            {
                y[i] = Math.Pow((h[0] * i * i) + (h[2] * i) + h[1], h[3]);
            }

            return y;
        }

        private static double[] SmPolyExp5(double[] h, int nY, int nPY)
        {
            double[] y = new double[nY + nPY];
            for (int i = 0; i < nY + nPY; i++)
            {
                y[i] = (h[1] * Math.Exp((h[0] * i * i * i) + (h[2] * i * i) + (h[3] * i))) + h[4];
            }

            return y;
        }

        private static double[] SmPolyPow5(double[] h, int nY, int nPY)
        {
            double[] y = new double[nY + nPY];
            for (int i = 0; i < nY + nPY; i++)
            {
                y[i] = Math.Pow((h[0] * i * i * i) + (h[2] * i * i) + (h[3] * i) + h[1], h[4]);
            }

            return y;
        }

        private static double[] SmPolyLn5(double[] h, int nY, int nPY)
        {
            double[] y = new double[nY + nPY];
            for (int i = 0; i < nY + nPY; i++)
            {
                y[i] = h[4] * Math.Log((h[0] * i * i * i) + (h[2] * i * i) + (h[3] * i) + h[1]);
            }

            return y;
        }

        private static double[] SmPolyLogist5(double[] h, int nY, int nPY)
        {
            double[] y = new double[nY + nPY];
            for (int i = 0; i < nY + nPY; i++)
            {
                y[i] = h[1] + (h[0] / (1 - (h[2] * Math.Exp((h[3] * i * i) + (h[4] * i)))));
            }

            return y;
        }

        private static double OptimLm(double[] x, double[] h, StepModel stm, int[] bin)
        {
            const double SIG = 0.1;
            int numYear = x.Length;
            int n_h = h.Length;

            const double DiffMinChange = 1e-8;
            const double DiffMaxChange = 0.1;

            double[] step = new double[n_h];
            double[] dh = new double[n_h];
            double[] p = new double[n_h];
            double[] x_st;
            double lambda = 10 ^ 4;
            double[] g = new double[numYear];
            double[] gx = new double[numYear];
            for (int i = 0; i < numYear; i++)
            {
                g[i] = Math.Exp(SIG * i) * bin[i];
                gx[i] = g[i] * x[i];
            }

            for (int count = 1; count < 100; count++)
            {
                for (int j = 0; j < n_h; j++)
                {
                    step[j] = 1e-8;
                }

                double[] h1 = new double[n_h];
                ////y = step_model(h, n_x,No);
                double[] y = stm(h, numYear, 0);
                for (int i = 0; i < numYear; i++)
                {
                    y[i] *= g[i];
                }
                
                double nev0 = 0;
                for (int k = 0; k < numYear; k++)
                {
                    nev0 += (y[k] - gx[k]) * (y[k] - gx[k]);
                }

                double[,] jac = new double[numYear, n_h];
                double[,] jacJ = new double[n_h, n_h];
                double[,] jacJprom = new double[n_h, n_h];
                double[] jacF = new double[n_h];

                for (int k = 0; k < n_h; k++)
                {
                    step[k] *= Math.Max(Math.Abs(h[k]), 1);
                    step[k] = Math.Sign(step[k]) * Math.Min(Math.Max(Math.Abs(step[k]), DiffMinChange), DiffMaxChange);

                    for (int i = 0; i < n_h; i++)
                    {
                        h1[i] = h[i];
                    }

                    h1[k] += step[k];
                    ////x_st = step_model(h1, n_x, No);
                    x_st = stm(h1, numYear, 0);
                    for (int i = 0; i < numYear; i++)
                    {
                        x_st[i] *= g[i];
                    }

                    for (int j = 0; j < numYear; j++)
                    {
                        jac[j, k] = (x_st[j] - y[j]) / step[k];
                    }
                }

                for (int i = 0; i < n_h; i++)
                {
                    for (int j = 0; j < n_h; j++)
                    {
                        jacJ[i, j] = 0;
                        for (int k = 0; k < numYear; k++)
                        {
                            jacJ[i, j] += jac[k, i] * jac[k, j];
                        }
                    }
                }

                for (int j = 0; j < n_h; j++)
                {
                    jacF[j] = 0;
                    for (int k = 0; k < numYear; k++)
                    {
                        jacF[j] += jac[k, j] * (y[k] - gx[k]);
                    }
                }

                for (int i = 0; i < n_h; i++)
                {
                    jacJ[i, i] += jacJ[i, i] * lambda;
                }

                for (int i = 0; i < n_h; i++)
                {
                    for (int j = 0; j < n_h; j++)
                    {
                        for (int k = 0; k < n_h; k++)
                        {
                            if (k == i)
                            {
                                jacJprom[j, k] = jacF[j];
                            }
                            else
                            {
                                jacJprom[j, k] = jacJ[j, k];
                            }
                        }
                    }

                    dh[i] = Determine(jacJprom) / Determine(jacJ);
                }

                for (int j = 0; j < n_h; j++)
                {
                    p[j] = h[j] - dh[j];
                }

                x_st = stm(p, numYear, 0);
                for (int i = 0; i < numYear; i++)
                {
                    x_st[i] *= g[i];
                }
                
                double nev = 0;
                for (int k = 0; k < numYear; k++)
                {
                    nev += (x_st[k] - gx[k]) * (x_st[k] - gx[k]);
                }

                if (nev < nev0)
                {
                    for (int j = 0; j < n_h; j++)
                    {
                        h[j] -= dh[j];
                    }

                    lambda /= 10;
                }
                else
                {
                    lambda *= 10;
                }
            } // end for count

            x_st = stm(h, numYear, 0);
            for (int i = 0; i < numYear; i++)
            {
                x_st[i] *= g[i];
            }

            double optimReturn = 0;
            for (int i1 = 0; i1 < numYear; i1++)
            {
                optimReturn += (x_st[i1] - gx[i1]) * (x_st[i1] - gx[i1]);
            }

            return optimReturn;
        }

        /*private static double OptimLm(double[] x, double[] h, StepModel stm)
        {
            int n_x = x.Length;
            int n_h = h.Length;

            const double DiffMinChange = 1e-8;
            const double DiffMaxChange = 0.1;

            double[] step = new double[n_h];
            double[] dh = new double[n_h];
            double[] p = new double[n_h];
            double[] x_st;

            double lambda = 10 ^ 4;
            for (int count = 1; count < 100; count++)
            {
                for (int j = 0; j < n_h; j++)
                {
                    step[j] = 1e-8;
                }

                double[] h1 = new double[n_h];
                ////y = step_model(h, n_x,No);
                double[] y = stm(h, n_x, 0);

                double nev0 = 0;
                for (int k = 0; k < n_x; k++)
                {
                    nev0 += (y[k] - x[k]) * (y[k] - x[k]);
                }

                double[,] jac = new double[n_x, n_h];
                double[,] jacJ = new double[n_h, n_h];
                double[,] jacJprom = new double[n_h, n_h];
                double[] jacF = new double[n_h];

                for (int k = 0; k < n_h; k++)
                {
                    step[k] *= Math.Max(Math.Abs(h[k]), 1);
                    step[k] = Math.Sign(step[k]) * Math.Min(Math.Max(Math.Abs(step[k]), DiffMinChange), DiffMaxChange);

                    for (int i = 0; i < n_h; i++)
                    {
                        h1[i] = h[i];
                    }

                    h1[k] += step[k];
                    ////x_st = step_model(h1, n_x, No);
                    x_st = stm(h1, n_x, 0);
                    
                    for (int j = 0; j < n_x; j++)
                    {
                        jac[j, k] = (x_st[j] - y[j]) / step[k];
                    }
                }

                for (int i = 0; i < n_h; i++)
                {
                    for (int j = 0; j < n_h; j++)
                    {
                        jacJ[i, j] = 0;
                        for (int k = 0; k < n_x; k++)
                        {
                            jacJ[i, j] += jac[k, i] * jac[k, j];
                        }
                    }
                }

                for (int j = 0; j < n_h; j++)
                {
                    jacF[j] = 0;
                    for (int k = 0; k < n_x; k++)
                    {
                        jacF[j] += jac[k, j] * (y[k] - x[k]);
                    }
                }

                for (int i = 0; i < n_h; i++)
                {
                    jacJ[i, i] += jacJ[i, i] * lambda;
                }

                for (int i = 0; i < n_h; i++)
                {
                    for (int j = 0; j < n_h; j++)
                    {
                        for (int k = 0; k < n_h; k++)
                        {
                            if (k == i)
                            {
                                jacJprom[j, k] = jacF[j];
                            }
                            else
                            {
                                jacJprom[j, k] = jacJ[j, k];
                            }
                        }
                    }

                    dh[i] = Determine(jacJprom) / Determine(jacJ);
                }

                for (int j = 0; j < n_h; j++)
                {
                    p[j] = h[j] - dh[j];
                }

                x_st = stm(p, n_x, 0);
                double nev = 0;
                for (int k = 0; k < n_x; k++)
                {
                    nev += (x_st[k] - x[k]) * (x_st[k] - x[k]);
                }

                if (nev < nev0)
                {
                    for (int j = 0; j < n_h; j++)
                    {
                        h[j] -= dh[j];
                    }

                    lambda /= 10;
                }
                else
                {
                    lambda *= 10;
                }
            } // end for count

            x_st = stm(h, n_x, 0);
            double optimReturn = 0;
            for (int i1 = 0; i1 < n_x; i1++)
            {
                optimReturn += (x_st[i1] - x[i1]) * (x_st[i1] - x[i1]);
            }

            return optimReturn;
        }*/
    }
}
