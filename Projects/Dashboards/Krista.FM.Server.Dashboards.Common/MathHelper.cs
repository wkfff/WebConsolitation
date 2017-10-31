using System;
using System.Data;
using System.Collections.Generic;

namespace Krista.FM.Server.Dashboards.Common
{
	/// <summary>
	/// Класс для математичских вычислений
	/// </summary>
	public static class MathHelper
    {

        #region

        /// <summary>
		/// Расчет среднего геометрического значений строки таблицы
		/// </summary>
		/// <param name="row">Строка таблицы</param>
		/// <param name="start">Стартовый индекс (с какой ячейки начинать)</param>
		/// <param name="start">До какой ячейки делать</param>
		/// <param name="step">Шаг (1 - все ячейки, 2 - через одну и т.д.)</param>
		/// <param name="defaultValue">Значение по умолчанию</param>
		/// <returns></returns>
		public static double GeoMean(DataRow row, int start, int finish, int step, double defaultValue)
		{
			double result = defaultValue;
			int valuesCount = 0;
			for (int i = start; i <= finish && i < row.ItemArray.Length; i += step)
			{
				double value;
				if (Double.TryParse(row[i].ToString(), out value) && (value != 0))
				{
					result = valuesCount == 0 ? value : value * result;
					++valuesCount;
				}
			}
			if (valuesCount == 0)
				return defaultValue;
			else
				return Math.Pow(result, 1.0 / valuesCount);
		}

		/// <summary>
		/// Расчет среднего геометрического значений строки таблицы
		/// </summary>
		/// <param name="row">Строка таблицы</param>
		/// <param name="start">Стартовый индекс (с какой ячейки начинать)</param>
		/// <param name="start">До какой ячейки делать</param>
		/// <param name="step">Шаг (1 - все ячейки, 2 - через одну и т.д.)</param>
		/// <param name="defaultValue">Значение по умолчанию</param>
		/// <returns>Среднее геометрическое</returns>
		public static object GeoMean(DataRow row, int start, int step, object defaultValue)
		{
			object result = defaultValue;
			int valuesCount = 0;
			for (int i = start; i < row.ItemArray.Length; i += step)
			{
				double value;
				if (Double.TryParse(row[i].ToString(), out value) && (value != 0))
				{
					result = valuesCount == 0 ? value : value * Convert.ToDouble(result);
					++valuesCount;
				}
			}
			if (valuesCount == 0)
				return defaultValue;
			else
				return Math.Pow(Convert.ToDouble(result), 1.0 / valuesCount);
		}

        /// <summary>
        /// Расчет среднего геометрического значений строки таблицы
        /// </summary>
        /// <param name="row">Строка таблицы</param>
        /// <param name="start">Стартовый индекс (с какой ячейки начинать)</param>
        /// <param name="start">До какой ячейки делать</param>
        /// <param name="step">Шаг (1 - все ячейки, 2 - через одну и т.д.)</param>
        /// <param name="defaultValue">Значение по умолчанию</param>
        /// <returns>Среднее геометрическое</returns>
        public static object GeoMean(DataRow row, int start, int step, object defaultValue, object additionalValue)
        {
            object result = (additionalValue != DBNull.Value) && (AboveZero(additionalValue)) ? additionalValue : defaultValue;
            int valuesCount = (additionalValue != DBNull.Value) && (AboveZero(additionalValue)) ? 1 : 0;
            for (int i = start; i < row.ItemArray.Length; i += step)
            {
                double value;
                if (Double.TryParse(row[i].ToString(), out value) && (value != 0))
                {
                    result = valuesCount == 0 ? value : value * Convert.ToDouble(result);
                    ++valuesCount;
                }
            }
            if (valuesCount == 0)
                return defaultValue;
            else
                return Math.Pow(Convert.ToDouble(result), 1.0 / valuesCount);
        }

        /// <summary>
        /// Расчет среднего геометрического значений столбца таблицы
        /// </summary>
        /// <param name="column">Столбец, по которому считаем</param>
        /// <param name="defaultValue">Значение по умолчанию</param>
        /// <returns>Среднее геометрическое</returns>
        public static object GeoMean(DataColumn column, object defaultValue)
        {
            object result = defaultValue;
            int valuesCount = 0;
            foreach (DataRow row in column.Table.Rows)
            {
                double value;
                if (Double.TryParse(row[column].ToString(), out value) && (value != 0))
                {
                    result = valuesCount == 0 ? value : value * Convert.ToDouble(result);
                    ++valuesCount;
                }
            }
            if (valuesCount == 0)
                return defaultValue;
            else
                return Math.Pow(Convert.ToDouble(result), 1.0 / valuesCount);
        }

        #endregion

        #region Базовые и не очень арифметичские операции (в основном для вычислений в таблицах)

        // Пока все сделаю для Double, потому как они наиболее часто используются (если что, можно округлить - int получится)

        // Модуль
        public static object Abs(object value)
        {
            double v;
            if (Double.TryParse(value.ToString(), out v))
                return Math.Abs(v);
            else
                return DBNull.Value;
        }

		// Сложение
		public static object Plus(object firstValue, object secondValue, object defaultValue, CalcMode mode)
		{
			double v1, v2;
			bool firstIsDouble = Double.TryParse(firstValue.ToString(), out v1);
			bool secondIsDouble = Double.TryParse(secondValue.ToString(), out v2);
			if (mode == CalcMode.CalcIfTwo)
			{
				if (firstIsDouble && secondIsDouble)
					return v1 + v2;
				else
					return defaultValue;
			}
			else if (mode == CalcMode.CalcIfOne)
			{
				if (firstIsDouble && secondIsDouble)
					return v1 + v2;
				else if (firstIsDouble || secondIsDouble)
					return firstIsDouble ? v1 : v2;
				else
					return defaultValue;
			}
			return defaultValue;
		}

		public static object Plus(object firstValue, object secondValue)
		{
			return Plus(firstValue, secondValue, DBNull.Value, CalcMode.CalcIfTwo);
		}

		// Вычитание
		public static object Minus(object firstValue, object secondValue, object defaultValue, CalcMode mode)
		{
			double v1, v2;
			bool firstIsDouble = Double.TryParse(firstValue.ToString(), out v1);
			bool secondIsDouble = Double.TryParse(secondValue.ToString(), out v2);
			if (mode == CalcMode.CalcIfTwo)
			{
				if (firstIsDouble && secondIsDouble)
					return v1 - v2;
				else
					return defaultValue;
			}
			else if (mode == CalcMode.CalcIfOne)
			{
				if (firstIsDouble && secondIsDouble)
					return v1 - v2;
				else if (firstIsDouble || secondIsDouble)
					return firstIsDouble ? v1 : -v2;
				else
					return defaultValue;
			}
			return defaultValue;
		}

		public static object Minus(object firstValue, object secondValue)
		{
			return Minus(firstValue, secondValue, DBNull.Value, CalcMode.CalcIfTwo);
		}

		// Умножение
		public static object Mult(object firstValue, object secondValue, object defaultValue, CalcMode mode)
		{
			double v1, v2;
			bool firstIsDouble = Double.TryParse(firstValue.ToString(), out v1);
			bool secondIsDouble = Double.TryParse(secondValue.ToString(), out v2);
			if (mode == CalcMode.CalcIfTwo)
			{
				if (firstIsDouble && secondIsDouble)
					return v1 * v2;
				else
					return defaultValue;
			}
			else if (mode == CalcMode.CalcIfOne)
			{
				if (firstIsDouble && secondIsDouble)
					return v1 * v2;
				else if (firstIsDouble || secondIsDouble)
					return firstIsDouble ? v1 : v2;
				else
					return defaultValue;
			}
			return defaultValue;
		}

		public static object Mult(object firstValue, object secondValue)
		{
			return Mult(firstValue, secondValue, DBNull.Value, CalcMode.CalcIfTwo);
		}

		// Деление
		public static object Div(object firstValue, object secondValue, object defaultValue, CalcMode mode)
		{
			double v1, v2;
			bool firstIsDouble = Double.TryParse(firstValue.ToString(), out v1);
			bool secondIsDouble = Double.TryParse(secondValue.ToString(), out v2);
			if (mode == CalcMode.CalcIfTwo)
			{
				if (firstIsDouble && secondIsDouble && v2 != 0)
					return v1 / v2;
				else
					return defaultValue;
			}
			else if (mode == CalcMode.CalcIfOne)
			{
				if (firstIsDouble && secondIsDouble && v2 != 0)
					return v1 / v2;
				else if (firstIsDouble)
					return v1;
				else if (secondIsDouble && v2 != 0)
					return 1.0 / v2;
				else
					return defaultValue;
			}
			return defaultValue;
		}

		public static object Div(object firstValue, object secondValue)
		{
			return Grown(firstValue, secondValue, DBNull.Value, CalcMode.CalcIfTwo);
		}

		// Прирост
		public static object Grown(object firstValue, object secondValue, object defaultValue, CalcMode mode)
		{
            double v1, v2;
            bool firstIsDouble = Double.TryParse(firstValue.ToString(), out v1);
            bool secondIsDouble = Double.TryParse(secondValue.ToString(), out v2);
            if (mode == CalcMode.CalcIfTwo)
            {
                if (firstIsDouble && secondIsDouble && v2 != 0)
                    return v1 / v2 - 1;
                else
                    return defaultValue;
            }
            else if (mode == CalcMode.CalcIfOne)
            {
                if (firstIsDouble && secondIsDouble && v2 != 0)
                    return v1 / v2 - 1;
                else if (firstIsDouble)
                    return v1;
                else if (secondIsDouble && v2 != 0)
                    return 1.0 / v2 - 1;
                else
                    return defaultValue;
            }
            return defaultValue;
        }

		public static object Grown(object firstValue, object secondValue)
		{
            return Grown(firstValue, secondValue, DBNull.Value, CalcMode.CalcIfTwo);
		}

		#endregion

		#region Операции над колонками таблиц

		public static void RowPlus(DataTable table, int firstColumn, int secondColumn, int destColumn)
		{
			foreach (DataRow row in table.Rows)
				row[destColumn] = Plus(row[firstColumn], row[secondColumn]);
		}

		public static void RowMinus(DataTable table, int firstColumn, int secondColumn, int destColumn)
		{
			foreach (DataRow row in table.Rows)
				row[destColumn] = Minus(row[firstColumn], row[secondColumn]);
		}

		public static void RowMult(DataTable table, int firstColumn, int secondColumn, int destColumn)
		{
			foreach (DataRow row in table.Rows)
				row[destColumn] = Mult(row[firstColumn], row[secondColumn]);
		}

		public static void RowDiv(DataTable table, int firstColumn, int secondColumn, int destColumn)
		{
			foreach (DataRow row in table.Rows)
				row[destColumn] = Div(row[firstColumn], row[secondColumn]);
		}

		public static void RowGrown(DataTable table, int firstColumn, int secondColumn, int destColumn)
		{
			foreach (DataRow row in table.Rows)
				row[destColumn] = Grown(row[firstColumn], row[secondColumn]);
		}

		#endregion

        #region Полезные проверки

        /// <summary>
        /// Проверяет, является ли значение отрицательным числом
        /// </summary>
        /// <param name="value">Значение</param>
        /// <returns>Если число отрицательное - true, если не число или не отрицательное, то false</returns>
        public static bool SubZero(object value)
        {
            double v;
            return value != null && Double.TryParse(value.ToString(), out v) && v < 0;
        }

        /// <summary>
        /// Проверяет, является ли значение положительным числом
        /// </summary>
        /// <param name="value">Значение</param>
        /// <returns>Если число положительное - true, если не число или не положительное, то false</returns>
        public static bool AboveZero(object value)
        {
            double v;
            return value != null && Double.TryParse(value.ToString(), out v) && v > 0;
        }

        public static bool IsDouble(object value)
        {
            double v;
            return value != null && Double.TryParse(value.ToString(), out v);
        }

        /// <summary>
        /// Проверяет, является ли значение отрицательным числом
        /// </summary>
        /// <param name="value">Значение</param>
        /// <returns>Если число отрицательное - true, если не число или не отрицательное, то false</returns>
        public static bool SubZero(string value)
        {
            double v;
            return value != null && Double.TryParse(value, out v) && v < 0;
        }

        /// <summary>
        /// Проверяет, является ли значение положительным числом
        /// </summary>
        /// <param name="value">Значение</param>
        /// <returns>Если число положительное - true, если не число или не положительное, то false</returns>
        public static bool AboveZero(string value)
        {
            double v;
            return value != null && Double.TryParse(value, out v) && v > 0;
        }

        public static bool IsDouble(string value)
        {
            double v;
            return value != null && Double.TryParse(value, out v);
        }

        #endregion


    }

	public enum CalcMode
	{
		CalcIfOne,
		CalcIfTwo
	}
    
    public class ReverseComparer : IComparer<double>
    {
        public int Compare(double x, double y)
        {
            // Compare y and x in reverse order.
            return y.CompareTo(x);
        }
    }


}
