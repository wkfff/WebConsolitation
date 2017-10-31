using System;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web.UI.WebControls;
using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Infragistics.Documents.Reports.Report;

namespace Krista.FM.Server.Dashboards.reports.FK_0001_0001
{
    public partial class DefaultCompare : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid = new DataTable();
        private DataTable dtChart1 = new DataTable();
        private DataTable dtChart2 = new DataTable();
        private DataTable dtRankRF = new DataTable();
        private DataTable dtRankFO = new DataTable();
        private int firstYear = 2000;
        private int endYear = 2011;
        private string month = "Январь";
        private GridHeaderLayout headerLayout;
        private CustomParam subj;
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);
            subj = UserParams.CustomParam("subj");
            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 15);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.45 - 180);
            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);
            UltraWebGrid.ActiveRowChange += new ActiveRowChangeEventHandler(UltraWebGrid_ActiveRowChange);

            UltraChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.5 - 15);
            UltraChart1.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.55 - 120);

            UltraChart2.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.50 - 15);
            UltraChart2.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.55 - 120);

            #region Настройка диаграмм

            UltraChart1.ChartType = ChartType.StackColumnChart;
            UltraChart1.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart1.Axis.Y.Labels.Visible = false;
            UltraChart1.Axis.X.Extent = 50;
            UltraChart1.Axis.Y.Extent = 10;
            UltraChart1.Tooltips.FormatString = "<ITEM_LABEL> <DATA_VALUE:P2>";
            CRHelper.FillCustomColorModel(UltraChart1, 11, false);
            UltraChart1.InvalidDataReceived += new Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

            UltraChart2.ChartType = ChartType.DoughnutChart;
            UltraChart2.Tooltips.FormatString = "<ITEM_LABEL> <DATA_VALUE:P2>";
            UltraChart2.DoughnutChart.Concentric = true;
            UltraChart2.DoughnutChart.OthersCategoryPercent = 0;
            UltraChart2.DoughnutChart.ShowConcentricLegend = false;
            UltraChart2.Data.SwapRowsAndColumns = true;
            UltraChart2.Legend.Visible = true;
            UltraChart2.Legend.Location = LegendLocation.Left;
            UltraChart2.Legend.SpanPercentage = 38;
            UltraChart2.Legend.Margins.Bottom = 0;
            UltraChart2.Legend.Margins.Top = 0;
            UltraChart2.Legend.Margins.Left = 0;
            UltraChart2.Legend.Margins.Right = 0;
            CRHelper.CopyCustomColorModel(UltraChart1, UltraChart2);
            UltraChart2.InvalidDataReceived += new Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

            CalloutAnnotation planAnnotation = new CalloutAnnotation();
            planAnnotation.Text = "Исполнено";
            planAnnotation.Width = 80;
            planAnnotation.Height = 20;
            planAnnotation.Location.Type = LocationType.Percentage;
            planAnnotation.Location.LocationX = 69;
            planAnnotation.Location.LocationY = 15;

            CalloutAnnotation factAnnotation = new CalloutAnnotation();
            factAnnotation.Text = "Назначено";
            factAnnotation.Width = 80;
            factAnnotation.Height = 20;
            factAnnotation.Location.Type = LocationType.Percentage;
            factAnnotation.Location.LocationX = 69;
            factAnnotation.Location.LocationY = 70;

            UltraChart2.Annotations.Add(planAnnotation);
            UltraChart2.Annotations.Add(factAnnotation);

            #endregion

            GridSearch1.LinkedGridId = this.UltraWebGrid.ClientID;

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (!Page.IsPostBack)
            {
                chartWebAsyncPanel.AddRefreshTarget(lbFO);
                chartWebAsyncPanel.AddRefreshTarget(lbSubject);
                chartWebAsyncPanel.AddRefreshTarget(lbFOSub);
                chartWebAsyncPanel.AddRefreshTarget(lbSubjectSub);
                chartWebAsyncPanel.AddRefreshTarget(UltraChart1);
                chartWebAsyncPanel.AddRefreshTarget(UltraChart2);
                chartWebAsyncPanel.AddLinkedRequestTrigger(UltraWebGrid);

                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FK_0001_0001_date");
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                month = dtDate.Rows[0][3].ToString();

                UserParams.PeriodYear.Value = endYear.ToString();
                UserParams.PeriodMonth.Value = month;
                UserParams.Filter.Value = "Все федеральные округа";

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(UserParams.PeriodYear.Value, true);

                ComboMonth.Title = "Месяц";
                ComboMonth.Width = 130;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.SetСheckedState(UserParams.PeriodMonth.Value, true);

                ComboFO.Title = "Федеральный округ";
                ComboFO.Width = 410;
                ComboFO.MultiSelect = false;
                ComboFO.FillDictionaryValues(CustomMultiComboDataHelper.FillFONames(RegionsNamingHelper.FoNames));
                ComboFO.SetСheckedState(UserParams.Filter.Value, true);

                ComboSKIFLevel.Width = 250;
                ComboSKIFLevel.ParentSelect = true;
                ComboSKIFLevel.Title = "Уровень бюджета";
                ComboSKIFLevel.FillDictionaryValues(CustomMultiComboDataHelper.FillSKIFLevels());
                ComboSKIFLevel.SetСheckedState("Консолидированный бюджет субъекта", true);

                lbSubject.Text = string.Empty;
                lbFO.Text = string.Empty;
                lbSubjectSub.Text = string.Empty;
                lbFOSub.Text = string.Empty;
            }

            Page.Title = string.Format("Структура расходов ({0})", ComboFO.SelectedIndex == 0 ? "РФ" :
                RegionsNamingHelper.ShortName(ComboFO.SelectedValue));
            Label1.Text = Page.Title;

            int monthNum = ComboMonth.SelectedIndex + 1;
            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);
            Label2.Text = string.Format("Сравнение структуры расходов субъектов РФ ({3}) за {0} {1} {2} года по разделам классификации расходов",
                monthNum, CRHelper.RusManyMonthGenitive(monthNum), yearNum, ComboSKIFLevel.SelectedValue);


            if (!chartWebAsyncPanel.IsAsyncPostBack)
            {
            UserParams.PeriodMonth.Value = ComboMonth.SelectedValue;
            UserParams.PeriodYear.Value = ComboYear.SelectedValue;
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.Subject.Value = string.Format("{0}].[{1}", UserParams.Region.Value, UserParams.StateArea.Value);
            UserParams.SKIFLevel.Value = ComboSKIFLevel.SelectedValue;

            UltraWebGrid.Bands.Clear();
            headerLayout = new GridHeaderLayout(UltraWebGrid);
            UltraWebGrid.DataBind();

            string patternValue = lbSubject.Text;
            int defaultRowIndex = 1;
            if (patternValue == string.Empty)
            {
                patternValue = UserParams.StateArea.Value;
                defaultRowIndex = 0;
            }

                UltraGridRow row = CRHelper.FindGridRow(UltraWebGrid, patternValue, 0, defaultRowIndex);
                ActivateGridRow(row);
            }


#warning метод databind() иногда вызывается 2 раза

            if (UltraChart1.DataSource == null)
            {
                UltraChart1.DataBind();
            }

            if (UltraChart2.DataSource == null)
            {
                UltraChart2.DataBind();
            }
        }

        #region Обработчики грида

        /// <summary>
        /// Активация строки грида
        /// </summary>
        /// <param name="row">строка</param>
        private void ActivateGridRow(UltraGridRow row)
        {
            if (row == null)
                return;

            string subject = row.Cells[0].Text;
            lbSubject.Text = subject;
            
            if (RegionsNamingHelper.IsRF(subject))
            {
                UserParams.Subject.Value = "]";
                UserParams.SubjectFO.Value = "]";
            }
            else if (RegionsNamingHelper.IsFO(subject))
            {
                UserParams.Region.Value = subject;
                UserParams.Subject.Value = string.Format("].[{0}]", UserParams.Region.Value);
                UserParams.SubjectFO.Value = string.Format("].[{0}]", UserParams.Region.Value);
                CRHelper.SaveToErrorLog(subject+"_1");
            }
            else
            {
                UserParams.Region.Value = RegionsNamingHelper.FullName(row.Cells[1].Text);
                UserParams.StateArea.Value = subject;
                UserParams.Subject.Value = string.Format("].[{0}].[{1}]", UserParams.Region.Value, UserParams.StateArea.Value);
                UserParams.SubjectFO.Value = string.Format("].[{0}]", UserParams.Region.Value);
                CRHelper.SaveToErrorLog(subject + "_2");
            }

            lbSubjectSub.Text = "Сравнение плановой и фактической структуры";
            lbFOSub.Text = string.Empty;

            UltraChart1.DataBind();
            UltraChart2.DataBind();
        }

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FK_0001_0001_compare_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Субъект", dtGrid);

            query = DataProvider.GetQueryText("FK_0001_0001_compare_rankRF");
            dtRankRF = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtRankRF);

            query = DataProvider.GetQueryText("FK_0001_0001_compare_grid_rankFO");
            dtRankFO = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtRankFO);

            if (dtGrid.Columns.Count > 2)
            {
                dtGrid.Columns[1].ColumnName = "ФО";
            }

            UserParams.Filter.Value = ComboFO.SelectedValue;
            if (ComboFO.SelectedIndex != 0)
            {
                UltraWebGrid.DataSource = CRHelper.SetDataTableFilter(dtGrid, "ФО", RegionsNamingHelper.ShortName(ComboFO.SelectedValue));
            }
            else
            {
                UltraWebGrid.DataSource = dtGrid;
            }
        }

        void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
            if (ComboFO.SelectedIndex != 0)
            {
                UltraWebGrid.Height = Unit.Empty;
            }
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;

            for (int i = 2; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                string formatString = "N0";
                int widthColumn = 60;

                int j = (i - 1) % 4;
                switch (j)
                {
                    case 1:
                        {
                            formatString = "N3";
                            widthColumn = 100;
                            break;
                        }
                    case 2:
                        {
                            formatString = "P2";
                            widthColumn = 55;
                            break;
                        }
                }

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = widthColumn;
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            e.Layout.Bands[0].Columns[0].Width = 250;
            e.Layout.Bands[0].Columns[1].Width = 45;

            headerLayout.AddCell("Субъект");
            headerLayout.AddCell("ФО");
            for (int i = 2; i < e.Layout.Bands[0].Columns.Count; i = i + 4)
            {
                string[] captions = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';');
                GridHeaderCell cell = headerLayout.AddCell(captions[0]);
                string rankFOCaption;
                string rankFOHint;
                rankFOCaption = "Ранг ФО";
                rankFOHint = "Место субъекта(округа) по доле расходов среди всех субъектов его федерального округа";
                cell.AddCell("Исполнено, млн.руб.", "Фактическое исполнение по разделу расходов");
                cell.AddCell("Доля", "Доля раздела расхода в общей сумме расходов");
                cell.AddCell(rankFOCaption, rankFOHint);
                cell.AddCell("Ранг РФ", "Место субъекта по доле расходов среди всех субъектов РФ");
            }
            headerLayout.ApplyHeaderInfo();
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {


            if (e.Row.Index == 0)
            {
                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    if (i > 3 & (i - 4) % 4 == 0)
                    {
                        e.Row.Cells[i].Value = string.Empty;
                        e.Row.Cells[i + 1].Value = string.Empty;
                    }
                    e.Row.Cells[i].Style.Font.Bold = true;
                }
                return;
            }


            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                int k = (i - 2) % 4;
                bool isFO = false;
                if (e.Row.Cells[0].Value != null && e.Row.Cells[0].Value.ToString() != string.Empty)
                {
                    if (!RegionsNamingHelper.IsSubject(e.Row.Cells[0].Value.ToString()))
                    {
                        isFO = true;
                        foreach (UltraGridCell cell in e.Row.Cells)
                        {
                            cell.Style.Font.Bold = true;
                        }
                    }
                }

                bool rankFO = (k == 2);
                bool rankRF = (k == 3);
                bool param;
                if (rankFO)
                {
                    param = Check1(e.Row, i);
                    string css = GetImg(e.Row, i, param);
                    string region = isFO ? "РФ" : "федеральном округе";
                    e.Row.Cells[i].Style.BackgroundImage = css;
                    if (css != string.Empty)
                    {
                        e.Row.Cells[i].Title = (css == "~/images/starGrayBB.png")
                           ? string.Format("Самая низкая доля расходов в {0}", region)
                           : string.Format("Самая высокая доля расходов в {0}", region);
                    }
                }
                else if (rankRF)
                {
                    param = Check2(e.Row, i);
                    string css = GetImg(e.Row, i, param);
                    e.Row.Cells[i].Style.BackgroundImage = css;
                    if (css != string.Empty)
                    {
                        e.Row.Cells[i].Title = (css == "~/images/starGrayBB.png")
                           ? "Самая низкая доля расходов в РФ"
                           : "Самая высокая доля расходов в РФ";
                    }
                }
                e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
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

        private static string GetImg(UltraGridRow row, int i, bool param)
        {
            if (param)
            {
                return "~/images/starGrayBB.png";
            }
            else if (Convert.ToInt32(row.Cells[i].Value) == 1)
            {
                return "~/images/starYellowBB.png";
            }
            return string.Empty;
        }

        private bool Check2(UltraGridRow row, int i)
        {
            return (dtRankRF.Rows.Count > 0 && dtRankRF.Rows[0][i / 5] != DBNull.Value && dtRankRF.Rows[0][i / 5].ToString() != string.Empty) &&
                Convert.ToInt32(row.Cells[i].Value) == Convert.ToInt32(dtRankRF.Rows[0][i / 5]);
        }

        private bool Check1(UltraGridRow row, int i)
        {
            /* return row.Cells[i] != null && row.Cells[i + 1] != null &&
                    Convert.ToInt32(row.Cells[i].Value) == Convert.ToInt32(row.Cells[i + 1].Value) &&
                    Convert.ToInt32(row.Cells[i].Value) != 0;*/
            return (dtRankFO.Rows.Count > 0 &&
                dtRankFO.Rows[row.Index][i / 4] != null &&
                    dtRankFO.Rows[row.Index][i / 4].ToString() != string.Empty) &&
                    Convert.ToInt32(row.Cells[i].Value) == Convert.ToInt32(dtRankFO.Rows[row.Index][i / 4]);
        }

        protected void UltraWebGrid_ActiveRowChange(object sender, RowEventArgs e)
        {
            ActivateGridRow(e.Row);
        }

        #endregion

        #region Обработчики диаграмм

        protected void UltraChart1_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FK_0001_0001_compare_chart1");
            dtChart1 = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Район", dtChart1);
            RegionsNamingHelper.ReplaceRegionNames(dtChart1, 0);

            if (dtChart1.Rows.Count > 0)
            {
                dtChart1.Rows[0][0] = "РФ";
            }

            foreach (DataColumn column in dtChart1.Columns)
            {
                column.ColumnName = column.ColumnName.TrimEnd('_');
            }

            UltraChart1.DataSource = dtChart1;
        }

        protected void UltraChart2_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FK_0001_0001_compare_chart2");
            dtChart2 = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Район", dtChart2);
            RegionsNamingHelper.ReplaceRegionNames(dtChart2, 2);

            foreach (DataColumn column in dtChart2.Columns)
            {
                column.ColumnName = column.ColumnName.TrimEnd('_');
            }

            UltraChart2.DataSource = dtChart2;
        }

        #endregion

        private Bitmap GetImageChart()
        {
            int picWidth = (int)(UltraChart1.Width.Value + 20 + UltraChart2.Width.Value-100);
            int picHeight = (int)(UltraChart2.Height.Value);
            Bitmap bmNewImg = new Bitmap(picWidth, picHeight);

            MemoryStream imageStream = new MemoryStream();

            UltraChart2.SaveTo(imageStream, ImageFormat.Png);
            Bitmap bm1 = new Bitmap(imageStream);


            imageStream = new MemoryStream();
            UltraChart1.Width = CRHelper.GetChartWidth(UltraChart1.Width.Value - 100);
            UltraChart1.SaveTo(imageStream, ImageFormat.Png);
            Bitmap bm2 = new Bitmap(imageStream);

            Graphics g = Graphics.FromImage(bmNewImg);
            g.DrawImage(bm1, 0, 0, (int)UltraChart2.Width.Value, (int)UltraChart2.Height.Value);
            g.DrawImage(bm2, (int)UltraChart2.Width.Value, 0, (int)UltraChart1.Width.Value,
                        (int)UltraChart1.Height.Value);

            g.Dispose();
            return bmNewImg;
        }
        #region Export excel
        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            Worksheet sheet2 = workbook.Worksheets.Add("Диаграмма");
            sheet1.Rows[0].Cells[0].Value = Label1.Text + " " + Label2.Text;
            sheet2.Rows[0].Cells[0].Value = Label1.Text + " " + Label2.Text;
            sheet2.Rows[1].Cells[0].Value = lbSubject.Text + " " + lbSubjectSub.Text;
            ReportExcelExporter1.HeaderCellHeight = 70;
            ReportExcelExporter1.GridColumnWidthScale = 1.3;
            ReportExcelExporter1.Export(headerLayout, sheet1, 3);
            ReportExcelExporter1.Export(GetImageChart(), sheet2, 3);

        }
        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            Report report = new Report();
            
            ISection section1 = report.AddSection();
            ReportPDFExporter1.HeaderCellHeight = 70;
            ReportPDFExporter1.Export(headerLayout, Label1.Text + " " + Label2.Text, section1);
            
            ISection section2 = report.AddSection();
            IText title = section2.AddText();
            Font font = new Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(Label1.Text + " " + Label2.Text);

            ReportPDFExporter1.Export(GetImageChart(), lbSubject.Text + " " + lbSubjectSub.Text, section2);
        }



        #endregion

    }
}