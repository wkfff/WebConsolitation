using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.MFRF_0004_0003
{
    public partial class DefaultValuation : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid = new DataTable();
        private DataTable dtChart = new DataTable();
        private int firstYear = 2008;
        private int endYear = 2009;
        private int year = 2008;
        private string month = "Январь";
        private int chartColumnIndex = 1;

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 15);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.5 - 160);
            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);

            UltraChart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 15);
            UltraChart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.5 - 70);
            
            #region Настройка диаграмм

            UltraChart.ChartType = ChartType.ColumnChart;
            UltraChart.Border.Thickness = 0;
            UltraChart.Axis.X.Extent = 150;
            UltraChart.Axis.Y.Extent = 25;

            UltraChart.Axis.X.Labels.Visible = true;
            UltraChart.Axis.X.Labels.SeriesLabels.Visible = false;
            UltraChart.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;

            UltraChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:P0>";

            UltraChart.Tooltips.FormatString = "<ITEM_LABEL>\n<DATA_VALUE:P2>";
            UltraChart.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);

            AxisLabelLayoutAppearance layout = UltraChart.Axis.X.Labels.SeriesLabels.Layout;

            layout.Behavior = AxisLabelLayoutBehaviors.UseCollection;
            layout.BehaviorCollection.Clear();
            ClipTextAxisLabelLayoutBehavior behavior = new ClipTextAxisLabelLayoutBehavior();
            behavior.ClipText = false;
            behavior.Enabled = true;
            behavior.Trimming = StringTrimming.None;
            behavior.UseOnlyToPreventCollisions = false;
            layout.BehaviorCollection.Add(behavior);

            #endregion

            GridSearch1.LinkedGridId = this.UltraWebGrid.ClientID;

            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            
            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler
                    <Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);
            UltraGridExporter1.PdfExporter.EndExport += new EventHandler
                <Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs>(PdfExporter_EndExport);
            UltraGridExporter1.MultiHeader = true;

            CrossLink1.Text = "Динамика&nbsp;исполнения&nbsp;расходов&nbsp;ГРБС";
            CrossLink1.NavigateUrl = "~/reports/MFRF_0004_0004/DefaultDynamic.aspx";
            CrossLink2.Text = "Структура&nbsp;расходов&nbsp;ГРБС";
            CrossLink2.NavigateUrl = "~/reports/MFRF_0004_0003/DefaultValuationChart.aspx";
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("MFRF_0004_0003_date");
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                month = dtDate.Rows[0][3].ToString();

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

                ComboKOSGU.Title = "Вид расхода КОСГУ";
                ComboKOSGU.Width = 300;
                ComboKOSGU.MultiSelect = false;
                ComboKOSGU.FillDictionaryValues(CustomMultiComboDataHelper.FillKOSGUNames(DataDictionariesHelper.OutcomesKOSGUTypes));
                ComboKOSGU.SetСheckedState("Итого", true);
            }
            
            Label1.Text = "Исполнение расходов ГРБС";
            Page.Title = Label1.Text;
            
            int monthNum = ComboMonth.SelectedIndex + 1;
            year = Convert.ToInt32(ComboYear.SelectedValue);
            Label2.Text = string.Format("за {0} {1} {2} года ({3})", monthNum, CRHelper.RusManyMonthGenitive(monthNum), year, ComboKOSGU.SelectedValue);

            lbSubject.Text = "Распределение ГРБС по проценту исполнения";

            UserParams.PeriodMonth.Value = ComboMonth.SelectedValue;
            UserParams.PeriodYear.Value = year.ToString();
            UserParams.PeriodLastYear.Value = (year - 1).ToString();
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.FKRFilter.Value = DataDictionariesHelper.OutcomesKOSGUTypes[ComboKOSGU.SelectedValue];

            chartColumnIndex = 1;
            UserParams.Filter.Value = (chartColumnIndex == 1) ? "Процент исполнения" : "Доля";

            UltraWebGrid.DataBind();
            UltraChart.DataBind();
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("MFRF_0004_0003_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, " ", dtGrid);

            foreach (DataRow row in dtGrid.Rows)
            {
                for (int i = 0; i < row.ItemArray.Length; i++)
                {
                    if (row[i] != DBNull.Value && (i == 2 || i == 3))
                    {
                        row[i] = Convert.ToDouble(row[i]) / 1000000;
                    }
                }
            }

            UltraWebGrid.DataSource = dtGrid;
        }

        void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
//            if (ComboFO.SelectedIndex != 0)
//            {
//                UltraWebGrid.Height = Unit.Empty;
//            }
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(370);

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                string formatString = "N0";
                int widthColumn = 25;

                switch (i)
                {
                    case 1:
                        {
                            formatString = "N0";
                            widthColumn = 42;
                            break;
                        }
                    case 2:
                    case 3:
                        {
                            formatString = "N3";
                            widthColumn = 150;
                            break;
                        }
                    case 4:
                    case 7:
                    case 8:
                        {
                            formatString = "P2";
                            widthColumn = 80;
                            break;
                        }
                    case 5:
                    case 9:
                        {
                            formatString = "N0";
                            widthColumn = 80;
                            break;
                        }
                }

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(widthColumn);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[i].Header.Style.Wrap = true;
            }

            e.Layout.Bands[0].Columns[6].Hidden = true;
            e.Layout.Bands[0].Columns[10].Hidden = true;

            int monthNum = ComboMonth.SelectedIndex + 1;

            if (monthNum == 12)
            {
                monthNum = 1;
            }
            else
            {
                monthNum++;
            }

            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 0, "Наименование ГРБС ФБ", "");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 1, "Код", "");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 2, string.Format("Уточненная роспись на {0}&nbsp;год, млн.руб.", year),
                string.Format("Объем бюджетных ассигнований ГРБС в {0} году согласно бюджетной росписи", year));
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 3, string.Format("Кассовое исполнение на 1&nbsp;{0}&nbsp;{1}&nbsp;года, млн.руб.", CRHelper.RusMonthGenitive(monthNum), year),
                string.Format("Кассовое исполнение расходов ГРБС за {0} {1} {2} года", monthNum - 1, CRHelper.RusManyMonthGenitive(monthNum), year));
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 4, "Процент исполнения", "Процент исполнения бюджетной росписи");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 5, "Ранг по %", "Ранг среди ГРБС по проценту исполнения бюджетной росписи");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 7, "Процент исполнения прошлый год", "Процент исполнения бюджетной росписи за аналогичниый период прошлого года");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 8, "Доля", "Удельный вес в общей сумме расходов");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 9, "Ранг по доле", "Ранг среди ГРБС по удельному весу в общей сумме расходов");
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                bool rankColumn = (i == 5 || i == 9);
                string bestStar = (i == 5)
                                      ? "Самый высокий процент исполнения среди ГРБС"
                                      : "Самая большая доля в расходах среди ГРБС";
                string badStar = (i == 5)
                      ? "Самый низкий процент исполнения среди ГРБС"
                      : "Самая маленькая доля в расходах среди ГРБС";

                if (e.Row.Cells[0].Value != null && e.Row.Cells[0].Value.ToString() == "Всего по ГРБС")
                {
                    e.Row.Cells[i].Style.Font.Bold = true;

                    if (rankColumn || i == 1)
                    {
                        e.Row.Cells[i].Value = string.Empty;
                    }
                }

                if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty &&
                     (rankColumn) && e.Row.Cells[i + 1].Value != null)
                {
                    int rank = Convert.ToInt32(e.Row.Cells[i].Value);
                    int badRank = Convert.ToInt32(e.Row.Cells[i + 1].Value);
                    string hint = string.Empty;

                    if (rank == 1)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/starYellowBB.png";
                        hint = bestStar;
                    }
                    else
                    {
                        if (rank == badRank)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/starGrayBB.png";
                            hint = badStar;
                        }
                    }
                    e.Row.Cells[i].Title = hint;
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }
                
                UltraGridCell c = e.Row.Cells[i];
                if (c.Value != null && c.Value.ToString() != string.Empty)
                {
                    decimal value;
                    if (decimal.TryParse(c.Value.ToString(), out value))
                    {
                        if (value < 0)
                        {
                            c.Style.ForeColor = Color.Red;
                        }
                    }
                }
            }
        }

        #endregion

        #region Обработчики диаграмм

        protected void UltraChart1_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("MFRF_0004_0003_chart");
            dtChart = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "ГРБС", dtChart);

            if (dtChart == null || dtChart.Rows.Count == 1)
            {
                UltraWebGrid.Height = Unit.Empty;
                return;
            }

            // заменяем двойные кавычки на одинарные в названии ГРБС
            foreach (DataRow row in dtChart.Rows)
            {
                if (row[0] != DBNull.Value && row[0].ToString() != string.Empty)
                {
                    row[0] = row[0].ToString().Replace("\"", "'");
                }
            }

            // получаем среднее значение (последний элемент в таблице)
            double avgValue = 0;
            if (dtChart.Rows.Count != 0 && dtChart.Rows[dtChart.Rows.Count - 1][0] != DBNull.Value)
            {
                DataRow row = dtChart.Rows[dtChart.Rows.Count - 1];
                if (row[0].ToString() == "Среднее")
                {
                    avgValue = Convert.ToDouble(row[chartColumnIndex]);
                    // удаляем строку со средним из таблицы
                    dtChart.Rows.Remove(row);
                }
            }

            // рассчитываем медиану
            int medianIndex = MedianIndex(dtChart.Rows.Count);
            DataTable medianDT = dtChart.Clone();
            for (int i = 0; i < dtChart.Rows.Count; i++)
            {
                medianDT.ImportRow(dtChart.Rows[i]);
                if (i == medianIndex)
                {
                    DataRow row = medianDT.NewRow();
                    row[0] = "Медиана";
                    row[1] = MedianValue(dtChart, "Процент исполнения");
                    row[2] = MedianValue(dtChart, "Доля");
                    medianDT.Rows.Add(row);
                }

                double value;
                Double.TryParse(dtChart.Rows[i][chartColumnIndex].ToString(), out value);
                if (value >= avgValue && i != dtChart.Rows.Count - 1)
                {
                    Double.TryParse(dtChart.Rows[i + 1][chartColumnIndex].ToString(), out value);
                    if (value < avgValue)
                    {
                        DataRow row = medianDT.NewRow();
                        row[0] = "Среднее";
                        row[chartColumnIndex] = avgValue;
                        medianDT.Rows.Add(row);
                    }
                }
            }

            NumericSeries series1 = CRHelper.GetNumericSeries(chartColumnIndex, medianDT);
            series1.Label = "Процент исполнения";
            UltraChart.Series.Add(series1);
        }

        protected void UltraChart_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Box)
                {
                    Box box = (Box)primitive;
                    if (box.DataPoint != null)
                    {
                        if (box.Value != null && box.Value.ToString() != string.Empty)
                        {
                            box.PE.Fill = Color.SkyBlue;
                            box.PE.FillStopColor = Color.RoyalBlue;
                        }

                        if (box.DataPoint.Label == "Среднее" || box.DataPoint.Label == "Медиана")
                        {
                            box.PE.Fill = Color.Yellow;
                            box.PE.FillStopColor = Color.Orange;
                        }

                        box.DataPoint.Label = DataDictionariesHelper.GetFullGRBSName(box.DataPoint.Label).Replace("\"", "'");
                    }
                }

                if (primitive is Text)
                {
                    Text text = (Text)primitive;
                    if (text.GetTextString() == "Среднее" || text.GetTextString() == "Медиана")
                    {
                        LabelStyle boldStyle = text.GetLabelStyle();
                        boldStyle.Font = new Font("Verdana", 8, FontStyle.Bold);
                        boldStyle.FontColor = Color.Black;
                        text.SetLabelStyle(boldStyle);
                    }
                }
            }
        }

        #endregion

        #region Расчет медианы

        private static bool Even(int input)
        {
            if (input % 2 == 0)
            {
                return true;
            }
            return false;
        }

        private static int MedianIndex(int length)
        {
            if (length == 0)
            {
                return 0;
            }

            if (Even(length))
            {
                return length / 2 - 1;
            }
            else
            {
                return (length + 1) / 2 - 1;
            }
        }

        private static double MedianValue(DataTable dt, string medianValueColumn)
        {
            if (dt.Rows.Count == 0)
            {
                return 0;
            }

            if (Even(dt.Rows.Count))
            {
                double value1;
                double value2;
                Double.TryParse(
                        dt.Rows[MedianIndex(dt.Rows.Count)][medianValueColumn].ToString(),
                        out value1);
                Double.TryParse(
                        dt.Rows[MedianIndex(dt.Rows.Count) + 1][medianValueColumn].ToString(),
                        out value2);
                return (value1 + value2) / 2;
            }
            else
            {
                double value;
                Double.TryParse(
                        dt.Rows[MedianIndex(dt.Rows.Count)][medianValueColumn].ToString(),
                        out value);
                return value;
            }
        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = Label1.Text + " " + Label2.Text;
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            for (int i = 0; i < UltraWebGrid.Bands[0].Columns.Count; i = i + 1)
            {
                string formatString = "#,##0.00;[Red]-#,##0.00";
                int widthColumn = 500;

                switch (i)
                {
                    case 1:
                    case 5:
                    case 8:
                        {
                            formatString = "#,##0";
                            widthColumn = 60;
                            break;
                        }
                    case 2:
                    case 3:
                        {
                            formatString = "#,##0.000;[Red]-#,##0.000";
                            widthColumn = 95;
                            break;
                        }
                    case 4:
                    case 7:
                    case 6:
                        {
                            formatString = UltraGridExporter.ExelPercentFormat;
                            widthColumn = 85;
                            break;
                        }
                }

                e.CurrentWorksheet.Columns[i].CellFormat.FormatString = formatString;
                e.CurrentWorksheet.Columns[i].Width = widthColumn * 37;
            }
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;
            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";

            UltraWebGrid.Bands[0].Columns[2].Header.Caption = UltraWebGrid.Bands[0].Columns[2].Header.Caption.Replace("&nbsp;", " ");
            UltraWebGrid.Bands[0].Columns[3].Header.Caption = UltraWebGrid.Bands[0].Columns[3].Header.Caption.Replace("&nbsp;", " ");

            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid);
        }

        #endregion

        #region Экспорт в Pdf

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";

            UltraWebGrid.Bands[0].Columns[2].Header.Caption = UltraWebGrid.Bands[0].Columns[2].Header.Caption.Replace("&nbsp;", " ");
            UltraWebGrid.Bands[0].Columns[3].Header.Caption = UltraWebGrid.Bands[0].Columns[3].Header.Caption.Replace("&nbsp;", " ");

            // почему-то сдвигаются заголовки
            UltraWebGrid.Bands[0].Columns[7].Header.Caption = "Доля";
            UltraWebGrid.Bands[0].Columns[8].Header.Caption = "Ранг по доле";

            UltraGridExporter1.PdfExporter.Export(UltraWebGrid);
        }

        private void PdfExporter_BeginExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
        {
            IText title = e.Section.AddText();
            Font font = new Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(Label1.Text);

            title = e.Section.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(Label2.Text);
        }

        private void PdfExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs e)
        {
            IText title = e.Section.AddText();
            Font font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(lbSubject.Text);

            Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromChart(UltraChart);
            e.Section.AddImage(img);
        }

        #endregion
    }
}