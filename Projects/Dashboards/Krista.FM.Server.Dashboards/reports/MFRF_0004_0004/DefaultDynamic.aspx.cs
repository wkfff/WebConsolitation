using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.MFRF_0004_0004
{
    public partial class DefaultDynamic : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid = new DataTable();
        private DataTable dtChart = new DataTable();
        private int firstYear = 2008;
        private int endYear = 2009;
        private string month = "Январь";
        private int yearNum = 2009;

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 12);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.5 - 170);

            UltraChart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 25);
            UltraChart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.5 - 115);

            #region Настройка диаграммы

            UltraChart.ChartType = ChartType.SplineChart;
            UltraChart.BorderWidth = 0;

            UltraChart.Axis.X.Extent = 42;
            UltraChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            UltraChart.Axis.Y.Extent = 60;

            UltraChart.Axis.X.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.Auto;

            UltraChart.Legend.Visible = true;
            UltraChart.Legend.Location = LegendLocation.Right;
            UltraChart.Legend.SpanPercentage = 15;
            UltraChart.Legend.Margins.Bottom = Convert.ToInt32(UltraChart.Height.Value) / 2;

            UltraChart.Tooltips.FormatString = "<ITEM_LABEL> <SERIES_LABEL>\n<DATA_VALUE:N3> млн.руб.";

            LineAppearance lineAppearance = new LineAppearance();
            lineAppearance.IconAppearance.Icon = SymbolIcon.Circle;
            lineAppearance.IconAppearance.IconSize = SymbolIconSize.Small;
            lineAppearance.Thickness = 3;
            lineAppearance.SplineTension = (float)0.3;
            UltraChart.SplineChart.LineAppearances.Add(lineAppearance);

            EmptyAppearance emptyAppearance = new EmptyAppearance();
            emptyAppearance.EnablePoint = true;
            emptyAppearance.EnablePE = true;
            emptyAppearance.EnableLineStyle = true;
            emptyAppearance.PointStyle.Icon = SymbolIcon.Circle;
            emptyAppearance.PointStyle.IconSize = SymbolIconSize.Large;
            emptyAppearance.LineStyle.MidPointAnchors = true;
            UltraChart.SplineChart.EmptyStyles.Add(emptyAppearance);

            UltraChart.SplineChart.NullHandling = NullHandling.DontPlot;
            UltraChart.Data.EmptyStyle.ShowInLegend = false;

            UltraChart.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);

            #endregion

            GridSearch1.LinkedGridId = UltraWebGrid.ClientID;

            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);

            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler
                    <Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);
            UltraGridExporter1.PdfExporter.EndExport += new EventHandler
                <Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs>(PdfExporter_EndExport);

            CrossLink1.Text = "Исполнение&nbsp;расходов&nbsp;ГРБС";
            CrossLink1.NavigateUrl = "~/reports/MFRF_0004_0003/DefaultValuation.aspx";
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (!Page.IsPostBack)
            {
                chartWebAsyncPanel.AddRefreshTarget(lbSubject);
                chartWebAsyncPanel.AddRefreshTarget(lbSubjectSub);
                chartWebAsyncPanel.AddRefreshTarget(UltraChart);
                chartWebAsyncPanel.AddLinkedRequestTrigger(useStack.ClientID);
                chartWebAsyncPanel.AddLinkedRequestTrigger(UltraWebGrid);

                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("MFRF_0004_0004_date");
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

                UserParams.Filter.Value = "[МФ РФ].[ГРБС ФБ Сопоставимый].[Все]";

                lbSubject.Text = "Всего по ГРБС";
                lbSubjectSub.Text = string.Empty;
            }

            Label1.Text = "Динамика исполнения расходов ГРБС";
            Page.Title = Label1.Text;

            int monthNum = ComboMonth.SelectedIndex + 1;
            yearNum = Convert.ToInt32(ComboYear.SelectedValue);
            Label2.Text = string.Format("Сравнение темпов роста фактических расходов ГРБС за {0} {1} {2} года", monthNum, CRHelper.RusManyMonthGenitive(monthNum), yearNum);
            string period = (useStack.Checked) ? "(с начала года)" : "(за месяц)";
            lbSubjectSub.Text = string.Format("Динамика исполнения бюджетных ассигнований за {0}-{1} годы {2}, млн.руб.", yearNum - 1, yearNum, period);

            // в случае асинхронного постбэка ничего не делаем
            if (!chartWebAsyncPanel.IsAsyncPostBack)
            {
                UserParams.PeriodMonth.Value = ComboMonth.SelectedValue;
                UserParams.PeriodYear.Value = yearNum.ToString();
                UserParams.PeriodLastYear.Value = (yearNum - 1).ToString();
                UserParams.PeriodHalfYear.Value =
                    string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(ComboMonth.SelectedIndex + 1));
                UserParams.PeriodQuater.Value =
                    string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(ComboMonth.SelectedIndex + 1));

                // биндим грид с новыми параметрами
                UltraWebGrid.DataBind();

                string patternValue = DataDictionariesHelper.GetFullGRBSName(lbSubject.Text);
                int defaultRowIndex = 1;
                if (patternValue == string.Empty)
                {
                    defaultRowIndex = 0;
                }

                // ищем строку
                UltraGridRow row = CRHelper.FindGridRow(UltraWebGrid, patternValue, 0, defaultRowIndex);
                // выделяем строку
                ActiveGridRow(row);
            }

            UserParams.FKRFilter.Value = (!useStack.Checked) ? "Факт за период" : "Факт";
            UltraChart.DataBind();

        }

        #region Обработчики грида

        /// <summary>
        /// Активация строки грида
        /// </summary>
        /// <param name="row"></param>
        private void ActiveGridRow(UltraGridRow row)
        {
            if (row == null)
                return;

            string subject = row.Cells[0].Text;
            lbSubject.Text = DataDictionariesHelper.GetShortGRBSName(subject);

            //subject = subject.Replace("\"", "'");

            if (subject == "Всего по ГРБС")
            {
                UserParams.Filter.Value = "[МФ РФ].[ГРБС ФБ Сопоставимый].[Все]";
            }
            else
            {
                UserParams.Filter.Value = string.Format("[МФ РФ].[ГРБС ФБ Сопоставимый].[Все].[{0}]", subject);
            }

            UltraChart.DataBind();
        }

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("MFRF_0004_0004_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, " ", dtGrid);

            foreach (DataRow row in dtGrid.Rows)
            {
                for (int i = 0; i < row.ItemArray.Length; i++)
                {
                    if (row[i] != DBNull.Value && (i == 2 || i == 3 || i == 5 || i == 6))
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

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(425);

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                string formatString = "N0";
                int widthColumn = 25;

                switch (i)
                {
                    case 1:
                        {
                            formatString = "N0";
                            widthColumn = 45;
                            break;
                        }
                    case 2:
                    case 3:
                    case 5:
                    case 6:
                        {
                            formatString = "N3";
                            widthColumn = 99;
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
                }

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(widthColumn);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[i].Header.Style.Wrap = true;
            }

            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 0, "Наименование ГРБС ФБ", "");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 1, "Код", "");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 2, "Кассовое исполнение, млн.руб.",
                string.Format("Кассовое исполнение расходов ГРБС за {0} {1} {2} года", ComboMonth.SelectedIndex + 1, CRHelper.RusManyMonthGenitive(ComboMonth.SelectedIndex + 1), endYear));
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 3, "Кассовое исполнение прошлый год, млн.руб.",
                string.Format("Кассовое исполнение расходов ГРБС за {0} {1} {2} года", ComboMonth.SelectedIndex + 1, CRHelper.RusManyMonthGenitive(ComboMonth.SelectedIndex + 1), endYear - 1));
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 4, "Фактический темп роста к прошлому году", "Темп роста фактических расходов в сравнении с прошлым годом");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 5, "Бюджетные ассигнования на текущий год, млн.руб.", "Бюджетные ассигнования расходов на текущий год");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 6, "Бюджетные ассигнования прошлый год, млн.руб.", "Бюджетные ассигнования расходов в прошлом году");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 7, "Плановый темп роста к прошлому году", "Темп роста бюджетных ассигнований в сравнении с прошлым годом");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 8, "Процент исполнения плана",
                string.Format("Исполнение бюджетных ассигнований  за {0} {1} {2} года", ComboMonth.SelectedIndex + 1, CRHelper.RusManyMonthGenitive(ComboMonth.SelectedIndex + 1), endYear));
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                bool rateFactColumn = (i == 4);
                bool ratePlanColumn = (i == 7);

                if (e.Row.Cells[0].Value != null && e.Row.Cells[0].Value.ToString() == "Всего по ГРБС")
                {
                    e.Row.Cells[i].Style.Font.Bold = true;
                    if (i == 1)
                    {
                        e.Row.Cells[i].Value = string.Empty;
                    }
                }

                if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty && (rateFactColumn || ratePlanColumn))
                {
                    double rate = Convert.ToDouble(e.Row.Cells[i].Value);
                    string hint = string.Empty;

                    if (rate > 1)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                        hint = "Рост к прошлому году";
                    }
                    else
                    {
                        if (rate < 1)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                            hint = "Падение к прошлому году";
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

        protected void UltraWebGrid_ActiveRowChange(object sender, RowEventArgs e)
        {
            ActiveGridRow(e.Row);
        }

        #endregion

        #region Обработчики диаграмм

        protected void UltraChart1_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("MFRF_0004_0004_chart");
            dtChart = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Месяц", dtChart);

            // делаем отдельные дататейблы
            DataTable dtChart1 = new DataTable();
            DataColumn column1 = new DataColumn("Месяц", typeof (string));
            dtChart1.Columns.Add(column1);
            DataColumn column2 = new DataColumn(string.Format("{0} г. (Заработная плата)", yearNum - 1), typeof(double));
            dtChart1.Columns.Add(column2);
            DataColumn column3 = new DataColumn(string.Format("{0} г.", yearNum - 1), typeof(double));
            dtChart1.Columns.Add(column3);
            DataColumn column4 = new DataColumn(string.Format("{0} г. (Заработная плата)", yearNum), typeof(double));
            dtChart1.Columns.Add(column4);
            DataColumn column5 = new DataColumn(string.Format("{0} г.", yearNum), typeof(double));
            dtChart1.Columns.Add(column5);

            for (int i = 0; i < dtChart.Rows.Count; i++)
            {
                DataRow row;
                int index1;
                int index2;
                if (i <= 11)
                {
                    row = dtChart1.NewRow();
                    dtChart1.Rows.Add(row);
                    index1 = 1;
                    index2 = 2;
                }
                else
                {
                    row = dtChart1.Rows[i - 12];
                    index1 = 3;
                    index2 = 4;
                }

                if (dtChart.Rows[i][0] != DBNull.Value)
                {
                    row[0] = dtChart.Rows[i][0];
                }

                if (dtChart.Rows[i][1] != DBNull.Value && dtChart.Rows[i][1].ToString() != string.Empty)
                {
                    row[index1] = Convert.ToDouble(dtChart.Rows[i][1]);
                }

                if (dtChart.Rows[i][2] != DBNull.Value && dtChart.Rows[i][2].ToString() != string.Empty)
                {
                    row[index2] = Convert.ToDouble(dtChart.Rows[i][2]);
                }
            }

            UltraChart.Series.Clear();
            for (int i = 1; i < dtChart1.Columns.Count; i++)
            {
                bool emptySerie = true;
                foreach (DataRow row in dtChart1.Rows)
                {
                    if (row[i] != DBNull.Value)
                    {
                        emptySerie = false;
                        break;
                    }
                }
                if (emptySerie)
                {
                    continue;
                }

                NumericSeries series = CRHelper.GetNumericSeries(i, dtChart1);
                UltraChart.Series.Add(series);
            }
        }

        void UltraChart_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {

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
                    case 3:
                    case 5:
                    case 6:
                        {
                            formatString = "#,##0.000;[Red]-#,##0.000";
                            widthColumn = 95;
                            break;
                        }
                    case 4:
                    case 7:
                    case 8:
                        {
                            formatString = "0.00%";
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

        #endregion

        #region Экспорт в Pdf

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";

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
                            widthColumn = 55;
                            break;
                        }
                    case 2:
                    case 3:
                    case 5:
                    case 6:
                        {
                            widthColumn = 109;
                            break;
                        }
                    case 4:
                    case 7:
                    case 8:
                        {
                            widthColumn = 90;
                            break;
                        }
                }
                
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(widthColumn);
            }
        }

        private void PdfExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs e)
        {
            e.Section.AddPageBreak();
            IText title = e.Section.AddText();
            Font font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(lbSubject.Text);

            title = e.Section.AddText();
            font = new Font("Verdana", 12);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(lbSubjectSub.Text);

            UltraChart.Width = 1200;
            Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromChart(UltraChart);
            e.Section.AddImage(img);
        }

        #endregion
    }
}
