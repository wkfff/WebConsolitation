using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.MFRF_0004_0003
{
    public partial class DefaultValuationChart : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid = new DataTable();
        private DataTable dtChart = new DataTable();
        private int firstYear = 2008;
        private int endYear = 2009;
        private int year = 2008;
        private string month = "Январь";
        private int monthNum = 1;

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);
            
            UltraChart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 35);
            UltraChart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight - 163);

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 15);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.5 - 160);
            // почему-то не меняются заголовки
            UltraWebGrid.EnableViewState = false;

            #region Настройка диаграмм

            UltraChart.ChartType = ChartType.StackBarChart;
            UltraChart.StackChart.StackStyle = StackStyle.Complete;

            UltraChart.Tooltips.FormatString = "<ITEM_LABEL>\n<DATA_VALUE_ITEM:N2>%";
            
            UltraChart.Legend.Visible = true;
            UltraChart.Legend.Location = LegendLocation.Bottom;
            UltraChart.Legend.SpanPercentage = 34;

            UltraChart.Axis.X.Extent = 30;
            UltraChart.Axis.Y.Extent = 40;

            UltraChart.Axis.X.Labels.Visible = true;
            UltraChart.Axis.Y.Labels.Visible = false;

            UltraChart.Axis.X.TickmarkStyle = AxisTickStyle.Percentage;
            UltraChart.Axis.X.Labels.ItemFormatString = "<DATA_VALUE:N0>%";

            UltraChart.Axis.Y.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart.Axis.Y.Labels.SeriesLabels.HorizontalAlign = StringAlignment.Near;
            UltraChart.Axis.Y.Labels.SeriesLabels.VerticalAlign = StringAlignment.Near;
            UltraChart.Axis.Y.Labels.SeriesLabels.Layout.Behavior = AxisLabelLayoutBehaviors.Auto;

            UltraChart.ColorModel.ModelStyle = ColorModels.PureRandom;
            
            UltraChart.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);
            UltraChart.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

            #endregion

            GridSearch1.LinkedGridId = this.UltraWebGrid.ClientID;

            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            
            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler
                    <Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);
            UltraGridExporter1.PdfExporter.EndExport += new EventHandler
                <Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs>(PdfExporter_EndExport);
            UltraGridExporter1.MultiHeader = true;

            CrossLink1.Text = "Динамика&nbsp;исполнения&nbsp;расходов&nbsp;ГРБС";
            CrossLink1.NavigateUrl = "~/reports/MFRF_0004_0004/DefaultDynamic.aspx";
            CrossLink2.Text = "Исполнение&nbsp;расходов&nbsp;ГРБС";
            CrossLink2.NavigateUrl = "~/reports/MFRF_0004_0003/DefaultValuation.aspx";
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
            }

            year = Convert.ToInt32(ComboYear.SelectedValue);
            monthNum = ComboMonth.SelectedIndex + 1;

            Label1.Text = "Структура расходов федерального бюджета по ГРБС";
            Page.Title = Label1.Text;
            Label2.Text = string.Format("за {0} {1} {2} года", monthNum, CRHelper.RusManyMonthGenitive(monthNum), year);

            UserParams.PeriodMonth.Value = ComboMonth.SelectedValue;
            UserParams.PeriodYear.Value = year.ToString();
            UserParams.PeriodLastYear.Value = (year - 1).ToString();
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(ComboMonth.SelectedIndex + 1));

            UltraWebGrid.DataBind();
            UltraChart.DataBind();
        }

        #region Обработчики диаграмм

        protected void UltraChart1_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("MFRF_0004_0003_chart_struct");
            dtChart = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "ГРБС", dtChart);
            
            if (dtChart != null)
            {
                dtChart.Columns[1].ColumnName = string.Format("План на {0} год", year);
                dtChart.Columns[2].ColumnName = string.Format("Факт\nза {0} {1}\n{2} года", monthNum, CRHelper.RusManyMonthGenitive(monthNum), year);
                dtChart.Columns[3].ColumnName = string.Format("Факт\nза {0} {1}\n{2} года", monthNum, CRHelper.RusManyMonthGenitive(monthNum), year - 1);
                dtChart.Columns[4].ColumnName = string.Format("Факт за {0} год", year - 1);
            }

            CRHelper.NormalizeDataTable(dtChart);

            for (int i = 1; i < dtChart.Columns.Count; i++)
            {
                NumericSeries series = CRHelper.GetNumericSeries(i, dtChart);
                ((UltraChart)sender).Series.Add(series);
            }
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
                        box.DataPoint.Label = DataDictionariesHelper.GetFullGRBSName(box.DataPoint.Label).Replace("\"", "'");
                    }
                }
            }
        }

        #endregion

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("MFRF_0004_0003_grid_struct");
            dtGrid = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Наименование ГРБС", dtGrid);

            foreach (DataRow row in dtGrid.Rows)
            {
                for (int i = 0; i < row.ItemArray.Length; i++)
                {
                    if (row[i] != DBNull.Value && (i == 2 || i == 4 || i == 6 || i == 8))
                    {
                        row[i] = Convert.ToDouble(row[i]) / 1000000;
                    }
                }
            }

            UltraWebGrid.DataSource = dtGrid;
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(365);

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
                    case 4:
                    case 6:
                    case 8:
                        {
                            formatString = "N3";
                            widthColumn = 90;
                            break;
                        }

                    case 3:
                    case 5:
                    case 7:
                    case 9:
                        {
                            formatString = "P4";
                            widthColumn = 90;
                            break;
                        }
                }

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(widthColumn);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[i].Header.Style.Wrap = true;
            }

            for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                if (i == 0 || i == 1)
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 0;
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.SpanY = 2;
                }
                else
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 1;
                }
            }

            int multiHeaderPos = 2;

            for (int i = 2; i < e.Layout.Bands[0].Columns.Count; i = i + 2)
            {
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i, "Сумма, млн.руб.", "");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 1, "Удельный вес", "Удельный вес в общей сумме расходов");

                string caption = string.Empty;
                string hint = string.Empty;
                int j = (i - 2)/2;
                switch (j)
                {
                    case 0:
                        {
                            caption = string.Format("План на {0} год", year);
                            hint = string.Format("Объем бюджетных ассигнований ГРБС в {0} году согласно бюджетной росписи", year);
                            break;
                        }
                    case 1:
                        {
                            caption = string.Format("Факт за<br />{0} {1} {2} года", monthNum,
                                              CRHelper.RusManyMonthGenitive(monthNum), year);
                            hint = string.Format("Кассовое исполнение расходов ГРБС за {0} {1} {2} года", monthNum,
                                              CRHelper.RusManyMonthGenitive(monthNum), year);
                            break;
                        }
                    case 2:
                        {
                            caption =
                                string.Format("Факт за<br />{0} {1} {2} года", monthNum,
                                              CRHelper.RusManyMonthGenitive(monthNum), year - 1);
                            hint = string.Format("Кассовое исполнение расходов ГРБС за {0} {1} {2} года", monthNum,
                                              CRHelper.RusManyMonthGenitive(monthNum), year - 1);
                            break;
                        }
                    case 3:
                        {
                            caption = string.Format("Факт за {0} год", year - 1);
                            hint = string.Format("Кассовое исполнение расходов ГРБС за {0} год", year - 1);
                            break;
                        }
                }

                ColumnHeader ch = CRHelper.AddHierarchyHeader(e.Layout.Grid,
                                                              0,
                                                              caption,
                                                              multiHeaderPos,
                                                              0,
                                                              2,
                                                              1);
                ch.Title = hint;

                multiHeaderPos += 2;
            }
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
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
                        {
                            formatString = "#,##0";
                            widthColumn = 60;
                            break;
                        }
                    case 2:
                    case 4:
                    case 6:
                    case 8:
                        {
                            formatString = "#,##0.000;[Red]-#,##0.000";
                            widthColumn = 95;
                            break;
                        }
                    case 3:
                    case 5:
                    case 7:
                    case 9:
                        {
                            formatString = "#,##0.0000%";
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

            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid);
        }

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {
            string caption = string.Empty;
            int i = (e.CurrentColumnIndex - 2) / 2;
            switch (i)
            {
                case 0:
                    {
                        caption = string.Format("План на {0} год", year);
                        break;
                    }
                case 1:
                    {
                        caption = string.Format("Факт за {0} {1} {2} года", monthNum,
                                          CRHelper.RusManyMonthGenitive(monthNum), year);
                        break;
                    }
                case 2:
                    {
                        caption =
                            string.Format("Факт за {0} {1} {2} года", monthNum,
                                          CRHelper.RusManyMonthGenitive(monthNum), year - 1);
                        break;
                    }
                case 3:
                    {
                        caption = string.Format("Факт за {0} год", year - 1);
                        break;
                    }
            }

            e.HeaderText = caption;
        }

        #endregion
        
        #region Экспорт в Pdf

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";

            foreach (HeaderBase header in UltraWebGrid.Bands[0].HeaderLayout)
            {
                header.Caption = header.Caption.Replace("<br />", " ");
            }

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

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                int widthColumn = 25;

                switch (i)
                {
                    case 1:
                        {
                            widthColumn = 52;
                            break;
                        }
                    case 2:
                    case 4:
                    case 6:
                    case 8:
                        {
                            widthColumn = 100;
                            break;
                        }

                    case 3:
                    case 5:
                    case 7:
                    case 9:
                        {
                            widthColumn = 100;
                            break;
                        }
                }
                e.Layout.Bands[0].Columns[i].Width = widthColumn;
            }
        }

        private void PdfExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs e)
        {
            UltraChart.Width = 1250;
            UltraChart.Height = 600;
            UltraChart.Legend.SpanPercentage = 40;
            Infragistics.Documents.Reports.Graphics.Image img1 = UltraGridExporter.GetImageFromChart(UltraChart);
            e.Section.AddImage(img1);
        }

        #endregion
    }
}