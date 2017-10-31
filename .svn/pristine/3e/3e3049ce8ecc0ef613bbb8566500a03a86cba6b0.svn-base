using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Krista.FM.RIA.Extensions.Forecast.MathStat
{
    public class ReverseRegression
    {
        private double[] y; // объясненная переменная
        private double[] previousYear; // значения  объяняющих переменных за предыдущий год
        private double[] regrCoeff; // коэффициенты регрессионного уравнения
        ////private double[,] x; // значения объясняющих переменных
        
        public ReverseRegression(double[] previous_year, double[] y, double[] regr_coeff)
        {
            this.previousYear = previous_year; 
            this.y = y;
            this.regrCoeff = regr_coeff;
            ////this.x = null;
        }

        public static double[] Solve_SLAE(double[,] temp_XX, double[] temp_Xy, int order)
        {
            double base_determine;
            double[] y = new double[order];
            double[,] temp_XX_Xy = new double[order, order];

            // Определяем значение определителя базовой матрицы X*X'
            base_determine = MathFunc.Determine(temp_XX);
            for (int i = 0; i < order; i++)
            {
                for (int k = 0; k < order; k++)
                {
                    for (int j = 0; j < order; j++)
                    {
                        if (k == i)
                        {
                            temp_XX_Xy[j, k] = temp_Xy[j];
                        }
                        else
                        {
                            temp_XX_Xy[j, k] = temp_XX[j, k];
                        }
                    }
                }

                y[i] = MathFunc.Determine(temp_XX_Xy) / base_determine;
            }

            return y;
        }

        public double[,] Compute()
        {
            int cntr; // счетчик
            int cntYear = y.GetLength(0); // число лет для которых выполняется расчет
            int numParam = previousYear.GetLength(0); // число объясняющих переменных
            double[] y_1 = new double[cntYear];
            double[] f = new double[numParam * cntYear];
            double[,] x = new double[numParam, cntYear];
            ////double[,] J = new double[N * M, N * M];
            ////double[,] I = new double[N * M, N * M];
            double[,] h = new double[numParam * cntYear, numParam * cntYear];
            double[,] aeq = new double[cntYear, numParam * cntYear];
            
            for (int i = 0; i < cntYear; i++)
            {
                y_1[i] = y[i] - regrCoeff[0]; // вычитаем a(0)
            }

            // Определяем элементы задачи 0.5*x'*H*x+f*x->min
            for (int i = 0; i < numParam; i++)
            {
                for (int j = 0; j < cntYear; j++)
                {
                    f[(i * cntYear) + j] = previousYear[i]; 
                }
            }

            for (int i = 0; i < numParam * cntYear; i++)
            {
                    h[i, i] = 1;
            }

            cntr = 0;

            // Определение элементов уравнений Aeq*x=y_1
            for (int i = 0; i < numParam; i++)
            {
                for (int j = 0; j < cntYear; j++)
                {
                    aeq[cntr % cntYear, cntr++] = regrCoeff[i + 1];                  
                }
            }

            // Задача может быть решена с помощью решения уравнения H_full* x = f_lambda;
            double[,] hfull = new double[(numParam * cntYear) + cntYear, (numParam * cntYear) + cntYear];
            for (int i = 0; i < numParam * cntYear; i++)
            {
                for (int j = 0; j < numParam * cntYear; j++)
                {
                    hfull[i, j] = h[i, j];
                }
            }

            for (int i = 0; i < numParam * cntYear; i++)
            {
                for (int j = numParam * cntYear; j < (numParam + 1) * cntYear; j++)
                {
                    hfull[i, j] = aeq[j - (numParam * cntYear), i];
                }
            }

            for (int i = numParam * cntYear; i < (numParam + 1) * cntYear; i++)
            {
                for (int j = 0; j < numParam * cntYear; j++)
                {
                    hfull[i, j] = aeq[i - (numParam * cntYear), j];
                }
            }

            double[] flambda = new double[(numParam + 1) * cntYear];
            double[] xlambda = new double[(numParam + 1) * cntYear];
            for (int i = 0; i < numParam * cntYear; i++)
            {
                flambda[i] = f[i];
            }

            for (int i = numParam * cntYear; i < (numParam + 1) * cntYear; i++)
            {
                flambda[i] = y_1[i - (numParam * cntYear)];
            }
            
            xlambda = Solve_SLAE(hfull, flambda, (numParam + 1) * cntYear);
            cntr = 0;
            
            // Пересохранение данных в x[,]
            for (int i = 0; i < numParam; i++)
            {
                for (int j = 0; j < cntYear; j++)
                {
                    x[i, j] = xlambda[cntr++];
                }
            }

            ////this.x = x;
            return x;
        }
    }
}