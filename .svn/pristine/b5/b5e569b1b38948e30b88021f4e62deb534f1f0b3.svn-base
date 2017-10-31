using System;
using System.Data;

namespace Krista.FM.Server.Dashboards.SkkSupport
{
	public class ParamDate
	{
		//        : prev : last
		// unique :  0 1 : 0 2 
		// month  :  1 1 : 1 2
		// year   :  2 1 : 2 2
		readonly DataTable dtPeriod = new DataTable();
		
		/// <summary>
		/// Работа с параметрами-датами
		/// </summary>
		/// <param name="queryName">идентификатор запрос</param>
		public ParamDate(string queryName)
		{
			dtPeriod = (new Query(queryName)).GetCommonDataTable();
		}

		/// <summary>
		/// Получает первый год
		/// </summary>
		/// <returns></returns>
		public int GetFirstYear()
		{
			return SKKHelper.defaultYear;
		}

		/// <summary>
		/// Получает последний год
		/// </summary>
		/// <returns></returns>
		public int GetLastYear()
		{
			int value;
			if (!Int32.TryParse(dtPeriod.Rows[2][2].ToString(), out value))
			{
				value = SKKHelper.defaultYear;
			}
			return value;
		}

		/// <summary>
		/// Получает предпоследний год
		/// </summary>
		/// <returns></returns>
		public int GetPrevYear()
		{
			int value;
			if (!Int32.TryParse(dtPeriod.Rows[2][1].ToString(), out value))
			{
				value = SKKHelper.defaultYear;
			}
			return value;
		}

		/// <summary>
		/// Получает последний месяц
		/// </summary>
		/// <returns></returns>
		public string GetLastMonth()
		{
			return 
				dtPeriod.Rows[1][2] != null
					?
				dtPeriod.Rows[1][2].ToString()
					:
				SKKHelper.defaultMonth;
		}

		/// <summary>
		/// Получает предпоследний месяц
		/// </summary>
		/// <returns></returns>
		public string GetPrevMonth()
		{
			return
				dtPeriod.Rows[1][1] != null
					?
				dtPeriod.Rows[1][1].ToString()
					:
				SKKHelper.defaultMonth;
		}

	}
}
