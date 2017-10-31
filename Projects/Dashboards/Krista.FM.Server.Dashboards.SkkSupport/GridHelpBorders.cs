using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Common.GridIndicatorRules;
using Krista.FM.Server.Dashboards.Components;

namespace Krista.FM.Server.Dashboards.SkkSupport
{
	// обертка над таблицей показателя (досмотрено/приостановлено)

	public class GridHelpBordersCommon : GridHelpBase
	{

		public override void Init(Page mainControl, string queryName)
		{
			page = mainControl;
			Grid = new GridHelpBorders((UltraGridBrick)page.LoadControl("../../../Components/UltraGridBrick.ascx"));
			
			Grid.SetStyle();
			Grid.DeleteColumns = 1;
			Grid.SetData(queryName);
		}

	}

	public class GridHelpBorders : GridBase
	{
		private Dictionary<string, object[]> parameters;
		
		public GridHelpBorders(UltraGridBrick grid)
			: base(grid)
		{
			parameters = new Dictionary<string, object[]>
			{
				{"Российско-норвежский", new[] {"россий-<br />ско-<br />норвеж-<br />ский"}},
				{"Российско-финский", new[] {"россий-<br />ско-<br />финский"}},
				{"Российско-эстонский", new[] {"россий-<br />ско-<br />эстон-<br />ский"}},
				{"Российско-латвийский", new[] {"россий-<br />ско-<br />латвий-<br />ский"}},
				{"Российско-литовский", new[] {"россий-<br />ско-<br />литов-<br />ский"}},
				{"Российско-польский", new[] {"россий-<br />ско-<br />поль-<br />ский"}},
				{"Российско-украинский", new[] {"россий-<br />ско-<br />украин-<br />ский"}},
				{"Российско-абхазский", new[] {"россий-<br />ско-<br />абхаз-<br />ский"}},
				{"Российско-южноосетинский", new[] {"россий-<br />ско-<br />южно-<br />осетин-<br />ский"}},
				{"Российско-грузинский", new[] {"россий-<br />ско-<br />грузин-<br />ский"}},
				{"Российско-азербайджанский", new[] {"россий-<br />ско-<br />азер-<br />байджан-<br />ский", (object)62}},
				{"Российско-казахстанский", new[] {"россий-<br />ско-<br />казах-<br />стан-<br />ский"}},
				{"Российско-монгольский", new[] {"россий-<br />ско-<br />монголь-<br />ский", (object)64}},
				{"Российско-китайский", new[] {"россий-<br />ско-<br />китай-<br />ский"}},
				{"Российско-корейский", new[] {"россий-<br />ско-<br />корей-<br />ский"}},
				
				{"Воздушный", new[] {"Воздушные ПП", (object)72}},
				{"Морской", new[] {"Морские ПП", (object)60}},
				{"Речной", new[] {"Речные ПП", (object)55}},

			};
		}

		/// <summary>
		/// настройка внешнего вида 
		/// </summary>
		public override void SetStyle()
		{
			SetFullAutoSizes();
			//Grid.Grid.Style.Add("margin", "0 auto");
			//Grid.Grid.Style.Add("*margin", "0");
		}
		

		/// <summary>
		/// настройка внешнего вида данных
		/// </summary>
		public override void SetDataStyle()
		{
			Band.Columns[0].CellStyle.Wrap = true;
			Band.Columns[0].Width = 165;
			Band.Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

			Band.HideColumns(1);
			Band.SetDefaultStyle(56, "N0", 1);

			foreach (UltraGridColumn column in Band.Columns)
			{
				
				if (parameters.ContainsKey(column.Header.Caption))
				{
					object[] param = parameters[column.Header.Caption];
					if (param.Length > 1)
					{
						column.Width = (int)param[1];
					}
				}
			}
			
		}

		/// <summary>
		/// настройка индикаторов
		/// </summary>
		public override void SetDataRules()
		{
			// правило жирности
			FontRowLevelRule levelRule = new FontRowLevelRule(Band.Columns.Count - 1);
			levelRule.AddFontLevel("0", Grid.BoldFont8pt);
			Grid.AddIndicatorRule(levelRule);

			// сдвиг текста согласно иерархии
			Grid.AddIndicatorRule(new PaddingRule(0, "Уровень", 10));
		}

		/// <summary>
		/// заголовки таблицы
		/// </summary>
		public override void SetDataHeader()
		{
			GridHeaderLayout headerLayout = Grid.GridHeaderLayout;
			headerLayout.AddCell("Субъект РФ/пункт<br />пропуска");

			foreach (UltraGridColumn column in Band.Columns)
			{
				if (parameters.ContainsKey(column.Header.Caption))
				{
					object[] param = parameters[column.Header.Caption];
					headerLayout.AddCell((string) param[0]);
				}
			}
			
			Grid.GridHeaderLayout.ApplyHeaderInfo();
		}

		public override void SetData(string queryName)
		{
			base.SetData(queryName);
			DataTable table = Grid.DataTable;

			// названиия колонок
			for (int i = 0; i < table.Columns.Count; i++)
			{
				DataColumn column = table.Columns[i];
				
				column.Caption = column.Caption.
					Replace("Все; Показатель; ", "").
					Replace("; Показатель; Все", "").
					Replace("Все; Уровень; Все", "Уровень");
				column.ColumnName = column.Caption;
			}

			// сортировка строк
			if (table.Rows.Count > 2)
			{
				BordersRow total = new BordersRow(table.Rows[0].ItemArray);

				// заполнение структуры и сортировка
				List<BordersRow> list = new List<BordersRow>();
				BordersRow current = new BordersRow(table.Rows[1].ItemArray);
				for (int i = 2; i < table.Rows.Count; i++)
				{
					DataRow row = table.Rows[i];
					if (row["Уровень"].ToString() == "0")
					{
						current.SubRows.Sort();
						list.Add(current);
						current = new BordersRow(table.Rows[i].ItemArray);
					}
					else
					{
						current.AddSubRow(table.Rows[i].ItemArray);
					}
				}
				list.Add(current);
				list.Sort();
				
				table.Clear();

				// заполнение таблицы
				table.Rows.Add(total.Items);
				foreach (BordersRow rowSubject in list)
				{
					table.Rows.Add(rowSubject.Items);
					foreach (BordersRow rowPP in rowSubject.SubRows)
					{
						table.Rows.Add(rowPP.Items);
					}
				}

			}

			
		}
		
		/// <summary>
		/// Вспомогательный класс для сортировки строк таблицы (с подстроками)
		/// </summary>
		private class BordersRow : IComparable<BordersRow>
		{
			public object[] Items { private set; get; }
			public List<BordersRow> SubRows { private set; get; }

			public BordersRow(object[] items)
			{
				Items = items;
				SubRows = new List<BordersRow>();
			}

			public void AddSubRow(object[] array)
			{
				SubRows.Add(new BordersRow(array));
			}
			
			public int CompareTo(BordersRow other)
			{
				List<object> rowA = new List<object>(Items);
				List<object> rowB = new List<object>(other.Items);
				int i = 1;
				while (i < rowA.Count && rowA[i] == rowB[i])
				{
					i++;
				}
				if (i < rowA.Count)
				{
					int valueA;
					if (!Int32.TryParse(rowA[i].ToString(), out valueA))
						valueA = -1;
					int valueB;
					if (!Int32.TryParse(rowB[i].ToString(), out valueB))
						valueB = -1;

					if (valueA > valueB)
						return -1;
					if (valueA < valueB)
						return 1;
					return 0;
				}
				return 0;
			}
			
		}

	}
}
