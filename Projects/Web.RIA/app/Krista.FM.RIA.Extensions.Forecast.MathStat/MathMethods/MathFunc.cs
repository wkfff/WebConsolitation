using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Krista.FM.RIA.Extensions.Forecast.MathStat
{
    public static class MathFunc
    {
        /// <summary>
        /// Расчет определителя квадратной матрицы
        /// </summary>
        /// <param name="a">Матрица для которой расчитывается определитель</param>
        /// <returns>Возвращает значение определителя</returns>
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
    }
}
