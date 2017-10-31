﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Infragistics.WebUI.UltraWebNavigator;

using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Infragistics.Documents.Reports.Report.Text;
using System.IO;
using System.Drawing.Imaging;

/**
 *  Мониторинг ситуации на рынке труда в субъекте РФ по состоянию на ЧЧ.ММ.ГГГГ
 */
namespace Krista.FM.Server.Dashboards.reports.STAT_0003_0002
{
	public partial class Default : CustomReportPage
	{
		#region Поля

		private DataTable dtDate;
		private DataTable dtGrid;
		private DataTable dtGridDescription;
		private DataTable dtChart1;
		private DataTable dtChart2;
		private DataTable dtRegion;
		private GridHeaderLayout headerLayout;

		#endregion

		// имя папки с картами региона
		private const string mapFolderName = "Сахалинской области";

		private static bool IsMozilla
		{
			get { return HttpContext.Current.Request.Browser.Browser == "Firefox"; }
		}

		private static string Browser
		{
			get { return HttpContext.Current.Request.Browser.Browser; }
		}

		#region Параметры запроса

		private CustomParam selectedDate;
		private CustomParam compareMode;
		private CustomParam selectedRegion;
		private CustomParam lastYear;

		#endregion
		// --------------------------------------------------------------------

		private const string PageTitleCaption = "Мониторинг ситуации на рынке труда ({0})";
		private const string PageSubTitleCaption = "Данные ежемесячного мониторинга ситуации на рынке труда в Сахалинской области по выбранному муниципальному образованию";
		private const string Chart1TitleCaption = "Динамика уровня зарегистрированной безработицы, {0}";
		private const string Chart2TitleCaption = "Динамика численности зарегистрированных безработных в расчете на одну вакансию, {0}";

		// --------------------------------------------------------------------

		protected override void Page_PreLoad(object sender, EventArgs e)
		{
			base.Page_PreLoad(sender, e);

			ComboDate.Title = "Дата";
			ComboDate.Width = 200;
			ComboDate.ParentSelect = true;
			ComboDate.MultiSelect = true;

			ComboCompareMode.Title = "Период для сравнения";
			ComboCompareMode.Width = 300;
			ComboCompareMode.ParentSelect = true;

			ComboRegion.Title = "Территория";
			ComboRegion.Width = 300;
			ComboRegion.ParentSelect = true;

			UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 12);
			UltraWebGrid.Height = 400;
			UltraWebGrid.DataBinding += new EventHandler(UltraWebGrid_DataBinding);
			UltraWebGrid.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid_InitializeLayout);
			UltraWebGrid.InitializeRow += new Infragistics.WebUI.UltraWebGrid.InitializeRowEventHandler(UltraWebGrid_InitializeRow);
			UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";
			LabelText.Width = UltraWebGrid.Width;
             
			#region Настройка диаграммы 1

			UltraChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 17);
			UltraChart1.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.5);

            UltraChart1.ChartType = //ChartType.StackAreaChart;
                ChartType.AreaChart;
			UltraChart1.Border.Thickness = 0;

			UltraChart1.Tooltips.FormatString = "<ITEM_LABEL>";
			UltraChart1.Axis.X.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.None;
			UltraChart1.Axis.X.Extent = 60;
			UltraChart1.Axis.X.Labels.Font = new Font("Verdana", 8);
			UltraChart1.Axis.X.Labels.FontColor = Color.Black;
			UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N2>%";
			UltraChart1.Axis.Y.Labels.Font = new Font("Verdana", 8);
			UltraChart1.Axis.Y.Labels.FontColor = Color.Black;
			UltraChart1.Axis.Y.Extent = 40;

			/*UltraChart1.TitleLeft.Visible = true;
			UltraChart1.TitleLeft.Text = "Уровень безработицы";
			UltraChart1.TitleLeft.HorizontalAlign = StringAlignment.Center;
			UltraChart1.TitleLeft.Orientation = TextOrientation.VerticalLeftFacing;
			UltraChart1.TitleLeft.Margins.Bottom = UltraChart1.Axis.X.Extent + 5;
			UltraChart1.TitleLeft.Font = new Font("Verdana", 10);
			UltraChart1.TitleLeft.FontColor = Color.Black;*/

			UltraChart1.Data.EmptyStyle.Text = " ";
			UltraChart1.EmptyChartText = " ";

			UltraChart1.AreaChart.NullHandling = NullHandling.DontPlot;

			UltraChart1.AreaChart.LineAppearances.Clear();

			LineAppearance lineAppearance = new LineAppearance();
			lineAppearance.IconAppearance.Icon = SymbolIcon.Circle;
			lineAppearance.IconAppearance.IconSize = SymbolIconSize.Small;
			lineAppearance.Thickness = 5;
			UltraChart1.AreaChart.LineAppearances.Add(lineAppearance);

			lineAppearance = new LineAppearance();
			lineAppearance.IconAppearance.Icon = SymbolIcon.Circle;
			lineAppearance.IconAppearance.IconSize = SymbolIconSize.Small;
			lineAppearance.Thickness = 3;
			UltraChart1.AreaChart.LineAppearances.Add(lineAppearance);

			lineAppearance = new LineAppearance();
			lineAppearance.IconAppearance.Icon = SymbolIcon.None;
			lineAppearance.IconAppearance.IconSize = SymbolIconSize.Small;
			lineAppearance.Thickness = 5;
			lineAppearance.LineStyle.MidPointAnchors = false;
			UltraChart1.AreaChart.LineAppearances.Add(lineAppearance);

			UltraChart1.Legend.Visible = true;
			UltraChart1.Legend.Location = LegendLocation.Top;
			UltraChart1.Legend.SpanPercentage = 10;
			UltraChart1.Legend.Font = new Font("Verdana", 10);

			UltraChart1.InvalidDataReceived +=
				new Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventHandler(
					CRHelper.UltraChartInvalidDataReceived);
			UltraChart1.FillSceneGraph +=
				new Infragistics.UltraChart.Shared.Events.FillSceneGraphEventHandler(UltraChart1_FillSceneGraph);
			UltraChart1.DataBinding += new EventHandler(UltraChart1_DataBinding);

			UltraChart1.ColorModel.ModelStyle = ColorModels.CustomSkin;
			UltraChart1.ColorModel.Skin.PEs.Clear();
			for (int i = 1; i <= 3; i++)
			{
				PaintElement pe = new PaintElement();
				Color color = Color.White;
				Color stopColor = Color.White;
				PaintElementType peType = PaintElementType.Gradient;
				switch (i)
				{
					case 1:
						{
							color = Color.Transparent;
							stopColor = Color.Gray;
							peType = PaintElementType.Hatch;
							pe.Hatch = FillHatchStyle.ForwardDiagonal;
							break;
						}
					case 2:
						{
							color = Color.MediumSeaGreen;
							stopColor = Color.Green;
							peType = PaintElementType.Gradient;
							break;
						}
				}
				pe.Fill = color;
				pe.FillStopColor = stopColor;
				pe.ElementType = peType;
				pe.FillGradientStyle = GradientStyle.ForwardDiagonal;
				pe.FillOpacity = (byte)150;
				pe.FillStopOpacity = (byte)150;
				if (i == 3)
				{
					pe.Stroke = Color.Red;
					pe.StrokeWidth = 5;
				}
				UltraChart1.ColorModel.Skin.PEs.Add(pe);
			}

			#endregion

			#region Настройка диаграммы 2
             
			UltraChart2.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 17);
			UltraChart2.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.5);

			UltraChart2.ChartType = ChartType.AreaChart;
			UltraChart2.Border.Thickness = 0;

			UltraChart2.Tooltips.FormatString = "<ITEM_LABEL>";
			UltraChart2.Axis.X.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.None;
			UltraChart2.Axis.X.Extent = 60;
			UltraChart2.Axis.X.Labels.Font = new Font("Verdana", 8);
			UltraChart2.Axis.X.Labels.FontColor = Color.Black;
			UltraChart2.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N2>";
			UltraChart2.Axis.Y.Labels.Font = new Font("Verdana", 8);
			UltraChart2.Axis.Y.Labels.FontColor = Color.Black;
			UltraChart2.Axis.Y.Extent = 40;

			/*UltraChart2.TitleLeft.Visible = true;
			UltraChart2.TitleLeft.Text = "Численность зарегистрированных\nбезработных в расчете на одну вакансию";
			UltraChart2.TitleLeft.HorizontalAlign = StringAlignment.Center;
			UltraChart2.TitleLeft.Orientation = TextOrientation.VerticalLeftFacing;
			UltraChart2.TitleLeft.Margins.Bottom = UltraChart2.Axis.X.Extent + 5;
			UltraChart2.TitleLeft.Font = new Font("Verdana", 10);
			UltraChart2.TitleLeft.FontColor = Color.Black;*/

			UltraChart2.Data.EmptyStyle.Text = " ";
			UltraChart2.EmptyChartText = " ";

			UltraChart2.AreaChart.NullHandling = NullHandling.DontPlot;

			UltraChart2.AreaChart.LineAppearances.Clear();

			lineAppearance = new LineAppearance();
			lineAppearance.IconAppearance.Icon = SymbolIcon.Circle;
			lineAppearance.IconAppearance.IconSize = SymbolIconSize.Small;
			lineAppearance.Thickness = 5;
			UltraChart2.AreaChart.LineAppearances.Add(lineAppearance);

			lineAppearance = new LineAppearance();
			lineAppearance.IconAppearance.Icon = SymbolIcon.Circle;
			lineAppearance.IconAppearance.IconSize = SymbolIconSize.Small;
			lineAppearance.Thickness = 3;
			UltraChart2.AreaChart.LineAppearances.Add(lineAppearance);

			lineAppearance = new LineAppearance();
			lineAppearance.IconAppearance.Icon = SymbolIcon.None;
			lineAppearance.IconAppearance.IconSize = SymbolIconSize.Small;
			lineAppearance.Thickness = 5;
			lineAppearance.LineStyle.MidPointAnchors = false;
			UltraChart2.AreaChart.LineAppearances.Add(lineAppearance);

			UltraChart2.Legend.Visible = true;
			UltraChart2.Legend.Location = LegendLocation.Top;
			UltraChart2.Legend.SpanPercentage = 15;
			UltraChart2.Legend.Font = new Font("Verdana", 10);

			UltraChart2.InvalidDataReceived +=
				new Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventHandler(
					CRHelper.UltraChartInvalidDataReceived);
			UltraChart2.FillSceneGraph +=
				new Infragistics.UltraChart.Shared.Events.FillSceneGraphEventHandler(UltraChart2_FillSceneGraph);
			UltraChart2.DataBinding += new EventHandler(UltraChart2_DataBinding);

			UltraChart2.ColorModel.ModelStyle = ColorModels.CustomSkin;
			UltraChart2.ColorModel.Skin.PEs.Clear();
			for (int i = 1; i <= 3; i++)
			{
				PaintElement pe = new PaintElement();
				Color color = Color.White;
				Color stopColor = Color.White;
				PaintElementType peType = PaintElementType.Gradient;
				switch (i)
				{
					case 1:
						{
							color = Color.Transparent;
							stopColor = Color.Gray;
							peType = PaintElementType.Hatch;
							pe.Hatch = FillHatchStyle.ForwardDiagonal;
							break;
						}
					case 2:
						{
							color = Color.MediumSeaGreen;
							stopColor = Color.Green;
							peType = PaintElementType.Gradient;
							break;
						}
				}
				pe.Fill = color;
				pe.FillStopColor = stopColor;
				pe.ElementType = peType;
				pe.FillGradientStyle = GradientStyle.ForwardDiagonal;
				pe.FillOpacity = (byte)150;
				pe.FillStopOpacity = (byte)150;
				if (i == 3)
				{
					pe.Stroke = Color.Red;
					pe.StrokeWidth = 5;
				}
				UltraChart2.ColorModel.Skin.PEs.Add(pe);
			}

			#endregion 

			#region Параметры
			selectedDate = UserParams.CustomParam("selected_date");
			compareMode = UserParams.CustomParam("compare_mode");
			selectedRegion = UserParams.CustomParam("selected_region");
			lastYear = UserParams.CustomParam("last_year");
			#endregion

			#region Ссылки
			CrossLink1.Text = "Мониторинг&nbspситуации&nbspна&nbspрынке&nbspтруда Сахалинской области";
			CrossLink1.NavigateUrl = "~/reports/STAT_0003_0001_Sahalin/Default.aspx";
			#endregion

			#region Экспорт
			
			ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
			ReportExcelExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
			ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click); ;
			
			#endregion
		}

		// --------------------------------------------------------------------

		protected override void Page_Load(object sender, EventArgs e)
		{
			base.Page_Load(sender, e);
			if (!Page.IsPostBack)
			{
				FillComboDate("STAT_0003_0002_list_of_dates");
				SetComboSelection(ComboDate);
				FillComboByCustomValues(ComboCompareMode);
				FillComboRegion("STAT_0003_0002_list_of_regions");
			}
			#region Анализ параметров
			selectedDate.Value = String.Empty;
			foreach (Node node in ComboDate.SelectedNodes)
			{
				if (node.Level == 2)
				{
					selectedDate.Value = AddStringWithSeparator(selectedDate.Value, StringToMDXDate(node.Text), ",\n");
					lastYear.Value = GetYearFromMDXDate(StringToMDXDate(node.Text));
				}
				else if (node.Level == 1)
				{
					foreach (Node dayNode in node.Nodes)
					{
						selectedDate.Value = AddStringWithSeparator(selectedDate.Value, StringToMDXDate(dayNode.Text), ",\n");
						lastYear.Value = GetYearFromMDXDate(StringToMDXDate(dayNode.Text));
					}
				}
				else if (node.Level == 0)
				{
					foreach (Node monthNode in node.Nodes)
						foreach (Node dayNode in monthNode.Nodes)
						{
							selectedDate.Value = AddStringWithSeparator(selectedDate.Value, StringToMDXDate(dayNode.Text), ",\n");
							lastYear.Value = GetYearFromMDXDate(StringToMDXDate(dayNode.Text));
						}
				}
			}
			selectedRegion.Value = ComboRegion.SelectedValue;
			compareMode.Value = ComboCompareMode.SelectedValue;
			#endregion

            //return;

			PageTitle.Text = String.Format(PageTitleCaption, selectedRegion.Value);
			Page.Title = PageTitle.Text;
			PageSubTitle.Text = PageSubTitleCaption;
			
			headerLayout = new GridHeaderLayout(UltraWebGrid);
			UltraWebGrid.DataBind();
			MakeDynamicText();
			UltraChart1.DataBind();
			UltraChart2.DataBind();
		}   

		#region Динамический текст

		protected void MakeDynamicText()
		{
            
			string query = DataProvider.GetQueryText("STAT_0003_0002_text");
			DataTable dtText = new DataTable();
			DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Период", dtText);
			DataRow row0 = dtGrid.Rows[dtGrid.Rows.Count - 4];
			DataRow row1 = dtGrid.Rows[dtGrid.Rows.Count - 3];
			DataRow row2 = dtGrid.Rows[dtGrid.Rows.Count - 2]; 
			string text1 = "Численность безработных граждан, зарегистрированных в органах службы занятости, по состоянию на <b>{0}</b> составила <b>{1:N3}</b> тыс.человек. {2} числа безработных граждан за период с <b>{3}</b> по <b>{0}</b> в целом по выбранному муниципальному образванию составил{4} <b>{5:N0}</b> чел. (темп {6}<b>{7:P2}</b>).";
			string text2 = "<br/>Уровень регистрируемой безработицы в течение отчетного периода {0} на <b>{1:N2}</b> процентных пункта и на <b>{2}</b> составил <b>{3:N2}%</b>.";   
            string text3 = "<br/>Численность населения на начало года составила <b>{0:N3}</b> тыс.чел., численность экономически активного населения на отчетную дату составила <b>{1:N3}</b> тыс.чел. (<b>{2:P2}</b> от численности населения).";
             
			object[] values1 = new object[8];
			values1[0] = dtGridDescription.Rows[dtGridDescription.Rows.Count - 1][0].ToString();
			values1[1] = Convert.ToDouble(row0[2].ToString()) / 1000;
			if (Convert.ToDouble(row2[2].ToString()) <= 0)
			{
				values1[2] = "Снижение <img src='../../images/ArrowGreenDownBB.png'>";
				values1[4] = "о";
				values1[6] = "снижения ";
			}
			else
			{
				values1[2] = "Прирост <img src='../../images/ArrowRedUpBB.png'>";
				values1[4] = String.Empty;
				values1[6] = "прироста +";
			}
			values1[3] = dtGridDescription.Rows[dtGridDescription.Rows.Count - 1][1].ToString();
			values1[5] = Convert.ToDouble(row2[2].ToString());
			values1[7] = Convert.ToDouble(row1[2].ToString());

			object[] values2 = new object[4];
			if (Convert.ToDouble(row1[3].ToString()) <= 0)
			{
				values2[0] = "уменьшился <img src='../../images/ArrowGreenDownBB.png'>";
			}
			else
			{
				values2[0] = "увеличился <img src='../../images/ArrowRedUpBB.png'>";
			}
			values2[1] = Convert.ToDouble(row1[3].ToString());
			values2[2] = dtGridDescription.Rows[dtGridDescription.Rows.Count - 1][0].ToString();
			values2[3] = Convert.ToDouble(row0[3].ToString());

			object[] values3 = new object[3];
			values3[0] = Convert.ToDouble(dtText.Rows[0][0]);
			values3[1] = Convert.ToDouble(row0[1].ToString()) / 1000;
			values3[2] = Convert.ToDouble(row0[1].ToString()) / Convert.ToDouble(dtText.Rows[0][0]) / 1000;

			LabelText.Text = String.Format(text1, values1) + String.Format(text2, values2) + String.Format(text3, values3);
		}

		#endregion

		// --------------------------------------------------------------------
		#region Обработчики грида

		protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
		{
			string query = DataProvider.GetQueryText("STAT_0003_0002_grid");
			dtGrid = new DataTable();
			DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Период", dtGrid);
			query = DataProvider.GetQueryText("STAT_0003_0002_grid_description");
			dtGridDescription = new DataTable();
			DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Даты для грида", dtGridDescription);
			foreach (DataRow row in dtGridDescription.Rows)
			{
				row[0] = MDXDateToShortDateString(row[1].ToString());
				row[1] = MDXDateToShortDateString(row[2].ToString());
			}
			if (dtGrid.Rows.Count > 0)
			{
				UltraWebGrid.DataSource = dtGrid;
			}
			else
			{
				UltraWebGrid.DataSource = null;
			}
		}

		protected void UltraWebGrid_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
		{
			e.Layout.AllowSortingDefault = AllowSorting.No;
			e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
			e.Layout.NullTextDefault = "-";
			e.Layout.RowAlternateStylingDefault = Infragistics.WebUI.Shared.DefaultableBoolean.False;
			e.Layout.Bands[0].Columns[0].MergeCells = true;
			e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(75);
			e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Center;
			int columnWidth = Convert.ToInt32((Convert.ToInt32(e.Layout.Grid.Width.Value) - CRHelper.GetColumnWidth(200)) / 7);
			// Заголовки
			e.Layout.HeaderStyleDefault.Wrap = true;
			e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
			e.Layout.HeaderStyleDefault.VerticalAlign = VerticalAlign.Middle;
			headerLayout.AddCell("Дата");
			for (int i = 1; i < e.Layout.Bands[0].Columns.Count; ++i)
			{
				e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(columnWidth);
				e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
				e.Layout.Bands[0].Columns[i].CellStyle.Padding.Right = 5;
				e.Layout.Bands[0].Columns[i].CellStyle.Padding.Left = 5;
				headerLayout.AddCell(e.Layout.Bands[0].Columns[i].Key);
                e.Layout.Bands[0].Columns[i].DataType = "decimal";
			}
			headerLayout.ApplyHeaderInfo();

            
		}

		protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
		{
			string date = dtGridDescription.Rows[e.Row.Index / 4][0].ToString();
			string cDate = dtGridDescription.Rows[e.Row.Index / 4][1].ToString();
			e.Row.Cells[0].Value = date;
			for (int i = 1; i < e.Row.Cells.Count; ++i)
			{
				UltraGridCell cell = e.Row.Cells[i];
				if (e.Row.Index % 4 != 0)
				{
					cell.Style.BorderDetails.StyleTop = BorderStyle.None;
				}
				if (e.Row.Index % 4 != 3)
				{
					cell.Style.BorderDetails.StyleBottom = BorderStyle.None;
				}
				if (e.Row.Index % 4 == 1 && i != 7)
				{
					if (String.IsNullOrEmpty(cDate))
					{
						cell.Title = null;
					}
					else
					{
						cell.Title = "Темп прироста к " + cDate;
					}
					if (cell.Value != null && Convert.ToDouble(cell.Value) != 0)
					{
						double value = Convert.ToDouble(cell.Value);
						if (value < 0)
						{
							if (i == 2 || i == 3 || i == 4 || i == 6)
							{
								cell.Style.BackgroundImage = "~/images/ArrowGreenDownBB.png";
								cell.Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
							}
							else if (i != 7)
							{
								cell.Style.BackgroundImage = "~/images/ArrowRedDownBB.png";
								cell.Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
							}
						}
						else
						{
							if (i == 2 || i == 3 || i == 4 || i == 6)
							{
								cell.Style.BackgroundImage = "~/images/ArrowRedUpBB.png";
								cell.Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
							}
							else if (i != 7)
							{
								cell.Style.BackgroundImage = "~/images/ArrowGreenUpBB.png";
								cell.Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
							}
						}
					}
				}
				if (e.Row.Index % 4 == 0 && i == 7)
				{
					if (cell.Value != null && Convert.ToDouble(cell.Value) != 0)
					{
						if (Convert.ToDouble(cell.Value) > 0)
						{
							cell.Style.BackgroundImage = "~/images/ballGreenBB.png";
							cell.Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
							cell.Title = "Профицит";
						}
						else
						{
							cell.Style.BackgroundImage = "~/images/ballRedBB.png";
							cell.Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
							cell.Title = "Дефицит";
						}
					}
				}
				if (e.Row.Index % 4 == 2 )
				{
					if (cell.Value != null && Convert.ToDouble(cell.Value) != 0)
					{
						double value = Convert.ToDouble(cell.Value);
						if (value > 0)
						{
							if (String.IsNullOrEmpty(cDate))
							{
								cell.Title = null;
							}
							else
							{
								cell.Title = "Прирост к " + cDate;
							}
						}
						else
						{
							if (String.IsNullOrEmpty(cDate))
							{
								cell.Title = null;
							}
							else
							{
								cell.Title = "Снижение относительно " + cDate;
							}
						}
					}
				}
				if ((i == 3 || i == 6) && (e.Row.Index % 4 == 3))
				{
					if (Convert.ToInt32(cell.Value) == 1)
					{
                        cell.Style.BackgroundImage = "~/images/StarYellowBB.png";
						
						cell.Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";

                        if (i == 3)
                        {
                            cell.Title = String.Format("Ранг по Сахалинской области на {1}: {0}\nСамый низкий уровень безработицы", cell.Value, date);
                        }
                        else
                        {
                            cell.Title = String.Format("Ранг по Сахалинской области на {1}: {0}\nСамая низкая численность безработных\nна одну вакансию", cell.Value, date);
                        }
					}
					else if (Convert.ToInt32(cell.Value) == ComboRegion.GetRootNodesCount())
					{
                        cell.Style.BackgroundImage = "~/images/StarGrayBB.png";
						cell.Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                        if (i == 3)
                        {
                            cell.Title = String.Format("Ранг по Сахалинской области на {1}: {0}\nСамый высокий уровень безработицы", cell.Value, date);
                        }
                        else
                        {
                            cell.Title = String.Format("Ранг по Сахалинской области на {1}: {0}\nСамая высокая численность безработных\nна одну вакансию", cell.Value, date);
                        }
					}
					else
					{
						cell.Title = String.Format("Ранг по Сахалинской области на {1}: {0}", cell.Value, date);
					}
				}
				switch (e.Row.Index % 4)
				{
					case 0:
						{
							if (i == 3 || i == 6)
							{
                                if (i == 6)
                                {
                                    cell.Value = String.Format("<b>{0:N1}</b>", cell.Value);
                                }
                                else
                                {
                                    cell.Value = String.Format("<b>{0:N2}</b>", cell.Value);
                                }								
							}
							else
							{
								cell.Value = String.Format("<b>{0:N0}</b>", cell.Value);
							}
							break;   
						}
					case 1:
						{
							cell.Value = String.Format("{0:P2}", cell.Value);
							break;
						}
					case 2:
						{
                            if (i == 3 || i == 6)
                            {
                                cell.Value = String.Format("{0:N2}", cell.Value);
                            }
                            else
                            { 
							    cell.Value = String.Format("{0:N0}", cell.Value);
                            }
							break;
						}
				}
			}
		}

		#endregion

		// --------------------------------------------------------------------

		#region Обработчики диаграммы 1

		protected void UltraChart1_DataBinding(object sender, EventArgs e)
		{
			LabelChart1.Text = String.Format(Chart1TitleCaption, selectedRegion.Value);
			string query = DataProvider.GetQueryText("STAT_0003_0002_chart1");
			dtChart1 = new DataTable();
			DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Дата", dtChart1);

			if (dtChart1.Rows.Count > 0)
			{
				dtChart1.Columns.RemoveAt(0);

				foreach (DataRow row in dtChart1.Rows)
				{
					if (row[0] != DBNull.Value)
					{
						row[0] = MDXDateToShortDateString(row[0].ToString());
					}
				}

				UltraChart1.Series.Clear();
				for (int i = 1; i < dtChart1.Columns.Count; i++)
				{
					NumericSeries series = CRHelper.GetNumericSeries(i, dtChart1);
					series.Label = dtChart1.Columns[i].ColumnName;
					UltraChart1.Series.Add(series);
				}

				//UltraChart.DataSource = dtChart;
			}
		}

		protected void UltraChart1_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
		{
			for (int i = 0; i < e.SceneGraph.Count; i++)
			{
				Primitive primitive = e.SceneGraph[i];

				if (primitive is Polyline)
				{
					Polyline polyline = (Polyline)primitive;
					foreach (DataPoint point in polyline.points)
					{
						if (point.Series != null)
						{
							string unit = "%";
							point.DataPoint.Label = string.Format("{2} на {3}\n {0:N2}{1}", ((NumericDataPoint)point.DataPoint).Value, unit, point.Series.Label, point.DataPoint.Label);

						}
					}
				}
			}
		}

		#endregion

		#region Обработчики диаграммы 2

		protected void UltraChart2_DataBinding(object sender, EventArgs e)
		{
			LabelChart2.Text = String.Format(Chart2TitleCaption, selectedRegion.Value);
			string query = DataProvider.GetQueryText("STAT_0003_0002_chart2");
			dtChart2 = new DataTable();
			DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Дата", dtChart2);

			if (dtChart2.Rows.Count > 0)
			{
				dtChart2.Columns.RemoveAt(0);

				foreach (DataRow row in dtChart2.Rows)
				{
					if (row[0] != DBNull.Value)
					{
						row[0] = MDXDateToShortDateString(row[0].ToString());
					}
				}

				UltraChart2.Series.Clear();
				for (int i = 1; i < dtChart2.Columns.Count; i++)
				{
					NumericSeries series = CRHelper.GetNumericSeries(i, dtChart2);
					series.Label = dtChart2.Columns[i].ColumnName;
					UltraChart2.Series.Add(series);
				}

				//UltraChart.DataSource = dtChart;
			}
		}

		protected void UltraChart2_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
		{
			for (int i = 0; i < e.SceneGraph.Count; i++)
			{
				Primitive primitive = e.SceneGraph[i];

				if (primitive is Polyline)
				{
					Polyline polyline = (Polyline)primitive;
					foreach (DataPoint point in polyline.points)
					{
						if (point.Series != null)
						{
							string label = String.Empty;
							if (point.Series.Label == "Численность зарегистрированных безработных в расчете на 1 вакансию в целом по Сахалинской области")
							{ 
								label = "Численность зарегистрированных безработных\nв расчете на 1 вакансию в целом по Сахалинской области";
							}
							else
							{
								label = point.Series.Label.Replace(",", ",\n");
							}
							point.DataPoint.Label = string.Format("{1} на {2}\n {0:N1}", ((NumericDataPoint)point.DataPoint).Value, label, point.DataPoint.Label);

						}
					}
				}
			}
		}

		#endregion

		// --------------------------------------------------------------------

		#region Заполнение словарей и выпадающих списков параметров

		/// <summary>
		/// Добаляет одну строку к другой с разделителем между ними. Если первая строка пустая, то разделитель не добавляется
		/// </summary>
		/// <param name="firstString">Строка к которой добавляем</param>
		/// <param name="secondString">Строка, которую добавляем</param>
		/// <param name="separator">Разделитель</param>
		/// <returns>Возвращает получившуюся в результате добавления строку</returns>
		protected string AddStringWithSeparator(string firstString, string secondString, string separator)
		{
			if (String.IsNullOrEmpty(firstString))
			{
				return secondString;
			}
			else
			{
				return firstString + separator + secondString;
			}
		}

		protected void SetComboSelection(CustomMultiCombo combo)
		{
			//for (int i = 0; i < combo.GetRootNodesCount(); ++i)
			//{
			combo.SetСheckedState(combo.GetRootNodesName(combo.GetRootNodesCount() - 1), true);
			//}
            //combo.SetСheckedState( , true);
            //combo.SelectLastNode();
		} 

		protected void FillComboByCustomValues(CustomMultiCombo combo)
		{
			Collection<string> collection = new Collection<string>();
			collection.Add("к началу года");
			collection.Add("к предыдущей дате"); 
			combo.FillValues(collection);
			combo.SelectLastNode();
		}
		
		protected void FillComboDate(string queryName)
		{
			// Загрузка списка актуальных дат
			dtDate = new DataTable();
			string query = DataProvider.GetQueryText(queryName);
			DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
			// Закачку придется делать через словарь
			Dictionary<string, int> dictDate = new Dictionary<string, int>();
			for (int row = 0; row < dtDate.Rows.Count; ++row)
			{
				string year = dtDate.Rows[row][0].ToString();
				string month = dtDate.Rows[row][3].ToString();
				string day = dtDate.Rows[row][4].ToString();
				AddPairToDictionary(dictDate, year + " год", 0);
				AddPairToDictionary(dictDate, month + " " + year + " года", 1);
				AddPairToDictionary(dictDate, day + " " + 
                    CRHelper.RusMonthGenitive(CRHelper.MonthNum(month)) + ' ' + year + " года", 2);
			}
			ComboDate.FillDictionaryValues(dictDate); 
		}

		protected void FillComboRegion(string queryName)
		{
			dtRegion = new DataTable();
			string query = DataProvider.GetQueryText(queryName);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Region", dtRegion); 
			Dictionary<string, int> dict = new Dictionary<string, int>();
			foreach (DataRow row in dtRegion.Rows)
			{
                AddPairToDictionary(dict, row["Region"].ToString(), 0);
			}
			ComboRegion.FillDictionaryValues(dict);
		}

		protected void AddPairToDictionary(Dictionary<string, int> dict, string key, int value)
		{
			if (!dict.ContainsKey(key))
			{
				dict.Add(key, value);
			}
		}

		#endregion

		#region Функции-полезняшки преобразования и все такое

		public string GetMaxMDXDate(string firstDate, string secondDate)
		{
			if (Convert.ToInt32(FormatMDXDate(firstDate, "{0}{1:00}{2:00}")) > Convert.ToInt32(FormatMDXDate(secondDate, "{0}{1:00}{2:00}")))
			{
				return firstDate;
			}
			else
			{
				return secondDate;
			}
		}

		public string FormatMDXDate(string mdxDate, string formatString, int yearIndex, int monthIndex, int dayIndex)
		{
			string[] separator = { "].[" };
			string[] dateElements = mdxDate.Split(separator, StringSplitOptions.None);
			if (dateElements.Length < 8)
			{
				return String.Format(formatString, 1998, 1, 1);
			}
			int year = Convert.ToInt32(dateElements[yearIndex]);
			int month = Convert.ToInt32(CRHelper.MonthNum(dateElements[monthIndex]));
			int day = Convert.ToInt32(dateElements[dayIndex]);
			return String.Format(formatString, year, month, day);
		}

		public string FormatMDXDate(string mdxDate, string formatString)
		{
			string[] separator = { "].[" };
			string[] dateElements = mdxDate.Split(separator, StringSplitOptions.None);
			if (dateElements.Length < 8)
			{
				return String.Format(formatString, 1998, 1, 1);
			}
			int year = Convert.ToInt32(dateElements[3]);
			int month = Convert.ToInt32(CRHelper.MonthNum(dateElements[6]));
			int day = Convert.ToInt32(dateElements[7].Replace("]", String.Empty));
			return String.Format(formatString, year, month, day);
		}

		public string StringToMDXDate(string str)
		{
			string template = "[Период__Период].[Период__Период].[Данные всех периодов].[{0}].[Полугодие {1}].[Квартал {2}].[{3}].[{4}]";
			string[] dateElements = str.Split(' ');
			int year = Convert.ToInt32(dateElements[2]);
			string month = CRHelper.RusMonth(CRHelper.MonthNum(dateElements[1]));
			int quarter = CRHelper.QuarterNumByMonthNum(CRHelper.MonthNum(month));
			int halfYear = CRHelper.HalfYearNumByQuarterNum(quarter);
			int day = Convert.ToInt32(dateElements[0]);
			return String.Format(template, year, halfYear, quarter, month, day);
		}

		public string MDXDateToShortDateString(string mdxDateString)
		{
			if (String.IsNullOrEmpty(mdxDateString))
			{
				return null;
			}
			string[] separator = { "].[" };
			string[] dateElements = mdxDateString.Split(separator, StringSplitOptions.None);
			string template = "{0}.{1}.{2}";
			if (dateElements.Length < 8)
			{
				return null;
			}
			string day = dateElements[7].Replace("]", String.Empty);
			day = day.Length == 1 ? "0" + day : day;
			string month = CRHelper.MonthNum(dateElements[6]).ToString();
			month = month.Length == 1 ? "0" + month : month;
			string year = dateElements[3];
			return String.Format(template, day, month, year);
		}

		public string GetYearFromMDXDate(string mdxDate)
		{
			string[] separator = { "].[" };
			string[] mdxDateElements = mdxDate.Split(separator, StringSplitOptions.None);
			if (mdxDateElements.Length == 8)
			{
				return mdxDateElements[3];
			}
			else
			{
				return "2010";
			}
		}

		#endregion

		#region Экспорт в PDF
		
		private void PdfExportButton_Click(object sender, EventArgs e)
		{
			Report report = new Report();
			ISection section1 = report.AddSection();
			ISection section2 = report.AddSection();
			ISection section3 = report.AddSection();

			foreach (UltraGridRow row in UltraWebGrid.Rows)
				foreach (UltraGridCell cell in row.Cells)
				{
					if (cell.Value != null)
					{
						cell.Value = Regex.Replace(cell.GetText(), "<[\\s\\S]*?>", String.Empty);
					}
				}

			IText title = section1.AddText();
			Font font = new Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
			title.Style.Font.Bold = true;
			title.AddContent(PageTitle.Text);

			title = section1.AddText();
			font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
			title.AddContent(PageSubTitle.Text);

			title = section1.AddText();
			font = new Font("Verdana", 12);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
			title.AddContent("\n" + Regex.Replace(LabelText.Text.Replace("<br/>", "\n"), "<[\\s\\S]*?>", String.Empty) + "\n");

			foreach (UltraGridRow row in headerLayout.Grid.Rows)
			{
				if (row.Index % 4 != 0)
				{
					row.Cells[0].Style.BorderDetails.StyleTop = BorderStyle.None;
				}
				else
				{
					row.Cells[0].Value = null;
				}
				if (row.Index % 4 != 3)
				{
					row.Cells[0].Style.BorderDetails.StyleBottom = BorderStyle.None;
				}
				else
				{
					row.Cells[0].Value = null;
				}
                if (row.Index % 4 != 2)
                {
                    //row.Cells[0].Style.BorderDetails.StyleBottom = BorderStyle.None;
                }
                else
                {
                    row.Cells[0].Value = null;
                }
			}

            

			ReportPDFExporter1.HeaderCellHeight = 60;
			ReportPDFExporter1.Export(headerLayout, section1);

            section2.PageSize = new PageSize(section1.PageSize.Width, section1.PageSize.Height);
            section3.PageSize = new PageSize(section1.PageSize.Width, section1.PageSize.Height);

            section2.PageMargins = section1.PageMargins;
            section3.PageMargins = section1.PageMargins;
            //UltraChart1.ColorModel.ModelStyle =  ColorModels.CustomSkin;
            //UltraChart2.ColorModel.ModelStyle = ColorModels.CustomSkin;

            //UltraChart2.ColorModel.Skin.PEs.Clear();
            //UltraChart2.ColorModel.Skin.PEs.Add(new PaintElement(Color.MediumSeaGreen));
            //UltraChart2.ColorModel.Skin.PEs.Add(new PaintElement(Color.LightGray)); 


            //UltraChart1.ColorModel.Skin.PEs.Clear();
            //UltraChart1.ColorModel.Skin.PEs.Add(new PaintElement(Color.MediumSeaGreen));
            //UltraChart1.ColorModel.Skin.PEs.Add(new PaintElement(Color.LightGray)); 

            title = section2.AddText();
            font = new Font("Verdana", 12);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent("\n" + LabelChart1.Text);

            UltraChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.8);
            UltraChart1.Height = 500;

            UltraChart1.Legend.Margins.Right = 0;
            UltraChart1.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.82));
            MemoryStream imageStream = new MemoryStream();
            UltraChart1.SaveTo(imageStream, ImageFormat.Png);
            Infragistics.Documents.Reports.Graphics.Image img1 = new Infragistics.Documents.Reports.Graphics.Image(imageStream);
            section2.AddImage(img1);

            title = section3.AddText();
            font = new Font("Verdana", 12);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent("\n" + LabelChart2.Text);
            
            UltraChart2.Legend.Margins.Right = 0;
            UltraChart2.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.82));
             imageStream = new MemoryStream();
            UltraChart2.SaveTo(imageStream, ImageFormat.Png);
            img1 = new Infragistics.Documents.Reports.Graphics.Image(imageStream);
            section3.AddImage(img1);

            
		}
		
		#endregion

		#region Экспорт в Excel

		private void ExcelExportButton_Click(object sender, EventArgs e)
		{
			ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
			ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;

			Workbook workbook = new Workbook();
			Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
			Worksheet sheet2 = workbook.Worksheets.Add("Диаграмма 1");
			Worksheet sheet3 = workbook.Worksheets.Add("Диаграмма 2");

			SetExportGridParams(headerLayout.Grid);

			ReportExcelExporter1.HeaderCellHeight = 80;
			ReportExcelExporter1.HeaderCellFont = new Font("Verdana", 11);
			ReportExcelExporter1.TitleFont = new Font("Verdana", 12, FontStyle.Bold);
			ReportExcelExporter1.SubTitleFont = new Font("Verdana", 11);
			ReportExcelExporter1.TitleAlignment = HorizontalCellAlignment.Center;
			ReportExcelExporter1.TitleStartRow = 0;

			foreach (UltraGridRow row in UltraWebGrid.Rows)
			{
				foreach (UltraGridCell cell in row.Cells)
				{
					if (cell.Value != null)
					{
						cell.Value = Regex.Replace(cell.GetText(), "<[\\s\\S]*?>", String.Empty);
					}
				}
			}

			ReportExcelExporter1.Export(headerLayout, sheet1, 7);

			foreach (UltraGridRow row in UltraWebGrid.Rows)
			{
				sheet1.Rows[8 + row.Index].Height = 255;
			}

			// Вывод динамичского текста
			sheet1.MergedCellsRegions.Add(3, 0, 3, 8);
			sheet1.MergedCellsRegions.Add(4, 0, 4, 8);
			sheet1.MergedCellsRegions.Add(5, 0, 5, 8);
			sheet1.Rows[3].Height = 510;

            sheet2.Columns[0].Width = 500 * 50;
            sheet3.Columns[0].Width = 500 * 50;
			string[] separator1 = { "<br/>" };
			string[] text = LabelText.Text.Split(separator1, StringSplitOptions.None);
			for (int i = 0; i < 3; ++i)
			{
				sheet1.Rows[3 + i].Cells[0].Value = Regex.Replace(text[i], "<[\\s\\S]*?>", String.Empty);
				sheet1.Rows[3 + i].Cells[0].CellFormat.WrapText = ExcelDefaultableBoolean.True;
			}

			sheet1.Rows[0].Cells[0].CellFormat.WrapText = ExcelDefaultableBoolean.False;
			sheet1.Rows[0].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
			sheet1.Rows[1].Cells[0].CellFormat.WrapText = ExcelDefaultableBoolean.False;
			sheet1.Rows[1].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
			for (int i = 0; i < UltraWebGrid.Rows.Count; i += 3)
			{
				for (int j = 0; j < UltraWebGrid.Columns.Count; ++j)
				{
					sheet1.Rows[8 + i].Cells[j].CellFormat.BottomBorderStyle = CellBorderLineStyle.None;
					sheet1.Rows[9 + i].Cells[j].CellFormat.BottomBorderStyle = CellBorderLineStyle.None;
					sheet1.Rows[9 + i].Cells[j].CellFormat.TopBorderStyle = CellBorderLineStyle.None;
					sheet1.Rows[10 + i].Cells[j].CellFormat.TopBorderStyle = CellBorderLineStyle.None;
				}
			}

			ReportExcelExporter1.WorksheetTitle = String.Empty;
			ReportExcelExporter1.WorksheetSubTitle = String.Empty;
			UltraChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.75);
			UltraChart1.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.6);
			ReportExcelExporter1.Export(UltraChart1, LabelChart1.Text, sheet2, 1);
			UltraChart2.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.75);
			UltraChart2.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.6);
			ReportExcelExporter1.Export(UltraChart2, LabelChart2.Text, sheet3, 1);

            //ReportExcelExporter1.Export(
		}

		private void SetEmptyCellFormat(Worksheet sheet, int row, int column)
		{
			WorksheetCell cell = sheet.Rows[9 + row * 3].Cells[column];
			cell.CellFormat.Font.Name = "Verdana";
			cell.CellFormat.Font.Height = 200;
			cell.CellFormat.VerticalAlignment = VerticalCellAlignment.Center;

			cell.CellFormat.LeftBorderStyle = CellBorderLineStyle.Thin;
			cell.CellFormat.LeftBorderColor = Color.Black;
			cell.CellFormat.RightBorderStyle = CellBorderLineStyle.Thin;
			cell.CellFormat.RightBorderColor = Color.Black;
			cell.CellFormat.TopBorderStyle = CellBorderLineStyle.Thin;
			cell.CellFormat.TopBorderColor = Color.Black;
			cell.CellFormat.BottomBorderStyle = CellBorderLineStyle.None;
			cell.CellFormat.BottomBorderColor = Color.Black;

			cell = sheet.Rows[10 + row * 3].Cells[column];
			cell.CellFormat.LeftBorderStyle = CellBorderLineStyle.Thin;
			cell.CellFormat.LeftBorderColor = Color.Black;
			cell.CellFormat.RightBorderStyle = CellBorderLineStyle.Thin;
			cell.CellFormat.RightBorderColor = Color.Black;
			cell.CellFormat.BottomBorderStyle = CellBorderLineStyle.None;
			cell.CellFormat.BottomBorderColor = Color.Black;
			cell.CellFormat.TopBorderStyle = CellBorderLineStyle.None;
			cell.CellFormat.TopBorderColor = Color.Black;

			cell = sheet.Rows[11 + row * 3].Cells[column];
			cell.CellFormat.BottomBorderStyle = CellBorderLineStyle.Thin;
			cell.CellFormat.BottomBorderColor = Color.Black;
			cell.CellFormat.LeftBorderStyle = CellBorderLineStyle.Thin;
			cell.CellFormat.LeftBorderColor = Color.Black;
			cell.CellFormat.RightBorderStyle = CellBorderLineStyle.Thin;
			cell.CellFormat.RightBorderColor = Color.Black;
			cell.CellFormat.TopBorderStyle = CellBorderLineStyle.None;
			cell.CellFormat.TopBorderColor = Color.Black;
		}

		private void SetCellFormat(WorksheetCell cell)
		{
			cell.CellFormat.Font.Name = "Verdana";
			cell.CellFormat.Font.Height = 200;
			cell.CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
			cell.CellFormat.BottomBorderStyle = CellBorderLineStyle.Thin;
			cell.CellFormat.BottomBorderColor = Color.Black;
			cell.CellFormat.LeftBorderStyle = CellBorderLineStyle.Thin;
			cell.CellFormat.LeftBorderColor = Color.Black;
			cell.CellFormat.RightBorderStyle = CellBorderLineStyle.Thin;
			cell.CellFormat.RightBorderColor = Color.Black;
			cell.CellFormat.TopBorderStyle = CellBorderLineStyle.Thin;
			cell.CellFormat.TopBorderColor = Color.Black;
		}

		private static void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
		{
			e.CurrentWorksheet.PrintOptions.Orientation = Infragistics.Documents.Excel.Orientation.Landscape;
			e.CurrentWorksheet.PrintOptions.PaperSize = PaperSize.A4;
			e.CurrentWorksheet.PrintOptions.BottomMargin = 0;
			e.CurrentWorksheet.PrintOptions.TopMargin = 0;
			e.CurrentWorksheet.PrintOptions.LeftMargin = 0;
			e.CurrentWorksheet.PrintOptions.RightMargin = 0;
		}

		private static void SetExportGridParams(UltraWebGrid grid)
		{
			string exportFontName = "Verdana";
			int fontSize = 10;
			double coeff = 1.0;
			foreach (UltraGridColumn column in grid.Columns)
			{
				column.Width = Convert.ToInt32(column.Width.Value * coeff);
				column.CellStyle.Font.Name = exportFontName;
				column.Header.Style.Font.Name = exportFontName;
				column.CellStyle.Font.Size = fontSize;
				column.Header.Style.Font.Size = fontSize;
			}
		}
		
		#endregion
	}
}
