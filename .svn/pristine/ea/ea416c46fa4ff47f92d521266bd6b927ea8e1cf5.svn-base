using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Krista.FM.RIA.Extensions.Forecast.MathStat
{
    public class PCAForecast
    {
        private double[,] x;             // матрица данных
        private int numParams;            // число параметров (число времянныхрядов) число строк
        private int numYear;              // число столбцов число лет
        private bool[] indexesExist;    // идентификатор для каких из временных рядов есть оценки на прогнозный год     
        private double[] expertValues;  // значения экспертных оценок на прогнозный год для тех временных рядов,
                                         // для которых они имеются

        // конструктор
        public PCAForecast(double[,] x, bool[] indexesExist, double[] expertValues)
        {
            this.numParams = x.GetLength(0);
            this.numYear = x.GetLength(1);
            this.x = x;
            this.indexesExist = indexesExist;
            this.expertValues = expertValues;
        }

        // решение СЛАУ
        public static double[] Solve_SLAE(double[,] temp_XX, double[] temp_Xy)
        {
            int order = temp_XX.GetLength(0);
            
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
        
        // PCA анализ (поиск собственных чисел и собственных векторов ковариационной матрицы)
        // ковариационная матрица x0, размером nxn.
        public static void PCA(double[,] x0, int n, out double[,] basis, out double[] sing_values)
        {
            basis = new double[n, n];             // базис итоговый 
            double[,] x = new double[n, n];       // копируем x0
            sing_values = new double[n];          // итоговые собственные числв
            double[] a = new double[n];           // промежуточные вектора
            double[] b = new double[n];
            Random rand = new Random();
            int ibasis, i, j, cntr;
            int max_iter = n * 4;

            for (i = 0; i < n; i++)
            {
                for (j = 0; j < n; j++)
                {
                    x[i, j] = x0[i, j];
                }
            }

            // последовательно ищем вектора выделяющие максиум остаточной энергии
            // сложнообъяснимый иттерационный алгоритм, для которого проверенно что он работает
            for (ibasis = 0; ibasis < n; ibasis++)
            {
                // нужно минимизировать norm( x(i)-<x(i)*basis_vector>basis_vector)
                // это задача может быть сведена к последовательному поиску собственных векторов.
                double anorm = 0;
                double bnorm = 0;

                for (i = 0; i < n; i++)
                {
                    a[i] = ((double)rand.Next(1000) / 500) - 1;
                }

                for (cntr = 0; cntr < max_iter; cntr++)
                {
                    anorm = 0;
                    for (i = 0; i < n; i++)
                    {
                        anorm += a[i] * a[i];
                    }

                    anorm = Math.Sqrt(anorm);
                    for (i = 0; i < n; i++)
                    {
                        a[i] /= anorm;
                    }

                    for (i = 0; i < n; i++)
                    {
                        b[i] = 0;

                        for (j = 0; j < n; j++)
                        {
                            b[i] += x[i, j] * a[j];
                        }
                    }

                    bnorm = 0;
                    for (i = 0; i < n; i++)
                    {
                        bnorm += b[i] * b[i];
                    }

                    bnorm = Math.Sqrt(bnorm);
                    for (i = 0; i < n; i++)
                    {
                        b[i] /= bnorm;
                    }

                    for (j = 0; j < n; j++)
                    {
                        a[j] = 0;

                        for (i = 0; i < n; i++)
                        {
                            a[j] += x[i, j] * b[i];
                        }
                    }
                }

                sing_values[ibasis] = Math.Sqrt(anorm * bnorm);

                for (i = 0; i < n; i++)
                {
                    basis[i, ibasis] = b[i];
                }

                for (i = 0; i < n; i++)
                {
                    for (j = 0; j < n; j++)
                    {
                        x[i, j] = x[i, j] - (a[j] * b[i]);
                    }
                }
            } // end_for i_basis
        }
        
        // метод стандартного (не тренд) прогнозирования
        public double[] StandartForecast()
        {
            return StandartForecastStatic(x, numParams, numYear, indexesExist, expertValues);
        }

        // метод прогнозирования с вычитанием трендов
        public double[] TrendForecast()
        {
            return TrendForecastStatic(x, numParams, numYear, indexesExist, expertValues); 
        }

        private double[] StandartForecastStatic(double[,] x, int numParams, int numYear, bool[] indexes_exist, double[] expert_values)
        {
            double energy = 0.75;                 // уровень энергии от 0 до 1 которую должен выделять набор базисных векторов
            int numFactors = 0;                      // число факторов в модели = число базисных векторов, которые остаются
            double[,] x_normed;                     // нормированная Х
            x_normed = new double[numParams, numYear];
            double[,] x_2;                          // матричное произведение x_normed'*x_normed
            x_2 = new double[numParams, numParams];
            int cntr;                               // счетчик
            double[] dispA = new double[numParams];  // дисперсия рядов
            double[] meanA = new double[numParams];  // матожидание рядов
            int sum_indexes_exist = 0;              // число существующих экспертных оценок
        
            for (int i = 0; i < numParams; i++)
            {
                if (indexes_exist[i])
                { 
                    sum_indexes_exist++; 
                }
            }

            double[] expert_values_normed;          // нормированные экспертные оценки
            expert_values_normed = new double[sum_indexes_exist];

            for (int i = 0; i < numParams; i++)
            {
                meanA[i] = 0;
                for (int j = 0; j < numYear; j++)
                {
                    meanA[i] += x[i, j];
                }

                meanA[i] /= numYear;
            }

            for (int i = 0; i < numParams; i++)
            {
                dispA[i] = 0;
                for (int j = 0; j < numYear; j++)
                {
                    dispA[i] += (x[i, j] - meanA[i]) * (x[i, j] - meanA[i]);
                }

                dispA[i] /= numYear;
            }

            // нормировка экспертных оценок
            cntr = 0;
            for (int i = 0; i < numParams; i++)
            {
                if (indexes_exist[i])
                {
                    expert_values_normed[cntr] = (expert_values[cntr] - meanA[i]) / Math.Sqrt(dispA[i]);
                    cntr++;
                }
            }

            // нормировка матрицы данных
            for (int i = 0; i < numParams; i++)
            {
                for (int j = 0; j < numYear; j++)
                {
                    x_normed[i, j] = (x[i, j] - meanA[i]) / Math.Sqrt(dispA[i]);
                }
            }

            // вычисление x_2 (делить на n_year не обязательно - это ничего не меняет) 
            for (int i = 0; i < numParams; i++)
            {
                for (int j = 0; j < numParams; j++)
                {
                    x_2[i, j] = 0;

                    for (int i1 = 0; i1 < numYear; i1++)
                    {
                        x_2[i, j] += x_normed[i, i1] * x_normed[j, i1] / numYear;
                    }
                }
            }
            
            double[,] basis;             // матрица базисных вектор-столбцов
            double[] sing_values;        // значение энергий, выделяемых i-ым вектором
            double sum_sqrt_W = 0;       // общая энергия
            double sum_sqrt_W_part = 0;  // ее часть

            // Анализ главных компонент
            PCA(x_2, numParams, out basis, out sing_values);

            // вычисление полной энергии
            for (int i = 0; i < numParams; i++)
            {
                sum_sqrt_W += Math.Sqrt(sing_values[i]);
            }

            // выбор числа факторов для прогнозированию
            // nFactors векторов должны выделять более t_energy (75%) энергии
            numFactors = 0;
            for (int i = 0; i < numParams; i++)
            {
                sum_sqrt_W_part += Math.Sqrt(sing_values[i]);
                numFactors++;
                if (sum_sqrt_W_part > energy * sum_sqrt_W)
                {
                    break;
                }
            }

            // чтобы задача была разрешима число оставленных факторов должно быть меньше числа экспертных оценок
            numFactors = Math.Min(numFactors, sum_indexes_exist);
            
            // Решение задачи оптимизации ||cutted_basis*coef-expert_values_normed||->min
            double[,] cutted_basis;
            cutted_basis = new double[sum_indexes_exist, numFactors];
            cntr = 0;
            for (int i = 0; i < numParams; i++)
            {
                if (indexes_exist[i])
                {
                    for (int j = 0; j < numFactors; j++)
                    {
                        cutted_basis[cntr, j] = basis[i, j];
                    }

                    cntr++;
                }
            }

            cntr = 0;

            double[] xy = new double[numFactors];
            double[,] xx = new double[numFactors, numFactors];

            // Находи матрицу cutted_basis'*cutted_basis
            for (int i1 = 0; i1 < numFactors; i1++)
            {
                for (int j1 = i1; j1 < numFactors; j1++)
                {
                    xx[i1, j1] = 0;
                    for (int i = 0; i < sum_indexes_exist; i++)
                    {
                        xx[i1, j1] += cutted_basis[i, i1] * cutted_basis[i, j1];
                    }

                    if (i1 != j1)
                    {
                        xx[j1, i1] = xx[i1, j1];
                    }
                }
            }

            // Находим вектор  cutted_basis' * expert_values_normed
            for (int i1 = 0; i1 < numFactors; i1++)
            {
                xy[i1] = 0;
                for (int i = 0; i < sum_indexes_exist; i++)
                {
                    xy[i1] += cutted_basis[i, i1] * expert_values_normed[i];
                }
            }

            int temp_wi = sum_indexes_exist;
            double[] coeff = new double[temp_wi];
            double[] temp_Xy = new double[temp_wi];
            double[,] temp_XX = new double[temp_wi, temp_wi];

            for (int i = 0; i < numFactors; i++)
            {
                for (int j = 0; j < numFactors; j++)
                {
                    temp_XX[i, j] = xx[i, j];
                }
            }

            for (int j = 0; j < numFactors; j++)
            {
                temp_Xy[j] = xy[j];
            }
            
            coeff = Solve_SLAE(temp_XX, temp_Xy);

            // coeff - это коэффициенты при базисных векторах в прогнозном году

            // необходимо произвести обратную нормировку
            double[] back_norm;
            back_norm = new double[numParams];
            for (int i = 0; i < numParams; i++)
            {
                back_norm[i] = 0;
                for (int j = 0; j < numFactors; j++)
                {
                    back_norm[i] += basis[i, j] * coeff[j];
                }
            }

            for (int i = 0; i < numParams; i++)
            {
                back_norm[i] = (back_norm[i] * Math.Sqrt(dispA[i])) + meanA[i];
            }

            return back_norm;
        }

        // Прогнозирование PCA с вычитанием тренда
        private double[] TrendForecastStatic(double[,] x, int numParams, int numYear, bool[] indexes_exist, double[] expert_values)
        {
            // Переменные играют те жн роли
            double energy = 0.75;                 // уровень энергии от 0 до 1 которую должен выделять набор базисных векторов
            int numFactors = 0;                      // число факторов в модели = число базисных векторов, которые остаются
            double[,] x_normed;                     // нормированная Х
            x_normed = new double[numParams, numYear];
            double[,] x_2;                          // матричное произведение x_normed'*x_normed
            x_2 = new double[numParams, numParams];
            int sum_indexes_exist = 0;                // число существующих экспертных оценок
            for (int i = 0; i < numParams; i++)
            {
                if (indexes_exist[i])
                { 
                    sum_indexes_exist++; 
                }
            }

            double[] expert_values_normed;          // нормированные экспертные оценки с вычтенным трендом
            expert_values_normed = new double[sum_indexes_exist];
            int cntr = 0;                               // счетчик
            double[] trend_next_year = new double[numParams];       // значение тренда на прогнозный (n_year)год
            double[] dispA = new double[numParams];  // дисперсия рядов
            double[] meanA = new double[numParams];  // матожидание рядов

            // Вычисление коэффициентов линейной регрессии для каждого ряда
            for (int i = 0; i < numParams; i++)
            {
                double ax, bx;
                double[,] a = new double[2, 2] { { 0, 0 }, { 0, 0 } };
                double[] b = new double[2] { 0, 0 };
            
                for (int j = 0; j < numYear; j++)
                {
                    ////if (X[j] == 0)
                    ////     bin[j] = 0;           
                    ////else
                    ////     bin[j] = 1;
                    a[0, 0] += j * j; // *bin[i];
                    a[0, 1] += j; // *bin[i];
                    a[1, 0] += j; // *bin[i];
                    a[1, 1] += 1; // bin[i];
                    b[0] += x[i, j] * j; // *bin[i];
                    b[1] += x[i, j]; // * bin[i];
                }

                ax = ((a[1, 1] * b[0]) - (a[1, 0] * b[1])) / ((a[0, 0] * a[1, 1]) - (a[0, 1] * a[1, 0]));
                bx = ((-a[0, 1] * b[0]) + (a[0, 0] * b[1])) / ((a[0, 0] * a[1, 1]) - (a[0, 1] * a[1, 0]));
                trend_next_year[i] = (ax * numYear) + bx; // Запомнить значение тренда для прогнозного года

                for (int j = 0; j < numYear; j++)
                {
                    x_normed[i, j] = x[i, j] - (ax * j) - bx; // вычитание тренда
                }

                dispA[i] = 0;
                for (int j = 0; j < numYear; j++)
                {
                    dispA[i] += x_normed[i, j] * x_normed[i, j]; // вычисление дисперсии (среднее=0)
                }

                dispA[i] /= numYear;

                // вычитание тренда и нормировка экспертных оценок
                if (indexes_exist[i])
                {
                    expert_values_normed[cntr] = (expert_values[cntr] - trend_next_year[i]) / Math.Sqrt(dispA[i]);
                    cntr++;
                }

                // нормировка входных данных с вычтенным трендом
                for (int j = 0; j < numYear; j++)
                {
                    x_normed[i, j] = x_normed[i, j] / Math.Sqrt(dispA[i]);
                }
            }

            // вычисление x_2 (делить на n_year не обязательно - это ничего не меняет) 
            for (int i = 0; i < numParams; i++)
            {
                for (int j = 0; j < numParams; j++)
                {
                    x_2[i, j] = 0;
                    for (int i1 = 0; i1 < numYear; i1++)
                    {
                        x_2[i, j] += x_normed[i, i1] * x_normed[j, i1] / numYear;
                    }
                }
            }

            double[,] basis;             // матрица базисных вектор-столбцов
            double[] sing_values;        // значение энергий, выделяемых i-ым вектором
            double sum_sqrt_W = 0;       // общая энергия
            double sum_sqrt_W_part = 0;  // ее часть

            // Анализ главных компонент
            PCA(x_2, numParams, out basis, out sing_values);

            // вычисление полной энергии
            for (int i = 0; i < numParams; i++)
            {
                sum_sqrt_W += Math.Sqrt(sing_values[i]);
            }

            // выбор числа факторов для прогнозированию
            // nFactors векторов должны выделять более t_energy (75%) энергии
            numFactors = 0;
            for (int i = 0; i < numParams; i++)
            {
                sum_sqrt_W_part += Math.Sqrt(sing_values[i]);
                numFactors++;
                if (sum_sqrt_W_part > energy * sum_sqrt_W)
                {
                    break;
                }
            }

            // для того, чтобы задача была разрешима число оставленных факторов 
            // должно быть меньше числа экспертных оценок
            numFactors = Math.Min(numFactors, sum_indexes_exist);

            // Решение задачи оптимизации ||cutted_basis*coef-expert_values_normed||->min
            double[,] cutted_basis;
            cutted_basis = new double[sum_indexes_exist, numFactors];
            cntr = 0;
            for (int i = 0; i < numParams; i++)
            {
                if (indexes_exist[i])
                {
                    for (int j = 0; j < numFactors; j++)
                    {
                        cutted_basis[cntr, j] = basis[i, j];
                    }

                    cntr++;
                }
            }

            cntr = 0;

            double[] xy = new double[numFactors];
            double[,] xx = new double[numFactors, numFactors];

            // Находи матрицу cutted_basis'*cutted_basis
            for (int i1 = 0; i1 < numFactors; i1++)
            {
                for (int j1 = i1; j1 < numFactors; j1++)
                {
                    xx[i1, j1] = 0;
                    for (int i = 0; i < sum_indexes_exist; i++)
                    {
                        xx[i1, j1] += cutted_basis[i, i1] * cutted_basis[i, j1];
                    }

                    if (i1 != j1)
                    {
                        xx[j1, i1] = xx[i1, j1];
                    }
                }
            }

            // Находим вектор  cutted_basis' * expert_values_normed
            for (int i1 = 0; i1 < numFactors; i1++)
            {
                xy[i1] = 0;
                for (int i = 0; i < sum_indexes_exist; i++)
                {
                    xy[i1] += cutted_basis[i, i1] * expert_values_normed[i];
                }
            }

            int temp_wi = sum_indexes_exist;
            double[] coeff = new double[temp_wi];
            double[] temp_Xy = new double[temp_wi];
            double[,] temp_XX = new double[temp_wi, temp_wi];

            for (int i = 0; i < numFactors; i++)
            {
                for (int j = 0; j < numFactors; j++)
                {
                    temp_XX[i, j] = xx[i, j];
                }
            }

            for (int j = 0; j < numFactors; j++)
            {
                temp_Xy[j] = xy[j];
            }

            coeff = Solve_SLAE(temp_XX, temp_Xy);

            // coeff - это коэффициенты при базисных векторах в прогнозном году

            // необходимо произвести обратную нормировку и прибавление тренда
            double[] back_norm;
            back_norm = new double[numParams];
            for (int i = 0; i < numParams; i++)
            {
                back_norm[i] = 0;
                for (int j = 0; j < numFactors; j++)
                {
                    back_norm[i] += basis[i, j] * coeff[j]; // сложение базисных векторов с весами
                }
            }

            // обратная нормировка и прибавление тренда
            for (int i = 0; i < numParams; i++)
            {
                back_norm[i] = (back_norm[i] * Math.Sqrt(dispA[i])) + trend_next_year[i];
            }

            return back_norm;
        }
    }
}
