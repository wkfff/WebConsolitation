using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Common.GridIndicatorRules;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.SKK.FO_0041_0001
{
	public partial class Default : CustomReportPage
	{
		private const int firstYear = 2009;
		private int lastYear;
		private Collection<string> paramTaxNames;
		private Dictionary<int, double> inflation;
		// параметры запроса
		private CustomParam paramYear;
		private CustomParam paramTax;
		

		public Default()
		{
			paramTaxNames = 
				new Collection<string>
					{
						"Налог на прибыль организаций", 
						"Налог на имущество организаций", 
						"Транспортный налог"
					};
			
			inflation =
				new Dictionary<int, double>
					{
						{2009, 1.11}
					};
		}

		protected override void Page_PreLoad(object sender, EventArgs e)
		{
			base.Page_PreLoad(sender, e);

			// Инициализация параметров запроса
			paramYear = UserParams.CustomParam("selected_year");
			paramTax = UserParams.CustomParam("selected_tax");

			// Настройка экспорта
			ReportExcelExporter1.ExcelExportButton.Click += ExcelExportButton_Click;
			ReportPDFExporter1.PdfExportButton.Click += PdfExportButton_Click;
		}

		protected override void Page_Load(object sender, EventArgs e)
		{
			base.Page_Load(sender, e);

			if (!Page.IsPostBack)
			{
				// последний год, на который есть данные
				DataTable table = Helper.GetDataTable("FO_0041_0001_last_period", Helper.DataProviderTypes.Primary);
				if (table.Rows.Count > 0 && table.Rows[0][0] != DBNull.Value)
				{
					if (!Int32.TryParse(table.Rows[0][0].ToString(), out lastYear))
					{
						lastYear = firstYear;
					}
				}

				// параметр - период
				ComboYear.Title = "Год";
				ComboYear.Width = 200;
				ComboYear.MultiSelect = false;
				ComboYear.ParentSelect = false;
				ComboYear.ShowSelectedValue = true;
				ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, lastYear));
				ComboYear.SelectLastNode();

				// параметр - раздел отчета
				ComboTax.Title = "Вид налога";
				ComboTax.Width = 400;
				ComboTax.MultiSelect = false;
				ComboTax.ParentSelect = false;
				ComboTax.ShowSelectedValue = true;
				ComboTax.FillValues(paramTaxNames);
				
			}

			// параметры для запроса
			paramYear.Value = String.Format("[Период__Период].[Период__Период].[Данные всех периодов].[{0}], ", ComboYear.SelectedValue);
			paramTax.Value = ComboTax.SelectedValue;

			// текстовики
			PageTitle.Text = String.Format(
				"Результаты оценки бюджетной, социальной и экономической эффективности предоставляемых " +
				"(планируемых к предоставлению) налоговых льгот за {0} год", ComboYear.SelectedValue);
			Page.Title = PageTitle.Text;
			
			// погнали!
			GenerateReport();

		}

		private void GenerateReport()
		{
			//scriptBlock.JavaScript_SetVerticalScrollBar(GridBrick);
			GridHelper grid = new GridHelper(this);
			grid.Init(GridBrick, "FO_0041_0001_grid");

			if (ComboTax.SelectedIndex == 0 || ComboTax.SelectedIndex == 2)
			{
				Label1.Text = "Предельные значения коэффициентов бюджетной, социальной и экономической эффективности >= 1,0";
			}
			else if (ComboTax.SelectedIndex == 1)
			{
				Label1.Text = String.Format(
					"Предельные значения коэффициентов бюджетной эффективности >= уровень инфляции ({0})<br />" +
					"Предельные значения коэффициентов социальной и экономической эффективности >= 1,0.",
					inflation[Convert.ToInt32(ComboYear.SelectedValue)]);
			}
			
		}

		private void PdfExportButton_Click(object sender, EventArgs e)
		{
			Report report = new Report();
			ISection section1 = report.AddSection();

			ReportPDFExporter1.HeaderCellHeight = 20;

			ReportPDFExporter1.PageTitle = PageTitle.Text;
			ReportPDFExporter1.Export(GridBrick.GridHeaderLayout, section1);
			
		}

		private void ExcelExportButton_Click(object sender, EventArgs e)
		{
			Workbook workbook = new Workbook();
			Worksheet sheet1 = workbook.Worksheets.Add("sheet1");

			ReportExcelExporter1.SheetColumnCount = 15;
			ReportExcelExporter1.HeaderCellHeight = 20;
			ReportExcelExporter1.GridColumnWidthScale = 1.3;
			ReportExcelExporter1.RowsAutoFitEnable = true;

			ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
			ReportExcelExporter1.Export(GridBrick.GridHeaderLayout, sheet1, 3);
			
		}

		#region Таблицы
		
		public class GridHelper
		{
			private Default Page { set; get; }
			private UltraGridBrick Grid { set; get; }
			private DataTable Data { set; get; }
			private UltraGridBand Band { set; get; }
			
			public int DeleteColumns { set; get; }
			public int HiddenColumns { set; get; }

			
			public GridHelper(Default page)
			{
				Page = page;
				DeleteColumns = 0;
				HiddenColumns = 1;
			}

			public virtual void Init(UltraGridBrick grid, string queryName)
			{
				Grid = grid;
				Grid.Grid.InitializeLayout += InitializeLayout;
				Grid.Grid.InitializeRow += InitializeRow;

				SetStyle();
				SetData(queryName);
			}
			
			public virtual void SetStyle()
			{
				Grid.EnableViewState = false;
				Grid.RedNegativeColoring = false;

				Grid.AutoSizeStyle = GridAutoSizeStyle.AutoHeight;
				Grid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth-10);
				
				if (Page.ComboTax.SelectedIndex == 1)
				{
					Grid.AutoHeightRowLimit = 15;
					Grid.Height = CRHelper.GetGridHeight(500);
				}
				
			}
			
			public virtual void SetData(string queryName)
			{
				Data = Helper.GetDataTable(queryName, Helper.DataProviderTypes.Primary);
				if (Data.Rows.Count > 0)
				{
					Data.DeleteColumns(DeleteColumns);
					
					for (int i = 0; i < Data.Columns.Count; i++)
					{
						if (i > 2 && i < Data.Columns.Count - HiddenColumns - 1)
						{
							Data.Rows[Data.Rows.Count - 1][i] = DBNull.Value;
						}
					}

					Grid.DataTable = Data;
				}

			}

			public virtual void InitializeRow(object sender, RowEventArgs e)
			{
				UltraGridRow row = e.Row;
				
				const string imgUp = "~/images/arrowGreenUpBB.png";
				const string imgDown = "~/images/arrowRedDownBB.png";
				const string hintUp = "Высокая эффективность";
				const string hintDown = "Низкая эффективность";
				
				foreach (int columnIndex in (new[] { 3, 5, 7 }))
				{
					double limit = 1;
					if (columnIndex == 3)
					{
						int year = Convert.ToInt32(Page.ComboYear.SelectedValue);
						if (Page.inflation.ContainsKey(year))
						{
							limit = Page.inflation[year];
						}
					}
					
					UltraGridCell cell = row.Cells[columnIndex];
					UltraGridCell target = row.Cells[columnIndex+1];

					double value;
					if (cell.Value != null && Double.TryParse(cell.Value.ToString(), out value))
					{
						if (value >= limit)
						{
							target.Style.BackgroundImage = imgUp;
							target.Title = hintUp;
						}
						else
						{
							target.Style.BackgroundImage = imgDown;
							target.Title = hintDown;
						}
						target.Style.CustomRules = "background-repeat: no-repeat; background-position: center center;";
					}


				}
			}

			protected virtual void InitializeLayout(object sender, LayoutEventArgs e)
			{
				e.Layout.RowAlternateStyleDefault.CopyFrom(e.Layout.RowStyleDefault);

				if (e.Layout.Bands[0].Columns.Count == 0)
				{
					return;
				}

				Band = e.Layout.Bands[0];
				SetDataStyle();
				SetDataRules();
				SetDataHeader();
			}

			public virtual void SetDataStyle()
			{
				Band.Columns[0].CellStyle.Wrap = true;
				Band.Columns[0].Width = CRHelper.GetColumnWidth(260);
				Band.Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

				Band.HideColumns(HiddenColumns);

				for (int i = 1; i < Band.Columns.Count - HiddenColumns; i++)
				{
					string caption = Band.Columns[i].Header.Caption;
					
					Band.Columns[i].Width = CRHelper.GetColumnWidth(110);

					string format;
					if (caption.EndsWith("%"))
						format = "P2";
					else if (caption.EndsWith("#"))
						format = "N0";
					else if (caption.EndsWith("#2"))
						format = "N2";
					else
						format = String.Empty;
					CRHelper.FormatNumberColumn(Band.Columns[i], format);
				}

			}

			public virtual void SetDataRules()
			{
				// правило для первой строки
				FontRowLevelRule levelRule = new FontRowLevelRule(Band.Columns.Count - 1);
				levelRule.AddFontLevel("0", Grid.BoldFont8pt);
				Grid.AddIndicatorRule(levelRule);
			}

			public virtual void SetDataHeader()
			{
				GridHeaderLayout headerLayout = Grid.GridHeaderLayout;
				GridHeaderCell header1;

				headerLayout.AddCell("Наименование вида экономической деятельности");
				headerLayout.AddCell("Сумма льготы, тыс. руб.");
				headerLayout.AddCell("Количество организаций, шт");
				header1 = headerLayout.AddCell("Бюджетная эффективность");
				header1.AddCell("Коэффициент");
				header1.AddCell("Эффективность");
				header1 = headerLayout.AddCell("Социальная  эффективность");
				header1.AddCell("Коэффициент");
				header1.AddCell("Эффективность");
				header1 = headerLayout.AddCell("Экономическая эффективность");
				header1.AddCell("Коэффициент");
				header1.AddCell("Эффективность");
				Grid.GridHeaderLayout.ApplyHeaderInfo();
			}


		}
		
		#endregion

		#region Вспомогательные функции

		
		#endregion
	}

	/// <summary>
	/// Класс доп функций и расширений
	/// </summary>
	public static class Helper
	{
		public enum DataProviderTypes
		{
			Primary, Secondary, Spare
		}
		
		/// <summary>
		/// получает таблицу по типу провайдера и идентификатору запроса 
		/// </summary>
		public static DataTable GetDataTable(string queryName, DataProviderTypes providerType)
		{
			DataTable table = new DataTable();
			string query = DataProvider.GetQueryText(queryName);
			switch (providerType)
			{
				case DataProviderTypes.Primary:
					DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "dummy", table);
					break;
				case DataProviderTypes.Secondary:
					DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "dummy", table);
					break;
				case DataProviderTypes.Spare:
					DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "dummy", table);
					break;
			}
			return table;
		}

		/// <summary>
		/// удалить колонки в начале таблицы
		/// </summary>
		public static void DeleteColumns(this DataTable table, int count)
		{
			for (int i = 0; i < count; i++)
			{
				table.Columns.RemoveAt(0);
			}
		}

		/// <summary>
		/// скрыть колонки в конце таблицы
		/// </summary>
		public static void HideColumns(this UltraGridBand band, int count)
		{
			int columnsCount = band.Columns.Count;

			for (int i = 0; i < count; i++)
			{
				band.Columns[columnsCount - 1].Hidden = true;
				columnsCount--;
			}
		}

		/// <summary>
		/// добавить javascript, изменяющий ширину грида под вертикальную полосу прокрутки
		/// </summary>
		public static void JavaScript_SetVerticalScrollBar(this HtmlGenericControl tag, UltraGridBrick grid)
		{
			StringBuilder scriptString = new StringBuilder();
			scriptString.Append("\n<script type='text/javascript'><!--\n");
			scriptString.AppendFormat(
					"window.onload = function(){{\ngetSize();\nSetGridScrollBar('{0}', '{1}', '{2}');\n}}\n",
					"G_" + grid.Grid.ClientID.Replace("_", "x"),
					grid.Grid.ClientID.Replace("_", "x") + "_main",
					17);
			scriptString.Append(GetJSVerticalScrollBar());
			scriptString.Append("\n//--></script>\n");

			tag.InnerHtml = scriptString.ToString();
		}

		/// <summary>
		/// текст javascript-функции, увеличивающей ширину элемента
		/// </summary>
		private static string GetJSVerticalScrollBar()
		{
			StringBuilder scriptString = new StringBuilder();
			scriptString.Append(
@"function SetGridScrollBar(data, main, scrollWidth)
{
	var data_obj = ig_csom.getElementById(data);
	var main_obj = ig_csom.getElementById(main);
	main_obj.style.width = parseInt(data_obj.style.width) + parseInt(scrollWidth) + 'px';
}");
			return scriptString.ToString();
		}

	}
}
