using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Common.GridIndicatorRules;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.FO_0002_0002_Settlement.Default.reports.FO_0002_0002_Settlement;

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0002_Settlement
{
	/// <summary>
	/// Анализ результатов исполнения бюджетов поселений
	/// Астрахань
	/// ФО_МесОтч_Дефицит Профицит
	/// </summary>
	public partial class Default : CustomReportPage
	{
		private const int firstYear = 2010;
		private int lastYear = firstYear;
		private string lastMonth = "Январь";
		
		// Параметры запроса
		private CustomParam paramPeriod;
		private CustomParam paramArea;
		private CustomParam paramScale;
		private CustomParam paramMeasure;
		private static MemberAttributesDigest digestAreas;
		
		
		protected override void Page_PreLoad(object sender, EventArgs e)
		{
			base.Page_PreLoad(sender, e);

			// Инициализация параметров запроса
			paramPeriod   = UserParams.CustomParam("selected_period");
			paramArea     = UserParams.CustomParam("selected_area");
			paramScale    = UserParams.CustomParam("selected_scale");
			paramMeasure = UserParams.CustomParam("selected_measure");

			// Настройка экспорта
			ReportExcelExporter1.ExcelExportButton.Click += ExcelExportButton_Click;
			ReportPDFExporter1.PdfExportButton.Click += PdfExportButton_Click;
			
		}

		protected override void Page_Load(object sender, EventArgs e)
		{
			base.Page_Load(sender, e);
			
			if (!Page.IsPostBack)
			{
				// javascript-события
				CheckFact.Attributes.Add("onclick", string.Format("uncheck('{0}')", CheckPlan.ClientID));
				CheckPlan.Attributes.Add("onclick", string.Format("uncheck('{0}')", CheckFact.ClientID));
				ArgScale.Attributes.Add("onclick", "document.getElementById('RefreshButton').className='Button';");

				// настройка асинхронной панели
				asyncChart.AddRefreshTarget(Chart);
				asyncChart.AddLinkedRequestTrigger(CheckFact.ClientID);
				asyncChart.AddLinkedRequestTrigger(CheckPlan.ClientID);

				// последний год, на который есть данные
				DataTable table = DataProvider.GetDataTableForChart("FO_0002_0002_Settlement_last_period", DataProvidersFactory.PrimaryMASDataProvider);
				if (table.Rows.Count > 0 && table.Rows[0][0] != DBNull.Value)
				{
					lastYear = firstYear;
					// [Период__Период].[Период__Период].[Данные всех периодов].[2011].[Полугодие 2].[Квартал 3].[Сентябрь]
					MatchCollection matches = Regex.Matches(table.Rows[0][0].ToString(), @"\[([^\]]*)\]");
					if (matches.Count > 3)
					{
						lastYear = Convert.ToInt32(matches[3].Groups[1].Value);
					}
					if (matches.Count > 6)
					{
						lastMonth = matches[6].Groups[1].Value;
					}
				}

				// параметр - год
				ArgYear.Title = "Год";
				ArgYear.Width = 100;
				ArgYear.MultiSelect = false;
				ArgYear.ParentSelect = false;
				ArgYear.ShowSelectedValue = true;
				ArgYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, lastYear));
				ArgYear.SelectLastNode();

				// параметр - месяц
				ArgMonth.Title = "Месяц";
				ArgMonth.Width = 150;
				ArgMonth.MultiSelect = false;
				ArgMonth.ParentSelect = false;
				ArgMonth.ShowSelectedValue = true;
				ArgMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
				ArgMonth.SetСheckedState(lastMonth, true);

				// параметр - раздел отчета
				ArgArea.Title = "Муниципальный район";
				ArgArea.Width = 300;
				ArgArea.MultiSelect = false;
				ArgArea.ParentSelect = false;
				ArgArea.ShowSelectedValue = true;
				digestAreas = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "FO_0002_0002_Settlement_filter_area");
				ArgArea.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(digestAreas.UniqueNames, digestAreas.MemberLevels));
			}

			// параметры для запроса
			paramPeriod.Value = String.Format(
				"[Период__Период].[Период__Период].[Данные всех периодов].[{0}].[Полугодие {1}].[Квартал {2}].[{3}]", 
				ArgYear.SelectedValue,
				CRHelper.HalfYearNumByMonthNum(ArgMonth.SelectedIndex + 1),
				CRHelper.QuarterNumByMonthNum(ArgMonth.SelectedIndex + 1),
				ArgMonth.SelectedValue);
			paramArea.Value = digestAreas.GetMemberUniqueName(ArgArea.SelectedValue);
			paramScale.Value = ArgScale.SelectedIndex == 0 ? "1000" : "1000000";

			if (!asyncChart.IsAsyncPostBack)
			{
				// текстовики
				Page.Title = "Анализ результатов исполнения бюджетов поселений";
				PageTitle.Text = Page.Title;
				PageSubTitle.Text = String.Format(
					"{0} район, данные за {1} {2} {3} года, {4}",
					ArgArea.SelectedValue,
					ArgMonth.SelectedIndex + 1,
					CRHelper.RusManyMonthGenitive(ArgMonth.SelectedIndex + 1),
					ArgYear.SelectedValue,
					ArgScale.SelectedValue
					);
				LabelChart.Text = "Распределение по объему профицита (+)/дефицита (-) поселений";
			}

			// погнали!

			if (!asyncChart.IsAsyncPostBack)
			{
				GridHelper grid = new GridHelper(this);
				grid.Init(GridBrick, "FO_0002_0002_Settlement_grid", DataProvidersFactory.PrimaryMASDataProvider);
			}

			paramMeasure.Value = 
				CheckPlan.Checked 
				? "[Measures].[Годовые назначения]" 
				: "[Measures].[Факт]";

			ChartHelper chart = new ChartHelper(this);
			chart.Init(Chart, "FO_0002_0002_Settlement_chart", DataProvidersFactory.PrimaryMASDataProvider);
		}
		

		#region Экспорт
		
		private void PdfExportButton_Click(object sender, EventArgs e)
		{
			Report report = new Report();
			ISection section1 = report.AddSection();
			ISection section2 = report.AddSection();

			ReportPDFExporter1.HeaderCellHeight = 20;

			ReportPDFExporter1.PageTitle = PageTitle.Text;
			ReportPDFExporter1.Export(GridBrick.GridHeaderLayout, section1);
			Chart.Width = 1000;
			ReportPDFExporter1.Export(Chart, LabelChart.Text, section2);

		}

		private void ExcelExportButton_Click(object sender, EventArgs e)
		{
			Workbook workbook = new Workbook();
			Worksheet sheet1 = workbook.Worksheets.Add("sheet1");
			Worksheet sheet2 = workbook.Worksheets.Add("sheet2");

			ReportExcelExporter1.SheetColumnCount = 15;
			ReportExcelExporter1.HeaderCellHeight = 20;
			ReportExcelExporter1.GridColumnWidthScale = 1.3;
			ReportExcelExporter1.RowsAutoFitEnable = true;

			ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
			ReportExcelExporter1.Export(GridBrick.GridHeaderLayout, sheet1, 3);
			Chart.Width = 1000;
			ReportExcelExporter1.Export(Chart, LabelChart.Text, sheet2, 3);
		}

		#endregion

		public class GridHelper : GridHelperBase
		{
			private Default Page;

			public GridHelper(Default page)
			{
				Page = page;
				HiddenColumns = 3;
			}

			protected override void SetStyle()
			{
				base.SetStyle();
				Grid.AutoSizeStyle = GridAutoSizeStyle.Auto;
			}

			protected override void SetData(string queryName, DataProvider provider)
			{
				Data = new DataTable();
				Data.Columns.Add(new DataColumn("показатель", typeof(string)));
				Data.Columns.Add(new DataColumn("годовые назначения", typeof(double)));
				Data.Columns.Add(new DataColumn("факт", typeof(double)));
				Data.Columns.Add(new DataColumn("ХинтГод", typeof(string)));
				Data.Columns.Add(new DataColumn("ХинтФакт", typeof(string)));
				Data.Columns.Add(new DataColumn("УровеньИерархии", typeof(int)));

				Page.paramMeasure.Value = "[Measures].[Годовые назначения]";
				DataTable table1 = DataProvider.GetDataTableForChart(queryName, provider);
				Page.paramMeasure.Value = "[Measures].[Факт]";
				DataTable table2 = DataProvider.GetDataTableForChart(queryName, provider);

				for (int i = 6; i <= 8; i++)
				{
					table1.Rows[0][i] = table1.Rows[0][i].ToString().Replace(",", ", \n");
					table2.Rows[0][i] = table2.Rows[0][i].ToString().Replace(",", ", \n");
				}

				Data.Rows.Add(new[]
				              	{
				              		"Количество поселений района, всего", 
									table1.Rows[0][0], table2.Rows[0][0], String.Empty, String.Empty, 0
				              	});
				Data.Rows.Add(new[]
				              	{
				              		"Профицитные", 
									table1.Rows[0][1], table2.Rows[0][1], table1.Rows[0][6], table2.Rows[0][6], 1
				              	});
				Data.Rows.Add(new[]
				              	{
				              		"Дефицитные", 
									table1.Rows[0][2], table2.Rows[0][2], table1.Rows[0][7], table2.Rows[0][7], 1
				              	});
				Data.Rows.Add(new[]
				              	{
				              		"Сбалансированные",
				              		table1.Rows[0][3], table2.Rows[0][3], table1.Rows[0][8], table2.Rows[0][8], 1
				              	});
				Data.Rows.Add(new[]
				              	{
				              		("Объем профицита, " + Page.ArgScale.SelectedValue), 
									table1.Rows[0][4], table2.Rows[0][4], String.Empty, String.Empty, 0
				              	});
				Data.Rows.Add(new[]
				              	{
				              		("Объем дефицита, " + Page.ArgScale.SelectedValue), 
									table1.Rows[0][5], table2.Rows[0][5], String.Empty, String.Empty, 0
				              	});
				
				Grid.DataTable = Data;
			}

			protected override void SetDataStyle()
			{
				Band.Columns[0].CellStyle.Wrap = true;
				Band.Columns[0].Width = CRHelper.GetColumnWidth(250);
				Band.Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

				Band.HideColumns(HiddenColumns);

				for (int i = 1; i < Band.Columns.Count - HiddenColumns; i++)
				{
					Band.Columns[i].Width = CRHelper.GetColumnWidth(180);
					Band.Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
				}

			}
			
			protected override void InitializeRow(object sender, RowEventArgs e)
			{
				UltraGridRow row = e.Row;
				
				// профицит, зеленый
				if (row.Index == 1 || row.Index == 4)
				{
					row.Style.ForeColor = Color.Green;
				}
				
				// дефицит, красный
				if (row.Index == 2 || row.Index == 5)
				{
					row.Style.ForeColor = Color.Red;
				}
				
				for (int i = 1; i < row.Cells.Count - HiddenColumns; i++)
				{
					UltraGridCell cell = row.Cells[i];
					cell.Style.Padding.Right = 8;
					
					// формат
					if (row.Index == 4 || row.Index == 5)
					{
						double value;
						if (cell.Value != null && Double.TryParse(cell.Value.ToString(), out value))
						{
							cell.Value = value.ToString("N3");
						}
					}
				}
				
			}

			protected override void SetDataRules()
			{
				// сдвиг текста согласно иерархии
				Grid.AddIndicatorRule(new PaddingRule(0, "УровеньИерархии", 10));

				// хинты
				Grid.AddIndicatorRule(new HintRule(1, "ХинтГод"));
				Grid.AddIndicatorRule(new HintRule(2, "ХинтФакт"));
			}

			protected override void SetDataHeader()
			{
				GridHeaderLayout headerLayout = Grid.GridHeaderLayout;
				
				headerLayout.AddCell("Показатель");
				headerLayout.AddCell("Уточненные годовые назначения");
				headerLayout.AddCell("Факт");
				
				Grid.GridHeaderLayout.ApplyHeaderInfo();
			}
		}
		
		public class ChartHelper : ChartHelperBase
		{
			private Default Page;

			public ChartHelper(Default page)
			{
				Page = page;
			}

			protected override void SetStyle()
			{
				Chart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 20);
				Chart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.55);
				
				Chart.ChartType = ChartType.ColumnChart;

				base.SetStyle();

				Chart.Axis.X.Extent = 140;

				Chart.Axis.X.StripLines.Visible = true;
				Chart.Axis.X.StripLines.Interval = 2;
				Chart.Axis.X.StripLines.PE.Fill = Color.Gainsboro;
				Chart.Axis.X.StripLines.PE.FillOpacity = 150;
				Chart.Axis.X.StripLines.PE.Stroke = Color.DarkGray;
				
				Chart.Axis.X.Labels.Visible = false;
				Chart.Axis.X.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.Auto;
				Chart.Axis.X.Labels.SeriesLabels.Visible = true;
				Chart.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;
				Chart.Axis.X.Labels.SeriesLabels.VerticalAlign = StringAlignment.Near;
				Chart.Axis.X.Labels.SeriesLabels.Font = new Font("Verdana", 8);
				Chart.Axis.X.Labels.SeriesLabels.FontSizeBestFit = false;

				Chart.Axis.Y.Extent = 50;
				Chart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
				Chart.Axis.Y.TickmarkStyle = AxisTickStyle.Smart;
				
				Chart.Legend.Margins.Right = 3 * Convert.ToInt32(Chart.Width.Value) / 4;
				Chart.Legend.Visible = true;
				Chart.Legend.Location = LegendLocation.Top;
				Chart.Legend.SpanPercentage = 8;

				Chart.TitleLeft.Visible = true;
				Chart.TitleLeft.HorizontalAlign = StringAlignment.Center;
				Chart.TitleLeft.Margins.Bottom = Convert.ToInt32(Chart.Height.Value) / 4;
				Chart.TitleLeft.Font = defaultFont;
				Chart.TitleLeft.Text = CRHelper.ToUpperFirstSymbol(Page.ArgScale.SelectedValue);

				Chart.Tooltips.FormatString = "<SERIES_LABEL>\n<ITEM_LABEL>";
				Chart.ColorModel.ModelStyle = ColorModels.CustomLinear;
				
			}

			protected override void DataBinding(object sender, EventArgs e)
			{
				Data = DataProvider.GetDataTableForChart(QueryName, Provider);
				if (Data.Rows.Count > 0)
				{
					Data.Columns[1].ColumnName = 
						Page.CheckFact.Checked 
						? "Фактический профицит/дефицит" 
						: "Плановый профицит/дефицит";

					Chart.DataSource = Data;
				}
			}

			protected override void FillSceneGraph(object sender, FillSceneGraphEventArgs e)
			{
				for (int i = 0; i < e.SceneGraph.Count; i++)
				{
					Primitive primitive = e.SceneGraph[i];
					if (primitive is Box)
					{
						Box box = (Box)primitive;
						if (box.DataPoint != null && box.Value != null)
						{
							double value = Convert.ToDouble(box.Value);
							if (value > 0)
							{
								box.DataPoint.Label = String.Format(
									"{0} {1:N2} {2}",
									Page.CheckFact.Checked 
										? "Фактический профицит" 
										: "Плановый профицит (годовой)",
									value, Page.ArgScale.SelectedValue);
								box.PE.ElementType = PaintElementType.Gradient;
								box.PE.FillGradientStyle = GradientStyle.Horizontal;
								box.PE.Fill = Color.Green;
								box.PE.FillStopColor = Color.ForestGreen;
							}
							else
							{
								box.DataPoint.Label = String.Format("{0} {1:N2} {2}",
									Page.CheckFact.Checked 
										? "Фактический дефицит" 
										: "Плановый дефицит (годовой)",
									value, Page.ArgScale.SelectedValue);
								box.PE.ElementType = PaintElementType.Gradient;
								box.PE.FillGradientStyle = GradientStyle.Horizontal;
								box.PE.Fill = Color.Red;
								box.PE.FillStopColor = Color.Maroon;
							}
						}
						else if (box.Path != null && box.Path.ToLower().Contains("legend") && box.rect.Width < 20)
						{
							box.PE.ElementType = PaintElementType.CustomBrush;
							LinearGradientBrush brush = new LinearGradientBrush(box.rect, Color.Green, Color.Red, 45, false);
							box.PE.CustomBrush = brush;
						}
					}
				}
			}

		}

	}

	
}
