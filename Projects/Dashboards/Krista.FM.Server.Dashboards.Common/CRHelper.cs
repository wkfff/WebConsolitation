using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Dundas.Maps.WebControl;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.WebUI.UltraWebGrid;
using Path = System.IO.Path;
using UltraGridSpace = Infragistics.WebUI.UltraWebGrid;

namespace Krista.FM.Server.Dashboards.Common
{
	public enum CompareAction
	{
		Greater = 0,
		GreaterOrEqual = 1,
		Less = 2,
		LessOrEqual = 3,
		Equal = 4,
		NotEqual = 5
	}


	/// <summary>
	/// Статический класс утилит произвольных отчетов
	/// </summary>
	public static class CRHelper
	{
		public static bool CompareTwo(double first, double second, CompareAction action)
		{
			try
			{
				if (action == CompareAction.Greater) return (first > second);
				if (action == CompareAction.GreaterOrEqual) return (first >= second);
				if (action == CompareAction.Less) return (first < second);
				if (action == CompareAction.LessOrEqual) return (first <= second);
				if (action == CompareAction.Equal) return (first == second);
				if (action == CompareAction.NotEqual) return (first != second);
			}
			catch
			{
			}
			return false;
		}


		//Путь к корню сайта
		public static string BasePath = HttpContext.Current.Server.MapPath(HttpContext.Current.Request.ApplicationPath);

		#region DateHelper

		/// <summary>
		/// Дурацкая функция, наверняка есть стандартная.
		/// По номеру месяца возвращает его русское название
		/// </summary>
		public static string RusMonth(int monthNum)
		{
			switch (monthNum)
			{
				case 1:
					return "январь";
				case 2:
					return "февраль";
				case 3:
					return "март";
				case 4:
					return "апрель";
				case 5:
					return "май";
				case 6:
					return "июнь";
				case 7:
					return "июль";
				case 8:
					return "август";
				case 9:
					return "сентябрь";
				case 10:
					return "октябрь";
				case 11:
					return "ноябрь";
				case 12:
					return "декабрь";
				default:
					return "январь";
			}
		}

		/// <summary>
		/// Дурацкая функция, наверняка есть стандартная.
		/// По номеру месяца возвращает его русское название
		/// </summary>
		public static string EnMonth(int monthNum)
		{
			switch (monthNum)
			{
				case 1:
					return "January";
				case 2:
					return "February";
				case 3:
					return "March";
				case 4:
					return "April";
				case 5:
					return "May";
				case 6:
					return "June";
				case 7:
					return "July";
				case 8:
					return "August";
				case 9:
					return "September";
				case 10:
					return "October";
				case 11:
					return "November";
				case 12:
					return "December";
				default:
					return "January";
			}
		}

		public static int MonthNum(string monthCaption)
		{
			string s = monthCaption.ToLower();

			if (s == "январь" || s == "января") return 1;
			if (s == "февраль" || s == "февраля") return 2;
			if (s == "март" || s == "марта") return 3;
			if (s == "апрель" || s == "апреля") return 4;
			if (s == "май" || s == "мая") return 5;
			if (s == "июнь" || s == "июня") return 6;
			if (s == "июль" || s == "июля") return 7;
			if (s == "август" || s == "августа") return 8;
			if (s == "сентябрь" || s == "сентября") return 9;
			if (s == "октябрь" || s == "октября") return 10;
			if (s == "ноябрь" || s == "ноября") return 11;
			if (s == "декабрь" || s == "декабря") return 12;
			return 1;
		}

		public static bool IsMonthCaption(string monthCaption)
		{
			string s = monthCaption.ToLower();
			return MonthNum(monthCaption) != 1 || s == "январь" || s == "января";
		}

		public static string RusMonthGenitive(int monthNum)
		{
			switch (monthNum)
			{
				case 1:
					return "января";
				case 2:
					return "февраля";
				case 3:
					return "марта";
				case 4:
					return "апреля";
				case 5:
					return "мая";
				case 6:
					return "июня";
				case 7:
					return "июля";
				case 8:
					return "августа";
				case 9:
					return "сентября";
				case 10:
					return "октября";
				case 11:
					return "ноября";
				case 12:
					return "декабря";
				default:
					return "января";
			}
		}

		public static string RusManyMonthGenitive(int monthCount)
		{
			switch (monthCount)
			{
				case 1:
					{
						return "месяц";
					}
				case 2:
				case 3:
				case 4:
					{
						return "месяца";
					}
				default:
					{
						return "месяцев";
					}
			}
		}

		public static string RusMonthDat(int monthNum)
		{
			switch (monthNum)
			{
				case 1:
					return "январю";
				case 2:
					return "февралю";
				case 3:
					return "марту";
				case 4:
					return "апрелю";
				case 5:
					return "маю";
				case 6:
					return "июню";
				case 7:
					return "июлю";
				case 8:
					return "августу";
				case 9:
					return "сентябрю";
				case 10:
					return "октябрю";
				case 11:
					return "ноябрю";
				case 12:
					return "декабрю";
				default:
					return "январю";
			}
		}

		/// <summary>
		/// По номеру месяца возвращает его русское название в творительном падеже
		/// Пример: в сравнении с _октябрем_
		/// </summary>
		public static string RusMonthTvorit(int monthNum)
		{
			switch (monthNum)
			{
				case 1:
					return "январем";
				case 2:
					return "февралем";
				case 3:
					return "мартом";
				case 4:
					return "апрелем";
				case 5:
					return "маем";
				case 6:
					return "июнем";
				case 7:
					return "июлем";
				case 8:
					return "августом";
				case 9:
					return "сентябрем";
				case 10:
					return "октябрем";
				case 11:
					return "ноябрем";
				case 12:
					return "декабрем";
				default:
					return "январем";
			}
		}

		public static string RusMonthAblative(int monthNum)
		{
			switch (monthNum)
			{
				case 1:
					return "января";
				case 2:
					return "февраля";
				case 3:
					return "марта";
				case 4:
					return "апреля";
				case 5:
					return "мая";
				case 6:
					return "июня";
				case 7:
					return "июля";
				case 8:
					return "августа";
				case 9:
					return "сентября";
				case 10:
					return "октября";
				case 11:
					return "ноября";
				case 12:
					return "декабря";
				default:
					return "января";
			}
		}

		public static string RusMonthPrepositional(int monthNum)
		{
			switch (monthNum)
			{
				case 1:
					return "январе";
				case 2:
					return "феврале";
				case 3:
					return "марте";
				case 4:
					return "апреле";
				case 5:
					return "мае";
				case 6:
					return "июне";
				case 7:
					return "июле";
				case 8:
					return "августе";
				case 9:
					return "сентябре";
				case 10:
					return "октябре";
				case 11:
					return "ноябре";
				case 12:
					return "декабре";
				default:
					return "январе";
			}
		}

		public static int QuarterNumByMonthNum(int MonthNum)
		{
			//странно работает
			//return (int)Math.Ceiling((double)(MothNum / 3));
			if (MonthNum <= 3) return 1;
			if (MonthNum <= 6) return 2;
			if (MonthNum <= 9) return 3;
			return 4;
		}

		/// <summary>
		/// Возвращает номер месяца в квартале
		/// </summary>
		/// <param name="MonthNum"></param>
		/// <returns></returns>
		public static int MonthNumInQuarter(int MonthNum)
		{
			if (MonthNum <= 3) return MonthNum;
			if (MonthNum <= 6) return MonthNum - 3;
			if (MonthNum <= 9) return MonthNum - 6;
			return MonthNum - 9;
		}

		public static int HalfYearNumByMonthNum(int MonthNum)
		{
			return (MonthNum < 7) ? 1 : 2;
		}

		public static int HalfYearNumByQuarterNum(int quarterName)
		{
			return (quarterName < 3) ? 1 : 2;
		}

		public static int QuarterLastMonth(int quarterNum)
		{
			return quarterNum * 3;
		}

		public static int MonthLastDay(int monthNum)
		{
			switch (monthNum)
			{
				case 1:
				case 3:
				case 5:
				case 7:
				case 8:
				case 10:
				case 12:
					return 31;
				case 2:
					return 28;
				case 4:
				case 6:
				case 9:
				case 11:
					return 30;
				default:
					return 31;
			}
		}

		public static int QuarterLastDay(int quarterNum)
		{
			switch (quarterNum)
			{
				case 1:
				case 4:
					return 31;
				default:
					return 30;
			}
		}

		/// <summary>
		/// Юник-нэйм измерения типа "период" по переданной дате
		/// </summary>
		/// <param name="staticHeader">Начало юник-нэйма: измерение.иерархия.олл-мембер</param>
		/// <param name="date">дата</param>
		/// <param name="detailLevel">
		/// уровень детализации: 
		/// 0-все, 1-год, 2-полугодие, 3-квартал, 4-месяц, 5-день
		/// </param>
		/// <returns></returns>
		public static string PeriodMemberUName(string staticHeader, DateTime date, int detailLevel)
		{
			string result = staticHeader;

			int quarter = QuarterNumByMonthNum(date.Month);
			int halfyear = HalfYearNumByMonthNum(date.Month);

			if (detailLevel > 0) result += string.Format(".[{0}]", date.Year);
			if (detailLevel > 1) result += string.Format(".[Полугодие {0}]", halfyear);
			if (detailLevel > 2) result += string.Format(".[Квартал {0}]", quarter);
			if (detailLevel > 3) result += string.Format(".[{0}]", RusMonth(date.Month));
			if (detailLevel > 4) result += string.Format(".[{0}]", date.Day);

			return result;
		}

		/// <summary>
		/// Дата по юник-нэйму измерения типа "период"
		/// </summary>
		public static DateTime DateByPeriodMemberUName(string periodMemberUName, int staticHeaderLength)
		{
			if (String.IsNullOrEmpty(periodMemberUName))
			{
				return DateTime.MinValue;
			}
			int year = 2008;
			int month = 1;
			int day = 1;

			string[] uName = periodMemberUName.Split('.');

			int pos = staticHeaderLength;
			DateTime result;
			try
			{
				if (pos < uName.Length)
				{
					year = Convert.ToInt32(uName[pos].Trim('[').Trim(']'));
				}
				if (pos + 3 < uName.Length)
				{
					month = MonthNum(uName[pos + 3].Trim('[').Trim(']'));
				}
				if (pos + 4 < uName.Length)
				{
					day = Convert.ToInt32(uName[pos + 4].Trim('[').Trim(']'));
				}

				result = new DateTime(year, month, day);
			}
			catch (Exception e)
			{
				return DateTime.MinValue;
			}
			return result;
		}

		/// <summary>
		/// Возвращает уровнь детализации для мембера период, день_фо, который в общем случае
		/// имеет такой формат
		/// [Период].[День_ФО].[Данные всех периодов].[год].[Полугодие].[Квартал].[Месяц].[День]
		/// </summary>
		public static int PeriodDayFoDetailLevel(string uname)
		{
			string[] parts = uname.Split('.');
			return parts.Length - 4;
		}

		/// <summary>
		/// Возвращает дату по мемберу период день_фо, который в общем случае
		/// имеет такой формат
		/// [Период].[День_ФО].[Данные всех периодов].[год].[Полугодие].[Квартал].[Месяц].[День]
		/// </summary>
		public static DateTime PeriodDayFoDate(string uname)
		{
#warning Лохматая функция. Модифицировать

            string[] parts = uname.Split('.');
			int dlavel = PeriodDayFoDetailLevel(uname);
			int year, month, day;

			if (dlavel < 4) //день не указан 
			{
				day = 1;
			}
            else //есть конкретный день
            {
                parts[7] = parts[7].Remove(0, 1);
                parts[7] = parts[7].Remove(parts[7].Length - 1, 1);

                int value;
                if (Int32.TryParse(parts[7], out value))
                {
                    day = value;
                }
                else
                {
                    day = 1;
                }
            }


			switch (dlavel)
			{
				case 4:
				case 3: //месяц указан явно
					parts[6] = parts[6].Remove(0, 1);
					parts[6] = parts[6].Remove(parts[6].Length - 1, 1);
					month = MonthNum(parts[6]);
					break;
				case 2: //квартал (берем его первый месяц)
					parts[5] = parts[5].Remove(0, 9);
					parts[5] = parts[5].Remove(parts[5].Length - 1, 1);
					month = int.Parse(parts[5]) * 3 - 2;
					break;
				case 1: //полугодие (берем первый месяц
					parts[4] = parts[4].Remove(0, 11);
					parts[4] = parts[4].Remove(parts[4].Length - 1, 1);
					month = int.Parse(parts[4]) * 6 - 5;
					break;
				default: //значит только год
					month = 1;
					break;
			}


			parts[3] = parts[3].Remove(0, 1);
			parts[3] = parts[3].Remove(parts[3].Length - 1, 1);
			year = int.Parse(parts[3]);


			return new DateTime(year, month, day);
		}


		/// <summary>
		/// Человеческое описание периода
		/// </summary>
		public static string PeriodDescr(DateTime date, int detailLevel)
		{
			switch (detailLevel)
			{
				case 1: //год
					return string.Format("{0} год", date.Year);
				case 2: //полугодие
					return string.Format("{0} полугодие {1} года",
										 HalfYearNumByMonthNum(date.Month), date.Year);
				case 3: //квартал
					return string.Format("{0} квартал {1} года",
										 QuarterNumByMonthNum(date.Month), date.Year);
				case 4: //месяц
					return string.Format("{0} {1} года", RusMonth(date.Month), date.Year);
				case 5: //день
					//return string.Empty;
					return string.Format("{0} {1} {2} г.",
										 date.Day, RusMonthGenitive(date.Month), date.Year);

				default:
					return string.Empty;
			}
		}

		#endregion

		//форматирование числовой ячейки грида
		public static void FormatNumberColumn(UltraGridColumn col, string cellFormat)
		{
			if (col != null)
			{
				if (!String.IsNullOrEmpty(cellFormat))
					col.Format = cellFormat;
				col.CellStyle.HorizontalAlign = HorizontalAlign.Right;
				col.CellStyle.Padding.Right = 10;
			}
		}

		/// <summary>
		/// Условная раскраска ячеек грида
		/// </summary>
		public static void BrushGridCells(UltraWebGrid grid,
										  int colnum, double etalonValue, CompareAction action, Color targetColor)
		{
			if (grid == null) return;
			if (grid.Columns.Count <= colnum) return;
			foreach (UltraGridRow row in grid.Rows)
			{
				try
				{
					double curVal;
					curVal = double.Parse(row.Cells[colnum].Value.ToString());
					if (CompareTwo(curVal, etalonValue, action))
						row.Cells[colnum].Style.BackColor = targetColor;
				}
				catch
				{
				}
			}
		}

		/// <summary>
		/// Установка условного рисунка для ячейки грида
		/// </summary>
		public static void SetConditionImageGridCells(UltraWebGrid grid,
													  int colnum, double etalonValue, CompareAction action,
													  string trueImageUrl, string falseImageUrl)
		{
			if (grid == null) return;
			if (grid.Columns.Count <= colnum) return;
			foreach (UltraGridRow row in grid.Rows)
			{
				try
				{
					double curVal;
					curVal = double.Parse(row.Cells[colnum].Value.ToString());
					if (CompareTwo(curVal, etalonValue, action))
					{
						row.Cells[colnum].Style.BackgroundImage = trueImageUrl;
					}
					else
					{
						row.Cells[colnum].Style.BackgroundImage = falseImageUrl;
					}
					row.Cells[colnum].Style.CustomRules =
						"background-repeat: no-repeat; background-position: left; margin-left: 5px";
				}
				catch
				{
				}
			}
		}

		/// <summary>
		/// Установка условного рисунка для ячейки грида
		/// </summary>
		public static void SetConditionImageGridCell(UltraGridCell cell,
													 double etalonValue, CompareAction action, string trueCss,
													 string falseCss)
		{
			if (cell == null) return;
			if (cell.Value == null) return;
			try
			{
				double curVal;
				curVal = double.Parse(cell.Value.ToString());
				if (CompareTwo(curVal, etalonValue, action))
				{
					cell.Style.CssClass = trueCss;
				}
				else
				{
					cell.Style.CssClass = falseCss;
				}
			}
			catch
			{
			}
		}

		#region Работа с логами

		/// <summary>
		/// Сохранить текст в лог
		/// </summary>
		private static void SaveToLog(string LogName, string content)
		{
			try
			{
				File.AppendAllText(
					HttpContext.Current.Server.MapPath("~/Logs/") + LogName,
					string.Format("{0}\r\n{1}\r\n{2}\r\n", DateTime.Now.ToString("dd/MM/yyyy_HH.mm.ss"), content,
								  CustomReportConst.logSeparator),
					Encoding.GetEncoding(1251));
			}
			catch
			{
			}
		}

		/// <summary>
		/// Упаковка инормации об ошибке для вывода в лог
		/// </summary>
		public static string GetExceptionInfo(Exception e)
		{
			return string.Format("{0}\r\n{1}\r\n{2}", e.Source, e.Message, e.StackTrace);
		}

		/// <summary>
		/// Упаковка инормации об ошибке для вывода в лог
		/// </summary>
		public static string GetExceptionInfo(string EMessage, Exception e)
		{
			return string.Format("{0}\r\n{1}\r\n{2}\r\n{3}", EMessage, e.Source, e.Message, e.StackTrace);
		}

		/// <summary>
		/// Сохранить в лог запросов
		/// </summary>
		public static void SaveToQueryLog(string QText)
		{
			SaveToLog(CustomReportConst.queryLogFileName, QText);
		}

		/// <summary>
		/// Сохранить в лог ошибок
		/// </summary>
		public static void SaveToErrorLog(string content)
		{
			SaveToLog(CustomReportConst.crashLogFileName, content);
		}

		/// <summary>
		/// Сохранить в лог пользователей
		/// </summary>
		public static void SaveToUserLog(string content)
		{
			SaveToLog(CustomReportConst.userLogFileName, content);
		}

		/// <summary>
		/// Сохранить в лог user-agents
		/// </summary>
		public static void SaveToUserAgentLog(string content)
		{
			SaveToLog(CustomReportConst.userAgentLogFileName, content);
		}

		#endregion

		/// <summary>
		/// Преобразует строку в другую кодировку.
		/// </summary>
		/// <param name="value">Исходная строка</param>
		/// <param name="sourceEncoding">Исходная кодировка.</param>
		/// <param name="targetEncoding">Целевая кодировка.</param>
		/// <returns>Перекодированная строка.</returns>
		public static string ConvertEncoding(string value, Encoding sourceEncoding, Encoding targetEncoding)
		{
			Decoder decoder = sourceEncoding.GetDecoder();
			byte[] source = targetEncoding.GetBytes(value);
			int length = decoder.GetCharCount(source, 0, source.Length);
			char[] target = new char[length];
			decoder.GetChars(source, 0, source.Length, target, 0);
			return new string(target);
		}

		/// <summary>
		/// Ищет строку грида по значению и устанавливает ее активной.
		/// </summary>
		/// <param name="grid">Грид.</param>
		/// <param name="patternValue">Значение в строке.</param>
		/// <param name="columnNumber">Номер колонки со значением.</param>
		/// <returns>Сыылка на найденную строку, или первую в случае неудачи.</returns>
		public static UltraGridRow FindGridRow(UltraWebGrid grid, string patternValue, int columnNumber)
		{
			if (grid.DisplayLayout.ViewType == ViewType.Flat)
			{
				foreach (UltraGridRow r in grid.Rows)
				{
					if (r.Cells[columnNumber].Value.ToString() == patternValue)
					{
						r.Selected = true;
						grid.DisplayLayout.ActiveRow = r;
						return r;
					}
				}
				return grid.Rows[0];
			}
			else
			{
				foreach (UltraGridBand b in grid.Bands)
				{
					foreach (UltraGridRow r in b.Grid.Rows)
					{
						if (r.Cells[columnNumber].Value.ToString() == patternValue)
						{
							r.Selected = true;
							grid.DisplayLayout.ActiveRow = r;
							return r;
						}

						if (!r.Expanded)
						{
							r.Expand(true);
						}

						foreach (UltraGridRow r2 in r.Rows)
						{
							if (r2.Cells[columnNumber].Value.ToString() == patternValue)
							{
								r2.Selected = true;
								grid.DisplayLayout.ActiveRow = r2;
								return r2;
							}
						}
						if (r.Expanded)
						{
							r.Collapse(true);
						}
					}
				}
			}

			if (grid.Rows.Count > 1)
			{
				grid.Rows[0].Selected = true;
				grid.DisplayLayout.ActiveRow = grid.Rows[0];
				return grid.Rows[0];
			}
			return null;
		}

		/// <summary>
		/// Ищет строку грида по значению и устанавливает ее активной.
		/// </summary>
		/// <param name="grid">Грид.</param>
		/// <param name="patternValue">Значение в строке.</param>
		/// <param name="columnNumber">Номер колонки со значением.</param>
		/// <param name="defaultRowNumber">Номер строки,которую нужно выделить, если нужная строка не найдена</param>
		/// <returns>Сыылка на найденную строку, или первую в случае неудачи.</returns>
		public static UltraGridRow FindGridRow(UltraWebGrid grid, string patternValue, int columnNumber,
											   int defaultRowNumber)
		{
			if (grid.DisplayLayout.ViewType == ViewType.Flat)
			{
				foreach (UltraGridRow r in grid.Rows)
				{
					if (r.Cells[columnNumber].Value != null && r.Cells[columnNumber].Value.ToString() == patternValue)
					{
						r.Selected = true;
						grid.DisplayLayout.ActiveRow = r;
						return r;
					}
				}
				return grid.Rows[defaultRowNumber];
			}
			else
			{
				foreach (UltraGridBand b in grid.Bands)
				{
					foreach (UltraGridRow r in b.Grid.Rows)
					{
						if (r.Cells[columnNumber].Value != null && r.Cells[columnNumber].Value.ToString() == patternValue)
						{
							r.Selected = true;
							grid.DisplayLayout.ActiveRow = r;
							return r;
						}

						if (!r.Expanded)
						{
							r.Expand(true);
						}

						foreach (UltraGridRow r2 in r.Rows)
						{
							if (r2.Cells[columnNumber].Value.ToString() == patternValue)
							{
								r2.Selected = true;
								grid.DisplayLayout.ActiveRow = r2;
								return r2;
							}
						}
						if (r.Expanded)
						{
							r.Collapse(true);
						}
					}
				}
			}

			if (grid.Rows.Count > 1)
			{
				grid.Rows[defaultRowNumber].Selected = true;
				grid.DisplayLayout.ActiveRow = grid.Rows[defaultRowNumber];
				//иногда съедается верхняя строчка, поэтому устанавливаем сколлбар в верхнее/левое положение
				grid.DisplayLayout.ScrollTop = 0;
				grid.DisplayLayout.ScrollLeft = 0;
				return grid.Rows[defaultRowNumber];
			}
			return null;
		}

		/// <summary>
		/// Установка наименования и хинта для заголовка грида
		/// </summary>
		/// <param name="grid">грид</param>
		/// <param name="bandIndex">номер банда</param>
		/// <param name="columnIndex">номер колонки</param>
		/// <param name="caption">заголовок</param>
		/// <param name="tooptip">хинт</param>
		public static void SetHeaderCaption(UltraWebGrid grid, int bandIndex, int columnIndex, string caption,
											string tooptip)
		{
			if (grid.Bands[bandIndex] != null && grid.Bands[bandIndex].Columns[columnIndex] != null)
			{
				grid.Bands[bandIndex].Columns[columnIndex].Header.Caption = caption;
				grid.Bands[bandIndex].Columns[columnIndex].Header.Title = tooptip;
			}
		}

		/// <summary>
		/// Добавление иерархического заголовка
		/// </summary>
		/// <param name="grid">грид</param>
		/// <param name="bandIndex">номер банда</param>
		/// <param name="caption">заголовок</param>
		/// <param name="originX">номер колонки по горизонтали</param>
		/// <param name="originY">номер колонки по вертикали</param>
		/// <param name="spanX">слияние колонок по горизонтали</param>
		/// <param name="spanY">слияние колонок по вертикали</param>
		/// <returns>Добавленный заголовок</returns>
		public static ColumnHeader AddHierarchyHeader(UltraWebGrid grid, int bandIndex, string caption, int originX,
													  int originY, int spanX, int spanY)
		{
			ColumnHeader ch = new ColumnHeader(true);
			ch.Caption = caption;
			ch.Style.Wrap = true;
			ch.RowLayoutColumnInfo.OriginX = originX;
			ch.RowLayoutColumnInfo.OriginY = originY;
			ch.RowLayoutColumnInfo.SpanX = spanX;
			ch.RowLayoutColumnInfo.SpanY = spanY;
			ch.Style.Padding.Top = 1;
			ch.Style.Padding.Bottom = 1;
			ch.Style.Height = Unit.Empty;
			ch.Style.VerticalAlign = VerticalAlign.Middle;
			grid.Bands[bandIndex].HeaderLayout.Add(ch);

			return ch;
		}

		/// <summary>
		/// Выставляет цветовой схеме диаграммы рандомные цвета
		/// </summary>
		/// <param name="chart">диаграмма</param>
		/// <param name="colorCount">количество цветов</param>
		/// <param name="rowWise">группировка по рядам</param>
		public static void FillCustomColorModel(UltraChart chart, int colorCount, bool rowWise)
		{
			FillCustomColorModelBase(chart, colorCount, rowWise, 0, 255);
		}

		/// <summary>
		/// Выставляет цветовой схеме диаграммы осветленные рандомные цвета
		/// </summary>
		/// <param name="chart">диаграмма</param>
		/// <param name="colorCount">количество цветов</param>
		/// <param name="rowWise">группировка по рядам</param>
		public static void FillCustomColorModelLight(UltraChart chart, int colorCount, bool rowWise)
		{
			FillCustomColorModelBase(chart, colorCount, rowWise, 100, 255);
		}

		/// <summary>
		/// Выставляет цветовой схеме диаграммы рандомные цвета
		/// </summary>
		/// <param name="chart">диаграмма</param>
		/// <param name="colorCount">количество цветов</param>
		/// <param name="rowWise">группировка по рядам</param>
		/// <param name="minColor">нижняя граница составляющих цвета</param>
		/// <param name="maxColor">верхняя граница составляющих цвета</param>
		public static void FillCustomColorModelBase(UltraChart chart, int colorCount, bool rowWise, int minColor, int maxColor)
		{
			Random rand = new Random(Environment.TickCount);
			chart.ColorModel.ModelStyle = ColorModels.CustomSkin;
			chart.ColorModel.Skin.ApplyRowWise = rowWise;
			chart.ColorModel.Skin.PEs.Clear();
			for (int i = 0; i < colorCount; i++)
			{
				PaintElement pe = new PaintElement();
				pe.Fill = Color.FromArgb(rand.Next(minColor, maxColor), rand.Next(minColor, maxColor), rand.Next(minColor, maxColor));
				chart.ColorModel.Skin.PEs.Add(pe);
			}
		}

		/// <summary>
		/// Получение рандомного цвета
		/// </summary>
		/// <returns></returns>
		public static Color GetRandomColor()
		{
			Random rand = new Random(Environment.TickCount);
			return Color.FromArgb(rand.Next(255), rand.Next(255), rand.Next(255));
		}

		/// <summary>
		/// Получение цвета темнее
		/// </summary>
		/// <param name="color">исходный цвет</param>
		/// <param name="k">коэффициент</param>
		/// <returns></returns>
		public static Color GetDarkColor(Color color, int k)
		{
			byte r = (color.R > k) ? (byte)(color.R - k) : (byte)(0);
			byte g = (color.G > k) ? (byte)(color.G - k) : (byte)(0);
			byte b = (color.B > k) ? (byte)(color.B - k) : (byte)(0);

			return Color.FromArgb(r, g, b);
		}

		/// <summary>
		/// Копирование цветовой схемы диаграммы
		/// </summary>
		/// <param name="sourceChart">исходная диаграмма</param>
		/// <param name="destChart">настраивамая диаграмма</param>
		public static void CopyCustomColorModel(UltraChart sourceChart, UltraChart destChart)
		{
			CopyCustomColorModelBase(sourceChart, destChart, false);
		}

		/// <summary>
		/// Копирование цветовой схемы диаграммы
		/// </summary>
		/// <param name="sourceChart">исходная диаграмма</param>
		/// <param name="destChart">настраивамая диаграмма</param>
		/// <param name="copyRowWise">копировать ли параметр ApplyRowWise</param>
		public static void CopyCustomColorModelBase(UltraChart sourceChart, UltraChart destChart, bool copyRowWise)
		{
			if (copyRowWise)
			{
				destChart.ColorModel.Skin.ApplyRowWise = sourceChart.ColorModel.Skin.ApplyRowWise;
			}
			destChart.ColorModel.ModelStyle = sourceChart.ColorModel.ModelStyle;
			destChart.ColorModel.Skin.PEs.Clear();
			foreach (PaintElement pe in sourceChart.ColorModel.Skin.PEs)
			{
				destChart.ColorModel.Skin.PEs.Add(pe);
			}
		}

		#region Работа с композитной диаграммой

		/// <summary>
		/// Получение элемента заливки
		/// </summary>
		/// <param name="fillColor">цвет заливки</param>
		/// <returns>элемент заливки</returns>
		public static PaintElement GetFillPaintElement(Color fillColor, byte opacity)
		{
			PaintElement pe = new PaintElement();
			pe.ElementType = PaintElementType.SolidFill;
			pe.Fill = fillColor;
			pe.FillOpacity = opacity;
			return pe;
		}

		/// <summary>
		/// Получение элемента заливки градиентом
		/// </summary>
		/// <param name="fillStartColor">начальный цвет заливки</param>
		/// <param name="fillOpacity">прозрачность заливки</param>
		/// <returns>элемент заливки</returns>
		public static PaintElement GetGradientPaintElement(Color fillStartColor, byte fillOpacity)
		{
			PaintElement pe = new PaintElement();
			pe.ElementType = PaintElementType.Gradient;
			pe.Fill = fillStartColor;
			pe.FillStopColor = Color.Transparent;
			pe.FillOpacity = fillOpacity;
			pe.FillGradientStyle = GradientStyle.BackwardDiagonal;
			return pe;
		}

		/// <summary>
		/// Получение числовой серии данных
		/// </summary>
		/// <param name="index">номер серии</param>
		/// <param name="dataSource">таблица данных</param>
		/// <returns>серия данных</returns>
		public static NumericSeries GetNumericSeries(int index, object dataSource)
		{
			DataTable dataTable = (DataTable)dataSource;

			NumericSeries numericSeries = new NumericSeries();
			numericSeries.Label = dataTable.Columns[index].ColumnName;
			numericSeries.Data.LabelColumn = dataTable.Columns[0].ColumnName;
			numericSeries.Data.ValueColumn = dataTable.Columns[index].ColumnName;

			numericSeries.Data.DataSource = dataTable;
			numericSeries.DataBind();

			return numericSeries;
		}

		/// <summary>
		/// Получениесерии серии данных XY
		/// </summary>
		/// <param name="indexX">номер серии X</param>
		/// <param name="indexY">номер серии Y</param>
		/// <param name="dataSource">таблица данных</param>
		/// <returns>серия данных</returns>
		public static XYSeries GetXYSeries(int indexX, int indexY, object dataSource)
		{
			DataTable dataTable = (DataTable)dataSource;

			XYSeries xySeries = new XYSeries();
			//xySeries.Label = name;
			xySeries.Data.LabelColumn = dataTable.Columns[0].ColumnName;
			xySeries.Data.ValueXColumn = dataTable.Columns[indexX].ColumnName;
			xySeries.Data.ValueYColumn = dataTable.Columns[indexY].ColumnName;

			xySeries.Data.DataSource = dataSource;
			xySeries.DataBind();

			return xySeries;
		}


		/// <summary>
		/// Получение аннотации-стрелки по номерам рядов-категорий
		/// </summary>
		/// <param name="columnLocation">положение по категориям</param>
		/// <param name="rowLocation">положение по рядам</param>
		/// <param name="columnOffset">смещение по категориям</param>
		/// <param name="rowOffset">смещение по рядам</param>
		/// <returns>аннотация-стрелка</returns>
		public static LineAnnotation GetArrowAnnotationRowColumn(int columnLocation, int rowLocation, int columnOffset,
																 int rowOffset)
		{
			LineAnnotation annotation = new LineAnnotation();
			annotation.Style.EndStyle = LineCapStyle.ArrowAnchor;
			annotation.Thickness = 5;

			annotation.Location.Type = LocationType.RowColumn;
			annotation.Location.Column = columnLocation;
			annotation.Location.Row = rowLocation;

			annotation.OffsetMode = LocationOffsetMode.Manual;
			annotation.Offset.Type = LocationType.RowColumn;
			annotation.Offset.Column = columnOffset;
			annotation.Offset.Row = rowOffset;

			return annotation;
		}

		/// <summary>
		/// Получение аннотации-стрелки по номерам рядов-категорий
		/// </summary>
		/// <param name="columnStartValueX">положение по категориям</param>
		/// <param name="rowStartValueY">положение по рядам</param>
		/// <param name="columnEndValueX">смещение по категориям</param>
		/// <param name="rowEndValueY">смещение по рядам</param>
		/// <returns>аннотация-стрелка</returns>
		public static LineAnnotation GetLineAnnotationDataValue(double columnStartValueX, double rowStartValueY,
																double columnEndValueX, double rowEndValueY)
		{
			LineAnnotation annotation = new LineAnnotation();
			annotation.Style.EndStyle = LineCapStyle.NoAnchor;
			annotation.Thickness = 5;

			annotation.Location.Type = LocationType.DataValues;
			annotation.Location.ValueX = columnStartValueX;
			annotation.Location.ValueY = rowStartValueY;

			annotation.OffsetMode = LocationOffsetMode.Manual;
			annotation.Offset.Type = LocationType.DataValues;
			annotation.Offset.ValueX = columnEndValueX;
			annotation.Offset.ValueY = rowEndValueY;

			return annotation;
		}

		/// <summary>
		/// Получение аннотации-текста по номерам рядов-категорий
		/// </summary>
		/// <param name="text">текст аннотации</param>
		/// <param name="columnLocation">положение по категориям</param>
		/// <param name="rowLocation">положение по рядам</param>
		/// <returns>аннотация-текст</returns>
		public static BoxAnnotation GetBoxAnnotationRowColumn(string text, int columnLocation, int rowLocation)
		{
			BoxAnnotation annotation = new BoxAnnotation();
			annotation.Border.Thickness = 0;
			annotation.Height = 50;
			annotation.Width = 150;
			annotation.Location.Type = LocationType.RowColumn;
			annotation.Location.Row = rowLocation;
			annotation.Location.Column = columnLocation;
			annotation.Text = text;
			annotation.TextStyle.HorizontalAlign = StringAlignment.Far;
			annotation.TextStyle.VerticalAlign = StringAlignment.Far;
			annotation.TextStyle.Font = new Font("Verdana", 10, FontStyle.Bold);
			return annotation;
		}

		#endregion

		#region Работа с диаграммой

		/// <summary>
		/// Обработчик диаграммы при пустых данных
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public static void UltraChartInvalidDataReceived(object sender, ChartDataInvalidEventArgs e)
		{
			e.Text = "Нет данных";
			e.LabelStyle.FontColor = Color.Black;
			e.LabelStyle.FontSizeBestFit = false;
			e.LabelStyle.HorizontalAlign = StringAlignment.Center;
			e.LabelStyle.VerticalAlign = StringAlignment.Center;
		}

		#endregion

		#region Работа с картой

		/// <summary>
		/// Тип элемента карты
		/// </summary>
		public enum MapShapeType
		{
			/// <summary>
			/// Территории
			/// </summary>
			Areas,
			/// <summary>
			/// Соседние территории
			/// </summary>
			SublingAreas,
			/// <summary>
			/// Водные объекты
			/// </summary>
			WaterObjects,
			/// <summary>
			/// Города
			/// </summary>
			Towns,
			/// <summary>
			/// Города-выноски
			/// </summary>
			CalloutTowns
		}

		/// <summary>
		/// Добавление слоя карты
		/// </summary>
		/// <param name="map">карта</param>
		/// <param name="layerFileName">имя файла с элементами</param>
		/// <param name="hasNames">добавлять ли имена элементов</param>
		/// <param name="shapeType">тип элементов</param>
		public static void AddMapLayer(MapControl map, string layerFileName, bool hasNames, MapShapeType shapeType)
		{
			if (!File.Exists(layerFileName))
			{
				return;
			}

			string layerName = Path.GetFileNameWithoutExtension(layerFileName);
			int oldShapesCount = map.Shapes.Count;

			map.LoadFromShapeFile(layerFileName, hasNames ? "NAME" : "", true);
			map.Layers.Add(layerName);

			for (int i = oldShapesCount; i < map.Shapes.Count; i++)
			{
				Shape shape = map.Shapes[i];
				shape.Layer = layerName;

				// раскрашиваем элементы
				switch (shapeType)
				{
					// регионы
					case MapShapeType.Areas:
						{
							shape.ToolTip = "#NAME";
							shape.Text = "";
							shape.Color = Color.White;
							break;
						}
					// соседние субъекты
					case MapShapeType.SublingAreas:
						{
							shape.ToolTip = "#NAME";
							shape.Color = Color.Gainsboro;
							shape.Text = "";
							break;
						}
					// водные объекты
					case MapShapeType.WaterObjects:
						{
							shape.Color = Color.LightBlue;
							shape.Text = "";
							break;
						}
					// города
					case MapShapeType.Towns:
						{
							shape.ToolTip = "#NAME";
							shape.Color = Color.White;
							shape.Text = "";
							break;
						}
				}
			}
		}

		/// <summary>
		/// Выделение элемента карты
		/// </summary>
		/// <param name="map">карта</param>
		/// <param name="shapeName">имя элемента</param>
		/// <param name="subString">поиск по подстроке</param> 
		/// <param name="text">подпись элемента</param>        
		public static void SelectMapShape(MapControl map, string shapeName, bool subString, string text)
		{
			foreach (Shape shape in map.Shapes)
			{
				if (shape.Name == shapeName && !subString ||
					shape.Name.Contains(shapeName) && subString)
				{
					Shape selectedShape = shape;
					shape.Text = text;
					SelectMapShape(selectedShape, true);
				}
			}
		}

		/// <summary>
		/// Выделение элемента карты
		/// </summary>
		/// <param name="shape">элемент карты</param>
		/// <param name="selected">выделять ли элемента</param>
		public static void SelectMapShape(Shape shape, bool selected)
		{
			if (shape != null)
			{
				if (selected)
				{
					shape.TextVisibility = TextVisibility.Shown;
					shape.Color = Color.PaleTurquoise;
					shape.TextColor = Color.Black;
					shape.GradientType = GradientType.Center;
					shape.SecondaryColor = Color.Gold;
					shape.ScaleFactor = 1.1;
				}
				else
				{
					shape.TextVisibility = TextVisibility.Hidden;
					shape.Text = "";
					shape.Color = Color.White;
					shape.SecondaryColor = Color.White;
					shape.GradientType = GradientType.Center;
					shape.ScaleFactor = 1;
				}
				shape.Selected = selected;
			}
		}

		/// <summary>
		/// Получение оптимального числа интервалов закраски карты
		/// </summary>
		/// <param name="dt">таблица данных</param>
		/// <param name="columnIndex">номер колонки</param>
		/// <param name="defaultIntervalCount">число интервалов по умолчанию</param>
		/// <param name="zeroColoring">учитываются ли нули в раскраске</param>
		/// <returns>оптимальное число интервалов</returns>
		public static int GetMapIntervalCount(DataTable dt, int columnIndex, int defaultIntervalCount, bool zeroColoring)
		{
			if (dt != null && columnIndex < dt.Columns.Count)
			{
				return GetMapIntervalCount(dt, dt.Columns[columnIndex].ColumnName, defaultIntervalCount, zeroColoring);
			}
			return -1;
		}

		/// <summary>
		/// Получение оптимального числа интервалов закраски карты
		/// </summary>
		/// <param name="dt">таблица данных</param>
		/// <param name="columnName">имя колонки</param>
		/// <param name="defaultIntervalCount">число интервалов по умолчанию</param>
		/// <param name="zeroColoring">учитываются ли нули в раскраске</param>
		/// <returns>оптимальное число интервалов</returns>
		public static int GetMapIntervalCount(DataTable dt, string columnName, int defaultIntervalCount, bool zeroColoring)
		{
			if (dt == null || !dt.Columns.Contains(columnName))
			{
				return -1;
			}

			Collection<double> differentValues = new Collection<double>();

			foreach (DataRow row in dt.Rows)
			{
				if (row[columnName] != DBNull.Value && row[columnName].ToString() != string.Empty)
				{
					double value = Convert.ToDouble(row[columnName]);
					if (value == 0 && !zeroColoring)
					{
						continue;
					}

					if (!differentValues.Contains(value))
					{
						differentValues.Add(value);
					}
				}
			}

			int intervalCount = differentValues.Count == 0 ? -1 : differentValues.Count - 1;

			return intervalCount > defaultIntervalCount ? defaultIntervalCount : intervalCount;
		}

		public static bool IsNormalSet(DataTable dt, string columnName, bool zeroColoring)
		{
			if (dt == null || !dt.Columns.Contains(columnName))
			{
				return false;
			}

			DataRow[] rows = dt.Select(string.Format("* ASC {0}", columnName));
			Collection<double> values = new Collection<double>();

			foreach (DataRow row in rows)
			{
				if (row[columnName] != DBNull.Value && row[columnName].ToString() != string.Empty)
				{
					double value = Convert.ToDouble(row[columnName]);
					if (value == 0 && !zeroColoring)
					{
						continue;
					}

					values.Add(value);
				}
			}
			if (values.Count > 0)
			{
				double minValue = values[0];
				double avgValue = values[values.Count / 2 - 1];
				double maxValue = values[values.Count - 1];


			}

			return false;
		}

		#endregion

		#region BrowsersHelper

		public static bool AllowedBrowser()
		{
			// Если опция не включена, то считаем, что баузер совместимый
			if (ConfigurationManager.AppSettings[CustomReportConst.BrowserCompatibilityKeyName] == null)
				return true;

			HttpBrowserCapabilities browser = HttpContext.Current.Request.Browser;
			//Trace.TraceVerbose("Проверка браузера пользователя: {0} {1} {2}", browser.Browser, browser.MajorVersion, HttpContext.Current.Request.UserAgent);

			// Если агент есть, но браузер Unknown, то считаем, что это ломаный эксплорер,
			// а, может, поисковые боты или сканеры уязвимостей.
			if (browser.Browser.Contains("Unknown"))
			{
				SaveToUserAgentLog(String.Format("{0}   {1}", HttpContext.Current.Request.UserAgent, browser.Browser));
				return true;
			}
			// Разбиваем строку со списком брузеров
			string[] allowedBrowsers =
				ConfigurationManager.AppSettings[CustomReportConst.BrowserCompatibilityKeyName].Split(';');

			for (int i = 0; i < allowedBrowsers.Length; i++)
			{
				// Если нашли текущий браузер в списке
				if (allowedBrowsers[i].Contains(browser.Browser))
				{
					// Отделяем номер версии
					string[] majorVersion = allowedBrowsers[i].Split(':');
					int version;
					// Если версия не записана
					if (majorVersion.Length < 2)
					{
						// считаем, что поддерживается любая.
						return true;
					}
					// берем поддерживаемую версию браузера
					Int32.TryParse(majorVersion[1], out version);
					// и сравниваем с текущей
					if (version <= browser.MajorVersion)
					{
						// еще бы минорные проверить неплохо
						// но это после тестов на совместимость.
						return true;
					}
					else
					{
						SaveToUserAgentLog(
							String.Format("{0}   {1} {2}", HttpContext.Current.Request.UserAgent, browser.Browser,
										  browser.MajorVersion));
						return false;
					}
				}
			}
			SaveToUserAgentLog(String.Format("{0}   {1}", HttpContext.Current.Request.UserAgent, browser.Browser));
			return false;
		}

		/// <summary>
		/// Подстраивает ширину колонки с учетом размера окна и типа браузера.
		/// </summary>
		/// <param name="defaultWidth">Стандартная ширина (IE, минимальный размер окна)</param>
		/// <returns>Ширина с учетом размера окна и типа браузера.</returns>
		public static int GetColumnWidth(double defaultWidth)
		{
			return GetColumnWidth(defaultWidth, CustomReportConst.minScreenWidth);
		}

		/// <summary>
		/// Подстраивает ширину колонки с учетом размера окна и типа браузера.
		/// </summary>
		/// <param name="defaultWidth">Стандартная ширина (IE, минимальный размер окна)</param>
		/// <param name="startExpandScreenWidth">Ширина окна, с которой начинать тянуть колонки.</param>
		/// <returns>Ширина с учетом размера окна и типа браузера.</returns>
		public static int GetColumnWidth(double defaultWidth, int startExpandScreenWidth)
		{
			// Если ширины достаточная
			if ((int)HttpContext.Current.Session["width_size"] > startExpandScreenWidth)
			{
				// Увеличиваем колонки на размер превышения
				defaultWidth =
					defaultWidth * (int)HttpContext.Current.Session["width_size"] / startExpandScreenWidth;
			}
			string browser = HttpContext.Current.Request.Browser.Browser;
			switch (browser)
			{
				case ("Firefox"):
					{
						return (int)(defaultWidth / 0.989);
					}
				case ("AppleMAC-Safari"):
					{
						return (int)(defaultWidth / 0.959);
					}
			}
			return (int)defaultWidth;
		}

		public static int GetColumnWidth(double defaultWidth, double koeff)
		{
			// Увеличиваем колонки на размер превышения
			defaultWidth = defaultWidth * (int)HttpContext.Current.Session["width_size"] / CustomReportConst.minScreenWidth *
						   koeff;

			string browser = HttpContext.Current.Request.Browser.Browser;
			switch (browser)
			{
				case ("Firefox"):
					{
						return (int)(defaultWidth / 0.989);
					}
				case ("AppleMAC-Safari"):
					{
						return (int)(defaultWidth / 0.959);
					}
			}
			return (int)defaultWidth;
		}

		/// <summary>
		/// Подстраивает ширину грида с учетом размера окна и типа браузера.
		/// </summary>
		/// <param name="defaultWidth">Стандартная ширина (IE, минимальный размер окна)</param>
		/// <returns>Ширина с учетом размера окна и типа браузера.</returns>
		public static Unit GetGridWidth(double defaultWidth)
		{
            if (defaultWidth == 0)
                return Unit.Empty;
			defaultWidth -= 20;
			if (HttpContext.Current.Session["PrintVersion"] != null &&
				(bool)HttpContext.Current.Session["PrintVersion"])
			{
				return Unit.Empty;
			}
			defaultWidth = defaultWidth * (int)HttpContext.Current.Session["width_size"] / CustomReportConst.minScreenWidth;
			string browser = HttpContext.Current.Request.Browser.Browser;
			switch (browser)
			{
				case ("Firefox"):
					{
						{
							return (int)(defaultWidth - 5);
						}
					}
				case ("AppleMAC-Safari"):
					{
						return (int)(defaultWidth * 0.998);
					}
			}
			return Unit.Pixel((int)defaultWidth);
		}

		/// <summary>
		/// Подстраивает ширину чарта с учетом размера окна.
		/// </summary>
		/// <param name="defaultWidth">Стандартная ширина (IE, минимальный размер окна)</param>
		/// <returns>Ширина с учетом размера окна и типа браузера.</returns>
		public static int GetChartWidth(double defaultWidth)
		{
			defaultWidth -= 10;
			defaultWidth = defaultWidth * (int)HttpContext.Current.Session["width_size"] / CustomReportConst.minScreenWidth;
			string browser = HttpContext.Current.Request.Browser.Browser;
			switch (browser)
			{
				case ("Firefox"):
				case ("AppleMAC-Safari"):
					{
						{
							return (int)(defaultWidth / 1.005);
						}
					}
			}
			return (int)defaultWidth;
		}

		/// <summary>
		/// Подстраивает высоту грида с учетом размера окна.
		/// </summary>
		/// <param name="defaultHeight">Стандартная высота (IE, минимальный размер окна)</param>
		/// <returns>Высота с учетом размера окна и типа браузера.</returns>
		public static Unit GetGridHeight(double defaultHeight)
		{
            if (defaultHeight == 0)
                return Unit.Empty;
			if (HttpContext.Current.Session["PrintVersion"] != null &&
				(bool)HttpContext.Current.Session["PrintVersion"])
			{
				return Unit.Empty;
			}
			defaultHeight -= 10;
			defaultHeight = defaultHeight * (int)HttpContext.Current.Session["height_size"] /
							CustomReportConst.minScreenHeight;
			string browser = HttpContext.Current.Request.Browser.Browser;
			switch (browser)
			{
				case ("Firefox"):
					{
						return (int)(defaultHeight * 1.08);
					}
				//                case ("AppleMAC-Safari"):
				//                    {
				//                        return (int)(defaultHeight * 0.8161);
				//                    }
			}

			return Unit.Pixel((int)defaultHeight);
		}


		/// <summary>
		/// Подстраивает высоту чарта с учетом размера окна.
		/// </summary>
		/// <param name="defaultHeight">Стандартная высота (IE, минимальный размер окна)</param>
		/// <returns>Высота с учетом размера окна и типа браузера.</returns>
		public static int GetChartHeight(double defaultHeight)
		{
			defaultHeight -= 10;
			defaultHeight = defaultHeight * (int)HttpContext.Current.Session["height_size"] /
							CustomReportConst.minScreenHeight;
			return (int)defaultHeight;
		}

		#endregion

		/// <summary>
		/// Установка фильтра по колонке таблицы
		/// </summary>
		/// <param name="source">исходная таблица</param>
		/// <param name="columnName">имя колонки</param>
		/// <param name="columnValue">значение колонки</param>
		/// <returns>выходная таблица</returns>
		public static DataTable SetDataTableFilter(DataTable source, string columnName, string columnValue)
		{
			DataTable dt = source.Clone();
			DataRow[] rows = source.Select(string.Format("{0} LIKE '{1}' or {0} IS NULL", columnName, columnValue));
			foreach (DataRow row in rows)
			{
				dt.ImportRow(row);
			}
			dt.AcceptChanges();

			return dt;
		}

		/// <summary>
		/// Сделать строку с заглавной буквы
		/// </summary>
		[Obsolete("Используйте подмешанный к строкам метод ToUpperFirstSymbol()")]
		public static string ToUpperFirstSymbol(string source)
		{
			return source.ToUpperFirstSymbol();
		}

		/// <summary>
		/// Сделать строку со строчной буквы
		/// </summary>
		[Obsolete("Используйте подмешанный к строкам метод ToLowerFirstSymbol()")]
		public static string ToLowerFirstSymbol(string source)
		{
			return source.ToLowerFirstSymbol();
		}

		/// <summary>
		/// Получить форматированную строку интервалов чисел
		/// </summary>
		/// <param name="source">исходная строка</param>
		/// <param name="separator">разделитель</param>
		/// <returns>строка интервалов</returns>
		public static string GetDigitIntervals(string source, char separator)
		{
			string[] digits = source.Split(separator);
			string result = string.Empty;
			List<int> intervals = new List<int>();

			for (int i = 0; i < digits.Length; i++)
			{
				int digit = Convert.ToInt32(digits[i]);
				if (intervals.Count == 0 || digit == intervals[intervals.Count - 1] + 1)
				{
					intervals.Add(digit);
				}
				else
				{
					if (intervals.Count == 1)
					{
						result += string.Format(" {0},", intervals[0]);
					}
					else
					{
						result += string.Format(" {0} - {1},", intervals[0], intervals[intervals.Count - 1]);
					}
					intervals.Clear();
					intervals.Add(digit);
				}
			}

			if (intervals.Count != 0)
			{
				if (intervals.Count == 1)
				{
					result += string.Format(" {0},", intervals[0]);
				}
				else
				{
					result += string.Format(" {0} - {1},", intervals[0], intervals[intervals.Count - 1]);
				}
			}

			if (result != string.Empty && result[result.Length - 1] == ',')
			{
				result = result.TrimEnd(',');
			}

			return result.TrimStart(' ');
		}

		/// <summary>
		/// Возвращает количество дней от начала квартала до заданной даты.
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		public static double QuarterDaysCountToDate(DateTime date)
		{
			int quarter = QuarterNumByMonthNum(date.Month);
			// Номер дня в году минус номер последнего дня предыдущего квартала.
			return date.DayOfYear - QuarterLastDayHumber(quarter - 1, date.Year);
		}

		/// <summary>
		/// Возвращает номер последнего дня квартала от начала года.
		/// </summary>
		/// <param name="quater">Номер квартала.</param>
		/// <param name="year">Год.</param>
		/// <returns></returns>
		public static int QuarterLastDayHumber(int quater, int year)
		{
			DateTime quarterLastDay;
			switch (quater)
			{
				case 1:
					{
						quarterLastDay = new DateTime(year, 3, 31);
						break;
					}
				case 2:
					{
						quarterLastDay = new DateTime(year, 6, 30);
						break;
					}
				case 3:
					{
						quarterLastDay = new DateTime(year, 9, 30);
						break;
					}
				default:
					{
						quarterLastDay = new DateTime(year, 12, 31);
						break;
					}
			}
			return quarterLastDay.DayOfYear;
		}

		/// <summary>
		/// Возвращает номер последнего дня квартала от начала года.
		/// </summary>
		public static int QuarterLastDayHumber(DateTime date)
		{
			int quarter = QuarterNumByMonthNum(date.Month);
			int year = date.Year;
			return QuarterLastDayHumber(quarter, year);
		}

		/// <summary>
		/// Возвращает количество дней в квартале.
		/// </summary>
		public static int QuarterDaysCount(DateTime date)
		{
			int quater = QuarterNumByMonthNum(date.Month);
			return (QuarterLastDayHumber(quater, date.Year) -
					QuarterLastDayHumber(quater - 1, date.Year));
		}

		/// <summary>
		/// Нормализует таблицу по колонкам.
		/// </summary>
		public static void NormalizeDataTable(DataTable dt)
		{
			if (dt == null)
			{
				return;
			}
			// Берем одну колонку.
			for (int colNum = 0; colNum < dt.Columns.Count; colNum++)
			{
				DataColumn col = dt.Columns[colNum];
				if (col.DataType.Equals(typeof(Double)) ||
					col.DataType.Equals(typeof(Int32)) ||
					col.DataType.Equals(typeof(Decimal)))
				{
					double sum = 0;
					for (int rowNum = 0; rowNum < dt.Rows.Count; rowNum++)
					{
						double value;
						double.TryParse(dt.Rows[rowNum][colNum].ToString(), out value);
						sum += value;
					}
					if (sum != 0)
					{
						{
							for (int rowNum = 0; rowNum < dt.Rows.Count; rowNum++)
							{
								double value;
								if (double.TryParse(dt.Rows[rowNum][colNum].ToString(), out value))
								{
									dt.Rows[rowNum][colNum] = value / sum * 100;
								}
							}
						}
					}
				}
			}
		}

		public static void SetPageTheme(Page page, string themeName)
		{
			// Если отчет iPhone, и не индексная страница ифон-отчетов то ставить не будем
			if (ConfigurationManager.AppSettings["iPhoneReport"] != null &&
				!HttpContext.Current.Request.Url.LocalPath.ToLower().Contains("/index.aspx"))
			{
				return;
			}

			// На всякий случай проверим, есть ли такая тема.
			DirectoryInfo themeDir = new DirectoryInfo(
				String.Format("{0}/App_Themes", HttpContext.Current.Request.PhysicalApplicationPath));
			DirectoryInfo[] themes = themeDir.GetDirectories();
			for (int i = 0; i < themes.Length; i++)
			{
				if (themes[i].Name == themeName)
				{
					// Если нашли, то ставим и уходим.
					page.Theme = themeName;
					return;
				}
			}

			// Присваиваем тему по умолчанию.
			page.Theme = "Default";
		}

		public static string RemoveParameterFromQuery(Collection<string> redudantKeys,
													  NameValueCollection queryStrings)
		{
			string query = "?";
			foreach (string key in queryStrings.Keys)
			{
				if (!redudantKeys.Contains(key))
				{
					query += key + "=" + HttpContext.Current.Request.Params[key] + "&";
				}
			}
			query = query.TrimEnd('&');
			query = query.TrimEnd('?');
			return query;
		}

		/// <summary>
		/// Получение ключа словаря по значению
		/// </summary>
		/// <param name="dictionary">словарь</param>
		/// <param name="value">значение</param>
		/// <returns>ключ первого попавшегося значения</returns>
		public static string GetKeyByDictionaryValue(Dictionary<string, string> dictionary, string value)
		{
			foreach (string key in dictionary.Keys)
			{
				if (dictionary[key] == value)
				{
					return key;
				}
			}
			return string.Empty;
		}

		public static int GetScreenWidth
		{
			get
			{
				HttpCookie cookie = HttpContext.Current.Request.Cookies[CustomReportConst.ScreenWidthKeyName];
				if (cookie != null)
				{
					int value = Int32.Parse(cookie.Value);
					return value;
				}

				return (int)HttpContext.Current.Session[CustomReportConst.ScreenWidthKeyName];
			}
		}

		public static int GetScreenHeight
		{
			get
			{
				HttpCookie cookie = HttpContext.Current.Request.Cookies[CustomReportConst.ScreenHeightKeyName];
				if (cookie != null)
				{
					int value = Int32.Parse(cookie.Value);
					return value;
				}

				return (int)HttpContext.Current.Session[CustomReportConst.ScreenHeightKeyName];
			}
		}

		/// <summary>
		/// Переводит пиксели в пункты
		/// </summary>
		public static float PixelsToPoints(double value)
		{
			return (float)(value * 0.75);
		}

		/// <summary>
		/// Преобразует значение в double, при неудаче возвращает 0
		/// </summary>
		public static double DBValueConvertToDoubleOrZero(object value)
		{
			double result = 0;
			if (value != DBNull.Value)
			{
				// если преобразование неудачно, возвращает 0
				Double.TryParse(value.ToString(), out result);
			}
			return result;
		}

		/// <summary>
		/// Преобразует значение в decimal, при неудаче возвращает 0
		/// </summary>
		public static decimal DBValueConvertToDecimalOrZero(object value)
		{
			decimal result = 0;
			if (value != DBNull.Value)
			{
				// если преобразование неудачно, возвращает 0
				Decimal.TryParse(value.ToString(), out result);
			}
			return result;
		}

		/// <summary>
		/// Преобразует значение в int, при неудаче возвращает 0
		/// </summary>
		public static int DBValueConvertToInt32OrZero(object value)
		{
			int result = 0;
			if (value != DBNull.Value)
			{
				// если преобразование неудачно, возвращает 0
				Int32.TryParse(value.ToString(), out result);
			}
			return result;
		}

		/// <summary>
		/// Показывает, пусто ли значение взятое из БД
		/// </summary>
		public static bool DBValueIsEmpty(object value)
		{
			return value == DBNull.Value
				   || value == null
			       || String.IsNullOrEmpty(value.ToString());
		}


		#region WMobile

		public const int fontSGM0009O = 12;
		public const int fontSGM0009V = 12;
		public const int fontSGM0009H = 12;

		public const int fontSGM0006O = 12;
		public const int fontSGM0006V = 12;
		public const int fontSGM0006H = 12;

		public const int fontFK0001O = 12;
		public const int fontFK0001H = 12;

		public const int fontFK0002O = 12;
		public const int fontFK0002V = 12;
		public const int fontFK0002H = 12;

		public const int fontFK0003O = 12;
		public const int fontFK0003V = 12;
		public const int fontFK0003H = 12;

		public const int fontFK0004O = 12;
		public const int fontFK0004V = 9;
		public const int fontFK0004H = 9;

		public const int fontMF0005H = 8;

		public const string boldBorderStyle = "#323232 2px solid";
		public const string noneBorderStyle = "#323232 0px solid";
		public static Color fontLightColor = Color.FromArgb(0xd1d1d1);
		public static Color fontGrayColor = Color.FromArgb(0xd1d1d1);
		public static Color fontTableCaptionColor = Color.White;
		public static Color fontTableDataColor = Color.White;

		public static void SetNextMonth(ref int yearNum, ref int monthNum)
		{
			if (monthNum == 12)
			{
				monthNum = 1;
				yearNum++;
			}
			else
			{
				monthNum++;
			}
		}

		public static void SetCellHBorder(TableRow row, bool isTopBorder)
		{
			string borderName = string.Empty;
			for (int i = 0; i < row.Cells.Count; i++)
			{
				borderName = "border-bottom";
				if (isTopBorder) borderName = "border-top";
				row.Cells[i].Style[borderName] = CRHelper.boldBorderStyle;
			}
		}

		public static void SetCellHBorder(TableRow row)
		{
			SetCellHBorder(row, false);
		}

		public static void SetCellVBorder(TableRow row, int cellIndex1, int cellIndex2)
		{
			row.Cells[cellIndex1].Style["border-right"] = CRHelper.boldBorderStyle;
			row.Cells[cellIndex2].Style["border-left"] = CRHelper.boldBorderStyle;
		}

		public static void SetCellVBorderNone(TableRow row, int cellIndex1, int cellIndex2)
		{
			row.Cells[cellIndex1].Style["border-right"] = CRHelper.noneBorderStyle;
			row.Cells[cellIndex2].Style["border-left"] = CRHelper.noneBorderStyle;
		}

		public static void SetCellHBorderNone(TableRow row, bool isTopBorder)
		{
			string borderName = string.Empty;
			for (int i = 0; i < row.Cells.Count; i++)
			{
				borderName = "border-bottom";
				if (isTopBorder) borderName = "border-top";
				row.Cells[i].Style[borderName] = CRHelper.noneBorderStyle;
			}
		}

		public static void SetCellHBorderNone(TableRow row)
		{
			SetCellHBorderNone(row, false);
		}

		public static string GetImage(DataRow dr, int index1, int index2)
		{
			string img = string.Empty;
			if (dr[index1] != DBNull.Value)
			{
				if (Convert.ToInt32(dr[index1]) == 1)
				{
					img = "<img height=\"15\" width=\"15\" src=\"../../../images/starYellow.png\">";
				}
				else if (Convert.ToInt32(dr[index1]) == Convert.ToInt32(dr[index2]) &&
						 Convert.ToInt32(dr[index1]) != 0)
				{
					img = "<img height=\"15\" width=\"15\" src=\"../../../images/starGray.png\">";
				}
			}
			return img;
		}

		public static void AddCaptionCell(TableRow row, string caption, int fontSize, Color fontColor)
		{
			AddCell(row, caption, true, HorizontalAlign.Center, fontSize, fontColor, false);
		}

		public static void AddDataCellL(TableRow row, string caption, int fontSize, Color fontColor)
		{
			AddCell(row, caption, false, HorizontalAlign.Left, fontSize, fontColor, false);
		}

		public static void AddDataCellR(TableRow row, string caption, int fontSize, Color fontColor)
		{
			AddCell(row, caption, false, HorizontalAlign.Right, fontSize, fontColor, false);
		}

		public static void AddDataCellL(TableRow row, string caption, int fontSize, Color fontColor, bool isBold)
		{
			AddCell(row, caption, false, HorizontalAlign.Left, fontSize, fontColor, isBold);
		}

		public static void AddDataCellR(TableRow row, string caption, int fontSize, Color fontColor, bool isBold)
		{
			AddCell(row, caption, false, HorizontalAlign.Right, fontSize, fontColor, isBold);
		}

		public static void AddCell(TableRow row, string caption, bool isCaption, HorizontalAlign halign, int fontSize, Color fontColor, bool isBold)
		{
			TableCell cell = new TableCell();
			Label text = new Label();
			text.Text = caption;
			text.ForeColor = fontColor;
			text.Font.Name = "Arial";
			text.Font.Bold = isBold;
			text.Font.Size = new System.Web.UI.WebControls.FontUnit(fontSize);
			cell.Controls.Add(text);
			if (isCaption)
			{
				cell.BackColor = Color.FromArgb(51, 51, 51);
			}

			cell.HorizontalAlign = halign;
			cell.BorderColor = Color.FromArgb(51, 51, 51);
			row.Cells.Add(cell);
		}

		#endregion

		#region Строки

		/// <summary>
		/// Возвращает значение последнего блока в MDX UniqueName
		/// </summary>
		/// <param name="mdxString">MDX имя</param>
		/// <returns>Последний блок</returns>
		public static string GetLastBlock(string mdxString)
		{
			string[] separator = { "].[" };
			string[] stringElements = mdxString.Split(separator, StringSplitOptions.None);
			return stringElements[stringElements.Length - 1].Replace("]", String.Empty);
		}

		/// <summary>
		/// Преобразует значение в строку со знаком, если значение положительное, то добавит перед ним +
		/// </summary>
		/// <param name="value">Значение</param>
		/// <returns>То, что получилось</returns>
		public static string NumberToStringWithSign(object value)
		{
			return (Convert.ToDouble(value) <= 0) ? String.Format("{0:N2}", value) : String.Format("+{0:N2}", value);
		}

		/// <summary>
		/// Преобразует значение в строку-процент со знаком, если значение положительное, то добавит перед ним +
		/// </summary>
		/// <param name="value">Значение</param>
		/// <returns>То, что получилось</returns>
		public static string PercentToStringWithSign(object value)
		{
			return (Convert.ToDouble(value) <= 0) ? String.Format("{0:P2}", value) : String.Format("+{0:P2}", value);
		}

		/// <summary>
		/// Преобразует значение в строку вылюты со знаком, если значение положительное, то добавит перед ним +
		/// </summary>
		/// <param name="value">Значение</param>
		/// <returns>То, что получилось</returns>
		public static string CurrencyToStringWithSign(object value)
		{
			return (Convert.ToDouble(value) <= 0) ? String.Format("{0:N2}", value) : String.Format("+{0:N2}", value);
		}

		#endregion

	}

	public class RankCalculator
	{
		private Collection<double> valuesCollection;
		private Collection<string> namesCollection;

		private RankDirection rankDireciton;

		public RankCalculator(RankDirection direciton)
		{
			rankDireciton = direciton;
			valuesCollection = new Collection<double>();
			namesCollection = new Collection<string>();
		}

		public void AddItem(double value)
		{
			AddItem(Guid.NewGuid().ToString(), value);
		}

		public void AddItem(string name, double value)
		{
			// повторяющиеся исключаем
			if (valuesCollection.Contains(value))
			{
				return;
			}

			// вставляем в отсортированную коллекцию сразу в нужное место
			int i = 0;
			while (i < valuesCollection.Count &&
				(rankDireciton == RankDirection.Asc && valuesCollection[i] > value ||
				 rankDireciton == RankDirection.Desc && valuesCollection[i] < value))
			{
				i++;
			}

			valuesCollection.Insert(i, value);
			namesCollection.Insert(i, name);
		}

		public int GetRank(double value)
		{
			for (int i = 0; i < valuesCollection.Count; i++)
			{
				if (IsDoubleEquals(valuesCollection[i], value))
				{
					return i + 1;
				}
			}

			return 0;
		}

		public int GetWorseRank()
		{
			return valuesCollection.Count;
		}

		public double GetMaxValue()
		{
			if (valuesCollection.Count > 0)
			{
				int index = rankDireciton == RankDirection.Desc ? valuesCollection.Count - 1 : 0;
				return valuesCollection[index];
			}
			return double.MinValue;
		}

		public double GetMinValue()
		{
			if (valuesCollection.Count > 0)
			{
				int index = rankDireciton == RankDirection.Asc ? valuesCollection.Count - 1 : 0;
				return valuesCollection[index];
			}
			return double.MaxValue;
		}

		public string GetBestItem()
		{
			if (namesCollection.Count > 0)
			{
				int index = rankDireciton == RankDirection.Desc ? namesCollection.Count - 1 : 0;
				return namesCollection[index];
			}
			return String.Empty;
		}

		public string GetWorseItem()
		{
			if (namesCollection.Count > 0)
			{
				int index = rankDireciton == RankDirection.Asc ? namesCollection.Count - 1 : 0;
				return namesCollection[index];
			}
			return String.Empty;
		}

		//        public void ItemsToLog()
		//        {
		//            for (int i = 0; i < itemCollection.Count; i++)
		//            {
		//                CRHelper.SaveToErrorLog(String.Format("{0} - {1}", i, itemCollection[i]));
		//            }
		//        }

		private bool IsDoubleEquals(double value1, double value2)
		{
			return Math.Round(value1 * 10000) == Math.Round(value2 * 10000);
		}
	}

	public enum RankDirection
	{
		Asc,
		Desc
	}
}