using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Color=System.Drawing.Color;
using Font=System.Drawing.Font;

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0002_Karelia
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable dtGrid;
        private DataTable dtChart;
        private DataTable dtChartPareto;
        private int firstYear = 2005;
        private int endYear = 2011;
        private string month = "Январь";
        private const int cellCount = 36;

        #endregion

        #region Параметры запроса

        // уровень МР и ГО
        private CustomParam regionsLevel;
        // функция для подсчета элементов
        private CustomParam elementCount;
        // уровень бюджета
        private CustomParam budgetLevel;
        // тип документа
        private CustomParam documentType;
        // выбранная мера
        private CustomParam selectedMeasure;
        // другая мера
        private CustomParam otherMeasure;

        #endregion

        private bool IsFact
        {
            get { return MeasureFact.Checked; }
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region Инициализация параметров запроса

            if (regionsLevel == null)
            {
                regionsLevel = UserParams.CustomParam("regions_level");
            }
            if (elementCount == null)
            {
                elementCount = UserParams.CustomParam("element_count");
            }
            if (budgetLevel == null)
            {
                budgetLevel = UserParams.CustomParam("budget_level");
            }
            if (documentType == null)
            {
                documentType = UserParams.CustomParam("document_type");
            }
            if (selectedMeasure == null)
            {
                selectedMeasure = UserParams.CustomParam("selected_measure");
            }
            if (otherMeasure == null)
            {
                otherMeasure = UserParams.CustomParam("other_measure");
            }

            #endregion

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 30);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.20 - 100);
            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);

            UltraChart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 25);
            UltraChart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.63);

            #region Настройка диаграммы

            UltraChart.ChartType = ChartType.ColumnChart;
            UltraChart.Border.Thickness = 0;
            UltraChart.Data.ZeroAligned = true;

            UltraChart.Axis.X.Extent = 160;
            UltraChart.Axis.X.Labels.Visible = false;
            UltraChart.Axis.X.Labels.SeriesLabels.Visible = true;
            UltraChart.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart.Axis.X.StripLines.PE.Fill = Color.Gainsboro;
            UltraChart.Axis.X.StripLines.PE.FillOpacity = 150;
            UltraChart.Axis.X.StripLines.PE.Stroke = Color.DarkGray;
            UltraChart.Axis.X.StripLines.Interval = 2;
            UltraChart.Axis.X.StripLines.Visible = true;
            UltraChart.Axis.Y.Extent = 65;
            UltraChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";

            UltraChart.Axis.X.Labels.WrapText = true;
            UltraChart.Axis.X.Labels.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart.Axis.X.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.None;

            UltraChart.Legend.Margins.Right = 3 * Convert.ToInt32(UltraChart.Width.Value) / 4;
            UltraChart.Legend.Visible = true;
            UltraChart.Legend.Location = LegendLocation.Top;
            UltraChart.Legend.SpanPercentage = 8;

            UltraChart.TitleLeft.Visible = true;
            UltraChart.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart.TitleLeft.Margins.Bottom = Convert.ToInt32(UltraChart.Height.Value) / 4;
            UltraChart.TitleLeft.Text = "Тыс.руб.";

            UltraChart.Tooltips.FormatString = "<SERIES_LABEL>\n<ITEM_LABEL>";
            UltraChart.ColorModel.ModelStyle = ColorModels.CustomLinear;
            UltraChart.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart.FillSceneGraph +=new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);

            #endregion

            UltraGridExporter1.MultiHeader = true;

            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);

            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler
                    <Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);
            UltraGridExporter1.PdfExporter.EndExport += new EventHandler
                <Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs>(PdfExporter_EndExport);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;
            UserParams.RegionDimension.Value = RegionSettingsHelper.Instance.RegionDimension;

            if (!Page.IsPostBack)
            {
                MeasureFact.Attributes.Add("onclick", string.Format("uncheck('{0}')", MeasurePlan.ClientID));
                MeasurePlan.Attributes.Add("onclick", string.Format("uncheck('{0}')", MeasureFact.ClientID));

                chartWebAsyncPanel.AddRefreshTarget(UltraChart);
                chartWebAsyncPanel.AddLinkedRequestTrigger(MeasureFact.ClientID);
                chartWebAsyncPanel.AddLinkedRequestTrigger(MeasurePlan.ClientID);

                dtDate = new DataTable();
                string queryName = string.Format("FO_0002_0002_Karelia_date_{0}", RegionSettingsHelper.Instance.GetPropertyValue("LastDateQueryName"));
                string query = DataProvider.GetQueryText(queryName);
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                if (dtDate.Rows.Count != 0)
                {
                    endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                    month = dtDate.Rows[0][3].ToString();
                }
                
                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);

                ComboMonth.Title = "Месяц";
                ComboMonth.Width = 130;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.SetСheckedState(month, true);
            }

            Page.Title = "Сбалансированность местных бюджетов";
            PageTitle.Text = Page.Title;
            chart1Label.Text = "Распределение по объему профицита(+)/дефицита(-) местных бюджетов";

            string monthValue = ComboMonth.SelectedValue;
            string yearValue = ComboYear.SelectedValue;

            if (!chartWebAsyncPanel.IsAsyncPostBack)
            {
                if (!Page.IsPostBack || !UserParams.PeriodYear.ValueIs(yearValue) ||
                    !UserParams.PeriodMonth.ValueIs(monthValue))
                {
                    int year = Convert.ToInt32(ComboYear.SelectedValue);

                    int monthNum = ComboMonth.SelectedIndex + 1;

                    UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(monthNum));
                    UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(monthNum));
                    UserParams.PeriodYear.Value = string.Format("[{0}].[{1}].[{2}].[{3}]", year,
                                                                UserParams.PeriodHalfYear.Value,
                                                                UserParams.PeriodQuater.Value,
                                                                CRHelper.RusMonth(monthNum));

                    regionsLevel.Value = RegionSettingsHelper.Instance.RegionsLevel;
                    elementCount.Value = RegionSettingsHelper.Instance.GetPropertyValue("ElementCount");
                    budgetLevel.Value = RegionSettingsHelper.Instance.GetPropertyValue("BudgetLevel");
                    documentType.Value = RegionSettingsHelper.Instance.GetPropertyValue("DocumentSKIFType");

                    PageSubTitle.Text = string.Format("за {0} {1} {2} года", monthNum, CRHelper.RusManyMonthGenitive(monthNum), year);
                }
                UltraWebGrid.DataBind();
            }
            selectedMeasure.Value = IsFact ? "Факт" : "Годовые назначения";
            otherMeasure.Value = !IsFact ? "Факт" : "Годовые назначения";
            UltraChart.DataBind();
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0002_Karelia_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Регион", dtGrid);

            DataTable dt = new DataTable();
            DataColumn column = new DataColumn("Показатель", typeof(string));
            dt.Columns.Add(column);
            column = new DataColumn("Всего; План", typeof(string));
            dt.Columns.Add(column);
            column = new DataColumn("Всего; Факт", typeof(string));
            dt.Columns.Add(column);
            column = new DataColumn("Муниципальные районы; План", typeof(string));
            dt.Columns.Add(column);
            column = new DataColumn("Муниципальные районы; Факт", typeof(string));
            dt.Columns.Add(column);
            column = new DataColumn("Городские округа; План", typeof(string));
            dt.Columns.Add(column);
            column = new DataColumn("Городские округа; Факт", typeof(string));
            dt.Columns.Add(column);

            int sixtetCounter = 0;

            for (int i = 0; i < cellCount; i += 6)
            {
                DataRow row = dt.NewRow();
                
                string markName = string.Empty;
                switch (sixtetCounter)
                {
                    case 0:
                        {
                            markName = "Количество местных бюджетов, всего";
                            break;
                        }
                    case 1:
                        {
                            markName = "Профицитные";
                            break;
                        }
                    case 2:
                        {
                            markName = "Дефицитные";
                            break;
                        }
                    case 3:
                        {
                            markName = "Сбалансированные";
                            break;
                        }
                    case 4:
                        {
                            markName = "Объем профицита, тыс.руб.";
                            break;
                        }
                    case 5:
                        {
                            markName = "Объем дефицита, тыс.руб.";
                            break;
                        }
                }

                row[0] = markName;

                for (int j = 0; j < 6; j++)
                {
                    if (dtGrid.Rows[0][j + i] != DBNull.Value && 
                        (markName == "Объем профицита, тыс.руб." || markName == "Объем дефицита, тыс.руб."))
                    {
                        row[j + 1] = (Convert.ToDouble(dtGrid.Rows[0][j + i])/1000).ToString("N3");
                    }
                    else if (dtGrid.Rows[0][j + i] != DBNull.Value)
                    {
                        row[j + 1] = Convert.ToInt32(dtGrid.Rows[0][j + i]);
                    }
                }

                dt.Rows.Add(row);
                sixtetCounter++;
            }

            UltraWebGrid.DataSource = dt;
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowSortingDefault = AllowSorting.No;

            for (int i =1; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(153);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[i].CellStyle.Padding.Right = 5;
            }

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(193);

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

            int multiHeaderPos = 1;

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 2)
            {
                ColumnHeader ch = new ColumnHeader(true);
                string[] captions = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';');
                ch.Caption = captions[0];

                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i, "План", "");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 1, "Факт", "");

                ch.RowLayoutColumnInfo.OriginY = 0;
                ch.RowLayoutColumnInfo.OriginX = multiHeaderPos;
                multiHeaderPos += 2;
                ch.RowLayoutColumnInfo.SpanX = 2;
                ch.Style.Wrap = true;
                e.Layout.Bands[0].HeaderLayout.Add(ch);
            }
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            switch (e.Row.Index)
            {
                case 1:
                case 4:
                    {
                        e.Row.Style.ForeColor = Color.Green;
                        break;
                    }
                case 2:
                case 5:
                    {
                        e.Row.Style.ForeColor = Color.Red;
                        break;
                    }
            }

            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                {
                    if (e.Row.Cells[0].Value != null &&
                        (e.Row.Cells[0].Value.ToString() == "Профицитные" ||
                         e.Row.Cells[0].Value.ToString() == "Дефицитные" ||
                         e.Row.Cells[0].Value.ToString() == "Сбалансированные"))
                    {
                        if (i == 0)
                        {
                            e.Row.Cells[i].Style.Padding.Left = 15;
                        }
                        else if (dtGrid != null)
                        {
                            int j = cellCount + (e.Row.Index - 1) * 6 + i - 1;

                            string value = dtGrid.Rows[0][j].ToString().Replace("br", "\r");
                            for (int k = 3; k <= 4; k++)
                            {
                                value = RegionsNamingHelper.CheckMultiplyValue(value.Replace("br", "\r"), k);
                            }
                            value = value.Replace("муниципальный район", "МР");
                            value = value.Replace("Муниципальный район", "МР");
                            value = value.Replace("район", "р-н");
                            e.Row.Cells[i].Title = value;
                        }
                    }
                }
            }
        }
        
        void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
            UltraWebGrid.Height = Unit.Empty;
        }

        #endregion

        #region Обработчики диаграммы

        protected void UltraChart_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0002_Karelia_chart");
            dtChart = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);

            foreach (DataRow row in dtChart.Rows)
            {
                for (int i = 0; i < row.ItemArray.Length; i++)
                {
                    if ((i == 1 || i == 2)
                         && row[i] != DBNull.Value)
                    {
                        row[i] = Convert.ToDouble(row[i]) / 1000;
                    }

                    if (i == 0 && row[i] != DBNull.Value)
                    {
                        row[i] = row[i].ToString().Replace("муниципальное образование", "МО");
                        row[i] = row[i].ToString().Replace("муниципальный район", "МР");
                        row[i] = row[i].ToString().Replace("Муниципальный район", "МР");
                        row[i] = row[i].ToString().Replace("\"", "'");
                        row[i] = row[i].ToString().Replace(" район", " р-н");
                    }
                }
            }

            if (dtChart.Columns.Count > 1)
            {
                dtChart.Columns[1].ColumnName = IsFact ? "Фактический профицит/дефицит" : "Плановый профицит/дефицит"; 
            }

            DataTable dtCopyChart = dtChart.Copy();
            if (dtCopyChart.Columns.Count > 2)
            {
                dtCopyChart.Columns.RemoveAt(2);
            }

            UltraChart.DataSource = dtCopyChart;
        }

        protected void UltraChart_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Text && primitive.Path != null && primitive.Path.Contains("Grid.X"))
                {
                    Text text = (Text)primitive;
                    text.bounds.Width = 30;
                    text.labelStyle.VerticalAlign = StringAlignment.Near;
                    text.labelStyle.FontSizeBestFit = false;
                    text.labelStyle.Font = new Font("Verdana", 8);
                    text.labelStyle.WrapText = true;
                }
                if (primitive is Box)
                {
                    Box box = (Box)primitive;
                    if (box.DataPoint != null && box.Value != null)
                    {
                        string otherMeasureText = string.Empty;
                        double otherValue = 0;
                        if (dtChart != null && dtChart.Rows[box.Row][2] != DBNull.Value && 
                            dtChart.Rows[box.Row][2].ToString() != string.Empty)
                        {
                            otherValue = Convert.ToDouble(dtChart.Rows[box.Row][2]);
                            if (IsFact)
                            {
                                otherMeasureText = otherValue > 0 ? "Плановый профицит (годовой)" : "Плановый дефицит (годовой)";
                            } 
                            else
                            {
                                otherMeasureText = otherValue > 0 ? "Фактический профицит" : "Фактический дефицит";
                            }
                        }

                        double value = Convert.ToDouble(box.Value);
                        if (value > 0)
                        {
                            box.DataPoint.Label = string.Format("{0} {1:N2} тыс.руб.\n ({2} {3:N2} тыс.руб.)", 
                                IsFact ? "Фактический профицит" : "Плановый профицит (годовой)",
                                value, otherMeasureText, otherValue);
                            box.PE.ElementType = PaintElementType.Gradient;
                            box.PE.FillGradientStyle = GradientStyle.Horizontal;
                            box.PE.Fill = Color.Green;
                            box.PE.FillStopColor = Color.ForestGreen;
                        }
                        else
                        {
                            box.DataPoint.Label = string.Format("{0} {1:N2} тыс.руб.\n ({2} {3:N2} тыс.руб.)",
                                IsFact ? "Фактический дефицит" : "Плановый дефицит (годовой)",
                                value, otherMeasureText, otherValue);
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

        #endregion

        #region Экспорт в Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = PageTitle.Text + " " + PageSubTitle.Text;
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            for (int i = 5; i < 11; i++)
            {
                string formatString = (i < 10) ? "#,##0" : "#,##0.000";
                for (int j = 1; j < 7; j++)
                {
                    e.CurrentWorksheet.Rows[i].Cells[j].Value = Convert.ToDecimal(e.CurrentWorksheet.Rows[i].Cells[j].Value);
                    e.CurrentWorksheet.Rows[i].Cells[j].CellFormat.FormatString = formatString;
                }
            }
        }

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {
            e.HeaderText = UltraWebGrid.DisplayLayout.Bands[0].Columns[e.CurrentColumnIndex].Header.Key.Split(';')[0];
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;

            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid);
        }

        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";
            UltraGridExporter1.PdfExporter.Export(UltraWebGrid);
        }

        private void PdfExporter_BeginExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
        {
            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(165);
            }

            IText title = e.Section.AddText();
            Font font = new Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(PageTitle.Text);

            title = e.Section.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(PageSubTitle.Text);
        }

        private void PdfExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs e)
        {
            IText title = e.Section.AddText();
            Font font = new Font("Verdana", 12);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(chart1Label.Text);

            Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromChart(UltraChart);
            e.Section.AddImage(img);
        }

        #endregion
    }
}
