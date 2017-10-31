using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
 
using Dundas.Maps.WebControl; 

using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Excel;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;

using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

/**
 *  Мониторинг ситуации на рынке труда в субъекте РФ по состоянию на ЧЧ.ММ.ГГГГ
 */
namespace Krista.FM.Server.Dashboards.reports.STAT_0003_0001_Novosib_YANAO
{
	public partial class Default : CustomReportPage
	{
        ICustomizerSize CustomizerSize;

        #region Подонка размеров элементов, под браузер и разрешение
        abstract class ICustomizerSize
        {
            public enum BrouseName { IE, FireFox, SafariOrHrome }

            public enum ScreenResolution { _800x600, _1280x1024, Default }

            public BrouseName NameBrouse;

            public abstract int GetGridWidth();
            public abstract int GetChartWidth();

            public abstract int GetMapHeight();

            protected abstract void ContfigurationGridIE(UltraWebGrid Grid);
            protected abstract void ContfigurationGridFireFox(UltraWebGrid Grid);
            protected abstract void ContfigurationGridGoogle(UltraWebGrid Grid);

            public virtual void ContfigurationGrid(UltraWebGrid Grid)
            {
                switch (this.NameBrouse)
                {
                    case BrouseName.IE:
                        this.ContfigurationGridIE(Grid);
                        return;
                    case BrouseName.FireFox:
                        this.ContfigurationGridFireFox(Grid);
                        return;
                    case BrouseName.SafariOrHrome:
                        this.ContfigurationGridGoogle(Grid);
                        return;
                    default:
                        throw new Exception("Охо!");
                }
            }

            public ICustomizerSize(BrouseName NameBrouse)
            {
                this.NameBrouse = NameBrouse;
            }

            public static BrouseName GetBrouse(string _BrouseName)
            {
                if (_BrouseName == "IE") { return BrouseName.IE; };
                if (_BrouseName == "FIREFOX") { return BrouseName.FireFox; };
                if (_BrouseName == "APPLEMAC-SAFARI") { return BrouseName.SafariOrHrome; };

                return BrouseName.IE;
            }

            public static ScreenResolution GetScreenResolution(int Width)
            {


                if (Width < 801)
                {
                    return ScreenResolution._800x600;
                }
                if (Width < 1281)
                {
                    return ScreenResolution._1280x1024;
                }
                return ScreenResolution.Default;
            }
        }

        class CustomizerSize_800x600 : ICustomizerSize
        {
            public override int GetGridWidth()
            {
                switch (this.NameBrouse)
                {
                    case BrouseName.IE:
                        return 1190;
                    case BrouseName.FireFox:
                        return 1190;//750;
                    case BrouseName.SafariOrHrome:
                        return 1190;//755;
                    default:
                        return 800;
                }
            }

            public override int GetChartWidth()
            {
                switch (this.NameBrouse)
                {
                    case BrouseName.IE:
                        return 740;
                    case BrouseName.FireFox:
                        return 750;
                    case BrouseName.SafariOrHrome:
                        return 750;
                    default:
                        return 800;
                }
            }

            public CustomizerSize_800x600(BrouseName NameBrouse) : base(NameBrouse) { }

            #region Настрока размера колонок для грида
            protected override void ContfigurationGridIE(UltraWebGrid Grid)
            {
                int onePercent = (int)Grid.Width.Value / 100;
                foreach (UltraGridColumn col in Grid.Columns)
                {
                    col.Width = onePercent * (100 - 0) / (Grid.Columns.Count);
                }

                //Grid.Columns[0].Width = onePercent * 18;
            }

            protected override void ContfigurationGridFireFox(UltraWebGrid Grid)
            {
                int onePercent = (int)Grid.Width.Value / 100;
                foreach (UltraGridColumn col in Grid.Columns)
                {
                    col.Width = onePercent * (100 - 0) / (Grid.Columns.Count);
                }

                //Grid.Columns[0].Width = onePercent * 18;
            }

            protected override void ContfigurationGridGoogle(UltraWebGrid Grid)
            {
                int onePercent = (int)Grid.Width.Value / 100;
                foreach (UltraGridColumn col in Grid.Columns)
                {
                    col.Width = onePercent * (100 - 0) / (Grid.Columns.Count);
                }

                //Grid.Columns[0].Width = onePercent * 18;
            }

            #endregion


            public override int GetMapHeight()
            {
                return 705;
            }
        }

        class CustomizerSize_1280x1024 : ICustomizerSize
        {
            public override int GetGridWidth()
            {
                switch (this.NameBrouse)
                {
                    case BrouseName.IE:
                        return (int)CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 20).Value;
                    case BrouseName.FireFox:
                        return (int)CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 20).Value;
                    case BrouseName.SafariOrHrome:
                        return (int)CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 20).Value;
                    default:
                        return (int)CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 100).Value;
                }
            }

            public override int GetChartWidth()
            {
                switch (this.NameBrouse)
                {
                    case BrouseName.IE:
                        return (int)CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 50);
                    case BrouseName.FireFox:
                        return (int)CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 50);
                    case BrouseName.SafariOrHrome:
                        return (int)CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 50);
                    default:
                        return (int)CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 50);
                }
            }

            public CustomizerSize_1280x1024(BrouseName NameBrouse) : base(NameBrouse) { }

            #region Настрока размера колонок для грида
            protected override void ContfigurationGridIE(UltraWebGrid Grid)
            {
                int onePercent = (int)Grid.Width.Value / 100;
                foreach (UltraGridColumn col in Grid.Columns)
                {
                    {
                        col.Width = onePercent * (100 - 20) / (Grid.Columns.Count - 1);
                    }
                }

                Grid.Columns[0].Width = onePercent * 18;
            }

            protected override void ContfigurationGridFireFox(UltraWebGrid Grid)
            {
                int onePercent = (int)Grid.Width.Value / 100;
                foreach (UltraGridColumn col in Grid.Columns)
                {
                    {
                        col.Width = onePercent * (100 - 20) / (Grid.Columns.Count - 1);
                    }
                }

                Grid.Columns[0].Width = onePercent * 18;
            }

            protected override void ContfigurationGridGoogle(UltraWebGrid Grid)
            {
                int onePercent = (int)Grid.Width.Value / 100;
                foreach (UltraGridColumn col in Grid.Columns)
                {
                   col.Width = onePercent * (100 - 19) / (Grid.Columns.Count - 1);
                }

                Grid.Columns[0].Width = onePercent * 16;
            }

            #endregion


            public override int GetMapHeight()
            {
                return 705;
            }
        }
        #endregion


		#region Поля

		private DataTable dtDate;
		private DataTable dtMap1;
		private DataTable dtMap2;
		private DataTable dtGrid;
		private DataTable dtChart;
		private GridHeaderLayout headerLayout;

		#endregion

		// имя папки с картами региона
        private const string mapFolderName = "ЯНАО";

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
		private CustomParam compareDate;
		private CustomParam selectedYear;
		private CustomParam compareYear;

		#endregion
		// --------------------------------------------------------------------

		private const string PageTitleCaption = "Мониторинг ситуации на рынке труда";
        private const string PageSubTitleCaption = "Данные еженедельного мониторинга ситуации на рынке труда в Ямало-Ненецком автономном округе по состоянию на {0}.";
		private const string Map1TitleCaption = "Уровень зарегистрированной безработицы по муниципальным образованиям на {0}";
		private string Map2TitleCaption = "Уровень напряженности на рынке труда, на {0}".Replace("незанятых","безработных");
        private const string Chart1TitleCaption = "Динамика числа безработных и числа вакансий, Ямало-Ненецкий автономный округ";

		// --------------------------------------------------------------------
        string BN = "IE";
		protected override void Page_PreLoad(object sender, EventArgs e)
		{
			base.Page_PreLoad(sender, e);
            UltraChart1.DataBinding += new EventHandler(UltraChart1_DataBinding);


			ComboDate.Title = "Выберите дату";
			ComboDate.Width = 300;
			ComboDate.ParentSelect = true;
			ComboCompareDate.Title = "Выберите дату для сравнения";
			ComboCompareDate.Width = 400;
			ComboCompareDate.ParentSelect = true;
			UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 12);
            UltraWebGrid.Height = 600;
			UltraWebGrid.DataBinding += new EventHandler(UltraWebGrid_DataBinding);
			UltraWebGrid.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid_InitializeLayout);
			UltraWebGrid.InitializeRow += new Infragistics.WebUI.UltraWebGrid.InitializeRowEventHandler(UltraWebGrid_InitializeRow);
			UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";
			LabelText.Width = UltraWebGrid.Width; ;

			#region Настройка диаграммы

			UltraChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 17);
			UltraChart1.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.5);

			UltraChart1.ChartType = ChartType.SplineAreaChart;
			UltraChart1.Data.ZeroAligned = true;

			LineAppearance lineAppearance = new LineAppearance();
			lineAppearance.IconAppearance.Icon = SymbolIcon.Circle;
			lineAppearance.IconAppearance.PE.Fill = Color.Black;
			lineAppearance.IconAppearance.IconSize = SymbolIconSize.Small;
			UltraChart1.AreaChart.LineAppearances.Add(lineAppearance);

			UltraChart1.Border.Thickness = 0;

			UltraChart1.Axis.X.Extent = 50;
			UltraChart1.Axis.X.Labels.Visible = true;
			UltraChart1.Axis.Y.Extent = 50;
			UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";

			UltraChart1.Legend.Location = LegendLocation.Top;
			UltraChart1.Legend.SpanPercentage = 17;
			UltraChart1.Legend.Font = new Font("Microsoft Sans Serif", 9);
			UltraChart1.Legend.Visible = true;

			UltraChart1.ColorModel.ModelStyle = ColorModels.PureRandom;

			
			UltraChart1.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart1_FillSceneGraph);
			UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

			#endregion

			#region Параметры
			selectedDate = UserParams.CustomParam("selected_date");
			compareDate = UserParams.CustomParam("compare_date");
			selectedYear = UserParams.CustomParam("selected_year");
			compareYear = UserParams.CustomParam("compare_year");
			#endregion

			#region Ссылки
            CrossLink1.Text = "Мониторинг&nbsp;ситуации&nbsp;на&nbsp;рынке&nbsp;труда&nbsp;<br>(по&nbsp;муниципальным&nbsp;образованиям&nbsp;Ямало-Ненецкого&nbsp;автономного&nbsp;округа)";
			CrossLink1.NavigateUrl = "~/reports/STAT_0003_0002_Novosib_YANAO/Default.aspx";
			#endregion

			#region Карты

			DundasMap1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 15);
			DundasMap1.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight - 190);
			LabelMap1.Width = DundasMap1.Width;

			DundasMap2.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 15);
			DundasMap2.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight - 190);
			LabelMap2.Width = DundasMap2.Width;

			DundasMap2.PostPaint += new MapPaintEvent(DundasMap2_PostPaint);

			#endregion

			#region Экспорт
			
			ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
			ReportExcelExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
			ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click); ;
			
			#endregion



            System.Web.HttpBrowserCapabilities myBrowserCaps = Request.Browser;

            BN = ((System.Web.Configuration.HttpCapabilitiesBase)myBrowserCaps).Browser.ToUpper();

            int add = 0;
            if (BN == "IE")
            {
                add = -50;
            }

            if (ICustomizerSize.GetScreenResolution(CRHelper.GetScreenWidth) == ICustomizerSize.ScreenResolution._1280x1024)
            {
                CustomizerSize = new CustomizerSize_1280x1024(ICustomizerSize.GetBrouse(BN));
            }
            else

                if (ICustomizerSize.GetScreenResolution(CRHelper.GetScreenWidth) == ICustomizerSize.ScreenResolution._800x600)
                {
                    CustomizerSize = new CustomizerSize_800x600(ICustomizerSize.GetBrouse(BN));
                }
                else
                {
                    CustomizerSize = new CustomizerSize_1280x1024(ICustomizerSize.GetBrouse(BN));
                }

            UltraWebGrid.Width = CustomizerSize.GetGridWidth();

            UltraChart1.Width = CustomizerSize.GetChartWidth();
            DundasMap1.Width = CustomizerSize.GetChartWidth();
            DundasMap2.Width = CustomizerSize.GetChartWidth();


            UltraWebGrid.DisplayLayout.AllowColSizingDefault = AllowSizing.Fixed;

		}

		// --------------------------------------------------------------------

		protected override void Page_Load(object sender, EventArgs e)
		{

			base.Page_Load(sender, e);
			if (!Page.IsPostBack)
			{
				FillComboDate(ComboDate, "STAT_0003_0001_list_of_dates", 0);
				FillComboDate(ComboCompareDate, "STAT_0003_0001_list_of_dates", 1);
			}
			#region Анализ параметров
			switch (ComboDate.SelectedNode.Level)
			{
				case 0:
					{
						selectedDate.Value = StringToMDXDate(ComboDate.GetLastChild(ComboDate.SelectedNode).FirstNode.Text);
						break;
					}
				case 1:
					{
						selectedDate.Value = StringToMDXDate(ComboDate.SelectedNode.FirstNode.Text);
						break;
					}
				case 2:
					{
						selectedDate.Value = StringToMDXDate(ComboDate.SelectedNode.Text);
						break;
					}
			}
			switch (ComboCompareDate.SelectedNode.Level)
			{
				case 0:
					{
						compareDate.Value = StringToMDXDate(ComboCompareDate.GetLastChild(ComboCompareDate.SelectedNode).FirstNode.Text);
						break;
					}
				case 1:
					{
						compareDate.Value = StringToMDXDate(ComboCompareDate.SelectedNode.FirstNode.Text);
						break;
					}
				case 2:
					{
						compareDate.Value = StringToMDXDate(ComboCompareDate.SelectedNode.Text);
						break;
					}
			}
			if (compareDate.Value == GetMaxMDXDate(selectedDate.Value, compareDate.Value))
			{
				string tmpDate = selectedDate.Value;
				selectedDate.Value = compareDate.Value;
				compareDate.Value = tmpDate;
			}
			/*selectedYear.Value = GetYearFromMDXDate(selectedDate.Value);
			compareYear.Value = GetYearFromMDXDate(compareDate.Value);
			if (selectedYear.Value == compareYear.Value)
			{
				compareYear.Value = Convert.ToString(Convert.ToUInt32(selectedYear.Value) - 1);
			}*/
			Years();

			#endregion

			PageTitle.Text = PageTitleCaption;
			Page.Title = PageTitle.Text;
			PageSubTitle.Text = String.Format(PageSubTitleCaption, 
                MDXDateToShortDateString(selectedDate.Value));
			
			headerLayout = new GridHeaderLayout(UltraWebGrid);
			UltraWebGrid.DataBind();

            try
            {
                MakeDynamicText();
            }
            catch { }

			UltraChart1.DataBind();
            try
            {
                #region Настройка карты 1

                DundasMap1.Meridians.Visible = false;
                DundasMap1.Parallels.Visible = false;
                DundasMap1.ZoomPanel.Visible = true;
                DundasMap1.NavigationPanel.Visible = true;
                DundasMap1.Viewport.EnablePanning = true;
                DundasMap1.Viewport.Zoom = 100;
                DundasMap1.ColorSwatchPanel.Visible = false;

                DundasMap1.Legends.Clear();
                // добавляем легенду
                Legend legend = new Legend("CostLegend");
                legend.Visible = true;
                legend.BackColor = Color.White;
                legend.Dock = PanelDockStyle.Left;
                legend.DockAlignment = DockAlignment.Far;
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
                legend.Title = "Уровень регистрируемой\nбезработицы";
                legend.AutoFitMinFontSize = 7;

                // добавляем легенду с символами
                Legend legend2 = new Legend("SymbolLegend");
                legend2.Visible = true;
                legend2.Dock = PanelDockStyle.Right;
                legend2.DockAlignment = DockAlignment.Far;
                legend2.BackColor = Color.White;
                legend2.BackSecondaryColor = Color.Gainsboro;
                legend2.BackGradientType = GradientType.DiagonalLeft;
                legend2.BackHatchStyle = MapHatchStyle.None;
                legend2.BorderColor = Color.Gray;
                legend2.BorderWidth = 1;
                legend2.BorderStyle = MapDashStyle.Solid;
                legend2.BackShadowOffset = 4;
                legend2.TextColor = Color.Black;
                legend2.Font = new Font("MS Sans Serif", 7, FontStyle.Regular);
                legend2.AutoFitText = true;
                legend2.Title = "Общая численность\n зарегистрированных безработных граждан";
                legend2.AutoFitMinFontSize = 7;

                DundasMap1.Legends.Add(legend2);
                DundasMap1.Legends.Add(legend);

                // добавляем правила раскраски
                DundasMap1.ShapeRules.Clear();
                ShapeRule rule = new ShapeRule();
                rule.Name = "CostRule";
                rule.Category = String.Empty;
                rule.ShapeField = "Cost";
                rule.DataGrouping = DataGrouping.EqualDistribution;
                rule.ColorCount = 5;
                rule.ColoringMode = ColoringMode.ColorRange;
                rule.FromColor = Color.Green;
                rule.MiddleColor = Color.Yellow;
                rule.ToColor = Color.Red;
                rule.BorderColor = Color.FromArgb(50, Color.Black);
                rule.GradientType = GradientType.None;
                rule.HatchStyle = MapHatchStyle.None;
                rule.ShowInLegend = "CostLegend";
                rule.LegendText = "#FROMVALUE{N1}% - #TOVALUE{N1}%";
                DundasMap1.ShapeRules.Add(rule);

                // добавляем поля
                DundasMap1.Shapes.Clear();
                DundasMap1.ShapeFields.Add("Name");
                DundasMap1.ShapeFields["Name"].Type = typeof(string);
                DundasMap1.ShapeFields["Name"].UniqueIdentifier = true;
                DundasMap1.ShapeFields.Add("Cost");
                DundasMap1.ShapeFields["Cost"].Type = typeof(double);
                DundasMap1.ShapeFields["Cost"].UniqueIdentifier = false;

                // добавляем поля для символов
                DundasMap1.SymbolFields.Add("UnemploymentPopulation");
                DundasMap1.SymbolFields["UnemploymentPopulation"].Type = typeof(double);
                DundasMap1.SymbolFields["UnemploymentPopulation"].UniqueIdentifier = false;

                // добавляем правила расстановки символов
                DundasMap1.SymbolRules.Clear();
                SymbolRule symbolRule = new SymbolRule();
                symbolRule.Name = "SymbolRule";
                symbolRule.Category = string.Empty;
                symbolRule.DataGrouping = DataGrouping.EqualInterval;
                symbolRule.SymbolField = "UnemploymentPopulation";
                symbolRule.ShowInLegend = "SymbolLegend";
                symbolRule.LegendText = "#FROMVALUE{N0} - #TOVALUE{N0}";
                DundasMap1.SymbolRules.Add(symbolRule);

                // звезды для легенды
                for (int i = 1; i < 4; i++)
                {
                    PredefinedSymbol predefined = new PredefinedSymbol();
                    predefined.Name = "PredefinedSymbol" + i;
                    predefined.MarkerStyle = MarkerStyle.Triangle;
                    predefined.Width = 5 + (i * 5);
                    predefined.Height = predefined.Width;
                    predefined.Color = Color.DarkViolet;
                    DundasMap1.SymbolRules["SymbolRule"].PredefinedSymbols.Add(predefined);
                }

                //AddMapLayer(DundasMap1, mapFolderName, "Выноски", CRHelper.MapShapeType.CalloutTowns);
                AddMapLayer(DundasMap1, mapFolderName, "Территории", CRHelper.MapShapeType.Areas);
                FillMap1Data();
                #endregion
            }
            catch { }
            try
            {
                #region Настройка карты 2

                DundasMap2.Meridians.Visible = false;
                DundasMap2.Parallels.Visible = false;
                DundasMap2.ZoomPanel.Visible = true;
                DundasMap2.ZoomPanel.Dock = PanelDockStyle.Left;
                DundasMap2.NavigationPanel.Visible = true;
                DundasMap2.NavigationPanel.Dock = PanelDockStyle.Left;
                DundasMap2.Viewport.EnablePanning = true;

                // добавляем легенду
                DundasMap2.Legends.Clear();

                // добавляем легенду раскраски
                Legend legend1 = new Legend("TensionLegend");
                legend1.Visible = true;
                legend1.Dock = PanelDockStyle.Left;
                legend1.DockAlignment = DockAlignment.Far;
                legend1.BackColor = Color.White;
                legend1.BackSecondaryColor = Color.Gainsboro;
                legend1.BackGradientType = GradientType.DiagonalLeft;
                legend1.BackHatchStyle = MapHatchStyle.None;
                legend1.BorderColor = Color.Gray;
                legend1.BorderWidth = 1;
                legend1.BorderStyle = MapDashStyle.Solid;
                legend1.BackShadowOffset = 4;
                legend1.TextColor = Color.Black;
                legend1.Font = new Font("MS Sans Serif", 7, FontStyle.Regular);
                legend1.AutoFitText = true;
                legend1.Title = "Уровень напряжённости\nна рынке труда, единица";
                legend1.AutoFitMinFontSize = 7;

                // добавляем легенду с символами
                Legend legend2 = new Legend("VacancyLegend");
                legend2.Visible = true;
                legend2.Dock = PanelDockStyle.Right;
                legend2.DockAlignment = DockAlignment.Far;
                legend2.BackColor = Color.White;
                legend2.BackSecondaryColor = Color.Gainsboro;
                legend2.BackGradientType = GradientType.DiagonalLeft;
                legend2.BackHatchStyle = MapHatchStyle.None;
                legend2.BorderColor = Color.Gray;
                legend2.BorderWidth = 1;
                legend2.BorderStyle = MapDashStyle.Solid;
                legend2.BackShadowOffset = 4;
                legend2.TextColor = Color.Black;
                legend2.Font = new Font("MS Sans Serif", 7, FontStyle.Regular);
                legend2.AutoFitText = true;
                legend2.Title = "Соотношение числа\n незанятых граждан\n и числа вакансий".Replace("незанятых", "безработных");
                legend2.AutoFitMinFontSize = 7;
                DundasMap2.Legends.Add(legend2);
                DundasMap2.Legends.Add(legend1);

                // добавляем поля для раскраски
                DundasMap2.ShapeFields.Clear();
                DundasMap2.ShapeFields.Add("Name");
                DundasMap2.ShapeFields["Name"].Type = typeof(string);
                DundasMap2.ShapeFields["Name"].UniqueIdentifier = true;
                DundasMap2.ShapeFields.Add("TensionKoeff");
                DundasMap2.ShapeFields["TensionKoeff"].Type = typeof(double);
                DundasMap2.ShapeFields["TensionKoeff"].UniqueIdentifier = false;

                // добавляем поля для символов
                DundasMap2.SymbolFields.Add("VacancyCount");
                DundasMap2.SymbolFields["VacancyCount"].Type = typeof(double);
                DundasMap2.SymbolFields["VacancyCount"].UniqueIdentifier = false;
                DundasMap2.SymbolFields.Add("RedundantCount");
                DundasMap2.SymbolFields["RedundantCount"].Type = typeof(double);
                DundasMap2.SymbolFields["RedundantCount"].UniqueIdentifier = false;

                LegendItem item = new LegendItem();
                item.Text = "Численность незанятых граждан, состоящих на учете, человек".Replace("незанятых", "безработных");
                item.Color = Color.DarkViolet;
                legend2.Items.Add(item);

                item = new LegendItem();
                item.Text = "Число заявленных вакансий, единица";
                item.Color = Color.Black;
                legend2.Items.Add(item);

                // добавляем правила раскраски
                DundasMap2.ShapeRules.Clear();
                ShapeRule rule = new ShapeRule();
                rule.Name = "TensionKoeffRule";
                rule.Category = String.Empty;
                rule.ShapeField = "TensionKoeff";
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
                rule.ShowInLegend = "TensionLegend";
                rule.LegendText = "#FROMVALUE{N2} - #TOVALUE{N2}";
                DundasMap2.ShapeRules.Add(rule);

                //AddMapLayer(DundasMap2, mapFolderName, "Выноски", CRHelper.MapShapeType.CalloutTowns);
                AddMapLayer(DundasMap2, mapFolderName, "Территории", CRHelper.MapShapeType.Areas);

                FillMapData2(DundasMap2);

                #endregion
            }
            catch { }
            CustomizerSize.ContfigurationGrid(UltraWebGrid);
            try
            {
                UltraWebGrid.Columns.Remove(UltraWebGrid.Columns[1]);
            }
            catch { }
		}

		#region Дополнительные функции только для этого отчета

		private void Years()
		{
			string query = DataProvider.GetQueryText("STAT_0003_0001_years");
			DataTable dtYears = new DataTable();
			DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Годы", dtYears);
			if (dtYears == null || dtYears.Columns.Count == 0)
			{
				compareYear.Value = GetYearFromMDXDate(compareDate.Value);
				selectedYear.Value = GetYearFromMDXDate(selectedDate.Value);
			}
			else
			{
				switch (dtYears.Columns.Count)
				{
					case 1:
						{
							compareYear.Value = GetYearFromMDXDate(compareDate.Value);
							selectedYear.Value = dtYears.Rows[0][0].ToString();
							break;
						}
					case 2:
						{
							compareYear.Value = dtYears.Rows[0][0].ToString();
							selectedYear.Value = dtYears.Rows[0][1].ToString();
							break;
						}
				}
			}
			if (selectedYear.Value == compareYear.Value)
			{
				compareYear.Value = Convert.ToString(Convert.ToUInt32(selectedYear.Value) - 1);
			}
		}

		#endregion

		#region Динамический текст

		protected void MakeDynamicText()
		{
			DataRow row0 = dtGrid.Rows[0];
			DataRow row1 = dtGrid.Rows[1];
			DataRow row2 = dtGrid.Rows[2];
            string text1 = "Численность официально зарегистрированных безработных, по состоянию на <b>{0}</b> составила <b>{1:N0}</b> человек. {2} числа безработных граждан за период с <b>{3}</b> по <b>{0}</b> в целом по Ямало-Ненецкому автономному округу составил{4} <b>{5:N0}</b> чел. (темп {6}<b>{7:P2}</b>).";
            string text2 = "<br/>Уровень регистрируемой безработицы в течение отчетного периода {0} на <b>{1:N2}</b> процентных пункта и на <b>{2}</b> составил <b>{3:N2}%</b>.";
			
            LabelText.Text = "";
            try
            {
                object[] values1 = new object[8];
                values1[0] = MDXDateToShortDateString(selectedDate.Value);
                values1[1] = Math.Abs((Convert.ToDouble(row0[2].ToString()) ));
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
                values1[3] = MDXDateToShortDateString(compareDate.Value);
                values1[5] = Math.Abs(Convert.ToDouble(row2[2].ToString()));
                values1[7] = Math.Abs(Convert.ToDouble(row1[2].ToString()));
                LabelText.Text = String.Format(text1, values1);
            }
            catch { }
            try
            {
                object[] values2 = new object[4];
                if (row1[3] != DBNull.Value)
                {
                    if (Convert.ToDouble(row1[3].ToString()) <= 0)
                    {
                        values2[0] = "уменьшился <img src='../../images/ArrowGreenDownBB.png'>";
                    }
                    else
                    {
                        values2[0] = "увеличился <img src='../../images/ArrowRedUpBB.png'>";
                    }
                    values2[1] = Math.Abs(Convert.ToDouble(row1[3].ToString()));
                    values2[2] = MDXDateToShortDateString(selectedDate.Value);
                    values2[3] = Math.Abs(Convert.ToDouble(row0[3].ToString()));
                }
                else
                {
                    text2 = String.Empty;
                }
                LabelText.Text += String.Format(text2, values2);
            }
            catch { }
		}

		#endregion

		// --------------------------------------------------------------------
		#region Обработчики карты 1

		/// <summary>
		/// Является ли форма городом-выноской
		/// </summary>
		/// <param name="shape">форма</param>
		/// <returns>true, если является</returns>
		public static bool IsCalloutTownShape(Shape shape)
		{
			return shape.Layer == CRHelper.MapShapeType.CalloutTowns.ToString();
		}

		/// <summary>
		/// Получение имени формы (с выделением имени из города-выноски)
		/// </summary>
		/// <param name="shape">форма</param>
		/// <returns>имя формы</returns>
		public static string GetShapeName(Shape shape)
		{
			string shapeName = shape.Name;
			if (IsCalloutTownShape(shape) && shape.Name.Split('_').Length > 1)
			{
				shapeName = shape.Name.Split('_')[0];
			}

			return shapeName;
		}

		private void AddMapLayer(MapControl map, string mapFolder, string layerFileName, CRHelper.MapShapeType shapeType)
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


        string FormatConditionString(string s)
        {
            return s.Replace("г.", "").Replace(" р-н", "").Replace("г. ", "").Replace(" район", "");
        }

		/// <summary>
		/// Поиск формы карты
		/// </summary>
		/// <param name="map">карта</param>
		/// <param name="patternValue">искомое имя формы</param>
		/// <returns>найденные формы</returns>
		public ArrayList FindMapShape(MapControl map, string patternValue, out bool hasCallout)
		{
			hasCallout = false;
			ArrayList shapeList = new ArrayList();
			foreach (Shape shape in map.Shapes)
			{

                //String.Format(shape.Name+"||"+patternValue);
                
                //if (GetShapeName(shape)
                //    .ToLower()
                //    .Replace(" ","")
                //    .Contains(patternValue
                //                        .ToLower()
                //                        .Replace(" ","")))
                if (FormatConditionString(shape.Name).Contains(FormatConditionString(patternValue)))
				{
					shapeList.Add(shape);
					if (IsCalloutTownShape(shape))
					{
						hasCallout = true;
					}
				}
			}

			return shapeList;
		}

		public void FillMap1Data()
		{
			string query = DataProvider.GetQueryText("STAT_0003_0001_map1");
			dtMap1 = new DataTable();
			DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование муниципального образования", dtMap1);
			bool hasCallout;
			string valueSeparator = IsMozilla ? ". " : "\n";
			string shapeHint = "{0} \nЧисленность безработных: {1:N0} чел.";
			LabelMap1.Text = String.Format(Map1TitleCaption, MDXDateToShortDateString(selectedDate.Value));
			if (dtMap1 == null || DundasMap1 == null || dtMap1.Columns.Count < 5)
			{
				return;
			}
			foreach (Shape shape in DundasMap1.Shapes)
			{
				shape.Text = String.Format("{0}", GetShapeName(shape).Replace(" ", "\n"));
				if (shape.Text.Substring(0, 2) != "г.")
				{ 
					shape.Text = shape.Text + " р-н";
					shape.TextVisibility = TextVisibility.Shown;
				}
			}
			foreach (DataRow row in dtMap1.Rows)
			{
				// заполняем карту данными
                try
                    {
				string subject = row[0].ToString().Replace(" муниципальный район", String.Empty).Replace("Город ", "г.");
				double value = Convert.ToDouble(row[4].ToString());
				double unemploymentPopulation = Convert.ToDouble(row[3].ToString());
				ArrayList shapeList = FindMapShape(DundasMap1, subject, out hasCallout);
                foreach (Shape shape in shapeList)
                {
                    
                        shape.Visible = true;
                        string shapeName = GetShapeName(shape);
                        shape["Name"] = subject;
                        if (row[4].ToString() != "бесконечность")
                        {
                        
                        shape["Cost"] = Convert.ToDouble(row[4].ToString());
                        }
                        if (IsCalloutTownShape(shape))
                        {
                            
                            shape.ToolTip = String.Format(shapeHint + (row[4].ToString() == "бесконечность" ? "" : " \nУровень безработицы: {2:N2}%"), row[0].ToString(), Convert.ToDouble(row[3].ToString()), Convert.ToDouble(row[4].ToString()));
                            shape.TextVisibility = TextVisibility.Shown;
                            shape.Text = String.Format("{0}\n{1:N0} чел.\n" + (row[4].ToString() == "бесконечность" ? "" : " {2:N2}%"), row[1].ToString(), Convert.ToDouble(row[3].ToString()), Convert.ToDouble(row[4].ToString()));
                            shape.TextVisibility = TextVisibility.Shown;
                            shape.TextAlignment = System.Drawing.ContentAlignment.BottomCenter;
                        }
                        else 
                        {
                            if (!hasCallout)
                            {
                                shape.ToolTip = String.Format(shapeHint+(row[4].ToString()=="бесконечность"?"":" \nУровень безработицы: {2:N2}%") , row[0].ToString(), Convert.ToDouble(row[3].ToString()), Convert.ToDouble(row[4].ToString()));
                                shape.TextVisibility = TextVisibility.Shown;
                                shape.Text = String.Format("{0}\n{1:N0} чел." + (row[4].ToString() == "бесконечность" ? "" : " {2:N2}%"), row[1].ToString(), Convert.ToDouble(row[3].ToString()), Convert.ToDouble(row[4].ToString()));
                                Symbol symbol = new Symbol();
                                symbol.Name = shape.Name + DundasMap1.Symbols.Count;
                                symbol.ParentShape = shape.Name;
                                symbol["UnemploymentPopulation"] = unemploymentPopulation;
                                symbol.Color = Color.DarkViolet;
                                symbol.MarkerStyle = MarkerStyle.Triangle;
                                symbol.Offset.Y = -30;
                                symbol.Layer = CRHelper.MapShapeType.Areas.ToString();
                                DundasMap1.Symbols.Add(symbol);
                            }
                            else
                            {
                                shape.Visible = false;
                            }
                        }

                }
                    }
                catch { }
			}
		}

		#endregion

		#region Обработчики карты 2

		public void FillMapData2(MapControl map)
		{
			LabelMap2.Text = String.Format(Map2TitleCaption, MDXDateToShortDateString(selectedDate.Value));
			if (map == null)
			{
				return;
			}

			string query = DataProvider.GetQueryText("STAT_0003_0001_map2");

			dtMap2 = new DataTable();
			DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Показатель", dtMap2);

			foreach (DataRow row in dtMap2.Rows)
			{
				// заполняем карту данными
				if (row[0] != DBNull.Value && row[0].ToString() != string.Empty &&
					row[1] != DBNull.Value && row[1].ToString() != string.Empty &&
					row[2] != DBNull.Value && row[2].ToString() != string.Empty)
				{
					string regionName = row[0].ToString().Replace(" муниципальный район", String.Empty).Replace("Город ", "г.");

					bool hasCallout;
					ArrayList shapeList = FindMapShape(map, regionName, out hasCallout);
					foreach (Shape shape in shapeList)
					{
						double tensionKoeff = Convert.ToDouble(row[1]);
						double redundantCount = Convert.ToDouble(row[2]);
						double vacancyCount = Convert.ToDouble(row[3]);
						double totalCount = vacancyCount + redundantCount;

						shape["Name"] = row[0].ToString();
						shape["TensionKoeff"] = tensionKoeff;
						if (!IsCalloutTownShape(shape) && (hasCallout))
						{
							shape.Visible = false;
						}
						else
						{
                            shape.ToolTip = String.Format("{0} \nУровень напряжённости на рынке труда, единица: {1:N2}" + (BN == "IE" ? "\n" : "\n") + "Число незанятых граждан: {2:N0} чел. \nЧисло вакансий: {3:N0}".Replace("незанятых", "безработных"),
									row[0].ToString(), tensionKoeff, redundantCount, vacancyCount);

							shape.TextColor = Color.Black;
							shape.BorderWidth = 2;
							shape.TextVisibility = TextVisibility.Shown;
							if (!IsCalloutTownShape(shape))
							{
								shape.Text = string.Format("{0}\nвакансий: {2:N0}\n{1:N2}", row[4].ToString(), tensionKoeff, vacancyCount);
								Symbol symbol = new Symbol();
								symbol.Name = shape.Name + map.Symbols.Count;
								symbol.ParentShape = shape.Name;
								symbol["vacancyCount"] = totalCount == 0 ? 0 : vacancyCount / totalCount * 100;
								symbol["redundantCount"] = totalCount == 0 ? 0 : redundantCount / totalCount * 100;
								symbol.Offset.Y = -33;
								symbol.MarkerStyle = MarkerStyle.Circle;
								map.Symbols.Add(symbol);
							}
							else
							{
								shape.TextAlignment = System.Drawing.ContentAlignment.BottomCenter;
								shape.Text = string.Format("{0}\nвак.: {2:N0}\n{1:N2}", row[4].ToString(), tensionKoeff, vacancyCount);
							}
						}
					}
				}
			}
		}

		void DundasMap2_PostPaint(object sender, MapPaintEventArgs e)
		{
			Symbol symbol = e.MapElement as Symbol;
			if (symbol != null && symbol.Visible)
			{
				// Размер диаграммы
				int width = 30;
				int height = 30;

				// Get the symbol location in pixels.
				MapGraphics mg = e.Graphics;
				PointF p = symbol.GetCenterPointInContentPixels(mg);
				int x = (int)p.X - width / 2;
				int y = (int)p.Y - height / 2;
				symbol.Width = width;
				symbol.Height = height;

				int startAngle, sweepAngle1, sweepAngle2;

				// Делим углы соотвественно долям
				startAngle = 0;
				sweepAngle1 = Convert.ToInt32(3.6 * Convert.ToInt32(symbol["redundantCount"]));
				sweepAngle2 = Convert.ToInt32(3.6 * Convert.ToInt32(symbol["vacancyCount"]));

				// Поверх символа рисуем круговую диаграмму
				Graphics g = mg.Graphics;
				g.FillPie(new SolidBrush(Color.DarkViolet), x, y, width, height, startAngle, sweepAngle1);
				startAngle += sweepAngle1;
				g.FillPie(new SolidBrush(Color.Black), x, y, width, height, startAngle, sweepAngle2);

				g.DrawEllipse(new Pen(Color.Gray, 1), x, y, width, height);
			}
		}
		#endregion

		// --------------------------------------------------------------------


        class RankingField
        {
            class SortKeyVal : System.Collections.Generic.IComparer<KeyVal>
            {
                #region Члены IComparer<KeyVal>

                public int Compare(KeyVal x, KeyVal y)
                {
                    if (x.Val > y.Val)
                    {
                        return -1;
                    }
                    if (x.Val < y.Val)
                    {
                        return 1;
                    }
                    return 0;
                }

                #endregion
            }

            struct KeyVal
            {
                public string Key;
                public decimal Val;
            }
                
            List<KeyVal> Fields = new List<KeyVal>();

            public void AddItem(string Key, decimal Val)
            {
                KeyVal NewFild = new KeyVal();
                NewFild.Key = Key;
                NewFild.Val = Val;
                Fields.Add(NewFild);
            }

            public object GetRang(string Key)
            {                
                Fields.Sort(new SortKeyVal());

                int i = 0;
                foreach (KeyVal kv in Fields)
                {
                    i++;
                    
                    if (kv.Key.Split(';')[0] == Key.Split(';')[0])
                    {
                        return Fields.Count - i + 1;
                    }
                }
                return DBNull.Value;
            }
            
        }

		#region Обработчики грида
        class CoolTable : DataTable
        {
            public DataRow AddOrFindRow(string NameMO)
            {

                foreach (DataRow Row in this.Rows)
                {
                    if (Row[0].ToString() == NameMO)
                    {
                        return Row;
                    }
                }
                DataRow NewRow = this.NewRow();
                this.Rows.Add(NewRow);
                NewRow[0] = NameMO;
                return NewRow;
            }
        }

        private DataTable ConvertTable(DataTable Table)
        {   
            RankingField Ranked = new RankingField();
            RankingField Ranked1 = new RankingField();
            RankingField Ranked2 = new RankingField();
            CoolTable ReportTable = new CoolTable();
            ReportTable.Columns.Add("MO");
            for (int i = 1; i < Table.Columns.Count; i+=2)
            {
                DataColumn ColCurVal = Table.Columns[i+1];
                DataColumn ColPrevVal = Table.Columns[i];
                string ColumnName = ColCurVal.ColumnName.Split(';')[0];
                ReportTable.Columns.Add(ColumnName,typeof(decimal));

                int indeRow = 0;
                foreach (DataRow BaseRow in Table.Rows)
                {                    
                    DataRow RowValue = ReportTable.AddOrFindRow(BaseRow[0].ToString().Replace("(","").Replace("ДАННЫЕ)","") + ";Значение");
                    DataRow RowDeviation = ReportTable.AddOrFindRow(BaseRow[0].ToString().Replace("(", "").Replace("ДАННЫЕ)", "") + ";Отклонение");
                    DataRow RowSpeedDeviation = ReportTable.AddOrFindRow(BaseRow[0].ToString().Replace("(", "").Replace("ДАННЫЕ)", "") + ";Прирост");
                    if (BaseRow[ColCurVal] != DBNull.Value)
                    {
                        try
                        {
                            RowValue[ColumnName] = (decimal)BaseRow[ColCurVal];
                            if (BaseRow[ColPrevVal] != DBNull.Value)
                            {
                                RowSpeedDeviation[ColumnName] = (decimal)BaseRow[ColCurVal] - (decimal)BaseRow[ColPrevVal];
                                RowDeviation[ColumnName] = (decimal)BaseRow[ColCurVal] / (decimal)BaseRow[ColPrevVal] - 1;
                            }
                        }
                        catch 
                        { }
                    } 

                    if (indeRow > 0)
                    {
                        try
                        {
                            if (ReportTable.Columns.Count == 5)
                                Ranked.AddItem(RowValue[0].ToString(), (decimal)RowValue[3]);
                            if (ReportTable.Columns.Count == 7)
                                Ranked1.AddItem(RowValue[0].ToString(), (decimal)RowValue[6]);
                            if (ReportTable.Columns.Count == 8)
                                Ranked2.AddItem(RowValue[0].ToString(), (decimal)RowValue[7]);
                        }
                        catch { }
                    }
                    indeRow++;
                    
                }
                
            }
            int j = 1;
            foreach (DataRow Row in ReportTable.Rows)
            {
                if (j != 3)
                {
                    if (j % 3 == 0)
                    {
                        Row[3] = Ranked.GetRang(Row[0].ToString());
                        Row[6] = Ranked1.GetRang(Row[0].ToString());
                        Row[7] = Ranked2.GetRang(Row[0].ToString());
                    }
                }
                else
                {
                    Row[3] = DBNull.Value;
                    Row[6] = DBNull.Value;
                    Row[7] = DBNull.Value;
                }
                j++;                
            }
            return ReportTable;
        }

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
		{
            UltraWebGrid.Columns.Clear();
            UltraWebGrid.Bands.Clear();
            UltraWebGrid.Rows.Clear();
            string query = DataProvider.GetQueryText("STAT_0003_0001_grid_NOVOSIB");
			dtGrid = new DataTable();
			DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование муниципального образования", dtGrid);

            dtGrid = ConvertTable(dtGrid);



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
			//e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(200);
			e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
			int columnWidth = Convert.ToInt32((Convert.ToInt32(e.Layout.Grid.Width.Value) - CRHelper.GetColumnWidth(300)) / 8);


            if (CustomizerSize is CustomizerSize_800x600)
            {
                e.Layout.HeaderStyleDefault.Wrap = true;
                
                e.Layout.HeaderStyleDefault.VerticalAlign = VerticalAlign.Middle;
            }
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;

			headerLayout.AddCell("Территория");

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; ++i)
            {
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(columnWidth);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[i].CellStyle.Padding.Right = 5;
                e.Layout.Bands[0].Columns[i].CellStyle.Padding.Left = 5;
                GridHeaderCell headerCell = headerLayout.AddCell(e.Layout.Bands[0].Columns[i].Key);
            }

            
            
			headerLayout.ApplyHeaderInfo();

            for(int i = 1;i<e.Layout.Bands[0].Columns.Count;i++)
            {
                e.Layout.Bands[0].Columns[i].DataType = "decimal";
                e.Layout.Bands[0].Columns[i].Header.Style.Wrap = true;
            }
            //e.Layout.Bands[0].Columns[1].Hidden = true;
            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1].Hidden = true;
            e.Layout.Bands[0].Columns[4].Hidden = true;
		}

		protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
		{
			e.Row.Cells[0].Value = e.Row.Cells[0].GetText().Split(';')[0];
			for (int i = 2; i < e.Row.Cells.Count+1; ++i)
            {
                try
                {
                    UltraGridCell cell = e.Row.Cells[i - 1];
                    if (e.Row.Index % 3 != 0)
                    {
                        cell.Style.BorderDetails.StyleTop = BorderStyle.None;
                    }
                    if (e.Row.Index % 3 != 2)
                    {
                        cell.Style.BorderDetails.StyleBottom = BorderStyle.None;
                    }
                    if (e.Row.Index % 3 == 1 && i != 8)
                    {
                        if (i != 2)
                        {
                            cell.Title = "Темп прироста к " + MDXDateToShortDateString(compareDate.Value);
                        }
                        else
                        {
                            cell.Title = "Темп прироста к 01.01." + compareYear.Value;
                        }
                        if (cell.Value != null && Convert.ToDouble(cell.Value) != 0)
                        {
                            double value = Convert.ToDouble(cell.Value);
                            if (value < 0)
                            {
                                if (i == 3 || i == 4 || i == 5 || i == 7)
                                {
                                    cell.Style.BackgroundImage = "~/images/ArrowGreenDownBB.png";
                                    cell.Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                                }
                                else if (i != 8)
                                {
                                    cell.Style.BackgroundImage = "~/images/ArrowRedDownBB.png";
                                    cell.Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                                }
                            }
                            else
                            {
                                if (i == 3 || i == 4 || i == 5 || i == 7)
                                {
                                    cell.Style.BackgroundImage = "~/images/ArrowRedUpBB.png";
                                    cell.Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                                }
                                else if (i != 8)
                                {
                                    cell.Style.BackgroundImage = "~/images/ArrowGreenUpBB.png";
                                    cell.Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                                }
                            }
                        }
                    }
                    if (e.Row.Index % 3 == 0 && i == 8)
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
                    if (e.Row.Index % 3 == 2 && i != 4 && i != 7)
                    {
                        if (cell.Value != null && Convert.ToDouble(cell.Value) != 0)
                        {
                            double value = Convert.ToDouble(cell.Value);
                            if (value > 0)
                            {
                                cell.Title = i != 2 ? "Прирост к " + MDXDateToShortDateString(compareDate.Value) : "Прирост к 01.01." + compareYear.Value;
                            }
                            else
                            {
                                cell.Title = i != 2 ? "Снижение относительно " + MDXDateToShortDateString(compareDate.Value) : "Снижение относительно 01.01." + compareYear.Value;
                            }
                        }
                    }
                    if ((i == 4 || i == 7) && (e.Row.Index != 2) && (e.Row.Index % 3 == 2))
                    {
                        if (Convert.ToInt32(cell.Value) == 1)
                        {
                            cell.Style.BackgroundImage = "~/images/StarYellowBB.png";
                            cell.Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                            if (i == 4)
                            {
                                cell.Title = String.Format("Ранг по Ямало-Ненецкому автономному округу: {0} \nСамый низкий уровень регистрируемой безработицы", cell.Value);
                            }
                            else
                            {
                                cell.Title = String.Format("Ранг по Ямало-Ненецкому автономному округу: {0} \n Самый низкий уровень напряжённости на рынке труда", cell.Value);
                            }
                        }
                        else if (Convert.ToInt32(cell.Value) == ((dtGrid.Rows.Count / 3) - 1))
                        {
                            
                            cell.Style.BackgroundImage = "~/images/StarGrayBB.png";
                            cell.Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                            if (i == 4)
                            {
                                cell.Title = String.Format("Ранг по Ямало-Ненецкому автономному округу: {0} \nСамый высокий уровень регистрируемой  безработицы", cell.Value);
                            }
                            else
                            {
                                cell.Title = String.Format("Ранг по Ямало-Ненецкому автономному округу: {0} \nСамый высокий уровень напряжённости на рынке труда", cell.Value);
                            }
                        }
                        else
                        {
                            cell.Title = String.Format("Ранг по Ямало-Ненецкому автономному округу: {0}", cell.Value);
                        }
                    }
                    switch (e.Row.Index % 3)
                    {
                        case 0:
                            {
                                if (i == 4 || i == 7)
                                {
                                    cell.Value = String.Format("<b>{0:N2}</b>", cell.Value);
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
                                cell.Value = String.Format("{0:N0}", cell.Value);
                                break;
                            }
                    }
                }
                catch { }
			}
		}

		#endregion

		// --------------------------------------------------------------------

		#region Обработчики диаграммы
		protected void UltraChart1_DataBinding(object sender, EventArgs e)
		{
            
            LabelChart1.Text = Chart1TitleCaption;
			UltraChart1.Tooltips.FormatString = "<SERIES_LABEL>, на <ITEM_LABEL>\n<b><DATA_VALUE:N0></b>";
			string query = DataProvider.GetQueryText("STAT_0003_0001_chart");
			dtChart = new DataTable();
			DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Параметры", dtChart);
			if (dtChart != null)
			{
				query = DataProvider.GetQueryText("STAT_0003_0001_chart_description");
				DataTable dtChartDescription = new DataTable();
				DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Описание столбцов", dtChartDescription);
				for (int i = 1; i < dtChart.Columns.Count && i < dtChartDescription.Columns.Count; ++i)
				{
					dtChart.Columns[i].ColumnName = MDXDateToShortDateString(dtChartDescription.Rows[0][i].ToString()).Substring(0, 10);
				}
                try
                {
                    dtChart.Rows[0][0] = "Численность официально  зарегистрированных безработных, человек".Replace("незанятых", "безработных");
                
                    dtChart.Rows[1][0] = "Число заявленных вакансий, единица";
                }
                catch { }
				UltraChart1.DataSource = dtChart.DefaultView;
			}
			else
			{
				UltraChart1.DataSource = null;
			}
		}

		protected void UltraChart1_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
		{
			//foreach (Primitive primitive in e.SceneGraph.P)
		}

		#endregion

		// --------------------------------------------------------------------

		#region Заполнение словарей и выпадающих списков параметров

		protected void FillComboDate(CustomMultiCombo combo, string queryName, int offset)
		{
			// Загрузка списка актуальных дат
			dtDate = new DataTable();
			string query = DataProvider.GetQueryText(queryName);
			DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
			// Закачку придется делать через словарь
			Dictionary<string, int> dictDate = new Dictionary<string, int>();
			for (int row = 0; row < dtDate.Rows.Count - offset; ++row)
			{
				string year = dtDate.Rows[row][0].ToString();
				string month = dtDate.Rows[row][3].ToString();
				string day = dtDate.Rows[row][4].ToString();
				AddPairToDictionary(dictDate, year + " год", 0);
				AddPairToDictionary(dictDate, month + " " + year + " года", 1);
				AddPairToDictionary(dictDate, day + " " + CRHelper.RusMonthGenitive(CRHelper.MonthNum(month)) + ' ' + year + " года", 2);
			}
			combo.FillDictionaryValues(dictDate);
			combo.SelectLastNode();
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

		public string MDXDateToShortDateString(string MDXDateString)
		{
			string[] separator = { "].[" };
			string[] dateElements = MDXDateString.Split(separator, StringSplitOptions.None);
			string template = "{0}.{1}.{2}";
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
			ISection section4 = report.AddSection();

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
				if (row.Index % 3 != 0)
				{
					row.Cells[0].Style.BorderDetails.StyleTop = BorderStyle.None;
				}
				else
				{
					row.Cells[0].Value = null;
				}
				if (row.Index % 3 != 2)
				{
					row.Cells[0].Style.BorderDetails.StyleBottom = BorderStyle.None;
				}
				else
				{
					row.Cells[0].Value = null;
				}
			}

			foreach (GridHeaderCell cell in headerLayout.childCells)
			{
				cell.Caption = cell.Caption.Replace("зарегистрирован-н", "зарегистрированн");
			}

			ReportPDFExporter1.HeaderCellHeight = 120;

            headerLayout.childCells.RemoveAt(1);
			ReportPDFExporter1.Export(headerLayout, section1);

			section2.PagePaddings = section1.PagePaddings;
			section2.PageMargins = section1.PageMargins;
			section2.PageBorders = section1.PageBorders;
			section2.PageSize = new PageSize(section1.PageSize.Height, section1.PageSize.Width);
			section2.PageOrientation = PageOrientation.Landscape;
			title = section2.AddText();
			title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
			title.AddContent(LabelMap1.Text);
			DundasMap1.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
			DundasMap1.ZoomPanel.Visible = false;
			DundasMap1.NavigationPanel.Visible = false;
			Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromMap(DundasMap1);
			section2.AddImage(img);

			section3.PagePaddings = section1.PagePaddings;
			section3.PageMargins = section1.PageMargins;
			section3.PageBorders = section1.PageBorders;
			section3.PageSize = new PageSize(section1.PageSize.Height, section1.PageSize.Width);
			section3.PageOrientation = PageOrientation.Landscape;
			title = section3.AddText();
			title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
			title.AddContent(LabelMap2.Text);
			DundasMap2.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
			DundasMap2.ZoomPanel.Visible = false;
			DundasMap2.NavigationPanel.Visible = false;
			img = UltraGridExporter.GetImageFromMap(DundasMap2);
			section3.AddImage(img);

			section4.PagePaddings = section1.PagePaddings;
			section4.PageMargins = section1.PageMargins;
			section4.PageBorders = section1.PageBorders;
			section4.PageSize = new PageSize(section1.PageSize.Height, section1.PageSize.Width);
		    	section4.PageOrientation = PageOrientation.Landscape;

            title = section4.AddText();
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(LabelChart1.Text);
            //
			ReportPDFExporter1.Export(UltraChart1, "", section4);
		}
		
		#endregion

		#region Экспорт в Excel

		private void ExcelExportButton_Click(object sender, EventArgs e)
		{
			ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
			ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;

			Workbook workbook = new Workbook();
			Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
			Worksheet sheet2 = workbook.Worksheets.Add("Карта 1");
			Worksheet sheet3 = workbook.Worksheets.Add("Карта 2");
			Worksheet sheet4 = workbook.Worksheets.Add("Диаграмма");

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

			foreach (GridHeaderCell cell in headerLayout.childCells)
			{
				cell.Caption = cell.Caption.Replace("зарегистрирован-н", "зарегистрированн");
			}

            headerLayout.childCells.RemoveAt(1);
            
            headerLayout.childCells.RemoveAt(4);
            headerLayout.childCells.RemoveAt(headerLayout.childCells.Count - 1);

			ReportExcelExporter1.Export(headerLayout, sheet1, 7);

			foreach (UltraGridRow row in UltraWebGrid.Rows)
			{
				sheet1.Rows[9 + row.Index].Height = 255;
			}
			sheet1.Rows[8].Height = 255;
			/*for (int i = 0; i < UltraWebGrid.Rows.Count; ++i)
			{
				sheet1.Rows[9 + i].Height = 255;
				for (int j = 0; j < UltraWebGrid.Columns.Count; ++j)
				{
					WorksheetCell cell = sheet1.Rows[9 + i].Cells[j];
					cell.Value = null;
				}
			}*/
			//sheet1.Columns[5].Width = (int)(sheet1.Columns[8].Width);

			//sheet1.MergedCellsRegions.Clear();
			//sheet1.MergedCellsRegions.Add(7, 0, 8, 0);
			// Вывод динамичского текста
            sheet1.MergedCellsRegions.Clear();
            sheet1.MergedCellsRegions.Add(1, 0, 1, 10);
			sheet1.MergedCellsRegions.Add(3, 0, 3, 10);
			sheet1.MergedCellsRegions.Add(4, 0, 4, 10);
			sheet1.MergedCellsRegions.Add(5, 0, 5, 10);
			sheet1.Rows[3].Height = 510;
			string[] separator1 = { "<br/>" };
			string[] text = LabelText.Text.Split(separator1, StringSplitOptions.None);
			for (int i = 0; i < 3; ++i)
			{
                try
                {
                    sheet1.Rows[3 + i].Cells[0].Value = Regex.Replace(text[i], "<[\\s\\S]*?>", String.Empty);
                    sheet1.Rows[3 + i].Cells[0].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                }
                catch { }
			}

			sheet1.Rows[0].Cells[0].CellFormat.WrapText = ExcelDefaultableBoolean.False;
			sheet1.Rows[0].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
			sheet1.Rows[1].Cells[0].CellFormat.WrapText = ExcelDefaultableBoolean.False;
			sheet1.Rows[1].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
			for (int i = 0; i < UltraWebGrid.Rows.Count; i += 3)
			{
				for (int j = 0; j < UltraWebGrid.Columns.Count; ++j)
				{
					sheet1.Rows[9 + i].Cells[j].CellFormat.BottomBorderStyle = CellBorderLineStyle.None;
					sheet1.Rows[10 + i].Cells[j].CellFormat.BottomBorderStyle = CellBorderLineStyle.None;
					sheet1.Rows[10 + i].Cells[j].CellFormat.TopBorderStyle = CellBorderLineStyle.None;
					sheet1.Rows[11 + i].Cells[j].CellFormat.TopBorderStyle = CellBorderLineStyle.None;
				}
			}
			ReportExcelExporter1.WorksheetTitle = String.Empty;
			ReportExcelExporter1.WorksheetSubTitle = String.Empty;
			sheet2.Rows[0].Cells[0].Value = LabelMap1.Text;
			DundasMap1.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.75));
            DundasMap1.Height = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.5));
			ReportExcelExporter.MapExcelExport(sheet2.Rows[1].Cells[0], DundasMap1);
			sheet3.Rows[0].Cells[0].Value = LabelMap2.Text;
			DundasMap2.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.75));
            DundasMap2.Height = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.5));
			ReportExcelExporter.MapExcelExport(sheet3.Rows[1].Cells[0], DundasMap2);
			UltraChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.6);
			UltraChart1.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.6);
			ReportExcelExporter1.Export(UltraChart1, LabelChart1.Text, sheet4, 1);
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
