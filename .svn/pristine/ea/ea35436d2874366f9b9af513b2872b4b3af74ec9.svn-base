using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Core.Primitives;
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
using Color = System.Drawing.Color;
using EndExportEventArgs = Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs;
using Infragistics.Documents.Reports.Report.Text;

namespace Krista.FM.Server.Dashboards.reports.STAT_0001_0008
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtGrid;
        private DataTable dtChart;
        private DataTable dtDate;
        private string gridFirstColumnCaption;
        private string queryPostfix;

        #endregion

        private int MinScreenHeight
        {
            get { return CustomReportConst.minScreenHeight; }
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid1.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 12);
            UltraWebGrid1.Height = Unit.Empty;
            UltraWebGrid1.DisplayLayout.NoDataMessage = "Нет данных";
            //UltraWebGrid1.DataBound += new EventHandler(UltraWebGrid_DataBound);

            UltraChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth / 2 - 30);
            UltraChart1.Height = CRHelper.GetChartHeight(300);

            UltraChart2.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth / 2 - 30);
            UltraChart2.Height = CRHelper.GetChartHeight(300);

            #region Настройка диаграммы

            UltraChart1.ChartType = ChartType.PieChart;
            UltraChart1.Border.Thickness = 0;

            UltraChart1.Axis.Y.Extent = 130;
            UltraChart1.Axis.X.Extent = 140;
            UltraChart1.PieChart.OthersCategoryPercent = 0;
            UltraChart1.PieChart.OthersCategoryText = "Прочие";

            UltraChart1.Tooltips.FormatString = "<ITEM_LABEL>\n<DATA_VALUE:N3> млн.руб.";

            UltraChart1.Legend.Visible = true;
            UltraChart1.Legend.Location = LegendLocation.Bottom;
            UltraChart1.Legend.SpanPercentage = 57;

            UltraChart1.Legend.Margins.Bottom = 10;
            UltraChart1.Legend.Margins.Top = 6;

            //UltraChart1.Legend.MoreIndicatorText = "";
            // UltraChart1.Legend.Font = new Font("Verdana", 10);

            UltraChart1.Data.SwapRowsAndColumns = false;

            CRHelper.FillCustomColorModel( UltraChart1, 17, true);

            UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart1.ChartDrawItem += new ChartDrawItemEventHandler(UltraChart1_ChartDrawItem);

            UltraChart2.ChartType = ChartType.StackColumnChart;
            UltraChart2.Border.Thickness = 0;

            UltraChart2.Axis.Y.Extent = 80;
            UltraChart2.Axis.Y.Labels.SeriesLabels.Orientation = TextOrientation.Horizontal;
            UltraChart2.Axis.Y.Labels.SeriesLabels.WrapText = true;
            UltraChart2.Axis.Y.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart2.Axis.Y.Labels.SeriesLabels.FontColor = Color.Black;
            //  UltraChart2.Axis.Y.Labels.SeriesLabels.Layout.Behavior = AxisLabelLayoutBehaviors.None;
            UltraChart2.Axis.Y.Labels.SeriesLabels.Visible = true;
            UltraChart2.Axis.X.Labels.SeriesLabels.Layout.Behavior = AxisLabelLayoutBehaviors.None;
            UltraChart2.Axis.X.Labels.SeriesLabels.Visible = true;
            UltraChart2.Axis.X.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.None;

            UltraChart2.Axis.Y.Labels.WrapText = false;
            UltraChart2.Axis.Y.Labels.SeriesLabels.WrapText = false;

            UltraChart2.Axis.X.Extent = 40;
            UltraChart2.Axis.X.TickmarkStyle = AxisTickStyle.Percentage;
            UltraChart2.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            UltraChart2.Axis.X.Labels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart2.Axis.X.Labels.FontColor = Color.Black;
            UltraChart2.TitleLeft.Visible = true;
            UltraChart2.TitleLeft.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart2.TitleLeft.Text = "млн.руб.";
            UltraChart2.TitleLeft.HorizontalAlign = StringAlignment.Far;
            UltraChart2.Axis.X.Labels.WrapText = false;
            UltraChart2.Axis.X.Labels.SeriesLabels.WrapText = false;

            UltraChart2.Tooltips.FormatString = "<ITEM_LABEL>\n<DATA_VALUE:N3> млн.руб.";

            UltraChart2.Legend.Visible = true;
            UltraChart2.Legend.Location = LegendLocation.Bottom;
            UltraChart2.Legend.SpanPercentage = 57;
            //  UltraChart2.Legend.Font = new Font("Verdana", 10);

            UltraChart2.Data.SwapRowsAndColumns = false;
            UltraChart3.Height = 190;

            CRHelper.CopyCustomColorModel(UltraChart1, UltraChart2);
            UltraChart2.ColorModel.Skin.ApplyRowWise = false;

            if (ComboKinds.SelectedIndex == 1)
            {
                UltraChart3.Height = 130;
            }
            else if (ComboKinds.SelectedIndex == 2)
            {
                UltraChart3.Height = 50;
            }
            else if (ComboKinds.SelectedIndex == 3)
            {
                UltraChart3.Height = 50;
            }
            
            UltraChart2.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

            UltraChart1.Legend.Visible = false;
            UltraChart2.Legend.Visible = false;

            UltraChart3.Width = Unit.Parse((UltraChart1.Width.Value + UltraChart2.Width.Value + 60).ToString());
            UltraChart3.Legend.SpanPercentage = 100;
            //UltraChart3.ChartDrawItem += new ChartDrawItemEventHandler(UltraChart1_ChartDrawItem);

            CRHelper.CopyCustomColorModel(UltraChart1, UltraChart3);
            UltraChart3.ColorModel.Skin.ApplyRowWise = true;
            UltraChart3.Legend.Visible = true;
            UltraChart3.Legend.Location = LegendLocation.Bottom;
            //UltraChart3.Axis.X.Extent = 500;
            UltraChart3.Axis.X.LineThickness = 0;
            UltraChart3.Axis.Y.LineThickness = 0;
            UltraChart3.Axis.Y.MajorGridLines.Thickness = 0;
            UltraChart3.Axis.Y.MinorGridLines.Thickness = 0;
            UltraChart3.Axis.X.MajorGridLines.Thickness = 0;
            UltraChart3.Axis.X.MinorGridLines.Thickness = 0;
            UltraChart3.Axis.X.Labels.Visible = false;
            UltraChart3.Border.Thickness = 0;

            #endregion

            UltraGridExporter1.Visible = true;
            UltraGridExporter1.MultiHeader = true;
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);

            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler
                    <DocumentExportEventArgs>(PdfExporter_BeginExport);
            UltraGridExporter1.PdfExporter.EndExport += new EventHandler<EndExportEventArgs>(PdfExporter_EndExport);
        }

        void UltraChart1_ChartDrawItem(object sender, ChartDrawItemEventArgs e)
        {
            //устанавливаем ширину текста легенды 
            Text text = e.Primitive as Text;
            if ((text != null) && !(string.IsNullOrEmpty(text.Path)) && text.Path.EndsWith("Legend"))
            {
                int widthLegendLabel;

                if ((UltraChart1.Legend.Location == LegendLocation.Top) || (UltraChart1.Legend.Location == LegendLocation.Bottom))
                {
                    widthLegendLabel = (int)UltraChart1.Width.Value - 20;
                }
                else
                {
                    widthLegendLabel = ((int)UltraChart1.Legend.SpanPercentage * (int)UltraChart1.Width.Value / 100) - 20;
                }

                widthLegendLabel -= UltraChart1.Legend.Margins.Left + UltraChart1.Legend.Margins.Right;
                if (text.labelStyle.Trimming != StringTrimming.None)
                {
                    text.bounds.Width = widthLegendLabel;
                }
            }
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                FillComboRegions();
                ComboRegion.Title = "Территория";
                ComboRegion.Width = 400;
                ComboRegion.SetСheckedState("Уральский федеральный округ", true);
                ComboRegion.ParentSelect = true;

                ComboKinds.Width = 300;
                ComboKinds.MultiSelect = false;
                ComboKinds.ParentSelect = true;
                ComboKinds.FillDictionaryValues(GetKindsDictionary());
                ComboKinds.Title = "Разрезность";
                ComboKinds.SetСheckedState("По видам деятельности", true);
            }

            Page.Title = "Структура инвестиций в основной капитал";
            PageTitle.Text = Page.Title;

            UserParams.SubjectFO.Value = ComboRegion.SelectedValue == "Уральский федеральный округ"
                                             ? String.Empty
                                             : String.Format(".[{0}]", ComboRegion.SelectedValue);

            queryPostfix = QueryPostfix;

            dtDate = new DataTable();
            string queryName = "STAT_0001_0008_date";
            string query;
            if (ComboKinds.SelectedIndex != 1)
            {
                queryName = String.Format("{0}_{1}", queryName, queryPostfix);
                query = DataProvider.GetQueryText(queryName);
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                UserParams.PeriodYear.Value = dtDate.Rows[0][0].ToString();
                UserParams.PeriodQuater.Value = dtDate.Rows[0][3].ToString();
            }
            
            SetSlicerParams();

            UltraWebGrid1.Bands.Clear();
            UltraWebGrid1.DataBind();

            query = DataProvider.GetQueryText(String.Format("STAT_0001_0008_chart1_{0}", queryPostfix));
            dtChart = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Дата", dtChart);

            for (int i = 0; i < dtChart.Rows.Count; i++)
            {
                dtChart.Rows[i][0] = dtChart.Rows[i][0].ToString().Replace(" ДАННЫЕ", String.Empty);
                dtChart.Rows[i][0] = dtChart.Rows[i][0].ToString().Replace(")", String.Empty);
                dtChart.Rows[i][0] = dtChart.Rows[i][0].ToString().Replace("(", String.Empty);
            }

            UltraChart1.DataSource = dtChart;

            for (int i = 0; i < dtChart.Rows.Count; i++)
            {
                dtChart.Rows[i][0] = dtChart.Rows[i][0].ToString().Replace(
                    "Оптовая и розничная торговля; ремонт автотранспортных средств, мотоциклов, бытовых изделий и предметов личного пользования",
                    "Оптовая и розничная торговля; ремонт автотранспортных средств, бытовых изделий");
                dtChart.Rows[i][0] = dtChart.Rows[i][0].ToString().Replace(
                    "Собственность общественных и религиозных организаций объединений",
                    "Собственность общественных и религиозных организаций");
            }
            dtChart.AcceptChanges();
            UltraChart3.DataSource = dtChart;

            UltraChart1.DataBind();
            UltraChart2.DataBind();
            UltraChart3.DataBind();
        }

        private void FillComboRegions()
        {
            DataTable dtRegions = new DataTable();
            string query = DataProvider.GetQueryText("STAT_0001_0008_regions");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Район", dtRegions);

            Dictionary<string, int> regions = new Dictionary<string, int>();
            //regions.Add(dtRegions.Rows[0][1].ToString(), 0);
            foreach (DataRow row in dtRegions.Rows)
            {
                regions.Add(row[0].ToString(), 0);
            }
            regions.Add("Уральский федеральный округ", 0);
            ComboRegion.FillDictionaryValues(regions);
        }

        private Dictionary<string, int> GetKindsDictionary()
        {
            Dictionary<string, int> kinds = new Dictionary<string, int>();
            kinds.Add("По видам деятельности", 0);
            kinds.Add("По формам собственности", 0);
            kinds.Add("По источникам финансирования", 0);
            kinds.Add("По видам основных фондов", 0);
            return kinds;
        }

        private string QueryPostfix
        {
            get
            {
                switch (ComboKinds.SelectedIndex)
                {
                    case (0):
                        {
                            return "activityKinds";
                        }
                    case (1):
                        {
                            return "privacyKinds";
                        }
                    case (2):
                        {
                            return "finSources";
                        }
                    case (3):
                        {
                            return "fonds";
                        }
                    default:
                        {
                            return "activityKinds";
                        }
                }
            }
        }

        private void SetSlicerParams()
        {

            switch (ComboKinds.SelectedIndex)
            {
                case (0):
                    {
                        string period =
                            string.Format("за&nbsp;{0}&nbsp;квартала {1} года",
                                          dtDate.Rows[0][2].ToString().Replace("Квартал ", String.Empty),
                                          dtDate.Rows[0][0]);
                        chart1ElementCaption.Text =
                            String.Format(
                                "Структура инвестиций в основной капитал по видам деятельности {0}", period);
                        chart2ElementCaption.Text = "Динамика и структура инвестиций в основной капитал по видам деятельности";
                        gridCaptionElement.Text = "Динамика и структура инвестиций в основной капитал по видам деятельности, рубли";
                        gridFirstColumnCaption = "Раздел ОКВЭД";
                        PageSubTitle.Text = string.Format("{0}, {1}", ComboRegion.SelectedValue, period);
                        break;
                    }
                case (1):
                    {
#warning из-за кривой базы используем магию
                        string query = DataProvider.GetQueryText("STAT_0001_0008_date_privacyKinds");
                        DataTable dt = new DataTable();
                        DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dt);

                        string period =
                            string.Format("за&nbsp;{0}&nbsp;год", dt.Rows[0][0]).Replace("2009", "2008");

                        chart1ElementCaption.Text =
                                String.Format("Структура инвестиций в основной капитал по формам собственности  {0}", period);

                        chart2ElementCaption.Text = "Динамика и структура инвестиций в основной капитал по формам собственности";
                        gridCaptionElement.Text = "Динамика и структура инвестиций в основной капитал по формам собственности, рубли";
                        gridFirstColumnCaption = "Форма собственности";
                        PageSubTitle.Text = string.Format("{0}, {1}", ComboRegion.SelectedValue, period);
                        break;
                    }
                case (2):
                    {
                        string period =
                            string.Format("за&nbsp;{0}&nbsp;квартала {1} года",
                                          dtDate.Rows[0][2].ToString().Replace("Квартал ", String.Empty),
                                          dtDate.Rows[0][0]);
                        chart1ElementCaption.Text =
                            String.Format(
                                "Структура инвестиций в основной капитал по источникам финансирования {0}", period);
                        chart2ElementCaption.Text = "Динамика и структура инвестиций в основной капитал по источникам финансирования";
                        gridCaptionElement.Text = "Динамика и структура инвестиций в основной капитал по источникам финансирования, рубли";
                        gridFirstColumnCaption = "Источник финансирования";
                        PageSubTitle.Text = string.Format("{0}, {1}", ComboRegion.SelectedValue, period);
                        break;
                    }
                case (3):
                    {
                        string period =
                            string.Format("за&nbsp;{0}&nbsp;квартала {1} года",
                                          dtDate.Rows[0][2].ToString().Replace("Квартал ", String.Empty),
                                          dtDate.Rows[0][0]);
                        chart1ElementCaption.Text =
                            String.Format(
                                "Структура инвестиций в основной капитал по видам основных фондов {0}", period);
                        chart2ElementCaption.Text = "Динамика и структура инвестиций в основной капитал по видам основных фондов";
                        gridCaptionElement.Text = "Динамика и структура инвестиций в основной капитал по видам основных фондов, рубли";
                        gridFirstColumnCaption = "Вид фонда";
                        PageSubTitle.Text = string.Format("{0}, {1}", ComboRegion.SelectedValue, period);
                        break;
                    }
                default:
                    {
                        string period =
                            string.Format("за&nbsp;{0}&nbsp;квартала {1} года",
                                          dtDate.Rows[0][2].ToString().Replace("Квартал ", String.Empty),
                                          dtDate.Rows[0][0]);
                        chart1ElementCaption.Text =
                            String.Format(
                                "Структура инвестиций в основной капитал по видам деятельности за&nbsp;{0}&nbsp;кварталa {1} года",
                                dtDate.Rows[0][2].ToString().Replace("Квартал ", String.Empty), dtDate.Rows[0][0]);
                        chart2ElementCaption.Text = "Динамика и структура инвестиций в основной капитал по видам деятельности";
                        gridCaptionElement.Text = "Динамика и структура инвестиций в основной капитал по видам деятельности, рубли";
                        gridFirstColumnCaption = "Раздел ОКВЭД";
                        PageSubTitle.Text = string.Format("{0}, {1}", ComboRegion.SelectedValue, period);
                        break;
                    }
            }

        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText(String.Format("STAT_0001_0008_grid_{0}", queryPostfix));
            dtGrid = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, gridFirstColumnCaption, dtGrid);
            ((UltraWebGrid)sender).DataSource = dtGrid;
        }

        protected void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
            UltraWebGrid1.Height = Unit.Empty;
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowSortingDefault = AllowSorting.No;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(150, 1280);
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            int year = 2007;
            int span = -1;
            int quarterNum = 1;
            Dictionary<int, int> spans = new Dictionary<int, int>();


            if (ComboKinds.SelectedIndex != 1)
            {
                for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
                {
                    if (i > 2 && (e.Layout.Bands[0].Columns[i].Header.Caption.Contains("Данные всех периодов") ||
                        Convert.ToInt32(e.Layout.Bands[0].Columns[i].Header.Caption.Split(';')[0].Split(' ')[1]) < quarterNum))
                    {
                        spans.Add(year, span);
                        year++;
                        span = 0;
                    }
                    if (i > 0 && !(e.Layout.Bands[0].Columns[i].Header.Caption.Contains("Данные всех периодов")))
                    {
                        quarterNum =
                            Convert.ToInt32(e.Layout.Bands[0].Columns[i].Header.Caption.Split(';')[0].Split(' ')[1]);
                    }
                    span++;
                    if (i == 0)
                    {
                        e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 0;
                        e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.SpanY = 3;
                    }
                    else
                    {
                        e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 2;
                    }
                }
            }
            else
            {
                for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
                {
                    if (i == 0)
                    {
                        e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 0;
                        e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.SpanY = 2;
                    }
                    else
                    {
                        e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 1;
                    }
                }
            }
            spans.Add(year, span);
            int multiHeaderPos = 1;

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count - 1; i = i + 2)
            {
                ColumnHeader ch = new ColumnHeader(true);
                ch.Caption = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';')[0];
                if (ComboKinds.SelectedIndex == 1)
                {
                    ch.Caption = ch.Caption.Replace(" ДАННЫЕ", String.Empty);
                    ch.Caption = ch.Caption.Replace(")", String.Empty);
                    ch.Caption = ch.Caption.Replace("(", String.Empty);
                }

                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i, "Нарастающий итог, млн.руб.", "");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 1, "Темп роста к АППГ", "");

                ch.RowLayoutColumnInfo.OriginY = ComboKinds.SelectedIndex != 1 ? 1 : 0;
                ch.RowLayoutColumnInfo.OriginX = multiHeaderPos;
                ch.RowLayoutColumnInfo.SpanY = 1;
                multiHeaderPos += 2;
                ch.RowLayoutColumnInfo.SpanX = 2;
                e.Layout.Bands[0].HeaderLayout.Add(ch);

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N3");
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i + 1], "P2");

                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(100, 1280);
                e.Layout.Bands[0].Columns[i + 1].Width = CRHelper.GetColumnWidth(80, 1280);
            }

            if (ComboKinds.SelectedIndex != 1)
            {
                multiHeaderPos = 1;

                for (int i = 2007; i < 2011; i = i + 1)
                {
                    if (spans.ContainsKey(i))
                    {
                        ColumnHeader ch = new ColumnHeader(true);

                        ch.Caption = i.ToString();

                        ch.RowLayoutColumnInfo.OriginY = 0;
                        ch.RowLayoutColumnInfo.OriginX = multiHeaderPos;
                        ch.RowLayoutColumnInfo.SpanY = 1;
                        multiHeaderPos += spans[i];
                        ch.RowLayoutColumnInfo.SpanX = spans[i];

                        e.Layout.Bands[0].HeaderLayout.Add(ch);
                    }
                }
            }

            if (ComboKinds.SelectedIndex == 2)
            {
                e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1].Hidden = true;
            }
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            if (ComboKinds.SelectedIndex == 2)
            {
                e.Row.Cells[0].Value = e.Row.Cells[0].Value.ToString().Replace(" ДАННЫЕ", String.Empty);
                e.Row.Cells[0].Value = e.Row.Cells[0].Value.ToString().Replace(")", String.Empty);
                e.Row.Cells[0].Value = e.Row.Cells[0].Value.ToString().Replace("(", String.Empty);
                switch (e.Row.Cells[e.Row.Cells.Count - 1].Value.ToString())
                {
                    case "Уровень 1":
                        {
                            break;
                        }
                    case "Уровень 2":
                        {
                            e.Row.Cells[0].Style.Padding.Left = 10;
                            break;
                        }
                    case "Уровень 3":
                        {
                            e.Row.Cells[0].Style.Padding.Left = 20;
                            break;
                        }
                }
            }
            e.Row.Cells[0].Value = e.Row.Cells[0].Value.ToString().Replace("Все виды деятельности", "Итог");
            e.Row.Cells[0].Value = e.Row.Cells[0].Value.ToString().Replace("все виды фондов", "Итого");
        }

        #endregion

        #region Обработчики диаграммы

        protected void UltraChart1_DataBinding(object sender, EventArgs e)
        {
           
        }

        protected void UltraChart2_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText(String.Format("STAT_0001_0008_chart2_{0}", queryPostfix));
            dtChart = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Дата", dtChart);

            if (ComboKinds.SelectedIndex != 1)
            {
                dtChart.Columns.RemoveAt(0);
            }

            for (int row = 0; row < dtChart.Rows.Count; row++)
            {
                dtChart.Rows[row][0] = dtChart.Rows[row][0].ToString().Replace("(", "\n(");

                for (int col = 1; col < dtChart.Columns.Count; col++)
                {
                    double value;
                    if (Double.TryParse(dtChart.Rows[row][col].ToString(), out value))
                    {
                        dtChart.Rows[row][col] = value / 1000000;
                    }
                }
            }

            for (int i = 0; i < dtChart.Columns.Count; i++)
            {
                dtChart.Columns[i].Caption = dtChart.Columns[i].Caption.ToString().Replace(" ДАННЫЕ", String.Empty);
                dtChart.Columns[i].Caption = dtChart.Columns[i].Caption.ToString().Replace(")", String.Empty);
                dtChart.Columns[i].Caption = dtChart.Columns[i].Caption.ToString().Replace("(", String.Empty);

                dtChart.Columns[i].ColumnName = dtChart.Columns[i].ColumnName.ToString().Replace(" ДАННЫЕ", String.Empty);
                dtChart.Columns[i].ColumnName = dtChart.Columns[i].ColumnName.ToString().Replace(")", String.Empty);
                dtChart.Columns[i].ColumnName = dtChart.Columns[i].ColumnName.ToString().Replace("(", String.Empty);
            }

            dtChart.AcceptChanges();

            UltraChart2.DataSource = dtChart;
        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = PageTitle.Text;
            e.CurrentWorksheet.Rows[1].Cells[0].Value = PageSubTitle.Text;
        }

        private void ExcelExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs e)
        {


        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;

            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid1);
        }

        private int offset = 0;

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {
            UltraGridColumn col = UltraWebGrid1.DisplayLayout.Bands[0].Columns[e.CurrentColumnIndex + offset];
            while (col.Hidden)
            {
                offset++;
                col = UltraWebGrid1.DisplayLayout.Bands[0].Columns[e.CurrentColumnIndex + offset];
            }
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
            System.Drawing.Font font = new System.Drawing.Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(PageTitle.Text.Replace("&nbsp;", String.Empty));

            title = e.Section.AddText();
            font = new System.Drawing.Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(PageSubTitle.Text.Replace("&nbsp;", String.Empty));

            UltraChart1.Legend.Margins.Right = 0;
            UltraChart1.Width = UltraWebGrid1.Width;
            e.Section.AddImage(UltraGridExporter.GetImageFromChart(UltraChart1));

            UltraChart2.Legend.Margins.Right = 0;
            UltraChart2.Width = UltraWebGrid1.Width;
            e.Section.AddImage(UltraGridExporter.GetImageFromChart(UltraChart2));
        }

        private void PdfExporter_EndExport(object sender, EndExportEventArgs e)
        {

        }

        #endregion

    }
}
