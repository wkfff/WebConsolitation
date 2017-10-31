using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.DocumentExport;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using EndExportEventArgs = Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Excel;

namespace Krista.FM.Server.Dashboards.reports.STAT_0001_0012_Sahalin
{
    public partial class _default : CustomReportPage
    {
        #region Поля

        private DataTable dtGrid;
        private DataTable dtChart;
        private DataTable dtChart4;
        private DataTable dtDate;
        private DataTable dtOperative;


        #endregion

        private CustomParam dataSource;
        private GridHeaderLayout headerLayout;
        private static bool IsSmallResolution
        {
            get { return CRHelper.GetScreenWidth < 1200; }
        }

        private static int MinScreenWidth
        {
            get { return IsSmallResolution ? 750 : CustomReportConst.minScreenWidth; }
        }

        private Dictionary<string, string> ShortRegionsNames
        {
            get
            {
                // если короткие имена регионов еще не получены
                if (shortRegionsNames == null || shortRegionsNames.Count == 0)
                {
                    // заполняем словарик
                    FillShortRegionsNames();
                }
                return shortRegionsNames;
            }
        }

        private  Dictionary<string, string> shortRegionsNames;

        private void FillShortRegionsNames()
        {
            shortRegionsNames = new Dictionary<string, string>();
            string query = DataProvider.GetQueryText("shortRegionsNames");
            DataTable dt = new DataTable();

            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Регион", dt);
            foreach (DataRow row in dt.Rows)
            {
                // пока нет нормального запроса с именами ФО, будем делать глупо.
                string key = row[0].ToString();
                key = key.Trim('(');
                key = key.Replace(" ДАННЫЕ)", string.Empty);
                shortRegionsNames.Add(key, row[1].ToString());
            }
        }

        private  string ShortName(string fullName)
        {
            if (fullName == null)
            {
                return String.Empty;
            }

            if (fullName == "Российская Федерация")
            {
                return "РФ";
            }
            if (ShortRegionsNames.ContainsKey(fullName))
            {
                return ShortRegionsNames[fullName];
            }
            return fullName;
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid1.Width = Unit.Empty;
            UltraWebGrid1.Height = Unit.Empty;
            UltraWebGrid1.DisplayLayout.NoDataMessage = "Нет данных";
            //UltraWebGrid1.DataBound += new EventHandler(UltraWebGrid_DataBound);

            UltraChart1.Width = CRHelper.GetChartWidth(MinScreenWidth - 70);
            UltraChart1.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight / 2);

            UltraChart2.Width = CRHelper.GetChartWidth(MinScreenWidth - 70);
            UltraChart2.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight / 2);

            UltraChart3.Width = CRHelper.GetChartWidth((CustomReportConst.minScreenWidth - 30) / 3);
            UltraChart3.Height = 400;//CRHelper.GetChartHeight(CustomReportConst.minScreenHeight / 3);

            UltraChart4.Width = CRHelper.GetChartWidth(MinScreenWidth / 2.4);
            UltraChart4.Height = 400;//CRHelper.GetChartHeight(CustomReportConst.minScreenHeight / 2.4);

           // UltraChart4.Visible = !IsSmallResolution;


            #region Настройка диаграммы

            SetupDynamicChart(UltraChart1, "<SERIES_LABEL>\n<ITEM_LABEL>\n<DATA_VALUE:P2>", "<DATA_VALUE:P0>");
            UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

            SetupDynamicChart(UltraChart2, "<SERIES_LABEL>\n<ITEM_LABEL>\n<DATA_VALUE:N0> человек", "<DATA_VALUE:N0>");
            UltraChart2.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
          //  UltraChart2.Legend.Margins.Right = IsSmallResolution ? 0 : (int)UltraChart2.Width.Value / 2;
            UltraChart2.TitleLeft.Visible = true;
            UltraChart2.TitleLeft.Text = "человек";
            UltraChart2.TitleLeft.HorizontalAlign = StringAlignment.Center;

            LineAppearance lineAppearance = new LineAppearance();
            lineAppearance.IconAppearance.Icon = SymbolIcon.Circle;
            lineAppearance.IconAppearance.IconSize = SymbolIconSize.Small;
            lineAppearance.Thickness = 3;
            lineAppearance.SplineTension = (float)0.3;
            UltraChart2.SplineChart.LineAppearances.Add(lineAppearance);

            UltraChart3.ChartType = ChartType.PieChart;
            UltraChart3.Border.Thickness = 0;
            UltraChart3.Axis.Y.Extent = 45;
            UltraChart3.Axis.X.Extent = 40;
            UltraChart3.Tooltips.FormatString = "<ITEM_LABEL>\n<DATA_VALUE:N0> человек (<PERCENT_VALUE:N2>%)";
            UltraChart3.Legend.Visible = true;
            UltraChart3.Legend.Location = LegendLocation.Bottom;
            UltraChart3.Legend.SpanPercentage = 17;
            UltraChart3.Axis.X.Labels.SeriesLabels.Visible = false;
            UltraChart3.Axis.X.Labels.Orientation = TextOrientation.Horizontal;
            //UltraChart3.Axis.X.Labels.Font = new Font("Verdana", 12);
            UltraChart3.Axis.X.Labels.FontColor = Color.Black;
            UltraChart3.Data.ZeroAligned = true;
            UltraChart3.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart3.PieChart.Labels.FormatString = "<PERCENT_VALUE:N2>%";
        //    UltraChart3.Legend.Font = new Font("Microsoft Sans Serif", 8);
            UltraChart3.ChartDrawItem += new ChartDrawItemEventHandler(UltraChart3_ChartDrawItem);
            //UltraChart1.TitleLeft.Visible = true;
            // UltraChart1.TitleLeft.Text = "руб.";
            //UltraChart1.TitleLeft.HorizontalAlign = StringAlignment.Far;

            UltraChart3.ColorModel.ModelStyle = ColorModels.CustomSkin;
            // UltraChart3.ColorModel.Skin.ApplyRowWise = false;
            UltraChart3.ColorModel.Skin.PEs.Clear();
            for (int i = 1; i <= 2; i++)
            {
                PaintElement pe = new PaintElement();
                Color color = Color.White;
                Color stopColor = Color.White;
                switch (i)
                {
                    case 2:
                        {
                            color = Color.Green;
                            stopColor = Color.ForestGreen;
                            break;
                        }
                    case 1:
                        {
                            color = Color.Red;
                            stopColor = Color.Gold;
                            break;
                        }
                }
                pe.Fill = color;
                pe.FillStopColor = stopColor;
                pe.ElementType = PaintElementType.Gradient;
                pe.FillGradientStyle = GradientStyle.ForwardDiagonal;
                pe.FillOpacity = 150;
                UltraChart3.ColorModel.Skin.PEs.Add(pe);
            }

            UltraChart3.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart3_FillSceneGraph);

            UltraChart4.ChartType = ChartType.StackColumnChart;
            UltraChart4.StackChart.StackStyle = StackStyle.Complete;
            UltraChart4.Border.Thickness = 0;
            UltraChart4.Axis.Y.Extent = 35;
            UltraChart4.Axis.X.Extent = 150;
            UltraChart4.Tooltips.FormatString = "<SERIES_LABEL>\n<ITEM_LABEL>";
            UltraChart4.Legend.Visible = false;
            UltraChart4.Axis.X.Labels.SeriesLabels.Visible = true;
            UltraChart4.Axis.X.Labels.SeriesLabels.Layout.Behavior = AxisLabelLayoutBehaviors.None;
            UltraChart4.Axis.X.Labels.SeriesLabels.Font = new Font("Verdana", 10);
            UltraChart4.Axis.X.Labels.SeriesLabels.FontColor = Color.Black;
            UltraChart4.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart4.Data.ZeroAligned = true;
            UltraChart4.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart4.Axis.Y.TickmarkStyle = AxisTickStyle.Percentage;
            UltraChart4.Axis.Y.TickmarkPercentage = 25;
            UltraChart4.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>%";
            UltraChart4.TitleLeft.Visible = true;
            UltraChart4.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart4.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart4_FillSceneGraph);
            UltraChart4.Annotations.Visible = true;

            UltraChart4.ColorModel.ModelStyle = ColorModels.CustomSkin;
            UltraChart4.ColorModel.Skin.ApplyRowWise = false;
            UltraChart4.ColorModel.Skin.PEs.Clear();
            for (int i = 1; i <= 2; i++)
            {
                PaintElement pe = new PaintElement();
                Color color = Color.White;
                Color stopColor = Color.White;
                switch (i)
                {
                    case 2:
                        {
                            color = Color.Green;
                            stopColor = Color.ForestGreen;
                            break;
                        }
                    case 1:
                        {
                            color = Color.Red;
                            stopColor = Color.Gold;
                            break;
                        }
                }
                pe.Fill = color;
                pe.FillStopColor = stopColor;
                pe.ElementType = PaintElementType.Gradient;
                pe.FillGradientStyle = GradientStyle.ForwardDiagonal;
                pe.FillOpacity = 150;
                UltraChart4.ColorModel.Skin.PEs.Add(pe);
            }

            #endregion
            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportExcelExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);

            ReportExcelExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
           /* UltraGridExporter1.Visible = true;
            UltraGridExporter1.MultiHeader = false;
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);

            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler
                    <DocumentExportEventArgs>(PdfExporter_BeginExport);
            UltraGridExporter1.PdfExporter.EndExport += new EventHandler<Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs>(PdfExporter_EndExport);*/
        }

        private static void SetupDynamicChart(UltraChart chart, string tooltipsFormatString, string axisYLabelsFormatString)
        {
            chart.ChartType = ChartType.SplineChart;
            chart.Border.Thickness = 0;
            chart.Axis.Y.Extent = 45; 
            chart.Axis.X.Extent = 100;
            chart.Tooltips.FormatString = tooltipsFormatString;
            chart.Legend.Visible = true;
            chart.Legend.Location = LegendLocation.Bottom;
            chart.Legend.SpanPercentage = IsSmallResolution ? 25 : 20;
            chart.Axis.X.Labels.SeriesLabels.Visible = false;
            chart.Axis.X.Labels.Orientation = TextOrientation.VerticalLeftFacing;
            chart.Axis.X.Labels.Font = new Font("Verdana", 8);
            chart.Axis.X.Labels.FontColor = Color.Black;
            chart.Data.ZeroAligned = true;
            chart.SplineChart.NullHandling = NullHandling.DontPlot;
            chart.ColorModel.ModelStyle = ColorModels.CustomLinear;
            chart.Data.SwapRowsAndColumns = true;
            chart.Axis.Y.Labels.ItemFormatString = axisYLabelsFormatString;
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            dtDate = new DataTable();
            string query = DataProvider.GetQueryText("STAT_0001_0012_date");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            UserParams.PeriodYear.Value = dtDate.Rows[0][0].ToString();
            FillPeriodCombo();

            if (!Page.IsPostBack)
            { 
                FillComboRegions();
                ComboRegion.Title = "Территория";
                ComboRegion.Width = 400;
                ComboRegion.SelectLastNode();
                ComboRegion.ParentSelect = true;

                ComboYear.Width = 200;
                ComboYear.Title = "Месяц";
                ComboYear.MultiSelect = false;
                ComboYear.ParentSelect = false;
                ComboYear.FillDictionaryValues(periodDictionary);
                ComboYear.SelectLastNode();
            }
            try
            {
                UserParams.PeriodYear.Value = comboToPeriodDictionary[ComboYear.SelectedValue];
            }
            catch
            {
                UserParams.PeriodYear.Value =
                    "[Период].[Год Квартал Месяц].[Данные всех периодов].[2010].[Полугодие 1].[Квартал 1].[Январь]";
            }
            Page.Title = "Анализ динамики уровня официальной безработицы и уровня общей безработицы";
            PageTitle.Text = "Анализ динамики уровня официальной зарегистрированной безработицы и уровня общей безработицы по методологии МОТ";

            DateTime date = CRHelper.PeriodDayFoDate(UserParams.PeriodYear.Value);
            PageSubTitle.Text = string.Format("{0}, в {1} {2} года.", ComboRegion.SelectedValue, CRHelper.RusMonthPrepositional(date.Month), date.Year);

            UserParams.SubjectFO.Value = ComboRegion.SelectedValue == "Дальневосточный федеральный округ"
                                             ? String.Empty
                                             : String.Format(".[{0}]", ComboRegion.SelectedValue);

            UserParams.Filter.Value = ComboRegion.SelectedValue == "Дальневосточный федеральный округ"
                                             ? ".DataMember"
                                             : String.Format(".[{0}]", ComboRegion.SelectedValue);

            chart1ElementCaption.Text = String.Format("Уровень безработицы (от экономически активного населения), {0}",ShortName(ComboRegion.SelectedValue));
            chart2ElementCaption.Text = String.Format("Численность безработных на конец месяца, {0}", ShortName(ComboRegion.SelectedValue));
            chart3ElementCaption.Text = String.Format("Уровень общей безработицы по методологии МОТ, {0}", ShortName(ComboRegion.SelectedValue));

            dataSource = UserParams.CustomParam("data_source");

            DataTable dtDataSource = new DataTable();
            query = DataProvider.GetQueryText("STAT_0001_0012_source");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "dummy", dtDataSource);
            dataSource.Value = dtDataSource.Rows[0][1].ToString();
            lbDescription.Text = GetDescritionText(date);


            query = DataProvider.GetQueryText(String.Format("STAT_0001_0012_unjobedOperative"));
            dtOperative = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "dummy", dtOperative);

            LineAppearance lineAppearance = new LineAppearance();
            lineAppearance.IconAppearance.Icon = SymbolIcon.Circle;
            lineAppearance.IconAppearance.IconSize = SymbolIconSize.Small;
            lineAppearance.Thickness = 3;
            lineAppearance.SplineTension = (float)0.3;
            lineAppearance.LineStyle.DrawStyle = LineDrawStyle.Dot;
            UltraChart1.SplineChart.LineAppearances.Add(lineAppearance);

            lineAppearance = new LineAppearance();
            lineAppearance.IconAppearance.Icon = SymbolIcon.Circle;
            lineAppearance.IconAppearance.IconSize = SymbolIconSize.Small;
            lineAppearance.Thickness = 3;
            lineAppearance.SplineTension = (float)0.3;
            lineAppearance.LineStyle.DrawStyle = LineDrawStyle.Dot;
            UltraChart1.SplineChart.LineAppearances.Add(lineAppearance);

            lineAppearance = new LineAppearance();
            lineAppearance.IconAppearance.Icon = SymbolIcon.Circle;
            lineAppearance.IconAppearance.IconSize = SymbolIconSize.Small;
            lineAppearance.Thickness = 3;
            lineAppearance.SplineTension = (float)0.3;
            if (ComboRegion.SelectedValue != "Дальневосточный федеральный округ")
            {
                lineAppearance.LineStyle.DrawStyle = LineDrawStyle.Dash;
            }
            else
            {
                lineAppearance.LineStyle.DrawStyle = LineDrawStyle.Solid;
            }
            UltraChart1.SplineChart.LineAppearances.Add(lineAppearance);

            lineAppearance = new LineAppearance();
            lineAppearance.IconAppearance.Icon = SymbolIcon.Circle;
            lineAppearance.IconAppearance.IconSize = SymbolIconSize.Small;
            lineAppearance.Thickness = 3;
            lineAppearance.SplineTension = (float)0.3;
            if (ComboRegion.SelectedValue != "Дальневосточный федеральный округ")
            {
                lineAppearance.LineStyle.DrawStyle = LineDrawStyle.Dash;
            }
            else
            {
                lineAppearance.LineStyle.DrawStyle = LineDrawStyle.Solid;
            }
            UltraChart1.SplineChart.LineAppearances.Add(lineAppearance);

            lineAppearance = new LineAppearance();
            lineAppearance.IconAppearance.Icon = SymbolIcon.Circle;
            lineAppearance.IconAppearance.IconSize = SymbolIconSize.Small;
            lineAppearance.Thickness = 3;
            lineAppearance.SplineTension = (float)0.3;
            UltraChart1.SplineChart.LineAppearances.Add(lineAppearance);

            lineAppearance = new LineAppearance();
            lineAppearance.IconAppearance.Icon = SymbolIcon.Circle;
            lineAppearance.IconAppearance.IconSize = SymbolIconSize.Small;
            lineAppearance.Thickness = 3;
            lineAppearance.SplineTension = (float)0.3;
            UltraChart1.SplineChart.LineAppearances.Add(lineAppearance);

            UltraChart3.TitleTop.Visible = true;
            UltraChart3.TitleTop.HorizontalAlign = StringAlignment.Center;
            UltraChart3.TitleTop.Text = ComboRegion.SelectedValue;
           // UltraChart3.TitleTop.Font = new Font("Verdana", 8);
        //    UltraWebGrid1.Bands.Clear();
            headerLayout = new GridHeaderLayout(UltraWebGrid1);
            UltraWebGrid1.DataBind();

            UltraChart1.DataBind();
            UltraChart2.DataBind();
            UltraChart3.DataBind();
            UltraChart4.DataBind();
        }

        private string GetDescritionText(DateTime date)
        {
            DataTable dtText = new DataTable();
            string query = DataProvider.GetQueryText("STAT_0001_0012_text");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtText);

            string description = String.Format(
                   "&nbsp;&nbsp;&nbsp;В {5} в {0} {1} года из <b>{2:N2}</b> тыс. человек экономически активного населения <b>{3:N2}</b> тыс. человек относятся к безработным, что составляет <b>{4:P2}</b>.<br/>",
                   CRHelper.RusMonthPrepositional(date.Month), date.Year, dtText.Rows[0][1], dtText.Rows[1][1],
                   dtText.Rows[2][1],ShortName(ComboRegion.SelectedValue));

            double thisMonth;
            double prevMonth;
            if (Double.TryParse(dtText.Rows[1][1].ToString(), out thisMonth) &&
                Double.TryParse(dtText.Rows[1][2].ToString(), out prevMonth))
            {
                double grownValue = prevMonth - thisMonth;
                string grown = grownValue < 0 ? "Рост" : "Снижение";
                string compile = grownValue < 0 ? "составил" : "составило";
                double grownTemp = thisMonth / prevMonth;
                description += String.Format("&nbsp;&nbsp;&nbsp;{0} {6} общего числа безработных граждан за {1} {2} года {5} <b>{3:N2}</b> тыс. человек (темп роста <b>{4:P2}</b>)<br/>", grown, CRHelper.RusMonth(date.Month), date.Year, Math.Abs(grownValue), grownTemp, compile, GetImage(grown));
            }

            description += String.Format("&nbsp;&nbsp;&nbsp;Уровень общей безработицы по методологии МОТ ");

            double thisMot;
            double ufoMot;
            if (ComboRegion.SelectedValue != "Дальневосточный федеральный округ")
            {
                if (Double.TryParse(dtText.Rows[2][1].ToString(), out thisMot) &&
                   Double.TryParse(dtText.Rows[3][1].ToString(), out ufoMot))
                {
                    double grownValue = ufoMot - thisMot;
                    string grown;
                    string middleLevel = String.Format("среднего уровня общей безработицы по ДФО на <b>{0:P2}</b>", Math.Abs(grownValue)); ;
                    if (grownValue < 0)
                    {
                        grown = "выше";
                    }
                    else if (grownValue > 0)
                    {
                        grown = "ниже";
                    }
                    else
                    {
                        grown = "соответствует";
                        middleLevel = "среднему уровню общей безработицы по ДФО";
                    }

                    description +=
                        String.Format(
                            "{0} {3} {1} (уровень безработицы по ДФО <b>{2:P2}</b>) и ", grown, middleLevel, ufoMot, GetImage(grown));
                }
            }

            if (Double.TryParse(dtText.Rows[2][1].ToString(), out thisMot) &&
               Double.TryParse(dtText.Rows[4][1].ToString(), out ufoMot))
            {
                double grownValue = ufoMot - thisMot;
                string grown;
                string middleLevel = String.Format("среднего уровня общей безработицы по РФ на <b>{0:P2}</b>", Math.Abs(grownValue)); ;
                if (grownValue < 0)
                {
                    grown = "выше";
                }
                else if (grownValue > 0)
                {
                    grown = "ниже";
                }
                else
                {
                    grown = "соответствует";
                    middleLevel = "среднему уровню общей безработицы по РФ";
                }

                description +=
                    String.Format(
                        "{0} {3} {1} (уровень безработицы по РФ <b>{2:P2}</b>)", grown, middleLevel, ufoMot, GetImage(grown));
            }

            return description;
        }

        private static string GetImage(string direction)
        {
            if (direction.ToLower() == "рост")
            {
                return "<img src='../../images/arrowRedUpBB.png'/>";
            }
            else if (direction.ToLower() == "снижение")
            {
                return "<img src='../../images/arrowGreenDownBB.png'/>";
            }
            else if (direction.ToLower() == "выше")
            {
                return "<img src='../../images/ballRedBB.png'/>";
            }
            else
            {
                return "<img src='../../images/ballGreenBB.png'/>";
            }
        }

        private void FillComboRegions()
        {
            DataTable dtRegions = new DataTable();
            string query = DataProvider.GetQueryText("STAT_0001_0012_regions");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Район", dtRegions);

            Dictionary<string, int> regions = new Dictionary<string, int>();
            foreach (DataRow row in dtRegions.Rows)
            {
                regions.Add(row[0].ToString(), 0);
            }
            regions.Add("Дальневосточный федеральный округ", 0);
            ComboRegion.FillDictionaryValues(regions);
        }

        void UltraChart3_ChartDrawItem(object sender, ChartDrawItemEventArgs e)
        {
            //устанавливаем ширину текста легенды 
            /*Text text = e.Primitive as Text;
            if ((text != null) && !(string.IsNullOrEmpty(text.Path)) && text.Path.EndsWith("Legend"))
            {
                int widthLegendLabel;

                if ((UltraChart3.Legend.Location == LegendLocation.Top) || (UltraChart3.Legend.Location == LegendLocation.Bottom))
                {
                    widthLegendLabel = (int)UltraChart3.Width.Value - 20;
                }
                else
                {
                    widthLegendLabel = (UltraChart3.Legend.SpanPercentage * (int)UltraChart3.Width.Value / 100) - 20;
                }

                widthLegendLabel -= UltraChart3.Legend.Margins.Left + UltraChart3.Legend.Margins.Right;
                if (text.labelStyle.Trimming != StringTrimming.None)
                {
                    text.bounds.Width = widthLegendLabel;
                }
            }*/
        }

        void UltraChart3_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Box)
                {
                    Box box = (Box)primitive;
                    if (box.DataPoint != null)
                    {
                        box.DataPoint.Label = String.Format("{0}\n{1}", ComboRegion.SelectedValue, box.DataPoint.Label);
                    }
                }
            }
        }

        void UltraChart4_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Text)
                {
                    Text text = (Text)primitive;
                    if (text.Path.Contains("Border.Title.Grid.X"))
                    {
                        text.SetTextString(ShortName(text.GetTextString().Trim(' ')));

                        text.labelStyle.Font = new Font("Verdana", 8);
                    }
                    if (text.Path.Contains("Border.Title.Grid.Y"))
                    {
                        text.labelStyle.Font = new Font("Verdana", 7);
                    }
                }

                if (primitive is Box)
                {
                    Box box = (Box)primitive;
                    if (box.DataPoint != null)
                    {
                        int row = box.Row;

                        double mot = Convert.ToDouble(dtChart4.Rows[0][row + 1]);
                        double workers = Convert.ToDouble(dtChart4.Rows[1][row + 1]);
                        double all = mot + workers;
                        double percent1 = mot / all;
                        double percent2 = workers / all;

                        if (box.DataPoint.Label.Contains("МОТ"))
                        {
                            box.DataPoint.Label = String.Format("{0}\n{2:N0} человек ({1:P2})", box.DataPoint.Label, percent1, mot);
                        }
                        else
                        {
                            box.DataPoint.Label = String.Format("{0}\n{2:N0} человек ({1:P2})", box.DataPoint.Label, percent2, workers);
                        }
                    }
                }
            }
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            UltraWebGrid1.Columns.Clear();
            UltraWebGrid1.Bands.Clear();
            string query = DataProvider.GetQueryText(String.Format("STAT_0001_0012_grid"));
            dtGrid = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Период", dtGrid);

            string period = dtGrid.Rows[dtGrid.Rows.Count - 1][dtGrid.Columns.Count - 1].ToString();
            DateTime date = CRHelper.PeriodDayFoDate(period);

            int operativeRowNum = 0;

            // Проходим по всем строкам неоперативного грида
            for (int mainRowNum = 0; mainRowNum < dtGrid.Rows.Count; mainRowNum++)
            {
                // если где-то пусто, ищем в оперативной
                if (dtGrid.Rows[mainRowNum][0].ToString()[0] != '2' &&
                    (dtGrid.Rows[mainRowNum]["Уровень зарегистрированной безработицы (от экономически активного населения)"] == DBNull.Value ||
                    dtGrid.Rows[mainRowNum]["Число безработных, зарегистрированных в службе занятости (на конец месяца)"] == DBNull.Value ||
                    dtGrid.Rows[mainRowNum]["Уровень зарегистрированной безработицы РФ "] == DBNull.Value ||
                    (dtOperative.Columns.Count == 6 && dtGrid.Rows[mainRowNum]["Уровень зарегистрированной безработицы ДФО "] == DBNull.Value)))
                {
                    string checkingPeriod =
                        dtGrid.Rows[mainRowNum][dtGrid.Columns.Count - 1].ToString().Replace("[Период].[Год Квартал Месяц]", "[Период].[Период]");
                    try
                    {
                        while (checkingPeriod != dtOperative.Rows[operativeRowNum][1].ToString() ||
                            (operativeRowNum < dtOperative.Rows.Count - 1 &&
                            dtOperative.Rows[operativeRowNum][1].ToString() == dtOperative.Rows[operativeRowNum + 1][1].ToString()))
                        {
                            operativeRowNum++;
                        }
                   
                    if (dtGrid.Rows[mainRowNum]["Уровень зарегистрированной безработицы (от экономически активного населения)"] == DBNull.Value)
                    {
                        dtGrid.Rows[mainRowNum]["Уровень зарегистрированной безработицы (от экономически активного населения)"] =
                            dtOperative.Rows[operativeRowNum][3];
                    }
                    if (dtGrid.Rows[mainRowNum]["Число безработных, зарегистрированных в службе занятости (на конец месяца)"] == DBNull.Value)
                    {
                        dtGrid.Rows[mainRowNum]["Число безработных, зарегистрированных в службе занятости (на конец месяца)"] =
                            dtOperative.Rows[operativeRowNum][2];
                    }
                    if (dtGrid.Rows[mainRowNum]["Уровень зарегистрированной безработицы РФ "] == DBNull.Value)
                    {
                        dtGrid.Rows[mainRowNum]["Уровень зарегистрированной безработицы РФ "] =
                            dtOperative.Rows[operativeRowNum][4];
                    }
                    if (dtOperative.Columns.Count == 6 && dtGrid.Rows[mainRowNum]["Уровень зарегистрированной безработицы ДФО "] == DBNull.Value)
                    {
                        dtGrid.Rows[mainRowNum]["Уровень зарегистрированной безработицы ДФО "] =
                            dtOperative.Rows[operativeRowNum][5];
                    }
                    }
                    catch { }
                }
            }

            for (; operativeRowNum < dtOperative.Rows.Count; operativeRowNum++)
            {
                string periodOperative = dtOperative.Rows[operativeRowNum][1].ToString();
                DateTime dateOperative = CRHelper.PeriodDayFoDate(periodOperative);
                if (dateOperative > date &&
                    (dateOperative.Month > date.Month && dateOperative.Year == date.Year))
                {
                    if (operativeRowNum < dtOperative.Rows.Count - 1)
                    {
                        DateTime dateOperativeNext = CRHelper.PeriodDayFoDate(dtOperative.Rows[operativeRowNum + 1][1].ToString());

                        if (dateOperativeNext.Month > dateOperative.Month)
                        {
                            DataRow row = dtGrid.NewRow();
                            row[0] = CRHelper.ToUpperFirstSymbol(CRHelper.RusMonth(dateOperative.Month));
                            row["Уровень зарегистрированной безработицы (от экономически активного населения)"] = dtOperative.Rows[operativeRowNum][3];
                            row["Число безработных, зарегистрированных в службе занятости (на конец месяца)"] = dtOperative.Rows[operativeRowNum][2];
                            row["Уровень зарегистрированной безработицы РФ "] = dtOperative.Rows[operativeRowNum][4];
                            if (dtOperative.Columns.Count == 6)
                            {
                                row["Уровень зарегистрированной безработицы ДФО "] = dtOperative.Rows[operativeRowNum][5];
                            }
                            dtGrid.Rows.Add(row);
                        }
                    }
                    else
                    {
                        DataRow row = dtGrid.NewRow();
                        row[0] = CRHelper.ToUpperFirstSymbol(CRHelper.RusMonth(dateOperative.Month));
                        row["Уровень зарегистрированной безработицы (от экономически активного населения)"] = dtOperative.Rows[operativeRowNum][3];
                        row["Число безработных, зарегистрированных в службе занятости (на конец месяца)"] = dtOperative.Rows[operativeRowNum][2];
                        row["Уровень зарегистрированной безработицы РФ "] = dtOperative.Rows[operativeRowNum - 1][4];
                        if (dtOperative.Columns.Count == 6)
                        {
                            row["Уровень зарегистрированной безработицы ДФО "] = dtOperative.Rows[operativeRowNum][5];
                        }
                        dtGrid.Rows.Add(row);
                    }
                }
            }

            if (IsSmallResolution)
            {
                if (ComboRegion.SelectedValue == "Дальневосточный федеральный округ")
                {
                    dtGrid.Columns.RemoveAt(3);
                    dtGrid.Columns.RemoveAt(5);
                }
                else
                {
                    dtGrid.Columns.RemoveAt(3);
                    dtGrid.Columns.RemoveAt(3);
                    dtGrid.Columns.RemoveAt(4);
                    dtGrid.Columns.RemoveAt(4);
                }
            }

            dtGrid.Columns.RemoveAt(dtGrid.Columns.Count - 1);
            dtGrid.AcceptChanges();

            ((UltraWebGrid)sender).DataSource = dtGrid;
        }

        protected void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
            UltraWebGrid1.Height = Unit.Empty;
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            //e.Layout.GroupByBox.Hidden = true;
            headerLayout.childCells.Clear();
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.HeaderStyleDefault.VerticalAlign = VerticalAlign.Middle;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.AllowSortingDefault = AllowSorting.No;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }
            e.Layout.Bands[0].Columns[0].Header.Caption = "Период";
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.08);
            headerLayout.AddCell("Период");

            if (IsSmallResolution)
            {
                
                for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
                {
                   /* e.Layout.Bands[0].Columns[i].Header.Caption =
                        e.Layout.Bands[0].Columns[i].Header.Caption.Replace("зарегистри", "зарегистри- ");
                    */
                    e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(120, 1280);
                    string columnFormat = "P2";
                    if (i == 1 || i == 3 || i == 5)
                    {
                        columnFormat = "N0";
                        //e.Layout.Bands[0].Columns[i].Header.Caption += ", человек";
                        headerLayout.AddCell(e.Layout.Bands[0].Columns[i].Header.Caption.Replace("зарегистри", "зарегистри- ") + ", человек");
                    }
                    else
                    {
                        headerLayout.AddCell(e.Layout.Bands[0].Columns[i].Header.Caption.Replace("зарегистри", "зарегистри- ") + ", процент");
                    }
                    CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], columnFormat);
                }

            }
            else
            {
                if (ComboRegion.SelectedValue == "Дальневосточный федеральный округ")
            {
              //  headerLayout.AddCell("Период");
                for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
                {
                    e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(150, 1280);
                    string columnFormat = "P2";
                    if (i == 1 || i == 4 || i == 7)
                    {
                        columnFormat = "N0";
                       // e.Layout.Bands[0].Columns[i].Header.Caption += ", человек";
                        headerLayout.AddCell(e.Layout.Bands[0].Columns[i].Header.Caption + ", человек");
                    }
                    else
                    {
                        headerLayout.AddCell(e.Layout.Bands[0].Columns[i].Header.Caption.TrimEnd(' ')+", процент");
                    }
                    CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], columnFormat);
                }
            }
                else
                {
                    //   headerLayout.AddCell("Период");
                    for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
                    {
                       /* e.Layout.Bands[0].Columns[i].Header.Caption =
                                e.Layout.Bands[0].Columns[i].Header.Caption.Replace("зарегистри", "зарегистри- ");
                        */
                        e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(115, 1280);
                        string columnFormat = "P2";
                        if (i == 1 || i == 5 || i == 9)
                        {
                            columnFormat = "N0";
                           // e.Layout.Bands[0].Columns[i].Header.Caption += ", человек";
                            headerLayout.AddCell(e.Layout.Bands[0].Columns[i].Header.Caption.Replace("зарегистри", "зарегистри- ") + ", человек");
                        }
                        else
                        {
                            headerLayout.AddCell(e.Layout.Bands[0].Columns[i].Header.Caption.Replace("зарегистри", "зарегистри- ").TrimEnd(' ')+", процент");
                        }
                        CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], columnFormat);
                    }
                }
            }
            
           // headerLayout.AddCell("Период");
            headerLayout.ApplyHeaderInfo();
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            int year;
            if (Int32.TryParse(e.Row.Cells[0].Value.ToString(), out year))
            {
                if (year == 2008)
                {
                    e.Row.Hidden = true;
                }
                e.Row.Cells[0].Style.Font.Bold = true;
                e.Row.Cells[0].ColSpan = IsSmallResolution ? 7 : e.Row.Cells.Count;
                for (int i = 1; i < e.Row.Cells.Count; i++)
                {
                    e.Row.Cells[i].Text = "";
                }
            }
            else
            {
                e.Row.Cells[0].Style.Padding.Left = 20;
            }
        }

        #endregion

        #region Обработчики диаграммы

        protected void UltraChart1_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText(String.Format("STAT_0001_0012_chart1"));
            dtChart = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "dummy", dtChart);

            int operativeRowNum = 0;

            // Проходим по всем строкам неоперативного грида
            for (int mainRowNum = 0; mainRowNum < dtChart.Rows.Count; mainRowNum++)
            {
                // если где-то пусто, ищем в оперативной
                try
                {
                    if (dtChart.Rows[mainRowNum]["Уровень зарегистрированной безработицы "] == DBNull.Value ||
                        dtChart.Rows[mainRowNum]["Уровень зарегистрированной безработицы РФ "] == DBNull.Value ||
                        (dtOperative.Columns.Count == 6 && dtChart.Rows[mainRowNum]["Уровень зарегистрированной безработицы ДФО "] == DBNull.Value))
                    {
                        string checkingPeriod =
                            dtChart.Rows[mainRowNum][1].ToString().Replace("[Период].[Год Квартал Месяц]", "[Период].[Период]");
                        while (checkingPeriod != dtOperative.Rows[operativeRowNum][1].ToString() ||
                            (operativeRowNum < dtOperative.Rows.Count - 1 &&
                            dtOperative.Rows[operativeRowNum][1].ToString() == dtOperative.Rows[operativeRowNum + 1][1].ToString()))
                        {
                            operativeRowNum++;
                        }

                        if (dtChart.Rows[mainRowNum]["Уровень зарегистрированной безработицы "] == DBNull.Value)
                        {
                            dtChart.Rows[mainRowNum]["Уровень зарегистрированной безработицы "] =
                                dtOperative.Rows[operativeRowNum][3];
                        }
                        if (dtChart.Rows[mainRowNum]["Уровень зарегистрированной безработицы РФ "] == DBNull.Value)
                        {
                            dtChart.Rows[mainRowNum]["Уровень зарегистрированной безработицы РФ "] =
                                dtOperative.Rows[operativeRowNum][4];
                        }
                        if (dtOperative.Columns.Count == 6 && dtChart.Rows[mainRowNum]["Уровень зарегистрированной безработицы ДФО "] == DBNull.Value)
                        {
                            dtChart.Rows[mainRowNum]["Уровень зарегистрированной безработицы ДФО "] =
                                dtOperative.Rows[operativeRowNum][5];
                        }
                    }
                }
                catch { }
            }

            string lastPeriod = dtChart.Rows[dtChart.Rows.Count - 1][1].ToString();
            DateTime lastDate = CRHelper.PeriodDayFoDate(lastPeriod);

            for (; operativeRowNum < dtOperative.Rows.Count; operativeRowNum++)
            {
                string periodOperative = dtOperative.Rows[operativeRowNum][1].ToString();
                DateTime dateOperative = CRHelper.PeriodDayFoDate(periodOperative);
                if (dateOperative > lastDate &&
                    (dateOperative.Month > lastDate.Month && dateOperative.Year == lastDate.Year))
                {
                    if (operativeRowNum < dtOperative.Rows.Count - 1)
                    {
                        DateTime dateOperativeNext = CRHelper.PeriodDayFoDate(dtOperative.Rows[operativeRowNum + 1][1].ToString());

                        if (dateOperativeNext.Month > dateOperative.Month)
                        {
                            DataRow row = dtChart.NewRow();
                            row[1] = periodOperative;
                            row["Уровень зарегистрированной безработицы "] = dtOperative.Rows[operativeRowNum][3];
                            row["Уровень зарегистрированной безработицы РФ "] = dtOperative.Rows[operativeRowNum][4];
                            if (dtOperative.Columns.Count == 6)
                            {
                                row["Уровень зарегистрированной безработицы ДФО "] = dtOperative.Rows[operativeRowNum][5];
                            }
                            dtChart.Rows.Add(row);
                        }
                    }
                    else
                    {
                        DataRow row = dtChart.NewRow();
                        row[1] = periodOperative;
                        row["Уровень зарегистрированной безработицы "] = dtOperative.Rows[operativeRowNum][3];
                        if (dtOperative.Rows[operativeRowNum][4] != DBNull.Value)
                        {
                            row["Уровень зарегистрированной безработицы РФ "] = dtOperative.Rows[operativeRowNum][4];
                        }
                        else
                        {
                            row["Уровень зарегистрированной безработицы РФ "] = dtOperative.Rows[operativeRowNum - 1][4];
                        }
                        if (dtOperative.Columns.Count == 6)
                        {
                            row["Уровень зарегистрированной безработицы ДФО "] = dtOperative.Rows[operativeRowNum][5];
                        }
                        dtChart.Rows.Add(row);
                    }
                }
            }

            dtChart.Columns["Уровень зарегистрированной безработицы "].ColumnName =
                    String.Format("{0} {1}", "Уровень зарегистрированной безработицы ",ShortName(ComboRegion.SelectedValue));
            dtChart.Columns["Уровень общей безработицы по методологии МОТ "].ColumnName =
                String.Format("{0} {1}", "Уровень общей безработицы по методологии МОТ ", ShortName(ComboRegion.SelectedValue));

            dtChart.AcceptChanges();
            UltraChart1.DataSource = ConvertPeriodNames(dtChart);
        }

        protected void UltraChart2_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText(String.Format("STAT_0001_0012_chart2"));
            dtChart = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "dummy", dtChart);

            string period = dtChart.Rows[dtChart.Rows.Count - 1][1].ToString();
            DateTime date = CRHelper.PeriodDayFoDate(period);
            int operativeRowNum = 0;

            // Проходим по всем строкам неоперативного грида
            for (int mainRowNum = 0; mainRowNum < dtChart.Rows.Count; mainRowNum++)
            {
                // если где-то пусто, ищем в оперативной
                try
                {
                    if (dtChart.Rows[mainRowNum]["Число безработных, зарегистрированных в службе занятости "] == DBNull.Value)
                    {
                        string checkingPeriod =
                            dtChart.Rows[mainRowNum][1].ToString().Replace("[Период].[Год Квартал Месяц]", "[Период].[Период]");
                        while (checkingPeriod != dtOperative.Rows[operativeRowNum][1].ToString() ||
                            (operativeRowNum < dtOperative.Rows.Count - 1 &&
                            dtOperative.Rows[operativeRowNum][1].ToString() == dtOperative.Rows[operativeRowNum + 1][1].ToString()))
                        {
                            operativeRowNum++;
                        }

                        dtChart.Rows[mainRowNum]["Число безработных, зарегистрированных в службе занятости "] =
                            dtOperative.Rows[operativeRowNum][2];

                    }
                }
                catch { }
            }


            for (; operativeRowNum < dtOperative.Rows.Count; operativeRowNum++)
            {
                string periodOperative = dtOperative.Rows[operativeRowNum][1].ToString();
                DateTime dateOperative = CRHelper.PeriodDayFoDate(periodOperative);
                if (dateOperative > date &&
                    (dateOperative.Month > date.Month && dateOperative.Year == date.Year))
                {
                    if (operativeRowNum < dtOperative.Rows.Count - 1)
                    {
                        DateTime dateOperativeNext = CRHelper.PeriodDayFoDate(dtOperative.Rows[operativeRowNum + 1][1].ToString());

                        if (dateOperativeNext.Month > dateOperative.Month)
                        {
                            DataRow row = dtChart.NewRow();
                            row[1] = periodOperative;
                            row["Число безработных, зарегистрированных в службе занятости "] = dtOperative.Rows[operativeRowNum][2];
                            dtChart.Rows.Add(row);
                        }
                    }
                    else
                    {
                        DataRow row = dtChart.NewRow();
                        row[1] = periodOperative;
                        row["Число безработных, зарегистрированных в службе занятости "] = dtOperative.Rows[operativeRowNum][2];
                        dtChart.Rows.Add(row);
                    }
                }
            }
            dtChart.Columns["Число безработных, зарегистрированных в службе занятости "].ColumnName =
                String.Format("{0} {1}", "Число безработных, зарегистрированных в службе занятости ", ShortName(ComboRegion.SelectedValue));
            dtChart.Columns["Общая численность безработных по методологии МОТ "].ColumnName =
               String.Format("{0} {1}", "Общая численность безработных по методологии МОТ ", ShortName(ComboRegion.SelectedValue));
            dtChart.AcceptChanges();
            UltraChart2.DataSource = ConvertPeriodNames(dtChart);
        } 

        protected void UltraChart3_DataBinding(object sender, EventArgs e)
        {
            dtChart = new DataTable();
            string query = DataProvider.GetQueryText(String.Format("STAT_0001_0012_chart3"));
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);

            UltraChart3.DataSource = dtChart;
        }

        protected void UltraChart4_DataBinding(object sender, EventArgs e)
        {
            dtChart4 = new DataTable();
            string query = DataProvider.GetQueryText(String.Format("STAT_0001_0012_compare"));
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart4);

            UltraChart4.Series.Clear();
            for (int i = 1; i < 9; i++)
            {
                UltraChart4.Series.Add(CRHelper.GetNumericSeries(i, dtChart4));
            }
        }

        private static DataTable ConvertPeriodNames(DataTable dt)
        {
            foreach (DataRow row in dt.Rows)
            {
                string period = row[1].ToString();
                DateTime date = CRHelper.PeriodDayFoDate(period);
                row[1] = String.Format("{0} {1}г.", CRHelper.RusMonth(date.Month), date.Year);
            }
            dt.Columns.RemoveAt(0);
            dt.AcceptChanges();
            return dt;
        }

        #endregion
/*
        #region Экспорт в Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = PageTitle.Text;
            e.CurrentWorksheet.Rows[1].Cells[0].Value = PageSubTitle.Text;
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            int columnCount = UltraWebGrid1.Columns.Count;
            int rowsCount = UltraWebGrid1.Rows.Count;

            if (IsSmallResolution)
            {
                for (int i = 1; i < columnCount; i++)
                {
                    string columnFormat = "0.00%";
                    if (i == 1 || i == 3 || i == 5)
                    {
                        columnFormat = "#,##0";
                    }
                    e.CurrentWorksheet.Columns[i].Width = 120 * 37;
                    e.CurrentWorksheet.Columns[i].CellFormat.FormatString = columnFormat;
                }

            }
            else if (ComboRegion.SelectedValue == "Дальневосточный федеральный округ")
            {
                for (int i = 1; i < columnCount; i++)
                {
                    string columnFormat = "0.00%";
                    if (i == 1 || i == 4 || i == 7)
                    {
                        columnFormat = "#,##0";
                    }
                    e.CurrentWorksheet.Columns[i].Width = 120 * 37;
                    e.CurrentWorksheet.Columns[i].CellFormat.FormatString = columnFormat;
                }
            }
            else
            {
                for (int i = 1; i < columnCount; i++)
                {
                    string columnFormat = "0.00%";
                    if (i == 1 || i == 5 || i == 9)
                    {
                        columnFormat = "#,##0";
                    }
                    e.CurrentWorksheet.Columns[i].Width = 120 * 37;
                    e.CurrentWorksheet.Columns[i].CellFormat.FormatString = columnFormat;
                }
            }

            // расставляем стили у начальных колонок
            for (int i = 4; i < rowsCount + 4; i++)
            {
                e.CurrentWorksheet.Rows[i].Cells[0].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                e.CurrentWorksheet.Rows[i].Cells[0].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
            }
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;

            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid1);
        }

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {
            UltraGridColumn col = UltraWebGrid1.DisplayLayout.Bands[0].Columns[e.CurrentColumnIndex];
            e.HeaderText = col.Header.Key.Split(';')[0];
        }

        #endregion

        #region Экспорт в PDF


        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";

            UltraGridExporter1.PdfExporter.Export(UltraWebGrid1);
            
        }

        private void PdfExporter_BeginExport(object sender, DocumentExportEventArgs e)
        {
            IText title = e.Section.AddText();
            Font font = new Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(PageTitle.Text);

            title = e.Section.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(PageSubTitle.Text);

            title = e.Section.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(String.Format("\n{0}", chart3ElementCaption.Text));
            ITable table = e.Section.AddTable();
            ITableRow row = table.AddRow();
            ITableCell cell = row.AddCell();

            cell.Width = new FixedWidth((float)UltraChart3.Width.Value + 20);
            cell.AddImage(UltraGridExporter.GetImageFromChart(UltraChart3));

            cell = row.AddCell();
            cell.AddImage(UltraGridExporter.GetImageFromChart(UltraChart4));

            title = e.Section.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);

            title.AddContent(CommentTextExportsReplaces(lbDescription.Text));
        }
        
        

        void PdfExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs e)
        {
            IText title = e.Section.AddText();
            Font font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(chart1ElementCaption.Text);

            UltraChart1.Legend.Margins.Right = 0;
            UltraChart1.Width = Unit.Pixel(CustomReportConst.minScreenWidth);
            Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromChart(UltraChart1);
            e.Section.AddImage(img);

            title = e.Section.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(chart2ElementCaption.Text);

            UltraChart2.Legend.Margins.Right = 0;
            UltraChart2.Width = Unit.Pixel(CustomReportConst.minScreenWidth);
            img = UltraGridExporter.GetImageFromChart(UltraChart2);
            e.Section.AddImage(img);
        }

        #endregion*/
        private static string CommentTextExportsReplaces(string source)
        {
            string commentText = source;

            commentText = commentText.Replace("<\n>", "");
            commentText = commentText.Replace("<\r>", "");
            commentText = commentText.Replace(@"
", " ");
            commentText = commentText.Replace("&nbsp;", " ");
            commentText = commentText.Replace("<br/>", "\n");
            commentText = commentText.Replace("<b>", "");
            commentText = commentText.Replace("</b>", "");
            commentText = Regex.Replace(commentText, "<img src=[\\s\\S]*?>", "");

            return commentText;
        }
        private static string GetDictionaryUniqueKey(Dictionary<string, string> dictionary, string key)
        {
            string newKey = key;
            while (dictionary.ContainsKey(newKey))
            {
                newKey += " ";
            }
            return newKey;
        }

        private static Dictionary<string, string> comboToPeriodDictionary = new Dictionary<string, string>();
        private static Dictionary<string, int> periodDictionary = new Dictionary<string, int>();

        private static void FillPeriodCombo()
        {
            if (periodDictionary.Count > 0)
                return;

            DataTable dtPeriod = new DataTable();
            string query = DataProvider.GetQueryText("STAT_0001_0012_period");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "dummy", dtPeriod);

            foreach (DataRow row in dtPeriod.Rows)
            {
                int year;
                string element;
                if (Int32.TryParse(row[0].ToString(), out year))
                {
                    element = String.Format("{0} год", year);
                    periodDictionary.Add(element, 0);
                    comboToPeriodDictionary.Add(element, row[1].ToString());
                }
                else
                {
                    element = GetDictionaryUniqueKey(comboToPeriodDictionary, row[0].ToString());
                    periodDictionary.Add(element, 1);
                    comboToPeriodDictionary.Add(element, row[1].ToString());
                }
            }
        }

        #region Экспорт в Excel
        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;
            Infragistics.Documents.Excel.Workbook workbook = new Infragistics.Documents.Excel.Workbook();
            Infragistics.Documents.Excel.Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            Infragistics.Documents.Excel.Worksheet sheet2 = workbook.Worksheets.Add("Диаграмма1");
            Infragistics.Documents.Excel.Worksheet sheet3 = workbook.Worksheets.Add("Диаграмма2");
            Infragistics.Documents.Excel.Worksheet sheet4 = workbook.Worksheets.Add("Диаграмма3");
            Infragistics.Documents.Excel.Worksheet sheet5 = workbook.Worksheets.Add("Диаграмма4");
            ReportExcelExporter1.HeaderCellFont = new System.Drawing.Font("Verdana", 11);
            ReportExcelExporter1.TitleFont = new System.Drawing.Font("Verdana", 11, FontStyle.Bold);
            ReportExcelExporter1.SubTitleFont = new System.Drawing.Font("Verdana", 9);
            ReportExcelExporter1.TitleAlignment = HorizontalCellAlignment.Left;

            ReportExcelExporter1.TitleStartRow = 0;

            ReportExcelExporter1.Export(headerLayout, sheet1, 3);
            ReportExcelExporter1.Export(UltraChart3,chart3ElementCaption.Text,sheet2, 3);
            ReportExcelExporter1.Export(UltraChart4, chart3ElementCaption.Text, sheet3, 3);
            ReportExcelExporter1.Export(UltraChart1, chart1ElementCaption.Text,sheet4, 3);
            ReportExcelExporter1.Export(UltraChart2, chart2ElementCaption.Text, sheet5,3);
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
        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            e.Workbook.Worksheets["Диаграмма3"].MergedCellsRegions.Clear();
            e.Workbook.Worksheets["Диаграмма3"].Rows[2].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
            e.Workbook.Worksheets["Диаграмма2"].MergedCellsRegions.Clear();
            e.Workbook.Worksheets["Диаграмма2"].Rows[2].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
            e.Workbook.Worksheets["Диаграмма1"].MergedCellsRegions.Clear();
            e.Workbook.Worksheets["Диаграмма1"].Rows[2].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
            e.Workbook.Worksheets["Диаграмма4"].MergedCellsRegions.Clear();
            e.Workbook.Worksheets["Диаграмма4"].Rows[2].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;

        }
        #endregion

        #region Экспорт в PDF


        private void PdfExportButton_Click(object sender, EventArgs e)
        {
           // ReportPDFExporter1.PageTitle = PageTitle .Text;
            //ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;
            ReportPDFExporter1.HeaderCellHeight = 80;
            Infragistics.Documents.Reports.Report.Report report = new Infragistics.Documents.Reports.Report.Report();

            Infragistics.Documents.Reports.Report.Section.ISection section1 = report.AddSection();
            Infragistics.Documents.Reports.Report.Section.ISection section2 = report.AddSection();
            Infragistics.Documents.Reports.Report.Section.ISection section3 = report.AddSection();

           
            
         //   section2.PageSize = new PageSize(MinScreenWidth, 700);
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
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(String.Format("\n{0}", chart3ElementCaption.Text));
            ITable table = section1.AddTable();
            ITableRow row = table.AddRow();
            ITableCell cell = row.AddCell();

            cell.Width = new FixedWidth((float)UltraChart3.Width.Value + 20);
            cell.AddImage(UltraGridExporter.GetImageFromChart(UltraChart3));

            cell = row.AddCell();
            cell.AddImage(UltraGridExporter.GetImageFromChart(UltraChart4));
            
            title = section1.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);

            title.AddContent(CommentTextExportsReplaces(lbDescription.Text));

            ReportPDFExporter1.Export(headerLayout, section1);
            UltraChart1.Width = 1050;
            UltraChart2.Width = 1050;
            ReportPDFExporter1.Export(UltraChart1, chart1ElementCaption.Text, section2);
            ReportPDFExporter1.Export(UltraChart2, chart2ElementCaption.Text, section3);
        }
        #endregion
    }
}