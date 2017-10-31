using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Excel;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Infragistics.WebUI.UltraWebNavigator;

using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Infragistics.Documents.Reports.Report.Text;
using System.Text.RegularExpressions;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Core.Primitives;
using System.Collections.ObjectModel;
using Infragistics.UltraChart.Core;
using Dundas.Maps.WebControl;

/**
 *  Анализ розничных цен на социально значимые продовольственные товары по состоянию на ЧЧ.ММ.ГГГГ
 */
namespace Krista.FM.Server.Dashboards.reports.GSM_0003_0001
{
	public partial class Default : CustomReportPage
	{

		#region Поля

        private static DataTable dtGrid;
        private static DataTable dtMap;
        private static DataTable dtChart1;
		private static GridHeaderLayout headerLayout;

        private static int badRank = 0;

		#endregion
        
        // имя папки с картами региона
        private string mapFolderName;

		// --------------------------------------------------------------------

		private const string PageTitleCaption = "Анализ средних розничных цен на нефтепродукты в разрезе муниципальных образований и поселений";
        private const string PageSubTitleCaption = "Еженедельный мониторинг цен на нефтепродукты на основании данных мониторинга предприятий торговли в разрезе муниципальных образований и поселений, по состоянию на {0:dd.MM.yyyy} года";
        private const string Chart1TitleCaption = "Распределение территоорий по средней розничной цене на «{0}», рублей за литр, по состоянию на {1:dd.MM.yyyy} г.";
        private const string MapTitleCaption = "Средняя розничная цена на «{0}», рублей за литр, по состоянию на {1:dd.MM.yyyy} г.";

        private static double hmaoPrice;

		// --------------------------------------------------------------------

		private static bool IsSmallResolution
		{
			get { return CRHelper.GetScreenWidth < 900; }
		}

		protected override void Page_PreLoad(object sender, EventArgs e)
		{
			base.Page_PreLoad(sender, e);

			if (!IsSmallResolution)
			{
				ComboDate.Width = 350;
				ComboFuel.Width = 400;
			}
			else
			{
				ComboDate.Width = 250;
				ComboFuel.Width = 350;
			}
			ComboDate.Title = "Выберите дату";
            ComboDate.AllowSelectionType = Components.AllowedSelectionType.LeafNodes;
			ComboFuel.Title = "Товар";
			ComboFuel.ParentSelect = true;

			#region Грид

			if (!IsSmallResolution)
				UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 12);
			else
				UltraWebGrid.Width = CRHelper.GetGridWidth(750);
			UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.4 + 10);
			UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";
            UltraWebGrid.DataBinding += new EventHandler(UltraWebGrid_DataBinding);
            UltraWebGrid.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid_InitializeLayout);
            UltraWebGrid.InitializeRow += new Infragistics.WebUI.UltraWebGrid.InitializeRowEventHandler(UltraWebGrid_InitializeRow);

			#endregion

            #region Настройка диаграммы

            if (!IsSmallResolution)
            {
                UltraChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 50);
            }
            else
            {
                UltraChart1.Width = CRHelper.GetChartWidth(752);
            }
            UltraChart1.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.5);

            UltraChart1.ChartType = ChartType.ColumnChart;
            UltraChart1.Border.Thickness = 0;

            //UltraChart1.ColorModel.ModelStyle = ColorModels.PureRandom;

            CRHelper.FillCustomColorModel(UltraChart1, 1, true);

            UltraChart1.ColumnChart.SeriesSpacing = 0;
            UltraChart1.ColumnChart.ColumnSpacing = 1;

            UltraChart1.Axis.X.Extent = 150;
            UltraChart1.Axis.X.Labels.Visible = true;
            UltraChart1.Axis.X.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.UseCollection;

            ClipTextAxisLabelLayoutBehavior behavior = new ClipTextAxisLabelLayoutBehavior();
            behavior.Trimming = StringTrimming.None;

            UltraChart1.Axis.X.Labels.Layout.BehaviorCollection.Add(behavior);

            UltraChart1.Axis.Y.Labels.Font = new Font("Verdana", 8);
            UltraChart1.Axis.X.Labels.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart1.Axis.X.Labels.Font = new Font("Verdana", 8);
            UltraChart1.Axis.X.Labels.WrapText = true;
            UltraChart1.Axis.X.Labels.SeriesLabels.Visible = false;
            UltraChart1.Axis.Y.Extent = 20;
            UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";

            UltraChart1.Axis.X.Margin.Far.Value = 3;
            UltraChart1.Axis.X.Margin.Near.Value = 3;

            UltraChart1.Tooltips.FormatString = "<ITEM_LABEL>\n<b><DATA_VALUE:N2></b>, рубль";
            UltraChart1.DataBinding += new EventHandler(UltraChart1_DataBinding);
            UltraChart1.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart1_FillSceneGraph);
            UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

            #endregion

            #region Карта

            if (!IsSmallResolution)
            {
                DundasMap.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 50);
            }
            else
            {
                DundasMap.Width = CRHelper.GetChartWidth(752);
            }
            DundasMap.Height = Unit.Parse("700px");

            mapFolderName = "ХМАОDeer";

            DundasMap.Shapes.Clear();
            DundasMap.ShapeFields.Add("Name");
            DundasMap.ShapeFields["Name"].Type = typeof(string);
            DundasMap.ShapeFields["Name"].UniqueIdentifier = true;
            DundasMap.ShapeFields.Add("Value");
            DundasMap.ShapeFields["Value"].Type = typeof(double);
            DundasMap.ShapeFields["Value"].UniqueIdentifier = false;

            SetMapSettings();
            AddMapLayer(DundasMap, mapFolderName, "Территор", CRHelper.MapShapeType.Areas);

            #endregion

            #region Экспорт

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
			ReportExcelExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
			ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);

			#endregion

		}

		// --------------------------------------------------------------------

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (!Page.IsPostBack)
            {
                FillComboDate("GSM_0003_0001_list_of_dates");
                FillComboFuel();
            }

            #region Анализ параметров

            UserParams.PeriodCurrentDate.Value = StringToMDXDate(ComboDate.SelectedValue);

            DataTable dt = DataProvider.GetDataTableForChart("GSM_0003_0001_dates", DataProvidersFactory.SpareMASDataProvider);
            if (dt.Rows[0]["Предыдущая дата"] != DBNull.Value)
            {
                UserParams.PeriodLastDate.Value = dt.Rows[0]["Предыдущая дата"].ToString();
            }
            else
            {
                UserParams.PeriodLastDate.Value = "[Период__День].[Период__День].[Данные всех периодов].[1998].[Полугодие 1].[Квартал 1].[Январь].[1]";
            }
            if (dt.Rows[0]["Начало года"] != DBNull.Value)
            {
                UserParams.PeriodYear.Value = dt.Rows[0]["Начало года"].ToString();
            }
            else
            {
                UserParams.PeriodYear.Value = "[Период__День].[Период__День].[Данные всех периодов].[1998].[Полугодие 1].[Квартал 1].[Январь].[1]";
            }

            UserParams.Oil.Value = ComboFuel.SelectedValue;

            #endregion

            PageTitle.Text = String.Format(PageTitleCaption, CRHelper.DateByPeriodMemberUName(UserParams.PeriodCurrentDate.Value, 3));
            Page.Title = PageTitle.Text;
            PageSubTitle.Text = String.Format(PageSubTitleCaption, CRHelper.DateByPeriodMemberUName(UserParams.PeriodCurrentDate.Value, 3));

            headerLayout = new GridHeaderLayout(UltraWebGrid);
            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();

            UltraChart1.DataBind();

            FillMapData();
        }

		// --------------------------------------------------------------------

		#region Обработчики грида

        private string GetStarOrNot(string moName)
        {
            if (moName == "Город Ханты-Мансийск" ||
                moName == "Город Нижневартовск" ||
                moName == "Город Покачи" ||
                moName == "Город Радужный" ||
                moName == "Город Урай" ||
                moName == "Белоярский муниципальный район" ||
                moName == "Березовский муниципальный район" ||
                moName == "Кондинский муниципальный район" ||
                moName == "Нижневартовский муниципальный район" ||
                moName == "Октябрьский муниципальный район" ||
                moName == "Ханты-Мансийский муниципальный район")
            {
                return "*";
            }
            else
            {
                return String.Empty;
            }
        }

		protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
		{
			string query = DataProvider.GetQueryText("GSM_0003_0001_grid");
			dtGrid = new DataTable();
			DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Территория", dtGrid);
            query = DataProvider.GetQueryText("GSM_0003_0001_grid_hmao");
            DataTable dtGridHMAO = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Территория", dtGridHMAO);
            hmaoPrice = 0;
            if (dtGrid.Rows.Count > 0)
            {
                badRank = Convert.ToInt32(dtGrid.Rows[0]["Худший ранг"]);
                dtGrid.Columns.Remove("Худший ранг");
                foreach (DataRow row in dtGrid.Rows)
                {
                    row["Территория"] = row["Территория"].ToString() + GetStarOrNot(row["Территория"].ToString());
                }
                DataRow hmaoRow = dtGrid.NewRow();
                hmaoRow["Территория"] = "Ханты-Мансийский автономный округ - Югра";
                hmaoRow["Средняя розничная цена на текущую дату"] = MathHelper.GeoMean(dtGridHMAO.Columns[1], DBNull.Value);
                if (MathHelper.IsDouble(hmaoRow["Средняя розничная цена на текущую дату"]))
                {
                    hmaoPrice = Convert.ToDouble(hmaoRow["Средняя розничная цена на текущую дату"]);
                }
                hmaoRow["Средняя розничная цена на предыдущую дату"] = MathHelper.GeoMean(dtGridHMAO.Columns[2], DBNull.Value);
                hmaoRow["Абсолютное отклонение к предыдущему периоду"] = MathHelper.Minus(hmaoRow["Средняя розничная цена на текущую дату"], hmaoRow["Средняя розничная цена на предыдущую дату"]);
                hmaoRow["Темп прироста к предыдущему периоду"] = MathHelper.Grown(hmaoRow["Средняя розничная цена на текущую дату"], hmaoRow["Средняя розничная цена на предыдущую дату"]);
                hmaoRow["Средняя розничная цена на начало года"] = MathHelper.GeoMean(dtGridHMAO.Columns[3], DBNull.Value);
                hmaoRow["Абсолютное отклонение к началу года"] = MathHelper.Minus(hmaoRow["Средняя розничная цена на текущую дату"], hmaoRow["Средняя розничная цена на начало года"]);
                hmaoRow["Темп прироста к началу года"] = MathHelper.Grown(hmaoRow["Средняя розничная цена на текущую дату"], hmaoRow["Средняя розничная цена на начало года"]);
                dtGrid.Rows.InsertAt(hmaoRow, 0);
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
			band.Columns[0].Width = Unit.Parse("310px");
			for (int i = 1; i < band.Columns.Count; ++i)
			{
                band.Columns[i].Width = i < 4 ? Unit.Parse("100px") : Unit.Parse("120px");
			}
            band.Columns[4].Width = Unit.Parse("75px");
			
			// Заголовки
			e.Layout.HeaderStyleDefault.Wrap = true;
			e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
			e.Layout.HeaderStyleDefault.VerticalAlign = VerticalAlign.Middle;
			headerLayout.AddCell("Территория");
			GridHeaderCell headerCell = headerLayout.AddCell("Средняя розничная цена, рубль");
            headerCell.AddCell(String.Format("{0:dd.MM.yyyy г.}", CRHelper.DateByPeriodMemberUName(UserParams.PeriodCurrentDate.Value, 3)));
            if (UserParams.PeriodLastDate.Value != "[Период__День].[Период__День].[Данные всех периодов].[1998].[Полугодие 1].[Квартал 1].[Январь].[1]")
            {
                headerCell.AddCell(String.Format("{0:dd.MM.yyyy г.}", CRHelper.DateByPeriodMemberUName(UserParams.PeriodLastDate.Value, 3)));
            }
            else
            {
                headerCell.AddCell("-");
            }
            if (UserParams.PeriodYear.Value != "[Период__День].[Период__День].[Данные всех периодов].[1998].[Полугодие 1].[Квартал 1].[Январь].[1]")
            {
                headerCell.AddCell(String.Format("{0:dd.MM.yyyy г.}", CRHelper.DateByPeriodMemberUName(UserParams.PeriodYear.Value, 3)));
            }
            else
            {
                headerCell.AddCell("-");
            }

            headerLayout.AddCell("Ранг по субъекту");
            headerCell = headerLayout.AddCell("Динамика к предыдущему отчетному периоду");
            headerCell.AddCell("Абсолютное отклонение, рубль");
            headerCell.AddCell("Темп прироста, %");
            headerCell = headerLayout.AddCell("Динамика за период с начала года");
            headerCell.AddCell("Абсолютное отклонение, рубль");
            headerCell.AddCell("Темп прироста, %");

            headerLayout.ApplyHeaderInfo();

            CRHelper.FormatNumberColumn(UltraWebGrid.Columns[1], "N2");
            CRHelper.FormatNumberColumn(UltraWebGrid.Columns[2], "N2");
            CRHelper.FormatNumberColumn(UltraWebGrid.Columns[3], "N2");
            CRHelper.FormatNumberColumn(UltraWebGrid.Columns[4], "N0");
            CRHelper.FormatNumberColumn(UltraWebGrid.Columns[5], "N2");
            CRHelper.FormatNumberColumn(UltraWebGrid.Columns[6], "P2");
            CRHelper.FormatNumberColumn(UltraWebGrid.Columns[7], "N2");
            CRHelper.FormatNumberColumn(UltraWebGrid.Columns[8], "P2");

		}

        protected void UltraWebGrid_SetCellFormatString(UltraGridCell cell)
        {
            if (cell.Column.Index == 0)
                return;
            string format = String.Empty;
            switch (cell.Column.Index)
            {
                case 1:
                case 2:
                case 3:
                case 5:
                case 7:
                    {
                        format = "{0:N2}";
                        break;
                    }
                case 6:
                case 8:
                    {
                        format = "{0:P2}";
                        break;
                    }
                default:
                    {
                        format = "{0:N0}";
                        break;
                    }
            }
            cell.Value = MathHelper.IsDouble(cell.Value) ? String.Format(format, Convert.ToDouble(cell.Value)) : null;
        }

		protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
		{
            UltraGridCell rankCell = e.Row.Cells[4];
            if (e.Row.Index == 0)
            {
                e.Row.Style.Font.Bold = true;
            }
            else if (rankCell.Value == null)
            {
                e.Row.Cells[0].Style.Padding.Left = Unit.Parse("25px");
            }
            else
            {
                if (Convert.ToInt32(rankCell.Value) == 1)
                {
                    rankCell.Style.BackgroundImage = "~/images/starYellowbb.png";
                    rankCell.Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                    rankCell.Title = "Самый низкий уровень цены";
                }
                else if (Convert.ToInt32(rankCell.Value) == badRank)
                {
                    rankCell.Style.BackgroundImage = "~/images/starGraybb.png";
                    rankCell.Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                    rankCell.Title = "Самый высокий уровень цены";
                }
                else
                {
                    rankCell.Title = String.Format("Ранг по ХМАО-Югре: {0:N0}", rankCell.Value);
                }
            }
            if (e.Row.Cells[5].Value != null)
            {
                e.Row.Cells[5].Title = String.Format("Изменение в руб. к {0:dd.MM.yyyy}", CRHelper.DateByPeriodMemberUName(UserParams.PeriodLastDate.Value, 3));
            }
            if (e.Row.Cells[6].Value != null)
            {
                e.Row.Cells[6].Title = String.Format("Изменение в % к {0:dd.MM.yyyy}", CRHelper.DateByPeriodMemberUName(UserParams.PeriodLastDate.Value, 3));
                if (MathHelper.AboveZero(e.Row.Cells[6].Value))
                {
                    e.Row.Cells[6].Style.BackgroundImage = "~/images/ArrowRedUpBB.png";
                    e.Row.Cells[6].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }
                else if (MathHelper.SubZero(e.Row.Cells[6].Value))
                {
                    e.Row.Cells[6].Style.BackgroundImage = "~/images/ArrowGreenDownBB.png";
                    e.Row.Cells[6].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }
            }
            if (e.Row.Cells[7].Value != null)
            {
                e.Row.Cells[7].Title = String.Format("Изменение в руб. к {0:dd.MM.yyyy}", CRHelper.DateByPeriodMemberUName(UserParams.PeriodYear.Value, 3));
            }
            if (e.Row.Cells[8].Value != null)
            {
                e.Row.Cells[8].Title = String.Format("Изменение в % к {0:dd.MM.yyyy}", CRHelper.DateByPeriodMemberUName(UserParams.PeriodYear.Value, 3));
                if (MathHelper.AboveZero(e.Row.Cells[8].Value))
                {
                    e.Row.Cells[8].Style.BackgroundImage = "~/images/ArrowRedUpBB.png";
                    e.Row.Cells[8].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }
                else if (MathHelper.SubZero(e.Row.Cells[8].Value))
                {
                    e.Row.Cells[8].Style.BackgroundImage = "~/images/ArrowGreenDownBB.png";
                    e.Row.Cells[8].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }
            }
            foreach (UltraGridCell cell in e.Row.Cells)
                UltraWebGrid_SetCellFormatString(cell);
        }

		#endregion  

		// --------------------------------------------------------------------

        #region Обработчики диаграммы

        protected void UltraChart1_DataBinding(object sender, EventArgs e)
        {
            LabelChart1.Text = String.Format(Chart1TitleCaption, UserParams.Oil.Value, CRHelper.DateByPeriodMemberUName(UserParams.PeriodCurrentDate.Value, 3));
            string query = DataProvider.GetQueryText("GSM_0003_0001_chart1");
            dtChart1 = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Территория", dtChart1);
            if (dtChart1.Rows.Count > 0)
            {
                foreach (DataRow row in dtChart1.Rows)
                {
                    row["Территория"] = row["Территория"].ToString().Replace("Город ", "г. ").Replace(" муниципальный район", " р-н");
                }
                double maxValue = Convert.ToDouble(dtChart1.Rows[dtChart1.Rows.Count - 1][1]);
                double minValue = Convert.ToDouble(dtChart1.Rows[0][1]);
                UltraChart1.Axis.Y.RangeType = AxisRangeType.Custom;
                UltraChart1.Axis.Y.RangeMin = minValue * 0.95;
                UltraChart1.Axis.Y.RangeMax = maxValue * 1.05;

                UltraChart1.Data.SwapRowsAndColumns = true;
                UltraChart1.DataSource = dtChart1.DefaultView;
            }
            else
            {
                UltraChart1.DataSource = null;
            }
        }

        protected void UltraChart1_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            if (hmaoPrice != 0)
            {
                UltraChart1_AddLineWithTitle(hmaoPrice, "Ханты-Мансийский автономный округ", Color.Red, e);
            }
        }
        
        protected void UltraChart1_AddLineWithTitle(double value, string region, Color color, FillSceneGraphEventArgs e)
        {
            string formatString = "{0}: {1:N2}, рубль";
            IAdvanceAxis xAxis = (IAdvanceAxis)e.Grid["X"];
            IAdvanceAxis yAxis = (IAdvanceAxis)e.Grid["Y"];
            Point p1 = new Point((int)xAxis.MapMinimum, (int)yAxis.Map(value));
            Point p2 = new Point((int)xAxis.MapMaximum, (int)yAxis.Map(value));
            Line line = new Line(p1, p2);
            line.lineStyle.DrawStyle = LineDrawStyle.Dash;
            line.PE.Stroke = color;
            line.PE.StrokeWidth = 2;
            e.SceneGraph.Add(line);

            Text text = new Text();
            text.labelStyle.Orientation = TextOrientation.Horizontal;
            text.labelStyle.Font = new System.Drawing.Font("Arial", 10, FontStyle.Italic);

            text.labelStyle.HorizontalAlign = StringAlignment.Near;
            text.labelStyle.VerticalAlign = StringAlignment.Near;

            Size size = new Size(500, 15);
            Point p;
            p = new Point(p1.X + 50, p1.Y - 20);

            text.bounds = new System.Drawing.Rectangle(p, size);

            text.SetTextString(String.Format(formatString, region, value));

            e.SceneGraph.Add(text);
        }

        #endregion

		// --------------------------------------------------------------------

        #region Обработчики карты

        protected void SetMapSettings()
        {
            DundasMap.Visible = true;
            DundasMap.Meridians.Visible = false;
            DundasMap.Parallels.Visible = false;
            DundasMap.ZoomPanel.Visible = true;
            DundasMap.NavigationPanel.Visible = true;
            DundasMap.Viewport.EnablePanning = true;
            DundasMap.Viewport.Zoom = 100;

            // добавляем легенду
            Legend legend = new Legend("Legend");
            legend.Visible = true;
            legend.BackColor = Color.White;
            legend.BackSecondaryColor = Color.Gainsboro;
            legend.BackGradientType = GradientType.DiagonalLeft;
            legend.BackHatchStyle = MapHatchStyle.None;
            legend.BorderColor = Color.Gray;
            legend.BorderWidth = 1;
            legend.BorderStyle = MapDashStyle.Solid;
            legend.BackShadowOffset = 4;
            legend.TextColor = Color.Black;
            legend.Font = new System.Drawing.Font("MS Sans Serif", 7, FontStyle.Regular);
            legend.AutoFitText = true;
            legend.Title = String.Format("Розничная цена на «{0}», рублей за литр", UserParams.Oil.Value);
            legend.AutoFitMinFontSize = 7;
            DundasMap.Legends.Clear();
            DundasMap.Legends.Add(legend);

            // добавляем правила раскраски
            DundasMap.ShapeRules.Clear();
            ShapeRule rule = new ShapeRule();
            rule.Name = "Rule";
            rule.Category = String.Empty;
            rule.ShapeField = "Value";
            rule.DataGrouping = DataGrouping.EqualInterval;
            rule.ColorCount = 5;
            rule.ColoringMode = ColoringMode.ColorRange;
            rule.FromColor = Color.Green;
            rule.MiddleColor = Color.Yellow;
            rule.ToColor = Color.Red;
            rule.BorderColor = Color.FromArgb(50, Color.Black);
            rule.GradientType = GradientType.None;
            rule.HatchStyle = MapHatchStyle.None;
            rule.ShowInColorSwatch = false;
            rule.ShowInLegend = "Legend";
            DundasMap.ShapeRules.Add(rule);
        }

        protected void AddMapLayer(MapControl map, string mapFolder, string layerFileName, CRHelper.MapShapeType shapeType)
        {
            string layerName = Server.MapPath(string.Format("../../maps/Субъекты/{0}/{1}.shp", mapFolder, layerFileName));
            int oldShapesCount = map.Shapes.Count;

            map.LoadFromShapeFile(layerName, "Name", true);
            map.Layers.Add(shapeType.ToString());

            for (int i = oldShapesCount; i < map.Shapes.Count; i++)
            {
                Shape shape = map.Shapes[i];
                shape.Layer = shapeType.ToString();
            }
        }

        string GetCompareName(string s)
        {
            return s.ToLower().Replace("автономный округ", "ао").Replace("область", "обл.").Replace("республика", "р.");
        }

        protected Shape FindMapShape(MapControl map, string patternValue)
        {
            foreach (Shape shape in map.Shapes)
            {
                if ((GetCompareName(shape.Name) == GetCompareName(patternValue)) || shape.Name.ToLower() == patternValue.ToLower())
                {
                    return shape;
                }
            }
            return null;
        }


        protected void FillMapData()
        {
            string valueSeparator = "\n";
            string shapeHint;
            DundasMap.Legends[0].Title = String.Format("Розничная цена на\n«{0}»,\nрублей за литр", UserParams.Oil.Value);
            shapeHint = "{0}" + valueSeparator + "{1:N2}, рубль" + valueSeparator + "Ранг: {2:N0}";
            LabelMap.Text = String.Format(MapTitleCaption, UserParams.Oil.Value, CRHelper.DateByPeriodMemberUName(UserParams.PeriodCurrentDate.Value, 3));
            string query = DataProvider.GetQueryText("GSM_0003_0001_map");
            dtMap = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Территория", dtMap);
            if (dtMap.Rows.Count == 0)
            {
                return;
            }
            foreach (Shape shape in DundasMap.Shapes)
            {
                shape.Text = String.Format("{0}", shape.Name);
            }
            foreach (DataRow row in dtMap.Rows)
            {
                // заполняем карту данными
                string subject = row["Территория"].ToString();
                string shortName = row["Территория"].ToString().Replace(" муниципальный район", " р-н").Replace("Город ", "г. ");
                int rank = Convert.ToInt32(row["Ранг"]);
                double value;
                if (!Double.TryParse(row["Средняя розничная цена на текущую дату"].ToString(), out value))
                {
                    value = 0;
                }
                Shape shape = FindMapShape(DundasMap, shortName);
                if (shape != null)
                {
                    shape.Visible = true;
                    string shapeName = shape.Name;

                    shape["Name"] = subject;
                    shape["Value"] = value;
                    shape.TextVisibility = TextVisibility.Shown;
                    shape.Text = String.Format("{0}\n{1:N2}", shapeName.Replace(" р-н", "\nр-н"), value);
                    shape.ToolTip = String.Format(shapeHint, subject, value, rank);
                }
            }
        }

        #endregion

		#region Заполнение словарей и выпадающих списков параметров

		protected void FillComboFuel()
		{
            Dictionary<string, int> dict = new Dictionary<string, int>();
            dict.Add("Бензин марки АИ-80", 0);
            dict.Add("Бензин марки АИ-92", 0);
            dict.Add("Бензин марки АИ-93", 0);
            dict.Add("Бензин марки АИ-95", 0);
            dict.Add("Бензин марки АИ-96", 0);
            dict.Add("Бензин марки АИ-98", 0);
            dict.Add("Дизельное топливо", 0);
            dict.Add("Газ сжиженный углеводородный для заправки автотранспортных средств", 0);
            ComboFuel.FillDictionaryValues(dict);

            string query = DataProvider.GetQueryText("GSM_0003_0001_list_of_oil");
            DataTable dtOilList = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Вид ГСМ", dtOilList);

            if (dtOilList.Rows.Count == 0)
            {
                ComboFuel.SetСheckedState("Бензин марки АИ-92", true);
            }
            else
            {
                ComboFuel.SetСheckedState(dtOilList.Rows[0]["Вид ГСМ"].ToString(), true);
            }
        }

		protected void FillComboDate(string queryName)
		{  
			DataTable dtDate = new DataTable(); 
			string query = DataProvider.GetQueryText(queryName);
			DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Число", dtDate);
			Dictionary<string, int> dict = new Dictionary<string, int>();
			if (dtDate.Rows.Count == 0)
				throw new Exception("Нет данных для построения отчета!");
			foreach (DataRow row in dtDate.Rows)
			{
                DateTime date = CRHelper.DateByPeriodMemberUName(row["Дата"].ToString(), 3);
                AddPairToDictionary(dict, String.Format("{0} год", date.Year), 0);
                AddPairToDictionary(dict, String.Format("{0} {1} года", CRHelper.ToUpperFirstSymbol(CRHelper.RusMonth(date.Month)), date.Year), 1);
                AddPairToDictionary(dict, String.Format("{0:D}", date).Replace("г.", "года"), 2);
            }
			ComboDate.FillDictionaryValues(dict);
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
            string template = "[Период__День].[Период__День].[Данные всех периодов].[{0}].[Полугодие {1}].[Квартал {2}].[{3}].[{4}]";
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
			UltraWebGrid grid = headerLayout.Grid;

			ReportPDFExporter1.PageTitle = PageTitle.Text;
			ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;

			Report report = new Report();
			ISection section1 = report.AddSection();
            ISection section2 = report.AddSection();
            ISection section3 = report.AddSection();

            UltraChart1.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
            DundasMap.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));

			ReportPDFExporter1.HeaderCellHeight = 70;

			ReportPDFExporter1.Export(headerLayout, section1);
            ReportPDFExporter1.Export(UltraChart1, LabelChart1.Text, section2);
            ReportPDFExporter1.Export(DundasMap, LabelMap.Text, section3);
        }

		#endregion

		#region Экспорт в Excel

		private void ExcelExportButton_Click(object sender, EventArgs e)
		{
			UltraWebGrid grid = headerLayout.Grid;

			Workbook workbook = new Workbook();
			Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            Worksheet sheet2 = workbook.Worksheets.Add("Диаграмма");
            Worksheet sheet3 = workbook.Worksheets.Add("Карта");

			SetExportGridParams(grid);

			ReportExcelExporter1.HeaderCellFont = new Font("Verdana", 11);
			ReportExcelExporter1.TitleFont = new Font("Verdana", 12, FontStyle.Bold);
			ReportExcelExporter1.SubTitleFont = new Font("Verdana", 11);
			ReportExcelExporter1.TitleAlignment = HorizontalCellAlignment.Center;
			ReportExcelExporter1.TitleStartRow = 0;

			foreach (UltraGridRow row in grid.Rows)
			{
				if (row.IsActiveRow())
				{
					row.Activated = false;
					row.Selected = false;
				}
			}

            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;

            ReportExcelExporter1.Export(headerLayout, sheet1, 3);

            ReportExcelExporter1.WorksheetTitle = String.Empty;
            ReportExcelExporter1.WorksheetSubTitle = String.Empty;

            UltraChart1.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.7));
            ReportExcelExporter1.Export(UltraChart1, LabelChart1.Text, sheet2, 1);
            DundasMap.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.7));
            ReportExcelExporter1.Export(DundasMap, LabelMap.Text, sheet3, 1);
            sheet2.MergedCellsRegions.Clear();
            sheet3.MergedCellsRegions.Clear();
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
