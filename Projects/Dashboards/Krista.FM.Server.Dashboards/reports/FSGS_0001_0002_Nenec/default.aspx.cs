using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
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
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

/**
 *  Анализ розничных цен на социально значимые продовольственные товары по состоянию на ЧЧ.ММ.ГГГГ
 */
namespace Krista.FM.Server.Dashboards.reports.FSGS_0001_0002_Nenec
{
	public partial class Default : CustomReportPage 
	{

		#region Поля

		private DataTable dtGrid;
		private DataTable dtChart1;
		private DataTable dtChart2;
		private GridHeaderLayout headerLayout;

		#endregion

		#region Параметры запроса

		private CustomParam selectedRegion;
		private CustomParam selectedFO;
		private CustomParam selectedDate;

		#endregion

		// --------------------------------------------------------------------

		// заголовок страницы
		private const string PageTitleCaption = "Анализ средних розничных цен на нефтепродукты в разрезе территорий РФ";
		private const string PageSubTitleCaption = "Еженедельный мониторинг средних розничных цен на нефтепродукты в разрезе территорий РФ, {0}, по состоянию на {1}";
		// заголовок для UltraChart
		private const string Chart1TitleCaption = "Динамика средней розничной цены на нефтепродукты, {0}, рублей за литр";
		private const string Chart2TitleCaption = "Уровень цен на различные виды топлива в разрезе территорий на {0} года, рублей за литр";

		private static Dictionary<string, string> dictRegions = new Dictionary<string, string>();
		private static Dictionary<string, string> dictDates = new Dictionary<string, string>();

		private static string prevDate;

		// --------------------------------------------------------------------

        private double widthMultiplier;
        private double heightMultiplier;
        private int fontSizeMultiplier;
        private int primitiveSizeMultiplier;
        private int pageWidth;
        private int pageHeight;
        private bool onWall;
        private bool blackStyle;

		private static bool IsSmallResolution
		{
			get { return CRHelper.GetScreenWidth < 900; }
		}

        protected override void Page_PreInit(object sender, EventArgs e)
        {
            base.Page_PreInit(sender, e);
            blackStyle = false;
            if (Session["blackStyle"] != null)
            {
                blackStyle = Convert.ToBoolean(((CustomParam)Session["blackStyle"]).Value);
            }

            string regionTheme = RegionSettings.Instance.Id;
            CRHelper.SetPageTheme(this, blackStyle ? regionTheme + "BlackStyle" : regionTheme);
            //CRHelper.SetPageTheme(this, blackStyle ? "MinfinBlackStyle" : "Minfin");
        }

        
		protected override void Page_PreLoad(object sender, EventArgs e)
		{
            

			double chartHeight = 0.45;
			int legendHeight = 20;

			base.Page_PreLoad(sender, e);

            #region Параметры

            selectedRegion = UserParams.CustomParam("selected_region", true);
            selectedFO = UserParams.CustomParam("selected_fo", true);
            selectedDate = UserParams.CustomParam("selected_date", true);

            #endregion

            onWall = false;
            if (Session["onWall"] != null)
            {
                onWall = Convert.ToBoolean(((CustomParam)Session["onWall"]).Value);
                Session["onWall"] = null;
            }

			ComboDate.Title = "Выберите дату";
			ComboDate.Width = 300;
			ComboDate.ParentSelect = true;
			ComboRegion.Title = "Территория";
			ComboRegion.Width = 400;
			ComboRegion.ParentSelect = true;

            pageWidth = onWall ? int.Parse(RegionSettingsHelper.Instance.GetPropertyValue("WallResolutionWidth")) : (int)Session["width_size"];
            pageHeight = onWall ? int.Parse(RegionSettingsHelper.Instance.GetPropertyValue("WallResolutionHeight")) : (int)Session["height_size"];

            UltraWebGrid.Width = CRHelper.GetGridWidth(pageWidth);

			UltraWebGrid.Height = Unit.Empty;
			UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";
			UltraWebGrid.DisplayLayout.SelectTypeRowDefault = SelectType.Single;
			UltraWebGrid.ActiveRowChange += new ActiveRowChangeEventHandler(UltraWebGrid_ActiveRowChange);

			#region Настройка диаграммы 1

			if (!IsSmallResolution)
				UltraChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 50);
			else
				UltraChart1.Width = CRHelper.GetChartWidth(750);

            UltraChart1.Height = 550;
            LabelChart1.Width = UltraChart1.Width;

			UltraChart1.ChartType = ChartType.LineChart;
			UltraChart1.LineChart.NullHandling = NullHandling.InterpolateSimple;
			UltraChart1.Border.Thickness = 0;

			UltraChart1.Axis.X.Extent = 60;
			UltraChart1.Axis.X.Labels.ItemFormatString = "<ITEM_LABEL:N0>";
			UltraChart1.Axis.Y.Extent = 50;
			UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";

			UltraChart1.ColorModel.ModelStyle = ColorModels.CustomSkin;
			UltraChart1.ColorModel.Skin.ApplyRowWise = true;
			UltraChart1.ColorModel.Skin.PEs.Clear();

			for (int i = 0; i < 4; ++i)
			{
				PaintElement pe = new PaintElement();

				switch (i)
				{
					case 0:
						{
							pe.Fill = Color.Green;
							break;
						}
					case 1:
						{
							pe.Fill = Color.Blue;
							break;
						}
					case 2:
						{
							pe.Fill = Color.Yellow;
							break;
						}
					case 3:
						{
							pe.Fill = Color.Red;
							break;
						}
				}

				pe.FillStopColor = pe.Fill;
				pe.FillGradientStyle = GradientStyle.ForwardDiagonal;
				pe.FillOpacity = 200;
				pe.FillStopOpacity = 100;

				UltraChart1.ColorModel.Skin.PEs.Add(pe);

				LineAppearance lineAppearance = new LineAppearance();

				lineAppearance.IconAppearance.Icon = SymbolIcon.Circle;
				lineAppearance.IconAppearance.IconSize = SymbolIconSize.Small;
				lineAppearance.IconAppearance.PE = pe;

				UltraChart1.LineChart.LineAppearances.Add(lineAppearance);
			}

			UltraChart1.Tooltips.FormatString = "<ITEM_LABEL>\nцена на <SERIES_LABEL> года\n<b><DATA_VALUE:N2></b>, рубль";
			UltraChart1.DataBinding += new EventHandler(UltraChart1_DataBinding);
			UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

			UltraChart1.Axis.X.Margin.Near.MarginType = LocationType.Pixels;
			UltraChart1.Axis.X.Margin.Near.Value = 20;
			UltraChart1.Axis.X.Margin.Far.MarginType = LocationType.Pixels;
			UltraChart1.Axis.X.Margin.Far.Value = 20;

			UltraChart1.Legend.Location = LegendLocation.Top;
			UltraChart1.Legend.SpanPercentage = 13;
			UltraChart1.Legend.Font = new Font("Microsoft Sans Serif", 9);
			UltraChart1.Legend.Visible = true;

            UltraChart1.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart1_FillSceneGraph);

			#endregion

			#region Настройка диаграммы 2

			if (!IsSmallResolution)
				UltraChart2.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 50);
			else
				UltraChart2.Width = CRHelper.GetChartWidth(750);
            UltraChart2.Height = 550;

			UltraChart2.ChartType = ChartType.LineChart;
			UltraChart2.LineChart.NullHandling = NullHandling.DontPlot;
			UltraChart2.Border.Thickness = 0;

			UltraChart2.Axis.X.Extent = 165;
			UltraChart2.Axis.X.Labels.WrapText = true;
			UltraChart2.Axis.Y.Extent = 50;
			UltraChart2.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";

            UltraChart2.Axis.X.Labels.Font = new Font("Arail", 10);

			UltraChart2.ColorModel.ModelStyle = ColorModels.CustomSkin;
			UltraChart2.ColorModel.Skin.ApplyRowWise = true;
			UltraChart2.ColorModel.Skin.PEs.Clear();

			for (int i = 0; i < 4; ++i)
			{
				PaintElement pe = new PaintElement();

				switch (i)
				{
					case 0:
						{
							pe.Fill = Color.Green;
							break;
						}
					case 1:
						{
							pe.Fill = Color.Blue;
							break;
						}
					case 2:
						{
							pe.Fill = Color.Yellow;
							break;
						}
					case 3:
						{
							pe.Fill = Color.Red;
							break;
						}
				}

				pe.FillStopColor = pe.Fill;
				pe.StrokeWidth = 0;
				pe.FillGradientStyle = GradientStyle.ForwardDiagonal;
				pe.FillOpacity = 200;
				pe.FillStopOpacity = 100;
				UltraChart2.ColorModel.Skin.PEs.Add(pe);

				pe.Stroke = Color.Black;
				pe.StrokeWidth = 0;

				LineAppearance lineAppearance = new LineAppearance();

				lineAppearance.IconAppearance.Icon = SymbolIcon.Square;
				lineAppearance.IconAppearance.IconSize = SymbolIconSize.Medium;
				lineAppearance.IconAppearance.PE = pe;

				UltraChart2.LineChart.LineAppearances.Add(lineAppearance);

				UltraChart2.LineChart.Thickness = 0;
			}

			UltraChart2.Tooltips.FormatString = "<ITEM_LABEL>\n<SERIES_LABEL>\n<b><DATA_VALUE:N2></b>, рубль";
			UltraChart2.DataBinding += new EventHandler(UltraChart2_DataBinding);
			UltraChart2.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

			UltraChart2.Axis.X.Margin.Near.MarginType = LocationType.Pixels;
			UltraChart2.Axis.X.Margin.Near.Value = 20;
			UltraChart2.Axis.X.Margin.Far.MarginType = LocationType.Pixels;
			UltraChart2.Axis.X.Margin.Far.Value = 20;
			UltraChart2.Axis.Y.Margin.Near.MarginType = LocationType.Pixels;
			UltraChart2.Axis.Y.Margin.Near.Value = 20;
			UltraChart2.Axis.Y.Margin.Far.MarginType = LocationType.Pixels;
			UltraChart2.Axis.Y.Margin.Far.Value = 20;

			UltraChart2.Legend.Location = LegendLocation.Top;
			UltraChart2.Legend.SpanPercentage = 13;
			UltraChart2.Legend.Font = new Font("Microsoft Sans Serif", 9);
			UltraChart2.Legend.Visible = true;
            UltraChart2.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart2_FillSceneGraph);
			#endregion

			

			#region Экспорт
            //ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            //ReportExcelExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            //ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click); ;
			#endregion

            #region Bigresolution
            heightMultiplier = onWall ? 5 : 1;
            fontSizeMultiplier = onWall ? 5 : 1;
            primitiveSizeMultiplier = onWall ? 4 : 1;

            pageWidth = onWall ? pageWidth : (int)Session["width_size"];

            pageHeight = onWall ? pageHeight : (int)Session["height_size"];            

            widthMultiplier = 1;
            if (Session["width_size"] != null && (int)Session["width_size"] != 0)
            {
                widthMultiplier = onWall ? 1.08 * pageWidth / (int)Session["width_size"] : 1;
            }

            Color fontColor = Color.Black;
                //blackStyle ? Color.White : Color.Black;

            UltraWebGrid.DisplayLayout.HeaderStyleDefault.Font.Size = Convert.ToInt32(9 * fontSizeMultiplier);
            UltraWebGrid.DisplayLayout.HeaderStyleDefault.BorderColor = blackStyle ? Color.DarkGray : onWall ? Color.Black : Color.Gray;
            UltraWebGrid.DisplayLayout.HeaderStyleDefault.BorderWidth = blackStyle ? 1 : onWall ? 3 : 1;

            UltraWebGrid.DisplayLayout.RowStyleDefault.Font.Size = Convert.ToInt32(9 * fontSizeMultiplier);
            UltraWebGrid.DisplayLayout.RowStyleDefault.BorderColor = blackStyle ? Color.DarkGray : onWall ? Color.Black : Color.Gray;
            UltraWebGrid.DisplayLayout.RowStyleDefault.BorderWidth = 1;

            PageTitle.Font.Size = Convert.ToInt32(14 * fontSizeMultiplier);
            PageSubTitle.Font.Size = Convert.ToInt32(12 * fontSizeMultiplier);

            LabelBottomGrid.Font.Size = Convert.ToInt32(9 * fontSizeMultiplier);
            LabelBottomGrid.ForeColor = fontColor;

            LabelBottomGrid0.Font.Size = Convert.ToInt32(9 * fontSizeMultiplier);
            LabelBottomGrid0.ForeColor = fontColor;

            string redGradientBar = String.Format("<img style=\"vertical-align:top\" src=\"../../images/RedGradientBarInverse.png\" width=\"{0}px\" height=\"{0}px\">",
                    18 * primitiveSizeMultiplier);

            string greenGradientBar = String.Format("<img style=\"vertical-align:top\" src=\"../../images/GreenGradientBarInverse.png\" width=\"{0}px\" height=\"{0}px\">",
                    18 * primitiveSizeMultiplier);

            LabelBottomGrid.Text = String.Format("Индикатор {0} / {1} - рост/снижение показателя относительно прошлого / позапрошлого года", greenGradientBar, redGradientBar);
              
            string bestStar = String.Format("<img style=\"vertical-align:middle\" src=\"../../images/starYellowBBLarge.png\" width=\"{0}px\" height=\"{0}px\">",
                    18 * primitiveSizeMultiplier);

            string worseStar = String.Format("<img style=\"vertical-align:middle\" src=\"../../images/starGrayBBLarge.png\" width=\"{0}px\" height=\"{0}px\">",
                    18 * primitiveSizeMultiplier);

            LabelBottomGrid0.Text = String.Format("{0} - лучший ранг&nbsp;&nbsp;&nbsp;{1} - худший ранг", bestStar, worseStar);

            if (onWall) 
            {
                ComprehensiveDiv.Style.Add("width", pageWidth.ToString()+"px");
                ComprehensiveDiv.Style.Add("height", pageHeight.ToString()+"px");                
            }

            if (onWall && Page.Master is IMasterPage)
            {
                ((IMasterPage)Page.Master).SetHeaderVisible(false);
            }

            PopupInformer1.Visible = !onWall;

            

            #endregion

            WallLink.Text = "Для&nbsp;видеостены";
            WallLink.NavigateUrl = String.Format("default.aspx?paramlist=onWall=true");
           
            ComboDate.Visible = !onWall;
            ComboRegion.Visible = !onWall;            
            RefreshButton1.Visible = !onWall;
        }   

        void UltraChart2_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            Text Caption = new Text();
            Caption.SetTextString("Рублей за литр");
            Caption.labelStyle.Orientation = TextOrientation.VerticalLeftFacing;
            Caption.labelStyle.FontColor = Color.Gray;
            Caption.bounds.X = -35;
            Caption.bounds.Y = 140;
            Caption.bounds.Width = 100;
            Caption.bounds.Height = 100;

            e.SceneGraph.Add(Caption);
        }

        void UltraChart1_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            Text Caption = new Text();
            Caption.SetTextString("Рублей за литр");
            Caption.labelStyle.Orientation = TextOrientation.VerticalLeftFacing;
            Caption.labelStyle.FontColor = Color.Gray;
            Caption.bounds.X = -35;
            Caption.bounds.Y = 150;
            Caption.bounds.Width = 100;
            Caption.bounds.Height = 100;

            e.SceneGraph.Add(Caption);
        }
		// --------------------------------------------------------------------

		protected override void Page_Load(object sender, EventArgs e)
		{
			base.Page_Load(sender, e);
			if (!Page.IsPostBack)
			{
				FillComboDate("FSGS_0001_0002_list_of_dates");
				FillComboRegion("FSGS_0001_0002_list_of_regions");

				PanelCharts.AddLinkedRequestTrigger(UltraWebGrid);
				PanelCharts.AddRefreshTarget(UltraChart1);
			}
			
			#region Анализ параметров

			string mdxRegionName;
			dictRegions.TryGetValue(ComboRegion.SelectedValue, out mdxRegionName);
			selectedRegion.Value = mdxRegionName;

			Node node = new Node();
			if (ComboDate.SelectedNode.Level == 0)
			{
				node = ComboDate.GetLastChild(ComboDate.GetLastChild(ComboDate.SelectedNode));
			}
			if (ComboDate.SelectedNode.Level == 1)
			{
				node = ComboDate.GetLastChild(ComboDate.SelectedNode);
			}
			if (ComboDate.SelectedNode.Level == 2)
			{
				node = ComboDate.SelectedNode;
			}
			selectedDate.Value = StringToMDXDate(node.Text);
			dictDates.TryGetValue(node.Text, out prevDate);

			#endregion
			
			PageTitle.Text = String.Format(PageTitleCaption, GetLastBlock(selectedRegion.Value), MDXDateToShortDateString(selectedDate.Value));
			Page.Title = PageTitle.Text;
            PageSubTitle.Text = String.Format(PageSubTitleCaption, "<b>"+ComboRegion.SelectedValue+"</b>", "<b>"+ComboDate.SelectedValue+"</b>");
			
			if (!PanelCharts.IsAsyncPostBack)
			{
				headerLayout = new GridHeaderLayout(UltraWebGrid);
				UltraWebGrid.Bands.Clear();
				UltraWebGrid.DataBind();
				UltraWebGrid_FillSceneGraph(UltraWebGrid);
				UltraWebGrid_MarkByStars(UltraWebGrid);
			}
            if (ComboRegion.SelectedIndex == 0)
            {
                UltraGridRow row = UltraWebGrid.Rows[0];                
            }
            else
            {
                UltraGridRow row = UltraWebGrid.Rows[1];                
            }
            UltraChart1.Visible = false;
            UltraChart2.Visible = false;

            string redGradientBar = String.Format("<img style=\"vertical-align:top\" src=\"../../images/RedGradientBarInverse.png\" width=\"{0}px\" height=\"{0}px\">",
                18 * (onWall?4:1));
            string greenGradientBar = String.Format("<img style=\"vertical-align:top\" src=\"../../images/GreenGradientBarInverse.png\" width=\"{0}px\" height=\"{0}px\">",
                    18 * (onWall ? 4 : 1));
            
            LabelBottomGrid.Text = String.Format("Индикатор {0} / {1} - рост/снижение показателя относительно прошлого / позапрошлого года", greenGradientBar, redGradientBar);


            
			
		}

		// --------------------------------------------------------------------

		#region Обработчики грида

		void UltraWebGrid_ActiveRowChange(object sender, RowEventArgs e)
		{
			UltraWebGrid_ChangeRow(e.Row);
		}

		protected void UltraWebGrid_ChangeRow(UltraGridRow row)
		{
			if (row == null)
				return;

            row.Activate();
            row.Activated = true;
            row.Selected = true;
			selectedFO.Value = row.Cells[row.Cells.Count - 1].GetText();
			UltraChart1.DataBind();
			UltraChart2.DataBind();
		}

		protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
		{
			string query = DataProvider.GetQueryText("FSGS_0001_0002_grid");
			dtGrid = new DataTable();
			DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Территория", dtGrid);
			if (dtGrid.Rows.Count > 0)
			{
                if (dtGrid.Rows[0][0].ToString().Contains(dtGrid.Rows[1][0].ToString()))
                {
                    dtGrid.Rows[0].Delete();
                }                
                UltraWebGrid.DataSource = dtGrid;                
			}
			else
			{
				UltraWebGrid.DataSource = null;
			}
		}

		protected void UltraWebGrid_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
		{
			UltraGridBand band = e.Layout.Bands[0];
			e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
			e.Layout.AllowSortingDefault = AllowSorting.No;
			e.Layout.NullTextDefault = "-"; 
			e.Layout.RowSelectorStyleDefault.Width = 20;
			band.Columns[0].CellStyle.Wrap = true;
			decimal columnWidth = Browser == "AppleMAC-Safari" ? 
                (onWall?95* 4: 65):
                (onWall ?  95 * 4 : 85);
            band.Columns[0].Width = onWall ? 130 * 4 : 130;
			for (int i = 1; i < band.Columns.Count; ++i)
			{
				UltraGridColumn column = band.Columns[i];
				column.Width = (int)columnWidth;
				column.CellStyle.HorizontalAlign = HorizontalAlign.Right;
				column.CellStyle.Padding.Right = 5;
				column.CellStyle.Padding.Left = 5;
				switch (i % 3)
				{
					case 1:
						{
							CRHelper.FormatNumberColumn(column, "N2");
							break;
						}
					case 2:
						{
							CRHelper.FormatNumberColumn(column, "N2");
							break;
						}
					case 0:
						{
							CRHelper.FormatNumberColumn(column, "P2");
							break;
						}
				}
			}
			
			// Заголовки
			e.Layout.HeaderStyleDefault.Wrap = true;
			e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
			e.Layout.HeaderStyleDefault.VerticalAlign = VerticalAlign.Middle;
			headerLayout.AddCell("Территория");
            string[] fuelTypes = { "Дизельное топливо", "Бензин марки АИ-80", "Бензин марки АИ-92", "Бензин марки АИ-95" };
			for (int i = 0; i < fuelTypes.Length; ++i)
			{
				GridHeaderCell cell = headerLayout.AddCell(fuelTypes[i]);
                cell.AddCell("Цена, рублей за литр");
				cell.AddCell("Абсолютное отклонение");
				cell.AddCell("Темп прироста");
			}
			headerLayout.AddCell("MDX имя");
			headerLayout.ApplyHeaderInfo();

			band.Columns[e.Layout.Bands[0].Columns.Count - 1].Hidden = true;
		}


        string ClearDataMemberCaption(string Caption)
        {
            if (Caption.Contains("ДАННЫЕ)"))
            {
                return Caption.Replace("ДАННЫЕ)", "").Remove(0, 1);
            }
            else
            {
                return Caption;
            }
        }

		protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
		{
            e.Row.Cells[0].Text = ClearDataMemberCaption(e.Row.Cells[0].Text); 
			UltraWebGrid grid = sender as UltraWebGrid;
			for (int i = 0; i < e.Row.Cells.Count - 1; ++i)
			{	
				UltraWebGrid_SetCellHint(sender as UltraWebGrid, e.Row.Cells[i]);
			}
            if (ComboRegion.SelectedNode.Parent == null)
            {
                e.Row.Style.Font.Bold = e.Row.Index < 1;
            }
            else
            {
                e.Row.Style.Font.Bold = e.Row.Index < 2;
            }
            
		}

		protected void UltraWebGrid_MarkByStars(UltraWebGrid grid)
		{
            primitiveSizeMultiplier = onWall ? 4 : 1;
            string bestStar = String.Format("<img style=\"vertical-align:middle\" src=\"../../images/starYellowBBLarge.png\" width=\"{0}px\" height=\"{0}px\">",
                    18 * primitiveSizeMultiplier);

            string worseStar = String.Format("<img style=\"vertical-align:middle\" src=\"../../images/starGrayBBLarge.png\" width=\"{0}px\" height=\"{0}px\">",
                    18 * primitiveSizeMultiplier);

			for (int columnIndex = 1; columnIndex < grid.Columns.Count - 1; columnIndex += 3)
			{
				string maxValueRows = String.Empty;
				string minValueRows = String.Empty;
				double maxValue = Double.NegativeInfinity;
				double minValue = Double.PositiveInfinity;
				int rowIndex = 0;
				for (rowIndex = 0; rowIndex < dtGrid.Rows.Count; ++rowIndex)
				{
					DataRow row = dtGrid.Rows[rowIndex];
					double value;
					if (Double.TryParse(row[columnIndex].ToString(), out value) && (value != 0))
					{
						if (value == maxValue)
						{
							maxValueRows = maxValueRows == String.Empty ? rowIndex.ToString() : maxValueRows + " " + rowIndex.ToString();
						}
						else if (value > maxValue)
						{
							maxValue = value;
							maxValueRows = rowIndex.ToString();
						}
						else if (value == minValue)
						{
							minValueRows = minValueRows == String.Empty ? rowIndex.ToString() : minValueRows + " " + rowIndex.ToString();
						}
						else if (value < minValue)
						{
							minValue = value;
							minValueRows = rowIndex.ToString();
						}
					}
				}
				string[] rows = null;
				if (!String.IsNullOrEmpty(maxValueRows))
				{
                    


					rows = maxValueRows.Split(' ');
					foreach (string row in rows)
					{
						rowIndex = Convert.ToInt32(row);

                        string img = "../../images/starGraybb.png";
                        int barHeight = onWall ? 80 : 20;
                        int barBottomMargin = onWall ? 5 - barHeight : -barHeight;
                        grid.Rows[rowIndex].Cells[columnIndex].Value = String.Format("{0} {1:N2}", bestStar, grid.Rows[rowIndex].Cells[columnIndex].Value);

                    }
				}
				if (!String.IsNullOrEmpty(minValueRows)) 
				{
					rows = minValueRows.Split(' ');
					foreach (string row in rows)
					{
						rowIndex = Convert.ToInt32(row);

                        string img = "../../images/starYellowbb.png";
                        int barHeight = onWall ? 80 : 20;
                        int barBottomMargin = onWall ? 5 - barHeight : -barHeight;

                        grid.Rows[rowIndex].Cells[columnIndex].Value = String.Format("{0} {1:N2}", worseStar, grid.Rows[rowIndex].Cells[columnIndex].Value);
					}
				}
			}
		}

        void SetImageFromCell(UltraGridCell Cell, string ImageName)
        {
            string ImagePath = "~/images/" + ImageName;
            Cell.Style.CustomRules = "background-repeat: no-repeat; background-position: left center";
            Cell.Style.BackgroundImage = ImagePath;
        }

		protected void UltraWebGrid_FillSceneGraph(UltraWebGrid grid)
		{
			for (int i = 0; i < grid.Rows.Count; ++i)
			{
                
				for (int j = 3; j < grid.Columns.Count - 1; j += 3)
				{
					double value; 
					if (Double.TryParse(grid.Rows[i].Cells[j].Text, out value))
					{
                        string img = String.Empty;
                        int barHeight = onWall ? 80 : 20;
                        int barBottomMargin = onWall ? 5 - barHeight : -barHeight;
                        if (Convert.ToDouble(grid.Rows[i].Cells[j].Value) < 0)
                        {
                            img = "../../images/RedGradientBarInverse.png";
                            grid.Rows[i].Cells[j].Title = "Снижение к прошлому году";
                        }
                        else if (Convert.ToDouble(grid.Rows[i].Cells[j].Value) > 0)
                        {
                            img = "../../images/GreenGradientBarInverse.png";
                            grid.Rows[i].Cells[j].Title = "Рост к прошлому году";
                        }
                        else
                        {
                            continue;
                        }
                        
                        string format = "N2";

                        grid.Rows[i].Cells[j].Value = String.Format("<div style='position: relative; z-index: 1; margin-bottom: {3}px; margin-right: -5px; width:100%; height:{2}px'><img src=\"{1}\" width=\"100%\" height=\"{2}px\"></div><div style='position: relative; z-index: 2;'>{0:N2}</div>",
                                value, img, barHeight, barBottomMargin);

					}
				}
			}
		}

		protected void UltraWebGrid_SetCellHint(UltraWebGrid grid, UltraGridCell cell)
		{
			if (cell.Column.Index % 3 != 1 && cell.Column.Index > 0 && cell.Column.Index < cell.Row.Cells.Count - 1)
			{
				if (cell.Column.Index % 3 == 2)
				{
					cell.Title = String.Format("Прирост к {0} г.", prevDate);
				}
				else if (cell.Column.Index % 3 == 0)
				{
					cell.Title = String.Format("Темп прироста к {0} г.", prevDate);
				}
			}
		}

		#endregion

		// --------------------------------------------------------------------

		#region Обработчики диаграммы 1

		protected void UltraChart1_DataBinding(object sender, EventArgs e)
		{   
            LabelChart1.Text = String.Format(Chart1TitleCaption,
                GetLastBlock(selectedFO.Value).Replace(".DATAMEMBER", "")).Replace(" ", "&nbsp;");
            
			string query = DataProvider.GetQueryText("FSGS_0001_0002_chart1");
			dtChart1 = new DataTable();
			DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Дата", dtChart1);
			if (dtChart1 == null || dtChart1.Rows.Count == 0)
			{
				UltraChart1.DataSource = null;
				return;
			}

			dtChart1.Columns.RemoveAt(0);

			foreach (DataRow row in dtChart1.Rows)
                try
                {
                    row[0] = MDXDateToShortDateString(row[0].ToString());
                }
                catch { }

			UltraChart1.Data.SwapRowsAndColumns = true;
			UltraChart1.DataSource = dtChart1.DefaultView;
		}

		#endregion

		#region Обработчики диаграммы 2

		protected void UltraChart2_DataBinding(object sender, EventArgs e)
		{
			LabelChart2.Text = String.Format(Chart2TitleCaption, MDXDateToShortDateString(selectedDate.Value));
			string query = DataProvider.GetQueryText("FSGS_0001_0002_chart2");
			dtChart2 = new DataTable();
			DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Территория", dtChart2);
			if (dtChart2 == null || dtChart2.Rows.Count == 0)
			{
				UltraChart2.DataSource = null;
				return;
			}

			dtChart2.Columns.RemoveAt(0);

            foreach (DataRow row in dtChart2.Rows)
            {
                row[0] = row[0].ToString().Replace("автономная обл.", "АО");
            }

			UltraChart2.Data.SwapRowsAndColumns = true;
			UltraChart2.DataSource = dtChart2.DefaultView;
		}

		#endregion

		// --------------------------------------------------------------------

		#region Заполнение словарей и выпадающих списков параметров

		protected void FillComboRegion(string queryName)
		{
			DataTable dtRegion = new DataTable();
			string query = DataProvider.GetQueryText(queryName);
			DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtRegion);
			if (dtRegion == null || dtRegion.Rows.Count == 0)
				throw new Exception("Нет данных для построения отчета!");
			Dictionary<string, int> dict = new Dictionary<string, int>();
			foreach (DataRow row in dtRegion.Rows)
			{
				string levelName = row[4].ToString();
				int level = levelName == "РФ" ? 0 : levelName == "Федеральный округ" ? 1 : 2;
				AddPairToDictionary(dict, row[2].ToString(), level);
				AddPairToDictionary(dictRegions, row[2].ToString(), row[3].ToString());
			}
			ComboRegion.FillDictionaryValues(dict);
            String FoName = RegionsNamingHelper.GetFoBySubject(RegionSettings.Instance.Name);
			if (dict.ContainsKey(FoName))
				ComboRegion.SetСheckedState(FoName, true);
		}

		protected void FillComboDate(string queryName)
		{
			DataTable dtDate = new DataTable();
			string query = DataProvider.GetQueryText(queryName);
			DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
			if (dtDate == null || dtDate.Rows.Count == 0)
				throw new Exception("Нет данных для построения отчета!");
			Dictionary<string, int> dictDate = new Dictionary<string, int>();
			string prev = null;
			for (int row = 0; row < dtDate.Rows.Count; ++row)
			{
				string year = dtDate.Rows[row][0].ToString();
				string month = dtDate.Rows[row][3].ToString();
				string day = dtDate.Rows[row][4].ToString();
				string date = day + " " + CRHelper.RusMonthGenitive(CRHelper.MonthNum(month)) + " " + year + " года";
				AddPairToDictionary(dictDate, year + " год", 0);
				AddPairToDictionary(dictDate, month + " " + year + " года", 1);
				AddPairToDictionary(dictDate, day + " " + CRHelper.RusMonthGenitive(CRHelper.MonthNum(month)) + " " + year + " года", 2);
				if (prev != null)
					AddPairToDictionary(dictDates, day + " " + CRHelper.RusMonthGenitive(CRHelper.MonthNum(month)) + " " + year + " года", prev);
				prev = MDXDateToShortDateString(dtDate.Rows[row][5].ToString());
			}
			ComboDate.FillDictionaryValues(dictDate);
			ComboDate.SelectLastNode();
		}

		protected void AddPairToDictionary(Dictionary<string, int> dict, string key, int value)
		{
			if (!dict.ContainsKey(key))
			{
				dict.Add(key, value);
			}
		}

		protected void AddPairToDictionary(Dictionary<string, string> dict, string key, string value)
		{
			if (!dict.ContainsKey(key))
			{
				dict.Add(key, value);
			}
		}

		#endregion

		#region Функции-полезняшки преобразования и все такое

		private static string Browser
		{
			get { return HttpContext.Current.Request.Browser.Browser; }
		}

		public string MDXDateToString(string MDXDateString)
		{
			string[] separator = { "].[" };
			string[] dateElements = MDXDateString.Split(separator, StringSplitOptions.None);
			string template = "{0} {1} {2} года";
			string day = dateElements[7].Replace("]", String.Empty);
			string month = CRHelper.RusMonthGenitive(CRHelper.MonthNum(dateElements[6].ToString()));
			string year = dateElements[3];
			return String.Format(template, day, month, year);
		}

		public string StringToMDXDate(string str)
		{
			string template = "[Период].[Период].[Данные всех периодов].[{0}].[Полугодие {1}].[Квартал {2}].[{3}].[{4}]";
			string[] dateElements = str.Split(' ');
			int year = Convert.ToInt32(dateElements[2]);
			string month = CRHelper.RusMonth(CRHelper.MonthNum(dateElements[1]));
			int quarter = CRHelper.QuarterNumByMonthNum(CRHelper.MonthNum(month));
			int halfYear = CRHelper.HalfYearNumByQuarterNum(quarter);
			int day = Convert.ToInt32(dateElements[0]);
			return String.Format(template, year, halfYear, quarter, month, day);
		}

		public string MDXDateToShortDateString(string MDXDateString)
		{
			string[] separator = { "].[" };
			string[] dateElements = MDXDateString.Split(separator, StringSplitOptions.None);
			if (dateElements.Length < 8)
			{
				return MDXDateString;
			}
			string template = "{0:00}.{1:00}.{2}";
			int day = Convert.ToInt32(dateElements[7].Replace("]", String.Empty));
			int month = CRHelper.MonthNum(dateElements[6]);
			int year = Convert.ToInt32(dateElements[3]);
			return String.Format(template, day, month, year);
		}

		public string MDXDateToShortDateString1(string MDXDateString)
		{
			string[] separator = { "].[" };
			string[] dateElements = MDXDateString.Split(separator, StringSplitOptions.None);
			string template = "{0:00}.{1:00}.{2}";
			int day = Convert.ToInt32(dateElements[7].Replace("]", String.Empty));
			int month = CRHelper.MonthNum(dateElements[6]);
			int year = Convert.ToInt32(dateElements[3].Substring(2, 2));
			return String.Format(template, day, month, year);
		}

		public string GetLastBlock(string mdxString)
		{
			if (String.IsNullOrEmpty(mdxString))
			{
				return String.Empty;
			}
			string[] separator = { "].[" };
			string[] stringElements = mdxString.Split(separator, StringSplitOptions.None);
			return stringElements[stringElements.Length - 1].Replace("]", String.Empty);
		}

		#endregion

		#region Экспорт в PDF

		private void PdfExportButton_Click(object sender, EventArgs e)
		{
            //ReportPDFExporter1.PageTitle = PageTitle.Text;
            //ReportPDFExporter1.PageSubTitle = PageSubTitle.Text.Replace("<b>", "").Replace("</b>", "");

			Report report = new Report();
			ISection section1 = report.AddSection();
			ISection section2 = report.AddSection();
			ISection section3 = report.AddSection();

			UltraChart1.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
			UltraChart2.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));

            //ReportPDFExporter1.HeaderCellHeight = 25;
			GridHeaderLayout workHeader = new GridHeaderLayout();
			workHeader = headerLayout;
			if (UltraWebGrid.DataSource != null)
			{
				workHeader.childCells.Remove(workHeader.childCells[workHeader.childCells.Count - 1]);
			}

            //ReportPDFExporter1.Export(workHeader, section1);
            //ReportPDFExporter1.Export(UltraChart1, LabelChart1.Text, section2);
            //ReportPDFExporter1.Export(UltraChart2, LabelChart2.Text, section3);
		}

		#endregion

		#region Экспорт в Excel

		private void ExcelExportButton_Click(object sender, EventArgs e)
		{
            //ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            //ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text.Replace("<b>", "").Replace("</b>", "");

			Workbook workbook = new Workbook();
			Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
			Worksheet sheet2 = workbook.Worksheets.Add("Диаграмма по времени");
			Worksheet sheet3 = workbook.Worksheets.Add("Диаграмма по территориям");

			sheet2.PrintOptions.ScalingType = ScalingType.FitToPages;

			GridHeaderLayout workHeader = headerLayout;
			workHeader.childCells.RemoveAt(workHeader.childCells.Count - 1);

			SetExportGridParams(workHeader.Grid);

            //ReportExcelExporter1.HeaderCellFont = new Font("Verdana", 11);
            //ReportExcelExporter1.TitleFont = new Font("Verdana", 12, FontStyle.Bold);
            //ReportExcelExporter1.SubTitleFont = new Font("Verdana", 11);
            //ReportExcelExporter1.TitleAlignment = HorizontalCellAlignment.Center;
            //ReportExcelExporter1.TitleStartRow = 0;

			foreach (UltraGridRow row in UltraWebGrid.Rows)
			{
				if (row.IsActiveRow())
				{
					row.Activated = false;
					row.Selected = false;
				}
			}

			int startGridRow = 3;

            //ReportExcelExporter1.Export(workHeader, sheet1, startGridRow);

			sheet1.Rows[0].Cells[0].CellFormat.WrapText = ExcelDefaultableBoolean.False;
			sheet1.Rows[0].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
			sheet1.Rows[1].Cells[0].CellFormat.WrapText = ExcelDefaultableBoolean.False;
			sheet1.Rows[1].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;

            //ReportExcelExporter1.WorksheetTitle = String.Empty;
            //ReportExcelExporter1.WorksheetSubTitle = String.Empty;

			UltraChart1.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.7));
            //ReportExcelExporter1.Export(UltraChart1, LabelChart1.Text, sheet2, 1);
			
			UltraChart2.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.7));
            //ReportExcelExporter1.Export(UltraChart2, LabelChart2.Text, sheet3, 1);
		}

		private static void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
		{
			e.CurrentWorksheet.PrintOptions.Orientation = Infragistics.Documents.Excel.Orientation.Landscape;
			e.CurrentWorksheet.PrintOptions.PaperSize = PaperSize.A4;
			e.CurrentWorksheet.PrintOptions.BottomMargin = 0.25;
			e.CurrentWorksheet.PrintOptions.TopMargin = 0.25;
			e.CurrentWorksheet.PrintOptions.LeftMargin = 0.25;
			e.CurrentWorksheet.PrintOptions.RightMargin = 0.25;
			e.CurrentWorksheet.PrintOptions.ScalingType = ScalingType.FitToPages;
		}

		private static void SetExportGridParams(UltraWebGrid grid)
		{
			string exportFontName = "Verdana";
			int fontSize = 10;
			double coeff = 1.4;
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
