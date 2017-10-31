using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Dundas.Maps.WebControl;
using Infragistics.Documents.Excel;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.DocumentExport;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using EndExportEventArgs=Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs;

namespace Krista.FM.Server.Dashboards.reports.RG_0001_0002
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable dtGrid = new DataTable();
        private string formatString = "N0";
        private string measure = String.Empty;
        private bool direct;

        #endregion

        #region Параметры запроса
        
        #endregion

        private bool IsSmallResolution
        {
            get { return CRHelper.GetScreenWidth < 1200; }
        }

        private int MinScreenWidth
        {
            get { return IsSmallResolution ? 750 : CustomReportConst.minScreenWidth; }
        }

        private int MinScreenHeight
        {
            get { return IsSmallResolution ? 600 : CustomReportConst.minScreenHeight; }
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region Инициализация параметров запроса

            

            #endregion

            UltraWebGrid.Width = Unit.Empty;
            UltraWebGrid.Height = Unit.Empty;

            UltraGridExporter1.MultiHeader = true;
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);

            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler<DocumentExportEventArgs>(PdfExporter_BeginExport);

            UltraChart1.Width = CRHelper.GetChartWidth(MinScreenWidth - 30);
            UltraChart1.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight / 2);

            UltraWebGrid.InitializeRow += new Infragistics.WebUI.UltraWebGrid.InitializeRowEventHandler(UltraWebGrid_InitializeRow);

            UltraChart1.ChartType = ChartType.LineChart;
            UltraChart1.Border.Thickness = 0;
            UltraChart1.Axis.Y.Extent = 55;
            UltraChart1.Axis.X.Extent = 45;
            
            UltraChart1.Legend.Visible = true;
            UltraChart1.Legend.Location = LegendLocation.Bottom;
            UltraChart1.Legend.SpanPercentage = 15;
            UltraChart1.Axis.X.Labels.SeriesLabels.Visible = true;
            UltraChart1.Axis.X.Labels.SeriesLabels.Layout.Behavior = AxisLabelLayoutBehaviors.None;
            UltraChart1.Axis.X.Labels.SeriesLabels.Font = new Font("Verdana", 10);
            UltraChart1.Axis.X.Labels.SeriesLabels.FontColor = Color.Black;
            
            UltraChart1.Data.ZeroAligned = false;
            UltraChart1.LineChart.NullHandling = NullHandling.DontPlot;
            UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart1.Axis.Y.TickmarkStyle = AxisTickStyle.Percentage;
            UltraChart1.Axis.Y.TickmarkPercentage = 25;
            UltraChart1.TitleLeft.Visible = true;
            UltraChart1.TitleLeft.Font = new Font("Verdana", 10);
            UltraChart1.TitleLeft.HorizontalAlign = StringAlignment.Center;
            //UltraChart1.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);
            UltraChart1.Annotations.Visible = true;

            UltraChart1.ColorModel.ModelStyle = ColorModels.CustomSkin;

            UltraChart1.ColorModel.Skin.PEs.Clear();
            for (int i = 1; i <= 7; i++)
            {
                PaintElement pe = new PaintElement();
                Color color = Color.White;
                switch (i)
                {
                    case 1:
                        {
                            color = Color.Green;
                            break;
                        }
                    case 2:
                        {
                            color = Color.Gold;
                            break;
                        }
                    case 3:
                        {
                            color = Color.Black;
                            break;
                        }
                    case 4:
                        {
                            color = Color.LightSlateGray;
                            break;
                        }
                    case 5:
                        {
                            color = Color.Red;
                            break;
                        }
                    case 6:
                        {
                            color = Color.Blue;
                            break;
                        }
                    case 7:
                        {
                            color = Color.DarkViolet;
                            break;
                        }
                }
                pe.Fill = color;
                pe.StrokeWidth = 2;
                
                UltraChart1.ColorModel.Skin.PEs.Add(pe);
            }
            UltraChart1.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                FillComboRegions();
                ComboRegion.Title = "Территория";
                ComboRegion.Width = 250;
                ComboRegion.SetСheckedState("Курганская область", true);
                ComboRegion.ParentSelect = true;

                FillComboIndicators();
                ComboIndicator.Title = "Показатель";
                ComboIndicator.Width = 700;
                ComboIndicator.SetСheckedState("Объем валового регионального продукта на одного жителя", true);
                ComboIndicator.ParentSelect = true;
                UserParams.Filter.Value = "Объем валового регионального продукта на одного жителя";
            } 
            
            Page.Title = "Анализ показателей по оценке эффективности деятельности ОИВ";
            PageTitle.Text = "Анализ показателей по оценке эффективности деятельности ОИВ";
            PageSubTitle.Text =
                "Анализ показателей по оценке эффективности деятельности органов исполнительной власти согласно оперативному краткому перечню индикаторов эффективности.";

            UserParams.SubjectFO.Value = ComboRegion.SelectedValue;
            UserParams.Filter.Value = ComboIndicator.SelectedValue;

            string query = DataProvider.GetQueryText("RG_0001_0002_date");
            dtDate = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
           
            query = DataProvider.GetQueryText("RG_0001_0002_measures");

            DataTable dtMeasures = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатель", dtMeasures);

            measure = dtMeasures.Rows[0][0].ToString().ToLower();
            direct = dtMeasures.Rows[0][2].ToString() == "0";
            
            string chartTitleLeft = measure;

            switch (measure)
            {
                case "процент":
                    {
                        measure = "%";
                        formatString = "N2";
                        UltraChart1.Tooltips.FormatString = String.Format("<SERIES_LABEL>\n<ITEM_LABEL>\n<DATA_VALUE:N2> {0}", measure);
                        UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>%";
                        UltraChart1.TitleLeft.Visible = false;
                        break;
                    }
                case "рубль":
                    {
                        measure = "руб.";
                        if (ComboIndicator.SelectedValue != "Оборот розничной торговли")
                        {
                            formatString = "N2";
                            UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
                            UltraChart1.Tooltips.FormatString = String.Format("<SERIES_LABEL>\n<ITEM_LABEL>\n<DATA_VALUE:N2> {0}", measure);
                        }
                        break;
                    }
                case "место":
                    {
                        measure = "мест";
                        formatString = "N0";
                        UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
                        UltraChart1.Tooltips.FormatString = String.Format("<SERIES_LABEL>\n<ITEM_LABEL>\n<DATA_VALUE:N0> {0}", measure);
                        break;
                    }
                case "год":
                    {
                        measure = "лет";
                        formatString = "N2";
                        UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
                        UltraChart1.Tooltips.FormatString = String.Format("<SERIES_LABEL>\n<ITEM_LABEL>\n<DATA_VALUE:N2> {0}", measure);
                        break;
                    }
                case "на 1000 человек":
                    {
                        formatString = "N2";
                        UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
                        UltraChart1.Tooltips.FormatString = String.Format("<SERIES_LABEL>\n<ITEM_LABEL>\n<DATA_VALUE:N2> {0}", measure);
                        break;
                    }
                case "на 10000 человек":
                    {
                        formatString = "N2";
                        UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
                        UltraChart1.Tooltips.FormatString = String.Format("<SERIES_LABEL>\n<ITEM_LABEL>\n<DATA_VALUE:N2> {0}", measure);
                        break;
                    }
                case "раз (отношение)":
                    {
                        formatString = "N3";
                        UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N2>";
                        UltraChart1.Tooltips.FormatString = String.Format("<SERIES_LABEL>\n<ITEM_LABEL>\n<DATA_VALUE:N3> {0}", measure);
                        break;
                    }
                case "доля":
                    {
                        formatString = "N3";
                        UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N2>";
                        UltraChart1.Tooltips.FormatString = String.Format("<SERIES_LABEL>\n<ITEM_LABEL>\n<DATA_VALUE:N3> {0}", measure);
                        break;
                    }
                default:
                    {
                        UltraChart1.Tooltips.FormatString = String.Format("<SERIES_LABEL>\n<ITEM_LABEL>\n<DATA_VALUE:N0> {0}", measure);
                        break;
                    }
            }

            UltraChart1.TitleLeft.Text = chartTitleLeft;
            UltraChart1.TitleLeft.Visible = true;

            chartElementCaption.Text =
                String.Format("Динамика {0}, {1}", UserParams.Filter.Value,
                              RegionsNamingHelper.ShortName(UserParams.SubjectFO.Value));

            //mapElementCaption.Text = String.Format("{0} по субъектам УрФО за {1}", UserParams.Filter.Value, element);
            
            UltraWebGrid.DataBind();

            if (quarterSpliting)
            {
                UltraChart1.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;
                UltraChart1.Axis.X.Extent = 100;
            }

            UltraChart1.DataBind();
        }
        

        #region Обработчики грида

        //private bool quaterSpliting = false;

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText(String.Format("RG_0001_0002_Grid"));
            dtGrid = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Период", dtGrid);

            Dictionary<int, string> years = new Dictionary<int, string>();

            for (int i = 0; i < dtGrid.Rows.Count; i++)
            {
                if (dtGrid.Rows[i][1] != DBNull.Value && 
                    !String.IsNullOrEmpty(dtGrid.Rows[i][1].ToString()))
                {
                    years.Add(i, dtGrid.Rows[i][1].ToString());
                }
            }
            int j = 0;
            foreach (KeyValuePair<int, string> pair in years)
            {
                DataRow row = dtGrid.NewRow();
                row[0] = pair.Value;
                dtGrid.Rows.InsertAt(row, pair.Key + j);
                j++;
            }
            dtGrid.Columns.RemoveAt(1);
            dtGrid.AcceptChanges();
            ((UltraWebGrid)sender).DataSource = dtGrid;
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.FilterOptionsDefault.AllowRowFiltering = RowFiltering.No;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(280, 1280);
            
            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
            }
        }

        bool quarterSpliting = false;
        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            
            if (e.Row.Cells[0].Value.ToString().Contains("год"))
            {
                e.Row.Cells[0].Style.Font.Bold = true;
                e.Row.Cells[0].ColSpan = e.Row.Cells.Count;
                quarterSpliting = true;
            }
            else if (quarterSpliting)
            {
                e.Row.Cells[0].Style.Padding.Left = 20;
            }
            else 
            {
                e.Row.Cells[0].Value = String.Format("{0} год", e.Row.Cells[0].Value);
            }

            int firstDataRow = quarterSpliting ? 1 : 0;
            //int firstQuarterOffset = quarterSpliting ? 1 : 0;

            if (e.Row.Index == firstDataRow)
            {
                return;
            }

            for (int i = 1; i < e.Row.Cells.Count; i++)
            {
                int firstQuarterOffset = 1;
                if (e.Row.Cells[0].Value != null && e.Row.Cells[0].Value.ToString().Contains("Квартал 1"))
                {
                    firstQuarterOffset = 2;
                }

                if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty &&
                    e.Row.Band.Grid.Rows[e.Row.Index - firstQuarterOffset].Cells[i].Value != null && e.Row.Band.Grid.Rows[e.Row.Index - firstQuarterOffset].Cells[i].Value.ToString() != string.Empty)
                {
                    if (Convert.ToInt32(e.Row.Cells[i].Value) < Convert.ToInt32(e.Row.Band.Grid.Rows[e.Row.Index - firstQuarterOffset].Cells[i].Value))
                    {
                        e.Row.Cells[i].Style.BackgroundImage = direct ? "~/images/arrowGreenDownBB.png" : "~/images/arrowRedDownBB.png";
                        //   e.Row.Cells[i].Title = isRedundantLevelRank ? "Самый высокий уровень безработицы" : "Самое большое число безработных на 1 вакансию";
                    }
                    else
                    {
                        e.Row.Cells[i].Style.BackgroundImage = direct ? "~/images/arrowRedUpBB.png" : "~/images/arrowGreenUpBB.png";
                        //   e.Row.Cells[i].Title = isRedundantLevelRank ? "Самый низкий уровень безработицы" : "Самое маленькое число безработных на 1 вакансию";
                    }
                }
                e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
            }
        }

        #endregion

        #region Обработчики диаграммы

        private static string GetDtColumnUniqueName(DataTable dtChart, string key)
        {
            string newKey = key;
            while (dtChart.Columns.Contains(newKey))
            {
                newKey += " ";
            }
            return newKey;
        }

        protected void UltraChart1_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("RG_0001_0002_Chart");
            DataTable dtChart = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Дата", dtChart);

            //for (int i = 0; i < dtChart.Columns.Count; i++ )
            //{
            //    dtChart.Columns[i].ColumnName = GetDtColumnUniqueName(dtChart, dtChart.Rows[0][i].ToString());
            //}

            dtChart.Columns.RemoveAt(0);
            dtChart.AcceptChanges();

            UltraChart1.Data.SwapRowsAndColumns = true;
            //UltraChart1.Series.Clear();
            //UltraChart1.Series.Add(CRHelper.GetNumericSeries(1, dtChart));
            //UltraChart1.Series.Add(CRHelper.GetNumericSeries(2, dtChart));
            //UltraChart1.Series.Add(CRHelper.GetNumericSeries(3, dtChart));
            //UltraChart1.Series.Add(CRHelper.GetNumericSeries(4, dtChart));
            //UltraChart1.Series.Add(CRHelper.GetNumericSeries(5, dtChart));
            //UltraChart1.Series.Add(CRHelper.GetNumericSeries(6, dtChart));
            UltraChart1.DataSource = dtChart;
        }
        #endregion
        
        #region Экпорт в Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            foreach (Worksheet sheet in e.Workbook.Worksheets)
            {
                if (sheet.Name == "Расходы по КОСГУ" || sheet.Name == "Расходы по РзПр")
                {
                    sheet.Columns[0].Width = 340*37;
                    sheet.Columns[1].Width = 30*37;
                    sheet.Columns[2].Width = 120*37;
                    sheet.Columns[3].Width = 120*37;
                    sheet.Columns[4].Width = 100*37;

                    sheet.Columns[1].CellFormat.FormatString = "#,##0";
                    sheet.Columns[2].CellFormat.FormatString = "#,##0.0##";
                    sheet.Columns[3].CellFormat.FormatString = "#,##0.0##";
                    sheet.Columns[4].CellFormat.FormatString = UltraGridExporter.ExelPercentFormat;
                }
                else
                {
                    sheet.Columns[0].Width = 340 * 37;
                    sheet.Columns[1].Width = 120 * 37;
                    sheet.Columns[2].Width = 120 * 37;
                    sheet.Columns[3].Width = 100 * 37;

                    sheet.Columns[1].CellFormat.FormatString = "#,##0.0##";
                    sheet.Columns[2].CellFormat.FormatString = "#,##0.0##";
                    sheet.Columns[3].CellFormat.FormatString = UltraGridExporter.ExelPercentFormat;
                }
            }
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            
         }

        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
           
        }

        private bool titleAndMapAdded = false;

        private void PdfExporter_BeginExport(object sender, DocumentExportEventArgs e)
        {
            
        }

        private void InitializeExportLayout(DocumentExportEventArgs e)
        {
           
        }

        #endregion

        private void FillComboRegions()
        {
            DataTable dtRegions = new DataTable();
            string query = DataProvider.GetQueryText("RG_0001_0002_regions");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Район", dtRegions);

            Dictionary<string, int> regions = new Dictionary<string, int>();
            foreach (DataRow row in dtRegions.Rows)
            {
                regions.Add(row[0].ToString(), 0);
            }
            ComboRegion.FillDictionaryValues(regions);
        }

        private void FillComboIndicators()
        {
            DataTable dtIndicators = new DataTable();
            string query = DataProvider.GetQueryText("RG_0001_0002_Indicators");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Район", dtIndicators);

            Dictionary<string, int> regions = new Dictionary<string, int>();
            foreach (DataRow row in dtIndicators.Rows)
            {
                regions.Add(row[0].ToString(), 0);
            }
            ComboIndicator.FillDictionaryValues(regions);
        }
    }
}