using System;
using System.Collections.Generic;
using System.Text;

namespace Krista.FM.RIA.Extensions.Forecast.MathStat
{
    public class ARMA
    {
        private double[] x;
        private double[] matHal; //// = new double[2] { -1, 1 };
        private double[] matHbt; //// = new double[2] { -1, 0 };

        public ARMA(double[] x, int a, int b)
        {
            this.X = x;
            matHal = new double[a];
            matHbt = new double[b];

            Random rnd = new Random();
            for (int i = 0; i < matHal.Length; i++)
            {
                matHal[i] = ((double)rnd.Next(2000) - 1000) / 1000;
            }

            for (int i = 0; i < matHbt.Length; i++)
            {
                matHbt[i] = ((double)rnd.Next(2000) - 1000) / 1000;
            }

            SolveARMA();
        }

        public double[] X
        {
            get { return x; }
            set { x = value; }
        }

        /// <summary>
        /// Calculates the coefficients of the ARMA method Margraft method
        /// </summary>
        /// <param name="x">input array x</param>
        /// <param name="alfa">array of AR coefficients</param>
        /// <param name="beta">array of MA coefficients</param>
        /// <returns>value of error</returns>
        public static double OptimLmArma(double[] x, ref double[] alfa, ref double[] beta)
        {
            int n_x = x.Length;
            int n_a = alfa.Length;
            int n_b = beta.Length;

            const double DiffMinChange = 1e-8;
            const double DiffMaxChange = 0.1;

            double[] step = new double[n_a + n_b];
            double[] dh = new double[n_a + n_b];
            double[] x_st;
            double[,] jac = new double[n_x, n_a + n_b];
            double[,] jacJ = new double[n_a + n_b, n_a + n_b];
            double[,] jacJprom = new double[n_a + n_b, n_a + n_b];
            double[] jacF = new double[n_a + n_b];
            double lambda = 10 ^ 4;

            for (int count = 1; count < 100; count++)
            {
                for (int j = 0; j < n_a + n_b; j++)
                {
                    step[j] = 1e-8;
                }

                double[] alfa1 = new double[n_a];
                double[] beta1 = new double[n_b];

                double[] y = ErrArma(x, alfa, beta);

                double nev0 = 0;
                for (int k = 0; k < n_x; k++)
                {
                    nev0 += y[k] * y[k];
                }

                for (int k = 0; k < n_a; k++)
                {
                    step[k] = step[k] * Math.Max(Math.Abs(alfa[k]), 1);
                    step[k] = Math.Sign(step[k]) * Math.Min(Math.Max(Math.Abs(step[k]), DiffMinChange), DiffMaxChange);

                    for (int i = 0; i < n_a; i++)
                    {
                        alfa1[i] = alfa[i];
                    }

                    for (int i = 0; i < n_b; i++)
                    {
                        beta1[i] = beta[i];
                    }

                    alfa1[k] += step[k];
                    ////x_st = step_model(h1, n_x, No);
                    x_st = ErrArma(x, alfa1, beta1);

                    for (int j = 0; j < n_x; j++)
                    {
                        jac[j, k] = (x_st[j] - y[j]) / step[k];
                    }
                }

                for (int k = 0; k < n_b; k++)
                {
                    step[k + n_a] = step[k + n_a] * Math.Max(Math.Abs(beta[k]), 1);
                    step[k + n_a] = Math.Sign(step[k + n_a]) * Math.Min(Math.Max(Math.Abs(step[k + n_a]), DiffMinChange), DiffMaxChange);

                    for (int i = 0; i < n_a; i++)
                    {
                        alfa1[i] = alfa[i];
                    }

                    for (int i = 0; i < n_b; i++)
                    {
                        beta1[i] = beta[i];
                    }

                    beta1[k] += step[k + n_a];
                    ////x_st = step_model(h1, n_x, No);
                    x_st = ErrArma(x, alfa1, beta1);

                    for (int j = 0; j < n_x; j++)
                    {
                        jac[j, k + n_a] = (x_st[j] - y[j]) / step[k + n_a];
                    }
                }

                for (int i = 0; i < n_a + n_b; i++)
                {
                    for (int j = 0; j < n_a + n_b; j++)
                    {
                        jacJ[i, j] = 0;
                        for (int k = 0; k < n_x; k++)
                        {
                            jacJ[i, j] += jac[k, i] * jac[k, j];
                        }
                    }
                }

                for (int j = 0; j < n_a + n_b; j++)
                {
                    jacF[j] = 0;
                    for (int k = 0; k < n_x; k++)
                    {
                        jacF[j] += jac[k, j] * y[k];
                    }
                }

                for (int i = 0; i < n_a + n_b; i++)
                {
                    jacJ[i, i] += jacJ[i, i] * lambda;
                }

                for (int i = 0; i < n_a + n_b; i++)
                {
                    for (int j = 0; j < n_a + n_b; j++)
                    {
                        for (int k = 0; k < n_a + n_b; k++)
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

                    dh[i] = MathFunc.Determine(jacJprom) / MathFunc.Determine(jacJ);
                }

                for (int j = 0; j < n_a; j++)
                {
                    alfa1[j] -= dh[j];
                }

                for (int j = 0; j < n_b; j++)
                {
                    beta1[j] -= dh[j + n_a];
                }

                x_st = ErrArma(x, alfa1, beta1);

                double nev = 0;
                for (int k = 0; k < n_x; k++)
                {
                    nev += x_st[k] * x_st[k];
                }

                if (nev < nev0)
                {
                    for (int j = 0; j < n_a; j++)
                    {
                        alfa[j] -= dh[j];
                    }

                    for (int j = 0; j < n_b; j++)
                    {
                        beta[j] -= dh[j + n_a];
                    }

                    lambda /= 10;
                }
                else
                {
                    lambda *= 10;
                }
            } // end for count

            ////x_st = step_model(h, n_x, No);
            x_st = ErrArma(x, alfa, beta);

            double optimReturn = 0;
            for (int i1 = 0; i1 < n_x; i1++)
            {
                optimReturn += x_st[i1] * x_st[i1];
            }

            return optimReturn;
        }

        /// <summary>
        /// Calculate error of ARMA method with alfa,beta coefficients
        /// </summary>
        /// <param name="x">input array x</param>
        /// <param name="alfa">array of AR coefficients</param>
        /// <param name="beta">array of MA coefficients</param>
        /// <returns>value of error</returns>
        public static double[] ErrArma(double[] x, double[] alfa, double[] beta)
        {
            int n_x = x.Length;
            int n_a = alfa.Length;
            int n_b = beta.Length;

            double[] y = new double[n_x];
            double[] err = new double[n_x];
            for (int i = 0; i < n_x; i++)
            {
                err[i] = 0;
            }

            for (int i = 0; i < n_x; i++)
            {
                y[i] = 0;
                if ((i - n_a) >= 0)
                {
                    for (int j = 1; j <= n_a; j++)
                    {
                        y[i] += y[i - j] * alfa[j - 1];
                    }
                }
                else
                {
                    y[i] = x[i];
                }

                for (int j = 1; j <= n_b; j++)
                {
                    if (((i - j) >= 0) && ((i - j) < n_x))
                    {
                        y[i] += err[i - j] * beta[j - 1];
                    }
                }

                if (i < n_x)
                {
                    err[i] = y[i] - x[i];
                }
            }

            return err;
        }

/*        public static double Determine(double[,] a)
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
        }*/

        /// <summary>
        /// Predict ARMA data  
        /// </summary>
        /// <param name="nyear">Number of predicted years into the future</param>
        /// <returns>Prdicted values</returns>
        public double[] PredictARMA(int nyear)
        {
            int n_x = X.Length;
            int n_a = matHal.Length;
            int n_b = matHbt.Length;

            double[] y = new double[n_x + nyear];
            double[] err = new double[n_x];
            Array.Clear(err, 0, err.Length);

            for (int i = 0; i < n_x + nyear; i++)
            {
                y[i] = 0;
                if ((i - n_a) >= 0)
                {
                    for (int j = 1; j <= n_a; j++)
                    {
                        y[i] += y[i - j] * matHal[j - 1];
                    }
                }
                else
                {
                    y[i] = X[i];
                }

                for (int j = 1; j <= n_b; j++)
                {
                    if (((i - j) >= 0) && ((i - j) < n_x))
                    {
                        y[i] += err[i - j] * matHbt[j - 1];
                    }
                }

                if (i < n_x)
                {
                    err[i] = y[i] - X[i];
                }
            }

            return y;
        }

        /// <summary>
        /// Get optimum ARMA coefficient
        /// </summary>
        /// <returns>value of error</returns>
        private double SolveARMA()
        {
            double d = OptimLmArma(X, ref matHal, ref matHbt);
           
            double[] tmp = new double[matHbt.Length];

            double sum = 0;
            for (int i = 0; i < matHbt.Length; i++)
            {
                tmp[i] = Math.Abs(matHbt[i]);
                sum += tmp[i];
            }

            Array.Sort(tmp);

            double min = tmp[0];
            double max = tmp[matHbt.Length - 1];

            sum /= matHbt.Length;

            if ((min / max < 0.0001) || (sum > 10))
            {
                for (int i = 0; i < matHbt.Length; i++)
                {
                    matHbt[i] = 0;
                }
            }

            return d;
        }
    }
}
