using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Krista.FM.RIA.Extensions.Forecast.MathStat
{
    // TODO: связанное письмо от Мочалова #260511
    public class MultiRegr
    {
        private double[,] x;
        private double[] y;
        private int[] weights;
        private int paramCount;
        private int yearCount;
        
        public MultiRegr(double[,] x, double[] y, bool[] weights)
        {
            this.x = x;
            this.y = y;

            paramCount = x.GetLength(1);
            yearCount = y.GetLength(0);

            this.weights = new int[weights.Count()];

            for (int i = 0; i < weights.Length; i++)
            {
                this.weights[i] = weights[i] ? 1 : 0;
            }
        }

        public MultiRegr(double[,] x, double[] y)
        {
            this.x = x;
            this.y = y;
            this.weights = null;
        }

        public double[] Coeff { get; private set; }

        public bool[] UsedInModel { get; private set; }

        public double[] CalcRegression()
        {
            if (weights == null)
            {
                return Multivariable_Links_Without_Weights();
            }
            else
            {
                return Multivariable_Links_With_Weights();
            }
        }

        public double[] Predict(double[,] xin)
        {
            int he = xin.GetLength(0);
            int wi = xin.GetLength(1);
            
            double[,] inx = new double[he, wi + 1];
            
            for (int i = 0; i < he; i++)
            {
                inx[i, 0] = 1;
                
                for (int j = 0; j < wi; j++)
                {
                    inx[i, j + 1] = xin[i, j];
                }
            }

            return CalcY(inx, UsedInModel, Coeff);
        }

        // Функция строит результат моделирования по множественной регрессионной модели
        public double[] Multivariable_Links_With_Weights()
        {
            double[] y_x = new double[yearCount];
            double[] y_w = new double[yearCount];
            double[,] x_w = new double[yearCount, paramCount + 1];
            for (int i = 0; i < yearCount; i++)
            {
                y_w[i] = y[i] * weights[i];
            }

            for (int i = 0; i < yearCount; i++)
            {
                x_w[i, 0] = weights[i];
                for (int j = 0; j < paramCount; j++)
                {
                    x_w[i, j + 1] = x[i, j] * weights[i];
                }
            }

            y_w = Multivariable_Links(x_w, y_w);
            for (int i = 0; i < y_w.Length; i++)
            {
                if (weights[i] != 0)
                {
                    y_x[i] = y_w[i] / weights[i];
                }
                else
                {
                    y_x[i] = 0;
                }
            }

            return y_x;
        }

        public double[] Multivariable_Links_Without_Weights()
        {
            double[] y_x = new double[yearCount];
            double[] y_w = new double[yearCount];
            double[,] x_w = new double[yearCount, paramCount + 1];
            
            for (int i = 0; i < yearCount; i++)
            {
                y_w[i] = y[i];
            }

            for (int i = 0; i < yearCount; i++)
            {
                x_w[i, 0] = 1;
                for (int j = 0; j < paramCount; j++)
                {
                    x_w[i, j + 1] = x[i, j];
                }
            }

            y_x = Multivariable_Links(x_w, y_w);

            return y_x;
        }

        private static double[] Solve_SLAE(double[,] temp_XX, double[] temp_Xy, int order)
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

        private static double[] CalcY(double[,] inx, bool[] is_in_model, double[] coeff)
        {
            int he = inx.GetLength(0);
            int wi = inx.GetLength(1);

            double[] y_x = new double[he];

            int j0;
            j0 = 0;
            for (int i = 0; i < wi; i++)
            {
                if (is_in_model[i])
                {
                    for (int j = 0; j < he; j++)
                    {
                        y_x[j] += coeff[j0] * inx[j, i];
                    }

                    j0++;
                }
            }

            return y_x;
        }

        private static void AddFactorsInModel(double[,] x, double[] y, ref bool[] is_in_model, double[] matXy, double[,] matXX)
        {
            int he = x.GetLength(0);
            int wi = x.GetLength(1);

            int v1 = 1;

            // Степение свободы
            int v2 = he;

            // индекс величины с лучшей f-статистикой
            int fstat_index;

            // Критическое F-значение
            //// double f_critical = 4.96;

            // Критическое p-значение, соответствующее критическому F-значению
            double criticalP = 0.95;

            // Значение критерия R^2
            double r2;

            bool[] is_in_model0 = new bool[wi];

            // Массив F-значений
            double[] arrf_multi = new double[wi];

            // Значение аппроксимации
            double[] y_x = new double[he];

            double[] coeffs;

            // Пытаемся найти первого кандидата на включение в множественную регрессию
            fstat_index = 1;
            for (int i = 1; i < wi; i++)
            {
                Array.Copy(is_in_model, is_in_model0, wi);

                // добавляем одного кандидата в модель
                is_in_model0[i] = true;

                // пересчитываем модель
                y_x = EvaluateMultiModel(x, matXX, matXy, is_in_model0, out coeffs);
                r2 = Criterias.R2(y, y_x);

                // находим частные F-значения
                if (r2 == 1)
                {
                    arrf_multi[i] = 1000000000000;
                }
                else
                {
                    arrf_multi[i] = r2 / (1 - r2) * (v2 - v1 - 1) / v1;
                }

                // находим кандидата с наибольшим F- значением
                if (arrf_multi[i] > arrf_multi[fstat_index])
                {
                    fstat_index = i;
                }
            }

            // если найденный кандидат имеет p-значение больше критического добавляем его в модель
            // На первом этапе если таких кандидатов нет необходимо вывести сообщение "нет корреляции"
            // if (f_multi[f_index] > f_critical)
            //    is_in_model[f_index] = true;
            // else
            //    Console.Write("нет корреляции");
            if (criticalP < FDistr.Incompletebeta(0.5 * v1, 0.5 * v2, 1 / (1 + (v2 / (v1 * arrf_multi[fstat_index])))))
            {
                is_in_model[fstat_index] = true;
            }
            else
            {
                ////   Console.Write("нет корреляции");
            }

            // делаем много итераций включения-исключения 
            for (int count = 1; count < 100; count++)
            {
                // Исключение (проверяем можно ли исключить незначимого кандидата из модели)   
                int factors_in_model;
                double baseValueR2;

                // смотрим как модель работает со всеми кандидатами
                y_x = EvaluateMultiModel(x, matXX, matXy, is_in_model, out coeffs);
                baseValueR2 = Criterias.R2(y, y_x);

                // опеределяем сколько факторов в модели
                factors_in_model = 0;
                fstat_index = 0;
                for (int i = 1; i < wi; i++)
                {
                    if (is_in_model[i])
                    {
                        factors_in_model++;
                    }
                }

                for (int i = 1; i < wi; i++)
                {
                    if (is_in_model[i])
                    {
                        fstat_index = i;
                        break;
                    }
                }

                // Проверяем изменения модели при удалении кандидатов, которые уже в модели по-одному    
                for (int i = 1; i < wi; i++)
                {
                    if (is_in_model[i])
                    {
                        // Копируем список кандидатов
                        Array.Copy(is_in_model, is_in_model0, wi);

                        // Если i-ый кандидат в модели присутствует - удаляем его
                        is_in_model0[i] = false;

                        // пересчитываем модль и частное F-значение
                        y_x = EvaluateMultiModel(x, matXX, matXy, is_in_model0, out coeffs);
                        r2 = Criterias.R2(y, y_x);
                        if (baseValueR2 == 1)
                        {
                            arrf_multi[i] = 1000000000000;
                        }
                        else
                        {
                            arrf_multi[i] = (baseValueR2 - r2) / (1 - baseValueR2) * (v2 - v1 - factors_in_model) / v1;
                        }

                        // находим учатника модели с наименьшим F- значением
                        if (arrf_multi[i] < arrf_multi[fstat_index])
                        {
                            fstat_index = i;
                        }
                    }
                }

                // если наименьшее p-значение меньше попрога - удаляем кандидата
                // if (f_multi[f_index] < f_critical)
                //    is_in_model[f_index] = false;
                if (criticalP > FDistr.Incompletebeta(0.5 * v1, 0.5 * v2, 1 / (1 + (v2 / (v1 * arrf_multi[fstat_index])))))
                {
                    is_in_model[fstat_index] = false;
                }

                // Включение в модель новых кандидатов////////////////////////////////////////

                // сколько кандидатов уже в модели
                factors_in_model = 0;
                for (int i = 1; i < wi; i++)
                {
                    if (is_in_model[i])
                    {
                        factors_in_model++;
                    }
                }

                for (int i = 1; i < wi; i++)
                {
                    if (!is_in_model[i])
                    {
                        fstat_index = i;
                        break;
                    }
                }

                // F_crit=finv(0.95,V1,V2-sum(g)-1);
                y_x = EvaluateMultiModel(x, matXX, matXy, is_in_model, out coeffs);
                baseValueR2 = Criterias.R2(y, y_x);
                for (int i = 1; i < wi; i++)
                {
                    if (!is_in_model[i])
                    {
                        // Если i-ый кандидат не в модели 
                        // Копируем список кандидатов
                        Array.Copy(is_in_model, is_in_model0, wi);

                        // Если i-ый кандидат не в модели - проверяем что будет если его добавить
                        is_in_model0[i] = true;

                        // Пересчитываем модель
                        y_x = EvaluateMultiModel(x, matXX, matXy, is_in_model0, out coeffs);
                        r2 = Criterias.R2(y, y_x);
                        if (r2 == 1)
                        {
                            arrf_multi[i] = 1000000000000;
                        }
                        else
                        {
                            // Факторов на 1 больше так как один мы временно добавили
                            arrf_multi[i] = (r2 - baseValueR2) / (1 - r2) * (v2 - v1 - factors_in_model - 1) / v1;
                        }

                        if (arrf_multi[i] > arrf_multi[fstat_index])
                        {
                            fstat_index = i;
                        }
                    }
                    else
                    {
                        arrf_multi[i] = 0;
                    }
                }
                //// if (f_multi[f_index] > f_critical)
                //// is_in_model[f_index] = true;

                if (criticalP < FDistr.Incompletebeta(0.5 * v1, 0.5 * v2, 1 / (1 + (v2 / (v1 * arrf_multi[fstat_index])))))
                {
                    is_in_model[fstat_index] = true;
                }
                else
                {
                    break;
                }
            } // end for count  
        }

        private static double[] EvaluateMultiModel(double[,] x, double[,] matXX, double[] matXy, bool[] is_in_model, out double[] coeffs)
        {
            int wi = x.GetLength(1);

            int temp_wi = 1;
            for (int i = 1; i < wi; i++)
            {
                if (is_in_model[i])
                {
                    temp_wi++;
                }
            }

            double[] coeff = new double[temp_wi];
            double[] temp_Xy = new double[temp_wi];
            double[,] temp_XX = new double[temp_wi, temp_wi];
            int i0 = 0;
            int j0;

            for (int i = 0; i < wi; i++)
            {
                if (is_in_model[i])
                {
                    j0 = 0;
                    for (int j = 0; j < wi; j++)
                    {
                        if (is_in_model[j])
                        {
                            temp_XX[i0, j0++] = matXX[i, j];
                        }
                    }

                    i0++;
                }
            }

            j0 = 0;
            for (int j = 0; j < wi; j++)
            {
                if (is_in_model[j])
                {
                    temp_Xy[j0++] = matXy[j];
                }
            }

            coeff = Solve_SLAE(temp_XX, temp_Xy, temp_wi);

            coeffs = coeff;

            return CalcY(x, is_in_model, coeff);
        }

        private double[] Multivariable_Links(double[,] x, double[] y)
        {
            int he = x.GetLength(0);
            int wi = x.GetLength(1);

            // Степение свободы
            int v2 = he;
            
            // Матрица X'*X и вектор X'*y (Для удобства вычислений)
            double[] matXy = new double[wi];
            double[,] matXX = new double[wi, wi];

            // Находи матрицу X'*X
            for (int i1 = 0; i1 < wi; i1++)
            {
                for (int j1 = i1; j1 < wi; j1++)
                {
                    matXX[i1, j1] = 0;
                    for (int i = 0; i < he; i++)
                    {
                        matXX[i1, j1] += x[i, i1] * x[i, j1];
                    }

                    if (i1 != j1)
                    {
                        matXX[j1, i1] = matXX[i1, j1];
                    }
                }
            }

            // Находим вектор X'*y
            for (int i1 = 0; i1 < wi; i1++)
            {
                matXy[i1] = 0;
                for (int i = 0; i < v2; i++)
                {
                    matXy[i1] += x[i, i1] * y[i];  //// y[i]
                }
            }
            
            // Присутствие величины в моделе
            bool[] is_in_model = new bool[wi];

            if (false)
            {
                is_in_model[0] = true;
                for (int i = 1; i < wi; i++)
                {
                    is_in_model[i] = false;
                }

                AddFactorsInModel(x, y, ref is_in_model, matXy, matXX);
            }
            else
            {
                for (int i = 0; i < wi; i++)
                {
                    is_in_model[i] = true;
                }
            }

            // Значение аппроксимации
            double[] y_x = new double[he];

            double[] coeffs;
            
            y_x = EvaluateMultiModel(x, matXX, matXy, is_in_model, out coeffs);

            Coeff = coeffs;

            UsedInModel = is_in_model;

            return y_x;
        }
    }
}
