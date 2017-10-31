using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Krista.FM.RIA.Extensions.Forecast.MathStat
{
    public struct Criteria
    {
        public string Name;
        public double Value;
        public string Text;
    }

    public class Criterias
    {
        /// <summary>
        /// Средний квадрат (Число Степеней Свободы = N-2). 
        /// Вычисляется по количеству элеменов в массиве x
        /// </summary>
        /// <param name="x">массив статистически значений</param>
        /// <param name="y">массив спрогнозированных значений</param>
        /// <returns>средний квадрат</returns>
        public static double MSE(double[] x, double[] y)
        {
            ////int n = x.Length;
            int n = x.Length < y.Length ? x.Length : y.Length;
            double sum_sq = 0;
            for (int i = 0; i < n; i++)
            {
                sum_sq += (x[i] - y[i]) * (x[i] - y[i]);
            }

            sum_sq /= n;
            return sum_sq;
        }

        /// <summary>
        /// Средний модуль(Число Степеней Свободы = N-2)
        /// Вычисляется по количеству элеменов в массиве x
        /// </summary>
        /// <param name="x">массив статистически значений</param>
        /// <param name="y">массив спрогнозированных значений</param>
        /// <returns>средний модуль</returns>
        public static double MAE(double[] x, double[] y)
        {
            ////int n = x.Length;
            int n = x.Length < y.Length ? x.Length : y.Length;
            double sum_abs = 0;
            for (int i = 0; i < n; i++)
            {
                sum_abs += Math.Abs(x[i] - y[i]);
            }

            sum_abs /= n;
            return sum_abs;
        }

        /// <summary>
        /// Критерий Дарбина-Уотсона (Или Ватсона)
        /// Вычисляется по количеству элеменов в массиве x
        /// </summary>
        /// <param name="x">массив статистически значений</param>
        /// <param name="y">массив спрогнозированных значений</param>
        /// <returns>средний модуль</returns>
        public static double DW(double[] x, double[] y)
        {
            ////int n = x.Length;
            int n = x.Length < y.Length ? x.Length : y.Length;
            double sum_sq = 0;
            double crit = 0;
            for (int i = 0; i < n; i++)
            {
                sum_sq += (x[i] - y[i]) * (x[i] - y[i]);
            }

            for (int i = 1; i < n; i++)
            {
                crit += Math.Pow((x[i] - y[i]) - (x[i - 1] - y[i - 1]), 2);
            }

            if (sum_sq == 0)
            {
                crit = 1;
            }
            else
            {
                crit /= sum_sq;
            }

            return crit;
        }

        // Корреляция
        public static double Corr(double[] x, double[] y)
        {
            ////int n = x.Length;
            int n = x.Length < y.Length ? x.Length : y.Length;
            double d_x, sum_x, sum_y, d_y, sum_xy;
            double crit;
            d_x = 0;
            sum_x = 0;
            sum_y = 0;
            d_y = 0;
            sum_xy = 0;
            for (int i = 0; i < n; i++)
            {
                sum_x += x[i];
                sum_y += y[i];
            }
            
            for (int i = 0; i < n; i++)
            {
                sum_xy += (y[i] - (sum_y / n)) * (x[i] - (sum_x / n));
                d_x += (x[i] - (sum_x / n)) * (x[i] - (sum_x / n));
                d_y += (y[i] - (sum_y / n)) * (y[i] - (sum_y / n));
            }
            
            if (d_x == 0 || d_y == 0)
            {
                crit = 1;
            }
            else
            {
                crit = sum_xy / Math.Sqrt(d_x * d_y);
            }

            return crit;
        }
        
        /// <summary>
        /// Расчет F-критерия
        /// </summary>
        /// <param name="x">Входные данные</param>
        /// <param name="y">спрогнозированные данные</param>
        /// <param name="nh">число степеней свободы у</param>
        /// <returns>величина критерия</returns>
        public static double F(double[] x, double[] y, double nh) 
        {
            ////int n = x.Length;
            int n = x.Length < y.Length ? x.Length : y.Length;
            double v1, v2, crit;
            double r2 = R2(x, y);

            // Число степеней свободы
            v1 = nh - 1;
            v2 = n - 1;

            // Делим большую дисперсию на меньшую
            crit = r2 / (1 - r2) * (v2 - v1) / v1;
            return crit;
        }
        
        public static double Fcrit(double[] x, double[] y, double nh)
        {
            ////int n = x.Length;
            int n = x.Length < y.Length ? x.Length : y.Length;
            double v1, v2;
            
            // Число степеней свободы
            v1 = nh - 1;
            v2 = n - 1;

            double crit = F(x, y, nh);

            // Значение левого хвоста распределения. Грубо говоря есть распределение фишера-
            // некий график сложной функции. Площадь под графиком - 1. Мы с нашим отношением
            // дисперсий данных = crit находимся где-то на этом графике
            // слева от этой точки - левый хвост со своей площадью под графиком, а справа - 
            // правый хвост со своей площадью. 
            // Если справа осталось больше чем уровень значимости (5%), то различие дисперсий не существенно.
            double lefttail = FDistr.Incompletebeta(0.5 * v1, 0.5 * v2, 1 / (1 + (v2 / (v1 * crit))));
            ////double lefttail = Fdistr.incompletebeta(0.5 * (1), 0.5 * (10), 1 / (1 + 10 / (1 * 4.96)));
            return lefttail;
        }

        public static bool Fcrit(double[] x, double[] y, double nh, double p)
        {
            var lefttail = Fcrit(x, y, nh);
            if (lefttail > (1 - p))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // статистика R^2 
        public static double R2(double[] x, double[] y)
        {
            int n = x.Length < y.Length ? x.Length : y.Length;
            double sum_sq, d_x, sum_x;
            double crit = 0;
            sum_sq = 0;
            d_x = 0;
            sum_x = 0;
            for (int i = 0; i < n; i++)
            {
                sum_sq += (x[i] - y[i]) * (x[i] - y[i]);
                sum_x += x[i];
            }

            for (int i = 0; i < n; i++)
            {
                d_x += (x[i] - (sum_x / n)) * (x[i] - (sum_x / n));
            }

            if (d_x == 0)
            {
                crit = 1;
            }
            else
            {
                crit = 1 - (sum_sq / d_x);         // +
            }

            return crit;
        }

        public static double[] Criterii(double[] x, double[] y)
        {
            ////int n = x.Length;
            int n = x.Length < y.Length ? x.Length : y.Length;
            double sum_sq, d_x, sum_x, sum_y, d_y, sum_xy;
            double[] crit = new double[5];
            sum_sq = 0;
            d_x = 0;
            sum_x = 0;
            sum_y = 0;
            d_y = 0;
            sum_xy = 0;

            for (int i = 0; i < n; i++)
            {
                crit[0] += (x[i] - y[i]) * (x[i] - y[i]);
                //// crit[1] += Math.Abs(x[i] - y[i]);
                sum_x += x[i];
                sum_y += y[i];
            }

            sum_sq = crit[0];
            crit[0] /= n - 2; // +
            //// crit[1] /= N;
            for (int i = 0; i < n; i++)
            {
                if (i > 0)
                {
                    crit[1] += Math.Pow((x[i] - y[i]) - (x[i - 1] - y[i - 1]), 2);
                }

                sum_xy += (y[i] - (sum_y / n)) * (x[i] - (sum_x / n));
                d_x += (x[i] - (sum_x / n)) * (x[i] - (sum_x / n));
                d_y += (y[i] - (sum_y / n)) * (y[i] - (sum_y / n));
            }

            // Дарбин-Уолш
            crit[1] /= sum_sq;

            // Корреляция
            crit[2] = sum_xy / Math.Sqrt(d_x * d_y);   // +

            // F
            if (d_x > d_y)                   
            {
                crit[3] = d_x / d_y;
            }
            else
            {
                crit[3] = d_y / d_x;
            }

            // R^2
            crit[4] = 1 - (sum_sq / d_x);         // +

            return crit;
        }

        public static string Descript_MSE(double[] x, double[] y)
        {
            // Средний квадрат (Число Степеней Свободы = N-2)
            int n = x.Length;
            double mean_x = 0;
            double disp_x = 0;
            double crit = MSE(x, y);
            string result;

            for (int i = 0; i < n; i++)
            {
                mean_x += x[i];
            }

            mean_x /= n;

            for (int i = 0; i < n; i++)
            {
                disp_x += (x[i] - mean_x) * (x[i] - mean_x);
            }

            disp_x /= n;

            double snr = 10 * Math.Log10(disp_x / crit);

            if (snr > 10)
            {
                if (snr > 15)
                {
                    if (snr > 20)
                    {
                        result = "Применяемая модель обладает очень высокой точностью.";
                    }
                    else
                    {
                        result = "Применяемая модель хорошо аппроксимирует исходные данные.";
                    }
                }
                else
                {
                    result = "Применяемая модель приемлимо аппроксимирует исходные данные.";
                }
            }
            else
            {
                result = "Применяемая модель плохо аппроксимирует исходные данные.";
            }

            return result;
        }

        public static string Descript_MAE(double[] x, double[] y)
        {
            // Средний квадрат (Число Степеней Свободы = N-2)
            int n = x.Length;
            double mean_x = 0;
            double dev_x = 0;
            double crit = MAE(x, y);
            string result;

            for (int i = 0; i < n; i++)
            {
                mean_x += x[i];
            }

            mean_x /= n;

            for (int i = 0; i < n; i++)
            {
                dev_x += Math.Abs(x[i] - mean_x);
            }

            dev_x /= n;

            double snr = 20 * Math.Log10(dev_x / crit); // 20 а не 10 т.к. (mae(a)/mae(b))^2~mse(a)/mse(b)

            if (snr > 10)
            {
                if (snr > 15)
                {
                    if (snr > 20)
                    {
                        result = "Применяемая модель обладает очень высокой точностью.";
                    }
                    else
                    {
                        result = "Применяемая модель хорошо аппроксимирует исходные данные.";
                    }
                }
                else
                {
                    result = "Применяемая модель приемлимо аппроксимирует исходные данные.";
                }
            }
            else
            {
                result = "Применяемая модель плохо аппроксимирует исходные данные.";
            }

            return result;
        }

        public static string Descript_Corr(double[] x, double[] y)
        {
            // Средний квадрат (Число Степеней Свободы = N-2)
            double crit = Corr(x, y);
            string result;

            if (crit > 0)
            {
                if (crit > 0.5)
                {
                    if (crit > 0.8)
                    {
                        if (crit > 0.95)
                        {
                            result = "Данные почти полностью коррелируют с результатом моделирования.";
                        }
                        else  
                        {
                            // (0.8<crit<0.95)
                            result = "Данные значительно коррелируют с результатом моделирования.";
                        }
                    }
                    else  
                    {
                        // (0.5<crit<0.8)
                        result = "Данные коррелируют с результатом моделирования незначительно.";
                    }
                }
                else  
                {
                    // (0<crit<0.5)
                    result = "Данные не коррелируют с результатом моделирования.";
                }
            }
            else  
            {
                // (crit<0)
                result = "Отрицательная корреляция исходных данных и результата моделирования.";
            }

            return result;
        }

        public static string Descript_R2(double[] x, double[] y)
        {
            // Средний квадрат (Число Степеней Свободы = N-2)
            double crit = R2(x, y);
            string result;

            if (crit > 0.7)
            {
                if (crit > 0.9)
                {
                    if (crit > 0.95)
                    {
                        result = "Данные почти полностью коррелируют с результатом моделирования.";
                    }
                    else 
                    {
                        // (0.9<crit<0.95)
                        result = "Данные значительно коррелируют с результатом моделирования.";
                    }
                }
                else 
                {
                    // (0.7<crit<0.9)
                    result = "Данные коррелируют с результатом моделирования незначительно.";
                }
            }
            else 
            {
                // (crit<0.7)
                result = "Данные не коррелируют с результатом моделирования.";
            }

            return result;
        }

        public static string Descript_F(double[] x, double[] y, double n_h)
        {
            int n = x.Length;
            double v1, v2, crit;
            double r2 = R2(x, y);
            string result;

            // Число степеней свободы
            v1 = n_h - 1;
            v2 = n - 1;

            // Делим большую дисперсию на меньшую
            crit = r2 / (1 - r2) * (v2 - v1) / v1;

            double lefttail = FDistr.Incompletebeta(0.5 * v1, 0.5 * v2, 1 / (1 + (v2 / (v1 * crit))));
            lefttail *= 100;
            ////double lefttail = Fdistr.incompletebeta(0.5 * (1), 0.5 * (10), 1 / (1 + 10 / (1 * 4.96)));

            /*///if (crit > 0.7)
            //    if (crit > 0.9)
            //        if (crit > 0.95)
            //            result = "Данные коррелируют с регрессией почти полностью.";
            //        else
            //            result = "Данные коррелируют с регрессией значительно.";
            //    else
            //        result = "Данные коррелированы с регрессией незначительно.";
            //else
            //    result = "Регрессия не коррелирует с исходными данными.";*/

            result = lefttail == 0 ? "Регрессия не значима." : "Регрессия значима с вероятностью: " + lefttail.ToString("#.#") + "%.";
            return result;
        }

        public static string Descript_DW(double[] x, double[] y)
        {
            double crit = DW(x, y);
            double ro = 1 - (crit / 2);
            string result;

            if (ro > 0.5)
            {
                if (ro > 0.9)
                {
                    result = "В остатках присутствует значительная корреляция.";
                }
                else           
                {
                    // (0.5<ro<0.9)
                    result = "В остатках присутствует незначительная корреляция.";
                }
            }
            else
            {
                if (ro < -0.5) 
                {
                    // (ro<-0.5)
                    result = "В остатках присутствует отрицательная корреляция.";
                }
                else
                {   // (-0.5<ro<0.5)
                    result = "Остатки не коррелируют друг с другом. Остатки случайны.";
                }
            }

            return result;
        }
    }
}
